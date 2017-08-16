namespace www
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class confdigitacao : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                //base.ExibirEstipulantes(cboEstipulante, false, true);
                base.ExibirOperadoras(cboOperadoras, false);

                this.CarregaContratosADM();
            }
        }

        void CarregaContratosADM()
        {
            lstContratos.SelectedIndex = -1;
            lstContratos.Items.Clear();
            lstContratos.DataValueField = "ID";
            lstContratos.DataTextField  = "Descricao";
            lstContratos.DataSource = ContratoADM.Carregar(cboOperadoras.SelectedValue, true);
            lstContratos.DataBind();
        }

        protected void cboEstipulante_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratosADM();
        }

        protected void cboOperadoras_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratosADM();
        }

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            if (lstContratos.Items.Count == 0)
            {
                base.Alerta(null, this, "_contradm", "Nenhum contrato administrativo informado.");
                return;
            }

            DateTime data = base.CStringToDateTime(txtVigencia.Text);
            if (data == DateTime.MinValue)
            {
                base.Alerta(null, this, "_data", "Data de vigência inválida.");
                return;
            }

            String[] contratoAdmIds = base.PegaIDsSelecionados(lstContratos);
            if (contratoAdmIds == null || contratoAdmIds.Length == 0)
            {
                base.Alerta(null, this, "_contradm", "Nenhum contrato administrativo informado.");
                return;
            }

            DataTable dt = ContratoFacade.Instance.RelatorioConferenciaDigitacao(contratoAdmIds, data);

            Int32 vidas = 0;
            if (dt.Rows.Count > 0) { vidas = Convert.ToInt32(dt.Compute("SUM(QtdVidas)", "")); }
            litSumario.Text = String.Concat("<b>Propostas:</b> ", dt.Rows.Count, " - <b>Vidas: </b>", vidas);

            grid.DataSource = dt;
            grid.DataBind();
        }
    }
}
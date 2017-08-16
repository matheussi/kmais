namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class contratoadm : PageBase
    {
        Object contratoId
        {
            get
            {
                if (Request[IDKey] != null) { return Request[IDKey]; }
                else { return ViewState[IDKey]; }
            }
            set { ViewState[IDKey] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadoras, false);
                base.ExibirEstipulantes(cboEstipulante, false, true);

                if (base.IDKeyParameterInProcess(ViewState, "_contrtADM"))
                {
                    this.CarregaContrato();
                }
            }
        }

        void CarregaContrato()
        {
            ContratoADM contrato = new ContratoADM();
            contrato.ID = contratoId;
            contrato.Carregar();
            cboOperadoras.SelectedValue = Convert.ToString(contrato.OperadoraID);
            cboEstipulante.SelectedValue = Convert.ToString(contrato.EstipulanteID);
            txtDescricao.Text = contrato.Descricao;
            txtNumero.Text = contrato.Numero;
            chkAtiva.Checked = contrato.Ativo;
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/contratosadm.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (cboEstipulante.Items.Count == 0)
            {
                base.Alerta(null, this, "__err0", "Não há um estipulante disponível.");
                cboEstipulante.Focus();
                return;
            }
            if (cboOperadoras.Items.Count == 0)
            {
                base.Alerta(null, this, "__err1", "Não há uma operadora disponível.");
                cboOperadoras.Focus();
                return;
            }
            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err2", "Informe a descrição.");
                txtDescricao.Focus();
                return;
            }
            #endregion

            ContratoADM contrato = new ContratoADM();
            contrato.ID = contratoId;

            if (contrato.ID != null) { contrato.Carregar(); }

            contrato.Descricao = txtDescricao.Text.ToUpper();
            contrato.Numero = txtNumero.Text;
            contrato.OperadoraID = cboOperadoras.SelectedValue;
            contrato.EstipulanteID = cboEstipulante.SelectedValue;
            contrato.Ativo = chkAtiva.Checked;
            contrato.Salvar();
            ViewState[IDKey] = contrato.ID;

            base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
        }
    }
}
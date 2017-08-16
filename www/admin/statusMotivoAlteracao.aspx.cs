namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class statusMotivoAlteracao : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                ContratoStatus.UI.FillComboWithTypes(cboTipo);
                if (base.IDKeyParameterInProcess(ViewState, "_"))
                {
                    this.CarregaMotivo();
                }
            }
        }

        void CarregaMotivo()
        {
            ContratoStatus motivo = new ContratoStatus(ViewState[IDKey]);
            motivo.Carregar();

            txtDescricao.Text = motivo.Descricao;
            cboTipo.SelectedValue = motivo.Tipo.ToString();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/statusMotivosAlteracao.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err", "Informe a descrição.");
                txtDescricao.Focus();
                return;
            }

            ContratoStatus motivo = new ContratoStatus();
            motivo.ID = ViewState[IDKey];
            motivo.Descricao = txtDescricao.Text;
            motivo.Tipo = Convert.ToInt32(cboTipo.SelectedValue);
            motivo.Salvar();
            ViewState[IDKey] = motivo.ID;

            base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
        }
    }
}

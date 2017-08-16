namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class cobrancaMotivoBaixa : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                if (base.IDKeyParameterInProcess(ViewState, "_"))
                {
                    this.CarregaMotivo();
                }
            }
        }

        void CarregaMotivo()
        {
            CobrancaMotivoBaixa motivo = new CobrancaMotivoBaixa(ViewState[IDKey]);
            motivo.Carregar();

            txtCodigo.Text = motivo.Codigo;
            txtDescricao.Text = motivo.Descricao;
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/cobrancaMotivosBaixa.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err", "Informe a descrição.");
                txtDescricao.Focus();
                return;
            }

            CobrancaMotivoBaixa motivo = new CobrancaMotivoBaixa();
            motivo.ID        = ViewState[IDKey];
            motivo.Descricao = txtDescricao.Text;
            motivo.Codigo    = txtCodigo.Text;
            motivo.Salvar();
            ViewState[IDKey] = motivo.ID;

            base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
        }
    }
}

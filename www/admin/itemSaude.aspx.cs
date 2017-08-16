namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class itemSaude : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, true);
                cboOperadora.Items[0].Text = "<TODAS>";

                if (base.IDKeyParameterInProcess(ViewState, "_item"))
                {
                    this.CarregaItem();
                }
            }
        }

        void CarregaItem()
        {
            ConferenciaItemSaude item = new ConferenciaItemSaude(ViewState[IDKey]);
            item.Carregar();

            txtDescricao.Text = item.Descricao;
            if (item.OperadoraID != null)
                cboOperadora.SelectedValue = Convert.ToString(item.OperadoraID);
            else
                cboOperadora.SelectedIndex = 0;
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/itensSaude.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err", "Informe uma descrição.");
                txtDescricao.Focus();
                return;
            }

            ConferenciaItemSaude item = new ConferenciaItemSaude(ViewState[IDKey]);
            item.Descricao = txtDescricao.Text.ToUpper();
            if (cboOperadora.SelectedIndex > 0)
                item.OperadoraID = cboOperadora.SelectedValue;

            item.Salvar();
            ViewState[IDKey] = item.ID;

            base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
        }
    }
}
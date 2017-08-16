namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class checklistDetalhe : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, true);
                cboOperadora.Items[0].Text = "<TODAS>";

                if (base.IDKeyParameterInProcess(ViewState, "_check"))
                {
                    this.CarregaChecklist();
                }
            }
        }

        void CarregaChecklist()
        {
            CheckList check = new CheckList(ViewState[IDKey]);
            check.Carregar();

            txtDescricao.Text = check.Descricao;
            if (check.OperadoraID != null)
                cboOperadora.SelectedValue = Convert.ToString(check.OperadoraID);
            else
                cboOperadora.SelectedIndex = 0;
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/checklistLista.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err", "Informe uma descrição.");
                txtDescricao.Focus();
                return;
            }

            CheckList check = new CheckList(ViewState[IDKey]);
            check.Descricao = txtDescricao.Text.ToUpper();
            if (cboOperadora.SelectedIndex > 0)
                check.OperadoraID = cboOperadora.SelectedValue;

            check.Salvar();
            ViewState[IDKey] = check.ID;

            base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
        }
    }
}
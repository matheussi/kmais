namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class itemsSaude : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, true);
                cboOperadora.Items[0].Text = "<TODAS>";
                this.CarregaLista();
            }
        }

        void CarregaLista()
        {
            Object operadoraId = null;
            if (cboOperadora.SelectedIndex > 0) { operadoraId = cboOperadora.SelectedValue; }
            grid.DataSource = ConferenciaItemSaude.Carregar(operadoraId);
            grid.DataBind();
        }

        protected void cboOperadora_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaLista();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Response.Redirect(String.Concat("~/admin/itemSaude.aspx?", IDKey, "=", grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0]));
            }
            else if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    ConferenciaItemSaude item = new ConferenciaItemSaude(grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                    item.Remover();
                    grid.PageIndex = 0;
                    this.CarregaLista();
                }
                catch
                {
                    base.Alerta(null, this, "_errExcl", "Não foi possível excluir.");
                }
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja prosseguir com a exclusão?");
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/itemSaude.aspx");
        }
    }
}

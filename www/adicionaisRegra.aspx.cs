namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class adicionaisRegra : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadoras, false);
            }
        }

        void CarregarRegras()
        {
            if (cboOperadoras.Items.Count > 0)
                grid.DataSource = AdicionalRegra.Carregar(cboOperadoras.SelectedValue);
            else
                grid.DataSource = null;

            grid.DataBind();
        }

        protected void cboOperadoras_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregarRegras();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("adicionalRegra.aspx?", IDKey, "=", id));
            }
            else if (e.CommandName == "excluir")
            {
                AdicionalRegra regra = new AdicionalRegra();
                regra.ID = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                regra.Remover();
                this.CarregarRegras();
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente prosseguir com a exclusão?\\nEssa operação não poderá ser desfeita.");
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/adicionalRegra.aspx");
        }
    }
}
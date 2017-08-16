namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class agregadosRegra : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadoras, false);
                this.CarregaContratos();
                this.CarregarRegras();
            }
        }

        void CarregaContratos()
        {
            cboContrato.Items.Clear();
            if (cboOperadoras.Items.Count == 0) { return; }
            cboContrato.DataValueField = "ID";
            cboContrato.DataTextField = "Descricao";
            cboContrato.DataSource = ContratoADM.Carregar(cboOperadoras.SelectedValue);
            cboContrato.DataBind();
        }

        void CarregarRegras()
        {
            if (cboContrato.Items.Count > 0)
                grid.DataSource = AgregadoRegra.Carregar(cboContrato.SelectedValue);
            else
                grid.DataSource = null;
            grid.DataBind();
        }

        protected void cboOperadoras_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
            this.CarregarRegras();
        }

        protected void cboContrato_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregarRegras();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("agregadoRegra.aspx?", IDKey, "=", id));
            }
            else if (e.CommandName == "excluir")
            {
                AgregadoRegra regra = new AgregadoRegra();
                regra.ID = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                regra.Remover();
                this.CarregarRegras();
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja realmente prosseguir com a exclusão?\\nEssa operação não poderá ser desfeita.");
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/agregadoRegra.aspx");
        }
    }
}
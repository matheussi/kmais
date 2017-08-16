namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class categorias : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaCategorias();
            }
        }

        void CarregaCategorias()
        {
            grid.DataSource = Categoria.Carregar_OrdenadoPorPerfil(false);
            grid.DataBind();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("inativar"))
            {
                Categoria categoria = new Categoria();
                categoria.ID = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                categoria.Carregar();
                categoria.Ativo = !categoria.Ativo;
                categoria.Salvar();
                this.CarregaCategorias();
            }
            else if (e.CommandName.Equals("excluir"))
            {
                Categoria categoria = new Categoria();
                categoria.ID = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                try
                {
                    categoria.Remover();
                    this.CarregaCategorias();
                }
                catch
                {
                    base.Alerta(null, this, "_errExcl", "Não foi possível excluir a categoria.");
                }
            }
            else if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect("~/admin/categoria.aspx");
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente prosseguir?");
                base.grid_RowDataBound_Confirmacao(sender, e, 3, "Deseja realmente excluir a categoria?");
                if (!Convert.ToBoolean(grid.DataKeys[e.Row.RowIndex][1]))
                {
                    e.Row.ForeColor = System.Drawing.Color.Red;
                    //((LinkButton)e.Row.Cells[1].Controls[0]).Text = "ativar";
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Text = "<img src='../images/unactive.png' alt='inativo' border='0'>";
                }
                else
                {
                    //((LinkButton)e.Row.Cells[1].Controls[0]).Text = "inativar";
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Text = "<img src='../images/active.png'  alt='ativo' border='0'>";
                }
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/categoria.aspx");
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.CarregaCategorias();
        }
    }
}
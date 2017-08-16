namespace www.financeiro
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

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
            grid.DataSource = CategoriaContaCorrente.CarregarTodas();
            grid.DataBind();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                Int32 indice = Convert.ToInt32(e.CommandArgument);
                Object id = grid.DataKeys[indice].Values[0];
                CategoriaContaCorrente obj = new CategoriaContaCorrente(id);
                obj.Carregar();
                if (obj.Sistema)
                {
                    base.Alerta(null,this, "__errex", "Esta categoria é usada internamente pelo sistema.");
                    return;
                }

                try
                {
                    obj.Remover();
                    this.CarregaCategorias();
                }
                catch
                {
                    base.Alerta(null, this, "__errexUn", "Não foi possível excluir a categoria.");
                }
            }
            else if (e.CommandName.Equals("editar"))
            {
                Int32 indice = Convert.ToInt32(e.CommandArgument);
                Object id = grid.DataKeys[indice].Values[0];
                CategoriaContaCorrente obj = new CategoriaContaCorrente(id);
                obj.Carregar();
                if (obj.Sistema)
                {
                    base.Alerta(null, this, "__erred", "Esta categoria é usada internamente pelo sistema.");
                    return;
                }

                Response.Redirect(String.Format("categoria.aspx?{0}={1}", IDKey, id));
            }
        }
        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente excluir o item?\\nAtenção: esta operação não poderá ser desfeita.");
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("categoria.aspx");
        }
    }
}

namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using LC.Web.PadraoSeguros.Entity;

    public partial class comissionamentosOperadora : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaTabelas();
            }
        }

        void CarregaTabelas()
        {
            gridTabelas.DataSource = ComissionamentoOperadora.CarregarTodos();
            gridTabelas.DataBind();
        }

        protected void gridTabelas_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = gridTabelas.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect("comissionamentoOperadora.aspx?" + IDKey + "=" + Session[IDKey]);
            }
            else if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    ComissionamentoOperadora co = new ComissionamentoOperadora();
                    co.ID = gridTabelas.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                    co.Remover();
                    this.CarregaTabelas();
                }
                catch
                {
                    base.Alerta(null,this, "gerr", "Não foi possível excluir a tabela de comissionamento.");
                }
            }
        }

        protected void gridTabelas_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente prosseguir com a exclusão?");
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("comissionamentoOperadora.aspx");
        }
    }
}

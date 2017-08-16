namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class feriados : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregarFeriados();
            }
        }

        void CarregarFeriados()
        {
            grid.DataSource = DiaFeriado.CarregarTodos();
            grid.DataBind();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Response.Redirect(String.Concat("~/admin/feriado.aspx?", IDKey, "=", grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0]));
            }
            else if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    DiaFeriado feriado = new DiaFeriado(grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                    feriado.Remover();
                    grid.PageIndex = 0;
                    this.CarregarFeriados();
                }
                catch
                {
                    base.Alerta(null, this, "_errExcl", "Não foi possível excluir o feriado.");
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
            Response.Redirect("feriado.aspx");
        }
    }
}

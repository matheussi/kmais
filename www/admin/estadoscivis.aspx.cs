namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class estadoscivis : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);
                this.CarregaEstadosCivis();
            }
        }

        void CarregaEstadosCivis()
        {
            if (!base.HaItemSelecionado(cboOperadora))
                grid.DataSource = null;
            else
                grid.DataSource = EstadoCivil.CarregarTodos(cboOperadora.SelectedValue);

            grid.DataBind();
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaEstadosCivis();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Response.Redirect(String.Concat("~/admin/estadocivil.aspx?", IDKey, "=", grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0]));
            }
            else if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    EstadoCivil estadocivil = new EstadoCivil(grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                    estadocivil.Remover();
                    grid.PageIndex = 0;
                    this.CarregaEstadosCivis();
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
            Response.Redirect("estadocivil.aspx");
        }
    }
}
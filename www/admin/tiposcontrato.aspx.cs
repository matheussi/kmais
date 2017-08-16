namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class tiposcontrato : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaTipos();
            }
        }

        void CarregaTipos()
        {
            grid.DataSource = TipoContrato.Carregar(false);
            grid.DataBind();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("inativar"))
            {
                TipoContrato tipo = new TipoContrato();
                tipo.ID = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                tipo.Carregar();
                tipo.Ativo = !tipo.Ativo;
                tipo.Salvar();
                this.CarregaTipos();
            }
            else if (e.CommandName.Equals("editar"))
            {
                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect("~/admin/tipocontrato.aspx?" + IDKey + "=" + id);
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja realmente prosseguir?");
                if (!Convert.ToBoolean(grid.DataKeys[e.Row.RowIndex][1]))
                {
                    e.Row.ForeColor = System.Drawing.Color.Red;
                    //((LinkButton)e.Row.Cells[1].Controls[0]).Text = "ativar";
                    ((LinkButton)e.Row.Cells[1].Controls[0]).Text = "<img src='../images/unactive.png' alt='inativo' border='0'>";
                }
                else
                {
                    //((LinkButton)e.Row.Cells[1].Controls[0]).Text = "inativar";
                    ((LinkButton)e.Row.Cells[1].Controls[0]).Text = "<img src='../images/active.png'  alt='ativo' border='0'>";
                }
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/tipocontrato.aspx");
        }
    }
}
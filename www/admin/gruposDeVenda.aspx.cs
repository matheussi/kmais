namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class gruposDeVenda : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaGrupos();
            }
        }

        void CarregaGrupos()
        {
            gridGrupos.DataSource = GrupoDeVenda.Carregar(false);
            gridGrupos.DataBind();
        }

        protected void gridGrupos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = gridGrupos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("~/admin/grupodevenda.aspx?", IDKey, "=", Session[IDKey]));
            }
            else if (e.CommandName.Equals("inativar"))
            {
                GrupoDeVenda grupo = new GrupoDeVenda();
                grupo.ID = gridGrupos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                grupo.Carregar();
                grupo.Ativo = !grupo.Ativo;
                grupo.Salvar();
                this.CarregaGrupos();
            }
            else if (e.CommandName.Equals("excluir"))
            {
                GrupoDeVenda grupo = new GrupoDeVenda();
                grupo.ID = gridGrupos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                try
                {
                    grupo.Remover();
                    this.CarregaGrupos();
                }
                catch
                {
                    base.Alerta(null, this, "__err", "Não foi possível excluir.");
                }
            }
        }

        protected void gridGrupos_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Boolean ativo = Convert.ToBoolean(gridGrupos.DataKeys[e.Row.RowIndex][1]);
                base.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja prosseguir com a exclusão?");

                if (!ativo)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Text = "<img src='../images/unactive.png' title='inativo' alt='inativo' border='0'>";
                    base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente ativar o grupo?");
                }
                else
                {
                    base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente inativar o grupo?");
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Text = "<img src='../images/active.png' title='ativo' alt='ativo' border='0'>";
                }
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/grupoDeVenda.aspx");
        }
    }
}
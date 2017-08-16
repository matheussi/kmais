namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class estipulantes : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                UIHelper.AuthCtrl(cmdNovo, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                this.CarregaEstipulantes();
            }
        }

        void CarregaEstipulantes()
        {
            grid.DataSource = Estipulante.Carregar(false);
            grid.DataBind();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("inativar"))
            {
                Estipulante estipulante = new Estipulante();
                estipulante.ID = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                estipulante.Carregar();
                estipulante.Ativo = !estipulante.Ativo;
                estipulante.Salvar();
                this.CarregaEstipulantes();
            }
            else if (e.CommandName.Equals("editar"))
            {
                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect("~/estipulante.aspx?" + IDKey + "=" + id);
            }
            else if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    Estipulante estipulante = new Estipulante();
                    estipulante.ID = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                    estipulante.Remover();
                    this.CarregaEstipulantes();
                }
                catch
                {
                    base.Alerta(null, this, "_errDel", "Não foi possível excluir o estipulante.");
                }
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UIHelper.AuthCtrl(e.Row.Cells[1].Controls[0], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                UIHelper.AuthCtrl(e.Row.Cells[2].Controls[0], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);

                base.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja realmente prosseguir com a exclusão?");
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente prosseguir?");
                if (!Convert.ToBoolean(grid.DataKeys[e.Row.RowIndex][1]))
                {
                    e.Row.ForeColor = System.Drawing.Color.Red;
                    //((LinkButton)e.Row.Cells[2].Controls[0]).Text = "ativar";
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Text = "<img src='images/unactive.png' title='inativo' alt='inativo' border='0'>";
                }
                else
                {
                    //((LinkButton)e.Row.Cells[2].Controls[0]).Text = "inativar";
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Text = "<img src='images/active.png' title='ativo' alt='ativo' border='0'>";
                }
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/estipulante.aspx");
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            CarregaEstipulantes();
        }
    }
}
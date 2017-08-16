namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class filiais : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaFiliais();
                UIHelper.AuthCtrl(cmdNovo, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            }
        }

        void CarregaFiliais()
        {
            grid.DataSource = Filial.CarregarTodas(false);
            grid.DataBind();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session[IDKey] = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("filial.aspx?", IDKey, "=", Session[IDKey]));
            }
            else if (e.CommandName == "inativar")
            {
                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Filial filial = new Filial();
                filial.ID = id;
                filial.Carregar();
                filial.Ativa = !filial.Ativa;
                filial.Salvar();

                this.CarregaFiliais();
            }
            else if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    Filial filial = new Filial();
                    filial.ID = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                    filial.Remover();
                    this.CarregaFiliais();
                }
                catch
                {
                    base.Alerta(null, this, "_errDel", "Não foi possível excluir a filial.");
                }
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UIHelper.AuthCtrl(e.Row.Cells[3].Controls[0], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                UIHelper.AuthCtrl(e.Row.Cells[4].Controls[0], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);

                base.grid_RowDataBound_Confirmacao(sender, e, 3, "Deseja realmente prosseguir com a exclusão?");
                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente prosseguir?");
                Boolean ativo = Convert.ToBoolean(grid.DataKeys[e.Row.RowIndex][1]);

                if (!ativo)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    //((LinkButton)e.Row.Cells[3].Controls[0]).Text = "ativar";
                    ((LinkButton)e.Row.Cells[4].Controls[0]).Text = "<img src='images/unactive.png' title='inativo' alt='inativo' border='0'>";
                }
                else
                {
                    //((LinkButton)e.Row.Cells[3].Controls[0]).Text = "ativar";
                    ((LinkButton)e.Row.Cells[4].Controls[0]).Text = "<img src='images/active.png' title='ativo' alt='ativo' border='0'>";
                }
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("filial.aspx");
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.CarregaFiliais();
        }
    }
}
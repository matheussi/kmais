namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class operadoras : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.CarregaOperadoras();
                UIHelper.AuthCtrl(cmdNovo, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            }

            this.CheckExtraPermission();
        }

        Boolean extraPermission()
        {
            if (Usuario.Autenticado.ExtraPermission2 || Usuario.Autenticado.PerfilID == Perfil.AdministradorIDKey)
                return true;
            else
                return false;
        }

        void CheckExtraPermission()
        {
            if (!extraPermission())
            {
                gridOperadoras.Columns[4].Visible = false;
                gridOperadoras.Columns[5].Visible = false;
            }
        }

        void CarregaOperadoras()
        {
            gridOperadoras.DataSource = Operadora.CarregarTodas();
            gridOperadoras.DataBind();
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("operadora.aspx");
        }

        protected void gridOperadoras_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session[IDKey] = gridOperadoras.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("operadora.aspx?", IDKey, "=", Session[IDKey]));
            }
            else if (e.CommandName == "inativar")
            {
                LinkButton btn = (LinkButton)gridOperadoras.Rows[Convert.ToInt32(e.CommandArgument)].Cells[4].Controls[0];
                Operadora.StatusOperadora status = Operadora.StatusOperadora.Inativa;

                Boolean inativo = Convert.ToBoolean(gridOperadoras.DataKeys[Convert.ToInt32(e.CommandArgument)][1]);
                //if (btn.Text == "ativar")
                //    status = Operadora.StatusOperadora.Ativa;

                if(inativo)
                    status = Operadora.StatusOperadora.Ativa;

                Object id = gridOperadoras.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Operadora.AlterarStatus(id, status);
                this.CarregaOperadoras();
            }
            else if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    Operadora operadora = new Operadora();
                    operadora.ID = gridOperadoras.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                    operadora.Remover();
                    gridOperadoras.PageIndex = 0;
                    this.CarregaOperadoras();
                }
                catch
                {
                    base.Alerta(null, this, "_errDel", "Não foi possível excluir a operadora.");
                }
            }
        }

        protected void gridOperadoras_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UIHelper.AuthWebCtrl(e.Row.Cells[4], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                UIHelper.AuthWebCtrl(e.Row.Cells[5], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);

                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja prosseguir com a exclusão?");
                base.grid_RowDataBound_Confirmacao(sender, e, 5, "Deseja prosseguir?");

                Boolean inativo = Convert.ToBoolean(gridOperadoras.DataKeys[e.Row.RowIndex][1]);

                if (inativo)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    //((LinkButton)e.Row.Cells[5].Controls[0]).Text = "ativar";
                    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='images/unactive.png' title='inativo' alt='inativo' border='0'>";
                }
                else
                {
                    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='images/active.png' title='ativo' alt='ativo' border='0'>";
                }
            }
        }

        protected void gridOperadoras_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            gridOperadoras.PageIndex = e.NewPageIndex;
            this.CarregaOperadoras();
        }
    }
}
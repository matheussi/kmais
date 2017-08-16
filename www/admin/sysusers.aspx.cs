namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class sysusers : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.CarregaPerfis();
            }
        }

        void CarregaPerfis()
        {
            cboPerfis.Items.Clear();
            cboPerfis.DataValueField = "ID";
            cboPerfis.DataTextField  = "Descricao";
            cboPerfis.DataSource     = Perfil.CarregarTodos(Perfil.eTipo.Indefinido);
            cboPerfis.DataBind();
            cboPerfis.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        void CarregaUsuarios()
        {
            Object perfilId = null;
            if (cboPerfis.SelectedIndex > 0) { perfilId = cboPerfis.SelectedValue; }

            if (cboPerfis.Items.Count > 0)
                gridUsuarios.DataSource = Usuario.CarregarPorNomeEmail(txtNome.Text, txtEmail.Text, perfilId);
            else
                gridUsuarios.DataSource = null;

            gridUsuarios.DataBind();
        }

        protected void gridUsuarios_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = gridUsuarios.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("~/admin/sysuser.aspx?", IDKey, "=", Session[IDKey]));
            }
            else if (e.CommandName.Equals("inativar"))
            {
                Usuario user = new Usuario();
                user.ID = gridUsuarios.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                user.Carregar();
                user.Ativo = !user.Ativo;
                user.Salvar();
                this.CarregaUsuarios();
            }
        }

        protected void gridUsuarios_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Boolean ativo = Convert.ToBoolean(gridUsuarios.DataKeys[e.Row.RowIndex][1]);

                if (!ativo)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    ((LinkButton)e.Row.Cells[3].Controls[0]).Text = "<img src='../images/unactive.png' alt='inativo' border='0'>";
                    base.grid_RowDataBound_Confirmacao(sender, e, 3, "Deseja realmente ativar o usuário?");
                }
                else
                {
                    base.grid_RowDataBound_Confirmacao(sender, e, 3, "Deseja realmente inativar o usuário?");
                    ((LinkButton)e.Row.Cells[3].Controls[0]).Text = "<img src='../images/active.png'  alt='ativo' border='0'>";
                }

                Boolean eUsuario = Convert.ToBoolean(gridUsuarios.DataKeys[e.Row.RowIndex][2]);
                if (!eUsuario) { ((Image)e.Row.Cells[4].Controls[1]).Visible = false; }
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("sysuser.aspx");
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            this.CarregaUsuarios();
        }

        protected void gridUsuarios_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            
        }

        protected void gridUsuarios_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridUsuarios.PageIndex = e.NewPageIndex;
            this.CarregaUsuarios();
        }
    }
}
namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class users : PageBase
    {
        public SortDirection GridViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Ascending;

                return (SortDirection)ViewState["sortDirection"];
            }
            set { ViewState["sortDirection"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtDocumento.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");

            if (!IsPostBack)
            {
                this.CarregaPerfis();
                this.CarregaFiliais();
                this.CarregaUsuarios();
            }
        }

        void CarregaFiliais()
        {
            base.ExibirFiliais(cboFiliais, false);
            cboFiliais.Items.Insert(0, new ListItem("USUÁRIOS SEM FILIAL", "-1"));
            cboFiliais.Items.Insert(0, new ListItem("TODOS", "0"));
        }

        void CarregaPerfis()
        {
            IList<Perfil> lista = Perfil.CarregarTodos(new Perfil.eTipo[] { Perfil.eTipo.Produtor, Perfil.eTipo.Telemarketing });

            cboPerfis.Items.Clear();
            cboPerfis.DataValueField = "ID";
            cboPerfis.DataTextField = "Descricao";
            cboPerfis.DataSource = lista;
            cboPerfis.DataBind();
            cboPerfis.Items.Insert(0, new ListItem("TODOS", "-1"));
        }

        void CarregaUsuarios()
        {
            gridUsuarios.DataSource = Usuario.CarregarPorParametro(cboFiliais.SelectedValue, cboPerfis.SelectedValue, 
                new Perfil.eTipo[] { Perfil.eTipo.Produtor, Perfil.eTipo.Telemarketing }, txtDocumento.Text, txtApelido.Text);
            gridUsuarios.DataBind();
        }

        protected void cmdBuscar_OnClick(Object sender, EventArgs e)
        {
            this.CarregaUsuarios();
        }

        protected void gridUsuarios_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = gridUsuarios.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("~/admin/user.aspx?", IDKey, "=", Session[IDKey]));
            }
            if (e.CommandName.Equals("acessos"))
            {
                Object id = gridUsuarios.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("~/admin/user_controleAcesso.aspx?", IDKey, "=", id));
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
            else if (e.CommandName.Equals("subordinados"))
            {
                gridSubordinados.DataSource = Usuario.CarregaSubordinados(gridUsuarios.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                gridSubordinados.DataBind();
                pnlSubordinados.Visible = true;

                String nome = gridUsuarios.Rows[Convert.ToInt32(e.CommandArgument)].Cells[0].Text;
                lblSuperior.InnerText = "Subordinados de " + nome.ToUpper();
                if (gridSubordinados.Rows.Count == 0)
                    lblSuperior.InnerText += " (nenhum)";
            }
            else if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    Usuario user = new Usuario(gridUsuarios.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                    user.Remover();
                    gridUsuarios.PageIndex = 0;
                    this.CarregaUsuarios();
                }
                catch
                {
                    base.Alerta(null, this, "_errExcl", "Não foi possível excluir.");
                }
            }
        }

        protected void gridUsuarios_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja prosseguir com a exclusão?");
                Boolean ativo = Convert.ToBoolean(gridUsuarios.DataKeys[e.Row.RowIndex][1]);

                if (!ativo)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    ((LinkButton)e.Row.Cells[6].Controls[0]).Text = "<img src='../images/unactive.png' title='inativo' alt='inativo' border='0'>";
                    base.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente ativar o usuário?");
                }
                else
                {
                    base.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente inativar o usuário?");
                    ((LinkButton)e.Row.Cells[6].Controls[0]).Text = "<img src='../images/active.png' title='ativo' alt='ativo' border='0'>";
                }
            }
        }

        protected void gridSubordinados_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = gridSubordinados.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("~/admin/user.aspx?", IDKey, "=", Session[IDKey]));
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/user.aspx");
        }

        protected void gridUsuarios_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            gridUsuarios.PageIndex = e.NewPageIndex;
            this.CarregaUsuarios();
        }

        //protected void gridUsuarios_OnSort(object sender, GridViewSortEventArgs e)
        //{
        //    string sortExpression = e.SortExpression;

        //    if (GridViewSortDirection == SortDirection.Ascending)
        //    {
        //        GridViewSortDirection = SortDirection.Descending;
        //        SortGridView(sortExpression, DESCENDING);
        //    }
        //    else
        //    {
        //        GridViewSortDirection = SortDirection.Ascending;
        //        SortGridView(sortExpression, ASCENDING);
        //    } 
        //}

        //private void SortGridView(string sortExpression, string direction)
        //{
        //    //  You can cache the DataTable for improving performance
        //    DataTable dt = GetData().Tables[0];

        //    DataView dv = new DataView(dt);
        //    dv.Sort = sortExpression + direction;

        //    GridView1.DataSource = dv;
        //    GridView1.DataBind();
        //}
    }
}
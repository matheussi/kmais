namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class perfis : PageBase
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
            gridPerfis.DataSource = Perfil.CarregarTodos(Perfil.eTipo.Indefinido);
            gridPerfis.DataBind();
        }

        protected void gridPerfis_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = gridPerfis.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("~/admin/perfil.aspx?", IDKey, "=", Session[IDKey]));
            }
            else if (e.CommandName.Equals("excluir"))
            {
                Perfil perfil = new Perfil();
                perfil.ID = gridPerfis.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                try
                {
                    perfil.Remover();
                    this.CarregaPerfis();
                }
                catch
                {
                    base.Alerta(null, this, "__err", "Não foi possível excluir.");
                }
            }
        }

        protected void gridPerfis_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja prosseguir com a exclusão?");
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/perfil.aspx");
        }

        protected void gridPerfis_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridPerfis.PageIndex = e.NewPageIndex;
            this.CarregaPerfis();
        }
    }
}

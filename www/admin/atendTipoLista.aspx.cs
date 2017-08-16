namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class atendTipoLista : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.carregaTiposDeAtendimento();
            }
        }

        void carregaTiposDeAtendimento()
        {
            grid.DataSource = AtendimentoTipo.CarregarTodos();
            grid.DataBind();
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.carregaTiposDeAtendimento();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                AtendimentoTipo obj = new AtendimentoTipo();
                obj.ID = grid.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                try
                {
                    obj.Remover();
                    this.carregaTiposDeAtendimento();
                }
                catch
                {
                    base.Alerta(null, this, "_errExcl", "Não foi possível excluir o tipo de atendimento.");
                }
            }
            else if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = grid.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                Response.Redirect("~/admin/atendTipo.aspx");
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja realmente excluir o tipo de atendimento?");
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/atendTipo.aspx");
        }
    }
}
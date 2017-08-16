namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class statusMotivosAlteracao : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaMotivos();
            }
        }

        void CarregaMotivos()
        {
            grid.DataSource = ContratoStatus.Carregar(ContratoStatus.eTipo.Indefinido);
            grid.DataBind();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                ContratoStatus motivo = new ContratoStatus();
                motivo.ID = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                try
                {
                    motivo.Remover();
                    this.CarregaMotivos();
                }
                catch
                {
                    base.Alerta(null, this, "_errExcl", "Não foi possível excluir.");
                }
            }
            else if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect("~/admin/statusMotivoAlteracao.aspx");
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente excluir o motivo de alteração?");
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/statusMotivoAlteracao.aspx");
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.CarregaMotivos();
        }
    }
}

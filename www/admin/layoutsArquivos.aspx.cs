namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class layoutsArquivos : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);
                base.ExibirTiposDeArquivo(cboTipoArquivo, true);
            }
        }

        protected void cmdBuscar_Click(Object sender, EventArgs e)
        {
            grid.DataSource = LayoutArquivoCustomizado.Carregar(cboOperadora.SelectedValue, 
                ((LayoutArquivoCustomizado.eTipoTransacao)Convert.ToInt32(cboTipoArquivo.SelectedValue)));
            grid.DataBind();

            if (grid.DataSource == null) { litResultado.Text = "nenhum layout de arquivo localizado"; }
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session[IDKey] = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("~/admin/layoutArquivo.aspx?", IDKey, "=", Session[IDKey]));
            }
            else if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    LayoutArquivoCustomizado arq = new LayoutArquivoCustomizado();
                    arq.ID = grid.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                    arq.Remover();
                    this.cmdBuscar_Click(null,null);
                }
                catch
                {
                    base.Alerta(null, this, "_errDel", "Não foi possível excluir o layout de arquivo.");
                }
            }
        }
        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja realmente excluir o layout de arquivo?");
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/layoutArquivo.aspx");
        }
    }
}
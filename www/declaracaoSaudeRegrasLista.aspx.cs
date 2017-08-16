namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class declaracaoSaudeRegrasLista : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, true);
            }
        }

        void CarregaItens()
        {
            if (cboOperadora.SelectedIndex > 0)
            {
                IList<RegraDeclaracaoSaude> lista = RegraDeclaracaoSaude.Carregar(cboOperadora.SelectedValue);
                gridRegras.DataSource = lista;
                gridRegras.DataBind();
            }
            else
            {
                gridRegras.DataSource = null;
                gridRegras.DataBind();
            }
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaItens();
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("declaracaoSaudeRegras.aspx");
        }

        protected void gridRegras_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = gridRegras.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect("declaracaoSaudeRegras.aspx");
            }
            else if (e.CommandName.Equals("excluir"))
            {
                RegraDeclaracaoSaude rds = new RegraDeclaracaoSaude();
                rds.ID = gridRegras.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                try
                {
                    rds.Remover();
                    this.CarregaItens();
                }
                catch
                {
                    //base.Alerta(null, this, "_err", "Não possível excluir essa regra.");
                    base.Alerta(MPE, ref litAlert, "Não possível excluir essa regra.", upnlAlerta);
                }
            }
        }

        protected void gridRegras_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja realmente prosseguir com a excluão?");
            }
        }
    }
}
namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class tabelasR : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaOperadoras();
                this.CarregaTabelasDeReajuste();
            }
        }

        void CarregaOperadoras()
        {
            cboOperadoras.DataTextField = "Nome";
            cboOperadoras.DataValueField = "ID";
            cboOperadoras.DataSource = Operadora.CarregarTodas(true);
            cboOperadoras.DataBind();
        }

        void CarregaContratos()
        {
            cboContratos.Items.Clear();
            if (cboOperadoras.Items.Count == 0) { return; }

            cboContratos.DataValueField = "ID";
            cboContratos.DataTextField = "Descricao";
            cboContratos.DataSource = ContratoADM.Carregar(cboOperadoras.SelectedValue);
            cboContratos.DataBind();
        }

        void CarregaTabelasDeReajuste()
        {
            if (cboContratos.Items.Count == 0) { gridTabelas.DataSource = null; gridTabelas.DataBind(); return; }

            gridTabelas.DataSource = TabelaReajuste.CarregarTodas(cboContratos.SelectedValue);
            gridTabelas.DataBind();
        }

        protected void cboOperadoras_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
            this.CarregaTabelasDeReajuste();
        }

        protected void cboContratos_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelasDeReajuste();
        }

        protected void gridTabelas_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session[IDKey] = gridTabelas.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("tabelaR.aspx?", IDKey, "=", Session[IDKey]));
            }
        }

        protected void gridTabelas_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Convert.ToString(gridTabelas.DataKeys[e.Row.RowIndex][0]) ==
                       Convert.ToString(gridTabelas.DataKeys[e.Row.RowIndex][1]))
                {
                    e.Row.Cells[1].Text = "Sim";
                }
                else
                {
                    e.Row.Cells[1].Text = "Não";
                }
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("tabelaR.aspx");
        }
    }
}
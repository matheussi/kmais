namespace www.financeiro
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class movimentacoes_cc : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                txtDe.Text = "01/" + DateTime.Now.ToString("MM/yyyy");
                txtAte.Text = base.UltimoDiaDoMes(DateTime.Now.Month, DateTime.Now.Year).ToString("dd/MM/yyyy");
                txtProdutor.Focus();
            }
        }

        void Localizar()
        {
            #region validacoes

            if (txtProdutor.Text.Trim() == "" || txtProdutorID.Value.Trim() == "")
            {
                base.Alerta(null, this, "__err1", "Informe o produtor.");
                txtProdutor.Focus();
                return;
            }

            DateTime de = base.CStringToDateTime(txtDe.Text);
            if (de == DateTime.MinValue)
            {
                base.Alerta(null, this, "__err2", "Data inválida.");
                txtDe.Focus();
                return;
            }
            de = new DateTime(de.Year, de.Month, de.Day, 0, 0, 0);

            DateTime ate = base.CStringToDateTime(txtAte.Text);
            if (ate == DateTime.MinValue)
            {
                base.Alerta(null, this, "__err3", "Data inválida.");
                txtAte.Focus();
                return;
            }
            ate = new DateTime(ate.Year, ate.Month, ate.Day, 23, 59, 59, 59);

            if (ate < de)
            {
                base.Alerta(null, this, "__err4", "Intervalo inválido.");
                txtAte.Focus();
                return;
            }

            #endregion

            IList <MovimentacaoContaCorrente> lista = MovimentacaoContaCorrente.Carregar(txtProdutorID.Value, de, ate);
            grid.DataSource = lista;
            grid.DataBind();

            if (lista == null)
                litMsg.Text = "Nenhum item localizado";
            else
            {
                Decimal total = MovimentacaoContaCorrente.Totalizar(lista);
                litMsg.Text = "Saldo do período: " + total.ToString("C");
            }
        }

        protected void cmdBuscar_OnClick(Object sender, EventArgs e)
        {
            this.Localizar();
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.Localizar();
        }
        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {

        }
        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((CategoriaContaCorrente.eTipo)Convert.ToInt32(grid.DataKeys[e.Row.RowIndex].Values[4])) == CategoriaContaCorrente.eTipo.Debito) //if (e.Row.Cells[2].Text.IndexOf('(') > -1)
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.Red;
                else
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.Blue;
            }
        }

        protected void cmdNova_Click(Object sender, EventArgs e)
        {
            Response.Redirect("movimentacao_cc.aspx");
        }
    }
}

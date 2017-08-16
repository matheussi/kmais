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

    public partial class movimentacao_cc : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtValor.Attributes.Add("onKeyUp", "mascara('" + txtValor.ClientID + "')");

            if (!IsPostBack)
            {
                txtDe.Text = "01/" + DateTime.Now.ToString("MM/yyyy");
                txtAte.Text = base.UltimoDiaDoMes(DateTime.Now.Month, DateTime.Now.Year).ToString("dd/MM/yyyy");
                txtProdutor.Focus();

                txtData.Text = DateTime.Now.ToString("dd/MM/yyyy");
                this.CarregaCategoriasDeCC();
            }
        }

        void CarregaCategoriasDeCC()
        {
            cboCategoria.DataValueField = "ID";
            cboCategoria.DataTextField = "Descricao";

            if(optCredito.Checked)
                cboCategoria.DataSource = CategoriaContaCorrente.CarregarTodas(CategoriaContaCorrente.eTipo.Credito);
            else
                cboCategoria.DataSource = CategoriaContaCorrente.CarregarTodas(CategoriaContaCorrente.eTipo.Debito);

            cboCategoria.DataBind();
        }

        void Localizar()
        {
            #region validacoes 

            if (txtProdutor.Text.Trim() == "" || txtProdutorID.Value.Trim() == "")
            {
                tab.ActiveTabIndex = 0;
                base.Alerta(null, this, "__err1", "Informe o produtor.");
                txtProdutor.Focus();
                return;
            }

            DateTime de = base.CStringToDateTime(txtDe.Text);
            if (de == DateTime.MinValue)
            {
                tab.ActiveTabIndex = 0;
                base.Alerta(null, this, "__err2", "Data inválida.");
                txtDe.Focus();
                return;
            }
            de = new DateTime(de.Year, de.Month, de.Day, 0, 0, 0);

            DateTime ate = base.CStringToDateTime(txtAte.Text);
            if (ate == DateTime.MinValue)
            {
                tab.ActiveTabIndex = 0;
                base.Alerta(null, this, "__err3", "Data inválida.");
                txtAte.Focus();
                return;
            }
            ate = new DateTime(ate.Year, ate.Month, ate.Day, 23, 59, 59, 500);

            if (ate < de)
            {
                tab.ActiveTabIndex = 0;
                base.Alerta(null, this, "__err4", "Intervalo inválido.");
                txtAte.Focus();
                return;
            }

            #endregion

            IList<MovimentacaoContaCorrente> lista = MovimentacaoContaCorrente.Carregar(txtProdutorID.Value, de, ate);
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

        protected void opt_Changed(Object sender, EventArgs e)
        {
            this.CarregaCategoriasDeCC();
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
                if (((CategoriaContaCorrente.eTipo)Convert.ToInt32(grid.DataKeys[e.Row.RowIndex].Values[4])) == CategoriaContaCorrente.eTipo.Debito)
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.Red;
                else
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.Blue;
            }
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("movimentacoes_cc.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (txtProdutor.Text.Trim() == "" || txtProdutorID.Value.Trim() == "")
            {
                tab.ActiveTabIndex = 0;
                base.Alerta(null, this, "__err1", "Informe o produtor.");
                txtProdutor.Focus();
                return;
            }

            if (cboCategoria.Items.Count == 0)
            {
                base.Alerta(null, this, "__err2", "Não há uma categoria cadastrada para esse tipo de movimentação.");
                cboCategoria.Focus();
                return;
            }

            if (txtValor.Text.Trim() == "" || Convert.ToDecimal(txtValor.Text) == 0)
            {
                base.Alerta(null, this, "__err4", "Informe o valor da movimentação.");
                txtValor.Focus();
                return;
            }

            DateTime data = base.CStringToDateTime(txtData.Text);
            if (data == DateTime.MinValue)
            {
                base.Alerta(null, this, "__err3", "Data inválida.");
                txtData.Focus();
                return;
            }
            data = new DateTime(data.Year, data.Month, data.Day);

            #endregion

            MovimentacaoContaCorrente mcc = new MovimentacaoContaCorrente(ViewState[IDKey]);
            mcc.CategoriaID = cboCategoria.SelectedValue;
            mcc.Data = data;
            mcc.ProdutorID = txtProdutorID.Value;
            mcc.Valor = Convert.ToDecimal(txtValor.Text);
            mcc.Motivo = txtMotivo.Text;
            //if (optDebito.Checked) { mcc.Valor *= -1; }

            try
            {
                mcc.Salvar();
                ViewState[IDKey] = mcc.ID;
                base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
            }
            catch
            {
                base.Alerta(null, this, "_errUn", "Erro inesperado. Por favor, tente novamente.");
            }
        }
    }
}
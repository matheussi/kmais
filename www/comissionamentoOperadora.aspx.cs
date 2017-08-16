namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using LC.Web.PadraoSeguros.Entity;

    public partial class comissionamentoOperadora : PageBase
    {
        Object TabelaID
        {
            get
            {
                if (ViewState[IDKey] != null)
                    return ViewState[IDKey];
                else
                    return Request[IDKey];
            }
            set { ViewState[IDKey] = value; }
        }

        /// <summary>
        /// Itens da tabela de comissionamento
        /// </summary>
        List<ComissionamentoOperadoraItem> ItensComissionamento
        {
            get
            {
                if (ViewState["_itensCom"] != null)
                {
                    List<ComissionamentoOperadoraItem> lista = ViewState["_itensCom"] as List<ComissionamentoOperadoraItem>;

                    for (int i = 0; i < gridItens.Rows.Count; i++)
                    {
                        lista[i].Parcela = base.CToInt(((TextBox)gridItens.Rows[i].Cells[0].Controls[1]).Text);
                        lista[i].Percentual = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[1].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualADM = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[4].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualCompraCarencia = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[2].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualEspecial = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[5].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualMigracao = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[3].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualIdade = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[6].Controls[1]).Text);
                        lista[i].ID = gridItens.DataKeys[i].Value;
                    }

                    return lista;
                }
                else
                    return null;
            }
            set { ViewState["_itensCom"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadoras, false);
                this.CarregaContratos();

                if (TabelaID != null) { this.CarregaTabela(); }
                else { txtData.Text = DateTime.Now.ToString("dd/MM/yyyy"); }
            }
        }

        void CarregaTabela()
        {
            ComissionamentoOperadora co = new ComissionamentoOperadora(this.TabelaID);
            co.Carregar();

            if (co.Data != DateTime.MinValue) { txtData.Text = co.Data.ToString("dd/MM/yyyy"); }
            txtDescricao.Text = co.Descricao;
            chkAtivo.Checked = co.Ativa;

            pnl.Visible = true;
            this.CarregarItensDaTabela_Comissionamento();
        }

        void CarregarItensDaTabela_Comissionamento()
        {
            List<ComissionamentoOperadoraItem> parcelas = null;

            if (cboContratos.Items.Count > 0)
            {
                parcelas = (List<ComissionamentoOperadoraItem>)
                    ComissionamentoOperadoraItem.Carregar(this.TabelaID);
            }

            gridItens.DataSource = parcelas;
            gridItens.DataBind();
            this.ItensComissionamento = parcelas;
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

        protected void cboOperadoras_Change(Object sender, EventArgs e)
        {
            this.CarregaContratos();
            this.CarregarItensDaTabela_Comissionamento();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("comissionamentosOperadora.aspx");
        }

        protected void gridComissionamentoItensDetalhe_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "excluir")
            {
                Object id = gridItens.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                if (id == null)
                {
                    List<ComissionamentoOperadoraItem> lista = this.ItensComissionamento;
                    lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                    this.ItensComissionamento = lista;
                    gridItens.DataSource = lista;
                    gridItens.DataBind();
                }
                else
                {
                    ComissionamentoOperadoraItem item = new ComissionamentoOperadoraItem();
                    item.ID = id;
                    item.Carregar();
                    item.Remover();
                    this.CarregarItensDaTabela_Comissionamento();
                }
            }
        }

        protected void gridComissionamentoItensDetalhe_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja realmente excluir a linha?");

                TextBox txtvalor1 = (TextBox)e.Row.Cells[1].Controls[1];
                txtvalor1.Attributes.Add("onKeyUp", "mascara('" + txtvalor1.ClientID + "')");

                TextBox txtvalor2 = (TextBox)e.Row.Cells[2].Controls[1];
                txtvalor2.Attributes.Add("onKeyUp", "mascara('" + txtvalor2.ClientID + "')");

                TextBox txtvalor3 = (TextBox)e.Row.Cells[3].Controls[1];
                txtvalor3.Attributes.Add("onKeyUp", "mascara('" + txtvalor3.ClientID + "')");

                TextBox txtvalor5 = (TextBox)e.Row.Cells[5].Controls[1];
                txtvalor5.Attributes.Add("onKeyUp", "mascara('" + txtvalor5.ClientID + "')");

                if (CToDecimal(txtvalor1.Text) == 0) { txtvalor1.Text = ""; }
                if (CToDecimal(txtvalor2.Text) == 0) { txtvalor2.Text = ""; }
                if (CToDecimal(txtvalor3.Text) == 0) { txtvalor3.Text = ""; }
                if (CToDecimal(txtvalor5.Text) == 0) { txtvalor5.Text = ""; }
            }
        }

        protected void cmdAddItemCom_Click(Object sender, EventArgs e)
        {
            List<ComissionamentoOperadoraItem> lista = this.ItensComissionamento;
            if (lista == null) { lista = new List<ComissionamentoOperadoraItem>(); }

            lista.Add(new ComissionamentoOperadoraItem());

            gridItens.DataSource = lista;
            gridItens.DataBind();
            this.ItensComissionamento = lista;
        }

        protected void cboContratos_Change(Object sender, EventArgs e)
        {
            this.CarregarItensDaTabela_Comissionamento();
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err0", "Informa uma descrição.");
                txtDescricao.Focus();
                return;
            }

            if (txtData.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err1", "Informe uma data.");
                txtData.Focus();
                return;
            }
            else
            {
                DateTime data = new DateTime();
                if (!UIHelper.TyParseToDateTime(txtData.Text, out data))
                {
                    base.Alerta(null, this.Page, "_err1b", "A data informada está inválida.");
                    txtData.Focus();
                    return;
                }
            }

            if (cboContratos.Items.Count == 0)
            {
                base.Alerta(null, this.Page, "_err2", "Não há um contrato selecionado.");
                return;
            }

            String msg = UIHelper.ChecaGridDeParcelas(gridItens, 0);
            if (msg != String.Empty)
            {
                base.Alerta(null, this, "_err3", msg);
                return;
            }
            #endregion

            ComissionamentoOperadora co = new ComissionamentoOperadora(this.TabelaID);
            co.Ativa = chkAtivo.Checked;
            co.Data = base.CStringToDateTime(txtData.Text);
            //co.Descricao = txtDescricao.Text;
            co.Salvar();
            this.TabelaID = co.ID;

            if (this.ItensComissionamento != null && this.ItensComissionamento.Count > 0)
            {
                foreach (ComissionamentoOperadoraItem coi in this.ItensComissionamento)
                {
                    coi.ComissionamentoID = co.ID;
                    coi.ContratoID = cboContratos.SelectedValue;
                    coi.Salvar();
                }
            }

            pnl.Visible = true;
            base.Alerta(null, this, "_ok", "Dados salvos com sucessos.");
        }
    }
}
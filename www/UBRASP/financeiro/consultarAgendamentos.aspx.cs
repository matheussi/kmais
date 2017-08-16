namespace www.UBRASP.financeiro
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class consultarAgendamentos : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.CarregaRemessas();
            }
        }

        void LimpaCamposDetalhe()
        {
            txtParcela.Text = "";
            txtDataVencimento.Text = "";
            txtValor.Text = "";
            txtDataPgto.Text = "";
            txtValorPagto.Text = "";
        }

        IList<ArquivoRemessaAgendamento> CarregaRemessas()
        {
            //DateTime? proc = base.CStringToDateTimeG(txtProcessadoEm.Text);
            //DateTime? venctoDe = base.CStringToDateTimeG(txtVencimentoDe.Text);
            //DateTime? venctoAte = base.CStringToDateTimeG(txtVencimentoAte.Text); ;
            //DateTime? vigDe = null;
            //DateTime? vigAte = null;

            //if (proc == null && (venctoDe == null || venctoAte == null) && (vigDe == null || vigAte == null))
            //{
            //    base.Alerta(null, this, "_errParam", "Erro ao informar os parâmetros.");
            //    return null;
            //}

            IList<ArquivoRemessaAgendamento> lista = ArquivoRemessaAgendamento.CarregarUltimos20Legado();

            gridResult.DataSource = lista;
            gridResult.DataBind();

            if (lista == null || lista.Count == 0)
                litMsg.Text = "nenhum agendamento localizado";
            else
            {
                litMsg.Text = "";
            }

            return lista;
        }

        protected void cmdLocalizarArquivos_Click(Object sender, EventArgs e)
        {
            //if (base.CStringToDateTime(txtVencimentoDe.Text) == DateTime.MinValue ||
            //    base.CStringToDateTime(txtVencimentoAte.Text) == DateTime.MinValue)
            //{
            //    base.Alerta(null, this, "_err", "Informe as datas de vencimento.");
            //    return;
            //}
            this.CarregaRemessas();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////// 

        void CarregaCobrancas(Object arquivoId, Int32 gridIndice)
        {
            IList<Cobranca> cobrancas = Cobranca.CarregarPorArquivoRemessaID(grid.DataKeys[gridIndice].Value, true);
            grid.SelectedIndex = gridIndice;

            gridCobrancas.DataSource = cobrancas;
            gridCobrancas.SelectedIndex = -1;
            gridCobrancas.DataBind();

            pnlDetalhe.Visible = false;

            if (cobrancas == null || cobrancas.Count == 0)
            {
                litMsgCobrancas.Text = "Nenhuma cobrança localizada";
            }
            else
            {
                litMsgCobrancas.Text = "Cobranças:";
            }
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            IList<ArquivoCobrancaUnibanco> lista = null;

            if (base.HaItemSelecionado(cboOperadora))
            {
                lista = ArquivoCobrancaUnibanco.Carregar(cboOperadora.SelectedValue,
                    Convert.ToInt32(cboMes.SelectedValue), Convert.ToInt32(cboAno.SelectedValue));
            }
            else
            {
                lista = ArquivoCobrancaUnibanco.Carregar(null,
                    Convert.ToInt32(cboMes.SelectedValue), Convert.ToInt32(cboAno.SelectedValue));
            }

            grid.DataSource = lista;
            grid.SelectedIndex = -1;
            grid.DataBind();
            ViewState[ArquivosObjKey] = lista;

            //zera o grid de cobrancas do arquivo
            gridCobrancas.DataSource = null;
            gridCobrancas.SelectedIndex = -1;
            gridCobrancas.DataBind();

            this.LimpaCamposDetalhe();
            pnlDetalhe.Visible = false;

            litMsgCobrancas.Text = "";
            if (lista == null || lista.Count == 0)
            {
                litMsg.Text = "Nenhum arquivo de remessa encontrato";
                pnlListaArquivos.Visible = false;
            }
            else
            {
                litMsg.Text = "Resultado:";
                pnlListaArquivos.Visible = true;
            }
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("cobrancas"))
            {
                this.CarregaCobrancas(grid.DataKeys[Convert.ToInt32(
                    e.CommandArgument)].Value, Convert.ToInt32(e.CommandArgument));
            }
            else if (e.CommandName.Equals("baixar"))
            {
                IList<ArquivoRemessaAgendamento> arquivos = CarregaRemessas();
                ArquivoRemessaAgendamento arquivo = arquivos[Convert.ToInt32(e.CommandArgument)];

                base.BaixaArquivoFinanceiro(arquivo.ArquivoNomeInstance);
            }
            else if (e.CommandName.Equals("marcarComoNaoEnviadas"))
            {
                //TODO: ao marcar cobrancas nao pagas como nao enviadas em arquivoCobrancasConsulta.aspx, regerar o arquivo de remessa sem essas cobrancas
                Cobranca.MarcarCobrancasComoNaoEnviadas(grid.DataKeys[Convert.ToInt32(e.CommandArgument)].Value, true);
                cmdLocalizar_Click(null, null);
                base.Alerta(null, this, "__naoEnvsOk", "As cobranças não pagas desse arquivo de remessa foram marcadas como \\'não enviadas\\'.");
            }
        }
        protected void grid_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton imbDownloadArquivoButton = (ImageButton)e.Row.FindControl("imbDownloadArquivo");
                imbDownloadArquivoButton.CommandArgument = e.Row.RowIndex.ToString();
                ScriptManager.GetCurrent(this).RegisterPostBackControl(imbDownloadArquivoButton);

                //base.grid_RowDataBound_Confirmacao(sender, e, 5, "Esta ação marcará todas as cobranças não pagas deste arquivo como \\'não enviadas\\'.\\nDeseja realmente continuar?");
            }
        }
        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
        }

        protected void gridCobrancas_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Object id = gridCobrancas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                Cobranca cobranca = new Cobranca(id);
                cobranca.Carregar();

                gridCobrancas.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                pnlDetalhe.Visible = true;

                txtParcela.Text = cobranca.Parcela.ToString();
                txtDataVencimento.Text = cobranca.DataVencimento.ToString("dd/MM/yyyy");
                txtValor.Text = cobranca.Valor.ToString("N2");

                if (cobranca.DataPgto != DateTime.MinValue)
                {
                    txtValorPagto.Text = cobranca.ValorPgto.ToString("N2");
                    txtDataPgto.Text = cobranca.DataPgto.ToString("dd/MM/yyyy");
                }
            }
            else if (e.CommandName.Equals("marcarComoNaoEnviada"))
            {
                Object arquivoId = grid.DataKeys[grid.SelectedIndex].Value;
                Object cobrancaId = gridCobrancas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                Cobranca.MarcarCobrancaComoNaoEnviadas(arquivoId, cobrancaId);

                //TODO: regerar o arquivo sem a cobranca marcada como não enviada
                base.Alerta(null, this, "__naoEnvOk", "A cobrança foi marcada como \\'não enviada\\'.");
            }
        }
        protected void gridCobrancas_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Convert.ToBoolean(gridCobrancas.DataKeys[e.Row.RowIndex][1]))
                {
                    e.Row.ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[8].Controls[0].Visible = false;
                    e.Row.Cells[9].Controls[0].Visible = false;
                }
            }
        }
        protected void gridCobrancas_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 9, "Esta ação marcará esta cobrança como \\'não enviada\\'.\\nDeseja realmente continuar?");
            }
        }

        protected void cmdFechar_Click(Object sender, EventArgs e)
        {
            gridCobrancas.SelectedIndex = -1;
            pnlDetalhe.Visible = false;
        }
        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes

            if (txtParcela.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err1", "Informe a parcela da cobrança.");
                txtParcela.Focus();
                return;
            }

            DateTime dataVencimento = DateTime.MinValue;
            if (!UIHelper.TyParseToDateTime(txtDataVencimento.Text, out dataVencimento))
            {
                base.Alerta(null, this, "__err2", "Data de vencimento inválida.");
                txtDataVencimento.Focus();
                return;
            }

            if (txtValor.Text.Trim() == "" || base.CToDecimal(txtValor.Text) == Decimal.Zero)
            {
                base.Alerta(null, this, "__err3", "Informe o valor da cobrança.");
                txtValor.Focus();
                return;
            }

            DateTime dataPgto = DateTime.MinValue;
            if (txtDataPgto.Text.Trim() != "")
            {
                if (!UIHelper.TyParseToDateTime(txtDataPgto.Text, out dataPgto))
                {
                    base.Alerta(null, this, "__err2", "Data de pagamento inválida.");
                    txtDataPgto.Focus();
                    return;
                }

                if (txtValorPagto.Text.Trim() == "" || UIHelper.CToDecimal(txtValorPagto.Text) == 0)
                {
                    base.Alerta(null, this, "__err3", "Informe o valor de pagamento.");
                    txtValorPagto.Focus();
                    return;
                }
            }

            #endregion

            Cobranca cobranca = new Cobranca(gridCobrancas.DataKeys[gridCobrancas.SelectedIndex].Value);
            cobranca.Carregar();

            cobranca.Parcela = Convert.ToInt32(txtParcela.Text);
            cobranca.DataVencimento = dataVencimento;
            cobranca.Valor = base.CToDecimal(txtValor.Text);

            if (dataPgto != DateTime.MinValue)
            {
                cobranca.DataPgto = dataPgto;
                cobranca.ValorPgto = base.CToDecimal(txtValorPagto.Text);
                cobranca.Pago = true;
            }
            else
            {
                cobranca.DataPgto = DateTime.MinValue;
                cobranca.ValorPgto = Decimal.Zero;
                cobranca.Pago = false;
            }

            cobranca.Salvar();
            this.CarregaCobrancas(grid.DataKeys[gridCobrancas.SelectedIndex].Value, gridCobrancas.SelectedIndex);

            gridCobrancas.SelectedIndex = -1;
            pnlDetalhe.Visible = false;
        }
    }
}
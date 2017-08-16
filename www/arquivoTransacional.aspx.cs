namespace www
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;
    using System.Configuration;

    using LC.Web.PadraoSeguros.Entity;

    public partial class arquivoTransacional : PageBase
    {
        #region Private Methods

        #region CarregaDataDiaria

        /// <summary>
        /// Método para preencher com a Data Diária.
        /// </summary>
        private void CarregaDataDiaria()
        {
            this.txtDataInicial.Text = DateTime.Now.ToString("dd/MM/yyyy");
            this.txtDataFinal.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        #endregion

        #region CarregaOperadoras

        /// <summary>
        /// Método para Carregar Operadoras.
        /// </summary>
        private void CarregaOperadoras()
        {
            IList<Operadora> operadoras = Operadora.CarregarTodas(true);

            this.cboOperadora.DataValueField = "ID";
            this.cboOperadora.DataTextField  = "Nome";
            this.cboOperadora.DataSource     = operadoras;
            this.cboOperadora.DataBind();

            this.cboOperadora.Items.Insert(0, new ListItem("Selecione...".ToUpper(), "-1"));

            this.cboOperadoraGerarArquivo.DataValueField = "ID";
            this.cboOperadoraGerarArquivo.DataTextField = "Nome";
            this.cboOperadoraGerarArquivo.DataSource = operadoras;
            this.cboOperadoraGerarArquivo.DataBind();

            this.cboOperadoraGerarArquivo.Items.Insert(0, new ListItem("Selecione...".ToUpper(), "-1"));
        }

        #endregion

        #region CarregaContratosADM

        /// <summary>
        /// Carrega contratos ADM e adicionais para operadoras
        /// </summary>
        private void CarregaContratosADMeAdicionais()
        {
            lstAdicionais.Items.Clear();
            lstContratosAdm.Items.Clear();
            if (!base.HaItemSelecionado(cboOperadoraGerarArquivo))
            {
                return;
            }

            lstContratosAdm.DataValueField = "ID";
            lstContratosAdm.DataTextField = "Descricao";
            lstContratosAdm.DataSource = ContratoADM.Carregar(cboOperadoraGerarArquivo.SelectedValue);
            lstContratosAdm.DataBind();

            lstAdicionais.DataValueField = "ID";
            lstAdicionais.DataTextField = "Descricao";
            lstAdicionais.DataSource = Adicional.CarregarPorOperadoraID(cboOperadoraGerarArquivo.SelectedValue);
            lstAdicionais.DataBind();
            lstAdicionais.Items.Insert(0, new ListItem("nenhum", "-1"));
        }
        #endregion

        #region CarregaLote

        /// <summary>
        /// Método para Carregar os Lotes por Operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        private void CarregaLote(Object OperadoraID, List<Object> TipoMovimentacao)
        {
            if (OperadoraID != null && !OperadoraID.ToString().Equals("-1") && TipoMovimentacao != null && TipoMovimentacao.Count > 0)
            {
                #region Date Validation

                Boolean sobrescreveHHmmss = false;
                DateTime dtFrom     = new DateTime();
                DateTime dtTo       = new DateTime();
                DateTime dtVigencia = new DateTime();

                // DATA VIGENCIA

                if (this.txtVigencia.Text.Trim().Length.Equals(0))
                    dtVigencia = DateTime.MinValue;
                else
                {
                    if (!UIHelper.TyParseToDateTime(this.txtVigencia.Text, out dtVigencia))
                    {
                        base.Alerta(null, this.Page, "_err2b", "A data de vigência informada está inválida.");
                        this.txtVigencia.Focus();

                        return;
                    }
                }

                // DATA INICIAL

                if (this.txtDataInicial.Text.Trim().Length.Equals(0))
                {
                    base.Alerta(null, this.Page, "_err1", "Você deve informar a data inicial.");
                    this.txtDataInicial.Focus();

                    return;
                }
                else
                {
                    if (!UIHelper.TyParseToDateTime(this.txtDataInicial.Text, out dtFrom))
                    {
                        base.Alerta(null, this.Page, "_err1b", "A data inicial informada está inválida.");
                        this.txtDataInicial.Focus();

                        return;
                    }
                }

                if (pnlDeLocalizar.Visible)
                {
                    dtFrom = new DateTime(dtFrom.Year, dtFrom.Month, dtFrom.Day, Int32.Parse(cboDeHoraLocalizar.SelectedValue), Int32.Parse(cboDeMinutoLocalizar.SelectedValue), 0, 0);
                    sobrescreveHHmmss = true;
                }

                // DATA FINAL

                if (this.txtDataFinal.Text.Trim().Length.Equals(0))
                {
                    base.Alerta(null, this.Page, "_err2", "Você deve informar a data final.");
                    this.txtDataFinal.Focus();

                    return;
                }
                else
                {
                    if (!UIHelper.TyParseToDateTime(this.txtDataFinal.Text, out dtTo))
                    {
                        base.Alerta(null, this.Page, "_err2b", "A data final informada está inválida.");
                        this.txtDataFinal.Focus();

                        return;
                    }
                }

                if (pnlAteLocalizar.Visible)
                {
                    dtTo = new DateTime(dtTo.Year, dtTo.Month, dtTo.Day, Int32.Parse(cboAteHoraLocalizar.SelectedValue), Int32.Parse(cboAteMinutoLocalizar.SelectedValue), 59, 500);
                }

                #endregion

                IList<ArqTransacionalLote> lstLotes = ArqTransacionalLote.CarregarPorOperadora(OperadoraID, dtFrom, dtTo, dtVigencia , TipoMovimentacao, chkExportacaoListar.Checked, sobrescreveHHmmss);

                if (lstLotes != null && lstLotes.Count > 0)
                {
                    this.gridLotes.DataSource     = lstLotes;
                    this.pnlListaArquivos.Visible = true;
                    this.litMessage.Visible       = false;
                }
                else
                {
                    this.gridLotes.DataSource     = null;
                    this.pnlListaArquivos.Visible = false;
                    this.litMessage.Text          = "Nenhum arquivo encontrado.";
                    this.litMessage.Visible       = true;
                }

                this.gridLotes.DataBind();
            }
            else
                base.Alerta(this.up, this.Page, "__erro_message__", "Favor selecionar todos os critérios de localização.");
        }

        #endregion

        #region GetTipoMovimentacao

        /// <summary>
        /// Método para pegar o nome amigavel do tipo de movimentação.
        /// </summary>
        /// <param name="TipoMovimentacao">Código do Tipo de Movimentação.</param>
        /// <returns>Retorna uma String com o nome amigável do Tipo de Movimentação.</returns>
        private String GetTipoMovimentacao(String TipoMovimentacao)
        {
            if (!String.IsNullOrEmpty(TipoMovimentacao))
            {
                switch (TipoMovimentacao)
                {
                    case "I":

                        return "Inclusão";

                    case "A":

                        return "Alteração";

                    default:

                        return TipoMovimentacao;
                }
            }
            else
                return String.Empty;
        }

        #endregion

        #region traduzTipo 

        LayoutArquivoCustomizado.eTipoTransacao traduzTipo(DropDownList cbo)
        {
            return (LayoutArquivoCustomizado.eTipoTransacao)Convert.ToInt32(cbo.SelectedValue);
        }

        #endregion

        #region configuraCarregaLayoutsArquivos 

        /// <summary>
        /// Checa se deve exibir os layouts customizados e se é uma exportação para o SEG.
        /// </summary>
        void configuraCarregaLayoutsArquivos(DropDownList cbo, DropDownList cboOper, DropDownList cboTrans, HtmlTableRow tr)
        {
            //cbo.Items.Clear();
            //if (base.HaItemSelecionado(cboOper) && ((!Operadora.IsUnimed(cboOper.SelectedValue) && !Operadora.IsUnimedFortaleza(cboOper.SelectedValue) && !Operadora.IsSalutar(cboOper.SelectedValue)) || chkExportacao.Checked))
            //{
                if (tr != null) { tr.Visible = true; }

                cbo.DataValueField = "ID";
                cbo.DataTextField = "Nome";

                IList<LayoutArquivoCustomizado> layouts = LayoutArquivoCustomizado.
                    Carregar(cboOper.SelectedValue, traduzTipo(cboTrans));
                cbo.DataSource = layouts;
                cbo.DataBind();
            //}
            //else //if (base.HaItemSelecionado(cboOperadoraGerarArquivo))
            //{
            //    if (tr != null) { tr.Visible = false; }
            //}

            LayoutArquivoCustomizado.eTipoTransacao tipo = traduzTipo(cboTrans);
            if (tipo == LayoutArquivoCustomizado.eTipoTransacao.Inclusao || tipo == LayoutArquivoCustomizado.eTipoTransacao.AdicionaBeneficiario)
            {
                trVigencia.Visible = true;
            }
            else
            {
                trVigencia.Visible = false;
            }

            if (tipo == LayoutArquivoCustomizado.eTipoTransacao.SINCRONIZACAO_SEG && cboTrans.ID == "cboTipo_Carregar")
            {
                pnlDeLocalizar.Visible = true;
                pnlAteLocalizar.Visible = true;
            }
            else
            {
                pnlDeLocalizar.Visible = false;
                pnlAteLocalizar.Visible = false;
            }
        }
        #endregion

        #endregion

        #region Page Event Handlers

        #region OnLoad

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.CarregaOperadoras();
                this.CarregaDataDiaria();
                this.CarregaContratosADMeAdicionais();
                base.ExibirTiposDeArquivo(cboTipoTransacao, false);
                base.ExibirTiposDeArquivo(cboTipo_Carregar, false);

                base.PreencheComboNumerico(cboDeHora, 0, 23, true);
                base.PreencheComboNumerico(cboDeMinuto, 0, 59, true);
                base.PreencheComboNumerico(cboAteHora, 0, 23, true);
                base.PreencheComboNumerico(cboAteMinuto, 0, 59, true);

                base.PreencheComboNumerico(cboDeHoraLocalizar, 0, 23, true);
                base.PreencheComboNumerico(cboDeMinutoLocalizar, 0, 59, true);
                base.PreencheComboNumerico(cboAteHoraLocalizar, 0, 23, true);
                base.PreencheComboNumerico(cboAteMinutoLocalizar, 0, 59, true);

                txtDe.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtAte.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        #endregion

        #endregion

        #region Protected Control Event Handlers

        protected void cboOperadoraGerarArquivo_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            base.ExibirTiposDeArquivo(cboTipoTransacao, chkExportacao.Checked);
            this.configuraCarregaLayoutsArquivos(cboLayouts, cboOperadoraGerarArquivo, cboTipoTransacao, trLayout);
            this.CarregaContratosADMeAdicionais();
            trAdicionais.Visible = chkExportacao.Checked;
        }

        protected void chkExportacaoListar_Checked(Object sender, EventArgs e)
        {
            base.ExibirTiposDeArquivo(cboTipo_Carregar, chkExportacaoListar.Checked);
        }

        protected void cboOperadora_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.configuraCarregaLayoutsArquivos(cboLayouts_Carregar, cboOperadora, cboTipo_Carregar, null);
        }

        protected void cboTipo_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.configuraCarregaLayoutsArquivos(cboLayouts, cboOperadoraGerarArquivo, cboTipoTransacao, trLayout);
            if (cboTipoTransacao.SelectedValue == "7") //sincronizar seg
                trPeriodo.Visible = true;
            else
                trPeriodo.Visible = false;
        }

        protected void cboTipo_Carregar_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.configuraCarregaLayoutsArquivos(cboLayouts_Carregar, cboOperadora, cboTipo_Carregar, null);
        }

        #region Button Gerar

        protected void btnGerarArquivo_OnClick(Object sender, EventArgs e)
        {
            #region validacao 

            if (!base.HaItemSelecionado(cboOperadoraGerarArquivo))
            {
                base.Alerta(null, this, "__errOper", "Selecione uma operadora.");
                return;
            }

            DateTime vigencia = DateTime.MinValue;
            if (trVigencia.Visible)
            {
                vigencia = base.CStringToDateTime(txtVigenciaGerar.Text);
                if (vigencia == DateTime.MinValue)
                {
                    base.Alerta(null, this, "__errVig", "Informe uma data de vigência.");
                    txtVigenciaGerar.Focus();
                    return;
                }
            }

            String[] contratoAdmIDs = PegaIDsSelecionados(lstContratosAdm);
            if (contratoAdmIDs == null || contratoAdmIDs.Length == 0)
            {
                base.Alerta(null, this, "__errContrAdm", "Selecione um ou mais contratos administrativos.");
                lstContratosAdm.Focus();
                return;
            }

            String[] adicionaisIDs = null;
            if (trAdicionais.Visible) { adicionaisIDs = PegaIDsSelecionados(lstAdicionais); }

            #endregion

            if (this.cboOperadoraGerarArquivo.SelectedValue != "-1")
            {
                String strArquivoNome = null;
                this.lblGerarArquivoMessage.Text = "";

                try
                {
                    //if (Operadora.IsUnimed(this.cboOperadoraGerarArquivo.SelectedValue) && !chkExportacao.Checked)
                    //{
                    //    #region Unimed 

                    //    ArqTransacionalUnimed arqTrans = new ArqTransacionalUnimed(Server.MapPath("/"));
                    //    arqTrans.GerarArquivoPorStatus(this.cboOperadoraGerarArquivo.SelectedValue, 
                    //        ref strArquivoNome, 
                    //        traduzTipoStatusContratoBeneficiarioTipoArquivo(this.cboTipoTransacao.SelectedValue), 
                    //        contratoAdmIDs, null, vigencia);

                    //    if (!String.IsNullOrEmpty(strArquivoNome))
                    //    {
                    //        this.lblGerarArquivoMessage.Text    = String.Empty;
                    //        this.lblGerarArquivoMessage.Visible = false;

                    //        this.BaixarArquivo(String.Concat(ArqTransacionalFilePath, strArquivoNome), strArquivoNome);
                    //    }
                    //    else
                    //    {
                    //        this.lblGerarArquivoMessage.Text    = "<br>Não possui nenhuma pendência para geração do arquivo selecionado.";
                    //        this.lblGerarArquivoMessage.Visible = true;
                    //    }
                    //    #endregion
                    //}
                    //else if (Operadora.IsUnimedFortaleza(this.cboOperadoraGerarArquivo.SelectedValue) && !chkExportacao.Checked)
                    //{
                    //    #region Unimed Fortaleza 

                    //    ArqTransacionalUnimed arqTrans = new ArqTransacionalUnimed(Server.MapPath("/"));
                    //    arqTrans.GerarArquivoPorStatus(this.cboOperadoraGerarArquivo.SelectedValue,
                    //        ref strArquivoNome,
                    //        traduzTipoStatusContratoBeneficiarioTipoArquivo(this.cboTipoTransacao.SelectedValue),
                    //        contratoAdmIDs, null, vigencia);

                    //    if (!String.IsNullOrEmpty(strArquivoNome))
                    //    {
                    //        this.lblGerarArquivoMessage.Text = String.Empty;
                    //        this.lblGerarArquivoMessage.Visible = false;

                    //        this.BaixarArquivo(String.Concat(ArqTransacionalFilePath, strArquivoNome), strArquivoNome);
                    //    }
                    //    else
                    //    {
                    //        this.lblGerarArquivoMessage.Text = "<br>Não possui nenhuma pendência para geração do arquivo selecionado.";
                    //        this.lblGerarArquivoMessage.Visible = true;
                    //    }
                    //    #endregion
                    //}
                    //else if (Operadora.IsSalutar(this.cboOperadoraGerarArquivo.SelectedValue))
                    //{
                    //    ArqTransacionalAmil arqTrans = new ArqTransacionalAmil(Server.MapPath("/"));
                    //    arqTrans.GerarArquivoPorStatus(this.cboOperadoraGerarArquivo.SelectedValue,
                    //        ref strArquivoNome,
                    //        traduzTipoStatusContratoBeneficiarioTipoArquivo(this.cboTipoTransacao.SelectedValue),
                    //        contratoAdmIDs, null, vigencia);

                    //    if (!String.IsNullOrEmpty(strArquivoNome))
                    //    {
                    //        this.lblGerarArquivoMessage.Text = String.Empty;
                    //        this.lblGerarArquivoMessage.Visible = false;

                    //        this.BaixarArquivo(String.Concat(ArqTransacionalFilePath, strArquivoNome), strArquivoNome);
                    //    }
                    //    else
                    //    {
                    //        this.lblGerarArquivoMessage.Text = "<br>Não possui nenhuma pendência para geração do arquivo selecionado.";
                    //        this.lblGerarArquivoMessage.Visible = true;
                    //    }
                    //}
                    //else // OUTRAS OPERADORAS OU ARQUIVO PARA EXPORTACAO
                    //{
                        #region Outras Operadoras

                        LayoutArquivoCustomizado layout = new LayoutArquivoCustomizado(cboLayouts.SelectedValue);
                        layout.Carregar();
                        String arquivoNome = null;
                        String strFullFilePath = null;

                        DateTime de = DateTime.MinValue, ate = DateTime.MinValue;

                        if (cboTipoTransacao.SelectedValue == "7") //sincronização SEG
                        {
                            de = base.CStringToDateTime(txtDe.Text, cboDeHora.SelectedValue + ":" + cboDeMinuto.SelectedValue, 0);
                            ate = base.CStringToDateTime(txtAte.Text, cboAteHora.SelectedValue + ":" + cboAteMinuto.SelectedValue, 59);

                            if (de == DateTime.MinValue || ate == DateTime.MinValue)
                            {
                                base.Alerta(null, this, "errPer", "Período de data inválido.");
                                return;
                            }
                        }

                        String content = null, erro = null;
                        try
                        {
                            content = layout.GeraConteudoDoArquivo(ref arquivoNome, ref strFullFilePath, contratoAdmIDs, adicionaisIDs, vigencia, chkExportacao.Checked, de, ate, chkSomenteComAdicional.Checked, ref erro);
                        }
                        catch
                        {
                            base.Alerta(null, this, "_layErr", "Houve um erro na configuração do layout de arquivo:\\n" + erro.Replace("'", "|"));
                            return;
                        }

                        if (!String.IsNullOrEmpty(content))
                        {
                            if (LayoutArquivoCustomizado.eFormatoArquivo.Xls == ((LayoutArquivoCustomizado.eFormatoArquivo)layout.Formato))
                            {
                                base.EscreveArquivo(content, strFullFilePath, arquivoNome);
                                Session["content"] = content;
                                Response.Redirect("xlsfile.aspx");
                            }
                            else
                            {
                                base.BaixarArquivo(content, strFullFilePath, arquivoNome);
                            }
                        }
                        else
                        {
                            this.lblGerarArquivoMessage.Text = "<br>Não foi possível gerar o arquivo.";
                            this.lblGerarArquivoMessage.Visible = true;
                        }

                        #endregion
                    //}
                }
                catch (Exception) { throw; }
            }
        }

        #endregion

        #region Button Localizar

        protected void btnListarLote_Click(Object sender, EventArgs e)
        {
            List<Object> lstTipoMovimentacao = new List<Object>();
            if ((Operadora.IsUnimed(cboOperadora.SelectedValue) || Operadora.IsUnimedFortaleza(cboOperadora.SelectedValue) ) && !chkExportacaoListar.Checked)
            {
                lstTipoMovimentacao.Add(traduzTipoStatusContratoBeneficiarioMovimentacao(cboTipo_Carregar.SelectedValue));
            }
            else //OUTRAS OPERADORAS
            {
                lstTipoMovimentacao.Add(LayoutArquivoCustomizado.TraduzMovimentacao((LayoutArquivoCustomizado.eTipoTransacao)Convert.ToInt32(cboTipo_Carregar.SelectedValue)));
            }

            this.CarregaLote(this.cboOperadora.SelectedValue, lstTipoMovimentacao);
        }

        #endregion

        #region GridView Lotes

        #region OnRowCreated

        protected void gridLotes_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton imbDownloadArquivoButton = (ImageButton)e.Row.FindControl("imbDownloadArquivo");
                imbDownloadArquivoButton.CommandArgument = e.Row.RowIndex.ToString();
                ScriptManager.GetCurrent(this).RegisterPostBackControl(imbDownloadArquivoButton);
            }
        }

        #endregion

        #region OnRowDataBound

        protected void gridLotes_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.DataItem != null)
            {
                ArqTransacionalLote lote = (ArqTransacionalLote)e.Row.DataItem;

                if (lote != null)
                {
                    Operadora operadora = new Operadora();
                    operadora.ID = lote.OperadoraID;
                    operadora.Carregar();

                    if (!String.IsNullOrEmpty(operadora.Nome))
                        e.Row.Cells[0].Text = operadora.Nome;
                    else
                        e.Row.Cells[0].Text = "Não Informado";

                    e.Row.Cells[1].Text = lote.Quantidade.ToString();
                    e.Row.Cells[2].Text = lote.Numeracao.ToString().PadLeft(3, '0');
                    e.Row.Cells[3].Text = lote.DataCriacao.ToString("dd/MM/yyyy HH:mm");
                    e.Row.Cells[4].Text = this.traduzMovimentacaoLote(lote.Movimentacao);
                }
            }
        }

        #endregion

        #region OnRowCommand

        protected void gridLotes_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "down":

                    ArqTransacionalLote lote = new ArqTransacionalLote();
                    lote.ID = this.gridLotes.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                    lote.Carregar();

                    String strFileName = lote.Arquivo;
                    Object operadoraId = lote.OperadoraID;
                    lote = null;

                    if (Operadora.IsUnimed(operadoraId) || Operadora.IsUnimedFortaleza(operadoraId))
                    {
                        this.BaixarArquivo(String.Concat(ArqTransacionalFilePath, strFileName), strFileName);
                    }
                    else //OUTRAS OPERADORAS
                    {
                        String path = LayoutArquivoCustomizado.ReservatoryPath;
                        if (strFileName.IndexOf("XLS") == -1) //ARQUIVO DE TEXTO
                        {
                            base.BaixarArquivo(path, strFileName);
                        }
                        else //ARQUIVO XLS
                        {
                            Session["content"] = base.LeArquivo(path, strFileName);
                            if (Session["content"] == null)
                            {
                                base.Alerta(null, this, "_errFileDown", "Arquivo inexistente.");
                                return;
                            }
                            Response.Redirect("xlsfile.aspx");
                        }
                    }

                    break;
            }
        }

        #endregion

        #region OnPageIndexChanging

        protected void gridLotes_OnPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            if (e != null && e.NewPageIndex > -1)
            {
                this.gridLotes.PageIndex = e.NewPageIndex;
                this.btnListarLote_Click(sender, new EventArgs());
            }
        }

        #endregion

        #endregion

        #endregion
    }
}

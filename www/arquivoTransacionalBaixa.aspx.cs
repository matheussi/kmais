namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Facade;
    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Entity.Untyped;

    public partial class arquivoTransacionalBaixa : PageBase
    {
        DataTable DTBeneficiarios
        {
            get { return Session["_atb_benef"] as DataTable; }
            set { Session["_atb_benef"] = value; }

        }

        #region Private Methods

        #region ResetGrid

        /// <summary>
        /// Reseta o Grid
        /// </summary>
        private void ResetGrid()
        {
            this.gridBeneficiario.DataSource = null;
            this.gridBeneficiario.DataBind();

            this.btnBaixa.Visible       = false;
            this.btnRegerar.Visible     = false;
            this.btnBaixarTodas.Visible = false;
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
        }

        #endregion

        #region CarregaBeneficiarios

        /// <summary>
        /// Método para Carregar os Beneficiários pendentes de uma Operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="TipoTransacao">Tipo de Transação.</param>
        private void CarregaBeneficiarios(Object OperadoraID, Object TipoTransacao, Int32 pageIndex)
        {
            if (OperadoraID != null)
            {
                DataTable dtBeneficiario = null;
                Object loteID = null;

                if (cboLotes.SelectedIndex > 0) { loteID = cboLotes.SelectedValue; }

                try
                {
                    if (Operadora.IsUnimed(OperadoraID))
                    {
                        ArqTransacionalUnimed arqTrans = new ArqTransacionalUnimed(null);
                        dtBeneficiario = arqTrans.GetBeneficiarioPorStatus(OperadoraID, traduzTipoStatusContratoBeneficiarioTipoArquivo_BAIXA(TipoTransacao.ToString(), false, true, false), null, null, loteID, DateTime.MinValue, null);
                    }
                    else
                    {
                        ArqTransacionalUnimed arqTrans = new ArqTransacionalUnimed(null);
                        if(TipoTransacao.ToString() != "0")
                            dtBeneficiario = arqTrans.GetBeneficiarioPorStatus(OperadoraID, traduzTipoStatusContratoBeneficiarioTipoArquivo_BAIXA(TipoTransacao.ToString(), false, false, true), null, null, loteID, DateTime.MinValue, null);
                        else
                            dtBeneficiario = arqTrans.GetBeneficiarioPorStatus(OperadoraID, traduzTipoStatusContratoBeneficiarioTipoArquivo_BAIXA(TipoTransacao.ToString(), false, true, false), null, null, loteID, DateTime.MinValue, null);
                    }
                }
                catch (Exception) { throw; }

                DTBeneficiarios = null;
                this.gridBeneficiario.PageIndex = pageIndex;

                if (dtBeneficiario != null && dtBeneficiario.Rows != null && dtBeneficiario.Rows.Count > 0)
                {
                    this.pnlListaBeneficiario.Visible = true;
                    this.litMessage.Visible           = false;

                    this.gridBeneficiario.DataSource = dtBeneficiario;
                    this.btnBaixa.Visible            = true;
                    this.btnRegerar.Visible          = true;
                    this.btnBaixarTodas.Visible      = true;
                    DTBeneficiarios                  = dtBeneficiario;

                    if(dtBeneficiario.Rows.Count > 1)
                        litQtdResult.Text = " (" + dtBeneficiario.Rows.Count.ToString() + " beneficiários encontrados)";
                    else
                        litQtdResult.Text = " (1 beneficiário encontrado)";
                }
                else
                {
                    litQtdResult.Text = "";
                    this.litMessage.Text = "Não há beneficiários pendentes na operadora para a operação solicitada.";

                    this.pnlListaBeneficiario.Visible = false;
                    this.litMessage.Visible           = true;

                    this.gridBeneficiario.DataSource = null;
                    this.btnBaixa.Visible            = false;
                    this.btnRegerar.Visible          = false;
                    this.btnBaixarTodas.Visible      = false;
                }

                this.gridBeneficiario.DataBind();
            }
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

        void preparaComboLote()
        {
            cboLotes.Items.Clear();
            cboLotes.Items.Add(new ListItem("TODOS", "-1"));

            if (this.DTBeneficiarios != null)
            {
                foreach (DataRow row in DTBeneficiarios.Rows)
                {
                    if (cboLotes.Items.FindByValue(Convert.ToString(row["item_lote_id"])) == null)
                    {
                        if (row["item_lote_id"] != DBNull.Value)
                        {
                            cboLotes.Items.Add(new ListItem(String.Concat("em ", Convert.ToDateTime(row["lote_data_criacao"]).ToString("dd/MM/yyyy HH:mm")), Convert.ToString(row["item_lote_id"])));
                        }
                    }
                }
            }
        }

        #endregion

        #region Page Event Handlers

        #region OnLoad

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.preparaComboLote();
                this.CarregaOperadoras();
                base.ExibirTiposDeArquivo(cboTipoTransacao, false);
            }
        }

        #endregion

        #endregion

        #region Protected Control Event Handlers

        #region DropDownList Generic Handler

        protected void genericDropDownList_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (this.cboOperadora.SelectedValue != "-1" && this.cboTipoTransacao.SelectedValue != "-1")
                this.CarregaBeneficiarios(this.cboOperadora.SelectedValue, this.cboTipoTransacao.SelectedValue, 0);
            else
                this.ResetGrid();
        }

        #endregion

        #region DropDownList Operadora

        protected void cboOperadora_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (cboLotes.Items.Count > 0) { cboLotes.SelectedIndex = 0; }
            this.genericDropDownList_OnSelectedIndexChanged(sender, e);
            this.preparaComboLote();
        }

        #endregion

        #region DropDownList Tipo Transação

        protected void cboTipoTransacao_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.genericDropDownList_OnSelectedIndexChanged(sender, e);
            this.preparaComboLote();
        }

        #endregion

        protected void cboLotes_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.genericDropDownList_OnSelectedIndexChanged(sender, e);

            //if (cboLotes.SelectedIndex > 0)
            //{
            //    DataRow[] rows = DTBeneficiarios.Select("item_lote_id=" + cboLotes.SelectedValue);
            //    gridBeneficiario.DataSource = rows;
            //    gridBeneficiario.DataBind();
            //}
            //else
            //{
            //    gridBeneficiario.DataSource = DTBeneficiarios;
            //    gridBeneficiario.DataBind();
            //}
        }

        #region Button Regerar Lote

        protected void btnRegerar_OnClick(Object sender, EventArgs e)
        {
            if (this.gridBeneficiario != null && this.gridBeneficiario.Rows != null && this.gridBeneficiario.Rows.Count > 0)
            {
                Boolean HasBeneficiarioBaixado  = false;
                Int32 intQtdBeneficiarioBaixado = 0;
                Object ContratoID               = null;
                Object BeneficiarioID           = null;
                Object ContratoBeneficiarioID   = null;

                List<Object> lstContratoID     = new List<Object>();
                List<Object> lstBeneficiarioID = new List<Object>();
                List<String> lstContrBeneficID = new List<String>();

                foreach (GridViewRow row in this.gridBeneficiario.Rows)
                {
                    Control ctrlCheck = row.Cells[0].FindControl("chkBeneficiario");

                    if (ctrlCheck != null && ctrlCheck is CheckBox)
                    {
                        if (((CheckBox)ctrlCheck).Checked)
                        {
                            HasBeneficiarioBaixado = true;
                            intQtdBeneficiarioBaixado++;

                            ContratoID             = this.gridBeneficiario.DataKeys[row.RowIndex].Values[0];
                            BeneficiarioID         = this.gridBeneficiario.DataKeys[row.RowIndex].Values[1];
                            ContratoBeneficiarioID = this.gridBeneficiario.DataKeys[row.RowIndex].Values[2];

                            lstContratoID.Add(ContratoID);
                            lstBeneficiarioID.Add(BeneficiarioID);
                            lstContrBeneficID.Add(Convert.ToString(ContratoBeneficiarioID));
                        }
                    }
                }

                if (HasBeneficiarioBaixado)
                {
                    //String strArquivoNome = null;

                    //if (Operadora.IsUnimed(cboOperadora.SelectedValue)) //UNIMED
                    //{
                    //    ArqTransacionalUnimed arqInclusao = new ArqTransacionalUnimed(Server.MapPath("/"));
                    //    arqInclusao.GerarArquivoPorStatus(this.cboOperadora.SelectedValue, ref strArquivoNome, traduzTipoStatusContratoBeneficiarioTipoArquivo(this.cboTipoTransacao.SelectedValue, true, false, false), lstContratoID.ToArray(), lstBeneficiarioID.ToArray());
                    //}
                    //else
                    //{
                    //    LayoutArquivoCustomizado.RegerarArquivo(lstContrBeneficID.ToArray());
                    //    //TODO: baixar arquivo
                    //}

                    //if (!String.IsNullOrEmpty(strArquivoNome))
                    //    this.BaixarArquivo(String.Concat(ArqTransacionalFilePath, strArquivoNome), strArquivoNome);

                    ArqTransacionalLote.DesfazerLote(lstContrBeneficID.ToArray());
                    Alerta(null, this, "_ok", "Lote(s) desfeito(s) com sucesso.\\nOs beneficiários selecionados estão disponíveis para geração de novo lote.");
                    this.CarregaBeneficiarios(this.cboOperadora.SelectedValue, this.cboTipoTransacao.SelectedValue, 0);
                    this.preparaComboLote();
                }
                else
                    base.Alerta(this.up, this, "__error_message__", "Nenhum beneficiario foi selecionada para baixa.");
            }
        }

        #endregion

        #region Button Baixa

        protected void btnBaixa_OnClick(Object sender, EventArgs e)
        {
            if (this.gridBeneficiario != null && this.gridBeneficiario.Rows != null && this.gridBeneficiario.Rows.Count > 0)
            {
                Boolean HasBeneficiarioBaixado  = false;
                Int32 intQtdBeneficiarioBaixado = 0;
                Object ContratoID               = null;
                Object BeneficiarioID           = null;

                List<String> contratoIds = null;

                foreach (GridViewRow row in this.gridBeneficiario.Rows)
                {
                    Control ctrlCheck = row.Cells[0].FindControl("chkBeneficiario");

                    if (ctrlCheck != null && ctrlCheck is CheckBox)
                    {
                        if (((CheckBox)ctrlCheck).Checked)
                        {
                            HasBeneficiarioBaixado = true;
                            intQtdBeneficiarioBaixado++;

                            ContratoID     = this.gridBeneficiario.DataKeys[row.RowIndex].Values[0];
                            BeneficiarioID = this.gridBeneficiario.DataKeys[row.RowIndex].Values[1];

                            ContratoBeneficiario.AlteraStatusBeneficiario(ContratoID, BeneficiarioID, traduzTipoStatusContratoBeneficiarioTipoArquivo(this.cboTipoTransacao.SelectedValue, false, false, true));
                            if (contratoIds == null) { contratoIds = new List<String>(); }
                            if (!contratoIds.Contains(Convert.ToString(ContratoID))) { contratoIds.Add(Convert.ToString(ContratoID)); }
                        }
                    }
                }

                //Lista de contratos para gerar ou atualizar as cobranças
                //if (contratoIds != null)
                //{
                //    ContratoFacade.Instance.GerarOuAtualizarCobrancas(contratoIds);
                //}

                if (HasBeneficiarioBaixado)
                {
                    this.CarregaBeneficiarios(this.cboOperadora.SelectedValue, this.cboTipoTransacao.SelectedValue, 0);

                    String strSuccessMessage = String.Empty;
                    String strQtdMessage     = String.Empty;

                    if (intQtdBeneficiarioBaixado == 1)
                        strQtdMessage = " beneficiario baixado.";
                    else
                        strQtdMessage = " beneficiarios baixados.";

                    base.Alerta(this.up, this, "__success_message__", String.Concat(intQtdBeneficiarioBaixado.ToString(), strQtdMessage));
                }
                else
                    base.Alerta(this.up, this, "__error_message__", "Nenhum beneficiario foi selecionada para baixa.");
            }
        }

        protected void btnBaixarTudo_OnClick(Object sender, EventArgs e)
        {
            if (this.DTBeneficiarios != null && this.DTBeneficiarios.Rows != null && this.DTBeneficiarios.Rows.Count > 0)
            {
                Boolean HasBeneficiarioBaixado = true;
                Int32 intQtdBeneficiarioBaixado = 0;
                Object ContratoID = null;
                Object BeneficiarioID = null;

                List<String> contratoIds = null;

                LC.Framework.Phantom.PersistenceManager pm = new LC.Framework.Phantom.PersistenceManager();
                pm.BeginTransactionContext();

                try
                {
                    foreach (DataRow row in this.DTBeneficiarios.Rows)
                    {
                        HasBeneficiarioBaixado = true;
                        intQtdBeneficiarioBaixado++;

                        ContratoID = row["contratobeneficiario_contratoId"];
                        BeneficiarioID = row["contratobeneficiario_beneficiarioId"];

                        ContratoBeneficiario.AlteraStatusBeneficiario(ContratoID, BeneficiarioID, traduzTipoStatusContratoBeneficiarioTipoArquivo(this.cboTipoTransacao.SelectedValue, false, false, true), pm);
                        if (contratoIds == null) { contratoIds = new List<String>(); }
                        if (!contratoIds.Contains(Convert.ToString(ContratoID))) { contratoIds.Add(Convert.ToString(ContratoID)); }
                    }

                    pm.Commit();
                }
                catch
                {
                    pm.Rollback();
                    base.Alerta(this.up, this, "__error_message__", "Houve um erro inesperado e a operação não pôde ser completada.");
                    return;
                }
                finally
                {
                    pm.Dispose();
                    pm = null;
                }

                if (HasBeneficiarioBaixado)
                {
                    this.CarregaBeneficiarios(this.cboOperadora.SelectedValue, this.cboTipoTransacao.SelectedValue, 0);
                    this.preparaComboLote();

                    String strSuccessMessage = String.Empty;
                    String strQtdMessage = String.Empty;

                    if (intQtdBeneficiarioBaixado == 1)
                        strQtdMessage = " beneficiario baixado.";
                    else
                        strQtdMessage = " beneficiarios baixados.";

                    base.Alerta(this.up, this, "__success_message__", String.Concat(intQtdBeneficiarioBaixado.ToString(), strQtdMessage));
                }
                else
                    base.Alerta(this.up, this, "__error_message__", "Nenhum beneficiario foi selecionada para baixa.");
            }
        }

        #endregion

        #region GridView Beneficiario

        #region OnRowCreated

        protected void gridBeneficiario_RowCreated(object sender, GridViewRowEventArgs e)
        {
            
        }

        #endregion

        #region OnRowDataBound

        protected void gridBeneficiario_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.DataItem != null)
            {
                Object ContratoID     = this.gridBeneficiario.DataKeys[e.Row.RowIndex].Values[0];
                Object BeneficiarioID = this.gridBeneficiario.DataKeys[e.Row.RowIndex].Values[1];

                if (ContratoID != null && BeneficiarioID != null)
                {
                    ArqTransacionalLote lote = new ArqTransacionalLote();

                    try
                    {
                        lote.CarregarPorContratoBeneficiario(ContratoID, BeneficiarioID);
                    }
                    catch (Exception) { throw; }

                    if (lote.ID != null)
                    {
                        e.Row.Cells[3].Text = lote.DataCriacao.ToString("dd/MM/yyyy HH:mm");
                        e.Row.Cells[4].Text = String.Concat(lote.Numeracao.ToString().PadLeft(3, '0'), " (", lote.ID.ToString().PadLeft(3, '0'), ")");
                        e.Row.Cells[5].Text = this.GetTipoMovimentacao(lote.TipoMovimentacao);
                    }
                }
            }
        }

        #endregion

        #region OnPageIndexChanging

        protected void gridBeneficiario_OnPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            gridBeneficiario.PageIndex = e.NewPageIndex;
            gridBeneficiario.DataSource = this.DTBeneficiarios;
            gridBeneficiario.DataBind();
            //this.CarregaBeneficiarios(this.cboOperadora.SelectedValue, this.cboTipoTransacao.SelectedValue, e.NewPageIndex);
        }

        #endregion

        #endregion

        #endregion
    }
}

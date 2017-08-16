namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Entity.Untyped;
    using LC.Web.PadraoSeguros.Entity.Filtro;
    using LC.Web.PadraoSeguros.Facade;

    public partial class manipulacaoGradeComissaoProd : PageBase
    {
        #region Private Members

        /// <summary>
        /// Data Source resultado de uma combinação de filtros.
        /// </summary>
        DataTable Produtores
        {
            get { return Cache["___dt" + Session.SessionID] as DataTable; }
            set { Cache.Remove("___dt" + Session.SessionID); if (value != null) { Cache.Insert("___dt" + Session.SessionID, value, null, DateTime.Now.AddHours(1), TimeSpan.Zero); } }
        }

        /// <summary>
        /// Tabelas de Comissionamento.
        /// </summary>
        IList<Comissionamento> tabelas
        {
            get { return Cache["___tbls" + Session.SessionID] as IList<Comissionamento>; }
            set { Cache.Remove("___tbls" + Session.SessionID); if (value != null) { Cache.Insert("___tbls" + Session.SessionID, value, null, DateTime.Now.AddHours(1), TimeSpan.Zero); } }
        }

        /// <summary>
        /// Coleção de Filtros.
        /// </summary>
        private List<ComissaoProducaoFiltro> FiltroItens
        {
            get { return Cache["___itensFiltro" + Session.SessionID] as List<ComissaoProducaoFiltro>; }
            set { Cache.Remove("___itensFiltro" + Session.SessionID); if (value != null) { Cache.Insert("___itensFiltro" + Session.SessionID, value, null, DateTime.Now.AddHours(1), TimeSpan.Zero); } }
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.Produtores  = null;
                this.FiltroItens = null;
                this.tabelas     = null;
                this.CarregaPerfis();
                this.CarregaOperadoras();
                this.CarregaTiposDeVenda();
                base.ExibirTipoDeProducao(cboTipoProducao);

                this.CarregaTabelasPorPerfil();
                this.PreencheGridCriterio();
                //this.ExibeTabelasCarregadasPorPerfil();
            }
        }

        private void PreencheGridCriterio()
        {
            List<ComissaoProducaoFiltro> lstFiltroItens = new List<ComissaoProducaoFiltro>();

            lstFiltroItens.Add(new ComissaoProducaoFiltro());
            lstFiltroItens.Add(new ComissaoProducaoFiltro());

            grid.DataSource = lstFiltroItens;
            grid.DataBind();

            this.FiltroItens = lstFiltroItens;
        }

        /// <summary>
        /// Método para Preparar o Valor de acordo com o tipo de produção.
        /// </summary>
        /// <param name="Valor">Objeto a ser Preparo.</param>
        /// <returns>Decimal para VALOR e Int32 para OUTROS.</returns>
        private Object PreparaValor(Object Valor)
        {
            if (this.cboTipoProducao.SelectedValue.Equals("VALOR"))
                return CToDecimal(Valor);
            else
                return CToInt(Valor);
        }

        /// <summary>
        /// Método para Atualizar a lista de Filtros.
        /// </summary>
        private void AtualizaFiltros()
        {
            if (grid.Rows.Count > 0)
            {
                List<ComissaoProducaoFiltro> lstFiltroItens = new List<ComissaoProducaoFiltro>();
                
                String strValorInicial, strValorFinal, strDataVigencia;
                Object objNovoValor;

                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    strValorInicial = ((TextBox)grid.Rows[i].Cells[0].FindControl("txtDe")).Text.Trim();
                    strValorFinal   = ((TextBox)grid.Rows[i].Cells[1].FindControl("txtAte")).Text.Trim();
                    objNovoValor    = ((DropDownList)grid.Rows[i].Cells[2].FindControl("cboTabela")).SelectedValue;
                    strDataVigencia = ((TextBox)grid.Rows[i].Cells[3].FindControl("txtVigencia")).Text.Trim();

                    lstFiltroItens.Add(new ComissaoProducaoFiltro(PreparaValor(strValorInicial), PreparaValor(strValorFinal), CStringToDateTime(strDataVigencia), objNovoValor));
                }

                this.FiltroItens = lstFiltroItens;
            }
        }

        private void CarregaPerfis()
        {
            cboPerfil.DataValueField = "ID";
            cboPerfil.DataTextField  = "Descricao";

            cboPerfil.DataSource = Perfil.CarregarTodos(true);
            cboPerfil.DataBind();
        }

        private void CarregaOperadoras()
        {
            lstOperadora.DataValueField = "ID";
            lstOperadora.DataTextField  = "Nome";
            lstOperadora.DataSource = Operadora.CarregarTodas(true);
            lstOperadora.DataBind();
        }

        private void CarregaTiposDeVenda()
        {
            lstTipoVenda.DataValueField = "ID";
            lstTipoVenda.DataTextField  = "Descricao";
            lstTipoVenda.DataSource = TipoContrato.Carregar(true);
            lstTipoVenda.DataBind();
        }

        private void CarregaTabelasPorPerfil()
        {
            IList<Comissionamento> lista = null;
            lista = Comissionamento.CarregarTodos(Comissionamento.eTipo.PagoAoOperador, cboPerfil.SelectedValue);
            tabelas = lista;
        }

        private void ExibeTabelasCarregadasPorPerfil()
        {
            if (grid.Rows.Count > 0)
            {
                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    DropDownList combo = ((DropDownList)grid.Rows[i].Cells[2].Controls[1]);
                    combo.DataValueField = "ID";
                    combo.DataTextField  = "Descricao";
                    combo.Items.Clear();
                    combo.DataSource = tabelas;
                    combo.DataBind();
                }
            }
        }

        private void ExibeTabelasCarregadasPorPerfil(ref DropDownList DropDown)
        {
            this.ExibeTabelasCarregadasPorPerfil(ref DropDown, null);
        }

        private void ExibeTabelasCarregadasPorPerfil(ref DropDownList DropDown, String SelectedValue)
        {
            if (DropDown != null)
            {
                DropDown.DataValueField = "ID";
                DropDown.DataTextField  = "Descricao";
                DropDown.Items.Clear();

                if (!String.IsNullOrEmpty(SelectedValue))
                {
                    foreach (Comissionamento tabelaComissao in this.tabelas)
                    {
                        DropDown.Items.Add(new ListItem(tabelaComissao.Descricao, tabelaComissao.ID.ToString()));

                        if (tabelaComissao.ID.ToString().Equals(SelectedValue))
                            DropDown.SelectedValue = SelectedValue;
                    }
                }
                else
                {
                    DropDown.DataSource = tabelas;
                    DropDown.DataBind();
                }
            }
        }

        #region ComboBox Perfil

        protected void cboPerfil_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelasPorPerfil();
            this.ExibeTabelasCarregadasPorPerfil();
        }

        #endregion

        #region ComboBox TipoProducao

        protected void cboTipoProducao_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (grid.Rows.Count > 0)
            {
                if (this.cboTipoProducao.SelectedValue.Equals("VALOR"))
                {
                    for (int i = 0; i < grid.Rows.Count; i++)
                    {
                        ((TextBox)grid.Rows[i].Cells[0].Controls[1]).Attributes.Add("onkeyup", "mascara('" + ((TextBox)grid.Rows[i].Cells[0].Controls[1]).ClientID + "')");
                        ((TextBox)grid.Rows[i].Cells[1].Controls[1]).Attributes.Add("onkeyup", "mascara('" + ((TextBox)grid.Rows[i].Cells[1].Controls[1]).ClientID + "')");

                        ((TextBox)grid.Rows[i].Cells[0].Controls[1]).Attributes.Remove("onkeypress");
                        ((TextBox)grid.Rows[i].Cells[1].Controls[1]).Attributes.Remove("onkeypress");
                    }
                }
                else
                {
                    for (int i = 0; i < grid.Rows.Count; i++)
                    {
                        
                        ((TextBox)grid.Rows[i].Cells[0].Controls[1]).Attributes.Remove("onkeyup");
                        ((TextBox)grid.Rows[i].Cells[1].Controls[1]).Attributes.Remove("onkeyup");

                        ((TextBox)grid.Rows[i].Cells[0].Controls[1]).Attributes.Add("onkeypress", "filtro_SoNumeros_AllBrowser(event);");
                        ((TextBox)grid.Rows[i].Cells[1].Controls[1]).Attributes.Add("onkeypress", "filtro_SoNumeros_AllBrowser(event);");
                    }
                }
            }
        }

        #endregion

        #region GridView Filtro

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                this.FiltroItens.RemoveAt(Convert.ToInt32(e.CommandArgument));
                grid.DataSource = this.FiltroItens;
                grid.DataBind();
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 4, "Excluir a linha?");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (this.cboTipoProducao.SelectedValue.Equals("VALOR"))
                {
                    ((TextBox)e.Row.Cells[0].Controls[1]).Attributes.Add("onkeyup", "mascara('" + ((TextBox)e.Row.Cells[0].Controls[1]).ClientID + "')");
                    ((TextBox)e.Row.Cells[1].Controls[1]).Attributes.Add("onkeyup", "mascara('" + ((TextBox)e.Row.Cells[1].Controls[1]).ClientID + "')");

                    ((TextBox)e.Row.Cells[0].Controls[1]).Attributes.Remove("onkeypress");
                    ((TextBox)e.Row.Cells[1].Controls[1]).Attributes.Remove("onkeypress");
                }
                else
                {
                    ((TextBox)e.Row.Cells[0].Controls[1]).Attributes.Remove("onkeyup");
                    ((TextBox)e.Row.Cells[1].Controls[1]).Attributes.Remove("onkeyup");

                    ((TextBox)e.Row.Cells[0].Controls[1]).Attributes.Add("onkeypress", "filtro_SoNumeros_AllBrowser(event);");
                    ((TextBox)e.Row.Cells[1].Controls[1]).Attributes.Add("onkeypress", "filtro_SoNumeros_AllBrowser(event);");
                }

                ComissaoProducaoFiltro filtro = (ComissaoProducaoFiltro)e.Row.DataItem;

                TextBox txtDe              = (TextBox)e.Row.Cells[0].FindControl("txtDe");
                TextBox txtAte             = (TextBox)e.Row.Cells[0].FindControl("txtAte");
                TextBox txtVigencia        = (TextBox)e.Row.Cells[0].FindControl("txtVigencia");
                DropDownList ddlNovaTabela = (DropDownList)e.Row.Cells[2].FindControl("cboTabela"); 

                if (filtro.IntervaloInicial != null)
                    txtDe.Text  = filtro.IntervaloInicial.ToString();

                if (filtro.IntervaloFinal != null)
                    txtAte.Text = filtro.IntervaloFinal.ToString();

                if (filtro.Vigencia.CompareTo(DateTime.MinValue) > 0)
                    txtVigencia.Text = filtro.Vigencia.ToString("dd/MM/yyyy");

                ddlNovaTabela.DataValueField = "ID";
                ddlNovaTabela.DataTextField  = "Descricao";

                ddlNovaTabela.DataSource = this.tabelas;
                ddlNovaTabela.DataBind();

                if (filtro.NovoValor != null)
                    ddlNovaTabela.SelectedValue = filtro.NovoValor.ToString();
                else
                    ddlNovaTabela.SelectedIndex = 0;
            }
        }

        #endregion

        #region GridView Equipe

        #region OnRowCommand

        protected void gridEquipe_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                this.Produtores.Rows.RemoveAt(Convert.ToInt32(e.CommandArgument));
                this.gridEquipe.DataSource = this.Produtores;
                this.gridEquipe.DataBind();

                if (this.Produtores.Rows.Count == 0) { tblAtribuicao.Visible = false; }
            }
        }

        #endregion

        #region OnRowDataBound

        protected void gridEquipe_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 4, "Confirma a exclusão?");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItem != null)
                {
                    DataRowView rowDataItem = (DataRowView)e.Row.DataItem;

                    Int32 intQtdeRowColIndex                  = 3;
                    Int32 intDropDownComissionamentoCellIndex = 2;
                    Int32 intTextBoxVigenciaCellIndex         = 3;

                    Int32 intQtde = UIHelper.CToInt(rowDataItem.Row[intQtdeRowColIndex]);

                    String strNovoValor = String.Empty;
                    String strVigencia  = String.Empty;

                    foreach (ComissaoProducaoFiltro filtro in this.FiltroItens)
                    {
                        if (filtro.CheckValue(intQtde))
                        {
                            strNovoValor = filtro.NovoValor.ToString();
                            strVigencia  = filtro.Vigencia.ToString("dd/MM/yyyy");

                            break;
                        }
                    }

                    DropDownList ddlComissionamento = (DropDownList)e.Row.Cells[intDropDownComissionamentoCellIndex].FindControl("cboTabela");
                    TextBox txtVigencia             = (TextBox)e.Row.Cells[intTextBoxVigenciaCellIndex].FindControl("txtVigencia");

                    this.ExibeTabelasCarregadasPorPerfil(ref ddlComissionamento, strNovoValor);
                    txtVigencia.Text = strVigencia;
                }
            }
        }

        #endregion

        #endregion

        #region Button AddItem

        protected void cmdAddItemCom_Click(Object sender, EventArgs e)
        {
            this.AtualizaFiltros();

            List<ComissaoProducaoFiltro> lstFiltroItens = this.FiltroItens;
            
            if (lstFiltroItens == null) 
                lstFiltroItens = new List<ComissaoProducaoFiltro>();

            lstFiltroItens.Add(new ComissaoProducaoFiltro());
            
            this.FiltroItens = lstFiltroItens;

            grid.DataSource = this.FiltroItens;
            grid.DataBind();
        }

        #endregion

        #region Button Exibir

        protected void cmdExibir_Click(object sender, EventArgs e)
        {
            #region Validation

            DateTime dtFrom              = new DateTime();
            DateTime dtTo                = new DateTime();
            List<Object> lstOperadora    = new List<Object>(this.lstOperadora.Items.Count);
            List<Object> lstTipoContrato = new List<Object>(this.lstTipoVenda.Items.Count);

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

            // CHECAGEM DO RANGE DAS DATAS

            if (dtFrom.CompareTo(dtTo) > 0)
            {
                base.Alerta(null, this, "_err3", "Data Inicial é maior que a Data Final.");
                return;
            }

            // OPERADORAS

            Boolean bolHasOperadora = false;

            for (Int32 i = 0; i < this.lstOperadora.Items.Count; i++)
            {
                if (this.lstOperadora.Items[i].Selected)
                {
                    lstOperadora.Add(this.lstOperadora.Items[i].Value);
                    bolHasOperadora = true;
                }
            }

            if (!bolHasOperadora)
            {
                base.Alerta(null, this, "_err4", "Você deve selecionar uma operadora a menos.");
                return;
            }

            Object perfilID = this.cboPerfil.SelectedValue;

            // TIPOS DE CONTRATO (VENDA)

            Boolean bolHasTipoContrato = false;

            for (Int32 i = 0; i < this.lstTipoVenda.Items.Count; i++)
            {
                if (this.lstTipoVenda.Items[i].Selected)
                {
                    lstTipoContrato.Add(this.lstTipoVenda.Items[i].Value);
                    bolHasTipoContrato = true;
                }
            }

            if (!bolHasTipoContrato)
            {
                base.Alerta(null, this, "_err5", "Você deve selecionar um tipo de venda pelo menos.");
                return;
            }

            if (this.FiltroItens == null || this.FiltroItens.Count == 0)
            {
                base.Alerta(null, this, "_err6", "Não existe nenhum critério definido.");
                return;
            }
            else
            {
                String result = UIHelper.ChecaGridIntervalosInterios(grid, 0, 1);

                if (!String.IsNullOrEmpty(result))
                {
                    Alerta(null, this, "_errInterv", result);
                    return;
                }
                else
                {
                    this.AtualizaFiltros();
                }
            }

            #endregion

            this.Produtores = null;

            switch (this.cboTipoProducao.SelectedValue)
            {
                case "CONTRATOS":
                    this.Produtores = UntypedProcesses.GetProdutoresByContratoRank(dtFrom, dtTo, lstOperadora.ToArray(), perfilID, lstTipoContrato.ToArray(), this.FiltroItens);
                    break;
                case "VALOR":
                    this.Produtores = UntypedProcesses.GetProdutoresByValorRank(dtFrom, dtTo, lstOperadora.ToArray(), perfilID, lstTipoContrato.ToArray(), this.FiltroItens);
                    break;
                case "VIDAS":
                    this.Produtores = UntypedProcesses.GetProdutoresByBenRank(dtFrom, dtTo, lstOperadora.ToArray(), perfilID, lstTipoContrato.ToArray(), this.FiltroItens);
                    break;
            }

            this.gridEquipe.DataSource = this.Produtores;
            this.gridEquipe.DataBind();

            if (this.Produtores != null && this.Produtores.Rows.Count > 0)
                tblAtribuicao.Visible = true;
            else
                tblAtribuicao.Visible = false;
        }

        #endregion

        #region Button Atribuir

        protected void cmdAtribuir_Click(Object sender, EventArgs e)
        {
            if (this.gridEquipe.Rows != null && this.gridEquipe.Rows.Count > 0)
            {
                Object objNovaTabela                       = null;
                String strVigencia                         = null;
                Object objUsuarioID                        = null;
                Object objPerfilID                         = null;
                DateTime dtVigencia                        = DateTime.MinValue;

                List<ComissionamentoUsuario> lstProdutores = new List<ComissionamentoUsuario>(this.gridEquipe.Rows.Count);
                List<Object> lstFailUsuarioID              = new List<Object>();

                for (Int32 i = 0; i < this.gridEquipe.Rows.Count; i++)
                {
                    objUsuarioID  = this.gridEquipe.DataKeys[i].Value;
                    objPerfilID   = Usuario.CarregarPerfilID(objUsuarioID);
                    objNovaTabela = ((DropDownList)this.gridEquipe.Rows[i].Cells[2].FindControl("cboTabela")).SelectedValue;
                    strVigencia   = ((TextBox)this.gridEquipe.Rows[i].Cells[3].FindControl("txtVigencia")).Text.Trim();

                    if (objUsuarioID != null && objPerfilID != null && objNovaTabela != null && !String.IsNullOrEmpty(strVigencia))
                    {

                        if (UIHelper.TyParseToDateTime(strVigencia, out dtVigencia))
                        {
                            ComissionamentoUsuario comissaoUsuario = new ComissionamentoUsuario();

                            comissaoUsuario.UsuarioID                 = objUsuarioID;
                            comissaoUsuario.PerfilID                  = objPerfilID;
                            comissaoUsuario.TabelaComissionamentoID   = objNovaTabela;
                            comissaoUsuario.TabelaComissionamentoData = dtVigencia;

                            lstProdutores.Add(comissaoUsuario);
                        }
                        else
                            lstFailUsuarioID.Add(objUsuarioID);
                    }
                    else
                        lstFailUsuarioID.Add(objUsuarioID);
                }

                try
                {
                    ComissionamentoUsuario.Salvar(lstProdutores);
                }
                catch (Exception)
                {
                    base.Alerta(null, this.Page, "_err1", "Houve um problema na atribuição de novas tabelas. Por favor tente novamente.");
                    return;
                }

                if (lstFailUsuarioID.Count > 0)
                {
                    Boolean removeBecauseNotFail;
                    List<DataRow> lstNotFailRow = new List<DataRow>();

                    foreach (DataRow row in this.Produtores.Rows)
                    {
                        removeBecauseNotFail = false;

                        for (Int32 j = 0; j < lstFailUsuarioID.Count; j++)
                            if (CToInt(row[0]) != CToInt(lstFailUsuarioID[j]))
                                removeBecauseNotFail = true;
                            else
                            {
                                removeBecauseNotFail = false;
                                break;
                            }

                        if (removeBecauseNotFail)
                            lstNotFailRow.Add(row);
                    }

                    foreach (DataRow row in lstNotFailRow)
                        this.Produtores.Rows.Remove(row);

                    this.gridEquipe.DataSource = this.Produtores;
                    this.gridEquipe.DataBind();

                    base.Alerta(null, this.Page, "_err2", "Existem atribuições que estão com inconsistências. Favor verificar e tentar novamente.");
                }
                else
                {
                    this.gridEquipe.DataSource = null;
                    this.gridEquipe.DataBind();

                    base.Alerta(null, this.Page, "_err2", "Atribuições realizadas com sucesso.");
                }
            }
        }

        #endregion
    }
}
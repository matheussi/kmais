namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Util;
    using LC.Web.PadraoSeguros.Facade;
    using LC.Web.PadraoSeguros.Entity;

    public partial class tabelavalorPoup : PageBase
    {
        List<TabelaValorItem> ItensTValores
        {
            get
            {
                if (Session["_itenstv"] != null)
                {
                    List<TabelaValorItem> lista = Session["_itenstv"] as List<TabelaValorItem>;

                    for (int i = 0; i < gridTabelaValoresItemDETALHE.Rows.Count; i++)
                    {
                        lista[i].IdadeFim = base.CToInt(((TextBox)gridTabelaValoresItemDETALHE.Rows[i].Cells[1].Controls[1]).Text);
                        lista[i].IdadeInicio = base.CToInt(((TextBox)gridTabelaValoresItemDETALHE.Rows[i].Cells[0].Controls[1]).Text);

                        lista[i].QCValor = base.CToDecimal(((TextBox)gridTabelaValoresItemDETALHE.Rows[i].Cells[4].Controls[1]).Text);
                        lista[i].QPValor = base.CToDecimal(((TextBox)gridTabelaValoresItemDETALHE.Rows[i].Cells[5].Controls[1]).Text);

                        lista[i].QCValorPagamento = base.CToDecimal(((TextBox)gridTabelaValoresItemDETALHE.Rows[i].Cells[2].Controls[1]).Text);
                        lista[i].QPValorPagamento = base.CToDecimal(((TextBox)gridTabelaValoresItemDETALHE.Rows[i].Cells[3].Controls[1]).Text);

                        lista[i].ID = gridTabelaValoresItemDETALHE.DataKeys[i].Value;

                        if (!String.IsNullOrEmpty(gridTabelaValoresItemDETALHE.Rows[i].Cells[5].Text))
                            lista[i].TabelaID = gridTabelaValoresItemDETALHE.Rows[i].Cells[5].Text;
                    }

                    return lista;
                }
                else
                    return null;
            }
            set { Session["_itenstv"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);
                if (!String.IsNullOrEmpty(Request[IDKey])) { cboOperadora.SelectedValue = Request[IDKey]; }
                this.CarregaContratos();
                if (!String.IsNullOrEmpty(Request[IDKey2])) { cboTValoresContrato.SelectedValue = Request[IDKey2]; }
                cboTValoresContrato_OnSelectedIndexChanged(null, null);


                ///////////////////////
                cboTValoresContratoDETALHE.Enabled = false;
                TabelaValor tabela = new TabelaValor();
                gridTabelaValores.SelectedIndex = Convert.ToInt32(0);
                tabela.ID = gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value;
                tabela.Carregar();

                cboTValoresContratoDETALHE.SelectedValue = Convert.ToString(tabela.ContratoID);
                cboTValoresContratoDETALHE_OnSelectedIndexChanged(null, null);

                this.CarregaPlanosParaTabelaDeValor();
                pnlTVPlanos.Visible = gridTVPlanos.Rows.Count > 0;

                pnlTabelaValorLista.Visible = false;
                pnlTabelaValorDetalhe.Visible = true;
                litTVSemPlano.Text = "";

                if (!String.IsNullOrEmpty(Request[IDKey3]))
                {
                    for (int i = 0; i < gridTVPlanos.Rows.Count; i++)
                    {
                        if (Convert.ToString(gridTVPlanos.DataKeys[i][0]) == Request[IDKey3])
                        {
                            pnlTabelaValorITENS.Visible = true;
                            gridTVPlanos.SelectedIndex = Convert.ToInt32(i);

                            //carrega os itens
                            if (gridTabelaValores.SelectedIndex > -1)
                            {
                                this.CarregaItensDaTabelaDeValores();

                            }
                            else
                            {
                                Plano plano = new Plano(gridTVPlanos.DataKeys[gridTVPlanos.SelectedIndex].Value);
                                plano.Carregar();
                                gridTabelaValoresItemDETALHE.Columns[2].Visible = false; // plano.QuartoComum;
                                gridTabelaValoresItemDETALHE.Columns[3].Visible = false; // plano.QuartoParticular;
                                gridTabelaValoresItemDETALHE.Columns[4].Visible = plano.QuartoComum;
                                gridTabelaValoresItemDETALHE.Columns[5].Visible = plano.QuartoParticular;
                            }
                            break;
                        }
                    }
                }
                ///////////////////////
            }
        }

        void CarregaContratos()
        {
            IList<ContratoADM> lista = ContratoADM.Carregar(cboOperadora.SelectedValue);

            cboTValoresContrato.DataValueField = "ID";
            cboTValoresContrato.DataTextField = "Descricao";
            cboTValoresContrato.DataSource = lista;
            cboTValoresContrato.DataBind();

            cboTValoresContratoDETALHE.DataValueField = "ID";
            cboTValoresContratoDETALHE.DataTextField = "Descricao";
            cboTValoresContratoDETALHE.DataSource = lista;
            cboTValoresContratoDETALHE.DataBind();
        }

        #region Tabelas de Valores

        protected void cboOperadora_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
        }

        protected void cboTValoresContrato_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelaValores();
        }

        void CarregaPlanosParaTabelaDeValor()
        {
            IList<Plano> lista = Plano.CarregarPorContratoID(cboTValoresContrato.SelectedValue, true);
            gridTVPlanos.DataSource = lista;
            gridTVPlanos.DataBind();
            pnlTVPlanos.Visible = (lista != null && lista.Count > 0);
            if (!pnlTVPlanos.Visible) { litTVSemPlano.Text = "<font size='1' color='red'>nenhum plano</font>"; }
            else { litTVSemPlano.Text = ""; }
        }

        void CarregaTabelaValores()
        {
            if (!base.HaItemSelecionado(cboTValoresContrato))
            {
                gridTabelaValores.DataSource = null;
                gridTabelaValores.DataBind();
                //litNenhumaTabelaValor.Text = " (nenhuma)";
                return;
            }

            gridTabelaValores.DataSource = TabelaValor.CarregarPorContratoID(cboTValoresContrato.SelectedValue);
            gridTabelaValores.DataBind();
            cmdOcultarTValoresDetalhes_Click(null, null);
        }

        void CarregaTabelaValoresITENS()
        {
        }

        protected void gridTabelaValores_OnRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                cboTValoresContratoDETALHE.Enabled = false;
                TabelaValor tabela = new TabelaValor();
                gridTabelaValores.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                tabela.ID = gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value;
                tabela.Carregar();

                cboTValoresContratoDETALHE.SelectedValue = Convert.ToString(tabela.ContratoID);
                cboTValoresContratoDETALHE_OnSelectedIndexChanged(null, null);

                this.CarregaPlanosParaTabelaDeValor();
                pnlTVPlanos.Visible = gridTVPlanos.Rows.Count > 0;

                pnlTabelaValorLista.Visible = false;
                pnlTabelaValorDetalhe.Visible = true;
                litTVSemPlano.Text = "";


                pnlTabelaValorITENS.Visible = true;
                pnlTabelaValorITENS_ListaPlanos.Visible = false;
            }
            else if (e.CommandName == "duplicar")
            {
                litTabelaValorDuplicarIDs.Text = Convert.ToString(gridTabelaValores.DataKeys[Convert.ToInt32(e.CommandArgument)].Value) + ";" + cboTValoresContrato.SelectedValue; ;
                pnlTabelaValorLista.Visible = false;
                pnlTabelaValorDuplicar.Visible = true;
                txtTValoresInicioDUPLICAR.Focus();
            }
            else if (e.CommandName.Equals("excluir"))
            {
                TabelaValor tabela = new TabelaValor(gridTabelaValores.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                tabela.Remover();
                this.CarregaTabelaValores();
            }
            else if (e.CommandName.Equals("recalcular"))
            {
                TabelaValor tabela = new TabelaValor(gridTabelaValores.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                tabela.Carregar();

                DateTime vigencia, vencimentoDe, vencimentoAte = DateTime.MinValue;
                int diaSemJuros = 0;
                Object valorLimite = null;
                CalendarioVencimento rcv = null;

                CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(tabela.ContratoID,
                    tabela.Inicio, out vigencia, out vencimentoDe, out diaSemJuros, out valorLimite, out rcv, null);

                CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(tabela.ContratoID,
                    tabela.Fim, out vigencia, out vencimentoAte, out diaSemJuros, out valorLimite, out rcv, null);

                if (vencimentoDe != DateTime.MinValue)
                    tabela.VencimentoInicio = vencimentoDe;
                else
                    tabela.VencimentoInicio = tabela.Inicio;

                if (vencimentoAte != DateTime.MinValue)
                    tabela.VencimentoFim = vencimentoAte;
                else
                    tabela.VencimentoFim = tabela.Fim;

                tabela.Salvar();
                this.CarregaTabelaValores();
            }
        }

        protected void gridTabelaValores_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja realmente excluir?");
            base.grid_RowDataBound_Confirmacao(sender, e, 9, "Deseja realmente duplicar essa tabela?");
            base.grid_RowDataBound_Confirmacao(sender, e, 11, "Recalcular limites de vencimento para essa tabela?");
        }

        protected void cmdOcultarTValoresDetalhes_Click(Object sender, EventArgs e)
        {
            gridTabelaValores.SelectedIndex = -1;
        }

        #region DUPLICAR

        protected void cmdTVDuplicarFechar_Click(Object sender, EventArgs e)
        {
            litTabelaValorDuplicarIDs.Text = "";
            pnlTabelaValorDuplicar.Visible = false;
            pnlTabelaValorLista.Visible = true;
        }

        protected void cmdDuplicarTabelaValor_Click(Object sender, EventArgs e)
        {
        }

        #endregion

        //DETALHE

        /// <summary>
        /// Exibe dados pré-preenchidos
        /// </summary>
        List<TabelaValorItem> GradeCompleta()
        {
            List<TabelaValorItem> lista = new List<TabelaValorItem>();
            TabelaValorItem item = new TabelaValorItem();
            item.IdadeInicio = 0;
            item.IdadeFim = 18;
            lista.Add(item);

            item = new TabelaValorItem();
            item.IdadeInicio = 19;
            item.IdadeFim = 23;
            lista.Add(item);

            item = new TabelaValorItem();
            item.IdadeInicio = 24;
            item.IdadeFim = 28;
            lista.Add(item);

            item = new TabelaValorItem();
            item.IdadeInicio = 29;
            item.IdadeFim = 33;
            lista.Add(item);

            item = new TabelaValorItem();
            item.IdadeInicio = 34;
            item.IdadeFim = 38;
            lista.Add(item);

            item = new TabelaValorItem();
            item.IdadeInicio = 39;
            item.IdadeFim = 43;
            lista.Add(item);

            item = new TabelaValorItem();
            item.IdadeInicio = 44;
            item.IdadeFim = 48;
            lista.Add(item);

            item = new TabelaValorItem();
            item.IdadeInicio = 49;
            item.IdadeFim = 53;
            lista.Add(item);

            item = new TabelaValorItem();
            item.IdadeInicio = 54;
            item.IdadeFim = 58;
            lista.Add(item);

            item = new TabelaValorItem();
            item.IdadeInicio = 59;
            item.IdadeFim = 200;
            lista.Add(item);

            return lista;
        }

        protected void gridTVPlanos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selecionar"))
            {
                pnlTabelaValorITENS.Visible = true;
                gridTVPlanos.SelectedIndex = Convert.ToInt32(e.CommandArgument);

                //carrega os itens
                if (gridTabelaValores.SelectedIndex > -1)
                {
                    this.CarregaItensDaTabelaDeValores();

                }
                else
                {
                    Plano plano = new Plano(gridTVPlanos.DataKeys[gridTVPlanos.SelectedIndex].Value);
                    plano.Carregar();
                    gridTabelaValoresItemDETALHE.Columns[2].Visible = false; // plano.QuartoComum;
                    gridTabelaValoresItemDETALHE.Columns[3].Visible = false; // plano.QuartoParticular;
                    gridTabelaValoresItemDETALHE.Columns[4].Visible = plano.QuartoComum;
                    gridTabelaValoresItemDETALHE.Columns[5].Visible = plano.QuartoParticular;
                }
            }
        }

        Boolean ChecaDataDeContratoETabelaDeValor()
        {
            //if (!base.HaItemSelecionado(cboTValoresContratoDETALHE))
            //{
            //    //base.Alerta(MPE, ref litAlert, "Informe o contrato da tabela de valores.", upnlAlerta);
            //    cboTValoresContratoDETALHE.Focus();
            //    cmdSalvarTabelaValor.Enabled = false;
            //    return false;
            //}

            //Plano plano = new Plano(gridTVPlanos.DataKeys[gridTVPlanos.SelectedIndex].Value);
            //plano.Carregar();

            //ContratoADM contrato = new ContratoADM(plano.ContratoID);
            //contrato.Carregar();

            //txtTValoresInicio.Text = contrato.Data.ToString("dd/MM/yyyy");

            //Object tabelaSelecionadaID = null;
            //if (gridTabelaValores.SelectedIndex > -1)
            //    tabelaSelecionadaID = gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value;

            //IList<TabelaValor> tabelas = TabelaValor.CarregarPorContratoID(cboTValoresContrato.SelectedValue);
            //if (tabelas == null || tabelas.Count == 0)
            //{
            //    txtTValoresInicio.ReadOnly = true;
            //    cmdSalvarTabelaValor.Enabled = true;
            //    return true;
            //}

            //if (tabelas.Count == 1 && tabelaSelecionadaID != null && Convert.ToString(tabelaSelecionadaID) == Convert.ToString(tabelas[0].ID))
            //    txtTValoresInicio.ReadOnly = true; //txtTValoresData.ReadOnly = true; denis
            //else
            //    txtTValoresInicio.ReadOnly = false; //txtTValoresData.ReadOnly = false; denis

            //cmdSalvarTabelaValor.Enabled = true;
            return true;
        }

        protected void cmdAddItemTabelaValor_Click(Object sender, EventArgs e)
        {
            List<TabelaValorItem> lista = this.ItensTValores;
            if (lista == null) { lista = new List<TabelaValorItem>(); }

            Int32 idadeinicio = 0;

            if (lista.Count > 0)
            {
                idadeinicio = lista[lista.Count - 1].IdadeFim + 1;
            }

            lista.Add(new TabelaValorItem());

            gridTabelaValoresItemDETALHE.DataSource = lista;
            gridTabelaValoresItemDETALHE.DataBind();
            this.ItensTValores = lista;

            if (lista.Count > 0)
            {
                ((TextBox)gridTabelaValoresItemDETALHE.Rows[lista.Count - 1].Cells[0].Controls[1]).Text = idadeinicio.ToString();
            }

            ((TextBox)gridTabelaValoresItemDETALHE.Rows[lista.Count - 1].Cells[1].Controls[1]).Focus();

            if (lista.Count == 1)
            {
                lista = this.GradeCompleta();
                gridTabelaValoresItemDETALHE.DataSource = lista;
                gridTabelaValoresItemDETALHE.DataBind();
                this.ItensTValores = lista;
            }
        }

        void CarregaItensDaTabelaDeValores()
        {
            List<TabelaValorItem> lista = (List<TabelaValorItem>)
                TabelaValorItem.CarregarPorTabela(
                gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value,
                gridTVPlanos.DataKeys[gridTVPlanos.SelectedIndex].Value);

            gridTabelaValoresItemDETALHE.DataSource = lista;
            gridTabelaValoresItemDETALHE.DataBind();
            this.ItensTValores = lista;

            Plano plano = new Plano();

            pnlTabelaValorITENS_ListaPlanos.Visible = true;
            TabelaValor tv = new TabelaValor();
            tv.ID = gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value;
            tv.Carregar();
            plano.ID = gridTVPlanos.DataKeys[gridTVPlanos.SelectedIndex].Value;
            plano.Carregar();

            gridTabelaValoresItemDETALHE.Columns[2].Visible = false; // plano.QuartoComum;
            gridTabelaValoresItemDETALHE.Columns[3].Visible = false; // plano.QuartoParticular;
            gridTabelaValoresItemDETALHE.Columns[4].Visible = plano.QuartoComum;
            gridTabelaValoresItemDETALHE.Columns[5].Visible = plano.QuartoParticular;
        }

        void MontaGridParaPrimeiraInsercao_TabelaValoresItens()
        {
            List<TabelaValorItem> lista = new List<TabelaValorItem>();
            for (int i = 1; i <= 1; i++)
            {
                lista.Add(new TabelaValorItem());
            }

            gridTabelaValoresItemDETALHE.DataSource = lista;
            gridTabelaValoresItemDETALHE.DataBind();
            this.ItensTValores = lista;
        }

        protected void gridTabelaValoresItemDETALHE_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("calculo"))
            {
                Object id = gridTabelaValoresItemDETALHE.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                Boolean calculo = Convert.ToBoolean(gridTabelaValoresItemDETALHE.DataKeys[Convert.ToInt32(e.CommandArgument)][1]);

                TabelaValorItem item = new TabelaValorItem();
                item.ID = id;
                item.Carregar();

                item.CalculoAutomatico = !calculo;
                item.Salvar();
                this.CarregaItensDaTabelaDeValores();
            }
            else if (e.CommandName.Equals("excluir"))
            {
                Object id = gridTabelaValoresItemDETALHE.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                if (id == null)
                {
                    List<TabelaValorItem> lista = this.ItensTValores;
                    lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                    this.ItensTValores = lista;
                    gridTabelaValoresItemDETALHE.DataSource = lista;
                    gridTabelaValoresItemDETALHE.DataBind();
                }
                else
                {
                    TabelaValorItem item = new TabelaValorItem();
                    item.ID = id;
                    item.Carregar();
                    item.Remover();
                    this.CarregaItensDaTabelaDeValores();
                }
            }
        }

        protected void gridTabelaValoresItemDETALHE_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente excluir a linha?");

                TextBox txtvalor1 = (TextBox)e.Row.Cells[2].Controls[1];
                txtvalor1.Attributes.Add("onKeyUp", "mascara('" + txtvalor1.ClientID + "')");
                TextBox txtvalor2 = (TextBox)e.Row.Cells[3].Controls[1];
                txtvalor2.Attributes.Add("onKeyUp", "mascara('" + txtvalor2.ClientID + "')");

                ((TextBox)e.Row.Cells[4].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[4].Controls[1]).ClientID + "')");
                ((TextBox)e.Row.Cells[5].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[5].Controls[1]).ClientID + "')");

                Object id = gridTabelaValoresItemDETALHE.DataKeys[e.Row.RowIndex].Value;

                if (id == null)
                {
                    if (CToDecimal(txtvalor1.Text) == 0) { txtvalor1.Text = ""; }
                    if (CToDecimal(txtvalor2.Text) == 0) { txtvalor2.Text = ""; }
                }

                if (CToDecimal(((TextBox)e.Row.Cells[4].Controls[1]).Text) == 0) { ((TextBox)e.Row.Cells[4].Controls[1]).Text = ""; }
                if (CToDecimal(((TextBox)e.Row.Cells[5].Controls[1]).Text) == 0) { ((TextBox)e.Row.Cells[5].Controls[1]).Text = ""; }

                if (Convert.ToBoolean(gridTabelaValoresItemDETALHE.DataKeys[e.Row.RowIndex][1]))
                {
                    ((ImageButton)e.Row.Cells[8].Controls[0]).ImageUrl = "~/images/cadeado.png";
                    ((ImageButton)e.Row.Cells[8].Controls[0]).ToolTip = "cálculo automático";

                    ((TextBox)e.Row.Cells[4].Controls[1]).ReadOnly = true;
                    ((TextBox)e.Row.Cells[4].Controls[1]).BackColor = System.Drawing.Color.FromName("lightgray");
                    ((TextBox)e.Row.Cells[5].Controls[1]).ReadOnly = true;
                    ((TextBox)e.Row.Cells[5].Controls[1]).BackColor = System.Drawing.Color.FromName("lightgray");
                }
                else
                {
                    ((ImageButton)e.Row.Cells[8].Controls[0]).ImageUrl = "~/images/cadeado_aberto.png";
                    ((ImageButton)e.Row.Cells[8].Controls[0]).ToolTip = "cálculo manual";

                    ((TextBox)e.Row.Cells[4].Controls[1]).ReadOnly = false;
                    ((TextBox)e.Row.Cells[4].Controls[1]).BackColor = System.Drawing.Color.FromName("white");
                    ((TextBox)e.Row.Cells[5].Controls[1]).ReadOnly = false;
                    ((TextBox)e.Row.Cells[5].Controls[1]).BackColor = System.Drawing.Color.FromName("white");
                }
            }
        }

        protected void cmdNovaTabelaValor_Click(Object sender, EventArgs e)
        {
            //cboTValoresContratoDETALHE.Enabled = true;
            //gridTabelaValores.SelectedIndex = -1;
            //pnlTabelaValorLista.Visible = false;
            //pnlTabelaValorDetalhe.Visible = true;
            //this.MontaGridParaPrimeiraInsercao_TabelaValoresItens();
            //txtTValoresFim.Text = "";
            //txtTValoresInicio.Text = "";

            //this.CarregaPlanosParaTabelaDeValor();
            //pnlTVPlanos.Visible = gridTVPlanos.Rows.Count > 0;

            //pnlTabelaValorLista.Visible = false;
            //pnlTabelaValorDetalhe.Visible = true;
            ////this.ChecaDataDeContratoETabelaDeValor();
            //cboTValoresContratoDETALHE_OnSelectedIndexChanged(null, null);
        }

        protected void cboTValoresContratoDETALHE_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            gridTVPlanos.DataSource = null;// cboTValoresPlanoDETALHE.Items.Clear();
            if (cboTValoresContratoDETALHE.Items.Count == 0)
            {
                gridTVPlanos.DataBind();
                return;
            }

            pnlTabelaValorITENS.Visible = false;
            gridTVPlanos.SelectedIndex = -1;

            IList<Plano> planos = Plano.CarregarPorContratoID(cboTValoresContratoDETALHE.SelectedValue, true);
            gridTVPlanos.DataSource = planos;
            gridTVPlanos.DataBind();
            pnlTVPlanos.Visible = planos != null;
            if (!pnlTVPlanos.Visible) { litTVSemPlano.Text = "<font size='1' color='red'>nenhum plano</font>"; }
            else { litTVSemPlano.Text = ""; }
        }

        protected void cmdTabelaValorFechar_Click(Object sender, EventArgs e)
        {
            pnlTabelaValorLista.Visible = true;
            pnlTabelaValorDetalhe.Visible = false;
            gridTabelaValores.SelectedIndex = -1;
            gridTVPlanos.SelectedIndex = -1;
            this.ItensTValores = null;
            pnlTVPlanos.Visible = false;
            pnlTabelaValorITENS.Visible = false;
        }

        protected void cmdSalvarTabelaValor_Click(Object sender, EventArgs e)
        {
            //#region validacoes

            //if (base.CToDecimal(txtFixo.Text) == 0) { txtFixo.Text = "0"; }
            //if (base.CToDecimal(txtOverPrice.Text) == 0) { txtOverPrice.Text = "0"; }
            //if (base.CToDecimal(txtValorEmbutido.Text) == 0) { txtValorEmbutido.Text = "0"; }

            //DateTime dtFim = new DateTime();
            //DateTime dtInicio = new DateTime();

            //Boolean resultado = UIHelper.TyParseToDateTime(txtTValoresInicio.Text, out dtInicio);
            //if (!resultado)
            //{
            //    //base.Alerta(MPE, ref litAlert, "Informe uma data de início válida.", upnlAlerta);
            //    txtTValoresInicio.Focus();

            //    if (gridTVPlanos.SelectedIndex == -1)
            //        pnlTabelaValorITENS_ListaPlanos.Visible = false;
            //    return;
            //}
            //resultado = UIHelper.TyParseToDateTime(txtTValoresFim.Text, out dtFim);
            //if (!resultado)
            //{
            //    //base.Alerta(MPE, ref litAlert, "Informe uma data final válida.", upnlAlerta);
            //    txtTValoresFim.Focus();

            //    if (gridTVPlanos.SelectedIndex == -1)
            //        pnlTabelaValorITENS_ListaPlanos.Visible = false;
            //    return;
            //}

            //if (dtInicio > dtFim)
            //{
            //    //base.Alerta(MPE, ref litAlert, "Data de início deve ser menor que a data final.", upnlAlerta);
            //    return;
            //}
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            //String msg = UIHelper.ChecaGridIntervaloDeIdades(gridTabelaValoresItemDETALHE, 0, 1);
            //if (msg != String.Empty)
            //{
            //    //base.Alerta(MPE, ref litAlert, msg, upnlAlerta);
            //    if (gridTVPlanos.SelectedIndex == -1)
            //        pnlTabelaValorITENS_ListaPlanos.Visible = false;
            //    return;
            //}

            //ContratoADM contrato = new ContratoADM(cboTValoresContratoDETALHE.SelectedValue);
            //contrato.Carregar();
            //DateTime dataContrato = new DateTime(contrato.Data.Year, contrato.Data.Month, contrato.Data.Day);
            //if (dtInicio < dataContrato)//if (dt < dataContrato) denis
            //{
            //    //base.Alerta(MPE, ref litAlert, "A data de início desta tabela de valores não pode ser inferior a " + contrato.Data.ToString("dd/MM/yyyy") + ".", upnlAlerta);
            //    txtTValoresInicio.Focus();

            //    if (gridTVPlanos.SelectedIndex == -1)
            //        pnlTabelaValorITENS_ListaPlanos.Visible = false;
            //    return;
            //}

            //List<TabelaValorItem> lista = this.ItensTValores;

            ////DateTime vigencia = base.CStringToDateTime(txtTValoresData.Text); denis
            //Object tabelaId = null;
            //if (gridTabelaValores.SelectedIndex > -1)
            //    tabelaId = gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value;

            ////if (TabelaValor.ExisteTabelaComVigencia(vigencia, cboTValoresContratoDETALHE.SelectedValue, tabelaId)) denis
            //if (TabelaValor.ExisteTabelaComVigencia(dtInicio, dtFim, cboTValoresContratoDETALHE.SelectedValue, tabelaId))
            //{
            //    //base.Alerta(null, this, "_errTVal7", "Já existe uma tabela de valor com essa data de vigência.");
            //    //base.Alerta(MPE, ref litAlert, "Já existe uma tabela de valor com essa data de vigência.", upnlAlerta);
            //    if (gridTVPlanos.SelectedIndex == -1)
            //        pnlTabelaValorITENS_ListaPlanos.Visible = false;
            //    return;
            //}
            //#endregion

            //TabelaValor tabela = new TabelaValor();

            //if (gridTabelaValores.SelectedIndex > -1)
            //{
            //    tabela.ID = gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value;
            //    tabela.Carregar();
            //}

            ////tabela.Descricao = txtTValoresDescricao.Text;
            ////if (txtTValoresData.Text.Trim() != "") denis
            ////    tabela.Data = Convert.ToDateTime(txtTValoresData.Text); denis

            //tabela.Inicio = dtInicio;
            //tabela.Fim = dtFim;

            //tabela.ContratoID = cboTValoresContratoDETALHE.SelectedValue;

            //DateTime vigencia, vencimentoDe, vencimentoAte = DateTime.MinValue;
            //int diaSemJuros = 0;
            //Object valorLimite = null;
            //CalendarioVencimento rcv = null;

            //#region calcula limites
            //CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(tabela.ContratoID,
            //    tabela.Inicio, out vigencia, out vencimentoDe, out diaSemJuros, out valorLimite, out rcv, null);

            //CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(tabela.ContratoID,
            //    tabela.Fim, out vigencia, out vencimentoAte, out diaSemJuros, out valorLimite, out rcv, null);

            //if (vencimentoDe != DateTime.MinValue)
            //    tabela.VencimentoInicio = vencimentoDe;
            //else
            //    tabela.VencimentoInicio = tabela.Inicio;

            //if (vencimentoAte != DateTime.MinValue)
            //    tabela.VencimentoFim = vencimentoAte;
            //else
            //    tabela.VencimentoFim = tabela.Fim;
            //#endregion

            //tabela.Salvar();

            //Taxa taxa = Taxa.CarregarPorTabela(tabela.ID);
            //if (taxa == null) { taxa = new Taxa(); }
            //taxa.Data = tabela.Inicio; //tabela.Data; denis
            //taxa.Embutido = true;
            //taxa.Fixo = CToDecimal(txtFixo.Text);
            //taxa.Over = CToDecimal(txtOverPrice.Text);
            //taxa.ValorEmbutido = CToDecimal(txtValorEmbutido.Text);
            //taxa.Embutido = chkValorEmbutido.Checked;
            //taxa.TabelaValorID = tabela.ID;
            //taxa.Salvar();

            //if (gridTVPlanos.SelectedIndex > -1)
            //{
            //    foreach (TabelaValorItem item in lista)
            //    {
            //        item.TabelaID = tabela.ID;
            //        item.PlanoID = gridTVPlanos.DataKeys[gridTVPlanos.SelectedIndex].Value;
            //        item.AplicaTaxa(taxa, false);
            //        item.Salvar();
            //    }

            //    this.ItensTValores = lista;
            //    gridTabelaValoresItemDETALHE.DataSource = lista;
            //    gridTabelaValoresItemDETALHE.DataBind();

            //    this.CarregaTabelaValores();
            //    //cmdTabelaValorFechar_Click(null, null);
            //    for (int i = 0; i < gridTabelaValores.Rows.Count; i++) //seleciona no grid a tabela recem salva
            //    {
            //        if (Convert.ToString(gridTabelaValores.DataKeys[i].Value) ==
            //            Convert.ToString(tabela.ID))
            //        {
            //            gridTabelaValores.SelectedIndex = i;
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    pnlTabelaValorITENS_ListaPlanos.Visible = false;
            //}

            ////base.Alerta(null, this, "_okTVal", "Dados salvos com sucesso.");
            //TabelaDeValorFacade.Instance.RecalcularTaxaEmPlanos(tabela.ID, taxa);
            //base.Alerta(null,this, "Dados salvos com sucesso.", "_ok");
        }

        #endregion
    }
}

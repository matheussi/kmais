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

    public partial class operadora : PageBase
    {
        //String tipoSel
        //{
        //    get { return base.CToString(ViewState["__selt"]); }
        //    set { ViewState["__selt"] = value; }
        //}

        /// <summary>
        /// Itens de produto adicional
        /// </summary>
        List<AdicionalFaixa> Itens
        {
            get
            {
                if (Session["_itens"] != null)
                {
                    List<AdicionalFaixa> lista = Session["_itens"] as List<AdicionalFaixa>;

                    for (int i = 0; i < gridAdicionalFaixaNovo.Rows.Count; i++)
                    {
                        lista[i].Vigencia = base.CStringToDateTime(((TextBox)gridAdicionalFaixaNovo.Rows[i].Cells[0].Controls[1]).Text);
                        lista[i].IdadeFim = base.CToInt(((TextBox)gridAdicionalFaixaNovo.Rows[i].Cells[2].Controls[1]).Text);
                        lista[i].IdadeInicio = base.CToInt(((TextBox)gridAdicionalFaixaNovo.Rows[i].Cells[1].Controls[1]).Text);
                        lista[i].Valor = base.CToDecimal(((TextBox)gridAdicionalFaixaNovo.Rows[i].Cells[3].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].ID = gridAdicionalFaixaNovo.DataKeys[i].Value;
                    }

                    return lista;
                }
                else
                    return null;
            }
            set { Session["_itens"] = value; }
        }

        /// <summary>
        /// Itens de tabela de valor
        /// </summary>
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

                        //lista[i].QCValorCompraCarencia = base.CToDecimal(((TextBox)gridTabelaValoresItemDETALHE.Rows[i].Cells[4].Controls[1]).Text);
                        //lista[i].QPValorCompraCarencia = base.CToDecimal(((TextBox)gridTabelaValoresItemDETALHE.Rows[i].Cells[5].Controls[1]).Text);

                        //lista[i].QCValorMigracao = base.CToDecimal(((TextBox)gridTabelaValoresItemDETALHE.Rows[i].Cells[6].Controls[1]).Text);
                        //lista[i].QPValorMigracao = base.CToDecimal(((TextBox)gridTabelaValoresItemDETALHE.Rows[i].Cells[7].Controls[1]).Text);

                        //lista[i].QCValorCondicaoEspecial = base.CToDecimal(((TextBox)gridTabelaValoresItemDETALHE.Rows[i].Cells[8].Controls[1]).Text);
                        //lista[i].QPValorCondicaoEspecial = base.CToDecimal(((TextBox)gridTabelaValoresItemDETALHE.Rows[i].Cells[9].Controls[1]).Text);

                        lista[i].ID = gridTabelaValoresItemDETALHE.DataKeys[i].Value;

                        if(!String.IsNullOrEmpty(gridTabelaValoresItemDETALHE.Rows[i].Cells[5].Text))
                            lista[i].TabelaID = gridTabelaValoresItemDETALHE.Rows[i].Cells[5].Text;
                    }

                    return lista;
                }
                else
                    return null;
            }
            set { Session["_itenstv"] = value; }
        }

        /// <summary>
        /// Itens de tabela de reajuste
        /// </summary>
        List<TabelaReajusteItem> ItensTReajuste
        {
            get
            {
                if (Session["_itensReaj"] != null)
                {
                    List<TabelaReajusteItem> lista = Session["_itensReaj"] as List<TabelaReajusteItem>;

                    for (int i = 0; i < gridTReajusteItemDETALHE.Rows.Count; i++)
                    {
                        lista[i].IdadeInicio = base.CToInt(((TextBox)gridTReajusteItemDETALHE.Rows[i].Cells[0].Controls[1]).Text);
                        lista[i].PercentualReajuste = CToDecimal(((TextBox)gridTReajusteItemDETALHE.Rows[i].Cells[1].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].ID = gridTReajusteItemDETALHE.DataKeys[i].Value;
                        //lista[i].TabelaID = ViewState[IDKey];
                    }

                    return lista;
                }
                else
                    return null;
            }
            set { Session["_itensReaj"] = value; }
        }

        /// <summary>
        /// Itens da tabela de comissionamento
        /// </summary>
        List<ComissionamentoOperadoraItem> ItensComissionamento
        {
            get
            {
                if (Session["_itensCom"] != null)
                {
                    List<ComissionamentoOperadoraItem> lista = Session["_itensCom"] as List<ComissionamentoOperadoraItem>;

                    for (int i = 0; i < gridComissionamentoItensDetalhe.Rows.Count; i++)
                    {
                        lista[i].Parcela = base.CToInt(((TextBox)gridComissionamentoItensDetalhe.Rows[i].Cells[0].Controls[1]).Text);
                        lista[i].Percentual = base.CToDecimal(((TextBox)gridComissionamentoItensDetalhe.Rows[i].Cells[1].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualADM = base.CToDecimal(((TextBox)gridComissionamentoItensDetalhe.Rows[i].Cells[4].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualCompraCarencia = base.CToDecimal(((TextBox)gridComissionamentoItensDetalhe.Rows[i].Cells[2].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualEspecial = base.CToDecimal(((TextBox)gridComissionamentoItensDetalhe.Rows[i].Cells[5].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualMigracao = base.CToDecimal(((TextBox)gridComissionamentoItensDetalhe.Rows[i].Cells[3].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualIdade = base.CToDecimal(((TextBox)gridComissionamentoItensDetalhe.Rows[i].Cells[6].Controls[1]).Text);
                        lista[i].ID = gridComissionamentoItensDetalhe.DataKeys[i].Value;
                    }

                    return lista;
                }
                else
                    return null;
            }
            set { Session["_itensCom"] = value; }
        }

        /// <summary>
        /// Itens da tabela de comissionamento por idade
        /// </summary>
        List<ComissionamentoIdadeItem> ItensComissionamentoIdade
        {
            get
            {
                List<ComissionamentoIdadeItem> lista = Session["_itensComId"] as List<ComissionamentoIdadeItem>;

                if (Session["_itensComId"] != null)
                {
                    for (int i = 0; i < gridComissionamentoIdadeItem.Rows.Count; i++)
                    {
                        lista[i].Parcela = base.CToInt(((TextBox)gridComissionamentoIdadeItem.Rows[i].Cells[0].Controls[1]).Text);
                        lista[i].Percentual = base.CToDecimal(((TextBox)gridComissionamentoIdadeItem.Rows[i].Cells[1].Controls[1]).Text);
                    }

                    return lista;
                }
                else
                    return lista;
            }
            set { Session["_itensComId"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtComissionamentoVitalicioPercentual.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentual.ClientID + "')");
            txtComissionamentoVitalicioPercentualCarencia.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualCarencia.ClientID + "')");
            txtComissionamentoVitalicioPercentualEspecial.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualEspecial.ClientID + "')");
            txtComissionamentoVitalicioPercentualMigracao.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualMigracao.ClientID + "')");

            txtOverPrice.Attributes.Add("onKeyUp", "mascara('" + txtOverPrice.ClientID + "')");
            txtFixo.Attributes.Add("onKeyUp", "mascara('" + txtFixo.ClientID + "')");
            txtValorEmbutido.Attributes.Add("onKeyUp", "mascara('" + txtValorEmbutido.ClientID + "')");
            txtOverPriceDuplicar.Attributes.Add("onKeyUp", "mascara('" + txtOverPriceDuplicar.ClientID + "')");
            txtFixoDuplicar.Attributes.Add("onKeyUp", "mascara('" + txtFixoDuplicar.ClientID + "')");
            txtValorEmbutidoDuplicar.Attributes.Add("onKeyUp", "mascara('" + txtValorEmbutidoDuplicar.ClientID + "')");
            txtReajusteDuplicar.Attributes.Add("onKeyUp", "mascara('" + txtReajusteDuplicar.ClientID + "')");

            cmdBuscaEndereco.Visible = base.useExternalCEPEngine();
            txtNumero.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");

            if (!IsPostBack)
            {
                pnlDetalheContrato.Attributes.Add("style", "display:none");
                this.ItensComissionamento = null;
                this.CarregaEstadoCivil();
                base.PreencheComboNumerico(cboDiaPagamento, 1, 31, true);
                base.PreencheComboNumerico(cboDiaRecebimento, 1, 31, true);
                base.PreencheComboNumerico(cboAdmissaoDe_DIA, 1, 31, true);
                base.PreencheComboNumerico(cboAdmissaoAte_DIA, 1, 31, true);
                base.PreencheComboNumerico(cboVigencia, 1, 31, true);
                base.PreencheComboNumerico(cboVencimento, 1, 31, true);
                base.PreencheComboNumerico(cboComissao_DIA, 1, 31, true);
                base.PreencheComboNumerico(cboFatura_DIA, 1, 31, true);
                base.PreencheComboNumerico(cboComissao_Percentual, 1, 100, true);
                base.PreencheComboNumerico(cboDataLimiteDia, 0, 31, true);
                base.PreencheComboNumerico(cboDataSemJuros_DIAS, 0, 31, true);
                base.PreencheComboNumerico(cboDataLimite2, 1, 100, false);
                cboComissao_Percentual.SelectedValue = "100";

                base.PreencheComboComOpcoesDeCalendario(cboFatura_Tipo, true);
                base.PreencheComboComOpcoesDeCalendario(cboComissao_Tipo, true);
                base.PreencheComboComOpcoesDeCalendario(cboVigencia_Tipo, true);
                base.PreencheComboComOpcoesDeCalendario(cboVencimento_Tipo, true);
                base.PreencheComboComOpcoesDeCalendario(cboAdmissaoDe_Tipo, true);
                base.PreencheComboComOpcoesDeCalendario(cboAdmissaoAte_Tipo, true);

                cboDiaPagamento.Items.Insert(0, new ListItem("---", "-1"));
                cboDiaRecebimento.Items.Insert(0, new ListItem("---", "-1"));

                if (base.IDKeyParameterInProcess(ViewState, "_operadora"))
                {
                    this.CarregarOperadora();

                    base.ExibirEstipulantes(cboContratoEstipulante, false, true);

                    this.CarregaContatos();
                    this.CarregaContratos();
                    this.CarregaAdicionais();

                    if (cboPlanoContrato.Items.Count > 0)
                        this.CarregaPlanos(cboPlanoContrato.SelectedValue);

                    this.CarregaTabelaValores();//this.CarregaPlanosParaTabelaDeValor();
                    this.CarregaTabelaReajustes();
                    this.CarregaCalendariosAdmissaoVigencia();
                }
                else
                {
                    tab.Tabs[5].Visible = false;
                    titP5.InnerText = "";

                    tab.Tabs[4].HeaderText = "";
                    tab.Tabs[4].Visible = false;
                    titP4.InnerHtml = "";

                    tab.Tabs[3].HeaderText = "";
                    tab.Tabs[3].Visible = false;
                    titP3.InnerHtml = "";

                    tab.Tabs[2].HeaderText = "";
                    tab.Tabs[2].Visible = false;
                    titP2.InnerHtml = "";

                    p1a.Visible = false;
                    titP1a.InnerHtml = "";
                    tab.Tabs[1].Visible = false;
                }

                upPlanos.Update();
                //upTabelaValores.Update();
                //upTabelaReajustes.Update();
                //upAdicionaisPlano.Update();
                //upComissionamento.Update();
            }

            UIHelper.AuthCtrl(cmdSalvar, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            UIHelper.AuthCtrl(cmdSalvarContato, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            UIHelper.AuthCtrl(cmdSalvarContrato, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            UIHelper.AuthCtrl(cmdSalvarComissionamento, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            UIHelper.AuthCtrl(cmdSalvarAdicional, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);

            this.CheckExtraPermission();
        }

        Boolean extraPermission()
        {
            if (Usuario.Autenticado.ExtraPermission2 || Usuario.Autenticado.PerfilID == Perfil.AdministradorIDKey)
                return true;
            else
                return false;
        }

        void CheckExtraPermission()
        {
            if (!extraPermission())
            {
                cmdSalvar.Visible = false;

                cmdSalvarContato.Visible = false;
                gridContatos.Columns[4].Visible = false;

                cmdSalvarContrato.Visible = false;
                gridContratos.Columns[4].Visible = false;
                gridContratos.Columns[5].Visible = false;

                cmdSalvarComissionamento.Visible = false;
                gridComissionamento.Columns[4].Visible = false;
                cmdAddItemCom.Visible = false;
                gridComissionamentoItensDetalhe.Columns[7].Visible = false;

                cmdSalvarAdicional.Visible = false;
                gridAdicional.Columns[2].Visible = false;
                cmdAddItem.Visible = false;
                gridAdicionalFaixaNovo.Columns[4].Visible = false;

                gridPlano.Columns[5].Visible = false;
                gridPlano.Columns[6].Visible = false;
                cmdPlanoSalvar.Visible = false;

                gridTabelaValores.Columns[7].Visible = false;
                gridTabelaValores.Columns[9].Visible = false;
                gridTabelaValores.Columns[11].Visible = false;

                cmdAddItemTabelaValor.Visible = false;
                cmdSalvarTabelaValor.Visible = false;
                gridTabelaValoresItemDETALHE.Columns[6].Visible = false;

                gridPlanoAdicional.Columns[1].Visible = false;
                cmdAddAdicional.Visible = false;

                cmdCalendarioSalvar.Enabled = false;
                cmdCalendarioSalvarVencimento.Enabled = false;
            }
        }

        void CarregarOperadora()
        {
            Operadora operadora = new Operadora();
            operadora.ID = ViewState[IDKey];
            operadora.Carregar();

            txtNome.Text = operadora.Nome;
            txtCnpj.Text = operadora.CNPJ;
            //txtEmail.Text = operadora.Email;
            //txtContato.Text = operadora.Contato;
            //txtFone1.Text = operadora.Fone;
            //txtDDD1.Text = operadora.DDD;
            //txtRamal1.Text = operadora.Ramal;
            txtMensagemRemessa.Text = operadora.MensagemRemessa;
            
            if (operadora.Endereco.ID != null)
                ViewState[IDKey2] = operadora.Endereco.ID;

            txtCEP.Text = operadora.Endereco.CEP;
            txtLogradouro.Text = operadora.Endereco.Logradouro;
            txtNumero.Text = operadora.Endereco.Numero;
            txtComplemento.Text = operadora.Endereco.Complemento;
            txtBairro.Text = operadora.Endereco.Bairro;
            txtCidade.Text = operadora.Endereco.Cidade;
            txtUF.Text = operadora.Endereco.UF;

            if (cboDiaPagamento.Items.FindByValue(operadora.DiaPagamento.ToString()) != null)
                cboDiaPagamento.SelectedValue = Convert.ToString(operadora.DiaPagamento);

            if (cboDiaRecebimento.Items.FindByValue(operadora.DiaRecebimento.ToString()) != null)
                cboDiaRecebimento.SelectedValue = Convert.ToString(operadora.DiaRecebimento);

            if (operadora.TamanhoMaximoLogradouroBeneficiario > 0)
                txtLogradouroTamanhoMaximo.Text = operadora.TamanhoMaximoLogradouroBeneficiario.ToString();

            chkEnviaCarta.Checked = operadora.EnviaCartaDeAviso;
            chkPermiteReativacao.Checked = operadora.PermiteReativacao;
        }

        void CarregaEstadoCivil()
        {
            //gridEstadosCivis.DataSource = EstadoCivil.CarregarTodos();
            //gridEstadosCivis.DataBind();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/operadoras.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (!IsValid) { return; }

            #region validacoes 

            if (txtNome.Text.Trim() == "")
            {
                tab.ActiveTabIndex = 0;
                //Alerta(null, this, "_err0", "Informe o campo Nome.");
                base.Alerta(MPE, ref litAlert, "Informe o campo Nome.", upnlAlerta);
                txtNome.Focus();
                return;
            }

            if (txtCnpj.Text.Trim() == "" || txtCnpj.Text.Length != 14)
            {
                tab.ActiveTabIndex = 0;
                //Alerta(null, this, "_err1", "Informe o campo CNPJ.");
                base.Alerta(MPE, ref litAlert, "Informe o campo CNPJ.", upnlAlerta);
                txtCnpj.Focus();
                return;
            }

            if (!UIHelper.VerificaCnpj(txtCnpj.Text))// || Operadora.CnpjEmUso(txtCnpj.Text, ViewState[IDKey]))
            {
                tab.ActiveTabIndex = 0;
                //Alerta(null, this, "_err1b", "CNPJ inválido ou em uso.");
                base.Alerta(MPE, ref litAlert, "CNPJ inválido.", upnlAlerta);
                txtCnpj.Focus();
                return;
            }

            if (Operadora.NomeEmUso(txtNome.Text, ViewState[IDKey]))
            {
                tab.ActiveTabIndex = 0;
                //Alerta(null, this, "_err1c", "Nome já usado.");
                base.Alerta(MPE, ref litAlert, "Nome já usado.", upnlAlerta);
                txtNome.Focus();
                return;
            }

            if (txtLogradouro.Text.Trim() == "" ||
                txtCEP.Text.Trim() == "" ||
                txtNumero.Text.Trim() == "" ||
                txtBairro.Text.Trim() == "" ||
                txtCidade.Text.Trim() == "" ||
                txtBairro.Text.Trim() == "" ||
                txtUF.Text.Trim() == "")
            {
                tab.ActiveTabIndex = 0;
                txtCEP.Focus();
                //base.Alerta(null, this, "_errEnd", "Informe o endereço da operadora.");
                base.Alerta(MPE, ref litAlert, "Informe o endereço da operadora.", upnlAlerta);
                return;
            }

            if (txtUF.Text.Trim() != "" && !UIHelper.ValidaUF(txtUF.Text))
            {
                tab.ActiveTabIndex = 0;
                //base.Alerta(null, this, "_errUF", "Unidade Federativa inválida.");
                base.Alerta(MPE, ref litAlert, "Unidade Federativa inválida.", upnlAlerta);
                txtUF.Focus();
                return;
            }
            #endregion

            Operadora operadora = new Operadora();

            operadora.ID = ViewState[IDKey];
            operadora.Nome = txtNome.Text;
            operadora.CNPJ = txtCnpj.Text;
            //operadora.Contato = txtContato.Text;
            //operadora.Fone = txtFone1.Text;
            //operadora.DDD = txtDDD1.Text;
            //operadora.Ramal = txtRamal1.Text;
            //operadora.Email = txtEmail.Text;
            operadora.MensagemRemessa = txtMensagemRemessa.Text;

            operadora.Endereco.ID = ViewState[IDKey2];
            operadora.Endereco.Bairro = txtBairro.Text;
            operadora.Endereco.CEP = txtCEP.Text;
            operadora.Endereco.Cidade = txtCidade.Text;
            operadora.Endereco.Complemento = txtComplemento.Text;
            operadora.Endereco.DonoTipo = Convert.ToInt32(Endereco.TipoDono.Operadora);
            operadora.Endereco.Logradouro = txtLogradouro.Text;
            operadora.Endereco.Numero = txtNumero.Text;
            operadora.Endereco.Tipo = Convert.ToInt32(Endereco.TipoEndereco.Residencial);
            operadora.Endereco.UF = txtUF.Text;

            if (txtLogradouroTamanhoMaximo.Text.Trim() != "")
                operadora.TamanhoMaximoLogradouroBeneficiario = Convert.ToInt32(txtLogradouroTamanhoMaximo.Text);

            operadora.DiaPagamento   = Convert.ToInt32(cboDiaPagamento.SelectedValue);
            operadora.DiaRecebimento = Convert.ToInt32(cboDiaRecebimento.SelectedValue);

            operadora.EnviaCartaDeAviso = chkEnviaCarta.Checked;
            operadora.PermiteReativacao = chkPermiteReativacao.Checked;

            operadora.Salvar();

            ViewState[IDKey] = operadora.ID;
            ViewState[IDKey2] = operadora.Endereco.ID;

            #region exibe paineis 

            tab.Tabs[5].Visible = true;
            titP5.InnerText = "Calendário";

            tab.Tabs[4].Visible = true;
            titP4.InnerHtml = "Planos";

            p3.Visible = true;
            tab.Tabs[3].Visible = true;
            titP3.InnerHtml = "Adicionais";

            p2.Visible = true;
            tab.Tabs[2].Visible = true;
            titP2.InnerHtml = "Contratos";

            p1a.Visible = true;
            tab.Tabs[1].Visible = true;
            titP1a.InnerHtml = "Contatos";

            #endregion

            //base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
            base.ExibirEstipulantes(cboContratoEstipulante, false, true);
        }

        #region Dados comuns 

        protected void cmdBuscaEndereco_Click(Object sender, EventArgs e)
        {
            base.PegaEndereco(this, txtCEP, txtLogradouro, txtBairro, txtCidade, txtUF, txtNumero, true);
            //wsCEP.calcula_frete proxy = new wsCEP.calcula_frete();
            //String endereco = proxy.traz_cep(txtCEP.Text);
            //proxy.Dispose();

            //if (!String.IsNullOrEmpty(endereco) && endereco != "#####")
            //{
            //    String[] enderecoArr = endereco.Split(
            //        new String[] { "##" }, StringSplitOptions.None);
            //    txtLogradouro.Text = enderecoArr[0].Replace("#", " ");
            //    txtBairro.Text = enderecoArr[1].Split('#')[0];
            //    txtCidade.Text = enderecoArr[1].Split('#')[1].Replace("�", "a");
            //    txtUF.Text = enderecoArr[1].Split('#')[2];
            //    txtNumero.Focus();
            //}
            //else
            //{
            //    txtLogradouro.Text = "";
            //    txtBairro.Text = "";
            //    txtCidade.Text = "";
            //    txtUF.Text = "";
            //}
        }

        #endregion

        #region Planos

        void CarregaPlanos(Object contratoId)
        {
            IList<Plano> lista = null;

            if (contratoId != null)
                lista = Plano.CarregarPorContratoID(contratoId);

            gridTVPlanos.DataSource = lista;
            gridTVPlanos.DataBind();

            gridPlano.DataSource = lista;
            gridPlano.DataBind();

            //if (lista == null || lista.Count == 0) { litNenhumPlano.Text = " (nenhum)"; }
            //else { litNenhumPlano.Text = ""; }

            //cboPlanoTValores.DataValueField = "ID";
            //cboPlanoTValores.DataTextField = "Descricao";
            //cboPlanoTValores.DataSource = lista;
            //cboPlanoTValores.DataBind();

            cboPlanoAdicional_Plano.DataValueField = "ID";
            cboPlanoAdicional_Plano.DataTextField = "Descricao";
            cboPlanoAdicional_Plano.DataSource = lista;
            cboPlanoAdicional_Plano.DataBind();
            cboPlanoAdicional_Plano_OnSelectedIndexChanged(null, null);
        }

        protected void gridPlano_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 5, "Deseja realmente prosseguir com a exclusão?");
                Boolean status = Convert.ToBoolean(gridPlano.DataKeys[e.Row.RowIndex][1]);

               if (status)
               {
                   ((LinkButton)e.Row.Cells[6].Controls[0]).Text = "<img src='images/active.png' title='ativo' alt='ativo' border='0'>";
                   base.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente inativar?");
               }
               else
               {
                   e.Row.ForeColor = System.Drawing.Color.FromName("#CC0000");
                   ((LinkButton)e.Row.Cells[6].Controls[0]).Text = "<img src='images/unactive.png' title='inativo' alt='inativo' border='0'>";
                   base.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente ativar?");
               }
            }
        }

        protected void gridPlano_OnRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                gridPlano.SelectedIndex = Convert.ToInt32(e.CommandArgument);

                Plano plano = new Plano();
                plano.ID = gridPlano.DataKeys[gridPlano.SelectedIndex].Value;
                plano.Carregar();
                chkPlanoAtivo.Checked = plano.Ativo;
                txtPlanoCodigo.Text = plano.Codigo;
                txtPlanoCodigoParticular.Text = plano.CodigoParticular;
                txtPlanoDescricao.Text = plano.Descricao;
                txtPlanoCaracteristicas.Text = plano.Caracteristicas;
                txtPlanoSubPlano.Text = plano.SubPlano;
                txtPlanoSubPlanoParticular.Text = plano.SubPlanoParticular;
                txtPlanoAnsComum.Text = plano.AnsQuartoComum;
                txtPlanoAnsParticular.Text = plano.AnsQuartoParticular;
                chkQuartoComum.Checked = plano.QuartoComum;
                chkQuartoParticular.Checked = plano.QuartoParticular;
                cboPlanoContratoDETALHE.SelectedValue = Convert.ToString(plano.ContratoID);

                if (plano.InicioColetivo != DateTime.MinValue)
                    txtPlanoQCInicio.Text = plano.InicioColetivo.ToString("dd/MM/yyyy");

                if (plano.InicioParticular != DateTime.MinValue)
                    txtPlanoQPInicio.Text = plano.InicioParticular.ToString("dd/MM/yyyy");

                pnlPlanoDetalhe.Visible = true;
                pnlPlanoLista.Visible = false;
            }
            else if (e.CommandName.Equals("inativar"))
            {
                Object id = gridPlano.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Boolean status = Convert.ToBoolean(gridPlano.DataKeys[Convert.ToInt32(e.CommandArgument)][1]);
                Plano.AlterarStatus(id, !status);
                this.CarregaPlanos(cboPlanoContrato.SelectedValue);
            }
            else if (e.CommandName.Equals("excluir"))
            {
                Object id = gridPlano.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Plano plano = new Plano(id);
                try
                {
                    plano.Remover();
                    this.CarregaPlanos(cboPlanoContrato.SelectedValue);
                }
                catch
                {
                    //base.Alerta(null, this, "_excPl", "Não foi possível excluir o plano.\\nTalvez ele esteja sendo usado em uma proposta.");
                    base.Alerta(MPE, ref litAlert, "Não foi possível excluir o plano.<br>Talvez ele esteja sendo usado em uma proposta.", upnlAlerta);
                }
            }
        }

        protected void cmdNovoPlano_Click(Object sender, EventArgs e)
        {
            pnlPlanoDetalhe.Visible = true;
            pnlPlanoLista.Visible = false;
            gridPlano.SelectedIndex = -1;
            txtPlanoDescricao.Text = "";
            txtPlanoCaracteristicas.Text = "";
            txtPlanoCodigo.Text = "";
            txtPlanoSubPlano.Text = "";
            txtPlanoCodigoParticular.Text = "";
            txtPlanoSubPlanoParticular.Text = "";
            txtPlanoAnsComum.Text = "";
            txtPlanoAnsParticular.Text = "";
        }

        protected void cmdSalvarPlano_Click(Object sender, EventArgs e)
        {
            #region validacoes

            if (cboPlanoContratoDETALHE.Items.Count == 0)
            {
                //base.Alerta(null, this, "errPl01", "Atenção!\\nNão há um contrato selecionado.");
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há um contrato selecionado.", upnlAlerta);
                cboPlanoContratoDETALHE.Focus();
                return;
            }

            if (txtPlanoDescricao.Text.Trim() == "")
            {
                //base.Alerta(null, this, "errPl00", "Atenção!\\nInforme uma descrição ao plano.");
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Informe uma descrição ao plano.", upnlAlerta);
                txtPlanoDescricao.Focus();
                return;
            }

            Object id = null;
            if (gridPlano.SelectedIndex > -1)
                id = gridPlano.DataKeys[gridPlano.SelectedIndex].Value;

            String qcCodigo     = txtPlanoCodigo.Text;
            String qcSubPlano   = txtPlanoSubPlano.Text;
            String qpCodigo     = txtPlanoCodigoParticular.Text;
            String qpSubPlano   = txtPlanoSubPlanoParticular.Text;

            if (!chkQuartoComum.Checked)
            {
                qcCodigo = ""; qcSubPlano = ""; txtPlanoQCInicio.Text = ""; txtPlanoAnsComum.Text = "";
            }
            if (!chkQuartoParticular.Checked)
            {
                qpCodigo = ""; qpSubPlano = ""; txtPlanoQPInicio.Text = ""; txtPlanoAnsParticular.Text = "";
            }

            if (Plano.Existe(cboPlanoContratoDETALHE.SelectedValue, id, 
                txtPlanoDescricao.Text, qcCodigo, qcSubPlano, qpCodigo, qpSubPlano))
            {
                //base.Alerta(null, this, "errPl00", "Atenção!\\nJá existe um plano com essa descrição.");
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Já existe um plano com essas informações.", upnlAlerta);
                txtPlanoDescricao.Focus();
                return;
            }

            #endregion

            Plano plano = new Plano();

            if (gridPlano.SelectedIndex > -1)
                plano.ID = gridPlano.DataKeys[gridPlano.SelectedIndex].Value;

            plano.Ativo = chkPlanoAtivo.Checked;
            plano.Codigo = txtPlanoCodigo.Text;
            plano.CodigoParticular = txtPlanoCodigoParticular.Text;
            plano.Descricao = txtPlanoDescricao.Text;
            plano.Caracteristicas = txtPlanoCaracteristicas.Text;
            plano.ContratoID = cboPlanoContratoDETALHE.SelectedValue;
            plano.SubPlano = txtPlanoSubPlano.Text;
            plano.SubPlanoParticular = txtPlanoSubPlanoParticular.Text;
            plano.QuartoComum = chkQuartoComum.Checked;
            plano.QuartoParticular = chkQuartoParticular.Checked;
            plano.AnsQuartoComum = txtPlanoAnsComum.Text;
            plano.AnsQuartoParticular = txtPlanoAnsParticular.Text;

            plano.InicioColetivo = base.CStringToDateTime(txtPlanoQCInicio.Text);
            if(plano.QuartoComum && plano.InicioColetivo == DateTime.MinValue)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Preencha a data de início para quarto coletivo.", upnlAlerta);
                return;
            }

            plano.InicioParticular  = base.CStringToDateTime(txtPlanoQPInicio.Text);
            if (plano.QuartoParticular && plano.InicioParticular == DateTime.MinValue)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Preencha a data de início para quarto particular.", upnlAlerta);
                return;
            }

            plano.Salvar();

            pnlPlanoDetalhe.Visible = false;
            pnlPlanoLista.Visible = true;
            gridPlano.SelectedIndex = -1;
            this.CarregaPlanos(cboPlanoContrato.SelectedValue);
            //base.Alerta(null, this, "_okPl", "Dados salvos com sucesso.");
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
        }

        protected void cmdPlanoFechar_Click(Object sender, EventArgs e)
        {
            gridPlano.SelectedIndex = -1;
            pnlPlanoLista.Visible = true;
            pnlPlanoDetalhe.Visible = false;
        }

        protected void cboPlanoContrato_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaPlanos(cboPlanoContrato.SelectedValue);
        }

        #endregion 

        #region Contatos 

        void CarregaContatos()
        {
            gridContatos.DataSource = Contato.Carregar(ViewState[IDKey]);
            gridContatos.DataBind();
        }

        protected void gridContatos_OnRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Object id = gridContatos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Contato contato = new Contato(id);
                contato.Carregar();
                txtContato.Text = contato.Nome;
                txtContatoDepartamento.Text = contato.Departamento;
                txtDDD1.Text = contato.DDD;
                txtFone1.Text = contato.Fone;
                txtRamal1.Text = contato.Ramal;
                txtEmail.Text = contato.Email;

                gridContatos.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                pnlListaContatos.Visible = false;
                pnlDetalheContato.Visible = true;
            }
            else if(e.CommandName.Equals("excluir"))
            {
                Object id = gridContatos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Contato contato = new Contato(id);
                contato.Remover();
                this.CarregaContatos();
            }
        }

        protected void gridContatos_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UIHelper.AuthWebCtrl(e.Row.Cells[4], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente excluir o contato?");
            }
        }

        protected void cmdNovoContato_Click(Object sender, EventArgs e)
        {
            gridContatos.SelectedIndex = -1;
            pnlListaContatos.Visible = false;
            pnlDetalheContato.Visible = true;
        }

        protected void cmdFecharContato_Click(Object sender, EventArgs e)
        {
            gridContatos.SelectedIndex = -1;
            pnlListaContatos.Visible = true;
            pnlDetalheContato.Visible = false;
        }

        protected void cmdSalvarContato_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (txtContato.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_errContNm", "Informe o nome do contato.");
                base.Alerta(MPE, ref litAlert, "Informe o nome do contato.", upnlAlerta);
                txtContato.Focus();
                return;
            }

            String msg = "";
            if (!base.ValidaEmail(txtEmail.Text, out msg))
            {
                //base.Alerta(null, this, "_errContMail", msg);
                base.Alerta(MPE, ref litAlert, msg, upnlAlerta);
                txtEmail.Focus();
                return;
            }
            #endregion

            Contato contato = new Contato();

            if (gridContatos.SelectedIndex > -1)
                contato.ID = gridContatos.DataKeys[gridContatos.SelectedIndex].Value;

            contato.DDD = txtDDD1.Text;
            contato.Email = txtEmail.Text;
            contato.Fone = txtFone1.Text;
            contato.Nome = txtContato.Text;
            contato.Departamento = txtContatoDepartamento.Text;
            contato.OperadoraID = ViewState[IDKey];
            contato.Ramal = txtRamal1.Text;
            contato.Salvar();

            this.CarregaContatos();

            pnlListaContatos.Visible = true;
            pnlDetalheContato.Visible = false;
            gridContatos.SelectedIndex = -1;
        }

        #endregion

        #region Contratos

        void CarregaContratos()
        {
            IList<ContratoADM> lista = ContratoADM.Carregar(ViewState[IDKey]);
            gridContratos.DataSource = lista;
            gridContratos.DataBind();

            cboPlanoContrato.DataValueField = "ID";
            cboPlanoContrato.DataTextField = "Descricao";
            cboPlanoContrato.DataSource = lista;
            cboPlanoContrato.DataBind();

            cboPlanoContratoDETALHE.DataValueField = "ID";
            cboPlanoContratoDETALHE.DataTextField = "Descricao";
            cboPlanoContratoDETALHE.DataSource = lista;
            cboPlanoContratoDETALHE.DataBind();

            cboTValoresContrato.DataValueField = "ID";
            cboTValoresContrato.DataTextField = "Descricao";
            cboTValoresContrato.DataSource = lista;
            cboTValoresContrato.DataBind();
            //cboTValoresContrato_OnSelectedIndexChanged(null, null);
            //cboTValoresPlano_OnSelectedIndexChanged(null, null);

            cboTValoresContratoDETALHE.DataValueField = "ID";
            cboTValoresContratoDETALHE.DataTextField = "Descricao";
            cboTValoresContratoDETALHE.DataSource = lista;
            cboTValoresContratoDETALHE.DataBind();

            cboTReajusteContrato.DataValueField = "ID";
            cboTReajusteContrato.DataTextField = "Descricao";
            cboTReajusteContrato.DataSource = lista;
            cboTReajusteContrato.DataBind();
            cboTReajusteContrato_OnSelectedIndexChanged(null, null);

            cboTRContratoDETALHE.DataValueField = "ID";
            cboTRContratoDETALHE.DataTextField = "Descricao";
            cboTRContratoDETALHE.DataSource = lista;
            cboTRContratoDETALHE.DataBind();

            cboPlanoAdicional_Contrato.DataValueField = "ID";
            cboPlanoAdicional_Contrato.DataTextField = "Descricao";
            cboPlanoAdicional_Contrato.DataSource = lista;
            cboPlanoAdicional_Contrato.DataBind();
            cboPlanoAdicional_Contrato_OnSelectedIndexChanged(null, null);

            ///////////////////////////////////////////////////////////////////////////////////////
            cboComissionamentoContrato.DataValueField = "ID";
            cboComissionamentoContrato.DataTextField = "Descricao";
            cboComissionamentoContrato.DataSource = lista;
            cboComissionamentoContrato.DataBind();
            this.CarregarComissionamentos();// cboComissionamentoContrato_OnSelectedIndexChanged(null, null);
            cboComissionamentoContratoDETALHE.DataValueField = "ID";
            cboComissionamentoContratoDETALHE.DataTextField = "Descricao";
            cboComissionamentoContratoDETALHE.DataSource = lista;
            cboComissionamentoContratoDETALHE.DataBind();
            //upComissionamento.Update();
            ///////////////////////////////////////////////////////////////////////////////////////

            cboCalendarioContrato.DataValueField = "ID";
            cboCalendarioContrato.DataTextField = "Descricao";
            cboCalendarioContrato.DataSource = lista;
            cboCalendarioContrato.DataBind();
        }

        protected void gridContratos_OnRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Object id = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                ContratoADM contrato = new ContratoADM(id);
                contrato.Carregar();
                cboContratoEstipulante.SelectedValue = Convert.ToString(contrato.EstipulanteID);
                txtContratoDescricao.Text = contrato.Descricao;
                txtContratoNumero.Text = contrato.Numero;
                txtContratoSaude.Text = contrato.ContratoSaude;
                txtContratoDental.Text = contrato.ContratoDental;
                txtContratoCodFilial.Text = contrato.CodFilial;
                txtContratoCodUnidade.Text = contrato.CodUnidade;
                txtContratoCodAdministradora.Text = contrato.CodAdministradora;

                chkContratoAtivo.Checked = contrato.Ativo;
                txtContratoData.Text = contrato.Data.ToString("dd/MM/yyyy");

                gridContratos.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                pnlListaContratos.Visible = false;
                pnlDetalheContrato.Attributes.Add("style", "display:"); //pnlDetalheContrato.Visible = true;
            }
            else if (e.CommandName.Equals("excluir"))
            {
                Object id = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                ContratoADM contrato = new ContratoADM(id);

                try
                {
                    contrato.Remover();
                    this.CarregaContratos();
                }
                catch
                {
                    //base.Alerta(null, this, "_contrErrExcl", "Erro ao excluir contrato.\\nNão é possível excluir contratos que já foram usados dentro do sistema.");
                    base.Alerta(MPE, ref litAlert, "Erro ao excluir contrato.<br>Não é possível excluir contratos que já foram usados dentro do sistema.", upnlAlerta);
                }
            }
            else if (e.CommandName.Equals("ativar"))
            {
                Object id = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                ContratoADM contrato = new ContratoADM(id);
                contrato.Carregar();
                contrato.Ativo = !contrato.Ativo;
                contrato.Salvar();
                this.CarregaContratos();
            }
        }

        protected void gridContratos_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UIHelper.AuthWebCtrl(e.Row.Cells[4], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                UIHelper.AuthWebCtrl(e.Row.Cells[5], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);

                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente prosseguir com a exclusão?");
                base.grid_RowDataBound_Confirmacao(sender, e, 5, "Deseja realmente prosseguir?");
                Boolean ativo = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][1]);

                if (!ativo)
                {
                    e.Row.ForeColor = System.Drawing.Color.FromName("#CC0000");
                    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='images/unactive.png' title='inativo' alt='inativo' border='0'>";
                }
                else
                    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='images/active.png' title='ativo' alt='ativo' border='0'>";
            }
        }

        protected void cmdNovoContrato_Click(Object sender, EventArgs e)
        {
            gridContratos.SelectedIndex = -1;
            pnlListaContratos.Visible = false;
            pnlDetalheContrato.Attributes.Add("style", "display:");  //pnlDetalheContrato.Visible = true;
            txtContratoData.Text = "";
            txtContratoDescricao.Text = "";
            txtContratoNumero.Text = "";
            txtContratoSaude.Text = "";
            txtContratoDental.Text = "";
            txtContratoCodFilial.Text = "";
            txtContratoCodUnidade.Text = "";
            txtContratoCodAdministradora.Text = "";
            chkContratoAtivo.Checked = true;
        }

        protected void cmdFecharContrato_Click(Object sender, EventArgs e)
        {
            gridContratos.SelectedIndex = -1;
            pnlListaContratos.Visible = true;
            pnlDetalheContrato.Attributes.Add("style", "display:none");  //pnlDetalheContrato.Visible = false;
        }

        protected void cmdSalvarContrato_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (cboContratoEstipulante.Items.Count == 0)
            {
                //base.Alerta(null, this, "errCont00", "Atenção!\\nNão há estipulante selecionado.");
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há estipulante selecionado.", upnlAlerta);
                tab.ActiveTabIndex = 1;
                cboContratoEstipulante.Focus();
                return;
            }

            if (txtContratoDescricao.Text.Trim() == "")
            {
                //base.Alerta(null, this, "errCont01", "Atenção!\\nInforme a descrição do contrato.");
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Informe a descrição do contrato.", upnlAlerta);
                tab.ActiveTabIndex = 1;
                txtContratoDescricao.Focus();
                return;
            }

            if (txtContratoNumero.Text.Trim() == "")
            {
                //base.Alerta(null, this, "errCont02", "Atenção!\\nInforme o número do contrato.");
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Informe o número do contrato.", upnlAlerta);
                tab.ActiveTabIndex = 1;
                txtContratoNumero.Focus();
                return;
            }

            if (txtContratoData.Text.Trim() != "")
            {
                DateTime dte;
                if (!UIHelper.TyParseToDateTime(txtContratoData.Text, out dte))
                {
                    //base.Alerta(null, this, "_erroContDt", "Data inválida.");
                    base.Alerta(MPE, ref litAlert, "Data inválida.", upnlAlerta);
                    txtContratoData.Focus();
                    return;
                }
            }
            else
            {
                //base.Alerta(null, this, "_erroContDt_b", "Informe a data do contrato.");
                base.Alerta(MPE, ref litAlert, "Informe a data do contrato.", upnlAlerta);
                txtContratoData.Focus();
                return;
            }

            Object id = null;
            if (gridContratos.SelectedIndex > -1)
                id = gridContratos.DataKeys[gridContratos.SelectedIndex].Value;

            if (ContratoADM.ExisteNumero(id, txtContratoNumero.Text, cboContratoEstipulante.SelectedValue, ViewState[IDKey]))
            {
                //base.Alerta(null, this, "errCont03", "Atenção!\\nO número do contrato informado já existe.");
                base.Alerta(MPE, ref litAlert, "Atenção!<br>O número do contrato informado já existe.", upnlAlerta);
                tab.ActiveTabIndex = 1;
                txtContratoNumero.Focus();
                return;
            }

            if (ContratoADM.ExisteDescricao(id, txtContratoDescricao.Text, ViewState[IDKey]))
            {
                //base.Alerta(null, this, "errCont04", "Atenção!\\nA descrição do contrato informado já existe.");
                base.Alerta(MPE, ref litAlert, "Atenção!<br>A descrição do contrato informado já existe.", upnlAlerta);
                tab.ActiveTabIndex = 1;
                txtContratoDescricao.Focus();
                return;
            }

            #endregion

            ContratoADM contrato = new ContratoADM();

            if (gridContratos.SelectedIndex > -1)
            {
                contrato.ID = gridContratos.DataKeys[gridContratos.SelectedIndex].Value;
            }

            contrato.Descricao = txtContratoDescricao.Text;
            contrato.Numero = txtContratoNumero.Text;
            contrato.ContratoSaude = txtContratoSaude.Text;
            contrato.ContratoDental = txtContratoDental.Text;
            contrato.EstipulanteID = cboContratoEstipulante.SelectedValue;
            contrato.Ativo = chkContratoAtivo.Checked;
            contrato.Data = base.CStringToDateTime(txtContratoData.Text);
            contrato.OperadoraID = this.ViewState[IDKey];
            contrato.CodFilial = txtContratoCodFilial.Text;
            contrato.CodUnidade = txtContratoCodUnidade.Text;
            contrato.CodAdministradora = txtContratoCodAdministradora.Text;
            contrato.Salvar();
            this.CarregaContratos();

            pnlListaContratos.Visible = true;
            pnlDetalheContrato.Attributes.Add("style", "display:none");  //pnlDetalheContrato.Visible = false;
            gridContratos.SelectedIndex = -1;
        }

        #endregion

        #region Adicionais

        void CarregaAdicionais()
        {
            IList<Adicional> adicionais = Adicional.CarregarPorOperadoraID(ViewState[IDKey]);
            gridAdicional.DataSource = adicionais;
            gridAdicional.DataBind();
            cmdOcultarAdicionalFaixa_Click(null, null);

            cboPlanoAdicional_Adicional.DataValueField = "ID";
            cboPlanoAdicional_Adicional.DataTextField = "Descricao";
            cboPlanoAdicional_Adicional.DataSource = adicionais;
            cboPlanoAdicional_Adicional.DataBind();
        }

        void CarregaAdicionalFaixa(Object adicionalId)
        {
            //this.Itens = (List<AdicionalFaixa>)AdicionalFaixa.CarregarPorTabela(adicionalId);
            gridAdicionalFaixa.DataSource = (List<AdicionalFaixa>)AdicionalFaixa.CarregarPorTabela(adicionalId, null);
            gridAdicionalFaixa.DataBind();
            tblAdicionalFaixa.Visible = true;
        }

        protected void gridAdicional_OnRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                if (ViewState[IDKey] == null)
                {
                    //base.Alerta(null, this, "errAd01", "Você deve primeiramente salvar o cadastro desta operadora."); 
                    base.Alerta(MPE, ref litAlert, "Você deve primeiramente salvar o cadastro desta operadora.", upnlAlerta);
                    return; 
                }

                pnlDetalheAdicional.Visible = true;
                pnlListaAdicionais.Visible = false;

                gridAdicional.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                Object id = gridAdicional.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Adicional ad = new Adicional();
                ad.ID = id;
                ad.Carregar();
                cboAdicionalTipo.SelectedValue = ad.Tipo.ToString();
                txtAdicionalDescricao.Text = ad.Descricao;
                txtAdicionalCodigoTitular.Text = ad.CodTitular;
                chkAdicionalTodaProposta.Checked = ad.ParaTodaProposta;
                chkDental.Checked = ad.Dental;
                //if (ad.ValorUnico > 0) { txtAdicionalValorUnico.Text = ad.ValorUnico.ToString("N2"); }
                //else { txtAdicionalValorUnico.Text = ""; }
                //txtAdicionalValorUnico.Attributes.Add("onKeyUp", "mascara('" + txtAdicionalValorUnico.ClientID + "')");
                //txtAdicionalData.Text = ad.Data.ToString("dd/MM/yyyy");

                IList<AdicionalFaixa> lista = AdicionalFaixa.CarregarPorTabela(ad.ID, null);
                gridAdicionalFaixaNovo.DataSource = lista;
                gridAdicionalFaixaNovo.DataBind();
                this.Itens = (List<AdicionalFaixa>)lista;
            }
            else if (e.CommandName.Equals("detalhes"))
            {
                Object id = gridAdicional.DataKeys[Convert.ToInt32(e.CommandArgument)][0];

                if (id == null)
                { 
                    //base.Alerta(null, this, "errAd01", "Você deve primeiramente salvar o cadastro desta operadora."); 
                    base.Alerta(MPE, ref litAlert, "Você deve primeiramente salvar o cadastro desta operadora.", upnlAlerta);
                    return;
                }

                gridAdicional.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                this.CarregaAdicionalFaixa(id);
            }
            else if (e.CommandName.Equals("ativar"))
            {
                Object id = gridAdicional.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Adicional ad = new Adicional();
                ad.ID = id;
                ad.Carregar();
                ad.Ativo = !ad.Ativo;
                ad.Salvar();
                this.CarregaAdicionais();
            }
        }

        protected void gridAdicional_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UIHelper.AuthWebCtrl(e.Row.Cells[2], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente prosseguir?");

                Boolean ativo = Convert.ToBoolean(gridAdicional.DataKeys[e.Row.RowIndex][1]);

                if (!ativo)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Text = "<img src='images/unactive.png' title='inativo' alt='inativo' border='0'>";
                }
                else
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Text = "<img src='images/active.png' title='ativo' alt='ativo' border='0'>";
            }
        }

        protected void cmdOcultarAdicionalFaixa_Click(Object sender, EventArgs e)
        {
            gridAdicionalFaixa.DataSource = null;
            gridAdicionalFaixa.DataBind();
            cmdOcultarAdicionalFaixa.Visible = false;
            //spanDetalhesTabelaValorSelecionada.Visible = false;
            tblAdicionalFaixa.Visible = false;
            gridAdicional.SelectedIndex = -1;
            this.Itens = null;
        }

        protected void cmdAddItem_Click(Object sender, EventArgs e)
        {
            List<AdicionalFaixa> lista = this.Itens;
            if (lista == null) { lista = new List<AdicionalFaixa>(); }

            lista.Add(new AdicionalFaixa());

            gridAdicionalFaixaNovo.DataSource = lista;
            gridAdicionalFaixaNovo.DataBind();
            this.Itens = lista;
        }

        protected void gridAdicionalFaixaNovo_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UIHelper.AuthWebCtrl(e.Row.Cells[4], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente excluir a linha?");

                TextBox txtvalor1 = (TextBox)e.Row.Cells[3].Controls[1];
                txtvalor1.Attributes.Add("onKeyUp", "mascara('" + txtvalor1.ClientID + "')");

                Object id = gridAdicionalFaixaNovo.DataKeys[e.Row.RowIndex].Value;

                if (id == null)
                {
                    //se a linha nao está salva (nao tem id), seta para "" onde for 0
                    //TextBox txtidade1 = (TextBox)e.Row.Cells[0].Controls[1];
                    //if (txtidade1.Text == "0") { txtidade1.Text = ""; }

                    if (CToDecimal(txtvalor1.Text) == 0) { txtvalor1.Text = ""; }
                }
            }
        }

        protected void gridAdicionalFaixaNovo_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                Object id = gridAdicionalFaixaNovo.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                List<AdicionalFaixa> lista = this.Itens;
                lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                this.Itens = lista;
                gridAdicionalFaixaNovo.DataSource = lista;
                gridAdicionalFaixaNovo.DataBind();

                if (id != null)
                {
                    AdicionalFaixa item = new AdicionalFaixa();
                    item.ID = id;
                    item.Remover();
                }
            }
        }

        void CarregaAdicionalFaixa_DETALHE(Object adicionalId)
        {
            if (adicionalId != null)
            {
                List<AdicionalFaixa> lista =
                    (List<AdicionalFaixa>)AdicionalFaixa.CarregarPorTabela(adicionalId, null);

                gridAdicionalFaixaNovo.DataSource = lista;
                gridAdicionalFaixaNovo.DataBind();
                this.Itens = lista;
            }
            else
            {
                this.MontaGridParaPrimeiraInsercao_ADICIONAL_FAIXA();
            }
        }

        void MontaGridParaPrimeiraInsercao_ADICIONAL_FAIXA()
        {
            List<AdicionalFaixa> lista = new List<AdicionalFaixa>();

            for (int i = 1; i <= 1; i++)
                lista.Add(new AdicionalFaixa());

            gridAdicionalFaixaNovo.DataSource = lista;
            gridAdicionalFaixaNovo.DataBind();
            this.Itens = lista;

            for (int i = 0; i < gridAdicionalFaixaNovo.Rows.Count; i++)
                ((TextBox)gridAdicionalFaixaNovo.Rows[i].Cells[1].Controls[1]).Text = "";
        }

        protected void cmdNovoAdicional_Click(Object sender, EventArgs e)
        {
            if (ViewState[IDKey] == null) 
            { 
                //base.Alerta(null, this, "errAd01", "Você deve primeiramente salvar o cadastro desta operadora."); 
                base.Alerta(MPE, ref litAlert, "Você deve primeiramente salvar o cadastro desta operadora.", upnlAlerta);
                return;
            }

            gridAdicional.SelectedIndex = -1;
            pnlDetalheAdicional.Visible = true;
            pnlListaAdicionais.Visible = false;
            this.MontaGridParaPrimeiraInsercao_ADICIONAL_FAIXA();
            //txtAdicionalValorUnico.Text = "";
            //txtAdicionalValorUnico.Attributes.Add("onKeyUp", "mascara('" + txtAdicionalValorUnico.ClientID + "')");
        }

        protected void cmdFecharAdicional_Click(Object sender, EventArgs e)
        {
            gridAdicional.SelectedIndex = -1;
            pnlListaAdicionais.Visible = true;
            pnlDetalheAdicional.Visible = false;
            this.Itens = null;
        }

        protected void cmdSalvarAdicional_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (txtAdicionalDescricao.Text.Trim() == "")
            {
                //base.Alerta(null, this, "errAd02", "Atenção!\\nInforme a descrição do produto adicional.");
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Informe a descrição do produto adicional.", upnlAlerta);
                //tab.ActiveTabIndex = 2;
                txtAdicionalDescricao.Focus();
                return;
            }

            //DateTime data = new DateTime();
            //if (!UIHelper.TyParseToDateTime(txtAdicionalData.Text, out data))
            //{
            //    base.Alerta(null, this, "errAd03", "Atenção!\\nData inválida.");
            //    //tab.ActiveTabIndex = 2;
            //    txtAdicionalData.Focus();
            //    return;
            //}

            String msg = UIHelper.ChecaGridIntervaloDeIdades(gridAdicionalFaixaNovo, 1, 2, 0);
            if (msg != String.Empty)
            {
                //base.Alerta(null, this, "errAd04", msg);
                base.Alerta(MPE, ref litAlert, msg, upnlAlerta);
                return;
            }
            #endregion

            //TODO: transacionar em facade...
            Adicional ad = new Adicional();

            if (gridAdicional.SelectedIndex != -1) //está editando um adicional existente.
            {
                ad.ID = gridAdicional.DataKeys[gridAdicional.SelectedIndex].Value;
                ad.Carregar();
            }

            //if (txtAdicionalData.Text.Trim() != "")
            //    ad.Data = base.CStringToDateTime(txtAdicionalData.Text);

            //ad.ValorUnico = base.CToDecimal(txtAdicionalValorUnico.Text);

            ad.Tipo = Convert.ToInt32(cboAdicionalTipo.SelectedValue);
            ad.Descricao = txtAdicionalDescricao.Text;
            ad.CodTitular = txtAdicionalCodigoTitular.Text;
            ad.ParaTodaProposta = chkAdicionalTodaProposta.Checked;
            ad.Dental = chkDental.Checked;
            ad.OperadoraID = ViewState[IDKey];
            ad.Salvar();

            if (this.Itens != null)
            {
                foreach (AdicionalFaixa faixa in this.Itens)
                {
                    faixa.AdicionalId = ad.ID;
                    faixa.Salvar();
                }
            }

            pnlListaAdicionais.Visible = true;
            pnlDetalheAdicional.Visible = false;
            gridAdicional.SelectedIndex = -1;
            this.CarregaAdicionais();
            this.Itens = null;
            //base.Alerta(null, this, "okAd", "Dados salvos com sucesso.");
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
        }

        #endregion

        #region Tabelas de Valores 

        protected void cboTValoresContrato_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelaValores();
            //this.CarregaPlanosParaTabelaDeValor();
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
            //IList<TabelaValorItem> lista = TabelaValorItem.CarregarPorTabela(
            //    gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value, 
            //    gridTVPlanos.DataKeys[gridTVPlanos.SelectedIndex].Value);

            //gridTabelaValoresItem.DataSource = lista;
            //gridTabelaValoresItem.DataBind();
            //cmdOcultarTValoresDetalhes.Visible = lista != null && lista.Count > 0;
            ////spanDetalhesTabelaValorSelecionada.Visible = cmdOcultarTValoresDetalhes.Visible;

            //TabelaValor tv = new TabelaValor();
            //tv.ID = gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value;
            //tv.Carregar();
            //spanDetalhesTabelaValorSelecionada.InnerText = String.Concat(
            //    "Detalhes da tabela selecionada");// (", tv.Descricao, ")");

            //if (lista != null && lista.Count > 0)
            //{
            //    Plano plano = new Plano();
            //    plano.ID = lista[0].PlanoID;
            //    plano.Carregar();

            //    gridTabelaValoresItem.Columns[2].Visible = plano.QuartoComum;
            //    gridTabelaValoresItem.Columns[3].Visible = plano.QuartoParticular;
            //    gridTabelaValoresItem.Columns[4].Visible = plano.QuartoComum;
            //    gridTabelaValoresItem.Columns[5].Visible = plano.QuartoParticular;
            //}
        }

        //void CarregaTabelaValoresDATAS()
        //{
        //    IList<TabelaValorData> lista = TabelaValorData.Carregar(
        //        gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value,
        //        gridTVPlanos.DataKeys[gridTVPlanos.SelectedIndex].Value);

        //    cboTValoresDatas.Items.Clear();
        //    cboTValoresDatas.DataValueField = "ID";
        //    cboTValoresDatas.DataTextField  = "strData";
        //    cboTValoresDatas.DataSource = lista;
        //    cboTValoresDatas.DataBind();
        //}

        protected void gridTabelaValores_OnRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                cboTValoresContratoDETALHE.Enabled = false;
                TabelaValor tabela = new TabelaValor();
                gridTabelaValores.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                tabela.ID = gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value;
                tabela.Carregar();

                //if (tabela.Data != DateTime.MinValue) { txtTValoresData.Text = tabela.Data.ToString("dd/MM/yyyy"); } denis
                if (tabela.Inicio != DateTime.MinValue) { txtTValoresInicio.Text = tabela.Inicio.ToString("dd/MM/yyyy"); }
                if (tabela.Fim != DateTime.MinValue) { txtTValoresFim.Text = tabela.Fim.ToString("dd/MM/yyyy"); }

                Taxa taxa = Taxa.CarregarPorTabela(tabela.ID);
                if (taxa != null)
                {
                    txtFixo.Text = taxa.Fixo.ToString("N2");
                    txtOverPrice.Text = taxa.Over.ToString("N2");
                    txtValorEmbutido.Text = taxa.ValorEmbutido.ToString("N2");
                    chkValorEmbutido.Checked = taxa.Embutido;
                }

                cboTValoresContratoDETALHE.SelectedValue = Convert.ToString(tabela.ContratoID);
                cboTValoresContratoDETALHE_OnSelectedIndexChanged(null, null);

                this.CarregaPlanosParaTabelaDeValor();
                pnlTVPlanos.Visible = gridTVPlanos.Rows.Count > 0;

                pnlTabelaValorLista.Visible = false;
                pnlTabelaValorDetalhe.Visible = true;
                //this.ChecaDataDeContratoETabelaDeValor();
                litTVSemPlano.Text = "";


                pnlTabelaValorITENS.Visible = true;
                pnlTabelaValorITENS_ListaPlanos.Visible = false;
            }
            else if (e.CommandName == "duplicar")
            {
                litTabelaValorDuplicarIDs.Text = Convert.ToString(gridTabelaValores.DataKeys[Convert.ToInt32(e.CommandArgument)].Value) + ";" + cboTValoresContrato.SelectedValue; ;
                pnlTabelaValorLista.Visible = false;
                pnlTabelaValorDuplicar.Visible = true;
                txtTValoresInicioDUPLICAR.Focus(); //txtTValoresDataDUPLICAR.Focus(); denis
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
            #region comentado
            //else if (e.CommandName.Equals("detalhes"))
            //{
            //    Object id = gridTabelaValores.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
            //    gridTabelaValores.SelectedIndex = Convert.ToInt32(e.CommandArgument);
            //    this.CarregaTabelaValoresITENS();
            //    tblTabelaValoresItem.Visible = true;
            //}
            //else if (e.CommandName.Equals("ativar"))
            //{
            //    Object tabelaValoresId = gridTabelaValores.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
            //    String planoId = cboPlanoTValores.SelectedValue;
            //    Plano.SetaTabelaValorAutal(planoId, tabelaValoresId, null);
            //    this.CarregaTabelaValores();
            //}
            #endregion
        }

        protected void gridTabelaValores_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja realmente excluir?");
            base.grid_RowDataBound_Confirmacao(sender, e, 9, "Deseja realmente duplicar essa tabela?");
            base.grid_RowDataBound_Confirmacao(sender, e, 11, "Recalcular limites de vencimento para essa tabela?");
        }

        protected void cmdOcultarTValoresDetalhes_Click(Object sender, EventArgs e)
        {
            //gridTabelaValoresItem.DataSource = null;
            //gridTabelaValoresItem.DataBind();
            //cmdOcultarTValoresDetalhes.Visible = false;
            //spanDetalhesTabelaValorSelecionada.Visible = false;
            //tblTabelaValoresItem.Visible = false;
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
            #region validacoes 

            //DateTime data = new DateTime();  DENIS /////////////////////////////////////////////////////
            //if (!UIHelper.TyParseToDateTime(txtTValoresDataDUPLICAR.Text, out data))
            //{
            //    //base.Alerta(null, this, "_errDup0", "A data de vigência informada está inválida.");
            //    base.Alerta(MPE, ref litAlert, "A data de vigência informada está inválida.", upnlAlerta);
            //    txtTValoresDataDUPLICAR.Focus();
            //    return;
            //}

            DateTime dtInicio = new DateTime();
            if (!UIHelper.TyParseToDateTime(txtTValoresInicioDUPLICAR.Text, out dtInicio))
            {
                base.Alerta(MPE, ref litAlert, "A data de início informada está inválida.", upnlAlerta);
                txtTValoresInicioDUPLICAR.Focus();
                return;
            }
            DateTime dtFim = new DateTime();
            if (!UIHelper.TyParseToDateTime(txtTValoresFimDUPLICAR.Text, out dtFim))
            {
                base.Alerta(MPE, ref litAlert, "A data final informada está inválida.", upnlAlerta);
                txtTValoresFimDUPLICAR.Focus();
                return;
            }

            if (dtInicio > dtFim)
            {
                base.Alerta(MPE, ref litAlert, "A data de início deve ser maior que a data final.", upnlAlerta);
                return;
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////

            if (txtOverPriceDuplicar.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_errDup2", "Informe a taxa de over price.");
                base.Alerta(MPE, ref litAlert, "Informe a taxa de over price.", upnlAlerta);
                txtOverPriceDuplicar.Focus();
                return;
            }

            if (txtFixoDuplicar.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_errDup1", "Informe a taxa fixa.");
                base.Alerta(MPE, ref litAlert, "Informe a taxa fixa.", upnlAlerta);
                txtFixoDuplicar.Focus();
                return;
            }

            if (txtValorEmbutidoDuplicar.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_errDup4", "Informe a tarifa.");
                base.Alerta(MPE, ref litAlert, "Informe a taxa tarifa.", upnlAlerta);
                txtValorEmbutidoDuplicar.Focus();
                return;
            }

            String tabelaId = litTabelaValorDuplicarIDs.Text.Split(';')[0];
            String contratoAdmId = litTabelaValorDuplicarIDs.Text.Split(';')[1];

            //DateTime vigencia = base.CStringToDateTime(txtTValoresDataDUPLICAR.Text); denis
            if (TabelaValor.ExisteTabelaComVigencia(dtInicio, dtFim, contratoAdmId, null))//if (TabelaValor.ExisteTabelaComVigencia(vigencia, contratoAdmId, null)) denis
            {
                //base.Alerta(null, this, "_errDup3", "Já existe uma tabela de valor com essa data de vigência.");
                base.Alerta(MPE, ref litAlert, "Já existe uma tabela de valor com essa data de vigência.", upnlAlerta);
                txtTValoresInicioDUPLICAR.Focus(); //txtTValoresDataDUPLICAR.Focus(); denis
                return;
            }

            #endregion

            Taxa taxa = new Taxa();
            taxa.Data = dtInicio; //vigencia;denis
            taxa.Embutido = true;
            taxa.Fixo = base.CToDecimal(txtFixoDuplicar.Text);
            taxa.Over = base.CToDecimal(txtOverPriceDuplicar.Text);
            taxa.Embutido = chkValorEmbutidoDuplicar.Checked;
            taxa.ValorEmbutido = base.CToDecimal(txtValorEmbutidoDuplicar.Text);
            taxa.PercentualReajuste = base.CToDecimal(txtReajusteDuplicar.Text);

            TabelaDeValorFacade.Instance.Duplicar(tabelaId, dtInicio, dtFim, taxa);

            pnlTabelaValorDuplicar.Visible = false;
            pnlTabelaValorLista.Visible = true;
            this.CarregaTabelaValores();
            litTabelaValorDuplicarIDs.Text = "";

            //base.Alerta(null, this, "_okTValDup", "Tabela duplicada com sucesso.");
            base.Alerta(MPE, ref litAlert, "Tabela duplicada com sucesso.", upnlAlerta);
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
                gridTVPlanos.SelectedIndex  = Convert.ToInt32(e.CommandArgument);

                //carrega os itens
                if (gridTabelaValores.SelectedIndex > -1)
                {
                    this.CarregaItensDaTabelaDeValores();

                }
                else
                {
                    Plano plano = new Plano(gridTVPlanos.DataKeys[gridTVPlanos.SelectedIndex].Value);
                    plano.Carregar();
                    gridTabelaValoresItemDETALHE.Columns[2].Visible = plano.QuartoComum;
                    gridTabelaValoresItemDETALHE.Columns[3].Visible = plano.QuartoParticular;
                    gridTabelaValoresItemDETALHE.Columns[4].Visible = plano.QuartoComum;
                    gridTabelaValoresItemDETALHE.Columns[5].Visible = plano.QuartoParticular;
                }
            }
        }

        Boolean ChecaDataDeContratoETabelaDeValor()
        {
            if (!base.HaItemSelecionado(cboTValoresContratoDETALHE))
            {
                //base.Alerta(null, this, "_erroTvNovoCheca", "Informe o contrato da tabela de valores.");
                base.Alerta(MPE, ref litAlert, "Informe o contrato da tabela de valores.", upnlAlerta);
                cboTValoresContratoDETALHE.Focus();
                cmdSalvarTabelaValor.Enabled = false;
                return false;
            }

            Plano plano = new Plano(gridTVPlanos.DataKeys[gridTVPlanos.SelectedIndex].Value);
            plano.Carregar();

            ContratoADM contrato = new ContratoADM(plano.ContratoID);
            contrato.Carregar();

            //txtTValoresData.Text = contrato.Data.ToString("dd/MM/yyyy"); denis
            txtTValoresInicio.Text = contrato.Data.ToString("dd/MM/yyyy");

            Object tabelaSelecionadaID = null;
            if(gridTabelaValores.SelectedIndex > -1)
                tabelaSelecionadaID = gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value;

            IList<TabelaValor> tabelas = TabelaValor.CarregarPorContratoID(cboTValoresContrato.SelectedValue);
            if (tabelas == null || tabelas.Count == 0)
            {
                //txtTValoresData.ReadOnly = true; denis
                txtTValoresInicio.ReadOnly = true;
                cmdSalvarTabelaValor.Enabled = true;
                return true;
            }

            if(tabelas.Count == 1 && tabelaSelecionadaID != null && Convert.ToString(tabelaSelecionadaID) == Convert.ToString(tabelas[0].ID))
                txtTValoresInicio.ReadOnly = true; //txtTValoresData.ReadOnly = true; denis
            else
                txtTValoresInicio.ReadOnly = false; //txtTValoresData.ReadOnly = false; denis

            cmdSalvarTabelaValor.Enabled = true;
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

            gridTabelaValoresItemDETALHE.Columns[2].Visible = plano.QuartoComum;
            gridTabelaValoresItemDETALHE.Columns[3].Visible = plano.QuartoParticular;
            gridTabelaValoresItemDETALHE.Columns[4].Visible = plano.QuartoComum;
            gridTabelaValoresItemDETALHE.Columns[5].Visible = plano.QuartoParticular;

            upPlanos.Update();
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
            cboTValoresContratoDETALHE.Enabled = true;
            gridTabelaValores.SelectedIndex = -1;
            pnlTabelaValorLista.Visible = false;
            pnlTabelaValorDetalhe.Visible = true;
            this.MontaGridParaPrimeiraInsercao_TabelaValoresItens();
            txtTValoresFim.Text = "";
            txtTValoresInicio.Text = "";

//            cboTValoresContratoDETALHE_OnSelectedIndexChanged(null, null);

            this.CarregaPlanosParaTabelaDeValor();
            pnlTVPlanos.Visible = gridTVPlanos.Rows.Count > 0;

            pnlTabelaValorLista.Visible = false;
            pnlTabelaValorDetalhe.Visible = true;
            //this.ChecaDataDeContratoETabelaDeValor();
            cboTValoresContratoDETALHE_OnSelectedIndexChanged(null, null);
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
            //cboTValoresPlanoDETALHE.DataValueField = "ID";
            //cboTValoresPlanoDETALHE.DataTextField = "Descricao";
            //cboTValoresPlanoDETALHE.DataSource = planos;
            //cboTValoresPlanoDETALHE.DataBind();
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
            #region validacoes 

            if (base.CToDecimal(txtFixo.Text) == 0) { txtFixo.Text = "0"; }
            if (base.CToDecimal(txtOverPrice.Text) == 0) { txtOverPrice.Text = "0"; }
            if (base.CToDecimal(txtValorEmbutido.Text) == 0) { txtValorEmbutido.Text = "0"; }

            DateTime dtFim = new DateTime();
            DateTime dtInicio = new DateTime();

            //denis /////////////////////////////////////////////////////////////////////////////////////////
            //Boolean resultado = UIHelper.TyParseToDateTime(txtTValoresData.Text, out dt);
            //if (!resultado)
            //{
            //    //base.Alerta(null, this, "_errTVal3", "Informe uma data válida.");
            //    base.Alerta(MPE, ref litAlert, "Informe uma data válida.", upnlAlerta);
            //    txtTValoresData.Focus();

            //    if (gridTVPlanos.SelectedIndex == -1)
            //        pnlTabelaValorITENS_ListaPlanos.Visible = false;
            //    return;
            //}
            Boolean resultado = UIHelper.TyParseToDateTime(txtTValoresInicio.Text, out dtInicio);
            if (!resultado)
            {
                base.Alerta(MPE, ref litAlert, "Informe uma data de início válida.", upnlAlerta);
                txtTValoresInicio.Focus();

                if (gridTVPlanos.SelectedIndex == -1)
                    pnlTabelaValorITENS_ListaPlanos.Visible = false;
                return;
            }
            resultado = UIHelper.TyParseToDateTime(txtTValoresFim.Text, out dtFim);
            if (!resultado)
            {
                base.Alerta(MPE, ref litAlert, "Informe uma data final válida.", upnlAlerta);
                txtTValoresFim.Focus();

                if (gridTVPlanos.SelectedIndex == -1)
                    pnlTabelaValorITENS_ListaPlanos.Visible = false;
                return;
            }

            if (dtInicio > dtFim)
            {
                base.Alerta(MPE, ref litAlert, "Data de início deve ser menor que a data final.", upnlAlerta);
                return;
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////

            String msg = UIHelper.ChecaGridIntervaloDeIdades(gridTabelaValoresItemDETALHE, 0, 1);
            if (msg != String.Empty)
            {
                //base.Alerta(null, this, "_errTVal4", msg);
                base.Alerta(MPE, ref litAlert, msg, upnlAlerta);
                if (gridTVPlanos.SelectedIndex == -1)
                    pnlTabelaValorITENS_ListaPlanos.Visible = false;
                return;
            }

            ContratoADM contrato = new ContratoADM(cboTValoresContratoDETALHE.SelectedValue);
            contrato.Carregar();
            DateTime dataContrato = new DateTime(contrato.Data.Year, contrato.Data.Month, contrato.Data.Day);
            if (dtInicio < dataContrato)//if (dt < dataContrato) denis
            {
                //base.Alerta(null, this, "_errTVal5", "A data de vigência desta tabela de valores não \\npode ser inferior a " + contrato.Data.ToString("dd/MM/yyyy") + ".");
                //base.Alerta(MPE, ref litAlert, "A data de vigência desta tabela de valores não pode ser inferior a " + contrato.Data.ToString("dd/MM/yyyy") + ".", upnlAlerta); denis
                base.Alerta(MPE, ref litAlert, "A data de início desta tabela de valores não pode ser inferior a " + contrato.Data.ToString("dd/MM/yyyy") + ".", upnlAlerta);
                //txtTValoresData.Focus(); denis
                txtTValoresInicio.Focus();

                if (gridTVPlanos.SelectedIndex == -1)
                    pnlTabelaValorITENS_ListaPlanos.Visible = false;
                return;
            }

            List<TabelaValorItem> lista = this.ItensTValores;

            //DateTime vigencia = base.CStringToDateTime(txtTValoresData.Text); denis
            Object tabelaId = null;
            if (gridTabelaValores.SelectedIndex > -1)
                tabelaId = gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value;

            //if (TabelaValor.ExisteTabelaComVigencia(vigencia, cboTValoresContratoDETALHE.SelectedValue, tabelaId)) denis
            if (TabelaValor.ExisteTabelaComVigencia(dtInicio, dtFim, cboTValoresContratoDETALHE.SelectedValue, tabelaId)) 
            {
                //base.Alerta(null, this, "_errTVal7", "Já existe uma tabela de valor com essa data de vigência.");
                base.Alerta(MPE, ref litAlert, "Já existe uma tabela de valor com essa data de vigência.", upnlAlerta);
                if (gridTVPlanos.SelectedIndex == -1)
                    pnlTabelaValorITENS_ListaPlanos.Visible = false;
                return;
            }
            #endregion

            TabelaValor tabela = new TabelaValor();

            if (gridTabelaValores.SelectedIndex > -1)
            {
                tabela.ID = gridTabelaValores.DataKeys[gridTabelaValores.SelectedIndex].Value;
                tabela.Carregar();
            }

            //tabela.Descricao = txtTValoresDescricao.Text;
            //if (txtTValoresData.Text.Trim() != "") denis
            //    tabela.Data = Convert.ToDateTime(txtTValoresData.Text); denis

            tabela.Inicio = dtInicio;
            tabela.Fim = dtFim;

            tabela.ContratoID = cboTValoresContratoDETALHE.SelectedValue;

            DateTime vigencia, vencimentoDe, vencimentoAte = DateTime.MinValue;
            int diaSemJuros = 0;
            Object valorLimite = null;
            CalendarioVencimento rcv = null;

            #region calcula limites 
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
            #endregion

            tabela.Salvar();

            Taxa taxa = Taxa.CarregarPorTabela(tabela.ID);
            if (taxa == null) { taxa = new Taxa(); }
            taxa.Data = tabela.Inicio; //tabela.Data; denis
            taxa.Embutido = true;
            taxa.Fixo = CToDecimal(txtFixo.Text);
            taxa.Over = CToDecimal(txtOverPrice.Text);
            taxa.ValorEmbutido = CToDecimal(txtValorEmbutido.Text);
            taxa.Embutido = chkValorEmbutido.Checked;
            taxa.TabelaValorID = tabela.ID;
            taxa.Salvar();

            if (gridTVPlanos.SelectedIndex > -1)
            {
                foreach (TabelaValorItem item in lista)
                {
                    item.TabelaID = tabela.ID;
                    item.PlanoID = gridTVPlanos.DataKeys[gridTVPlanos.SelectedIndex].Value;
                    item.AplicaTaxa(taxa, false);
                    item.Salvar();
                }

                this.ItensTValores = lista;
                gridTabelaValoresItemDETALHE.DataSource = lista;
                gridTabelaValoresItemDETALHE.DataBind();

                this.CarregaTabelaValores();
                //cmdTabelaValorFechar_Click(null, null);
                for (int i = 0; i < gridTabelaValores.Rows.Count; i++) //seleciona no grid a tabela recem salva
                {
                    if (Convert.ToString(gridTabelaValores.DataKeys[i].Value) ==
                        Convert.ToString(tabela.ID))
                    {
                        gridTabelaValores.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                pnlTabelaValorITENS_ListaPlanos.Visible = false;
            }

            //base.Alerta(null, this, "_okTVal", "Dados salvos com sucesso.");
            TabelaDeValorFacade.Instance.RecalcularTaxaEmPlanos(tabela.ID, taxa);
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
        }

        #endregion

        #region Tabelas de Reajuste 

        protected void cboTReajusteContrato_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelaReajustes();
        }

        void CarregaTabelaReajustes()
        {
            if (cboTReajusteContrato.Items.Count == 0)
            {
                gridTabelaReajustes.DataSource = null;
                gridTabelaReajustes.DataBind();
                //litNenhumaTabelaReajuste.Text = " (nenhuma)";
                return;
            }

            gridTabelaReajustes.DataSource = TabelaReajuste.CarregarTodas(cboTReajusteContrato.SelectedValue);
            gridTabelaReajustes.DataBind();
            cmdOcultarTReajustesDetalhes_Click(null, null);
            //if (gridTabelaReajustes.Rows.Count == 0) { litNenhumaTabelaReajuste.Text = " (nenhuma)"; }
            //else { litNenhumaTabelaReajuste.Text = ""; }
        }

        void CarregaTabelaReajustesITENS(Object tabelaReajusteId)
        {
            IList<TabelaReajusteItem> lista = TabelaReajusteItem.CarregarPorTabela(tabelaReajusteId);

            gridTabelaReajustesItem.DataSource = lista;
            gridTabelaReajustesItem.DataBind();
            cmdOcultarTReajustesDetalhes.Visible = lista != null && lista.Count > 0;
            tblTabelaReajustesItem.Visible = cmdOcultarTReajustesDetalhes.Visible;

            TabelaReajuste tr = new TabelaReajuste();
            tr.ID = tabelaReajusteId;
            tr.Carregar();
            spanDetalhesTabelaReajusteSelecionada.InnerText = String.Concat(
                "Detalhes da tabela ", tr.Descricao);

            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("IdadeInicio");
            dt.Columns.Add("Reajuste");
        }

        protected void gridTabelaReajustes_OnRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                TabelaReajuste tabela = new TabelaReajuste();
                gridTabelaReajustes.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                tabela.ID = gridTabelaReajustes.DataKeys[gridTabelaReajustes.SelectedIndex].Value;
                tabela.Carregar();

                txtTRDescricao.Text = tabela.Descricao;
                if (tabela.Data != DateTime.MinValue) { txtTRData.Text = tabela.Data.ToString("dd/MM/yyyy"); }
                cboTRContratoDETALHE.SelectedValue = Convert.ToString(tabela.ContratoID);

                //carrega os itens
                List<TabelaReajusteItem> lista = (List<TabelaReajusteItem>)TabelaReajusteItem.
                    CarregarPorTabela(gridTabelaReajustes.DataKeys[gridTabelaReajustes.SelectedIndex].Value);

                gridTReajusteItemDETALHE.DataSource = lista;
                gridTReajusteItemDETALHE.DataBind();
                this.ItensTReajuste = lista;

                pnlTReajusteLista.Visible = false;
                pnlTReajusteDetalhe.Visible = true;
            }
            else if (e.CommandName.Equals("detalhes"))
            {
                Object id = gridTabelaReajustes.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                this.CarregaTabelaReajustesITENS(id);
                gridTabelaReajustes.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                tblTabelaReajustesItem.Visible = true;
            }
            else if (e.CommandName.Equals("ativar"))
            {
                if (ViewState[IDKey] == null) 
                {
                    //base.Alerta(null, this, "__tr", "Você deve primeiramente gravar o cadastro da operadora.");
                    base.Alerta(MPE, ref litAlert, "Você deve primeiramente gravar o cadastro da operadora.", upnlAlerta);
                    return; 
                }

                Object tabelaReajusteId = gridTabelaReajustes.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                ContratoADM.SetaTabelaReajusteAutal(ViewState[IDKey], tabelaReajusteId, null);
                this.CarregaTabelaReajustes();
            }
        }

        protected void gridTabelaReajustes_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente tornar essa tabela a atual do plano?");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Object corrente = gridTabelaReajustes.DataKeys[e.Row.RowIndex][1];

                if (corrente == null)
                    e.Row.Cells[2].Text = "NAO";
                else
                    e.Row.Cells[2].Text = "SIM";
            }
        }

        protected void cmdOcultarTReajustesDetalhes_Click(Object sender, EventArgs e)
        {
            gridTabelaReajustesItem.DataSource = null;
            gridTabelaReajustesItem.DataBind();
            cmdOcultarTReajustesDetalhes.Visible = false;
            tblTabelaReajustesItem.Visible = false;
            gridTabelaReajustes.SelectedIndex = -1;
        }

        //DETALHE

        protected void cmdNovaTabelaReajuste_Click(Object sender, EventArgs e)
        {
            gridTabelaReajustes.SelectedIndex = -1;
            pnlTReajusteLista.Visible = false;
            pnlTReajusteDetalhe.Visible = true;
            this.MontaGridParaPrimeiraInsercao_TabelaReajusteItem();
        }

        void MontaGridParaPrimeiraInsercao_TabelaReajusteItem()
        {
            List<TabelaReajusteItem> lista = new List<TabelaReajusteItem>();
            for (int i = 1; i <= 1; i++)
            {
                lista.Add(new TabelaReajusteItem());
            }

            gridTReajusteItemDETALHE.DataSource = lista;
            gridTReajusteItemDETALHE.DataBind();
            this.ItensTReajuste = lista;
        }

        void CarregaItensDaTabelaDeReajuste()
        {
            List<TabelaReajusteItem> lista = (List<TabelaReajusteItem>)TabelaReajusteItem.
                CarregarPorTabela(gridTabelaReajustes.DataKeys[gridTabelaReajustes.SelectedIndex].Value);

            gridTReajusteItemDETALHE.DataSource = lista;
            gridTReajusteItemDETALHE.DataBind();
            this.ItensTReajuste = lista;
        }

        protected void cmdAddItemTabelaReajuste_Click(Object sender, EventArgs e)
        {
            List<TabelaReajusteItem> lista = this.ItensTReajuste;
            if (lista == null) { lista = new List<TabelaReajusteItem>(); }

            lista.Add(new TabelaReajusteItem());

            gridTReajusteItemDETALHE.DataSource = lista;
            gridTReajusteItemDETALHE.DataBind();
            this.ItensTReajuste = lista;
        }

        protected void cmdTabelaReajusteFechar_Click(Object sender, EventArgs e)
        {
            pnlTReajusteLista.Visible = true;
            pnlTReajusteDetalhe.Visible = false;
            gridTabelaReajustes.SelectedIndex = -1;
            this.ItensTReajuste = null;
        }

        protected void gridTReajusteItemDETALHE_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "excluir")
            {
                Object id = gridTReajusteItemDETALHE.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                if (id == null)
                {
                    List<TabelaReajusteItem> lista = this.ItensTReajuste;
                    lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                    this.ItensTReajuste = lista;
                    gridTReajusteItemDETALHE.DataSource = lista;
                    gridTReajusteItemDETALHE.DataBind();
                }
                else
                {
                    TabelaReajusteItem item = new TabelaReajusteItem();
                    item.ID = id;
                    item.Carregar();
                    item.Remover();
                    this.CarregaItensDaTabelaDeReajuste();
                }
            }
        }

        protected void gridTReajusteItemDETALHE_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente excluir a linha?");

                TextBox txtvalor1 = (TextBox)e.Row.Cells[1].Controls[1];
                txtvalor1.Attributes.Add("onKeyUp", "mascara('" + txtvalor1.ClientID + "')");
                if (txtvalor1.Text == "0" || CToDecimal(txtvalor1.Text) == 0) { txtvalor1.Text = ""; }

                Object id = gridTReajusteItemDETALHE.DataKeys[e.Row.RowIndex].Value;

                if (id == null)
                {
                    //se a linha nao está salva (nao tem id), seta para "" onde for 0
                    //TextBox txtidade1 = (TextBox)e.Row.Cells[0].Controls[1];
                    //if (txtidade1.Text == "0" || CToDecimal(txtvalor1.Text) == 0) { txtidade1.Text = ""; }
                }
            }
        }

        protected void cmdSalvarTabelaReajuste_Click(Object sender, EventArgs e)
        {
            #region validacoes

            if (cboTValoresContratoDETALHE.Items.Count == 0)
            {
                //base.Alerta(null, this, "_errTR1", "Não há um contrato selecionado.");
                base.Alerta(MPE, ref litAlert, "Não há um contrato selecionado.", upnlAlerta);
                return;
            }

            if (txtTRDescricao.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_errTR2", "Informe a descrição.");
                base.Alerta(MPE, ref litAlert, "Informe a descrição.", upnlAlerta);
                txtTRDescricao.Focus();
                return;
            }

            if (txtTRData.Text.Trim() != "")
            {
                DateTime dt = new DateTime();
                Boolean resultado = UIHelper.TyParseToDateTime(txtTRData.Text, out dt);
                if (!resultado)
                {
                    //base.Alerta(null, this, "_errTR3", "Informe uma data válida.");
                    base.Alerta(MPE, ref litAlert, "Informe uma data válida.", upnlAlerta);
                    txtTRData.Focus();
                    return;
                }
            }
            else
                txtTRData.Text = DateTime.Now.ToString("dd/MM/yyyy");

            String msg = UIHelper.ChecaGridIdades(gridTReajusteItemDETALHE, 0);
            if (msg != String.Empty)
            {
                //base.Alerta(null, this, "_errTR4", msg);
                base.Alerta(MPE, ref litAlert, msg, upnlAlerta);
                return;
            }

            #endregion

            TabelaReajuste tabela = new TabelaReajuste();

            if(gridTabelaReajustes.SelectedIndex > -1)
                tabela.ID = gridTabelaReajustes.DataKeys[gridTabelaReajustes.SelectedIndex].Value;

            tabela.Descricao = txtTRDescricao.Text;
            tabela.Data = Convert.ToDateTime(txtTRData.Text);

            tabela.ContratoID = cboTValoresContratoDETALHE.SelectedValue;
            tabela.Salvar();

            if (this.ItensTReajuste != null)
            {
                List<TabelaReajusteItem> lista = this.ItensTReajuste;
                foreach (TabelaReajusteItem item in lista)
                {
                    item.TabelaID = tabela.ID;
                    item.Salvar();
                }
            }

            cmdTabelaReajusteFechar_Click(null, null);
            this.CarregaTabelaReajustes();
            //base.Alerta(null, this, "_okTRea", "Dados salvos com sucesso.");
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
        }

        #endregion

        #region Planos x Adicionais 

        protected void cboPlanoAdicional_Contrato_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            cboPlanoAdicional_Plano.Items.Clear();

            if (cboPlanoAdicional_Contrato.Items.Count > 0)
            {
                cboPlanoAdicional_Plano.DataValueField = "ID";
                cboPlanoAdicional_Plano.DataTextField = "Descricao";
                cboPlanoAdicional_Plano.DataSource = Plano.CarregarPorContratoID(cboPlanoAdicional_Contrato.SelectedValue);
                cboPlanoAdicional_Plano.DataBind();
            }

            cboPlanoAdicional_Plano_OnSelectedIndexChanged(null, null);
        }

        protected void cboPlanoAdicional_Plano_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaAdicionaisEmPlano();
        }

        protected void cmdAddAdicional_Click(Object sender, EventArgs e)
        {
            if (cboPlanoAdicional_Adicional.Items.Count == 0) { return; }

            ContratoADMPlanoAdicional apa = new ContratoADMPlanoAdicional();
            apa.AdicionalID = cboPlanoAdicional_Adicional.SelectedValue;
            apa.ContratoID = cboPlanoAdicional_Contrato.SelectedValue;
            apa.PlanoID = cboPlanoAdicional_Plano.SelectedValue;
            apa.Salvar();

            this.CarregaAdicionaisEmPlano();
        }

        void CarregaAdicionaisEmPlano()
        {
            if (cboPlanoAdicional_Plano.Items.Count > 0)
            {
                gridPlanoAdicional.DataSource = ContratoADMPlanoAdicional.
                    Carregar(cboPlanoAdicional_Contrato.SelectedValue, cboPlanoAdicional_Plano.SelectedValue);
            }
            else
                gridPlanoAdicional.DataSource = null;

            gridPlanoAdicional.DataBind();
        }

        protected void gridPlanoAdicional_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "excluir")
            {
                Object id = gridPlanoAdicional.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                ContratoADMPlanoAdicional cpa = new ContratoADMPlanoAdicional(id);
                cpa.Remover();
                this.CarregaAdicionaisEmPlano();
                //base.Alerta(null, this, "_err", "Função temporariamente indisponível.\\nSe viável, você poderá inativar este adicional na aba \"Adicionais\".");
            }
        }

        protected void gridPlanoAdicional_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja realmente excluir a linha?");
        }

        #endregion

        #region Comissionamento 

        //void MontaTabsComissionamento(Int32 index)
        //{

        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "montaTabs", "$(function() { $('#subtab_contrato').tabs(" + index + ");});", true);
        //}

        void CarregarComissionamentos()
        {
            if (cboComissionamentoContrato.Items.Count > 0)
                gridComissionamento.DataSource = ComissionamentoOperadora.CarregarPorContratoId(cboComissionamentoContrato.SelectedValue);
            else
                gridComissionamento.DataSource = null;

            gridComissionamento.DataBind();
        }

        protected void cboComissionamentoContrato_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregarComissionamentos();
            this.ChecaDataDeContratoETabelaDeComissionamento();
            //this.MontaTabsComissionamento(2);
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "montaTabs", "$(function() { $('#subtab_contrato').tabs(2);});", true);
        }

        protected void gridComissionamento_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                ComissionamentoOperadora tabela = new ComissionamentoOperadora();
                gridComissionamento.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                tabela.ID = gridComissionamento.DataKeys[gridComissionamento.SelectedIndex].Value;
                tabela.Carregar();
                this.CarregaVitaliciedade();

                //txtComissionamentoDescricao.Text = tabela.Descricao;
                if (tabela.Data != DateTime.MinValue) { txtComissionamentoData.Text = tabela.Data.ToString("dd/MM/yyyy"); }

                //chkComissionamentoVitalicio.Checked = tabela.Vitalicia;
                //if (tabela.Vitalicia)
                //{
                //    txtComissionamentoNumeroParcelaVitalicio.Text = Convert.ToString(tabela.VitaliciaNumeroParcela);
                //    txtComissionamentoVitalicioPercentual.Text = tabela.VitaliciaPercentual.ToString("N2");
                //}

                cboComissionamentoContratoDETALHE.SelectedValue = Convert.ToString(tabela.ContratoAdmID);

                //carrega os itens
                this.CarregarItensDaTabela_Comissionamento();

                pnlComissaoLista.Visible = false;
                pnlComissaoDetalhe.Visible = true;
                this.ChecaDataDeContratoETabelaDeComissionamento();
            }
            else if (e.CommandName.Equals("excluir"))
            {
                ComissionamentoOperadora com = new ComissionamentoOperadora();
                com.ID = gridComissionamento.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                com.Remover();
                this.CarregarComissionamentos();
            }
            else if (e.CommandName.Equals("idade"))
            {
                Object id = gridComissionamento.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                ComissionamentoOperadora com = new ComissionamentoOperadora(id);
                com.Carregar();
                litCondicoesEtarias.Text = " - " + com.Descricao;
                gridComissionamento.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                pnlComissaoLista.Visible = false;
                pnlComissaoIdade.Visible = true;
                this.CarregaCondicoesEtarias();
            }
            //else if (e.CommandName.Equals("ativar"))
            //{
            //    Object id = gridComissionamento.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
            //    ContratoADM.SetaTabelaComissionamentoAutal(cboComissionamentoContrato.SelectedValue, id, null);
            //    this.CarregarComissionamentos();
            //}
        }

        protected void gridComissionamento_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UIHelper.AuthWebCtrl(e.Row.Cells[4], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente prosseguir com a exclusão?\\nEssa operação não poderá ser desfeita.");
            }
        }

        Boolean ChecaDataDeContratoETabelaDeComissionamento()
        {
            if (!base.HaItemSelecionado(cboComissionamentoContratoDETALHE))
            {
                //base.Alerta(null, this, "_erroComNovoCheca", "Informe o contrato da tabela de comissionamento.");
                base.Alerta(MPE, ref litAlert, "Informe o contrato da tabela de comissionamento.", upnlAlerta);
                cboComissionamentoContratoDETALHE.Focus();
                cmdSalvarComissionamento.Enabled = false;
                return false;
            }

            IList<ComissionamentoOperadora> lista = ComissionamentoOperadora.CarregarPorContratoId(cboComissionamentoContratoDETALHE.SelectedValue);
            if (lista == null)
            {
                ContratoADM contrato = new ContratoADM(cboComissionamentoContratoDETALHE.SelectedValue);
                contrato.Carregar();
                txtComissionamentoData.Text = contrato.Data.ToString("dd/MM/yyyy");
                txtComissionamentoData.ReadOnly = true;
            }
            else
                txtComissionamentoData.ReadOnly = false;

            cmdSalvarComissionamento.Enabled = true;
            return true;
        }

        protected void cmdNovoComissionamento_Click(Object sender, EventArgs e)
        {
            if (cboComissionamentoContrato.Items.Count == 0)
            {
                //base.Alerta(null, this, "_erroComNovo1", "Informe o contrato da tabela de comissionamento.");
                base.Alerta(MPE, ref litAlert, "Informe o contrato da tabela de comissionamento.", upnlAlerta);
                cboComissionamentoContrato.Focus();
                return;
            }

            pnlComissaoLista.Visible = false;
            pnlComissaoDetalhe.Visible = true;
            this.ItensComissionamento = null;
            this.ChecaDataDeContratoETabelaDeComissionamento();
        }

        /////DETALHES

        void CarregaVitaliciedade()
        {
            if (gridComissionamento.SelectedIndex == -1)
            {
                txtComissionamentoNumeroParcelaVitalicio.Text = "";
                txtComissionamentoVitalicioPercentual.Text = "";
                chkComissionamentoVitalicio.Checked = false;
                return;
            }

            //Normal
            ComissionamentoOperadoraVitaliciedade cov1 = ComissionamentoOperadoraVitaliciedade.Carregar(gridComissionamento.DataKeys[gridComissionamento.SelectedIndex].Value, TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal);
            if (cov1 != null)
            {
                chkComissionamentoVitalicio.Checked = cov1.Vitalicia;
                if (cov1.Vitalicia)
                {
                    txtComissionamentoNumeroParcelaVitalicio.Text = Convert.ToString(cov1.ParcelaInicio);
                    txtComissionamentoVitalicioPercentual.Text = cov1.Percentual.ToString("N2");
                }
                else
                {
                    txtComissionamentoNumeroParcelaVitalicio.Text = "";
                    txtComissionamentoVitalicioPercentual.Text = "";
                }
            }
            else
            {
                txtComissionamentoNumeroParcelaVitalicio.Text = "";
                txtComissionamentoVitalicioPercentual.Text = "";
                chkComissionamentoVitalicio.Checked = false;
            }

            //Carencia
            cov1 = ComissionamentoOperadoraVitaliciedade.Carregar(gridComissionamento.DataKeys[gridComissionamento.SelectedIndex].Value, TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia);
            if (cov1 != null)
            {
                chkComissionamentoVitalicioCarencia.Checked = cov1.Vitalicia;
                if (cov1.Vitalicia)
                {
                    txtComissionamentoNumeroParcelaVitalicioCarencia.Text = Convert.ToString(cov1.ParcelaInicio);
                    txtComissionamentoVitalicioPercentualCarencia.Text = cov1.Percentual.ToString("N2");
                }
                else
                {
                    txtComissionamentoNumeroParcelaVitalicioCarencia.Text = "";
                    txtComissionamentoVitalicioPercentualCarencia.Text = "";
                }
            }
            else
            {
                txtComissionamentoNumeroParcelaVitalicioCarencia.Text = "";
                txtComissionamentoVitalicioPercentualCarencia.Text = "";
                chkComissionamentoVitalicioCarencia.Checked = false;
            }

            //Migracao
            cov1 = ComissionamentoOperadoraVitaliciedade.Carregar(gridComissionamento.DataKeys[gridComissionamento.SelectedIndex].Value, TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao);
            if (cov1 != null)
            {
                chkComissionamentoVitalicioMigracao.Checked = cov1.Vitalicia;
                if (cov1.Vitalicia)
                {
                    txtComissionamentoNumeroParcelaVitalicioMigracao.Text = Convert.ToString(cov1.ParcelaInicio);
                    txtComissionamentoVitalicioPercentualMigracao.Text = cov1.Percentual.ToString("N2");
                }
                else
                {
                    txtComissionamentoNumeroParcelaVitalicioMigracao.Text = "";
                    txtComissionamentoVitalicioPercentualMigracao.Text = "";
                }
            }
            else
            {
                txtComissionamentoNumeroParcelaVitalicioMigracao.Text = "";
                txtComissionamentoVitalicioPercentualMigracao.Text = "";
                chkComissionamentoVitalicioMigracao.Checked = false;
            }

            //Especial
            cov1 = ComissionamentoOperadoraVitaliciedade.Carregar(gridComissionamento.DataKeys[gridComissionamento.SelectedIndex].Value, TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial);
            if (cov1 != null)
            {
                chkComissionamentoVitalicioEspecial.Checked = cov1.Vitalicia;
                if (cov1.Vitalicia)
                {
                    txtComissionamentoNumeroParcelaVitalicioEspecial.Text = Convert.ToString(cov1.ParcelaInicio);
                    txtComissionamentoVitalicioPercentualEspecial.Text = cov1.Percentual.ToString("N2");
                }
                else
                {
                    txtComissionamentoNumeroParcelaVitalicioEspecial.Text = "";
                    txtComissionamentoVitalicioPercentualEspecial.Text = "";
                }
            }
            else
            {
                txtComissionamentoNumeroParcelaVitalicioEspecial.Text = "";
                txtComissionamentoVitalicioPercentualEspecial.Text = "";
                chkComissionamentoVitalicioEspecial.Checked = false;
            }
        }

        void SalvarVitaliciedade(Object tabelaId)
        {
            //Normal
            ComissionamentoOperadoraVitaliciedade cov = ComissionamentoOperadoraVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal);
            if (cov == null) { cov = new ComissionamentoOperadoraVitaliciedade(); }
            cov.TabelaID = tabelaId;
            cov.Vitalicia = chkComissionamentoVitalicio.Checked;
            cov.TipoColunaComissao = (int)TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal;
            if (chkComissionamentoVitalicio.Checked)
            {
                cov.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicio.Text);
                cov.Percentual = CToDecimal(txtComissionamentoVitalicioPercentual.Text);
            }
            else
            {
                cov.ParcelaInicio = 0;
                cov.Percentual = 0;
            }
            cov.Salvar();

            //Carencia
            cov = ComissionamentoOperadoraVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia);
            if (cov == null) { cov = new ComissionamentoOperadoraVitaliciedade(); }
            cov.TabelaID = tabelaId;
            cov.Vitalicia = chkComissionamentoVitalicioCarencia.Checked;
            cov.TipoColunaComissao = (int)TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia;
            if (chkComissionamentoVitalicioCarencia.Checked)
            {
                cov.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioCarencia.Text);
                cov.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualCarencia.Text);
            }
            else
            {
                cov.ParcelaInicio = 0;
                cov.Percentual = 0;
            }
            cov.Salvar();

            //Migracao
            cov = ComissionamentoOperadoraVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao);
            if (cov == null) { cov = new ComissionamentoOperadoraVitaliciedade(); }
            cov.TabelaID = tabelaId;
            cov.Vitalicia = chkComissionamentoVitalicioMigracao.Checked;
            cov.TipoColunaComissao = (int)TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao;
            if (chkComissionamentoVitalicioMigracao.Checked)
            {
                cov.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioMigracao.Text);
                cov.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualMigracao.Text);
            }
            else
            {
                cov.ParcelaInicio = 0;
                cov.Percentual = 0;
            }
            cov.Salvar();

            //Especial
            cov = ComissionamentoOperadoraVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial);
            if (cov == null) { cov = new ComissionamentoOperadoraVitaliciedade(); }
            cov.TabelaID = tabelaId;
            cov.Vitalicia = chkComissionamentoVitalicioEspecial.Checked;
            cov.TipoColunaComissao = (int)TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial;
            if (chkComissionamentoVitalicioEspecial.Checked)
            {
                cov.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioEspecial.Text);
                cov.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualEspecial.Text);
            }
            else
            {
                cov.ParcelaInicio = 0;
                cov.Percentual = 0;
            }
            cov.Salvar();
        }

        protected void cboComissionamentoContratoDETALHE_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.ChecaDataDeContratoETabelaDeComissionamento();
            if (!txtComissionamentoData.ReadOnly)
            {
                txtComissionamentoData.Text = "";
                txtComissionamentoData.Focus();
            }
        }

        protected void cmdComissaoFechar_Click(Object sender, EventArgs e)
        {
            gridComissionamento.SelectedIndex = -1;
            pnlComissaoLista.Visible = true;
            pnlComissaoDetalhe.Visible = false;
            this.ItensComissionamento = null;
            gridComissionamentoItensDetalhe.DataSource = null;
            gridComissionamentoItensDetalhe.DataBind();
            cmdSalvarComissionamento.Enabled = true;
        }

        protected void cmdAddItemCom_Click(Object sender, EventArgs e)
        {
            List<ComissionamentoOperadoraItem> lista = this.ItensComissionamento;
            if (lista == null) { lista = new List<ComissionamentoOperadoraItem>(); }

            lista.Add(new ComissionamentoOperadoraItem());

            gridComissionamentoItensDetalhe.DataSource = lista;
            gridComissionamentoItensDetalhe.DataBind();
            this.ItensComissionamento = lista;
        }

        void CarregarItensDaTabela_Comissionamento()
        {
            List<ComissionamentoOperadoraItem> lista = (List<ComissionamentoOperadoraItem>)
                ComissionamentoOperadoraItem.Carregar(gridComissionamento.DataKeys[gridComissionamento.SelectedIndex].Value);

            //DynamicComparer<ComissionamentoItem> comparer = new DynamicComparer<ComissionamentoItem>("Parcela");
            //((List<ComissionamentoItem>)lista).Sort(comparer.Compare);

            gridComissionamentoItensDetalhe.DataSource = lista;
            gridComissionamentoItensDetalhe.DataBind();
            this.ItensComissionamento = lista;
        }

        protected void gridComissionamentoItensDetalhe_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "excluir")
            {
                Object id = gridComissionamentoItensDetalhe.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                if (id == null)
                {
                    List<ComissionamentoOperadoraItem> lista = this.ItensComissionamento;
                    lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                    this.ItensComissionamento = lista;
                    gridComissionamentoItensDetalhe.DataSource = lista;
                    gridComissionamentoItensDetalhe.DataBind();
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
                UIHelper.AuthWebCtrl(e.Row.Cells[7], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                base.grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja realmente excluir a linha?");

                TextBox txtvalor1 = (TextBox)e.Row.Cells[1].Controls[1];
                txtvalor1.Attributes.Add("onKeyUp", "mascara('" + txtvalor1.ClientID + "')");

                TextBox txtvalor2 = (TextBox)e.Row.Cells[2].Controls[1];
                txtvalor2.Attributes.Add("onKeyUp", "mascara('" + txtvalor2.ClientID + "')");

                TextBox txtvalor3 = (TextBox)e.Row.Cells[3].Controls[1];
                txtvalor3.Attributes.Add("onKeyUp", "mascara('" + txtvalor3.ClientID + "')");

                TextBox txtvalor5 = (TextBox)e.Row.Cells[5].Controls[1];
                txtvalor5.Attributes.Add("onKeyUp", "mascara('" + txtvalor5.ClientID + "')");

                //Object id = gridComissionamentoItensDetalhe.DataKeys[e.Row.RowIndex].Value;

                //TextBox txtparcela = (TextBox)e.Row.Cells[0].Controls[1];
                //if (txtparcela.Text == "0") { txtparcela.Text = ""; }

                if (CToDecimal(txtvalor1.Text) == 0) { txtvalor1.Text = ""; }
                if (CToDecimal(txtvalor2.Text) == 0) { txtvalor2.Text = ""; }
                if (CToDecimal(txtvalor3.Text) == 0) { txtvalor3.Text = ""; }
                if (CToDecimal(txtvalor5.Text) == 0) { txtvalor5.Text = ""; }
            }
        }

        protected void cmdSalvarComissionamento_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            //if (txtComissionamentoDescricao.Text.Trim() == "")
            //{
            //    base.Alerta(null, this, "_erroCom0", "Informe uma descrição.");
            //    //txtComissionamentoDescricao.Focus();
            //    return;
            //}

            DateTime dte;
            //System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("pt-Br");
            //if (!DateTime.TryParse(txtComissionamentoData.Text, info, System.Globalization.DateTimeStyles.None, out dte))
            if (!UIHelper.TyParseToDateTime(txtComissionamentoData.Text, out dte))
            {
                //base.Alerta(null, this, "_erroComDt", "Data de vigência inválida.");
                base.Alerta(MPE, ref litAlert, "Data de vigência inválida.", upnlAlerta);
                txtComissionamentoData.Focus();
                return;
            }

            if (chkComissionamentoVitalicio.Checked)
            {
                int result = 0;
                if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicio.Text, out result))
                {
                    //base.Alerta(null, this, "_erroCom1", "Informe o número da parcela para tabela vitalícia.");
                    base.Alerta(MPE, ref litAlert, "Informe o número da parcela para tabela vitalícia.", upnlAlerta);
                    txtComissionamentoNumeroParcelaVitalicio.Focus();
                    return;
                }

                Decimal result2 = 0;
                if (!Decimal.TryParse(txtComissionamentoVitalicioPercentual.Text, out result2))
                {
                    //base.Alerta(null, this, "_erroCom2", "Informe o percentual vitalício.");
                    base.Alerta(MPE, ref litAlert, "Informe o percentual vitalício.", upnlAlerta);
                    txtComissionamentoVitalicioPercentual.Focus();
                    return;
                }
            }

            //if (cboComissionamentoContratoDETALHE.Items.Count == 0)
            //{
            //    base.Alerta(null, this, "_erroCom4", "Informe o contrato da tabela de comissionamento.");
            //    cboComissionamentoContratoDETALHE.Focus();
            //    return;
            //}

            String msg = UIHelper.ChecaGridDeParcelas(gridComissionamentoItensDetalhe, 0);
            if (msg != String.Empty)
            {
                //base.Alerta(null, this, "_erroCom5", msg);
                base.Alerta(MPE, ref litAlert, msg, upnlAlerta);
                return;
            }

            List<ComissionamentoOperadoraItem> lista = this.ItensComissionamento;
            if (lista == null || lista.Count == 0)
            {
                //base.Alerta(null, this, "_erroCom6", "Nenhuma parcela de comissionamento foi informada.");
                base.Alerta(MPE, ref litAlert, "Nenhuma parcela de comissionamento foi informada.", upnlAlerta);
                return;
            }

            if (!base.HaItemSelecionado(cboComissionamentoContratoDETALHE))
            {
                //base.Alerta(null, this, "_erroCom7", "Nenhum contrato foi selecionado.");
                base.Alerta(MPE, ref litAlert, "Nenhum contrato foi selecionado.", upnlAlerta);
                return;
            }
            else
            {
                ContratoADM contrato = new ContratoADM(cboComissionamentoContratoDETALHE.SelectedValue);
                contrato.Carregar();
                DateTime dteContrato = new DateTime(contrato.Data.Year, contrato.Data.Month, contrato.Data.Day, 0, 0, 0);

                if (dte < dteContrato) //checa se a data de vigencia da tabela é menor que a vigencia do contrato
                {
                    //base.Alerta(null, this, "_erroCom8", "A data de vigência desta tabela de comissionamento não \\npode ser inferior a " + dteContrato.ToString("dd/MM/yyyy HH:mm:ss") + ".");
                    base.Alerta(MPE, ref litAlert, "A data de vigência desta tabela de comissionamento não pode ser inferior a " + dteContrato.ToString("dd/MM/yyyy HH:mm:ss") + ".", upnlAlerta);
                    return;
                }
            }

            Object id = null;
            if (gridComissionamento.SelectedIndex > -1)
                id = gridComissionamento.DataKeys[gridComissionamento.SelectedIndex].Value;

            if (ComissionamentoOperadora.ExisteTabela(cboComissionamentoContratoDETALHE.SelectedValue, id, dte))
            {
                //base.Alerta(null, this, "_erroCom9", "Já existe uma tabela com essa data de vigência.");
                base.Alerta(MPE, ref litAlert, "Já existe uma tabela com essa data de vigência.", upnlAlerta);
                return;
            }

            #endregion

            ComissionamentoOperadora tabela = new ComissionamentoOperadora();

            if(gridComissionamento.SelectedIndex > -1)
                tabela.ID = gridComissionamento.DataKeys[gridComissionamento.SelectedIndex].Value;

            if (txtComissionamentoData.Text.Trim() != "")
                tabela.Data = base.CStringToDateTime(txtComissionamentoData.Text);

            tabela.ContratoAdmID = cboComissionamentoContratoDETALHE.SelectedValue;

            tabela.Salvar();
            this.SalvarVitaliciedade(tabela.ID);

            gridComissionamento.SelectedIndex = -1;

            foreach (ComissionamentoOperadoraItem item in lista)
            {
                item.ComissionamentoID = tabela.ID;
                item.Salvar();
            }

            //this.ItensComissionamento = null;
            //pnlComissaoLista.Visible = true;
            //pnlComissaoDetalhe.Visible = false;
            cboComissionamentoContrato.SelectedValue = Convert.ToString(tabela.ContratoAdmID);
            this.CarregarComissionamentos();
            for (int i = 0; i < gridComissionamento.Rows.Count; i++)
            {
                if (Convert.ToString(gridComissionamento.DataKeys[i].Value) ==
                    Convert.ToString(tabela.ID))
                {
                    gridComissionamento.SelectedIndex = i;
                    break;
                }
            }

            //base.Alerta(null, this, "_okCom", "Dados salvos com sucesso.");
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
        }

        /////EXIBE CONDICOES ETARIAS

        void CarregaCondicoesEtarias()
        {
            gridComissionamentoIdade.DataSource = ComissionamentoIdade.Carregar(
                gridComissionamento.DataKeys[gridComissionamento.SelectedIndex].Value);
            gridComissionamentoIdade.DataBind();
        }

        protected void cmdComissaoFecharIdade_Click(Object sender, EventArgs e)
        {
            gridComissionamento.SelectedIndex = -1;
            gridComissionamentoIdade.SelectedIndex = -1;
            pnlComissaoLista.Visible = true;
            pnlComissaoIdade.Visible = false;
            pnlComissaoIdadeDETALHE.Visible = false;
            this.ItensComissionamentoIdade = null;
        }

        protected void gridComissionamentoIdade_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                gridComissionamentoIdade.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                Object id = gridComissionamentoIdade.DataKeys[gridComissionamentoIdade.SelectedIndex].Value;
                ComissionamentoIdade ci = new ComissionamentoIdade(id);
                ci.Carregar();

                pnlComissaoIdadeDETALHE.Visible = true;
                txtComissaoIdade.Text = ci.Idade.ToString();

                CarregaCondicoesEtariasITENS(ci.ID);
            }
            else if (e.CommandName.Equals("excluir"))
            {
                Object id = gridComissionamentoIdade.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                ComissionamentoIdade ci = new ComissionamentoIdade(id);
                ci.Remover();
                gridComissionamentoIdade.SelectedIndex = -1;
                this.CarregaCondicoesEtarias();
                this.ItensComissionamentoIdade = null;
                gridComissionamentoIdadeItem.DataSource = null;
                gridComissionamentoIdadeItem.DataBind();
            }
        }

        protected void gridComissionamentoIdade_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja realmente prosseguir?");
        }

        void CarregaCondicoesEtariasITENS(Object parentId)
        {
            IList<ComissionamentoIdadeItem> lista = ComissionamentoIdadeItem.Carregar(parentId);
            this.ItensComissionamentoIdade = (List<ComissionamentoIdadeItem>)lista;
            gridComissionamentoIdadeItem.DataSource = lista;
            gridComissionamentoIdadeItem.DataBind();
        }

        protected void cmdNovaTabelaIdade_Click(Object sender, EventArgs e)
        {
            this.ItensComissionamentoIdade = null;
            gridComissionamentoIdade.SelectedIndex =-1;

            pnlComissaoIdadeDETALHE.Visible = true;
            txtComissaoIdade.Text = "";

            this.MontaGridTabelaIdadeItens_PrimeiraInsercao();
        }

        void MontaGridTabelaIdadeItens_PrimeiraInsercao()
        {
            List<ComissionamentoIdadeItem> lista = new List<ComissionamentoIdadeItem>();
            for (int i = 1; i <= 5; i++)
            {
                lista.Add(new ComissionamentoIdadeItem());
            }

            gridComissionamentoIdadeItem.DataSource = lista;
            gridComissionamentoIdadeItem.DataBind();
            this.ItensComissionamentoIdade = lista;
        }

        /////////////////ITENS

        protected void gridComissionamentoIdadeItem_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                Object id = gridComissionamentoIdadeItem.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                List<ComissionamentoIdadeItem> lista = this.ItensComissionamentoIdade;
                lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                this.ItensComissionamentoIdade = lista;
                gridComissionamentoIdadeItem.DataSource = lista;
                gridComissionamentoIdadeItem.DataBind();

                if (id != null)
                {
                    ComissionamentoIdadeItem item = new ComissionamentoIdadeItem(id);
                    item.Remover();
                    item = null;
                }
            }
        }

        protected void gridComissionamentoIdadeItem_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente prosseguir?");

                Object id = gridComissionamentoIdadeItem.DataKeys[e.Row.RowIndex].Value;

                TextBox txtvalor1 = (TextBox)e.Row.Cells[1].Controls[1];
                txtvalor1.Attributes.Add("onKeyUp", "mascara('" + txtvalor1.ClientID + "')");
                if ((txtvalor1.Text == "0" || CToDecimal(txtvalor1.Text) == 0) && id==null) { txtvalor1.Text = ""; }

                //Object id = gridTReajusteItemDETALHE.DataKeys[e.Row.RowIndex].Value;

                //if (id == null)
                //{
                //    //se a linha nao está salva (nao tem id), seta para "" onde for 0
                //    TextBox txtidade1 = (TextBox)e.Row.Cells[0].Controls[1];
                //    if (txtidade1.Text == "0" || CToDecimal(txtvalor1.Text) == 0) { txtidade1.Text = ""; }
                //}
            }
        }

        protected void cmdAddItemIdade_Click(Object sender, EventArgs e)
        {
            List<ComissionamentoIdadeItem> lista = this.ItensComissionamentoIdade;
            if (lista == null) { lista = new List<ComissionamentoIdadeItem>(); }

            lista.Add(new ComissionamentoIdadeItem());

            gridComissionamentoIdadeItem.DataSource = lista;
            gridComissionamentoIdadeItem.DataBind();
            this.ItensComissionamentoIdade = lista;
        }

        protected void cmdComissaoFecharIdadeItem_Click(Object sender, EventArgs e)
        {
            this.ItensComissionamentoIdade = null;
            gridComissionamentoIdade.SelectedIndex = -1;
            pnlComissaoIdadeDETALHE.Visible = false;
        }

        protected void cmdSalvarTabelaIdade_Click(Object sender, EventArgs e)
        {
            #region validacoes

            if (txtComissaoIdade.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_errConId", "Informe a idade.");
                base.Alerta(MPE, ref litAlert, "Informe a idade.", upnlAlerta);
                return;
            }
            #endregion

            ComissionamentoIdade tabela = new ComissionamentoIdade();

            if (gridComissionamentoIdade.SelectedIndex > -1)
                tabela.ID = gridComissionamentoIdade.DataKeys[gridComissionamentoIdade.SelectedIndex].Value;

            tabela.ComissionamentoID = gridComissionamento.DataKeys[gridComissionamento.SelectedIndex].Value;
            tabela.Idade = Convert.ToInt32(txtComissaoIdade.Text);
            tabela.Salvar();

            if (this.ItensComissionamentoIdade != null)
            {
                List<ComissionamentoIdadeItem> lista = this.ItensComissionamentoIdade;
                foreach (ComissionamentoIdadeItem item in lista)
                {
                    item.ComissionamentoIdadeID = tabela.ID;
                    item.Salvar();
                }
            }

            cmdComissaoFecharIdadeItem_Click(null, null);
            this.CarregaCondicoesEtarias();
        }

        #endregion

        #region Calendario 

        void CarregaCalendariosAdmissaoVigencia()
        {
            if (!base.HaItemSelecionado(cboCalendarioContrato)) { return; }
            dlCalendario.DataSource = CalendarioAdmissaoVigencia.
                CarregarPorContrato(cboCalendarioContrato.SelectedValue);
            dlCalendario.DataBind();
        }

        protected void cboTipo_Changed(Object sender, EventArgs e)
        {
            if (cboTipo.SelectedValue == "1") //dia
            {
                cboDataLimiteDia.Visible = true;
                txtTextopersonalizado.Visible = false;
            }
            else
            {
                cboDataLimiteDia.Visible = false;
                txtTextopersonalizado.Visible = true;
            }
        }

        protected void cboPlanoCalendario_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaCalendariosAdmissaoVigencia();
        }

        protected void cmdCalendarioSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (!base.HaItemSelecionado(cboCalendarioContrato))
            {
                //base.Alerta(null, this, "_calenErr0", "Não há um contrato selecionado.");
                base.Alerta(MPE, ref litAlert, "Não há um contrato selecionado.", upnlAlerta);
                return;
            }

            //Admissao
            if (Int32.Parse(cboAdmissaoAte_Tipo.SelectedValue) < Int32.Parse(cboAdmissaoDe_Tipo.SelectedValue))
            {
                //base.Alerta(null, this, "_calenErr1", "Intervalo de admissão inválido.");
                base.Alerta(MPE, ref litAlert, "Intervalo de admissão inválido.", upnlAlerta);
                return;
            }
            else if (Int32.Parse(cboAdmissaoAte_Tipo.SelectedValue) == Int32.Parse(cboAdmissaoDe_Tipo.SelectedValue))
            {
                if (Int32.Parse(cboAdmissaoAte_DIA.SelectedValue) <= Int32.Parse(cboAdmissaoDe_DIA.SelectedValue))
                {
                    //base.Alerta(null, this, "_calenErr1", "Intervalo de admissão inválido.");
                    base.Alerta(MPE, ref litAlert, "Intervalo de admissão inválido.", upnlAlerta);
                    return;
                }
            }

            //Vigencia
            if (Int32.Parse(cboVigencia_Tipo.SelectedValue) < Int32.Parse(cboAdmissaoAte_Tipo.SelectedValue))
            {
                //base.Alerta(null, this, "_calenErr2", "Data de vigência inválida.");
                base.Alerta(MPE, ref litAlert, "Data de vigência inválida.", upnlAlerta);
                return;
            }
            else if (Int32.Parse(cboVigencia_Tipo.SelectedValue) == Int32.Parse(cboAdmissaoAte_Tipo.SelectedValue))
            {
                if (Int32.Parse(cboVigencia.SelectedValue) < Int32.Parse(cboAdmissaoAte_DIA.SelectedValue))
                {
                    //base.Alerta(null, this, "_calenErr2", "Data de vigência inválida.");
                    base.Alerta(MPE, ref litAlert, "Data de vigência inválida.", upnlAlerta);
                    return;
                }
            }

            //data
            DateTime data = base.CStringToDateTime(txtCalendarioData.Text);
            if (data == DateTime.MinValue)
            {
                base.Alerta(MPE, ref litAlert, "Data do calendário inválida.", upnlAlerta);
                return;
            }

            //////Vencimento denis
            ////if (Int32.Parse(cboVencimento_Tipo.SelectedValue) < Int32.Parse(cboVigencia_Tipo.SelectedValue))
            ////{
            ////    //base.Alerta(null, this, "_calenErr3", "Data de vencimento inválida.");
            ////    base.Alerta(MPE, ref litAlert, "Data de vencimento inválida.", upnlAlerta);
            ////    return;
            ////}
            ////else if (Int32.Parse(cboVencimento_Tipo.SelectedValue) == Int32.Parse(cboVigencia_Tipo.SelectedValue))
            ////{
            ////    if (Int32.Parse(cboVencimento.SelectedValue) < Int32.Parse(cboVigencia.SelectedValue))
            ////    {
            ////        //base.Alerta(null, this, "_calenErr3", "Data de vencimento inválida.");
            ////        base.Alerta(MPE, ref litAlert, "Data de vencimento inválida.", upnlAlerta);
            ////        return;
            ////    }
            ////}

            ////////////////////////

            ////if (cboTipo.SelectedValue == "2" && txtTextopersonalizado.Text.Trim() == "") denis
            ////{
            ////    base.Alerta(MPE, ref litAlert, "Informe o texto personalizado para a data limite.", upnlAlerta);
            ////    return;
            ////}

            #endregion

            CalendarioAdmissaoVigencia cav = new CalendarioAdmissaoVigencia();
            cav.AdmissaoAte_Dia = Int32.Parse(cboAdmissaoAte_DIA.SelectedValue);
            cav.AdmissaoAte_Tipo = Int32.Parse(cboAdmissaoAte_Tipo.SelectedValue);
            cav.AdmissaoDe_Dia = Int32.Parse(cboAdmissaoDe_DIA.SelectedValue);
            cav.AdmissaoDe_Tipo = Int32.Parse(cboAdmissaoDe_Tipo.SelectedValue);
            cav.ContratoID = cboCalendarioContrato.SelectedValue;
            ////cav.VencimentoDia = Int32.Parse(cboVencimento.SelectedValue); denis
            ////cav.VencimentoTipo = Int32.Parse(cboVencimento_Tipo.SelectedValue);
            cav.VigenciaDia  = Int32.Parse(cboVigencia.SelectedValue);
            cav.VigenciaTipo = Int32.Parse(cboVigencia_Tipo.SelectedValue);
            cav.Data         = new DateTime(data.Year, data.Month, data.Day, 0, 0, 0, 0);

            ////cav.DataSemJuros_Dia = Convert.ToInt32(cboDataSemJuros_DIAS.SelectedValue);
            ////if (cboTipo.SelectedValue == "1") //todo dia x
            ////{
            ////    cav.DataLimite_Tipo = (Int32)CalendarioAdmissaoVigencia.eDataLimiteTipo.TodoDia;
            ////    cav.DataLimite_Valor = Convert.ToInt32(cboDataLimiteDia.SelectedValue);
            ////}
            ////else
            ////{
            ////    cav.DataLimite_Tipo = (Int32)CalendarioAdmissaoVigencia.eDataLimiteTipo.TextoPersonalizado;
            ////    cav.DataLimite_Valor = txtTextopersonalizado.Text;
            ////}

            cav.Salvar();
            this.CarregaCalendariosAdmissaoVigencia();
        }

        protected void dlCalendario_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                if (!extraPermission())
                {
                    base.Alerta(null, this, "_err", "Você não tem permissão para executar essa ação.");
                    return;
                }

                Object id = dlCalendario.DataKeys[e.Item.ItemIndex];
                CalendarioAdmissaoVigencia cav = new CalendarioAdmissaoVigencia(id);
                try
                {
                    cav.Remover();
                    this.CarregaCalendariosAdmissaoVigencia();
                }
                catch
                {
                    //base.Alerta(null, this, "genErr0", "Não foi possível excluir.\\nCertifique-se de que o calendário não possua previsão de recebimento.");
                    base.Alerta(MPE, ref litAlert, "Não foi possível excluir.<br>Certifique-se de que o calendário não possua previsão de vencimento.", upnlAlerta);
                }
            }
            else if (e.CommandName.Equals("vencimento"))
            {
                if (dlCalendario.SelectedIndex > -1)
                {
                    ((ImageButton)dlCalendario.Items[dlCalendario.SelectedIndex].FindControl("cmdCalendarioExibirVencimento")).ImageUrl = "~/images/detail2.png";
                }

                dlCalendario.SelectedIndex = e.Item.ItemIndex;
                ((ImageButton)e.Item.FindControl("cmdCalendarioExibirVencimento")).ImageUrl = "~/images/arrowleft.png";
                pnlCalendarioVencimento.Visible = true; //pnlCalendarioFaturaLista.Visible = true;
                this.CarregaCalendarioVencimento(); //this.CarregaCalendariosRecebimento();
            }
        }

        /////////////////////////CALENDARIO DE RECEBIMENTO///////////////////////////////////////////
        void CarregaCalendariosRecebimento()
        {
            if (dlCalendarioVencimento.SelectedIndex == -1) { return; }
            Object calendarioVenctoId = dlCalendarioVencimento.DataKeys[dlCalendarioVencimento.SelectedIndex];

            dlCalendarioRecebimento.DataSource =
                CalendarioRecebimento.CarregarPorCalendarioDeVencimento(calendarioVenctoId);
            dlCalendarioRecebimento.DataBind();
        }

        protected void dlCalendarioRecebimento_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                if (!extraPermission())
                {
                    base.Alerta(null, this, "_err", "Você não tem permissão para executar essa ação.");
                    return;
                }

                Object id = dlCalendarioRecebimento.DataKeys[e.Item.ItemIndex];
                CalendarioRecebimento cr = new CalendarioRecebimento(id);
                cr.Remover();

                this.CarregaCalendariosRecebimento();
            }
        }

        protected void cmdCalendarioRecebimentoSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (dlCalendario.SelectedIndex == -1) { return; }

            //Comissao
            //if (Int32.Parse(cboComissao_Tipo.SelectedValue) < Int32.Parse(cboFatura_Tipo.SelectedValue))
            //{
            //    //base.Alerta(null, this, "_errCR1", "Data para pagamento da comissão inválida.");
            //    base.Alerta(MPE, ref litAlert, "Data para pagamento da comissão inválida.", upnlAlerta);
            //    return;
            //}
            //else if (Int32.Parse(cboComissao_Tipo.SelectedValue) == Int32.Parse(cboFatura_Tipo.SelectedValue))
            //{
            //    if (Int32.Parse(cboComissao_DIA.SelectedValue) < Int32.Parse(cboFatura_DIA.SelectedValue))
            //    {
            //        //base.Alerta(null, this, "_errCR1", "Data para pagamento da comissão inválida.");
            //        base.Alerta(MPE, ref litAlert, "Data para pagamento da comissão inválida.", upnlAlerta);
            //        return;
            //    }
            //}

            CalendarioVencimento cv = new CalendarioVencimento(dlCalendarioVencimento.DataKeys[dlCalendarioVencimento.SelectedIndex]);
            cv.Carregar();

            //Fatura
            //if (cv.VencimentoTipo > Int32.Parse(cboFatura_Tipo.SelectedValue))
            //{
            //    //base.Alerta(null, this, "_errCR2", "Data para pagamento da fatura inválida.");
            //    base.Alerta(MPE, ref litAlert, "Data para pagamento da fatura inválida.", upnlAlerta);
            //    return;
            //}
            //else if (cv.VencimentoTipo == Int32.Parse(cboFatura_Tipo.SelectedValue))
            //{
            //    if (cv.VencimentoDia >= Int32.Parse(cboFatura_DIA.SelectedValue))
            //    {
            //        //base.Alerta(null, this, "_errCR2", "Data para pagamento da fatura inválida.");
            //        base.Alerta(MPE, ref litAlert, "Data para pagamento da fatura inválida.", upnlAlerta);
            //        return;
            //    }
            //}

            //Nao pode ultrapassar 100% de Percentual. sobre Fatura
            Object calendarioVenctoId = cv.ID;
            IList<CalendarioRecebimento> lista = CalendarioRecebimento.CarregarPorCalendarioDeVencimento(calendarioVenctoId);
            if (lista != null)
            {
                int totalPercentual = 0;
                foreach (CalendarioRecebimento _cr in lista)
                {
                    totalPercentual += _cr.ComissaoPercentual;
                }

                totalPercentual += Int32.Parse(cboComissao_Percentual.SelectedValue);
                if (totalPercentual > 100)
                {
                    //base.Alerta(null, this, "_errCR3", "O total de percentual sobre fatura ultrapassou 100%.");
                    base.Alerta(MPE, ref litAlert, "O total de percentual sobre fatura ultrapassou 100%.", upnlAlerta);
                    return;
                }
            }

            #endregion

            CalendarioRecebimento cr = new CalendarioRecebimento();
            cr.CalendarioVencimentoID = dlCalendarioVencimento.DataKeys[dlCalendarioVencimento.SelectedIndex];
            cr.ComissaoDia = Int32.Parse(cboComissao_DIA.SelectedValue);
            cr.ComissaoTipo = Int32.Parse(cboComissao_Tipo.SelectedValue);
            cr.ComissaoPercentual = Int32.Parse(cboComissao_Percentual.SelectedValue);
            cr.FaturaDia = Int32.Parse(cboFatura_DIA.SelectedValue);
            cr.FaturaTipo = Int32.Parse(cboFatura_Tipo.SelectedValue);
            cr.Salvar();

            this.CarregaCalendariosRecebimento();
        }

        protected void cmdFecharCalendarioRecebimento_Click(Object sender, EventArgs e)
        {
            ((ImageButton)dlCalendarioVencimento.Items[dlCalendarioVencimento.SelectedIndex].FindControl("cmdCalendarioVencimentoExibirRecebimento")).ImageUrl = "~/images/detail2.png";
            dlCalendarioVencimento.SelectedIndex = -1;
            pnlCalendarioFaturaLista.Visible = false;
        }

        #endregion

        #region Calendario vencimento 

        void CarregaCalendarioVencimento()
        {
            if (dlCalendario.SelectedIndex == -1) { return; }
            Object calendarioAdmissaoId = dlCalendario.DataKeys[dlCalendario.SelectedIndex];

            dlCalendarioVencimento.DataSource =
                CalendarioVencimento.CarregarTodos(calendarioAdmissaoId);
            dlCalendarioVencimento.DataBind();
        }

        protected void cmdFecharCalendarioVencimento_Click(Object sender, EventArgs e)
        {
            ((ImageButton)dlCalendario.Items[dlCalendario.SelectedIndex].FindControl("cmdCalendarioExibirVencimento")).ImageUrl = "~/images/detail2.png";
            dlCalendario.SelectedIndex = -1;
            pnlCalendarioVencimento.Visible = false;

            dlCalendarioRecebimento.DataSource = null;
            dlCalendarioRecebimento.DataBind();
            dlCalendarioRecebimento.SelectedIndex = -1;
            pnlCalendarioFaturaLista.Visible = false;
        }

        protected void cmdCalendarioVencimentoSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes 
            ////Vencimento 
            //if (Int32.Parse(cboVencimento_Tipo.SelectedValue) < Int32.Parse(cboVigencia_Tipo.SelectedValue))
            //{
            //    //base.Alerta(null, this, "_calenErr3", "Data de vencimento inválida.");
            //    base.Alerta(MPE, ref litAlert, "Data de vencimento inválida.", upnlAlerta);
            //    return;
            //}
            //else if (Int32.Parse(cboVencimento_Tipo.SelectedValue) == Int32.Parse(cboVigencia_Tipo.SelectedValue))
            //{
            //    if (Int32.Parse(cboVencimento.SelectedValue) < Int32.Parse(cboVigencia.SelectedValue))
            //    {
            //        //base.Alerta(null, this, "_calenErr3", "Data de vencimento inválida.");
            //        base.Alerta(MPE, ref litAlert, "Data de vencimento inválida.", upnlAlerta);
            //        return;
            //    }
            //}

            ////////////////////

            if (cboTipo.SelectedValue == "2" && txtTextopersonalizado.Text.Trim() == "")
            {
                base.Alerta(MPE, ref litAlert, "Informe o texto personalizado para a data limite.", upnlAlerta);
                return;
            }

            DateTime data = base.CStringToDateTime(txtDataCalendarioVencimento.Text);
            if (data == DateTime.MinValue)
            {
                base.Alerta(MPE, ref litAlert, "Informe a data do calendário de vencimento.", upnlAlerta);
                return;
            }
            #endregion

            CalendarioVencimento cv = new CalendarioVencimento();
            cv.CalendarioAdmissaoID = dlCalendario.DataKeys[dlCalendario.SelectedIndex];
            cv.Data = new DateTime(data.Year, data.Month, data.Day, 0, 0, 0, 0);
            cv.VencimentoDia = Int32.Parse(cboVencimento.SelectedValue); 
            cv.VencimentoTipo = Int32.Parse(cboVencimento_Tipo.SelectedValue);
            cv.DataSemJuros_Dia = Convert.ToInt32(cboDataSemJuros_DIAS.SelectedValue);
            if (cboTipo.SelectedValue == "1") //todo dia x
            {
                cv.DataLimite_Tipo = (Int32)CalendarioVencimento.eDataLimiteTipo.TodoDia;
                cv.DataLimite_Valor = Convert.ToInt32(cboDataLimiteDia.SelectedValue);
            }
            else
            {
                cv.DataLimite_Tipo = (Int32)CalendarioVencimento.eDataLimiteTipo.TextoPersonalizado;
                cv.DataLimite_Valor = txtTextopersonalizado.Text;
            }

            cv.LimiteAposVencimento = Convert.ToInt32(cboDataLimite2.SelectedValue);

            cv.Salvar();
            this.CarregaCalendarioVencimento();
        }

        protected void dlCalendarioVencimento_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                if (!extraPermission())
                {
                    base.Alerta(null, this, "_err", "Você não tem permissão para executar essa ação.");
                    return;
                }

                Object id = dlCalendarioVencimento.DataKeys[e.Item.ItemIndex];
                CalendarioVencimento cv = new CalendarioVencimento(id);
                try
                {
                    cv.Remover();
                    this.CarregaCalendarioVencimento();
                }
                catch
                {
                    base.Alerta(MPE, ref litAlert, "Não foi possível excluir.<br>Certifique-se de que o calendário não possua previsão de recebimento.", upnlAlerta);
                }
            }
            else if (e.CommandName.Equals("recebimento"))
            {
                if (dlCalendarioVencimento.SelectedIndex > -1)
                {
                    ((ImageButton)dlCalendarioVencimento.Items[dlCalendarioVencimento.SelectedIndex].FindControl("cmdCalendarioVencimentoExibirRecebimento")).ImageUrl = "~/images/detail2.png";
                }

                dlCalendarioVencimento.SelectedIndex = e.Item.ItemIndex;
                ((ImageButton)e.Item.FindControl("cmdCalendarioVencimentoExibirRecebimento")).ImageUrl = "~/images/arrowleft.png";
                pnlCalendarioFaturaLista.Visible = true;
                this.CarregaCalendariosRecebimento();
            }
        }

        #endregion

        #region Calendario financeiro

        protected void grid_OnRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                List<CalendarioFinanceiro> lista = (List<CalendarioFinanceiro>)ViewState["_cf"];

                if (lista == null) { return; }

                lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                ViewState["_cf"] = lista;
                grid.DataSource = lista;
                grid.DataBind();
            }
        }

        protected void cmdAddCal_click (Object sender, EventArgs e)
        {
            CalendarioFinanceiro cf = new CalendarioFinanceiro();
            if (cboDiaPagamento.SelectedItem.Text != "---")
                cf.DiaPagamento = cboDiaPagamento.SelectedItem.Text;
            if (cboDiaRecebimento.SelectedItem.Text != "---")
                cf.DiaRecebimento = cboDiaRecebimento.SelectedItem.Text;

            List<CalendarioFinanceiro> lista = (List<CalendarioFinanceiro>)ViewState["_cf"];

            if (lista == null) { lista = new List<CalendarioFinanceiro>(); }

            lista.Add(cf);
            grid.DataSource = lista;
            grid.DataBind();

            ViewState["_cf"] = lista;
        }

        #endregion
    }

    [Serializable]
    public class CalendarioFinanceiro
    {
        Object _id;
        String _diaPagamento;
        String _diaRecebimento;

        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public String DiaPagamento
        {
            get { return _diaPagamento; }
            set { _diaPagamento = value; }
        }

        public String DiaRecebimento
        {
            get { return _diaRecebimento; }
            set { _diaRecebimento = value; }
        }
    }
}
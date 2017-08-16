namespace www.UBRASP
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Net.Mail;
    using System.Collections;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;
    using LC.Framework.Phantom;
    using System.Text;
    using System.Security.Cryptography;

    public partial class contrato : PageBase
    {
        #region properties

        Object ContratoImpressoID
        {
            get { return Cache["___aci_" + Session.SessionID]; }
            set { Cache.Remove("___aci_" + Session.SessionID); if (value != null) { Cache.Insert("___aci_" + Session.SessionID, value, null, DateTime.Now.AddHours(1), TimeSpan.Zero); } }
        }

        Object contratoId
        {
            get
            {
                if (!String.IsNullOrEmpty(Request[IDKey])) { return Request[IDKey]; }
                else { return ViewState[IDKey]; }
            }
            set { ViewState[IDKey] = value; }
        }

        /// <summary>
        /// Guarda o ID da entidade Beneficiario para o titular.
        /// </summary>
        Object TitularID
        {
            get { return ViewState["_titId"]; }
            set
            {
                ViewState["_titId"] = value;
                Session["idBenefTit"] = value;
                Cache.Remove(Session.SessionID);
                if (value != null)
                {
                    Cache.Insert(Session.SessionID, value, null, DateTime.Now.AddMinutes(40), TimeSpan.Zero);
                }
            }
        }

        Object EnderecoTitularID
        {
            get { return ViewState["_titEndId"]; }
            set { ViewState["_titEndId"] = value; }
        }

        /// <summary>
        /// Guarda o ID da entidade ContratoBeneficiario para o titular.
        /// </summary>
        Object TitularID_ContratoBeneficiario
        {
            get { return ViewState["_tit_contr_benef"]; }
            set { ViewState["_tit_contr_benef"] = value; }
        }

        Object DependenteID
        {
            get { return ViewState["_depId"]; }
            set { ViewState["_depId"] = value; }
        }

        IList<ContratoBeneficiario> Dependentes
        {
            get { return ViewState["_depen"] as IList<ContratoBeneficiario>; }
            set { ViewState["_depen"] = value; }
        }

        /// <summary>
        /// Guarada a taxa assiciativa de estipulate, quando esta for para o contrato inteiro, e não por vida.
        /// </summary>
        Decimal ValorTaxaAssociativaContrato
        {
            get { if (ViewState["__valorTaxaConrato"] == null) { return Decimal.Zero; } else { return Convert.ToDecimal(ViewState["__valorTaxaConrato"]); } }
            set { ViewState["__valorTaxaConrato"] = value; }
        }

        Hashtable Valores
        {
            get { return ViewState["__valores"] as Hashtable; }
            set { ViewState["__valores"] = value; }
        }

        Decimal ValorTotalProposta
        {
            get { if (ViewState["__valorTotal"] == null) { return Decimal.Zero; } else { return Convert.ToDecimal(ViewState["__valorTotal"]); } }
            set { ViewState["__valorTotal"] = value; }
        }

        #endregion

        void AdicionaValor(Object idContratoBeneficiario, Decimal valor)
        {
            if (Valores == null) { Valores = new Hashtable(); }
            Valores[Convert.ToString(idContratoBeneficiario)] = valor;
        }
        Decimal PegaValor(Object idContratoBeneficiario)
        {
            if (this.Valores == null) { return Decimal.Zero; }
            return Convert.ToDecimal(this.Valores[Convert.ToString(idContratoBeneficiario)]);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            txtDetalheSeguro_Premio.Attributes.Add("onKeyUp", "mascara('" + txtDetalheSeguro_Premio.ClientID + "')");
            txtDetalheSeguro_Cobertura.Attributes.Add("onKeyUp", "mascara('" + txtDetalheSeguro_Premio.ClientID + "')");

            txtValorCob.Attributes.Add("onKeyUp", "mascara('" + txtValorCob.ClientID + "')");

            cmdNovoTitular.Attributes.Add("onClick", "win = window.open('beneficiarioP.aspx?et=1', 'janela', 'toolbar=no,scrollbars=1,width=860,height=420'); win.moveTo(100,150); return false;");
            cmdNovoBeneficiario.Attributes.Add("onClick", "win = window.open('beneficiarioP.aspx?et=2&keyid=" + Session.SessionID + "', 'janela', 'toolbar=no,scrollbars=1,width=860,height=420'); win.moveTo(100,150); return false;");

            txtCarenciaTempoContrato.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");
            txtEmpresaAnteriorMeses.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");
            txtCarenciaDependenteTempoContrato.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");

            txtValorTotal.Attributes.Add("onKeyUp", "mascara('" + txtValorTotal.ClientID + "')");
            txtTitPeso.Attributes.Add("onKeyUp", "mascara('" + txtTitPeso.ClientID + "')");
            txtTitAltura.Attributes.Add("onKeyUp", "mascara('" + txtTitAltura.ClientID + "')");
            txtDepPeso.Attributes.Add("onKeyUp", "mascara('" + txtDepPeso.ClientID + "')");
            txtDepAltura.Attributes.Add("onKeyUp", "mascara('" + txtDepAltura.ClientID + "')");

            txtIdCobrancaEmDetalhe.Attributes.Add("style", "display:none");

            this.PreparaLinkParaEditarTitular();

            UIHelper.AuthCtrl(cmdRecalcularComposicao, null);

            if (!IsPostBack)
            {
                txtObs.Visible = false;
                txtDataInicio.Text = DateTime.Now.ToString("dd/MM/yyyy");

                this.carregaMotivoStatus();

                this.carregaTiposDeAtendimento();
                AtendimentoTemp.UI.FillCombo(cboSubTipoAtendimento);
                cboSubTipoAtendimento.Items.Insert(0, new ListItem("selecione", "0"));

                txtIdKey.Value = Session.SessionID;
                this.ContratoImpressoID = null;
                Usuario user = new Usuario();
                user.ID = Usuario.Autenticado.ID;
                user.Carregar();
                txtValorTotal.ReadOnly = !user.AlteraValorContratos;
                user = null;
                this.TitularID = null;

                txtDataContrato.Text = DateTime.Now.ToString("dd/MM/yyyy");

                this.CarregaOperadoras();
                if (base.IDKeyParameterInProcess(ViewState, "_contr"))
                    base.ExibirEstipulantes(cboEstipulante, true, false);
                else
                    base.ExibirEstipulantes(cboEstipulante, true, true);

                this.CarregaContratoADM();
                this.CarregaPlanos();
                this.CarregaEstadoCivil();
                base.ExibirOpcoesDeSexo(cboSexo, false);
                base.ExibirOpcoesDeSexo(cboSexoDependente, false);
                base.ExibirOpcoesDeTipoDeContrato(cboTipoProposta, true);
                this.CarregaFiliais();
                if (base.IDKeyParameterInProcess(ViewState, "_contr"))
                {
                    //this.checaEnriquecimento();
                    Session[ConferenciaObjKey] = null;
                    this.CarregaContrato();
                    this.carregaAdicionais();
                    this.MontaCombosDeBeneficiarios();
                    this.SetaEstadoDosAdicionais();
                    this.ExibeSumario();
                    lnkOkContrato.Visible = false;
                    pnlHistoricoPlano.Visible = true;
                    gridHistoricoPlano.DataSource = ContratoPlano.Carregar(contratoId);
                    gridHistoricoPlano.DataBind();

                    txtObs.Visible = true;
                    txtObs.ReadOnly = true;
                }
                else if (Session[ConferenciaObjKey] != null)
                {
                    Conferencia conferencia = (Conferencia)Session[ConferenciaObjKey];
                    this.CarregaContratoAPartirDaConferencia(conferencia);
                }
                else
                {
                    lnkOkContrato.Visible = true;
                    cmdSalvar.Enabled = false;
                    cmdAlterarPlano.Visible = false;
                }


                UIHelper.AuthCtrl(cmdSalvar, new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.PropostaBeneficiarioIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.ConsultaPropostaBeneficiarioIDKey, Perfil.Atendimento_Liberacao_Vencimento }); //Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                UIHelper.AuthCtrl(cmdAlterarBeneficiarioTitular, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                UIHelper.AuthCtrl(cmdNovoBeneficiario, new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.PropostaBeneficiarioIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.Atendimento_Liberacao_Vencimento });
                UIHelper.AuthCtrl(cmdAddDependente, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);

                //UIHelper.AuthCtrl(cmdAlterarPlano, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);

                UIHelper.AuthCtrl(p7Atendimento, new String[] { Perfil.JuridicoIDKey, Perfil.ConsultaPropostaBeneficiarioIDKey, Perfil.Atendimento_Liberacao_Vencimento, Perfil.OperadorIDKey, Perfil.ConsulPropBenefProdLiberBoletoIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.SupervidorIDKey, Perfil.Financeiro_RecupPendencias });
                UIHelper.AuthCtrl(pnlAtendimento, new String[] { Perfil.JuridicoIDKey, Perfil.ConsultaPropostaBeneficiarioIDKey, Perfil.Atendimento_Liberacao_Vencimento, Perfil.OperadorIDKey, Perfil.ConsulPropBenefProdLiberBoletoIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.SupervidorIDKey, Perfil.Financeiro_RecupPendencias });
                UIHelper.AuthCtrl(litAtendimentoHeader, new String[] { Perfil.JuridicoIDKey, Perfil.ConsultaPropostaBeneficiarioIDKey, Perfil.Atendimento_Liberacao_Vencimento, Perfil.OperadorIDKey, Perfil.ConsulPropBenefProdLiberBoletoIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.SupervidorIDKey, Perfil.Financeiro_RecupPendencias });

                UIHelper.AuthCtrl(trDesconto, new String[] { Perfil.ConsultaPropostaBeneficiarioIDKey, Perfil.Atendimento_Liberacao_Vencimento });

                if (pnlAtendimento.Visible) { this.ConfiguraAtendimento(); this.CarregaAtendimentoEmFoco(); }

                UIHelper.AuthCtrl(lnkAlterarStatus, new String[] { Perfil.Atendimento_Liberacao_Vencimento, Perfil.ConsulPropBenefProdLiberBoletoIDKey, Perfil.OperadorIDKey, Perfil.OperadorLiberBoletoIDKey });
            }

            if (Usuario.Autenticado.PerfilID == Perfil.ConsultaPropostaBeneficiarioIDKey)
            {
                cmdSalvar.Visible = false;

                gridEnderecosDisponiveis_Titular.Columns[2].Visible = false;
                gridEnderecosDisponiveis_Titular.Columns[3].Visible = false;

                chkCobrarTaxa.Enabled = false;

                cmdNovoAtendimento.Visible = false;
                cmdSalvarAtendimento.Visible = false;
                cmdGerarCobranca.Visible = false;

                //foreach (AjaxControlToolkit.TabPanel _tab in this.tab.conte.Tabs)
                //{
                //    foreach (Control ctrl in _tab.Controls)
                //    {
                //        if (ctrl is TextBox) ((TextBox)ctrl).ReadOnly = true;
                //        else if (ctrl is DropDownList) ((DropDownList)ctrl).Enabled = false;
                //    }
                //}

                txtAdmissao.ReadOnly = true;
                txtBairro.ReadOnly = true;
                txtCarenciaCodigo.ReadOnly = true;
                txtCarenciaDependenteCodigo.ReadOnly = true;
                txtCarenciaDependenteMatricula.ReadOnly = true;
                txtCarenciaDependenteTempoContrato.ReadOnly = true;
                txtCarenciaMatricula.ReadOnly = true;
                txtCarenciaTempoContrato.ReadOnly = true;
                txtCEP.ReadOnly = true;
                txtCidade.ReadOnly = true;
                txtComplemento.ReadOnly = true;
                txtCorretor.ReadOnly = true;
                txtCorretorTerceiroCPF.ReadOnly = true;
                txtCorretorTerceiroIdentificacao.ReadOnly = true;
                txtCPF.ReadOnly = true;
                txtCPFDependente.ReadOnly = true;
                txtCPFResponsavel.ReadOnly = true;
                txtDataContrato.ReadOnly = true;
                txtDataInativacao.ReadOnly = true;
                txtDataInicio.ReadOnly = true;
                txtDataNascimento.ReadOnly = true;
                txtDataNascimentoDependente.ReadOnly = true;
                txtDataNascimentoResponsavel.ReadOnly = true;
                txtDataPrevisao.ReadOnly = true;
                txtDDD1.ReadOnly = true;
                txtDDD2.ReadOnly = true;
                txtDDD3.ReadOnly = true;
                txtDepAdmissao.ReadOnly = true;
                txtDepAltura.ReadOnly = true;
                txtDepDataCasamento.ReadOnly = true;
                txtDependentePortabilidade.ReadOnly = true;
                txtDepPeso.ReadOnly = true;
                txtDepTempoContratoAte.ReadOnly = true;
                txtDepTempoContratoDe.ReadOnly = true;
                txtDesconto.ReadOnly = true;
                txtEmail.ReadOnly = true;
                //txtEmailAtendimento.ReadOnly = true;
                //txtEmailAtendimentoCC.ReadOnly = true;
                txtEmpresaAnterior.ReadOnly = true;
                txtEmpresaAnteriorMatricula.ReadOnly = true;
                txtEmpresaAnteriorMeses.ReadOnly = true;
                txtFone1.ReadOnly = true;
                txtFone2.ReadOnly = true;
                txtFone3.ReadOnly = true;
                txtLogradouro.ReadOnly = true;
                txtNome.ReadOnly = true;
                txtNomeDependente.ReadOnly = true;
                txtNomeMae.ReadOnly = true;
                txtNomeResponsavel.ReadOnly = true;
                txtNumero.ReadOnly = true;
                txtNumeroContrato.ReadOnly = true;
                txtNumeroContratoConfirme.ReadOnly = true;
                txtNumeroMatricula.ReadOnly = true;
                txtNumMatriculaDental.ReadOnly = true;
                txtNumMatriculaDentalDep.ReadOnly = true;
                txtNumMatriculaSaude.ReadOnly = true;
                txtNumMatriculaSaudeDep.ReadOnly = true;
                txtObs.ReadOnly = true;
                txtObsEdit.ReadOnly = true;
                txtOperador.ReadOnly = true;
                txtParcelaCob.ReadOnly = true;
                txtPlanoAdmissao.ReadOnly = true;
                txtPortabilidade.ReadOnly = true;
                txtRamal1.ReadOnly = true;
                txtRamal2.ReadOnly = true;
                txtRG.ReadOnly = true;
                txtRGDependente.ReadOnly = true;
                txtRGResponsavel.ReadOnly = true;
                txtSenha.ReadOnly = true;
                txtSuperiorTerceiroCPF.ReadOnly = true;
                txtSuperiorTerceiroIdentificacao.ReadOnly = true;
                txtTexto.ReadOnly = true;
                txtTexto2.ReadOnly = true;
                txtTitAltura.ReadOnly = true;
                txtTitDataCasamento.ReadOnly = true;
                txtTitPeso.ReadOnly = true;
                txtTitTempoContratoAte.ReadOnly = true;
                txtTitTempoContratoDe.ReadOnly = true;
                txtTitulo.ReadOnly = true;
                txtUF.ReadOnly = true;
                txtValorCob.ReadOnly = true;
                txtValorTotal.ReadOnly = true;
                txtVencimento.ReadOnly = true;
                txtVencimentoCob.ReadOnly = true;
                txtVigencia.ReadOnly = true;

                cboAcomodacao.Enabled = false;
                cboAcomodacaoAltera.Enabled = false;
                cboCarenciaDependenteOperadora.Enabled = false;
                cboCarenciaOperadora.Enabled = false;
                cboContrato.Enabled = false;
                cboEstadoCivil.Enabled = false;
                cboEstadoCivilDependente.Enabled = false;
                cboEstipulante.Enabled = false;
                cboFilial.Enabled = false;
                cboOperadora.Enabled = false;
                cboParentescoDependente.Enabled = false;
                cboParentescoResponsavel.Enabled = false;
                cboPlano.Enabled = false;
                cboPlanoAltera.Enabled = false;
                cboSexo.Enabled = false;
                cboSexoResponsavel.Enabled = false;
                cboStatusMotivo.Enabled = false;
                cboSubTipoAtendimento.Enabled = false;
                cboTipoAtendimento.Enabled = false;
                cboTipoEndereco.Enabled = false;
                cboTipoProposta.Enabled = false;

            }
        }

        void carregaAdicionais()
        {
            IList<Adicional> adicionais = Adicional.CarregarPorOperadoraID(cboOperadora.SelectedValue);
            cboAdicionaisParaAdicionar.Items.Clear();
            if (adicionais != null)
            {
                foreach (Adicional ad in adicionais)
                {
                    if (!ad.Ativo) continue;
                    cboAdicionaisParaAdicionar.Items.Add(new ListItem(ad.Descricao, Convert.ToString(ad.ID)));
                }
            }
        }

        void checaEnriquecimento()
        {
            LC.Framework.Phantom.PersistenceManager pm = new LC.Framework.Phantom.PersistenceManager();
            pm.UseSingleCommandInstance();
            IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID_Parcial(this.contratoId, true, false, pm);
            List<String> ids = new List<String>();
            foreach (ContratoBeneficiario benef in beneficiarios) { ids.Add(Convert.ToString(benef.BeneficiarioID)); }

            String qry = String.Concat(
                "select beneficiario_nome,id_telMail,id_beneficiario,tipo,ddd,telefone,ramal,email ",
                "   from mailing ",
                "       inner join beneficiario on beneficiario_id = id_beneficiario ",
                "   where ",
                "       (concluido = 0 or concluido is null) and id_beneficiario in (", String.Join(",", ids.ToArray()), ")");

            DataTable dt = LC.Framework.Phantom.LocatorHelper.Instance.ExecuteQuery(qry, "result", pm).Tables[0];

            pm.CloseSingleCommandInstance();
            pm.Dispose();

            if (dt.Rows.Count > 0)
                pnlEnriquecimento.Visible = true;
            else
                pnlEnriquecimento.Visible = false;

            gridEnriquecimento.DataSource = dt;
            gridEnriquecimento.DataBind();

            dt.Dispose();
        }

        void PreparaLinkParaEditarTitular()
        {
            if (this.TitularID != null)
            {
                cmdAlterarBeneficiarioTitular.Attributes.Add("onClick", "win = window.open('beneficiarioP.aspx?et=1&" + IDKey + "=" + this.TitularID + "', 'janela', 'toolbar=no,scrollbars=1,width=860,height=420'); win.moveTo(100,150); return false;");
            }
        }

        void carregaMotivoStatus()
        {
            cboStatusMotivo.DataValueField = "ID";
            cboStatusMotivo.DataTextField = "Descricao";

            ContratoStatus.eTipo tipo = ContratoStatus.eTipo.Indefinido;
            if (optNormalEdit.Checked) tipo = ContratoStatus.eTipo.Reativacao;
            else if (optInativoEdit.Checked) tipo = ContratoStatus.eTipo.Inativacao;
            else if (optCanceladoEdit.Checked) tipo = ContratoStatus.eTipo.Cancelamento;

            cboStatusMotivo.DataSource = ContratoStatus.Carregar(tipo);
            cboStatusMotivo.DataBind();
        }

        void PreencheCampoCorretor(Object corretorId)
        {
            Usuario corretor = Usuario.CarregarParcial(corretorId);
            txtCorretor.Text = String.Concat(corretor.Nome, " (", corretor.Documento1, ")");
            txtCorretorID.Value = Convert.ToString(corretor.ID);
        }

        void CarregaContrato()
        {
            ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(ViewState[IDKey], null);
            if (titular != null && titular.Status != (Int32)ContratoBeneficiario.eStatus.Novo)
            {
                cmdCarregaBeneficiarioPorRG.Visible = false;
                cmdNovoTitular.Visible = false;
            }
            titular = null;

            Contrato contrato = new Contrato();
            contrato.ID = ViewState[IDKey];
            contrato.Carregar();

            if (contrato.Inativo || contrato.Cancelado)
                cmdGerarCobranca.Enabled = false;

            if (contrato.FilialID != null)
            {
                cboFilial.SelectedValue = Convert.ToString(contrato.FilialID);
            }

            if (contrato.ID == null) { ViewState[IDKey] = null; return; }

            this.ContratoImpressoID = contrato.NumeroID;

            #region Aba1

            AlmoxContratoImpresso aci = null;//

            if (contrato.NumeroID != null)
            {
                aci = new AlmoxContratoImpresso(contrato.NumeroID);
                aci.Carregar();

                txtNumeroContrato.Text = aci.NumeroDoImpresso;
                txtNumeroContratoConfirme.Text = txtNumeroContrato.Text;
            }
            else
            {
                txtNumeroContrato.Text = contrato.Numero;
                txtNumeroContratoConfirme.Text = txtNumeroContrato.Text;
            }

            txtCorretorTerceiroCPF.Text = contrato.CorretorTerceiroCPF;
            txtCorretorTerceiroIdentificacao.Text = contrato.CorretorTerceiroNome;
            txtSuperiorTerceiroCPF.Text = contrato.SuperiorTerceiroCPF;
            txtSuperiorTerceiroIdentificacao.Text = contrato.SuperiorTerceiroNome;

            if (Usuario.Autenticado.PerfilID != Perfil.AdministradorIDKey)
            {
                txtNumeroContrato.ReadOnly = true;
                txtNumeroContratoConfirme.ReadOnly = true;
            }

            lnkOkContrato.Visible = false;
            txtNumeroMatricula.Text = contrato.NumeroMatricula;

            if (contrato.Admissao != DateTime.MinValue)
            {
                txtAdmissao.Text = contrato.Admissao.ToString("dd/MM/yyyy");
                txtVigencia.Text = contrato.Vigencia.ToString("dd/MM/yyyy");
                txtVencimento.Text = contrato.Vencimento.ToString("dd/MM/yyyy");
            }

            cboEstipulante.SelectedValue = Convert.ToString(contrato.EstipulanteID);
            cboOperadora.SelectedValue = Convert.ToString(contrato.OperadoraID);
            cboOperadora_SelectedIndexChanged(null, null);
            cboContrato.SelectedValue = Convert.ToString(contrato.ContratoADMID);
            cboContrato_SelectedIndexChanged(null, null);
            cboPlano.SelectedValue = Convert.ToString(contrato.PlanoID);
            this.CarregaAcomodacoes();

            cboAcomodacao.SelectedValue = Convert.ToString(contrato.TipoAcomodacao);

            if (cboTipoProposta.Items.FindByValue(Convert.ToString(contrato.TipoContratoID)) != null)
                cboTipoProposta.SelectedValue = Convert.ToString(contrato.TipoContratoID);
            else
                cboTipoProposta.SelectedIndex = 0;

            if (chkCobrarTaxa.Visible && contrato.CobrarTaxaAssociativa)
                chkCobrarTaxa.Checked = true;

            if (!String.IsNullOrEmpty(contrato.EmpresaAnterior))
            {
                pnlInfoAnterior.Visible = true;
                txtEmpresaAnterior.Text = contrato.EmpresaAnterior;
                txtEmpresaAnteriorMeses.Text = Convert.ToString(contrato.EmpresaAnteriorTempo);
                txtEmpresaAnteriorMatricula.Text = contrato.EmpresaAnteriorMatricula;
            }

            this.PreencheCampoCorretor(contrato.DonoID);

            if (contrato.OperadorTmktID != null)
            {
                Usuario operador = Usuario.CarregarParcial(contrato.OperadorTmktID);

                if (operador != null)
                {
                    txtOperadorID.Value = Convert.ToString(operador.ID);
                    txtOperador.Text = String.Concat(operador.Nome, " (", operador.Documento1, ")");
                }
            }

            #endregion

            #region Abas 2, 3 e 4

            txtNomeResponsavel.Text = contrato.ResponsavelNome;
            txtCPFResponsavel.Text = contrato.ResponsavelCPF;
            txtRGResponsavel.Text = contrato.ResponsavelRG;
            if (contrato.ResponsavelDataNascimento != DateTime.MinValue)
                txtDataNascimentoResponsavel.Text = contrato.ResponsavelDataNascimento.ToString("dd/MM/yyyy");
            cboSexoResponsavel.SelectedValue = contrato.ResponsavelSexo;
            if (contrato.ResponsavelParentescoID != null)
                cboParentescoResponsavel.SelectedValue = Convert.ToString(contrato.ResponsavelParentescoID);


            IList<ContratoBeneficiario> beneficiariosContrato =
                ContratoBeneficiario.CarregarPorContratoID(contrato.ID, false);

            if (beneficiariosContrato != null)
            {
                IList<ContratoBeneficiario> dependentes = new List<ContratoBeneficiario>();

                foreach (ContratoBeneficiario _beneficiarioContrato in beneficiariosContrato)
                {
                    if (_beneficiarioContrato.Tipo == Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular))
                    {
                        this.TitularID = _beneficiarioContrato.BeneficiarioID;
                        this.TitularID_ContratoBeneficiario = _beneficiarioContrato.ID;
                        if (_beneficiarioContrato.EstadoCivilID != null)
                            cboEstadoCivil.SelectedValue = Convert.ToString(_beneficiarioContrato.EstadoCivilID);

                        if (_beneficiarioContrato.DataCasamento != DateTime.MinValue)
                            txtTitDataCasamento.Text = _beneficiarioContrato.DataCasamento.ToString("dd/MM/yyyy");

                        txtNumMatriculaSaude.Text = _beneficiarioContrato.NumeroMatriculaSaude;
                        txtNumMatriculaDental.Text = _beneficiarioContrato.NumeroMatriculaDental;

                        Beneficiario beneficiarioTitular = new Beneficiario();
                        beneficiarioTitular.ID = _beneficiarioContrato.BeneficiarioID;
                        beneficiarioTitular.Carregar();

                        this.ExibeBeneficiarioCarregado(beneficiarioTitular, null);

                        cboCarenciaOperadora.Text = CToString(_beneficiarioContrato.CarenciaOperadora);
                        txtCarenciaOperadoraID.Value = CToString(_beneficiarioContrato.CarenciaOperadoraID);
                        txtCarenciaCodigo.Text = _beneficiarioContrato.CarenciaCodigo;
                        txtCarenciaMatricula.Text = _beneficiarioContrato.CarenciaMatriculaNumero;
                        txtCarenciaTempoContrato.Text = _beneficiarioContrato.CarenciaContratoTempo.ToString();
                        if (_beneficiarioContrato.CarenciaContratoDe != DateTime.MinValue)
                            txtTitTempoContratoDe.Text = _beneficiarioContrato.CarenciaContratoDe.ToString("dd/MM/yyyy");
                        if (_beneficiarioContrato.CarenciaContratoAte != DateTime.MinValue)
                            txtTitTempoContratoAte.Text = _beneficiarioContrato.CarenciaContratoAte.ToString("dd/MM/yyyy");

                        if (_beneficiarioContrato.CarenciaContratoDe != DateTime.MinValue && _beneficiarioContrato.CarenciaContratoAte != DateTime.MinValue)
                        {
                            txtCarenciaTempoContrato.Text = base.GetMonthsBetween(_beneficiarioContrato.CarenciaContratoDe, _beneficiarioContrato.CarenciaContratoAte).ToString();
                        }

                        txtPortabilidade.Text = _beneficiarioContrato.Portabilidade;
                    }
                    else // se NAO é titular, adiciona na colecao de dependentes
                    {
                        dependentes.Add(_beneficiarioContrato);
                    }
                }

                if (dependentes.Count > 0) { this.Dependentes = dependentes; }
                gridDependentes.DataSource = dependentes;
                gridDependentes.DataBind();
            }

            #endregion

            #region Aba5

            txtDataContrato.Text = contrato.Data.ToString("dd/MM/yyyy");
            txtDesconto.Text = contrato.Desconto.ToString("N2");

            Usuario usuarioEmissor = new Usuario();
            usuarioEmissor.ID = contrato.DonoID;
            usuarioEmissor.Carregar();

            if (contrato.Inativo)
            {
                optInativo.Checked = true;
                optCancelado.Checked = false;
                optNormal.Checked = false;
            }
            else if (contrato.Cancelado)
            {
                optCancelado.Checked = true;
                optInativo.Checked = false;
                optNormal.Checked = false;
            }
            else
            {
                optNormal.Checked = true;
                optInativo.Checked = false;
                optCancelado.Checked = false;
            }

            #endregion

            ViewState[PropostaEndCobranca] = contrato.EnderecoCobrancaID;
            ViewState[PropostaEndReferecia] = contrato.EnderecoReferenciaID;

            try
            {
                this.ExibeEnderecosDaProposta();
            }
            catch
            {
            }

            if (contrato.Legado) this.calculaValorCobranca_Boleto();

            txtObs.Text = contrato.Obs;
            lblCodigoCliente.Text = contrato.CodCobranca.ToString();

            String msg = "";
            ContratoFacade.Instance.AtualizaValorDeCobrancas(contrato.ID, out msg);

            if (!String.IsNullOrEmpty(msg))
            {
                base.Alerta(null, this, "_msg", "ATENÇÃO!\\n" + msg);
            }

            this.carregaHistoricoStatus();

            //DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ir_dados_preprod_sp where UTILIZAR_REGISTRO = 1 and ENVIAR_DMED = 1 and idproposta=" + contratoId, "result", null).Tables[0];
            //if (dados.Rows.Count == 0)
            //{
            //    imgDemonstPagtoMail.OnClientClick      = "return false;";
            //    imgDemonstPagtoQualiMail.OnClientClick = "return false;";
            //    dados.Dispose();
            //    return;
            //}
            //DataRow[] ret = dados.Select("UTILIZAR_REGISTRO = 0 OR ENVIAR_DMED = 0");
            //if (ret.Length > 0)
            //{
            //    imgDemonstPagtoMail.OnClientClick      = "return false;";
            //    imgDemonstPagtoQualiMail.OnClientClick = "return false;";
            //}
            //dados.Dispose();
        }

        void carregaHistoricoStatus()
        {
            gridHistoricoStatus.DataSource = null;
            if (this.contratoId == null) { gridHistoricoStatus.DataBind(); return; }

            gridHistoricoStatus.DataSource = ContratoStatusInstancia.Carregar_SemObs(this.contratoId);
            gridHistoricoStatus.DataBind();

            upFinalizacao.Update();
        }

        /// <summary>
        /// Caso a tela seja carregada fruto de uma pesquisa por protocolo de atendimento
        /// </summary>
        void CarregaAtendimentoEmFoco()
        {
            if (!String.IsNullOrEmpty(Request["prot"]))
            {
                tab.ActiveTabIndex = 7;
                AtendimentoTemp atendimento = new AtendimentoTemp(Request["prot"]);
                atendimento.Carregar();
                this.exibeAtendimento(atendimento);

                for (int i = 0; i < gridAtendimento.Rows.Count; i++)
                {
                    if (Convert.ToString(gridAtendimento.DataKeys[Convert.ToInt32(i)][0]) ==
                        Request["prot"])
                    {
                        gridAtendimento.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Monta a proposta a partir de um obj Conferencia (pré-cadastro).
        /// </summary>
        void CarregaContratoAPartirDaConferencia(Conferencia conferencia)
        {
            cmdAlterarPlano.Visible = false;
            txtNumeroContrato.Text = conferencia.PropostaNumero;
            txtNumeroContratoConfirme.Text = conferencia.PropostaNumero;
            lnkOkContrato_Click(null, null);
            if (pnlSelNumeroContral.Visible)
            {
                pnlSelNumeroContral.Visible = false;
                gridSelNumeroContral.DataSource = null;
                gridSelNumeroContral.DataBind();
            }

            txtCorretorID.Value = Convert.ToString(conferencia.CorretorID);
            Usuario corretor = new Usuario(conferencia.CorretorID);
            corretor.Carregar();
            txtCorretor.Text = corretor.Nome;

            if (conferencia.FilialID != null)
            {
                cboFilial.SelectedValue = Convert.ToString(conferencia.FilialID);
                cboFilial.Enabled = false;
            }

            cboTipoProposta.SelectedValue = Convert.ToString(conferencia.TipoContratoID);
            cboTipoProposta_OnSelectedIndexChanged(null, null);
            if (conferencia.TipoContratoExplicito)
                cboTipoProposta.Enabled = false;

            cboEstipulante.SelectedValue = Convert.ToString(conferencia.EstipulanteID);
            cboEstipulante_OnSelectedIndexChanged(null, null);

            cboOperadora.SelectedValue = Convert.ToString(conferencia.OperadoraID);
            cboOperadora_SelectedIndexChanged(null, null);

            cboContrato.SelectedValue = Convert.ToString(conferencia.ContratoAdmID);
            cboContrato_SelectedIndexChanged(null, null);

            cboPlano.SelectedValue = Convert.ToString(conferencia.PlanoID);
            cboPlano_OnSelectedIndexChanged(null, null);

            cboAcomodacao.SelectedValue = Convert.ToString(conferencia.AcomodacaoID);
            cboAcomodacao_OnSelectedIndexChanged(null, null);

            cboEstipulante.Enabled = false;
            cboContrato.Enabled = false;
            cboPlano.Enabled = false;
            cboAcomodacao.Enabled = false;
            txtAdmissao.ReadOnly = true;

            txtAdmissao.Text = conferencia.Admissao.ToString("dd/MM/yyyy");
            txtAdmissao_OnTextChanged(null, null);

            #region TITULAR

            if (conferencia.Titular_BeneficiarioID != null)
            {
                txtCPF.Text = conferencia.TitularCPF;
                if (conferencia.TitularDataNascimento != DateTime.MinValue)
                    txtDataNascimento.Text = conferencia.TitularDataNascimento.ToString("dd/MM/yyyy");

                Beneficiario beneficiario = new Beneficiario(conferencia.Titular_BeneficiarioID);
                beneficiario.Carregar();
                this.ExibeBeneficiarioCarregado(beneficiario, null);
                cmdCarregaBeneficiarioPorCPF_Click(null, null);
                txtTitPeso.Text = conferencia.TitularPeso.ToString("N2");
                txtTitAltura.Text = conferencia.TitularAltura.ToString("N2");

                List<AdicionalBeneficiario> itens = new List<AdicionalBeneficiario>();
                if (conferencia.AdicionalIDs != null && conferencia.AdicionalIDs.Count > 0)
                {
                    Adicional adicional = null;
                    foreach (String adicionalId in conferencia.AdicionalIDs)
                    {
                        AdicionalBeneficiario ab = new AdicionalBeneficiario();
                        adicional = new Adicional(adicionalId);
                        adicional.Carregar();
                        ab.AdicionalID = adicionalId;
                        ab.AdicionalDescricao = adicional.Descricao;
                        ab.BeneficiarioID = conferencia.Titular_BeneficiarioID;
                        itens.Add(ab);
                    }

                    //guarda a colecao de adicionais no viewstate para o titular
                    ViewState["adben_" + Convert.ToString(conferencia.Titular_BeneficiarioID)] = itens;
                    this.CarregaAdicionais(false);
                }
            }
            else
            {
                txtCPF.Text = conferencia.TitularCPF;
                txtTitAltura.Text = conferencia.TitularAltura.ToString("N2");
                txtTitPeso.Text = conferencia.TitularPeso.ToString("N2");
            }
            #endregion

            #region DEPENDENTES


            IList<ContratoBeneficiario> lista = this.Dependentes;
            if (lista == null) { lista = new List<ContratoBeneficiario>(); }

            ContratoBeneficiario cb = null;
            IList<ConferenciaBeneficiario> dependentes = ConferenciaBeneficiario.Carregar(conferencia.ID);
            if (dependentes != null)
            {
                int sequencial = 0;
                foreach (ConferenciaBeneficiario dep in dependentes)
                {
                    if (dep.BeneficiarioID != null)
                    {
                        sequencial++;
                        cb = new ContratoBeneficiario();
                        cb.Altura = dep.Altura;
                        cb.Ativo = true;
                        cb.BeneficiarioID = dep.BeneficiarioID;
                        cb.BeneficiarioCPF = dep.CPF;
                        cb.DataCasamento = dep.DataCasamento;
                        cb.EstadoCivilID = dep.EstadoCivilID;
                        cb.ParentescoDescricao = dep.ParentescoDescricao;
                        cb.ParentescoID = dep.ParentescoID;
                        cb.Peso = dep.Peso;
                        cb.Valor = dep.Valor;
                        cb.Tipo = (int)ContratoBeneficiario.TipoRelacao.Dependente;
                        cb.Data = dep.PropostaData;
                        cb.NumeroSequencial = sequencial;

                        if (dep.AdicionalIDs != null && dep.AdicionalIDs.Count > 0)
                        {
                            CheckBox chk = null;
                            foreach (String id in dep.AdicionalIDs)
                            {
                                for (int i = 0; i < gridAdicional.Rows.Count; i++)
                                {
                                    if (Convert.ToString(gridAdicional.DataKeys[i][0]) == id)
                                    {
                                        chk = (CheckBox)gridAdicional.Rows[i].Cells[1].FindControl("chkSimAd");
                                        chk.Checked = true;
                                        this.checkboxGridAdicional_Changed(chk, null);
                                        break;
                                    }
                                }
                            }
                        }

                        lista.Add(cb);
                    }
                }

                this.Dependentes = lista;

                gridDependentes.DataSource = this.Dependentes;
                gridDependentes.SelectedIndex = -1;
                gridDependentes.DataBind();

                this.DependenteID = null;
                this.MontaCombosDeBeneficiarios();
            }
            #endregion

            this.ExibeSumario();
        }

        void CarregaOperadoras()
        {
            if (base.IDKeyParameterInProcess(ViewState, "_contr"))
                base.ExibirOperadoras(cboOperadora, true, false);
            else
                base.ExibirOperadoras(cboOperadora, true, true);
        }

        void CarregaEstadoCivil()
        {
            cboEstadoCivil.Items.Clear();
            cboEstadoCivilDependente.Items.Clear();

            if (!base.HaItemSelecionado(cboOperadora)) { return; }

            IList<EstadoCivil> lista = EstadoCivil.CarregarTodos(cboOperadora.SelectedValue);
            cboEstadoCivil.DataValueField = "ID";
            cboEstadoCivil.DataTextField = "Descricao";
            cboEstadoCivil.DataSource = lista;
            cboEstadoCivil.DataBind();

            cboEstadoCivilDependente.DataValueField = "ID";
            cboEstadoCivilDependente.DataTextField = "Descricao";
            cboEstadoCivilDependente.DataSource = lista;
            cboEstadoCivilDependente.DataBind();

        }

        void CarregaPlanos()
        {
            this.CarregaAdicionais(true);
            if (!base.HaItemSelecionado(cboContrato)) { cboPlano.Items.Clear(); cboAcomodacao.Items.Clear(); return; }
            IList<Plano> planos = Plano.CarregarPorContratoID(cboContrato.SelectedValue, true);

            cboPlano.Items.Clear();
            cboPlano.DataValueField = "ID";
            cboPlano.DataTextField = "DescricaoPlanoSubPlano";
            cboPlano.DataSource = planos;
            cboPlano.DataBind();
            cboPlano.Items.Insert(0, new ListItem("Selecione", "-1"));

            cboPlanoAltera.Items.Clear();
            cboPlanoAltera.DataValueField = "ID";
            cboPlanoAltera.DataTextField = "DescricaoPlanoSubPlano";
            cboPlanoAltera.DataSource = planos;
            cboPlanoAltera.DataBind();

            this.CarregaAcomodacoes();
        }

        void CarregaAcomodacoes()
        {
            if (cboPlano.SelectedIndex > 0)
            {
                Plano plano = new Plano(cboPlano.SelectedValue);
                plano.Carregar();

                base.ExibirTiposDeAcomodacao(cboAcomodacao, plano.QuartoComum, plano.QuartoParticular, true);
                ///////////////////////////////////////////////////////////////////////////////////////////
                this.CarregaAcomodacoesAlteracao(plano);
            }
            else
            {
                cboAcomodacao.Items.Clear();
                cboAcomodacao.Items.Add(new ListItem("Selecione", "-1"));
                cboAcomodacaoAltera.Items.Clear();
            }
        }

        void CarregaAcomodacoesAlteracao(Plano plano)
        {
            base.ExibirTiposDeAcomodacao(cboAcomodacaoAltera, plano.QuartoComum, plano.QuartoParticular, false);
        }

        protected void cmdCarregarComboFichaSaudeBeneficiarios_Click(Object sender, EventArgs e) { }

        void CarregaDependentes()
        {
            gridDependentes.DataSource = ContratoBeneficiario.CarregarPorContratoID(ViewState[IDKey], true);
            gridDependentes.DataBind();
            spanDependentesCadastrados.Visible = gridDependentes.Rows.Count > 0;
        }

        void CarregaOpcoesParaAgregadosOuDependentes()
        {
            cboParentescoDependente.Items.Clear();
            cboParentescoResponsavel.Items.Clear();

            if (cboContrato.Items.Count == 0) { return; }

            IList<ContratoADMParentescoAgregado> lista =
                ContratoADMParentescoAgregado.Carregar(
                cboContrato.SelectedValue,
                Parentesco.eTipo.Indeterminado);

            cboParentescoDependente.DataValueField = "ID";
            cboParentescoDependente.DataTextField = "ParentescoDescricao";
            cboParentescoResponsavel.DataValueField = "ID";
            cboParentescoResponsavel.DataTextField = "ParentescoDescricao";

            cboParentescoDependente.DataSource = lista;
            cboParentescoDependente.DataBind();
            cboParentescoResponsavel.DataSource = lista;
            cboParentescoResponsavel.DataBind();
        }

        Boolean validaPeso(TextBox txtPeso)
        {
            if (this.contratoId != null) { return true; }

            Decimal peso = base.CToDecimal(txtPeso.Text);

            if (peso >= Decimal.Zero && peso <= 300M)//if (peso >= 1M && peso <= 300M)
                return true;
            else
                return false;
        }

        Boolean validaAltura(TextBox txtAlt)
        {
            if (this.contratoId != null) { return true; }

            Decimal altura = base.CToDecimal(txtAlt.Text);

            if (altura >= decimal.Zero && altura <= 2.5M)//if (altura >= 0.1M && altura <= 2.5M)
                return true;
            else
                return false;
        }

        Boolean validaIMC_Titular(TextBox txtpeso, TextBox txtaltura)
        {
            if (this.contratoId != null) { return true; }

            Decimal imc = getIMC(txtpeso.Text, txtaltura.Text);

            if (imc > 30M)
                return false;
            else
                return true;
        }

        Decimal getIMC(String strpeso, String straltura)
        {
            Decimal peso = base.CToDecimal(strpeso);
            Decimal altura = base.CToDecimal(straltura);

            if (altura == 0) return decimal.Zero;

            return (peso / (altura * altura));
        }

        #region dados comuns

        void CarregaFiliais()
        {
            base.ExibirFiliais(cboFilial, false);
            cboFilial.Items.Insert(0, new ListItem("", "branco"));

            if (Session[FilialIDKey] != null)
            {
                cboFilial.SelectedValue = Convert.ToString(Session[FilialIDKey]);
            }
        }

        protected void txtContrato_TextChanged(Object sender, EventArgs e)
        {
            lnkOkContrato.Visible = true;
            cmdSalvar.Enabled = false;
        }

        protected void lnkOkContrato_Click(Object sender, EventArgs e)
        {
            Boolean teste = true;
            this.ContratoImpressoID = null;

            if (txtNumeroContrato.Text.Trim() == "")
            {
                base.Alerta(MPE, ref litAlert, "Informe o número de contrato.", upnlAlerta);
                txtNumeroContrato.Focus();
                teste = false;
            }

            if (txtNumeroContratoConfirme.Text.Trim() == "")
            {
                base.Alerta(MPE, ref litAlert, "Informe o número de contrato.", upnlAlerta);
                txtNumeroContratoConfirme.Focus();
                teste = false;
            }

            if (txtNumeroContrato.Text.Trim() != txtNumeroContratoConfirme.Text.Trim())
            {
                base.Alerta(MPE, ref litAlert, "Os números de contrato informados não coincidem.", upnlAlerta);
                txtNumeroContrato.Focus();
                teste = false;
            }

            if (!teste)
            {
                cmdSalvar.Enabled = false;
                upSalvar.Update();
                return;
            }

            teste = Contrato.NumeroJaUtilizado(txtNumeroContrato.Text, this.contratoId);

            if (!teste)
            {
                base.Alerta(null, this, "_err", "Número de contrato ja utilizado.\\nFavor verificar.");
            }

            cmdSalvar.Enabled = true;
            upSalvar.Update();

            if (ViewState[IDKey] == null)
            {
                txtCorretor.Text = "";
                txtCorretorID.Value = "";
                txtOperador.Text = "";
                txtOperadorID.Value = "";
                cboOperadora.SelectedIndex = 0;
                limpaCamposDaProposta();

                cmdSalvar.Enabled = true;
                //txtCorretor.ReadOnly = false;
                cboOperadora.Enabled = true;
            }
        }

        void limpaCamposDaProposta()
        {
            //se NAO está salvo o contrato, limpa os campos
            txtNumeroMatricula.Text = "";
            cboTipoProposta.SelectedIndex = 0;
            cboEstipulante.SelectedIndex = 0;

            cboContrato.Items.Clear();
            cboPlano.Items.Clear();
            cboAcomodacao.Items.Clear();
            txtAdmissao.Text = "";
            txtVigencia.Text = "";
            txtVencimento.Text = "";
            LimpaCamposDeDependente();
            LimpaCamposENDERECO_TITULAR();
            TitularID = null;
            Dependentes = null;
            gridEnderecosDisponiveis_Titular.DataSource = null;
            gridEnderecosDisponiveis_Titular.DataBind();
            gridEnderecosSelecionados.DataSource = null;
            gridEnderecosSelecionados.DataBind();
            ViewState.Remove(PropostaEndCobranca);
            ViewState.Remove(PropostaEndReferecia);
            gridDependentes.DataSource = null;
            gridDependentes.DataBind();

            cboBeneficiarioFicha.Items.Clear();
            dlFicha.DataSource = null;
            dlFicha.DataBind();

            //cboBeneficiarioAdicional.Items.Clear();
            gridAdicional.DataSource = null;
            gridAdicional.DataBind();

            ExibeBeneficiarioCarregado(null, null);

            litSumario.Text = "";
            txtValorTotal.Text = "";
            txtObs.Text = "";
            txtObsEdit.Text = "";

            upDadosComuns.Update();
            upFinalizacao.Update();
        }

        protected void gridSelNumeroContral_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("usar"))
            {
                Object id = gridSelNumeroContral.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Object opid = gridSelNumeroContral.DataKeys[Convert.ToInt32(e.CommandArgument)][1];
                Object agid = gridSelNumeroContral.DataKeys[Convert.ToInt32(e.CommandArgument)][2];

                this.PreencheCampoCorretor(agid);

                cboOperadora.SelectedValue = Convert.ToString(opid);
                cboOperadora_SelectedIndexChanged(cboOperadora, null);
                cboOperadora.Enabled = false;
                cmdSalvar.Enabled = true;
                upSalvar.Update();
                txtNumeroMatricula.Focus();

                if (ViewState[IDKey] == null)
                {
                    this.limpaCamposDaProposta();
                }

                AlmoxContratoImpresso aci = new AlmoxContratoImpresso(id);
                aci.Carregar();
                if (aci.Rasurado)
                {
                    txtCorretor.Text = "";
                    txtCorretorID.Value = "";
                    txtOperador.Text = "";
                    txtOperadorID.Value = "";
                    cboOperadora.SelectedIndex = 0;
                    cboOperadora.Enabled = true;
                    cboOperadora.SelectedIndex = 0;
                    cboOperadora_SelectedIndexChanged(null, null);
                    this.ContratoImpressoID = null;
                }
                else
                    this.ContratoImpressoID = aci.ID;
            }
        }

        protected void cboTipoProposta_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            TipoContrato tipo = new TipoContrato();
            tipo.ID = cboTipoProposta.SelectedValue;
            tipo.Carregar();
            pnlInfoAnterior.Visible = tipo.SolicitarInfoAnterior;
            this.ExibeSumario();
        }

        protected void txtAdmissao_OnTextChanged(Object sender, EventArgs e)
        {
            DateTime date = base.CStringToDateTime(txtAdmissao.Text);

            DateTime hoje = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (date == DateTime.MinValue || date > hoje)
            {
                txtVigencia.Text = "";
                txtVencimento.Text = "";
                base.Alerta(MPE, ref litAlert, "Data de admissão inválida.", upnlAlerta);
            }

            DateTime vigencia, vencimento;
            Int32 diaDataSemJuros; Object valorDataLimite;
            CalendarioVencimento rcv = null;

            if (!base.HaItemSelecionado(cboContrato)) { return; }
            CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(cboContrato.SelectedValue,
                date, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv);

            if (vigencia == DateTime.MinValue)
                txtVigencia.Text = "";
            else
                txtVigencia.Text = vigencia.ToString("dd/MM/yyyy"); ;

            if (vencimento == DateTime.MinValue)
                txtVencimento.Text = "";
            else
                txtVencimento.Text = vencimento.ToString("dd/MM/yyyy");

            if (vigencia == DateTime.MinValue || vencimento == DateTime.MinValue)
            {
                base.Alerta(MPE, ref litAlert, "Data de admissão não coberta pelo calendário da operadora.", upnlAlerta);
                cmdSalvar.Enabled = true;
            }
            else
            {
                //TODO: sabe-se que tem vigencia e vencimento. mas tem que checar se tem RECEBIMENTO.
                cmdSalvar.Enabled = true;
            }

            upSalvar.Update();
        }

        void CarregaContratoADM()
        {
            cboContrato.Items.Clear();
            if (!base.HaItemSelecionado(cboEstipulante) || !base.HaItemSelecionado(cboOperadora))
            {
                cboContrato.Items.Clear();
                return;
            }

            cboContrato.DataValueField = "ID";
            cboContrato.DataTextField = "DescricaoCodigoSaudeDental";

            IList<ContratoADM> lista = null;
            if (base.IDKeyParameterInProcess(ViewState, "_contr"))
                lista = ContratoADM.CarregarTodos(
                    cboEstipulante.SelectedValue, cboOperadora.SelectedValue, false);
            else
                lista = ContratoADM.Carregar(
                    cboEstipulante.SelectedValue, cboOperadora.SelectedValue, true);

            cboContrato.DataSource = lista;
            cboContrato.DataBind();
            cboContrato.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratoADM();
            this.CarregaPlanos();
            this.CarregaEstadoCivil();
            this.CarregaFichaDeSaude(true);
            this.carregaAdicionais();
        }

        protected void cboEstipulante_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            EstipulanteTaxa taxa = EstipulanteTaxa.CarregarVigente(cboEstipulante.SelectedValue);
            if (taxa == null)
            {
                chkCobrarTaxa.Checked = false;
                chkCobrarTaxa.Visible = false;
            }
            else
            {
                chkCobrarTaxa.Checked = true;
                chkCobrarTaxa.Visible = true;
            }

            this.CarregaContratoADM();
            this.CarregaPlanos();
            upFinalizacao.Update();
        }

        protected void chkCobrarTaxa_CheckedChanged(Object sender, EventArgs e)
        {
            this.ExibeSumario();
        }

        protected void cboContrato_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaPlanos();
            this.CarregaOpcoesParaAgregadosOuDependentes();
        }

        protected void cboPlano_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaAdicionais(true);
            this.CarregaAcomodacoes();
        }

        protected void cboPlanoAltera_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            Plano plano = new Plano(cboPlanoAltera.SelectedValue);
            plano.Carregar();
            this.CarregaAcomodacoesAlteracao(plano);
            MPEPlano.Show();
            upPlanoAlteracao.Update();
        }

        protected void cboAcomodacao_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.ExibeSumario();
            txtAdmissao.Focus();
        }

        protected void chkHistoricoPlano_OnCheckedChanged(Object sender, EventArgs e)
        {
            trHistoricoPlano.Visible = chkHistoricoPlano.Checked;
        }

        protected void gridHistoricoPlano_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                Object id = gridHistoricoPlano.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                ContratoPlano cp = new ContratoPlano();
                cp.ID = id;
                cp.Remover();
                gridHistoricoPlano.DataSource = ContratoPlano.Carregar(contratoId);
                gridHistoricoPlano.DataBind();
            }
        }

        protected void gridHistoricoPlano_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            UIHelper.AuthCtrl(e.Row.Cells[2], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente remover o item?\\nEssa ação não poderá ser desfeita.");
        }

        protected void cmdAlterarPlano_Click(Object sender, EventArgs e)
        {
            MPEPlano.Show();
        }

        protected void cmdFecharPlano_Click(Object sender, EventArgs e)
        {
            litMigrarPlanoMsg.Text = "";
            MPEPlano.Hide();
        }

        protected void cmdSalvarPlano_Click(Object sender, EventArgs e)
        {
            DateTime data = new DateTime();
            if (!UIHelper.TyParseToDateTime(txtPlanoAdmissao.Text, out data))
            {
                //base.Alerta(MPE, ref litAlert, "Atenção!<br>Data de admissão inválida.", upnlAlerta);
                MPEPlano.Show();
                litMigrarPlanoMsg.Text = "Data de admissão inválida.";
                txtPlanoAdmissao.Focus();
                return;
            }

            String planoId = cboPlanoAltera.SelectedValue;
            String acomodacaoId = cboAcomodacaoAltera.SelectedValue;

            cboPlano.SelectedValue = planoId;
            cboPlano_OnSelectedIndexChanged(null, null);

            cboAcomodacao.SelectedValue = acomodacaoId;
            cboAcomodacao_OnSelectedIndexChanged(null, null);

            ViewState[AlteraPlanoKey] = 1;
            ViewState[NovaDataAdmisssaoKey] = txtPlanoAdmissao.Text;
            upDadosComuns.Update();

            MPEPlano.Hide();
            litMigrarPlanoMsg.Text = "";
        }

        #endregion

        #region beneficiario titular e dados cadastrais

        protected void gridSelTitular_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("usar"))
            {
                Object id = gridSelTitular.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Beneficiario benef = new Beneficiario(id);
                benef.Carregar();
                this.ExibeBeneficiarioCarregado(benef, null);
                MontaCombosDeBeneficiarios();
                pnlSelTitular.Visible = false;
            }
        }

        public void callBackPostInUserControlTit() { }

        protected void cmdCarregaBeneficiarioPorCPF_Click(Object sender, EventArgs e)
        {
            if (txtCPF.Text.Trim() == "") { cmdCarregarComboFichaSaudeBeneficiarios.Visible = true; return; }

            Boolean cpfValido = UIHelper.ValidaCpf(txtCPF.Text);
            if (!cpfValido)
            {
                base.Alerta(MPE, ref litAlert, "Cpf inválido.", upnlAlerta);
                return;
            }
            //IList<Beneficiario> lista = Beneficiario.CarregarPorParametro(txtCPF.Text, base.CStringToDateTime(txtDataNascimento.Text));denis
            IList<Beneficiario> lista = Beneficiario.CarregarPorParametro(txtCPF.Text, DateTime.MinValue);

            if (lista == null)
                this.ExibeBeneficiarioCarregado(null, null);
            else if (lista.Count > 1)
            {
                gridSelTitular.DataSource = lista;
                gridSelTitular.DataBind();
                pnlSelTitular.Visible = true;
                return;
            }
            else
                this.ExibeBeneficiarioCarregado(lista[0], null);

            MontaCombosDeBeneficiarios();

            if (lista == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), "novoTit", "beneficiarioNaoLocalizado('1');", true);
            }
        }

        protected void cmdCarregaBeneficiarioPorRG_Click(Object sender, EventArgs e)
        {
            if (txtRG.Text.Trim() == "") { return; }
            IList<Beneficiario> lista = Beneficiario.CarregarPorParametro("", "", txtRG.Text, SearchMatchType.QualquerParteDoCampo);

            if (lista == null)
                this.ExibeBeneficiarioCarregado(null, null);
            else
                this.ExibeBeneficiarioCarregado(lista[0], null);

            MontaCombosDeBeneficiarios();

            if (lista == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), "novoTit", "beneficiarioNaoLocalizado('1');", true);
            }
        }

        /// <summary>
        /// TODO: Otimizar. Está carregando o ContratoBeneficiario titular novamente.
        /// </summary>
        void ExibeBeneficiarioCarregado(Beneficiario beneficiario, Object idEnderecoAExibir)
        {
            if (beneficiario == null)
            {
                this.TitularID = null;
                txtNome.Text = "";
                txtDataNascimento.Text = "";
                txtNomeMae.Text = "";
                txtFone1.Text = "";
                txtFone2.Text = "";
                txtFone3.Text = "";
                txtDDD1.Text = "";
                txtDDD2.Text = "";
                txtDDD3.Text = "";
                txtRamal1.Text = "";
                txtRamal2.Text = "";
                txtEmail.Text = "";
                txtTitAltura.Text = "";
                txtTitPeso.Text = "";
                cmdAlterarBeneficiarioTitular.Visible = false;
                ExibeEnderecosDeBeneficiarioCarregado(null, null);
                txtCarenciaCodigo.Text = "";
                txtPortabilidade.Text = "";
                txtCarenciaMatricula.Text = "";
                txtCarenciaTempoContrato.Text = "";
                txtTitTempoContratoDe.Text = "";
                txtTitTempoContratoAte.Text = "";
            }
            else
            {
                this.TitularID = beneficiario.ID;
                txtNome.Text = beneficiario.Nome.ToUpper();
                txtCPF.Text = beneficiario.CPF;
                txtRG.Text = beneficiario.RG;

                if (!String.IsNullOrEmpty(beneficiario.Sexo))
                    cboSexo.SelectedValue = beneficiario.Sexo;

                if (beneficiario.DataNascimento != DateTime.MinValue)
                    txtDataNascimento.Text = beneficiario.DataNascimento.ToString("dd/MM/yyyy");

                txtNomeMae.Text = beneficiario.NomeMae;

                if (ViewState[IDKey] != null)
                {
                    ContratoBeneficiario cb = ContratoBeneficiario.CarregarPorContratoEBeneficiario(ViewState[IDKey], beneficiario.ID, null);

                    if (cb != null)
                    {
                        txtTitPeso.Text = cb.Peso.ToString("N2"); //beneficiario.Peso.ToString("N2");
                        txtTitAltura.Text = cb.Altura.ToString("N2");//beneficiario.Altura.ToString("N2");
                        this.SetaEstadoCivil(cb.EstadoCivilID);
                    }
                }

                txtFone1.Text = base.PegaTelefone(beneficiario.Telefone);
                txtFone2.Text = base.PegaTelefone(beneficiario.Telefone2);
                txtFone3.Text = base.PegaTelefone(beneficiario.Celular);
                txtDDD1.Text = base.PegaDDD(beneficiario.Telefone);
                txtDDD2.Text = base.PegaDDD(beneficiario.Telefone2);
                txtDDD3.Text = base.PegaDDD(beneficiario.Celular);
                txtRamal1.Text = beneficiario.Ramal;
                txtRamal2.Text = beneficiario.Ramal2;
                txtEmail.Text = beneficiario.Email;

                this.PreparaLinkParaEditarTitular();
                cmdAlterarBeneficiarioTitular.Visible = true;
                ExibeEnderecosDeBeneficiarioCarregado(beneficiario.ID, idEnderecoAExibir);

                cboEstadoCivil.Focus();
            }
        }

        void ExibeEnderecosDeBeneficiarioCarregado(Object beneficiarioId, Object idEnderecoAExibir)
        {
            IList<Endereco> lista = null;

            if (beneficiarioId != null)
            {
                lista = Endereco.CarregarPorDono(beneficiarioId, Endereco.TipoDono.Beneficiario);
            }

            if (lista == null)
            {
                ExibeEnderecoDeBeneficiarioCarregado(null);
                gridEnderecosDisponiveis_Titular.DataSource = null;
                gridEnderecosDisponiveis_Titular.DataBind();
                spanEnderecosDisponiveis_Titular.Visible = false;
            }
            else
            {
                gridEnderecosDisponiveis_Titular.DataSource = lista;
                gridEnderecosDisponiveis_Titular.DataBind();
                spanEnderecosDisponiveis_Titular.Visible = true;
            }
        }

        void ExibeEnderecoDeBeneficiarioCarregado(Endereco endereco)
        {
            if (endereco == null)
            {
                txtCEP.Text = "";
                txtLogradouro.Text = "";
                txtNumero.Text = "";
                txtComplemento.Text = "";
                txtBairro.Text = "";
                txtCidade.Text = "";
                txtUF.Text = "";
            }
            else
            {
                txtCEP.Text = endereco.CEP;
                txtLogradouro.Text = endereco.Logradouro;
                txtNumero.Text = endereco.Numero;
                txtComplemento.Text = endereco.Complemento;
                txtBairro.Text = endereco.Bairro;
                txtCidade.Text = endereco.Cidade;
                txtUF.Text = endereco.UF.ToUpper();
                cboTipoEndereco.SelectedValue = Convert.ToString(endereco.Tipo);
                cmdEnderecoAcoes.Text = "Alterar";
            }
        }

        protected void gridEnderecosDisponiveis_Titular_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("usar"))
            {
                Object id = gridEnderecosDisponiveis_Titular.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Endereco endereco = new Endereco();
                endereco.ID = id;
                endereco.Carregar();
                this.ExibeEnderecoDeBeneficiarioCarregado(endereco);

            }
            else if (e.CommandName.Equals("excluir"))
            {
                Object id = gridEnderecosDisponiveis_Titular.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Endereco endereco = new Endereco();
                endereco.ID = id;
                endereco.Remover();
                this.ExibeEnderecosDeBeneficiarioCarregado(this.TitularID, null);
            }
            if (e.CommandName.Equals("cobranca"))
            {
                ViewState[PropostaEndCobranca] = gridEnderecosDisponiveis_Titular.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                this.ExibeEnderecosDaProposta();

            }
            else if (e.CommandName.Equals("referencia"))
            {
                ViewState[PropostaEndReferecia] = gridEnderecosDisponiveis_Titular.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                this.ExibeEnderecosDaProposta();
            }
        }

        protected void gridEnderecosSelecionados_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                if (Convert.ToInt32(e.CommandArgument) == 0)
                {
                    if (
                        Convert.ToString(gridEnderecosSelecionados.DataKeys[Convert.ToInt32(e.CommandArgument)].Value) ==
                        CToString(ViewState[PropostaEndReferecia]))
                    {
                        ViewState.Remove(PropostaEndReferecia);
                        this.ExibeEnderecosDaProposta();
                    }
                    else if (
                        Convert.ToString(gridEnderecosSelecionados.DataKeys[Convert.ToInt32(e.CommandArgument)].Value) ==
                        CToString(ViewState[PropostaEndCobranca]))
                    {
                        ViewState.Remove(PropostaEndCobranca);
                        this.ExibeEnderecosDaProposta();
                    }
                }
                else
                {
                    if (
                        Convert.ToString(gridEnderecosSelecionados.DataKeys[Convert.ToInt32(e.CommandArgument)].Value) ==
                        CToString(ViewState[PropostaEndCobranca]))
                    {
                        ViewState.Remove(PropostaEndCobranca);
                        this.ExibeEnderecosDaProposta();
                    }
                    else if (
                        Convert.ToString(gridEnderecosSelecionados.DataKeys[Convert.ToInt32(e.CommandArgument)].Value) ==
                        CToString(ViewState[PropostaEndReferecia]))
                    {
                        ViewState.Remove(PropostaEndReferecia);
                        this.ExibeEnderecosDaProposta();
                    }
                }
            }
        }

        protected void gridEnderecosSelecionados_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            UIHelper.AuthCtrl(e.Row.Cells[2], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente remover o endereço?");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == 0)
                {
                    if (Convert.ToString(gridEnderecosSelecionados.DataKeys[e.Row.RowIndex].Value) ==
                        CToString(ViewState[PropostaEndReferecia]))
                    {
                        e.Row.Cells[1].Text = "REFERENCIA";
                    }
                    else if (Convert.ToString(gridEnderecosSelecionados.DataKeys[e.Row.RowIndex].Value) ==
                        CToString(ViewState[PropostaEndCobranca]))
                    {
                        e.Row.Cells[1].Text = "COBRANCA";
                    }
                }
                else
                {
                    if (Convert.ToString(gridEnderecosSelecionados.DataKeys[e.Row.RowIndex].Value) ==
                        CToString(ViewState[PropostaEndCobranca]))
                    {
                        e.Row.Cells[1].Text = "COBRANCA";
                    }
                    else if (Convert.ToString(gridEnderecosSelecionados.DataKeys[e.Row.RowIndex].Value) ==
                        CToString(ViewState[PropostaEndReferecia]))
                    {
                        e.Row.Cells[1].Text = "REFERENCIA";
                    }
                }
            }
        }

        void ExibeEnderecosDaProposta()
        {
            System.Collections.ArrayList arr = new System.Collections.ArrayList();
            List<Endereco> lista = null;

            if (ViewState[PropostaEndCobranca] != null)
            {
                Endereco endereco = new Endereco();
                endereco.ID = ViewState[PropostaEndCobranca];
                endereco.Carregar();
                if (lista == null) { lista = new List<Endereco>(); }
                lista.Add(endereco);
            }
            if (ViewState[PropostaEndReferecia] != null)
            {
                Endereco endereco = new Endereco();
                endereco.ID = ViewState[PropostaEndReferecia];
                endereco.Carregar();
                if (lista == null) { lista = new List<Endereco>(); }
                lista.Add(endereco);
            }

            gridEnderecosSelecionados.DataSource = lista;
            gridEnderecosSelecionados.DataBind();
            upFinalizacao.Update();
        }

        protected void gridEnderecosDisponiveis_Titular_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((ImageButton)e.Row.Cells[1].Controls[0]).Attributes.Add("title", "visualizar");
            }
        }

        protected void cmdEnderecoAcoes_Click(Object sender, EventArgs e)
        {
            switch (cmdEnderecoAcoes.Text)
            {
                case "Alterar":
                    {
                        Endereco endereco = new Endereco();
                        endereco.ID = gridEnderecosDisponiveis_Titular.DataKeys[gridEnderecosDisponiveis_Titular.SelectedIndex].Value;
                        endereco.Carregar();
                        endereco.Bairro = txtBairro.Text;
                        endereco.CEP = txtCEP.Text;
                        endereco.Cidade = txtCidade.Text;
                        endereco.Complemento = txtComplemento.Text;
                        endereco.Logradouro = txtLogradouro.Text;
                        endereco.Numero = txtNumero.Text;
                        endereco.Tipo = Convert.ToInt32(cboTipoEndereco.SelectedValue);
                        endereco.UF = txtUF.Text;
                        endereco.Salvar();

                        ExibeEnderecosDeBeneficiarioCarregado(endereco.DonoId, null);
                        gridEnderecosDisponiveis_Titular.SelectedIndex = -1;

                        this.LimpaCamposENDERECO_TITULAR();
                        cmdEnderecoAcoes.Text = "Gravar";
                        break;
                    }
                case "Gravar":
                    {
                        Endereco endereco = new Endereco();
                        endereco.DonoId = this.TitularID;
                        endereco.DonoTipo = Convert.ToInt32(Endereco.TipoDono.Beneficiario);
                        endereco.Bairro = txtBairro.Text;
                        endereco.CEP = txtCEP.Text;
                        endereco.Cidade = txtCidade.Text;
                        endereco.Complemento = txtComplemento.Text;
                        endereco.Logradouro = txtLogradouro.Text;
                        endereco.Numero = txtNumero.Text;
                        endereco.Tipo = Convert.ToInt32(cboTipoEndereco.SelectedValue);
                        endereco.UF = txtUF.Text;
                        endereco.Salvar();

                        ExibeEnderecosDeBeneficiarioCarregado(endereco.DonoId, endereco.ID);
                        cmdEnderecoAcoes.Text = "Alterar";
                        break;
                    }
            }
        }

        void LimpaCamposENDERECO_TITULAR()
        {
            txtBairro.Text = "";
            txtCEP.Text = "";
            txtCidade.Text = "";
            txtComplemento.Text = "";
            txtLogradouro.Text = "";
            txtNumero.Text = "";
            txtUF.Text = "";
        }

        #endregion

        #region dependentes

        protected void gridSelDependente_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("usar"))
            {
                Object id = gridSelDependente.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Beneficiario benef = new Beneficiario(id);
                benef.Carregar();
                this.ExibeBeneficiarioDependenteCarregado(benef);
                this.MontaCombosDeBeneficiarios();
                pnlSelDependente.Visible = false;
            }
        }

        protected void cmdCarregaBeneficiarioDependentePorCPF_Click(Object sender, EventArgs e)
        {
            if (txtCPFDependente.Text.Trim().Replace("_", "").Replace(".", "").Replace("-", "").Length == 0) { return; }

            Boolean cpfValido = UIHelper.ValidaCpf(txtCPFDependente.Text);
            if (!cpfValido)
            {
                if (Session[ConferenciaObjKey] == null) //só emite o alerta se não for um cadastro vindo da conferencia
                    base.Alerta(MPE, ref litAlert, "Cpf inválido.", upnlAlerta);
                return;
            }

            IList<Beneficiario> lista = Beneficiario.CarregarPorParametro(txtCPFDependente.Text, base.CStringToDateTime(txtDataNascimentoDependente.Text));

            if (lista == null)
                this.ExibeBeneficiarioDependenteCarregado(null);
            else if (lista.Count > 1)
            {
                gridSelDependente.DataSource = lista;
                gridSelDependente.DataBind();
                pnlSelDependente.Visible = true;
                return;
            }
            else
                this.ExibeBeneficiarioDependenteCarregado(lista[0]);

            this.MontaCombosDeBeneficiarios();

            if (lista == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), "novoDep", "beneficiarioNaoLocalizado('2');", true);
            }
        }

        protected void cmdCarregaBeneficiarioDependentePorRG_Click(Object sender, EventArgs e)
        {
            if (txtRGDependente.Text.Trim() == "") { return; }
            IList<Beneficiario> lista = Beneficiario.CarregarPorParametro(txtRGDependente.Text, "", "", SearchMatchType.QualquerParteDoCampo);

            if (lista == null)
                this.ExibeBeneficiarioDependenteCarregado(null);
            else if (lista.Count > 1)
            {
                gridSelDependente.DataSource = lista;
                gridSelDependente.DataBind();
                pnlSelDependente.Visible = true;
                return;
            }
            else
                this.ExibeBeneficiarioDependenteCarregado(lista[0]);

            MontaCombosDeBeneficiarios();

            if (lista == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), "novoDep", "beneficiarioNaoLocalizado('2');", true);
            }
        }

        void ExibeBeneficiarioDependenteCarregado(Beneficiario beneficiario)
        {
            if (beneficiario == null)
            {
                this.DependenteID = null;
                txtNomeDependente.Text = "";
                txtDataNascimentoDependente.Text = "";
                txtDepAltura.Text = "";
                txtDepPeso.Text = "";
                txtDepAdmissao.Text = "";
            }
            else
            {
                this.DependenteID = beneficiario.ID;
                txtNomeDependente.Text = beneficiario.Nome.ToUpper();
                txtCPFDependente.Text = beneficiario.CPF;
                txtRGDependente.Text = beneficiario.RG;

                if (!String.IsNullOrEmpty(beneficiario.Sexo))
                    cboSexoDependente.SelectedValue = beneficiario.Sexo;

                if (beneficiario.DataNascimento != DateTime.MinValue)
                    txtDataNascimentoDependente.Text = beneficiario.DataNascimento.ToString("dd/MM/yyyy");

                if (ViewState[IDKey] != null)
                {
                    ContratoBeneficiario cb = ContratoBeneficiario.CarregarPorContratoEBeneficiario(ViewState[IDKey], beneficiario.ID, null);

                    if (cb != null)
                    {
                        txtDepPeso.Text = cb.Peso.ToString("N2"); //beneficiario.Peso.ToString("N2");
                        txtDepAltura.Text = cb.Altura.ToString("N2");//beneficiario.Altura.ToString("N2");
                        cboEstadoCivilDependente.SelectedValue = Convert.ToString(cb.EstadoCivilID);
                    }
                }

                cboParentescoDependente.Focus();
            }
        }

        protected void cmdAddDependente_Click(Object sender, EventArgs e)
        {
            #region validacao

            if (this.DependenteID == null || cboParentescoDependente.Items.Count == 0)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Selecione um parentesco.", upnlAlerta);
                return;
            }

            if (!validaAltura(txtDepAltura))
            {
                base.Alerta(null, this, "_altDep", "Atenção!\\nA altura do dependente deve estar entre 10cm e 2,5m. ");
                txtDepAltura.Focus();
                return;
            }

            if (!validaPeso(txtDepPeso))
            {
                base.Alerta(null, this, "_pesDep", "Atenção!\\nO peso do dependente deve estar entre 1kg e 300kg. ");
                txtDepPeso.Focus();
                return;
            }

            Decimal imcdep = getIMC(txtDepPeso.Text, txtDepAltura.Text);
            if (imcdep > 30M)
            {
                base.Alerta(null, this, "_err", "Atenção!\\nIMC fora da faixa. Encaminhar para área técnica.");
                return;
            }

            //if (txtCarenciaDependenteCodigo.Text.Trim() == "" && this.contratoId == null)
            //{
            //    base.Alerta(null, this, "_PRCTit", "Atenção!\\nPRC do dependente é obrigatório.");
            //    txtCarenciaDependenteCodigo.Focus();
            //    return;
            //}

            #region checa regras para agregados
            //se o contrato ja está salvo, checa as regras de adicao de agregados/dependentes
            if (ViewState[IDKey] != null && base.HaItemSelecionado(cboContrato))
            {
                ContratoADMParentescoAgregado parentesco = new ContratoADMParentescoAgregado(cboParentescoDependente.SelectedValue);
                parentesco.Carregar();
                IList<AgregadoRegra> regras = AgregadoRegra.Carregar(cboContrato.SelectedValue, (Parentesco.eTipo)parentesco.ParentescoTipo);
                if (regras != null)
                {
                    foreach (AgregadoRegra regra in regras)
                    {
                        if (regra.TipoAgregado == parentesco.ParentescoTipo)
                        {
                            if (((AgregadoRegra.eTipoLimite)regra.TipoLimite) == AgregadoRegra.eTipoLimite.LimiteDeIdade)
                            {
                                Beneficiario benef = new Beneficiario(this.DependenteID);
                                benef.Carregar();
                                int idade = Beneficiario.CalculaIdade(benef.DataNascimento);
                                benef = null;

                                if (idade > regra.ValorLimite)
                                {
                                    base.Alerta(MPE, ref litAlert, "Não é possível adicionar esse beneficiário. A seguinte regra foi infringida:<br><br>" + regra.ToString(), upnlAlerta);
                                    return;
                                }
                            }
                            else if (((AgregadoRegra.eTipoLimite)regra.TipoLimite) == AgregadoRegra.eTipoLimite.LimiteDiasDeContrato && contratoId != null)
                            {
                                Contrato contrato = new Contrato(contratoId);
                                contrato.Carregar();
                                String[] tempoContrato = base.DateDiff(1, contrato.Admissao).Replace("a", "").Replace("m", "").Replace("d", "").Split(' ');

                                int dias = Convert.ToInt32(tempoContrato[2]);

                                if (tempoContrato[0] != "0") //checa se passaram anos
                                {
                                    dias += (Convert.ToInt32(tempoContrato[0]) * 365);
                                }
                                if (tempoContrato[1] != "0") //checa se passaram meses
                                {
                                    dias += (Convert.ToInt32(tempoContrato[1]) * 30);
                                }

                                if (dias > regra.ValorLimite)
                                {
                                    base.Alerta(MPE, ref litAlert, "Não é possível adicionar esse beneficiário. A seguinte regra foi infringida:<br><br>" + regra.ToString(), upnlAlerta);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            DateTime data = new DateTime();
            if (!UIHelper.TyParseToDateTime(txtDepAdmissao.Text, out data))
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Data de admissão inválida.", upnlAlerta);
                txtDepAdmissao.Focus();
                return;
            }

            IList<ContratoBeneficiario> lista = this.Dependentes;
            if (lista == null) { lista = new List<ContratoBeneficiario>(); }
            else if (gridDependentes.SelectedIndex == -1)
            {
                foreach (ContratoBeneficiario adicionado in this.Dependentes)
                {
                    if (Convert.ToString(adicionado.BeneficiarioID) ==
                        Convert.ToString(this.DependenteID))
                    {
                        base.Alerta(MPE, ref litAlert, "Atenção!<br>Dependente já consta no contrato.", upnlAlerta);
                        return;
                    }
                }
            }
            #endregion

            ContratoBeneficiario cb = new ContratoBeneficiario();

            if (gridDependentes.SelectedIndex > -1)
            {
                cb.ID = gridDependentes.DataKeys[gridDependentes.SelectedIndex].Value;
                if (cb.ID != null) { cb.Carregar(); }
            }

            cb.BeneficiarioID = this.DependenteID;
            cb.BeneficiarioNome = txtNomeDependente.Text;
            cb.ParentescoID = cboParentescoDependente.SelectedValue;
            cb.ParentescoDescricao = cboParentescoDependente.SelectedItem.Text;
            cb.NumeroMatriculaDental = txtNumMatriculaDentalDep.Text;
            cb.NumeroMatriculaSaude = txtNumMatriculaSaudeDep.Text;
            cb.EstadoCivilID = cboEstadoCivilDependente.SelectedValue;
            cb.EstadoCivilDescricao = cboEstadoCivilDependente.SelectedItem.Text;
            cb.DataCasamento = base.CStringToDateTime(txtDepDataCasamento.Text);

            cb.Tipo = (int)ContratoBeneficiario.TipoRelacao.Dependente;
            cb.Data = data;
            cb.Altura = UIHelper.CToDecimal(txtDepAltura.Text);
            cb.Peso = UIHelper.CToDecimal(txtDepPeso.Text);
            cb.ContratoID = ViewState[IDKey];

            if (ViewState[IDKey] != null && cb.ID == null) //se o contrato nao é novo, mas o beneficiario é
            {
                DateTime vigencia = DateTime.MinValue, vencimento = DateTime.MinValue;
                Int32 diasDataSemJuros = 0; Object valorDataLimite = null;
                CalendarioVencimento rcv = null;

                CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(cboContrato.SelectedValue,
                    cb.Data, out vigencia, out vencimento, out diasDataSemJuros, out valorDataLimite, out rcv);
                cb.Vigencia = vigencia;
            }

            cb.Portabilidade = txtDependentePortabilidade.Text;
            cb.CarenciaOperadora = cboCarenciaDependenteOperadora.Text;
            cb.CarenciaOperadoraID = base.CToObject(txtCarenciaDependenteOperadoraID.Value);
            cb.CarenciaCodigo = txtCarenciaDependenteCodigo.Text;

            cb.CarenciaContratoDe = base.CStringToDateTime(txtDepTempoContratoDe.Text);
            cb.CarenciaContratoAte = base.CStringToDateTime(txtDepTempoContratoAte.Text);
            cb.CarenciaMatriculaNumero = txtCarenciaDependenteMatricula.Text;

            if (cb.CarenciaContratoDe != DateTime.MinValue && cb.CarenciaContratoAte != DateTime.MinValue)
            {
                txtCarenciaDependenteTempoContrato.Text = base.GetMonthsBetween(cb.CarenciaContratoDe, cb.CarenciaContratoAte).ToString();
            }

            cb.CarenciaContratoTempo = base.CToInt(txtCarenciaDependenteTempoContrato.Text);

            if (gridDependentes.SelectedIndex == -1)
                lista.Add(cb);
            else
                lista[gridDependentes.SelectedIndex] = cb;

            this.Dependentes = lista;

            gridDependentes.DataSource = this.Dependentes;
            gridDependentes.SelectedIndex = -1;
            gridDependentes.DataBind();

            spanDependentesCadastrados.Visible = true;
            this.DependenteID = null;
            this.LimpaCamposDeDependente();
            this.MontaCombosDeBeneficiarios();

            //if (this.contratoId != null && cboBeneficiarioAdicional.Items.Count > 0 && gridAdicional.Rows.Count > 0)
            //{
            //    //se precisar, o sistema marcará o novo beneficiário com adicional
            //    CheckBox chk = ((CheckBox)gridAdicional.Rows[0].Cells[1].Controls[1]);
            //    if (chk != null && chk.Checked) { checkboxGridAdicional_Changed(chk, null); }
            //}
        }

        void LimpaCamposDeDependente()
        {
            this.DependenteID = null;
            txtRGDependente.Text = "";
            txtCPFDependente.Text = "";
            txtNomeDependente.Text = "";
            txtDataNascimentoDependente.Text = "";
            txtDepAltura.Text = "";
            txtDepPeso.Text = "";
            txtDepAdmissao.Text = "";

            cboCarenciaDependenteOperadora.Text = "";
            txtCarenciaDependenteCodigo.Text = "";
            txtCarenciaDependenteMatricula.Text = "";
            txtCarenciaDependenteTempoContrato.Text = "";
            txtDepTempoContratoDe.Text = "";
            txtDepTempoContratoAte.Text = "";
            txtDependentePortabilidade.Text = "";
        }

        protected void cmdAlterarBeneficiarioDependente_Click(Object sender, EventArgs e)
        {
        }

        protected void gridDependentes_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editarDadosDaProposta"))
            {
                Object id = gridDependentes.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                IList<ContratoBeneficiario> lista = this.Dependentes;
                ContratoBeneficiario cb = lista[Convert.ToInt32(e.CommandArgument)];

                if (cb.ID != null) { cb.Carregar(); }

                this.DependenteID = cb.BeneficiarioID;
                txtNomeDependente.Text = cb.BeneficiarioNome;

                if (cb.ParentescoID != null)
                {
                    if (cboParentescoDependente.Items.FindByValue(Convert.ToString(cb.ParentescoID)) != null)
                        cboParentescoDependente.SelectedValue = Convert.ToString(cb.ParentescoID);
                    else if (cboParentescoDependente.Items.FindByText(Convert.ToString(cb.ParentescoDescricao)) != null)
                        cboParentescoDependente.SelectedValue = cboParentescoDependente.Items.FindByText(Convert.ToString(cb.ParentescoDescricao)).Value;
                }

                txtNumMatriculaDentalDep.Text = cb.NumeroMatriculaDental;
                txtNumMatriculaSaudeDep.Text = cb.NumeroMatriculaSaude;

                if (cb.EstadoCivilID != null)
                    cboEstadoCivilDependente.SelectedValue = Convert.ToString(cb.EstadoCivilID);

                if (cb.DataCasamento != DateTime.MinValue)
                    txtDepDataCasamento.Text = cb.DataCasamento.ToString("dd/MM/yyyy");

                if (cb.Data != DateTime.MinValue)
                    txtDepAdmissao.Text = cb.Data.ToString("dd/MM/yyyy");

                txtDepAltura.Text = cb.Altura.ToString("N2");
                txtDepPeso.Text = cb.Peso.ToString("N2");

                txtDependentePortabilidade.Text = cb.Portabilidade;
                cboCarenciaDependenteOperadora.Text = cb.CarenciaOperadora;
                txtCarenciaDependenteOperadoraID.Value = base.CToString(cb.CarenciaOperadoraID);
                txtCarenciaDependenteCodigo.Text = cb.CarenciaCodigo;

                if (cb.CarenciaContratoDe != DateTime.MinValue)
                    txtDepTempoContratoDe.Text = cb.CarenciaContratoDe.ToString("dd/MM/yyyy");
                else
                    txtDepTempoContratoDe.Text = "";

                if (cb.CarenciaContratoAte != DateTime.MinValue)
                    txtDepTempoContratoAte.Text = cb.CarenciaContratoAte.ToString("dd/MM/yyyy");
                else
                    txtDepTempoContratoAte.Text = "";

                txtCarenciaDependenteTempoContrato.Text = cb.CarenciaContratoTempo.ToString();
                txtCarenciaDependenteMatricula.Text = cb.CarenciaMatriculaNumero;

                Beneficiario benef = new Beneficiario(cb.BeneficiarioID);
                benef.Carregar();

                txtCPFDependente.Text = benef.CPF;

                if (benef.DataNascimento != DateTime.MinValue)
                    txtDataNascimentoDependente.Text = benef.DataNascimento.ToString("dd/MM/yyyy");

                cboSexoDependente.SelectedValue = benef.Sexo;

                gridDependentes.SelectedIndex = Convert.ToInt32(e.CommandArgument);
            }
            if (e.CommandName.Equals("editar"))
            {
            }
            else if (e.CommandName.Equals("excluir"))
            {
                Object id = gridDependentes.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                IList<ContratoBeneficiario> lista = this.Dependentes;
                lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                this.Dependentes = lista;
                gridDependentes.DataSource = lista;
                gridDependentes.DataBind();

                if (id != null)
                {
                    //TODO: se ja incluido, inativa. se nao incluido, pode deletar
                    ContratoBeneficiario item = new ContratoBeneficiario();
                    item.ID = id;
                    item.Carregar();
                    item.Ativo = false;
                    item.DataInativacao = DateTime.Now;
                    item.Salvar();
                    this.CarregaDependentes();
                }

                MontaCombosDeBeneficiarios();
            }
        }

        protected void gridDependentes_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            UIHelper.AuthCtrl(e.Row.Cells[6], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            base.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente remover o dependente deste contrato?");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[4].Text = base.
                    TraduzTipoRelacaoDependenteContrato(Convert.ToInt32(e.Row.Cells[4].Text));

                Object id = gridDependentes.DataKeys[e.Row.RowIndex][1];
                e.Row.Cells[7].Attributes.Add("onClick", "win = window.open('beneficiarioP.aspx?et=2&" + IDKey + "=" + id + "', 'janela', 'toolbar=no,scrollbars=1,width=860,height=420'); win.moveTo(100,150); return false;");
            }
        }

        #endregion

        #region ficha de saúde

        protected void dlFicha_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            if (e.CommandName.Equals("salvar"))
            {
                Int32 index = Convert.ToInt32(e.CommandArgument);
                Object id = dlFicha.DataKeys[e.Item.ItemIndex];

                IList<ItemDeclaracaoSaudeINSTANCIA> itens =
                    (IList<ItemDeclaracaoSaudeINSTANCIA>)ViewState["fic_" + cboBeneficiarioFicha.SelectedValue];

                if (itens == null) { itens = new List<ItemDeclaracaoSaudeINSTANCIA>(); }

                for (int i = 0; i < dlFicha.Items.Count; i++)
                {
                    Object itemDeclaracaoId = ((Literal)e.Item.FindControl("litItemDeclaracaoID")).Text;

                    ItemDeclaracaoSaudeINSTANCIA item = PegaItemNaColecao(itens,
                        itemDeclaracaoId, Server.HtmlDecode(((Label)e.Item.FindControl("lblQuesta")).Text));

                    Boolean adiciona = false;
                    if (item == null)
                    {
                        item = new ItemDeclaracaoSaudeINSTANCIA();
                        adiciona = true;
                    }

                    item.ItemDeclaracaoID = itemDeclaracaoId;
                    item.BeneficiarioID = cboBeneficiarioFicha.SelectedValue;
                    item.ItemDeclaracaoTexto = Server.HtmlDecode(((Label)e.Item.FindControl("lblQuesta")).Text);

                    DateTime dte;
                    if (!UIHelper.TyParseToDateTime(((TextBox)e.Item.FindControl("txtFichaSaudeData")).Text, out dte))
                    {
                        ((Literal)e.Item.FindControl("litFichaAviso")).Text = "<br><font color='red' face='1'>Data inválida.</font>";
                        ((TextBox)e.Item.FindControl("txtFichaSaudeData")).Focus();
                        return;
                    }

                    if (((TextBox)e.Item.FindControl("txtFichaSaudeDescricao")).Text.Trim() == "")
                    {
                        ((Literal)e.Item.FindControl("litFichaAviso")).Text = "<br><font color='red' face='1'>Informe uma descrição.</font>";
                        ((TextBox)e.Item.FindControl("txtFichaSaudeDescricao")).Focus();
                        return;
                    }

                    item.Data = dte;
                    item.Descricao = ((TextBox)e.Item.FindControl("txtFichaSaudeDescricao")).Text;
                    item.CIDInicial = ((TextBox)e.Item.FindControl("txtCIDInicial")).Text;
                    item.CIDFinal = ((TextBox)e.Item.FindControl("txtCIDFinal")).Text;
                    item.ObsMedico = ((TextBox)e.Item.FindControl("txtOBSMedico")).Text;
                    item.AprovadoPeloMedico = ((CheckBox)e.Item.FindControl("chkAprovado")).Checked;

                    if (i == index)
                        item.Sim = true;

                    item.Salvar();

                    if (adiciona)
                        itens.Add(item);

                    ((Literal)e.Item.FindControl("litFichaAviso")).Text = "<br><font color='blue' face='1'>Informação salva.</font>";
                }

                ViewState["fic_" + cboBeneficiarioFicha.SelectedValue] = itens;
            }
        }

        protected void dlFicha_ItemDataBound(Object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                UIHelper.AuthWebCtrl((CheckBox)e.Item.FindControl("chkFSim"), Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                if (((CheckBox)e.Item.FindControl("chkFSim")).Checked)
                {
                    ((HtmlTableRow)e.Item.FindControl("tr1Ficha")).Visible = true;
                    ((HtmlTableRow)e.Item.FindControl("tr2Ficha")).Visible = true;
                    ((HtmlTableRow)e.Item.FindControl("tr3Ficha")).Visible = true;
                    ((HtmlTableRow)e.Item.FindControl("tr4Ficha")).Visible = true;
                    ((HtmlTableRow)e.Item.FindControl("tr5Ficha")).Visible = true;
                }
            }
        }

        void MontaCombosDeBeneficiarios()
        {
            IList<Beneficiario> lista = null;
            if (ViewState["list_benefs"] == null)
            {
                lista = new List<Beneficiario>();

                if (this.TitularID != null)
                {
                    Beneficiario titular = new Beneficiario();
                    titular.ID = Convert.ToString(this.TitularID);
                    titular.Nome = txtNome.Text;
                    lista.Add(titular);
                }

                if (this.Dependentes != null)
                {
                    foreach (ContratoBeneficiario _dependente in this.Dependentes)
                    {
                        Beneficiario dependente = new Beneficiario();
                        dependente.ID = _dependente.BeneficiarioID;
                        dependente.Nome = _dependente.BeneficiarioNome;
                        lista.Add(dependente);
                    }
                }

                ViewState["list_benefs"] = lista;
            }
            else
            {
                lista = ViewState["list_benefs"] as List<Beneficiario>;
            }

            cboBeneficiarioFicha.DataValueField = "ID";
            cboBeneficiarioFicha.DataTextField = "Nome";
            cboBeneficiarioFicha.DataSource = lista;
            cboBeneficiarioFicha.DataBind();

            this.CarregaFichaDeSaudeDeTodosBeneficiarios();

            //cboBeneficiarioAdicional.DataValueField = "ID";
            //cboBeneficiarioAdicional.DataTextField = "Nome";
            //cboBeneficiarioAdicional.DataSource = lista;
            //cboBeneficiarioAdicional.DataBind();
            //cboBeneficiarioAdicional_SelectedIndexChanged(null, null);

            this.ExibeSumario();
        }

        void CarregaFichaDeSaudeDeTodosBeneficiarios()
        {
            foreach (ListItem beneficiario in cboBeneficiarioFicha.Items)
            {
                IList<ItemDeclaracaoSaudeINSTANCIA> lista = ItemDeclaracaoSaudeINSTANCIA.
                    Carregar(beneficiario.Value, cboOperadora.SelectedValue);

                if (lista != null)
                {
                    foreach (ItemDeclaracaoSaudeINSTANCIA item in lista)
                        item.BeneficiarioID = beneficiario.Value;

                    ViewState["fic_" + beneficiario.Value] = lista;

                    if (beneficiario.Value == cboBeneficiarioFicha.SelectedValue)
                        dlFicha.DataSource = lista;
                }
            }

            dlFicha.DataBind();
        }

        void CarregaFichaDeSaude(Boolean atualizar)
        {
            if (cboBeneficiarioFicha.SelectedValue == "" || !base.HaItemSelecionado(cboOperadora)) { return; }

            if (atualizar)
            {
                foreach (ListItem item in cboBeneficiarioFicha.Items)
                    ViewState["fic_" + item.Value] = null;
            }

            if (ViewState["fic_" + cboBeneficiarioFicha.SelectedValue] == null || atualizar)
            {
                IList<ItemDeclaracaoSaudeINSTANCIA> lista = ItemDeclaracaoSaudeINSTANCIA.
                    Carregar(cboBeneficiarioFicha.SelectedValue, cboOperadora.SelectedValue);

                if (lista != null)
                {
                    foreach (ItemDeclaracaoSaudeINSTANCIA item in lista)
                        item.BeneficiarioID = cboBeneficiarioFicha.SelectedValue;

                    dlFicha.DataSource = lista;
                    ViewState["fic_" + cboBeneficiarioFicha.SelectedValue] = lista;
                }
            }
            else
            {
                dlFicha.DataSource = ViewState["fic_" + cboBeneficiarioFicha.SelectedValue];
            }

            dlFicha.DataBind();
        }

        protected void cboBeneficiarioFicha_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaFichaDeSaude(false);
        }

        protected void checkboxSkin_Changed2(Object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;

            DataListItem grow = (DataListItem)check.NamingContainer;
            ((HtmlTableRow)grow.FindControl("tr1Ficha")).Visible = check.Checked;
            ((HtmlTableRow)grow.FindControl("tr2Ficha")).Visible = check.Checked;
            ((HtmlTableRow)grow.FindControl("tr3Ficha")).Visible = check.Checked;
            ((HtmlTableRow)grow.FindControl("tr4Ficha")).Visible = check.Checked;
            ((HtmlTableRow)grow.FindControl("tr5Ficha")).Visible = check.Checked;

            if (check.Checked)
            {
                ((TextBox)grow.FindControl("txtFichaSaudeData")).Focus();
            }
            else
            {
                ((TextBox)grow.FindControl("txtFichaSaudeData")).Text = "";
                ((TextBox)grow.FindControl("txtFichaSaudeDescricao")).Text = "";
                ((TextBox)grow.FindControl("txtCIDInicial")).Text = "";
                ((TextBox)grow.FindControl("txtCIDFinal")).Text = "";
                ((TextBox)grow.FindControl("txtOBSMedico")).Text = "";
                ((CheckBox)grow.FindControl("chkAprovado")).Checked = false;

                IList<ItemDeclaracaoSaudeINSTANCIA> itens =
                    (IList<ItemDeclaracaoSaudeINSTANCIA>)ViewState["fic_" + cboBeneficiarioFicha.SelectedValue];

                itens[grow.ItemIndex].Data = DateTime.MinValue;
                itens[grow.ItemIndex].Descricao = "";
                itens[grow.ItemIndex].Sim = false;

                ViewState["fic_" + cboBeneficiarioFicha.SelectedValue] = itens;
            }
        }

        ItemDeclaracaoSaudeINSTANCIA PegaItemNaColecao(IList<ItemDeclaracaoSaudeINSTANCIA> itens, Object itemDeclaracaoId, String itemDeclaracaoTexto)
        {
            foreach (ItemDeclaracaoSaudeINSTANCIA _item in itens)
            {
                if (Convert.ToString(_item.ItemDeclaracaoID) == Convert.ToString(itemDeclaracaoId) ||
                    _item.ItemDeclaracaoTexto == itemDeclaracaoTexto)
                {
                    return _item;
                }
            }

            return null;
        }

        protected void gridFicha_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox check = (CheckBox)e.Row.Cells[1].Controls[1];
                check.ID += "_" + e.Row.RowIndex.ToString();
            }
        }

        protected void gridFicha_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
        }

        #endregion

        #region adicionais

        void SetaEstadoDosAdicionais()
        {
            //if (!base.HaItemSelecionado(cboPlano)) { return; }
            //foreach (ListItem item in cboBeneficiarioAdicional.Items)
            //{
            //    IList<AdicionalBeneficiario> lista = AdicionalBeneficiario.
            //        Carregar(cboContrato.SelectedValue, cboPlano.SelectedValue, ViewState[IDKey], item.Value);
            //    ViewState["adben_" + item.Value] = lista;
            //}

            //gridAdicional.DataSource = ViewState["adben_" + cboBeneficiarioAdicional.SelectedValue];
            //gridAdicional.DataBind();
        }

        void CarregaAdicionais(Boolean atualizar)
        {
            if (ViewState[IDKey] == null) return;

            if (atualizar || !IsPostBack)
            {
                ViewState["adben_"] = AdicionalBeneficiario.Carregar(ViewState[IDKey], -1, false, true);
            }

            IList<AdicionalBeneficiario> lista = (IList<AdicionalBeneficiario>)ViewState["adben_"];

            List<AdicionalBeneficiario> taxas = new List<AdicionalBeneficiario>();
            List<AdicionalBeneficiario> seguros = new List<AdicionalBeneficiario>();
            List<AdicionalBeneficiario> previdencias = new List<AdicionalBeneficiario>();

            if (lista != null)
            {
                foreach (var ad in lista)
                {
                    if (ad.AdicionalTipo == 0) taxas.Add(ad);
                    else if (ad.AdicionalTipo == 1) seguros.Add(ad);
                    else if (ad.AdicionalTipo == 2) previdencias.Add(ad);
                }
            }

            gridAdicional.DataSource = taxas;
            gridAdicional.DataBind();

            gridSeguro.DataSource = seguros;
            gridSeguro.DataBind();

            gridPrevidencia.DataSource = previdencias;
            gridPrevidencia.DataBind();

            #region comentado 2

            ////if (cboBeneficiarioAdicional.SelectedValue == "" || cboOperadora.SelectedValue == "" || cboOperadora.SelectedValue == "-1") { return; }

            //IList<AdicionalBeneficiario> lista =null;

            //if (atualizar || !IsPostBack)
            //{
            //    lista = AdicionalBeneficiario.Carregar(ViewState[IDKey], /*cboBeneficiarioAdicional.SelectedValue,*/ 0, true);
            //    ViewState["adben_"] = lista;
            //}

            //gridAdicional.DataSource = ViewState["adben_"];
            //gridAdicional.DataBind();
            ////upAdicionais.Update();

            //gridSeguro.DataSource = AdicionalBeneficiario.Carregar(ViewState[IDKey], /*cboBeneficiarioAdicional.SelectedValue,*/ 1);
            //gridSeguro.DataBind();

            #endregion

            #region comentado
            //if (cboBeneficiarioAdicional.SelectedValue == "" || cboOperadora.SelectedValue == "" || cboOperadora.SelectedValue == "-1") { return; }

            //if (atualizar)
            //{
            //    foreach (ListItem item in cboBeneficiarioAdicional.Items)
            //        ViewState["adben_" + item.Value] = null;
            //}

            //if (ViewState["adben_" + cboBeneficiarioAdicional.SelectedValue] == null || atualizar)
            //{
            //    foreach (ListItem item in cboBeneficiarioAdicional.Items)
            //    {
            //        if (cboPlano.SelectedIndex > 0)
            //        {
            //            IList<AdicionalBeneficiario> lista = AdicionalBeneficiario.
            //                Carregar(cboContrato.SelectedValue, cboPlano.SelectedValue, ViewState[IDKey], item.Value);

            //            if (item.Value == cboBeneficiarioAdicional.SelectedValue)
            //            {
            //                gridAdicional.DataSource = lista;
            //            }
            //            ViewState["adben_" + item.Value] = lista;
            //        }
            //    }
            //}
            //else
            //{
            //    gridAdicional.DataSource = ViewState["adben_" + cboBeneficiarioAdicional.SelectedValue];
            //}

            //gridAdicional.DataBind();
            #endregion
        }

        protected void lnkAdicionarAdicional_click(object sender, EventArgs e)
        {
            if (ViewState[IDKey] == null || Convert.ToString(ViewState[IDKey]).Trim() == "")
            {
                base.Alerta(null, this, "_valid", "É necessário salvar a proposta para prosseguir.");
                return;
            }

            AdicionalBeneficiario ab = new AdicionalBeneficiario();
            ab.AdicionalID = cboAdicionaisParaAdicionar.SelectedValue;
            ab.PropostaID = ViewState[IDKey];
            ab.BeneficiarioID = this.TitularID;
            ab.FormaPagto = 11;
            ab.ST_SG_1 = "A";
            ab.Status01 = "A";
            ab.Status = "A";
            ab.Salvar();
            this.CarregaAdicionais(true);
            upAdicionais.Update();
        }

        //protected void cboBeneficiarioAdicional_SelectedIndexChanged(Object sender, EventArgs e)
        //{
        //    this.CarregaAdicionais(false);
        //}

        protected void gridPrevidencia_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("gravar"))
            {
                int indice = Convert.ToInt32(e.CommandArgument);
                Object id = gridPrevidencia.DataKeys[Convert.ToInt32(e.CommandArgument)][1];
                AdicionalBeneficiario ab = new AdicionalBeneficiario();
                ab.ID = id;
                ab.Carregar();

                ab.Valor01 = Convert.ToDecimal(((TextBox)gridPrevidencia.Rows[indice].Cells[1].Controls[1]).Text);
                ab.FormaPagto = Convert.ToInt32(((DropDownList)gridPrevidencia.Rows[indice].Cells[2].Controls[1]).SelectedValue);

                ab.Recorrente = ((CheckBox)gridPrevidencia.Rows[indice].Cells[3].Controls[1]).Checked;

                ab.Salvar();
                this.ExibeSumario();
                this.calculaValorCobranca_Boleto();
                base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
            }
            else if (e.CommandName.Equals("excluir"))
            {
                int indice = Convert.ToInt32(e.CommandArgument);
                Object id = gridPrevidencia.DataKeys[Convert.ToInt32(e.CommandArgument)][1];
                AdicionalBeneficiario ab = new AdicionalBeneficiario();
                ab.ID = id;
                ab.Carregar();

                ab.ST_SG_1 = "B";
                ab.ST_SG_2 = "B";
                ab.ST_SG_3 = "B";
                ab.ST_SG_4 = "B";
                ab.ST_SG_5 = "B";
                ab.ST_SG_6 = "B";
                ab.Status = "B";
                ab.Status01 = "B";

                ab.Salvar();
                this.CarregaAdicionais(true);
                this.ExibeSumario();
                this.calculaValorCobranca_Boleto();
                base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
            }
        }

        protected void gridPrevidencia_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((TextBox)e.Row.Cells[1].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[1].Controls[1]).ClientID + "')");
                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente gravar os dados?");
                base.grid_RowDataBound_Confirmacao(sender, e, 5, "Deseja realmente cancelar o adicional?");
            }
        }

        protected void gridAdicional_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("gravar"))
            {
                int indice = Convert.ToInt32(e.CommandArgument);
                Object id = gridAdicional.DataKeys[Convert.ToInt32(e.CommandArgument)][1];
                AdicionalBeneficiario ab = new AdicionalBeneficiario();
                ab.ID = id;
                ab.Carregar();

                ab.Valor01 = Convert.ToDecimal(((TextBox)gridAdicional.Rows[indice].Cells[1].Controls[1]).Text);
                ab.FormaPagto = Convert.ToInt32(((DropDownList)gridAdicional.Rows[indice].Cells[2].Controls[1]).SelectedValue);
                ab.Recorrente = ((CheckBox)gridAdicional.Rows[indice].Cells[3].Controls[1]).Checked;

                ab.Salvar();
                this.ExibeSumario();
                this.calculaValorCobranca_Boleto();
                base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
            }
            else if (e.CommandName.Equals("excluir"))
            {
                int indice = Convert.ToInt32(e.CommandArgument);
                Object id = gridAdicional.DataKeys[Convert.ToInt32(e.CommandArgument)][1];
                AdicionalBeneficiario ab = new AdicionalBeneficiario();
                ab.ID = id;
                ab.Carregar();

                Adicional a = new Adicional(ab.AdicionalID);
                a.Carregar();

                if (a.Tipo != 3)
                {
                    ab.ST_SG_1 = "B";
                    ab.ST_SG_2 = "B";
                    ab.ST_SG_3 = "B";
                    ab.ST_SG_4 = "B";
                    ab.ST_SG_5 = "B";
                    ab.ST_SG_6 = "B";
                    ab.Status = "B";
                    ab.Status01 = "B";

                    ab.Salvar();
                }
                else
                {
                    ab.Remover();
                }

                this.CarregaAdicionais(true);
                this.ExibeSumario();
                this.calculaValorCobranca_Boleto();
                base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
            }
        }

        protected void gridAdicional_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //UIHelper.AuthWebCtrl((CheckBox)e.Row.Cells[1].Controls[1], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                ((TextBox)e.Row.Cells[1].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[1].Controls[1]).ClientID + "')");
                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente gravar os dados?");
                base.grid_RowDataBound_Confirmacao(sender, e, 5, "Deseja realmente cancelar o adicional?");
            }
        }

        protected void gridSeguro_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("gravar"))
            {
                int indice = Convert.ToInt32(e.CommandArgument);
                Object id = gridSeguro.DataKeys[Convert.ToInt32(e.CommandArgument)][1];
                AdicionalBeneficiario ab = new AdicionalBeneficiario();
                ab.ID = id;
                ab.Carregar();

                ab.PRE_COB_1 = Convert.ToDecimal(((TextBox)gridSeguro.Rows[indice].Cells[1].Controls[1]).Text);
                ab.PRE_COB_2 = Convert.ToDecimal(((TextBox)gridSeguro.Rows[indice].Cells[2].Controls[1]).Text);
                ab.PRE_COB_3 = Convert.ToDecimal(((TextBox)gridSeguro.Rows[indice].Cells[3].Controls[1]).Text);
                ab.PRE_COB_4 = Convert.ToDecimal(((TextBox)gridSeguro.Rows[indice].Cells[4].Controls[1]).Text);
                ab.PRE_COB_5 = Convert.ToDecimal(((TextBox)gridSeguro.Rows[indice].Cells[5].Controls[1]).Text);
                ab.PRE_COB_6 = Convert.ToDecimal(((TextBox)gridSeguro.Rows[indice].Cells[6].Controls[1]).Text);

                ab.FormaPagto = Convert.ToInt32(((DropDownList)gridSeguro.Rows[indice].Cells[7].Controls[1]).SelectedValue);

                ab.Recorrente = ((CheckBox)gridSeguro.Rows[indice].Cells[8].Controls[1]).Checked;

                ab.Salvar();
                this.ExibeSumario();
                this.calculaValorCobranca_Boleto();
                base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
            }
            else if (e.CommandName.Equals("excluir"))
            {
                int indice = Convert.ToInt32(e.CommandArgument);
                Object id = gridSeguro.DataKeys[Convert.ToInt32(e.CommandArgument)][1];
                AdicionalBeneficiario ab = new AdicionalBeneficiario();
                ab.ID = id;
                ab.Carregar();

                ab.ST_SG_1 = "B";
                ab.ST_SG_2 = "B";
                ab.ST_SG_3 = "B";
                ab.ST_SG_4 = "B";
                ab.ST_SG_5 = "B";
                ab.ST_SG_6 = "B";
                ab.Status = "B";
                ab.Status01 = "B";

                ab.Salvar();
                this.CarregaAdicionais(true);
                this.ExibeSumario();
                this.calculaValorCobranca_Boleto();
                base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
            }
            else if (e.CommandName.Equals("detalheSeguro"))
            {
                MPEDetalheSeguro.Show();

                Object id = gridSeguro.DataKeys[Convert.ToInt32(e.CommandArgument)][1];

                AdicionalBeneficiario ab = new AdicionalBeneficiario();
                ab.ID = id;
                ab.Carregar();

                Adicional a = new Adicional(ab.AdicionalID);
                a.Carregar();

                litDetalheSeguroTitulo.Text = string.Concat("Detalhes de: ", a.Descricao);

                cboPremioDetalhe.Items.Clear();

                if (ab.PRE_COB_1 > 0) cboPremioDetalhe.Items.Add(new ListItem("Prêmio 1", "1"));
                if (ab.PRE_COB_2 > 0) cboPremioDetalhe.Items.Add(new ListItem("Prêmio 2", "2"));
                if (ab.PRE_COB_3 > 0) cboPremioDetalhe.Items.Add(new ListItem("Prêmio 3", "3"));
                if (ab.PRE_COB_4 > 0) cboPremioDetalhe.Items.Add(new ListItem("Prêmio 4", "4"));
                if (ab.PRE_COB_5 > 0) cboPremioDetalhe.Items.Add(new ListItem("Prêmio 5", "5"));
                if (ab.PRE_COB_6 > 0) cboPremioDetalhe.Items.Add(new ListItem("Prêmio 6", "6"));

                //txtDetalheSeguro_Premio.Attributes.Add("onKeyUp", "mascara('" + txtDetalheSeguro_Premio.ClientID + "')");
                //txtDetalheSeguro_Cobertura.Attributes.Add("onKeyUp", "mascara('" + txtDetalheSeguro_Premio.ClientID + "')");

                ViewState["seguroid"] = Convert.ToInt64(ab.ID);
                this.exibirDetalheDoPremio(ab);
                //upDetalheSeguro.Update();
            }
        }

        protected void gridSeguro_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((TextBox)e.Row.Cells[1].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[1].Controls[1]).ClientID + "')");
                ((TextBox)e.Row.Cells[2].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[2].Controls[1]).ClientID + "')");
                ((TextBox)e.Row.Cells[3].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[3].Controls[1]).ClientID + "')");
                ((TextBox)e.Row.Cells[4].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[4].Controls[1]).ClientID + "')");
                ((TextBox)e.Row.Cells[5].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[5].Controls[1]).ClientID + "')");
                ((TextBox)e.Row.Cells[6].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[6].Controls[1]).ClientID + "')");

                base.grid_RowDataBound_Confirmacao(sender, e, 9, "Deseja realmente gravar os dados?");
                base.grid_RowDataBound_Confirmacao(sender, e, 10, "Deseja realmente cancelar o adicional?");
            }
        }

        protected void cboPremioDetalhe_change(object sender, EventArgs e)
        {
            AdicionalBeneficiario ab = new AdicionalBeneficiario();
            ab.ID = ViewState["seguroid"];
            ab.Carregar();
            this.exibirDetalheDoPremio(ab);
            MPEDetalheSeguro.Show();
            //upDetalheSeguro.Update();
        }

        void exibirDetalheDoPremio(AdicionalBeneficiario ab)
        {
            #region if
            if (cboPremioDetalhe.SelectedValue == "1")
            {
                txtDetalheSeguro_Premio.Text = ab.PRE_COB_1.ToString("N2");
                txtDetalheSeguro_Cobertura.Text = "0,00";
                txtDetalheSeguro_DataInicio.Text = ab.DT_SG_1.ToString("dd/MM/yyyy");
                cboDetalheSeguro_Status.SelectedValue = ab.ST_SG_1;
                cboDetalheSeguro_Pagto.SelectedValue = ab.FormaPagto.ToString();
            }
            if (cboPremioDetalhe.SelectedValue == "2")
            {
                txtDetalheSeguro_Premio.Text = ab.PRE_COB_2.ToString("N2");
                txtDetalheSeguro_Cobertura.Text = "0,00";
                txtDetalheSeguro_DataInicio.Text = ab.DT_SG_2.ToString("dd/MM/yyyy");
                cboDetalheSeguro_Status.SelectedValue = ab.ST_SG_2;
                cboDetalheSeguro_Pagto.SelectedValue = ab.FormaPagto.ToString();
            }
            if (cboPremioDetalhe.SelectedValue == "3")
            {
                txtDetalheSeguro_Premio.Text = ab.PRE_COB_3.ToString("N2");
                txtDetalheSeguro_Cobertura.Text = "0,00";
                txtDetalheSeguro_DataInicio.Text = ab.DT_SG_3.ToString("dd/MM/yyyy");
                cboDetalheSeguro_Status.SelectedValue = ab.ST_SG_3;
                cboDetalheSeguro_Pagto.SelectedValue = ab.FormaPagto.ToString();
            }
            if (cboPremioDetalhe.SelectedValue == "4")
            {
                txtDetalheSeguro_Premio.Text = ab.PRE_COB_4.ToString("N2");
                txtDetalheSeguro_Cobertura.Text = "0,00";
                txtDetalheSeguro_DataInicio.Text = ab.DT_SG_4.ToString("dd/MM/yyyy");
                cboDetalheSeguro_Status.SelectedValue = ab.ST_SG_4;
                cboDetalheSeguro_Pagto.SelectedValue = ab.FormaPagto.ToString();
            }
            if (cboPremioDetalhe.SelectedValue == "5")
            {
                txtDetalheSeguro_Premio.Text = ab.PRE_COB_5.ToString("N2");
                txtDetalheSeguro_Cobertura.Text = "0,00";
                txtDetalheSeguro_DataInicio.Text = ab.DT_SG_5.ToString("dd/MM/yyyy");
                cboDetalheSeguro_Status.SelectedValue = ab.ST_SG_5;
                cboDetalheSeguro_Pagto.SelectedValue = ab.FormaPagto.ToString();
            }
            if (cboPremioDetalhe.SelectedValue == "6")
            {
                txtDetalheSeguro_Premio.Text = ab.PRE_COB_6.ToString("N2");
                txtDetalheSeguro_Cobertura.Text = "0,00";
                txtDetalheSeguro_DataInicio.Text = ab.DT_SG_6.ToString("dd/MM/yyyy");
                cboDetalheSeguro_Status.SelectedValue = ab.ST_SG_6;
                cboDetalheSeguro_Pagto.SelectedValue = ab.FormaPagto.ToString();
            }

            if (txtDetalheSeguro_DataInicio.Text == "01/01/0001") txtDetalheSeguro_DataInicio.Text = "";
            #endregion
        }




        protected void cmdFecharDetalheSeguro_Click(Object sender, EventArgs e)
        {
            ViewState["seguroid"] = null;
            ViewState.Remove("seguroid");
            litDetalheSeguroMsg.Text = "";
            MPEDetalheSeguro.Hide();
        }

        protected void cmdSalvarDetalheSeguro_Click(Object sender, EventArgs e)
        {
            if (cboPremioDetalhe.Items.Count == 0)
            {
                MPEDetalheSeguro.Show();
                litDetalheSeguroMsg.Text = "Nenhumm prêmio disponível.";
                return;
            }

            DateTime data = new DateTime();
            if (!UIHelper.TyParseToDateTime(txtDetalheSeguro_DataInicio.Text, out data))
            {
                MPEDetalheSeguro.Show();
                litDetalheSeguroMsg.Text = "Data de início inválida.";
                txtDetalheSeguro_DataInicio.Focus();
                return;
            }

            AdicionalBeneficiario ab = new AdicionalBeneficiario();
            ab.ID = ViewState["seguroid"];
            ab.Carregar();

            #region if
            if (cboPremioDetalhe.SelectedValue == "1")
            {
                ab.PRE_COB_1 = Convert.ToDecimal(txtDetalheSeguro_Premio.Text);
                //txtDetalheSeguro_Cobertura.Text = "0,00";
                ab.DT_SG_1 = data;
                ab.ST_SG_1 = cboDetalheSeguro_Status.SelectedValue;
            }
            if (cboPremioDetalhe.SelectedValue == "2")
            {
                ab.PRE_COB_2 = Convert.ToDecimal(txtDetalheSeguro_Premio.Text);
                //txtDetalheSeguro_Cobertura.Text = "0,00";
                ab.DT_SG_2 = data;
                ab.ST_SG_2 = cboDetalheSeguro_Status.SelectedValue;
            }
            if (cboPremioDetalhe.SelectedValue == "3")
            {
                ab.PRE_COB_3 = Convert.ToDecimal(txtDetalheSeguro_Premio.Text);
                //txtDetalheSeguro_Cobertura.Text = "0,00";
                ab.DT_SG_3 = data;
                ab.ST_SG_3 = cboDetalheSeguro_Status.SelectedValue;
            }
            if (cboPremioDetalhe.SelectedValue == "4")
            {
                ab.PRE_COB_4 = Convert.ToDecimal(txtDetalheSeguro_Premio.Text);
                //txtDetalheSeguro_Cobertura.Text = "0,00";
                ab.DT_SG_4 = data;
                ab.ST_SG_4 = cboDetalheSeguro_Status.SelectedValue;
            }
            if (cboPremioDetalhe.SelectedValue == "5")
            {
                ab.PRE_COB_5 = Convert.ToDecimal(txtDetalheSeguro_Premio.Text);
                //txtDetalheSeguro_Cobertura.Text = "0,00";
                ab.DT_SG_5 = data;
                ab.ST_SG_5 = cboDetalheSeguro_Status.SelectedValue;
            }
            if (cboPremioDetalhe.SelectedValue == "6")
            {
                ab.PRE_COB_6 = Convert.ToDecimal(txtDetalheSeguro_Premio.Text);
                //txtDetalheSeguro_Cobertura.Text = "0,00";
                ab.DT_SG_6 = data;
                ab.ST_SG_6 = cboDetalheSeguro_Status.SelectedValue;
            }
            #endregion

            ab.Salvar();
            litDetalheSeguroMsg.Text = "Dados salvos";
            MPEDetalheSeguro.Show();
            //upDetalheSeguro.Update();

            //ViewState["seguroid"] = null;
            //ViewState.Remove("seguroid");
            //litDetalheSeguroMsg.Text = "";
            //MPEDetalheSeguro.Hide();
        }

        AdicionalBeneficiario PegaNaColecao(IList<AdicionalBeneficiario> itens, Object adicionalId, Object beneficiarioId)
        {
            foreach (AdicionalBeneficiario _item in itens)
            {
                if (Convert.ToString(_item.AdicionalID) == Convert.ToString(adicionalId))
                {
                    return _item;
                }
            }

            return null;
        }

        protected void checkboxGridAdicional_Changed(Object sender, EventArgs e)
        {
            return;
            CheckBox check = (CheckBox)sender;

            GridViewRow grow = (GridViewRow)check.NamingContainer;
            int index = grow.RowIndex;

            Adicional ad = new Adicional();
            ad.ID = gridAdicional.DataKeys[index].Value;
            ad.Carregar();

            if (check.Checked && !ad.Ativo)
            {
                check.Checked = false;
                base.Alerta(null, this, "_err", "Adicional inativo no momento.");
                return;
            }

            IList<AdicionalBeneficiario> itens = null;
            //(IList<AdicionalBeneficiario>)ViewState["adben_" + cboBeneficiarioAdicional.SelectedValue];

            if (itens == null) { itens = new List<AdicionalBeneficiario>(); }

            if (check.Checked) //adiciona na colecao
            {
                for (int i = 0; i < gridAdicional.Rows.Count; i++)
                {
                    Object atualAdicionalId = null;
                    Object atualBeneficiarioId = null;

                    if (gridAdicional.DataKeys[0].Values.Count > 0)
                    {
                        atualAdicionalId = gridAdicional.DataKeys[i][0];
                        //atualBeneficiarioId = cboBeneficiarioAdicional.SelectedValue;
                    }

                    AdicionalBeneficiario item = PegaNaColecao(itens, atualAdicionalId, atualBeneficiarioId);

                    Boolean adiciona = false;
                    if (item == null)
                    {
                        item = new AdicionalBeneficiario();
                        adiciona = true;
                    }

                    item.AdicionalID = atualAdicionalId;
                    item.PropostaID = ViewState[IDKey];

                    //if (i == index)
                    //    item.BeneficiarioID = cboBeneficiarioAdicional.SelectedValue;

                    if (adiciona)
                        itens.Add(item);
                }

                //checa se ha alguma regra

                if (ad.ParaTodaProposta)// (ar != null && ar.Tipo == Convert.ToInt32(AdicionalRegra.eTipo.TitularETodosDependentes))
                {
                    //foreach (ListItem _item in cboBeneficiarioAdicional.Items)
                    //{
                    //    IList<AdicionalBeneficiario> _itens =
                    //        (IList<AdicionalBeneficiario>)ViewState["adben_" + _item.Value];
                    //    if (_itens == null)
                    //    {
                    //        _itens = AdicionalBeneficiario.Carregar(cboContrato.SelectedValue, cboPlano.SelectedValue, ViewState[IDKey], _item.Value);
                    //    }

                    //    foreach (AdicionalBeneficiario aben in _itens)
                    //    {
                    //        if (Convert.ToString(ad.ID) == Convert.ToString(aben.AdicionalID))
                    //        {
                    //            aben.AdicionalID = ad.ID;
                    //            aben.BeneficiarioID = _item.Value;
                    //            aben.PropostaID = ViewState[IDKey];
                    //            break;
                    //        }
                    //    }

                    //    ViewState["adben_" + _item.Value] = _itens;
                    //}
                }
            }
            else //remove da colecao
            {
                //checa se ha alguma regra
                if (ad.ParaTodaProposta) // (ar != null && ar.Tipo == Convert.ToInt32(AdicionalRegra.eTipo.TitularETodosDependentes))
                {
                    //foreach (ListItem _item in cboBeneficiarioAdicional.Items)
                    //{
                    //    IList<AdicionalBeneficiario> _itens =
                    //        (IList<AdicionalBeneficiario>)ViewState["adben_" + _item.Value];

                    //    foreach (AdicionalBeneficiario aben in _itens)
                    //    {
                    //        if (Convert.ToString(ad.ID) == Convert.ToString(aben.AdicionalID))
                    //        {
                    //            aben.BeneficiarioID = null;

                    //            if (aben.ID != null) { aben.Remover(); }
                    //            break;
                    //        }
                    //    }

                    //    ViewState["adben_" + _item.Value] = _itens;
                    //}
                }
                else
                {
                    if (itens != null)
                    {
                        itens[index].BeneficiarioID = null;

                        if (itens[index].ID != null) { itens[index].Remover(); }
                    }
                }
            }

            //ViewState["adben_" + cboBeneficiarioAdicional.SelectedValue] = itens;
            this.ExibeSumario();
        }

        #endregion

        #region finalização

        protected void txtDesconto_Changed(Object sender, EventArgs e)
        {
            Decimal result = 0;
            if (!Decimal.TryParse(txtDesconto.Text, out result))
                txtDesconto.Text = "0";
            this.ExibeSumario();
        }

        void ExibeSumario()
        {
            if (ViewState[IDKey] == null) return;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Decimal total = 0;

            Contrato contrato = new Contrato(this.contratoId);
            if (this.contratoId != null) { contrato.Carregar(); }

            //checa se tem taxa associativa para a proposta inteira (e se deve cobrar)
            if (base.HaItemSelecionado(cboEstipulante) && chkCobrarTaxa.Visible && chkCobrarTaxa.Checked)
            {
                //checa se há taxa associativa

                EstipulanteTaxa taxa = null;
                if (this.contratoId == null)
                    taxa = EstipulanteTaxa.CarregarVigente(cboEstipulante.SelectedValue);
                else
                    taxa = EstipulanteTaxa.CarregarVigente(cboEstipulante.SelectedValue, contrato.Admissao);

                if (taxa != null && taxa.Valor > 0 && ((EstipulanteTaxa.eTipoTaxa)taxa.TipoTaxa) == EstipulanteTaxa.eTipoTaxa.PorProposta)
                {
                    sb.Append("<table cellpadding='2' cellspacing='0' border='0' width='100%'>");
                    sb.AppendLine("<tr><td class='tdPrincipal1'>TAXA ASSOCIATIVA: ");
                    sb.AppendLine(taxa.Valor.ToString("C"));
                    sb.AppendLine("</td></tr>");
                    sb.AppendLine("</table>");
                    total += taxa.Valor;
                    ValorTaxaAssociativaContrato = taxa.Valor;
                }
            }

            IList<AdicionalBeneficiario> taxas = AdicionalBeneficiario.Carregar(ViewState[IDKey]);

            if (taxas != null)
            {
                if (sb.Length > 0) { sb.AppendLine("<br>"); }
                sb.AppendLine("<table cellpadding='2' cellspacing='0' border='0' width='100%'>");

                IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID(ViewState[IDKey], true, false);
                Beneficiario beneficiario = new Beneficiario();
                beneficiario.ID = beneficiarios[0].BeneficiarioID;
                beneficiario.Carregar();

                int idade = Convert.ToInt32(base.DateDiff(2, beneficiario.DataNascimento));

                sb.AppendLine("<tr><td class='tdPrincipal1' colspan='2'>");
                sb.Append(beneficiario.Nome);
                sb.Append("</td></tr>");

                //IDADE
                sb.AppendLine("<tr class='tdNormal1'><td width='25%'>Idade</td><td>");
                sb.Append(idade);
                sb.Append("</td></tr>");

                Decimal valor = 0, valorBeneficiario = 0;

                foreach (AdicionalBeneficiario ab in taxas)
                {
                    if (ab.AdicionalTipo == 0 && ab.Status01 == "A") //taxa
                    {
                        valor += ab.Valor01;
                    }
                    else if (ab.AdicionalTipo == 2 && ab.Status01 == "A") //previdencia
                    {
                        valor += ab.Valor01;
                    }
                    else if (ab.AdicionalTipo == 1) //seguro
                    {
                        if (ab.PRE_COB_1 > 0 && ab.ST_SG_1 == "A") valor += ab.PRE_COB_1;
                        if (ab.PRE_COB_2 > 0 && ab.ST_SG_1 == "A") valor += ab.PRE_COB_2;
                        if (ab.PRE_COB_3 > 0 && ab.ST_SG_1 == "A") valor += ab.PRE_COB_3;
                        if (ab.PRE_COB_4 > 0 && ab.ST_SG_1 == "A") valor += ab.PRE_COB_4;
                        if (ab.PRE_COB_5 > 0 && ab.ST_SG_1 == "A") valor += ab.PRE_COB_5;
                        if (ab.PRE_COB_6 > 0 && ab.ST_SG_1 == "A") valor += ab.PRE_COB_6;
                    }
                }

                total += valor;
                sb.AppendLine("<tr class='tdNormal1'><td>Valor</td><td>");
                sb.Append(valor.ToString("C"));
                sb.Append("</td></tr>");

                #region comentado
                //    



                //    
                //    if (base.HaItemSelecionado(cboPlano))
                //    {
                //        //VALOR DO PLANO
                //        if (cboAcomodacao.SelectedIndex > 0)
                //        {
                //            valor = TabelaValor.CalculaValor(itemBeneficiario.Value, idade, cboContrato.SelectedValue, cboPlano.SelectedValue, ((Contrato.eTipoAcomodacao)Convert.ToInt32(cboAcomodacao.SelectedValue)), base.CStringToDateTime(txtAdmissao.Text), null);
                //            valorBeneficiario += valor;
                //        }

                //    }

                //    if (base.HaItemSelecionado(cboEstipulante) && chkCobrarTaxa.Visible && chkCobrarTaxa.Checked)
                //    {
                //        //checa se há taxa associativa
                //        EstipulanteTaxa taxa = EstipulanteTaxa.CarregarVigente(cboEstipulante.SelectedValue);
                //        if (taxa != null && taxa.Valor > 0 && ((EstipulanteTaxa.eTipoTaxa)taxa.TipoTaxa) == EstipulanteTaxa.eTipoTaxa.PorVida)
                //        {
                //            sb.AppendLine("<tr class='tdNormal1'><td>");
                //            sb.Append("Taxa associativa");
                //            sb.Append("</td><td>");
                //            total += taxa.Valor;
                //            valorBeneficiario += taxa.Valor;
                //            sb.Append(taxa.Valor.ToString("C"));
                //            sb.Append("</td></tr>");
                //        }
                //    }
                //    if (adicionais != null)
                //    {
                //        foreach (AdicionalBeneficiario adicional in adicionais)
                //        {
                //            if (adicional.BeneficiarioID == null) { continue; }

                //            Adicional produto = new Adicional();
                //            produto.ID = adicional.AdicionalID;
                //            produto.Carregar();
                //            sb.AppendLine("<tr class='tdNormal1'><td>");
                //            sb.Append(produto.Descricao);
                //            sb.Append("</td><td>");

                //            if(contrato.ID == null)
                //                valor = Adicional.CalculaValor(adicional.AdicionalID, adicional.BeneficiarioID, idade);
                //            else
                //                valor = Adicional.CalculaValor(adicional.AdicionalID, adicional.BeneficiarioID, idade, contrato.Vigencia, null);

                //            total += valor;
                //            valorBeneficiario += valor;
                //            sb.Append(valor.ToString("C"));
                //            sb.Append("</td></tr>");
                //        }
                //    }

                //    sb.AppendLine("</table>");
                //    this.AdicionaValor(itemBeneficiario.Value, valorBeneficiario);
                #endregion
            }

            #region comentado
            //foreach (ListItem itemBeneficiario in cboBeneficiarioAdicional.Items)
            //{
            //    if (sb.Length > 0) { sb.AppendLine("<br>"); }
            //    sb.AppendLine("<table cellpadding='2' cellspacing='0' border='0' width='100%'>");

            //    IList<AdicionalBeneficiario> adicionais =
            //        (IList<AdicionalBeneficiario>)ViewState["adben_" + itemBeneficiario.Value];

            //    Beneficiario beneficiario = new Beneficiario();
            //    beneficiario.ID = itemBeneficiario.Value;
            //    beneficiario.Carregar();
            //    int idade = Convert.ToInt32(base.DateDiff(2, beneficiario.DataNascimento));

            //    sb.AppendLine("<tr><td class='tdPrincipal1' colspan='2'>");
            //    sb.Append(beneficiario.Nome);
            //    sb.Append("</td></tr>");

            //    //IDADE
            //    sb.AppendLine("<tr class='tdNormal1'><td width='25%'>Idade</td><td>");
            //    sb.Append(idade);
            //    sb.Append("</td></tr>");

            //    Decimal valor = 0, valorBeneficiario = 0;
            //    if (base.HaItemSelecionado(cboPlano))
            //    {
            //        //VALOR DO PLANO
            //        if (cboAcomodacao.SelectedIndex > 0)
            //        {
            //            valor = TabelaValor.CalculaValor(itemBeneficiario.Value, idade, cboContrato.SelectedValue, cboPlano.SelectedValue, ((Contrato.eTipoAcomodacao)Convert.ToInt32(cboAcomodacao.SelectedValue)), base.CStringToDateTime(txtAdmissao.Text), null);
            //            valorBeneficiario += valor;
            //        }
            //        total += valor;
            //        sb.AppendLine("<tr class='tdNormal1'><td>Valor plano</td><td>");
            //        sb.Append(valor.ToString("C"));
            //        sb.Append("</td></tr>");
            //    }

            //    if (base.HaItemSelecionado(cboEstipulante) && chkCobrarTaxa.Visible && chkCobrarTaxa.Checked)
            //    {
            //        //checa se há taxa associativa
            //        EstipulanteTaxa taxa = EstipulanteTaxa.CarregarVigente(cboEstipulante.SelectedValue);
            //        if (taxa != null && taxa.Valor > 0 && ((EstipulanteTaxa.eTipoTaxa)taxa.TipoTaxa) == EstipulanteTaxa.eTipoTaxa.PorVida)
            //        {
            //            sb.AppendLine("<tr class='tdNormal1'><td>");
            //            sb.Append("Taxa associativa");
            //            sb.Append("</td><td>");
            //            total += taxa.Valor;
            //            valorBeneficiario += taxa.Valor;
            //            sb.Append(taxa.Valor.ToString("C"));
            //            sb.Append("</td></tr>");
            //        }
            //    }
            //    if (adicionais != null)
            //    {
            //        foreach (AdicionalBeneficiario adicional in adicionais)
            //        {
            //            if (adicional.BeneficiarioID == null) { continue; }

            //            Adicional produto = new Adicional();
            //            produto.ID = adicional.AdicionalID;
            //            produto.Carregar();
            //            sb.AppendLine("<tr class='tdNormal1'><td>");
            //            sb.Append(produto.Descricao);
            //            sb.Append("</td><td>");

            //            if(contrato.ID == null)
            //                valor = Adicional.CalculaValor(adicional.AdicionalID, adicional.BeneficiarioID, idade);
            //            else
            //                valor = Adicional.CalculaValor(adicional.AdicionalID, adicional.BeneficiarioID, idade, contrato.Vigencia, null);

            //            total += valor;
            //            valorBeneficiario += valor;
            //            sb.Append(valor.ToString("C"));
            //            sb.Append("</td></tr>");
            //        }
            //    }
            #endregion

            sb.AppendLine("</table>");
            //    this.AdicionaValor(itemBeneficiario.Value, valorBeneficiario);
            //}

            litSumario.Text = sb.ToString();
            sb.Remove(0, sb.Length);
            sb = null;
            Decimal desconto = base.CToDecimal(txtDesconto.Text);
            total = total - desconto;
            txtValorTotal.Text = total.ToString("N2");
            this.ValorTotalProposta = total;

            this.calculaValorCobranca_Boleto();

            upFinalizacao.Update();
        }

        protected void lnkAlterarStatus_Click(Object sender, EventArgs e)
        {
            mpeAlteraStatus.Show();
        }

        protected void optStatusEdit_Changed(Object sender, EventArgs e)
        {
            carregaMotivoStatus();
            mpeAlteraStatus.Show();
        }

        protected void cmdSalvarHistoricoStatus_Click(Object sender, EventArgs e)
        {
            if (this.contratoId == null) { return; }

            if (cboStatusMotivo.Items.Count == 0)
            {
                base.Alerta(null, this, "_msg", "Selecione uma ação.");
                mpeAlteraStatus.Show();
                return;
            }

            ContratoStatusInstancia status = ContratoStatusInstancia.CarregarUltima(this.contratoId);
            if (status != null)
            {
                if (cboStatusMotivo.SelectedValue == Convert.ToString(status.StatusID))
                {
                    base.Alerta(null, this, "_msg", "A proposta ja está com este status.");
                    mpeAlteraStatus.Show();
                    return;
                }
            }

            ContratoStatusInstancia novoStatus = new ContratoStatusInstancia();
            novoStatus.ContratoID = this.contratoId;
            novoStatus.DataSistema = DateTime.Now;
            novoStatus.StatusID = cboStatusMotivo.SelectedValue;
            novoStatus.UsuarioID = Usuario.Autenticado.ID;

            novoStatus.Data = base.CStringToDateTime(txtDataInativacao.Text);
            if (novoStatus.Data == DateTime.MinValue)
            {
                base.Alerta(null, this, "_msg", "Data de reativação, inativação ou cancelamento inválida.");
                mpeAlteraStatus.Show();
                return;
            }

            novoStatus.Salvar();

            this.carregaHistoricoStatus();

            Contrato contrato = new Contrato(this.contratoId);
            contrato.Carregar();

            optNormal.Checked = optNormalEdit.Checked;
            optInativo.Checked = optInativoEdit.Checked;
            optCancelado.Checked = optCanceladoEdit.Checked;

            if (optInativoEdit.Checked)
            {
                contrato.Inativo = true;
                contrato.Cancelado = false;
            }
            else if (optCanceladoEdit.Checked)
            {
                contrato.Inativo = false;
                contrato.Cancelado = true;
            }
            else
            {
                contrato.Inativo = false;
                contrato.Cancelado = false;
            }

            if (optInativoEdit.Checked || optCanceladoEdit.Checked)
            {
                contrato.DataCancelamento = novoStatus.Data; //base.CStringToDateTime(txtDataInativacao.Text);
            }
            else
            {
                contrato.DataCancelamento = DateTime.MinValue;
            }

            contrato.Obs += Environment.NewLine + Environment.NewLine + txtObsEdit.Text;
            txtObsEdit.Text = "";
            txtObs.Text = contrato.Obs;

            contrato.Salvar();
            mpeAlteraStatus.Hide();
        }

        #endregion

        void SetaEstadoCivil(Object estadoCivilId)
        {
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            if (Session[ConferenciaObjKey] != null)
                Response.Redirect("~/admin/conferenciaLista.aspx");
            else if (Usuario.Autenticado.PerfilID == Perfil.Financeiro_RecupPendencias)
                Response.Redirect("~/admin/cobrancaRecuperacao.aspx");
            else
                Response.Redirect("~/UBRASP/contratos.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (!IsValid) { return; }

            #region validacoes

            #region Tab1

            if (txtCorretorTerceiroIdentificacao.Text.Trim() != "" && txtCorretorTerceiroCPF.Text.Trim() == "")
            {
                base.Alerta(null, this, "_CorrTerc", "Atenção!\\nVocê deve informar o CPF do corretor.");
                txtCorretorTerceiroCPF.Focus();
                tab.ActiveTabIndex = 0;
                return;
            }

            if (txtCorretorTerceiroCPF.Text.Trim() != "")
            {
                if (!Beneficiario.ValidaCpf(txtCorretorTerceiroCPF.Text))
                {
                    base.Alerta(MPE, ref litAlert, "Atenção!<br>CPF do corretor pessoa física inválido.", upnlAlerta);
                    tab.ActiveTabIndex = 0;
                    return;
                }
            }

            if (txtSuperiorTerceiroIdentificacao.Text.Trim() != "" && txtSuperiorTerceiroCPF.Text.Trim() == "")
            {
                base.Alerta(null, this, "_SupeTerc", "Atenção!\\nVocê deve informar o CPF do superior.");
                txtSuperiorTerceiroCPF.Focus();
                tab.ActiveTabIndex = 0;
                return;
            }

            if (txtSuperiorTerceiroCPF.Text.Trim() != "")
            {
                if (!Beneficiario.ValidaCpf(txtSuperiorTerceiroCPF.Text))
                {
                    base.Alerta(null, this, "_SupeTercCpf", "Atenção!<br>CPF do superior pessoa física inválido.");
                    tab.ActiveTabIndex = 0;
                    txtSuperiorTerceiroCPF.Focus();
                    return;
                }
            }

            //nao pode acontecer de um titular ser tambem um dependente na proposta
            if (this.Dependentes != null)
            {
                foreach (ContratoBeneficiario cb in this.Dependentes)
                {
                    if (Convert.ToString(cb.BeneficiarioID) == Convert.ToString(this.TitularID))
                    {
                        base.Alerta(MPE, ref litAlert, "Atenção!<br>Titular e dependente são a mesma pessoa.", upnlAlerta);
                        tab.ActiveTabIndex = 0;
                        return;
                    }
                }
            }

            if (String.IsNullOrEmpty(txtCorretorID.Value))
            {
                //base.Alerta(MPE, ref litAlert, "Atenção!<br>Selecione um profissional emissor do contrato.", upnlAlerta);
                //tab.ActiveTabIndex = 0;
                //return;
                txtCorretorID.Value = "3";
            }

            if (txtNumeroContrato.Text.Trim() == String.Empty)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Informe o número do contrato.", upnlAlerta);
                txtNumeroContrato.Focus();
                tab.ActiveTabIndex = 0;
                return;
            }

            if (!base.HaItemSelecionado(cboEstipulante))
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há um estipulante selecionado.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                cboEstipulante.Focus();
                return;
            }

            if (!base.HaItemSelecionado(cboPlano))
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há um plano selecionado.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                return;
            }

            if (cboTipoProposta.SelectedIndex <= 0)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há um tipo de proposta selecionado.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                cboTipoProposta.Focus();
                return;
            }

            String msg = "";
            if (!Contrato.ContratoDisponivel(ViewState[IDKey], cboOperadora.SelectedValue, txtNumeroContrato.Text, ref msg))
            {
                base.Alerta(MPE, ref litAlert, msg, upnlAlerta);
                tab.ActiveTabIndex = 0;
                txtNumeroContrato.Focus();
                return;
            }

            if (cboAcomodacao.SelectedIndex <= 0)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há um tipo de acomodação selecionado.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                cboAcomodacao.Focus();
                return;
            }

            if (txtVigencia.Text == "" || txtVencimento.Text == "")
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Informe uma data de admissão valida.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                return;
            }

            if (pnlInfoAnterior.Visible && (txtEmpresaAnterior.Text.Trim() == "" || txtEmpresaAnteriorMeses.Text.Trim() == "" || txtEmpresaAnteriorMatricula.Text == ""))
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Informe os dados da empresa anterior.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                return;
            }

            //checa se o impresso foi rasurado...
            String letra = String.Empty;
            if (UIHelper.PrimeiraPosicaoELetra(txtNumeroContrato.Text))
                letra = txtNumeroContrato.Text.Substring(0, 1);

            AlmoxContratoImpresso aci = null;

            try
            {
                if (!String.IsNullOrEmpty(letra))
                    aci = AlmoxContratoImpresso.CarregarPorNumeroProduto(Convert.ToInt32(txtNumeroContrato.Text.Replace(letra, "")), cboOperadora.SelectedValue, letra, -1);
                else
                    aci = AlmoxContratoImpresso.CarregarPorNumeroProduto(Convert.ToInt32(txtNumeroContrato.Text), cboOperadora.SelectedValue, letra, -1);

                if (aci != null && aci.Rasurado)
                {
                    base.Alerta(MPE, ref litAlert, "Atenção!<br>Essa proposta foi confirmada pelo almoxarifado como rasurada.", upnlAlerta);
                    return;
                }
            }
            catch
            {
                aci = null;
            }

            #endregion

            #region Tab2

            if (String.IsNullOrEmpty(txtNome.Text.Trim()))
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há um titular.", upnlAlerta);
                tab.ActiveTabIndex = 1;
                return;
            }
            else
            {
                IList<Beneficiario> lista = Beneficiario.CarregarPorParametro("", txtCPF.Text, "", SearchMatchType.QualquerParteDoCampo); //TODO: criar um load parcial. Está carregando o obj inteiro.
                if (lista == null || lista.Count == 0)
                {
                    base.Alerta(MPE, ref litAlert, "Atenção!<br>CPF do titular não encontrado.", upnlAlerta);
                    tab.ActiveTabIndex = 1;
                    return;
                }
            }

            if (txtDataNascimentoResponsavel.Text.Trim() != "")
            {
                DateTime data = new DateTime();
                if (!UIHelper.TyParseToDateTime(txtDataNascimentoResponsavel.Text, out data))
                {
                    base.Alerta(null, this, "_dataNascResp", "Atenção!\\nData de nascimento do responsável legal inválida.");
                    tab.ActiveTabIndex = 1;
                    txtDataNascimentoResponsavel.Focus();
                    return;
                }
            }

            //if (txtCarenciaCodigo.Text.Trim() == "" && this.contratoId == null)
            //{
            //    base.Alerta(null, this, "_PRCTit", "Atenção!\\nPRC do titular é obrigatório.");
            //    tab.ActiveTabIndex = 1;
            //    txtCarenciaCodigo.Focus();
            //    return;
            //}

            //if (Operadora.IsAmil(cboOperadora.SelectedValue) && txtCarenciaCodigo.Text.Trim() != "")
            //{
            //    if (!Operadora.ValidaPRCAmil(txtCarenciaCodigo.Text))
            //    {
            //        String validos = String.Join(", ", Operadora.AmilPRCs);

            //        if (validos.Trim() != "")
            //        {
            //            base.Alerta(null, this, "_PRCTit1", "Atenção!\\nPRC do titular é inválido.\\nValores válidos: " + validos);
            //            tab.ActiveTabIndex = 1;
            //            txtCarenciaCodigo.Focus();
            //            return;
            //        }
            //    }
            //}

            DateTime nasc = base.CStringToDateTime(txtDataNascimento.Text);
            Int32 idade = Beneficiario.CalculaIdade(nasc, DateTime.Now);

            if (idade < 18) // && (txtNomeResponsavel.Text.Trim() == "" || txtCPFResponsavel.Text.Trim() == ""))
            {
                if (this.contratoId == null)
                {
                    if (txtNomeResponsavel.Text.Trim() == "" || txtCPFResponsavel.Text.Trim() == "")
                    {
                        base.Alerta(null, this, "_RespTit", "Atenção!\\nVocê deve informar o responsável legal do titular (nome e CPF). ");
                        tab.ActiveTabIndex = 1;
                        txtNomeResponsavel.Focus();
                        return;
                    }

                    if (!Beneficiario.ValidaCpf(txtCPFResponsavel.Text))
                    {
                        base.Alerta(null, this, "_RespCpfTit", "Atenção!\\nO CPF do responsável legal do titular é inválido. ");
                        tab.ActiveTabIndex = 1;
                        txtCPFResponsavel.Focus();
                        return;
                    }
                    else if (txtCPF.Text == txtCPFResponsavel.Text)
                    {
                        base.Alerta(null, this, "_err", "Atenção!\\nOs CPFs do responsável legal e do titular não podem ser iguais. ");
                        tab.ActiveTabIndex = 1;
                        txtCPFResponsavel.Focus();
                        return;
                    }

                    DateTime data = new DateTime();
                    if (!UIHelper.TyParseToDateTime(txtDataNascimentoResponsavel.Text, out data))
                    {
                        base.Alerta(null, this, "_dataNascResp", "Atenção!\\nData de nascimento do responsável legal inválida.");
                        tab.ActiveTabIndex = 1;
                        txtDataNascimentoResponsavel.Focus();
                        return;
                    }
                }
            }

            if (txtCPFResponsavel.Text.Trim() != "")
            {
                if (!Beneficiario.ValidaCpf(txtCPFResponsavel.Text))
                {
                    base.Alerta(null, this, "_RespCpfTit", "Atenção!\\nO CPF do responsável legal do titular é inválido. ");
                    tab.ActiveTabIndex = 1;
                    txtCPFResponsavel.Focus();
                    return;
                }
            }

            if (!validaAltura(txtTitAltura))
            {
                base.Alerta(null, this, "_altTit", "Atenção!\\nA altura do titular deve estar entre 10cm e 2,5m. ");
                tab.ActiveTabIndex = 1;
                txtTitAltura.Focus();
                return;
            }

            if (!validaPeso(txtTitPeso))
            {
                base.Alerta(null, this, "_pesTit", "Atenção!\\nO peso do titular deve estar entre 1kg e 300kg. ");
                tab.ActiveTabIndex = 1;
                txtTitPeso.Focus();
                return;
            }

            if (!validaIMC_Titular(txtTitPeso, txtTitAltura))
            {
                base.Alerta(null, this, "_err", "Atenção!\\nIMC fora da faixa. Encaminhar para área técnica. ");
            }

            #endregion

            Beneficiario titular = new Beneficiario(this.TitularID);
            titular.Carregar();
            int v_idade = Beneficiario.CalculaIdade(titular.DataNascimento, DateTime.Now);// base.CStringToDateTime(txtAdmissao.Text));

            if (this.Dependentes != null)
            {
                int countMae = 0, countConjuge = 0;
                ContratoADMParentescoAgregado parentesco = null;

                foreach (ContratoBeneficiario dependente in this.Dependentes)
                {
                    //if (dependente.ID != null) { dependente.Carregar(); }
                    if (dependente.ParentescoID == null) { continue; }

                    parentesco = new ContratoADMParentescoAgregado(dependente.ParentescoID);
                    parentesco.Carregar();

                    if (parentesco.ParentescoDescricao.ToLower().IndexOf("filh") > -1 && v_idade <= 12)
                    {
                        base.Alerta(MPE, ref litAlert, "Não é possível cadastrar um(a) filho(a) como dependente para este titular.", upnlAlerta);
                        tab.ActiveTabIndex = 0;
                        return;
                    }

                    if (parentesco.ParentescoDescricao.ToLower().IndexOf("pai") > -1 ||
                        parentesco.ParentescoDescricao.ToLower().Replace("ã", "a").IndexOf("mae") > -1)
                    {
                        countMae++;
                    }

                    if (parentesco.ParentescoDescricao.ToLower().IndexOf("espos") > -1 ||
                        parentesco.ParentescoDescricao.ToLower().Replace("ô", "o").IndexOf("conjuge") > -1)
                    {
                        countConjuge++;
                    }
                }

                if (countConjuge > 1)
                {
                    base.Alerta(MPE, ref litAlert, "Apenas um cônjuge como dependente é permitido.", upnlAlerta);
                    tab.ActiveTabIndex = 0;
                    return;
                }

                if (countMae > 2)
                {
                    base.Alerta(MPE, ref litAlert, "Apenas um pai e uma mãe serão permitidos como dependentes.", upnlAlerta);
                    tab.ActiveTabIndex = 0;
                    return;
                }
            }


            this.MontaCombosDeBeneficiarios();
            if (cboBeneficiarioFicha.Items.Count == 0)
            {
                //base.Alerta(null, this, "_numBenef", "Atenção!\\nNão há beneficiários na proposta.");
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há beneficiários na proposta.", upnlAlerta);
                return;
            }

            if (!ChecaNumeroDeContrato())
            {
                tab.ActiveTabIndex = 0;
                return;
            }

            if (ViewState[PropostaEndReferecia] == null)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Deve haver um endereço de referência.", upnlAlerta);
                return;
            }

            if (ViewState[PropostaEndCobranca] == null)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Deve haver um endereço de cobrança.", upnlAlerta);
                return;
            }

            //ContratoADM contrato = new ContratoADM(cboContrato.SelectedValue);
            //contrato.Carregar();
            //DateTime dteContrato = new DateTime(contrato.Data.Year, contrato.Data.Month, contrato.Data.Day, 0, 0, 0);
            //DateTime admissao = base.CStringToDateTime(txtAdmissao.Text);
            //if (admissao < dteContrato)
            //{
            //    base.Alerta(MPE, ref litAlert, "Atenção!<br>A data de admissão não pode ser inferior à data do contrato administrativo.", upnlAlerta);
            //    return;
            //}

            #endregion

            try
            {
                this.Salvar(null, false);
            }
            catch (Exception ex)
            {
                base.Alerta(null, this, "_errSvInes", ex.Message.Replace("'", "´"));
            }
        }

        protected void cmdRascunho_Click(Object sender, EventArgs e)
        {
            if (cboTipoProposta.SelectedIndex <= 0)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há um tipo de proposta selecionado.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                cboTipoProposta.Focus();
                return;
            }

            try
            {
                this.Salvar(null, true);
            }
            catch
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Houve um erro inesperado.<br>Por favor tente novamente.", upnlAlerta);
            }
        }

        Object[] ObtemFichasPreenchidas()
        {
            Object[] fichas = new Object[cboBeneficiarioFicha.Items.Count];
            for (int i = 0; i < cboBeneficiarioFicha.Items.Count; i++)
            {
                fichas[i] = ViewState["fic_" + cboBeneficiarioFicha.Items[i].Value];
            }
            return fichas;
        }

        protected void cmdLiberar_Click(Object sender, EventArgs e)
        {
            if (!IsValid) { return; }

            #region validacoes (centralizar)

            #region Tab1

            if (String.IsNullOrEmpty(txtCorretorID.Value)) //if (cboProfissional.Items.Count == 0)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Selecione um profissional emissor do contrato.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                return;
            }

            if (txtNumeroContrato.Text.Trim() == String.Empty)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Informe o número do contrato.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                return;
            }

            if (!base.HaItemSelecionado(cboEstipulante))
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há um estipulante selecionado.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                cboEstipulante.Focus();
                return;
            }

            if (!base.HaItemSelecionado(cboPlano))
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há um plano selecionado.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                return;
            }

            if (cboTipoProposta.SelectedIndex <= 0)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há um tipo de proposta selecionado.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                cboTipoProposta.Focus();
                return;
            }
            #endregion

            #region Tab2

            if (String.IsNullOrEmpty(txtNome.Text.Trim()))
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há um titular.", upnlAlerta);
                tab.ActiveTabIndex = 1;
                return;
            }
            else
            {
                IList<Beneficiario> lista = Beneficiario.CarregarPorParametro("", txtCPF.Text, "", SearchMatchType.QualquerParteDoCampo); //TODO: criar um load parcial. Está carregando o obj inteiro.
                if (lista == null || lista.Count == 0)
                {
                    base.Alerta(MPE, ref litAlert, "Atenção!<br>CPF do titular não encontrado.", upnlAlerta);
                    tab.ActiveTabIndex = 1;
                    return;
                }
            }
            #endregion

            if (cboBeneficiarioFicha.Items.Count == 0)
            {
                base.Alerta(MPE, ref litAlert, "Atenção!<br>Não há beneficiários na proposta.", upnlAlerta);
                return;
            }

            #endregion

            Usuario user = Usuario.Autentica(txtLogin.Text, txtSenha.Text);

            if (user == null || !user.LiberaContratos) // || Usuario.Autenticado.ID == Convert.ToString(user.ID))
            {
                base.Alerta(MPE, ref litAlert, "Usuário inválido.", upnlAlerta);
                return;
            }

            this.Salvar(user.ID, false);
        }

        void pegaObs(ref Contrato contrato)
        {
            if (!String.IsNullOrEmpty(txtObs.Text.Trim()))
                contrato.Obs = txtObs.Text;

            if (!String.IsNullOrEmpty(txtObsEdit.Text.Trim()))
            {
                if (!String.IsNullOrEmpty(contrato.Obs) && contrato.Obs.Length > 0)
                    contrato.Obs += Environment.NewLine + Environment.NewLine + txtObsEdit.Text;
                else
                    contrato.Obs = txtObsEdit.Text;
            }

            txtObs.Text = contrato.Obs;
            txtObsEdit.Text = "";
            upFinalizacao.Update();
        }

        void Salvar(Object usuarioLiberadorId, Boolean rascunho)
        {
            if (cboFilial.SelectedIndex == 0 && this.contratoId == null) { base.Alerta(null, this, "_err", "Você deve informar a filial."); return; }
            if (cboEstadoCivil.Items.Count == 0) { base.Alerta(null, this, "_err", "Não há um estado civil selecionado."); return; }

            Object[] fichas = this.ObtemFichasPreenchidas();

            Contrato contrato = new Contrato();
            contrato.Rascunho = rascunho;
            contrato.Legado = true;
            contrato.ID = ViewState[IDKey];

            if (cboFilial.SelectedIndex > 0)
                contrato.FilialID = cboFilial.SelectedValue;
            else
                contrato.FilialID = null;

            Boolean novo = true;

            if (contrato.ID != null)
            {
                Contrato prova = new Contrato(contrato.ID);
                prova.Carregar();
                contrato.Pendente = prova.Pendente;
                contrato.Data = prova.Data;
                contrato.CodCobranca = prova.CodCobranca;
                contrato.Alteracao = DateTime.Now;
                contrato.UsuarioID = prova.UsuarioID;
                contrato.Inativo = prova.Inativo;
                contrato.Cancelado = prova.Cancelado;
                contrato.DataCancelamento = prova.DataCancelamento;
                contrato.Legado = prova.Legado;

                ContratoBeneficiario titularAntigo = ContratoBeneficiario.CarregarTitular(prova.ID, null);
                if (titularAntigo != null && this.TitularID_ContratoBeneficiario != null)
                {
                    if (Convert.ToString(titularAntigo.BeneficiarioID) != Convert.ToString(this.TitularID))
                    {
                        //alterou o titular, log a ação
                        LC.Framework.Phantom.NonQueryHelper.Instance.ExecuteNonQuery(
                            String.Concat("insert into CONTRATO_BENEFICIARIO_LOG (log_contratoId,log_beneficiarioId, log_usuarioId, log_data) values (", prova.ID, ",", titularAntigo.BeneficiarioID, ",", Usuario.Autenticado.ID, ", getdate())"),
                            null);
                    }
                }

                novo = false;
            }
            else
            {
                contrato.Alteracao = DateTime.MinValue;
                contrato.Data = base.CStringToDateTime(txtDataContrato.Text);
                contrato.Data = new DateTime(contrato.Data.Year, contrato.Data.Month, contrato.Data.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                contrato.UsuarioID = Usuario.Autenticado.ID;
            }

            contrato.EstipulanteID = cboEstipulante.SelectedValue;
            contrato.OperadoraID = cboOperadora.SelectedValue;
            contrato.ContratoADMID = cboContrato.SelectedValue;
            if (chkCobrarTaxa.Visible && chkCobrarTaxa.Checked)
                contrato.CobrarTaxaAssociativa = true;
            else
                contrato.CobrarTaxaAssociativa = false;

            contrato.NumeroID = this.ContratoImpressoID;

            if (ViewState[PropostaEndCobranca] != null)
                contrato.EnderecoCobrancaID = ViewState[PropostaEndCobranca];
            else
                contrato.EnderecoCobrancaID = ViewState[PropostaEndReferecia];

            contrato.EnderecoReferenciaID = ViewState[PropostaEndReferecia];

            contrato.Admissao = base.CStringToDateTime(txtAdmissao.Text);
            contrato.Vigencia = base.CStringToDateTime(txtVigencia.Text);
            contrato.Vencimento = base.CStringToDateTime(txtVencimento.Text);
            contrato.Desconto = base.CToDecimal(txtDesconto.Text);

            //checa se houve participacao de um operador de telemarketing
            if (!String.IsNullOrEmpty(txtOperador.Text))
            {
                contrato.OperadorTmktID = txtOperadorID.Value;
            }

            contrato.DonoID = txtCorretorID.Value;

            contrato.CorretorTerceiroNome = txtCorretorTerceiroIdentificacao.Text;
            contrato.CorretorTerceiroCPF = txtCorretorTerceiroCPF.Text;
            contrato.SuperiorTerceiroNome = txtSuperiorTerceiroIdentificacao.Text;
            contrato.SuperiorTerceiroCPF = txtSuperiorTerceiroCPF.Text;

            contrato.Numero = txtNumeroContrato.Text;
            contrato.NumeroMatricula = txtNumeroMatricula.Text;
            contrato.PlanoID = cboPlano.SelectedValue;
            contrato.TipoAcomodacao = Convert.ToInt32(cboAcomodacao.SelectedValue);
            contrato.TipoContratoID = cboTipoProposta.SelectedValue;

            contrato.ResponsavelParentescoID = cboParentescoResponsavel.SelectedValue;
            contrato.ResponsavelSexo = cboSexoResponsavel.SelectedValue;
            contrato.ResponsavelRG = txtRGResponsavel.Text;
            contrato.ResponsavelNome = txtNomeResponsavel.Text;
            contrato.ResponsavelCPF = txtCPFResponsavel.Text;
            if (txtDataNascimentoResponsavel.Text.Trim() != "")
            {
                DateTime data = new DateTime();
                if (UIHelper.TyParseToDateTime(txtDataNascimentoResponsavel.Text, out data))
                {
                    contrato.ResponsavelDataNascimento = data;
                }
            }

            ContratoBeneficiario titular = new ContratoBeneficiario();
            titular.ID = this.TitularID_ContratoBeneficiario;
            if (titular.ID != null) { titular.Carregar(); }
            titular.BeneficiarioID = this.TitularID;
            titular.EstadoCivilID = cboEstadoCivil.SelectedValue; //cboEstadoCivilDependente.SelectedValue;
            titular.DataCasamento = base.CStringToDateTime(txtTitDataCasamento.Text);
            titular.NumeroMatriculaSaude = txtNumMatriculaSaude.Text;
            titular.NumeroMatriculaDental = txtNumMatriculaDental.Text;
            titular.Altura = UIHelper.CToDecimal(txtTitAltura.Text);
            titular.Peso = UIHelper.CToDecimal(txtTitPeso.Text);

            if (this.chkCobrarTaxa.Visible && this.chkCobrarTaxa.Checked)
                titular.Valor = this.PegaValor(titular.BeneficiarioID) + ValorTaxaAssociativaContrato;
            else
                titular.Valor = this.PegaValor(titular.BeneficiarioID);

            titular.Portabilidade = txtPortabilidade.Text;
            titular.CarenciaOperadora = cboCarenciaOperadora.Text;
            titular.CarenciaOperadoraID = base.CToObject(txtCarenciaOperadoraID.Value);
            titular.CarenciaCodigo = txtCarenciaCodigo.Text;
            titular.CarenciaContratoDe = base.CStringToDateTime(txtTitTempoContratoDe.Text);
            titular.CarenciaContratoAte = base.CStringToDateTime(txtTitTempoContratoAte.Text);
            titular.CarenciaContratoTempo = base.CToInt(txtCarenciaTempoContrato.Text);
            titular.CarenciaMatriculaNumero = txtCarenciaMatricula.Text;

            //obtem os adicionais contratados
            List<AdicionalBeneficiario> adicionaisContratados = null;
            //foreach (ListItem item in cboBeneficiarioAdicional.Items)
            //{
            //    if (ViewState["adben_" + item.Value] != null)
            //    {
            //        if (adicionaisContratados == null) { adicionaisContratados = new List<AdicionalBeneficiario>(); }
            //        foreach (AdicionalBeneficiario _ab in ((List<AdicionalBeneficiario>)ViewState["adben_" + item.Value]))
            //        {
            //            if (_ab.BeneficiarioID != null) { adicionaisContratados.Add(_ab); }
            //        }
            //    }
            //}

            this.pegaObs(ref contrato);

            if (pnlInfoAnterior.Visible)
            {
                contrato.EmpresaAnterior = txtEmpresaAnterior.Text;
                contrato.EmpresaAnteriorTempo = Convert.ToInt32(txtEmpresaAnteriorMeses.Text);
                contrato.EmpresaAnteriorMatricula = txtEmpresaAnteriorMatricula.Text;
            }
            else
            {
                contrato.EmpresaAnterior = null;
                contrato.EmpresaAnteriorTempo = 0;
                contrato.EmpresaAnteriorMatricula = null;
            }

            if (this.Dependentes != null)
            {
                foreach (ContratoBeneficiario dependente in this.Dependentes)
                {
                    dependente.Valor = this.PegaValor(dependente.BeneficiarioID);
                }
            }

            if (ViewState[IDKey] != null)
            {
                //checa se houve alteracao de plano. se houve, grava historico
                Contrato contratoAntigo = new Contrato();
                contratoAntigo.ID = ViewState[IDKey];
                contratoAntigo.Carregar();

                if (ViewState[AlteraPlanoKey] != null) //cboPlano.SelectedValue != Convert.ToString(contratoAntigo.PlanoID) &&  : Também há a possibilidade de alterar o tipo de acomodação, mas sem necessariamente trocar de plano!
                {
                    DateTime admissao = new DateTime();
                    UIHelper.TyParseToDateTime(Convert.ToString(ViewState[NovaDataAdmisssaoKey]), out admissao);
                    ContratoPlano cp = new ContratoPlano();
                    cp.ContratoID = contratoAntigo.ID;
                    cp.Data = admissao;
                    cp.PlanoID = cboPlano.SelectedValue;
                    cp.TipoAcomodacao = Convert.ToInt32(cboAcomodacao.SelectedValue);
                    cp.Salvar();

                    //guarda para gerar arquivo de movimentação
                    if (Convert.ToString(contrato.OperadoraID) == Convert.ToString(Operadora.UnimedID))
                    {
                        ItemAgendaArquivoUnimed item = new ItemAgendaArquivoUnimed();
                        item.PropostaID = cp.ContratoID;
                        item.BeneficiarioID = titular.BeneficiarioID;
                        item.Tipo = 4;
                        item.TipoDescricao = "MUDANÇA DE PLANO";
                        item.Salvar();
                    }
                }

                Conferencia conferencia = Session[ConferenciaObjKey] as Conferencia;
                if (conferencia != null) { titular.Data = conferencia.PropostaData; }
                contrato.Alteracao = DateTime.Now;

                ContratoFacade.Instance.Salvar(contrato, titular, this.Dependentes, fichas, usuarioLiberadorId, adicionaisContratados, conferencia, this.ValorTotalProposta, false);
                ViewState.Remove(AlteraPlanoKey);
                Session[ConferenciaObjKey] = null;
            }
            else
            {
                Conferencia conferencia = Session[ConferenciaObjKey] as Conferencia;
                if (conferencia != null) { titular.Data = conferencia.PropostaData; }
                ContratoFacade.Instance.Salvar(contrato, titular, this.Dependentes, fichas, usuarioLiberadorId, adicionaisContratados, conferencia, this.ValorTotalProposta, false);

                ContratoPlano cp = new ContratoPlano();
                cp.ContratoID = contrato.ID;
                cp.Data = contrato.Data;
                cp.PlanoID = contrato.PlanoID;
                cp.Salvar();
            }

            ViewState[IDKey] = contrato.ID;
            this.CarregaFichaDeSaude(true);
            //cmdAlterarPlano.Visible = true; denis

            tblMsgRegras.Visible = false;
            Session[ConferenciaObjKey] = null;

            if (novo) 
            { 
                Session[FilialIDKey] = cboFilial.SelectedValue;
                Response.Redirect("contrato.aspx?" + IDKey + "=" + contrato.ID); 
            }
            else
            {
                String msg = "";
                ContratoFacade.Instance.AtualizaValorDeCobrancas(contrato.ID, out msg);
                this.ConfiguraAtendimento();

                if (String.IsNullOrEmpty(msg))
                    base.Alerta(null, this, "_salvo", "Contrato salvo com sucesso.");
                else
                    base.Alerta(null, this, "_salvo", "Contrato salvo com sucesso.\\nATENÇÃO: " + msg);

                upAtendimento.Update();
            }
        }

        Boolean ChecaNumeroDeContrato()
        {
            return true;
        }

        ////////////////////////////// ATENDIMENTO /////////////////////////////////////////

        void carregaTiposDeAtendimento()
        {
            cboTipoAtendimento.Items.Clear();
            IList<AtendimentoTipo> tipos = null;

            if (String.IsNullOrEmpty(Usuario.Autenticado.EmpresaCobrancaID))
                tipos = AtendimentoTipo.CarregarTodos();
            else
                tipos = AtendimentoTipo.CarregarTodos(AtendimentoTipo.eTipo.EmpresaCobranca);

            if (tipos != null)
            {
                foreach (AtendimentoTipo tipo in tipos)
                {
                    cboTipoAtendimento.Items.Add(new ListItem(
                        tipo.Descricao, Convert.ToString(tipo.ID)));
                }
            }

            cboTipoAtendimento.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        void ConfiguraAtendimento()
        {
            if (this.contratoId == null) { return; }
            this.CarregarAtendimentos();

            calculaValorCobranca_Boleto();

            ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(this.contratoId, null);

            if (titular != null)
            {
                Beneficiario beneficiario = new Beneficiario(titular.BeneficiarioID);
                beneficiario.Carregar();
                txtEmailAtendimento.Text = beneficiario.Email;
            }
        }

        protected void optAtendimento_Changed(Object sender, EventArgs e)
        {
        }

        void CarregarAtendimentos()
        {
            if (this.contratoId == null) { pnlAtendimento.Visible = false; return; }

            if (String.IsNullOrEmpty(Usuario.Autenticado.EmpresaCobrancaID))
                gridAtendimento.DataSource = AtendimentoTemp.CarregarPorProposta(this.contratoId);
            else
                gridAtendimento.DataSource = AtendimentoTemp.CarregarPorProposta(this.contratoId, AtendimentoTipo.eTipo.EmpresaCobranca);

            gridAtendimento.DataBind();
        }

        void calculaValorCobranca_Boleto() //chamado por ConfiguraAtendimento
        {
            IList<Cobranca> cobrancas = this.CarregarCobrancas();
            List<CobrancaComposite> composite = null;

            Contrato contrato = new Contrato(this.contratoId);
            contrato.Carregar();

            if (cobrancas != null && cobrancas.Count > 0)
            {
                txtParcelaCob.Text = (cobrancas[0].Parcela + 1).ToString(); // (cobrancas[cobrancas.Count - 1].Parcela + 1).ToString();
                DateTime proxVencto = DateTime.MinValue;

                proxVencto = cobrancas[0].DataVencimento.AddMonths(1);

                DateTime vigencia, vencimento, admissao = base.CStringToDateTime(txtAdmissao.Text);
                Int32 diaDataSemJuros; Object valorDataLimite;
                CalendarioVencimento rcv = null;

                if (!base.HaItemSelecionado(cboContrato)) { return; } //base.CStringToDateTime(txtAdmissao.Text)
                CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(cboContrato.SelectedValue,
                   admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv);

                if (cobrancas.Count >= 2)
                {
                    if (proxVencto.Month != 2)
                    {
                        proxVencto = new DateTime(proxVencto.Year,
                            proxVencto.Month, contrato.Vencimento.Day, 23, 59, 59, 599);
                    }
                    else
                    {
                        proxVencto = new DateTime(proxVencto.Year,
                            proxVencto.Month, UltimoDiaDoMes(2, proxVencto.Year).Day, 23, 59, 59, 599);
                    }
                }
                else if (cobrancas.Count == 1)
                    proxVencto = new DateTime(contrato.Vencimento.Year,
                        contrato.Vencimento.Month, contrato.Vencimento.Day, 23, 59, 59, 599);

                txtVencimentoCob.Text = proxVencto.ToString("dd/MM/yyyy");
                txtValorCob.Text = Contrato.CalculaValorDaProposta2(this.contratoId, proxVencto, null, false, true, ref composite, false).ToString("N2");
            }
            else
            {
                txtParcelaCob.Text = "1";
                txtVencimentoCob.Text = contrato.Vencimento.ToString("dd/MM/yyyy");
                txtValorCob.Text = Contrato.CalculaValorDaProposta2(contrato.ID, contrato.Vencimento, null, false, true, ref composite, false).ToString("N2");
            }

            if (Convert.ToDecimal(txtValorCob.Text) == Decimal.Zero)
                cmdGerarCobranca.Enabled = false;
        }

        void LimparCamposAtendimento()
        {
            gridAtendimento.SelectedIndex = -1;
            txtTitulo.Text = "";
            txtTexto.Text = "";
            txtTexto2.Text = "";
            txtDataInicio.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtDataPrevisao.Text = "";
            lblAtendimentoProtocolo.Text = "-------";

            txtDataInicio.ReadOnly = false;
            imgDataInicio.Visible = true;

            txtDataPrevisao.ReadOnly = false;
            imgDataPrevisao.Visible = true;

            chkAtendimentoConcluido.Checked = false; //txtDataConclusao.Text = "";
            chkAtendimentoConcluido.Enabled = true;

            litResolvidoPor.Text = "";
            cboSubTipoAtendimento.SelectedIndex = 0;
        }

        protected void cboTipoAtendimento_Change(Object sender, EventArgs e)
        {
            if (cboTipoAtendimento.SelectedIndex == 0) { return; }

            AtendimentoTipo obj = new AtendimentoTipo(cboTipoAtendimento.SelectedValue);
            obj.Carregar();

            DateTime inicio = base.CStringToDateTime(txtDataInicio.Text);
            if (inicio != DateTime.MinValue && litResolvidoPor.Text.Trim() == "")
            {
                DateTime previsao = inicio.AddDays(obj.PrazoConclusao);
                while (previsao.DayOfWeek == DayOfWeek.Saturday || previsao.DayOfWeek == DayOfWeek.Sunday)
                    previsao = previsao.AddDays(1);

                txtDataPrevisao.Text = previsao.ToString("dd/MM/yyyy");
            }
        }

        /// <summary>
        /// Não está sendo usado.
        /// </summary>
        [Obsolete("Em desuso", true)]
        protected void cmdNovoAtendimento_Click(Object sender, EventArgs e)
        {
        }

        protected void cmdSalvarAtendimento_Click(Object sender, EventArgs e)
        {
            #region validacoes

            DateTime dataParam = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (txtTexto.Text.Trim() == "")
            {
                Alerta(null, this, "_err1", "Nenhum atendimento informado.");
                txtTexto.Focus();
                return;
            }
            if (txtTexto2.Visible && txtTexto2.Text.Trim() == "")
            {
                Alerta(null, this, "_err1a", "Nenhum atendimento informado.");
                txtTexto2.Focus();
                return;
            }

            if (txtTitulo.Text.Trim() == "")
            {
                if (txtTexto.Text.Length > 30)
                    txtTitulo.Text = txtTexto.Text.Substring(0, 29) + " (...)";
                else
                    txtTitulo.Text = txtTexto.Text;
            }

            if (cboTipoAtendimento.SelectedIndex == 0)
            {
                Alerta(null, this, "_err1", "Você deve informar o tipo de atendimento.");
                cboTipoAtendimento.Focus();
                return;
            }
            if (cboSubTipoAtendimento.SelectedIndex == 0)
            {
                Alerta(null, this, "_err1", "Você deve informar o subtipo de atendimento.");
                cboSubTipoAtendimento.Focus();
                return;
            }

            if (base.CStringToDateTime(txtDataInicio.Text) == DateTime.MinValue)
            {
                txtDataInicio.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }

            if (base.CStringToDateTime(txtDataPrevisao.Text) == DateTime.MinValue)
            {
                txtDataPrevisao.Text = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
            }

            if (gridAtendimento.SelectedIndex == -1 && cboTipoAtendimento.SelectedIndex >= 1)
            {
                //não gerar mais do que uma ocorrência pelo mesmo motivo quando a 
                //primeira ainda estiver em aberto
                IList<AtendimentoTemp> atendimentos = AtendimentoTemp.
                    CarregarPorProposta(this.contratoId, cboTipoAtendimento.SelectedValue);

                if (atendimentos != null)
                {
                    foreach (AtendimentoTemp _atend in atendimentos)
                    {
                        if (String.IsNullOrEmpty(_atend.ResolvidoPor))
                        {
                            Alerta(null, this, "_err1", "Já há um atendimento em aberto para esse tipo.");
                            cboTipoAtendimento.Focus();
                            return;
                        }
                    }
                }
            }
            #endregion

            AtendimentoTemp atendimento = new AtendimentoTemp();
            if (gridAtendimento.SelectedIndex != -1)
            {
                atendimento.ID = gridAtendimento.DataKeys[gridAtendimento.SelectedIndex][0];
                atendimento.Carregar();
            }
            else
            {
                atendimento.IniciadoPor = Usuario.Autenticado.Nome;
            }

            if (chkAtendimentoConcluido.Checked)
                atendimento.DataFim = DateTime.Now; //base.CStringToDateTime(txtDataConclusao.Text);
            else
                atendimento.DataFim = DateTime.MinValue;

            if (atendimento.DataFim != DateTime.MinValue && String.IsNullOrEmpty(atendimento.ResolvidoPor))
            {
                atendimento.ResolvidoPor = Usuario.Autenticado.Nome;
            }

            atendimento.DataInicio = base.CStringToDateTime(txtDataInicio.Text);
            if (atendimento.DataInicio < dataParam && gridAtendimento.SelectedIndex == -1)
            {
                Alerta(null, this, "_err1", "Data não pode ser inferior à data atual.");
                txtDataInicio.Focus();
                return;
            }

            atendimento.DataPrevisao = base.CStringToDateTime(txtDataPrevisao.Text);
            if (atendimento.DataPrevisao < dataParam && gridAtendimento.SelectedIndex == -1)
            {
                Alerta(null, this, "_err1", "Previsão de conclusão não pode ser inferior à data atual.");
                txtDataPrevisao.Focus();
                return;
            }

            atendimento.PropostaID = this.contratoId;
            atendimento.Texto = txtTexto.Text;
            if (txtTexto2.Visible)
            {
                atendimento.Texto += String.Concat("\n-------\n", txtTexto2.Text, "\n[",
                    Usuario.Autenticado.Nome, " - ", DateTime.Now.ToString("dd/MM/yyyy HH:mm"), " - ", cboSubTipoAtendimento.SelectedItem.Text + "]");
            }
            else
            {
                atendimento.Texto += String.Concat("\n[",
                    Usuario.Autenticado.Nome, " - ", DateTime.Now.ToString("dd/MM/yyyy HH:mm"), " - ", cboSubTipoAtendimento.SelectedItem.Text + "]");
            }

            atendimento.Titulo = txtTitulo.Text;

            atendimento.SubTipoID = cboSubTipoAtendimento.SelectedValue;

            #region envia email

            if (cboTipoAtendimento.SelectedIndex > 0)
            {
                atendimento.TipoID = cboTipoAtendimento.SelectedValue;

                AtendimentoTipo tipo = new AtendimentoTipo(cboTipoAtendimento.SelectedValue);
                tipo.Carregar();
                if (!String.IsNullOrEmpty(tipo.Email))
                {
                    //envia email
                    MailMessage msg = new MailMessage(
                        new MailAddress(ConfigurationManager.AppSettings["mailFrom"], ConfigurationManager.AppSettings["mailFromName"]),
                        new MailAddress(tipo.Email));
                    msg.Subject = "[SABE] Atendimento";
                    msg.IsBodyHtml = false;

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("Contrato: "); sb.AppendLine(txtNumeroContrato.Text);
                    sb.Append("Tipo de contrato: "); sb.AppendLine(cboTipoProposta.SelectedItem.Text);
                    sb.Append("Estipulante: "); sb.AppendLine(cboEstipulante.SelectedItem.Text);
                    sb.Append("Operadora: "); sb.AppendLine(cboOperadora.SelectedItem.Text);
                    sb.Append("Contrato ADM: "); sb.AppendLine(cboContrato.SelectedItem.Text);
                    sb.Append("Plano: "); sb.AppendLine(cboPlano.SelectedItem.Text);
                    sb.Append("Acomodação: "); sb.AppendLine(cboAcomodacao.SelectedItem.Text);
                    sb.Append("Admissão: "); sb.AppendLine(txtAdmissao.Text);
                    sb.AppendLine(""); sb.AppendLine(""); sb.AppendLine("");
                    sb.Append("Detalhes do atendimento");
                    sb.AppendLine(""); sb.AppendLine("");
                    sb.AppendLine("Tipo"); sb.AppendLine(tipo.Descricao);
                    sb.AppendLine(""); sb.AppendLine("");
                    sb.AppendLine("Título"); sb.AppendLine(atendimento.Titulo);
                    sb.AppendLine(""); sb.AppendLine("");
                    sb.AppendLine("Texto"); sb.AppendLine(atendimento.Texto);
                    sb.AppendLine(""); sb.AppendLine("");
                    sb.AppendLine("Atendente"); sb.AppendLine(atendimento.IniciadoPor);
                    sb.AppendLine(""); sb.AppendLine("");
                    sb.AppendLine("Data"); sb.AppendLine(atendimento.DataInicio.ToString("dd/MM/yyyy"));
                    sb.AppendLine(""); sb.AppendLine("");
                    sb.AppendLine("Previsão"); sb.AppendLine(atendimento.DataPrevisao.ToString("dd/MM/yyyy"));
                    sb.AppendLine(""); sb.AppendLine("");

                    if (atendimento.DataFim != DateTime.MinValue)
                    {
                        sb.AppendLine("Conclusão"); sb.AppendLine(atendimento.DataFim.ToString("dd/MM/yyyy"));
                        sb.AppendLine(""); sb.AppendLine("");
                        sb.AppendLine("Atendente (conclusão)"); sb.AppendLine(atendimento.ResolvidoPor);
                    }

                    msg.Body = sb.ToString();

                    try
                    {
                        SmtpClient client = new SmtpClient();
                        client.Send(msg);
                        msg.Dispose();
                        client = null;
                    }
                    catch { }
                }
            }
            #endregion

            atendimento.Salvar();

            AtendimentoTempItem item = new AtendimentoTempItem();
            item.AtendimentoID = atendimento.ID;
            item.Data = DateTime.Now;
            item.SubTipoID = atendimento.SubTipoID;

            if (!txtTexto2.ReadOnly && txtTexto2.Visible)
                item.Texto = txtTexto2.Text;
            else
                item.Texto = txtTexto.Text;

            item.TipoID = atendimento.TipoID;
            item.UsuarioID = Usuario.Autenticado.ID;
            item.Salvar();

            txtTexto2.Visible = false;
            txtTexto.ReadOnly = false;
            cmdSalvarAtendimento.Enabled = true;

            Alerta(null, this, "_atendOk", "Atendimento gravado com sucesso!");
            this.LimparCamposAtendimento();
            this.CarregarAtendimentos();
            txtTexto.Focus();
        }

        protected void cmdFecharAtendimento_Click(Object sender, EventArgs e)
        {
            this.LimparCamposAtendimento();
            cboTipoAtendimento_Change(null, null);
            txtTexto.ReadOnly = false;
            txtTexto2.Visible = false;
            txtTexto.Focus();
            cmdSalvarAtendimento.Enabled = true;
        }

        String dateToString(DateTime date)
        {
            if (date == DateTime.MinValue)
                return String.Empty;
            else
                return date.ToString("dd/MM/yyyy");
        }

        void exibeAtendimento(AtendimentoTemp atendimento)
        {
            if (atendimento.TipoID != null)
                cboTipoAtendimento.SelectedValue = Convert.ToString(atendimento.TipoID);
            else
                cboTipoAtendimento.SelectedIndex = 0;

            lblAtendimentoProtocolo.Text = Convert.ToString(atendimento.ID);
            txtTitulo.Text = atendimento.Titulo;
            txtTexto.Text = atendimento.Texto;
            txtTexto.ReadOnly = true;
            txtTexto2.Visible = true;
            chkAtendimentoConcluido.Checked = !String.IsNullOrEmpty(atendimento.ResolvidoPor); //txtDataConclusao.Text = dateToString(atendimento.DataFim);
            txtDataInicio.Text = dateToString(atendimento.DataInicio);
            txtDataPrevisao.Text = dateToString(atendimento.DataPrevisao);

            if (atendimento.IniciadoPor != null && atendimento.IniciadoPor.Length > 32) { atendimento.IniciadoPor = atendimento.IniciadoPor.Substring(0, 31); }
            litCriadoPor.Text = "por: " + atendimento.IniciadoPor;

            if (atendimento.SubTipoID != null)
                cboSubTipoAtendimento.SelectedValue = Convert.ToString(atendimento.SubTipoID);
            else
                cboSubTipoAtendimento.SelectedIndex = 0;

            txtDataInicio.ReadOnly = true;
            imgDataInicio.Visible = false;

            txtTexto2.Focus();

            if (!String.IsNullOrEmpty(atendimento.ResolvidoPor))
            {
                txtDataPrevisao.ReadOnly = true;
                imgDataPrevisao.Visible = false;

                chkAtendimentoConcluido.Enabled = false;

                if (atendimento.ResolvidoPor.Length > 42) { atendimento.ResolvidoPor = atendimento.ResolvidoPor.Substring(0, 41); }
                litResolvidoPor.Text = String.Concat("por ", atendimento.ResolvidoPor, " em ", atendimento.DataFim.ToString("dd/MM/yyyy HH:mm"));
                cmdSalvarAtendimento.Enabled = false;
            }
            else
            {
                txtDataPrevisao.ReadOnly = false;
                imgDataPrevisao.Visible = true;

                chkAtendimentoConcluido.Enabled = true;

                cmdSalvarAtendimento.Enabled = true;
            }
        }

        protected void gridAtendimento_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "detalhe")
            {
                this.LimparCamposAtendimento();
                Object id = gridAtendimento.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                gridAtendimento.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                AtendimentoTemp atendimento = new AtendimentoTemp(id);
                atendimento.Carregar();

                this.exibeAtendimento(atendimento);
            }
        }

        protected void gridAtendimento_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
        }

        protected void gridAtendimento_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            gridAtendimento.PageIndex = e.NewPageIndex;
            this.CarregarAtendimentos();
        }

        ////////////////////////////////

        protected void cmdFecharPnlAtendimento_Click(Object sender, EventArgs e)
        {
            pnlAtendimento.Visible = false;
        }

        IList<Cobranca> CarregarCobrancas()
        {
            if (this.contratoId == null) { return null; }
            IList<Cobranca> cobrancas = Cobranca.CarregarTodasComParcelamentoInfo_Composite(this.contratoId, true, true, null);
            gridCobranca.DataSource = cobrancas; //NEGOCIACAO ComParcelamentoInfo
            gridCobranca.DataBind();
            return cobrancas;
        }

        protected void cmdAtendimentoVerParcelas_Click(Object sender, EventArgs e)
        {
            this.CarregarCobrancas();
        }

        protected void cmdRecalcularComposicao_click(Object sender, EventArgs e)
        {
            LC.Framework.Phantom.PersistenceManager pm = new LC.Framework.Phantom.PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                List<CobrancaComposite> composite = null;
                Cobranca cobranca = new Cobranca(txtIdCobrancaEmDetalhe.Text);
                pm.Load(cobranca);
                if (cobranca.Tipo == (int)Cobranca.eTipo.Parcelamento) //NEGOCIACAO
                {
                    pm.Rollback();
                    pm.Dispose();
                    this.exibeDetalheCobranca(cobranca);
                    return;
                }

                Contrato.CalculaValorDaProposta2(cobranca.PropostaID,
                    cobranca.DataVencimento, pm, true, true, ref composite, true);

                if (composite != null && composite.Count > 0)
                {
                    CobrancaComposite.Remover(cobranca.ID, pm);
                    foreach (CobrancaComposite comp in composite)
                    {
                        comp.CobrancaID = cobranca.ID;
                        pm.Save(comp);
                    }
                }
                else
                {
                    base.Alerta(null, this, "_msg", "Não foi possível recalcular a composição.");
                }

                pm.Commit();

                this.exibeDetalheCobranca(cobranca);
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm.Dispose();
            }
        }

        protected void cmdRelacaoCobrancas_Click(Object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this,
                    this.GetType(),
                    "_abrirRelacao_",
                    String.Concat(" window.open(\"relacaoCobrancas.aspx?", IDKey, "=", this.contratoId, "\", \"janela\", \"toolbar=no,scrollbars=1\"); "), //
                    true);
        }

        protected void cmdTabelaValor_Click(Object sender, EventArgs e)
        {
            String param = "?" + IDKey + "=";
            if (cboOperadora.SelectedIndex > 0) { param += cboOperadora.SelectedValue; }
            if (cboContrato.SelectedIndex > 0) { param += "&" + IDKey2 + "=" + cboContrato.SelectedValue; }
            if (cboPlano.SelectedIndex > 0) { param += "&" + IDKey3 + "=" + cboPlano.SelectedValue; }

            ScriptManager.RegisterClientScriptBlock(this,
                    this.GetType(),
                    "_abrirRelacao_",
                    String.Concat(" window.open(\"tabelavalorPoup.aspx", param, "\", \"janela\", \"toolbar=no,scrollbars=1\"); "), //
                    true);
        }

        //-------//

        protected void cmdDemonsPagto_Click(Object sender, EventArgs e)
        {
            if (this.contratoId == null) { return; }

            /////////////////
            //DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ir_dados_preprod_sp (nolock) where UTILIZAR_REGISTRO = 1 and ENVIAR_DMED = 1 and idcedente=2 and idproposta=" + this.contratoId, "result", null).Tables[0];
            //if (dados.Rows.Count == 0)
            //{
            //    dados.Dispose();
            //    return;
            //}

            ////DataRow[] ret = dados.Select("UTILIZAR_REGISTRO = 0 OR ENVIAR_DMED = 0");
            ////if (ret.Length > 0)
            ////{
            ////    dados.Dispose();
            ////    return;
            ////}
            //dados.Dispose();
            /////////////////

            //String ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"];

            string ano = "2016";

            ScriptManager.RegisterClientScriptBlock(this,
                    this.GetType(),
                    "_abrir_",
                    String.Concat(" window.open(\"../demonstPagtos.aspx?ano=", ano, "&", IDKey, "=", this.contratoId, "\", \"janela\", \"toolbar=no,scrollbars=1\"); "),
                    true);
        }

        protected void imgDemonstPagtoMail_Click(Object sender, EventArgs e)
        {
            if (this.contratoId == null) { return; }
            if (String.IsNullOrEmpty(txtEmailAtendimento.Text.Trim())) { return; }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            String ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"];
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ir_dados_preprod_sp where UTILIZAR_REGISTRO = 1 AND ENVIAR_DMED = 1 AND idcedente=2 and idproposta=" + contratoId, "result", pm).Tables[0];
            if (dados.Rows.Count == 0)
            {
                dados.Dispose();
                //Alerta(null, this, "err", "Demonstrativo indisponível.");
                pm.CloseSingleCommandInstance();
                pm.Dispose();
                return;
            }

            //DataRow[] ret = dados.Select("UTILIZAR_REGISTRO = 0 OR ENVIAR_DMED = 0");
            //if (ret.Length > 0)
            //{
            //    dados.Dispose();
            //    //Alerta(null, this, "err", "Demonstrativo indisponível.");
            //    pm.CloseSingleCommandInstance();
            //    pm.Dispose();
            //    return;
            //}

            dados.Dispose();

            string titularContratoBeneficiarioId = "";
            List<String> dependContratoBeneficiarioIds = new List<string>();

            foreach (DataRow row in dados.Rows)
            {
                if (base.CToString(row["SEQUENCIA"]) == "0") { titularContratoBeneficiarioId = base.CToString(row["IDPROPONENTE"]); continue; }

                dependContratoBeneficiarioIds.Add(base.CToString(row["IDPROPONENTE"]));
            }

            #region corpo do e-mail

            Contrato contrato = Contrato.CarregarParcial((Object)contratoId, pm);
            Operadora operadora = new Operadora(contrato.OperadoraID);
            pm.Load(operadora);
            ContratoBeneficiario cTitular = ContratoBeneficiario.CarregarPorIDContratoBeneficiario(titularContratoBeneficiarioId, pm);

            //if (cTitular.DMED == false)
            //{
            //    Alerta(null, this, "err", "Titular com pendências DMED.");
            //    pm.CloseSingleCommandInstance();
            //    pm.Dispose();
            //    return;
            //}

            sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
            sb.Append("<head><title></title>");
            sb.Append("<style type='text/css'>body, html{ font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:#b8b8b8; font-size:11px; background-color:white; margin:0px; height:100%; }link              { font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:blue; font-size:11px; } table             { font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:black; font-size:11px; }</style>");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append("<table align=\"center\" width=\"95%\">");
            sb.Append(" <tr>");
            sb.Append(" <td>");
            sb.Append("<table><tr><td><h2>Demonstrativo de Pagamentos "); sb.Append(ano); sb.Append("</td><td width='35'>&nbsp;</td><td align='left'><img align=\"right\" src='http://www.linkecerebro.com.br/LogoMail.png' /></h2></td></tr></table>");
            sb.Append(" <table style=\"font-size:12px\">");
            sb.Append(" <tr>");
            sb.Append(" <td colspan=\"2\">");
            sb.Append(String.Concat("São Paulo, ", DateTime.Now.Day, " de ", DateTime.Now.ToString("MMMM"), " de ", DateTime.Now.Year, "."));
            sb.Append(" </td>");
            sb.Append(" </tr>");
            sb.Append(" <tr><td height='5px'></td></tr>");

            if (!string.IsNullOrEmpty(contrato.ResponsavelNome) && !string.IsNullOrEmpty(contrato.ResponsavelCPF))
            {
                sb.Append(" <tr><td width=\"140px\"><b>Ilmo(a). Senhor(a)</b></td>"); sb.Append("<td>"); sb.Append(contrato.ResponsavelNome); sb.Append("</td></tr>");
                sb.Append(" <tr><td><b>CPF:</b></td><td>"); sb.Append(contrato.ResponsavelCPF); sb.Append("</td></tr>");
            }
            else
            {
                sb.Append(" <tr><td width=\"140px\"><b>Ilmo(a). Senhor(a)</b></td>"); sb.Append("<td>"); sb.Append(cTitular.BeneficiarioNome); sb.Append("</td></tr>");
                sb.Append(" <tr><td><b>CPF:</b></td><td>"); sb.Append(cTitular.BeneficiarioCPF); sb.Append("</td></tr>");
            }
            sb.Append("</table><br />");

            sb.Append("<table style=\"font-size:12px\"><tr><td><b>Cliente PS Padrão,</b></td></tr><tr><td height='8'></td></tr><tr><td>");
            sb.Append("Abaixo o demonstrativo de pagamentos efetuados, durante o ano calendário de "); sb.Append(ano); sb.Append(", à PS Padrão ");
            sb.Append("Administradora de Benefícios Ltda., inscrita no CNPJ/MF sob o nº 11.273.573/0001-05, e destinados à ");
            sb.Append("manutenção do plano privado de assistência à saúde, coletivo por adesão, por meio de contrato coletivo ");
            sb.Append("firmado com a operadora [operadora].<br />");
            sb.Append("Esse demonstrativo relaciona as despesas médicas que foram pagas pelo(a) Sr(a). e que são dedutíveis em ");
            sb.Append("Declaração de Imposto de Renda.");
            sb.Append("</td></tr></table></td></tr></table><br />");

            #region MESES

            Decimal total = 0, totalJan = 0, totalFev = 0, totalMar = 0, totalAbr = 0, totalMaio = 0, totalJun = 0, totalJul = 0, totalAgo = 0, totalSet = 0;

            totalJan = base.CToDecimal(dados.Compute("SUM(JAN)", ""));
            totalFev = base.CToDecimal(dados.Compute("SUM(FEV)", ""));
            totalMar = base.CToDecimal(dados.Compute("SUM(MAR)", ""));
            totalAbr = base.CToDecimal(dados.Compute("SUM(ABR)", ""));
            totalMaio = base.CToDecimal(dados.Compute("SUM(MAI)", ""));
            totalJun = base.CToDecimal(dados.Compute("SUM(JUN)", ""));
            totalJul = base.CToDecimal(dados.Compute("SUM(JUL)", ""));
            totalAgo = base.CToDecimal(dados.Compute("SUM(AGO)", ""));
            totalSet = base.CToDecimal(dados.Compute("SUM(SETEM)", ""));

            total = totalJan + totalFev + totalMar + totalAbr + totalMaio + totalJun + totalJul + totalAgo + totalSet;

            sb.Append("<table align='center' cellpadding=\"4\" cellspacing=\"0\" style=\"border:solid 1px black;font-size:12px\" width=\"400px\"><tr><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Competência</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>*Valor mensal</b></td></tr>");

            sb.Append("<tr><td>Janeiro</td><td>");
            sb.Append(totalJan.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Fevereiro</td><td>");
            sb.Append(totalFev.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Março</td><td>");
            sb.Append(totalMar.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Abril</td><td>");
            sb.Append(totalAbr.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Maio</td><td>");
            sb.Append(totalMaio.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Junho</td><td>");
            sb.Append(totalJun.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Julho</td><td>");
            sb.Append(totalJul.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Agosto</td><td>");
            sb.Append(totalAgo.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Setembro</td><td>");
            sb.Append(totalSet.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>*Valor total</td><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>" + total.ToString("N2") + "</td></tr>");
            sb.Append("<tr><td colspan=\"2\" style=\"border-top:solid 1px black;font-size:11px\"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td></tr></table>");
            sb.Append("<br /><br />");

            #endregion MESES

            sb.Append("<center><div style=\"color:black\"><b>COMPOSIÇÃO DO GRUPO FAMILIAR</b></div></center><table align='center' cellpadding=\"4\" cellspacing=\"0\" style=\"border:solid 1px black;font-size:12px\" width=\"400px\"><tr><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Condição</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Nome</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>*Valor por beneficiário(a)</b></td></tr>");

            IList<ContratoBeneficiario> cbeneficiarios = null;
            if (dependContratoBeneficiarioIds != null && dependContratoBeneficiarioIds.Count > 0)
                cbeneficiarios = ContratoBeneficiario.Carregar(dependContratoBeneficiarioIds.ToArray(), pm);

            cTitular.Valor = base.CToDecimal(dados.Compute(
                "sum(JAN)+sum(FEV)+sum(MAR)+sum(ABR)+sum(MAI)+sum(JUN)+sum(JUL)+sum(AGO)+sum(SETEM)", "IDPROPONENTE=" + cTitular.ID));

            if (cbeneficiarios != null)
            {
                foreach (ContratoBeneficiario cb in cbeneficiarios)
                {
                    cb.Valor = base.CToDecimal(dados.Compute(
                        "sum(JAN)+sum(FEV)+sum(MAR)+sum(ABR)+sum(MAI)+sum(JUN)+sum(JUL)+sum(AGO)+sum(SETEM)", "IDPROPONENTE=" + cb.ID));
                }
            }
            else
                cbeneficiarios = new List<ContratoBeneficiario>();

            cbeneficiarios.Insert(0, cTitular);

            foreach (ContratoBeneficiario cb in cbeneficiarios)
            {
                if (cb.Valor > 0)
                {
                    sb.Append("<tr>");

                    sb.Append("<td>");
                    if (cb.Tipo == 0)
                        sb.Append("Titular");
                    else
                        sb.Append("Dependente");
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(cb.BeneficiarioNome);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(cb.Valor.ToString("N2"));
                    sb.Append("</td>");

                    sb.Append("</tr>");
                }
            }
            sb.Append("<tr><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>*Valor total</td><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke' colspan=\"2\">" + total.ToString("N2") + "</td></tr>");
            sb.Append("<tr><td style=\"border-top:solid 1px black;font-size:11px\" colspan=\"3\"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td></tr></table><br /><br />");

            sb.Append("<table align=\"center\" width=\"95%\"><tr><td>Atenção: Caso este informe seja utilizado para fins de declaração de Imposto de Renda, esclarecemos que somente podem ser deduzidas as parcelas relativas ao contribuinte e aos dependentes devidamente relacionados na própria declaração. As deduções estão sujeitas às regras estabelecidas pela legislação que regulamenta o imposto (Decreto nº 3.000/99).</td></tr><tr><td height='8'></td></tr><tr><td><b>PS Padrão Administradora de Benefícios</b></td></tr></table>");

            sb.Append("<br><br><font size='1' color='red'>Este é um e-mail automático. Por favor, não o responda.</font>");
            sb.Append("</body>");
            sb.Append("</html>");


            String corpo = sb.ToString();
            if (cboOperadora.SelectedItem.Text.IndexOf("-") > -1)
                corpo = corpo.Replace("[operadora]", cboOperadora.SelectedItem.Text.Split('-')[1].Trim());
            else
                corpo = corpo.Replace("[operadora]", cboOperadora.SelectedItem.Text.Trim());

            #endregion corpo do e-mail

            //envia email
            MailMessage msg = new MailMessage(
                new MailAddress(ConfigurationManager.AppSettings["mailFrom"], ConfigurationManager.AppSettings["mailFromName"]),
                new MailAddress(txtEmailAtendimento.Text));
            msg.Subject = "Demonstrativo de Pagamentos " + ano;
            msg.IsBodyHtml = true;
            msg.Body = corpo;

            try
            {
                SmtpClient client = new SmtpClient();
                client.Send(msg);
                msg.Dispose();
                client = null;
                base.Alerta(null, this, "_ok", "E-mail enviado com sucesso.");
            }
            catch { }
        }

        protected void cmdDemonsPagtoQuali_Click(Object sender, EventArgs e)
        {
            if (this.contratoId == null) { return; }

            //IList<ContratoBeneficiario> benefs = ContratoBeneficiario.CarregarPorContratoID_Parcial(this.contratoId, false, false, null);
            //if (benefs != null)
            //{
            //    foreach (ContratoBeneficiario cb in benefs)
            //    {
            //        if (!cb.DMED)
            //        {
            //            Alerta(null, this, "err", "Há um ou mais beneficiários com pendência DMED.");
            //            return;
            //        }
            //    }
            //}

            /////////////////////////////////////////
            DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ir_dados_preprod_sp (nolock) where UTILIZAR_REGISTRO = 1 and ENVIAR_DMED = 1 and idcedente=1 and idproposta=" + contratoId, "result", null).Tables[0];
            if (dados.Rows.Count == 0)
            {
                dados.Dispose();
                //Alerta(null, this, "err", "Demonstrativo indisponível.");
                return;
            }
            //DataRow[] ret = dados.Select("UTILIZAR_REGISTRO = 0 OR ENVIAR_DMED = 0");
            //if (ret.Length > 0)
            //{
            //    dados.Dispose();
            //    //Alerta(null, this, "err", "Demonstrativo indisponível.");
            //    return;
            //}
            dados.Dispose();
            /////////////////////////////////////////

            String ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"];

            ScriptManager.RegisterClientScriptBlock(this,
                    this.GetType(),
                    "_abrir_",
                    String.Concat(" window.open(\"demonstPagtosQuali.aspx?ano=", ano, "&", IDKey, "=", this.contratoId, "\", \"janela\", \"toolbar=no,scrollbars=1\"); "),
                    true);
        }

        protected void imgDemonstPagtoQualiMail_Click(Object sender, EventArgs e)
        {
            if (this.contratoId == null) { return; }
            if (String.IsNullOrEmpty(txtEmailAtendimento.Text.Trim())) { return; }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            String ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"];
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            //TODO: confirmar as condicoes para saber se está aprovado na DMED
            DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ir_dados_preprod_sp where UTILIZAR_REGISTRO = 1 AND ENVIAR_DMED = 1 and idcedente=1 and idproposta=" + contratoId, "result", pm).Tables[0];
            if (dados.Rows.Count == 0)
            {
                dados.Dispose();
                pm.CloseSingleCommandInstance();
                pm.Dispose();
                return;
            }
            //DataRow[] ret = dados.Select("UTILIZAR_REGISTRO = 0 OR ENVIAR_DMED = 0");
            //if (ret.Length > 0)
            //{
            //    dados.Dispose();
            //    pm.CloseSingleCommandInstance();
            //    pm.Dispose();
            //    return;
            //}
            dados.Dispose();

            string titularContratoBeneficiarioId = "";
            List<String> dependContratoBeneficiarioIds = new List<string>();

            foreach (DataRow row in dados.Rows)
            {
                if (base.CToString(row["SEQUENCIA"]) == "0") { titularContratoBeneficiarioId = base.CToString(row["IDPROPONENTE"]); continue; }

                dependContratoBeneficiarioIds.Add(base.CToString(row["IDPROPONENTE"]));
            }

            #region corpo do e-mail

            Contrato contrato = Contrato.CarregarParcial((Object)contratoId, pm);
            Operadora operadora = new Operadora(contrato.OperadoraID);
            pm.Load(operadora);
            ContratoBeneficiario cTitular = ContratoBeneficiario.CarregarPorIDContratoBeneficiario(titularContratoBeneficiarioId, pm);

            //if (cTitular.DMED == false)
            //{
            //    Alerta(null, this, "err", "Titular com pendências DMED.");
            //    pm.CloseSingleCommandInstance();
            //    pm.Dispose();
            //    return;
            //}

            #region cobrancas

            //List<CobrancaComposite> comp = new List<CobrancaComposite>();
            //IList<Cobranca> temp = Cobranca.CarregarTodas(contrato.ID, true, pm);
            //List<Cobranca> cobrancas = new List<Cobranca>();
            //ContratoBeneficiario depend = null;
            //foreach (Cobranca cob in temp)
            //{
            //    //if (cob.DataVencimento.Year.ToString() != ano || !cob.Pago || cob.Parcela == 1 || cob.DataVencimento.Month <= 9) 
            //    if (cob.DataPgto.Year.ToString() != ano || !cob.Pago || cob.Parcela == 1 || cob.DataPgto.Month <= 9 || cob.ValorPgto == 1) 
            //    { continue; }

            //    ///////demonstrativo
            //    cob.Valor = Contrato.CalculaValorDaProposta_TODOS(
            //            contrato.ID, cob.DataVencimento, pm, false, false, ref comp, false);

            //    CobrancaComposite.Remover(cob.ID, pm);
            //    CobrancaComposite.Salvar(cob.ID, comp, pm);

            //    icomp = CobrancaComposite.Carregar(cob.ID, pm);
            //    if (icomp != null && icomp.Count > 0)
            //    {
            //        cob.Valor = 0;
            //        foreach (CobrancaComposite item in icomp)
            //        {
            //            if (item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Plano ||
            //               item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Adicional)
            //            {
            //                if (item.BeneficiarioID == null || Convert.ToString(item.BeneficiarioID) == Convert.ToString(cTitular.BeneficiarioID))
            //                    cob.Valor += item.Valor;
            //                else
            //                {
            //                    depend = ContratoBeneficiario.CarregarPorContratoEBeneficiario(contrato.ID, item.BeneficiarioID, pm);
            //                    depend.DataInativacao = new DateTime(depend.DataInativacao.Year, depend.DataInativacao.Month, depend.DataInativacao.Day, 23, 59, 59, 998);
            //                    if (depend.DMED && (depend.Ativo || depend.DataInativacao == DateTime.MinValue || depend.DataInativacao >= cob.DataVencimento)) //if (depend.Ativo || depend.DataInativacao == DateTime.MinValue || depend.DataInativacao >= cob.DataVencimento)
            //                        cob.Valor += item.Valor;
            //                }
            //            }
            //        }
            //    }

            //    cobrancas.Add(cob);
            //}
            #endregion

            sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
            sb.Append("<head><title></title>");
            sb.Append("<style type='text/css'>body, html{ font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:#b8b8b8; font-size:11px; background-color:white; margin:0px; height:100%; }link              { font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:blue; font-size:11px; } table             { font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:black; font-size:11px; }</style>");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append("<table align=\"center\" width=\"95%\">");
            sb.Append(" <tr>");
            sb.Append(" <td>");
            sb.Append("<table><tr><td><h2>Demonstrativo de Pagamentos "); sb.Append(ano); sb.Append("</td><td width='35'>&nbsp;</td><td align='left'><img align=\"right\" src='http://www.linkecerebro.com.br/LogoMail.png' /></h2></td></tr></table>");
            sb.Append(" <table style=\"font-size:12px\">");
            sb.Append(" <tr>");
            sb.Append(" <td colspan=\"2\">");
            sb.Append(String.Concat("São Paulo, ", DateTime.Now.Day, " de ", DateTime.Now.ToString("MMMM"), " de ", DateTime.Now.Year, "."));
            sb.Append(" </td>");
            sb.Append(" </tr>");
            sb.Append(" <tr><td height='5px'></td></tr>");

            if (!string.IsNullOrEmpty(contrato.ResponsavelNome) && !string.IsNullOrEmpty(contrato.ResponsavelCPF))
            {
                sb.Append(" <tr><td width=\"140px\"><b>Ilmo(a). Senhor(a)</b></td>"); sb.Append("<td>"); sb.Append(contrato.ResponsavelNome); sb.Append("</td></tr>");
                sb.Append(" <tr><td><b>CPF:</b></td><td>"); sb.Append(contrato.ResponsavelCPF); sb.Append("</td></tr>");
            }
            else
            {
                sb.Append(" <tr><td width=\"140px\"><b>Ilmo(a). Senhor(a)</b></td>"); sb.Append("<td>"); sb.Append(cTitular.BeneficiarioNome); sb.Append("</td></tr>");
                sb.Append(" <tr><td><b>CPF:</b></td><td>"); sb.Append(cTitular.BeneficiarioCPF); sb.Append("</td></tr>");
            }

            sb.Append("</table><br />");

            sb.Append("<table style=\"font-size:12px\"><tr><td><b>Cliente Qualicorp,</b></td></tr><tr><td height='8'></td></tr><tr><td>");
            sb.Append("Abaixo o demonstrativo de pagamentos efetuados, durante o ano calendário de "); sb.Append(ano); sb.Append(", à Qualicorp ");
            sb.Append("Administradora de Benefícios LTDA., inscrita no CNPJ/MF sob o nº 07.658.098/0001-18, e destinados à ");
            sb.Append("manutenção do plano privado de assistência à saúde, coletivo por adesão, por meio de contrato coletivo ");
            sb.Append("firmado com a operadora [operadora].<br />");
            sb.Append("Esse demonstrativo relaciona as despesas médicas que foram pagas pelo(a) Sr(a). e que são dedutíveis em ");
            sb.Append("Declaração de Imposto de Renda.");
            sb.Append("</td></tr></table></td></tr></table><br />");

            #region MESES

            Decimal total = 0, totalOut = 0, totalNov = 0, totalDez = 0;
            sb.Append("<table align='center' cellpadding=\"4\" cellspacing=\"0\" style=\"border:solid 1px black;font-size:12px\" width=\"400px\"><tr><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Competência</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>*Valor mensal</b></td></tr>");

            totalOut = base.CToDecimal(dados.Compute("SUM(OUTU)", ""));
            totalNov = base.CToDecimal(dados.Compute("SUM(NOV)", ""));
            totalDez = base.CToDecimal(dados.Compute("SUM(DEZ)", ""));

            total = totalOut + totalNov + totalDez;

            sb.Append("<tr><td>Outubro</td><td>");
            sb.Append(totalOut.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Novembro</td><td>");
            sb.Append(totalNov.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Dezembro</td><td>");
            sb.Append(totalDez.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>*Valor total</td><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>" + total.ToString("N2") + "</td></tr>");

            sb.Append("<tr><td colspan=\"2\" style=\"border-top:solid 1px black;font-size:11px\"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td></tr></table>");
            sb.Append("<br /><br />");

            #endregion MESES

            sb.Append("<center><div style=\"color:black\"><b>COMPOSIÇÃO DO GRUPO FAMILIAR</b></div></center><table align='center' cellpadding=\"4\" cellspacing=\"0\" style=\"border:solid 1px black;font-size:12px\" width=\"400px\"><tr><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Condição</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Nome</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>*Valor por beneficiário(a)</b></td></tr>");

            IList<ContratoBeneficiario> cbeneficiarios = null;
            if (dependContratoBeneficiarioIds != null && dependContratoBeneficiarioIds.Count > 0)
                cbeneficiarios = ContratoBeneficiario.Carregar(dependContratoBeneficiarioIds.ToArray(), pm);

            cTitular.Valor = base.CToDecimal(dados.Compute("sum(OUTU)+sum(NOV)+sum(DEZ)", "IDPROPONENTE=" + cTitular.ID));

            if (cbeneficiarios != null)
            {
                foreach (ContratoBeneficiario cb in cbeneficiarios)
                {
                    cb.Valor = base.CToDecimal(dados.Compute("sum(OUTU)+sum(NOV)+sum(DEZ)", "IDPROPONENTE=" + cb.ID));
                }
            }
            else
                cbeneficiarios = new List<ContratoBeneficiario>();

            cbeneficiarios.Insert(0, cTitular);

            foreach (ContratoBeneficiario cb in cbeneficiarios)
            {
                //if (!cb.DMED)
                //{
                //    Alerta(null, this, "err", "Beneficiário com pendências DMED");
                //    return;
                //}

                if (cb.Valor > 0)
                {
                    sb.Append("<tr>");

                    sb.Append("<td>");
                    if (cb.Tipo == 0)
                        sb.Append("Titular");
                    else
                        sb.Append("Dependente");
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(cb.BeneficiarioNome);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(cb.Valor.ToString("N2"));
                    sb.Append("</td>");

                    sb.Append("</tr>");
                }
            }
            sb.Append("<tr><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>*Valor total</td><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke' colspan=\"2\">" + total.ToString("N2") + "</td></tr>");
            sb.Append("<tr><td style=\"border-top:solid 1px black;font-size:11px\" colspan=\"3\"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td></tr></table><br /><br />");

            sb.Append("<table align=\"center\" width=\"95%\"><tr><td>Atenção: Caso este informe seja utilizado para fins de declaração de Imposto de Renda, esclarecemos que somente podem ser deduzidas as parcelas relativas ao contribuinte e aos dependentes devidamente relacionados na própria declaração. As deduções estão sujeitas às regras estabelecidas pela legislação que regulamenta o imposto (Decreto nº 3.000/99).</td></tr><tr><td height='8'></td></tr><tr><td><b>Qualicorp Administradora de Benefícios</b></td></tr></table>");

            sb.Append("<br><br><font size='1' color='red'>Este é um e-mail automático. Por favor, não o responda.</font>");
            sb.Append("</body>");
            sb.Append("</html>");


            String corpo = sb.ToString();

            if (cboOperadora.SelectedItem.Text.IndexOf("-") > -1)
                corpo = corpo.Replace("[operadora]", cboOperadora.SelectedItem.Text.Split('-')[1].Trim());
            else
                corpo = corpo.Replace("[operadora]", cboOperadora.SelectedItem.Text.Trim());

            #endregion corpo do e-mail

            //envia email
            MailMessage msg = new MailMessage(
                new MailAddress(ConfigurationManager.AppSettings["mailFrom"], ConfigurationManager.AppSettings["mailFromName"]),
                new MailAddress(txtEmailAtendimento.Text));
            msg.Subject = "Demonstrativo de Pagamentos " + ano;
            msg.IsBodyHtml = true;
            msg.Body = corpo;

            try
            {
                SmtpClient client = new SmtpClient();
                client.Send(msg);
                msg.Dispose();
                client = null;
                base.Alerta(null, this, "_ok", "E-mail enviado com sucesso.");
            }
            catch { }
        }

        public void enviaEmailsViaPost()
        {
            txtEmailAtendimento.Text = Request["email"];
        }

        //-------//

        protected void cmdCartaDePermanecia_Click(Object sender, EventArgs e)
        {
            if (this.contratoId == null) { return; }
            if (String.IsNullOrEmpty(txtEmailAtendimento.Text.Trim())) { return; }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            #region corpo do e-mail

            sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
            sb.Append("<head><title></title></head>");
            sb.Append("<body>");
            sb.Append(" <br />");
            sb.Append(" <p>[dataagora]</p>");
            sb.Append(" <br />");
            //sb.Append(" <table style=\"width:750px\"><tr><td>");

            //sb.Append(" </td></tr></table>");
            //sb.Append(" <br /><br />");
            sb.Append(" <table style=\"text-align:center;width:750px\">");
            sb.Append("     <tr>");
            sb.Append("         <td>");
            sb.Append("     <h3>DECLARAÇÃO DE PERMANÊNCIA</h3>");
            sb.Append("         <td>");
            sb.Append("         <td><img align=\"right\" src='http://www.linkecerebro.com.br/LogoMail.png' /></td>");
            sb.Append("     <tr>");
            sb.Append(" </table>");
            sb.Append(" <br /><br />");
            sb.Append(" <table style=\"text-align:justify;text-justify:inter-word;width:750px\">");
            sb.Append("     <tr>");
            sb.Append("         <td>");
            sb.Append("     Na qualidade de administradora do seu plano de assistência à saúde, coletivo por adesão mantido pela Padrão ");
            sb.Append("     Seguros Administradora de Benefícios junto com a [operadora] ");
            sb.Append("     em convênio com a [estipulante], declaramos para devidos fins de direito que ");
            sb.Append("     o(s) beneficiários(s) abaixo está(ão) inscrito(s) no plano [plano] ([acomodacao]).");
            sb.Append("         <td>");
            sb.Append("     <tr>");
            sb.Append(" </table>");
            sb.Append(" <p>");
            sb.Append("     Informamos ainda que todas as mensalidades do referido plano estão quitadas até [dataquitado].");
            sb.Append(" </p>");
            sb.Append(" <table width=\"750\" cellpadding=\"3\" cellspacing=\"0\" style=\"border:solid 1px black\">");
            sb.Append("     <tr>");
            sb.Append("         <th align=\"left\">Nome do beneficiário</th>");
            sb.Append("         <th align=\"left\">Vigência</th>");
            sb.Append("         <th align=\"left\">Código de Identificação</th>");
            sb.Append("         <th align=\"left\">Condição</th>");
            sb.Append("     </tr>");
            sb.Append("     [beneficiarios]");
            sb.Append(" </table>");
            sb.Append(" <br />");
            sb.Append(" <p>");
            sb.Append("     Acrescentamos que este plano privado de assistência à saúde coletivo atende às normas e à legislação aplicável.");
            sb.Append(" </p>");
            sb.Append(" <br />");
            sb.Append(" <p>");
            sb.Append("     Atenciosamente,<br /><br />");
            sb.Append("     PS Padrão Administradora de Benefícios Ltda.<br /><br />");
            sb.Append("     CNPJ : 11.273.573/0001-05<br /><br />");
            sb.Append("     <span>ANS nº 417172</span>");
            sb.Append(" </p>");
            sb.Append("<br><br><font size='1' color='red'>Este é um e-mail automático. Por favor, não o responda.</font>");
            sb.Append("</body>");
            sb.Append("</html>");

            Plano plano = new Plano(cboPlano.SelectedValue);
            plano.Carregar();

            String corpo = sb.ToString();
            corpo = corpo.Replace("[operadora]", cboOperadora.SelectedItem.Text.Split('-')[1].Trim());
            corpo = corpo.Replace("[dataagora]", String.Concat("São Paulo, ", DateTime.Now.ToLongDateString(), "."));
            corpo = corpo.Replace("[estipulante]", cboEstipulante.SelectedItem.Text);
            corpo = corpo.Replace("[plano]", plano.Descricao);
            corpo = corpo.Replace("[acomodacao]", cboAcomodacao.SelectedItem.Text.ToLower());

            IList<ContratoBeneficiario> beneficiarios =
                ContratoBeneficiario.CarregarPorContratoID(this.contratoId, true, false);

            System.Text.StringBuilder sbBenef = new System.Text.StringBuilder();
            foreach (ContratoBeneficiario cb in beneficiarios)
            {
                sbBenef.Append("<tr>");

                sbBenef.Append("<td align=\"left\">");
                sbBenef.Append(cb.BeneficiarioNome);
                sbBenef.Append("</td>");

                sbBenef.Append("<td align=\"left\">");
                sbBenef.Append(cb.Data.ToString("dd/MM/yyyy"));
                sbBenef.Append("</td>");

                sbBenef.Append("<td align=\"left\">");
                sbBenef.Append(cb.NumeroMatriculaSaude);
                sbBenef.Append("</td>");

                sbBenef.Append("<td align=\"left\">");
                if (cb.NumeroSequencial == 0)
                    sbBenef.Append("Titular");
                else
                    sbBenef.Append("Dependente");
                sbBenef.Append("</td>");

                sbBenef.Append("</tr>");
            }

            corpo = corpo.Replace("[beneficiarios]", sbBenef.ToString());

            IList<Cobranca> cobrancas = Cobranca.CarregarTodas(this.contratoId, true, Cobranca.eTipo.Normal, null);
            DateTime dataQuitadoPago = DateTime.MinValue, dataQuitadoEmAberto = DateTime.MinValue;
            if (cobrancas != null)
            {
                foreach (Cobranca cobranca in cobrancas)
                {
                    if (cobranca.Pago)
                        dataQuitadoPago = cobranca.DataVencimento;
                    else
                    {
                        dataQuitadoEmAberto = cobranca.DataVencimento;
                        break;
                    }
                }
            }

            if (dataQuitadoPago != DateTime.MinValue)
                corpo = corpo.Replace("[dataquitado]", dataQuitadoPago.ToString("dd") + " de " + dataQuitadoPago.ToString("MMMM") + " de " + dataQuitadoPago.Year.ToString());
            else
                corpo = corpo.Replace("[dataquitado]", dataQuitadoEmAberto.ToString("dd") + " de " + dataQuitadoEmAberto.ToString("MMMM") + " de " + dataQuitadoEmAberto.Year.ToString());

            #endregion corpo do e-mail

            //envia email
            MailMessage msg = new MailMessage(
                new MailAddress(ConfigurationManager.AppSettings["mailFrom"], ConfigurationManager.AppSettings["mailFromName"]),
                new MailAddress(txtEmailAtendimento.Text));
            msg.Subject = "Carta de permanência";
            msg.IsBodyHtml = true;

            msg.Body = corpo;

            try
            {
                SmtpClient client = new SmtpClient();
                client.Send(msg);
                msg.Dispose();
                client = null;
                base.Alerta(null, this, "_ok", "E-mail enviado com sucesso.");
            }
            catch { }
        }

        protected void cmdCartaDePermanenciaPrint_Click(Object sender, EventArgs e)
        {
            if (this.contratoId == null) { return; }
            ScriptManager.RegisterClientScriptBlock(this,
                    this.GetType(),
                    "_openCartaPerm",
                    String.Concat(" window.open(\"", "cartapermanencia.aspx?" + IDKey + "=" + Convert.ToString(this.contratoId), "\", \"janela\", \"toolbar=no,scrollbars=1,width=860,height=550\"); "),
                    true);
        }

        //-------//  TERMO ANUAL

        protected void cmdTermoAnual_Click(Object sender, EventArgs e)
        {
            if (this.contratoId == null) { return; }
            if (!checaRegraTermoAnual())
            {
                Alerta(null, this, "_errTermo", "Termo de Declaração de Quitação Anual de Débitos não pode ser gerado.");
                return;
            }
            if (String.IsNullOrEmpty(txtEmailAtendimento.Text.Trim()))
            {
                Alerta(null, this, "_errTermoMail", "Não há um e-mail informado.");
                return;
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            string nome = "", cpf = "";

            Contrato contrato = new Contrato(contratoId);
            contrato.Carregar();
            ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(contrato.ID, null);

            if (!string.IsNullOrEmpty(contrato.ResponsavelNome) && !string.IsNullOrEmpty(contrato.ResponsavelCPF))
            {
                //responsavel financeiro
                nome = contrato.ResponsavelNome;
                cpf = contrato.ResponsavelCPF;
            }
            else
            {
                //titular cadastrado
                nome = titular.BeneficiarioNome;
                cpf = titular.BeneficiarioCPF;
            }

            #region corpo do e-mail

            sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
            sb.Append("<head><title></title></head>");
            sb.Append("<body style=\"font-family:arial\"> ");
            sb.Append("<table width=\"99%\" align=\"center\">");
            sb.Append("<tr>");
            sb.Append("<td align=\"right\"><img src=\"http://linkecerebro.com.br/clientes/qualicorp/qualicorp.jpg\" width='180' /></td>");  //sb.Append("<td align=\"right\"><img src=\"https://sistemas.qualicorp.com.br/images/qualicorp.jpg\" width='180px' /></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<tr>");
            sb.Append("<td align=\"left\">Cliente Sr(a). "); sb.Append(nome); sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td align=\"left\">CPF: "); sb.Append(cpf); sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("<br /><br /><br /><br /><br /><br />");

            sb.Append("<table width=\"99%\" align=\"center\">");
            sb.Append("<tr><td align=\"left\">Ref.: DECLARACÃO DE QUITAÇÃO ANUAL DE DÉBITOS</td></tr>");
            sb.Append("</table>");
            sb.Append("<br /><br /><br /><br /><br /><br />");

            sb.Append("<table width=\"99%\" align=\"center\">");
            sb.Append("<tr><td align=\"left\">Em cumprimento à  Lei nº 12.007, de 29 de julho de 2009, declaramos que o cliente acima está quite com suas faturas vencidas no ano de 2013 e anos anteriores.</td></tr>");
            sb.Append("</table>");
            sb.Append("<br />");

            sb.Append("<table width=\"99%\" align=\"center\">");
            sb.Append("<tr><td align=\"left\">Atenciosamente,</td></tr>");
            sb.Append("</table>");
            sb.Append("<br />");

            sb.Append("<table width=\"99%\" align=\"center\">");
            sb.Append("<tr><td align=\"left\" colspan=\"2\">QUALICORP ADM. E SERV. LTDA</td></tr>");
            sb.Append("<tr><td align=\"left\">CNPJ 03.609.855/0001-02</td><td align=\"right\">www.qualicorp.com.br</td></tr>");
            sb.Append("</table>");
            sb.Append("<br />");

            sb.Append("<table width=\"99%\" align=\"center\">");
            sb.Append("<tr><td width=\"350\" align=\"left\">TODO O BRASIL (exceto RJ)</td><td align=\"left\">RIO DE JANEIRO</td></tr>");
            sb.Append("<tr><td align=\"left\"><b>0800-16-2000</b></td><td align=\"left\"><b>0800-778-4004</b></td></tr>");
            sb.Append("</table>");
            sb.Append("<br />");

            sb.Append("<br><br><font size='1' color='red'>Este é um e-mail automático. Por favor, não o responda.</font>");
            sb.Append("</body>");
            sb.Append("</html>");


            #endregion corpo do e-mail

            //envia email
            MailMessage msg = new MailMessage(
                new MailAddress(ConfigurationManager.AppSettings["mailFrom"], ConfigurationManager.AppSettings["mailFromName"]),
                new MailAddress(txtEmailAtendimento.Text));
            msg.Subject = "[Qualicorp]Termo de Declaração de Quitação";
            msg.IsBodyHtml = true;

            msg.Body = sb.ToString();

            try
            {
                SmtpClient client = new SmtpClient();
                client.Send(msg);
                msg.Dispose();
                client = null;
                base.Alerta(null, this, "_ok", "E-mail enviado com sucesso.");
            }
            catch { }
        }

        protected void cmdTermoAnualPrint_Click(Object sender, EventArgs e)
        {
            if (this.contratoId == null) { return; }
            if (!checaRegraTermoAnual())
            {
                Alerta(null, this, "_errTermo", "Termo de Declaração de Quitação Anual de Débitos não pode ser gerado.");
                return;
            }

            ScriptManager.RegisterClientScriptBlock(this,
                    this.GetType(),
                    "_openTermo",
                    String.Concat(" window.open(\"", "termoquitacao.aspx?" + IDKey + "=" + Convert.ToString(this.contratoId), "\", \"janela\", \"toolbar=yes,menubar=1,scrollbars=1,width=860,height=550\"); "),
                    true);
        }

        bool checaRegraTermoAnual()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            Contrato contrato = new Contrato(this.contratoId);
            contrato.Carregar();

            ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(this.contratoId, pm);
            if (titular.BeneficiarioNome.IndexOf('*') > -1) { pm.CloseSingleCommandInstance(); return false; }

            if (titular.BeneficiarioDataNascimento != DateTime.MinValue)
            {
                int idade = Beneficiario.CalculaIdade(titular.BeneficiarioDataNascimento);
                if (idade < 18 && string.IsNullOrEmpty(contrato.ResponsavelNome)) { pm.CloseSingleCommandInstance(); return false; }
            }

            string qry = "* from cobranca left JOIN cobranca_parcelamentoCobrancaOriginal ON cobranca_id = parccob_cobrancaId  LEFT JOIN cobranca_parcelamentoItem ON cobranca_id = parcitem_cobrancaId where year(cobranca_datavencimento)=2013 and (cobranca_cancelada is null or cobranca_cancelada = 0) and cobranca_propostaId=" + contrato.ID;

            qry += " order by cobranca_datavencimento";

            IList<Cobranca> cobrancas2013 = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);

            //ideia: trabalhar com colecoes de cobrancas: ok e naoOk

            //se nao tem cobrancas no periodo
            if (cobrancas2013 == null || cobrancas2013.Count == 0)
            {
                pm.CloseSingleCommandInstance(); pm.Dispose(); return false;
            }

            //checa inadimplencia em 2013
            foreach (Cobranca cob in cobrancas2013)
            {
                if (contrato.Cancelado || contrato.Inativo)
                {
                    if (cob.DataVencimento > contrato.DataCancelamento && cob.Tipo != (int)Cobranca.eTipo.Parcelamento)
                        continue;
                }

                if (!cob.Pago && cob.Tipo != (int)Cobranca.eTipo.Parcelamento && cob.HeaderParcID == null)
                {
                    //cobrancas nao parceladas
                    pm.CloseSingleCommandInstance(); pm.Dispose(); return false;
                }
                else if (!cob.Pago && cob.Tipo == (int)Cobranca.eTipo.Parcelamento)
                {
                    //checa se a negociacao 
                    pm.CloseSingleCommandInstance(); pm.Dispose(); return false;
                }
            }

            //valida quanto às baixas liminares e provisorias
            CobrancaBaixa baixa = null;
            IList<Cobranca> todas = Cobranca.CarregarTodas(contratoId, true, pm);
            foreach (Cobranca cob in todas)
            {
                baixa = CobrancaBaixa.CarregarUltima(cob.ID, pm);
                if (baixa == null) continue;

                if (baixa.BaixaProvisoria && baixa.Tipo == 0)
                { pm.CloseSingleCommandInstance(); pm.Dispose(); return false; }

                if ((Convert.ToString(baixa.MotivoID) == "12" || Convert.ToString(baixa.MotivoID) == "13") && baixa.Tipo == 0)
                { pm.CloseSingleCommandInstance(); pm.Dispose(); return false; }
            }

            pm.CloseSingleCommandInstance();
            pm.Dispose();

            return true;
        }

        //////////////////////////////////////////// 

        protected void gridCobranca_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                /// NEGOCIACAO ///////////////////////////////
                if (gridCobranca.DataKeys[e.Row.RowIndex][1] != null && Convert.ToString(gridCobranca.DataKeys[e.Row.RowIndex][1]).Trim() != "")
                {
                    e.Row.Cells[5].ForeColor = System.Drawing.Color.Green;
                    e.Row.Cells[5].Text = "negociada";
                    e.Row.Cells[6].Text = "";
                    e.Row.ToolTip = "parcela negociada";

                    e.Row.Cells[7].Enabled = false;
                    e.Row.Cells[7].Controls[0].Visible = false;
                    e.Row.Cells[9].Enabled = false;
                    e.Row.Cells[9].Controls[0].Visible = false;
                }

                if (gridCobranca.DataKeys[e.Row.RowIndex][2] != null && Convert.ToString(gridCobranca.DataKeys[e.Row.RowIndex][2]).Trim() != "")
                {
                    e.Row.ToolTip = "Negociação";
                    e.Row.Cells[5].Text += "*";

                    Cobranca cobranca = (Cobranca)e.Row.DataItem;

                    if (!String.IsNullOrEmpty(cobranca.ItemParcelamentoOBS))
                        e.Row.Cells[0].Text = cobranca.ItemParcelamentoOBS.Split(' ')[1];
                }
                /////////////////////////////////////////////////

                UIHelper.AuthWebCtrl(((TextBox)e.Row.Cells[3].Controls[1]), new String[] { Perfil.Atendimento_Liberacao_Vencimento, Perfil.ConsulPropBenefProdLiberBoletoIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.SupervidorIDKey });

                ///////////////////////////////////////////
                Panel panel = (Panel)e.Row.Cells[1].FindControl("pnlComposite");
                Literal lit = (Literal)panel.FindControl("litComposite");

                lit.Text = ((Cobranca)e.Row.DataItem).ComposicaoResumo;
                ///////////////////////////////////////////

                if (Server.HtmlDecode(e.Row.Cells[5].Text) == "Não")
                {
                    DateTime vencto = base.CStringToDateTime(((TextBox)e.Row.Cells[3].Controls[1]).Text);
                    if (vencto < DateTime.Now)
                    {
                        e.Row.Cells[5].ForeColor = System.Drawing.Color.Red;
                    }

                    if (!((TextBox)e.Row.Cells[3].Controls[1]).Enabled)
                    {
                        DateTime vigencia, vencimento, admissao = base.CStringToDateTime(txtAdmissao.Text);
                        Int32 diaDataSemJuros = 0, limiteAposVencto = 0; Object valorDataLimite;
                        CalendarioVencimento rcv = null;

                        if (!base.HaItemSelecionado(cboContrato))
                        {
                            CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(cboContrato.SelectedValue,
                               admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, out limiteAposVencto, null);
                        }

                        if (limiteAposVencto > 0)
                        {
                            DateTime venctoLimite = DateTime.Now.AddDays(limiteAposVencto);
                            venctoLimite = new DateTime(venctoLimite.Year, venctoLimite.Month, venctoLimite.Day, 23, 59, 59, 999);

                            if (venctoLimite < DateTime.Now)
                            {
                                ((ImageButton)e.Row.Cells[7].Controls[0]).Visible = false;
                                ((ImageButton)e.Row.Cells[9].Controls[0]).Visible = false;
                            }
                        }
                    }
                }
                else
                {
                    Decimal cobrado = 0, pago = 0;
                    pago = Convert.ToDecimal(Server.HtmlDecode(e.Row.Cells[2].Text.Replace("R$ ", "")));
                    cobrado = Convert.ToDecimal(Server.HtmlDecode(((LinkButton)e.Row.Cells[1].Controls[1]).Text.Replace("R$ ", "")));

                    if (pago >= cobrado)
                    {
                        e.Row.Cells[7].Controls[0].Visible = false;
                    }
                    else
                    {
                        ((LinkButton)e.Row.Cells[7].Controls[0]).Attributes.Add("onClick", "if(confirm('Esta ação criará uma cobrança complementar no valor de " + (cobrado - pago).ToString("C") + ".\\nConfirma a operação?')) { return true; } else { return false; }");
                    }
                }

                //((LinkButton)e.Row.Cells[6].Controls[0]).ToolTip = "enviar e-mail";
                ((ImageButton)e.Row.Cells[8].Controls[0]).ToolTip = "recalcular";
            }
        }

        protected void gridCobranca_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "recalcular")
            {
                Object id = gridCobranca.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Cobranca cobranca = new Cobranca(id);
                cobranca.Carregar();
                List<CobrancaComposite> composite = null;

                if (cobranca.Tipo == (int)Cobranca.eTipo.Normal)
                {
                    cobranca.Valor = Contrato.CalculaValorDaProposta2(this.contratoId, cobranca.DataVencimento, null, true, true, ref composite, false);
                    cobranca.Salvar();
                    cobranca = null;
                    this.CarregarCobrancas();
                }
            }
            if (e.CommandName == "email" || e.CommandName == "print")
            {
                #region - comentado 

                //List<CobrancaComposite> composite = null;

                //String naoReceber = "Não receber após o vencimento.";

                //Int32 indice = Convert.ToInt32(e.CommandArgument);
                //DateTime vencto = base.CStringToDateTime(((TextBox)gridCobranca.Rows[indice].Cells[3].Controls[1]).Text);
                //if (vencto == DateTime.MinValue)
                //{
                //    base.Alerta(null, this, "_errNCobVecto", "Data de vencimento inválida.");
                //    return;
                //}

                //vencto = new DateTime(vencto.Year, vencto.Month, vencto.Day, 23, 59, 59, 500);

                //String email = "";

                //ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(this.contratoId, null);
                //Beneficiario beneficiario = new Beneficiario(titular.BeneficiarioID);
                //beneficiario.Carregar();
                //if (!String.IsNullOrEmpty(beneficiario.Email) && txtEmailAtendimento.Text.Trim() == "") { txtEmailAtendimento.Text = beneficiario.Email; }

                //if (txtEmailAtendimento.Text.Trim() == "" && txtEmailAtendimentoCC.Text.Trim() == "" && e.CommandName == "email")
                //{
                //    base.Alerta(null, this, "_errNCobMail", "Nenhum endereço de e-mail informado.");
                //    return;
                //}

                //String nome = beneficiario.Nome;

                //if (beneficiario.Email != txtEmailAtendimento.Text && txtEmailAtendimento.Text.IndexOf("linkecerebro") == -1 && txtEmailAtendimento.Text.IndexOf("pspadrao") == -1 && txtEmailAtendimento.Text.IndexOf("padraoseguros") == -1 && e.CommandName == "email")
                //{
                //    beneficiario.Email = txtEmailAtendimento.Text;
                //    beneficiario.Salvar();
                //}

                //if (txtEmailAtendimento.Text.Trim() != "")
                //    email = txtEmailAtendimento.Text.Trim();

                //if (txtEmailAtendimentoCC.Text.Trim() != "")
                //{
                //    if (email.Length > 0) { email += ";"; }
                //    email += txtEmailAtendimentoCC.Text.Trim();
                //}

                //Object id = gridCobranca.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                //Cobranca cobranca = new Cobranca(id);
                //cobranca.Carregar();

                ////if (cobranca.DataVencimento < DateTime.Now)
                ////{
                ////    //está vencida
                ////    DateTime dataMaxima = DateTime.Now.AddDays(2);
                ////    dataMaxima = new DateTime(dataMaxima.Year, dataMaxima.Month, dataMaxima.Day, 23, 59, 59, 950);

                ////    if (dataMaxima.DayOfWeek == DayOfWeek.Saturday)
                ////        dataMaxima = dataMaxima.AddDays(2);
                ////    else if (dataMaxima.DayOfWeek == DayOfWeek.Sunday)
                ////        dataMaxima = dataMaxima.AddDays(1);

                ////    if (vencto > dataMaxima && Usuario.Autenticado.ID != "32481")
                ////    {
                ////        base.Alerta(null, this, "_err", "Data de vencimento não pode exceder " + dataMaxima.ToString("dd/MM/yyyy") + ".");
                ////        return;
                ////    }
                ////}

                //String nossoNumero = "";
                //Int32 dia = DateTime.Now.AddDays(1).Day;
                //Int32 mes = DateTime.Now.AddDays(1).Month;
                //Int32 ano = DateTime.Now.AddDays(1).Year;
                //Decimal Valor = 0;

                //Contrato contrato = new Contrato(this.contratoId);
                //contrato.Carregar();
                //cobranca.ContratoCodCobranca = contrato.CodCobranca.ToString();

                //if (!cobranca.Pago) // se NÃO está pago //////////////////////
                //{
                //    nossoNumero = cobranca.GeraNossoNumero();

                //    if (!Cobranca.NossoNumeroITAU)
                //        nossoNumero = nossoNumero.Substring(0, nossoNumero.Length - 1); //tira o DV

                //    if (cobranca.Tipo == (Int32)Cobranca.eTipo.Normal)
                //    {
                //        Valor = Contrato.CalculaValorDaProposta(contrato.ID, cobranca.DataVencimento, null, true, true, ref composite, true);
                //        if (Valor == 0)
                //        {
                //            base.Alerta(null, this, "_err", "Não foi possível localizar uma tabela de valor para a cobrança.");
                //            return;
                //        }
                //    }
                //    else
                //        Valor = cobranca.Valor;
                //    DateTime DataVenct = vencto;

                //    if (cobranca.DataVencimento < vencto) //loga alteraçao de vencto para isencao de juros
                //    {
                //        cobranca.DataVencimentoISENCAOJURO = vencto;
                //        Cobranca.LogaNovaDataDeVencimentoParaEmissao(
                //            cobranca.ID, cobranca.DataVencimento, vencto, Usuario.Autenticado.ID, null);
                //    }

                //    DateTime vigencia, vencimento;
                //    Int32 diaDataSemJuros = -1;
                //    Object valorDataLimite = null;
                //    CalendarioVencimento rcv = null;

                //    CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contrato.ContratoADMID,
                //        contrato.Admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv);

                //    DateTime dataSemJuros = DateTime.MinValue;

                //    try
                //    {
                //        if (diaDataSemJuros == -1 || diaDataSemJuros == 0) { diaDataSemJuros = cobranca.DataVencimento.Day; }
                //        dataSemJuros = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, diaDataSemJuros, 23, 59, 59);
                //    }
                //    catch { }

                //    if (DataVenct > DateTime.Now)
                //    {
                //        dia = DataVenct.Day;
                //        mes = DataVenct.Month;
                //        ano = DataVenct.Year;
                //    }

                //    Boolean calculaJuros = true;
                //    if (DataVenct <= new DateTime(2012, 7, 13, 23, 59, 59, 990) && DataVenct.Day == 10 && DataVenct.Month == 7 && DataVenct.Year == 2012)
                //    { calculaJuros = false; }

                //    if (DataVenct.Year == 2014 && DataVenct.Month == 3 &&
                //        (DataVenct.Day == 10 || DataVenct.Day == 25))
                //    {
                //        calculaJuros = false;
                //    }


                //    //todo: denis, medida temporaria. remover////////
                //    if (nome.IndexOf("(N*)") > -1) { calculaJuros = false; }
                //    /////////////////////////////////////////////////


                //    if (calculaJuros && dataSemJuros != DateTime.MinValue && dataSemJuros < DateTime.Now && cobranca.DataVencimentoISENCAOJURO < DateTime.Now)
                //    {
                //        DateTime database = new DateTime(ano, mes, dia, 23, 59, 59, 500);
                //        //CALCULA OS JUROS
                //        TimeSpan tempoAtraso = database.Subtract(dataSemJuros);

                //        Decimal atraso = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["jurosAtraso"]);
                //        Decimal atrasoDia = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["jurosDia"]);

                //        Valor += Valor * atraso;

                //        if (tempoAtraso.Days > 1)
                //        {
                //            Valor += Valor * (atrasoDia * (tempoAtraso.Days));
                //        }
                //    }
                //}
                //else
                //{
                //    #region cobrança COMPLEMENTAR ////////////////////////////////////

                //    Cobranca tempCompl = Cobranca.CarregarPor(cobranca.PropostaID, cobranca.Parcela, (int)Cobranca.eTipo.Complementar);
                //    if (tempCompl == null)
                //    {
                //        cobranca.Tipo = (int)Cobranca.eTipo.Complementar;
                //        cobranca.DataVencimento = cobranca.DataVencimento; //DateTime.Now.AddDays(7);
                //        cobranca.Pago = false;
                //        cobranca.CobrancaRefID = cobranca.ID;
                //        cobranca.ID = null;
                //        cobranca.Valor = cobranca.Valor - cobranca.ValorPgto;
                //        cobranca.ValorPgto = 0;
                //        cobranca.DataPgto = DateTime.MinValue;
                //        cobranca.DataCriacao = DateTime.Now;
                //        cobranca.ContratoCodCobranca = contrato.CodCobranca.ToString();

                //        if (cobranca.Valor > 0)
                //        {
                //            cobranca.Salvar();

                //            nossoNumero = cobranca.GeraNossoNumero();
                //            nossoNumero = nossoNumero.Substring(0, nossoNumero.Length - 1); //TIRA o DV
                //            Valor = cobranca.Valor;
                //            dia = cobranca.DataVencimento.Day;
                //            mes = cobranca.DataVencimento.Month;
                //            ano = cobranca.DataVencimento.Year;
                //            id = cobranca.ID;
                //        }
                //    }

                //    #endregion
                //}

                //if (e.CommandName == "print") { email = ""; }

                //String uri = "";

                //String instrucoes = String.Concat("<br>Este boleto é referente ao período de cobertura de ", cobranca.DataVencimento.Month, "/", cobranca.DataVencimento.Year, ".");

                //////PARCELAMENTO/////////////////////////////////////////////////////////
                //if (cobranca.Tipo == (int)Cobranca.eTipo.Parcelamento)
                //{
                //    ParcelamentoItem item = ParcelamentoItem.CarregarPorCobrancaId(cobranca.ID);
                //    instrucoes = "<br>" + item.Observacoes;
                //}
                ///////////////////////////////////////////////////////////////////////////

                //if ((contrato.ContratoADMID != null && Convert.ToInt32(contrato.ContratoADMID) >= Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"])) ||
                //    Convert.ToInt32(cboContrato.SelectedValue) >= Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]))
                //{
                //    uri = EntityBase.RetiraAcentos(String.Concat("?nossonum=", nossoNumero, "&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano, "&numdoc2=", contrato.Numero, "&nome=", nome, "&cod_cli=", id, "&mailto=", email, "&instr=", instrucoes, ".<br><br>" + naoReceber));
                //}
                //else
                //{
                //    uri = EntityBase.RetiraAcentos(String.Concat("?nossonum=", nossoNumero, "&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano, "&numdoc2=", contrato.Numero, "&nome=", nome, "&cod_cli=", id, "&mailto=", email, "&instr=", instrucoes, ".<br><br>" + naoReceber));
                //}

                //String finalUrl = string.Concat(
                //    "http://186.233.90.19/boleto_itau.php?param=", //"http://localhost/phpBoleto/boleto_itau.php?param=",
                //    EncryptBetweenPHP(uri));

                //ScriptManager.RegisterClientScriptBlock(this,
                //    this.GetType(),
                //    "_geraBoleto_" + id,
                //    String.Concat(" window.open(\"", finalUrl, "\", \"janela\", \"toolbar=no,scrollbars=1,width=860,height=420\"); "),
                //    true);

                //this.CarregarCobrancas();
                //CobrancaLog log = new CobrancaLog();
                //log.CobrancaEnviada(cobranca.ID, Usuario.Autenticado.ID, CobrancaLog.Fonte.Sistema);
                //log = null;

                #endregion

                #region

                List<CobrancaComposite> composite = null;

                String naoReceber = "Não receber após o vencimento.";

                Int32 indice = Convert.ToInt32(e.CommandArgument);
                DateTime vencto = base.CStringToDateTime(((TextBox)gridCobranca.Rows[indice].Cells[3].Controls[1]).Text);
                if (vencto == DateTime.MinValue)
                {
                    base.Alerta(null, this, "_errNCobVecto", "Data de vencimento inválida.");
                    return;
                }

                vencto = new DateTime(vencto.Year, vencto.Month, vencto.Day, 23, 59, 59, 500);

                String email = "";

                ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(this.contratoId, null);
                Beneficiario beneficiario = new Beneficiario(titular.BeneficiarioID);
                beneficiario.Carregar();
                if (!String.IsNullOrEmpty(beneficiario.Email) && txtEmailAtendimento.Text.Trim() == "") { txtEmailAtendimento.Text = beneficiario.Email; }

                if (txtEmailAtendimento.Text.Trim() == "" && txtEmailAtendimentoCC.Text.Trim() == "" && e.CommandName == "email")
                {
                    base.Alerta(null, this, "_errNCobMail", "Nenhum endereço de e-mail informado.");
                    return;
                }

                String nome = beneficiario.Nome;

                if (beneficiario.Email != txtEmailAtendimento.Text && txtEmailAtendimento.Text.IndexOf("ubrasp") == -1 && txtEmailAtendimento.Text.IndexOf("wedigi") == -1 && txtEmailAtendimento.Text.IndexOf("pspadrao") == -1 && txtEmailAtendimento.Text.IndexOf("padraoseguros") == -1 && e.CommandName == "email")
                {
                    beneficiario.Email = txtEmailAtendimento.Text;
                    beneficiario.Salvar();
                }

                if (txtEmailAtendimento.Text.Trim() != "")
                    email = txtEmailAtendimento.Text.Trim();

                if (txtEmailAtendimentoCC.Text.Trim() != "")
                {
                    if (email.Length > 0) { email += ";"; }
                    email += txtEmailAtendimentoCC.Text.Trim();
                }

                Object id = gridCobranca.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Cobranca cobranca = new Cobranca(id);
                cobranca.Carregar();

                //if (cobranca.DataVencimento < DateTime.Now)
                //{
                //    //está vencida
                //    DateTime dataMaxima = DateTime.Now.AddDays(2);
                //    dataMaxima = new DateTime(dataMaxima.Year, dataMaxima.Month, dataMaxima.Day, 23, 59, 59, 950);

                //    if (dataMaxima.DayOfWeek == DayOfWeek.Saturday)
                //        dataMaxima = dataMaxima.AddDays(2);
                //    else if (dataMaxima.DayOfWeek == DayOfWeek.Sunday)
                //        dataMaxima = dataMaxima.AddDays(1);

                //    if (vencto > dataMaxima && Usuario.Autenticado.ID != "32481")
                //    {
                //        base.Alerta(null, this, "_err", "Data de vencimento não pode exceder " + dataMaxima.ToString("dd/MM/yyyy") + ".");
                //        return;
                //    }
                //}

                String nossoNumero = "";
                Int32 dia = DateTime.Now.AddDays(1).Day;
                Int32 mes = DateTime.Now.AddDays(1).Month;
                Int32 ano = DateTime.Now.AddDays(1).Year;
                Decimal Valor = 0;

                Contrato contrato = new Contrato(this.contratoId);
                contrato.Carregar();
                cobranca.ContratoCodCobranca = contrato.CodCobranca.ToString();

                if (!cobranca.Pago) // se NÃO está pago //////////////////////
                {
                    nossoNumero = cobranca.GeraNossoNumero();

                    if (!Cobranca.NossoNumeroITAU)
                        nossoNumero = nossoNumero.Substring(0, nossoNumero.Length - 1); //tira o DV

                    if (cobranca.Tipo == (Int32)Cobranca.eTipo.Normal && !contrato.Legado)
                    {
                        Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, null, true, true, ref composite, true);
                        if (Valor == 0)
                        {
                            base.Alerta(null, this, "_err", "Não foi possível localizar uma tabela de valor para a cobrança.");
                            return;
                        }
                    }
                    else
                        Valor = cobranca.Valor;
                    DateTime DataVenct = vencto;

                    if (cobranca.DataVencimento < vencto) //loga alteraçao de vencto para isencao de juros
                    {
                        cobranca.DataVencimentoISENCAOJURO = vencto;
                        Cobranca.LogaNovaDataDeVencimentoParaEmissao(
                            cobranca.ID, cobranca.DataVencimento, vencto, Usuario.Autenticado.ID, null);
                    }

                    DateTime vigencia, vencimento;
                    Int32 diaDataSemJuros = -1;
                    Object valorDataLimite = null;
                    CalendarioVencimento rcv = null;

                    CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contrato.ContratoADMID,
                        contrato.Admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv);

                    DateTime dataSemJuros = DateTime.MinValue;

                    try
                    {
                        if (diaDataSemJuros == -1 || diaDataSemJuros == 0) { diaDataSemJuros = cobranca.DataVencimento.Day; }
                        dataSemJuros = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, diaDataSemJuros, 23, 59, 59);
                    }
                    catch { }

                    if (DataVenct > DateTime.Now)
                    {
                        dia = DataVenct.Day;
                        mes = DataVenct.Month;
                        ano = DataVenct.Year;
                    }

                    Boolean calculaJuros = true;
                    if (DataVenct <= new DateTime(2012, 7, 13, 23, 59, 59, 990) && DataVenct.Day == 10 && DataVenct.Month == 7 && DataVenct.Year == 2012)
                    { calculaJuros = false; }

                    if (DataVenct.Year == 2014 && DataVenct.Month == 3 &&
                        (DataVenct.Day == 10 || DataVenct.Day == 25))
                    {
                        calculaJuros = false;
                    }


                    //todo: denis, medida temporaria. remover////////
                    if (nome.IndexOf("(N*)") > -1) { calculaJuros = false; }
                    /////////////////////////////////////////////////


                    if (calculaJuros && dataSemJuros != DateTime.MinValue && dataSemJuros < DateTime.Now && cobranca.DataVencimentoISENCAOJURO < DateTime.Now)
                    {
                        DateTime database = new DateTime(ano, mes, dia, 23, 59, 59, 500);
                        //CALCULA OS JUROS
                        TimeSpan tempoAtraso = database.Subtract(dataSemJuros);

                        Decimal atraso = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["jurosAtraso"]);
                        Decimal atrasoDia = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["jurosDia"]);

                        Valor += Valor * atraso;

                        if (tempoAtraso.Days > 1)
                        {
                            Valor += Valor * (atrasoDia * (tempoAtraso.Days));
                        }
                    }
                }
                else
                {
                    #region cobrança COMPLEMENTAR ////////////////////////////////////

                    Cobranca tempCompl = Cobranca.CarregarPor(cobranca.PropostaID, cobranca.Parcela, (int)Cobranca.eTipo.Complementar);
                    if (tempCompl == null)
                    {
                        cobranca.Tipo = (int)Cobranca.eTipo.Complementar;
                        cobranca.DataVencimento = cobranca.DataVencimento; //DateTime.Now.AddDays(7);
                        cobranca.Pago = false;
                        cobranca.CobrancaRefID = cobranca.ID;
                        cobranca.ID = null;
                        cobranca.Valor = cobranca.Valor - cobranca.ValorPgto;
                        cobranca.ValorPgto = 0;
                        cobranca.DataPgto = DateTime.MinValue;
                        cobranca.DataCriacao = DateTime.Now;
                        cobranca.ContratoCodCobranca = contrato.CodCobranca.ToString();

                        if (cobranca.Valor > 0)
                        {
                            cobranca.Salvar();

                            nossoNumero = cobranca.GeraNossoNumero();
                            nossoNumero = nossoNumero.Substring(0, nossoNumero.Length - 1); //TIRA o DV
                            Valor = cobranca.Valor;
                            dia = cobranca.DataVencimento.Day;
                            mes = cobranca.DataVencimento.Month;
                            ano = cobranca.DataVencimento.Year;
                            id = cobranca.ID;
                        }
                    }

                    #endregion
                }

                if (e.CommandName == "print") { email = ""; }

                String uri = "";

                String instrucoes = String.Concat("<br>Este boleto é referente ao período de cobertura de ", cobranca.DataVencimento.Month, "/", cobranca.DataVencimento.Year, ".");

                ////PARCELAMENTO/////////////////////////////////////////////////////////
                if (cobranca.Tipo == (int)Cobranca.eTipo.Parcelamento)
                {
                    ParcelamentoItem item = ParcelamentoItem.CarregarPorCobrancaId(cobranca.ID);
                    instrucoes = "<br>" + item.Observacoes;
                }
                /////////////////////////////////////////////////////////////////////////

                if ((contrato.ContratoADMID != null && Convert.ToInt32(contrato.ContratoADMID) >= Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"])) ||
                    Convert.ToInt32(cboContrato.SelectedValue) >= Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]))
                {
                    uri = EntityBase.RetiraAcentos(String.Concat("?nossonum=", nossoNumero, "&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano, "&numdoc2=", contrato.Numero, "&nome=", nome, "&cod_cli=", id, "&mailto=", email, "&instr=", instrucoes, ".<br><br>" + naoReceber));
                }
                else
                {
                    uri = EntityBase.RetiraAcentos(String.Concat("?nossonum=", nossoNumero, "&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano, "&numdoc2=", contrato.Numero, "&nome=", nome, "&cod_cli=", id, "&mailto=", email, "&instr=", instrucoes, ".<br><br>" + naoReceber));
                }

                String finalUrl = "";
                //string.Concat(
                //"http://186.233.90.19/boleto_itau.php?param=", //"http://localhost/phpBoleto/boleto_itau.php?param=",
                //EncryptBetweenPHP(uri));

                #region monta boleto bb

                uri = EntityBase.RetiraAcentos(
                    String.Concat(
                    "?nossonum=", nossoNumero,
                    "&contid=", contrato.ID,
                    "&valor=", Valor.ToString("N2"),
                    "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano,
                    "&bid=", beneficiario.ID,
                    "&cobid=", id,
                    "&mailto=", email));

                //finalUrl = "/boleto/bb.aspx" + uri;
                finalUrl = "/boleto/santander2.aspx" + uri;

                string host = "";

                if (HttpContext.Current.Request.Url.Host.IndexOf("localhost") > -1)
                    host = string.Concat("http://", HttpContext.Current.Request.Url.Host, ":", HttpContext.Current.Request.Url.Port);
                else
                    host = string.Concat("http://", HttpContext.Current.Request.Url.Host);


                System.Net.WebRequest request = System.Net.WebRequest.Create(string.Concat(host, finalUrl));
                System.Net.WebResponse response = request.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("iso-8859-1"));
                String html = sr.ReadToEnd().Replace(@"D:\http\ubrasp\web\var\boleto_file\", @"http://ubrasp.iphotel.info/images/boleto/"); //BoletoNetBarra.gif
                //String html = sr.ReadToEnd().Replace(@"C:\inetpub\wwwroot\Ubrasp\www\var\boleto_file\", @"http://ubrasp.iphotel.info/images/boleto/");//BoletoNetBarra.gif

                sr.Close();
                response.Close();

                File.WriteAllText(base.ArqBoletoPath + Session.SessionID + ".html", html, System.Text.Encoding.GetEncoding("iso-8859-1"));

                #endregion

                //finalUrl = string.Concat(host, finalUrl);
                finalUrl = string.Concat(host, "/", ConfigurationManager.AppSettings["boletoFilePath"], Session.SessionID, ".html");

                ScriptManager.RegisterClientScriptBlock(this,
                    this.GetType(),
                    "_geraBoleto_" + id,
                    String.Concat(" window.open(\"", finalUrl, "\", \"janela\", \"toolbar=no,scrollbars=1,width=860,height=420\"); "),
                    true);

                if (e.CommandName == "email" && !string.IsNullOrEmpty(email))
                {
                    //email = "denis.goncalves@wedigi.com.br";

                    try
                    {
                        MailServASMX.mail proxy = new MailServASMX.mail();
                        string ret = proxy.EnviaEmail(
                            ConfigurationManager.AppSettings["mailFrom"],
                            ConfigurationManager.AppSettings["mailFromName"],
                            "atendimentorj@ubrasp.com.br",
                            email,
                            "[UBRASP] Atendimento - Boleto",
                            html, //.Replace("src='/images/boleto/", "src='http://ubrasp.iphotel.info/images/boleto/"),
                            true,
                            "",
                            base._mailToken);
                        proxy.Dispose();
                    }
                    catch { }
                }

                ////ScriptManager.RegisterClientScriptBlock(this,
                ////    this.GetType(),
                ////    "_geraBoleto_" + id,
                ////    String.Concat(" window.open(\"", finalUrl, "\", \"janela\", \"toolbar=no,scrollbars=1,width=860,height=420\"); "),
                ////    true);

                //this.CarregarCobrancas();
                //CobrancaLog log = new CobrancaLog();
                //log.CobrancaEnviada(cobranca.ID, Usuario.Autenticado.ID, CobrancaLog.Fonte.Sistema);
                //log = null;

                #endregion
            }
            else if (e.CommandName.Equals("detalhe"))
            {
                Cobranca cobranca = new Cobranca(gridCobranca.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                cobranca.Carregar();

                txtIdCobrancaEmDetalhe.Text = Convert.ToString(cobranca.ID);
                this.exibeDetalheCobranca(cobranca);
            }
        }

        string EncryptBetweenPHP(string param)
        {
            byte[] key = Encoding.UTF8.GetBytes("passwordDR0wSS@P6660juht");
            byte[] iv = Encoding.UTF8.GetBytes("password");
            byte[] data = Encoding.UTF8.GetBytes(param);
            byte[] enc = new byte[0];
            TripleDES tdes = TripleDES.Create();
            tdes.IV = iv;
            tdes.Key = key;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.Zeros;
            ICryptoTransform ict = tdes.CreateEncryptor();
            enc = ict.TransformFinalBlock(data, 0, data.Length);
            return Bin2Hex(enc);
        }

        string Bin2Hex(byte[] bin)
        {
            StringBuilder sb = new StringBuilder(bin.Length * 2);
            foreach (byte b in bin)
            {
                sb.Append(b.ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        protected void gridCobranca_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            gridCobranca.PageIndex = e.NewPageIndex;
            this.CarregarCobrancas();
        }

        void exibeDetalheCobranca(Cobranca cobranca)
        {
            pnlCobrancaDetalhe.Visible = true;
            litTitulo.Text = String.Concat("<b>Parcela ", cobranca.Parcela,
                " - Vencto.: ", cobranca.DataVencimento.ToString("dd/MM/yyyy"), " - Valor: ", cobranca.Valor.ToString("C"), "</b>");

            IList<CobrancaComposite> composite = CobrancaComposite.Carregar(cobranca.ID);
            gridComposicao.DataSource = composite;
            gridComposicao.DataBind();

            if (cobranca.Pago)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                CobrancaBaixa baixa = CobrancaBaixa.CarregarUltima(cobranca.ID);
                sb.Append("<tr height='4'><td height='4'></td></tr><tr><td height='20' align='center' style='background-color:whitesmoke;'><b>Informações da Baixa</b></td></tr>");
                if (baixa != null)
                {
                    CobrancaMotivoBaixa motivo = new CobrancaMotivoBaixa(baixa.MotivoID);
                    motivo.Carregar();

                    sb.Append("<tr><td>Data: "); sb.Append(baixa.Data.ToString("dd/MM/yyyy HH:mm")); sb.Append("</td></tr>");
                    sb.Append("<tr><td>Valor Pago: "); sb.Append(cobranca.ValorPgto.ToString("C")); sb.Append("</td></tr>");
                    sb.Append("<tr><td>Motivo: "); sb.Append(motivo.Descricao); sb.Append("</td></tr>");
                    sb.Append("<tr><td>Tipo: "); sb.Append(baixa.strTipo); sb.Append("</td></tr>");
                    sb.Append("<tr><td>Baixa financeira: "); sb.Append(baixa.BaixaFinanceira ? "Sim" : "Não"); sb.Append("</td></tr>");
                    sb.Append("<tr><td>Baixa provisória: "); sb.Append(baixa.BaixaProvisoria ? "Sim" : "Não"); sb.Append("</td></tr>");

                    if (baixa.UsuarioID != null)
                    {
                        Usuario u = new Usuario(baixa.UsuarioID);
                        u.Carregar();
                        sb.Append("<tr><td>Usuário: "); sb.Append(u.Nome); sb.Append("</td></tr>");
                    }

                    if (!String.IsNullOrEmpty(baixa.Obs))
                    {
                        sb.Append("<tr height='5'><td height='5'></td></tr>");
                        sb.Append("<tr><td>"); sb.Append(baixa.Obs); sb.Append("</td></tr>");
                    }
                }
                else
                {
                    sb.Append("<tr><td>Data: ");
                    if (cobranca.Parcela == 1)
                        sb.Append(cobranca.DataCriacao.ToString("dd/MM/yyyy HH:mm"));
                    else if (cobranca.DataBaixaAutomatica != DateTime.MinValue)
                        sb.Append(cobranca.DataBaixaAutomatica.ToString("dd/MM/yyyy HH:mm"));
                    else
                        sb.Append(cobranca.DataPgto.ToString("dd/MM/yyyy HH:mm"));
                    sb.Append("</td></tr>");
                    sb.Append("<tr><td>Valor Pago: "); sb.Append(cobranca.ValorPgto.ToString("C")); sb.Append("</td></tr>");
                    sb.Append("<tr><td>Tipo: AUTOMÁTICA</td></tr>");
                }

                litBaixa.Text = sb.ToString();
                sb.Remove(0, sb.Length);
            }
            else
            {
                litBaixa.Text = "";
            }


            /////// NEGOCIACAO /////////////////////////////////////////////
            litNegociacao.Text = "";
            if (cobranca.Tipo == (int)Cobranca.eTipo.Parcelamento)
            {
                ParcelamentoItem item = ParcelamentoItem.CarregarPorCobrancaId(cobranca.ID);
                if (item != null)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<tr height='4'><td height='4'></td></tr><tr><td height='20' align='center' style='background-color:whitesmoke;'><b>Informações de Negociação</b></td></tr>");
                    sb.Append("<tr><td>");
                    sb.Append(item.Observacoes);

                    ParcelamentoHeader header = new ParcelamentoHeader();
                    header.ID = item.HeaderID;
                    header.Carregar();

                    sb.Append("<br><br>Data negociação: "); sb.Append(header.Data.ToString("dd/MM/yyyy"));
                    sb.Append("<br>Empresa: "); sb.Append(ParcelamentoHeader.GetEmpresaNome(header.EmpresaID));
                    sb.Append("<br>Parcelas negociadas: ");

                    IList<Cobranca> negociadas = ParcelamentoCobrancaOriginal.CarregarParcelasNegociadas(header.ID);
                    if (negociadas != null)
                    {
                        for (int i = 0; i < negociadas.Count; i++)
                        {
                            if (i > 0 && i < (negociadas.Count - 1)) sb.Append(", ");
                            else if (i > 0) sb.Append(" e ");

                            sb.Append(negociadas[i].Parcela);
                        }
                    }

                    if (!String.IsNullOrEmpty(header.OBS))
                    {
                        sb.Append("<br>Obs.: "); sb.Append(header.OBS.Replace(Environment.NewLine, "<br>"));
                    }
                    sb.Append("</td></tr>");
                    litNegociacao.Text = sb.ToString();
                    sb.Remove(0, sb.Length);
                }
            }
            else
            {
                ParcelamentoCobrancaOriginal pco = ParcelamentoCobrancaOriginal.CarregarPorCobrancaId(cobranca.ID);
                if (pco != null)
                {
                    ParcelamentoHeader header = new ParcelamentoHeader();
                    header.ID = pco.HeaderID;
                    header.Carregar();

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<tr height='4'><td height='4'></td></tr><tr><td height='20' align='center' style='background-color:whitesmoke;'><b>Informações de Negociação</b></td></tr>");
                    sb.Append("<tr><td>");
                    sb.Append("<font color='red'>ESTA COBRANÇA FAZ PARTE DE UMA NEGOCIAÇÃO ATIVA.</font>");

                    sb.Append("<br><br>Data negociação: "); sb.Append(header.Data.ToString("dd/MM/yyyy"));
                    sb.Append("<br>Empresa: "); sb.Append(ParcelamentoHeader.GetEmpresaNome(header.EmpresaID));
                    sb.Append("<br>Parcelas negociadas: ");

                    IList<Cobranca> negociadas = ParcelamentoCobrancaOriginal.CarregarParcelasNegociadas(header.ID);
                    if (negociadas != null)
                    {
                        for (int i = 0; i < negociadas.Count; i++)
                        {
                            if (i > 0 && i < (negociadas.Count - 1)) sb.Append(", ");
                            else if (i > 0) sb.Append(" e ");

                            sb.Append(negociadas[i].Parcela);
                        }
                    }

                    if (!String.IsNullOrEmpty(header.OBS))
                    {
                        sb.Append("<br>Obs.: "); sb.Append(header.OBS.Replace(Environment.NewLine, "<br>"));
                    }

                    sb.Append("</td></tr>");
                    litNegociacao.Text = sb.ToString();
                    sb.Remove(0, sb.Length);
                }
            }
            /////////////////////////////////////////////

            mpeCobrancaDetalhe.Show();
            upCobrancaDetalhe.Update();
        }

        protected void btnCalcularValorCob_Click(Object sender, EventArgs e)
        {
            txtValorCob.Text = "";
            DateTime vencto = base.CStringToDateTime(txtVencimentoCob.Text);
            if (vencto == DateTime.MinValue) { return; }
            List<CobrancaComposite> composite = null;

            txtValorCob.Text = Contrato.CalculaValorDaProposta2(this.contratoId, vencto, null, false, true, ref composite, false).ToString("N2");
        }

        protected void cmdGerarCobranca_Click(Object sender, EventArgs e)
        {
            #region validacoes

            if (txtParcelaCob.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err1", "Informe a parcela da cobrança.");
                txtParcelaCob.Focus();
                return;
            }

            DateTime dataVencimento = DateTime.MinValue;
            if (!UIHelper.TyParseToDateTime(txtVencimentoCob.Text, out dataVencimento))
            {
                base.Alerta(null, this, "__err2", "Data de vencimento inválida.");
                txtVencimentoCob.Focus();
                return;
            }
            dataVencimento = new DateTime(dataVencimento.Year, dataVencimento.Month, dataVencimento.Day, 23, 59, 59, 0);

            if (txtValorCob.Text.Trim() == "" || base.CToDecimal(txtValorCob.Text) == Decimal.Zero)
            {
                base.Alerta(null, this, "__err3", "Informe o valor da cobrança.");
                txtValorCob.Focus();
                return;
            }

            ////Regra para não gerar muitos meses a frente
            //DateTime dataRef = DateTime.MinValue;
            //if (dataVencimento > DateTime.Now)
            //{
            //    dataRef = new DateTime(
            //        DateTime.Now.AddMonths(2).Year,
            //        DateTime.Now.AddMonths(2).Month, 1);
            //    dataRef = dataRef.AddDays(-1);
            //}


            //if (dataRef != DateTime.MinValue && dataVencimento > dataRef)
            //{
            //    base.Alerta(null, this, "__err", "Não é possível gerar cobranças com esse vencimento.");
            //    return;
            //}

            #endregion

            Cobranca cobranca = new Cobranca();

            cobranca.Parcela = Convert.ToInt32(txtParcelaCob.Text);
            cobranca.DataVencimento = dataVencimento;
            cobranca.Valor = base.CToDecimal(txtValorCob.Text);
            cobranca.Tipo = Convert.ToInt32(Cobranca.eTipo.Normal);
            cobranca.CobrancaRefID = null;
            cobranca.DataPgto = DateTime.MinValue;
            cobranca.ValorPgto = Decimal.Zero;
            cobranca.Pago = false;
            cobranca.PropostaID = this.contratoId;
            cobranca.Cancelada = false;
            cobranca.Salvar();

            List<CobrancaComposite> composite = new List<CobrancaComposite>();

            Contrato.CalculaValorDaProposta2(this.contratoId, cobranca.DataVencimento, null, false, true, ref composite, false);
            CobrancaComposite.Salvar(cobranca.ID, composite, null);
            cobranca = null;

            this.ConfiguraAtendimento();
            base.Alerta(null, this, "_okNCob", "Cobrança salva com sucesso.");
        }

        protected void gridEnriquecimento_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja confirmar os dados?");
            }
        }

        protected void gridEnriquecimento_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("ok"))
            {
                Object id = gridEnriquecimento.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Object bid = gridEnriquecimento.DataKeys[Convert.ToInt32(e.CommandArgument)][1];

                LC.Framework.Phantom.NonQueryHelper.Instance.ExecuteNonQuery("update mailing set concluido=1,dataConclusao=getdate(),usuarioId=" + Usuario.Autenticado.ID + " where id_telMail=" + id + " and id_beneficiario=" + bid, null);
                this.checaEnriquecimento();

                base.Alerta(null, this, "ok", "Dados confirmados com sucesso.");
            }
        }
    }
}
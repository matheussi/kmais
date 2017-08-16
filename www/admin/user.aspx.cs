namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class user : PageBase
    {
        List<ExcecaoItem> Itens
        {
            get
            {
                String key = "__itens" + Session.SessionID;
                if (Cache[key] != null)
                {
                    List<ExcecaoItem> lista = Cache[key] as List<ExcecaoItem>;

                    for (int i = 0; i < gridItens.Rows.Count; i++)
                    {
                        lista[i].Parcela = base.CToInt(((TextBox)gridItens.Rows[i].Cells[0].Controls[1]).Text);
                        lista[i].Percentual = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[1].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualADM = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[4].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualCompraCarencia = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[2].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualEspecial = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[5].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualMigracao = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[3].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].Idade = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[6].Controls[1]).Text);
                        lista[i].ID = gridItens.DataKeys[i].Value;
                        //lista[i].TabelaExcecaoID = ViewState[IDKey];
                    }

                    return lista;
                }
                else
                    return null;
            }
            set { String key = "__itens" + Session.SessionID; Cache.Remove(key); if (value != null) { Cache.Insert(key, value, null, DateTime.Now.AddHours(1), TimeSpan.Zero); } }
        }

        Boolean alteraProdutor
        {
            get { if (ViewState["_altera"] == null) { return false; } else { return Convert.ToBoolean(ViewState["_altera"]); } }
            set { ViewState["_altera"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtComissionamentoVitalicioPercentual.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentual.ClientID + "')");
            txtComissionamentoVitalicioPercentualADM.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualADM.ClientID + "')");
            txtComissionamentoVitalicioPercentualCarencia.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualCarencia.ClientID + "')");
            txtComissionamentoVitalicioPercentualEspecial.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualEspecial.ClientID + "')");
            txtComissionamentoVitalicioPercentualIdade.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualIdade.ClientID + "')");
            txtComissionamentoVitalicioPercentualMigracao.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualMigracao.ClientID + "')");

            txtNumero.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");

            if (!IsPostBack)
            {
                this.ChecaPermissaoEscrita();
                base.ExibirOperadoras(cboTabelaExcecaoOperadora, false);
                this.CarregaContratosDaOperadora();
                base.ExibirTiposDeConta(cboTipoContaBancaria);
                //acc.SelectedIndex = -1;
                base.ExibirOpcoesDeSexo(cboSexo, false);
                base.ExibirEstadosCivisDeUsuario(cboEstadoCivil, false);
                this.CarregaPerfis();
                this.CarregaCategorias();
                base.ExibirFiliais(cboFilial, false);
                this.CarregaTabelasDeComissionamento();
                this.CarregaGruposDeVenda();
                cmdBuscaEndereco.Visible = base.useExternalCEPEngine();

                if (base.IDKeyParameterInProcess(ViewState, "_user"))
                {
                    this.CarregaUsuario();
                    this.CarregaTabelasDeExcecao();
                }
                else
                {
                    InabilitaComboSuperiores();
                    tab.Tabs[1].Visible = false;
                    tab.Tabs[2].Visible = false;
                    tab.Tabs[3].Visible = false;
                    p4_GrupoVenda.Visible = false;
                    p5_Contatos.Visible = false;
                }

                cboPerfil.Focus();
            }
        }

        /// <summary>
        /// Checa se o usuário tem permissão para salvar informações.
        /// </summary>
        void ChecaPermissaoEscrita()
        {
            if (Usuario.Autenticado.PerfilID == Perfil.AdministradorIDKey) { return; }

            Usuario user = new Usuario(Usuario.Autenticado.ID);
            user.Carregar();

            cmdSalvar.Visible = user.AlteraProdutor;
            Button1.Visible = user.AlteraProdutor;
            Button2.Visible = user.AlteraProdutor;
            Button3.Visible = user.AlteraProdutor;
            Button4.Visible = user.AlteraProdutor;
            Button5.Visible = user.AlteraProdutor;
            cmdAdicionarFilial.Visible = user.AlteraProdutor;
            cmdAdicionar.Visible = user.AlteraProdutor;
            cmdAdicionarEquipe.Visible = user.AlteraProdutor;
            cmdAdicionarGrupo.Visible = user.AlteraProdutor;
            lnkAdicionarContato.Visible = user.AlteraProdutor;
            this.alteraProdutor = user.AlteraProdutor;
        }

        void CarregaUsuario()
        {
            Usuario usuario = new Usuario();
            usuario.ID = ViewState[IDKey];
            usuario.Carregar();

            if(usuario.SuperiorID == null)
                this.CarregaPerfilSuperior(usuario.PerfilID, null);
            else
                this.CarregaPerfilSuperior(null, usuario.SuperiorID);

            txtNome.Text = usuario.Nome;
            txtApelido.Text = usuario.Apelido;
            txtEmail.Text = usuario.Email;
            //txtCodigo.Text = usuario.Codigo;
            //chkAtivo.Checked = usuario.Ativo;
            //chkLibera.Checked = usuario.LiberaContratos;
            //chkAlteraValor.Checked = usuario.AlteraValorContratos;

            if(usuario.DataNascimento != DateTime.MinValue)
                txtDataNascimento.Text = usuario.DataNascimento.ToString("dd/MM/yyyy");

            if (!String.IsNullOrEmpty(usuario.MarcaOtica))
            {
                txtMarcaOtica.Text = usuario.MarcaOtica;
            }

            cboSexo.SelectedValue = Convert.ToString(usuario.Sexo);
            cboEstadoCivil.SelectedValue = Convert.ToString(usuario.EstadoCivil);
            optFisica.Checked = usuario.TipoPessoa == 1;
            optJuridica.Checked = usuario.TipoPessoa == 2;
            optChanged(null, null);

            txtCPFouCNPJ.Text = usuario.Documento1;
            txtRGouIE.Text = usuario.Documento2;

            IList<Endereco> endereco = Endereco.CarregarPorDono(usuario.ID, Endereco.TipoDono.Produtor);
            if (endereco != null && endereco.Count > 0)
            {
                ViewState[IDKey2] = endereco[0].ID;
                txtCEP.Text = endereco[0].CEP;
                txtLogradouro.Text = endereco[0].Logradouro;
                txtNumero.Text = endereco[0].Numero;
                txtComplemento.Text = endereco[0].Complemento;
                txtBairro.Text = endereco[0].Bairro;
                txtCidade.Text = endereco[0].Cidade;
                txtBairro.Text = endereco[0].Bairro;
                txtUF.Text = endereco[0].UF.ToUpper();
            }

            txtDDD1.Text = usuario.DDD1;
            txtFone1.Text = usuario.Fone1;
            txtRamal1.Text = usuario.Ramal1;
            txtDDD2.Text = usuario.DDD2;
            txtFone2.Text = usuario.Fone2;
            txtRamal2.Text = usuario.Ramal2;
            txtDDDCelular.Text = usuario.DDD3;
            txtCelular.Text = usuario.Celular;
            txtCelularOperadora.Text = usuario.CelularOperadora;

            txtEntrevistadoPor.Text = usuario.EntrevistadoPor;
            if (usuario.EntrevistadoEm != DateTime.MinValue)
                txtEntrevistadoEm.Text = usuario.EntrevistadoEm.ToString("dd/MM/yyyy");

            txtBanco.Text = usuario.Banco;
            txtAgencia.Text = usuario.Agencia;
            cboTipoContaBancaria.SelectedValue = usuario.ContaTipo;
            txtContaCorrente.Text = usuario.Conta;
            txtFavorecido.Text = usuario.Favorecido;

            txtObs.Text = usuario.Obs;

            cboPerfil.SelectedValue = Convert.ToString(usuario.PerfilID);
            if (usuario.SuperiorID != null)
            {
                if (cboSuperior.Items.FindByValue(Convert.ToString(usuario.SuperiorID)) != null)
                    cboSuperior.SelectedValue = Convert.ToString(usuario.SuperiorID);
            }

            if (trCategoriaComissionamento.Visible)
            {
                if (usuario.CategoriaID != null && cboCategoriaComissionamento.Items.FindByValue(Convert.ToString(usuario.CategoriaID)) != null)
                    cboCategoriaComissionamento.SelectedValue = Convert.ToString(usuario.CategoriaID);
                else
                    cboCategoriaComissionamento.SelectedIndex = 0;
            }

            //if (trFilial.Visible)
            //{
            //    if (usuario.FilialID != null && cboFilial.Items.FindByValue(Convert.ToString(usuario.FilialID)) != null)
            //        cboFilial.SelectedValue = Convert.ToString(usuario.FilialID);
            //    else
            //        cboFilial.SelectedIndex = 0;
            //}

            //lnkUser.Checked = usuario.SystemUser;
            //pnlUser.Visible = usuario.SystemUser;
            //if (usuario.SystemUser)
            //{
            //    pnlUser.Visible = true;
            //}
            //else
            //{
            //    pnlUser.Visible = false;
            //}

            this.CarregaEquipes();
            this.CarregaFiliaisDoProdutor();
            this.CarregaTabelasDeComissionamentoDoProdutor();
            this.CarregaTabelasDeComissionamento();
            
            this.HabilitaAbaGrupoDeVenda();
            if (p4_GrupoVenda.Visible) { this.CarregaGruposDoProdutor(); }
            this.CarregaContatosDoProdutor();
        }

        /// <summary>
        /// Carrega o perfil superior ao perfil do usuário em foco.
        /// </summary>
        void CarregaPerfilSuperior(Object usuarioPerfilID, Object superiorID)
        {
            cboSuperior.Items.Clear();
            Perfil perfil = new Perfil();

            if (superiorID != null)
            {
                Usuario superior = new Usuario();
                superior.ID = superiorID;
                superior.Carregar();
                perfil.ID = superior.PerfilID;
            }
            else
                perfil.ID = usuarioPerfilID;

            perfil.Carregar();

            //this.HabilitaTRsParaPerfilCorrente(perfil);

            //if (perfil.ParentID == null)
            //{
            //    InabilitaComboSuperiores();
            //    return;
            //}

            if (usuarioPerfilID != null)
            {
                //cboSuperiorPerfil.SelectedValue = Convert.ToString(perfil.ParentID);
                if (perfil.ParentID != null)
                {
                    cboSuperiorPerfil.SelectedValue = Convert.ToString(perfil.ParentID);
                    cboSuperior.Enabled = true;
                    cboSuperiorPerfil.Enabled = true;
                    CarregaOpcoesParaSuperior(perfil.ParentID);
                }
                else
                {
                    //cboSuperior.Enabled = false;
                    //cboSuperiorPerfil.Enabled = false;
                }
            }
            else if (superiorID != null)
            {
                cboSuperiorPerfil.SelectedValue = Convert.ToString(perfil.ID);
                CarregaOpcoesParaSuperior(perfil.ID);
                cboSuperior.SelectedValue = Convert.ToString(superiorID);
            }

            //cboSuperior.Enabled = true;
            //Perfil perfilSuperior = new Perfil();
            //perfilSuperior.ID = perfil.ParentID;
            //perfilSuperior.Carregar();

            //spanSuperior.InnerText = perfilSuperior.Descricao;
            //cboSuperiorPerfil.SelectedValue = Convert.ToString(perfil.ID);
        }

        /// <summary>
        /// Checa se pode exibir e configura as tr's de categoria de comissionamento e 
        /// filial.
        /// </summary>
        //void HabilitaTRsParaPerfilCorrente(Perfil perfil)
        //{
        //    //trCategoriaComissionamento.Visible = perfil.ParticipanteContrato;
        //    trFilial.Visible = perfil.ParticipanteContrato || perfil.Comissionavel;
        //}

        void InabilitaComboSuperiores()
        {
            //cboSuperior.Enabled = false;
            //cboSuperiorPerfil.Enabled = false;
            //spanSuperior.InnerText = "Superior";
            if (cboSuperior.Items.Count > 0)
                cboSuperior.SelectedIndex = 0;
        }

        void CarregaPerfis()
        {
            IList<Perfil> lista = Perfil.CarregarTodos(new Perfil.eTipo[] { Perfil.eTipo.Produtor, Perfil.eTipo.Telemarketing });
            cboPerfil.DataValueField = "ID";
            cboPerfil.DataTextField = "Descricao";
            cboPerfil.DataSource = lista;
            cboPerfil.DataBind();

            cboSuperiorPerfil.DataValueField = "ID";
            cboSuperiorPerfil.DataTextField  = "Descricao";
            cboSuperiorPerfil.DataSource = lista;
            cboSuperiorPerfil.DataBind();
            //cboSuperiorPerfil.Items.Insert(0, new ListItem("nenhum", "-1"));
        }

        void CarregaCategorias()
        {
            base.ExibirCategorias(cboCategoriaComissionamento, true, true);
        }

        void CarregaOpcoesParaSuperior(Object perfilId)
        {
            if (Convert.ToInt32(perfilId) != -1 && base.HaItemSelecionado(cboSuperiorPerfil))
            {
                cboSuperior.Items.Clear();
                cboSuperior.DataValueField = "ID";
                cboSuperior.DataTextField = "Nome";
                cboSuperior.DataSource = Usuario.CarregarTodos_Parcial(perfilId);
                cboSuperior.DataBind();
                //cboSuperior.Items.Insert(0, new ListItem("selecione", "-1"));

                if (ViewState[IDKey] != null)
                {
                    for (int i = 0; i < cboSuperior.Items.Count; i++)
                    {
                        if (cboSuperior.Items[i].Value == Convert.ToString(ViewState[IDKey]))
                        {
                            cboSuperior.Items.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            else
            {
                cboSuperior.Items.Clear();
            }
        }

        protected void optChanged(Object sender, EventArgs e)
        {
            //txtCPFouCNPJ.Text = "";
            if (optFisica.Checked)
            {
                lblCPFouCNPJ.Text = "CPF";
                lblRGouIE.Text = "RG";
                meeCPF.Mask = "999,999,999-99";
                tblDadosPessoaFisica.Visible = true;
                litBr.Text = "<br>";
            }
            else
            {
                lblCPFouCNPJ.Text = "CNPJ";
                lblRGouIE.Text = "IE";
                meeCPF.Mask = "99,999,999/9999-99";
                tblDadosPessoaFisica.Visible = false;
                litBr.Text = "";
            }

            txtCPFouCNPJ.Focus();
            HabilitaAbaContatos();
        }

        protected void cmdBuscaEndereco_Click(Object sender, EventArgs e)
        {
            base.PegaEndereco(this.Page, txtCEP, txtLogradouro, txtBairro, txtCidade, txtUF, txtNumero, true);
        }

        //protected void lnkUser_Click(Object sender, EventArgs e)
        //{
        //    pnlUser.Visible = lnkUser.Checked;
        //}

        protected void cboSuperiorPerfil_Change(Object sender, EventArgs e)
        {
            CarregaOpcoesParaSuperior(cboSuperiorPerfil.SelectedValue);
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/users.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (!IsValid) { return; }

            #region validacoes 

            //if (txtCodigo.Text.Trim() == "")
            //{
            //    //base.Alerta(null, this, "_errCod", "Você deve informar o código.");
            //    base.Alerta(MPE, ref litAlert, "Você deve informar o código.", upnlAlerta);
            //    txtCodigo.Focus();
            //    return;
            //}

            String msg = "";
            if (txtEmail.Text.Trim() != "" && !base.ValidaEmail(txtEmail.Text, out msg))
            {
                //base.Alerta(null, this, "_errMail", msg);
                base.Alerta(MPE, ref litAlert, msg, upnlAlerta);
                txtEmail.Focus();
                return;
            }

            /////////////////////////////////////
            if (optFisica.Checked)
            {
                if (txtDataNascimento.Text.Trim() == "")
                {
                    //base.Alerta(null, this.Page, "_err10", "Você deve informar a data de nascimento.");
                    base.Alerta(MPE, ref litAlert, "Você deve informar a data de nascimento.", upnlAlerta);
                    txtDataNascimento.Focus();
                    return;
                }
                else
                {
                    DateTime data = new DateTime();
                    if (!UIHelper.TyParseToDateTime(txtDataNascimento.Text, out data))
                    {
                        //base.Alerta(null, this.Page, "_err10b", "A data de nascimento informada está inválida.");
                        base.Alerta(MPE, ref litAlert, "A data de nascimento informada está inválida.", upnlAlerta);
                        txtDataNascimento.Focus();
                        return;
                    }
                }
            }
            /////////////////////////////////////
            if (txtUF.Text.Trim() != "" && !UIHelper.ValidaUF(txtUF.Text))
            {
                //base.Alerta(null, this, "_err11", "Unidade Federativa inválida.");
                base.Alerta(MPE, ref litAlert, "Unidade Federativa inválida.", upnlAlerta);
                txtUF.Focus();
                return;
            }

            if (txtLogradouro.Text.Trim() != "" && txtCEP.Text.Trim() != "" && txtBairro.Text.Trim() != ""
                && txtBairro.Text.Trim() != "" && txtCidade.Text.Trim() != "" && txtNumero.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_err11a", "Informe o endereço corretamente.");
                base.Alerta(MPE, ref litAlert, "Informe o endereço corretamente.", upnlAlerta);
                tab.ActiveTabIndex = 0;
                txtNumero.Focus();
                return;
            }
            /////////////////////////////////////
            if (txtCPFouCNPJ.Text.Trim() == "")
            {
                base.Alerta(MPE, ref litAlert, "O campo " + lblCPFouCNPJ.Text + " deve ser preenchido.", upnlAlerta);
                txtCPFouCNPJ.Focus();
                return;
            }
            else if (!Usuario.ChecaDocumento(ViewState[IDKey], txtCPFouCNPJ.Text, out msg, lblCPFouCNPJ.Text == "CPF"))
            {
                //base.Alerta(null, this, "_err12", "Documento informado inválido ou já utilizado.");
                base.Alerta(MPE, ref litAlert, msg, upnlAlerta);
                txtCPFouCNPJ.Focus();
                return;
            }
            /////////////////////////////////////
            if (txtEntrevistadoEm.Text.Trim() != "")
            {
                DateTime data = new DateTime();
                if (!UIHelper.TyParseToDateTime(txtEntrevistadoEm.Text, out data))
                {
                    //base.Alerta(null, this.Page, "_err13", "A data de entrevista informada está inválida.");
                    base.Alerta(MPE, ref litAlert, "A data de entrevista informada está inválida.", upnlAlerta);
                    txtEntrevistadoEm.Focus();
                    return;
                }
            }

            #endregion

            Usuario user = new Usuario();
            Endereco endereco = new Endereco();
            endereco.DonoTipo = (Int32)Endereco.TipoDono.Produtor;

            if (ViewState[IDKey] != null)
            {
                user.ID = ViewState[IDKey];
                user.Carregar();
            }

            if (ViewState[IDKey2] != null)
            {
                endereco.ID = ViewState[IDKey2];
                endereco.Carregar();
            }

            if (txtNome.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_erro3", "Informe seu nome.");
                base.Alerta(MPE, ref litAlert, "Informe seu nome.", upnlAlerta);
                txtNome.Focus();
                return;
            }

            //if (txtEmail.Text.Trim() == "")
            //{
            //    base.Alerta(null, this, "_erro4", "Informe seu e-mail.");
            //    txtEmail.Focus();
            //    return;
            //}

            user.Nome = txtNome.Text;
            user.Apelido = txtApelido.Text;
            user.Email = txtEmail.Text;
            user.MarcaOtica = txtMarcaOtica.Text;
            user.Codigo = txtMarcaOtica.Text;

            //user.LiberaContratos = chkLibera.Checked;
            //user.AlteraValorContratos = chkAlteraValor.Checked;

            if(cboPerfil.Items.Count >0)
                user.PerfilID = cboPerfil.SelectedValue;

            if (cboSuperior.SelectedIndex > 0)
                user.SuperiorID = cboSuperior.SelectedValue;

            if (trCategoriaComissionamento.Visible)
            {
                if (cboCategoriaComissionamento.SelectedIndex <= 0)
                {
                    //base.Alerta(null, this, "_erro1", "Informe a categoria de comissionamento.");
                    base.Alerta(MPE, ref litAlert, "Informe a categoria de comissionamento.", upnlAlerta);
                    cboCategoriaComissionamento.Focus();
                    return;
                }
                else
                    user.CategoriaID = cboCategoriaComissionamento.SelectedValue;
            }
            else
                user.CategoriaID = null;

            if (txtDataNascimento.Text.Trim() != "" && optFisica.Checked)
                user.DataNascimento = base.CStringToDateTime(txtDataNascimento.Text);

            user.Sexo = Convert.ToInt32(cboSexo.SelectedValue);
            user.EstadoCivil = Convert.ToInt32(cboEstadoCivil.SelectedValue);
            if (optFisica.Checked)
                user.TipoPessoa = 1;
            else
                user.TipoPessoa = 2;

            user.Documento1 = txtCPFouCNPJ.Text;
            user.Documento2 = txtRGouIE.Text;

            user.DDD1 = txtDDD1.Text;
            user.Fone1 = txtFone1.Text;
            user.Ramal1 = txtRamal1.Text;
            user.DDD2 = txtDDD2.Text;
            user.Fone2 = txtFone2.Text;
            user.Ramal2 = txtRamal2.Text;
            user.DDD3 = txtDDDCelular.Text;// txtDDD3.Text;
            user.Celular = txtCelular.Text;// txtFone3.Text;
            user.CelularOperadora = txtCelularOperadora.Text;
            //user.Ramal3 = txtRamal3.Text;

            if (txtEntrevistadoEm.Text.Trim() != "")
                user.EntrevistadoEm = base.CStringToDateTime(txtEntrevistadoEm.Text);
            else
                user.EntrevistadoEm = DateTime.MinValue;

            user.EntrevistadoPor = txtEntrevistadoPor.Text;

            user.Banco = txtBanco.Text;
            user.Agencia = txtAgencia.Text;
            user.ContaTipo = cboTipoContaBancaria.SelectedValue;
            user.Conta = txtContaCorrente.Text;
            user.Favorecido = txtFavorecido.Text;

            user.Obs = txtObs.Text;

            user.Salvar();
            ViewState[IDKey] = user.ID;

            endereco.DonoId = user.ID;
            endereco.Bairro = txtBairro.Text;
            endereco.CEP = txtCEP.Text;
            endereco.Cidade = txtCidade.Text;
            endereco.Complemento = txtComplemento.Text;
            endereco.Logradouro = txtLogradouro.Text;
            endereco.Numero = txtNumero.Text;
            endereco.UF = txtUF.Text;
            endereco.Salvar();
            ViewState[IDKey2] = endereco.ID;

            tab.Tabs[1].Visible = true;
            tab.Tabs[2].Visible = true;
            tab.Tabs[3].Visible = true;
            this.HabilitaAbaContatos();
            this.HabilitaAbaGrupoDeVenda();
            this.CarregaOpcoesParaSuperior(cboSuperiorPerfil.SelectedValue);
            //base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
        }

        protected void cboPerfil_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaPerfilSuperior(cboPerfil.SelectedValue, null);
            this.HabilitaAbaGrupoDeVenda();
            this.CarregaTabelasDeComissionamento();
        }

        void HabilitaAbaGrupoDeVenda()
        {
            if (ViewState[IDKey] == null) { return; }
            if (cboPerfil.Items.Count == 0) { return; }
            Perfil perfil = new Perfil(cboPerfil.SelectedValue);
            perfil.Carregar();
            if (perfil.ParticipanteContrato)
            {
                p4_GrupoVenda.Visible = true;
                trTabelaGrupoVenda.Visible = false;
            }
            else
            {
                if (tab.ActiveTabIndex == 4) { tab.ActiveTabIndex = 3; }
                p4_GrupoVenda.Visible = false;
                trTabelaGrupoVenda.Visible = true;
            }
        }

        void HabilitaAbaContatos()
        {
            if (ViewState[IDKey] == null) { return; }
            if (optFisica.Checked)
                p5_Contatos.Visible = false;
            else
                p5_Contatos.Visible = true;
        }

        #region filial 

        void CarregaFiliaisDoProdutor()
        {
            if (ViewState[IDKey] == null) { return; }
            gridFiliais.DataSource = UsuarioFilial.CarregarTodos(ViewState[IDKey]);
            gridFiliais.DataBind();

            if (gridFiliais.Rows.Count > 0)
                gridFiliais.Rows[0].Font.Bold = true;
        }

        protected void gridFiliais_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                if (!this.alteraProdutor) { return; }

                Object id = gridFiliais.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                UsuarioFilial obj = new UsuarioFilial();
                obj.ID = id;
                obj.Remover();
                this.CarregaFiliaisDoProdutor();

                if (gridFiliais.Rows.Count == 0)
                {
                    Usuario user = new Usuario();
                    user.ID = ViewState[IDKey];
                    user.Carregar();
                    user.FilialID = null;
                    user.Salvar();
                }
            }
        }

        protected void gridFiliais_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente excluir a linha?");
            }
        }

        protected void cmdAdicionarFilial_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (cboFilial.Items.Count == 0) { return; }

            if (gridFiliais.Rows.Count > 0)
            {
                if (Convert.ToString(gridFiliais.DataKeys[0][1]) == cboFilial.SelectedValue)
                {
                    //base.Alerta(null, this, "_errF", "Essa ja é a filial atual deste produtor.");
                    base.Alerta(MPE, ref litAlert, "Essa ja é a filial atual deste produtor.", upnlAlerta);
                    return;
                }
            }

            /////////////////////////////////////
            if (txtDataFilial.Text.Trim() == "")
            {
                //base.Alerta(null, this.Page, "_errF1", "Você deve informar a data.");
                base.Alerta(MPE, ref litAlert, "Você deve informar a data.", upnlAlerta);
                txtDataFilial.Focus();
                return;
            }
            else
            {
                DateTime data = new DateTime();
                if (!UIHelper.TyParseToDateTime(txtDataFilial.Text, out data))
                {
                    //base.Alerta(null, this.Page, "_errF1b", "A data informada está inválida.");
                    base.Alerta(MPE, ref litAlert, "A data informada está inválida.", upnlAlerta);
                    txtDataFilial.Focus();
                    return;
                }
            }
            /////////////////////////////////////

            #endregion

            UsuarioFilial obj = new UsuarioFilial();
            obj.Data = base.CStringToDateTime(txtDataFilial.Text);
            obj.FilialID = cboFilial.SelectedValue;
            obj.UsuarioID = ViewState[IDKey];
            obj.Salvar();

            UsuarioFilial.SetaAtual(ViewState[IDKey]);

            this.CarregaFiliaisDoProdutor();
            txtDataFilial.Text = "";
        }

        #endregion

        #region comissionamento 

        void CarregaTabelasDeComissionamentoDoProdutor()
        {
            if (ViewState[IDKey] == null) { return; }
            gridTabelasDoUsuario.DataSource = ComissionamentoUsuario.Carregar(ViewState[IDKey]);
            gridTabelasDoUsuario.DataBind();

            if(gridTabelasDoUsuario.Rows.Count > 0)
                gridTabelasDoUsuario.Rows[0].Font.Bold = true;
        }

        void CarregaTabelasDeComissionamento()
        {
            cboTabela.Items.Clear();
            cboTabela.DataValueField = "ID";
            cboTabela.DataTextField  = "Descricao";
            cboTabela.DataSource = Comissionamento.CarregarTodos(Comissionamento.eTipo.PagoAoOperador, cboPerfil.SelectedValue);
            cboTabela.DataBind();
        }

        protected void gridTabelasDoUsuario_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                if (!this.alteraProdutor) { return; }

                Object id = gridTabelasDoUsuario.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                ComissionamentoUsuario obj = new ComissionamentoUsuario();
                obj.ID = id;
                obj.Remover();
                this.CarregaTabelasDeComissionamentoDoProdutor();
            }
            else if (e.CommandName.Equals("tabelaExcecao"))
            {
                gridTabelasDoUsuario.SelectedIndex = Convert.ToInt32(e.CommandArgument);

                //cmdNovaTabelaDeExcecao.Visible = false;
                pnlTabelaExcecaoLista.Visible = false;
                pnlTabelaExcecaoDetalhe.Visible = true;

                //TODO: 
                //checar se ja nao há uma tabela de excecao ja criada para esta tabela de comissionamento

                List<ExcecaoItem> lista = new List<ExcecaoItem>();
                lista.Add(new ExcecaoItem());
                lista.Add(new ExcecaoItem());
                lista.Add(new ExcecaoItem());

                gridItens.DataSource = lista;
                gridItens.DataBind();
                this.Itens = lista;
            }
        }

        protected void gridTabelasDoUsuario_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente excluir a linha?");
                e.Row.Cells[7].Attributes.Add("onClick", "win = window.open('../comissionamentoP.aspx?" + IDKey + "=" + gridTabelasDoUsuario.DataKeys[e.Row.RowIndex][1] + "', 'janela', 'toolbar=no,scrollbars=1,width=480,height=430'); win.moveTo(100,150); return false;");
            }
        }

        protected void cmdAdicionar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (cboTabela.Items.Count == 0) { return; }

            if (gridTabelasDoUsuario.Rows.Count > 0)
            {
                if (Convert.ToString(gridTabelasDoUsuario.DataKeys[0][1]) == cboTabela.SelectedValue)
                {
                    if (trTabelaGrupoVenda.Visible)
                    {
                        if (Convert.ToString(gridTabelasDoUsuario.DataKeys[0][2]) == cboTabelaGrupoVenda.SelectedValue)
                        {
                            base.Alerta(MPE, ref litAlert, "Essa ja é a tabela atual deste produtor.", upnlAlerta);
                            return;
                        }
                    }
                    else
                    {
                        //base.Alerta(null,this,"_errTC", "Essa ja é a tabela atual deste produtor.");
                        base.Alerta(MPE, ref litAlert, "Essa ja é a tabela atual deste produtor.", upnlAlerta);
                        return;
                    }
                }
            }

            /////////////////////////////////////
            if (txtDataVigencia.Text.Trim() == "")
            {
                //base.Alerta(null, this.Page, "_errTC1", "Você deve informar a data de vigência.");
                base.Alerta(MPE, ref litAlert, "Você deve informar a data de vigência.", upnlAlerta);
                txtDataVigencia.Focus();
                return;
            }
            else
            {
                DateTime data = new DateTime();
                if (!UIHelper.TyParseToDateTime(txtDataVigencia.Text, out data))
                {
                    //base.Alerta(null, this.Page, "_errTC1b", "A data de vigência informada está inválida.");
                    base.Alerta(MPE, ref litAlert, "A data de vigência informada é inválida.", upnlAlerta);
                    txtDataVigencia.Focus();
                    return;
                }
            }

            Comissionamento tabela = new Comissionamento(cboTabela.SelectedValue);
            tabela.Carregar();
            DateTime dataTabela = new DateTime(tabela.Data.Year, tabela.Data.Month, tabela.Data.Day);
            if (base.CStringToDateTime(txtDataVigencia.Text) < dataTabela)
            {
                //base.Alerta(null, this.Page, "_errTC10b", "A data de vigência informada não pode ser inferior \\nà data de vigência da tabela de comissionamento.");
                base.Alerta(MPE, ref litAlert, "A data de vigência informada não pode ser inferior <br>à data de vigência da tabela de comissionamento.", upnlAlerta);
                txtDataVigencia.Focus();
                return;
            }
            /////////////////////////////////////
            if (gridTabelasDoUsuario.Rows.Count > 0)
            {
                IList<ComissionamentoUsuario> tabelas = ComissionamentoUsuario.Carregar(ViewState[IDKey]);
                foreach (ComissionamentoUsuario _tabela in tabelas)
                {
                    if (_tabela.Data.ToString("dd/MM/yyyy") == base.CStringToDateTime(txtDataVigencia.Text).ToString("dd/MM/yyyy"))
                    {
                        if ((_tabela.GrupoVendaID != null && Convert.ToString(_tabela.GrupoVendaID) == cboTabelaGrupoVenda.SelectedValue) || _tabela.GrupoVendaID == null)
                        {
                            base.Alerta(MPE, ref litAlert, "Já há uma tabela com essa data de vigência.", upnlAlerta);
                            txtDataVigencia.Focus();
                            return;
                        }
                    }
                }
            }

            if (trTabelaGrupoVenda.Visible && cboTabelaGrupoVenda.Items.Count == 0)
            {
                base.Alerta(MPE, ref litAlert, "Não há um grupo de venda selecionado.", upnlAlerta);
                return;
            }


            #endregion

            ComissionamentoUsuario obj = new ComissionamentoUsuario();
            obj.UsuarioID = ViewState[IDKey];
            obj.PerfilID = cboPerfil.SelectedValue;
            obj.TabelaComissionamentoID = cboTabela.SelectedValue;
            obj.Data = base.CStringToDateTime(txtDataVigencia.Text);
            if (trTabelaGrupoVenda.Visible)
                obj.GrupoVendaID = cboTabelaGrupoVenda.SelectedValue;
            obj.Salvar();
            this.CarregaTabelasDeComissionamentoDoProdutor();
            txtDataVigencia.Text = "";
            //base.Alerta(null, this, "_ok2", "Tabela de comissionamento relacionada ao produtor com sucesso.");
            base.Alerta(MPE, ref litAlert, "Tabela de comissionamento relacionada ao produtor com sucesso.", upnlAlerta);
        }

        #endregion

        #region tabelas de excecao 

        void CarregaTabelasDeExcecao()
        {
            gridTabelasDeExcecao.DataSource = TabelaExcecao.Carregar(ViewState[IDKey]);
            gridTabelasDeExcecao.DataBind();

            if (gridTabelasDeExcecao.DataSource == null)
                litTabelaExcecaoLista.Text = "&nbsp;&nbsp;<font size='1'><i>(nenhuma)</i></font>";
            else
                litTabelaExcecaoLista.Text = "";
        }

        protected void gridTabelasDeExcecao_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                if (!this.alteraProdutor) { return; }

                Object id = gridTabelasDeExcecao.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                TabelaExcecao obj = new TabelaExcecao(id);

                try
                {
                    obj.Remover();
                    this.CarregaTabelasDeExcecao();
                }
                catch
                {
                    Alerta(null, this, "_DelErrTE", "Não foi possível excluir a tabela de exceção.");
                }
            }
            else if (e.CommandName.Equals("editar"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                gridTabelasDeExcecao.SelectedIndex = index;
                Object id = gridTabelasDeExcecao.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;

                TabelaExcecao tabela = new TabelaExcecao(id);
                tabela.Carregar();

                for (int i = 0; i < gridTabelasDoUsuario.Rows.Count; i++)
                {
                    if (Convert.ToInt32(gridTabelasDoUsuario.DataKeys[i][1]) == Convert.ToInt32(tabela.TabelaComissionamentoID))
                    {
                        gridTabelasDoUsuario.SelectedIndex = i;
                        break;
                    }
                }

                this.CarregaVitaliciedade(tabela.ID);

                ContratoADM contrato = new ContratoADM(tabela.ContratoAdmID);
                contrato.Carregar();

                cboTabelaExcecaoOperadora.SelectedValue = Convert.ToString(contrato.OperadoraID);
                cboTabelaExcecaoOperadora_OnSelectedIndexChanged(null, null);
                cboTabelaExcecaoContrato.SelectedValue = Convert.ToString(contrato.ID);
                txtTabelaExcecaoVigencia.Text = tabela.Vigencia.ToString("dd/MM/yyyy");

                List<ExcecaoItem> itens = (List<ExcecaoItem>)ExcecaoItem.CarregarPorTabelaExcecaoID(tabela.ID);
                this.Itens = itens;
                gridItens.DataSource = itens;
                gridItens.DataBind();

                //cmdNovaTabelaDeExcecao.Visible = false;
                pnlTabelaExcecaoLista.Visible = false;
                pnlTabelaExcecaoDetalhe.Visible = true;
            }
        }

        protected void gridTabelasDeExcecao_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 3, "Deseja realmente excluir a tabela de exceção?");
            }
        }

        void CarregaVitaliciedade(Object tabelaId)
        {
            if (tabelaId != null)
            {
                #region NORMAL
                TabelaExcecaoVitaliciedade cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal);

                if (cv != null)
                {
                    chkComissionamentoVitalicio.Checked = cv.Vitalicia;
                    if (chkComissionamentoVitalicio.Checked)
                    {
                        txtComissionamentoNumeroParcelaVitalicio.Text = Convert.ToString(cv.ParcelaInicio);
                        txtComissionamentoVitalicioPercentual.Text = cv.Percentual.ToString("N2");
                    }
                    else
                    {
                        txtComissionamentoNumeroParcelaVitalicio.Text = "";
                        txtComissionamentoVitalicioPercentual.Text = "";
                    }
                }
                else
                {
                    chkComissionamentoVitalicio.Checked = false;
                    txtComissionamentoNumeroParcelaVitalicio.Text = "";
                    txtComissionamentoVitalicioPercentual.Text = "";
                }
                #endregion

                #region CARENCIA
                cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia);

                if (cv != null)
                {
                    chkComissionamentoVitalicioCarencia.Checked = cv.Vitalicia;
                    if (chkComissionamentoVitalicioCarencia.Checked)
                    {
                        txtComissionamentoNumeroParcelaVitalicioCarencia.Text = Convert.ToString(cv.ParcelaInicio);
                        txtComissionamentoVitalicioPercentualCarencia.Text = cv.Percentual.ToString("N2");
                    }
                    else
                    {
                        txtComissionamentoNumeroParcelaVitalicioCarencia.Text = "";
                        txtComissionamentoVitalicioPercentualCarencia.Text = "";
                    }
                }
                else
                {
                    chkComissionamentoVitalicioCarencia.Checked = false;
                    txtComissionamentoNumeroParcelaVitalicioCarencia.Text = "";
                    txtComissionamentoVitalicioPercentualCarencia.Text = "";
                }
                #endregion

                #region MIGRACAO
                cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao);

                if (cv != null)
                {
                    chkComissionamentoVitalicioMigracao.Checked = cv.Vitalicia;
                    if (chkComissionamentoVitalicioMigracao.Checked)
                    {
                        txtComissionamentoNumeroParcelaVitalicioMigracao.Text = Convert.ToString(cv.ParcelaInicio);
                        txtComissionamentoVitalicioPercentualMigracao.Text = cv.Percentual.ToString("N2");
                    }
                    else
                    {
                        txtComissionamentoNumeroParcelaVitalicioMigracao.Text = "";
                        txtComissionamentoVitalicioPercentualMigracao.Text = "";
                    }
                }
                else
                {
                    chkComissionamentoVitalicioMigracao.Checked = false;
                    txtComissionamentoNumeroParcelaVitalicioMigracao.Text = "";
                    txtComissionamentoVitalicioPercentualMigracao.Text = "";
                }
                #endregion

                #region ADM
                cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa);

                if (cv != null)
                {
                    chkComissionamentoVitalicioADM.Checked = cv.Vitalicia;
                    if (chkComissionamentoVitalicioADM.Checked)
                    {
                        txtComissionamentoNumeroParcelaVitalicioADM.Text = Convert.ToString(cv.ParcelaInicio);
                        txtComissionamentoVitalicioPercentualADM.Text = cv.Percentual.ToString("N2");
                    }
                    else
                    {
                        txtComissionamentoNumeroParcelaVitalicioADM.Text = "";
                        txtComissionamentoVitalicioPercentualADM.Text = "";
                    }
                }
                else
                {
                    chkComissionamentoVitalicioADM.Checked = false;
                    txtComissionamentoNumeroParcelaVitalicioADM.Text = "";
                    txtComissionamentoVitalicioPercentualADM.Text = "";
                }
                #endregion

                #region ESPECIAL
                cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial);

                if (cv != null)
                {
                    chkComissionamentoVitalicioEspecial.Checked = cv.Vitalicia;
                    if (chkComissionamentoVitalicioEspecial.Checked)
                    {
                        txtComissionamentoNumeroParcelaVitalicioEspecial.Text = Convert.ToString(cv.ParcelaInicio);
                        txtComissionamentoVitalicioPercentualEspecial.Text = cv.Percentual.ToString("N2");
                    }
                    else
                    {
                        txtComissionamentoNumeroParcelaVitalicioEspecial.Text = "";
                        txtComissionamentoVitalicioPercentualEspecial.Text = "";
                    }
                }
                else
                {
                    chkComissionamentoVitalicioEspecial.Checked = false;
                    txtComissionamentoNumeroParcelaVitalicioEspecial.Text = "";
                    txtComissionamentoVitalicioPercentualEspecial.Text = "";
                }
                #endregion

                #region IDADE
                cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade);

                if (cv != null)
                {
                    chkComissionamentoVitalicioIdade.Checked = cv.Vitalicia;
                    if (chkComissionamentoVitalicioIdade.Checked)
                    {
                        txtComissionamentoNumeroParcelaVitalicioIdade.Text = Convert.ToString(cv.ParcelaInicio);
                        txtComissionamentoVitalicioPercentualIdade.Text = cv.Percentual.ToString("N2");
                    }
                    else
                    {
                        txtComissionamentoNumeroParcelaVitalicioIdade.Text = "";
                        txtComissionamentoVitalicioPercentualIdade.Text = "";
                    }
                }
                else
                {
                    chkComissionamentoVitalicioIdade.Checked = false;
                    txtComissionamentoNumeroParcelaVitalicioIdade.Text = "";
                    txtComissionamentoVitalicioPercentualIdade.Text = "";
                }
                #endregion
            }
            else
            {
                #region limpa campos 

                chkComissionamentoVitalicio.Checked = false;
                txtComissionamentoNumeroParcelaVitalicio.Text = "";
                txtComissionamentoVitalicioPercentual.Text = "";

                chkComissionamentoVitalicioCarencia.Checked = false;
                txtComissionamentoNumeroParcelaVitalicioCarencia.Text = "";
                txtComissionamentoVitalicioPercentualCarencia.Text = "";

                chkComissionamentoVitalicioMigracao.Checked = false;
                txtComissionamentoNumeroParcelaVitalicioMigracao.Text = "";
                txtComissionamentoVitalicioPercentualMigracao.Text = "";

                chkComissionamentoVitalicioADM.Checked = false;
                txtComissionamentoNumeroParcelaVitalicioADM.Text = "";
                txtComissionamentoVitalicioPercentualADM.Text = "";

                chkComissionamentoVitalicioEspecial.Checked = false;
                txtComissionamentoNumeroParcelaVitalicioEspecial.Text = "";
                txtComissionamentoVitalicioPercentualEspecial.Text = "";

                chkComissionamentoVitalicioIdade.Checked = false;
                txtComissionamentoNumeroParcelaVitalicioIdade.Text = "";
                txtComissionamentoVitalicioPercentualIdade.Text = "";

                #endregion
            }
        }

        protected void cmdNovaTabelaDeExcecao_OnClick(Object sender, EventArgs e)
        {
            //cmdNovaTabelaDeExcecao.Visible = false;
            pnlTabelaExcecaoLista.Visible = false;
            pnlTabelaExcecaoDetalhe.Visible = true;

            List<ExcecaoItem> lista = this.Itens;
            if (lista == null) { lista = new List<ExcecaoItem>(); }

            if (lista.Count == 0)
            {
                lista.Add(new ExcecaoItem());
                lista.Add(new ExcecaoItem());
                lista.Add(new ExcecaoItem());
            }

            gridItens.DataSource = lista;
            gridItens.DataBind();
            this.Itens = lista;
        }

        void CarregaItensDaTabelaDeExcecao()
        {
            List<ExcecaoItem> lista = null;

            if (cboTabelaExcecaoContrato.Items.Count > 0)
            {
                Object contratoID = cboTabelaExcecaoContrato.SelectedValue;
                lista = (List<ExcecaoItem>)ExcecaoItem.Carregar(ViewState[IDKey], contratoID);
            }

            gridItens.DataSource = lista;
            gridItens.DataBind();
            this.Itens = lista;

            //if (lista == null || lista.Count == 0) { this.MontaGridParaPrimeiraInsercao(); }
            //pnl2.Visible = lista != null && lista.Count > 0;
            //cmdAdicionar.Visible = (lista == null || lista.Count == 0) && gridContratos.SelectedIndex > -1; //&& cboContrato.Items.Count > 0;
        }

        void CarregaContratosDaOperadora()
        {
            cboTabelaExcecaoContrato.Items.Clear();
            if (cboTabelaExcecaoOperadora.Items.Count == 0) { return; }

            cboTabelaExcecaoContrato.DataValueField = "ID";
            cboTabelaExcecaoContrato.DataTextField = "Descricao";
            cboTabelaExcecaoContrato.DataSource = ContratoADM.Carregar(cboTabelaExcecaoOperadora.SelectedValue);
            cboTabelaExcecaoContrato.DataBind();
        }

        protected void cboTabelaExcecaoOperadora_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratosDaOperadora();
        }

        protected void gridItens_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "excluir")
            {
                if (!this.alteraProdutor) { return; }

                //Se nao houver mais nenuma linha de parcela, excluir tb o comissionamento vitalicio?
                Object id = gridItens.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                if (id == null)
                {
                    List<ExcecaoItem> lista = this.Itens;
                    lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                    this.Itens = lista;
                    gridItens.DataSource = lista;
                    gridItens.DataBind();
                }
                else
                {
                    ExcecaoItem item = new ExcecaoItem();
                    item.ID = id;
                    item.Carregar();
                    item.Remover();
                    this.CarregaItensDaTabelaDeExcecao();
                }
            }
        }

        protected void gridItens_RowDataBound(Object sender, GridViewRowEventArgs e)
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

                TextBox txtvalor4 = (TextBox)e.Row.Cells[4].Controls[1];
                txtvalor4.Attributes.Add("onKeyUp", "mascara('" + txtvalor4.ClientID + "')");

                TextBox txtvalor5 = (TextBox)e.Row.Cells[5].Controls[1];
                txtvalor5.Attributes.Add("onKeyUp", "mascara('" + txtvalor5.ClientID + "')");

                TextBox txtvalor6 = (TextBox)e.Row.Cells[6].Controls[1];
                txtvalor6.Attributes.Add("onKeyUp", "mascara('" + txtvalor6.ClientID + "')");

                Object id = gridItens.DataKeys[e.Row.RowIndex].Value;

                if (id == null)
                {
                    if (CToDecimal(txtvalor1.Text) == 0) { txtvalor1.Text = ""; }
                    if (CToDecimal(txtvalor2.Text) == 0) { txtvalor2.Text = ""; }
                    if (CToDecimal(txtvalor3.Text) == 0) { txtvalor3.Text = ""; }
                    if (CToDecimal(txtvalor4.Text) == 0) { txtvalor4.Text = ""; }
                    if (CToDecimal(txtvalor5.Text) == 0) { txtvalor5.Text = ""; }
                    if (CToDecimal(txtvalor6.Text) == 0) { txtvalor6.Text = ""; }
                }
            }
        }

        protected void cmdAddItemTabelaExcecao_Click(Object sender, EventArgs e)
        {
            List<ExcecaoItem> lista = this.Itens;
            if (lista == null) { lista = new List<ExcecaoItem>(); }

            lista.Add(new ExcecaoItem());

            gridItens.DataSource = lista;
            gridItens.DataBind();
            this.Itens = lista;
        }

        void FechaModoDetalhe()
        {
            //cmdNovaTabelaDeExcecao.Visible = true;
            pnlTabelaExcecaoLista.Visible = true;
            pnlTabelaExcecaoDetalhe.Visible = false;
            gridTabelasDeExcecao.SelectedIndex = -1;
            gridTabelasDoUsuario.SelectedIndex = -1;
            this.Itens = null;
        }

        protected void cmdVoltarTabelaExcecao_Click(Object sender, EventArgs e)
        {
            this.FechaModoDetalhe();
        }

        protected void cmdSalvarTabelaExcecao_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            DateTime data = new DateTime();
            if (!UIHelper.TyParseToDateTime(txtTabelaExcecaoVigencia.Text, out data))
            {
                base.Alerta(null, this.Page, "__errTE00", "A data de vigência informada está inválida.");
                //base.Alerta(MPE, ref litAlert, "A data de vigência informada é inválida.", upnlAlerta);
                txtTabelaExcecaoVigencia.Focus();
                return;
            }

            if (cboTabelaExcecaoContrato.Items.Count == 0)
            {
                base.Alerta(null, this.Page, "__errTE02", "Não há um contrato administrativo selecionado.");
                cboTabelaExcecaoContrato.Focus();
                return;
            }

            String ret = UIHelper.ChecaGridDeParcelas(gridItens, 0);
            if (!String.IsNullOrEmpty(ret))
            {
                base.Alerta(null, this, "__errTE01", ret);
                return;
            }

            #region vitaliciedade 

            int result = 0;
            Decimal result2 = 0;

            #region NORMAL 

            if (chkComissionamentoVitalicio.Checked)
            {
                if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicio.Text, out result) || base.CToInt(txtComissionamentoNumeroParcelaVitalicio.Text) == 0)
                {
                    base.Alerta(null, this, "_erroTEV1", "Informe o número da parcela para tabela vitalícia.");
                    //ExibeMsg("Informe o número da parcela para tabela vitalícia.", true);
                    txtComissionamentoNumeroParcelaVitalicio.Focus();
                    return;
                }

                if (!Decimal.TryParse(txtComissionamentoVitalicioPercentual.Text, out result2))
                {
                    base.Alerta(null, this, "_erroTEV2", "Informe o percentual vitalício.");
                    //ExibeMsg("Informe o percentual vitalício.", true);
                    txtComissionamentoVitalicioPercentual.Focus();
                    return;
                }
            }
            #endregion

            #region CARENCIA

            if (chkComissionamentoVitalicioCarencia.Checked)
            {
                if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicioCarencia.Text, out result) || base.CToInt(txtComissionamentoNumeroParcelaVitalicioCarencia.Text) == 0)
                {
                    base.Alerta(null, this, "_erro1", "Informe o número da parcela para tabela vitalícia.");
                    txtComissionamentoNumeroParcelaVitalicioCarencia.Focus();
                    return;
                }

                if (!Decimal.TryParse(txtComissionamentoVitalicioPercentualCarencia.Text, out result2))
                {
                    base.Alerta(null, this, "_erro2", "Informe o percentual vitalício.");
                    txtComissionamentoVitalicioPercentualCarencia.Focus();
                    return;
                }
            }
            #endregion

            #region MIGRACAO

            if (chkComissionamentoVitalicioMigracao.Checked)
            {
                if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicioMigracao.Text, out result) || base.CToInt(txtComissionamentoNumeroParcelaVitalicioMigracao.Text) == 0)
                {
                    base.Alerta(null, this, "_erro1", "Informe o número da parcela para tabela vitalícia.");
                    txtComissionamentoNumeroParcelaVitalicioMigracao.Focus();
                    return;
                }

                if (!Decimal.TryParse(txtComissionamentoVitalicioPercentualMigracao.Text, out result2))
                {
                    base.Alerta(null, this, "_erro2", "Informe o percentual vitalício.");
                    txtComissionamentoVitalicioPercentualMigracao.Focus();
                    return;
                }
            }
            #endregion

            #region ADM

            if (chkComissionamentoVitalicioADM.Checked)
            {
                if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicioADM.Text, out result) || base.CToInt(txtComissionamentoNumeroParcelaVitalicioADM.Text) == 0)
                {
                    base.Alerta(null, this, "_erro1", "Informe o número da parcela para tabela vitalícia.");
                    txtComissionamentoNumeroParcelaVitalicioADM.Focus();
                    return;
                }

                if (!Decimal.TryParse(txtComissionamentoVitalicioPercentualADM.Text, out result2))
                {
                    base.Alerta(null, this, "_erro2", "Informe o percentual vitalício.");
                    txtComissionamentoVitalicioPercentualADM.Focus();
                    return;
                }
            }
            #endregion

            #region ESPECIAL

            if (chkComissionamentoVitalicioEspecial.Checked)
            {
                if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicioEspecial.Text, out result) || base.CToInt(txtComissionamentoNumeroParcelaVitalicioEspecial.Text) == 0)
                {
                    base.Alerta(null, this, "_erro1", "Informe o número da parcela para tabela vitalícia.");
                    txtComissionamentoNumeroParcelaVitalicioEspecial.Focus();
                    return;
                }

                if (!Decimal.TryParse(txtComissionamentoVitalicioPercentualEspecial.Text, out result2))
                {
                    base.Alerta(null, this, "_erro2", "Informe o percentual vitalício.");
                    txtComissionamentoVitalicioPercentualEspecial.Focus();
                    return;
                }
            }
            #endregion

            #region IDADE

            if (chkComissionamentoVitalicioIdade.Checked)
            {
                if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicioIdade.Text, out result) || base.CToInt(txtComissionamentoNumeroParcelaVitalicioIdade.Text) == 0)
                {
                    base.Alerta(null, this, "_erroTEV1", "Informe o número da parcela para tabela vitalícia.");
                    txtComissionamentoNumeroParcelaVitalicioIdade.Focus();
                    return;
                }

                if (!Decimal.TryParse(txtComissionamentoVitalicioPercentualIdade.Text, out result2))
                {
                    base.Alerta(null, this, "_erro2", "Informe o percentual vitalício.");
                    txtComissionamentoVitalicioPercentualIdade.Focus();
                    return;
                }
            }
            #endregion

            #endregion

            if (gridTabelasDoUsuario.SelectedIndex > -1)
            {
                IList<TabelaExcecao> check = TabelaExcecao.Carregar(
                    gridTabelasDoUsuario.DataKeys[gridTabelasDoUsuario.SelectedIndex][1],
                    ViewState[IDKey], cboTabelaExcecaoContrato.SelectedValue, data, null);

                if (check != null && check.Count > 0)
                {
                    base.Alerta(null, this, "__errTE02", "Já existe uma tabela de exeção para esse contrato na data informada.");
                    return;
                }
            }

            #endregion

            TabelaExcecao tabela = new TabelaExcecao();

            if (gridTabelasDeExcecao.SelectedIndex > -1) //se está editando...
            {
                tabela.ID = gridTabelasDeExcecao.DataKeys[gridTabelasDeExcecao.SelectedIndex].Value;
                tabela.Carregar();
            }

            tabela.ContratoAdmID = cboTabelaExcecaoContrato.SelectedValue;
            tabela.ProdutorID = ViewState[IDKey];
            tabela.Vigencia = data;

            if(gridTabelasDoUsuario.SelectedIndex > -1)
                tabela.TabelaComissionamentoID = gridTabelasDoUsuario.DataKeys[gridTabelasDoUsuario.SelectedIndex][1];

            tabela.Salvar();

            if (this.Itens != null)
            {
                IList<ExcecaoItem> _itens = this.Itens;
                foreach (ExcecaoItem _item in _itens)
                {
                    _item.TabelaExcecaoID = tabela.ID;
                    _item.Salvar();
                }
            }

            this.SalvarVitaliciedade(tabela.ID);

            this.FechaModoDetalhe();
            this.CarregaTabelasDeExcecao();

            base.Alerta(null, this, "__okTE", "Dados salvos com sucesso!");
        }

        void SalvarVitaliciedade(Object tabelaId)
        {
            if (tabelaId == null) { return; }
            TabelaExcecaoVitaliciedade cv = null;

            #region NORMAL

            cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal);
            if (cv == null) { cv = new TabelaExcecaoVitaliciedade(); }

            cv.Vitalicia = chkComissionamentoVitalicio.Checked;
            cv.TabelaExcecaoID = tabelaId;
            //cv.ContratoID = contratoId; //cboContrato.SelectedValue;
            cv.TipoColunaComissao = (Int32)TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal;

            if (chkComissionamentoVitalicio.Checked)
            {
                cv.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicio.Text);
                cv.Percentual = CToDecimal(txtComissionamentoVitalicioPercentual.Text);
            }
            else
            {
                cv.Percentual = 0;
                cv.ParcelaInicio = 0;
            }

            cv.Salvar();

            #endregion

            #region CARENCIA

            cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia);
            if (cv == null) { cv = new TabelaExcecaoVitaliciedade(); }
            cv.Vitalicia = chkComissionamentoVitalicioCarencia.Checked;
            cv.TabelaExcecaoID = tabelaId;
            //cv.ContratoID = contratoId;
            cv.TipoColunaComissao = (Int32)TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia;

            if (chkComissionamentoVitalicioCarencia.Checked)
            {
                cv.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioCarencia.Text);
                cv.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualCarencia.Text);
            }
            else
            {
                cv.Percentual = 0;
                cv.ParcelaInicio = 0;
            }

            cv.Salvar();

            #endregion

            #region MIGRACAO

            cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao);
            if (cv == null) { cv = new TabelaExcecaoVitaliciedade(); }

            cv.Vitalicia = chkComissionamentoVitalicioMigracao.Checked;
            cv.TabelaExcecaoID = tabelaId;
            //cv.ContratoID = contratoId;
            cv.TipoColunaComissao = (Int32)TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao;

            if (chkComissionamentoVitalicioMigracao.Checked)
            {
                cv.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioMigracao.Text);
                cv.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualMigracao.Text);
            }
            else
            {
                cv.Percentual = 0;
                cv.ParcelaInicio = 0;
            }

            cv.Salvar();

            #endregion

            #region ADM

            cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa);
            if (cv == null) { cv = new TabelaExcecaoVitaliciedade(); }

            cv.Vitalicia = chkComissionamentoVitalicioADM.Checked;
            cv.TabelaExcecaoID = tabelaId;
            //cv.ContratoID = contratoId;
            cv.TipoColunaComissao = (Int32)TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa;

            if (chkComissionamentoVitalicioADM.Checked)
            {
                cv.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioADM.Text);
                cv.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualADM.Text);
            }
            else
            {
                cv.Percentual = 0;
                cv.ParcelaInicio = 0;
            }

            cv.Salvar();

            #endregion

            #region ESPECIAL

            cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial);
            if (cv == null) { cv = new TabelaExcecaoVitaliciedade(); }
            cv.Vitalicia = chkComissionamentoVitalicioEspecial.Checked;
            cv.TabelaExcecaoID = tabelaId;
            //cv.ContratoID = contratoId;
            cv.TipoColunaComissao = (Int32)TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial;

            if (chkComissionamentoVitalicioEspecial.Checked)
            {
                cv.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioEspecial.Text);
                cv.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualEspecial.Text);
            }
            else
            {
                cv.Percentual = 0;
                cv.ParcelaInicio = 0;
            }

            cv.Salvar();

            #endregion

            #region IDADE

            cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade);
            if (cv == null) { cv = new TabelaExcecaoVitaliciedade(); }
            cv.Vitalicia = chkComissionamentoVitalicioIdade.Checked;
            cv.TabelaExcecaoID = tabelaId;
            //cv.ContratoID = contratoId; //cboContrato.SelectedValue;
            cv.TipoColunaComissao = (Int32)TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade;

            if (chkComissionamentoVitalicioIdade.Checked)
            {
                cv.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioIdade.Text);
                cv.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualIdade.Text);
            }
            else
            {
                cv.Percentual = 0;
                cv.ParcelaInicio = 0;
            }

            cv.Salvar();

            #endregion
        }

        #endregion

        #region Equipe

        void CarregaEquipes()
        {
            if (ViewState[IDKey] == null) { return; }
            gridEquipe.DataSource = SuperiorSubordinado.CarregarSuperiores(ViewState[IDKey]);
            gridEquipe.DataBind();

            if (gridEquipe.Rows.Count > 0)
                gridEquipe.Rows[0].Font.Bold = true;
        }

        protected void gridEquipe_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    if (!this.alteraProdutor) { return; }

                    Object id = gridEquipe.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                    SuperiorSubordinado obj = new SuperiorSubordinado(id);
                    obj.Remover();
                    this.CarregaEquipes();

                    if (gridEquipe.Rows.Count > 0)
                    {
                        id = gridEquipe.DataKeys[0].Value;
                        Usuario.AlteraSuperior(ViewState[IDKey], id);
                    }
                    else
                        Usuario.AlteraSuperior(ViewState[IDKey], null);
                }
                catch
                {
                    base.Alerta(null, this, "_errExcEq", "Não foi possível remover.");
                }
            }
        }

        protected void gridEquipe_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 3, "Deseja realmente excluir a linha?");
            }
        }

        protected void cmdAdicionarEquipe_Click(Object sender, EventArgs e)
        {
            #region validacoes

            if (cboSuperior.Items.Count == 0) { return; }

            if (gridEquipe.Rows.Count > 0)
            {
                if (Convert.ToString(gridEquipe.DataKeys[0][1]) == cboSuperior.SelectedValue)
                {
                    //base.Alerta(null, this, "_errS", "Esse ja é o superior deste produtor.");
                    base.Alerta(MPE, ref litAlert, "Esse ja é o superior deste produtor.", upnlAlerta);
                    return;
                }
            }

            if (Convert.ToString(ViewState[IDKey]) == cboSuperior.SelectedValue)
            {
                //base.Alerta(null, this, "_errS2", "Subordinado e superior são a mesma pessoa.");
                base.Alerta(MPE, ref litAlert, "Subordinado e superior são a mesma pessoa.", upnlAlerta);
                return;
            }

            /////////////////////////////////////
            if (txtDataVigenciaEquipe.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_errS1", "Você deve informar a data de vigência.");
                base.Alerta(MPE, ref litAlert, "Você deve informar a data de vigência.", upnlAlerta);
                txtDataVigenciaEquipe.Focus();
                return;
            }
            else
            {
                DateTime data = new DateTime();
                if (!UIHelper.TyParseToDateTime(txtDataVigenciaEquipe.Text, out data))
                {
                    //base.Alerta(null, this, "_errS1b", "A data de vigência informada está inválida.");
                    base.Alerta(MPE, ref litAlert, "A data de vigência informada está inválida.", upnlAlerta);
                    txtDataVigenciaEquipe.Focus();
                    return;
                }
            }
            /////////////////////////////////////

            #endregion

            SuperiorSubordinado obj = new SuperiorSubordinado();
            obj.Data = base.CStringToDateTime(txtDataVigenciaEquipe.Text);
            obj.SubordinadoID = ViewState[IDKey];
            obj.SuperiorID = cboSuperior.SelectedValue;
            obj.Salvar();

            Usuario.AlteraSuperior(ViewState[IDKey], cboSuperior.SelectedValue);
            this.CarregaEquipes();
            txtDataVigenciaEquipe.Text = "";
            //base.Alerta(null, this, "_ok3", "Superior relacionado ao produtor com sucesso.");
            base.Alerta(MPE, ref litAlert, "Superior relacionado ao produtor com sucesso.", upnlAlerta);
        }

        #endregion

        #region grupos de venda 

        void CarregaGruposDoProdutor()
        {
            if (ViewState[IDKey] == null) { return; }
            gridGruposDoUsuario.DataSource = UsuarioGrupoVenda.CarregarTodos(ViewState[IDKey]);
            gridGruposDoUsuario.DataBind();

            if (gridGruposDoUsuario.Rows.Count > 0)
                gridGruposDoUsuario.Rows[0].Font.Bold = true;
        }

        void CarregaGruposDeVenda()
        {
            IList<GrupoDeVenda> grupos = GrupoDeVenda.Carregar(true);
            cboGrupo.Items.Clear();
            cboGrupo.DataValueField = "ID";
            cboGrupo.DataTextField = "Descricao";
            cboGrupo.DataSource = grupos;
            cboGrupo.DataBind();

            cboTabelaGrupoVenda.Items.Clear();
            cboTabelaGrupoVenda.DataValueField = "ID";
            cboTabelaGrupoVenda.DataTextField = "Descricao";
            cboTabelaGrupoVenda.DataSource = grupos;
            cboTabelaGrupoVenda.DataBind();
        }

        protected void gridGruposDoUsuario_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    if (!this.alteraProdutor) { return; }

                    Object id = gridGruposDoUsuario.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                    UsuarioGrupoVenda obj = new UsuarioGrupoVenda(id);
                    obj.Remover();
                    this.CarregaGruposDoProdutor();
                }
                catch
                {
                    base.Alerta(MPE, ref litAlert, "Não foi possível excluir.", upnlAlerta);
                }
            }
        }

        protected void gridGruposDoUsuario_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente excluir a linha?");
            }
        }

        protected void cmdAdicionarGrupo_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (cboGrupo.Items.Count == 0) { return; }

            if (gridGruposDoUsuario.Rows.Count > 0)
            {
                if (Convert.ToString(gridGruposDoUsuario.DataKeys[0][1]) == cboGrupo.SelectedValue)
                {
                    base.Alerta(MPE, ref litAlert, "Esse ja é grupo atual deste produtor.", upnlAlerta);
                    return;
                }
            }

            ///////////////////////////////////////
            if (txtDataVigenciaGrupo.Text.Trim() == "")
            {
                base.Alerta(MPE, ref litAlert, "Você deve informar a data de vigência.", upnlAlerta);
                txtDataVigenciaGrupo.Focus();
                return;
            }
            else
            {
                DateTime data = new DateTime();
                if (!UIHelper.TyParseToDateTime(txtDataVigenciaGrupo.Text, out data))
                {
                    base.Alerta(MPE, ref litAlert, "A data de vigência informada é inválida.", upnlAlerta);
                    txtDataVigenciaGrupo.Focus();
                    return;
                }
            }

            ///////////////////////////////////////
            if (gridGruposDoUsuario.Rows.Count > 0)
            {
                IList<UsuarioGrupoVenda> grupos = UsuarioGrupoVenda.CarregarTodos(ViewState[IDKey]);
                foreach (UsuarioGrupoVenda _grupo in grupos)
                {
                    if (_grupo.Data.ToString("dd/MM/yyyy") == base.CStringToDateTime(txtDataVigenciaGrupo.Text).ToString("dd/MM/yyyy"))
                    {
                        base.Alerta(MPE, ref litAlert, "Já há um grupo com essa data de vigência.", upnlAlerta);
                        txtDataVigenciaGrupo.Focus();
                        return;
                    }
                }
            }

            #endregion

            UsuarioGrupoVenda obj = new UsuarioGrupoVenda();
            obj.UsuarioID = ViewState[IDKey];
            obj.GrupoVendaID = cboGrupo.SelectedValue;
            obj.Data = base.CStringToDateTime(txtDataVigenciaGrupo.Text);
            obj.Salvar();
            this.CarregaGruposDoProdutor();
            txtDataVigenciaGrupo.Text = "";
            base.Alerta(MPE, ref litAlert, "Grupo de venda relacionado ao produtor com sucesso.", upnlAlerta);
        }

        #endregion

        #region contato 

        void LimparCampos()
        {
            txtDDDCont.Text = "";
            txtContatoDepartamento.Text = "";
            txtEmailCont.Text = "";
            txtFoneCont.Text = "";
            txtContato.Text = "";
            txtRamalCont.Text = "";
        }

        void CarregaContatosDoProdutor()
        {
            gridContatos.DataSource = UsuarioContato.Carregar(ViewState[IDKey]);
            gridContatos.DataBind();
        }

        protected void gridContatos_OnRowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                if (!this.alteraProdutor) { return; }

                Object id = gridContatos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                UsuarioContato contato = new UsuarioContato(id);
                contato.Remover();
                this.CarregaContatosDoProdutor();
            }
            else if (e.CommandName.Equals("editar"))
            {
                Object id = gridContatos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                UsuarioContato uc = new UsuarioContato(id);
                uc.Carregar();

                txtDDDCont.Text = uc.DDD;
                txtContatoDepartamento.Text = uc.Departamento;
                txtEmailCont.Text = uc.Email;
                txtFoneCont.Text = uc.Fone;
                txtContato.Text = uc.Nome;
                txtRamalCont.Text = uc.Ramal;

                gridContatos.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                lnkAdicionarContato.Text = "alterar";
            }
        }

        protected void gridContatos_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente excluir o contato?");
            }
        }

        protected void cmdAdicionarContato_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (txtContato.Text.Trim() == "")
            {
                base.Alerta(MPE, ref litAlert, "Informe o nome do contato.", upnlAlerta);
                return;
            }

            #endregion

            UsuarioContato contato = new UsuarioContato();

            if (gridContatos.SelectedIndex > -1)
            {
                contato.ID = gridContatos.DataKeys[gridContatos.SelectedIndex].Value;
                gridContatos.SelectedIndex = -1;
            }

            contato.DDD = txtDDDCont.Text;
            contato.Departamento = txtContatoDepartamento.Text;
            contato.Email = txtEmailCont.Text;
            contato.Fone = txtFoneCont.Text;
            contato.Nome = txtContato.Text;
            contato.Ramal = txtRamalCont.Text;
            contato.UsuarioID = ViewState[IDKey];
            contato.Salvar();
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            this.CarregaContatosDoProdutor();
            this.LimparCampos();
            base.Alerta(MPE, ref litAlert, "Contato salvo com sucesso.", upnlAlerta);
        }

        #endregion
    }
}
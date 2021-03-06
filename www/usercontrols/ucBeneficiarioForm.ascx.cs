﻿namespace www.usercontrols
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class ucBeneficiarioForm : UserControlBase
    {
        #region Propriedades 

        public TextBox TXTCPF
        {
            get { return txtCPF; }
            set { txtCPF = value; }
        }

        public Literal LitP3
        {
            get { return litP3; }
            set { litP3 = value; }
        }

        public AjaxControlToolkit.TabPanel TP3
        {
            get { return p3; }
            set { p3 = value; }
        }

        public Literal LitP4
        {
            get { return litP4; }
            set { litP4 = value; }
        }

        public AjaxControlToolkit.TabPanel TP4
        {
            get { return p4; }
            set { p4 = value; }
        }

        public Object OperadoraId
        {
            get { return ViewState["_ucOpId"]; }
            set { ViewState["_ucOpId"] = value; }
        }

        public Object ContratoADMId
        {
            get { return ViewState["_ucConADMId"]; }
            set { ViewState["_ucConADMId"] = value; }
        }

        public Object PlanoId
        {
            get { return ViewState["_ucPlId"]; }
            set { ViewState["_ucPlId"] = value; }
        }

        public Object ContratoId
        {
            get { return ViewState["_ucCId"]; }
            set { ViewState["_ucCId"] = value; }
        }

        public HtmlTableRow TRParentesco
        {
            get { return trParentesco; }
            set { trParentesco= value; }
        }

        public DropDownList CBOParentesco
        {
            get { return cboParentesco; }
            set { cboParentesco = value; }
        }

        public Boolean EsconderBotaoVoltar
        {
            get
            {
                if (ViewState["_voltarVis"] == null) { return false; }
                else { return Convert.ToBoolean(ViewState["_voltarVis"]); }
            }
            set { ViewState["_voltarVis"] = value; }
        }

        public Boolean EsconderBotaoSalvar
        {
            get
            {
                if (ViewState["_voltarSa"] == null) { return false; }
                else { return Convert.ToBoolean(ViewState["_voltarSa"]); }
            }
            set { ViewState["_voltarSa"] = value; }
        }

        public Boolean FecharJanela
        {
            get
            {
                if (ViewState["_fecharJanela"] == null) { return true; }
                else { return Convert.ToBoolean(ViewState["_fecharJanela"]); }
            }
            set { ViewState["_fecharJanela"] = value; }
        }

        public Boolean CarregarDeInicio
        {
            get
            {
                if (ViewState["_carr"] == null) { return false; }
                else { return Convert.ToBoolean(ViewState["_carr"]); }
            }
            set { ViewState["_carr"] = value; }
        }

        public String CampoCPF
        {
            get { return txtCPF.Text; }
        }

        IList<Endereco> Enderecos
        {
            get { return ViewState["_end"] as IList<Endereco>; }
            set { ViewState["_end"] = value; }
        }

        #endregion

        public Delegates.GenericMessage postBackMessage;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.EsconderBotaoVoltar) { cmdVoltar.Visible = false; }
            if (this.EsconderBotaoSalvar) { cmdSalvar.Visible = false; }

            cmdAddEndereco.Attributes.Add("onClick", "return semNumeroConfirm('" + txtNumero.ClientID + "');");
            txtNumero.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");
            //txtPeso.Attributes.Add("onKeyUp", "mascara('" + txtPeso.ClientID + "')");
            //txtAltura.Attributes.Add("onKeyUp", "mascara('" + txtAltura.ClientID + "')");

            UIHelper.AuthCtrl(cmdAddEndereco, new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.PropostaBeneficiarioIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.ConsultaPropostaBeneficiarioIDKey, Perfil.Atendimento_Liberacao_Vencimento, Perfil.SupervidorIDKey });//UIHelper.AuthCtrl(cmdAddEndereco, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            UIHelper.AuthCtrl(cmdSalvar, new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.PropostaBeneficiarioIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.ConsultaPropostaBeneficiarioIDKey, Perfil.Atendimento_Liberacao_Vencimento, Perfil.SupervidorIDKey });

            txtFone1.Attributes.Add("onkeyup", "mascara_cel9(this, event)");
            txtFone1.Attributes.Add("onkeypress", "filtro_SoNumeros(event)");
            txtDDD1.Attributes.Add("onkeypress", "filtro_SoNumeros(event)");

            txtFone2.Attributes.Add("onkeyup", "mascara_cel9(this, event)");
            txtFone2.Attributes.Add("onkeypress", "filtro_SoNumeros(event)");
            txtDDD2.Attributes.Add("onkeypress", "filtro_SoNumeros(event)");

            txtCelular.Attributes.Add("onkeyup", "mascara_cel9(this, event)");
            txtCelular.Attributes.Add("onkeypress", "filtro_SoNumeros(event)");
            txtDDDCelular.Attributes.Add("onkeypress", "filtro_SoNumeros(event)");

            cmdBuscaEndereco.Visible = base.useExternalCEPEngine();
            if (!IsPostBack)
            {
                base.ExibirOpcoesDeSexo(cboSexo, false);

                if (Request["leg"] == "1")
                {
                    trMatriculaAssociativa.Visible = true;
                }

                if (base.IDKeyParameterInProcess(ViewState, "_beneficiario"))// && CarregarDeInicio)
                {
                    this.CarregaBeneficiario();
                    this.CarregaEnderecos();
                    this.checaEnriquecimento();
                }
                else
                    spanEnderecosCadastrados.Visible = false;

                if (base.CToString(Request["et"]) == "2" && base.CToString(Request["keyid"]) != "") 
                {
                    cmdPuxarEnderecosDoTitular.Visible = true;
                    cmdPuxarFonesDoTitular.Visible = true;
                }

                if (base.CToString(Request["cpf"]) != "")
                {
                    txtCPF.Text = base.CToString(Request["cpf"]);
                }
                if (base.CToString(Request["nome"]) != "")
                {
                    txtNome.Text = base.CToString(Request["nome"]);
                }

                txtNome.Focus();
            }

            //solicitacao
            if (Usuario.Autenticado.PerfilID == Perfil.ConsultaPropostaBeneficiarioIDKey)
            {
                txtNome.ReadOnly = true; txtDataNascimento.ReadOnly = true; txtRG.ReadOnly = true; txtCPF.ReadOnly = true;
                txtRgOrgao.ReadOnly = true; txtRgUF.ReadOnly = true;
                cboSexo.Enabled = false;
                txtNomeMae.ReadOnly = true;
                txtDeclaracaoNascimentoVivo.ReadOnly = true;
                txtCNS.ReadOnly = true;
                txtEmail.ReadOnly = true;
                txtDDD1.ReadOnly = true;
                txtDDD2.ReadOnly = true;
                txtDDDCelular.ReadOnly = true;

                txtRamal1.ReadOnly = true;
                txtRamal2.ReadOnly = true;
                txtCelularOperadora.ReadOnly = true;

                txtFone1.ReadOnly = true; txtFone2.ReadOnly = true; txtCelular.ReadOnly = true;
                txtEmail.ReadOnly = true;

                cmdSalvar.Visible = false;

                cmdAddEndereco.Visible = false;
                gridEnriquecimento.Enabled = false;
                gridEnriquecimento.Visible = false;

                pnlEnriquecimento.Visible = false;
                pnlEnriquecimentoValido.Visible = false;

                gridEnderecos.Columns[4].Visible = false;
            }
        }

        void checaEnriquecimento()
        {
            LC.Framework.Phantom.PersistenceManager pm = new LC.Framework.Phantom.PersistenceManager();
            pm.UseSingleCommandInstance();

            pnlEnriquecimento.Visible = false;
            pnlEnriquecimentoValido.Visible = false;

            String qry = String.Concat(
                "select beneficiario_nome,id_telMail,id_beneficiario,tipo,ddd,telefone,ramal,email,concluido,valido ",
                "   from mailing ",
                "       inner join beneficiario on beneficiario_id = id_beneficiario ",
                "   where ",
                "       id_beneficiario=", ViewState[IDKey]); //(concluido = 0 or concluido is null) and

            DataTable dt = LC.Framework.Phantom.LocatorHelper.Instance.ExecuteQuery(qry, "result", pm).Tables[0];

            pm.CloseSingleCommandInstance();
            pm.Dispose();

            if (dt.Rows.Count == 0) { return; }

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();

            DataTable dtPendentes = new DataTable();
            dtPendentes.Columns.Add("beneficiario_nome");
            dtPendentes.Columns.Add("id_telMail");
            dtPendentes.Columns.Add("id_beneficiario");
            dtPendentes.Columns.Add("tipo");
            dtPendentes.Columns.Add("ddd");
            dtPendentes.Columns.Add("telefone");
            dtPendentes.Columns.Add("ramal");
            dtPendentes.Columns.Add("email");
            dtPendentes.Columns.Add("concluido");
            dtPendentes.Columns.Add("valido");
            dtPendentes.Columns.Add("dado");

            DataTable dtAprovados = new DataTable();
            dtAprovados.Columns.Add("beneficiario_nome");
            dtAprovados.Columns.Add("id_telMail");
            dtAprovados.Columns.Add("id_beneficiario");
            dtAprovados.Columns.Add("tipo");
            dtAprovados.Columns.Add("ddd");
            dtAprovados.Columns.Add("telefone");
            dtAprovados.Columns.Add("ramal");
            dtAprovados.Columns.Add("email");
            dtAprovados.Columns.Add("concluido");
            dtAprovados.Columns.Add("valido");
            dtAprovados.Columns.Add("dado");

            String aux = "";

            foreach (DataRow row in dt.Rows)
            {
                aux = "";

                if (Convert.ToInt32(row["concluido"]) == 0)
                {
                    if (!pnlEnriquecimento.Visible) { pnlEnriquecimento.Visible = true; }
                    DataRow rowPendente = dtPendentes.NewRow();
                    rowPendente["beneficiario_nome"] = row["beneficiario_nome"];
                    rowPendente["id_telMail"] = row["id_telMail"];
                    rowPendente["id_beneficiario"] = row["id_beneficiario"];
                    rowPendente["tipo"] = row["tipo"];
                    rowPendente["ddd"] = row["ddd"];
                    rowPendente["telefone"] = row["telefone"];
                    rowPendente["ramal"] = row["ramal"];
                    rowPendente["email"] = row["email"];
                    rowPendente["concluido"] = row["concluido"];
                    rowPendente["valido"] = row["valido"];

                    if (base.CToString(rowPendente["tipo"]).ToUpper().Replace("-", "").Replace(" ", "") == "EMAIL")
                        rowPendente["dado"] = rowPendente["email"];
                    else
                    {
                        if (base.CToString(rowPendente["ddd"]).Trim() != "0" && base.CToString(rowPendente["ddd"]).Trim() != "")
                            aux = "(" + base.CToString(rowPendente["ddd"]).Trim() + ") ";

                        if (base.CToString(rowPendente["telefone"]).Trim() != "0" && base.CToString(rowPendente["telefone"]).Trim() != "")
                            aux += base.CToString(rowPendente["telefone"]);

                        if (base.CToString(rowPendente["ramal"]).Trim() != "0" && base.CToString(rowPendente["ramal"]).Trim() != "")
                            aux += " R.: " + base.CToString(rowPendente["ramal"]);

                        rowPendente["dado"] = aux;
                    }

                    dtPendentes.Rows.Add(rowPendente);
                }
                else if (Convert.ToInt32(row["valido"]) == 1)
                {
                    if (!pnlEnriquecimentoValido.Visible) { pnlEnriquecimentoValido.Visible = true; }
                    DataRow rowAprovado = dtAprovados.NewRow();
                    rowAprovado["beneficiario_nome"] = row["beneficiario_nome"];
                    rowAprovado["id_telMail"] = row["id_telMail"];
                    rowAprovado["id_beneficiario"] = row["id_beneficiario"];
                    rowAprovado["tipo"] = row["tipo"];
                    rowAprovado["ddd"] = row["ddd"];
                    rowAprovado["telefone"] = row["telefone"];
                    rowAprovado["ramal"] = row["ramal"];
                    rowAprovado["email"] = row["email"];
                    rowAprovado["concluido"] = row["concluido"];
                    rowAprovado["valido"] = row["valido"];

                    if (base.CToString(rowAprovado["tipo"]).ToUpper().Replace("-", "").Replace(" ", "") == "EMAIL")
                        rowAprovado["dado"] = rowAprovado["email"];
                    else
                    {
                        if (base.CToString(rowAprovado["ddd"]).Trim() != "0" && base.CToString(rowAprovado["ddd"]).Trim() != "")
                            aux = "(" + base.CToString(rowAprovado["ddd"]).Trim() + ") ";

                        if (base.CToString(rowAprovado["telefone"]).Trim() != "0" && base.CToString(rowAprovado["telefone"]).Trim() != "")
                            aux += base.CToString(rowAprovado["telefone"]);

                        if (base.CToString(rowAprovado["ramal"]).Trim() != "0" && base.CToString(rowAprovado["ramal"]).Trim() != "")
                            aux += " R.: " + base.CToString(rowAprovado["ramal"]);

                        rowAprovado["dado"] = aux;
                    }

                    dtAprovados.Rows.Add(rowAprovado);

                    #region comentado 
                    //sb.Append("<td>");
                    //sb.Append("<table cellpadding=\"2\" border=\"0\" style=\"border: solid 1px #507CD1;background-color:#EFF3FB\">");
                    //sb.Append("<tr><td>Tipo</td><td>"); sb.Append(row["tipo"]); sb.Append("</tr>");
                    //sb.Append("<tr><td>DDD</td><td>"); sb.Append(row["ddd"]); sb.Append("</tr>");
                    //sb.Append("<tr><td>Número</td><td>"); sb.Append(row["telefone"]); sb.Append("</tr>");
                    //sb.Append("<tr><td>Ramal</td><td>"); sb.Append(row["ramal"]); sb.Append("</tr>");
                    //sb.Append("<tr><td>E-mail</td><td>"); sb.Append(row["email"]); sb.Append("</tr>");
                    //sb.Append("</table>");
                    //sb.Append("</td>");
                    #endregion
                }
            }

            //litEnriquecimentoValido.Text = sb.ToString();

            gridEnriquecimento.DataSource = dtPendentes;
            gridEnriquecimento.DataBind();

            dlEnriquecimento.DataSource = dtAprovados;
            dlEnriquecimento.DataBind();

            dt.Dispose();
            dtPendentes.Dispose();
        }

        public void CarregaBeneficiario(Object id)
        {
            Beneficiario beneficiario = new Beneficiario();
            beneficiario.ID = id;
            beneficiario.Carregar();

            if (beneficiario.ID != null)
            {
                txtCelular.Text = PegaTelefone(beneficiario.Celular);
                txtCelularOperadora.Text = beneficiario.CelularOperadora;
                txtCPF.Text = beneficiario.CPF;

                if (beneficiario.DataNascimento != DateTime.MinValue)
                    txtDataNascimento.Text = beneficiario.DataNascimento.ToString("dd/MM/yyyy");

                //if (beneficiario.DataCasamento != DateTime.MinValue)
                //    txtDataCasamento.Text = beneficiario.DataCasamento.ToString("dd/MM/yyyy");

                txtDDD1.Text = PegaDDD(beneficiario.Telefone);
                txtDDD2.Text = PegaDDD(beneficiario.Telefone2);
                txtDDDCelular.Text = PegaDDD(beneficiario.Celular);
                txtEmail.Text = beneficiario.Email;
                txtFone1.Text = PegaTelefone(beneficiario.Telefone);
                txtFone2.Text = PegaTelefone(beneficiario.Telefone2);
                txtNome.Text = beneficiario.Nome;
                txtNomeMae.Text = beneficiario.NomeMae;
                txtRamal1.Text = beneficiario.Ramal;
                txtRamal2.Text = beneficiario.Ramal2;
                txtRG.Text = beneficiario.RG;
                txtRgOrgao.Text = beneficiario.RgOrgaoExp;
                txtRgUF.Text = beneficiario.RgUF;

                txtDeclaracaoNascimentoVivo.Text = beneficiario.DeclaracaoNascimentoVivo;
                txtCNS.Text = beneficiario.CNS;

                if (trMatriculaAssociativa.Visible)
                {
                    txtMatriculaAssociativa.Text = beneficiario.MatriculaAssociativa;
                }

                //txtPeso.Text = beneficiario.Peso.ToString("N2");
                //txtAltura.Text = beneficiario.Altura.ToString("N2");

                if (beneficiario.Sexo != null)
                    cboSexo.SelectedValue = beneficiario.Sexo;
                ViewState[IDKey] = id;
                this.CarregaEnderecos();
            }
            else
                ViewState[IDKey] = null;
        }

        void CarregaBeneficiario()
        {
            CarregaBeneficiario(ViewState[IDKey]);
        }

        void CarregaEnderecos()
        {
            IList<Endereco> enderecos = Endereco.
                CarregarPorDono(ViewState[IDKey], Endereco.TipoDono.Beneficiario);
            this.Enderecos = enderecos;

            gridEnderecos.DataSource = enderecos;
            gridEnderecos.DataBind();

            spanEnderecosCadastrados.Visible = enderecos != null && enderecos.Count > 0;
        }

        protected void cmdBuscaEndereco_Click(Object sender, EventArgs e)
        {
            base.PegaEndereco(this.Page, txtCEP, txtLogradouro, txtBairro, txtCidade, txtUF, txtNumero);
        }

        protected void cmdPuxarEnderecosDoTitular_Click(Object sender, EventArgs e)
        {
            if (Cache[base.CToString(Request["keyid"])] == null) { return; }

            Beneficiario benef = new Beneficiario();
            //benef.ID = Session["idBenefTit"];
            benef.ID = Cache[base.CToString(Request["keyid"])];
            benef.Carregar();

            if (benef.ID == null)
            {
                base.Alerta(null, this.Page, "errEndTit", "Não foi possível encontrar o titular.");
                return;
            }

            IList<Endereco> enderecos = Endereco.CarregarPorDono(benef.ID, Endereco.TipoDono.Beneficiario);

            if (enderecos == null) { return; }

            foreach (Endereco end in enderecos)
            {
                end.ID = null;
                end.DonoId = null;
            }

            this.Enderecos = enderecos;

            gridEnderecos.DataSource = enderecos;
            gridEnderecos.DataBind();
        }

        protected void cmdPuxarFonesDoTitular_Click(Object sender, EventArgs e)
        {
            //base.CToString(Session["idBenefTit"]) != ""

            if (Cache[base.CToString(Request["keyid"])] == null) { return; }

            Beneficiario benef = new Beneficiario();
            benef.ID = Cache[base.CToString(Request["keyid"])];
            benef.Carregar();

            if (benef.ID == null)
            {
                base.Alerta(null, this.Page, "errEndTit", "Não foi possível encontrar o titular.");
                return;
            }

            txtRamal1.Text = benef.Ramal;
            txtRamal2.Text = benef.Ramal2;
            txtDDD1.Text = PegaDDD(benef.Telefone);
            txtDDD2.Text = PegaDDD(benef.Telefone2);
            txtDDDCelular.Text = PegaDDD(benef.Celular);
            txtEmail.Text = benef.Email;
            txtFone1.Text = PegaTelefone(benef.Telefone);
            txtFone2.Text = PegaTelefone(benef.Telefone2);
            txtCelular.Text = PegaTelefone(benef.Celular);
        }

        public Boolean Salvar(Boolean limpaCampos, out Object beneficiarioId)
        {
            Boolean result = this.Salvar(limpaCampos);

            if (result)
                beneficiarioId = ViewState[IDKey];
            else
                beneficiarioId = null;

            return result;
        }

        public Boolean Salvar(Boolean limpaCampos)
        {
            #region validacoes

            if (txtNome.Text.Trim() == "")
            {
                base.Alerta(null, this.Page, "_err0", "Você deve informar o nome.");
                tab.ActiveTabIndex = 0;
                txtNome.Focus();
                if (postBackMessage != null) { postBackMessage(); }
                return false;
            }

            if (trMatriculaAssociativa.Visible && txtMatriculaAssociativa.Text.Trim() == "")
            {
                base.Alerta(null, this.Page, "_err0", "Você deve informar a matrícula associativa.");
                tab.ActiveTabIndex = 0;
                txtMatriculaAssociativa.Focus();
                if (postBackMessage != null) { postBackMessage(); }
                return false;
            }



            //String[] spt = txtNome.Text.Split(' ');
            //foreach (String _itemspt in spt)
            //{
            //    if (_itemspt.Length == 1 || _itemspt.IndexOf('.') > -1)
            //    {
            //        base.Alerta(null, this.Page, "_err0", "Não é permitido abreviar o nome.");
            //        tab.ActiveTabIndex = 0;
            //        txtNome.Focus();
            //        if (postBackMessage != null) { postBackMessage(); }
            //        return false;
            //    }
            //}

            DateTime data = new DateTime();
            if (!UIHelper.TyParseToDateTime(txtDataNascimento.Text, out data))
                data = DateTime.MinValue;

            if (data > DateTime.Now)
            {
                base.Alerta(null, this.Page, "_err1b", "A data de nascimento informada é inválida.");
                tab.ActiveTabIndex = 0;
                txtDataNascimento.Focus();
                if (postBackMessage != null) { postBackMessage(); }
                return false;
            }

            //if (txtDataNascimento.Text.Trim() == "")
            //{
            //    base.Alerta(null, this.Page, "_err1", "Você deve informar a data de nascimento.");
            //    tab.ActiveTabIndex = 0;
            //    txtDataNascimento.Focus();
            //    if (postBackMessage != null) { postBackMessage(); }
            //    return false;
            //}
            //else
            //{
            //    if (!UIHelper.TyParseToDateTime(txtDataNascimento.Text, out data))
            //    {
            //        base.Alerta(null, this.Page, "_err1b", "A data de nascimento informada é inválida.");
            //        tab.ActiveTabIndex = 0;
            //        txtDataNascimento.Focus();
            //        if (postBackMessage != null) { postBackMessage(); }
            //        return false;
            //    }
            //    else if (data > DateTime.Now)
            //    {
            //        base.Alerta(null, this.Page, "_err1b", "A data de nascimento informada é inválida.");
            //        tab.ActiveTabIndex = 0;
            //        txtDataNascimento.Focus();
            //        if (postBackMessage != null) { postBackMessage(); }
            //        return false;
            //    }
            //}

            //if (txtCPF.Text.Trim() == "")
            //{
            //    base.Alerta(null, this.Page, "_err2", "Você deve informar o CPF.");
            //    tab.ActiveTabIndex = 0;
            //    txtCPF.Focus();
            //    if (postBackMessage != null) { postBackMessage(); }
            //    return false;
            //}
            //Boolean result = Beneficiario.ChecaCpf(ViewState[IDKey], txtCPF.Text);
            //if (!result)
            //{
            //    base.Alerta(null, this.Page, "_err2b", "O CPF informado é inválido.");
            //    tab.ActiveTabIndex = 0;
            //    txtCPF.Focus();
            //    if (postBackMessage != null) { postBackMessage(); }
            //    return false;
            //}


            //int idade = Beneficiario.CalculaIdade(data, DateTime.Now);
            //if (txtCPF.Text.Trim() == "" && idade > 17)
            //{
            //    base.Alerta(null, this.Page, "_err2", "Você deve informar o CPF.");
            //    tab.ActiveTabIndex = 0;
            //    txtCPF.Focus();
            //    if (postBackMessage != null) { postBackMessage(); }
            //    return false;
            //}
            //else if (txtCPF.Text.Trim() != "")
            //{
            //    Boolean result = Beneficiario.ChecaCpf(ViewState[IDKey], txtCPF.Text);
            //    if (!result)
            //    {
            //        base.Alerta(null, this.Page, "_err2b", "O CPF informado é inválido.");
            //        tab.ActiveTabIndex = 0;
            //        txtCPF.Focus();
            //        if (postBackMessage != null) { postBackMessage(); }
            //        return false;
            //    }
            //}

            //if (cboEstadoCivil.SelectedItem.Text.IndexOf("CASAD") > -1)
            //{
            //    if (txtDataCasamento.Text.Trim() == "")
            //    {
            //        base.Alerta(null, this.Page, "_err5", "Você deve informar a data de casamento.");
            //        tab.ActiveTabIndex = 0;
            //        txtDataCasamento.Focus();
            //        if (postBackMessage != null) { postBackMessage(); }
            //        return false;
            //    }

            //    DateTime dataCasamento = new DateTime();
            //    if (!UIHelper.TyParseToDateTime(txtDataCasamento.Text, out dataCasamento))
            //    {
            //        base.Alerta(null, this.Page, "_err5b", "A data de casamento informada é inválida.");
            //        tab.ActiveTabIndex = 0;
            //        txtDataCasamento.Focus();
            //        if (postBackMessage != null) { postBackMessage(); }
            //        return false;
            //    }
            //}
            //else
            //    txtDataCasamento.Text = "";

            //if (txtNomeMae.Text.Trim() == "")
            //{
            //    base.Alerta(null, this.Page, "_err3", "Você deve informar o nome da mãe.");
            //    tab.ActiveTabIndex = 0;
            //    txtNomeMae.Focus();
            //    if (postBackMessage != null) { postBackMessage(); }
            //    return false;
            //}

            //if ((txtDDD1.Text.Trim() == "" || txtFone1.Text.Trim() == "") &&
            //    (txtDDD2.Text.Trim() == "" || txtFone2.Text.Trim() == "") &&
            //    (txtDDDCelular.Text.Trim() == "" || txtCelular.Text.Trim() == ""))
            //{
            //    base.Alerta(null, this.Page, "_err4", "Você deve informar ao menos um número de telefone\\nO campo DDD é obrigatório.");
            //    tab.ActiveTabIndex = 0;
            //    if (postBackMessage != null) { postBackMessage(); }
            //    return false;
            //}

            if (this.Enderecos == null || this.Enderecos.Count == 0)
            {
                base.Alerta(null, this.Page, "_errEnd", "Você deve informar ao menos um endereço.");
                tab.ActiveTabIndex = 1;
                if (postBackMessage != null) { postBackMessage(); }
                return false;
            }

            String msg = "";

            if (txtEmail.Text.Trim() != "")
            {
                if (!base.ValidaEmail(txtEmail.Text, out msg))
                {
                    base.Alerta(null, this.Page, "_errMail", msg);
                    txtEmail.Focus();
                    if (postBackMessage != null) { postBackMessage(); }
                    return false;
                }
            }
            #endregion

            cmdSalvar_Click(null, null);

            if (limpaCampos)
                LimparCampos();

            return true;
        }

        public void LimparCampos()
        {
            ViewState[IDKey] = null;
            txtBairro.Text = "";
            txtCelular.Text = "";
            txtCEP.Text = "";
            txtCidade.Text = "";
            txtComplemento.Text = "";
            txtCPF.Text = "";
            txtDataNascimento.Text = "";
            txtDDD1.Text = "";
            txtDDD2.Text = "";
            txtDDDCelular.Text = "";
            txtEmail.Text = "";
            txtFone1.Text = "";
            txtFone2.Text = "";
            txtLogradouro.Text = "";
            txtNome.Text = "";
            txtNomeMae.Text = "";
            txtNumero.Text = "";
            txtRamal1.Text = "";
            txtRamal2.Text = "";
            txtRG.Text = "";
            txtRgOrgao.Text = "";
            txtRgUF.Text = "";
            txtUF.Text = "";
            gridEnderecos.DataSource = null;
            gridEnderecos.DataBind();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            string param = Request["leg"];

            if(!string.IsNullOrEmpty(param))
                Response.Redirect("beneficiarios.aspx?leg=1");
            else
                Response.Redirect("beneficiarios.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (txtNome.Text.Trim() == "")
            {
                base.Alerta(null, this.Page, "_err0", "Você deve informar o nome.");
                tab.ActiveTabIndex = 0;
                txtNome.Focus();
                if (postBackMessage != null) { postBackMessage(); }
                return;
            }

            if (trMatriculaAssociativa.Visible && txtMatriculaAssociativa.Text.Trim() == "")
            {
                base.Alerta(null, this.Page, "_err0", "Você deve informar a matrícula associativa.");
                tab.ActiveTabIndex = 0;
                txtMatriculaAssociativa.Focus();
                if (postBackMessage != null) { postBackMessage(); }
                return;
            }

            //String[] spt = txtNome.Text.Split(' ');
            //foreach (String _itemspt in spt)
            //{
            //    if (_itemspt.Length == 1 || _itemspt.IndexOf('.') > -1)
            //    {
            //        base.Alerta(null, this.Page, "_err0", "Não é permitido abreviar o nome.");
            //        tab.ActiveTabIndex = 0;
            //        txtNome.Focus();
            //        if (postBackMessage != null) { postBackMessage(); }
            //        return;
            //    }
            //}

            DateTime data = new DateTime();
            if (!UIHelper.TyParseToDateTime(txtDataNascimento.Text, out data))
                data = DateTime.MinValue;

            if (data > DateTime.Now)
            {
                base.Alerta(null, this.Page, "_err1b", "A data de nascimento informada é inválida.");
                tab.ActiveTabIndex = 0;
                txtDataNascimento.Focus();
                if (postBackMessage != null) { postBackMessage(); }
                return;
            }
            //DateTime data = new DateTime();
            //if (txtDataNascimento.Text.Trim() == "")
            //{
            //    base.Alerta(null, this.Page, "_err1", "Você deve informar a data de nascimento.");
            //    tab.ActiveTabIndex = 0;
            //    txtDataNascimento.Focus();
            //    if (postBackMessage != null) { postBackMessage(); }
            //    return;
            //}
            //else
            //{
            //    if (!UIHelper.TyParseToDateTime(txtDataNascimento.Text, out data))
            //    {
            //        base.Alerta(null, this.Page, "_err1b", "A data de nascimento informada está inválida.");
            //        tab.ActiveTabIndex = 0;
            //        txtDataNascimento.Focus();
            //        if (postBackMessage != null) { postBackMessage(); }
            //        return;
            //    }
            //    else if (data > DateTime.Now)
            //    {
            //        base.Alerta(null, this.Page, "_err1b", "A data de nascimento informada está inválida.");
            //        tab.ActiveTabIndex = 0;
            //        txtDataNascimento.Focus();
            //        if (postBackMessage != null) { postBackMessage(); }
            //        return;
            //    }
            //}

            //int idade = Beneficiario.CalculaIdade(data, DateTime.Now);
            //if (txtCPF.Text.Trim() == "" && idade > 17)
            //{
            //    base.Alerta(null, this.Page, "_err2", "Você deve informar o CPF.");
            //    tab.ActiveTabIndex = 0;
            //    txtCPF.Focus();
            //    if (postBackMessage != null) { postBackMessage(); }
            //    return;
            //}
            //else if(txtCPF.Text.Trim() != "")
            //{
            //    Boolean result = Beneficiario.ChecaCpf(ViewState[IDKey], txtCPF.Text);
            //    if (!result)
            //    {
            //        base.Alerta(null, this.Page, "_err2b", "O CPF informado é inválido.");
            //        tab.ActiveTabIndex = 0;
            //        txtCPF.Focus();
            //        if (postBackMessage != null) { postBackMessage(); }
            //        return;
            //    }
            //}

            //result = Beneficiario.ChecaCpfEmUso(ViewState[IDKey], txtCPF.Text);
            //if (!result)
            //{
            //    base.Alerta(null, this.Page, "_err2c", "O CPF informado já está em uso.");
            //    tab.ActiveTabIndex = 0;
            //    txtCPF.Focus();
            //    if (postBackMessage != null) { postBackMessage(); }
            //    return;
            //}

            //if (cboEstadoCivil.SelectedItem.Text.IndexOf("CASAD") > -1)
            //{
            //    if (txtDataCasamento.Text.Trim() == "")
            //    {
            //        base.Alerta(null, this.Page, "_err5", "Você deve informar a data de casamento.");
            //        tab.ActiveTabIndex = 0;
            //        txtDataCasamento.Focus();
            //        if (postBackMessage != null) { postBackMessage(); }
            //        return;
            //    }

            //    DateTime dataCasamento = new DateTime();
            //    if (!UIHelper.TyParseToDateTime(txtDataCasamento.Text, out dataCasamento))
            //    {
            //        base.Alerta(null, this.Page, "_err5b", "A data de casamento informada é inválida.");
            //        tab.ActiveTabIndex = 0;
            //        txtDataCasamento.Focus();
            //        if (postBackMessage != null) { postBackMessage(); }
            //        return;
            //    }
            //}
            //else
            //    txtDataCasamento.Text = "";

            //if (txtNomeMae.Text.Trim() == "")
            //{
            //    base.Alerta(null, this.Page, "_err3", "Você deve informar o nome da mãe.");
            //    tab.ActiveTabIndex = 0;
            //    txtNomeMae.Focus();
            //    if (postBackMessage != null) { postBackMessage(); }
            //    return;
            //}

            //if ((txtDDD1.Text.Trim() == "" || txtFone1.Text.Trim() == "") &&
            //    (txtDDD2.Text.Trim() == "" || txtFone2.Text.Trim() == "") &&
            //    (txtDDDCelular.Text.Trim() == "" || txtCelular.Text.Trim() == ""))
            //{
            //    base.Alerta(null, this.Page, "_err4", "Você deve informar ao menos um número de telefone\\nO campo DDD é obrigatório.");
            //    tab.ActiveTabIndex = 0;
            //    if (postBackMessage != null) { postBackMessage(); }
            //    return;
            //}
            
            if (this.Enderecos == null || this.Enderecos.Count == 0)
            {
                base.Alerta(null, this.Page, "_errEnd", "Você deve informar ao menos um endereço.");
                tab.ActiveTabIndex = 1;
                if (postBackMessage != null) { postBackMessage(); }
                return;
            }

            String msg = "";
            if (txtEmail.Text.Trim() != "")
            {
                if (!base.ValidaEmail(txtEmail.Text, out msg))
                {
                    base.Alerta(null, this.Page, "_errMail", msg);
                    tab.ActiveTabIndex = 0;
                    txtEmail.Focus();
                    if (postBackMessage != null) { postBackMessage(); }
                    return;
                }
            }

            if (cmdSalvar.Text == "Salvar" && ViewState[IDKey] == null)
            {
                String cpf = txtCPF.Text;
                IList<Beneficiario> lista = Beneficiario.CarregarPorParametro(base.CStringToDateTime(txtDataNascimento.Text), txtNome.Text, txtCPF.Text);

                if (lista != null && lista.Count > 0)
                {
                    //cmdSalvar.Text = "Salvar mesmo assim";
                    //trUsar.Visible = true;
                    base.Alerta(null, this.Page, "_errMail", "Beneficiário já cadastrado.");
                    return;
                }
            }
            #endregion

            Beneficiario beneficiario = new Beneficiario();
            beneficiario.ID = ViewState[IDKey];

            if (beneficiario.ID != null)
            {
                Beneficiario prova = new Beneficiario(beneficiario.ID);
                prova.Carregar();
                beneficiario.ImportID = prova.ImportID;
                beneficiario.MatriculaAssociativa = prova.MatriculaAssociativa;
                beneficiario.MatriculaFuncional = prova.MatriculaFuncional;
            }

            if (trMatriculaAssociativa.Visible)
            {
                beneficiario.MatriculaAssociativa = txtMatriculaAssociativa.Text;
            }

            beneficiario.DeclaracaoNascimentoVivo = txtDeclaracaoNascimentoVivo.Text;
            beneficiario.CNS = txtCNS.Text;

            beneficiario.Celular = "(" + txtDDDCelular.Text + ") " + txtCelular.Text;
            beneficiario.CelularOperadora = txtCelularOperadora.Text;
            beneficiario.CPF = txtCPF.Text;
            beneficiario.DataNascimento = base.CStringToDateTime(txtDataNascimento.Text);
            //beneficiario.DataCasamento  = base.CStringToDateTime(txtDataCasamento.Text);
            beneficiario.Email = txtEmail.Text;
            //beneficiario.EstadoCivilID = cboEstadoCivil.SelectedValue;
            beneficiario.Nome = txtNome.Text;
            beneficiario.NomeMae = txtNomeMae.Text;
            beneficiario.RG = txtRG.Text;
            beneficiario.RgOrgaoExp = txtRgOrgao.Text;
            beneficiario.RgUF = txtRgUF.Text;
            beneficiario.Telefone = "(" + txtDDD1.Text + ") " + txtFone1.Text;
            beneficiario.Telefone2 = "(" + txtDDD2.Text + ") " + txtFone2.Text;
            beneficiario.Ramal = txtRamal1.Text;
            beneficiario.Ramal2 = txtRamal2.Text;
            beneficiario.Sexo = cboSexo.SelectedValue;
            //beneficiario.Peso = base.CToDecimal(txtPeso.Text);
            //beneficiario.Altura = base.CToDecimal(txtAltura.Text);

            beneficiario.Salvar();

            ViewState[IDKey] = beneficiario.ID;

            // Adicionais e Fichas
            if (ViewState["fic_"] != null)
            {
                IList<ItemDeclaracaoSaudeINSTANCIA> fichas = (IList<ItemDeclaracaoSaudeINSTANCIA>)ViewState["fic_"];
                foreach (ItemDeclaracaoSaudeINSTANCIA ficha in fichas)
                {
                    ficha.BeneficiarioID = beneficiario.ID;
                    ficha.Salvar();
                }
            }

            List<AdicionalBeneficiario> adicionaisContratados = null;
            if (ViewState["adben_"] != null)
            {
                if (adicionaisContratados == null) { adicionaisContratados = new List<AdicionalBeneficiario>(); }
                foreach (AdicionalBeneficiario _ab in ((List<AdicionalBeneficiario>)ViewState["adben_"]))
                {
                    if (_ab.BeneficiarioID != null) { adicionaisContratados.Add(_ab); }
                }
            }
            if (adicionaisContratados != null && adicionaisContratados.Count > 0)
            {
                foreach (AdicionalBeneficiario ad in adicionaisContratados)
                {
                    ad.PropostaID = ContratoId;
                    ad.Salvar();
                }
            }
            //\ Adicionais e Fichas

            IList<Endereco> enderecos = this.Enderecos;
            foreach (Endereco endereco in enderecos)
            {
                endereco.DonoId = beneficiario.ID;
                endereco.DonoTipo = (Int32)Endereco.TipoDono.Beneficiario;
                endereco.Salvar();
            }
            this.Enderecos = enderecos;
            this.SetaDataAlteracaoContrato(beneficiario.ID);

            if (!EsconderBotaoVoltar) //não foi chamado de poupup
            {
                base.Alerta(null, this.Page, "_ok", "Dados salvos com sucesso.");
            }
            else if (!FecharJanela)  // não precisa fechar a janela pois está dentro de um panel
            {
                base.Alerta(null, this.Page, "_ok", "Dados salvos com sucesso.");
            }
            else //if(EsconderBotaoSalvar || FecharJanela) //foi chamado de um poupup, a partir da tela de contrato. Então fecha e preenche os campos.
            {
                if (base.CToString(Request["et"]) == "1") //preenche os campos do titular
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "preenche", "<script>opener.window.document.getElementById('ctl00_cphContent_tab_p2_txtCPF').value='" + txtCPF.Text + "';opener.window.document.getElementById('ctl00_cphContent_tab_p2_txtDataNascimento').value='" + txtDataNascimento.Text + "';opener.window.document.getElementById('ctl00_cphContent_tab_p2_cmdCarregaBeneficiarioPorCPF').click();</script>", false);
                }
                else if (base.CToString(Request["et"]) == "2") //preenche os campos do dependente
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "preenche", "<script>opener.window.document.getElementById('ctl00_cphContent_tab_p4_txtCPFDependente').value='" + txtCPF.Text + "';opener.window.document.getElementById('ctl00_cphContent_tab_p4_txtDataNascimentoDependente').value='" + txtDataNascimento.Text + "';opener.window.document.getElementById('ctl00_cphContent_tab_p4_cmdCarregaBeneficiarioDependentePorCPF').click();</script>", false);
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "close", "<script>javascript:window.close();</script>", false);
            }

            trUsar.Visible = false;
            cmdSalvar.Text = "Salvar";
            if (postBackMessage != null) { postBackMessage(); }
        }

        protected void lnkUsar_Click(Object sender, EventArgs e)
        {
            trUsar.Visible = false;
            cmdSalvar.Text = "Salvar";

            IList<Beneficiario> lista = Beneficiario.CarregarPorParametro(base.CStringToDateTime(txtDataNascimento.Text), txtNome.Text, txtCPF.Text);

            ViewState[IDKey] = lista[0].ID;
            this.CarregaBeneficiario(lista[0].ID);
        }

        protected void cmdAddEndereco_Click(Object sender, EventArgs e)
        {
            if (txtBairro.Text.Trim() == "" || txtCEP.Text.Trim() == "" ||
                txtCidade.Text.Trim() == "" || txtLogradouro.Text.Trim() == "" ||
                txtUF.Text.Trim() == "") //txtNumero.Text.Trim() == "" || 
            {
                base.Alerta(null, this.Page, "_err5", "Todos os campos são obrigatórios.");
                return;
            }

            if (!UIHelper.ValidaUF(txtUF.Text))
            {
                base.Alerta(null, this.Page, "_errUF", "Unidade Federativa inválida.");
                txtUF.Focus();
                return;
            }

            Endereco endereco = null;

            if (txtNumero.Text.Trim() == "") { txtNumero.Text = "s/n"; }

            IList<Endereco> enderecos = this.Enderecos;
            if (enderecos == null) { enderecos = new List<Endereco>(); }

            if (gridEnderecos.SelectedIndex == -1)
                endereco = new Endereco();
            else
                endereco = enderecos[gridEnderecos.SelectedIndex];

            endereco.Bairro = txtBairro.Text;
            endereco.CEP = txtCEP.Text;
            endereco.Cidade = txtCidade.Text;
            endereco.Complemento = txtComplemento.Text;
            endereco.DonoTipo = Convert.ToInt32(Endereco.TipoDono.Beneficiario);
            endereco.Logradouro = txtLogradouro.Text;
            endereco.Numero = txtNumero.Text;
            endereco.Tipo = Convert.ToInt32(cboTipoEndereco.SelectedValue);
            endereco.UF = txtUF.Text;

            if (endereco.ID != null && ViewState[IDKey] != null) 
            {
                endereco.DonoId = ViewState[IDKey];
                endereco.Salvar();
            }

            if (gridEnderecos.SelectedIndex == -1)
                enderecos.Add(endereco);
            else
                enderecos[gridEnderecos.SelectedIndex] = endereco;

            this.Enderecos = enderecos;

            gridEnderecos.DataSource = enderecos;
            gridEnderecos.DataBind();
            spanEnderecosCadastrados.Visible = true;

            txtCEP.Text = "";
            txtLogradouro.Text = "";
            txtNumero.Text = "";
            txtComplemento.Text = "";
            txtBairro.Text = "";
            txtCidade.Text = "";
            txtUF.Text = "";
            cboTipoEndereco.SelectedIndex = 0;

            gridEnderecos.SelectedIndex = -1;
            cmdAddEndereco.Text = "Adicionar";

            if (postBackMessage != null) { postBackMessage(); }
        }

        protected void gridEnderecos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("alterar"))
            {
                int indice = Convert.ToInt32(e.CommandArgument);

                gridEnderecos.SelectedIndex = indice;

                Endereco endereco = this.Enderecos[indice];
                txtBairro.Text = endereco.Bairro;
                txtCEP.Text = endereco.CEP;
                txtCidade.Text = endereco.Cidade;
                txtComplemento.Text = endereco.Complemento;
                txtLogradouro.Text = endereco.Logradouro;
                txtNumero.Text = endereco.Numero;
                txtUF.Text = endereco.UF.ToUpper();
                cboTipoEndereco.SelectedValue = Convert.ToInt32(endereco.Tipo).ToString();

                cmdAddEndereco.Text = "Alterar";

                if (postBackMessage != null) { postBackMessage(); }
            }
            else if (e.CommandName.Equals("excluir"))
            {
                gridEnderecos.SelectedIndex = -1;
                Object id = gridEnderecos.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                if (id == null)
                {
                    IList<Endereco> lista = this.Enderecos;
                    lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                    this.Enderecos = lista;
                    gridEnderecos.DataSource = lista;
                    gridEnderecos.DataBind();
                }
                else
                {
                    Endereco endereco = new Endereco();
                    endereco.ID = id;
                    try
                    {
                        endereco.Remover();
                        this.CarregaEnderecos();
                    }
                    catch
                    {
                        base.Alerta(null, this.Page, "errExc", "Não foi possível excluir o endereço.\\nVerifique se ele está sendo utilizado em alguma proposta.");
                        return;
                    }
                }

                if (postBackMessage != null) { postBackMessage(); }
            }
        }

        protected void gridEnderecos_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UIHelper.AuthCtrl(e.Row.Cells[4].Controls[0], new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.PropostaBeneficiarioIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.ConsultaPropostaBeneficiarioIDKey, Perfil.Atendimento_Liberacao_Vencimento, Perfil.SupervidorIDKey }); //UIHelper.AuthCtrl(e.Row.Cells[4].Controls[0], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente excluir o endereço\\n\\n" + e.Row.Cells[1].Text.Replace("'", "`") + " ?\\n\\nEssa operação não poderá ser desfeita.");

                if (e.Row.Cells[2].Text == "0")
                    e.Row.Cells[2].Text = "RESIDENCIAL";
                else
                    e.Row.Cells[2].Text = "COMERCIAL";
            }
        }

        #region FichaDeSaude 

        public void CarregaFichaDeSaude()
        {
            ViewState["fic_"] = null;
            if (ViewState["fic_"] == null)
            {
                IList<ItemDeclaracaoSaudeINSTANCIA> lista = null;

                if (ViewState[IDKey] != null)
                {
                    lista = ItemDeclaracaoSaudeINSTANCIA.Carregar(ViewState[IDKey], OperadoraId);

                    if (lista != null)
                    {
                        foreach (ItemDeclaracaoSaudeINSTANCIA item in lista)
                            item.BeneficiarioID = ViewState[IDKey];
                    }
                }
                else
                {
                    lista = ItemDeclaracaoSaudeINSTANCIA.Carregar(OperadoraId);
                }

                dlFicha.DataSource = lista;
                ViewState["fic_"] = lista;
            }
            else
            {
                dlFicha.DataSource = ViewState["fic_"];
            }

            dlFicha.DataBind();
        }

        protected void checkboxSkin_Changed2(Object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            DataListItem grow = (DataListItem)check.NamingContainer;
            ((HtmlTableRow)grow.FindControl("tr1Ficha")).Visible = check.Checked;
            ((HtmlTableRow)grow.FindControl("tr2Ficha")).Visible = check.Checked;
            ((HtmlTableRow)grow.FindControl("tr3Ficha")).Visible = check.Checked;
        }

        protected void dlFicha_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            if (e.CommandName.Equals("salvar"))
            {
                Int32 index = Convert.ToInt32(e.CommandArgument);
                Object id = dlFicha.DataKeys[e.Item.ItemIndex];

                IList<ItemDeclaracaoSaudeINSTANCIA> itens =
                    (IList<ItemDeclaracaoSaudeINSTANCIA>)ViewState["fic_"];

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
                    item.BeneficiarioID = ViewState[IDKey];
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
                        //base.Alerta(null, this, "_itemFicErr1", "Informe uma descrição.");
                        ((Literal)e.Item.FindControl("litFichaAviso")).Text = "<br><font color='red' face='1'>Informe uma descrição.</font>";
                        ((TextBox)e.Item.FindControl("txtFichaSaudeDescricao")).Focus();
                        return;
                    }

                    item.Data = dte;
                    item.Descricao = ((TextBox)e.Item.FindControl("txtFichaSaudeDescricao")).Text;

                    if (i == index)
                        item.Sim = true;

                    //item.Salvar();

                    if (adiciona)
                        itens.Add(item);
                    
                    ((Literal)e.Item.FindControl("litFichaAviso")).Text = "<br><font color='blue' face='1'>Informação salva.</font>";
                }

                ViewState["fic_"] = itens;
            }
        }

        protected void dlFicha_ItemDataBound(Object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (((CheckBox)e.Item.FindControl("chkFSim")).Checked)
                {
                    ((HtmlTableRow)e.Item.FindControl("tr1Ficha")).Visible = true;
                    ((HtmlTableRow)e.Item.FindControl("tr2Ficha")).Visible = true;
                    ((HtmlTableRow)e.Item.FindControl("tr3Ficha")).Visible = true;
                }
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

        #endregion

        #region Adicionais

        public void CarregaAdicionais()
        {
            ViewState["adben_"] = null;
            if (ViewState["adben_"] == null)
            {
                if (Convert.ToInt32(PlanoId) > 0)
                {
                    IList<AdicionalBeneficiario> lista = null;

                    if (ViewState[IDKey] == null)
                    {
                        lista = AdicionalBeneficiario.
                            Carregar(ContratoADMId, PlanoId, ContratoId);
                    }
                    else
                    {
                        lista = AdicionalBeneficiario.
                            Carregar(ContratoADMId, PlanoId, ContratoId, ViewState[IDKey]);
                    }

                    gridAdicional.DataSource = lista;
                    ViewState["adben_"] = lista;
                }
            }
            else
            {
                gridAdicional.DataSource = ViewState["adben_"];
            }
            gridAdicional.DataBind();
        }

        AdicionalBeneficiario PegaNaColecao(IList<AdicionalBeneficiario> itens, Object adicionalId, Object beneficiarioId)
        {
            foreach (AdicionalBeneficiario _item in itens)
            {
                if (Convert.ToString(_item.AdicionalID) == Convert.ToString(adicionalId))// && 
                {
                    return _item;
                }
            }

            return null;
        }

        protected void gridAdicional_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
        }

        protected void gridAdicional_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
        }

        protected void checkboxGridAdicional_Changed(Object sender, EventArgs e)
        {
            //CheckBox check = (CheckBox)sender;

            //GridViewRow grow = (GridViewRow)check.NamingContainer;
            //int index = grow.RowIndex;

            //IList<AdicionalBeneficiario> itens =
            //    (IList<AdicionalBeneficiario>)ViewState["adben_"];

            //if (itens == null) { itens = new List<AdicionalBeneficiario>(); }

            //if (check.Checked) //adiciona na colecao
            //{
            //    for (int i = 0; i < gridAdicional.Rows.Count; i++)
            //    {
            //        Object atualAdicionalId = null;
            //        Object atualBeneficiarioId = null;

            //        if (gridAdicional.DataKeys[0].Values.Count > 0)
            //        {
            //            atualAdicionalId = gridAdicional.DataKeys[i][0];
            //            atualBeneficiarioId = ViewState[IDKey];
            //        }

            //        AdicionalBeneficiario item = PegaNaColecao(itens, atualAdicionalId, atualBeneficiarioId);

            //        Boolean adiciona = false;
            //        if (item == null)
            //        {
            //            item = new AdicionalBeneficiario();
            //            adiciona = true;
            //        }

            //        item.AdicionalID = atualAdicionalId;
            //        item.PropostaID = ViewState[IDKey];

            //        if (i == index)
            //            item.BeneficiarioID = ViewState[IDKey];

            //        if (adiciona)
            //            itens.Add(item);
            //    }

            //    Adicional ad = new Adicional();
            //    ad.ID = gridAdicional.DataKeys[index].Value;
            //    ad.Carregar();
            //    if (ad.ParaTodaProposta)// (ar != null && ar.Tipo == Convert.ToInt32(AdicionalRegra.eTipo.TitularETodosDependentes))
            //    {
            //        foreach (ListItem _item in cboBeneficiarioAdicional.Items)
            //        {
            //            IList<AdicionalBeneficiario> _itens =
            //                (IList<AdicionalBeneficiario>)ViewState["adben_"];
            //            if (_itens == null)
            //            {
            //                _itens = AdicionalBeneficiario.Carregar(ContratoADMId, PlanoId, ViewState[IDKey], _item.Value);
            //            }

            //            foreach (AdicionalBeneficiario aben in _itens)
            //            {
            //                if (Convert.ToString(ad.ID) == Convert.ToString(aben.AdicionalID))
            //                {
            //                    aben.AdicionalID = ad.ID;
            //                    aben.BeneficiarioID = _item.Value;
            //                    aben.PropostaID = ViewState[IDKey];
            //                    break;
            //                }
            //            }

            //            ViewState["adben_"] = _itens;
            //        }
            //    }
            //}
            //else //remove da colecao
            //{
            //    Adicional ad = new Adicional();
            //    ad.ID = gridAdicional.DataKeys[index].Value;
            //    ad.Carregar();
            //    if (ad.ParaTodaProposta) // (ar != null && ar.Tipo == Convert.ToInt32(AdicionalRegra.eTipo.TitularETodosDependentes))
            //    {
            //        foreach (ListItem _item in cboBeneficiarioAdicional.Items)
            //        {
            //            IList<AdicionalBeneficiario> _itens =
            //                (IList<AdicionalBeneficiario>)ViewState["adben_"];

            //            foreach (AdicionalBeneficiario aben in _itens)
            //            {
            //                if (Convert.ToString(ad.ID) == Convert.ToString(aben.AdicionalID))
            //                {
            //                    aben.BeneficiarioID = null;

            //                    if (aben.ID != null) { aben.Remover(); }
            //                    break;
            //                }
            //            }

            //            ViewState["adben_"] = _itens;
            //        }
            //    }
            //    else
            //    {
            //        if (itens != null)
            //        {
            //            itens[index].BeneficiarioID = null;

            //            if (itens[index].ID != null) { itens[index].Remover(); }
            //        }
            //    }
            //}

            //ViewState["adben_"] = itens;
        }

        #endregion

        void SetaDataAlteracaoContrato(Object beneficiarioId)
        {
            if (beneficiarioId == null) { return; }
            LC.Web.PadraoSeguros.Facade.ContratoFacade.Instance.SetaDataAlteracaoContratos(beneficiarioId);
        }

        protected void gridEnriquecimento_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja invalidar os dados?");
                base.grid_RowDataBound_Confirmacao(sender, e, 8, "Deseja confirmar os dados?");
            }
        }

        protected void gridEnriquecimento_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("ok"))
            {
                Object id = gridEnriquecimento.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Object bid = gridEnriquecimento.DataKeys[Convert.ToInt32(e.CommandArgument)][1];

                LC.Framework.Phantom.NonQueryHelper.Instance.ExecuteNonQuery("update mailing set concluido=1,valido=1,dataConclusao=getdate(),usuarioId=" + Usuario.Autenticado.ID + " where id_telMail=" + id + " and id_beneficiario=" + bid, null);
                this.checaEnriquecimento();

                base.Alerta(null, this.Page, "ok", "Dados confirmados com sucesso.");
            }
            else if (e.CommandName.Equals("invalido"))
            {
                Object id = gridEnriquecimento.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Object bid = gridEnriquecimento.DataKeys[Convert.ToInt32(e.CommandArgument)][1];

                LC.Framework.Phantom.NonQueryHelper.Instance.ExecuteNonQuery("update mailing set concluido=1,valido=0,dataConclusao=getdate(),usuarioId=" + Usuario.Autenticado.ID + " where id_telMail=" + id + " and id_beneficiario=" + bid, null);
                this.checaEnriquecimento();

                base.Alerta(null, this.Page, "ok", "Dados invalidados com sucesso.");
            }
        }

        protected void dlEnriquecimento_ItemCommand(Object source, DataListCommandEventArgs e)
        {
            if (e.CommandName == "fechar")
            {
                Object id = e.CommandArgument;

                LC.Framework.Phantom.NonQueryHelper.Instance.ExecuteNonQuery("update mailing set concluido=1,valido=0,dataConclusao=getdate(),usuarioId=" + Usuario.Autenticado.ID + " where id_telMail=" + id, null);
                this.checaEnriquecimento();

                if (!EsconderBotaoVoltar) //não foi chamado de poupup
                {
                    Response.Redirect("beneficiario.aspx?" + IDKey + "=" + Request[IDKey]);
                }
            }
        }
    }
}
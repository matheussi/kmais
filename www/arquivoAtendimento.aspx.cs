namespace www
{
    using System;
    using System.IO;
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

    using LC.Framework.Phantom;

    public partial class arquivoAtendimento : PageBase
    {
        List<String> idsBeneficiarios
        {
            get
            {
                if (ViewState["_ids"] == null) { return null; }
                else { return ViewState["_ids"] as List<String>; }
            }
            set
            {
                ViewState["_ids"] = value;
            }
        }

        List<String> idsParentescos
        {
            get
            {
                if (ViewState["_Pids"] == null) { return null; }
                else { return ViewState["_Pids"] as List<String>; }
            }
            set
            {
                ViewState["_Pids"] = value;
            }
        }

        void adicionaID(Object id)
        {
            if (idsBeneficiarios == null) { idsBeneficiarios = new List<String>(); }

            Boolean addId = true;
            
            foreach (String _id in idsBeneficiarios)
            {
                if (Convert.ToInt32(id) == Convert.ToInt32(_id))
                {
                    addId = false;
                    break;
                }
            }

            if (addId)
                idsBeneficiarios.Add(Convert.ToString(id));
        }

        void adicionaParentescoID(Object id)
        {
            if (idsParentescos == null) { idsParentescos = new List<String>(); }

            Boolean addId = true;

            foreach (String _id in idsParentescos)
            {
                if (Convert.ToInt32(id) == Convert.ToInt32(_id))
                {
                    addId = false;
                    break;
                }
            }

            if (addId)
                idsParentescos.Add(Convert.ToString(id));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);
                base.ExibirTiposDeAtendimento(cboTipo);

                setaDatas();

                UcBeneficiarioForm.EsconderBotaoSalvar = true;
                UcBeneficiarioForm.EsconderBotaoVoltar = true;
                UcBeneficiarioForm.FecharJanela = false;
                UcBeneficiarioForm.TP3.Visible = true;
                UcBeneficiarioForm.TP4.Visible = true;
                UcBeneficiarioForm.LitP3.Visible = true;
                UcBeneficiarioForm.LitP4.Visible = true;

                ViewState[IDKey2] = null;
                ViewState["at_protocolo"] = null;
            }
        }

        private Boolean ValidaAtendimentoItem()
        {
            Boolean retorno = true;
            String strMsg = "Erro: Os campos abaixo devem ser preenchidos!<br /><br />";

            if (Convert.ToInt32(cboTipo.SelectedValue) == -1)
            {
                strMsg = String.Concat(strMsg, "Assunto<br />");
                retorno = false;
            }
            if (txtTexto.Text.Trim() == "")
            {
                strMsg = String.Concat(strMsg, "Mensagem<br />");
                retorno = false;
            }

            Contrato contrato = new Contrato(ViewState[IDKey]);
            contrato.Carregar();
            //contrato = Contrato.CarregarPorParametros(txtNumeroContrato.Text, cboOperadora.SelectedValue, null);

            if (Convert.ToInt32(cboTipo.SelectedValue) == 3 || Convert.ToInt32(cboTipo.SelectedValue) == 7) // Segunda via Cartão ou Cancelar Beneficiário
            {
                Boolean checado = false;

                foreach (GridViewRow row in this.gdvClientes.Rows)
                {
                    Control ctrlCheck = row.Cells[0].FindControl("chkCliente");

                    if (ctrlCheck != null && ctrlCheck is CheckBox)
                    {
                        if (((CheckBox)ctrlCheck).Checked)
                        {
                            checado = true;
                            break;
                        }
                    }
                }

                if (!checado)
                {
                    strMsg = String.Concat(strMsg, "Selecione os beneficiários<br />");
                    retorno = false;
                }
            }
            else if (Convert.ToInt32(cboTipo.SelectedValue) == 5) // Mudança de Plano
            {
                Boolean retornoPlano = true;
                if (cboContrato.SelectedIndex < 1)
                {
                    strMsg = String.Concat(strMsg, "Contrato<br />");
                    retorno = false;
                    retornoPlano = false;
                }
                if (cboPlano.SelectedIndex < 1)
                {
                    strMsg = String.Concat(strMsg, "Plano<br />");
                    retorno = false;
                    retornoPlano = false;
                }
                if (cboAcomodacao.SelectedIndex < 1)
                {
                    strMsg = String.Concat(strMsg, "Acomodação<br />");
                    retorno = false;
                    retornoPlano = false;
                }

                if (retornoPlano)
                {
                    // Verificando se algum dos dados do plano foram alterados
                    if (Convert.ToInt32(cboContrato.SelectedValue) == Convert.ToInt32(contrato.ContratoADMID) &&
                        Convert.ToInt32(cboPlano.SelectedValue) == Convert.ToInt32(contrato.PlanoID) &&
                        Convert.ToInt32(cboAcomodacao.SelectedValue) == Convert.ToInt32(contrato.TipoAcomodacao))
                    {
                        strMsg = String.Concat(strMsg, "É necessária a alteração de alguma das informações do Plano!<br />");
                        retorno = false;
                    }
                }
            }
            else if (Convert.ToInt32(cboTipo.SelectedValue) == 4 || Convert.ToInt32(cboTipo.SelectedValue) == 6) // Alteração no cadastro ou Adicionar Beneficiário
            {
                if (idsBeneficiarios == null || idsBeneficiarios.Count <= 0)
                {
                    String strAcao = "";
                    if (Convert.ToInt32(cboTipo.SelectedValue) == 4)
                        strAcao = "alteração";
                    else if (Convert.ToInt32(cboTipo.SelectedValue) == 6)
                        strAcao = "inclusão";

                    strMsg = String.Concat(strMsg, "É necessária a ", strAcao, " de um beneficiário!<br />");
                    retorno = false;
                }
            }

            if (txtPrazo.Text.Trim() != "")
            {
                if (base.CStringToDateTime(txtPrazo.Text, "23:59") == DateTime.MinValue)
                {
                    strMsg = String.Concat(strMsg, "Prazo inválido!<br />");
                    retorno = false;
                }
            }

            //// Validando dados do contrato
            //if (contrato == null)
            //{
            //    strMsg = String.Concat(strMsg, "Contrato não encontrado!<br />");
            //    retorno = false;
            //}

            if (!retorno)
            {
                //base.Alerta(null, this, "_ErrMsg", strMsg);
                base.Alerta(MPE, ref litAlert, strMsg, upnlAlerta);
            }

            return retorno;
        }

        public Boolean CarregaDadosContrato(Object contratoId)
        {
            Boolean retorno = true;

            IList<Contrato> listContrato = Contrato.Carregar(contratoId);
            gdvClientes.DataSource = listContrato;
            gdvClientes.DataBind();

            if (listContrato != null)
            {
                gdvClientes.Columns[0].Visible = false;
                gdvClientes.Columns[1].Visible = false;

                if (Convert.ToInt32(cboTipo.SelectedValue) == 3 || Convert.ToInt32(cboTipo.SelectedValue) == 7)
                {
                    gdvClientes.Columns[0].Visible = true;
                    if (Convert.ToInt32(cboTipo.SelectedValue) == 3)
                        gdvClientes.Rows[0].Cells[0].Enabled = true;
                    else if (Convert.ToInt32(cboTipo.SelectedValue) == 7)
                        gdvClientes.Rows[0].Cells[0].Enabled = false;
                }
                else if (Convert.ToInt32(cboTipo.SelectedValue) == 4)
                    gdvClientes.Columns[1].Visible = true;

                if (listContrato[0].Cancelado)
                {
                    retorno = false;
                    base.Alerta(MPE, ref litAlert, "Contrato cancelado!", upnlAlerta);
                }
                else
                {
                    ContratoBeneficiario contratoBenAtendimento = ContratoBeneficiario.CarregarTitular(listContrato[0].ID, new PersistenceManager());

                    if (contratoBenAtendimento != null)
                    {
                        Beneficiario benAtendimento = new Beneficiario(contratoBenAtendimento.BeneficiarioID);

                        try
                        {
                            benAtendimento.Carregar();
                        }
                        catch (Exception) { throw; }

                        this.txtNome.Text     = benAtendimento.Nome;
                        this.txtEmail.Text    = benAtendimento.Email;
                        this.txtTelefone.Text = UIHelper.MascaraTelefone(benAtendimento.Telefone);

                        benAtendimento = null;
                    }

                    contratoBenAtendimento = null;
                }
            }
            else
            {
                retorno = false;
                base.Alerta(MPE, ref litAlert, "Contrato não encontrado!", upnlAlerta);
            }

            return retorno;
        }

        void BloqueiaCampos()
        {
            txtData.Enabled = false;
            txtHora.Enabled = false;
            txtNome.Enabled = false;
            txtTelefone.Enabled = false;
            txtEmail.Enabled = false;
            txtNumeroContrato.Enabled = false;
            txtCPF.Enabled = false;
            cboOperadora.Enabled = false;
            cmdAbrir.Enabled = false;
        }

        private Boolean ValidaAtendimento()
        {
            //Boolean retorno = true;
            //String strMsg = "Erro: Os campos abaixo devem ser preenchidos!<br /><br />";

            //if (cboOperadora.SelectedIndex < 0)
            //{
            //    strMsg = String.Concat(strMsg, "Operadora<br />");
            //    retorno = false;
            //}
            //if (txtNumeroContrato.Text.Trim() == "")
            //{
            //    strMsg = String.Concat(strMsg, "Número do Contrato<br />");
            //    retorno = false;
            //}

            //if (!retorno)
            //{
            //    //base.Alerta(null, this, "_ErrMsg", strMsg);
            //    base.Alerta(MPE, ref litAlert, strMsg, upnlAlerta);
            //}

            //return retorno;

            return true;
        }

        public void AbrirAtendimento(Object contratoId)
        {
            //if (ValidaAtendimento())
            //{
            ViewState[IDKey] = contratoId;
            gridContratos.DataSource = null;
            gridContratos.DataBind();

            if (CarregaDadosContrato(contratoId))
            {
                if (ViewState[IDKey2] == null)
                {
                    pnlItensAtendimento.Visible = true;
                    pnlClientes.Visible = true;

                    Atendimento atendimento = new Atendimento();
                    atendimento.OperadoraID = Convert.ToInt32(cboOperadora.SelectedValue);
                    atendimento.NumeroContrato = txtNumeroContrato.Text;
                    atendimento.Nome = txtNome.Text;
                    atendimento.Email = txtEmail.Text;
                    atendimento.Telefone = txtTelefone.Text;
                    atendimento.CPF = txtCPF.Text;
                    //atendimento.Status = (Int32)Atendimento.eStatus.Pendente;
                    atendimento.DataHora = base.CStringToDateTime(txtData.Text, txtHora.Text);
                    atendimento.AtendenteID = Usuario.Autenticado.ID;
                    atendimento.Salvar();

                    atendimento.Protocolo = atendimento.DataHora.ToString("yyyyMMdd") + atendimento.ID.ToString().PadLeft(4, '0');
                    atendimento.Salvar();

                    ViewState[IDKey2] = atendimento.ID;
                    ViewState["at_protocolo"] = atendimento.Protocolo;

                    litProtocolo.Text = "<font size='4'>Protocolo do Atendimento: <font color='yellow'>" + ViewState["at_protocolo"] + "</font></font>";

                    BloqueiaCampos();
                }
            }
            else
            {
                pnlClientes.Visible = false;
                pnlItensAtendimento.Visible = false;
            }
            //}
            //else
            //{
            //    pnlItensAtendimento.Visible = false;
            //}
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            IList<Contrato> lista = null;

            //this.limparCampos();
            this.DesbloqueiaCampos();

            if (cboOperadora.SelectedIndex > -1 && (txtNumeroContrato.Text.Trim() != "" || txtBeneificarioNome.Text.Trim() != ""))
                lista = Contrato.CarregarPorOperadoraID(cboOperadora.SelectedValue, txtNumeroContrato.Text.Trim(), txtBeneificarioNome.Text.Trim());
            else if (txtNumeroContrato.Text.Trim() != "" || txtBeneificarioNome.Text.Trim() != "")
                lista = Contrato.CarregarPorParametros(txtNumeroContrato.Text, txtBeneificarioNome.Text);
            else
            {
                base.Alerta(null, this, "erP", "Informe algum parâmetro para busca.");
            }

            gridContratos.DataSource = lista;
            gridContratos.DataBind();
        }

        protected void gridContratos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session[IDKey] = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect("contrato.aspx?" + IDKey + "=" + Session[IDKey]);
            }
            else if (e.CommandName == "abrir")
            {
                Object id = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                this.AbrirAtendimento(id);
                //Boolean status = Convert.ToBoolean(gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][2]);

                //Contrato.AlteraStatusDeContrato(id, !status);
                //this.CarregaContratos();
            }
        }

        protected void gridContratos_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Boolean rascunho = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][1]);
                ((Image)e.Row.Cells[3].Controls[1]).Visible = rascunho;

                Boolean cancelado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][2]);

                if (cancelado)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    ((LinkButton)e.Row.Cells[4].Controls[0]).Text = "<img onclick='return false;' src='../images/unactive.png' title='inativo' alt='inativo' border='0'>";
                    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente ativar o contrato?");
                }
                else
                {
                    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente cancelar o contrato?");
                    ((LinkButton)e.Row.Cells[4].Controls[0]).Text = "<img onclick='return false;' src='../images/active.png' title='ativo' alt='ativo' border='0'>";
                }

                base.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente abrir o atendimento?");
            }
        }

        protected void gridContratos_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            gridContratos.PageIndex = e.NewPageIndex;
            cmdLocalizar_Click(null, null);
        }

        protected void cmdAbrir_OnClick(Object sender, EventArgs e)
        {
            //AbrirAtendimento();
        }

        protected void cmdSalvar_OnClick(Object sender, EventArgs e)
        {
            Boolean clienteValidado = true;
            if (Convert.ToInt32(cboTipo.SelectedValue) == 4 || Convert.ToInt32(cboTipo.SelectedValue) == 6)    // Alteração no cadastro e adicionar beneficiário
            {
                if (pnlAlteracaoCadastral.Visible)
                    if (!UcBeneficiarioForm.Salvar(false))
                        clienteValidado = false;
            }

            if (clienteValidado)
            {
                if (ValidaAtendimentoItem())
                {
                    AtendimentoItem item = new AtendimentoItem();
                    item.AtendimentoId = Convert.ToInt32(ViewState[IDKey2]);
                    item.DataHora = base.CStringToDateTime(txtData.Text, txtHora.Text);
                    item.Tipo = Convert.ToInt32(cboTipo.SelectedValue);
                    item.Texto = txtTexto.Text;
                    item.Prazo = base.CStringToDateTime(txtPrazo.Text, "23:59");

                    if (Convert.ToInt32(cboTipo.SelectedValue) != 1)
                        item.Status = (Int32)Atendimento.eStatus.Pendente; //false;

                    String strBeneficiarioIds = "";
                    String strParentescoIds = "";

                    // Cancela contrato
                    if (Convert.ToInt32(cboTipo.SelectedValue) == 2) 
                    {
                        //Object contratoId = Contrato.CarregaContratoID(cboOperadora.SelectedValue, txtNumeroContrato.Text, null);
                        //Contrato.AlteraStatusDeContrato(contratoId, true);
                    }
                    // Segunda via de cartão e Cancelar beneficiários
                    else if (Convert.ToInt32(cboTipo.SelectedValue) == 3 || Convert.ToInt32(cboTipo.SelectedValue) == 7)
                    {
                        foreach (GridViewRow row in this.gdvClientes.Rows)
                        {
                            Control ctrlCheck = row.Cells[0].FindControl("chkCliente");
                            if (ctrlCheck != null && ctrlCheck is CheckBox)
                            {
                                if (((CheckBox)ctrlCheck).Checked)
                                {
                                    if (strBeneficiarioIds.Trim() != "")
                                        strBeneficiarioIds += ",";
                                    strBeneficiarioIds += gdvClientes.DataKeys[Convert.ToInt32(row.RowIndex)].Values[0];
                                    // Inativa o Beneficiário
                                    //if (Convert.ToInt32(cboTipo.SelectedValue) == 7)
                                    //    ContratoBeneficiario.InativaBeneficiario(Contrato.CarregaContratoID(cboOperadora.SelectedValue, txtNumeroContrato.Text, null), gdvClientes.DataKeys[Convert.ToInt32(row.RowIndex)].Values[0], null);
                                }
                            }
                        }
                    }
                    // Alteração no cadastro e adicionar beneficiário
                    else if (Convert.ToInt32(cboTipo.SelectedValue) == 4 || Convert.ToInt32(cboTipo.SelectedValue) == 6)
                    {
                        if (idsBeneficiarios != null)
                        {
                            foreach (String _id in idsBeneficiarios)
                            {
                                if (strBeneficiarioIds.Trim() != "")
                                    strBeneficiarioIds += ",";
                                strBeneficiarioIds += _id;
                            }
                        }
                        if (idsParentescos != null)
                        {
                            foreach (String _id in idsParentescos)
                            {
                                if (strParentescoIds.Trim() != "")
                                    strParentescoIds += ",";
                                strParentescoIds += _id;
                            }
                        }
                    }
                    // Mudança de Plano
                    else if (Convert.ToInt32(cboTipo.SelectedValue) == 5)
                    {
                        item.AcomodacaoId = Convert.ToInt32(cboAcomodacao.SelectedValue);
                        item.PlanoId = Convert.ToInt32(cboPlano.SelectedValue);
                    }

                    item.BeneficiarioIds = strBeneficiarioIds;
                    item.ParentescoIds = strParentescoIds;
                    item.Salvar();
                    base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
                    limparAtendimentoItens();
                }
            }
        }

        //protected void cboOperadora_OnSelectedIndexChanged(Object sender, EventArgs e)
        //{
        //    if (txtNumeroContrato.Text.ToString().Trim() != "")
        //    {
        //        AbrirAtendimento();
        //        if (Convert.ToInt32(cboTipo.SelectedValue) == 5)
        //            CarregaContratos();
        //    }

        //    txtNumeroContrato.Focus();
        //}

        protected void cboTipo_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            CarregaDadosContrato(ViewState[IDKey]);

            pnlAlteracaoCadastralCPF.Visible = false;
            pnlAlteracaoPlano.Visible = false;
            pnl2ViaBoleto.Visible = false;
            pnlAlteracaoCadastral.Visible = false;

            if (Convert.ToInt32(cboTipo.SelectedValue) == 1)
                CarregaBoletos();
            else if (Convert.ToInt32(cboTipo.SelectedValue) == 5)
                CarregaContratos();
            else if (Convert.ToInt32(cboTipo.SelectedValue) == 6)
                CarregarFormBeneficiario();
        }

        //protected void txtNumeroContrato_OnTextChanged(Object sender, EventArgs e)
        //{
        //    cboTipo_OnSelectedIndexChanged(null, null);

        //    txtCPF.Focus();
        //}

        private void CarregaBoletos()
        {
            pnl2ViaBoleto.Visible = true;
            IList<Cobranca> listBoleto = Cobranca.CarregarBoletos(cboOperadora.SelectedValue, txtNumeroContrato.Text);
            gdvCobrancas.DataSource = listBoleto;
            gdvCobrancas.DataBind();

            if (listBoleto != null)
            {
                gdvCobrancas.Visible = true;
            }
            else
            {
                gdvCobrancas.Visible = false;
                base.Alerta(MPE, ref litAlert, "Nenhuma cobrança pendente encontrada!", upnlAlerta);
            }
        }

        protected void gdvCobrancas_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button buttonGerarBoleto = (Button)e.Row.FindControl("btnGerarBoleto");
                buttonGerarBoleto.CommandArgument = e.Row.RowIndex.ToString();
                ScriptManager.GetCurrent(this).RegisterPostBackControl(buttonGerarBoleto);
            }
        }

        protected void gdvCobrancas_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("GerarBoleto"))
            {
                Object Id = gdvCobrancas.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[0];
                    Cobranca cobranca = new Cobranca(Id);
                    cobranca.Carregar();
                    String nossoNumero = cobranca.GeraNossoNumero();
                    nossoNumero = nossoNumero.Substring(1);

                Decimal Valor      = cobranca.Valor; // Convert.ToDecimal(gdvCobrancas.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[1]);
                DateTime DataVenct = cobranca.DataVencimento;// Convert.ToDateTime(gdvCobrancas.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[2]);

                Contrato contrato = new Contrato(cobranca.PropostaID);
                contrato.Carregar();

                DateTime vigencia, vencimento;
                Int32 diaDataSemJuros=-1;
                Object valorDataLimite = null;
                CalendarioVencimento rcv = null;

                CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contrato.ContratoADMID,
                    contrato.Admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv);

                DateTime dataSemJuros = DateTime.MinValue;

                try
                {
                    dataSemJuros = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, diaDataSemJuros, 23, 59, 59);
                }
                catch { }

                Int32 dia = DateTime.Now.AddDays(1).Day;
                Int32 mes = DateTime.Now.AddDays(1).Month;
                Int32 ano = DateTime.Now.AddDays(1).Year;

                if (DataVenct > DateTime.Now) //vencimento > data atual
                {
                    dia = DataVenct.Day;
                    mes = DataVenct.Month;
                    ano = DataVenct.Year;
                }

                if (dataSemJuros != DateTime.MinValue && dataSemJuros < DateTime.Now)
                {
                    //CALCULA OS JUROS
                    DateTime database = new DateTime(ano, mes, dia, 23, 59, 59, 500);
                    TimeSpan tempoAtraso = database.Subtract(dataSemJuros);

                    Decimal atraso = Convert.ToDecimal(ConfigurationManager.AppSettings["jurosAtraso"]);
                    Decimal atrasoDia = Convert.ToDecimal(ConfigurationManager.AppSettings["jurosDia"]);

                    Valor += Valor * atraso;

                    if (tempoAtraso.Days > 1)
                    {
                        Valor += Valor * (atrasoDia * (tempoAtraso.Days));
                    }
                }

                String Nome = txtNome.Text; //Convert.ToString(gdvCobrancas.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[3]);
                String Email = txtEmail.Text; //Convert.ToString(gdvCobrancas.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[4]);

                //ScriptManager.RegisterClientScriptBlock(this, 
                //    this.GetType(), 
                //    "_geraBoleto_" + Id,
                //    String.Concat(" window.open(\"http://www.boletomail.com.br/do.php?nossonum=", nossoNumero, "&valor=", Valor, "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano, "&nome=", Nome, "&cod_cli=", Id, "&mailto=", Email, Cobranca.BoletoUrlComp, "&user=padraovida&action=3\", \"janela\", \"toolbar=no,scrollbars=1,width=860,height=420\"); "), 
                //    true);

            }
        }

        protected void gdvClientes_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton buttonEditarCadastro = (LinkButton)e.Row.FindControl("btnEditaCadastro");
                buttonEditarCadastro.CommandArgument = e.Row.RowIndex.ToString();
                ScriptManager.GetCurrent(this).RegisterPostBackControl(buttonEditarCadastro);
            }
        }

        protected void gdvClientes_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("EditarCadastro"))
            {
                Object Id = gdvClientes.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[0];

                UcBeneficiarioForm.CarregaBeneficiario(Id);
                UcBeneficiarioForm.TRParentesco.Visible = false;
                pnlAlteracaoCadastral.Visible = true;
                pnlAlteracaoCadastralCPF.Visible = false;

                Contrato contrato = new Contrato(Contrato.CarregaContratoID(cboOperadora.SelectedValue, txtNumeroContrato.Text, null));
                contrato.Carregar();

                UcBeneficiarioForm.ContratoId = contrato.ID;
                UcBeneficiarioForm.ContratoADMId = Contrato.CarregaContratoAdmID(contrato.ID, null);
                UcBeneficiarioForm.PlanoId = contrato.PlanoID;
                UcBeneficiarioForm.OperadoraId = contrato.OperadoraID;

                UcBeneficiarioForm.CarregaFichaDeSaude();
                UcBeneficiarioForm.CarregaAdicionais();
            }
        }

        protected void btnFechar_OnClick(Object sender, EventArgs e)
        {
            fechaAlteracaoDadosCadastrais();
        }

        protected void btnSalvarCadastro_OnClick(Object sender, EventArgs e)
        {
            Object beneficiarioId = null;

            if (UcBeneficiarioForm.Salvar(false, out beneficiarioId))
            {
                fechaAlteracaoDadosCadastrais();
                adicionaID(beneficiarioId);

                if (Convert.ToInt32(cboTipo.SelectedValue) == 6)    // Adicionar Beneficiário
                    adicionaParentescoID(UcBeneficiarioForm.CBOParentesco.SelectedValue);

                UcBeneficiarioForm.LimparCampos();
            }
        }

        void fechaAlteracaoDadosCadastrais()
        {
            pnlAlteracaoCadastral.Visible = false;
            if (Convert.ToInt32(cboTipo.SelectedValue) == 6) // Novo Beneficiário
                pnlAlteracaoCadastralCPF.Visible = true;
        }

        public void limparCampos()
        {
            setaDatas();
            ViewState[IDKey] = null;
            ViewState["_ids"] = null;
            ViewState["_Pids"] = null;
            ViewState[IDKey2] = null;
            ViewState["at_protocolo"] = null;

            litProtocolo.Text = null;
            txtNome.Text = "";
            txtTelefone.Text = "";
            txtEmail.Text = "";
            //cboOperadora.SelectedIndex = 0;
            txtNumeroContrato.Text = "";
            txtCPF.Text = "";
            cboTipo.SelectedValue = "0";
            txtTexto.Text = "";
            pnl2ViaBoleto.Visible = false;
            pnlAlteracaoCadastral.Visible = false;
            pnlClientes.Visible = false;
            pnlAlteracaoPlano.Visible = false;
            pnlItensAtendimento.Visible = false;
            pnlAlteracaoCadastralCPF.Visible = false;
        }

        public void limparAtendimentoItens()
        {
            idsBeneficiarios = null;
            idsParentescos = null;
            cboTipo.SelectedValue = "0";
            txtTexto.Text = "";
            pnl2ViaBoleto.Visible = false;
            pnlAlteracaoCadastral.Visible = false;
            pnlAlteracaoCadastralCPF.Visible = false;
            pnlAlteracaoPlano.Visible = false;
        }

        void setaDatas()
        {
            txtData.Text = String.Concat(DateTime.Now.Day.ToString().PadLeft(2, '0'), "/", DateTime.Now.Month.ToString().PadLeft(2, '0'), "/", DateTime.Now.Year);
            txtHora.Text = String.Concat(DateTime.Now.Hour.ToString().PadLeft(2, '0'), ":", DateTime.Now.Minute.ToString().PadLeft(2, '0'));

            txtPrazo.Text = String.Concat(DateTime.Now.AddDays(2).Day.ToString().PadLeft(2, '0'), "/", DateTime.Now.AddDays(2).Month.ToString().PadLeft(2, '0'), "/", DateTime.Now.AddDays(2).Year);
        }

        private void CarregaContratos()
        {
            pnlAlteracaoPlano.Visible = true;

            Contrato contrato = new Contrato();
            contrato = Contrato.CarregarPorParametros(txtNumeroContrato.Text, (Object)cboOperadora.SelectedValue, null);

            if (contrato != null)
            {
                montaComboContratos(contrato.EstipulanteID, false);
                cboContrato.SelectedValue = Convert.ToString(contrato.ContratoADMID);
                montaComboPlanos(false);
                cboPlano.SelectedValue = Convert.ToString(contrato.PlanoID);
                montaComboAcomodacoes();
                cboAcomodacao.SelectedValue = Convert.ToString(contrato.TipoAcomodacao);
            }
            else
            {
                pnlAlteracaoPlano.Visible = false;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "_alert_", " alert(\"Contrato não encontrado!\"); ", true);
            }
        }

        private void montaComboContratos(Object estipulanteId, Boolean carregaProximo)
        {
            cboContrato.Items.Clear();
            cboContrato.DataValueField = "ID";
            cboContrato.DataTextField = "Descricao";
            IList<ContratoADM> lista = ContratoADM.Carregar(estipulanteId, cboOperadora.SelectedValue);
            cboContrato.DataSource = lista;
            cboContrato.DataBind();
            cboContrato.Items.Insert(0, new ListItem("selecione", "-1"));

            if (carregaProximo)
                montaComboPlanos(true);
        }

        protected void cboContrato_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            montaComboPlanos(false);
            cboAcomodacao.Items.Clear();
        }

        private void montaComboPlanos(Boolean carregaProximo)
        {
            cboPlano.Items.Clear();
            IList<Plano> planos = Plano.CarregarPorContratoID(cboContrato.SelectedValue, true);

            cboPlano.Items.Clear();
            cboPlano.DataValueField = "ID";
            cboPlano.DataTextField = "Descricao";
            cboPlano.DataSource = planos;
            cboPlano.DataBind();
            cboPlano.Items.Insert(0, new ListItem("Selecione", "-1"));

            if (carregaProximo)
                montaComboAcomodacoes();
        }

        protected void cboPlano_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            montaComboAcomodacoes();
        }

        private void montaComboAcomodacoes()
        {
            if (cboPlano.SelectedIndex >= 0)
            {
                Plano plano = new Plano(cboPlano.SelectedValue);
                plano.Carregar();
                base.ExibirTiposDeAcomodacao(cboAcomodacao, plano.QuartoComum, plano.QuartoParticular, true);
            }
            else
            {
                cboAcomodacao.Items.Clear();
                cboAcomodacao.Items.Add(new ListItem("Selecione", "-1"));
            }
        }

        private void CarregarFormBeneficiario()
        {
            if (Convert.ToInt32(cboTipo.SelectedValue) == 6) //novo
            {
                if (pnlAlteracaoCadastralCPF.Visible)
                {
                    UcBeneficiarioForm.LimparCampos();

                    Contrato contrato = new Contrato(Contrato.CarregaContratoID(cboOperadora.SelectedValue, txtNumeroContrato.Text, null));
                    contrato.Carregar();

                    UcBeneficiarioForm.ContratoId = contrato.ID;
                    UcBeneficiarioForm.ContratoADMId = Contrato.CarregaContratoAdmID(contrato.ID, null);
                    UcBeneficiarioForm.PlanoId = contrato.PlanoID;
                    UcBeneficiarioForm.OperadoraId = contrato.OperadoraID;

                    Object beneficiarioId = null;
                    Boolean cpfEmUsoOuInvalido = false;

                    if (Beneficiario.ChecaCpf(beneficiarioId, txtCPFNovoBeneficiario.Text))
                    {
                        pnlAlteracaoCadastral.Visible = true;
                        beneficiarioId = Beneficiario.VerificaExistenciaCPF(txtCPFNovoBeneficiario.Text);

                        UcBeneficiarioForm.TRParentesco.Visible = true;
                        CarregaOpcoesParentesco();

                        if (beneficiarioId != null)
                        {
                            if (Contrato.VerificaExistenciaBeneficiarioNoContrato(beneficiarioId, Contrato.CarregaContratoID(cboOperadora.SelectedValue, txtNumeroContrato.Text, null)))
                            {
                                UcBeneficiarioForm.CarregaBeneficiario(beneficiarioId);
                                UcBeneficiarioForm.CarregaFichaDeSaude();
                                UcBeneficiarioForm.CarregaAdicionais();
                            }
                            else
                                cpfEmUsoOuInvalido = true;
                        }
                        else
                        {
                            UcBeneficiarioForm.CarregaFichaDeSaude();
                            UcBeneficiarioForm.CarregaAdicionais();
                        }
                    }
                    else
                        cpfEmUsoOuInvalido = true;

                    if (cpfEmUsoOuInvalido)
                    {
                        pnlAlteracaoCadastral.Visible = false;
                        UcBeneficiarioForm.TRParentesco.Visible = false;
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "_alert_", " alert(\"O CPF informado é inválido ou já está em uso.\"); ", true);
                        pnlAlteracaoCadastralCPF.Visible = true;
                    }
                    else
                    {
                        pnlAlteracaoCadastralCPF.Visible = false;
                        UcBeneficiarioForm.TXTCPF.Text = txtCPFNovoBeneficiario.Text;
                    }
                }
                else
                {
                    pnlAlteracaoCadastralCPF.Visible = true;
                }
            }
        }

        void CarregaOpcoesParentesco()
        {
            UcBeneficiarioForm.CBOParentesco.Items.Clear();

            IList<ContratoADMParentescoAgregado> lista =
                ContratoADMParentescoAgregado.Carregar(
                Contrato.CarregaContratoAdmID(Contrato.CarregaContratoID(cboOperadora.SelectedValue, txtNumeroContrato.Text, null), null),
                Parentesco.eTipo.Indeterminado);

            UcBeneficiarioForm.CBOParentesco.DataValueField = "ID";
            UcBeneficiarioForm.CBOParentesco.DataTextField = "ParentescoDescricao";
            UcBeneficiarioForm.CBOParentesco.DataSource = lista;
            UcBeneficiarioForm.CBOParentesco.DataBind();
        }

        void DesbloqueiaCampos()
        {
            txtData.Enabled = true;
            txtHora.Enabled = true;
            txtNome.Enabled = true;
            txtTelefone.Enabled = true;
            txtEmail.Enabled = true;
            txtNumeroContrato.Enabled = true;
            txtCPF.Enabled = true;
            cboOperadora.Enabled = true;
            cmdAbrir.Enabled = true;
        }

        protected void cmdFinalizar_OnClick(Object sender, EventArgs e)
        {
            IList<AtendimentoItem> lista = AtendimentoItem.CarregaAtendimentoItens(ViewState[IDKey2]);

            if (lista != null && lista.Count > 0)
            {
                DesbloqueiaCampos();
                limparCampos();
            }
            else
                base.Alerta(MPE, ref litAlert, "É necessária a realização de alguma operação\npara finalizar o atendimento.", upnlAlerta);
        }

        protected void cmdLocalizar_OnClick(Object sender, EventArgs e)
        {
            CarregarFormBeneficiario();
        }

    }
}
namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    using LC.Framework.Phantom;

    public partial class cobrancaRecuperacao : PageBase
    {
        String _motivoId = "11";

        Object contratoId
        {
            get { return ViewState["contratoid"]; }
            set { ViewState["contratoid"] = value; }
        }

        IList<Cobranca> parcelamento
        {
            get { return Session["_parc"] as IList<Cobranca>; }
            set { if (value == null) { Session.Remove("_parc"); } else { Session["_parc"] = value; } }
        }

        List<String> iDsSelecionados
        {
            get { return Session["_ids"] as List<String>; }
            set { if (value == null) { Session.Remove("_ids"); } else { Session["_ids"] = value; } }
        }
        
        protected void Page_Load(Object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.CarregaMultas();
                this.CarregaDescontos();
                this.CarregaEmpresas();
                base.ExibirOperadoras(cboOperadoras, true);

                cmdCancelarParcelamento.Enabled = Usuario.Autenticado.ExtraPermission;
                if (Perfil.IsAdmin(Usuario.Autenticado.PerfilID)) cmdCancelarParcelamento.Enabled = true;

                Usuario user = new Usuario(Usuario.Autenticado.ID);
                user.Carregar();

                if (user.EmpresaCobrancaID != null)
                {
                    cboEmpresaFiltro.SelectedValue = Convert.ToString(user.EmpresaCobrancaID);
                    cboEmpresaFiltro.Enabled = false;
                }
                else
                {
                    cboEmpresaFiltro.Items.Insert(0, new ListItem("selecione", ""));
                }
            }

            if (Usuario.Autenticado.PerfilID == Perfil.ConsultaPropostaBeneficiarioIDKey)
            {
                cmdCalcular.Visible = false;
                cmdCalcular.Enabled = false;
                cmdGravar.Visible = false;
                cmdGravar.Enabled = false;
                cmdCancelarParcelamento.Visible = false;
                cmdCancelarParcelamento.Enabled = false;
            }
        }

        void CarregaDescontos()
        {
            cboDesconto.Items.Clear();
            String descontos = ConfigurationManager.AppSettings["desconto"];

            cboDesconto.Items.Add(new ListItem("nenhum", "-1"));

            if (!String.IsNullOrEmpty(descontos))
            {
                String[] arr = descontos.Split(';');
                foreach (String item in arr)
                {
                    cboDesconto.Items.Add(new ListItem(item.Split('|')[0], item.Split('|')[1]));
                }
            }

            //cboDesconto.Items.Add(new ListItem("isentar juros", "0"));
        }

        void CarregaMultas()
        {
            cboMulta.Items.Clear();
            String multaConf = ConfigurationManager.AppSettings["jurosAtrasoArr"];

            cboMulta.Items.Add(new ListItem("isentar", "0"));

            if (!String.IsNullOrEmpty(multaConf))
            {
                String[] arr = multaConf.Split(';');
                foreach (String item in arr)
                {
                    cboMulta.Items.Add(new ListItem(item.Split('|')[0], item.Split('|')[1]));
                }

                cboMulta.SelectedIndex = 1;
            }
        }

        void CarregaContratos()
        {
            if (cboEmpresaFiltro.Items.Count == 0) { return; }

            this.pnlCobrancas.Visible = false;
            gridParcelamento.DataSource = null;
            gridParcelamento.DataBind();

            DataTable lista = null;

            Object empresaId = null;
            if (cboEmpresaFiltro.SelectedValue != "") empresaId = cboEmpresaFiltro.SelectedValue;

            Boolean somenteInativos = true, somenteAtivos = false;
            if (Usuario.Autenticado.ExtraPermission) somenteInativos = false;
            if (Usuario.Autenticado.EmpresaCobrancaID == "2") { somenteAtivos = true; somenteInativos = false; }

            if (cboOperadoras.SelectedIndex > 0 && (txtNumProposta.Text.Trim() != "" || txtBeneficiarioNome.Text.Trim() != "" || txtIDCobranca.Text.Trim() != "" || txtProtocolo.Text.Trim() != "" || txtBeneficiarioCPF.Text.Trim() != ""))
                lista = Contrato.DTCarregarPorOperadoraID(cboOperadoras.SelectedValue, txtNumProposta.Text.Trim(), txtBeneficiarioNome.Text.Trim(), txtIDCobranca.Text.Trim(), txtProtocolo.Text.Trim(), empresaId, somenteInativos, txtBeneficiarioCPF.Text, somenteAtivos);
            else if (txtNumProposta.Text.Trim() != "" || txtBeneficiarioNome.Text.Trim() != "" || txtIDCobranca.Text.Trim() != "" || txtProtocolo.Text.Trim() != "" || txtBeneficiarioCPF.Text.Trim() != "")
                lista = Contrato.DTCarregarPorParametros(txtNumProposta.Text, txtBeneficiarioNome.Text, txtIDCobranca.Text, txtProtocolo.Text.Trim(), empresaId, somenteInativos, txtBeneficiarioCPF.Text, somenteAtivos);

            gridContratos.DataSource = lista;
            gridContratos.DataBind();
        }

        void CarregaEmpresas()
        {
            DataTable dt = LocatorHelper.Instance.ExecuteQuery("select * from cobranca_empresa order by empresa_nome", "result").Tables[0];

            cboEmpresaFiltro.DataValueField = "empresa_id";
            cboEmpresaFiltro.DataTextField = "empresa_nome";
            cboEmpresaFiltro.DataSource = dt;
            cboEmpresaFiltro.DataBind();
            //cboEmpresa.DataValueField = "empresa_id";
            //cboEmpresa.DataTextField  = "empresa_nome";
            //cboEmpresa.DataSource = dt;
            //cboEmpresa.DataBind();
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            this.CarregaContratos();
            pnlCobrancas.Visible = false;
            pnlEmails.Visible = false;
            if (gridContratos.Rows.Count == 0) { litAviso.Text = "Nenhuma proposta localizada"; }
        }

        protected void gridContratos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session[IDKey] = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                String uri = "../contrato.aspx?" + IDKey + "=" + Session[IDKey];
                //if (txtProtocolo.Text.Trim() != "")
                //    uri += "&prot=" + txtProtocolo.Text;

                Response.Redirect(uri);
            }
            else if (e.CommandName == "exibirCobrancas")
            {
                gridParcelamento.DataSource = null;
                gridParcelamento.DataBind();

                this.pnlEmails.Visible = false;
                this.pnlCobrancas.Visible = true;
                this.contratoId = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                this.CarregarCobrancas();

                ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(this.contratoId, null);
                if (titular != null)
                {
                    Beneficiario beneficiario = new Beneficiario(titular.BeneficiarioID);
                    beneficiario.Carregar();
                    txtEmail.Text = beneficiario.Email;
                }
            }
            else if (e.CommandName == "email")
            {
                this.pnlEmails.Visible = true;
                this.pnlCobrancas.Visible = false;
                this.contratoId = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                this.exibirEnvioDeEmail(this.contratoId);
            }
        }

        void exibirEnvioDeEmail(Object contratoid)
        {
            IList<Cobranca> parcelamento = this.CarregarCobrancasTipoParcelamento();
            if (parcelamento != null)
            {
                ParcelamentoItem parcItem = ParcelamentoItem.CarregarPorCobrancaId(parcelamento[0].ID);
                ParcelamentoHeader header = new ParcelamentoHeader(parcItem.HeaderID);
                header.Carregar();

                txtVenctoR.Text = header.VenctoInicial.ToString("dd/MM/yyyy");
                txtQtdParcelasR.Text = header.Parcelas.ToString();

                chkIsentarJurosR.Checked = header.IsentaJuros;
                cboMultaR.Text = header.Multa;
                cboDescontoR.Text = header.Desconto;
                txtEmailR.Text = header.Email;

                txtObsR.Text = header.OBS;
            }
        }

        protected void gridContratos_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Boolean rascunho = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][1]);
                //((Image)e.Row.Cells[3].Controls[1]).Visible = rascunho;

                //Boolean cancelado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][2]);
                //Boolean inativado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][3]);

                //if (cancelado || inativado)
                //{
                //    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                //    ((LinkButton)e.Row.Cells[4].Controls[0]).Text = "<img src='../images/unactive.png' title='inativo' alt='inativo' border='0'>";
                //}
                //else
                //{
                //    ((LinkButton)e.Row.Cells[4].Controls[0]).Text = "<img src='../images/active.png' title='ativo' alt='ativo' border='0'>";
                //}
            }
        }

        protected void gridContratos_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            gridContratos.PageIndex = e.NewPageIndex;
            this.CarregaContratos();
        }

        ///// COBRANCAS

        void CarregarCobrancas()
        {
            Contrato contrato = new Contrato(this.contratoId);
            contrato.Carregar();
            gridCobranca.DataSource = Cobranca.CarregarTodasComParcelamentoInfo(this.contratoId, true, true, -1, null, contrato.DataCancelamento);
            gridCobranca.DataBind();
        }

        /// <summary>
        /// Para o detalhe do parcelamento
        /// </summary>
        IList<Cobranca> CarregarCobrancasTipoParcelamento()
        {
            Contrato contrato = new Contrato(this.contratoId);
            contrato.Carregar();

            IList<Cobranca> cobr = Cobranca.CarregarTodasComParcelamentoInfo(this.contratoId, true, Cobranca.eTipo.Parcelamento, null);
            gridEmail.DataSource = cobr;
            gridEmail.DataBind();

            return cobr;
        }

        protected void gridCobranca_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (gridCobranca.DataKeys[e.Row.RowIndex][1] != null && 
                    gridCobranca.DataKeys[e.Row.RowIndex][1] != DBNull.Value &&
                    Convert.ToString(gridCobranca.DataKeys[e.Row.RowIndex][1]).Trim() != "") 
                {
                    ((CheckBox)e.Row.Cells[4].FindControl("chk")).Enabled = false;
                    ((CheckBox)e.Row.Cells[4].FindControl("chk")).ToolTip = "parcelamento ja efetuado";
                    e.Row.ForeColor = System.Drawing.Color.Gray;

                }

                //Cobranca.eTipo tipo = (Cobranca.eTipo)Convert.ToInt32(gridCobranca.DataKeys[e.Row.RowIndex][2]);
                //if (tipo != Cobranca.eTipo.Parcelamento)
                //{
                //    e.Row.Cells[5].Enabled = false;
                //    e.Row.Cells[5].Controls[0].Visible = false;
                //}
            }
        }

        protected void gridCobranca_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            gridCobranca.PageIndex = e.NewPageIndex;
            this.CarregarCobrancas();
        }

        protected void gridCobranca_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "recalcular")
            {
            }
            if (e.CommandName == "email" || e.CommandName == "print")
            {
            }
            else if (e.CommandName.Equals("detalhe"))
            {
            }
        }

        Boolean valida()
        {
            if (gridCobranca.Rows.Count == 0)
            {
                base.Alerta(null, this, "err", "Não há cobranças pendentes.");
                return false;
            }

            if (CStringToDateTime(txtVencto.Text) == DateTime.MinValue)
            {
                base.Alerta(null, this, "err", "Informe a data de vencimento inicial.");
                return false;
            }

            int aux = 0;
            if (!Int32.TryParse(txtQtdParcelas.Text, out aux))
            {
                base.Alerta(null, this, "err", "Informe a quantidade de parcelas.");
                return false;
            }

            if (aux < 1)
            {
                base.Alerta(null, this, "err", "Quantidade de parcelas inválida.");
                return false;
            }

            if (!Usuario.Autenticado.ExtraPermission)
            {
                if (aux > 6)
                {
                    base.Alerta(null, this, "err", "Quantidade máxima de parcelas: 6.");
                    return false;
                }
            }

            //!Perfil.IsFinanceiro(Usuario.Autenticado.PerfilID) && 
            if (!Perfil.IsAdmin(Usuario.Autenticado.PerfilID) || !Usuario.Autenticado.ExtraPermission)
            {
                if (aux != 1 && cboDesconto.SelectedIndex > 0) //nao é à vista, e está permitindo desconto
                {
                    base.Alerta(null, this, "err", "Não permitido conceder desconto para negociações parceladas.");
                    return false;
                }
            }

            return true;
        }

        protected void cmdCalcular_click(Object sender, EventArgs e)
        {
            //CommandArgument='<%# Container.DataItemIndex %>'

            #region validacoes 

            if (!valida()) { return; }
            
            #endregion

            Contrato contrato = new Contrato(this.contratoId);
            contrato.Carregar();

            List<String> ids = new List<String>();
            for (int i = 0; i < gridCobranca.Rows.Count; i++)
            {
                if(((CheckBox)gridCobranca.Rows[i].Cells[4].FindControl("chk")).Checked)
                {
                    ids.Add(Convert.ToString(gridCobranca.DataKeys[i][0]));
                }
            }

            if (ids.Count == 0) { base.Alerta(null, this, "err", "Não há cobranças selecionadas."); return; }
            this.iDsSelecionados = ids;

            IList<Cobranca> cobrancas = Cobranca.CarregarTodas(ids, null);

            Decimal totalBruto = 0, totalCalculado = 0, totalMulta = 0, totalJuro = 0, valorTemp = 0;
            Decimal atrasoMulta = 0; 
            Decimal atrasoDia = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["jurosDia"]);

            if (cboMulta.SelectedIndex == 0) { atrasoMulta = 0; }
            else { atrasoMulta = Convert.ToDecimal(cboMulta.SelectedValue); }

            //calcula juro e multa //////////////////////////////////////////////////////
            DateTime database = DateTime.MinValue;

            TimeSpan tempoAtraso = TimeSpan.Zero;

            int j = 1;
            foreach (Cobranca c in cobrancas)
            {
                if (c.Tipo == (int)Cobranca.eTipo.Parcelamento)
                {
                    base.Alerta(null, this, "_err", "Não é possível negociar parcelas de negociações passadas.");
                    return;
                }

                database = CStringToDateTime(txtVencto.Text, "23:59", 59).AddMonths(j - 1);
                j++;

                valorTemp = c.Valor;

                totalBruto += c.Valor;

                tempoAtraso = database.Subtract(c.DataVencimento);

                if (cboDesconto.SelectedValue != "-1") //&& cboDesconto.SelectedValue != "0")
                {
                    //foi aplicado desconto
                    c.Valor -= (c.Valor * Convert.ToDecimal(cboDesconto.SelectedValue));
                    valorTemp = c.Valor;
                }

                //multa
                if (atrasoMulta > 0)
                {
                    c.MultaRS = c.Valor * atrasoMulta; 
                    totalMulta += c.MultaRS;
                }

                if (tempoAtraso.Days > 1 && !chkIsentarJuros.Checked)// cboDesconto.SelectedValue != "0") //se está atrasado e nao foi concedido isençao de juros
                {
                    Double a = Math.Pow(1.000333333333, Convert.ToDouble(tempoAtraso.Days));
                    valorTemp = Convert.ToDecimal(a) * c.Valor;
                }

                c.JurosRS = valorTemp - c.Valor;
                totalJuro += c.JurosRS;

                c.Valor = c.Valor + c.JurosRS + c.MultaRS;

                totalCalculado += c.Valor;
            }

            litTotalParcelamento.Text = String.Concat("Total bruto: ", 
                totalBruto.ToString("C"), "  -  Total calculado: ", totalCalculado.ToString("C"));

            ///////////////////////////////////////////////////////////////////////////////

            Decimal valorParcela = totalCalculado / Convert.ToDecimal(txtQtdParcelas.Text);
            Decimal multaParcela = totalMulta / Convert.ToDecimal(txtQtdParcelas.Text);

            IList<Cobranca> temp = Cobranca.CarregarTodas(cobrancas[0].PropostaID, true, null);

            int parcelaAtual = temp[temp.Count - 1].Parcela;
            List<Cobranca> parcelas = new List<Cobranca>();

            database = CStringToDateTime(txtVencto.Text, "23:59", 59);

            Decimal saldoDevedor = totalCalculado; // amortizacao = 0;
            for (int i = 1; i <= Convert.ToInt32(txtQtdParcelas.Text); i++)
            {
                valorTemp = 0;

                Cobranca parcela = new Cobranca();
                parcela.Cancelada = false;
                parcela.Carteira = 0;
                parcela.ComissaoPaga = false;
                parcela.DataCriacao = DateTime.Now;
                parcela.DataVencimento = CStringToDateTime(txtVencto.Text, "23:59", 59).AddMonths(i - 1);
                parcela.Pago = false;
                parcela.Parcela = parcelaAtual + i;
                parcela.PropostaID = cobrancas[0].PropostaID;
                parcela.Tipo = (int)Cobranca.eTipo.Parcelamento;

                if (i == 1)
                {
                    if (Convert.ToInt32(txtQtdParcelas.Text) == 1 || chkIsentarJuros.Checked)// cboDesconto.SelectedValue == "0") //ou concede desconto
                    {
                        parcela.Valor = valorParcela;
                    }
                    else
                    {
                        valorTemp = Convert.ToDecimal((Convert.ToDouble(totalCalculado) * Convert.ToDouble(0.01)) / (Convert.ToDouble(1) - (Convert.ToDouble(1) / (Math.Pow((Convert.ToDouble(1) + 0.01), Convert.ToDouble(txtQtdParcelas.Text))))));
                        //parcela.JurosRS = valorTemp - valorParcela;
                        parcela.Valor = valorTemp; //Convert.ToDecimal((Convert.ToDouble(totalCalculado) * Convert.ToDouble(0.01)) / (Convert.ToDouble(1) - (Convert.ToDouble(1) / (Math.Pow((Convert.ToDouble(1) + 0.01), Convert.ToDouble(txtQtdParcelas.Text))))));
                    }
                }
                else if (i > 1 && !chkIsentarJuros.Checked)
                {
                    tempoAtraso = parcela.DataVencimento.Subtract(database);
                    if (tempoAtraso.Days > 1 && cboDesconto.SelectedValue != "0") //e NAO concede desconto)
                    {
                        valorTemp = Convert.ToDecimal((Convert.ToDouble(totalCalculado) * Convert.ToDouble(0.01)) / (Convert.ToDouble(1) - (Convert.ToDouble(1) / (Math.Pow((Convert.ToDouble(1) + 0.01), Convert.ToDouble(txtQtdParcelas.Text))))));
                        parcela.Valor = valorTemp;
                    }
                    else
                        parcela.Valor = valorParcela;
                }
                else
                {
                    parcela.Valor = valorParcela;
                }

                if (!chkIsentarJuros.Checked)
                {
                    if (Convert.ToInt32(txtQtdParcelas.Text) > 1)
                    {
                        if (saldoDevedor == totalCalculado)
                        {
                            //amortizacao = c.Valor - juros
                            parcela.JurosRS = totalCalculado * 0.1M;
                            saldoDevedor -= parcela.Valor - parcela.JurosRS;
                            parcela.Amortizacao = parcela.Valor - parcela.JurosRS;
                        }
                        else
                        {
                            parcela.JurosRS = saldoDevedor * 0.1M;
                            saldoDevedor -= parcela.Valor - parcela.JurosRS;
                            parcela.Amortizacao = parcela.Valor - parcela.JurosRS;
                        }
                    }
                    else
                    {
                        parcela.JurosRS = (totalCalculado - totalBruto);// / Convert.ToDecimal(txtQtdParcelas.Text);
                        parcela.MultaRS = multaParcela;
                        parcela.Amortizacao = parcela.Valor; // -parcela.JurosRS;
                    }
                }
                else
                {
                    parcela.JurosRS = 0;
                    parcela.Amortizacao = parcela.Valor;
                }

                parcelas.Add(parcela);
            }

            //iguala as parcelas /////////////////////////////////////////////////////////
            //Decimal totaltemp = 0;
            //foreach (Cobranca cobranca in parcelas) { totaltemp += cobranca.Valor; }
            //foreach (Cobranca cobranca in parcelas)
            //{
            //    cobranca.Valor = totaltemp / parcelas.Count;
            //}
            /////////////////////////////////////////////////////////////////////////////

            String obsCompetencias = "";
            for (int i = 0; i < cobrancas.Count; i++)
            {
                if (i == 0)
                {
                    obsCompetencias = "referente a(s) competencia(s): " + cobrancas[i].DataVencimento.ToString("MM/yyyy");
                }
                else
                {
                    if (i < cobrancas.Count - 1)
                    {
                        obsCompetencias += ", " + cobrancas[i].DataVencimento.ToString("MM/yyyy");
                    }
                    else
                    {
                        obsCompetencias += " e " + cobrancas[i].DataVencimento.ToString("MM/yyyy") + ".";
                    }
                }
            }

            for (int i = 0; i < parcelas.Count; i++)
            {
                parcelas[i].ItemParcelamentoOBS = String.Concat("Parcela ", i + 1, "/", parcelas.Count, " ", obsCompetencias);
            }

            this.parcelamento = parcelas;

            gridParcelamento.DataSource = parcelas;
            gridParcelamento.DataBind();
        }

        protected void cmdGravar_click(Object sender, EventArgs e)
        {
            if (!valida()) { return; }

            if (parcelamento == null || parcelamento.Count == 0)
            {
                base.Alerta(null, this, "_err", "Nenhum parcelamento foi configurado.");
                return; 
            }

            if (txtEmail.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err", "Informe o e-mail.");
                return; 
            }

            IList<Cobranca> _parcelamento = this.parcelamento;
            Decimal total = 0;

            PersistenceManager pm = new PersistenceManager();
            pm.IsoLevel = IsolationLevel.ReadUncommitted;
            pm.BeginTransactionContext();

            try
            {
                List<ParcelamentoItem> itens = new List<ParcelamentoItem>();

                for (int i = 0; i < gridParcelamento.Rows.Count; i++)
                {
                    total += _parcelamento[i].Valor; //c.Valor;

                    pm.Save(_parcelamento[i]);

                    ParcelamentoItem item = new ParcelamentoItem();
                    item.CobrancaID = _parcelamento[i].ID; //c.ID;
                    item.Observacoes = ((TextBox)gridParcelamento.Rows[i].Cells[3].FindControl("txtObsGrid")).Text;

                    //TODO: ler e validar demais itens editaveis no grid...
                    itens.Add(item);
                }


                Object empresaCobrancaId = LocatorHelper.Instance.ExecuteScalar("select contrato_empresaCobrancaId from contrato where contrato_id=" + this.contratoId, null, null, pm);

                if (empresaCobrancaId == null || empresaCobrancaId == DBNull.Value)
                {
                    base.Alerta(null, this, "_err", "Não há uma empresa de cobrança relacionada.");
                    pm.Rollback();
                    pm.Dispose();
                    return;
                }

                ParcelamentoHeader header = new ParcelamentoHeader();
                header.EmpresaID = empresaCobrancaId; //cboEmpresaFiltro.SelectedValue;
                header.ContratoID = this.contratoId;
                header.UsuarioID = Usuario.Autenticado.ID;
                header.OBS = txtObs.Text;
                header.Parcelas = Convert.ToInt32(txtQtdParcelas.Text);
                header.ValorTotal = total;
                header.IsentaJuros = chkIsentarJuros.Checked;
                header.VenctoInicial = _parcelamento[0].DataVencimento;
                header.Multa = cboMulta.SelectedItem.Text;
                header.Desconto = cboDesconto.SelectedItem.Text;
                header.IsentaJuros = chkIsentarJuros.Checked;
                header.Email = txtEmail.Text;
                pm.Save(header);

                foreach (ParcelamentoItem item in itens)
                {
                    item.HeaderID = header.ID;
                    pm.Save(item);
                }

                CobrancaBaixa baixa = null;
                Cobranca cobrancaOriginal = null;
                foreach (String cobrancaOriginalId in this.iDsSelecionados)
                {
                    ParcelamentoCobrancaOriginal pco = new ParcelamentoCobrancaOriginal();
                    pco.HeaderID = header.ID;
                    pco.CobrancaID = cobrancaOriginalId;
                    pm.Save(pco);

                    cobrancaOriginal = new Cobranca(cobrancaOriginalId);
                    pm.Load(cobrancaOriginal);
                    cobrancaOriginal.Pago = true;
                    cobrancaOriginal.DataPgto = DateTime.Now;
                    pm.Save(cobrancaOriginal);

                    baixa = CobrancaBaixa.CarregarUltima(cobrancaOriginalId, pm);
                    if (baixa == null)
                    {
                        baixa = new CobrancaBaixa();
                        baixa.BaixaFinanceira = false;
                        baixa.BaixaProvisoria = false;
                        baixa.CobrancaID = cobrancaOriginalId;
                        baixa.Data = cobrancaOriginal.DataPgto;
                        baixa.MotivoID = _motivoId; //parcela negociada
                        baixa.Obs = "Cobrança negociada.";
                        baixa.Tipo = 0; //baixa
                        baixa.UsuarioID = Usuario.Autenticado.ID;
                        pm.Save(baixa);
                    }
                }

                pm.Commit();
                base.Alerta(null, this, "_ok", "Parcelamento salvo com sucesso.");

                this.parcelamento = null;
                this.iDsSelecionados = null;

                gridParcelamento.DataSource = null;
                gridParcelamento.DataBind();

                txtObs.Text = "";
                gridCobranca.DataSource = null;
                gridCobranca.DataBind();

                pnlCobrancas.Visible = false;
                pnlEmails.Visible = true;
                this.exibirEnvioDeEmail(this.contratoId);
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

        protected void Button1_Click(Object sender, EventArgs e)
        {
            NonQueryHelper.Instance.ExecuteNonQuery("delete from cobranca where cobranca_tipo=4;truncate table cobranca_parcelamentoItem;truncate table cobranca_parcelamentoCobrancaOriginal;truncate table cobranca_parcelamentoHeader", null);
        }

        //-----------------------------------DETALHE-----------------------------------------------------//

        protected void gridEmail_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
        }

        protected void gridEmail_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
        }

        protected void gridEmail_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("email"))
            {
                Object cobrancaId = gridEmail.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Cobranca cobranca = new Cobranca(cobrancaId);
                cobranca.Carregar();

                String nossoNumero = cobranca.GeraNossoNumero();

                Contrato contrato = new Contrato(cobranca.PropostaID);
                contrato.Carregar();

                ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(this.contratoId, null);

                String instrucoes = String.Concat("<br>Este boleto é referente ao período de cobertura de ", cobranca.DataVencimento.Month, "/", cobranca.DataVencimento.Year);
                ParcelamentoItem item = ParcelamentoItem.CarregarPorCobrancaId(cobranca.ID);
                instrucoes = "<br>" + item.Observacoes;

                String naoReceber = "Não receber após o vencimento.";

                String uri = "";

                if ((contrato.ContratoADMID != null && Convert.ToInt32(contrato.ContratoADMID) >= Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"])))
                {
                    uri = EntityBase.RetiraAcentos(String.Concat("http://www.boletomail.com.br/do.php?nossonum=", nossoNumero, Cobranca.BoletoUrlCompQualicorp, "&valor=", cobranca.Valor, "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", cobranca.DataVencimento.Day, "&v_mes=", cobranca.DataVencimento.Month, "&v_ano=", cobranca.DataVencimento.Year, "&numdoc2=", contrato.Numero, "&nome=", titular.BeneficiarioNome, "&cod_cli=", contrato.ID, "&mailto=", txtEmailR.Text, "&instr=", instrucoes, "<br><br>" + naoReceber, "&user=qualicorp&action=5"));
                }
                else
                {
                    uri = EntityBase.RetiraAcentos(String.Concat("http://www.boletomail.com.br/do.php?nossonum=", nossoNumero, Cobranca.BoletoUrlCompPSPadrao, "&valor=", cobranca.Valor, "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", cobranca.DataVencimento.Day, "&v_mes=", cobranca.DataVencimento.Month, "&v_ano=", cobranca.DataVencimento.Year, "&numdoc2=", contrato.Numero, "&nome=", titular.BeneficiarioNome, "&cod_cli=", contrato.ID, "&mailto=", txtEmailR.Text, "&instr=", instrucoes, "<br><br>" + naoReceber, "&user=qualicorp&action=5"));
                }

                System.Net.WebRequest request = System.Net.WebRequest.Create(uri);
                System.Net.WebResponse response = request.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
                String finalUrl = sr.ReadToEnd();
                sr.Close();
                response.Close();

                ScriptManager.RegisterClientScriptBlock(this,
                    this.GetType(),
                    "_geraBoleto_",
                    String.Concat(" window.open(\"", finalUrl, "\", \"janela\", \"toolbar=no,scrollbars=1,width=860,height=420\"); "),
                    true);
            }
        }

        protected void cmdCancelarParcelamento_click(Object sender, EventArgs e)
        {
            IList<Cobranca> parcelamento = this.CarregarCobrancasTipoParcelamento();
            if (parcelamento != null)
            {
                ParcelamentoItem parcItem = ParcelamentoItem.CarregarPorCobrancaId(parcelamento[0].ID);
                ParcelamentoHeader header = new ParcelamentoHeader(parcItem.HeaderID);
                header.Carregar();

                IList<Cobranca> cobrancasGeradas = ParcelamentoItem.CarregarParcelasGeradas(header.ID);
                if (cobrancasGeradas != null)
                {
                    foreach (Cobranca cobrancaGerada in cobrancasGeradas)
                    {
                        if (cobrancaGerada.Pago)
                        {
                            base.Alerta(null, this, "_err", "Já existe(m) cobrança(s) do parcelamento paga(s).");
                            return;
                        }
                    }
                }

                PersistenceManager pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();

                try
                {
                    //deleta as novas cobrancas geradas (tipo 4 - parcelamento)
                    foreach (Cobranca cobrancaGerada in cobrancasGeradas) pm.Remove(cobrancaGerada);

                    //deleta os itens de cobranca original, restaura as parcelas originais, desfaz as baixas
                    Cobranca cobranca = null;
                    IList<ParcelamentoCobrancaOriginal> parcOriginais = ParcelamentoCobrancaOriginal.CarregarPorHeaderId(header.ID, pm);
                    foreach (ParcelamentoCobrancaOriginal parcOriginal in parcOriginais)
                    { 
                        pm.Remove(parcOriginal);

                        cobranca = new Cobranca(parcOriginal.CobrancaID);
                        pm.Load(cobranca);
                        cobranca.Pago = false;
                        cobranca.DataPgto = DateTime.MinValue;
                        pm.Save(cobranca);

                        CobrancaBaixa.Remover(cobranca.ID, _motivoId, pm);
                    }

                    //deleta os itens de cobranca gerada
                    IList<ParcelamentoItem> parcItens = ParcelamentoItem.CarregarPorHeaderId(header.ID, pm);
                    foreach (ParcelamentoItem _parcItem in parcItens) { pm.Remove(_parcItem); }

                    //deleta o cabecalho
                    pm.Remove(header);

                    pm.Commit();
                }
                catch
                {
                    pm.Rollback();
                    base.Alerta(null, this, "_err", "Erro inesperado.");
                }
                finally
                {
                    pm.Dispose();
                }

                //Limpa os campos
                txtVenctoR.Text = "";
                txtQtdParcelasR.Text = "";
                chkIsentarJurosR.Checked = false;
                cboMultaR.Text = "";
                cboDescontoR.Text = "";
                txtEmailR.Text = "";
                txtObsR.Text = "";
                gridEmail.DataSource = null;
                gridEmail.DataBind();

                base.Alerta(null, this, "_err", "Parcelamento cancelado com sucesso.");
            }
        }
    }
}
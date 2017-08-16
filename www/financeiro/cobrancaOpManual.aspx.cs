namespace www.financeiro
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;
    using System.IO;
    using System.Text;
    using System.Configuration;

    public partial class cobrancaOpManual : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            txtParcela.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");
            txtValor.Attributes.Add("onKeyUp", "mascara('" + txtValor.ClientID + "')");
            txtValorPagto.Attributes.Add("onKeyUp", "mascara('" + txtValorPagto.ClientID + "')");

            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, true);
                this.CarregaMotivosBaixa();
                txtNumContrato.Focus();
            }
        }

        protected void opt_changed(Object sender, EventArgs e)
        {
            if (optBuscaPorContrato.Checked)
            {
                pnlBuscaPorNome.Visible = false;
                pnlBuscaPorContrato.Visible = true;
                pnlBuscaPorNossoNumero.Visible = false;
            }
            else if (optBuscaPorNossoNumero.Checked)
            {
                pnlBuscaPorNome.Visible = false;
                pnlBuscaPorContrato.Visible = false;
                pnlBuscaPorNossoNumero.Visible = true;
            }
            else
            {
                pnlBuscaPorNome.Visible = true;
                pnlBuscaPorContrato.Visible = false;
                pnlBuscaPorNossoNumero.Visible = false;
            }
        }

        void CarregaMotivosBaixa()
        {
            IList<CobrancaMotivoBaixa> list = CobrancaMotivoBaixa.CarregarTodos();
            cboMotivo.DataValueField = "ID";
            cboMotivo.DataTextField = "Descricao";
            cboMotivo.DataSource = list;
            cboMotivo.DataBind();
        }

        void CarregaCobrancas()
        {
            #region validacoes 

            if (txtNumContrato.Text.Trim() == "")
            {
                base.Alerta(null, this, "__errNumContr", "Informe o número do contrato.");
                txtNumContrato.Focus();
                return;
            }
            if (!base.HaItemSelecionado(cboOperadora))
            {
                base.Alerta(null, this, "__errOper", "Não há uma operadora selecionada.");
                cboOperadora.Focus();
                return;
            }
            #endregion
        }

        void LimpaCampos()
        {
            txtParcela.Text         = "";
            txtDataVencimento.Text  = "";
            txtValor.Text           = "";
            txtDataPgto.Text        = "";
            txtValorPagto.Text      = "";
            cboTipo.SelectedIndex   = 0;
            chkCancelada.Checked    = false;
        }

        void AbreDetalhe()
        {
            txtNumContrato.Enabled = false;
            cboOperadora.Enabled = false;
            pnlDetalhe.Visible = true;
            litMsg.Text = "";
            pnlResultado.Visible = false;
            txtParcela.Focus();
        }

        void FechaDetalhe()
        {
            ViewState[IDKey] = null;
            txtNumContrato.Enabled = true;
            cboOperadora.Enabled = true;
            pnlDetalhe.Visible = false;
            litMsg.Text = "";
            pnlResultado.Visible = true;

            this.FechaComboParaDupla();
        }

        void FechaComboParaDupla()
        {
            cboCobrancaReferente.Items.Clear();
            trDupla.Visible = false;
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            #region validacoes

            pnlIntermediario_BuscaPorNome.Visible = false;
            String operadoraId = null;

            if (String.IsNullOrEmpty(txtNumContrato.Text))
            {
                base.Alerta(null, this, "__err1", "Informe o número do contrato.");
                txtNumContrato.Focus();
                return;
            }

            if (!base.HaItemSelecionado(cboOperadora))
            {
                //base.Alerta(null, this, "__err2", "Não há uma operadora selecionada.");
                //cboOperadora.Focus();
                //return;
            }
            else
                operadoraId = cboOperadora.SelectedValue;

            IList<Contrato> contratos = null;
            if (operadoraId != null)
                contratos = Contrato.CarregarPorOperadoraID(operadoraId, txtNumContrato.Text, null);
            else
                contratos = Contrato.CarregarPorNumero(txtNumContrato.Text, null);

            if (contratos == null)
            {
                base.Alerta(null, this, "__err1", "Nenhum contrato localizado.");
                txtNumContrato.Focus();
                return;
            }
            else if (contratos.Count != 1)
            {
                base.Alerta(null, this, "__err1", "Para este número de contrato, você deverá informar uma operadora.");
                txtNumContrato.Focus();
                return;
            }

            #endregion

            this.FechaDetalhe();
            IList<Cobranca> cobrancas = Cobranca.CarregarPorNumeroDeContrato(operadoraId, txtNumContrato.Text, null);
            grid.DataSource = cobrancas;
            grid.DataBind();

            if (cobrancas != null)
            {
                litMsg.Text = "Resultado:";
                pnlResultado.Visible = true;
            }
            else
            {
                pnlResultado.Visible = false;
                litMsg.Text = "Nenhuma cobrança encontrada.";
            }
        }

        protected void cmdLocalizar2_Click(Object sender, EventArgs e)
        {
            #region validacoes

            pnlIntermediario_BuscaPorNome.Visible = false;

            if (String.IsNullOrEmpty(txtNossoNumero.Text.Trim()))
            {
                base.Alerta(null, this, "__err1", "Informe o 'nosso número'.");
                txtNumContrato.Focus();
                return;
            }

            #endregion

            this.FechaDetalhe();
            IList<Cobranca> cobrancas = Cobranca.CarregarPorID(txtNossoNumero.Text, null);

            if (cobrancas != null)
            {
                grid.DataSource = cobrancas;
                grid.DataBind();

                litMsg.Text = "Resultado:";
                pnlResultado.Visible = true;

                Contrato contrato = new Contrato(cobrancas[0].PropostaID);
                contrato.Carregar();
                txtNumContrato.Text = contrato.Numero;
                //cboOperadora.SelectedValue = Convert.ToString(contrato.OperadoraID);
            }
            else
            {
                Cobranca cobranca = new Cobranca();
                cobranca.LeNossoNumeroUNIBANCO(txtNossoNumero.Text);

                if (cobranca.Parcela == -1)
                {
                    base.Alerta(null, this, "__err1", "Nosso número inválido.");
                    pnlResultado.Visible = false;
                    litMsg.Text = "Nosso número inválido.";
                    return;
                }

                Contrato contrato = Contrato.CarregarParcialPorCodCobranca(cobranca.ContratoCodCobranca, null);
                if (contrato == null)
                {
                    base.Alerta(null, this, "__err1", "Contrato não localizado.");
                    pnlResultado.Visible = false;
                    litMsg.Text = "Contrato não localizado.";
                    return;
                }

                txtNumContrato.Text = contrato.Numero;
                //cboOperadora.SelectedValue = Convert.ToString(contrato.OperadoraID);

                cobranca = Cobranca.CarregarPor(cobranca.PropostaID, cobranca.Parcela, cobranca.Tipo, null);
                if (cobranca == null)
                {
                    base.Alerta(null, this, "__err1", "Cobrança não localizada.");
                    pnlResultado.Visible = false;
                    litMsg.Text = "Cobrança não localizada.";
                    return;
                }

                List<Cobranca> _cobrancas = new List<Cobranca>();
                _cobrancas.Add(cobranca);

                grid.DataSource = _cobrancas;
                grid.DataBind();

                litMsg.Text = "Resultado:";
                pnlResultado.Visible = true;
            }
        }

        protected void cmdLocalizarPorNome_click(Object sender, EventArgs e)
        {
            #region validacoes

            litMsg.Text = "";

            int? operadoraId = null;
            litMsg.Text = "";

            if (String.IsNullOrEmpty(txtNomeBenefciario.Text))
            {
                base.Alerta(null, this, "__err1", "Informe o nome do beneficiário.");
                txtNomeBenefciario.Focus();
                return;
            }

            if (!base.HaItemSelecionado(cboOperadora))
            {
                //base.Alerta(null, this, "__err2", "Não há uma operadora selecionada.");
                //cboOperadora.Focus();
                //return;
            }
            else
                operadoraId = Convert.ToInt32(cboOperadora.SelectedValue);


            IList<Contrato> contratos = Contrato.CarregarPorParametros(operadoraId, txtNomeBenefciario.Text);

            if (contratos == null)
            {
                base.Alerta(null, this, "__err1", "Nenhum contrato localizado.");
                txtNumContrato.Focus();
                return;
            }
            else if(contratos.Count > 0)
            {
                pnlIntermediario_BuscaPorNome.Visible = true;
                gridIntermediario.DataSource = contratos;
                gridIntermediario.DataBind();

                this.FechaDetalhe();
                pnlResultado.Visible = false;
                return;
            }

            #endregion

            //this.FechaDetalhe();
            //IList<Cobranca> cobrancas = Cobranca.CarregarPorNomeBeneficiario(operadoraId, txtNumContrato.Text, null);
            //grid.DataSource = cobrancas;
            //grid.DataBind();
        }

        protected void gridIntermediario_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selecionar"))
            {
                this.FechaDetalhe();

                Int32 index = Convert.ToInt32(e.CommandArgument);
                Object contratoId = gridIntermediario.DataKeys[index].Value;

                IList<Cobranca> cobrancas = Cobranca.CarregarTodas(contratoId);
                grid.DataSource = cobrancas;
                grid.DataBind();

                if (cobrancas != null)
                {
                    litMsg.Text = "Resultado:";
                    pnlResultado.Visible = true;
                }
                else
                {
                    pnlResultado.Visible = false;
                    litMsg.Text = "Nenhuma cobrança encontrada.";
                }

                pnlIntermediario_BuscaPorNome.Visible = false;
                gridIntermediario.DataSource = null;
                gridIntermediario.DataBind();
            }
        }

        /************************************************************************************/

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                cboTipo.Items.Clear();
                cboTipo.Items.Add(new ListItem("Normal", "0"));
                cboTipo.Items.Add(new ListItem("Complementar", "1"));
                cboTipo.Items.Add(new ListItem("Dupla", "2"));
                cboTipo.Items.Add(new ListItem("Especial", "3"));
                cboTipo.Items.Add(new ListItem("Parcelamento", "4"));
                cboTipo.Items.Add(new ListItem("Diferença UBRASP", "5"));
                cboTipo.Items.Add(new ListItem("Boleto UBRASP", "6"));

                Int32 index = Convert.ToInt32(e.CommandArgument);
                Object id = grid.DataKeys[index].Value;
                Cobranca cobranca = new Cobranca(id);
                cobranca.Carregar();

                tblLog.Visible = true;
                //CobrancaBaixa baixa = CobrancaBaixa.CarregarUltima(cobranca.ID);
                //if (baixa != null)
                //{
                //    chkBaixaFinanceira.Checked = baixa.BaixaFinanceira;
                //    baixa.Data = DateTime.Now;
                //    cboMotivo.SelectedValue = Convert.ToString(baixa.MotivoID);
                //    cboTipoBaixa.SelectedValue = baixa.Tipo.ToString();
                //    txtObs.Text = baixa.Obs;
                //}

                this.AbreDetalhe();
                this.LimpaCampos();

                ViewState[IDKey] = id;
                txtParcela.Text = cobranca.Parcela.ToString();
                txtDataVencimento.Text = cobranca.DataVencimento.ToString("dd/MM/yyyy");
                txtValor.Text = cobranca.Valor.ToString("N2");

                chkCancelada.Checked = cobranca.Cancelada;

                cboTipo.SelectedValue = cobranca.Tipo.ToString();
                cboTipo_Changed(null, null);

                if (cboTipo.SelectedValue == "3") { cboTipo.Enabled = false; }
                else { cboTipo.Enabled = true; }

                if (trDupla.Visible && cobranca.CobrancaRefID != null)
                {
                    cboCobrancaReferente.SelectedValue = Convert.ToString(cobranca.CobrancaRefID);
                }

                if (cobranca.DataPgto != DateTime.MinValue)
                {
                    txtValorPagto.Text = cobranca.ValorPgto.ToString("N2");
                    txtDataPgto.Text = cobranca.DataPgto.ToString("dd/MM/yyyy");
                }

                CobrancaBaixa baixa = CobrancaBaixa.CarregarUltima(cobranca.ID);
                if (baixa != null)
                {
                    chkBaixaFinanceira.Checked = baixa.BaixaFinanceira;
                    chkBaixaProvisoria.Checked = baixa.BaixaProvisoria;
                    //baixa.Data = DateTime.Now;
                    cboMotivo.SelectedValue = Convert.ToString(baixa.MotivoID);
                    cboTipoBaixa.SelectedValue = baixa.Tipo.ToString();
                    txtObs.Text = baixa.Obs;
                }
            }
            else if (e.CommandName == "cnab")
            {
                Int32 index = Convert.ToInt32(e.CommandArgument);
                string id = Convert.ToString(grid.DataKeys[index].Value);

                string ret = ArquivoCobrancaUnibanco.GeraDocumentoCobranca_SANTANDER(id, null);

                StringWriter oStringWriter = new StringWriter();
                oStringWriter.Write(ret);
                Response.ContentType = "text/plain";

                Response.AddHeader("content-disposition", "attachment;filename=" + string.Format("remessa-{0}.dat", string.Format("{0:ddMMyyyyHHmmss}", DateTime.Now)));
                Response.Clear();

                using (StreamWriter writer = new StreamWriter(Response.OutputStream, Encoding.ASCII))
                {
                    writer.Write(oStringWriter.ToString());
                }
                Response.End();
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Convert.ToBoolean(grid.DataKeys[e.Row.RowIndex][1]))
                    e.Row.ForeColor = System.Drawing.Color.Red;
                else
                    e.Row.ForeColor = System.Drawing.Color.Black;

                //LinkButton btn = e.Row.Cells[10].Controls[0] as LinkButton;
                //((layout)this.Master).SM.RegisterAsyncPostBackControl(btn);
            }
        }

        protected void grid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //LinkButton btn = e.Row.Cells[10].Controls[0] as LinkButton;

                //AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
                //trigger.ControlID = btn.ClientID;
                //trigger.EventName = "Click";
                //up.Triggers.Add(trigger);

                LinkButton lnkCNAB = (LinkButton)e.Row.FindControl("lnkCnab");
                lnkCNAB.CommandArgument = e.Row.RowIndex.ToString();
                ScriptManager.GetCurrent(this).RegisterPostBackControl(lnkCNAB);
            }
        }

        protected void cmdNovaCobranca_Click(Object sender, EventArgs e)
        {
            #region validacoes

            if (String.IsNullOrEmpty(txtNumContrato.Text))
            {
                base.Alerta(null, this, "__err1", "Informe o número do contrato.");
                txtNumContrato.Focus();
                return;
            }

            if (!base.HaItemSelecionado(cboOperadora))
            {
                base.Alerta(null, this, "__err2", "Não há uma operadora selecionada.");
                cboOperadora.Focus();
                return;
            }

            #endregion

            tblLog.Visible = false;

            cboTipo.Enabled = true;
            cboTipo.Items.Clear();
            cboTipo.Items.Add(new ListItem("Normal", "0"));
            cboTipo.Items.Add(new ListItem("Dupla", "2"));
            cboTipo.Items.Add(new ListItem("Complementar", "1"));
            cboTipo.Items.Add(new ListItem("Diferença UBRASP", "5"));
            cboTipo.Items.Add(new ListItem("Boleto UBRASP", "6"));

            this.LimpaCampos();
            this.AbreDetalhe();
        }

        //TODO: em vez de usar um grid, usar uma nova tr com um dropdow com text= parcela (valor), ja que a selecao 
        //      é unica.
        protected void cboTipo_Changed(Object sender, EventArgs e)
        {
            if (((Cobranca.eTipo)Convert.ToInt32(cboTipo.SelectedValue)) == Cobranca.eTipo.Dupla) //se é cobranca dupla
            {
                IList<Cobranca> cobrancas = Cobranca.CarregarPorNumeroDeContrato(cboOperadora.SelectedValue, txtNumContrato.Text, null);
                if (cobrancas == null || cobrancas.Count == 0) { cboTipo.SelectedValue = "0"; return; }

                List<Cobranca> selecionaveis = new List<Cobranca>();

                foreach (Cobranca cobranca in cobrancas)
                {
                    if (cobranca.Pago || cobranca.ID.ToString() == Convert.ToString(ViewState[IDKey]) ) { continue; }
                    selecionaveis.Add(cobranca);
                }

                trDupla.Visible = true;
                cboCobrancaReferente.Items.Clear();
                foreach (Cobranca cob in selecionaveis)
                {
                    cboCobrancaReferente.Items.Add(new ListItem(String.Concat("Parcela ", cob.Parcela.ToString(), " (", cob.Valor.ToString("C") + ")"), cob.ID.ToString()));
                }
            }
            else
            {
                this.FechaComboParaDupla();
            }

            txtDataVencimento.Focus();
        }

        protected void cmdFechar_Click(Object sender, EventArgs e)
        {
            this.FechaDetalhe();
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if(txtParcela.Text.Trim() == "")
            {
                base.Alerta(null,this, "__err1", "Informe a parcela da cobrança.");
                txtParcela.Focus();
                return;
            }

            DateTime dataVencimento = DateTime.MinValue;
            if (!UIHelper.TyParseToDateTime(txtDataVencimento.Text, out dataVencimento))
            {
                base.Alerta(null, this, "__err2", "Data de vencimento inválida.");
                txtDataVencimento.Focus();
                return;
            }

            if (txtValor.Text.Trim() == "" || base.CToDecimal(txtValor.Text) == Decimal.Zero)
            {
                base.Alerta(null, this, "__err3", "Informe o valor da cobrança.");
                txtValor.Focus();
                return;
            }

            DateTime dataPgto = DateTime.MinValue;
            if (txtDataPgto.Text.Trim() != "")
            {
                if (!UIHelper.TyParseToDateTime(txtDataPgto.Text, out dataPgto))
                {
                    base.Alerta(null, this, "__err2", "Data de pagamento inválida.");
                    txtDataPgto.Focus();
                    return;
                }

                if (txtValorPagto.Text.Trim() == "" || Convert.ToDouble(txtValorPagto.Text) == 0)
                {
                    base.Alerta(null, this, "__err3", "Informe o valor de pagamento.");
                    txtValorPagto.Focus();
                    return;
                }
            }

            if (cboCobrancaReferente.Items.Count == 0 && cboTipo.SelectedValue == "2")
            {
                //cobranca dupla, mas nao tem nenhuma outra cobranca
                base.Alerta(null, this, "__err4", "Não é possível criar uma cobrança dupla.");
                cboTipo.Focus();
                return;
            }

            if (tblLog.Visible && cboMotivo.Items.Count == 0)
            {
                base.Alerta(null, this, "__err5", "O motivo da baixa deve ser especificado.");
                return;
            }

            if (tblLog.Visible && txtObs.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err6", "O campo \"Obs.\" é obrigatório.");
                txtObs.Focus();
                return;
            }

            #endregion

            Cobranca cobranca = new Cobranca();

            if (ViewState[IDKey] != null)
            {
                cobranca.ID = ViewState[IDKey];
                cobranca.Carregar();

                if (chkReenvioBanco.Checked) cobranca.ArquivoIDUltimoEnvio = null;
            }

            cobranca.Parcela = Convert.ToInt32(txtParcela.Text);
            cobranca.DataVencimento = new DateTime(dataVencimento.Year, dataVencimento.Month, dataVencimento.Day, 23, 59, 59, 789);
            cobranca.Valor = base.CToDecimal(txtValor.Text);
            cobranca.Tipo = Convert.ToInt32(cboTipo.SelectedValue);

            if (((Cobranca.eTipo)cobranca.Tipo) == Cobranca.eTipo.Dupla)
                cobranca.CobrancaRefID = cboCobrancaReferente.SelectedValue;
            else
                cobranca.CobrancaRefID = null;

            if (dataPgto != DateTime.MinValue)
            {
                cobranca.DataPgto = dataPgto;
                cobranca.ValorPgto = base.CToDecimal(txtValorPagto.Text);
                cobranca.Pago = true;
            }
            else
            {
                cobranca.DataPgto = DateTime.MinValue;
                cobranca.ValorPgto = Decimal.Zero;
                cobranca.Pago = false;
            }

            if (cobranca.ID == null) //se é uma nova cobranca...
            {
                Contrato contrato = Contrato.CarregarParcial(txtNumContrato.Text, cboOperadora.SelectedValue);
                if (contrato == null) { return; }
                cobranca.PropostaID = contrato.ID;
            }

            cobranca.Cancelada = chkCancelada.Checked;

            cobranca.Salvar();

            if (tblLog.Visible && cboMotivo.Items.Count > 0)
            {
                CobrancaBaixa baixa = new CobrancaBaixa();
                baixa.BaixaFinanceira = chkBaixaFinanceira.Checked;
                baixa.BaixaProvisoria = chkBaixaProvisoria.Checked;
                baixa.CobrancaID = cobranca.ID;
                baixa.Data = DateTime.Now;
                baixa.MotivoID = cboMotivo.SelectedValue;
                baixa.Tipo = Convert.ToInt32(cboTipoBaixa.SelectedValue);
                baixa.UsuarioID = Usuario.Autenticado.ID;
                baixa.Obs = txtObs.Text;
                baixa.Salvar();
            }

            this.FechaDetalhe();
            cmdLocalizar_Click(null, null);
        }

        protected void gridCobrancasParaDupla_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                RadioButton rd = (RadioButton)e.Row.Cells[0].Controls[1];
                rd.GroupName = "group";
            }
        }
    }
}
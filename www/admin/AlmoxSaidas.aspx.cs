namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class AlmoxSaidas : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cmdSalvar.Attributes.Add("onClick", "return confirm('Deseja realmente prosseguir?');");

            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);
                cboOperadora.Items.Insert(0, new ListItem("<sem vínculo>", "-1"));
                cboOperadora.Items.Insert(0, new ListItem("<TODAS>", "0"));

                base.ExibirFiliais(cboFilial, false);
                cboFilial.Items.Insert(0, new ListItem("<TODAS>", "-1"));

                //cboProduto.Items.Insert(0, new ListItem("NENHUM FILTRO", "-1"));
                trObs.Visible = false;
                this.CarregaMotivos();
                //this.CarregaCorretores();
                base.ExibirTiposDeProdutos(cboTipo, true, "<TODOS>");
                this.CarregaEntradas();
            }
        }

        #region CarregaMotivos 

        void CarregaMotivos()
        {
            cboMotivo.Items.Clear();
            cboMotivo.Items.Add(new ListItem("Retirada de funcionário", "1"));
            //cboMotivo.Items.Add(new ListItem("Perda de material", "2"));
            //cboMotivo.Items.Add(new ListItem("Devolução de material", "3"));
            cboMotivo.Items.Add(new ListItem("Perda de material", "5"));
            cboMotivo.Items.Add(new ListItem("Outros não relacionados", "4"));
        }
        #endregion

        #region CarregaCorretores 

        //void CarregaCorretores()
        //{
        //    cboCorretores.DataValueField = "ID";
        //    cboCorretores.DataTextField = "Nome";
        //    cboCorretores.DataSource = Usuario.CarregarCorretores();
        //    cboCorretores.DataBind();
        //}

        #endregion

        void CarregaEntradas()
        {
            //if (cboTipo.SelectedValue == "-1")
            //    gridEntradas.DataSource = AlmoxMovimentacao.CarregarTodos(AlmoxMovimentacao.TipoMovimentacao.Entrada);
            //else
            //{
            //    Object prodId = null;
            //    if (cboProduto.SelectedIndex > 0) { prodId = cboProduto.SelectedValue; }
            //    gridEntradas.DataSource = AlmoxMovimentacao.CarregarTodos(AlmoxMovimentacao.TipoMovimentacao.Entrada, cboTipo.SelectedValue, prodId);
            //}
            gridEntradas.DataSource = AlmoxMovimentacao.CarregarTodasEntradas(cboOperadora.SelectedValue, cboFilial.SelectedValue, cboTipo.SelectedValue);
            gridEntradas.DataBind();
        }

        protected void gridEntradas_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("acao"))
            {
                ViewState[IDKey3] = gridEntradas.DataKeys[Convert.ToInt32(e.CommandArgument)][0]; //ID da movimentacao de entrada
                Object produtoId = gridEntradas.DataKeys[Convert.ToInt32(e.CommandArgument)][1];
                this.CarregaTelaDeSaida(produtoId);

                //MPE.X = 192;
                //MPE.Y = 315;
                //MPE.Show();
                pnl.Visible = true;
                cboMotivo.SelectedIndex = 0;
                this.GerenciaCombosDeTipo();
            }
            else if (e.CommandName.Equals("detalhes"))
            {
                Object prodid = gridEntradas.DataKeys[Convert.ToInt32(e.CommandArgument)][1];
                AlmoxProduto prod = new AlmoxProduto(prodid);
                prod.Carregar();
                AlmoxTipoProduto tipo = new AlmoxTipoProduto(prod.TipoProdutoID);
                tipo.Carregar();
                Object id = gridEntradas.DataKeys[Convert.ToInt32(e.CommandArgument)][0];

                if (!tipo.Numerado)
                {
                    IList<ProdutoCorretor> lista = ProdutoCorretor.Carregar(id);
                    gridDetalhes.DataSource = ProdutoCorretor.Carregar(id);
                    gridDetalhes.Columns[3].Visible = false;
                    gridDetalhes.Columns[4].Visible = false;
                }
                else
                {
                    gridDetalhes.DataSource = AlmoxContratoImpresso.CarregarRetiradas(id);
                    gridDetalhes.Columns[3].Visible = true;
                    gridDetalhes.Columns[4].Visible = true;
                }

                gridDetalhes.DataBind();
                tblDetalhe.Visible = true;
                gridEntradas.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                if (gridEntradas.Rows.Count == 0) { litHistorico.Text = "&nbsp;(nenhuma)"; }
                else { litHistorico.Text = ""; }
            }
        }

        protected void gridEntradas_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[2].Text.Trim() != "" &&
                   Convert.ToInt32(e.Row.Cells[2].Text) <= 0)
                {
                    ((LinkButton)e.Row.Cells[6].Controls[0]).Visible = false;
                }
            }
        }

        protected void gridDetalhes_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("rasurar"))
            {
                Object id = gridDetalhes.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                Boolean rasurado = UIHelper.CToBool(gridDetalhes.DataKeys[Convert.ToInt32(e.CommandArgument)][1]);

                if (!rasurado)
                {
                    AlmoxContratoImpresso aci = new AlmoxContratoImpresso(id);
                    aci.Carregar();

                    AlmoxProduto prod = new AlmoxProduto(aci.ProdutoID);
                    prod.Carregar();

                    if (Contrato.NumeroDeContratoEmUso(aci.Numero.ToString(), aci.Letra, aci.QtdZerosAEsquerda, prod.OperadoraID, null))
                    {
                        base.Alerta(null, this, "_errRas", "Contrato já vendido.");
                        return;
                    }
                    else
                        AlmoxContratoImpresso.RasuraProposta(id);
                }
                else
                    AlmoxContratoImpresso.DESrasuraProposta(id);

                if (gridEntradas.SelectedIndex == -1)
                    cmdFecharDetalhe_Click(null, null);
                else
                {
                    id = gridEntradas.DataKeys[gridEntradas.SelectedIndex][0];
                    gridDetalhes.DataSource = AlmoxContratoImpresso.CarregarRetiradas(id);
                    gridDetalhes.DataBind();
                }
            }
        }

        protected void gridDetalhes_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente marcar a proposta como rasurada?");
                Boolean rasurado = UIHelper.CToBool(gridDetalhes.DataKeys[e.Row.RowIndex][1]);

                if (rasurado)
                {
                    base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente DESMARCAR a proposta como rasurada?");
                    e.Row.ForeColor = System.Drawing.Color.Red;
                    ((LinkButton)e.Row.Cells[4].Controls[0]).Text = "<img src='../images/active.png' border='0' title='desmarcar como rasurada' />";
                }
                else
                {
                    base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente MARCAR a proposta como rasurada?");
                    e.Row.ForeColor = System.Drawing.Color.Black;
                }
            }
        }

        protected void cmdFecharDetalhe_Click(Object sender, EventArgs e)
        {
            gridDetalhes.DataSource = null;
            gridDetalhes.DataBind();
            tblDetalhe.Visible = false;
            gridEntradas.SelectedIndex = -1;
        }

        void CarregaTelaDeSaida(Object produtoId)
        {
            AlmoxProduto prod = new AlmoxProduto();
            prod.ID = produtoId;
            prod.Carregar();

            ViewState[IDKey] = prod.ID;
            lblProdutoCarregado.Text = prod.Descricao;

            AlmoxTipoProduto tipo = new AlmoxTipoProduto();
            tipo.ID = prod.TipoProdutoID;
            tipo.Carregar();

            ViewState[IDKey2] = tipo.ID;

            if (tipo.Numerado)
            {
                //exibeCamposDeNumeracao = true;
                trNumeracao.Visible = true;
                txtNumDe.AutoPostBack = true;
            }
            else
            {
                //exibeCamposDeNumeracao = false;
                trNumeracao.Visible = false;
                txtNumDe.AutoPostBack = false;
            }
        }

        protected void cmdCancelar_Click(Object sender, EventArgs e)
        {
            pnl.Visible = false;// MPE.Hide();
            this.LimbaCampos();
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region Validacoes 

            if (txtQtd.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err0", "Informe a quantidade.");
                pnl.Visible = true; //MPE.Show();
                txtQtd.Focus();
                return;
            }

            if (cboMotivo.SelectedValue == "1" && txtCorretorIDSearch.Value.Trim() == "") //retirada de funcionário e sem corretor
            {
                base.Alerta(null, this, "_err1", "Não há um usuário selecionado.");
                // MPE.Show();
                txtCorretorSearch.Focus();
                return;
            }
            else if ((cboMotivo.SelectedValue == "4"  || cboMotivo.SelectedValue == "5") && txtObs.Text.Trim() == "") //outros e nenhuma obs
            {
                base.Alerta(null, this, "_err1a", "Você deve informar uma observação.");
                //MPE.Show();
                txtObs.Focus();
                return;
            }

            if (trNumeracao.Visible && (txtNumDe.Text.Trim() == "" || txtNumAte.Text.Trim() == ""))
            {
                base.Alerta(null, this, "_err2", "Não há uma numeração informada.");
                //MPE.Show();
                txtQtd.Focus();
                return;
            }

            AlmoxMovimentacao test = new AlmoxMovimentacao();
            test.ID = ViewState[IDKey3];
            test.Carregar();

            AlmoxTipoProduto tipo = new AlmoxTipoProduto();
            tipo.ID = ViewState[IDKey2];
            tipo.Carregar();

            ///////////////////////////
            String letra = "";
            int qtdZeros = 0, intNumDe = 0, intNumAte = 0;

            if (UIHelper.PrimeiraPosicaoELetra(txtNumDe.Text))
            {
                letra = txtNumDe.Text.Substring(0, 1);

                qtdZeros = txtNumAte.Text.Replace(letra, "").Length;
                intNumDe = base.CToInt(txtNumDe.Text.Replace(letra, ""));
                intNumAte = base.CToInt(txtNumAte.Text.Replace(letra, ""));
            }
            else
            {
                try
                {
                    intNumDe = base.CToInt(txtNumDe.Text);
                    intNumAte = base.CToInt(txtNumAte.Text);
                }
                catch
                {
                    Alerta(null, this, "_formErr", "Formato de dados inválido.");
                    return;
                }
            }

            if (tipo.Numerado && Convert.ToInt32(txtQtd.Text) > test.QTDFlutuante)
            {
                base.Alerta(null, this, "_err3", "Quantidade indisponível no lote.");
                //MPE.Show();
                txtQtd.Focus();
                return;
            }
            else
            {
                AlmoxProduto prod = new AlmoxProduto();
                prod.ID = ViewState[IDKey];
                prod.Carregar();

                if (!Usuario.TemVinculoComOperadora(prod.OperadoraID, txtCorretorIDSearch.Value))
                {
                    base.Alerta(null, this, "_err4", "O corretor não possui vínculo com a operadora.");
                    return;
                }

                if (Convert.ToInt32(txtQtd.Text) > prod.QTD)
                {
                    base.Alerta(null, this, "__errI", "Quantidade superior à disponível em estoque.");
                    //MPE.Show();
                    return;
                }
            }

            if (AlmoxMovimentacao.ExisteIntervaloRetirado(intNumDe, intNumAte, ViewState[IDKey], qtdZeros, letra))
            {
                base.Alerta(null, this, "__errJaRet", "Há contratos já retirados do estoque no intervalo informado.");
                //MPE.Show();
                return;
            }

            #endregion

            AlmoxMovimentacao mov = new AlmoxMovimentacao();
            mov.MovimentacaoID = ViewState[IDKey3];
            if (trNumeracao.Visible)
            {
                mov.NumeracaoInicial = intNumDe;
                mov.NumeracaoFinal   = intNumAte;
            }

            mov.ProdutoID = ViewState[IDKey];
            mov.QTD = Convert.ToInt32(txtQtd.Text);
            mov.UsuarioID = Usuario.Autenticado.ID;
            if (cboMotivo.SelectedValue == "1") //retirada de funcionário
            {
                mov.UsuarioRetiradaID = txtCorretorIDSearch.Value;
            }
            else if(cboMotivo.SelectedValue == "4")
            {
                mov.SubTipoDeMovimentacao = (int)AlmoxMovimentacao.SubTipoMovimentacao.Outros;
                mov.Obs = txtObs.Text;
            }
            else if (cboMotivo.SelectedValue == "5")
            {
                mov.SubTipoDeMovimentacao = (int)AlmoxMovimentacao.SubTipoMovimentacao.Perda;
                mov.Obs = txtObs.Text;
            }

            mov.Letra = letra;
            mov.QtdZerosAEsquerda = qtdZeros;

            String msg = "";
            Boolean result = AlmoxMovimentacaoFacade.Instance.SalvarSaida(mov, ref msg);

            if (!result)
            {
                base.Alerta(null, this, "__errIb", msg);
                //MPE.Show();
                return;
            }

            pnl.Visible = false;// MPE.Hide();
            this.LimbaCampos();
            this.CarregaEntradas();
            txtCorretorSearch.Text = "";
            txtCorretorIDSearch.Value = "";
        }

        void LimbaCampos()
        {
            ViewState[IDKey] = null;
            ViewState[IDKey2] = null;
            ViewState[IDKey3] = null;

            txtObs.Text = "";
            txtNumAte.Text = "";
            txtNumDe.Text = "";
            txtQtd.Text = "";
        }

        protected void cboMotivo_SelectedIndexChanged(Object sender, EventArgs e)
        {
            //MPE.Show();
            this.GerenciaCombosDeTipo();
        }

        void GerenciaCombosDeTipo()
        {
            if (cboMotivo.SelectedValue == "1")
            {
                trObs.Visible = false;
                trCorretor.Visible = true;
            }
            else
            {
                trObs.Visible = true;
                trCorretor.Visible = false;
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/AlmoxEntrada.aspx");
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaEntradas();
        }

        protected void cboFilial_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaEntradas();
        }

        protected void cboTipo_SelectedIndexChanged(Object sender, EventArgs e)
        {
            gridEntradas.SelectedIndex = -1;
            this.CarregaEntradas();

            //cboProduto.Items.Clear();

            //if (cboTipo.SelectedIndex > 0)
            //{
            //    cboProduto.DataValueField = "ID";
            //    cboProduto.DataTextField = "Descricao";
            //    cboProduto.DataSource = AlmoxProduto.CarregarTodos(cboTipo.SelectedValue, true);
            //    cboProduto.DataBind();
            //}

            //cboProduto.Items.Insert(0, new ListItem("NENHUM FILTRO", "-1"));
            //cboProduto.SelectedIndex = 0;
        }

        protected void cboProduto_SelectedIndexChanged(Object sender, EventArgs e)
        {
            gridEntradas.SelectedIndex = -1;
            this.CarregaEntradas();
        }

        protected void txtNumDe_TextChanged(Object sender, EventArgs e)
        {
            if (((TextBox)sender).ID == "txtNumDe")
            {
                if (txtQtd.Text.Trim() == "") { return; }
                if (txtNumDe.Text.Trim() == "" || !txtNumDe.AutoPostBack) { return; }
            }
            else if (((TextBox)sender).ID == "txtQtd")
            {
                if (txtNumDe.Text.Trim() == "")
                {
                    txtNumDe.Focus();
                    //MPE.Show();
                    return;
                }
            }

            int qtd = Convert.ToInt32(txtQtd.Text);

            AlmoxMovimentacao mov = new AlmoxMovimentacao();
            mov.ID = ViewState[IDKey3];
            mov.Carregar();

            String strQtdInicial = txtNumDe.Text;
            if (!String.IsNullOrEmpty(mov.Letra)) { strQtdInicial = strQtdInicial.Replace(mov.Letra, ""); }

            int qtdInicial = 0;

            try
            {
                qtdInicial = Convert.ToInt32(strQtdInicial);// mov.NumeracaoInicial;
            }
            catch
            {
                Alerta(null, this, "_formErr", "Formato de dados inválido.");
                //MPE.Show();
                return;
            }

            String mascara = new String('0', mov.QtdZerosAEsquerda);
            //String numDe  = String.Format("{0:" + mascara + "}", mov.NumeracaoInicialFlutuante);

            //txtNumDe.Text = numDe; // mov.NumeracaoInicialFlutuante.ToString();

            String letra = "";
            int qtdZeros = 0, intNumDe = 0;

            try
            {
                if (UIHelper.PrimeiraPosicaoELetra(txtNumDe.Text))
                {
                    letra = txtNumDe.Text.Substring(0, 1);

                    qtdZeros = txtNumAte.Text.Replace(letra, "").Length;
                    intNumDe = base.CToInt(txtNumDe.Text.Replace(letra, ""));
                }
                else
                {
                    intNumDe = base.CToInt(txtNumDe.Text);
                }
            }
            catch
            {
                //MPE.Show();
                base.Alerta(null, this, "_errInput", "Entrada inválida.");
                return;
            }

            int numFinal = intNumDe; // mov.NumeracaoInicialFlutuante;

            for (int i = 1; i < qtd; i++)
            {
                if (numFinal >= mov.NumeracaoFinal) { break; }
                numFinal++;
            }

            txtNumAte.Text = String.Format("{0:" + mascara + "}", numFinal); // numFinal.ToString();

            if (!String.IsNullOrEmpty(mov.Letra))
            {
                //txtNumDe.Text  = mov.Letra + txtNumDe.Text;
                txtNumAte.Text = mov.Letra + txtNumAte.Text;
            }

            //MPE.Show();

            if (cboMotivo.SelectedValue == "1")
            {
                cmdSalvar.Focus();
            }
            else if (cboMotivo.SelectedValue == "4" || cboMotivo.SelectedValue == "5")
            {
                txtObs.Focus();
            }
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(txtCorretorID.Value.Trim()))
            {
                gridCondensado.DataSource = null;
                gridCondensado.DataBind();
                litMsg.Text = "";
                return;
            }

            DataTable dt = AlmoxContratoImpresso.SumarioRetiradas(txtCorretorID.Value, null, true);
            gridCondensado.DataSource = dt;
            gridCondensado.DataBind();

            if(dt.Rows.Count > 0)
                litMsg.Text = "";
            else
                litMsg.Text = "<b>nenhum resultado</b>";
            dt.Dispose();
        }

        protected void gridCondensado_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "detalhes")
            {
                Object prodid = gridCondensado.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Object corrid = gridCondensado.DataKeys[Convert.ToInt32(e.CommandArgument)][1];

                DataTable dt = AlmoxContratoImpresso.SumarioRetiradas(corrid, prodid, false);

                gridCondensado_Detalhe.DataSource = dt;
                gridCondensado_Detalhe.DataBind();
                gridCondensado_Detalhe.Visible = true;
                dt.Dispose();
            }
        }

        protected void gridCondensado_Detalhe_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("rasurar"))
            {
                Object id = gridCondensado_Detalhe.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                AlmoxContratoImpresso.RasuraProposta(id);
                gridCondensado_Detalhe.Visible = false;
            }
        }

        protected void gridCondensado_Detalhe_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente marcar a proposta como rasurada?");

                if (CToString(gridCondensado_Detalhe.DataKeys[e.Row.RowIndex][2]) != String.Empty)
                {
                    String numeroContrato = CToString(gridCondensado_Detalhe.DataKeys[e.Row.RowIndex][2]) + e.Row.Cells[1].Text.PadLeft(CToInt(gridCondensado_Detalhe.DataKeys[e.Row.RowIndex][3]), '0');
                    e.Row.Cells[1].Text = numeroContrato;
                }
            }
        }
    }
}
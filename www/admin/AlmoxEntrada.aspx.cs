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

    public partial class AlmoxEntrada : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                pnlNumeracao.Visible = false;
                base.ExibirOperadoras(cboOperadoras, false);
                cboOperadoras.Items.Insert(0, new ListItem("<sem vínculo>", "-1"));
                this.CarregaTIPOSDeProduto();
                this.CarregaTiposDeMovimentacao();
                //this.CarregaCorretores();
            }
        }

        //void CarregaCorretores()
        //{
        //    cboAgente.DataValueField = "ID";
        //    cboAgente.DataTextField = "Nome";
        //    cboAgente.DataSource = Usuario.CarregarCorretores();
        //    cboAgente.DataBind();
        //}

        void CarregaTiposDeMovimentacao()
        {
            cboMovimentacao.Items.Clear();
            cboMovimentacao.Items.Add(new ListItem("Nova entrada de material", "0"));
            cboMovimentacao.Items.Add(new ListItem("Devolução de material", "2"));
        }

        void CarregaTIPOSDeProduto()
        {
            IList<AlmoxTipoProduto> lista = AlmoxTipoProduto.CarregarTodos();
            cboTipo.DataValueField = "ID";
            cboTipo.DataTextField = "Descricao";
            cboTipo.DataSource = lista;
            cboTipo.DataBind();

            //this.CarregaProdutos();
            SetaOperadora();
        }

        //void CarregaProdutos()
        //{
            //cboDescricao.Items.Clear();
            //if (cboTipo.SelectedIndex == -1 || cboOperadoras.Items.Count == 0) { pnlNumeracao.Visible = false; return; }

            //AlmoxTipoProduto obj = new AlmoxTipoProduto();
            //obj.ID = cboTipo.SelectedValue;
            //obj.Carregar();
            //pnlNumeracao.Visible = obj.Numerado;

            //cboDescricao.DataValueField = "ID";
            //cboDescricao.DataTextField  = "Descricao";
            //cboDescricao.DataSource = AlmoxProduto.CarregarTodosPorOperadora(cboOperadoras.SelectedValue, cboTipo.SelectedValue);
            //cboDescricao.DataBind();
        //}

        protected void Qtd_TextChanged(Object sender, EventArgs e)
        {
            if (((TextBox)sender).ID == "txtDe")
            {
                if (txtDe.Text.Trim() == "" || txtQtd.Text.Trim() == "") { return; }
                if (txtDe.Text.Trim() == "" || !txtDe.AutoPostBack) { return; }
            }
            else if (((TextBox)sender).ID == "txtQtd")
            {
                if (txtDe.Text.Trim() == "")
                {
                    txtDe.Focus();
                    return;
                }
            }

            String letra = "";

            int qtdZeros = 0;
            int intNumDe = 0;
            //int intNumAte = 0;

            try
            {
                if (UIHelper.PrimeiraPosicaoELetra(txtDe.Text))
                {
                    letra = txtDe.Text.Substring(0, 1);
                    qtdZeros = txtDe.Text.Replace(letra, "").Length; // UIHelper.QtdZerosAEsquerda(txtDe.Text.Replace(letra, ""));
                    intNumDe = base.CToInt(txtDe.Text.Replace(letra, ""));
                    //intNumAte = base.CToInt(txtAte.Text.Replace(letra, ""));
                }
                else
                {
                    //qtdZeros = txtDe.Text.Length;

                    try
                    {
                        intNumDe = base.CToInt(txtDe.Text);
                        //intNumAte = base.CToInt(txtAte.Text);
                    }
                    catch
                    {
                        //Alerta(null, this, "_formErr", "Formato de dados inválido.");
                        base.Alerta(MPE, ref litAlert, "Formato de dados inválido.", upnlAlerta);
                        return;
                    }
                }
            }
            catch
            {
                base.Alerta(MPE, ref litAlert, "Entrata inválida.", upnlAlerta);
                return;
            }

            int qtd = Convert.ToInt32(txtQtd.Text);

            //AlmoxMovimentacao mov = new AlmoxMovimentacao();
            //mov.ID = ViewState[IDKey3];
            //mov.Carregar();

            String strQtdInicial = txtDe.Text;
            if (!String.IsNullOrEmpty(letra)) { strQtdInicial = strQtdInicial.Replace(letra, ""); }

            int qtdInicial = 0;

            try
            {
                qtdInicial = Convert.ToInt32(strQtdInicial);// mov.NumeracaoInicial;
            }
            catch
            {
                Alerta(null, this, "_formErr", "Formato de dados inválido.");
                MPE.Show();
                return;
            }

            String mascara = new String('0', qtdZeros);
            //String numDe  = String.Format("{0:" + mascara + "}", mov.NumeracaoInicialFlutuante);

            //txtNumDe.Text = numDe; // mov.NumeracaoInicialFlutuante.ToString();

            int numFinal = intNumDe; // mov.NumeracaoInicialFlutuante;

            for (int i = 1; i < qtd; i++)
            {
                //if (numFinal >= mov.NumeracaoFinal) { break; }
                numFinal++;
            }

            txtAte.Text = String.Format("{0:" + mascara + "}", numFinal); // numFinal.ToString();

            if (!String.IsNullOrEmpty(letra))
            {
                //txtNumDe.Text  = mov.Letra + txtNumDe.Text;
                txtAte.Text = letra + txtAte.Text;
            }
        }

        protected void cboOperadoras_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            //this.CarregaProdutos();
        }

        protected void cboTipo_SelectedIndexChanged(Object sender, EventArgs e)
        {
            SetaOperadora();
            //this.CarregaProdutos();
        }

        void SetaOperadora()
        {
            if (cboTipo.Items.Count == 0) { return; }
            IList<AlmoxProduto> lista = AlmoxProduto.CarregarTodos(cboTipo.SelectedValue, true);
            if (lista != null)
            {
                if (lista[0].OperadoraID == null)
                {
                    cboOperadoras.SelectedValue = "-1";
                    cboMovimentacao.Focus();
                }
                else
                    cboOperadoras.Focus();
            }
            else if(IsPostBack)
            {
                base.Alerta(MPE, ref litAlert, "Não há nenhum produto desse tipo cadastrado.", upnlAlerta);
                pnlNumeracao.Visible = false;
            }

            AlmoxTipoProduto tipo = new AlmoxTipoProduto(cboTipo.SelectedValue);
            tipo.Carregar();
            if (tipo.Numerado)
                pnlNumeracao.Visible = true;
            else
                pnlNumeracao.Visible = false;
        }

        protected void cboMovimentacao_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            trAgente.Visible = cboMovimentacao.SelectedValue != "0";
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/AlmoxEntradas.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validação 

            if (cboOperadoras.Items.Count == 0)
            {
                //base.Alerta(null, this, "__err00", "Não há nenhuma operadora selecionada.");
                base.Alerta(MPE, ref litAlert, "Não há nenhuma operadora selecionada.", upnlAlerta);
                return;
            }

            if (trAgente.Visible && txtCorretorIDSearch.Value.Trim() == "")
            {
                //base.Alerta(null, this, "__err", "Não há nenhum corretor selecionado.");
                base.Alerta(MPE, ref litAlert, "Não há nenhum corretor selecionado.", upnlAlerta);
                return;
            }

            if (cboTipo.Items.Count == 0)
            {
                //base.Alerta(null, this, "__err00", "Não há nenhuma operadora selecionada.");
                base.Alerta(MPE, ref litAlert, "Não há nenhum tipo de produto selecionado.", upnlAlerta);
                return;
            }

            //if (cboDescricao.Items.Count == 0)
            //{
            //    //base.Alerta(null, this, "__err0", "Não há nenhum produto selecionado.");
            //    base.Alerta(MPE, ref litAlert, "Não há nenhum produto selecionado.", upnlAlerta);
            //    return;
            //}

            Object operadoraId = null;
            if (cboOperadoras.SelectedValue != "-1") { operadoraId = cboOperadoras.SelectedValue; }
            AlmoxProduto prod = AlmoxProduto.Carregar(cboTipo.SelectedValue, operadoraId);
            if (prod == null)
            {
                base.Alerta(MPE, ref litAlert, "Não há nenhum produto deste tipo para esta operadora.", upnlAlerta);
                return;
            }

            int qtd = base.CToInt(txtQtd.Text);
            if (qtd == 0)
            {
                //base.Alerta(null, this, "__err1", "Informe a quantidade.");
                base.Alerta(MPE, ref litAlert, "Informe a quantidade.", upnlAlerta);
                txtQtd.Focus();
                return;
            }

            String letra = "";

            int qtdZeros = 0;
            int intNumDe = 0;
            int intNumAte = 0;
            if (UIHelper.PrimeiraPosicaoELetra(txtDe.Text))
            {
                letra = txtDe.Text.Substring(0, 1);
                qtdZeros = txtAte.Text.Replace(letra, "").Length; // UIHelper.QtdZerosAEsquerda(txtDe.Text.Replace(letra, ""));
                intNumDe = base.CToInt(txtDe.Text.Replace(letra, ""));
                intNumAte = base.CToInt(txtAte.Text.Replace(letra, ""));
            }
            else
            {
                try
                {
                    intNumDe = base.CToInt(txtDe.Text);
                    intNumAte = base.CToInt(txtAte.Text);
                }
                catch
                {
                    //Alerta(null, this, "_formErr", "Formato de dados inválido.");
                    base.Alerta(MPE, ref litAlert, "Formato de dados inválido.", upnlAlerta);
                    return;
                }
            }

            if (pnlNumeracao.Visible)
            {
                if (intNumDe == 0)
                {
                    //base.Alerta(null, this, "__err2", "Informe a numeração inicial.");
                    base.Alerta(MPE, ref litAlert, "Informe a numeração inicial.", upnlAlerta);
                    txtDe.Focus();
                    return;
                }
                if (intNumAte == 0)
                {
                    //base.Alerta(null, this, "__err3", "Informe a numeração final.");
                    base.Alerta(MPE, ref litAlert, "Informe a numeração final.", upnlAlerta);
                    txtAte.Focus();
                    return;
                }

                if (intNumAte < intNumDe)
                {
                    //base.Alerta(null, this, "__err4", "A numeração final deve ser maior que a numeração inicial.");
                    base.Alerta(MPE, ref litAlert, "A numeração final deve ser maior que a numeração inicial.", upnlAlerta);
                    txtAte.Focus();
                    return;
                }

                if (cboMovimentacao.SelectedValue == "0") //procedimento normal
                {
                    int i = 0;
                    //for (i = intNumDe; i < intNumAte; i++) { }

                    i = (intNumAte - intNumDe) + 1;

                    if (i != qtd)
                    {
                        //base.Alerta(null, this, "__err5", "O intervalo não corresponde à quantidade informada.");
                        base.Alerta(MPE, ref litAlert, "O intervalo não corresponde à quantidade informada.", upnlAlerta);
                        txtQtd.Focus();
                        return;
                    }
                }
                else if (cboMovimentacao.SelectedValue == "2")
                {
                    int i = (intNumAte + 1) - intNumDe;
                    //for (i = intNumDe; i < intNumAte; i++) { }

                    if (i != qtd)
                    {
                        //base.Alerta(null, this, "__err6", "O intervalo não corresponde à quantidade informada.");
                        base.Alerta(MPE, ref litAlert, "O intervalo não corresponde à quantidade informada.", upnlAlerta);
                        txtQtd.Focus();
                        return;
                    }
                }
            }

            if (cboMovimentacao.SelectedValue == "0") //procedimento normal
            {
                if (AlmoxMovimentacao.ExisteIntervalo(intNumDe, intNumAte, cboOperadoras.SelectedValue, qtdZeros, letra))
                {
                    base.Alerta(MPE, ref litAlert, "O intervalo já está cadastrado em estoque.", upnlAlerta);
                    return;
                }
            }

            #endregion

            AlmoxMovimentacao obj = new AlmoxMovimentacao();
            obj.NumeracaoFinal = intNumAte;
            obj.NumeracaoInicial = intNumDe;
            obj.NumeracaoFinalFlutuante = intNumAte;
            obj.NumeracaoInicialFlutuante = intNumDe;
            obj.ProdutoID = prod.ID; // cboDescricao.SelectedValue;
            obj.QTD = qtd;
            obj.QTDFlutuante = qtd;
            obj.TipoDeMovimentacao = (int)AlmoxMovimentacao.TipoMovimentacao.Entrada;
            obj.UsuarioID = Usuario.Autenticado.ID;

            obj.Letra = letra;
            obj.QtdZerosAEsquerda = qtdZeros;

            if (trAgente.Visible)
                obj.UsuarioRetiradaID = txtCorretorIDSearch.Value;

            String msg = "";
            if (cboMovimentacao.SelectedValue == "0") //procedimento padrao
            {
                 //result = AlmoxMovimentacaoFacade.Instance.SalvarEntrada(obj, ref msg);
            }
            else //devolução. deve alterar a quantidade em estoque do produto
            {
                obj.SubTipoDeMovimentacao = (int)AlmoxMovimentacao.SubTipoMovimentacao.Devolucao;
            }

            Boolean result = AlmoxMovimentacaoFacade.Instance.SalvarEntrada(obj, ref msg, cboOperadoras.SelectedValue);
            if (!result)
            {
                //base.Alerta(null, this, "__errI", msg);
                base.Alerta(MPE, ref litAlert, msg, upnlAlerta);
                return;
            }

            cmdSalvar.Enabled = false;
            //base.Alerta(null, this, "__ok", "Dados salvos com sucesso.");
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
        }
    }
}
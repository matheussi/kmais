namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class conferencia2 : PageBaseConferencia
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                ucCPM.EscreveMensagens();
                base.ExibirFiliais(cboFilial, false);
                ucConferenciaPassos.SetaPasso(1, "Fase 1");

                base.ExibirEstipulantes(cboEstipulante, false, true);
                this.CarregaOperadora();
                this.CarregaContratosADM();
                this.CarregaPlano();
                this.CarregaAcomadacoes();
                this.CarregaCheckList();
                this.CarregaTipoContrato();

                this.LeConferencia();
                //this.CarregaContratosADM();
            }
        }

        void CarregaContratosADM()
        {
            cboContratoADM.Items.Clear();
            if (!base.HaItemSelecionado(cboEstipulante) || !base.HaItemSelecionado(cboOperadora)) { return; }

            cboContratoADM.DataValueField = "ID";
            cboContratoADM.DataTextField = "Descricao";
            cboContratoADM.DataSource = ContratoADM.Carregar(cboEstipulante.SelectedValue, cboOperadora.SelectedValue, true);
            cboContratoADM.DataBind();
        }

        void CarregaOperadora()
        {
            base.ExibirOperadoras(cboOperadora, false);
        }

        void CarregaPlano()
        {
            cboPlano.Items.Clear();
            if (!base.HaItemSelecionado(cboContratoADM)) { return; }

            cboPlano.DataValueField = "ID";
            cboPlano.DataTextField = "Descricao";
            cboPlano.DataSource = Plano.CarregarPorContratoID(cboContratoADM.SelectedValue, true);
            cboPlano.DataBind();
        }

        void CarregaAcomadacoes()
        {
            cboAcomodacao.Items.Clear();
            if (!base.HaItemSelecionado(cboPlano)) { return; }

            Plano plano = new Plano(cboPlano.SelectedValue);
            plano.Carregar();
            base.ExibirTiposDeAcomodacao(cboAcomodacao, plano.QuartoComum, plano.QuartoParticular, false);
            plano = null;
        }

        void CarregaTipoContrato()
        {
            cboTipoContrato.Items.Clear();
            IList<TipoContrato> tipos = TipoContrato.Carregar(true);
            cboTipoContrato.DataValueField = "ID";
            cboTipoContrato.DataTextField = "Descricao";
            cboTipoContrato.DataSource = tipos;
            cboTipoContrato.DataBind();
        }

        void CarregaCheckList()
        {
            chklCheckList.Items.Clear();
            if (!base.HaItemSelecionado(cboOperadora)) { return; }
            IList<CheckList> list = CheckList.Carregar(cboOperadora.SelectedValue);
            chklCheckList.DataValueField = "ID";
            chklCheckList.DataTextField = "Descricao";
            chklCheckList.DataSource = list;
            chklCheckList.DataBind();
        }

        void LimpaMensagens()
        {
            ucCPM.RemoveMsg("___numProposta1");
            ucCPM.RemoveMsg("___numProposta2");
            ucCPM.RemoveMsg("___numProposta1e2");
            ucCPM.RemoveMsg("__inexistente");
            ucCPM.RemoveMsg("__inesperado1");
            ucCPM.RemoveMsg("__emEstoque");
            ucCPM.RemoveMsg("__rasurado");
            ucCPM.RemoveMsg("__identCorretorPF");
        }

        void HabilitaCampos()
        {
            cboOperadora.Enabled = true;
            txtCorretor.ReadOnly = false;
            //txtCorretor.Text = "";
            //txtCorretorID.Value = "";
        }

        void PreencheCorretor(Object corretorId)
        {
            Usuario corretor = Usuario.CarregarParcial(corretorId);
            txtCorretor.Text = String.Concat(corretor.Nome, " (", corretor.Documento1, ")");
            txtCorretorID.Value = Convert.ToString(corretor.ID);
        }

        Boolean ValidaProposta(Boolean recarregaOperadora)
        {
            this.LimpaMensagens();
            Boolean returned = true;

            if (txtNumeroProposta.Text.Trim() == "")
            {
                ucCPM.SetaMsg("___numProposta1", "Informe o número da proposta");
                txtNumeroProposta.Focus();
                returned = false;
            }
            else
                ucCPM.RemoveMsg("___numProposta1");

            if (base.CStringToDateTime(txtAdmissao.Text) == DateTime.MinValue)
            {
                ucCPM.SetaMsg("___admissao", "Informe a data de admissão");
                txtAdmissao.Focus();
                returned = false;
            }
            else
                ucCPM.RemoveMsg("___admissao");

            if (txtNumeroPropostaConfirme.Text.Trim() == "")
            {
                ucCPM.SetaMsg("___numProposta2", "Confirme o número da proposta");
                txtNumeroPropostaConfirme.Focus();
                returned = false;
            }
            else
                ucCPM.RemoveMsg("___numProposta2");

            if (txtNumeroProposta.Text.Trim() != txtNumeroPropostaConfirme.Text.Trim())
            {
                ucCPM.SetaMsg("___numProposta1e2", "Os números de proposta informados não coincidem");
                txtNumeroProposta.Focus();
                returned = false;
            }
            else
                ucCPM.RemoveMsg("___numProposta1e2");

            ucCPM.RemoveMsg("__checklist");
            for (int i = 0; i < chklCheckList.Items.Count; i++)
            {
                if (!chklCheckList.Items[i].Selected)
                {
                    ucCPM.SetaMsg("__checklist", "Há itens não conferidos no checklist");
                    //returned = false;
                }
            }

            if (txtCorretorID.Value.Trim() == "") { ucCPM.SetaMsg("___corret", "Não há foi informado um corretor"); returned = false; } else { ucCPM.RemoveMsg("___corret"); }
            if (!base.HaItemSelecionado(cboEstipulante)) { ucCPM.SetaMsg("___estip", "Não há um estipulante selecionado"); returned = false; } else { ucCPM.RemoveMsg("___estip"); }
            if (!base.HaItemSelecionado(cboOperadora)) { ucCPM.SetaMsg("___oper", "Não há uma operadora selecionado"); returned = false; } else { ucCPM.RemoveMsg("___oper"); }
            if (!base.HaItemSelecionado(cboContratoADM)) { ucCPM.SetaMsg("___contr", "Não há um contrato administrativo selecionado"); return false; } else { ucCPM.RemoveMsg("___contr"); }
            if (!base.HaItemSelecionado(cboPlano)) { ucCPM.SetaMsg("___plano", "Não há um plano selecionado"); returned = false; } else { ucCPM.RemoveMsg("___plano"); }
            if (cboAcomodacao.Items.Count == 0) { ucCPM.SetaMsg("___acomod", "Não uma acomodação selecionada"); returned = false; } else { ucCPM.RemoveMsg("___acomod"); }

            if (!String.IsNullOrEmpty(txtCorretorID.Value))
            {
                Usuario corretor = new Usuario(txtCorretorID.Value);
                corretor.Carregar();
                if (corretor.TipoPessoa == 2) //pessoa jurídica: checa se foi identificado o corretor pessoa fisica
                {
                    if (!UIHelper.VerificaCnpj(txtCorretorTerceiroCPF.Text) || String.IsNullOrEmpty(txtCorretorTerceiroIdentificacao.Text))
                    {
                        ucCPM.SetaMsg("__identCorretorPF", "Informe o corretor pessoa física");
                    }
                }

                corretor = null;
            }

            #region valida numero da proposta (impresso)

            if (!this.ValidaNumeroDeProposta(true, recarregaOperadora)) { returned = false; }

            #endregion

            ucCPM.EscreveMensagens();
            return returned;
        }

        Boolean ValidaNumeroDeProposta(Boolean preencheCorretorEncontrato, Boolean recarregaOperadora)
        {
            #region valida numero da proposta (impresso) 

            this.HabilitaCampos();

            Boolean teste = true;

            String letra = "";
            if (UIHelper.PrimeiraPosicaoELetra(txtNumeroProposta.Text))
                letra = txtNumeroProposta.Text.Substring(0, 1).ToUpper();

            String numero = txtNumeroProposta.Text.ToUpper();
            if (letra.Length > 0)
                numero = numero.Replace(letra, "");

            Int32 result = 0;
            if (!Int32.TryParse(numero, out result))
            {
                ucCPM.SetaMsg("__inesperado1", "Número de proposta inválido");
                return false;
            }

            teste = AlmoxContratoImpresso.ExisteContrato(Convert.ToInt32(numero), letra, -1);

            if (!teste)
            {
                //nao existe
                ucCPM.SetaMsg("__inexistente", "Proposta inexistente");
                return true;
            }
            else
            {
                ucCPM.RemoveMsg("__inexistente");
                //checa a operadora do contrato e com qual corretor está. seleciona nos combos e 
                //trava a selecao. Se o mesmo numero de contrato existir em mais de uma operadora,
                //o usuário deverá selecionar qual delas será uttilizada.
                IList<AlmoxContratoImpresso> lista = null;

                lista = AlmoxContratoImpresso.CarregarPorNumeroProduto(Convert.ToInt32(numero), letra, -1);

                if (lista == null || lista.Count == 0)
                {
                    //erro inesperado
                    ucCPM.SetaMsg("__inesperado1", "Proposta indisponível - Erro insperado");
                    return true;
                }
                else if (lista.Count == 1)
                {
                    //checa se o contrato (físico, o papel) foi retirado e está com um corretor
                    teste = AlmoxContratoImpresso.ContratoFoiRetiradoDoEstoque(Convert.ToInt32(numero), letra, -1, lista[0].OperadoraID);

                    if (!teste)
                    {
                        //o contrato ainda consta como em estoque.
                        ucCPM.SetaMsg("__emEstoque", "O contrato ainda encontra-se em estoque");
                    }

                    if (lista[0].Rasurado)
                    {
                        ucCPM.SetaMsg("__rasurado", "O contrato está rasurado");
                        return false;
                    }
                    else
                    {
                        if (lista[0].AgenteID != null)
                        {
                            //está com corretor, então preenche os campos e nao permite alterar

                            if (preencheCorretorEncontrato)
                            {
                                this.PreencheCorretor(lista[0].AgenteID);
                                //txtCorretor.ReadOnly = true;
                            }

                            if (recarregaOperadora)
                            {
                                cboOperadora.SelectedValue = Convert.ToString(lista[0].OperadoraID);
                                cboOperadora.Enabled = false;
                                cboOperadora_SelectedIndexChanged(null, null);
                            }
                            
                        }
                    }
                }
                else if (lista.Count > 1) //há mais de uma proposta com esse número. o usuário deverá selecionar
                {
                    pnlSelNumeroContral.Visible = true;
                    gridSelNumeroContral.DataSource = lista;
                    gridSelNumeroContral.DataBind();
                }
            }

            return true;

            #endregion
        }

        void LeConferencia()
        {
            Conferencia conferencia = base.ConferenciaCorrente;
            if (conferencia == null) { return; }

            txtNumeroProposta.Text = conferencia.PropostaNumero;
            txtNumeroPropostaConfirme.Text = conferencia.PropostaNumero;

            if (conferencia.FilialID != null)
            {
                cboFilial.SelectedValue = Convert.ToString(conferencia.FilialID);
            }

            if (conferencia.EstipulanteID != null)
            {
                cboEstipulante.SelectedValue = Convert.ToString(conferencia.EstipulanteID);
                cboEstipulante_Changing(null, null);
            }

            if (conferencia.OperadoraID != null)
            {
                cboOperadora.SelectedValue = Convert.ToString(conferencia.OperadoraID);
                cboOperadora_SelectedIndexChanged(null, null);
                cboOperadora.Enabled = false;
            }

            if (conferencia.ContratoAdmID != null)
            {
                cboContratoADM.SelectedValue = Convert.ToString(conferencia.ContratoAdmID);
                cboContratoADM_Changing(null, null);
            }

            if (conferencia.PlanoID != null)
            {
                cboPlano.SelectedValue = Convert.ToString(conferencia.PlanoID);
                cboPlano_Changing(null, null);
            }

            if (conferencia.AcomodacaoID != null)
            {
                cboAcomodacao.SelectedValue = Convert.ToString(conferencia.AcomodacaoID);
            }

            if (conferencia.CorretorID != null && Convert.ToString(conferencia.CorretorID) != "" && Convert.ToInt32(conferencia.CorretorID) > 0)
            {
                txtCorretorID.Value = Convert.ToString(conferencia.CorretorID);

                if (String.IsNullOrEmpty(conferencia.CorretorNome))
                {
                    Usuario user = new Usuario(conferencia.CorretorID);
                    user.Carregar();
                    conferencia.CorretorNome = user.Nome;
                    user = null;
                }
                txtCorretor.Text = conferencia.CorretorNome;
                //txtCorretor.ReadOnly = true;
            }

            txtCorretorTerceiroCPF.Text = conferencia.CorretorTerceiroCPF;
            txtCorretorTerceiroIdentificacao.Text = conferencia.CorretorTerceiroNome;
            txtSuperiorTerceiroCPF.Text = conferencia.SuperiorTerceiroCPF;
            txtSuperiorTerceiroIdentificacao.Text = conferencia.SuperiorTerceiroNome;

            if (conferencia.TipoContratoID != null)
            {
                cboTipoContrato.SelectedValue = Convert.ToString(conferencia.TipoContratoID);

                if (conferencia.TipoContratoExplicito) //setado no passo 1
                    cboTipoContrato.Enabled = false;
                else
                    cboTipoContrato.Enabled = true;
            }

            if (conferencia.Admissao != DateTime.MinValue)
                txtAdmissao.Text = conferencia.Admissao.ToString("dd/MM/yyyy");

            #region le checklist 

            if (conferencia.ID != null && conferencia.OperadoraID != null)
            {
                //carrega itens de checklist
                IList<ConferenciaCheckList> itens = ConferenciaCheckList.Carregar(conferencia.ID);
                if (itens != null)
                {
                    foreach (ConferenciaCheckList item in itens)
                    {
                        for (int i = 0; i < chklCheckList.Items.Count; i++)
                        {
                            if (Convert.ToString(item.ItemCheckListID).Equals(chklCheckList.Items[i].Value))
                            {
                                chklCheckList.Items[i].Selected = item.Valor;
                                break;
                            }
                        }
                    }
                }
            }
            #endregion
        }

        void PersisteConferencia()
        {
            if (cboFilial.Items.Count == 0)
            {
                base.Alerta(null, this, "_errFi", "Você deve informar a filial.");
                return;
            }

            Conferencia conferencia = base.ConferenciaCorrente;
            if (conferencia == null) { conferencia = new Conferencia(); }// Response.Redirect("conferenciaLista.aspx"); }

            conferencia.PropostaNumero = txtNumeroProposta.Text;

            conferencia.FilialID = cboFilial.SelectedValue;

            if (base.HaItemSelecionado(cboEstipulante))
                conferencia.EstipulanteID = cboEstipulante.SelectedValue;
            else
                conferencia.EstipulanteID = null;

            if (base.HaItemSelecionado(cboOperadora))
                conferencia.OperadoraID = cboOperadora.SelectedValue;
            else
                conferencia.OperadoraID = null;

            if (base.HaItemSelecionado(cboContratoADM))
                conferencia.ContratoAdmID = cboContratoADM.SelectedValue;
            else
                conferencia.ContratoAdmID = null;

            if (base.HaItemSelecionado(cboPlano))
                conferencia.PlanoID = cboPlano.SelectedValue;
            else
                conferencia.PlanoID = null;

            if (cboAcomodacao.Items.Count > 0)
                conferencia.AcomodacaoID = cboAcomodacao.SelectedValue;
            else
                conferencia.AcomodacaoID = null;

            conferencia.CorretorID = txtCorretorID.Value;
            conferencia.CorretorNome = txtCorretor.Text;
            conferencia.TipoContratoID = cboTipoContrato.SelectedValue;

            conferencia.Admissao = base.CStringToDateTime(txtAdmissao.Text);

            conferencia.CorretorTerceiroCPF = txtCorretorTerceiroCPF.Text;
            conferencia.CorretorTerceiroNome = txtCorretorTerceiroIdentificacao.Text;
            conferencia.SuperiorTerceiroCPF = txtSuperiorTerceiroCPF.Text;
            conferencia.SuperiorTerceiroNome = txtSuperiorTerceiroIdentificacao.Text;

            conferencia.Salvar();

            if (chklCheckList.Items.Count > 0)
            {
                List<ConferenciaCheckList> itens = new List<ConferenciaCheckList>();
                for (int i = 0; i < chklCheckList.Items.Count; i++)
                {
                    ConferenciaCheckList item = new ConferenciaCheckList();
                    item.ConferenciaID = conferencia.ID;
                    item.ItemCheckListID = chklCheckList.Items[i].Value;
                    item.Valor = chklCheckList.Items[i].Selected;
                    itens.Add(item);
                }

                ConferenciaCheckList.Salvar(itens);
            }
            else
            {
                //remove tudo
            }
            
            base.ConferenciaCorrente = conferencia;
        }

        protected void gridSelNumeroContral_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("usar"))
            {
                Object id   = gridSelNumeroContral.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Object opid = gridSelNumeroContral.DataKeys[Convert.ToInt32(e.CommandArgument)][1];
                Object agid = gridSelNumeroContral.DataKeys[Convert.ToInt32(e.CommandArgument)][2];

                this.PreencheCorretor(agid);
                //txtCorretor.ReadOnly = true;

                cboOperadora.SelectedValue = Convert.ToString(opid);
                cboOperadora_SelectedIndexChanged(cboOperadora, null);
                cboOperadora.Enabled = false;

                AlmoxContratoImpresso aci = new AlmoxContratoImpresso(id);
                aci.Carregar();
                if (aci.Rasurado)
                {
                    txtCorretor.Text = "";
                    txtCorretorID.Value = "";
                    //txtOperador.Text = "";
                    //txtOperadorID.Value = "";
                    cboOperadora.SelectedIndex = 0;
                    cboOperadora.Enabled = true;
                    cboOperadora.SelectedIndex = 0;
                    cboOperadora_SelectedIndexChanged(null, null);
                    //this.ContratoImpressoID = null;
                }
                //else
                //    this.ContratoImpressoID = aci.ID;
            }
        }

        protected void cmdValidarNumeroProposta_Click(Object sender, EventArgs e)
        {
            this.ValidaNumeroDeProposta(true, true);
            this.CarregaCheckList();
        }

        protected void cboEstipulante_Changing(Object sender, EventArgs e)
        {
            this.CarregaContratosADM();
            this.CarregaPlano();
            this.CarregaAcomadacoes();
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratosADM();
            this.CarregaPlano();
            this.CarregaAcomadacoes();
            this.CarregaCheckList();
        }

        protected void cboContratoADM_Changing(Object sender, EventArgs e)
        {
            this.CarregaPlano();
            this.CarregaAcomadacoes();
        }

        protected void cboPlano_Changing(Object sender, EventArgs e)
        {
            this.CarregaAcomadacoes();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            //this.PersisteConferencia();
            Response.Redirect("conferenciaLista.aspx");
        }

        protected void cmdProximo_Click(Object sender, EventArgs e)
        {
            if (!this.ValidaProposta(false)) { return; }

            String checkMsg = "";
            if (!Contrato.ContratoDisponivel(null, cboOperadora.SelectedValue, txtNumeroProposta.Text, ref checkMsg))
            {
                ucCPM.SetaMsg("_emUso", checkMsg);
                return;
            }
            else
            {
                ucCPM.RemoveMsg("_emUso");
            }

            this.PersisteConferencia();
            Response.Redirect("conferencia2.aspx");
        }
    }
}
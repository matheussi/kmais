/*
conferencia@padraoseguros.com.br
1@1a


conferencia nao pode ser recusada (nao pode ir para o cadastro)
doenca marcada: reter o cheque. ir para análise
por data de admissao
contratos adm listados: soh os ativos, com data de admissao >= a data da admissao
peso altura titular

cadastro
   tirar o botao "cadastrar" do grid
   colocar um textbox para digitar o numero e um botao "cadastrar"
   nao pode trocar etipulante, tipo, enfim, nenhum combo
   forma de devolver para a conferencia 
   
   bug do calendário
   
   regra do casamento entre os conjuges na tela de contrato
   tela de beneficiarios (parentesco, primeiro item deve ser em branco e obrigar a preencher)
*/

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

    public partial class conferencia3 : PageBaseConferencia
    {
        List<Adicional> adicionaisParaTodos
        {
            get { return ViewState["__adicionaisParaTodos"] as List<Adicional>; }
            set { ViewState["__adicionaisParaTodos"] = value; }
        }

        List<ConferenciaBeneficiario> beneficiariosAdicionados
        {
            get { return ViewState["__beneficiarios"] as List<ConferenciaBeneficiario>; }
            set { ViewState["__beneficiarios"] = value; }
        }

        Object DependenteID
        {
            get { return ViewState["__depID"]; }
            set { ViewState["__depID"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtPeso.Attributes.Add("onKeyUp", "mascara('" + txtPeso.ClientID + "')");
            txtAltura.Attributes.Add("onKeyUp", "mascara('" + txtAltura.ClientID + "')");

            if (!IsPostBack)
            {
                tblAdicionados.Visible = false;
                ucCPM.EscreveMensagens();
                ucConferenciaPassos.SetaPasso(2, "Fase 2");

                this.CarregaParentescos();
                this.CarregaAdicionais();
                this.LeConferencia();
                this.CarregaEstadosCivis();
            }
        }

        void CarregaEstadosCivis()
        {
            cboEstadoCivil.Items.Clear();
            cboEstadoCivil.DataValueField = "ID";
            cboEstadoCivil.DataTextField  = "Descricao";
            cboEstadoCivil.DataSource     = EstadoCivil.CarregarTodos(base.ConferenciaCorrente.OperadoraID);
            cboEstadoCivil.DataBind();
        }

        #region comentado

        //Boolean ValidaCPF()
        //{
            //litCPF.Text = "";
            //if (!UIHelper.ValidaCpf(txtCpf.Text))
            //{
            //    ucCPM.SetaMsg("cpfTitular", "CPF do titular inválido.");
            //    gridBeneficiarios.DataSource = null;
            //    gridBeneficiarios.DataBind();
            //    return false;
            //}
            //else
            //{
            //    ucCPM.RemoveMsg("cpfTitular");

            //    IList<Beneficiario> ret = Beneficiario.CarregarPorParametro(null, txtCpf.Text, null, SearchMatchType.QualquerParteDoCampo);
            //    if (ret == null)
            //    {
            //        trLista.Visible = false;
            //        trNome.Visible = true;
            //        txtNomeTitular.Focus();
            //    }
            //    else
            //    {
            //        List<Beneficiario> lista = new List<Beneficiario>();
            //        Beneficiario novo = new Beneficiario(-1);
            //        novo.Nome = "<b>NOVO BENEFICIÁRIO</b>";
            //        //lista.Add(novo); denis

            //        foreach (Beneficiario benef in ret)
            //            lista.Add(benef);

            //        if (lista.Count == 0) { lista.Add(novo); } //denis

            //        gridBeneficiarios.DataSource = lista;
            //        gridBeneficiarios.DataBind();
            //        trLista.Visible = true;
            //        trNome.Visible = false;
            //    }

            //    #region comentado
            //    //ucCPM.RemoveMsg("canc6meses");

            //    //if (base.ConferenciaCorrente == null) { base.ConferenciaCorrente = new Conferencia(); }
            //    //base.ConferenciaCorrente.TipoContratoExplicito = false;

            //    //if (UIHelper.PerfilConferencia(Usuario.Autenticado.PerfilID))
            //    //{
            //    //    //Checa se há um contrato adimplente anterior. se sim, a venda é administrativa.
            //    //    ContratoStatusHistorico entradaHistorico = null;
            //    //    Contrato existente = Contrato.CarregarUltimoContratoDoBeneficiario(txtCpf.Text, ref entradaHistorico, true);
            //    //    if (existente != null)
            //    //    {
            //    //        base.ConferenciaCorrente.TipoContratoExplicito = true;
            //    //        base.ConferenciaCorrente.TipoContratoID = 4; //ADMINISTRATIVO
            //    //    }
            //    //    else
            //    //    {
            //    //        //checa se existe um contrato cancelado em até 6 meses, ou em mais de 6 meses
            //    //        existente = Contrato.CarregarUltimoContratoDoBeneficiario(txtCpf.Text, ref entradaHistorico, false);

            //    //        if (existente != null && existente.DataCancelamento != DateTime.MinValue)
            //    //        {
            //    //            DateTime propostaData = base.ConferenciaCorrente.PropostaData;
            //    //            TimeSpan result = propostaData.Subtract(existente.DataCancelamento);
            //    //            if (result.Days <= 180) //menor ou igual a 6 meses
            //    //            {
            //    //                int meses = result.Days / 30;
            //    //                base.ConferenciaCorrente.TipoContratoExplicito = true;
            //    //                base.ConferenciaCorrente.TipoContratoID = 4; //ADMINISTRATIVO

            //    //                ucCPM.SetaMsg("canc6meses", "Há um contrato anterior cancelado em até " + meses.ToString() + " meses. Necessita antecipar cheque");
            //    //            }
            //    //            else if (result.Days > 180)
            //    //            {
            //    //                base.ConferenciaCorrente.TipoContratoExplicito = true;
            //    //                base.ConferenciaCorrente.TipoContratoID = 1; //NOVO
            //    //            }
            //    //        }
            //    //    }
            //    //}
            //    #endregion
            //}

            //litCPF.Text = "ok";
            //return true;
        //}
        #endregion

        Boolean ValidaCPF_DEPENDENTE()
        {
            litCpfDependente.Text = "";
            if (!UIHelper.ValidaCpf(txtCpfDependente.Text))
            {
                base.Alerta(null, this, "__errCPFDep", "CPF do dependente inválido.");
                gridBeneficiariosDependentes.DataSource = null;
                gridBeneficiariosDependentes.DataBind();
                txtCpfDependente.Focus();
                return false;
            }
            else
            {
                IList<Beneficiario> ret = Beneficiario.CarregarPorParametro(txtCpfDependente.Text, base.CStringToDateTime(txtDataNascimento.Text));
                if (ret == null)
                {
                    trListaDependentes.Visible = false;
                    //trNomeDependente.Visible = false;
                    //txtNomeDependente.ReadOnly = true;
                    txtNomeDependente.Focus(); //txtDataNascimento.Focus();
                    //cmdAdicionarBeneficiario.Enabled = true; removido por solicitacao
                    txtCpfDependente.ReadOnly = true;
                }
                else
                {
                    List<Beneficiario> lista = new List<Beneficiario>();
                    Beneficiario novo = new Beneficiario(-1);
                    novo.Nome = "<b>NOVO DEPENDENTE</b>";
                    //lista.Add(novo); denis

                    foreach (Beneficiario benef in ret)
                        lista.Add(benef);

                    if (lista.Count == 0) { lista.Add(novo); } //denis

                    gridBeneficiariosDependentes.DataSource = lista;
                    gridBeneficiariosDependentes.DataBind();
                    trListaDependentes.Visible = true;
                    //trNomeDependente.Visible = false;
                    //txtNomeDependente.ReadOnly = true;

                    txtCpfDependente.ReadOnly = true;
                }

                #region comentado
                //ucCPM.RemoveMsg("canc6meses");

                //if (base.ConferenciaCorrente == null) { base.ConferenciaCorrente = new Conferencia(); }
                //base.ConferenciaCorrente.TipoContratoExplicito = false;

                //if (UIHelper.PerfilConferencia(Usuario.Autenticado.PerfilID))
                //{
                //    //Checa se há um contrato adimplente anterior. se sim, a venda é administrativa.
                //    ContratoStatusHistorico entradaHistorico = null;
                //    Contrato existente = Contrato.CarregarUltimoContratoDoBeneficiario(txtCpf.Text, ref entradaHistorico, true);
                //    if (existente != null)
                //    {
                //        base.ConferenciaCorrente.TipoContratoExplicito = true;
                //        base.ConferenciaCorrente.TipoContratoID = 4; //ADMINISTRATIVO
                //    }
                //    else
                //    {
                //        //checa se existe um contrato cancelado em até 6 meses, ou em mais de 6 meses
                //        existente = Contrato.CarregarUltimoContratoDoBeneficiario(txtCpf.Text, ref entradaHistorico, false);

                //        if (existente != null && existente.DataCancelamento != DateTime.MinValue)
                //        {
                //            DateTime propostaData = base.ConferenciaCorrente.PropostaData;
                //            TimeSpan result = propostaData.Subtract(existente.DataCancelamento);
                //            if (result.Days <= 180) //menor ou igual a 6 meses
                //            {
                //                int meses = result.Days / 30;
                //                base.ConferenciaCorrente.TipoContratoExplicito = true;
                //                base.ConferenciaCorrente.TipoContratoID = 4; //ADMINISTRATIVO

                //                ucCPM.SetaMsg("canc6meses", "Há um contrato anterior cancelado em até " + meses.ToString() + " meses. Necessita antecipar cheque");
                //            }
                //            else if (result.Days > 180)
                //            {
                //                base.ConferenciaCorrente.TipoContratoExplicito = true;
                //                base.ConferenciaCorrente.TipoContratoID = 1; //NOVO
                //            }
                //        }
                //    }
                //}
                #endregion
            }

            litCpfDependente.Text = "ok";
            return true;
        }

        /// <summary>
        /// TODO: validar cada beneficiário: titular e dependentes
        /// </summary>
        Boolean ValidaCPF(String cpf)
        {
            ucCPM.RemoveMsg("canc6meses");

            if (base.ConferenciaCorrente == null) { base.ConferenciaCorrente = new Conferencia(); }
            base.ConferenciaCorrente.TipoContratoExplicito = false;

            if (UIHelper.PerfilConferencia(Usuario.Autenticado.PerfilID))
            {
                //Checa se há um contrato adimplente anterior. se sim, a venda é administrativa.
                ContratoStatusHistorico entradaHistorico = null;
                Contrato existente = Contrato.CarregarUltimoContratoDoBeneficiario(cpf, ref entradaHistorico, true);
                if (existente != null)
                {
                    base.ConferenciaCorrente.TipoContratoExplicito = true;
                    base.ConferenciaCorrente.TipoContratoID = 4; //ADMINISTRATIVO
                }
                else
                {
                    //checa se existe um contrato cancelado em até 6 meses, ou em mais de 6 meses
                    existente = Contrato.CarregarUltimoContratoDoBeneficiario(cpf, ref entradaHistorico, false);

                    if (existente != null && existente.DataCancelamento != DateTime.MinValue)
                    {
                        DateTime propostaData = base.ConferenciaCorrente.PropostaData;
                        TimeSpan result = propostaData.Subtract(existente.DataCancelamento);
                        if (result.Days <= 180) //menor ou igual a 6 meses
                        {
                            int meses = result.Days / 30;
                            base.ConferenciaCorrente.TipoContratoExplicito = true;
                            base.ConferenciaCorrente.TipoContratoID = 4; //ADMINISTRATIVO

                            ucCPM.SetaMsg("canc6meses", "Há um contrato anterior cancelado em até " + meses.ToString() + " meses. Necessita antecipar cheque");
                        }
                        else if (result.Days > 180)
                        {
                            base.ConferenciaCorrente.TipoContratoExplicito = true;
                            base.ConferenciaCorrente.TipoContratoID = 1; //NOVO
                        }
                    }
                }
            }

            return true;
        }

        Boolean ValidaCEP()
        {
            if (txtCep.Text.Trim() == "") { return false; }

            litCEP.Text = "";
            String[] arr = base.PegaEndereco(this, txtCep, new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox(), false);
            litEndereco.Text = "";

            if (arr == null)
            {
                ucCPM.SetaMsg("cepProposta", "CEP não encontrado.");
                return false;
                //arr = new String[] { "SP", "Sao Paulo", "Cambuci", "Av. Lins de Vasconcelos" };
                //litEndereco.Text = arr[3] + " - " + arr[2] + "<br>" + arr[1] + " - " + arr[0];
                //ucCPM.RemoveMsg("cepProposta");
                //litCEP.Text = "ok";
                //return true;
            }
            else
            {
                litEndereco.Text = arr[3] + " - " + arr[2] + "<br>" + arr[1] + " - " + arr[0];
                ucCPM.RemoveMsg("cepProposta");
                litCEP.Text = "ok";
                return true;
            }
        }

        Boolean ValidaProposta()
        {
            ucCPM.RemoveMsg("__semTitular");

            if (this.beneficiariosAdicionados == null || this.beneficiariosAdicionados.Count == 0)
            {
                ucCPM.SetaMsg("__semTitular", "Nenhum titular encontrado");
            }
            else
            {
                Boolean comTitular = false;
                foreach (ConferenciaBeneficiario cb in this.beneficiariosAdicionados)
                {
                    if (Convert.ToString(cb.ParentescoID) == "-1") { comTitular = true; break; }
                }

                if (!comTitular)
                {
                    ucCPM.SetaMsg("__semTitular", "Nenhum titular encontrado");
                    return false;
                }
            }

            return true;
        }

        void CarregaAdicionais()
        {
            Conferencia conferencia = base.ConferenciaCorrente;
            if (conferencia == null) { Response.Redirect("conferenciaLista.aspx"); }

            IList<Adicional> adicionais = Adicional.CarregarPorOperadoraID(base.ConferenciaCorrente.OperadoraID);

            gridAdicionais.DataSource = adicionais;
            gridAdicionais.DataBind();
            if (gridAdicionais.DataSource == null) { litNenhumAdicional.Text = "<i>(nenhum)</i>"; }
            else { litNenhumAdicional.Text = ""; }

            //gridAdicionaisTitular.DataSource = adicionais;
            //gridAdicionaisTitular.DataBind();
            //if (gridAdicionaisTitular.DataSource == null) { litNenhumAdicionalTitular.Text = "<i>(nenhum)</i>"; }
            //else { litNenhumAdicionalTitular.Text = ""; }
        }

        void CarregaParentescos()
        {
            Conferencia conferencia = base.ConferenciaCorrente;

            if (conferencia == null) { Response.Redirect("conferenciaLista.aspx"); }

            cboParentesco.Items.Clear();
            cboParentesco.DataValueField = "ID";
            cboParentesco.DataTextField = "ParentescoDescricao";
            cboParentesco.DataSource = ContratoADMParentescoAgregado.Carregar(conferencia.ContratoAdmID, Parentesco.eTipo.Indeterminado);
            cboParentesco.DataBind();

            cboParentesco.Items.Insert(0, new ListItem("TITULAR", "-1"));
        }

        void LeConferencia()
        {
            Conferencia conferencia = base.ConferenciaCorrente;
            if (conferencia == null) { Response.Redirect("~/admin/conferenciaLista.aspx"); }

            txtCep.Text = conferencia.CEP;
            //txtCpf.Text = conferencia.TitularCPF;
            //txtNomeTitular.Text = conferencia.TitularNome;

            //if (conferencia.TitularDataNascimento != DateTime.MinValue)
            //    txtDataNascimentoTitular.Text = conferencia.TitularDataNascimento.ToString("dd/MM/yyyy");

            //if (conferencia.AdicionalIDs != null)
            //{
            //    Adicional adicional = null;
            //    foreach (String id in conferencia.AdicionalIDs)
            //    {
            //        for (Int32 i = 0; i < gridAdicionaisTitular.Rows.Count; i++)
            //        {
            //            if (id == Convert.ToString(gridAdicionaisTitular.DataKeys[i].Value))
            //            {
            //                adicional = new Adicional(id);
            //                adicional.Carregar();
            //                ((CheckBox)gridAdicionaisTitular.Rows[i].Cells[1].Controls[1]).Checked = true;
            //                this.AddAdicionalParaTodaProposta(adicional);
            //                break;
            //            }
            //        }
            //    }
            //}

            this.beneficiariosAdicionados = (List<ConferenciaBeneficiario>)ConferenciaBeneficiario.Carregar(conferencia.ID);
            if (this.beneficiariosAdicionados == null) { this.beneficiariosAdicionados = new List<ConferenciaBeneficiario>(); }

            if (conferencia.TitularDataNascimento > DateTime.MinValue)
            {
                ConferenciaBeneficiario cb = new ConferenciaBeneficiario();
                cb.AdicionalIDs = conferencia.AdicionalIDs;
                cb.Altura = conferencia.TitularAltura;
                cb.BeneficiarioID = conferencia.Titular_BeneficiarioID;
                cb.ConferenciaID = conferencia.ID;
                cb.CPF = conferencia.TitularCPF;
                cb.Nome = conferencia.TitularNome;
                cb.DataNascimento = conferencia.TitularDataNascimento;
                cb.ParentescoDescricao = "TITULAR";
                cb.ParentescoID = "-1";
                cb.PropostaData = conferencia.PropostaData;
                cb.Peso = conferencia.TitularPeso;
                cb.Valor = conferencia.TitularValor;
                this.beneficiariosAdicionados.Insert(0, cb);
            }

            this.ExibeBeneficiariosAdicionados();

            //seta tb os adicionais que valem para toda proposta
            if (this.beneficiariosAdicionados != null)
            {
                IList<Adicional> lista = null;
                foreach (ConferenciaBeneficiario beneficiario in this.beneficiariosAdicionados)
                {
                    if (beneficiario.AdicionalIDs == null || beneficiario.AdicionalIDs.Count == 0) { continue; }
                    lista = Adicional.Carregar(beneficiario.AdicionalIDs.ToArray());
                    if (lista != null)
                    {
                        foreach (Adicional adicional in lista)
                        {
                            this.AddAdicionalParaTodaProposta(adicional);
                        }
                    }
                }
            }

            this.ExibeTotal();
        }

        void ExibeTotal()
        {
            Decimal total = 0;
            if (beneficiariosAdicionados != null)
            {
                foreach (ConferenciaBeneficiario cb in beneficiariosAdicionados) { total += cb.Valor; }
            }
            litTotal.Text = "<b>Total: </b>" + total.ToString("C");
        }

        /// <summary>
        /// Se for um adicional que vale para toda proposta, adiciona na coleção apropriada
        /// para poder adicionar automaticamente nos próximos beneficiários
        /// </summary>
        void AddAdicionalParaTodaProposta(Adicional adicional)
        {
            if (adicional.ParaTodaProposta)
            {
                #region lógica para adicionais para toda proposta

                if (this.adicionaisParaTodos == null) { adicionaisParaTodos = new List<Adicional>(); }
                Boolean jaAdicionado = false;
                foreach (Adicional _ad in adicionaisParaTodos)
                {
                    if (Convert.ToInt32(_ad.ID) == Convert.ToInt32(adicional.ID))
                    {
                        jaAdicionado = true;
                        break;
                    }
                }

                if (!jaAdicionado) { adicionaisParaTodos.Add(adicional); }

                #endregion
            }
        }

        void PersisteConferencia()
        {
            Conferencia conferencia = base.ConferenciaCorrente;
            conferencia.CEP = txtCep.Text;
            //conferencia.TitularCPF = txtCpf.Text;
            //conferencia.TitularDataNascimento = base.CStringToDateTime(txtDataNascimentoTitular.Text);
            //conferencia.TitularNome = txtNomeTitular.Text;

            #region Adicionais Titular - comentado 

            //conferencia.AdicionalIDs = new List<String>();
            //Adicional adicional = null;
            //for (int i = 0; i < gridAdicionaisTitular.Rows.Count; i++)
            //{
            //    if (((CheckBox)gridAdicionaisTitular.Rows[i].Cells[1].Controls[1]).Checked)
            //    {
            //        conferencia.AdicionalIDs.Add(Convert.ToString(gridAdicionaisTitular.DataKeys[i].Value));
            //        adicional = new Adicional(gridAdicionaisTitular.DataKeys[i].Value);
            //        adicional.Carregar();

            //        this.AddAdicionalParaTodaProposta(adicional);
            //    }
            //}

            //if (this.adicionaisParaTodos != null)
            //{
            //    #region lógica para adicionar adicionais que sejam para a proposta toda 

            //    foreach (Adicional _ad in this.adicionaisParaTodos)
            //    {
            //        Boolean jaAdicionado = false;

            //        foreach (String _id in conferencia.AdicionalIDs)
            //        {
            //            if (Convert.ToString(_ad.ID) == _id)
            //            {
            //                jaAdicionado = true;
            //                break;
            //            }
            //        }

            //        if (!jaAdicionado) { conferencia.AdicionalIDs.Add(Convert.ToString(_ad.ID)); }
            //    }
            //    #endregion
            //}

            #endregion

            conferencia.Salvar();

            if (this.beneficiariosAdicionados != null && this.beneficiariosAdicionados.Count > 0)
            {
                //TODO: checar se não falta nenhum adicional para adicionar aos dependentes
                ConferenciaBeneficiario.Salvar(this.beneficiariosAdicionados);
            }
        }

        void ExibeBeneficiariosAdicionados()
        {
            //if (beneficiariosAdicionados != null && beneficiariosAdicionados.Count > 0)
            //{
            //    for(int i = 0; i < beneficiariosAdicionados.Count; i++)
            //    {
            //        if (beneficiariosAdicionados[i].CPF.Split('_').Length == 0)
            //        {
            //            beneficiariosAdicionados[i].CPF += "_" + i.ToString();
            //        }
            //    }
            //}

            gridAdicionados.DataSource = this.beneficiariosAdicionados;
            gridAdicionados.DataBind();

            if (this.beneficiariosAdicionados != null && this.beneficiariosAdicionados.Count > 0)
                tblAdicionados.Visible = true;
            else
                tblAdicionados.Visible = false;
        }

        //protected void gridBeneficiarios_RowCommand(Object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName == "sel")
        //    {
        //        Int32 indice = Convert.ToInt32(e.CommandArgument);
        //        Int32 id = Convert.ToInt32(gridBeneficiarios.DataKeys[indice].Value);
        //        trLista.Visible = false;
        //        trNome.Visible = true;

        //        if (id == -1)
        //        {
        //            txtNomeTitular.Text = "";
        //            txtNomeTitular.Focus();
        //            base.ConferenciaCorrente.Titular_BeneficiarioID = null;
        //        }
        //        else
        //        {
        //            base.ConferenciaCorrente.Titular_BeneficiarioID = id;
        //            txtNomeTitular.Text = ((LinkButton)gridBeneficiarios.Rows[indice].Cells[0].Controls[0]).Text;
        //            Beneficiario titular = new Beneficiario(id);
        //            titular.Carregar();
        //            if (titular.DataNascimento != DateTime.MinValue)
        //            {
        //                txtDataNascimentoTitular.Text = titular.DataNascimento.ToString("dd/MM/yyyy");
        //                txtCep.Focus();
        //            }
        //            else
        //            {
        //                txtDataNascimentoTitular.Text = "";
        //                txtDataNascimentoTitular.Focus();
        //            }
        //        }

        //        gridBeneficiarios.DataSource = null;
        //        gridBeneficiarios.DataBind();
        //    }
        //}

        protected void gridBeneficiariosDEPENDENTES_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "sel")
            {
                Int32 indice = Convert.ToInt32(e.CommandArgument);
                Int32 id = Convert.ToInt32(gridBeneficiariosDependentes.DataKeys[indice].Value);
                trListaDependentes.Visible = false;

                if (id == -1)
                {
                    txtNomeDependente.Text = "";
                    //txtNomeDependente.Focus();
                    //base.ConferenciaCorrente.Titular_BeneficiarioID = null;
                    this.DependenteID = null;
                    //trNomeDependente.Visible = false;
                    //txtNomeDependente.ReadOnly = true;
                    txtDataNascimento.Focus();
                }
                else
                {
                    this.DependenteID = id;
                    txtNomeDependente.Text = ((LinkButton)gridBeneficiariosDependentes.Rows[indice].Cells[0].Controls[0]).Text;
                    Beneficiario dependente = new Beneficiario(id);
                    dependente.Carregar();
                    //trNomeDependente.Visible = true;
                    txtNomeDependente.ReadOnly = false;

                    if (dependente.DataNascimento != DateTime.MinValue)
                    {
                        txtDataNascimento.Text = dependente.DataNascimento.ToString("dd/MM/yyyy");
                        cboParentesco.Focus();
                    }
                    else
                    {
                        txtDataNascimento.Text = "";
                        txtDataNascimento.Focus();
                    }

                    //ao adicionar, setar esses valores no obj ConferenciaBeneficiario
                    //txtPeso.Text = dependente.Peso.ToString("N2");
                    //txtAltura.Text = dependente.Altura.ToString("N2");
                }

                gridBeneficiariosDependentes.DataSource = null;
                gridBeneficiariosDependentes.DataBind();
                //cmdAdicionarBeneficiario.Enabled = true; removido por solicitacao
            }
        }

        //protected void cmdValidarCpf_Click(Object sender, EventArgs e)
        //{
        //    this.ValidaCPF();
        //}

        protected void cmdValidarCpfDependente_Click(Object sender, EventArgs e)
        {
            if (cmdValidarCpfDependente.Text == "validar")
            {
                if (txtCpfDependente.Text.Trim() == "") { Alerta(null, this, "_cpfBranco", "Informe o cpf."); txtCpfDependente.Focus(); return; }
                if (beneficiariosAdicionados != null)
                {
                    foreach (ConferenciaBeneficiario cb in beneficiariosAdicionados)
                    {
                        if (cb.CPF == txtCpfDependente.Text)
                        {
                            base.Alerta(null, this, "__errCpfEx", "CPF já foi informado.");
                            txtCpfDependente.Focus();
                            return;
                        }
                    }
                }

                if (this.ValidaCPF_DEPENDENTE())
                {
                    cmdValidarCpfDependente.Text = "limpar";
                    //cmdAdicionarBeneficiario.Enabled = true;
                    //txtCpfDependente.ReadOnly = true;
                }
            }
            else
            {
                this.ResetaCamposDeAdicionarDependentes();
            }
        }

        void ResetaCamposDeAdicionarDependentes()
        {
            cmdValidarCpfDependente.Text = "validar";
            //cmdAdicionarBeneficiario.Enabled = false; removido por solicitacao
            txtCpfDependente.ReadOnly = false;
            trListaDependentes.Visible = false;
            this.DependenteID = null;
            txtNomeDependente.Text = "";
            //trNomeDependente.Visible = false;
            txtNomeDependente.ReadOnly = false;
            txtDataNascimento.Text = "";
            txtPeso.Text = ""; txtAltura.Text = "";
            txtCpfDependente.Text = "";
            txtCpfDependente.Focus();
        }

        protected void cmdAdicionarBeneficiario_Click(Object sender, EventArgs e)
        {
            #region validacoes

            Conferencia conferencia = base.ConferenciaCorrente;
            if (conferencia == null) { Response.Redirect("conferenciaLista.aspx"); }

            //if (!UIHelper.ValidaCpf(txtCpfDependente.Text)) { base.Alerta(null, this, "__errCPF", "CPF inválido."); txtCpfDependente.Focus(); return; }

            DateTime dataNascimento = new DateTime();
            if (!UIHelper.TyParseToDateTime(txtDataNascimento.Text, out dataNascimento))
            {
                base.Alerta(null, this, "__errNasc", "Data de nascimento inválida.");
                txtDataNascimento.Focus();
                return;
            }

            if (cboParentesco.SelectedValue == "-1" && txtNomeDependente.Text.Trim() == "")
            {
                base.Alerta(null, this, "__errNm", "O nome do titular é obrigatório.");
                txtNomeDependente.Focus();
                return;
            }

            if (cboParentesco.SelectedValue == "-1")
            {
                for (int i = 0; i < gridAdicionados.Rows.Count; i++)
                {
                    if (gridAdicionados.Rows[i].Cells[1].Text == "TITULAR")
                    {
                        base.Alerta(null, this, "__errTit", "Já há um titular na proposta.");
                        cboParentesco.Focus();
                        return;
                    }
                }
            }

            if (txtCpfDependente.Text != "" && !UIHelper.ValidaCpf(txtCpfDependente.Text))
            {
                base.Alerta(null, this, "__errCpf", "CPF inválido.");
                txtCpfDependente.Focus();
                return;
            }

            if (txtPeso.Text == "")
            {
                //base.Alerta(null, this, "__errPs", "Informe o peso.");
                //txtPeso.Focus();
                //return;
                txtPeso.Text = "0";
            }

            if (txtAltura.Text == "")
            {
                //base.Alerta(null, this, "__errAl", "Informe a altura.");
                //txtAltura.Focus();
                //return;
                txtAltura.Text = "0";
            }

            if (cboParentesco.SelectedItem.Text.ToUpper() == "TITULAR" && gridAdicionados.Rows.Count > 0)
            {
                for (int i = 0; i < gridAdicionados.Rows.Count; i++)
                {
                    if (gridAdicionados.Rows[i].Cells[0].Text.ToUpper() == "TITULAR")
                    {
                        base.Alerta(null, this, "__errTit", "Já há um titular para a proposta.");
                        return;
                    }
                }
            }

            #endregion

            //String key = "";
            //if (beneficiariosAdicionados == null || beneficiariosAdicionados.Count == 0)
            //    key = "_0";
            //else if (beneficiariosAdicionados[beneficiariosAdicionados.Count - 1].CPF.Split('_').Length > 0)
            //    key = "_" + Convert.ToInt32(beneficiariosAdicionados[beneficiariosAdicionados.Count - 1].CPF.Split('_')[1]) + 1;

            ConferenciaBeneficiario beneficiario = null;

            beneficiario                = new ConferenciaBeneficiario();
            beneficiario.Altura         = base.CToDecimal(txtAltura.Text);
            beneficiario.ConferenciaID  = base.ConferenciaCorrente.ID;
            beneficiario.CPF            = txtCpfDependente.Text;// +key;
            beneficiario.DataNascimento = dataNascimento;
            beneficiario.Peso           = base.CToDecimal(txtPeso.Text);
            beneficiario.PropostaData   = base.ConferenciaCorrente.PropostaData;
            beneficiario.ParentescoID   = cboParentesco.SelectedValue;
            beneficiario.ParentescoDescricao = cboParentesco.SelectedItem.Text;
            beneficiario.Nome = txtNomeDependente.Text;
            if (cboEstadoCivil.Items.Count > 0)
                beneficiario.EstadoCivilID = cboEstadoCivil.SelectedValue;

            beneficiario.DataCasamento = base.CStringToDateTime(txtDataCasamento.Text);

            if (txtCpfDependente.Text != "")
            {
                IList<Beneficiario> ret = Beneficiario.CarregarPorParametro(txtCpfDependente.Text, dataNascimento);
                if (ret != null && ret.Count > 0)
                {
                    beneficiario.BeneficiarioID = ret[0].ID;
                }
                ret = null;
            }

            Adicional adicional = null;

            int _beneficiarioIdade = Beneficiario.CalculaIdade(beneficiario.DataNascimento, beneficiario.PropostaData);

            if (_beneficiarioIdade >= 18 && !UIHelper.ValidaCpf(txtCpfDependente.Text))
            {
                base.Alerta(null, this, "__errCpfM", "É obrigatório informar o CPF para maiores de idade.");
                txtCpfDependente.Focus();
                return;
            }

            Decimal valor = TabelaValor.CalculaValor(null, _beneficiarioIdade, 
                conferencia.ContratoAdmID, conferencia.PlanoID, 
                ((Contrato.eTipoAcomodacao)Convert.ToInt32(conferencia.AcomodacaoID)),
                conferencia.Admissao, null);

            if (valor == -1) { valor = 0; }

            //Calcula a taxa por vida (taxa associativa)
            //EstipulanteTaxa taxa = EstipulanteTaxa.CarregarVigente(base.ConferenciaCorrente.EstipulanteID);
            //if (taxa != null && ((EstipulanteTaxa.eTipoTaxa)taxa.TipoTaxa) == EstipulanteTaxa.eTipoTaxa.PorVida)
            //{
            //    valor += taxa.Valor;
            //}

            for (int i = 0; i < gridAdicionais.Rows.Count; i++)
            {
                if (((CheckBox)gridAdicionais.Rows[i].Cells[1].Controls[1]).Checked)
                {
                    beneficiario.AdicionalIDs.Add(Convert.ToString(gridAdicionais.DataKeys[i].Value));
                    adicional = new Adicional(gridAdicionais.DataKeys[i].Value);
                    adicional.Carregar();

                    this.AddAdicionalParaTodaProposta(adicional);
                }
            }

            if (this.adicionaisParaTodos != null)
            {
                #region lógica para adicionar adicionais que sejam para a proposta toda

                foreach (Adicional _ad in this.adicionaisParaTodos)
                {
                    Boolean jaAdicionado = false;

                    foreach (String _id in beneficiario.AdicionalIDs)
                    {
                        if (Convert.ToString(_ad.ID) == _id)
                        {
                            jaAdicionado = true;
                            break;
                        }
                    }

                    if (!jaAdicionado) { beneficiario.AdicionalIDs.Add(Convert.ToString(_ad.ID)); }
                }
                #endregion
            }

            foreach (String _id in beneficiario.AdicionalIDs)
            {
                valor += Adicional.CalculaValor(_id, null, _beneficiarioIdade);
            }

            beneficiario.Valor = valor;

            if (beneficiariosAdicionados == null) { beneficiariosAdicionados = new List<ConferenciaBeneficiario>(); }
            //else
            //{
            //    //checa se o cpf ja está em uso
            //    foreach (ConferenciaBeneficiario cb in beneficiariosAdicionados)
            //    {
            //        if (cb.CPF == beneficiario.CPF) 
            //        {
            //            base.Alerta(null, this, "__errCpfEx", "CPF já foi informado.");
            //            return;
            //        }
            //    }
            //}
            beneficiariosAdicionados.Add(beneficiario);
            this.ExibeBeneficiariosAdicionados();
            this.DependenteID = null;

            //Se titular, preenche o objeto conferencia
            if (cboParentesco.SelectedValue == "-1")
            {
                conferencia.Titular_BeneficiarioID  = beneficiario.BeneficiarioID;
                conferencia.TitularCPF              = beneficiario.CPF;
                conferencia.TitularNome             = txtNomeDependente.Text;
                conferencia.TitularDataNascimento   = beneficiario.DataNascimento;
                conferencia.TitularNome             = txtNomeDependente.Text;
                conferencia.AdicionalIDs            = beneficiario.AdicionalIDs;
                conferencia.TitularAltura           = beneficiario.Altura;
                conferencia.TitularPeso             = beneficiario.Peso;
                conferencia.TitularValor            = beneficiario.Valor;

                base.ConferenciaCorrente            = conferencia;
            }

            this.ResetaCamposDeAdicionarDependentes();

            //cmdAdicionarBeneficiario.Enabled = false; removido por solicitacao

            this.ExibeTotal();
        }

        protected void cmdValidarCep_Click(Object sender, EventArgs e)
        {
            this.ValidaCEP();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            //this.PersisteConferencia();
            Response.Redirect("conferencia1.aspx");
        }

        protected void cmdProximo_Click(Object sender, EventArgs e)
        {
            //if (!this.ValidaCPF()) { return; }
            if (!this.ValidaCEP()) { /* return; */ }
            if (!this.ValidaProposta()) { return; }
            this.PersisteConferencia();
            Response.Redirect("conferencia3.aspx");
        }

        protected void gridAdicionados_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                Int32 index = Convert.ToInt32(e.CommandArgument);
                if (this.beneficiariosAdicionados[index].ID != null)
                {
                    this.beneficiariosAdicionados[index].Remover();
                }

                this.beneficiariosAdicionados.RemoveAt(index);
                this.ExibeBeneficiariosAdicionados();

                this.ExibeTotal();

                //TODO: e os adicionais para toda proposta do TITULAR ?
                if (gridAdicionados.Rows.Count == 0) { this.adicionaisParaTodos = null; }
            }
        }

        protected void gridAdicionados_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente excluir o beneficiário?");
        }
    }
}
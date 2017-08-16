﻿namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Data;
    using System.Configuration;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using System.IO;

    internal class CobrancaConfig
    {
        CobrancaConfig() { }

        /// <summary>
        /// Número agencia, C/C do cedente e DV do cedente
        /// </summary>
        public static readonly String C006 = ConfigurationManager.AppSettings["UniC006"];
        /// <summary>
        /// Codigo (grupo) empresarial com 4 digitos
        /// </summary>
        public static readonly String C007 = ConfigurationManager.AppSettings["UniC007"];
        /// <summary>
        /// Tipo de formulário (2 para bloqueto)
        /// </summary>
        public static readonly String C008 = ConfigurationManager.AppSettings["UniC008"];
        /// <summary>
        /// Tipo de crítica: 0=cliente informa DV ; 1=cliente não informa
        /// </summary>
        public static readonly String C009 = ConfigurationManager.AppSettings["UniC009"];
        /// <summary>
        /// Tipo de postagem
        /// </summary>
        public static readonly String C010 = ConfigurationManager.AppSettings["UniC010"];
        /// <summary>
        /// Codigo empresarial com 7 digitos
        /// </summary>
        public static readonly String C011 = ConfigurationManager.AppSettings["UniC011"];
        /// <summary>
        /// Codigo da mensagem
        /// </summary>
        public static readonly String C012 = ConfigurationManager.AppSettings["UniC012"];
        /// <summary>
        /// Número agencia depositaria
        /// </summary>
        public static readonly String C025 = ConfigurationManager.AppSettings["UniC025"];
        /// <summary>
        /// Número agencia, C/C do cedente e DV (11 digitos)
        /// </summary>
        public static readonly String C026 = ConfigurationManager.AppSettings["UniC026"];
        /// <summary>
        /// Juros de mora por dia
        /// </summary>
        public static readonly String C039 = ConfigurationManager.AppSettings["UniC039"];

        public static readonly String C044a = "1";
        /// <summary>
        /// Espécie de documento
        /// </summary>
        public static readonly String C044 = ConfigurationManager.AppSettings["UniC044"];
        /// <summary>
        /// Identificacao de aceite do titulo.
        /// </summary>
        public static readonly String C045 = ConfigurationManager.AppSettings["UniC045"];
        /// <summary>
        /// Código da carteira.
        /// </summary>
        public static readonly String C047 = ConfigurationManager.AppSettings["UniC047"];

        public static readonly String MultaPercentual = "0,02";
    }

    [DBTable("cobranca")]
    public class Cobranca : EntityBase, IPersisteableEntity 
    {
        public class UI
        {
            public static void FillComboCarteira(System.Web.UI.WebControls.DropDownList cbo)
            {
                cbo.Items.Clear();
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Unibanco", "0"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Itau sem registro", "1"));
            }

            public static void FillComboFormato(System.Web.UI.WebControls.DropDownList cbo)
            {
                cbo.Items.Clear();
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Formato antigo", "0"));
                cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Formato novo (Carnê)", "1"));
            }
        }

        public static string ConvenioUBRASP  = "2887755";
        public static string Convenio2UBRASP = "2896830"; //

        public enum eTipoBanco : int
        {
            Unibanco,
            Itau,
            BancoDoBrasil,
            Santander
        }

        public enum eTipo : int 
        {
            Normal,
            Complementar,
            Dupla,
            Indefinido,
            Parcelamento,
            DiferencaUbraSP,
            BoletoUbraspSP
        }

        public enum eCarteira : int 
        {
            /// <summary>
            /// 0
            /// </summary>
            Unibanco,
            /// <summary>
            /// 1
            /// </summary>
            ItauSemRegistro
        }

        #region Fields 

        Object _id;
        Object _propostaId;
        Decimal _valor;
        Decimal _valorNominal;
        DateTime _vencimento;
        DateTime _vencimentoIsencaoJuro;
        DateTime _dataCriacao;
        DateTime _dataBaixaAuto;
        Boolean _pago;
        DateTime _dataPagto;
        Decimal _valorPagto;
        Object _arquivoIdUltimoEnvio;
        Int32 _parcela;
        Int32 _tipo;
        Int32 _tipoTemp;
        Object _cobrancaRefId;
        Boolean _comissaoPaga;
        Boolean _cancelada;
        Boolean _dataVencimentoForcada;
        String _nossoNumero;

        Int32 _carteira;

        String _contratoCodCobranca;
        Object _operadoraId;
        String _contratoNumero;
        String _contratoTitularNome;
        string _contratoTitularCpf;
        Object _contratoEnderecoCobrancaId;
        String _filialNome;
        String _operadoraNome;
        String _estipulanteNome;
        Object _contratoBeneficiarioId;
        String _contratoBeneficiarioEmail;

        Object _composicaoBeneficiarioId;
        Int32 _composicaoTipo;
        Decimal _composicaoValor;
        String _composicaoResumo;

        Object _contratoAdmId;
        DateTime _contratoDataAdmissao;

        Object _headerParcId;
        Object _headerItemId;
        String _itemParcObs;

        Decimal _jurosRS;
        Decimal _multaRS;
        Decimal _amortizacao;

        //String _obsGerais;

        ArquivoRemessaCriterio _criterio;
        #endregion

        #region Properties 

        [DBFieldInfo("cobranca_id", FieldType.PrimaryKeyAndIdentity)]
        //[DBFieldInfo("cobranca_id", FieldType.Single)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("cobranca_propostaId", FieldType.Single)]
        public Object PropostaID
        {
            get { return _propostaId; }
            set { _propostaId = value; }
        }

        /// <summary>
        /// Quando cobrança dupla, esta propriedade guardará o ID da cobrança que compões esta cobrança.
        /// </summary>
        [DBFieldInfo("cobranca_cobrancaRefId", FieldType.Single)]
        public Object CobrancaRefID
        {
            get { return _cobrancaRefId; }
            set { _cobrancaRefId= value; }
        }

        [DBFieldInfo("cobranca_arquivoUltimoEnvioId", FieldType.Single)]
        public Object ArquivoIDUltimoEnvio
        {
            get { return _arquivoIdUltimoEnvio; }
            set { _arquivoIdUltimoEnvio= value; }
        }

        [DBFieldInfo("cobranca_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor= value; }
        }

        /// <summary>
        /// Usado em cobranças duplas, qdo necessário, para guardar o valor original da cobrança antes de ser dupla
        /// </summary>
        [DBFieldInfo("cobranca_valorNominal", FieldType.Single)]
        public Decimal ValorNominal
        {
            get { return _valorNominal; }
            set { _valorNominal= value; }
        }

        [DBFieldInfo("cobranca_dataVencimentoIsencaoJuro", FieldType.Single)]
        public DateTime DataVencimentoISENCAOJURO
        {
            get { return _vencimentoIsencaoJuro; }
            set { _vencimentoIsencaoJuro= value; }
        }

        [DBFieldInfo("cobranca_dataVencimento", FieldType.Single)]
        public DateTime DataVencimento
        {
            get { return _vencimento; }
            set { _vencimento= value; }
        }

        [DBFieldInfo("cobranca_dataCriacao", FieldType.Single)]
        public DateTime DataCriacao
        {
            get { return _dataCriacao; }
            set { _dataCriacao= value; }
        }

        [DBFieldInfo("cobranca_dataBaixaAuto", FieldType.Single)]
        public DateTime DataBaixaAutomatica
        {
            get { return _dataBaixaAuto; }
            set { _dataBaixaAuto= value; }
        }

        [DBFieldInfo("cobranca_pago", FieldType.Single)]
        public Boolean Pago
        {
            get { return _pago; }
            set { _pago= value; }
        }

        [DBFieldInfo("cobranca_dataPagto", FieldType.Single)]
        public DateTime DataPgto
        {
            get { return _dataPagto; }
            set { _dataPagto= value; }
        }

        [DBFieldInfo("cobranca_valorPagto", FieldType.Single)]
        public Decimal ValorPgto
        {
            get { return _valorPagto; }
            set { _valorPagto= value; }
        }

        [DBFieldInfo("cobranca_parcela", FieldType.Single)]
        public Int32 Parcela
        {
            get { return _parcela; }
            set { _parcela= value; }
        }

        [DBFieldInfo("cobranca_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [DBFieldInfo("cobranca_tipoTemp", FieldType.Single)]
        public Int32 TipoTemp
        {
            get { return _tipoTemp; }
            set { _tipoTemp = value; }
        }

        [DBFieldInfo("cobranca_instrucoes", FieldType.Single)]
        public string Instrucoes
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se a comissao sobre esta cobrança já foi paga.
        /// </summary>
        [DBFieldInfo("cobranca_comissaoPaga", FieldType.Single)]
        public Boolean ComissaoPaga
        {
            get { return _comissaoPaga; }
            set { _comissaoPaga= value; }
        }

        [DBFieldInfo("cobranca_cancelada", FieldType.Single)]
        public Boolean Cancelada
        {
            get { return _cancelada; }
            set { _cancelada= value; }
        }

        [DBFieldInfo("cobranca_dataVencimentoForcada", FieldType.Single)]
        public Boolean DataVencimentoForcada
        {
            get { return _dataVencimentoForcada; }
            set { _dataVencimentoForcada = value; }
        }

        [DBFieldInfo("cobranca_nossoNumero", FieldType.Single)]
        public String NossoNumero
        {
            get { return _nossoNumero; }
            set { _nossoNumero= value; }
        }

        [DBFieldInfo("cobranca_carteira", FieldType.Single)]
        public Int32 Carteira
        {
            get { return _carteira; }
            set { _carteira= value; }
        }
        [DBFieldInfo("cobranca_banco", FieldType.Single)]
        public Int32 Banco
        {
            get;
            set;
        }

        [Joinned("contrato_codcobranca")]
        public String ContratoCodCobranca
        {
            get { return _contratoCodCobranca; }
            set { _contratoCodCobranca= value; }
        }

        [Joinned("operadora_id")]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        /// <summary>
        /// O nome do titular da proposta.
        /// </summary>
        [Joinned("beneficiario_nome")]
        public String ContratoTitularNome
        {
            get { return _contratoTitularNome; }
            set { _contratoTitularNome= value; }
        }

        [Joinned("beneficiario_cpf")]
        public String ContratoTitularCPF
        {
            get { return _contratoTitularCpf; }
            set { _contratoTitularCpf= value; }
        }

        /// <summary>
        /// O número da proposta, do contrato impresso.
        /// </summary>
        [Joinned("contrato_numero")]
        public String ContratoNumero
        {
            get { return _contratoNumero; }
            set { _contratoNumero= value; }
        }

        [Joinned("contrato_enderecoCobrancaId")]
        public Object ContratoEnderecoCobrancaID
        {
            get { return _contratoEnderecoCobrancaId; }
            set { _contratoEnderecoCobrancaId= value; }
        }

        [Joinned("filial_nome")]
        public String FilialNome
        {
            get { return _filialNome; }
            set { _filialNome= value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraNome
        {
            get { return _operadoraNome; }
            set { _operadoraNome= value; }
        }

        [Joinned("estipulante_descricao")]
        public String EstipulanteNome
        {
            get { return _estipulanteNome; }
            set { _estipulanteNome= value; }
        }

        [Joinned("beneficiario_id")]
        public Object BeneficiarioId
        {
            get { return _contratoBeneficiarioId; }
            set { _contratoBeneficiarioId = value; }
        }

        [Joinned("beneficiario_email")]
        public String BeneficiarioEmail
        {
            get { return _contratoBeneficiarioEmail; }
            set { _contratoBeneficiarioEmail = value; }
        }

        [Joinned("beneficiario_nome")]
        public String BeneficiarioNome
        {
            get;
            set;
        }

        [Joinned("cobrancacomp_beneficiarioId")]
        public Object ComposicaoBeneficiarioID
        {
            get { return _composicaoBeneficiarioId; }
            set { _composicaoBeneficiarioId = value; }
        }

        [Joinned("cobrancacomp_tipo")]
        public Int32 ComposicaoTipo
        {
            get { return _composicaoTipo; }
            set { _composicaoTipo= value; }
        }

        [Joinned("cobrancacomp_valor")]
        public Decimal ComposicaoValor
        {
            get { return _composicaoValor; }
            set { _composicaoValor= value; }
        }

        [Joinned("contratoadm_id")]
        public Object ContratoAdmID
        {
            get { return _contratoAdmId; }
            set { _contratoAdmId= value; }
        }

        [Joinned("contrato_admissao")]
        public DateTime ContratoDataAdmissao
        {
            get { return _contratoDataAdmissao; }
            set { _contratoDataAdmissao = value; }
        }

        /// <summary>
        /// Para parcelas originais.
        /// </summary>
        [Joinned("parccob_headerId")]
        public Object HeaderParcID
        {
            get { return _headerParcId; }
            set { _headerParcId= value; }
        }

        /// <summary>
        /// Para parcelas geradas para o parcelamento.
        /// </summary>
        [Joinned("parcitem_headerId")]
        public Object HeaderItemID
        {
            get { return _headerItemId; }
            set { _headerItemId= value; }
        }

        /// <summary>
        /// Observação cadastrada durante a criação da parcela de negociação
        /// </summary>
        [Joinned("parcitem_obs")]
        public String ItemParcelamentoOBS
        {
            get { return _itemParcObs; }
            set { _itemParcObs= value; }
        }

        [Joinned("endereco_id")]
        public object TitularEnderecoID
        {
            get;
            set;
        }
        [Joinned("endereco_logradouro")]
        public string TitularEnderecoLogradouro
        {
            get;
            set;
        }
        [Joinned("endereco_numero")]
        public string TitularEnderecoNumero
        {
            get;
            set;
        }
        [Joinned("endereco_complemento")]
        public string TitularEnderecoComplemento
        {
            get;
            set;
        }
        [Joinned("endereco_bairro")]
        public string TitularEnderecoBairro
        {
            get;
            set;
        }
        [Joinned("endereco_cidade")]
        public string TitularEnderecoCidade
        {
            get;
            set;
        }
        [Joinned("endereco_uf")]
        public string TitularEnderecoUF
        {
            get;
            set;
        }
        [Joinned("endereco_cep")]
        public string TitularEnderecoCEP
        {
            get;
            set;
        }

        public String ComposicaoResumo
        {
            get { return _composicaoResumo; }
        }

        public String STRNossoNumero
        {
            get
            {
                if (this._dataCriacao.Year >= 2013)
                    return geraNossoNumeroItau();
                else
                    return geraNossoNumeroUnibanco();
            }
        }

        public String strPago
        {
            get { if (_pago) { return "Sim"; } else { return "Não"; } }
        }

        public String strDataPago
        {
            get
            {
                if (_dataPagto != DateTime.MinValue)
                    return _dataPagto.ToString("dd/MM/yyyy");
                else
                    return "";
            }
        }

        public String strEnviado
        {
            get { if (_arquivoIdUltimoEnvio == null) { return "Não"; } else { return "Sim"; } }
        }

        public static Boolean NossoNumeroITAU
        {
            get
            {
                String tipo = ConfigurationManager.AppSettings["tipoNossoNumero"];
                if (String.IsNullOrEmpty(tipo)) { return false; }
                if (tipo.ToLower() == "itau")
                    return true;
                else
                    return false;
            }
        }

        public ArquivoRemessaCriterio Criterio
        {
            get { return _criterio; }
            set { _criterio= value; }
        }

        /// <summary>
        /// Juros incididos sobre a parcela, em R$.
        /// </summary>
        public Decimal JurosRS
        {
            get { return _jurosRS; }
            set { _jurosRS= value; }
        }

        /// <summary>
        /// Multa sobre o atraso da parcela, em R$.
        /// </summary>
        public Decimal MultaRS
        {
            get { return _multaRS; }
            set { _multaRS= value; }
        }

        public Decimal Amortizacao
        {
            get { return _amortizacao; }
            set { _amortizacao= value; }
        }

        /// <summary>
        /// No caso do boleto itaú, essa propriedade retorna o parâmetro necessário para ser enviado 
        /// ao boletomail, utilizando a conta da PSPADRAO.
        /// </summary>
        public static String BoletoUrlCompPSPadrao
        {
            get
            {
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["boletoMailUrlParam"]))
                {
                    return String.Empty;
                }
                else
                {
                    return String.Concat("&", ConfigurationManager.AppSettings["boletoMailUrlParam"]);
                }
            }
        }

        /// <summary>
        /// No caso do boleto itaú, essa propriedade retorna o parâmetro necessário para ser enviado 
        /// ao boletomail, utilizando a conta da QUALICORP.
        /// </summary>
        public static String BoletoUrlCompQualicorp
        {
            get
            {
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["boletoMailUrlParamQ"]))
                {
                    return String.Empty;
                }
                else
                {
                    return String.Concat("&", ConfigurationManager.AppSettings["boletoMailUrlParamQ"]);
                }
            }
        }

        #endregion

        public Cobranca(Object id) : this() { _id = id; }
        public Cobranca() { _dataCriacao = DateTime.Now; _pago = false; _cancelada = false; _carteira = (Int32)eCarteira.Unibanco; _dataVencimentoForcada = false; }

        #region EntityBase methods 

        public void Salvar()
        {
            if (((eTipo)this._tipo) == eTipo.Dupla && this._valorNominal == 0)
            {
                if (this._cobrancaRefId != null)
                {
                    Cobranca cobrancaRef = new Cobranca(this._cobrancaRefId);
                    cobrancaRef.Carregar();
                    this._valorNominal = this._valor - cobrancaRef.Valor;
                }
                else
                {
                    Cobranca cobrancaRef = new Cobranca();
                }
            }

            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        /// <summary>
        /// Para o unibanco, gera com DV. Para o itau, gera SEM o DV.
        /// </summary>
        public String GeraNossoNumero()
        {
            //if (!Cobranca.NossoNumeroITAU)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    sb.Append(this._tipo);
            //    sb.Append(this._contratoCodCobranca.PadLeft(10, '0'));

            //    String dv = this._CalculaDVMod11(sb.ToString() + _parcela.ToString().PadLeft(3, '0'));

            //    sb.Append(_parcela.ToString().PadLeft(3, '0'));
            //    sb.Append(dv);

            //    String nossonumero = sb.ToString();
            //    sb.Remove(0, sb.Length);
            //    sb = null;
            //    return nossonumero;
            //}
            //else
            //{
                //String nossonumero = Convert.ToString(this._id).PadLeft(8, '0');
                ////nossonumero = String.Concat(nossonumero, CalculaDVMod11(nossonumero));
                //return nossonumero;

            if(this.Carteira == 17)
                return string.Concat(Cobranca.Convenio2UBRASP, Convert.ToString(this._id).PadLeft(10, '0'));
            else
                return string.Concat(Cobranca.ConvenioUBRASP, Convert.ToString(this._id).PadLeft(10, '0'));
            //}
        }
        public String GeraNossoNumeroSantander()
        {
            return string.Concat(this._id, this._CalculaDVMod11(Convert.ToString(this._id))).PadLeft(13, '0');
        }
        //public String GeraNossoNumeroSantander_Boleto()
        //{
        //    return string.Concat(this._id, this._CalculaDVMod11(Convert.ToString(this._id)));
        //}

        String geraNossoNumeroItau()
        {
            return Convert.ToString(this._id).PadLeft(8, '0');
        }
        String geraNossoNumeroUnibanco()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this._tipo);
            sb.Append(this._contratoCodCobranca.PadLeft(10, '0'));

            String dv = this._CalculaDVMod11(sb.ToString() + _parcela.ToString().PadLeft(3, '0'));

            sb.Append(_parcela.ToString().PadLeft(3, '0'));
            sb.Append(dv);

            String nossonumero = sb.ToString();
            sb.Remove(0, sb.Length);
            sb = null;
            return String.Format("{0:" +  new String('0', 12) + "}", Convert.ToInt64(nossonumero));
        }

        public String _CalculaDVMod11(Int32 tipo, String contratoCodCobranca, Int32 parcela)
        {
            StringBuilder sb = new StringBuilder();

            if (!Cobranca.NossoNumeroITAU)
            {
                sb.Append(tipo);
                sb.Append(contratoCodCobranca.PadLeft(10, '0'));
            }
            else
            {
                sb.Append(Convert.ToString(this._id).PadLeft(11, '0'));
            }

            return this._CalculaDVMod11(sb.ToString() + parcela.ToString().PadLeft(3, '0'));
        }

        public String ___CalculaDVMod11__(String nossoNumero)
        {
            Int32 fatorMult = 2;
            Int32 resultado = 0;

            char[] buffer = nossoNumero.ToCharArray();
            Array.Reverse(buffer);
            String nossoNumeroReverso = new String(buffer);

            for (int i = 0; i < nossoNumeroReverso.Length; i++)
            {
                resultado += Convert.ToInt32(nossoNumeroReverso.Substring(i, 1)) * fatorMult;
                fatorMult++;
                if (fatorMult > 9) { fatorMult = 2; }
            }

            resultado *= 10;
            resultado %= 11;
            resultado %= 10;

            return resultado.ToString();
        }

        public String _CalculaDVMod11(string seq)
        {
            int lim = 9;
            int flag = 0;

            int mult = 0;
            int total = 0;
            int pos = 1;
            //int res = 0;
            int ndig = 0;
            int nresto = 0;
            string num = string.Empty;

            mult = 1 + (seq.Length % (lim - 1));

            if (mult == 1)
                mult = lim;


            while (pos <= seq.Length)
            {
                num = Microsoft.VisualBasic.Strings.Mid(seq, pos, 1);
                total += Convert.ToInt32(num) * mult;

                mult -= 1;
                if (mult == 1)
                    mult = lim;

                pos += 1;
            }
            nresto = (total % 11);
            if (flag == 1)
                return nresto.ToString();
            else
            {
                if (nresto == 0 || nresto == 1 || nresto == 10)
                    ndig = 1;
                else
                    ndig = (11 - nresto);

                return ndig.ToString();
            }
        }

        public String CalculaDVMod10(Int32 tipo, String contratoCodCobranca, Int32 parcela)
        {
            StringBuilder sb = new StringBuilder();

            if (!Cobranca.NossoNumeroITAU)
            {
                sb.Append(tipo);
                sb.Append(contratoCodCobranca.PadLeft(10, '0'));
                return this.CalculaDVMod10(sb.ToString() + parcela.ToString().PadLeft(3, '0'));
            }
            else
            {
                sb.Append(Convert.ToString(this._id).PadLeft(8, '0'));
                return this.CalculaDVMod10(sb.ToString());
            }
        }

        public string CalculaDVMod10(String nossoNumero)
        {
            nossoNumero = String.Concat("0646", "04260", "175", nossoNumero);

            int i = 2;
            int sum = 0;
            int res = 0;
            Char[] inverse = nossoNumero.ToCharArray();
            Array.Reverse(inverse);
            String[] strarray = new String[nossoNumero.Length];

            int index = 0;
            foreach (char c in inverse)
            {
                res = Convert.ToInt32(c.ToString()) * i;
                sum += res > 9 ? (res - 9) : res;
                i = i == 2 ? 1 : 2;

                strarray[index] = res.ToString();
                index++;
            }

            sum = 0;
            Array.Reverse(strarray);

            foreach (String item in strarray)
            {
                if (item.Length == 1)
                {
                    sum += Convert.ToInt32(item);
                }
                else
                {
                    sum += Convert.ToInt32(item.Substring(0,1));
                    sum += Convert.ToInt32(item.Substring(1,1));
                }
            }

            int resto = sum % 10;
            return Convert.ToString(10 - resto);
        }

        public void LeNossoNumero(String nossoNumero)
        {
            if (!Cobranca.NossoNumeroITAU)
            {
                Cobranca.LeNossoNumero(nossoNumero, out this._tipo, out this._contratoCodCobranca, out this._parcela);
            }
            else
            {
                this._id = Convert.ToInt32(nossoNumero);
            }
        }

        public void LeNossoNumeroUNIBANCO(String nossoNumero)
        {
            Cobranca.LeNossoNumero(nossoNumero, out this._tipo, out this._contratoCodCobranca, out this._parcela);
        }

        public static void LeNossoNumero(String nossoNumero, out Int32 tipo, out String codCobranca, out Int32 parcela)
        {
            //tipo = 0; propostaId = 0; parcela = 0;
            try
            {
                tipo = Convert.ToInt32(nossoNumero.Substring(0, 1));
                codCobranca = Convert.ToInt32(nossoNumero.Substring(1, 10)).ToString();
                parcela = Convert.ToInt32(nossoNumero.Substring(11, 3));
            }
            catch
            {
                tipo = (int)Cobranca.eTipo.Indefinido;
                codCobranca = null;
                parcela = -1;
            }
        }

        #region Load methods 

        public static Cobranca CarregarPor(Object propostaId, Int32 mes, Int32 ano, Cobranca.eTipo tipo, PersistenceManager pm)
        {
            IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>("* FROM cobranca WHERE cobranca_propostaId=" + propostaId + " AND MONTH(cobranca_dataVencimento)=" + mes.ToString() + " AND YEAR(cobranca_dataVencimento)=" + ano.ToString() + " AND cobranca_tipo=" + Convert.ToInt32(tipo).ToString(), typeof(Cobranca), pm);

            if (cobrancas == null || cobrancas.Count == 0)
                return null;
            else
                return cobrancas[0];
        }
        public static Cobranca CarregarPor(Object propostaId, Int32 parcela, Int32 cobrancaTipo)
        {
            return CarregarPor(propostaId, parcela, cobrancaTipo, null);
        }
        public static Cobranca CarregarPor(Object propostaId, Int32 parcela, Int32 cobrancaTipo, PersistenceManager pm)
        {
            String tipoCond = "";
            if (((eTipo)cobrancaTipo) != eTipo.Indefinido)
            {
                tipoCond = " cobranca_tipo=" + cobrancaTipo + " AND ";
            }

            String qry = String.Concat(
                "SELECT operadora_id, filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ", 
                "       cobranca_parcela=", parcela, " AND ",
                tipoCond,
                "       cobranca_propostaId =", propostaId,
                "   ORDER BY filial_nome, estipulante_descricao, operadora_nome, cobranca_dataVencimento");

            IList<Cobranca> lista = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
            if (lista == null)
                return null;
            else if (lista.Count == 1)
                return lista[0];
            else
                return lista[0]; //return null; //throw new ApplicationException("Mais de uma cobrança foi retornada.");
        }

        public static Cobranca CarregarPorNossoNumero(string nossoNumero, PersistenceManager pm)
        {
            String qry = String.Concat(
                "SELECT operadora_id, filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                "       cobranca_nossoNumero='", nossoNumero, "'");

            IList<Cobranca> lista = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
            if (lista == null)
                return null;
            else if (lista.Count == 1)
                return lista[0];
            else
                throw new ApplicationException("Mais de uma cobrança foi retornada com o nosso número " + nossoNumero);
        }

        public static Cobranca CarregarPor(Object propostaId, DateTime vencimento, Int32 cobrancaTipo, PersistenceManager pm)
        {
            String tipoCond = "";
            if (((eTipo)cobrancaTipo) != eTipo.Indefinido)
            {
                tipoCond = " cobranca_tipo=" + cobrancaTipo + " AND ";
            }

            String qry = String.Concat(
                "SELECT top 1 cobranca_id ", // operadora_id, filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, cobranca.* ",
                "   FROM cobranca ",
                "   WHERE ",
                "       DAY(cobranca_dataVencimento)=", vencimento.Day, " AND MONTH(cobranca_dataVencimento)=", vencimento.Month, " and YEAR(cobranca_dataVencimento)=", vencimento.Year, " AND ",
                tipoCond,
                "       cobranca_propostaId =", propostaId);

            IList<Cobranca> lista = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else //if (lista.Count == 1)
                return lista[0];
            //else
            //    return lista[0]; //return null; //throw new ApplicationException("Mais de uma cobrança foi retornada.");
        }

        public static Cobranca CarregarEnviadasPor(Object propostaId, DateTime vencimento, Int32 cobrancaTipo, PersistenceManager pm)
        {
            String tipoCond = "";
            if (((eTipo)cobrancaTipo) != eTipo.Indefinido)
            {
                tipoCond = " cobranca_tipo=" + cobrancaTipo + " AND ";
            }

            String qry = String.Concat(
                "SELECT cobranca_id ", 
                "   FROM cobranca ",
                "   WHERE cobranca_arquivoultimoenvioid is not null and ",
                "       DAY(cobranca_dataVencimento)=", vencimento.Day, " AND MONTH(cobranca_dataVencimento)=", vencimento.Month, " and YEAR(cobranca_dataVencimento)=", vencimento.Year, " AND ",
                tipoCond,
                "       cobranca_propostaId =", propostaId);

            IList<Cobranca> lista = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else 
                return lista[0];
        }

        public static Cobranca CarregarPorParcelaTipo(Object cobrancaId, Int32 parcela, Int32 cobrancaTipo)
        {
            return null;//CarregarPor(propostaId, parcela, cobrancaTipo, null);
        }

        public static IList<Cobranca> CarregarTodasNaoPagas(DateTime venctoDe, DateTime venctoAte)
        {
            String qry = "* FROM cobranca WHERE cobranca_pago <> 1 AND cobranca_cancelada <> 1 AND cobranca_dataVencimento BETWEEN '" + venctoDe.ToString("yyyy-MM-dd 00:00:00.000") + "' AND '" + venctoAte.ToString("yyyy-MM-dd 23:59:59.850") + "'";

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca));
        }

        public static IList<Cobranca> CarregarTodas(Object propostaId)
        {
            return CarregarTodas(propostaId, null);
        }
        public static IList<Cobranca> CarregarTodas(Object propostaId, PersistenceManager pm)
        {
            String qry = "cobranca.*, operadora_id,contrato_numero,operadora_nome, estipulante_descricao FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId inner join estipulante on estipulante_id = contrato_estipulanteId WHERE cobranca_propostaId=" + propostaId + " ORDER BY cobranca_parcela, cobranca_id";

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarTodasORDERBYVencto(Object propostaId, PersistenceManager pm)
        {
            String qry = "cobranca.*, operadora_id FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_propostaId=" + propostaId + " ORDER BY cobranca_dataVencimento, cobranca_id";

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarTodas(Object propostaId, Boolean apenasAtivas, PersistenceManager pm)
        {
            return CarregarTodas(propostaId, apenasAtivas, false, pm);
        }
        public static IList<Cobranca> CarregarTodas(Object propostaId, Boolean apenasAtivas, Boolean apenasEmAberto, Int32 anoVencimento, PersistenceManager pm)
        {
            String ativasCond = "";
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }
            if (apenasEmAberto)
            {
                ativasCond += " AND (cobranca_pago=0 or cobranca_pago is null) ";
            }

            if(anoVencimento != -1)
                ativasCond += " AND year(cobranca_dataVencimento)=" + anoVencimento.ToString();

            String qry = String.Concat("cobranca.*, operadora_id FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_propostaId=", propostaId, ativasCond, " ORDER BY cobranca_parcela");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }
        public static IList<Cobranca> CarregarTodas(Object propostaId, Boolean apenasAtivas, Boolean parcelaDesc, PersistenceManager pm)
        {
            String ativasCond = "";
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }

            String strdesc = " ";
            if (parcelaDesc) { strdesc = " DESC "; }
            String qry = String.Concat("cobranca.*, operadora_id FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_propostaId=", propostaId, ativasCond, " ORDER BY cobranca_parcela", strdesc,",cobranca_id");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }
        public static IList<Cobranca> CarregarTodas(Object propostaId, Boolean apenasAtivas, eTipo tipo, PersistenceManager pm)
        {
            String ativasCond = "";
            String tipoCond = "";
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }

            if (tipo != eTipo.Indefinido)
            {
                tipoCond = " AND cobranca_tipo=" + Convert.ToInt32(tipo).ToString();
            }

            String qry = String.Concat("cobranca.*, operadora_id FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_propostaId=", propostaId, ativasCond, tipoCond, " ORDER BY cobranca_parcela, cobranca_id");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }
        public static IList<Cobranca> CarregarTodas_Composite(Object propostaId, Boolean apenasAtivas, Boolean parcelaDesc, PersistenceManager pm)
        {
            String ativasCond = "";
            if (apenasAtivas) { ativasCond = " AND cobranca_cancelada <> 1 "; }

            String strdesc = " ";
            if (parcelaDesc) { strdesc = " DESC "; }

            String qry = String.Concat("cobranca.*,operadora_id,cobrancacomp_beneficiarioId,cobrancacomp_tipo,cobrancacomp_valor ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id = contrato_operadoraId ",
                "       LEFT JOIN cobranca_composicao on cobranca_id=cobrancacomp_cobranaId and cobrancacomp_valor > 0 ",
                "   WHERE cobranca_propostaId=", propostaId, ativasCond, 
                "   ORDER BY cobranca_parcela", strdesc, ",cobranca_id");

            IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);

            if (cobrancas == null) { return null; }

            List<String> ids = new List<String>();

            List<Cobranca> cobrancasARetornar =new List<Cobranca>();

            foreach (Cobranca _cob in cobrancas)
            {
                if (ids.Contains(Convert.ToString(_cob.ID))) { continue; }

                cobrancasARetornar.Add(_cob);
                ids.Add(Convert.ToString(_cob.ID));
            }

            foreach (Cobranca cobrancaARetornar in cobrancasARetornar)
            {
                foreach (Cobranca cob in cobrancas)
                {
                    if (Convert.ToString(cob.ID) == Convert.ToString(cobrancaARetornar.ID))
                    {
                        if (cobrancaARetornar._composicaoResumo != null && cobrancaARetornar._composicaoResumo.Length > 0) { cobrancaARetornar._composicaoResumo += "<br>"; }

                        if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.Adicional)
                            cobrancaARetornar._composicaoResumo += "Adicional: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.Desconto)
                            cobrancaARetornar._composicaoResumo += "Desconto: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.Plano)
                            cobrancaARetornar._composicaoResumo += "Plano: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.TaxaAssociacao)
                            cobrancaARetornar._composicaoResumo += "Taxa associativa: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.TaxaTabelaValor)
                            cobrancaARetornar._composicaoResumo += "Taxa: ";

                        cobrancaARetornar._composicaoResumo += cob._composicaoValor.ToString("C");
                    }
                }
            }

            return cobrancasARetornar;
        }

        public static IList<Cobranca> CarregarTodas(IList<String> cobrancaIDs, PersistenceManager pm)
        {
            String qry = String.Concat("operadora_id, filial_nome, beneficiario_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_enderecoCobrancaId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                //"       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                //"       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                //"       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "       LEFT JOIN usuario_filial          ON contrato_donoId=usuariofilial_usuarioId ",
                "       LEFT JOIN filial                  ON filial_id=usuariofilial_filialId ",
                "   WHERE cobranca_id IN (", EntityBase.Join(cobrancaIDs, ","), ")",
                "   ORDER BY filial_nome, estipulante_descricao, operadora_nome, cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarTodas_OrdemPorContratoParcela(IList<String> cobrancaIDs, PersistenceManager pm)
        {
            String qry = String.Concat("operadora_id, filial_nome, beneficiario_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_enderecoCobrancaId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                //"       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                //"       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                //"       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "       LEFT JOIN usuario_filial          ON contrato_donoId=usuariofilial_usuarioId ",
                "       LEFT JOIN filial                  ON filial_id=usuariofilial_filialId ",
                "   WHERE cobranca_id IN (", EntityBase.Join(cobrancaIDs, ","), ")",
                "   ORDER BY contrato_id, cobranca_parcela");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarTodas_OrdemPorContratoParcela_Optimized(IList<String> cobrancaIDs, PersistenceManager pm)
        {
            List<Cobranca> cobrancas = null;

            String qry = String.Concat("operadora_id, filial_nome,beneficiario_id, beneficiario_nome, beneficiario_cpf, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_enderecoCobrancaId, contrato_codcobranca, contratoadm_id, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       INNER JOIN contratoadm on contratoadm_id=contrato_contratoadmId ",
                //"       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                //"       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                //"       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "       LEFT JOIN usuario_filial          ON contrato_donoId=usuariofilial_usuarioId ",
                "       LEFT JOIN filial                  ON filial_id=usuariofilial_filialId ",
                "   WHERE cobranca_id IN (", cobrancaIDs[0], ")",
                "   ORDER BY contrato_id, cobranca_parcela");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result", pm).Tables[0];
            DataTable aux = null;DataRow novaRow = null;
            for (int i = 1; i < cobrancaIDs.Count; i++)
            {
                qry = String.Concat("operadora_id, filial_nome, beneficiario_nome,beneficiario_id, beneficiario_cpf, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_enderecoCobrancaId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN usuario_filial          ON contrato_donoId=usuariofilial_usuarioId ",
                "       LEFT JOIN filial                  ON filial_id=usuariofilial_filialId ",
                "   WHERE cobranca_id =", cobrancaIDs[i]);

                aux = LocatorHelper.Instance.ExecuteQuery(qry, "result", pm).Tables[0];

                if(aux.Rows.Count == 0){ continue; }

                novaRow = dt.NewRow();
                novaRow["cobranca_id"] = aux.Rows[0]["cobranca_id"];
                novaRow["cobranca_arquivoUltimoEnvioId"] = aux.Rows[0]["cobranca_arquivoUltimoEnvioId"];
                novaRow["cobranca_cancelada"] = aux.Rows[0]["cobranca_cancelada"];
                novaRow["cobranca_carteira"] = aux.Rows[0]["cobranca_carteira"];
                novaRow["cobranca_cobrancaRefId"] = aux.Rows[0]["cobranca_cobrancaRefId"];
                novaRow["cobranca_comissaoPaga"] = aux.Rows[0]["cobranca_comissaoPaga"];
                novaRow["contrato_codcobranca"] = aux.Rows[0]["contrato_codcobranca"];
                novaRow["contrato_numero"] = aux.Rows[0]["contrato_numero"];
                novaRow["beneficiario_nome"] = aux.Rows[0]["beneficiario_nome"];
                novaRow["beneficiario_cpf"] = aux.Rows[0]["beneficiario_cpf"];
                novaRow["beneficiario_id"] = aux.Rows[0]["beneficiario_id"];
                novaRow["cobranca_dataCriacao"] = aux.Rows[0]["cobranca_dataCriacao"];
                novaRow["cobranca_dataPagto"] = aux.Rows[0]["cobranca_dataPagto"];
                novaRow["cobranca_dataVencimentoForcada"] = aux.Rows[0]["cobranca_dataVencimentoForcada"];
                novaRow["estipulante_descricao"] = aux.Rows[0]["estipulante_descricao"];
                novaRow["filial_nome"] = aux.Rows[0]["filial_nome"];
                novaRow["cobranca_nossoNumero"] = aux.Rows[0]["cobranca_nossoNumero"];
                novaRow["operadora_id"] = aux.Rows[0]["operadora_id"];
                novaRow["operadora_nome"] = aux.Rows[0]["operadora_nome"];
                novaRow["cobranca_pago"] = aux.Rows[0]["cobranca_pago"];
                novaRow["cobranca_parcela"] = aux.Rows[0]["cobranca_parcela"];
                novaRow["cobranca_propostaId"] = aux.Rows[0]["cobranca_propostaId"];
                novaRow["cobranca_tipo"] = aux.Rows[0]["cobranca_tipo"];
                novaRow["cobranca_tipoTemp"] = aux.Rows[0]["cobranca_tipoTemp"];
                novaRow["cobranca_valor"] = aux.Rows[0]["cobranca_valor"];
                novaRow["cobranca_valorNominal"] = aux.Rows[0]["cobranca_valorNominal"];
                novaRow["cobranca_valorPagto"] = aux.Rows[0]["cobranca_valorPagto"];
                novaRow["cobranca_dataVencimento"] = aux.Rows[0]["cobranca_dataVencimento"];
                novaRow["cobranca_dataVencimentoIsencaoJuro"] = aux.Rows[0]["cobranca_dataVencimentoIsencaoJuro"];
                novaRow["contrato_enderecoCobrancaId"] = aux.Rows[0]["contrato_enderecoCobrancaId"];
                dt.Rows.Add(novaRow);
            }

            if(dt.Rows.Count > 0){ cobrancas = new List<Cobranca>(); }

            DataRow[] arrRow = dt.Select("cobranca_id <> -1", "cobranca_propostaId ASC, cobranca_parcela ASC");

            Cobranca cobranca = null;
            foreach (DataRow row in arrRow)
            {
                cobranca = new Cobranca(row["cobranca_id"]);
                cobranca._arquivoIdUltimoEnvio = toObject(row["cobranca_arquivoUltimoEnvioId"]);
                cobranca._cancelada = toBoolean(row["cobranca_cancelada"]);
                cobranca._carteira = toInt(row["cobranca_carteira"]);
                cobranca._cobrancaRefId = toObject(row["cobranca_cobrancaRefId"]);
                cobranca._comissaoPaga = toBoolean(row["cobranca_comissaoPaga"]);
                cobranca._contratoCodCobranca = toString(row["contrato_codcobranca"]);
                cobranca._contratoNumero = toString(row["contrato_numero"]);
                cobranca._contratoTitularNome = toString(row["beneficiario_nome"]);
                cobranca._contratoTitularCpf = toString(row["beneficiario_cpf"]);
                cobranca._dataCriacao = Convert.ToDateTime(row["cobranca_dataCriacao"]);
                cobranca._dataPagto = toDateTime(row["cobranca_dataPagto"]);
                cobranca._dataVencimentoForcada = toBoolean(row["cobranca_dataVencimentoForcada"]);
                cobranca._estipulanteNome = toString(row["estipulante_descricao"]);
                cobranca._filialNome = toString(row["filial_nome"]);
                cobranca._nossoNumero = toString(row["cobranca_nossoNumero"]);
                cobranca._operadoraId = row["operadora_id"];
                cobranca._operadoraNome = toString(row["operadora_nome"]);
                cobranca._pago = toBoolean(row["cobranca_pago"]);
                cobranca._parcela = toInt(row["cobranca_parcela"]);
                cobranca._propostaId = row["cobranca_propostaId"];
                cobranca._tipo = toInt(row["cobranca_tipo"]);
                cobranca._tipoTemp = toInt(row["cobranca_tipoTemp"]);
                cobranca._valor = toDecimal(row["cobranca_valor"]);
                cobranca._valorNominal = toDecimal(row["cobranca_valorNominal"]);
                cobranca._valorPagto = toDecimal(row["cobranca_valorPagto"]);
                cobranca._vencimento = toDateTime(row["cobranca_dataVencimento"]);
                cobranca._vencimentoIsencaoJuro = toDateTime(row["cobranca_dataVencimentoIsencaoJuro"]);
                cobranca._contratoEnderecoCobrancaId = toObject(row["contrato_enderecoCobrancaId"]);
                cobranca.BeneficiarioId = toObject(row["beneficiario_id"]);

                cobrancas.Add(cobranca);
            }

            dt.Dispose();
            return cobrancas;
        }

        public static IList<Cobranca> CarregarTodas_Optimized(Object propostaId, Boolean apenasAtivas, eTipo tipo, PersistenceManager pm)
        {
            String ativasCond = "";
            String tipoCond = "";
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }

            if (tipo != eTipo.Indefinido)
            {
                tipoCond = " AND cobranca_tipo=" + Convert.ToInt32(tipo).ToString();
            }

            String qry = String.Concat("operadora_id, cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_propostaId=", propostaId, ativasCond, tipoCond, " ORDER BY cobranca_parcela, cobranca_id");
            //String qry = String.Concat("operadora_id, cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_id=2715955");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];
            if (dt.Rows.Count == 0) { return null; }
            List<Cobranca> cobrancas = new List<Cobranca>();

            Cobranca cobranca = null;
            foreach (DataRow row in dt.Rows)
            {
                cobranca = new Cobranca(row["cobranca_id"]);
                cobranca._arquivoIdUltimoEnvio = toObject(row["cobranca_arquivoUltimoEnvioId"]);
                cobranca._cancelada = toBoolean(row["cobranca_cancelada"]);
                cobranca._carteira = toInt(row["cobranca_carteira"]);
                cobranca._cobrancaRefId = toObject(row["cobranca_cobrancaRefId"]);
                cobranca._comissaoPaga = toBoolean(row["cobranca_comissaoPaga"]);
                cobranca._dataCriacao = Convert.ToDateTime(row["cobranca_dataCriacao"]);
                cobranca._dataPagto = toDateTime(row["cobranca_dataPagto"]);
                cobranca._dataVencimentoForcada = toBoolean(row["cobranca_dataVencimentoForcada"]);
                cobranca._nossoNumero = toString(row["cobranca_nossoNumero"]);
                cobranca._operadoraId = row["operadora_id"];
                cobranca._pago = toBoolean(row["cobranca_pago"]);
                cobranca._parcela = toInt(row["cobranca_parcela"]);
                cobranca._propostaId = row["cobranca_propostaId"];
                cobranca._tipo = toInt(row["cobranca_tipo"]);
                cobranca._tipoTemp = toInt(row["cobranca_tipoTemp"]);
                cobranca._valor = toDecimal(row["cobranca_valor"]);
                cobranca._valorNominal = toDecimal(row["cobranca_valorNominal"]);
                cobranca._valorPagto = toDecimal(row["cobranca_valorPagto"]);
                cobranca._vencimento = toDateTime(row["cobranca_dataVencimento"]);
                cobranca._vencimentoIsencaoJuro = toDateTime(row["cobranca_dataVencimentoIsencaoJuro"]);

                cobrancas.Add(cobranca);
            }

            return cobrancas;
        }

        public static IList<Cobranca> CarregarTodas_Optimized_temp(PersistenceManager pm)
        {
            String ativasCond = "";
            String tipoCond = "";

            String qry = String.Concat("operadora_id, cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_arquivoUltimoEnvioId=44 ORDER BY cobranca_propostaId,cobranca_parcela");
            //String qry = String.Concat("operadora_id, cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_id=2715955");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];
            if (dt.Rows.Count == 0) { return null; }
            List<Cobranca> cobrancas = new List<Cobranca>();

            Cobranca cobranca = null;
            foreach (DataRow row in dt.Rows)
            {
                cobranca = new Cobranca(row["cobranca_id"]);
                cobranca._arquivoIdUltimoEnvio = toObject(row["cobranca_arquivoUltimoEnvioId"]);
                cobranca._cancelada = toBoolean(row["cobranca_cancelada"]);
                cobranca._carteira = toInt(row["cobranca_carteira"]);
                cobranca._cobrancaRefId = toObject(row["cobranca_cobrancaRefId"]);
                cobranca._comissaoPaga = toBoolean(row["cobranca_comissaoPaga"]);
                cobranca._dataCriacao = Convert.ToDateTime(row["cobranca_dataCriacao"]);
                cobranca._dataPagto = toDateTime(row["cobranca_dataPagto"]);
                cobranca._dataVencimentoForcada = toBoolean(row["cobranca_dataVencimentoForcada"]);
                cobranca._nossoNumero = toString(row["cobranca_nossoNumero"]);
                cobranca._operadoraId = row["operadora_id"];
                cobranca._pago = toBoolean(row["cobranca_pago"]);
                cobranca._parcela = toInt(row["cobranca_parcela"]);
                cobranca._propostaId = row["cobranca_propostaId"];
                cobranca._tipo = toInt(row["cobranca_tipo"]);
                cobranca._tipoTemp = toInt(row["cobranca_tipoTemp"]);
                cobranca._valor = toDecimal(row["cobranca_valor"]);
                cobranca._valorNominal = toDecimal(row["cobranca_valorNominal"]);
                cobranca._valorPagto = toDecimal(row["cobranca_valorPagto"]);
                cobranca._vencimento = toDateTime(row["cobranca_dataVencimento"]);
                cobranca._vencimentoIsencaoJuro = toDateTime(row["cobranca_dataVencimentoIsencaoJuro"]);

                cobrancas.Add(cobranca);
            }

            return cobrancas;
        }

        public static IList<Cobranca> CarregarTodas_Optimized(Boolean apenasAtivas, eTipo tipo, PersistenceManager pm)
        {
            String ativasCond = "";
            String tipoCond = "";
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }

            if (tipo != eTipo.Indefinido)
            {
                tipoCond = " AND cobranca_tipo=" + Convert.ToInt32(tipo).ToString();
            }

            String qry = String.Concat("operadora_id, cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE (cobranca_arquivoUltimoEnvioId is null or cobranca_arquivoUltimoEnvioId=-2 or cobranca_arquivoUltimoEnvioId=-3 or cobranca_arquivoUltimoEnvioId=-4) ", ativasCond, tipoCond, " ORDER BY cobranca_parcela, cobranca_id");
            //String qry = String.Concat("operadora_id, cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_id=2715955");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];
            if (dt.Rows.Count == 0) { return null; }
            List<Cobranca> cobrancas = new List<Cobranca>();

            Cobranca cobranca = null;
            foreach (DataRow row in dt.Rows)
            {
                cobranca = new Cobranca(row["cobranca_id"]);
                cobranca._arquivoIdUltimoEnvio = toObject(row["cobranca_arquivoUltimoEnvioId"]);
                cobranca._cancelada = toBoolean(row["cobranca_cancelada"]);
                cobranca._carteira = toInt(row["cobranca_carteira"]);
                cobranca._cobrancaRefId = toObject(row["cobranca_cobrancaRefId"]);
                cobranca._comissaoPaga = toBoolean(row["cobranca_comissaoPaga"]);
                cobranca._dataCriacao = Convert.ToDateTime(row["cobranca_dataCriacao"]);
                cobranca._dataPagto = toDateTime(row["cobranca_dataPagto"]);
                cobranca._dataVencimentoForcada = toBoolean(row["cobranca_dataVencimentoForcada"]);
                cobranca._nossoNumero = toString(row["cobranca_nossoNumero"]);
                cobranca._operadoraId = row["operadora_id"];
                cobranca._pago = toBoolean(row["cobranca_pago"]);
                cobranca._parcela = toInt(row["cobranca_parcela"]);
                cobranca._propostaId = row["cobranca_propostaId"];
                cobranca._tipo = toInt(row["cobranca_tipo"]);
                cobranca._tipoTemp = toInt(row["cobranca_tipoTemp"]);
                cobranca._valor = toDecimal(row["cobranca_valor"]);
                cobranca._valorNominal = toDecimal(row["cobranca_valorNominal"]);
                cobranca._valorPagto = toDecimal(row["cobranca_valorPagto"]);
                cobranca._vencimento = toDateTime(row["cobranca_dataVencimento"]);
                cobranca._vencimentoIsencaoJuro = toDateTime(row["cobranca_dataVencimentoIsencaoJuro"]);

                cobrancas.Add(cobranca);
            }

            return cobrancas;
        }

        public static IList<Cobranca> CarregarTodas_Optimized_DIF_UBRASP(PersistenceManager pm)
        {
            String ativasCond = "";
            String tipoCond = "";
            bool apenasAtivas = true;
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }

            eTipo tipo = eTipo.DiferencaUbraSP;
            if (tipo != eTipo.Indefinido)
            {
                tipoCond = " AND cobranca_tipo=" + Convert.ToInt32(tipo).ToString();
            }

            String qry = String.Concat("operadora_id, cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE (cobranca_arquivoUltimoEnvioId=-2) and (cobranca_cancelada is null or cobranca_cancelada=0)", ativasCond, tipoCond, " ORDER BY cobranca_parcela, cobranca_id");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];
            if (dt.Rows.Count == 0) { return null; }
            List<Cobranca> cobrancas = new List<Cobranca>();

            Cobranca cobranca = null;
            foreach (DataRow row in dt.Rows)
            {
                cobranca = new Cobranca(row["cobranca_id"]);
                cobranca._arquivoIdUltimoEnvio = toObject(row["cobranca_arquivoUltimoEnvioId"]);
                cobranca._cancelada = toBoolean(row["cobranca_cancelada"]);
                cobranca._carteira = toInt(row["cobranca_carteira"]);
                cobranca._cobrancaRefId = toObject(row["cobranca_cobrancaRefId"]);
                cobranca._comissaoPaga = toBoolean(row["cobranca_comissaoPaga"]);
                cobranca._dataCriacao = Convert.ToDateTime(row["cobranca_dataCriacao"]);
                cobranca._dataPagto = toDateTime(row["cobranca_dataPagto"]);
                cobranca._dataVencimentoForcada = toBoolean(row["cobranca_dataVencimentoForcada"]);
                cobranca._nossoNumero = toString(row["cobranca_nossoNumero"]);
                cobranca._operadoraId = row["operadora_id"];
                cobranca._pago = toBoolean(row["cobranca_pago"]);
                cobranca._parcela = toInt(row["cobranca_parcela"]);
                cobranca._propostaId = row["cobranca_propostaId"];
                cobranca._tipo = toInt(row["cobranca_tipo"]);
                cobranca._tipoTemp = toInt(row["cobranca_tipoTemp"]);
                cobranca._valor = toDecimal(row["cobranca_valor"]);
                cobranca._valorNominal = toDecimal(row["cobranca_valorNominal"]);
                cobranca._valorPagto = toDecimal(row["cobranca_valorPagto"]);
                cobranca._vencimento = toDateTime(row["cobranca_dataVencimento"]);
                cobranca._vencimentoIsencaoJuro = toDateTime(row["cobranca_dataVencimentoIsencaoJuro"]);

                cobrancas.Add(cobranca);
            }

            return cobrancas;
        }

        public static IList<Cobranca> CarregarTodas_Optimized_BOLETO_UBRASP(PersistenceManager pm)
        {
            String ativasCond = "";
            String tipoCond = "";

            bool apenasAtivas = true;
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }

            eTipo tipo = eTipo.BoletoUbraspSP;
            if (tipo != eTipo.Indefinido)
            {
                tipoCond = " AND cobranca_tipo=" + Convert.ToInt32(tipo).ToString();
            }

            String qry = String.Concat("operadora_id, cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE (cobranca_arquivoUltimoEnvioId=-3) and (cobranca_cancelada is null or cobranca_cancelada=0)", ativasCond, tipoCond, " ORDER BY cobranca_parcela, cobranca_id");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];
            if (dt.Rows.Count == 0) { return null; }
            List<Cobranca> cobrancas = new List<Cobranca>();

            Cobranca cobranca = null;
            foreach (DataRow row in dt.Rows)
            {
                cobranca = new Cobranca(row["cobranca_id"]);
                cobranca._arquivoIdUltimoEnvio = toObject(row["cobranca_arquivoUltimoEnvioId"]);
                cobranca._cancelada = toBoolean(row["cobranca_cancelada"]);
                cobranca._carteira = toInt(row["cobranca_carteira"]);
                cobranca._cobrancaRefId = toObject(row["cobranca_cobrancaRefId"]);
                cobranca._comissaoPaga = toBoolean(row["cobranca_comissaoPaga"]);
                cobranca._dataCriacao = Convert.ToDateTime(row["cobranca_dataCriacao"]);
                cobranca._dataPagto = toDateTime(row["cobranca_dataPagto"]);
                cobranca._dataVencimentoForcada = toBoolean(row["cobranca_dataVencimentoForcada"]);
                cobranca._nossoNumero = toString(row["cobranca_nossoNumero"]);
                cobranca._operadoraId = row["operadora_id"];
                cobranca._pago = toBoolean(row["cobranca_pago"]);
                cobranca._parcela = toInt(row["cobranca_parcela"]);
                cobranca._propostaId = row["cobranca_propostaId"];
                cobranca._tipo = toInt(row["cobranca_tipo"]);
                cobranca._tipoTemp = toInt(row["cobranca_tipoTemp"]);
                cobranca._valor = toDecimal(row["cobranca_valor"]);
                cobranca._valorNominal = toDecimal(row["cobranca_valorNominal"]);
                cobranca._valorPagto = toDecimal(row["cobranca_valorPagto"]);
                cobranca._vencimento = toDateTime(row["cobranca_dataVencimento"]);
                cobranca._vencimentoIsencaoJuro = toDateTime(row["cobranca_dataVencimentoIsencaoJuro"]);

                cobrancas.Add(cobranca);
            }

            return cobrancas;
        }


        public static IList<Cobranca> CarregarTodasComParcelamentoInfo(IList<String> cobrancaIDs, PersistenceManager pm)
        {
            String qry = String.Concat("operadora_id, parccob_headerId, parcitem_headerId,filial_nome, beneficiario_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_enderecoCobrancaId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN usuario_filial          ON contrato_donoId=usuariofilial_usuarioId ",
                "       LEFT JOIN filial                  ON filial_id=usuariofilial_filialId ",
                "       LEFT JOIN cobranca_parcelamentoCobrancaOriginal ON cobranca_id = parccob_cobrancaId ",
                "       LEFT JOIN cobranca_parcelamentoItem ON cobranca_id = parcitem_cobrancaId ",
                "   WHERE cobranca_id IN (", EntityBase.Join(cobrancaIDs, ","), ")",
                "   ORDER BY filial_nome, estipulante_descricao, operadora_nome, cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarTodasComParcelamentoInfo(Object propostaId, Boolean apenasAtivas, Boolean apenasEmAberto, Int32 anoVencimento, PersistenceManager pm, DateTime cancelamentoContrato)
        {
            String ativasCond = "";
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }
            if (apenasEmAberto)
            {
                ativasCond += " AND ((cobranca_pago=0 or cobranca_pago is null) or cobranca_id in (select parccob_cobrancaId from cobranca_parcelamentoCobrancaOriginal) )";
            }

            if (anoVencimento != -1)
                ativasCond += " AND year(cobranca_dataVencimento)=" + anoVencimento.ToString();

            if (cancelamentoContrato != DateTime.MinValue)
            {
                ativasCond += " and (cobranca_datavencimento < '" + cancelamentoContrato.ToString("yyyy-MM-dd 23:59:59.995") + "' or cobranca_tipo = 4) ";
            }

            String qry = String.Concat("cobranca.*, parccob_headerId, operadora_id FROM cobranca ",
                "   INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "   INNER JOIN operadora ON operadora_id = contrato_operadoraId ",
                "   LEFT  JOIN cobranca_parcelamentoCobrancaOriginal ON cobranca_id = parccob_cobrancaId ",
                "   WHERE cobranca_propostaId=", propostaId, ativasCond, " ORDER BY cobranca_parcela");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }
        public static IList<Cobranca> CarregarTodasComParcelamentoInfo_Composite(Object propostaId, Boolean apenasAtivas, Boolean parcelaDesc, PersistenceManager pm)
        {
            String ativasCond = "";
            if (apenasAtivas) { ativasCond = " AND cobranca_cancelada <> 1 "; }

            String strdesc = " ";
            if (parcelaDesc) { strdesc = " DESC "; }

            String qry = String.Concat("cobranca.*,parccob_headerId,parcitem_headerId,parcitem_obs,operadora_id,cobrancacomp_beneficiarioId,cobrancacomp_tipo,cobrancacomp_valor ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id = contrato_operadoraId ",
                "       LEFT JOIN cobranca_composicao on cobranca_id=cobrancacomp_cobranaId and cobrancacomp_valor > 0 ",
                "       LEFT JOIN cobranca_parcelamentoCobrancaOriginal ON cobranca_id = parccob_cobrancaId ",
                "       LEFT JOIN cobranca_parcelamentoItem ON cobranca_id = parcitem_cobrancaId ",
                "   WHERE cobranca_propostaId=", propostaId, ativasCond,
                "   ORDER BY cobranca_dataVencimento", strdesc, ",cobranca_id");

            IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);

            if (cobrancas == null) { return null; }

            List<String> ids = new List<String>();

            List<Cobranca> cobrancasARetornar = new List<Cobranca>();

            foreach (Cobranca _cob in cobrancas)
            {
                if (ids.Contains(Convert.ToString(_cob.ID))) { continue; }

                cobrancasARetornar.Add(_cob);
                ids.Add(Convert.ToString(_cob.ID));
            }

            foreach (Cobranca cobrancaARetornar in cobrancasARetornar)
            {
                foreach (Cobranca cob in cobrancas)
                {
                    if (Convert.ToString(cob.ID) == Convert.ToString(cobrancaARetornar.ID))
                    {
                        if (cobrancaARetornar._composicaoResumo != null && cobrancaARetornar._composicaoResumo.Length > 0) { cobrancaARetornar._composicaoResumo += "<br>"; }

                        if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.Adicional)
                            cobrancaARetornar._composicaoResumo += "Adicional: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.Desconto)
                            cobrancaARetornar._composicaoResumo += "Desconto: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.Plano)
                            cobrancaARetornar._composicaoResumo += "Plano: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.TaxaAssociacao)
                            cobrancaARetornar._composicaoResumo += "Taxa associativa: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.TaxaTabelaValor)
                            cobrancaARetornar._composicaoResumo += "Taxa: ";

                        cobrancaARetornar._composicaoResumo += cob._composicaoValor.ToString("C");
                    }
                }
            }

            return cobrancasARetornar;
        }

        public static IList<Cobranca> CarregarTodasComParcelamentoInfo(Object propostaId, Boolean apenasAtivas, eTipo tipo, PersistenceManager pm)
        {
            String cond = "";
            if (apenasAtivas)
            {
                cond = " AND cobranca_cancelada <> 1 ";
            }
            if (tipo != eTipo.Indefinido)
            {
                cond += " AND cobranca_tipo=" + Convert.ToInt32(tipo); ;
            }

            String qry = String.Concat("cobranca.*, parccob_headerId, operadora_id FROM cobranca ",
                "   INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "   INNER JOIN operadora ON operadora_id = contrato_operadoraId ",
                "   LEFT  JOIN cobranca_parcelamentoCobrancaOriginal ON cobranca_id = parccob_cobrancaId ",
                "   WHERE cobranca_propostaId=", propostaId, cond, " ORDER BY cobranca_parcela");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        static Decimal toDecimal(Object param)
        {
            if (param == DBNull.Value || param == null)
                return Decimal.Zero;
            else
            {
                try
                {
                    return Convert.ToDecimal(param);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }
        }
        static DateTime toDateTime(Object param)
        {
            if (param == DBNull.Value || param == null)
                return DateTime.MinValue;
            else
            {
                try
                {
                    return Convert.ToDateTime(param);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }
        static Object toObject(Object param)
        {
            if (param == DBNull.Value)
                return null;
            else
                return param;
        }
        static Boolean toBoolean(Object param)
        {
            if (param == DBNull.Value || param == null)
                return false;
            else
            {
                try
                {
                    return Convert.ToBoolean(param);
                }
                catch
                {
                    return false;
                }
            }
        }
        static Int32 toInt(Object param)
        {
            if (param == DBNull.Value || param == null)
                return 0;
            else
            {
                try
                {
                    return Convert.ToInt32(param);
                }
                catch
                {
                    return 0;
                }
            }
        }
        static String toString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return null;
            else
                return Convert.ToString(param);
        }

        public static IList<Cobranca> CarregarPorArquivoRemessaID(Object arquivoId, Boolean emAberto)
        {
            return CarregarPorArquivoRemessaID(arquivoId, emAberto, null);
        }
        public static IList<Cobranca> CarregarPorArquivoRemessaID(Object arquivoId, Boolean emAberto, PersistenceManager pm)
        {
            String emAbertoCond = "";
            if (emAberto)
            {
                emAbertoCond = " cobranca_pago=0 AND cobranca_cancelada=0 and ";
            }

            String qry = String.Concat(
                "SELECT DISTINCT(cobranca_id) as cid, operadora_id, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.*,beneficiario_id,beneficiario_nome, endereco.* ", //filial_nome
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_contratoId=contrato_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioid and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                "       LEFT JOIN endereco ON endereco_donoId=beneficiario_id and endereco_donoTipo= ", Convert.ToInt32(Endereco.TipoDono.Beneficiario),
                //"       INNER JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                //"       INNER JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                //"       INNER JOIN filial ON filial_id=almox_produto_filialId ",
                //"       INNER JOIN usuario_filial          ON contrato_donoId=usuariofilial_usuarioId ",
                //"       INNER JOIN filial                  ON filial_id=usuariofilial_filialId ",
                "       INNER JOIN arquivoCobrancaUnibanco_cobanca ON arqitem_cobrancaId=cobranca_id ",
                "   WHERE ",emAbertoCond,
                "       arqitem_arquivoId =", arquivoId,
                "   ORDER BY cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarTodas(IList<String> cobrancaIDs)
        {
            return CarregarTodas(cobrancaIDs, null);
        }

        /// <summary>
        /// Checa se só há uma cobrança por proposta em uma coleção de cobranças.
        /// </summary>
        /// <param name="lista">Lista de cobranças.</param>
        /// <param name="propostaId">Id da proposta a ser comparado</param>
        /// <returns>True, caso só haja uma cobrança por proposta. Do contrário, False.</returns>
        static Boolean umaCobrancaPorProposta(IList<Cobranca> lista, String propostaId)
        {
            Int32 qtd = 0;
            foreach (Cobranca cobranca in lista)
            {
                if (Convert.ToString(cobranca.PropostaID) == propostaId)
                    qtd++;

                if (qtd > 1) { return false; }
            }

            return true;
        }

        public static IList<Cobranca> CarregarPorNumeroDeContrato(Object operadoraId, String contratoNumero, PersistenceManager pm)
        {
            String cond = "";
            if (operadoraId != null) { cond = " and operadora_id=" + operadoraId; }

            String qry = String.Concat(
                "filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca (nolock) ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                "       contrato_numero='", contratoNumero, "' ",
                cond,
                "   ORDER BY cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

//        public static IList<Cobranca> CarregarPorNomeBeneficiario(Object operadoraId, String nomeBeneficiario, PersistenceManager pm)
//        {
//            if (nomeBeneficiario == null || string.IsNullOrEmpty(nomeBeneficiario)) return null;

//            String cond = "";
//            if (operadoraId != null) { cond = " and operadora_id=" + operadoraId; }

//            String qry = String.Concat(
//                "filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, beneficiario_nome, cobranca.* ",
//                "   FROM cobranca (nolock) ",
//                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
//                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
//                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
//                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_contratoId = contratoId=contrato_id and contratobeneficiario_ativo=1 and contratobeneficiario_tipo = 0 ",
//                "       INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId = contratoId = beneficiario_id and contratobeneficiario_ativo=1 and contratobeneficiario_tipo = 0 ",
////              "       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
////              "       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
////              "       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
//                "   WHERE ",
//                "       contrato_numero like '%", nomeBeneficiario.Replace("'", ""), "%' ",
//                cond,
//                "   ORDER BY cobranca_dataVencimento");

//            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
//        }

        public static IList<Cobranca> CarregarPorID(Object id, PersistenceManager pm)
        {
            String qry = String.Concat(
                "filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                "       cobranca_id=", id);

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static Cobranca CarregarUnicaPorID(string id, PersistenceManager pm)
        {
            String qry = String.Concat(
                "SELECT operadora_id, filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                "       cobranca_id=", id);

            IList<Cobranca> lista = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
            if (lista == null)
                return null;
            else if (lista.Count == 1)
                return lista[0];
            else
                throw new ApplicationException("Mais de uma cobrança foi retornada com o id " + id);
        }

        public static IList<Cobranca> CarregarPagasDoAno(Object contratoId, int ano, PersistenceManager pm)
        {
            String qry = String.Concat(
                "estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                //"       INNER JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                //"       INNER JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                //"       INNER JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                "       YEAR(cobranca_dataVencimento) =", ano, " and cobranca_cancelada=0 AND cobranca_pago=1 AND cobranca_propostaId=", contratoId,
                "   ORDER BY cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarAtrasadas(Object operadoraId)
        {
            return CarregarAtrasadas(operadoraId, null);
        }
        public static IList<Cobranca> CarregarAtrasadas(Object operadoraId, PersistenceManager pm)
        {
            String qry = String.Concat(
                "filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       INNER JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       INNER JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       INNER JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                "       cobranca_dataVencimento < GETDATE() AND cobranca_pago=0 AND operadora_id=", operadoraId,
                "   ORDER BY cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarAtrasadas(Object operadoraId, Int32 mes, Int32 ano)
        {
            return CarregarAtrasadas(operadoraId, mes, ano, null);
        }
        public static IList<Cobranca> CarregarAtrasadas(Object operadoraId, Int32 mes, Int32 ano, PersistenceManager pm)
        {
            String qry = String.Concat(
                "filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       left JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       left JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       left JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                Contrato.CondicaoBasicaQuery, " AND ", 
                "       MONTH(cobranca_dataVencimento)=", mes, " AND ",
                "       YEAR(cobranca_dataVencimento)=", ano, " AND ",
                "       cobranca_pago=0 AND operadora_id=", operadoraId,
                "   ORDER BY cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        #endregion

        public static void MarcarCobrancaComoNaoEnviadas(Object arquivoRemessaId, Object cobrancaId)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                MarcarCobrancaComoNaoEnviadas(arquivoRemessaId, cobrancaId, pm);
                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }
        public static void MarcarCobrancaComoNaoEnviadas(Object arquivoRemessaId, Object cobrancaId, PersistenceManager pm)
        {
            Cobranca cobranca = new Cobranca(cobrancaId);
            pm.Load(cobranca);
            cobranca.ArquivoIDUltimoEnvio = null;
            pm.Save(cobranca);

            String command = "DELETE FROM arquivoCobrancaUnibanco_cobanca WHERE arqitem_arquivoId=" + arquivoRemessaId + " AND arqitem_cobrancaId=" + cobrancaId;
            NonQueryHelper.Instance.ExecuteNonQuery(command, pm);
        }

        public static void MarcarCobrancasComoNaoEnviadas(Object arquivoRemessaId, Boolean apenasNaoPagas)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                MarcarCobrancasComoNaoEnviadas(arquivoRemessaId, apenasNaoPagas, pm);
                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }
        public static void MarcarCobrancasComoNaoEnviadas(Object arquivoRemessaId, Boolean apenasNaoPagas, PersistenceManager pm)
        {
            IList<Cobranca> cobrancas = Cobranca.CarregarPorArquivoRemessaID(arquivoRemessaId, apenasNaoPagas, pm);
            if (cobrancas == null) { return; }

            String command = "DELETE FROM arquivoCobrancaUnibanco_cobanca WHERE arqitem_arquivoId=" + arquivoRemessaId + " AND arqitem_cobrancaId=";
            foreach (Cobranca cobranca in cobrancas)
            {
                if (cobranca.Pago) { continue; }
                cobranca.ArquivoIDUltimoEnvio = null;

                if (pm != null)
                    pm.Save(cobranca);
                else
                    cobranca.Salvar();

                NonQueryHelper.Instance.ExecuteNonQuery(command + cobranca.ID, pm);
            }
        }

        /// <summary>
        /// Carrega cobranças NÃO enviadas em arquivo de remessa.
        /// </summary>
        public static IList<Cobranca> ProcessarCobrancasPorTipoParaGerarRemessa(String[] estipulanteIDs, String[] operadoraIDs, String[] filialIDs, Int32 mes, Int32 ano, Cobranca.eTipo tipo)
        {
            #region query 

            String filIN = String.Join(",", filialIDs);
            String opeIN = String.Join(",", operadoraIDs);
            String estIN = String.Join(",", estipulanteIDs);

            String qry = String.Concat(
                "SELECT DISTINCT(cobranca_id), cobranca.*, operadora_id, filial_nome, estipulante_descricao, operadora_nome, contrato_numero, contrato_enderecoCobrancaId, contrato_codcobranca, contrato_vencimento ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON contrato_id=cobranca_propostaId ",
                "       INNER JOIN usuario_filial ON contrato_donoId=usuariofilial_usuarioId ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN filial ON filial_id=usuariofilial_filialId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_status NOT IN (0,1,10,15,17,21) AND contratobeneficiario_contratoId=contrato_id ",
                "   WHERE cobranca_tipo=", Convert.ToInt32(tipo), " AND ", Contrato.CondicaoBasicaQuery,
                "       AND cobranca_cancelada <> 1 AND cobranca_arquivoUltimoEnvioId IS NULL AND ",
                "       usuariofilial_filialId IN (", filIN, ") AND ",
                "       contrato_estipulanteId IN (", estIN, ") AND ",
                "       contrato_operadoraId IN (", opeIN, ") ",
                "   ORDER BY filial_nome, estipulante_descricao, operadora_nome, contrato_vencimento");

            #endregion

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca));
        }

        static Cobranca ultimaCobrancaNormalDaColecao(IList<Cobranca> cobrancas)
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            foreach (Cobranca cobranca in cobrancas)
            {
                if (cobranca.Tipo == (int)Cobranca.eTipo.Normal) { list.Add(cobranca); }
            }

            if (list.Count > 0) { return list[list.Count - 1] as Cobranca; }
            else { return null; }
        }

        /// <summary>
        /// Versão 1
        /// </summary>
        [Obsolete("Não usar", false)]
        public static IList<Cobranca> ProcessarCobrancasNormaisParaGerarRemessa(String formattedContratoAdmIds, ArquivoRemessaCriterio.eTipoTaxa tipoTaxa, DateTime venctoDe, DateTime venctoAte, DateTime vigenciaDe, DateTime vigenciaAte, eCarteira carteira)
        {
            decimal temp = 0;

            #region query

            venctoAte = new DateTime(venctoAte.Year, venctoAte.Month, venctoAte.Day, venctoAte.Hour, venctoAte.Minute, venctoAte.Second, 901);

            String condicaoTaxa = "";
            if (tipoTaxa == ArquivoRemessaCriterio.eTipoTaxa.SemTaxa)
                condicaoTaxa = " AND contrato_cobrarTaxaAssociativa=0 ";
            else if (tipoTaxa == ArquivoRemessaCriterio.eTipoTaxa.ComTaxa)
                condicaoTaxa = " AND contrato_cobrarTaxaAssociativa=1 ";

            String condicaoVigencia = String.Concat(" AND contrato_vigencia BETWEEN '", vigenciaDe.ToString("yyyy-MM-dd"), "' AND '", vigenciaAte.ToString("yyyy-MM-dd 23:59:59") + "' ");

            String qry = String.Concat(
                "SELECT DISTINCT(contrato_id), contrato_operadoraid, filial_nome, estipulante_descricao, operadora_nome, contrato_numero, contrato_numeroId, contrato_vigencia, contrato_admissao, contrato_vencimento, contrato_enderecoCobrancaId, contrato_codcobranca, contrato_contratoadmid ",
                "   FROM contrato ",
                "       LEFT JOIN usuario_filial   ON contrato_donoId=usuariofilial_usuarioId ",
                "       LEFT JOIN filial           ON filial_id=usuariofilial_filialId ",
                "       INNER JOIN operadora        ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante      ON estipulante_id=contrato_estipulanteId ",
                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_status NOT IN (10,15,17,21) AND contratobeneficiario_ativo=1 AND contratobeneficiario_contratoId=contrato_id ", //0,1,
                "   WHERE ", Contrato.CondicaoBasicaQuery,
                "       AND contrato_contratoAdmId IN (", formattedContratoAdmIds, ") ", condicaoTaxa, condicaoVigencia,
                "   ORDER BY filial_nome, estipulante_descricao, operadora_nome, contrato_vencimento");

            #endregion

            PersistenceManager pm = null;

            try
            {
                IList<Contrato> contratos = LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato));

                if (contratos == null) { return null; }

                IList<Cobranca> _cobrancas = null;
                List<Cobranca> retornar = new List<Cobranca>();
                int parcela = 1; Boolean existente = false;
                int i = 0;

                DateTime vigencia, vencimento;
                Int32 diaDataSemJuros; Object valorDataLimite;
                CalendarioVencimento rcv = null; List<CobrancaComposite> composite = null;

                foreach (Contrato contrato in contratos)
                {
                    pm = new PersistenceManager();
                    pm.UseSingleCommandInstance();

                    //Para cada contrato gerar a cobrança.
                    //  checar se há uma cobrança ativa com as mesmas características. se existir, não gerar.
                    _cobrancas = Cobranca.CarregarTodas(contrato.ID, true, eTipo.Indefinido, pm);


                    if (_cobrancas == null || _cobrancas.Count == 0)
                    {
                        parcela = 1;

                        #region gera cobranca

                        Cobranca cobranca = new Cobranca();
                        cobranca.BeneficiarioEmail = "";
                        cobranca.BeneficiarioId = null;
                        cobranca.ContratoEnderecoCobrancaID = contrato.EnderecoCobrancaID;
                        cobranca.ContratoNumero = contrato.Numero;
                        cobranca.ContratoTitularNome = "";
                        cobranca.DataCriacao = DateTime.Now;
                        cobranca.DataVencimento = new DateTime(contrato.Admissao.Year, contrato.Admissao.Month, contrato.Admissao.Day, 23, 59, 59, 0);
                        cobranca.EstipulanteNome = "";
                        cobranca.FilialNome = "";
                        cobranca.OperadoraID = contrato.OperadoraID;
                        cobranca.OperadoraNome = "";
                        cobranca.Pago = false;
                        cobranca.Parcela = parcela;
                        cobranca.PropostaID = contrato.ID;
                        cobranca.Tipo = (Int32)Cobranca.eTipo.Normal;
                        cobranca.Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite, false);
                        if (cobranca.Valor > 0)
                        {
                            if (contrato.Vencimento >= venctoDe && contrato.Vencimento <= venctoAte)
                            {
                                cobranca.Carteira = (Int32)carteira;
                                pm.Save(cobranca);
                                retornar.Add(cobranca);
                                CobrancaComposite.Salvar(cobranca.ID, composite, pm);
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        existente = false;
                        i = 0;

                        foreach (Cobranca _existente in _cobrancas)
                        {
                            if (_existente.DataVencimento >= venctoDe &&
                                _existente.DataVencimento <= venctoAte &&
                                _existente.Tipo == (Int32)Cobranca.eTipo.Normal)
                            {
                                if (_existente.ArquivoIDUltimoEnvio == null && // nao enviado
                                    _existente.Pago == false &&                // nao pago
                                    _existente.Cancelada == false)             // ativa
                                {
                                    if (i > 0)
                                    {
                                        if (_cobrancas[i - 1].Pago) //checa se cobrança anterior está paga
                                        {
                                            temp = Contrato.CalculaValorDaProposta2(contrato.ID, _existente.DataVencimento, pm, false, true, ref composite, false);
                                            if (_existente.Valor != temp)
                                            {
                                                _existente.Valor = temp;
                                                pm.Save(_existente);
                                            }

                                            _existente.Carteira = (Int32)carteira;
                                            retornar.Add(_existente);
                                        }
                                    }
                                    else
                                    {
                                        temp = Contrato.CalculaValorDaProposta2(contrato.ID, _existente.DataVencimento, pm, false, true, ref composite, false);
                                        if (_existente.Valor != temp)
                                        {
                                            _existente.Valor = temp;
                                            pm.Save(_existente);
                                        }

                                        _existente.Carteira = (Int32)carteira;
                                        retornar.Add(_existente);
                                    }
                                }

                                existente = true;
                                break;
                            }

                            i++;
                        }

                        if (!existente) //cria uma nova cobranca
                        {
                            if (_cobrancas.Count == 0 || _cobrancas[_cobrancas.Count - 1].Pago) //checa se cobrança anterior está paga
                            {
                                parcela = _cobrancas[_cobrancas.Count - 1].Parcela + 1; //_cobrancas.Count + 1;

                                #region gera cobranca

                                Cobranca cobranca = new Cobranca();
                                cobranca.BeneficiarioEmail = "";
                                cobranca.BeneficiarioId = null;
                                cobranca.ContratoEnderecoCobrancaID = contrato.EnderecoCobrancaID;
                                cobranca.ContratoNumero = contrato.Numero;
                                cobranca.ContratoTitularNome = "";
                                cobranca.DataCriacao = DateTime.Now;

                                if (_cobrancas.Count > 1)
                                {
                                    cobranca.DataVencimento = _cobrancas[_cobrancas.Count - 1].DataVencimento.AddMonths(1);// new DateTime(ano, mes, contrato.Vencimento.Day, 23, 59, 59);

                                    if (_cobrancas[_cobrancas.Count - 1].Tipo != (int)eTipo.Normal)
                                    {
                                        //DateTime vigencia, vencimento;
                                        //Int32 diaDataSemJuros; Object valorDataLimite;
                                        //CalendarioVencimento rcv = null;
                                        //CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contrato.ContratoADMID,
                                        //    DateTime.Now, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                                        Cobranca ultimaCobrancaNormal = ultimaCobrancaNormalDaColecao(_cobrancas);
                                        if (ultimaCobrancaNormal != null)
                                        {
                                            cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year,
                                                cobranca.DataVencimento.Month, ultimaCobrancaNormal.DataVencimento.Day, 23, 59, 59, 900);
                                            ultimaCobrancaNormal = null;
                                        }
                                    }
                                }
                                else if (_cobrancas.Count == 0)
                                    cobranca.DataVencimento = new DateTime(contrato.Admissao.Year, contrato.Admissao.Month, contrato.Admissao.Day, 23, 59, 59, 500);
                                else
                                    cobranca.DataVencimento = new DateTime(contrato.Vencimento.Year, contrato.Vencimento.Month, contrato.Vencimento.Day, 23, 59, 59, 500);

                                cobranca.EstipulanteNome = "";
                                cobranca.FilialNome = "";
                                cobranca.OperadoraID = contrato.OperadoraID;
                                cobranca.OperadoraNome = "";
                                cobranca.Pago = false;
                                cobranca.Parcela = parcela;
                                cobranca.PropostaID = contrato.ID;
                                cobranca.Tipo     = (Int32)Cobranca.eTipo.Normal;
                                cobranca.Carteira = (Int32)carteira;

                                try
                                {
                                    vigencia = DateTime.MinValue; vencimento = DateTime.MinValue;
                                    diaDataSemJuros = -1; valorDataLimite = null; rcv = null;

                                    CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contrato.ContratoADMID,
                                        contrato.Admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                                    if (cobranca.DataVencimento.Day != vencimento.Day && vencimento != DateTime.MinValue)
                                    {
                                        cobranca.DataVencimento = new DateTime(
                                            cobranca.DataVencimento.Year,
                                            cobranca.DataVencimento.Month, vencimento.Day, 23, 59, 59, 900);
                                    }
                                }
                                catch
                                {
                                }

                                try
                                {
                                    cobranca.Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite, false);
                                }
                                catch (Exception ex)
                                {
                                    pm.CloseSingleCommandInstance();
                                    throw ex;
                                }

                                if (cobranca.Valor > 0)
                                {
                                    if (cobranca.DataVencimento >= venctoDe && cobranca.DataVencimento <= venctoAte)
                                    {
                                        try
                                        {
                                            pm.Save(cobranca);
                                            retornar.Add(cobranca);
                                            CobrancaComposite.Salvar(cobranca.ID, composite, pm);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }

                    pm.CloseSingleCommandInstance();
                }

                return retornar;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (pm != null)
                {
                    pm.CloseSingleCommandInstance();
                    pm.Dispose();
                    pm = null;
                }
            }
        }

        /// <summary>
        /// Versão 2
        /// </summary>
        public static IList<Cobranca> ProcessarCobrancasNormaisParaGerarRemessa(ArquivoRemessaCriterio.eTipoTaxa tipoTaxa, DateTime venctoDe, DateTime venctoAte, DateTime vigenciaDe, DateTime vigenciaAte, IList<ArquivoRemessaAgendamento> aras)
        {
            Decimal valorCobranca = 0;
            List<Cobranca> retornar = new List<Cobranca>();

            #region query 

            //if (aras[0].QtdBoletos > 1)
            //{
            //    venctoAte = venctoDe.AddMonths(aras[0].QtdBoletos - 1);
            //}

            venctoAte = aras[0].VencimentoAte; //new DateTime(venctoAte.Year, venctoAte.Month, venctoAte.Day, 23, 59, 59, 997);

            String condicaoTaxa = "";
            if(tipoTaxa == ArquivoRemessaCriterio.eTipoTaxa.SemTaxa)
                condicaoTaxa = " AND contrato_cobrarTaxaAssociativa=0 ";
            else if(tipoTaxa == ArquivoRemessaCriterio.eTipoTaxa.ComTaxa)
                condicaoTaxa = " AND contrato_cobrarTaxaAssociativa=1 ";

            String condicaoVigencia = String.Concat(" AND contrato_vigencia BETWEEN '", vigenciaDe.ToString("yyyy-MM-dd"), "' AND '", vigenciaAte.ToString("yyyy-MM-dd 23:59:59.997") + "' ");

            String formattedContratoAdmIds = "";
            ArquivoRemessaCriterio crit = null;
            foreach (ArquivoRemessaAgendamento _ara in aras)
            {
                crit = new ArquivoRemessaCriterio(_ara.CriterioID);
                crit.Carregar();

                if (formattedContratoAdmIds.Length > 0) { formattedContratoAdmIds += ","; }
                formattedContratoAdmIds = String.Concat(formattedContratoAdmIds, crit.ContratoAdmIDs);
            }

            String qry = String.Concat(
                "SELECT DISTINCT(contrato_id), contrato_operadoraid, contrato_contratoAdmId, filial_nome, estipulante_descricao, operadora_nome, contrato_numero, contrato_numeroId, contrato_vigencia, contrato_admissao, contrato_vencimento, contrato_vencimentoAlterado, contrato_enderecoCobrancaId, contrato_codcobranca ",
                "   FROM contrato ",
                "       LEFT JOIN usuario_filial a ON contrato_donoId = a.usuariofilial_usuarioId and a.usuariofilial_data = (select max(b.usuariofilial_data) from usuario_filial b where b.usuariofilial_usuarioId = a.usuariofilial_usuarioId) ",
                "       LEFT JOIN filial           ON filial_id = a.usuariofilial_filialId ",
                "       INNER JOIN operadora       ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante     ON estipulante_id=contrato_estipulanteId ",
//              "       INNER JOIN contrato_beneficiario ON contratobeneficiario_status NOT IN (10,15,17,21) AND contratobeneficiario_ativo=1 AND contratobeneficiario_contratoId=contrato_id ", //0,1, 

                "   WHERE ", Contrato.CondicaoBasicaQuery, //todo: denis tirar id de contrato hardcoded contrato_id in(43740) and
                "       AND contrato_contratoAdmId IN (", formattedContratoAdmIds, ") ", condicaoTaxa, condicaoVigencia, //" and contrato_id=43578 ", //TODO: denis excluir hard code de contrato id
                "   ORDER BY filial_nome, estipulante_descricao, operadora_nome, contrato_vencimento");

            //qry = "SELECT DISTINCT(contrato_id), contrato_operadoraid, contrato_contratoAdmId, filial_nome, estipulante_descricao, operadora_nome, contrato_numero, contrato_numeroId, contrato_vigencia,  contrato_admissao, contrato_vencimento, contrato_vencimentoAlterado, contrato_enderecoCobrancaId, contrato_codcobranca    FROM contrato        LEFT JOIN usuario_filial a ON contrato_donoId = a.usuariofilial_usuarioId and a.usuariofilial_data = (select max(b.usuariofilial_data) from usuario_filial b where b.usuariofilial_usuarioId = a.usuariofilial_usuarioId)        LEFT JOIN filial           ON filial_id = a.usuariofilial_filialId        INNER JOIN operadora       ON operadora_id=contrato_operadoraId        INNER JOIN estipulante     ON estipulante_id=contrato_estipulanteId    WHERE  contrato_id=43563 ORDER BY filial_nome, estipulante_descricao, operadora_nome, contrato_vencimento";

            #endregion

            PersistenceManager pm = null;
            Contrato _contrato = null;

            int kk = 0;
            try
            {
                DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];
                if (dt.Rows.Count == 0) { return null; }
                List<Contrato> contratos = new List<Contrato>();

                Contrato __contrato = null;
                foreach (DataRow row in dt.Rows)
                {
                    __contrato = new Contrato(row["contrato_id"]);
                    __contrato.OperadoraID = row["contrato_operadoraid"];
                    __contrato.ContratoADMID = row["contrato_contratoAdmId"];
                    __contrato.OperadoraDescricao = toString(row["operadora_nome"]);
                    __contrato.Numero = toString(row["contrato_numero"]);
                    __contrato.NumeroID = toObject(row["contrato_numeroId"]);
                    __contrato.Vigencia = toDateTime(row["contrato_vigencia"]);
                    __contrato.Admissao = toDateTime(row["contrato_admissao"]);
                    __contrato.Vencimento = toDateTime(row["contrato_vencimento"]);
                    __contrato.EnderecoCobrancaID = row["contrato_enderecoCobrancaId"];
                    __contrato.CodCobranca = toInt(row["contrato_codcobranca"]);
                    __contrato.VencimentoAlterado = toInt(row["contrato_vencimentoAlterado"]);

                    contratos.Add(__contrato);
                }

                dt.Dispose();

                IList<Cobranca> _cobrancas = null;
                List<Cobranca> _cobrancasTemp = null;
                IList<TabelaValor> tabelaValor = null;
                int parcela = 1;
                int i = 0;
                int cobrancasAdicionadas = 0;
                int indiceUltimaCobranca = 0;
                int dif = 0; //diferenca entre cobrancas geradas e cobrancas a serem geradas
                DateTime dataLimiteContratoAdm = DateTime.MinValue;

                int j = 0; List<CobrancaComposite> composite = null;

                pm = new PersistenceManager();
                pm.UseSingleCommandInstance();

                foreach (Contrato contrato in contratos)
                {
                    kk++;

                    System.Windows.Forms.Application.DoEvents();
                    cobrancasAdicionadas = 0;
                    _contrato = contrato;

                    //Para cada contrato gerar a cobrança.
                    //  checar se há uma cobrança ativa com as mesmas características. se existir, não gerar.
                    _cobrancas = Cobranca.CarregarTodas_Optimized(contrato.ID, true, eTipo.Normal, pm);

                    #region for qtd cobrancas  

                    if (_cobrancas == null || _cobrancas.Count == 0)
                    {
                        parcela = 1;

                        #region gera cobranca

                        Cobranca cobranca = new Cobranca();
                        cobranca.BeneficiarioEmail = "";
                        cobranca.BeneficiarioId = null;
                        cobranca.ContratoEnderecoCobrancaID = contrato.EnderecoCobrancaID;
                        cobranca.ContratoNumero = contrato.Numero;
                        cobranca.ContratoTitularNome = "";
                        cobranca.DataCriacao = DateTime.Now;
                        cobranca.DataVencimento = new DateTime(contrato.Admissao.Year, contrato.Admissao.Month, contrato.Admissao.Day, 23, 59, 59, 0);
                        cobranca.EstipulanteNome = "";
                        cobranca.FilialNome = "";
                        cobranca.OperadoraID = contrato.OperadoraID;
                        cobranca.OperadoraNome = "";
                        cobranca.Pago = false;
                        cobranca.Parcela = parcela;
                        cobranca.PropostaID = contrato.ID;
                        cobranca.Tipo = (Int32)Cobranca.eTipo.Normal;
                        cobranca.Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite, false);
                        cobranca.Carteira = aras[0].Carteira;

                        if (cobranca.Valor > 0)
                        {
                            if (contrato.Vencimento >= venctoDe && contrato.Vencimento <= venctoAte)
                            {
                                pm.Save(cobranca);
                                retornar.Add(cobranca);
                                CobrancaComposite.Salvar(cobranca.ID, composite, pm);
                                cobrancasAdicionadas++;
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        _cobrancasTemp = new List<Cobranca>();
                        int indiceCobranca = 0; indiceUltimaCobranca = 0;

                        foreach (Cobranca _existente in _cobrancas)
                        {
                            _cobrancasTemp.Add(_existente);

                            //pega o indice da ultima cobranca anteriormente criada
                            if (_existente.DataVencimento < venctoDe) { indiceUltimaCobranca = indiceCobranca; }
                            indiceCobranca++; i++;
                        }

                        i = 0;
                        //NAO pode gerar cobranca com data de vencimento alem de tabelaValor[0].VencimentoFim.AddMonths(1)
                        if ((_cobrancas != null && _cobrancas.Count == 1 && _cobrancas[0].Parcela == 1) || indiceUltimaCobranca == 0)             //_cobrancas[indiceUltimaCobranca].Parcela == 1)
                        {
                            //se for a parcela 2, nao pode simplesmente somar um mes, pois a data do 
                            //primeiro pagto é irregular. contudo, pode-se usar a data de primeiro vencimento,
                            //cadastrada no contrato.
                            tabelaValor = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, contrato.Vencimento, pm, true); //força vigencia
                        }
                        else
                        {
                            tabelaValor = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, _cobrancas[indiceUltimaCobranca].DataVencimento.AddMonths(1), pm, true); //força vigencia
                        }

                        if (tabelaValor != null && tabelaValor.Count > 0)
                        {
                            tabelaValor[0].VencimentoFim = new DateTime(tabelaValor[0].VencimentoFim.Year,
                                tabelaValor[0].VencimentoFim.Month, tabelaValor[0].VencimentoFim.Day, 23, 59, 59, 997);
                        }
                        else
                        {
                            //Logar a falha em obter uma tabela de valor
                            CobrancaFalha.LogFalhaTabelaValor(contrato.ID, _cobrancas[indiceUltimaCobranca].DataVencimento.AddMonths(1), pm, aras[0].ID);
                            continue;
                        }

                        if (cobrancasAdicionadas >= aras[0].QtdBoletos) { break; }

                        int incrementaDataVenctoAte = 0;
                        foreach (Cobranca _existente in _cobrancas)
                        {
                            if (cobrancasAdicionadas >= aras[0].QtdBoletos) { break; }

                            if (_existente.DataVencimento >= venctoDe &&
                                _existente.DataVencimento <= venctoAte.AddMonths(incrementaDataVenctoAte) &&
                                _existente.Tipo == (Int32)Cobranca.eTipo.Normal)
                            {
                                if (_existente.ArquivoIDUltimoEnvio == null && // nao enviado 
                                    _existente.Pago == false &&                // nao pago
                                    _existente.Cancelada == false)             // ativa
                                {
                                    incrementaDataVenctoAte++;

                                    if (i > 0)
                                    {
                                        //NAO CHECAR MAIS A ADIMPLENCIA. APENAS SE ESTÁ ATIVO
                                        //if (_cobrancas[indiceUltimaCobranca].Pago || //checa se cobrança anterior está paga ou nao está vencida
                                        //    _cobrancas[indiceUltimaCobranca].DataVencimento <= DateTime.Now)
                                        //{
                                            #region checa valor 

                                            valorCobranca = Contrato.CalculaValorDaProposta2(contrato.ID, _existente.DataVencimento, pm, false, true, ref composite, true); //força vigencia

                                            if (valorCobranca == 0)
                                            {
                                                CobrancaFalha.LogFalhaTabelaValor(contrato.ID, _existente.DataVencimento, pm, aras[0].ID);
                                                break;
                                            }

                                            if (_existente.Valor != valorCobranca)
                                            {
                                                _existente.Valor = valorCobranca;
                                                pm.Save(_existente);
                                            }
                                            #endregion
                                            _existente.Carteira = aras[0].Carteira;

                                            if (_existente.DataVencimento.Day != venctoDe.Day && contrato.VencimentoAlterado == 0) { break; }///////////// if (_existente.DataVencimento.Day < venctoDe.Day || _existente.DataVencimento.Day > venctoAte.Day)
                                            if (_existente.DataVencimento <= tabelaValor[0].VencimentoFim)
                                            {
                                                retornar.Add(_existente);
                                            }
                                            cobrancasAdicionadas++; j++;
                                        //}
                                    }
                                    else
                                    {
                                        #region checa valor 

                                        valorCobranca = Contrato.CalculaValorDaProposta2(contrato.ID, _existente.DataVencimento, pm, false, true, ref composite, true); //força vigenci
                                        if (valorCobranca == 0)
                                        {
                                            CobrancaFalha.LogFalhaTabelaValor(contrato.ID, _existente.DataVencimento, pm, aras[0].ID);
                                            break;
                                        }

                                        if (_existente.Valor != valorCobranca)
                                        {
                                            _existente.Valor = valorCobranca;
                                            pm.Save(_existente);
                                        }
                                        #endregion
                                        _existente.Carteira = aras[0].Carteira;

                                        if (_existente.DataVencimento.Day != venctoDe.Day && contrato.VencimentoAlterado == 0) { break; }///////////// if (_existente.DataVencimento.Day < venctoDe.Day || _existente.DataVencimento.Day > venctoAte.Day)
                                        if (_existente.DataVencimento <= tabelaValor[0].VencimentoFim)
                                        {
                                            retornar.Add(_existente);
                                        }
                                        cobrancasAdicionadas++; j++;
                                    }
                                }
                            }

                            i++;
                        } //foreach (Cobranca _existente in _cobrancas)

                        // Se não gerou todas as cobranças que devia e está adimplente (nao verifica mais a adimplencia)
                        //if (cobrancasAdicionadas < aras[0].QtdBoletos && _cobrancas[indiceUltimaCobranca].Pago)

                        // Se não gerou todas as cobranças que devia
                        if (cobrancasAdicionadas < aras[0].QtdBoletos)
                        {
                            dif = aras[0].QtdBoletos - cobrancasAdicionadas;

                            for (int k = 1; k <= dif; k++)
                            {
                                #region gera cobranca 

                                parcela = _cobrancasTemp[_cobrancasTemp.Count - 1].Parcela + 1;

                                Cobranca cobranca = new Cobranca();
                                cobranca.BeneficiarioEmail = "";
                                cobranca.BeneficiarioId = null;
                                cobranca.ContratoEnderecoCobrancaID = contrato.EnderecoCobrancaID;
                                cobranca.ContratoNumero = contrato.Numero;
                                cobranca.ContratoTitularNome = "";
                                cobranca.DataCriacao = DateTime.Now;

                                if (_cobrancasTemp.Count > 1)
                                {
                                    cobranca.DataVencimento = _cobrancasTemp[_cobrancasTemp.Count - 1].DataVencimento.AddMonths(1);// new DateTime(ano, mes, contrato.Vencimento.Day, 23, 59, 59);

                                    cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 500);
                                    if (cobranca.DataVencimento > venctoAte) { break; }

                                    if (_cobrancasTemp[_cobrancas.Count - 1].Tipo != (int)eTipo.Normal)
                                    {
                                        Cobranca ultimaCobrancaNormal = ultimaCobrancaNormalDaColecao(_cobrancasTemp);
                                        if (ultimaCobrancaNormal != null)
                                        {
                                            if (contrato.VencimentoAlterado == 0)
                                            {
                                                cobranca.DataVencimento = new DateTime(ultimaCobrancaNormal.DataVencimento.AddMonths(1).Year,
                                                    ultimaCobrancaNormal.DataVencimento.AddMonths(1).Month, ultimaCobrancaNormal.DataVencimento.Day, 23, 59, 59, 500);
                                            }
                                            else
                                            {
                                                cobranca.DataVencimento = new DateTime(ultimaCobrancaNormal.DataVencimento.AddMonths(1).Year,
                                                    ultimaCobrancaNormal.DataVencimento.AddMonths(1).Month, contrato.VencimentoAlterado, 23, 59, 59, 500);
                                            }

                                            ultimaCobrancaNormal = null;
                                        }
                                    }
                                }
                                else if (_cobrancasTemp == null || _cobrancasTemp.Count == 0)
                                {
                                    if (contrato.VencimentoAlterado == 0)
                                        cobranca.DataVencimento = new DateTime(contrato.Admissao.Year, contrato.Admissao.Month, contrato.Admissao.Day, 23, 59, 59, 500);
                                    else
                                        cobranca.DataVencimento = new DateTime(contrato.Admissao.Year, contrato.Admissao.Month, contrato.VencimentoAlterado, 23, 59, 59, 500);
                                }
                                else
                                {
                                    if (_cobrancasTemp[0].Parcela == 1)
                                    {
                                        if (contrato.VencimentoAlterado == 0)
                                            cobranca.DataVencimento = new DateTime(contrato.Vencimento.Year, contrato.Vencimento.Month, contrato.Vencimento.Day, 23, 59, 59, 500);
                                        else
                                            cobranca.DataVencimento = new DateTime(contrato.Vencimento.Year, contrato.Vencimento.Month, contrato.VencimentoAlterado, 23, 59, 59, 500);
                                    }
                                    else
                                    {
                                        if (contrato.VencimentoAlterado == 0)
                                        {
                                            cobranca.DataVencimento = new DateTime(
                                                aras[0].VencimentoDe.AddMonths(cobrancasAdicionadas).Year,
                                                aras[0].VencimentoDe.AddMonths(cobrancasAdicionadas).Month,
                                                contrato.Vencimento.Day, 23, 59, 59, 500);
                                        }
                                        else
                                        {
                                            cobranca.DataVencimento = new DateTime(
                                                aras[0].VencimentoDe.AddMonths(cobrancasAdicionadas).Year,
                                                aras[0].VencimentoDe.AddMonths(cobrancasAdicionadas).Month,
                                                contrato.VencimentoAlterado, 23, 59, 59, 500);
                                        }
                                    }
                                }

                                //if (cobranca.DataVencimento.Day < venctoDe.Day || cobranca.DataVencimento.Day > venctoAte.Day) { break; }
                                /////////////////////////////if (cobranca.DataVencimento.Day != venctoDe.Day) { CobrancaFalha.LogFalha(contrato.ID, cobranca.DataVencimento, "Dia de vencimento inválido.", pm); break; }//////////////////////

                                cobranca.EstipulanteNome = "";
                                cobranca.FilialNome = "";
                                cobranca.OperadoraID = contrato.OperadoraID;
                                cobranca.OperadoraNome = "";
                                cobranca.Pago = false;
                                cobranca.Parcela = parcela;
                                cobranca.PropostaID = contrato.ID;
                                cobranca.Tipo = (Int32)Cobranca.eTipo.Normal;

                                //if (cobranca.DataVencimento.Day == 7 && !aras[0].Legado)
                                //{
                                //    cobranca.DataVencimento = new DateTime(
                                //        cobranca.DataVencimento.Year,
                                //        cobranca.DataVencimento.Month, 10,
                                //        cobranca.DataVencimento.Hour,
                                //        cobranca.DataVencimento.Minute,
                                //        cobranca.DataVencimento.Second,
                                //        499);
                                //}

                                //para contratos para os quais nao foram emitidas cobrancas ja faz tempo
                                if (cobranca.DataVencimento < venctoDe && cobranca.DataVencimento.Month != venctoDe.Month)
                                {
                                    cobranca.DataVencimento = new DateTime(
                                        venctoDe.Year, venctoDe.Month, venctoDe.Day, 23, 59, 59, 500); 
                                    cobranca.DataVencimentoForcada = true;

                                    CobrancaFalha.LogFalha(contrato.ID, cobranca.DataVencimento, "Furo na cadeia de cobranças", pm, aras[0].ID);
                                    //break;
                                }
                                ///////////////////////////////////////////////////////////////////////////

                                tabelaValor = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cobranca.DataVencimento, pm, true); //força vigencia
                                if (tabelaValor == null || tabelaValor.Count == 0)
                                {
                                    CobrancaFalha.LogFalhaTabelaValor(contrato.ID, cobranca.DataVencimento, pm, aras[0].ID);
                                    break;
                                }

                                try
                                {
                                    cobranca.Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite, true); //força vigencia
                                    if (cobranca.Valor == 0)
                                    {
                                        CobrancaFalha.LogFalhaTabelaValor(contrato.ID, cobranca.DataVencimento, pm, aras[0].ID);
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    pm.CloseSingleCommandInstance();
                                    throw ex;
                                }

                                if (cobranca.Valor > 0)
                                {

                                    if (cobranca.DataVencimento.Day != contrato.Vencimento.Day)
                                    {
                                        cobranca.DataVencimento = new DateTime(
                                            cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, contrato.Vencimento.Day,
                                            cobranca.DataVencimento.Hour, cobranca.DataVencimento.Minute, cobranca.DataVencimento.Second,
                                            cobranca.DataVencimento.Millisecond);
                                    }

                                    _cobrancasTemp.Add(cobranca);

                                    if (cobranca.DataVencimento >= venctoDe && cobranca.DataVencimento <= venctoAte && cobranca.DataVencimento <= tabelaValor[0].VencimentoFim) 
                                    {
                                        cobranca.Carteira = aras[0].Carteira;

                                        Cobranca cob = Cobranca.CarregarPor(cobranca.PropostaID, cobranca.DataVencimento, (int)Cobranca.eTipo.Normal, pm); 
                                        if (cob == null)
                                        {
                                            try
                                            {
                                                //cobranca.DataVencimento = new DateTime(2016, 12, 10, 23, 59, 59, 996);//TODO: Denis, deletar isso
                                                pm.Save(cobranca);
                                                CobrancaComposite.Salvar(cobranca.ID, composite, pm);
                                                retornar.Add(cobranca);
                                                cobrancasAdicionadas++; j++;
                                            }
                                            catch 
                                            {
                                                CobrancaFalha.LogFalha(cobranca.PropostaID, cobranca.DataVencimento, string.Concat("Duplicidade para a parcela ", cobranca.Parcela.ToString()), pm, aras[0].ID);
                                                break;
                                            }
                                        }
                                    }
                                    else if (cobranca.DataVencimento > venctoAte)
                                        break;
                                    else if (cobranca.DataVencimento > tabelaValor[0].VencimentoFim)
                                    {
                                        CobrancaFalha.LogFalha(contrato.ID, cobranca.DataVencimento, "Data de vencimento excede vencimento final da tabela. ContratoADM: " + contrato.ContratoADMID, pm, aras[0].ID);
                                        break;
                                    }
                                    else
                                        k--;
                                }
                                #endregion
                            }
                        }
                    }

                    #endregion

                } //foreach (Contrato contrato in contratos)

                pm.CloseSingleCommandInstance();

                return retornar;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pm != null)
                {
                    pm.CloseSingleCommandInstance();
                    pm.Dispose();
                    pm = null;
                }
            }
        }

        public static IList<Cobranca> ProcessarNormais_TEMP_DEVIDO_ERRO_GERACAO_MES06()
        {
            DateTime agora = DateTime.Now;
            List<Cobranca> geradas = new List<Cobranca>();
            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    var cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>("select * from cobranca where cobranca_tipotemp=255", typeof(Cobranca), pm);
                    //var cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>("select * from cobranca where cobranca_arquivoUltimoEnvioId in (69,70)", typeof(Cobranca), pm);

                    //foreach (var cob in cobrancas)
                    //{
                    //    cob.ID = null;
                    //    cob.Cancelada = false;
                    //    cob.DataVencimento = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, 7, 23, 59, 59, 995);
                    //    cob.ArquivoIDUltimoEnvio = -99;
                    //    cob.NossoNumero = null;
                    //    cob.DataCriacao = agora;
                    //    cob.TipoTemp = 255;

                    //    pm.Save(cob);
                    //    geradas.Add(cob);
                    //}

                    GeraDocumentoCobranca_TEMP_DEVIDO_ERRO_GERACAO_MES06(pm, geradas);

                    pm.Commit();
                    return geradas;
                }
                catch
                {
                    pm.Rollback();
                    return null;
                }
                finally
                {
                    pm.Dispose();
                }
            }
        }

        public static String GeraDocumentoCobranca_TEMP_DEVIDO_ERRO_GERACAO_MES06 (PersistenceManager _pm, List<Cobranca> cobrancasObj)
        {
            List<string> cobrancaIDs = new List<string>();
            foreach (var c in cobrancasObj)
            {
                if (cobrancaIDs.Contains(Convert.ToString(c.ID))) continue;

                cobrancaIDs.Add(Convert.ToString(c.ID));
            }

            Int32 numSequencial = 0;

            PersistenceManager pm = _pm;

            Int32 contratoadmQualicorpID = Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]);

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();
            }

            String headerCedente = "UBRASP - UNIAO BRASILEIRA";
            string agencia = "0001"; //, agenciadv = "0";
            string numcontacorr = "13014765";
            string dvcontacorr = "2";
            string cnpj = "49938327000106"; //
            string codTransmissao = "000100001201344";

            string numConvenio = "1201344"; // Cobranca.ConvenioUBRASP.PadLeft(9, '0');

            int numeroDoArquivo = 1;
            string[] arquivos = Directory.GetFiles(ConfigurationManager.AppSettings["financialFilePathFISICO"], "*.dat");
            if (arquivos != null && arquivos.Length > 0) numeroDoArquivo = arquivos.Length + 1;
            if (numeroDoArquivo > 999) numeroDoArquivo = 1 + (arquivos.Length - 999);

            try
            {
                String dataAgora = DateTime.Now.ToString("ddMMyyyy");
                StringBuilder doc = new StringBuilder();

                //string arquivoNome = "arquivo.dat"; // 

                #region HEADER -------------------------------------------

                EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0000", 4);         //lote de serviço
                EntityBase.AppendPreparedField(ref doc, "0", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "        ", 8);     //brancos
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //tipo de inscrição - 2 para CNPJ, 1 para CPF

                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);          //CNPJ (ou cpf) 
                EntityBase.AppendPreparedField(ref doc, codTransmissao.PadLeft(15, '0'), 15); //Codigo de transmissao 
                EntityBase.AppendPreparedField(ref doc, " ", 25);           //brancos
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //nome empresa
                EntityBase.AppendPreparedField(ref doc, "Banco Santander".PadRight(30, ' '), 30); //Nome do Banco
                EntityBase.AppendPreparedField(ref doc, " ", 10);           //brancos
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //Código Remessa: 1 = remessa
                EntityBase.AppendPreparedField(ref doc, dataAgora, 8);      //data de geracao do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 6);            //brancos
                EntityBase.AppendPreparedField(ref doc, numeroDoArquivo.ToString().PadLeft(6, '0'), 6); //brancos
                EntityBase.AppendPreparedField(ref doc, "040", 3);          //versão do layout do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 74);           //brancos

                #endregion

                #region HEADER DE LOTE -----------------------------------

                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "R", 1);            //tipo de operação
                EntityBase.AppendPreparedField(ref doc, "01", 2);           //tipo de serviço
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //brancos
                EntityBase.AppendPreparedField(ref doc, "030", 3);          //Nº da Versão do Layout do Lote - nao criticado pelo banco
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //Tipo de Inscrição da Empresa - 2=CNPJ
                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);  //CNPJ (ou cpf) //faltou: cnpj
                EntityBase.AppendPreparedField(ref doc, " ", 20);           //brancos
                EntityBase.AppendPreparedField(ref doc, codTransmissao.PadLeft(15, '0'), 15);//Codigo de transmissao 
                EntityBase.AppendPreparedField(ref doc, " ", 05);           //brancos
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome do beneficiario
                EntityBase.AppendPreparedField(ref doc, " ", 40);           //Mensagem 1
                EntityBase.AppendPreparedField(ref doc, " ", 40);           //Mensagem 2
                EntityBase.AppendPreparedField(ref doc, "0001".PadLeft(8, '0'), 8); //Número remessa/retorno
                EntityBase.AppendPreparedField(ref doc, dataAgora, 8);      //data de gravaçao do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 41);           //brancos

                #endregion

                //Carrega cobrancas envolvidas
                IList<Cobranca> _cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela_Optimized(cobrancaIDs, pm);
                if (_cobrancas == null) { return null; }
                List<Object> cobrancaIds = new List<Object>();

                IList<Cobranca> cobrancas = new List<Cobranca>();
                foreach (var c in _cobrancas)
                {
                    cobrancas.Add(c);
                }

                if (cobrancas == null || cobrancas.Count == 0) { return null; }

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL -------------------------------------------

                Endereco endereco = null;
                //, diaDataSemJuros, result = 0, 
                int totalRegistrosDetalhe = 0;
                String mascara6 = new String('0', 6); //, nossoNumero = "";
                Decimal valorTotal = 0; //, mora = 0, multa = 0;
                //Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                //CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                int i = 0;

                System.Collections.Hashtable contratoADM_Criterio = new System.Collections.Hashtable();
                List<string> parametrosProcessados = new List<string>();

                foreach (Cobranca cob in cobrancas)
                {
                    if (parametrosProcessados.Contains(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID))) continue;

                    parametrosProcessados.Add(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID));

                    i++;
                    try
                    {
                        #region DETAIL P ---------------------------------

                        #region

                        #region localiza o critério correspondente

                        contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                        #endregion

                        numSequencial++;

                        cobrancaIds.Add(cob.ID);

                        if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                        {
                            numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                            qtdTotalBoletos = 0;
                            foreach (Cobranca _temp in cobrancas)
                            {
                                if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                                else if (qtdTotalBoletos > 0) { break; }
                            }
                        }

                        numBoletoCorrente++;

                        #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:
                        #region data de admissao do contrato

                        //Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                        else { continue; }
                        #endregion

                        #endregion

                        doc.Append(Environment.NewLine);

                        EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Numero do lote remessa
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº seqüencial do registro de lote
                        EntityBase.AppendPreparedField(ref doc, "P", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de movimento remessa (pag3) - nota 14 (pag.14)
                        EntityBase.AppendPreparedField(ref doc, agencia, 4);        //Agência do Destinatária FIDC
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Dígito da Agência do Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, numcontacorr.PadLeft(9, '0'), 9); //Número da conta corrente
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);            //Dígito da conta corrente (definido pela homologação)
                        EntityBase.AppendPreparedField(ref doc, numcontacorr.PadLeft(9, '0'), 9); //Conta cobrança Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1); //Dígito da conta cobrança Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, "  ", 2);           //branco
                        cob.NossoNumero = cob.GeraNossoNumeroSantander();
                        EntityBase.AppendPreparedField(ref doc, cob.NossoNumero, 13); //Identificação do título no Banco
                        EntityBase.AppendPreparedField(ref doc, "5", 1);            //Tipo de cobrança 
                        EntityBase.AppendPreparedField(ref doc, "1", 1);            //Forma de Cadastramento - 1 para cobranca registrada
                        EntityBase.AppendPreparedField(ref doc, "1", 1);            //Tipo de documento - 1- Tradicional , 2- Escritural
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(15, '0'), 15); //Seu número
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyyyy"), 8);  // Data de vencimento - pos 78

                        //valor do boleto
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[0].Replace(".", "").PadLeft(13, '0'), 13);
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[1].PadLeft(2, '0'), 2);

                        EntityBase.AppendPreparedField(ref doc, "0000", 4);         //Agência encarregada da cobrança
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Dígito da Agência do Beneficiário
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "02", 2);           //Espécie do título Nota 20 pag.15 ???
                        EntityBase.AppendPreparedField(ref doc, "N", 1);            //Identif. de título Aceito/Não Aceito
                        EntityBase.AppendPreparedField(ref doc, cob.DataCriacao.ToString("ddMMyyyy"), 8); //Data da emissão do título
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //Código do juros de mora - nota 21 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8); //Data do juros de mora - nota 22
                        EntityBase.AppendPreparedField(ref doc, "0".PadRight(15, '0'), 15); //Valor da mora/dia ou Taxa mensal - nota 18 pag.15
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Código do desconto 1 - nota 23 pag.16
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8);     //Data de desconto 1 - nota 23 pag.16
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor ou Percentual do desconto concedido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do IOF a ser recolhido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do abatimento

                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(25, '0'), 25); //Identificação do título na empresa
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Código para protesto - nota 25 pag.16 - 0=nao protestar
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Número de dias para protesto
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //Código para Baixa/Devolução - nota 26 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Número de dias para Baixa/Devolução
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Código da moeda - nota 27 - pag.16
                        EntityBase.AppendPreparedField(ref doc, " ".PadRight(11, ' '), 11); //branco

                        //
                        #endregion

                        #endregion

                        #region DETAIL Q ---------------------------------

                        doc.Append(Environment.NewLine);

                        numSequencial++;

                        EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Numero do lote remessa
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº seqüencial do registro no lote - nota 11 - pg.14
                        EntityBase.AppendPreparedField(ref doc, "Q", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de movimento remessa - nota 14 - pag 14

                        EntityBase.AppendPreparedField(ref doc, "1", 1);           //Tipo de inscrição do Pagador - nota 29 - pag.17 - 1 = cpf
                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularCPF.PadLeft(15, '0'), 15); //Número de inscrição do Pagador - pag. 4

                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 40); //Nome Pagador

                        endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                        endereco.Carregar(pm);
                        if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                        if (endereco.Numero == null) endereco.Numero = "";
                        if (string.IsNullOrEmpty(endereco.Complemento))
                        {
                            int tamanhoDisponivel = 40 - (endereco.Numero.Length + 1);

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero);
                        }
                        else
                        {
                            int tamanhoDisponivel = 40 - ((endereco.Numero.Length + 1) + (endereco.Complemento.Length + 1));

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero, ",", endereco.Complemento);
                            else
                                endereco.Logradouro += string.Concat(",", endereco.Complemento);
                        }

                        if (string.IsNullOrEmpty(endereco.CEP)) { endereco.CEP = "00000000"; } else { endereco.CEP = endereco.CEP.Replace("-", ""); }
                        EntityBase.AppendPreparedField(ref doc, endereco.Logradouro, 40); // 
                        EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP, 8);
                        EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);

                        //if(string.IsNullOrEmpty(cob.ContratoTitularCPF) || cob.ContratoTitularCPF == "00000000000")
                        EntityBase.AppendPreparedField(ref doc, "0", 1); //Tipo de inscrição Sacador/avalista - nota 29 - pag.17
                        //else
                        //    EntityBase.AppendPreparedField(ref doc, "1", 1); //Tipo de inscrição Sacador/avalista - nota 29 - pag.17

                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Nº de inscrição Sacador/avalista

                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(40, ' '), 40); //Nome do Sacador/Avalista

                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Identificador de carne - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Sequencial da Parcela ou número inicial da parcela - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Quantidade total de parcelas - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Número do plano - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, " ".PadRight(19, ' '), 19); //Brancos

                        #endregion

                        totalRegistrosDetalhe++;
                        valorTotal += cob.Valor;

                        cob.Carteira = 101;

                        pm.Save(cob);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string path = System.Configuration.ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                            System.IO.File.WriteAllText(path + "err_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".txt",
                                ex.Message +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + ex.StackTrace +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + Convert.ToString(cob.ID) +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + i.ToString(),
                                System.Text.Encoding.ASCII);
                        }
                        catch { }
                        continue;
                    }
                }
                #endregion

                #region TRAILLER DE LOTE ---------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "033", 3);
                EntityBase.AppendPreparedField(ref doc, "0001", 4);
                EntityBase.AppendPreparedField(ref doc, "5", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9);
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 2).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 217);

                #endregion

                #region TRAILLER -----------------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "033", 3);
                EntityBase.AppendPreparedField(ref doc, "9999", 4); //9999 - Numero do lote remessa
                EntityBase.AppendPreparedField(ref doc, "9", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9); //branco
                EntityBase.AppendPreparedField(ref doc, "1".PadLeft(6, '0'), 6); //qtd de lotes - registros tipo 1
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 4).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 211); //

                #endregion


                if (_pm == null) { pm.Commit(); } //se não está partcipando de uma transacao externa, comita //pm.Rollback();
                string arquivoConteudo = doc.ToString();
                return arquivoConteudo;
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }


        /// <summary>
        /// Versão 2 - LEGADO
        /// </summary>
        public static IList<Cobranca> ProcessarCobrancasNormaisParaGerarRemessaLEGADO(ArquivoRemessaCriterio.eTipoTaxa tipoTaxa, DateTime venctoDe, DateTime venctoAte, DateTime vigenciaDe, DateTime vigenciaAte, ArquivoRemessaAgendamento ara)//IList<ArquivoRemessaAgendamento> aras)
        {
            List<Cobranca> retornar = new List<Cobranca>();

            #region query

            venctoAte = new DateTime(venctoAte.Year, venctoAte.Month, venctoAte.Day, 23, 59, 59, 997);

            String condicaoTaxa = "";
            //if (tipoTaxa == ArquivoRemessaCriterio.eTipoTaxa.SemTaxa)
            //    condicaoTaxa = " AND contrato_cobrarTaxaAssociativa=0 ";
            //else if (tipoTaxa == ArquivoRemessaCriterio.eTipoTaxa.ComTaxa)
            //    condicaoTaxa = " AND contrato_cobrarTaxaAssociativa=1 ";

            String condicaoVigencia = String.Concat(" AND contrato_vigencia BETWEEN '", vigenciaDe.ToString("yyyy-MM-dd"), "' AND '", vigenciaAte.ToString("yyyy-MM-dd 23:59:59.997") + "' ");

            String qry = String.Concat(
                "SELECT DISTINCT(contrato_id), contrato_operadoraid, contrato_contratoAdmId, filial_nome, estipulante_descricao, operadora_nome, contrato_numero, contrato_numeroId, contrato_vigencia, contrato_admissao, contrato_vencimento, contrato_enderecoCobrancaId, contrato_codcobranca ",
                "   FROM contrato ",
                "       LEFT JOIN usuario_filial a ON contrato_donoId = a.usuariofilial_usuarioId and a.usuariofilial_data = (select max(b.usuariofilial_data) from usuario_filial b where b.usuariofilial_usuarioId = a.usuariofilial_usuarioId) ",
                "       LEFT JOIN filial           ON filial_id = a.usuariofilial_filialId ",
                "       INNER JOIN operadora       ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante     ON estipulante_id=contrato_estipulanteId ",

                "   WHERE ", Contrato.CondicaoBasicaQuery,
                condicaoTaxa, condicaoVigencia, //"       AND contrato_contratoAdmId IN (", formattedContratoAdmIds, ") ", 
                "   ORDER BY filial_nome, estipulante_descricao, operadora_nome, contrato_vencimento");

            #endregion

            PersistenceManager pm = null;
            Contrato _contrato = null;
            IList<Cobranca> _cobrancas = null;

            int kk = 0;
            try
            {
                if (ara.SomenteNaoRecorrentes)
                {
                    _cobrancas = Cobranca.CarregarTodas_Optimized_DIF_UBRASP(pm);

                    if (_cobrancas != null) retornar.AddRange(_cobrancas);
                        
                   return retornar;
                }
                else if (ara.SomenteBoletosUBRASP)
                {
                    _cobrancas = Cobranca.CarregarTodas_Optimized_BOLETO_UBRASP(pm);

                    if (_cobrancas != null) retornar.AddRange(_cobrancas);

                    return retornar;
                }
                else
                {
                    DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];
                    if (dt.Rows.Count == 0) { return null; }
                    List<Contrato> contratos = new List<Contrato>();

                    Contrato __contrato = null;
                    foreach (DataRow row in dt.Rows)
                    {
                        __contrato = new Contrato(row["contrato_id"]);
                        __contrato.OperadoraID = row["contrato_operadoraid"];
                        __contrato.ContratoADMID = row["contrato_contratoAdmId"];
                        __contrato.OperadoraDescricao = toString(row["operadora_nome"]);
                        __contrato.Numero = toString(row["contrato_numero"]);
                        __contrato.NumeroID = toObject(row["contrato_numeroId"]);
                        __contrato.Vigencia = toDateTime(row["contrato_vigencia"]);
                        __contrato.Admissao = toDateTime(row["contrato_admissao"]);
                        __contrato.Vencimento = toDateTime(row["contrato_vencimento"]);
                        __contrato.EnderecoCobrancaID = row["contrato_enderecoCobrancaId"];
                        __contrato.CodCobranca = toInt(row["contrato_codcobranca"]);

                        contratos.Add(__contrato);
                    }

                    dt.Dispose();

                    //List<Cobranca> _cobrancasTemp = null;
                    //IList<TabelaValor> tabelaValor = null;
                    int parcela = 1;
                    //int i = 0;
                    int cobrancasAdicionadas = 0;
                    //int indiceUltimaCobranca = 0;
                    //int dif = 0; //diferenca entre cobrancas geradas e cobrancas a serem geradas
                    DateTime dataLimiteContratoAdm = DateTime.MinValue;

                    int j = 0; List<CobrancaComposite> composite = null;

                    pm = new PersistenceManager();
                    pm.UseSingleCommandInstance();

                    foreach (Contrato contrato in contratos)
                    {
                        kk++;

                        System.Windows.Forms.Application.DoEvents();
                        //cobrancasAdicionadas = 0;
                        _contrato = contrato;

                        #region for qtd cobrancas

                        //Para cada contrato gerar a cobrança.
                        //  checar se há uma cobrança ativa com as mesmas características. se existir, não gerar.
                        _cobrancas = Cobranca.CarregarTodas_Optimized(contrato.ID, true, eTipo.Normal, pm);

                        if ((_cobrancas == null || _cobrancas.Count == 0))
                        {
                            parcela = 1;

                            #region gera cobranca

                            Cobranca cobranca = new Cobranca();
                            cobranca.BeneficiarioEmail = "";
                            cobranca.BeneficiarioId = null;
                            cobranca.ContratoEnderecoCobrancaID = contrato.EnderecoCobrancaID;
                            cobranca.ContratoNumero = contrato.Numero;
                            cobranca.ContratoTitularNome = "";
                            cobranca.DataCriacao = DateTime.Now;
                            cobranca.DataVencimento = new DateTime(ara.VencimentoDe.Year, ara.VencimentoDe.Month, ara.VencimentoDe.Day, 23, 59, 59, 0); //new DateTime(contrato.Admissao.Year, contrato.Admissao.Month, contrato.Admissao.Day, 23, 59, 59, 0);
                            cobranca.EstipulanteNome = "";
                            cobranca.FilialNome = "";
                            cobranca.OperadoraID = contrato.OperadoraID;
                            cobranca.OperadoraNome = "";
                            cobranca.Pago = false;
                            cobranca.Parcela = parcela;
                            cobranca.PropostaID = contrato.ID;
                            cobranca.Tipo = (Int32)Cobranca.eTipo.Normal;
                            cobranca.Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite, false);
                            cobranca.Carteira = 2; // aras[0].Carteira;

                            if (cobranca.Valor > 0)
                            {
                                //if (contrato.Vencimento >= venctoDe && contrato.Vencimento <= venctoAte)
                                //{
                                pm.Save(cobranca);
                                retornar.Add(cobranca);
                                CobrancaComposite.Salvar(cobranca.ID, composite, pm);
                                cobrancasAdicionadas++;
                                //}
                            }

                            #endregion

                            for (int qtdCobrancas = 2; qtdCobrancas <= ara.QtdBoletos; qtdCobrancas++)
                            {
                                parcela = qtdCobrancas;

                                #region gera cobranca

                                cobranca = new Cobranca();
                                cobranca.BeneficiarioEmail = "";
                                cobranca.BeneficiarioId = null;
                                cobranca.ContratoEnderecoCobrancaID = contrato.EnderecoCobrancaID;
                                cobranca.ContratoNumero = contrato.Numero;
                                cobranca.ContratoTitularNome = "";
                                cobranca.DataCriacao = DateTime.Now;
                                cobranca.DataVencimento = new DateTime(ara.VencimentoDe.AddMonths(qtdCobrancas - 1).Year, ara.VencimentoDe.AddMonths(qtdCobrancas - 1).Month, ara.VencimentoDe.AddMonths(qtdCobrancas - 1).Day, 23, 59, 59, 0); //new DateTime(contrato.Admissao.Year, contrato.Admissao.Month, contrato.Admissao.Day, 23, 59, 59, 0);
                                cobranca.EstipulanteNome = "";
                                cobranca.FilialNome = "";
                                cobranca.OperadoraID = contrato.OperadoraID;
                                cobranca.OperadoraNome = "";
                                cobranca.Pago = false;
                                cobranca.Parcela = parcela;
                                cobranca.PropostaID = contrato.ID;
                                cobranca.Tipo = (Int32)Cobranca.eTipo.Normal;
                                cobranca.Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite, true);
                                cobranca.Carteira = 2; // aras[0].Carteira;

                                if (cobranca.Valor > 0)
                                {
                                    //if (contrato.Vencimento >= venctoDe && contrato.Vencimento <= venctoAte)
                                    //{
                                    pm.Save(cobranca);
                                    retornar.Add(cobranca);
                                    CobrancaComposite.Salvar(cobranca.ID, composite, pm);
                                    cobrancasAdicionadas++;
                                    //}
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            parcela = _cobrancas[_cobrancas.Count - 1].Parcela + 1;
                            bool _break = false;

                            for (int qtdCobrancas = 1; qtdCobrancas <= ara.QtdBoletos; qtdCobrancas++)
                            {
                                #region gera cobranca

                                _break = false;

                                Cobranca cobranca = new Cobranca();
                                cobranca.BeneficiarioEmail = "";
                                cobranca.BeneficiarioId = null;
                                cobranca.ContratoEnderecoCobrancaID = contrato.EnderecoCobrancaID;
                                cobranca.ContratoNumero = contrato.Numero;
                                cobranca.ContratoTitularNome = "";
                                cobranca.DataCriacao = DateTime.Now;
                                cobranca.DataVencimento = new DateTime(ara.VencimentoDe.AddMonths(qtdCobrancas - 1).Year, ara.VencimentoDe.AddMonths(qtdCobrancas - 1).Month, ara.VencimentoDe.AddMonths(qtdCobrancas - 1).Day, 23, 59, 59, 0); //new DateTime(contrato.Admissao.Year, contrato.Admissao.Month, contrato.Admissao.Day, 23, 59, 59, 0);
                                cobranca.EstipulanteNome = "";
                                cobranca.FilialNome = "";
                                cobranca.OperadoraID = contrato.OperadoraID;
                                cobranca.OperadoraNome = "";
                                cobranca.Pago = false;
                                cobranca.Parcela = parcela;
                                cobranca.PropostaID = contrato.ID;
                                cobranca.Tipo = (Int32)Cobranca.eTipo.Normal;
                                cobranca.Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite, true);
                                cobranca.Carteira = 2; // aras[0].Carteira;

                                foreach (var aux in _cobrancas)
                                {
                                    if (aux.DataVencimento.Day == cobranca.DataVencimento.Day &&
                                        aux.DataVencimento.Year == cobranca.DataVencimento.Year &&
                                        aux.DataVencimento.Month == cobranca.DataVencimento.Month)
                                    {
                                        //JA TEM UMA COBRANCA COM ESSA DATA DE VENCIMENTO
                                        _break = true;
                                        break;
                                    }
                                }

                                if (_break) continue;

                                if (cobranca.Valor > 0)
                                {
                                    //if (contrato.Vencimento >= venctoDe && contrato.Vencimento <= venctoAte)
                                    //{
                                    pm.Save(cobranca);
                                    retornar.Add(cobranca);
                                    CobrancaComposite.Salvar(cobranca.ID, composite, pm);
                                    cobrancasAdicionadas++;
                                    //}
                                }

                                #endregion

                                parcela++;
                            }
                        }


                        #endregion

                    } //foreach (Contrato contrato in contratos)

                    pm.CloseSingleCommandInstance();

                }

                return retornar;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pm != null)
                {
                    pm.CloseSingleCommandInstance();
                    pm.Dispose();
                    pm = null;
                }
            }
        }

        [Obsolete("Usar a outra sobrecarga disponível.", true)]
        static IList<Cobranca> __ProcessarCobrancasNormaisParaGerarRemessa(String[] estipulanteIDs, String[] operadoraIDs, String[] filialIDs, Int32 mes, Int32 ano)
        {
            #region query 

            String filIN = String.Join(",", filialIDs);
            String opeIN = String.Join(",", operadoraIDs);
            String estIN = String.Join(",", estipulanteIDs);

            String qry = String.Concat(
                "SELECT DISTINCT(contrato_id), contrato_operadoraid, filial_nome, estipulante_descricao, operadora_nome, contrato_numero, contrato_numeroId, contrato_vigencia, contrato_admissao, contrato_vencimento, contrato_enderecoCobrancaId, contrato_codcobranca ",
                "   FROM contrato ",
                "       INNER JOIN usuario_filial   ON contrato_donoId=usuariofilial_usuarioId ",
                "       INNER JOIN filial           ON filial_id=usuariofilial_filialId ",
                "       INNER JOIN operadora        ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante      ON estipulante_id=contrato_estipulanteId ",
                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_status NOT IN (0,1,10,15,17,21) AND contratobeneficiario_contratoId=contrato_id ",
                "   WHERE ", Contrato.CondicaoBasicaQuery, " AND ",                                         
                "       usuariofilial_filialId IN (", filIN, ") AND ",
                "       contrato_estipulanteId IN (", estIN, ") AND ",
                "       contrato_operadoraId IN (", opeIN, ") ", 
                "   ORDER BY filial_nome, estipulante_descricao, operadora_nome, contrato_vencimento");

                #endregion

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();
            List<CobrancaComposite> composite = null;

            try
            {
                IList<Contrato> contratos = LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato), pm);

                if (contratos == null) { return null; }

                IList<Cobranca> _cobrancas = null;
                List<Cobranca> retornar = new List<Cobranca>();
                int parcela = 1; Boolean existente = false;
                int i = 0;
                foreach (Contrato contrato in contratos)
                {
                    //Para cada contrato gerar a cobrança.
                    //  checar se há uma cobrança ativa com as mesmas características. se existir, não gerar.
                    _cobrancas = Cobranca.CarregarTodas(contrato.ID, true, eTipo.Normal, pm);

                    if (_cobrancas == null || _cobrancas.Count == 0)
                    {
                        parcela = 1;

                        #region gera cobranca

                        Cobranca cobranca = new Cobranca();
                        cobranca.BeneficiarioEmail = "";
                        cobranca.BeneficiarioId = null;
                        cobranca.ContratoEnderecoCobrancaID = contrato.EnderecoCobrancaID;
                        cobranca.ContratoNumero = contrato.Numero;
                        cobranca.ContratoTitularNome = "";
                        cobranca.DataCriacao = DateTime.Now;
                        cobranca.DataVencimento = new DateTime(ano, mes, contrato.Vencimento.Day, 23, 59, 59, 0);
                        cobranca.EstipulanteNome = "";
                        cobranca.FilialNome = "";
                        cobranca.OperadoraID = contrato.OperadoraID;
                        cobranca.OperadoraNome = "";
                        cobranca.Pago = false;
                        cobranca.Parcela = parcela;
                        cobranca.PropostaID = contrato.ID;
                        cobranca.Tipo = (Int32)Cobranca.eTipo.Normal;
                        cobranca.Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite, false);
                        if (cobranca.Valor > 0)
                        {
                            pm.Save(cobranca);
                            retornar.Add(cobranca);
                            CobrancaComposite.Salvar(cobranca.ID, composite, pm);
                        }

                        #endregion
                    }
                    else
                    {
                        existente = false;
                        i = 0;

                        foreach (Cobranca _existente in _cobrancas)
                        {
                            if (_existente.DataVencimento.Month == mes &&
                                _existente.DataVencimento.Year == ano &&
                                _existente.Tipo == (Int32)Cobranca.eTipo.Normal)
                            {
                                if (_existente.ArquivoIDUltimoEnvio == null && // nao enviado
                                    _existente.Pago == false &&                // nao pago
                                    _existente.Cancelada == false)             // ativa
                                {
                                    if (i > 0)
                                    {
                                        if (_cobrancas[i - 1].Pago) //checa se cobrança anterior está paga
                                        {
                                            retornar.Add(_existente);
                                        }
                                    }
                                    else
                                    {
                                        retornar.Add(_existente);
                                    }
                                }

                                existente = true;
                                break;
                            }

                            i++;
                        }

                        if (!existente) //cria uma nova cobranca
                        {
                            if (_cobrancas.Count == 0 ||
                                _cobrancas[_cobrancas.Count - 1].Pago) //checa se cobrança anterior está paga
                            {
                                parcela = _cobrancas.Count + 1;

                                #region gera cobranca

                                Cobranca cobranca = new Cobranca();
                                cobranca.BeneficiarioEmail = "";
                                cobranca.BeneficiarioId = null;
                                cobranca.ContratoEnderecoCobrancaID = contrato.EnderecoCobrancaID;
                                cobranca.ContratoNumero = contrato.Numero;
                                cobranca.ContratoTitularNome = "";
                                cobranca.DataCriacao = DateTime.Now;
                                cobranca.DataVencimento = new DateTime(ano, mes, contrato.Vencimento.Day, 23, 59, 59);
                                cobranca.EstipulanteNome = "";
                                cobranca.FilialNome = "";
                                cobranca.OperadoraID = contrato.OperadoraID;
                                cobranca.OperadoraNome = "";
                                cobranca.Pago = false;
                                cobranca.Parcela = parcela;
                                cobranca.PropostaID = contrato.ID;
                                cobranca.Tipo = (Int32)Cobranca.eTipo.Normal;
                                cobranca.Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite, false);

                                if (cobranca.Valor > 0)
                                {
                                    pm.Save(cobranca);
                                    retornar.Add(cobranca);
                                    CobrancaComposite.Salvar(cobranca.ID, composite, pm);
                                }
                                #endregion
                            }
                        }
                    }
                }

                return retornar;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                pm.CloseSingleCommandInstance();
                pm.Dispose();
                pm = null;
            }
        }

        static int MonthDifference(DateTime lValue, DateTime rValue)
        {
            return Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
        }

        public static IList<Cobranca> ArrumaFurosNaCadeia(ref String err)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            String qry = String.Concat(
                "select contrato_id,contrato_vencimento from contrato ",
                "   where ",
                //"       contrato_id > 202402 and ", // contrato_id <> 177176 and
                "       (contrato_inativo is null or contrato_inativo=0) and ",
                "       (contrato_cancelado is null or contrato_cancelado=0) and year(contrato_data) >= 2012 order by contrato_id ");

            List<Contrato> contratos = new List<Contrato>();
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                Contrato contrato = new Contrato(row[0]);
                contrato.Vencimento = Convert.ToDateTime(row[1], cinfo);
                contratos.Add(contrato);
            }

            //IList<Contrato> contratos = LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato), pm);

            if (contratos == null) { pm.Dispose(); return null; }

            //int min = 1, max = 0, curr = 0;

            //Object ret = null;
            Cobranca cobranca = null;
            IList<Cobranca> cobrancas = new List<Cobranca>();
            DateTime vencimento, proxVencimento;
            List<CobrancaComposite> composite = null;
            int diff = 0, index = 0;

            qry = "select max(cobranca_parcela) from cobranca where cobranca_propostaId=";
            qry = "select cobranca_id,cobranca_datavencimento,cobranca_parcela from cobranca where cobranca_tipo=0 and cobranca_propostaId=";

            foreach (Contrato contrato in contratos)
            {
                vencimento = DateTime.MinValue;
                proxVencimento = DateTime.MinValue;

                index++;

                cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>(String.Concat(qry, contrato.ID, " order by cobranca_dataVencimento"), typeof(Cobranca), pm);

                if (cobrancas == null) { continue; }

                foreach(Cobranca _cobranca in cobrancas)
                {
                    //a primeira parcela sempre existe, pois é criada com o cadastro do contrato
                    if (_cobranca.Parcela == 1) { continue; }

                    if (_cobranca.Parcela == 2)
                    {
                        proxVencimento = _cobranca.DataVencimento.AddMonths(1);
                        continue;
                    }

                    //reseta as datas para comparação
                    proxVencimento = new DateTime(proxVencimento.Year, proxVencimento.Month, proxVencimento.Day, 0, 0, 0, 0);
                    _cobranca.DataVencimento = new DateTime(_cobranca.DataVencimento.Year,
                        _cobranca.DataVencimento.Month, _cobranca.DataVencimento.Day, 0, 0, 0, 0);

                    if (_cobranca.DataVencimento > proxVencimento && 
                        _cobranca.DataVencimento.Month != proxVencimento.Month)
                    {
                        //TEM FURO NA CADEIA, arrumar
                        //tira a diferenca para saber qtas cobrancas devem ser geradas
                        diff = MonthDifference(proxVencimento, _cobranca.DataVencimento);
                        vencimento = proxVencimento;

                        if (diff < 1) { break; }

                        if (diff > 1) { int j = 0; }

                        for (int i = 1; i <= diff; i++)
                        {
                            cobranca = new Cobranca();
                            cobranca.BeneficiarioEmail = "";
                            cobranca.BeneficiarioId = null;
                            cobranca.ContratoEnderecoCobrancaID = contrato.EnderecoCobrancaID;
                            cobranca.ContratoNumero = contrato.Numero;
                            cobranca.ContratoTitularNome = "";
                            cobranca.DataCriacao = DateTime.Now;

                            cobranca.EstipulanteNome = "";
                            cobranca.FilialNome = "";
                            cobranca.OperadoraID = contrato.OperadoraID;
                            cobranca.OperadoraNome = "";
                            cobranca.Pago = false;
                            cobranca.Parcela = 0;
                            cobranca.PropostaID = contrato.ID;
                            cobranca.Tipo = (Int32)Cobranca.eTipo.Normal;

                            ////calcula a data de vencimento
                            //if (index == 2)
                            //{
                            //    //se é a segunda parcela, usa o vencto. cadastrado na proposta
                            //    cobranca.DataVencimento = new DateTime(contrato.Vencimento.Year,
                            //        contrato.Vencimento.Month, contrato.Vencimento.Day, 23, 59, 59, 500);
                            //}
                            //else
                            //{
                                //atribui o vencimento
                                cobranca.DataVencimento = new DateTime(proxVencimento.Year,
                                    proxVencimento.Month, proxVencimento.Day, 23, 59, 59, 500);
                            //}

                            cobranca.Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite, true); //força vigencia

                            if (cobranca.Valor == 0) { err += Convert.ToString(contrato.ID) + ","; break; } //throw new ApplicationException("Erro valorando cobrança"); }

                            pm.Save(cobranca);////////////////////////////////////

                            //incrementa o vencimento
                            if(i == diff)
                                proxVencimento = _cobranca.DataVencimento.AddMonths(1);// proxVencimento.AddMonths(1);
                            else
                                proxVencimento = proxVencimento.AddMonths(1);

                            //index++;
                        }
                    }
                    else
                    {
                        proxVencimento = _cobranca.DataVencimento.AddMonths(1);
                        if (proxVencimento.Day != contrato.Vencimento.Day)
                        {
                            proxVencimento = new DateTime(proxVencimento.Year,
                                    proxVencimento.Month, contrato.Vencimento.Day, 23, 59, 59, 500);
                        }
                    }
                }
            }


            pm.CloseSingleCommandInstance();
            pm.Dispose();

            return null;
        }

        //As cobrancas nao sao mais geradas em lote
        //public static void Gerar(Object propostaId, DateTime vencimento, Int32 qtd, PersistenceManager pm)
        //{
        //    DateTime _vencto = new DateTime(vencimento.Year, vencimento.Month, vencimento.Day, 23, 59, 59);
        //    List<Cobranca> cobrancas = new List<Cobranca>();
        //    Boolean firstInteraction = true;

        //    Int32 parcela = 0;
        //    IList<Cobranca> cobrancasExistentes = Cobranca.CarregarTodas(propostaId, pm);
        //    if (cobrancasExistentes != null && cobrancasExistentes.Count > 0)
        //    {
        //        parcela = cobrancasExistentes[cobrancasExistentes.Count - 1].Parcela + 1;
        //        cobrancasExistentes = null;
        //    }

        //    for (Int32 i = 1; i <= qtd; i++)
        //    {
        //        Cobranca cobranca = new Cobranca();
        //        cobranca.PropostaID = propostaId;

        //        if (firstInteraction)
        //        {
        //            cobranca.DataVencimento = _vencto;
        //            firstInteraction = false;
        //        }
        //        else
        //            cobranca.DataVencimento = _vencto.AddMonths(i - 1);

        //        cobranca.Valor = Contrato.CalculaValorDaProposta(propostaId, cobranca.DataVencimento, pm);// valorPlano + valorAdicionais + valorEstipulante;

        //        if (parcela == 0)
        //            cobranca.Parcela = i;
        //        else
        //        {
        //            cobranca.Parcela = parcela;
        //            parcela++;
        //        }

        //        cobrancas.Add(cobranca);
        //    }

        //    if (pm != null)
        //        Gerar(cobrancas, pm);
        //    else
        //    {
        //        PersistenceManager _pm = new PersistenceManager();
        //        _pm.BeginTransactionContext();
        //        try
        //        {
        //            Gerar(cobrancas, _pm);
        //            _pm.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            _pm.Rollback();
        //            throw ex;
        //        }
        //    }
        //}
        //static void Gerar(List<Cobranca> cobrancas, PersistenceManager pm)
        //{
        //    foreach (Cobranca cobranca in cobrancas)
        //    {
        //        pm.Save(cobranca);
        //    }
        //}

        public static IList<Cobranca> CarregarBoletos(Object operadoraId, String numeroContrato)
        {
            String qry = String.Concat("SELECT cobranca_id, cobranca_valor, cobranca_dataVencimento, beneficiario_nome, beneficiario_id, beneficiario_email ",
                "   FROM contrato ",
                "       INNER JOIN endereco ON contrato_enderecoCobrancaId = endereco_id ",
                "       INNER JOIN cobranca ON cobranca_propostaId = contrato_id ",
                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_contratoId = contrato_id ",
                "       INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId = beneficiario_id ",
                "   WHERE ",
                "       contrato_numero = @NumeroContrato AND ",
                "       contrato_operadoraId = ", operadoraId, " AND ",
                "       contratobeneficiario_tipo = 0 AND ",
                "       cobranca_pago = 0 ",
                "   ORDER BY ",
                "       cobranca_dataVencimento ASC");

            String[] pName = new String[] { "@NumeroContrato" };
            String[] pValue = new String[] { numeroContrato };

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Cobranca>(qry, pName, pValue, typeof(Cobranca));
        }

        /// <summary>
        /// Loga a alteração da data de vencimento de uma cobrana, para emissão sem juro e multa.
        /// </summary>
        public static void LogaNovaDataDeVencimentoParaEmissao(Object cobrancaId, DateTime venctoOriginal, DateTime venctoNovo, Object usuarioId, PersistenceManager pm)
        {
            try
            {
                CobrancaVencimentoLog log = new CobrancaVencimentoLog();
                log.CobrancaID = cobrancaId;
                log.Data = DateTime.Now;
                log.UsuarioID = usuarioId;
                log.VenctoNovo = venctoNovo;
                log.VenctoOriginal = venctoOriginal;

                if (pm != null)
                    pm.Save(log);
                else
                {
                    PersistenceManager _pm = new PersistenceManager(log);
                    _pm.Save();
                    _pm.Dispose();
                }
            }
            catch { }
        }

        //public static void LogaCobrancaEnviada(Object boletoId
    }

    [Serializable]
    [DBTable("arquivoCobrancaUnibanco")]
    public class ArquivoCobrancaUnibanco : EntityBase, IPersisteableEntity 
    {
        #region fields 

        Object _id;
        Object _operadoraId;
        Object _agendamentoId;
        String _nome;
        Int32 _versao;
        DateTime _dataCriacao;
        Int32 _mesReferencia;
        Int32 _anoReferencia;
        Int32 _qtdCobrancas;

        Int32  _qtdCobrancasEnviadas;
        String _operadoraNome;

        String _arquivoNome;
        String _descricaoCriterio;

        #endregion

        #region properties 

        [DBFieldInfo("arquivocobranca_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("arquivocobranca_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("arquivocobranca_agendamentoId", FieldType.Single)]
        public Object AgendamentoID
        {
            get { return _agendamentoId; }
            set { _agendamentoId= value; }
        }

        [DBFieldInfo("arquivocobranca_nome", FieldType.Single)]
        public String Nome
        {
            get { return _nome; }
            set { _nome= value; }
        }

        [DBFieldInfo("arquivocobranca_versao", FieldType.Single)]
        public Int32 Versao //1 até 99
        {
            get { return _versao; }
            set { _versao= value; }
        }

        [DBFieldInfo("arquivocobranca_data", FieldType.Single)]
        public DateTime DataCriacao
        {
            get { return _dataCriacao; }
            set { _dataCriacao= value; }
        }

        [DBFieldInfo("arquivocobranca_mesReferencia", FieldType.Single)]
        public Int32 MesReferencia
        {
            get { return _mesReferencia; }
            set { _mesReferencia= value; }
        }

        [DBFieldInfo("arquivocobranca_anoReferencia", FieldType.Single)]
        public Int32 AnoReferencia
        {
            get { return _anoReferencia; }
            set { _anoReferencia= value; }
        }

        /// <summary>
        /// Número detectado de cobranças que deveriam estar no arquivo.
        /// </summary>
        [DBFieldInfo("arquivocobranca_qtdCobrancas", FieldType.Single)]
        public Int32 QtdCobrancas
        {
            get { return _qtdCobrancas; }
            set { _qtdCobrancas= value; }
        }

        /// <summary>
        /// Número atual de cobranças no arquivo. Este número pode flutuar.
        /// </summary>
        [Joinned("cobrancas_qtd")]
        public Int32 QtdCobrancasEnviadas
        {
            get { return _qtdCobrancasEnviadas; }
            set { _qtdCobrancasEnviadas= value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraNome
        {
            get { return _operadoraNome; }
            set { _operadoraNome= value; }
        }

        [Joinned("arcrit_arquivoNome")]
        public String ArquivoNome
        {
            get { return _arquivoNome; }
            set { _arquivoNome= value; }
        }

        [Joinned("arcrit_descricao")]
        public String DescricaoCriterio
        {
            get { return _descricaoCriterio; }
            set { _descricaoCriterio= value; }
        }

        #endregion

        public ArquivoCobrancaUnibanco() { _qtdCobrancas = -1; }

        #region EntityBase methods 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public void SalvaItens(IList<Object> itemIDs)
        {
            ArquivoCobrancaUnibanco.SalvaItens(this._id, itemIDs, null);
        }
        public void SalvaItens(IList<Object> itemIDs, PersistenceManager pm)
        {
            ArquivoCobrancaUnibanco.SalvaItens(this._id, itemIDs, pm);
        }

        public static IList<ArquivoCobrancaUnibanco> Carregar(DateTime venctoDe, DateTime venctoAte)
        {
            String qry = String.Concat("arquivoCobrancaUnibanco.*, arquivocobranca_operadoraId,arquivocobranca_agendamentoId,arcrit_arquivoNome, arcrit_descricao, COUNT(arqitem_arquivoId) AS cobrancas_qtd ",
                "   FROM arquivoCobrancaUnibanco ",
                "       INNER JOIN arquivoRemessaAgendamento ON arcage_id=arquivocobranca_agendamentoId ",
                "       INNER JOIN arquivoRemessaCriterio ON arcage_criterioId=arcrit_id ",
                "       LEFT JOIN arquivoCobrancaUnibanco_cobanca ON arquivocobranca_id=arqitem_arquivoId ",
                "   WHERE ",
                "       arcage_vencimentoDe >=  '", venctoDe.ToString("yyyy-MM-dd 00:00:00"), "' AND ",
                "       arcage_vencimentoAte <= '", venctoAte.ToString("yyyy-MM-dd 23:59:59"), "'",
                "   GROUP BY arcage_vencimentoDe,arquivocobranca_id,arquivocobranca_operadoraId,arquivocobranca_agendamentoId,arquivocobranca_nome,arquivocobranca_versao,arquivocobranca_data,arquivocobranca_mesReferencia,arquivocobranca_anoReferencia, arquivocobranca_qtdCobrancas, arcrit_arquivoNome, arcrit_descricao ",
                "   ORDER BY arcage_vencimentoDe DESC, arcrit_arquivoNome, arquivocobranca_versao");

            return LocatorHelper.Instance.ExecuteQuery<ArquivoCobrancaUnibanco>(qry, typeof(ArquivoCobrancaUnibanco));
        }

        public static IList<ArquivoCobrancaUnibanco> Carregar(Object operadoraId, Int32 mesRef, Int32 anoRef)
        {
            String operadoraCond = "";
            if (operadoraId != null)
            {
                operadoraCond = " AND arquivocobranca_operadoraId=" + operadoraId.ToString();
            }

            String qry = String.Concat("arquivoCobrancaUnibanco.*, operadora_nome, COUNT(arqitem_arquivoId) AS cobrancas_qtd ",
                "   FROM arquivoCobrancaUnibanco ",
                "       INNER JOIN operadora ON operadora_id=arquivocobranca_operadoraId",
                "       LEFT JOIN arquivoCobrancaUnibanco_cobanca ON arquivocobranca_id=arqitem_arquivoId ",
                "   WHERE ",
                "       arquivocobranca_mesReferencia=", mesRef, 
                "       AND arquivocobranca_anoReferencia=", anoRef, operadoraCond,
                "   GROUP BY arquivocobranca_id,arquivocobranca_operadoraId,arquivocobranca_nome,arquivocobranca_versao,arquivocobranca_data,arquivocobranca_mesReferencia,arquivocobranca_anoReferencia, arquivocobranca_qtdCobrancas, operadora_nome",
                "   ORDER BY operadora_nome, arquivocobranca_versao");

            return LocatorHelper.Instance.ExecuteQuery<ArquivoCobrancaUnibanco>(qry, typeof(ArquivoCobrancaUnibanco));
        }

        public static void SalvaItens(Object arquivoId, IList<Object> itemIDs)
        {
            SalvaItens(arquivoId, itemIDs, null);
        }
        public static void SalvaItens(Object arquivoId, IList<Object> itemIDs, PersistenceManager pm)
        {
            StringBuilder command = new StringBuilder();
            foreach (Object itemID in itemIDs)
            {
                if (command.Length > 0) { command.Append("; "); }
                command.Append("INSERT INTO arquivoCobrancaUnibanco_cobanca (arqitem_arquivoId, arqitem_cobrancaId) VALUES (");
                command.Append(arquivoId);
                command.Append(","); command.Append(itemID); command.Append(")");
            }

            NonQueryHelper.Instance.ExecuteNonQuery(command.ToString(), pm);
        }

        public static Int32 ObtemProximaVersao()
        {
            return ObtemProximaVersao(null);
        }
        public static Int32 ObtemProximaVersao(PersistenceManager pm)
        {
            String qry = "SELECT MAX(arquivocobranca_versao) FROM arquivoCobrancaUnibanco";

            Object returned = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
            if (returned == DBNull.Value || Convert.ToInt32(returned) == 99)
                return 1;
            else
                return Convert.ToInt32(returned) + 1;
        }

        public static List<SumarioArquivoGeradoVO> GeraDocumentoCobrancaDUPLA_UNIBANCO(String[] operadoraIDs, Int32 mes, Int32 ano)
        {
            List<SumarioArquivoGeradoVO> vos = new List<SumarioArquivoGeradoVO>();
            String opeIN = String.Join(",", operadoraIDs);

            DateTime referencia = new DateTime(ano, mes, 2);
            DateTime referenciaPassda = referencia.AddMonths(-1);

            String qry = String.Concat("SELECT cobranca.*, operadora_id, operadora_nome ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON contrato_operadoraId=operadora_id ",
                "   WHERE ",
                "       contrato_inativo=0 AND contrato_cancelado=0 AND contrato_rascunho=0 AND (cobranca_tipo=0 OR cobranca_tipo=1 OR (cobranca_tipo=2 AND cobranca_arquivoUltimoEnvioId IS NULL)) AND ", 
                "       ((YEAR(cobranca_dataVencimento)=", ano, " AND ",
                "       MONTH(cobranca_dataVencimento)=", mes, ") OR ",
                "       (MONTH(cobranca_dataVencimento)=",referenciaPassda.Month ," AND ",
                "       YEAR(cobranca_dataVencimento)=", referenciaPassda.Year, " )) AND ",
                "       contrato_operadoraId IN (", opeIN, ") ",
                "   ORDER BY operadora_id ASC, cobranca_propostaId ASC, cobranca_dataVencimento DESC");

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                IList<Cobranca> cobrancas = 
                    LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);

                if (cobrancas == null) { pm.Commit(); pm = null; return null; }

                Decimal jurosDia    = Convert.ToDecimal(ConfigurationManager.AppSettings["jurosDia"]);
                Decimal jurosAtraso = Convert.ToDecimal(ConfigurationManager.AppSettings["jurosAtraso"]);

                foreach (String operadoraId in operadoraIDs)
                {
                    String arquivoNome = "", arquivoConteudo = "", arquivoVersao = "";
                    Object arquivoId = null;
                    Int32 qtdCobrancas = 0;
                    List<String> cobrancaIDs = new List<String>();

                    //Separa as cobrancas da operadora corrente
                    List<Cobranca> cobrancasDaOperadora = new List<Cobranca>();
                    foreach (Cobranca cobranca in cobrancas)
                    {
                        if (Convert.ToString(cobranca.OperadoraID) != operadoraId) { continue; }
                        cobrancasDaOperadora.Add(cobranca);
                    }

                    List<Cobranca> cobrancasDuplas = new List<Cobranca>();
                    foreach (Cobranca cobranca in cobrancasDaOperadora)
                    {
                        //Calcula as cobrancas DUPLAS
                        if (((Cobranca.eTipo)cobranca.Tipo) == Cobranca.eTipo.Dupla && cobranca.ArquivoIDUltimoEnvio == null)
                        {
                            cobrancasDuplas.Add(cobranca);
                            cobrancaIDs.Add(Convert.ToString(cobranca.ID));
                        }
                        else
                        {
                            //checa se tem uma cobranca de parcela anterior à cobranca corrente,
                            //vencida e em aberto. Se tem, gera acobranca dupla.
                            Cobranca cobrancaAnterior = existeParcelaAnterior(cobranca, cobrancasDaOperadora);
                            if (cobrancaAnterior != null && cobrancaAnterior.Pago == false)
                            {
                                //Há uma cobranca anterior a esta em aberto. Cria-se a cobranca dupla
                                cobranca.Tipo = (Int32)Cobranca.eTipo.Dupla;
                                cobranca.ValorNominal = cobranca.Valor;
                                cobranca.Valor += cobrancaAnterior.Valor;
                                cobranca.CobrancaRefID = cobrancaAnterior.ID;

                                //calcula o juros da cobrancaAnterior até o vencimento da 
                                //cobranca autal, pois está atrasada.
                                TimeSpan atraso = cobranca.DataVencimento.Subtract(cobrancaAnterior.DataVencimento);
                                cobranca.Valor += cobrancaAnterior.Valor * jurosAtraso;
                                if (atraso.Days > 1)
                                {
                                    cobranca.Valor += cobrancaAnterior.Valor * (jurosDia * (atraso.Days - 1));
                                }

                                pm.Save(cobranca);
                                cobrancasDuplas.Add(cobranca);
                                cobrancaIDs.Add(Convert.ToString(cobranca.ID));
                            }
                        }
                    }

                    if (cobrancasDuplas.Count == 0) { return null; }

                    /////////////////////////////////////////
                    qtdCobrancas = cobrancasDuplas.Count;
                    GeraDocumentoCobranca_UNIBANCO(cobrancaIDs, ref arquivoNome, ref arquivoConteudo, ref arquivoId, ref arquivoVersao, null, pm);

                    //armazena na colecao para retorno à UI
                    SumarioArquivoGeradoVO vo = new SumarioArquivoGeradoVO();
                    vo.ArquivoConteudo = arquivoConteudo;
                    vo.ArquivoID = arquivoId;
                    vo.ArquivoNome = arquivoNome;
                    vo.ArquivoVersao = arquivoVersao;
                    vo.OperadoraID = operadoraId;
                    vo.OperadoraNome = cobrancasDuplas[0].OperadoraNome;
                    vo.QtdCobrancas = qtdCobrancas;
                    vos.Add(vo);
                }

                pm.Commit();
            }
            catch(Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }

            return vos;
        }

        /// <summary>
        /// Retorna a cobrança anterior a uma cobranca em foco.
        /// </summary>
        /// <param name="referencia">Cobrança em foco.</param>
        /// <param name="lista">Lista de cobranças em que ocorrerá a procura.</param>
        /// <returns>Cobrança anterior, ou Null.</returns>
        static Cobranca existeParcelaAnterior(Cobranca referencia, IList<Cobranca> lista)
        {
            if (referencia.Parcela == 1 || lista == null || lista.Count == 0)
                return null;

            foreach (Cobranca cobranca in lista)
            {
                if (Convert.ToString(cobranca.ID) == Convert.ToString(referencia.ID)) { continue; }
                if (Convert.ToString(cobranca.PropostaID) == Convert.ToString(referencia.PropostaID) &&
                    cobranca.Parcela == (referencia.Parcela - 1) &&
                    Convert.ToString(cobranca.OperadoraID) == Convert.ToString(referencia.OperadoraID))
                {
                    return cobranca;
                }
            }

            return null;
        }


        /// <summary>
        /// SEGUNDA VERSÃO DO MÉTODO - temporario para gerar cobrancas de clientes inativos
        /// </summary>
        public static String __GeraDocumentoCobranca_UNIBANCO2B(IList<String> cobrancaIDs, ref String arquivoNome, ref String arquivoConteudo, PersistenceManager _pm)
        {
            PersistenceManager pm = _pm;

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.BeginTransactionContext();
            }

            try
            {
                if (cobrancaIDs == null) { return null; }

                String dataAgora = DateTime.Now.ToString("ddMMyy");
                String versao = "";
                StringBuilder doc = new StringBuilder();

                Cobranca.eCarteira carteira = Cobranca.eCarteira.Unibanco;

                #region HEADER 

                EntityBase.AppendPreparedField(ref doc, "0", 1);
                EntityBase.AppendPreparedField(ref doc, "1", 1);
                EntityBase.AppendPreparedField(ref doc, "REMESSA", 7);
                EntityBase.AppendPreparedField(ref doc, "03", 2);
                EntityBase.AppendPreparedField(ref doc, "COBR.  ESPECIAL", 15); //005

                //////////////////////////////////////////////////////////////////////////////////////
                if (carteira == Cobranca.eCarteira.Unibanco)
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C006.PadLeft(11, ' '), 11);
                else
                    EntityBase.AppendPreparedField(ref doc, "9108080533".PadLeft(11, ' '), 11);
                //////////////////////////////////////////////////////////////////////////////////////

                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C007, 4);//Se utilizado, o 011 é preenchido com zeros. Se 011 é ultilizado, 007 é preenchido com zeros
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C008, 1);  //008
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C009, 1);  //009 
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C010, 1);  //010
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C011, 7);  //011
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C012, 2);  //012 ????
                EntityBase.AppendPreparedField(ref doc, " ", 41);       //013
                EntityBase.AppendPreparedField(ref doc, dataAgora, 6);  //014
                EntityBase.AppendPreparedField(ref doc, "01600", 5);    //015
                EntityBase.AppendPreparedField(ref doc, "BPI", 3);      //016
                EntityBase.AppendPreparedField(ref doc, " ", 116);      //017
                EntityBase.AppendPreparedField(ref doc, "0".PadRight(167, '0'), 167); //018

                String arquivoVersao = versao.PadRight(3, '0');
                EntityBase.AppendPreparedField(ref doc, versao, 3);     //019
                EntityBase.AppendPreparedField(ref doc, " ".PadLeft(200, ' '), 200); //20
                EntityBase.AppendPreparedField(ref doc, "000001", 6);   //021

                #endregion

                //Carrega cobrancas envolvidas
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela(cobrancaIDs, pm);
                List<Object> cobrancaIds = new List<Object>();
                if (cobrancas == null) { return null; }

                //arquivoId = null;

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL

                Endereco endereco = null;
                Int32 numSequencial = 1, diaDataSemJuros, result = 0;
                String mascara6 = new String('0', 6), nossoNumero = "";
                Decimal valorTotal = 0, mora = 0, multa = 0;
                Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;
                IList<ArquivoRemessaCriterio> arcrits = ArquivoRemessaCriterio.CarregarTodos();
                ArquivoRemessaCriterio arqCriterio = null;String[] arr=null;

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                foreach (Cobranca cob in cobrancas)
                {
                    contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                    #region localiza o critério correspondente

                    foreach (ArquivoRemessaCriterio obj in arcrits)
                    {
                        if (Convert.ToString(obj.OperadoraID) == Convert.ToString(contrato.OperadoraID))
                        {
                            arr = obj.ContratoAdmIDs.Split(',');
                            foreach (String itemarr in arr)
                            {
                                if (itemarr.Trim() == Convert.ToString(contrato.ContratoADMID).Trim())
                                {
                                    arqCriterio = obj;
                                    break;
                                }
                            }
                        }
                    }
                    #endregion

                    numSequencial++;
                    cobrancaIds.Add(cob.ID);

                    cob.DataVencimento = new DateTime(2011, 10, 31, 23, 59, 59, 995);

                    if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                    {
                        numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                        qtdTotalBoletos = 0;
                        foreach (Cobranca _temp in cobrancas)
                        {
                            if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                            else if (qtdTotalBoletos > 0) { break; }
                        }
                    }

                    numBoletoCorrente++;

                    #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                    retTvv = TabelaValorVencimentoVO.ExisteProposta(cob.PropostaID, lista);
                    if (retTvv == null)
                    {
                        //contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cob.DataVencimento, pm);
                        if (tabela != null && tabela.Count > 0)
                        {
                            retTvv = TabelaValorVencimentoVO.ExisteTabela(tabela[0].ID, lista);
                            if (retTvv == null)
                            {
                                taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                                if (taxa != null && !taxa.Embutido)
                                {
                                    cob.Valor += taxa.ValorEmbutido;
                                }

                                retTvv = new TabelaValorVencimentoVO();
                                retTvv.PropostaID = contrato.ID;
                                retTvv.TabelaID = tabela[0].ID;
                                if (tabela[0].VencimentoFim != DateTime.MinValue)
                                { retTvv.Vencimento = tabela[0].VencimentoFim.AddMonths(1); }
                                if (taxa != null && !taxa.Embutido)
                                { retTvv.ValorBancario = taxa.ValorEmbutido; }

                                if (contrato.CobrarTaxaAssociativa)
                                {
                                    estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                                    if (estipulanteTaxa != null)
                                    { retTvv.ValorSindicalizacao = estipulanteTaxa.Valor; }
                                }

                                lista.Add(retTvv);
                                //taxa = null;
                            }
                            else
                                cob.Valor += retTvv.ValorBancario;
                        }
                    }
                    else
                        cob.Valor += retTvv.ValorBancario;

                    #endregion

                    #region data de admissao do contrato

                    if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                    else { continue; /*TODO: Logar */ }
                    #endregion

                    valorTotal += cob.Valor;
                    doc.Append(Environment.NewLine);

                    EntityBase.AppendPreparedField(ref doc, "2", 1); //021

                    cob.Tipo = 4;
                    nossoNumero = cob.GeraNossoNumero();
                    EntityBase.AppendPreparedField(ref doc, nossoNumero, 14); //022
                    EntityBase.AppendPreparedField(ref doc, cob._CalculaDVMod11(cob.Tipo, cob.ContratoCodCobranca, cob.Parcela), 1); //023 - Depende do 009 ou do 302 (usamos o 009)
                    EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyy"), 6); //024
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C025, 4);  //025
                    cob.Tipo = 0;

                    //////////////////////////////////////////////////////////////////////////////////////
                    if (carteira == Cobranca.eCarteira.Unibanco)
                        EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C026.PadLeft(11, '0'), 11); //026
                    else
                        EntityBase.AppendPreparedField(ref doc, "9108080533".PadLeft(11, ' '), 11);
                    //////////////////////////////////////////////////////////////////////////////////////

                    EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 30); //027

                    endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                    endereco.Carregar(pm);
                    if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                    EntityBase.AppendPreparedField(ref doc, "", 30);              //028
                    EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 20); //029
                    EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 20); //030
                    EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);      //031
                    EntityBase.AppendPreparedField(ref doc, endereco.CEP.Substring(0, 5), 5); //032
                    EntityBase.AppendPreparedField(ref doc, endereco.CEP.Substring(5, 3), 3); //033 
                    EntityBase.AppendPreparedField(ref doc, dataAgora, 6);        //034
                    EntityBase.AppendPreparedField(ref doc, "", 2);               //035
                    EntityBase.AppendPreparedField(ref doc, "0000000000", 10);    //036 -- quantidade de moeda - para moeda diferente de REAL
                    EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2").Replace(".", "").Replace(",", "").PadLeft(15, '0'), 15); //037 -- ???? valor do titulo - como preencher
                    EntityBase.AppendPreparedField(ref doc, "000000000000000", 15);//038 -- desconto 

                    mora = Convert.ToDecimal(CobrancaConfig.C039) * cob.Valor;
                    EntityBase.AppendPreparedField(ref doc, mora.ToString("N2").Replace(",", "").Replace(".", "").PadLeft(12, '0'), 12);   //039 -- juros de mora 

                    multa = Convert.ToDecimal(CobrancaConfig.MultaPercentual) * cob.Valor;
                    EntityBase.AppendPreparedField(ref doc, multa.ToString("N2").Replace(",", "").Replace(".", "").PadLeft(12, '0'), 12);   //040 -- multa atraso 
                    EntityBase.AppendPreparedField(ref doc, cob.Parcela.ToString().PadLeft(3, '0'), 3);             //041 -- num da parcela. deve ser 000 ????
                    EntityBase.AppendPreparedField(ref doc, " ", 42);              //042
                    EntityBase.AppendPreparedField(ref doc, cob.ContratoNumero.PadRight(18, ' '), 18); //043 - numero do documento
                    //EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C044a, 1);
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C044, 6);//044 -- especie documento
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C045, 2);//045 -- ???? aceite
                    EntityBase.AppendPreparedField(ref doc, dataAgora, 6);          //046 -- data processamento


                    //////////////////////////////////////////////////////////////////////////////////////
                    if (carteira == Cobranca.eCarteira.Unibanco)
                        EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C047, 2);//047
                    else
                        EntityBase.AppendPreparedField(ref doc, "198", 3);//047
                    //////////////////////////////////////////////////////////////////////////////////////

                    EntityBase.AppendPreparedField(ref doc, "0", 1);                //048 -- ???? indicador do registro de msg
                    EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyy"), 6); //049 data limite desconto

                    EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.AddDays(1).ToString("ddMMyy"), 6);//050 -- data para multa

                    EntityBase.AppendPreparedField(ref doc, "0", 1);                   //051 -- prazo de mora diária
                    EntityBase.AppendPreparedField(ref doc, "0", 1);                   //052 -- codigo de moeda
                    EntityBase.AppendPreparedField(ref doc, "0".PadRight(18, ' '), 18);//053 -- uso do banco

                    //contr
                    if (endereco.Logradouro.Length > 48)
                    {
                        endereco.Logradouro = endereco.Logradouro.Substring(0, 48);
                    }

                    endereco.Logradouro += ", " + endereco.Numero;

                    if (!String.IsNullOrEmpty(endereco.Complemento))
                    {
                        endereco.Logradouro += " " + endereco.Complemento;
                    }

                    EntityBase.AppendPreparedField(ref doc, endereco.Logradouro.PadRight(60, ' '), 60); //054

                    EntityBase.AppendPreparedField(ref doc, " ", 3);                                    //055

                    EntityBase.AppendPreparedField(ref doc, arqCriterio.Projeto.PadLeft(4, '0'), 4);           //Projeto
                    EntityBase.AppendPreparedField(ref doc, qtdTotalBoletos.ToString().PadLeft(2, '0'), 2);    //QtdBoletos col.:399
                    EntityBase.AppendPreparedField(ref doc, numBoletoCorrente.ToString().PadLeft(2, '0'), 2);  //Boleto atual
                    EntityBase.AppendPreparedField(ref doc, arqCriterio.FoneAtendimento.PadRight(15, ' '), 15);//Telefone de atendimento

                    if (retTvv != null && retTvv.Vencimento != DateTime.MinValue)
                        EntityBase.AppendPreparedField(ref doc, retTvv.Vencimento.ToString("dd/MM/yyyy"), 10); //Mes reajuste
                    else
                        EntityBase.AppendPreparedField(ref doc, retTvv.Vencimento.ToString("0000000000"), 10); //Mes reajuste

                    EntityBase.AppendPreparedField(ref doc, arqCriterio.Operadora.PadRight(30, ' '), 30);      //Operadora

                    CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contrato.ContratoADMID, cob.DataVencimento,
                        out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                    if (diaDataSemJuros <= 0)
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(6, ' '), 6); //Data Sem juros TODO
                    else
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.AddDays(diaDataSemJuros).ToString("ddMMyy"), 6); //Data Sem juros TODO

                    #region Texto data limite
                    if (valorDataLimite != null)
                    {
                        if (Int32.TryParse(Convert.ToString(valorDataLimite), out result))
                        {
                            if (result < cob.DataVencimento.Day)
                            {
                                dataLimite = new DateTime(cob.DataVencimento.AddMonths(1).Year,
                                    cob.DataVencimento.AddMonths(1).Month, result);
                            }
                            else
                            {
                                dataLimite = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, result);
                            }

                            EntityBase.AppendPreparedField(ref doc, dataLimite.ToString("dd/MM/yyyy").PadRight(50, ' '), 50); //data limite calculada
                        }
                        else
                        {
                            EntityBase.AppendPreparedField(ref doc, Convert.ToString(valorDataLimite).PadRight(50, ' '), 50);
                        }
                    }
                    else
                        EntityBase.AppendPreparedField(ref doc, " ".PadRight(50, ' '), 50);  //Texto data limite

                    #endregion

                    EntityBase.AppendPreparedField(ref doc, arqCriterio.Ans.PadLeft(6, '0'), 6); //ANS

                    //Taxa associativa (tarifa bancaria + taxa de sindicalizacao)
                    if (retTvv != null)
                        EntityBase.AppendPreparedField(ref doc, (retTvv.ValorBancario + retTvv.ValorSindicalizacao).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(15, '0'), 15);
                    else
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(15, '0'), 15);

                    //////////////////////////////////////////////////////////////////////////////////////
                    if (carteira == Cobranca.eCarteira.Unibanco)
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(60, ' '), 60); //reservado
                    else
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(59, ' '), 59); //reservado
                    //////////////////////////////////////////////////////////////////////////////////////

                    EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6);//056

                    //cob.ArquivoIDUltimoEnvio = arquivo.ID;
                    //pm.Save(cob);
                }
                #endregion

                #region TRAILLER

                numSequencial++;
                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "9", 1);  //142
                EntityBase.AppendPreparedField(ref doc, "", 25);  //143
                //////////////////////////////////////////////////////////////////////////////////////
                if (carteira == Cobranca.eCarteira.Unibanco)
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C006, 11); //144
                else
                    EntityBase.AppendPreparedField(ref doc, "9108080533".PadLeft(11, ' '), 11);
                //////////////////////////////////////////////////////////////////////////////////////
                EntityBase.AppendPreparedField(ref doc, "", 334); //145
                EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6); //146
                EntityBase.AppendPreparedField(ref doc, valorTotal.ToString("N2").Replace(".", "").Replace(",", "").PadLeft(17, '0'), 17); //147
                EntityBase.AppendPreparedField(ref doc, " ".PadLeft(200, ' '), 200); //148
                EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6); //149

                #endregion

                if (_pm == null) { pm.Commit(); } //commit apenas se nao estiver participando de uma transacao externa
                arquivoConteudo = doc.ToString();
                return arquivoConteudo;
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }



        /// <summary>
        /// SEGUNDA VERSÃO DO MÉTODO
        /// </summary>
        public static List<SumarioArquivoGeradoVO> GeraDocumentoCobranca_UNIBANCO2(IList<Cobranca> cobrancas, IList<ArquivoRemessaAgendamento> aras)
        {
            List<SumarioArquivoGeradoVO> vos = new List<SumarioArquivoGeradoVO>();

            //separa as operadoras
            List<String> operadoraIDs = new List<String>();
            System.Collections.Hashtable ht = new System.Collections.Hashtable();

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            //foreach (Cobranca cobranca in cobrancas)
            //{
            //    if (!operadoraIDs.Contains(cobranca.OperadoraID.ToString()))
            //        operadoraIDs.Add(cobranca.OperadoraID.ToString());
            //}

            pm.CloseSingleCommandInstance();
            pm.Dispose();

            //processa os arquivos, um para cada operadora.
            //foreach (String operadoraId in operadoraIDs)
            //{
                String arquivoNome = "", arquivoConteudo = "", arquivoVersao = "", operadoraNome = "";
                Object arquivoId = null;
                Int32 qtdCobrancas = 0;

                Operadora operadora = new Operadora(cobrancas[0].OperadoraID); //Operadora operadora = new Operadora(operadoraId);
                operadora.Carregar();
                operadoraNome = operadora.Nome;

                List<String> cobrancaIDs = new List<String>();
                foreach (Cobranca cobranca in cobrancas)
                {
                    //if (cobranca.OperadoraID.ToString() != operadoraId) { continue; }
                    cobrancaIDs.Add(cobranca.ID.ToString());
                }

                qtdCobrancas = cobrancaIDs.Count;
                GeraDocumentoCobranca_UNIBANCO2(cobrancaIDs, ref arquivoNome, ref arquivoConteudo, ref arquivoId, ref arquivoVersao, aras, null);

                //armazena na colecao para retorno à UI
                SumarioArquivoGeradoVO vo = new SumarioArquivoGeradoVO();
                vo.ArquivoConteudo = arquivoConteudo;
                vo.ArquivoID = arquivoId;
                vo.ArquivoNome = arquivoNome;
                vo.ArquivoVersao = arquivoVersao;
                vo.OperadoraID = cobrancas[0].OperadoraID; //operadoraId;
                vo.OperadoraNome = operadora.Nome;
                vo.QtdCobrancas = qtdCobrancas;
                vos.Add(vo);
            //}

            return vos;
        }
        /// <summary>
        /// SEGUNDA VERSÃO DO MÉTODO
        /// </summary>
        internal static String GeraDocumentoCobranca_UNIBANCO2(IList<String> cobrancaIDs, ref String arquivoNome, ref String arquivoConteudo, ref Object arquivoId, ref String arquivoVersao, IList<ArquivoRemessaAgendamento> aras, PersistenceManager _pm)
        {
            if (cobrancaIDs == null) { return null; }

            PersistenceManager pm = _pm;

            Int32 contratoadmQualicorpID = Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]);

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();
            }

            Cobranca cobprova = new Cobranca(cobrancaIDs[0]);
            pm.Load(cobprova);
            Contrato contratoprova = new Contrato(cobprova.PropostaID);
            pm.Load(contratoprova);

            String agCCDV_itau = ""; 
            String headerCedente = "";

            ////////////////////////////////////////////////// Só haverá cedente qualicorp
            agCCDV_itau = "0646042606";    //qualicorp
            headerCedente = " QUALICORP ADM. E SERV. LTDA";
            //////////////////////////////////////////////////

            try
            {
                if (cobrancaIDs == null) { return null; }

                String dataAgora = DateTime.Now.ToString("ddMMyy");
                String versao = "";
                StringBuilder doc = new StringBuilder();

                Cobranca.eCarteira carteira = (Cobranca.eCarteira)aras[0].Carteira;
                arquivoNome = String.Concat(aras[0].ArquivoNomeInstance, "_", DateTime.Now.ToString("yyyyMMddHHmmss"), ".dat");

                #region HEADER 

                EntityBase.AppendPreparedField(ref doc, "0", 1);
                EntityBase.AppendPreparedField(ref doc, "1", 1);
                EntityBase.AppendPreparedField(ref doc, "REMESSA", 7);
                EntityBase.AppendPreparedField(ref doc, "03", 2);
                EntityBase.AppendPreparedField(ref doc, "COBR.  ESPECIAL", 15); //005

                //////////////////////////////////////////////////////////////////////////////////////
                if (carteira == Cobranca.eCarteira.Unibanco)
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C006.PadLeft(11, ' '), 11);
                else
                    EntityBase.AppendPreparedField(ref doc, agCCDV_itau.PadLeft(11, ' '), 11);
                //////////////////////////////////////////////////////////////////////////////////////

                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C007, 4);//Se utilizado, o 011 é preenchido com zeros. Se 011 é ultilizado, 007 é preenchido com zeros
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C008, 1);  //008
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C009, 1);  //009 
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C010, 1);  //010
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C011, 7);  //011
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C012, 2);  //012 ????
                EntityBase.AppendPreparedField(ref doc, " ", 41);                 //013
                EntityBase.AppendPreparedField(ref doc, dataAgora, 6);            //014
                EntityBase.AppendPreparedField(ref doc, "01600", 5);              //015
                EntityBase.AppendPreparedField(ref doc, "BPI", 3);                //016
                EntityBase.AppendPreparedField(ref doc, headerCedente, 116);      //017
                EntityBase.AppendPreparedField(ref doc, "0".PadRight(167, '0'), 167); //018

                ArquivoCobrancaUnibanco arquivo = new ArquivoCobrancaUnibanco();
                arquivo.DataCriacao = DateTime.Now;
                versao = ArquivoCobrancaUnibanco.ObtemProximaVersao(pm).ToString().PadLeft(3, '0');
                arquivo.Versao = Convert.ToInt32(versao);
                arquivo.AgendamentoID = null; // ara.ID; 

                ArquivoRemessaCriterio arqCriterio = null; //new ArquivoRemessaCriterio(ara.CriterioID);
                //pm.Load(arqCriterio);

                arquivoVersao = versao.PadRight(3, '0');
                EntityBase.AppendPreparedField(ref doc, versao, 3);     //019
                EntityBase.AppendPreparedField(ref doc, " ".PadLeft(200, ' '), 200); //20
                EntityBase.AppendPreparedField(ref doc, "000001", 6);   //021

                #endregion

                //Carrega cobrancas envolvidas
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela_Optimized(cobrancaIDs, pm); //
                List<Object> cobrancaIds = new List<Object>();
                if (cobrancas == null) { return null; }

                arquivo.MesReferencia = cobrancas[0].DataVencimento.Month;
                arquivo.AnoReferencia = cobrancas[0].DataVencimento.Year;
                arquivo.OperadoraID   = cobrancas[0].OperadoraID;
                arquivo.Nome          = arquivoNome;
                arquivo.QtdCobrancas  = cobrancaIDs.Count;
                pm.Save(arquivo);
                arquivoId = arquivo.ID;

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL

                Endereco endereco = null;
                Int32 numSequencial = 1, diaDataSemJuros, result = 0;
                String mascara6 = new String('0', 6), nossoNumero = "";
                Decimal valorTotal = 0, mora = 0, multa = 0;
                Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                int i = 0;

                String[] contratoAdmIds = null;
                System.Collections.Hashtable contratoADM_Criterio = new System.Collections.Hashtable();
                List<string> parametrosProcessados = new List<string>();
                Cobranca _cob = null;

                foreach (Cobranca cob in cobrancas)
                {
                    if (parametrosProcessados.Contains(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID))) continue;

                    parametrosProcessados.Add(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID));

                    if (cob.ID == null) { continue; }

                    _cob = null;
                    _cob = Cobranca.CarregarEnviadasPor(cob.PropostaID, cob.DataVencimento, (int)Cobranca.eTipo.Normal, pm);
                    if (_cob != null) { continue; }

                    i++;
                    try
                    {
                        #region processamento

                        #region localiza o critério correspondente

                        contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                        if (contratoADM_Criterio.Contains(Convert.ToString(contrato.ContratoADMID)))
                        {
                                arqCriterio = (ArquivoRemessaCriterio)contratoADM_Criterio[Convert.ToString(contrato.ContratoADMID)];
                        }
                        else
                        {
                            foreach (ArquivoRemessaAgendamento ara in aras)
                            {
                                if (cob.Criterio != null) { break; }

                                contratoAdmIds = ArquivoRemessaCriterio.CarregarContratoAdmIds(ara.CriterioID, pm); /////

                                foreach (String contratoAdmId in contratoAdmIds)
                                {
                                    if (Convert.ToString(contrato.ContratoADMID).Equals(contratoAdmId))
                                    {
                                        ArquivoRemessaCriterio criterio = new ArquivoRemessaCriterio(ara.CriterioID);
                                        pm.Load(criterio);
                                        arqCriterio = criterio;

                                        try
                                        {
                                            contratoADM_Criterio.Add(contratoAdmId, criterio);
                                        }
                                        catch //(Exception ex)
                                        {
                                            continue;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion

                        numSequencial++;
                        cobrancaIds.Add(cob.ID);

                        if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                        {
                            numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                            qtdTotalBoletos = 0;
                            foreach (Cobranca _temp in cobrancas)
                            {
                                if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                                else if (qtdTotalBoletos > 0) { break; }
                            }
                        }

                        numBoletoCorrente++;

                        #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                        retTvv = TabelaValorVencimentoVO.ExisteProposta(cob.PropostaID, lista);
                        if (retTvv == null)
                        {
                            //contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                            tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cob.DataVencimento, pm);
                            if (tabela != null && tabela.Count > 0)
                            {
                                retTvv = TabelaValorVencimentoVO.ExisteTabela(tabela[0].ID, lista);
                                if (retTvv == null)
                                {
                                    taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                                    if (taxa != null && !taxa.Embutido)
                                    {
                                        cob.Valor += taxa.ValorEmbutido;
                                    }

                                    retTvv = new TabelaValorVencimentoVO();
                                    retTvv.PropostaID = contrato.ID;
                                    retTvv.TabelaID = tabela[0].ID;
                                    if (tabela[0].VencimentoFim != DateTime.MinValue)
                                    { retTvv.Vencimento = tabela[0].VencimentoFim.AddMonths(1); }
                                    if (taxa != null && !taxa.Embutido)
                                    { retTvv.ValorBancario = taxa.ValorEmbutido; }

                                    if (contrato.CobrarTaxaAssociativa)
                                    {
                                        estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                                        if (estipulanteTaxa != null)
                                        { retTvv.ValorSindicalizacao = estipulanteTaxa.Valor; }
                                    }

                                    lista.Add(retTvv);
                                    //taxa = null;
                                }
                                else
                                    cob.Valor += retTvv.ValorBancario;
                            }
                        }
                        else
                            cob.Valor += retTvv.ValorBancario;

                        #endregion

                        #region data de admissao do contrato

                        //Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                        else { continue; }
                        #endregion

                        valorTotal += cob.Valor;
                        doc.Append(Environment.NewLine);

                        EntityBase.AppendPreparedField(ref doc, "2", 1); //021

                        nossoNumero = cob.GeraNossoNumero();
                        if (Cobranca.NossoNumeroITAU)
                            doc.Append(nossoNumero.PadLeft(14, '0'));
                        else
                            EntityBase.AppendPreparedField(ref doc, nossoNumero, 14); //022
                        EntityBase.AppendPreparedField(ref doc, cob.CalculaDVMod10(cob.Tipo, cob.ContratoCodCobranca, cob.Parcela), 1); //023 - Depende do 009 ou do 302 (usamos o 009)
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyy"), 6); //024
                        EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C025, 4);  //025

                        //////////////////////////////////////////////////////////////////////////////////////
                        if (!Cobranca.NossoNumeroITAU) //if (carteira == Cobranca.eCarteira.Unibanco)
                            EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C026.PadLeft(11, '0'), 11); //026
                        else
                            EntityBase.AppendPreparedField(ref doc, agCCDV_itau.PadLeft(11, '0'), 11);
                        //////////////////////////////////////////////////////////////////////////////////////

                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 30); //027

                        endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                        endereco.Carregar(pm);
                        if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                        EntityBase.AppendPreparedField(ref doc, "", 30);              //028
                        EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 20); //029
                        EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 20); //030
                        EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);      //031
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP.Substring(0, 5), 5); //032
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP.Substring(5, 3), 3); //033 
                        EntityBase.AppendPreparedField(ref doc, dataAgora, 6);        //034
                        EntityBase.AppendPreparedField(ref doc, "", 2);               //035
                        EntityBase.AppendPreparedField(ref doc, "0000000000", 10);    //036 -- quantidade de moeda - para moeda diferente de REAL
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2").Replace(".", "").Replace(",", "").PadLeft(15, '0'), 15); //037 -- ???? valor do titulo - como preencher
                        EntityBase.AppendPreparedField(ref doc, "000000000000000", 15);//038 -- desconto 

                        mora = Convert.ToDecimal(CobrancaConfig.C039) * cob.Valor;
                        EntityBase.AppendPreparedField(ref doc, mora.ToString("N2").Replace(",", "").Replace(".", "").PadLeft(12, '0'), 12);   //039 -- juros de mora 

                        multa = Convert.ToDecimal(CobrancaConfig.MultaPercentual) * cob.Valor;
                        EntityBase.AppendPreparedField(ref doc, multa.ToString("N2").Replace(",", "").Replace(".", "").PadLeft(12, '0'), 12);   //040 -- multa atraso 
                        EntityBase.AppendPreparedField(ref doc, cob.Parcela.ToString().PadLeft(3, '0'), 3);             //041 -- num da parcela. deve ser 000 ????
                        EntityBase.AppendPreparedField(ref doc, " ", 42);              //042
                        EntityBase.AppendPreparedField(ref doc, cob.ContratoNumero.PadRight(18, ' '), 18); //043 - numero do documento
                        //EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C044a, 1);
                        EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C044, 6);//044 -- especie documento
                        EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C045, 2);//045 -- aceite
                        EntityBase.AppendPreparedField(ref doc, dataAgora, 6);          //046 -- data processamento


                        //////////////////////////////////////////////////////////////////////////////////////
                        if (!Cobranca.NossoNumeroITAU) //if (carteira == Cobranca.eCarteira.Unibanco)
                            EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C047, 2);//047
                        else
                            EntityBase.AppendPreparedField(ref doc, "175", 3);//047
                        //////////////////////////////////////////////////////////////////////////////////////

                        EntityBase.AppendPreparedField(ref doc, "0", 1);                //048 -- ???? indicador do registro de msg
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyy"), 6); //049 data limite desconto

                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.AddDays(1).ToString("ddMMyy"), 6);//050 -- data para multa

                        EntityBase.AppendPreparedField(ref doc, "0", 1);                   //051 -- prazo de mora diária
                        EntityBase.AppendPreparedField(ref doc, "0", 1);                   //052 -- codigo de moeda
                        EntityBase.AppendPreparedField(ref doc, "0".PadRight(18, ' '), 18);//053 -- uso do banco

                        //contr
                        if (endereco.Logradouro.Length > 48)
                        {
                            endereco.Logradouro = endereco.Logradouro.Substring(0, 48);
                        }

                        endereco.Logradouro += ", " + endereco.Numero;

                        if (!String.IsNullOrEmpty(endereco.Complemento))
                        {
                            endereco.Logradouro += " " + endereco.Complemento;
                        }

                        EntityBase.AppendPreparedField(ref doc, endereco.Logradouro.PadRight(60, ' '), 60); //054

                        EntityBase.AppendPreparedField(ref doc, " ", 3);                                    //055

                        EntityBase.AppendPreparedField(ref doc, arqCriterio.Projeto.PadLeft(4, '0'), 4);           //Projeto
                        EntityBase.AppendPreparedField(ref doc, qtdTotalBoletos.ToString().PadLeft(2, '0'), 2);    //QtdBoletos col.:399
                        EntityBase.AppendPreparedField(ref doc, numBoletoCorrente.ToString().PadLeft(2, '0'), 2);  //Boleto atual
                        EntityBase.AppendPreparedField(ref doc, arqCriterio.FoneAtendimento.PadRight(15, ' '), 15);//Telefone de atendimento

                        if (retTvv != null && retTvv.Vencimento != DateTime.MinValue)
                            EntityBase.AppendPreparedField(ref doc, retTvv.Vencimento.ToString("dd/MM/yyyy"), 10); //Mes reajuste
                        else
                            EntityBase.AppendPreparedField(ref doc, retTvv.Vencimento.ToString("0000000000"), 10); //Mes reajuste

                        EntityBase.AppendPreparedField(ref doc, arqCriterio.Operadora.PadRight(30, ' '), 30);      //Operadora

                        CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contrato.ContratoADMID, cob.DataVencimento,
                            out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                        if (diaDataSemJuros <= 0)
                            EntityBase.AppendPreparedField(ref doc, " ".PadLeft(6, ' '), 6); //Data Sem juros TODO
                        else
                            EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.AddDays(diaDataSemJuros).ToString("ddMMyy"), 6); //Data Sem juros TODO

                        #region Texto data limite
                        if (valorDataLimite != null) // && Convert.ToString(valorDataLimite) != "0")
                        {
                            if (Int32.TryParse(Convert.ToString(valorDataLimite), out result))
                            {
                                if (result < cob.DataVencimento.Day)
                                {
                                    dataLimite = new DateTime(cob.DataVencimento.AddMonths(1).Year,
                                        cob.DataVencimento.AddMonths(1).Month, result);
                                }
                                else
                                {
                                    dataLimite = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, result);
                                }

                                EntityBase.AppendPreparedField(ref doc, dataLimite.ToString("dd/MM/yyyy").PadRight(50, ' '), 50); //data limite calculada
                            }
                            else
                            {
                                EntityBase.AppendPreparedField(ref doc, Convert.ToString(valorDataLimite).PadRight(50, ' '), 50);
                            }
                        }
                        else
                            EntityBase.AppendPreparedField(ref doc, " ".PadRight(50, ' '), 50);  //Texto data limite

                        #endregion

                        EntityBase.AppendPreparedField(ref doc, arqCriterio.Ans.PadLeft(6, '0'), 6); //ANS

                        //Taxa associativa (tarifa bancaria + taxa de sindicalizacao)
                        if (retTvv != null)
                            EntityBase.AppendPreparedField(ref doc, (retTvv.ValorBancario + retTvv.ValorSindicalizacao).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(15, '0'), 15);
                        else
                            EntityBase.AppendPreparedField(ref doc, " ".PadLeft(15, '0'), 15);

                        //////////////////////////////////////////////////////////////////////////////////////
                        if (!Cobranca.NossoNumeroITAU) //if (carteira == Cobranca.eCarteira.Unibanco)
                            EntityBase.AppendPreparedField(ref doc, " ".PadLeft(60, ' '), 60); //reservado
                        else
                            EntityBase.AppendPreparedField(ref doc, " ".PadLeft(59, ' '), 59); //reservado
                        //////////////////////////////////////////////////////////////////////////////////////

                        EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6);//056

                        cob.ArquivoIDUltimoEnvio = arquivo.ID;
                        cob.DataCriacao = DateTime.Now;
                        pm.Save(cob);

                        #endregion
                    }
                    catch(Exception ex)
                    {
                        try
                        {
                            string path = System.Configuration.ConfigurationSettings.AppSettings["financialFilePath"].Replace("/", @"\");
                            System.IO.File.WriteAllText(path + "err_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".txt",
                                ex.Message +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + ex.StackTrace +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + Convert.ToString(cob.ID) +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + i.ToString(),
                                System.Text.Encoding.ASCII);
                        }
                        catch { }
                        continue;
                    }
                }
                #endregion

                #region TRAILLER

                numSequencial++;
                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "9", 1);  //142
                EntityBase.AppendPreparedField(ref doc, "", 25);  //143
                //////////////////////////////////////////////////////////////////////////////////////
                if (!Cobranca.NossoNumeroITAU) //if (carteira == Cobranca.eCarteira.Unibanco)
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C006, 11); //144
                else
                    EntityBase.AppendPreparedField(ref doc, "9108080533".PadLeft(11, ' '), 11);
                //////////////////////////////////////////////////////////////////////////////////////
                EntityBase.AppendPreparedField(ref doc, "", 334); //145
                EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6); //146
                EntityBase.AppendPreparedField(ref doc, valorTotal.ToString("N2").Replace(".", "").Replace(",", "").PadLeft(17, '0'), 17); //147
                EntityBase.AppendPreparedField(ref doc, " ".PadLeft(200, ' '), 200); //148
                EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6); //149

                #endregion

                arquivo.SalvaItens(cobrancaIds, pm);

                if (_pm == null) { pm.Commit(); } //commit apenas se nao estiver participando de uma transacao externa
                arquivoConteudo = doc.ToString();
                return arquivoConteudo;
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }

        /// <summary>
        /// PRIMEIRA VERSÃO DO MÉTODO
        /// </summary>
        public static List<SumarioArquivoGeradoVO> GeraDocumentoCobranca_UNIBANCO(IList<Cobranca> cobrancas, ArquivoRemessaAgendamento ara)
        {
            List<SumarioArquivoGeradoVO> vos = new List<SumarioArquivoGeradoVO>();

            //separa as operadoras
            List<String> operadoraIDs = new List<String>();
            foreach (Cobranca cobranca in cobrancas)
            {
                if (!operadoraIDs.Contains(cobranca.OperadoraID.ToString()))
                    operadoraIDs.Add(cobranca.OperadoraID.ToString());
            }

            //processa os arquivos, um para cada operadora.
            foreach (String operadoraId in operadoraIDs)
            {
                String arquivoNome = "", arquivoConteudo = "", arquivoVersao = "", operadoraNome = "";
                Object arquivoId = null;
                Int32 qtdCobrancas = 0;

                Operadora operadora = new Operadora(operadoraId);
                operadora.Carregar();
                operadoraNome = operadora.Nome;

                List<String> cobrancaIDs = new List<String>();
                foreach (Cobranca cobranca in cobrancas)
                {
                    if (cobranca.OperadoraID.ToString() != operadoraId) { continue; }
                    cobrancaIDs.Add(cobranca.ID.ToString());
                }

                qtdCobrancas = cobrancaIDs.Count;
                GeraDocumentoCobranca_UNIBANCO(cobrancaIDs, ref arquivoNome, ref arquivoConteudo, ref arquivoId, ref arquivoVersao, ara.ID, null);

                //armazena na colecao para retorno à UI
                SumarioArquivoGeradoVO vo = new SumarioArquivoGeradoVO();
                vo.ArquivoConteudo = arquivoConteudo;
                vo.ArquivoID = arquivoId;
                vo.ArquivoNome = arquivoNome;
                vo.ArquivoVersao = arquivoVersao;
                vo.OperadoraID = operadoraId;
                vo.OperadoraNome = operadora.Nome;
                vo.QtdCobrancas = qtdCobrancas;
                vos.Add(vo);
            }

            return vos;
        }
        /// <summary>
        /// PRIMEIRA versão do método.
        /// </summary>
        internal static String GeraDocumentoCobranca_UNIBANCO      (IList<String> cobrancaIDs, ref String arquivoNome, ref String arquivoConteudo, ref Object arquivoId, ref String arquivoVersao, Object agendamentoId, PersistenceManager _pm)
        {
            PersistenceManager pm = _pm;

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.BeginTransactionContext();
            }

            try
            {
                if (cobrancaIDs == null) { return null; }

                //Carrega cobrancas envolvidas
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas(cobrancaIDs, pm);
                List<Object> cobrancaIds = new List<Object>();
                if (cobrancas == null) { return null; }

                String dataAgora = DateTime.Now.ToString("ddMMyy");
                String versao = "";
                StringBuilder doc = new StringBuilder();

                Cobranca.eCarteira carteira = (Cobranca.eCarteira)cobrancas[0].Carteira;

                #region HEADER 

                EntityBase.AppendPreparedField(ref doc, "0", 1);
                EntityBase.AppendPreparedField(ref doc, "1", 1);
                EntityBase.AppendPreparedField(ref doc, "REMESSA", 7);
                EntityBase.AppendPreparedField(ref doc, "03", 2);
                EntityBase.AppendPreparedField(ref doc, "COBR.  ESPECIAL", 15); //005

                //////////////////////////////////////////////////////////////////////////////////////
                if (carteira == Cobranca.eCarteira.Unibanco)
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C006.PadLeft(11, ' '), 11);
                else
                    EntityBase.AppendPreparedField(ref doc, "9108080533".PadLeft(11, ' '), 11);
                //////////////////////////////////////////////////////////////////////////////////////

                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C007, 4);//Se utilizado, o 011 é preenchido com zeros. Se 011 é ultilizado, 007 é preenchido com zeros
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C008, 1);  //008
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C009, 1);  //009 
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C010, 1);  //010
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C011, 7);  //011
                EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C012, 2);  //012 ????
                EntityBase.AppendPreparedField(ref doc, " ", 41);       //013
                EntityBase.AppendPreparedField(ref doc, dataAgora, 6);  //014
                EntityBase.AppendPreparedField(ref doc, "01600", 5);    //015
                EntityBase.AppendPreparedField(ref doc, "BPI", 3);      //016
                EntityBase.AppendPreparedField(ref doc, " ", 116);      //017
                EntityBase.AppendPreparedField(ref doc, "0".PadRight(167, '0'), 167); //018

                ArquivoCobrancaUnibanco arquivo = new ArquivoCobrancaUnibanco();
                arquivo.DataCriacao = DateTime.Now;
                versao = ArquivoCobrancaUnibanco.ObtemProximaVersao(pm).ToString().PadLeft(3, '0');
                arquivo.Versao = Convert.ToInt32(versao);
                arquivo.AgendamentoID = agendamentoId;

                arquivoNome = String.Concat(versao, "_", arquivo.DataCriacao.ToString("ddMMyyHHmmffff"), ".txt");

                arquivoVersao = versao.PadRight(3, '0');
                EntityBase.AppendPreparedField(ref doc, versao, 3);     //019
                EntityBase.AppendPreparedField(ref doc, "000001", 6);   //020

                #endregion

                arquivo.MesReferencia = cobrancas[0].DataVencimento.Month;
                arquivo.AnoReferencia = cobrancas[0].DataVencimento.Year;
                arquivo.OperadoraID = cobrancas[0].OperadoraID;
                arquivo.Nome = arquivoNome;
                arquivo.QtdCobrancas = cobrancaIDs.Count;
                pm.Save(arquivo);
                arquivoId = arquivo.ID;

                #region DETAIL 

                Endereco endereco = null;
                Int32 numSequencial = 1, diaDataSemJuros, result = 0;
                String mascara6 = new String('0', 6), nossoNumero = "";
                Decimal valorTotal = 0, mora = 0, multa = 0;
                Object contratoAdmID = null, valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                CalendarioVencimento rcv = null;

                foreach (Cobranca cob in cobrancas)
                {
                    numSequencial++;
                    cobrancaIds.Add(cob.ID);

                    #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                    contratoAdmID = Contrato.CarregaContratoAdmID(cob.PropostaID, pm);
                    IList<TabelaValor> tabela = TabelaValor.CarregarTabelaAtual(contratoAdmID, pm);
                    if (tabela != null && tabela.Count > 0)
                    {
                        Taxa taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                        tabela = null;
                        if (taxa != null && !taxa.Embutido)
                        {
                            cob.Valor += taxa.ValorEmbutido;
                            taxa = null;
                        }
                    }
                    #endregion

                    #region data de admissao do contrato

                    Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                    if (_contrato != null) { contratoAdmissao = _contrato.Admissao; _contrato = null; }
                    else { continue; }
                    #endregion

                    valorTotal += cob.Valor;
                    doc.Append(Environment.NewLine);

                    EntityBase.AppendPreparedField(ref doc, "2", 1); //021

                    nossoNumero = cob.GeraNossoNumero();
                    EntityBase.AppendPreparedField(ref doc, nossoNumero, 14); //022
                    EntityBase.AppendPreparedField(ref doc, cob._CalculaDVMod11(cob.Tipo, cob.ContratoCodCobranca, cob.Parcela), 1); //cob.CalculaDVMod11(nossoNumero), 1); //023 - Depende do 009 ou do 302 (usamos o 009)
                    EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyy"), 6); //024
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C025, 4);  //025

                    //////////////////////////////////////////////////////////////////////////////////////
                    if (carteira == Cobranca.eCarteira.Unibanco)
                        EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C026.PadLeft(11, '0'), 11); //026
                    else
                        EntityBase.AppendPreparedField(ref doc, "9108080533".PadLeft(11, ' '), 11);
                    //////////////////////////////////////////////////////////////////////////////////////

                    EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 30); //027

                    endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                    endereco.Carregar(pm);
                    if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                    EntityBase.AppendPreparedField(ref doc, "", 30);              //028
                    EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 20); //029
                    EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 20); //030
                    EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);      //031
                    EntityBase.AppendPreparedField(ref doc, endereco.CEP.Substring(0, 5), 5); //032
                    EntityBase.AppendPreparedField(ref doc, endereco.CEP.Substring(5, 3), 3); //033 
                    EntityBase.AppendPreparedField(ref doc, dataAgora, 6);        //034
                    EntityBase.AppendPreparedField(ref doc, "", 2);               //035
                    EntityBase.AppendPreparedField(ref doc, "0000000000", 10);    //036 -- quantidade de moeda - para moeda diferente de REAL
                    EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2").Replace(".", "").Replace(",", "").PadLeft(15, '0'), 15); //037 -- ???? valor do titulo - como preencher
                    EntityBase.AppendPreparedField(ref doc, "000000000000000", 15);//038 -- desconto 

                    mora = Convert.ToDecimal(CobrancaConfig.C039) * cob.Valor;
                    EntityBase.AppendPreparedField(ref doc, mora.ToString("N2").Replace(",","").Replace(".","").PadLeft(12,'0'), 12);   //039 -- juros de mora 

                    multa = Convert.ToDecimal(CobrancaConfig.MultaPercentual) * cob.Valor;
                    EntityBase.AppendPreparedField(ref doc, multa.ToString("N2").Replace(",", "").Replace(".", "").PadLeft(12, '0'), 12);   //040 -- multa atraso 
                    EntityBase.AppendPreparedField(ref doc, "000", 3);             //041 -- num da parcela. deve ser 000 ????
                    EntityBase.AppendPreparedField(ref doc, " ", 42);              //042
                    EntityBase.AppendPreparedField(ref doc, cob.ContratoNumero.PadRight(17, ' '), 17); //043 - numero do documento
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C044a, 1);
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C044, 6);//044 -- especie documento
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C045, 2);//045 -- ???? aceite
                    EntityBase.AppendPreparedField(ref doc, dataAgora, 6);          //046 -- data processamento

                    //////////////////////////////////////////////////////////////////////////////////////
                    if (carteira == Cobranca.eCarteira.Unibanco)
                        EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C047, 2);//047
                    else
                        EntityBase.AppendPreparedField(ref doc, "20", 2);//047
                    //////////////////////////////////////////////////////////////////////////////////////

                    EntityBase.AppendPreparedField(ref doc, "0", 1);                //048 -- ???? indicador do registro de msg
                    EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyy"), 6); //049 data limite desconto

                    //CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contratoAdmID, contratoAdmissao,
                    //    out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, pm);

                    //if(diaDataSemJuros > -1)
                    //    dataSemJuros = cob.DataVencimento.AddDays(diaDataSemJuros);
                    //else
                    //    dataSemJuros = cob.DataVencimento;
                    //EntityBase.AppendPreparedField(ref doc, dataSemJuros.ToString("ddMMyy"), 6);//050 -- ???? data para multa

                    EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.AddDays(1).ToString("ddMMyy"), 6);//050 -- data para multa

                    EntityBase.AppendPreparedField(ref doc, "0", 1);                   //051 -- prazo de mora diária
                    EntityBase.AppendPreparedField(ref doc, "0", 1);                   //052 -- codigo de moeda
                    EntityBase.AppendPreparedField(ref doc, "0".PadRight(18,' '), 18); //053 -- uso do banco

                    //contr
                    if (endereco.Logradouro.Length > 48)
                    {
                        endereco.Logradouro = endereco.Logradouro.Substring(0, 48);
                    }

                    endereco.Logradouro += ", " + endereco.Numero;

                    if(!String.IsNullOrEmpty(endereco.Complemento))
                    {
                        endereco.Logradouro += " " + endereco.Complemento;
                    }

                    EntityBase.AppendPreparedField(ref doc, endereco.Logradouro.PadRight(60, ' '), 60); //054

                    EntityBase.AppendPreparedField(ref doc, " ", 3);                                                 //055
                    EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6);//056

                    #region  descomentar bloco qdo o biro estiver preparado  
                    //CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contratoAdmID, cob.DataVencimento,
                    //    out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                    //if (valorDataLimite != null)
                    //{
                    //    if (Int32.TryParse(Convert.ToString(valorDataLimite), out result))
                    //    {
                    //        if (result < cob.DataVencimento.Day)
                    //        {
                    //            dataLimite = new DateTime(cob.DataVencimento.AddMonths(1).Year,
                    //                cob.DataVencimento.AddMonths(1).Month, result);
                    //        }
                    //        else
                    //        {
                    //            dataLimite = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, result);
                    //        }

                    //        EntityBase.AppendPreparedField(ref doc, dataLimite.ToString("ddMMyy"), 6); //data limite calculada
                    //    }
                    //    else
                    //    {
                    //        EntityBase.AppendPreparedField(ref doc, Convert.ToString(valorDataLimite), 40);
                    //    }
                    //}
                    #endregion

                    cob.ArquivoIDUltimoEnvio = arquivo.ID;
                    pm.Save(cob);
                }
                #endregion

                #region TRAILLER 

                numSequencial++;
                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "9", 1);  //142
                EntityBase.AppendPreparedField(ref doc, "", 25);  //143
                //////////////////////////////////////////////////////////////////////////////////////
                if (carteira == Cobranca.eCarteira.Unibanco)
                    EntityBase.AppendPreparedField(ref doc, CobrancaConfig.C006, 11); //144
                else
                    EntityBase.AppendPreparedField(ref doc, "9108080533".PadLeft(11, ' '), 11);
                //////////////////////////////////////////////////////////////////////////////////////
                EntityBase.AppendPreparedField(ref doc, "", 334); //145
                EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6); //146
                EntityBase.AppendPreparedField(ref doc, valorTotal.ToString("N2").Replace(".", "").Replace(",", "").PadLeft(17, '0'), 17); //147
                EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6); //148

                #endregion

                arquivo.SalvaItens(cobrancaIds, pm);

                if (_pm == null) { pm.Commit(); }
                arquivoConteudo = doc.ToString();
                return arquivoConteudo;
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }

        static DateTime CToDateTime(String param6pos, Int32 hora, Int32 minunto, Int32 segundo, Boolean ddMMyy)
        {
            Int32 dia;
            Int32 mes;
            Int32 ano;

            if (ddMMyy)
            {
                dia = Convert.ToInt32(param6pos.Substring(0, 2));
                mes = Convert.ToInt32(param6pos.Substring(2, 2));
                ano = Convert.ToInt32(param6pos.Substring(4, 2));
            }
            else
            {
                ano = Convert.ToInt32(param6pos.Substring(0, 2));
                mes = Convert.ToInt32(param6pos.Substring(2, 2));
                dia = Convert.ToInt32(param6pos.Substring(4, 2));
            }

            if (ano >= 0 && ano <= 95)
                ano = Convert.ToInt32("20" + ano.ToString());
            else
                ano = Convert.ToInt32("19" + ano.ToString());

            DateTime data = new DateTime(ano, mes, dia, hora, minunto, segundo);
            return data;
        }

        public static List<CriticaRetornoVO> ProcessaRetorno(IList<String> linhas, out Int32 titulosProcessados, out Int32 titulosBaixados)
        {
            PersistenceManager pm = new PersistenceManager();
            //pm.BeginTransactionContext();

            int i = 0; String linhaAtual = null;

            try
            {
                String codigoRegistro = null, codigoRejeicao = null;
                CriticaRetornoVO vo = null;
                Cobranca cobranca = null;

                List<CriticaRetornoVO> vos = new List<CriticaRetornoVO>();
                List<String> idsCobrancas = new List<String>();
                Contrato contrato = null;
                IList<TabelaValor> tabela = null;
                Taxa taxa = null;
                EstipulanteTaxa taxaEstipulante = null;

                titulosProcessados = linhas.Count - 2;
                titulosBaixados = 0;
                Boolean dupla = false;

                for (i = 0; i < linhas.Count; i++)
                {
                    pm = new PersistenceManager();
                    pm.BeginTransactionContext();

                    linhaAtual = linhas[i];

                    //codigo registro
                    if (linhaAtual == null || linhaAtual.Trim() == "") { continue; }
                    codigoRegistro = linhaAtual.Substring(0, 1);
                    if (codigoRegistro == "0" || codigoRegistro == "9") { continue; } //se é cabecalho ou trailler, ignora

                    cobranca = new Cobranca();
                    vo = new CriticaRetornoVO();

                    //DATA VENCIMENTO
                    String strDataVencimento = linhaAtual.Substring(146, 6);
                    vo.DataVencto = CToDateTime(strDataVencimento, 23, 59, 59, true);


                    //NOSSO NÚMERO - identifica tipo de cobranca, codigo de cobrança e num da parcela
                    vo.NossoNumero = linhaAtual.Substring(37, 16);
                    cobranca.LeNossoNumero(vo.NossoNumero);

                    if (cobranca.Tipo == (int)Cobranca.eTipo.Dupla)
                        dupla = true;
                    else
                        dupla = false;

                    vo.Parcela = cobranca.Parcela.ToString();
                    vo.PropostaCodCobranca = cobranca.ContratoCodCobranca;
                    vo.CobrancaTipo = cobranca.Tipo.ToString();

                    contrato = Contrato.CarregarParcialPorCodCobranca(vo.PropostaCodCobranca, pm);
                    if (contrato == null)
                    {
                        vo.Status = "Título não localizado";
                        vos.Add(vo); continue;
                    }
                    cobranca.PropostaID = contrato.ID;

                    if (contrato.Inativo) { vo.PropostaInativa = true; }

                    //valores do titulo
                    vo.Valor     = Convert.ToDecimal(linhaAtual.Substring(153, 12), new System.Globalization.CultureInfo("pt-Br")) / 100;
                    vo.ValorPgto = Convert.ToDecimal(linhaAtual.Substring(254, 12), new System.Globalization.CultureInfo("pt-Br")) / 100;

                    cobranca = Cobranca.CarregarPor(cobranca.PropostaID, cobranca.Parcela, ((int)Cobranca.eTipo.Indefinido), pm);

                    if (cobranca != null)
                    {
                        vo.CobrancaID      = Convert.ToString(cobranca.ID);
                        cobranca.ValorPgto = vo.ValorPgto;
                        vo.OperadoraNome   = cobranca.OperadoraNome;
                        vo.PropostaNumero  = cobranca.ContratoNumero;
                        if (vo.OperadoraNome != null && vo.OperadoraNome.Length > 30)
                            vo.OperadoraNome = vo.OperadoraNome.Substring(0, 30);

                        if (idsCobrancas.Contains(vo.CobrancaID)) { vo.EmDuplicidade = true; }
                        else { idsCobrancas.Add(vo.CobrancaID); }
                    }

                    //checa se foi rejeitado
                    codigoRejeicao = linhaAtual.Substring(378, 2);
                    if (codigoRejeicao != "00") // foi rejeitado
                    {
                        vo.CodigoRejeicao = linhaAtual.Substring(378, 2);
                        vo.Status = linhaAtual.Substring(378, 2) + " Rejeitado"; //TODO
                        vo.PagamentoRejeitado = true;
                        vos.Add(vo);
                        continue;
                    }
                    else
                    {
                        //DATA PAGTO
                        String strDataPgto = linhaAtual.Substring(292, 6);
                        vo.DataPgto = CToDateTime(strDataPgto, 0, 0, 0, false);
                    }

                    if (cobranca == null) //cobranca nao localizada
                    {
                        if (String.IsNullOrEmpty(vo.Status)) { vo.Status = "Título não localizado"; }
                        vos.Add(vo); continue;
                    } 
                    else
                    {
                        cobranca.DataPgto  = vo.DataPgto;
                        cobranca.ValorPgto = vo.ValorPgto;

                        tabela = TabelaValor.CarregarTabelaAtual(contrato.ContratoADMID, pm);
                        if (tabela != null)
                        {
                            taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                        }
                        else
                        {
                            taxa = null;
                        }

                        Decimal valorBruto = vo.ValorPgto;
                        if (taxa != null && !taxa.Embutido) { valorBruto -= taxa.ValorEmbutido; }

                        //COBRANCA FOI PAGA
                        //calcula o valor pago
                        if (contrato.CobrarTaxaAssociativa && ((Cobranca.eTipo)cobranca.Tipo) == Cobranca.eTipo.Normal)
                        {
                            taxaEstipulante = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID);
                            if (taxaEstipulante != null)
                            {
                                valorBruto -= taxaEstipulante.Valor; //ATENCAO: e se for taxa por vida?
                            }
                        }

                        if (valorBruto < cobranca.Valor)
                        {
                            vo.ValorMenor = true;
                        }

                        cobranca.Pago = true;
                        if (!dupla) //if (((Cobranca.eTipo)cobranca.Tipo) != Cobranca.eTipo.Dupla)
                        {
                            cobranca.ValorPgto = valorBruto; //a comissao será paga sobre este valor
                            pm.Save(cobranca);
                        }
                        //se a cobranca é dupla e está paga, baixa tb a cobranca referencia
                        else //if (((Cobranca.eTipo)cobranca.Tipo) == Cobranca.eTipo.Dupla)
                        {
                            Cobranca cobrancaReferencia = Cobranca.CarregarPor(cobranca.PropostaID, (cobranca.Parcela - 1), ((int)Cobranca.eTipo.Indefinido), pm); //new Cobranca(cobranca.CobrancaRefID);
                            if (cobrancaReferencia != null)
                            {
                                cobrancaReferencia.ValorPgto = valorBruto - cobranca.Valor; //o valor pago menos o valor nominal da cobranca mais atual é o valor pago da cobranca mais antiga
                                cobrancaReferencia.DataPgto = cobranca.DataPgto;
                                cobrancaReferencia.Pago = true;
                                pm.Save(cobrancaReferencia);
                            }
                            else
                                cobrancaReferencia = new Cobranca();

                            cobranca.ValorPgto = cobranca.Valor;
                            cobrancaReferencia.Pago = true;
                            pm.Save(cobranca);
                        }

                        titulosBaixados++;
                        vo.Status = "Título baixado";
                        vos.Add(vo);
                    }

                    pm.Commit();
                }

                if (titulosBaixados > 0)
                {
                    //LC.Web.PadraoSeguros.Facade.ContratoFacade.Instance.AtribuiStatusAdimplenteOuInadimplente(pm);
                }

                //pm.Commit();

                return vos;
            }
            catch //(Exception ex)
            {
                try
                {
                    pm.Rollback();
                }
                catch { }
                throw; //ex;
            }
            finally
            {
                pm = null;
            }
        }



        /***************************************************************************************************************/

        /// <summary>
        /// SEGUNDA VERSÃO DO MÉTODO
        /// </summary>
        public static List<SumarioArquivoGeradoVO> GeraDocumentoCobranca_BB(IList<Cobranca> cobrancas, IList<ArquivoRemessaAgendamento> aras)
        {
            List<SumarioArquivoGeradoVO> vos = new List<SumarioArquivoGeradoVO>();

            //separa as operadoras
            List<String> operadoraIDs = new List<String>();
            System.Collections.Hashtable ht = new System.Collections.Hashtable();

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            //foreach (Cobranca cobranca in cobrancas)
            //{
            //    if (!operadoraIDs.Contains(cobranca.OperadoraID.ToString()))
            //        operadoraIDs.Add(cobranca.OperadoraID.ToString());
            //}

            pm.CloseSingleCommandInstance();
            pm.Dispose();

            //processa os arquivos, um para cada operadora.
            //foreach (String operadoraId in operadoraIDs)
            //{
            String arquivoNome = "", arquivoConteudo = "", arquivoVersao = "", operadoraNome = "";
            Object arquivoId = null;
            Int32 qtdCobrancas = 0;

            Operadora operadora = new Operadora(cobrancas[0].OperadoraID); //Operadora operadora = new Operadora(operadoraId);
            operadora.Carregar();
            operadoraNome = operadora.Nome;

            List<String> cobrancaIDs = new List<String>();
            foreach (Cobranca cobranca in cobrancas)
            {
                //if (cobranca.OperadoraID.ToString() != operadoraId) { continue; }
                cobrancaIDs.Add(cobranca.ID.ToString());
            }

            qtdCobrancas = cobrancaIDs.Count;
            /////////////GeraDocumentoCobranca_UNIBANCO2(cobrancaIDs, ref arquivoNome, ref arquivoConteudo, ref arquivoId, ref arquivoVersao, aras, null);
            GeraDocumentoCobranca_BB(cobrancaIDs, ref arquivoNome, ref arquivoConteudo, ref arquivoId, ref arquivoVersao, aras, null);

            //armazena na colecao para retorno à UI
            SumarioArquivoGeradoVO vo = new SumarioArquivoGeradoVO();
            vo.ArquivoConteudo = arquivoConteudo;
            vo.ArquivoID = arquivoId;
            vo.ArquivoNome = arquivoNome;
            vo.ArquivoVersao = arquivoVersao;
            vo.OperadoraID = cobrancas[0].OperadoraID; //operadoraId;
            vo.OperadoraNome = operadora.Nome;
            vo.QtdCobrancas = qtdCobrancas;
            vos.Add(vo);
            //}

            return vos;
        }
        /// <summary>
        /// SEGUNDA VERSÃO DO MÉTODO
        /// </summary>
        public static String GeraDocumentoCobranca_BB(IList<String> cobrancaIDs, ref String arquivoNome, ref String arquivoConteudo, ref Object arquivoId, ref String arquivoVersao, IList<ArquivoRemessaAgendamento> aras, PersistenceManager _pm)
        {
            if (cobrancaIDs == null) { return null; }

            Int32 numSequencial = 0;

            PersistenceManager pm = _pm;

            Int32 contratoadmQualicorpID = Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]);

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();
            }

            Cobranca cobprova = new Cobranca(cobrancaIDs[0]);
            pm.Load(cobprova);
            Contrato contratoprova = new Contrato(cobprova.PropostaID);
            pm.Load(contratoprova);

            //String agCCDV_itau = "";
            String headerCedente = "UBRASP";
            string agencia = "05956", agenciadv = "0";
            string numcontacorr = "40037".PadLeft(12, '0');
            string dvcontacorr = "8";
            string cnpj = "49938327000106";

            ////////////////////////////////////////////////// Só haverá cedente ubrasp

            //agCCDV_itau = "0646042606";    //qualicorp
            string numConvenio = Cobranca.ConvenioUBRASP.PadLeft(9, '0'), carteira = "11", variacao = "019";
            //////////////////////////////////////////////////

            try
            {
                if (cobrancaIDs == null) { return null; }

                String dataAgora = DateTime.Now.ToString("ddMMyy");
                String versao = "";
                StringBuilder doc = new StringBuilder();

                ////Cobranca.eCarteira carteira = (Cobranca.eCarteira)aras[0].Carteira;

                if( aras == null)
                    arquivoNome = "arquivo.dat"; // 
                else
                    arquivoNome = String.Concat(aras[0].ArquivoNome, "_", DateTime.Now.ToString("yyyyMMddHHmmss"), ".dat");  //String.Concat(aras[0].ArquivoNomeInstance, "_", DateTime.Now.ToString("yyyyMMddHHmmss"), ".dat");

                #region HEADER -------------------------------------------

                EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0000", 4);         //lote de serviço
                EntityBase.AppendPreparedField(ref doc, "0", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "         ", 9);    //brancos
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //tipo de inscrição - 2 para CNPJ, 1 para CPF
                //EntityBase.AppendPreparedField(ref doc, "0".PadLeft(14, '0'), 14);  //CNPJ (ou cpf) //faltou: cnpj
                EntityBase.AppendPreparedField(ref doc, cnpj, 14);          //CNPJ (ou cpf) 
                EntityBase.AppendPreparedField(ref doc, numConvenio, 9);    //Nùmero do convênio de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "0014", 4);         //Cobrança Cedente BB
                EntityBase.AppendPreparedField(ref doc, carteira, 2);       //Número da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, variacao, 3);       //Número da variação da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //Campo reservado BB
                EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome da Empresa
                EntityBase.AppendPreparedField(ref doc, "BANCO DO BRASIL S.A.".PadRight(30, ' '), 30); //Nome do Banco
                EntityBase.AppendPreparedField(ref doc, " ", 10);           //Uso Exclusivo FEBRABAN / CNAB
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //Código Remessa / Retorno - 1=remessa, 2=retorno
                EntityBase.AppendPreparedField(ref doc, DateTime.Now.ToString("ddMMyyyy"), 8); //Data de Geração do Arquivo
                EntityBase.AppendPreparedField(ref doc, DateTime.Now.ToString("HHmmss"), 6);   //Hora de Geração do Arquivo

                //posso usar a propriedade versao da classe...
                EntityBase.AppendPreparedField(ref doc, "000000", 6);       //Número Seqüencial do Arquivo

                EntityBase.AppendPreparedField(ref doc, "000", 3);          //Nº da Versão do Layout do Arquivo
                EntityBase.AppendPreparedField(ref doc, "00000", 5);        //Densidade de Gravação do Arquivo
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(20, ' '), 20); //Para Uso Reservado do Banco
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(20, ' '), 20); //Para Uso Reservado da Empresa
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(29, ' '), 29); //Para Uso Reservado da Empresa

                ArquivoCobrancaUnibanco arquivo = new ArquivoCobrancaUnibanco();
                arquivo.DataCriacao = DateTime.Now;
                versao = ArquivoCobrancaUnibanco.ObtemProximaVersao(pm).ToString().PadLeft(3, '0');
                arquivo.Versao = Convert.ToInt32(versao);
                arquivo.AgendamentoID = aras[0].ID; // ara.ID; 

                ArquivoRemessaCriterio arqCriterio = null; //new ArquivoRemessaCriterio(ara.CriterioID);
                //pm.Load(arqCriterio);

                //arquivoVersao = versao.PadRight(3, '0');
                //EntityBase.AppendPreparedField(ref doc, versao, 3);     //019
                //EntityBase.AppendPreparedField(ref doc, " ".PadLeft(200, ' '), 200); //20
                //EntityBase.AppendPreparedField(ref doc, "000001", 6);   //021

                #endregion

                #region HEADER DE LOTE -----------------------------------

                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "R", 1);            //tipo de operação
                EntityBase.AppendPreparedField(ref doc, "01", 2);           //tipo de serviço
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //
                EntityBase.AppendPreparedField(ref doc, "000", 3);          //Nº da Versão do Layout do Lote - nao criticado pelo banco
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //Tipo de Inscrição da Empresa - 2=CNPJ
                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);  //CNPJ (ou cpf) //faltou: cnpj

                EntityBase.AppendPreparedField(ref doc, numConvenio, 9);    //Nùmero do convênio de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "0014", 4);         //Cobrança Cedente BB
                EntityBase.AppendPreparedField(ref doc, carteira, 2);       //Número da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, variacao, 3);       //Número da variação da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //campo que identifica remessa de testes: brancos ou TS
                EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.

                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome da Empresa
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(40, ' '), 40); //Mensagem 1
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(40, ' '), 40); //Mensagem 2
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Número Remessa/Retorno 
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Data de Gravação Remessa/Retorno - pode ser zeros 
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Data do Crédito 
                EntityBase.AppendPreparedField(ref doc, " ", 33);           //Uso Exclusivo FEBRABAN/CNAB

                #endregion

                //Carrega cobrancas envolvidas
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela_Optimized(cobrancaIDs, pm); //
                List<Object> cobrancaIds = new List<Object>();
                if (cobrancas == null) { return null; }

                arquivo.MesReferencia = cobrancas[0].DataVencimento.Month;
                arquivo.AnoReferencia = cobrancas[0].DataVencimento.Year;
                arquivo.OperadoraID = cobrancas[0].OperadoraID;
                arquivo.Nome = arquivoNome;
                arquivo.QtdCobrancas = cobrancaIDs.Count;
                pm.Save(arquivo); ////////////////////////////////////////////////////////////////////////
                arquivoId = arquivo.ID;

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL -------------------------------------------

                Endereco endereco = null;
                 //, diaDataSemJuros, result = 0, 
                int totalRegistrosDetalhe = 0;
                String mascara6 = new String('0', 6); //, nossoNumero = "";
                Decimal valorTotal = 0; //, mora = 0, multa = 0;
                //Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                //CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                int i = 0;

                //String[] contratoAdmIds = null;
                System.Collections.Hashtable contratoADM_Criterio = new System.Collections.Hashtable();
                List<string> parametrosProcessados = new List<string>();
                Cobranca _cob = null;

                foreach (Cobranca cob in cobrancas)
                {
                    if (parametrosProcessados.Contains(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID))) continue;

                    parametrosProcessados.Add(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID));

                    if (cob.ID == null) { continue; }

                    _cob = null;
                    _cob = Cobranca.CarregarEnviadasPor(cob.PropostaID, cob.DataVencimento, (int)Cobranca.eTipo.Normal, pm);
                    if (_cob != null) { continue; }

                    i++;
                    try
                    {
                        #region DETAIL P ---------------------------------

                        #region

                        #region localiza o critério correspondente

                        contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                        //if (contratoADM_Criterio.Contains(Convert.ToString(contrato.ContratoADMID)))
                        //{
                        //    arqCriterio = (ArquivoRemessaCriterio)contratoADM_Criterio[Convert.ToString(contrato.ContratoADMID)];
                        //}
                        //else
                        //{
                        //    foreach (ArquivoRemessaAgendamento ara in aras)
                        //    {
                        //        if (cob.Criterio != null) { break; }

                        //        contratoAdmIds = ArquivoRemessaCriterio.CarregarContratoAdmIds(ara.CriterioID, pm); /////

                        //        foreach (String contratoAdmId in contratoAdmIds)
                        //        {
                        //            if (Convert.ToString(contrato.ContratoADMID).Equals(contratoAdmId))
                        //            {
                        //                ArquivoRemessaCriterio criterio = new ArquivoRemessaCriterio(ara.CriterioID);
                        //                pm.Load(criterio);
                        //                arqCriterio = criterio;

                        //                try
                        //                {
                        //                    contratoADM_Criterio.Add(contratoAdmId, criterio);
                        //                }
                        //                catch //(Exception ex)
                        //                {
                        //                    continue;
                        //                }
                        //                break;
                        //            }
                        //        }
                        //    }
                        //}
                        #endregion

                        numSequencial++;

                        cobrancaIds.Add(cob.ID);

                        if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                        {
                            numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                            qtdTotalBoletos = 0;
                            foreach (Cobranca _temp in cobrancas)
                            {
                                if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                                else if (qtdTotalBoletos > 0) { break; }
                            }
                        }

                        numBoletoCorrente++;

                        #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                        retTvv = TabelaValorVencimentoVO.ExisteProposta(cob.PropostaID, lista);
                        if (retTvv == null)
                        {
                            //contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                            tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cob.DataVencimento, pm);
                            if (tabela != null && tabela.Count > 0)
                            {
                                retTvv = TabelaValorVencimentoVO.ExisteTabela(tabela[0].ID, lista);
                                if (retTvv == null)
                                {
                                    taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                                    if (taxa != null && !taxa.Embutido)
                                    {
                                        cob.Valor += taxa.ValorEmbutido;
                                    }

                                    retTvv = new TabelaValorVencimentoVO();
                                    retTvv.PropostaID = contrato.ID;
                                    retTvv.TabelaID = tabela[0].ID;
                                    if (tabela[0].VencimentoFim != DateTime.MinValue)
                                    { retTvv.Vencimento = tabela[0].VencimentoFim.AddMonths(1); }
                                    if (taxa != null && !taxa.Embutido)
                                    { retTvv.ValorBancario = taxa.ValorEmbutido; }

                                    if (contrato.CobrarTaxaAssociativa)
                                    {
                                        estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                                        if (estipulanteTaxa != null)
                                        { retTvv.ValorSindicalizacao = estipulanteTaxa.Valor; }
                                    }

                                    lista.Add(retTvv);
                                    //taxa = null;
                                }
                                else
                                    cob.Valor += retTvv.ValorBancario;
                            }
                        }
                        else
                            cob.Valor += retTvv.ValorBancario;

                        #endregion

                        #region data de admissao do contrato

                        //Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                        else { continue; }
                        #endregion

                        #endregion

                        doc.Append(Environment.NewLine);

                        EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº do Registro - 04.3A
                        EntityBase.AppendPreparedField(ref doc, "P", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de Movimento Remessa - C004

                        //
                        EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                        EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                        EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.

                        cob.NossoNumero = cob.GeraNossoNumero();
                        EntityBase.AppendPreparedField(ref doc, cob.GeraNossoNumero(), 20); //Seu Número
                        //EntityBase.AppendPreparedField(ref doc, "0".PadRight(20, '0'), 20); //Seu Número - pos 38

                        EntityBase.AppendPreparedField(ref doc, "1", 1);   // Código da carteira 11
                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Forma de Cadastr. do Título no Banco - ampo não tratado pelo sistema do Banco do Brasil
                        EntityBase.AppendPreparedField(ref doc, " ", 1);   // Tipo de documento - campo não tratado pelo sistema do Banco do Brasil

                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Identificação da Emissão do Bloqueto: 1 banco emite, 0 nos emitimos C009 - 17.3P faltou: definir
                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Identificação da Destibuição do Bloqueto: 1 banco distribui, 0 nos distribuimos C008 - C010 - 18.3P faltou: definir
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadRight(15,' '), 15); // Número do Documento de Cobrança
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyyyy"), 8);  // Data de vencimento - pos 78

                        //valor do boleto
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2",cinfo).Split(',')[0].Replace(".","").PadLeft(13, '0') , 13);
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[1].PadLeft(2, '0'), 2);


                        EntityBase.AppendPreparedField(ref doc, "00000", 5);    //Agência Encarregada da Cobrança
                        EntityBase.AppendPreparedField(ref doc, " ", 1);        //Dígito Verificador da Agência

                        EntityBase.AppendPreparedField(ref doc, "02", 2);        //Espécie do Título - posicao 107
                        EntityBase.AppendPreparedField(ref doc, "A", 1);        //dentific. de Título Aceito/Não Aceito 
                        EntityBase.AppendPreparedField(ref doc, cob.DataCriacao.ToString("ddMMyyyy"), 8);  // Data de emissão - pos 110
                        EntityBase.AppendPreparedField(ref doc, "1", 1);        //Código do Juros de Mora faltou: definir '1'=Valor por Dia '2'=Taxa Mensal '3'=Isento
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.AddDays(1).ToString("ddMMyyyy"), 8);  // Data do Juros de Mora - pos 119
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Juros de Mora por Dia/Taxa faltou: definir

                        EntityBase.AppendPreparedField(ref doc, "0", 1);        //Código do Desconto 1 - pos 142
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8); //Data do Desconto 1  - pos 143
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Desconto 1 Valor/Percentual a ser Concedido 
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do IOF a ser Recolhido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do Abatimento
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(25, '0'), 25); //Identificação do Título na Empresa - pos 196

                        EntityBase.AppendPreparedField(ref doc, "3", 1); //Código para Protesto - 1' – Protestar dias corridos, '2' – Protestar dias úteis, e '3' – Não protestar. faltou: definir - pos 221
                        EntityBase.AppendPreparedField(ref doc, "00", 2); //Número de Dias para Protesto - Preencher de acordo com o código informado na posição 221. Para código '1' – é possível, de 6 a 29 dias, 35º, 40º, dia corrido. Para código '2' – é possível, 3º, 4º ou 5º dia útil. Para código '3' preencher com Zeros. faltou: definir
                        EntityBase.AppendPreparedField(ref doc, "0", 1); // Código para Baixa/Devolução - não tratado
                        EntityBase.AppendPreparedField(ref doc, "000", 3); // Número de Dias para Baixa/Devolução - não tratado
                        EntityBase.AppendPreparedField(ref doc, "09", 2); // Código da Moeda - g065
                        EntityBase.AppendPreparedField(ref doc, "0000000000", 10); // Nº do Contrato da Operação de Créd.  - não tratado
                        EntityBase.AppendPreparedField(ref doc, " ", 1); //Uso Exclusivo FEBRABAN/CNAB 


                        #endregion 

                        #region DETAIL Q ---------------------------------

                        doc.Append(Environment.NewLine);

                        numSequencial++;

                        EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº do Registro - 04.3A
                        EntityBase.AppendPreparedField(ref doc, "Q", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de Movimento Remessa - C004 - pos 16

                        EntityBase.AppendPreparedField(ref doc, "1", 1);           //Tipo e inscrição - C005
                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularCPF.PadLeft(15, '0'), 15); //Número de inscrição - C006 - pos 19

                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 40); //Nome - C013 - pos 34

                        endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                        endereco.Carregar(pm);
                        if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                        if (endereco.Numero == null) endereco.Numero = "";
                        if (string.IsNullOrEmpty(endereco.Complemento))
                        {
                            int tamanhoDisponivel = 40 - (endereco.Numero.Length + 1);

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if(!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero);
                        }
                        else
                        {
                            int tamanhoDisponivel = 40 - ((endereco.Numero.Length + 1) + (endereco.Complemento.Length + 1));

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero, ",", endereco.Complemento);
                            else
                                endereco.Logradouro += string.Concat(",", endereco.Complemento);
                        }

                        EntityBase.AppendPreparedField(ref doc, endereco.Logradouro, 40); // endereco - pos 74
                        EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP, 8);
                        EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);

                        EntityBase.AppendPreparedField(ref doc, "0", 1); //Tipo de Inscrição - pos 154
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Número de Inscrição

                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(40, ' '), 40); //Nome do Sacador/Avalista
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(3, '0'), 3); //Cód. Bco. Corresp. na Compensação - não tratado - pos 210
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(20, ' '), 20); //Nosso Nº no Banco Correspondente - não tratado
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(8, ' '), 8); //Uso Exclusivo FEBRABAN/CNAB 

                        #region Texto data limite - comentado 
                        //if (valorDataLimite != null) // && Convert.ToString(valorDataLimite) != "0")
                        //{
                        //    if (Int32.TryParse(Convert.ToString(valorDataLimite), out result))
                        //    {
                        //        if (result < cob.DataVencimento.Day)
                        //        {
                        //            dataLimite = new DateTime(cob.DataVencimento.AddMonths(1).Year,
                        //                cob.DataVencimento.AddMonths(1).Month, result);
                        //        }
                        //        else
                        //        {
                        //            dataLimite = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, result);
                        //        }

                        //        EntityBase.AppendPreparedField(ref doc, dataLimite.ToString("dd/MM/yyyy").PadRight(50, ' '), 50); //data limite calculada
                        //    }
                        //    else
                        //    {
                        //        EntityBase.AppendPreparedField(ref doc, Convert.ToString(valorDataLimite).PadRight(50, ' '), 50);
                        //    }
                        //}
                        //else
                        //    EntityBase.AppendPreparedField(ref doc, " ".PadRight(50, ' '), 50);  //Texto data limite

                        #endregion

                        cob.Carteira = Convert.ToInt32(carteira);
                        cob.ArquivoIDUltimoEnvio = arquivo.ID;
                        cob.DataCriacao = DateTime.Now;
                        cob.Carteira = 11;
                        pm.Save(cob); /////////////////////////////////////////////////////////////////////////////////

                        #endregion

                        totalRegistrosDetalhe++;
                        valorTotal += cob.Valor;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string path = System.Configuration.ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                            System.IO.File.WriteAllText(path + "err_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".txt",
                                ex.Message +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + ex.StackTrace +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + Convert.ToString(cob.ID) +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + i.ToString(),
                                System.Text.Encoding.ASCII);
                        }
                        catch { }
                        continue;
                    }

                    //numSequencial++;
                }
                #endregion

                #region TRAILLER DE LOTE ---------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "001", 3); 
                EntityBase.AppendPreparedField(ref doc, "0001", 4);
                EntityBase.AppendPreparedField(ref doc, "5", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9);
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 2).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 217);

                #endregion

                #region TRAILLER -----------------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "001", 3);
                EntityBase.AppendPreparedField(ref doc, "9999", 4);
                EntityBase.AppendPreparedField(ref doc, "9", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9); //branco
                EntityBase.AppendPreparedField(ref doc, "1".PadLeft(6, '0'), 6); //qtd de lotes
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 4).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, "0".PadLeft(6, '0'), 6); //Qtde de Contas p/ Conc. (Lotes)
                EntityBase.AppendPreparedField(ref doc, " ", 205); //

                //EntityBase.AppendPreparedField(ref doc, "", 334); //145
                //EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6); //146
                //EntityBase.AppendPreparedField(ref doc, valorTotal.ToString("N2").Replace(".", "").Replace(",", "").PadLeft(17, '0'), 17); //147
                //EntityBase.AppendPreparedField(ref doc, " ".PadLeft(200, ' '), 200); //148
                //EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6); //149

                #endregion

                arquivo.SalvaItens(cobrancaIds, pm);

                if (_pm == null) { pm.Commit(); } //commit apenas se nao estiver participando de uma transacao externa
                arquivoConteudo = doc.ToString();
                return arquivoConteudo;
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }

        public static String GeraDocumentoCobranca_BB(string cobrancaId, PersistenceManager _pm)
        {
            Int32 numSequencial = 0;

            PersistenceManager pm = _pm;

            Int32 contratoadmQualicorpID = Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]);

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();
            }

            Cobranca cobprova = new Cobranca(cobrancaId);
            pm.Load(cobprova);
            Contrato contratoprova = new Contrato(cobprova.PropostaID);
            pm.Load(contratoprova);

            //String agCCDV_itau = "";
            String headerCedente = "UBRASP";
            string agencia = "05956", agenciadv = "0";
            string numcontacorr = "40037".PadLeft(12, '0');
            string dvcontacorr = "8";
            string cnpj = "49938327000106"; //

            ////////////////////////////////////////////////// Só haverá cedente ubrasp

            //agCCDV_itau = "0646042606";    //qualicorp
            string numConvenio = Cobranca.ConvenioUBRASP.PadLeft(9, '0'), carteira = "11", variacao = "019";
            //////////////////////////////////////////////////

            try
            {
                if (cobrancaId == null) { return null; }

                String dataAgora = DateTime.Now.ToString("ddMMyy");
                StringBuilder doc = new StringBuilder();

                ////Cobranca.eCarteira carteira = (Cobranca.eCarteira)aras[0].Carteira;

                #region HEADER -------------------------------------------

                EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0000", 4);         //lote de serviço
                EntityBase.AppendPreparedField(ref doc, "0", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "         ", 9);    //brancos
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //tipo de inscrição - 2 para CNPJ, 1 para CPF
                //EntityBase.AppendPreparedField(ref doc, "0".PadLeft(14, '0'), 14);  //CNPJ (ou cpf) //faltou: cnpj
                EntityBase.AppendPreparedField(ref doc, cnpj, 14);          //CNPJ (ou cpf) 
                EntityBase.AppendPreparedField(ref doc, numConvenio, 9);    //Nùmero do convênio de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "0014", 4);         //Cobrança Cedente BB
                EntityBase.AppendPreparedField(ref doc, carteira, 2);       //Número da carteira de cobrança BB - pos.46
                EntityBase.AppendPreparedField(ref doc, variacao, 3);       //Número da variação da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //Campo reservado BB
                EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome da Empresa
                EntityBase.AppendPreparedField(ref doc, "BANCO DO BRASIL S.A.".PadRight(30, ' '), 30); //Nome do Banco
                EntityBase.AppendPreparedField(ref doc, " ", 10);           //Uso Exclusivo FEBRABAN / CNAB
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //Código Remessa / Retorno - 1=remessa, 2=retorno
                EntityBase.AppendPreparedField(ref doc, DateTime.Now.ToString("ddMMyyyy"), 8); //Data de Geração do Arquivo
                EntityBase.AppendPreparedField(ref doc, DateTime.Now.ToString("HHmmss"), 6);   //Hora de Geração do Arquivo

                //posso usar a propriedade versao da classe...
                EntityBase.AppendPreparedField(ref doc, "000000", 6);       //Número Seqüencial do Arquivo

                EntityBase.AppendPreparedField(ref doc, "000", 3);          //Nº da Versão do Layout do Arquivo
                EntityBase.AppendPreparedField(ref doc, "00000", 5);        //Densidade de Gravação do Arquivo
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(20, ' '), 20); //Para Uso Reservado do Banco
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(20, ' '), 20); //Para Uso Reservado da Empresa
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(29, ' '), 29); //Para Uso Reservado da Empresa

                #endregion

                #region HEADER DE LOTE -----------------------------------

                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "R", 1);            //tipo de operação
                EntityBase.AppendPreparedField(ref doc, "01", 2);           //tipo de serviço
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //
                EntityBase.AppendPreparedField(ref doc, "000", 3);          //Nº da Versão do Layout do Lote - nao criticado pelo banco
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //Tipo de Inscrição da Empresa - 2=CNPJ
                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);  //CNPJ (ou cpf) //faltou: cnpj

                EntityBase.AppendPreparedField(ref doc, numConvenio, 9);    //Nùmero do convênio de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "0014", 4);         //Cobrança Cedente BB
                EntityBase.AppendPreparedField(ref doc, carteira, 2);       //Número da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, variacao, 3);       //Número da variação da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //campo que identifica remessa de testes: brancos ou TS
                EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.

                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome da Empresa
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(40, ' '), 40); //Mensagem 1
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(40, ' '), 40); //Mensagem 2
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Número Remessa/Retorno 
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Data de Gravação Remessa/Retorno - pode ser zeros 
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Data do Crédito 
                EntityBase.AppendPreparedField(ref doc, " ", 33);           //Uso Exclusivo FEBRABAN/CNAB

                #endregion

                //Carrega cobrancas envolvidas
                List<string> cobrancaIDs = new List<string>(); cobrancaIDs.Add(cobrancaId);
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela_Optimized(cobrancaIDs, pm); //
                List<Object> cobrancaIds = new List<Object>();
                if (cobrancas == null) { return null; }

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL -------------------------------------------

                Endereco endereco = null;
                //, diaDataSemJuros, result = 0, 
                int totalRegistrosDetalhe = 0;
                String mascara6 = new String('0', 6); //, nossoNumero = "";
                Decimal valorTotal = 0; //, mora = 0, multa = 0;
                //Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                //CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                int i = 0;

                //String[] contratoAdmIds = null;
                System.Collections.Hashtable contratoADM_Criterio = new System.Collections.Hashtable();
                List<string> parametrosProcessados = new List<string>();

                foreach (Cobranca cob in cobrancas)
                {
                    if (parametrosProcessados.Contains(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID))) continue;

                    parametrosProcessados.Add(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID));

                    i++;
                    try
                    {
                        #region DETAIL P ---------------------------------

                        #region

                        #region localiza o critério correspondente

                        contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                        #endregion

                        numSequencial++;

                        cobrancaIds.Add(cob.ID);

                        if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                        {
                            numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                            qtdTotalBoletos = 0;
                            foreach (Cobranca _temp in cobrancas)
                            {
                                if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                                else if (qtdTotalBoletos > 0) { break; }
                            }
                        }

                        numBoletoCorrente++;

                        #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                        retTvv = TabelaValorVencimentoVO.ExisteProposta(cob.PropostaID, lista);
                        if (retTvv == null)
                        {
                            //contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                            tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cob.DataVencimento, pm);
                            if (tabela != null && tabela.Count > 0)
                            {
                                retTvv = TabelaValorVencimentoVO.ExisteTabela(tabela[0].ID, lista);
                                if (retTvv == null)
                                {
                                    taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                                    if (taxa != null && !taxa.Embutido)
                                    {
                                        cob.Valor += taxa.ValorEmbutido;
                                    }

                                    retTvv = new TabelaValorVencimentoVO();
                                    retTvv.PropostaID = contrato.ID;
                                    retTvv.TabelaID = tabela[0].ID;
                                    if (tabela[0].VencimentoFim != DateTime.MinValue)
                                    { retTvv.Vencimento = tabela[0].VencimentoFim.AddMonths(1); }
                                    if (taxa != null && !taxa.Embutido)
                                    { retTvv.ValorBancario = taxa.ValorEmbutido; }

                                    if (contrato.CobrarTaxaAssociativa)
                                    {
                                        estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                                        if (estipulanteTaxa != null)
                                        { retTvv.ValorSindicalizacao = estipulanteTaxa.Valor; }
                                    }

                                    lista.Add(retTvv);
                                    //taxa = null;
                                }
                                else
                                    cob.Valor += retTvv.ValorBancario;
                            }
                        }
                        else
                            cob.Valor += retTvv.ValorBancario;

                        #endregion

                        #region data de admissao do contrato

                        //Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                        else { continue; }
                        #endregion

                        #endregion

                        doc.Append(Environment.NewLine);

                        EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº do Registro - 04.3A
                        EntityBase.AppendPreparedField(ref doc, "P", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de Movimento Remessa - C004

                        //
                        EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                        EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                        EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.

                        cob.NossoNumero = cob.GeraNossoNumero();
                        EntityBase.AppendPreparedField(ref doc, cob.NossoNumero, 20); //Seu Número
                        //EntityBase.AppendPreparedField(ref doc, "0".PadRight(20, '0'), 20); //Seu Número - pos 38

                        EntityBase.AppendPreparedField(ref doc, "1", 1);   // Código da carteira 11
                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Forma de Cadastr. do Título no Banco - ampo não tratado pelo sistema do Banco do Brasil
                        EntityBase.AppendPreparedField(ref doc, " ", 1);   // Tipo de documento - campo não tratado pelo sistema do Banco do Brasil

                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Identificação da Emissão do Bloqueto: 1 banco emite, 0 nos emitimos C009 - 17.3P faltou: definir
                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Identificação da Destibuição do Bloqueto: 1 banco distribui, 0 nos distribuimos C008 - C010 - 18.3P faltou: definir
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadRight(15, ' '), 15); // Número do Documento de Cobrança
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyyyy"), 8);  // Data de vencimento - pos 78

                        //valor do boleto
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[0].Replace(".","").PadLeft(13, '0'), 13);
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[1].PadLeft(2, '0'), 2);


                        EntityBase.AppendPreparedField(ref doc, "00000", 5);    //Agência Encarregada da Cobrança
                        EntityBase.AppendPreparedField(ref doc, " ", 1);        //Dígito Verificador da Agência

                        EntityBase.AppendPreparedField(ref doc, "02", 2);        //Espécie do Título - posicao 107
                        EntityBase.AppendPreparedField(ref doc, "A", 1);        //dentific. de Título Aceito/Não Aceito 
                        EntityBase.AppendPreparedField(ref doc, cob.DataCriacao.ToString("ddMMyyyy"), 8);  // Data de emissão - pos 110
                        EntityBase.AppendPreparedField(ref doc, "1", 1);        //Código do Juros de Mora faltou: definir '1'=Valor por Dia '2'=Taxa Mensal '3'=Isento
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.AddDays(1).ToString("ddMMyyyy"), 8);  // Data do Juros de Mora - pos 119
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Juros de Mora por Dia/Taxa faltou: definir

                        EntityBase.AppendPreparedField(ref doc, "0", 1);        //Código do Desconto 1 - pos 142
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8); //Data do Desconto 1  - pos 143
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Desconto 1 Valor/Percentual a ser Concedido 
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do IOF a ser Recolhido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do Abatimento
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(25, '0'), 25); //Identificação do Título na Empresa - pos 196

                        EntityBase.AppendPreparedField(ref doc, "3", 1); //Código para Protesto - 1' – Protestar dias corridos, '2' – Protestar dias úteis, e '3' – Não protestar. faltou: definir - pos 221
                        EntityBase.AppendPreparedField(ref doc, "00", 2); //Número de Dias para Protesto - Preencher de acordo com o código informado na posição 221. Para código '1' – é possível, de 6 a 29 dias, 35º, 40º, dia corrido. Para código '2' – é possível, 3º, 4º ou 5º dia útil. Para código '3' preencher com Zeros. faltou: definir
                        EntityBase.AppendPreparedField(ref doc, "0", 1); // Código para Baixa/Devolução - não tratado
                        EntityBase.AppendPreparedField(ref doc, "000", 3); // Número de Dias para Baixa/Devolução - não tratado
                        EntityBase.AppendPreparedField(ref doc, "09", 2); // Código da Moeda - g065
                        EntityBase.AppendPreparedField(ref doc, "0000000000", 10); // Nº do Contrato da Operação de Créd.  - não tratado
                        EntityBase.AppendPreparedField(ref doc, " ", 1); //Uso Exclusivo FEBRABAN/CNAB 


                        #endregion

                        #region DETAIL Q ---------------------------------

                        doc.Append(Environment.NewLine);

                        numSequencial++;

                        EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº do Registro - 04.3A
                        EntityBase.AppendPreparedField(ref doc, "Q", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de Movimento Remessa - C004 - pos 16

                        EntityBase.AppendPreparedField(ref doc, "1", 1);           //Tipo e inscrição - C005
                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularCPF.PadLeft(15, '0'), 15); //Número de inscrição - C006 - pos 19

                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 40); //Nome - C013 - pos 34

                        endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                        endereco.Carregar(pm);
                        if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                        if (endereco.Numero == null) endereco.Numero = "";
                        if (string.IsNullOrEmpty(endereco.Complemento))
                        {
                            int tamanhoDisponivel = 40 - (endereco.Numero.Length + 1);

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero);
                        }
                        else
                        {
                            int tamanhoDisponivel = 40 - ((endereco.Numero.Length + 1) + (endereco.Complemento.Length + 1));

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero, ",", endereco.Complemento);
                            else
                                endereco.Logradouro += string.Concat(",", endereco.Complemento);
                        }

                        EntityBase.AppendPreparedField(ref doc, endereco.Logradouro, 40); // endereco - pos 74
                        EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP, 8);
                        EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);

                        EntityBase.AppendPreparedField(ref doc, "0", 1); //Tipo de Inscrição - pos 154
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Número de Inscrição

                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(40, ' '), 40); //Nome do Sacador/Avalista
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(3, '0'), 3); //Cód. Bco. Corresp. na Compensação - não tratado - pos 210
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(20, ' '), 20); //Nosso Nº no Banco Correspondente - não tratado
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(8, ' '), 8); //Uso Exclusivo FEBRABAN/CNAB 

                        #endregion

                        totalRegistrosDetalhe++;
                        valorTotal += cob.Valor;

                        cob.Carteira = Convert.ToInt32(carteira);
                        cob.ArquivoIDUltimoEnvio = -10;
                        pm.Save(cob);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string path = System.Configuration.ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                            System.IO.File.WriteAllText(path + "err_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".txt",
                                ex.Message +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + ex.StackTrace +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + Convert.ToString(cob.ID) +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + i.ToString(),
                                System.Text.Encoding.ASCII);
                        }
                        catch { }
                        continue;
                    }
                }
                #endregion

                #region TRAILLER DE LOTE ---------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "001", 3);
                EntityBase.AppendPreparedField(ref doc, "0001", 4);
                EntityBase.AppendPreparedField(ref doc, "5", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9);
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 2).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 217);

                #endregion

                #region TRAILLER -----------------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "001", 3);
                EntityBase.AppendPreparedField(ref doc, "9999", 4);
                EntityBase.AppendPreparedField(ref doc, "9", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9); //branco
                EntityBase.AppendPreparedField(ref doc, "1".PadLeft(6, '0'), 6); //qtd de lotes
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 4).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, "0".PadLeft(6, '0'), 6); //Qtde de Contas p/ Conc. (Lotes)
                EntityBase.AppendPreparedField(ref doc, " ", 205); //

                #endregion

                if (_pm == null) { pm.Commit(); } //se não está partcipando de uma transacao externa, comita
                return doc.ToString();
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }

        public static String GeraDocumentoCobranca_BB_Test(string cobrancaId, PersistenceManager _pm)
        {
            Int32 numSequencial = 0;

            PersistenceManager pm = _pm;

            Int32 contratoadmQualicorpID = Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]);

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();
            }

            //Cobranca cobprova = new Cobranca(cobrancaId);
            //pm.Load(cobprova);
            //Contrato contratoprova = new Contrato(cobprova.PropostaID);
            //pm.Load(contratoprova);

            //String agCCDV_itau = "";
            String headerCedente = "UBRASP";
            string agencia = "05956", agenciadv = "0";
            string numcontacorr = "40037".PadLeft(12, '0');
            string dvcontacorr = "8";
            string cnpj = "49938327000106"; //

            ////////////////////////////////////////////////// Só haverá cedente ubrasp

            //agCCDV_itau = "0646042606";    //qualicorp
            string numConvenio = Cobranca.ConvenioUBRASP.PadLeft(9, '0'), carteira = "17", variacao = "019";
            //////////////////////////////////////////////////

            try
            {
                String dataAgora = DateTime.Now.ToString("ddMMyy");
                StringBuilder doc = new StringBuilder();

                ////Cobranca.eCarteira carteira = (Cobranca.eCarteira)aras[0].Carteira;

                #region HEADER -------------------------------------------

                EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0000", 4);         //lote de serviço
                EntityBase.AppendPreparedField(ref doc, "0", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "         ", 9);    //brancos
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //tipo de inscrição - 2 para CNPJ, 1 para CPF
                //EntityBase.AppendPreparedField(ref doc, "0".PadLeft(14, '0'), 14);  //CNPJ (ou cpf) //faltou: cnpj
                EntityBase.AppendPreparedField(ref doc, cnpj, 14);          //CNPJ (ou cpf) 
                EntityBase.AppendPreparedField(ref doc, numConvenio, 9);    //Nùmero do convênio de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "0014", 4);         //Cobrança Cedente BB
                EntityBase.AppendPreparedField(ref doc, carteira, 2);       //Número da carteira de cobrança BB - pos.46
                EntityBase.AppendPreparedField(ref doc, variacao, 3);       //Número da variação da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //Campo reservado BB
                EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome da Empresa
                EntityBase.AppendPreparedField(ref doc, "BANCO DO BRASIL S.A.".PadRight(30, ' '), 30); //Nome do Banco
                EntityBase.AppendPreparedField(ref doc, " ", 10);           //Uso Exclusivo FEBRABAN / CNAB
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //Código Remessa / Retorno - 1=remessa, 2=retorno
                EntityBase.AppendPreparedField(ref doc, DateTime.Now.ToString("ddMMyyyy"), 8); //Data de Geração do Arquivo
                EntityBase.AppendPreparedField(ref doc, DateTime.Now.ToString("HHmmss"), 6);   //Hora de Geração do Arquivo

                //posso usar a propriedade versao da classe...
                EntityBase.AppendPreparedField(ref doc, "000000", 6);       //Número Seqüencial do Arquivo

                EntityBase.AppendPreparedField(ref doc, "000", 3);          //Nº da Versão do Layout do Arquivo
                EntityBase.AppendPreparedField(ref doc, "00000", 5);        //Densidade de Gravação do Arquivo
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(20, ' '), 20); //Para Uso Reservado do Banco
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(20, ' '), 20); //Para Uso Reservado da Empresa
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(29, ' '), 29); //Para Uso Reservado da Empresa

                #endregion

                #region HEADER DE LOTE -----------------------------------

                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "R", 1);            //tipo de operação
                EntityBase.AppendPreparedField(ref doc, "01", 2);           //tipo de serviço
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //
                EntityBase.AppendPreparedField(ref doc, "000", 3);          //Nº da Versão do Layout do Lote - nao criticado pelo banco
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //Tipo de Inscrição da Empresa - 2=CNPJ
                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);  //CNPJ (ou cpf) //faltou: cnpj

                EntityBase.AppendPreparedField(ref doc, numConvenio, 9);    //Nùmero do convênio de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "0014", 4);         //Cobrança Cedente BB
                EntityBase.AppendPreparedField(ref doc, carteira, 2);       //Número da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, variacao, 3);       //Número da variação da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //campo que identifica remessa de testes: brancos ou TS
                EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.

                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome da Empresa
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(40, ' '), 40); //Mensagem 1
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(40, ' '), 40); //Mensagem 2
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Número Remessa/Retorno 
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Data de Gravação Remessa/Retorno - pode ser zeros 
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Data do Crédito 
                EntityBase.AppendPreparedField(ref doc, " ", 33);           //Uso Exclusivo FEBRABAN/CNAB

                #endregion

                //Carrega cobrancas envolvidas
                List<string> cobrancaIDs = new List<string>(); 
                cobrancaIDs.Add("2839");
                cobrancaIDs.Add("2811");
                cobrancaIDs.Add("2897");
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela_Optimized(cobrancaIDs, pm); //
                List<Object> cobrancaIds = new List<Object>();
                if (cobrancas == null) { return null; }

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL -------------------------------------------

                Endereco endereco = null;
                //, diaDataSemJuros, result = 0, 
                int totalRegistrosDetalhe = 0;
                String mascara6 = new String('0', 6); //, nossoNumero = "";
                Decimal valorTotal = 0; //, mora = 0, multa = 0;
                //Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                //CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                int i = 0;

                //String[] contratoAdmIds = null;
                System.Collections.Hashtable contratoADM_Criterio = new System.Collections.Hashtable();
                List<string> parametrosProcessados = new List<string>();

                foreach (Cobranca cob in cobrancas)
                {
                    if (parametrosProcessados.Contains(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID))) continue;

                    parametrosProcessados.Add(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID));

                    i++;
                    try
                    {
                        #region DETAIL P ---------------------------------

                        #region

                        #region localiza o critério correspondente

                        contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                        #endregion

                        numSequencial++;

                        cobrancaIds.Add(cob.ID);

                        if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                        {
                            numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                            qtdTotalBoletos = 0;
                            foreach (Cobranca _temp in cobrancas)
                            {
                                if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                                else if (qtdTotalBoletos > 0) { break; }
                            }
                        }

                        numBoletoCorrente++;

                        #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                        retTvv = TabelaValorVencimentoVO.ExisteProposta(cob.PropostaID, lista);
                        if (retTvv == null)
                        {
                            //contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                            tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cob.DataVencimento, pm);
                            if (tabela != null && tabela.Count > 0)
                            {
                                retTvv = TabelaValorVencimentoVO.ExisteTabela(tabela[0].ID, lista);
                                if (retTvv == null)
                                {
                                    taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                                    if (taxa != null && !taxa.Embutido)
                                    {
                                        cob.Valor += taxa.ValorEmbutido;
                                    }

                                    retTvv = new TabelaValorVencimentoVO();
                                    retTvv.PropostaID = contrato.ID;
                                    retTvv.TabelaID = tabela[0].ID;
                                    if (tabela[0].VencimentoFim != DateTime.MinValue)
                                    { retTvv.Vencimento = tabela[0].VencimentoFim.AddMonths(1); }
                                    if (taxa != null && !taxa.Embutido)
                                    { retTvv.ValorBancario = taxa.ValorEmbutido; }

                                    if (contrato.CobrarTaxaAssociativa)
                                    {
                                        estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                                        if (estipulanteTaxa != null)
                                        { retTvv.ValorSindicalizacao = estipulanteTaxa.Valor; }
                                    }

                                    lista.Add(retTvv);
                                    //taxa = null;
                                }
                                else
                                    cob.Valor += retTvv.ValorBancario;
                            }
                        }
                        else
                            cob.Valor += retTvv.ValorBancario;

                        #endregion

                        #region data de admissao do contrato

                        //Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                        else { continue; }
                        #endregion

                        #endregion

                        doc.Append(Environment.NewLine);

                        EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº do Registro - 04.3A
                        EntityBase.AppendPreparedField(ref doc, "P", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de Movimento Remessa - C004

                        //
                        EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                        EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                        EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.

                        cob.NossoNumero = cob.GeraNossoNumero();
                        EntityBase.AppendPreparedField(ref doc, cob.NossoNumero, 20); //Seu Número
                        //EntityBase.AppendPreparedField(ref doc, "0".PadRight(20, '0'), 20); //Seu Número - pos 38

                        EntityBase.AppendPreparedField(ref doc, "1", 1);   // Código da carteira 11
                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Forma de Cadastr. do Título no Banco - ampo não tratado pelo sistema do Banco do Brasil
                        EntityBase.AppendPreparedField(ref doc, " ", 1);   // Tipo de documento - campo não tratado pelo sistema do Banco do Brasil

                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Identificação da Emissão do Bloqueto: 1 banco emite, 0 nos emitimos C009 - 17.3P faltou: definir
                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Identificação da Destibuição do Bloqueto: 1 banco distribui, 0 nos distribuimos C008 - C010 - 18.3P faltou: definir
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadRight(15, ' '), 15); // Número do Documento de Cobrança
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyyyy"), 8);  // Data de vencimento - pos 78

                        //valor do boleto
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[0].Replace(".", "").PadLeft(13, '0'), 13);
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[1].PadLeft(2, '0'), 2);


                        EntityBase.AppendPreparedField(ref doc, "00000", 5);    //Agência Encarregada da Cobrança
                        EntityBase.AppendPreparedField(ref doc, " ", 1);        //Dígito Verificador da Agência

                        EntityBase.AppendPreparedField(ref doc, "02", 2);        //Espécie do Título - posicao 107
                        EntityBase.AppendPreparedField(ref doc, "A", 1);        //dentific. de Título Aceito/Não Aceito 
                        EntityBase.AppendPreparedField(ref doc, cob.DataCriacao.ToString("ddMMyyyy"), 8);  // Data de emissão - pos 110
                        EntityBase.AppendPreparedField(ref doc, "1", 1);        //Código do Juros de Mora faltou: definir '1'=Valor por Dia '2'=Taxa Mensal '3'=Isento
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.AddDays(1).ToString("ddMMyyyy"), 8);  // Data do Juros de Mora - pos 119
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Juros de Mora por Dia/Taxa faltou: definir

                        EntityBase.AppendPreparedField(ref doc, "0", 1);        //Código do Desconto 1 - pos 142
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8); //Data do Desconto 1  - pos 143
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Desconto 1 Valor/Percentual a ser Concedido 
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do IOF a ser Recolhido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do Abatimento
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(25, '0'), 25); //Identificação do Título na Empresa - pos 196

                        EntityBase.AppendPreparedField(ref doc, "3", 1); //Código para Protesto - 1' – Protestar dias corridos, '2' – Protestar dias úteis, e '3' – Não protestar. faltou: definir - pos 221
                        EntityBase.AppendPreparedField(ref doc, "00", 2); //Número de Dias para Protesto - Preencher de acordo com o código informado na posição 221. Para código '1' – é possível, de 6 a 29 dias, 35º, 40º, dia corrido. Para código '2' – é possível, 3º, 4º ou 5º dia útil. Para código '3' preencher com Zeros. faltou: definir
                        EntityBase.AppendPreparedField(ref doc, "0", 1); // Código para Baixa/Devolução - não tratado
                        EntityBase.AppendPreparedField(ref doc, "000", 3); // Número de Dias para Baixa/Devolução - não tratado
                        EntityBase.AppendPreparedField(ref doc, "09", 2); // Código da Moeda - g065
                        EntityBase.AppendPreparedField(ref doc, "0000000000", 10); // Nº do Contrato da Operação de Créd.  - não tratado
                        EntityBase.AppendPreparedField(ref doc, " ", 1); //Uso Exclusivo FEBRABAN/CNAB 


                        #endregion

                        #region DETAIL Q ---------------------------------

                        doc.Append(Environment.NewLine);

                        numSequencial++;

                        EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº do Registro - 04.3A
                        EntityBase.AppendPreparedField(ref doc, "Q", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de Movimento Remessa - C004 - pos 16

                        EntityBase.AppendPreparedField(ref doc, "1", 1);           //Tipo e inscrição - C005
                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularCPF.PadLeft(15, '0'), 15); //Número de inscrição - C006 - pos 19

                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 40); //Nome - C013 - pos 34

                        endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                        endereco.Carregar(pm);
                        if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                        if (endereco.Numero == null) endereco.Numero = "";
                        if (string.IsNullOrEmpty(endereco.Complemento))
                        {
                            int tamanhoDisponivel = 40 - (endereco.Numero.Length + 1);

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero);
                        }
                        else
                        {
                            int tamanhoDisponivel = 40 - ((endereco.Numero.Length + 1) + (endereco.Complemento.Length + 1));

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero, ",", endereco.Complemento);
                            else
                                endereco.Logradouro += string.Concat(",", endereco.Complemento);
                        }

                        EntityBase.AppendPreparedField(ref doc, endereco.Logradouro, 40); // endereco - pos 74
                        EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP, 8);
                        EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);

                        EntityBase.AppendPreparedField(ref doc, "0", 1); //Tipo de Inscrição - pos 154
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Número de Inscrição

                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(40, ' '), 40); //Nome do Sacador/Avalista
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(3, '0'), 3); //Cód. Bco. Corresp. na Compensação - não tratado - pos 210
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(20, ' '), 20); //Nosso Nº no Banco Correspondente - não tratado
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(8, ' '), 8); //Uso Exclusivo FEBRABAN/CNAB 

                        #endregion

                        totalRegistrosDetalhe++;
                        valorTotal += cob.Valor;

                        cob.Carteira = Convert.ToInt32(carteira);
                        cob.ArquivoIDUltimoEnvio = -10;
                        pm.Save(cob);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string path = System.Configuration.ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                            System.IO.File.WriteAllText(path + "err_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".txt",
                                ex.Message +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + ex.StackTrace +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + Convert.ToString(cob.ID) +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + i.ToString(),
                                System.Text.Encoding.ASCII);
                        }
                        catch { }
                        continue;
                    }
                }
                #endregion

                #region TRAILLER DE LOTE ---------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "001", 3);
                EntityBase.AppendPreparedField(ref doc, "0001", 4);
                EntityBase.AppendPreparedField(ref doc, "5", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9);
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 2).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 217);

                #endregion

                #region TRAILLER -----------------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "001", 3);
                EntityBase.AppendPreparedField(ref doc, "9999", 4);
                EntityBase.AppendPreparedField(ref doc, "9", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9); //branco
                EntityBase.AppendPreparedField(ref doc, "1".PadLeft(6, '0'), 6); //qtd de lotes
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 4).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, "0".PadLeft(6, '0'), 6); //Qtde de Contas p/ Conc. (Lotes)
                EntityBase.AppendPreparedField(ref doc, " ", 205); //

                #endregion

                if (_pm == null) { pm.Rollback(); } //se não está partcipando de uma transacao externa, comita
                return doc.ToString();
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }

        /***************/

        public static String GeraDocumentoCobranca_BB17(IList<String> cobrancaIDs, ref String arquivoNome, ref String arquivoConteudo, ref Object arquivoId, ref String arquivoVersao, IList<ArquivoRemessaAgendamento> aras, PersistenceManager _pm)
        {
            if (cobrancaIDs == null) { return null; }

            Int32 numSequencial = 0;

            PersistenceManager pm = _pm;

            Int32 contratoadmQualicorpID = Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]);

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();
            }

            Cobranca cobprova = new Cobranca(cobrancaIDs[0]);
            pm.Load(cobprova);
            Contrato contratoprova = new Contrato(cobprova.PropostaID);
            pm.Load(contratoprova);

            //String agCCDV_itau = "";
            String headerCedente = "UBRASP";
            string agencia = "05956", agenciadv = "0";
            string numcontacorr = "40037".PadLeft(12, '0');
            string dvcontacorr = "8";
            string cnpj = "49938327000106";

            ////////////////////////////////////////////////// Só haverá cedente ubrasp

            //agCCDV_itau = "0646042606";    //qualicorp
            string numConvenio = Cobranca.Convenio2UBRASP.PadLeft(9, '0'), carteira = "17", variacao = "019";
            //////////////////////////////////////////////////

            try
            {
                if (cobrancaIDs == null) { return null; }

                String dataAgora = DateTime.Now.ToString("ddMMyy");
                String versao = "";
                StringBuilder doc = new StringBuilder();

                ////Cobranca.eCarteira carteira = (Cobranca.eCarteira)aras[0].Carteira;

                if (aras == null)
                    arquivoNome = "arquivo.dat"; // 
                else
                    arquivoNome = String.Concat(aras[0].ArquivoNome, "_", DateTime.Now.ToString("yyyyMMddHHmmss"), ".dat");  //String.Concat(aras[0].ArquivoNomeInstance, "_", DateTime.Now.ToString("yyyyMMddHHmmss"), ".dat");

                #region HEADER -------------------------------------------

                EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0000", 4);         //lote de serviço
                EntityBase.AppendPreparedField(ref doc, "0", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "         ", 9);    //brancos
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //tipo de inscrição - 2 para CNPJ, 1 para CPF
                //EntityBase.AppendPreparedField(ref doc, "0".PadLeft(14, '0'), 14);  //CNPJ (ou cpf) //faltou: cnpj
                EntityBase.AppendPreparedField(ref doc, cnpj, 14);          //CNPJ (ou cpf) 

                //EntityBase.AppendPreparedField(ref doc, numConvenio, 9);    //Nùmero do convênio de cobrança BB
                //EntityBase.AppendPreparedField(ref doc, "0014", 4);         //Cobrança Cedente BB
                //EntityBase.AppendPreparedField(ref doc, carteira, 2);       //Número da carteira de cobrança BB
                //EntityBase.AppendPreparedField(ref doc, variacao, 3);       //Número da variação da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "002896830001417019", 18);

                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //Campo reservado BB
                EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome da Empresa
                EntityBase.AppendPreparedField(ref doc, "BANCO DO BRASIL S.A.".PadRight(30, ' '), 30); //Nome do Banco
                EntityBase.AppendPreparedField(ref doc, " ", 10);           //Uso Exclusivo FEBRABAN / CNAB
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //Código Remessa / Retorno - 1=remessa, 2=retorno
                EntityBase.AppendPreparedField(ref doc, DateTime.Now.ToString("ddMMyyyy"), 8); //Data de Geração do Arquivo
                EntityBase.AppendPreparedField(ref doc, DateTime.Now.ToString("HHmmss"), 6);   //Hora de Geração do Arquivo

                //posso usar a propriedade versao da classe...
                EntityBase.AppendPreparedField(ref doc, "000000", 6);       //Número Seqüencial do Arquivo

                EntityBase.AppendPreparedField(ref doc, "000", 3);          //Nº da Versão do Layout do Arquivo
                EntityBase.AppendPreparedField(ref doc, "00000", 5);        //Densidade de Gravação do Arquivo
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(20, ' '), 20); //Para Uso Reservado do Banco
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(20, ' '), 20); //Para Uso Reservado da Empresa
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(29, ' '), 29); //Para Uso Reservado da Empresa

                ArquivoCobrancaUnibanco arquivo = new ArquivoCobrancaUnibanco();
                arquivo.DataCriacao = DateTime.Now;
                versao = ArquivoCobrancaUnibanco.ObtemProximaVersao(pm).ToString().PadLeft(3, '0');
                arquivo.Versao = Convert.ToInt32(versao);
                arquivo.AgendamentoID = aras[0].ID; // ara.ID; 

                ArquivoRemessaCriterio arqCriterio = null; //new ArquivoRemessaCriterio(ara.CriterioID);
                //pm.Load(arqCriterio);

                //arquivoVersao = versao.PadRight(3, '0');
                //EntityBase.AppendPreparedField(ref doc, versao, 3);     //019
                //EntityBase.AppendPreparedField(ref doc, " ".PadLeft(200, ' '), 200); //20
                //EntityBase.AppendPreparedField(ref doc, "000001", 6);   //021

                #endregion

                #region HEADER DE LOTE -----------------------------------

                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "R", 1);            //tipo de operação
                EntityBase.AppendPreparedField(ref doc, "01", 2);           //tipo de serviço
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //
                EntityBase.AppendPreparedField(ref doc, "000", 3);          //Nº da Versão do Layout do Lote - nao criticado pelo banco
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //Tipo de Inscrição da Empresa - 2=CNPJ
                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);  //CNPJ (ou cpf) //faltou: cnpj

                //EntityBase.AppendPreparedField(ref doc, numConvenio, 9);    //Nùmero do convênio de cobrança BB
                //EntityBase.AppendPreparedField(ref doc, "0014", 4);         //Cobrança Cedente BB
                //EntityBase.AppendPreparedField(ref doc, carteira, 2);       //Número da carteira de cobrança BB
                //EntityBase.AppendPreparedField(ref doc, variacao, 3);       //Número da variação da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "002896830001417019", 18);

                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //campo que identifica remessa de testes: brancos ou TS
                EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.

                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome da Empresa
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(40, ' '), 40); //Mensagem 1
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(40, ' '), 40); //Mensagem 2
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Número Remessa/Retorno 
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Data de Gravação Remessa/Retorno - pode ser zeros 
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Data do Crédito 
                EntityBase.AppendPreparedField(ref doc, " ", 33);           //Uso Exclusivo FEBRABAN/CNAB

                #endregion

                //Carrega cobrancas envolvidas
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela_Optimized(cobrancaIDs, pm); //
                List<Object> cobrancaIds = new List<Object>();
                if (cobrancas == null) { return null; }

                arquivo.MesReferencia = cobrancas[0].DataVencimento.Month;
                arquivo.AnoReferencia = cobrancas[0].DataVencimento.Year;
                arquivo.OperadoraID = cobrancas[0].OperadoraID;
                arquivo.Nome = arquivoNome;
                arquivo.QtdCobrancas = cobrancaIDs.Count;
                pm.Save(arquivo); ////////////////////////////////////////////////////////////////////////
                arquivoId = arquivo.ID;

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL -------------------------------------------

                Endereco endereco = null;
                //, diaDataSemJuros, result = 0, 
                int totalRegistrosDetalhe = 0;
                String mascara6 = new String('0', 6); //, nossoNumero = "";
                Decimal valorTotal = 0; //, mora = 0, multa = 0;
                //Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                //CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                int i = 0;

                //String[] contratoAdmIds = null;
                System.Collections.Hashtable contratoADM_Criterio = new System.Collections.Hashtable();
                List<string> parametrosProcessados = new List<string>();
                Cobranca _cob = null;

                foreach (Cobranca cob in cobrancas)
                {
                    if (parametrosProcessados.Contains(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID))) continue;

                    parametrosProcessados.Add(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID));

                    if (cob.ID == null) { continue; }

                    _cob = null;
                    _cob = Cobranca.CarregarEnviadasPor(cob.PropostaID, cob.DataVencimento, (int)Cobranca.eTipo.Normal, pm);
                    if (_cob != null) { continue; }

                    i++;
                    try
                    {
                        #region DETAIL P ---------------------------------

                        #region

                        #region localiza o critério correspondente

                        contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                        //if (contratoADM_Criterio.Contains(Convert.ToString(contrato.ContratoADMID)))
                        //{
                        //    arqCriterio = (ArquivoRemessaCriterio)contratoADM_Criterio[Convert.ToString(contrato.ContratoADMID)];
                        //}
                        //else
                        //{
                        //    foreach (ArquivoRemessaAgendamento ara in aras)
                        //    {
                        //        if (cob.Criterio != null) { break; }

                        //        contratoAdmIds = ArquivoRemessaCriterio.CarregarContratoAdmIds(ara.CriterioID, pm); /////

                        //        foreach (String contratoAdmId in contratoAdmIds)
                        //        {
                        //            if (Convert.ToString(contrato.ContratoADMID).Equals(contratoAdmId))
                        //            {
                        //                ArquivoRemessaCriterio criterio = new ArquivoRemessaCriterio(ara.CriterioID);
                        //                pm.Load(criterio);
                        //                arqCriterio = criterio;

                        //                try
                        //                {
                        //                    contratoADM_Criterio.Add(contratoAdmId, criterio);
                        //                }
                        //                catch //(Exception ex)
                        //                {
                        //                    continue;
                        //                }
                        //                break;
                        //            }
                        //        }
                        //    }
                        //}
                        #endregion

                        numSequencial++;

                        cobrancaIds.Add(cob.ID);

                        if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                        {
                            numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                            qtdTotalBoletos = 0;
                            foreach (Cobranca _temp in cobrancas)
                            {
                                if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                                else if (qtdTotalBoletos > 0) { break; }
                            }
                        }

                        numBoletoCorrente++;

                        #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                        retTvv = TabelaValorVencimentoVO.ExisteProposta(cob.PropostaID, lista);
                        if (retTvv == null)
                        {
                            //contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                            tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cob.DataVencimento, pm);
                            if (tabela != null && tabela.Count > 0)
                            {
                                retTvv = TabelaValorVencimentoVO.ExisteTabela(tabela[0].ID, lista);
                                if (retTvv == null)
                                {
                                    taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                                    if (taxa != null && !taxa.Embutido)
                                    {
                                        cob.Valor += taxa.ValorEmbutido;
                                    }

                                    retTvv = new TabelaValorVencimentoVO();
                                    retTvv.PropostaID = contrato.ID;
                                    retTvv.TabelaID = tabela[0].ID;
                                    if (tabela[0].VencimentoFim != DateTime.MinValue)
                                    { retTvv.Vencimento = tabela[0].VencimentoFim.AddMonths(1); }
                                    if (taxa != null && !taxa.Embutido)
                                    { retTvv.ValorBancario = taxa.ValorEmbutido; }

                                    if (contrato.CobrarTaxaAssociativa)
                                    {
                                        estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                                        if (estipulanteTaxa != null)
                                        { retTvv.ValorSindicalizacao = estipulanteTaxa.Valor; }
                                    }

                                    lista.Add(retTvv);
                                    //taxa = null;
                                }
                                else
                                    cob.Valor += retTvv.ValorBancario;
                            }
                        }
                        else
                            cob.Valor += retTvv.ValorBancario;

                        #endregion

                        #region data de admissao do contrato

                        //Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                        else { continue; }
                        #endregion

                        #endregion

                        doc.Append(Environment.NewLine);

                        EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº do Registro - 04.3A
                        EntityBase.AppendPreparedField(ref doc, "P", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de Movimento Remessa - C004

                        //
                        EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                        EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                        EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.

                        cob.Carteira = 17;
                        cob.NossoNumero = cob.GeraNossoNumero();
                        EntityBase.AppendPreparedField(ref doc, cob.GeraNossoNumero(), 20); //Seu Número
                        //EntityBase.AppendPreparedField(ref doc, "0".PadRight(20, '0'), 20); //Seu Número - pos 38

                        EntityBase.AppendPreparedField(ref doc, "1", 1);   // Código da carteira 11
                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Forma de Cadastr. do Título no Banco - ampo não tratado pelo sistema do Banco do Brasil
                        EntityBase.AppendPreparedField(ref doc, " ", 1);   // Tipo de documento - campo não tratado pelo sistema do Banco do Brasil

                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Identificação da Emissão do Bloqueto: 1 banco emite, 0 nos emitimos C009 - 17.3P faltou: definir
                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Identificação da Destibuição do Bloqueto: 1 banco distribui, 0 nos distribuimos C008 - C010 - 18.3P faltou: definir
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadRight(15, ' '), 15); // Número do Documento de Cobrança
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyyyy"), 8);  // Data de vencimento - pos 78

                        //valor do boleto
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[0].Replace(".", "").PadLeft(13, '0'), 13);
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[1].PadLeft(2, '0'), 2);


                        EntityBase.AppendPreparedField(ref doc, "00000", 5);    //Agência Encarregada da Cobrança
                        EntityBase.AppendPreparedField(ref doc, " ", 1);        //Dígito Verificador da Agência

                        EntityBase.AppendPreparedField(ref doc, "02", 2);        //Espécie do Título - posicao 107
                        EntityBase.AppendPreparedField(ref doc, "A", 1);        //dentific. de Título Aceito/Não Aceito 
                        EntityBase.AppendPreparedField(ref doc, cob.DataCriacao.ToString("ddMMyyyy"), 8);  // Data de emissão - pos 110
                        EntityBase.AppendPreparedField(ref doc, "1", 1);        //Código do Juros de Mora faltou: definir '1'=Valor por Dia '2'=Taxa Mensal '3'=Isento
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.AddDays(1).ToString("ddMMyyyy"), 8);  // Data do Juros de Mora - pos 119
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Juros de Mora por Dia/Taxa faltou: definir

                        EntityBase.AppendPreparedField(ref doc, "0", 1);        //Código do Desconto 1 - pos 142
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8); //Data do Desconto 1  - pos 143
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Desconto 1 Valor/Percentual a ser Concedido 
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do IOF a ser Recolhido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do Abatimento
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(25, '0'), 25); //Identificação do Título na Empresa - pos 196

                        EntityBase.AppendPreparedField(ref doc, "3", 1); //Código para Protesto - 1' – Protestar dias corridos, '2' – Protestar dias úteis, e '3' – Não protestar. faltou: definir - pos 221
                        EntityBase.AppendPreparedField(ref doc, "00", 2); //Número de Dias para Protesto - Preencher de acordo com o código informado na posição 221. Para código '1' – é possível, de 6 a 29 dias, 35º, 40º, dia corrido. Para código '2' – é possível, 3º, 4º ou 5º dia útil. Para código '3' preencher com Zeros. faltou: definir
                        EntityBase.AppendPreparedField(ref doc, "0", 1); // Código para Baixa/Devolução - não tratado
                        EntityBase.AppendPreparedField(ref doc, "000", 3); // Número de Dias para Baixa/Devolução - não tratado
                        EntityBase.AppendPreparedField(ref doc, "09", 2); // Código da Moeda - g065
                        EntityBase.AppendPreparedField(ref doc, "0000000000", 10); // Nº do Contrato da Operação de Créd.  - não tratado
                        EntityBase.AppendPreparedField(ref doc, " ", 1); //Uso Exclusivo FEBRABAN/CNAB 


                        #endregion

                        #region DETAIL Q ---------------------------------

                        doc.Append(Environment.NewLine);

                        numSequencial++;

                        EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº do Registro - 04.3A
                        EntityBase.AppendPreparedField(ref doc, "Q", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de Movimento Remessa - C004 - pos 16

                        EntityBase.AppendPreparedField(ref doc, "1", 1);           //Tipo e inscrição - C005
                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularCPF.PadLeft(15, '0'), 15); //Número de inscrição - C006 - pos 19

                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 40); //Nome - C013 - pos 34

                        endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                        endereco.Carregar(pm);
                        if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                        if (endereco.Numero == null) endereco.Numero = "";
                        if (string.IsNullOrEmpty(endereco.Complemento))
                        {
                            int tamanhoDisponivel = 40 - (endereco.Numero.Length + 1);

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero);
                        }
                        else
                        {
                            int tamanhoDisponivel = 40 - ((endereco.Numero.Length + 1) + (endereco.Complemento.Length + 1));

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero, ",", endereco.Complemento);
                            else
                                endereco.Logradouro += string.Concat(",", endereco.Complemento);
                        }

                        EntityBase.AppendPreparedField(ref doc, endereco.Logradouro, 40); // endereco - pos 74
                        EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP, 8);
                        EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);

                        EntityBase.AppendPreparedField(ref doc, "0", 1); //Tipo de Inscrição - pos 154
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Número de Inscrição

                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(40, ' '), 40); //Nome do Sacador/Avalista
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(3, '0'), 3); //Cód. Bco. Corresp. na Compensação - não tratado - pos 210
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(20, ' '), 20); //Nosso Nº no Banco Correspondente - não tratado
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(8, ' '), 8); //Uso Exclusivo FEBRABAN/CNAB 

                        #region Texto data limite - comentado
                        //if (valorDataLimite != null) // && Convert.ToString(valorDataLimite) != "0")
                        //{
                        //    if (Int32.TryParse(Convert.ToString(valorDataLimite), out result))
                        //    {
                        //        if (result < cob.DataVencimento.Day)
                        //        {
                        //            dataLimite = new DateTime(cob.DataVencimento.AddMonths(1).Year,
                        //                cob.DataVencimento.AddMonths(1).Month, result);
                        //        }
                        //        else
                        //        {
                        //            dataLimite = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, result);
                        //        }

                        //        EntityBase.AppendPreparedField(ref doc, dataLimite.ToString("dd/MM/yyyy").PadRight(50, ' '), 50); //data limite calculada
                        //    }
                        //    else
                        //    {
                        //        EntityBase.AppendPreparedField(ref doc, Convert.ToString(valorDataLimite).PadRight(50, ' '), 50);
                        //    }
                        //}
                        //else
                        //    EntityBase.AppendPreparedField(ref doc, " ".PadRight(50, ' '), 50);  //Texto data limite

                        #endregion

                        cob.Carteira = Convert.ToInt32(carteira);
                        cob.ArquivoIDUltimoEnvio = arquivo.ID;
                        cob.DataCriacao = DateTime.Now;
                        pm.Save(cob); /////////////////////////////////////////////////////////////////////////////////

                        #endregion

                        totalRegistrosDetalhe++;
                        valorTotal += cob.Valor;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string path = System.Configuration.ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                            System.IO.File.WriteAllText(path + "err_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".txt",
                                ex.Message +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + ex.StackTrace +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + Convert.ToString(cob.ID) +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + i.ToString(),
                                System.Text.Encoding.ASCII);
                        }
                        catch { }
                        continue;
                    }

                    //numSequencial++;
                }
                #endregion

                #region TRAILLER DE LOTE ---------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "001", 3);
                EntityBase.AppendPreparedField(ref doc, "0001", 4);
                EntityBase.AppendPreparedField(ref doc, "5", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9);
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 2).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 217);

                #endregion

                #region TRAILLER -----------------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "001", 3);
                EntityBase.AppendPreparedField(ref doc, "9999", 4);
                EntityBase.AppendPreparedField(ref doc, "9", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9); //branco
                EntityBase.AppendPreparedField(ref doc, "1".PadLeft(6, '0'), 6); //qtd de lotes
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 4).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, "0".PadLeft(6, '0'), 6); //Qtde de Contas p/ Conc. (Lotes)
                EntityBase.AppendPreparedField(ref doc, " ", 205); //

                //EntityBase.AppendPreparedField(ref doc, "", 334); //145
                //EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6); //146
                //EntityBase.AppendPreparedField(ref doc, valorTotal.ToString("N2").Replace(".", "").Replace(",", "").PadLeft(17, '0'), 17); //147
                //EntityBase.AppendPreparedField(ref doc, " ".PadLeft(200, ' '), 200); //148
                //EntityBase.AppendPreparedField(ref doc, String.Format("{0:" + mascara6 + "}", numSequencial), 6); //149

                #endregion

                arquivo.SalvaItens(cobrancaIds, pm);

                if (_pm == null) { pm.Commit(); } //commit apenas se nao estiver participando de uma transacao externa
                arquivoConteudo = doc.ToString();
                return arquivoConteudo;
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }

        public static String GeraDocumentoCobranca_BB17(string cobrancaId, PersistenceManager _pm)
        {
            Int32 numSequencial = 0;

            PersistenceManager pm = _pm;

            Int32 contratoadmQualicorpID = Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]);

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();
            }

            Cobranca cobprova = new Cobranca(cobrancaId);
            pm.Load(cobprova);
            Contrato contratoprova = new Contrato(cobprova.PropostaID);
            pm.Load(contratoprova);

            //String agCCDV_itau = "";
            String headerCedente = "UBRASP";
            string agencia = "05956", agenciadv = "0";
            string numcontacorr = "40037".PadLeft(12, '0');
            string dvcontacorr = "8";
            string cnpj = "49938327000106"; //

            ////////////////////////////////////////////////// Só haverá cedente ubrasp

            //agCCDV_itau = "0646042606";    //qualicorp
            string numConvenio = Cobranca.Convenio2UBRASP.PadLeft(9, '0'), carteira = "17", variacao = "019";
            //////////////////////////////////////////////////

            try
            {
                if (cobrancaId == null) { return null; }

                String dataAgora = DateTime.Now.ToString("ddMMyy");
                StringBuilder doc = new StringBuilder();

                ////Cobranca.eCarteira carteira = (Cobranca.eCarteira)aras[0].Carteira;

                #region HEADER -------------------------------------------

                EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0000", 4);         //lote de serviço
                EntityBase.AppendPreparedField(ref doc, "0", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "         ", 9);    //brancos
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //tipo de inscrição - 2 para CNPJ, 1 para CPF
                //EntityBase.AppendPreparedField(ref doc, "0".PadLeft(14, '0'), 14);  //CNPJ (ou cpf) //faltou: cnpj
                EntityBase.AppendPreparedField(ref doc, cnpj, 14);          //CNPJ (ou cpf) 

                //EntityBase.AppendPreparedField(ref doc, numConvenio, 9);    //Nùmero do convênio de cobrança BB
                //EntityBase.AppendPreparedField(ref doc, "0014", 4);         //Cobrança Cedente BB
                //EntityBase.AppendPreparedField(ref doc, carteira, 2);       //Número da carteira de cobrança BB - pos.46
                //EntityBase.AppendPreparedField(ref doc, variacao, 3);       //Número da variação da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "002896830001417019", 18);

                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //Campo reservado BB
                EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome da Empresa
                EntityBase.AppendPreparedField(ref doc, "BANCO DO BRASIL S.A.".PadRight(30, ' '), 30); //Nome do Banco
                EntityBase.AppendPreparedField(ref doc, " ", 10);           //Uso Exclusivo FEBRABAN / CNAB
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //Código Remessa / Retorno - 1=remessa, 2=retorno
                EntityBase.AppendPreparedField(ref doc, DateTime.Now.ToString("ddMMyyyy"), 8); //Data de Geração do Arquivo
                EntityBase.AppendPreparedField(ref doc, DateTime.Now.ToString("HHmmss"), 6);   //Hora de Geração do Arquivo

                //posso usar a propriedade versao da classe...
                EntityBase.AppendPreparedField(ref doc, "000000", 6);       //Número Seqüencial do Arquivo

                EntityBase.AppendPreparedField(ref doc, "000", 3);          //Nº da Versão do Layout do Arquivo
                EntityBase.AppendPreparedField(ref doc, "00000", 5);        //Densidade de Gravação do Arquivo
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(20, ' '), 20); //Para Uso Reservado do Banco
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(20, ' '), 20); //Para Uso Reservado da Empresa
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(29, ' '), 29); //Para Uso Reservado da Empresa

                #endregion

                #region HEADER DE LOTE -----------------------------------

                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "R", 1);            //tipo de operação
                EntityBase.AppendPreparedField(ref doc, "01", 2);           //tipo de serviço
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //
                EntityBase.AppendPreparedField(ref doc, "000", 3);          //Nº da Versão do Layout do Lote - nao criticado pelo banco
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //Tipo de Inscrição da Empresa - 2=CNPJ
                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);  //CNPJ (ou cpf) //faltou: cnpj

                //EntityBase.AppendPreparedField(ref doc, numConvenio, 9);    //Nùmero do convênio de cobrança BB
                //EntityBase.AppendPreparedField(ref doc, "0014", 4);         //Cobrança Cedente BB
                //EntityBase.AppendPreparedField(ref doc, carteira, 2);       //Número da carteira de cobrança BB
                //EntityBase.AppendPreparedField(ref doc, variacao, 3);       //Número da variação da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "002896830001417019", 18);

                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //campo que identifica remessa de testes: brancos ou TS
                EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.

                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome da Empresa
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(40, ' '), 40); //Mensagem 1
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(40, ' '), 40); //Mensagem 2
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Número Remessa/Retorno 
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Data de Gravação Remessa/Retorno - pode ser zeros 
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Data do Crédito 
                EntityBase.AppendPreparedField(ref doc, " ", 33);           //Uso Exclusivo FEBRABAN/CNAB

                #endregion

                //Carrega cobrancas envolvidas
                List<string> cobrancaIDs = new List<string>(); cobrancaIDs.Add(cobrancaId);
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela_Optimized(cobrancaIDs, pm); //
                List<Object> cobrancaIds = new List<Object>();
                if (cobrancas == null) { return null; }

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL -------------------------------------------

                Endereco endereco = null;
                //, diaDataSemJuros, result = 0, 
                int totalRegistrosDetalhe = 0;
                String mascara6 = new String('0', 6); //, nossoNumero = "";
                Decimal valorTotal = 0; //, mora = 0, multa = 0;
                //Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                //CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                int i = 0;

                //String[] contratoAdmIds = null;
                System.Collections.Hashtable contratoADM_Criterio = new System.Collections.Hashtable();
                List<string> parametrosProcessados = new List<string>();

                foreach (Cobranca cob in cobrancas)
                {
                    if (parametrosProcessados.Contains(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID))) continue;

                    parametrosProcessados.Add(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID));

                    i++;
                    try
                    {
                        #region DETAIL P ---------------------------------

                        #region

                        #region localiza o critério correspondente

                        contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                        #endregion

                        numSequencial++;

                        cobrancaIds.Add(cob.ID);

                        if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                        {
                            numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                            qtdTotalBoletos = 0;
                            foreach (Cobranca _temp in cobrancas)
                            {
                                if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                                else if (qtdTotalBoletos > 0) { break; }
                            }
                        }

                        numBoletoCorrente++;

                        #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                        retTvv = TabelaValorVencimentoVO.ExisteProposta(cob.PropostaID, lista);
                        if (retTvv == null)
                        {
                            //contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                            tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cob.DataVencimento, pm);
                            if (tabela != null && tabela.Count > 0)
                            {
                                retTvv = TabelaValorVencimentoVO.ExisteTabela(tabela[0].ID, lista);
                                if (retTvv == null)
                                {
                                    taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                                    if (taxa != null && !taxa.Embutido)
                                    {
                                        cob.Valor += taxa.ValorEmbutido;
                                    }

                                    retTvv = new TabelaValorVencimentoVO();
                                    retTvv.PropostaID = contrato.ID;
                                    retTvv.TabelaID = tabela[0].ID;
                                    if (tabela[0].VencimentoFim != DateTime.MinValue)
                                    { retTvv.Vencimento = tabela[0].VencimentoFim.AddMonths(1); }
                                    if (taxa != null && !taxa.Embutido)
                                    { retTvv.ValorBancario = taxa.ValorEmbutido; }

                                    if (contrato.CobrarTaxaAssociativa)
                                    {
                                        estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                                        if (estipulanteTaxa != null)
                                        { retTvv.ValorSindicalizacao = estipulanteTaxa.Valor; }
                                    }

                                    lista.Add(retTvv);
                                    //taxa = null;
                                }
                                else
                                    cob.Valor += retTvv.ValorBancario;
                            }
                        }
                        else
                            cob.Valor += retTvv.ValorBancario;

                        #endregion

                        #region data de admissao do contrato

                        //Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                        else { continue; }
                        #endregion

                        #endregion

                        doc.Append(Environment.NewLine);

                        EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº do Registro - 04.3A
                        EntityBase.AppendPreparedField(ref doc, "P", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de Movimento Remessa - C004

                        //
                        EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                        EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                        EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.

                        cob.Carteira = 17;
                        cob.NossoNumero = cob.GeraNossoNumero();
                        EntityBase.AppendPreparedField(ref doc, cob.NossoNumero, 20); //Seu Número
                        //EntityBase.AppendPreparedField(ref doc, "0".PadRight(20, '0'), 20); //Seu Número - pos 38

                        EntityBase.AppendPreparedField(ref doc, "1", 1);   // Código da carteira 11
                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Forma de Cadastr. do Título no Banco - ampo não tratado pelo sistema do Banco do Brasil
                        EntityBase.AppendPreparedField(ref doc, " ", 1);   // Tipo de documento - campo não tratado pelo sistema do Banco do Brasil

                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Identificação da Emissão do Bloqueto: 1 banco emite, 0 nos emitimos C009 - 17.3P faltou: definir
                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Identificação da Destibuição do Bloqueto: 1 banco distribui, 0 nos distribuimos C008 - C010 - 18.3P faltou: definir
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadRight(15, ' '), 15); // Número do Documento de Cobrança
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyyyy"), 8);  // Data de vencimento - pos 78

                        //valor do boleto
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[0].Replace(".", "").PadLeft(13, '0'), 13);
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[1].PadLeft(2, '0'), 2);


                        EntityBase.AppendPreparedField(ref doc, "00000", 5);    //Agência Encarregada da Cobrança
                        EntityBase.AppendPreparedField(ref doc, " ", 1);        //Dígito Verificador da Agência

                        EntityBase.AppendPreparedField(ref doc, "02", 2);        //Espécie do Título - posicao 107
                        EntityBase.AppendPreparedField(ref doc, "A", 1);        //dentific. de Título Aceito/Não Aceito 
                        EntityBase.AppendPreparedField(ref doc, cob.DataCriacao.ToString("ddMMyyyy"), 8);  // Data de emissão - pos 110
                        EntityBase.AppendPreparedField(ref doc, "1", 1);        //Código do Juros de Mora faltou: definir '1'=Valor por Dia '2'=Taxa Mensal '3'=Isento
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.AddDays(1).ToString("ddMMyyyy"), 8);  // Data do Juros de Mora - pos 119
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Juros de Mora por Dia/Taxa faltou: definir

                        EntityBase.AppendPreparedField(ref doc, "0", 1);        //Código do Desconto 1 - pos 142
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8); //Data do Desconto 1  - pos 143
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Desconto 1 Valor/Percentual a ser Concedido 
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do IOF a ser Recolhido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do Abatimento
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(25, '0'), 25); //Identificação do Título na Empresa - pos 196

                        EntityBase.AppendPreparedField(ref doc, "3", 1); //Código para Protesto - 1' – Protestar dias corridos, '2' – Protestar dias úteis, e '3' – Não protestar. faltou: definir - pos 221
                        EntityBase.AppendPreparedField(ref doc, "00", 2); //Número de Dias para Protesto - Preencher de acordo com o código informado na posição 221. Para código '1' – é possível, de 6 a 29 dias, 35º, 40º, dia corrido. Para código '2' – é possível, 3º, 4º ou 5º dia útil. Para código '3' preencher com Zeros. faltou: definir
                        EntityBase.AppendPreparedField(ref doc, "0", 1); // Código para Baixa/Devolução - não tratado
                        EntityBase.AppendPreparedField(ref doc, "000", 3); // Número de Dias para Baixa/Devolução - não tratado
                        EntityBase.AppendPreparedField(ref doc, "09", 2); // Código da Moeda - g065
                        EntityBase.AppendPreparedField(ref doc, "0000000000", 10); // Nº do Contrato da Operação de Créd.  - não tratado
                        EntityBase.AppendPreparedField(ref doc, " ", 1); //Uso Exclusivo FEBRABAN/CNAB 


                        #endregion

                        #region DETAIL Q ---------------------------------

                        doc.Append(Environment.NewLine);

                        numSequencial++;

                        EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº do Registro - 04.3A
                        EntityBase.AppendPreparedField(ref doc, "Q", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de Movimento Remessa - C004 - pos 16

                        EntityBase.AppendPreparedField(ref doc, "1", 1);           //Tipo e inscrição - C005
                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularCPF.PadLeft(15, '0'), 15); //Número de inscrição - C006 - pos 19

                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 40); //Nome - C013 - pos 34

                        endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                        endereco.Carregar(pm);
                        if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                        if (endereco.Numero == null) endereco.Numero = "";
                        if (string.IsNullOrEmpty(endereco.Complemento))
                        {
                            int tamanhoDisponivel = 40 - (endereco.Numero.Length + 1);

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero);
                        }
                        else
                        {
                            int tamanhoDisponivel = 40 - ((endereco.Numero.Length + 1) + (endereco.Complemento.Length + 1));

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero, ",", endereco.Complemento);
                            else
                                endereco.Logradouro += string.Concat(",", endereco.Complemento);
                        }

                        EntityBase.AppendPreparedField(ref doc, endereco.Logradouro, 40); // endereco - pos 74
                        EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP, 8);
                        EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);

                        EntityBase.AppendPreparedField(ref doc, "0", 1); //Tipo de Inscrição - pos 154
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Número de Inscrição

                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(40, ' '), 40); //Nome do Sacador/Avalista
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(3, '0'), 3); //Cód. Bco. Corresp. na Compensação - não tratado - pos 210
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(20, ' '), 20); //Nosso Nº no Banco Correspondente - não tratado
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(8, ' '), 8); //Uso Exclusivo FEBRABAN/CNAB 

                        #endregion

                        totalRegistrosDetalhe++;
                        valorTotal += cob.Valor;

                        cob.Carteira = Convert.ToInt32(carteira);
                        cob.ArquivoIDUltimoEnvio = -10;
                        pm.Save(cob);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string path = System.Configuration.ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                            System.IO.File.WriteAllText(path + "err_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".txt",
                                ex.Message +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + ex.StackTrace +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + Convert.ToString(cob.ID) +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + i.ToString(),
                                System.Text.Encoding.ASCII);
                        }
                        catch { }
                        continue;
                    }
                }
                #endregion

                #region TRAILLER DE LOTE ---------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "001", 3);
                EntityBase.AppendPreparedField(ref doc, "0001", 4);
                EntityBase.AppendPreparedField(ref doc, "5", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9);
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 2).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 217);

                #endregion

                #region TRAILLER -----------------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "001", 3);
                EntityBase.AppendPreparedField(ref doc, "9999", 4);
                EntityBase.AppendPreparedField(ref doc, "9", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9); //branco
                EntityBase.AppendPreparedField(ref doc, "1".PadLeft(6, '0'), 6); //qtd de lotes
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 4).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, "0".PadLeft(6, '0'), 6); //Qtde de Contas p/ Conc. (Lotes)
                EntityBase.AppendPreparedField(ref doc, " ", 205); //

                #endregion

                if (_pm == null) { pm.Commit(); } //se não está partcipando de uma transacao externa, comita
                return doc.ToString();
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }

        public static String GeraDocumentoCobranca_BB17_Test(IList<String> cobrancaIDs, PersistenceManager _pm)
        {
            Int32 numSequencial = 0;

            PersistenceManager pm = _pm;

            Int32 contratoadmQualicorpID = Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]);

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                //pm.BeginTransactionContext();
            }

            //String agCCDV_itau = "";
            String headerCedente = "UBRASP";
            string agencia = "05956", agenciadv = "0";
            string numcontacorr = "40037".PadLeft(12, '0');
            string dvcontacorr = "8";
            string cnpj = "49938327000106"; //

            ////////////////////////////////////////////////// Só haverá cedente ubrasp

            //agCCDV_itau = "0646042606";    //qualicorp
            string numConvenio = Cobranca.Convenio2UBRASP.PadLeft(9, '0'), carteira = "17", variacao = "019";
            //////////////////////////////////////////////////

            try
            {
                String dataAgora = DateTime.Now.ToString("ddMMyy");
                StringBuilder doc = new StringBuilder();

                ////Cobranca.eCarteira carteira = (Cobranca.eCarteira)aras[0].Carteira;

                #region HEADER -------------------------------------------

                EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0000", 4);         //lote de serviço
                EntityBase.AppendPreparedField(ref doc, "0", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "         ", 9);    //brancos
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //tipo de inscrição - 2 para CNPJ, 1 para CPF
                //EntityBase.AppendPreparedField(ref doc, "0".PadLeft(14, '0'), 14);  //CNPJ (ou cpf) //faltou: cnpj
                EntityBase.AppendPreparedField(ref doc, cnpj, 14);          //CNPJ (ou cpf) 

                //EntityBase.AppendPreparedField(ref doc, numConvenio, 9);    //Nùmero do convênio de cobrança BB
                //EntityBase.AppendPreparedField(ref doc, "0014", 4);         //Cobrança Cedente BB
                //EntityBase.AppendPreparedField(ref doc, carteira, 2);       //Número da carteira de cobrança BB - pos.46
                //EntityBase.AppendPreparedField(ref doc, variacao, 3);       //Número da variação da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "002896830001417019", 18);

                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //Campo reservado BB
                EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome da Empresa
                EntityBase.AppendPreparedField(ref doc, "BANCO DO BRASIL S.A.".PadRight(30, ' '), 30); //Nome do Banco
                EntityBase.AppendPreparedField(ref doc, " ", 10);           //Uso Exclusivo FEBRABAN / CNAB
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //Código Remessa / Retorno - 1=remessa, 2=retorno
                EntityBase.AppendPreparedField(ref doc, DateTime.Now.ToString("ddMMyyyy"), 8); //Data de Geração do Arquivo
                EntityBase.AppendPreparedField(ref doc, DateTime.Now.ToString("HHmmss"), 6);   //Hora de Geração do Arquivo

                //posso usar a propriedade versao da classe...
                EntityBase.AppendPreparedField(ref doc, "000000", 6);       //Número Seqüencial do Arquivo

                EntityBase.AppendPreparedField(ref doc, "000", 3);          //Nº da Versão do Layout do Arquivo
                EntityBase.AppendPreparedField(ref doc, "00000", 5);        //Densidade de Gravação do Arquivo
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(20, ' '), 20); //Para Uso Reservado do Banco
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(20, ' '), 20); //Para Uso Reservado da Empresa
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(29, ' '), 29); //Para Uso Reservado da Empresa

                #endregion

                #region HEADER DE LOTE -----------------------------------

                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "R", 1);            //tipo de operação
                EntityBase.AppendPreparedField(ref doc, "01", 2);           //tipo de serviço
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //
                EntityBase.AppendPreparedField(ref doc, "000", 3);          //Nº da Versão do Layout do Lote - nao criticado pelo banco
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //Tipo de Inscrição da Empresa - 2=CNPJ
                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);  //CNPJ (ou cpf) //faltou: cnpj

                //EntityBase.AppendPreparedField(ref doc, numConvenio, 9);    //Nùmero do convênio de cobrança BB
                //EntityBase.AppendPreparedField(ref doc, "0014", 4);         //Cobrança Cedente BB
                //EntityBase.AppendPreparedField(ref doc, carteira, 2);       //Número da carteira de cobrança BB
                //EntityBase.AppendPreparedField(ref doc, variacao, 3);       //Número da variação da carteira de cobrança BB
                EntityBase.AppendPreparedField(ref doc, "002896830001417019", 18);

                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //campo que identifica remessa de testes: brancos ou TS
                EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.

                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome da Empresa
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(40, ' '), 40); //Mensagem 1
                EntityBase.AppendPreparedField(ref doc, " ".PadRight(40, ' '), 40); //Mensagem 2
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Número Remessa/Retorno 
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Data de Gravação Remessa/Retorno - pode ser zeros 
                EntityBase.AppendPreparedField(ref doc, "00000000", 8);            //Data do Crédito 
                EntityBase.AppendPreparedField(ref doc, " ", 33);           //Uso Exclusivo FEBRABAN/CNAB

                #endregion

                //Carrega cobrancas envolvidas
                //List<string> cobrancaIDs = new List<string>();
                //cobrancaIDs.Add("2839");
                //cobrancaIDs.Add("2811");
                //cobrancaIDs.Add("2897");
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela_Optimized(cobrancaIDs, pm); //
                List<Object> cobrancaIds = new List<Object>();
                if (cobrancas == null) { return null; }

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL -------------------------------------------

                Endereco endereco = null;
                //, diaDataSemJuros, result = 0, 
                int totalRegistrosDetalhe = 0;
                String mascara6 = new String('0', 6); //, nossoNumero = "";
                Decimal valorTotal = 0; //, mora = 0, multa = 0;
                //Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                //CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                int i = 0;

                //String[] contratoAdmIds = null;
                System.Collections.Hashtable contratoADM_Criterio = new System.Collections.Hashtable();
                List<string> parametrosProcessados = new List<string>();

                foreach (Cobranca cob in cobrancas)
                {
                    if (parametrosProcessados.Contains(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID))) continue;

                    parametrosProcessados.Add(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID));

                    i++;
                    try
                    {
                        #region DETAIL P ---------------------------------

                        #region

                        #region localiza o critério correspondente

                        contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                        #endregion

                        numSequencial++;

                        cobrancaIds.Add(cob.ID);

                        if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                        {
                            numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                            qtdTotalBoletos = 0;
                            foreach (Cobranca _temp in cobrancas)
                            {
                                if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                                else if (qtdTotalBoletos > 0) { break; }
                            }
                        }

                        numBoletoCorrente++;

                        #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                        retTvv = TabelaValorVencimentoVO.ExisteProposta(cob.PropostaID, lista);
                        if (retTvv == null)
                        {
                            //contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                            tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cob.DataVencimento, pm);
                            if (tabela != null && tabela.Count > 0)
                            {
                                retTvv = TabelaValorVencimentoVO.ExisteTabela(tabela[0].ID, lista);
                                if (retTvv == null)
                                {
                                    taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                                    if (taxa != null && !taxa.Embutido)
                                    {
                                        cob.Valor += taxa.ValorEmbutido;
                                    }

                                    retTvv = new TabelaValorVencimentoVO();
                                    retTvv.PropostaID = contrato.ID;
                                    retTvv.TabelaID = tabela[0].ID;
                                    if (tabela[0].VencimentoFim != DateTime.MinValue)
                                    { retTvv.Vencimento = tabela[0].VencimentoFim.AddMonths(1); }
                                    if (taxa != null && !taxa.Embutido)
                                    { retTvv.ValorBancario = taxa.ValorEmbutido; }

                                    if (contrato.CobrarTaxaAssociativa)
                                    {
                                        estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                                        if (estipulanteTaxa != null)
                                        { retTvv.ValorSindicalizacao = estipulanteTaxa.Valor; }
                                    }

                                    lista.Add(retTvv);
                                    //taxa = null;
                                }
                                else
                                    cob.Valor += retTvv.ValorBancario;
                            }
                        }
                        else
                            cob.Valor += retTvv.ValorBancario;

                        #endregion

                        #region data de admissao do contrato

                        //Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                        else { continue; }
                        #endregion

                        #endregion

                        doc.Append(Environment.NewLine);

                        EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº do Registro - 04.3A
                        EntityBase.AppendPreparedField(ref doc, "P", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de Movimento Remessa - C004

                        //
                        EntityBase.AppendPreparedField(ref doc, agencia, 5);        //Agência mantenedora da conta
                        EntityBase.AppendPreparedField(ref doc, agenciadv, 1);      //Dígito Verificador da Agência
                        EntityBase.AppendPreparedField(ref doc, numcontacorr, 12);  //Número da Conta Corrente
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);    //Dígito Verificador da Conta
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //Dígito Verificador da Ag/Conta - Campo não tratado pelo Banco do Brasil. Informar 'branco' (espaço) OU zero.

                        cob.Carteira = 17;
                        cob.NossoNumero = cob.GeraNossoNumero();
                        EntityBase.AppendPreparedField(ref doc, cob.NossoNumero, 20); //Seu Número
                        //EntityBase.AppendPreparedField(ref doc, "0".PadRight(20, '0'), 20); //Seu Número - pos 38

                        EntityBase.AppendPreparedField(ref doc, "1", 1);   // Código da carteira 11
                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Forma de Cadastr. do Título no Banco - ampo não tratado pelo sistema do Banco do Brasil
                        EntityBase.AppendPreparedField(ref doc, " ", 1);   // Tipo de documento - campo não tratado pelo sistema do Banco do Brasil

                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Identificação da Emissão do Bloqueto: 1 banco emite, 0 nos emitimos C009 - 17.3P faltou: definir
                        EntityBase.AppendPreparedField(ref doc, "0", 1);   // Identificação da Destibuição do Bloqueto: 1 banco distribui, 0 nos distribuimos C008 - C010 - 18.3P faltou: definir
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadRight(15, ' '), 15); // Número do Documento de Cobrança
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyyyy"), 8);  // Data de vencimento - pos 78

                        //valor do boleto
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[0].Replace(".", "").PadLeft(13, '0'), 13);
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[1].PadLeft(2, '0'), 2);


                        EntityBase.AppendPreparedField(ref doc, "00000", 5);    //Agência Encarregada da Cobrança
                        EntityBase.AppendPreparedField(ref doc, " ", 1);        //Dígito Verificador da Agência

                        EntityBase.AppendPreparedField(ref doc, "02", 2);        //Espécie do Título - posicao 107
                        EntityBase.AppendPreparedField(ref doc, "A", 1);        //dentific. de Título Aceito/Não Aceito 
                        EntityBase.AppendPreparedField(ref doc, cob.DataCriacao.ToString("ddMMyyyy"), 8);  // Data de emissão - pos 110
                        EntityBase.AppendPreparedField(ref doc, "1", 1);        //Código do Juros de Mora faltou: definir '1'=Valor por Dia '2'=Taxa Mensal '3'=Isento
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.AddDays(1).ToString("ddMMyyyy"), 8);  // Data do Juros de Mora - pos 119
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Juros de Mora por Dia/Taxa faltou: definir

                        EntityBase.AppendPreparedField(ref doc, "0", 1);        //Código do Desconto 1 - pos 142
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8); //Data do Desconto 1  - pos 143
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Desconto 1 Valor/Percentual a ser Concedido 
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do IOF a ser Recolhido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do Abatimento
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(25, '0'), 25); //Identificação do Título na Empresa - pos 196

                        EntityBase.AppendPreparedField(ref doc, "3", 1); //Código para Protesto - 1' – Protestar dias corridos, '2' – Protestar dias úteis, e '3' – Não protestar. faltou: definir - pos 221
                        EntityBase.AppendPreparedField(ref doc, "00", 2); //Número de Dias para Protesto - Preencher de acordo com o código informado na posição 221. Para código '1' – é possível, de 6 a 29 dias, 35º, 40º, dia corrido. Para código '2' – é possível, 3º, 4º ou 5º dia útil. Para código '3' preencher com Zeros. faltou: definir
                        EntityBase.AppendPreparedField(ref doc, "0", 1); // Código para Baixa/Devolução - não tratado
                        EntityBase.AppendPreparedField(ref doc, "000", 3); // Número de Dias para Baixa/Devolução - não tratado
                        EntityBase.AppendPreparedField(ref doc, "09", 2); // Código da Moeda - g065
                        EntityBase.AppendPreparedField(ref doc, "0000000000", 10); // Nº do Contrato da Operação de Créd.  - não tratado
                        EntityBase.AppendPreparedField(ref doc, " ", 1); //Uso Exclusivo FEBRABAN/CNAB 


                        #endregion

                        #region DETAIL Q ---------------------------------

                        doc.Append(Environment.NewLine);

                        numSequencial++;

                        EntityBase.AppendPreparedField(ref doc, "001", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº do Registro - 04.3A
                        EntityBase.AppendPreparedField(ref doc, "Q", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de Movimento Remessa - C004 - pos 16

                        EntityBase.AppendPreparedField(ref doc, "1", 1);           //Tipo e inscrição - C005
                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularCPF.PadLeft(15, '0'), 15); //Número de inscrição - C006 - pos 19

                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 40); //Nome - C013 - pos 34

                        endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                        endereco.Carregar(pm);
                        if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                        if (endereco.Numero == null) endereco.Numero = "";
                        if (string.IsNullOrEmpty(endereco.Complemento))
                        {
                            int tamanhoDisponivel = 40 - (endereco.Numero.Length + 1);

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero);
                        }
                        else
                        {
                            int tamanhoDisponivel = 40 - ((endereco.Numero.Length + 1) + (endereco.Complemento.Length + 1));

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero, ",", endereco.Complemento);
                            else
                                endereco.Logradouro += string.Concat(",", endereco.Complemento);
                        }

                        EntityBase.AppendPreparedField(ref doc, endereco.Logradouro, 40); // endereco - pos 74
                        EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP, 8);
                        EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);

                        EntityBase.AppendPreparedField(ref doc, "0", 1); //Tipo de Inscrição - pos 154
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Número de Inscrição

                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(40, ' '), 40); //Nome do Sacador/Avalista
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(3, '0'), 3); //Cód. Bco. Corresp. na Compensação - não tratado - pos 210
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(20, ' '), 20); //Nosso Nº no Banco Correspondente - não tratado
                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(8, ' '), 8); //Uso Exclusivo FEBRABAN/CNAB 

                        #endregion

                        totalRegistrosDetalhe++;
                        valorTotal += cob.Valor;

                        cob.Carteira = Convert.ToInt32(carteira);
                        cob.ArquivoIDUltimoEnvio = -10;
                        //pm.Save(cob);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string path = System.Configuration.ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                            System.IO.File.WriteAllText(path + "err_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".txt",
                                ex.Message +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + ex.StackTrace +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + Convert.ToString(cob.ID) +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + i.ToString(),
                                System.Text.Encoding.ASCII);
                        }
                        catch { }
                        continue;
                    }
                }
                #endregion

                #region TRAILLER DE LOTE ---------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "001", 3);
                EntityBase.AppendPreparedField(ref doc, "0001", 4);
                EntityBase.AppendPreparedField(ref doc, "5", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9);
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 2).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 217);

                #endregion

                #region TRAILLER -----------------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "001", 3);
                EntityBase.AppendPreparedField(ref doc, "9999", 4);
                EntityBase.AppendPreparedField(ref doc, "9", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9); //branco
                EntityBase.AppendPreparedField(ref doc, "1".PadLeft(6, '0'), 6); //qtd de lotes
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 4).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, "0".PadLeft(6, '0'), 6); //Qtde de Contas p/ Conc. (Lotes)
                EntityBase.AppendPreparedField(ref doc, " ", 205); //

                #endregion

                if (_pm == null) { pm.Rollback(); } //se não está partcipando de uma transacao externa, comita
                return doc.ToString();
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }

        /***************************************************************************************************************/

        public static List<SumarioArquivoGeradoVO> GeraDocumentoCobranca_SANTANDER(IList<Cobranca> cobrancas, IList<ArquivoRemessaAgendamento> aras)
        {
            List<SumarioArquivoGeradoVO> vos = new List<SumarioArquivoGeradoVO>();

            //separa as operadoras
            List<String> operadoraIDs = new List<String>();
            System.Collections.Hashtable ht = new System.Collections.Hashtable();

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            //foreach (Cobranca cobranca in cobrancas)
            //{
            //    if (!operadoraIDs.Contains(cobranca.OperadoraID.ToString()))
            //        operadoraIDs.Add(cobranca.OperadoraID.ToString());
            //}

            pm.CloseSingleCommandInstance();
            pm.Dispose();

            //processa os arquivos, um para cada operadora.
            //foreach (String operadoraId in operadoraIDs)
            //{
            String arquivoNome = "", arquivoConteudo = "", arquivoVersao = "", operadoraNome = "";
            Object arquivoId = null;
            Int32 qtdCobrancas = 0;

            Operadora operadora = new Operadora(cobrancas[0].OperadoraID); //Operadora operadora = new Operadora(operadoraId);
            operadora.Carregar();
            operadoraNome = operadora.Nome;

            List<String> cobrancaIDs = new List<String>();
            foreach (Cobranca cobranca in cobrancas)
            {
                //if (cobranca.OperadoraID.ToString() != operadoraId) { continue; }
                cobrancaIDs.Add(cobranca.ID.ToString());
            }

            qtdCobrancas = cobrancaIDs.Count;
            /////////////GeraDocumentoCobranca_UNIBANCO2(cobrancaIDs, ref arquivoNome, ref arquivoConteudo, ref arquivoId, ref arquivoVersao, aras, null);
            GeraDocumentoCobranca_SANTANDER(cobrancaIDs, ref arquivoNome, ref arquivoConteudo, ref arquivoId, ref arquivoVersao, aras, null);

            //armazena na colecao para retorno à UI
            SumarioArquivoGeradoVO vo = new SumarioArquivoGeradoVO();
            vo.ArquivoConteudo = arquivoConteudo;
            vo.ArquivoID = arquivoId;
            vo.ArquivoNome = arquivoNome;
            vo.ArquivoVersao = arquivoVersao;
            vo.OperadoraID = cobrancas[0].OperadoraID; //operadoraId;
            vo.OperadoraNome = operadora.Nome;
            vo.QtdCobrancas = qtdCobrancas;
            vos.Add(vo);
            //}

            return vos;
        }

        public static String GeraDocumentoCobranca_SANTANDER(IList<String> cobrancaIDs, ref String arquivoNome, ref String arquivoConteudo, ref Object arquivoId, ref String arquivoVersao, IList<ArquivoRemessaAgendamento> aras, PersistenceManager _pm)
        {
            Int32 numSequencial = 0;

            PersistenceManager pm = _pm;

            Int32 contratoadmQualicorpID = Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]);

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();
            }

            String headerCedente = "UBRASP - UNIAO BRASILEIRA";
            string agencia = "0001"; //, agenciadv = "0";
            string numcontacorr = "13014765";
            string dvcontacorr = "2";
            string cnpj = "49938327000106"; //
            string codTransmissao = "000100001201344";
            //string complemento = "I 52";

            //agCCDV_itau = "0646042606";    //qualicorp
            string numConvenio = "1201344"; // Cobranca.ConvenioUBRASP.PadLeft(9, '0');
            //string carteira = "5"; //, variacao = "019";

            int numeroDoArquivo = 1;
            string[] arquivos = Directory.GetFiles(ConfigurationManager.AppSettings["financialFilePathFISICO"], "*.dat");
            if (arquivos != null && arquivos.Length > 0) numeroDoArquivo = arquivos.Length + 1;
            if (numeroDoArquivo > 999) numeroDoArquivo = 1 + (arquivos.Length - 999);

            ArquivoCobrancaUnibanco arquivo = new ArquivoCobrancaUnibanco();
            arquivo.DataCriacao = DateTime.Now;
            arquivo.Versao = 1;
            arquivo.AgendamentoID = aras[0].ID; 

            try
            {
                String dataAgora = DateTime.Now.ToString("ddMMyyyy");
                StringBuilder doc = new StringBuilder();

                if (aras == null)
                    arquivoNome = "arquivo.dat"; // 
                else
                    arquivoNome = String.Concat(aras[0].ArquivoNomeInstance, "_", DateTime.Now.ToString("yyyyMMddHHmmss"), ".dat");

                #region HEADER -------------------------------------------

                EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0000", 4);         //lote de serviço
                EntityBase.AppendPreparedField(ref doc, "0", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "        ", 8);     //brancos
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //tipo de inscrição - 2 para CNPJ, 1 para CPF

                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);          //CNPJ (ou cpf) 
                EntityBase.AppendPreparedField(ref doc, codTransmissao.PadLeft(15, '0'), 15); //Codigo de transmissao 
                EntityBase.AppendPreparedField(ref doc, " ", 25);           //brancos
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //nome empresa
                EntityBase.AppendPreparedField(ref doc, "Banco Santander".PadRight(30, ' '), 30); //Nome do Banco
                EntityBase.AppendPreparedField(ref doc, " ", 10);           //brancos
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //Código Remessa: 1 = remessa
                EntityBase.AppendPreparedField(ref doc, dataAgora, 8);      //data de geracao do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 6);            //brancos
                EntityBase.AppendPreparedField(ref doc, numeroDoArquivo.ToString().PadLeft(6, '0'), 6); //brancos
                EntityBase.AppendPreparedField(ref doc, "040", 3);          //versão do layout do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 74);           //brancos

                #endregion

                #region HEADER DE LOTE -----------------------------------

                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "R", 1);            //tipo de operação
                EntityBase.AppendPreparedField(ref doc, "01", 2);           //tipo de serviço
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //brancos
                EntityBase.AppendPreparedField(ref doc, "030", 3);          //Nº da Versão do Layout do Lote - nao criticado pelo banco
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //Tipo de Inscrição da Empresa - 2=CNPJ
                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);  //CNPJ (ou cpf) //faltou: cnpj
                EntityBase.AppendPreparedField(ref doc, " ", 20);           //brancos
                EntityBase.AppendPreparedField(ref doc, codTransmissao.PadLeft(15, '0'), 15);//Codigo de transmissao 
                EntityBase.AppendPreparedField(ref doc, " ", 05);           //brancos
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome do beneficiario
                EntityBase.AppendPreparedField(ref doc, " ", 40);           //Mensagem 1
                EntityBase.AppendPreparedField(ref doc, " ", 40);           //Mensagem 2
                EntityBase.AppendPreparedField(ref doc, "0001".PadLeft(8, '0'), 8); //Número remessa/retorno
                EntityBase.AppendPreparedField(ref doc, dataAgora, 8);      //data de gravaçao do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 41);           //brancos

                #endregion

                //Carrega cobrancas envolvidas
                IList<Cobranca> _cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela_Optimized(cobrancaIDs, pm);
                if (_cobrancas == null) { return null; }
                List<Object> cobrancaIds = new List<Object>();

                IList<Cobranca> cobrancas = new List<Cobranca>();
                foreach (var c in _cobrancas)
                {
                    //if (Convert.ToString(c.BeneficiarioId) == "39453" || Convert.ToString(c.BeneficiarioId) == "12971" || Convert.ToString(c.BeneficiarioId) == "13366") continue;
                    cobrancas.Add(c);
                }

                if (cobrancas == null || cobrancas.Count == 0) { return null; }

                arquivo.MesReferencia = cobrancas[0].DataVencimento.Month;
                arquivo.AnoReferencia = cobrancas[0].DataVencimento.Year;
                arquivo.OperadoraID = cobrancas[0].OperadoraID;
                arquivo.Nome = arquivoNome;
                arquivo.QtdCobrancas = cobrancas.Count; // cobrancaIDs.Count;
                pm.Save(arquivo); ////////////////////////////////////////////////////////////////////////
                arquivoId = arquivo.ID;

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL -------------------------------------------

                Endereco endereco = null;
                //, diaDataSemJuros, result = 0, 
                int totalRegistrosDetalhe = 0;
                String mascara6 = new String('0', 6); //, nossoNumero = "";
                Decimal valorTotal = 0; //, mora = 0, multa = 0;
                //Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                //CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                int i = 0;

                System.Collections.Hashtable contratoADM_Criterio = new System.Collections.Hashtable();
                List<string> parametrosProcessados = new List<string>();

                foreach (Cobranca cob in cobrancas)
                {
                    if (parametrosProcessados.Contains(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID))) continue;

                    parametrosProcessados.Add(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID));

                    i++;
                    try
                    {
                        #region DETAIL P ---------------------------------

                        #region

                        #region localiza o critério correspondente

                        contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                        #endregion

                        numSequencial++;

                        cobrancaIds.Add(cob.ID);

                        if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                        {
                            numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                            qtdTotalBoletos = 0;
                            foreach (Cobranca _temp in cobrancas)
                            {
                                if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                                else if (qtdTotalBoletos > 0) { break; }
                            }
                        }

                        numBoletoCorrente++;

                        #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                        retTvv = TabelaValorVencimentoVO.ExisteProposta(cob.PropostaID, lista);
                        if (retTvv == null)
                        {
                            //contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                            tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cob.DataVencimento, pm);
                            if (tabela != null && tabela.Count > 0)
                            {
                                retTvv = TabelaValorVencimentoVO.ExisteTabela(tabela[0].ID, lista);
                                if (retTvv == null)
                                {
                                    taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                                    if (taxa != null && !taxa.Embutido)
                                    {
                                        cob.Valor += taxa.ValorEmbutido;
                                    }

                                    retTvv = new TabelaValorVencimentoVO();
                                    retTvv.PropostaID = contrato.ID;
                                    retTvv.TabelaID = tabela[0].ID;
                                    if (tabela[0].VencimentoFim != DateTime.MinValue)
                                    { retTvv.Vencimento = tabela[0].VencimentoFim.AddMonths(1); }
                                    if (taxa != null && !taxa.Embutido)
                                    { retTvv.ValorBancario = taxa.ValorEmbutido; }

                                    if (contrato.CobrarTaxaAssociativa)
                                    {
                                        estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                                        if (estipulanteTaxa != null)
                                        { retTvv.ValorSindicalizacao = estipulanteTaxa.Valor; }
                                    }

                                    lista.Add(retTvv);
                                    //taxa = null;
                                }
                                else
                                    cob.Valor += retTvv.ValorBancario;
                            }
                        }
                        else
                            cob.Valor += retTvv.ValorBancario;

                        #endregion

                        #region data de admissao do contrato

                        //Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                        else { continue; }
                        #endregion

                        #endregion

                        doc.Append(Environment.NewLine);

                        EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Numero do lote remessa
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº seqüencial do registro de lote
                        EntityBase.AppendPreparedField(ref doc, "P", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de movimento remessa (pag3) - nota 14 (pag.14)
                        EntityBase.AppendPreparedField(ref doc, agencia, 4);        //Agência do Destinatária FIDC
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Dígito da Agência do Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, numcontacorr.PadLeft(9, '0'), 9); //Número da conta corrente
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);            //Dígito da conta corrente (definido pela homologação)
                        EntityBase.AppendPreparedField(ref doc, numcontacorr.PadLeft(9, '0'), 9); //Conta cobrança Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1); //Dígito da conta cobrança Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, "  ", 2);           //branco
                        cob.NossoNumero = cob.GeraNossoNumeroSantander();
                        EntityBase.AppendPreparedField(ref doc, cob.NossoNumero, 13); //Identificação do título no Banco
                        EntityBase.AppendPreparedField(ref doc, "5", 1);            //Tipo de cobrança 
                        EntityBase.AppendPreparedField(ref doc, "1", 1);            //Forma de Cadastramento - 1 para cobranca registrada
                        EntityBase.AppendPreparedField(ref doc, "1", 1);            //Tipo de documento - 1- Tradicional , 2- Escritural
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(15, '0'), 15); //Seu número
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyyyy"), 8);  // Data de vencimento - pos 78

                        //valor do boleto
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[0].Replace(".", "").PadLeft(13, '0'), 13);
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[1].PadLeft(2, '0'), 2);

                        EntityBase.AppendPreparedField(ref doc, "0000", 4);         //Agência encarregada da cobrança
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Dígito da Agência do Beneficiário
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "02", 2);           //Espécie do título Nota 20 pag.15 ???
                        EntityBase.AppendPreparedField(ref doc, "N", 1);            //Identif. de título Aceito/Não Aceito
                        EntityBase.AppendPreparedField(ref doc, cob.DataCriacao.ToString("ddMMyyyy"), 8); //Data da emissão do título
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //Código do juros de mora - nota 21 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8); //Data do juros de mora - nota 22
                        EntityBase.AppendPreparedField(ref doc, "0".PadRight(15, '0'), 15); //Valor da mora/dia ou Taxa mensal - nota 18 pag.15
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Código do desconto 1 - nota 23 pag.16
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8);     //Data de desconto 1 - nota 23 pag.16
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor ou Percentual do desconto concedido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do IOF a ser recolhido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do abatimento

                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(25, '0'), 25); //Identificação do título na empresa
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Código para protesto - nota 25 pag.16 - 0=nao protestar
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Número de dias para protesto
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //Código para Baixa/Devolução - nota 26 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Número de dias para Baixa/Devolução
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Código da moeda - nota 27 - pag.16
                        EntityBase.AppendPreparedField(ref doc, " ".PadRight(11, ' '), 11); //branco

                        //
                        #endregion

                        #region DETAIL Q ---------------------------------

                        doc.Append(Environment.NewLine);

                        numSequencial++;

                        EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Numero do lote remessa
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº seqüencial do registro no lote - nota 11 - pg.14
                        EntityBase.AppendPreparedField(ref doc, "Q", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de movimento remessa - nota 14 - pag 14

                        EntityBase.AppendPreparedField(ref doc, "1", 1);           //Tipo de inscrição do Pagador - nota 29 - pag.17 - 1 = cpf
                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularCPF.PadLeft(15, '0'), 15); //Número de inscrição do Pagador - pag. 4

                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 40); //Nome Pagador

                        endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                        endereco.Carregar(pm);
                        if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                        if (endereco.Numero == null) endereco.Numero = "";
                        if (string.IsNullOrEmpty(endereco.Complemento))
                        {
                            int tamanhoDisponivel = 40 - (endereco.Numero.Length + 1);

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero);
                        }
                        else
                        {
                            int tamanhoDisponivel = 40 - ((endereco.Numero.Length + 1) + (endereco.Complemento.Length + 1));

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero, ",", endereco.Complemento);
                            else
                                endereco.Logradouro += string.Concat(",", endereco.Complemento);
                        }

                        if (string.IsNullOrEmpty(endereco.CEP)) { endereco.CEP = "00000000"; } else { endereco.CEP = endereco.CEP.Replace("-", ""); }
                        EntityBase.AppendPreparedField(ref doc, endereco.Logradouro, 40); // 
                        EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP, 8);
                        EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);

                        //if(string.IsNullOrEmpty(cob.ContratoTitularCPF) || cob.ContratoTitularCPF == "00000000000")
                        EntityBase.AppendPreparedField(ref doc, "0", 1); //Tipo de inscrição Sacador/avalista - nota 29 - pag.17
                        //else
                        //    EntityBase.AppendPreparedField(ref doc, "1", 1); //Tipo de inscrição Sacador/avalista - nota 29 - pag.17

                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Nº de inscrição Sacador/avalista

                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(40, ' '), 40); //Nome do Sacador/Avalista

                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Identificador de carne - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Sequencial da Parcela ou número inicial da parcela - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Quantidade total de parcelas - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Número do plano - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, " ".PadRight(19, ' '), 19); //Brancos

                        #endregion

                        totalRegistrosDetalhe++;
                        valorTotal += cob.Valor;

                        cob.ArquivoIDUltimoEnvio = arquivo.ID;
                        cob.Carteira = 101;

                        pm.Save(cob);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string path = System.Configuration.ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                            System.IO.File.WriteAllText(path + "err_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".txt",
                                ex.Message +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + ex.StackTrace +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + Convert.ToString(cob.ID) +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + i.ToString(),
                                System.Text.Encoding.ASCII);
                        }
                        catch { }
                        continue;
                    }
                }
                #endregion

                #region TRAILLER DE LOTE ---------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "033", 3);
                EntityBase.AppendPreparedField(ref doc, "0001", 4);
                EntityBase.AppendPreparedField(ref doc, "5", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9);
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 2).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 217);

                #endregion

                #region TRAILLER -----------------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "033", 3);
                EntityBase.AppendPreparedField(ref doc, "9999", 4); //9999 - Numero do lote remessa
                EntityBase.AppendPreparedField(ref doc, "9", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9); //branco
                EntityBase.AppendPreparedField(ref doc, "1".PadLeft(6, '0'), 6); //qtd de lotes - registros tipo 1
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 4).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 211); //

                #endregion

                arquivo.SalvaItens(cobrancaIds, pm);

                if (_pm == null) { pm.Commit(); } //se não está partcipando de uma transacao externa, comita //pm.Rollback();
                arquivoConteudo = doc.ToString();
                return arquivoConteudo;
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }

        public static String GeraDocumentoCobranca_SANTANDER(string cobrancaId, PersistenceManager _pm)
        {
            Int32 numSequencial = 0;

            PersistenceManager pm = _pm;

            Int32 contratoadmQualicorpID = Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]);

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();
            }

            //String agCCDV_itau = "";
            String headerCedente = "UBRASP - UNIAO BRASILEIRA";
            string agencia = "0001"; //, agenciadv = "0";
            string numcontacorr = "13014765";
            string dvcontacorr = "2";
            string cnpj = "49938327000106"; //
            string codTransmissao = "000100001201344";
            //string complemento = "I 52";

            //agCCDV_itau = "0646042606";    //qualicorp
            string numConvenio = "1201344"; // Cobranca.ConvenioUBRASP.PadLeft(9, '0');
            //string carteira = "5"; //, variacao = "019";

            int numeroDoArquivo = 1;
            string[] arquivos = Directory.GetFiles(ConfigurationManager.AppSettings["financialFilePathFISICO"], "*.dat");
            if (arquivos != null && arquivos.Length > 0) numeroDoArquivo = arquivos.Length + 1;
            if (numeroDoArquivo > 999) numeroDoArquivo = 1 + (arquivos.Length - 999);
            File.WriteAllText(
                ConfigurationManager.AppSettings["financialFilePathFISICO"] + cobrancaId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".dat",
                "conteudo");

            //numeroDoArquivo = 5;


            try
            {
                if (cobrancaId == null) { return null; }

                String dataAgora = DateTime.Now.ToString("ddMMyyyy");
                StringBuilder doc = new StringBuilder();

                ////Cobranca.eCarteira carteira = (Cobranca.eCarteira)aras[0].Carteira;

                #region HEADER -------------------------------------------

                EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0000", 4);         //lote de serviço
                EntityBase.AppendPreparedField(ref doc, "0", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "        ", 8);     //brancos
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //tipo de inscrição - 2 para CNPJ, 1 para CPF

                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15,'0'), 15);          //CNPJ (ou cpf) 
                EntityBase.AppendPreparedField(ref doc, codTransmissao.PadLeft(15, '0'), 15); //Codigo de transmissao 
                EntityBase.AppendPreparedField(ref doc, " ", 25);           //brancos
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //nome empresa
                EntityBase.AppendPreparedField(ref doc, "Banco Santander".PadRight(30, ' '), 30); //Nome do Banco
                EntityBase.AppendPreparedField(ref doc, " ", 10);           //brancos
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //Código Remessa: 1 = remessa
                EntityBase.AppendPreparedField(ref doc, dataAgora, 8);      //data de geracao do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 6);            //brancos
                EntityBase.AppendPreparedField(ref doc, numeroDoArquivo.ToString().PadLeft(6, '0'), 6); //brancos
                EntityBase.AppendPreparedField(ref doc, "040", 3);          //versão do layout do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 74);           //brancos

                #endregion

                #region HEADER DE LOTE -----------------------------------

                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "R", 1);            //tipo de operação
                EntityBase.AppendPreparedField(ref doc, "01", 2);           //tipo de serviço
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //brancos
                EntityBase.AppendPreparedField(ref doc, "030", 3);          //Nº da Versão do Layout do Lote - nao criticado pelo banco
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //Tipo de Inscrição da Empresa - 2=CNPJ
                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);  //CNPJ (ou cpf) //faltou: cnpj
                EntityBase.AppendPreparedField(ref doc, " ", 20);           //brancos
                EntityBase.AppendPreparedField(ref doc, codTransmissao.PadLeft(15,'0'), 15);//Codigo de transmissao 
                EntityBase.AppendPreparedField(ref doc, " ", 05);           //brancos
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome do beneficiario
                EntityBase.AppendPreparedField(ref doc, " ", 40);           //Mensagem 1
                EntityBase.AppendPreparedField(ref doc, " ", 40);           //Mensagem 2
                EntityBase.AppendPreparedField(ref doc, "0001".PadLeft(8,'0'), 8); //Número remessa/retorno
                EntityBase.AppendPreparedField(ref doc, dataAgora, 8);      //data de gravaçao do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 41);           //brancos

                #endregion

                //Carrega cobrancas envolvidas
                List<string> cobrancaIDs = new List<string>(); cobrancaIDs.Add(cobrancaId);
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela_Optimized(cobrancaIDs, pm); //
                List<Object> cobrancaIds = new List<Object>();
                if (cobrancas == null) { return null; }

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL -------------------------------------------

                Endereco endereco = null;
                //, diaDataSemJuros, result = 0, 
                int totalRegistrosDetalhe = 0;
                String mascara6 = new String('0', 6); //, nossoNumero = "";
                Decimal valorTotal = 0; //, mora = 0, multa = 0;
                //Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                //CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                int i = 0;

                System.Collections.Hashtable contratoADM_Criterio = new System.Collections.Hashtable();
                List<string> parametrosProcessados = new List<string>();

                foreach (Cobranca cob in cobrancas)
                {
                    if (parametrosProcessados.Contains(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID))) continue;

                    parametrosProcessados.Add(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID));

                    i++;
                    try
                    {
                        #region DETAIL P ---------------------------------

                        #region

                        #region localiza o critério correspondente

                        contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                        #endregion

                        numSequencial++;

                        cobrancaIds.Add(cob.ID);

                        if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                        {
                            numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                            qtdTotalBoletos = 0;
                            foreach (Cobranca _temp in cobrancas)
                            {
                                if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                                else if (qtdTotalBoletos > 0) { break; }
                            }
                        }

                        numBoletoCorrente++;

                        #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                        retTvv = TabelaValorVencimentoVO.ExisteProposta(cob.PropostaID, lista);
                        if (retTvv == null)
                        {
                            //contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                            tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cob.DataVencimento, pm);
                            if (tabela != null && tabela.Count > 0)
                            {
                                retTvv = TabelaValorVencimentoVO.ExisteTabela(tabela[0].ID, lista);
                                if (retTvv == null)
                                {
                                    taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                                    if (taxa != null && !taxa.Embutido)
                                    {
                                        cob.Valor += taxa.ValorEmbutido;
                                    }

                                    retTvv = new TabelaValorVencimentoVO();
                                    retTvv.PropostaID = contrato.ID;
                                    retTvv.TabelaID = tabela[0].ID;
                                    if (tabela[0].VencimentoFim != DateTime.MinValue)
                                    { retTvv.Vencimento = tabela[0].VencimentoFim.AddMonths(1); }
                                    if (taxa != null && !taxa.Embutido)
                                    { retTvv.ValorBancario = taxa.ValorEmbutido; }

                                    if (contrato.CobrarTaxaAssociativa)
                                    {
                                        estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                                        if (estipulanteTaxa != null)
                                        { retTvv.ValorSindicalizacao = estipulanteTaxa.Valor; }
                                    }

                                    lista.Add(retTvv);
                                    //taxa = null;
                                }
                                else
                                    cob.Valor += retTvv.ValorBancario;
                            }
                        }
                        else
                            cob.Valor += retTvv.ValorBancario;

                        #endregion

                        #region data de admissao do contrato

                        //Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                        else { continue; }
                        #endregion

                        #endregion

                        doc.Append(Environment.NewLine);

                        EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Numero do lote remessa
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº seqüencial do registro de lote
                        EntityBase.AppendPreparedField(ref doc, "P", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de movimento remessa (pag3) - nota 14 (pag.14)
                        EntityBase.AppendPreparedField(ref doc, agencia, 4);        //Agência do Destinatária FIDC
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Dígito da Agência do Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, numcontacorr.PadLeft(9, '0'), 9); //Número da conta corrente
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);            //Dígito da conta corrente (definido pela homologação)
                        EntityBase.AppendPreparedField(ref doc, numcontacorr.PadLeft(9, '0'), 9); //Conta cobrança Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1); //Dígito da conta cobrança Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, "  ", 2);           //branco
                        cob.NossoNumero = cob.GeraNossoNumeroSantander();
                        EntityBase.AppendPreparedField(ref doc, cob.NossoNumero, 13); //Identificação do título no Banco
                        EntityBase.AppendPreparedField(ref doc, "5", 1);            //Tipo de cobrança 
                        EntityBase.AppendPreparedField(ref doc, "1", 1);            //Forma de Cadastramento - 1 para cobranca registrada
                        EntityBase.AppendPreparedField(ref doc, "1", 1);            //Tipo de documento - 1- Tradicional , 2- Escritural
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(15,'0'), 15); //Seu número
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyyyy"), 8);  // Data de vencimento - pos 78

                        //valor do boleto
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[0].Replace(".","").PadLeft(13, '0'), 13);
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[1].PadLeft(2, '0'), 2);

                        EntityBase.AppendPreparedField(ref doc, "0000", 4);         //Agência encarregada da cobrança
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Dígito da Agência do Beneficiário
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "02", 2);           //Espécie do título Nota 20 pag.15 ???
                        EntityBase.AppendPreparedField(ref doc, "N", 1);            //Identif. de título Aceito/Não Aceito
                        EntityBase.AppendPreparedField(ref doc, cob.DataCriacao.ToString("ddMMyyyy"), 8); //Data da emissão do título
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //Código do juros de mora - nota 21 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8); //Data do juros de mora - nota 22
                        EntityBase.AppendPreparedField(ref doc, "0".PadRight(15, '0'), 15); //Valor da mora/dia ou Taxa mensal - nota 18 pag.15
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Código do desconto 1 - nota 23 pag.16
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8);     //Data de desconto 1 - nota 23 pag.16
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor ou Percentual do desconto concedido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do IOF a ser recolhido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do abatimento

                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(25, '0'), 25); //Identificação do título na empresa
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Código para protesto - nota 25 pag.16 - 0=nao protestar
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Número de dias para protesto
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //Código para Baixa/Devolução - nota 26 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Número de dias para Baixa/Devolução
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Código da moeda - nota 27 - pag.16
                        EntityBase.AppendPreparedField(ref doc, " ".PadRight(11, ' '), 11); //branco

                        //
                        #endregion

                        #region DETAIL Q ---------------------------------

                        doc.Append(Environment.NewLine);

                        numSequencial++;

                        EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Numero do lote remessa
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº seqüencial do registro no lote - nota 11 - pg.14
                        EntityBase.AppendPreparedField(ref doc, "Q", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de movimento remessa - nota 14 - pag 14

                        EntityBase.AppendPreparedField(ref doc, "1", 1);           //Tipo de inscrição do Pagador - nota 29 - pag.17 - 1 = cpf
                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularCPF.PadLeft(15, '0'), 15); //Número de inscrição do Pagador - pag. 4

                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 40); //Nome Pagador

                        endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                        endereco.Carregar(pm);
                        if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                        if (endereco.Numero == null) endereco.Numero = "";
                        if (string.IsNullOrEmpty(endereco.Complemento))
                        {
                            int tamanhoDisponivel = 40 - (endereco.Numero.Length + 1);

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero);
                        }
                        else
                        {
                            int tamanhoDisponivel = 40 - ((endereco.Numero.Length + 1) + (endereco.Complemento.Length + 1));

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero, ",", endereco.Complemento);
                            else
                                endereco.Logradouro += string.Concat(",", endereco.Complemento);
                        }

                        if (string.IsNullOrEmpty(endereco.CEP)) { endereco.CEP = "00000000"; } else { endereco.CEP = endereco.CEP.Replace("-", ""); }
                        EntityBase.AppendPreparedField(ref doc, endereco.Logradouro, 40); // 
                        EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP, 8);
                        EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);

                        //if(string.IsNullOrEmpty(cob.ContratoTitularCPF) || cob.ContratoTitularCPF == "00000000000")
                            EntityBase.AppendPreparedField(ref doc, "0", 1); //Tipo de inscrição Sacador/avalista - nota 29 - pag.17
                        //else
                        //    EntityBase.AppendPreparedField(ref doc, "1", 1); //Tipo de inscrição Sacador/avalista - nota 29 - pag.17

                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Nº de inscrição Sacador/avalista

                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(40, ' '), 40); //Nome do Sacador/Avalista

                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Identificador de carne - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Sequencial da Parcela ou número inicial da parcela - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Quantidade total de parcelas - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Número do plano - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, " ".PadRight(19, ' '), 19); //Brancos

                        #endregion

                        totalRegistrosDetalhe++;
                        valorTotal += cob.Valor;


                        if (cob.ArquivoIDUltimoEnvio == null) { cob.ArquivoIDUltimoEnvio = -10; }
                        cob.Banco = 353;
                        cob.Carteira = 101;
                        pm.Save(cob);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string path = System.Configuration.ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                            System.IO.File.WriteAllText(path + "err_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".txt",
                                ex.Message +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + ex.StackTrace +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + Convert.ToString(cob.ID) +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + i.ToString(),
                                System.Text.Encoding.ASCII);
                        }
                        catch { }
                        continue;
                    }
                }
                #endregion

                #region TRAILLER DE LOTE ---------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "033", 3);
                EntityBase.AppendPreparedField(ref doc, "0001", 4);
                EntityBase.AppendPreparedField(ref doc, "5", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9);
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 2).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 217);

                #endregion

                #region TRAILLER -----------------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "033", 3);
                EntityBase.AppendPreparedField(ref doc, "9999", 4); //9999 - Numero do lote remessa
                EntityBase.AppendPreparedField(ref doc, "9", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9); //branco
                EntityBase.AppendPreparedField(ref doc, "1".PadLeft(6, '0'), 6); //qtd de lotes - registros tipo 1
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 4).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 211); //

                #endregion

                if (_pm == null) { pm.Commit(); } //se não está partcipando de uma transacao externa, comita //pm.Rollback();
                return doc.ToString();
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }

        public static String GeraDocumentoCobranca_SANTANDER_Test(string cobrancaId, PersistenceManager _pm)
        {
            Int32 numSequencial = 0;

            PersistenceManager pm = _pm;

            Int32 contratoadmQualicorpID = Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]);

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();
            }

            //Cobranca cobprova = new Cobranca(cobrancaId);
            //pm.Load(cobprova);
            //Contrato contratoprova = new Contrato(cobprova.PropostaID);
            //pm.Load(contratoprova);

            //String agCCDV_itau = "";
            String headerCedente = "UBRASP - UNIAO BRASILEIRA";
            string agencia = "0001"; //, agenciadv = "0";
            string numcontacorr = "13014765";
            string dvcontacorr = "2";
            string cnpj = "49938327000106"; //
            string codTransmissao = "000100001201344";
            //string complemento = "I 52";

            //agCCDV_itau = "0646042606";    //qualicorp
            string numConvenio = "1201344"; // Cobranca.ConvenioUBRASP.PadLeft(9, '0');
            //string carteira = "5"; //, variacao = "019";

            int numeroDoArquivo = 1;
            string[] arquivos = Directory.GetFiles(ConfigurationManager.AppSettings["financialFilePathFISICO"], "*.dat");
            if (arquivos != null && arquivos.Length > 0) numeroDoArquivo = arquivos.Length + 1;
            if (numeroDoArquivo > 999) numeroDoArquivo = 1 + (arquivos.Length - 999);


            try
            {
                if (cobrancaId == null) { return null; }

                String dataAgora = DateTime.Now.ToString("ddMMyyyy");
                StringBuilder doc = new StringBuilder();

                ////Cobranca.eCarteira carteira = (Cobranca.eCarteira)aras[0].Carteira;

                #region HEADER -------------------------------------------

                EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0000", 4);         //lote de serviço
                EntityBase.AppendPreparedField(ref doc, "0", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "        ", 8);     //brancos
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //tipo de inscrição - 2 para CNPJ, 1 para CPF

                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);          //CNPJ (ou cpf) 
                EntityBase.AppendPreparedField(ref doc, codTransmissao.PadLeft(15, '0'), 15); //Codigo de transmissao 
                EntityBase.AppendPreparedField(ref doc, " ", 25);           //brancos
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //nome empresa
                EntityBase.AppendPreparedField(ref doc, "Banco Santander".PadRight(30, ' '), 30); //Nome do Banco
                EntityBase.AppendPreparedField(ref doc, " ", 10);           //brancos
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //Código Remessa: 1 = remessa
                EntityBase.AppendPreparedField(ref doc, dataAgora, 8);      //data de geracao do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 6);            //brancos
                EntityBase.AppendPreparedField(ref doc, numeroDoArquivo.ToString().PadLeft(6, '0'), 6); //brancos
                EntityBase.AppendPreparedField(ref doc, "040", 3);          //versão do layout do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 74);           //brancos

                #endregion

                #region HEADER DE LOTE -----------------------------------

                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "R", 1);            //tipo de operação
                EntityBase.AppendPreparedField(ref doc, "01", 2);           //tipo de serviço
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //brancos
                EntityBase.AppendPreparedField(ref doc, "030", 3);          //Nº da Versão do Layout do Lote - nao criticado pelo banco
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //Tipo de Inscrição da Empresa - 2=CNPJ
                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);  //CNPJ (ou cpf) //faltou: cnpj
                EntityBase.AppendPreparedField(ref doc, " ", 20);           //brancos
                EntityBase.AppendPreparedField(ref doc, codTransmissao.PadLeft(15, '0'), 15);//Codigo de transmissao 
                EntityBase.AppendPreparedField(ref doc, " ", 05);           //brancos
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome do beneficiario
                EntityBase.AppendPreparedField(ref doc, " ", 40);           //Mensagem 1
                EntityBase.AppendPreparedField(ref doc, " ", 40);           //Mensagem 2
                EntityBase.AppendPreparedField(ref doc, "0001".PadLeft(8, '0'), 8); //Número remessa/retorno
                EntityBase.AppendPreparedField(ref doc, dataAgora, 8);      //data de gravaçao do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 41);           //brancos

                #endregion

                //Carrega cobrancas envolvidas
                List<string> cobrancaIDs = new List<string>(); //cobrancaIDs.Add(cobrancaId);
                cobrancaIDs.AddRange(new string[] { "3375", "3376", "3377", "3378", "3379", "3380", "3381", "3382", "3383", "3384", "3385", "3386", "3387", "3388", "3389", "3390", "3391", "3392", "3393", "3394", "3395", "3396", "3397", "3398", "3399", "3400", "3401", "3402", "3403", "3404", "3405", "3406", "3407", "3408", "3409", "3410", "3411" });
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela_Optimized(cobrancaIDs, pm); //
                List<Object> cobrancaIds = new List<Object>();
                if (cobrancas == null) { return null; }

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL -------------------------------------------

                Endereco endereco = null;
                //, diaDataSemJuros, result = 0, 
                int totalRegistrosDetalhe = 0;
                String mascara6 = new String('0', 6); //, nossoNumero = "";
                Decimal valorTotal = 0; //, mora = 0, multa = 0;
                //Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                //CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                int i = 0;

                System.Collections.Hashtable contratoADM_Criterio = new System.Collections.Hashtable();
                List<string> parametrosProcessados = new List<string>();

                foreach (Cobranca cob in cobrancas)
                {
                    if (parametrosProcessados.Contains(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID))) continue;

                    parametrosProcessados.Add(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID));

                    i++;
                    try
                    {
                        #region DETAIL P ---------------------------------

                        #region

                        #region localiza o critério correspondente

                        contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                        #endregion

                        numSequencial++;

                        cobrancaIds.Add(cob.ID);

                        if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                        {
                            numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                            qtdTotalBoletos = 0;
                            foreach (Cobranca _temp in cobrancas)
                            {
                                if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                                else if (qtdTotalBoletos > 0) { break; }
                            }
                        }

                        numBoletoCorrente++;

                        #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                        retTvv = TabelaValorVencimentoVO.ExisteProposta(cob.PropostaID, lista);
                        if (retTvv == null)
                        {
                            //contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                            tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cob.DataVencimento, pm);
                            if (tabela != null && tabela.Count > 0)
                            {
                                retTvv = TabelaValorVencimentoVO.ExisteTabela(tabela[0].ID, lista);
                                if (retTvv == null)
                                {
                                    taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                                    if (taxa != null && !taxa.Embutido)
                                    {
                                        cob.Valor += taxa.ValorEmbutido;
                                    }

                                    retTvv = new TabelaValorVencimentoVO();
                                    retTvv.PropostaID = contrato.ID;
                                    retTvv.TabelaID = tabela[0].ID;
                                    if (tabela[0].VencimentoFim != DateTime.MinValue)
                                    { retTvv.Vencimento = tabela[0].VencimentoFim.AddMonths(1); }
                                    if (taxa != null && !taxa.Embutido)
                                    { retTvv.ValorBancario = taxa.ValorEmbutido; }

                                    if (contrato.CobrarTaxaAssociativa)
                                    {
                                        estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                                        if (estipulanteTaxa != null)
                                        { retTvv.ValorSindicalizacao = estipulanteTaxa.Valor; }
                                    }

                                    lista.Add(retTvv);
                                    //taxa = null;
                                }
                                else
                                    cob.Valor += retTvv.ValorBancario;
                            }
                        }
                        else
                            cob.Valor += retTvv.ValorBancario;

                        #endregion

                        #region data de admissao do contrato

                        //Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                        else { continue; }
                        #endregion

                        #endregion

                        doc.Append(Environment.NewLine);

                        EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Numero do lote remessa
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº seqüencial do registro de lote
                        EntityBase.AppendPreparedField(ref doc, "P", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de movimento remessa (pag3) - nota 14 (pag.14)
                        EntityBase.AppendPreparedField(ref doc, agencia, 4);        //Agência do Destinatária FIDC
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Dígito da Agência do Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, numcontacorr.PadLeft(9, '0'), 9); //Número da conta corrente
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);            //Dígito da conta corrente (definido pela homologação)
                        EntityBase.AppendPreparedField(ref doc, numcontacorr.PadLeft(9, '0'), 9); //Conta cobrança Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1); //Dígito da conta cobrança Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, "  ", 2);           //branco
                        cob.NossoNumero = cob.GeraNossoNumeroSantander();
                        EntityBase.AppendPreparedField(ref doc, cob.NossoNumero, 13); //Identificação do título no Banco
                        EntityBase.AppendPreparedField(ref doc, "5", 1);            //Tipo de cobrança 
                        EntityBase.AppendPreparedField(ref doc, "1", 1);            //Forma de Cadastramento - 1 para cobranca registrada
                        EntityBase.AppendPreparedField(ref doc, "1", 1);            //Tipo de documento - 1- Tradicional , 2- Escritural
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(15, '0'), 15); //Seu número
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyyyy"), 8);  // Data de vencimento - pos 78

                        //valor do boleto
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[0].Replace(".", "").PadLeft(13, '0'), 13);
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[1].PadLeft(2, '0'), 2);

                        EntityBase.AppendPreparedField(ref doc, "0000", 4);         //Agência encarregada da cobrança
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Dígito da Agência do Beneficiário
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "02", 2);           //Espécie do título Nota 20 pag.15 ???
                        EntityBase.AppendPreparedField(ref doc, "N", 1);            //Identif. de título Aceito/Não Aceito
                        EntityBase.AppendPreparedField(ref doc, cob.DataCriacao.ToString("ddMMyyyy"), 8); //Data da emissão do título
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //Código do juros de mora - nota 21 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8); //Data do juros de mora - nota 22
                        EntityBase.AppendPreparedField(ref doc, "0".PadRight(15, '0'), 15); //Valor da mora/dia ou Taxa mensal - nota 18 pag.15
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Código do desconto 1 - nota 23 pag.16
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8);     //Data de desconto 1 - nota 23 pag.16
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor ou Percentual do desconto concedido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do IOF a ser recolhido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do abatimento

                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(25, '0'), 25); //Identificação do título na empresa
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Código para protesto - nota 25 pag.16 - 0=nao protestar
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Número de dias para protesto
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //Código para Baixa/Devolução - nota 26 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Número de dias para Baixa/Devolução
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Código da moeda - nota 27 - pag.16
                        EntityBase.AppendPreparedField(ref doc, " ".PadRight(11, ' '), 11); //branco

                        //
                        #endregion

                        #region DETAIL Q ---------------------------------

                        doc.Append(Environment.NewLine);

                        numSequencial++;

                        EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Numero do lote remessa
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº seqüencial do registro no lote - nota 11 - pg.14
                        EntityBase.AppendPreparedField(ref doc, "Q", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de movimento remessa - nota 14 - pag 14

                        EntityBase.AppendPreparedField(ref doc, "1", 1);           //Tipo de inscrição do Pagador - nota 29 - pag.17 - 1 = cpf
                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularCPF.PadLeft(15, '0'), 15); //Número de inscrição do Pagador - pag. 4

                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 40); //Nome Pagador

                        endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                        endereco.Carregar(pm);
                        if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                        if (endereco.Numero == null) endereco.Numero = "";
                        if (string.IsNullOrEmpty(endereco.Complemento))
                        {
                            int tamanhoDisponivel = 40 - (endereco.Numero.Length + 1);

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero);
                        }
                        else
                        {
                            int tamanhoDisponivel = 40 - ((endereco.Numero.Length + 1) + (endereco.Complemento.Length + 1));

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero, ",", endereco.Complemento);
                            else
                                endereco.Logradouro += string.Concat(",", endereco.Complemento);
                        }

                        if (string.IsNullOrEmpty(endereco.CEP)) { endereco.CEP = "00000000"; } else { endereco.CEP = endereco.CEP.Replace("-", ""); }
                        EntityBase.AppendPreparedField(ref doc, endereco.Logradouro, 40); // 
                        EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP, 8);
                        EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);

                        //if(string.IsNullOrEmpty(cob.ContratoTitularCPF) || cob.ContratoTitularCPF == "00000000000")
                        EntityBase.AppendPreparedField(ref doc, "0", 1); //Tipo de inscrição Sacador/avalista - nota 29 - pag.17
                        //else
                        //    EntityBase.AppendPreparedField(ref doc, "1", 1); //Tipo de inscrição Sacador/avalista - nota 29 - pag.17

                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Nº de inscrição Sacador/avalista

                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(40, ' '), 40); //Nome do Sacador/Avalista

                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Identificador de carne - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Sequencial da Parcela ou número inicial da parcela - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Quantidade total de parcelas - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Número do plano - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, " ".PadRight(19, ' '), 19); //Brancos

                        #endregion

                        totalRegistrosDetalhe++;
                        valorTotal += cob.Valor;


                        cob.ArquivoIDUltimoEnvio = -10;
                        cob.Carteira = 101;
                        pm.Save(cob);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string path = System.Configuration.ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                            System.IO.File.WriteAllText(path + "err_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".txt",
                                ex.Message +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + ex.StackTrace +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + Convert.ToString(cob.ID) +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + i.ToString(),
                                System.Text.Encoding.ASCII);
                        }
                        catch { }
                        continue;
                    }
                }
                #endregion

                #region TRAILLER DE LOTE ---------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "033", 3);
                EntityBase.AppendPreparedField(ref doc, "0001", 4);
                EntityBase.AppendPreparedField(ref doc, "5", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9);
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 2).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 217);

                #endregion

                #region TRAILLER -----------------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "033", 3);
                EntityBase.AppendPreparedField(ref doc, "9999", 4); //9999 - Numero do lote remessa
                EntityBase.AppendPreparedField(ref doc, "9", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9); //branco
                EntityBase.AppendPreparedField(ref doc, "1".PadLeft(6, '0'), 6); //qtd de lotes - registros tipo 1
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 4).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 211); //

                #endregion

                if (_pm == null) { pm.Commit(); } //se não está partcipando de uma transacao externa, comita //pm.Rollback();
                return doc.ToString();
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }


        public static String GeraDocumentoCobranca_SANTANDER___TEMP(IList<String> cobrancaIDs, IList<ArquivoRemessaAgendamento> aras, PersistenceManager _pm)
        {
            Int32 numSequencial = 0;

            PersistenceManager pm = _pm;

            Int32 contratoadmQualicorpID = Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]);

            if (_pm == null)
            {
                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();
            }

            String headerCedente = "UBRASP - UNIAO BRASILEIRA";
            string agencia = "0001"; //, agenciadv = "0";
            string numcontacorr = "13014765";
            string dvcontacorr = "2";
            string cnpj = "49938327000106"; //
            string codTransmissao = "000100001201344";
            //string complemento = "I 52";

            //agCCDV_itau = "0646042606";    //qualicorp
            string numConvenio = "1201344"; // Cobranca.ConvenioUBRASP.PadLeft(9, '0');
            //string carteira = "5"; //, variacao = "019";

            int numeroDoArquivo = 1;
            string[] arquivos = Directory.GetFiles(ConfigurationManager.AppSettings["financialFilePathFISICO"], "*.dat");
            if (arquivos != null && arquivos.Length > 0) numeroDoArquivo = arquivos.Length + 1;
            if (numeroDoArquivo > 999) numeroDoArquivo = 1 + (arquivos.Length - 999);

            try
            {
                String dataAgora = DateTime.Now.ToString("ddMMyyyy");
                StringBuilder doc = new StringBuilder();

                #region HEADER -------------------------------------------

                EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0000", 4);         //lote de serviço
                EntityBase.AppendPreparedField(ref doc, "0", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "        ", 8);     //brancos
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //tipo de inscrição - 2 para CNPJ, 1 para CPF

                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);          //CNPJ (ou cpf) 
                EntityBase.AppendPreparedField(ref doc, codTransmissao.PadLeft(15, '0'), 15); //Codigo de transmissao 
                EntityBase.AppendPreparedField(ref doc, " ", 25);           //brancos
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //nome empresa
                EntityBase.AppendPreparedField(ref doc, "Banco Santander".PadRight(30, ' '), 30); //Nome do Banco
                EntityBase.AppendPreparedField(ref doc, " ", 10);           //brancos
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //Código Remessa: 1 = remessa
                EntityBase.AppendPreparedField(ref doc, dataAgora, 8);      //data de geracao do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 6);            //brancos
                EntityBase.AppendPreparedField(ref doc, numeroDoArquivo.ToString().PadLeft(6, '0'), 6); //brancos
                EntityBase.AppendPreparedField(ref doc, "040", 3);          //versão do layout do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 74);           //brancos

                #endregion

                #region HEADER DE LOTE -----------------------------------

                doc.Append(Environment.NewLine);
                EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Lote de Serviço
                EntityBase.AppendPreparedField(ref doc, "1", 1);            //tipo de registro
                EntityBase.AppendPreparedField(ref doc, "R", 1);            //tipo de operação
                EntityBase.AppendPreparedField(ref doc, "01", 2);           //tipo de serviço
                EntityBase.AppendPreparedField(ref doc, "  ", 2);           //brancos
                EntityBase.AppendPreparedField(ref doc, "030", 3);          //Nº da Versão do Layout do Lote - nao criticado pelo banco
                EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                EntityBase.AppendPreparedField(ref doc, "2", 1);            //Tipo de Inscrição da Empresa - 2=CNPJ
                EntityBase.AppendPreparedField(ref doc, cnpj.PadLeft(15, '0'), 15);  //CNPJ (ou cpf) //faltou: cnpj
                EntityBase.AppendPreparedField(ref doc, " ", 20);           //brancos
                EntityBase.AppendPreparedField(ref doc, codTransmissao.PadLeft(15, '0'), 15);//Codigo de transmissao 
                EntityBase.AppendPreparedField(ref doc, " ", 05);           //brancos
                EntityBase.AppendPreparedField(ref doc, headerCedente.PadRight(30, ' '), 30); //Nome do beneficiario
                EntityBase.AppendPreparedField(ref doc, " ", 40);           //Mensagem 1
                EntityBase.AppendPreparedField(ref doc, " ", 40);           //Mensagem 2
                EntityBase.AppendPreparedField(ref doc, "0001".PadLeft(8, '0'), 8); //Número remessa/retorno
                EntityBase.AppendPreparedField(ref doc, dataAgora, 8);      //data de gravaçao do arquivo
                EntityBase.AppendPreparedField(ref doc, " ", 41);           //brancos

                #endregion

                //Carrega cobrancas envolvidas
                IList<Cobranca> _cobrancas = Cobranca.CarregarTodas_OrdemPorContratoParcela_Optimized(cobrancaIDs, pm);
                if (_cobrancas == null) { return null; }
                List<Object> cobrancaIds = new List<Object>();

                IList<Cobranca> cobrancas = new List<Cobranca>();
                foreach (var c in _cobrancas)
                {
                    //if (Convert.ToString(c.BeneficiarioId) == "39453" || Convert.ToString(c.BeneficiarioId) == "12971" || Convert.ToString(c.BeneficiarioId) == "13366") continue;
                    cobrancas.Add(c);
                }

                if (cobrancas == null || cobrancas.Count == 0) { return null; }

                List<TabelaValorVencimentoVO> lista = new List<TabelaValorVencimentoVO>();

                #region DETAIL -------------------------------------------

                Endereco endereco = null;
                //, diaDataSemJuros, result = 0, 
                int totalRegistrosDetalhe = 0;
                String mascara6 = new String('0', 6); //, nossoNumero = "";
                Decimal valorTotal = 0; //, mora = 0, multa = 0;
                //Object valorDataLimite = null;
                DateTime contratoAdmissao = DateTime.MinValue, vigencia, vencimento, dataLimite = DateTime.MinValue;
                //CalendarioVencimento rcv = null;
                TabelaValorVencimentoVO retTvv = null;
                Contrato contrato = null;
                IList<TabelaValor> tabela = null; Taxa taxa = null; EstipulanteTaxa estipulanteTaxa = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                int qtdTotalBoletos = 0, numBoletoCorrente = 0, idContratoAtual = 0;

                int i = 0;

                System.Collections.Hashtable contratoADM_Criterio = new System.Collections.Hashtable();
                List<string> parametrosProcessados = new List<string>();

                foreach (Cobranca cob in cobrancas)
                {
                    if (Convert.ToString(cob.ID) == "4977") { int guenta = 0; }

                    if (parametrosProcessados.Contains(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID))) continue;

                    parametrosProcessados.Add(string.Concat(cob.DataVencimento.ToString("ddMMyyyy"), cob.PropostaID));

                    i++;
                    try
                    {
                        #region DETAIL P ---------------------------------

                        #region

                        #region localiza o critério correspondente

                        contrato = Contrato.CarregarParcial(cob.PropostaID, pm);

                        #endregion

                        numSequencial++;

                        cobrancaIds.Add(cob.ID);

                        if (Convert.ToInt32(cob.PropostaID) != idContratoAtual)
                        {
                            numBoletoCorrente = 0; idContratoAtual = Convert.ToInt32(cob.PropostaID);
                            qtdTotalBoletos = 0;
                            foreach (Cobranca _temp in cobrancas)
                            {
                                if (Convert.ToInt32(_temp.PropostaID) == idContratoAtual) { qtdTotalBoletos++; }
                                else if (qtdTotalBoletos > 0) { break; }
                            }
                        }

                        numBoletoCorrente++;

                        #region checa se tem despesa nao embutida com postagem. se tem, incrementa o valor da cobranca:

                        retTvv = TabelaValorVencimentoVO.ExisteProposta(cob.PropostaID, lista);
                        if (retTvv == null)
                        {
                            //contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                            tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cob.DataVencimento, pm);
                            if (tabela != null && tabela.Count > 0)
                            {
                                retTvv = TabelaValorVencimentoVO.ExisteTabela(tabela[0].ID, lista);
                                if (retTvv == null)
                                {
                                    taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                                    if (taxa != null && !taxa.Embutido)
                                    {
                                        cob.Valor += taxa.ValorEmbutido;
                                    }

                                    retTvv = new TabelaValorVencimentoVO();
                                    retTvv.PropostaID = contrato.ID;
                                    retTvv.TabelaID = tabela[0].ID;
                                    if (tabela[0].VencimentoFim != DateTime.MinValue)
                                    { retTvv.Vencimento = tabela[0].VencimentoFim.AddMonths(1); }
                                    if (taxa != null && !taxa.Embutido)
                                    { retTvv.ValorBancario = taxa.ValorEmbutido; }

                                    if (contrato.CobrarTaxaAssociativa)
                                    {
                                        estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                                        if (estipulanteTaxa != null)
                                        { retTvv.ValorSindicalizacao = estipulanteTaxa.Valor; }
                                    }

                                    lista.Add(retTvv);
                                    //taxa = null;
                                }
                                else
                                    cob.Valor += retTvv.ValorBancario;
                            }
                        }
                        else
                            cob.Valor += retTvv.ValorBancario;

                        #endregion

                        #region data de admissao do contrato

                        //Contrato _contrato = Contrato.CarregarParcial(cob.PropostaID, pm);
                        if (contrato != null) { contratoAdmissao = contrato.Admissao; }
                        else { continue; }
                        #endregion

                        #endregion

                        doc.Append(Environment.NewLine);

                        EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Numero do lote remessa
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº seqüencial do registro de lote
                        EntityBase.AppendPreparedField(ref doc, "P", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de movimento remessa (pag3) - nota 14 (pag.14)
                        EntityBase.AppendPreparedField(ref doc, agencia, 4);        //Agência do Destinatária FIDC
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Dígito da Agência do Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, numcontacorr.PadLeft(9, '0'), 9); //Número da conta corrente
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1);            //Dígito da conta corrente (definido pela homologação)
                        EntityBase.AppendPreparedField(ref doc, numcontacorr.PadLeft(9, '0'), 9); //Conta cobrança Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, dvcontacorr, 1); //Dígito da conta cobrança Destinatária FIDC ????
                        EntityBase.AppendPreparedField(ref doc, "  ", 2);           //branco
                        cob.NossoNumero = cob.GeraNossoNumeroSantander();
                        EntityBase.AppendPreparedField(ref doc, cob.NossoNumero, 13); //Identificação do título no Banco
                        EntityBase.AppendPreparedField(ref doc, "5", 1);            //Tipo de cobrança 
                        EntityBase.AppendPreparedField(ref doc, "1", 1);            //Forma de Cadastramento - 1 para cobranca registrada
                        EntityBase.AppendPreparedField(ref doc, "1", 1);            //Tipo de documento - 1- Tradicional , 2- Escritural
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(15, '0'), 15); //Seu número
                        EntityBase.AppendPreparedField(ref doc, cob.DataVencimento.ToString("ddMMyyyy"), 8);  // Data de vencimento - pos 78

                        //valor do boleto
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[0].Replace(".", "").PadLeft(13, '0'), 13);
                        EntityBase.AppendPreparedField(ref doc, cob.Valor.ToString("N2", cinfo).Split(',')[1].PadLeft(2, '0'), 2);

                        EntityBase.AppendPreparedField(ref doc, "0000", 4);         //Agência encarregada da cobrança
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Dígito da Agência do Beneficiário
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "02", 2);           //Espécie do título Nota 20 pag.15 ???
                        EntityBase.AppendPreparedField(ref doc, "N", 1);            //Identif. de título Aceito/Não Aceito
                        EntityBase.AppendPreparedField(ref doc, cob.DataCriacao.ToString("ddMMyyyy"), 8); //Data da emissão do título
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //Código do juros de mora - nota 21 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8); //Data do juros de mora - nota 22
                        EntityBase.AppendPreparedField(ref doc, "0".PadRight(15, '0'), 15); //Valor da mora/dia ou Taxa mensal - nota 18 pag.15
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Código do desconto 1 - nota 23 pag.16
                        EntityBase.AppendPreparedField(ref doc, "00000000", 8);     //Data de desconto 1 - nota 23 pag.16
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor ou Percentual do desconto concedido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do IOF a ser recolhido
                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Valor do abatimento

                        EntityBase.AppendPreparedField(ref doc, Convert.ToString(cob.ID).PadLeft(25, '0'), 25); //Identificação do título na empresa
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //Código para protesto - nota 25 pag.16 - 0=nao protestar
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Número de dias para protesto
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //Código para Baixa/Devolução - nota 26 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "0", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Número de dias para Baixa/Devolução
                        EntityBase.AppendPreparedField(ref doc, "00", 2);           //Código da moeda - nota 27 - pag.16
                        EntityBase.AppendPreparedField(ref doc, " ".PadRight(11, ' '), 11); //branco

                        //
                        #endregion

                        #region DETAIL Q ---------------------------------

                        doc.Append(Environment.NewLine);

                        numSequencial++;

                        EntityBase.AppendPreparedField(ref doc, "033", 3);          //codigo do banco na compensacao
                        EntityBase.AppendPreparedField(ref doc, "0001", 4);         //Numero do lote remessa
                        EntityBase.AppendPreparedField(ref doc, "3", 1);            //tipo de registro
                        EntityBase.AppendPreparedField(ref doc, numSequencial.ToString().PadLeft(5, '0'), 5); //Nº seqüencial do registro no lote - nota 11 - pg.14
                        EntityBase.AppendPreparedField(ref doc, "Q", 1);            //Código de Segmento do Reg. Detalhe
                        EntityBase.AppendPreparedField(ref doc, " ", 1);            //branco
                        EntityBase.AppendPreparedField(ref doc, "01", 2);           //Código de movimento remessa - nota 14 - pag 14

                        EntityBase.AppendPreparedField(ref doc, "1", 1);           //Tipo de inscrição do Pagador - nota 29 - pag.17 - 1 = cpf
                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularCPF.PadLeft(15, '0'), 15); //Número de inscrição do Pagador - pag. 4

                        EntityBase.AppendPreparedField(ref doc, cob.ContratoTitularNome, 40); //Nome Pagador

                        endereco = new Endereco(cob.ContratoEnderecoCobrancaID);
                        endereco.Carregar(pm);
                        if (endereco.CEP.IndexOf('-') > -1) { endereco.CEP = endereco.CEP.Replace("-", ""); }

                        if (endereco.Numero == null) endereco.Numero = "";
                        if (string.IsNullOrEmpty(endereco.Complemento))
                        {
                            int tamanhoDisponivel = 40 - (endereco.Numero.Length + 1);

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero);
                        }
                        else
                        {
                            int tamanhoDisponivel = 40 - ((endereco.Numero.Length + 1) + (endereco.Complemento.Length + 1));

                            if (endereco.Logradouro.Length > tamanhoDisponivel)
                            {
                                endereco.Logradouro = endereco.Logradouro.Substring(0, tamanhoDisponivel);
                            }

                            if (!string.IsNullOrEmpty(endereco.Numero))
                                endereco.Logradouro += string.Concat(",", endereco.Numero, ",", endereco.Complemento);
                            else
                                endereco.Logradouro += string.Concat(",", endereco.Complemento);
                        }

                        if (string.IsNullOrEmpty(endereco.CEP)) { endereco.CEP = "00000000"; } else { endereco.CEP = endereco.CEP.Replace("-", ""); }
                        EntityBase.AppendPreparedField(ref doc, endereco.Logradouro, 40); // 
                        EntityBase.AppendPreparedField(ref doc, endereco.Bairro, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.CEP, 8);
                        EntityBase.AppendPreparedField(ref doc, endereco.Cidade, 15);
                        EntityBase.AppendPreparedField(ref doc, endereco.UF, 2);

                        //if(string.IsNullOrEmpty(cob.ContratoTitularCPF) || cob.ContratoTitularCPF == "00000000000")
                        EntityBase.AppendPreparedField(ref doc, "0", 1); //Tipo de inscrição Sacador/avalista - nota 29 - pag.17
                        //else
                        //    EntityBase.AppendPreparedField(ref doc, "1", 1); //Tipo de inscrição Sacador/avalista - nota 29 - pag.17

                        EntityBase.AppendPreparedField(ref doc, "0".PadLeft(15, '0'), 15); //Nº de inscrição Sacador/avalista

                        EntityBase.AppendPreparedField(ref doc, " ".PadLeft(40, ' '), 40); //Nome do Sacador/Avalista

                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Identificador de carne - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Sequencial da Parcela ou número inicial da parcela - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Quantidade total de parcelas - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, "000", 3); //Número do plano - nota 28 - pag.16
                        EntityBase.AppendPreparedField(ref doc, " ".PadRight(19, ' '), 19); //Brancos

                        #endregion

                        totalRegistrosDetalhe++;
                        valorTotal += cob.Valor;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            string path = System.Configuration.ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                            System.IO.File.WriteAllText(path + "err_" + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".txt",
                                ex.Message +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + ex.StackTrace +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + Convert.ToString(cob.ID) +
                                Environment.NewLine + "-----------------------------------------" +
                                Environment.NewLine + i.ToString(),
                                System.Text.Encoding.ASCII);
                        }
                        catch { }
                        continue;
                    }
                }
                #endregion

                #region TRAILLER DE LOTE ---------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "033", 3);
                EntityBase.AppendPreparedField(ref doc, "0001", 4);
                EntityBase.AppendPreparedField(ref doc, "5", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9);
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 2).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 217);

                #endregion

                #region TRAILLER -----------------------------------------

                doc.Append(Environment.NewLine);

                EntityBase.AppendPreparedField(ref doc, "033", 3);
                EntityBase.AppendPreparedField(ref doc, "9999", 4); //9999 - Numero do lote remessa
                EntityBase.AppendPreparedField(ref doc, "9", 1);
                EntityBase.AppendPreparedField(ref doc, " ", 9); //branco
                EntityBase.AppendPreparedField(ref doc, "1".PadLeft(6, '0'), 6); //qtd de lotes - registros tipo 1
                EntityBase.AppendPreparedField(ref doc, (numSequencial + 4).ToString().PadLeft(6, '0'), 6);
                EntityBase.AppendPreparedField(ref doc, " ", 211); //

                #endregion

                return doc.ToString();
            }
            catch
            {
                if (_pm == null) { pm.Rollback(); }
                throw;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }
    }

    #region demais classes 

    class TabelaValorVencimentoVO
    {
        Object _propostaId;
        Object _tabelaId;
        DateTime _vencimento;
        Decimal _valorBancario;
        Decimal _valorSindicalizacao;

        public Object PropostaID
        {
            get { return _propostaId; }
            set { _propostaId= value; }
        }

        public Object TabelaID
        {
            get { return _tabelaId; }
            set { _tabelaId= value; }
        }

        public DateTime Vencimento
        {
            get { return _vencimento; }
            set { _vencimento= value; }
        }

        public Decimal ValorBancario
        {
            get { return _valorBancario; }
            set { _valorBancario = value; }
        }

        public Decimal ValorSindicalizacao
        {
            get { return _valorSindicalizacao; }
            set { _valorSindicalizacao = value; }
        }

        public static TabelaValorVencimentoVO ExisteProposta(Object propostaId, IList<TabelaValorVencimentoVO> lista)
        {
            foreach (TabelaValorVencimentoVO tvv in lista)
            {
                if (Convert.ToString(tvv.PropostaID).Equals(Convert.ToString(propostaId))) { return tvv; }
            }

            return null;
        }

        public static TabelaValorVencimentoVO ExisteTabela(Object tabelaId, IList<TabelaValorVencimentoVO> lista)
        {
            foreach (TabelaValorVencimentoVO tvv in lista)
            {
                if (Convert.ToString(tvv.TabelaID).Equals(Convert.ToString(tabelaId))) { return tvv; }
            }

            return null;
        }
    }

    /// <summary>
    /// Value object.
    /// </summary>
    [Serializable]
    public class SumarioArquivoGeradoVO
    {
        Object _arquivoId;
        String _arquivoNome;
        String _arquivoConteudo;
        String _arquivoVersao;
        Object _operadoraId;
        String _operadoraNome;
        Int32  _qtdCobrancas;

        public Object ArquivoID
        {
            get { return _arquivoId; }
            set { _arquivoId= value; }
        }
        public String ArquivoNome
        {
            get { return _arquivoNome; }
            set { _arquivoNome= value; }
        }
        public String ArquivoConteudo
        {
            get { return _arquivoConteudo; }
            set { _arquivoConteudo= value; }
        }
        public String ArquivoVersao
        {
            get { return _arquivoVersao; }
            set { _arquivoVersao= value; }
        }
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }
        public String OperadoraNome
        {
            get { return _operadoraNome; }
            set { _operadoraNome= value; }
        }
        public Int32 QtdCobrancas
        {
            get { return _qtdCobrancas; }
            set { _qtdCobrancas= value; }
        }
    }

    /// <summary>
    /// Value object.
    /// </summary>
    [Serializable]
    public class CriticaRetornoVO : ICloneable
    {
        //public enum Marcador : int
        //{
        //    Normal,
        //    PropostaInativa,
        //    ValorMenor,
        //    PagamentoRejeitado,
        //    PagamentoEmDuplicidade
        //}

        public Boolean PropostaInativa
        {
            get { return _propostaInativa; }
            set { _propostaInativa= value; }
        }
        public Boolean ValorMenor
        {
            get { return _valorMenor; }
            set { _valorMenor= value; }
        }
        public Boolean PagamentoRejeitado
        {
            get { return _pagamentoRejeicao; }
            set { _pagamentoRejeicao= value; }
        }
        public Boolean EmDuplicidade
        {
            get { return _emDuplicidade; }
            set { _emDuplicidade= value; }
        }
        public Boolean NaoLocalizado
        {
            get { return _naoLocalizado; }
            set { _naoLocalizado= value; }
        }

        Boolean _propostaInativa;
        Boolean _valorMenor;
        Boolean _pagamentoRejeicao;
        Boolean _emDuplicidade;
        Boolean _naoLocalizado;

        String _cobrancaId;
        String _codigoRejeicao;
        String _status;
        String _parcela;
        String _propostaId;
        String _propostaNumero;
        String _cobrancaTipo;
        decimal _taxaBoleto;
        Decimal _valor;
        Decimal _valorPgto;
        DateTime _dataVencto;
        DateTime _dataPgto;
        String _operadoraNome;
        String _nossoNumero;

        String _titularNome;
        String _titularCpf;

        DateTime _dataInativacaoCancelamento;

        public String CobrancaID
        {
            get { return _cobrancaId; }
            set { _cobrancaId= value; }
        }
        public String CodigoRejeicao
        {
            get { return _codigoRejeicao; }
            set { _codigoRejeicao= value; }
        }
        public String Status
        {
            get { return _status; }
            set { _status= value; }
        }
        public String Parcela
        {
            get { return _parcela; }
            set { _parcela= value; }
        }
        public String PropostaCodCobranca
        {
            get { return _propostaId; }
            set { _propostaId= value; }
        }
        public String PropostaNumero
        {
            get { return _propostaNumero; }
            set { _propostaNumero= value; }
        }
        public String CobrancaTipo
        {
            get { return _cobrancaTipo; }
            set { _cobrancaTipo= value; }
        }
        public Decimal TaxaBoleto
        {
            get { return _taxaBoleto; }
            set { _taxaBoleto= value; }
        }
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor= value; }
        }
        public Decimal ValorPgto
        {
            get { return _valorPgto; }
            set { _valorPgto= value; }
        }
        public DateTime DataVencto
        {
            get { return _dataVencto; }
            set { _dataVencto= value; }
        }
        public DateTime DataPgto
        {
            get { return _dataPgto; }
            set { _dataPgto= value; }
        }
        public String OperadoraNome
        {
            get { return _operadoraNome; }
            set { _operadoraNome = value; }
        }
        public String NossoNumero
        {
            get { return _nossoNumero; }
            set { _nossoNumero= value; }
        }

        public String TitularNome
        {
            get { return _titularNome; }
            set { _titularNome= value; }
        }

        public String TitularCPF
        {
            get { return _titularCpf; }
            set { _titularCpf= value; }
        }

        public DateTime DataInativacaoCancelamento
        {
            get { return _dataInativacaoCancelamento; }
            set { _dataInativacaoCancelamento= value; }
        }

        #region ICloneable Members

        public Object Clone()
        {
            CriticaRetornoVO clone = new CriticaRetornoVO();

            clone._cobrancaId = this._cobrancaId;
            clone._cobrancaTipo = this._cobrancaTipo;
            clone._codigoRejeicao = this._codigoRejeicao;
            clone._dataPgto = this._dataPgto;
            clone._dataVencto = this._dataVencto;
            clone._emDuplicidade = this._emDuplicidade;
            clone._nossoNumero = this._nossoNumero;
            clone._operadoraNome = this._operadoraNome;
            clone._pagamentoRejeicao = this._pagamentoRejeicao;
            clone._parcela = this._parcela;
            clone._propostaId = this._propostaId;
            clone._propostaInativa = this._propostaInativa;
            clone._propostaNumero = this._propostaNumero;
            clone._status = this._status;
            clone._valor = this._valor;
            clone._valorMenor = this._valorMenor;
            clone._valorPgto = this._valorPgto;
            clone._titularCpf = this._titularCpf;
            clone._titularNome = this._titularNome;
            clone._dataInativacaoCancelamento = this._dataInativacaoCancelamento;

            return clone;
        }

        #endregion
    }

    [DBTable("retornoinput")]
    public class RetornoInput : EntityBase, IPersisteableEntity
    {
        Object _id;
        String _arquivoNome;
        String _texto;
        DateTime _data;
        Int32 _tipoBanco;
        Boolean _processado;

        [DBFieldInfo("retornoinput_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("retornoinput_arquivo", FieldType.Single)]
        public String ArquivoNome
        {
            get { return _arquivoNome; }
            set { _arquivoNome= value; }
        }

        [DBFieldInfo("retornoinput_texto", FieldType.Single)]
        public String Texto
        {
            get { return _texto; }
            set { _texto= value; }
        }

        [DBFieldInfo("retornoinput_tipoBanco", FieldType.Single)]
        public Int32 TipoBanco
        {
            get { return _tipoBanco; }
            set { _tipoBanco= value; }
        }

        [DBFieldInfo("retornoinput_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [DBFieldInfo("retornoinput_processado", FieldType.Single)]
        public Boolean Processado
        {
            get { return _processado; }
            set { _processado= value; }
        }

        public RetornoInput()
        {
            _tipoBanco = (Int32)Cobranca.eTipoBanco.Itau;
        }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        #endregion

        public static RetornoInput CarregarPendente()
        {
            String qry = "SELECT TOP 1 * FROM retornoinput WHERE retornoinput_processado=0 order by retornoinput_id";

            IList<RetornoInput> ret = LocatorHelper.Instance.ExecuteQuery<RetornoInput>(qry, typeof(RetornoInput));

            if (ret == null)
                return null;
            else
                return ret[0];
        }
    }

    [DBTable("retornoOutput")]
    public class RetornoOutput : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _inputId;
        String _descricao;
        String _serializedValueObject;
        String _serializedBusinessObject;
        DateTime _data;

        [DBFieldInfo("retornooutput_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("retornooutput_inputId", FieldType.Single)]
        public Object InputID
        {
            get { return _inputId; }
            set { _inputId= value; }
        }

        [DBFieldInfo("retornooutpu_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("retornooutpu_serializedVo", FieldType.Single)]
        public String SerializedValueObject
        {
            get { return _serializedValueObject; }
            set { _serializedValueObject= value; }
        }

        [DBFieldInfo("retornooutpu_serializedBo", FieldType.Single)]
        public String SerializedBusinessObject
        {
            get { return _serializedBusinessObject; }
            set { _serializedBusinessObject= value; }
        }

        [DBFieldInfo("retornooutput_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public static List<CriticaRetornoVO> Desserializar(String data)
        {
            String[] arr = data.Split('|');

            List<CriticaRetornoVO> vos = new List<CriticaRetornoVO>();

            String[] current = null;
            foreach (String strvo in arr)
            {
                if (strvo.Length == 0) { continue; }
                current = strvo.Split(';');

                CriticaRetornoVO vo = new CriticaRetornoVO();

                vo.CobrancaID = current[0];
                vo.CobrancaTipo = current[1];
                vo.CodigoRejeicao = current[2];
                vo.DataPgto = Convert.ToDateTime(current[3]);
                vo.DataVencto = Convert.ToDateTime(current[4]);
                vo.EmDuplicidade = Convert.ToBoolean(current[5]);
                vo.NossoNumero = current[6];
                vo.OperadoraNome = current[7];
                vo.PagamentoRejeitado = Convert.ToBoolean(current[8]);
                vo.Parcela = current[9];
                vo.PropostaCodCobranca = current[10];
                vo.PropostaInativa = Convert.ToBoolean(current[11]);
                vo.PropostaNumero = current[12];
                vo.Status = current[13];
                vo.Valor = Convert.ToDecimal(current[14]);
                vo.ValorMenor = Convert.ToBoolean(current[15]);
                vo.ValorPgto = Convert.ToDecimal(current[16]);

                if (current.Length > 17)
                {
                    vo.TitularCPF = current[18];
                    vo.TitularNome = current[17];
                }

                if (current.Length > 19 && current[19].Trim() != "")
                    vo.DataInativacaoCancelamento = Convert.ToDateTime(current[19]);

                vos.Add(vo);
            }

            return vos;
        }

        public static String Serializar(List<CriticaRetornoVO> vos)
        {
            StringBuilder sb = new StringBuilder();

            foreach (CriticaRetornoVO vo in vos)
            {
                if (sb.Length > 0) { sb.Append("|"); }

                sb.Append(vo.CobrancaID);
                sb.Append(";");
                sb.Append(vo.CobrancaTipo);
                sb.Append(";");
                sb.Append(vo.CodigoRejeicao);
                sb.Append(";");
                sb.Append(vo.DataPgto.ToString("dd/MM/yyyy HH:mm"));
                sb.Append(";");
                sb.Append(vo.DataVencto.ToString("dd/MM/yyyy HH:mm"));
                sb.Append(";");
                sb.Append(vo.EmDuplicidade);
                sb.Append(";");
                sb.Append(vo.NossoNumero);
                sb.Append(";");
                sb.Append(vo.OperadoraNome);
                sb.Append(";");
                sb.Append(vo.PagamentoRejeitado);
                sb.Append(";");
                sb.Append(vo.Parcela);
                sb.Append(";");
                sb.Append(vo.PropostaCodCobranca);
                sb.Append(";");
                sb.Append(vo.PropostaInativa);
                sb.Append(";");
                sb.Append(vo.PropostaNumero);
                sb.Append(";");
                sb.Append(vo.Status);
                sb.Append(";");
                sb.Append(vo.Valor.ToString("N2"));
                sb.Append(";");
                sb.Append(vo.ValorMenor);
                sb.Append(";");
                sb.Append(vo.ValorPgto.ToString("N2"));
                sb.Append(";");
                sb.Append(vo.TitularNome);
                sb.Append(";");
                sb.Append(vo.TitularCPF);
                sb.Append(";");
                if (vo.DataInativacaoCancelamento != DateTime.MinValue)
                    sb.Append(vo.DataInativacaoCancelamento.ToString("dd/MM/yyyy"));
            }

            return sb.ToString();
        }

        public System.Collections.ArrayList ValueObjects
        {
            get
            {
                if (_serializedValueObject == null) { return null; }
                return (System.Collections.ArrayList)LC.Framework.DataUtil.SerializationHelper.Desserializar(_serializedValueObject);
            }
        }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Salvar(IList<CriticaRetornoVO> vos)
        {
            this.Salvar();

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            try
            {
                foreach (CriticaRetornoVO vo in vos)
                {
                    if (vo.Status != "Título baixado") { continue; }
                    NonQueryHelper.Instance.ExecuteNonQuery(String.Concat("insert into cobranca_titulosBaixados values (", vo.CobrancaID, ")"), pm);
                }
            }
            catch { }

            pm.CloseSingleCommandInstance();
            pm.Dispose();
        }

        public void SalvarDescricao()
        {
            NonQueryHelper.Instance.ExecuteNonQuery("UPDATE retornoOutput SET retornooutput_descricao='" + this._descricao.ToUpper() + "' WHERE retornooutput_id=" + this._id, null);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        #endregion

        public static IList<RetornoOutput> CarregarTodos()
        {
            String qry = "TOP 75 retornooutput_id, retornooutpu_descricao, retornooutput_data FROM retornoOutput ORDER BY retornooutput_id DESC";

            return LocatorHelper.Instance.ExecuteQuery<RetornoOutput>(qry, typeof(RetornoOutput));
        }

        public static IList<RetornoOutput> CarregarTodos(DateTime de, DateTime ate)
        {
            String qry = String.Concat("retornooutput_id, retornooutpu_descricao, retornooutput_data ",
                "   FROM retornoOutput ",
                "   WHERE retornooutput_data BETWEEN '", de.ToString("yyyy-MM-dd"), "' AND '", ate.ToString("yyyy-MM-dd 23:59:59.990"), "' ",
                "   ORDER BY retornooutput_id DESC");

            return LocatorHelper.Instance.ExecuteQuery<RetornoOutput>(qry, typeof(RetornoOutput));
        }

        public static RetornoOutput CarregarPorInputID(Object inputId)
        {
            String qry = String.Concat("retornooutput_id, retornooutpu_descricao, retornooutput_data ",
                "   FROM retornoOutput ",
                "   WHERE retornooutput_inputId=", inputId);

            IList<RetornoOutput> ret = LocatorHelper.Instance.ExecuteQuery<RetornoOutput>(qry, typeof(RetornoOutput));

            if (ret == null)
                return null;
            else
                return ret[0];
        }
    }

    [DBTable("arquivoRemessaCriterio")]
    public class ArquivoRemessaCriterio : EntityBase, IPersisteableEntity
    {
        public enum eTipoTaxa : int
        {
            Indiferente,
            ComTaxa,
            SemTaxa
        }

        public class UI
        {
            private UI() { }

            public static void PreencheComboComTiposFiltroTaxa(System.Web.UI.WebControls.DropDownList combo)
            {
                combo.Items.Clear();
                combo.Items.Add(new System.Web.UI.WebControls.ListItem("Todos", "0"));
                combo.Items.Add(new System.Web.UI.WebControls.ListItem("Apenas COM taxa associativa", "1"));
                combo.Items.Add(new System.Web.UI.WebControls.ListItem("Apenas SEM taxa associativa", "2"));
            }
        }

        #region fields 

        Object _id;
        Object _operadoraId;
        String _projeto;
        String _arquivo;
        String _descricao;
        String _foneAtendimento;
        String _operadora;
        String _ans;
        String _contratoAdmIDs;
        Int32 _tipoTaxa;

        String _operadoraNome;

        #endregion

        #region properties 

        [DBFieldInfo("arcrit_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("arcrit_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("arcrit_projeto", FieldType.Single)]
        public String Projeto
        {
            get { return _projeto; }
            set { _projeto= value; }
        }

        [DBFieldInfo("arcrit_arquivoNome", FieldType.Single)]
        public String ArquivoNome
        {
            get { return _arquivo; }
            set { _arquivo = value; }
        }

        [DBFieldInfo("arcrit_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("arcrit_foneAtendimento", FieldType.Single)]
        public String FoneAtendimento
        {
            get { return _foneAtendimento; }
            set { _foneAtendimento= value; }
        }

        [DBFieldInfo("arcrit_operadora", FieldType.Single)]
        public String Operadora
        {
            get { return _operadora; }
            set { _operadora= value; }
        }

        [DBFieldInfo("arcrit_ans", FieldType.Single)]
        public String Ans
        {
            get { return _ans; }
            set { _ans= value; }
        }

        /// <summary>
        /// Uma string com os ids de contratos administrativos separados por vírgula
        /// </summary>
        [DBFieldInfo("arcrit_contratoAdmIds", FieldType.Single)]
        public String ContratoAdmIDs
        {
            get { return _contratoAdmIDs; }
            set { _contratoAdmIDs= value; }
        }

        [DBFieldInfo("arcrit_tipoTaxa", FieldType.Single)]
        public Int32 TipoTaxa
        {
            get { return _tipoTaxa; }
            set { _tipoTaxa= value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraNome
        {
            get { return _operadoraNome; }
            set { _operadoraNome = value; }
        }

        #endregion

        public ArquivoRemessaCriterio() { }
        public ArquivoRemessaCriterio(Object id) { _id = id; }

        #region EntityBase methods

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        #endregion

        public static IList<ArquivoRemessaCriterio> CarregarTodos()
        {
            String qry = "arquivoRemessaCriterio.*, operadora_nome FROM arquivoRemessaCriterio INNER JOIN operadora ON arcrit_operadoraId=operadora_id ORDER BY operadora_nome ";

            return LocatorHelper.Instance.ExecuteQuery<ArquivoRemessaCriterio>(qry, typeof(ArquivoRemessaCriterio));
        }

        public static String[] CarregarContratoAdmIds(Object criterioId, PersistenceManager pm)
        {
            String ret = Convert.ToString(LocatorHelper.Instance.ExecuteScalar("SELECT arcrit_contratoAdmIds FROM arquivoRemessaCriterio WHERE arcrit_id=" + criterioId, null, null, pm));

            return ret.Split(',');
        }
    }

    [DBTable("arquivoRemessaAgendamento")]
    public class ArquivoRemessaAgendamento : EntityBase, IPersisteableEntity
    {
        #region fields

        Object _id;
        Object _criterioId;
        DateTime _vencimentoDe;
        DateTime _vencimentoAte;
        DateTime _vigenciaDe;
        DateTime _vigenciaAte;
        DateTime _processamentoEm;
        Int32 _qtdBoletos;
        Boolean _processado;
        DateTime _dataProcessado;
        String _grupo;
        Int32 _carteira;
        String _arquivoNomeInstancia;
        bool _legado;

        String _arquivoNome;
        String _criterioDescricao;
        Int32 _arquivoCobrancaId;

        #endregion

        #region properties 

        [DBFieldInfo("arcage_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("arcage_criterioId", FieldType.Single)]
        public Object CriterioID
        {
            get { return _criterioId; }
            set { _criterioId= value; }
        }

        [DBFieldInfo("arcage_vencimentoDe", FieldType.Single)]
        public DateTime VencimentoDe
        {
            get { return _vencimentoDe; }
            set { _vencimentoDe= value; }
        }

        [DBFieldInfo("arcage_vencimentoAte", FieldType.Single)]
        public DateTime VencimentoAte
        {
            get { return _vencimentoAte; }
            set { _vencimentoAte= value; }
        }

        [DBFieldInfo("arcage_vigenciaDe", FieldType.Single)]
        public DateTime VigenciaDe
        {
            get { return _vigenciaDe; }
            set { _vigenciaDe= value; }
        }

        [DBFieldInfo("arcage_vigenciaAte", FieldType.Single)]
        public DateTime VigenciaAte
        {
            get { return _vigenciaAte; }
            set { _vigenciaAte= value; }
        }

        [DBFieldInfo("arcage_qtdBoletos", FieldType.Single)]
        public Int32 QtdBoletos
        {
            get { return _qtdBoletos; }
            set { _qtdBoletos= value; }
        }

        [DBFieldInfo("arcage_legado", FieldType.Single)]
        public bool Legado
        {
            get { return _legado; }
            set { _legado= value; }
        }

        /// <summary>
        /// Se legado, retorna SP. Do contrário retorna RJ.
        /// </summary>
        public string Origem
        {
            get
            {
                if (this._legado) return "SP";
                else              return "RJ";
            }
        }

        [DBFieldInfo("arcage_grupo", FieldType.Single)]
        public String Grupo
        {
            get { return _grupo; }
            set { _grupo= value; }
        }

        /// <summary>
        /// Data para a qual foi agendado o processamento.
        /// </summary>
        [DBFieldInfo("arcage_processamentoEm", FieldType.Single)]
        public DateTime ProcessamentoEm
        {
            get { return _processamentoEm; }
            set { _processamentoEm= value; }
        }

        [DBFieldInfo("arcage_processado", FieldType.Single)]
        public Boolean Processado
        {
            get { return _processado; }
            set { _processado= value; }
        }

        /// <summary>
        /// Data em que efetivamente ocorreu o processamento.
        /// </summary>
        [DBFieldInfo("arcage_dataProcessado", FieldType.Single)]
        public DateTime DataProcessado
        {
            get { return _dataProcessado; }
            set { _dataProcessado= value; }
        }

        [DBFieldInfo("arcage_carteira", FieldType.Single)]
        public Int32 Carteira
        {
            get { return _carteira; }
            set { _carteira= value; }
        }


        [DBFieldInfo("arcage_banco", FieldType.Single)]
        public Int32 Banco
        {
            get;
            set;
        }

        [DBFieldInfo("arcage_arquivoNomeInstancia", FieldType.Single)]
        public String ArquivoNomeInstance
        {
            get { return _arquivoNomeInstancia; }
            set
            {
                if (value != null && value != "[nenhum arquivo gerado - 0 cobraças geradas]") { value = value.Replace(@"/", "_").Replace(@"\", "_").Replace(" ", "_"); }
                _arquivoNomeInstancia = value;
            }
        }

        /// <summary>
        /// Mesmo que DIFERENCA UBRASP
        /// </summary>
        [DBFieldInfo("arcage_somenteNaoRecorrentes", FieldType.Single)]
        public bool SomenteNaoRecorrentes
        {
            get;
            set;
        }

        [DBFieldInfo("arcage_somenteBoletoUbrasp", FieldType.Single)]
        public bool SomenteBoletosUBRASP
        {
            get;
            set;
        }

        [Joinned("arcrit_arquivoNome")]
        public String ArquivoNome
        {
            get { return _arquivoNome; }
            set { _arquivoNome= value; }
        }

        [Joinned("arcrit_descricao")]
        public String CriterioDescricao
        {
            get { return _criterioDescricao; }
            set { _criterioDescricao= value; }
        }

        [Joinned("arquivocobranca_id")]
        public Int32 ArquivoCobrancaID
        {
            get { return _arquivoCobrancaId;  }
            set { _arquivoCobrancaId = value; }
        }

        public String STRVencimento
        {
            get
            {
                if (_vencimentoDe.ToString("dd/MM/yyyy") != _vencimentoAte.ToString("dd/MM/yyyy"))
                    return String.Concat("de ", _vencimentoDe.ToString("dd/MM/yyyy"), " até ", _vencimentoAte.ToString("dd/MM/yyyy"));
                else
                    return _vencimentoDe.ToString("dd/MM/yyyy");
            }
        }

        public String STRVigencia
        {
            get
            {
                if (_vigenciaDe.ToString("dd/MM/yyyy") != _vigenciaAte.ToString("dd/MM/yyyy"))
                    return String.Concat("de ", _vigenciaDe.ToString("dd/MM/yyyy"), " até ", _vigenciaAte.ToString("dd/MM/yyyy"));
                else
                    return _vigenciaDe.ToString("dd/MM/yyyy");
            }
        }

        #endregion

        public ArquivoRemessaAgendamento() { _legado = false; _processado = false; _carteira = (Int32)Cobranca.eCarteira.Unibanco; }
        public ArquivoRemessaAgendamento(Object id) : this() { _id = id; }

        #region EntityBase methods 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        #endregion

        public static IList<ArquivoRemessaAgendamento> CarregarTodos(Boolean somenteNaoProcessados, DateTime? dataProcessamento)
        {
            String cond = "";

            if (somenteNaoProcessados)
                cond = " WHERE arcage_processado=0 and arcage_legado=0 ";

            if (dataProcessamento != null)
            {
                if (cond.Length == 0)
                    cond = " WHERE ";
                else
                    cond += " AND ";

                cond += " GETDATE() >= arcage_processamentoEm ";
            }

            String qry = " arquivoRemessaAgendamento.*, arcrit_arquivoNome FROM arquivoRemessaAgendamento INNER JOIN arquivoRemessaCriterio ON arcage_criterioId=arcrit_id " + cond + " ORDER BY arcage_processamentoEm";

            return LocatorHelper.Instance.ExecuteQuery<ArquivoRemessaAgendamento>(qry, typeof(ArquivoRemessaAgendamento));
        }

        public static IList<ArquivoRemessaAgendamento> CarregarTodosLEGADO(Boolean somenteNaoProcessados, DateTime? dataProcessamento)
        {
            String cond = "";

            if (somenteNaoProcessados)
                cond = " WHERE arcage_processado=0 and arcage_legado=1 ";

            if (dataProcessamento != null)
            {
                if (cond.Length == 0)
                    cond = " WHERE ";
                else
                    cond += " AND ";

                cond += " GETDATE() >= arcage_processamentoEm ";
            }

            String qry = " arquivoRemessaAgendamento.*, arcrit_arquivoNome FROM arquivoRemessaAgendamento left JOIN arquivoRemessaCriterio ON arcage_criterioId=arcrit_id " + cond + " ORDER BY arcage_processamentoEm";

            return LocatorHelper.Instance.ExecuteQuery<ArquivoRemessaAgendamento>(qry, typeof(ArquivoRemessaAgendamento));
        }

        public static IList<ArquivoRemessaAgendamento> CarregarTodos(Object id)
        {
            String qry = "arquivoRemessaAgendamento.*, arcrit_arquivoNome FROM arquivoRemessaAgendamento LEFT JOIN arquivoRemessaCriterio ON arcage_criterioId=arcrit_id WHERE arcage_id =  " + id; // " arquivoRemessaAgendamento.*, arcrit_arquivoNome FROM arquivoRemessaAgendamento INNER JOIN arquivoRemessaCriterio ON arcage_criterioId=arcrit_id " + cond + " ORDER BY arcage_processamentoEm DESC";

            return LocatorHelper.Instance.ExecuteQuery<ArquivoRemessaAgendamento>(qry, typeof(ArquivoRemessaAgendamento));
        }

        public static IList<ArquivoRemessaAgendamento> CarregarTodos(Boolean somenteProcessados, DateTime? dataProcessamento, DateTime? venctoDe, DateTime? venctoAte, DateTime? vigenciaDe, DateTime? vigenciaAte)
        {
            String condSomenteProcessado = "";
            String condProcessadoEm = "";
            String condVencto = "";
            String condVigencia = "";

            if (somenteProcessados)
                condSomenteProcessado = " arcage_processado=1 ";

            if(dataProcessamento != null)
                condProcessadoEm += String.Concat(" DAY(arcage_processamentoEm)=", dataProcessamento.Value.Day, " AND MONTH(arcage_processamentoEm)=", dataProcessamento.Value.Month, "  AND YEAR(arcage_processamentoEm)=", dataProcessamento.Value.Year);

            if (venctoDe != null && venctoAte != null)
                condVencto = String.Concat(" arcage_vencimentoDe >= '", venctoDe.Value.ToString("yyyy-MM-dd 00:00:00.000"), "' AND arcage_vencimentoAte <= '", venctoAte.Value.ToString("yyyy-MM-dd 23:59:59.998"), "' ");

            if (vigenciaDe != null && vigenciaAte != null)
                condVencto = String.Concat(" arcage_vigenciaDe >= '", vigenciaDe.Value.ToString("yyyy-MM-dd 00:00:00.000"), "' AND arcage_vigenciaAte <= '", vigenciaAte.Value.ToString("yyyy-MM-dd 23:59:59.998"), "' ");

            String cond = "";
            if (condSomenteProcessado != "" || condProcessadoEm != "" || condVencto != "" || condVigencia != "")
            {
                String and = " and ";
                cond = " WHERE (arcage_arquivoNomeInstancia IS NOT NULL AND arcage_arquivoNomeInstancia <> '') ";

                if (condSomenteProcessado != "") { cond += and + condSomenteProcessado; and = " and "; }
                if (condProcessadoEm != "") { cond += and + condProcessadoEm; and = " and "; }
                if (condVencto != "") { cond += and + condVencto; and = " and "; }
                if (condVigencia != "") { cond += and + condVigencia; }
            }

            //String qry = " arquivoRemessaAgendamento.*, arcrit_descricao,arcrit_arquivoNome FROM arquivoRemessaAgendamento INNER JOIN arquivoRemessaCriterio ON arcage_criterioId=arcrit_id " + cond + " ORDER BY arcage_processamentoEm DESC";
            String qry = String.Concat("SELECT arcage_arquivoNomeInstancia,arcage_qtdBoletos,arcage_vigenciade,arcage_vigenciaAte,arcage_vencimentoDe,arcage_vencimentoAte FROM ",
                "   arquivoRemessaAgendamento ",
                "       INNER JOIN arquivoRemessaCriterio ON arcage_criterioId=arcrit_id " ,
                cond,
                "   GROUP BY arcage_arquivoNomeInstancia,arcage_qtdBoletos, arcage_vigenciade,arcage_vigenciaAte,arcage_vencimentoDe,arcage_vencimentoAte ",
                "   ORDER BY arcage_arquivoNomeInstancia DESC");

            return LocatorHelper.Instance.ExecuteQuery<ArquivoRemessaAgendamento>(qry, typeof(ArquivoRemessaAgendamento));
        }

        public static IList<ArquivoRemessaAgendamento> CarregarTodosMesmoSemCobrancas(Boolean somenteProcessados, DateTime? dataProcessamento, DateTime? venctoDe, DateTime? venctoAte, DateTime? vigenciaDe, DateTime? vigenciaAte)
        {
            String condSomenteProcessado = "";
            String condProcessadoEm = "";
            String condVencto = "";
            String condVigencia = "";

            if (somenteProcessados)
                condSomenteProcessado = " arcage_processado=1 ";

            if (dataProcessamento != null)
                condProcessadoEm += String.Concat(" DAY(arcage_processamentoEm)=", dataProcessamento.Value.Day, " AND MONTH(arcage_processamentoEm)=", dataProcessamento.Value.Month, "  AND YEAR(arcage_processamentoEm)=", dataProcessamento.Value.Year);

            if (venctoDe != null && venctoAte != null)
                condVencto = String.Concat(" arcage_vencimentoDe >= '", venctoDe.Value.ToString("yyyy-MM-dd 00:00:00.000"), "' AND arcage_vencimentoAte <= '", venctoAte.Value.ToString("yyyy-MM-dd 23:59:59.998"), "' ");

            if (vigenciaDe != null && vigenciaAte != null)
                condVencto = String.Concat(" arcage_vigenciaDe >= '", vigenciaDe.Value.ToString("yyyy-MM-dd 00:00:00.000"), "' AND arcage_vigenciaAte <= '", vigenciaAte.Value.ToString("yyyy-MM-dd 23:59:59.998"), "' ");

            String cond = "", top = "";
            if (condSomenteProcessado != "" || condProcessadoEm != "" || condVencto != "" || condVigencia != "")
            {
                String and = " and ";
                cond = " where arcage_id > 0 "; //" WHERE (arcage_arquivoNomeInstancia IS NOT NULL or arcage_arquivoNomeInstancia IS NULL) ";

                if (condSomenteProcessado != "") { cond += and + condSomenteProcessado; and = " and "; }
                if (condProcessadoEm != "") { cond += and + condProcessadoEm; and = " and "; }
                if (condVencto != "") { cond += and + condVencto; and = " and "; }
                if (condVigencia != "") { cond += and + condVigencia; }
            }
            
            if(condProcessadoEm == "" && condVencto == "" && condVigencia == "") top = " top 30 ";

          //String qry = " arquivoRemessaAgendamento.*, arcrit_descricao,arcrit_arquivoNome FROM arquivoRemessaAgendamento INNER JOIN arquivoRemessaCriterio ON arcage_criterioId=arcrit_id " + cond + " ORDER BY arcage_processamentoEm DESC";
            String qry = String.Concat("SELECT ", top, " arcage_arquivoNomeInstancia,arcage_qtdBoletos,arcage_vigenciade,arcage_vigenciaAte,arcage_vencimentoDe,arcage_vencimentoAte,arquivocobranca_id, arcage_processamentoEm,arcage_legado FROM ",
                "   arquivoRemessaAgendamento ",
                "       left JOIN arquivoRemessaCriterio ON arcage_criterioId=arcrit_id ",
                "       INNER JOIN arquivoCobrancaUnibanco ON arcage_id=arquivocobranca_agendamentoId ",
                cond,
                "   GROUP BY arcage_arquivoNomeInstancia,arcage_qtdBoletos, arcage_vigenciade,arcage_vigenciaAte,arcage_vencimentoDe,arcage_vencimentoAte,arquivocobranca_id,arcage_processamentoEm,arcage_legado ",
                "   ORDER BY arquivocobranca_id DESC");

            return LocatorHelper.Instance.ExecuteQuery<ArquivoRemessaAgendamento>(qry, typeof(ArquivoRemessaAgendamento));
        }

        public static IList<ArquivoRemessaAgendamento> CarregarUltimos20Legado()
        {
            //String condSomenteProcessado = "";
            //String condProcessadoEm = "";
            //String condVencto = "";
            //String condVigencia = "";

            String cond = " where arcage_legado=1 ";
            //if (condSomenteProcessado != "" || condProcessadoEm != "" || condVencto != "" || condVigencia != "")
            //{
            //    String and = " and ";
            //    cond = " WHERE (arcage_arquivoNomeInstancia IS NOT NULL AND arcage_arquivoNomeInstancia <> '') ";

            //    if (condSomenteProcessado != "") { cond += and + condSomenteProcessado; and = " and "; }
            //    if (condProcessadoEm != "") { cond += and + condProcessadoEm; and = " and "; }
            //    if (condVencto != "") { cond += and + condVencto; and = " and "; }
            //    if (condVigencia != "") { cond += and + condVigencia; }
            //}

            String qry = String.Concat("SELECT top 20 arcage_arquivoNomeInstancia,arcage_qtdBoletos,arcage_vigenciade,arcage_vigenciaAte,arcage_vencimentoDe,arcage_vencimentoAte FROM ",
                "   arquivoRemessaAgendamento ",
                "       LEFT JOIN arquivoRemessaCriterio ON arcage_criterioId=arcrit_id ",
                cond,
                "   GROUP BY arcage_arquivoNomeInstancia,arcage_qtdBoletos, arcage_vigenciade,arcage_vigenciaAte,arcage_vencimentoDe,arcage_vencimentoAte ",
                "   ORDER BY arcage_arquivoNomeInstancia DESC");

            return LocatorHelper.Instance.ExecuteQuery<ArquivoRemessaAgendamento>(qry, typeof(ArquivoRemessaAgendamento));
        }

        public static String NovoGrupo()
        {
            String qry = "SELECT TOP 1 arcage_grupo FROM arquivoRemessaAgendamento WHERE arcage_processado=0 ORDER BY arcage_id DESC";

            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, null, null);

            if (ret == null || ret == DBNull.Value)
                return "1";
            else
                return Convert.ToString(Convert.ToInt32(ret) + 1);
        }
    }

    internal sealed class CobrancaFalha
    {
        CobrancaFalha() { }

        public static void LogFalha(Object propostaId, DateTime? vencimento, String descricao, PersistenceManager pm, object agendaId)
        {
            try
            {
                if (descricao == null) { descricao = ""; }
                String vencto = " NULL ";
                if (vencimento != null && vencimento.HasValue && vencimento.Value != DateTime.MinValue)
                {
                    vencto = String.Concat("'", vencimento.Value.ToString("yyyy-MM-dd"), "'");
                }

                if (agendaId != null)
                {
                    String cmd = String.Concat("insert into cobranca_falha (cobrancafalha_propostaId,cobrancafalha_vencimento,cobrancafalha_motivo, cobrancafalha_agendaId) values (", propostaId, ",", vencto, ", '", descricao.Replace("'", ""), "',", agendaId, ")");
                    NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);
                }
                else
                {
                    String cmd = String.Concat("insert into cobranca_falha (cobrancafalha_propostaId,cobrancafalha_vencimento,cobrancafalha_motivo) values (", propostaId, ",", vencto, ", '", descricao.Replace("'", ""), "')");
                    NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);
                }
            }
            catch { }
        }

        public static void LogFalhaTabelaValor(Object propostaId, DateTime? venvimento, PersistenceManager pm, object agendaId = null)
        {
            LogFalha(propostaId, venvimento, "Tabela de valor não encontrada", pm, agendaId);
        }
    }

    [DBTable("cobranca_venctoLog")]
    internal sealed class CobrancaVencimentoLog : IPersisteableEntity
    {
        Object _cobrancaId;
        DateTime _venctoOriginal;
        DateTime _venctoNovo;
        Object _usuarioId;
        DateTime _data;

        public CobrancaVencimentoLog() { _data = DateTime.Now; }

        public Object ID
        {
            get { return null; }
            set { }
        }

        [DBFieldInfo("cobrancavectolog_cobrancaId", FieldType.Single)]
        public Object CobrancaID
        {
            get { return _cobrancaId; }
            set { _cobrancaId= value; }
        }

        [DBFieldInfo("cobrancavectolog_venctoOriginal", FieldType.Single)]
        public DateTime VenctoOriginal
        {
            get { return _venctoOriginal; }
            set { _venctoOriginal= value; }
        }

        [DBFieldInfo("cobrancavectolog_venctoNovo", FieldType.Single)]
        public DateTime VenctoNovo
        {
            get { return _venctoNovo; }
            set { _venctoNovo= value; }
        }

        [DBFieldInfo("cobrancavectolog_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("cobrancavectolog_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }
    }

    /// <summary>
    /// Representa o cabeçalho do parcelamento de dívida.
    /// </summary>
    [DBTable("cobranca_parcelamentoHeader")]
    public class ParcelamentoHeader : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _contratoId;
        Object _empresaId;
        Object _usuarioId;
        DateTime _venctoInicial;
        String _desconto;
        String _multa;
        String _email;
        Boolean _isentajuros;
        Int32 _parcelas;
        String _obs;
        Decimal _valorTotal;
        DateTime _data;

        #region properties 

        [DBFieldInfo("parcheader_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("parcheader_contratoId", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId= value; }
        }

        [DBFieldInfo("parcheader_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("parcheader_empresaId", FieldType.Single)]
        public Object EmpresaID
        {
            get { return _empresaId; }
            set { _empresaId= value; }
        }

        [DBFieldInfo("parcheader_venctoInicial", FieldType.Single)]
        public DateTime VenctoInicial
        {
            get { return _venctoInicial; }
            set { _venctoInicial= value; }
        }

        [DBFieldInfo("parcheader_desconto", FieldType.Single)]
        public String Desconto
        {
            get { return _desconto; }
            set { _desconto= value; }
        }

        [DBFieldInfo("parcheader_parcelas", FieldType.Single)]
        public Int32 Parcelas
        {
            get { return _parcelas; }
            set { _parcelas= value; }
        }

        [DBFieldInfo("parcheader_multa", FieldType.Single)]
        public String Multa
        {
            get { return _multa; }
            set { _multa= value; }
        }


        [DBFieldInfo("parcheader_email", FieldType.Single)]
        public String Email
        {
            get { return _email; }
            set { _email= value; }
        }

        [DBFieldInfo("parcheader_isentaJuros", FieldType.Single)]
        public Boolean IsentaJuros
        {
            get { return _isentajuros; }
            set { _isentajuros= value; }
        }

        [DBFieldInfo("parcheader_obs", FieldType.Single)]
        public String OBS
        {
            get { return _obs; }
            set { _obs= value; }
        }

        [DBFieldInfo("parcheader_valorTotal", FieldType.Single)]
        public Decimal ValorTotal
        {
            get { return _valorTotal; }
            set { _valorTotal= value; }
        }

        [DBFieldInfo("parcheader_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        #endregion

        public ParcelamentoHeader() { _data = DateTime.Now; }
        public ParcelamentoHeader(Object id) : this() { _id = id; }

        public static String GetEmpresaNome(Object empresaId)
        {
            String qry = "select empresa_nome from cobranca_empresa where empresa_id=" + empresaId;

            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, null, null);
            if (ret != null && ret != DBNull.Value)
                return Convert.ToString(ret);
            else
                return String.Empty;
        }

        #region EntityBase methods 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        #endregion 
    }

    /// <summary>
    /// Mantém o relacionamento entre as novas cobranças geradas e o cabeçalho do parcelamento.
    /// </summary>
    [DBTable("cobranca_parcelamentoItem")]
    public class ParcelamentoItem : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _headerId;
        Object _cobrancaId;
        DateTime _competencia;
        String _obs;

        #region properties 

        [DBFieldInfo("parcitem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("parcitem_headerId", FieldType.Single)]
        public Object HeaderID
        {
            get { return _headerId; }
            set { _headerId= value; }
        }

        [DBFieldInfo("parcitem_cobrancaId", FieldType.Single)]
        public Object CobrancaID
        {
            get { return _cobrancaId; }
            set { _cobrancaId= value; }
        }

        [DBFieldInfo("parcitem_competencia", FieldType.Single)]
        public DateTime Competencia
        {
            get { return _competencia; }
            set { _competencia= value; }
        }

        [DBFieldInfo("parcitem_obs", FieldType.Single)]
        public String Observacoes
        {
            get { return _obs; }
            set { _obs= value; }
        }

        #endregion

        public static ParcelamentoItem CarregarPorCobrancaId(Object cobrancaId)
        {
            String qry = "select * from cobranca_parcelamentoItem where parcitem_cobrancaId=" + cobrancaId;

            IList<ParcelamentoItem> itens = LocatorHelper.Instance.ExecuteQuery<ParcelamentoItem>(qry, typeof(ParcelamentoItem));
            if (itens == null)
                return null;
            else
                return itens[0];
        }

        public static IList<ParcelamentoItem> CarregarPorHeaderId(Object headerId, PersistenceManager pm)
        {
            String qry = "select * from cobranca_parcelamentoItem where parcitem_headerId=" + headerId;

            return LocatorHelper.Instance.ExecuteQuery<ParcelamentoItem>(qry, typeof(ParcelamentoItem), pm);
        }

        public static IList<Cobranca> CarregarParcelasGeradas(Object headerId)
        {
            String qry = String.Concat("* from cobranca ",
                "       inner join cobranca_parcelamentoItem on parcitem_cobrancaId=cobranca_id ",
                "       inner join cobranca_parcelamentoHeader on parcitem_headerId = parcheader_id ",
                "   where parcheader_id=", headerId,
                "   order by cobranca_parcela");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca));
        }
    }

    /// <summary>
    /// Mantém o relacionamento entre as cobranças originais parceladas e o cabeçalho do parcelamento.
    /// </summary>
    [DBTable("cobranca_parcelamentoCobrancaOriginal")]
    public class ParcelamentoCobrancaOriginal : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _headerId;
        Object _cobrancaId;

        #region properties

        [DBFieldInfo("parccob_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("parccob_headerId", FieldType.Single)]
        public Object HeaderID
        {
            get { return _headerId; }
            set { _headerId= value; }
        }

        [DBFieldInfo("parccob_cobrancaId", FieldType.Single)]
        public Object CobrancaID
        {
            get { return _cobrancaId; }
            set { _cobrancaId= value; }
        }

        #endregion

        public static ParcelamentoCobrancaOriginal CarregarPorCobrancaId(Object cobrancaId)
        {
            String qry = "select * from cobranca_parcelamentoCobrancaOriginal where parccob_cobrancaId=" + cobrancaId;

            IList<ParcelamentoCobrancaOriginal> itens = LocatorHelper.Instance.ExecuteQuery<ParcelamentoCobrancaOriginal>(qry, typeof(ParcelamentoCobrancaOriginal));
            if (itens == null)
                return null;
            else
                return itens[0];
        }

        public static IList<ParcelamentoCobrancaOriginal> CarregarPorHeaderId(Object headerId, PersistenceManager pm)
        {
            String qry = "select * from cobranca_parcelamentoCobrancaOriginal where parccob_headerId=" + headerId;

            return LocatorHelper.Instance.ExecuteQuery<ParcelamentoCobrancaOriginal>(qry, typeof(ParcelamentoCobrancaOriginal), pm);
        }

        public static IList<Cobranca> CarregarParcelasNegociadas(Object headerId)
        {
            String qry = String.Concat("* from cobranca ",
                "       inner join cobranca_parcelamentoCobrancaOriginal on parccob_cobrancaId=cobranca_id ",
                "       inner join cobranca_parcelamentoHeader on parccob_headerId = parcheader_id ",
                "   where parcheader_id=", headerId,
                "   order by cobranca_parcela");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca));
        }
    }

    public class CobrancaLog
    {
        public enum Fonte : int
        {
            Sistema,
            Site,
            URA
        }

        public void CobrancaEnviada(Object cobrancaId,Object usuarioId, Fonte fonte)
        {
            try
            {
                String strUsuarioId = " NULL ";
                if (usuarioId != null) { strUsuarioId = Convert.ToString(usuarioId); }

                String cmd = String.Concat("insert into log_cobrancaEnviada (logenvcob_boletoId,logenvcob_usuarioId,logenvcob_fonte,logenvcob_data) values (",
                    cobrancaId, ",", strUsuarioId, ",", Convert.ToInt32(fonte), ", getdate())");

                NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
            }
            catch
            {
            }
        }
    }

    #endregion
}
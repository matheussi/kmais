using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using LC.Web.PadraoSeguros.Entity.Untyped;

//using DevExpress.XtraReports.UI;

namespace www.reports
{
    /*
    #region Collections

    /// <summary>
    /// Representa uma Coleção de Produtores. (Report Object Collection)
    /// </summary>
    internal class ProdutorROCollection : ArrayList, ITypedList
    {
        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (listAccessors != null && listAccessors.Length > 0)
            {
                PropertyDescriptor listAccessor = listAccessors[listAccessors.Length - 1];
                if (listAccessor.PropertyType.Equals(typeof(OperadoraROCollection)))
                    return TypeDescriptor.GetProperties(typeof(OperadoraRO));
            }
            return TypeDescriptor.GetProperties(typeof(ProdutorRO));
        }
        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return "Produtores";
        }
    }

    /// <summary>
    /// Representa uma Coleção de Operadoras. (Report Object Collection)
    /// </summary>
    internal class OperadoraROCollection : ArrayList, ITypedList
    {
        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (listAccessors != null && listAccessors.Length > 0)
            {
                PropertyDescriptor listAccessor = listAccessors[listAccessors.Length - 1];
                if (listAccessor.PropertyType.Equals(typeof(ParcelaROCollection)))
                    return TypeDescriptor.GetProperties(typeof(ParcelaRO));
            }
            return TypeDescriptor.GetProperties(typeof(OperadoraRO));
        }
        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return "Operadoras";
        }
    }

    /// <summary>
    /// Representa uma Coleção de Parcelas. (Report Object Collection)
    /// </summary>
    internal class ParcelaROCollection : ArrayList, ITypedList
    {
        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return TypeDescriptor.GetProperties(typeof(ParcelaRO));
        }
        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return "Parcelas";
        }
    }

    #endregion

    #region Bases

    /// <summary>
    /// Representa um Report Object Base.
    /// </summary>
    internal class BaseRO
    {
        /// <summary>
        /// Convert um Objeto não nulo nem DBNull para String.
        /// </summary>
        /// <param name="Data">Objeto a ser convertido.</param>
        /// <returns>Retorna Data em formato String.</returns>
        protected static String ConvertToString(Object Data)
        {
            if (Data != null && !(Data is DBNull))
                return Convert.ToString(Data);
            else
                return "N/A";
        }
    }

    #endregion

    #region Report Objects

    /// <summary>
    /// Representa um Produtor (Report Object).
    /// </summary>
    internal class ProdutorRO : BaseRO
    {
        #region Private Fields

        private Object _produtor_id;
        private String _produtor_nome;
        private String _produtor_perfil;
        private String _produtor_banco;
        private String _produtor_agencia;
        private String _produtor_conta;
        private ProdutorRO _superior;
        private ProdutorROCollection _subordinados;
        private OperadoraROCollection _operadoras;

        #endregion

        #region Public Members

        /// <summary>
        /// ID do Produtor.
        /// </summary>
        public Object ID
        {
            get { return this._produtor_id; }
            set { this._produtor_id = value; }
        }

        /// <summary>
        /// Nome do Produtor.
        /// </summary>
        public String Nome
        {
            get { return this._produtor_nome; }
            set { this._produtor_nome = value; }
        }

        /// <summary>
        /// Descrição do Perfil.
        /// </summary>
        public String Perfil
        {
            get { return this._produtor_perfil; }
            set { this._produtor_perfil = value; }
        }

        /// <summary>
        /// Banco.
        /// </summary>
        public String Banco
        {
            get { return _produtor_banco; }
            set { _produtor_banco = value; }
        }

        /// <summary>
        /// Número da Agência.
        /// </summary>
        public String Agencia
        {
            get { return _produtor_agencia; }
            set { _produtor_agencia = value; }
        }

        /// <summary>
        /// Número da Conta.
        /// </summary>
        public String Conta
        {
            get { return _produtor_conta; }
            set { _produtor_conta = value; }
        }

        /// <summary>
        /// Superior Imediato.
        /// </summary>
        public ProdutorRO Superior
        {
            get { return this._superior; }
            set { this._superior = value; }
        }

        /// <summary>
        /// Coleção com Subordinados Imediatos.
        /// </summary>
        public ProdutorROCollection Subordinados
        {
            get { return this._subordinados; }
            set { this._subordinados = value; }
        }

        /// <summary>
        /// Coleção com Operadoras.
        /// </summary>
        public OperadoraROCollection Operadoras
        {
            get { return this._operadoras; }
            set { this._operadoras = value; }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Construtor Default, inicializa a coleção de subordinados.
        /// </summary>
        public ProdutorRO()
        {
            this._subordinados = new ProdutorROCollection();
            this._operadoras = new OperadoraROCollection();
        }

        /// <summary>
        /// Construtor que inicializa a coleção de subordinados, ID e Nome.
        /// </summary>
        /// <param name="ID">ID do Produtor.</param>
        /// <param name="Nome">Nome do Produtor.</param>
        public ProdutorRO(Object ID, String Nome)
            : this()
        {
            this._produtor_id = ID;
            this._produtor_nome = Nome;
        }

        /// <summary>
        /// Construtor que inicializa a coleção de subordinados, ID, Nome e Perfil.
        /// </summary>
        /// <param name="ID">ID do Produtor.</param>
        /// <param name="Nome">Nome do Produtor.</param>
        /// <param name="Perfil">Descrição do Perfil do Produtor.</param>
        public ProdutorRO(Object ID, String Nome, String Perfil)
            : this(ID, Nome)
        {
            this._produtor_perfil = Perfil;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Carrega todos os Produtores c/ Perfil informado de uma determinada Listagem. 
        /// </summary>
        /// <param name="PerfilID">Array de ID's de Perfil</param>
        /// <param name="ListagemID">ID da Listagem</param>
        /// <returns>Retorna uma coleção de Produtores preenchidos.</returns>
        public static ProdutorROCollection GetProdutores(Object[] PerfilID, Object ListagemID)
        {
            ProdutorROCollection arrProdutores = null;

            using (DataTable dtProdutores = UntypedProcesses.CarregaCabecalhoRelatorioComissionamento(PerfilID, ListagemID))
            {
                if (dtProdutores != null && dtProdutores.Rows != null && dtProdutores.Rows.Count > 0)
                {
                    arrProdutores = new ProdutorROCollection();
                    ProdutorRO newProdutor    = null;
                    ProdutorRO newSubordinado = null;
                    Int32 intProdutorID       = -1;
                    String strProdutorNome    = null;
                    String strProdutorPerfil  = null;
                    String strProdutorBanco   = null;
                    String strProdutorAgencia = null;
                    String strProdutorConta   = null;
                    String strSubordinadoNome = null;
                    Boolean bolExist          = false;

                    foreach (DataRow row in dtProdutores.Rows)
                    {
                        if (row["produtorId"] != null && !(row["produtorId"] is DBNull))
                        {
                            intProdutorID = Convert.ToInt32(row["produtorId"]);

                            bolExist = false;
                            Int32 existIndex = -1;

                            for (Int32 i = 0; i < arrProdutores.Count; i++)
                                if (Convert.ToInt32(((ProdutorRO)arrProdutores[i]).ID) == intProdutorID)
                                {
                                    bolExist = true;
                                    existIndex = i;
                                    break;
                                }

                            if (!bolExist)
                            {
                                strProdutorNome    = (row["produtorNome"] != null && !(row["produtorNome"] is DBNull)) ? Convert.ToString(row["produtorNome"]) : String.Empty;
                                strProdutorPerfil  = (row["perfil_descricao"] != null && !(row["perfil_descricao"] is DBNull)) ? Convert.ToString(row["perfil_descricao"]) : String.Empty;
                                strProdutorBanco   = (row["listagemrelacaografo_superiorBanco"] != null && !(row["listagemrelacaografo_superiorBanco"] is DBNull)) ? Convert.ToString(row["listagemrelacaografo_superiorBanco"]) : String.Empty; ;
                                strProdutorAgencia = (row["listagemrelacaografo_superiorAgencia"] != null && !(row["listagemrelacaografo_superiorAgencia"] is DBNull)) ? Convert.ToString(row["listagemrelacaografo_superiorAgencia"]) : String.Empty; ;
                                strProdutorConta   = (row["listagemrelacaografo_superiorConta"] != null && !(row["listagemrelacaografo_superiorConta"] is DBNull)) ? Convert.ToString(row["listagemrelacaografo_superiorConta"]) : String.Empty; ;

                                newProdutor = new ProdutorRO(intProdutorID, strProdutorNome, strProdutorPerfil);
                                newProdutor.Banco   = strProdutorBanco;
                                newProdutor.Agencia = strProdutorAgencia;
                                newProdutor.Conta   = strProdutorConta;
                                newProdutor.Superior = new ProdutorRO();

                                // POSSUI SUPERIOR
                                if (row["superiorId"] != null && !(row["superiorId"] is DBNull))
                                {
                                    newProdutor.Superior.ID = Convert.ToInt32(row["superiorId"]);
                                    newProdutor.Superior.Nome = (row["superiorNome"] != null && !(row["superiorNome"] is DBNull)) ? Convert.ToString(row["superiorNome"]) : String.Empty;
                                }
                                else
                                {
                                    newProdutor.Superior.ID = newProdutor.ID;
                                    newProdutor.Superior.Nome = newProdutor.Nome;
                                }
                            }
                            else
                                newProdutor = (ProdutorRO)arrProdutores[existIndex];
                        }

                        // SUBORDINADO

                        if (row["imediatoId"] != null && !(row["imediatoId"] is DBNull))
                        {
                            strSubordinadoNome = (row["imediatoNome"] != null && !(row["imediatoNome"] is DBNull)) ? Convert.ToString(row["imediatoNome"]) : String.Empty;
                            newSubordinado = new ProdutorRO(Convert.ToInt32(row["imediatoId"]), strSubordinadoNome);
                            newSubordinado.Operadoras = OperadoraRO.GetOperadoras(newProdutor.ID, ListagemID);

                            newProdutor.Subordinados.Add(newSubordinado);
                            //if (Convert.ToInt32(newProdutor.ID) == 3)
                            //{
                            //    ProdutorRO subordinadoAux = new ProdutorRO(499, "jUCK");
                            //    subordinadoAux.Operadoras = newSubordinado.Operadoras;

                            //    newProdutor.Subordinados.Add(subordinadoAux);
                            //}
                        }

                        if (newProdutor != null && !bolExist)
                        {
                            arrProdutores.Add(newProdutor);
                        }
                    }
                }
            }

            return arrProdutores;
        }

        #endregion
    }

    /// <summary>
    /// Representa uma Operadora (Report Object).
    /// </summary>
    internal class OperadoraRO : BaseRO
    {
        #region Private Fields

        private Object _operadora_id;
        private String _operadora_nome;
        private ParcelaROCollection _parcelas;

        #endregion

        #region Public Members

        /// <summary>
        /// ID da Operadora.
        /// </summary>
        public Object ID
        {
            get { return _operadora_id; }
            set { _operadora_id = value; }
        }

        /// <summary>
        /// Nome da Operadora.
        /// </summary>
        public String Nome
        {
            get { return _operadora_nome; }
            set { _operadora_nome = value; }
        }

        /// <summary>
        /// Coleção de Parcelas.
        /// </summary>
        public ParcelaROCollection Parcelas
        {
            get { return _parcelas; }
            set { _parcelas = value; }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Construtor Default, inicializa a Coleção de Parcelas.
        /// </summary>
        public OperadoraRO()
        {
            this._parcelas = new ParcelaROCollection();
        }

        /// <summary>
        /// Inicializa com ID, Nome e a Coleção de Parcelas.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Nome"></param>
        public OperadoraRO(Object ID, String Nome)
            : this()
        {
            this._operadora_id = ID;
            this._operadora_nome = Nome;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Carrega as Operadoras de um Superior.
        /// </summary>
        /// <param name="SuperiorID">ID do Superior.</param>
        /// <param name="ListagemID">ID da Listagem.</param>
        /// <returns>Retorna uma Coleção de Operadoras preenchidas com Parcleas.</returns>
        public static OperadoraROCollection GetOperadoras(Object SuperiorID, Object ListagemID)
        {
            if (SuperiorID != null && ListagemID != null)
            {
                List<Object> lstCorretores = new List<Object>();
                OperadoraROCollection operadoras = null;

                DataTable dtOperadoras = null;
                String strOperadoraNome = null;

                operadoras = new OperadoraROCollection();

                using (dtOperadoras = UntypedProcesses.CarregaOperadorasDoProdutor(SuperiorID, ListagemID))
                {
                    if (dtOperadoras != null && dtOperadoras.Rows != null && dtOperadoras.Rows.Count > 0)
                    {
                        OperadoraRO newOperadora;

                        foreach (DataRow row in dtOperadoras.Rows)
                        {
                            if (row["listagemrelacao_operadoraId"] != null && !(row["listagemrelacao_operadoraId"] is DBNull))
                            {
                                strOperadoraNome = (row["operadora_nome"] != null && !(row["operadora_nome"] is DBNull)) ? Convert.ToString(row["operadora_nome"]) : String.Empty;

                                newOperadora = new OperadoraRO(Convert.ToInt32(row["listagemrelacao_operadoraId"]), strOperadoraNome);
                                newOperadora.Parcelas = ParcelaRO.GetParcelas(SuperiorID, newOperadora.ID, ListagemID);
                                operadoras.Add(newOperadora);

                                //operadoras.Add(new OperadoraRO(34, "AMIL SEGUROS"));
                            }
                        }
                    }
                }

                return operadoras;
            }
            else
                throw new ArgumentNullException("O ID do Superior ou ID da Listagem são nulos.");
        }

        #endregion
    }

    /// <summary>
    /// Representa uma Parcela. (Report Object)
    /// </summary>
    internal class ParcelaRO : BaseRO
    {
        #region Private Fields

        private String _contrato_numero;
        private String _contrato_nome_titular;
        private String _contrato_data_admissao;
        private String _cobranca_parcela;
        private String _cobranca_data_pago;
        private String _cobranca_valor_pago;
        private String _contrato_perc_comi;
        private String _contrato_valor_comi;
        private String _cobranca_data_venc;
        private String _contrato_data_vigencia;

        #endregion

        #region Public Members

        /// <summary>
        /// Número da Proposta.
        /// </summary>
        public String ContratoNumero
        {
            get { return _contrato_numero; }
            set { _contrato_numero = value; }
        }

        /// <summary>
        /// Nome do Titular.
        /// </summary>
        public String ContratoNomeTitular
        {
            get { return _contrato_nome_titular; }
            set { _contrato_nome_titular = value; }
        }

        /// <summary>
        /// Data de Admissão.
        /// </summary>
        public String ContratoDataAdmissao
        {
            get { return _contrato_data_admissao; }
            set { _contrato_data_admissao = value; }
        }

        /// <summary>
        /// Número da Parcela.
        /// </summary>
        public String CobrancaParcela
        {
            get { return _cobranca_parcela; }
            set { _cobranca_parcela = value; }
        }

        /// <summary>
        /// Data de Pagamento da Cobranca.
        /// </summary>
        public String CobrancaDataPago
        {
            get { return _cobranca_data_pago; }
            set { _cobranca_data_pago = value; }
        }

        /// <summary>
        /// Valor pago da Cobranca.
        /// </summary>
        public String CobrancaValorPago
        {
            get { return _cobranca_valor_pago; }
            set { _cobranca_valor_pago = value; }
        }

        /// <summary>
        /// Percentual de Comissão.
        /// </summary>
        public String ContratoPercentualComi
        {
            get { return _contrato_perc_comi; }
            set { _contrato_perc_comi = value; }
        }

        /// <summary>
        /// Valor de Comissão.
        /// </summary>
        public String ContratoValorComi
        {
            get { return _contrato_valor_comi; }
            set { _contrato_valor_comi = value; }
        }

        /// <summary>
        /// Data de Vencimento da Cobranca.
        /// </summary>
        public String CobrancaDataVencimento
        {
            get { return _cobranca_data_venc; }
            set { _cobranca_data_venc = value; }
        }

        /// <summary>
        /// Data de Vigência do Contrato.
        /// </summary>
        public String ContratoDataVigencia
        {
            get { return _contrato_data_vigencia; }
            set { _contrato_data_vigencia = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Carrega as Parcelas de um Produtor de uma Operadora de uma Listagem.
        /// </summary>
        /// <param name="ProdutorID">ID do Produtor.</param>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ListagemID">ID da Listagem.</param>
        /// <returns>Retorna uma coleção de Parcelas preenchidas.</returns>
        public static ParcelaROCollection GetParcelas(Object ProdutorID, Object OperadoraID, Object ListagemID)
        {
            if (ProdutorID != null && OperadoraID != null && ListagemID != null)
            {
                using (DataTable dtParcelas = UntypedProcesses.CarregaParcelasDoProdutor(ProdutorID, OperadoraID, ListagemID))
                {
                    ParcelaROCollection arrParcelas = null;

                    if (dtParcelas != null && dtParcelas.Rows != null && dtParcelas.Rows.Count > 0)
                    {
                        arrParcelas = new ParcelaROCollection();
                        ParcelaRO parcela;

                        foreach (DataRow row in dtParcelas.Rows)
                        {
                            parcela = new ParcelaRO();

                            parcela._contrato_numero = ConvertToString(row["listagemrelacao_contratoNumero"]);
                            parcela._contrato_nome_titular = ConvertToString(row["listagemrelacao_contratoNomeTitular"]);
                            parcela._contrato_data_admissao = ConvertToString(row["listagemrelacao_contratoAdmissao"]);
                            parcela._cobranca_parcela = ConvertToString(row["listagemrelacao_cobrancaParcela"]);
                            parcela._cobranca_data_pago = ConvertToString(row["listagemrelacao_cobrancaDataPago"]);
                            parcela._cobranca_valor_pago = ConvertToString(row["listagemrelacao_cobrancaValorPago"]);
                            parcela._contrato_perc_comi = ConvertToString(row["listagemrelacao_percentualComissao"]);
                            parcela._contrato_valor_comi = ConvertToString(row["listagemrelacao_produtorValor"]);
                            parcela._cobranca_data_venc = ConvertToString(row["listagemrelacao_cobrancaDataVencto"]);
                            parcela._contrato_data_vigencia = ConvertToString(row["listagemrelacao_contratoVigencia"]);

                            arrParcelas.Add(parcela);
                        }
                    }

                    return arrParcelas;
                }
            }
            else
                throw new ArgumentNullException("ID do Produtor ou ID da Listagem ou ID da Operadora são nulos.");
        }

        #endregion
    }

    #endregion

    */
}
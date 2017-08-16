namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    /// <summary>
    /// Representa a associação de um produto adicional e um beneficiário.
    /// </summary>
    [Serializable]
    [DBTable("adicional_beneficiario")]
    public class AdicionalBeneficiario : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _propostaid;
        Object _adicionalId;
        Object _beneficiarioid;

        String _adicionalDescricao;
        string _adicionalCodigo;
        String _adicionalCodTitular;
        Object _adicionalOperadoraId;
        Boolean _adicionalDental;
        Boolean _recorrente;
        Boolean _atualizado;

        #endregion

        /// <summary>
        /// 31
        /// </summary>
        public static readonly int _FormaPagtoBoleto    = 31;
        /// <summary>
        /// 09
        /// </summary>
        public static readonly int _FormaPagtoCredito   = 09;
        /// <summary>
        /// 10
        /// </summary>
        public static readonly int _FormaPagtoDebito    = 10;
        /// <summary>
        /// 11
        /// </summary>
        public static readonly int _FormaPagtoDescFolha = 11;
        /// <summary>
        /// 81
        /// </summary>
        public static readonly int _FormaPagtoDescConta = 81;

        #region propriedades 

        [DBFieldInfo("adicionalbeneficiario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("adicionalbeneficiario_propostaId", FieldType.Single)]
        public Object PropostaID
        {
            get { return _propostaid; }
            set { _propostaid= value; }
        }

        [DBFieldInfo("adicionalbeneficiario_atualizado", FieldType.Single)]
        public Boolean Atualizado
        {
            get { return _atualizado; }
            set { _atualizado = value; }
        }

        [Joinned("adicional_id")]
        [DBFieldInfo("adicionalbeneficiario_adicionalid", FieldType.Single)]
        public Object AdicionalID
        {
            get { return _adicionalId; }
            set { _adicionalId= value; }
        }

        [DBFieldInfo("adicionalbeneficiario_beneficiarioid", FieldType.Single)]
        public Object BeneficiarioID
        {
            get { return _beneficiarioid; }
            set { _beneficiarioid= value; }
        }

        [DBFieldInfo("adicionalbeneficiario_recorrente", FieldType.Single)]
        public Boolean Recorrente
        {
            get { return _recorrente; }
            set { _recorrente= value; }
        }

        /// <summary>
        /// Join
        /// </summary>
        [Joinned("adicional_descricao")]
        public String AdicionalDescricao
        {
            get { return _adicionalDescricao; }
            set { _adicionalDescricao= value; }
        }

        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("adicional_codigo")]
        public String AdicionalCodigo
        {
            get { return _adicionalCodigo; }
            set { _adicionalCodigo= value; }
        }

        /// <summary>
        /// Join
        /// </summary>
        [Joinned("adicional_codTitular")]
        public String AdicionalCodTitular
        {
            get { return _adicionalCodTitular; }
            set { _adicionalCodTitular= value; }
        }
        /// <summary>
        /// Join
        /// </summary>
        [Joinned("adicional_operadoraId")]
        public Object AdicionalOperadoraID
        {
            get { return _adicionalOperadoraId; }
            set { _adicionalOperadoraId= value; }
        }
        /// <summary>
        /// Join
        /// </summary>
        [Joinned("adicional_dental")]
        public Boolean AdicionalDental
        {
            get { return _adicionalDental; }
            set { _adicionalDental= value; }
        }
        /// <summary>
        /// Joinned - 0 = Taxa ou Plano, 1 = Seguro, 2 = Previdencia, 3 = Normal
        /// </summary>
        [Joinned("adicional_tipo")]
        public Int32 AdicionalTipo
        {
            get;
            set;
        }

        public Boolean Sim
        {
            get { return _beneficiarioid != null; }
        }

        [DBFieldInfo("adicionalbeneficiario_formaPagto", FieldType.Single)]
        public int FormaPagto { get; set; }

        //TAXA
        [DBFieldInfo("adicionalbeneficiario_valor", FieldType.Single)]
        public decimal Valor01 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_status01", FieldType.Single)]
        public string Status01 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_status", FieldType.Single)]
        public string Status { get; set; }

        //SEGURO
        [DBFieldInfo("adicionalbeneficiario_codCob1", FieldType.Single)]
        public int COD_COB_1 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_preCob1", FieldType.Single)]
        public decimal PRE_COB_1 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_stSgCob1", FieldType.Single)]
        public string ST_SG_1 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_dtSgCob1", FieldType.Single)]
        public DateTime DT_SG_1 { get; set; }

        [DBFieldInfo("adicionalbeneficiario_codCob2", FieldType.Single)]
        public int COD_COB_2 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_preCob2", FieldType.Single)]
        public decimal PRE_COB_2 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_stSgCob2", FieldType.Single)]
        public string ST_SG_2 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_dtSgCob2", FieldType.Single)]
        public DateTime DT_SG_2 { get; set; }

        [DBFieldInfo("adicionalbeneficiario_codCob3", FieldType.Single)]
        public int COD_COB_3 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_preCob3", FieldType.Single)]
        public decimal PRE_COB_3 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_stSgCob3", FieldType.Single)]
        public string ST_SG_3 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_dtSgCob3", FieldType.Single)]
        public DateTime DT_SG_3 { get; set; }

        [DBFieldInfo("adicionalbeneficiario_codCob4", FieldType.Single)]
        public int COD_COB_4 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_preCob4", FieldType.Single)]
        public decimal PRE_COB_4 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_stSgCob4", FieldType.Single)]
        public string ST_SG_4 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_dtSgCob4", FieldType.Single)]
        public DateTime DT_SG_4 { get; set; }

        [DBFieldInfo("adicionalbeneficiario_codCob5", FieldType.Single)]
        public int COD_COB_5 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_preCob5", FieldType.Single)]
        public decimal PRE_COB_5 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_stSgCob5", FieldType.Single)]
        public string ST_SG_5 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_dtSgCob5", FieldType.Single)]
        public DateTime DT_SG_5 { get; set; }

        [DBFieldInfo("adicionalbeneficiario_codCob6", FieldType.Single)]
        public int COD_COB_6 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_preCob6", FieldType.Single)]
        public decimal PRE_COB_6 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_stSgCob6", FieldType.Single)]
        public string ST_SG_6 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_dtSgCob6", FieldType.Single)]
        public DateTime DT_SG_6 { get; set; }

        #endregion

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }
        #endregion

        public AdicionalBeneficiario() { _recorrente = true; }

        public static IList<AdicionalBeneficiario> Carregar(Object propostaId, int tipo = -1, bool somenteAtivos = false, bool somenteInstanciasAtivas = true)
        {
            String query = "adicional_beneficiario.*, adicional_descricao, adicional_operadoraId, adicional_codTitular,adicional_dental,adicional_tipo FROM adicional_beneficiario INNER JOIN adicional ON adicionalbeneficiario_adicionalid=adicional_id WHERE adicionalbeneficiario_propostaId=" + propostaId;
            if (tipo > -1) query += " and adicional_tipo = " + tipo;
            if (somenteAtivos) query += " and adicional_ativo=1 ";
            if (somenteInstanciasAtivas)
            {
                if (tipo > -1)
                {
                    if (tipo == 0 || tipo == 2)
                        query += string.Concat("and ( adicionalbeneficiario_status = 'A'  ) "); //or adicionalbeneficiario_status01 = 'A'
                    else
                        query += string.Concat("and ( adicionalbeneficiario_stSgCob1 = 'A' or adicionalbeneficiario_stSgCob2 = 'A' or adicionalbeneficiario_stSgCob3 = 'A' or adicionalbeneficiario_stSgCob4 = 'A' or adicionalbeneficiario_stSgCob5 = 'A' or adicionalbeneficiario_stSgCob6 = 'A' ) ");
                }
                else
                {
                    query += string.Concat("and ( (adicional_tipo=3) or ((adicional_tipo=0 or adicional_tipo=2) and (adicionalbeneficiario_status = 'A' /*or adicionalbeneficiario_status01 = 'A'*/)) or (adicional_tipo=1 and (adicionalbeneficiario_stSgCob1 = 'A' or adicionalbeneficiario_stSgCob2 = 'A' or adicionalbeneficiario_stSgCob3 = 'A' or adicionalbeneficiario_stSgCob4 = 'A' or adicionalbeneficiario_stSgCob5 = 'A' or adicionalbeneficiario_stSgCob6 = 'A')) ) ");
                }
            }
            query += " ORDER BY adicional_descricao";
            return LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario));
        }

        public static IList<AdicionalBeneficiario> Carregar(Object propostaId, Object beneficiarioId, int tipo = -1)
        {
            return Carregar(propostaId, beneficiarioId, null, tipo);
        }
        public static IList<AdicionalBeneficiario> Carregar(Object propostaId, Object beneficiarioId, PersistenceManager pm, int tipo = -1)
        {
            String query = "adicional_beneficiario.*, adicional_descricao, adicional_tipo, adicional_operadoraId, adicional_codTitular,adicional_dental FROM adicional_beneficiario INNER JOIN adicional ON adicionalbeneficiario_adicionalid=adicional_id WHERE adicionalbeneficiario_propostaId=" + propostaId + " AND adicionalbeneficiario_beneficiarioid=" + beneficiarioId;
            if (tipo > -1) query += " and adicional_tipo = " + tipo;
            query += " ORDER BY adicional_descricao";
            return LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario), pm);
        }

        public static Boolean TemDental(IList<AdicionalBeneficiario> lista)
        {
            if (lista == null) { return false; }

            foreach (AdicionalBeneficiario ab in lista)
            {
                if (ab == null) { continue; }
                if (ab.AdicionalDental) { return true; }
            }

            return false;
        }

        public static Boolean EDental(AdicionalBeneficiario ab)
        {
            if (ab != null && ab.AdicionalDental)
                return true;
            else
                return false;
        }

        public static IList<AdicionalBeneficiario> Carregar(Object contratoADMId, Object planoId, Object propostaId, Object beneficiarioId)
        {
            String query = String.Concat("adicional_beneficiario.*, adicional_id, adicional_descricao ",
                "FROM adicional",
                "  INNER JOIN contratoADM_plano_adicional ON adicional_id=contratoplanoadicional_adicionalid",
                "  LEFT JOIN adicional_beneficiario ON adicionalbeneficiario_adicionalid=adicional_id ");
                
                if(propostaId != null) { query += "AND adicionalbeneficiario_propostaid="+ propostaId; }

            query = String.Concat(query, " AND adicionalbeneficiario_beneficiarioid=", beneficiarioId,
                " WHERE contratoplanoadicional_contratoid=", contratoADMId, " AND contratoplanoadicional_planoid=", planoId, " ORDER BY adicional_descricao");

            return LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario));
        }

        public static AdicionalBeneficiario CarregarParaBeneficiario(Object contratoId, Object beneficiarioId, Object adicionalId, PersistenceManager pm)
        {
            String query = String.Concat("* ",
                "FROM adicional_beneficiario ",
                "  WHERE adicionalbeneficiario_propostaId=", contratoId, " AND adicionalbeneficiario_beneficiarioId=", beneficiarioId, " AND adicionalbeneficiario_adicionalId=", adicionalId);

            IList<AdicionalBeneficiario> lista = LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario));

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        public static IList<AdicionalBeneficiario> Carregar(Object contratoADMId, Object planoId, Object propostaId)
        {
            String query = String.Concat("adicional_beneficiario.*, adicional_id, adicional_descricao ",
                "FROM adicional",
                "  INNER JOIN contratoADM_plano_adicional ON adicional_id=contratoplanoadicional_adicionalid",
                "  LEFT JOIN adicional_beneficiario ON adicionalbeneficiario_adicionalid=adicional_id ");

            if (propostaId != null) { query += "AND adicionalbeneficiario_propostaid=" + propostaId; }

            query = String.Concat(query, " WHERE contratoplanoadicional_contratoid=", contratoADMId, " AND contratoplanoadicional_planoid=", planoId, " ORDER BY adicional_descricao");

            return LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario));
        }

        public static AdicionalBeneficiario Carregar(Object adicionalBeneficiarioId, PersistenceManager pm)
        {
            //String query = "adicional_beneficiario.*, adicional_descricao, adicional_codigo, adicional_tipo, adicional_operadoraId, adicional_codTitular,adicional_dental FROM adicional_beneficiario INNER JOIN adicional ON adicionalbeneficiario_adicionalid=adicional_id WHERE adicionalbeneficiario_id=" + adicionalBeneficiarioId;
            String query = "adicional_beneficiario.*, adicional_descricao, adicional_codigo, adicional_tipo, adicional_operadoraId, adicional_codTitular,adicional_dental FROM adicional_beneficiario INNER JOIN adicional ON adicionalbeneficiario_adicionalid=adicional_id WHERE adicionalbeneficiario_id=" + adicionalBeneficiarioId;
            IList<AdicionalBeneficiario> lista = LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario), pm);

            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }

        public string RetornaNome()
        {
            if (this.AdicionalTipo == 0) return "Taxa";
            else if (this.AdicionalTipo == 3) return "Normal";
            else if (this.AdicionalTipo == 1) return "Seguro";
            else if (this.AdicionalTipo == 2) return "Previdencia";
            else return "";
        }

        public decimal RetornaValor(int formaPagto, DateTime vigencia, bool naoRecorrente = false)
        {
            if (this.Recorrente && naoRecorrente) return 0;

            if (this.AdicionalTipo != 1) //não é seguro
            {
                if (this.FormaPagto == formaPagto || formaPagto == -1)
                {
                    if (this.FormaPagto == AdicionalBeneficiario._FormaPagtoBoleto)
                    {
                        if (this.Status == null || this.Status.ToUpper() != "A") return 0;
                        else return this.Valor01;
                    }
                    else
                    {
                        if (this.Status == null || this.Status.ToUpper() != "A") return 0;
                        else return this.Valor01;
                    }
                }
                else
                    return 0;
            }
            else if (this.AdicionalTipo == 1) //seguro
            {
                if (this.FormaPagto == formaPagto || formaPagto == -1)
                {
                    //if (this.Status == null || this.Status.ToUpper() != "A") return 0;

                    decimal total = 0;

                    if (this.ST_SG_1 != null && this.ST_SG_1.ToUpper() == "A") total += this.PRE_COB_1;
                    if (this.ST_SG_2 != null && this.ST_SG_2.ToUpper() == "A") total += this.PRE_COB_2;
                    if (this.ST_SG_3 != null && this.ST_SG_3.ToUpper() == "A") total += this.PRE_COB_3;
                    if (this.ST_SG_4 != null && this.ST_SG_4.ToUpper() == "A") total += this.PRE_COB_4;
                    if (this.ST_SG_5 != null && this.ST_SG_5.ToUpper() == "A") total += this.PRE_COB_5;
                    if (this.ST_SG_6 != null && this.ST_SG_6.ToUpper() == "A") total += this.PRE_COB_6;

                    return total;
                }
                else
                    return 0;
            }
            else
                return 0;
        }

        public void AplicaRejuste(decimal reajustePlano, decimal reajusteTaxa, decimal reajusteSeguro)
        {
            if (this.AdicionalTipo != 1) //NAO é SEGURO
            {
                string status = this.Status;

                //if (this.FormaPagto == AdicionalBeneficiario._FormaPagtoBoleto) status = this.Status01;

                if (status != null && status.ToUpper() == "A")
                {
                    int plano = Convert.ToInt32(this._adicionalCodigo);

                    //decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M; <= cola

                    if (plano == 4425 || plano == 4424)
                    {
                        this.Valor01 = (reajusteTaxa / 100M) * this.Valor01 + this.Valor01;
                    }
                    else if (plano >= 4435 && plano <= 4442) //4439
                    {
                        this.Valor01 = (reajustePlano / 100M) * this.Valor01 + this.Valor01;
                    }
                    else
                        this.Valor01 = (reajusteTaxa / 100M) * this.Valor01 + this.Valor01;
                }
            }
            else if (this.AdicionalTipo == 1) //seguro
            {
                if (this.ST_SG_1 != null && this.ST_SG_1.ToUpper() == "A") this.PRE_COB_1 = (reajusteSeguro / 100M) * this.PRE_COB_1 + this.PRE_COB_1;
                if (this.ST_SG_2 != null && this.ST_SG_2.ToUpper() == "A") this.PRE_COB_2 = (reajusteSeguro / 100M) * this.PRE_COB_2 + this.PRE_COB_2;
                if (this.ST_SG_3 != null && this.ST_SG_3.ToUpper() == "A") this.PRE_COB_3 = (reajusteSeguro / 100M) * this.PRE_COB_3 + this.PRE_COB_3;
                if (this.ST_SG_4 != null && this.ST_SG_4.ToUpper() == "A") this.PRE_COB_4 = (reajusteSeguro / 100M) * this.PRE_COB_4 + this.PRE_COB_4;
                if (this.ST_SG_5 != null && this.ST_SG_5.ToUpper() == "A") this.PRE_COB_5 = (reajusteSeguro / 100M) * this.PRE_COB_5 + this.PRE_COB_5;
                if (this.ST_SG_6 != null && this.ST_SG_6.ToUpper() == "A") this.PRE_COB_6 = (reajusteSeguro / 100M) * this.PRE_COB_6 + this.PRE_COB_6;
            }
        }
    }
}

namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [Serializable()]
    [DBTable("reajusteCabecalho")]
    public class ReajusteCabecalho : EntityBase, IPersisteableEntity
    {
        public ReajusteCabecalho() { Data = DateTime.Now; }

        #region Propriedades

        [DBFieldInfo("reajuste_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID { get; set; }

        [DBFieldInfo("reajuste_usuarioId", FieldType.Single)]
        public Object UsuarioID { get; set; }

        [DBFieldInfo("reajuste_descricao", FieldType.Single)]
        public string Descricao { get; set; }

        [DBFieldInfo("reajuste_contratoAdmId", FieldType.Single)]
        public int ContratoADMId { get; set; }

        /// <summary>
        /// 1 = PSCC
        /// </summary>
        [DBFieldInfo("reajuste_tipoArquivo", FieldType.Single)]
        public int TipoArquivo { get; set; }

        [DBFieldInfo("reajuste_formaPagto", FieldType.Single)]
        public string FormaPagto { get; set; }

        [DBFieldInfo("reajuste_tipo", FieldType.Single)]
        public int Tipo { get; set; }

        [DBFieldInfo("reajuste_adicionalId", FieldType.Single)]
        public int AdicionalId { get; set; }

        [DBFieldInfo("reajuste_premio", FieldType.Single)]
        public decimal Premio { get; set; }

        [DBFieldInfo("reajuste_cobertura", FieldType.Single)]
        public decimal Cobertura { get; set; }

        [DBFieldInfo("reajuste_data", FieldType.Single)]
        public DateTime Data { get; set; }

        #endregion

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
    }

    [Serializable()]
    [DBTable("reajusteItem")]
    public class ReajusteItem : EntityBase, IPersisteableEntity
    {
        public ReajusteItem() { }

        #region Propriedades

        [DBFieldInfo("reajusteitem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID { get; set; }

        [DBFieldInfo("reajusteitem_cabecalhoId", FieldType.Single)]
        public Object CabecalhoId { get; set; }

        [DBFieldInfo("reajusteitem_adicionalBeneficiarioId", FieldType.Single)]
        public object AdicionalBeneficiarioID { get; set; }

        [DBFieldInfo("reajusteitem_valorAtual", FieldType.Single)]
        public decimal ValorAtual { get; set; }

        [DBFieldInfo("reajusteitem_valorReajustado", FieldType.Single)]
        public decimal ValorReajustado { get; set; }

        #endregion

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
    }
}

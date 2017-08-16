namespace LC.Web.PadraoSeguros.Entity.ArquivoUbrasp
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [Serializable]
    [DBTable("pscc_desconto_header")]
    public class PSCCDesconto_Header : EntityBase, IPersisteableEntity
    {
        public PSCCDesconto_Header() { Data = DateTime.Now; }

        [DBFieldInfo("psccHeader_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID { get; set; }

        /// <summary>
        /// Contrato ADM
        /// </summary>
        [DBFieldInfo("psccHeader_contratoAdmId", FieldType.Single)]
        public object OrgaoID { get; set; }

        [DBFieldInfo("psccHeader_nome", FieldType.Single)]
        public String Nome { get; set; }

        [DBFieldInfo("psccHeader_nomeLocal", FieldType.Single)]
        public String NomeLocal { get; set; }

        [DBFieldInfo("psccHeader_data", FieldType.Single)]
        public DateTime Data { get; set; }

        [DBFieldInfo("psccHeader_dataRef", FieldType.Single)]
        public DateTime DataRef { get; set; }

        [DBFieldInfo("psccHeader_dataArquivo", FieldType.Single)]
        public DateTime DataArquivo { get; set; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        #endregion

        public List<PSCCDesconto_Item> ProcessarConteudo(string conteudo, string nomeLocal, object orgaoId)
        {
            List<PSCCDesconto_Item> itens = new List<PSCCDesconto_Item>();

            int i = 0; 
            String linhaAtual = null;
            PSCCDesconto_Item item = null;
            String[] arrLinhas = conteudo.Split( new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            linhaAtual = arrLinhas[0];

            this.Nome = linhaAtual.Substring(1, 35).Trim();
            this.NomeLocal = nomeLocal;
            this.OrgaoID   = orgaoId;

            int ano = Convert.ToInt32(linhaAtual.Substring(72, 4));
            int mes = Convert.ToInt32(linhaAtual.Substring(76, 2));
            int dia = Convert.ToInt32(linhaAtual.Substring(78, 2));

            this.DataRef = new DateTime(ano, mes, dia);

            ano = Convert.ToInt32(linhaAtual.Substring(80, 4));
            mes = Convert.ToInt32(linhaAtual.Substring(84, 2));
            dia = Convert.ToInt32(linhaAtual.Substring(86, 2));
            int hora = Convert.ToInt32(linhaAtual.Substring(88, 2));
            int minu = Convert.ToInt32(linhaAtual.Substring(90, 2));

            this.DataArquivo = new DateTime(ano, mes, dia, hora, minu, 1);

            for (i = 1; i < arrLinhas.Length; i++)
            {
                linhaAtual = arrLinhas[i];

                if (linhaAtual.Substring(0, 1) != "1") continue; //só processa o registro detalhe

                item = new PSCCDesconto_Item();

                item.BeneficiarioCPF = linhaAtual.Substring(25,11).Trim();
                item.BeneficiarioMatricula = linhaAtual.Substring(36, 15).Trim();
                item.BeneficiarioNome = linhaAtual.Substring(200, 150).Trim();
                item.CodigoRetornoConciliacao = linhaAtual.Substring(139, 3).Trim();
                item.OrgaoID = linhaAtual.Substring(51, 15).Trim();
                item.ParcelaMes = string.Concat(linhaAtual.Substring(103, 2), "/", linhaAtual.Substring(105, 4));
                item.ParcelaNumero = linhaAtual.Substring(101, 2).Trim();
                item.ParcelaValor = Convert.ToDecimal(linhaAtual.Substring(109, 13) + "," + linhaAtual.Substring(122, 2), cinfo);
                item.ParcelaValorENTE = Convert.ToDecimal(linhaAtual.Substring(124, 13) + "," + linhaAtual.Substring(137, 2), cinfo);

                itens.Add(item);
            }

            return itens;
        }
    }

    [Serializable]
    [DBTable("pscc_desconto_item")]
    public class PSCCDesconto_Item : EntityBase, IPersisteableEntity
    {
        #region properties 

        [DBFieldInfo("psccItem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID { get; set; }

        [DBFieldInfo("psccItem_headerId", FieldType.Single)]
        public Object HeaderID { get; set; }

        [DBFieldInfo("psccItem_beneficiarioNome", FieldType.Single)]
        public string BeneficiarioNome { get; set; }

        /// <summary>
        /// Matrícula FUNCIONAL
        /// </summary>
        [DBFieldInfo("psccItem_beneficiarioMatricula", FieldType.Single)]
        public string BeneficiarioMatricula { get; set; }

        [DBFieldInfo("psccItem_beneficiarioCPF", FieldType.Single)]
        public string BeneficiarioCPF { get; set; }

        [DBFieldInfo("psccItem_orgaoId", FieldType.Single)]
        public string OrgaoID { get; set; }

        [DBFieldInfo("psccItem_parcelaNumero", FieldType.Single)]
        public string ParcelaNumero { get; set; }

        [DBFieldInfo("psccItem_parcelaMes", FieldType.Single)]
        public string ParcelaMes { get; set; }

        [DBFieldInfo("psccItem_parcelaValor", FieldType.Single)]
        public decimal ParcelaValor { get; set; }

        [DBFieldInfo("psccItem_parcelaValorENTE", FieldType.Single)]
        public decimal ParcelaValorENTE { get; set; }

        [DBFieldInfo("psccItem_codigoConciliacao", FieldType.Single)]
        public string CodigoRetornoConciliacao { get; set; }

        [DBFieldInfo("psccItem_cobrancaId", FieldType.Single)]
        public object CobrancaID { get; set; }

        [DBFieldInfo("psccItem_beneficiarioId", FieldType.Single)]
        public object BeneficiarioId { get; set; }

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

        #endregion
    }
}

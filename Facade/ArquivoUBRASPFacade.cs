namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Entity.ArquivoUbrasp;

    public sealed class ArquivoUBRASPFacade
    {
        #region Singleton 

        static ArquivoUBRASPFacade _instance;
        public static ArquivoUBRASPFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new ArquivoUBRASPFacade(); }
                return _instance;
            }
        }
        #endregion

        ArquivoUBRASPFacade() { }

        public void SalvaArquivoPSCC(PSCCDesconto_Header header, List<PSCCDesconto_Item> itens)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                pm.Save(header);

                foreach (var item in itens)
                {
                    item.HeaderID = header.ID;

                    item.BeneficiarioId = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select beneficiario_id from beneficiario where beneficiario_matriculaFuncional='",
                        item.BeneficiarioMatricula.TrimStart('0'), "'"), null, null, pm);

                    if (item.BeneficiarioId == null)
                    {
                        item.BeneficiarioId = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select beneficiario_id from beneficiario where beneficiario_matriculaFuncional='",
                            item.BeneficiarioMatricula, "'"), null, null, pm);
                    }

                    if (item.BeneficiarioId == null)
                    {
                        item.BeneficiarioId = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select beneficiario_id from beneficiario where beneficiario_nome='",
                            item.BeneficiarioNome.Trim(), "'"), null, null, pm);
                    }

                    pm.Save(item);
                }

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

        public IList<PSCCDesconto_Header> CarregaArquivosPSCC(object contratoadmId)
        {
            string qry = string.Concat("select top 20 * from pscc_desconto_header where psccHeader_contratoAdmId=", contratoadmId, " order by psccHeader_data desc");

            return LocatorHelper.Instance.ExecuteQuery<PSCCDesconto_Header>(qry, typeof(PSCCDesconto_Header));
        }

        public IList<PSCCDesconto_Item> CarregaItensDeArquivoPSCC(object headerId, string codigoRetorno)
        {
            string qry = "";
            
            if(string.IsNullOrEmpty(codigoRetorno))
                qry = string.Concat("select * from pscc_desconto_item where psccItem_headerId=", headerId, " order by psccItem_beneficiarioNome");
            else
                qry = string.Concat("select * from pscc_desconto_item where psccItem_headerId=", headerId, " and psccItem_codigoConciliacao='", codigoRetorno, "' order by psccItem_beneficiarioNome");

            return LocatorHelper.Instance.ExecuteQuery<PSCCDesconto_Item>(qry, typeof(PSCCDesconto_Item));
        }

        public IList<PSCCDesconto_Item> CarregaItensDeArquivoPSCC_DoBeneficiarioParaBoleto(object headerId, object itemId, object beneficiarioId)
        {
            string qry = "";

            qry = string.Concat(
                "select * from pscc_desconto_item ",
                "   where ",
                "       psccItem_headerId=", headerId, 
                "       and psccItem_beneficiarioId=", beneficiarioId,
                "       and psccItem_codigoConciliacao <> '000'",
                " order by psccItem_beneficiarioNome");

            return LocatorHelper.Instance.ExecuteQuery<PSCCDesconto_Item>(qry, typeof(PSCCDesconto_Item));
        }

        public void GerarCobrancaParaItens(IList<PSCCDesconto_Item> itens, string contratoAdmId, DateTime vencimento)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                decimal valorACobrar = 0;

                foreach (PSCCDesconto_Item i in itens)
                {
                    valorACobrar += (i.ParcelaValor - i.ParcelaValorENTE);
                }

                string qry = string.Concat(
                    "select contrato_id ",
                    "   from contrato ",
                    "       inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 ",
                    "   where ",
                    "       contratobeneficiario_beneficiarioId = ", itens[0].BeneficiarioId, 
                    "       and contrato_contratoAdmId = ", contratoAdmId);

                object contratoId = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);

                Cobranca cobranca = new Cobranca();

                cobranca.Parcela = 0;
                cobranca.DataVencimento = vencimento;
                cobranca.Valor = valorACobrar;
                cobranca.Tipo = Convert.ToInt32(Cobranca.eTipo.DiferencaUbraSP);
                cobranca.CobrancaRefID = null;
                cobranca.DataPgto = DateTime.MinValue;
                cobranca.ValorPgto = Decimal.Zero;
                cobranca.Pago = false;
                cobranca.PropostaID = contratoId;
                cobranca.Cancelada = false;
                pm.Save(cobranca);

                foreach (PSCCDesconto_Item i in itens)
                {
                    i.CobrancaID = cobranca.ID;
                    pm.Save(i);
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
            }
            finally
            {
                pm.Dispose();
            }
        }
    }
}

namespace WS
{
    using System;
    using System.Web;
    using System.Data;
    using System.Text;
    using System.Web.Services;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using System.Web.Services.Protocols;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Framework.Phantom;

    /// <summary>
    /// Summary description for srv
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [WebServiceBinding("WebService1", "http://someurl/WSDL/WebService/v1_0_1", "http://urlofwebservice/TestVD/WebService.wsdl")]
    [System.ComponentModel.ToolboxItem(false)]
    public class srv : System.Web.Services.WebService
    {
        [WebMethod(Description = "Dados um cpf ou marca ótica, retorna dados cadastrais e de cobranças da proposta. Versão para URA.")]
        public String ConsultarDadosComunsURA(String param)
        {
            String cpf = ""; String marcaOtica = ""; String contratoId = "";
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            if (param == null) { return "null"; }

            try
            {
                String[] arr = param.Split(';');
                if (arr.Length != 3) { return "diferent lenght"; }

                cpf = arr[0]; marcaOtica = arr[1]; contratoId = arr[2];
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            DateTime vencto = DateTime.MinValue, admissao = DateTime.MinValue;

            try
            {
                String qry = String.Concat("select * ",
                    "   from beneficiario ",
                    "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                    "       inner join contrato on contrato_id=contratobeneficiario_contratoId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                    "       inner join operadora on operadora_id=contrato_operadoraId ",
                    "   where ");

                if (!String.IsNullOrEmpty(contratoId))
                {
                    qry += " contrato_id=" + contratoId;
                }
                else
                {
                    if (!String.IsNullOrEmpty(cpf))
                        qry += "       beneficiario_cpf='" + cpf.Replace(".", "").Replace("-", "") + "'";
                    else if (!String.IsNullOrEmpty(marcaOtica))
                        qry += "       contrato_numeroMatricula='" + marcaOtica + "'";
                    else
                        return String.Empty;
                }

                DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];
                if (dt.Rows.Count == 0) { return String.Empty; }

                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<envelope>");

                if (dt.Rows.Count > 1)
                {
                    #region RETORNOU MAIS DE UMA PROPOSTA

                    //Localiza a proposta ATIVA mais atual 
                    Object _contratoId = null; DateTime vigencia = DateTime.MinValue;
                    foreach (DataRow row in dt.Rows)
                    {
                        if ((row["contrato_inativo"] == DBNull.Value || Convert.ToInt32(row["contrato_inativo"]) == 0) &&
                            (row["contrato_cancelado"] == DBNull.Value || Convert.ToInt32(row["contrato_cancelado"]) == 0))
                        {
                            if (Convert.ToDateTime(row["contrato_vigencia"], cinfo) > vigencia)
                            {
                                _contratoId = row["contrato_id"];
                                vigencia = Convert.ToDateTime(row["contrato_vigencia"], cinfo);
                            }
                        }
                    }

                    //NÃO HÁ CONTRATO ATIVO, entao seleciona o mais atual
                    if (_contratoId == null)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (Convert.ToDateTime(row["contrato_vigencia"], cinfo) > vigencia)
                            {
                                _contratoId = row["contrato_id"];
                                vigencia = Convert.ToDateTime(row["contrato_vigencia"], cinfo);
                            }
                        }
                    }

                    #region RETORNA PROPOSTA + ATUAL

                    int operId;

                    sb.Append("<resultType>final</resultType>");

                    DataTable dtAux = null;
                    DateTime dataInativo = DateTime.MinValue;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (Convert.ToString(row["contrato_id"]) != Convert.ToString(_contratoId)) { continue; }

                        sb.Append("<titular>");

                        sb.Append("<cpf>"); sb.Append(row["beneficiario_cpf"]); sb.Append("</cpf>");
                        sb.Append("<nome><![CDATA["); sb.Append(row["beneficiario_nome"]); sb.Append("]]></nome>");
                        sb.Append("<dataNascimento>"); sb.Append(Convert.ToDateTime(row["beneficiario_dataNascimento"]).ToString("dd/MM/yyyy")); sb.Append("</dataNascimento>");
                        sb.Append("<email><![CDATA["); sb.Append(row["beneficiario_email"]); sb.Append("]]></email>");

                        admissao = Convert.ToDateTime(row["contrato_admissao"], cinfo);

                        sb.Append("<status>");
                        if (Convert.ToInt32(row["contrato_inativo"]) == 1)
                        {
                            sb.Append("Inativo");
                            dataInativo = Convert.ToDateTime(row["contrato_datacancelamento"], cinfo);
                        }
                        else if (Convert.ToInt32(row["contrato_cancelado"]) == 1)
                        {
                            sb.Append("Cancelado");
                            dataInativo = Convert.ToDateTime(row["contrato_datacancelamento"], cinfo);
                        }
                        else
                            sb.Append("Ativo");

                        sb.Append("</status>");

                        sb.Append("<matriculaOperadoraSaude>");
                        sb.Append(row["contratobeneficiario_numMatriculaSaude"]);
                        sb.Append("</matriculaOperadoraSaude>");

                        sb.Append("<matriculaOperadoraDental>");
                        sb.Append(row["contratobeneficiario_numMatriculaDental"]);
                        sb.Append("</matriculaOperadoraDental>");

                        sb.Append("</titular>");

                        #region dependentes

                        qry = String.Concat("select beneficiario.*,contratobeneficiario_numMatriculaSaude,contratobeneficiario_numMatriculaDental,estadocivil_descricao,contratoadmparentescoagregado_parentescoDescricao,contratobeneficiario_data ",
                        "   from beneficiario ",
                        "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_ativo=1 ",
                        "       inner join contrato on contrato_id=contratobeneficiario_contratoId and contratobeneficiario_ativo=1 ",
                        "       left join estado_civil on contratobeneficiario_estadocivilid=estadocivil_id ",
                        "       inner join contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId ",
                        "   where ",
                        "       contratobeneficiario_tipo <> 0 and contratobeneficiario_contratoId=", row["contrato_id"]);

                        dtAux = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];
                        if (dtAux.Rows.Count == 0)
                            sb.Append("<dependentes></dependentes>");
                        else
                        {
                            sb.Append("<dependentes>");
                            foreach (DataRow rowdep in dtAux.Rows)
                            {
                                sb.Append("<dependente>");

                                sb.Append("<nome><![CDATA[");
                                sb.Append(rowdep["beneficiario_nome"]);
                                sb.Append("]]></nome>");
                                sb.Append("<parentesco><![CDATA[");
                                sb.Append(rowdep["contratoadmparentescoagregado_parentescoDescricao"]);
                                sb.Append("]]></parentesco>");
                                sb.Append("<estadoCivil><![CDATA[");
                                sb.Append(rowdep["estadocivil_descricao"]);
                                sb.Append("]]></estadoCivil>");
                                sb.Append("<admissao>");
                                sb.Append(Convert.ToDateTime(rowdep["contratobeneficiario_data"]).ToString("dd/MM/yyyy"));
                                sb.Append("</admissao>");

                                sb.Append("<matriculaOperadoraSaude>");
                                sb.Append(rowdep["contratobeneficiario_numMatriculaSaude"]);
                                sb.Append("</matriculaOperadoraSaude>");

                                sb.Append("<matriculaOperadoraDental>");
                                sb.Append(rowdep["contratobeneficiario_numMatriculaDental"]);
                                sb.Append("</matriculaOperadoraDental>");

                                sb.Append("</dependente>");
                            }
                            sb.Append("</dependentes>");
                        }

                        #endregion dependentes

                        #region dados do contrato

                        qry = String.Concat("select contrato_id,operadora_nome,estipulante_descricao, contratoadm_descricao, plano_descricao,contrato_numero,contrato_numeroMatricula,usuario_nome,contrato_tipoAcomodacao ",
                        "   from contrato ",
                        "       inner join usuario on usuario_id=contrato_donoid ",
                        "       inner join operadora on operadora_id=contrato_operadoraid ",
                        "       inner join estipulante on contrato_estipulanteid=estipulante_id ",
                        "       inner join contratoADM on contratoadm_id=contrato_contratoAdmId ",
                        "       inner join plano on plano_id=contrato_planoid ",
                        "   where ",
                        "       contrato_id=", row["contrato_id"]);

                        dtAux = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];

                        sb.Append("<proposta>");
                        sb.Append("<operadora><![CDATA[");
                        sb.Append(dtAux.Rows[0]["operadora_nome"]);
                        sb.Append("]]></operadora>");
                        sb.Append("<estipulante><![CDATA[");
                        sb.Append(dtAux.Rows[0]["estipulante_descricao"]);
                        sb.Append("]]></estipulante>");
                        sb.Append("<contratoAdm><![CDATA[");
                        sb.Append(dtAux.Rows[0]["contratoadm_descricao"]);
                        sb.Append("]]></contratoAdm>");
                        sb.Append("<plano><![CDATA[");
                        sb.Append(dtAux.Rows[0]["plano_descricao"]);
                        sb.Append("]]></plano>");
                        sb.Append("<numeroContrato>");
                        sb.Append(dtAux.Rows[0]["contrato_numero"]);
                        sb.Append("</numeroContrato>");
                        sb.Append("<propostaId>");
                        sb.Append(dtAux.Rows[0]["contrato_id"]);
                        sb.Append("</propostaId>");
                        sb.Append("<numeroMatricula>");
                        sb.Append(dtAux.Rows[0]["contrato_numeroMatricula"]);
                        sb.Append("</numeroMatricula>");
                        sb.Append("<produtor><![CDATA[");
                        sb.Append(dtAux.Rows[0]["usuario_nome"]);
                        sb.Append("]]></produtor>");
                        sb.Append("<acomodacao>");
                        if (Convert.ToInt32(dtAux.Rows[0]["contrato_tipoAcomodacao"]) == 1)
                            sb.Append("Apartamento");
                        else
                            sb.Append("Enfermaria");
                        sb.Append("</acomodacao>");
                        sb.Append("</proposta>");

                        #endregion dados do contrato

                        #region cobrancas

                        dtAux = LocatorHelper.Instance.ExecuteQuery("select cobranca_id, cobranca_tipo, cobranca_parcela,cobranca_valor,cobranca_valorPagto,cobranca_datavencimento,cobranca_pago,cobranca_dataPagto from cobranca where cobranca_cancelada=0 and (cobranca_tipo=0 or cobranca_tipo=1 or cobranca_tipo=4) and cobranca_pago=0 and cobranca_propostaId=" + row["contrato_id"] + " order by cobranca_parcela desc", "resultset").Tables[0];
                        sb.Append("<cobrancas>");
                        Boolean quinzeDias = false, erroPago = false, erroInativo = false, erroCancelado = false;
                        foreach (DataRow rowcob in dtAux.Rows)
                        {
                            if (rowcob["cobranca_datavencimento"] != DBNull.Value)
                                vencto = Convert.ToDateTime(rowcob["cobranca_datavencimento"], cinfo);
                            else
                            {
                                vencto = admissao;
                            }

                            if (dataInativo != DateTime.MinValue)
                            {
                                if (vencto > dataInativo &&
                                    Convert.ToInt32(rowcob["cobranca_tipo"]) != (int)Cobranca.eTipo.Parcelamento)
                                { continue; }
                            }

                            quinzeDias = false; erroInativo = false;
                            erroPago = false; erroCancelado = false;

                            if (Convert.ToInt32(row["contrato_inativo"]) == 1)
                                erroInativo = true;
                            if (Convert.ToInt32(row["contrato_cancelado"]) == 1)
                                erroCancelado = true;
                            //else
                            //{
                            operId = Convert.ToInt32(row["operadora_id"]);
                            if (vencto < DateTime.Now && Convert.ToInt32(rowcob["cobranca_pago"]) == 0 &&
                                Convert.ToInt32(rowcob["cobranca_parcela"]) != 2) // &&
                            //operId != 20 && operId != 23 && operId != 24 && operId != 25)
                            {
                                if (DateTime.Now.Subtract(vencto).Days > 17 && (!erroInativo && !erroCancelado))
                                {
                                    quinzeDias = true;
                                }
                            }
                            else if (Convert.ToInt32(rowcob["cobranca_pago"]) == 1)
                            {
                                erroPago = true;
                            }
                            //}

                            sb.Append("<cobranca>");
                            sb.Append("<id>");

                            if (quinzeDias)
                                sb.Append("_err_15d_");
                            else if (erroPago)
                                sb.Append("_err_pay_");
                            //else if (erroInativo)
                            //    sb.Append("_inactive_");
                            //else if (erroCancelado)
                            //    sb.Append("_canceled_");
                            else
                                sb.Append(rowcob["cobranca_id"]);

                            sb.Append("</id>");
                            sb.Append("<parcela>");
                            sb.Append(rowcob["cobranca_parcela"]);
                            sb.Append("</parcela>");
                            sb.Append("<vencimento>");
                            sb.Append(rowcob["cobranca_datavencimento"]);
                            sb.Append("</vencimento>");
                            sb.Append("<valor>");
                            sb.Append(rowcob["cobranca_valor"]);
                            sb.Append("</valor>");
                            sb.Append("<valorPago>");
                            sb.Append(rowcob["cobranca_valorPagto"]);
                            sb.Append("</valorPago>");
                            sb.Append("<pago>");
                            sb.Append(Convert.ToInt32(rowcob["cobranca_pago"]).ToString());
                            sb.Append("</pago>");
                            sb.Append("<dataPago>");
                            if (rowcob["cobranca_dataPagto"] != DBNull.Value)
                            {
                                sb.Append(Convert.ToDateTime(rowcob["cobranca_dataPagto"]).ToString("dd/MM/yyyy"));
                            }
                            sb.Append("</dataPago>");
                            sb.Append("</cobranca>");
                        }
                        sb.Append("</cobrancas>");

                        #endregion cobrancas

                        break;
                    }

                    sb.Append("</envelope>");
                    dt.Dispose();

                    return sb.ToString();

                    #endregion RETORNA PROPOSTA + ATUAL

                    #endregion
                }
                else
                {
                    #region RETORNOU UMA PROPOSTA

                    int operId;

                    sb.Append("<resultType>final</resultType>");

                    DataTable dtAux = null;
                    DateTime dataInativo = DateTime.MinValue;

                    foreach (DataRow row in dt.Rows)
                    {
                        sb.Append("<titular>");

                        sb.Append("<cpf>"); sb.Append(row["beneficiario_cpf"]); sb.Append("</cpf>");
                        sb.Append("<nome><![CDATA["); sb.Append(row["beneficiario_nome"]); sb.Append("]]></nome>");
                        sb.Append("<dataNascimento>"); sb.Append(Convert.ToDateTime(row["beneficiario_dataNascimento"]).ToString("dd/MM/yyyy")); sb.Append("</dataNascimento>");
                        sb.Append("<email><![CDATA["); sb.Append(row["beneficiario_email"]); sb.Append("]]></email>");

                        admissao = Convert.ToDateTime(row["contrato_admissao"], cinfo);

                        sb.Append("<status>");
                        if (Convert.ToInt32(row["contrato_inativo"]) == 1)
                        {
                            sb.Append("Inativo");
                            dataInativo = Convert.ToDateTime(row["contrato_datacancelamento"], cinfo);
                        }
                        else if (Convert.ToInt32(row["contrato_cancelado"]) == 1)
                        {
                            sb.Append("Cancelado");
                            dataInativo = Convert.ToDateTime(row["contrato_datacancelamento"], cinfo);
                        }
                        else
                            sb.Append("Ativo");

                        sb.Append("</status>");

                        sb.Append("<matriculaOperadoraSaude>");
                        sb.Append(row["contratobeneficiario_numMatriculaSaude"]);
                        sb.Append("</matriculaOperadoraSaude>");

                        sb.Append("<matriculaOperadoraDental>");
                        sb.Append(row["contratobeneficiario_numMatriculaDental"]);
                        sb.Append("</matriculaOperadoraDental>");

                        sb.Append("</titular>");

                        #region dependentes

                        qry = String.Concat("select beneficiario.*,contratobeneficiario_numMatriculaSaude,contratobeneficiario_numMatriculaDental,estadocivil_descricao,contratoadmparentescoagregado_parentescoDescricao,contratobeneficiario_data ",
                        "   from beneficiario ",
                        "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_ativo=1 ",
                        "       inner join contrato on contrato_id=contratobeneficiario_contratoId and contratobeneficiario_ativo=1 ",
                        "       left join estado_civil on contratobeneficiario_estadocivilid=estadocivil_id ",
                        "       inner join contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId ",
                        "   where ",
                        "       contratobeneficiario_tipo <> 0 and contratobeneficiario_contratoId=", row["contrato_id"]);

                        dtAux = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];
                        if (dtAux.Rows.Count == 0)
                            sb.Append("<dependentes></dependentes>");
                        else
                        {
                            sb.Append("<dependentes>");
                            foreach (DataRow rowdep in dtAux.Rows)
                            {
                                sb.Append("<dependente>");

                                sb.Append("<nome><![CDATA[");
                                sb.Append(rowdep["beneficiario_nome"]);
                                sb.Append("]]></nome>");
                                sb.Append("<parentesco><![CDATA[");
                                sb.Append(rowdep["contratoadmparentescoagregado_parentescoDescricao"]);
                                sb.Append("]]></parentesco>");
                                sb.Append("<estadoCivil><![CDATA[");
                                sb.Append(rowdep["estadocivil_descricao"]);
                                sb.Append("]]></estadoCivil>");
                                sb.Append("<admissao>");
                                sb.Append(Convert.ToDateTime(rowdep["contratobeneficiario_data"]).ToString("dd/MM/yyyy"));
                                sb.Append("</admissao>");

                                sb.Append("<matriculaOperadoraSaude>");
                                sb.Append(rowdep["contratobeneficiario_numMatriculaSaude"]);
                                sb.Append("</matriculaOperadoraSaude>");

                                sb.Append("<matriculaOperadoraDental>");
                                sb.Append(rowdep["contratobeneficiario_numMatriculaDental"]);
                                sb.Append("</matriculaOperadoraDental>");

                                sb.Append("</dependente>");
                            }
                            sb.Append("</dependentes>");
                        }

                        #endregion dependentes

                        #region dados do contrato

                        qry = String.Concat("select contrato_id,operadora_nome,estipulante_descricao, contratoadm_descricao, plano_descricao,contrato_numero,contrato_numeroMatricula,usuario_nome,contrato_tipoAcomodacao ",
                        "   from contrato ",
                        "       inner join usuario on usuario_id=contrato_donoid ",
                        "       inner join operadora on operadora_id=contrato_operadoraid ",
                        "       inner join estipulante on contrato_estipulanteid=estipulante_id ",
                        "       inner join contratoADM on contratoadm_id=contrato_contratoAdmId ",
                        "       inner join plano on plano_id=contrato_planoid ",
                        "   where ",
                        "       contrato_id=", row["contrato_id"]);

                        dtAux = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];

                        sb.Append("<proposta>");
                        sb.Append("<operadora><![CDATA[");
                        sb.Append(dtAux.Rows[0]["operadora_nome"]);
                        sb.Append("]]></operadora>");
                        sb.Append("<estipulante><![CDATA[");
                        sb.Append(dtAux.Rows[0]["estipulante_descricao"]);
                        sb.Append("]]></estipulante>");
                        sb.Append("<contratoAdm><![CDATA[");
                        sb.Append(dtAux.Rows[0]["contratoadm_descricao"]);
                        sb.Append("]]></contratoAdm>");
                        sb.Append("<plano><![CDATA[");
                        sb.Append(dtAux.Rows[0]["plano_descricao"]);
                        sb.Append("]]></plano>");
                        sb.Append("<numeroContrato>");
                        sb.Append(dtAux.Rows[0]["contrato_numero"]);
                        sb.Append("</numeroContrato>");
                        sb.Append("<propostaId>");
                        sb.Append(dtAux.Rows[0]["contrato_id"]);
                        sb.Append("</propostaId>");
                        sb.Append("<numeroMatricula>");
                        sb.Append(dtAux.Rows[0]["contrato_numeroMatricula"]);
                        sb.Append("</numeroMatricula>");
                        sb.Append("<produtor><![CDATA[");
                        sb.Append(dtAux.Rows[0]["usuario_nome"]);
                        sb.Append("]]></produtor>");
                        sb.Append("<acomodacao>");
                        if (Convert.ToInt32(dtAux.Rows[0]["contrato_tipoAcomodacao"]) == 1)
                            sb.Append("Apartamento");
                        else
                            sb.Append("Enfermaria");
                        sb.Append("</acomodacao>");
                        sb.Append("</proposta>");

                        #endregion dados do contrato

                        #region cobrancas

                        dtAux = LocatorHelper.Instance.ExecuteQuery("select cobranca_id, cobranca_tipo, cobranca_parcela,cobranca_valor,cobranca_valorPagto,cobranca_datavencimento,cobranca_pago,cobranca_dataPagto from cobranca where cobranca_cancelada=0 and (cobranca_tipo=0 or cobranca_tipo=1 or cobranca_tipo=4) and cobranca_propostaId=" + row["contrato_id"] + " order by cobranca_parcela desc", "resultset").Tables[0];
                        sb.Append("<cobrancas>");
                        Boolean quinzeDias = false, erroPago = false, erroInativo = false, erroCancelado = false;
                        foreach (DataRow rowcob in dtAux.Rows)
                        {
                            if (rowcob["cobranca_datavencimento"] != DBNull.Value)
                                vencto = Convert.ToDateTime(rowcob["cobranca_datavencimento"], cinfo);
                            else
                            {
                                vencto = admissao;
                            }

                            if (dataInativo != DateTime.MinValue)
                            {
                                if (vencto > dataInativo &&
                                    Convert.ToInt32(rowcob["cobranca_tipo"]) != (int)Cobranca.eTipo.Parcelamento)
                                { continue; }
                            }

                            quinzeDias = false; erroInativo = false;
                            erroPago = false; erroCancelado = false;

                            if (Convert.ToInt32(row["contrato_inativo"]) == 1)
                                erroInativo = true;
                            if (Convert.ToInt32(row["contrato_cancelado"]) == 1)
                                erroCancelado = true;
                            //else
                            //{
                            operId = Convert.ToInt32(row["operadora_id"]);
                            if (vencto < DateTime.Now && Convert.ToInt32(rowcob["cobranca_pago"]) == 0 &&
                                Convert.ToInt32(rowcob["cobranca_parcela"]) != 2) // &&
                            //operId != 20 && operId != 23 && operId != 24 && operId != 25)
                            {
                                if (DateTime.Now.Subtract(vencto).Days > 17 && (!erroInativo && !erroCancelado))
                                {
                                    quinzeDias = true;
                                }
                            }
                            else if (Convert.ToInt32(rowcob["cobranca_pago"]) == 1)
                            {
                                erroPago = true;
                            }
                            //}

                            sb.Append("<cobranca>");
                            sb.Append("<id>");

                            if (quinzeDias)
                                sb.Append("_err_15d_");
                            else if (erroPago)
                                sb.Append("_err_pay_");
                            //else if (erroInativo)
                            //    sb.Append("_inactive_");
                            //else if (erroCancelado)
                            //    sb.Append("_canceled_");
                            else
                                sb.Append(rowcob["cobranca_id"]);

                            sb.Append("</id>");
                            sb.Append("<parcela>");
                            sb.Append(rowcob["cobranca_parcela"]);
                            sb.Append("</parcela>");
                            sb.Append("<vencimento>");
                            sb.Append(rowcob["cobranca_datavencimento"]);
                            sb.Append("</vencimento>");
                            sb.Append("<valor>");
                            sb.Append(rowcob["cobranca_valor"]);
                            sb.Append("</valor>");
                            sb.Append("<valorPago>");
                            sb.Append(rowcob["cobranca_valorPagto"]);
                            sb.Append("</valorPago>");
                            sb.Append("<pago>");
                            sb.Append(Convert.ToInt32(rowcob["cobranca_pago"]).ToString());
                            sb.Append("</pago>");
                            sb.Append("<dataPago>");
                            if (rowcob["cobranca_dataPagto"] != DBNull.Value)
                            {
                                sb.Append(Convert.ToDateTime(rowcob["cobranca_dataPagto"]).ToString("dd/MM/yyyy"));
                            }
                            sb.Append("</dataPago>");
                            sb.Append("</cobranca>");
                        }
                        sb.Append("</cobrancas>");

                        #endregion cobrancas

                        break;
                    }

                    sb.Append("</envelope>");
                    dt.Dispose();

                    return sb.ToString();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod(Description = "Dados um cpf ou marca ótica, retorna dados cadastrais e de cobranças da proposta.")]
        public String ConsultarDadosComuns(String param)
        {
            String cpf = ""; String marcaOtica = ""; String contratoId = "";
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            if (param == null) { return "null"; }

            try
            {
                String[] arr = param.Split(';');
                if (arr.Length != 3) { return "diferent lenght"; }

                cpf = arr[0]; marcaOtica = arr[1]; contratoId = arr[2];
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            DateTime vencto = DateTime.MinValue, admissao = DateTime.MinValue;

            try
            {
                String qry = String.Concat("select * ",
                    "   from beneficiario ",
                    "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                    "       inner join contrato on contrato_id=contratobeneficiario_contratoId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                    "       inner join operadora on operadora_id=contrato_operadoraId ",
                    "   where ");

                if (!String.IsNullOrEmpty(contratoId))
                {
                    qry += " contrato_id=" + contratoId;
                }
                else
                {
                    if (!String.IsNullOrEmpty(cpf))
                        qry += "       beneficiario_cpf='" + cpf.Replace(".", "").Replace("-", "") + "'";
                    else if (!String.IsNullOrEmpty(marcaOtica))
                        qry += "       contrato_numeroMatricula='" + marcaOtica + "'";
                    else
                        return String.Empty;
                }

                DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];
                if (dt.Rows.Count == 0) { return String.Empty; }

                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<envelope>");

                if (dt.Rows.Count > 1)
                {
                    #region RETORNOU MAIS DE UMA PROPOSTA

                    //RETORNOU MAIS DE UMA PROPOSTA, O USUÁRIO DEVERÁ FAZER A ESCOLHA
                    sb.Append("<resultType>parcial</resultType>");
                    sb.Append("<propostas>");

                    foreach (DataRow row in dt.Rows)
                    {
                        sb.Append("<proposta>");
                        sb.Append("<propostaId>"); sb.Append(row["contrato_id"]); sb.Append("</propostaId>");
                        sb.Append("<operadora>"); sb.Append(row["operadora_nome"]); sb.Append("</operadora>");
                        sb.Append("<propostaNumero>"); sb.Append(row["contrato_numero"]); sb.Append("</propostaNumero>");
                        sb.Append("<titular><![CDATA["); sb.Append(row["beneficiario_nome"]); sb.Append("]]></titular>");

                        sb.Append("<status>");
                        if (Convert.ToInt32(row["contrato_inativo"]) == 1)
                            sb.Append("Inativo");
                        else if (Convert.ToInt32(row["contrato_cancelado"]) == 1)
                            sb.Append("Cancelado");
                        else
                            sb.Append("Ativo");

                        sb.Append("</status>");
                        sb.Append("</proposta>");
                    }

                    sb.Append("</propostas>");
                    sb.Append("</envelope>");
                    dt.Dispose();

                    return sb.ToString();

                    #endregion
                }
                else
                {
                    #region RETORNOU UMA PROPOSTA

                    int operId;

                    sb.Append("<resultType>final</resultType>");

                    DataTable dtAux = null;
                    DateTime dataInativo = DateTime.MinValue;

                    foreach (DataRow row in dt.Rows)
                    {
                        sb.Append("<titular>");

                        sb.Append("<cpf>"); sb.Append(row["beneficiario_cpf"]); sb.Append("</cpf>");
                        sb.Append("<nome><![CDATA["); sb.Append(row["beneficiario_nome"]); sb.Append("]]></nome>");
                        sb.Append("<dataNascimento>"); sb.Append(Convert.ToDateTime(row["beneficiario_dataNascimento"]).ToString("dd/MM/yyyy")); sb.Append("</dataNascimento>");
                        sb.Append("<email><![CDATA["); sb.Append(row["beneficiario_email"]); sb.Append("]]></email>");

                        admissao = Convert.ToDateTime(row["contrato_admissao"], cinfo);

                        sb.Append("<status>");
                        if (Convert.ToInt32(row["contrato_inativo"]) == 1)
                        {
                            sb.Append("Inativo");
                            dataInativo = Convert.ToDateTime(row["contrato_datacancelamento"], cinfo);
                        }
                        else if (Convert.ToInt32(row["contrato_cancelado"]) == 1)
                        {
                            sb.Append("Cancelado");
                            dataInativo = Convert.ToDateTime(row["contrato_datacancelamento"], cinfo);
                        }
                        else
                            sb.Append("Ativo");

                        sb.Append("</status>");

                        sb.Append("<matriculaOperadoraSaude>");
                        sb.Append(row["contratobeneficiario_numMatriculaSaude"]);
                        sb.Append("</matriculaOperadoraSaude>");

                        sb.Append("<matriculaOperadoraDental>");
                        sb.Append(row["contratobeneficiario_numMatriculaDental"]);
                        sb.Append("</matriculaOperadoraDental>");

                        sb.Append("</titular>");

                        #region dependentes

                        qry = String.Concat("select beneficiario.*,contratobeneficiario_numMatriculaSaude,contratobeneficiario_numMatriculaDental,estadocivil_descricao,contratoadmparentescoagregado_parentescoDescricao,contratobeneficiario_data ",
                        "   from beneficiario ",
                        "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_ativo=1 ",
                        "       inner join contrato on contrato_id=contratobeneficiario_contratoId and contratobeneficiario_ativo=1 ",
                        "       left join estado_civil on contratobeneficiario_estadocivilid=estadocivil_id ",
                        "       inner join contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId ",
                        "   where ",
                        "       contratobeneficiario_tipo <> 0 and contratobeneficiario_contratoId=", row["contrato_id"]);

                        dtAux = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];
                        if (dtAux.Rows.Count == 0)
                            sb.Append("<dependentes></dependentes>");
                        else
                        {
                            sb.Append("<dependentes>");
                            foreach (DataRow rowdep in dtAux.Rows)
                            {
                                sb.Append("<dependente>");

                                sb.Append("<nome><![CDATA[");
                                sb.Append(rowdep["beneficiario_nome"]);
                                sb.Append("]]></nome>");
                                sb.Append("<parentesco><![CDATA[");
                                sb.Append(rowdep["contratoadmparentescoagregado_parentescoDescricao"]);
                                sb.Append("]]></parentesco>");
                                sb.Append("<estadoCivil><![CDATA[");
                                sb.Append(rowdep["estadocivil_descricao"]);
                                sb.Append("]]></estadoCivil>");
                                sb.Append("<admissao>");
                                sb.Append(Convert.ToDateTime(rowdep["contratobeneficiario_data"]).ToString("dd/MM/yyyy"));
                                sb.Append("</admissao>");

                                sb.Append("<matriculaOperadoraSaude>");
                                sb.Append(rowdep["contratobeneficiario_numMatriculaSaude"]);
                                sb.Append("</matriculaOperadoraSaude>");

                                sb.Append("<matriculaOperadoraDental>");
                                sb.Append(rowdep["contratobeneficiario_numMatriculaDental"]);
                                sb.Append("</matriculaOperadoraDental>");

                                sb.Append("</dependente>");
                            }
                            sb.Append("</dependentes>");
                        }

                        #endregion dependentes

                        #region dados do contrato

                        qry = String.Concat("select contrato_id,operadora_nome,estipulante_descricao, contratoadm_descricao, plano_descricao,contrato_numero,contrato_numeroMatricula,usuario_nome,contrato_tipoAcomodacao ",
                        "   from contrato ",
                        "       inner join usuario on usuario_id=contrato_donoid ",
                        "       inner join operadora on operadora_id=contrato_operadoraid ",
                        "       inner join estipulante on contrato_estipulanteid=estipulante_id ",
                        "       inner join contratoADM on contratoadm_id=contrato_contratoAdmId ",
                        "       inner join plano on plano_id=contrato_planoid ",
                        "   where ",
                        "       contrato_id=", row["contrato_id"]);

                        dtAux = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];

                        sb.Append("<proposta>");
                        sb.Append("<operadora><![CDATA[");
                        sb.Append(dtAux.Rows[0]["operadora_nome"]);
                        sb.Append("]]></operadora>");
                        sb.Append("<estipulante><![CDATA[");
                        sb.Append(dtAux.Rows[0]["estipulante_descricao"]);
                        sb.Append("]]></estipulante>");
                        sb.Append("<contratoAdm><![CDATA[");
                        sb.Append(dtAux.Rows[0]["contratoadm_descricao"]);
                        sb.Append("]]></contratoAdm>");
                        sb.Append("<plano><![CDATA[");
                        sb.Append(dtAux.Rows[0]["plano_descricao"]);
                        sb.Append("]]></plano>");
                        sb.Append("<numeroContrato>");
                        sb.Append(dtAux.Rows[0]["contrato_numero"]);
                        sb.Append("</numeroContrato>");
                        sb.Append("<propostaId>");
                        sb.Append(dtAux.Rows[0]["contrato_id"]);
                        sb.Append("</propostaId>");
                        sb.Append("<numeroMatricula>");
                        sb.Append(dtAux.Rows[0]["contrato_numeroMatricula"]);
                        sb.Append("</numeroMatricula>");
                        sb.Append("<produtor><![CDATA[");
                        sb.Append(dtAux.Rows[0]["usuario_nome"]);
                        sb.Append("]]></produtor>");
                        sb.Append("<acomodacao>");
                        if (Convert.ToInt32(dtAux.Rows[0]["contrato_tipoAcomodacao"]) == 1)
                            sb.Append("Apartamento");
                        else
                            sb.Append("Enfermaria");
                        sb.Append("</acomodacao>");
                        sb.Append("</proposta>");

                        #endregion dados do contrato

                        #region cobrancas

                        dtAux = LocatorHelper.Instance.ExecuteQuery("select cobranca_id, cobranca_tipo, cobranca_parcela,cobranca_valor,cobranca_valorPagto,cobranca_datavencimento,cobranca_pago,cobranca_dataPagto from cobranca where cobranca_cancelada=0 and (cobranca_tipo=0 or cobranca_tipo=1 or cobranca_tipo=4) and cobranca_propostaId=" + row["contrato_id"] + " order by cobranca_datavencimento desc,cobranca_parcela desc", "resultset").Tables[0];
                        sb.Append("<cobrancas>");
                        Boolean quinzeDias = false, erroPago = false, erroInativo = false, erroCancelado = false;
                        foreach (DataRow rowcob in dtAux.Rows)
                        {
                            if (rowcob["cobranca_datavencimento"] != DBNull.Value)
                                vencto = Convert.ToDateTime(rowcob["cobranca_datavencimento"], cinfo);
                            else
                            {
                                vencto = admissao;
                            }

                            if (dataInativo != DateTime.MinValue)
                            {
                                if (vencto > dataInativo &&
                                    Convert.ToInt32(rowcob["cobranca_tipo"]) != (int)Cobranca.eTipo.Parcelamento)
                                { continue; }
                            }

                            quinzeDias = false; erroInativo = false;
                            erroPago = false; erroCancelado = false;

                            if (Convert.ToInt32(row["contrato_inativo"]) == 1)
                                erroInativo = true;
                            if (Convert.ToInt32(row["contrato_cancelado"]) == 1)
                                erroCancelado = true;
                            //else
                            //{
                            operId = Convert.ToInt32(row["operadora_id"]);
                            if (vencto < DateTime.Now && Convert.ToInt32(rowcob["cobranca_pago"]) == 0 &&
                                Convert.ToInt32(rowcob["cobranca_parcela"]) != 2) // &&
                            //operId != 20 && operId != 23 && operId != 24 && operId != 25)
                            {
                                if (DateTime.Now.Subtract(vencto).Days > 17 && (!erroInativo && !erroCancelado))
                                {
                                    quinzeDias = true;
                                }
                            }
                            else if (Convert.ToInt32(rowcob["cobranca_pago"]) == 1)
                            {
                                erroPago = true;
                            }
                            //}

                            sb.Append("<cobranca>");
                            sb.Append("<id>");

                            if (quinzeDias)
                                sb.Append("_err_15d_");
                            else if (erroPago)
                                sb.Append("_err_pay_");
                            //else if (erroInativo)
                            //    sb.Append("_inactive_");
                            //else if (erroCancelado)
                            //    sb.Append("_canceled_");
                            else
                                sb.Append(rowcob["cobranca_id"]);

                            sb.Append("</id>");
                            sb.Append("<parcela>");
                            sb.Append(rowcob["cobranca_parcela"]);
                            sb.Append("</parcela>");
                            sb.Append("<vencimento>");
                            sb.Append(rowcob["cobranca_datavencimento"]);
                            sb.Append("</vencimento>");
                            sb.Append("<valor>");
                            sb.Append(rowcob["cobranca_valor"]);
                            sb.Append("</valor>");
                            sb.Append("<valorPago>");
                            sb.Append(rowcob["cobranca_valorPagto"]);
                            sb.Append("</valorPago>");
                            sb.Append("<pago>");
                            sb.Append(Convert.ToInt32(rowcob["cobranca_pago"]).ToString());
                            sb.Append("</pago>");
                            sb.Append("<dataPago>");
                            if (rowcob["cobranca_dataPagto"] != DBNull.Value)
                            {
                                sb.Append(Convert.ToDateTime(rowcob["cobranca_dataPagto"]).ToString("dd/MM/yyyy"));
                            }
                            sb.Append("</dataPago>");
                            sb.Append("</cobranca>");
                        }
                        sb.Append("</cobrancas>");

                        #endregion cobrancas

                        break;
                    }

                    sb.Append("</envelope>");
                    dt.Dispose();

                    return sb.ToString();

                    #endregion
                }
            }
            catch //(Exception ex)
            {
                return "Serviço indisponível"; //ex.Message;
            }


            #region modelo xml

            /*
MODELO 1 => RETORNO PARCIAL
<?xml version="1.0" encoding="ISO-8859-1"?>
<envelope>
   <resultType>[final | parcial]</resultType>
    <propostas>
        <proposta>
            <propostaId />
            <operadora />
            <propostaNumero />
            <titular />
        <proposta>
    <propostas>
</envelope>

------------------------------------------------------------------------------------------------
MODELO 2 => RETORNO FINAL
<?xml version="1.0" encoding="ISO-8859-1"?>
<envelope>
   <resultType>[final | parcial]</resultType>
   <titular>
      <cpf></cpf>
      <nome></nome>
      <dataNascimento></dataNascimento>
      <status></status>
   </titular>
   
   <dependentes>
      <dependente>
         <nome></nome>
         <parentesco></parentesco>
         <estadoCivil></estadoCivil>
         <admissao></admissao>
      </dependente>
   </dependentes>
   
   <proposta>
      <operadora></operadora>
      <estipulante></estipulante>
      <contratoAdm></contratoAdm>
      <plano></plano>
      <numeroContrato></numeroContrato>
      <numeroMatricula></numeroMatricula>
      <produtor></produtor>
      <acomodacao></acomodacao>
   </proposta>]
   
   <cobrancas>
      <cobranca>
         <id></id>
         <parcela></parcela>
         <vencimento></vencimento>
         <valor></valor>
         <valorPago></valorPago>
         <pago></pago>
         <dataPago></dataPago>
      <cobranca>
   </cobrancas>

</envelope>

*/
            #endregion
        }

        [WebMethod(Description = "Dado um id de cobrança, retorna os detalhes da parcela.")]  //public String ConsultarParcela(String cobrancaId, String acao, String email)
        public String ConsultarParcela(String param)
        {
            String cobrancaId, acao, email;
            String[] arr = param.Split(';');

            if (arr.Length != 3 && arr.Length != 4) { return String.Empty; }

            cobrancaId = arr[0];
            acao = "5"; //arr[1];
            email = arr[2];

            String qry = String.Concat("select cobranca_arquivoUltimoEnvioId, contrato_operadoraId, contrato_codcobranca, cobranca_tipo, contrato_contratoAdmId, contrato_admissao, cobranca_id, cobranca_parcela, beneficiario_cpf,operadora_nome,beneficiario_nome,cobranca_dataVencimento,cobranca_valor,cobranca_nossoNumero,contrato_numero ",
                "   from beneficiario ",
                "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                "       inner join contrato on contrato_id=contratobeneficiario_contratoId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                "       inner join operadora on operadora_id=contrato_operadoraId ",
                "       inner join cobranca on cobranca_propostaId=contrato_id ",
                "   where ",
                "       cobranca_id=", cobrancaId);


            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                PersistenceManager pm = new PersistenceManager();
                pm.UseSingleCommandInstance();

                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                SqlDataAdapter adp = new SqlDataAdapter(qry, conn);
                DataSet ds = new DataSet();
                adp.Fill(ds);

                adp.Dispose();
                conn.Close();
                conn.Dispose();

                if (ds.Tables[0].Rows.Count == 0) { return null; }

                String nossoNumero = "";
                DateTime dataVencimento; //, vigencia, vencimento; //Int32 diaDataSemJuros, aux = 0;
                Double Valor;
                //Object valorDataLimite; //CalendarioVencimento rcv = null; 
                Cobranca cobranca = new Cobranca();

                String msgLimitePagto = "", link = "";

                int k = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (sb.Length > 0) { sb.Append(Environment.NewLine); }

                    //link
                    sb.Append("http://www.boletomail.com.br/do.php?nossonum=");

                    cobranca.Tipo = Convert.ToInt32(row["cobranca_tipo"]);
                    cobranca.ContratoCodCobranca = Convert.ToString(row["contrato_codcobranca"]);
                    cobranca.Parcela = Convert.ToInt32(row["cobranca_parcela"]);
                    cobranca.ID = row["cobranca_id"];

                    String beneficiarioNome = "";

                    if (row["beneficiario_nome"] != null && row["beneficiario_nome"] != DBNull.Value)
                        beneficiarioNome = Convert.ToString(row["beneficiario_nome"]);

                    nossoNumero = cobranca.GeraNossoNumero(); //sem DV por padrao
                    sb.Append(nossoNumero);

                    dataVencimento = Convert.ToDateTime(row["cobranca_dataVencimento"]);
                    //                    if (dataVencimento.Day == 25 && dataVencimento.Month == 12) { dataVencimento = new DateTime(dataVencimento.Year, 12, 26, 23, 59, 59, 990); }
                    DateTime vencimentoOriginal = Convert.ToDateTime(row["cobranca_dataVencimento"]);
                    //                    if (vencimentoOriginal.Day == 25 && vencimentoOriginal.Month == 12) { vencimentoOriginal = new DateTime(vencimentoOriginal.Year, 12, 26, 23, 59, 59, 990); }

                    dataVencimento = new DateTime(dataVencimento.Year, dataVencimento.Month, dataVencimento.Day, 23, 59, 59, 998);
                    Valor = Convert.ToDouble(row["cobranca_valor"]);
                    Double original = Convert.ToDouble(row["cobranca_valor"]);

                    if (dataVencimento < DateTime.Now)
                    {
                        //vencido. calcula os juros de 2% ao mês e 0,10% ao dia
                        TimeSpan diff = DateTime.Now.Subtract(dataVencimento);
                        Int32 diffDays = diff.Days;
                        //TODO: remover essa checagem temporária
                        Boolean naoCalculaJuros = false;
                        if (dataVencimento <= new DateTime(2012, 7, 13, 23, 59, 59, 990) && dataVencimento.Day == 10 && dataVencimento.Month == 7 && dataVencimento.Year == 2012) { naoCalculaJuros = true; }

                        //todo: denis, remover //////////////////////////
                        if (dataVencimento.Day == 10 && dataVencimento.Month == 1 && dataVencimento.Year == 2013)
                        { naoCalculaJuros = true; }
                        /////////////////////////////////////////////////

                        //todo: denis, remover //////////////////////////
                        Int32 operId = Convert.ToInt32(row["contrato_operadoraId"]);
                        if (dataVencimento.Day == 25 && dataVencimento.Month == 1 && dataVencimento.Year == 2013 && DateTime.Now <= new DateTime(2013, 1, 28, 23, 59, 59, 990) ||
                           (operId == 20 || operId == 23 || operId == 24 || operId == 25))
                        { naoCalculaJuros = true; }
                        /////////////////////////////////////////////////

                        if (dataVencimento.Day == 25 && dataVencimento.Month == 11 && dataVencimento.Year == 2013)
                        { naoCalculaJuros = true; }


                        //todo: denis, medida temporaria. remover////////
                        if (beneficiarioNome.IndexOf("(N*)") > -1) { naoCalculaJuros = true; }
                        /////////////////////////////////////////////////

                        //nao calcula juro para vencimento em fim de semana 
                        if (diffDays >= 2 || ((dataVencimento.DayOfWeek != DayOfWeek.Saturday && dataVencimento.DayOfWeek != DayOfWeek.Sunday)) || DateTime.Now.DayOfWeek == DayOfWeek.Tuesday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday || DateTime.Now.DayOfWeek == DayOfWeek.Thursday || DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                        {
                            if (Convert.ToInt32(row["cobranca_parcela"]) > 2 && !naoCalculaJuros)//nao calcular juro para parcelas 1 e 2
                            {
                                //juro de 2%
                                Valor = Valor * 1.02; //2%

                                //multa diária
                                for (int i = 1; i < diffDays; i++)
                                {
                                    Valor += (original * 0.001); //0,10% ao dia
                                }
                            }

                            //seta o vencimento para hoje
                            dataVencimento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59, 998);
                            msgLimitePagto = "<br>Sr. Caixa,<br>Apos o vencimento, aplicar mora diaria de 0,1%<br>Nao receber apos " + vencimentoOriginal.AddDays(17).ToString("dd/MM");
                        }

                        msgLimitePagto = "<br>Sr. Caixa,<br>Apos o vencimento, aplicar mora diaria de 0,1%<br>Nao receber apos " + vencimentoOriginal.AddDays(17).ToString("dd/MM");
                    }
                    else
                    {
                        msgLimitePagto = "<br>Sr. Caixa,<br>Apos o vencimento, aplicar multa de 2% mais mora diaria de 0,1%<br>Nao receber apos " + vencimentoOriginal.AddDays(17).ToString("dd/MM");
                    }

                    //TODO: denis remover isso ///////////////////////////////////////
                    Int32 operId2 = Convert.ToInt32(row["contrato_operadoraId"]);
                    DateTime _venctoTemp = Convert.ToDateTime(row["cobranca_dataVencimento"], new System.Globalization.CultureInfo("pt-Br"));
                    if (cobranca.Parcela == 2 || operId2 == 20 || operId2 == 23 || operId2 == 24 || operId2 == 25)
                    {
                        msgLimitePagto = "Nao receber apos " + _venctoTemp.AddDays(40).ToString("dd/MM/yyyy") + ".";
                    }

                    msgLimitePagto = "<br>Nao receber apos o vencimento.";
                    /////////////////////////////////////////////////////////////////

                    if (Convert.ToInt32(row["contrato_contratoAdmId"]) >= Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]))
                    {
                        sb.Append(String.Concat("&valor=", Valor, "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dataVencimento.Day, "&v_mes=", dataVencimento.Month, "&v_ano=", dataVencimento.Year, "&numdoc2=", row["contrato_numero"], "&nome=", row["beneficiario_nome"], "&cod_cli=", row["cobranca_id"], "&mailto=", email, "&user=qualicorp&action=", acao, Cobranca.BoletoUrlCompQualicorp, "&instr=", msgLimitePagto, "<br>"));
                    }
                    else
                    {
                        sb.Append(String.Concat("&valor=", Valor, "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dataVencimento.Day, "&v_mes=", dataVencimento.Month, "&v_ano=", dataVencimento.Year, "&numdoc2=", row["contrato_numero"], "&nome=", row["beneficiario_nome"], "&cod_cli=", row["cobranca_id"], "&mailto=", email, "&user=padraovida&action=", acao, Cobranca.BoletoUrlCompPSPadrao, "&instr=", msgLimitePagto, "<br>"));
                    }

                    //sb.Append("&cod_config=3"); //sb.Append(Cobranca.BoletoUrlComp);

                    if (k == 0)
                    {
                        link = sb.ToString();

                        System.Net.WebRequest request = System.Net.WebRequest.Create(link);
                        System.Net.WebResponse response = request.GetResponse();
                        System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
                        link = sr.ReadToEnd();
                        sr.Close();
                        response.Close();
                    }

                    k++;
                }

                pm.CloseSingleCommandInstance();
                pm.Dispose();
                ds.Dispose();

                try
                {
                    CobrancaLog log = new CobrancaLog();
                    if (arr.Length == 4 && arr[3] == "1")
                        log.CobrancaEnviada(cobranca.ID, null, CobrancaLog.Fonte.URA);
                    else
                        log.CobrancaEnviada(cobranca.ID, null, CobrancaLog.Fonte.Site);
                    log = null;
                }
                catch
                {
                }

                return link.Trim();
            }
        }

        [WebMethod(Description = "Dado um id de cobrança, retorna a composição do valor da parcela.")]
        public String MemoriaDeCalculo(String cobrancaId)
        {
            IList<CobrancaComposite> lista = CobrancaComposite.Carregar(cobrancaId);

            if (lista == null || lista.Count == 0) { return String.Empty; }

            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("<envelope>");
            sb.Append("<composicao>");

            foreach (CobrancaComposite item in lista)
            {
                if (item.Valor == 0) { continue; }

                sb.Append("<item valor=\">");
                sb.Append(item.Valor.ToString("N2"));
                sb.Append("\" descricao=\"");
                sb.Append(item.StrTipo);
                sb.Append("\"/>");
            }

            sb.Append("</composicao>");
            sb.Append("</envelope>");

            return sb.ToString();
        }

        [WebMethod(Description = "Retorna uma string xml com os dados do demonstrativo de pagamento.")]
        public String DemonstrativoPagto(String param)
        {
            StringBuilder sb = new StringBuilder();

            String ano = param.Split(';')[1];
            String contratoId = param.Split(';')[0];
            if (String.IsNullOrEmpty(ano)) { ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"]; }

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            IList<CobrancaComposite> comp = null;

            try
            {
                Contrato contrato = new Contrato(contratoId);
                pm.Load(contrato);
                Operadora operadora = new Operadora(contrato.OperadoraID);
                pm.Load(operadora);

                ContratoBeneficiario cTitular = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);

                IList<ContratoBeneficiario> beneficiarios =
                    ContratoBeneficiario.CarregarPorContratoID(contratoId, false, false, pm);
                IList<Cobranca> cobrancas = separaCobrancasDoAno(contratoId, ano, pm, cTitular);

                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<envelope>");

                sb.Append("<operadora><![CDATA[");
                if (operadora.Nome.IndexOf("-") > -1)
                    sb.Append(operadora.Nome.Split('-')[1].Trim());
                else
                    sb.Append(operadora.Nome);
                sb.Append("]]></operadora>");

                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                configuraDadosDeCadaMes(cobrancas, ref sb, cinfo);

                Decimal valorPerBenef = 0, total = 0;
                sb.Append("<beneficiarios>");

                Boolean calculadoViaComposite = false;
                foreach (ContratoBeneficiario cb in beneficiarios)
                {
                    valorPerBenef = 0;
                    calculadoViaComposite = false;

                    foreach (Cobranca cobr in cobrancas)
                    {
                        if (!cb.Ativo && cb.DataInativacao != DateTime.MinValue)
                        {
                            cb.DataInativacao = new DateTime(cb.DataInativacao.Year,
                                cb.DataInativacao.Month, cb.DataInativacao.Day, 23, 59, 59, 998);
                            if (cb.DataInativacao < cobr.DataVencimento) { continue; }
                        }
                        else if (!cb.Ativo)
                        {
                            comp = CobrancaComposite.Carregar(cobr.ID, pm);
                            if (comp == null)
                            {
                                continue;
                            }
                            else
                            {
                                foreach (CobrancaComposite item in comp)
                                {
                                    if ((item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Plano ||
                                        item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Adicional) &&
                                        item.BeneficiarioID != null &&
                                        Convert.ToString(item.BeneficiarioID) == Convert.ToString(cb.BeneficiarioID))
                                    {
                                        valorPerBenef += item.Valor;
                                    }
                                }
                                calculadoViaComposite = true;
                            }
                        } ///////demonstrativo 2

                        if (!calculadoViaComposite)
                        {
                            comp = CobrancaComposite.Carregar(cobr.ID, pm);
                            if (comp == null)
                            {
                                valorPerBenef +=
                                    Contrato.CalculaValorDaPropostaSemTaxaAssociativa(
                                    contrato.ID, cb, cobr.DataVencimento, pm);
                            }
                            else
                            {
                                foreach (CobrancaComposite item in comp)
                                {
                                    if ((item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Plano ||
                                        item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Adicional) &&
                                        item.BeneficiarioID != null &&
                                        Convert.ToString(item.BeneficiarioID) == Convert.ToString(cb.BeneficiarioID))
                                    {
                                        valorPerBenef += item.Valor;
                                    }
                                }
                            }
                        }
                    }

                    if (valorPerBenef > 0)
                    {
                        sb.Append("<beneficiario>");
                        sb.Append("<nome><![CDATA["); sb.Append(cb.BeneficiarioNome); sb.Append("]]></nome>");
                        sb.Append("<tipo>");
                        if (cb.Tipo == 0)
                            sb.Append("titular");
                        else
                            sb.Append("dependente");
                        sb.Append("</tipo>");

                        sb.Append("<valor>");
                        sb.Append(valorPerBenef.ToString("N2", cinfo));
                        sb.Append("</valor>");

                        sb.Append("</beneficiario>");

                        total += valorPerBenef;
                    }
                }

                sb.Append("</beneficiarios>");
                sb.Append("<total>");
                sb.Append(total.ToString("N2", cinfo));
                sb.Append("</total>");

                sb.Append("</envelope>");

                return sb.ToString();
            }
            catch
            {
                throw;
            }
            finally
            {
                pm.CloseSingleCommandInstance();
                pm.Dispose();
            }
        }

        IList<Cobranca> separaCobrancasDoAno(String contratoId, String ano, PersistenceManager pm, ContratoBeneficiario cTitular)
        {
            List<CobrancaComposite> comp = new List<CobrancaComposite>();
            IList<CobrancaComposite> icomp = null;
            IList<Cobranca> temp = Cobranca.CarregarTodas(contratoId, true, pm);
            List<Cobranca> cobrancas = new List<Cobranca>();
            ContratoBeneficiario depend = null;

            foreach (Cobranca cob in temp)
            {
                if (cob.DataVencimento.Year.ToString() != ano || !cob.Pago || cob.Parcela == 1) { continue; }

                ///////demonstrativo
                cob.Valor = Contrato.CalculaValorDaProposta_TODOS(
                    contratoId, cob.DataVencimento, pm, false, false, ref comp, false);

                icomp = CobrancaComposite.Carregar(cob.ID, pm);
                if (icomp != null && icomp.Count > 0)
                {
                    cob.Valor = 0;
                    foreach (CobrancaComposite item in icomp)
                    {
                        if (item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Plano ||
                           item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Adicional)
                        {
                            if (item.BeneficiarioID == null || Convert.ToString(item.BeneficiarioID) == Convert.ToString(cTitular.BeneficiarioID))
                                cob.Valor += item.Valor;
                            else
                            {
                                depend = ContratoBeneficiario.CarregarPorContratoEBeneficiario(contratoId, item.BeneficiarioID, pm);
                                depend.DataInativacao = new DateTime(depend.DataInativacao.Year, depend.DataInativacao.Month, depend.DataInativacao.Day, 23, 59, 59, 998);
                                if (depend.Ativo || depend.DataInativacao == DateTime.MinValue || depend.DataInativacao >= cob.DataVencimento)
                                    cob.Valor += item.Valor;
                            }
                        }
                    }
                }

                cobrancas.Add(cob);
            }

            return cobrancas;
        }
        void configuraDadosDeCadaMes(IList<Cobranca> cobrancas, ref StringBuilder sb, System.Globalization.CultureInfo cinfo)
        {
            sb.Append("<meses>");
            foreach (Cobranca cob in cobrancas)
            {
                sb.Append("<mes>");
                switch (cob.DataVencimento.Month)
                {
                    case 1:
                        {
                            sb.Append("<nome>Janeiro</nome>");
                            break;
                        }
                    case 2:
                        {
                            sb.Append("<nome>Fevereiro</nome>");
                            break;
                        }
                    case 3:
                        {
                            sb.Append("<nome>Março</nome>");
                            break;
                        }
                    case 4:
                        {
                            sb.Append("<nome>Abril</nome>");
                            break;
                        }
                    case 5:
                        {
                            sb.Append("<nome>Maio</nome>");
                            break;
                        }
                    case 6:
                        {
                            sb.Append("<nome>Junho</nome>");
                            break;
                        }
                    case 7:
                        {
                            sb.Append("<nome>Julho</nome>");
                            break;
                        }
                    case 8:
                        {
                            sb.Append("<nome>Agosto</nome>");
                            break;
                        }
                    case 9:
                        {
                            sb.Append("<nome>Setembro</nome>");
                            break;
                        }
                    case 10:
                        {
                            sb.Append("<nome>Outubro</nome>");
                            break;
                        }
                    case 11:
                        {
                            sb.Append("<nome>Novembro</nome>");
                            break;
                        }
                    case 12:
                        {
                            sb.Append("<nome>Dezembro</nome>");
                            break;
                        }
                }
                sb.Append("<valor>");
                sb.Append(cob.Valor.ToString("N2", cinfo));
                sb.Append("</valor>");
                sb.Append("</mes>");
            }
            sb.Append("</meses>");
        }

        [WebMethod(Description = "Retorna uma string xml com os dados da carta de quitação.")]
        public String CartaQuitacao(String param)
        {
            StringBuilder sb = new StringBuilder();

            String ano = param.Split(';')[1];
            String contratoId = param.Split(';')[0];
            if (String.IsNullOrEmpty(ano)) { ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"]; }

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            try
            {
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas(contratoId, true, true, Convert.ToInt32(ano), pm);
                if (cobrancas != null && cobrancas.Count > 0) //há cobrancas em aberto 
                { pm.CloseSingleCommandInstance(); pm.Dispose(); return String.Empty; }

                Contrato contrato = new Contrato(contratoId);
                pm.Load(contrato);
                Operadora operadora = new Operadora(contrato.OperadoraID);
                pm.Load(operadora);
                Plano plano = new Plano(contrato.PlanoID);
                pm.Load(plano);

                ContratoBeneficiario cTitular = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);

                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<envelope>");
                sb.Append("<titularNome><![CDATA[");
                sb.Append(cTitular.BeneficiarioNome);
                sb.Append("]]></titularNome>");
                sb.Append("<titularCPF><![CDATA[");
                sb.Append(cTitular.BeneficiarioCPF);
                sb.Append("]]></titularCPF>");

                //IList <AdicionalBeneficiario> adicionais = 
                //    AdicionalBeneficiario.Carregar(contrato.ID, cTitular.BeneficiarioID, pm);
                //sb.Append("<adicionais>");
                //if (adicionais != null)
                //{
                //    foreach (AdicionalBeneficiario adicional in adicionais)
                //    {
                //        sb.Append("<adicional><![CDATA[");
                //        sb.Append(adicional.AdicionalDescricao);
                //        sb.Append("]]></adicional>");
                //    }
                //}
                //sb.Append("</adicionais>");

                sb.Append("<operadora><![CDATA[");
                if (operadora.Nome.IndexOf("-") > -1)
                    sb.Append(operadora.Nome.Split('-')[1].Trim());
                else
                    sb.Append(operadora.Nome);
                sb.Append("]]></operadora>");

                sb.Append("<plano><![CDATA[");
                sb.Append(plano.Descricao);
                sb.Append("]]></plano>");

                sb.Append("<contratoNumero><![CDATA[");
                sb.Append(contrato.Numero);
                sb.Append("]]></contratoNumero>");

                sb.Append("</envelope>");

                return cTitular.BeneficiarioNome + ";" + cTitular.BeneficiarioCPF; //sb.ToString();
            }
            catch
            {
                throw;
            }
            finally
            {
                pm.CloseSingleCommandInstance();
                pm.Dispose();
            }
        }
    }
}

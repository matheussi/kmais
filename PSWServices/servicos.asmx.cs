namespace PSWServices
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
    using System.Net.Mail;
    using System.Collections;
    using System.Security.Cryptography;

    /// <summary>
    /// Serviços de segunda via de boleto
    /// </summary>
    [WebService(Namespace = "https://sistemas.cadben.com.br/ws")]
                                                                                                        //[WebService(Namespace = "https://pspadrao.com.br")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class servicos : System.Web.Services.WebService
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

                            operId = Convert.ToInt32(row["operadora_id"]);
                            if (vencto < DateTime.Now && Convert.ToInt32(rowcob["cobranca_pago"]) == 0 &&
                                Convert.ToInt32(rowcob["cobranca_parcela"]) != 2) 
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

                            sb.Append("<cobranca>");
                            sb.Append("<id>");

                            if (quinzeDias)
                            {
                                if (vencto.Year == 2014 && vencto.Month == 3)
                                    sb.Append(rowcob["cobranca_id"]);
                                else
                                    sb.Append("_err_15d_");
                            }
                            else if (erroPago)
                                sb.Append("_err_pay_");
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

                        #region checa se tem IR 

                        bool temIRCobr = false, temIRDmed = false;
                        this.checaSeTemIR(row["contrato_id"], out temIRCobr, out temIRDmed);

                        sb.Append("<IR>");
                        if (temIRCobr && temIRDmed) sb.Append("1");
                        else sb.Append("0");
                        sb.Append("</IR>");

                        #endregion checa se tem IR

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
                            {
                                if (vencto.Year == 2014 && vencto.Month == 3)
                                    sb.Append(rowcob["cobranca_id"]);
                                else
                                    sb.Append("_err_15d_");
                            }
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

                        #region checa se tem IR

                        bool temIRCobr = false, temIRDmed = false;
                        //this.checaSeTemIR(row["contrato_id"], out temIRCobr, out temIRDmed);

                        sb.Append("<IR>");
                        if (temIRCobr && temIRDmed) sb.Append("1");
                        else sb.Append("0");
                        sb.Append("</IR>");

                        #endregion checa se tem IR

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

        void checaSeTemIR(object contratoId, out bool irCobr, out bool irDmed)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();
            irCobr = false; irDmed = false;

            try
            {
                IList<Cobranca> temp = Cobranca.CarregarTodas(contratoId, true, pm);
                String ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"];

                if (temp != null)
                {
                    foreach (Cobranca cobranca in temp)
                    {
                        if (cobranca.DataPgto.Year.ToString() != ano || cobranca.Parcela == 1) { continue; }
                        if (cobranca.Pago && cobranca.ValorPgto != 1)
                        {
                            irCobr = true; break;
                        }
                    }
                }

                DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ir_dados_preprod_sp where idproposta=" + contratoId, "result", pm).Tables[0];
                if (dados.Rows.Count == 0)
                {
                    dados.Dispose();
                    irDmed = false;
                    return;
                }

                //DataRow[] retQuali0 = dados.Select("(UTILIZAR_REGISTRO = 0 OR ENVIAR_DMED = 0) AND IDCEDENTE = 1");
                //DataRow[] retPadra0 = dados.Select("(UTILIZAR_REGISTRO = 0 OR ENVIAR_DMED = 0) AND IDCEDENTE = 2");

                DataRow[] retQuali = dados.Select("IDCEDENTE = 1 AND UTILIZAR_REGISTRO = 1 AND ENVIAR_DMED = 1"); //DataRow[] retQuali = dados.Select("IDCEDENTE = 1");
                DataRow[] retPadra = dados.Select("IDCEDENTE = 2 AND UTILIZAR_REGISTRO = 1 AND ENVIAR_DMED = 1"); //DataRow[] retPadra = dados.Select("IDCEDENTE = 2");

                bool quali = false, padrao = false;

                if (retQuali.Length > 0) quali = true;
                if (retPadra.Length > 0) padrao = true;

                //if (retQuali0.Length > 0 || retQuali.Length == 0)
                //{
                //    quali = false;
                //}
                //else if (retQuali0.Length == 0 && retQuali.Length > 0)
                //{
                //    quali = true;
                //}

                //if (retPadra0.Length > 0 || retPadra.Length == 0)
                //{
                //    padrao = false;
                //}
                //else if (retPadra0.Length == 0 && retPadra.Length > 0)
                //{
                //    padrao = true;
                //}

                irDmed = quali || padrao;

                dados.Dispose();

                ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(contratoId, pm);
                //if (titular != null && titular.DMED) irDmed = true;

                IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID_Parcial(contratoId, false, false, pm);
                if (beneficiarios != null)
                {
                    foreach (ContratoBeneficiario benef in beneficiarios)
                    {
                        //if (!benef.DMED) { irDmed = false; break; }
                    }
                }

                if (irDmed && irCobr) //se está apto, por ultimo, checa se tem email cadastrado
                {
                    Beneficiario beneficiarioTit = new Beneficiario(titular.BeneficiarioID);
                    pm.Load(beneficiarioTit);
                    if (string.IsNullOrEmpty(beneficiarioTit.Email)) irDmed = false;
                }
            }
            finally
            {
                pm.CloseSingleCommandInstance();
                pm.Dispose();
            }
        }

        [WebMethod(Description = "Dado o cpf do titular de uma proposta, envia para o email do mesmo titular as informações de demonstrativo de pagamento para o ano vigente.")]
        public string EnviarDemonstrativoDePagto(string CpfTitular)
        {
            string xmlRetorno = String.Concat(
                "<envelope>",
                "<sucesso>[result]</sucesso>",
                "</envelope>");

            String qry = String.Concat("select * ",
                    "   from beneficiario ",
                    "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                    "       inner join contrato on contrato_id=contratobeneficiario_contratoId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                    "       inner join operadora on operadora_id=contrato_operadoraId ",
                    "   where beneficiario_cpf='", CpfTitular.Replace(".", "").Replace("-", "").Replace(";", "") + "'");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];
            if (dt.Rows.Count == 0) { dt.Dispose(); return xmlRetorno.Replace("[result]", "0"); }

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            Object _contratoId = null;
            if (dt.Rows.Count > 1)
            {
                #region RETORNOU MAIS DE UMA PROPOSTA - pega a + atual

                //Localiza a proposta ATIVA mais atual 
                DateTime vigencia = DateTime.MinValue;
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
                #endregion
            }
            else
                _contratoId = Convert.ToString(dt.Rows[0]["contrato_id"]);


            bool quali  = enviaIRQuali(_contratoId);
            bool padrao = enviaIRPadrao(_contratoId);

            if (quali || padrao)
                return "<envelope><sucesso>1</sucesso></envelope>";
            else
                return "<envelope><sucesso>0</sucesso></envelope>";
        }

        bool enviaIRQuali(object contratoId)
        {
            if (contratoId == null) { return false; }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            String ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"];
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ir_dados_preprod_sp where UTILIZAR_REGISTRO = 1 AND ENVIAR_DMED = 1 AND idcedente = 1 and idproposta=" + contratoId, "result", null).Tables[0];
            if (dados.Rows.Count == 0)
            {
                dados.Dispose();
                pm.CloseSingleCommandInstance();
                pm.Dispose();
                return false;
            }

            //DataRow[] ret = dados.Select("UTILIZAR_REGISTRO = 1 AND ENVIAR_DMED = 1");
            //if (ret.Length == 0)
            //{
            //    dados.Dispose();
            //    pm.CloseSingleCommandInstance();
            //    pm.Dispose();
            //    return false;
            //}

            string titularContratoBeneficiarioId = "";
            List<String> dependContratoBeneficiarioIds = new List<string>();

            foreach (DataRow row in dados.Rows)
            {
                if (this.CToString(row["SEQUENCIA"]) == "0") { titularContratoBeneficiarioId = this.CToString(row["IDPROPONENTE"]); continue; }

                dependContratoBeneficiarioIds.Add(this.CToString(row["IDPROPONENTE"]));
            }

            #region corpo do e-mail

            Contrato contrato = Contrato.CarregarParcial((Object)contratoId, pm);
            Operadora operadora = new Operadora(contrato.OperadoraID);
            pm.Load(operadora);
            ContratoBeneficiario cTitular = ContratoBeneficiario.CarregarPorIDContratoBeneficiario(titularContratoBeneficiarioId, pm);

            //if (cTitular.DMED == false)
            //{
            //    pm.CloseSingleCommandInstance();
            //    pm.Dispose();
            //    return false;
            //}

            Beneficiario beneficiario = new Beneficiario(cTitular.BeneficiarioID);
            pm.Load(beneficiario);

            sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
            sb.Append("<head><title></title>");
            sb.Append("<style type='text/css'>body, html{ font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:#b8b8b8; font-size:11px; background-color:white; margin:0px; height:100%; }link              { font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:blue; font-size:11px; } table             { font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:black; font-size:11px; }</style>");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append("<table align=\"center\" width=\"95%\">");
            sb.Append(" <tr>");
            sb.Append(" <td>");
            sb.Append("<table><tr><td><h2>Demonstrativo de Pagamentos "); sb.Append(ano); sb.Append("</td><td width='35'>&nbsp;</td><td align='left'><img align=\"right\" src='http://www.linkecerebro.com.br/LogoMail.png' /></h2></td></tr></table>");
            sb.Append(" <table style=\"font-size:12px\">");
            sb.Append(" <tr>");
            sb.Append(" <td colspan=\"2\">");
            sb.Append(String.Concat("São Paulo, ", DateTime.Now.Day, " de ", DateTime.Now.ToString("MMMM"), " de ", DateTime.Now.Year, "."));
            sb.Append(" </td>");
            sb.Append(" </tr>");
            sb.Append(" <tr><td height='5px'></td></tr>");

            if (!string.IsNullOrEmpty(contrato.ResponsavelNome) && !string.IsNullOrEmpty(contrato.ResponsavelCPF))
            {
                sb.Append(" <tr><td width=\"140px\"><b>Ilmo(a). Senhor(a)</b></td>"); sb.Append("<td>"); sb.Append(contrato.ResponsavelNome); sb.Append("</td></tr>");
                sb.Append(" <tr><td><b>CPF:</b></td><td>"); sb.Append(contrato.ResponsavelCPF); sb.Append("</td></tr>");
            }
            else
            {
                sb.Append(" <tr><td width=\"140px\"><b>Ilmo(a). Senhor(a)</b></td>"); sb.Append("<td>"); sb.Append(cTitular.BeneficiarioNome); sb.Append("</td></tr>");
                sb.Append(" <tr><td><b>CPF:</b></td><td>"); sb.Append(cTitular.BeneficiarioCPF); sb.Append("</td></tr>");
            }
            sb.Append("</table><br />");

            sb.Append("<table style=\"font-size:12px\"><tr><td><b>Cliente Qualicorp,</b></td></tr><tr><td height='8'></td></tr><tr><td>");
            sb.Append("Abaixo o demonstrativo de pagamentos efetuados, durante o ano calendário de "); sb.Append(ano); sb.Append(", à Qualicorp ");
            sb.Append("Administradora de Benefícios LTDA., inscrita no CNPJ/MF sob o nº 07.658.098/0001-18, e destinados à ");
            sb.Append("manutenção do plano privado de assistência à saúde, coletivo por adesão, por meio de contrato coletivo ");
            sb.Append("firmado com a operadora [operadora].<br />");
            sb.Append("Esse demonstrativo relaciona as despesas médicas que foram pagas pelo(a) Sr(a). e que são dedutíveis em ");
            sb.Append("Declaração de Imposto de Renda.");
            sb.Append("</td></tr></table></td></tr></table><br />");

            #region MESES 

            Decimal total = 0, totalOut = 0, totalNov = 0, totalDez = 0;
            sb.Append("<table align='center' cellpadding=\"4\" cellspacing=\"0\" style=\"border:solid 1px black;font-size:12px\" width=\"400px\"><tr><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Competência</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>*Valor mensal</b></td></tr>");

            totalOut = this.CToDecimal(dados.Compute("SUM(OUTU)", ""));
            totalNov = this.CToDecimal(dados.Compute("SUM(NOV)", ""));
            totalDez = this.CToDecimal(dados.Compute("SUM(DEZ)", ""));
            total    = totalOut + totalNov + totalDez;

            sb.Append("<tr><td>Outubro</td><td>");
            sb.Append(totalOut.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Novembro</td><td>");
            sb.Append(totalNov.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Dezembro</td><td>");
            sb.Append(totalDez.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>*Valor total</td><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>" + total.ToString("N2") + "</td></tr>");

            sb.Append("<tr><td colspan=\"2\" style=\"border-top:solid 1px black;font-size:11px\"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td></tr></table>");
            sb.Append("<br /><br />");

            #endregion MESES 

            sb.Append("<center><div style=\"color:black\"><b>COMPOSIÇÃO DO GRUPO FAMILIAR</b></div></center><table align='center' cellpadding=\"4\" cellspacing=\"0\" style=\"border:solid 1px black;font-size:12px\" width=\"400px\"><tr><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Condição</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Nome</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>*Valor por beneficiário(a)</b></td></tr>");

            IList<ContratoBeneficiario> cbeneficiarios = null;
            if (dependContratoBeneficiarioIds != null && dependContratoBeneficiarioIds.Count > 0)
                cbeneficiarios = ContratoBeneficiario.Carregar(dependContratoBeneficiarioIds.ToArray(), pm);

            cTitular.Valor = this.CToDecimal(dados.Compute("sum(OUTU)+sum(NOV)+sum(DEZ)", "IDPROPONENTE=" + cTitular.ID));

            if (cbeneficiarios != null)
            {
                foreach (ContratoBeneficiario cb in cbeneficiarios)
                {
                    cb.Valor = this.CToDecimal(dados.Compute("sum(OUTU)+sum(NOV)+sum(DEZ)", "IDPROPONENTE=" + cb.ID));
                }
            }
            else
                cbeneficiarios = new List<ContratoBeneficiario>();

            cbeneficiarios.Insert(0, cTitular);

            foreach (ContratoBeneficiario cb in cbeneficiarios)
            {
                //if (!cb.DMED) continue;

                if (cb.Valor > 0)
                {
                    sb.Append("<tr>");

                    sb.Append("<td>");
                    if (cb.Tipo == 0)
                        sb.Append("Titular");
                    else
                        sb.Append("Dependente");
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(cb.BeneficiarioNome);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(cb.Valor.ToString("N2"));
                    sb.Append("</td>");

                    sb.Append("</tr>");
                }
            }
            sb.Append("<tr><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>*Valor total</td><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke' colspan=\"2\">" + total.ToString("N2") + "</td></tr>");
            sb.Append("<tr><td style=\"border-top:solid 1px black;font-size:11px\" colspan=\"3\"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td></tr></table><br /><br />");

            sb.Append("<table align=\"center\" width=\"95%\"><tr><td>Atenção: Caso este informe seja utilizado para fins de declaração de Imposto de Renda, esclarecemos que somente podem ser deduzidas as parcelas relativas ao contribuinte e aos dependentes devidamente relacionados na própria declaração. As deduções estão sujeitas às regras estabelecidas pela legislação que regulamenta o imposto (Decreto nº 3.000/99).</td></tr><tr><td height='8'></td></tr><tr><td><b>Qualicorp Administradora de Benefícios</b></td></tr></table>");

            sb.Append("<br><br><font size='1' color='red'>Este é um e-mail automático. Por favor, não o responda.</font>");
            sb.Append("</body>");
            sb.Append("</html>");


            String corpo = sb.ToString();

            if(operadora.Nome.IndexOf('-') > -1)
                corpo = corpo.Replace("[operadora]", operadora.Nome.Split('-')[1].Trim());
            else
                corpo = corpo.Replace("[operadora]", operadora.Nome.Trim());

            pm.Dispose();

            #endregion corpo do e-mail

            //envia email
            MailMessage msg = new MailMessage(
                new MailAddress(ConfigurationManager.AppSettings["mailFrom"], ConfigurationManager.AppSettings["mailFromName"]),
                new MailAddress(beneficiario.Email));
            msg.Subject = "Demonstrativo de Pagamentos " + ano;
            msg.IsBodyHtml = true;
            msg.Body = corpo;

            try
            {
                SmtpClient client = new SmtpClient();
                client.EnableSsl = false;
                client.Send(msg); 
                client = null;
                return true;
            }
            catch
            {
                //log
                return false;
            }
            finally
            {
                msg.Dispose();
            }
        }
        bool enviaIRPadrao(object contratoId)
        {
            if (contratoId == null) { return false; }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            String ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"];
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ir_dados_preprod_sp where idcedente=2 and UTILIZAR_REGISTRO = 1 AND ENVIAR_DMED = 1 AND idproposta=" + contratoId, "result", pm).Tables[0];
            if (dados.Rows.Count == 0)
            {
                dados.Dispose();
                pm.CloseSingleCommandInstance();
                pm.Dispose();
                return false;
            }

            //DataRow[] ret = dados.Select("UTILIZAR_REGISTRO = 0 OR ENVIAR_DMED = 0");
            //if (ret.Length > 0)
            //{
            //    dados.Dispose();
            //    pm.CloseSingleCommandInstance();
            //    pm.Dispose();
            //    return false;
            //}

            string titularContratoBeneficiarioId = "";
            List<String> dependContratoBeneficiarioIds = new List<string>();

            foreach (DataRow row in dados.Rows)
            {
                if (this.CToString(row["SEQUENCIA"]) == "0") { titularContratoBeneficiarioId = this.CToString(row["IDPROPONENTE"]); continue; }

                dependContratoBeneficiarioIds.Add(this.CToString(row["IDPROPONENTE"]));
            }

            #region corpo do e-mail 

            Contrato contrato = Contrato.CarregarParcial((Object)contratoId, pm);
            Operadora operadora = new Operadora(contrato.OperadoraID);
            pm.Load(operadora);
            ContratoBeneficiario cTitular = ContratoBeneficiario.CarregarPorIDContratoBeneficiario(titularContratoBeneficiarioId, pm);

            //if (cTitular.DMED == false)
            //{
            //    pm.CloseSingleCommandInstance();
            //    pm.Dispose();
            //    return false;
            //}

            Beneficiario beneficiario = new Beneficiario(cTitular.BeneficiarioID);
            pm.Load(beneficiario);

            #region cobrancas

            //List<CobrancaComposite> comp = new List<CobrancaComposite>();
            //IList<Cobranca> temp = Cobranca.CarregarTodas(contrato.ID, true, pm);
            //List<Cobranca> cobrancas = new List<Cobranca>();
            //ContratoBeneficiario depend = null;
            //foreach (Cobranca cob in temp)
            //{
            //    if (cob.DataPgto.Year.ToString() != ano || !cob.Pago || cob.Parcela == 1 || cob.DataPgto.Month > 9 || cob.ValorPgto == 1)
            //    { continue; }

            //    ///////demonstrativo
            //    cob.Valor = Contrato.CalculaValorDaProposta_TODOS(
            //            contrato.ID, cob.DataVencimento, pm, false, false, ref comp, false);

            //    //CobrancaComposite.Remover(cob.ID, pm);
            //    //CobrancaComposite.Salvar(cob.ID, comp, pm);

            //    icomp = CobrancaComposite.Carregar(cob.ID, pm);
            //    if (icomp != null && icomp.Count > 0)
            //    {
            //        cob.Valor = 0;
            //        foreach (CobrancaComposite item in icomp)
            //        {
            //            if (item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Plano ||
            //               item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Adicional)
            //            {
            //                if (item.BeneficiarioID == null || Convert.ToString(item.BeneficiarioID) == Convert.ToString(cTitular.BeneficiarioID))
            //                    cob.Valor += item.Valor;
            //                else
            //                {
            //                    depend = ContratoBeneficiario.CarregarPorContratoEBeneficiario(contrato.ID, item.BeneficiarioID, pm);
            //                    depend.DataInativacao = new DateTime(depend.DataInativacao.Year, depend.DataInativacao.Month, depend.DataInativacao.Day, 23, 59, 59, 998);
            //                    if (depend.DMED && (depend.Ativo || depend.DataInativacao == DateTime.MinValue || depend.DataInativacao >= cob.DataVencimento))  //if (depend.Ativo || depend.DataInativacao == DateTime.MinValue || depend.DataInativacao >= cob.DataVencimento)
            //                        cob.Valor += item.Valor;
            //                }
            //            }
            //        }
            //    }

            //    cobrancas.Add(cob);
            //}
            #endregion

            sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
            sb.Append("<head><title></title>");
            sb.Append("<style type='text/css'>body, html{ font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:#b8b8b8; font-size:11px; background-color:white; margin:0px; height:100%; }link              { font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:blue; font-size:11px; } table             { font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:black; font-size:11px; }</style>");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append("<table align=\"center\" width=\"95%\">");
            sb.Append(" <tr>");
            sb.Append(" <td>");
            sb.Append("<table><tr><td><h2>Demonstrativo de Pagamentos "); sb.Append(ano); sb.Append("</td><td width='35'>&nbsp;</td><td align='left'><img align=\"right\" src='http://www.linkecerebro.com.br/LogoMail.png' /></h2></td></tr></table>");
            sb.Append(" <table style=\"font-size:12px\">");
            sb.Append(" <tr>");
            sb.Append(" <td colspan=\"2\">");
            sb.Append(String.Concat("São Paulo, ", DateTime.Now.Day, " de ", DateTime.Now.ToString("MMMM"), " de ", DateTime.Now.Year, "."));
            sb.Append(" </td>");
            sb.Append(" </tr>");
            sb.Append(" <tr><td height='5px'></td></tr>");

            if (!string.IsNullOrEmpty(contrato.ResponsavelNome) && !string.IsNullOrEmpty(contrato.ResponsavelCPF))
            {
                sb.Append(" <tr><td width=\"140px\"><b>Ilmo(a). Senhor(a)</b></td>"); sb.Append("<td>"); sb.Append(contrato.ResponsavelNome); sb.Append("</td></tr>");
                sb.Append(" <tr><td><b>CPF:</b></td><td>"); sb.Append(contrato.ResponsavelCPF); sb.Append("</td></tr>");
            }
            else
            {
                sb.Append(" <tr><td width=\"140px\"><b>Ilmo(a). Senhor(a)</b></td>"); sb.Append("<td>"); sb.Append(cTitular.BeneficiarioNome); sb.Append("</td></tr>");
                sb.Append(" <tr><td><b>CPF:</b></td><td>"); sb.Append(cTitular.BeneficiarioCPF); sb.Append("</td></tr>");
            }
            sb.Append("</table><br />");

            sb.Append("<table style=\"font-size:12px\"><tr><td><b>Cliente PS Padrão,</b></td></tr><tr><td height='8'></td></tr><tr><td>");
            sb.Append("Abaixo o demonstrativo de pagamentos efetuados, durante o ano calendário de "); sb.Append(ano); sb.Append(", à PS Padrão ");
            sb.Append("Administradora de Benefícios Ltda., inscrita no CNPJ/MF sob o nº 11.273.573/0001-05, e destinados à ");
            sb.Append("manutenção do plano privado de assistência à saúde, coletivo por adesão, por meio de contrato coletivo ");
            sb.Append("firmado com a operadora [operadora].<br />");
            sb.Append("Esse demonstrativo relaciona as despesas médicas que foram pagas pelo(a) Sr(a). e que são dedutíveis em ");
            sb.Append("Declaração de Imposto de Renda.");
            sb.Append("</td></tr></table></td></tr></table><br />");

            #region MESES 

            Decimal total = 0, totalJan = 0, totalFev = 0, totalMar = 0, totalAbr = 0, totalMaio = 0, totalJun = 0, totalJul = 0, totalAgo = 0, totalSet = 0;
            sb.Append("<table align='center' cellpadding=\"4\" cellspacing=\"0\" style=\"border:solid 1px black;font-size:12px\" width=\"400px\"><tr><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Competência</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>*Valor mensal</b></td></tr>");

            totalJan  = this.CToDecimal(dados.Compute("SUM(JAN)", ""));
            totalFev  = this.CToDecimal(dados.Compute("SUM(FEV)", ""));
            totalMar  = this.CToDecimal(dados.Compute("SUM(MAR)", ""));
            totalAbr  = this.CToDecimal(dados.Compute("SUM(ABR)", ""));
            totalMaio = this.CToDecimal(dados.Compute("SUM(MAI)", ""));
            totalJun  = this.CToDecimal(dados.Compute("SUM(JUN)", ""));
            totalJul  = this.CToDecimal(dados.Compute("SUM(JUL)", ""));
            totalAgo  = this.CToDecimal(dados.Compute("SUM(AGO)", ""));
            totalSet  = this.CToDecimal(dados.Compute("SUM(SETEM)", ""));
            total     = totalJan + totalFev + totalMar + totalAbr + totalMaio + totalJun + totalJul + totalAgo + totalSet;

            sb.Append("<tr><td>Janeiro</td><td>");
            sb.Append(totalJan.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Fevereiro</td><td>");
            sb.Append(totalFev.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Março</td><td>");
            sb.Append(totalMar.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Abril</td><td>");
            sb.Append(totalAbr.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Maio</td><td>");
            sb.Append(totalMaio.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Junho</td><td>");
            sb.Append(totalJun.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Julho</td><td>");
            sb.Append(totalJul.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Agosto</td><td>");
            sb.Append(totalAgo.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Setembro</td><td>");
            sb.Append(totalSet.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>*Valor total</td><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>" + total.ToString("N2") + "</td></tr>");

            sb.Append("<tr><td colspan=\"2\" style=\"border-top:solid 1px black;font-size:11px\"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td></tr></table>");
            sb.Append("<br /><br />");

            #endregion MESES 

            sb.Append("<center><div style=\"color:black\"><b>COMPOSIÇÃO DO GRUPO FAMILIAR</b></div></center><table align='center' cellpadding=\"4\" cellspacing=\"0\" style=\"border:solid 1px black;font-size:12px\" width=\"400px\"><tr><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Condição</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Nome</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>*Valor por beneficiário(a)</b></td></tr>");

            IList<ContratoBeneficiario> cbeneficiarios = null;
            if (dependContratoBeneficiarioIds != null && dependContratoBeneficiarioIds.Count > 0)
                cbeneficiarios = ContratoBeneficiario.Carregar(dependContratoBeneficiarioIds.ToArray(), pm);

            cTitular.Valor = this.CToDecimal(dados.Compute(
                "sum(JAN)+sum(FEV)+sum(MAR)+sum(ABR)+sum(MAI)+sum(JUN)+sum(JUL)+sum(AGO)+sum(SETEM)", "IDPROPONENTE=" + cTitular.ID));

            if (cbeneficiarios != null)
            {
                foreach (ContratoBeneficiario cb in cbeneficiarios)
                {
                    cb.Valor = this.CToDecimal(dados.Compute(
                        "sum(JAN)+sum(FEV)+sum(MAR)+sum(ABR)+sum(MAI)+sum(JUN)+sum(JUL)+sum(AGO)+sum(SETEM)", "IDPROPONENTE=" + cb.ID));
                }
            }
            else
                cbeneficiarios = new List<ContratoBeneficiario>();

            cbeneficiarios.Insert(0, cTitular);

            foreach (ContratoBeneficiario cb in cbeneficiarios)
            {
                //if (!cb.DMED) continue;

                if (cb.Valor > 0)
                {
                    sb.Append("<tr>");

                    sb.Append("<td>");
                    if (cb.Tipo == 0)
                        sb.Append("Titular");
                    else
                        sb.Append("Dependente");
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(cb.BeneficiarioNome);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(cb.Valor.ToString("N2"));
                    sb.Append("</td>");

                    sb.Append("</tr>");
                }
            }
            sb.Append("<tr><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>*Valor total</td><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke' colspan=\"2\">" + total.ToString("N2") + "</td></tr>");
            sb.Append("<tr><td style=\"border-top:solid 1px black;font-size:11px\" colspan=\"3\"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td></tr></table><br /><br />");

            sb.Append("<table align=\"center\" width=\"95%\"><tr><td>Atenção: Caso este informe seja utilizado para fins de declaração de Imposto de Renda, esclarecemos que somente podem ser deduzidas as parcelas relativas ao contribuinte e aos dependentes devidamente relacionados na própria declaração. As deduções estão sujeitas às regras estabelecidas pela legislação que regulamenta o imposto (Decreto nº 3.000/99).</td></tr><tr><td height='8'></td></tr><tr><td><b>PS Padrão Administradora de Benefícios</b></td></tr></table>");

            sb.Append("<br><br><font size='1' color='red'>Este é um e-mail automático. Por favor, não o responda.</font>");
            sb.Append("</body>");
            sb.Append("</html>");


            String corpo = sb.ToString();

            if(operadora.Nome.IndexOf('-') > -1)
                corpo = corpo.Replace("[operadora]", operadora.Nome.Split('-')[1].Trim());
            else
                corpo = corpo.Replace("[operadora]", operadora.Nome.Trim());

            pm.Dispose();

            #endregion corpo do e-mail

            //envia email
            MailMessage msg = new MailMessage(
                new MailAddress(ConfigurationManager.AppSettings["mailFrom"], ConfigurationManager.AppSettings["mailFromName"]),
                new MailAddress(beneficiario.Email));
            msg.Subject = "Demonstrativo de Pagamentos " + ano;
            msg.IsBodyHtml = true;
            msg.Body = corpo;

            try
            {
                SmtpClient client = new SmtpClient();

                client.EnableSsl = false;
                client.Send(msg);
                client = null;
                return true;
            }
            catch
            {
                //log
                return false;
            }
            finally
            {
                msg.Dispose();
            }
        }

        //---------------------------------------------------------------------//

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
                        sb.Append("<dataNascimento>"); sb.Append(CToDateTime(row["beneficiario_dataNascimento"]).ToString("dd/MM/yyyy")); sb.Append("</dataNascimento>");
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

                        //dtAux = LocatorHelper.Instance.ExecuteQuery("select cobranca_id, cobranca_tipo, cobranca_parcela,cobranca_valor,cobranca_valorPagto,cobranca_datavencimento,cobranca_pago,cobranca_dataPagto from cobranca where cobranca_cancelada=0 and (cobranca_tipo=0 or cobranca_tipo=1 or cobranca_tipo=4) and cobranca_propostaId=" + row["contrato_id"] + " order by cobranca_datavencimento desc,cobranca_parcela desc", "resultset").Tables[0];
                        dtAux = LocatorHelper.Instance.ExecuteQuery("select cobranca_id, cobranca_tipo, cobranca_parcela,cobranca_valor,cobranca_valorPagto,cobranca_datavencimento,cobranca_pago,cobranca_dataPagto from cobranca where cobranca_cancelada=0 and cobranca_propostaId=" + row["contrato_id"] + " order by cobranca_datavencimento desc,cobranca_parcela desc", "resultset").Tables[0];
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
                                Convert.ToInt32(rowcob["cobranca_parcela"]) != 2) // && //operId != 20 && operId != 23 && operId != 24 && operId != 25)
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
                            {
                                if (vencto.Year == 2014 && vencto.Month == 3)
                                    sb.Append(rowcob["cobranca_id"]);
                                else
                                    sb.Append(rowcob["cobranca_id"]); //sb.Append("_err_15d_");
                            }
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

        //--------------------------------------------------------------------//

        public String ConsultarParcela_antigo(String param)
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
                DateTime dataVencimento; 
                Double Valor;

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
                    DateTime vencimentoOriginal = Convert.ToDateTime(row["cobranca_dataVencimento"]);

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


                        //todo: remover////////
                        if (beneficiarioNome.IndexOf("(N*)") > -1) { naoCalculaJuros = true; }
                        /////////////////////////////////////////////////

                        //nao calcula juro para vencimento em fim de semana 
                        if (diffDays >= 2 || ((dataVencimento.DayOfWeek != DayOfWeek.Saturday && dataVencimento.DayOfWeek != DayOfWeek.Sunday)) || DateTime.Now.DayOfWeek == DayOfWeek.Tuesday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday || DateTime.Now.DayOfWeek == DayOfWeek.Thursday || DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                        {
                            if (Convert.ToInt32(row["cobranca_parcela"]) > 2 && !naoCalculaJuros)//nao calcular juro para parcelas 1 e 2
                            {
                                //juro de 2%
                                Valor = Valor * 1.02; 

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

                    //TODO: remover ///////////////////////////////////////
                    Int32 operId2 = Convert.ToInt32(row["contrato_operadoraId"]);
                    DateTime _venctoTemp = Convert.ToDateTime(row["cobranca_dataVencimento"], new System.Globalization.CultureInfo("pt-Br"));
                    if (cobranca.Parcela == 2 || operId2 == 20 || operId2 == 23 || operId2 == 24 || operId2 == 25)
                    {
                        msgLimitePagto = "Nao receber apos " + _venctoTemp.AddDays(40).ToString("dd/MM/yyyy") + ".";
                    }

                    msgLimitePagto = "<br>Nao receber apos o vencimento.";
                    /////////////////////////////////////////////////////////////////

                    //if (Convert.ToInt32(row["contrato_contratoAdmId"]) >= Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]))
                    //{
                        sb.Append(String.Concat("&valor=", Valor, "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dataVencimento.Day, "&v_mes=", dataVencimento.Month, "&v_ano=", dataVencimento.Year, "&numdoc2=", row["contrato_numero"], "&nome=", row["beneficiario_nome"], "&cod_cli=", row["cobranca_id"], "&mailto=", email, "&user=qualicorp&action=", acao, Cobranca.BoletoUrlCompQualicorp, "&instr=", msgLimitePagto, "<br>"));
                    //}
                    //else
                    //{
                    //    sb.Append(String.Concat("&valor=", Valor, "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dataVencimento.Day, "&v_mes=", dataVencimento.Month, "&v_ano=", dataVencimento.Year, "&numdoc2=", row["contrato_numero"], "&nome=", row["beneficiario_nome"], "&cod_cli=", row["cobranca_id"], "&mailto=", email, "&user=padraovida&action=", acao, Cobranca.BoletoUrlCompPSPadrao, "&instr=", msgLimitePagto, "<br>"));
                    //}

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

        [WebMethod(Description = "Dado um id de cobrança, retorna os detalhes da parcela. (nova versão)")]  //public String ConsultarParcela(String cobrancaId, String acao, String email)
        public String ConsultarParcela(String param)
        {
            String cobrancaId, acao, email;
            String[] arr = param.Split(';');

            if (arr.Length != 3 && arr.Length != 4) { return String.Empty; }

            cobrancaId = arr[0];
            acao = "5";
            email = arr[2];

            String qry = String.Concat("select contrato_id, beneficiario_id,contrato_enderecoCobrancaId,contrato_enderecoReferenciaId,cobranca_arquivoUltimoEnvioId, contrato_operadoraId, contrato_codcobranca, cobranca_tipo, contrato_contratoAdmId, contrato_admissao, cobranca_id, cobranca_parcela, beneficiario_cpf,operadora_nome,beneficiario_nome,cobranca_dataVencimento,cobranca_valor,cobranca_nossoNumero,contrato_numero ",
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
                DateTime dataVencimento; 
                Double Valor;

                Cobranca cobranca = new Cobranca();

                String msgLimitePagto = "", link = "";

                int k = 0;


                #region foreach 

                string baseBoletoUrl = "http://ubrasp.iphotel.info/boleto/santander2.aspx"; //"http://186.233.90.19/";//"http://localhost/phpBoleto/"; //"http://localhost/phpBoleto/"; //"http://smartsite.com.br/boletoquali/";

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (sb.Length > 0) { sb.Append(Environment.NewLine); }

                    Endereco endereco = null;
                    if (row["contrato_enderecoCobrancaId"] != DBNull.Value)
                        endereco = new Endereco(row["contrato_enderecoCobrancaId"]);
                    else
                        endereco = new Endereco(row["contrato_enderecoReferenciaId"]);

                    endereco.Carregar();

                    string end1 = string.Concat(endereco.Logradouro, ", ", endereco.Numero);
                    if (!string.IsNullOrEmpty(endereco.Complemento)) { end1 += string.Concat(" - ", endereco.Complemento); }
                    if (!string.IsNullOrEmpty(endereco.Bairro)) { end1 += string.Concat(" - ", endereco.Bairro); }

                    string end2 = string.Concat(endereco.Cidade, " - ", endereco.UF, " - CEP: ", endereco.CEP);

                    //link
                    sb.Append(baseBoletoUrl);
                    sb.Append("?nossonum=");

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
                    DateTime vencimentoOriginal = Convert.ToDateTime(row["cobranca_dataVencimento"]);

                    dataVencimento = new DateTime(dataVencimento.Year, dataVencimento.Month, dataVencimento.Day, 23, 59, 59, 998);
                    Valor = Convert.ToDouble(row["cobranca_valor"]);
                    Double original = Convert.ToDouble(row["cobranca_valor"]);

                    if (dataVencimento < DateTime.Now)
                    {
                        //vencido. calcula os juros de 2% ao mês e 0,10% ao dia
                        TimeSpan diff = DateTime.Now.Subtract(dataVencimento);
                        Int32 diffDays = diff.Days;

                        Boolean naoCalculaJuros = false;

                        #region todo: denis, medida temporaria. remover////////
                        if (beneficiarioNome.IndexOf("(N*)") > -1) { naoCalculaJuros = true; }

                        if (dataVencimento.Year == 2014 && dataVencimento.Month == 3 &&
                        (dataVencimento.Day == 10 || dataVencimento.Day == 25))
                        {
                            naoCalculaJuros = true;
                        }

                        #endregion /////////////////////////////////////////////////

                        #region nao calcula juro para vencimento em fim de semana 
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
                    #endregion  

                        #region TODO: denis remover isso ///////////////////////////////////////
                        Int32 operId2 = Convert.ToInt32(row["contrato_operadoraId"]);
                        DateTime _venctoTemp = Convert.ToDateTime(row["cobranca_dataVencimento"], new System.Globalization.CultureInfo("pt-Br"));
                        if (cobranca.Parcela == 2 || operId2 == 20 || operId2 == 23 || operId2 == 24 || operId2 == 25)
                        {
                            msgLimitePagto = "Não receber após " + _venctoTemp.AddDays(40).ToString("dd/MM/yyyy") + ".";
                        }

                        msgLimitePagto = "<br>Não receber após o vencimento.";
                        #endregion

                    if (Convert.ToInt32(row["contrato_contratoAdmId"]) >= Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]))
                    {
                        sb.Append(String.Concat("&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.ToString("MM"), "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.ToString("MM"), "&p_ano=", DateTime.Now.Year, "&v_dia=", dataVencimento.Day, "&v_mes=", dataVencimento.ToString("MM"), "&v_ano=", dataVencimento.Year, "&numdoc2=", row["contrato_numero"], "&nome=", row["beneficiario_nome"], "&mailto=", email, "&instr1=", msgLimitePagto));
                    }
                    else
                    {
                        sb.Append(String.Concat("&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.ToString("MM"), "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.ToString("MM"), "&p_ano=", DateTime.Now.Year, "&v_dia=", dataVencimento.Day, "&v_mes=", dataVencimento.ToString("MM"), "&v_ano=", dataVencimento.Year, "&numdoc2=", row["contrato_numero"], "&nome=", row["beneficiario_nome"], "&mailto=", email, "&instr1=", msgLimitePagto));
                    }

                    sb.Append("&end1="); sb.Append(end1);
                    sb.Append("&end2="); sb.Append(end2);

                    String demonst1 = String.Concat("<br>Este boleto é referente ao período de cobertura de ", vencimentoOriginal.ToString("MM"), "/", vencimentoOriginal.Year, ".");
                    sb.Append("&demonst1="); sb.Append(demonst1);

                    if (k == 0)
                    {
                        string uri = EntityBase.RetiraAcentos(
                            String.Concat(
                            "?nossonum=", row["cobranca_id"],
                            "&contid=", row["contrato_id"],
                            "&valor=", Valor.ToString("N2"),
                            "&v_dia=", vencimentoOriginal.Day, "&v_mes=", vencimentoOriginal.Month, "&v_ano=", vencimentoOriginal.Year,
                            "&bid=", row["beneficiario_id"],
                            "&cobid=", row["cobranca_id"],
                            "&mailto=", email)); 

                        //link = string.Concat(baseBoletoUrl, "boleto_itau.php?param=", EncryptBetweenPHP(sb.ToString())); //RetiraAcentos( sb.ToString();
                        link = string.Concat(baseBoletoUrl, uri);

                        #region webrequest comentado 
                        //System.Net.WebRequest  request = System.Net.WebRequest.Create(link);
                        //System.Net.WebResponse response = request.GetResponse();
                        //System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("ISO-8859-1"));
                        //string corpo = sr.ReadToEnd();
                        //sr.Close();
                        //response.Close();
                        #endregion 

                        #region envio de email comentado
                        //MailMessage msg = new MailMessage(
                        //new MailAddress(ConfigurationManager.AppSettings["mailFrom"], ConfigurationManager.AppSettings["mailFromName"]),
                        //new MailAddress("denis@linkecerebro.com.br"));

                        //msg.Subject = "Boleto";
                        //msg.IsBodyHtml = true;

                        //SmtpClient client = new SmtpClient();

                        //msg.Body = corpo;
                        //////client.Send(msg);

                        //msg.Body = corpoBoletoTexto(link);
                        //////client.Send(msg);

                        //msg.Dispose();
                        //client = null;
                        #endregion
                    }

                    k++;
                    break;//////////////////////////////////
                }
                #endregion foreach

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

        //--------------------------------------------------------------------//

        //[WebMethod(Description = "Dado um id de cobrança, retorna a composição do valor da parcela.")]
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

        //--------------------------------------------------------------------//

        //[WebMethod(Description = "Dado um contrato, valida e retorna os dados para termo de quitação.")]
        public string TermoDeQuitacaoAnual(string contratoId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("<envelope>");
            sb.Append("<resultado>");

            if (checaRegraTermoAnual(contratoId))
            {
                sb.Append("1</resultado>");

                sb.Append("<corpo><![CDATA[");

                #region corpo 

                string nome = "", cpf = "";

                Contrato contrato = new Contrato(contratoId);
                contrato.Carregar();

                if (!string.IsNullOrEmpty(contrato.ResponsavelNome) && !string.IsNullOrEmpty(contrato.ResponsavelCPF))
                {
                    //responsavel financeiro
                    nome = contrato.ResponsavelNome;
                    cpf = contrato.ResponsavelCPF;
                }
                else
                {
                    //titular cadastrado
                    ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(contrato.ID, null);
                    nome = titular.BeneficiarioNome;
                    cpf = titular.BeneficiarioCPF;
                }

                sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
                sb.Append("<head><title></title></head>");
                sb.Append("<body style=\"font-family:arial\"> ");
                sb.Append("<table width=\"99%\" align=\"center\">");
                sb.Append("<tr>");
                sb.Append("<td align=\"right\"><img src=\"http://linkecerebro.com.br/clientes/qualicorp/qualicorp.jpg\" width='180' /></td>");  //sb.Append("<td align=\"right\"><img src=\"https://sistemas.qualicorp.com.br/images/qualicorp.jpg\" width='180px' /></td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<tr>");
                sb.Append("<td align=\"left\">Cliente Sr(a). "); sb.Append(nome); sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td align=\"left\">CPF: "); sb.Append(cpf); sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("<br /><br /><br /><br /><br /><br />");

                sb.Append("<table width=\"99%\" align=\"center\">");
                sb.Append("<tr><td align=\"left\">Ref.: DECLARAÇÃO DE QUITAÇÃO ANUAL DE DÉBITOS</td></tr>");
                sb.Append("</table>");
                sb.Append("<br /><br /><br /><br /><br /><br />");

                sb.Append("<table width=\"99%\" align=\"center\">");
                sb.Append("<tr><td align=\"left\">Em cumprimento à Lei nº 12.007, de 29 de julho de 2009, declaramos que o cliente acima está quite com suas faturas vencidas no ano de 2013 e anos anteriores.</td></tr>");
                sb.Append("</table>");
                sb.Append("<br />");

                sb.Append("<table width=\"99%\" align=\"center\">");
                sb.Append("<tr><td align=\"left\">Atenciosamente,</td></tr>");
                sb.Append("</table>");
                sb.Append("<br />");

                sb.Append("<table width=\"99%\" align=\"center\">");
                sb.Append("<tr><td align=\"left\" colspan=\"2\">QUALICORP ADM. E SERV. LTDA</td></tr>");
                sb.Append("<tr><td align=\"left\">CNPJ 03.609.855/0001-02</td><td align=\"right\">www.qualicorp.com.br</td></tr>");
                sb.Append("</table>");
                sb.Append("<br />");

                sb.Append("<table width=\"99%\" align=\"center\">");
                sb.Append("<tr><td width=\"350\" align=\"left\">TODO O BRASIL (exceto RJ)</td><td align=\"left\">RIO DE JANEIRO</td></tr>");
                sb.Append("<tr><td align=\"left\"><b>0800-16-2000</b></td><td align=\"left\"><b>0800-778-4004</b></td></tr>");
                sb.Append("</table>");
                sb.Append("<br />");

                sb.Append("<br><br><font size='1' color='red'>Este é um e-mail automático. Por favor, não o responda.</font>");
                sb.Append("</body>");
                sb.Append("</html>");

                #endregion

                sb.Append("]]></corpo>");
            }
            else
            {
                sb.Append("0</resultado>");
            }

            sb.Append("</envelope>");

            return sb.ToString();
        }

        bool checaRegraTermoAnual(string contratoId)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(contratoId, pm);
            if (titular.BeneficiarioNome.IndexOf('*') > -1) { pm.CloseSingleCommandInstance(); return false; }

            Contrato contrato = new Contrato(contratoId);
            contrato.Carregar();

            if (titular.BeneficiarioDataNascimento != DateTime.MinValue)
            {
                int idade = Beneficiario.CalculaIdade(titular.BeneficiarioDataNascimento);
                if (idade < 18 && string.IsNullOrEmpty(contrato.ResponsavelNome)) { pm.CloseSingleCommandInstance(); return false; }
            }

            string qry = "* from cobranca left JOIN cobranca_parcelamentoCobrancaOriginal ON cobranca_id = parccob_cobrancaId  LEFT JOIN cobranca_parcelamentoItem ON cobranca_id = parcitem_cobrancaId where year(cobranca_datavencimento)=2013 and (cobranca_cancelada is null or cobranca_cancelada = 0) and cobranca_propostaId=" + contrato.ID;
            qry += " order by cobranca_datavencimento";

            IList<Cobranca> cobrancas2013 = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca));

            //se nao tem cobrancas no periodo
            if (cobrancas2013 == null || cobrancas2013.Count == 0)
            {
                pm.CloseSingleCommandInstance(); pm.Dispose(); return false;
            }

            //checa inadimplencia
            foreach (Cobranca cob in cobrancas2013)
            {
                if (contrato.Cancelado || contrato.Inativo)
                {
                    if (cob.DataVencimento > contrato.DataCancelamento && cob.Tipo != (int)Cobranca.eTipo.Parcelamento)
                        continue;
                }

                if (!cob.Pago && cob.Tipo != (int)Cobranca.eTipo.Parcelamento && cob.HeaderParcID == null)
                {
                    //cobrancas nao parceladas
                    pm.CloseSingleCommandInstance(); pm.Dispose(); return false;
                }
                else if (!cob.Pago && cob.Tipo == (int)Cobranca.eTipo.Parcelamento)
                {
                    //checa se a negociacao 
                    pm.CloseSingleCommandInstance(); pm.Dispose(); return false;
                }
            }

            //valida quanto às baixas liminares e provisorias
            CobrancaBaixa baixa = null;
            IList<Cobranca> todas = Cobranca.CarregarTodas(contratoId, true, pm);
            foreach (Cobranca cob in todas)
            {
                baixa = CobrancaBaixa.CarregarUltima(cob.ID, pm);
                if (baixa == null) continue;

                if (baixa.BaixaProvisoria && baixa.Tipo == 0)
                { pm.CloseSingleCommandInstance(); pm.Dispose(); return false; }

                if ((Convert.ToString(baixa.MotivoID) == "12" || Convert.ToString(baixa.MotivoID) == "13") && baixa.Tipo == 0)
                { pm.CloseSingleCommandInstance(); pm.Dispose(); return false; }
            }

            pm.CloseSingleCommandInstance();
            pm.Dispose();

            return true;
        }

        //[WebMethod(Description = "Retorna uma string xml com os dados do demonstrativo de pagamento. Método para PS Padrão.")]
        //public String DemonstrativoPagto(String param)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    String ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"];
        //    String contratoId = param.Split(';')[0];

        //    PersistenceManager pm = new PersistenceManager();
        //    pm.UseSingleCommandInstance();

        //    IList<CobrancaComposite> comp = null;

        //    try
        //    {
        //        Contrato contrato = new Contrato(contratoId);
        //        pm.Load(contrato);
        //        Operadora operadora = new Operadora(contrato.OperadoraID);
        //        pm.Load(operadora);

        //        ContratoBeneficiario cTitular = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);

        //        IList<ContratoBeneficiario> beneficiarios =
        //            ContratoBeneficiario.CarregarPorContratoID(contratoId, false, false, pm);
        //        IList<Cobranca> cobrancas = separaCobrancasDoAno(contratoId, ano, pm, cTitular, false);

        //        sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        //        sb.Append("<envelope>");

        //        sb.Append("<operadora><![CDATA[");
        //        if (operadora.Nome.IndexOf("-") > -1)
        //            sb.Append(operadora.Nome.Split('-')[1].Trim());
        //        else
        //            sb.Append(operadora.Nome);
        //        sb.Append("]]></operadora>");

        //        System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

        //        configuraDadosDeCadaMes(cobrancas, ref sb, cinfo, false);

        //        Decimal valorPerBenef = 0, total = 0;
        //        sb.Append("<beneficiarios>");

        //        Boolean calculadoViaComposite = false;
        //        foreach (ContratoBeneficiario cb in beneficiarios)
        //        {
        //            valorPerBenef = 0;
        //            calculadoViaComposite = false;

        //            foreach (Cobranca cobr in cobrancas)
        //            {
        //                if (!cb.Ativo && cb.DataInativacao != DateTime.MinValue)
        //                {
        //                    cb.DataInativacao = new DateTime(cb.DataInativacao.Year,
        //                        cb.DataInativacao.Month, cb.DataInativacao.Day, 23, 59, 59, 998);
        //                    if (cb.DataInativacao < cobr.DataVencimento) { continue; }
        //                }
        //                else if (!cb.Ativo)
        //                {
        //                    comp = CobrancaComposite.Carregar(cobr.ID, pm);
        //                    if (comp == null)
        //                    {
        //                        continue;
        //                    }
        //                    else
        //                    {
        //                        foreach (CobrancaComposite item in comp)
        //                        {
        //                            if ((item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Plano ||
        //                                item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Adicional) &&
        //                                item.BeneficiarioID != null &&
        //                                Convert.ToString(item.BeneficiarioID) == Convert.ToString(cb.BeneficiarioID))
        //                            {
        //                                valorPerBenef += item.Valor;
        //                            }
        //                        }
        //                        calculadoViaComposite = true;
        //                    }
        //                } ///////demonstrativo 2

        //                if (!calculadoViaComposite)
        //                {
        //                    comp = CobrancaComposite.Carregar(cobr.ID, pm);
        //                    if (comp == null)
        //                    {
        //                        valorPerBenef +=
        //                            Contrato.CalculaValorDaPropostaSemTaxaAssociativa(
        //                            contrato.ID, cb, cobr.DataVencimento, pm);
        //                    }
        //                    else
        //                    {
        //                        foreach (CobrancaComposite item in comp)
        //                        {
        //                            if ((item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Plano ||
        //                                item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Adicional) &&
        //                                item.BeneficiarioID != null &&
        //                                Convert.ToString(item.BeneficiarioID) == Convert.ToString(cb.BeneficiarioID))
        //                            {
        //                                valorPerBenef += item.Valor;
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            if (valorPerBenef > 0)
        //            {
        //                sb.Append("<beneficiario>");
        //                sb.Append("<nome><![CDATA["); sb.Append(cb.BeneficiarioNome); sb.Append("]]></nome>");
        //                sb.Append("<tipo>");
        //                if (cb.Tipo == 0)
        //                    sb.Append("titular");
        //                else
        //                    sb.Append("dependente");
        //                sb.Append("</tipo>");

        //                sb.Append("<valor>");
        //                sb.Append(valorPerBenef.ToString("N2", cinfo));
        //                sb.Append("</valor>");

        //                sb.Append("</beneficiario>");

        //                total += valorPerBenef;
        //            }
        //        }

        //        sb.Append("</beneficiarios>");
        //        sb.Append("<total>");
        //        sb.Append(total.ToString("N2", cinfo));
        //        sb.Append("</total>");

        //        sb.Append("</envelope>");

        //        return sb.ToString();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        pm.CloseSingleCommandInstance();
        //        pm.Dispose();
        //    }
        //}

        //[WebMethod(Description = "Retorna uma string xml com os dados do demonstrativo de pagamento. Método para Qualicorp.")]
        //public String DemonstrativoPagtoQuali(String param)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    String ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"];
        //    String contratoId = param.Split(';')[0];

        //    PersistenceManager pm = new PersistenceManager();
        //    pm.UseSingleCommandInstance();

        //    IList<CobrancaComposite> comp = null;

        //    try
        //    {
        //        Contrato contrato = new Contrato(contratoId);
        //        pm.Load(contrato);
        //        Operadora operadora = new Operadora(contrato.OperadoraID);
        //        pm.Load(operadora);

        //        ContratoBeneficiario cTitular = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);

        //        IList<ContratoBeneficiario> beneficiarios =
        //            ContratoBeneficiario.CarregarPorContratoID(contratoId, false, false, pm);
        //        IList<Cobranca> cobrancas = separaCobrancasDoAno(contratoId, ano, pm, cTitular, true);

        //        sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        //        sb.Append("<envelope>");

        //        sb.Append("<operadora><![CDATA[");
        //        if (operadora.Nome.IndexOf("-") > -1)
        //            sb.Append(operadora.Nome.Split('-')[1].Trim());
        //        else
        //            sb.Append(operadora.Nome);
        //        sb.Append("]]></operadora>");

        //        System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

        //        configuraDadosDeCadaMes(cobrancas, ref sb, cinfo, true);

        //        Decimal valorPerBenef = 0, total = 0;
        //        sb.Append("<beneficiarios>");

        //        Boolean calculadoViaComposite = false;
        //        foreach (ContratoBeneficiario cb in beneficiarios)
        //        {
        //            valorPerBenef = 0;
        //            calculadoViaComposite = false;

        //            foreach (Cobranca cobr in cobrancas)
        //            {
        //                if (!cb.Ativo && cb.DataInativacao != DateTime.MinValue)
        //                {
        //                    cb.DataInativacao = new DateTime(cb.DataInativacao.Year,
        //                        cb.DataInativacao.Month, cb.DataInativacao.Day, 23, 59, 59, 998);
        //                    if (cb.DataInativacao < cobr.DataVencimento) { continue; }
        //                }
        //                else if (!cb.Ativo)
        //                {
        //                    comp = CobrancaComposite.Carregar(cobr.ID, pm);
        //                    if (comp == null)
        //                    {
        //                        continue;
        //                    }
        //                    else
        //                    {
        //                        foreach (CobrancaComposite item in comp)
        //                        {
        //                            if ((item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Plano ||
        //                                item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Adicional) &&
        //                                item.BeneficiarioID != null &&
        //                                Convert.ToString(item.BeneficiarioID) == Convert.ToString(cb.BeneficiarioID))
        //                            {
        //                                valorPerBenef += item.Valor;
        //                            }
        //                        }
        //                        calculadoViaComposite = true;
        //                    }
        //                } ///////demonstrativo 2

        //                if (!calculadoViaComposite)
        //                {
        //                    comp = CobrancaComposite.Carregar(cobr.ID, pm);
        //                    if (comp == null)
        //                    {
        //                        valorPerBenef +=
        //                            Contrato.CalculaValorDaPropostaSemTaxaAssociativa(
        //                            contrato.ID, cb, cobr.DataVencimento, pm);
        //                    }
        //                    else
        //                    {
        //                        foreach (CobrancaComposite item in comp)
        //                        {
        //                            if ((item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Plano ||
        //                                item.Tipo == (int)CobrancaComposite.eComposicaoTipo.Adicional) &&
        //                                item.BeneficiarioID != null &&
        //                                Convert.ToString(item.BeneficiarioID) == Convert.ToString(cb.BeneficiarioID))
        //                            {
        //                                valorPerBenef += item.Valor;
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            if (valorPerBenef > 0)
        //            {
        //                sb.Append("<beneficiario>");
        //                sb.Append("<nome><![CDATA["); sb.Append(cb.BeneficiarioNome); sb.Append("]]></nome>");
        //                sb.Append("<tipo>");
        //                if (cb.Tipo == 0)
        //                    sb.Append("titular");
        //                else
        //                    sb.Append("dependente");
        //                sb.Append("</tipo>");

        //                sb.Append("<valor>");
        //                sb.Append(valorPerBenef.ToString("N2", cinfo));
        //                sb.Append("</valor>");

        //                sb.Append("</beneficiario>");

        //                total += valorPerBenef;
        //            }
        //        }

        //        sb.Append("</beneficiarios>");
        //        sb.Append("<total>");
        //        sb.Append(total.ToString("N2", cinfo));
        //        sb.Append("</total>");

        //        sb.Append("</envelope>");

        //        return sb.ToString();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        pm.CloseSingleCommandInstance();
        //        pm.Dispose();
        //    }
        //}

        IList<Cobranca> separaCobrancasDoAno(String contratoId, String ano, PersistenceManager pm, ContratoBeneficiario cTitular, Boolean qualicorp)
        {
            List<CobrancaComposite> comp = new List<CobrancaComposite>();
            IList<CobrancaComposite> icomp = null;
            IList<Cobranca> temp = Cobranca.CarregarTodas(contratoId, true, pm);
            List<Cobranca> cobrancas = new List<Cobranca>();
            ContratoBeneficiario depend = null;

            foreach (Cobranca cob in temp)
            {
                if (cob.DataVencimento.Year.ToString() != ano || !cob.Pago || cob.Parcela == 1) { continue; }

                if (qualicorp)
                {
                    if (cob.DataVencimento.Month < 10) { continue; }
                }
                else
                {
                    if (cob.DataVencimento.Month > 9) { continue; }
                }

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
        void configuraDadosDeCadaMes(IList<Cobranca> cobrancas, ref StringBuilder sb, System.Globalization.CultureInfo cinfo, bool qualicorp)
        {
            if (cobrancas == null || cobrancas.Count == 0) { return; }
            sb.Append("<meses>");
            foreach (Cobranca cob in cobrancas)
            {
                sb.Append("<mes>");

                if (qualicorp)
                {
                    #region 

                    switch (cob.DataVencimento.Month)
                    {
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
                    #endregion
                }
                else
                {
                    #region 

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
                    }
                    #endregion
                }

                sb.Append("<valor>");
                sb.Append(cob.Valor.ToString("N2", cinfo));
                sb.Append("</valor>");
                sb.Append("</mes>");
            }
            sb.Append("</meses>");
        }

        //[WebMethod(Description = "Retorna uma string xml com os dados da carta de quitação.")]
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

                return sb.ToString(); //cTitular.BeneficiarioNome + ";" + cTitular.BeneficiarioCPF; //
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

        string CToString(object param)
        {
            if (param == null || param == DBNull.Value)
                return string.Empty;
            else
                return Convert.ToString(param);
        }

        Decimal CToDecimal(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
                return Convert.ToDecimal(param, new System.Globalization.CultureInfo("pt-Br"));
        }

        DateTime CToDateTime(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return DateTime.MinValue;
            else
            {
                try
                {
                    return Convert.ToDateTime(param, new System.Globalization.CultureInfo("pt-Br"));
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }

        //-----------------------------------------------------

        /// <summary>
        /// http://www.sanity-free.org/131/triple_des_between_php_and_csharp.html
        /// http://www.sanity-free.org/forum/viewtopic.php?id=133
        /// </summary>
        string EncryptBetweenPHP(string param)
        {
            byte[] key = Encoding.UTF8.GetBytes("passwordDR0wSS@P6660juht");
            byte[] iv = Encoding.UTF8.GetBytes("password");
            byte[] data = Encoding.UTF8.GetBytes(param);
            byte[] enc = new byte[0];
            TripleDES tdes = TripleDES.Create();
            tdes.IV = iv;
            tdes.Key = key;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.Zeros;
            ICryptoTransform ict = tdes.CreateEncryptor();
            enc = ict.TransformFinalBlock(data, 0, data.Length);
            return Bin2Hex(enc);
        }

        static string DecryptBetweenPHP(string Data)
        {
            byte[] key = Encoding.UTF8.GetBytes("passwordDR0wSS@P6660juht");
            byte[] iv = Encoding.UTF8.GetBytes("password");
            byte[] data = StringToByteArray(Data);
            byte[] enc = new byte[0];
            TripleDES tdes = TripleDES.Create();
            tdes.IV = iv;
            tdes.Key = key;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.Zeros;
            ICryptoTransform ict = tdes.CreateDecryptor();
            enc = ict.TransformFinalBlock(data, 0, data.Length);
            return Encoding.UTF8.GetString(enc);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        string Bin2Hex(byte[] bin)
        {
            StringBuilder sb = new StringBuilder(bin.Length * 2);
            foreach (byte b in bin)
            {
                sb.Append(b.ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        string RetiraAcentos(String Texto)
        {
            if (String.IsNullOrEmpty(Texto)) { return Texto; }
            String comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            String semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
                Texto = Texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());

            return Texto.Replace("'", "");
        }

        string corpoBoletoTexto(string link)
        {
            return string.Concat(
               "<br/><br/>",
                "<a href='", link, "'>",
                "Voce está recebendo um boleto bancário via email enviado pela QUALICORP ADM. E SERV. LTDA.",
                "<br/><br/>",
                "Clique aqui para gerar seu boleto (voce deve estar conectado a internet para gera-lo).",
                "</a><br/><br/>",
                "Em caso de dúvidas, entre em contato com ps_boletos@qualicorp.com.br");
        }

        //------------------------------------------------------
    }

    //public class WSCredential : SoapHeader
    //{
    //    String _pwd;

    //    public String PWD
    //    {
    //        get { return _pwd; }
    //        set { _pwd= value; }
    //    }
    //}
}
namespace www.financeiro
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;
    using System.Data;
    using System.Data.SqlClient;

    public partial class exportartxt : PageBase
    {
        protected void Page_Load(Object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        //Boolean casoExcepcional(Object cobrancaId)
        //{
        //    String strcobrancaid = Convert.ToString(cobrancaId);

        //    String[] arr = new String[] { "1341378", "1359474", "1401856", "1402195", "1402371", "1402401", "1402571", "1402800", "1403823", "1404295", "1404808", "1405117", "1405794", "1406301", "1406604", "1406657", "1407146", "1412645", "1412881", "1412999", "1413145", "1358872", "1397180", "1399695", "1400134", "1400536", "1401294", "1402058", "1402725", "1403437", "1404050", "1406086", "1406278", "1406302", "1406658", "1406977", "1407047", "1407335", "1412721", "1412800", "1413050", "1413289", "1413345", "1344724", "1384671", "1357372", "1358517", "1359451", "1401187", "1402509", "1402766", "1403134", "1403581", "1403775", "1404041", "1404220", "1404845", "1404945", "1405274", "1405582", "1405674", "1405788", "1406013", "1406080", "1406135", "1406273", "1406394", "1406532", "1406680", "1412623", "1412867", "1413192", "1413271", "1413333", "1344461", "1357178", "1357880", "1359098", "1359225", "1361039", "1400184", "1400446", "1400682", "1402516", "1403136", "1403432", "1404253", "1404855", "1404967", "1406276", "1406399", "1406513", "1406784", "1407265", "1407379", "1412507", "1412715", "1413144", "1343825", "1344654", "1373355", "1400680", "1402319", "1402369", "1402510", "1402704", "1403295", "1403674", "1403890", "1404806", "1404966", "1405962", "1406250", "1406345", "1406570", "1406933", "1406951", "1406973", "1407354", "1407378", "1412577", "1413203", "1413273", "1340769", "1357771", "1402310", "1403131", "1403280", "1403412", "1403923", "1403955", "1404923", "1405021", "1405649", "1405814", "1405875", "1406190", "1406365", "1406508", "1407303", "1412773", "1413022", "1413182", "1413262", "1342830", "1344140", "1344320", "1357410", "1358792", "1400369", "1401017", "1401992", "1402324", "1402465", "1402616", "1402726", "1402875", "1403834", "1404102", "1404430", "1404973", "1405885", "1406141", "1406235", "1406255", "1406279", "1406638", "1406856", "1407031", "1412524", "1412889", "1413220", "1343455", "1345552", "1358134", "1359345", "1401888", "1402038", "1402762", "1404880", "1404964", "1405057", "1405658", "1406212", "1406781", "1407019", "1407093", "1407328", "1412571", "1412982", "1413263", "1343607", "1357673", "1399769", "1400463", "1401303", "1402155", "1404016", "1404325", "1404667", "1405739", "1406354", "1406377", "1406661", "1407006", "1412823", "1413058", "1359322", "1361062", "1399603", "1401544", "1402222", "1403123", "1403171", "1403456", "1404723", "1406125", "1406647", "1406794", "1412755", "1413309", "1343005", "1359024", "1385393", "1400629", "1400720", "1401154", "1402487", "1402694", "1403603", "1403877", "1404244", "1404910", "1405084", "1405643", "1405872", "1406187", "1406338", "1406648", "1412933", "1413019", "1413242", "1413376", "1358999", "1400193", "1400716", "1400945", "1401877", "1402007", "1402680", "1403873", "1403909", "1403937", "1404020", "1404061", "1404327", "1404412", "1404820", "1405129", "1405637", "1405866", "1406307", "1406407", "1406524", "1406642", "1407152", "1407210", "1407455", "1412741", "1412830", "1413297", "1343977", "1357675", "1359142", "1368467", "1402211", "1402334", "1402385", "1403520", "1403593", "1403872", "1404059", "1404411", "1405036", "1405096", "1405428", "1406067", "1406496", "1406941", "1407127", "1412597", "1412826", "1412903", "1413157", "1413296", "1413353", "1343855", "1344336", "1372253", "1399673", "1400151", "1400389", "1401043", "1402479", "1402993", "1403061", "1403117", "1403268", "1403487", "1404824", "1404904", "1405013", "1405189", "1405470", "1406147", "1406239", "1406336", "1406445", "1406587", "1406792", "1407276", "1412603", "1412835", "1413013", "1413087", "1413308", "1357155", "1364939", "1399730", "1399938", "1401159", "1402390", "1403064", "1403275", "1403461", "1403803", "1404335", "1404829", "1405134", "1405169", "1406009", "1406697", "1406753", "1407219", "1407476", "1408900", "1412612", "1412763", "1413379", "1344000", "1344634", "1359337", "1400498", "1400724", "1402498", "1403279", "1403411", "1403996", "1405270", "1405747", "1405874", "1406270", "1406754", "1406822", "1407039", "1413384" };

        //    foreach (String arritem in arr)
        //    {
        //        if (arritem == strcobrancaid) { return true; }
        //    }

        //    return false;
        //}

        Boolean casoExcepcional(Object cobrancaId, List<String> excepcionais)
        {
            String strcobrancaid = Convert.ToString(cobrancaId);

            foreach (String arritem in excepcionais)
            {
                if (arritem == strcobrancaid) { return true; }
            }

            return false;
        }

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            DateTime dtDe = base.CStringToDateTime(txtDe.Text);
            DateTime dtAte = base.CStringToDateTime(txtAte.Text);

            if (dtDe == DateTime.MinValue || dtAte == DateTime.MinValue || (dtAte < dtDe)) { return; }

            String qry = String.Concat("select cobranca_arquivoUltimoEnvioId, contrato_codcobranca, cobranca_tipo, contrato_contratoAdmId, contrato_admissao, cobranca_id, cobranca_parcela, beneficiario_cpf,operadora_nome,beneficiario_nome,cobranca_dataVencimento,cobranca_valor,cobranca_nossoNumero,contrato_numero ",
	            "   from beneficiario ",
		        "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 ",
		        "       inner join contrato on contrato_id=contratobeneficiario_contratoId and contrato_cancelado <> 1 ",
		        "       inner join operadora on operadora_id=contrato_operadoraId ",
		        "       inner join cobranca on cobranca_propostaId=contrato_id ",
                "   where ",
//                "       cobranca_pago=0 AND cobranca_id in (select arqitem_cobrancaId from arquivoCobrancaUnibanco_cobanca where arqitem_arquivoId >= 808 and arqitem_arquivoId <= 881) ORDER BY beneficiario_cpf, cobranca_parcela");
                "       cobranca_pago=0 AND cobranca_dataVencimento BETWEEN '", dtDe.ToString("yyyy-MM-dd 00:00:00.000"), "' AND '", dtAte.ToString("yyyy-MM-dd 23:59:59.998"), "' ORDER BY beneficiario_cpf, cobranca_parcela");

            #region carrega excepcionais - deletar depois denis

            //DataTable dt = LocatorHelper.Instance.ExecuteQuery("select arqitem_cobrancaId from arquivoCobrancaUnibanco_cobanca where arqitem_arquivoId >= 808 and arqitem_arquivoId <= 881 ", "resultset").Tables[0];
            //List<String> excepcionais = new List<String>();
            //foreach (DataRow row in dt.Rows)
            //{
            //    excepcionais.Add(Convert.ToString(row[0]));
            //}

            #endregion

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
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

                if (ds.Tables[0].Rows.Count == 0) { return; }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                String nossoNumero = "";
                DateTime dataVencimento, vigencia, vencimento;
                Int32 diaDataSemJuros, aux = 0;
                Decimal Valor;
                Object valorDataLimite;
                CalendarioVencimento rcv = null; Cobranca cobranca = new Cobranca();

                String msgLimitePagto = "";
                //Boolean excepcional = false;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (sb.Length > 0) { sb.Append(Environment.NewLine); }

                    //excepcional = casoExcepcional(row["cobranca_id"], excepcionais);

                    CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(row["contrato_contratoAdmId"],
                        Convert.ToDateTime(row["contrato_admissao"]), out vigencia, out vencimento, 
                        out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                    sb.Append(row["beneficiario_cpf"]);
                    sb.Append(";");
                    sb.Append(Convert.ToString(row["operadora_nome"]).Split('-')[1].Trim());
                    sb.Append(";");
                    sb.Append(row["beneficiario_nome"]);
                    sb.Append(";");
                    //link
                    sb.Append("http://www.boletomail.com.br/do.php?nossonum=");

                    if (row["cobranca_nossonumero"] != DBNull.Value)
                    {
                        nossoNumero = Convert.ToString(row["cobranca_nossonumero"]).Substring(1);
                    }
                    else
                    {
                        cobranca.Tipo = Convert.ToInt32(row["cobranca_tipo"]);
                        cobranca.ContratoCodCobranca = Convert.ToString(row["contrato_codcobranca"]);
                        cobranca.Parcela = Convert.ToInt32(row["cobranca_parcela"]);

                        nossoNumero = cobranca.GeraNossoNumero().Substring(1);
                    }

                    sb.Append(nossoNumero.Substring(0, nossoNumero.Length - 1));

                    //if (excepcional) //temporario denis
                    //{
                    //    dataVencimento = new DateTime(2011, 9, 21, 23, 59, 59, 998);
                    //}
                    //else
                    //{
                        dataVencimento = Convert.ToDateTime(row["cobranca_dataVencimento"]);
                    //}
                    Valor = Convert.ToDecimal(row["cobranca_valor"]);

                    //if (excepcional) //temporario denis
                    //{
                    //    msgLimitePagto = String.Concat("<br><br>Excepcionalmente neste mes o vencimento foi alterado para dia 21 e data limite dia 26.",
                    //        "<br>",
                    //        "Nao receber apos 26/09/2011.");
                    //}
                    //else
                    //{
                        if (valorDataLimite == null)
                            msgLimitePagto = "";
                        else
                        {
                            if (Int32.TryParse(Convert.ToString(valorDataLimite), out aux))
                                msgLimitePagto = "<br><br>Nao receber apos o dia " + aux.ToString() + "/" + dataVencimento.Month.ToString() + "/" + dataVencimento.Year.ToString() + ".";
                            else
                                msgLimitePagto = "<br><br>Nao receber apos " + Convert.ToString(valorDataLimite).ToLower() + ".";
                        }
                    //}

                    sb.Append(String.Concat("&valor=", Valor, "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dataVencimento.Day, "&v_mes=", dataVencimento.Month, "&v_ano=", dataVencimento.Year, "&numdoc2=", row["contrato_numero"], "&nome=", row["beneficiario_nome"], "&cod_cli=", row["cobranca_id"], "&action=1&user=padraovida&cod_config=1&instr=<br>Excepcionalmente neste vencimento, isento de multa e juros.", msgLimitePagto, "<br>"));
                    //sb.Append(Cobranca.BoletoUrlComp);
                    sb.Append(";");
                }

                pm.CloseSingleCommandInstance();
                pm.Dispose();
                ds.Dispose();

                String basepath = Server.MapPath("/") + System.Configuration.ConfigurationManager.AppSettings["otherFilePath"].Replace("/", "\\");
                if (!Directory.Exists(basepath)) { Directory.CreateDirectory(basepath); }

                String fileName = "exp2viacob" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".dat";

                File.WriteAllText(String.Concat(basepath, fileName), sb.ToString());

                base.BaixarArquivo(String.Concat(basepath, fileName), fileName);
            }
        }
    }
}

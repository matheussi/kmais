namespace www.financeiro
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using System.Data.OleDb;
    using System.Data.SqlClient;
    using LC.Web.PadraoSeguros.Entity;

    public partial class painel : PageBase
    {
        String arquivoNome
        {
            get { return ViewState["_fn"] as String; }
            set { ViewState["_fn"] = value; }
        }

        String caminhoBase
        {
            get
            {
                return String.Concat(Server.MapPath("/"), "/", ConfigurationSettings.AppSettings["bill_file"]);
            }
        }

        String arquivoExcel
        {
            get { return @"C:\inetpub\wwwroot\PadraoSeguros\www\var\bill_file\nt1.xlsx"; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                txtCompetencia.Text = DateTime.Now.AddMonths(-1).ToString("MM/yyyy");
                txtDataCorte10.Text = DateTime.Now.AddMonths(-1).ToString("10/MM");
                txtDataCorte20.Text = DateTime.Now.AddMonths(-1).ToString("20/MM");
                txtCompetencia.Focus();
            }
        }

        protected void cmdEnviar_Click(Object sender, EventArgs e)
        {
            if (upl.PostedFile != null && upl.PostedFile.FileName != null)
            {
                arquivoNome = System.IO.Path.GetFileName(upl.PostedFile.FileName);

                upl.SaveAs(caminhoBase + arquivoNome);

                lblArquivoEnviado.Text = arquivoNome;
            }
        }

        protected void cmdEmitir_Click(object sender, EventArgs e)
        {
            DataSet dsExcel = new DataSet();DataTable dtExcel=new DataTable();

            if (!String.IsNullOrEmpty(arquivoNome))
            {
                using (OleDbConnection excelConn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + arquivoExcel + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\";"))
                {
                    excelConn.Open();
                    OleDbDataAdapter adp = new OleDbDataAdapter("select * from [plan1$]", excelConn);
                    adp.Fill(dsExcel);
                    adp.Dispose();
                    excelConn.Close();
                }

                dtExcel = dsExcel.Tables[0];
            }

            dsExcel.Dispose();

            String[] arrCompetencia = txtCompetencia.Text.Split('/');
            DateTime competencia = base.UltimoDiaDoMes(Convert.ToInt32(arrCompetencia[0]), Convert.ToInt32(arrCompetencia[1]));
            competencia = new DateTime(competencia.Year, competencia.Month, competencia.Day, 23, 59, 59, 998);

            String qry = String.Concat("select cobranca.*,contrato_numero,contrato_cancelado,contrato_dataCancelamento,operadora_id,operadora_nome,estipulante_id,estipulante_descricao,contratoadm_id,contratoadm_descricao,contratoadm_numero,contratoadm_contratoSaude, contratoadm_contratoDental from cobranca ",
                "   inner join contrato on contrato_id=cobranca_propostaId ",
                "   inner join operadora on operadora_id=contrato_operadoraId ",
                "   inner join estipulante on contrato_estipulanteid=estipulante_id ",
                "   inner join contratoAdm on contrato_contratoadmid=contratoadm_id ",
                //"   inner join contrato_beneficiario on contratobeneficiario_contratoId=contrato_id and contratobeneficiario_ativo=1 ",
                //"   inner join beneficiario on contratobeneficiario_beneficiarioId=beneficiario_id ",
                "   inner join usuario on usuario_id=contrato_donoId ",
                "   where ",
                "   (month(cobranca_dataVencimento)=", arrCompetencia[0], " and year(cobranca_dataVencimento)=", arrCompetencia[1], ") or ",
                "   (month(cobranca_dataPagto)=", arrCompetencia[0], " and year(cobranca_dataPagto)=", arrCompetencia[1], ") ",
                "   order by cobranca_id");
                DataSet ds = new DataSet();

            //List<String> idsCobrancasProcessadas = new List<String>();
            String cobrancaIdCorrente = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter(qry, conn);
                adp.Fill(ds);

                //DataTable operadoras = ds.Tables[0].DefaultView.ToTable(true, new String[] { "operadora_nome", "estipulante_descricao" });
                //DataRow[] arrOperadoras = operadoras.Select("operadora_nome is not null", "operadora_nome,estipulante_descricao");

                DataTable operadoras = ds.Tables[0].DefaultView.ToTable(true, new String[] { "operadora_nome" });
                DataRow[] arrOperadoras = operadoras.Select("operadora_nome is not null", "operadora_nome");

                DataTable dtFinal = new DataTable();
                dtFinal.Columns.Add("operadora_nome");
                dtFinal.Columns.Add("estipulante_descricao");
                dtFinal.Columns.Add("total_boletado");  //parcela > 1
                dtFinal.Columns.Add("venda_nova");      //parcela = 1
                dtFinal.Columns.Add("total_cancelado"); //boletos em aberto de contratos cancelados no mes atual ou mes + 1
                dtFinal.Columns.Add("em_aberto");
                dtFinal.Columns.Add("atrasado");
                dtFinal.Columns.Add("antecipado");
                dtFinal.Columns.Add("fatura");
                dtFinal.Columns.Add("faturaCopart");

                DataRow[] rows = null; //Object ret = null;
                Decimal valorCancelado = 0, valorEmAberto = 0, atrasado = 0, antecipado = 0, boletado = 0, vendaNova = 0;
                DateTime vencto, pagto, data;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
                foreach (DataRow operadora in arrOperadoras)
                {
                    valorCancelado = 0; valorEmAberto = 0; atrasado = 0; antecipado = 0; boletado = 0; vendaNova = 0;

                    DataRow row = dtFinal.NewRow();
                    row["operadora_nome"] = operadora["operadora_nome"];
                    //row["estipulante_descricao"] = operadora["estipulante_descricao"];

                    //rows = ds.Tables[0].Select(String.Concat("operadora_nome='", Convert.ToString(row["operadora_nome"]), "' AND estipulante_descricao='", row["estipulante_descricao"], "'"));
                    rows = ds.Tables[0].Select(String.Concat("operadora_nome='", Convert.ToString(row["operadora_nome"]), "'"));
                    foreach (DataRow _subrow in rows)
                    {
                        if (cobrancaIdCorrente == Convert.ToString(_subrow["cobranca_id"])) { continue; }
                        cobrancaIdCorrente = Convert.ToString(_subrow["cobranca_id"]);

                        vencto = Convert.ToDateTime(_subrow["cobranca_dataVencimento"], cinfo);
                        vencto = new DateTime(vencto.Year, vencto.Month, vencto.Day, 23, 59, 59, 998);

                        if (vencto.Month == competencia.Month && vencto.Year == competencia.Year)
                        {
                            if (_subrow["contrato_dataCancelamento"] == DBNull.Value ||
                                Convert.ToDateTime(_subrow["contrato_dataCancelamento"], cinfo) > vencto)
                            {
                                //venda nova
                                if (Convert.ToInt32(_subrow["cobranca_parcela"]) == 1)
                                {
                                    vendaNova += Convert.ToDecimal(_subrow["cobranca_valor"], cinfo);
                                }
                                //total boletado (parcela > 1)
                                else
                                {
                                    boletado += Convert.ToDecimal(_subrow["cobranca_valor"], cinfo);
                                }
                            }
                        }

                        vencto = Convert.ToDateTime(_subrow["cobranca_dataVencimento"], cinfo);
                        vencto = UltimoDiaDoMes(vencto.Month, vencto.Year);
                        vencto = new DateTime(vencto.Year, vencto.Month, vencto.Day, 23, 59, 59, 998);

                        //cancelado
                        if (Convert.ToInt32(_subrow["contrato_cancelado"]) == 1 && Convert.ToInt32(_subrow["cobranca_pago"]) == 0)
                        {
                            if (_subrow["contrato_dataCancelamento"] == DBNull.Value) { continue; }
                            data = Convert.ToDateTime(_subrow["contrato_dataCancelamento"], cinfo);

                            if (data.Year == DateTime.Now.Year && (data.Month == DateTime.Now.Month || data.Month == (DateTime.Now.Month + 1)))
                            {
                                valorCancelado += Convert.ToDecimal(_subrow["cobranca_valor"], cinfo);
                            }
                        }
                        //em aberto
                        if (Convert.ToInt32(_subrow["contrato_cancelado"]) == 0 && Convert.ToInt32(_subrow["cobranca_pago"]) == 0 &&
                            Convert.ToInt32(_subrow["cobranca_cancelada"]) == 0)
                        {
                            valorEmAberto += Convert.ToDecimal(_subrow["cobranca_valor"], cinfo);
                        }
                        if (Convert.ToInt32(_subrow["contrato_cancelado"]) == 0 && Convert.ToInt32(_subrow["cobranca_pago"]) == 1 &&
                                 Convert.ToInt32(_subrow["cobranca_cancelada"]) == 0)
                        {
                            pagto = Convert.ToDateTime(_subrow["cobranca_dataPagto"], cinfo);
                            pagto = UltimoDiaDoMes(pagto.Month, pagto.Year);
                            pagto = new DateTime(pagto.Year, pagto.Month, pagto.Day, 23, 59, 59, 998);

                            //atrasado
                            if ((vencto.Month < competencia.Month && vencto.Year == competencia.Year) ||
                                (vencto < competencia))
                            {
                                atrasado += Convert.ToDecimal(_subrow["cobranca_valor"], cinfo);
                            }
                            //antecipado
                            else if ((vencto.Month > competencia.Month && vencto.Year == competencia.Year) ||
                                     (vencto > competencia))
                            {
                                antecipado += Convert.ToDecimal(_subrow["cobranca_valor"], cinfo);
                            }
                        }
                    }

                    row["em_aberto"] = valorEmAberto.ToString("N2");
                    row["total_cancelado"] = valorCancelado.ToString("N2");
                    row["atrasado"] = atrasado.ToString("N2");
                    row["antecipado"] = antecipado.ToString("N2");
                    row["total_boletado"] = boletado.ToString("N2");
                    row["venda_nova"] = vendaNova.ToString("N2");
                    row["fatura"] = "0,00";
                    row["faturaCopart"] = "0,00";

                    dtFinal.Rows.Add(row);
                }

                grid1.DataSource = dtFinal;
                grid1.DataBind();
            }
        }
    }
}
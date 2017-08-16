using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using LC.Framework.Phantom;
using LC.Web.PadraoSeguros.Entity;
using System.Data;
using System.Data.SqlClient;

namespace www.financeiro
{
    public partial class relatorioFinanceiro : PageBase
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaFiliais();
                this.CarregaOperadoras();
                this.CarregaEstipulantes();

                base.PreencheComboNumerico(cboHora, 0, 23, true);
                base.PreencheComboNumerico(cboMinuto, 0, 59, true);

                this.ConfiguraCabecalho();
            }
        }

        void ConfiguraCabecalho()
        {
            DateTime procEm = DateTime.MinValue, venctoDe = DateTime.MinValue, venctoAte = DateTime.MinValue;

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter("SELECT TOP 1 * FROM reportbase", conn);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                conn.Close();
                adp.Dispose();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    procEm = Convert.ToDateTime(ds.Tables[0].Rows[0]["PROCESSADO_EM"]);
                    venctoDe = Convert.ToDateTime(ds.Tables[0].Rows[0]["VENCIMENTO_DE"]);
                    venctoAte = Convert.ToDateTime(ds.Tables[0].Rows[0]["VENCIMENTO_ATE"]);

                    lnkExibir.Text += " processado em " + procEm.ToString("dd/MM/yyyy HH:mm") + " para o período de " + venctoDe.ToString("dd/MM/yyyy") + " e " + venctoAte.ToString("dd/MM/yyyy");
                }
            }
        }

        void CarregaFiliais()
        {
            IList<Filial> filiais = Filial.CarregarTodas(false);

            this.lstFilial.DataValueField = "ID";
            this.lstFilial.DataTextField = "Nome";
            this.lstFilial.DataSource = filiais;
            this.lstFilial.DataBind();
        }

        void CarregaEstipulantes()
        {
            IList<Estipulante> estipulantes = Estipulante.Carregar(false);

            this.lstEstipulantes.DataValueField = "ID";
            this.lstEstipulantes.DataTextField = "Descricao";
            this.lstEstipulantes.DataSource = estipulantes;
            this.lstEstipulantes.DataBind();
        }

        void CarregaOperadoras()
        {
            IList<Operadora> operadoras = Operadora.CarregarTodas(false);

            this.lstOperadora.DataValueField = "ID";
            this.lstOperadora.DataTextField = "Nome";
            this.lstOperadora.DataSource = operadoras;
            this.lstOperadora.DataBind();
        }

        void OcultaPanels()
        {
            pnlRecebidos.Visible = false;
            pnlRecebidosGRID.Visible = false;
            pnlRecebiveis.Visible = false;
            pnlRecebiveisGRID.Visible = false;
        }

        ////////////////////////////////////////////////////////////////

        void ValoresRECEBIDO(DataSet ds, out Decimal total_boletado, out Decimal total_reativacao, out Decimal antecipacoes, out Decimal atrasos, out Decimal juroMultaMesVencto, out Decimal juroMultaAtraso, out Decimal taxaMesVectoBoletado, 
            out Decimal taxaMesVectoReativacao, out Decimal taxaAntecipacao, out Decimal taxaAtraso, out Decimal over_boletado, out Decimal over_antecipacao, out Decimal over_atraso, out DateTime processadoEm, out DateTime venctoDe, out DateTime venctoAte)
        {
            processadoEm = DateTime.MinValue; venctoDe = DateTime.MinValue; venctoAte = DateTime.MinValue;

            if (ds.Tables[0].Rows.Count > 0)
            {
                venctoDe = Convert.ToDateTime(ds.Tables[0].Rows[0]["VENCIMENTO_DE"]);
                venctoAte = Convert.ToDateTime(ds.Tables[0].Rows[0]["VENCIMENTO_ATE"]);
                processadoEm = Convert.ToDateTime(ds.Tables[0].Rows[0]["PROCESSADO_EM"]);
            }

            total_boletado = CToDecimal(ds.Tables[0].Compute("SUM(VALOR_LIMPO)", "cobranca_arquivoUltimoEnvioId IS NOT NULL AND ANTECIPACAO=0 AND ATRASO=0"));
            //foreach (DataRow row in rows) { total_boletado += CToDecimal(row["VALOR_LIMPO"]); }

            //rows = ds.Tables[0].Select("cobranca_arquivoUltimoEnvioId IS NULL");
            total_reativacao = CToDecimal(ds.Tables[0].Compute("SUM(VALOR_LIMPO)", "cobranca_arquivoUltimoEnvioId IS NULL AND ANTECIPACAO=0 AND ATRASO=0"));
            //foreach (DataRow row in rows) { total_reativacao += CToDecimal(row["VALOR_LIMPO"]); }

            antecipacoes = CToDecimal(ds.Tables[0].Compute("SUM(ANTECIPACAO)", "ANTECIPACAO > 0"));

            atrasos = CToDecimal(ds.Tables[0].Compute("SUM(ATRASO)", "ATRASO > 0"));
/////////////////////////////// MULTA
            juroMultaMesVencto = CToDecimal(ds.Tables[0].Compute("SUM(JURO_MULTA_REF_MES_VECTO)", "JURO_MULTA_REF_MES_VECTO > 0"));

            juroMultaAtraso = CToDecimal(ds.Tables[0].Compute("SUM(JURO_MULTA_PAGTO_ATRASO)", "JURO_MULTA_PAGTO_ATRASO > 0"));
/////////////////////////////// TX ASSOC BANCO
            taxaMesVectoBoletado = CToDecimal(ds.Tables[0].Compute("SUM(TAXA_ASSOC)", "ANTECIPACAO = 0 AND ATRASO=0 AND cobranca_arquivoUltimoEnvioId IS NOT NULL"));
            taxaMesVectoBoletado += CToDecimal(ds.Tables[0].Compute("SUM(TAXA_BANCO)", "ANTECIPACAO = 0 AND ATRASO=0 AND cobranca_arquivoUltimoEnvioId IS NOT NULL"));

            taxaMesVectoReativacao = CToDecimal(ds.Tables[0].Compute("SUM(TAXA_ASSOC)", "ANTECIPACAO = 0 AND ATRASO=0 AND cobranca_arquivoUltimoEnvioId IS NULL"));
            taxaMesVectoReativacao += CToDecimal(ds.Tables[0].Compute("SUM(TAXA_BANCO)", "ANTECIPACAO = 0 AND ATRASO=0 AND cobranca_arquivoUltimoEnvioId IS NULL"));

            taxaAntecipacao = CToDecimal(ds.Tables[0].Compute("SUM(TAXA_ASSOC)", "ANTECIPACAO > 0"));
            taxaAntecipacao += CToDecimal(ds.Tables[0].Compute("SUM(TAXA_BANCO)", "ANTECIPACAO > 0"));

            taxaAtraso = CToDecimal(ds.Tables[0].Compute("SUM(TAXA_ASSOC)", "ATRASO > 0"));
            taxaAtraso += CToDecimal(ds.Tables[0].Compute("SUM(TAXA_BANCO)", "ATRASO > 0"));
///////////////////////////////OVER
            over_boletado    = CToDecimal(ds.Tables[0].Compute("SUM(OVER_VALOR)", "ANTECIPACAO=0 AND ATRASO=0"));
            over_antecipacao = CToDecimal(ds.Tables[0].Compute("SUM(OVER_VALOR)", "ANTECIPACAO > 0"));
            over_atraso      = CToDecimal(ds.Tables[0].Compute("SUM(OVER_VALOR)", "ATRASO > 0"));
        }

        void MontaRelatorioRecebidoPDF()
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM reportbase WHERE cobranca_pago=1", conn);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                conn.Close();
                adp.Dispose();

                StringBuilder sb = new StringBuilder();
                Decimal total_boletado = 0, total_reativacao = 0, antecipacoes = 0, atrasos = 0, juroMultaMesVencto = 0, juroMultaAtraso = 0, taxaMesVectoBoletado = 0, taxaMesVectoReativacao = 0, taxaAntecipacao = 0, taxaAtraso = 0, over_boletado = 0, over_antecipacao = 0, over_atraso = 0;
                DateTime processadoEm, venctoDe, venctoAte;

                ValoresRECEBIDO(ds, out total_boletado, out total_reativacao, out antecipacoes, out atrasos,
                    out juroMultaMesVencto, out juroMultaAtraso, out taxaMesVectoBoletado, out taxaMesVectoReativacao,
                    out taxaAntecipacao, out taxaAtraso, out over_boletado, out over_antecipacao, out over_atraso, out processadoEm, out venctoDe, out venctoAte);

                sb.Append("<br><br>");

                sb.Append("<table cellspacing='0' width='400' style=\"border: solid 1px #507CD1\">");
                sb.Append("<tr>");
                sb.Append("<td class='tdPrincipal1' style='border-bottom: solid 1px #507CD1' align='center'>RELATÓRIO FINANCEIRO - RECEBIDOS</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td style='border-top: solid 1px #507CD1' class='tdNormal1'>Processado em: "); sb.Append(processadoEm.ToString("dd/MM/yyyy hh:mm:ss")); sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td class='tdNormal1'>Recebidos entre "); sb.Append(venctoDe.ToString("dd/MM/yyyy")); sb.Append(" e "); sb.Append(venctoAte.ToString("dd/MM/yyyy")); sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                #region NET 

                sb.Append("<table cellspacing='0' width='400' style=\"border: solid 1px #507CD1\">");
                sb.Append("<tr>");
                sb.Append("<td class='tdPrincipal1' style='border-bottom: solid 1px #507CD1' colspan='2'>VALOR NET</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td width='250'>REF. MÊS VENCTO. EMITIDOS</td>");
                sb.Append("<td>");
                sb.Append(total_boletado.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td>REF. MÊS VENCTO. REATIVAÇÕES</td>");
                sb.Append("<td>");
                sb.Append(total_reativacao.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td><b>TOTAL PARCIAL</b></td>");
                sb.Append("<td><b>");
                sb.Append((total_boletado + total_reativacao).ToString("C"));
                sb.Append("</b></td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td>ANTECIPAÇÕES</td>");
                sb.Append("<td>");
                sb.Append(antecipacoes.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td>PAGTOS. EM ATRASO</td>");
                sb.Append("<td>");
                sb.Append(atrasos.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td style='border-top: solid 1px #507CD1' class='tdNormal1'><b>TOTAL NET</b></td>");
                sb.Append("<td style='border-top: solid 1px #507CD1' class='tdNormal1'>");
                sb.Append((atrasos + antecipacoes + total_reativacao + total_boletado).ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                #endregion

                //sb.Append("<tr><td height='8' colspan='2'>&nbsp;</td></tr>");
                sb.Append("<br>");

                #region MULTA E JUROS 

                sb.Append("<table cellspacing='0' width='400' style=\"border: solid 1px #507CD1\">");
                sb.Append("<tr>");
                sb.Append("<td class='tdPrincipal1' style='border-bottom: solid 1px #507CD1' colspan='2'>JUROS E MULTA</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td width='250'>REF. MÊS VENCTO</td>");
                sb.Append("<td>");
                sb.Append(juroMultaMesVencto.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td>PAGTOS. EM ATRASO</td>");
                sb.Append("<td>");
                sb.Append(juroMultaAtraso.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td style='border-top: solid 1px #507CD1' class='tdNormal1'><b>TOTAL JUROS E MULTA</b></td>");
                sb.Append("<td style='border-top: solid 1px #507CD1' class='tdNormal1'>");
                sb.Append((juroMultaMesVencto + juroMultaAtraso).ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                #endregion

                sb.Append("<br>");

                #region TAXA ASSOC. / BANCO 

                sb.Append("<table cellspacing='0' width='400' style=\"border: solid 1px #507CD1\">");
                sb.Append("<tr>");
                sb.Append("<td class='tdPrincipal1' style='border-bottom: solid 1px #507CD1' colspan='2'>TAXA SIND. / BANCOS</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td width='250'>REF. MÊS VENCTO. EMITIDOS</td>");
                sb.Append("<td>");
                sb.Append(taxaMesVectoBoletado.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td>REF. MÊS VENCTO. REATIVAÇÕES</td>");
                sb.Append("<td>");
                sb.Append(taxaMesVectoReativacao.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td>ANTECIPAÇÕES</td>");
                sb.Append("<td>");
                sb.Append(taxaAntecipacao.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td>PAGTOS. EM ATRASO</td>");
                sb.Append("<td>");
                sb.Append(taxaAtraso.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td style='border-top: solid 1px #507CD1' class='tdNormal1'><b>TOTAL TAXAS</b></td>");
                sb.Append("<td style='border-top: solid 1px #507CD1' class='tdNormal1'>");
                sb.Append((taxaAtraso + taxaAntecipacao + taxaMesVectoReativacao + taxaMesVectoBoletado).ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                #endregion

                sb.Append("<br>");

                #region OVER PRICE 

                sb.Append("<table cellspacing='0' width='400' style=\"border: solid 1px #507CD1\">");
                sb.Append("<tr>");
                sb.Append("<td class='tdPrincipal1' style='border-bottom: solid 1px #507CD1' colspan='2'>OVER PRICE</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td width='250'>SOBRE MÊS VENCTO.</td>");
                sb.Append("<td>");
                sb.Append(over_boletado.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td>SOBRE ANTECIPAÇÕES</td>");
                sb.Append("<td>");
                sb.Append(over_antecipacao.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td>SOBRE PAGTOS. EM ATRASOS</td>");
                sb.Append("<td>");
                sb.Append(over_atraso.ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td style='border-top: solid 1px #507CD1' class='tdNormal1'><b>TOTAL OVER</b></td>");
                sb.Append("<td style='border-top: solid 1px #507CD1' class='tdNormal1'>");
                sb.Append((over_boletado + over_antecipacao + over_atraso).ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                #endregion

                sb.Append("<br>");

                sb.Append("<table cellspacing='0' width='400' style=\"border: solid 1px #507CD1\">");
                sb.Append("<tr>");
                sb.Append("<td width='250' class='tdPrincipal1' style='border-bottom: solid 1px #507CD1'>TOTAL</td>");
                sb.Append("<td class='tdPrincipal1' style='border-bottom: solid 1px #507CD1'>");
                sb.Append((atrasos + antecipacoes + total_reativacao + total_boletado + juroMultaMesVencto + juroMultaAtraso + taxaAtraso + taxaAntecipacao + taxaMesVectoReativacao + taxaMesVectoBoletado + over_boletado + over_antecipacao + over_atraso).ToString("C"));
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                litCorpo.Text = sb.ToString();
            }
        }

        void MontaRelatorioRecebidoGRID()
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM reportbase WHERE cobranca_pago=1", conn);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                conn.Close();
                adp.Dispose();

                Decimal total_boletado = 0, total_reativacao = 0, antecipacoes = 0, atrasos = 0, juroMultaMesVencto = 0, juroMultaAtraso = 0, taxaMesVectoBoletado = 0, taxaMesVectoReativacao = 0, taxaAntecipacao = 0, taxaAtraso = 0, over_mes = 0, over_antecipacoes = 0, over_atrasos = 0;
                DateTime processadoEm, venctoDe, venctoAte;

                ValoresRECEBIDO(ds, out total_boletado, out total_reativacao, out antecipacoes, out atrasos,
                    out juroMultaMesVencto, out juroMultaAtraso, out taxaMesVectoBoletado, out taxaMesVectoReativacao,
                    out taxaAntecipacao, out taxaAtraso, out over_mes, out over_antecipacoes, out over_atrasos, out processadoEm, out venctoDe, out venctoAte);

                DataTable dt = new DataTable();
                dt.Columns.Add("REF_MES_VENCTO_BOLETADOS");
                dt.Columns.Add("REF_MES_VENCTO_REATIVACOES");
                dt.Columns.Add("TOTAL_PARCIAL");
                dt.Columns.Add("ANTECIPACOES");
                dt.Columns.Add("PAGTOS_EM_ATRASO");
                dt.Columns.Add("TOTAL_NET");
                dt.Columns.Add("MULTA_REF_MES_VENCTO");
                dt.Columns.Add("MULTA_PAGTOS_EM_ATRASO");
                dt.Columns.Add("TOTAL_JUROS_MULTA");
                dt.Columns.Add("TAXAS_REF_MES_VENCTO_BOLETADOS");
                dt.Columns.Add("TAXAS_REF_MES_VENCTO_REATIVACOES");
                dt.Columns.Add("TAXAS_ATENCIPACOES");
                dt.Columns.Add("TAXAS_PAGTOS_EM_ATRASO");
                dt.Columns.Add("TOTAL_TAXAS");
                dt.Columns.Add("OVER_REF_MES_VENCTO");
                dt.Columns.Add("OVER_ANTECIPACOES");
                dt.Columns.Add("OVER_ATRASOS");

                DataRow row = dt.NewRow();

                row["REF_MES_VENCTO_BOLETADOS"] = total_boletado.ToString("N2");
                row["REF_MES_VENCTO_REATIVACOES"] = total_reativacao.ToString("N2");
                row["TOTAL_PARCIAL"] = (total_boletado + total_reativacao).ToString("N2");
                row["ANTECIPACOES"] = antecipacoes.ToString("N2");
                row["PAGTOS_EM_ATRASO"] = atrasos.ToString("N2");
                row["TOTAL_NET"] = (atrasos + antecipacoes + total_reativacao + total_boletado).ToString("N2");
                row["MULTA_REF_MES_VENCTO"] = juroMultaMesVencto.ToString("N2");
                row["MULTA_PAGTOS_EM_ATRASO"] = juroMultaAtraso.ToString("N2");
                row["TOTAL_JUROS_MULTA"] = (juroMultaMesVencto + juroMultaAtraso).ToString("N2");
                row["TAXAS_REF_MES_VENCTO_BOLETADOS"] = taxaMesVectoBoletado.ToString("N2");
                row["TAXAS_REF_MES_VENCTO_REATIVACOES"] = taxaMesVectoReativacao.ToString("N2");
                row["TAXAS_ATENCIPACOES"] = taxaAntecipacao.ToString("N2");
                row["TAXAS_PAGTOS_EM_ATRASO"] = atrasos.ToString("N2");
                row["TOTAL_TAXAS"] = (taxaAtraso + taxaAntecipacao + taxaMesVectoReativacao + taxaMesVectoBoletado).ToString("N2");
                row["OVER_REF_MES_VENCTO"] = over_mes.ToString("N2");
                row["OVER_ANTECIPACOES"] = over_antecipacoes.ToString("N2");
                row["OVER_ATRASOS"] = over_atrasos.ToString("N2");

                dt.Rows.Add(row);

                grid.DataSource = dt;
                grid.DataBind();
            }
        }

        ////////////////

        void ValoresRECEBIVEIS(out Decimal total_net, out Decimal taxas, out Decimal over, out Decimal agenciamentos, out Decimal vitalicios, out DateTime processadoEm, out DateTime venctoDe, out DateTime venctoAte)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM reportbase", conn);
                adp.Fill(ds);
                conn.Close();
                adp.Dispose();
            }

            processadoEm = DateTime.MinValue; venctoDe = DateTime.MinValue; venctoAte = DateTime.MinValue;

            if (ds.Tables[0].Rows.Count > 0)
            {
                venctoDe = Convert.ToDateTime(ds.Tables[0].Rows[0]["VENCIMENTO_DE"]);
                venctoAte = Convert.ToDateTime(ds.Tables[0].Rows[0]["VENCIMENTO_ATE"]);
                processadoEm = Convert.ToDateTime(ds.Tables[0].Rows[0]["PROCESSADO_EM"]);
            }

            total_net = CToDecimal(ds.Tables[0].Compute("SUM(VALOR_LIMPO)", ""));

            /////////////////////////////// TX ASSOC BANCO
            taxas = CToDecimal(ds.Tables[0].Compute("SUM(TAXA_ASSOC)", ""));
            taxas += CToDecimal(ds.Tables[0].Compute("SUM(TAXA_BANCO)", ""));

            ///////////////////////////////OVER
            over = CToDecimal(ds.Tables[0].Compute("SUM(OVER_VALOR)", "OVER_VALOR <> 0"));

            ///////////////////////////////COMISSOES OPERADORA
            vitalicios = CToDecimal(ds.Tables[0].Compute("SUM(VITALICIO)", "VITALICIO <> 0"));
            agenciamentos = CToDecimal(ds.Tables[0].Compute("SUM(AGENCIAMENTO)", "AGENCIAMENTO <> 0"));
        }

        void MontaRelatorioRecebiveisPDF()
        {
            StringBuilder sb = new StringBuilder();
            Decimal total_net = 0, taxas = 0, over = 0, agenciamentos = 0, vitalicios = 0;
            DateTime processadoEm, venctoDe, venctoAte;

            this.ValoresRECEBIVEIS(out total_net, out taxas, out over, out agenciamentos, out vitalicios, out processadoEm, out venctoDe, out venctoAte);

            sb.Append("<br><br>");

            sb.Append("<table cellspacing='0' width='400' style=\"border: solid 1px #507CD1\">");
            sb.Append("<tr>");
            sb.Append("<td class='tdPrincipal1' style='border-bottom: solid 1px #507CD1' align='center'>RELATÓRIO FINANCEIRO - RECEBÍVEIS</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='border-top: solid 1px #507CD1' class='tdNormal1'>Processado em: "); sb.Append(processadoEm.ToString("dd/MM/yyyy hh:mm:ss")); sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class='tdNormal1'>Vencimento de "); sb.Append(venctoDe.ToString("dd/MM/yyyy")); sb.Append(" até "); sb.Append(venctoAte.ToString("dd/MM/yyyy")); sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            #region corpo  

            sb.Append("<table cellspacing='0' width='400' style=\"border: solid 1px #507CD1\">");
            sb.Append("<tr>");
            sb.Append("<td width='250'>VALOR NET</td>");
            sb.Append("<td>");
            sb.Append(total_net.ToString("C"));
            sb.Append("</td>");
            sb.Append("</tr>");

            sb.Append("<tr>");
            sb.Append("<td>TAXAS SIND. / BCO</td>");
            sb.Append("<td>");
            sb.Append(taxas.ToString("C"));
            sb.Append("</td>");
            sb.Append("</tr>");

            sb.Append("<tr>");
            sb.Append("<td>OVER PRICE</td>");
            sb.Append("<td>");
            sb.Append(over.ToString("C"));
            sb.Append("</td>");
            sb.Append("</tr>");

            sb.Append("<tr>");
            sb.Append("<td><b>SUB TOTAL</b></td>");
            sb.Append("<td>");
            sb.Append((total_net + taxas + over).ToString("C"));
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            sb.Append("<br>");

            sb.Append("<table cellspacing='0' width='400' style=\"border: solid 1px #507CD1\">");
            sb.Append("<tr>");
            sb.Append("<td class='tdPrincipal1' style='border-bottom: solid 1px #507CD1' colspan='2'>COMISSÃO</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td width='250'>AGENCIAMENTO</td>");
            sb.Append("<td>");
            sb.Append(agenciamentos.ToString("C"));
            sb.Append("</td>");
            sb.Append("</tr>");

            sb.Append("<tr>");
            sb.Append("<td>VITALÍCIO</td>");
            sb.Append("<td>");
            sb.Append(vitalicios.ToString("C"));
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            sb.Append("<br>");

            sb.Append("<table cellspacing='0' width='400' style=\"border: solid 1px #507CD1\">");
            sb.Append("<tr>");
            sb.Append("<td width='250' class='tdPrincipal1' style='border-bottom: solid 1px #507CD1'>TOTAL</td>");
            sb.Append("<td class='tdPrincipal1' style='border-bottom: solid 1px #507CD1'>");
            sb.Append((total_net + taxas + over + vitalicios + agenciamentos).ToString("C"));
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            #endregion
            
            litCorpoRecebiveis.Text = sb.ToString();
        }

        void MontaRelatorioRecebiveisGRID()
        {
            Decimal total_net = 0, taxas = 0, over = 0, agenciamentos = 0, vitalicios = 0;
            DateTime processadoEm, venctoDe, venctoAte;

            this.ValoresRECEBIVEIS(out total_net, out taxas, out over, out agenciamentos, out vitalicios, out processadoEm, out venctoDe, out venctoAte);

            DataTable dt = new DataTable();
            dt.Columns.Add("NET");
            dt.Columns.Add("TAXAS");
            dt.Columns.Add("OVER");
            dt.Columns.Add("SUBTOTAL");
            dt.Columns.Add("AGENCIAMENTO");
            dt.Columns.Add("VITALICIO");
            dt.Columns.Add("TOTAL");

            DataRow row = dt.NewRow();

            row["NET"] = total_net.ToString("N2");
            row["TAXAS"] = taxas.ToString("N2");
            row["OVER"] = over.ToString("N2");
            row["SUBTOTAL"] = (total_net + taxas + over).ToString("N2");
            row["AGENCIAMENTO"] = agenciamentos.ToString("N2");
            row["VITALICIO"] = vitalicios.ToString("N2");
            row["TOTAL"] = (total_net + taxas + over + vitalicios + agenciamentos).ToString("N2");

            dt.Rows.Add(row);

            gridRecebiveis.DataSource = dt;
            gridRecebiveis.DataBind();
        }

        ////////////////////////////////////////////////////////////////

        protected void cmdToExcel_Click(Object sender, EventArgs e)
        {
            String attachment = "attachment; filename=file.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }
        protected void cmdToExcelRecebiveis_Click(Object sender, EventArgs e)
        {
            String attachment = "attachment; filename=file.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gridRecebiveis.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        protected void cmdRecebidosPdf_Click(Object sender, EventArgs e)
        {
            this.OcultaPanels();
            this.MontaRelatorioRecebidoPDF();
            this.MontaRelatorioRecebiveisPDF();
            pnlRecebidos.Visible = true;
        }

        protected void cmdRecebidosExcel_Click(Object sender, EventArgs e)
        {
            this.OcultaPanels();
            this.MontaRelatorioRecebidoGRID();
            pnlRecebidosGRID.Visible = true;
            grid.Visible = true;
        }

        protected void cmdRecebiveisPdf_Click(Object sender, EventArgs e)
        {
            this.OcultaPanels();
            this.MontaRelatorioRecebiveisPDF();
            pnlRecebiveis.Visible = true;
        }

        protected void cmdRecebiveisExcel_Click(Object sender, EventArgs e)
        {
            this.OcultaPanels();
            this.MontaRelatorioRecebiveisGRID();
            pnlRecebiveisGRID.Visible = true;
        }

        protected void lnkAgendar_Click(Object sender, EventArgs e)
        {
            this.OcultaPanels();
            if (pnlAgendar.Visible)
            {
                pnlAgendar.Visible = false;
                pnlExibir.Visible = true;
            }
            else
            {
                pnlAgendar.Visible = true;
                pnlExibir.Visible = false;
            }
        }

        protected void lnkExibir_Click(Object sender, EventArgs e)
        {
            this.OcultaPanels();
            if (pnlExibir.Visible)
            {
                pnlAgendar.Visible = true;
                pnlExibir.Visible = false;
            }
            else
            {
                pnlAgendar.Visible = false;
                pnlExibir.Visible = true;
                cmdRecebidosPdf_Click(null, null);
            }
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            DateTime proc = base.CStringToDateTime(txtProcessarEm.Text, cboHora.SelectedValue + ":" + cboMinuto.SelectedValue);
            DateTime de = base.CStringToDateTime(txtDe.Text);
            DateTime ate = base.CStringToDateTime(txtAte.Text, "23:59", 59);

            String[] estIDS = PegaIDsSelecionados(lstEstipulantes);
            if (estIDS == null)
            {
                base.Alerta(null, this, "__errEst", "Selecione ao menos um estipulante.");
                return;
            }

            String[] operIDs = PegaIDsSelecionados(lstOperadora);
            if (operIDs == null)
            {
                base.Alerta(null, this, "__errOpe", "Selecione ao menos uma operadora.");
                return;
            }

            String[] filIDS = PegaIDsSelecionados(lstFilial);
            if (filIDS == null)
            {
                base.Alerta(null, this, "__errFil", "Selecione ao menos uma filial.");
                return;
            }

            #endregion

            FinancialReportBase frp = new FinancialReportBase();
            frp.EstipulanteIdArr = String.Join(",", estIDS);
            frp.FilialIdArr = String.Join(",", filIDS);
            frp.OperadoraIdArr = String.Join(",", operIDs);
            frp.Processado = false;
            frp.ProcessarEm = proc;
            frp.VencimentoAte = ate;
            frp.VencimentoDe = de;

            frp.Salvar(true);

            base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
        }
    }
}
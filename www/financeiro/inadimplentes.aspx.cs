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

namespace www.financeiro
{
    public partial class inadimplentes : PageBase
    {
        DataTable dataCache
        {
            get { return Session["dataCache"] as DataTable; }
            set 
            {
                if (value == null) { Session.Remove("dataCache"); }
                else { Session["dataCache"] = value; }
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dataCache = null;

                this.CarregaFiliais();
                this.CarregaEstipulantes();
                this.CarregaOperadoras();
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

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            DateTime dtFrom = new DateTime();
            DateTime dtTo = new DateTime();
            UIHelper.TyParseToDateTime(txtDe.Text, out dtFrom);
            UIHelper.TyParseToDateTime(txtAte.Text, out dtTo);

            String[] oper = base.PegaIDsSelecionados(lstOperadora);
            String[] fili = base.PegaIDsSelecionados(lstFilial);
            String[] estp = base.PegaIDsSelecionados(lstEstipulantes);

            if (estp == null || oper == null || dtFrom == DateTime.MinValue || dtTo == DateTime.MinValue) { return; }

            String filialCond = "";
            if (fili != null)
            {
                filialCond = " and usuario_filialId IN (" + String.Join(",", fili) + ") ";
            }

            String cobrancaCond = "";
            if (cboTipoCobrancas.SelectedIndex > 0)
            {
                cobrancaCond = " AND cobranca_tipo=" + cboTipoCobrancas.SelectedValue;
            }

            String qry = String.Concat(
                "select contrato_id, contrato_codcobranca, estipulante_descricao, contratoadm_descricao, contratoadm_contratoSaude,contratoadm_contratoDental, operadora_nome, contrato_numero, beneficiario_nome, contratobeneficiario_numMatriculaSaude,contratobeneficiario_numMatriculaDental, cobranca_valor, cobranca_parcela, cobranca_dataVencimento, '' as 'cobranca_dataVencimentoAdiantado', '' as 'cobranca_dataPagtoAdiantado', ",
                "       (select count(*) from contrato_beneficiario where contratobeneficiario_ativo=1 and contratobeneficiario_contratoId=contrato_id) as Vidas ",
                "	from beneficiario ",
                "		inner join contrato_beneficiario on contratobeneficiario_beneficiarioId=beneficiario_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                "		inner join contrato on contrato_id=contratobeneficiario_contratoId ",
                "		inner join operadora on operadora_id=contrato_operadoraId ",
                "		inner join cobranca on cobranca_propostaId=contrato_id ",
                "       inner join usuario on usuario_id = contrato_donoId ",
                "       inner join estipulante on estipulante_id = contrato_estipulanteId ",
                "       inner join contratoAdm on contratoadm_id = contrato_contratoAdmId ",
                "	where ", 
                "		cobranca_pago=0 and cobranca_cancelada <> 1 and contrato_cancelado <> 1 and contrato_inativo <> 1 ",
                "		and cobranca_dataVencimento between '", dtFrom.ToString("yyyy-MM-dd 00:00:00.000"), "' and '", dtTo.ToString("yyyy-MM-dd 23:59:59.990"), "' ",
                "       and contrato_operadoraId IN (", String.Join(",", oper), ")",
                "       and contrato_estipulanteId IN (", String.Join(",", estp), ")", filialCond, cobrancaCond,
                "	order by operadora_nome, beneficiario_nome, cobranca_parcela ");

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                SqlDataAdapter adp = new SqlDataAdapter(qry, conn);
                DataSet ds = new DataSet();
                adp.Fill(ds);

                // parcelas pagas em adiantamento ////////////////////////////////////////////////////////
                List<String> contratoIds = new List<String>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (!contratoIds.Contains(Convert.ToString(row["contrato_id"])))
                        contratoIds.Add(Convert.ToString(row["contrato_id"]));
                }

                qry = String.Concat(
                    "select contrato_id, estipulante_descricao, contratoadm_descricao, operadora_nome, contrato_numero, beneficiario_nome, cobranca_valor, cobranca_parcela, cobranca_dataVencimento,cobranca_dataPagto,cobranca_pago ",
                    "	from beneficiario ",
                    "		inner join contrato_beneficiario on contratobeneficiario_beneficiarioId=beneficiario_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                    "		inner join contrato on contrato_id=contratobeneficiario_contratoId ",
                    "		inner join operadora on operadora_id=contrato_operadoraId ",
                    "		inner join cobranca on cobranca_propostaId=contrato_id ",
                    "       inner join usuario on usuario_id = contrato_donoId ",
                    "       inner join estipulante on estipulante_id = contrato_estipulanteId ",
                    "       inner join contratoAdm on contratoadm_id = contrato_contratoAdmId ",
                    "	where cobranca_propostaId in (", String.Join(",", contratoIds.ToArray()), ") ", //and cobranca_pago=0
                    "	order by operadora_nome, beneficiario_nome, cobranca_parcela ");

                if (contratoIds.Count == 0)
                {
                    pnl.Visible = false;
                    grid.DataSource = null;
                    grid.DataBind();

                    return;
                }

                DataSet ds2 = new DataSet();
                adp.SelectCommand.CommandText = qry;
                adp.Fill(ds2);

                DataRow[] rows = null, arrTemp = null; DataRow temp = null;
                foreach (String contratoId in contratoIds)
                {
                    rows = ds2.Tables[0].Select("contrato_id=" + contratoId);
                    arrTemp = ds.Tables[0].Select("contrato_id=" + contratoId);
                    temp = arrTemp[arrTemp.Length - 1];

                    foreach (DataRow row in rows)
                    {
                        if (Convert.ToInt32(row["cobranca_parcela"]) <= Convert.ToInt32(temp["cobranca_parcela"])) { continue; }

                        if (Convert.ToBoolean(row["cobranca_pago"]))
                        {
                            temp["cobranca_dataPagtoAdiantado"] = Convert.ToDateTime(row["cobranca_dataPagto"]).ToString("dd/MM/yyyy");
                            temp["cobranca_dataVencimentoAdiantado"] = Convert.ToDateTime(row["cobranca_dataVencimento"]).ToString("dd/MM/yyyy");

                            //adiciona à coleção final (ds.Tables[0])
                            //DataRow nova = ds.Tables[0].NewRow();
                            //nova["operadora_nome"] = row["operadora_nome"];
                            //nova["contrato_numero"] = row["contrato_numero"];
                            //nova["beneficiario_nome"] = row["beneficiario_nome"];
                            //nova["cobranca_valor"] = row["cobranca_valor"];
                            //nova["cobranca_parcela"] = row["cobranca_parcela"];
                            //nova["cobranca_dataVencimento"] = row["cobranca_dataVencimento"];
                            //nova["estipulante_descricao"] = row["estipulante_descricao"];
                            //nova["contratoadm_descricao"] = row["contratoadm_descricao"];
                            //nova["cobranca_dataPagto"] = row["cobranca_dataPagto"];

                            //ds.Tables[0].Rows.Add(nova);
                            break;
                        }
                    }
                }
                //////////////////////////////////////////////////////////////////////////////////////////

                grid.DataSource = ds.Tables[0]; 
                grid.DataBind();

                dataCache = ds.Tables[0];

                if (grid.DataSource == null)
                    pnl.Visible = false;
                else
                    pnl.Visible = true;

                adp.Dispose();
                ds.Dispose();
                conn.Close();
            }
        }

        protected void cmdToExcel_Click(Object sender, ImageClickEventArgs e)
        {
            if (dataCache != null)
            {
                foreach (DataRow row in dataCache.Rows)
                {
                    if (Convert.ToString(row["contrato_numero"]).StartsWith("0"))
                    {
                        row["contrato_numero"] = String.Concat( "'", Convert.ToString(row["contrato_numero"]));
                    }
                }

                grid.DataSource = dataCache;
                grid.DataBind();
            }

            String attachment = "attachment; filename=file.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();

            //GeraExcel(dataCache, "C:\\");
        }

        public static void GeraExcel(DataSet dataSet, string Output_Path)
        {
            //Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            //if (excel != null)
            //{
            //    // Mantem a instancia do Excel invisivel
            //    excel.Visible = false;
            //    // Adiciona um WorkBook ao Excel
            //    Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(Type.Missing);

            //    // Verifica se o DataSet informado não esta nulo
            //    if (dataSet != null)
            //    {
            //        // Realiza o loop na coleção de DataTable
            //        for (int i = 0; i < dataSet.Tables.Count; i++)
            //        {
            //            // Pega o DataTable corrente
            //            System.Data.DataTable dataTable = dataSet.Tables[i];
            //            // Se o total de Worksheet é igual a posição atual do loop, cria um novo WorkSheet após o último
            //            if (workbook.Worksheets.Count == i) workbook.Worksheets.Add(Type.Missing, workbook.Sheets[i], Type.Missing, Type.Missing);
            //            // Recupera o WorkSheet
            //            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(i + 1);

            //            // Se o WorkSheet não for nulo
            //            if (worksheet != null)
            //            {
            //                // Renomeia o WorkSheet para o mesmo nome do DataTable
            //                worksheet.Name = dataTable.TableName;

            //                // Realiza loop nas colunas do DataTable para pegar o nome das mesmas
            //                for (int j = 0; j < dataTable.Columns.Count; j++)
            //                {
            //                    // Pega o Index da coluna. No DataTable o index inicia em 0 e no Excel em 1
            //                    int ColumnIndex = j + 1;

            //                    // Retorna a celula do Excel
            //                    Microsoft.Office.Interop.Excel.Range range = (worksheet.Cells[1, ColumnIndex] as Microsoft.Office.Interop.Excel.Range);
            //                    // Atribui valor a celula
            //                    range.Value2 = dataTable.Columns[j].ColumnName;
            //                }

            //                // Realiza loop nas linhas do DataTable
            //                for (int r = 1; r < dataTable.Rows.Count; r++)
            //                {
            //                    // Realiza loop nas colunas do DataTable
            //                    for (int c = 0; c < dataTable.Columns.Count; c++)
            //                    {
            //                        // Recupera o valor da da celula do DataTable
            //                        object value = dataTable.Rows[r][c];
            //                        // Pega o Index da linha. No DataTable o index inicia em 0 e no Excel em 1
            //                        int RowIndex = r + 1;
            //                        // Pega o Index da coluna. No DataTable o index inicia em 0 e no Excel em 1
            //                        int ColumnIndex = c + 1;
            //                        // Retorna a celula do Excel
            //                        Microsoft.Office.Interop.Excel.Range range = (worksheet.Cells[RowIndex, ColumnIndex] as Microsoft.Office.Interop.Excel.Range);
            //                        // Atribui valor a celula
            //                        range.Value2 = value;
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // Define o caminho onde deverá ser salvo o arquivo excel. Utilizando o nome do DataSet para gerar o nome do arquivo Excel
            //    string pathFile = System.IO.Path.Combine(Output_Path, string.Format("{0}.xls", dataSet.DataSetName));
            //    // Verifica se o diretorio destino não existe. Se não existir cria o diretorio
            //    if (!System.IO.Directory.Exists(Output_Path)) System.IO.Directory.CreateDirectory(Output_Path);
            //    // Verifica se existe algum arquivo com o mesmo nome e caminho de destino, se existir exclui
            //    if (System.IO.File.Exists(pathFile)) System.IO.File.Delete(pathFile);
            //    // Salva o arquivo excel no caminho especificado
            //    workbook.SaveAs(pathFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            //    // Fecha o Excel
            //    excel.Quit();
            //}
            //else
            //{
            //    throw new Exception("Microsoft Excel não pôde ser iniciado. Verifique se a instalação do Office e referências do projeto estão corretas.");
            //}
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LC.Web.PadraoSeguros.Entity;
using System.Text;
using LC.Framework.Phantom;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace www.financeiro
{
    public partial class boletos_em_pdf : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request["ID"])) return;

                this.gerarArquivoBoletoLote(Request["ID"]);
            }
        }

        void gerarArquivoBoletoLote(string arquivoRemessaId)
        {
            StringBuilder sb = new StringBuilder();

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                string host = "http://ubrasp.iphotel.info";
                string nossoNumero = "", uri = "", finalUrl = "";
                object beneficiarioId = null;

                IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>("* from cobranca where cobranca_cancelada <> 1 and cobranca_pago <> 1 and cobranca_arquivoUltimoEnvioId in (" + arquivoRemessaId + ")", typeof(Cobranca), pm);

                int i = 0;
                foreach (Cobranca cobranca in cobrancas)
                {
                    beneficiarioId = ContratoBeneficiario.CarregaTitularID(cobranca.PropostaID, pm);

                    uri = EntityBase.RetiraAcentos(
                        String.Concat(
                            "?nossonum=", nossoNumero,
                            "&contid=", cobranca.PropostaID,
                            "&valor=", cobranca.Valor.ToString("N2"),
                            "&v_dia=", cobranca.DataVencimento.Day, "&v_mes=", cobranca.DataVencimento.Month, "&v_ano=", cobranca.DataVencimento.Year,
                            "&bid=", beneficiarioId,
                            "&cobid=", cobranca.ID,
                            "&mailto="));

                    finalUrl = string.Concat("/boleto/santander2.aspx", uri);

                    System.Net.WebRequest request = System.Net.WebRequest.Create(string.Concat(host, finalUrl));
                    System.Net.WebResponse response = request.GetResponse();
                    System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("iso-8859-1"));
                    String html = sr.ReadToEnd().Replace("Versão demonstrativa do Gerador de Boletos (www.boletoasp.com.br)", "");
                    sr.Close();
                    response.Close();

                    sb.Append(html);
                    sb.Append("<div style=\"page-break-before: always;\"></div>");
                    i++;
                }

                string final = sb.ToString().Replace(@"D:\http\ubrasp\web\var\boleto_file\BoletoNetBarra.gif", @"http://ubrasp.iphotel.info/images/boleto/BoletoNetBarra.gif");
                pm.CloseSingleCommandInstance();

                Response.Write(final);

                //byte[] ret = GetPDF(final);
                //Response.Clear();
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-disposition", "filename=" + "boletos.pdf");
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.BinaryWrite(ret);
                //Response.End();
            }
        }

        public byte[] GetPDF(string pHTML)
        {
            //localhost:4123/financeiro/boletos_em_pdf.aspx?ID=49
            //http://www.rswebsols.com/tutorials/programming/asp-net-generate-pdf-html-itextsharp

            byte[] bPDF = null;

            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(pHTML);

            // 1: create object of a itextsharp document class
            Document doc = new Document(PageSize.A4, 25, 25, 25, 25);

            // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file
            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);

            // 3: we create a worker parse the document
            HTMLWorker htmlWorker = new HTMLWorker(doc);

            // 4: we open document and start the worker on the document
            doc.Open();
            htmlWorker.StartDocument();

            // 5: parse the html into the document
            htmlWorker.Parse(txtReader);

            // 6: close the document and the worker
            htmlWorker.EndDocument();
            htmlWorker.Close();
            doc.Close();

            bPDF = ms.ToArray();

            return bPDF;
        }
    }
}
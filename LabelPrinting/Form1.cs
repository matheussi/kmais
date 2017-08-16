/*
REFERENCIAS 
https://www.google.com.br/webhp?sourceid=chrome-instant&ion=1&espv=2&ie=UTF-8#q=c%23%20print%20preview - busca
https://msdn.microsoft.com/pt-br/library/ms404294(v=vs.110).aspx - preview
https://msdn.microsoft.com/pt-br/library/system.drawing.printing.printdocument.print(v=vs.110).aspx - printdocument
http://wiki.portugal-a-programar.pt/dev_net:vb.net:relatorios - exemplo etiqueta
http://www.endmemo.com/sconvert/millimeterpixel.php - conversor de milimetros e pixels
http://stackoverflow.com/questions/18814493/how-to-jump-to-the-next-page-in-a-printdocument - printdocument - proxima pagina
 * 
https://www.google.com.br/webhp?sourceid=chrome-instant&ion=1&espv=2&ie=UTF-8#q=c%23%20print%20document%20page%20size
https://msdn.microsoft.com/pt-br/library/system.drawing.printing.pagesettings.papersize(v=vs.110).aspx
http://stackoverflow.com/questions/21826507/change-printer-default-paper-size
*/

namespace LabelPrinting
{
    using System;
    using System.IO;
    using System.Data;
    using System.Text;
    using System.Drawing;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing.Printing;
    using System.Collections.Generic;
    using System.Xml;

    public partial class Form1 : Form
    {
        string stringToPrint;
        string documentContents;

        List<DocItem> Itens = new List<DocItem>();
        int printed = 0;

        public Form1()
        {
            InitializeComponent();

            printDoc.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);

            cboFormato.SelectedIndex = 0;
            ofd.Filter = "Arquivox XML (*.xml)|*.xml";

            for (int i = 0; i < printDoc.PrinterSettings.PaperSources.Count; i++)
            {
                var pkSource = printDoc.PrinterSettings.PaperSources[i];
                string temp = pkSource.ToString();
            }
        }

        private void GetData(string xml)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(xml);
            }
            catch
            {
                MessageBox.Show("Arquivo em formato inválido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            XmlNodeList nos = doc.DocumentElement.SelectNodes("/boletos/boleto");

            Itens.Clear();
            //int ind = 0;
            foreach (XmlNode no in nos)
            {
                //ind++;
                Itens.Add(new DocItem {
                    Nome = no.ChildNodes[2].InnerText, 
                    Endereco = no.ChildNodes[3].InnerText, 
                    Numero = no.ChildNodes[4].InnerText, 
                    Complemento = no.ChildNodes[5].InnerText, 
                    Bairro = no.ChildNodes[6].InnerText, 
                    Cidade = no.ChildNodes[7].InnerText, 
                    UF = no.ChildNodes[8].InnerText, 
                    CEP = no.ChildNodes[9].InnerText,
                    Valor = no.ChildNodes[1].InnerText,
                    Vencto = no.ChildNodes[0].InnerText
                });

                //if (ind >= 3) break;
            }

            this.Itens.Sort((x, y) => string.Compare(x.Nome, y.Nome));
            this.NormalizeList();

            cboDadosCarregados.Items.Clear();
            foreach (var i in this.Itens)
            {
                cboDadosCarregados.Items.Add(string.Concat(i.Nome, " - Vencto.: ", i.Vencto, " - Valor: R$ ", i.Valor));
            }

            cboDadosCarregados.SelectedIndex = 0;
            lblSumario.Text = string.Concat("Dados carregados: ", this.Itens.Count.ToString());

            #region comentado 
            /*
            Itens.Add(new DocItem { Nome = "Nome2", Endereco = "Avenida Lins de Vasconcelos", Numero = "473", Complemento = "Apto 24", Bairro = "Cambuci", Cidade = "Sao Paulo", UF = "SP", CEP = "03303-000" });
            Itens.Add(new DocItem { Nome = "Nome3", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome4", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome5", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome6", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome7", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome8", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome9", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome10", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome11", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome12", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome13", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome14", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome15", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome16", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome17", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome18", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome19", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome20", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome21", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome22", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome23", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome24", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome25", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome26", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome27", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome28", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome29", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            Itens.Add(new DocItem { Nome = "Nome30", Endereco = "", Numero = "", Complemento = "", Bairro = "", Cidade = "", UF = "", CEP = "" });
            */
            #endregion
        }

        private void ReadDocument()
        {
            string docName = "testPage.txt";
            string docPath = @"C:\Users\ACER E1 572 6830\Documents\";

            printDoc.DocumentName = docName;
            using (FileStream stream = new FileStream(docPath + docName, FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                documentContents = reader.ReadToEnd();
            }

            stringToPrint = documentContents;
        }

        void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            /*
             PIMACO 7072
             segunda coluna = 76.2mm (288px)
             terceira coluna = 142.3mm (537.826772px)
             linha 2 = 65.8mm (248.692913px)
             linha 3 = 116.4mm (439.937008px)
            */

            if (Itens.Count == 0) return;

            int width = 240, height = 176; //largura e altura do retangulo
            int col1 = 32, col2 = 288, col3 = 550; //int col1 = 34, col2 = 288, col3 = 550;
            int lin1 = 61, lin2 = 260, lin3 = 460, lin4 = (lin3 + 200), lin5 = (lin4 + 200); //int lin1 = 61, lin2 = 257, lin3 = 460, lin4 = (lin3 + 209), lin5 = (lin4 + 209);

            Rectangle rect01 = new Rectangle(col1, lin1, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect01);
            this.printItem(Itens[printed], col1, lin1, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col1+2, lin1+2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            Rectangle rect02 = new Rectangle(col2, lin1, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect02);
            this.printItem(Itens[printed], col2, lin1, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col2+2, lin1+2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            Rectangle rect03 = new Rectangle(col3, lin1, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect03);
            this.printItem(Itens[printed], col3, lin1, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col3+2, lin1+2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            //2a LINHA

            Rectangle rect04 = new Rectangle(col1, lin2, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect04);
            this.printItem(Itens[printed], col1, lin2, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col1 + 2, lin2 + 2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            Rectangle rect05 = new Rectangle(col2, lin2, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect05);
            this.printItem(Itens[printed], col2, lin2, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col2 + 2, lin2 + 2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            Rectangle rect06 = new Rectangle(col3, lin2, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect06);
            this.printItem(Itens[printed], col3, lin2, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col3 + 2, lin2 + 2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            //3a LINHA

            Rectangle rect07 = new Rectangle(col1, lin3, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect07);
            this.printItem(Itens[printed], col1, lin3, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col1 + 2, lin3 + 2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            Rectangle rect08 = new Rectangle(col2, lin3, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect08);
            this.printItem(Itens[printed], col2, lin3, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col2 + 2, lin3 + 2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            Rectangle rect09 = new Rectangle(col3, lin3, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect09);
            this.printItem(Itens[printed], col3, lin3, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col3 + 2, lin3 + 2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            //4a LINHA

            Rectangle rect10 = new Rectangle(col1, lin4, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect10);
            this.printItem(Itens[printed], col1, lin4, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col1 + 2, lin4 + 2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            Rectangle rect11 = new Rectangle(col2, lin4, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect11);
            this.printItem(Itens[printed], col2, lin4, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col2 + 2, lin4 + 2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            Rectangle rect12 = new Rectangle(col3, lin4, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect12);
            this.printItem(Itens[printed], col3, lin4, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col3 + 2, lin4 + 2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            //5a LINHA

            Rectangle rect13 = new Rectangle(col1, lin5, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect13);
            this.printItem(Itens[printed], col1, lin5, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col1 + 2, lin5 + 2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            Rectangle rect14 = new Rectangle(col2, lin5, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect14);
            this.printItem(Itens[printed], col2, lin5, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col2 + 2, lin5 + 2);

            printed++;
            if (printed >= Itens.Count) { printed = 0; return; }

            Rectangle rect15 = new Rectangle(col3, lin5, width, height);
            e.Graphics.DrawRectangle(Pens.White, rect15);
            this.printItem(Itens[printed], col3, lin5, e); //e.Graphics.DrawString(Itens[printed].Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, col3 + 2, lin5 + 2);

            printed++;
            if (printed >= Itens.Count) { e.HasMorePages = false; printed = 0; return; }
            else e.HasMorePages = true;

            #region comentado 
            /*

            int charactersOnPage = 0;
            int linesPerPage = 0;

            // Sets the value of charactersOnPage to the number of characters 
            // of stringToPrint that will fit within the bounds of the page.
            e.Graphics.MeasureString(stringToPrint, this.Font,
                e.MarginBounds.Size, StringFormat.GenericTypographic,
                out charactersOnPage, out linesPerPage);

            // Draws the string within the bounds of the page.
            e.Graphics.DrawString(stringToPrint, this.Font, Brushes.Black,
            e.MarginBounds, StringFormat.GenericTypographic);

            // Remove the portion of the string that has been printed.
            stringToPrint = stringToPrint.Substring(charactersOnPage);

            // Check to see if more pages are to be printed.
            e.HasMorePages = (stringToPrint.Length > 0);

            // If there are no more pages, reset the string to be printed.
            if (!e.HasMorePages)
                stringToPrint = documentContents;

            */
            #endregion
        }

        void printItem(DocItem item, int col, int lin, PrintPageEventArgs e)
        {
            int _col = col + 2, _lin = lin + 2, maxLenght = 29, lineInc = 17;

            if (item.Nome.Length <= maxLenght)
                e.Graphics.DrawString(item.Nome, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, _col, _lin);
            else
                e.Graphics.DrawString(item.Nome.Substring(0, maxLenght), new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, _col, _lin);

            string endereco = item.Endereco;
            //if (endereco.IndexOf(",") == -1 && !string.IsNullOrEmpty(item.Numero)) endereco += "," + item.Numero;
            if (!string.IsNullOrEmpty(endereco))
            {
                //if (endereco.ToLower().IndexOf("monsenhor") > -1) { int temp = 0; }
                //if (item.Nome.ToUpper().IndexOf("BERNADETE") > -1) { int temp = 0; }

                _lin += lineInc;
                //if (endereco.Length + <= maxLenght)
                if( (item.Endereco.Length + item.Numero.Length + 1) <= maxLenght)
                    e.Graphics.DrawString(item.Endereco + "," + item.Numero, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, _col, _lin);
                else
                {
                    int numeroLen = item.Numero.Length, dif = 0;
                    if (item.Endereco.Length > maxLenght)
                        endereco = item.Endereco.Substring(0, maxLenght);
                    else
                    {
                        endereco = item.Endereco;
                        dif = maxLenght - endereco.Length;
                    }

                    e.Graphics.DrawString(endereco.Substring(0, (endereco.Length-numeroLen)-dif) + "," + item.Numero, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, _col, _lin);
                }
            }

            if (!string.IsNullOrEmpty(item.Complemento))
            {
                _lin += lineInc;
                if (item.Complemento.Length <= maxLenght)
                    e.Graphics.DrawString(item.Complemento, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, _col, _lin);
                else
                    e.Graphics.DrawString(item.Complemento.Substring(0, maxLenght) + "," + item.Numero, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, _col, _lin);
            }

            _lin += lineInc;
            if (item.Bairro.Length <= maxLenght)
                e.Graphics.DrawString(item.Bairro, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, _col, _lin);
            else
                e.Graphics.DrawString(item.Bairro.Substring(0, maxLenght) + "," + item.Numero, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, _col, _lin);

            _lin += lineInc;
            e.Graphics.DrawString(item.Cidade + " - " + item.UF, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, _col, _lin);

            _lin += lineInc;
            e.Graphics.DrawString(item.CEP, new Font("Verdana", 9, FontStyle.Regular), Brushes.Black, _col, _lin);
        }


        private void cmdAbrir_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string xml = "";
                using (System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName, Encoding.GetEncoding("iso-8859-1")))
                {
                    xml = sr.ReadToEnd();
                    sr.Close();
                }

                this.GetData(xml);
            }
        }

        void NormalizeList()
        {
            foreach (DocItem i in this.Itens)
            {
                if (string.IsNullOrEmpty(i.Numero) && i.Endereco.IndexOf(',') > -1)
                {
                    string[] arr = i.Endereco.Split(',');
                    i.Endereco = arr[0].Trim();
                    i.Numero = arr[1].Trim();

                    if (i.Numero.IndexOf(' ') > -1 && string.IsNullOrEmpty(i.Complemento))
                    {
                        arr = i.Numero.Split(' ');
                        i.Numero = arr[0];
                        i.Complemento = arr[1];
                    }
                }
            }
        }

        private void cmdVisualiar_Click(object sender, EventArgs e)
        {
            if (this.Itens == null || this.Itens.Count == 0)
            {
                MessageBox.Show("Nenhuma informação carregada.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            printed = 0;
            printPreviewDialog.Document = printDoc;
            printPreviewDialog.ShowDialog();

            //printDoc.Print();
        }

        private void printPreviewButton_Click(object sender, EventArgs e)
        {
            //ReadDocument();

            if (this.Itens == null || this.Itens.Count == 0)
            {
                MessageBox.Show("Nenhuma informação carregada.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Deseja imprimir?", "Impressão", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return;

            printed = 0;
            //PrinterSettings ps = new PrinterSettings();
            //printDoc.DefaultPageSettings
            //printPreviewDialog.Document = printDoc;
            //printPreviewDialog.ShowDialog();

            printDoc.Print();
        }
    }

    public class DocItem
    {
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }

        public string Vencto { get; set; }
        public string Valor { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www
{
    public partial class boletoBB : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.montaBoleto();
        }

        void montaBoleto()
        {
            var bolBB = new BoletoBrasil();

            bolBB.Aceite = true;
            bolBB.CedenteAgencia = "59560";
            bolBB.CedenteConta = "40037";
            bolBB.CedenteContaDV = "8";
            bolBB.CedenteNome = "UBRASP";
            bolBB.Carteira = 17; // 11;
            bolBB.Instrucao1 = "";

            bolBB.Sequencial = 1;
            bolBB.Documento = "1";
            bolBB.DtDocumento = DateTime.Now;
            bolBB.DtEmissao = DateTime.Now;
            bolBB.DtProcessamento = DateTime.Now;
            bolBB.DtVencimento = DateTime.Now.AddDays(5);
            bolBB.Valor = 50;

            bolBB.SacadoNome = "Denis Gonçalves";
            bolBB.SacadoEndereco = "Rua Paulo Orozimbo, 35";
            bolBB.SacadoCPF_CNPJ = "302.789.608-39";
            bolBB.SacadoCidade = "São Paulo";
            bolBB.SacadoUF = "UF";
            bolBB.SacadoBairro = "Vila Mariana";
            bolBB.SacadoCEP = "01535-000";

            var boletoHtml = new HTMLBoleto();

            boletoHtml.ImagesFolder = "images/boleto";
            boletoHtml.AddBoleto(bolBB);
            boletoHtml.SaveToFile(@"C:\Users\ACER E1 572 6830\Desktop\dupot-cielo\boletoBB");
            Response.Write(boletoHtml.ToString());
        }
    }
}
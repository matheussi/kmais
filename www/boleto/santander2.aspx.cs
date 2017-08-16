using System;
using System.Web;
using System.Data;
using System.Web.UI;
using Impactro.Cobranca;
using System.Collections;
using System.Web.Security;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using Ent = LC.Web.PadraoSeguros.Entity;
using BoletoNet;
using System.IO;

namespace www.boleto
{
    public partial class santander2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Entity 

            Ent.Beneficiario b = new Ent.Beneficiario(Request["bid"]);
            b.Carregar();

            Ent.Contrato c = new Ent.Contrato(Request["contid"]);
            c.Carregar();

            Ent.Cobranca cobr = new Ent.Cobranca(Request["cobid"]);
            cobr.Carregar();

            if (c.EnderecoCobrancaID == null && c.EnderecoReferenciaID == null)
            {
                throw new ApplicationException("Beneficiario " + b.ID + " sem endereco.");
            }

            Ent.Endereco en = null;
            if (c.EnderecoCobrancaID != null) en = new Ent.Endereco(c.EnderecoCobrancaID);
            else en = new Ent.Endereco(c.EnderecoReferenciaID);

            try
            {
                en.Carregar();
            }
            catch
            {
                var ends = Ent.Endereco.CarregarPorDono(b.ID, Ent.Endereco.TipoDono.Beneficiario);
                if (ends != null && ends.Count > 0)
                {
                    en = new Ent.Endereco(ends[0].ID);
                    en.Carregar();
                }
            }

            string logradouro = en.Logradouro;
            if (!string.IsNullOrEmpty(en.Numero)) logradouro += string.Concat(", ", en.Numero);
            if (!string.IsNullOrEmpty(en.Complemento)) logradouro += string.Concat(" - ", en.Complemento);
            #endregion

            BoletoBancario bb;

            short _codigoBanco = 353;
            string carteira = "101";

            //string caminhoFisico = @"C:\inetpub\wwwroot\Ubrasp\www\var\boleto_file\";
            string caminhoFisico = @"D:\http\ubrasp\web\var\boleto_file\";
            //string caminhoVirtual = "http://ubrasp.iphotel.info/var/boleto_file/";

            bb = new BoletoBancario();
            bb.CodigoBanco = _codigoBanco;

            Cedente ced = new Cedente("49.938.327.0001-06", "UBRASP UNIÃO BRASILEIRA", "0001", "130147652");

            ced.Codigo = 1201344;  //Convert.ToInt32(dtoced.conta.ToString());
            ced.DigitoCedente = 2;
            ced.ContaBancaria = new ContaBancaria("0001", "13014765");

            BoletoNet.Boleto bol = new BoletoNet.Boleto(cobr.DataVencimento,
                Convert.ToDouble(cobr.Valor), carteira, Convert.ToString(cobr.ID).PadLeft(12, '0'), ced);
            bol.NumeroDocumento = c.Numero.PadLeft(11, '0');

            bol.Sacado = new Sacado(b.CPF.PadLeft(11, '0'), Ent.EntityBase.RetiraAcentos(b.Nome));
            bol.Sacado.Endereco.End = Ent.EntityBase.RetiraAcentos(logradouro);
            bol.Sacado.Endereco.Bairro = Ent.EntityBase.RetiraAcentos(en.Bairro);
            bol.Sacado.Endereco.Cidade = Ent.EntityBase.RetiraAcentos(en.Cidade);
            bol.Sacado.Endereco.CEP = en.CEP;
            bol.Sacado.Endereco.UF = en.UF;

            //if (Convert.ToString(c.ID) == "43715")
            //{
            //    Instrucao_Santander instr0 = new Instrucao_Santander();
            //    instr0.Descricao = "Boleto referente ao plano Salutar 600 Mais.";
            //    bol.Instrucoes.Add(instr0);
            //}

            if (!string.IsNullOrEmpty(cobr.Instrucoes))
            {
                Instrucao_Santander instr0 = new Instrucao_Santander();
                instr0.Descricao = cobr.Instrucoes;
                bol.Instrucoes.Add(instr0);
            }

            if (!c.Legado)
            {
                Ent.Plano pl = new Ent.Plano(c.PlanoID);
                pl.Carregar();

                Instrucao_Santander instr0 = new Instrucao_Santander();
                instr0.Descricao = string.Concat("Boleto referente ao plano ", pl.Descricao, ".");
                bol.Instrucoes.Add(instr0);
            }

            Instrucao_Santander instr = new Instrucao_Santander();
            instr.Descricao = "Não receber após " + cobr.DataVencimento.AddDays(30).ToString("dd/MM/yyyy") + ".";
            bol.Instrucoes.Add(instr);

            Instrucao_Santander instr2 = new Instrucao_Santander();
            instr2.Descricao = "Sr(a) Caixa, apos o vencimento cobrar multa de 2% e juros de 0,033% ao dia.";
            bol.Instrucoes.Add(instr2);

            bb.Boleto = bol;
            bb.Boleto.Valida();

            string dvBarras = bol.CodigoBarra.Codigo.Substring(4, 1);
            string linhaDig = bol.CodigoBarra.LinhaDigitavel;
            bol.CodigoBarra.LinhaDigitavel = linhaDig.Substring(0, 38) + dvBarras + linhaDig.Substring(39, 15);

            System.Drawing.Image img = bb.GeraImagemCodigoBarras(bol);
            String arquivoCodigoBarra = caminhoFisico + Convert.ToString(cobr.ID) + ".gif";
            if (File.Exists(arquivoCodigoBarra)) File.Delete(arquivoCodigoBarra);
            img.Save(arquivoCodigoBarra);

            string corpoBoleto = bb.MontaHtml(caminhoFisico, Convert.ToString(cobr.ID))
                .Replace(@"C:\Users\ACER E1 572 6830\AppData\Local\Temp\", "http://ubrasp.iphotel.info/images/boleto/")
                .Replace(@"http://ubrasp.iphotel.info/images/boleto/BoletoNetBarra.gifBoletoNetBarra.gif", "http://ubrasp.iphotel.info/images/boleto/BoletoNetBarra.gif.gif");
            Response.Write(corpoBoleto);
        }
    }
}
namespace www.boleto
{
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
    using LC.Web.PadraoSeguros.Entity;

    //http://www.boletoasp.com.br/FAQ.aspx

    public partial class bb : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                CedenteInfo Cedente = new CedenteInfo();
                Cedente.Cedente = "UBRASP UNIÃO BRASILEIRA DOS SERVIDORES PÚBLICOS";
                Cedente.Banco = "001-9";
                Cedente.Agencia = "5956-0";
                Cedente.Conta = "40037-8";
                Cedente.CNPJ = "49.938.327.0001-06";

                Cobranca cobr = new Cobranca(Request["cobid"]);
                cobr.Carregar();

                Cedente.Convenio = "2887755"; //Cedente.Convenio = "859120";

                cobr.Carteira = 11;

                // ATENÇÃO: Geralmente o banco informa a Carteira da seguinte forma: 16/019
                // Para o gerador isso significa sempre CARTEIRA/MODALIDADE, e ambas com apenas 2 dígitos
                // E estes devem ser configurados separadamente como indicado abaixo neste exemplo
                if (cobr.Carteira == 0)
                {
                    Cedente.Carteira = "17"; // "17"; //11";//Cedente.Carteira = "18";
                    cobr.Carteira = 17;
                    if (cobr.DataCriacao < new DateTime(2016, 9, 29))
                    { Cedente.Carteira = "11"; cobr.Carteira = 11; }
                }
                else
                    Cedente.Carteira = cobr.Carteira.ToString();

                if (Cedente.Carteira == "17")
                {
                    Cedente.Convenio = "2896830";
                }

                Cedente.Modalidade = "1"; //Cedente.Modalidade = "21"; //Modalidade da carteira: COBRANÇA SIMPLES
                

                Beneficiario b = new Beneficiario(Request["bid"]);
                b.Carregar();

                Contrato c = new Contrato(Request["contid"]);
                c.Carregar();

                if (c.EnderecoCobrancaID == null && c.EnderecoReferenciaID == null)
                {
                    throw new ApplicationException("Beneficiario " + b.ID + " sem endereco.");
                }

                Endereco en = null;
                if (c.EnderecoCobrancaID != null) en = new Endereco(c.EnderecoCobrancaID);
                else en = new Endereco(c.EnderecoReferenciaID);

                try
                {
                    en.Carregar();
                }
                catch
                {
                    var ends = Endereco.CarregarPorDono(b.ID, Endereco.TipoDono.Beneficiario);
                    if (ends != null && ends.Count > 0)
                    {
                        en = new Endereco(ends[0].ID);
                        en.Carregar();
                    }
                }

                string logradouro = en.Logradouro;
                if (!string.IsNullOrEmpty(en.Numero)) logradouro += string.Concat(", ", en.Numero);
                if (!string.IsNullOrEmpty(en.Complemento)) logradouro += string.Concat(" - ", en.Complemento);

                // Definição dos dados do sacado
                SacadoInfo Sacado = new SacadoInfo();
                Sacado.Sacado = EntityBase.RetiraAcentos(b.Nome);
                Sacado.Documento = b.CPF;
                Sacado.Endereco = EntityBase.RetiraAcentos(logradouro);
                Sacado.Cidade = EntityBase.RetiraAcentos(en.Cidade);
                Sacado.Bairro = EntityBase.RetiraAcentos(en.Bairro);
                Sacado.Cep = en.CEP;
                Sacado.UF = en.UF;
                //Sacado.Avalista = "Banco / Empresa - CNPJ: 123.456.789/00001-23";

                // Definição das Variáveis do boleto
                BoletoInfo Boleto = new BoletoInfo();

                if (cobr.Carteira == 17) cobr.NossoNumero = cobr.GeraNossoNumero();
                if (string.IsNullOrEmpty(cobr.NossoNumero))
                    Boleto.NossoNumero = Request["nossonum"]; //"131872";
                else
                    Boleto.NossoNumero = cobr.NossoNumero; //"131872";

                Boleto.NumeroDocumento = c.Numero; //"131872";
                Boleto.ValorDocumento = Convert.ToDouble(Request["valor"]); //3.50f;
                Boleto.DataDocumento = DateTime.Now;
                //Boleto.DataVencimento = new DateTime(Convert.ToInt32(Request["v_ano"]), Convert.ToInt32(Request["v_mes"]), Convert.ToInt32(Request["v_dia"]), 23, 59, 59, 990);  //DateTime.Now.AddDays(30); // DateTime.Parse("10/03/2014");
                Boleto.DataVencimento = new DateTime(cobr.DataVencimento.Year, cobr.DataVencimento.Month, cobr.DataVencimento.Day, 23, 59, 59, 990);  //DateTime.Now.AddDays(30); // DateTime.Parse("10/03/2014");

                String instrucoes = "<br>Sr(a) Caixa,<br><br>Apos o vencimento, cobrar multa de 2% e juros de 0,033% ao dia."; // String.Concat("<br>Este boleto é referente ao período de cobertura de ", Boleto.DataVencimento.Month, "/", Boleto.DataVencimento.Year, ".<br/><br/>Não receber após o vencimento.");

                Boleto.Instrucoes = instrucoes; // EntityBase.RetiraAcentos(Request["instr"]); //"Todas as informações deste bloqueto são de exclusiva responsabilidade do cedente";

                // Desabilita a parte superior do boleto (Recibo do Sacado) que pode ter um layout livre
                // bltPag.ExibirReciboSacado = false;

                // Configura as imagens
                bltPag.ImagePath = "/images/boleto/boleto/";
                bltPag.ImageType = Impactro.WebControls.BoletoImageType.gif;
                bltPag.ImageLogo = "LogoB.png"; //"Impactro-Logo.gif"; //Define a imagem do logotipo da sua empresa

                try
                {
                    // monta o boleto com os dados específicos nas classes
                    bltPag.MakeBoleto(Cedente, Sacado, Boleto);
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message + "<hr>" + ex.StackTrace);
                }
            }
        }
    }
}
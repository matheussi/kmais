namespace www
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Net.Mail;
    using System.Collections;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class cartapermanencia : PageBase
    {
        protected void Page_Load(Object sender, EventArgs e)
        {
            if (!IsPostBack && !String.IsNullOrEmpty(Request[IDKey]))
            {
                this.montaCarta();
            }
        }

        void montaCarta()
        {
            Contrato contrato = new Contrato(Request[IDKey]);
            contrato.Carregar();

            Operadora operadora = new Operadora(contrato.OperadoraID);
            operadora.Carregar();

            Estipulante estipulante = new Estipulante(contrato.EstipulanteID);
            estipulante.Carregar();

            Plano plano = new Plano(contrato.PlanoID);
            plano.Carregar();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            #region corpo do e-mail 

            sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
            sb.Append("<head><title></title></head>");
            sb.Append("<body>");
            sb.Append(" <br />");
            sb.Append(" <p>[dataagora]</p>");
            sb.Append(" <br />");

            sb.Append(" <table style=\"text-align:center;width:750px\">");
            sb.Append("     <tr>");
            sb.Append("         <td>");
            sb.Append("     <h3>DECLARAÇÃO DE PERMANÊNCIA</h3>");
            sb.Append("         <td>");
            sb.Append("         <td><img align=\"right\" src='http://www.linkecerebro.com.br/LogoMail.png' /></td>");
            sb.Append("     <tr>");
            sb.Append(" </table>");
            sb.Append(" <br /><br />");
            sb.Append(" <table style=\"text-align:justify;text-justify:inter-word;width:750px\">");
            sb.Append("     <tr>");
            sb.Append("         <td>");
            sb.Append("     Na qualidade de administradora do seu plano de assistência à saúde, coletivo por adesão mantido pela Padrão ");
            sb.Append("     Seguros Administradora de Benefícios junto com a [operadora] ");
            sb.Append("     em convênio com a [estipulante], declaramos para devidos fins de direito que ");
            sb.Append("     o(s) beneficiários(s) abaixo está(ão) inscrito(s) no plano [plano] ([acomodacao]).");
            sb.Append("         <td>");
            sb.Append("     <tr>");
            sb.Append(" </table>");
            sb.Append(" <p>");
            sb.Append("     Informamos ainda que todas as mensalidades do referido plano estão quitadas até [dataquitado].");
            sb.Append(" </p>");
            sb.Append(" <table width=\"750\" cellpadding=\"3\" cellspacing=\"0\" style=\"border:solid 1px black\">");
            sb.Append("     <tr>");
            sb.Append("         <th align=\"left\">Nome do beneficiário</th>");
            sb.Append("         <th align=\"left\">Vigência</th>");
            sb.Append("         <th align=\"left\">Código de Identificação</th>");
            sb.Append("         <th align=\"left\">Condição</th>");
            sb.Append("     </tr>");
            sb.Append("     [beneficiarios]");
            sb.Append(" </table>");
            sb.Append(" <br />");
            sb.Append(" <p>");
            sb.Append("     Acrescentamos que este plano privado de assistência à saúde coletivo atende às normas e à legislação aplicável.");
            sb.Append(" </p>");
            sb.Append(" <br />");
            sb.Append(" <p>");
            sb.Append("     Atenciosamente,<br /><br />");
            sb.Append("     PS Padrão Administradora de Benefícios Ltda.<br /><br />");
            sb.Append("     CNPJ : 11.273.573/0001-05<br /><br />");
            sb.Append("     <span>ANS nº 417172</span>");
            sb.Append(" </p>");
            sb.Append("</body>");
            sb.Append("</html>");

            String corpo = sb.ToString();
            corpo = corpo.Replace("[operadora]", operadora.Nome.Split('-')[1].Trim());
            corpo = corpo.Replace("[dataagora]", String.Concat("São Paulo, ", DateTime.Now.ToLongDateString(), "."));
            corpo = corpo.Replace("[estipulante]", estipulante.Descricao);
            corpo = corpo.Replace("[plano]", plano.Descricao);

            if(contrato.TipoAcomodacao == (int)Contrato.eTipoAcomodacao.quartoParticular)
                corpo = corpo.Replace("[acomodacao]", "quarto particular");
            else
                corpo = corpo.Replace("[acomodacao]", "quarto coletivo");

            IList<ContratoBeneficiario> beneficiarios =
                ContratoBeneficiario.CarregarPorContratoID(contrato.ID, true, false);

            System.Text.StringBuilder sbBenef = new System.Text.StringBuilder();
            foreach (ContratoBeneficiario cb in beneficiarios)
            {
                sbBenef.Append("<tr>");

                sbBenef.Append("<td align=\"left\">");
                sbBenef.Append(cb.BeneficiarioNome);
                sbBenef.Append("</td>");

                sbBenef.Append("<td align=\"left\">");
                sbBenef.Append(cb.Data.ToString("dd/MM/yyyy"));
                sbBenef.Append("</td>");

                sbBenef.Append("<td align=\"left\">");
                sbBenef.Append(cb.NumeroMatriculaSaude);
                sbBenef.Append("</td>");

                sbBenef.Append("<td align=\"left\">");
                if (cb.NumeroSequencial == 0)
                    sbBenef.Append("Titular");
                else
                    sbBenef.Append("Dependente");
                sbBenef.Append("</td>");

                sbBenef.Append("</tr>");
            }

            corpo = corpo.Replace("[beneficiarios]", sbBenef.ToString());

            IList<Cobranca> cobrancas = Cobranca.CarregarTodas(contrato.ID, true, Cobranca.eTipo.Normal, null);
            DateTime dataQuitadoPago = DateTime.MinValue, dataQuitadoEmAberto = DateTime.MinValue;
            if (cobrancas != null)
            {
                foreach (Cobranca cobranca in cobrancas)
                {
                    if (cobranca.Pago)
                        dataQuitadoPago = cobranca.DataVencimento;
                    else
                    {
                        dataQuitadoEmAberto = cobranca.DataVencimento;
                        break;
                    }
                }
            }

            //if (dataQuitadoEmAberto != DateTime.MinValue)
            //    corpo = corpo.Replace("[dataquitado]", dataQuitadoEmAberto.ToString("dd") + " de " + dataQuitadoEmAberto.ToString("MMMM") + " de " + dataQuitadoEmAberto.Year.ToString());
            //else
            //    corpo = corpo.Replace("[dataquitado]", dataQuitadoPago.ToString("dd") + " de " + dataQuitadoPago.ToString("MMMM") + " de " + dataQuitadoPago.Year.ToString());
            if (dataQuitadoPago != DateTime.MinValue)
                corpo = corpo.Replace("[dataquitado]", dataQuitadoPago.ToString("dd") + " de " + dataQuitadoPago.ToString("MMMM") + " de " + dataQuitadoPago.Year.ToString());
            else
                corpo = corpo.Replace("[dataquitado]", dataQuitadoEmAberto.ToString("dd") + " de " + dataQuitadoEmAberto.ToString("MMMM") + " de " + dataQuitadoEmAberto.Year.ToString());

            #endregion corpo do e-mail 

            litCorpo.Text = corpo;
        }
    }
}

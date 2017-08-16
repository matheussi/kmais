namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;
    using System.Collections;

    public partial class demonstPagtosQuali : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.carregaInfo();
            }
        }

        void carregaInfo()
        {
            String ano = Request.QueryString["ano"];
            if (String.IsNullOrEmpty(ano)) { return; }

            String contratoId = Request.QueryString[IDKey];
            if (String.IsNullOrEmpty(contratoId)) { return; }

            litAno.Text = ano;
            litAnoCalendario.Text = ano;
            litData.Text = String.Concat("São Paulo, ", DateTime.Now.Day, " de ", DateTime.Now.ToString("MMMM"), " de ", DateTime.Now.Year, ".");

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            //DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ir_dados_preprod_sp (nolock) where UTILIZAR_REGISTRO = 1 AND ENVIAR_DMED = 1 AND idcedente=1 and idproposta=" + contratoId, "result", pm).Tables[0];
            //if (dados.Rows.Count == 0)
            //{
            //    dados.Dispose();
            //    Alerta(null, this, "err", "Demonstrativo indisponível.");
            //    pm.CloseSingleCommandInstance();
            //    pm.Dispose();
            //    return;
            //}
            ////DataRow[] ret = dados.Select("UTILIZAR_REGISTRO = 0 OR ENVIAR_DMED = 0");
            ////if (ret.Length > 0)
            ////{
            ////    dados.Dispose();
            ////    Alerta(null, this, "err", "Demonstrativo indisponível.");
            ////    pm.CloseSingleCommandInstance();
            ////    pm.Dispose();
            ////    return;
            ////}

            /********************************/

            //string titularContratoBeneficiarioId = "";
            //List<String> dependContratoBeneficiarioIds = new List<string>();

            //foreach (DataRow row in dados.Rows)
            //{
            //    if (base.CToString(row["SEQUENCIA"]) == "0") { titularContratoBeneficiarioId = base.CToString(row["IDPROPONENTE"]); continue; }

            //    dependContratoBeneficiarioIds.Add(base.CToString(row["IDPROPONENTE"]));
            //}


            Contrato contrato = Contrato.CarregarParcial((Object)contratoId, pm);
            Operadora operadora = new Operadora(contrato.OperadoraID);
            pm.Load(operadora);

            if (operadora.Nome.IndexOf('-') == -1)
                litOperadoraNome.Text = operadora.Nome;
            else
                litOperadoraNome.Text = operadora.Nome.Split('-')[1].Trim();

            ContratoBeneficiario cbeneficiario = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);

            if (!string.IsNullOrEmpty(contrato.ResponsavelNome) && !string.IsNullOrEmpty(contrato.ResponsavelCPF))
            {
                litNomeTitular.Text = contrato.ResponsavelNome;
                litCpfTitular.Text  = contrato.ResponsavelCPF;
            }
            else
            {
                litNomeTitular.Text = cbeneficiario.BeneficiarioNome;
                litCpfTitular.Text = cbeneficiario.BeneficiarioCPF;
            }

            if (Convert.ToString(contrato.ID) == "43623") //neste caso deve inverter os dados de dependente e titular
            {
                Beneficiario dep = new Beneficiario(40312);
                pm.Load(dep);

                contrato.ResponsavelNome = dep.Nome;
                contrato.ResponsavelCPF = dep.CPF;

                litNomeTitular.Text = dep.Nome;
                litCpfTitular.Text = dep.CPF;
            }

            //if (cbeneficiario.DMED == false)
            //{
            //    Alerta(null, this, "err", "Titular reprovado na DMED.");
            //    pm.CloseSingleCommandInstance();
            //    pm.Dispose();
            //    return;
            //}

            IList<Cobranca> cobrancas = Cobranca.CarregarPagasDoAno(contratoId, Convert.ToInt32(ano), pm);
            this.configValorDoMes(cobrancas);

            //this.calculaValores(pm, contrato, ano, cbeneficiario, dados, null);

            pm.CloseSingleCommandInstance();
            pm.Dispose();
        }

        void calculaValores(PersistenceManager pm, Contrato contrato, String ano, ContratoBeneficiario cTitular, DataTable dados, List<string> dependContratoBeneficiarioIds)
        {
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            Decimal totalOut = 0, totalNov = 0, totalDez = 0;


            totalOut = base.CToDecimal(dados.Compute("SUM(OUTU)", ""));
            totalNov = base.CToDecimal(dados.Compute("SUM(NOV)", ""));
            totalDez = base.CToDecimal(dados.Compute("SUM(DEZ)", ""));

            IList<ContratoBeneficiario> cbeneficiarios = null;

            if (dependContratoBeneficiarioIds != null && dependContratoBeneficiarioIds.Count > 0)
                cbeneficiarios = ContratoBeneficiario.Carregar(dependContratoBeneficiarioIds.ToArray(), pm);

            cTitular.Valor = base.CToDecimal(dados.Compute("sum(OUTU)+sum(NOV)+sum(DEZ)", "IDPROPONENTE=" + cTitular.ID));

            if (cbeneficiarios != null)
            {
                foreach (ContratoBeneficiario cb in cbeneficiarios)
                {
                    cb.Valor = base.CToDecimal(dados.Compute("sum(OUTU)+sum(NOV)+sum(DEZ)", "IDPROPONENTE=" + cb.ID));
                }
            }
            else
                cbeneficiarios = new List<ContratoBeneficiario>();

            cbeneficiarios.Insert(0, cTitular);
            this.escreveDocumento(cbeneficiarios, totalOut, totalNov, totalDez);
        }

        void escreveDocumento(IList<ContratoBeneficiario> cbeneficiarios, Decimal totalOut, Decimal totalNov, Decimal totalDez)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Decimal total = 0;

            foreach (ContratoBeneficiario cb in cbeneficiarios)
            {
                if (cb.Valor > 0)
                {
                    sb.Append("<tr>");

                    sb.Append("<td>");
                    if (cb.Tipo == 0)
                        sb.Append("Titular");
                    else
                        sb.Append("Dependente");
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(cb.BeneficiarioNome);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(cb.Valor.ToString("N2"));
                    sb.Append("</td>");

                    sb.Append("</tr>");
                }

                total += cb.Valor;
            }

            litTotalBeneficiario.Text = total.ToString("N2");
            litTrs.Text = sb.ToString();

            litOutubro.Text = totalOut.ToString("N2");
            litNovembro.Text = totalNov.ToString("N2");
            litDezembro.Text = totalDez.ToString("N2");

            litTotal.Text = total.ToString("N2");
        }

        void configValorDoMes(IList<Cobranca> cobrancas)
        {
            if (cobrancas == null) return;

            Decimal total = 0, jan = 0, fev = 0, mar = 0, abr = 0, mai = 0, jun = 0, jul = 0, ago = 0, sete = 0, outu = 0, nov = 0, dez = 0;

            foreach (Cobranca cob in cobrancas)
            {
                switch (cob.DataVencimento.Month)
                {
                    case 1:
                    {
                        jan += cob.Valor;
                        litJaneiro.Text = jan.ToString("N2");
                        total += cob.Valor;
                        break;
                    }
                    case 2:
                    {
                        fev += cob.Valor;
                        litFevereiro.Text = cob.Valor.ToString("N2");
                        total += cob.Valor;
                        break;
                    }
                    case 3:
                    {
                        mar += cob.Valor;
                        litMarco.Text = mar.ToString("N2");
                        total += cob.Valor;
                        break;
                    }
                    case 4:
                    {
                        abr += cob.Valor;
                        litAbril.Text = abr.ToString("N2");
                        total += cob.Valor;
                        break;
                    }
                    case 5:
                    {
                        mai += cob.Valor;
                        litMaio.Text = mai.ToString("N2");
                        total += cob.Valor;
                        break;
                    }
                    case 6:
                    {
                        jun += cob.Valor;
                        litJunho.Text = jun.ToString("N2");
                        total += cob.Valor;
                        break;
                    }
                    case 7:
                    {
                        jul += cob.Valor;
                        litJulho.Text = jul.ToString("N2");
                        total += cob.Valor;
                        break;
                    }
                    case 8:
                    {
                        ago += cob.Valor;
                        litAgosto.Text = ago.ToString("N2");
                        total += cob.Valor;
                        break;
                    }
                    case 9:
                    {
                        sete += cob.Valor;
                        litSetembro.Text = sete.ToString("N2");
                        total += cob.Valor;
                        break;
                    }
                    case 10:
                    {
                        outu += cob.Valor;
                        litOutubro.Text = outu.ToString("N2");
                        total += cob.Valor;
                        break;
                    }
                    case 11:
                    {
                        nov += cob.Valor;
                        litNovembro.Text = nov.ToString("N2");
                        total += cob.Valor;
                        break;
                    }
                    case 12:
                    {
                        dez += cob.Valor;
                        litDezembro.Text = dez.ToString("N2");
                        total += cob.Valor;
                        break;
                    }
                }
            }

            litTotal.Text = total.ToString("N2");
        }
    }
}

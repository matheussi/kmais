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

    public partial class demonstPagtos : PageBase
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

            litAnoCalendario.Text = ano;

            String contratoId = Request.QueryString[IDKey];
            if (String.IsNullOrEmpty(contratoId)) { return; }

            litAno.Text = ano;
            litData.Text = String.Concat("São Paulo, ", DateTime.Now.Day, " de ", DateTime.Now.ToString("MMMM"), " de ", DateTime.Now.Year, ".");

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            Contrato contrato = Contrato.CarregarParcial((Object)contratoId, pm);
            Operadora operadora = new Operadora(contrato.OperadoraID);
            pm.Load(operadora);

            if (operadora.Nome.IndexOf('-') == -1)
                litOperadoraNome.Text = operadora.Nome;
            else
                litOperadoraNome.Text = operadora.Nome.Split('-')[1].Trim();

            ContratoBeneficiario cbeneficiario = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);
            litNomeTitular.Text = cbeneficiario.BeneficiarioNome;
            litCpfTitular.Text = cbeneficiario.BeneficiarioCPF;

            this.carregaInfoFinanceira(pm, contrato, ano);

            pm.CloseSingleCommandInstance();
            pm.Dispose();
        }

        void carregaInfoFinanceira(PersistenceManager pm, Contrato contrato, String ano)
        {
            List<CobrancaComposite> comp = new List<CobrancaComposite>();
            IList<Cobranca> temp = Cobranca.CarregarTodas(contrato.ID, true, pm);
            List<Cobranca> cobrancas = new List<Cobranca>();
            foreach (Cobranca cob in temp)
            {
                if (cob.DataVencimento.Year.ToString() != ano || !cob.Pago) { continue; }

                comp = (List<CobrancaComposite>)CobrancaComposite.Carregar(cob.ID, pm);

                if (comp == null || comp.Count == 0)
                {
                    cob.Valor = Contrato.CalculaValorDaProposta2(
                        contrato.ID, cob.DataVencimento, pm, false, false, ref comp, false);
                }
                else
                {
                    cob.Valor = 0;
                    foreach (CobrancaComposite c in comp)
                    {
                        if (c.Tipo == (int)CobrancaComposite.eComposicaoTipo.Adicional ||
                            c.Tipo == (int)CobrancaComposite.eComposicaoTipo.Plano)
                        {
                            cob.Valor += c.Valor;
                        }
                    }
                }

                cobrancas.Add(cob);
            }

            configValorDoMes(cobrancas);

            IList<ContratoBeneficiario> cbeneficiarios = ContratoBeneficiario.CarregarPorContratoID(contrato.ID, false, false, pm);

            Decimal valorPerBenef = 0, total = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (ContratoBeneficiario cb in cbeneficiarios)
            {
                valorPerBenef = 0;

                foreach (Cobranca cobr in cobrancas)
                {
                    valorPerBenef +=
                        Contrato.CalculaValorDaPropostaSemTaxaAssociativa(
                        contrato.ID, cb, cobr.DataVencimento, pm);
                }

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
                sb.Append(valorPerBenef.ToString("N2"));
                sb.Append("</td>");

                sb.Append("</tr>");

                total += valorPerBenef;
            }

            litTotalBeneficiario.Text = total.ToString("N2");
            litTrs.Text = sb.ToString();
        }

        void configValorDoMes(List<Cobranca> cobrancas)
        {
            Decimal total = 0;
            foreach (Cobranca cob in cobrancas)
            {
                switch (cob.DataVencimento.Month)
                {
                    case 1:
                        {
                            litJaneiro.Text = cob.Valor.ToString("N2");
                            total += cob.Valor;
                            break;
                        }
                    case 2:
                        {
                            litFevereiro.Text = cob.Valor.ToString("N2");
                            total += cob.Valor;
                            break;
                        }
                    case 3:
                        {
                            litMarco.Text = cob.Valor.ToString("N2");
                            total += cob.Valor;
                            break;
                        }
                    case 4:
                        {
                            litAbril.Text = cob.Valor.ToString("N2");
                            total += cob.Valor;
                            break;
                        }
                    case 5:
                        {
                            litMaio.Text = cob.Valor.ToString("N2");
                            total += cob.Valor;
                            break;
                        }
                    case 6:
                        {
                            litJunho.Text = cob.Valor.ToString("N2");
                            total += cob.Valor;
                            break;
                        }
                    case 7:
                        {
                            litJulho.Text = cob.Valor.ToString("N2");
                            total += cob.Valor;
                            break;
                        }
                    case 8:
                        {
                            litAgosto.Text = cob.Valor.ToString("N2");
                            total += cob.Valor;
                            break;
                        }
                    case 9:
                        {
                            litSetembro.Text = cob.Valor.ToString("N2");
                            total += cob.Valor;
                            break;
                        }
                    case 10:
                        {
                            litOutubro.Text = cob.Valor.ToString("N2");
                            total += cob.Valor;
                            break;
                        }
                    case 11:
                        {
                            litNovembro.Text = cob.Valor.ToString("N2");
                            total += cob.Valor;
                            break;
                        }
                    case 12:
                        {
                            litDezembro.Text = cob.Valor.ToString("N2");
                            total += cob.Valor;
                            break;
                        }
                }
            }

            litTotal.Text = total.ToString("N2");
        }
    }
}

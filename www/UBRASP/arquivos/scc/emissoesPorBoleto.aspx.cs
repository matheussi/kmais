namespace www.UBRASP.arquivos.scc
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Facade;
    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Entity.ArquivoUbrasp;

    public partial class emissoesPorBoleto : PageBase
    {
        List<ItemBoletoVO> Boletos
        {
            get { return ViewState["bolet"] as List<ItemBoletoVO>; }
            set { ViewState["bolet"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtVencimento.Text = string.Concat("15/", DateTime.Now.Month + 1, "/", DateTime.Now.Year);
                this.carregaDados();
            }
        }

        void carregaDados()
        {
            //Cobranca cob = null;
            DataTable dtAdicionais = new DataTable();
            object aux = null;
            AdicionalBeneficiario ab = null;
            decimal valor = 0;
            DateTime vigencia = new DateTime(1850, 01, 01);
            Beneficiario beneficiario = null;
            bool temPlano = false;
            List<string> idsOk = new List<string>();
            decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M;

            DateTime vencimento = base.CStringToDateTime(txtVencimento.Text, "23:59", 59, 995);

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    DataTable dt1 = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.*,beneficiario_matriculaFuncional,beneficiario_id,beneficiario_nome,beneficiario_matriculaAssociativa  from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid  inner join beneficiario on beneficiario_id = adicionalbeneficiario_beneficiarioid  inner join contrato on contrato_id = adicionalbeneficiario_propostaId where adicionalbeneficiario_formaPagto=31 and adicionalbeneficiario_recorrente=1 and contrato_inativo=0 and contrato_cancelado=0 order by beneficiario_nome,adicionalbeneficiario_propostaId", "result", pm).Tables[0]; //and contrato_id=35598 
                    //List<Result> resultado = new List<Result>();
                    //List<string> funcionais = new List<string>();
                    List<string> ids = new List<string>();

                    foreach (DataRow row in dt1.Rows)
                    {
                        //if (funcionais.Contains(Convert.ToString(row["beneficiario_matriculaFuncional"]))) continue;
                        //funcionais.Add(Convert.ToString(row["beneficiario_matriculaFuncional"]));

                        if (ids.Contains(Convert.ToString(row["beneficiario_id"]))) continue;
                        ids.Add(Convert.ToString(row["beneficiario_id"]));
                    }

                    int total = dt1.Rows.Count;
                    int i = 0;

                    List<ItemBoletoVO> vos = new List<ItemBoletoVO>();
                    foreach (string id in ids)//foreach (DataRow row in dt1.Rows)
                    {
                        i++;
                        valor = 0;
                        aux = id;

                        ItemBoletoVO vo = new ItemBoletoVO();

                        beneficiario = new Beneficiario(aux);
                        pm.Load(beneficiario);

                        vo.Funcional    = beneficiario.MatriculaFuncional;
                        vo.CPF          = beneficiario.CPF;
                        vo.Marcado      = true;
                        vo.Atualizado   = "SIM";
                        vo.Nome         = beneficiario.Nome;
                        vo.BeneficiarioId = Convert.ToString(beneficiario.ID);
                        

                        dtAdicionais.Rows.Clear();
                        dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid where adicionalbeneficiario_formaPagto=31 and adicionalbeneficiario_recorrente=1 and adicionalbeneficiario_beneficiarioid=" + aux, "result", pm).Tables[0];

                        //valorSemReajuste = 0;

                        temPlano = false;

                        foreach (DataRow rowAd in dtAdicionais.Rows)
                        {
                            ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                            if (ab == null) continue;
                            if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes
                            if (ab.Atualizado == false) { vo.Atualizado = "NAO"; }

                            //valorSemReajuste += ab.RetornaValor(AdicionalBeneficiario._FormaPagtoBoleto, vigencia);

                            //todo: denis estava comentado. descomentei, mas nao descomentei o salvar logo abaixo: pm.Save(ab);
                            if (!ab.Atualizado)
                            {
                                ab.AplicaRejuste(reajustePlano, reajusteTaxa, reajusteSeguro);
                                ab.Atualizado = true;
                            }

                            valor += ab.RetornaValor(AdicionalBeneficiario._FormaPagtoBoleto, vigencia);//valorTotalReajustado += ab.RetornaValor(-1, vigencia);

                            if (Convert.ToInt32(ab.AdicionalCodigo) >= 4435 && Convert.ToInt32(ab.AdicionalCodigo) <= 4442) temPlano = true;

                            //excecao para SEBASTIANA CANO RODRIGUES CESETTI e JOAO BATISTA FERREIRA GAIA
                            //if (Convert.ToInt32(beneficiario.ID) == 31095) temPlano = true; //Convert.ToInt32(beneficiario.ID) == 36155 || 

                            //pm.Save(ab);
                        }

                        //verifica se tem plano
                        if (!temPlano || valor == 0) // não gera boleto para quem não é medial ou nao tem cobranca por boleto
                        {
                            continue;
                        }

                        //TODO: tirar DUVIDA: e se tiver previdencia ou seguro ??? não pode ? ======================

                        //obtem id do contrato
                        aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 where contrato_cancelado=0 and contrato_inativo=0 and contratobeneficiario_beneficiarioId=" + beneficiario.ID, null, null, pm);
                        if (aux == null || aux == DBNull.Value) continue;

                        vo.ContratoId = Convert.ToString(aux);

                        aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select cobranca_id from cobranca where cobranca_cancelada=0 and cobranca_propostaid=", aux, " and month(cobranca_dataVencimento)=", vencimento.Month, " and year(cobranca_dataVencimento)=", vencimento.Year), null, null, pm);
                        if (aux != null && aux != DBNull.Value) continue;

                        //cob = new Cobranca();
                        //cob.Tipo = (int)Cobranca.eTipo.BoletoUbraspSP;
                        //cob.Parcela = 0;
                        //cob.DataVencimento = new DateTime(2016, 10, 18, 23, 59, 59, 995);
                        //cob.Valor = valor;
                        //cob.CobrancaRefID = null;
                        //cob.DataPgto = DateTime.MinValue;
                        //cob.ValorPgto = Decimal.Zero;
                        //cob.Pago = false;
                        //cob.PropostaID = aux;
                        //cob.Cancelada = false;
                        //cob.ArquivoIDUltimoEnvio = -4;

                        vo.ValorSistema = valor;
                        vos.Add(vo);

                        if (vo.Marcado)
                        {
                            idsOk.Add(vo.BeneficiarioId);
                        }

                        //if (cob.Valor > 3)
                        //{
                        //    //pm.Save(cob);
                        //    //NonQueryHelper.Instance.ExecuteNonQuery("update ___result set result_boletoGerado=1 where result_matriculaAsso='" + r.Associativa + "'", pm);
                        //}
                    }

                    //foreach (var r in resultado)
                    //{
                    //    //pm.Save(r);
                    //}

                    pm.Commit();
                    grid.DataSource = vos;
                    grid.DataBind();

                    this.Boletos = vos;
                }
                catch
                {
                    pm.Rollback();
                    throw;
                }
                finally
                {
                    pm.Dispose();
                }
            }
        }

        protected void cmdBoleto_click(object sender, EventArgs e)
        {
            if (this.Boletos == null || this.Boletos.Count == 0) { return; }

            object aux = null;
            Cobranca cob = null;

            DateTime vencimento = base.CStringToDateTime(txtVencimento.Text, "23:59", 59, 995);  //new DateTime(2016, 12, 15, 23, 59, 59, 995);
            if (vencimento == DateTime.MinValue)
            {
                base.Alerta(null, this, "err_", "Data de vencimento inválida.");
                return;
            }

            if (txtQtd.Text.Trim() == "")
            {
                base.Alerta(null, this, "err_", "Informe a quantidade de boletos.");
                return;
            }

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    foreach (var vo in this.Boletos)
                    {
                        //obtem id do contrato
                        aux = vo.ContratoId;// LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 where contratobeneficiario_beneficiarioId=" + vo.BeneficiarioId, null, null, pm);

                        if (aux == null || aux == DBNull.Value)
                        { throw new ApplicationException("Não encontrou o id de contrato."); }

                        for (int i = 1; i <= Convert.ToInt32(txtQtd.Text); i++)
                        {
                            cob = new Cobranca();
                            cob.Tipo = (int)Cobranca.eTipo.BoletoUbraspSP;
                            cob.Parcela = 0;

                            if (i == 1)
                                cob.DataVencimento = vencimento;
                            else
                                cob.DataVencimento = vencimento.AddMonths(i - 1);

                            cob.Valor = vo.ValorSistema;
                            cob.Carteira = 101;
                            cob.Banco = 353; //santander
                            cob.CobrancaRefID = null;
                            cob.DataPgto = DateTime.MinValue;
                            cob.ValorPgto = Decimal.Zero;
                            cob.Pago = false;
                            cob.PropostaID = aux;
                            cob.Cancelada = false;
                            cob.ArquivoIDUltimoEnvio = -3;

                            pm.Save(cob);
                        }
                    }

                    //gera o agendamento do arquivo
                    ArquivoRemessaAgendamento ara = new ArquivoRemessaAgendamento();
                    ara.CriterioID = null;
                    ara.Processado = false;
                    ara.ProcessamentoEm = DateTime.Now.AddMinutes(10);

                    if (txtQtd.Text == "1")
                        ara.VencimentoAte = vencimento;
                    else
                        ara.VencimentoAte = vencimento.AddMonths((Convert.ToInt32(txtQtd.Text) - 1));
                    
                    ara.VencimentoDe = vencimento;
                    ara.VigenciaDe = new DateTime(1800, 1, 1, 23, 59, 59, 995);
                    ara.VigenciaAte = DateTime.Now.AddDays(90);
                    ara.QtdBoletos = Convert.ToInt32(txtQtd.Text);
                    ara.Carteira = 101;
                    ara.Banco = 353; //santander

                    ara.SomenteBoletosUBRASP  = true;
                    ara.SomenteNaoRecorrentes = false;

                    ara.Grupo = null;
                    ara.ArquivoNomeInstance = "remessa_boletos";
                    ara.Legado = true;
                    pm.Save(ara);

                    pm.Commit();

                    Alerta(null, this, "_ok", "Agendamento salvo com sucesso.");
                }
                catch
                {
                    pm.Rollback();
                    throw;
                }
                finally
                {
                    pm.Dispose();
                }
            }
        }
    }

    [Serializable]
    class ItemBoletoVO
    {
        public string Nome { get; set; }
        public string BeneficiarioId { get; set; }
        public string ContratoId { get; set; }
        public string NomeLocalizado { get; set; }
        public string Funcional { get; set; }
        public string CPF { get; set; }
        public string ORGAO { get; set; }
        //public string NUAverb { get; set; }
        public string Parcela { get; set; }
        public string Status { get; set; }
        public decimal ValorArquivo { get; set; }
        public decimal ValorSistema { get; set; }
        public decimal ValorDescontado { get; set; }
        public decimal Residual { get; set; }
        public bool Marcado { get; set; }
        public string Atualizado { get; set; }
    }
}
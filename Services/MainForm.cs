﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

using System.Data.SqlClient;
using System.Net.Mail;

using LC.Framework.Phantom;
using LC.Framework.DataUtil;
using LC.Web.PadraoSeguros.Entity;
using System.Configuration;
using System.Net;
using System.Data.OleDb;
using Excel;

using iTextSharp;
using iTextSharp.text;
using iTextSharp.xtra;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace Services
{
    public partial class MainForm : Form
    {
        Boolean busy = false;

        public MainForm()
        {
            InitializeComponent();

            timerRetorno.Interval = 600000;//////////////////

            label1.Text = "";
            label7.Text = "";
            lblMsgRemessa2.Text = "";
            lblMsgRemessa.Text = "";

            ///////////////////////////////////
            timerImportacao.Enabled = false;
            timerRelatorioFinanceiro.Enabled = false;

            button2.Enabled = false; //importacao amil
            cmdRelAgend.Enabled = false;
            //////////////////////////////////
            timerRemessa.Enabled = false;
            cmdIniciarRemessa.Enabled = false;
            //////////////////////////////////
            cmdIniciarRemessa2.Enabled = true; ///////////////////////////
            timerRemessa2.Enabled = true;
            //////////////////////////////////
            timerRetorno.Enabled = true;
            cmdIniciar.Enabled = true; //retorno
            /////////////////////////////////
            cmdIniciarListagem.Enabled = false;
            timerComissao.Enabled = false;

            //RetornoInput inp = new RetornoInput();
            //inp.ID = 302;
            //inp.Carregar();

            //enviaIR();/////////////////////
            //timerMeuResultadoEMAILS_Tick(null, null);


            //IList<string> lista = new List<string>();

            #region teste nao descontados 
            //lista.Add("2744");
            //lista.Add("2754");
            //lista.Add("2764");
            //lista.Add("2752");
            //lista.Add("2755");
            //lista.Add("2746");
            //lista.Add("2774");
            //lista.Add("2757");
            //lista.Add("2745");
            //lista.Add("2769");
            //lista.Add("2777");
            //lista.Add("2778");
            //lista.Add("2759");
            //lista.Add("2761");
            //lista.Add("2758");
            //lista.Add("2748");
            //lista.Add("2775");
            //lista.Add("2780");
            //lista.Add("2747");
            //lista.Add("2742");
            //lista.Add("2765");
            //lista.Add("2741");
            //lista.Add("2750");
            //lista.Add("2743");
            //lista.Add("2766");
            //lista.Add("2767");
            //lista.Add("2763");
            //lista.Add("2760");
            //lista.Add("2773");
            //lista.Add("2753");
            //lista.Add("2762");
            //lista.Add("2768");
            //lista.Add("2771");
            //lista.Add("2776");
            //lista.Add("2749");
            //lista.Add("2770");
            //lista.Add("2751");
            //lista.Add("2779");
            //lista.Add("2756");
            //lista.Add("2772");
            #endregion

            #region boletos 
            //lista.Add("2782");
            //lista.Add("2789");
            //lista.Add("2787");
            //lista.Add("2781");
            //lista.Add("2792");
            //lista.Add("2793");
            //lista.Add("2784");
            //lista.Add("2788");
            //lista.Add("2797");
            //lista.Add("2786");
            //lista.Add("2785");
            //lista.Add("2794");
            //lista.Add("2790");
            //lista.Add("2783");
            //lista.Add("2796");
            //lista.Add("2795");
            //lista.Add("2791");
            #endregion

            //string ret = ArquivoCobrancaUnibanco.GeraDocumentoCobranca_BB17_Test(lista, null);
            //busy = true;

            //busy = true;
            //setaNossosNumeros();
            //gerarArquivoBoletoLote2();
            ////geraBoletosNAOrecorrentes();
            //gerarArquivoEtiquetaLote_TEMP3(); // USAR ESTE
            //verificaDadosProdesp();

            //batimentoCobrancas();
            //batimentoCobrancasBAIXA();

            //string ret = ArquivoCobrancaUnibanco.GeraDocumentoCobranca_SANTANDER("3163", null);

            //string ret = ArquivoCobrancaUnibanco.GeraDocumentoCobranca_BB_Test(null, null);
            //string ret2 = ret;

            //string id2 = "000000003374";
            //string id = "3374";
            //string ret1 = Mod11(id, 9, 0).ToString();
            //string ret2 = DVMod11(id);
            //string ret3 = DVMod11_prova(Convert.ToInt64( id)).ToString();
            //string ret4 = DVMod11_prova2(id);

            //this.verificarArquivosGerados();

            //gerarCNAB_TEMP();

            //busy = true;
            //Cobranca.ProcessarNormais_TEMP_DEVIDO_ERRO_GERACAO_MES06();

            //busy = true;
            //arrumaCobrancasErradas();
        }

        void verificarArquivosGerados()
        {
            //string[] arr = File.ReadAllLines(@"C:\remessa_diferenca.txt");
            string[] arr = File.ReadAllLines(@"C:\remessa_boletos.txt");
            string id = "", dv = "", prova = "";
            List<string> idsComErroDv = new List<string>();

            int i = 0;
            foreach(string linha in arr)
            {
                if (linha.Substring(13, 1) == "P")
                {
                    i++;
                    id = linha.Substring(52, 4);
                    dv = linha.Substring(56, 1);

                    prova = Mod11(id, 9, 0).ToString();
                    if (prova != dv)
                        idsComErroDv.Add(id);
                }
            }

            string ids = string.Join(",", idsComErroDv.ToArray());
        }


        int Mod11(string seq, int lim, int flag)
        {
            int mult = 0;
            int total = 0;
            int pos = 1;
            //int res = 0;
            int ndig = 0;
            int nresto = 0;
            string num = string.Empty;

            mult = 1 + (seq.Length % (lim - 1));

            if (mult == 1)
                mult = lim;


            while (pos <= seq.Length)
            {
                num = Microsoft.VisualBasic.Strings.Mid(seq, pos, 1);
                total += Convert.ToInt32(num) * mult;

                mult -= 1;
                if (mult == 1)
                    mult = lim;

                pos += 1;
            }
            nresto = (total % 11);
            if (flag == 1)
                return nresto;
            else
            {
                if (nresto == 0 || nresto == 1 || nresto == 10)
                    ndig = 1;
                else
                    ndig = (11 - nresto);

                return ndig;
            }
        }
        String DVMod11(String nossoNumero)
        {
            Int32 fatorMult = 2;
            Int32 resultado = 0;

            char[] buffer = nossoNumero.ToCharArray();
            Array.Reverse(buffer);
            String nossoNumeroReverso = new String(buffer);

            for (int i = 0; i < nossoNumeroReverso.Length; i++)
            {
                resultado += Convert.ToInt32(nossoNumeroReverso.Substring(i, 1)) * fatorMult;
                fatorMult++;
                if (fatorMult > 9) { fatorMult = 2; }
            }

            resultado *= 10;
            resultado %= 11;
            resultado %= 10;

            return resultado.ToString();
        }
        int DVMod11_prova(long intNumero)
        {
            int[] intPesos = { 2, 3, 4, 5, 6, 7, 8, 9, 2, 3, 4, 5, 6, 7, 8, 9 };
            string strText = intNumero.ToString();

            if (strText.Length > 16)
                throw new Exception("Número não suportado pela função!");

            int intSoma = 0;
            int intIdx = 0;
            for (int intPos = strText.Length - 1; intPos >= 0; intPos--)
            {
                intSoma += Convert.ToInt32(strText[intPos].ToString()) * intPesos[intIdx];
                intIdx++;
            }
            int intResto = (intSoma * 10) % 11;
            int intDigito = intResto;
            if (intDigito >= 10)
                intDigito = 0;

            return intDigito;
        }
        public string DVMod11_prova2(string number)
        {
            int sum = 0;
            for (int i = number.Length - 1, multiplier = 2; i >= 0; i--)
            {
                sum += (int)char.GetNumericValue(number[i]) * multiplier;
                if (++multiplier > 9) multiplier = 2;
            }
            int mod = (sum % 11);
            if (mod == 0 || mod == 1) return "0";
            return (11 - mod).ToString();
        }

        #region HELPERS 

        DateTime CToDateTime(String param6pos, Int32 hora, Int32 minunto, Int32 segundo, Boolean ddMMyy, bool ddMMyyyy = false)
        {
            Int32 dia;
            Int32 mes;
            Int32 ano;

            if (ddMMyyyy)
            {
                dia = Convert.ToInt32(param6pos.Substring(0, 2));
                mes = Convert.ToInt32(param6pos.Substring(2, 2));
                ano = Convert.ToInt32(param6pos.Substring(4, 4));

                DateTime data = new DateTime(ano, mes, dia, hora, minunto, segundo);
                return data;
            }
            else
            {
                if (ddMMyy)
                {
                    dia = Convert.ToInt32(param6pos.Substring(0, 2));
                    mes = Convert.ToInt32(param6pos.Substring(2, 2));
                    ano = Convert.ToInt32(param6pos.Substring(4, 2));
                }
                else
                {
                    ano = Convert.ToInt32(param6pos.Substring(0, 2));
                    mes = Convert.ToInt32(param6pos.Substring(2, 2));
                    dia = Convert.ToInt32(param6pos.Substring(4, 2));
                }

                if (ano >= 0 && ano <= 95)
                    ano = Convert.ToInt32("20" + ano.ToString());
                else
                    ano = Convert.ToInt32("19" + ano.ToString());

                DateTime data = new DateTime(ano, mes, dia, hora, minunto, segundo);
                return data;
            }
        }

        String CToString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return String.Empty;
            else
                return Convert.ToString(param).Replace("'", "");
        }
        Int32 CToInt32(Object param)
        {
            if (param == null || param == DBNull.Value)
                return 0;
            else
            {
                try
                {
                    return Convert.ToInt32(param);
                }
                catch
                {
                    return 0;
                }
            }
        }
        Decimal CToDecimal(Object param)
        {
            if (param == null || param == DBNull.Value)
                return 0;
            else
                return Convert.ToDecimal(param, new System.Globalization.CultureInfo("pt-Br"));
        }

        #endregion

        #region ARQUIVO DE RETORNO ///////////////////////////////////

        private void timer_Tick(Object sender, EventArgs e)
        {
            try
            {
                if (cmdIniciar.Enabled == false || busy) { return; }
                this.processaInputsRetorno();
            }
            catch
            {
                busy = false;
            }
        }

        /// <summary>
        /// Em desuso
        /// </summary>
        void processaInputsRetornoITAU(RetornoInput retorno)
        {
            if (busy) { return; }
            busy = true;

            if (retorno == null) { label1.Text = "Sem agenda - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"); busy = false; return; }

            pb.Value = 0;
            lblPerc.Text = "0%";
            cmdIniciar.Enabled = false;

            #region processamento

            #region variaveis

            Int32 titulosProcessados = 0, titulosBaixados = 0;

            PersistenceManager pm = null;

            int i = 0; String linhaAtual = null;
            String[] arrLinhas = retorno.Texto.Split(
                new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            lblMsg.Text = "";


            String codigoRegistro = null;
            CriticaRetornoVO vo = null;
            Cobranca cobranca = null;

            List<CriticaRetornoVO> vos = new List<CriticaRetornoVO>();
            List<String> idsCobrancas = new List<String>();
            Contrato contrato = null;
            IList<TabelaValor> tabela = null;
            Taxa taxa = null;
            EstipulanteTaxa taxaEstipulante = null;
            ContratoBeneficiario cbTitular = null;
            Operadora operadora = null;

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            TimeSpan tempoAtraso = TimeSpan.Zero;
            #endregion

            try
            {
                titulosProcessados = arrLinhas.Length - 2;
                titulosBaixados = 0;

                pm = new PersistenceManager();
                pm.IsoLevel = IsolationLevel.ReadUncommitted;
                pm.BeginTransactionContext();

                for (i = 0; i < arrLinhas.Length; i++)
                {
                    label1.Text = i.ToString();


                    linhaAtual = arrLinhas[i];

                    pb.Value = percentagemAtual(arrLinhas.Length, i);
                    lblPerc.Text = pb.Value.ToString() + "%";
                    Application.DoEvents();

                    //codigo registro
                    if (linhaAtual == null || linhaAtual.Trim() == "") { continue; }
                    codigoRegistro = linhaAtual.Substring(0, 1);
                    if (codigoRegistro == "0" || codigoRegistro == "9") { continue; } //se é cabecalho ou trailler, ignora

                    cobranca = new Cobranca();
                    vo = new CriticaRetornoVO();
                    vo.Valor = Convert.ToDecimal(linhaAtual.Substring(153, 12), cinfo) / 100;
                    vo.ValorPgto = Convert.ToDecimal(linhaAtual.Substring(254, 12), cinfo) / 100;

                    //NOSSO NÚMERO - é o ID da cobrança
                    vo.NossoNumero = linhaAtual.Substring(62, 8);

                    //////////// PARA TRATAR O ERRO INICIAL DO NOSSO NUMERO NO BOLETO MAIL ////////////////
                    ///////////////////////////////////////////////////////////////////////////////////////
                    if (Path.GetFileNameWithoutExtension(retorno.ArquivoNome).ToUpper().EndsWith("C"))
                    {
                        if (Convert.ToString(Convert.ToInt64(vo.NossoNumero)).Length < 7)
                        {
                            vo.Status = "Título não localizado";
                            vo.NaoLocalizado = true;
                            vos.Add(vo);
                            continue;
                        }
                    }
                    ///////////////////////////////////////////////////////////////////////////////////////

                    cobranca.ID = Convert.ToInt32(vo.NossoNumero);
                    try
                    {
                        pm.Load(cobranca);
                    }
                    catch
                    {
                        cobranca.ID = null;
                    }

                    #region tenta achar pelo unibanco

                    if (cobranca.ID == null)
                    {
                        //Tenta ler o nosso numero UNIBANCO
                        vo.NossoNumero = linhaAtual.Substring(85, 9);
                        cobranca.LeNossoNumeroUNIBANCO(vo.NossoNumero.PadLeft(15, '0'));
                        contrato = Contrato.CarregarParcialPorCodCobranca(cobranca.ContratoCodCobranca, pm);

                        if (contrato != null)
                        {
                            IList<Cobranca> _cobrancas = Cobranca.CarregarTodas(contrato.ID, true, pm);
                            if (_cobrancas != null)
                            {
                                Boolean achou = false;
                                foreach (Cobranca _cobranca in _cobrancas)
                                {
                                    if (vo.Valor == _cobranca.Valor && cobranca.Parcela == _cobranca.Parcela)
                                    {
                                        cobranca.ID = _cobranca.ID;
                                        cobranca.Carregar();
                                        achou = true; break;
                                    }
                                }

                                if (!achou)
                                {
                                    cobranca.ID = null;
                                }
                            }
                            else
                                cobranca.ID = null;
                        }
                        else
                            cobranca.ID = null;
                    }

                    if (cobranca.ID == null)
                    {
                        //Tenta ler o nosso numero UNIBANCO
                        vo.NossoNumero = linhaAtual.Substring(85, 9);
                        cobranca.LeNossoNumeroUNIBANCO(("1" + vo.NossoNumero).PadLeft(15, '0'));
                        contrato = Contrato.CarregarParcialPorCodCobranca(cobranca.ContratoCodCobranca, pm);

                        if (contrato != null)
                        {
                            IList<Cobranca> _cobrancas = Cobranca.CarregarTodas(contrato.ID, true, pm);
                            if (_cobrancas != null)
                            {
                                Boolean achou = false;
                                foreach (Cobranca _cobranca in _cobrancas)
                                {
                                    if (vo.Valor == _cobranca.Valor && cobranca.Parcela == _cobranca.Parcela)
                                    {
                                        cobranca.ID = _cobranca.ID;
                                        cobranca.Carregar();
                                        achou = true; break;
                                    }
                                }

                                if (!achou)
                                {
                                    cobranca.ID = null;
                                }
                            }
                            else
                                cobranca.ID = null;
                        }
                        else
                            cobranca.ID = null;
                    }

                    if (cobranca.ID == null)
                    {
                        //Tenta ler o nosso numero UNIBANCO
                        vo.NossoNumero = linhaAtual.Substring(85, 9);
                        cobranca.LeNossoNumeroUNIBANCO(("2" + vo.NossoNumero).PadLeft(15, '0'));
                        contrato = Contrato.CarregarParcialPorCodCobranca(cobranca.ContratoCodCobranca, pm);

                        if (contrato != null)
                        {
                            IList<Cobranca> _cobrancas = Cobranca.CarregarTodas(contrato.ID, true, pm);
                            if (_cobrancas != null)
                            {
                                Boolean achou = false;
                                foreach (Cobranca _cobranca in _cobrancas)
                                {
                                    if (vo.Valor == _cobranca.Valor && cobranca.Parcela == _cobranca.Parcela)
                                    {
                                        cobranca.ID = _cobranca.ID;
                                        cobranca.Carregar();
                                        achou = true; break;
                                    }
                                }

                                if (!achou)
                                {
                                    cobranca.ID = null;
                                }
                            }
                            else
                                cobranca.ID = null;
                        }
                        else
                            cobranca.ID = null;
                    }
                    #endregion

                    //só baixa normais, complementares e parcelamentos
                    if (cobranca.ID == null || (cobranca.Tipo != (int)Cobranca.eTipo.Parcelamento && cobranca.Tipo != (int)Cobranca.eTipo.Normal && cobranca.Tipo != (int)Cobranca.eTipo.Complementar))
                    {
                        vo.Status = "Título não localizado";
                        vo.NaoLocalizado = true;
                        vos.Add(vo);
                        continue;
                    }

                    contrato = Contrato.CarregarParcial(cobranca.PropostaID, pm);
                    vo.Parcela = cobranca.Parcela.ToString();
                    vo.CobrancaTipo = cobranca.Tipo.ToString();

                    if (contrato == null)
                    {
                        vo.Status = "Título não localizado";
                        vo.NaoLocalizado = true;
                        vos.Add(vo);
                        continue;
                    }

                    cobranca.ContratoCodCobranca = contrato.CodCobranca.ToString();

                    operadora = new Operadora(contrato.OperadoraID);
                    pm.Load(operadora);

                    vo.PropostaCodCobranca = cobranca.ContratoCodCobranca;

                    try
                    {
                        cbTitular = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);
                        if (cbTitular != null)
                        {
                            vo.TitularCPF = cbTitular.BeneficiarioCPF;
                            vo.TitularNome = cbTitular.BeneficiarioNome;
                        }
                    }
                    catch { }

                    if (contrato.Inativo || contrato.Cancelado)
                    { vo.PropostaInativa = true; vo.DataInativacaoCancelamento = contrato.DataCancelamento; }

                    vo.CobrancaID = Convert.ToString(cobranca.ID);
                    String strDataPgto = linhaAtual.Substring(110, 6);
                    vo.DataPgto = CToDateTime(strDataPgto, 0, 0, 0, true);
                    vo.DataVencto = cobranca.DataVencimento;
                    vo.OperadoraNome = operadora.Nome;
                    vo.PropostaNumero = contrato.Numero;
                    if (vo.OperadoraNome != null && vo.OperadoraNome.Length > 30) { vo.OperadoraNome = vo.OperadoraNome.Substring(0, 30); }

                    cobranca.DataPgto = vo.DataPgto;
                    cobranca.ValorPgto = vo.ValorPgto;

                    if (idsCobrancas.Contains(vo.CobrancaID)) { vo.EmDuplicidade = true; }
                    else { idsCobrancas.Add(vo.CobrancaID); }

                    if (cobranca == null) //cobranca nao localizada
                    {
                        if (String.IsNullOrEmpty(vo.Status))
                        { vo.NaoLocalizado = true; vo.Status = "Título não localizado"; }
                        vos.Add(vo); pm.Dispose(); continue;
                    }

                    if (cobranca.Pago)
                    {
                        vo.Status = "Título baixado";
                        vo.EmDuplicidade = true;
                        vo.NaoLocalizado = false;
                        vo.PagamentoRejeitado = false;
                        vos.Add(vo); continue;
                    }

                    tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cobranca.DataVencimento, pm);
                    if (tabela != null)
                    {
                        taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                    }
                    else
                    {
                        taxa = null;
                    }

                    Decimal valorBruto = vo.ValorPgto;
                    if (taxa != null && !taxa.Embutido) { valorBruto -= taxa.ValorEmbutido; }

                    //COBRANCA FOI PAGA
                    //calcula o valor pago
                    if (contrato.CobrarTaxaAssociativa && ((Cobranca.eTipo)cobranca.Tipo) == Cobranca.eTipo.Normal)
                    {
                        taxaEstipulante = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID);
                        if (taxaEstipulante != null)
                        {
                            //Removido por solicitação
                        }
                    }

                    if (vo.ValorPgto < valorBruto)
                    {
                        vo.ValorMenor = true;
                    }

                    cobranca.Pago = true;
                    cobranca.ValorPgto = valorBruto;
                    cobranca.DataBaixaAutomatica = DateTime.Now;
                    pm.Save(cobranca);

                    titulosBaixados++;
                    vo.Status = "Título baixado";
                    vos.Add(vo);

                }

                label1.Text = "Processou as cobranças";
                Application.DoEvents();

                retorno.Processado = true;
                retorno.Salvar();

                Application.DoEvents();

                RetornoOutput output = RetornoOutput.CarregarPorInputID(retorno.ID);
                if (output == null)
                {
                    output = new RetornoOutput();
                    output.Data = DateTime.Now;
                }

                output.InputID = retorno.ID;
                int total = arrLinhas.Length - 2; if (total < 0) { total = 0; }
                output.Descricao = string.Concat(retorno.ArquivoNome, " - Títulos: ", total.ToString(), " - Baixados: ", titulosBaixados.ToString());
                output.SerializedBusinessObject = "";
                output.SerializedValueObject = RetornoOutput.Serializar(vos);
                output.Salvar(vos);

                Application.DoEvents();

                pm.Commit();
                pm.Dispose();
            }
            catch (Exception ex)
            {
                try
                {
                    pm.Rollback();
                }
                catch { }
                throw ex;
            }
            finally
            {
                pm = null;
                busy = false;
            }

            #endregion

            cmdIniciar.Enabled = true;
            lblMsg.Text = "Concluído";
            pb.Value = 100;
            lblPerc.Text = "100%";
            busy = false;
        }

        private void cmdIniciar_Click(Object sender, EventArgs e)
        {
            if (busy) { MessageBox.Show("ocupado"); return; }
            this.processaInputsRetorno();
        }

        /// <summary>
        /// Método inicial
        /// </summary>
        void processaInputsRetorno()
        {
            if (busy) { return; }
            RetornoInput retorno = null;

            retorno = RetornoInput.CarregarPendente();

            //retorno = new RetornoInput();
            //retorno.ID = 108;
            //retorno.Carregar();

            if (retorno == null) { label1.Text = "Sem agenda - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"); return; }

            if (retorno.TipoBanco == (Int32)Cobranca.eTipoBanco.Unibanco) { /*processaInputsRetornoUnibanco2(retorno);*/ }
            else if (retorno.TipoBanco == (int)Cobranca.eTipoBanco.Itau)  { /*processaInputsRetornoITAU(retorno);*/ }
            else                                                          { processaInputsRetornoSANTANDER(retorno); }
            //else { processaInputsRetornoBB(retorno); }
        }

        /// <summary>
        /// Em desuso
        /// </summary>
        /// <param name="retorno"></param>
        void processaInputsRetornoUnibanco2(RetornoInput retorno)
        {
            if (busy) { return; }
            busy = true;

            if (retorno == null) { label1.Text = "Sem agenda - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"); busy = false; return; }

            pb.Value = 0;
            lblPerc.Text = "0%";
            cmdIniciar.Enabled = false;

            #region processamento

            #region variaveis

            Int32 titulosProcessados = 0, titulosBaixados = 0;

            PersistenceManager pm = null;

            int i = 0; String linhaAtual = null;
            String[] arrLinhas = retorno.Texto.Split(
                new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            lblMsg.Text = "";


            String codigoRegistro = null, codigoRejeicao = null;
            CriticaRetornoVO vo = null;
            Cobranca cobranca = null;

            List<CriticaRetornoVO> vos = new List<CriticaRetornoVO>();
            List<String> idsCobrancas = new List<String>();
            Contrato contrato = null;
            IList<TabelaValor> tabela = null;
            Taxa taxa = null;
            EstipulanteTaxa taxaEstipulante = null;
            ContratoBeneficiario cbTitular = null;
            Operadora operadora = null;
            Boolean dupla = false;

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            #endregion

            try
            {
                titulosProcessados = arrLinhas.Length - 2;
                titulosBaixados = 0;

                for (i = 0; i < arrLinhas.Length; i++)
                {
                    label1.Text = i.ToString();
                    pm = new PersistenceManager();
                    pm.BeginTransactionContext();

                    linhaAtual = arrLinhas[i];

                    pb.Value = percentagemAtual(arrLinhas.Length, i);
                    lblPerc.Text = pb.Value.ToString() + "%";
                    Application.DoEvents();

                    //codigo registro
                    if (linhaAtual == null || linhaAtual.Trim() == "") { pm.Rollback(); continue; }
                    codigoRegistro = linhaAtual.Substring(0, 1);
                    if (codigoRegistro == "0" || codigoRegistro == "9") { pm.Rollback(); continue; } //se é cabecalho ou trailler, ignora

                    cobranca = new Cobranca();
                    vo = new CriticaRetornoVO();
                    vo.Valor = Convert.ToDecimal(linhaAtual.Substring(153, 12), new System.Globalization.CultureInfo("pt-Br")) / 100;
                    vo.ValorPgto = Convert.ToDecimal(linhaAtual.Substring(254, 12), new System.Globalization.CultureInfo("pt-Br")) / 100;

                    // NOSSO NÚMERO - ITAU
                    vo.NossoNumero = linhaAtual.Substring(37, 14);

                    try
                    {
                        cobranca.ID = Convert.ToInt64(vo.NossoNumero);
                        pm.Load(cobranca);
                    }
                    catch
                    {
                        cobranca.ID = null;
                    }

                    //////////// PARA TRATAR O ERRO INICIAL DO NOSSO NUMERO NO BOLETO MAIL ////////////////
                    ///////////////////////////////////////////////////////////////////////////////////////
                    if (Path.GetFileNameWithoutExtension(retorno.ArquivoNome).ToUpper().EndsWith("C"))
                    {
                        if (Convert.ToString(Convert.ToInt64(vo.NossoNumero)).Length < 7)
                        {
                            vo.Status = "Título não localizado";
                            vo.NaoLocalizado = true;
                            vos.Add(vo);
                            pm.Rollback(); continue;
                        }
                    }
                    ///////////////////////////////////////////////////////////////////////////////////////

                    if (cobranca.ID != null && Convert.ToInt64(cobranca.ID) > 1000) // ITAU 
                    {
                        #region ITAU

                        //só baixa normais, complementares e parcelamentos
                        if (cobranca.ID == null || (cobranca.Tipo != (int)Cobranca.eTipo.Parcelamento && cobranca.Tipo != (int)Cobranca.eTipo.Normal && cobranca.Tipo != (int)Cobranca.eTipo.Complementar))
                        {
                            vo.Status = "Título não localizado";
                            vo.NaoLocalizado = true;
                            vos.Add(vo);
                            pm.Rollback(); continue;
                        }

                        contrato = Contrato.CarregarParcial(cobranca.PropostaID, pm);
                        vo.Parcela = cobranca.Parcela.ToString();
                        vo.CobrancaTipo = cobranca.Tipo.ToString();

                        if (contrato == null)
                        {
                            vo.Status = "Título não localizado";
                            vo.NaoLocalizado = true;
                            vos.Add(vo);
                            pm.Rollback(); continue;
                        }

                        cobranca.ContratoCodCobranca = contrato.CodCobranca.ToString();

                        operadora = new Operadora(contrato.OperadoraID);
                        pm.Load(operadora);

                        vo.PropostaCodCobranca = cobranca.ContratoCodCobranca;

                        try
                        {
                            cbTitular = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);
                            if (cbTitular != null)
                            {
                                vo.TitularCPF = cbTitular.BeneficiarioCPF;
                                vo.TitularNome = cbTitular.BeneficiarioNome;
                            }
                        }
                        catch { }

                        if (contrato.Inativo) { vo.PropostaInativa = true; }

                        vo.CobrancaID = Convert.ToString(cobranca.ID);
                        String strDataPgto = linhaAtual.Substring(110, 6);
                        vo.DataPgto = CToDateTime(strDataPgto, 0, 0, 0, true);
                        vo.DataVencto = cobranca.DataVencimento;
                        vo.OperadoraNome = operadora.Nome;
                        vo.PropostaNumero = contrato.Numero;
                        if (vo.OperadoraNome != null && vo.OperadoraNome.Length > 30) { vo.OperadoraNome = vo.OperadoraNome.Substring(0, 30); }

                        cobranca.DataPgto = vo.DataPgto;
                        cobranca.ValorPgto = vo.ValorPgto;

                        if (idsCobrancas.Contains(vo.CobrancaID)) { vo.EmDuplicidade = true; }
                        else { idsCobrancas.Add(vo.CobrancaID); }

                        if (cobranca == null) //cobranca nao localizada
                        {
                            if (String.IsNullOrEmpty(vo.Status))
                            { vo.NaoLocalizado = true; vo.Status = "Título não localizado"; }
                            vos.Add(vo); continue;
                        }

                        tabela = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, contrato.Admissao, cobranca.DataVencimento, pm);
                        if (tabela != null)
                        {
                            taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                        }
                        else
                        {
                            taxa = null;
                        }

                        Decimal valorBruto = vo.ValorPgto;
                        if (taxa != null && !taxa.Embutido) { valorBruto -= taxa.ValorEmbutido; }

                        //COBRANCA FOI PAGA
                        //calcula o valor pago
                        if (contrato.CobrarTaxaAssociativa && ((Cobranca.eTipo)cobranca.Tipo) == Cobranca.eTipo.Normal)
                        {
                            taxaEstipulante = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID);
                            if (taxaEstipulante != null)
                            {
                                //Removido por solicitacao
                            }
                        }

                        if (vo.ValorPgto < cobranca.Valor)
                        {
                            vo.ValorMenor = true;
                        }

                        cobranca.Pago = true;
                        cobranca.ValorPgto = valorBruto;
                        cobranca.DataBaixaAutomatica = DateTime.Now;
                        pm.Save(cobranca);

                        titulosBaixados++;
                        vo.Status = "Título baixado";
                        vos.Add(vo);

                        pm.Commit();

                        #endregion
                    }
                    else
                    {
                        #region UNIBANCO

                        cobranca = new Cobranca();
                        vo = new CriticaRetornoVO();

                        //DATA VENCIMENTO
                        String strDataVencimento = linhaAtual.Substring(146, 6);
                        vo.DataVencto = CToDateTime(strDataVencimento, 23, 59, 59, true);
                        vo.DataPgto = vo.DataVencto;

                        //NOSSO NÚMERO - identifica tipo de cobranca, codigo de cobrança e num da parcela
                        vo.NossoNumero = linhaAtual.Substring(37, 16);

                        vo.Valor = Convert.ToDecimal(linhaAtual.Substring(153, 12), new System.Globalization.CultureInfo("pt-Br")) / 100;
                        vo.ValorPgto = Convert.ToDecimal(linhaAtual.Substring(254, 12), new System.Globalization.CultureInfo("pt-Br")) / 100;

                        try
                        {
                            cobranca.LeNossoNumeroUNIBANCO(vo.NossoNumero);
                        }
                        catch { }

                        //só baixa normais e complementares
                        if (cobranca.ID == null || (cobranca.Tipo != (int)Cobranca.eTipo.Parcelamento && cobranca.Tipo != (int)Cobranca.eTipo.Normal && cobranca.Tipo != (int)Cobranca.eTipo.Complementar))
                        {
                            vo.Status = "Título não localizado";
                            vo.NaoLocalizado = true;
                            vos.Add(vo);
                            pm.Rollback(); continue;
                        }

                        if (cobranca.Tipo == (int)Cobranca.eTipo.Dupla)
                            dupla = true;
                        else
                            dupla = false;

                        vo.Parcela = cobranca.Parcela.ToString();
                        vo.PropostaCodCobranca = cobranca.ContratoCodCobranca;
                        vo.CobrancaTipo = cobranca.Tipo.ToString();

                        if (vo.PropostaCodCobranca != null && vo.PropostaCodCobranca != "0")
                            contrato = Contrato.CarregarParcialPorCodCobranca(vo.PropostaCodCobranca, pm);
                        else
                            contrato = null;

                        if (contrato == null)
                        {
                            vo.Status = "Título não localizado";
                            vo.NaoLocalizado = true;
                            vos.Add(vo);
                            pm.Rollback(); continue;
                        }
                        cobranca.PropostaID = contrato.ID;

                        cbTitular = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);
                        if (cbTitular != null)
                        {
                            vo.TitularCPF = cbTitular.BeneficiarioCPF;
                            vo.TitularNome = cbTitular.BeneficiarioNome;
                        }

                        if (contrato.Inativo) { vo.PropostaInativa = true; }

                        //valores do titulo
                        vo.Valor = Convert.ToDecimal(linhaAtual.Substring(153, 12), new System.Globalization.CultureInfo("pt-Br")) / 100;
                        vo.ValorPgto = Convert.ToDecimal(linhaAtual.Substring(254, 12), new System.Globalization.CultureInfo("pt-Br")) / 100;

                        cobranca = Cobranca.CarregarPor(cobranca.PropostaID, cobranca.Parcela, cobranca.Tipo, pm);

                        if (cobranca != null)
                        {
                            vo.PropostaNumero = cobranca.ContratoNumero;
                            if (vo.OperadoraNome != null && vo.OperadoraNome.Length > 30)
                                vo.OperadoraNome = vo.OperadoraNome.Substring(0, 30);
                            vo.OperadoraNome = cobranca.OperadoraNome;
                            vo.CobrancaID = Convert.ToString(cobranca.ID);
                            vo.PropostaNumero = contrato.Numero;
                            vo.DataVencto = cobranca.DataVencimento;

                            if (cobranca.Pago)
                            {
                                vo.Status = "Título baixado";
                                vo.EmDuplicidade = false;
                                vo.NaoLocalizado = false;
                                vo.PagamentoRejeitado = false;
                                titulosBaixados++;
                                vos.Add(vo); pm.Rollback(); continue;
                            }

                            cobranca.ValorPgto = vo.ValorPgto;


                            if (idsCobrancas.Contains(vo.CobrancaID)) { vo.EmDuplicidade = true; }
                            else { idsCobrancas.Add(vo.CobrancaID); }
                        }

                        //checa se foi rejeitado
                        codigoRejeicao = linhaAtual.Substring(378, 2);
                        if (codigoRejeicao != "00") // foi rejeitado
                        {
                            vo.CodigoRejeicao = linhaAtual.Substring(378, 2);
                            vo.Status = linhaAtual.Substring(378, 2) + " Rejeitado";
                            vo.PagamentoRejeitado = true;
                            vos.Add(vo);
                            continue;
                        }
                        else
                        {
                            //DATA PAGTO
                            String strDataPgto = linhaAtual.Substring(292, 6);
                            vo.DataPgto = CToDateTime(strDataPgto, 0, 0, 0, false);
                        }

                        if (cobranca == null) //cobranca nao localizada
                        {
                            if (String.IsNullOrEmpty(vo.Status)) { vo.NaoLocalizado = true; vo.Status = "Título não localizado"; }
                            vos.Add(vo); continue;
                        }
                        else
                        {
                            cobranca.DataPgto = vo.DataPgto;
                            cobranca.ValorPgto = vo.ValorPgto;

                            tabela = TabelaValor.CarregarTabelaAtual(contrato.ContratoADMID, pm);
                            if (tabela != null)
                            {
                                taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                            }
                            else
                            {
                                taxa = null;
                            }

                            Decimal valorBruto = vo.ValorPgto;
                            if (taxa != null && !taxa.Embutido) { valorBruto -= taxa.ValorEmbutido; }

                            //COBRANCA FOI PAGA
                            //calcula o valor pago
                            if (contrato.CobrarTaxaAssociativa && ((Cobranca.eTipo)cobranca.Tipo) == Cobranca.eTipo.Normal)
                            {
                                taxaEstipulante = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID);
                                if (taxaEstipulante != null)
                                {
                                    // Removido por solicitação
                                }
                            }

                            if (valorBruto < cobranca.Valor)
                            {
                                vo.ValorMenor = true;
                            }

                            cobranca.Pago = true;
                            if (!dupla)
                            {
                                cobranca.ValorPgto = valorBruto;
                                cobranca.DataBaixaAutomatica = DateTime.Now;
                                pm.Save(cobranca);
                            }
                            //se a cobranca é dupla e está paga, baixa tb a cobranca referencia
                            else
                            {
                                Cobranca cobrancaReferencia = Cobranca.CarregarPor(cobranca.PropostaID, (cobranca.Parcela - 1), ((int)Cobranca.eTipo.Normal), pm); //new Cobranca(cobranca.CobrancaRefID);
                                if (cobrancaReferencia != null)
                                {
                                    cobrancaReferencia.ValorPgto = cobrancaReferencia.Valor; //o valor pago menos o valor nominal da cobranca mais atual é o valor pago da cobranca mais antiga
                                    cobrancaReferencia.DataPgto = cobranca.DataPgto;
                                    cobrancaReferencia.Pago = true;
                                    pm.Save(cobrancaReferencia);
                                    cobranca.Valor = cobranca.Valor - cobrancaReferencia.Valor;
                                }

                                cobranca.ValorPgto = cobranca.Valor;
                                pm.Save(cobranca);
                            }

                            titulosBaixados++;
                            vo.Status = "Título baixado";
                            vo.DataVencto = cobranca.DataVencimento;
                            vos.Add(vo);
                        }

                        pm.Commit();

                        #endregion
                    }
                }

                label1.Text = "Processou as cobranças";
                Application.DoEvents();

                retorno.Processado = true;
                retorno.Salvar();

                Application.DoEvents();

                RetornoOutput output = RetornoOutput.CarregarPorInputID(retorno.ID);

                if (output == null)
                {
                    output = new RetornoOutput();
                    output.Data = DateTime.Now;
                }

                output.InputID = retorno.ID;
                int total = arrLinhas.Length - 2; if (total < 0) { total = 0; }
                output.Descricao = string.Concat(retorno.ArquivoNome, " - Títulos: ", total.ToString(), " - Baixados: ", titulosBaixados.ToString());
                output.SerializedBusinessObject = "";
                output.SerializedValueObject = RetornoOutput.Serializar(vos);
                output.Salvar(vos);

                Application.DoEvents();
            }
            catch (Exception ex)
            {
                try
                {
                    pm.Rollback();
                }
                catch { }
                label1.Text = ex.Message;
                return;
            }
            finally
            {
                pm = null;
                busy = false;
            }

            #endregion

            cmdIniciar.Enabled = true;
            lblMsg.Text = "Concluído";
            pb.Value = 100;
            lblPerc.Text = "100%";
            busy = false;
        }

        /// <summary>
        /// Em desuso
        /// </summary>
        void processaInputsRetornoBB(RetornoInput retorno)
        {
            if (busy) { return; }
            busy = true;

            if (retorno == null) { label1.Text = "Sem agenda - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"); busy = false; return; }

            pb.Value = 0;
            lblPerc.Text = "0%";
            cmdIniciar.Enabled = false;

            #region processamento

            #region variaveis

            Int32 titulosProcessados = 0, titulosBaixados = 0;

            PersistenceManager pm = null;

            int i = 0; String linhaAtual = null;
            String[] arrLinhas = retorno.Texto.Split(
                new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            lblMsg.Text = "";


            String codigoRegistro = null, codigoRejeicao = null;
            CriticaRetornoVO vo = null;
            Cobranca cobranca = null;

            List<CriticaRetornoVO> vos = new List<CriticaRetornoVO>();
            List<String> idsCobrancas = new List<String>();
            Contrato contrato = null;
            IList<TabelaValor> tabela = null;
            Taxa taxa = null;
            EstipulanteTaxa taxaEstipulante = null;
            ContratoBeneficiario cbTitular = null;
            Operadora operadora = null;
            Boolean dupla = false;

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            string aux = "";

            #endregion

            try
            {
                titulosProcessados = arrLinhas.Length - 2;
                titulosBaixados = 0;

                for (i = 0; i < arrLinhas.Length; i++)
                {
                    //System.Threading.Thread.Sleep(1000);

                    label1.Text = i.ToString();
                    pm = new PersistenceManager();
                    pm.BeginTransactionContext();

                    linhaAtual = arrLinhas[i];

                    //if (linhaAtual.IndexOf("ARIANA MAYARA VIANA DE SOUZA") == -1) continue;

                    pb.Value = percentagemAtual(arrLinhas.Length, i);
                    lblPerc.Text = pb.Value.ToString() + "%";
                    Application.DoEvents();

                    //codigo registro
                    if (linhaAtual == null || linhaAtual.Trim() == "") { pm.Rollback(); continue; }
                    codigoRegistro = CToInt32(linhaAtual.Substring(1, 1)).ToString();
                    if (codigoRegistro != "7") { pm.Rollback(); continue; } //se não é registro de retorno, não processa

                    if (linhaAtual.Substring(83, 3).Trim() != "LQB" && linhaAtual.Substring(83, 3).Trim() != "LQ")
                    {
                        pm.Rollback();
                        continue;
                    }

                    vo = new CriticaRetornoVO();

                    // NOSSO NÚMERO 

                    try
                    {
                        vo.NossoNumero = linhaAtual.Substring(3, 17);
                        cobranca = new Cobranca();
                        vo.Valor = Convert.ToDecimal(linhaAtual.Substring(141, 10), cinfo);
                        vo.TaxaBoleto = Convert.ToDecimal(linhaAtual.Substring(128, 7), cinfo);
                        vo.ValorPgto = Convert.ToDecimal(linhaAtual.Substring(96, 10), cinfo);
                        vo.DataPgto = CToDateTime(linhaAtual.Substring(63, 8), 0, 0, 0, false, true);

                        aux = linhaAtual.Substring(21, 13).Trim();
                    }
                    catch
                    {
                        pm.Rollback();
                        continue;
                    }

                    //pm.Rollback();
                    //continue;

                    try
                    {
                        cobranca = Cobranca.CarregarUnicaPorID(aux, pm); //cobranca = Cobranca.CarregarPorNossoNumero(vo.NossoNumero, pm);
                        if (cobranca == null) cobranca = new Cobranca();

                        cobranca.NossoNumero = vo.NossoNumero;
                    }
                    catch
                    {
                        cobranca.ID = null;
                    }

                    if (cobranca == null) { int jk = 0; }

                    if (cobranca.ID == null)
                    {
                        #region Não localizado 

                        vo.Status = "Título não localizado";
                        vo.NaoLocalizado = true;
                        vos.Add(vo);
                        pm.Rollback(); continue;

                        #endregion
                    }
                    else
                    {
                        //if (cobranca.Tipo == (int)Cobranca.eTipo.Dupla)
                        //    dupla = true;
                        //else
                        //    dupla = false;

                        vo.Parcela = cobranca.Parcela.ToString();
                        vo.PropostaCodCobranca = cobranca.ContratoCodCobranca;
                        vo.CobrancaTipo = cobranca.Tipo.ToString();

                        if (vo.PropostaCodCobranca != null && vo.PropostaCodCobranca != "0")
                            contrato = Contrato.CarregarParcialPorCodCobranca(vo.PropostaCodCobranca, pm);
                        else
                            contrato = null;

                        if (contrato == null)
                        {
                            vo.Status = "Título não localizado";
                            vo.NaoLocalizado = true;
                            vos.Add(vo);
                            pm.Rollback(); continue;
                        }
                        cobranca.PropostaID = contrato.ID;

                        cbTitular = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);
                        if (cbTitular != null)
                        {
                            vo.TitularCPF = cbTitular.BeneficiarioCPF;
                            vo.TitularNome = cbTitular.BeneficiarioNome;
                        }

                        if (contrato.Inativo) { vo.PropostaInativa = true; }

                        vo.PropostaNumero = cobranca.ContratoNumero;
                        if (cobranca.OperadoraNome != null && cobranca.OperadoraNome.Length > 30)
                            vo.OperadoraNome = cobranca.OperadoraNome.Substring(0, 30);
                        else
                            vo.OperadoraNome = cobranca.OperadoraNome;

                        vo.CobrancaID = Convert.ToString(cobranca.ID);
                        vo.PropostaNumero = contrato.Numero;
                        vo.DataVencto = cobranca.DataVencimento;

                        if (cobranca.Pago)
                        {
                            vo.Status = "Título baixado";
                            vo.EmDuplicidade = true; ////estava false ?!?
                            vo.NaoLocalizado = false;
                            vo.PagamentoRejeitado = false;
                            titulosBaixados++;
                            vos.Add(vo); pm.Rollback(); continue;
                        }

                        cobranca.ValorPgto = vo.ValorPgto + vo.TaxaBoleto;//vo.Valor + vo.TaxaBoleto;
                        cobranca.DataPgto = vo.DataPgto;
                        cobranca.Pago = true;

                        if (idsCobrancas.Contains(vo.CobrancaID)) { vo.EmDuplicidade = true; }
                        else { idsCobrancas.Add(vo.CobrancaID); }

                        pm.Save(cobranca);
                        titulosBaixados++;

                        vo.Status = "Título baixado";
                        vo.DataVencto = cobranca.DataVencimento;
                        vos.Add(vo);

                        pm.Commit();
                        //pm.Rollback();
                    }
                }

                label1.Text = "Processou as cobranças";
                Application.DoEvents();

                retorno.Processado = true;
                retorno.Salvar();

                Application.DoEvents();

                RetornoOutput output = RetornoOutput.CarregarPorInputID(retorno.ID);

                if (output == null)
                {
                    output = new RetornoOutput();
                    output.Data = DateTime.Now;
                }

                output.InputID = retorno.ID;
                int total = arrLinhas.Length - 2; if (total < 0) { total = 0; }
                output.Descricao = string.Concat(retorno.ArquivoNome, " - Títulos: ", total.ToString(), " - Baixados: ", titulosBaixados.ToString());
                output.SerializedBusinessObject = "";
                output.SerializedValueObject = RetornoOutput.Serializar(vos);
                output.Salvar(vos);

                Application.DoEvents();
            }
            catch (Exception ex)
            {
                try
                {
                    pm.Rollback();
                }
                catch { }
                label1.Text = ex.Message;
                return;
            }
            finally
            {
                pm = null;
                busy = false;
            }

            #endregion

            cmdIniciar.Enabled = true;
            lblMsg.Text = "Concluído";
            pb.Value = 100;
            lblPerc.Text = "100%";
            busy = false;
        }

        void processaInputsRetornoSANTANDER(RetornoInput retorno)
        {
            if (busy) { return; }
            busy = true;

            if (retorno == null) { label1.Text = "Sem agenda - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"); busy = false; return; }

            pb.Value = 0;
            lblPerc.Text = "0%";
            cmdIniciar.Enabled = false;

            #region processamento

            #region variaveis

            Int32 titulosProcessados = 0, titulosBaixados = 0, total = 0;

            PersistenceManager pm = null;

            int i = 0; String linhaAtual = null;
            String[] arrLinhas = retorno.Texto.Split(
                new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            lblMsg.Text = "";


            String codigoRegistro = null;
            string codigoMovimento = "";
            CriticaRetornoVO vo = null;
            Cobranca cobranca = null;

            List<CriticaRetornoVO> vos = new List<CriticaRetornoVO>();
            List<String> idsCobrancas = new List<String>();
            Contrato contrato = null;
            ContratoBeneficiario cbTitular = null;

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            string aux = "";

            #endregion

            try
            {
                titulosProcessados = arrLinhas.Length - 2;
                titulosBaixados = 0; 

                for (i = 0; i < arrLinhas.Length; i++)
                {
                    //System.Threading.Thread.Sleep(1000);

                    if (i == 0) continue;

                    label1.Text = i.ToString();
                    pm = new PersistenceManager();
                    pm.BeginTransactionContext();

                    linhaAtual = arrLinhas[i];

                    pb.Value = percentagemAtual(arrLinhas.Length, i);
                    lblPerc.Text = pb.Value.ToString() + "%";
                    Application.DoEvents();

                    //codigo registro
                    if (linhaAtual == null || linhaAtual.Trim() == "") { pm.Rollback(); continue; }

                    codigoRegistro = CToInt32(linhaAtual.Substring(7, 1)).ToString();
                    if (codigoRegistro != "3") { pm.Rollback(); continue; } //se não for detail, ignora

                    codigoRegistro = linhaAtual.Substring(13, 1).ToUpper();
                    if (codigoRegistro != "T") { pm.Rollback(); continue; } //se não for detail tipo T, ignora

                    total++;

                    vo = new CriticaRetornoVO();

                    try
                    {
                        vo.NossoNumero = linhaAtual.Substring(40, 13);
                        cobranca = new Cobranca();
                        vo.Valor = Convert.ToDecimal(linhaAtual.Substring(77, 15), cinfo) / 100M;
                        vo.TaxaBoleto = Convert.ToDecimal(linhaAtual.Substring(193, 15), cinfo) / 100M;

                        vo.ValorPgto = Convert.ToDecimal(arrLinhas[i+1].Substring(92, 15), cinfo) / 100M; //pag.9
                        vo.DataPgto = CToDateTime(arrLinhas[i+1].Substring(137, 8), 0, 0, 0, false, true);

                        aux = Convert.ToInt32(linhaAtual.Substring(54, 15).Trim()).ToString(); //ID - pag.8

                        //OU
                        //aux = Convert.ToInt32(linhaAtual.Substring(100, 25).Trim()).ToString(); //ID - pag.8
                    }
                    catch
                    {
                        pm.Rollback();
                        continue;
                    }

                    try
                    {
                        cobranca = Cobranca.CarregarUnicaPorID(aux, pm); //cobranca = Cobranca.CarregarPorNossoNumero(vo.NossoNumero, pm);
                        if (cobranca == null) cobranca = new Cobranca();

                        cobranca.NossoNumero = vo.NossoNumero;

                        //if (cobranca.ID != null && Convert.ToInt32(cobranca.ID) != 5855)
                        //{
                        //    //TODO: Denis remover ou comentar todo este item
                        //    pm.Rollback();
                        //    continue;
                        //}

                        ////todo: denis, comentar esse if e else
                        //if (Convert.ToString(cobranca.PropostaID) == "35015" && cobranca.DataVencimento.Month==1 && cobranca.DataVencimento.Year==2017)//if (Convert.ToString(cobranca.ID) == "3425") //if (Convert.ToString(cobranca.ID) == "3425")
                        //{
                        //    int temp2 = 0;
                        //}
                        //else
                        //{
                        //    pm.Rollback();
                        //    continue;
                        //}
                    }
                    catch
                    {
                        cobranca.ID = null;
                    }

                    if (cobranca == null) { int jk = 0; }

                    if (cobranca.ID == null)
                    {
                        #region Não localizado

                        vo.Status = "Título não localizado";
                        vo.NaoLocalizado = true;
                        vos.Add(vo);
                        pm.Rollback(); continue;

                        #endregion
                    }
                    else
                    {
                        //if (cobranca.Tipo == (int)Cobranca.eTipo.Dupla)
                        //    dupla = true;
                        //else
                        //    dupla = false;

                        vo.Parcela = cobranca.Parcela.ToString();
                        vo.PropostaCodCobranca = cobranca.ContratoCodCobranca;
                        vo.CobrancaTipo = cobranca.Tipo.ToString();

                        if (vo.PropostaCodCobranca != null && vo.PropostaCodCobranca != "0")
                            contrato = Contrato.CarregarParcialPorCodCobranca(vo.PropostaCodCobranca, pm);
                        else
                            contrato = null;

                        if (contrato == null)
                        {
                            vo.Status = "Título não localizado";
                            vo.NaoLocalizado = true;
                            vos.Add(vo);
                            pm.Rollback(); continue;
                        }
                        cobranca.PropostaID = contrato.ID;

                        cbTitular = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);
                        if (cbTitular != null)
                        {
                            vo.TitularCPF = cbTitular.BeneficiarioCPF;
                            vo.TitularNome = cbTitular.BeneficiarioNome;
                        }

                        if (contrato.Inativo) { vo.PropostaInativa = true; }

                        vo.PropostaNumero = cobranca.ContratoNumero;
                        if (cobranca.OperadoraNome != null && cobranca.OperadoraNome.Length > 30)
                            vo.OperadoraNome = cobranca.OperadoraNome.Substring(0, 30);
                        else
                            vo.OperadoraNome = cobranca.OperadoraNome;

                        vo.CobrancaID = Convert.ToString(cobranca.ID);
                        vo.PropostaNumero = contrato.Numero;
                        vo.DataVencto = cobranca.DataVencimento;

                        codigoMovimento = arrLinhas[i + 1].Substring(15, 2).Trim(); //////////////////////////////////////////

                        //TODO: denis descomentar abaixo
                        if (cobranca.Pago && cobranca.ValorPgto > 0)
                        {
                            vo.Status = "Título baixado";
                            vo.EmDuplicidade = true; ////estava false ?!?
                            vo.NaoLocalizado = false;
                            vo.PagamentoRejeitado = false;
                            titulosBaixados++;
                            vos.Add(vo); pm.Rollback(); continue;
                        }

                        cobranca.ValorPgto = vo.ValorPgto + vo.TaxaBoleto;//vo.Valor + vo.TaxaBoleto;

                        if(contrato.Legado) if (cobranca.ValorPgto == vo.TaxaBoleto) cobranca.ValorPgto = 0;

                        if (codigoMovimento == "06") //Liquidacao
                        {
                            cobranca.Pago = true;
                            vo.Status = "Título baixado";
                            cobranca.DataPgto = vo.DataPgto;
                        }
                        else if (codigoMovimento == "09") //Baixa
                        {
                            vo.Status = string.Concat("Baixa (", codigoMovimento, ")"); 

                            //string tmp = arrLinhas[i].Substring(208, 2).Trim();
                            //if (tmp != "") { int intj = 0; }

                            //string tmp2 = arrLinhas[i+1].Substring(153, 4).Trim();
                            //if (tmp2 != "") { int intk = 0; }

                            ////if (cobranca.Pago && contrato.Legado && cobranca.Valor > cobranca.ValorPgto)
                            ////{
                            ////    cobranca.Pago = false; cobranca.ValorPgto = 0; cobranca.DataPgto = DateTime.MinValue;
                            ////}
                        }
                        else if (codigoMovimento == "03") //rejeitado
                        {
                            if (!cobranca.Pago) cobranca.DataPgto = DateTime.MinValue;
                            vo.Status = string.Concat("Título rejeitado (", codigoMovimento, ")");
                            //if (cobranca.Pago && contrato.Legado)
                            //{
                            //    cobranca.Pago = false; cobranca.ValorPgto = 0; cobranca.DataPgto = DateTime.MinValue;
                            //}
                        }
                        else if (codigoMovimento == "02") //confirmado
                        {
                            if (!cobranca.Pago) cobranca.DataPgto = DateTime.MinValue;
                            vo.Status = string.Concat("Título confirmado (", codigoMovimento, ")");
                            //if (cobranca.Pago && contrato.Legado)
                            //{
                            //    cobranca.Pago = false; cobranca.ValorPgto = 0; cobranca.DataPgto = DateTime.MinValue;
                            //}
                        }
                        else
                        {
                            vo.Status = string.Concat("Ocorrencia (", codigoMovimento, ")");
                        }

                        if (idsCobrancas.Contains(vo.CobrancaID)) { vo.EmDuplicidade = true; }
                        else { idsCobrancas.Add(vo.CobrancaID); }

                        pm.Save(cobranca);
                        titulosBaixados++;

                        
                        vo.DataVencto = cobranca.DataVencimento;
                        vos.Add(vo);

                        pm.Commit();
                        //pm.Rollback();
                    }
                }

                label1.Text = "Processou as cobranças";
                Application.DoEvents();

                retorno.Processado = true;
                retorno.Salvar();

                Application.DoEvents();

                RetornoOutput output = RetornoOutput.CarregarPorInputID(retorno.ID);

                if (output == null)
                {
                    output = new RetornoOutput();
                    output.Data = DateTime.Now;
                }

                output.InputID = retorno.ID;
                //int total = arrLinhas.Length - 2; if (total < 0) { total = 0; }
                output.Descricao = string.Concat(retorno.ArquivoNome, " - Títulos: ", total.ToString(), " - Baixados: ", titulosBaixados.ToString()); //todo: denis, descomentar esta linha
                output.SerializedBusinessObject = "";
                output.SerializedValueObject = RetornoOutput.Serializar(vos); //todo: denis, descomentar esta linha
                output.Salvar(vos); 

                Application.DoEvents();
            }
            catch (Exception ex)
            {
                try
                {
                    pm.Rollback();
                }
                catch { }
                label1.Text = ex.Message;
                return;
            }
            finally
            {
                pm = null;
                busy = false;
            }

            #endregion

            cmdIniciar.Enabled = true;
            lblMsg.Text = "Concluído";
            pb.Value = 100;
            lblPerc.Text = "100%";
            busy = false;
        }

        #endregion

        int percentagemAtual(int total, int atual)
        {
            return (atual * 100) / total;
        }

        #region GERAR COBRANCA ///////////////////////////////////////

        IList<Cobranca> pegaCobrancasDaVez(ArquivoRemessaCriterio criterio, IList<Cobranca> cobrancas)
        {
            List<Cobranca> daVez = new List<Cobranca>();
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            String[] idsADM = criterio.ContratoAdmIDs.Split(',');

            Contrato contrato = null;
            foreach (Cobranca cobranca in cobrancas)
            {
                contrato = new Contrato(cobranca.PropostaID);
                pm.Load(contrato);

                foreach (String idADM in idsADM)
                {
                    if (idADM == Convert.ToString(contrato.ContratoADMID))
                    {
                        daVez.Add(cobranca);
                        break;
                    }
                }
            }

            pm.CloseSingleCommandInstance();
            pm.Dispose();
            return daVez;
        }
        Cobranca cobrancaCerta(IList<Cobranca> cobrancas)
        {
            foreach (Cobranca cob in cobrancas)
            {
                if (cob.DataVencimento.Month == 8 && cob.DataVencimento.Year == 2011)
                    return cob;
            }

            return null;
        }

        //---------------------------------------------------------------------------------------------//

        void upload(string arquivoLocal)
        {
            try
            {
                string ftpBase = ConfigurationManager.AppSettings["ftpRemessa"] + Path.GetFileName(arquivoLocal);

                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpBase);
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                ftpRequest.Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["ftpLogin"], ConfigurationManager.AppSettings["ftpSenha"]);

                StreamReader sourceStream = new StreamReader(arquivoLocal);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd()); //Encoding.GetEncoding("iso-8859-1").GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                ftpRequest.ContentLength = fileContents.Length;

                Stream requestStream = ftpRequest.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                ftpResponse.Close();
            }
            catch
            {
            }
        }

        void gerarNAOLegado()
        {
            if (busy) { return; }
            busy = true;

            IList<ArquivoRemessaAgendamento> lista = ArquivoRemessaAgendamento.CarregarTodos(true, DateTime.Now);

            //lista = ArquivoRemessaAgendamento.CarregarTodos(85); //todo: denis comentar esta linha
            if (lista == null || lista.Count == 0) { busy = false; lblMsgRemessa2.Text = "Sem agenda [novo formato]"; return; }

            cmdIniciarRemessa2.Enabled = false;
            pbRemessa2.Value = 0;
            lblPercRemessa2.Text = "0%";

            ArquivoRemessaCriterio crit = null;
            int i = 1;

            IList<Cobranca> cobrancas = null;
            pbRemessa2.Value = percentagemAtual(lista.Count, i);
            lblPercRemessa2.Text = pbRemessa2.Value.ToString() + "%";
            Application.DoEvents();

            //if (lista[0].VencimentoAte.Day == 7)
            //{
            //    lista[0].VencimentoAte = new DateTime(lista[0].VencimentoAte.Year, lista[0].VencimentoAte.Month, 10, 23, 59, 59, 995);
            //}

            if (lista[0].QtdBoletos > 1)
            {
                lista[0].VencimentoAte = new DateTime(
                    lista[0].VencimentoDe.AddMonths(lista[0].QtdBoletos - 1).Year,
                    lista[0].VencimentoDe.AddMonths(lista[0].QtdBoletos - 1).Month,
                    lista[0].VencimentoAte.Day, 23, 59, 59, 990);
            }

            foreach (var _i in lista) { _i.Grupo = "1"; }//deletar isso e arrumar 

            if (!String.IsNullOrEmpty(lista[0].Grupo) && lista[0].Legado == false)
            {
                String grupo = lista[0].Grupo;
                List<ArquivoRemessaAgendamento> segundaLista = new List<ArquivoRemessaAgendamento>();
                foreach (ArquivoRemessaAgendamento _item in lista)
                {
                    if (String.IsNullOrEmpty(_item.Grupo)) { _item.Grupo = "1"; /*continue;*/ }

                    //if (String.IsNullOrEmpty(grupo))
                    //{
                    //    busy = false;
                    //    cmdIniciarRemessa2.Enabled = true;
                    //    pbRemessa2.Value = 0;
                    //    lblMsgRemessa2.Text = "Agenda no formato antigo. Abortado."; return;
                    //}

                    if (_item.Grupo.Equals(grupo)) { segundaLista.Add(_item); }
                }

                lista.Clear();
                foreach (ArquivoRemessaAgendamento _item in segundaLista)
                {
                    lista.Add(_item);
                }
            }
            else
            {
                { busy = false; cmdIniciarRemessa2.Enabled = true; pbRemessa2.Value = 0; lblMsgRemessa2.Text = "Agenda no formato antigo. Abortado."; return; }
            }

            crit = new ArquivoRemessaCriterio(lista[0].CriterioID);
            crit.Carregar();
            lblMsgRemessa2.Text = "Gerando parcelas";
            this.Text = "Serviços - PS -- Gerando parcelas";
            Application.DoEvents();
            cobrancas = Cobranca.ProcessarCobrancasNormaisParaGerarRemessa((ArquivoRemessaCriterio.eTipoTaxa)crit.TipoTaxa, lista[0].VencimentoDe, lista[0].VencimentoAte, lista[0].VigenciaDe, lista[0].VigenciaAte, lista);
            //cobrancas = Cobranca.CarregarTodas_Optimized_temp(null);

            String fileName = "";
            if (cobrancas != null && cobrancas.Count > 0)
            {
                lblMsgRemessa2.Text = "Gerando arquivo";
                this.Text = "Serviços - PS -- Gerando arquivos";
                Application.DoEvents();
                IList<SumarioArquivoGeradoVO> vos = ArquivoCobrancaUnibanco.GeraDocumentoCobranca_SANTANDER(cobrancas, lista);

                if (vos != null)
                {
                    String path = System.Configuration.ConfigurationSettings.AppSettings["financialFilePath"].Replace("/", @"\");

                    fileName = vos[0].ArquivoNome;
                    foreach (SumarioArquivoGeradoVO vo in vos)
                    {
                        File.WriteAllText(String.Concat(path, fileName), vo.ArquivoConteudo, System.Text.Encoding.ASCII);
                        this.upload(String.Concat(path, fileName));
                    }
                }
            }

            foreach (ArquivoRemessaAgendamento ara in lista)
            {
                if (!string.IsNullOrEmpty(fileName))
                    ara.ArquivoNomeInstance = fileName;

                ara.Processado = true;
                ara.DataProcessado = DateTime.Now;
                ara.Salvar();

                i++;
            }

            cmdIniciarRemessa2.Enabled = true;
            lblMsgRemessa2.Text = "Concluído";
            pbRemessa2.Value = 100;
            lblPercRemessa2.Text = "100%";
            this.Text = "Serviços - PS";
            busy = false;
        }

        void gerarLegado()
        {
            if (busy) { return; }
            busy = true;

            IList<ArquivoRemessaAgendamento> lista = ArquivoRemessaAgendamento.CarregarTodosLEGADO(true, DateTime.Now);

            //lista = ArquivoRemessaAgendamento.CarregarTodos(44);

            if (lista == null || lista.Count == 0) { busy = false; lblMsgRemessa2.Text = "Sem agenda [novo formato]"; return; }

            cmdIniciarRemessa2.Enabled = false;
            pbRemessa2.Value = 0;
            lblPercRemessa2.Text = "0%";

            int i = 1;

            IList<Cobranca> cobrancas = null;
            pbRemessa2.Value = percentagemAtual(lista.Count, i);
            lblPercRemessa2.Text = pbRemessa2.Value.ToString() + "%";
            Application.DoEvents();

            lblMsgRemessa2.Text = "Gerando parcelas";
            this.Text = "Serviços - PS -- Gerando parcelas";
            Application.DoEvents();
            cobrancas = Cobranca.ProcessarCobrancasNormaisParaGerarRemessaLEGADO(ArquivoRemessaCriterio.eTipoTaxa.ComTaxa, lista[0].VencimentoDe, lista[0].VencimentoAte, lista[0].VigenciaDe, lista[0].VigenciaAte, lista[0]);

            string arquivo = "";
            if (cobrancas != null && cobrancas.Count > 0)
            {
                lblMsgRemessa2.Text = "Gerando arquivo";
                this.Text = "Serviços - PS -- Gerando arquivos";
                Application.DoEvents();
                //IList<SumarioArquivoGeradoVO> vos = ArquivoCobrancaUnibanco.GeraDocumentoCobranca_UNIBANCO2(cobrancas, lista);

                List<string> ids = new List<string>();
                foreach (var cob in cobrancas) { ids.Add(Convert.ToString(cob.ID)); }

                string conteudo = "", versao = ""; object obj = null;
                //ArquivoCobrancaUnibanco.GeraDocumentoCobranca_BB(ids, ref arquivo, ref conteudo, ref obj, ref versao, lista, null);
                ArquivoCobrancaUnibanco.GeraDocumentoCobranca_SANTANDER(ids, ref arquivo, ref conteudo, ref obj, ref versao, lista, null);

                if (!string.IsNullOrEmpty(conteudo))
                {
                    String path = ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                    File.WriteAllText(String.Concat(path, arquivo), conteudo, System.Text.Encoding.ASCII);
                    this.upload(String.Concat(path, arquivo));
                }

                //if (vos != null)
                //{
                //    String path = System.Configuration.ConfigurationSettings.AppSettings["financialFilePath"].Replace("/", @"\");

                //    fileName = vos[0].ArquivoNome;
                //    foreach (SumarioArquivoGeradoVO vo in vos)
                //    {
                //        File.WriteAllText(String.Concat(path, fileName), vo.ArquivoConteudo, System.Text.Encoding.ASCII);
                //    }
                //}
            }

            lista[0].ArquivoNomeInstance = arquivo;
            lista[0].Carteira = 101;
            lista[0].Banco = 353;

            lista[0].Processado = true;
            lista[0].DataProcessado = DateTime.Now;
            lista[0].Salvar();

            //foreach (ArquivoRemessaAgendamento ara in lista)
            //{
            //    if (!string.IsNullOrEmpty(fileName))
            //        ara.ArquivoNomeInstance = fileName;

            //    ara.Processado = true;
            //    ara.DataProcessado = DateTime.Now;
            //    ara.Salvar();

            //    i++;
            //}

            cmdIniciarRemessa2.Enabled = true;
            lblMsgRemessa2.Text = "Concluído";
            pbRemessa2.Value = 100;
            lblPercRemessa2.Text = "100%";
            this.Text = "Serviços - PS";
            busy = false;
        }

        /// <summary>
        /// V2
        /// </summary>
        void gerarArquivoDeRemessa__2()
        {
            this.gerarLegado();
            this.gerarNAOLegado();
        }

        /// <summary>
        /// primeira versao
        /// </summary>
        [Obsolete("Obsoleto",true)]
        void gerarArquivoDeRemessa()
        {
            if (busy) { return; }
            busy = true;

            IList<ArquivoRemessaAgendamento> lista = ArquivoRemessaAgendamento.CarregarTodos(true, DateTime.Now);
            if (lista == null || lista.Count == 0) { lblMsgRemessa.Text = "Sem agenda [antigo formato]"; busy = false; return; }

            cmdIniciarRemessa.Enabled = false;
            pbRemessa.Value = 0;
            lblPercRemessa.Text = "0%";

            ArquivoRemessaCriterio crit = null;
            int i = 1;

            IList<Cobranca> cobrancas = null;
            foreach (ArquivoRemessaAgendamento ara in lista)
            {
                if (ara.Grupo != null && ara.Grupo.Trim() != "") { continue; }

                pbRemessa.Value = percentagemAtual(lista.Count, i);
                lblPercRemessa.Text = pbRemessa.Value.ToString() + "%";
                Application.DoEvents();

                crit = new ArquivoRemessaCriterio(ara.CriterioID);
                crit.Carregar();
                if (crit.ID == null) { i++; continue; }

                //processa
                cobrancas = Cobranca.ProcessarCobrancasNormaisParaGerarRemessa(crit.ContratoAdmIDs, (ArquivoRemessaCriterio.eTipoTaxa)crit.TipoTaxa, ara.VencimentoDe, ara.VencimentoAte, ara.VigenciaDe, ara.VigenciaAte, (Cobranca.eCarteira)ara.Carteira);
                String fileName = "";

                if (cobrancas != null && cobrancas.Count > 0)
                {
                    IList<SumarioArquivoGeradoVO> vos = ArquivoCobrancaUnibanco.GeraDocumentoCobranca_UNIBANCO(cobrancas, ara);

                    if (vos != null)
                    {
                        String path = System.Configuration.ConfigurationSettings.AppSettings["financialFilePath"].Replace("/", @"\"); //@"C:\arquivos\"; //
                        if (ara.ArquivoNome.IndexOf('.') == -1)
                        {
                            ara.ArquivoNome += ".dat";
                        }
                        fileName = ara.ArquivoNome.Split('.')[0] + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "." + ara.ArquivoNome.Split('.')[1];
                        foreach (SumarioArquivoGeradoVO vo in vos)
                        {
                            File.WriteAllText(String.Concat(path, fileName), vo.ArquivoConteudo, System.Text.Encoding.ASCII);
                        }
                    }
                }

                ara.ArquivoNomeInstance = fileName;
                ara.Processado = true;
                ara.DataProcessado = DateTime.Now;
                ara.Salvar();

                i++;
                break;
            }

            cmdIniciarRemessa.Enabled = true;
            lblMsgRemessa.Text = "Concluído";
            pbRemessa.Value = 100;
            lblPercRemessa.Text = "100%";
            busy = false;
        }

        private void timerRemessa_Tick(Object sender, EventArgs e)
        {
            //this.cmdIniciarRemessa_Click(null, null);
        }
        private void cmdIniciarRemessa_Click(Object sender, EventArgs e)
        {
            //if (!cmdIniciarRemessa.Enabled || busy) { lblMsgRemessa.Text = "ocupado 1"; return; }
            //lblMsgRemessa.Text = "";
            ////this.gerarArquivoDeRemessa();
        } 

        private void cmdIniciarRemessa2_Click(Object sender, EventArgs e)
        {
            if (!cmdIniciarRemessa2.Enabled || busy) { lblMsgRemessa2.Text = "ocupado 2"; return; }
            lblMsgRemessa2.Text = "";

            try
            {
                this.gerarArquivoDeRemessa__2();
            }
            catch
            {
                busy = false;
            }
        }
        private void timerRemessa2_Tick(Object sender, EventArgs e)
        {
            cmdIniciarRemessa2_Click(null, null);
        }

        #endregion

        /************************************************************************************/

        #region RELATORIO FINANCEIRO//////////////////////////////////

        void cmdIniciarRelatorioFaturamento_Click(Object sender, EventArgs e)
        {
            if (busy) { return; }

            busy = true;

            lblRelatorio.Text = "Inicializando. Aguarde...";
            Application.DoEvents();

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            FinancialReportBase frb = FinancialReportBase.CarregarPendente(pm);
            if (frb == null) { busy = false; pm.CloseSingleCommandInstance(); pm.Dispose(); lblRelatorio.Text = "Concluído..."; return; }

            String filial, estipulante, operadora;
            DateTime de, ate;

            filial = frb.FilialIdArr; estipulante = frb.EstipulanteIdArr; operadora = frb.OperadoraIdArr;
            de = frb.VencimentoDe; ate = frb.VencimentoAte;

            String qry = String.Concat("SELECT contratoadm_id, operadora_nome,estipulante_descricao, contratoadm_descricao, plano_descricao, contrato_id,contrato_numero, contrato_tipoContratoId, usuario_id, usuario_nome, usuario_documento1, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato on contrato_id=cobranca_propostaid ",
                "       INNER JOIN contrato_beneficiario on contratobeneficiario_contratoId=contrato_id and contratobeneficiario_tipo=0 ",
                "       INNER JOIN beneficiario on contratobeneficiario_beneficiarioId= beneficiario_id and contratobeneficiario_tipo=0 ",
                "       INNER JOIN operadora on operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante on estipulante_id=contrato_estipulanteId ",
                "       INNER JOIN contratoadm on contratoadm_id=contrato_contratoadmid ",
                "       INNER JOIN plano on plano_id=contrato_planoId ",
                "       INNER JOIN usuario on usuario_id=contrato_donoId ",
                "   WHERE ",
                "       (cobranca_dataVencimento between '", de.ToString("yyyy-MM-dd 00:00:00.000"), "' and '", ate.ToString("yyyy-MM-dd 23:59:59:995"), "' OR cobranca_dataPagto between '", de.ToString("yyyy-MM-dd 00:00:00.000"), "' and '", ate.ToString("yyyy-MM-dd 23:59:59:995"), "')",
                "       AND estipulante_id IN (", estipulante, ")",
                "       AND operadora_id IN (", operadora, ")",
                "   ORDER BY operadora_nome, estipulante_descricao, cobranca_dataVencimento, contrato_numero");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];

            #region configura DataTable

            dt.Columns.Add("ID");
            dt.Columns.Add("VALOR_LIMPO");

            //VALOR NET
            dt.Columns.Add("REF_MES_VENCTO_BOLETADO");
            dt.Columns.Add("REF_MES_VENCTO_REATIVACAO");
            dt.Columns.Add("ANTECIPACAO");
            dt.Columns.Add("ATRASO");

            //JUROS MULTA
            dt.Columns.Add("JURO_MULTA_REF_MES_VECTO");
            dt.Columns.Add("JURO_MULTA_PAGTO_ATRASO");

            dt.Columns.Add("TAXA_ASSOC");
            dt.Columns.Add("TAXA_BANCO");
            dt.Columns.Add("OVER_PERC");
            dt.Columns.Add("OVER_VALOR");
            dt.Columns.Add("FIXO");

            dt.Columns.Add("AGENCIAMENTO");
            dt.Columns.Add("VITALICIO");

            #endregion

            //Decimal aux = 0;
            DateTime vencto, dataPagto;//////////////////////////////////////////////////////////////////////
            Boolean atrasado = false;
            int i = 0;
            ComissionamentoOperadora co = null;
            ComissionamentoOperadoraItem coitem = null;
            ComissionamentoOperadoraVitaliciedade covit = null;
            TipoContrato.TipoComissionamentoProdutorOuOperadora tipo = TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal;

            foreach (DataRow row in dt.Rows)
            {
                #region zerar
                row["REF_MES_VENCTO_BOLETADO"] = "0";
                row["REF_MES_VENCTO_REATIVACAO"] = "0";
                row["ANTECIPACAO"] = "0";
                row["ATRASO"] = "0";
                row["JURO_MULTA_PAGTO_ATRASO"] = "0";
                row["JURO_MULTA_REF_MES_VECTO"] = "0";
                row["VITALICIO"] = 0;
                row["AGENCIAMENTO"] = 0;
                row["TAXA_ASSOC"] = 0;
                row["TAXA_BANCO"] = 0;
                row["OVER_PERC"] = 0;
                row["OVER_VALOR"] = 0;
                row["FIXO"] = 0;
                row["JURO_MULTA_PAGTO_ATRASO"] = "0";
                row["JURO_MULTA_REF_MES_VECTO"] = "0";
                #endregion

                i++;

                lblRelatorio.Text = "Processando " + i.ToString() + " de " + dt.Rows.Count;
                Application.DoEvents();

                row["ID"] = i;
                atrasado = false;

                this.LimpaValorCobranca(row, pm);

                vencto = Convert.ToDateTime(row["cobranca_dataVencimento"]);
                vencto = new DateTime(vencto.Year, vencto.Month, vencto.Day, 23, 59, 59, 997);

                //RECEBIDOS 
                if (CToInt32(row["cobranca_pago"]) == 1)
                {
                    dataPagto = Convert.ToDateTime(row["cobranca_dataPagto"]);

                    //VALOR NET ///////////////
                    if (dataPagto.Month == vencto.Month && dataPagto.Year == vencto.Year) //NO MES REF
                    {
                        if (row["cobranca_arquivoUltimoEnvioId"] != null)
                        {
                            row["REF_MES_VENCTO_BOLETADO"] = CToDecimal(row["VALOR_LIMPO"]).ToString("N2");
                        }
                        else
                        {
                            row["REF_MES_VENCTO_REATIVACAO"] = CToDecimal(row["VALOR_LIMPO"]).ToString("N2");
                        }
                    }

                    if (dataPagto.Month < vencto.Month && (dataPagto.Year <= vencto.Year)) //ANTECIPACOES
                    {
                        row["ANTECIPACAO"] = CToDecimal(row["VALOR_LIMPO"]).ToString("N2");
                    }

                    if (dataPagto.Month > vencto.Month && (dataPagto.Year >= vencto.Year)) //ATRASO
                    {
                        atrasado = true;
                        row["ATRASO"] = CToDecimal(row["VALOR_LIMPO"]).ToString("N2");
                    }

                    //JUROS E MULTA ///////////////
                    if (dataPagto > vencto || CToInt32(row["cobranca_tipo"]) == 2)
                    {
                        this.CalculaValorMultaJuros(row, atrasado, vencto, dataPagto, CToInt32(row["cobranca_tipo"]) == 2, pm);
                    }
                }

                if (vencto <= frb.VencimentoAte && vencto >= frb.VencimentoDe && row["cobranca_arquivoUltimoEnvioId"] != null && row["cobranca_arquivoUltimoEnvioId"] != DBNull.Value)
                {
                    //RECEBIVEIS

                    //Comissionamento da operadora
                    co = ComissionamentoOperadora.CarregarAtualPorContratoId(row["contratoadm_id"], pm);
                    if (co == null) { continue; }
                    coitem = ComissionamentoOperadoraItem.Carregar(co.ID, Convert.ToInt32(row["cobranca_parcela"]), pm);

                    tipo = TipoContrato.UI.TraduzTipoContratoComissionamento(Convert.ToInt32(row["contrato_TipoContratoId"]));

                    if (coitem != null)
                    {
                        try
                        {
                            //Checa o tipo de contrato
                            if (tipo == TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal)
                                row["AGENCIAMENTO"] = ((100 * coitem.Percentual) / CToDecimal(row["VALOR_LIMPO"])).ToString("N2");
                            else if (tipo == TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa)
                                row["AGENCIAMENTO"] = ((100 * coitem.PercentualADM) / CToDecimal(row["VALOR_LIMPO"])).ToString("N2");
                            else if (tipo == TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia)
                                row["AGENCIAMENTO"] = ((100 * coitem.PercentualCompraCarencia) / CToDecimal(row["VALOR_LIMPO"])).ToString("N2");
                            else if (tipo == TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial)
                                row["AGENCIAMENTO"] = ((100 * coitem.PercentualEspecial) / CToDecimal(row["VALOR_LIMPO"])).ToString("N2");
                            else if (tipo == TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao)
                                row["AGENCIAMENTO"] = ((100 * coitem.PercentualMigracao) / CToDecimal(row["VALOR_LIMPO"])).ToString("N2");
                        }
                        catch
                        {
                            row["AGENCIAMENTO"] = 0;
                        }
                    }

                    covit = ComissionamentoOperadoraVitaliciedade.Carregar(co.ID, tipo, Convert.ToInt32(row["cobranca_parcela"]), pm);
                    if (covit != null)
                    {
                        try
                        {
                            row["VITALICIO"] = ((100 * covit.Percentual) / CToDecimal(row["VALOR_LIMPO"])).ToString("N2");
                        }
                        catch
                        {
                            row["VITALICIO"] = 0;
                        }
                    }
                }

                row["OVER_VALOR"] = CToDecimal(row["cobranca_valorPagto"]) - CToDecimal(row["VALOR_LIMPO"]) - CToDecimal(row["TAXA_BANCO"]) - CToDecimal(row["TAXA_ASSOC"]) - CToDecimal(row["JURO_MULTA_PAGTO_ATRASO"]) - CToDecimal(row["JURO_MULTA_REF_MES_VECTO"]);
                if (CToDecimal(row["OVER_VALOR"]) < 0) { row["OVER_VALOR"] = 0; }
            }

            this.SalvaRelatorio(dt, frb.ProcessarEm, frb.VencimentoDe, frb.VencimentoAte, pm);
            frb.Processado = true;
            pm.Save(frb);
            pm.CloseSingleCommandInstance();
            busy = false;
        }

        void LimpaValorCobranca(DataRow row, PersistenceManager pm)
        {
            Contrato contrato = new Contrato(row["contrato_id"]);
            pm.Load(contrato);

            //Decimal cobrancaValor = CToDecimal(row["cobranca_valor"]);
            Decimal cobrancaValor = TabelaValor.CalculaValorNET(contrato, pm);
            Decimal valorTaxaAssociativa = 0;
            row["TAXA_BANCO"] = 0;
            row["OVER_PERC"] = 0;
            row["OVER_VALOR"] = 0;
            row["FIXO"] = 0;

            //Tira taxa bancaria
            IList<TabelaValor> tabela = TabelaValor.CarregarTabelaAtual(row["contratoadm_id"], pm);
            if (tabela != null && tabela.Count > 0)
            {
                Taxa taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                tabela = null;

                if (taxa != null)
                {
                    row["OVER_PERC"] = taxa.Over.ToString("N2");
                    row["FIXO"] = taxa.Fixo.ToString("N2");

                    row["TAXA_BANCO"] = taxa.ValorEmbutido.ToString("N2");
                    taxa = null;
                }
            }

            //Taxa associativa
            if (contrato.CobrarTaxaAssociativa)
            {
                valorTaxaAssociativa = Contrato.CalculaValorDaTaxaAssociativa(contrato, -1, null, pm);
                row["TAXA_ASSOC"] = valorTaxaAssociativa.ToString("N2");
            }

            row["VALOR_LIMPO"] = cobrancaValor.ToString("N2");
        }

        void CalculaValorMultaJuros(DataRow row, Boolean atrasado, DateTime vencto, DateTime pagto, Boolean cobrancaDupla, PersistenceManager pm)
        {
            Decimal multa = CToDecimal(row["cobranca_valorPagto"]) - CToDecimal(row["cobranca_valor"]);

            if (cobrancaDupla)
            {
                //localiza a cobranca referencia
                Cobranca cobrancaRef = Cobranca.CarregarPor(row["contrato_id"], Convert.ToInt32(row["cobranca_parcela"]) - 1, (int)Cobranca.eTipo.Normal);
                if (cobrancaRef != null)
                {
                    multa -= cobrancaRef.Valor;
                    vencto = cobrancaRef.DataVencimento; //deve usar a data de vencimento da cobranca referencia como parametro
                }
            }

            if (multa > 0)
            {
                if (vencto.Month == pagto.Month && vencto.Year == pagto.Year)
                {
                    row["JURO_MULTA_PAGTO_ATRASO"] = "0";
                    row["JURO_MULTA_REF_MES_VECTO"] = multa.ToString("N2");
                }
                else
                {
                    row["JURO_MULTA_PAGTO_ATRASO"] = multa.ToString("N2");
                    row["JURO_MULTA_REF_MES_VECTO"] = "0";
                }
            }
            else
            {
                row["JURO_MULTA_PAGTO_ATRASO"] = "0";
                row["JURO_MULTA_REF_MES_VECTO"] = "0";
            }
        }

        void SalvaRelatorio(DataTable dt, DateTime processadoEm, DateTime venctoDe, DateTime venctoAte, PersistenceManager pm)
        {
            StringBuilder sb = new StringBuilder();

            int i = 0, j = 0;
            NonQueryHelper.Instance.ExecuteNonQuery("truncate table reportbase", pm);

            foreach (DataRow row in dt.Rows)
            {
                i++; j++;

                if (row["cobranca_valorNominal"] == DBNull.Value) { row["cobranca_valorNominal"] = 0; }

                lblRelatorio.Text = "Salvando " + i.ToString() + " de " + dt.Rows.Count;
                Application.DoEvents();

                #region SQL

                sb.Append(" INSERT INTO reportbase (ID,VALOR_LIMPO,REF_MES_VENCTO_BOLETADO,REF_MES_VENCTO_REATIVACAO,ANTECIPACAO,ATRASO,JURO_MULTA_REF_MES_VECTO,JURO_MULTA_PAGTO_ATRASO,TAXA_ASSOC,TAXA_BANCO,OVER_PERC,OVER_VALOR,AGENCIAMENTO,VITALICIO,PROCESSADO_EM,VENCIMENTO_DE,VENCIMENTO_ATE,operadora_nome,estipulante_descricao,contratoadm_descricao,plano_descricao,contrato_id,contrato_numero,usuario_id,usuario_nome,usuario_documento1,cobranca_id,cobranca_propostaId,cobranca_parcela,cobranca_arquivoUltimoEnvioId,cobranca_valor,cobranca_valorNominal,cobranca_dataVencimento,cobranca_dataVencimentoIsencaoJuro,cobranca_dataCriacao,cobranca_pago,cobranca_dataPagto,cobranca_valorPagto,cobranca_tipo,cobranca_cobrancaRefId,cobranca_comissaoPaga,cobranca_cancelada,cobranca_nossoNumero) VALUES (");
                sb.Append(row["ID"]); sb.Append(", ");

                sb.Append("'"); sb.Append(CToString(row["VALOR_LIMPO"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["REF_MES_VENCTO_BOLETADO"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["REF_MES_VENCTO_REATIVACAO"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["ANTECIPACAO"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["ATRASO"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["JURO_MULTA_REF_MES_VECTO"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["JURO_MULTA_PAGTO_ATRASO"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["TAXA_ASSOC"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["TAXA_BANCO"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["OVER_PERC"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["OVER_VALOR"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");

                sb.Append("'"); sb.Append(CToString(row["AGENCIAMENTO"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["VITALICIO"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");

                sb.Append("'"); sb.Append(processadoEm.ToString("yyyy-MM-dd hh:mm:ss")); sb.Append("', ");
                sb.Append("'"); sb.Append(venctoDe.ToString("yyyy-MM-dd hh:mm:ss")); sb.Append("', ");
                sb.Append("'"); sb.Append(venctoAte.ToString("yyyy-MM-dd hh:mm:ss")); sb.Append("', ");

                sb.Append("'"); sb.Append(CToString(row["operadora_nome"])); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["estipulante_descricao"])); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["contratoadm_descricao"])); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["plano_descricao"])); sb.Append("', ");
                sb.Append(row["contrato_id"]); sb.Append(", ");
                sb.Append("'"); sb.Append(CToString(row["contrato_numero"])); sb.Append("', ");
                sb.Append(row["usuario_id"]); sb.Append(", ");
                sb.Append("'"); sb.Append(CToString(row["usuario_nome"]).Replace("'", "")); sb.Append("', ");
                sb.Append("'"); sb.Append(row["usuario_documento1"]); sb.Append("', ");
                sb.Append(row["cobranca_id"]); sb.Append(", ");
                sb.Append(row["cobranca_propostaId"]); sb.Append(", ");
                sb.Append(row["cobranca_parcela"]); sb.Append(", ");

                if (row["cobranca_arquivoUltimoEnvioId"] != null && row["cobranca_arquivoUltimoEnvioId"] != DBNull.Value)
                    sb.Append(row["cobranca_arquivoUltimoEnvioId"]);
                else
                    sb.Append("NULL");
                sb.Append(", ");

                sb.Append("'"); sb.Append(CToString(row["cobranca_valor"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(CToString(row["cobranca_valorNominal"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                sb.Append("'"); sb.Append(Convert.ToDateTime(row["cobranca_dataVencimento"]).ToString("yyyy-MM-dd HH:mm:ss.fff")); sb.Append("', ");

                if (row["cobranca_dataVencimentoIsencaoJuro"] != null && row["cobranca_dataVencimentoIsencaoJuro"] != DBNull.Value)
                {
                    sb.Append("'"); sb.Append(Convert.ToDateTime(row["cobranca_dataVencimentoIsencaoJuro"]).ToString("yyyy-MM-dd HH:mm:ss.fff")); sb.Append("', ");
                }
                else
                {
                    sb.Append("NULL,");
                }

                sb.Append("'"); sb.Append(Convert.ToDateTime(row["cobranca_dataCriacao"]).ToString("yyyy-MM-dd HH:mm:ss.fff")); sb.Append("', ");
                sb.Append(CToInt32(row["cobranca_pago"])); sb.Append(", ");

                if (row["cobranca_dataPagto"] != null && row["cobranca_dataPagto"] != DBNull.Value)
                {
                    sb.Append("'"); sb.Append(Convert.ToDateTime(row["cobranca_dataPagto"]).ToString("yyyy-MM-dd HH:mm:ss.fff")); sb.Append("', ");
                }
                else
                {
                    sb.Append("NULL,");
                }

                if (row["cobranca_valorPagto"] != null && row["cobranca_valorPagto"] != DBNull.Value)
                {
                    sb.Append("'"); sb.Append(CToString(row["cobranca_valorPagto"]).Replace(".", "").Replace(",", ".")); sb.Append("', ");
                }
                else
                {
                    sb.Append("'0',");
                }

                sb.Append(row["cobranca_tipo"]); sb.Append(", ");

                if (row["cobranca_cobrancaRefId"] != null && row["cobranca_cobrancaRefId"] != DBNull.Value)
                {
                    sb.Append(row["cobranca_cobrancaRefId"]); sb.Append(", ");
                }
                else
                {
                    sb.Append("NULL,");
                }

                sb.Append(CToInt32(row["cobranca_comissaoPaga"])); sb.Append(", ");
                sb.Append(CToInt32(row["cobranca_cancelada"])); sb.Append(", ");

                if (row["cobranca_nossoNumero"] != null && row["cobranca_nossoNumero"] != DBNull.Value)
                {
                    sb.Append("'"); sb.Append(row["cobranca_nossoNumero"]); sb.Append("')");
                }
                else
                {
                    sb.Append("NULL); ");
                }
                #endregion

                if (j == 500)
                {
                    lblRelatorio.Text = "Acessando banco de dados... ";
                    Application.DoEvents();

                    NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
                    sb.Remove(0, sb.Length);
                    j = 0;
                }
            }

            if (sb.Length > 0)
            {
                NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
                sb.Remove(0, sb.Length);
            }

            lblRelatorio.Text = "Status: concluído!";
        }

        private void timerRelatorioFinanceiro_Tick(Object sender, EventArgs e)
        {
            if (busy) { return; }
            try
            {
                cmdIniciarRelatorioFaturamento_Click(null, null);
            }
            catch
            {
                busy = false;
            }
        }

        #endregion

        #region IMPORTACAO XML

        private void timerImportacao_Tick(Object sender, EventArgs e)
        {
            try
            {
                button2_Click(null, null);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Comentado
        /// </summary>
        public void Importacao()
        {
            //if (busy) { return; }
            //busy = true;

            //IList<ImportacaoProposta.ItemAgendamento> itens = ImportacaoProposta.CarregarPendentes();
            //if (itens == null || itens.Count == 0) { busy = false; lblImportacaoStatus.Text = "Parado"; return; }

            //lblImportacaoStatus.Text = "Importando...";
            //Application.DoEvents();

            //ImportacaoProposta.ItemAgendamento item = itens[0];

            //ImportacaoProposta ip = new ImportacaoProposta();
            //lblImportacaoStatus.Text = "";

            //try
            //{
            //    ip.Importar(item);
            //    item.Processado = true;
            //    item.ProcessadoData = DateTime.Now;
            //    item.Salvar();
            //}
            //catch (Exception ex)
            //{
            //    lblImportacaoStatus.Text = ex.Message + "-";
            //}

            //busy = false;
            //lblImportacaoStatus.Text += "Concluído: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm");
        }

        /// <summary>
        /// Comentado
        /// </summary>
        public void CorrigirAdicionaisRelacionadosErradamente()
        {
            //DataTable dt = LocatorHelper.Instance.ExecuteQuery("select contrato_id, contrato_operadoraId from contrato where contrato_obs LIKE '%importado%'", "resultset").Tables[0];

            //PersistenceManager pm = new PersistenceManager();
            //pm.UseSingleCommandInstance();

            //Adicional adicionalCorreto = null;
            //IList<AdicionalBeneficiario> adicionais = null;
            //IList<ContratoBeneficiario> beneficiarios = null;
            //foreach (DataRow row in dt.Rows)
            //{
            //    beneficiarios = ContratoBeneficiario.CarregarPorContratoID(row["contrato_id"], false, false, pm);

            //    foreach (ContratoBeneficiario cb in beneficiarios)
            //    {
            //        adicionais = AdicionalBeneficiario.Carregar(cb.ContratoID, cb.BeneficiarioID, pm);

            //        if (adicionais == null) { continue; }

            //        foreach (AdicionalBeneficiario ab in adicionais)
            //        {
            //            if (ab.AdicionalOperadoraID == null) { continue; }

            //            if (Convert.ToString(row["contrato_operadoraId"]) != Convert.ToString(ab.AdicionalOperadoraID))
            //            {
            //                adicionalCorreto = new Adicional();
            //                adicionalCorreto.ID = Adicional.CarregarIDPorCodigoTitular(ab.AdicionalCodTitular, row["contrato_operadoraId"], pm);
            //                if (adicionalCorreto.ID != null)
            //                {
            //                    ab.AdicionalID = adicionalCorreto.ID;
            //                    pm.Save(ab);
            //                }
            //            }
            //        }
            //    }
            //}

            //pm.CloseSingleCommandInstance();
            //pm.Dispose();
        }

        private void button2_Click(Object sender, EventArgs e)
        {
            this.Importacao();
        }

        #endregion

        #region COMISSIONAMENTO////////////////////////////////////////////////////////////////////

        class CobrancaCorretorChefiaVO
        {
            String _cobrancaId;
            ArrayList _produtorIds;

            public String CobrancaID
            {
                get { return _cobrancaId; }
                set { _cobrancaId = value; }
            }
            public ArrayList ProdutorIDs
            {
                get { if (_produtorIds == null) { _produtorIds = new ArrayList(); } return _produtorIds; }
                set { _produtorIds = value; }
            }

            public CobrancaCorretorChefiaVO() { }
        }

        class ProdutorVO
        {
            public ProdutorVO(object id, ProdutorVO.Tipo tipo)
            {
                ProdutorId = Convert.ToString(id); PTipo = tipo;
            }

            public enum Tipo
            {
                Corretor,
                Plataforma
            }

            public string ProdutorId { get; set; }
            public ProdutorVO.Tipo PTipo { get; set; }

        }

        Boolean ExisteProdutorVO(List<ProdutorVO> vos, Object produtorId, ProdutorVO.Tipo tipo)
        {
            foreach (ProdutorVO vo in vos)
            {
                if (vo.ProdutorId == Convert.ToString(produtorId) &&
                    vo.PTipo == tipo)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Comentado
        /// </summary>
        void logErroComissao(Object listagemId, Object filialId, Object cobrancaId, Object produtorId, Object tabelaComissionamentoId, Object grupoComissionamentoId, String msg, PersistenceManager pm)
        {
            //String _tabelaComissionamentoId = "NULL";
            //String _grupoComissionamentoId = "NULL";

            //if (grupoComissionamentoId != null) { _grupoComissionamentoId = Convert.ToString(grupoComissionamentoId); }
            //if (tabelaComissionamentoId != null) { _tabelaComissionamentoId = Convert.ToString(tabelaComissionamentoId); }

            //NonQueryHelper.Instance.ExecuteNonQuery(String.Concat("INSERT INTO comissaoLog (listagemId,filialId,cobrancaId,produtorId,tabelaComissionamentoId,grupoComissionamentoId,msg) VALUES (",
            //    listagemId, ",", filialId, ",", cobrancaId, ",", produtorId, ",", _tabelaComissionamentoId, ",",
            //    _grupoComissionamentoId, ",'", msg, "');"), pm);
        }

        /// <summary>
        /// Comentado
        /// </summary>
        public DataTable CalculaFechamento(String nomeListagem, String mensagemListagem, String[] filialIDs, String[] operadoraIDs, String[] perfilIDs, DateTime? dataCorte, Object produtorId, String[] contratoAdmIDs, String competencia)
        {
            return null;
            //if (busy) { return null; }

            //busy = true;

            //cmdIniciarListagem.Enabled = false;
            //pbListagem.Value = 0;
            //lblPercListagem.Text = "0%";

            //#region variáveis

            //Object grupoId = null;
            //List<CobrancaCorretorChefiaVO> vos = new List<CobrancaCorretorChefiaVO>();
            //DataTable dt = null, resultado = new DataTable();
            //Usuario corretor = null;
            //ComissionamentoUsuario comU = null;
            //DateTime? vigencia = null, admissao = null;
            //IList<ComissionamentoItem> itensComissionamento = null;
            //IList<ExcecaoItem> itensTabelaExcecao = null;
            //ComissionamentoVitaliciedade itemVitaliciedade = null;
            //TabelaExcecaoVitaliciedade itemVitaliciedadeExcecao = null;
            //IList<SuperiorSubordinado> superiores = null;
            //System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            //Int32 tipoContrato = -1, parcela = -1;
            //Decimal valorComissao = 0, creditos = 0, percentualComissao = 0, valorPago = 0, valorCobrado = 0;
            //List<MovimentacaoContaCorrente> itensCC = null;
            //MovimentacaoContaCorrente itemCC = null;
            //TipoContrato.TipoComissionamentoProdutorOuOperadora tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal;
            //UsuarioFilial usuarioFilial = null;
            //Comissionamento comissionamentoModelo = null;
            //Contrato contrato = null;
            //IList<TabelaValor> tabelasValor = null;
            //Taxa taxa = null;

            //#endregion

            //#region configura DataTable resultado

            //resultado.Columns.Add("CobrancaID");
            //resultado.Columns.Add("CobrancaVencimento");
            //resultado.Columns.Add("ContratoNumero");
            //resultado.Columns.Add("ContratoID");
            //resultado.Columns.Add("ContratoVigencia");
            //resultado.Columns.Add("OperadoraID");
            //resultado.Columns.Add("OperadoraNome");
            //resultado.Columns.Add("CobrancaValorPago");
            //resultado.Columns.Add("CobrancaDataPago");
            //resultado.Columns.Add("CobrancaParcela");
            //resultado.Columns.Add("ProdutorID");
            //resultado.Columns.Add("ProdutorNome");
            //resultado.Columns.Add("ProdutorValor");
            //resultado.Columns.Add("ProdutorPercentualComissao");
            //resultado.Columns.Add("ProdutorCredito");
            //resultado.Columns.Add("ProdutorPerfilID");
            //resultado.Columns.Add("ProdutorApelido");
            //resultado.Columns.Add("SuperiorApelido");
            //resultado.Columns.Add("NomeTitular");
            //resultado.Columns.Add("ContratoAdmissao");
            //resultado.Columns.Add("FecharComissao");

            //resultado.Columns.Add("ProdutorBanco");
            //resultado.Columns.Add("ProdutorAgencia");
            //resultado.Columns.Add("ProdutorConta");

            //resultado.Columns.Add("ContaCorrenteID");
            //resultado.Columns.Add("ContaCorrenteMotivo");

            //#endregion

            //#region configura parâmetros da query

            //String filiaisParam = "", operadorasParam = "", perfisParam = "", contratosAdmParam = "", dataParam = "";
            //if (filialIDs != null && filialIDs.Length > 0)
            //{
            //    filiaisParam = String.Concat(" AND usuariofilial_filialId IN (", String.Join(",", filialIDs), ") "); //String.Concat(" AND almox_produto_filialId IN (", String.Join(",", filialIDs), ") "); //filiaisParam = String.Concat(" AND usuariofilial_data <= contrato_admissao AND usuario_filialId IN (", String.Join(",", filialIDs), ") ");
            //}

            //if (operadoraIDs != null && operadoraIDs.Length > 0)
            //    operadorasParam = String.Concat(" AND contrato_operadoraId IN (", String.Join(",", operadoraIDs), ") ");

            //if (perfilIDs != null && perfilIDs.Length > 0)
            //    perfisParam = String.Join(",", perfilIDs);

            //if (contratoAdmIDs != null && contratoAdmIDs.Length > 0)
            //    contratosAdmParam = String.Concat(" AND contrato_contratoAdmId IN (", String.Join(",", contratoAdmIDs), ") ");

            //if (dataCorte != null)
            //{
            //    //período
            //    dataParam = String.Concat(" AND cobranca_dataPagto <= '", dataCorte.Value.ToString("yyyy-MM-dd 23:59:59.996"), "' and cobranca_dataPagto >= '", dataCorte.Value.AddMonths(-2).ToString("yyyy-MM-dd"), "' "); //2011, 07, 21

            //    //dataParam = " AND cobranca_dataPagto BETWEEN '2012-07-17' and '2012-08-15 23:59:59.998' ";
            //}

            //#endregion

            //String qry = String.Concat(" cobranca.*, beneficiario_nome, contrato_cobrarTaxaAssociativa, contrato_donoId, contrato_numero,contrato_filialId, contrato_contratoAdmId, contrato_operadorTmktId, contrato_admissao, contrato_vigencia, contrato_tipoContratoId, operadora_id, operadora_nome ",
            //    "   FROM cobranca ",
            //    "       INNER JOIN contrato                ON cobranca_propostaId=contrato_id ",
            //    "       INNER JOIN operadora               ON operadora_id=contrato_operadoraId ",
            //    "       INNER JOIN contrato_beneficiario   ON contratobeneficiario_contratoId=contrato_id AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
            //    "       INNER JOIN beneficiario            ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
            //    "       INNER JOIN usuario                 ON usuario_id=contrato_donoId ",
            //    "       INNER JOIN usuario_filial          ON usuario_id=usuariofilial_usuarioId ",
            //    "   WHERE cobranca_parcela <= 5 and  ", //,contrato_operadorTmktId = 33165 and
            //    "     ( (cobranca_comissaoPaga=0 OR cobranca_comissaoPaga IS NULL) AND cobranca_pago=1 AND cobranca_parcela > 1 ",
            //    filiaisParam, operadorasParam, contratosAdmParam, dataParam, ") ");

            //PersistenceManager pm = new PersistenceManager();
            //pm.IsoLevel = IsolationLevel.ReadCommitted;
            //pm.BeginTransactionContext();

            //try
            //{
            //    dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];
            //    if (dt.Rows.Count == 0) { pm.Commit(); dt.Dispose(); return null; }

            //    #region ARMAZENA todos os corretores em uma colecao de ids de corretor, sem duplicar.

            //    //System.Collections.ArrayList array = new System.Collections.ArrayList();
            //    List<ProdutorVO> array = new List<ProdutorVO>();
            //    foreach (DataRow row in dt.Rows)
            //    {
            //        //if (!array.Contains(Convert.ToString(row["contrato_donoId"])))
            //        //{
            //        //    array.Add(Convert.ToString(row["contrato_donoId"]));
            //        //}
            //        if (row["contrato_operadorTmktId"] == DBNull.Value)
            //        {
            //            if (!ExisteProdutorVO(array, row["contrato_donoId"], ProdutorVO.Tipo.Corretor))
            //            {
            //                array.Add(new ProdutorVO(row["contrato_donoId"], ProdutorVO.Tipo.Corretor));
            //            }
            //        }
            //        else
            //        {
            //            if (!ExisteProdutorVO(array, row["contrato_operadorTmktId"], ProdutorVO.Tipo.Plataforma))
            //            {
            //                array.Add(new ProdutorVO(row["contrato_operadorTmktId"], ProdutorVO.Tipo.Plataforma));
            //            }
            //        }
            //    }
            //    #endregion

            //    IList<TabelaExcecao> tabelasExcecao = null;

            //    #region Salva a listagem ------------------------------------------------------------

            //    Listagem listagem = new Listagem();
            //    listagem.Nome = nomeListagem;
            //    listagem.Mensagem = mensagemListagem;
            //    if (dataCorte != null) { listagem.DataCorte = dataCorte.Value; }
            //    pm.Save(listagem);

            //    #region filiais

            //    if (filialIDs != null && filialIDs.Length > 0)
            //    {
            //        Listagem.Filial lf;
            //        foreach (String filialID in filialIDs)
            //        {
            //            lf = new Listagem.Filial();
            //            lf.ListagemID = listagem.ID;
            //            lf.FilialID = filialID;
            //            pm.Save(lf);
            //        }
            //    }
            //    #endregion

            //    #region operadoras

            //    if (operadoraIDs != null && operadoraIDs.Length > 0)
            //    {
            //        Listagem.Operadora lo;
            //        foreach (String operadoraID in operadoraIDs)
            //        {
            //            lo = new Listagem.Operadora();
            //            lo.ListagemID = listagem.ID;
            //            lo.OperadoraID = operadoraID;
            //            pm.Save(lo);
            //        }
            //    }
            //    #endregion

            //    #region perfis

            //    if (perfilIDs != null && perfilIDs.Length > 0)
            //    {
            //        Listagem.Perfil lp;
            //        foreach (String perfilID in perfilIDs)
            //        {
            //            lp = new Listagem.Perfil();
            //            lp.ListagemID = listagem.ID;
            //            lp.PerfilID = perfilID;
            //            pm.Save(lp);
            //        }
            //    }
            //    #endregion

            //    #endregion

            //    //passeia por todos os corretores
            //    DataRow[] rows;
            //    DataRow resultadoLinha;
            //    Int32 count = 0;
            //    foreach (ProdutorVO vo in array) //foreach (Object corretorId in array)
            //    {
            //        count++;

            //        pbListagem.Value = percentagemAtual(array.Count, count);
            //        lblPercListagem.Text = pbListagem.Value.ToString() + "%";
            //        Application.DoEvents();

            //        //percorre os contratos do corretor corrente
            //        if (vo.PTipo == ProdutorVO.Tipo.Corretor)
            //        {
            //            rows = dt.Select("contrato_operadorTmktId is null and contrato_donoId=" + vo.ProdutorId);
            //            if (rows.Length == 0) { continue; }
            //            corretor = new Usuario(rows[0]["contrato_donoId"]);
            //        }
            //        else
            //        {
            //            if (vo.ProdutorId == "0") { continue; }
            //            rows = dt.Select("contrato_operadorTmktId=" + vo.ProdutorId);
            //            if (rows.Length == 0) { continue; }
            //            corretor = new Usuario(rows[0]["contrato_operadorTmktId"]);
            //        }
            //        pm.Load(corretor);

            //        foreach (DataRow row in rows)
            //        {
            //            if (row["contrato_vigencia"] == DBNull.Value) { continue; }
            //            valorComissao = 0;
            //            vigencia = Convert.ToDateTime(row["contrato_vigencia"], cinfo);
            //            admissao = Convert.ToDateTime(row["contrato_admissao"], cinfo);
            //            tabelasExcecao = null;

            //            if (vo.PTipo == ProdutorVO.Tipo.Corretor)
            //                usuarioFilial = UsuarioFilial.CarregarVigente(row["contrato_donoId"], vigencia.Value, pm);
            //            else
            //            {
            //                usuarioFilial = new UsuarioFilial();
            //                usuarioFilial.FilialID = row["contrato_filialId"];
            //            }

            //            if (usuarioFilial == null || !contemValor(filialIDs, usuarioFilial.FilialID))
            //            {
            //                logErroComissao(listagem.ID, filialIDs[0], row["cobranca_id"], vo.ProdutorId, null, null, "Produtor/Plataforma não encontrado(a) para essa filial", pm);
            //                continue;
            //            }

            //            ////Corretor
            //            //if (vo.PTipo == ProdutorVO.Tipo.Corretor)
            //            //    corretor = new Usuario(row["contrato_donoId"]);
            //            //else
            //            //    corretor = new Usuario(row["contrato_operadorTmktId"]);
            //            //pm.Load(corretor);

            //            comU = ComissionamentoUsuario.CarregarVigente(corretor.ID, vigencia, pm);

            //            if (comU != null && comU.TabelaComissionamentoID != null)
            //                tabelasExcecao = TabelaExcecao.Carregar(comU.TabelaComissionamentoID, corretor.ID, row["contrato_contratoAdmId"], vigencia.Value, pm);

            //            if (tabelasExcecao != null)
            //            {
            //                #region Tabela de Exceção

            //                resultadoLinha = resultado.NewRow();
            //                resultadoLinha["ContaCorrenteID"] = null;
            //                resultadoLinha["ContaCorrenteMotivo"] = "";

            //                #region preenche a linha

            //                resultadoLinha["CobrancaID"] = row["cobranca_id"];
            //                resultadoLinha["CobrancaVencimento"] = Convert.ToDateTime(row["cobranca_dataVencimento"], cinfo).ToString("dd/MM/yyyy");

            //                valorCobrado = Convert.ToDecimal(row["cobranca_valor"], cinfo);
            //                valorPago = valorCobrado;

            //                resultadoLinha["CobrancaValorPago"] = valorPago.ToString("N2");
            //                resultadoLinha["CobrancaDataPago"] = Convert.ToDateTime(row["cobranca_dataPagto"], cinfo).ToString("dd/MM/yyyy");
            //                resultadoLinha["ContratoNumero"] = row["contrato_numero"];
            //                resultadoLinha["ContratoID"] = row["cobranca_propostaId"];
            //                resultadoLinha["ContratoVigencia"] = Convert.ToDateTime(row["contrato_vigencia"], cinfo).ToString("dd/MM/yyyy");
            //                resultadoLinha["CobrancaParcela"] = row["cobranca_parcela"];
            //                resultadoLinha["OperadoraID"] = row["operadora_id"];
            //                resultadoLinha["OperadoraNome"] = row["operadora_nome"];
            //                resultadoLinha["ProdutorID"] = row["contrato_donoId"];
            //                resultadoLinha["ProdutorPerfilID"] = corretor.PerfilID;
            //                resultadoLinha["ProdutorApelido"] = corretor.Apelido;
            //                resultadoLinha["NomeTitular"] = row["beneficiario_nome"];
            //                resultadoLinha["ContratoAdmissao"] = Convert.ToDateTime(row["contrato_admissao"], cinfo).ToString("dd/MM/yyyy");

            //                resultadoLinha["ProdutorBanco"] = corretor.Banco;
            //                resultadoLinha["ProdutorAgencia"] = corretor.Agencia;
            //                resultadoLinha["ProdutorConta"] = corretor.Conta;

            //                resultadoLinha["FecharComissao"] = "0";
            //                #endregion

            //                #region Comissionamento - Tabela de Excecao

            //                percentualComissao = 0;
            //                itensTabelaExcecao = ExcecaoItem.CarregarPorTabelaExcecaoID(tabelasExcecao[0].ID, pm);

            //                if (itensTabelaExcecao == null)
            //                {
            //                    logErroComissao(listagem.ID, filialIDs[0], row["cobranca_id"], vo.ProdutorId, comissionamentoModelo.ID, null, "Não foi encontrado parcelas comissionáveis na tabela de exceção.", pm);
            //                    continue;
            //                }

            //                parcela = Convert.ToInt32(row["cobranca_parcela"]);
            //                tipoContrato = Convert.ToInt32(row["contrato_tipoContratoId"]);
            //                foreach (ExcecaoItem item in itensTabelaExcecao)
            //                {
            //                    if (parcela == item.Parcela)
            //                    {
            //                        if (tipoContrato == 1) //normal
            //                        {
            //                            tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal;
            //                            valorComissao = valorPago * (item.Percentual / 100);
            //                            percentualComissao = item.Percentual;
            //                            break;
            //                        }
            //                        else if (tipoContrato == 4) //carencia
            //                        {
            //                            tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia;
            //                            valorComissao = valorPago * (item.PercentualCompraCarencia / 100);
            //                            percentualComissao = item.PercentualCompraCarencia;
            //                            break;
            //                        }
            //                        else if (tipoContrato == 3) //migracao
            //                        {
            //                            tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao;
            //                            valorComissao = valorPago * (item.PercentualMigracao / 100);
            //                            percentualComissao = item.PercentualMigracao;
            //                            break;
            //                        }
            //                        else if (tipoContrato == 2) //adm
            //                        {
            //                            tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa;
            //                            valorComissao = valorPago * (item.PercentualADM / 100);
            //                            percentualComissao = item.PercentualADM;
            //                            break;
            //                        }
            //                        else if (tipoContrato == 5) //especial
            //                        {
            //                            tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial;
            //                            valorComissao = valorPago * (item.PercentualEspecial / 100);
            //                            percentualComissao = item.PercentualEspecial;
            //                            break;
            //                        }
            //                        else if (tipoContrato == 6) //idade
            //                        {
            //                            tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade;
            //                            valorComissao = valorPago * (item.Idade / 100);
            //                            percentualComissao = item.Idade;
            //                            break;
            //                        }
            //                    }
            //                }
            //                #endregion

            //                #region Vitaliciedade - COMO EXIBIR NO RELATORIO ESTE PERCENTUAL DE COMISSAO ???

            //                itemVitaliciedadeExcecao = TabelaExcecaoVitaliciedade.Carregar(tabelasExcecao[0].ID, tipoComissionamento, pm);
            //                if (itemVitaliciedadeExcecao != null && itemVitaliciedadeExcecao.ParcelaInicio <= parcela && itemVitaliciedadeExcecao.Vitalicia && itemVitaliciedadeExcecao.TipoColunaComissao == tipoContrato)
            //                {
            //                    valorComissao += (valorPago * (itemVitaliciedadeExcecao.Percentual / 100));
            //                }
            //                #endregion

            //                resultadoLinha["ProdutorNome"] = corretor.Nome;
            //                resultadoLinha["ProdutorValor"] = valorComissao.ToString("N2");
            //                resultadoLinha["ProdutorPercentualComissao"] = percentualComissao.ToString("N2");

            //                //se nao foi informado um produtor especifico ou 
            //                //foi informado e é o mesmo que o dono do contrato (corretor que vendeu)
            //                if (produtorId == null ||
            //                    Convert.ToString(produtorId) == Convert.ToString(corretor.ID))
            //                {
            //                    //checa também se o perfil está incluso na relacao de perfis enviada por parametro
            //                    if (perfilIDs == null || contemValor(perfilIDs, corretor.PerfilID))
            //                    {
            //                        resultadoLinha["FecharComissao"] = "1";

            //                        #region salva a comissao na c/c e checa itens abertos na conta corrente

            //                        //só salva o credito se ele ainda nao foi feito (COBRANÇA).
            //                        if (valorComissao > 0 && !MovimentacaoContaCorrente.CreditoJaFeitoPara(corretor.ID, row["cobranca_id"], pm)) //itemCC != null && itemCC.Valor > 0)
            //                        {
            //                            itemCC = new MovimentacaoContaCorrente();
            //                            itemCC.CategoriaID = CategoriaContaCorrente.SysPremiacaoCategoriaID;
            //                            itemCC.CobrancaID = row["cobranca_id"];
            //                            itemCC.Data = DateTime.Now;
            //                            itemCC.LisagemFechamentoID = listagem.ID;
            //                            itemCC.ProdutorID = corretor.ID;
            //                            itemCC.Valor = valorComissao;
            //                            pm.Save(itemCC);
            //                        }

            //                        creditos = 0;
            //                        itensCC = (List<MovimentacaoContaCorrente>)MovimentacaoContaCorrente.CarregarEmAberto(corretor.ID, dataCorte.Value, pm);
            //                        if (itensCC == null) { itensCC = new List<MovimentacaoContaCorrente>(); }
            //                        foreach (MovimentacaoContaCorrente _item in itensCC)
            //                        {
            //                            _item.LisagemFechamentoID = listagem.ID;
            //                            pm.Save(_item);
            //                        }
            //                        #endregion
            //                    }
            //                }

            //                resultadoLinha["ProdutorCredito"] = "0,00";
            //                resultado.Rows.Add(resultadoLinha);
            //                adicionaGrafoDeProdutoresParaCobranca(ref vos, row["cobranca_id"], corretor.ID, false);

            //                if (valorComissao > 0)
            //                {
            //                    salvarGrafoEquipeCorretor(corretor.ID, corretor.PerfilID, listagem.ID, pm);
            //                }
            //                else
            //                {
            //                    logErroComissao(listagem.ID, filialIDs[0], row["cobranca_id"], vo.ProdutorId, comissionamentoModelo.ID, grupoId, "Sem configuração de parcela comissionável (tabela de exceção) para parcela " + parcela.ToString(), pm);
            //                }

            //                #endregion Tabela de Exceção
            //            }
            //            else
            //            {
            //                #region Tabela de comisssionamento

            //                if (comU != null)
            //                {
            //                    resultadoLinha = resultado.NewRow();
            //                    resultadoLinha["ContaCorrenteID"] = null;
            //                    resultadoLinha["ContaCorrenteMotivo"] = "";

            //                    #region preenche a linha

            //                    resultadoLinha["CobrancaID"] = row["cobranca_id"];
            //                    resultadoLinha["CobrancaVencimento"] = Convert.ToDateTime(row["cobranca_dataVencimento"], cinfo).ToString("dd/MM/yyyy");

            //                    valorCobrado = Convert.ToDecimal(row["cobranca_valor"], cinfo);
            //                    //valorPago = Convert.ToDecimal(row["cobranca_valorPagto"], cinfo);
            //                    //if (valorPago > Convert.ToDecimal(row["cobranca_valor"], cinfo))
            //                    //{
            //                    //    valorPago = Convert.ToDecimal(row["cobranca_valor"], cinfo);
            //                    //}
            //                    valorPago = valorCobrado;

            //                    resultadoLinha["CobrancaValorPago"] = valorPago.ToString("N2");
            //                    resultadoLinha["CobrancaDataPago"] = Convert.ToDateTime(row["cobranca_dataPagto"], cinfo).ToString("dd/MM/yyyy");
            //                    resultadoLinha["ContratoNumero"] = row["contrato_numero"];
            //                    resultadoLinha["ContratoID"] = row["cobranca_propostaId"];
            //                    resultadoLinha["ContratoVigencia"] = Convert.ToDateTime(row["contrato_vigencia"], cinfo).ToString("dd/MM/yyyy");
            //                    resultadoLinha["CobrancaParcela"] = row["cobranca_parcela"];
            //                    resultadoLinha["OperadoraID"] = row["operadora_id"];
            //                    resultadoLinha["OperadoraNome"] = row["operadora_nome"];

            //                    //if(vo.PTipo == ProdutorVO.Tipo.Corretor)
            //                    //    resultadoLinha["ProdutorID"] = row["contrato_donoId"];
            //                    //else
            //                    //    resultadoLinha["ProdutorID"] = row["contrato_operadorTmktId"];
            //                    resultadoLinha["ProdutorID"] = corretor.ID;

            //                    resultadoLinha["ProdutorPerfilID"] = corretor.PerfilID;
            //                    resultadoLinha["ProdutorApelido"] = corretor.Apelido;
            //                    resultadoLinha["NomeTitular"] = row["beneficiario_nome"];
            //                    resultadoLinha["ContratoAdmissao"] = Convert.ToDateTime(row["contrato_admissao"], cinfo).ToString("dd/MM/yyyy");

            //                    resultadoLinha["ProdutorBanco"] = corretor.Banco;
            //                    resultadoLinha["ProdutorAgencia"] = corretor.Agencia;
            //                    resultadoLinha["ProdutorConta"] = corretor.Conta;

            //                    resultadoLinha["FecharComissao"] = "0";
            //                    #endregion

            //                    #region Comissionamento

            //                    percentualComissao = 0;
            //                    comissionamentoModelo = new Comissionamento(comU.TabelaComissionamentoID);
            //                    pm.Load(comissionamentoModelo);

            //                    grupoId = ComissionamentoGrupo.ObterID(comissionamentoModelo.ID, row["contrato_contratoAdmId"], pm);
            //                    if (grupoId != null)
            //                    {
            //                        itensComissionamento = ComissionamentoItem.Carregar(grupoId, pm);
            //                    }
            //                    else
            //                    {
            //                        //if (temp.Length > 0) { temp.Append(","); }
            //                        //temp.Append(row["cobranca_propostaId"]);
            //                        logErroComissao(listagem.ID, filialIDs[0], row["cobranca_id"], vo.ProdutorId, comissionamentoModelo.ID, null, "Não foi encontrado um grupo para esta tabela de comissionamento, com este contrato adm", pm);
            //                        continue;
            //                    }

            //                    if (itensComissionamento != null)
            //                    {
            //                        parcela = Convert.ToInt32(row["cobranca_parcela"]);
            //                        tipoContrato = Convert.ToInt32(row["contrato_tipoContratoId"]);
            //                        foreach (ComissionamentoItem item in itensComissionamento)
            //                        {
            //                            if (parcela == item.Parcela)
            //                            {
            //                                if (tipoContrato == 1) //normal
            //                                {
            //                                    tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal;
            //                                    valorComissao = valorPago * (item.Percentual / 100);
            //                                    percentualComissao = item.Percentual;
            //                                    break;
            //                                }
            //                                else if (tipoContrato == 4) //carencia
            //                                {
            //                                    tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia;
            //                                    valorComissao = valorPago * (item.PercentualCompraCarencia / 100);
            //                                    percentualComissao = item.PercentualCompraCarencia;
            //                                    break;
            //                                }
            //                                else if (tipoContrato == 3) //migracao
            //                                {
            //                                    tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao;
            //                                    valorComissao = valorPago * (item.PercentualMigracao / 100);
            //                                    percentualComissao = item.PercentualMigracao;
            //                                    break;
            //                                }
            //                                else if (tipoContrato == 2) //adm
            //                                {
            //                                    tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa;
            //                                    valorComissao = valorPago * (item.PercentualADM / 100);
            //                                    percentualComissao = item.PercentualADM;
            //                                    break;
            //                                }
            //                                else if (tipoContrato == 5) //especial
            //                                {
            //                                    tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial;
            //                                    valorComissao = valorPago * (item.PercentualEspecial / 100);
            //                                    percentualComissao = item.PercentualEspecial;
            //                                    break;
            //                                }
            //                                else if (tipoContrato == 6) //idade
            //                                {
            //                                    tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade;
            //                                    valorComissao = valorPago * (item.Idade / 100);
            //                                    percentualComissao = item.Idade;
            //                                    break;
            //                                }
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        logErroComissao(listagem.ID, filialIDs[0], row["cobranca_id"], vo.ProdutorId, comissionamentoModelo.ID, grupoId, "Não foram encontradas parcelas comissionáveis para esta tabela de comissionamento", pm);
            //                    }
            //                    #endregion

            //                    #region Vitaliciedade - COMO EXIBIR NO RELATORIO ESTE PERCENTUAL DE COMISSAO ???

            //                    itemVitaliciedade = ComissionamentoVitaliciedade.Carregar(grupoId, tipoComissionamento, pm);
            //                    if (itemVitaliciedade != null && itemVitaliciedade.ParcelaInicio >= parcela)
            //                    {
            //                        valorComissao += (valorPago * (itemVitaliciedade.Percentual / 100));
            //                    }
            //                    #endregion

            //                    resultadoLinha["ProdutorNome"] = corretor.Nome;
            //                    resultadoLinha["ProdutorValor"] = valorComissao.ToString("N2");
            //                    resultadoLinha["ProdutorPercentualComissao"] = percentualComissao.ToString("N2");

            //                    //se nao foi informado um produtor especifico ou 
            //                    //foi informado e é o mesmo que o dono do contrato (corretor que vendeu)
            //                    if (produtorId == null ||
            //                        Convert.ToString(produtorId) == Convert.ToString(corretor.ID))
            //                    {
            //                        //checa também se o perfil está incluso na relacao de perfis enviada por parametro
            //                        if (perfilIDs == null || contemValor(perfilIDs, corretor.PerfilID))
            //                        {
            //                            resultadoLinha["FecharComissao"] = "1";

            //                            #region itens abertos na conta corrente

            //                            //só salva o credito se ele ainda nao foi feito (COBRANÇA).
            //                            if (valorComissao > 0 && !MovimentacaoContaCorrente.CreditoJaFeitoPara(corretor.ID, row["cobranca_id"], pm)) //itemCC != null && itemCC.Valor > 0)
            //                            {
            //                                itemCC = new MovimentacaoContaCorrente();
            //                                itemCC.CategoriaID = CategoriaContaCorrente.SysPremiacaoCategoriaID;
            //                                itemCC.CobrancaID = row["cobranca_id"];
            //                                itemCC.Data = DateTime.Now;
            //                                itemCC.LisagemFechamentoID = listagem.ID;
            //                                itemCC.ProdutorID = corretor.ID;
            //                                itemCC.Valor = valorComissao;
            //                                pm.Save(itemCC);
            //                            }

            //                            creditos = 0;
            //                            itensCC = (List<MovimentacaoContaCorrente>)MovimentacaoContaCorrente.CarregarEmAberto(corretor.ID, dataCorte.Value, pm);
            //                            if (itensCC == null) { itensCC = new List<MovimentacaoContaCorrente>(); }
            //                            foreach (MovimentacaoContaCorrente _item in itensCC)
            //                            {
            //                                //if (CategoriaContaCorrente.eTipo.Credito == (CategoriaContaCorrente.eTipo)_item.CategoriaTipo)
            //                                //    creditos += _item.Valor;
            //                                //else
            //                                //    creditos -= _item.Valor;

            //                                _item.LisagemFechamentoID = listagem.ID;
            //                                pm.Save(_item);
            //                            }
            //                            #endregion
            //                        }
            //                    }

            //                    resultadoLinha["ProdutorCredito"] = "0,00"; //creditos.ToString("N2");
            //                    resultado.Rows.Add(resultadoLinha);
            //                    adicionaGrafoDeProdutoresParaCobranca(ref vos, row["cobranca_id"], corretor.ID, false);

            //                    if (valorComissao > 0)
            //                    {
            //                        salvarGrafoEquipeCorretor(corretor.ID, corretor.PerfilID, listagem.ID, pm);
            //                    }
            //                    else
            //                    {
            //                        logErroComissao(listagem.ID, filialIDs[0], row["cobranca_id"], vo.ProdutorId, comissionamentoModelo.ID, grupoId, "Sem configuração de parcela comissionável para parcela " + parcela.ToString(), pm);
            //                    }
            //                }
            //                else
            //                {
            //                    if (comissionamentoModelo != null)
            //                        logErroComissao(listagem.ID, filialIDs[0], row["cobranca_id"], vo.ProdutorId, comissionamentoModelo.ID, null, "Produtor/Plataforma sem tabela de comissionamento", pm);
            //                    else
            //                        logErroComissao(listagem.ID, filialIDs[0], row["cobranca_id"], vo.ProdutorId, null, null, "Produtor/Plataforma sem tabela de comissionamento", pm);
            //                }
            //                #endregion Tabela de comisssionamento
            //            }

            //            ////Superiores /////////////////////////////////////////////////////////////
            //            superiores = SuperiorSubordinado.CarregarSuperiores(corretor.ID, admissao, pm);
            //            List<SuperiorSubordinado> lista = new List<SuperiorSubordinado>();
            //            //lista.Find(delegate(SuperiorSubordinado ss) { return ss.SubordinadoNome == ""; });

            //            while (superiores != null && superiores.Count > 0)
            //            {
            //                valorComissao = 0;
            //                comU = ComissionamentoUsuario.CarregarVigente(superiores[0].SuperiorID, vigencia, pm);

            //                if (comU == null) { superiores = SuperiorSubordinado.CarregarSuperiores(superiores[0].SuperiorID, admissao, pm); continue; }
            //                comissionamentoModelo = new Comissionamento(comU.TabelaComissionamentoID);
            //                pm.Load(comissionamentoModelo);

            //                salvarGrafoEquipe(superiores[0], listagem.ID, pm);

            //                if (comU != null)
            //                {
            //                    resultadoLinha = resultado.NewRow();

            //                    #region preenche a linha

            //                    resultadoLinha["CobrancaID"] = row["cobranca_id"];
            //                    resultadoLinha["CobrancaVencimento"] = Convert.ToDateTime(row["cobranca_dataVencimento"], cinfo).ToString("dd/MM/yyyy");
            //                    resultadoLinha["CobrancaValorPago"] = valorPago; //Convert.ToDecimal(row["cobranca_valorPagto"], cinfo).ToString("N2");
            //                    resultadoLinha["CobrancaDataPago"] = Convert.ToDateTime(row["cobranca_dataPagto"], cinfo).ToString("dd/MM/yyyy");
            //                    resultadoLinha["CobrancaParcela"] = row["cobranca_parcela"];
            //                    resultadoLinha["ContratoNumero"] = row["contrato_numero"];
            //                    resultadoLinha["ContratoID"] = row["cobranca_propostaId"];
            //                    resultadoLinha["ContratoVigencia"] = Convert.ToDateTime(row["contrato_vigencia"], cinfo).ToString("dd/MM/yyyy");
            //                    resultadoLinha["OperadoraID"] = row["operadora_id"];
            //                    resultadoLinha["OperadoraNome"] = row["operadora_nome"];
            //                    resultadoLinha["ProdutorID"] = superiores[0].SuperiorID;
            //                    resultadoLinha["ProdutorPerfilID"] = superiores[0].SuperiorPerfilID;
            //                    resultadoLinha["ProdutorApelido"] = superiores[0].SuperiorApelido;
            //                    resultadoLinha["NomeTitular"] = row["beneficiario_nome"];
            //                    resultadoLinha["ContratoAdmissao"] = Convert.ToDateTime(row["contrato_admissao"], cinfo).ToString("dd/MM/yyyy");
            //                    resultadoLinha["FecharComissao"] = "0";

            //                    resultadoLinha["ProdutorBanco"] = superiores[0].SuperiorBanco;
            //                    resultadoLinha["ProdutorAgencia"] = superiores[0].SuperiorAgencia;
            //                    resultadoLinha["ProdutorConta"] = superiores[0].SuperiorConta;

            //                    #endregion

            //                    #region Comissionamento

            //                    comissionamentoModelo = new Comissionamento(comU.TabelaComissionamentoID);
            //                    pm.Load(comissionamentoModelo);

            //                    if (comissionamentoModelo.GrupoID == null) { itensComissionamento = null; }
            //                    else { itensComissionamento = ComissionamentoItem.Carregar(comissionamentoModelo.GrupoID, pm); }
            //                    valorComissao = 0; percentualComissao = 0;

            //                    if (itensComissionamento != null)
            //                    {
            //                        parcela = Convert.ToInt32(row["cobranca_parcela"]);
            //                        tipoContrato = Convert.ToInt32(row["contrato_tipoContratoId"]);
            //                        foreach (ComissionamentoItem item in itensComissionamento)
            //                        {
            //                            if (parcela == item.Parcela)
            //                            {
            //                                if (tipoContrato == 1) //normal
            //                                {
            //                                    tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal;
            //                                    valorComissao = valorPago * (item.Percentual / 100);
            //                                    percentualComissao = item.Percentual;
            //                                    break;
            //                                }
            //                                else if (tipoContrato == 4) //carencia
            //                                {
            //                                    tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia;
            //                                    valorComissao = valorPago * (item.PercentualCompraCarencia / 100);
            //                                    percentualComissao = item.PercentualCompraCarencia;
            //                                    break;
            //                                }
            //                                else if (tipoContrato == 3) //migracao
            //                                {
            //                                    tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao;
            //                                    valorComissao = valorPago * (item.PercentualMigracao / 100);
            //                                    percentualComissao = item.PercentualMigracao;
            //                                    break;
            //                                }
            //                                else if (tipoContrato == 2) //adm
            //                                {
            //                                    tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa;
            //                                    valorComissao = valorPago * (item.PercentualADM / 100);
            //                                    percentualComissao = item.PercentualADM;
            //                                    break;
            //                                }
            //                                else if (tipoContrato == 5) //especial
            //                                {
            //                                    tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial;
            //                                    valorComissao = valorPago * (item.PercentualEspecial / 100);
            //                                    percentualComissao = item.PercentualEspecial;
            //                                    break;
            //                                }
            //                                else if (tipoContrato == 6) //idade
            //                                {
            //                                    tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade;
            //                                    valorComissao = valorPago * (item.Idade / 100);
            //                                    percentualComissao = item.Idade;
            //                                    break;
            //                                }
            //                            }
            //                        }
            //                    }
            //                    #endregion

            //                    #region Vitaliciedade COMO EXIBIR NO RELATORIO ESTE PERCENTUAL DE COMISSAO ???

            //                    itemVitaliciedade = ComissionamentoVitaliciedade.Carregar(grupoId, tipoComissionamento, pm); //ComissionamentoVitaliciedade.Carregar(comissionamentoModelo.GrupoID, tipoComissionamento, pm);
            //                    if (itemVitaliciedade != null && itemVitaliciedade.ParcelaInicio >= parcela)
            //                    {
            //                        valorComissao += (valorPago * (itemVitaliciedade.Percentual / 100));
            //                    }
            //                    #endregion

            //                    atualizaApelidoSuperior(ref resultado, superiores[0].SubordinadoID, superiores[0].SuperiorApelido);

            //                    resultadoLinha["ProdutorNome"] = superiores[0].SuperiorNome;
            //                    resultadoLinha["ProdutorValor"] = valorComissao.ToString("N2");
            //                    resultadoLinha["ProdutorPercentualComissao"] = percentualComissao.ToString("N2");

            //                    //se nao foi informado um produtor especifico ou 
            //                    //foi informado e é o mesmo que o produtor corrente
            //                    if (produtorId == null ||
            //                        Convert.ToString(produtorId) == Convert.ToString(superiores[0].SuperiorID))
            //                    {
            //                        //checa também se o perfil está incluso na relacao de perfis enviada por parametro
            //                        if (perfilIDs == null || contemValor(perfilIDs, superiores[0].SuperiorPerfilID))
            //                        {
            //                            resultadoLinha["FecharComissao"] = "1";

            //                            #region itens abertos na conta corrente

            //                            creditos = 0;
            //                            itensCC = (List<MovimentacaoContaCorrente>)MovimentacaoContaCorrente.CarregarEmAberto(superiores[0].SuperiorID, dataCorte.Value, pm);
            //                            if (itensCC == null) { itensCC = new List<MovimentacaoContaCorrente>(); }

            //                            //só salva o credito se ele ainda nao foi feito.
            //                            if (valorComissao > 0 && !MovimentacaoContaCorrente.CreditoJaFeitoPara(superiores[0].SuperiorID, row["cobranca_id"], pm))
            //                            {
            //                                itemCC = new MovimentacaoContaCorrente();
            //                                itemCC.CategoriaID = CategoriaContaCorrente.SysPremiacaoCategoriaID;
            //                                itemCC.CobrancaID = row["cobranca_id"];
            //                                itemCC.Data = DateTime.Now;
            //                                itemCC.LisagemFechamentoID = listagem.ID;
            //                                itemCC.ProdutorID = superiores[0].SuperiorID;
            //                                itemCC.Valor = valorComissao;
            //                                pm.Save(itemCC);
            //                            }

            //                            foreach (MovimentacaoContaCorrente _item in itensCC)
            //                            {
            //                                if (CategoriaContaCorrente.eTipo.Credito == (CategoriaContaCorrente.eTipo)_item.CategoriaTipo)
            //                                    creditos += _item.Valor;
            //                                else
            //                                    creditos -= _item.Valor;
            //                                _item.LisagemFechamentoID = listagem.ID;
            //                                pm.Save(_item);
            //                            }
            //                            #endregion
            //                        }
            //                    }

            //                    resultadoLinha["ProdutorCredito"] = creditos.ToString("N2");
            //                    resultado.Rows.Add(resultadoLinha);
            //                    adicionaGrafoDeProdutoresParaCobranca(ref vos, row["cobranca_id"], superiores[0].SuperiorID, true);
            //                }

            //                superiores = SuperiorSubordinado.CarregarSuperiores(superiores[0].SuperiorID, admissao, pm);
            //            }
            //        }
            //    }

            //    /////////////////////////////////////////////////////////////////////////////////////////
            //    //TODOS os corretores que venderam foram processados
            //    //agora, os que NÃO venderam, mas têm créditos ou débitos na conta corrente, devem ser
            //    //processados
            //    qry = String.Concat("contacorrenteMovimentacao.*, usuario_id, usuario_nome, usuario_perfilId, usuario_apelido, usuario_documento1, usuario_banco, usuario_agencia, usuario_conta, cccategoria_tipo ",
            //    "   FROM contacorrenteMovimentacao ",
            //    "       INNER JOIN usuario                ON usuario_id    = contacorrentemov_produtorId ",
            //    "       INNER JOIN usuario_filial         ON usuario_id    = usuariofilial_usuarioId ",
            //    "       INNER JOIN contacorrenteCategoria ON cccategoria_id=contacorrentemov_categoriaId ",
            //    "   WHERE ",
            //    "       contacorrentemov_produtorid NOT IN (SELECT listagemrelacao_produtorId FROM listagem_relacao WHERE listagemrelacao_listagemid=", listagem.ID, ") ",
            //    "       AND contacorrentemov_data <= '", dataCorte.Value.ToString("yyyy-MM-dd 23:59:59.950"), "' ",
            //    filiaisParam, " AND usuario_perfilId IN (", perfisParam, ") AND contacorrentemov_listagemFechamentoId is null");

            //    dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];
            //    if (dt.Rows.Count > 0)
            //    {
            //        //ARMAZENA todos os corretores em uma colecao de ids de corretor, sem duplicar.
            //        System.Collections.ArrayList _array = new System.Collections.ArrayList();
            //        foreach (DataRow row in dt.Rows)
            //        {
            //            if (!_array.Contains(Convert.ToString(row["usuario_id"])))
            //            {
            //                _array.Add(Convert.ToString(row["usuario_id"]));
            //            }
            //        }

            //        foreach (DataRow row in dt.Rows)
            //        {
            //            resultadoLinha = resultado.NewRow();

            //            #region preenche a linha

            //            resultadoLinha["ProdutorID"] = row["usuario_id"];
            //            resultadoLinha["ProdutorNome"] = row["usuario_nome"];
            //            resultadoLinha["ProdutorPerfilID"] = row["usuario_perfilId"];
            //            resultadoLinha["ProdutorApelido"] = row["usuario_apelido"];

            //            if (Convert.ToInt32(row["cccategoria_tipo"]) == (Int32)CategoriaContaCorrente.eTipo.Credito)
            //                resultadoLinha["ProdutorValor"] = row["contacorrentemov_valor"];
            //            else
            //                resultadoLinha["ProdutorValor"] = -1 * Convert.ToDecimal(row["contacorrentemov_valor"]);

            //            resultadoLinha["ContaCorrenteID"] = row["contacorrentemov_id"];
            //            resultadoLinha["ContaCorrenteMotivo"] = row["contacorrentemov_motivo"];

            //            resultadoLinha["ProdutorPercentualComissao"] = 0;

            //            resultadoLinha["CobrancaID"] = null;
            //            resultadoLinha["CobrancaVencimento"] = null;
            //            resultadoLinha["CobrancaValorPago"] = 0; //Convert.ToDecimal(row["cobranca_valorPagto"], cinfo).ToString("N2");
            //            resultadoLinha["CobrancaDataPago"] = null;
            //            resultadoLinha["CobrancaParcela"] = null;
            //            resultadoLinha["ContratoNumero"] = null;
            //            resultadoLinha["ContratoID"] = null;
            //            resultadoLinha["ContratoVigencia"] = null;
            //            resultadoLinha["OperadoraID"] = null;
            //            resultadoLinha["OperadoraNome"] = null;
            //            resultadoLinha["NomeTitular"] = null;
            //            resultadoLinha["ContratoAdmissao"] = null;
            //            resultadoLinha["FecharComissao"] = "1";

            //            resultadoLinha["ProdutorBanco"] = row["usuario_banco"];
            //            resultadoLinha["ProdutorAgencia"] = row["usuario_agencia"];
            //            resultadoLinha["ProdutorConta"] = row["usuario_conta"];

            //            #endregion

            //            //atualiza a linha de c/c para o id deste fechamento
            //            NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contacorrenteMovimentacao SET contacorrentemov_listagemFechamentoId=" + listagem.ID + " WHERE contacorrentemov_id=" + row["contacorrentemov_id"], pm);

            //            resultado.Rows.Add(resultadoLinha);

            //            //salva o grafo
            //            salvarGrafoEquipeCorretor(row["usuario_id"], row["usuario_perfilId"], listagem.ID, pm);
            //        }
            //    }

            //    //salva a relacao de pagamento de comissao
            //    salvarRelacao(resultado, listagem.ID, pm);

            //    //baixa as cobrancas que devem ser baixadas
            //    baixarCobrancas(vos, pm);

            //    pm.Commit();
            //}
            //catch (Exception ex)
            //{
            //    pm.Rollback();
            //    throw ex;
            //}
            //finally
            //{
            //    dt.Dispose();
            //    pm = null;
            //    cmdIniciarListagem.Enabled = true;
            //    lblPercListagem.Text = "Concluído";
            //}

            //return resultado;
        }

        #region metodos auxiliares

        void adicionaGrafoDeProdutoresParaCobranca(ref List<CobrancaCorretorChefiaVO> vos, Object cobrancaId, Object produtorId, Boolean procurar)
        {
            if (procurar) //checa se ja existe antes adicionar
            {
                foreach (CobrancaCorretorChefiaVO vo in vos)
                {
                    if (vo.CobrancaID == Convert.ToString(cobrancaId)) //primeira chave é o ID da cobrança
                    {
                        foreach (Object id in vo.ProdutorIDs) //para a cobrança corrente, checa os produtores adicionados
                        {
                            if (Convert.ToString(id) == Convert.ToString(produtorId)) { return; } //já está adicionado. nao faz nada
                        }

                        vo.ProdutorIDs.Add(Convert.ToString(produtorId));
                        return;
                    }
                }
            }

            CobrancaCorretorChefiaVO _vo = new CobrancaCorretorChefiaVO();
            _vo.CobrancaID = Convert.ToString(cobrancaId);
            _vo.ProdutorIDs.Add(produtorId);
            vos.Add(_vo);
        }

        void salvarGrafoEquipe(SuperiorSubordinado relacao, Object listagemId, PersistenceManager pm)
        {
            String cmd = String.Concat("if(NOT EXISTS(SELECT NULL FROM listagem_relacao_grafo WHERE listagemrelacaografo_listagemId=", listagemId, " AND listagemrelacaografo_superiorId=", relacao.SuperiorID, " AND listagemrelacaografo_imediatoId=", relacao.SubordinadoID, " AND listagemrelacaografo_superiorPerfilId=", relacao.SuperiorPerfilID, "AND listagemrelacaografo_imediatoPerfilId=", relacao.SubordinadoPerfilID, ")) ",
                " BEGIN ",
                "   INSERT INTO listagem_relacao_grafo (listagemrelacaografo_listagemId,listagemrelacaografo_superiorId,listagemrelacaografo_superiorPerfilId,listagemrelacaografo_imediatoId,listagemrelacaografo_imediatoPerfilId) VALUES (", listagemId, ", ", relacao.SuperiorID, ", ", relacao.SuperiorPerfilID, ", ", relacao.SubordinadoID, ", ", relacao.SubordinadoPerfilID, ")",
                " END ");

            NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);
        }

        void salvarGrafoEquipeCorretor(Object corretorId, Object corretorPerfilId, Object listagemId, PersistenceManager pm)
        {
            String cmd = String.Concat("if(NOT EXISTS(SELECT NULL FROM listagem_relacao_grafo WHERE listagemrelacaografo_listagemId=", listagemId, " AND listagemrelacaografo_superiorId=", corretorId, " AND listagemrelacaografo_imediatoId IS NULL AND listagemrelacaografo_superiorPerfilId=", corretorPerfilId, " AND listagemrelacaografo_imediatoPerfilId IS NULL)) ",
                " BEGIN ",
                "   INSERT INTO listagem_relacao_grafo (listagemrelacaografo_listagemId,listagemrelacaografo_superiorId,listagemrelacaografo_superiorPerfilId,listagemrelacaografo_imediatoId,listagemrelacaografo_imediatoPerfilId) VALUES (", listagemId, ", ", corretorId, ",", corretorPerfilId, ", NULL, NULL)",
                " END ");

            NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);
        }

        /// <summary>
        /// Otimizar rotina.
        /// </summary>
        void salvarRelacao(DataTable dt, Object listagemId, PersistenceManager pm)
        {
            StringBuilder sb = new StringBuilder();
            String[] pValues;
            String[] pNames = new String[] { "@OperadoraNome", "@CobrancaValorPago", "@ProdutorNome", "@ProdutorApelido", "@SuperiorApelido", "@NomeTitular", "@ProdutorBanco", "@ProdutorAgencia", "@ProdutorConta", "@ContaCorrenteMotivo" };
            foreach (DataRow row in dt.Rows)
            {
                if ((CToString(row["ProdutorPercentualComissao"]) == "0.00" || CToString(row["ProdutorPercentualComissao"]) == "0,00") && row["ContaCorrenteID"] == null)
                {
                    continue;
                }

                pValues = new String[] { CToString(row["OperadoraNome"]), CToString(row["CobrancaValorPago"]),
                    CToString(row["ProdutorNome"]), CToString(row["ProdutorApelido"]), 
                    CToString(row["SuperiorApelido"]), CToString(row["NomeTitular"]), CToString(row["ProdutorBanco"]), CToString(row["ProdutorAgencia"]), CToString(row["ProdutorConta"]), CToString(row["ContaCorrenteMotivo"]) };

                sb.Append("INSERT INTO listagem_relacao (listagemrelacao_listagemId, listagemrelacao_contratoNumero,listagemrelacao_contratoId,listagemrelacao_operadoraId,listagemrelacao_operadoraNome,listagemrelacao_cobrancaId,listagemrelacao_cobrancaValorPago,listagemrelacao_cobrancaDataPago,listagemrelacao_percentualComissao,listagemrelacao_cobrancaParcela,listagemrelacao_produtorId,listagemrelacao_produtorNome,listagemrelacao_produtorValor,listagemrelacao_produtorCredito,listagemrelacao_produtorPerfilId,listagemrelacao_produtorApelido,listagemrelacao_superiorApelido,listagemrelacao_contratoNomeTitular,listagemrelacao_produtorBanco,listagemrelacao_produtorAgencia,listagemrelacao_produtorConta, listagemrelacao_cobrancaDataVencto,listagemrelacao_contratoVigencia,listagemrelacao_fechado,listagemrelacao_contratoAdmissao, listagemrelacao_contacorrenteId, listagemrelacao_contacorrenteMotivo) VALUES (");
                sb.Append(listagemId); sb.Append(",'");
                sb.Append(CToString(row["ContratoNumero"])); sb.Append("',");

                if (CToString(row["ContratoID"]) != "")
                    sb.Append(row["ContratoID"]);
                else
                    sb.Append("NULL");

                sb.Append(",");

                if (CToString(row["OperadoraID"]) != "")
                    sb.Append(row["OperadoraID"]);
                else
                    sb.Append("NULL");

                sb.Append(",");

                sb.Append("@OperadoraNome"); sb.Append(",");

                if (CToString(row["CobrancaID"]) != "")
                    sb.Append(row["CobrancaID"]);
                else
                    sb.Append("NULL");

                sb.Append(",");

                sb.Append("@CobrancaValorPago"); sb.Append(",'");
                sb.Append(row["CobrancaDataPago"]); sb.Append("','");
                sb.Append(row["ProdutorPercentualComissao"]); sb.Append("',");

                if (CToString(row["CobrancaParcela"]) != "")
                    sb.Append(row["CobrancaParcela"]);
                else
                    sb.Append("NULL");

                sb.Append(",");

                sb.Append(row["ProdutorID"]); sb.Append(",");
                sb.Append("@ProdutorNome"); sb.Append(",'");
                sb.Append(CToString(row["ProdutorValor"])); sb.Append("','");
                sb.Append(row["ProdutorCredito"]); sb.Append("',");
                sb.Append(row["ProdutorPerfilID"]); sb.Append(",");
                sb.Append("@ProdutorApelido"); sb.Append(",");
                sb.Append("@SuperiorApelido"); sb.Append(",");
                sb.Append("@NomeTitular"); sb.Append(",");

                sb.Append("@ProdutorBanco"); sb.Append(",");
                sb.Append("@ProdutorAgencia"); sb.Append(",");
                sb.Append("@ProdutorConta"); sb.Append(",'");

                sb.Append(row["CobrancaVencimento"]); sb.Append("','");
                sb.Append(row["ContratoVigencia"]); sb.Append("',");
                sb.Append(row["FecharComissao"]); sb.Append(",'");
                sb.Append(row["ContratoAdmissao"]); sb.Append("',");

                if (CToString(row["ContaCorrenteID"]) == "")
                    sb.Append("NULL");
                else
                    sb.Append(row["ContaCorrenteID"]);

                sb.Append(",");
                sb.Append("@ContaCorrenteMotivo); ");

                try
                {
                    NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pNames, pValues, pm);

                }
                catch (Exception ex)
                {
                    int z = 0;
                }

                sb.Remove(0, sb.Length);
            }
        }

        void baixarCobrancas(List<CobrancaCorretorChefiaVO> vos, PersistenceManager pm)
        {
            if (vos == null || vos.Count == 0) { return; }
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (CobrancaCorretorChefiaVO vo in vos)
            {
                if (sb.Length > 0) { sb.Append(" ; "); }
                i = 0;

                sb.Append("IF (");

                foreach (Object id in vo.ProdutorIDs)
                {
                    if (i > 0) { sb.Append(" AND "); }
                    sb.Append("EXISTS(SELECT listagemrelacao_cobrancaId FROM listagem_relacao WHERE listagemrelacao_fechado=1 AND listagemrelacao_cobrancaId="); sb.Append(vo.CobrancaID); sb.Append(" AND listagemrelacao_produtorId="); sb.Append(id); sb.Append(")");
                    i++;
                }

                sb.Append(") BEGIN UPDATE cobranca SET cobranca_comissaoPaga=1 WHERE cobranca_id=");
                sb.Append(vo.CobrancaID); sb.Append(" END ");

                NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
                sb.Remove(0, sb.Length);
            }

            //NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
        }

        void atualizaApelidoSuperior(ref DataTable dt, Object produtorId, String apelidoSuperior)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToString(row["ProdutorID"]) == Convert.ToString(produtorId))
                {
                    row["SuperiorApelido"] = apelidoSuperior;
                    break;
                }
            }
        }

        /// <summary>
        /// Checa se um valor existe no array.
        /// </summary>
        Boolean contemValor(String[] array, Object valor)
        {
            if (array == null) { return false; }

            String _valor = Convert.ToString(valor);
            foreach (String item in array)
            {
                if (item == _valor) { return true; }
            }

            //return array.Contains<String>(_valor);
            return false;
        }

        #endregion

        /// <summary>
        /// Checar/Alterar parâmetros da query no metodo CarregaRelacaoEmAberto
        /// Deletar linhas a mais em contacorrentemovimentacao
        /// Rodar a rotina que arruma os corretores fakes
        /// Checar string de conexao (nao deve estar para o banco em producao, mas para o local)
        /// Checar parâmetros enviados na chamada abaixo
        /// </summary>
        private void cmdIniciarListagem_Click(Object sender, EventArgs e)
        {
            //this.CalculaFechamento("PGTO ADESAO SP AGOSTO 2012",
            //    "<mensagem>", new String[] { "4" },
            //    new String[] { "3", "4", "5", "6", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" }, //new String[] { "10", "12", "14" },
            //    new String[] { "3", "7", "6", "5", "8", "4", "2" },
            //    new DateTime(2012, 08, 15, 23, 59, 59, 998), null, //2012, 07, 16, 23, 59, 59, 998
            //    null,
            //    null);

            fechaListagem();
        }

        /// <summary>
        /// Comentado
        /// </summary>
        void fechaListagem()
        {
            //if (busy) { return; }

            //ListagemAgendamento item = ListagemAgendamento.CarregarPendente();

            //if (item == null) { busy = false; return; }

            //try
            //{
            //    this.CalculaFechamento(
            //        item.Descricao, "<mensagem>",
            //        new String[] { Convert.ToString(item.FilialID) },
            //        item.OperadoraIDs.Split(','),
            //        new String[] { "3", "7", "6", "5", "8", "4", "2" },
            //        item.DataCorte,
            //        null,
            //        null,
            //        item.Competencia);

            //    item.Processado = true;
            //    item.ProcessadoData = DateTime.Now;
            //    item.Salvar();
            //}
            //catch
            //{
            //    throw;
            //}
            //finally
            //{
            //    busy = false;
            //}
        }

        private void timerComissao_Tick(object sender, EventArgs e)
        {
            try
            {
                fechaListagem();
            }
            catch
            {
            }
        }

        //private void button1_Click(Object sender, EventArgs e)
        //{
        //    String[] lines = textBox1.Text.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        //    String aux = ""; Object id = null;
        //    Cobranca cobranca = new Cobranca();

        //    using (SqlConnection conn = new SqlConnection("Server=(local);Database=padrao_producaoDB;USER ID=sa;PWD=!-sql4f34U!65;timeout=1999999999"))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = conn.CreateCommand();

        //        int i = 0;
        //        foreach (String line in lines)
        //        {
        //            aux = line.Substring(1, 15);
        //            cobranca.LeNossoNumero(aux);

        //            cmd.CommandText = String.Concat("SELECT cobranca_id FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id WHERE cobranca_tipo=",
        //                cobranca.Tipo, " AND cobranca_parcela=", cobranca.Parcela, " AND contrato_codcobranca=", cobranca.ContratoCodCobranca);
        //            id = cmd.ExecuteScalar();

        //            if (id == null) { continue; }

        //            cmd.CommandText = "UPDATE cobranca set cobranca_arquivoUltimoEnvioId=-2 WHERE cobranca_id=" + id;
        //            cmd.ExecuteNonQuery();

        //            i++;
        //            lblcount.Text = i.ToString();
        //            Application.DoEvents();
        //        }

        //        MessageBox.Show("ok");
        //    }
        //}

        #endregion

        #region RELATORIOS AGENDADOS

        void cmdRelAgend_Click(Object sender, EventArgs e)
        {
            try
            {
                processaRelatoriosAgendados();
            }
            catch (Exception ex)
            {
                busy = false;
                lblRelAgend.Text = ex.Message;
            }
        }

        void timerRelatoriosAgendados_Tick(Object sender, EventArgs e)
        {
            try
            {
                processaRelatoriosAgendados();
            }
            catch (Exception ex)
            {
                busy = false;
                lblRelAgend.Text = ex.Message;
            }
        }

        /// <summary>
        /// Comentado
        /// </summary>
        void processaRelatoriosAgendados()
        {
            //string qryDeta = AgendaRelatorio.ControladoriaDetalheQUERY(DateTime.Now,
            //            DateTime.Now, new string[] { "1" }, new string[] { "1" });

            //string qryTaxa = AgendaRelatorio.ControladoriaTaxaQUERY(DateTime.Now,
            //            DateTime.Now, new string[] { "1" }, new string[] { "1" });

            //return;

            IList<AgendaRelatorio> agenda = AgendaRelatorio.CarregarTodos(true, DateTime.Now);
            if (agenda == null || agenda.Count == 0)
            { lblRelAgend.Text = "Nenhum - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"); return; }

            String caminhoBase = System.Configuration.ConfigurationManager.AppSettings["reports_file"];
            if (!Directory.Exists(caminhoBase)) { Directory.CreateDirectory(caminhoBase); }

            String qry = "";
            DataTable dt = null;
            StringBuilder sb = null;

            lblRelAgend.Text = "Processando - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            Application.DoEvents();

            foreach (AgendaRelatorio item in agenda)
            {
                if (item.Tipo == (int)AgendaRelatorio.eTipo.ControladoriaDetalhe)
                    qry = AgendaRelatorio.ControladoriaDetalheQUERY(item.DataDe,
                        item.DataAte, item.OperadoraIDs.Split(','), item.EstipulanteIDs.Split(','));
                else if (item.Tipo == (int)AgendaRelatorio.eTipo.ControladoriaTaxa)
                    qry = AgendaRelatorio.ControladoriaTaxaQUERY(item.DataDe,
                        item.DataAte, item.OperadoraIDs.Split(','), item.EstipulanteIDs.Split(','));
                else if (item.Tipo == (int)AgendaRelatorio.eTipo.CReceberAberto)
                    qry = AgendaRelatorio.ContasReceberAbertoQUERY(item.DataDe,
                        item.DataAte, item.OperadoraIDs.Split(','), item.EstipulanteIDs.Split(','));
                else if (item.Tipo == (int)AgendaRelatorio.eTipo.CReceberPago)
                    qry = AgendaRelatorio.ContasReceberPagoQUERY(item.DataDe,
                        item.DataAte, item.OperadoraIDs.Split(','), item.EstipulanteIDs.Split(','));
                else
                    continue;

                dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];

                sb = new StringBuilder();

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (sb.Length > 0) { sb.Append("|"); }
                    else { sb.Append("sep=|"); sb.Append(Environment.NewLine); }

                    sb.Append(AgendaRelatorio.UI.TrataNomeColuna(dt.Columns[i].ColumnName));
                }

                sb.Append(Environment.NewLine);

                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (i > 0) { sb.Append("|"); }

                        if (row[i] != null && row[i] != DBNull.Value && Convert.ToString(row[i]).StartsWith("0") &&
                            Convert.ToString(row[i]).Length > 4 && Convert.ToString(row[i]).IndexOf("anos") == -1 &&
                            Convert.ToString(row[i]).IndexOf("/") == -1)
                        {
                            sb.Append("'");
                        }

                        sb.Append(row[i]);
                    }

                    sb.Append(Environment.NewLine);
                }

                File.WriteAllText(caminhoBase + item.ID + ".csv", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));
                item.Processado = true;
                item.Salvar();

                sb.Remove(0, sb.Length);

                dt.Rows.Clear();
                dt.Dispose();
            }

            lblRelAgend.Text = "Concluído - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        #endregion RELATORIOS AGENDADOS

        void setaNossosNumeros()
        {
            busy = true;
            string arquivo = @"C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\seta_nossonumero\cobrancas13.ret";

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                string[] linhas = File.ReadAllLines(arquivo);
                string temp = "";

                Cobranca cobranca = null;
                int i = 0;
                foreach (string linha in linhas)
                {
                    i++;
                    try
                    {
                        cobranca = new Cobranca(Convert.ToInt32(linha.Substring(21, 13)));
                        pm.Load(cobranca);
                    }
                    catch
                    {
                        continue;
                    }

                    if (cobranca.Pago) continue;

                    //if (!string.IsNullOrEmpty(cobranca.NossoNumero)) continue;//////////////

                    //cobranca.NossoNumero = linha.Substring(3, 17);
                    ////pm.Save(cobranca);

                    temp = cobranca.GeraNossoNumero();

                    if (cobranca.DataVencimento.Month != 6)
                    {
                        cobranca.NossoNumero = linha.Substring(3, 17);
                        if (cobranca.NossoNumero.Trim() == "") continue;
                        NonQueryHelper.Instance.ExecuteNonQuery(string.Concat(
                            "update cobranca set cobranca_nossoNumero='", linha.Substring(3, 17), "' where cobranca_id=", cobranca.ID), pm);
                    }
                    else
                    {
                        //NonQueryHelper.Instance.ExecuteNonQuery(string.Concat(
                        //    "update cobranca set cobranca_dataVencimento='", cobranca.DataVencimento.Year, "-", cobranca.DataVencimento.Month, "-", "10 23:59:59:990" ,"', cobranca_nossoNumero='", linha.Substring(3, 17), "' where cobranca_id=", cobranca.ID), pm);
                        cobranca.NossoNumero = linha.Substring(3, 17);
                        if (cobranca.NossoNumero.Trim() == "") continue;
                        NonQueryHelper.Instance.ExecuteNonQuery(string.Concat(
                            "update cobranca set cobranca_nossoNumero='", linha.Substring(3, 17), "' where cobranca_id=", cobranca.ID), pm);
                    }
                }

                pm.CloseSingleCommandInstance();
            }
        }

        void gerarArquivoBoletoLote___()
        {
            busy = true;
            string arquivo = @"C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\seta_nossonumero\CBR7242702710201624140.txt";
            StringBuilder sb = new StringBuilder();

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                string[] linhas = File.ReadAllLines(arquivo);

                Cobranca cobranca = null;
                string host = "http://localhost:4123";
                string nossoNumero = "", uri = "", finalUrl = "";
                object beneficiarioId = null; //ret = null;

                //IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>("* from cobranca where cobranca_arquivoUltimoEnvioId in (23,24)", typeof(Cobranca), pm);

                int i = 0;
                foreach (string linha in linhas)
                //foreach(Cobranca cobranca in cobrancas)
                {
                    try
                    {
                        cobranca = new Cobranca(Convert.ToInt32(linha.Substring(21, 13)));
                        pm.Load(cobranca);
                    }
                    catch
                    {
                        continue;
                    }

                    if (cobranca.Pago) { continue; }

                    //if (cobranca.DataVencimento.Month <= 6) continue;

                    //nossoNumero = linha.Substring(3, 17);

                    beneficiarioId = ContratoBeneficiario.CarregaTitularID(cobranca.PropostaID, pm);

                    //ret = LocatorHelper.Instance.ExecuteScalar("select endereco_uf from endereco where endereco_donoTipo=0 and endereco_donoid=" + beneficiarioId, null, null, pm);
                    //if (ret == null) continue;
                    //if (Convert.ToString(ret).ToUpper() != "SP") continue; fiz no dia em que gerei boletos de sp, mas nao usei

                    uri = EntityBase.RetiraAcentos(
                        String.Concat(
                            "?nossonum=", nossoNumero,
                            "&contid=", cobranca.PropostaID,
                            "&valor=", cobranca.Valor.ToString("N2"),
                            "&v_dia=", cobranca.DataVencimento.Day, "&v_mes=", cobranca.DataVencimento.Month, "&v_ano=", cobranca.DataVencimento.Year,
                            "&bid=", beneficiarioId,
                            "&cobid=", cobranca.ID,
                            "&mailto="));

                    finalUrl = string.Concat("/boleto/bb.aspx", uri);

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

                string final = sb.ToString();
                pm.CloseSingleCommandInstance();
            }
        }
        void gerarArquivoBoletoLote2()
        {
            busy = true;
            //string arquivo = @"C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\cobrancas12.ret";
            StringBuilder sb = new StringBuilder();

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                //string[] linhas = null; // File.ReadAllLines(arquivo);

                //Cobranca cobranca = null;
                //string host = "http://localhost:4123";

                string host = "http://ubrasp.iphotel.info";
                string nossoNumero = "", uri = "", finalUrl = "";
                object beneficiarioId = null; //ret = null;

                //List<string> novosIds = new List<string>();

                IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>("* from cobranca where cobranca_arquivoUltimoEnvioId in (71)", typeof(Cobranca), pm);
                //IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>("* from cobranca where cobranca_id in (3343,3326,3270,3303,3312,3351,3219,3267,3334,3365,3214,3245,3222,3348,3320,3357,3276,3360,3231,3262,3317,3205,3228,3293,3284,3253,3259,3309,3298,3236,3178,3195,3169,3181,3186,3172,3200)", typeof(Cobranca), pm);

                int i = 0;
                //foreach (string linha in linhas)
                foreach (Cobranca cobranca in cobrancas)
                {
                    //cobranca.ID = null;
                    //pm.Save(cobranca);
                    //novosIds.Add(Convert.ToString(cobranca.ID));

                    //try
                    //{
                    //    cobranca = new Cobranca(Convert.ToInt32(linha.Substring(21, 13)));
                    //    pm.Load(cobranca);
                    //}
                    //catch
                    //{
                    //    continue;
                    //}

                    //if (cobranca.DataVencimento.Month <= 6) continue;

                    //nossoNumero = linha.Substring(3, 17);

                    beneficiarioId = ContratoBeneficiario.CarregaTitularID(cobranca.PropostaID, pm);

                    //ret = LocatorHelper.Instance.ExecuteScalar("select endereco_uf from endereco where endereco_donoTipo=0 and endereco_donoid=" + beneficiarioId, null, null, pm);
                    //if (ret == null) continue;
                    //if (Convert.ToString(ret).ToUpper() != "SP") continue; fiz no dia em que gerei boletos de sp, mas nao usei

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
                    //finalUrl = string.Concat("/boleto/bb.aspx", uri);

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


                //string novos = string.Join(",", novosIds.ToArray());
                string final = sb.ToString().Replace(@"D:\http\ubrasp\web\var\boleto_file\BoletoNetBarra.gif", @"http://ubrasp.iphotel.info/images/boleto/BoletoNetBarra.gif");
                pm.CloseSingleCommandInstance();

                File.WriteAllText(@"C:\Users\ACER E1 572 6830\___bol.html", final);
            }
        }
        public byte[] GetPDF(string pHTML)
        {
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
        void gerarPdfDeBoletosEmTolote(string html)
        {
            //http://www.rswebsols.com/tutorials/programming/asp-net-generate-pdf-html-itextsharp


            /////////////////////////////////////////////////////////////////////////////// BOM
            //http://www.codigomaster.com.br/desenvolvimento/como-exportar-html-para-pdf-em-c-e-itextsharp
            //HtmlForm form = new HtmlForm(); 
            Document document = new Document(PageSize.A4, 20, 20, 20, 20); 
            MemoryStream ms = new MemoryStream(); 
            PdfWriter writer = PdfWriter.GetInstance(document, ms); 
            HTMLWorker obj = new HTMLWorker(document);


            StringReader se = new StringReader(html); 
            document.Open(); 
            obj.Parse(se); 
            document.Close();

            //Response.Clear(); 
            //Response.AddHeader("content-disposition", "filename=NomedoMeuArquivo.pdf"); 
            //Response.ContentType = "application/pdf"; 
            //Response.Buffer = true;
            //Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length); 
            //Response.OutputStream.Flush(); Response.End();

            ///////////////////////////////////////////////////////////////////////////////
            //Document document = new Document();
            //try
            //{
            //    PdfWriter.GetInstance(document, new FileStream("c:\\my.pdf", FileMode.Create));
            //    document.Open();
            //    WebClient wc = new WebClient();
            //    List<IElement> htmlarraylist = HTMLWorker.ParseToList(new StringReader(html), null);
            //    for (int k = 0; k < htmlarraylist.Count; k++)
            //    {
            //        document.Add((IElement)htmlarraylist[k]);
            //    }

            //    document.Close();
            //}
            //catch
            //{
            //}
            /////////////////////////////////////////////////////////////////////////////////////
            //var bytes = System.Text.Encoding.UTF8.GetBytes(html);

            //using (var input = new MemoryStream(bytes))
            //{
            //    var output = new MemoryStream(); // this MemoryStream is closed by FileStreamResult

            //    var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER, 50, 50, 50, 50);
            //    var writer = PdfWriter.GetInstance(document, output);
            //    writer.CloseStream = false;
            //    document.Open();

            //    var xmlWorker = XMLWorkerHelper.GetInstance();
            //    xmlWorker.ParseXHtml(writer, document, input, null);
            //    document.Close();
            //    output.Position = 0;

            //    return new FileStreamResult(output, "application/pdf");
            //}

        }

        /****************************************************************/

        void gerarArquivoEtiquetaLote()
        {
            busy = true;
            //string arquivo = @"C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\cobrancas12.ret";
            StringBuilder sb = new StringBuilder();
            Beneficiario benef = null;
            IList<Endereco> enderecos = null;

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                //string[] linhas = null; //File.ReadAllLines(arquivo);

                //Cobranca cobranca = null;
                object beneficiarioId = null;

                int iLinha = 0, iColuna = 0;

                IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>("* from cobranca where cobranca_arquivoUltimoEnvioId in (38)", typeof(Cobranca), pm);

                sb.Append("<html><body><table width='100%'>");
                foreach (Cobranca cobranca in cobrancas)  //foreach (string linha in linhas)
                {
                    //try
                    //{
                    //    cobranca = new Cobranca(Convert.ToInt32(linha.Substring(21, 13)));
                    //    pm.Load(cobranca);
                    //}
                    //catch
                    //{
                    //    continue;
                    //}


                    beneficiarioId = ContratoBeneficiario.CarregaTitularID(cobranca.PropostaID, pm);

                    benef = new Beneficiario(beneficiarioId);
                    pm.Load(benef);

                    enderecos = Endereco.CarregarPorDono(benef.ID, Endereco.TipoDono.Beneficiario, pm);

                    if (iColuna == 0)
                    {
                        sb.Append("<tr>");
                    }

                    sb.Append("<td>"); 
                    sb.Append(benef.Nome);
                    sb.Append("<br>");
                    sb.Append(enderecos[0].Logradouro);
                    if (!string.IsNullOrEmpty(enderecos[0].Numero)) { sb.Append(", "); sb.Append(enderecos[0].Numero); }
                    if (!string.IsNullOrEmpty(enderecos[0].Complemento)) { sb.Append(", "); sb.Append(enderecos[0].Complemento); }
                    sb.Append("<br>");
                    sb.Append(enderecos[0].Bairro); sb.Append(" - "); sb.Append(enderecos[0].Cidade);
                    sb.Append("<br>");
                    sb.Append(enderecos[0].UF); sb.Append(" - "); sb.Append(enderecos[0].CEP);
                    sb.Append("</td>"); 

                        
                    iColuna++;

                    if (iColuna == 2)
                    {
                        sb.Append("</tr>");
                        sb.Append("<tr height='8px'><td colspan='3' height='8px'>&nbsp;</td></tr>");
                        iColuna = 0;
                        iLinha++;
                    }

                    if (iLinha == 7)
                    {
                        iLinha = 0;
                        sb.Append("</table>");
                        sb.Append("<div style=\"page-break-before: always;\"></div>");
                        sb.Append("<table width='100%'>");
                    }

                }

                sb.Append("</table></body></html>");

                string final = sb.ToString();
                pm.CloseSingleCommandInstance();
            }
        }

        void gerarArquivoEtiquetaLote_TEMP()
        {
            busy = true;
            string arquivo = @"C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\cobrancas11_SP.ret";
            StringBuilder sb = new StringBuilder();
            Beneficiario benef = null;
            IList<Endereco> enderecos = null;

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                string[] linhas = File.ReadAllLines(arquivo);

                object beneficiarioId = null;

                int iLinha = 0, iColuna = 0;

                string ids = "38368,39168,39169,39212,39223,39225,39226,39232,39248,39254,39256,39260,39271,39294,39304,39325,39333,39343,39356,39375,39381,39382,39387,39392,39395,39397,39437,39444,39445,39447,39450,39455,39459,39460,39464,39470,39481,39498,39507,39514,39517,39518,39522,39536,39537,39555,39566,39570,39571,39572,39586,39591,39596,39598,39615,39616,39623,39636,39644,39660,39698,39704,39738,39767,39788,39789,39805,39807,39811,39818,39820,39824,39825,39836,39838,39852,39856,39868,39881,39882,39883,39889,39900,39909,39914,39924,39933,39935,39942,39949,39950,39952,39953,39958,39964,39973,39984,39986,39987,40007,40016,40017,40018,40023,40026,40032,40034,40059,40064,40073,40076,40094,40100,40102,40105,40109,40116,40117,40141,40155,40163,40164,40185,40188,40193,40202";
                       ids = "38368,39168,39169,39212,39223,39225,39226,39232,39248,39254,39256,39260,39271,39294,39304,39325,39333,39343,39356,39375,39381,39382,39387,39392,39395,39397,39437,39444,39445,39447,39450,39455,39459,39460,39464,39470,39481,39498,39507,39514,39517,39518,39522,39536,39537,39555,39566,39570,39571,39572,39586,39591,39596,39598,39615,39616,39623,39636,39644,39660,39698,39704,39738,39767,39788,39789,39805,39807,39811,39818,39820,39824,39825,39836,39838,39852,39856,39868,39881,39882,39883,39889,39900,39909,39914,39924,39933,39935,39942,39949,39950,39952,39953,39958,39964,39973,39984,39986,39987,40007,40016,40017,40018,40023,40026,40032,40034,40059,40064,40073,40076,40094,40100,40102,40105,40109,40116,40117,40141,40155,40163,40164,40185,40188,40193,40202";

                string[] arr = ids.Split(',');

                List<string> _ids = new List<string>();
                foreach (string i in arr)
                {
                    if (!_ids.Contains(i))
                    {
                        _ids.Add(i);
                    }
                }

                sb.Append("<html><body><table width='100%'>");
                foreach (string id in _ids)
                {
                    beneficiarioId = id;

                    benef = new Beneficiario(beneficiarioId);
                    pm.Load(benef);

                    enderecos = Endereco.CarregarPorDono(benef.ID, Endereco.TipoDono.Beneficiario, pm);

                    if (iColuna == 0)
                    {
                        sb.Append("<tr>");
                    }

                    sb.Append("<td>");
                    sb.Append(benef.Nome);
                    sb.Append("<br>");
                    sb.Append(enderecos[0].Logradouro);
                    if (!string.IsNullOrEmpty(enderecos[0].Numero)) { sb.Append(", "); sb.Append(enderecos[0].Numero); }
                    if (!string.IsNullOrEmpty(enderecos[0].Complemento)) { sb.Append(", "); sb.Append(enderecos[0].Complemento); }
                    sb.Append("<br>");
                    sb.Append(enderecos[0].Bairro); sb.Append(" - "); sb.Append(enderecos[0].Cidade);
                    sb.Append("<br>");
                    sb.Append(enderecos[0].UF); sb.Append(" - "); sb.Append(enderecos[0].CEP);
                    sb.Append("</td>");


                    iColuna++;

                    if (iColuna == 2)
                    {
                        sb.Append("</tr>");
                        sb.Append("<tr height='8px'><td colspan='3' height='8px'>&nbsp;</td></tr>");
                        iColuna = 0;
                        iLinha++;
                    }

                    if (iLinha == 7)
                    {
                        iLinha = 0;
                        sb.Append("</table>");
                        sb.Append("<div style=\"page-break-before: always;\"></div>");
                        sb.Append("<table width='100%'>");
                    }

                }

                sb.Append("</table></body></html>");

                string final = sb.ToString();
                pm.CloseSingleCommandInstance();
            }
        }
        void gerarArquivoEtiquetaLote_TEMP2()
        {
            busy = true;
            StringBuilder sb = new StringBuilder();
            Beneficiario benef = null;
            IList<Endereco> enderecos = null;

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                object beneficiarioId = null;
                string ids = "39709,39694,39720,39239,39458,39634,39606,40036,39713,40080,39312,39932,39251,39323,40160,39453,39600,39551,39856,39564,39957,39841,39257,39778,39974,39462,40056,39983,40168,39863,39434,39625,40024,39483,39515,40138,39626,39801,39641,40038,39837,39454,39398,39217,39368,40003,39859,39939,39297,39477,39647,39777,40077,40111,39361,39435,39383,39539,39495,39500,40137,39182,39829,39945,39491,39524,39725,39237,39466,39438,39711,40133,39915,39969,39389,39869,39898,40010,39236,39475,39508,39700,40124,39692,39554,39927,39526,39681,39975,39171,39465,39903,39962,39686,39985,40157,39699,39798,39867,40136,39448,39940,39376,39351,39858,39252,39520,40006,39832,39740,40067,39639,39803,39391,39374,39684,39306,39675,39843,39972,39280,39813,39816,39781,39926,39683,39509,39776,39971,39774,40047,40084,39191,40055,39879,40052,39638,39603,39266,39227,40190,39386,39795,39545,40030,40037,40004,39276,40063,40086,39250,40206,39827,39195,39302,39563,39400,39405,39762,39451,39873,39241,39785,39468,39728,39722,39471,39286,39414,39439,39345,40103,39627,39961,40020,40199,39876,39360,39172,39723,39735,40035,39484,39544,40019,39764,39316,39291,39938,40004,12971,39744,40123,39622,39486,39501,39300,40172,39678,40115,39669,39620,40083,39661,39955,39443,13366,40134,40134,39346,39632,39905,39240,39221,39337,39895";

                string[] arr = ids.Split(',');

                List<string> _ids = new List<string>();
                foreach (string i in arr)
                {
                    if (!_ids.Contains(i))
                    {
                        _ids.Add(i);
                    }
                }

                sb.Append("<html><body>");
                int registro = 0;
                foreach (string id in _ids)
                {
                    if (registro == 0)
                    {
                        sb.Append("<table width='100%'>");
                    }

                    registro++;
                    beneficiarioId = id;

                    benef = new Beneficiario(beneficiarioId);
                    pm.Load(benef);

                    enderecos = Endereco.CarregarPorDono(benef.ID, Endereco.TipoDono.Beneficiario, pm);

                    if (enderecos == null)
                    {
                        continue;
                    }

                    sb.Append("<tr>");

                    sb.Append("<td>");
                    sb.Append(benef.Nome);
                    sb.Append("<br>");
                    sb.Append(enderecos[0].Logradouro);
                    if (!string.IsNullOrEmpty(enderecos[0].Numero)) { sb.Append(", "); sb.Append(enderecos[0].Numero); }
                    if (!string.IsNullOrEmpty(enderecos[0].Complemento)) { sb.Append(", "); sb.Append(enderecos[0].Complemento); }
                    sb.Append("<br>");
                    sb.Append(enderecos[0].Bairro); sb.Append(" - "); sb.Append(enderecos[0].Cidade);
                    sb.Append("<br>");
                    sb.Append(enderecos[0].UF); sb.Append(" - "); sb.Append(enderecos[0].CEP);
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    sb.Append("<tr height='8px'><td height='8px'>&nbsp;</td></tr>");

                    if (registro >= 10)
                    {
                        registro = 0;
                        sb.Append("</table>");
                        //quebra pagina
                        sb.Append("<div style=\"page-break-before: always;\"></div>");
                    }

                }

                sb.Append("</body></html>");

                string final = sb.ToString();
                pm.CloseSingleCommandInstance();
            }
        }
        void gerarArquivoEtiquetaLote_TEMP3()
        {
            busy = true;
            StringBuilder sb = new StringBuilder();
            Beneficiario benef = null;
            object beneficiarioId = null;
            IList<Endereco> enderecos = null;
            List<string> ids = new List<string>();

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>(
                    "* from cobranca where cobranca_arquivoUltimoEnvioId in (47)", typeof(Cobranca), pm);

                sb.Append("<html><body>");
                int registro = 0;
                foreach(var c in cobrancas)
                {
                    if (registro == 0)
                    {
                        sb.Append("<table width='100%'>");
                    }

                    beneficiarioId = ContratoBeneficiario.CarregaTitularID(c.PropostaID, pm);

                    if (ids.Contains(Convert.ToString(beneficiarioId))) continue;
                    ids.Add(Convert.ToString(beneficiarioId));

                    registro++;

                    benef = new Beneficiario(beneficiarioId);
                    pm.Load(benef);

                    enderecos = Endereco.CarregarPorDono(benef.ID, Endereco.TipoDono.Beneficiario, pm);

                    if (enderecos == null)
                    {
                        continue;
                    }

                    sb.Append("<tr>");

                    sb.Append("<td>");
                    sb.Append(benef.Nome);
                    sb.Append("<br>");
                    sb.Append(enderecos[0].Logradouro);
                    if (!string.IsNullOrEmpty(enderecos[0].Numero)) { sb.Append(", "); sb.Append(enderecos[0].Numero); }
                    if (!string.IsNullOrEmpty(enderecos[0].Complemento)) { sb.Append(", "); sb.Append(enderecos[0].Complemento); }
                    sb.Append("<br>");
                    sb.Append(enderecos[0].Bairro); sb.Append(" - "); sb.Append(enderecos[0].Cidade);
                    sb.Append("<br>");
                    sb.Append(enderecos[0].UF); sb.Append(" - "); sb.Append(enderecos[0].CEP);
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    sb.Append("<tr height='8px'><td height='8px'>&nbsp;</td></tr>");

                    if (registro >= 10)
                    {
                        registro = 0;
                        sb.Append("</table>");
                        //quebra pagina
                        sb.Append("<div style=\"page-break-before: always;\"></div>");
                    }

                }

                sb.Append("</body></html>");

                string final = sb.ToString();
                pm.CloseSingleCommandInstance();
            }
        }


        /***************************************************************/

        private void timerMeuResultadoEMAILS_Tick(object sender, EventArgs e)
        {
            try
            {
                System.Net.WebRequest request = System.Net.WebRequest.Create("http://www.meuresultado.com.br/enviaEmail.asp?appKey=WE2972131_12sa12GshxS1");
                System.Net.WebResponse response = request.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("iso-8859-1"));
                String html = sr.ReadToEnd();
                sr.Close();
                response.Close();
            }
            catch
            {
            }
        }

        void gerarCNAB_TEMP()
        {
            busy = true;
            //string arquivo = @"C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\cobrancas12.ret";
            StringBuilder sb = new StringBuilder();

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();


                IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>("* from cobranca where cobranca_arquivoUltimoEnvioId in (54) and cobranca_cancelada=0", typeof(Cobranca), pm);

                List<string> ids = new List<string>();
                foreach (var c in cobrancas) { ids.Add(Convert.ToString(c.ID)); }

                string conteudo = ArquivoCobrancaUnibanco.GeraDocumentoCobranca_SANTANDER___TEMP(ids, null, pm);
            }
        }


        void geraBoletosNAOrecorrentes_1aEmissao()
        {
            //string mdbPath = @"C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\boletos\agosto_dif.mdb";

            DataSet ds = new DataSet();
            //using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM ___2aEmissaoNaoDescontados", conn); //SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM ___dados", conn);
                adp.Fill(ds, "dados");
                adp.Dispose();
                conn.Close();
            }

            using (PersistenceManager pm = new PersistenceManager())
            {
                string casa_decimal = "";
                string valor_sem_decimal = "";
                string aux = "";
                object benefId = null, proposId = null;
                Cobranca cob = null;
                AdicionalBeneficiario ab = null;
                pm.BeginTransactionContext();

                List<string> naoachados = new List<string>();

                try
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        //benefId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculaAssociativa='" + row["sist"] + "'", null, null, pm);
                        benefId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculaFuncional='" + row["Funcional"] + "'", null, null, pm);

                        if (benefId == null || benefId == DBNull.Value)
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update ___2aEmissaoNaoDescontados set naoLocalizado=1 where id=" + row["id"], pm);
                            naoachados.Add(Convert.ToString(row["sist"])); continue;
                        }

                        proposId = LocatorHelper.Instance.ExecuteScalar("select contratobeneficiario_contratoId from contrato_beneficiario where contratobeneficiario_tipo=0 and contratobeneficiario_beneficiarioId=" + benefId, null, null, pm);

                        if (proposId == null || proposId == DBNull.Value)
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update ___2aEmissaoNaoDescontados set naoLocalizado=2 where id=" + row["id"], pm);
                            naoachados.Add(Convert.ToString(row["sist"])); continue;
                        }

                        //aux = Convert.ToString(row["valor"]);
                        //casa_decimal = aux.Substring(aux.Length - 2, 2);
                        //valor_sem_decimal = aux.Substring(0, aux.Length - 2);
                        //aux = valor_sem_decimal + "," + casa_decimal;
                        aux = Convert.ToString(row["ValorTratado"]);

                        ab = new AdicionalBeneficiario();
                        ab.AdicionalID = 87;
                        ab.BeneficiarioID = benefId;
                        ab.PropostaID = proposId;
                        ab.FormaPagto = AdicionalBeneficiario._FormaPagtoBoleto;
                        ab.Recorrente = false;
                        ab.ST_SG_1  = "A";
                        ab.Status01 = "A";
                        ab.Status   = "A";
                        ab.Valor01  = Convert.ToDecimal(aux);

                        pm.Save(ab);

                        cob = new Cobranca();
                        cob.Tipo = (int)Cobranca.eTipo.DiferencaUbraSP;
                        cob.Parcela = 0;
                        //cob.DataVencimento = Convert.ToDateTime(row["vencimento"]);
                        //cob.DataVencimento = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, cob.DataVencimento.Day, 23, 59, 59, 995);
                        cob.DataVencimento = new DateTime(2016, 09, 15, 23, 59, 59, 995);
                        cob.Valor = Convert.ToDecimal(aux);
                        cob.CobrancaRefID = null;
                        cob.DataPgto = DateTime.MinValue;
                        cob.ValorPgto = Decimal.Zero;
                        cob.Pago = false;
                        cob.PropostaID = proposId;
                        cob.Cancelada = false;
                        cob.ArquivoIDUltimoEnvio = -3;

                        pm.Save(cob);
                    }

                    pm.Commit();
                }
                catch
                {
                    pm.Rollback();
                }
                finally
                {
                    pm.Dispose();
                }
            }
        }
        void geraBoletosNAOrecorrentes()
        {
            //string mdbPath = @"C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\boletos\agosto_dif.mdb";

            DataSet ds = new DataSet();
            //using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM ___2aEmissaoNaoDescontados order by funcional", conn); //SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM ___dados", conn);
                adp.Fill(ds, "dados");
                adp.Dispose();
                conn.Close();
            }

            List<string> funcionais = new List<string>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (funcionais.Contains(Convert.ToString(row["Funcional"]))) continue;

                funcionais.Add(Convert.ToString(row["Funcional"]));
            }

            DataRow[] rows = null;
            using (PersistenceManager pm = new PersistenceManager())
            {
                decimal aux = 0;
                object benefId = null, proposId = null;
                Cobranca cob = null;
                AdicionalBeneficiario ab = null;
                pm.BeginTransactionContext();

                List<string> naoachados = new List<string>();

                try
                {
                    foreach(string func in funcionais)//foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        //benefId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculaAssociativa='" + row["sist"] + "'", null, null, pm);
                        benefId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculaFuncional='" + func + "'", null, null, pm);

                        if (benefId == null || benefId == DBNull.Value)
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update ___2aEmissaoNaoDescontados set naoLocalizado=1 where Funcional='" + func + "'", pm);
                            continue;
                        }

                        proposId = LocatorHelper.Instance.ExecuteScalar("select contratobeneficiario_contratoId from contrato_beneficiario where contratobeneficiario_tipo=0 and contratobeneficiario_beneficiarioId=" + benefId, null, null, pm);

                        if (proposId == null || proposId == DBNull.Value)
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update ___2aEmissaoNaoDescontados set naoLocalizado=2 where id='" + func + "'", pm);
                            continue;
                        }

                        //aux = Convert.ToString(row["valor"]);
                        //casa_decimal = aux.Substring(aux.Length - 2, 2);
                        //valor_sem_decimal = aux.Substring(0, aux.Length - 2);
                        //aux = valor_sem_decimal + "," + casa_decimal;
                        
                        aux = 0;
                        rows = ds.Tables[0].Select("Funcional='" + func + "'");

                        foreach (DataRow row in rows)
                        {
                            aux += Convert.ToDecimal(row["ValorTratado"]);
                        }

                        ab = new AdicionalBeneficiario();
                        ab.AdicionalID = 87;
                        ab.BeneficiarioID = benefId;
                        ab.PropostaID = proposId;
                        ab.FormaPagto = AdicionalBeneficiario._FormaPagtoBoleto;
                        ab.Recorrente = false;
                        ab.ST_SG_1 = "A";
                        ab.Status01 = "A";
                        ab.Status = "A";
                        ab.Valor01 = aux;

                        pm.Save(ab);

                        cob = new Cobranca();
                        cob.Tipo = (int)Cobranca.eTipo.DiferencaUbraSP;
                        cob.Parcela = 0;
                        //cob.DataVencimento = Convert.ToDateTime(row["vencimento"]);
                        //cob.DataVencimento = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, cob.DataVencimento.Day, 23, 59, 59, 995);
                        cob.DataVencimento = new DateTime(2016, 09, 15, 23, 59, 59, 995);
                        cob.Valor = Convert.ToDecimal(aux);
                        cob.CobrancaRefID = null;
                        cob.DataPgto = DateTime.MinValue;
                        cob.ValorPgto = Decimal.Zero;
                        cob.Pago = false;
                        cob.PropostaID = proposId;
                        cob.Cancelada = false;
                        cob.ArquivoIDUltimoEnvio = -3;

                        pm.Save(cob);
                    }

                    pm.Commit();
                }
                catch
                {
                    pm.Rollback();
                }
                finally
                {
                    pm.Dispose();
                }
            }
        }

        void verificaDadosProdesp()
        {
            DataTable dt = new DataTable();
            //using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM ___prodesp order by id", conn); //SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM ___dados", conn);
                adp.Fill(dt);
                adp.Dispose();
                conn.Close();

            }

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                string straux = "";
                object auxId = null, auxNome = null, auxFunc = null;

                foreach (DataRow row in dt.Rows)
                {
                    straux = Convert.ToString(row["IDENT"]).Substring(0, Convert.ToString(row["IDENT"]).Length - 1);

                    auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculaFuncional like '%" + straux + "%'", null, null, pm);

                    if (auxId == null || auxId == DBNull.Value) continue;

                    auxNome = LocatorHelper.Instance.ExecuteScalar("select beneficiario_nome from beneficiario where beneficiario_id = " + auxId, null, null, pm);

                    auxFunc = LocatorHelper.Instance.ExecuteScalar("select beneficiario_matriculaFuncional from beneficiario where beneficiario_id = " + auxId, null, null, pm);

                    NonQueryHelper.Instance.ExecuteNonQuery(
                        string.Concat("update ___prodesp set sysid=", auxId, ", sysnome='", auxNome, "', sysfuncional='", auxFunc, "' where id=", row["id"]),
                        pm);
                }

                pm.CloseSingleCommandInstance();
            }
        }

        void batimentoCobrancas()
        {
            FileStream stream = File.Open(@"C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\batimento\PAGAMENTO NOVEMBRO TOTAL.xls", FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader = null;
            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            DataSet ds = excelReader.AsDataSet();
            stream.Close(); stream.Dispose();

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                IList<Cobranca> cob = null;

                int i = -1;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    i++;

                    if (i == 0) continue;

                    Batimento b = new Batimento();

                    b.Beneficiario = CToString(row[3]);
                    b.NossoNumero = CToString(row[0]);
                    b.Situacao = CToString(row[7]);
                    b.Valor = CToString(row[5]);
                    b.Vencimento = CToString(row[6]);

                    cob = LocatorHelper.Instance.ExecuteQuery<Cobranca>("cobranca.*,beneficiario_nome from cobranca left join contrato_beneficiario on contratobeneficiario_contratoId = cobranca_propostaId and contratobeneficiario_tipo=0 left join beneficiario on contratobeneficiario_beneficiarioId = beneficiario_id and contratobeneficiario_tipo=0 where cobranca_nossoNumero='" + b.NossoNumero.Split('-')[0] + "'", typeof(Cobranca), pm);

                    if (cob == null)
                    {
                        b.Status = "NAO LOCALIZADO";
                        pm.Save(b);
                    }
                    else
                    {
                        if (cob[0].Pago)
                            b.Status = "COBRANCA JA PAGA";
                        else
                            b.Status = "COBRANCA NAO PAGA";

                        b.BeneficiarioLocalizado = Convert.ToString(cob[0].BeneficiarioNome);
                        b.CobrancaId = Convert.ToString(cob[0].ID);
                        pm.Save(b);
                    }
                }

                pm.CloseSingleCommandInstance();
            }
        }
        void batimentoCobrancasBAIXA()
        {
            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();
                IList<Batimento> batimentos = LocatorHelper.Instance.ExecuteQuery<Batimento>("* from ___batimento where status <> 'COBRANCA JA PAGA'", typeof(Batimento), pm);

                //batimentos = new List<Batimento>();
                //batimentos.Add(new Batimento { CobrancaId = "2662" });
                //batimentos.Add(new Batimento { CobrancaId = "2663" });
                //batimentos.Add(new Batimento { CobrancaId = "2672" });
                //batimentos.Add(new Batimento { CobrancaId = "2674" });
                //batimentos.Add(new Batimento { CobrancaId = "2676" });
                //batimentos.Add(new Batimento { CobrancaId = "2677" });
                //batimentos.Add(new Batimento { CobrancaId = "2694" });
                //batimentos.Add(new Batimento { CobrancaId = "3103" });
                //batimentos.Add(new Batimento { CobrancaId = "2668" });
                //batimentos.Add(new Batimento { CobrancaId = "2670" });
                //batimentos.Add(new Batimento { CobrancaId = "2671" });
                //batimentos.Add(new Batimento { CobrancaId = "2680" });
                //batimentos.Add(new Batimento { CobrancaId = "2659" });
                //batimentos.Add(new Batimento { CobrancaId = "2664" });
                //batimentos.Add(new Batimento { CobrancaId = "2686" });

                DateTime data = DateTime.Now;
                Cobranca cob = null;
                CobrancaBaixa baixa = null;
                foreach (var bat in batimentos)
                {
                    if (string.IsNullOrEmpty(bat.CobrancaId)) continue;

                    cob = new Cobranca(bat.CobrancaId);
                    pm.Load(cob);

                    if (!cob.Pago)
                    {
                        cob.Pago = true;
                        cob.DataPgto = data;
                        cob.ValorPgto = cob.Valor;
                        pm.Save(cob);

                        baixa = new CobrancaBaixa();
                        baixa.BaixaFinanceira = true;
                        baixa.BaixaProvisoria = false;
                        baixa.CobrancaID = cob.ID;
                        baixa.Data = data;
                        baixa.MotivoID = 1;
                        baixa.Obs = "Batimento realizado em " + DateTime.Now.ToString("dd/MM/yyyy");
                        baixa.Tipo = 0; //baixa
                        baixa.UsuarioID = 1;
                        pm.Save(baixa);
                    }
                }
            }
        }

        [DBTableAttribute("___batimento")]
        class Batimento : IPersisteableEntity
        {
            [DBFieldInfo("id", FieldType.PrimaryKeyAndIdentity)]
            public Object ID { get; set; }

            [DBFieldInfo("cobranca_id", FieldType.Single)]
            public string CobrancaId { get; set; }

            [DBFieldInfo("nosso_numero", FieldType.Single)]
            public string NossoNumero { get; set; }

            [DBFieldInfo("beneficiario", FieldType.Single)]
            public string Beneficiario { get; set; }

            [DBFieldInfo("beneficiario_localizado", FieldType.Single)]
            public string BeneficiarioLocalizado { get; set; }

            [DBFieldInfo("valor", FieldType.Single)]
            public string Valor { get; set; }

            [DBFieldInfo("vencimento", FieldType.Single)]
            public string Vencimento { get; set; }

            [DBFieldInfo("situacao", FieldType.Single)]
            public string Situacao { get; set; }

            [DBFieldInfo("status", FieldType.Single)]
            public string Status { get; set; }
        }

        void arrumaEnd()
        {

        }


        void enviaIR()
        {
            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                IList<Contrato> contratos = LocatorHelper.Instance.ExecuteQuery<Contrato>("select contrato_id from contrato where contrato_inativo=0 and contrato_cancelado=0and contrato_legado=0", typeof(Contrato), pm);

                foreach (var c in contratos)
                {
                    enviaEmailIR(c.ID, pm);
                    System.Threading.Thread.Sleep(2000);
                }
            }
        }

        protected void enviaEmailIR(object contratoId, PersistenceManager pm)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            String ano = (DateTime.Now.Year - 1).ToString(); 

            #region corpo do e-mail

            Contrato contrato = Contrato.CarregarParcial((Object)contratoId, pm);
            Operadora operadora = new Operadora(contrato.OperadoraID);
            pm.Load(operadora);

            ContratoBeneficiario cTitular = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);

            if (String.IsNullOrEmpty(cTitular.BeneficiarioEmail)) { return; }

            sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
            sb.Append("<head><title></title>");
            sb.Append("<style type='text/css'>body, html{ font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:#b8b8b8; font-size:11px; background-color:white; margin:0px; height:100%; }link              { font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:blue; font-size:11px; } table             { font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:black; font-size:11px; }</style>");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append("<table align=\"center\" width=\"95%\">");
            sb.Append(" <tr>");
            sb.Append(" <td>");
            sb.Append("<table>");
            sb.Append("<tr><td align='center' colspan=\"2\"><h2>Demonstrativo de Pagamentos "); sb.Append(ano); sb.Append("</td><td width='35'>&nbsp;</td><td align='left'><img align=\"right\" src='http://www.linkecerebro.com.br/LogoMail2.png' width='149px' height='89px'/></h2></td></tr>");
            sb.Append("</table>");
            sb.Append("<table style=\"font-size:12px\">");
            sb.Append(" <tr>");
            sb.Append(" <td colspan=\"2\">");
            sb.Append(String.Concat("São Paulo, ", DateTime.Now.Day, " de ", DateTime.Now.ToString("MMMM"), " de ", DateTime.Now.Year, "."));
            sb.Append(" </td>");
            sb.Append(" </tr>");
            sb.Append(" <tr><td height='5px'></td></tr>");

            if (!string.IsNullOrEmpty(contrato.ResponsavelNome) && !string.IsNullOrEmpty(contrato.ResponsavelCPF))
            {
                sb.Append(" <tr><td width=\"140px\"><b>Ilmo(a). Senhor(a)</b></td>"); sb.Append("<td>"); sb.Append(contrato.ResponsavelNome); sb.Append("</td></tr>");
                sb.Append(" <tr><td><b>CPF:</b></td><td>"); sb.Append(contrato.ResponsavelCPF); sb.Append("</td></tr>");
            }
            else
            {
                sb.Append(" <tr><td width=\"140px\"><b>Ilmo(a). Senhor(a)</b></td>"); sb.Append("<td>"); sb.Append(cTitular.BeneficiarioNome); sb.Append("</td></tr>");
                sb.Append(" <tr><td><b>CPF:</b></td><td>"); sb.Append(cTitular.BeneficiarioCPF); sb.Append("</td></tr>");
            }

            sb.Append("</table><br />");

            sb.Append("<table style=\"font-size:12px\"><tr><td><b>Beneficiário(a) UBRASP,</b></td></tr><tr><td height='8'></td></tr><tr><td>");
            sb.Append("Abaixo o demonstrativo de pagamentos efetuados, durante o ano calendário de "); sb.Append(ano); sb.Append(", à UBRASP ");
            sb.Append("Administradora de Benefícios LTDA., inscrita no CNPJ/MF sob o nº 49.938.327/0001-06, e destinados à ");
            sb.Append("manutenção do plano privado de assistência à saúde, coletivo por adesão, por meio de contrato coletivo ");
            sb.Append("firmado com a operadora [operadora].<br />");
            sb.Append("Esse demonstrativo relaciona as despesas médicas que foram pagas pelo(a) Sr(a). e que são dedutíveis em ");
            sb.Append("Declaração de Imposto de Renda.");
            sb.Append("</td></tr></table></td></tr></table><br />");

            #region MESES

            Decimal total = 0, totalJan = 0, totalFev = 0, totalMar = 0, totalAbr = 0, totalMaio = 0, totalJun = 0, totalJul = 0, totalAgo = 0, totalSet = 0, totalOut = 0, totalNov = 0, totalDez = 0;

            sb.Append("<table align='center' cellpadding=\"4\" cellspacing=\"0\" style=\"border:solid 1px black;font-size:12px\" width=\"400px\"><tr><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Competência</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>*Valor mensal</b></td></tr>");

            IList<Cobranca> cobrancas = Cobranca.CarregarPagasDoAno(contratoId, Convert.ToInt32(ano), pm);

            if (cobrancas == null || cobrancas.Count == 0)
            {
                return;
            }

            foreach (Cobranca cob in cobrancas)
            {
                switch (cob.DataVencimento.Month)
                {
                    case 1:
                    {
                        totalJan += cob.Valor;
                        total += cob.Valor;
                        break;
                    }
                    case 2:
                    {
                        totalFev += cob.Valor;
                        total += cob.Valor;
                        break;
                    }
                    case 3:
                    {
                        totalMar += cob.Valor;
                        total += cob.Valor;
                        break;
                    }
                    case 4:
                    {
                        totalAbr += cob.Valor;
                        total += cob.Valor;
                        break;
                    }
                    case 5:
                    {
                        totalMaio += cob.Valor;
                        total += cob.Valor;
                        break;
                    }
                    case 6:
                    {
                        totalJun += cob.Valor;
                        total += cob.Valor;
                        break;
                    }
                    case 7:
                    {
                        totalJul += cob.Valor;
                        total += cob.Valor;
                        break;
                    }
                    case 8:
                    {
                        totalAgo += cob.Valor;
                        total += cob.Valor;
                        break;
                    }
                    case 9:
                    {
                        totalSet += cob.Valor;
                        total += cob.Valor;
                        break;
                    }
                    case 10:
                    {
                        totalOut += cob.Valor;
                        total += cob.Valor;
                        break;
                    }
                    case 11:
                    {
                        totalNov += cob.Valor;
                        total += cob.Valor;
                        break;
                    }
                    case 12:
                    {
                        totalDez += cob.Valor;
                        total += cob.Valor;
                        break;
                    }
                }
            }

            sb.Append("<tr><td>Janeiro</td><td>");
            sb.Append(totalJan.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Fevereiro</td><td>");
            sb.Append(totalFev.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Março</td><td>");
            sb.Append(totalMar.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Abril</td><td>");
            sb.Append(totalAbr.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Maio</td><td>");
            sb.Append(totalMaio.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Junho</td><td>");
            sb.Append(totalJun.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Julho</td><td>");
            sb.Append(totalJul.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Agosto</td><td>");
            sb.Append(totalAgo.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Setembro</td><td>");
            sb.Append(totalSet.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Outubro</td><td>");
            sb.Append(totalOut.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Novembro</td><td>");
            sb.Append(totalNov.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Dezembro</td><td>");
            sb.Append(totalDez.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>*Valor total</td><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>" + total.ToString("N2") + "</td></tr>");

            sb.Append("<tr><td colspan=\"2\" style=\"border-top:solid 1px black;font-size:11px\"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td></tr></table>");
            sb.Append("<br /><br />");

            #endregion MESES

            sb.Append("<table align=\"center\" width=\"95%\"><tr><td>Atenção: Caso este informe seja utilizado para fins de declaração de Imposto de Renda, esclarecemos que somente podem ser deduzidas as parcelas relativas ao contribuinte e aos dependentes devidamente relacionados na própria declaração. As deduções estão sujeitas às regras estabelecidas pela legislação que regulamenta o imposto (Decreto nº 3.000/99).</td></tr><tr><td height='8'></td></tr><tr><td><b>UBRASP Administradora de Benefícios</b></td></tr></table>");

            sb.Append("<br><br><font size='1' color='red'>Este é um e-mail automático. Por favor não o responda.</font>");
            sb.Append("</body>");
            sb.Append("</html>");


            String corpo = sb.ToString();

            if (operadora.Nome.IndexOf("-") > -1)
                corpo = corpo.Replace("[operadora]", operadora.Nome.Split('-')[1].Trim());
            else
                corpo = corpo.Replace("[operadora]", operadora.Nome.Trim());

            #endregion corpo do e-mail

            try
            {
                string _mailToken = "d1352873-cd5f-4374-98ca-1912ec02333a";

                MailServASMX.mail proxy = new MailServASMX.mail();
                string ret = proxy.EnviaEmail(
                    ConfigurationManager.AppSettings["mailFrom"],
                    ConfigurationManager.AppSettings["mailFromName"],
                    "atendimentorj@ubrasp.com.br",
                    cTitular.BeneficiarioEmail, //"denis@wedigi.com.br",
                    "[UBRASP] Atendimento - Informe",
                    corpo,
                    true,
                    "",
                    _mailToken);
                proxy.Dispose();

            }
            catch { }
        }



        void arrumaCobrancasErradas()
        {
            IList<Cobranca> cobrancas = null;
            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                ////vig 01 - vencimento dia 01
                //var contratos = LocatorHelper.Instance.ExecuteQuery<Contrato>("contrato_id from contrato where contrato_contratoadmid=413", typeof(Contrato), pm);
                //foreach (Contrato c in contratos)
                //{
                //    //carrega cobrancas
                //    cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>("* from cobranca where cobranca_pago=0 and cobranca_cancelada=0 and cobranca_propostaid=" + c.ID, typeof(Cobranca), pm);

                //    foreach (Cobranca cobr in cobrancas)
                //    {
                //        cobr.DataVencimento = new DateTime(cobr.DataVencimento.Year, cobr.DataVencimento.Month, 1, 23, 59, 59, cobr.DataVencimento.Millisecond);
                //        pm.Save(cobr);
                //    }
                //}

                //vig 10 - vencimento dia 10
                var contratos = LocatorHelper.Instance.ExecuteQuery<Contrato>("contrato_id from contrato where contrato_contratoadmid=414", typeof(Contrato), pm);
                foreach (Contrato c in contratos)
                {
                    //carrega cobrancas
                    cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>("* from cobranca where cobranca_pago=0 and cobranca_cancelada=0 and cobranca_propostaid=" + c.ID, typeof(Cobranca), pm);

                    foreach (Cobranca cobr in cobrancas)
                    {
                        cobr.DataVencimento = new DateTime(cobr.DataVencimento.Year, cobr.DataVencimento.Month, 10, 23, 59, 59, cobr.DataVencimento.Millisecond);
                        pm.Save(cobr);
                    }
                }
            }
        }
    }
}
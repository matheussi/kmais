namespace www.UBRASP.arquivos.prodesp
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using System.Data.OleDb;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Facade;
    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Entity.ArquivoUbrasp;
    using Excel;

    public partial class processar : PageBase
    {
        List<ItemVO> Boletos
        {
            get { return ViewState["bolet"] as List<ItemVO>; }
            set { ViewState["bolet"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtVencimento.Text = string.Concat("15/", DateTime.Now.Month, "/", DateTime.Now.Year);
            }
        }

        protected void cmdEnviar_Click(object sender, EventArgs e)
        {
            string arquivoNome = "";

            if (fuArquivo.PostedFile != null && !string.IsNullOrEmpty(fuArquivo.PostedFile.FileName))
            {
                if (Path.GetExtension(fuArquivo.PostedFile.FileName).ToUpper() != ".TXT")
                {
                    return;
                }

                string diretorioBase = String.Concat(Server.MapPath("/"), @"var\prodesp\");

                if (!Directory.Exists(diretorioBase)) Directory.CreateDirectory(diretorioBase);

                arquivoNome = string.Concat(diretorioBase, "prodesp", Path.GetExtension(fuArquivo.PostedFile.FileName));

                if (File.Exists(arquivoNome)) File.Delete(arquivoNome);

                fuArquivo.PostedFile.SaveAs(arquivoNome);
            }
            else
                return;

            string[] linhas = File.ReadAllLines(arquivoNome);

            List<ItemVO> vos = this.processaArquivoEnviado_txt(linhas);

            //TODO: denis, descomentar linha abaixo
            this.processaBeneficiariosQueNaoEstaoNoArquivo(ref vos);

            grid.DataSource = vos;
            grid.DataBind();

            this.Boletos = vos;
        }

        string[] obtemLinhas(string[] linhas, string nome)
        {
            List<string> linhasRetornadas = new List<string>();

            for (int index = 0; index < linhas.Length; index++)
            {
                if (index == 0) continue; //ignora cabecalho do arquivo
                if (linhas[index].Trim() == "") continue;

                if (nome.PadRight(30, ' ') == linhas[index].Substring(0, 30))
                    linhasRetornadas.Add(linhas[index]);
            }

            return linhasRetornadas.ToArray();
        }
        string obtemFuncional(string[] linhas, string nome)
        {
            string[] _linhas = obtemLinhas(linhas, nome);

            return _linhas[0].Substring(35, 7);
        }
        decimal obtemValor(string[] linhas, string nome, System.Globalization.CultureInfo cinfo)
        {
            string[] _linhas = obtemLinhas(linhas, nome);

            decimal total = 0;

            string aux = "";
            foreach (string linha in _linhas)
            {
                aux = string.Concat(
                    linha.Substring(87, 10), ",", linha.Substring(97, 2));

                total += Convert.ToDecimal(aux, cinfo);

                aux = string.Concat(
                    linha.Substring(104, 10), ",", linha.Substring(114, 2));

                total += Convert.ToDecimal(aux, cinfo);
            }

            return total;
        }

        List<ItemVO> processaArquivoEnviado_txt(string[] linhas)
        {
            decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M;
            DateTime competencia = base.CStringToDateTime(txtVencimento.Text, "23:59", 59, 995).AddMonths(-1);

            List<string> nomes = new List<string>();
            for (int index = 0; index < linhas.Length; index++)
            {
                if (index == 0) continue; //ignora cabecalho do arquivo
                if (linhas[index].Trim() == "") continue;

                if (nomes.Contains(linhas[index].Substring(0, 30).Trim())) continue;
                nomes.Add(linhas[index].Substring(0, 30).Trim());
            }

            List<ItemVO> vos = new List<ItemVO>();

            //int i = -1;
            string funcional = ""; // straux = "";
            object auxId = null, auxNome = null;

            decimal valorTotal = 0, valorSemReajuste = 0; //, valorSeguradoras = 0;
            bool temPlano = false, nomeForcado = false;
            DateTime vigencia = new DateTime(1850, 01, 01);

            AdicionalBeneficiario ab = null;
            DataTable dtAdicionais = new DataTable();

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();
                //NonQueryHelper.Instance.ExecuteNonQuery("update __rspp set mesano=null", pm); //nao pode ser feito

                foreach (string nome in nomes)
                {
                    //i++;
                    //if (i == 0) continue;

                    valorSemReajuste = 0;
                    nomeForcado = false;

                    ItemVO vo = new ItemVO();

                    vo.Residual = 0;
                    vo.ValorSistema = 0;

                    vo.Funcional = obtemFuncional(linhas, nome);
                    vo.Nome = nome;
                    vo.ValorArquivo = obtemValor(linhas, nome, cinfo).ToString("N2", cinfo);

                    funcional = vo.Funcional.Substring(0, vo.Funcional.Length - 1); //remove o utlimo caractere, pois é usado somente pela Prodesp internamente

                    if (nome.ToUpper().Trim() == "MARIA LUCIA DE FREITAS")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 12971, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "SANDRA REGINA DA SILVA SOUSA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 13366, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "MARIA CARMELITA SILVA"){
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 35837, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "DOLARICE ROSA DE OLIVEIRA"){
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 39240, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "GELNI MOURA DOS SANTOS"){
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 39221, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "BLANCA ELEONORA DE CAMARGO"){
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 39337, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "DALVA OLIVEIRA PEIXOTO ROMEIRO"){
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 39895, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "ROSEVANIA DOMINGUES P SILVA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 37718, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "DINAH DE OLIVEIRA MATIAS")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 35065, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "AMENOEL DIAS AQUINO")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 32627, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "DIRCE EUGENIO DA SILVA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 34067, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "ELENICE DOS SANTOS RIBEIRO")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 32637, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "EDUARDO RIBEIRO BARBOSA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 34420, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "JOAO LUIZ DA SILVA GUIMARAES")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 34548, null, null, pm);
                        nomeForcado = true;
                    }
                    else
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_matriculafuncional like '%" + funcional + "%'", null, null, pm);

                    if (auxId == null || auxId == DBNull.Value)
                    {
                        vo.Status = "Nao Localizado";
                        vos.Add(vo);
                        continue;
                    }

                    vo.BeneficiarioId = Convert.ToString(auxId);

                    #region Aqui deve manipular as tabelas RSPP e Chubb //////////////////////////////////////////////////////////

                    //RSPP/////////////
                    if (CToDecimal(vo.ValorArquivo) > 0)
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select id from __rspp where (beneficiarioID=", vo.BeneficiarioId, " or beneficiarioID2=", vo.BeneficiarioId, ")"),
                            null, null, pm);

                        if (auxId != null && auxId != DBNull.Value)
                        {
                            //valorSeguradoras = CToDecimal(LocatorHelper.Instance.ExecuteScalar(
                            //    string.Concat("select R_VALOR from __rspp id=", auxId),
                            //    null, null, pm), cinfo);

                            //valorSeguradoras += CToDecimal(vo.ValorArquivo, cinfo);

                            //NonQueryHelper.Instance.ExecuteNonQuery(
                            //    string.Concat("update __rspp set mesano='", competencia.ToString("MMyyyy"), "', R_VALOR='", valorSeguradoras.ToString("N2", cinfo).Replace(".", "").Replace(",", "."), "' where id=", auxId),
                            //    pm);

                            NonQueryHelper.Instance.ExecuteNonQuery(
                                string.Concat("update __rspp set mesano='", competencia.ToString("MMyyyy"), "' where id=", auxId),
                                pm);
                        }
                    }

                    ///////////////////
                    //Chubb////////////
                    if (CToDecimal(vo.ValorArquivo) > 0)
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select id from __chubb_envio2016 where beneficiarioID=", vo.BeneficiarioId),
                            null, null, pm);

                        if (auxId != null && auxId != DBNull.Value)
                        {
                            //valorSeguradoras = CToDecimal(LocatorHelper.Instance.ExecuteScalar(
                            //    string.Concat("select R_VALOR from __rspp id=", auxId),
                            //    null, null, pm), cinfo);

                            //valorSeguradoras += CToDecimal(vo.ValorArquivo, cinfo);

                            //NonQueryHelper.Instance.ExecuteNonQuery(
                            //    string.Concat("update __rspp set mesano='", competencia.ToString("MMyyyy"), "', R_VALOR='", valorSeguradoras.ToString("N2", cinfo).Replace(".", "").Replace(",", "."), "' where id=", auxId),
                            //    pm);

                            NonQueryHelper.Instance.ExecuteNonQuery(
                                string.Concat("update __chubb_envio2016 set competencia='", competencia.ToString("MMyyyy"), "' where id=", auxId),
                                pm);
                        }
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #endregion

                    //else
                    //{
                        auxId = vo.BeneficiarioId;
                        auxNome = LocatorHelper.Instance.ExecuteScalar("select beneficiario_nome from beneficiario where beneficiario_id = " + auxId, null, null, pm);

                        if (Convert.ToString(auxNome).ToUpper() != vo.Nome.ToUpper() && !nomeForcado)
                        {
                            vo.Status = "Nao Localizado";
                            vo.NomeLocalizado = Convert.ToString(auxNome);
                        }
                        //else
                        //{
                        vo.NomeLocalizado = Convert.ToString(auxNome);

                        if (nomeForcado) { vo.NomeLocalizado = nome; vo.Status = ""; }

                        //verifica se tem planos
                        dtAdicionais.Rows.Clear();
                        dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid where adicionalbeneficiario_recorrente=1 and adicionalbeneficiario_beneficiarioid=" + auxId, "result", pm).Tables[0];

                        vo.BeneficiarioId = CToString(auxId);
                        valorTotal = 0;
                        temPlano = false;

                        //verifica contrato cancelado
                        auxId = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contrato_id = contratobeneficiario_contratoId where contrato_cancelado=0 and contrato_inativo=0 and contratobeneficiario_beneficiarioId = " + vo.BeneficiarioId, null, null, pm);
                        if (auxId == null || auxId == DBNull.Value)
                        {
                            vo.Marcado = false;
                            continue;
                        }
                        else
                            vo.ContratoId = Convert.ToString(auxId);

                        //primeiro verifica se tem plano de saude (se nao tivar nao continua.
                        foreach (DataRow rowAd in dtAdicionais.Rows)
                        {
                            ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                            if (ab == null) continue;
                            if (!ab.Recorrente) continue;

                            if (base.CToInt(ab.AdicionalCodigo) >= 4435 && base.CToInt(ab.AdicionalCodigo) <= 4442) { temPlano = true; break; }
                        }

                        if (temPlano)
                        {
                            foreach (DataRow rowAd in dtAdicionais.Rows)
                            {
                                ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                                if (ab == null) continue;
                                if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                                valorSemReajuste += ab.RetornaValor(-1, vigencia); //AdicionalBeneficiario._FormaPagtoBoleto

                                if (!ab.Atualizado)
                                {
                                    ab.AplicaRejuste(reajustePlano, reajusteTaxa, reajusteSeguro);
                                    ab.Atualizado = true;

                                    if(nomeForcado || string.IsNullOrEmpty(vo.Status))
                                    {
                                        pm.Save(ab);
                                    }
                                }

                                if (ab.AdicionalTipo == 0) //somente taxas e planos
                                {
                                    valorTotal += ab.RetornaValor(-1, vigencia);
                                }
                            }

                            vo.Marcado = true;
                            vo.ValorSistema = valorTotal;
                            vo.Residual = valorTotal - Convert.ToDecimal(vo.ValorArquivo, cinfo);
                            if (vo.Residual <= 2) vo.Marcado = false;////////////
                        }
                        else
                        {
                            vo.Marcado = false; ///////////////
                            if (string.IsNullOrEmpty(vo.Status))
                                vo.Status = "Nao possui plano";
                            else
                                vo.Status += " - Nao possui plano";
                        }
                        //}
                    //}

                    vos.Add(vo);
                }
            }

            return vos;
        }

        bool jaProcessado(DataRow row, List<ItemVO> colecao, ref List<ItemVO> naoMencionadosNoArquivo)
        {
            //if (colecao == null || colecao.Count == 0) return false;
            //if (Convert.ToString(row["beneficiario_nome"]).ToUpper().Contains("MARLI RODRIGUES SILVA")) { int j = 0; }

            foreach (var vo in colecao)
            {

                if (vo.BeneficiarioId == Convert.ToString(row["beneficiario_id"]))
                    return true;
            }

            ItemVO novo = new ItemVO();

            novo.BeneficiarioId = Convert.ToString(row["beneficiario_id"]);
            novo.ContratoId = Convert.ToString(row["contrato_id"]);
            novo.Nome = Convert.ToString(row["beneficiario_nome"]);
            //novo.CPF = base.CToString(row[0]).PadLeft(11, '0');
            novo.Residual = 0;
            novo.ValorSistema = 0;
            novo.Funcional = base.CToString(row["beneficiario_matriculaFuncional"]);
            //novo.ORGAO = base.CToString(row["contratoadm_descricao"]);
            //novo.Parcela = "0";
            novo.Marcado = false;
            //novo.ValorDescontado = 0;
            novo.ValorArquivo = "0";
            //novo.Atualizado = "SIM";

            naoMencionadosNoArquivo.Add(novo);

            return false;
        }

        void processaBeneficiariosQueNaoEstaoNoArquivo(ref List<ItemVO> vos)
        {
            DateTime vencimento = base.CStringToDateTime(txtVencimento.Text, "23:59", 59, 995);  //new DateTime(2016, 12, 15, 23, 59, 59, 995);
            if (vencimento == DateTime.MinValue)
            {
                return;
            }

            if (vos == null) vos = new List<ItemVO>();

            List<ItemVO> naoMencionadosNoArquivo = new List<ItemVO>();
            Beneficiario beneficiario = null;
            DataTable dtAdicionais = new DataTable();
            decimal valorTotal = 0;
            bool temPlano = false;
            AdicionalBeneficiario ab = null;

            decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M;
            string statusSemPlano = "NAO TEM PLANO";
            string statusPrividen = "TEM PREVIDENCIA";
            string statusSeguro = "TEM SEGURO";
            string statusSemDeb = "NENHUM DEBITO";
            List<string> ids = new List<string>();
            object aux = null;

            DateTime vigencia = new DateTime(1850, 01, 01);

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                string sql = string.Concat(
                    "select contrato_id,contrato_numero,beneficiario_id,beneficiario_cpf,beneficiario_nome,beneficiario_matriculaFuncional,contratoadm_descricao ",
                    "   from contrato ",
                    "       inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1",
                    "       inner join beneficiario on contratobeneficiario_beneficiarioId = beneficiario_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1",
                    "       inner join contratoadm on contrato_contratoadmId = contratoadm_id ",
                    "where ",
                    "   contrato_inativo<>1 and contrato_cancelado<>1 and contrato_rascunho <> 1 and contrato_legado=1 ",
                    "   and contrato_id in (select adicionalbeneficiario_propostaId from adicional_beneficiario inner join adicional on adicionalbeneficiario_adicionalid=adicional_id where adicionalbeneficiario_propostaId = contrato.contrato_id and adicional_codigo >= 4435 and adicional_codigo <= 4442) ",
                    "   and contrato_id not in (select cobranca_propostaId from cobranca where cobranca_cancelada=0 and cobranca_propostaId=contrato.contrato_id and month(cobranca_datavencimento)=", vencimento.Month, " and year(cobranca_datavencimento)=", vencimento.Year, ") ",
                    "   and contrato_contratoadmId in (40,45) "); //somente quem FOR do tribunal de justica e assembleia

                DataTable dt = LocatorHelper.Instance.ExecuteQuery(sql, "result", pm).Tables[0];

                if (dt == null || dt.Rows == null || dt.Rows.Count == 0) { pm.CloseSingleCommandInstance(); return; }

                foreach (DataRow row in dt.Rows)
                {
                    //if (jaProcessado(row, jaProcessados, ref naoMencionadosNoArquivo)) continue;
                    if (jaProcessado(row, vos, ref naoMencionadosNoArquivo)) continue;
                }

                dt.Dispose();

                if (naoMencionadosNoArquivo.Count > 0)
                {
                    foreach (var vo in naoMencionadosNoArquivo)
                    {

                        if (ids.Contains(vo.BeneficiarioId)) continue;

                        beneficiario = new Beneficiario(vo.BeneficiarioId);
                        pm.Load(beneficiario);

                        //carrega adicionais 
                        dtAdicionais.Rows.Clear();
                        dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid where adicionalbeneficiario_recorrente=1 and adicionalbeneficiario_beneficiarioid=" + vo.BeneficiarioId, "result", pm).Tables[0]; //adicionalbeneficiario_recorrente=1 and 

                        valorTotal = 0;
                        temPlano = false;

                        foreach (DataRow rowAd in dtAdicionais.Rows)
                        {
                            ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                            if (ab == null) continue;
                            if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                            if (ab.Atualizado == false)
                            {
                                //vo.Atualizado = "NAO";
                                ab.AplicaRejuste(reajustePlano, reajusteTaxa, reajusteSeguro);
                                ab.Atualizado = true;
                                //pm.Save(ab);
                            }

                            valorTotal += ab.RetornaValor(AdicionalBeneficiario._FormaPagtoDescFolha, vigencia);

                            if (Convert.ToInt32(ab.AdicionalCodigo) >= 4435 && Convert.ToInt32(ab.AdicionalCodigo) <= 4442) temPlano = true;
                        }

                        vo.ValorSistema = valorTotal;

                        if (!temPlano)
                        {
                            vo.Status = statusSemPlano;
                            vo.Marcado = false;
                            //vos.Add(vo);
                            continue;
                        }

                        vo.Residual = vo.ValorSistema; //vo.ValorDescontado; //vo.ValorArquivo;
                        if (vo.Residual > 1) vo.Marcado = true;
                        else
                        {
                            vo.Marcado = false;
                            vo.Status = statusSemDeb;
                            //vos.Add(vo);
                            continue;
                        }

                        #region verifica previdencia e seguro

                        if (!string.IsNullOrEmpty(beneficiario.MatriculaFuncional))
                        {
                            //verifica se tem previdencia. se tiver, nao deve enviar boleto. 
                            aux = LocatorHelper.Instance.ExecuteScalar("select id from ___previdencia where dados like '%" + beneficiario.MatriculaFuncional + "%'", null, null, pm);
                            if (aux != null && aux != DBNull.Value)
                            {
                                vo.Status = statusPrividen;
                                vo.Marcado = false;
                                //vos.Add(vo);
                                continue;
                            }

                            //verifica se tem seguro. se tiver, nao deve enviar boleto. 
                            aux = LocatorHelper.Instance.ExecuteScalar("select id from ___seguros where mat_ass like '%" + beneficiario.MatriculaAssociativa + "%'", null, null, pm);
                            if (aux != null && aux != DBNull.Value)
                            {
                                vo.Status = statusSeguro;
                                vo.Marcado = false;
                                //vos.Add(vo);
                                continue;
                            }
                        }
                        #endregion

                        vo.Status = "OK";

                        vos.Add(vo);

                        if (vo.Status == "OK")
                        {
                            ids.Add(vo.BeneficiarioId);
                        }
                    }
                }
            }
        }


        /******************************************************************/

        protected void cmdEnviar_xlsx_Click(object sender, EventArgs e)
        {
            string arquivoNome = "";

            if (fuArquivo.PostedFile != null && !string.IsNullOrEmpty(fuArquivo.PostedFile.FileName))
            {
                if (Path.GetExtension(fuArquivo.PostedFile.FileName).ToUpper() != ".XLSX" &&
                    Path.GetExtension(fuArquivo.PostedFile.FileName).ToUpper() != ".XLS")
                {
                    return;
                }

                string diretorioBase = String.Concat(Server.MapPath("/"), @"var\prodesp\");

                if (!Directory.Exists(diretorioBase)) Directory.CreateDirectory(diretorioBase);

                arquivoNome = string.Concat(diretorioBase, "prodesp", Path.GetExtension(fuArquivo.PostedFile.FileName));

                if (File.Exists(arquivoNome)) File.Delete(arquivoNome);

                fuArquivo.PostedFile.SaveAs(arquivoNome);
            }
            else
                return;

            FileStream stream = File.Open(arquivoNome, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader = null;

            if (Path.GetExtension(arquivoNome).ToUpper() == ".XLSX")
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            else
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            DataSet ds = excelReader.AsDataSet();

            stream.Close(); stream.Dispose();

            List<ItemVO> vos = this.processaPlanilhaEnviada(ds.Tables[0]);

            grid.DataSource = vos;
            grid.DataBind();

            this.Boletos = vos;
        }

        List<ItemVO> old_processaPlanilhaEnviada(DataTable dt)
        {
            decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M;

            Beneficiario ben = null;
            List<ItemVO> vos = new List<ItemVO>();
            List<string> ids = new List<string>();

            int i = -1;
            string straux = "", funcional = "", nome = "";
            object auxId = null, auxNome = null, aux = null;

            decimal valorTotal = 0, valorSemReajuste = 0;
            bool temPlano = false, nomeForcado = false;
            DateTime vigencia = new DateTime(1850, 01, 01);

            AdicionalBeneficiario ab = null;
            DataTable dtAdicionais = new DataTable();

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                foreach (DataRow row in dt.Rows)
                {
                    i++;
                    if (i == 0) continue;

                    ItemVO vo = new ItemVO();

                    vo.Marcado = true;
                    valorSemReajuste = 0;
                    vo.Residual = 0;
                    vo.ValorSistema = 0;
                    nomeForcado = false;

                    vo.Funcional = base.CToString(row[3]);
                    vo.Nome = base.CToString(row[0]);
                    nome = vo.Nome;
                    straux = base.CToString(row[12]);
                    vo.ValorArquivo = string.Concat(straux.Substring(0, straux.Length - 2), ",", straux.Substring(straux.Length - 2, 2));
                    straux = base.CToString(row[3]);
                    funcional = straux.Substring(0, straux.Length - 1); //remove o utlimo caractere, pois é usado somente pela Prodesp internamente

                    //if (vo.Nome != "MARIA LUCIA DE FREITAS") continue;

                    if (nome.ToUpper().Trim() == "MARIA LUCIA DE FREITAS")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 12971, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "SANDRA REGINA DA SILVA SOUSA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 13366, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "MARIA CARMELITA SILVA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 35837, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "DOLARICE ROSA DE OLIVEIRA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 39240, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "GELNI MOURA DOS SANTOS")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 39221, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "BLANCA ELEONORA DE CAMARGO")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 39337, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "DALVA OLIVEIRA PEIXOTO ROMEIRO")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 39895, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "ROSEVANIA DOMINGUES P SILVA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 37718, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "DINAH DE OLIVEIRA MATIAS")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 35065, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "AMENOEL DIAS AQUINO")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 32627, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "DIRCE EUGENIO DA SILVA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 34067, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "ELENICE DOS SANTOS RIBEIRO")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 32637, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "EDUARDO RIBEIRO BARBOSA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 34420, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "JOAO LUIZ DA SILVA GUIMARAES")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 34548, null, null, pm);
                        nomeForcado = true;
                    }
                    else
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_matriculafuncional like '%" + funcional + "%'", null, null, pm);

                    if (auxId == null || auxId == DBNull.Value)
                    {
                        vo.Status = "Nao Localizado";
                        vo.Marcado = false;
                        vos.Add(vo);
                        continue;
                    }
                    else
                    {
                        auxNome = LocatorHelper.Instance.ExecuteScalar("select beneficiario_nome from beneficiario where beneficiario_id = " + auxId, null, null, pm);

                        if (Convert.ToString(auxNome).ToUpper() != vo.Nome.ToUpper() && !nomeForcado)
                        {
                            vo.Marcado = false;
                            vo.Status = "Nao Localizado";
                            vo.NomeLocalizado = Convert.ToString(auxNome);
                        }
                        //else
                        //{
                        vo.NomeLocalizado = Convert.ToString(auxNome);

                        if (nomeForcado) { vo.NomeLocalizado = nome; vo.Status = ""; vo.Marcado = true; }

                        ben = new Beneficiario(auxId);
                        pm.Load(ben);

                        vo.BeneficiarioId = Convert.ToString(ben.ID);

                        #region verifica previdencia, seguro e contrato cancelado 

                        //verifica se tem previdencia. se tiver, nao deve enviar boleto. 
                        aux = LocatorHelper.Instance.ExecuteScalar("select id from ___previdencia where dados like '%" + ben.MatriculaFuncional + "%'", null, null, pm);
                        if (aux != null && aux != DBNull.Value)
                        {
                            vo.Marcado = false;
                            continue;
                        }

                        //verifica se tem seguro. se tiver, nao deve enviar boleto. 
                        aux = LocatorHelper.Instance.ExecuteScalar("select id from ___seguros where mat_ass like '%" + ben.MatriculaAssociativa + "%'", null, null, pm);
                        if (aux != null && aux != DBNull.Value)
                        {
                            vo.Marcado = false;
                            continue;
                        }

                        //verifica contrato cancelado
                        aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contrato_id = contratobeneficiario_contratoId where contrato_cancelado=0 and contrato_inativo=0 and contratobeneficiario_beneficiarioId = " + ben.ID, null, null, pm);
                        if (aux == null || aux == DBNull.Value)
                        {
                            vo.Marcado = false;
                            continue;
                        }
                        else
                            vo.ContratoId = Convert.ToString(aux);

                        #endregion

                        //verifica se tem planos
                        dtAdicionais.Rows.Clear();
                        dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid where adicionalbeneficiario_recorrente=1 and adicionalbeneficiario_beneficiarioid=" + auxId, "result", pm).Tables[0];

                        valorTotal = 0;
                        temPlano = false;

                        //primeiro verifica se tem plano de saude (se nao tivar nao continua.
                        foreach (DataRow rowAd in dtAdicionais.Rows)
                        {
                            ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                            if (ab == null) continue;
                            if (!ab.Recorrente) continue;

                            if (base.CToInt(ab.AdicionalCodigo) >= 4435 && base.CToInt(ab.AdicionalCodigo) <= 4442) { temPlano = true; break; }
                        }

                        if (temPlano)
                        {
                            vo.Marcado = true;

                            if (string.IsNullOrEmpty(vo.Status))
                            {
                                ids.Add(Convert.ToString(ben.ID));
                            }

                            #region comentado 
                            //foreach (DataRow rowAd in dtAdicionais.Rows)
                            //{
                            //    ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                            //    if (ab == null) continue;
                            //    if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                            //    if (ab.AdicionalTipo == 0) //somente taxas e planos
                            //    {
                            //        valorTotal += ab.RetornaValor(-1, vigencia);
                            //    }
                            //}

                            //vo.ValorSistema = valorTotal;
                            //vo.Residual = valorTotal - Convert.ToDecimal(vo.ValorArquivo, cinfo);
                            #endregion
                            foreach (DataRow rowAd in dtAdicionais.Rows)
                            {
                                ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                                if (ab == null) continue;
                                if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                                valorSemReajuste += ab.RetornaValor(-1, vigencia); //AdicionalBeneficiario._FormaPagtoBoleto

                                if (!ab.Atualizado)
                                {
                                    ab.AplicaRejuste(reajustePlano, reajusteTaxa, reajusteSeguro);
                                    ab.Atualizado = true;

                                    if (nomeForcado || string.IsNullOrEmpty(vo.Status))
                                    {
                                        pm.Save(ab);
                                    }
                                }

                                if (ab.AdicionalTipo == 0) //somente taxas e planos
                                {
                                    valorTotal += ab.RetornaValor(-1, vigencia);
                                }
                            }

                            vo.ValorSistema = valorTotal;
                            vo.Residual = valorTotal - Convert.ToDecimal(vo.ValorArquivo, cinfo);
                            if (vo.Residual <= 2) vo.Marcado = false;
                        }
                        else
                        {
                            vo.Marcado = false;
                            if(string.IsNullOrEmpty(vo.Status))
                                vo.Status = "Nao possui plano";
                            else
                                vo.Status += " - Nao possui plano";
                        }
                        //}
                    }

                    vos.Add(vo);
                }
            }

            return vos;
        }
        List<ItemVO> processaPlanilhaEnviada(DataTable dt)
        {
            decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M;

            Beneficiario ben = null;
            List<ItemVO> vos = new List<ItemVO>();
            List<string> ids = new List<string>();

            int i = -1;
            string straux = "", straux2 = "", funcional = "", nome = "";
            object auxId = null, auxNome = null, aux = null;

            decimal valorTotal = 0, valorSemReajuste = 0;
            bool temPlano = false, nomeForcado = false;
            DateTime vigencia = new DateTime(1850, 01, 01);

            AdicionalBeneficiario ab = null;
            DataTable dtAdicionais = new DataTable();

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                foreach (DataRow row in dt.Rows)
                {
                    i++;
                    if (i == 0) continue;

                    ItemVO vo = new ItemVO();

                    vo.Marcado = true;
                    valorSemReajuste = 0;
                    vo.Residual = 0;
                    vo.ValorSistema = 0;
                    nomeForcado = false;

                    vo.Funcional = base.CToString(row[3]);
                    vo.Nome = base.CToString(row[0]);

                    if (vo.Nome.Trim() == "") continue;

                    nome = vo.Nome;

                    //straux = (base.CToDecimal(row[9]) + base.CToDecimal(row[12])).ToString("N2");

                    straux = base.CToString(row[9]);
                    straux2 = base.CToString(row[12]);

                    if (straux.Length >= 2 && straux2.Length >= 2)
                    {
                        vo.ValorArquivo = Convert.ToDecimal(
                            base.CToDecimal(string.Concat(Convert.ToString(row[9]).Substring(0, Convert.ToString(row[9]).Length - 2), ",", Convert.ToString(row[9]).Substring(Convert.ToString(row[9]).Length - 2, 2))) +
                            base.CToDecimal(string.Concat(Convert.ToString(row[12]).Substring(0, Convert.ToString(row[12]).Length - 2), ",", Convert.ToString(row[12]).Substring(Convert.ToString(row[12]).Length - 2, 2)))).ToString("N2");
                    }
                    else if (straux.Length >= 2)
                    {
                        vo.ValorArquivo = Convert.ToDecimal(
                            base.CToDecimal(string.Concat(straux.Substring(0, straux.Length - 2), ",", straux.Substring(straux.Length - 2, 2)))).ToString("N2");
                    }
                    else
                    {
                        vo.ValorArquivo = Convert.ToDecimal(
                            base.CToDecimal(string.Concat(straux2.Substring(0, straux2.Length - 2), ",", straux2.Substring(straux2.Length - 2, 2)))).ToString("N2");
                    }

                    straux = base.CToString(row[3]);
                    funcional = straux.Substring(0, straux.Length - 1); //remove o utlimo caractere, pois é usado somente pela Prodesp internamente

                    //if (vo.Nome != "MARIA LUCIA DE FREITAS") continue;

                    if (nome.ToUpper().Trim() == "JOAO EDUARDO DE SOUZA")
                    {
                        int asdfasdf = 0;
                    }

                    if (nome.ToUpper().Trim() == "MARIA LUCIA DE FREITAS")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 12971, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "SANDRA REGINA DA SILVA SOUSA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 13366, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "MARIA CARMELITA SILVA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 35837, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "DOLARICE ROSA DE OLIVEIRA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 39240, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "GELNI MOURA DOS SANTOS")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 39221, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "BLANCA ELEONORA DE CAMARGO")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 39337, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "DALVA OLIVEIRA PEIXOTO ROMEIRO")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 39895, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "ROSEVANIA DOMINGUES P SILVA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 37718, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "DINAH DE OLIVEIRA MATIAS")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 35065, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "AMENOEL DIAS AQUINO")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 32627, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "DIRCE EUGENIO DA SILVA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 34067, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "ELENICE DOS SANTOS RIBEIRO")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 32637, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "EDUARDO RIBEIRO BARBOSA")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 34420, null, null, pm);
                        nomeForcado = true;
                    }
                    else if (nome.ToUpper().Trim() == "JOAO LUIZ DA SILVA GUIMARAES")
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_id=" + 34548, null, null, pm);
                        nomeForcado = true;
                    }
                    else
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_matriculafuncional like '%" + funcional + "%'", null, null, pm);

                    if (auxId == null || auxId == DBNull.Value)
                    {
                        auxId = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 where beneficiario_nome like '%" + vo.Nome.Trim() + "%'", null, null, pm);

                        if (auxId == null || auxId == DBNull.Value)
                        {
                            vo.Status = "Nao Localizado";
                            vo.Marcado = false;
                            vos.Add(vo);
                            continue;
                        }
                    }
                    //else
                    //{
                        auxNome = LocatorHelper.Instance.ExecuteScalar("select beneficiario_nome from beneficiario where beneficiario_id = " + auxId, null, null, pm);

                        if (Convert.ToString(auxNome).Trim().ToUpper() != vo.Nome.Trim().ToUpper() && !nomeForcado)
                        {
                            vo.Marcado = false;
                            vo.Status = "Nao Localizado";
                            vo.NomeLocalizado = Convert.ToString(auxNome);
                        }
                        //else
                        //{
                        vo.NomeLocalizado = Convert.ToString(auxNome);

                        if (nomeForcado) { vo.NomeLocalizado = nome; vo.Status = ""; vo.Marcado = true; }

                        ben = new Beneficiario(auxId);
                        pm.Load(ben);

                        vo.BeneficiarioId = Convert.ToString(ben.ID);

                        #region verifica previdencia, seguro e contrato cancelado 

                        //verifica se tem previdencia. se tiver, nao deve enviar boleto. 
                        aux = LocatorHelper.Instance.ExecuteScalar("select id from ___previdencia where dados like '%" + ben.MatriculaFuncional + "%'", null, null, pm);
                        if (aux != null && aux != DBNull.Value)
                        {
                            vo.Marcado = false;
                            continue;
                        }

                        //verifica se tem seguro. se tiver, nao deve enviar boleto. 
                        aux = LocatorHelper.Instance.ExecuteScalar("select id from ___seguros where mat_ass like '%" + ben.MatriculaAssociativa + "%'", null, null, pm);
                        if (aux != null && aux != DBNull.Value)
                        {
                            vo.Marcado = false;
                            continue;
                        }

                        //verifica contrato cancelado
                        aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contrato_id = contratobeneficiario_contratoId where contrato_cancelado=0 and contrato_inativo=0 and contratobeneficiario_beneficiarioId = " + ben.ID, null, null, pm);
                        if (aux == null || aux == DBNull.Value)
                        {
                            vo.Marcado = false;
                            continue;
                        }
                        else
                            vo.ContratoId = Convert.ToString(aux);

                        #endregion

                        //verifica se tem planos
                        dtAdicionais.Rows.Clear();
                        dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid where adicionalbeneficiario_recorrente=1 and adicionalbeneficiario_beneficiarioid=" + auxId, "result", pm).Tables[0];

                        valorTotal = 0;
                        temPlano = false;

                        //primeiro verifica se tem plano de saude (se nao tivar nao continua.
                        foreach (DataRow rowAd in dtAdicionais.Rows)
                        {
                            ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                            if (ab == null) continue;
                            if (!ab.Recorrente) continue;

                            if (base.CToInt(ab.AdicionalCodigo) >= 4435 && base.CToInt(ab.AdicionalCodigo) <= 4442) { temPlano = true; break; }
                        }

                        if (temPlano)
                        {
                            vo.Marcado = true;

                            if (string.IsNullOrEmpty(vo.Status))
                            {
                                ids.Add(Convert.ToString(ben.ID));
                            }

                            #region comentado 
                            //foreach (DataRow rowAd in dtAdicionais.Rows)
                            //{
                            //    ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                            //    if (ab == null) continue;
                            //    if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                            //    if (ab.AdicionalTipo == 0) //somente taxas e planos
                            //    {
                            //        valorTotal += ab.RetornaValor(-1, vigencia);
                            //    }
                            //}

                            //vo.ValorSistema = valorTotal;
                            //vo.Residual = valorTotal - Convert.ToDecimal(vo.ValorArquivo, cinfo);
                            #endregion
                            foreach (DataRow rowAd in dtAdicionais.Rows)
                            {
                                ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                                if (ab == null) continue;
                                if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                                valorSemReajuste += ab.RetornaValor(-1, vigencia); //AdicionalBeneficiario._FormaPagtoBoleto

                                if (!ab.Atualizado)
                                {
                                    ab.AplicaRejuste(reajustePlano, reajusteTaxa, reajusteSeguro);
                                    ab.Atualizado = true;

                                    if (nomeForcado || string.IsNullOrEmpty(vo.Status))
                                    {
                                        pm.Save(ab);
                                    }
                                }

                                if (ab.AdicionalTipo == 0) //somente taxas e planos
                                {
                                    valorTotal += ab.RetornaValor(-1, vigencia);
                                }
                            }

                            vo.ValorSistema = valorTotal;
                            vo.Residual = valorTotal - Convert.ToDecimal(vo.ValorArquivo, cinfo);
                            if (vo.Residual <= 2) vo.Marcado = false;
                        }
                        else
                        {
                            vo.Marcado = false;
                            if(string.IsNullOrEmpty(vo.Status))
                                vo.Status = "Nao possui plano";
                            else
                                vo.Status += " - Nao possui plano";
                        }
                        //}
                    //}

                    vos.Add(vo);
                }
            }

            return vos;
        }

        protected void cmdBoleto_click(object sender, EventArgs e)
        {
            if (this.Boletos == null || this.Boletos.Count == 0) { return; }

            object aux = null;
            Cobranca cob = null;

            //DateTime vencimento = new DateTime(2016, 11, 21, 23, 59, 59, 995);
            DateTime vencimento = base.CStringToDateTime(txtVencimento.Text, "23:59", 59, 995);  //new DateTime(2016, 12, 15, 23, 59, 59, 995);
            if (vencimento == DateTime.MinValue)
            {
                base.Alerta(null, this, "err_", "Data de vencimento inválida.");
                return;
            }

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    foreach (var vo in this.Boletos)
                    {
                        if (!vo.Marcado) continue;
                        //obtem id do contrato
                        aux = vo.ContratoId;// LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 where contratobeneficiario_beneficiarioId=" + vo.BeneficiarioId, null, null, pm);

                        if (aux == null || aux == DBNull.Value || Convert.ToString(aux).Trim() == "")
                        { throw new ApplicationException("Não encontrou o id de contrato."); }

                        cob = new Cobranca();
                        cob.Tipo = (int)Cobranca.eTipo.DiferencaUbraSP;
                        cob.Parcela = 0;
                        cob.DataVencimento = vencimento;
                        cob.Valor = vo.Residual;
                        cob.Carteira = 101;
                        cob.Banco = 353; //santander
                        cob.CobrancaRefID = null;
                        cob.DataPgto = DateTime.MinValue;
                        cob.ValorPgto = Decimal.Zero;
                        cob.Pago = false;
                        cob.PropostaID = aux;
                        cob.Cancelada = false;
                        cob.ArquivoIDUltimoEnvio = -2;

                        pm.Save(cob);
                    }

                    //gera o agendamento do arquivo
                    ArquivoRemessaAgendamento ara = new ArquivoRemessaAgendamento();
                    ara.CriterioID = null;
                    ara.Processado = false;
                    ara.ProcessamentoEm = DateTime.Now.AddMinutes(5);
                    ara.VencimentoAte = vencimento;
                    ara.VencimentoDe = vencimento;
                    ara.VigenciaDe = new DateTime(1800, 1, 1, 23, 59, 59, 995);
                    ara.VigenciaAte = DateTime.Now.AddDays(90);
                    ara.QtdBoletos = 1;
                    ara.Carteira = 101;
                    ara.Banco = 353; //santander                    

                    ara.SomenteBoletosUBRASP = false;
                    ara.SomenteNaoRecorrentes = true;

                    ara.Grupo = null;
                    ara.ArquivoNomeInstance = "remessa_prodesp";
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
    class ItemVO
    {
        public string Nome { get; set; }
        public string NomeLocalizado { get; set; }
        public string Funcional { get; set; }
        public string ValorArquivo { get; set; }
        public string Status { get; set; }
        public decimal ValorSistema { get; set; }
        public decimal Residual { get; set; }
        public bool Marcado { get; set; }
        public string BeneficiarioId { get; set; }
        public string ContratoId { get; set; }

    }

    [Serializable]
    class old_ItemVO
    {
        public string Nome { get; set; }
        public string NomeLocalizado { get; set; }
        public string Funcional { get; set; }
        public string ValorArquivo { get; set; }
        public string Status { get; set; }
        public decimal ValorSistema { get; set; }
        public decimal Residual { get; set; }
    }
}
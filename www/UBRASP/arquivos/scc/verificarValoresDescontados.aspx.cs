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

    using Excel;

    public partial class verificarValoresDescontados : PageBase
    {
        String mPath
        {
            get { return String.Concat(Server.MapPath("/"), ConfigurationManager.AppSettings["psccFilePath"].Replace("/", "\\")); }
        }

        List<ItemVO> Descontados
        {
            get { return ViewState["descont"] as List<ItemVO>; }
            set { ViewState["descont"] = value; }
        }

        List<ItemVO> NaoDescontados
        {
            get { return ViewState["ndescont"] as List<ItemVO>; }
            set { ViewState["ndescont"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtVencimentoDescontados.Text = string.Concat("15/", DateTime.Now.Month, "/", DateTime.Now.Year);
                txtVencimentoNAODescontados.Text = string.Concat("15/", DateTime.Now.Month, "/", DateTime.Now.Year);
            }
        }

        DataTable geraDataTable()
        {
            DataTable dt = new DataTable();

            //dt.Columns.Add("CPF");
            //dt.Columns.Add("Funcional");
            //dt.Columns.Add("Orgao");
            //dt.Columns.Add("IdentOrgao");
            //dt.Columns.Add("Nuavebev");
            //dt.Columns.Add("Parcela");
            //dt.Columns.Add("Valor");
            //dt.Columns.Add("Valor_Descontado");
            //dt.Columns.Add("Valor_Tratado");
            //dt.Columns.Add("Erro");

            dt.Columns.Add("Column1");
            dt.Columns.Add("Column2");
            dt.Columns.Add("Column3");
            dt.Columns.Add("Column4");
            dt.Columns.Add("Column5");
            dt.Columns.Add("Column6");
            dt.Columns.Add("Column7");
            dt.Columns.Add("Column8");
            dt.Columns.Add("Column9");
            dt.Columns.Add("Column10");
            dt.Columns.Add("Column11");
            //

            return dt;
        }

        void adicionaLinhaEmDataTable_v01(ref DataTable dt, string linha, bool descontado)
        {
            string valor = Convert.ToInt32(linha.Substring(115, 9)).ToString();
            string valorDesc = linha.Substring(124, 15);

            //if      (Convert.ToInt32(valorDesc) == 0 && descontado == true) return;
            //else if (Convert.ToInt32(valorDesc) > 0 && descontado == false) return;

            DataRow row = dt.NewRow();


            row[0] = linha.Substring(25, 11);
            row[1] = Convert.ToInt64(linha.Substring(36, 15));
            row[2] = "";
            row[3] = linha.Substring(61, 5);
            row[4] = linha.Substring(80, 21);
            row[5] = linha.Substring(101, 2);
            row[6] = valor;
            row[7] = Convert.ToInt32(valorDesc);
            row[8] = string.Concat(valor.Substring(0, valor.Length - 2), ",", valor.Substring(valor.Length - 2, 2));
            row[9] = "";
            row[10] = linha.Substring(200, 150).Trim(); //nome

            dt.Rows.Add(row);
        }

        void adicionaLinhaEmDataTable_v02(ref DataTable dt, string linha, bool descontado)
        {
            string valor = Convert.ToInt32(linha.Substring(293, 15)).ToString(); //Convert.ToInt32(linha.Substring(284, 9)).ToString();
            string valorDesc = linha.Substring(308, 15); //linha.Substring(293, 15);

            DataRow row = dt.NewRow();

            row[0] = linha.Substring(25, 11); //cpf
            row[1] = Convert.ToInt64(linha.Substring(36, 15)); //funcional
            row[2] = ""; //orgao
            row[3] = linha.Substring(211, 5); // orgao id
            row[4] = linha.Substring(230, 21); //averbacao
            row[5] = linha.Substring(270, 2); //Parcela
            row[6] = valor;
            row[7] = Convert.ToInt32(valorDesc);
            row[8] = string.Concat(valor.Substring(0, valor.Length - 2), ",", valor.Substring(valor.Length - 2, 2));
            row[9] = "";
            row[10] = linha.Substring(384, 150).Trim(); //nome

            dt.Rows.Add(row);
        }

        protected void cmdProcessar_click(object sender, EventArgs e)
        {
            string arquivoNome = "";

            if (fuArquivo.PostedFile != null && !string.IsNullOrEmpty(fuArquivo.PostedFile.FileName))
            {
                if (Path.GetExtension(fuArquivo.PostedFile.FileName).ToUpper() == ".TXT")
                {
                    #region upload e leitura do arquivo 

                    string diretorioBase = this.mPath;

                    if (!Directory.Exists(diretorioBase)) Directory.CreateDirectory(diretorioBase);

                    arquivoNome = string.Concat(diretorioBase, "pcss_descontos", Path.GetExtension(fuArquivo.PostedFile.FileName));

                    if (File.Exists(arquivoNome)) File.Delete(arquivoNome);

                    fuArquivo.PostedFile.SaveAs(arquivoNome);

                    String content = "";
                    using (StreamReader stream = new StreamReader(arquivoNome))
                    {
                        content = stream.ReadToEnd();
                        stream.Close();
                    }

                    String[] arrLinhas = content.Split(
                        new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    #endregion

                    DataTable dtDesc    = geraDataTable();
                    DataTable dtNAODesc = geraDataTable();

                    foreach (string linha in arrLinhas)
                    {
                        if (!linha.StartsWith("1")) continue;

                        adicionaLinhaEmDataTable_v02(ref dtDesc, linha, true);
                        //adicionaLinhaEmDataTable(ref dtNAODesc, linha, false);
                    }

                    //DESCONTADOS
                    List<ItemVO> vos = processaPlanilha_DESCONTADOS(dtDesc);

                    /////////todo: denis: tirar isso
                    //foreach (var vo in vos) { if (vo.Marcado) { vo.Marcado = false; } }
                    ////////////////////////////////

                    //todo: denis, descomentar linha abaixo!
                    this.processaBeneficiariosQueNaoEstaoNoArquivo(ref vos);

                    gridDescontados.DataSource = vos;
                    gridDescontados.DataBind();

                    if (vos == null || vos.Count == 0)
                    {
                        litDesc.Text = " - Nenhum beneficiário com residual a pagar"; cmdBoletoDesc.Visible = false;
                    }
                    else
                    {
                        litDesc.Text = ""; cmdBoletoDesc.Visible = true;
                    }

                    this.Descontados = vos;

                    //NAO DESCONTADOS ///////////////////////////////////////////////////////////////////
                    List<ItemVO> vosN = null; //processaPlanilha_NAO_DESCONTADOS(dtNAODesc);
                    grid.DataSource = vosN;
                    grid.DataBind();

                    if (vosN == null || vosN.Count == 0)
                    {
                        litNaoDesc.Text = " - Nenhum beneficiário com residual a pagar"; cmdBoletoNaoDesc.Visible = false;
                    }
                    else
                    {
                        litNaoDesc.Text = ""; cmdBoletoNaoDesc.Visible = true;
                    }

                    this.NaoDescontados = vosN;
                }
                else if (Path.GetExtension(fuArquivo.PostedFile.FileName).ToUpper() == ".XLS" || Path.GetExtension(fuArquivo.PostedFile.FileName).ToUpper() == ".XLSX")
                {
                    #region 

                    #region upload da planilha enviada

                    string diretorioBase = this.mPath;

                    if (!Directory.Exists(diretorioBase)) Directory.CreateDirectory(diretorioBase);

                    arquivoNome = string.Concat(diretorioBase, "pcss_descontos", Path.GetExtension(fuArquivo.PostedFile.FileName));

                    if (File.Exists(arquivoNome)) File.Delete(arquivoNome);

                    fuArquivo.PostedFile.SaveAs(arquivoNome);
                    #endregion

                    FileStream stream = File.Open(arquivoNome, FileMode.Open, FileAccess.Read);

                    IExcelDataReader excelReader = null;

                    if (Path.GetExtension(arquivoNome).ToUpper() == ".XLSX")
                        excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    else
                        excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

                    DataSet ds = excelReader.AsDataSet();

                    stream.Close(); stream.Dispose();

                    //DESCONTADOS
                    List<ItemVO> vos = processaPlanilha_DESCONTADOS(ds.Tables[1]);
                    gridDescontados.DataSource = vos;
                    gridDescontados.DataBind();

                    if (vos == null || vos.Count == 0) 
                    {
                        litDesc.Text = " - Nenhum beneficiário com residual a pagar"; cmdBoletoDesc.Visible = false; 
                    }
                    else 
                    {
                        litDesc.Text = ""; cmdBoletoDesc.Visible = true; 
                    }

                    this.Descontados = vos;

                    //NAO DESCONTADOS ///////////////////////////////////////////////////////////////////
                    List<ItemVO> vosN = processaPlanilha_NAO_DESCONTADOS(ds.Tables[0]);
                    grid.DataSource = vosN;
                    grid.DataBind();

                    if (vosN == null || vosN.Count == 0)
                    {
                        litNaoDesc.Text = " - Nenhum beneficiário com residual a pagar"; cmdBoletoNaoDesc.Visible = false; 
                    }
                    else 
                    {
                        litNaoDesc.Text = ""; cmdBoletoNaoDesc.Visible = true; 
                    }

                    this.NaoDescontados = vosN;

                    #endregion
                }
            }
        }

        void processaBeneficiariosQueNaoEstaoNoArquivo(ref List<ItemVO> vos)
        {
            DateTime vencimento = base.CStringToDateTime(txtVencimentoDescontados.Text, "23:59", 59, 995);  //new DateTime(2016, 12, 15, 23, 59, 59, 995);
            if (vencimento == DateTime.MinValue)
            {
                return;
            }

            //var jaProcessados = this.Descontados;
            //if (jaProcessados == null) jaProcessados = new List<ItemVO>();
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
                    "   and contrato_contratoadmId <> 40 and contrato_contratoadmId <> 45"); //somente quem NAO for do tribunal de justica e assembleia

                //sql = string.Concat(
                //    "select contrato_id,contrato_numero,beneficiario_id,beneficiario_cpf,beneficiario_nome,beneficiario_matriculaFuncional,contratoadm_descricao ",
                //    "   from contrato ",
                //    "       inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1",
                //    "       inner join beneficiario on contratobeneficiario_beneficiarioId = beneficiario_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1",
                //    "       inner join contratoadm on contrato_contratoadmId = contratoadm_id ",
                //    "where ",
                //    "   contrato_id=34576"); //LUIZ VANDERLEI DE LIMA

                //sql = string.Concat(
                //    "select contrato_id,contrato_numero,beneficiario_id,beneficiario_cpf,beneficiario_nome,beneficiario_matriculaFuncional,contratoadm_descricao ",
                //    "   from contrato ",
                //    "       inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1",
                //    "       inner join beneficiario on contratobeneficiario_beneficiarioId = beneficiario_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1",
                //    "       inner join contratoadm on contrato_contratoadmId = contratoadm_id ",
                //    "where ",
                //    "   beneficiario_nome='VALTER COSTA'"); //CONCEICAO APARECIDA DOS SANTOS

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
                        if (vo.Nome.ToUpper().Contains("CONCEICAO APARECIDA DOS SANTOS")) { int j = 0; }

                        //if (vo.ValorDescontado == vo.ValorArquivo) continue;

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
                                vo.Atualizado = "NAO";
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

                        vo.Residual = vo.ValorSistema - vo.ValorDescontado; //vo.ValorArquivo;
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

        /// <summary>
        /// Verifica se tem na colecao de ja processados. Se tem nao faz nada, senao adiciona na colecao naoMencionadosNoArquivo
        /// </summary>
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
            novo.Nome = Convert.ToString(row["beneficiario_nome"]);
            novo.CPF = base.CToString(row[0]).PadLeft(11, '0');
            novo.Residual = 0;
            novo.ValorSistema = 0;
            novo.Funcional = base.CToString(row["beneficiario_matriculaFuncional"]);
            novo.ORGAO = base.CToString(row["contratoadm_descricao"]);
            novo.Parcela = "0";
            novo.Marcado = false;
            novo.ValorDescontado = 0;
            novo.ValorArquivo = 0;
            novo.Atualizado = "SIM";

            naoMencionadosNoArquivo.Add(novo);

            return false;
        }

        List<ItemVO> processaPlanilha_DESCONTADOS(DataTable dt)
        {
            DateTime competencia = base.CStringToDateTime(txtVencimentoDescontados.Text, "23:59", 59, 995).AddMonths(-1);
            decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M;
            string statusNaoLocal = "NAO LOCALIZADO";
            string statusSemPlano = "NAO TEM PLANO";
            string statusPrividen = "TEM PREVIDENCIA";
            string statusSeguro   = "TEM SEGURO";
            string statusSemDeb   = "NENHUM DEBITO";
            List<ItemVO> vos = new List<ItemVO>();
            List<string> ids = new List<string>();

            int i = -1;
            string straux = "";
            object aux = null; object aux2 = null, aux3 = null;

            decimal valorTotal = 0; //, valorSeguradoras = 0;
            bool temPlano = false;
            DateTime vigencia = new DateTime(1850, 01, 01);

            AdicionalBeneficiario ab = null;
            Beneficiario beneficiario = null;
            DataTable dtAdicionais = new DataTable();

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            List<string> cpfs = new List<string>();
            #region agrupa CPFS's
            foreach (DataRow row in dt.Rows)
            {
                i++;
                //if (i == 0) continue;
                if (base.CToString(row[0]).Trim() == "") continue;
                if (cpfs.Contains(base.CToString(row[0]).PadLeft(11, '0'))) continue;

                cpfs.Add(base.CToString(row[0]).PadLeft(11, '0'));
            }
            #endregion

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();
                //NonQueryHelper.Instance.ExecuteNonQuery("update __rspp set mesano=null", pm); //nao pode ser feito

                DataRow[] rows = null;

                foreach (string cpf in cpfs) //foreach (DataRow row in dt.Rows)
                {
                    //if (cpf != "05116458806") continue; //todo: denis deletar esta linha

                    valorTotal = 0;
                    rows = dt.Select("Column1='" + cpf + "'");

                    ItemVO vo = new ItemVO();

                    vo.Residual         = 0;
                    vo.ValorSistema     = 0;
                    vo.Funcional        = base.CToString(rows[0][1]);
                    vo.CPF              = cpf;
                    vo.ORGAO            = base.CToString(rows[0][2]);
                    vo.Parcela          = base.CToString(rows[0][5]);
                    vo.Marcado          = false;
                    vo.ValorDescontado  = 0;
                    vo.Atualizado       = "SIM";
                    vo.Nome             = base.CToString(rows[0][10]);

                    //soma os valores vindos do arquivo 
                    foreach (DataRow row in rows)
                    {
                        //valor cobrado
                        straux = base.CToString(row[6]);
                        vo.ValorArquivo += Convert.ToDecimal(
                            string.Concat(straux.Substring(0, straux.Length - 2), ",", straux.Substring(straux.Length - 2, 2)),
                            cinfo);

                        //valor descontado
                        straux = base.CToString(row[7]);

                        if (straux != "0")
                        {
                            vo.ValorDescontado += Convert.ToDecimal(
                                string.Concat(straux.Substring(0, straux.Length - 2), ",", straux.Substring(straux.Length - 2, 2)),
                                cinfo);
                        }
                    }

                    //if (vo.ValorDescontado == vo.ValorArquivo) continue;

                    //localiza o beneficiario 
                    aux = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculaFuncional like '%" + vo.Funcional + "'", null, null, pm);
                    if (aux == null)
                    {
                        aux = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_cpf = '" + cpf + "'", null, null, pm);
                        if (aux == null)
                        {
                            aux = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculafuncional like '%" + vo.Funcional + "'", null, null, pm);
                        }
                    }

                    if (aux == null)
                    {
                        vo.Status = statusNaoLocal;
                        vo.Marcado = false;
                        vos.Add(vo);
                        continue;
                    }

                    beneficiario = new Beneficiario(aux);
                    pm.Load(beneficiario);

                    vo.BeneficiarioId = base.CToString(aux);
                    vo.Nome = beneficiario.Nome;

                    //Aqui deve manipular as tabelas RSPP e Chubb //////////////////////////////////////////////////////////

                    //RSPP/////////////
                    if (vo.ValorDescontado > 0)
                    {
                        aux = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select id from __rspp where (beneficiarioID=", vo.BeneficiarioId, " or beneficiarioID2=", vo.BeneficiarioId, ")"),
                            null, null, pm);

                        if (aux != null && aux != DBNull.Value)
                        {
                            //valorSeguradoras = CToDecimal(LocatorHelper.Instance.ExecuteScalar(
                            //    string.Concat("select R_VALOR from __rspp id=", aux),
                            //    null, null, pm), cinfo);

                            //valorSeguradoras += vo.ValorDescontado;

                            //NonQueryHelper.Instance.ExecuteNonQuery(
                            //    string.Concat("update __rspp set mesano='", competencia.ToString("MMyyyy"), "', R_VALOR='", valorSeguradoras.ToString("N2", cinfo).Replace(".", "").Replace(",", "."), "' where id=", aux), 
                            //    pm);


                            NonQueryHelper.Instance.ExecuteNonQuery(
                                string.Concat("update __rspp set mesano='", competencia.ToString("MMyyyy"), "' where id=", aux),
                                pm);
                        }
                    }

                    ///////////////////
                    //Chubb////////////
                    if (CToDecimal(vo.ValorArquivo) > 0)
                    {
                        aux = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select id from __chubb_envio2016 where beneficiarioID=", vo.BeneficiarioId),
                            null, null, pm);

                        if (aux != null && aux != DBNull.Value)
                        {
                            //valorSeguradoras = CToDecimal(LocatorHelper.Instance.ExecuteScalar(
                            //    string.Concat("select R_VALOR from __rspp id=", auxId),
                            //    null, null, pm), cinfo);

                            //valorSeguradoras += CToDecimal(vo.ValorArquivo, cinfo);

                            //NonQueryHelper.Instance.ExecuteNonQuery(
                            //    string.Concat("update __rspp set mesano='", competencia.ToString("MMyyyy"), "', R_VALOR='", valorSeguradoras.ToString("N2", cinfo).Replace(".", "").Replace(",", "."), "' where id=", auxId),
                            //    pm);

                            NonQueryHelper.Instance.ExecuteNonQuery(
                                string.Concat("update __chubb_envio2016 set competencia='", competencia.ToString("MMyyyy"), "' where id=", aux),
                                pm);
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////

                    #region  comentado
                    //if (vo.Nome.IndexOf("NEUSA DAS GRACAS MARTINS ABACHERLI") > -1)
                    //{
                    //    int neusa = 0;
                    //}
                    //if (vo.Nome != "ANA CLAUDIA DA CUNHA RIBEIRO") continue;

                    //if (vo.Nome.IndexOf("CAMPELO") == -1) continue;

                    //if (vo.Nome.Contains("EUNICE COSTA M")) { int sesbast = 0; }
                    #endregion

                    //verifica se o contrato está cancelado
                    aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contrato_id = contratobeneficiario_contratoId where contrato_cancelado=0 and contrato_inativo=0 and contratobeneficiario_beneficiarioId = " + vo.BeneficiarioId, null, null, pm);
                    if (aux == null || aux == DBNull.Value)
                    {
                        continue;
                    }

                    aux2 = LocatorHelper.Instance.ExecuteScalar("select contrato_inativo from contrato where contrato_id = " + aux, null, null, pm);
                    if (aux2 != null && aux2 != DBNull.Value && Convert.ToInt32(aux2) == 1)
                    {
                        continue;
                    }

                    aux3 = LocatorHelper.Instance.ExecuteScalar("select contrato_cancelado from contrato where contrato_id = " + aux, null, null, pm);
                    if (aux3 != null && aux3 != DBNull.Value && Convert.ToInt32(aux3) == 1)
                    {
                        continue;
                    }

                    //TODO: denis, remover, checagem temporaria
                    //aux = LocatorHelper.Instance.ExecuteScalar("select cobranca_id from cobranca where cobranca_cancelada=0 and month(cobranca_dataVencimento)=5 and year(cobranca_dataVencimento)=2017 and cobranca_propostaid = " + aux, null, null, pm);
                    //if (aux != null && aux != DBNull.Value && Convert.ToInt32(aux) > 0)
                    //{
                    //    continue;
                    //}

                    aux = vo.BeneficiarioId;

                    //carrega adicionais 
                    dtAdicionais.Rows.Clear();
                    dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid where adicionalbeneficiario_recorrente=1 and adicionalbeneficiario_beneficiarioid=" + aux, "result", pm).Tables[0]; //adicionalbeneficiario_recorrente=1 and 

                    valorTotal = 0;
                    temPlano = false;

                    foreach (DataRow rowAd in dtAdicionais.Rows)
                    {
                        ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                        if (ab == null) continue;
                        if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                        if (ab.Atualizado == false) 
                        { 
                            vo.Atualizado = "NAO";
                            ab.AplicaRejuste(reajustePlano, reajusteTaxa, reajusteSeguro);
                            ab.Atualizado = true;
                            pm.Save(ab);
                        }

                        valorTotal += ab.RetornaValor(-1, vigencia);

                        if (Convert.ToInt32(ab.AdicionalCodigo) >= 4435 && Convert.ToInt32(ab.AdicionalCodigo) <= 4442) temPlano = true;
                    }

                    vo.ValorSistema = valorTotal;
                    //vo.Residual = vo.ValorSistema - vo.ValorArquivo;

                    if (!temPlano)
                    {
                        vo.Status = statusSemPlano;
                        vo.Marcado = false;
                        vos.Add(vo);
                        continue;
                    }

                    vo.Residual = vo.ValorSistema - vo.ValorDescontado; //vo.ValorArquivo;
                    if (vo.Residual > 1) vo.Marcado = true;
                    else
                    {
                        vo.Marcado = false;
                        vo.Status = statusSemDeb;
                        vos.Add(vo);
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

            return vos;
        }

        /// <summary>
        /// Fora de uso
        /// </summary>
        List<ItemVO> processaPlanilha_NAO_DESCONTADOS(DataTable dt)
        {
            string statusNaoLocal = "NAO LOCALIZADO";
            string statusSemPlano = "NAO TEM PLANO";
            string statusPrividen = "TEM PREVIDENCIA";
            List<ItemVO> vos = new List<ItemVO>();
            List<string> ids = new List<string>();

            int i = -1;
            string straux = "";
            object aux = null, aux2 = null;

            decimal valorTotal = 0;
            bool temPlano = false;
            DateTime vigencia = new DateTime(1850, 01, 01);

            AdicionalBeneficiario ab = null;
            Beneficiario beneficiario = null;
            DataTable dtAdicionais = new DataTable();

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            List<string> cpfs = new List<string>();
            #region agrupa CPFS's 
            foreach (DataRow row in dt.Rows)
            {
                i++;
                if (i == 0) continue;
                if (base.CToString(row[0]).Trim() == "") continue;
                if (cpfs.Contains(base.CToString(row[0]).PadLeft(11, '0'))) continue;

                cpfs.Add(base.CToString(row[0]).PadLeft(11, '0'));
            }
            #endregion

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();
                DataRow[] rows = null;

                foreach (string cpf in cpfs) //foreach (DataRow row in dt.Rows)
                {
                    valorTotal = 0;
                    rows = dt.Select("Column1='" + cpf + "'");

                    ItemVO vo = new ItemVO();

                    vo.Residual     = 0;
                    vo.ValorSistema = 0;
                    vo.Funcional    = base.CToString(rows[0][1]);
                    vo.CPF          = cpf;
                    vo.ORGAO        = base.CToString(rows[0][2]);
                    vo.Parcela      = base.CToString(rows[0][5]);
                    vo.Marcado      = true;
                    vo.ValorDescontado = 0;

                    //soma os valores vindos do arquivo 
                    foreach (DataRow row in rows)
                    {
                        straux = base.CToString(row[6]);
                        vo.ValorArquivo += Convert.ToDecimal(
                            string.Concat(straux.Substring(0, straux.Length - 2), ",", straux.Substring(straux.Length - 2, 2)), 
                            cinfo);
                    }

                    //localiza o beneficiario
                    aux = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_cpf = '" + cpf + "'", null, null, pm);
                    if (aux == null)
                    {
                        aux = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculafuncional like '%" + vo.Funcional + "'", null, null, pm);
                    }

                    if (aux == null)
                    {
                        vo.Status = statusNaoLocal;
                        vo.Marcado = false;
                        vos.Add(vo);
                        continue;
                    }

                    beneficiario = new Beneficiario(aux);
                    pm.Load(beneficiario);

                    vo.BeneficiarioId = base.CToString(aux);
                    vo.Nome = beneficiario.Nome;

                    //auxNome = LocatorHelper.Instance.ExecuteScalar("select beneficiario_nome from beneficiario where beneficiario_id = " + aux, null, null, pm);
                    //vo.Nome = base.CToString(auxNome);

                    //verifica se o contrato está cancelado
                    aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contrato_id = contratobeneficiario_contratoId where contrato_cancelado=0 and contrato_inativo=0 and contratobeneficiario_beneficiarioId = " + vo.BeneficiarioId, null, null, pm);
                    if (aux == null || aux == DBNull.Value)
                    {
                        continue;
                    }

                    aux2 = LocatorHelper.Instance.ExecuteScalar("select contrato_inativo from contrato where contrato_id = " + aux, null, null, pm);
                    if (aux2 != null && aux2 != DBNull.Value && Convert.ToInt32(aux2) == 1)
                    {
                        continue;
                    }

                    aux = vo.BeneficiarioId;

                    //carrega adicionais 
                    dtAdicionais.Rows.Clear();
                    dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid where adicionalbeneficiario_beneficiarioId=" + aux, "result", pm).Tables[0]; //adicionalbeneficiario_recorrente=1 and 

                    valorTotal = 0;
                    temPlano = false;

                    foreach (DataRow rowAd in dtAdicionais.Rows)
                    {
                        ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                        if (ab == null) continue;
                        if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                        //if (ab.AdicionalTipo == 0) //somente taxas ou planos
                        //{
                            valorTotal += ab.RetornaValor(-1, vigencia);
                        //}

                        if (Convert.ToInt32(ab.AdicionalCodigo) >= 4435 && Convert.ToInt32(ab.AdicionalCodigo) <= 4442) temPlano = true;
                    }

                    vo.ValorSistema = valorTotal;
                    vo.Residual = vo.ValorArquivo; //vo.ValorSistema - vo.ValorArquivo;

                    if (!temPlano)
                    {
                        vo.Status = statusSemPlano;
                        vo.Marcado = false;
                        vos.Add(vo);
                        continue;
                    }

                    //verifica se tem previdencia. se tiver, nao deve enviar boleto. 
                    aux = LocatorHelper.Instance.ExecuteScalar("select id from ___previdencia where dados like '%" + beneficiario.MatriculaFuncional + "%'", null, null, pm);
                    if (aux != null && aux != DBNull.Value)
                    {
                        vo.Status = statusPrividen;
                        vo.Marcado = false;
                        vos.Add(vo);
                        continue;
                    }

                    //verifica se tem seguro. se tiver, nao deve enviar boleto. 
                    aux = LocatorHelper.Instance.ExecuteScalar("select id from ___seguros where mat_ass like '%" + beneficiario.MatriculaAssociativa + "%'", null, null, pm);
                    if (aux != null && aux != DBNull.Value)
                    {
                        vo.Status = statusPrividen;
                        vo.Marcado = false;
                        vos.Add(vo);
                        continue;
                    }

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

                    vo.Status = "OK";
                    vos.Add(vo); 
                    
                    if (vo.Status == "OK")
                    {
                        ids.Add(vo.BeneficiarioId);
                    }
                }
            }

            return vos;
        }

        protected void cmdBoletoDesc_click(object sender, EventArgs e)
        {
            if (this.Descontados == null || this.Descontados.Count == 0) { return; }

            //if (this.Descontados == null) this.Descontados = new List<ItemVO>(); //{ return; }
            //if (this.NaoDescontados == null) this.NaoDescontados = new List<ItemVO>();
            //this.Descontados.AddRange(this.NaoDescontados);

            //IList<ItemVO> final = new List<ItemVO>();

            //foreach (ItemVO desc in this.Descontados)
            //{
            //    if (desc.Marcado) final.Add(desc);
            //}

            //bool localizado = false;
            //foreach (ItemVO naodesc in this.NaoDescontados)
            //{
            //    if (!naodesc.Marcado) continue;

            //    localizado = false;

            //    foreach (ItemVO desc in final)
            //    {
            //        if (naodesc.CPF == desc.CPF)
            //        {
            //            desc.Residual += naodesc.Residual;
            //            localizado = true;
            //        }
            //    }

            //    if (!localizado)
            //    {
            //        final.Add(naodesc);
            //    }
            //}

            //if (final.Count == 0) return;

            //return;

            object aux = null;
            Cobranca cob = null;

            DateTime vencimento = base.CStringToDateTime(txtVencimentoDescontados.Text, "23:59", 59, 995);  //new DateTime(2016, 12, 15, 23, 59, 59, 995);
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
                    foreach (var vo in this.Descontados)
                    //foreach (var vo in final)
                    {
                        if (!vo.Marcado) { continue; }

                        //obtem id do contrato
                        aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 where contratobeneficiario_beneficiarioId=" + vo.BeneficiarioId, null, null, pm);

                        if (aux == null || aux == DBNull.Value) 
                        { throw new ApplicationException("Não encontrou o id de contrato."); }

                        cob = new Cobranca();
                        cob.Tipo = (int)Cobranca.eTipo.DiferencaUbraSP;
                        cob.Parcela = 0;
                        //cob.DataVencimento = Convert.ToDateTime(row["vencimento"]);
                        //cob.DataVencimento = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, cob.DataVencimento.Day, 23, 59, 59, 995);
                        cob.DataVencimento = vencimento;
                        cob.Valor = vo.Residual;
                        cob.CobrancaRefID = null;
                        cob.DataPgto = DateTime.MinValue;
                        cob.ValorPgto = Decimal.Zero;
                        cob.Pago = false;
                        cob.PropostaID = aux;
                        cob.Cancelada = false;
                        cob.ArquivoIDUltimoEnvio = -2;
                        cob.Banco = 353;
                        cob.Carteira = 101;
                        //cob.Instrucoes = "Tendo em vista o feriado de Páscoa o vencimento será dia 17/04/2017";

                        pm.Save(cob); //todo: denis descomentar
                    }

                    //gera o agendamento do arquivo
                    ArquivoRemessaAgendamento ara = new ArquivoRemessaAgendamento();
                    ara.CriterioID = null;
                    ara.Processado = false;
                    ara.ProcessamentoEm = DateTime.Now.AddMinutes(10);
                    ara.VencimentoAte = vencimento;
                    ara.VencimentoDe = vencimento;
                    ara.VigenciaDe = new DateTime(1800, 1, 1, 23, 59, 59, 995);
                    ara.VigenciaAte = DateTime.Now.AddDays(90);
                    ara.QtdBoletos = 1;
                    ara.Banco = 353;
                    ara.Carteira = 101;

                    ara.SomenteBoletosUBRASP = false;
                    ara.SomenteNaoRecorrentes = true;

                    ara.Grupo = null;
                    ara.ArquivoNomeInstance = "remessa_descontados";
                    ara.Legado = true;
                    pm.Save(ara); //todo: denis descomentar

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

        protected void cmdBoletoNaoDesc_click(object sender, EventArgs e)
        {
            if (this.NaoDescontados == null || this.NaoDescontados.Count == 0) { return; }

            object aux = null;
            Cobranca cob = null;

            DateTime vencimento = base.CStringToDateTime(txtVencimentoNAODescontados.Text, "23:59", 59, 995);  //new DateTime(2016, 12, 15, 23, 59, 59, 995);
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
                    foreach (var vo in this.NaoDescontados)
                    {
                        if (!vo.Marcado) continue;

                        //obtem id do contrato
                        aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 where contratobeneficiario_beneficiarioId=" + vo.BeneficiarioId, null, null, pm);

                        if (aux == null || aux == DBNull.Value) 
                        { throw new ApplicationException("Não encontrou o id de contrato."); }

                        cob = new Cobranca();
                        cob.Tipo = (int)Cobranca.eTipo.DiferencaUbraSP;
                        cob.Parcela = 0;
                        //cob.DataVencimento = Convert.ToDateTime(row["vencimento"]);
                        //cob.DataVencimento = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, cob.DataVencimento.Day, 23, 59, 59, 995);
                        cob.DataVencimento = vencimento;
                        cob.Valor = vo.Residual;
                        cob.CobrancaRefID = null;
                        cob.DataPgto = DateTime.MinValue;
                        cob.ValorPgto = Decimal.Zero;
                        cob.Pago = false;
                        cob.PropostaID = aux;
                        cob.Cancelada = false;
                        cob.ArquivoIDUltimoEnvio = -2;
                        cob.Carteira = 101;
                        cob.Banco = 353;

                        pm.Save(cob);
                    }

                    //gera o agendamento do arquivo
                    ArquivoRemessaAgendamento ara = new ArquivoRemessaAgendamento();
                    ara.CriterioID = null;
                    ara.Processado = false;
                    ara.ProcessamentoEm = DateTime.Now.AddMinutes(10);
                    ara.VencimentoAte = vencimento;
                    ara.VencimentoDe = vencimento;
                    ara.VigenciaDe = new DateTime(1800, 1, 1, 23, 59, 59, 995);
                    ara.VigenciaAte = DateTime.Now.AddDays(90);
                    ara.QtdBoletos = 1;
                    ara.Banco = 353;
                    ara.Carteira = 101;

                    ara.SomenteBoletosUBRASP = false;
                    ara.SomenteNaoRecorrentes = true;

                    ara.Grupo = null;
                    ara.ArquivoNomeInstance = "remessa_naodescontados";
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
        public string BeneficiarioId { get; set; }
        public string NomeLocalizado { get; set; }
        public string Funcional { get; set; }
        public string CPF { get; set; }
        public string ORGAO { get; set; }
        //public string NUAverb { get; set; }
        public string Parcela { get; set; }
        public string Status { get; set; }
        public decimal ValorArquivo { get; set; }
        public decimal ValorSistema { get; set; }
        /// <summary>
        /// Valor que foi descontado e informado no arquivo - TOTAL
        /// </summary>
        public decimal ValorDescontado { get; set; }
        public decimal Residual { get; set; }
        public bool Marcado { get; set; }
        public string Atualizado { get; set; }
    }

    //string[] linhas = File.ReadAllLines(arquivoNome);
    //List<ItemVO> vos = null; // this.processaArquivoEnviado(linhas);
    //grid.DataSource = vos;
    //grid.DataBind();
}
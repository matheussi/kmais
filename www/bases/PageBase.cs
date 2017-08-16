namespace www
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using Impactro.Cobranca;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using LC.Web.PadraoSeguros.Entity;

    public class PageBase : Page
    {
        #region Private Fields

        protected readonly String ArqTransacionalRelativePath = ConfigurationManager.AppSettings["transactFilePath"];

        #endregion

        #region Private Members

        /// <summary>
        /// Path para os Arquivos Transacionais.
        /// </summary>
        protected String ArqTransacionalFilePath
        {
            get
            {
                return String.Concat(Server.MapPath("/"), ArqTransacionalRelativePath.Replace("/", "\\"));
            }
        }

        /// <summary>
        /// Caminho base da pasta de boletos
        /// </summary>
        protected String ArqBoletoPath
        {
            get
            {
                return String.Concat(Server.MapPath("/"), ConfigurationManager.AppSettings["boletoFilePath"].Replace("/", "\\"));
            }
        }

        #endregion

        protected string _mailToken = "d1352873-cd5f-4374-98ca-1912ec02333a";

        protected CedenteInfo GetCedente()
        {
            // Definição dos dados do cedente
            CedenteInfo Cedente = new CedenteInfo();
            Cedente.Cedente = "UBRASP UNIÃO BRASILEIRA DOS SERVIDORES PÚBLICOS";
            Cedente.Banco = "001-9";
            Cedente.Agencia = "5956-0";
            Cedente.Conta = "40037-8";
            Cedente.CNPJ = "49.938.327.0001-06";
            // ATENÇÃO: Geralmente o banco informa a Carteira da seguinte forma: 16/019
            // Para o gerador isso significa sempre CARTEIRA/MODALIDADE, e ambas com apenas 2 dígitos
            // E estes devem ser configurados separadamente como indicado abaixo neste exemplo
            Cedente.Carteira = "11";//Cedente.Carteira = "18";
            Cedente.Modalidade = "1"; //Cedente.Modalidade = "21"; //Modalidade da carteira: COBRANÇA SIMPLES
            Cedente.Convenio = "2887755"; //Cedente.Convenio = "859120";

            return Cedente;
        }

        protected ObjectStateFormatter _formatter = new ObjectStateFormatter(); 

        protected PageBase() { }

        protected String[] PegaIDsSelecionados(ListBox lst)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (ListItem item in lst.Items)
            {
                if (item.Selected && item.Value != "-1")
                {
                    if (sb.Length > 0) { sb.Append(","); }
                    sb.Append(item.Value);
                }
            }

            if (sb.Length == 0)
                return null;
            else
                return sb.ToString().Split(',');
        }

        protected DateTime UltimoDiaDoMes(Int32 mes, Int32 ano)
        {
            DateTime data = new DateTime(ano, mes, 1);
            return data.AddMonths(1).AddDays(-1);
        }

        protected void CarregarTiposDeCobranca(DropDownList cbo)
        {
            cbo.Items.Clear();
            cbo.Items.Add(new ListItem("Normal", "0"));
            cbo.Items.Add(new ListItem("Dupla", "2"));
            cbo.Items.Add(new ListItem("Complementar", "1"));
        }

        /// <summary>
        /// Gera o arquivo físico em disco e inicia seu download.
        /// </summary>
        /// <param name="path">Caminho do arquivo.</param>
        /// <param name="arquivoNome">Nome do arquivo.</param>
        protected void BaixarArquivo(String path, String arquivoNome)
        {
            this.BaixarArquivo(null, path, arquivoNome);
        }

        /// <summary>
        /// Gera o arquivo físico em disco e inicia seu download.
        /// </summary>
        /// <param name="conteudo">Conteúdo do arquivo.</param>
        /// <param name="path">Caminho do arquivo.</param>
        /// <param name="arquivoNome">Nome do arquivo.</param>
        protected void BaixarArquivo(String conteudo, String path, String arquivoNome)
        {
            System.IO.FileStream arquivo = null;
            
            // Caso venha conteúdo ele Cria e Escreve
            if (!String.IsNullOrEmpty(conteudo))
            {
                this.EscreveArquivo(conteudo, path, arquivoNome);
            }

            try
            {
                if(path.IndexOf (arquivoNome) == -1)
                    arquivo = new System.IO.FileStream(path + arquivoNome, System.IO.FileMode.Open);
                else
                    arquivo = new System.IO.FileStream(path, System.IO.FileMode.Open);
            }
            catch (System.IO.FileNotFoundException)
            {
                throw;
            }

            Byte[] arrByte = new Byte[arquivo.Length];
            arquivo.Read(arrByte, 0, arrByte.Length);
            arquivo.Close();
            arquivo.Dispose();
            arquivo = null;

            this.Response.Clear();
            this.Response.ContentType = "application/octet-stream";
            this.Response.AppendHeader("Content-Length", arrByte.Length.ToString());
            this.Response.AppendHeader("Content-Disposition", String.Concat("attachment; filename=", arquivoNome));
            this.Response.BinaryWrite(arrByte);
            this.Response.Flush();
        }

        /// <summary>
        /// Escreve o arquivo em disco.
        /// </summary>
        /// <param name="conteudo">Conteúdo do arquivo.</param>
        /// <param name="path">Caminho do arquivo.</param>
        /// <param name="arquivoNome">Nome do arquivo.</param>
        protected void EscreveArquivo(String conteudo, String path, String arquivoNome)
        {
            try
            {
                File.WriteAllText(path + arquivoNome, conteudo, System.Text.Encoding.GetEncoding("iso-8859-1"));
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Lê o conteúdo de um arquivo.
        /// </summary>
        /// <param name="path">Caminho do arquivo.</param>
        /// <param name="arquivoNome">Nome do arquivo.</param>
        /// <returns>String contendo o conteúdo do arquivo, ou null, caso ele não exista.</returns>
        protected String LeArquivo(String path, String arquivoNome)
        {
            if (!File.Exists(path + arquivoNome)) { return null; }
            String content = File.ReadAllText(path + arquivoNome, System.Text.Encoding.GetEncoding("iso-8859-1"));
            return content;
        }

        /// <summary>
        /// Método para baixar o arquivo de remessa.
        /// </summary>
        /// <param name="ArquivoNome">Nome do arquivo de remessa.</param>
        protected void BaixaArquivoFinanceiro(String arquivoNome)
        {
            if (!String.IsNullOrEmpty(arquivoNome))
            {
                String ArquivoCaminho = Server.MapPath("/") + ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                if (!System.IO.Directory.Exists(ArquivoCaminho))
                {
                    System.IO.Directory.CreateDirectory(ArquivoCaminho);
                }

                String strFilePath = String.Concat(ArquivoCaminho, arquivoNome);
                //System.IO.File.WriteAllText(strFilePath, content, System.Text.Encoding.ASCII);

                System.IO.FileStream fileStream = null;

                try
                {
                    fileStream = new System.IO.FileStream(strFilePath, System.IO.FileMode.Open);
                }
                catch (System.IO.FileNotFoundException)
                {
                    throw;
                }

                Byte[] arrByte = new Byte[fileStream.Length];
                fileStream.Read(arrByte, 0, arrByte.Length);
                fileStream.Close();
                fileStream.Dispose();
                fileStream = null;

                this.Response.Clear();
                this.Response.ContentType = "application/octet-stream";
                this.Response.AppendHeader("Content-Length", arrByte.Length.ToString());
                this.Response.AppendHeader("Content-Disposition", String.Concat("attachment; filename=", arquivoNome));
                this.Response.BinaryWrite(arrByte);
                this.Response.Flush();
            }
        }

        //protected override void SavePageStateToPersistenceMedium(Object viewState)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    _formatter.Serialize(ms, viewState);
        //    Byte[] viewStateArray = ms.ToArray();

        //    ClientScript.RegisterHiddenField("__COMPRESSEDVIEWSTATE",
        //        Convert.ToBase64String(UIHelper.Compress(viewStateArray)));
        //}

        //protected override Object LoadPageStateFromPersistenceMedium()
        //{
        //    String vsString = Request.Form["__COMPRESSEDVIEWSTATE"];
        //    Byte[] bytes = Convert.FromBase64String(vsString);
        //    bytes = UIHelper.Decompress(bytes);
        //    return _formatter.Deserialize(Convert.ToBase64String(bytes));
        //}

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (String.IsNullOrEmpty(Usuario.Autenticado.ID)) //&& Request.Url.ToString().IndexOf("boleto/bb.aspx") == -1)
            {
                Response.Redirect("~/login.aspx?msg=expired_session");
            }

            //if (Page.Request.ServerVariables["http_user_agent"].ToLower().Contains("safari"))
            //{
                Page.ClientTarget = "uplevel";
            //}
        }

        protected Boolean ValidaEmail(String email, out String mensagem)
        {
            mensagem = "";

            if (email.Trim() == "") { return true; }

            String pattern = "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$";

            Regex rg = new Regex(pattern);
            Boolean result = rg.IsMatch(email);

            if (!result)
                mensagem = "E-mail inválido.";

            return result;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected Boolean HaItemSelecionado(DropDownList combo)
        {
            if (combo.Items.Count == 0) { return false; }

            return combo.SelectedValue != "0" &&
                   combo.SelectedValue != "-1" &&
                   combo.SelectedValue != "";
        }

        protected Boolean useExternalCEPEngine()
        {
            String use = ConfigurationManager.AppSettings["useExternalCEPEngine"];

            return use != null && use.ToUpper() == "Y";
        }

        protected String[] PegaEndereco(Page page, TextBox txtCEP, TextBox txtRua, TextBox txtBairro, TextBox txtCidade, TextBox txtUF, TextBox txtNumero, Boolean exibeAlerta)
        {
            if (txtRua != null) { txtRua.Text = ""; }
            if (txtBairro != null) { txtBairro.Text = ""; }
            if (txtCidade != null) { txtCidade.Text = ""; }
            if (txtUF != null) { txtUF.Text = ""; }

            String xml = "";

            try
            {
                String url = String.Concat("http://cep.republicavirtual.com.br/web_cep.php?cep=", txtCEP.Text.Replace("-", ""), "&formato=xml");
                System.Net.WebRequest request = System.Net.WebRequest.Create(url);
                System.Net.WebResponse response = request.GetResponse();

                System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("ISO-8859-1"));
                xml = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                response.Close();
            }
            catch
            {
                if (exibeAlerta)
                {
                    Alerta(null, page, "_errCep0", "CEP não encontrado.\\nVerfique os dados informados e tente novamente.");
                }

                txtCEP.Focus();
                return null;
            }

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);

            System.Xml.XmlNode root = doc.DocumentElement;
            System.Xml.XmlNode current = root.SelectSingleNode("/webservicecep/resultado");

            if (current.InnerText == "0")
            {
                if (exibeAlerta)
                {
                    Alerta(null, page, "_errCep1", "CEP não encontrado.\\nVerfique os dados informados e tente novamente.");
                }

                txtCEP.Focus(); 
                return null;
            }

            current = root.SelectSingleNode("/webservicecep/uf");
            txtUF.Text = current.InnerText.ToUpper();

            current = root.SelectSingleNode("/webservicecep/cidade");
            txtCidade.Text = current.InnerText;

            current = root.SelectSingleNode("/webservicecep/bairro");
            txtBairro.Text = current.InnerText;

            current = root.SelectSingleNode("/webservicecep/tipo_logradouro");
            String tipoLogradouro = current.InnerText;

            current = root.SelectSingleNode("/webservicecep/logradouro");
            String logradouro = current.InnerText;

            txtRua.Text = tipoLogradouro + " " + logradouro;

            if(exibeAlerta)
                txtNumero.Focus();

            String[] arr = new String[] { txtUF.Text, txtCidade.Text, txtBairro.Text, txtRua.Text };
            return arr;
        }

        /// <summary>
        /// Emite alertas ao usuário.
        /// </summary>
        protected void Alerta(UpdatePanel uPanel, Page page, String chave, String Mensagem)
        {
            if (uPanel != null)
            {
                ScriptManager.RegisterClientScriptBlock(
                    uPanel, page.GetType(), chave, "alert('" + Mensagem + "');", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(
                    page, page.GetType(), chave, "alert('" + Mensagem + "');", true);
            }
        }

        protected void Alerta(AjaxControlToolkit.ModalPopupExtender MPE, ref Literal lit, String msg, UpdatePanel panel)
        {
            lit.Text = String.Concat("<font face='arial' size='1'>", msg, "</font>");
            MPE.Show();

            if (panel != null) { panel.Update(); }
        }

        /// <summary>
        /// Retorna a diferença de tempo entre uma data e a data correte.
        /// </summary>
        /// <param name="interval">Indica qual será o retorno [1 = Tempo Completo (N anos N meses N dias), 2 = Anos]</param>
        /// <param name="data">Data Inicial</param>
        /// <returns>Retorna a diferença de acordo com o Intervalo escolhido</returns>
        protected String DateDiff(int interval, DateTime data)
        {
            String retorno = "";

            TimeSpan tsDuration;
            tsDuration = DateTime.Now - data;

            Int32 dias = 0;
            Decimal iMeses = 0;
            Int32 meses = 0;
            Decimal iAnos = 0;
            Int32 anos = 0;

            if (interval == 1)
            {
                iAnos = Convert.ToDecimal(tsDuration.Days / 365.25);
                anos = (int)iAnos;
                iMeses = Convert.ToDecimal((iAnos - anos) * 12);
                meses = (int)iMeses;
                dias = (int)((iMeses - meses) * 24);

                TimeSpan tsDurationDia;
                data = data.AddYears(anos);
                data = data.AddMonths(meses);
                tsDurationDia = DateTime.Now - data;

                retorno = Convert.ToString(anos + "a " + meses + "m " + tsDurationDia.Days + "d");
            }
            else if (interval == 2)
            //    retorno = Convert.ToString(Convert.ToInt32(tsDuration.Days / 365));
            //else if (interval == 3)
            {
                iAnos = Convert.ToDecimal(tsDuration.Days / 365.25);
                anos = (int)iAnos;

                TimeSpan tsDurationDia;
                data = data.AddYears(anos);
                data = data.AddMonths(meses);
                tsDurationDia = DateTime.Now - data;

                retorno = anos.ToString();
            }

            return retorno;
        }

        protected int GetMonthsBetween(DateTime from, DateTime to)
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }

            var months = 0;
            while (from <= to) // at least one time
            {
                from = from.AddMonths(1);
                months++;
            }

            return months - 1;
        }

        protected String TraduzTipoRelacaoDependenteContrato(int tipo)
        {
            ContratoBeneficiario.TipoRelacao tipoEnum = (ContratoBeneficiario.TipoRelacao)tipo;

            if (tipoEnum == ContratoBeneficiario.TipoRelacao.Agregado)
                return "Agregado";
            else if (tipoEnum == ContratoBeneficiario.TipoRelacao.Dependente)
                return "Dependente";
            else if (tipoEnum == ContratoBeneficiario.TipoRelacao.Titular)
                return "Titular";

            return String.Empty;
        }

        

        protected DateTime? CStringToDateTimeG(String strdata)
        {
            String[] arr = strdata.Split('/');
            if (arr.Length != 3) { return null; }

            return CStringToDateTime(strdata);
        }
        protected DateTime CStringToDateTime(String strdata)
        {
            String[] arr = strdata.Split('/');

            if (arr.Length != 3) { return DateTime.MinValue; }

            try
            {
                DateTime data = new DateTime(Int32.Parse(arr[2]), Int32.Parse(arr[1]), Int32.Parse(arr[0]));
                return data;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        protected DateTime CStringToDateTime(String strdata, String strhora)
        {
            String[] arr = strdata.Split('/');
            String[] arrH = strhora.Split(':');

            if (arr.Length != 3) { return DateTime.MinValue; }
            if (arrH.Length != 2) { return DateTime.MinValue; }
            try
            {
                DateTime data = new DateTime(Int32.Parse(arr[2]), Int32.Parse(arr[1]), Int32.Parse(arr[0]), Int32.Parse(arrH[0]), Int32.Parse(arrH[1]), Int32.Parse(DateTime.Now.Second.ToString()));
                return data;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        protected DateTime CStringToDateTime(String strdata, String strhora, Int32 segundos)
        {
            String[] arr = strdata.Split('/');
            String[] arrH = strhora.Split(':');

            if (arr.Length != 3) { return DateTime.MinValue; }
            if (arrH.Length != 2) { return DateTime.MinValue; }
            try
            {
                DateTime data = new DateTime(Int32.Parse(arr[2]), Int32.Parse(arr[1]), Int32.Parse(arr[0]), Int32.Parse(arrH[0]), Int32.Parse(arrH[1]), segundos);
                return data;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        protected DateTime CStringToDateTime(String strdata, String strhora, Int32 segundos, int milessegundos)
        {
            String[] arr = strdata.Split('/');
            String[] arrH = strhora.Split(':');

            if (arr.Length != 3) { return DateTime.MinValue; }
            if (arrH.Length != 2) { return DateTime.MinValue; }
            try
            {
                DateTime data = new DateTime(Int32.Parse(arr[2]), Int32.Parse(arr[1]), Int32.Parse(arr[0]), Int32.Parse(arrH[0]), Int32.Parse(arrH[1]), segundos, milessegundos);
                return data;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        protected String CToString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return String.Empty;
            else
                return Convert.ToString(param);
        }

        protected Object CToObject(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return null;
            else
                return param;
        }

        protected int CToInt(Object param)
        {
            return UIHelper.CToInt(param);
        }

        protected Decimal CToDecimal(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
                return Convert.ToDecimal(param, new System.Globalization.CultureInfo("pt-Br"));
        }

        protected Decimal CToDecimal(Object param, System.Globalization.CultureInfo cinfo)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
                return Convert.ToDecimal(param, cinfo);
        }

        protected Boolean IDKeyParameterInProcess(StateBag viewstate, String keyToUseInCache)
        {
            if (Session[IDKey] != null)
            {
                viewstate[IDKey] = Session[IDKey];
                Session.Remove(IDKey);
                //Cache.Remove(IDKey + keyToUseInCache);
                //Cache.Insert(IDKey + keyToUseInCache, viewstate[IDKey],
                //    null, DateTime.Now.AddMinutes(35), TimeSpan.Zero);
                return true;
            }
            else if (Cache[IDKey + keyToUseInCache] != null)
            {
                viewstate[IDKey] = Cache[IDKey + keyToUseInCache];
                return true;
            }
            else if (Request[IDKey] != null)
            {
                viewstate[IDKey] = Request[IDKey];
                return true;
            }
            else
                return false;
        }

        protected const String IDKey                = "_idkey";
        protected const String IDKey2               = "_idkey2";
        protected const String IDKey3               = "_idkey3";
        protected const String IDKey4               = "_idkey4";
        protected const String IDAdicionalKey       = "_idAdicK";
        protected const String AlteraPlanoKey       = "_altPlan";
        protected const String NovaDataAdmisssaoKey = "_nvDtAdm";
        protected const String PropostaEndReferecia = "_propEndRef";
        protected const String PropostaEndCobranca  = "_propEndCob";
        protected const String ConferenciaObjKey    = "_confObjKey";
        protected const String ArquivosObjKey       = "_arquivosObjKey";
        protected const String FilialIDKey          = "_filialIDKey";

        protected void grid_RowDataBound_Confirmacao(Object sender, GridViewRowEventArgs e, int indiceControle, String Msg)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[indiceControle].Attributes.Add("onClick", "return confirm('" + Msg + "');");
            }
        }

        protected String PegaDDD(String fone)
        {
            if (String.IsNullOrEmpty(fone)) { return String.Empty; }

            String[] aux = fone.Split(')');
            return aux[0].Replace("(", "").Trim();
        }
        protected String PegaTelefone(String fone)
        {
            if (String.IsNullOrEmpty(fone)) { return String.Empty; }

            String[] aux = fone.Split(')');
            if (aux.Length == 1) { return fone; }

            return aux[1].Trim();
        }

        protected void PreencheComboComOpcoesDeCalendario(DropDownList combo, Boolean limpaCombo)
        {
            if (limpaCombo) { combo.Items.Clear(); }

            combo.Items.Add(new ListItem("mês anterior", "-1"));
            combo.Items.Add(new ListItem("mês atual", "0"));
            combo.Items.Add(new ListItem("mês próximo", "1"));
            combo.Items.Add(new ListItem("mês seguinte ao próximo", "2"));
        }

        protected void PreencheComboNumerico(DropDownList combo, int inicio, int fim, Boolean limpaCombo)
        {
            if (limpaCombo) { combo.Items.Clear(); }

            String mascara = new String('0', 2);

            for (int i = inicio; i <= fim; i++)
            {
                combo.Items.Add(new ListItem(String.Format("{0:" + mascara + "}", i), i.ToString()));
            }
        }

        protected void ExibirEstadosCivis(DropDownList combo, Boolean itemSELECIONE, Object operadoraId)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = EstadoCivil.CarregarTodos(operadoraId);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirEstadosCivisDeUsuario(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = EstadoCivilUsuario.CarregarTodos();
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirOperadoras(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Nome";
            combo.DataSource = Operadora.CarregarTodas(true);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirOperadoras(DropDownList combo, Boolean itemSELECIONE, Boolean somenteAtivas)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Nome";
            combo.DataSource = Operadora.CarregarTodas(somenteAtivas);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirOperadoras(ListBox list)
        {
            list.Items.Clear();
            list.DataValueField = "ID";
            list.DataTextField = "Nome";
            list.DataSource = Operadora.CarregarTodas(true);
            list.DataBind();
        }

        protected void ExibirPlanos(DropDownList combo, Object contratoID, Boolean itemSELECIONE, Boolean apenasAtivos)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = Plano.CarregarPorContratoID(contratoID, apenasAtivos);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirEstipulantes(DropDownList combo, Boolean itemSELECIONE, Boolean apenasAtivos)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField  = "Descricao";
            combo.DataSource     = Estipulante.Carregar(apenasAtivos);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirOpcoesDeSexo(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();

            if (itemSELECIONE)
                combo.Items.Add(new ListItem("Selecione", "-1"));

            combo.Items.Add(new ListItem("MASCULINO", "1"));
            combo.Items.Add(new ListItem("FEMININO", "2"));
        }

        protected void ExibirOpcoesDeTipoDeContrato(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();
            //combo.Items.Add(new ListItem("NOVA", "1"));
            //combo.Items.Add(new ListItem("COMPRA DE CARÊNCIA", "2"));
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = TipoContrato.Carregar(true);
            combo.DataBind();

            if (itemSELECIONE)
                combo.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        protected void ExibirParentescos(DropDownList combo, Object planoId, Boolean itemSELECIONE, Parentesco.eTipo tipo)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";

            if (planoId == null)
                combo.DataSource = Parentesco.CarregarTodos(tipo);
            else
                combo.DataSource = Parentesco.CarregarNaoUsadosPor(planoId, tipo);

            combo.DataBind();
            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirCategorias(DropDownList cboCategoriaComissionamento, Boolean apenasAtivos, Boolean itemSelecione)
        {
            cboCategoriaComissionamento.Items.Clear();
            cboCategoriaComissionamento.DataValueField = "ID";
            cboCategoriaComissionamento.DataTextField = "Descricao";
            cboCategoriaComissionamento.DataSource = Categoria.Carregar(apenasAtivos);
            cboCategoriaComissionamento.DataBind();

            if(itemSelecione)
                cboCategoriaComissionamento.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        protected void ExibirFiliais(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();

            combo.DataValueField = "ID";
            combo.DataTextField = "Nome";
            combo.DataSource = Filial.CarregarTodas(true);
            combo.DataBind();

            if (itemSELECIONE) { combo.Items.Insert(0, new ListItem("Selecione", "-1")); }
        }

        protected void ExibirTiposDeProdutos(DropDownList combo, Boolean itemSELECIONE, String textoSELECIONE)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = AlmoxTipoProduto.CarregarTodos();
            combo.DataBind();

            if (itemSELECIONE) { combo.Items.Insert(0, new ListItem(textoSELECIONE, "-1")); }
        }

        protected void ExibirProdutos(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField  = "Descricao";
            combo.DataSource = AlmoxProduto.CarregarTodos(true);
            combo.DataBind();

            if (itemSELECIONE) { combo.Items.Insert(0, new ListItem("Selecione", "-1")); }
        }

        protected void ExibirTiposDeAcomodacao(DropDownList combo, Boolean comum, Boolean particular, Boolean itemSELECIONE)
        {
            combo.Items.Clear();
            if (itemSELECIONE)
                combo.Items.Add(new ListItem("Selecione", "-1"));

            if(comum)
                combo.Items.Add(new ListItem("QUARTO COLETIVO", "0"));

            if(particular)
                combo.Items.Add(new ListItem("QUARTO PARTICULAR", "1"));
        }

        protected void ExibirTipoDeTaxaEstipulante(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();

            if (itemSELECIONE)
                combo.Items.Add(new ListItem("Selecione", "-1"));

            combo.Items.Add(new ListItem("POR VIDA", "0"));
            combo.Items.Add(new ListItem("POR PROPOSTA", "1"));
        }

        protected void ExibirTiposDeConta(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("Conta corrente", "CONTA CORRENTE"));
            combo.Items.Add(new ListItem("Conta poupança", "CONTA POUPANCA"));
            combo.Items.Add(new ListItem("Cartão salário", "CARTAO SALARIO"));
        }

        protected void ExibirTipoDeProducao(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("CONTRATOS", "CONTRATOS"));
            combo.Items.Add(new ListItem("VALOR", "VALOR"));
            combo.Items.Add(new ListItem("VIDAS", "VIDAS"));
        }

        protected void ExibirTiposDeAtendimento(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("Reclamação", "0"));
            combo.Items.Add(new ListItem("Segunda Via Boleto", "1"));
            combo.Items.Add(new ListItem("Cancelamento de contrato", "2"));
            combo.Items.Add(new ListItem("Segunda Via Cartão", "3"));
            combo.Items.Add(new ListItem("Alteração no Cadastro", "4"));
            combo.Items.Add(new ListItem("Mudança de Plano", "5"));
            combo.Items.Add(new ListItem("Adicionar Beneficiário", "6"));
            combo.Items.Add(new ListItem("Cancelar Beneficiário", "7"));
        }

        #region traduzTipoStatusContratoBeneficiario

        /// <summary>
        /// Traduz o Tipo de Status do ContratoBeneficiario (ATENDIMENTO).
        /// </summary>
        /// <returns>Retorna um Tipo de Status do ContratoBeneficiario.</returns>
        protected ContratoBeneficiario.eStatus traduzTipoStatusContratoBeneficiario(String Valor)
        {
            if (Valor == "2") //cancelamento
                return ContratoBeneficiario.eStatus.CancelamentoPendente;
            else if (Valor == "3") //2via de cartao
                return ContratoBeneficiario.eStatus.SegundaViaCartaoPendente;
            else if (Valor == "4") // alteracao de cadastro
                return ContratoBeneficiario.eStatus.AlteracaoCadastroPendente;
            else if (Valor == "5") // mudanca de plano
                return ContratoBeneficiario.eStatus.MudancaPlanoPendente;
            else if (Valor == "6") // adicionar beneficiario
                return ContratoBeneficiario.eStatus.Novo;
            else
                return ContratoBeneficiario.eStatus.Desconhecido;
        }

        #endregion

        #region traduzTipoStatusContratoBeneficiarioTipoArquivo

        /// <summary>
        /// Traduz o Tipo de Status do ContratoBeneficiario (TIPO DE ARQUIVO).
        /// </summary>
        /// <param name="Valor">Valor a ser traduzido.</param>
        /// <returns>Retorna um Tipo de Status do ContratoBeneficiario.</returns>
        protected ContratoBeneficiario.eStatus traduzTipoStatusContratoBeneficiarioTipoArquivo(String Valor)
        {
            try
            {
                return this.traduzTipoStatusContratoBeneficiarioTipoArquivo(Valor, false, false, false);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Traduz o Tipo de Status do ContratoBeneficiario (TIPO DE ARQUIVO).
        /// </summary>
        /// <param name="Valor">Valor a ser traduzido.</param>
        /// <param name="IsDevolvido">True se for uma situação de Devolução.</param>
        /// <param name="IsRegerar">True se for uma situação de Regerar um lote, caso os Beneficiarios tenham sido recusados na Operdora.</param>
        /// <param name="IsBaixa">True se for uma situação de Baixa.</param>
        /// <returns>Retorna um Tipo de Status do ContratoBeneficiario.</returns>
        protected ContratoBeneficiario.eStatus traduzTipoStatusContratoBeneficiarioTipoArquivo(String Valor, Boolean IsDevolvido, Boolean IsRegerar, Boolean IsBaixa)
        {
            if (String.IsNullOrEmpty(Valor))
                throw new ArgumentNullException("O Valor do Tipo de Status não pode ser nulo.");

            if (Valor == "6") //cancelamento
                return (IsDevolvido) ? ContratoBeneficiario.eStatus.CancelamentoDevolvido : 
                       (IsRegerar) ? ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora :
                       (IsBaixa) ? ContratoBeneficiario.eStatus.Cancelado : ContratoBeneficiario.eStatus.CancelamentoPendente;
            else if (Valor == "5") //2via de cartao
                return (IsDevolvido) ? ContratoBeneficiario.eStatus.SegundaViaCartaoDevolvido : 
                       (IsRegerar) ? ContratoBeneficiario.eStatus.SegundaViaCartaoPendenteNaOperadora :
                       (IsBaixa) ? ContratoBeneficiario.eStatus.Incluido : ContratoBeneficiario.eStatus.SegundaViaCartaoPendente;
            else if (Valor == "1") // alteracao de cadastro
                return (IsDevolvido) ? ContratoBeneficiario.eStatus.AlteracaoCadastroDevolvido : 
                       (IsRegerar) ? ContratoBeneficiario.eStatus.AlteracaoCadastroPendenteNaOperadora :
                       (IsBaixa) ? ContratoBeneficiario.eStatus.Incluido : ContratoBeneficiario.eStatus.AlteracaoCadastroPendente;
            else if (Valor == "4") // mudanca de plano
                return (IsDevolvido) ? ContratoBeneficiario.eStatus.MudancaDePlanoDevolvido : 
                       (IsRegerar) ? ContratoBeneficiario.eStatus.MudancaPlanoPendenteNaOperadora :
                       (IsBaixa) ? ContratoBeneficiario.eStatus.Incluido : ContratoBeneficiario.eStatus.MudancaPlanoPendente;
            else if (Valor == "2" || Valor == "0") // adicionar beneficiario
                return (IsDevolvido) ? ContratoBeneficiario.eStatus.Devolvido : 
                       (IsRegerar) ? ContratoBeneficiario.eStatus.PendenteNaOperadora :
                       (IsBaixa) ? ContratoBeneficiario.eStatus.Incluido : ContratoBeneficiario.eStatus.Novo;
            else if (Valor == "3") // cancelar beneficiario
                return (IsDevolvido) ? ContratoBeneficiario.eStatus.ExclusaoDevolvido : 
                       (IsRegerar) ? ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora :
                       (IsBaixa) ? ContratoBeneficiario.eStatus.Excluido : ContratoBeneficiario.eStatus.ExclusaoPendente;
            else
                return ContratoBeneficiario.eStatus.Desconhecido;
        }

        protected ContratoBeneficiario.eStatus traduzTipoStatusContratoBeneficiarioTipoArquivo_BAIXA(String Valor, Boolean IsDevolvido, Boolean IsRegerar, Boolean IsBaixa)
        {
            if (String.IsNullOrEmpty(Valor))
                throw new ArgumentNullException("O Valor do Tipo de Status não pode ser nulo.");

            if (Valor == "6") //cancelamento
                return ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora;
            else if (Valor == "5") //2via de cartao
                return (IsDevolvido) ? ContratoBeneficiario.eStatus.SegundaViaCartaoDevolvido :
                       (IsRegerar) ? ContratoBeneficiario.eStatus.SegundaViaCartaoPendenteNaOperadora :
                       (IsBaixa) ? ContratoBeneficiario.eStatus.Incluido : ContratoBeneficiario.eStatus.SegundaViaCartaoPendente;
            else if (Valor == "1") // alteracao de cadastro
                return (IsDevolvido) ? ContratoBeneficiario.eStatus.AlteracaoCadastroDevolvido :
                       (IsRegerar) ? ContratoBeneficiario.eStatus.AlteracaoCadastroPendenteNaOperadora :
                       (IsBaixa) ? ContratoBeneficiario.eStatus.Incluido : ContratoBeneficiario.eStatus.AlteracaoCadastroPendente;
            else if (Valor == "4") // mudanca de plano
                return (IsDevolvido) ? ContratoBeneficiario.eStatus.MudancaDePlanoDevolvido :
                       (IsRegerar) ? ContratoBeneficiario.eStatus.MudancaPlanoPendenteNaOperadora :
                       (IsBaixa) ? ContratoBeneficiario.eStatus.Incluido : ContratoBeneficiario.eStatus.MudancaPlanoPendente;
            else if (Valor == "2" || Valor == "0") // adicionar beneficiario
                return (IsDevolvido) ? ContratoBeneficiario.eStatus.Devolvido :
                       (IsRegerar) ? ContratoBeneficiario.eStatus.PendenteNaOperadora :
                       (IsBaixa) ? ContratoBeneficiario.eStatus.Incluido : ContratoBeneficiario.eStatus.Novo;
            else if (Valor == "3") // cancelar beneficiario
                return (IsDevolvido) ? ContratoBeneficiario.eStatus.ExclusaoDevolvido :
                       (IsRegerar) ? ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora :
                       (IsBaixa) ? ContratoBeneficiario.eStatus.Excluido : ContratoBeneficiario.eStatus.ExclusaoPendente;
            else
                return ContratoBeneficiario.eStatus.Desconhecido;
        }

        #endregion

        #region traduzTipoStatusContratoBeneficiarioMovimentacao

        /// <summary>
        /// Traduz o Tipo de Status Contrato Beneficiário para um valor de Movimentação (LOTE).
        /// </summary>
        /// <param name="Valor">Valor a ser traduzido.</param>
        /// <returns>Retorna uma String com o status da movimentação.</returns>
        protected String traduzTipoStatusContratoBeneficiarioMovimentacao(String Valor)
        {
            if (Valor == "6") //cancelamento
                return ArqTransacionalUnimed.Movimentacao.CancelamentoContrato;
            else if (Valor == "5") //2via de cartao
                return ArqTransacionalUnimed.Movimentacao.SegundaViaCartaoBeneficiario;
            else if (Valor == "1") // alteracao de cadastro
                return ArqTransacionalUnimed.Movimentacao.AlteracaoBeneficiario;
            else if (Valor == "4") // mudanca de plano
                return ArqTransacionalUnimed.Movimentacao.MudancaDePlano;
            else if (Valor == "2" || Valor == "0") // adicionar beneficiario
                return ArqTransacionalUnimed.Movimentacao.InclusaoBeneficiario;
            else if (Valor == "3") // cancelar beneficiario
                return ArqTransacionalUnimed.Movimentacao.ExclusaoBeneficiario;
            else
                return null;
        }

        #endregion

        #region traduzMovimentacaoLote

        /// <summary>
        /// Traduz a Movimentação do Lote.
        /// </summary>
        /// <param name="Valor">Valor a ser traduzido.</param>
        /// <returns>Retorna uma String com a Movimentação do Lote.</returns>
        protected String traduzMovimentacaoLote(String Valor)
        {
            switch (Valor)
            {
                case ArqTransacionalUnimed.Movimentacao.CancelamentoContrato:
                    return "Cancelamento de Contrato";
                case ArqTransacionalUnimed.Movimentacao.SegundaViaCartaoBeneficiario:
                    return "Emissão de 2º Via de Cartão";
                case ArqTransacionalUnimed.Movimentacao.AlteracaoBeneficiario:
                    return "Alteração de Beneficiário";
                case ArqTransacionalUnimed.Movimentacao.MudancaDePlano:
                    return "Mudança de Plano";
                case ArqTransacionalUnimed.Movimentacao.InclusaoBeneficiario:
                    return "Inclusão de Beneficiário";
                case ArqTransacionalUnimed.Movimentacao.ExclusaoBeneficiario:
                    return "Exclusão de Beneficiário";
                default:
                    return Valor;
            }
        }

        #endregion

        #region layout de arquivo customizado 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="combo"></param>
        /// <param name="sincSeg">Arquivo para sincronização com o SEG</param>
        protected void ExibirTiposDeArquivo(DropDownList combo, Boolean sincSeg)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("Inclusão de contrato", "0"));
            combo.Items.Add(new ListItem("Cancelar contrato", "6"));
            combo.Items.Add(new ListItem("Alteração cadastral", "1"));
            combo.Items.Add(new ListItem("Adicionar beneficiário", "2"));
            combo.Items.Add(new ListItem("Cancelar beneficiário", "3"));
            combo.Items.Add(new ListItem("Mudança de plano", "4"));
            combo.Items.Add(new ListItem("2ª Via de cartão", "5"));

            if(sincSeg)
                combo.Items.Add(new ListItem("SINCRONIZAÇÃO SEG", "7"));
        }

        protected void ExibirFormatosDeArquivo(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("Texto formatado", "0"));
            combo.Items.Add(new ListItem("Texto delimitado", "1"));
            combo.Items.Add(new ListItem("Planilha Xls", "2"));
            combo.Items.Add(new ListItem("Documento Xml", "3"));
        }

        protected void ExibirBeneficiariosInclusos(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("Apenas o titular", "0"));
            combo.Items.Add(new ListItem("Todos", "1"));
            combo.Items.Add(new ListItem("Específicos", "2"));
            combo.Items.Add(new ListItem("Apenas Inativos", "3"));
            combo.Items.Add(new ListItem("Apenas Ativos", "4"));
            combo.Items.Add(new ListItem("Pendentes", "5"));
        }

        protected void ExibirSecoes(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("Header", "0"));
            combo.Items.Add(new ListItem("Header Detail", "1"));
            combo.Items.Add(new ListItem("Detail", "3"));
            combo.Items.Add(new ListItem("Trailler", "2"));
        }

        protected void ExibirTiposValor(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("Alfanumérico", "0"));
            combo.Items.Add(new ListItem("Numérico", "1"));
        }

        protected void ExibirTiposDado(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("Normal", "0"));
            combo.Items.Add(new ListItem("Monetário", "1"));
            combo.Items.Add(new ListItem("Data", "2"));
        }

        protected void ExibirFormatos(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("", ""));
            combo.Items.Add(new ListItem("DD/MM/yyyy", "DD/MM/yyyy"));
            combo.Items.Add(new ListItem("DDMMyyyy", "DDMMyyyy"));
            combo.Items.Add(new ListItem("DD/MM/yy", "DD/MM/yy"));
            combo.Items.Add(new ListItem("DDMMyy", "DDMMyy"));

            combo.Items.Add(new ListItem("yyyy/MM/DD", "yyyy/MM/DD"));
            combo.Items.Add(new ListItem("yyyyMMDD", "yyyyMMDD"));
            combo.Items.Add(new ListItem("yy/MM/DD", "yy/MM/DD"));
            combo.Items.Add(new ListItem("yyMMDD", "yyMMDD"));

            combo.Items.Add(new ListItem("MM/yyyy/DD", "MM/yyyy/DD"));
            combo.Items.Add(new ListItem("MMyyyyDD", "MMyyyyDD"));
            combo.Items.Add(new ListItem("MM/yy/DD", "MM/yy/DD"));
            combo.Items.Add(new ListItem("MMyyDD", "MMyyDD"));
        }

        protected void ExibirBehaviors(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("Fonte de dados", "0"));
            combo.Items.Add(new ListItem("Literal", "1"));
            combo.Items.Add(new ListItem("Quebra de linha", "2"));
            combo.Items.Add(new ListItem("Contador da linha", "3"));
            combo.Items.Add(new ListItem("Data corrente", "4"));
        }

        protected void ExibirTiposPreenchimento(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("À direita", "0"));
            combo.Items.Add(new ListItem("À esquerda", "1"));
        }

        protected void ExibirCampos(DropDownList combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new ListItem("Beneficiário ID", "beneficiario_id"));
            combo.Items.Add(new ListItem("Beneficiário Nome", "beneficiario_nome"));
            combo.Items.Add(new ListItem("Beneficiário Estado Civil Código", "estadocivil_codigo"));
            combo.Items.Add(new ListItem("Beneficiário Estado Civil Descrição", "estadocivil_descricao"));
            combo.Items.Add(new ListItem("Beneficiário Sexo", "beneficiario_sexo"));
            combo.Items.Add(new ListItem("Beneficiário CPF", "beneficiario_cpf"));
            combo.Items.Add(new ListItem("Beneficiário Código Carência", "contratobeneficiario_carenciaCodigo"));
            combo.Items.Add(new ListItem("Beneficiário RG", "beneficiario_rg"));
            combo.Items.Add(new ListItem("Beneficiário Data Inclusão", "beneficiario_data"));
            combo.Items.Add(new ListItem("Beneficiário Tipo", "contratobeneficiario_tipo"));
            combo.Items.Add(new ListItem("Beneficiário Dt. Nasc.", "beneficiario_dataNascimento"));
            combo.Items.Add(new ListItem("Beneficiário Dt. Casam.", "contratobeneficiario_dataCasamento"));
            combo.Items.Add(new ListItem("Beneficiário Fone", "beneficiario_telefone"));
            combo.Items.Add(new ListItem("Beneficiário Ramal", "beneficiario_ramal"));
            combo.Items.Add(new ListItem("Beneficiário Fone 2", "beneficiario_telefone2"));
            combo.Items.Add(new ListItem("Beneficiário Ramal 2", "beneficiario_ramal2"));
            combo.Items.Add(new ListItem("Beneficiário Celular", "beneficiario_celular"));
            combo.Items.Add(new ListItem("Beneficiário Celular Operadora", "beneficiario_celularOperadora"));
            combo.Items.Add(new ListItem("Beneficiário E-mail", "beneficiario_email"));
            combo.Items.Add(new ListItem("Beneficiário Nome Mãe", "beneficiario_nomeMae"));
            combo.Items.Add(new ListItem("Beneficiário Altura", "contratobeneficiario_altura"));
            combo.Items.Add(new ListItem("Beneficiário Peso", "contratobeneficiario_peso"));
            combo.Items.Add(new ListItem("Beneficiário Grau Parentesco Código", "contratoAdmparentescoagregado_parentescoCodigo"));
            combo.Items.Add(new ListItem("Beneficiário Grau Parentesco Descrição", "contratoAdmparentescoagregado_parentescoDescricao"));
            combo.Items.Add(new ListItem("Beneficiário Núm. Sequencial", "contratobeneficiario_numeroSequencia"));

            combo.Items.Add(new ListItem("Adicional código", "adicional_codTitular")); //campo fórmula
            combo.Items.Add(new ListItem("Adicional descrição", "adicional_descricao"));

            combo.Items.Add(new ListItem("Contrato Tipo", "tipocontrato_descricao"));
            combo.Items.Add(new ListItem("Contrato Nome do Corretor Principal", "usuario_nome"));
            combo.Items.Add(new ListItem("Contrato Documento do Corretor Principal", "usuario_documento1"));
            combo.Items.Add(new ListItem("Contrato Nome do Corretor", "contrato_corretorTerceiroNome"));
            combo.Items.Add(new ListItem("Contrato Documento do Corretor", "contrato_corretorTerceiroCPF"));

            combo.Items.Add(new ListItem("Contrato End. Cobrança Tipo Logradouro", "TipoEndCobr_Lograd"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança Logradouro", "EndCobr_Lograd"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança Numero", "EndCobr_Numero"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança Compl", "EndCobr_Compl"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança Bairro", "EndCobr_Bairro"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança Cidade", "EndCobr_Cidade"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança UF", "EndCobr_UF"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança CEP", "EndCobr_CEP"));

            combo.Items.Add(new ListItem("Contrato End. Atendimento Tipo Logradouro", "TipoEndRef_Lograd"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento Logradouro", "EndRef_Lograd"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento Numero", "EndRef_Numero"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento Compl", "EndRef_Compl"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento Bairro", "EndRef_Bairro"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento Cidade", "EndRef_Cidade"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento UF", "EndRef_UF"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento CEP", "EndRef_CEP"));

            combo.Items.Add(new ListItem("Titular Contrato End. Atendimento Tipo Logradouro", "TipoEndRef_Lograd_tit"));
            combo.Items.Add(new ListItem("Titular Contrato End. Atendimento Logradouro", "EndRef_Lograd_tit"));
            combo.Items.Add(new ListItem("Titular Contrato End. Atendimento Numero", "EndRef_Numero_tit"));
            combo.Items.Add(new ListItem("Titular Contrato End. Atendimento Compl", "EndRef_Compl_tit"));
            combo.Items.Add(new ListItem("Titular Contrato End. Atendimento Bairro", "EndRef_Bairro_tit"));
            combo.Items.Add(new ListItem("Titular Contrato End. Atendimento Cidade", "EndRef_Cidade_tit"));
            combo.Items.Add(new ListItem("Titular Contrato End. Atendimento UF", "EndRef_UF_tit"));
            combo.Items.Add(new ListItem("Titular Contrato End. Atendimento CEP", "EndRef_CEP_tit"));

            combo.Items.Add(new ListItem("Contrato Estipulante", "estipulante_descricao"));
            combo.Items.Add(new ListItem("Contrato Operadora CNPJ", "operadora_cnpj"));
            combo.Items.Add(new ListItem("Contrato Número", "contrato_numero"));
            combo.Items.Add(new ListItem("Contrato Matrícula", "contrato_numeroMatricula"));

            combo.Items.Add(new ListItem("Contrato Dt. Vigência", "contrato_vigencia"));
            combo.Items.Add(new ListItem("Contrato Dt. Vencimento", "contrato_vencimento"));

            combo.Items.Add(new ListItem("Contrato Dt. Admissão", "contrato_admissao"));
            combo.Items.Add(new ListItem("Contrato Cancelado", "contrato_cancelado")); // NAO FOI PARA XML
            combo.Items.Add(new ListItem("Contrato Inativo", "contrato_inativo")); // NAO FOI PARA XML
            combo.Items.Add(new ListItem("Contrato Dt. Cancelamento", "contrato_dataCancelamento")); // NAO FOI PARA XML
            combo.Items.Add(new ListItem("Contrato Código de Cobrança", "contrato_codcobranca")); // NAO FOI PARA XML
            combo.Items.Add(new ListItem("Contrato Cobra Taxa Associiativa", "contrato_cobrarTaxaAssociativa")); // NAO FOI PARA XML
            combo.Items.Add(new ListItem("Contrato Observação", "contrato_obs")); //campo fórmula  // NAO FOI PARA XML

            combo.Items.Add(new ListItem("Contrato ADM Descr.", "contratoadm_descricao"));
            combo.Items.Add(new ListItem("Contrato ADM Código", "contratoadm_numero"));

            combo.Items.Add(new ListItem("Plano Nome", "plano_descricao"));
            combo.Items.Add(new ListItem("Plano Código", "plano_codigo")); //campo fórmula
            combo.Items.Add(new ListItem("Plano SubPlano", "plano_subplano")); //campo fórmula
            combo.Items.Add(new ListItem("Plano Código Particular", "plano_codigoParticular"));
            combo.Items.Add(new ListItem("Plano SubPlano Particular", "plano_subplanoParticular"));

            //FÓRMULAS
            combo.Items.Add(new ListItem("FORM.: Titular=0; Dependente=X", "titx_dep0"));
            combo.Items.Add(new ListItem("FORM.: Titular=0; Dependente=1", "tit0_dep1"));
            combo.Items.Add(new ListItem("FORM.: Titular=1; Dependente=2", "tit1_dep2"));
            combo.Items.Add(new ListItem("FORM.: Titular=01; Dependente=00", "tit00_dep01"));
            combo.Items.Add(new ListItem("FORM.: Beneficiario CPF (do titular se dependente nao possuir)", "beneficiario_cpf_tit"));

            combo.Items.Add(new ListItem("FORM.: Beneficiario DDD Fone 1", "beneficiario_telefoneDDD"));
            combo.Items.Add(new ListItem("FORM.: Beneficiario Fone 1", "beneficiario_telefoneNumero"));
        }

        protected void ExibirCamposRelatorioGeral(DropDownList combo)
        {
            combo.Items.Clear();

            combo.Items.Add(new ListItem("Adicional código", "adicional_codTitular"));
            combo.Items.Add(new ListItem("Adicional descrição", "adicional_descricao"));

            combo.Items.Add(new ListItem("Beneficiário ID", "beneficiario_id"));
            combo.Items.Add(new ListItem("Beneficiário Nome", "beneficiario_nome"));
            combo.Items.Add(new ListItem("Beneficiário Estado Civil Código", "estadocivil_codigo"));
            combo.Items.Add(new ListItem("Beneficiário Estado Civil Descrição", "estadocivil_descricao"));
            combo.Items.Add(new ListItem("Beneficiário Sexo", "beneficiario_sexo"));
            combo.Items.Add(new ListItem("Beneficiário CPF", "beneficiario_cpf"));
            combo.Items.Add(new ListItem("Beneficiário Código Carência", "contratobeneficiario_carenciaCodigo"));
            combo.Items.Add(new ListItem("Beneficiário RG", "beneficiario_rg"));
            combo.Items.Add(new ListItem("Beneficiário Data Inclusão", "beneficiario_data"));
            combo.Items.Add(new ListItem("Beneficiário Tipo", "contratobeneficiario_tipo"));
            combo.Items.Add(new ListItem("Beneficiário Dt. Nasc.", "beneficiario_dataNascimento"));
            combo.Items.Add(new ListItem("Beneficiário Dt. Casam.", "contratobeneficiario_dataCasamento"));
            combo.Items.Add(new ListItem("Beneficiário Fone", "beneficiario_telefone"));
            combo.Items.Add(new ListItem("Beneficiário Ramal", "beneficiario_ramal"));
            combo.Items.Add(new ListItem("Beneficiário Fone 2", "beneficiario_telefone2"));
            combo.Items.Add(new ListItem("Beneficiário Ramal 2", "beneficiario_ramal2"));
            combo.Items.Add(new ListItem("Beneficiário Celular", "beneficiario_celular"));
            combo.Items.Add(new ListItem("Beneficiário Celular Operadora", "beneficiario_celularOperadora"));
            combo.Items.Add(new ListItem("Beneficiário E-mail", "beneficiario_email"));
            combo.Items.Add(new ListItem("Beneficiário Nome Mãe", "beneficiario_nomeMae"));
            combo.Items.Add(new ListItem("Beneficiário Altura", "contratobeneficiario_altura"));
            combo.Items.Add(new ListItem("Beneficiário Peso", "contratobeneficiario_peso"));
            combo.Items.Add(new ListItem("Beneficiário Grau Parentesco Código", "contratoAdmparentescoagregado_parentescoCodigo"));
            combo.Items.Add(new ListItem("Beneficiário Grau Parentesco Descrição", "contratoAdmparentescoagregado_parentescoDescricao"));
            combo.Items.Add(new ListItem("Beneficiário Núm. Sequencial", "contratobeneficiario_numeroSequencia"));


            combo.Items.Add(new ListItem("Cobrança Parcela", "cobranca_parcela"));
            combo.Items.Add(new ListItem("Cobrança Valor", "cobranca_valor"));
            combo.Items.Add(new ListItem("Cobrança Vencimento", "cobranca_dataVencimento"));
            combo.Items.Add(new ListItem("Cobrança Paga", "cobranca_pago"));
            combo.Items.Add(new ListItem("Cobrança Valor Pagto", "cobranca_valorPagto"));
            combo.Items.Add(new ListItem("Cobrança Comissão Paga", "cobranca_comissaoPaga"));
            combo.Items.Add(new ListItem("Cobrança Cancelada", "cobranca_cancelada"));


            combo.Items.Add(new ListItem("Contrato Tipo", "tipocontrato_descricao"));
            combo.Items.Add(new ListItem("Contrato Nome do Corretor Principal", "usuario_nome"));
            combo.Items.Add(new ListItem("Contrato Documento do Corretor Principal", "usuario_documento1"));
            combo.Items.Add(new ListItem("Contrato Nome do Corretor", "contrato_corretorTerceiroNome"));
            combo.Items.Add(new ListItem("Contrato Documento do Corretor", "contrato_corretorTerceiroCPF"));

            //combo.Items.Add(new ListItem("Contrato End. Cobrança Tipo Logradouro", "endCobr.TipoEndCobr_Lograd"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança Logradouro", "endCobr.endereco_logradouro"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança Numero", "endCobr.endereco_numero"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança Compl", "endCobr.endereco_complemento"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança Bairro", "endCobr.endereco_bairro"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança Cidade", "endCobr.endereco_cidade"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança UF", "endCobr.endereco_uf"));
            combo.Items.Add(new ListItem("Contrato End. Cobrança CEP", "endCobr.endereco_cep"));

            //combo.Items.Add(new ListItem("Contrato End. Atendimento Tipo Logradouro", "TipoEndRef_Lograd"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento Logradouro", "endRef.endereco_logradouro"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento Numero", "endRef.endereco_numero"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento Compl", "endRef.endereco_complemento"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento Bairro", "endRef.endereco_bairro"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento Cidade", "endRef.endereco_cidade"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento UF", "endRef.endereco_uf"));
            combo.Items.Add(new ListItem("Contrato End. Atendimento CEP", "endRef.endereco_cep"));

            //combo.Items.Add(new ListItem("Titular Contrato End. Atendimento Tipo Logradouro", "TipoEndRef_Lograd_tit"));
            //combo.Items.Add(new ListItem("Titular Contrato End. Atendimento Logradouro", "EndRef_Lograd_tit"));
            //combo.Items.Add(new ListItem("Titular Contrato End. Atendimento Numero", "EndRef_Numero_tit"));
            //combo.Items.Add(new ListItem("Titular Contrato End. Atendimento Compl", "EndRef_Compl_tit"));
            //combo.Items.Add(new ListItem("Titular Contrato End. Atendimento Bairro", "EndRef_Bairro_tit"));
            //combo.Items.Add(new ListItem("Titular Contrato End. Atendimento Cidade", "EndRef_Cidade_tit"));
            //combo.Items.Add(new ListItem("Titular Contrato End. Atendimento UF", "EndRef_UF_tit"));
            //combo.Items.Add(new ListItem("Titular Contrato End. Atendimento CEP", "EndRef_CEP_tit"));

            combo.Items.Add(new ListItem("Contrato Estipulante", "estipulante_descricao"));
            combo.Items.Add(new ListItem("Contrato Operadora CNPJ", "operadora_cnpj"));
            combo.Items.Add(new ListItem("Contrato Número", "contrato_numero"));
            combo.Items.Add(new ListItem("Contrato Matrícula", "contrato_numeroMatricula"));

            combo.Items.Add(new ListItem("Contrato Dt. Vigência", "contrato_vigencia"));
            combo.Items.Add(new ListItem("Contrato Dt. Vencimento", "contrato_vencimento"));

            combo.Items.Add(new ListItem("Contrato Dt. Admissão", "contrato_admissao"));
            combo.Items.Add(new ListItem("Contrato Cancelado", "contrato_cancelado")); // NAO FOI PARA XML
            combo.Items.Add(new ListItem("Contrato Inativo", "contrato_inativo")); // NAO FOI PARA XML
            combo.Items.Add(new ListItem("Contrato Dt. Cancelamento", "contrato_dataCancelamento")); // NAO FOI PARA XML
            combo.Items.Add(new ListItem("Contrato Código de Cobrança", "contrato_codcobranca")); // NAO FOI PARA XML
            combo.Items.Add(new ListItem("Contrato Cobra Taxa Associiativa", "contrato_cobrarTaxaAssociativa")); // NAO FOI PARA XML
            combo.Items.Add(new ListItem("Contrato Observação", "contrato_obs")); //campo fórmula  // NAO FOI PARA XML

            combo.Items.Add(new ListItem("Contrato ADM Descr.", "contratoadm_descricao"));
            combo.Items.Add(new ListItem("Contrato ADM Código", "contratoadm_numero"));

            combo.Items.Add(new ListItem("Plano Nome", "plano_descricao"));
            combo.Items.Add(new ListItem("Plano Código", "plano_codigo")); //campo fórmula
            combo.Items.Add(new ListItem("Plano SubPlano", "plano_subplano")); //campo fórmula
            combo.Items.Add(new ListItem("Plano Código Particular", "plano_codigoParticular"));
            combo.Items.Add(new ListItem("Plano SubPlano Particular", "plano_subplanoParticular"));
        }

        #endregion

        protected String ReplaceStart(String text, String searchFor, String replaceWith)
        {
            if (String.IsNullOrEmpty(text)) { return String.Empty; }
            if (!text.StartsWith(searchFor)) { return text; }

            String aux = "";

            Boolean finished = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (text.Substring(i, 1) == searchFor && !finished)
                    aux += replaceWith;
                else
                {
                    finished = true;
                    aux += text.Substring(i, 1);
                }
            }

            return aux;
        }
    }
}
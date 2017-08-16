namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using LC.Web.PadraoSeguros.Entity;

    public class UserControlBase : UserControl
    {
        public UserControlBase() { }

        protected Boolean useExternalCEPEngine()
        {
            String use = ConfigurationManager.AppSettings["useExternalCEPEngine"];

            return use != null && use.ToUpper() == "Y";
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

        protected String[] PegaEndereco(Page page, TextBox txtCEP, TextBox txtRua, TextBox txtBairro, TextBox txtCidade, TextBox txtUF, TextBox txtNumero)
        {
            txtRua.Text = "";
            txtBairro.Text = "";
            txtCidade.Text = "";
            txtUF.Text = "";

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
                Alerta(null, page, "_errCep0", "CEP não encontrado.\\nVerfique os dados informados e tente novamente.");
                txtCEP.Focus();
                return null;
            }

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);

            System.Xml.XmlNode root = doc.DocumentElement;
            System.Xml.XmlNode current = root.SelectSingleNode("/webservicecep/resultado");

            if (current.InnerText == "0") { Alerta(null, page, "_errCep1", "CEP não encontrado.\\nVerfique os dados informados e tente novamente."); txtCEP.Focus(); return null; }

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
                retorno = Convert.ToString(Convert.ToInt32(tsDuration.Days / 365));

            return retorno;
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

            DateTime data = new DateTime(Int32.Parse(arr[2]), Int32.Parse(arr[1]), Int32.Parse(arr[0]));
            return data;
        }

        protected String CToString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return String.Empty;
            else
                return Convert.ToString(param);
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

        protected const String IDKey = "_idkey";
        protected const String IDKey2 = "_idkey2";
        protected const String IDKey3 = "_idkey3";

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

        protected void PreencheComboNumerico(DropDownList combo, int inicio, int fim, Boolean limpaCombo)
        {
            if (limpaCombo) { combo.Items.Clear(); }

            for (int i = inicio; i <= fim; i++)
            {
                combo.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }

        //protected void ExibirEstadosCivis(DropDownList combo, Boolean itemSELECIONE)
        //{
        //    combo.DataValueField = "ID";
        //    combo.DataTextField = "Descricao";
        //    combo.DataSource = EstadoCivil.CarregarTodos();
        //    combo.DataBind();

        //    if (itemSELECIONE)
        //    {
        //        combo.Items.Insert(0, new ListItem("Selecione", "-1"));
        //    }
        //}

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
            combo.DataValueField = "ID";
            combo.DataTextField = "Nome";
            combo.DataSource = Operadora.CarregarTodas();
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirPlanos(DropDownList combo, Object contratoID, Boolean itemSELECIONE, Boolean apenasAtivos)
        {
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
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = Estipulante.Carregar(apenasAtivos);
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
            cboCategoriaComissionamento.DataValueField = "ID";
            cboCategoriaComissionamento.DataTextField = "Descricao";
            cboCategoriaComissionamento.DataSource = Categoria.Carregar(apenasAtivos);
            cboCategoriaComissionamento.DataBind();

            if (itemSelecione)
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
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = AlmoxTipoProduto.CarregarTodos();
            combo.DataBind();

            if (itemSELECIONE) { combo.Items.Insert(0, new ListItem(textoSELECIONE, "-1")); }
        }

        protected void ExibirProdutos(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = AlmoxProduto.CarregarTodos(true);
            combo.DataBind();

            if (itemSELECIONE) { combo.Items.Insert(0, new ListItem("Selecione", "-1")); }
        }
    }
}
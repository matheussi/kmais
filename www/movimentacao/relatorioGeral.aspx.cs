namespace www.movimentacao
{
    using System;
    using System.IO;
    using System.Web;
    using System.Linq;
    using System.Web.UI;
    using System.Collections;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using System.Data;
    using LC.Framework.Phantom;
    using System.Data.SqlClient;
    using LC.Web.PadraoSeguros.Entity;

    public partial class relatorioGeral : PageBase
    {
        Hashtable CamposSelecionados
        {
            get
            {
                if (ViewState["_cps"] == null)
                {
                    Hashtable list = new Hashtable();
                    ViewState["_cps"] = list;
                    return (Hashtable)ViewState["_cps"];
                }
                else
                    return (Hashtable)ViewState["_cps"];
            }
            set
            {
                ViewState["_cps"] = value;
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                //Response.Redirect("~/default.aspx");
                base.ExibirCamposRelatorioGeral(cboCampos);
                this.CarregaFiliais();
                this.CarregaOperadoras();
                this.CarregaEstipulantes();
                this.LeCookies();
            }
        }

        void LeCookies()
        {
            if (Request.Cookies["estipulante"] != null)
            {
                String[] estip = Request.Cookies["estipulante"].Value.Split(',');
                foreach (String e in estip)
                {
                    foreach (ListItem item in lstEstipulantes.Items)
                    {
                        if (item.Value == e) { item.Selected = true; continue; }
                    }
                }
            }

            if (Request.Cookies["operadora"] != null)
            {
                String[] oper = Request.Cookies["operadora"].Value.Split(',');
                foreach (String o in oper)
                {
                    foreach (ListItem item in lstOperadora.Items)
                    {
                        if (item.Value == o) { item.Selected = true; continue; }
                    }
                }
            }

            if (Request.Cookies["filial"] != null)
                lstFilial.SelectedValue = Request.Cookies["filial"].Value;

            //////////////////////

            if (Request.Cookies["pagtoDe"] != null)
                txtDePagto.Text = Request.Cookies["pagtoDe"].Value;

            if (Request.Cookies["pagtoAte"] != null)
                txtAtePagto.Text = Request.Cookies["pagtoAte"].Value;

            ////////////////////

            if (Request.Cookies["venctoDe"] != null)
                txtDeVencto.Text = Request.Cookies["venctoDe"].Value;

            if (Request.Cookies["venctoAte"] != null)
                txtAteVencto.Text = Request.Cookies["venctoAte"].Value;

            ////////////////////

            if (Request.Cookies["vigDe"] != null)
                txtDe.Text = Request.Cookies["vigDe"].Value;

            if (Request.Cookies["vigAte"] != null)
                txtAte.Text = Request.Cookies["vigAte"].Value;

            if (Request.Cookies["campos"] != null)
            {
                Hashtable ht = new Hashtable();
                String[] conteudoCookie = Request.Cookies["campos"].Value.Split('|');

                String campo, descricao = "";
                foreach (String conteudo in conteudoCookie)
                {
                    campo = conteudo;
                    foreach (ListItem item in cboCampos.Items)
                    {
                        if (item.Value == campo)
                        {
                            descricao = item.Text;
                            break;
                        }
                    }

                    ht.Add(campo, descricao);
                }

                this.CamposSelecionados = ht;
                this.carregaCamposSelecionados();
            }
        }

        void CarregaFiliais()
        {
            IList<Filial> filiais = Filial.CarregarTodas(false);

            this.lstFilial.DataValueField = "ID";
            this.lstFilial.DataTextField = "Nome";
            this.lstFilial.DataSource = filiais;
            this.lstFilial.DataBind();

            this.lstFilial.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        void CarregaEstipulantes()
        {
            IList<Estipulante> estipulantes = Estipulante.Carregar(false);

            this.lstEstipulantes.DataValueField = "ID";
            this.lstEstipulantes.DataTextField = "Descricao";
            this.lstEstipulantes.DataSource = estipulantes;
            this.lstEstipulantes.DataBind();
        }
        void CarregaOperadoras()
        {
            IList<Operadora> operadoras = Operadora.CarregarTodas(false);

            this.lstOperadora.DataValueField = "ID";
            this.lstOperadora.DataTextField = "Nome";
            this.lstOperadora.DataSource = operadoras;
            this.lstOperadora.DataBind();
        }

        Boolean temCobranca()
        {
            if (this.CamposSelecionados == null) { return false; }
            foreach (DictionaryEntry entry in this.CamposSelecionados)
            {
                if (Convert.ToString(entry.Key).IndexOf("cobranca_") > -1) { return true; }
            }

            return false;
        }
        String getCampos()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (DictionaryEntry entry in this.CamposSelecionados)
            {
                if (sb.Length > 0) { sb.Append(","); }
                sb.Append(entry.Key);
            }

            return sb.ToString();
        }

        void carregar()
        {
            if (this.CamposSelecionados.Count == 0)
            {
                base.Alerta(null, this, "errParam", "Não há campos selecionados para criar o relatório.");
                return;
            }

            this.escreveCamposEmCookie();

            DateTime dtFrom = new DateTime();
            DateTime dtTo = new DateTime();
            UIHelper.TyParseToDateTime(txtDe.Text, out dtFrom);
            UIHelper.TyParseToDateTime(txtAte.Text, out dtTo);

            String[] estp = base.PegaIDsSelecionados(lstEstipulantes);
            String[] oper = base.PegaIDsSelecionados(lstOperadora);

            if (oper == null || estp == null || dtFrom == DateTime.MinValue || dtTo == DateTime.MinValue) 
            {
                base.Alerta(null, this, "errParam", "Você deve informar a operadora, o estipulante e o intervalo da vigência."); 
                return;
            }

            Response.Cookies.Add(new HttpCookie("vigDe", txtDe.Text));
            Response.Cookies.Add(new HttpCookie("vigAte", txtAte.Text));

            Response.Cookies.Add(new HttpCookie("estipulante", String.Join(",", estp)));
            Response.Cookies.Add(new HttpCookie("operadora",   String.Join(",", oper)));

            String ativoCond = "", taxaCond = "", cobrancasJoin = "", filialCond = "", pagtoCond = "", venctoCond = "";

            DateTime test = DateTime.MinValue;
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            if (DateTime.TryParse(txtDePagto.Text, cinfo, System.Globalization.DateTimeStyles.None, out test) &&
                DateTime.TryParse(txtAtePagto.Text, cinfo, System.Globalization.DateTimeStyles.None, out test))
            {
                DateTime pagtoDe = base.CStringToDateTime(txtDePagto.Text);
                DateTime pagtoAte = base.CStringToDateTime(txtAtePagto.Text);
                pagtoCond = String.Concat(" AND cobranca_dataPagto BETWEEN '", pagtoDe.ToString("yyyy-MM-dd 00:00:00.000"), "' AND '", pagtoAte.ToString("yyyy-MM-dd 23:59:59.700"), "' ");

                Response.Cookies.Add(new HttpCookie("pagtoDe", txtDePagto.Text));
                Response.Cookies.Add(new HttpCookie("pagtoAte", txtAtePagto.Text));
            }
            else
            {
                Response.Cookies.Add(new HttpCookie("pagtoDe", ""));
                Response.Cookies.Add(new HttpCookie("pagtoAte", ""));
            }

            if (DateTime.TryParse(txtDeVencto.Text, cinfo, System.Globalization.DateTimeStyles.None, out test) &&
                DateTime.TryParse(txtAteVencto.Text, cinfo, System.Globalization.DateTimeStyles.None, out test))
            {
                DateTime venctoDe = base.CStringToDateTime(txtDeVencto.Text);
                DateTime venctoAte = base.CStringToDateTime(txtAteVencto.Text);
                venctoCond = String.Concat(" AND cobranca_dataVencimento BETWEEN '", venctoDe.ToString("yyyy-MM-dd 00:00:00.000"), "' AND '", venctoAte.ToString("yyyy-MM-dd 23:59:59.700"), "' ");
                cobrancasJoin = " INNER JOIN cobranca on cobranca_propostaId=contrato_id ";

                Response.Cookies.Add(new HttpCookie("venctoDe", txtDeVencto.Text));
                Response.Cookies.Add(new HttpCookie("venctoAte", txtAteVencto.Text));
            }
            else
            {
                Response.Cookies.Add(new HttpCookie("venctoDe", ""));
                Response.Cookies.Add(new HttpCookie("venctoAte", ""));
            }

            if (temCobranca()) { cobrancasJoin = " INNER JOIN cobranca on cobranca_propostaId=contrato_id "; }

            if (lstFilial.SelectedIndex > 0)
            {
                filialCond = " and contrato_filialId=" + lstFilial.SelectedValue;
                Response.Cookies.Add(new HttpCookie("filial", lstFilial.SelectedValue));
            }
            else
                Response.Cookies.Add(new HttpCookie("filial", "-1"));

            String _camposDaQuery = this.getCampos();
            String qry = String.Concat("SELECT contrato_id, ", _camposDaQuery,
            "   FROM contrato ",
            "       INNER JOIN estipulante ON estipulante_id = contrato_estipulanteId ",
            "       INNER JOIN tipo_contrato ON contrato_tipoContratoId = tipocontrato_id ",
            "       INNER JOIN usuario ON contrato_donoId=usuario_id ",
            "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 ",
            "       INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId = beneficiario_id ",
            "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
            "       INNER JOIN plano ON plano_id = contrato_planoId ",
            "       INNER JOIN operadora ON operadora_id = contrato_operadoraId ",
            "       LEFT JOIN endereco endCobr ON endCobr.endereco_id=contrato_enderecoCobrancaId ",
            "       LEFT JOIN endereco endRef ON endRef.endereco_id=contrato_enderecoReferenciaId ",
            "       LEFT JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
            "       LEFT JOIN contratoAdm_parentesco_agregado ON contratobeneficiario_parentescoId=contratoAdmparentescoagregado_id ",
            cobrancasJoin,
            "   WHERE ",
            "       contrato_operadoraId   IN (" + String.Join(",", oper) + ") AND ",
            "       contrato_estipulanteId IN (" + String.Join(",", estp) + ") AND ",
            "       contrato_vigencia BETWEEN '", dtFrom.ToString("yyyy-MM-dd 00:00:00.000"), "' AND '", dtTo.ToString("yyyy-MM-dd 23:59:59.700"), "'",
            venctoCond, pagtoCond, ativoCond, taxaCond, filialCond,
            "   ORDER BY contrato_numero,contratoadm_descricao, contratobeneficiario_numeroSequencia");
            if (this.temCobranca()) { qry += ",cobranca_parcela"; }

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                SqlDataAdapter adp = new SqlDataAdapter(qry, conn);
                DataSet ds = new DataSet();
                adp.Fill(ds);

                grid.DataSource = ds.Tables[0];
                grid.DataBind();

                Session["dtGeral"] = ds.Tables[0];

                if (ds.Tables[0].Rows.Count > 0)
                    pnlResultado.Visible = true;
                else
                    pnlResultado.Visible = false;
            }
        }

        void escreveCamposEmCookie()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Hashtable ht = this.CamposSelecionados;
            foreach (DictionaryEntry entry in ht)
            {
                if (sb.Length > 0) { sb.Append("|"); }
                sb.Append(entry.Key);
            }

            Response.Cookies.Add(new HttpCookie("campos", sb.ToString()));
        }

        protected void cmdToExcel_Click(Object sender, EventArgs e)
        {
            DataTable dtOrigem = (DataTable)Session["dtGeral"];

            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.Charset = "";

            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment;filename=\"file.xls\"");

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    DataGrid dg = new DataGrid();
                    //dg.AutoGenerateColumns = false;
                    dg.DataSource = dtOrigem;
                    dg.DataBind();
                    dg.RenderControl(htw);
                    response.Write(sw.ToString());
                    response.End();
                }
            }
        }

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            this.carregar();
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            grid.DataSource = (DataTable)Session["dtGeral"];
            grid.DataBind();
        }

        ////////////////////////////////////////////////////////////////

        protected void gridCampos_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            //UIHelper.AuthCtrl(e.Row.Cells[6], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            //base.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente remover o dependente deste contrato?");

            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    e.Row.Cells[4].Text = base.
            //        TraduzTipoRelacaoDependenteContrato(Convert.ToInt32(e.Row.Cells[4].Text));

            //    Object id = gridDependentes.DataKeys[e.Row.RowIndex][1];
            //    e.Row.Cells[7].Attributes.Add("onClick", "win = window.open('beneficiarioP.aspx?et=2&" + IDKey + "=" + id + "', 'janela', 'toolbar=no,scrollbars=1,width=860,height=420'); win.moveTo(100,150); return false;");
            //}
        }

        protected void gridCampos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                Object campo = gridCampos.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                this.CamposSelecionados.Remove(Convert.ToString(campo));
                this.carregaCamposSelecionados();
            }
        }

        void carregaCamposSelecionados()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("campo");
            dt.Columns.Add("descricao");
            Hashtable ht = this.CamposSelecionados;

            DataRow row;
            foreach (DictionaryEntry entry in ht)
            {
                row = dt.NewRow();
                row["campo"] = entry.Key;
                row["descricao"] = entry.Value;
                dt.Rows.Add(row);
            }

            gridCampos.DataSource = dt;
            gridCampos.DataBind();
        }

        protected void gridCampos_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.carregaCamposSelecionados();
        }

        protected void cmdAddCampo_Click(Object sender, EventArgs e)
        {
            Hashtable list = this.CamposSelecionados;

            foreach (DictionaryEntry campo in list)
            {
                if (Convert.ToString(campo.Key) == cboCampos.SelectedValue)
                {
                    base.Alerta(null, this, "_errAddField", "O campo selecionado ja foi adicionado à coleção.");
                    return;
                }
            }

            this.CamposSelecionados.Add(cboCampos.SelectedValue, cboCampos.SelectedItem.Text);
            this.carregaCamposSelecionados();
        }
    }
}
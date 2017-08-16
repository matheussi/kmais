namespace www.financeiro
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using System.Data.SqlClient;
    using LC.Web.PadraoSeguros.Entity;

    public partial class cobrancaNaoGeradas : PageBase
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.CarregaEstipulantes();
                this.CarregaOperadoras();

                txtDe.Text = string.Concat("01/", DateTime.Now.ToString("MM"), "/", DateTime.Now.Year);
                txtAte.Text = string.Concat(base.UltimoDiaDoMes(DateTime.Now.Month,DateTime.Now.Year).Day, "/", DateTime.Now.ToString("MM"), "/", DateTime.Now.Year); //DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        void CarregaAgenda()
        {
            //gridAgenda.DataSource = AgendaRelatorio.CarregarPorUsuario(Usuario.Autenticado.ID, AgendaRelatorio.eTipo.CReceberPago);
            //gridAgenda.DataBind();

            //if (gridAgenda.Rows.Count > 0) pnlAgenda.Visible = true;
            //else pnlAgenda.Visible = false;
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

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            #region parametros e validacoes 

            DateTime dtFrom = new DateTime();
            DateTime dtTo = new DateTime();
            UIHelper.TyParseToDateTime(txtDe.Text, out dtFrom);
            UIHelper.TyParseToDateTime(txtAte.Text, out dtTo);

            String[] oper = base.PegaIDsSelecionados(lstOperadora);
            if (oper == null || oper.Length == 0)
            {
                int total = lstOperadora.Items.Count;
                oper = new String[total];
                for (int i = 0; i < total; i++) { oper[i] = lstOperadora.Items[i].Value; }
            }

            String[] estp = base.PegaIDsSelecionados(lstEstipulantes);
            if (estp == null || estp.Length == 0)
            {
                int total = lstEstipulantes.Items.Count;
                estp = new String[total];
                for (int i = 0; i < total; i++) { estp[i] = lstEstipulantes.Items[i].Value; }
            }

            if (dtFrom == DateTime.MinValue || dtTo == DateTime.MinValue)
            { base.Alerta(null, this, "_err", "Os parâmetros de data são requeridos."); return; }

            #endregion

            String qry = AgendaRelatorio.CobrancasNaoGeradasQUERY(dtFrom, dtTo, oper, estp);
            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];

            grid.DataSource = dt;
            grid.DataBind();

            if (dt.Rows.Count == 0)
                pnl.Visible = false;
            else
                pnl.Visible = true;
        }

        protected void cmdToExcel_Click(Object sender, ImageClickEventArgs e)
        {
            String attachment = "attachment; filename=cobrancasNaoGeradas.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        protected void BaixaArquivo_REPORT(String arquivoNome)
        {
            if (!String.IsNullOrEmpty(arquivoNome))
            {
                String ArquivoCaminho = Server.MapPath("/") + ConfigurationManager.AppSettings["reports_file"].Replace("/", @"\");

                String strFilePath = String.Concat(ArquivoCaminho, arquivoNome);

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

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("detalhe"))
            {
                mpeFalhaDetalhe.Show();
                upFalhaDetalhe.Update();

                DateTime dtFrom = new DateTime();
                DateTime dtTo = new DateTime();
                UIHelper.TyParseToDateTime(txtDe.Text, out dtFrom);
                UIHelper.TyParseToDateTime(txtAte.Text, out dtTo);

                if (dtFrom == DateTime.MinValue || dtTo == DateTime.MinValue)
                { base.Alerta(null, this, "_err", "Os parâmetros de data são requeridos."); return; }

                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];

                using(PersistenceManager pm = new PersistenceManager())
                {
                    string qry = string.Concat(
                        "select distinct(cobrancafalha_motivo) from cobranca_falha where ",
                        "cobrancafalha_data >= '", dtFrom.ToString("yyyy-MM-dd"), "'",
                        " and cobrancafalha_data <= '", dtTo.ToString("yyyy-MM-dd 23:59:59.995"), "'",
                        " and cobrancafalha_propostaId=", id);

                    DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result", pm).Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        litDetalheFalha.Text = "";
                        foreach (DataRow row in dt.Rows)
                        {
                            if (!string.IsNullOrEmpty(litDetalheFalha.Text)) litDetalheFalha.Text += "<br/>";

                            litDetalheFalha.Text += Convert.ToString(row[0]);
                        }

                        litDetalheFalha.Text = "<br><b>" + litDetalheFalha.Text + "</b><br>";
                    }
                    else
                        litDetalheFalha.Text = "<br>Nenhum registro encontrado.<br>Confirme por favor se o critério de geração da cobrança faz referência ao contrato administrativo.<br>";
                }

            }
        }
    }
}
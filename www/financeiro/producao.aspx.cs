using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using LC.Framework.Phantom;
using LC.Web.PadraoSeguros.Entity;
using System.Data;
using System.Data.SqlClient;

namespace www.financeiro
{
    public partial class producao : PageBase
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected void Page_Load(Object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TipoContrato.UI.FillComboWithTiposContrato(lstTipoContrato);
                this.CarregaFiliais();
                this.CarregaEstipulantes();
                this.CarregaOperadoras();
            }
        }

        void Escondepanels()
        {
            pnlRanking.Visible = false;
            pnlSintetico.Visible = false;
        }

        void CarregaFiliais()
        {
            IList<Filial> filiais = Filial.CarregarTodas(false);

            this.lstFilial.DataValueField = "ID";
            this.lstFilial.DataTextField = "Nome";
            this.lstFilial.DataSource = filiais;
            this.lstFilial.DataBind();
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
            if (cboTipo.SelectedIndex == 0)
                this.RelatorioRanking();
            else if (cboTipo.SelectedIndex == 1)
                this.RelatorioSintetico();
        }

        protected void cmdRankingToExcel_Click(Object sender, EventArgs e)
        {
            String attachment = "attachment; filename=file.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gridRanking.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        protected void cmdSinteticoToExcel_Click(Object sender, EventArgs e)
        {
            String attachment = "attachment; filename=file.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gridSintetico.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        void RelatorioRanking()
        {
            this.Escondepanels();

            DateTime dtFrom = new DateTime();
            DateTime dtTo = new DateTime();
            UIHelper.TyParseToDateTime(txtDe.Text, out dtFrom);
            UIHelper.TyParseToDateTime(txtAte.Text, out dtTo);

            String[] estp = base.PegaIDsSelecionados(lstEstipulantes);
            String[] oper = base.PegaIDsSelecionados(lstOperadora);
            String[] fili = base.PegaIDsSelecionados(lstFilial);
            String[] tipo = base.PegaIDsSelecionados(lstTipoContrato);

            Int32 vidas = 0, contratos = 0;
            Decimal total = 0;
            DataTable dt = new DataTable();

            if (tipo == null || fili == null || oper == null || estp == null || dtFrom == DateTime.MinValue || dtTo == DateTime.MinValue) { base.Alerta(null, this, "errParam", "Informe todos os parâmetros."); return; }

            String statusCond = "";
            if (cboStatus.SelectedValue == "0")
                statusCond = " AND contrato_inativo <> 1 ";
            else if (cboStatus.SelectedValue == "1")
                statusCond = " AND contrato_inativo = 1 ";

            String diaVigenciaCond = "";
            if (txtDeDia.Text != "" && txtDeDia.Text != "__" && Convert.ToInt32(txtDeDia.Text) > 0 &&
                txtAteDia.Text != "" && txtAteDia.Text != "__" && Convert.ToInt32(txtAteDia.Text) > 0)
            {
                diaVigenciaCond = " AND DAY(contrato_vigencia) BETWEEN " + txtDeDia.Text + " AND " + txtAteDia.Text;
            }

            String qry = "SELECT contrato_id, usuario_id, usuario_nome, usuario_documento1, ";
            qry += " (SELECT COUNT(contratobeneficiario_id) FROM contrato_beneficiario WHERE contratobeneficiario_ativo <> 0 AND contratobeneficiario_contratoId=contrato_id) as Vidas, ";
            qry += " (SELECT SUM(contratobeneficiario_valor) FROM contrato_beneficiario WHERE contratobeneficiario_ativo <> 0 AND contratobeneficiario_contratoId=contrato_id) as Valor  ";
            qry += " FROM usuario INNER JOIN contrato ON contrato_donoId=usuario_id ";
            qry += " WHERE contrato_cancelado <> 1 AND contrato_tipoContratoId IN (" + String.Join(",", tipo) + ") AND usuario_filialId IN (" + String.Join(",", fili) + ") AND contrato_operadoraId IN (" + String.Join(",", oper) + ") AND contrato_estipulanteId IN (" + String.Join(",", estp) + ") AND contrato_vigencia BETWEEN '" + dtFrom.ToString("yyyy-MM-dd 00:00:00.000") + "' AND '" + dtTo.ToString("yyyy-MM-dd 23:59:59.700") + "'" + statusCond + diaVigenciaCond + " ORDER BY usuario_nome, usuario_id";

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                SqlDataAdapter adp = new SqlDataAdapter(qry, conn);
                DataSet ds = new DataSet();
                adp.Fill(ds);

                dt.Columns.Add("contrato_id");
                dt.Columns.Add("usuario_id");
                dt.Columns.Add("usuario_nome");
                dt.Columns.Add("usuario_documento1");
                dt.Columns.Add("Contratos", typeof(int));
                dt.Columns.Add("Vidas", typeof(int));
                dt.Columns.Add("Valor", typeof(decimal));

                String usuarioCorrenteId = null;

                foreach (DataRow _row in ds.Tables[0].Rows)
                {
                    if (CToString(_row["usuario_id"]) == usuarioCorrenteId) { continue; }

                    usuarioCorrenteId = CToString(_row["usuario_id"]);

                    DataRow row = dt.NewRow();
                    row["contrato_id"] = _row["contrato_id"];
                    row["usuario_id"] = _row["usuario_id"];
                    row["usuario_nome"] = _row["usuario_nome"];
                    row["usuario_documento1"] = _row["usuario_documento1"];
                    row["Contratos"] = ds.Tables[0].Compute("COUNT(usuario_id)", "usuario_id=" + usuarioCorrenteId);
                    row["Vidas"] = ds.Tables[0].Compute("SUM(Vidas)", "usuario_id=" + usuarioCorrenteId);
                    row["Valor"] = ds.Tables[0].Compute("SUM(Valor)", "usuario_id=" + usuarioCorrenteId);

                    vidas += Convert.ToInt32(row["Vidas"]);
                    total += Convert.ToDecimal(row["Valor"]);

                    dt.Rows.Add(row);
                }

                contratos = ds.Tables[0].Rows.Count;

                gridRanking.DataSource = dt;
                gridRanking.DataBind();
                pnlRanking.Visible = true;

                litTotalizadorRanking.Text = String.Concat(
                    "<b>Contratos:</b> ", contratos.ToString(), "<br>",
                    "<b>Vidas:</b> ", vidas.ToString(), "<br>",
                    "<b>Valor:</b> ", total.ToString("C"));
            }
        }

        void RelatorioSintetico()
        {
            this.Escondepanels();

            DateTime dtFrom = new DateTime();
            DateTime dtTo = new DateTime();
            UIHelper.TyParseToDateTime(txtDe.Text, out dtFrom);
            UIHelper.TyParseToDateTime(txtAte.Text, out dtTo);

            String[] estp = base.PegaIDsSelecionados(lstEstipulantes);
            String[] oper = base.PegaIDsSelecionados(lstOperadora);
            String[] fili = base.PegaIDsSelecionados(lstFilial);
            String[] tipo = base.PegaIDsSelecionados(lstTipoContrato);

            if (tipo == null || fili == null || oper == null || estp == null || dtFrom == DateTime.MinValue || dtTo == DateTime.MinValue) { base.Alerta(null, this, "errParam", "Informe todos os parâmetros."); return; }

            String statusCond = "";
            if (cboStatus.SelectedValue == "0")
                statusCond = " AND contrato_inativo <> 1 ";
            else if (cboStatus.SelectedValue == "1")
                statusCond = " AND contrato_inativo = 1 ";

            String diaVigenciaCond = "";
            if (txtDeDia.Text != "" && txtDeDia.Text != "__" && Convert.ToInt32(txtDeDia.Text) > 0 &&
                txtAteDia.Text != "" && txtAteDia.Text != "__" && Convert.ToInt32(txtAteDia.Text) > 0)
            {
                diaVigenciaCond = " AND DAY(contrato_vigencia) BETWEEN " + txtDeDia.Text + " AND " + txtAteDia.Text;
            }

            String qry = "select operadora_id, estipulante_id, operadora_nome, estipulante_descricao, COUNT(c1.contrato_operadoraId) as Contratos, ";
            qry += " (SELECT COUNT(contratobeneficiario_id)  FROM contrato_beneficiario WHERE contratobeneficiario_ativo <> 0 and contratobeneficiario_contratoId IN (SELECT c2.contrato_id FROM contrato c2 WHERE c2.contrato_cancelado <> 1 AND c2.contrato_tipoContratoId IN (" + String.Join(",", tipo) + ") AND c2.contrato_operadoraId=c1.contrato_operadoraId AND c2.contrato_estipulanteId=c1.contrato_estipulanteId AND c2.contrato_vigencia BETWEEN '" + dtFrom.ToString("yyyy-MM-dd 00:00:00.000") + "' AND '" + dtTo.ToString("yyyy-MM-dd 23:59:59.700") + "'" + statusCond + diaVigenciaCond + ")) as Vidas, ";
            qry += " (SELECT SUM(contratobeneficiario_valor) FROM contrato_beneficiario WHERE contratobeneficiario_ativo <> 0 and contratobeneficiario_contratoId IN (SELECT c2.contrato_id FROM contrato c2 WHERE c2.contrato_cancelado <> 1 AND c2.contrato_tipoContratoId IN (" + String.Join(",", tipo) + ") AND c2.contrato_operadoraId=c1.contrato_operadoraId AND c2.contrato_estipulanteId=c1.contrato_estipulanteId AND c2.contrato_vigencia BETWEEN '" + dtFrom.ToString("yyyy-MM-dd 00:00:00.000") + "' AND '" + dtTo.ToString("yyyy-MM-dd 23:59:59.700") + "'" + statusCond + diaVigenciaCond + ")) as Valor ";
            qry += " FROM contrato c1 inner join operadora on operadora_id=c1.contrato_operadoraId inner join estipulante on estipulante_id=c1.contrato_estipulanteId ";
            qry += " WHERE c1.contrato_cancelado <> 1 AND c1.contrato_tipoContratoId IN (" + String.Join(",", tipo) + ") AND c1.contrato_donoId in (SELECT usuario_id FROM usuario WHERE usuario_filialId IN (" + String.Join(",", fili) + ")) AND c1.contrato_operadoraId IN (" + String.Join(",", oper) + ") AND c1.contrato_estipulanteId IN (" + String.Join(",", estp) + ") AND c1.contrato_vigencia BETWEEN '" + dtFrom.ToString("yyyy-MM-dd 00:00:00.000") + "' AND '" + dtTo.ToString("yyyy-MM-dd 23:59:59.700") + "'" + statusCond + diaVigenciaCond;
            qry += " GROUP BY operadora_id, estipulante_id, operadora_nome, estipulante_descricao, c1.contrato_operadoraId, contrato_estipulanteId order by Vidas DESC";

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                SqlDataAdapter adp = new SqlDataAdapter(qry, conn);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                adp.Dispose();

                gridSintetico.DataSource = ds.Tables[0];
                gridSintetico.DataBind();
                pnlSintetico.Visible = true;

                Int32 vidas   = 0;
                Decimal total = 0;

                if (ds.Tables[0].Rows.Count > 0)
                {
                    vidas = Convert.ToInt32(ds.Tables[0].Compute("Sum(Vidas)", "Vidas <> -1"));
                    total = Convert.ToDecimal(ds.Tables[0].Compute("Sum(Valor)", "Valor <> -1"));
                }

                litTotalizadorSintetico.Text = String.Concat(
                    "<b>Vidas:</b> ", vidas.ToString(), "<br>",
                    "<b>Valor:</b> ", total.ToString("C"));
            }
        }

    }
}

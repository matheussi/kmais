namespace www.movimentacao
{
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

    public partial class relatorioEntidades : PageBase
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.SetUpComboTaxas();
                this.SetUpComboAtivas();
                this.CarregaOperadoras();
                this.CarregaEstipulantes();
            }
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
        void SetUpComboAtivas()
        {
            cboAtivas.Items.Add(new ListItem("Todas as propostas", "0"));
            cboAtivas.Items.Add(new ListItem("Somente propostas ativas", "1"));
        }
        void SetUpComboTaxas()
        {
            cboTaxas.Items.Add(new ListItem("Todas as propostas", "0"));
            cboTaxas.Items.Add(new ListItem("Somente propostas com taxa", "1"));
        }

        protected void cmdToExcel_Click(Object sender, EventArgs e)
        {
            String attachment = "attachment; filename=file.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            DateTime dtFrom = new DateTime();
            DateTime dtTo = new DateTime();
            UIHelper.TyParseToDateTime(txtDe.Text, out dtFrom);
            UIHelper.TyParseToDateTime(txtAte.Text, out dtTo);

            String[] estp = base.PegaIDsSelecionados(lstEstipulantes);
            String[] oper = base.PegaIDsSelecionados(lstOperadora);

            if (oper == null || estp == null || dtFrom == DateTime.MinValue || dtTo == DateTime.MinValue) { base.Alerta(null, this, "errParam", "Informe todos os parâmetros."); return; }

            String ativoCond = "", taxaCond = "", somentePagasJoin = "", somentePagasCampos = "";

            if(cboAtivas.SelectedIndex == 1)
            {
                ativoCond = " AND contrato_cancelado <> 1 AND contrato_inativo <> 1 ";
            }

            if(cboTaxas.SelectedIndex == 1)
            {
                taxaCond = " AND contrato_cobrarTaxaAssociativa=1 ";
            }

            if (chkSomentePagas.Checked)
            {
                somentePagasCampos = " ,cobranca_valor, cobranca_valorPagto ";
                somentePagasJoin = " INNER JOIN cobranca on cobranca_propostaId=contrato_id and cobranca_tipo=0 and cobranca_parcela=2 AND cobranca_pago=1 ";

                grid.Columns[7].Visible = true;
                grid.Columns[8].Visible = true;
            }
            else
            {
                somentePagasCampos = " ,cobranca_valor=0, cobranca_valorPagto=0 ";
                grid.Columns[7].Visible = false;
                grid.Columns[8].Visible = false;
            }

            String qry = String.Concat("SELECT operadora_nome, contrato_numero, contratoadm_descricao, contratoadm_contratoSaude, beneficiario_nome, '''' + beneficiario_cpf as beneficiario_cpf, contrato_vigencia,(select count(*) from contrato_beneficiario where contratobeneficiario_contratoId=contrato_id and contratobeneficiario_ativo=1) as qtdvidas ", somentePagasCampos,
            "   FROM contrato ",
            "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId ",
            "       INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId = beneficiario_id ",
            "       INNER JOIN contratoadm ON contratoadm_id = contrato_contratoAdmId ",
            "       INNER JOIN operadora ON contrato_operadoraId = operadora_id ",
            somentePagasJoin,
            "   WHERE ",
            "       contrato_operadoraId   IN (" + String.Join(",", oper) + ") AND ",
            "       contrato_estipulanteId IN (" + String.Join(",", estp) + ") AND ",
            "       contrato_vigencia BETWEEN '" + dtFrom.ToString("yyyy-MM-dd 00:00:00.000") + "' AND '" + dtTo.ToString("yyyy-MM-dd 23:59:59.700") + "'",
            ativoCond, taxaCond,
            "   ORDER BY contrato_numero,contratoadm_descricao, contratobeneficiario_numeroSequencia");

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                SqlDataAdapter adp = new SqlDataAdapter(qry, conn);
                DataSet ds = new DataSet();
                adp.Fill(ds);

                grid.DataSource = ds.Tables[0];
                grid.DataBind();

                if (ds.Tables[0].Rows.Count > 0)
                    pnlResultado.Visible = true;
                else
                    pnlResultado.Visible = false;
            }
        }
    }
}
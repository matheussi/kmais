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
    public partial class etiqueta : PageBase
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected void Page_Load(Object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.CarregaFiliais();
                this.CarregaEstipulantes();
                this.CarregaOperadoras();
            }
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
            String[] estp = base.PegaIDsSelecionados(lstEstipulantes);
            String[] oper = base.PegaIDsSelecionados(lstOperadora);
            String[] fili = base.PegaIDsSelecionados(lstFilial);

            DateTime dtFrom = new DateTime();
            DateTime dtTo = new DateTime();
            UIHelper.TyParseToDateTime(txtDe.Text, out dtFrom);
            UIHelper.TyParseToDateTime(txtAte.Text, out dtTo);

            if (fili == null || oper == null || estp == null || dtFrom == DateTime.MinValue || dtTo == DateTime.MinValue) { base.Alerta(null, this, "errParam", "Informe todos os parâmetros."); return; }

            String qry = String.Concat(
                "select contrato_numero, beneficiario_nome, endereco_logradouro, endereco_numero, endereco_complemento, endereco_bairro, endereco_cidade, endereco_uf, endereco_cep ",
                "	from beneficiario ",
                "		inner join contrato_beneficiario on contratobeneficiario_beneficiarioId=beneficiario_id and contratobeneficiario_tipo=0 ",
                "		inner join contrato on contratobeneficiario_contratoId = contrato_id ",
                "		inner join endereco on contrato_enderecoCobrancaId=endereco_id ",
                "		inner join usuario on usuario_id=contrato_donoId ",
                "	where contrato_cancelado <> 1 AND contrato_inativo <> 1 AND ",
                "		usuario_filialId in (", String.Join(",", fili), ") AND ",
                "       contrato_operadoraId in (", String.Join(",", oper), ") AND ",
                "       contrato_estipulanteId in (", String.Join(",", estp), ") AND contrato_vigencia BETWEEN '" + dtFrom.ToString("yyyy-MM-dd 00:00:00.000") + "' AND '" + dtTo.ToString("yyyy-MM-dd 23:59:59.700") + "'",
                "	order by beneficiario_nome, contrato_numero ");

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                SqlDataAdapter adp = new SqlDataAdapter(qry, conn);
                DataSet ds = new DataSet();
                adp.Fill(ds);

                grid.DataSource = ds.Tables[0];
                grid.DataBind();
                pnl.Visible = true;
            }
        }
    }
}
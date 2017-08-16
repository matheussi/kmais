namespace www.reports
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

    public partial class vendas : PageBase
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaEstipulantes();
                this.CarregaOperadoras();
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

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            //DateTime dtFrom = new DateTime();
            //DateTime dtTo   = new DateTime();
            //UIHelper.TyParseToDateTime(txtDe.Text, out dtFrom);
            //UIHelper.TyParseToDateTime(txtAte.Text, out dtTo);

            DateTime dtVigFrom = new DateTime();
            DateTime dtVigTo   = new DateTime();
            UIHelper.TyParseToDateTime(txtVigDe.Text, out dtVigFrom);
            UIHelper.TyParseToDateTime(txtVigAte.Text, out dtVigTo);

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

            //if (estp == null || oper == null || dtVigFrom == DateTime.MinValue || dtVigTo == DateTime.MinValue)
            if (dtVigFrom == DateTime.MinValue || dtVigTo == DateTime.MinValue)
            { base.Alerta(null, this, "_err", "Os parâmetros de data são requeridos."); return; }

            String tipoCond = " cobranca_parcela=1 ";
            if (cboTipoCobrancas.SelectedValue == "2") { tipoCond = " cobranca_parcela=2 "; }

            String qry = String.Concat(
                "select filial.filial_nome as FilialContrato, contrato_id,operadora_nome,estipulante_descricao,CONVERT(varchar(14), contrato_vigencia, 103) as contrato_vigencia,CONVERT(varchar(14), cobranca_dataVencimento, 103) as cobranca_dataVencimento,CONVERT(varchar(14), cobranca_dataPagto, 103) as cobranca_dataPagto,contrato_corretorTerceiroNome,contrato_corretorTerceiroCPF,contrato.contrato_superiorTerceiroNome,contrato.contrato_superiorTerceiroCPF,prod.usuario_nome,prod.usuario_codigo,prod.usuario_documento1,beneficiario_nome,beneficiario_cpf,contrato_numero, REPLACE(REPLACE(CONVERT(varchar(100), cobranca_valor), ',',''), '.',',') as cobranca_valor,REPLACE(REPLACE(CONVERT(varchar(100), cobranca_valorPagto), ',',''), '.',',') as cobranca_valorPagto,cobranca_parcela as Parcela,(SELECT COUNT(*) from contrato_beneficiario where contratobeneficiario_contratoId=contrato_id and contratobeneficiario_ativo=1) as qtdvidas,operador.usuario_nome as operadorNome,contrato_data,adicional_descricao,contrato_datacancelamento, ",
                "   case ",
                "       WHEN  operadora_id IN (3,4,5,6,8,9,16,22) THEN 'SP' ",
                "       WHEN  operadora_id IN (10,11,12,13,14,15,17,21) THEN 'RJ' ",
                "       WHEN  operadora_id = 18 THEN 'DF' ",
                "       WHEN  operadora_id = 19 THEN 'PR' ",
                "       WHEN  operadora_id IN (20,23) THEN 'CE' ",
                "       WHEN  operadora_id = 24 THEN 'RN' ",
                "       WHEN  operadora_id = 25 THEN 'PE' ",
                "       WHEN  operadora_id = 26 THEN 'MG' ",
                "       WHEN  operadora_id = 27 THEN 'GO' ",
                "   end as filial ",
                "	from contrato ",
                "		inner join contrato_beneficiario on contratobeneficiario_contratoId=contrato_id and contratobeneficiario_tipo=0 ",
                "		left  join adicional_beneficiario on contratobeneficiario_contratoId=adicionalbeneficiario_propostaId and adicionalbeneficiario_beneficiarioid=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 ",
                "       left join adicional on adicional_id = adicionalbeneficiario_adicionalid ",
                "		inner join beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 ",
                "		inner join operadora on operadora_id = contrato_operadoraId ",
                "		inner join estipulante on estipulante_id = contrato_estipulanteId ",
                "       inner join usuario prod on prod.usuario_id=contrato_donoId ",
                "       left join usuario operador on operador.usuario_id=contrato_usuarioId ",
                "       left  join filial on filial.filial_id = contrato.contrato_filialId ",
                "       inner join contratoadm on contratoadm_id=contrato_contratoadmid ",
                "       inner join cobranca on cobranca_propostaId=contrato_id ",
                "	where ", tipoCond,
                "		and cobranca_tipo=0 and (contrato_datacancelamento > cobranca_dataVencimento or contrato_datacancelamento is null) ", //and cobranca_cancelada <> 1 and contrato_cancelado <> 1 and contrato_inativo <> 1
//              "		and cobranca_dataVencimento between '", dtFrom.ToString("yyyy-MM-dd 00:00:00.000"), "' and '", dtTo.ToString("yyyy-MM-dd 23:59:59.998"), "' ",
                "		and contrato_vigencia between '", dtVigFrom.ToString("yyyy-MM-dd 00:00:00.000"), "' and '", dtVigTo.ToString("yyyy-MM-dd 23:59:59.998"), "' ",
                "       and contrato_operadoraId IN (", String.Join(",", oper), ")",
                "       and contrato_estipulanteId IN (", String.Join(",", estp), ")", /*filialCond,cobrancaCond,*/
                "	order by operadora_nome,contrato_numero,contrato_vigencia,cobranca_parcela");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];

            #region datatable final 

            DataTable final = new DataTable();
            final.Columns.Add("Filial");
            final.Columns.Add("FilialContrato");
            final.Columns.Add("operadora_nome");
            final.Columns.Add("estipulante_descricao");
            final.Columns.Add("contrato_vigencia");
            final.Columns.Add("cobranca_dataVencimento");
            final.Columns.Add("cobranca_dataPagto");
            final.Columns.Add("contrato_corretorTerceiroNome");
            final.Columns.Add("contrato_corretorTerceiroCPF");
            final.Columns.Add("contrato_superiorTerceiroNome");
            final.Columns.Add("contrato_superiorTerceiroCPF");
            final.Columns.Add("usuario_nome");
            final.Columns.Add("usuario_codigo");
            final.Columns.Add("usuario_documento1");
            final.Columns.Add("beneficiario_nome");
            final.Columns.Add("beneficiario_cpf");
            final.Columns.Add("contrato_numero");
            final.Columns.Add("cobranca_valor");
            final.Columns.Add("cobranca_valorPagto");
            final.Columns.Add("Parcela");
            final.Columns.Add("qtdvidas");
            final.Columns.Add("operadorNome");
            final.Columns.Add("contrato_data");
            final.Columns.Add("contrato_dataCancelamento");
            final.Columns.Add("aditivos");

            String aditivos = "";
            DataRow[] rows = null;
            List<String> contratoIDs = new List<String>();
            foreach (DataRow row in dt.Rows)
            {
                if (contratoIDs.Contains(Convert.ToString(row["contrato_id"]))) { continue; }
                contratoIDs.Add(Convert.ToString(row["contrato_id"]));

                aditivos = "";
                DataRow nova = final.NewRow();

                nova["Filial"] = row["Filial"];
                nova["operadora_nome"] = row["operadora_nome"];
                nova["FilialContrato"] = row["FilialContrato"];
                nova["estipulante_descricao"] = row["estipulante_descricao"];
                nova["contrato_vigencia"] = row["contrato_vigencia"];
                nova["cobranca_dataVencimento"] = row["cobranca_dataVencimento"];
                nova["cobranca_dataPagto"] = row["cobranca_dataPagto"];
                nova["contrato_corretorTerceiroNome"] = row["contrato_corretorTerceiroNome"];
                nova["contrato_corretorTerceiroCPF"] = row["contrato_corretorTerceiroCPF"];
                nova["contrato_superiorTerceiroNome"] = row["contrato_superiorTerceiroNome"];
                nova["contrato_superiorTerceiroCPF"] = row["contrato_superiorTerceiroCPF"];
                nova["usuario_nome"] = row["usuario_nome"];
                nova["usuario_codigo"] = row["usuario_codigo"];
                nova["usuario_documento1"] = row["usuario_documento1"];
                nova["beneficiario_nome"] = row["beneficiario_nome"];
                nova["beneficiario_cpf"] = row["beneficiario_cpf"];
                nova["contrato_numero"] = row["contrato_numero"];
                nova["cobranca_valor"] = row["cobranca_valor"];
                nova["cobranca_valorPagto"] = row["cobranca_valorPagto"];
                nova["Parcela"] = row["Parcela"];
                nova["qtdvidas"] = row["qtdvidas"];
                nova["operadorNome"] = row["operadorNome"];
                nova["contrato_data"] = Convert.ToDateTime(row["contrato_data"]).ToString("dd/MM/yyyy HH:mm");

                if (row["contrato_dataCancelamento"] != null && row["contrato_dataCancelamento"] != DBNull.Value)
                    nova["contrato_dataCancelamento"] = Convert.ToDateTime(row["contrato_dataCancelamento"]).ToString("dd/MM/yyyy");
                else
                    nova["contrato_dataCancelamento"] = "";

                rows = dt.Select("contrato_id=" + Convert.ToString(row["contrato_id"]));
                foreach (DataRow subrow in rows)
                {
                    if (aditivos.Length == 0)
                        aditivos = Convert.ToString(subrow["adicional_descricao"]);
                    else
                        aditivos = String.Concat(aditivos, ", ", subrow["adicional_descricao"]);
                }

                nova["aditivos"] = aditivos;
                final.Rows.Add(nova);
                rows = null;
            }

            #endregion

            dt.Dispose();
            contratoIDs.Clear();
            contratoIDs = null;
            grid.DataSource = final;
            grid.DataBind();

            if (dt.Rows.Count == 0)
                pnl.Visible = false;
            else
                pnl.Visible = true;
        }

        protected void cmdToExcel_Click(Object sender, ImageClickEventArgs e)
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
    }
}

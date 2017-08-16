namespace www.reports
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using System.Data.SqlClient;
    using LC.Web.PadraoSeguros.Entity;

    public partial class fichaFinanceira : PageBase
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaOperadoras();
            }
        }

        void CarregaOperadoras()
        {
            base.ExibirOperadoras(lstOperadora, true, false);
        }

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            String operCond = "", numCond = "", codCobrCond = "", benefCond = "";

            if (lstOperadora.SelectedIndex > 0)
                operCond = " and contrato_operadoraId=" + lstOperadora.SelectedValue;

            if (txtNumProposta.Text.Trim() != "")
                numCond = " and contrato_numero=@contrato_numero ";

            if (txtIDCobranca.Text.Trim() != "")
                codCobrCond = " and contrato_codcobranca=@contrato_codcobranca ";

            if (txtBeneificarioNome.Text.Trim() != "")
                benefCond = " and beneficiario_nome like @beneficiario_nome ";

            String qry = String.Concat(
                "select CONVERT(varchar(7), cobranca_dataVencimento, 111) as CompeVencto,  operadora_nome as Operadora, estipulante_descricao as Estipulante,contrato_numero as NumeroContrato, contratobeneficiario_numeroSequencia as Seq,beneficiario_nome as NomeBeneficiario,beneficiario_cpf as CPFbeneficiario, ",
                "   case ",
                "       WHEN  operadora_id IN (3,4,5,6,8,9,16,22) THEN 'SP' ",
                "       WHEN  operadora_id IN (10,11,12,13,14,15,17,21) THEN 'RJ' ",
                "       WHEN  operadora_id = 18 THEN 'DF' ",
                "       WHEN  operadora_id = 19 THEN 'PR' ",
                "       WHEN  operadora_id IN (20,23) THEN 'CE' ",
                "       WHEN  operadora_id = 24 THEN 'RN' ",
                "       WHEN  operadora_id = 25 THEN 'PE' ",
                "   end as filial, ",
                "   case ",
                "       WHEN  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  < 19) THEN '0  a 18 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 18) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  24) THEN '19 a 23 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 23) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  29) THEN '24 a 28 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 28) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  34) THEN '29 a 33 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 33) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  39) THEN '34 a 38 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 38) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  44) THEN '39 a 43 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 43) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  49) THEN '44 a 48 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 48) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  54) THEN '49 a 53 anos' ",
                "       WHEN ((datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento) / 365  > 53) AND  (datediff(dd,beneficiario_dataNascimento,cobranca_dataVencimento)) / 365  <  59) THEN '54 a 58 anos' ",
                "       ELSE '59 anos ou mais' ",
                "   end as FaixaEtaria, ",

                "   datediff(dd,beneficiario_dataNascimento,cobranca.cobranca_dataVencimento) / 365 as IDADE, ",
                "   CONVERT(varchar(14), beneficiario.beneficiario_dataNascimento, 103) as DataNascimento, ",
                "   contratoAdmparentescoagregado_parentescoDescricao as Parentesco,",
                "   CONVERT(varchar(14), contrato_vigencia, 103) as DataVigência, ",
                "   CONVERT(varchar(14), contrato_datacancelamento, 103) as DataCancelInativo, ",
                "   CONVERT(varchar(14), cobranca_dataVencimento, 103) as DataVencto,",
                "   CONVERT(varchar(14), cobranca_dataPagto, 103) as DataPagto, ",

                "   case ",
                "       when contratobeneficiario_numeroSequencia = 0  THEN REPLACE(REPLACE(CONVERT(varchar(100), cobranca_valor), ',',''), '.',',')",
                "   else NULL",
                "   end as Valor_Boleto,",


                "   case ",
                "       WHEN contratobeneficiario_numeroSequencia = 0 and cobranca_composicao.cobrancacomp_tipo = 0 THEN REPLACE(REPLACE(CONVERT(varchar(100), cobranca_valorPagto), ',',''), '.',',') ",
                "       ELSE NULL ",
                "   end as Valor_Pago, ",

                "   case ",
                "       WHEN cobranca.cobranca_tipo = 0 then 'NORMAL' ",
                "       WHEN cobranca.cobranca_tipo = 1 then 'COMPLEMENTAR' ",
                "       WHEN cobranca.cobranca_tipo = 2 then 'DUPLA' ",
                "       ELSE 'NAO IDENTIFICADO' ",
                "   end as Tipo_Boleto, ",

                "   case ",
                "       WHEN cobranca_composicao.cobrancacomp_tipo = 1 then 'TAXA ASSOCIATIVA' ",
                "       WHEN cobranca_composicao.cobrancacomp_tipo = 3 then 'ADICONAL' ",
                "       WHEN cobranca_composicao.cobrancacomp_tipo = 0 then 'VALOR MEDICO' ",
                "       WHEN cobranca_composicao.cobrancacomp_tipo = 4 then 'DESCONTO' ",
                "       WHEN cobranca_composicao.cobrancacomp_tipo = 2 then 'TARIFA BANCARIA' ",
                "       ELSE 'NAO IDENTIFICADO' ",
                "   end as TIPO, ",

                "   REPLACE(REPLACE(CONVERT(varchar(100), cobranca_composicao.cobrancacomp_valor), ',',''), '.',',') as Valor_Benef ",

                "	from contrato ",
                "		inner join contrato_beneficiario on contratobeneficiario_contratoId=contrato_id ",
                "		inner join beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId  ",
                "		inner join operadora on operadora_id = contrato_operadoraId ",
                "		inner join estipulante on estipulante_id = contrato_estipulanteId ",
                "       inner join usuario on usuario_id=contrato_donoId ",
                "       inner join contratoadm on contratoadm_id=contrato_contratoadmid ",
                "       inner join cobranca on cobranca_propostaId=contrato_id ",
                "       left  join dbo.cobranca_composicao on cobranca.cobranca_id = cobranca_composicao.cobrancacomp_cobranaId and contrato_beneficiario.contratobeneficiario_beneficiarioId = cobranca_composicao.cobrancacomp_beneficiarioId ",
                "       left  join contratoADM_parentesco_agregado on contratobeneficiario_parentescoId = contratoAdmparentescoagregado_Id ",
                "	where ",
                "		cobranca_parcela > 1 and cobranca_cancelada <> 1 ",
                operCond, numCond, codCobrCond, benefCond,
                "	order by 5,6");

            String[] paramNm = new String[] { "@contrato_numero", "@contrato_codcobranca", "@beneficiario_nome" };
            String[] paramVl = new String[] { txtNumProposta.Text, txtIDCobranca.Text, "%" + txtBeneificarioNome.Text + "%" };

            DataTable dt = LocatorHelper.Instance.ExecuteParametrizedQuery(qry, paramNm, paramVl).Tables[0];

            grid.DataSource = dt;
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

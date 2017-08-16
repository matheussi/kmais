namespace www.movimentacao
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;
    using System.Configuration;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;

    public partial class arquivotransacionalAMIL : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaOperadoras();
                this.CarregaContratosADM();
                btnGerarArquivo.Attributes.Add("style", "display:none");
            }
        }

        void CarregaOperadoras()
        {
            IList<Operadora> operadoras = LocatorHelper.Instance.ExecuteQuery<Operadora>("select operadora_id,operadora_nome from operadora where operadora_id in (16,17,18,19,23,24,25,26,27) order by operadora_nome", typeof(Operadora));
            cboOperadora.Items.Clear();
            cboOperadora.DataValueField = "ID";
            cboOperadora.DataTextField = "Nome";
            cboOperadora.DataSource = operadoras;
            cboOperadora.DataBind();
        }

        void CarregaContratosADM()
        {
            lstContratosAdm.Items.Clear();

            lstContratosAdm.DataValueField = "ID";
            lstContratosAdm.DataTextField = "Descricao";
            lstContratosAdm.DataSource = ContratoADM.Carregar(cboOperadora.SelectedValue);
            lstContratosAdm.DataBind();
        }

        void CarregaContratos()
        {
            DateTime vigencia = DateTime.MinValue;
            String[] ids = null;

            if (txtNumContrato.Text.Trim() == "")
            {
                ids = base.PegaIDsSelecionados(lstContratosAdm);
                if (ids == null || ids.Length == 0)
                { base.Alerta(null, this, "err", "Selecione ao menos um contrato administrativo"); return; }

                vigencia = base.CStringToDateTime(txtVigenciaGerar.Text);
                if (vigencia == DateTime.MinValue)
                { base.Alerta(null, this, "err", "A data fornecida é inválida."); txtVigenciaGerar.Focus(); return; }
            }

            #region qry

            String qry = String.Concat("select contrato_id, contrato_numero, contrato_vigencia, contratoadm_descricao,beneficiario_nome ",
                "from contrato ",
                "   inner join contratoadm on contratoadm_id=contrato_contratoAdmid ",
                "   inner join contrato_beneficiario on contratobeneficiario_contratoId=contrato_id and contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 ",
                "   inner join beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId  and contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 ",
                "where contrato_inativo <> 1 and contrato_cancelado <> 1 ");

            if (txtNumContrato.Text.Trim() != "")
            {
                qry += " and contrato_numero='" + txtNumContrato.Text + "'";
            }
            else
            {
                qry += String.Concat(" and contrato_vigencia='", vigencia.ToString("yyyy-MM-dd"),
                    "' and contrato_contratoadmId in (", String.Join(",", ids), ")");
            }

            qry += " order by contratoadm_descricao,beneficiario_nome ";

            #endregion

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];

            grid.DataSource = dt;
            grid.DataBind();

            if (dt.Rows.Count == 0)
            {
                btnGerarArquivo.Attributes.Add("style", "display:none");
                litAviso.Text = "Nenhuma proposta encontrada";
            }
            else
            {
                litAviso.Text = "";
                btnGerarArquivo.Attributes.Add("style", "display:inline");
            }
        }

        protected void chkHr_changed(Object sender, EventArgs e)
        {
            Boolean currval = ((CheckBox)sender).Checked;
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                ((CheckBox)grid.Rows[i].Cells[0].Controls[1]).Checked = currval;
            }
        }

        protected void cboOperadora_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratosADM();
        }

        protected void btnVisualizar_OnClick(Object sender, EventArgs e)
        {
            this.CarregaContratos();
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.CarregaContratos();
        }
        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {

        }
        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {

        }

        protected void btnGerarArquivo_OnClick(Object sender, EventArgs e)
        {
            List<String> ids = new List<String>();
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (((CheckBox)grid.Rows[i].Cells[0].Controls[1]).Checked)
                {
                    ids.Add(Convert.ToString(grid.DataKeys[i][0]));
                }
            }

            if (ids.Count == 0)
            {
                base.Alerta(null, this, "err", "Você deve selecionar ao menos uma proposta.");
                return;
            }


            String arquivoNome = "";
            ArqTransacionalAmil arqTrans = new ArqTransacionalAmil(Server.MapPath("/"));
            arqTrans.GerarArquivoDeInclusaoPorContratoIDs(ref arquivoNome, ids.ToArray());

            if (!String.IsNullOrEmpty(arquivoNome))
            {
                this.BaixarArquivo(String.Concat(ArqTransacionalFilePath, arquivoNome), arquivoNome);
            }
        }
    }
}
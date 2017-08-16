namespace www.relatorios
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

    public partial class repTotalPorAdicinal : PageBase
    {
        DataTable dados
        {
            get
            {
                return Session["_dados"] as DataTable;
            }
            set
            {
                Session["_dados"] = value;
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregarContratosAdm();

                if (Request["b"] != null && Request["b"] == "1")
                {
                    trFiltroTaxas.Visible = false;
                    cboOrgaos.Items.RemoveAt(0);
                    litTitulo.Text = "Relatório de beneficiários por órgão";
                }
            }
        }

        void carregarContratosAdm()
        {
            IList <ContratoADM> contratos = ContratoADM.CarregarTodos();

            cboOrgaos.Items.Clear();
            cboOrgaos.Items.Add(new ListItem("Todos", "-1"));

            if (contratos != null && contratos.Count > 0)
            {
                foreach (var c in contratos)
                {
                    cboOrgaos.Items.Add(new ListItem(c.Descricao, Convert.ToString(c.ID)));
                }
            }
        }

        protected void cmdEmitir_click(Object sender, EventArgs e)
        {
            if (Request["b"] == null || Request["b"] != "1")
                this.carregarParaValores();
            else
                this.carregarParaOrgaos();
        }

        void carregarParaOrgaos()
        {
            if (cboOrgaos.Items.Count == 0) return;

            litTotais.Text = "<br/>Nenhum resultado";
            grid.PageIndex = 0;

            string qry = string.Concat(
                "select contratoadm_id,contratoadm_descricao,beneficiario_id,contrato_numero,beneficiario_nome, '' as adicional_descricao, '' as tipo, '' as pagto, '' as Total ",
                "   from beneficiario ",
                "       inner join contrato_beneficiario on contratobeneficiario_beneficiarioId=beneficiario_id ",
                "       inner join contrato on contratobeneficiario_contratoId=contrato_id ",
                "       inner join contratoADM on contratoadm_id = contrato_contratoAdmId ",
                "   where contratoadm_id=", cboOrgaos.SelectedValue,
                "   order by contratoadm_descricao, beneficiario_nome ");

            grid.Columns[3].Visible = false;
            grid.Columns[4].Visible = false;
            grid.Columns[5].Visible = false;
            grid.Columns[6].Visible = false;

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "ds").Tables[0];

            grid.DataSource = dt;
            grid.DataBind();
            this.dados = dt;

            if (dt.Rows.Count > 0)
            {
                cmdToExcel.Visible = true;
                litTotais.Text = string.Concat("<br/>Total de beneficiários: ", dt.Rows.Count);
            }
            else
            {
                cmdToExcel.Visible = false;
            }
        }

        void carregarParaValores()
        {
            #region condições

            bool temCondicao = false;
            string where = "", aux = "";

            if (cboOrgaos.SelectedIndex > 0)
            {
                temCondicao = true;
                where = string.Concat(" where contratoadm_id=", cboOrgaos.SelectedValue, " ");
            }

            foreach (ListItem item in cblTiposAdicionais.Items)
            {
                if (item.Selected)
                {
                    if (aux.Length > 0) aux += ",";
                    aux += item.Value;
                }
            }

            if (aux.Length > 0)
            {
                if (temCondicao)
                    where = string.Concat(where, " and adicional_tipo in (", aux, ") ");
                else
                    where = string.Concat(" where adicional_tipo in (", aux, ") ");

                temCondicao = true;

                aux = "";
            }

            foreach (ListItem item in cblFormasPato.Items)
            {
                if (item.Selected)
                {
                    if (aux.Length > 0) aux += ",";
                    aux += item.Value;
                }
            }

            if (aux.Length > 0)
            {
                if (temCondicao)
                    where = string.Concat(where, " and adicionalbeneficiario_formaPagto in (", aux, ") ");
                else
                    where = string.Concat(" where adicionalbeneficiario_formaPagto in (", aux, ") ");
            }

            where += " and ( (adicional_tipo=3) or ((adicional_tipo=0 or adicional_tipo=2) and (adicionalbeneficiario_status = 'A' or adicionalbeneficiario_status01 = 'A')) or (adicional_tipo=1 and (adicionalbeneficiario_stSgCob1 = 'A' or adicionalbeneficiario_stSgCob2 = 'A' or adicionalbeneficiario_stSgCob3 = 'A' or adicionalbeneficiario_stSgCob4 = 'A' or adicionalbeneficiario_stSgCob5 = 'A' or adicionalbeneficiario_stSgCob6 = 'A')) ) ";

            #endregion

            litTotais.Text = "<br/>Nenhum resultado";
            grid.PageIndex = 0;

            string qry = string.Concat(
                "select contrato_id, contrato_numero, beneficiario_nome,adicional_descricao, adicional_tipo, ",
                "tipo = case when adicional_tipo = 0 then 'Taxa' when adicional_tipo=1 then 'Seguro' when adicional_tipo = 2 then 'Previdência' else 'Indefinido' end, ",
                "pagto = case when adicionalbeneficiario_formaPagto = 31 then 'Boleto' when adicionalbeneficiario_formaPagto=9 then 'Crédito' when adicionalbeneficiario_formaPagto = 10 then 'Débito' when adicionalbeneficiario_formaPagto=81 then 'Desc. em conta' when adicionalbeneficiario_formaPagto=11 then 'Desconto em folha' else 'Indefinido' end, ",
                "contratoadm_descricao,adicionalbeneficiario_valor,adicionalbeneficiario_preCob1,adicionalbeneficiario_preCob2,adicionalbeneficiario_preCob3,adicionalbeneficiario_preCob4,adicionalbeneficiario_preCob5,adicionalbeneficiario_preCob6 ",
                "   from beneficiario ",
                "       inner join contrato_beneficiario on contratobeneficiario_beneficiarioId=beneficiario_id ",
                "       inner join contrato on contratobeneficiario_contratoId=contrato_id ",
                "       inner join contratoADM on contratoadm_id = contrato_contratoAdmId ",
                "       inner join adicional_beneficiario on adicionalbeneficiario_propostaId=contrato_id ",
                "       inner join adicional on adicional_id = adicionalbeneficiario_adicionalid ",
                where,
                "   order by contratoadm_descricao, beneficiario_nome ");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "ds").Tables[0];

            dt.Columns.Add("Total");

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            if (dt.Rows.Count > 0)
            {
                cmdToExcel.Visible = true;

                List<int> total = new List<int>();

                foreach (DataRow row in dt.Rows)
                {
                    if (CToInt(row["adicional_tipo"]) == 0 || CToInt(row["adicional_tipo"]) == 2)
                    {
                        row["Total"] = CToDecimal(row["adicionalbeneficiario_valor"], cinfo).ToString("N2");
                    }
                    else
                    {
                        row["Total"] = (
                            CToDecimal(row["adicionalbeneficiario_preCob1"], cinfo) +
                            CToDecimal(row["adicionalbeneficiario_preCob2"], cinfo) +
                            CToDecimal(row["adicionalbeneficiario_preCob3"], cinfo) +
                            CToDecimal(row["adicionalbeneficiario_preCob4"], cinfo) +
                            CToDecimal(row["adicionalbeneficiario_preCob5"], cinfo) +
                            CToDecimal(row["adicionalbeneficiario_preCob6"], cinfo)).ToString("N2");
                    }

                    if (total.Contains(Convert.ToInt32(row["contrato_id"]))) continue;

                    total.Add(Convert.ToInt32(row["contrato_id"]));
                }

                litTotais.Text = string.Concat("<br/>Total de beneficiários: ", total.Count);
                total = null;
            }
            else
                cmdToExcel.Visible = false;

            grid.DataSource = dt;
            grid.DataBind();
            this.dados = dt;
        }

        protected void cmdToExcel_Click(object sender, EventArgs e)
        {
            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.Charset = "";

            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment;filename=\"file.xls\"");

            DataTable final = new DataTable();

            if (Request["b"] == null || Request["b"] != "1")
            {
                final.Columns.Add("Orgao");
                final.Columns.Add("Matr.Funcional");
                final.Columns.Add("Nome");
                final.Columns.Add("Adicional");
                final.Columns.Add("Tipo Adicional");
                final.Columns.Add("Forma Pagto.");
                final.Columns.Add("Valor");

                DataRow nova = null;
                foreach (DataRow row in dados.Rows)
                {
                    nova = final.NewRow();

                    nova["Orgao"] = row["contratoadm_descricao"];
                    nova["Matr.Funcional"] = string.Concat("'", row["contrato_numero"]);
                    nova["Nome"] = row["beneficiario_nome"];
                    nova["Adicional"] = row["adicional_descricao"];
                    nova["Tipo Adicional"] = row["tipo"];
                    nova["Forma Pagto."] = row["pagto"];
                    nova["Valor"] = row["Total"];


                    final.Rows.Add(nova);
                }
            }
            else // relatório de órgãos
            {
                final.Columns.Add("Orgao");
                final.Columns.Add("Matr.Funcional");
                final.Columns.Add("Nome");

                DataRow nova = null;
                foreach (DataRow row in dados.Rows)
                {
                    nova = final.NewRow();

                    nova["Orgao"] = row["contratoadm_descricao"];
                    nova["Matr.Funcional"] = string.Concat("'", row["contrato_numero"]);
                    nova["Nome"] = row["beneficiario_nome"];

                    final.Rows.Add(nova);
                }
            }

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    // instantiate a datagrid
                    DataGrid dg = new DataGrid();
                    //dg.AutoGenerateColumns = false;
                    dg.DataSource = final;
                    dg.DataBind();
                    dg.RenderControl(htw);
                    response.Write(sw.ToString());
                    response.End();
                }
            }
        }

        protected void grid_OnPageChanging(Object sender, GridViewPageEventArgs e)
        {
            if (e != null && e.NewPageIndex > -1)
            {
                this.grid.PageIndex = e.NewPageIndex;
                this.grid.DataSource = this.dados;
                this.grid.DataBind();
            }
        }
    }
}
namespace www.admin
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class conferenciaRel : PageBaseConferencia
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.carregarOperadoras();
                base.ExibirDepartamentos(cboStatus, Usuario.Autenticado.PerfilID);
            }
        }

        void carregarOperadoras()
        {
            base.ExibirOperadoras(cboOperadora);
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            String[] operadoraIds = base.PegaIDsSelecionados(cboOperadora);
            if (operadoraIds == null || operadoraIds.Length == 0)
            {
                base.Alerta(null, this, "_err", "Informe a operadora.");
                return;
            }

            DateTime de = base.CStringToDateTime(txtDe.Text);
            DateTime ate = base.CStringToDateTime(txtAte.Text);

            if (de == DateTime.MinValue || ate == DateTime.MinValue || ate < de)
            {
                base.Alerta(null, this, "_err", "Informe o intervalo de data.");
                return;
            }

            DataTable dt = Conferencia.CarregarPorParametros(operadoraIds, cboStatus.SelectedValue, de, ate);
            grid.DataSource = dt;
            grid.DataBind();

            if (grid.Rows.Count == 0)
            {
                litCabecalho.Text = "";
                cmdToExcel.Visible = false;
                litAviso.Text = "nenhum cadastro encontrado.";
            }
            else
            {
                Object vidas = dt.Compute("SUM(QtdVidas)", "");

                Decimal total = 0;
                foreach (DataRow row in dt.Rows)
                {
                    total += base.CToDecimal(row["ValorFinal"]);
                }
                //Object ret2 = dt.Compute("SUM(ValorFinal)", "");

                cmdToExcel.Visible = true;
                litAviso.Text = "";
                litCabecalho.Text = String.Concat("<b>Propostas: ", 
                    dt.Rows.Count, " - Vidas: ", vidas, " - Valor Total: ", total.ToString("C"),"</b>");
            }
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.cmdLocalizar_Click(null, null);
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    AtendimentoTemp obj = e.Row.DataItem as AtendimentoTemp;
            //    if (obj != null)
            //    {
            //        if (String.IsNullOrEmpty(obj.ResolvidoPor) && obj.DataPrevisao < DateTime.Now)
            //        {
            //            e.Row.ForeColor = System.Drawing.Color.Red;
            //            e.Row.ToolTip = "atrasado";
            //        }
            //    }
            //}
        }

        protected void cmdToExcel_Click(Object sende, EventArgs e)
        {
            String attachment = "attachment;filename=file.xls";
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
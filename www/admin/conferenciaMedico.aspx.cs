namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class conferenciaMedico : PageBaseConferencia
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            optCom.Attributes.Add("onClick", "document.getElementById('" + txtData.ClientID + "').focus();");
            txtData.Attributes.Add("onFocus", "document.getElementById('" + optCom.ClientID + "').checked=true;");
            if (!IsPostBack){}
        }

        void Carrega()
        {
            if (optSem.Checked)
            {
                grid.DataSource = MedicalReport.CarregarParaMedico(null);
            }
            else
            {
                DateTime data = DateTime.MinValue;
                if (!UIHelper.TyParseToDateTime(txtData.Text, out data))
                {
                    base.Alerta(null, this, "_errData", "A data informada é inválida.");
                    txtData.Focus();
                    return;
                }

                grid.DataSource = MedicalReport.CarregarParaMedico(data);
            }

            grid.DataBind();

            if (grid.Rows.Count == 0)
                tblSalvar.Visible = false;
            else
                tblSalvar.Visible = true;
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            grid.PageIndex = 0;
            this.Carrega();
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if(grid.Rows.Count == 0) { return; }
            DateTime data = DateTime.MinValue;
            if (!UIHelper.TyParseToDateTime(txtDataSalvar.Text, out data))
            {
                base.Alerta(null, this, "_errData1", "A data informada é inválida.");
                txtDataSalvar.Focus();
                return;
            }

            ItemDeclaracaoSaudeINSTANCIA item = null;
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                item = new ItemDeclaracaoSaudeINSTANCIA();
                item.ID = grid.DataKeys[i].Value;
                item.Carregar();

                item.CIDInicial = ((TextBox)grid.Rows[i].Cells[7].Controls[1]).Text;
                item.CIDFinal = ((TextBox)grid.Rows[i].Cells[8].Controls[1]).Text;
                item.AprovadoPeloMedico = ((CheckBox)grid.Rows[i].Cells[9].Controls[1]).Checked;
                item.ObsMedico = ((TextBox)grid.Rows[i].Cells[10].Controls[1]).Text;

                //if (item.AprovadoPeloMedico) { item.DataAprovadoPeloMedico = data; }
                //else { item.DataAprovadoPeloMedico = DateTime.MinValue; }
                item.DataAprovadoPeloMedico = data;

                item.Salvar();
            }

            grid.PageIndex = 0;
            this.Carrega();
            base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.Carrega();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
        }
    }
}
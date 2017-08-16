namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class beneficiarios : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                UIHelper.AuthCtrl(cmdNovo, new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.PropostaBeneficiarioIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.SupervidorIDKey });

                //Se beneficiarios legado (SP), permite a busca por matrículas
                if (Request["leg"] == "1")
                {
                    trMatricula.Visible = true;
                }
                else
                {
                    gridBeneficiarios.Columns[4].Visible = false;
                }
            }
        }

        //void CarregaOperadoras()
        //{
        //    cboOperadoras.DataValueField = "ID";
        //    cboOperadoras.DataTextField = "Nome";
        //    cboOperadoras.DataSource = Operadora.CarregarTodas();
        //    cboOperadoras.DataBind();
        //    cboOperadoras.Items.Insert(0, new ListItem("", "-1"));

        //    if(cboOperadoras.Items.Count > 1)
        //        cboOperadoras.SelectedIndex = 1;
        //}

        protected void cmdConsultar_Click(Object sender, EventArgs e)
        {
            string matricula = null;

            if (Request["leg"] == "1") matricula = txtMatricula.Text;

            IList<Beneficiario> lista = null;
            lista = Beneficiario.CarregarPorParametro(txtNome.Text, txtCPF.Text, 
                txtRG.Text, UIHelper.PegaTipoDeBusca(optQualquer, optInicio, optInteiro), matricula); 

            gridBeneficiarios.DataSource = lista;
            gridBeneficiarios.DataBind();
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("beneficiario.aspx");
        }

        protected void gridBeneficiarios_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = gridBeneficiarios.DataKeys[Convert.ToInt32(e.CommandArgument)][0];

                if (string.IsNullOrEmpty(Request["leg"]))
                    Response.Redirect(String.Concat("beneficiario.aspx?", IDKey, "=", Session[IDKey]));
                else
                    Response.Redirect(String.Concat("beneficiario.aspx?leg=1&", IDKey, "=", Session[IDKey]));
            }
            else if (e.CommandName.Equals("contratos"))
            {
                gridContratos.DataSource = Contrato.CarregarPorBeneficiário(gridBeneficiarios.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                gridContratos.DataBind();
                pnlContratos.Visible = gridContratos.Rows.Count > 0;
                lblSuperior.InnerText = "Contratos do beneficiário " + gridBeneficiarios.Rows[Convert.ToInt32(e.CommandArgument)].Cells[0].Text;
                if (gridContratos.Rows.Count == 0)
                {
                    Alerta(null, this.Page, "_nenhum", "Nenhuma proposta para esse beneficiário.");
                }
            }
        }

        protected void gridContratos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];

                if(string.IsNullOrEmpty(Request["leg"]))
                    Response.Redirect(String.Concat("contrato.aspx?", IDKey, "=", Session[IDKey]));
                else
                    Response.Redirect(String.Concat("UBRASP/contrato.aspx?", IDKey, "=", Session[IDKey]));
            }
        }

        protected void gridContratos_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[4].Text == "0")
                    e.Row.Cells[4].Text = "SIM";
                else
                    e.Row.Cells[4].Text = "NÃO";
            }
        }

        protected void gridBeneficiarios_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Object idEnriq = gridBeneficiarios.DataKeys[e.Row.RowIndex][1];

                if (idEnriq != null)
                {
                    e.Row.ForeColor = System.Drawing.Color.Blue;
                    e.Row.ToolTip = "enriquecimento pendente";
                }
            }
        }
    }
}
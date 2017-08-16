namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class contratosadm : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadoras, false);
                base.ExibirEstipulantes(cboEstipulante, false, true);
                this.CarregaContratos();
            }
        }

        void CarregaContratos()
        {
            if (cboEstipulante.Items.Count == 0 || cboOperadoras.Items.Count == 0)
            {
                gridContratos.DataSource = null;
                gridContratos.DataBind();
                return;
            }

            gridContratos.DataSource = ContratoADM.Carregar(
                cboEstipulante.SelectedValue, cboOperadoras.SelectedValue);
            gridContratos.DataBind();
        }

        protected void cboEstipulante_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
        }

        protected void cboOperadoras_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
        }

        protected void gridContratos_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente prosseguir?");
                if (!Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][1]))
                {
                    e.Row.ForeColor = System.Drawing.Color.Red;
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Text = "<img src='../images/unactive.png' alt='inativo' border='0'>";
                }
                else
                {
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Text = "<img src='../images/active.png'  alt='ativo' border='0'>";
                }
            }
        }

        protected void gridContratos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Session[IDKey] = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect("~/contratoadm.aspx");
            }
            else if (e.CommandName.Equals("ativar"))
            {
                ContratoADM contrato = new ContratoADM();
                contrato.ID = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                contrato.Carregar();
                contrato.Ativo = !contrato.Ativo;
                contrato.Salvar();
                this.CarregaContratos();
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("contratoadm.aspx");
        }
    }
}

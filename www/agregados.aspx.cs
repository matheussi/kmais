namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class agregados : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirEstipulantes(cboEstipulante, false, true);
                base.ExibirOperadoras(cboOperadora, false);
                this.CarregaContratos();
                this.CarregaAgregados();

                if (Request[IDKey] != null)
                {
                    cboEstipulante.SelectedValue = Request[IDKey];
                    cboEstipulante_SelectedIndexChanged(null, null);
                }
                if (Request[IDKey2] != null)
                {
                    cboOperadora.SelectedValue = Request[IDKey2];
                    cboOperadora_SelectedIndexChanged(null, null);
                }
                if (Request[IDKey3] != null)
                {
                    cboContrato.SelectedValue = Request[IDKey3];
                    cboContrato_SelectedIndexChanged(null, null);
                }
            }
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
            this.CarregaAgregados();
        }

        protected void cboEstipulante_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
            this.CarregaAgregados();
        }

        protected void cboContrato_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaAgregados();
        }

        void CarregaContratos()
        {
            cboContrato.Items.Clear();

            if (cboOperadora.Items.Count == 0 || cboEstipulante.Items.Count == 0) { return; }
            IList<ContratoADM> contratos = ContratoADM.Carregar(cboEstipulante.SelectedValue, cboOperadora.SelectedValue);
            cboContrato.DataValueField = "ID";
            cboContrato.DataTextField = "Descricao";
            cboContrato.DataSource = contratos;
            cboContrato.DataBind();
        }

        void CarregaAgregados()
        {
            if (cboContrato.Items.Count == 0) { grid.DataSource = null; grid.DataBind(); return; }
            Parentesco.eTipo tipo = Parentesco.eTipo.Agregado;
            if (optDependente.Checked) { tipo = Parentesco.eTipo.Dependente; }
            grid.DataSource = ContratoADMParentescoAgregado.Carregar(cboContrato.SelectedValue, tipo);
            grid.DataBind();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("agregado.aspx?", IDKey, "=", id));
            }
            else if (e.CommandName.Equals("excluir"))
            {
                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                ContratoADMParentescoAgregado parentesco = new ContratoADMParentescoAgregado();
                parentesco.ID = id;
                parentesco.Remover();
                this.CarregaAgregados();
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente excluir o item?");
        }

        protected void opt_CheckedChanged(Object sender, EventArgs e)
        {
            this.CarregaAgregados();
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect(String.Concat("~/agregado.aspx?", IDKey2, "=", cboEstipulante.SelectedValue, "&", IDKey3, "=", cboOperadora.SelectedValue, "&", IDKey4, "=", cboContrato.SelectedValue));
        }
    }
}
namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class planos : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaOperadoras();
                this.CarregaContratos();
                this.CarregaPlanos();
            }
        }

        void CarregaOperadoras()
        {
            cboOperadoras.DataTextField = "Nome";
            cboOperadoras.DataValueField = "ID";
            cboOperadoras.DataSource = Operadora.CarregarTodas();
            cboOperadoras.DataBind();
        }

        void CarregaContratos()
        {
            if (cboOperadoras.Items.Count == 0)
            {
                cboContrato.Items.Clear();
                return;
            }

            cboContrato.DataValueField = "ID";
            cboContrato.DataTextField = "Descricao";
            IList<ContratoADM> lista = ContratoADM.Carregar(cboOperadoras.SelectedValue);
            cboContrato.DataSource = lista;
            cboContrato.DataBind();
            if (lista == null) { cboContrato.Items.Clear(); gridPlanos.DataSource = null; gridPlanos.DataBind(); }
        }

        void CarregaPlanos()
        {
            if (cboContrato.Items.Count == 0) { gridPlanos.DataSource = null; gridPlanos.DataBind(); return; }

            gridPlanos.DataSource = Plano.CarregarPorContratoID(cboContrato.SelectedValue);
            gridPlanos.DataBind();
        }

        protected void gridPlanos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session[IDKey] = gridPlanos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("plano.aspx?", IDKey, "=", Session[IDKey]));
            }
            else if (e.CommandName == "inativar")
            {
                //LinkButton btn = (LinkButton)gridPlanos.Rows[Convert.ToInt32(e.CommandArgument)].Cells[2].Controls[0];
                //if (btn.Text == "ativar")
                //    status = true;

                Boolean ativo = Convert.ToBoolean(gridPlanos.DataKeys[Convert.ToInt32(e.CommandArgument)][1]);

                Object id = gridPlanos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Plano.AlterarStatus(id, !ativo);
                this.CarregaPlanos();
            }
        }

        protected void gridPlanos_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja prosseguir?");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Boolean ativo = Convert.ToBoolean(gridPlanos.DataKeys[e.Row.RowIndex][1]);

                if (!ativo)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    //((LinkButton)e.Row.Cells[1].Controls[0]).Text = "ativar";
                    ((LinkButton)e.Row.Cells[1].Controls[0]).Text = "<img src='images/unactive.png' alt='inativo' border='0'>";
                }
                else
                {
                    ((LinkButton)e.Row.Cells[1].Controls[0]).Text = "<img src='images/active.png'  alt='ativo' border='0'>";
                }
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/plano.aspx");
        }

        protected void cboOperadoras_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
            this.CarregaPlanos();
        }

        protected void cboContrato_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaPlanos();
        }
    }
}
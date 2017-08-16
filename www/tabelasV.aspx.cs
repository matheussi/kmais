namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class tabelasV : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaCategorias();
                //this.CarregaOperadoras();
                //this.CarregaContratos();
                this.CarregaTabelasDeValores();
            }
        }

        //void CarregaOperadoras()
        //{
        //    cboOperadoras.DataTextField = "Nome";
        //    cboOperadoras.DataValueField = "ID";
        //    cboOperadoras.DataSource = Operadora.CarregarTodas(true);
        //    cboOperadoras.DataBind();
        //}

        //void CarregaContratos()
        //{
        //    cboContratos.Items.Clear();
        //    if (cboOperadoras.Items.Count == 0){ return; }

        //    cboContratos.DataValueField = "ID";
        //    cboContratos.DataTextField = "Descricao";
        //    cboContratos.DataSource = ContratoADM.Carregar(cboOperadoras.SelectedValue);
        //    cboContratos.DataBind();
        //}

        void CarregaCategorias()
        {
            base.ExibirCategorias(cboCategoria, true, false);
        }

        void CarregaTabelasDeValores()
        {
            if (cboCategoria.Items.Count == 0 ) { gridTabelas.DataSource = null; gridTabelas.DataBind(); return; } 

            //gridTabelas.DataSource = TabelaValor.CarregarPorCategoriaID(cboCategoria.SelectedValue);
            gridTabelas.DataBind();
        }

        protected void gridTabelas_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session[IDKey] = gridTabelas.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("tabelaV.aspx?", IDKey, "=", Session[IDKey]));
            }
        }

        protected void gridTabelas_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            //base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja prosseguir?");

            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    Boolean ativo = Convert.ToBoolean(gridTabelas.DataKeys[e.Row.RowIndex][1]);

            //    if (!ativo)
            //    {
            //        e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
            //        ((LinkButton)e.Row.Cells[2].Controls[0]).Text = "ativar";
            //    }
            //}
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    if (Convert.ToString(gridTabelas.DataKeys[e.Row.RowIndex][0]) ==
            //       Convert.ToString(gridTabelas.DataKeys[e.Row.RowIndex][1]))
            //    {
            //        e.Row.Cells[1].Text = "Sim";
            //    }
            //    else
            //    {
            //        e.Row.Cells[1].Text = "Não";
            //    }
            //}
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/tabelaV.aspx");
        }

        protected void cboOperadoras_SelectedIndexChanged(Object sender, EventArgs e)
        {
            //this.CarregaContratos();
            //this.CarregaPlanos();
            this.CarregaTabelasDeValores();
        }

        protected void cboContratos_SelectedIndexChanged(Object sender, EventArgs e)
        {
            //this.CarregaPlanos();
            this.CarregaTabelasDeValores();
        }

        protected void cboCategoria_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelasDeValores();
        }
    }
}

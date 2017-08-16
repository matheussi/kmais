namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class almoxTiposProd : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadoras, false);
                cboOperadoras.Items.Insert(0, new ListItem("sem vínculo", "-1"));
                this.CarregaProdutos();
            }
        }

        void CarregaProdutos()
        {
            if (cboOperadoras.Items.Count > 0 && base.HaItemSelecionado(cboOperadoras))
                gridProdutos.DataSource = AlmoxProduto.CarregarTodosPorOperadora(cboOperadoras.SelectedValue);
            else if (cboOperadoras.SelectedIndex == 0)
                gridProdutos.DataSource = AlmoxProduto.CarregarTodosSemOperadora();
            else
                gridProdutos.DataSource = null;

            gridProdutos.DataBind();
        }

        protected void cboOperadoras_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaProdutos();
        }

        protected void gridProdutos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Object id = gridProdutos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("almoxTipoProd.aspx?", IDKey, "=", id));
            }
            else if (e.CommandName.Equals("inativar"))
            {
                Boolean ativo = Convert.ToBoolean(gridProdutos.DataKeys[Convert.ToInt32(e.CommandArgument)][1]);

                Object id = gridProdutos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                AlmoxProduto.AlterarStatus(id, !ativo);
                this.CarregaProdutos();
            }
            else if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    Object id = gridProdutos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                    AlmoxProduto prod = new AlmoxProduto(id);
                    prod.Remover();
                    this.CarregaProdutos();
                }
                catch
                {
                    base.Alerta(null, this, "_exclErr", "Não foi possível excluir o produto.");
                }
            }
        }

        protected void gridProdutos_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja prosseguir?");
            base.grid_RowDataBound_Confirmacao(sender, e, 5, "Deseja prosseguir com a exclusão?");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Boolean ativo = Convert.ToBoolean(gridProdutos.DataKeys[e.Row.RowIndex][1]);

                if (!ativo)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    e.Row.Cells[1].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    ((LinkButton)e.Row.Cells[4].Controls[0]).Text = "<img src='../images/unactive.png' title='inativo' alt='inativo' border='0'>";
                }
                else
                {
                    ((LinkButton)e.Row.Cells[4].Controls[0]).Text = "<img src='../images/active.png' title='ativo' alt='ativo' border='0'>";
                }
            }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/almoxTipoProd.aspx");
        }
    }
}
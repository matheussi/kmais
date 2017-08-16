namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class AlmoxEntradas : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);
                cboOperadora.Items.Insert(0, new ListItem("<sem vínculo>", "-1"));
                cboOperadora.Items.Insert(0, new ListItem("<TODAS>", "0"));

                base.ExibirFiliais(cboFilial, false);
                cboFilial.Items.Insert(0, new ListItem("<TODAS>", "-1"));

                //cboProduto.Items.Insert(0, new ListItem("NENHUM FILTRO", "-1"));
                base.ExibirTiposDeProdutos(cboTipo, true, "<TODOS>");
                this.CarregaMovimentacoes();
            }
        }

        void CarregaMovimentacoes()
        {
            //if (cboTipo.SelectedValue == "-1")
            //    gridEntradas.DataSource = AlmoxMovimentacao.CarregarTodos(AlmoxMovimentacao.TipoMovimentacao.Entrada);
            //else
            //{
            //    Object prodId = null;
            //    if (cboProduto.SelectedIndex > 0) { prodId = cboProduto.SelectedValue; }
            //    gridEntradas.DataSource = AlmoxMovimentacao.CarregarTodos(AlmoxMovimentacao.TipoMovimentacao.Entrada, cboTipo.SelectedValue, prodId);
            //}
            gridEntradas.DataSource = AlmoxMovimentacao.CarregarTodasEntradas(cboOperadora.SelectedValue, cboFilial.SelectedValue, cboTipo.SelectedValue);
            gridEntradas.DataBind();
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/AlmoxEntrada.aspx");
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaMovimentacoes();
        }

        protected void cboFilial_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaMovimentacoes();
        }

        protected void cboTipo_SelectedIndexChanged(Object sender, EventArgs e)
        {
            gridEntradas.SelectedIndex = -1;
            this.CarregaMovimentacoes();

            //cboProduto.Items.Clear();

            //if (cboTipo.SelectedIndex > 0)
            //{
            //    cboProduto.DataValueField = "ID";
            //    cboProduto.DataTextField = "Descricao";
            //    cboProduto.DataSource = AlmoxProduto.CarregarTodos(cboTipo.SelectedValue, true);
            //    cboProduto.DataBind();
            //}

            //cboProduto.Items.Insert(0, new ListItem("NENHUM FILTRO", "-1"));
            //cboProduto.SelectedIndex = 0;
        }

        protected void cboProduto_SelectedIndexChanged(Object sender, EventArgs e)
        {
            gridEntradas.SelectedIndex = -1;
            this.CarregaMovimentacoes();
        }
    }
}
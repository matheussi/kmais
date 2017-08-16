namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class adicionalRegra : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);
                this.CarregaTipoRestricao();
                this.CarregaProdutosAdicionais();

                if (base.IDKeyParameterInProcess(ViewState, "_adic"))
                {
                    this.CarregaRegra();
                }
            }
        }

        void CarregaTipoRestricao()
        {
            //cboRestricao.Items.Add(new ListItem("Titular e QUALQUER dependente ou agregado", "0"));
            cboRestricao.Items.Add(new ListItem("Titular e TODOS dependentes ou agregados", "1"));
        }

        void CarregaProdutosAdicionais()
        {
            cboProduto.Items.Clear();
            if (cboOperadora.Items.Count == 0) { return; }
            cboProduto.DataValueField = "ID";
            cboProduto.DataTextField = "Descricao";
            cboProduto.DataSource = Adicional.CarregarPorOperadoraID(cboOperadora.SelectedValue);
            cboProduto.DataBind();
        }

        void CarregaRegra()
        {
            AdicionalRegra regra = new AdicionalRegra(ViewState[IDKey]);
            regra.Carregar();

            cboOperadora.SelectedValue = Convert.ToString(regra.OperadoraID);
            cboOperadora_SelectedIndexChanged(null, null);
            cboProduto.SelectedValue   = Convert.ToString(regra.AdicionalID);
            cboRestricao.SelectedValue = Convert.ToString(regra.Tipo);
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaProdutosAdicionais();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/adicionaisRegra.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region Validacoes 

            if (cboOperadora.Items.Count == 0)
            {
                base.Alerta(null, this, "_err0", "Não há uma operadora selecionada.");
                return;
            }

            if (cboProduto.Items.Count == 0)
            {
                base.Alerta(null, this, "_err1", "Não há um produto adicional selecionado.");
                return;
            }

            AdicionalRegra teste = AdicionalRegra.Carregar(cboOperadora.SelectedValue, cboProduto.SelectedValue);
            if (teste != null)
            {
                base.Alerta(null, this, "_err2", "Já há uma regra para o produto adicional selecionado.");
                return;
            }

            #endregion

            AdicionalRegra regra = new AdicionalRegra(ViewState[IDKey]);
            regra.AdicionalID = cboProduto.SelectedValue;
            regra.OperadoraID = cboOperadora.SelectedValue;
            regra.Tipo = Convert.ToInt32(cboRestricao.SelectedValue);
            regra.Salvar();
            ViewState[IDKey] = regra.ID;

            base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
        }
    }
}

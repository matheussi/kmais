namespace www.financeiro
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class categoria : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                if (base.IDKeyParameterInProcess(ViewState, "_ccategoria"))
                {
                    this.CarregaCategoria();
                }
            }
        }

        void CarregaCategoria()
        {
            CategoriaContaCorrente obj = new CategoriaContaCorrente(ViewState[IDKey]);
            obj.Carregar();
            txtNome.Text = obj.Descricao;
            obj = null;
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("categorias.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtNome.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err", "Informe a descrição da categoria.");
                txtNome.Focus();
                return;
            }

            CategoriaContaCorrente obj = new CategoriaContaCorrente(ViewState[IDKey]);
            if (ViewState[IDKey] != null) { obj.Carregar(); };

            obj.Descricao = txtNome.Text;
            if (optCredito.Checked)
                obj.Tipo = Convert.ToInt32(CategoriaContaCorrente.eTipo.Credito);
            else
                obj.Tipo = Convert.ToInt32(CategoriaContaCorrente.eTipo.Debito);

            obj.Salvar();
            ViewState[IDKey] = obj.ID;
            base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
        }
    }
}

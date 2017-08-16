namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class categoria : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaPerfis();

                if (base.IDKeyParameterInProcess(ViewState, "_cat"))
                {
                    this.CarregaCategoria();
                }
            }
        }

        void CarregaPerfis()
        {
            cboPerfil.DataValueField = "ID";
            cboPerfil.DataTextField = "Descricao";
            cboPerfil.DataSource = Perfil.CarregarComissionaveis();
            cboPerfil.DataBind();
        }

        void CarregaCategoria()
        {
            Categoria cat = new Categoria();
            cat.ID = ViewState[IDKey];
            cat.Carregar();
            txtDescricao.Text = cat.Descricao;
            chkAtiva.Checked = cat.Ativo;
            if (cat.PerfilID != null)
                cboPerfil.SelectedValue = Convert.ToString(cat.PerfilID);
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/categorias.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err", "Informe o nome da categoria.");
                txtDescricao.Focus();
                return;
            }

            Categoria cat = new Categoria();
            cat.ID = ViewState[IDKey];
            cat.Descricao = txtDescricao.Text.ToUpper();
            cat.Ativo = chkAtiva.Checked;
            if (cboPerfil.Items.Count > 0)
                cat.PerfilID = cboPerfil.SelectedValue;
            cat.Salvar();
            ViewState[IDKey] = cat.ID;

            base.Alerta(null, this, "_salvo", "Categoria salva com sucesso.");
        }
    }
}
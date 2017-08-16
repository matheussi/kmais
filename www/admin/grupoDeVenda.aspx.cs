namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class grupoDeVenda : PageBase
    {
        Object grupoId
        {
            get
            {
                if (ViewState[IDKey] != null)
                    return ViewState[IDKey];
                else
                    return Request[IDKey];
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                if (grupoId != null) { this.CarregaGrupo(); }
            }
        }

        void CarregaGrupo()
        {
            GrupoDeVenda grupo = new GrupoDeVenda(grupoId);
            grupo.Carregar();
            txtDescricao.Text = grupo.Descricao;
            chkAtivo.Checked = grupo.Ativo;
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/gruposDeVenda.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err", "Informe a descrição do grupo de venda.");
                txtDescricao.Focus();
                return;
            }

            GrupoDeVenda grupo = new GrupoDeVenda(grupoId);
            grupo.Descricao = txtDescricao.Text.ToUpper();
            grupo.Ativo = chkAtivo.Checked;
            grupo.Salvar();
            ViewState[IDKey] = grupo.ID;

            base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
        }
    }
}
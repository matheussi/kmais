namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class sysuser : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.CarregaPerfis();

                if (base.IDKeyParameterInProcess(ViewState, "_user"))
                {
                    this.CarregaUsuario();
                    txtNome.Focus();
                }
                else
                    cboPerfil.Focus();
            }
        }

        void CarregaPerfis()
        {
            cboPerfil.Items.Clear();
            cboPerfil.DataValueField = "ID";
            cboPerfil.DataTextField  = "Descricao";
            cboPerfil.DataSource     = Perfil.CarregarTodos(Perfil.eTipo.Indefinido);
            cboPerfil.DataBind();
        }

        void CarregaUsuario()
        {
            Usuario usuario = new Usuario();
            usuario.ID = ViewState[IDKey];
            usuario.Carregar();

            cboPerfil.SelectedValue = Convert.ToString(usuario.PerfilID);
            this.ChecaPerfil();
            txtNome.Text = usuario.Nome;
            txtEmail.Text = usuario.Email;

            chkAtivo.Checked = usuario.Ativo;
            chkLibera.Checked = usuario.LiberaContratos;
            chkAlteraProdutor.Checked = usuario.AlteraProdutor;
            chkAlteraValor.Checked = usuario.AlteraValorContratos;

            txtObs.Text = usuario.Obs;
        }

        protected void cboPerfil_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.ChecaPerfil();
        }

        void ChecaPerfil()
        {
            Perfil perfil = new Perfil();
            perfil.ID = cboPerfil.SelectedValue;
            perfil.Carregar();
            chkAlteraValor.Enabled = perfil.Tipo != (Int32)Perfil.eTipo.Produtor;
            chkLibera.Enabled = perfil.Tipo != (Int32)Perfil.eTipo.Produtor;
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/sysusers.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes

            if (cboPerfil.Items.Count == 0)
            {
                base.Alerta(null, this, "_errPl", "Não há um perfil selecionado.");
                return;
            }

            if (txtNome.Text.Trim().Length <= 2)
            {
                base.Alerta(null, this, "_errNm", "Informe o nome do usuário.");
                txtNome.Focus();
                return;
            }

            if (txtEmail.Text.Trim() == "")
            {
                base.Alerta(null, this, "_errMail", "Informe o e-mail do usuário.");
                txtEmail.Focus();
                return;
            }

            String msg = "";
            if (!base.ValidaEmail(txtEmail.Text, out msg))
            {
                base.Alerta(null, this, "_errMail", msg);
                txtEmail.Focus();
                return;
            }

            if (txtSenha1.Text.Trim() != "")
            {
                if (!txtSenha1.Text.Trim().Equals(txtSenha2.Text.Trim()))
                {
                    base.Alerta(null, this, "_erro0", "Senhas não conferem.");
                    txtSenha1.Focus();
                    return; //senhas não conferem
                }
            }

            if (!IsValid) { return; }

            #endregion

            Usuario user = new Usuario();
            if (ViewState[IDKey] != null)
            {
                user.ID = ViewState[IDKey];
                user.Carregar();
            }

            user.Nome = txtNome.Text;
            user.Email = txtEmail.Text;
            user.PerfilID = cboPerfil.SelectedValue;

            if (txtSenha1.Text.Trim() != "")
                user.Senha = txtSenha1.Text;

            user.AlteraValorContratos = chkAlteraValor.Checked;
            user.LiberaContratos = chkLibera.Checked;
            user.AlteraProdutor = chkAlteraProdutor.Checked;
            user.Ativo = chkAtivo.Checked;

            user.Obs = txtObs.Text;

            user.Salvar();
            ViewState[IDKey] = user.ID;
            base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
        }
    }
}

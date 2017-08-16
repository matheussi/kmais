namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class alterarSenha : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtSenha1.Attributes.Add("value", txtSenha1.Text);
            txtSenha2.Attributes.Add("value", txtSenha2.Text);
            txtSenhaAtual.Attributes.Add("value", txtSenhaAtual.Text);

            if (!IsPostBack)
            {
                Usuario user = new Usuario(Usuario.Autenticado.ID);
                user.Carregar();
                txtLogin.Text = user.Email;
            }
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtSenhaAtual.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err1", "Informe a senha atual.");
                txtSenhaAtual.Focus();
                return;
            }

            if (txtLogin.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err1", "Informe seu login.");
                txtLogin.Focus();
                return;
            }

            if (txtSenha1.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err2", "Informe sua nova senha.");
                txtSenha1.Focus();
                return;
            }

            if (txtSenha2.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err3", "Você deve confirmar sua nova senha.");
                txtSenha2.Focus();
                return;
            }

            if (txtSenha1.Text != txtSenha2.Text)
            {
                base.Alerta(null, this, "_err4", "As senhas informadas não conferem.");
                txtSenha1.Focus();
                return;
            }

            Usuario user = new Usuario(Usuario.Autenticado.ID);

            if (!Usuario.ChecaLogin(user.ID, txtLogin.Text))
            {
                base.Alerta(null, this, "_err1", "O login informado não pode ser utilizado.");
                txtLogin.Focus();
                return;
            }

            user.Carregar();

            if (user.Senha != user.Encripta(txtSenhaAtual.Text))
            {
                base.Alerta(null, this, "_err5", "A senha atual não confere com a informada.");
                txtSenhaAtual.Focus();
                return;
            }

            String novaSenhaEncriptada = user.Encripta(txtSenha1.Text);
            if (novaSenhaEncriptada != user.Senha)
            {
                user.Senha = txtSenha1.Text;
                user.Salvar();
            }

            base.Alerta(null, this, "_ok", "Senha alterada com sucesso.");
        }
    }
}
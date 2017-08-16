using System;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using LC.Web.PadraoSeguros.Entity;

namespace www
{
    public partial class novousuario : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes

            if (txtNome.Text.Trim().Length <= 2)
            {
                Alerta(null, this, "_errNm", "Informe o nome do usuário.");
                txtNome.Focus();
                return;
            }

            if (txtEmail.Text.Trim() == "")
            {
                Alerta(null, this, "_errMail", "Informe o login do usuário.");
                txtEmail.Focus();
                return;
            }

            //String msg = "";
            //if (!base.ValidaEmail(txtEmail.Text, out msg))
            //{
            //    base.Alerta(null, this, "_errMail", msg);
            //    txtEmail.Focus();
            //    return;
            //}

            if (txtSenha1.Text.Trim() != "")
            {
                if (!txtSenha1.Text.Trim().Equals(txtSenha2.Text.Trim()))
                {
                    Alerta(null, this, "_erro0", "Senhas não conferem.");
                    txtSenha1.Focus();
                    return; //senhas não conferem
                }
            }

            if (!IsValid) { return; }

            #endregion

            Usuario user = new Usuario();

            user.Nome     = txtNome.Text;
            user.Email    = txtEmail.Text;

            if(!chkTipo.Checked)
                user.PerfilID = 13;
            else
                user.PerfilID = 16;

            if (txtSenha1.Text.Trim() != "")
                user.Senha = txtSenha1.Text;

            user.AlteraValorContratos = false;
            user.LiberaContratos = false;
            user.Ativo = true;

            user.Obs = "";

            user.Salvar();
            Alerta(null, this, "_salvo", "Usuário criado com sucesso.");

            txtNome.Text = "";
            txtEmail.Text = "";
            txtSenha1.Text = "";
            txtSenha2.Text = "";
        }

        protected void Alerta(UpdatePanel uPanel, Page page, String chave, String Mensagem)
        {
            if (uPanel != null)
            {
                ScriptManager.RegisterClientScriptBlock(
                    uPanel, page.GetType(), chave, "alert('" + Mensagem + "');", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(
                    page, page.GetType(), chave, "alert('" + Mensagem + "');", true);
            }
        }
    }
}

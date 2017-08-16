namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class filial : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cmdBuscaEndereco.Visible = base.useExternalCEPEngine();
            txtNumero.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");

            if (!IsPostBack)
            {
                UIHelper.AuthCtrl(cmdSalvar, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                if (base.IDKeyParameterInProcess(ViewState, "_filial"))
                {
                    this.CarregaFilial();
                }
            }
        }

        void CarregaFilial()
        {
            Filial filial = new Filial();
            filial.ID = ViewState[IDKey];
            filial.Carregar();

            txtNome.Text = filial.Nome;
            txtEmail.Text = filial.Email;
            txtFone.Text = filial.Telefone;
            chkAtiva.Checked = filial.Ativa;

            if (filial.Endereco != null)
            {
                ViewState[IDKey2] = filial.Endereco.ID;
                txtCEP.Text = filial.Endereco.CEP;
                txtLogradouro.Text = filial.Endereco.Logradouro;
                txtNumero.Text = filial.Endereco.Numero;
                txtComplemento.Text = filial.Endereco.Complemento;
                txtBairro.Text = filial.Endereco.Bairro;
                txtCidade.Text = filial.Endereco.Cidade;
                txtUF.Text = filial.Endereco.UF.Trim();
            }
        }

        protected void cmdBuscaEndereco_Click(Object sender, EventArgs e)
        {
            base.PegaEndereco(this, txtCEP, txtLogradouro, txtBairro, txtCidade, txtUF, txtNumero, true);
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/filiais.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtNome.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_err0", "Informe o nome da filial.");
                base.Alerta(MPE, ref litAlert, "Informe o nome da filial.", upnlAlerta);
                txtNome.Focus();
                return;
            }

            String msg = "";
            if (!base.ValidaEmail(txtEmail.Text, out msg))
            {
                //base.Alerta(null, this, "_errMail", msg);
                base.Alerta(MPE, ref litAlert, msg, upnlAlerta);
                txtEmail.Focus();
                return;
            }

            if (txtUF.Text.Trim() != "" && !UIHelper.ValidaUF(txtUF.Text))
            {
                //base.Alerta(null, this, "_errUF", "Unidade Federativa inválida.");
                base.Alerta(MPE, ref litAlert, "Unidade Federativa inválida.", upnlAlerta);
                txtUF.Focus();
                return;
            }

            if (Filial.Duplicado(ViewState[IDKey], txtNome.Text.Trim()))
            {
                base.Alerta(MPE, ref litAlert, "Já existe uma filial com esse nome.", upnlAlerta);
                txtNome.Focus();
                return;
            }

            Filial filial = new Filial();
            filial.ID = ViewState[IDKey];

            if (filial.ID != null) { filial.Carregar(); }

            filial.Ativa = chkAtiva.Checked;
            filial.Email = txtEmail.Text;
            filial.Nome = txtNome.Text.ToUpper();
            filial.Telefone = txtFone.Text;

            if (filial.Endereco == null) { filial.Endereco = new Endereco(); }
            filial.Endereco.Bairro = txtBairro.Text.ToUpper();
            filial.Endereco.CEP = txtCEP.Text;
            filial.Endereco.Cidade = txtCidade.Text.ToUpper();
            filial.Endereco.Complemento = txtComplemento.Text.ToUpper();
            filial.Endereco.Logradouro = txtLogradouro.Text.ToUpper();
            filial.Endereco.Numero = txtNumero.Text.ToUpper();
            filial.Endereco.Tipo = (Int32)Endereco.TipoEndereco.Residencial;
            filial.Endereco.UF = txtUF.Text.ToUpper();

            filial.Salvar();

            ViewState[IDKey] = filial.ID;
            ViewState[IDKey2] = filial.Endereco.ID;

            //base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
        }
    }
}

namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class estadocivil : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);
                if (base.IDKeyParameterInProcess(ViewState, "_estadoCivil"))
                {
                    this.CarregaEstadoCivil();
                }
            }
        }

        void CarregaEstadoCivil()
        {
            EstadoCivil estado = new EstadoCivil(ViewState[IDKey]);
            estado.Carregar();

            cboOperadora.SelectedValue  = Convert.ToString(estado.OperadoraID);
            txtDescricao.Text           = estado.Descricao;
            txtCodigo.Text              = estado.Codigo;
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("estadoscivis.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacao 

            if (!base.HaItemSelecionado(cboOperadora))
            {
                base.Alerta(null, this, "_err1", "Não há uma operadora selecionada.");
                cboOperadora.Focus();
                return;
            }

            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err2", "Informe uma descrição.");
                txtDescricao.Focus();
                return;
            }

            if (txtCodigo.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err3", "Informe um código.");
                txtCodigo.Focus();
                return;
            }

            #endregion

            EstadoCivil estadocivil = new EstadoCivil(ViewState[IDKey]);
            estadocivil.OperadoraID = cboOperadora.SelectedValue;
            estadocivil.Codigo      = txtCodigo.Text;
            estadocivil.Descricao   = txtDescricao.Text;

            estadocivil.Salvar();
            ViewState[IDKey]        = estadocivil.ID;

            base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
        }
    }
}
namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class atendTipo : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                AtendimentoTipo.UI.FillComboWithTypes(cboTipo);

                if (base.IDKeyParameterInProcess(ViewState, "_tipoAtend"))
                {
                    this.CarregaTipoDeAtendimento();
                }
            }
        }

        void CarregaTipoDeAtendimento()
        {
            AtendimentoTipo obj = new AtendimentoTipo();
            obj.ID = ViewState[IDKey];
            obj.Carregar();
            txtDescricao.Text = obj.Descricao;
            cboTipo.SelectedValue = obj.Tipo.ToString();
            txtEmail.Text = obj.Email;
            txtPrazoConclusao.Text = obj.PrazoConclusao.ToString();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/atendTipoLista.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err", "Informe a descrição do tipo de atendimento.");
                txtDescricao.Focus();
                return;
            }

            AtendimentoTipo obj = new AtendimentoTipo(ViewState[IDKey]);
            obj.Descricao = txtDescricao.Text;
            obj.Email = txtEmail.Text;
            obj.Tipo = Convert.ToInt32(cboTipo.SelectedValue);
            obj.PrazoConclusao = base.CToInt(txtPrazoConclusao.Text);
            obj.Salvar();

            ViewState[IDKey] = obj.ID;

            base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
        }
    }
}

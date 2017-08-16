namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class feriado : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                if (base.IDKeyParameterInProcess(ViewState, "_filial"))
                {
                    this.CarregarFeriado();
                }

                txtDescricao.Focus();
            }
        }

        void CarregarFeriado()
        {
            DiaFeriado feriado = new DiaFeriado(ViewState[IDKey]);
            feriado.Carregar();
            txtData.Text = feriado.Data.ToString("dd/MM/yyyy");
            txtDescricao.Text = feriado.Descricao;
            txtObs.Text = feriado.OBS;
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("feriados.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if(txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "err", "Preencha o campo \"Descrição\".");
                txtDescricao.Focus();
                return;
            }

            DiaFeriado feriado = new DiaFeriado(ViewState[IDKey]);
            if (feriado.ID != null) { feriado.Carregar(); }

            feriado.Data = base.CStringToDateTime(txtData.Text);
            if (feriado.Data == DateTime.MinValue)
            {
                base.Alerta(null, this, "err", "A data informada é inválida.");
                txtData.Focus();
                return;
            }

            feriado.Descricao = txtDescricao.Text;
            feriado.OBS = txtObs.Text;
            feriado.UsuarioID = Usuario.Autenticado.ID;
            feriado.Salvar();

            Response.Redirect("feriados.aspx");
        }
    }
}
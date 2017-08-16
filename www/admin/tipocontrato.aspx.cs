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

    public partial class tipocontrato : PageBase
    {
        Object tipoContratoId
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
                this.CarregaTiposDeComissionamento();
                if (tipoContratoId != null) { this.CarregaTipoContrato(); }
            }
        }

        void CarregaTiposDeComissionamento()
        {
            cboComissao.Items.Clear();
            Hashtable ht = TipoContrato.CarregaTiposDeComissionamento();
            foreach (DictionaryEntry entry in ht)
            {
                cboComissao.Items.Add(new ListItem(
                    Convert.ToString(entry.Value).ToUpper(), Convert.ToString(entry.Key)));
            }
            ht = null;
        }

        void CarregaTipoContrato()
        {
            TipoContrato tipo = new TipoContrato();
            tipo.ID = tipoContratoId;
            tipo.Carregar();

            if (tipo.ID == null) { Response.Redirect("~/admin/tiposcontrato.aspx"); }

            txtDescricao.Text = tipo.Descricao;
            chkAtivo.Checked = tipo.Ativo;
            cboComissao.SelectedValue = Convert.ToString(tipo.TipoComissionamento);
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/tiposcontrato.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err", "Informe a descrição do tipo de contrato.");
                txtDescricao.Focus();
                return;
            }

            TipoContrato tipo = new TipoContrato();
            tipo.ID = tipoContratoId;
            tipo.Descricao = txtDescricao.Text.ToUpper();
            tipo.Ativo = chkAtivo.Checked;
            tipo.TipoComissionamento = Int32.Parse(cboComissao.SelectedValue);
            tipo.Salvar();
            ViewState[IDKey] = tipo.ID;

            base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
        }
    }
}
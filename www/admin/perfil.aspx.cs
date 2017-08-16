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

    public partial class perfil : PageBase
    {
        Object perfilId 
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
                this.CarregaTiposDePerfil();
                this.CarregaPerfisSegundoTipo();
                if (perfilId != null) { this.CarregaPerfil(); }
            }
        }

        void CarregaTiposDePerfil()
        {
            cboTipo.Items.Clear();
            Hashtable ht = Perfil.CarregarTipos();
            foreach (DictionaryEntry entry in ht)
            {
                cboTipo.Items.Add(new ListItem(
                    Convert.ToString(entry.Value).ToUpper(),
                    Convert.ToString(entry.Key)));
            }
        }

        void CarregaPerfisSegundoTipo()
        {
            cboPerfilPai.Items.Clear();
            cboPerfilPai.DataValueField = "ID";
            cboPerfilPai.DataTextField = "Descricao";
            int tipo = Int32.Parse(cboTipo.SelectedValue);
            cboPerfilPai.DataSource = Perfil.CarregarTodos((Perfil.eTipo)tipo);
            cboPerfilPai.DataBind();
            cboPerfilPai.Items.Insert(0, new ListItem("Selecione", "-1"));
        }

        void CarregaPerfil()
        {
            Perfil obj = new Perfil(perfilId);
            obj.Carregar();
            txtDescricao.Text = obj.Descricao;
            cboTipo.SelectedValue = obj.Tipo.ToString();
            this.CarregaPerfisSegundoTipo();

            if (obj.ParentID != null)
                cboPerfilPai.SelectedValue = Convert.ToString(obj.ParentID);
            else
                cboPerfilPai.SelectedIndex = 0;

            
            chkComissionavel.Checked = obj.Comissionavel;
            chkVende.Checked = obj.ParticipanteContrato;
        }

        protected void cboTipo_OnSelectChanged(Object sender, EventArgs e)
        {
            this.CarregaPerfisSegundoTipo();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/perfis.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err", "Informe a descrição do perfil.");
                txtDescricao.Focus();
                return;
            }

            if (perfilId != null && cboPerfilPai.SelectedValue == Convert.ToString(perfilId))
            {
                base.Alerta(null, this, "__errB", "Perfil pai inválido.");
                txtDescricao.Focus();
                return;
            }

            Perfil obj = new Perfil(perfilId);
            obj.Descricao = txtDescricao.Text.ToUpper();
            obj.Comissionavel = chkComissionavel.Checked;

            if (cboPerfilPai.SelectedIndex > 0)
                obj.ParentID = cboPerfilPai.SelectedValue;
            else
                obj.ParentID = null;

            obj.ParticipanteContrato = chkVende.Checked;
            obj.Tipo = Int32.Parse(cboTipo.SelectedValue);
            obj.Salvar();
            ViewState[IDKey] = obj.ID;

            base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
        }
    }
}
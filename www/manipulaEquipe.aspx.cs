namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;
    using LC.Web.PadraoSeguros.Entity.Untyped;

    public partial class manipulaEquipe : PageBase
    {
        /// <summary>
        /// Representa a Lista de Usuários Subordinados de um Superior escolhido via Auto Complete.
        /// </summary>
        private IList<Usuario> UsuarioSubordinado
        {
            get { return Cache["__UsuarioSubordinadoList__"] as IList<Usuario>; }
            set { Cache.Remove("__UsuarioSubordinadoList__"); if (value != null) { Cache.Insert("__UsuarioSubordinadoList__", value, null, DateTime.Now.AddHours(1), TimeSpan.Zero); } }
        }

        /// <summary>
        /// Método para Configurar as Regras para os Campos.
        /// </summary>
        private void SetFieldConstraints()
        {
            this.litAutoCompSuperiorPara.Text = String.Concat("\"#", this.txtSuperiorPara.ClientID ,"\"");
            this.litCorretorParaID.Text       = this.txtCorretorParaID.ClientID;
        }

        /// <summary>
        /// Método para Resetar os Campos do Form.
        /// </summary>
        private void ResetFields()
        {
            this.txtSuperior.Text        = String.Empty;
            this.txtSuperiorID.Value     = String.Empty;
            this.txtSuperiorPara.Text    = String.Empty;
            this.txtCorretorParaID.Value = String.Empty;
            this.txtVigenciaNova.Text    = String.Empty;
        }

        /// <summary>
        /// Método para Carregar os Perfis na ComboBox de Perfil.
        /// </summary>
        private void CarregaPerfis()
        {
            cboPerfil.DataTextField  = "Descricao";
            cboPerfil.DataValueField = "ID";
            cboPerfil.DataSource     = Perfil.CarregarTodos(true);
            cboPerfil.DataBind();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Cache.Remove("__UsuarioSubordinadoList__");
                this.CarregaPerfis();
                this.SetFieldConstraints();
            }
        }

        protected void gridEquipe_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                this.UsuarioSubordinado.RemoveAt(Convert.ToInt32(e.CommandArgument));
                gridEquipe.DataSource = this.UsuarioSubordinado;
                gridEquipe.DataBind();
            }
        }

        protected void gridEquipe_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 3, "Confirma a exclusão?");
        }

        protected void cmdExibir_OnClick(Object sender, EventArgs e)
        {
            Object perfilId = null, superiorId = null;

            perfilId = cboPerfil.SelectedValue;

            if (txtSuperior.Text.Trim() != "" && txtSuperiorID.Value.Trim() != "")
                superiorId = txtSuperiorID.Value;

            List<Usuario> lstUsuarioSubordinados = new List<Usuario>();
            
            Usuario.CarregaSubordinadosRecursivo(superiorId, perfilId, ref lstUsuarioSubordinados);

            this.UsuarioSubordinado = lstUsuarioSubordinados;

            if (this.UsuarioSubordinado != null && this.UsuarioSubordinado.Count > 0)
                this.cmdAtribuir.Visible = true;
            else
                this.cmdAtribuir.Visible = false;

            gridEquipe.DataSource = this.UsuarioSubordinado;
            gridEquipe.DataBind();
        }

        protected void cmdAtribuir_OnClick(Object sender, EventArgs e)
        {
            #region Validations

            if (this.txtSuperior.Text.Trim().Length.Equals(0) || this.txtSuperiorID.Value.Length.Equals(0))
            {
                base.Alerta(null, this.Page, "_err1", "Você deve informar o atual superior.");
                this.txtSuperior.Focus();

                return;
            }
            
            if (this.txtSuperiorPara.Text.Trim().Length.Equals(0) || this.txtCorretorParaID.Value.Length.Equals(0))
            {
                base.Alerta(null, this.Page, "_err2", "Você deve informar o novo superior.");
                this.txtSuperiorPara.Focus();

                return;
            }

            DateTime dtVigencia = new DateTime();

            if (txtVigenciaNova.Text.Trim().Length.Equals(0))
            {
                base.Alerta(null, this.Page, "_err3", "Você deve informar a data de vigência.");
                txtVigenciaNova.Focus();

                return;
            }
            else
            {
                if (!UIHelper.TyParseToDateTime(txtVigenciaNova.Text, out dtVigencia))
                {
                    base.Alerta(null, this.Page, "_err3b", "A data de vigência informada está inválida.");
                    txtVigenciaNova.Focus();

                    return;
                }
            }

            #endregion

            if (this.gridEquipe.Rows != null && this.gridEquipe.Rows.Count > 0)
            {
                Object[] arrSubordinadoID = new Object[this.gridEquipe.Rows.Count];

                for (Int32 i = 0; i < this.gridEquipe.Rows.Count; i++)
                    arrSubordinadoID[i] = this.gridEquipe.DataKeys[i].Value.ToString().Trim();

                if (arrSubordinadoID != null && arrSubordinadoID.Length > 0)
                {
                    try
                    {
                        Usuario.AlteraSuperior(arrSubordinadoID, this.txtCorretorParaID.Value.Trim(), dtVigencia);

                        this.UsuarioSubordinado    = null;
                        this.cmdAtribuir.Visible   = false;
                        this.gridEquipe.DataSource = null;
                        this.gridEquipe.DataBind();

                        this.ResetFields();

                        base.Alerta(null, this, "_ok", "Dados salvos com sucesso!");
                    }
                    catch (Exception)
                    {
                        base.Alerta(null, this, "_err4", "Houve um erro inesperado.");
                    }
                }
            }
            else
            {
                // Não ha nada selecionado.
            }
        }
    }
}
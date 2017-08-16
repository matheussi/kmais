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

    public partial class manipulaGradeComissao : PageBase
    {
        DataTable dt
        {
            get { return Cache["___dt" + Session.SessionID] as DataTable; }
            set { Cache.Remove("___dt" + Session.SessionID); if (value != null) { Cache.Insert("___dt" + Session.SessionID, value, null, DateTime.Now.AddHours(1), TimeSpan.Zero); } }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                dt = null;
                this.CarregaPerfis();
                this.CarregaFiliais();
                this.CarregaGrupoDeVendas();
                this.CarregaTabelasDeComissionamento();
            }
        }

        void CarregaPerfis()
        {
            cboPerfil.DataTextField = "Descricao";
            cboPerfil.DataValueField = "ID";
            cboPerfil.DataSource = Perfil.CarregarTodos(true);
            cboPerfil.DataBind();
        }

        void CarregaFiliais()
        {
            cboFilial.DataValueField = "ID";
            cboFilial.DataTextField = "Nome";
            cboFilial.DataSource = Filial.CarregarTodas(true);
            cboFilial.DataBind();
            cboFilial.Items.Insert(0, new ListItem("TODAS", "-1"));
        }

        void CarregaGrupoDeVendas()
        {
            cboGrupoVendas.DataValueField = "ID";
            cboGrupoVendas.DataTextField = "Descricao";
            cboGrupoVendas.DataSource = GrupoDeVenda.Carregar(true);
            cboGrupoVendas.DataBind();
            cboGrupoVendas.Items.Insert(0, new ListItem("QUALQUER GRUPO", "-1"));
        }

        void CarregaTabelasDeComissionamento()
        {
            cboTabelaComissaoAtual.Items.Clear();
            cboTabelaComissaoNova.Items.Clear();
            if (cboPerfil.Items.Count == 0) { return; }

            IList<Comissionamento> lista = Comissionamento.CarregarTodos(Comissionamento.eTipo.PagoAoOperador, cboPerfil.SelectedValue);

            cboTabelaComissaoAtual.DataValueField = "ID";
            cboTabelaComissaoAtual.DataTextField = "Descricao";
            cboTabelaComissaoAtual.DataSource = lista;
            cboTabelaComissaoAtual.DataBind();
            cboTabelaComissaoAtual.Items.Insert(0, new ListItem("QUALQUER TABELA", "-1"));

            cboTabelaComissaoNova.DataValueField = "ID";
            cboTabelaComissaoNova.DataTextField = "Descricao";
            cboTabelaComissaoNova.DataSource = lista;
            cboTabelaComissaoNova.DataBind();
        }

        protected void cboPerfil_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelasDeComissionamento();
        }

        protected void cmdExibir_OnClick(Object sender, EventArgs e)
        {
            Object perfilId = null, filialId = null, grupoId = null, superiorId = null, tabelaId = null;

            perfilId = cboPerfil.SelectedValue;
            if (cboFilial.SelectedIndex > 0) { filialId = cboFilial.SelectedValue; }
            if (cboGrupoVendas.SelectedIndex > 0) { grupoId = cboGrupoVendas.SelectedValue; }
            if (txtSuperior.Text.Trim() != "" && txtSuperiorID.Value.Trim() != "") { superiorId = txtSuperiorID.Value; }
            if (cboTabelaComissaoAtual.SelectedIndex > 0) { tabelaId = cboTabelaComissaoAtual.SelectedValue; }

            dt = UntypedProcesses.ManipulacaoGradeComissaoQuery(perfilId, filialId, grupoId, superiorId, tabelaId);
            grid.DataSource = dt;
            grid.DataBind();

            if (dt != null && dt.Rows.Count > 0)
            {
                tblAtribuir.Visible = true;
                tblResultado.Visible = true;
            }
            else
            {
                tblAtribuir.Visible = false;
                tblResultado.Visible = false;
            }
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                dt.Rows.RemoveAt(Convert.ToInt32(e.CommandArgument));
                grid.DataSource = dt;
                grid.DataBind();
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 4, "Confirma a exclusão?");
        }

        protected void cmdAtribuir_OnClick(Object sender, EventArgs e)
        {
            #region validacoes 

            if (dt == null)
            {
                base.Alerta(null, this, "_err1", "As informações foram removidas da memória.\\nPor favor, refaça a pesquisa.");
                return;
            }

            DateTime vigencia = new DateTime();

            if (txtVigenciaNova.Text.Trim() == "")
            {
                base.Alerta(null, this.Page, "_err2", "Você deve informar a data de vigência.");
                txtVigenciaNova.Focus();
                return;
            }
            else
            {
                if (!UIHelper.TyParseToDateTime(txtVigenciaNova.Text, out vigencia))
                {
                    base.Alerta(null, this.Page, "_err2b", "A data de vigência informada está inválida.");
                    txtVigenciaNova.Focus();
                    return;
                }
            }

            if (cboTabelaComissaoAtual.SelectedValue == cboTabelaComissaoNova.SelectedValue)
            {
                base.Alerta(null, this, "_err3", "Informe uma tabela de comissionamento diferente.");
                cboTabelaComissaoNova.Focus();
                return;
            }

            #endregion

            try
            {
                List<ComissionamentoUsuario> list = new List<ComissionamentoUsuario>();
                foreach (DataRow row in dt.Rows)
                {
                    ComissionamentoUsuario obj = new ComissionamentoUsuario();
                    obj.Ativo = true;
                    obj.Data  = vigencia;
                    obj.PerfilID = cboPerfil.SelectedValue;

                    if (row["GrupoID"] != null && row["GrupoID"] != DBNull.Value)
                        obj.GrupoVendaID = row["GrupoID"];

                    obj.TabelaComissionamentoID = cboTabelaComissaoNova.SelectedValue;
                    obj.UsuarioID = row["UsuarioID"];

                    list.Add(obj);
                }

                ComissionamentoUsuario.Salvar(list); 
                base.Alerta(null, this, "_ok", "Dados salvos com sucesso!");
                dt = null;
                tblAtribuir.Visible = false;
                tblResultado.Visible = false;
                grid.DataSource = null;
                grid.DataBind();
            }
            catch
            {
                base.Alerta(null, this, "_err4", "Houve um erro inesperado.");
            }
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            grid.DataSource = dt;
            grid.DataBind();
        }
    }
}
namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Util;
    using LC.Web.PadraoSeguros.Entity;

    public partial class comissionamentosUsuario : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cmdSalvar.Attributes.Add("onClick", "return confirm('Deseja realmente prosseguir?');");

            if (!IsPostBack)
            {
                base.ExibirFiliais(cboFiliais, false);
                base.ExibirOperadoras(cboOperadora, false);
                this.CarregaContratos();
                this.CarregaUsuarios();
                this.CarregaTabelas(false);
                this.CarregaTabelas(true);
            }
        }

        void CarregaSuperiores()
        {
            cboSuperiores.Items.Clear();
            chklTabelasSuperiores.Items.Clear();

            if (cboUsuarios.Items.Count == 0) { return; }

            cboSuperiores.DataValueField = "ID";
            cboSuperiores.DataTextField  = "Nome_e_Perfil";
            cboSuperiores.DataSource = Usuario.CarregaSuperiores(cboUsuarios.SelectedValue);
            cboSuperiores.DataBind();
        }

        void CarregaUsuarios()
        {
            if (cboFiliais.Items.Count == 0) { cboUsuarios.Items.Clear(); return; }
            cboUsuarios.DataValueField = "ID";
            cboUsuarios.DataTextField  = "Nome";
            IList<Usuario> lista  = Usuario.CarregarPorFilial(cboFiliais.SelectedValue, true, true);

            if (lista != null && lista.Count > 0)
            {
                cboUsuarios.DataSource = lista;
                cboUsuarios.DataBind();
            }
            else
                cboUsuarios.Items.Clear();

            this.CarregaSuperiores();
        }

        protected void cboFiliais_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaUsuarios();
            this.CarregaTabelas(false);
            this.CarregaTabelas(true);
        }

        protected void cboUsuarios_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelas(false);
            this.CarregaSuperiores();
            this.CarregaTabelas(true);
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
            this.CarregaTabelas(false);
            this.CarregaTabelas(true);
        }

        protected void cboContrato_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelas(false);
        }

        protected void cboSuperiores_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelas(true);
        }

        void CarregaContratos()
        {
            cboContrato.Items.Clear();
            if (cboOperadora.Items.Count == 0) { return; }
            cboContrato.DataValueField = "ID";
            cboContrato.DataTextField = "Descricao";
            cboContrato.DataSource = ContratoADM.Carregar(cboOperadora.SelectedValue);
            cboContrato.DataBind();
        }

        void CarregaTabelas(Boolean superior)
        {
            String usuarioId = "";

            if (!superior)
            {
                chklTabelas.Items.Clear();
                if (cboUsuarios.Items.Count == 0 || cboContrato.Items.Count == 0) { return; }
                usuarioId = cboUsuarios.SelectedValue;
            }
            else
            {
                chklTabelasSuperiores.Items.Clear();
                if (cboSuperiores.Items.Count == 0 || cboContrato.Items.Count == 0) { return; }
                usuarioId = cboSuperiores.SelectedValue;
            }

            IList<ComissionamentoUsuario> _ulist = ComissionamentoUsuario.Carregar(cboContrato.SelectedValue, usuarioId, false);
            IList<Comissionamento> _clist = Comissionamento.CarregarPorContratoId(cboContrato.SelectedValue, Comissionamento.eTipo.PagoAoOperador);

            if (_clist == null) { return; }

            if (_ulist == null) { _ulist = new List<ComissionamentoUsuario>(); }
            foreach (Comissionamento com in _clist)
            {
                if (!Contem(_ulist, com))
                {
                    ComissionamentoUsuario _novo = new ComissionamentoUsuario();
                    _novo.TabelaComissionamentoNome = com.Descricao;
                    _novo.TabelaComissionamentoID = com.ID;
                    _novo.TabelaComissionamentoCategoriaNome = com.CategoriaNome;
                    _ulist.Add(_novo);
                }
            }

            String orderBy = "TabelaComissionamentoCategoriaNome";
            DynamicComparer<ComissionamentoUsuario> comparer = new DynamicComparer<ComissionamentoUsuario>(orderBy);
            ((List<ComissionamentoUsuario>)_ulist).Sort(comparer.Compare);

            if (!superior)
            {
                chklTabelas.DataValueField = "IDIndexado";
                chklTabelas.DataTextField = "Resumo";
                chklTabelas.DataSource = _ulist;
                chklTabelas.DataBind();
            }
            else
            {
                chklTabelasSuperiores.DataValueField = "IDIndexado";
                chklTabelasSuperiores.DataTextField = "Resumo";
                chklTabelasSuperiores.DataSource = _ulist;
                chklTabelasSuperiores.DataBind();
            }
        }

        Boolean Contem(IList<ComissionamentoUsuario> _ulist, Comissionamento com)
        {
            foreach (ComissionamentoUsuario comU in _ulist)
            {
                if (Convert.ToString(comU.TabelaComissionamentoID) == Convert.ToString(com.ID))
                    return true;
            }

            return false;
        }

        protected void chklTabelas_DataBound(Object sender, EventArgs e)
        {
            RadioButtonList _ctrl = (RadioButtonList)sender;
            IList<ComissionamentoUsuario> _list = (IList<ComissionamentoUsuario>)_ctrl.DataSource;

            if (_list == null || _list.Count == 0) { return; }

            for (int i = 0; i < _list.Count; i++)
                _ctrl.Items[i].Selected = _list[i].UsuarioID != null;
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (cboFiliais.Items.Count == 0 || cboUsuarios.Items.Count == 0)
            {
                base.Alerta(null, this, "_err0", "Não há agentes selecionados.");
                return;
            }

            if (chklTabelas.Items.Count > 0)
            {
                String[] arr = null;
                for (int i = 0; i < chklTabelas.Items.Count; i++)
                {
                    arr = chklTabelas.Items[i].Value.Split('|');
                    if (chklTabelas.Items[i].Selected && arr[0] == "_")
                    {
                        //insere nova
                        ComissionamentoUsuario obj = new ComissionamentoUsuario();
                        obj.TabelaComissionamentoID = arr[1];
                        obj.UsuarioID = cboUsuarios.SelectedValue;
                        obj.Salvar();
                    }
                    else if (!chklTabelas.Items[i].Selected && arr[0] != "_")
                    {
                        //remove a associacao
                        ComissionamentoUsuario obj = new ComissionamentoUsuario();
                        obj.ID = arr[0];
                        obj.Remover();
                    }
                }
            }

            this.CarregaTabelas(false);
            this.CarregaTabelas(true);
            base.Alerta(null, this, "_ok", "Dados alterados com sucesso.");
        }

        protected void cmdSalvarSuperior_Click(Object sender, EventArgs e)
        {
            if (cboFiliais.Items.Count == 0 || cboSuperiores.Items.Count == 0)
            {
                base.Alerta(null, this, "_err0", "Não há agentes selecionados.");
                return;
            }

            if (chklTabelasSuperiores.Items.Count > 0)
            {
                String[] arr = null;
                for (int i = 0; i < chklTabelasSuperiores.Items.Count; i++)
                {
                    arr = chklTabelasSuperiores.Items[i].Value.Split('|');
                    if (chklTabelasSuperiores.Items[i].Selected && arr[0] == "_")
                    {
                        //insere nova
                        ComissionamentoUsuario obj = new ComissionamentoUsuario();
                        obj.TabelaComissionamentoID = arr[1];
                        obj.UsuarioID = cboSuperiores.SelectedValue;
                        obj.Salvar();
                    }
                    else if (!chklTabelasSuperiores.Items[i].Selected && arr[0] != "_")
                    {
                        //remove a associacao
                        ComissionamentoUsuario obj = new ComissionamentoUsuario();
                        obj.ID = arr[0];
                        obj.Remover();
                    }
                }
            }

            this.CarregaTabelas(false);
            this.CarregaTabelas(true);
            base.Alerta(null, this, "_ok", "Dados alterados com sucesso.");
        }
    }
}
namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class plano : PageBase
    {
/*
        IList<PlanoParentescoAgregado> Itens
        {
            get { return ViewState["_itens"] as IList<PlanoParentescoAgregado>; }
            set { ViewState["_itens"] = value; }
        }

        IList<PlanoParentescoAgregado> Dependentes
        {
            get { return ViewState["_itensDep"] as IList<PlanoParentescoAgregado>; }
            set { ViewState["_itensDep"] = value; }
        }
*/
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);

                if (base.IDKeyParameterInProcess(ViewState, "_plano"))
                {
                    this.CarregaPlano();
                    this.CarregaParentescosPermitidos();
                    this.CarregaDependentesPermitidos();
                }

                this.CarregaAgregados();
                this.CarregaDependentes();
            }
        }

        void CarregaAgregados()
        {
            base.ExibirParentescos(cboParentesco, ViewState[IDKey], false, Parentesco.eTipo.Agregado);
        }

        void CarregaDependentes()
        {
            base.ExibirParentescos(cboDependente, ViewState[IDKey], false, Parentesco.eTipo.Dependente);
        }

        void CarregaContratos(String operadoraId)
        {
            if (String.IsNullOrEmpty(operadoraId))
            {
                cboContrato.Items.Clear();
                return;
            }
            cboContrato.DataValueField = "ID";
            cboContrato.DataTextField = "Descricao";
            cboContrato.DataSource = ContratoADM.Carregar(operadoraId);
            cboContrato.DataBind();
        }

        void CarregaPlano()
        {
            Plano plano = new Plano();
            plano.ID = ViewState[IDKey];
            plano.Carregar();

            txtNome.Text = plano.Descricao;
            txtCodigo.Text = plano.Codigo;
            txtSubPlano.Text = plano.SubPlano;
            chkAtivo.Checked = plano.Ativo;

            IList<Operadora> operadora = Operadora.CarregarPorContratoADM_ID(plano.ContratoID);
            if (operadora != null)
            {
                this.CarregaContratos(Convert.ToString(operadora[0].ID));
                cboOperadora.SelectedValue = Convert.ToString(operadora[0].ID);
                cboContrato.SelectedValue = Convert.ToString(plano.ContratoID);
            }
        }

        void CarregaParentescosPermitidos()
        {
            //this.Itens = PlanoParentescoAgregado.Carregar(ViewState[IDKey], Parentesco.eTipo.Agregado);
            //gridPPA.DataSource = this.Itens;
            //gridPPA.DataBind();
            //spanParentescosPermitidos.Visible = this.Itens != null && this.Itens.Count > 0;
        }

        protected void cboOperadora_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (cboOperadora.Items.Count == 0)
            {
                cboContrato.Items.Clear();
                return;
            }

            this.CarregaContratos(cboOperadora.SelectedValue);
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/planos.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (cboContrato.Items.Count ==0)
            {
                base.Alerta(null, this, "_err1", "Não há um contrato selecionado.");
                cboContrato.Focus();
                return;
            }

            if (txtNome.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err0", "Informe o campo Nome.");
                txtNome.Focus();
                return;
            }

            Plano plano = new Plano();
            plano.ID = ViewState[IDKey];
            plano.Descricao = txtNome.Text;
            plano.Codigo = txtCodigo.Text;
            plano.SubPlano = txtSubPlano.Text;
            plano.Ativo = chkAtivo.Checked;
            plano.ContratoID = cboContrato.SelectedValue;
            plano.Salvar();

            ViewState[IDKey] = plano.ID;

            base.Alerta(null, this, "__ok", "Dados salvos com sucesso.");
        }

        protected void cmdAdd_Click(Object sender, EventArgs e)
        {
            //if (cboParentesco.Items.Count == 0) { return; }
            //IList<PlanoParentescoAgregado> lista = this.Itens;
            //if (lista == null) { lista = new List<PlanoParentescoAgregado>(); }
            //else
            //{
            //    foreach (PlanoParentescoAgregado _ppa in lista)
            //    {
            //        if (_ppa.ParentescoDescricao.ToUpper() == cboParentesco.SelectedItem.Text.ToUpper())
            //        {
            //            base.Alerta(null, this, "_err1", "O parentesco escolhido já foi adicionado.");
            //            return;
            //        }
            //    }
            //}

            //PlanoParentescoAgregado ppa = new PlanoParentescoAgregado();
            //ppa.PlanoID = ViewState[IDKey];
            //ppa.ParentescoDescricao = cboParentesco.SelectedItem.Text;

            //if (ViewState[IDKey] != null)
            //{
            //    ppa.ParentescoId = cboParentesco.SelectedValue;
            //    ppa.Salvar();
            //}

            //lista.Add(ppa);
            //this.Itens = lista;

            //gridPPA.DataSource = lista;
            //gridPPA.DataBind();
            //spanParentescosPermitidos.Visible = true;
        }

        protected void gridPPA_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            //if (e.CommandName.Equals("excluir"))
            //{
            //    Object id = gridPPA.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
            //    if (id == null)
            //    {
            //        IList<PlanoParentescoAgregado> lista = this.Itens;
            //        lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
            //        this.Itens = lista;
            //        gridPPA.DataSource = lista;
            //        gridPPA.DataBind();
            //    }
            //    else
            //    {
            //        PlanoParentescoAgregado ppa = new PlanoParentescoAgregado();
            //        ppa.ID = id;
            //        ppa.Remover();
            //        this.CarregaParentescosPermitidos();
            //        CarregaAgregados();// base.ExibirParentescos(cboParentesco, ViewState[IDKey], false);
            //    }
            //}
        }

        protected void gridPPA_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja realmente excluir?");
        }

        #region Dependentes 

        void CarregaDependentesPermitidos()
        {
        //    this.Dependentes = PlanoParentescoAgregado.Carregar(ViewState[IDKey], Parentesco.eTipo.Dependente);
        //    gridDependentes.DataSource = this.Dependentes;
        //    gridDependentes.DataBind();
        //    spanDependentesPermitidos.Visible = this.Dependentes != null && this.Dependentes.Count > 0;
        }

        protected void cmdAddDependente_Click(Object sender, EventArgs e)
        {
        //    if (cboDependente.Items.Count == 0) { return; }
        //    IList<PlanoParentescoAgregado> lista = this.Dependentes;
        //    if (lista == null) { lista = new List<PlanoParentescoAgregado>(); }
        //    else
        //    {
        //        foreach (PlanoParentescoAgregado _ppa in lista)
        //        {
        //            if (_ppa.ParentescoDescricao.ToUpper() == cboParentesco.SelectedItem.Text.ToUpper())
        //            {
        //                base.Alerta(null, this, "_err1a", "O dependente escolhido já foi adicionado.");
        //                return;
        //            }
        //        }
        //    }

        //    PlanoParentescoAgregado ppa = new PlanoParentescoAgregado();
        //    ppa.PlanoID = ViewState[IDKey];
        //    ppa.ParentescoDescricao = cboDependente.SelectedItem.Text;

        //    if (ViewState[IDKey] != null)
        //    {
        //        ppa.ParentescoId = cboDependente.SelectedValue;
        //        ppa.Salvar();
        //    }

        //    lista.Add(ppa);
        //    this.Dependentes = lista;

        //    gridDependentes.DataSource = lista;
        //    gridDependentes.DataBind();
        //    spanDependentesPermitidos.Visible = true;
        }

        protected void gridDependentes_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
        //    if (e.CommandName.Equals("excluir"))
        //    {
        //        Object id = gridDependentes.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
        //        if (id == null)
        //        {
        //            IList<PlanoParentescoAgregado> lista = this.Dependentes;
        //            lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
        //            this.Dependentes = lista;
        //            gridDependentes.DataSource = lista;
        //            gridDependentes.DataBind();
        //        }
        //        else
        //        {
        //            PlanoParentescoAgregado ppa = new PlanoParentescoAgregado();
        //            ppa.ID = id;
        //            ppa.Remover();
        //            this.CarregaDependentes();
        //            CarregaDependentesPermitidos();
        //        }
        //    }
        }

        protected void gridDependentes_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
        //    base.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja realmente excluir?");
        }

        #endregion
    }
}

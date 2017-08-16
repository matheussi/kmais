namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class tabelaV : PageBase
    {
        //List<TabelaValorItem> Itens
        //{
        //    get
        //    {
        //        if (ViewState["_itens"] != null)
        //        {
        //            List<TabelaValorItem> lista = ViewState["_itens"] as List<TabelaValorItem>;

        //            for (int i = 0; i < gridItens.Rows.Count; i++)
        //            {
        //                lista[i].IdadeFim = base.CToInt(((TextBox)gridItens.Rows[i].Cells[1].Controls[1]).Text);
        //                lista[i].IdadeInicio = base.CToInt(((TextBox)gridItens.Rows[i].Cells[0].Controls[1]).Text);

        //                lista[i].QCValor = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[4].Controls[1]).Text);//.Replace(",", ".")
        //                lista[i].QPValor = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[5].Controls[1]).Text);

        //                lista[i].QCValorPagamento = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[2].Controls[1]).Text);
        //                lista[i].QPValorPagamento = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[3].Controls[1]).Text);

        //                //lista[i].QCValorMigracao = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[6].Controls[1]).Text);//.Replace(",", ".")
        //                //lista[i].QPValorMigracao = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[7].Controls[1]).Text);

        //                //lista[i].QCValorCondicaoEspecial = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[8].Controls[1]).Text);//.Replace(",", ".")
        //                //lista[i].QPValorCondicaoEspecial = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[9].Controls[1]).Text);

        //                lista[i].ID = gridItens.DataKeys[i].Value;
        //                lista[i].TabelaID = ViewState[IDKey];
        //            }

        //            return lista;
        //        }
        //        else
        //            return null;
        //    }
        //    set { ViewState["_itens"] = value; }
        //}

        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //    if (!IsPostBack)
        //    {
        //        this.CarregaOperadoras();
        //        this.CarregaContratos();
        //        this.CarregaCategorias();
        //        this.PreencheGridDePlanos();

        //        if (base.IDKeyParameterInProcess(ViewState, "_tabVal"))
        //        {
        //            this.CarregaTabela();
        //        }
        //        else
        //        {
        //            this.MontaGridParaPrimeiraInsercao();
        //        }
        //    }
        //}

        //void CarregaTabela()
        //{
        //    TabelaValor tabela = new TabelaValor();
        //    tabela.ID = ViewState[IDKey];
        //    tabela.Carregar();

        //    //txtNome.Text = tabela.Descricao;
        //    if (tabela.Data != DateTime.MinValue) { txtData.Text = tabela.Data.ToString("dd/MM/yyyy"); }
        //    cboCategoria.SelectedValue = Convert.ToString(tabela.CategoriaID);

        //    //Categoria plano = new Plano(tabela.PlanoID);
        //    //plano.Carregar();

        //    //IList<Operadora> listaoperadora = Operadora.CarregarPorContratoADM_ID(plano.ContratoID);

        //    //if (listaoperadora != null && listaoperadora.Count > 0)
        //    //{
        //    //    cboOperadora.SelectedValue = Convert.ToString(listaoperadora[0].ID);
        //    //    cboOperadora_SelectedIndexChanged(cboOperadora, null);
        //    //    //cboContrato.SelectedValue = Convert.ToString(plano.ContratoID);
        //    //    cboContrato_SelectedIndexChanged(cboContrato, null);
        //    //    cboCategoria.SelectedValue = Convert.ToString(tabela.CategoriaID);
        //    //}

        //    //this.CarregaItensDaTabela();
        //}

        //void CarregaItensDaTabela()
        //{
        //    Object planoId = gridPlanos.DataKeys[gridPlanos.SelectedIndex].Value;
        //    List<TabelaValorItem> lista = 
        //        (List<TabelaValorItem>)TabelaValorItem.CarregarPorTabela(ViewState[IDKey], planoId);

        //    gridItens.DataSource = lista;
        //    gridItens.DataBind();
        //    this.Itens = lista;
        //    pnl2.Visible = true;
        //}

        //void MontaGridParaPrimeiraInsercao()
        //{
        //    List<TabelaValorItem> lista = new List<TabelaValorItem>();
        //    for (int i = 1; i <= 12; i++)
        //    {
        //        lista.Add(new TabelaValorItem());
        //    }

        //    gridItens.DataSource = lista;
        //    gridItens.DataBind();
        //    this.Itens = lista;
        //}

        //void CarregaOperadoras()
        //{
        //    cboOperadora.DataTextField = "Nome";
        //    cboOperadora.DataValueField = "ID";
        //    cboOperadora.DataSource = Operadora.CarregarTodas();
        //    cboOperadora.DataBind();
        //}

        //void CarregaContratos()
        //{
        //    cboContrato.Items.Clear();
        //    if (cboOperadora.Items.Count == 0) { return; }
        //    cboContrato.DataValueField = "ID";
        //    cboContrato.DataTextField = "Descricao";
        //    cboContrato.DataSource = ContratoADM.Carregar(cboOperadora.SelectedValue);
        //    cboContrato.DataBind();
        //}

        //void CarregaCategorias()
        //{
        //    base.ExibirCategorias(cboCategoria, true, false);
        //}

        //void PreencheGridDePlanos()
        //{
        //    if (base.HaItemSelecionado(cboContrato))
        //    {
        //        gridPlanos.DataSource = Plano.CarregarPorContratoID(cboContrato.SelectedValue, true);
        //        pnl.Visible = true;
        //    }
        //    else
        //    {
        //        gridPlanos.DataSource = null;
        //        pnl.Visible = false;
        //    }

        //    gridPlanos.DataBind();
        //}

        //protected void gridPlanos_RowCommand(Object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName == "detalhe")
        //    {
        //        //Object id = gridPlanos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
        //        gridPlanos.SelectedIndex = Convert.ToInt32(e.CommandArgument);
        //        this.CarregaItensDaTabela();
        //    }
        //}

        //protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        //{
        //    gridPlanos.SelectedIndex = -1;
        //    pnl2.Visible = false;
        //    this.CarregaContratos();
        //    this.PreencheGridDePlanos();
        //}

        //protected void cboContrato_SelectedIndexChanged(Object sender, EventArgs e)
        //{
        //    gridPlanos.SelectedIndex = -1;
        //    pnl2.Visible = false;
        //    this.PreencheGridDePlanos();
        //}

        //protected void cmdVoltar_Click(Object sender, EventArgs e)
        //{
        //    Response.Redirect("~/tabelasV.aspx");
        //}

        //protected void cmdSalvar_Click(Object sender, EventArgs e)
        //{
        //    if (!this.IsValid) { return; }

        //    #region validacoes 

        //    if (cboCategoria.Items.Count == 0)
        //    {
        //        base.Alerta(null, this, "_err1", "Não há uma categoria selecionada.");
        //        return;
        //    }

        //    //if (txtNome.Text.Trim() == "")
        //    //{
        //    //    base.Alerta(null, this, "_err2", "Informe a descrição.");
        //    //    txtNome.Focus();
        //    //    return;
        //    //}

        //    if (txtData.Text.Trim() != "")
        //    {
        //        DateTime dt = new DateTime();
        //        Boolean resultado = UIHelper.TyParseToDateTime(txtData.Text, out dt);
        //        if (!resultado)
        //        {
        //            base.Alerta(null, this, "_err3", "Informe uma data válida.");
        //            txtData.Focus();
        //            return;
        //        }
        //    }

        //    String msg = UIHelper.ChecaGridIntervaloDeIdades(gridItens, 0, 1);
        //    if (!String.IsNullOrEmpty(msg))
        //    {
        //        base.Alerta(null, this, "_err4", msg);
        //        return;
        //    }
        //    #endregion

        //    if (txtData.Text.Trim() == "") { txtData.Text = DateTime.Now.ToString("dd/MM/yyyy"); }

        //    //TODO: transacionar
        //    TabelaValor tabela = new TabelaValor();
        //    tabela.ID = ViewState[IDKey];
        //    //tabela.Descricao = txtNome.Text;
        //    if(txtData.Text.Trim() != "")
        //        tabela.Data = Convert.ToDateTime(txtData.Text);
        //    tabela.CategoriaID = cboCategoria.SelectedValue;
        //    tabela.Salvar();
        //    ViewState[IDKey] = tabela.ID;

        //    List<TabelaValorItem> lista = this.Itens;
        //    foreach (TabelaValorItem item in lista)
        //    {
        //        item.TabelaID = tabela.ID;
        //        item.PlanoID = gridPlanos.DataKeys[gridPlanos.SelectedIndex].Value;
        //        item.Salvar();
        //    }

        //    this.Itens = lista;
        //    gridItens.DataSource = lista;
        //    gridItens.DataBind();

        //    base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
        //}

        //protected void cmdAddItem_Click(Object sender, EventArgs e)
        //{
        //    List<TabelaValorItem> lista = this.Itens;
        //    if (lista == null) { lista = new List<TabelaValorItem>(); }

        //    lista.Add(new TabelaValorItem());

        //    gridItens.DataSource = lista;
        //    gridItens.DataBind();
        //    this.Itens = lista;
        //}

        //protected void gridItens_RowDataBound(Object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        base.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente excluir a linha?");

        //        TextBox txtvalor1 = (TextBox)e.Row.Cells[2].Controls[1];
        //        txtvalor1.Attributes.Add("onKeyUp", "mascara('" + txtvalor1.ClientID + "')");
        //        TextBox txtvalor2 = (TextBox)e.Row.Cells[3].Controls[1];
        //        txtvalor2.Attributes.Add("onKeyUp", "mascara('" + txtvalor2.ClientID + "')");

        //        ((TextBox)e.Row.Cells[4].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[4].Controls[1]).ClientID + "')");
        //        ((TextBox)e.Row.Cells[5].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[5].Controls[1]).ClientID + "')");
        //        //((TextBox)e.Row.Cells[6].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[6].Controls[1]).ClientID + "')");
        //        //((TextBox)e.Row.Cells[7].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[7].Controls[1]).ClientID + "')");
        //        //((TextBox)e.Row.Cells[8].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[8].Controls[1]).ClientID + "')");
        //        //((TextBox)e.Row.Cells[9].Controls[1]).Attributes.Add("onKeyUp", "mascara('" + ((TextBox)e.Row.Cells[9].Controls[1]).ClientID + "')");

        //        Object id = gridItens.DataKeys[e.Row.RowIndex].Value;

        //        if (id == null)
        //        {
        //            //se a linha nao está salva (nao tem id), seta para "" onde for 0
        //            TextBox txtidade1 = (TextBox)e.Row.Cells[0].Controls[1];
        //            if (txtidade1.Text == "0") { txtidade1.Text = ""; }

        //            TextBox txtidade2 = (TextBox)e.Row.Cells[1].Controls[1];
        //            if (txtidade2.Text == "0") { txtidade2.Text = ""; }

        //            if (CToDecimal(txtvalor2.Text) == 0) { txtvalor2.Text = ""; }
        //        }

        //        if (CToDecimal(((TextBox)e.Row.Cells[4].Controls[1]).Text) == 0) { ((TextBox)e.Row.Cells[4].Controls[1]).Text = ""; }
        //        if (CToDecimal(((TextBox)e.Row.Cells[5].Controls[1]).Text) == 0) { ((TextBox)e.Row.Cells[5].Controls[1]).Text = ""; }
        //        //if (CToDecimal(((TextBox)e.Row.Cells[6].Controls[1]).Text) == 0) { ((TextBox)e.Row.Cells[6].Controls[1]).Text = ""; }
        //        //if (CToDecimal(((TextBox)e.Row.Cells[7].Controls[1]).Text) == 0) { ((TextBox)e.Row.Cells[7].Controls[1]).Text = ""; }
        //        //if (CToDecimal(((TextBox)e.Row.Cells[8].Controls[1]).Text) == 0) { ((TextBox)e.Row.Cells[8].Controls[1]).Text = ""; }
        //        //if (CToDecimal(((TextBox)e.Row.Cells[9].Controls[1]).Text) == 0) { ((TextBox)e.Row.Cells[9].Controls[1]).Text = ""; }
        //    }
        //}

        //protected void gridItens_RowCommand(Object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName == "excluir")
        //    {
        //        Object id = gridItens.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
        //        if (id == null)
        //        {
        //            List<TabelaValorItem> lista = this.Itens;
        //            lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
        //            this.Itens = lista;
        //            gridItens.DataSource = lista;
        //            gridItens.DataBind();
        //        }
        //        else
        //        {
        //            TabelaValorItem item = new TabelaValorItem();
        //            item.ID = id;
        //            item.Carregar();
        //            item.Remover();
        //            this.CarregaItensDaTabela();
        //        }
        //    }
        //}
    }
}
namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class tabelaR : PageBase
    {
        List<TabelaReajusteItem> Itens
        {
            get
            {
                if (ViewState["_itens"] != null)
                {
                    List<TabelaReajusteItem> lista = ViewState["_itens"] as List<TabelaReajusteItem>;

                    for (int i = 0; i < gridItens.Rows.Count; i++)
                    {
                        lista[i].IdadeInicio = base.CToInt(((TextBox)gridItens.Rows[i].Cells[0].Controls[1]).Text);
                        lista[i].PercentualReajuste = CToDecimal(((TextBox)gridItens.Rows[i].Cells[1].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].ID = gridItens.DataKeys[i].Value;
                        lista[i].TabelaID = ViewState[IDKey];
                    }

                    return lista;
                }
                else
                    return null;
            }
            set { ViewState["_itens"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.CarregaOperadoras();
                this.CarregaContratos();

                if (base.IDKeyParameterInProcess(ViewState, "_tabReaj"))
                {
                    this.CarregaTabela();
                }
                else
                {
                    this.MontaGridParaPrimeiraInsercao();
                }
            }
        }

        void CarregaTabela()
        {
            TabelaReajuste tabela = new TabelaReajuste();
            tabela.ID = ViewState[IDKey];
            tabela.Carregar();

            txtNome.Text = tabela.Descricao;
            if (tabela.Data != DateTime.MinValue) { txtData.Text = tabela.Data.ToString("dd/MM/yyyy"); }

            IList<Operadora> listaoperadora = Operadora.CarregarPorContratoADM_ID(tabela.ContratoID);

            if (listaoperadora != null && listaoperadora.Count > 0)
            {
                cboOperadora.SelectedValue = Convert.ToString(listaoperadora[0].ID);
                cboOperadora_OnSelectedIndexChanged(null, null);
                cboContrato.SelectedValue = Convert.ToString(tabela.ContratoID);
            }

            this.CarregaItensDaTabela();
        }

        void CarregaItensDaTabela()
        {
            List<TabelaReajusteItem> lista =
                (List<TabelaReajusteItem>)TabelaReajusteItem.CarregarPorTabela(ViewState[IDKey]);

            gridItens.DataSource = lista;
            gridItens.DataBind();
            this.Itens = lista;
        }

        void CarregaOperadoras()
        {
            cboOperadora.DataTextField = "Nome";
            cboOperadora.DataValueField = "ID";
            cboOperadora.DataSource = Operadora.CarregarTodas(true);
            cboOperadora.DataBind();
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

        protected void cboOperadora_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
        }

        void MontaGridParaPrimeiraInsercao()
        {
            List<TabelaReajusteItem> lista = new List<TabelaReajusteItem>();
            for (int i = 1; i <= 9; i++)
            {
                lista.Add(new TabelaReajusteItem());
            }

            gridItens.DataSource = lista;
            gridItens.DataBind();
            this.Itens = lista;
        }

        protected void gridItens_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "excluir")
            {
                Object id = gridItens.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                if (id == null)
                {
                    List<TabelaReajusteItem> lista = this.Itens;
                    lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                    this.Itens = lista;
                    gridItens.DataSource = lista;
                    gridItens.DataBind();
                }
                else
                {
                    TabelaReajusteItem item = new TabelaReajusteItem();
                    item.ID = id;
                    item.Carregar();
                    item.Remover();
                    this.CarregaItensDaTabela();
                }
            }
        }

        protected void gridItens_RowCreated(Object sender, GridViewRowEventArgs e)
        {
        }

        protected void gridItens_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente excluir a linha?");

                TextBox txtvalor1 = (TextBox)e.Row.Cells[1].Controls[1];
                txtvalor1.Attributes.Add("onKeyUp", "mascara('" + txtvalor1.ClientID + "')");

                Object id = gridItens.DataKeys[e.Row.RowIndex].Value;

                if (id == null)
                {
                    //se a linha nao está salva (nao tem id), seta para "" onde for 0
                    TextBox txtidade1 = (TextBox)e.Row.Cells[0].Controls[1];
                    if (txtidade1.Text == "0") { txtidade1.Text = ""; }

                    if (CToDecimal(txtvalor1.Text) == 0) { txtvalor1.Text = ""; }
                }
            }
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/tabelasR.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (!this.IsValid) { return; }

            #region validacoes

            if (cboContrato.Items.Count == 0)
            {
                base.Alerta(null, this, "_err1", "Não há um contrato selecionado.");
                return;
            }

            if (txtNome.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err2", "Informe a descrição.");
                txtNome.Focus();
                return;
            }

            if (txtData.Text.Trim() != "")
            {
                DateTime dt = new DateTime();
                Boolean resultado = UIHelper.TyParseToDateTime(txtData.Text, out dt);
                if (!resultado)
                {
                    base.Alerta(null, this, "_err3", "Informe uma data válida.");
                    txtData.Focus();
                    return;
                }
            }
            else
                txtData.Text = DateTime.Now.ToString("dd/MM/yyyy");
            #endregion
            

            TabelaReajuste tabela = new TabelaReajuste();
            tabela.ID = ViewState[IDKey];
            tabela.Descricao = txtNome.Text;

            if (txtData.Text.Trim() != "") { tabela.Data = Convert.ToDateTime(txtData.Text); }

            tabela.ContratoID = cboContrato.SelectedValue;
            tabela.Salvar();
            ViewState[IDKey] = tabela.ID;

            List<TabelaReajusteItem> lista = this.Itens;
            foreach (TabelaReajusteItem item in lista)
            {
                item.Salvar();
            }

            this.Itens = lista;
            gridItens.DataSource = lista;
            gridItens.DataBind();
            base.Alerta(null, this, "__ok", "Dados salvos com sucesso.");
        }

        protected void cmdAddItem_Click(Object sender, EventArgs e)
        {
            List<TabelaReajusteItem> lista = this.Itens;
            if (lista == null) { lista = new List<TabelaReajusteItem>(); }

            lista.Add(new TabelaReajusteItem());

            gridItens.DataSource = lista;
            gridItens.DataBind();
            this.Itens = lista;
        }
    }
}

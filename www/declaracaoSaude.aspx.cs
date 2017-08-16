namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class declaracaoSaude : PageBase
    {
        IList<ItemDeclaracaoSaude> pegaItens(Boolean pegaValorDoGrid)
        {
            if (ViewState["_itens"] != null)
            {
                List<ItemDeclaracaoSaude> lista = ViewState["_itens"] as List<ItemDeclaracaoSaude>;

                if (pegaValorDoGrid)
                {
                    for (int i = 0; i < gridItens.Rows.Count; i++)
                    {
                        if (gridItens.Rows[i].Cells[1].Controls[1] is Label)
                            lista[i].Ordem = base.CToInt(((Label)gridItens.Rows[i].Cells[1].Controls[1]).Text);
                        else
                            lista[i].Ordem = base.CToInt(((TextBox)gridItens.Rows[i].Cells[1].Controls[1]).Text);

                        lista[i].ID = gridItens.DataKeys[i].Value;

                        if (gridItens.Rows[i].Cells[2].Controls[1] is Label)
                            lista[i].Codigo = ((Label)gridItens.Rows[i].Cells[2].Controls[1]).Text;
                        else
                            lista[i].Codigo = ((TextBox)gridItens.Rows[i].Cells[2].Controls[1]).Text;

                        if (gridItens.Rows[i].Cells[3].Controls[1] is Label)
                            lista[i].Texto = ((Label)gridItens.Rows[i].Cells[3].Controls[1]).Text;
                        else
                            lista[i].Texto = ((TextBox)gridItens.Rows[i].Cells[3].Controls[1]).Text;
                    }
                }

                return lista;
            }
            else
                return null;
        }

        void setaItens(IList<ItemDeclaracaoSaude> Itens)
        {
            ViewState["_itens"] = Itens;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                tblItens.Visible = false;
                base.ExibirOperadoras(cboOperadora, true);
            }
        }

        void CarregaItens()
        {
            if (cboOperadora.SelectedIndex > 0)
            {
                tblItens.Visible = true;
                IList<ItemDeclaracaoSaude> lista = ItemDeclaracaoSaude.Carregar(cboOperadora.SelectedValue);
                gridItens.EditIndex = -1;
                gridItens.DataSource = lista;
                gridItens.DataBind();

                setaItens(lista);

                //tblItens.Visible = gridItens.Rows.Count > 0;
                if (lista == null || lista.Count == 0)
                    cmdAddItem_Click(cmdAddItem, null);
            }
            else //limpa
            {
                tblItens.Visible = false;
                gridItens.DataSource = null;
                gridItens.DataBind();
            }
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaItens();
        }

        protected void gridItens_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Update"))
            {
                int indice = Convert.ToInt32(e.CommandArgument);
                IList<ItemDeclaracaoSaude> items = (IList<ItemDeclaracaoSaude>)ViewState["_itens"];
                items[indice].Ativo = true;
                items[indice].Codigo = ((TextBox)gridItens.Rows[Convert.ToInt32(e.CommandArgument)].Cells[2].Controls[1]).Text;
                items[indice].OperadoraID = cboOperadora.SelectedValue;
                items[indice].Ordem = base.CToInt(((TextBox)gridItens.Rows[Convert.ToInt32(e.CommandArgument)].Cells[1].Controls[1]).Text);
                items[indice].Texto = ((TextBox)gridItens.Rows[Convert.ToInt32(e.CommandArgument)].Cells[3].Controls[1]).Text;
                items[indice].Salvar();

                ViewState["_itens"] = items;
                gridItens.DataSource = items;
                gridItens.EditIndex = items.Count - 1;
                gridItens.DataBind();
                setaItens(items);

            }
            else if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    //TODO: Atencao ao excluir. Pode estar em uso e nesse caso só desativar
                    Object id = gridItens.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                    if (id != null)
                    {
                        ItemDeclaracaoSaude item = new ItemDeclaracaoSaude();
                        item.ID = id;
                        item.Remover();
                    }

                    IList<ItemDeclaracaoSaude> lista = pegaItens(false);
                    lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                    setaItens(lista);
                    gridItens.DataSource = lista;
                    gridItens.DataBind();
                }
                catch
                {
                    base.Alerta(MPE, ref litAlert, "Não foi possível excluir.", upnlAlerta);
                }
            }
        }

        protected void gridItens_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente excluir a questão?");
        }

        protected void cmdAddItem_Click(Object sender, EventArgs e)
        {
            IList<ItemDeclaracaoSaude> lista = pegaItens(false);
            if (lista == null) { lista = new List<ItemDeclaracaoSaude>(); }

            lista.Add(new ItemDeclaracaoSaude());

            gridItens.DataSource = lista;
            gridItens.EditIndex = lista.Count - 1;
            gridItens.DataBind();
            setaItens(lista);
        }

        protected void gridItens_RowEditing(Object sender, GridViewEditEventArgs e)
        {
            gridItens.EditIndex = e.NewEditIndex;
            gridItens.DataSource = pegaItens(false);
            gridItens.DataBind();
        }

        protected void gridItens_RowUpdating(Object sender, GridViewUpdateEventArgs e)
        {
            //ItemDeclaracaoSaude item = new ItemDeclaracaoSaude();
            //item.ID = gridItens.DataKeys[e.RowIndex].Value;
            //item.OperadoraID = cboOperadora.SelectedValue;
            //item.Ordem = base.CToInt(((TextBox)gridItens.Rows[e.RowIndex].Cells[1].Controls[1]).Text);
            //item.Texto = Convert.ToString(((TextBox)gridItens.Rows[e.RowIndex].Cells[2].Controls[1]).Text);

            IList<ItemDeclaracaoSaude> lista = pegaItens(true);

            ItemDeclaracaoSaude item = lista[e.RowIndex];
            item.OperadoraID = cboOperadora.SelectedValue;
            item.ID = gridItens.DataKeys[e.RowIndex].Value;

            if (ItemDeclaracaoSaude.Duplicado(item))
            {
                base.Alerta(null, this, "_errDupl", "Questão já existe para essa operadora.");
                return;
            }

            item.Salvar();

            setaItens(lista);
            gridItens.EditIndex = -1;
            gridItens.DataSource = lista;
            gridItens.DataBind();

            base.Alerta(null, this, "_ok", "Questão salva com sucesso.");
        }

        protected void gridItens_RowCancelingEdit(Object sender, GridViewCancelEditEventArgs e)
        {
            gridItens.EditIndex = -1;
            gridItens.DataSource = pegaItens(false);
            gridItens.DataBind();
        }
    }
}
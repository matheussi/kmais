namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class layoutArquivo : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
 	         base.OnLoad(e);
             txtTamanho.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");

             if (!IsPostBack)
             {
                 base.ExibirOperadoras(cboOperadora, false);
                 base.ExibirTiposDeArquivo(cboTipo, true);
                 base.ExibirFormatosDeArquivo(cboFormato);

                 base.ExibirSecoes(cboSecao);
                 base.ExibirTiposValor(cboTipoValor);
                 base.ExibirTiposDado(cboTipoDado);
                 base.ExibirFormatos(cboFormatoItem);
                 base.ExibirBehaviors(cboBehavior);
                 base.ExibirTiposPreenchimento(cboTipoPreenchimento);
                 base.ExibirCampos(cboValor);

                 if (base.IDKeyParameterInProcess(ViewState, "_layoutArquivo"))
                 {
                     this.CarregarArquivo();
                 }
             }
        }

        void CarregarArquivo()
        {
            LayoutArquivoCustomizado arquivo = new LayoutArquivoCustomizado(ViewState[IDKey]);
            arquivo.Carregar();

            cboOperadora.SelectedValue = Convert.ToString(arquivo.OperadoraID);
            txtNomeArquivo.Text = arquivo.Nome;
            cboTipo.SelectedValue = arquivo.Tipo.ToString();
            cboFormato.SelectedValue = arquivo.Formato.ToString();
            txtDelimitador.Text = arquivo.Delimitador;

            ViewState["itens"] = ItemLayoutArquivoCustomizado.Carregar(arquivo.ID);
            this.ExibirItens();
        }

        void ExibirItens()
        {
            grid.DataSource = ViewState["itens"] as List<ItemLayoutArquivoCustomizado>;
            grid.DataBind();
        }

        protected void cboBehavior_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (((ItemLayoutArquivoCustomizado.eBehavior)Convert.ToInt32(cboBehavior.SelectedValue)) == ItemLayoutArquivoCustomizado.eBehavior.FonteDeDados)
            {
                txtValor.Visible = false;
                cboValor.Visible = true;
                txtValor.Text = "";
            }
            else if (((ItemLayoutArquivoCustomizado.eBehavior)Convert.ToInt32(cboBehavior.SelectedValue)) == ItemLayoutArquivoCustomizado.eBehavior.Literal)
            {
                txtValor.Visible = true;
                cboValor.Visible = false;
            }
            else
            {
                txtValor.Visible = false;
                cboValor.Visible = false;
                txtValor.Text = "";
            }
        }

        void PreencheItem(ref ItemLayoutArquivoCustomizado item)
        {
            item.Behavior = Convert.ToInt32(cboBehavior.SelectedValue);
            item.Formato = cboFormatoItem.SelectedValue;
            item.Secao = Convert.ToInt32(cboSecao.SelectedValue);
            if (txtTamanho.Text.Trim() == "") { txtTamanho.Text = "0"; }
            item.Tamanho = Convert.ToInt32(txtTamanho.Text);
            item.TipoDado = Convert.ToInt32(cboTipoDado.SelectedValue);
            item.TipoPreenchimento = Convert.ToInt32(cboTipoPreenchimento.SelectedValue);
            item.TipoValor = Convert.ToInt32(cboTipoValor.SelectedValue);
            if (cboValor.Visible)
            {
                item.Valor = cboValor.SelectedValue;
                item.ValorRotulo = cboValor.SelectedItem.Text;
            }
            else
            {
                item.Valor = null;
                item.ValorRotulo = null;
                item.Valor = txtValor.Text;
            }

            if (item.Behavior == Convert.ToInt32(ItemLayoutArquivoCustomizado.eBehavior.Contador))
            {
                item.ValorRotulo = "[Contador da Linha]";
            }
        }

        protected void cmdAdd_Click(Object sender, EventArgs e)
        {
            List<ItemLayoutArquivoCustomizado> itens = (List<ItemLayoutArquivoCustomizado>)ViewState["itens"];
            if (itens == null) { itens = new List<ItemLayoutArquivoCustomizado>(); }

            ItemLayoutArquivoCustomizado item = null;

            if (grid.SelectedIndex == -1)
                item = new ItemLayoutArquivoCustomizado();
            else
                item = itens[grid.SelectedIndex];

            this.PreencheItem(ref item);

            if (grid.SelectedIndex == -1)
                itens.Add(item);
            else
                itens[grid.SelectedIndex] = item;

            ViewState["itens"] = itens;
            grid.SelectedIndex = -1;
            cmdAdd.Text = "Adicionar";
            this.ExibirItens();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                List<ItemLayoutArquivoCustomizado> itens = (List<ItemLayoutArquivoCustomizado>)ViewState["itens"];
                if (itens == null) { return; }

                Int32 index = Convert.ToInt32(e.CommandArgument);
                grid.SelectedIndex = index;
                cmdAdd.Text = "Editar";

                ItemLayoutArquivoCustomizado item = itens[index];

                cboSecao.SelectedValue = item.Secao.ToString();
                cboTipoValor.SelectedValue = item.TipoValor.ToString();
                cboTipoDado.SelectedValue = item.TipoDado.ToString();

                if (cboFormatoItem.Items.FindByValue(item.Formato) != null)
                {
                    cboFormatoItem.SelectedValue = item.Formato;
                }
                cboBehavior.SelectedValue = item.Behavior.ToString();
                cboTipoPreenchimento.SelectedValue = item.TipoPreenchimento.ToString();
                txtTamanho.Text = item.Tamanho.ToString();

                cboBehavior_SelectedIndexChanged(null, null);

                cboValor.SelectedIndex = -1;
                if (((ItemLayoutArquivoCustomizado.eBehavior)Convert.ToInt32(cboBehavior.SelectedValue)) == ItemLayoutArquivoCustomizado.eBehavior.FonteDeDados)
                {
                    foreach (ListItem litem in cboValor.Items)
                    {
                        if (litem.Value.ToLower() == item.Valor.ToLower())
                        {
                            litem.Selected = true;
                            break;
                        }
                    }
                }
                else if (((ItemLayoutArquivoCustomizado.eBehavior)Convert.ToInt32(cboBehavior.SelectedValue)) == ItemLayoutArquivoCustomizado.eBehavior.Literal)
                    txtValor.Text = item.Valor;
            }
            else if (e.CommandName.Equals("excluir"))
            {
                List<ItemLayoutArquivoCustomizado> itens = (List<ItemLayoutArquivoCustomizado>)ViewState["itens"];
                if (itens == null) { return; }

                Int32 index = Convert.ToInt32(e.CommandArgument);
                Object id = grid.DataKeys[index].Value;

                ItemLayoutArquivoCustomizado item = itens[index];
                if (item.ID != null) { item.Remover(); }

                itens.RemoveAt(index);
                ViewState["itens"] = itens;
                grid.SelectedIndex = -1;
                this.ExibirItens();
            }
        }
        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 3, "Deseja realmente excluir o item?");
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/layoutsArquivos.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (!base.HaItemSelecionado(cboOperadora))
            {
                Alerta(null, this, "_erroOp", "Não há uma operadora selecionada.");
                return;
            }

            if (txtNomeArquivo.Text.Trim() == "")
            {
                Alerta(null, this, "_erroNmArq", "Informe o nome do arquivo.");
                return;
            }

            if (cboFormato.SelectedValue == "1" && txtDelimitador.Text.Trim() == "")
            {
                Alerta(null, this, "_erroDelim", "Informe o caractér do arquivo delimitado.");
                return;
            }

            if (ViewState["itens"] == null)
            {
                Alerta(null, this, "_erroItens", "Nenhum item adicionado ao layout.");
                return;
            }

            #endregion

            LayoutArquivoCustomizado arquivo = new LayoutArquivoCustomizado(ViewState[IDKey]);

            arquivo.Delimitador = txtDelimitador.Text;
            arquivo.Formato = Convert.ToInt32(cboFormato.SelectedValue);
            arquivo.Nome = txtNomeArquivo.Text;
            arquivo.OperadoraID = cboOperadora.SelectedValue;
            arquivo.Tipo = Convert.ToInt32(cboTipo.SelectedValue);

            if (chkNovo.Checked)
            {
                arquivo.ID = null;
                for (int i = 0; i < ((List<ItemLayoutArquivoCustomizado>)ViewState["itens"]).Count; i++)
                {
                    ((List<ItemLayoutArquivoCustomizado>)ViewState["itens"])[i].ID = null;
                }
            }

            arquivo.Salvar((List<ItemLayoutArquivoCustomizado>)ViewState["itens"]);
            ViewState["itens"] = ItemLayoutArquivoCustomizado.Carregar(arquivo.ID);

            ViewState[IDKey] = arquivo.ID;
            Alerta(null, this, "_ok", "Arquivo salvo com sucesso.");
        }
    }
}
namespace www.movimentacao
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;
    using System.Configuration;

    using LC.Web.PadraoSeguros.Entity;

    public partial class arquivoTransacionalUNIMED : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                base.ExibirTiposDeArquivo(cboTipoTransacao, false);
                cboTipoTransacao.Items.RemoveAt(0);
                base.ExibirTiposDeArquivo(cboTipoTransacaoGerar, false);
                cboTipoTransacaoGerar.Items.RemoveAt(0);

                this.CarregarItens();

                cboContratoAdm.DataValueField = "ID";
                cboContratoAdm.DataTextField = "Descricao";
                IList<ContratoADM> lista = ContratoADM.Carregar(Operadora.UnimedID);
                cboContratoAdm.DataSource = lista;
                cboContratoAdm.DataBind();
                cboContratoAdm.Items.Insert(0, new ListItem("selecione", "0"));
            }
        }

        Int32 CarregarItens()
        {
            IList<ItemAgendaArquivoUnimed> lista = ItemAgendaArquivoUnimed.CarregarTodos();
            grid.DataSource = lista;
            grid.DataBind();

            if (lista == null)
            {
                pnlAgenda.Visible = false;
                return 0;
            }
            else
            {
                pnlAgenda.Visible = true;
                return lista.Count;
            }
        }

        void CarregarBeneneficiarios()
        {
            gridBeneficiarios.SelectedIndex = -1;
            IList<ContratoBeneficiario> lista = ContratoBeneficiario.CarregarPorContratoNumero(txtProposta.Text, Operadora.UnimedID, true);
            gridBeneficiarios.DataSource = lista;
            gridBeneficiarios.DataBind();
            if (lista == null || lista.Count == 0) { pnlBeneficiarios.Visible = false; base.Alerta(null, this, "_errBenef", "Proposta não localizada."); return; }
            pnlBeneficiarios.Visible = true;
        }

        protected void gridBeneficiarios_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[3].Text.ToUpper() == "TRUE")
                    e.Row.Cells[3].Text = "Sim";
                else
                    e.Row.Cells[3].Text = "<font color='red'>Não</font>";
            }
        }

        protected void gridBeneficiarios_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selecionar"))
            {
                gridBeneficiarios.SelectedIndex = Convert.ToInt32(e.CommandArgument);
            }
        }

        protected void imgLocalizarBeneficiarios_Click(Object sender, ImageClickEventArgs e)
        {
            this.CarregarBeneneficiarios();   
        }

        protected void cmdGuardar_Click(Object sender, EventArgs e)
        {
            #region validações 

            if ((gridBeneficiarios.SelectedIndex == -1) && cboTipoTransacao.SelectedValue != "4")
            {
                base.Alerta(null, this, "_errBenef", "Você deve informar um beneficiário.");
                return;
            }
            else if (txtProposta.Text.Trim() == "")
            {
                base.Alerta(null, this, "_errProp", "Você deve informar uma proposta.");
                return;
            }

            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (Convert.ToInt32(gridBeneficiarios.DataKeys[gridBeneficiarios.SelectedIndex][2]) == Convert.ToInt32(grid.DataKeys[i][3]) && //beneficiario
                    Convert.ToInt32(gridBeneficiarios.DataKeys[gridBeneficiarios.SelectedIndex][1]) == Convert.ToInt32(grid.DataKeys[i][2]) && //contrato
                    Convert.ToInt32(cboTipoTransacao.SelectedValue) == Convert.ToInt32(grid.DataKeys[i][1]))                                   //tipo transacao
                {
                    base.Alerta(null, this, "_errExist", "Beneficiário ja adicionado à lista.");
                    return;
                }
            }

            #endregion

            ItemAgendaArquivoUnimed obj = new ItemAgendaArquivoUnimed();
            obj.BeneficiarioID = gridBeneficiarios.DataKeys[gridBeneficiarios.SelectedIndex][2];
            obj.PropostaID = gridBeneficiarios.DataKeys[gridBeneficiarios.SelectedIndex][1];
            obj.Tipo = Convert.ToInt32(cboTipoTransacao.SelectedValue);
            obj.TipoDescricao = cboTipoTransacao.SelectedItem.Text;
            obj.Salvar();

            this.CarregarItens();
            txtProposta.Focus();
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    ItemAgendaArquivoUnimed obj = new ItemAgendaArquivoUnimed();
                    obj.ID = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                    obj.Remover();
                    grid.PageIndex = 0;
                    Int32 qtd = this.CarregarItens();

                    if (qtd == 0) { ItemAgendaArquivoUnimed.Clear(); }
                }
                catch
                {
                    base.Alerta(null, this, "_errDel", "Não foi possível excluir.");
                }
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja prosseguir com a exclusão?");
            }
        }

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            List<ItemAgendaArquivoUnimed> itens = new List<ItemAgendaArquivoUnimed>();
            Object contratoAdmId = null;

            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if(Convert.ToString(grid.DataKeys[i][1]).Equals(cboTipoTransacaoGerar.SelectedValue))
                {
                    if (cboContratoAdm.SelectedIndex > 0)
                    {
                        contratoAdmId = Contrato.CarregaContratoAdmID(grid.DataKeys[i][2], null);
                        if (Convert.ToString(contratoAdmId) != cboContratoAdm.SelectedValue) { continue; }
                    }

                    ItemAgendaArquivoUnimed item = new ItemAgendaArquivoUnimed(grid.DataKeys[i][0]);
                    item.PropostaID = grid.DataKeys[i][2];
                    item.BeneficiarioID = grid.DataKeys[i][3];
                    itens.Add(item);
                }
            }

            if (itens.Count == 0)
            {
                base.Alerta(null, this, "_err", "Nenhuma ação disponível para essa movimentação.");
                return;
            }

            String strArquivoNome = null;

            ArqTransacionalUnimed arqTrans = new ArqTransacionalUnimed(Server.MapPath("/"));

            if (contratoAdmId == null)
            {
                arqTrans.GerarArquivoUNIMED_DemaisMovimentacoes_VariosContratosADM(
                    ref strArquivoNome,
                    itens,
                    traduzTipoStatusContratoBeneficiarioMovimentacao(cboTipoTransacaoGerar.SelectedValue));
            }
            else
            {
                arqTrans.GerarArquivoUNIMED_DemaisMovimentacoes(
                    ref strArquivoNome,
                    itens,
                    traduzTipoStatusContratoBeneficiarioMovimentacao(cboTipoTransacaoGerar.SelectedValue));
            }

            if (!String.IsNullOrEmpty(strArquivoNome))
            {
                foreach (ItemAgendaArquivoUnimed item in itens) { item.Remover(); }
                this.CarregarItens();

                this.BaixarArquivo(String.Concat(ArqTransacionalFilePath, strArquivoNome), strArquivoNome);
            }
            else
            {
                base.Alerta(null, this, "_nofile", "Nenhum arquivo pôde ser gerado.");
            }
        }
    }
}

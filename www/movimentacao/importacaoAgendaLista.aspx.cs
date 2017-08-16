namespace www.movimentacao
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class importacaoAgendaLista : PageBase
    {
        protected override void  OnLoad(EventArgs e)
        {
 	         base.OnLoad(e);

             if (!IsPostBack)
             {
                 txtAte.Text = DateTime.Now.ToString("dd/MM/yyyy");
                 txtDe.Text = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
                 txtDe.Focus();
             }
        }

        void CarregarAgendamento()
        {
            DateTime de = base.CStringToDateTime(txtDe.Text);
            if (de == DateTime.MinValue) { base.Alerta(null, this, "_err", "Data inicial inválida."); txtDe.Focus(); return; }
            DateTime ate = base.CStringToDateTime(txtAte.Text);
            if (ate == DateTime.MinValue) { base.Alerta(null, this, "_err", "Data final inválida."); txtAte.Focus(); return; }

            if (optPendentes.Checked)
            {
                gridItens.Columns[3].Visible = false; //coluna Processado em
                gridItens.Columns[4].Visible = false; //coluna botao crítica
                gridItens.Columns[5].Visible = true; //coluna botao excluir
            }
            else
            {
                gridItens.Columns[3].Visible = true; //coluna Processado em
                gridItens.Columns[4].Visible = true; //coluna botao crítica
                gridItens.Columns[5].Visible = false; //coluna botao excluir
            }

            gridItens.DataSource = ImportacaoProposta.Carregar(optProcessadas.Checked, de, ate);
            gridItens.DataBind();
        }

        protected void cmdFiltrar_Click(Object sender, EventArgs e)
        {
            this.CarregarAgendamento();
        }

        protected void gridItens_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            gridItens.PageIndex = e.NewPageIndex;
            this.CarregarAgendamento();
        }

        protected void gridItens_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                Object id = gridItens.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                ImportacaoProposta.ItemAgendamento item = new ImportacaoProposta.ItemAgendamento(id);
                item.Remover();
                this.CarregarAgendamento();
            }
            else if (e.CommandName.Equals("critica"))
            {
                gridCritica.DataSource = ImportacaoProposta.CarregarCritica(gridItens.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                gridCritica.DataBind();
                pnlCritica.Visible = true;
            }
            else if (e.CommandName.Equals("erro"))
            {
            }
        }

        protected void gridItens_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 5, "Deseja realmente excluir o item de agenda?\\nEssa operação não poderá ser desfeita.");

                if (optPendentes.Checked)
                {
                    Object id = gridItens.DataKeys[Convert.ToInt32(e.Row.RowIndex)][0];
                    ImportacaoProposta.ItemAgendamento item = new ImportacaoProposta.ItemAgendamento(id);
                    item.Carregar();

                    if (!String.IsNullOrEmpty(item.Erro))
                    {
                        e.Row.Cells[7].Controls[0].Visible = true;
                        ((LinkButton)e.Row.Cells[7].Controls[0]).ToolTip = "Erro no arquivo xml";
                    }
                    else
                        e.Row.Cells[7].Controls[0].Visible = false;
                }
                else
                {
                    e.Row.Cells[7].Controls[0].Visible = false;
                }
            }
        }
    }
}

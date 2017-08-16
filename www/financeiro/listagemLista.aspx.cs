namespace www.financeiro
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

    public partial class comissionamentoLista : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                //this.CarregaOperadoras();
                this.CarregaFiliais();

                SetaDatas();
            }
        }

        public void SetaDatas()
        {
            txtPeriodoDe.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            txtPeriodoAte.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        new String[] PegaIDsSelecionados(ListBox lst)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (ListItem item in lst.Items)
            {
                if (item.Selected)
                {
                    if (sb.Length > 0) { sb.Append(","); }
                    sb.Append(item.Value);
                }
            }

            if (sb.Length == 0)
                return null;
            else
                return sb.ToString().Split(',');
        }
        DateTime? PegaDataDe()
        {
            DateTime data = base.CStringToDateTime(txtPeriodoDe.Text);
            if (data == DateTime.MinValue)
                return null;
            else
                return data;
        }
        DateTime? PegaDataAte()
        {
            DateTime data = base.CStringToDateTime(txtPeriodoAte.Text);
            if (data == DateTime.MinValue)
                return null;
            else
                return data;
        }
        DateTime? PegaDataDeCorte()
        {
            DateTime data = base.CStringToDateTime(txtDataCorte.Text);
            if (data == DateTime.MinValue)
                return null;
            else
                return data;
        }

        //void CarregaOperadoras()
        //{
        //    lstOperadora.Items.Clear();
        //    lstOperadora.DataValueField = "ID";
        //    lstOperadora.DataTextField = "Nome";
        //    lstOperadora.DataSource = Operadora.CarregarTodas(true);
        //    lstOperadora.DataBind();
        //    //cboOperadora.Items.Insert(0, new ListItem("TODAS", "-1"));
        //}

        void CarregaFiliais()
        {
            lstFilial.DataValueField = "ID";
            lstFilial.DataTextField = "Nome";
            lstFilial.DataSource = Filial.CarregarTodas(true);
            lstFilial.DataBind();
            //cboFilial.Items.Insert(0, new ListItem("TODAS", "-1"));
        }

        void carregarAgenda()
        {
            String filial = lstFilial.SelectedValue;
            DateTime? dataDe = PegaDataDe();
            DateTime? dataAte = PegaDataAte();
            DateTime? dataDeCorte = PegaDataDeCorte();

            if (dataDe == null && (dataDe == null || dataAte == null))
            {
                base.Alerta(null, this, "_err", "Parâmetros incorretos.");
                return;
            }

            IList<ListagemAgendamento> lista = ListagemAgendamento.Carregar(dataDe, dataAte, dataDeCorte, ListagemAgendamento.eStatusBusca.Todos); //Listagem.CarregaPorParametros(txtNome.Text, filiais, operadoras, perfis, dataDe, dataAte, dataDeCorte, produtorId);

            if (lista != null && lista.Count > 0)
            {
                gdvListagem.DataSource = lista;
                gdvListagem.DataBind();
                gdvListagem.Visible = true;
                pnlNaoEncontrado.Visible = false;
            }
            else
            {
                gdvListagem.Visible = false;
                pnlNaoEncontrado.Visible = true;
            }
        }

        protected void cmdExibir_Click(Object sender, EventArgs e)
        {
            this.carregarAgenda();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    ListagemAgendamento la = new ListagemAgendamento(gdvListagem.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                    la.Remover();
                    this.carregarAgenda();
                }
                catch
                {
                    base.Alerta(null, this, "_errExcl", "Não foi possível excluir.");
                }
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 5, "Deseja prosseguir com a exclusão?");
            }
        }
    }
}
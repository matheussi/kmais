namespace www.UBRASP.arquivos.scc
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Facade;
    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Entity.ArquivoUbrasp;

    public partial class processados : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.CarregaContratoADM();
                this.CarregaArquivos();
            }
        }

        void CarregaContratoADM()
        {
            cboContratoADM.Items.Clear();

            cboContratoADM.DataValueField = "ID";
            cboContratoADM.DataTextField = "DescricaoCodigoSaudeDental";

            IList<ContratoADM> lista = ContratoADM.CarregarTodos(4, false);
            cboContratoADM.DataSource = lista;
            cboContratoADM.DataBind();
        }

        void CarregaArquivos()
        {
            cboArquivos.Items.Clear();
            if (cboContratoADM.Items.Count == 0) return;

            IList<PSCCDesconto_Header> arq = ArquivoUBRASPFacade.Instance.CarregaArquivosPSCC(cboContratoADM.SelectedValue);

            if (arq != null)
            {
                foreach (var i in arq)
                {
                    cboArquivos.Items.Add(new ListItem(
                        string.Concat("Ref.: ", i.DataRef.ToString("dd/MM/yyyy"), " - Data Proc.: ", i.Data.ToString("dd/MM/yyyy HH:mm")),
                        Convert.ToString(i.ID)));
                }
            }
        }

        void CarregaItens(string filtro, bool refazFiltro)
        {
            pnlResultado.Visible = false;
            if (cboArquivos.Items.Count == 0) return;

            IList<PSCCDesconto_Item> itens = ArquivoUBRASPFacade.Instance.CarregaItensDeArquivoPSCC(cboArquivos.SelectedValue, filtro);

            grid.DataSource = itens;
            grid.DataBind();

            if (itens != null && itens.Count > 0)
            {
                pnlResultado.Visible = true;

                if (refazFiltro)
                {
                    cboFiltro.Items.Clear();
                    cboFiltro.Items.Add(new ListItem("Todos", "-1"));
                    List<string> codigos = new List<string>();

                    foreach (PSCCDesconto_Item i in itens)
                    {
                        if (codigos.Contains(i.CodigoRetornoConciliacao)) continue;

                        codigos.Add(i.CodigoRetornoConciliacao);
                    }

                    foreach (string c in codigos)
                    {
                        cboFiltro.Items.Add(new ListItem(c, c));
                    }
                }
            }
        }

        protected void cboContratoAdm_SelectChange(object sender, EventArgs e)
        {
            this.CarregaArquivos();
            this.CarregaItens(null, true);
        }

        protected void cboArquivos_SelectChange(object sender, EventArgs e)
        {
            this.CarregaItens(null, true);
        }

        protected void cboFiltro_SelectChange(object sender, EventArgs e)
        {
            if (cboFiltro.SelectedIndex == 0)   this.CarregaItens(null, false);
            else                                this.CarregaItens(cboFiltro.SelectedValue, false);
        }

        protected void grid_OnPageChanging(Object sender, GridViewPageEventArgs e)
        {
            //if (e != null && e.NewPageIndex > -1)
            //{
            //    this.grid.PageIndex = e.NewPageIndex;
            //    this.grid.DataSource = this.dados;
            //    this.grid.DataBind();
            //}
        }

        protected void grid_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    PSCCDesconto_Item item = e.Row.DataItem as PSCCDesconto_Item;

            //    if (item != null && item.ParcelaValor != item.ParcelaValorENTE)
            //    {
            //        ((LinkButton)e.Row.Cells[7].Controls[1]).OnClientClick = "Confirma geração de boleto?";
            //    }
            //}
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                PSCCDesconto_Item item = e.Row.DataItem as PSCCDesconto_Item;

                if (item != null && item.ParcelaValor != item.ParcelaValorENTE)
                {
                    e.Row.ForeColor = System.Drawing.Color.Red;
                    ((LinkButton)e.Row.Cells[7].Controls[1]).OnClientClick = "return confirm('Confirma geração de boleto?');";
                    ((TextBox)e.Row.Cells[7].Controls[3]).Text = DateTime.Now.AddDays(20).ToString("dd/MM/yyyy");
                }
                else
                {
                    e.Row.Cells[7].Controls[1].Visible = false;
                    e.Row.Cells[7].Controls[3].Visible = false;
                }

                if (item.BeneficiarioId == null)
                {
                    e.Row.Cells[7].Controls[1].Visible = false;
                    e.Row.Cells[7].Controls[3].Visible = false;
                }
                else if (item.CobrancaID != null) 
                {
                    ((TextBox)e.Row.Cells[7].Controls[3]).Visible = false;
                    ((LinkButton)e.Row.Cells[7].Controls[1]).Text = "boleto gerado";
                }
            }
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "boletoAction")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                Object id  = grid.DataKeys[index][0];
                Object bid = grid.DataKeys[index][1];
                Object hid = grid.DataKeys[index][2];

                string strdata = ((TextBox)grid.Rows[index].Cells[7].Controls[3]).Text;
                DateTime vencto = base.CStringToDateTime(strdata, "23:59", 58);

                IList<PSCCDesconto_Item> itens = ArquivoUBRASPFacade.Instance.
                    CarregaItensDeArquivoPSCC_DoBeneficiarioParaBoleto(hid, id, bid);

                if (itens == null || itens.Count == 0) return;

                ArquivoUBRASPFacade.Instance.GerarCobrancaParaItens(itens, cboContratoADM.SelectedValue, vencto);

                this.CarregaItens(cboFiltro.SelectedValue, false);

                base.Alerta(null, this, "_ok", "Cobrança gerada com sucesso.");

            }
        }
    }
}
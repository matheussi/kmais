namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class comissionamentoP : PageBase
    {
        protected override void  OnLoad(EventArgs e)
        {
 	         base.OnLoad(e);

             if (!IsPostBack)
             {
                 //base.ExibirOperadoras(cboOperadora, false);
                 //this.CarregaContratos();
                 base.ExibirCategorias(cboCategoriaComissionamento, true, true);
                 this.PreencheGrid();

                 if (base.IDKeyParameterInProcess(ViewState, "_tabVal"))
                 {
                     this.CarregaTabela();
                     pnl.Visible = true;
                 }
             }
        }

        void PreencheGrid()
        {
            gridContratos.DataSource = ContratoADM.CarregarPorTabelaComissionamento(Request[IDKey]); //CarregarTodos();
            gridContratos.DataBind();
        }

        //void CarregaContratos()
        //{
        //    cboContrato.Items.Clear();
        //    if (cboOperadora.Items.Count == 0) { return; }
        //    cboContrato.DataValueField = "ID";
        //    cboContrato.DataTextField = "Descricao";
        //    cboContrato.DataSource = ContratoADM.Carregar(cboOperadora.SelectedValue);
        //    cboContrato.DataBind();
        //}

        void CarregaTabela()
        {
            Comissionamento tabela = new Comissionamento();
            tabela.ID = ViewState[IDKey];
            tabela.Carregar();

            //txtNome.Text = tabela.Descricao;
            if (tabela.Data != DateTime.MinValue) { txtData.Text = tabela.Data.ToString("dd/MM/yyyy"); }

            if (tabela.IdadeEspecial > 0)
                txtIdade.Text = Convert.ToString(tabela.IdadeEspecial);

            if (tabela.CategoriaID != null && cboCategoriaComissionamento.Items.FindByValue(Convert.ToString(tabela.CategoriaID)) != null)
                cboCategoriaComissionamento.SelectedValue = Convert.ToString(tabela.CategoriaID);
            else
                cboCategoriaComissionamento.SelectedIndex = 0;

            this.CarregaVitaliciedade();
            this.CarregaItensDaTabela();
        }

        void CarregaItensDaTabela()
        {
            IList<ComissionamentoItem> lista = null;

            //if (cboContrato.Items.Count > 0)
            //    lista = (List<ComissionamentoItem>)ComissionamentoItem.Carregar(ViewState[IDKey], cboContrato.SelectedValue);

            if (gridContratos.SelectedIndex > -1)
            {
                Object contratoID = gridContratos.DataKeys[gridContratos.SelectedIndex].Value;
                lista = ComissionamentoItem.Carregar(gridContratos.DataKeys[gridContratos.SelectedIndex][2]);
            }
            gridItens.DataSource = lista;
            gridItens.DataBind();

            pnl2.Visible = lista != null && lista.Count > 0;
        }

        void CarregaVitaliciedade()
        {
            //if (cboContrato.Items.Count > 0)
            //{
            if (gridContratos.SelectedIndex > -1)
            {
                Object contratoID = gridContratos.DataKeys[gridContratos.SelectedIndex][2];

                #region NORMAL
                ComissionamentoVitaliciedade cv = ComissionamentoVitaliciedade.Carregar(ViewState[IDKey], TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal);

                if (cv != null)
                {
                    chkComissionamentoVitalicio.Checked = cv.Vitalicia;
                    if (chkComissionamentoVitalicio.Checked)
                    {
                        txtComissionamentoNumeroParcelaVitalicio.Text = Convert.ToString(cv.ParcelaInicio);
                        txtComissionamentoVitalicioPercentual.Text = cv.Percentual.ToString("N2");
                    }
                    else
                    {
                        txtComissionamentoNumeroParcelaVitalicio.Text = "";
                        txtComissionamentoVitalicioPercentual.Text = "";
                    }
                }
                else
                {
                    chkComissionamentoVitalicio.Checked = false;
                    txtComissionamentoNumeroParcelaVitalicio.Text = "";
                    txtComissionamentoVitalicioPercentual.Text = "";
                }
                #endregion

                #region CARENCIA
                cv = ComissionamentoVitaliciedade.Carregar(ViewState[IDKey], TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia);

                if (cv != null)
                {
                    chkComissionamentoVitalicioCarencia.Checked = cv.Vitalicia;
                    if (chkComissionamentoVitalicioCarencia.Checked)
                    {
                        txtComissionamentoNumeroParcelaVitalicioCarencia.Text = Convert.ToString(cv.ParcelaInicio);
                        txtComissionamentoVitalicioPercentualCarencia.Text = cv.Percentual.ToString("N2");
                    }
                    else
                    {
                        txtComissionamentoNumeroParcelaVitalicioCarencia.Text = "";
                        txtComissionamentoVitalicioPercentualCarencia.Text = "";
                    }
                }
                else
                {
                    chkComissionamentoVitalicioCarencia.Checked = false;
                    txtComissionamentoNumeroParcelaVitalicioCarencia.Text = "";
                    txtComissionamentoVitalicioPercentualCarencia.Text = "";
                }
                #endregion

                #region MIGRACAO
                cv = ComissionamentoVitaliciedade.Carregar(ViewState[IDKey], TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao);

                if (cv != null)
                {
                    chkComissionamentoVitalicioMigracao.Checked = cv.Vitalicia;
                    if (chkComissionamentoVitalicioMigracao.Checked)
                    {
                        txtComissionamentoNumeroParcelaVitalicioMigracao.Text = Convert.ToString(cv.ParcelaInicio);
                        txtComissionamentoVitalicioPercentualMigracao.Text = cv.Percentual.ToString("N2");
                    }
                    else
                    {
                        txtComissionamentoNumeroParcelaVitalicioMigracao.Text = "";
                        txtComissionamentoVitalicioPercentualMigracao.Text = "";
                    }
                }
                else
                {
                    chkComissionamentoVitalicioMigracao.Checked = false;
                    txtComissionamentoNumeroParcelaVitalicioMigracao.Text = "";
                    txtComissionamentoVitalicioPercentualMigracao.Text = "";
                }
                #endregion

                #region ADM
                cv = ComissionamentoVitaliciedade.Carregar(ViewState[IDKey], TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa);

                if (cv != null)
                {
                    chkComissionamentoVitalicioADM.Checked = cv.Vitalicia;
                    if (chkComissionamentoVitalicioADM.Checked)
                    {
                        txtComissionamentoNumeroParcelaVitalicioADM.Text = Convert.ToString(cv.ParcelaInicio);
                        txtComissionamentoVitalicioPercentualADM.Text = cv.Percentual.ToString("N2");
                    }
                    else
                    {
                        txtComissionamentoNumeroParcelaVitalicioADM.Text = "";
                        txtComissionamentoVitalicioPercentualADM.Text = "";
                    }
                }
                else
                {
                    chkComissionamentoVitalicioADM.Checked = false;
                    txtComissionamentoNumeroParcelaVitalicioADM.Text = "";
                    txtComissionamentoVitalicioPercentualADM.Text = "";
                }
                #endregion

                #region ESPECIAL
                cv = ComissionamentoVitaliciedade.Carregar(ViewState[IDKey], TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial);

                if (cv != null)
                {
                    chkComissionamentoVitalicioEspecial.Checked = cv.Vitalicia;
                    if (chkComissionamentoVitalicioEspecial.Checked)
                    {
                        txtComissionamentoNumeroParcelaVitalicioEspecial.Text = Convert.ToString(cv.ParcelaInicio);
                        txtComissionamentoVitalicioPercentualEspecial.Text = cv.Percentual.ToString("N2");
                    }
                    else
                    {
                        txtComissionamentoNumeroParcelaVitalicioEspecial.Text = "";
                        txtComissionamentoVitalicioPercentualEspecial.Text = "";
                    }
                }
                else
                {
                    chkComissionamentoVitalicioEspecial.Checked = false;
                    txtComissionamentoNumeroParcelaVitalicioEspecial.Text = "";
                    txtComissionamentoVitalicioPercentualEspecial.Text = "";
                }
                #endregion

                #region IDADE
                cv = ComissionamentoVitaliciedade.Carregar(ViewState[IDKey], TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade);

                if (cv != null)
                {
                    chkComissionamentoVitalicioIdade.Checked = cv.Vitalicia;
                    if (chkComissionamentoVitalicioIdade.Checked)
                    {
                        txtComissionamentoNumeroParcelaVitalicioIdade.Text = Convert.ToString(cv.ParcelaInicio);
                        txtComissionamentoVitalicioPercentualIdade.Text = cv.Percentual.ToString("N2");
                    }
                    else
                    {
                        txtComissionamentoNumeroParcelaVitalicioIdade.Text = "";
                        txtComissionamentoVitalicioPercentualIdade.Text = "";
                    }
                }
                else
                {
                    chkComissionamentoVitalicioIdade.Checked = false;
                    txtComissionamentoNumeroParcelaVitalicioIdade.Text = "";
                    txtComissionamentoVitalicioPercentualIdade.Text = "";
                }
                #endregion
            }
            else
            {
                chkComissionamentoVitalicio.Checked = false;
                txtComissionamentoNumeroParcelaVitalicio.Text = "";
                txtComissionamentoVitalicioPercentual.Text = "";

                chkComissionamentoVitalicioCarencia.Checked = false;
                txtComissionamentoNumeroParcelaVitalicioCarencia.Text = "";
                txtComissionamentoVitalicioPercentualCarencia.Text = "";

                chkComissionamentoVitalicioMigracao.Checked = false;
                txtComissionamentoNumeroParcelaVitalicioMigracao.Text = "";
                txtComissionamentoVitalicioPercentualMigracao.Text = "";

                chkComissionamentoVitalicioADM.Checked = false;
                txtComissionamentoNumeroParcelaVitalicioADM.Text = "";
                txtComissionamentoVitalicioPercentualADM.Text = "";

                chkComissionamentoVitalicioEspecial.Checked = false;
                txtComissionamentoNumeroParcelaVitalicioEspecial.Text = "";
                txtComissionamentoVitalicioPercentualEspecial.Text = "";

                chkComissionamentoVitalicioIdade.Checked = false;
                txtComissionamentoNumeroParcelaVitalicioIdade.Text = "";
                txtComissionamentoVitalicioPercentualIdade.Text = "";
            }
        }

        protected void cboTipo_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaVitaliciedade();
        }

        protected void gridContratos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "detalhe")
            {
                //Object id = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                gridContratos.SelectedIndex = Convert.ToInt32(e.CommandArgument);

                this.CarregaItensDaTabela();
                this.CarregaVitaliciedade();
            }
        }

        //protected void cboOperadora_OnSelectedIndexChanged(Object sender, EventArgs e)
        //{
        //    this.CarregaContratos();
        //    this.CarregaItensDaTabela();
        //    this.CarregaVitaliciedade();
        //}

        //protected void cboContrato_OnSelectedIndexChanged(Object sender, EventArgs e)
        //{
        //    this.CarregaItensDaTabela();
        //    this.CarregaVitaliciedade();
        //}
    }
}

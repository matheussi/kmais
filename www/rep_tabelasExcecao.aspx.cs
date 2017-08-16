namespace www
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

    public partial class rep_tabelasExcecao : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, true);
            }
        }

        void CarregaVitaliciedade(Object tabelaId)
        {
            if (tabelaId != null)
            {
                #region NORMAL
                TabelaExcecaoVitaliciedade cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal);

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
                cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia);

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
                cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao);

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
                cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa);

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
                cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial);

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
                cv = TabelaExcecaoVitaliciedade.Carregar(tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade);

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
                #region limpa campos

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

                #endregion
            }
        }

        protected void cboOperadora_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            litMsg.Text = "";
            if (cboOperadora.SelectedIndex <= 0)
                grid.DataSource = null;
            else
            {
                DataTable dt = UntypedProcesses.RelacaoDeTabelasDeExcecaoQuery(cboOperadora.SelectedValue);
                grid.DataSource = dt;
                if (dt == null || dt.Rows.Count == 0) { litMsg.Text = "nenhuma..."; }
            }

            grid.DataBind();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("detalhe"))
            {
                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                TabelaExcecao tabela = new TabelaExcecao(id);
                tabela.Carregar();

                ContratoADM contrato = new ContratoADM(tabela.ContratoAdmID);
                contrato.Carregar();

                Operadora operadora = new Operadora();
                operadora.ID = contrato.OperadoraID;
                operadora.Carregar();

                lblOperadora.Text = operadora.Nome;
                lblContrato.Text = contrato.Descricao;
                lblDataVigencia.Text = tabela.Vigencia.ToString("dd/MM/yyyy");

                List<ExcecaoItem> itens = (List<ExcecaoItem>)ExcecaoItem.CarregarPorTabelaExcecaoID(tabela.ID);
                gridItens.DataSource = itens;
                gridItens.DataBind();

                this.CarregaVitaliciedade(tabela.ID);

                pnlTabelaExcecaoDetalhe.Visible = true;
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Confirma a exclusão?");
        }

        protected void cmdVoltarTabelaExcecao_Click(Object sender, EventArgs e)
        {
            pnlTabelaExcecaoDetalhe.Visible = false;
        }
    }
}
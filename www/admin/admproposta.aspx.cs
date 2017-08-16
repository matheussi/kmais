namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class admproposta : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);
                txtNumContrato.Focus();
            }
        }

        void Carregar()
        {
            Contrato contrato = Contrato.CarregarPorParametros(txtNumContrato.Text, (Object)cboOperadora.SelectedValue, null);

            IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoNumero(txtNumContrato.Text, cboOperadora.SelectedValue, true);
            grid.DataSource = beneficiarios;
            grid.DataBind();

            if(contrato != null)// if (beneficiarios != null && beneficiarios.Count > 0)
            {
                pnlResultado.Visible = true;
                ViewState[IDKey] = contrato.ID; //beneficiarios[0].ContratoID;
                txtNumContratoNovo.Text = txtNumContrato.Text;
                litMsg.Text = "";
            }
            else
            {
                pnlResultado.Visible = false;
                ViewState[IDKey] = null;
                txtNumContratoNovo.Text = "";
                litMsg.Text = "nenhum contrato localizado";
            }
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            this.Carregar();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("salvar"))
            {
                TextBox txt = (TextBox)grid.Rows[Convert.ToInt32(e.CommandArgument)].FindControl("txtNumero");

                if (txt.Text.Trim() == "")
                {
                    base.Alerta(null, this, "_err", "Informe o novo número de contrato.");
                    txt.Focus();
                    return;
                }

                Contrato.AlterarNumeroDeContrato(grid.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[1], txt.Text, null);
                base.Alerta(null, this, "_numOk", "Número de proposta alterado com sucesso.");
            }
            //if (e.CommandName.Equals("editar"))
            //{
            //    cboTipo.Items.Clear();
            //    cboTipo.Items.Add(new ListItem("Normal", "0"));
            //    cboTipo.Items.Add(new ListItem("Complementar", "1"));
            //    cboTipo.Items.Add(new ListItem("Dupla", "2"));

            //    Int32 index = Convert.ToInt32(e.CommandArgument);
            //    Object id = grid.DataKeys[index].Value;
            //    Cobranca cobranca = new Cobranca(id);
            //    cobranca.Carregar();

            //    this.AbreDetalhe();
            //    this.LimpaCampos();

            //    ViewState[IDKey] = id;
            //    txtParcela.Text = cobranca.Parcela.ToString();
            //    txtDataVencimento.Text = cobranca.DataVencimento.ToString("dd/MM/yyyy");
            //    txtValor.Text = cobranca.Valor.ToString("N2");

            //    chkCancelada.Checked = cobranca.Cancelada;

            //    cboTipo.SelectedValue = cobranca.Tipo.ToString();
            //    cboTipo_Changed(null, null);
            //    if (trDupla.Visible && cobranca.CobrancaRefID != null)
            //    {
            //        cboCobrancaReferente.SelectedValue = Convert.ToString(cobranca.CobrancaRefID);
            //    }

            //    if (cobranca.DataPgto != DateTime.MinValue)
            //    {
            //        txtValorPagto.Text = cobranca.ValorPgto.ToString("N2");
            //        txtDataPgto.Text = cobranca.DataPgto.ToString("dd/MM/yyyy");
            //    }
            //}
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ContratoBeneficiario cb = (ContratoBeneficiario)e.Row.DataItem;
                ((DropDownList)e.Row.FindControl("cboStatus")).SelectedValue = cb.Status.ToString();
            }
        }

        protected void grid_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ContratoBeneficiario.UI.FillDropdownWithStatus((DropDownList)e.Row.FindControl("cboStatus"));
            }
        }

        protected void cmdDetalhes_Click(Object sender, EventArgs e)
        {
            Response.Redirect(String.Concat("~/contrato.aspx?", IDKey, "=", ViewState[IDKey]));
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtNumContratoNovo.Text.Trim() == "")
            {
                base.Alerta(null, this, "err", "Informe o número do conrato.");
                return;
            }

            Contrato.AlterarNumeroDeContrato(ViewState[IDKey], txtNumContratoNovo.Text, null);
            base.Alerta(null, this, "_numOk", "Número de proposta alterado com sucesso.");
        }
    }
}
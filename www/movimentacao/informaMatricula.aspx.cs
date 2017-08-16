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

    public partial class informaMatricula : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);
                cboOperadora.Focus();
            }

            if (Usuario.Autenticado.PerfilID == Perfil.ConsultaPropostaBeneficiarioIDKey)
            {
                cmdFiltrar.Visible = false;
            }
        }

        void Filtrar()
        {
            if (txtNumeroProposta.Text.Trim() == "" && txtTitularCPF.Text.Length != 11 && txtTitularNome.Text.Trim() == "")
            {
                base.Alerta(null, this, "_numparam", "Você deve informar mais parâmetros para o filtro.");
                txtNumeroProposta.Focus();
                return;
            }

            IList<Contrato> contratos = Contrato.CarregarPorParametros(
                txtNumeroProposta.Text,cboOperadora.SelectedValue, 
                base.CStringToDateTime(txtVigencia.Text), txtTitularCPF.Text, txtTitularNome.Text);
            grid.DataSource = contratos;

            if (contratos == null)
            {
                pnlGrid.Visible = false;
                base.Alerta(null, this, "_semcontratos", "Nenhuma proposta pôde ser localizada com esses parâmetros.");
            }
            else
            {
                grid.SelectedIndex = -1;
                pnlGrid.Visible = true;
                if (contratos.Count == 1)
                {
                    grid.SelectedIndex = 0;
                    txtMatricula.Focus();
                    ExibeMatriculaAtual(contratos[0].ID);
                }
            }

            grid.DataBind();
        }

        void ExibeMatriculaAtual(Object contratoId)
        {
            Contrato contrato = new Contrato(contratoId);
            contrato.Carregar();

            Object titularId = ContratoBeneficiario.CarregaTitularID(contratoId, null);

            Beneficiario beneficiario = new Beneficiario(titularId);
            beneficiario.Carregar();

            if (String.IsNullOrEmpty(contrato.NumeroMatricula)) { contrato.NumeroMatricula = "nenhuma"; }

            lblMatriculaAtual.Text = String.Concat(
                beneficiario.Nome, " - Matrícula atual: ", contrato.NumeroMatricula);
        }

        protected void cmdFiltrar_Click(Object sender, EventArgs e)
        {
            this.Filtrar();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selecionar"))
            {
                grid.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                txtMatricula.Focus();

                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                ExibeMatriculaAtual(id);
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
        }

        protected void cmdAlterar_Click(Object sender, EventArgs e)
        {
            #region validação 
            if (grid.SelectedIndex == -1)
            {
                base.Alerta(null, this, "_errSel", "Nenhuma proposta selecionada.");
                return;
            }

            if (txtMatricula.Text.Trim() == "")
            {
                base.Alerta(null, this, "_errMatr", "Nenhum número de matrícula informado.");
                txtMatricula.Focus();
                return;
            }
            #endregion

            Contrato.AlterarNumeroDeMatricula(grid.DataKeys[grid.SelectedIndex].Value, txtMatricula.Text);
            txtMatricula.Text = "";
            grid.DataSource = null;
            grid.SelectedIndex = -1;
            grid.DataBind();
            pnlGrid.Visible = false;
            txtNumeroProposta.Focus();
            Alerta(null, this, "_ok", "Dados alterados com sucesso.");
        }
    }
}
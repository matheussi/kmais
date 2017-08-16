namespace www.UBRASP
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class contratos : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                UIHelper.AuthCtrl(cmdNovo, new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.PropostaBeneficiarioIDKey });

                base.ExibirOperadoras(cboOperadoras, true);

                this.CarregaContratos();
            }
        }

        void CarregaContratos()
        {
            DataTable lista = null;
            if (cboOperadoras.Items.Count > 1) cboOperadoras.SelectedIndex = 1;

            if (cboOperadoras.SelectedIndex == 0)
            {
                litAviso.Text = "Nenhuma operadora cadastrada";
                return;
            }

            litAviso.Text = "";

            if ((txtMatriculaFuncional.Text.Trim() != "" || txtBeneificarioNome.Text.Trim() != ""))
            {
                lista = Contrato.DTCarregarPorOperadoraID_LEGADO(cboOperadoras.SelectedValue, txtMatriculaFuncional.Text.Trim(), txtBeneificarioNome.Text.Trim());
                gridContratos.DataSource = lista;
                gridContratos.DataBind();
                if (gridContratos.Rows.Count == 0) { litAviso.Text = "Nenhuma proposta localizada"; }
            }
            else
            {
                litAviso.Text = "Informe ao menos um parâmetro para a pesquisa";
                gridContratos.DataSource = null;
                gridContratos.DataBind();
            }


            //if (cboOperadoras.SelectedIndex > 0 && (txtNumProposta.Text.Trim() != "" || txtBeneificarioNome.Text.Trim() != "" || txtIDCobranca.Text.Trim() != "" || txtProtocolo.Text.Trim() != ""))
            //    lista = Contrato.DTCarregarPorOperadoraID(cboOperadoras.SelectedValue, txtNumProposta.Text.Trim(), txtBeneificarioNome.Text.Trim(), txtIDCobranca.Text.Trim(), txtProtocolo.Text.Trim());
            //else if (txtNumProposta.Text.Trim() != "" || txtBeneificarioNome.Text.Trim() != "" || txtIDCobranca.Text.Trim() != "" || txtProtocolo.Text.Trim() != "")
            //    lista = Contrato.DTCarregarPorParametros(txtNumProposta.Text, txtBeneificarioNome.Text, txtIDCobranca.Text, txtProtocolo.Text.Trim());

            //gridContratos.DataSource = lista;
            //gridContratos.DataBind();
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            this.CarregaContratos();
            //if (gridContratos.Rows.Count == 0) { litAviso.Text = "Nenhuma proposta localizada"; }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Session[ConferenciaObjKey] = null;
            Response.Redirect("contrato.aspx");
        }

        protected void gridContratos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session[IDKey] = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                String uri = "contrato.aspx?" + IDKey + "=" + Session[IDKey];
                //if (txtProtocolo.Text.Trim() != "")
                //    uri += "&prot=" + txtProtocolo.Text;

                Response.Redirect(uri);
            }
            //else if (e.CommandName == "inativar")
            //{
            //    Object id = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
            //    Boolean status = Convert.ToBoolean(gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][2]);

            //    Contrato.AlteraStatusDeContrato(id, !status);
            //    this.CarregaContratos();
            //}
            else if (e.CommandName == "excluir")
            {
                Object id = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Contrato contrato = new Contrato(id);
                try
                {
                    contrato.Remover();
                    this.cmdLocalizar_Click(null, null);
                    base.Alerta(null, this, "_delOk", "Contrato excluído com sucesso.");
                }
                catch
                {
                    base.Alerta(null, this, "_delErr", "Não foi possível excluir o contrato.");
                }
            }
        }

        protected void gridContratos_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Boolean rascunho = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][1]);
                ((Image)e.Row.Cells[4].Controls[1]).Visible = rascunho;

                Boolean cancelado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][2]);
                Boolean inativado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][3]);

                UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[5].Controls[0], new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.PropostaBeneficiarioIDKey });
                UIHelper.AuthCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
                UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
                base.grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja realmente prosseguir com a exclusão?\\nEssa operação não poderá ser desfeita.");

                if (Usuario.Autenticado.PerfilID != Perfil.AdministradorIDKey) { gridContratos.Columns[7].Visible = false; }

                if (cancelado || inativado)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../images/unactive.png' title='inativo' alt='inativo' border='0'>";
                    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente ativar o contrato?");
                }
                else
                {
                    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente cancelar o contrato?");
                    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../images/active.png' title='ativo' alt='ativo' border='0'>";
                }
            }
        }

        protected void gridContratos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridContratos.PageIndex = e.NewPageIndex;
            this.CarregaContratos();
        }
    }
}
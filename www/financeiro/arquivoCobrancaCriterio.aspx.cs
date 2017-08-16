namespace www.financeiro
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class arquivoCobrancaCriterio : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaEstipulantes();
                this.CarregaOperadoras();
                this.CarregarCriterios();
                ArquivoRemessaCriterio.UI.PreencheComboComTiposFiltroTaxa(cboFiltroTaxa);
            }
        }

        void CarregaEstipulantes()
        {
            lstEstipulantes.Items.Clear();
            lstEstipulantes.DataValueField = "ID";
            lstEstipulantes.DataTextField = "Descricao";
            lstEstipulantes.DataSource = Estipulante.Carregar(true);
            lstEstipulantes.DataBind();
        }
        void CarregaOperadoras()
        {
            lstOperadoras.Items.Clear();
            lstOperadoras.DataValueField = "ID";
            lstOperadoras.DataTextField = "Nome";
            lstOperadoras.DataSource = Operadora.CarregarTodas(true);
            lstOperadoras.DataBind();
        }
        void CarregaContratos()
        {
            lstContrato.Items.Clear();
            lstContrato.DataTextField = "Descricao";
            lstContrato.DataValueField = "ID";

            String[] oIDS = this.PegaIDsSelecionados(lstOperadoras);
            if (oIDS == null || oIDS.Length == 0) { return; }

            String[] eIDS = this.PegaIDsSelecionados(lstEstipulantes);

            lstContrato.DataSource = ContratoADM.Carregar(oIDS, eIDS);
            lstContrato.DataBind();
        }
        void CarregarCriterios()
        {
            grid.DataSource = ArquivoRemessaCriterio.CarregarTodos();
            grid.DataBind();
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

        void LimparCampos()
        {
            lstContrato.Items.Clear();
            lstEstipulantes.SelectedIndex = -1;
            lstOperadoras.SelectedIndex = -1;
            txtNomeArquivo.Text = "";
            txtDescricao.Text = "";
            txtAns.Text = "";
            txtFone.Text = "";
            txtOperadora.Text = "";
            txtProjeto.Text = "";
        }

        protected void cmdRefinarContratoAdm_Click(Object sender, EventArgs e)
        {
            this.CarregaContratos();
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            String[] contratoIDs = this.PegaIDsSelecionados(lstContrato);
            if (contratoIDs == null || contratoIDs.Length == 0)
            {
                Alerta(null, this, "err1", "Selecione ao menos um contrato administrativo.");
                return;
            }

            //if (txtNomeArquivo.Text.Trim() == "")
            //{
            //    Alerta(null, this, "err2", "Informe o nome do arquivo.");
            //    txtNomeArquivo.Focus();
            //    return;
            //}

            if (txtDescricao.Text.Trim() == "")
            {
                Alerta(null, this, "err3", "Informe a descrição deste critério.");
                txtDescricao.Focus();
                return;
            }
            #endregion

            ArquivoRemessaCriterio arc = new ArquivoRemessaCriterio();

            if (grid.SelectedIndex > -1)
            {
                arc.ID = grid.DataKeys[grid.SelectedIndex].Value;
            }

            arc.Descricao = txtDescricao.Text;
            arc.ArquivoNome = txtNomeArquivo.Text;
            arc.Ans = txtAns.Text;
            arc.FoneAtendimento = txtFone.Text;
            arc.Operadora = txtOperadora.Text;
            arc.Projeto = txtProjeto.Text;
            arc.ContratoAdmIDs = String.Join(",", contratoIDs);
            arc.OperadoraID = lstOperadoras.SelectedValue;
            arc.TipoTaxa = Convert.ToInt32(cboFiltroTaxa.SelectedValue);
            arc.Salvar();

            Alerta(null, this, "ok", "Dados salvos com sucesso");
            grid.SelectedIndex = -1;
            this.CarregarCriterios();
            this.LimparCampos();
        }

        protected void grid_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton cmdExcluir = (ImageButton)e.Row.FindControl("imgExcluir");
                cmdExcluir.OnClientClick = "return confirm('ATENÇÃO!\\nDeseja realmente excluir o critério?');";
                cmdExcluir.CommandArgument = e.Row.RowIndex.ToString();
                //ScriptManager.GetCurrent(this).RegisterPostBackControl(imbDownloadArquivoButton);

                ImageButton cmdEditar = (ImageButton)e.Row.FindControl("imgEditar");
                cmdEditar.CommandArgument = e.Row.RowIndex.ToString();
            }
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                ArquivoRemessaCriterio arc = new ArquivoRemessaCriterio(id);
                arc.Remover();
                this.CarregarCriterios();
                Alerta(null, this, "excOK", "Critério excluído com sucesso.");
            }
            else if (e.CommandName.Equals("editar"))
            {
                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                ArquivoRemessaCriterio arc = new ArquivoRemessaCriterio(id);
                arc.Carregar();

                if (arc.ID == null) { this.CarregarCriterios(); return; }

                grid.SelectedIndex = Convert.ToInt32(e.CommandArgument);

                txtDescricao.Text = arc.Descricao;
                txtNomeArquivo.Text = arc.ArquivoNome;
                txtFone.Text = arc.FoneAtendimento;
                txtOperadora.Text = arc.Operadora;
                txtProjeto.Text = arc.Projeto;
                txtAns.Text = arc.Ans;

                lstOperadoras.SelectedValue = Convert.ToString(arc.OperadoraID);
                cboFiltroTaxa.SelectedValue = arc.TipoTaxa.ToString();
                this.CarregaContratos();

                String[] arr = arc.ContratoAdmIDs.Split(',');
                foreach (String contratoId in arr)
                {
                    foreach (ListItem item in lstContrato.Items)
                    {
                        if (item.Value == contratoId)
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
        }
    }
}

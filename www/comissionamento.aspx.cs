namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class comissionamento : PageBase
    {
        List<ComissionamentoItem> Itens
        {
            get
            {
                if (ViewState["_itens"] != null)
                {
                    List<ComissionamentoItem> lista = ViewState["_itens"] as List<ComissionamentoItem>;

                    for (int i = 0; i < gridItens.Rows.Count; i++)
                    {
                        lista[i].Parcela = base.CToInt(((TextBox)gridItens.Rows[i].Cells[0].Controls[1]).Text);
                        lista[i].Percentual                 = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[1].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualADM              = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[4].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualCompraCarencia   = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[2].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualEspecial         = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[5].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].PercentualMigracao         = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[3].Controls[1]).Text);//.Replace(",", ".")
                        lista[i].Idade                      = base.CToDecimal(((TextBox)gridItens.Rows[i].Cells[6].Controls[1]).Text);
                        lista[i].ID = gridItens.DataKeys[i].Value;
                        lista[i].OwnerID = ViewState[IDKey];
                    }

                    return lista;
                }
                else
                    return null;
            }
            set { ViewState["_itens"] = value; }
        }
        IList<ContratoADM> Contratos
        {
            get { return ViewState["_contratos"] as IList<ContratoADM>; }
            set { ViewState["_contratos"] = value; }
        }

        String tipoSel
        {
            get { return base.CToString(ViewState["__sel"]); }
            set { ViewState["__sel"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtComissionamentoVitalicioPercentual.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentual.ClientID + "')");
            txtComissionamentoVitalicioPercentualADM.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualADM.ClientID + "')");
            txtComissionamentoVitalicioPercentualCarencia.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualCarencia.ClientID + "')");
            txtComissionamentoVitalicioPercentualEspecial.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualEspecial.ClientID + "')");
            txtComissionamentoVitalicioPercentualIdade.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualIdade.ClientID + "')");
            txtComissionamentoVitalicioPercentualMigracao.Attributes.Add("onKeyUp", "mascara('" + txtComissionamentoVitalicioPercentualMigracao.ClientID + "')");

            txtIdade.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");

            if (!IsPostBack)
            {
                txtData.Text = DateTime.Now.ToString("dd/MM/yyyy");
                this.CarregaCategorias();
                base.ExibirOperadoras(cboOperadora_Novo, false);
                this.CarregaContratosADM();

                if (base.IDKeyParameterInProcess(ViewState, "_tabVal"))
                {
                    this.CarregaTabela();
                    this.CarregaGrupos();
                    //pnl.Visible = true;
                }
                else
                    trAdicionarGrupo.Visible = false;

                //this.PreencheGridDeContratos();
            }
        }

        void CarregaContratosADM()
        {
            cboContratoADM_Novo.Items.Clear();
            if (cboOperadora_Novo.Items.Count > 0)
            {
                cboContratoADM_Novo.DataValueField = "ID";
                cboContratoADM_Novo.DataTextField = "Descricao";
                cboContratoADM_Novo.DataSource = ContratoADM.Carregar(cboOperadora_Novo.SelectedValue, true);
                cboContratoADM_Novo.DataBind();
            }
        }

        protected void CarregaCategorias()
        {
            IList<Categoria> lista = Categoria.Carregar(true);

            cboCategoriaComissionamento.Items.Clear();

            if (lista != null)
            {
                foreach (Categoria categoria in lista)
                {
                    cboCategoriaComissionamento.Items.Add(new ListItem(
                            String.Concat(categoria.Descricao, " (", categoria.PerfilDescricao, ")"),
                            Convert.ToString(categoria.ID)));
                }
            }

            cboCategoriaComissionamento.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        void CarregaGrupos()
        {
            gridGrupos.DataSource = ComissionamentoGrupo.CarregarTodos(ViewState[IDKey]);
            gridGrupos.DataBind();
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

        void PreencheGridDeContratos()
        {
            Contratos = ContratoAdmGrupoComissionamento.Carregar(gridGrupos.DataKeys[gridGrupos.SelectedIndex].Value, ViewState[IDKey]);
            gridContratos.DataSource = Contratos;
            gridContratos.DataBind();

            //if (this.Contratos != null)
                pnlContratos.Visible = true;
        }

        void CarregaTabela()
        {
            Comissionamento tabela = new Comissionamento();
            tabela.ID = ViewState[IDKey];
            tabela.Carregar();

            //txtNome.Text = tabela.Descricao;
            if (tabela.Data != DateTime.MinValue) { txtData.Text = tabela.Data.ToString("dd/MM/yyyy"); }

            if (tabela.IdadeEspecial > 0)
                txtIdade.Text = Convert.ToString(tabela.IdadeEspecial);
            else
                txtIdade.Text = "";

            if (tabela.CategoriaID != null && cboCategoriaComissionamento.Items.FindByValue(Convert.ToString(tabela.CategoriaID)) != null)
                cboCategoriaComissionamento.SelectedValue = Convert.ToString(tabela.CategoriaID);
            else
                cboCategoriaComissionamento.SelectedIndex = 0;

            //if (tabela.ContratoAdmID != null)
            //{
            //    ContratoADM contrato = new ContratoADM(tabela.ContratoAdmID);
            //    contrato.Carregar();
            //    cboOperadora.SelectedValue = Convert.ToString(contrato.OperadoraID);
            //    this.CarregaContratos();
            //    cboContrato.SelectedValue = Convert.ToString(tabela.ContratoAdmID);
            //}

            //this.CarregaVitaliciedade();
            //this.CarregaItensDaTabela();
        }

        void CarregaItensDoGrupo()
        {
            List<ComissionamentoItem> lista = null;

            if (gridGrupos.SelectedIndex > -1)
            {
                Object grupoId = gridGrupos.DataKeys[gridGrupos.SelectedIndex].Value;
                lista = (List<ComissionamentoItem>)ComissionamentoItem.Carregar(grupoId);
            }

            gridItens.DataSource = lista;
            gridItens.DataBind();
            this.Itens = lista;

            pnl2.Visible = true; // lista != null && lista.Count > 0;
            //trAdicionar.Visible = false; //(lista == null || lista.Count == 0) && gridGrupos.SelectedIndex > -1;

            if (lista == null || lista.Count == 0)
            {
                this.MontaGridParaPrimeiraInsercao();
            }
        }

        void CarregaVitaliciedade()
        {
            if (gridGrupos.SelectedIndex > -1)
            {
                Object grupoID = gridGrupos.DataKeys[gridGrupos.SelectedIndex].Value;

                #region NORMAL 
                ComissionamentoVitaliciedade cv = ComissionamentoVitaliciedade.Carregar(grupoID, TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal);

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
                cv = ComissionamentoVitaliciedade.Carregar(grupoID, TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia);

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
                cv = ComissionamentoVitaliciedade.Carregar(grupoID, TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao);

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
                cv = ComissionamentoVitaliciedade.Carregar(grupoID, TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa);

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
                cv = ComissionamentoVitaliciedade.Carregar(grupoID, TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial);

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
                cv = ComissionamentoVitaliciedade.Carregar(grupoID, TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade);

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

        void MontaGridParaPrimeiraInsercao()
        {
            List<ComissionamentoItem> lista = new List<ComissionamentoItem>();
            for (int i = 1; i <= 4; i++)
            {
                lista.Add(new ComissionamentoItem());
            }

            gridItens.DataSource = lista;
            gridItens.DataBind();
            this.Itens = lista;
        }

        //protected void cboTipo_OnSelectedIndexChanged(Object sender, EventArgs e)
        //{
        //    if (chkVitalicio.Checked)
        //    {
        //        int result = 0;
        //        if (!Int32.TryParse(txtNumeroParcelaVitalicio.Text, out result))
        //        {
        //            base.Alerta(null, this, "_erro1", "Informe o número da parcela para tabela vitalícia.");
        //            txtNumeroParcelaVitalicio.Focus();
        //            cboTipo.SelectedValue = tipoSel;
        //            return;
        //        }

        //        Decimal result2 = 0;
        //        if (!Decimal.TryParse(txtVitalicioPercentual.Text, out result2))
        //        {
        //            base.Alerta(null, this, "_erro2", "Informe o percentual vitalício.");
        //            txtVitalicioPercentual.Focus();
        //            cboTipo.SelectedValue = tipoSel;
        //            return;
        //        }
        //    }

        //    this.SalvarVitaliciedade(ViewState[IDKey], Convert.ToInt32(tipoSel));
        //    this.CarregaVitaliciedade();
        //    tipoSel = cboTipo.SelectedValue;
        //}

        #region grupos 

        protected void gridGrupos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            gridContratos.DataSource = null;
            gridContratos.DataBind();
            pnlGrupoDetalhe.Visible = false;
            pnl2.Visible = false;
            trAdicionarGrupo.Visible = true;
            gridGrupos.SelectedIndex = -1;
            pnlContratos.Visible = false;

            if (e.CommandName == "editar")
            {
                ComissionamentoGrupo grupo = new ComissionamentoGrupo(gridGrupos.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                grupo.Carregar();
                txtGrupoDescricao.Text = grupo.Descricao;
                txtGrupoID.Value = Convert.ToString(grupo.ID);

                pnlGrupoDetalhe.Visible  = true;
                trAdicionarGrupo.Visible = false;
                gridGrupos.Visible       = false;
            }
            else if (e.CommandName.Equals("parcelas"))
            {
                pnl.Visible = true;
                gridGrupos.SelectedIndex = Convert.ToInt32(e.CommandArgument);

                this.CarregaItensDoGrupo();
                this.CarregaVitaliciedade();
            }
            else if (e.CommandName.Equals("contratos"))
            {
                gridGrupos.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                this.PreencheGridDeContratos();
            }
            else if (e.CommandName.Equals("excluir"))
            {
                ComissionamentoGrupo grupo = new ComissionamentoGrupo(gridGrupos.DataKeys[Convert.ToInt32(e.CommandArgument)].Value);
                try
                {
                    grupo.Remover();
                    this.CarregaGrupos();
                }
                catch
                {
                    base.Alerta(null, this, "_errGrupoExcl", "Não foi possível excluir o grupo.");
                }
            }
        }

        protected void gridGrupos_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente excluir este grupo?\\nEssa operação não poderá ser desfeita!");
        }

        protected void cmdAdicionarGrupo_Click(Object sender, EventArgs e)
        {
            txtGrupoID.Value = "";
            pnlGrupoDetalhe.Visible = true;
            trAdicionarGrupo.Visible = false;
            gridGrupos.Visible = false;
        }

        protected void cmdFecharGrupoDetalhe_Click(Object sender, EventArgs e)
        {
            pnlGrupoDetalhe.Visible = false;
            trAdicionarGrupo.Visible = true;
            gridGrupos.Visible = true;
        }

        protected void cmdSalvarGrupo_Click(Object sender, EventArgs e)
        {
            if (txtGrupoDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "_errGrupoDesc", "Informe uma descrição para o grupo.");
                txtGrupoDescricao.Focus();
                return;
            }

            ComissionamentoGrupo grupo = new ComissionamentoGrupo();
            if (txtGrupoID.Value != "") { grupo.ID = txtGrupoID.Value; }
            grupo.Descricao = txtGrupoDescricao.Text;
            grupo.TabelaComissionamentoID = ViewState[IDKey];
            grupo.Salvar();

            cmdFecharGrupoDetalhe_Click(null, null);
            this.CarregaGrupos();
        }

        protected void cmdAdicionar_Click(Object sender, EventArgs e)
        {
            this.MontaGridParaPrimeiraInsercao();
            //trAdicionar.Visible = false;
            pnl2.Visible = true;
        }

        protected void cmdFecharParcelas_Click(Object sender, EventArgs e)
        {
            gridGrupos.SelectedIndex = -1;
            pnl.Visible = false;
            this.Itens = null;
        }

        protected void cmdSalvarParcelas_Click(Object sender, EventArgs e)
        {
            List<ComissionamentoItem> lista = this.Itens;
            if (pnl.Visible && pnl2.Visible)
            {
                foreach (ComissionamentoItem item in lista)
                {
                    item.OwnerID = gridGrupos.DataKeys[gridGrupos.SelectedIndex][0];
                    item.Salvar();
                }
            }

            this.SalvarVitaliciedade();
            cmdFecharParcelas_Click(null, null);
            this.CarregaGrupos();
        }
        #endregion

        #region contratos adm 

        protected void gridContratos_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente excluir este item?");
        }

        protected void gridContratos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "excluir")
            {
                //Object contratoId = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                //Object grupoId = gridGrupos.DataKeys[gridGrupos.SelectedIndex].Value;
                Object id = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[1];
                //ContratoAdmGrupoComissionamento.Remover(contratoId, grupoId);

                //Contratos.RemoveAt(Convert.ToInt32(e.CommandArgument));
                //gridContratos.DataSource = Contratos;
                //gridContratos.DataBind();

                ContratoAdmGrupoComissionamento cgc = new ContratoAdmGrupoComissionamento(id);
                cgc.Remover();
                this.PreencheGridDeContratos();
            }
        }

        protected void cmdFecharContratos_Click(Object sender, EventArgs e)
        {
            pnlContratos.Visible = false;
            if (this.Contratos != null)
            {
                this.Contratos.Clear();
                this.Contratos = null;
            }
            gridGrupos.SelectedIndex = -1;
        }

        protected void cmdExcluirContratos_Click(Object sender, EventArgs e)
        {
            List<String> ids = new List<String>();
            foreach (GridViewRow row in this.gridContratos.Rows)
            {
                Control ctrlCheck = row.Cells[0].FindControl("chkContrato");

                if (ctrlCheck != null && ctrlCheck is CheckBox)
                {
                    if (((CheckBox)ctrlCheck).Checked)
                    {
                        ids.Add(Convert.ToString(this.gridContratos.DataKeys[row.RowIndex].Values[0]));
                    }
                }
            }

            if (ids.Count == 0)
            {
                base.Alerta(null, this, "_errSelContr", "Selecione ao menos um contrato administrativo.");
                return;
            }

            Object grupoId = gridGrupos.DataKeys[gridGrupos.SelectedIndex].Value;
            ContratoAdmGrupoComissionamento.Remover(ids, grupoId);

            foreach (String id in ids)
            {
                for (int i = 0; i < Contratos.Count; i++)
                {
                    if (Convert.ToString(Contratos[i].ID) == id)
                    {
                        Contratos.RemoveAt(i);
                        break;
                    }
                }
            }

            gridContratos.DataSource = Contratos;
            gridContratos.DataBind();
        }

        protected void cmdSalvarContratos_Click(Object sender, EventArgs e)
        {
            ContratoAdmGrupoComissionamento cac = null;
            foreach (GridViewRow row in gridContratos.Rows)
            {
                if (gridContratos.DataKeys[row.RowIndex][1] != null) { continue; }

                cac = new ContratoAdmGrupoComissionamento();
                cac.ContratoAdmID = gridContratos.DataKeys[row.RowIndex][0];
                cac.GrupoID = gridGrupos.DataKeys[gridGrupos.SelectedIndex].Value;
                cac.TabelaID = ViewState[IDKey];
                cac.Salvar();
            }

            cmdFecharContratos_Click(null, null);
            base.Alerta(null, this, "_contrOk", "Dados salvos com sucesso.");
        }

        #endregion

        protected void gridItens_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "excluir")
            {
                //Se nao houver mais nenuma linha de parcela, excluir tb o comissionamento vitalicio?
                Object id = gridItens.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                if (id == null)
                {
                    List<ComissionamentoItem> lista = this.Itens;
                    lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                    this.Itens = lista;
                    gridItens.DataSource = lista;
                    gridItens.DataBind();
                }
                else
                {
                    ComissionamentoItem item = new ComissionamentoItem();
                    item.ID = id;
                    item.Carregar();
                    item.Remover();
                    this.CarregaItensDoGrupo();
                }
            }
        }

        protected void gridItens_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja realmente excluir a linha?");

                TextBox txtvalor1 = (TextBox)e.Row.Cells[1].Controls[1];
                txtvalor1.Attributes.Add("onKeyUp", "mascara('" + txtvalor1.ClientID + "')");

                TextBox txtvalor2 = (TextBox)e.Row.Cells[2].Controls[1];
                txtvalor2.Attributes.Add("onKeyUp", "mascara('" + txtvalor2.ClientID + "')");

                TextBox txtvalor3 = (TextBox)e.Row.Cells[3].Controls[1];
                txtvalor3.Attributes.Add("onKeyUp", "mascara('" + txtvalor3.ClientID + "')");

                TextBox txtvalor4 = (TextBox)e.Row.Cells[4].Controls[1];
                txtvalor4.Attributes.Add("onKeyUp", "mascara('" + txtvalor4.ClientID + "')");

                TextBox txtvalor5 = (TextBox)e.Row.Cells[5].Controls[1];
                txtvalor5.Attributes.Add("onKeyUp", "mascara('" + txtvalor5.ClientID + "')");

                TextBox txtvalor6 = (TextBox)e.Row.Cells[6].Controls[1];
                txtvalor6.Attributes.Add("onKeyUp", "mascara('" + txtvalor6.ClientID + "')");

                Object id = gridItens.DataKeys[e.Row.RowIndex].Value;

                if (id == null)
                {
                    //se a linha nao está salva (nao tem id), seta para "" onde for 0
                    //TextBox txtparcela = (TextBox)e.Row.Cells[0].Controls[1];
                    //if (txtparcela.Text == "0") { txtparcela.Text = ""; }

                    if (CToDecimal(txtvalor1.Text) == 0) { txtvalor1.Text = ""; }
                    if (CToDecimal(txtvalor2.Text) == 0) { txtvalor2.Text = ""; }
                    if (CToDecimal(txtvalor3.Text) == 0) { txtvalor3.Text = ""; }
                    if (CToDecimal(txtvalor4.Text) == 0) { txtvalor4.Text = ""; }
                    if (CToDecimal(txtvalor5.Text) == 0) { txtvalor5.Text = ""; }
                    if (CToDecimal(txtvalor6.Text) == 0) { txtvalor6.Text = ""; }
                }
            }
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/comissionamentos.aspx");
        }

        void ExibeMsg(String msg, Boolean err)
        {
            if(err)
                litMsg.Text = String.Concat("<font size='1' color='red'>", msg, "</font>");
            else
                litMsg.Text = String.Concat("<font size='1' color='blue'>", msg, "</font>");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            if (!this.IsValid) { return; }

            //if (txtNome.Text.Trim() == "")
            //{
            //    base.Alerta(null, this, "_erro0", "Informe uma descrição.");
            //    txtNome.Focus();
            //    return;
            //}

            if (!base.HaItemSelecionado(cboCategoriaComissionamento))
            {
                //base.Alerta(null, this, "_err", "Não há uma categoria selecionada.");
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "_err", "alerta('Não há uma categoria selecionada.');", true);
                ExibeMsg("Não há uma categoria selecionada.", true);
                cboCategoriaComissionamento.Focus();
                return;
            }

            /////////////////////////////////////
            if (txtData.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_err0", "Você deve informar a data de vigência.");
                ExibeMsg("Você deve informar a data de vigência.", true);
                txtData.Focus();
                return;
            }
            else
            {
                DateTime data = new DateTime();
                if (!UIHelper.TyParseToDateTime(txtData.Text, out data))
                {
                    //base.Alerta(null, this, "_err0c", "A data de vigência informada está inválida.");
                    ExibeMsg("A data de vigência informada está inválida.", true);
                    txtData.Focus();
                    return;
                }
            }
            /////////////////////////////////////

            //checa se já nao há uma tabela com a vigência informada
            if(Comissionamento.ExisteTabelaComVigenciaInformada(cboCategoriaComissionamento.SelectedValue, base.CStringToDateTime(txtData.Text), ViewState[IDKey]))
            {
                //base.Alerta(null, this, "_err0b", "Já existe uma tabela para essa categoria e data de vigência.");
                ExibeMsg("Já existe uma tabela para essa categoria e data de vigência.", true);
                txtData.Focus();
                return;
            }

            if (pnl2.Visible)
            {
                int result = 0;
                Decimal result2 = 0;

                #region NORMAL 

                if (chkComissionamentoVitalicio.Checked)
                {
                    if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicio.Text, out result) || base.CToInt(txtComissionamentoNumeroParcelaVitalicio.Text) == 0)
                    {
                        //base.Alerta(null, this, "_erro1", "Informe o número da parcela para tabela vitalícia.");
                        ExibeMsg("Informe o número da parcela para tabela vitalícia.", true);
                        txtComissionamentoNumeroParcelaVitalicio.Focus();
                        return;
                    }

                    if (!Decimal.TryParse(txtComissionamentoVitalicioPercentual.Text, out result2))
                    {
                        //base.Alerta(null, this, "_erro2", "Informe o percentual vitalício.");
                        ExibeMsg("Informe o percentual vitalício.", true);
                        txtComissionamentoVitalicioPercentual.Focus();
                        return;
                    }
                }
                #endregion

                #region CARENCIA 

                if (chkComissionamentoVitalicioCarencia.Checked)
                {
                    if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicioCarencia.Text, out result) || base.CToInt(txtComissionamentoNumeroParcelaVitalicioCarencia.Text) == 0)
                    {
                        //base.Alerta(null, this, "_erro1", "Informe o número da parcela para tabela vitalícia.");
                        ExibeMsg("Informe o número da parcela para tabela vitalícia.", true);
                        txtComissionamentoNumeroParcelaVitalicioCarencia.Focus();
                        return;
                    }

                    if (!Decimal.TryParse(txtComissionamentoVitalicioPercentualCarencia.Text, out result2))
                    {
                        //base.Alerta(null, this, "_erro2", "Informe o percentual vitalício.");
                        ExibeMsg("Informe o percentual vitalício.", true);
                        txtComissionamentoVitalicioPercentualCarencia.Focus();
                        return;
                    }
                }
                #endregion

                #region MIGRACAO 

                if (chkComissionamentoVitalicioMigracao.Checked)
                {
                    if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicioMigracao.Text, out result) || base.CToInt(txtComissionamentoNumeroParcelaVitalicioMigracao.Text) == 0)
                    {
                        //base.Alerta(null, this, "_erro1", "Informe o número da parcela para tabela vitalícia.");
                        ExibeMsg("Informe o número da parcela para tabela vitalícia.", true);
                        txtComissionamentoNumeroParcelaVitalicioMigracao.Focus();
                        return;
                    }

                    if (!Decimal.TryParse(txtComissionamentoVitalicioPercentualMigracao.Text, out result2))
                    {
                        //base.Alerta(null, this, "_erro2", "Informe o percentual vitalício.");
                        ExibeMsg("Informe o percentual vitalício.", true);
                        txtComissionamentoVitalicioPercentualMigracao.Focus();
                        return;
                    }
                }
                #endregion

                #region ADM 

                if (chkComissionamentoVitalicioADM.Checked)
                {
                    if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicioADM.Text, out result) || base.CToInt(txtComissionamentoNumeroParcelaVitalicioADM.Text) == 0)
                    {
                        //base.Alerta(null, this, "_erro1", "Informe o número da parcela para tabela vitalícia.");
                        ExibeMsg("Informe o número da parcela para tabela vitalícia.", true);
                        txtComissionamentoNumeroParcelaVitalicioADM.Focus();
                        return;
                    }

                    if (!Decimal.TryParse(txtComissionamentoVitalicioPercentualADM.Text, out result2))
                    {
                        //base.Alerta(null, this, "_erro2", "Informe o percentual vitalício.");
                        ExibeMsg("Informe o percentual vitalício.", true);
                        txtComissionamentoVitalicioPercentualADM.Focus();
                        return;
                    }
                }
                #endregion

                #region ESPECIAL 

                if (chkComissionamentoVitalicioEspecial.Checked)
                {
                    if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicioEspecial.Text, out result) || base.CToInt(txtComissionamentoNumeroParcelaVitalicioEspecial.Text) == 0)
                    {
                        //base.Alerta(null, this, "_erro1", "Informe o número da parcela para tabela vitalícia.");
                        ExibeMsg("Informe o número da parcela para tabela vitalícia.", true);
                        txtComissionamentoNumeroParcelaVitalicioEspecial.Focus();
                        return;
                    }

                    if (!Decimal.TryParse(txtComissionamentoVitalicioPercentualEspecial.Text, out result2))
                    {
                        //base.Alerta(null, this, "_erro2", "Informe o percentual vitalício.");
                        ExibeMsg("Informe o percentual vitalício.", true);
                        txtComissionamentoVitalicioPercentualEspecial.Focus();
                        return;
                    }
                }
                #endregion

                #region IDADE 

                if (chkComissionamentoVitalicioIdade.Checked)
                {
                    if (!Int32.TryParse(txtComissionamentoNumeroParcelaVitalicioIdade.Text, out result) || base.CToInt(txtComissionamentoNumeroParcelaVitalicioIdade.Text) == 0)
                    {
                        //base.Alerta(null, this, "_erro1", "Informe o número da parcela para tabela vitalícia.");
                        ExibeMsg("Informe o número da parcela para tabela vitalícia.", true);
                        txtComissionamentoNumeroParcelaVitalicioIdade.Focus();
                        return;
                    }

                    if (!Decimal.TryParse(txtComissionamentoVitalicioPercentualIdade.Text, out result2))
                    {
                        //base.Alerta(null, this, "_erro2", "Informe o percentual vitalício.");
                        ExibeMsg("Informe o percentual vitalício.", true);
                        txtComissionamentoVitalicioPercentualIdade.Focus();
                        return;
                    }
                }
                #endregion

                String msg = UIHelper.ChecaGridDeParcelas(gridItens, 0);
                if (msg != String.Empty)
                {
                    //base.Alerta(null, this, "_erroCom4", msg);
                    ExibeMsg(msg, true);
                    return;
                }
            }

            if (cboCategoriaComissionamento.SelectedIndex <= 0)
            {
                //base.Alerta(null, this, "_erro3", "Informe a categoria da tabela de comissionamento.");
                ExibeMsg("Informe a categoria da tabela de comissionamento.", true);
                cboCategoriaComissionamento.Focus();
                return;
            }

            #endregion

            Comissionamento tabela = new Comissionamento();
            tabela.ID   = ViewState[IDKey];
            tabela.Tipo = (Int32)Comissionamento.eTipo.PagoAoOperador;
            //tabela.Descricao = txtNome.Text;
            if (txtData.Text.Trim() != "")
                tabela.Data = base.CStringToDateTime(txtData.Text);

            tabela.IdadeEspecial = UIHelper.CToInt(txtIdade.Text);

            tabela.CategoriaID = cboCategoriaComissionamento.SelectedValue;

            Boolean primeiraInsercao = ViewState[IDKey] == null;
            tabela.Salvar();
            ViewState[IDKey] = tabela.ID;

            if (pnl2.Visible)
            {
                //List<ComissionamentoItem> lista = this.Itens;
                //if (pnl.Visible && pnl2.Visible)
                //{
                //    foreach (ComissionamentoItem item in lista)
                //    {
                //        item.OwnerID = tabela.ID;
                //        //item.ContratoID = gridContratos.DataKeys[gridContratos.SelectedIndex][0]; //cboContrato.SelectedValue;
                //        item.Salvar();
                //    }
                //}

                //this.Itens = lista;
                //gridItens.DataSource = lista;
                //gridItens.DataBind();
                //this.SalvarVitaliciedade(tabela.ID);
            }

            pnl.Visible = true;
            trAdicionarGrupo.Visible = true;
            if (primeiraInsercao) { pnl2.Visible = false; /*trAdicionar.Visible = false;cboContrato_OnSelectedIndexChanged(null, null);*/ }
            //this.PreencheGridDeContratos();
            base.Alerta(null, this, "_salvo", "Tabela salva com sucesso!");
        }

        void SalvarVitaliciedade()
        {
            ComissionamentoVitaliciedade cv = null;
            Object grupoId = gridGrupos.DataKeys[gridGrupos.SelectedIndex][0];

            #region NORMAL 

            cv = ComissionamentoVitaliciedade.Carregar(grupoId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal);
            if (cv == null) { cv = new ComissionamentoVitaliciedade(); }
            cv.Vitalicia = chkComissionamentoVitalicio.Checked;
            cv.GrupoID = grupoId;
            cv.TipoColunaComissao = (Int32)TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal;

            if (chkComissionamentoVitalicio.Checked)
            {
                cv.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicio.Text);
                cv.Percentual = CToDecimal(txtComissionamentoVitalicioPercentual.Text);
            }
            else
            {
                cv.Percentual = 0;
                cv.ParcelaInicio = 0;
            }

            cv.Salvar();

            #endregion

            #region CARENCIA 

            cv = ComissionamentoVitaliciedade.Carregar(grupoId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia);
            if (cv == null) { cv = new ComissionamentoVitaliciedade(); }
            cv.Vitalicia = chkComissionamentoVitalicioCarencia.Checked;
            cv.GrupoID = grupoId;
            cv.TipoColunaComissao = (Int32)TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia;

            if (chkComissionamentoVitalicioCarencia.Checked)
            {
                cv.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioCarencia.Text);
                cv.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualCarencia.Text);
            }
            else
            {
                cv.Percentual = 0;
                cv.ParcelaInicio = 0;
            }

            cv.Salvar();

            #endregion

            #region MIGRACAO 

            cv = ComissionamentoVitaliciedade.Carregar(grupoId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao);
            if (cv == null) { cv = new ComissionamentoVitaliciedade(); }
            cv.Vitalicia = chkComissionamentoVitalicioMigracao.Checked;
            cv.GrupoID = grupoId;
            cv.TipoColunaComissao = (Int32)TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao;

            if (chkComissionamentoVitalicioMigracao.Checked)
            {
                cv.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioMigracao.Text);
                cv.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualMigracao.Text);
            }
            else
            {
                cv.Percentual = 0;
                cv.ParcelaInicio = 0;
            }

            cv.Salvar();

            #endregion

            #region ADM 

            cv = ComissionamentoVitaliciedade.Carregar(grupoId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa);
            if (cv == null) { cv = new ComissionamentoVitaliciedade(); }
            cv.Vitalicia = chkComissionamentoVitalicioADM.Checked;
            cv.GrupoID = grupoId;
            cv.TipoColunaComissao = (Int32)TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa;

            if (chkComissionamentoVitalicioADM.Checked)
            {
                cv.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioADM.Text);
                cv.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualADM.Text);
            }
            else
            {
                cv.Percentual = 0;
                cv.ParcelaInicio = 0;
            }

            cv.Salvar();

            #endregion

            #region ESPECIAL 

            cv = ComissionamentoVitaliciedade.Carregar(grupoId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial);
            if (cv == null) { cv = new ComissionamentoVitaliciedade(); }
            cv.Vitalicia = chkComissionamentoVitalicioEspecial.Checked;
            cv.GrupoID = grupoId;
            cv.TipoColunaComissao = (Int32)TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial;

            if (chkComissionamentoVitalicioEspecial.Checked)
            {
                cv.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioEspecial.Text);
                cv.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualEspecial.Text);
            }
            else
            {
                cv.Percentual = 0;
                cv.ParcelaInicio = 0;
            }

            cv.Salvar();

            #endregion

            #region IDADE 

            cv = ComissionamentoVitaliciedade.Carregar(grupoId, TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade);
            if (cv == null) { cv = new ComissionamentoVitaliciedade(); }
            cv.Vitalicia = chkComissionamentoVitalicioIdade.Checked;
            cv.GrupoID = grupoId;
            cv.TipoColunaComissao = (Int32)TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade;

            if (chkComissionamentoVitalicioIdade.Checked)
            {
                cv.ParcelaInicio = Convert.ToInt32(txtComissionamentoNumeroParcelaVitalicioIdade.Text);
                cv.Percentual = CToDecimal(txtComissionamentoVitalicioPercentualIdade.Text);
            }
            else
            {
                cv.Percentual = 0;
                cv.ParcelaInicio = 0;
            }

            cv.Salvar();

            #endregion
        }

        protected void cmdAddItem_Click(Object sender, EventArgs e)
        {
            List<ComissionamentoItem> lista = this.Itens;
            if (lista == null) { lista = new List<ComissionamentoItem>(); }

            lista.Add(new ComissionamentoItem());

            gridItens.DataSource = lista;
            gridItens.DataBind();
            this.Itens = lista;
        }

        protected void cboOperadora_Novo_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratosADM();
        }

        protected void cmdAddRelacionamento_Click(Object sender, EventArgs e)
        {
            ContratoAdmGrupoComissionamento cgc = new ContratoAdmGrupoComissionamento();
            cgc.ContratoAdmID = cboContratoADM_Novo.SelectedValue;
            cgc.GrupoID = gridGrupos.DataKeys[gridGrupos.SelectedIndex].Value;
            cgc.TabelaID = ViewState[IDKey];
            cgc.Salvar();
            this.PreencheGridDeContratos();
        }
    }
}
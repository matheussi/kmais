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

    public partial class cobrancas : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaEstipulantes();
                this.CarregaOperadoras();
                this.CarregaFiliais();
                base.CarregarTiposDeCobranca(cboTipo);
                this.CarregarCriterios();
                this.CarregaAgendamento();

                base.PreencheComboNumerico(cboMes, 1, 12, true);
                base.PreencheComboNumerico(cboAno, DateTime.Now.AddYears(-5).Year, DateTime.Now.AddYears(5).Year, true);
                cboAno.SelectedValue = DateTime.Now.Year.ToString();
                cboMes.SelectedValue = DateTime.Now.Month.ToString();

                base.PreencheComboNumerico(cboHora, 0, 23, true);
                base.PreencheComboNumerico(cboMinuto, 0, 59, true);

                Cobranca.UI.FillComboFormato(cboFormato); 
                Cobranca.UI.FillComboCarteira(cboCarteira);
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
        void CarregaFiliais()
        {
            lstFiliais.Items.Clear();
            lstFiliais.DataValueField = "ID";
            lstFiliais.DataTextField = "Nome";
            lstFiliais.DataSource = Filial.CarregarTodas(true);
            lstFiliais.DataBind();
        }
        void CarregarCriterios()
        {
            gridCriterios.DataSource = ArquivoRemessaCriterio.CarregarTodos();
            gridCriterios.DataBind();
        }

        void CarregaAgendamento()
        {
            gridAgendamento.DataSource = ArquivoRemessaAgendamento.CarregarTodos(true, null);
            gridAgendamento.DataBind();

            if (gridAgendamento.Rows.Count == 0)
                litAgendamento.Text = "Agendamentos de processamento (nenhum)";
            else
                litAgendamento.Text = "Agendamentos de processamento";
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

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (cboFormato.SelectedIndex == 1 && txtNomeArquivo.Text.Trim() == "")
            {
                base.Alerta(null, this, "__errNmArq", "Informe o nome do arquivo.");
                txtNomeArquivo.Focus();
                return;
            }

            DateTime proc = base.CStringToDateTime(txtProcessarEm.Text, cboHora.SelectedValue + ":" + cboMinuto.SelectedValue);
            DateTime de   = base.CStringToDateTime(txtVencimentoDe.Text);
            DateTime ate  = base.CStringToDateTime(txtVencimentoAte.Text, "23:59", 59);

            DateTime deVig  = base.CStringToDateTime(txtVigenciaDe.Text);
            DateTime ateVig = base.CStringToDateTime(txtVigenciaAte.Text, "23:59", 59);

            if (deVig == DateTime.MinValue || ateVig == DateTime.MinValue ||
                de == DateTime.MinValue || ate == DateTime.MinValue)
            {
                base.Alerta(null, this, "_dateErr", "Informe os parâmetros de data.");
                return;
            }

            if (txtQtdBoletos.Text.Trim() == "") { txtQtdBoletos.Text = "1"; }

            String grupo = null;

            if (cboFormato.SelectedValue == "1") //novo formato, precisa agrupar
            {
                grupo = ArquivoRemessaAgendamento.NovoGrupo();
            }

            Boolean chk = false;
            for (int i = 0; i < gridCriterios.Rows.Count; i++)
            {
                if (((CheckBox)gridCriterios.Rows[i].Cells[0].Controls[1]).Checked)
                {
                    chk = true;

                    ArquivoRemessaAgendamento ara = new ArquivoRemessaAgendamento();
                    ara.CriterioID      = gridCriterios.DataKeys[i].Value;
                    ara.Processado      = false;
                    ara.ProcessamentoEm = proc;
                    ara.VencimentoAte   = ate;
                    ara.VencimentoDe    = de;
                    ara.VigenciaDe      = deVig;
                    ara.VigenciaAte     = ateVig;
                    ara.QtdBoletos      = Convert.ToInt32(txtQtdBoletos.Text);
                    ara.Carteira        = Convert.ToInt32(cboCarteira.SelectedValue);
                    ara.Grupo           = grupo;
                    ara.ArquivoNomeInstance = txtNomeArquivo.Text;
                    ara.Salvar();
                }
            }

            if (!chk)
            {
                base.Alerta(null, this, "_err", "Informe ao menos um critério para geração do arquivo de remessa.");
            }
            else
            {
                this.CarregaAgendamento();
                base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
            }
        }

        protected void cboFormato_Changed(Object sender, EventArgs e)
        {
            txtNomeArquivo.Text = "";

            if (cboFormato.SelectedIndex == 0)
                trNomeArquivo.Visible = false;
            else
            {
                trNomeArquivo.Visible = true;
                txtNomeArquivo.Focus();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            String[] estIDS = PegaIDsSelecionados(lstEstipulantes);
            if (estIDS == null)
            {
                base.Alerta(null, this, "__errEst", "Selecione ao menos um estipulante.");
                return;
            }

            String[] operIDs = PegaIDsSelecionados(lstOperadoras);
            if (operIDs == null)
            {
                base.Alerta(null, this, "__errOpe", "Selecione ao menos uma operadora.");
                return;
            }

            String[] filIDS = PegaIDsSelecionados(lstFiliais);
            if (filIDS == null)
            {
                base.Alerta(null, this, "__errFil", "Selecione ao menos uma filial.");
                return;
            }

            #endregion

            IList<Cobranca> cobrancas = null;

            if (cboTipo.SelectedValue == "0") //normal
            {
                cobrancas = null;
                    //Cobranca.ProcessarCobrancasNormaisParaGerarRemessa(estIDS, operIDs,
                    //filIDS, Convert.ToInt32(cboMes.SelectedValue), Convert.ToInt32(cboAno.SelectedValue));
            }
            else
            {
                cobrancas = null; //Cobranca.ProcessarCobrancasPorTipoParaGerarRemessa(estIDS, operIDs, filIDS, Convert.ToInt32(cboMes.SelectedValue), Convert.ToInt32(cboAno.SelectedValue), ((Cobranca.eTipo)Convert.ToInt32(cboTipo.SelectedValue)));
            }

            pnlResultado.Visible = true;

            if (cobrancas != null && cobrancas.Count > 0)
            {
                litMsg.Text = "Resultado:";

                try
                {
                    IList<SumarioArquivoGeradoVO> vos = null; //ArquivoCobrancaUnibanco.GeraDocumentoCobranca_UNIBANCO(cobrancas, null);
                    grid.DataSource = vos;
                    grid.DataBind();
                    ViewState["ret"] = vos;

                    if (vos != null)
                    {
                        String path = Server.MapPath("/") + ConfigurationManager.AppSettings["financialFilePath"].Replace("/", @"\");
                        String fileName = String.Empty;
                        foreach (SumarioArquivoGeradoVO vo in vos)
                        {
                            fileName = vo.ArquivoNome;
                            File.WriteAllText(String.Concat(path, fileName), vo.ArquivoConteudo, System.Text.Encoding.ASCII);
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                litMsg.Text = "Nenhuma cobrança encontrada.";
            }
        }

        protected void grid_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton imbDownloadArquivoButton = (ImageButton)e.Row.FindControl("imbDownloadArquivo");
                imbDownloadArquivoButton.CommandArgument = e.Row.RowIndex.ToString();
                ScriptManager.GetCurrent(this).RegisterPostBackControl(imbDownloadArquivoButton);
            }
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("baixar"))
            {
                IList<SumarioArquivoGeradoVO> vos = (IList<SumarioArquivoGeradoVO>)ViewState["ret"];
                SumarioArquivoGeradoVO vo = vos[Convert.ToInt32(e.CommandArgument)];

                base.BaixaArquivoFinanceiro(vo.ArquivoNome);
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////

        protected void gridAgendamento_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton cmdExcluir = (ImageButton)e.Row.FindControl("imgExcluir");
                cmdExcluir.OnClientClick = "return confirm('ATENÇÃO!\\nDeseja realmente excluir o agendamento?');";
                cmdExcluir.CommandArgument = e.Row.RowIndex.ToString();
                //ScriptManager.GetCurrent(this).RegisterPostBackControl(imbDownloadArquivoButton);
            }
        }

        protected void gridAgendamento_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                Object id = gridAgendamento.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                ArquivoRemessaAgendamento arc = new ArquivoRemessaAgendamento(id);
                arc.Remover();
                this.CarregaAgendamento();
                Alerta(null, this, "excOK", "Agendamento excluído com sucesso.");
            }
        }
    }
}
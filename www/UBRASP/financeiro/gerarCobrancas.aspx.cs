namespace www.UBRASP.financeiro
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

    public partial class gerarCobrancas : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.CarregaEstipulantes();
                this.CarregaOperadoras();
                this.CarregaFiliais();
                base.CarregarTiposDeCobranca(cboTipo);
                this.CarregarCriterios();
                //this.CarregaAgendamento();

                base.PreencheComboNumerico(cboMes, 1, 12, true);
                base.PreencheComboNumerico(cboAno, DateTime.Now.AddYears(-5).Year, DateTime.Now.AddYears(5).Year, true);
                cboAno.SelectedValue = DateTime.Now.Year.ToString();
                cboMes.SelectedValue = DateTime.Now.Month.ToString();

                base.PreencheComboNumerico(cboHora, 0, 23, true);
                base.PreencheComboNumerico(cboMinuto, 0, 59, true);

                //Cobranca.UI.FillComboFormato(cboFormato);
                //Cobranca.UI.FillComboCarteira(cboCarteira);
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
            //gridCriterios.DataSource = ArquivoRemessaCriterio.CarregarTodos();
            //gridCriterios.DataBind();
        }

        void CarregaAgendamento()
        {
            //gridAgendamento.DataSource = ArquivoRemessaAgendamento.CarregarTodos(true, null);
            //gridAgendamento.DataBind();

            //if (gridAgendamento.Rows.Count == 0)
            //    litAgendamento.Text = "Agendamentos de processamento (nenhum)";
            //else
            //    litAgendamento.Text = "Agendamentos de processamento";
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

        protected void cmdGerar_Click(object sender, EventArgs e)
        {
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            //if (cboFormato.SelectedIndex == 1 && txtNomeArquivo.Text.Trim() == "")
            //{
            //    base.Alerta(null, this, "__errNmArq", "Informe o nome do arquivo.");
            //    txtNomeArquivo.Focus();
            //    return;
            //}

            DateTime proc = base.CStringToDateTime(txtProcessarEm.Text, cboHora.SelectedValue + ":" + cboMinuto.SelectedValue);
            //DateTime de = base.CStringToDateTime(txtVencimentoDe.Text);
            //DateTime ate = base.CStringToDateTime(txtVencimentoAte.Text, "23:59", 59);

            DateTime vencimento = base.CStringToDateTime(txtVencimento.Text, "23:59", 59);

            DateTime deVig = base.CStringToDateTime(txtVigenciaDe.Text);
            DateTime ateVig = base.CStringToDateTime(txtVigenciaAte.Text, "23:59", 59);

            if (deVig == DateTime.MinValue || ateVig == DateTime.MinValue || vencimento == DateTime.MinValue)
            {
                base.Alerta(null, this, "_dateErr", "Informe os parâmetros de data.");
                return;
            }

            if (txtQtdBoletos.Text.Trim() == "") { txtQtdBoletos.Text = "1"; }

            #region comentado 

            // String grupo = null;

            //if (cboFormato.SelectedValue == "1") //novo formato, precisa agrupar
            //{
            //    grupo = ArquivoRemessaAgendamento.NovoGrupo();
            //}

            //Boolean chk = false;
            //for (int i = 0; i < gridCriterios.Rows.Count; i++)
            //{
            //    if (((CheckBox)gridCriterios.Rows[i].Cells[0].Controls[1]).Checked)
            //    {
            //        chk = true;

            //        ArquivoRemessaAgendamento ara = new ArquivoRemessaAgendamento();
            //        ara.CriterioID = gridCriterios.DataKeys[i].Value;
            //        ara.Processado = false;
            //        ara.ProcessamentoEm = proc;
            //        ara.VencimentoAte = ate;
            //        ara.VencimentoDe = de;
            //        ara.VigenciaDe = deVig;
            //        ara.VigenciaAte = ateVig;
            //        ara.QtdBoletos = Convert.ToInt32(txtQtdBoletos.Text);
            //        ara.Carteira = Convert.ToInt32(cboCarteira.SelectedValue);
            //        ara.Grupo = grupo;
            //        ara.ArquivoNomeInstance = txtNomeArquivo.Text;
            //        ara.Salvar();
            //    }
            //}

            //if (!chk)
            //{
            //    base.Alerta(null, this, "_err", "Informe ao menos um critério para geração do arquivo de remessa.");
            //}
            //else
            //{
            //    this.CarregaAgendamento();
            //    base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
            //}
            #endregion

            ArquivoRemessaAgendamento ara = new ArquivoRemessaAgendamento();
            ara.CriterioID = null;
            ara.Processado = false;
            ara.ProcessamentoEm = proc;
            ara.VencimentoAte = vencimento;
            ara.VencimentoDe = vencimento;
            ara.VigenciaDe = deVig;
            ara.VigenciaAte = ateVig;
            ara.QtdBoletos = Convert.ToInt32(txtQtdBoletos.Text);
            ara.Carteira = 0; // Convert.ToInt32(cboCarteira.SelectedValue);
            ara.Grupo = null;
            ara.ArquivoNomeInstance = "remessa"; // txtNomeArquivo.Text;
            ara.Legado = true;
            ara.Salvar();

            Alerta(null, this, "_ok", "Agendamento salvo com sucesso.");
        }
    }
}
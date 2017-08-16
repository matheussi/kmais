namespace www.financeiro
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

    using www.reports;
    //using www.reports.financeiro;

    public partial class relatorios : PageBase
    {
        #region Protected Members

        protected HtmlTableRow TRPeriodo
        {
            get { return trPeriodo; }
            set { trPeriodo = value; }
        }

        protected HtmlTableRow TRComissionamento
        {
            get { return trComissionamento; }
            set { trComissionamento = value; }
        }

        /// <summary>
        /// Tag TR do ReportViewer
        /// </summary>
        protected HtmlTableRow TRReportViewer
        {
            get { return this.trReportViewer; }
        }

        #endregion

        #region Private Methods

        String[] PegaIDsSelecionados(ListBox lst)
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

        DateTime? PegaDataDe()
        {
            DateTime data = base.CStringToDateTime(txtPeriodoDe.Text);
            if (data == DateTime.MinValue)
                return null;
            else
                return data;
        }
        DateTime? PegaDataAte()
        {
            DateTime data = base.CStringToDateTime(txtPeriodoAte.Text);
            if (data == DateTime.MinValue)
                return null;
            else
                return data;
        }

        void CarregaListagens()
        {
            lstListagem.Items.Clear();
            lstListagem.DataTextField = "Nome";
            lstListagem.DataValueField = "ID";
            if (PegaDataAte() != null && PegaDataAte() != null)
            {
                lstListagem.DataSource = Listagem.CarregaPorParametros("", null, null, null, PegaDataDe(), PegaDataAte(), null, null);
                lstListagem.DataBind();
                lstListagem.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
            else
            {
                IList<Listagem> lstListagemAtual = Listagem.CarregaListagemAtual();
                lstListagem.DataSource = lstListagemAtual;
                lstListagem.DataBind();

                if (lstListagemAtual == null || lstListagemAtual.Count == 0)
                    lstListagem.Items.Insert(0, new ListItem("Selecione", "-1"));
            }

            if (lstListagem.Items.Count > 0)
            {
                lstListagem.SelectedIndex = 0;
                CarregaPerfis();
            }
        }

        void CarregaPerfis()
        {
            lstPerfil.Items.Clear();
            lstPerfil.DataTextField = "Descricao";
            lstPerfil.DataValueField = "ID";
            if (Convert.ToInt32(lstListagem.SelectedValue) > -1)
            {
                lstPerfil.DataSource = Perfil.CarregarPorListagem(lstListagem.SelectedValue);
                lstPerfil.DataBind();
            }
        }

        private void SetaDatas()
        {
            txtPeriodoDe.Text  = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            txtPeriodoAte.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void WriteReport(Object[] Perfil, Listagem Listagem)
        {
            if (Perfil != null && Perfil.Length > 0)
            {
                String strListagemID  = Listagem.ID.ToString();
                String strPerfilArray = String.Empty;

                for (Int16 i = 0; i < Perfil.Length; i++)
                {
                    if (i > 0)
                        strPerfilArray += ",";

                    strPerfilArray += Perfil[i].ToString();
                }

                String semRecibo = "", consolidado = "";
                if (chkSemRecibo.Checked) { semRecibo = "&semRecibo=1"; }
                if (chkConsolidadoBanco.Checked) { consolidado = "&consolidadoBanco=1"; }

                String strPopUpScript = String.Concat("largura = screen.width - 10; ",
                                                      "altura = screen.height - 100; ", //popupRelatorioListagem
                                                      "window.open('relatorioRuntimeComissao.aspx?listID=", strListagemID , semRecibo, consolidado, "&arrPerfil=", strPerfilArray , "', 'ExameWindow', 'width='+largura+',height='+altura+',top=0,left=0,screenX=0,screenY=0,status=yes,scrollbars=yes,toolbar=no,resizable=yes,maximized=yes,menubar=no,location=no');");

                ScriptManager.RegisterClientScriptBlock(this, typeof(relatorios), "__open_pop_up__", strPopUpScript, true);
            }
        }

        #endregion

        #region Protected Page Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.CarregaListagens();
            }
        }

        #endregion

        #region Protected Control Event Handlers

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            if (!this.ddlTipo.SelectedValue.Equals("-1") && !this.lstListagem.SelectedValue.Equals("-1"))
            {
                List<Object> lstPerfilSelecionado = new List<Object>(this.lstPerfil.Items.Count);

                foreach (ListItem item in this.lstPerfil.Items)
                    if (item.Selected)
                        lstPerfilSelecionado.Add(item.Value);

                if (lstPerfilSelecionado.Count > 0)
                {
                    Listagem listagemSelecionada = new Listagem(this.lstListagem.SelectedValue);
                    listagemSelecionada.Carregar();

                    this.WriteReport(lstPerfilSelecionado.ToArray(),listagemSelecionada);
                }
            }
        }

        protected void cmdAnteriores_Click(Object sender, EventArgs e)
        {
            if (txtPeriodoAte.Text == "" || txtPeriodoDe.Text == "")
                SetaDatas();

            TRPeriodo.Visible = true;
            CarregaListagens();
        }

        protected void lstListagem_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            CarregaPerfis();
        }

        protected void ddlTipo_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            TRComissionamento.Visible = false;

            if (Convert.ToInt32(ddlTipo.SelectedValue) == 1)
                TRComissionamento.Visible = true;
        }

        protected void txtPeriodoChange(Object sender, EventArgs e)
        {
            CarregaListagens();
        }

        #endregion
    }
}
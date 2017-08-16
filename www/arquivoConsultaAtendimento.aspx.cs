namespace www
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

    public partial class arquivoConsultaAtendimento : PageBase
    {
        protected override void  OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);
                base.ExibirTiposDeAtendimento(cboTipo);

                carregaAtendimentos();

                txtDataDe.Text = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
                txtDataAte.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        #region Protected Control Event Handlers

        #region onParamaterChange

        protected void onParamaterChange(Object sender, EventArgs e)
        {
            this.carregaAtendimentos();
        }

        #endregion

        #region DropdownList Operadora

        protected void cboOperadora_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            
            this.carregaAtendimentos();
        }

        #endregion

        #region DropDownList Tipo

        protected void cboTipo_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.carregaAtendimentos();
        }

        #endregion

        protected void cboAtendente_Changed(Object sender, EventArgs e)
        {
            this.carregaAtendimentos();
        }

        protected void cmdEfetivarAtendimento_OnClick(Object sender, EventArgs e)
        {
            alteraStatusAtendimento();
        }

        protected void cmdDeclinar_OnClick(Object sender, EventArgs e)
        {
            List<String> atendimentoIDs = new List<String>();

            foreach (DataListItem item in this.dlAtendimento.Items)
            {
                Control ctrlCheck = item.FindControl("chkDlCheck");

                if (ctrlCheck != null && ctrlCheck is CheckBox)
                {
                    if (((CheckBox)ctrlCheck).Checked)
                    {
                        atendimentoIDs.Add(Convert.ToString(
                            this.dlAtendimento.DataKeys[item.ItemIndex]));
                    }
                }
            }

            if (atendimentoIDs.Count > 0)
            {
                try
                {
                    AtendimentoFacade.Instance.CancelaAtendimento(atendimentoIDs);
                    carregaAtendimentos();
                }
                catch
                {
                    base.Alerta(null, this, "_err", "Houve um erro inesperado.\\nPor favor, tente novamente.");
                }
            }
            else
                base.Alerta(null, this, "_SelAtendimento", "Selecione ao menos um item.");
        }

        protected void gdvAtendimentos_OnRowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DateTime data = Convert.ToDateTime(gdvAtendimentos.DataKeys[e.Row.RowIndex][4]);
                if (data < DateTime.Now)
                    e.Row.ForeColor = System.Drawing.Color.Red;
                else
                    e.Row.ForeColor = System.Drawing.Color.Black;
            }
        }
        protected void dlAtendimento_ItemDataBound(Object sender, DataListItemEventArgs e)
        {
            if (rdbPendente.Checked && (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem))
            {
                if (!String.IsNullOrEmpty(((Literal)e.Item.FindControl("litItemPrazoKey")).Text))
                {
                    DateTime data = Convert.ToDateTime(((Literal)e.Item.FindControl("litItemPrazoKey")).Text);
                    if (data < DateTime.Now)
                        ((Literal)e.Item.FindControl("lblProtocolo")).Text = "<font color='red'>" + ((Literal)e.Item.FindControl("lblProtocolo")).Text + "</font>";
                    else
                        e.Item.ForeColor = System.Drawing.Color.Black;
                }
            }
        }

        #endregion

        LayoutArquivoCustomizado.eTipoTransacao traduzTipo()
        {
            if (cboTipo.SelectedValue == "2") //cancelamento
            {
                return LayoutArquivoCustomizado.eTipoTransacao.CancelaContrato;
            }
            else if (cboTipo.SelectedValue == "3") //2via de cartao
            {
                return LayoutArquivoCustomizado.eTipoTransacao.SegundaViaCartao;
            }
            else if (cboTipo.SelectedValue == "4") // alteracao de cadastro
            {
                return LayoutArquivoCustomizado.eTipoTransacao.AlteracaoCadastral;
            }
            else if (cboTipo.SelectedValue == "5") // mudanca de plano
            {
                return LayoutArquivoCustomizado.eTipoTransacao.MudancaPlano;
            }
            else if (cboTipo.SelectedValue == "6") // adicionar beneficiario
                return LayoutArquivoCustomizado.eTipoTransacao.AdicionaBeneficiario;
            else
                return LayoutArquivoCustomizado.eTipoTransacao.CancelaBeneficiario;
        }

        

        //void configuraCarregaArquivos()
        //{
        //    cboLayouts.Items.Clear();
        //    if (gdvAtendimentos.DataSource != null && cboTipo.SelectedIndex > 1)
        //    {
        //        cmdGerarArquivos.Visible = true;
        //        cboLayouts.Visible = true;

        //        cboLayouts.DataValueField = "ID";
        //        cboLayouts.DataTextField = "Nome";

        //        IList<LayoutArquivoCustomizado> layouts = LayoutArquivoCustomizado.
        //            Carregar(cboOperadora.SelectedValue, traduzTipo());
        //        cboLayouts.DataSource = layouts;
        //        cboLayouts.DataBind();
        //    }
        //    else
        //    {
        //        cmdGerarArquivos.Visible = false;
        //        cboLayouts.Visible = false;
        //    }
        //}

        private void carregaAtendimentos()
        {
            if (validaCampos())
            {
                pnlAtendimentos.Visible = true;
                Atendimento.eStatus status = Atendimento.eStatus.Concluido;
                if (rdbPendente.Checked) { status = Atendimento.eStatus.Pendente; }

                Object atendenteId = null;
                if (base.HaItemSelecionado(cboAtendente)) { atendenteId = cboAtendente.SelectedValue; }

                IList<Atendimento> listAtendimento = Atendimento.ConsultaAtendimentos(cboOperadora.SelectedValue, cboTipo.SelectedValue, txtDataDe.Text, txtDataAte.Text, atendenteId, status);
                //gdvAtendimentos.DataSource = listAtendimento;
                //gdvAtendimentos.DataBind();

                dlAtendimento.DataSource = listAtendimento;
                dlAtendimento.DataBind();

                cboAtendente.Items.Clear();
                cboAtendente.Items.Add(new ListItem("<TODOS>", "-1"));

                //this.configuraCarregaArquivos();

                if (listAtendimento != null)
                {
                    foreach (Atendimento atend in listAtendimento)
                    {
                        if (String.IsNullOrEmpty(atend.AtendenteApelido)) { continue; }
                        cboAtendente.Items.Add(new ListItem(
                            atend.AtendenteApelido, Convert.ToString(atend.AtendenteID)));
                    }

                    lblMsg.Visible = false;
                    //gdvAtendimentos.Visible = true;
                    dlAtendimento.Visible = true;

                    if (rdbConcluido.Checked)
                    {
                        gdvAtendimentos.Columns[0].Visible = false;
                        cmdEfetivarAtendimento.Visible = false;
                        cmdDeclinar.Visible = false;
                        //cmdGerarArquivos.Visible = false;
                        //cboLayouts.Visible = false;
                    }
                    else
                    {
                        gdvAtendimentos.Columns[0].Visible = true;
                        cmdEfetivarAtendimento.Visible = true;
                        cmdDeclinar.Visible = true;
                        //cmdGerarArquivos.Visible = true;
                        //cboLayouts.Visible = !Operadora.IsUnimed(this.cboOperadora.SelectedValue);
                    }
                }
                else
                {
                    lblMsg.Visible = true;
                    //gdvAtendimentos.Visible = false;
                    dlAtendimento.Visible = false;
                    cmdEfetivarAtendimento.Visible = false;
                    cmdDeclinar.Visible = false;
                }
            }
        }

        private void alteraStatusAtendimento()
        {
            List<String> atendimentoIDs = new List<String>();

            foreach (DataListItem item in this.dlAtendimento.Items)
            {
                Control ctrlCheck = item.FindControl("chkDlCheck");

                if (ctrlCheck != null && ctrlCheck is CheckBox)
                {
                    if (((CheckBox)ctrlCheck).Checked)
                    {
                        atendimentoIDs.Add(Convert.ToString(
                            this.dlAtendimento.DataKeys[item.ItemIndex]));
                    }
                }
            }

            if (atendimentoIDs.Count > 0)
            {
                try
                {
                    AtendimentoFacade.Instance.EfetivaAtendimento(atendimentoIDs);
                    carregaAtendimentos();
                }
                catch
                {
                    base.Alerta(null, this, "_err", "Houve um erro inesperado.\\nPor favor, tente novamente.");
                }
            }
            else
                base.Alerta(null, this, "_SelAtendimento", "Selecione ao menos um item.");
        }

        private Boolean validaCampos()
        {
            Boolean retorno = true;
            
            if (txtDataDe.Text.Trim() != "" || txtDataAte.Text.Trim() != "")
            {
                if (txtDataDe.Text.Trim() == "")
                {
                    retorno = false;
                }
                else if (txtDataAte.Text.Trim() == "")
                {
                    retorno = false;
                }
                else if (base.CStringToDateTime(txtDataDe.Text, "00:00") == DateTime.MinValue)
                {
                    retorno = false;
                }
                else if (base.CStringToDateTime(txtDataAte.Text, "00:00") == DateTime.MinValue)
                {
                    retorno = false;
                }
            }

            return retorno;
        }

        //protected void cmdGerarArquivo_OnClick(Object sender, EventArgs e)
        //{
        //    if (!base.HaItemSelecionado(cboOperadora) || !base.HaItemSelecionado(cboTipo))
        //        return;

        //    if (cboOperadora.SelectedValue.Equals("1")) // UNIMED (Workflow Diferenciado)
        //    {
        //        if (this.gdvAtendimentos != null && this.gdvAtendimentos.Rows != null && this.gdvAtendimentos.Rows.Count > 0)
        //        {
        //            Boolean HasBeneficiarioBaixado  = false;
        //            Int32 intQtdBeneficiarioBaixado = 0;
        //            Object OperadoraID              = null;
        //            String NumeroContrato           = null;
        //            Object ContratoID               = null;
        //            Object BeneficiarioID           = null;

        //            List<Object> lstContratoID      = new List<Object>();
        //            List<Object> lstBeneficiarioID  = new List<Object>();
        //            String[] splBeneficiarioID      = null;

        //            foreach (GridViewRow row in this.gdvAtendimentos.Rows)
        //            {
        //                Control ctrlCheck = row.Cells[0].FindControl("chkAtendimento");

        //                if (ctrlCheck != null && ctrlCheck is CheckBox)
        //                {
        //                    if (((CheckBox)ctrlCheck).Checked)
        //                    {
        //                        HasBeneficiarioBaixado = true;
        //                        intQtdBeneficiarioBaixado++;

        //                        OperadoraID       = this.gdvAtendimentos.DataKeys[row.RowIndex].Values[1];
        //                        NumeroContrato    = Convert.ToString(this.gdvAtendimentos.DataKeys[row.RowIndex].Values[2]);
        //                        ContratoID        = Contrato.CarregaContratoID(OperadoraID, NumeroContrato, null);
        //                        splBeneficiarioID = this.gdvAtendimentos.DataKeys[row.RowIndex].Values[3].ToString().Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

        //                        if (splBeneficiarioID != null && splBeneficiarioID.Length > 0)
        //                            BeneficiarioID = splBeneficiarioID[splBeneficiarioID.Length - 1];

        //                        if (ContratoID == null)
        //                            continue;

        //                        ContratoBeneficiario.AlteraStatusBeneficiario(ContratoID, BeneficiarioID, traduzTipoStatusContratoBeneficiario());

        //                        lstContratoID.Add(ContratoID);
        //                        lstBeneficiarioID.Add(BeneficiarioID);
        //                    }
        //                }
        //            }

        //            if (HasBeneficiarioBaixado)
        //            {
        //                String strArquivoNome = null;

        //                ArqTransacionalUnimed arqTrans = new ArqTransacionalUnimed(Server.MapPath("/"));
        //                arqTrans.GerarArquivoPorStatus(this.cboOperadora.SelectedValue, ref strArquivoNome, traduzTipoStatusContratoBeneficiario(), lstContratoID.ToArray(), lstBeneficiarioID.ToArray());

        //                if (!String.IsNullOrEmpty(strArquivoNome))
        //                    this.BaixarArquivo(String.Concat(ArqTransacionalFilePath, strArquivoNome), strArquivoNome);
        //            }
        //            else
        //                base.Alerta(this.upDadosComuns, this, "__error_message__", "Nenhum beneficiario foi selecionado.");
        //        }
        //    }
        //    else
        //    {
        //        if (!base.HaItemSelecionado(cboLayouts))
        //            return;

        //        LayoutArquivoCustomizado.eTipoTransacao tipoTransacao = traduzTipo();
        //        LayoutArquivoCustomizado.eBeneficiariosInclusos tipoInclusao = LayoutArquivoCustomizado.eBeneficiariosInclusos.Especifico;

        //        LayoutArquivoCustomizado layout = new LayoutArquivoCustomizado(cboLayouts.SelectedValue);
        //        layout.Carregar();

        //        List<String> contratoIDs = null; List<String> beneficiarios = null; Hashtable ht = null;
        //        String numeroContrato = "", operadoraId = "", contratoId = "", strBeneficiarioIDs = "";
        //        String[] arrBeneficiarios = null;
        //        foreach (GridViewRow row in this.gdvAtendimentos.Rows)
        //        {
        //            Control ctrlCheck = row.Cells[0].FindControl("chkAtendimento");

        //            if (ctrlCheck != null && ctrlCheck is CheckBox)
        //            {
        //                if (((CheckBox)ctrlCheck).Checked)
        //                {
        //                    operadoraId = Convert.ToString(this.gdvAtendimentos.DataKeys[row.RowIndex].Values[1]);
        //                    numeroContrato = Convert.ToString(this.gdvAtendimentos.DataKeys[row.RowIndex].Values[2]);
        //                    contratoId = UIHelper.CToString(Contrato.CarregaContratoID(operadoraId, numeroContrato, null));
        //                    strBeneficiarioIDs = UIHelper.CToString(this.gdvAtendimentos.DataKeys[row.RowIndex].Values[3]);

        //                    if (contratoId == "") { continue; }

        //                    if (contratoIDs == null) { contratoIDs = new List<String>(); }
        //                    if (!contratoIDs.Contains(contratoId)) { contratoIDs.Add(contratoId); }

        //                    if (tipoTransacao != LayoutArquivoCustomizado.eTipoTransacao.CancelaContrato &&
        //                        tipoTransacao != LayoutArquivoCustomizado.eTipoTransacao.Inclusao &&
        //                        tipoTransacao != LayoutArquivoCustomizado.eTipoTransacao.MudancaPlano)
        //                    {
        //                        if (strBeneficiarioIDs == "") { continue; }
        //                        if (ht == null) { ht = new Hashtable(); }

        //                        beneficiarios = (List<String>)ht[contratoId];
        //                        if (beneficiarios == null) { beneficiarios = new List<String>(); }
        //                        arrBeneficiarios = strBeneficiarioIDs.Split(',');
        //                        foreach (String arrItem in arrBeneficiarios)
        //                        {
        //                            if (!beneficiarios.Contains(arrItem)) { beneficiarios.Add(arrItem); }
        //                        }

        //                        ht[contratoId] = beneficiarios;
        //                    }
        //                    else if (tipoTransacao == LayoutArquivoCustomizado.eTipoTransacao.CancelaContrato ||
        //                             tipoTransacao == LayoutArquivoCustomizado.eTipoTransacao.MudancaPlano)
        //                    {
        //                        tipoInclusao = LayoutArquivoCustomizado.eBeneficiariosInclusos.ApenasTitular;
        //                    }
        //                    else if (tipoTransacao == LayoutArquivoCustomizado.eTipoTransacao.Inclusao)
        //                    {
        //                        tipoInclusao = LayoutArquivoCustomizado.eBeneficiariosInclusos.Novos;
        //                    }
        //                }
        //            }
        //        }

        //        if (contratoIDs == null) { return; }
        //        String content = layout.GeraConteudoDoArquivo(contratoIDs, ht, tipoInclusao);

        //        if (LayoutArquivoCustomizado.eFormatoArquivo.Xls == ((LayoutArquivoCustomizado.eFormatoArquivo)layout.Formato))
        //        {
        //            Session["content"] = content;
        //            Response.Redirect("xlsfile.aspx");
        //        }
        //        else
        //        {
        //            String arquivoNome = String.Concat("_mov_", DateTime.Now.ToString("ddMMyyyyHHmmfff"), ".txt");

        //            if (!Directory.Exists(String.Concat(Server.MapPath("/") + ConfigurationManager.AppSettings["transactcustom_file"].Replace("/", @"\"))))
        //            {
        //                Directory.CreateDirectory(String.Concat(Server.MapPath("/") + ConfigurationManager.AppSettings["transactcustom_file"].Replace("/", @"\")));
        //            }

        //            String strFullFilePath = String.Concat(Server.MapPath("/") + ConfigurationManager.AppSettings["transactcustom_file"].Replace("/", @"\"), arquivoNome);

        //            if (!String.IsNullOrEmpty(content))
        //            {
        //                base.BaixarArquivo(content, strFullFilePath, arquivoNome);
        //            }
        //            else
        //            {
        //                base.Alerta(null, this, "_err", "Não foi possível gerar o arquivo.");
        //            }
        //        }
        //    }
        //}
    }
}
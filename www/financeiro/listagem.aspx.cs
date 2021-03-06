﻿namespace www.financeiro
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

    public partial class comissionamento : PageBase
    {
        Object agendaID
        {
            get
            {
                if (ViewState[IDKey] != null)
                    return ViewState[IDKey];
                else if (Request[IDKey] != null)
                    return Request[IDKey];
                else
                    return null;
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaOperadoras();
                this.CarregaFiliais();

                base.PreencheComboNumerico(cboHora, 0, 23, true);
                base.PreencheComboNumerico(cboMinuto, 0, 59, true);
            }
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
        DateTime? PegaDataDeCorte()
        {
            DateTime data = base.CStringToDateTime(txtDataCorte.Text);
            if (data == DateTime.MinValue)
                return null;
            else
            {
                return new DateTime(data.Year, data.Month, data.Day, 23, 59, 59, 990);
            }
        }

        void CarregaOperadoras()
        {
            lstOperadora.Items.Clear();
            lstOperadora.DataValueField = "ID";
            lstOperadora.DataTextField = "Nome";
            lstOperadora.DataSource = Operadora.CarregarTodas(true);
            lstOperadora.DataBind();
            //cboOperadora.Items.Insert(0, new ListItem("TODAS", "-1"));
        }
        void CarregaFiliais()
        {
            lstFilial.DataValueField = "ID";
            lstFilial.DataTextField = "Nome";
            lstFilial.DataSource = Filial.CarregarTodas(true);
            lstFilial.DataBind();
        }

        protected void cmdFecharListagem_Click(Object sender, EventArgs e)
        {
            if (txtNome.Text.Trim() == "")
            {
                base.Alerta(null, this, "__err", "Informe a descrição da listagem.");
                txtNome.Focus();
                return;
            }
            if (txtCompetenciaP1.Text.Trim() == "" || txtCompetenciaP2.Text.Length != 4)
            {
                base.Alerta(null, this, "__err", "Informe a competência da listagem.");
                txtCompetenciaP1.Focus();
                return;
            }

            String filialId     = lstFilial.SelectedValue;
            String[] operadoras = this.PegaIDsSelecionados(lstOperadora);
            DateTime? dataCorte = this.PegaDataDeCorte();
            DateTime procEm     = base.CStringToDateTime(txtProcessarEm.Text, cboHora.SelectedValue + ":" + cboMinuto.SelectedValue);

            if (filialId == "" || operadoras == null || dataCorte == null)
            {
                base.Alerta(null, this, "__err2", "Os campos para seleção de filiais, operadoras e data de corte são obrigatórios.");
                return;
            }

            ListagemAgendamento la = new ListagemAgendamento(this.agendaID);
            la.Descricao = txtNome.Text;
            la.DataCorte = dataCorte.Value;
            la.FilialID = filialId;
            la.Competencia = txtCompetenciaP1.Text + "/" + txtCompetenciaP2.Text;
            la.OperadoraIDs = String.Join(",", operadoras);
            la.ProcessarEm = procEm;
            la.Salvar();

            base.Alerta(null, this, "_ok", "Agendamento realizado com sucesso.");
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("listagemLista.aspx");
        }

        protected void dl_ItemCommand(Object sender, DataListCommandEventArgs e)
        {
            if (e.CommandName.Equals("salvar"))
            {
                //Int32 index = Convert.ToInt32(e.CommandArgument);
                //Object id = dlFicha.DataKeys[e.Item.ItemIndex];

                //IList<ItemDeclaracaoSaudeINSTANCIA> itens =
                //    (IList<ItemDeclaracaoSaudeINSTANCIA>)ViewState["fic_" + cboBeneficiarioFicha.SelectedValue];

                //if (itens == null) { itens = new List<ItemDeclaracaoSaudeINSTANCIA>(); }

                //for (int i = 0; i < dlFicha.Items.Count; i++)
                //{
                //    Object itemDeclaracaoId = ((Literal)e.Item.FindControl("litItemDeclaracaoID")).Text;

                //    ItemDeclaracaoSaudeINSTANCIA item = PegaItemNaColecao(itens,
                //        itemDeclaracaoId, Server.HtmlDecode(((Label)e.Item.FindControl("lblQuesta")).Text));

                //    Boolean adiciona = false;
                //    if (item == null)
                //    {
                //        item = new ItemDeclaracaoSaudeINSTANCIA();
                //        adiciona = true;
                //    }

                //    item.ItemDeclaracaoID = itemDeclaracaoId;
                //    item.BeneficiarioID = cboBeneficiarioFicha.SelectedValue;
                //    item.ItemDeclaracaoTexto = Server.HtmlDecode(((Label)e.Item.FindControl("lblQuesta")).Text);

                //    DateTime dte;
                //    if (!UIHelper.TyParseToDateTime(((TextBox)e.Item.FindControl("txtFichaSaudeData")).Text, out dte))
                //    {
                //        //base.Alerta(null, this, "_itemFicErr0", "Data inválida.");
                //        ((Literal)e.Item.FindControl("litFichaAviso")).Text = "<br><font color='red' face='1'>Data inválida.</font>";
                //        ((TextBox)e.Item.FindControl("txtFichaSaudeData")).Focus();
                //        return;
                //    }

                //    if (((TextBox)e.Item.FindControl("txtFichaSaudeDescricao")).Text.Trim() == "")
                //    {
                //        //base.Alerta(null, this, "_itemFicErr1", "Informe uma descrição.");
                //        ((Literal)e.Item.FindControl("litFichaAviso")).Text = "<br><font color='red' face='1'>Informe uma descrição.</font>";
                //        ((TextBox)e.Item.FindControl("txtFichaSaudeDescricao")).Focus();
                //        return;
                //    }

                //    item.Data = dte;
                //    item.Descricao = ((TextBox)e.Item.FindControl("txtFichaSaudeDescricao")).Text;

                //    if (i == index)
                //        item.Sim = true;

                //    item.Salvar();

                //    if (adiciona)
                //        itens.Add(item);
                //    //else
                //    //    itens[index] = item;
                //    ((Literal)e.Item.FindControl("litFichaAviso")).Text = "<br><font color='blue' face='1'>Informação salva.</font>";
                //}

                //ViewState["fic_" + cboBeneficiarioFicha.SelectedValue] = itens;
                ////base.Alerta(null, this, "_itemFichaSalva", "Item salvo com sucesso.");
            }
        }
        protected void dl_ItemDataBound(Object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //if (((CheckBox)e.Item.FindControl("chkFSim")).Checked)
                //{
                //    ((HtmlTableRow)e.Item.FindControl("tr1Ficha")).Visible = true;
                //    ((HtmlTableRow)e.Item.FindControl("tr2Ficha")).Visible = true;
                //    ((HtmlTableRow)e.Item.FindControl("tr3Ficha")).Visible = true;
                //}
            }
        }
    }
}
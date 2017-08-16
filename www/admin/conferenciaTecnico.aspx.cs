namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class conferenciaTecnico : PageBaseConferencia
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            optCom.Attributes.Add("onClick", "document.getElementById('" + txtData.ClientID + "').focus();");
            txtData.Attributes.Add("onFocus", "document.getElementById('" + optCom.ClientID + "').checked=true;");
            if (!this.IsPostBack) 
            {
                txtData.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        void Carrega()
        {
            IList<MedicalReport> report = null;

            if (optSem.Checked)
            {
                grid.Columns[5].Visible = false;
                report = MedicalReport.CarregarParaTecnico(null);
            }
            else
            {
                DateTime data = DateTime.MinValue;
                if (!UIHelper.TyParseToDateTime(txtData.Text, out data))
                {
                    base.Alerta(null, this, "_errData", "A data informada é inválida.");
                    txtData.Focus();
                    return;
                }

                grid.Columns[5].Visible = true;
                report = MedicalReport.CarregarParaTecnico(data);
                
            }

            grid.DataSource = report;
            grid.DataBind();
            //dlItens.DataSource = report;
            //dlItens.DataBind();

            //if (grid.Rows.Count == 0)
            //    tblSalvar.Visible = false;
            //else
            //    tblSalvar.Visible = true;
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            this.Carrega();
        }

        protected void dlItens_ItemDataBound(Object sender, DataListItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem))
            {
                if (((Literal)e.Item.FindControl("lblAprovMed")).Text.ToUpper() == "TRUE")
                    ((Literal)e.Item.FindControl("lblAprovMed")).Text = "<font color='green'>Sim</font>";
                else
                    ((Literal)e.Item.FindControl("lblAprovMed")).Text = "<font color='red'>Não</font>";
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        protected void lnkRegra_Click(Object source, EventArgs e)
        {
            Object id = ((LinkButton)source).CommandArgument;
            MedicalReport dados = MedicalReport.CarregarParaTecnico(id);

            IList<RegraDeclaracaoSaude> regras = RegraDeclaracaoSaude.Carregar(dados.OperadoraID);

            if (regras == null || regras.Count == 0)
            {
                //nenhuma regra encontrada
                return;
            }

            MPE.Show();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("detail"))
            {
                Object beneficiarioId    = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Object contratoId        = grid.DataKeys[Convert.ToInt32(e.CommandArgument)][1];
                IList<MedicalReport> rep = MedicalReport.CarregarParaTecnico(beneficiarioId, contratoId);
                dlItens.DataSource       = rep;
                dlItens.DataBind();

                litBeneficiario.Text     = rep[0].BeneficiarioNome;
                MPE.Show();
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && optCom.Checked)
            {
                if (Convert.ToBoolean(grid.DataKeys[e.Row.RowIndex][2]))
                {
                    ((Image)e.Row.Cells[5].FindControl("imgStatus")).ImageUrl = "~/images/active.png";
                    ((Image)e.Row.Cells[5].FindControl("imgStatus")).ToolTip = "Aprovado";
                }
                else
                {
                    ((Image)e.Row.Cells[5].FindControl("imgStatus")).ImageUrl = "~/images/unactive.png";
                    ((Image)e.Row.Cells[5].FindControl("imgStatus")).ToolTip = "Reprovado";
                }
            }
        }

        protected void cmdRegras_Click(Object sender, EventArgs e)
        {
            Object id = null; MedicalReport dados = null;
            IList<RegraDeclaracaoSaude> regras = null; ItemRegraDeclaracaoSaude itemSaudeRegra = null;
            String regraOutput = "";
            for (int i = 0; i < dlItens.Items.Count; i++)
            {
                regraOutput = "";
                id = dlItens.DataKeys[i];
                dados = MedicalReport.CarregarParaTecnico(id);

                if (regras == null) { regras = RegraDeclaracaoSaude.Carregar(dados.OperadoraID); }

                if (regras == null) { regras = new List<RegraDeclaracaoSaude>(); }

                if(regras.Count == 0)
                {
                    ((Literal)dlItens.Items[i].FindControl("litRegraOutput")).Text = "<font color='blue'>nenhuma regra</font>";
                    continue;
                }

                foreach (RegraDeclaracaoSaude regra in regras)
                {
                    itemSaudeRegra = ItemRegraDeclaracaoSaude.Carregar(regra.ID, dados.ItemSaudeID);

                    if (itemSaudeRegra != null && !regra.Valida(dados.BeneficiarioIdade, dados.BeneficiarioSexoID))
                    {
                        if (regraOutput.Length > 0) { regraOutput += "<br>"; }
                        regraOutput += regra.Descricao;
                    }
                }

                if(regraOutput.Length > 0)
                    ((Literal)dlItens.Items[i].FindControl("litRegraOutput")).Text = "<font color='red'>" + regraOutput + "</font>";
                else
                    ((Literal)dlItens.Items[i].FindControl("litRegraOutput")).Text = "<font color='blue'>regras em conformidade</font>";
            }
            MPE.Show();
        }

        protected void cmdReprovar_Click(Object sender, EventArgs e)
        {
            if (dlItens.Items.Count == 0) { return; }

            Object id = dlItens.DataKeys[0];
            MedicalReport dados = MedicalReport.CarregarParaTecnico(id);

            //reprova o beneficiário
            ItemDeclaracaoSaudeINSTANCIA item = null;
            for (Int32 i = 0; i < dlItens.Items.Count; i++)
            {
                item = new ItemDeclaracaoSaudeINSTANCIA();
                item.ID = dlItens.DataKeys[i];
                item.Carregar();
                item.AprovadoPeloDeptoTecnico = false;
                item.DataAprovadoPeloDeptoTecnico = DateTime.Now;
                item.Salvar();
            }

            //marca o contrato como pendente
            Contrato contrato = new Contrato(dados.ContratoID);
            contrato.Carregar();
            contrato.Pendente = true;
            contrato.Salvar();

            this.Carrega();
        }

        protected void cmdAprovar_Click(Object sender, EventArgs e)
        {
            Object id = dlItens.DataKeys[0];
            MedicalReport dados = MedicalReport.CarregarParaTecnico(id);

            //aprova o beneficiário
            ItemDeclaracaoSaudeINSTANCIA item = null;
            for (Int32 i = 0; i < dlItens.Items.Count; i++)
            {
                item = new ItemDeclaracaoSaudeINSTANCIA();
                item.ID = dlItens.DataKeys[i];
                item.Carregar();
                item.AprovadoPeloDeptoTecnico = true;
                item.DataAprovadoPeloDeptoTecnico = DateTime.Now;
                item.Salvar();
            }

            //só tirar o status pendente do contrato se NAO houver nenhum beneficiario reprovado pelo depto tecnico.
            IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID(dados.ContratoID, true, false);
            foreach (ContratoBeneficiario beneficiario in beneficiarios)
            {
                IList<MedicalReport> itens = MedicalReport.CarregarParaTecnico(beneficiario.BeneficiarioID, dados.ContratoID);
                if (itens == null) { continue; }

                foreach (MedicalReport instancia in itens)
                {
                    //se foi reprovado pelo tecnico
                    if (!instancia.AprovadoDeptoTecnico && instancia.DataAprovadoDeptoTecnico != DateTime.MinValue)
                    {
                        this.Carrega();
                        return; //não altera status do contrato
                    }
                }
            }

            Contrato contrato = new Contrato(dados.ContratoID);
            contrato.Carregar();
            contrato.Pendente = false;
            contrato.Salvar();

            this.Carrega();
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            //if (grid.Rows.Count == 0) { return; }
            //DateTime data = DateTime.MinValue;
            //if (!UIHelper.TyParseToDateTime(txtDataSalvar.Text, out data))
            //{
            //    base.Alerta(null, this, "_errData1", "A data informada é inválida.");
            //    txtDataSalvar.Focus();
            //    return;
            //}

            ////TODO: transacionar e otimizar código
            //ItemDeclaracaoSaudeINSTANCIA item = null;
            //for (int i = 0; i < grid.Rows.Count; i++)
            //{
            //    item = new ItemDeclaracaoSaudeINSTANCIA();
            //    item.ID = grid.DataKeys[i].Value;
            //    item.Carregar();

            //    item.CIDInicial = ((TextBox)grid.Rows[i].Cells[6].Controls[1]).Text;
            //    item.CIDFinal = ((TextBox)grid.Rows[i].Cells[7].Controls[1]).Text;
            //    item.AprovadoPeloMedico = ((CheckBox)grid.Rows[i].Cells[8].Controls[1]).Checked;
            //    item.ObsMedico = ((TextBox)grid.Rows[i].Cells[9].Controls[1]).Text;

            //    if (item.AprovadoPeloMedico) { item.DataAprovadoPeloMedico = data; }
            //    else { item.DataAprovadoPeloMedico = DateTime.MinValue; }

            //    item.Salvar();
            //}

            //this.Carrega();
            //base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
        }
    }
}
/*
checa se o contrato tem algum beneficiario com positivacao de ficha de saude SEM aprovado do tecnico
se nao tiver, seta cancelado=false e pendente para false, where pendente=true
*/
namespace www.admin
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

    public partial class conferencia4 : PageBaseConferencia
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                ucCPM.EscreveMensagens();
                ucConferenciaPassos.SetaPasso(3, "Fase 3");

                base.ExibirDepartamentos(cboDepartamento, Usuario.Autenticado.PerfilID);
                this.CarregaItensSaude();
                this.LeConferencia();
            }
        }

        void CarregaItensSaude()
        {
            //chklItens.Items.Clear();
            //chklItens.DataValueField = "ID";
            //chklItens.DataTextField = "Descricao";

            Conferencia conferencia = base.ConferenciaCorrente;
            if (conferencia == null) { Response.Redirect("conferenciaLista.aspx"); }

            if (conferencia == null || conferencia.OperadoraID == null) { Response.Redirect("~/admin/conferenciaLista.aspx"); }

            dlFicha.DataSource = ConferenciaItemSaude.Carregar(conferencia.OperadoraID);
            dlFicha.DataBind();
        }

        void LeConferencia()
        {
            Conferencia conferencia = base.ConferenciaCorrente;
            if (conferencia == null) { Response.Redirect("~/admin/conferenciaLista.aspx"); }

            cboBeneficiario.Items.Clear();

            if(!String.IsNullOrEmpty(conferencia.TitularNome))
                cboBeneficiario.Items.Add(new ListItem("Titular: " + conferencia.TitularNome, conferencia.TitularCPF + "_0"));
            else
                cboBeneficiario.Items.Add(new ListItem("Titular: " + conferencia.TitularCPF, conferencia.TitularCPF + "_0"));

            List<ConferenciaBeneficiario> dependentes = (List<ConferenciaBeneficiario>)ConferenciaBeneficiario.Carregar(conferencia.ID);
            if (dependentes != null)
            {
                for (int i = 0; i < dependentes.Count; i++)
                {
                    cboBeneficiario.Items.Add(new ListItem("Dependente: " + dependentes[i].CPF, dependentes[i].CPF + "_" + dependentes[i].DataNascimento.ToString("yyyyMMdd")));
                }

                dependentes = null;
            }

            #region lê os itens de saúde que ja foram preenchidos 

            this.LeItensDeSaude(conferencia.ID, cboBeneficiario.SelectedValue.Split('_')[0]);

            #endregion

            if (conferencia.Prazo != DateTime.MinValue)
                txtPrazo.Text = conferencia.Prazo.ToString("dd/MM/yyyy");

            txtOBS.Text = conferencia.OBS;

            if(conferencia.Departamento > 0)
                cboDepartamento.SelectedValue = conferencia.Departamento.ToString();
        }

        void LeItensDeSaude(Object conferenciaId, String beneficiarioCpf)
        {
            for (int i = 0; i < dlFicha.Items.Count; i++)
            {
                ((CheckBox)dlFicha.Items[i].FindControl("chkFSim")).Checked = false;
            }

            IList<ConferenciaItemSaudeInstancia> itens = ConferenciaItemSaudeInstancia.Carregar(conferenciaId, beneficiarioCpf);
            if (itens != null)
            {
                foreach (ConferenciaItemSaudeInstancia item in itens)
                {
                    for (int i = 0; i < dlFicha.Items.Count; i++)
                    {
                        String _itemSaudeId = ((Literal)dlFicha.Items[i].FindControl("litItemDeclaracaoID")).Text;
                        if (Convert.ToString(item.ItemSaudeID).Equals(_itemSaudeId))
                        {
                            ((CheckBox)dlFicha.Items[i].FindControl("chkFSim")).Checked = item.Valor;
                            break;
                        }
                    }
                }
            }
        }

        Boolean PersisteConferencia()
        {
            Conferencia conferencia = base.ConferenciaCorrente;
            if (conferencia == null) { Response.Redirect("~/admin/conferenciaLista.aspx"); }

            conferencia.Departamento = Convert.ToInt32(cboDepartamento.SelectedValue);
            conferencia.OBS = txtOBS.Text;
            conferencia.Prazo = base.CStringToDateTime(txtPrazo.Text);

            conferencia.Salvar();

            for (int i = 0; i < dlFicha.Items.Count; i++)
            {
                if (((CheckBox)dlFicha.Items[i].FindControl("chkFSim")).Checked)
                    return false;
            }

            base.ConferenciaCorrente = conferencia;

            return true;
        }

        //void PersisteItensDeSaude(Object conferenciaId)
        //{
        //    if (dlFicha.Items.Count > 0)
        //    {
        //        List<ConferenciaItemSaudeInstancia> itens = new List<ConferenciaItemSaudeInstancia>();
        //        for (int i = 0; i < dlFicha.Items.Count; i++)
        //        {
        //            ConferenciaItemSaudeInstancia item = new ConferenciaItemSaudeInstancia();
        //            item.ConferenciaID = conferenciaId;
        //            item.ItemSaudeID = ((Literal)dlFicha.Items[i].FindControl("litItemDeclaracaoID")).Text;// chklItens.Items[i].Value;
        //            item.Valor = ((CheckBox)dlFicha.Items[i].FindControl("chkFSim")).Checked;// chklItens.Items[i].Selected;
        //            item.BeneficiarioCPF = cboBeneficiario.SelectedValue.Split('_')[0];
        //            itens.Add(item);
        //        }

        //        ConferenciaItemSaudeInstancia.Salvar(itens);
        //    }
        //    else
        //    {
        //        //remove tudo ? pensar... 
        //    }
        //}

        protected void cboBeneficiario_OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.LeItensDeSaude(base.ConferenciaCorrente.ID, cboBeneficiario.SelectedValue.Split('_')[0]);
        }

        protected void chkFSim_Changed(Object sender, EventArgs e)
        {
            cmdProximo.Text = "Salvar";
            CheckBox check = (CheckBox)sender;

            DataListItem grow = (DataListItem)check.NamingContainer;
            ConferenciaItemSaudeInstancia item = new ConferenciaItemSaudeInstancia();
            item.ConferenciaID = base.ConferenciaCorrente.ID;
            item.ItemSaudeID = ((Literal)grow.FindControl("litItemDeclaracaoID")).Text;
            item.Valor = check.Checked;
            item.BeneficiarioCPF = cboBeneficiario.SelectedValue.Split('_')[0];//Tirar o split e gravar todo o value ?

            List<ConferenciaItemSaudeInstancia> itens = new List<ConferenciaItemSaudeInstancia>();
            itens.Add(item);
            ConferenciaItemSaudeInstancia.Salvar(itens);
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            //this.PersisteConferencia();
            Response.Redirect("conferencia2.aspx");
        }

        Boolean ValidaProposta()
        {
            if (cmdProximo.Text != "Terminar")
            {
                String cpf = ""; IList<ConferenciaItemSaudeInstancia> itens = null;
                for (int i = 0; i < cboBeneficiario.Items.Count; i++)
                {
                    cpf = cboBeneficiario.Items[i].Value.Split('_')[0];
                    itens = ConferenciaItemSaudeInstancia.Carregar(base.ConferenciaCorrente.ID, cpf);
                    if (itens != null)
                    {
                        foreach (ConferenciaItemSaudeInstancia item in itens)
                        {
                            if (item.Valor)
                            {
                                ucCPM.SetaMsg("_errItemSaude", "Um ou mais itens de saúde foram marcados.Reter cheque e enviar para análise médica.");
                                cmdProximo.Text = "Terminar";
                                return false;
                            }
                        }
                    }
                }
            }

            ucCPM.RemoveMsg("_errItemSaude");
            return true;
        }

        protected void cmdProximo_Click(Object sender, EventArgs e)
        {
            //TODO: checar se no numero do contrato nao esta em uso para a operadora selecionada,
            //aqui e quando mandar para o cadastro, e quando efetivar.
            if (!this.ValidaProposta())
            {
                return;
            }

            Boolean alteraEstagio = base.ConferenciaCorrente.Departamento != Convert.ToInt32(cboDepartamento.SelectedValue);

            this.PersisteConferencia();
            if (alteraEstagio)
            {
                List<String> id = new List<String>(), numero = new List<String>();
                id.Add(Convert.ToString(base.ConferenciaCorrente.ID));
                numero.Add(Convert.ToString(base.ConferenciaCorrente.PropostaNumero));
                Conferencia.AlteraEstagio(base.ConferenciaCorrente.OperadoraID, id, numero, ((ContratoStatusHistorico.eStatus)Convert.ToInt32(cboDepartamento.SelectedValue)));
            }
            Response.Redirect("conferenciaLista.aspx");
        }
    }
}
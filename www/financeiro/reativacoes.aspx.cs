namespace www.financeiro
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class reativacoes : PageBase
    {
        protected void Page_Load(Object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregaMotivos();
                base.ExibirOperadoras(cboOperadoras, true);
                cboOperadoras.Focus();
            }
        }

        void carregaMotivos()
        {
            cboStatusMotivo.DataValueField = "ID";
            cboStatusMotivo.DataTextField  = "Descricao";
            cboStatusMotivo.DataSource     = ContratoStatus.Carregar(ContratoStatus.eTipo.Reativacao);
            cboStatusMotivo.DataBind();
        }

        protected void cmdReativar_Click(Object sender, EventArgs e)
        {
            if (cboOperadoras.SelectedIndex <= 0)
            { base.Alerta(null, this, "_err", "Nenhuma operadora informada."); return; }

            String[] numeros = txtPropostas.Text.Split(new String[] { "\n" }, StringSplitOptions.None); //Environment.NewLine

            if (numeros == null || numeros.Length == 0) { base.Alerta(null, this, "_err", "Nenhuma proposta informada."); return; }

            DateTime reativadoEm = base.CStringToDateTime(txtReativadoEm.Text);
            if (reativadoEm == DateTime.MinValue) { base.Alerta(null, this, "_err", "Data inválida."); txtReativadoEm.Focus(); return; }

            //DateTime? faltaDePagtoEm = base.CStringToDateTime(txtFaltaPagtoEM.Text);
            //if (faltaDePagtoEm == DateTime.MinValue) { faltaDePagtoEm = null; /* base.Alerta(null, this, "_err", "Data inválida."); txtFaltaPagtoEM.Focus(); return; */ }

            System.Collections.Hashtable ht = ContratoFacade.Instance.ReativaContratos(numeros, cboOperadoras.SelectedValue, reativadoEm, cboStatusMotivo.SelectedValue, txtObs.Text, Usuario.Autenticado.ID);

            if (ht != null)
            {
                System.Collections.Hashtable final = new System.Collections.Hashtable();
                txtPropostas.Text = "";
                List<String> arrNumeros = new List<String>();

                //List<ItemAgendaArquivoUnimed> itens = null;
                LC.Framework.Phantom.PersistenceManager pm = new LC.Framework.Phantom.PersistenceManager();
                pm.UseSingleCommandInstance();

                foreach (System.Collections.DictionaryEntry entry in ht)
                {
                    arrNumeros.Add(String.Concat(entry.Key, " - ", entry.Value));

                    ////guarda para gerar arquivo de exclusão de contrato
                    //if (trGerarArquivo.Visible && chkGerarArquivo.Checked && Convert.ToString(entry.Value).Trim() == "ok")
                    //{
                    //    if (itens == null) { itens = new List<ItemAgendaArquivoUnimed>(); }

                    //    ItemAgendaArquivoUnimed item = new ItemAgendaArquivoUnimed();
                    //    item.BeneficiarioID = ContratoBeneficiario.CarregaTitularID(Convert.ToString(entry.Key), cboOperadoras.SelectedValue, pm);
                    //    item.PropostaID = Contrato.CarregaContratoID(cboOperadoras.SelectedValue, Convert.ToString(entry.Key), pm);
                    //    item.Tipo = 6;
                    //    item.TipoDescricao = "CANCELAR CONTRATO";
                    //    pm.Save(item);
                    //}
                }

                pm.CloseSingleCommandInstance();
                pm.Dispose();

                arrNumeros.Reverse();
                foreach (String numero in arrNumeros)
                {
                    txtPropostas.Text += numero + Environment.NewLine;
                }
            }
        }

        protected void cboOperadoras_SelectedIndexChanged(Object sender, EventArgs e)
        {
            //if (cboOperadoras.SelectedValue == Convert.ToString(Operadora.UnimedID))
            //    trGerarArquivo.Visible = true;
            //else
            //    trGerarArquivo.Visible = false;
        }
    }
}

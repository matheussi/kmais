namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Util;
    using LC.Web.PadraoSeguros.Facade;
    using LC.Web.PadraoSeguros.Entity;

    using LC.Framework.Phantom;

    public partial class copiaContratoAdm : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false, false);
                this.carregaContratosAdm();
            }
        }

        void carregaContratosAdm()
        {
            cboContratoAdm.Items.Clear();
            cboContratoAdm.DataValueField = "ID";
            cboContratoAdm.DataTextField= "Descricao";
            cboContratoAdm.DataSource = ContratoADM.Carregar(cboOperadora.SelectedValue, txtParam1.Text, txtParam2.Text, txtParam3.Text, true);
            cboContratoAdm.DataBind();
        }

        protected void cboOperadora_changed(Object sender, EventArgs e)
        {
            this.carregaContratosAdm();
        }

        protected void imgFiltrar_click(Object sender, EventArgs e)
        {
            this.carregaContratosAdm();
        }

        protected void cmdModelo_click(Object sender, EventArgs e)
        {
            cboModeloSelecionado.Items.Clear();
            ListItem _sel = new ListItem(cboContratoAdm.SelectedItem.Text, cboContratoAdm.SelectedValue);
            cboModeloSelecionado.Items.Add(_sel);
        }

        protected void cmdDestino_click(Object sender, EventArgs e)
        {
            foreach (ListItem item in cboContratoAdm.Items)
            {
                if (item.Selected)
                {
                    ListItem _sel = new ListItem(item.Text, item.Value);

                    if (lstDestinosSelecionados.Items.FindByValue(_sel.Value) == null &&
                        cboModeloSelecionado.Items.FindByValue(_sel.Value) == null)
                    {
                        lstDestinosSelecionados.Items.Add(item);
                    }
                }
            }
        }

        protected void cmdReset_click(Object sender, EventArgs e)
        {
            cboModeloSelecionado.Items.Clear();
            lstDestinosSelecionados.Items.Clear();
        }

        protected void cmdCopiar_click(Object sender, EventArgs e)
        {
            String de = cboModeloSelecionado.SelectedValue;
            String[] para = base.PegaIDsSelecionados(lstDestinosSelecionados);

            if (String.IsNullOrEmpty(de) || para == null || para.Length == 0)
            {
                base.Alerta(null, this, "_err", "Todos os parâmetros são necessários.");
                return;
            }

            Boolean ok = CopiaCalendario_Plano_TabelaValor(de, para);

            if(ok)
                base.Alerta(null, this, "_ok", "Informações copiadas com sucesso.");
            else
                base.Alerta(null, this, "_err", "Houve um erro durante o processo.");
        }

        Boolean CopiaCalendario_Plano_TabelaValor(String contratoAdmID_De, String[] contratoAdmID_Para)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.IsoLevel = IsolationLevel.ReadUncommitted;
            pm.BeginTransactionContext();

            try
            {
                IList<CalendarioAdmissaoVigencia> cals = CalendarioAdmissaoVigencia.CarregarPorContrato(contratoAdmID_De, pm);
                CalendarioAdmissaoVigencia cav = cals[cals.Count - 1];
                IList<CalendarioVencimento> calVencto = CalendarioVencimento.CarregarTodos(cav.ID, pm);

                IList<TabelaValor> tabelas = TabelaValor.CarregarPorContratoID(contratoAdmID_De, pm);
                TabelaValor tabela = tabelas[0];
                IList<TabelaValorItem> itens = TabelaValorItem.CarregarPorTabela(tabela.ID, pm);
                Taxa taxa = Taxa.CarregarPorTabela(tabela.ID, pm);

                Plano plano = null;
                IList<Plano> planos = Plano.CarregarPorContratoID(tabela.ContratoID);

                IList<ContratoADM> contratos = LocatorHelper.Instance.ExecuteQuery<ContratoADM>("select contratoadm_id, contratoadm_descricao from contratoadm  where contratoadm_id in (" + String.Join(",", contratoAdmID_Para) + ")", typeof(ContratoADM), pm);

                foreach (ContratoADM contrato in contratos)
                {
                    NonQueryHelper.Instance.ExecuteNonQuery("delete from taxa where taxa_tabelavalorid in (select tabelavalor_id from tabela_valor where tabelavalor_contratoId=" + contrato.ID + ")", pm);
                    NonQueryHelper.Instance.ExecuteNonQuery("delete from tabela_valor_item where tabelavaloritem_tabelaid in (select tabelavalor_id from tabela_valor where tabelavalor_contratoId=" + contrato.ID + ")", pm);
                    NonQueryHelper.Instance.ExecuteNonQuery("delete from tabela_valor where tabelavalor_contratoid=" + contrato.ID, pm);

                    tabela.ID = null;
                    tabela.ContratoID = contrato.ID;
                    pm.Save(tabela);

                    taxa.ID = null;
                    taxa.TabelaValorID = tabela.ID;
                    pm.Save(taxa);

                    NonQueryHelper.Instance.ExecuteNonQuery("delete from plano where plano_contratoID=" + contrato.ID, pm);
                    foreach (Plano _plano in planos)
                    {
                        _plano.ID = null;
                        _plano.ContratoID = contrato.ID;
                        pm.Save(_plano);
                    }

                    foreach (TabelaValorItem item in itens)
                    {
                        plano = new Plano(item.PlanoID);
                        pm.Load(plano);
                        plano.ID = LocatorHelper.Instance.ExecuteScalar("select plano_id from plano where plano_descricao='" + plano.Descricao + "' and plano_contratoId=" + contrato.ID, null, null, pm);

                        item.ID = null;
                        item.PlanoID = plano.ID;
                        item.TabelaID = tabela.ID;
                        pm.Save(item);
                    }

                    #region calendario

                    NonQueryHelper.Instance.ExecuteNonQuery("delete from calendarioVencimento where calendariovencto_calendarioAdmissaoId in (select calendario_id from calendario where calendario_contratoid=" + contrato.ID + ")", pm);
                    NonQueryHelper.Instance.ExecuteNonQuery("delete from calendario where calendario_contratoid=" + contrato.ID, pm);

                    cav.ID = null;
                    cav.ContratoID = contrato.ID;
                    pm.Save(cav);

                    calVencto[0].ID = null;
                    calVencto[0].CalendarioAdmissaoID = cav.ID;
                    pm.Save(calVencto[0]);

                    #endregion
                }

                pm.Commit();
                return true;
            }
            catch
            {
                pm.Rollback();
                return false;
            }
            finally
            {
                pm.Dispose();
            }
        }
    }
}
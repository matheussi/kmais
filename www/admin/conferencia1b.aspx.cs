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

    public partial class conferencia1 : PageBaseConferencia
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                ucCPM.EscreveMensagens();
                ucConferenciaPassos.SetaPasso(1, "Fase 1");

                LeConferencia();
            }
        }

        Boolean ValidaCPF()
        {
            if (!UIHelper.ValidaCpf(txtCpf.Text))
            {
                ucCPM.SetaMsg("cpfTitular", "CPF do titular inválido.");
                return false;
            }
            else
            {
                ucCPM.RemoveMsg("cpfTitular");
                ucCPM.RemoveMsg("canc6meses");

                if (base.ConferenciaCorrente == null) { base.ConferenciaCorrente = new Conferencia(); }
                base.ConferenciaCorrente.TipoContratoExplicito = false;

                if (UIHelper.PerfilConferencia(Usuario.Autenticado.PerfilID))
                {
                    //Checa se há um contrato adimplente anterior. se sim, a venda é administrativa.
                    ContratoStatusHistorico entradaHistorico = null;
                    Contrato existente = Contrato.CarregarUltimoContratoDoBeneficiario(txtCpf.Text, ref entradaHistorico, true);
                    if (existente != null)
                    {
                        base.ConferenciaCorrente.TipoContratoExplicito = true;
                        base.ConferenciaCorrente.TipoContratoID = 4; //ADMINISTRATIVO
                    }
                    else
                    {
                        //checa se existe um contrato cancelado em até 6 meses, ou em mais de 6 meses
                        existente = Contrato.CarregarUltimoContratoDoBeneficiario(txtCpf.Text, ref entradaHistorico, false);

                        if (existente != null && existente.DataCancelamento != DateTime.MinValue)
                        {
                            DateTime propostaData = base.ConferenciaCorrente.PropostaData;
                            TimeSpan result = propostaData.Subtract(existente.DataCancelamento);
                            if (result.Days <= 180) //menor ou igual a 6 meses
                            {
                                int meses = result.Days / 30;
                                base.ConferenciaCorrente.TipoContratoExplicito = true;
                                base.ConferenciaCorrente.TipoContratoID = 4; //ADMINISTRATIVO

                                ucCPM.SetaMsg("canc6meses", "Há um contrato anterior cancelado em até " + meses.ToString() + " meses. Necessita antecipar cheque");
                            }
                            else if (result.Days > 180)
                            {
                                base.ConferenciaCorrente.TipoContratoExplicito = true;
                                base.ConferenciaCorrente.TipoContratoID = 1; //NOVO
                            }
                        }
                    }
                }
            }

            return true;
        }

        Boolean ValidaCEP()
        {
            String[] arr = base.PegaEndereco(this, txtCep, new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox(), false);
            litEndereco.Text = "";

            if (arr == null)
            {
                ucCPM.SetaMsg("cepProposta", "CEP não encontrado.");
                return false;
            }
            else
            {
                litEndereco.Text = arr[3] + " - " + arr[2] + "<br>" + arr[1] + " - " + arr[0];
                ucCPM.RemoveMsg("cepProposta");
                return true;
            }
        }

        void LeConferencia()
        {
            Conferencia conferencia = base.ConferenciaCorrente;
            if (conferencia == null) { return; }

            txtCep.Text = conferencia.CEP;
            txtCpf.Text = conferencia.TitularCPF;
            txtNomeTitular.Text = conferencia.TitularNome;
        }

        void PersisteConferencia()
        {
            Conferencia conferencia = base.ConferenciaCorrente;
            if (conferencia == null) { conferencia = new Conferencia(); }

            conferencia.CEP = txtCep.Text;
            conferencia.TitularCPF = txtCpf.Text;
            conferencia.TitularNome = txtNomeTitular.Text;

            conferencia.Salvar();
            base.ConferenciaCorrente = conferencia;
        }

        protected void cmdValidarCpf_Click(Object sender, EventArgs e)
        {
            this.ValidaCPF();
        }

        protected void cmdValidarCep_Click(Object sender, EventArgs e)
        {
            this.ValidaCEP();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("conferenciaLista.aspx");
        }

        protected void cmdProximo_Click(Object sender, EventArgs e)
        {
            Boolean ok = true;
            if (!this.ValidaCPF()) { ok = false; }
            if (!this.ValidaCEP()) { ok = false; }

            if (txtNomeTitular.Text.Trim() == "")
            {
                ucCPM.SetaMsg("nomeTitular", "Informe o nome do titular.");
                ok = false;
            }
            else
                ucCPM.RemoveMsg("nomeTitular");

            this.PersisteConferencia();

            if (ok) { Response.Redirect("conferencia2.aspx"); }
        }
    }
}
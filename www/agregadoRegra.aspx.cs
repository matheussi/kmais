namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class agregadoRegra : PageBase
    {
        protected override void  OnLoad(EventArgs e)
        {
 	        base.OnLoad(e);
            txtValor.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");

            if (!IsPostBack)
            {
                base.ExibirOperadoras(cboOperadora, false);
                this.CarregaContratos();
                this.CarregaTipoRestricao();
                if (base.IDKeyParameterInProcess(ViewState, "_agreg"))
                {
                    this.CarregaRegra();
                }
            }
        }

        void CarregaTipoRestricao()
        {
            cboRestricao.Items.Add(new ListItem("Idade limite de", "0"));
            cboRestricao.Items.Add(new ListItem("Dias decorridos do contrato", "1"));
        }

        void CarregaContratos()
        {
            cboContrato.Items.Clear();
            if (cboOperadora.Items.Count == 0) { return; }
            cboContrato.DataValueField = "ID";
            cboContrato.DataTextField = "Descricao";
            cboContrato.DataSource = ContratoADM.Carregar(cboOperadora.SelectedValue);
            cboContrato.DataBind();
        }

        void CarregaRegra()
        {
            AgregadoRegra regra = new AgregadoRegra(ViewState[IDKey]);
            regra.Carregar();

            ContratoADM contrato = new ContratoADM(regra.ContratoAdmID);
            contrato.Carregar();

            cboOperadora.SelectedValue = Convert.ToString(contrato.OperadoraID);
            cboOperadora_SelectedIndexChanged(null, null);

            cboContrato.SelectedValue = Convert.ToString(contrato.ID);

            cboRestricao.SelectedValue = regra.TipoLimite.ToString();
            txtValor.Text = regra.ValorLimite.ToString();

            if (((Parentesco.eTipo)regra.TipoAgregado) == Parentesco.eTipo.Dependente)
                optDependente.Checked = true;
            else
                optAgregado.Checked = true;
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/agregadosRegra.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region Validacoes 

            if (cboContrato.Items.Count == 0)
            {
                base.Alerta(null, this, "_err0", "Não há um contrato selecionado.");
                return;
            }

            #endregion

            AgregadoRegra regra = new AgregadoRegra(ViewState[IDKey]);
            regra.ContratoAdmID = cboContrato.SelectedValue;

            Parentesco.eTipo tipo = Parentesco.eTipo.Agregado;
            if (optDependente.Checked) { tipo = Parentesco.eTipo.Dependente; }
            regra.TipoAgregado = Convert.ToInt32(tipo);
            regra.TipoLimite = Convert.ToInt32(cboRestricao.SelectedValue);
            regra.ValorLimite = base.CToInt(txtValor.Text);

            regra.Salvar();
            ViewState[IDKey] = regra.ID;

            base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
        }
    }
}

namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class agregado : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirEstipulantes(cboEstipulante, false, true);
                base.ExibirOperadoras(cboOperadora, false);
                this.CarregaContratos();

                if (base.IDKeyParameterInProcess(ViewState, "_agreg"))
                {
                    this.CarregaAgregado();
                }
                else
                {
                    if (Request[IDKey2] != null)
                    {
                        cboEstipulante.SelectedValue = Request[IDKey2];
                        cboEstipulante_SelectedIndexChanged(null, null);
                    }
                    if (Request[IDKey3] != null)
                    {
                        cboOperadora.SelectedValue = Request[IDKey3];
                        cboOperadora_SelectedIndexChanged(null, null);
                    }
                    if (Request[IDKey4] != null)
                    {
                        cboContrato.SelectedValue = Request[IDKey4];
                    }
                }
            }
        }

        void CarregaContratos()
        {
            cboContrato.Items.Clear();

            if (cboOperadora.Items.Count == 0 || cboEstipulante.Items.Count == 0) { return; }
            IList<ContratoADM> contratos = ContratoADM.Carregar(cboEstipulante.SelectedValue, cboOperadora.SelectedValue);
            cboContrato.DataValueField = "ID";
            cboContrato.DataTextField = "Descricao";
            cboContrato.DataSource = contratos;
            cboContrato.DataBind();
        }

        void CarregaAgregado()
        {
            ContratoADMParentescoAgregado agregado = new ContratoADMParentescoAgregado();
            agregado.ID = ViewState[IDKey];
            agregado.Carregar();

            ContratoADM contrato = new ContratoADM(agregado.ContratoAdmID);
            contrato.Carregar();

            cboEstipulante.SelectedValue = Convert.ToString(contrato.EstipulanteID);
            cboEstipulante_SelectedIndexChanged(null, null);

            cboOperadora.SelectedValue = Convert.ToString(contrato.OperadoraID);
            cboOperadora_SelectedIndexChanged(null, null);

            cboContrato.SelectedValue = Convert.ToString(agregado.ContratoAdmID);

            optAgregado.Checked = ((Parentesco.eTipo)agregado.ParentescoTipo) == Parentesco.eTipo.Agregado;
            optDependente.Checked = ((Parentesco.eTipo)agregado.ParentescoTipo) == Parentesco.eTipo.Dependente;

            txtCodigo.Text    = agregado.ParentescoCodigo;
            txtDescricao.Text = agregado.ParentescoDescricao;
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
        }

        protected void cboEstipulante_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaContratos();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect(String.Concat("~/agregados.aspx?", IDKey, "=", cboEstipulante.SelectedValue, "&", IDKey2, "=", cboOperadora.SelectedValue, "&", IDKey3, "=", cboContrato.SelectedValue));
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validacoes

            if (cboContrato.Items.Count == 0)
            {
                //base.Alerta(null, this, "_err0", "Não há um contrato administrativo selecionado.");
                base.Alerta(MPE, ref litAlert, "Informe o nome da filial.", upnlAlerta);
                cboContrato.Focus();
                return;
            }

            if (txtDescricao.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_err1", "Informe uma descrição.");
                base.Alerta(MPE, ref litAlert, "Informe uma descrição.", upnlAlerta);
                txtDescricao.Focus();
                return;
            }

            //if (txtCodigo.Text.Trim() == "")
            //{
            //    base.Alerta(null, this, "_err2", "Informe um código.");
            //    txtCodigo.Focus();
            //    return;
            //}

            #endregion

            ContratoADMParentescoAgregado agregado = new ContratoADMParentescoAgregado();
            agregado.ContratoAdmID = cboContrato.SelectedValue;
            agregado.ID = ViewState[IDKey];
            agregado.ParentescoDescricao = txtDescricao.Text;
            agregado.ParentescoCodigo = txtCodigo.Text;

            Parentesco.eTipo tipo = Parentesco.eTipo.Agregado;
            if(optDependente.Checked) { tipo = Parentesco.eTipo.Dependente; }
            agregado.ParentescoTipo = (int)tipo;

            if (ContratoADMParentescoAgregado.Duplicado(agregado))
            {
                base.Alerta(MPE, ref litAlert, "Descrição já existente para esse contrato administrativo.", upnlAlerta);
                txtDescricao.Focus();
                return;
            }

            agregado.Salvar();
            ViewState[IDKey] = agregado.ID;

            //base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
        }
    }
}

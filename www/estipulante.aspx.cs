namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class estipulante : PageBase
    {
        Object estipulanteID
        {
            get
            {
                if (ViewState[IDKey] != null)
                    return ViewState[IDKey];
                else
                    return Request[IDKey];
            }
            set
            {
                ViewState[IDKey] = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtTaxa.Attributes.Add("onKeyUp", "mascara('" + txtTaxa.ClientID + "')");
            if (!IsPostBack)
            {
                UIHelper.AuthCtrl(cmdAdd, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                UIHelper.AuthCtrl(cmdSalvar, Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);

                base.ExibirTipoDeTaxaEstipulante(cboTipoTaxa, false);
                if (estipulanteID != null) { this.CarregaEstipulante(); }
                else { cmdAdd.Visible = false; }
                txtDescricao.Focus();
            }
        }

        void CarregaEstipulante()
        {
            Estipulante estipulante = new Estipulante();
            estipulante.ID = estipulanteID;
            estipulante.Carregar();

            txtDescricao.Text = estipulante.Descricao;
            txtTextoBoleto.Text = estipulante.TextoBoleto;
            chkAtiva.Checked = estipulante.Ativo;

            this.CarregaTaxas();
        }

        void CarregaTaxas()
        {
            IList<EstipulanteTaxa> lista = EstipulanteTaxa.CarregarTodas(estipulanteID);
            gridTaxas.DataSource = lista;
            gridTaxas.DataBind();

            if (lista != null && lista.Count > 0)
                gridTaxas.Rows[0].Font.Bold = true;
        }

        protected void cmdAdd_Click(Object sender, EventArgs e)
        {
            #region validacoes 
            if (base.CStringToDateTime(txtVigencia.Text) == DateTime.MinValue)
            {
                //base.Alerta(null, this, "__errT", "Informe a data de vigência.");
                base.Alerta(MPE, ref litAlert, "Informe a data de vigência.", upnlAlerta);
                txtVigencia.Focus();
                return;
            }
            //if (base.CToDecimal(txtTaxa.Text) == 0)
            //{
            //    base.Alerta(null, this, "__errT", "Informe o valor da taxa.");
            //    txtTaxa.Focus();
            //    return;
            //}
            #endregion

            EstipulanteTaxa et = new EstipulanteTaxa();
            et.EstipulanteID = estipulanteID;
            et.TipoTaxa = Convert.ToInt32(cboTipoTaxa.SelectedValue);
            et.Valor = base.CToDecimal(txtTaxa.Text);
            et.Vigencia = base.CStringToDateTime(txtVigencia.Text);
            et.Salvar();

            this.CarregaTaxas();
            txtTaxa.Text = "";
            txtVigencia.Text = "";
        }

        protected void gridTaxas_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                Object id = gridTaxas.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                EstipulanteTaxa obj = new EstipulanteTaxa(id);
                obj.Remover();
                this.CarregaTaxas();
            }
        }

        protected void gridTaxas_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                UIHelper.AuthCtrl(e.Row.Cells[3].Controls[0], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
                base.grid_RowDataBound_Confirmacao(sender, e, 3, "Deseja realmente excluir?");
            }
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/estipulantes.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (txtDescricao.Text.Trim() == "")
            {
                //base.Alerta(null, this, "__err", "Informe o nome do estipulante.");
                base.Alerta(MPE, ref litAlert, "Informe o nome do estipulante.", upnlAlerta);
                txtDescricao.Focus();
                return;
            }

            if (Estipulante.Duplicado(estipulanteID, txtDescricao.Text.Trim()))
            {
                base.Alerta(MPE, ref litAlert, "Já existe um estipulante com essa descrição.", upnlAlerta);
                txtDescricao.Focus();
                return;
            }

            Estipulante estipulante = new Estipulante();
            estipulante.ID = estipulanteID;
            estipulante.Descricao = txtDescricao.Text.Trim();
            estipulante.TextoBoleto = txtTextoBoleto.Text;
            estipulante.Ativo = chkAtiva.Checked;
            estipulante.Salvar();
            ViewState[IDKey] = estipulante.ID;

            if (!cmdAdd.Visible && base.CStringToDateTime(txtVigencia.Text) != DateTime.MinValue)
            {
                cmdAdd_Click(cmdAdd, null);
            }

            cmdAdd.Visible = true;
            //base.Alerta(null, this, "__ok", "Dados salvos com sucesso.");
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
        }
    }
}
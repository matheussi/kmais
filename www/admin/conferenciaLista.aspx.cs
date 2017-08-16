namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class conferenciaLista : PageBaseConferencia
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                Session["___msg"] = null; 
                base.ExibirDepartamentos(cboNovoEstagio, Usuario.Autenticado.PerfilID);

                base.ConferenciaCorrente = null;
                base.ExibirOperadoras(cboOperadora, false);
                this.CarregaConferencias();
                if (!UIHelper.PerfilConferencia(Usuario.Autenticado.PerfilID) &&
                    !UIHelper.PerfilADMIN(Usuario.Autenticado.PerfilID) &&
                    !UIHelper.PerfilCadastro(Usuario.Autenticado.PerfilID))
                {
                    cmdNovo.Visible = false;
                }
            }
        }

        void CarregaConferencias()
        {
            Object operadoraId = null;
            if(base.HaItemSelecionado(cboOperadora)) { operadoraId = cboOperadora.SelectedValue; }

            cmdNovo.Visible = true;
            if (UIHelper.PerfilConferencia(Usuario.Autenticado.PerfilID))
            {
                grid.DataSource = Conferencia.Carregar(operadoraId, txtNum.Text, new ContratoStatusHistorico.eStatus[] { ContratoStatusHistorico.eStatus.EmConferencia, ContratoStatusHistorico.eStatus.ComCorretor });
                grid.Columns[5].Visible = true;
                grid.Columns[6].Visible = true;
                grid.Columns[7].Visible = false;
            }
            else if (UIHelper.PerfilCadastro(Usuario.Autenticado.PerfilID) || UIHelper.PerfilADMIN(Usuario.Autenticado.PerfilID))
            {
                grid.DataSource = Conferencia.Carregar(operadoraId, txtNum.Text, new ContratoStatusHistorico.eStatus[] { ContratoStatusHistorico.eStatus.NoCadastro });
                grid.Columns[5].Visible = false;
                grid.Columns[6].Visible = false;
                grid.Columns[7].Visible = true;
            }
            else //if (UIHelper.PerfilADMIN(Usuario.Autenticado.PerfilID))
            {
                //grid.DataSource = Conferencia.Carregar(operadoraId, new ContratoStatusHistorico.eStatus[] { ContratoStatusHistorico.eStatus.EmConferencia, ContratoStatusHistorico.eStatus.ComCorretor });
                //grid.Columns[5].Visible = true;
                //grid.Columns[6].Visible = true;
                //grid.Columns[7].Visible = false;
                cmdNovo.Visible = false;
            }

            grid.DataBind();
            if (grid.Rows.Count == 0)
                tblNovoEstagio.Visible = false;
            else
                tblNovoEstagio.Visible = true;
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaConferencias();
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            this.CarregaConferencias();
        }

        protected void chkCheckHeader_Change(Object sender, EventArgs e)
        {
            Boolean mainValue = ((CheckBox)grid.HeaderRow.Cells[0].Controls[1]).Checked;
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                ((CheckBox)grid.Rows[i].Cells[0].Controls[1]).Checked = mainValue;
            }
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Conferencia conferencia = new Conferencia(grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                conferencia.Carregar();
                base.ConferenciaCorrente = conferencia;
                Response.Redirect("~/admin/conferencia1.aspx?");
            }
            else if (e.CommandName.Equals("excluir"))
            {
                try
                {
                    Conferencia conferencia = new Conferencia(grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                    conferencia.Remover();
                    grid.PageIndex = 0;
                    this.CarregaConferencias();
                }
                catch
                {
                    base.Alerta(null, this, "_errExcl", "Não foi possível excluir.");
                }
            }
            else if (e.CommandName.Equals("cadastrar"))
            {
                Conferencia conferencia = new Conferencia(grid.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                conferencia.Carregar();
                Session[ConferenciaObjKey] = conferencia;
                Response.Redirect("~/contrato.aspx");
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                base.grid_RowDataBound_Confirmacao(sender, e, 5, "Deseja prosseguir com a exclusão?");
                if (e.Row.Cells[3].Text == "01/01/0001")
                {
                    e.Row.Cells[3].Text = "-------------";
                }
                else
                {
                    DateTime data = base.CStringToDateTime(e.Row.Cells[2].Text);
                    if (data < DateTime.Now && data != DateTime.MinValue)
                        e.Row.ForeColor = System.Drawing.Color.Red;
                }
                switch (e.Row.Cells[4].Text)
                {
                    case "3":
                    {
                        e.Row.Cells[4].Text = "Conferência";
                        break;
                    }
                    case "4":
                    {
                        e.Row.Cells[4].Text = "Cadastro";
                        break;
                    }
                    default:
                    {
                        e.Row.Cells[4].Text = "Corretor";
                        break;
                    }
                }
            }
        }

        protected void cmdNovoEstagio_Click(Object sender, EventArgs e)
        {
            List<String> ids = null; List<String> numeros = null;
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (((CheckBox)grid.Rows[i].Cells[0].Controls[1]).Checked)
                {
                    if (ids == null) { ids = new List<String>(); numeros = new List<String>(); }
                    ids.Add(Convert.ToString(grid.DataKeys[i].Value));
                    numeros.Add(grid.Rows[i].Cells[1].Text);
                }
            }

            if (ids != null)
            {
                try
                {
                    if (Conferencia.AlteraEstagio(cboOperadora.SelectedValue, ids, numeros, ((ContratoStatusHistorico.eStatus)Convert.ToInt32(cboNovoEstagio.SelectedValue))))
                    {
                        base.Alerta(null, this, "_ok", "Você alterou o estágio das propostas selecionadas.");
                    }
                    else
                    {
                        base.Alerta(null, this, "_ok", "Um ou mais contratos selecionados já estão devidamente cadastrados no sistema.");
                    }
                    this.CarregaConferencias();
                }
                catch
                {
                    base.Alerta(null, this, "_errUnex", "Não foi possível realizar a operação.");
                }
            }
            else
                base.Alerta(null, this, "_err", "Você deve selecionar ao menos uma proposta.");
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            base.ConferenciaCorrente = null;
            Response.Redirect("conferencia1.aspx");
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.CarregaConferencias();
        }
    }
}
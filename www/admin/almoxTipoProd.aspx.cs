namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class almoxTipoProd : PageBase
    {
        String idProduto
        {
            get
            {
                if (Request[IDKey] == null) { return null; }
                return Convert.ToString(Request[IDKey]);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.ExibirFiliais(cboFilial, false);
                base.ExibirOperadoras(cboOperadora, false);
                cboOperadora.Items.Insert(0, new ListItem("sem vínculo", "-1"));
                this.CarregaTiposDeProduto();

                if (idProduto != null) { this.CarregaProduto(); txtQtdMin.Focus(); }
                else { txtTipo.Focus(); }
                if (cboOperadora.Items.Count > 1) { cboOperadora.SelectedIndex = 1; }
            }
        }

        void CarregaProduto()
        {
            AlmoxProduto prod = new AlmoxProduto();
            prod.ID = idProduto;
            prod.Carregar();

            if (prod.ID == null) { Response.Redirect("~/admin/almoxTiposProd.aspx"); }

            cboTipo.SelectedValue = Convert.ToString(prod.TipoProdutoID);
            cboTipo_SelectedIndexChanged(cboTipo, null);
            //txtDescricao.Text = prod.Descricao;
            txtQtd.Text    = Convert.ToString(prod.QTD);
            txtQtdMin.Text = Convert.ToString(prod.QTDMin);
            //txtQtdMax.Text = Convert.ToString(prod.QTDMax);

            if (prod.OperadoraID != null)
                cboOperadora.SelectedValue = Convert.ToString(prod.OperadoraID);
            else
                cboOperadora.SelectedIndex = 0;

            cboFilial.SelectedValue = Convert.ToString(prod.FilialID);
        }

        void CarregaTiposDeProduto()
        {
            IList<AlmoxTipoProduto> lista = AlmoxTipoProduto.CarregarTodos();
            cboTipo.DataValueField = "ID";
            cboTipo.DataTextField = "Descricao";
            cboTipo.DataSource = lista;
            cboTipo.DataBind();

            cboTipo.Items.Insert(0, new ListItem("novo ->", "-1"));
        }

        protected void cboTipo_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (cboTipo.SelectedIndex > 0)
            {
                chkControlado.Enabled = false;

                AlmoxTipoProduto obj = new AlmoxTipoProduto();
                obj.ID = cboTipo.SelectedValue;
                obj.Carregar();

                chkControlado.Checked = obj.Numerado;
                cboFilial.Focus();
            }
            else
            {
                chkControlado.Checked = false;
                chkControlado.Enabled = true;
                txtTipo.Focus();
            }
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/almoxTiposProd.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (cboFilial.Items.Count == 0)
            {
                base.Alerta(MPE, ref litAlert, "Não há uma filial selecionada.", upnlAlerta);
                cboFilial.Focus();
                return;
            }

            if (cboOperadora.Items.Count == 0)
            {
                //base.Alerta(null, this, "__err", "Não há uma operadora selecionada.");
                base.Alerta(MPE, ref litAlert, "Não há uma operadora selecionada.", upnlAlerta);
                cboOperadora.Focus();
                return;
            }

            //if (txtDescricao.Text.Trim() == "")
            //{
            //    //base.Alerta(null, this, "__err0", "Informe a descrição.");
            //    base.Alerta(MPE, ref litAlert, "Informe a descrição.", upnlAlerta);
            //    txtDescricao.Focus();
            //    return;
            //}

            if (txtQtdMin.Text.Trim() == "")
            {
                base.Alerta(MPE, ref litAlert, "Informe a quantidade mínima.", upnlAlerta);
                return;
            }

            AlmoxProduto prod = new AlmoxProduto();
            if (idProduto != null)
            {
                prod.ID = idProduto;
                prod.Carregar();
            }

            String tipoId = "";
            if (cboTipo.SelectedIndex <= 0)
            {
                if (txtTipo.Text.Trim() == "")
                {
                    //base.Alerta(null, this, "__err1", "Informe o tipo de produto.");
                    base.Alerta(MPE, ref litAlert, "Informe o tipo de produto.", upnlAlerta);
                    txtTipo.Focus();
                    return;
                }
                else if (cboTipo.Items.FindByText(txtTipo.Text.ToUpper()) != null)
                {
                    base.Alerta(MPE, ref litAlert, "O tipo de produto informado já existe.", upnlAlerta);
                    txtTipo.Focus();
                    return;
                }

                AlmoxTipoProduto obj = new AlmoxTipoProduto();
                obj.Descricao = txtTipo.Text;
                obj.Numerado = chkControlado.Checked;
                obj.Salvar();

                tipoId = Convert.ToString(obj.ID);
                prod.Descricao = obj.Descricao;
            }
            else
            {
                tipoId = cboTipo.SelectedValue;
                prod.Descricao = cboTipo.SelectedItem.Text;
            }

            //prod.Descricao = txtDescricao.Text;
            if (base.HaItemSelecionado(cboOperadora))
            {
                prod.OperadoraID = cboOperadora.SelectedValue;

                if (cboTipo.SelectedIndex > 0)
                {
                    if (AlmoxProduto.ExisteProdutoParaEsteTipoOperadoraFilial(cboTipo.SelectedValue, cboOperadora.SelectedValue, cboFilial.SelectedValue, prod.ID))
                    {
                        base.Alerta(MPE, ref litAlert, "Esse produto já está cadastrado para essa operadora e nessa filial.", upnlAlerta);
                        return;
                    }
                }
            }
            else
            {
                prod.OperadoraID = null;
                if (cboTipo.SelectedIndex > 0)
                {
                    if (AlmoxProduto.ExisteProdutoParaEsteTipoOperadoraFilial(cboTipo.SelectedValue, null, cboFilial.SelectedValue, prod.ID))
                    {
                        base.Alerta(MPE, ref litAlert, "Esse produto já está cadastrado para essa filial.", upnlAlerta);
                        return;
                    }
                }
            }

            //prod.QTDMax = base.CToInt(txtQtdMax.Text);
            prod.FilialID = cboFilial.SelectedValue;
            prod.QTDMin = base.CToInt(txtQtdMin.Text);
            prod.TipoProdutoID = tipoId;
            prod.UsuarioID = Usuario.Autenticado.ID;
            prod.Salvar();
            //base.Alerta(null, this, "__ok", "Dados salvos com sucesso.");
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
        }
    }
}
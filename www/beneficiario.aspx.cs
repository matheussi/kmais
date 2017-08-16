namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class beneficiario : PageBase
    {
        //IList<Endereco> Enderecos
        //{
        //    get { return ViewState["_end"] as IList<Endereco>; }
        //    set { ViewState["_end"] = value; }
        //}

        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);

        //    if (!IsPostBack)
        //    {
        //        base.ExibirOpcoesDeSexo(cboSexo, false);
        //        base.ExibirEstadosCivis(cboEstadoCivil, false);

        //        if (base.IDKeyParameterInProcess(ViewState, "_beneficiario"))
        //        {
        //            this.CarregaBeneficiario();
        //            this.CarregaEnderecos();
        //        }
        //        else
        //            spanEnderecosCadastrados.Visible = false;
        //    }
        //}

        //void CarregaBeneficiario()
        //{
        //    Beneficiario beneficiario = new Beneficiario();
        //    beneficiario.ID = ViewState[IDKey];
        //    beneficiario.Carregar();

        //    if (beneficiario.ID != null)
        //    {
        //        txtCelular.Text = PegaTelefone(beneficiario.Celular);
        //        txtCPF.Text = beneficiario.CPF;

        //        if (beneficiario.DataNascimento != DateTime.MinValue)
        //            txtDataNascimento.Text = beneficiario.DataNascimento.ToString("dd/MM/yyyy");
        //        txtDDD1.Text = PegaDDD(beneficiario.Telefone);
        //        txtDDD2.Text = PegaDDD(beneficiario.Telefone2);
        //        txtDDDCelular.Text = PegaDDD(beneficiario.Celular);
        //        txtEmail.Text = beneficiario.Email;
        //        txtFone1.Text = PegaTelefone(beneficiario.Telefone);
        //        txtFone2.Text = PegaTelefone(beneficiario.Telefone2);
        //        txtNome.Text = beneficiario.Nome;
        //        txtNomeMae.Text = beneficiario.NomeMae;
        //        txtRamal1.Text = beneficiario.Ramal;
        //        txtRamal2.Text = beneficiario.Ramal2;
        //        txtRG.Text = beneficiario.RG;

        //        if (beneficiario.Sexo != null)
        //            cboSexo.SelectedValue = beneficiario.Sexo;
        //    }
        //    else
        //        ViewState[IDKey] = null;
        //}

        //void CarregaEnderecos()
        //{
        //    IList<Endereco> enderecos = Endereco.
        //        CarregarPorDono(ViewState[IDKey], Endereco.TipoDono.Beneficiario);
        //    this.Enderecos = enderecos;

        //    gridEnderecos.DataSource = enderecos;
        //    gridEnderecos.DataBind();

        //    spanEnderecosCadastrados.Visible = enderecos != null && enderecos.Count > 0;
        //}

        //protected void cmdVoltar_Click(Object sender, EventArgs e)
        //{
        //    Response.Redirect("beneficiarios.aspx");
        //}

        //protected void cmdSalvar_Click(Object sender, EventArgs e)
        //{
        //    Beneficiario beneficiario = new Beneficiario();
        //    beneficiario.ID = ViewState[IDKey];
        //    beneficiario.Celular = "(" + txtDDDCelular.Text + ") " + txtCelular.Text;
        //    beneficiario.CPF = txtCPF.Text;
        //    beneficiario.DataNascimento = base.CStringToDateTime(txtDataNascimento.Text);
        //    beneficiario.Email = txtEmail.Text;
        //    beneficiario.EstadoCivilID = cboEstadoCivil.SelectedValue;
        //    beneficiario.Nome = txtNome.Text;
        //    beneficiario.NomeMae = txtNomeMae.Text;
        //    beneficiario.RG = txtRG.Text;
        //    beneficiario.Telefone = "(" + txtDDD1.Text + ") " + txtFone1.Text;
        //    beneficiario.Telefone2 = "(" + txtDDD2.Text + ") " + txtFone2.Text;
        //    beneficiario.Ramal = txtRamal1.Text;
        //    beneficiario.Ramal2 = txtRamal2.Text;
        //    beneficiario.Sexo = cboSexo.SelectedValue;
        //    beneficiario.Salvar();

        //    ViewState[IDKey] = beneficiario.ID;

        //    if (this.Enderecos != null)
        //    {
        //        IList<Endereco> enderecos = this.Enderecos;

        //        foreach(Endereco endereco in enderecos)
        //        {
        //            endereco.DonoId = beneficiario.ID;
        //            endereco.DonoTipo = (Int32)Endereco.TipoDono.Beneficiario;
        //            endereco.Salvar();
        //        }

        //        this.Enderecos = enderecos;
        //    }
        //}

        //protected void cmdAddEndereco_Click(Object sender, EventArgs e)
        //{
        //    Endereco endereco = null;

        //    IList<Endereco> enderecos = this.Enderecos;
        //    if (enderecos == null) { enderecos = new List<Endereco>(); }

        //    if (gridEnderecos.SelectedIndex == -1)
        //        endereco = new Endereco();
        //    else
        //        endereco = enderecos[gridEnderecos.SelectedIndex];

        //    endereco.Bairro = txtBairro.Text;
        //    endereco.CEP = txtCEP.Text;
        //    endereco.Cidade = txtCidade.Text;
        //    endereco.Complemento = txtComplemento.Text;
        //    endereco.DonoTipo = Convert.ToInt32(Endereco.TipoDono.Beneficiario);
        //    endereco.Logradouro = txtLogradouro.Text;
        //    endereco.Numero = txtNumero.Text;
        //    endereco.Tipo = Convert.ToInt32(cboTipoEndereco.SelectedValue);
        //    endereco.UF = txtUF.Text;

        //    if (gridEnderecos.SelectedIndex == -1)
        //        enderecos.Add(endereco);
        //    else
        //        enderecos[gridEnderecos.SelectedIndex] = endereco;

        //    this.Enderecos = enderecos;

        //    gridEnderecos.DataSource = enderecos;
        //    gridEnderecos.DataBind();
        //    spanEnderecosCadastrados.Visible = true;

        //    txtCEP.Text = "";
        //    txtLogradouro.Text = "";
        //    txtNumero.Text = "";
        //    txtComplemento.Text = "";
        //    txtBairro.Text = "";
        //    txtCidade.Text = "";
        //    txtUF.Text = "";
        //    cboTipoEndereco.SelectedIndex = 0;

        //    gridEnderecos.SelectedIndex = -1;
        //}

        //protected void gridEnderecos_RowCommand(Object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName.Equals("alterar"))
        //    {
        //        int indice = Convert.ToInt32(e.CommandArgument);

        //        gridEnderecos.SelectedIndex = indice;

        //        Endereco endereco = this.Enderecos[indice];
        //        txtBairro.Text = endereco.Bairro;
        //        txtCEP.Text = endereco.CEP;
        //        txtCidade.Text = endereco.Cidade;
        //        txtComplemento.Text = endereco.Complemento;
        //        txtLogradouro.Text = endereco.Logradouro;
        //        txtNumero.Text = endereco.Numero;
        //        txtUF.Text = endereco.UF.ToUpper();
        //    }
        //    else if (e.CommandName.Equals("excluir"))
        //    {
        //        gridEnderecos.SelectedIndex = -1;
        //        Object id = gridEnderecos.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
        //        if (id == null)
        //        {
        //            IList<Endereco> lista = this.Enderecos;
        //            lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
        //            this.Enderecos = lista;
        //            gridEnderecos.DataSource = lista;
        //            gridEnderecos.DataBind();
        //        }
        //        else
        //        {
        //            Endereco endereco = new Endereco();
        //            endereco.ID = id;
        //            endereco.Remover();
        //            this.CarregaEnderecos();
        //        }
        //    }
        //}

        //protected void gridEnderecos_RowDataBound(Object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente excluir o endereço\\n\\n" + e.Row.Cells[1].Text.Replace("'", "`") + " ?\\n\\nEssa operação não poderá ser desfeita.");

        //        if (e.Row.Cells[2].Text == "0")
        //            e.Row.Cells[2].Text = "RESIDENCIAL";
        //        else
        //            e.Row.Cells[2].Text = "COBRANCA";
        //    }
        //}
    }
}
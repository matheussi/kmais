namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class declaracaoSaudeRegras : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtIdade.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");

            if (!IsPostBack)
            {
                base.ExibirOpcoesDeSexo(cboSexo, false);
                base.ExibirOperadoras(cboOperadora, true);

                if (base.IDKeyParameterInProcess(ViewState, "regra"))
                {
                    this.CarregaRegra();
                }
            }
        }

        void CarregaRegra()
        {
            RegraDeclaracaoSaude regra = new RegraDeclaracaoSaude();
            regra.ID = ViewState[IDKey];
            regra.Carregar();

            if (regra.Idade != -1)
            {
                chkIdade.Checked = true;
                txtIdade.Text = regra.Idade.ToString();
                cboIdadeOperador.SelectedValue = regra.OperadorIdade;
            }

            if (regra.SexoID != -1)
            {
                chkSexo.Checked = true;
                cboSexo.SelectedValue = Convert.ToString(regra.SexoID) ;
            }

            txtDescricao.Text = regra.Descricao;
            cboOperadora.SelectedValue = Convert.ToString(regra.OperadoraID);
            cboOperadora_SelectedIndexChanged(null, null);
            cboOperadora.Enabled = false;

            IList<ItemRegraDeclaracaoSaude> lista = ItemRegraDeclaracaoSaude.Carregar(regra.ID);

            if (lista != null)
            {
                foreach (ItemRegraDeclaracaoSaude item in lista)
                {
                    for (int i = 0; i < checks.Items.Count; i++)
                    {
                        if (Convert.ToString(item.ItemDeclaracaoID) == checks.Items[i].Value)
                        {
                            checks.Items[i].Selected = true;
                            break;
                        }
                    }
                }
            }
        }

        void CarregaItens()
        {
            if (cboOperadora.SelectedIndex > 0)
            {
                checks.DataValueField = "ID";
                checks.DataTextField = "Texto";
                checks.DataSource = ItemDeclaracaoSaude.Carregar(cboOperadora.SelectedValue);
                checks.DataBind();
            }
            else
            {
                checks.Items.Clear();
            }
        }

        protected void cboOperadora_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaItens();
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("declaracaoSaudeRegrasLista.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            #region validações 

            if (cboOperadora.SelectedIndex <= 0)
            {
                //base.Alerta(null, this, "_op", "Selecione a operadora.");
                base.Alerta(MPE, ref litAlert, "Selecione a operadora.", upnlAlerta);
                return;
            }

            if (txtDescricao.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_descr", "Informe uma descrição para a regra.");
                base.Alerta(MPE, ref litAlert, "Informe uma descrição para a regra.", upnlAlerta);
                return;
            }

            if (!chkIdade.Checked && !chkSexo.Checked)
            {
                //base.Alerta(null, this, "_logica", "Uma regra deve checar a idade ou o sexo.");
                base.Alerta(MPE, ref litAlert, "Uma regra deve checar a idade ou o sexo.", upnlAlerta);
                return;
            }

            if (chkIdade.Checked && txtIdade.Text.Trim() == "")
            {
                //base.Alerta(null, this, "_semIdade", "Informe a idade.");
                base.Alerta(MPE, ref litAlert, "Informe a idade.", upnlAlerta);
                return;
            }
            else if (chkIdade.Checked)
            {
                int result = 0;
                if (!Int32.TryParse(txtIdade.Text, out result))
                {
                    //base.Alerta(null, this, "_idadeInv", "Idade inválida.");
                    base.Alerta(MPE, ref litAlert, "Informe a idade.", upnlAlerta);
                    return;
                }
            }

            if (checks.Items.Count == 0 || checks.SelectedIndex == -1)
            {
                //base.Alerta(null, this, "_qu", "Selecione ao menos um item da ficha de saúde.");
                base.Alerta(MPE, ref litAlert, "Selecione ao menos um item da ficha de saúde.", upnlAlerta);
                return;
            }
            #endregion

            RegraDeclaracaoSaude regra = new RegraDeclaracaoSaude();
            regra.ID = ViewState[IDKey];
            regra.OperadoraID = cboOperadora.SelectedValue;
            regra.Descricao = txtDescricao.Text;

            if (chkIdade.Checked)
            {
                regra.Idade = Convert.ToInt32(txtIdade.Text);
                regra.OperadorIdade = cboIdadeOperador.SelectedValue;
            }

            if (chkSexo.Checked)
            {
                regra.SexoID = Convert.ToInt32(cboSexo.SelectedValue);
            }

            regra.Salvar();

            ViewState[IDKey] = regra.ID;

            for (int i = 0; i < checks.Items.Count; i++)
            {
                ItemRegraDeclaracaoSaude item = ItemRegraDeclaracaoSaude.Carregar(regra.ID, checks.Items[i].Value);

                if (checks.Items[i].Selected && item == null) //se está ticado e nao existe no banco, salva
                {
                    item = new ItemRegraDeclaracaoSaude();
                    item.RegraDeclaracaoID = regra.ID;
                    item.ItemDeclaracaoID = checks.Items[i].Value;
                    item.Salvar();
                }
                else if (item != null && !checks.Items[i].Selected)//se nao esta ticado mas existe no banco, remove
                {
                    item.Remover();
                }
            }

            //base.Alerta(null, this, "_salvo", "Dados salvos com sucesso.");
            base.Alerta(MPE, ref litAlert, "Dados salvos com sucesso.", upnlAlerta);
        }
    }
}
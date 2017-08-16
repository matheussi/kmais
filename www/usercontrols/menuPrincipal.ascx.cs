namespace www.usercontrols
{
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class menuPrincipal : System.Web.UI.UserControl
    {
        protected override void  OnLoad(EventArgs e)
        {
 	        base.OnLoad(e);
            if (!IsPostBack)
            {
                this.SetupMenu();
            }
        }

        void SetupMenu()
        {
            if (!String.IsNullOrEmpty(Usuario.Autenticado.PerfilID))
            {
                switch (Usuario.Autenticado.PerfilID)
                {
                    case Perfil.SupervidorIDKey: //perfil supervisor
                    {
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems[1].ChildItems.Clear();

                        mnuMovimentacao.Items[0].ChildItems.Clear();

                        mnuAdministracao.Items[0].ChildItems.RemoveAt(1);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(1);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(1);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(1);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(1);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(1);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(1);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(1);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(1);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(1);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(1);

                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(1);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(1);
                        mnuFinanceiro.Items[0].ChildItems[1].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems[1].ChildItems.RemoveAt(3);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(3);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(2);
                        //mnuFinanceiro.Items[0].ChildItems.RemoveAt(1);
                        //mnuFinanceiro.Items[0].ChildItems.RemoveAt(1);
                        //mnuFinanceiro.Items[0].ChildItems.RemoveAt(1);
                        break;
                    }
                    case Perfil.ConferenciaIDKey: //perfil conferencia
                    {
                        mnuCadastros.Items[0].ChildItems.Clear();

                        mnuAdministracao.Items[0].ChildItems.RemoveAt(11);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(10);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(9);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(8);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(7);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(6);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(5);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(4);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(3);
                        mnuAdministracao.Items[0].ChildItems[2].ChildItems.RemoveAt(1);
                        mnuAdministracao.Items[0].ChildItems[2].ChildItems.RemoveAt(0);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(1);
                        mnuAdministracao.Items[0].ChildItems.RemoveAt(0);

                        mnuMovimentacao.Items[0].ChildItems.Clear();
                        mnuFinanceiro.Items[0].ChildItems.Clear();
                        break;
                    }
                    case Perfil.PropostaBeneficiarioIDKey:
                    case Perfil.ConsultaPropostaBeneficiarioIDKey:
                    case Perfil.Atendimento_Liberacao_Vencimento:
                    {
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);

                        mnuMovimentacao.Items[0].ChildItems.Clear();
                        mnuAdministracao.Items[0].ChildItems.Clear();
                        //mnuFinanceiro.Items[0].ChildItems.Clear();
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);

                        break;
                    }
                    case Perfil.ConsulPropBenefProdLiberBoletoIDKey:
                    {
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(1);

                        mnuCadastros.Items[0].ChildItems[2].ChildItems.Clear();

                        mnuMovimentacao.Items[0].ChildItems.Clear();
                        mnuAdministracao.Items[0].ChildItems.Clear();
                        mnuFinanceiro.Items[0].ChildItems.Clear();
                        break;
                    }
                    case Perfil.PropostaBeneficiarioDemaisLeituraIDKey:
                    {
                        mnuMovimentacao.Items[0].ChildItems.Clear();
                        mnuAdministracao.Items[0].ChildItems.Clear();
                        mnuFinanceiro.Items[0].ChildItems.Clear();
                        break;
                    }
                    case Perfil.JuridicoIDKey:
                    {
                        //mnuCadastros.Items[0].ChildItems.Clear();
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        //mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems[1].ChildItems.Clear();

                        mnuMovimentacao.Items[0].ChildItems.Clear();
                        mnuAdministracao.Items[0].ChildItems.Clear();

                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(1);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(1);
                        mnuFinanceiro.Items[0].ChildItems[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems[0].ChildItems.RemoveAt(2);
                        //mnuFinanceiro.Items[0].ChildItems[0].ChildItems.RemoveAt(0);
                        //mnuFinanceiro.Items[0].ChildItems[0].ChildItems.RemoveAt(0);
                        break;
                    }
                    case Perfil.MarcaOticaIDKey:
                    {
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems.RemoveAt(0);
                        mnuCadastros.Items[0].ChildItems[0].Selectable = false;
                        mnuCadastros.Items[0].ChildItems[0].ChildItems.RemoveAt(0);

                        mnuMovimentacao.Items[0].ChildItems.Clear();
                        mnuAdministracao.Items[0].ChildItems.Clear();
                        mnuFinanceiro.Items[0].ChildItems.Clear();
                        break;
                    }
                    case Perfil.Financeiro_RecupPendencias:
                    {
                        mnuCadastros.Items[0].ChildItems.Clear();
                        mnuMovimentacao.Items[0].ChildItems.Clear();
                        mnuAdministracao.Items[0].ChildItems.Clear();
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);
                        mnuFinanceiro.Items[0].ChildItems.RemoveAt(0);

                        break;
                    }
                }
            }
        }
    }
}
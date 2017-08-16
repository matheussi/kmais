namespace www
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using LC.Web.PadraoSeguros.Entity;

    public abstract class PageBaseConferencia : PageBase
    {
        protected Conferencia ConferenciaCorrente
        {
            get { return Session["_conf_" + Session.SessionID] as Conferencia; }
            set { Session["_conf_" + Session.SessionID] = value; }
        }

        /// <summary>
        /// Conferencia
        /// </summary>
        protected void ExibirDepartamentos(DropDownList combo, Object perfilId)
        {
            combo.Items.Clear();

            if (UIHelper.PerfilConferencia(Convert.ToString(perfilId)))
            {
                combo.Items.Add(new ListItem("Em Conferência", "3"));
                combo.Items.Add(new ListItem("No Cadastro", "4"));
                combo.Items.Add(new ListItem("Com Corretor", "1"));
                combo.Items.Add(new ListItem("Em análise técnica", "5"));
            }
            else if (UIHelper.PerfilCadastro(Convert.ToString(perfilId)) ||
                UIHelper.PerfilADMIN(Convert.ToString(perfilId)))
            {
                combo.Items.Add(new ListItem("Em Conferência", "3"));
                combo.Items.Add(new ListItem("No Cadastro", "4"));
                combo.Items.Add(new ListItem("Com Corretor", "1"));
                combo.Items.Add(new ListItem("Em análise técnica", "5"));
            }

            combo.Items.Add(new ListItem("Já cadastrados", "6"));
        }

        //public abstract void LeConferencia();
    }
}

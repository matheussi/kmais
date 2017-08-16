namespace www
{
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class _Default : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                lblUsuario.Text = Usuario.Autenticado.Nome;
                lblData.Text = DateTime.Now.ToLongDateString();

                UIHelper.AuthCtrl(pnlAtalhos, new String[] { Perfil.AdministradorIDKey });
            }
        }
    }
}

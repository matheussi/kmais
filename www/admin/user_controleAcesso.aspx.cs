namespace www.admin
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class user_controleAcesso : PageBase
    {
        Object usuarioId
        {
            get { return Request[IDKey]; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.PreencheCombos();
                this.CarregaDados();
            }
        }

        void CarregaDados()
        {
            Usuario usuario = new Usuario(usuarioId);
            usuario.Carregar();
            litUsuario.Text = usuario.Nome.ToUpper();

            ControleAcesso ca;

            #region domingo

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Domingo);
            if (ca != null)
            {
                chkDomingo.Checked = true;
                cboDomDasHora.SelectedValue   = ca.HoraInicio.Split(':')[0];
                cboDomDasMinuto.SelectedValue = ca.HoraInicio.Split(':')[1];

                cboDomAsHora.SelectedValue   = ca.HoraFim.Split(':')[0];
                cboDomAsMinuto.SelectedValue = ca.HoraFim.Split(':')[1];
                txtDomingoIPs.Text           = ca.IPs;
            }
            #endregion

            #region segunda

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Segunda);
            if (ca != null)
            {
                chkSegunda.Checked = true;
                cboSegundaDasHora.SelectedValue = ca.HoraInicio.Split(':')[0];
                cboSegundaDasMinuto.SelectedValue = ca.HoraInicio.Split(':')[1];

                cboSegundaAsHora.SelectedValue = ca.HoraFim.Split(':')[0];
                cboSegundaAsMinuto.SelectedValue = ca.HoraFim.Split(':')[1];
                txtSegundaIPs.Text = ca.IPs;
            }
            #endregion

            #region terca

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Terca);
            if (ca != null)
            {
                chkTerca.Checked = true;
                cboTercaDasHora.SelectedValue   = ca.HoraInicio.Split(':')[0];
                cboTercaDasMinuto.SelectedValue = ca.HoraInicio.Split(':')[1];

                cboTercaAsHora.SelectedValue   = ca.HoraFim.Split(':')[0];
                cboTercaAsMinuto.SelectedValue = ca.HoraFim.Split(':')[1];
                txtTercaIPs.Text = ca.IPs;
            }
            #endregion

            #region quarta

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Quarta);
            if (ca != null)
            {
                chkQuarta.Checked = true;
                cboQuartaDasHora.SelectedValue = ca.HoraInicio.Split(':')[0];
                cboQuartaDasMinuto.SelectedValue = ca.HoraInicio.Split(':')[1];

                cboQuartaAsHora.SelectedValue = ca.HoraFim.Split(':')[0];
                cboQuartaAsMinuto.SelectedValue = ca.HoraFim.Split(':')[1];
                txtQuartaIPs.Text = ca.IPs;
            }
            #endregion

            #region quinta

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Quinta);
            if (ca != null)
            {
                chkQuinta.Checked = true;
                cboQuintaDasHora.SelectedValue = ca.HoraInicio.Split(':')[0];
                cboQuintaDasMinuto.SelectedValue = ca.HoraInicio.Split(':')[1];

                cboQuintaAsHora.SelectedValue = ca.HoraFim.Split(':')[0];
                cboQuintaAsMinuto.SelectedValue = ca.HoraFim.Split(':')[1];
                txtQuintaIPs.Text = ca.IPs;
            }
            #endregion

            #region sexta

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Sexta);
            if (ca != null)
            {
                chkSexta.Checked = true;
                cboSextaDasHora.SelectedValue = ca.HoraInicio.Split(':')[0];
                cboSextaDasMinuto.SelectedValue = ca.HoraInicio.Split(':')[1];

                cboSextaAsHora.SelectedValue = ca.HoraFim.Split(':')[0];
                cboSextaAsMinuto.SelectedValue = ca.HoraFim.Split(':')[1];
                txtSextaIPs.Text = ca.IPs;
            }
            #endregion

            #region sabado

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Sabado);
            if (ca != null)
            {
                chkSabado.Checked = true;
                cboSabadoDasHora.SelectedValue = ca.HoraInicio.Split(':')[0];
                cboSabadoDasMinuto.SelectedValue = ca.HoraInicio.Split(':')[1];

                cboSabadoAsHora.SelectedValue = ca.HoraFim.Split(':')[0];
                cboSabadoAsMinuto.SelectedValue = ca.HoraFim.Split(':')[1];
                txtSabadoIPs.Text = ca.IPs;
            }
            #endregion
        }

        void PreencheCombos()
        {
            base.PreencheComboNumerico(cboDomDasHora, 0, 23, true);
            base.PreencheComboNumerico(cboDomDasMinuto, 0, 59, true);
            base.PreencheComboNumerico(cboDomAsHora, 0, 23, true);
            base.PreencheComboNumerico(cboDomAsMinuto, 0, 59, true);

            base.PreencheComboNumerico(cboSegundaDasHora, 0, 23, true);
            base.PreencheComboNumerico(cboSegundaDasMinuto, 0, 59, true);
            base.PreencheComboNumerico(cboSegundaAsHora, 0, 23, true);
            base.PreencheComboNumerico(cboSegundaAsMinuto, 0, 59, true);

            base.PreencheComboNumerico(cboTercaDasHora, 0, 23, true);
            base.PreencheComboNumerico(cboTercaDasMinuto, 0, 59, true);
            base.PreencheComboNumerico(cboTercaAsHora, 0, 23, true);
            base.PreencheComboNumerico(cboTercaAsMinuto, 0, 59, true);

            base.PreencheComboNumerico(cboQuartaDasHora, 0, 23, true);
            base.PreencheComboNumerico(cboQuartaDasMinuto, 0, 59, true);
            base.PreencheComboNumerico(cboQuartaAsHora, 0, 23, true);
            base.PreencheComboNumerico(cboQuartaAsMinuto, 0, 59, true);

            base.PreencheComboNumerico(cboQuintaDasHora, 0, 23, true);
            base.PreencheComboNumerico(cboQuintaDasMinuto, 0, 59, true);
            base.PreencheComboNumerico(cboQuintaAsHora, 0, 23, true);
            base.PreencheComboNumerico(cboQuintaAsMinuto, 0, 59, true);

            base.PreencheComboNumerico(cboSextaDasHora, 0, 23, true);
            base.PreencheComboNumerico(cboSextaDasMinuto, 0, 59, true);
            base.PreencheComboNumerico(cboSextaAsHora, 0, 23, true);
            base.PreencheComboNumerico(cboSextaAsMinuto, 0, 59, true);

            base.PreencheComboNumerico(cboSabadoDasHora, 0, 23, true);
            base.PreencheComboNumerico(cboSabadoDasMinuto, 0, 59, true);
            base.PreencheComboNumerico(cboSabadoAsHora, 0, 23, true);
            base.PreencheComboNumerico(cboSabadoAsMinuto, 0, 59, true);
        }

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/admin/users.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            ControleAcesso ca;

            #region domingo 

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Domingo);
            if (!chkDomingo.Checked && ca != null)
            {
                ca.Remover();
            }
            else if (chkDomingo.Checked)
            {
                ca.DiaDaSemana = (int)ControleAcesso.eDiaSemana.Domingo;
                ca.HoraInicio  = cboDomDasHora.SelectedValue + ":" + cboDomDasMinuto.SelectedValue;
                ca.HoraFim     = cboDomAsHora.SelectedValue + ":" + cboDomAsMinuto.SelectedValue;
                ca.IPs         = txtDomingoIPs.Text;
                ca.UsuarioID   = usuarioId;
                ca.Salvar();
            }
            #endregion

            #region segunda 

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Segunda);
            if (!chkSegunda.Checked && ca != null)
            {
                ca.Remover();
            }
            else if (chkSegunda.Checked)
            {
                ca.DiaDaSemana = (int)ControleAcesso.eDiaSemana.Segunda;
                ca.HoraInicio = cboSegundaDasHora.SelectedValue + ":" + cboSegundaDasMinuto.SelectedValue;
                ca.HoraFim = cboSegundaAsHora.SelectedValue + ":" + cboSegundaAsMinuto.SelectedValue;
                ca.IPs = txtSegundaIPs.Text;
                ca.UsuarioID = usuarioId;
                ca.Salvar();
            }
            #endregion

            #region terca 

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Terca);
            if (!chkTerca.Checked && ca != null)
            {
                ca.Remover();
            }
            else if (chkTerca.Checked)
            {
                ca.DiaDaSemana = (int)ControleAcesso.eDiaSemana.Terca;
                ca.HoraInicio = cboTercaDasHora.SelectedValue + ":" + cboTercaDasMinuto.SelectedValue;
                ca.HoraFim = cboTercaAsHora.SelectedValue + ":" + cboTercaAsMinuto.SelectedValue;
                ca.IPs = txtTercaIPs.Text;
                ca.UsuarioID = usuarioId;
                ca.Salvar();
            }
            #endregion

            #region quarta 

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Quarta);
            if (!chkQuarta.Checked && ca != null)
            {
                ca.Remover();
            }
            else if (chkQuarta.Checked)
            {
                ca.DiaDaSemana = (int)ControleAcesso.eDiaSemana.Quarta;
                ca.HoraInicio = cboQuartaDasHora.SelectedValue + ":" + cboQuartaDasMinuto.SelectedValue;
                ca.HoraFim = cboQuartaAsHora.SelectedValue + ":" + cboQuartaAsMinuto.SelectedValue;
                ca.IPs = txtQuartaIPs.Text;
                ca.UsuarioID = usuarioId;
                ca.Salvar();
            }
            #endregion

            #region quinta 

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Quinta);
            if (!chkQuinta.Checked && ca != null)
            {
                ca.Remover();
            }
            else if (chkQuinta.Checked)
            {
                ca.DiaDaSemana = (int)ControleAcesso.eDiaSemana.Quinta;
                ca.HoraInicio = cboQuintaDasHora.SelectedValue + ":" + cboQuintaDasMinuto.SelectedValue;
                ca.HoraFim = cboQuintaAsHora.SelectedValue + ":" + cboQuintaAsMinuto.SelectedValue;
                ca.IPs = txtQuintaIPs.Text;
                ca.UsuarioID = usuarioId;
                ca.Salvar();
            }
            #endregion

            #region sexta 

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Sexta);
            if (!chkSexta.Checked && ca != null)
            {
                ca.Remover();
            }
            else if (chkSexta.Checked)
            {
                ca.DiaDaSemana = (int)ControleAcesso.eDiaSemana.Sexta;
                ca.HoraInicio = cboSextaDasHora.SelectedValue + ":" + cboSextaDasMinuto.SelectedValue;
                ca.HoraFim = cboSextaAsHora.SelectedValue + ":" + cboSextaAsMinuto.SelectedValue;
                ca.IPs = txtSextaIPs.Text;
                ca.UsuarioID = usuarioId;
                ca.Salvar();
            }
            #endregion

            #region sabado 

            ca = ControleAcesso.CarregarPorUsuario(usuarioId, ControleAcesso.eDiaSemana.Sabado);
            if (!chkSabado.Checked && ca != null)
            {
                ca.Remover();
            }
            else if (chkSabado.Checked)
            {
                ca.DiaDaSemana = (int)ControleAcesso.eDiaSemana.Sabado;
                ca.HoraInicio = cboSabadoDasHora.SelectedValue + ":" + cboSabadoDasMinuto.SelectedValue;
                ca.HoraFim = cboSabadoAsHora.SelectedValue + ":" + cboSabadoAsMinuto.SelectedValue;
                ca.IPs = txtSabadoIPs.Text;
                ca.UsuarioID = usuarioId;
                ca.Salvar();
            }
            #endregion

            base.Alerta(null, this, "_ok", "Informações salvas com sucesso.");
        }
    }
}
namespace www.movimentacao
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class importacaoAgenda : PageBase
    {
        String fPath
        {
            get { return String.Concat(Server.MapPath("/"), ImportacaoProposta.BaseFileTargetPath.Replace("/", "\\")); }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                base.PreencheComboNumerico(cboHora, 0, 23, true);
                base.PreencheComboNumerico(cboMinuto, 0, 59, true);
            }

            if (Usuario.Autenticado.PerfilID == Perfil.ConsultaPropostaBeneficiarioIDKey)
            {
                cmdLer.Visible = false;
            }
        }

        protected void cmdLer_Click(object sender, EventArgs e)
        {
            DateTime procData = base.CStringToDateTime(txtProcessarEm.Text, cboHora.SelectedValue + ":" + cboMinuto.SelectedValue);

            if (String.IsNullOrEmpty(uplFile.FileName)) { return; }

            if (Path.GetExtension(Path.GetFileName(uplFile.FileName)).Replace(".", "").ToUpper() != "XML")
            {
                base.Alerta(null, this, "__errExt", "O arquivo de importação deve ter a extensão XML.");
                return;
            }

            if (!Directory.Exists(this.fPath)) { Directory.CreateDirectory(this.fPath); }

            ImportacaoProposta.ItemAgendamento obj = new ImportacaoProposta.ItemAgendamento();
            obj.Arquivo = Path.GetFileName(uplFile.FileName);
            obj.Descricao = txtDescricao.Text;
            obj.ProcessarEm = procData;
            obj.Salvar();

            uplFile.SaveAs(String.Concat(this.fPath, obj.ID, Path.GetExtension(obj.Arquivo)));

            base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
            txtDescricao.Focus();
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
        }
    }
}

namespace www
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class arquivoAniversario : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.LoadCombos();
            }
        }

        private void LoadCombos()
        {
            base.PreencheComboNumerico(this.cboDia, 0, 31, true);
            base.PreencheComboNumerico(this.cboMes, 1, 12, true);
            this.cboDia.Items[0].Text = "--";
        }

        /// <summary>
        /// TODO: centralizar
        /// </summary>
        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            #region validacoes

            #endregion

            String conteudo = Usuario.GerarArquivoAniversariantes(Convert.ToInt32(cboDia.SelectedValue), Convert.ToInt32(cboMes.SelectedValue));

            if (String.IsNullOrEmpty(conteudo) == false)
            {
                String arquivoNome = String.Concat("_aniv_",
                                                   DateTime.Now.Day.ToString(),
                                                   DateTime.Now.Month.ToString(),
                                                   DateTime.Now.Year.ToString(),
                                                   DateTime.Now.Hour.ToString(),
                                                   DateTime.Now.Minute.ToString(),
                                                   DateTime.Now.Millisecond.ToString(), ".DAT");

                if (!Directory.Exists(String.Concat(Server.MapPath("/") + ConfigurationManager.AppSettings["otherFilePath"].Replace("/", @"\"))))
                {
                    Directory.CreateDirectory(String.Concat(Server.MapPath("/") + ConfigurationManager.AppSettings["otherFilePath"].Replace("/", @"\")));
                }

                String strFilePath = String.Concat(Server.MapPath("/") + ConfigurationManager.AppSettings["otherFilePath"].Replace("/", @"\"), arquivoNome);

                System.IO.FileStream arquivo = null;

                try
                {
                    File.WriteAllText(strFilePath, conteudo, System.Text.Encoding.GetEncoding("iso-8859-1"));
                }
                catch (Exception) { throw; }

                try
                {
                    arquivo = new System.IO.FileStream(strFilePath, System.IO.FileMode.Open);
                }
                catch (System.IO.FileNotFoundException)
                {
                    throw;
                }

                //TODO: centralizar esses métodos que geram e baixam arquivos
                Byte[] arrByte = new Byte[arquivo.Length];
                arquivo.Read(arrByte, 0, arrByte.Length);
                arquivo.Close();
                arquivo.Dispose();
                arquivo = null;

                this.Response.Clear();
                this.Response.ContentType = "application/octet-stream";
                this.Response.AppendHeader("Content-Length", arrByte.Length.ToString());
                this.Response.AppendHeader("Content-Disposition", String.Concat("attachment; filename=", arquivoNome));
                this.Response.BinaryWrite(arrByte);
                this.Response.Flush();
            }
            else
            {
                base.Alerta(null,this, "__err", "Nenhum aniversariante encontrado.");
            }
        }
    }
}

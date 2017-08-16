
namespace www.UBRASP.arquivos.scc
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Facade;
    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Entity.ArquivoUbrasp;

    public partial class descontosOcorridos : PageBase
    {
        String Path
        {
            get { return String.Concat(Server.MapPath("/"), ConfigurationManager.AppSettings["psccFilePath"].Replace("/", "\\")); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.CarregaContratoADM();
            }
        }

        void CarregaContratoADM()
        {
            cboContratoADM.Items.Clear();

            cboContratoADM.DataValueField = "ID";
            cboContratoADM.DataTextField = "DescricaoCodigoSaudeDental";

            IList<ContratoADM> lista = ContratoADM.CarregarTodos(4, false);
            cboContratoADM.DataSource = lista;
            cboContratoADM.DataBind();
        }

        protected void cmdProcessar_click(object sender, EventArgs e)
        {
            if (ufArquivo.PostedFile == null || string.IsNullOrEmpty(ufArquivo.PostedFile.FileName)) return;

            if (!Directory.Exists(this.Path))
            {
                Directory.CreateDirectory(this.Path);
            }

            String fileName = String.Concat("_pscc_", DateTime.Now.ToString("yyyyMMddHHmmssffff"), ".txt");
            ufArquivo.SaveAs(String.Concat(this.Path, fileName));

            String arquivo = String.Concat(this.Path, fileName);

            String content = "";
            using (StreamReader stream = new StreamReader(arquivo))
            {
                content = stream.ReadToEnd();
                stream.Close();
            }

            PSCCDesconto_Header header = new PSCCDesconto_Header();
            List<PSCCDesconto_Item> itens = null;

            try
            {
                itens = header.ProcessarConteudo(content, fileName, cboContratoADM.SelectedValue);
            }
            catch
            {
                base.Alerta(null, this, "_ok", "Erro ao processar o arquivo. Código 00.");
                return;
            }

            try
            {
                ArquivoUBRASPFacade.Instance.SalvaArquivoPSCC(header, itens);
            }
            catch
            {
                base.Alerta(null, this, "_ok", "Erro ao salvar os dados. Código 01.");
                return;
            }

            base.Alerta(null, this, "_ok", "Dados salvos com sucesso.");
        }
    }
}
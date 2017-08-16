using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using LC.Web.PadraoSeguros.Entity;

using www.reports;
//using www.reports.financeiro;

namespace www.financeiro
{
    public partial class popupRelatorioListagem : System.Web.UI.Page
    {
        #region Private Members

        /// <summary>
        /// Array de ID do Perfil Request. Array separado por virgula.
        /// </summary>
        private String PerfilIDRequest
        {
            get { return this.Request["arrPerfil"]; }
        }

        /// <summary>
        /// ID da Listagem Request.
        /// </summary>
        private String ListagemIDRequest
        {
            get {return  this.Request["listID"]; }
        }

        #endregion

        #region Protected Control Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            /*
            if (!String.IsNullOrEmpty(this.PerfilIDRequest) && !String.IsNullOrEmpty(this.ListagemIDRequest))
            {
                Object[] arrPerfil = (Object[])this.PerfilIDRequest.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (arrPerfil != null && arrPerfil.Length > 0)
                {
                    Listagem listagem = new Listagem(this.ListagemIDRequest);
                    listagem.Carregar();

                    this.rptReportViewer.ShowLoadingPanelImage = true;

                    ProdutorReport report = new ProdutorReport(listagem);
                    report.DataSource = ProdutorRO.GetProdutores(arrPerfil, listagem.ID);
                    
                    this.rptReportViewer.Report = report;
                    this.rptReportViewer.WritePdfTo(HttpContext.Current.Response);
                }
            }

            */
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LC.Web.PadraoSeguros.Entity;

namespace www
{
    public partial class termoquitacao : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) 
            {
                this.montaTermo();
            }

            //HttpContext.Current.Response.Clear();
            //HttpContext.Current.Response.ClearHeaders();
            //HttpContext.Current.Response.ClearContent();
            //HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=termo.doc");
            //HttpContext.Current.Response.ContentType = "application/msword"; //"application/pdf";
        }

        void montaTermo()
        {
            String contratoId = Request.QueryString[IDKey];
            if (String.IsNullOrEmpty(contratoId)) { return; }

            Contrato contrato = new Contrato(contratoId);
            contrato.Carregar();

            if (!string.IsNullOrEmpty(contrato.ResponsavelNome) && !string.IsNullOrEmpty(contrato.ResponsavelCPF))
            {
                //responsavel financeiro
                litNome.Text = contrato.ResponsavelNome;
                litCpf.Text = contrato.ResponsavelCPF;
            }
            else
            {
                //titular cadastrado
                ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(contrato.ID, null);
                litNome.Text = titular.BeneficiarioNome;
                litCpf.Text = titular.BeneficiarioCPF;
            }
        }
    }
}

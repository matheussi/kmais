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

namespace www.reports
{
    public partial class relacaoCobrancas : PageBase
    {
        protected void Page_Load(Object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request[IDKey] == null) { return; }
                gridCobranca.DataSource = Cobranca.CarregarTodas(Request[IDKey], true, null);
                gridCobranca.DataBind();

                Contrato contrato = new Contrato(Request[IDKey]);
                contrato.Carregar();
                litContrato.Text = contrato.Numero;

                Object titularId = ContratoBeneficiario.CarregaTitularID(contrato.ID, null);

                Beneficiario benef = new Beneficiario(titularId);
                benef.Carregar();

                litTitNome.Text = benef.Nome;
                litTitCpf.Text  = benef.CPF;
            }
        }
    }
}

/*

public override void VerifyRenderingInServerForm(Control control)
{
    //if (!exportando)
    //{
    //    base.VerifyRenderingInServerForm(control);
    //}
}
 
protected void cmdToExcel_Click(Object sende, EventArgs e)
{
    grid.DataSource = filtraValueObjects(true);
    grid.DataBind();
    String attachment = "attachment; filename=file.xls";
    Response.ClearContent();
    Response.AddHeader("content-disposition", attachment);
    Response.ContentType = "application/ms-excel";
    StringWriter sw = new StringWriter();
    HtmlTextWriter htw = new HtmlTextWriter(sw);
    grid.RenderControl(htw);
    Response.Write(sw.ToString());
    Response.End(); 
}

*/
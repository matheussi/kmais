namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.Services;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;

    public partial class searchCorrTercMethod : System.Web.UI.Page
    {
        protected void Page_Load(Object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ProcessRequest(Request["q"]);
            }
        }

         public void ProcessRequest(String param)
        {
            if (!String.IsNullOrEmpty(param) && param.Length >= 4)
            {
                String q = param.Trim();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                DataTable dt = null;
                String qry = String.Concat(
                    "select distinct top 25 contrato_corretorTerceiroNome,contrato_corretorTerceiroCPF ",
                    "    from contrato ",
                    "    where (contrato_corretorTerceiroNome like @param or  contrato_corretorTerceiroCPF like @param)", 
                    "       and contrato_corretorTerceiroNome is not null and contrato_corretorTerceiroNome <> '' ",
                    "       and contrato_corretorTerceiroCPF is not null and contrato_corretorTerceiroCPF <> '' ",
                    "    order by contrato_corretorTerceiroNome");

                String[] paramNm = new String[] { "@param" };
                String[] paramVl = new String[] { String.Concat("%", param, "%") };

                dt = LocatorHelper.Instance.ExecuteParametrizedQuery(qry, paramNm, paramVl).Tables[0];

                sb.Append("{ \"CorretoresTerc\" : [");

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i > 0) { sb.Append(" , "); }
                        sb.Append(" { \"Nome\" : \""); sb.Append(dt.Rows[i]["contrato_corretorTerceiroNome"]); sb.Append("\", ");
                        sb.Append("\"CPF\" : \""); sb.Append(dt.Rows[i]["contrato_corretorTerceiroCPF"]); sb.Append("\" } ");
                    }
                }

                sb.Append(" ] } ");
                Response.Clear();
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.ContentType = "application/json";
                Response.Write(sb.ToString());
                Response.End();
            }
        }
    }
}

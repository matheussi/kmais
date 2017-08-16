namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.Services;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class searchOperadoraOrigemMethod : System.Web.UI.Page
    {
        protected void Page_Load(Object sender, EventArgs e)
        {
            ProcessRequest(Request["q"]);
        }

        public void ProcessRequest(String param)
        {
            if (!String.IsNullOrEmpty(param))
            {
                String q = param.Trim();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                DataTable dt = Operadora.CarregarOperadorasOrigem(q);

                sb.Append("{ \"Operadoras\" : [");
                if (dt != null && dt.Rows != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i > 0) { sb.Append(" , "); }
                        sb.Append(" { \"ID\" : \""); sb.Append(dt.Rows[i]["ID"]); sb.Append("\", ");
                        sb.Append("\"Nome\" : \""); sb.Append(dt.Rows[i]["Nome"]); sb.Append("\", ");
                        sb.Append("\"ANS\" : \""); sb.Append(dt.Rows[i]["Codigo"]); sb.Append("\" } ");
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

namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.Services;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Framework.Phantom;

    public partial class searchAtendenteMethod : System.Web.UI.Page
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
            String qry = "select distinct(atendimento_cadastrado) as Atendente from _atendimento where atendimento_cadastrado like '%" + param + "%'";
            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("{ \"Atendentes\" : [");

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i > 0) { sb.Append(" , "); }
                    sb.Append(" { \"Atendente\" : \""); sb.Append(dt.Rows[i][0]); sb.Append("\" } ");
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
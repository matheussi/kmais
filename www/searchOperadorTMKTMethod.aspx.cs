namespace www
{
    using System;
    using System.Web;
    using System.Web.Services;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class searchOperadorTMKTMethod : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ProcessRequest(Request["q"]);
            }
        }

        public void ProcessRequest(String param)
        {
            if (!String.IsNullOrEmpty(param))
            {
                String q = param.Trim();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                IList<Usuario> lista = Usuario.CarregarOperadorMKTPorDoc(q);

                sb.Append("{ \"Operadores\" : [");
                if (lista != null)
                {
                    for (int i = 0; i < lista.Count; i++)
                    {
                        if (i > 0) { sb.Append(" , "); }
                        sb.Append(" { \"ID\" : \""); sb.Append(lista[i].ID); sb.Append("\", ");
                        sb.Append("\"Nome\" : \""); sb.Append(lista[i].Nome); sb.Append("\", ");
                        sb.Append("\"Doc\" : \""); sb.Append(lista[i].Documento1); sb.Append("\" } ");
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

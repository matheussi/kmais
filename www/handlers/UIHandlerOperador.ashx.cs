namespace www.handlers
{
    using System;
    using System.Web;
    using System.Web.Services;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    /// <summary>
    /// 
    /// </summary>
    [WebService(Namespace = "http://linkecerebro.com.br/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class UIHandlerOperador : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.QueryString["q"] != null)
            {
                String q = context.Request.QueryString["q"].Trim();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                IList<Usuario> lista = Usuario.CarregarOperadorMKTPorDoc(q);

                if (lista != null)
                {
                    sb.Append("{ \"Corretores\" : [");
                    for (int i = 0; i < lista.Count; i++)
                    {
                        if (i > 0) { sb.Append(" , "); }
                        sb.Append(" { \"ID\" : \""); sb.Append(lista[i].ID); sb.Append("\", ");
                        sb.Append("\"Nome\" : \""); sb.Append(lista[i].Nome); sb.Append("\", ");
                        sb.Append("\"Doc\" : \""); sb.Append(lista[i].Documento1); sb.Append("\" } ");
                    }
                    sb.Append(" ] } ");
                }

                context.Response.ContentType = "text/plain";
                context.Response.Write(sb.ToString());
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}

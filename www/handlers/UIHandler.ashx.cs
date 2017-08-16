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
    public class UIHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.QueryString["q"] != null)
            {
                String q = context.Request.QueryString["q"].Trim();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                if (context.Request.UrlReferrer.Segments[1].Equals("contrato.aspx") ||
                    context.Request.UrlReferrer.Segments[2].Equals("almoxSaidas.aspx"))
                {
                    IList<Usuario> lista = Usuario.CarregarCorretorPorDoc(q);

                    if (lista != null)
                    {
                        sb.Append("{ \"Corretores\" : [");
                        for(int i = 0; i < lista.Count; i++)
                        {
                            if (i > 0) { sb.Append(" , "); }
                            sb.Append(" { \"ID\" : \""); sb.Append(lista[i].ID); sb.Append("\", ");
                            sb.Append("\"Nome\" : \""); sb.Append(lista[i].Nome); sb.Append("\", ");
                            sb.Append("\"Doc\" : \""); sb.Append(lista[i].Documento1); sb.Append("\" } ");
                        }
                        sb.Append(" ] } ");
                    }
                }

                context.Response.Clear();
                context.Response.ContentEncoding = System.Text.Encoding.UTF8;
                context.Response.ContentType = "text/json";
                context.Response.Write(sb.ToString());
                context.Response.End();
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

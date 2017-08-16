namespace www
{
    using System;
    using System.Web;
    using System.Web.Services;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class searchCorretorMethod : System.Web.UI.Page
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

                Object reqFilial = Request["filial"];

                if (reqFilial == null || Convert.ToString(reqFilial).Trim() == "branco" || Convert.ToString(reqFilial).Trim() == "-1")
                {
                    reqFilial = null;
                }

                IList <Usuario> lista = null;

                if (Request["filial"] != "branco")
                {
                    lista = Usuario.CarregarCorretorPorDocOuNomeOuCodigo(q, reqFilial, true, null);
                }

                sb.Append("{ \"Corretores\" : [");

                if (lista != null)
                {
                    for (int i = 0; i < lista.Count; i++)
                    {
                        if (i > 0) { sb.Append(" , "); }
                        sb.Append(" { \"ID\" : \""); sb.Append(lista[i].ID); sb.Append("\", ");
                        sb.Append("\"Nome\" : \""); sb.Append(lista[i].Nome); sb.Append("\", ");
                        sb.Append("\"Doc\" : \""); sb.Append(lista[i].Documento1); sb.Append("\", ");
                        sb.Append("\"Codigo\" : \""); sb.Append(lista[i].Codigo); sb.Append("\", ");
                        sb.Append("\"Tipo\" : \""); sb.Append(lista[i].TipoPessoa); sb.Append("\" } ");
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

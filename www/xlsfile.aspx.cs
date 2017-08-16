namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    public partial class xlsfile : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=movimentacao.xls");
                HttpContext.Current.Response.Charset = "";

                String content = "";
                if (Session["content"] != null) { content = UIHelper.CToString(Session["content"]); }

                Literal lit = new Literal();
                lit.EnableViewState=false;
                lit.Text = content;

                System.IO.StringWriter tw = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
                lit.RenderControl(hw);
                HttpContext.Current.Response.Write(tw.ToString());
                HttpContext.Current.Response.End();
            }
        }
    }
}

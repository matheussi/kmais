namespace www
{
    using System;
    using System.IO;
    using System.Web;
    using System.Linq;
    using System.Web.Security;
    using System.Configuration;
    using System.Web.SessionState;
    using System.Collections.Generic;

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(Object sender, EventArgs e)
        {
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            String logError = ConfigurationManager.AppSettings["logErrorMessages"];
            if (logError == null || logError.ToUpper() != "Y") { return; }

            String dir = HttpContext.Current.Server.MapPath(".") + "\\log";
            if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }

            Exception ex = Server.GetLastError().GetBaseException();
            String msg = String.Concat("ERRO EM: ", Request.Url.ToString(),
                Environment.NewLine, "DATA: ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), 
                Environment.NewLine, "MENSAGEM: ", ex.Message,
                Environment.NewLine, "SOURCE: ", ex.Source, Environment.NewLine, "STACK TRACE: ", ex.StackTrace,
                Environment.NewLine, "USUARIO: ", Context.User.Identity.Name, Environment.NewLine, "-----------------------------------------------------------------------------------------------------------------------------------------------------------", Environment.NewLine);

            String filename = DateTime.Now.ToString("yyyyMMdd") + ".log";
            using (FileStream stream = new FileStream(dir + @"\" + filename, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    sw.Write(msg);
                    sw.Close();
                }
            }

            Server.ClearError();
            Response.Redirect("~/login.aspx?err=1"); //50187023972
        }

        protected void Session_End(Object sender, EventArgs e)
        {
        }

        protected void Application_End(Object sender, EventArgs e)
        {
        }
    }
}
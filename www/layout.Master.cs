using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace www
{
    public partial class layout : System.Web.UI.MasterPage
    {
        public UpdatePanel UPanel
        {
            get { return up; }
            set { up= value; }
        }

        public ToolkitScriptManager SM
        {
            get { return sm; }
            set { sm = value; }
        }

        protected void Page_Load(Object sender, EventArgs e)
        {
        }
    }
}

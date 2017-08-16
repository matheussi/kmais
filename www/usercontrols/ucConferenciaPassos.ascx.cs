using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace www.usercontrols
{
    public partial class ucConferenciaPassos : System.Web.UI.UserControl
    {
        protected override void  OnLoad(EventArgs e)
        {
 	         base.OnLoad(e);
        }

        public void SetaPasso(Int32 indice, String texto)
        {
            switch (indice)
            {
                case 1:
                    {
                        setStatus(IMG1);
                        unsetStatus(IMG2);
                        unsetStatus(IMG3);
                        //unsetStatus(IMG4);
                        break;
                    }
                case 2:
                    {
                        unsetStatus(IMG1);
                        setStatus(IMG2);
                        unsetStatus(IMG3);
                        //unsetStatus(IMG4);
                        break;
                    }
                case 3:
                    {
                        unsetStatus(IMG1);
                        unsetStatus(IMG2);
                        setStatus(IMG3);
                        //unsetStatus(IMG4);
                        break;
                    }
                default:
                    {
                        unsetStatus(IMG1);
                        unsetStatus(IMG2);
                        unsetStatus(IMG3);
                        //setStatus(IMG4);
                        break;
                    }
            }
        }

        void setStatus(System.Web.UI.HtmlControls.HtmlImage img)
        {
            img.Src = "~/images/site/Passo_on.gif";
        }

        void unsetStatus(System.Web.UI.HtmlControls.HtmlImage img)
        {
            img.Src = "~/images/site/Passo_off.gif";
        }
    }
}
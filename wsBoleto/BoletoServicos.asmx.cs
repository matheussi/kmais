namespace wsBoleto
{
    using System;
    using System.Web;
    using System.Data;
    using System.Text;
    using System.Net.Mail;
    using System.Collections;
    using System.Web.Services;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using System.Web.Services.Protocols;

    using LC.Web.PadraoSeguros.Entity;
    using System.Security.Cryptography;
    using LC.Framework.Phantom;

    [WebService(Namespace = "https://sistemas.qualicorp.com.br/ws")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class BoletoServicos : System.Web.Services.WebService
    {
        [WebMethod]
        public string SegundaViaBoleto()
        {
            return "Hello World";
        }
    }
}

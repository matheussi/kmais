namespace www
{
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Collections;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using System.Web.Security;

    using LC.Web.PadraoSeguros.Entity;
    using System.Data;

    public partial class login : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                //this.compFormatos();
                //String contasReceber = AgendaRelatorio.ContasReceberAbertoQUERY(DateTime.Now, DateTime.Now, new string[] { "1" }, new string[] { "1" });
                //String contasPago = AgendaRelatorio.ContasReceberPagoQUERY(DateTime.Now, DateTime.Now, new string[] { "1" }, new string[] { "1" });
                //String controladoriaDet = AgendaRelatorio.ControladoriaDetalheQUERY(DateTime.Now, DateTime.Now, new string[] { "1" }, new string[] { "1" });
                //String controladoriaTAXA = AgendaRelatorio.ControladoriaTaxaQUERY(DateTime.Now, DateTime.Now, new string[] { "1" }, new string[] { "1" });
                
                Usuario.Autenticado.Encerrar();

                String currUrl = Request.Url.ToString();
                if (!string.IsNullOrEmpty(currUrl) && currUrl.IndexOf("pspadrao") > -1 && string.IsNullOrEmpty(Request["bypass"]))
                {
                    Response.Redirect("https://sistemas.qualicorp.com.br/");
                }

                //if (currUrl.IndexOf("http://pspadraohomolog.iphotel.info") > -1)
                //{
                //    Response.Redirect("http://sistema.pspadrao.com.br");
                //}
                //else if (currUrl.IndexOf("http://homolog.pspadrao.com.br") > -1)
                //{
                //    Response.Redirect("https://homolog.pspadrao.com.br");
                //}
                //else if (currUrl.IndexOf("http://sistema.pspadrao.com.br") > -1)
                //{
                //    Response.Redirect("https://sistema.pspadrao.com.br");
                //}
            }
        }

        /// <summary>
        /// Feito para mudar o layout do scc, ja esta concluido.
        /// </summary>
        void compFormatos()
        {
            string antiga = "14993832746379400097048  0491619588400000000002130000000000000005660747318000162A2014080100000086621035062017000000000046844000000000046844000                              00210056                    LUCIA HELENA MARTINEZ                                                                                                                                 0010021";
            string novo = "14993832746379400097048  04916195884000000000021300                                                                                                                                                      00000000000005660747318000162A2014080100000086621000020070601        36072017000000000046844000000000046844000000000046844000000000000000100               00210056                    LUCIA HELENA MARTINEZ                                                                                                                                 0010021";

            this.processaAntiga(antiga);
            this.processaNovo(novo);
        }

        void processaAntiga(string linha)
        {
            DataTable dt = geraDataTable();

            string valor = Convert.ToInt32(linha.Substring(115, 9)).ToString();
            string valorDesc = linha.Substring(124, 15);

            DataRow row = dt.NewRow();

            row[0] = linha.Substring(25, 11);
            row[1] = Convert.ToInt64(linha.Substring(36, 15));
            row[2] = "";
            row[3] = linha.Substring(61, 5);
            row[4] = linha.Substring(80, 21);
            row[5] = linha.Substring(101, 2);
            row[6] = valor;
            row[7] = Convert.ToInt32(valorDesc);
            row[8] = string.Concat(valor.Substring(0, valor.Length - 2), ",", valor.Substring(valor.Length - 2, 2));
            row[9] = "";
            row[10] = linha.Substring(200, 150).Trim(); //nome

            write(row);

            dt.Dispose();
        }

        void processaNovo(string linha)
        {
            DataTable dt = geraDataTable();

            string valor = Convert.ToInt32(linha.Substring(284, 9)).ToString();
            string valorDesc = linha.Substring(293, 15);

            DataRow row = dt.NewRow();

            row[0] = linha.Substring(25, 11); //cpf
            row[1] = Convert.ToInt64(linha.Substring(36, 15)); //funcional
            row[2] = ""; //orgao
            row[3] = linha.Substring(211, 5); // orgao id
            row[4] = linha.Substring(230, 21); //averbacao
            row[5] = linha.Substring(270, 2); //Parcela
            row[6] = valor;
            row[7] = Convert.ToInt32(valorDesc);
            row[8] = string.Concat(valor.Substring(0, valor.Length - 2), ",", valor.Substring(valor.Length - 2, 2));
            row[9] = "";
            row[10] = linha.Substring(384, 150).Trim(); //nome

            write(row);

            dt.Dispose();
        }

        DataTable geraDataTable()
        {
            DataTable dt = new DataTable();

            //dt.Columns.Add("CPF");
            //dt.Columns.Add("Funcional");
            //dt.Columns.Add("Orgao");
            //dt.Columns.Add("IdentOrgao");
            //dt.Columns.Add("Nuavebev");
            //dt.Columns.Add("Parcela");
            //dt.Columns.Add("Valor");
            //dt.Columns.Add("Valor_Descontado");
            //dt.Columns.Add("Valor_Tratado");
            //dt.Columns.Add("Erro");

            dt.Columns.Add("Column1");
            dt.Columns.Add("Column2");
            dt.Columns.Add("Column3");
            dt.Columns.Add("Column4");
            dt.Columns.Add("Column5");
            dt.Columns.Add("Column6");
            dt.Columns.Add("Column7");
            dt.Columns.Add("Column8");
            dt.Columns.Add("Column9");
            dt.Columns.Add("Column10");
            dt.Columns.Add("Column11"); //nome

            return dt;
        }

        void write(DataRow row)
        {
            string table = string.Concat(
                "<table border='1'><tr>",
                "<th>CPF</th>",
                "<th>Funcional</th>",
                "<th>Orgao</th>",
                "<th>IDOrgao</th>",
                "<th>Nuavebev</th>",
                "<th>Parcela</th>",
                "<th>Valor</th>",
                "<th>Valor_Descontado</th>",
                "<th>Valor_Tratado</th>",
                "<th>Erro</th>",
                "<th>Nome</th>",
                "</tr>",
                "<tr>", 
                "<td>", row[0] ,"</td>", //cpf
                "<td>", row[1], "</td>",//funcional
                "<td>", row[2], "</td>",//orgao
                "<td>", row[3], "</td>",//id orgao
                "<td>", row[4], "</td>",//averb
                "<td>", row[5], "</td>",//parcela
                "<td>", row[6], "</td>",//valor
                "<td>", row[7], "</td>",//valor desc
                "<td>", row[8], "</td>",//valor trat
                "<td>", row[9], "</td>",//erro
                "<td>", row[10], "</td>",
                "</tr></table>");

            Response.Write(table);
        }

        protected void cmdEntrar_Click(Object sender, ImageClickEventArgs e)
        {
            if (txtEmail.Value.Trim() == "" || txtSenha.Value.Trim() == "")
            {
                litMsg.Text = "Preencha ambos os campos";
                return;
            }

            Usuario usuario = null;
            try
            {
                usuario = Usuario.Autentica(txtEmail.Value, txtSenha.Value);
                
#if DEBUG
                //usuario = new Usuario(); 
                //usuario.ID = 1; usuario.Carregar(); //3746
#endif
            }
            catch
            {
                litMsg.Text = "Não foi possível acessar o recurso";
                return;
            }

            if (usuario != null && usuario.SystemUser)
            {
                FormsAuthentication.SetAuthCookie(usuario.Nome, false);
                Usuario.Autenticado.ID    = Convert.ToString(usuario.ID);
                Usuario.Autenticado.Email = usuario.Email;

                if(usuario.EmpresaCobrancaID != null)
                    Usuario.Autenticado.EmpresaCobrancaID = Convert.ToString(usuario.EmpresaCobrancaID);

                if (usuario.PerfilID != null)
                    Usuario.Autenticado.PerfilID = Convert.ToString(usuario.PerfilID);

                if (Usuario.Autenticado.ID == "1") Usuario.Autenticado.PerfilID = "1";

                Usuario.Autenticado.PerfilDescricao = usuario.PerfilDescricao;
                Usuario.Autenticado.ExtraPermission = usuario.ExtraPermission;
                Usuario.Autenticado.ExtraPermission2 = usuario.ExtraPermission2;

                if (UIHelper.PerfilMedico(Usuario.Autenticado.PerfilID))
                    Response.Redirect("~/admin/conferenciaMedico.aspx");
                if (UIHelper.PerfilDeptoTecnico(Usuario.Autenticado.PerfilID))
                    Response.Redirect("~/admin/conferenciaTecnico.aspx");
                if (UIHelper.PerfilConferencia(Usuario.Autenticado.PerfilID))
                    Response.Redirect("~/admin/conferenciaLista.aspx");
                if (UIHelper.PerfilCadastro(Usuario.Autenticado.PerfilID))
                    Response.Redirect("~/admin/conferenciaLista.aspx");
                else
                    Response.Redirect("default.aspx");
            }
            else
            {
                litMsg.Text = "Senha ou login inválido(s)";
            }
        }
    }
}
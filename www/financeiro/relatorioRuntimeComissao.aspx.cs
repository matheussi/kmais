namespace www.financeiro
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Xml.Linq;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Framework.Phantom;

    using www.reports;
    //using www.reports.financeiro;

    public partial class relatorioRuntimeComissao : PageBase
    {
        #region Private Members

        /// <summary>
        /// Array de ID do Perfil Request. Array separado por virgula.
        /// </summary>
        private String PerfilIDRequest
        {
            get { return this.Request["arrPerfil"]; }
        }

        /// <summary>
        /// ID da Listagem Request.
        /// </summary>
        private String ListagemIDRequest
        {
            get { return this.Request["listID"]; } 
        }

        #endregion

        protected void Page_Load(Object sender, EventArgs e)
        {
            if (Request["consolidadoBanco"] == null)
            {
                Response.ContentType = "application/msword";
                Response.AppendHeader("Content-Type", "application/msword");
                Response.AppendHeader("Content-disposition", "attachment; filename=fechamento.doc");
            }
            else
                Response.ContentType = "application/vnd.ms-excel";

            Response.ContentEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");

            if (!String.IsNullOrEmpty(this.PerfilIDRequest) && !String.IsNullOrEmpty(this.ListagemIDRequest))
            {
                String[] arrPerfil = this.PerfilIDRequest.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (arrPerfil != null && arrPerfil.Length > 0)
                {
                    PersistenceManager pm = new PersistenceManager();
                    pm.UseSingleCommandInstance();

                    DateTime agora = DateTime.Now;
                    DataRow[] rows = null;
                    Object ret = null;
                    String aux = null, nomeProdutor = null, documentoProdutor = null;
                    Decimal totalPremio = 0, totalCredito = 0;
                    String linhaHorizontal = "<hr/>";
                    UIHelper.ValorExtenso valEx = new UIHelper.ValorExtenso();
                    IList<SuperiorSubordinado> superiores = null;
                    IList<MovimentacaoContaCorrente> movimentacao = null;
                    UsuarioFilial usuarioFilial = null;

                    Listagem listagem = new Listagem(this.ListagemIDRequest);
                    listagem.Carregar();

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    DataTable dtRelacaoGrafo = LocatorHelper.Instance.ExecuteQuery("listagem_relacao_grafo.*, perfil_descricao FROM listagem_relacao_grafo INNER JOIN perfil ON perfil_id=listagemrelacaografo_superiorPerfilId WHERE listagemrelacaografo_listagemId=" + this.ListagemIDRequest + " AND listagemrelacaografo_superiorPerfilId IN (" + this.PerfilIDRequest + ") ORDER BY listagemrelacaografo_imediatoId", "relacaoGrafo").Tables[0]; //and listagemrelacaografo_superiorid=1640   listagemrelacaografo_superiorperfilid=8 and 

                    DataTable dtExclusosDoGrafo = LocatorHelper.Instance.ExecuteQuery("select usuario_nome,usuario_conta,usuario_agencia,usuario_banco,contacorrenteMovimentacao.*, perfil_id,perfil_descricao from contacorrenteMovimentacao inner join usuario on usuario_id=contacorrentemov_produtorId inner join perfil on perfil_id=usuario_perfilId  where contacorrentemov_produtorId not in (select listagemrelacaografo_superiorId from listagem_relacao_grafo where listagemrelacaografo_listagemId=" + this.ListagemIDRequest + ") and contacorrentemov_listagemFechamentoId=" + this.ListagemIDRequest + " order by contacorrentemov_produtorId", "relacaoGrafoExclusos").Tables[0];
                    List<string> exclusosAdd = new List<string>();
                    DataRow nova = null;
                    foreach (DataRow rowexc in dtExclusosDoGrafo.Rows)
                    {
                        if (exclusosAdd.Contains(Convert.ToString(rowexc["contacorrentemov_produtorId"]))) { continue; }

                        nova = dtRelacaoGrafo.NewRow();
                        nova["listagemrelacaografo_listagemId"] = this.ListagemIDRequest;
                        nova["listagemrelacaografo_superiorId"] = rowexc["contacorrentemov_produtorId"];
                        nova["listagemrelacaografo_superiorPerfilId"] = rowexc["perfil_id"];
                        nova["listagemrelacaografo_superiorBanco"] = rowexc["usuario_banco"];
                        nova["listagemrelacaografo_superioragencia"] = rowexc["usuario_agencia"];
                        nova["listagemrelacaografo_superiorconta"] = rowexc["usuario_conta"];
                        dtRelacaoGrafo.Rows.Add(nova);

                        exclusosAdd.Add(Convert.ToString(rowexc["contacorrentemov_produtorId"]));
                    }

                    if (dtRelacaoGrafo == null || dtRelacaoGrafo.Rows.Count == 0) { return; }

                    String[] produtorIDs = new String[dtRelacaoGrafo.Rows.Count];
                    for (Int32 count = 0; count < dtRelacaoGrafo.Rows.Count; count++) { produtorIDs[count] = Convert.ToString(dtRelacaoGrafo.Rows[count]["listagemrelacaografo_superiorPerfilId"]); }

                    DataTable dtRelacao = LocatorHelper.Instance.ExecuteQuery("listagem_relacao.*, usuario_documento1 FROM listagem_relacao INNER JOIN usuario ON listagemrelacao_produtorId=usuario_id WHERE listagemrelacao_listagemId=" + this.ListagemIDRequest + " AND  listagemrelacao_produtorPerfilId IN (" + String.Join(",", produtorIDs) + ") AND (listagemrelacao_percentualComissao <> '0,00' OR listagemrelacao_produtorCredito <> '0,00')", "relacao").Tables[0]; // and listagemrelacao_produtorId=3346 (listagemrelacao_contacorrenteMotivo IS NULL OR listagemrelacao_contacorrenteMotivo = '') AND 
                    //if (dtRelacao == null || dtRelacao.Rows.Count == 0) { return; }

                    if (Request["consolidadoBanco"] == null)
                    {
                        #region Listagem 

                        foreach (DataRow rwGrafo in dtRelacaoGrafo.Rows)
                        {
                            #region se o produtor nao tem comissionamento nem movimentacao na conta, ignora 

                            rows = dtRelacao.Select("listagemrelacao_produtorId=" + rwGrafo["listagemrelacaografo_superiorId"] + " AND (listagemrelacao_percentualComissao <> '0,00' OR listagemrelacao_produtorCredito <> '0,00')");
                            if (rows.Length == 0)
                            {
                                movimentacao = MovimentacaoContaCorrente.Carregar(rwGrafo["listagemrelacaografo_superiorId"], listagem.ID);
                                if (movimentacao == null)
                                {
                                    continue;
                                }
                            }

                            #endregion

                            totalPremio = 0; totalCredito = 0;

                            //sb.Append("<div style='width:100%' class=\"quebra\">");
                            sb.Append("<table width='100%'>");
                            sb.Append("<tr>");
                            sb.Append("<td>Demonstrativo de "); sb.Append(rwGrafo["perfil_descricao"]); sb.Append("</td>");
                            sb.Append("<td>Relatório "); sb.Append(agora.ToString("dd/MM/yyyy HH:mm:ss")); sb.Append("</td>");
                            sb.Append("</tr>");
                            sb.Append("</table>");
                            sb.Append(linhaHorizontal);

                            sb.Append("<table width='100%'>");
                            sb.Append("<tr>");

                            //FILIAL
                            sb.Append("<td>Filial ");
                            usuarioFilial = UsuarioFilial.CarregarVigente(rwGrafo["listagemrelacaografo_superiorId"], DateTime.Now, pm);
                            if (usuarioFilial != null)
                                sb.Append(usuarioFilial.FilialNome);
                            else
                                sb.Append("&nbsp;");
                            sb.Append("</td>");

                            //GRUPO
                            sb.Append("<td>Grupo ");
                            ret = LocatorHelper.Instance.ExecuteScalar("SELECT TOP 1 grupovenda_descricao ID FROM usuario_grupoVenda INNER JOIN grupo_venda ON grupovenda_id=usuariogvenda_grupovendaId WHERE usuariogvenda_usuarioId=" + rwGrafo["listagemrelacaografo_superiorId"] + " ORDER BY usuariogvenda_id DESC", null, null, pm);
                            if (ret != null && ret != DBNull.Value)
                                sb.Append(ret);
                            else
                                sb.Append("----");
                            //rows = dtRelacaoGrafo.Select("listagemrelacaografo_imediatoId=" + rwGrafo["listagemrelacaografo_superiorId"]);
                            //if (rows != null && rows.Length > 0)
                            //{
                            //    ret = rows[0]["listagemrelacaografo_superiorId"];
                            //    rows = dtRelacao.Select("listagemrelacao_produtorId = " + ret);
                            //    if(rows != null && rows.Length > 0)
                            //        sb.Append(rows[0]["listagemrelacao_produtorNome"]); 
                            //    else
                            //        sb.Append("----");
                            //}
                            //else
                            //    sb.Append("----");
                            sb.Append("</td>");

                            //NOME do produtor
                            rows = dtRelacao.Select("listagemrelacao_produtorId = " + rwGrafo["listagemrelacaografo_superiorId"]);

                            if (rows.Length > 0)
                            {
                                nomeProdutor = CToString(rows[0]["listagemrelacao_produtorNome"]);
                                documentoProdutor = Convert.ToString(rows[0]["usuario_documento1"]);
                                totalCredito = CToDecimal(rows[0]["listagemrelacao_produtorCredito"]);
                            }
                            else
                            {
                                Usuario tempprod = new Usuario(rwGrafo["listagemrelacaografo_superiorId"]);
                                pm.Load(tempprod);
                                nomeProdutor = tempprod.Nome;
                                documentoProdutor = tempprod.Documento1;
                            }
                            sb.Append("<td>Produtor "); sb.Append(nomeProdutor); sb.Append("</td>");
                            //Equipe
                            superiores = SuperiorSubordinado.CarregarSuperiores(rwGrafo["listagemrelacaografo_superiorId"], pm);
                            if (superiores != null && superiores.Count > 0)
                            {
                                sb.Append("<td>Equipe "); sb.Append(superiores[0].SuperiorApelido); sb.Append("</td>");
                            }
                            else
                            {
                                sb.Append("<td>&nbsp;</td> ");
                            }
                            superiores = null;
                            sb.Append("</tr>");
                            sb.Append("</table>");

                            sb.Append(linhaHorizontal);

                            #region DETALHADO POR OPERADORA / PROPOSTA

                            //cabeçalho
                            sb.Append("<table width='100%'>");
                            sb.Append("<tr>");
                            sb.Append("<td width='8%'>Ficha</td>");
                            sb.Append("<td width='35%'>Associado</td>");
                            sb.Append("<td width='70'>Adm</td>");
                            sb.Append("<td align='center' width='20'>P</td>");
                            sb.Append("<td width='70'>Pagto</td>");
                            sb.Append("<td align='center' width='60'>Valor Pg</td>");
                            sb.Append("<td width='40'>Com</td>");
                            sb.Append("<td align='center' width='60'>Prêmio</td>");
                            sb.Append("<td align='center' width='40'>Desc</td>");
                            sb.Append("<td align='center' width='60'>Val Desc</td>");
                            sb.Append("<td>Líquido</td>");
                            sb.Append("</tr>");
                            sb.Append("</table>");

                            //detail
                            rows = dtRelacao.Select("listagemrelacao_produtorId=" + rwGrafo["listagemrelacaografo_superiorId"], "listagemrelacao_operadoraNome ASC");
                            if (rows != null && rows.Length > 0)
                            {
                                sb.Append("<table width='100%'>");

                                aux = "";
                                foreach (DataRow row in rows)
                                {
                                    if (CToString(row["listagemrelacao_contacorrenteMotivo"]) != "") { continue; }

                                    if (aux != Convert.ToString(row["listagemrelacao_operadoraNome"]))
                                    {
                                        aux = Convert.ToString(row["listagemrelacao_operadoraNome"]);

                                        sb.Append("<tr>");
                                        sb.Append("<td colspan='11'><b>EMPRESA: </b>");
                                        sb.Append(aux);
                                        sb.Append("</td>");
                                        sb.Append("</tr>");
                                    }

                                    sb.Append("<tr>");
                                    sb.Append("<td width='8%'>"); sb.Append(row["listagemrelacao_contratoNumero"]); sb.Append("</td>");
                                    sb.Append("<td width='35%'>"); sb.Append(row["listagemrelacao_contratoNomeTitular"]); sb.Append("</td>");
                                    sb.Append("<td width='70'>"); sb.Append(row["listagemrelacao_contratoAdmissao"]); sb.Append("</td>");
                                    sb.Append("<td align='center' width='20'>"); sb.Append(row["listagemrelacao_cobrancaParcela"]); sb.Append("</td>");
                                    sb.Append("<td width='70'>"); sb.Append(row["listagemrelacao_cobrancaDataPago"]); sb.Append("</td>");
                                    sb.Append("<td align='center' width='60'>"); sb.Append(row["listagemrelacao_cobrancaValorPago"]); sb.Append("</td>");
                                    sb.Append("<td width='40'>"); sb.Append(row["listagemrelacao_percentualComissao"]); sb.Append("%</td>");
                                    sb.Append("<td align='center' width='60'>"); sb.Append(row["listagemrelacao_produtorValor"]); sb.Append("</td>");
                                    sb.Append("<td align='center' width='40'>0,00%</td>");
                                    sb.Append("<td align='center' width='60'>R$ 0,00</td>");
                                    sb.Append("<td>"); sb.Append(row["listagemrelacao_produtorValor"]); sb.Append("</td>");

                                    totalPremio += Convert.ToDecimal(row["listagemrelacao_produtorValor"]);
                                }

                                sb.Append("</table>");
                            }

                            #endregion

                            sb.Append(linhaHorizontal);

                            //SUBORDINADO 
                            if (rwGrafo["listagemrelacaografo_imediatoId"] != null && rwGrafo["listagemrelacaografo_imediatoId"] != DBNull.Value)
                            {
                                sb.Append(linhaHorizontal);
                            }

                            if (CToString(rwGrafo["listagemrelacaografo_superiorId"]) == "3387") { int k = 0; }

                            movimentacao = MovimentacaoContaCorrente.Carregar(rwGrafo["listagemrelacaografo_superiorId"], listagem.ID);
                            if (movimentacao != null)
                            {
                                foreach (MovimentacaoContaCorrente mcc in movimentacao)
                                {
                                    if (mcc.CategoriaTipo == Convert.ToInt32(CategoriaContaCorrente.eTipo.Credito))
                                    {
                                        totalCredito += mcc.Valor;
                                    }
                                    else if (mcc.CategoriaTipo == Convert.ToInt32(CategoriaContaCorrente.eTipo.Debito))
                                    {
                                        totalCredito -= mcc.Valor;
                                    }
                                }
                            }

                            #region TOTAIS E DADOS BANCÁRIOS

                            //CALCULA CREDITOS -- denis
                            //movimentacao = MovimentacaoContaCorrente.CarregarEmAberto(rwGrafo["listagemrelacaografo_superiorId"], listagem.DataCorte, null);
                            //if (movimentacao != null && movimentacao.Count > 0)
                            //{
                            //    totalCredito = MovimentacaoContaCorrente.Totalizar(movimentacao);
                            //}

                            sb.Append("<table>");
                            sb.Append("<tr>");
                            sb.Append("<td valign='top'>");

                            //EXIBICAO DOS DADOS DE BANCO
                            rows = dtRelacao.Select("listagemrelacao_produtorId=" + rwGrafo["listagemrelacaografo_superiorId"]);
                            sb.Append("<table >");
                            sb.Append("<tr>");
                            sb.Append("<td rowspan='3' width='185'><b>Dados para depósito bancário</b></td>");
                            sb.Append("<td width='85'><b>Banco: </b></td>");
                            sb.Append("<td >");
                            if (rows.Length > 0)
                                sb.Append(rows[0]["listagemrelacao_produtorBanco"]);
                            else
                                sb.Append("&nbsp;");

                            sb.Append("</td>");
                            sb.Append("</tr>");
                            sb.Append("<tr>");
                            sb.Append("<td><b>Agência: </b></td>");
                            sb.Append("<td>");
                            if(rows.Length > 0)
                                sb.Append(rows[0]["listagemrelacao_produtorAgencia"]); 
                            else
                                sb.Append("&nbsp;");

                            sb.Append("</td>");
                            sb.Append("</tr>");
                            sb.Append("<tr>");
                            sb.Append("<td><b>Núm. C/C: </b></td>");
                            sb.Append("<td>");
                            if(rows.Length > 0)
                                sb.Append(rows[0]["listagemrelacao_produtorConta"]);
                            else
                                sb.Append("&nbsp;");

                            sb.Append("</td>");
                            sb.Append("</tr>");
                            sb.Append("</table>");

                            sb.Append("</td>");
                            sb.Append("<td valign='top'>");

                            //EXIBICAO DOS TOTAIS
                            sb.Append("<table >");
                            sb.Append("<tr>");
                            sb.Append("<td rowspan='3' width='177'><b>Totais do "); sb.Append(Convert.ToString(rwGrafo["perfil_descricao"]).ToLower()); sb.Append("</b></td>");
                            sb.Append("<td width='100'><b>Prêmios: </b></td>");
                            sb.Append("<td width='95'>"); sb.Append(totalPremio.ToString("C")); sb.Append("</td>");
                            sb.Append("</tr>");
                            sb.Append("<tr>");
                            sb.Append("<td><b>Créditos: </b></td>");
                            sb.Append("<td>"); sb.Append(totalCredito.ToString("C")); sb.Append("</td>");
                            sb.Append("</tr>");
                            sb.Append("<tr>");
                            sb.Append("<td><b>Total Líquido: </b></td>");
                            sb.Append("<td>"); sb.Append((totalCredito + totalPremio).ToString("C")); sb.Append("</td>");
                            sb.Append("</tr>");
                            sb.Append("</table>");

                            sb.Append("</td></tr></table>");

                            #endregion

                            sb.Append(linhaHorizontal);
                            sb.Append("<b>");
                            sb.Append(listagem.Nome);
                            sb.Append("</b><br>");

                            #region RECIBO

                            if (Request["semRecibo"] == null || Request["semRecibo"] != "1")
                            {
                                sb.Append("<table border='1'>");
                                sb.Append("<tr>");
                                sb.Append("<td width='300' align='center'><h2>RECIBO DE COMISSÕES</h2></td>");
                                sb.Append("<td nowrap align='center'><h2>"); sb.Append((totalPremio + totalCredito).ToString("C")); sb.Append("</h2></td>");
                                sb.Append("</tr>");
                                sb.Append("<tr>");

                                //if (nomeProdutor == "SOLIDOM SAUDE E SEGUROS")
                                //{
                                //    int tmep = 0;
                                //}

                                //if ((totalPremio + totalCredito) < 0)
                                //{
                                    sb.Append("<td colspan='2'>Declaro para os devidos fins de direito que recebi da empresa QUALICORP ADMINISTRADORA DE BENEFÍCIOS LTDA. a quantia de ");
                                //}
                                //else
                                //{
                                //    sb.Append("<td colspan='2'>Declaro para os devidos fins de direito que ficou negativa a quantia de ");
                                //}
                                sb.Append((totalPremio + totalCredito).ToString("C"));
                                sb.Append(" (");
                                sb.Append(valEx.Extenso_Valor((totalPremio + totalCredito)));
                                sb.Append(") referente a pagamento de comissões conforme relatório em anexo.<br><br>");
                                sb.Append("<table width='100%'");
                                sb.Append("<tr>");
                                sb.Append("<td width='40%'>&nbsp;</td>");
                                sb.Append("<td align='center'>");
                                sb.Append(nomeProdutor);
                                sb.Append("<br>");
                                sb.Append(documentoProdutor);
                                sb.Append("</td>");
                                sb.Append("<td width='25'>&nbsp;</td>");
                                sb.Append("</table>");

                                sb.Append("</td>");
                                sb.Append("</tr>");
                                sb.Append("</table>");
                            }

                            #endregion

                            if (movimentacao != null)
                            {
                                sb.Append("<br><br><table border='1' width='100%'>");
                                sb.Append("<tr><td></td><td></td><td>Crédito</td><td>Débito</td></tr>");

                                foreach (MovimentacaoContaCorrente mcc in movimentacao)
                                {
                                    sb.Append("<tr>");
                                    sb.Append("<td>");
                                    sb.Append(mcc.Data.ToString("dd/MM/yyyy"));
                                    sb.Append("</td>");

                                    sb.Append("<td>");
                                    sb.Append(mcc.Motivo);
                                    sb.Append("</td>");

                                    sb.Append("<td>");
                                    if (mcc.CategoriaTipo == Convert.ToInt32(CategoriaContaCorrente.eTipo.Credito))
                                    {
                                        sb.Append(mcc.Valor.ToString("C"));
                                    }
                                    else
                                        sb.Append("&nbsp;");
                                    sb.Append("</td>");

                                    sb.Append("<td>");
                                    if (mcc.CategoriaTipo == Convert.ToInt32(CategoriaContaCorrente.eTipo.Debito))
                                    {
                                        sb.Append(mcc.Valor.ToString("C"));
                                    }
                                    else
                                        sb.Append("&nbsp;");
                                    sb.Append("</td>");
                                    sb.Append("</tr>");
                                }

                                sb.Append("</table>");
                            }
                            //}

                            sb.Append("<br clear=all style='mso-special-character:line-break;page-break-after:always'>");
                            //sb.Append("<p style=\"page-break-after: always\"></p>");//
                        }

                        #endregion
                    }
                    else
                    {
                        #region Bancos 

                        exclusosAdd = new List<string>();
                        foreach (DataRow rowexc in dtExclusosDoGrafo.Rows)
                        {
                            if (exclusosAdd.Contains(Convert.ToString(rowexc["contacorrentemov_produtorId"]))) { continue; }

                            nova = dtRelacao.NewRow();
                            nova["listagemrelacao_produtorId"] = rowexc["contacorrentemov_produtorId"];
                            nova["listagemrelacao_produtorNome"] = rowexc["usuario_nome"];
                            nova["listagemrelacao_produtorBanco"] = rowexc["usuario_banco"];
                            nova["listagemrelacao_produtorAgencia"] = rowexc["usuario_agencia"];
                            nova["listagemrelacao_produtorConta"] = rowexc["usuario_conta"];
                            nova["listagemrelacao_produtorValor"] = 0;
                            dtRelacao.Rows.Add(nova);

                            exclusosAdd.Add(Convert.ToString(rowexc["contacorrentemov_produtorId"]));
                        }

                        DataRow[] rowstemp = null;

                        List<String> bancos = new List<String>();
                        #region Separa os bancos 
                        foreach (DataRow row in dtRelacao.Rows)
                        {
                            if (!bancos.Contains(CToString(row["listagemrelacao_produtorBanco"]).Trim()))
                            {
                                bancos.Add(CToString(row["listagemrelacao_produtorBanco"]).Trim());
                            }
                        }
                        #endregion

                        List<String> produtores = new List<String>();
                        #region Separa os produtores 

                        foreach (DataRow row in dtRelacao.Rows)
                        {
                            if (!produtores.Contains(CToString(row["listagemrelacao_produtorId"]).Trim()))
                            {
                                produtores.Add(CToString(row["listagemrelacao_produtorId"]).Trim());
                            }
                        }

                        #endregion

                        Decimal totalBanco, totalProdutor;
                        String descricaoTotalBanco = "";

                        foreach (String banco in bancos) //Loop pelos bancos
                        {
                            #region cabecalho da tabela 
                            sb.Append("<table>");
                            sb.Append("<tr>");
                            sb.Append("<td>FILIAL</td>");
                            sb.Append("<td>PRODUTOR</td>");
                            sb.Append("<td>BANCO</td>");
                            sb.Append("<td>AG</td>");
                            sb.Append("<td>CONTA</td>");
                            sb.Append("<td>VALOR</td>");
                            sb.Append("<td>GRUPO</td>");
                            sb.Append("</tr>");
                            #endregion

                            totalBanco = 0;
                            if (banco == "") { descricaoTotalBanco = "TOTAL EM DINHEIRO: "; }
                            else { descricaoTotalBanco = "TOTAL DO BANCO " + banco + ": "; }

                            foreach (String produtorId in produtores) //Loop pelos produtores 
                            {
                                totalProdutor = 0;

                                //obtém as linhas para o banco corrente para o corretor corrente
                                rows = dtRelacao.Select(String.Concat("listagemrelacao_produtorBanco='", banco, "' AND listagemrelacao_produtorId=", produtorId));
                                if (rows.Length == 0) { continue; }

                                //FILIAL
                                sb.Append("<tr>");
                                sb.Append("<td>");
                                usuarioFilial = UsuarioFilial.CarregarVigente(produtorId, DateTime.Now, pm);
                                if (usuarioFilial != null)
                                    sb.Append(usuarioFilial.FilialNome);
                                else
                                    sb.Append("&nbsp;");
                                sb.Append("</td>");

                                //PRODUTOR
                                sb.Append("<td>");
                                sb.Append(rows[0]["listagemrelacao_produtorNome"]);
                                sb.Append("</td>");

                                //BANCO
                                sb.Append("<td>");
                                sb.Append(rows[0]["listagemrelacao_produtorBanco"]);
                                sb.Append("</td>");

                                //AGENCIA
                                sb.Append("<td>");
                                sb.Append(rows[0]["listagemrelacao_produtorAgencia"]);
                                sb.Append("</td>");

                                //CONTA
                                sb.Append("<td>");
                                sb.Append(rows[0]["listagemrelacao_produtorConta"]);
                                sb.Append("</td>");

                                //loop para somar o total (testar o metodo compute)
                                foreach (DataRow row in rows)
                                {
                                    //ATENCAO: checar se precisa checar pela coluna listagemrelacao_produtorCredito 
                                    totalProdutor += Convert.ToDecimal(row["listagemrelacao_produtorValor"]);
                                }

                                //processa conta corrente
                                movimentacao = MovimentacaoContaCorrente.Carregar(produtorId, listagem.ID);
                                if (movimentacao != null)
                                {
                                    foreach (MovimentacaoContaCorrente mcc in movimentacao)
                                    {
                                        rowstemp = dtRelacao.Select(String.Concat("listagemrelacao_contacorrenteId=", mcc.ID));
                                        if (rowstemp == null || rowstemp.Length == 0)
                                        {
                                            if (mcc.CategoriaTipo == Convert.ToInt32(CategoriaContaCorrente.eTipo.Credito))
                                            {
                                                totalProdutor += mcc.Valor;
                                            }
                                            else if (mcc.CategoriaTipo == Convert.ToInt32(CategoriaContaCorrente.eTipo.Debito))
                                            {
                                                totalProdutor -= mcc.Valor;
                                            }
                                        }
                                    }
                                }

                                //VALOR
                                sb.Append("<td>");
                                sb.Append(totalProdutor.ToString("C"));
                                sb.Append("</td>");

                                //GRUPO
                                sb.Append("<td>");
                                ret = LocatorHelper.Instance.ExecuteScalar("SELECT TOP 1 grupovenda_descricao FROM usuario_grupoVenda INNER JOIN grupo_venda ON grupovenda_id=usuariogvenda_grupovendaId WHERE usuariogvenda_usuarioId=" + produtorId + " ORDER BY usuariogvenda_id DESC", null, null, pm);
                                if (ret != null && ret != DBNull.Value)
                                    sb.Append(ret);
                                else
                                    sb.Append("&nbsp;");
                                sb.Append("</td>");
                                sb.Append("</tr>");

                                totalBanco += totalProdutor;
                            }

                            sb.Append("<tr><td colspan='7'>"); 
                            sb.Append(descricaoTotalBanco);
                            sb.Append(totalBanco.ToString("C"));
                            sb.Append("</td></tr></table><br>");
                        }

                        #endregion
                    }

                    pm.CloseSingleCommandInstance();
                    pm.Dispose();
                    dtRelacao.Dispose();
                    dtRelacaoGrafo.Dispose();
                    litReport.Text = sb.ToString();
                }
            }
        }
    }
}
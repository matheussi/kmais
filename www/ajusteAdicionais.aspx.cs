namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Framework.Phantom;
    using System.IO;

    public partial class ajusteAdicionais : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtPremio.Attributes.Add("onKeyUp", "mascara('" + txtPremio.ClientID + "')");
            txtCobertura.Attributes.Add("onKeyUp", "mascara('" + txtCobertura.ClientID + "')");

            if (!IsPostBack)
            {
                this.CarregaContratoADM();
                this.carregaAdicionaisPorTipo();
            }
        }

        void carregaAdicionaisPorTipo()
        {
            if (cboTipoAdicional.Items.Count == 0) return;

            //cboTipoAdicional.DataValueField = "ID";
            cboAdicional.DataValueField = "ID";
            cboAdicional.DataTextField = "Descricao";
            cboAdicional.DataSource = Adicional.Carregar(Convert.ToInt32(cboTipoAdicional.SelectedValue));
            cboAdicional.DataBind();
            cboAdicional.Items.Insert(0, new ListItem("todos", "-1"));


            if (cboTipoAdicional.SelectedValue == "0" || cboTipoAdicional.SelectedValue == "2") //taxa ou previdencia
            {
                txtCobertura.Enabled = false;
                txtCobertura.Text = "0,00";
                grid.Columns[6].Visible = true;
            }
            else
            {
                txtCobertura.Enabled = true;
                grid.Columns[6].Visible = false;
            }
        }

        void CarregaContratoADM()
        {
            cboContratoADM.Items.Clear();

            cboContratoADM.DataValueField = "ID";
            cboContratoADM.DataTextField = "DescricaoCodigoSaudeDental";

            IList<ContratoADM> lista = ContratoADM.CarregarTodos(4, false);
            cboContratoADM.DataSource = lista;
            cboContratoADM.DataBind();
            //cboContratoADM.Items.Insert(0, new ListItem("todos", "-1"));
        }

        DataTable dados
        {
            get
            {
                return ViewState["_dados"] as DataTable;
            }
            set
            {
                ViewState["_dados"] = value;
            }
        }

        protected void cboTipoAdicional_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaAdicionaisPorTipo();
        }

        protected void cmdAtribuirReajuste_Click(object sender, EventArgs e)
        {
            #region validacao 

            if (this.dados == null || this.dados.Rows.Count == 0)
            {
                base.Alerta(null, this, "_err", "Não há dados para fazer o reajuste.");
                return;
            }

            if (txtDescricao.Text.Trim() == "")
            {
                base.Alerta(null, this, "_err", "Informe uma descrição para o reajuste.");
                txtDescricao.Focus();
                return;
            }
            #endregion

            ReajusteCabecalho cab = new ReajusteCabecalho();
            List<ReajusteItem> itens = new List<ReajusteItem>();

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    #region salva cabeçalho 

                    cab.AdicionalId     = CToInt(cboAdicional.SelectedValue);
                    cab.Cobertura       = CToDecimal(txtCobertura.Text);
                    cab.ContratoADMId   = CToInt(cboContratoADM.SelectedValue);
                    cab.Descricao       = txtDescricao.Text;
                    cab.FormaPagto      = cboFormaPagto.SelectedValue;
                    cab.Premio          = CToDecimal(txtPremio.Text);
                    cab.Tipo            = CToInt(cboTipoAdicional.SelectedValue);
                    cab.TipoArquivo     = CToInt(cboDestino.SelectedValue);
                    cab.UsuarioID       = Usuario.Autenticado.ID;
                    //cad.NomeArquivo     = "";
                    pm.Save(cab);
                    #endregion

                    System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                    foreach (DataRow row in this.dados.Rows)
                    {
                        var item = new ReajusteItem();

                        item.AdicionalBeneficiarioID = row["adicionalbeneficiario_id"];
                        item.CabecalhoId = cab.ID;
                        item.ValorAtual = CToDecimal(row["valorAtual"], cinfo);
                        item.ValorReajustado = CToDecimal(row["valorReajuste"], cinfo);
                        pm.Save(item);

                        if (CToInt(row["adicional_tipo"]) == 0 || CToInt(row["adicional_tipo"]) == 2) //Taxa ou Previdencia
                        {
                            //Atualiza o valor //TODO: descomentar!

                            //NonQueryHelper.Instance.ExecuteNonQuery(
                            //    string.Concat("update adicional_beneficiario set adicionalbeneficiario_valor='", item.ValorReajustado.ToString("N2", cinfo).Replace(".", "").Replace(",", "."), "' where adicionalbeneficiario_id=", item.AdicionalBeneficiarioID), 
                            //    pm);
                        }
                        else //seguro
                        {
                            //TODO:
                        }

                        itens.Add(item);
                    }

                    pm.Commit();

                    #region escreve arquivo 

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    string CNPJBASEIF               = "00000000"; //??
                    string DATAREF                  = cab.Data.ToString("yyyyMMddHHmm"); //?? formato
                    String SEQIF                    = Convert.ToInt32(cab.ID).ToString().PadLeft(7, '0'); //??
                    String BaseCNPJConsignatario    = "12345678"; // 
                    String BaseCNPJCIP              = "02992335";

                    string nomeArquivo = string.Concat(
                        Convert.ToInt32(cab.ID).ToString().PadLeft(7, '0'),
                        "PSCC_", CNPJBASEIF, "_", DATAREF, "_", SEQIF);

                    sb.Append("0");                                 //IdentcLinha
                    sb.Append(nomeArquivo.PadRight(35, ' '));       //NCNPJBaseEmissor. 
                    sb.Append(BaseCNPJConsignatario);               //CNPJBaseEmissor. 
                    sb.Append(BaseCNPJCIP);                         //CNPJBaseDestinatario
                    sb.Append(Convert.ToInt32(cab.ID).ToString().PadLeft(20, '0')); //NumCtrlReq
                    sb.Append(cab.Data.ToString("yyyyMMdd"));       //DtRef
                    sb.Append(cab.Data.ToString("yyyyMMddHHmm"));   //DtHrArq
                    sb.Append(" ".PadRight(111, ' '));              //Brancos

                    int i = 0;
                    string cpf = "", matricula = "", valor = "0";
                    foreach (DataRow row in this.dados.Rows)
                    {
                        sb.Append(Environment.NewLine);
                        cpf = base.CToString(row["beneficiario_cpf"]).Replace(".", "").Replace("-", "");
                        matricula = base.CToString(row["beneficiario_matriculaFuncional"]).Replace(".", "").Replace("-", "");
                        valor = itens[i].ValorReajustado.ToString("N2").Replace(",", "").Replace(".", "").PadLeft(17, '0');

                        sb.Append("1");                                 //IdentcLinha
                        sb.Append("00000000");                          //?? IdentdPartAdmdo
                        sb.Append(Convert.ToInt32(cab.ID).ToString().PadLeft(20, '0')); //NumCtrlConsigrio
                        sb.Append("46379400");                          //CNPJBaseEnte
                        sb.Append("097NNN");                            //?? NumConsigrioEnte
                        sb.Append("  ");                                //NumDigtConsigrioEnte - não obrigatório
                        sb.Append(cpf.PadLeft(11, '0'));                //NumCPFServdr
                        sb.Append(matricula.PadLeft(15, '0'));          //IdentcServdr
                        sb.Append("0".PadRight(15, '0'));               //IdentcOrgao - não obrigatório
                        sb.Append(" ".PadRight(08, ' '));               //CNPJBaseOrgaoPagdr - não obrigatório
                        sb.Append("0000");                              //?? IdentcEsp
                        sb.Append("I");                                 //?? TpParcmnt - estou enviado interminado, tem problema?
                        sb.Append("000");                               //?? QtdTotParcl - estou enviado interminado, tem problema?
                        sb.Append("05");                                // DiaVencParcl
                        sb.Append("00000000");                          //?? DtIniAvebc - Data de Início da Averbação
                        sb.Append("        ");                          //DtFimAvebc - combinado com a especie interminada
                        sb.Append(valor);                               //VlrParcl
                        sb.Append("0".PadLeft(17, '0'));                //VlrTotAvebc - não obrigatório
                        sb.Append(CToInt(row["beneficiario_id"]).ToString().PadLeft(8, '0')); //NumOpConsigncConsigrio
                        sb.Append(" ".PadRight(21, ' '));               //NUAvebcSCC - não obrigatório
                        sb.Append(" ".PadRight(04, ' '));               //SitProc - não obrigatório
                        sb.Append(" ".PadRight(20, ' '));               //NumCtrlCIP - não obrigatório

                        i++;
                    }

                    sb.Append(Environment.NewLine);
                    sb.Append("9");                                                     //IdentcLinha
                    sb.Append((this.dados.Rows.Count + 2).ToString().PadLeft(9, '0'));  //QtdLinhas
                    sb.Append(" ".PadRight(193, ' '));                                  //Filler 


                    #endregion

                    this.dados.Clear();
                    this.dados.Dispose();
                }
                catch
                {
                    pm.Rollback();
                    throw;
                }
                finally
                {
                    pm.Dispose();
                }
            }

            base.Alerta(null, this, "_ok", "Reajuste aplicado com sucesso.");
        }

        protected void cmdVisualizar_click(object sender, EventArgs e)
        {
            string where = "";

            //if (cboContratoADM.SelectedIndex > 0)   { where = string.Concat(where, " and contratoadm_id=", cboContratoADM.SelectedValue, " "); }
            if (cboFormaPagto.SelectedIndex > 0)    { where = string.Concat(where, " and adicionalbeneficiario_formaPagto='", cboFormaPagto.SelectedValue, "' "); }
            if (cboAdicional.SelectedIndex > 0)     { where = string.Concat(where, " and adicional_id=", cboAdicional.SelectedValue, " "); }

            where = string.Concat(where, " and adicional_tipo=", cboTipoAdicional.SelectedValue, " ");

            string contratoAdmIds = ""; bool contratoSelecionado = false;

            foreach (ListItem item in cboContratoADM.Items)
            {
                if (item.Selected)
                {
                    if (!contratoSelecionado)
                    {
                        contratoAdmIds = item.Value; contratoSelecionado = true;
                    }
                    else
                    {
                        contratoAdmIds += "," + item.Value;
                    }
                }
            }

            where += " and contratoadm_id in (" + contratoAdmIds + ") ";

            if (!contratoSelecionado)
            {
                base.Alerta(null, this, "_err", "Selecione ao menos um órgão.");
                return;
            }

            string sql = string.Concat(
                "select contrato_numero,adicional_descricao,adicional_tipo,beneficiario_nome,contratoadm_descricao,adicional_descricao, adicional_beneficiario.*, ",
                "       pagto = case when adicionalbeneficiario_formaPagto = 31 then 'Boleto' when adicionalbeneficiario_formaPagto=9 then 'Crédito' when adicionalbeneficiario_formaPagto = 10 then 'Débito' when adicionalbeneficiario_formaPagto=81 then 'Desc. em conta' when adicionalbeneficiario_formaPagto=11 then 'Desconto em folha' else 'Indefinido' end, ",
                "       beneficiario_cpf,beneficiario_matriculaFuncional,beneficiario_id",
                "   from beneficiario ",
                "       inner join contrato_beneficiario on contratobeneficiario_beneficiarioId = beneficiario_id and contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 ",
                "       inner join contrato on contratobeneficiario_contratoId = contrato_id ",
                "       inner join contratoadm on contrato_contratoAdmId = contratoadm_id ",
                "       inner join adicional_beneficiario on adicionalbeneficiario_propostaId=contrato_id and adicionalbeneficiario_beneficiarioid = beneficiario_id ",
                "       inner join adicional on adicional_id = adicionalbeneficiario_adicionalid ",
                "   where ",
                "       contrato_legado=1 and (contrato_inativo=0 or contrato_inativo is null) and (contrato_cancelado=0 or contrato_cancelado is null) ",
                "       and ( (adicional_tipo=3) or ((adicional_tipo=0 or adicional_tipo=2) and (adicionalbeneficiario_status = 'A' or adicionalbeneficiario_status01 = 'A')) or (adicional_tipo=1 and (adicionalbeneficiario_stSgCob1 = 'A' or adicionalbeneficiario_stSgCob2 = 'A' or adicionalbeneficiario_stSgCob3 = 'A' or adicionalbeneficiario_stSgCob4 = 'A' or adicionalbeneficiario_stSgCob5 = 'A' or adicionalbeneficiario_stSgCob6 = 'A')) ) ",
                "       and adicionalbeneficiario_recorrente=1 ", where,
//                "       and beneficiario_nome='GERALDO SAMPAIO MAGALHAES' ",
                "   order by contratoadm_descricao,beneficiario_nome ");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(sql, "ds").Tables[0];
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            dt.Columns.Add("tipo"); dt.Columns.Add("valorAtual"); dt.Columns.Add("valorReajuste");

            foreach (DataRow row in dt.Rows)
            {
                row["tipo"] = cboTipoAdicional.SelectedItem.Text;

                if (CToInt(row["adicional_tipo"]) == 0 || CToInt(row["adicional_tipo"]) == 2)
                {
                    row["valorAtual"] = CToDecimal(row["adicionalbeneficiario_valor"], cinfo).ToString("N2");
                }
                else
                {
                    row["valorAtual"] = (
                        CToDecimal(row["adicionalbeneficiario_preCob1"], cinfo) +
                        CToDecimal(row["adicionalbeneficiario_preCob2"], cinfo) +
                        CToDecimal(row["adicionalbeneficiario_preCob3"], cinfo) +
                        CToDecimal(row["adicionalbeneficiario_preCob4"], cinfo) +
                        CToDecimal(row["adicionalbeneficiario_preCob5"], cinfo) +
                        CToDecimal(row["adicionalbeneficiario_preCob6"], cinfo)).ToString("N2");
                }

                row["valorReajuste"] = CToDecimal(CToDecimal(row["valorAtual"], cinfo) * (CToDecimal(txtPremio.Text, cinfo) / 100) + CToDecimal(row["valorAtual"], cinfo), cinfo).ToString("N2");
            }

            grid.DataSource = dt;
            grid.DataBind();

            this.dados = dt;
            pnlResultado.Visible = true;
        }

        protected void cmdToExcel_Click(object sender, EventArgs e)
        {
            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.Charset = "";

            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment;filename=\"reajuste.xls\"");

            DataTable final = new DataTable();

            //if (Request["b"] == null || Request["b"] != "1")
            //{
                final.Columns.Add("Orgao");
                final.Columns.Add("Matr.Funcional");
                final.Columns.Add("Nome");
                final.Columns.Add("Adicional");
                final.Columns.Add("Tipo Adicional");
                final.Columns.Add("Forma Pagto.");
                final.Columns.Add("valorAtual");
                final.Columns.Add("valorReajuste");

                DataRow nova = null;
                foreach (DataRow row in dados.Rows)
                {
                    nova = final.NewRow();

                    nova["Orgao"] = row["contratoadm_descricao"];
                    nova["Matr.Funcional"] = string.Concat("'", row["contrato_numero"]);
                    nova["Nome"] = row["beneficiario_nome"];
                    nova["Adicional"] = row["adicional_descricao"];
                    nova["Tipo Adicional"] = row["tipo"];
                    nova["Forma Pagto."] = row["pagto"];
                    nova["valorAtual"] = row["valorAtual"];
                    nova["valorReajuste"] = row["valorReajuste"];


                    final.Rows.Add(nova);
                }
            //}
            //else // relatório de órgãos
            //{
            //    final.Columns.Add("Orgao");
            //    final.Columns.Add("Matr.Funcional");
            //    final.Columns.Add("Nome");

            //    DataRow nova = null;
            //    foreach (DataRow row in dados.Rows)
            //    {
            //        nova = final.NewRow();

            //        nova["Orgao"] = row["contratoadm_descricao"];
            //        nova["Matr.Funcional"] = string.Concat("'", row["contrato_numero"]);
            //        nova["Nome"] = row["beneficiario_nome"];

            //        final.Rows.Add(nova);
            //    }
            //}

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    // instantiate a datagrid
                    DataGrid dg = new DataGrid();
                    //dg.AutoGenerateColumns = false;
                    dg.DataSource = final;
                    dg.DataBind();
                    dg.RenderControl(htw);
                    response.Write(sw.ToString());
                    response.End();
                }
            }
        }

        protected void grid_OnPageChanging(Object sender, GridViewPageEventArgs e)
        {
            if (e != null && e.NewPageIndex > -1)
            {
                this.grid.PageIndex = e.NewPageIndex;
                this.grid.DataSource = this.dados;
                this.grid.DataBind();
            }
        }
    }
}
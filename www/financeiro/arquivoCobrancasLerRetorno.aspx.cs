namespace www.financeiro
{
    using System;
    using System.IO;
    using System.Data;
    using System.Web.UI;
    using System.Web;
    using System.Collections;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    using LC.Framework.Phantom;
    using LC.Framework.DataUtil;

    public partial class arquivoCobrancasLerRetorno : PageBase
    {
        String Path
        {
            get { return String.Concat(Server.MapPath("/"), ConfigurationManager.AppSettings["financialFilePath"].Replace("/", "\\")); }
        }
        List<String> linhas
        {
            get { if (Session["_ln"] == null) { return new List<String>(); } else { return Session["_ln"] as List<String>; } }
            set { Session["_ln"] = value; }
        }

        Object outputId
        {
            get { return ViewState["_retId"]; }
            set { ViewState["_retId"]= value; }
        }
        List<CriticaRetornoVO> vos
        {
            get { if (Session["_vos"] == null) { return new List<CriticaRetornoVO>(); } else { return Session["_vos"] as List<CriticaRetornoVO>; } }
            set { Session["_vos"] = value; }
        }


        public override void VerifyRenderingInServerForm(Control control)
        {
            //if (!exportando)
            //{
            //    base.VerifyRenderingInServerForm(control);
            //}
        }

        protected override void  OnLoad(EventArgs e)
        {
 	         base.OnLoad(e);
             if (!IsPostBack)
             {
                 txtDe.Text  = DateTime.Now.AddDays(-59).ToString("dd/MM/yyyy");
                 txtAte.Text = DateTime.Now.ToString("dd/MM/yyyy");
                 this.CarregaOutputs();
             }
        }

        void CarregaOutputs()
        {
            DateTime de  = base.CStringToDateTime(txtDe.Text);
            DateTime ate = base.CStringToDateTime(txtAte.Text);

            if (de == DateTime.MinValue || ate == DateTime.MinValue || de > ate)
            {
                base.Alerta(null, this, "_errDt", "As datas informadas são inválidas.");
                return;
            }

            if (ate.Subtract(de).Days > 60)
            {
                base.Alerta(null, this, "_errDt", "O intervalo máximo entre as datas não pode exceder 60 dias.");
                return;
            }

            gridOutput.DataSource = RetornoOutput.CarregarTodos(de, ate);
            gridOutput.DataBind();
        }

        protected void cmdLer_Click(Object sender, EventArgs e)
        {
            if (uplFile.FileName == null) { return; }

            if (!Directory.Exists(this.Path))
            {
                Directory.CreateDirectory(this.Path);
            }

            String fileName = String.Concat("_ret_", DateTime.Now.ToString("yyyyMMddHHmmssffff"), ".txt");
            uplFile.SaveAs(String.Concat(this.Path, fileName));

            String arquivo = String.Concat(this.Path, fileName);

            String content = "";
            using (StreamReader stream = new StreamReader(arquivo))
            {
                content = stream.ReadToEnd();
                stream.Close();
            }

            RetornoInput ri = new RetornoInput();
            ri.ArquivoNome = uplFile.FileName;
            ri.Data = DateTime.Now;
            ri.Processado = false;
            ri.Texto = content;
            ri.TipoBanco = (int)Cobranca.eTipoBanco.Santander;
            if (optBancoBrasil.Checked) ri.TipoBanco = (int)Cobranca.eTipoBanco.BancoDoBrasil;
            //if (optUnibanco.Checked)
            //    ri.TipoBanco = (Int32)Cobranca.eTipoBanco.Unibanco;
            //else
            //    ri.TipoBanco = (Int32)Cobranca.eTipoBanco.Itau;

            ri.Salvar();

            File.Delete(String.Concat(this.Path, fileName));
            base.Alerta(null, this, "_retinp", "Arquivo enviado com sucesso.");

            #region comentado
            //Int32 titulosProcessados, titulosBaixados;
            //IList<CriticaRetornoVO> vos = ArquivoCobrancaUnibanco.ProcessaRetorno(mensagemLinha, out titulosProcessados, out titulosBaixados);
            //litMsg.Text = String.Concat("<b>Títulos processados: ", titulosProcessados, "  -  Títulos baixados: ", titulosBaixados, "</b>&nbsp;");
            //grid.DataSource = vos;
            //grid.DataBind();

            //Session[Session.SessionID + IDKey] = vos;
            //if (vos != null && vos.Count > 0) { cmdToExcel.Visible = true; cboStatus.Visible = true; } else { cmdToExcel.Visible = false; cboStatus.Visible = false; }
            #endregion
        }

        protected void cboStatus_Change(Object sender, EventArgs e)
        {
            List<CriticaRetornoVO> nova = filtraValueObjects(false);

            grid.DataSource = nova;
            grid.DataBind();
        }

        List<CriticaRetornoVO> filtraValueObjects(Boolean clonar)
        {
            if (vos == null) { return null; }

            //grid.Columns[7].Visible = false;

            List<CriticaRetornoVO> nova = new List<CriticaRetornoVO>();
            foreach (CriticaRetornoVO vo in vos)
            {
                #region 

                String carac = "'";

                switch (cboStatus.SelectedValue)
                {
                    case "0":
                    {
                        this.Adiciona(clonar, vo, nova, carac);
                        break;
                    }
                    case "1":
                    {
                        if (vo.ValorMenor)
                        {
                            this.Adiciona(clonar, vo, nova, carac);
                        }
                        break;
                    }
                    case "2":
                    {
                        if (vo.EmDuplicidade)
                        {
                            this.Adiciona(clonar, vo, nova, carac);
                        }
                        break;
                    }
                    case "3":
                    {
                        if (vo.PropostaInativa)
                        {
                            this.Adiciona(clonar, vo, nova, carac);
                        }
                        break;
                    }
                    case "4":
                    {
                        if (vo.PagamentoRejeitado)
                        {
                            this.Adiciona(clonar, vo, nova, carac);
                        }
                        break;
                    }
                    case "5":
                    {
                        grid.Columns[7].Visible = true;
                        if (vo.NaoLocalizado || String.IsNullOrEmpty(vo.OperadoraNome))
                        {
                            this.Adiciona(clonar, vo, nova, carac);
                        }
                        break;
                    }
                }
                #endregion
            }

            return nova;
        }

        void Adiciona(Boolean clonar, CriticaRetornoVO vo, List<CriticaRetornoVO> nova, String carac)
        {
            if (!clonar)
                nova.Add(vo);
            else
            {
                nova.Add((CriticaRetornoVO)vo.Clone());
                nova[nova.Count - 1].NossoNumero = carac + nova[nova.Count - 1].NossoNumero;
                nova[nova.Count - 1].PropostaNumero = carac + nova[nova.Count - 1].PropostaNumero;
            }
        }

        Boolean existeParcela(IList<Cobranca> cobrancas, Int32 parcela, int tipo, ref Object id)
        {
            foreach (Cobranca cob in cobrancas)
            {
                if (cob.Parcela == parcela && tipo == cob.Tipo) { id = cob.ID; return true; }
            }

            return false;
        }

        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("gerar"))
            {
                RetornoOutput rout = new RetornoOutput();
                rout.ID = outputId;
                rout.Carregar();

                List<CriticaRetornoVO> _vos = RetornoOutput.Desserializar(rout.SerializedValueObject);
                Int32 total = _vos.Count;

                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    Int32 index = i;
                    String nossoNumero = grid.Rows[index].Cells[1].Text;

                    Cobranca cobranca = new Cobranca();
                    cobranca.LeNossoNumero(nossoNumero);

                    if (cobranca.Tipo == 3) { continue; }

                    Contrato contrato = Contrato.CarregarParcialPorCodCobranca(cobranca.ContratoCodCobranca, null);

                    if (contrato == null)
                    {
                        //base.Alerta(null, this, "errNC1", "Proposta não pôde ser localizada.");
                        //return;
                        continue;
                    }

                    Object cobrancaId = null;

                    IList<Cobranca> cobrancas = Cobranca.CarregarPorNumeroDeContrato(contrato.OperadoraID, contrato.Numero, null);
                    if (cobrancas == null || ((cobrancas.Count + 1) != cobranca.Parcela))
                    {
                        if (cobrancas != null && existeParcela(cobrancas, cobranca.Parcela, cobranca.Tipo, ref cobrancaId))
                        {
                        }
                        else
                        {
                            //base.Alerta(null, this, "errNC2", "Número de parcela inválida.\\nA cobrança não pôde ser gerada.");
                            //return;
                            //if (cobranca.Tipo == (Int32)Cobranca.eTipo.Normal)
                            //{
                            //    continue;
                            //}
                        }
                    }

                    PersistenceManager pm = new PersistenceManager();
                    pm.UseSingleCommandInstance();

                    foreach (CriticaRetornoVO vo in _vos)
                    {
                        if (vo.NaoLocalizado || String.IsNullOrEmpty(vo.OperadoraNome) || vo.PagamentoRejeitado) { total--; }

                        if ((vo.NaoLocalizado || String.IsNullOrEmpty(vo.OperadoraNome)) && vo.NossoNumero == nossoNumero)
                        {
                            total++;

                            Operadora operadora = new Operadora(contrato.OperadoraID);
                            //operadora.Carregar();
                            pm.Load(operadora);

                            vo.PropostaNumero = contrato.Numero;
                            vo.OperadoraNome = operadora.Nome;
                            vo.NaoLocalizado = false;

                            rout.SerializedValueObject = RetornoOutput.Serializar(_vos);
                            this.vos = _vos;

                            Cobranca nova = new Cobranca();
                            nova.ID = cobrancaId;

                            if (nova.ID != null)
                            { 
                                //nova.Carregar();
                                pm.Load(nova);
                                //if (nova.Tipo == (int)Cobranca.eTipo.Dupla) { continue; }
                            }
                            else
                                nova.DataVencimento = vo.DataVencto;

                            nova.Cancelada = false;
                            nova.ComissaoPaga = false;
                            nova.ContratoCodCobranca = cobranca.ContratoCodCobranca;
                            nova.ContratoNumero = contrato.Numero;
                            nova.NossoNumero = vo.NossoNumero;
                            nova.Parcela = Convert.ToInt32(vo.Parcela);
                            nova.PropostaID = contrato.ID;
                            nova.Tipo = Convert.ToInt32(vo.CobrancaTipo);
                            nova.Valor = vo.Valor;

                            if (nova.ID == null)
                            {
                                nova.DataPgto = vo.DataPgto;
                                nova.ValorPgto = vo.ValorPgto;
                                nova.DataCriacao = DateTime.Now;
                                nova.Pago = true;
                                //nova.Salvar();
                                pm.Save(nova);

                                if (nova.Tipo == (Int32)Cobranca.eTipo.Dupla)
                                {
                                    //baixa aparcela "amarrada" à cobrança dupla
                                    Cobranca cobnormal = Cobranca.CarregarPor(nova.PropostaID, nova.Parcela - 1, (Int32)Cobranca.eTipo.Normal, pm);
                                    if (cobnormal != null && cobnormal.ID != null && !cobnormal.Pago)
                                    {
                                        cobnormal.ValorPgto = cobnormal.Valor;
                                        cobnormal.DataPgto = nova.DataPgto;
                                        cobnormal.Pago = true;
                                        //cobnormal.Salvar();
                                        pm.Save(cobnormal);

                                        nova.ValorPgto = nova.ValorPgto - cobnormal.ValorPgto;
                                        //nova.Salvar();
                                        pm.Save(nova);
                                    }
                                }
                            }
                            else if (!nova.Pago)
                            {
                                nova.DataPgto = vo.DataPgto;
                                nova.ValorPgto = vo.ValorPgto;
                                nova.Pago = true;
                                //nova.Salvar();
                                pm.Save(nova);

                                if (nova.Tipo == (Int32)Cobranca.eTipo.Dupla)
                                {
                                    //baixa aparcela "amarrada" à cobrança dupla
                                    Cobranca cobnormal = Cobranca.CarregarPor(nova.PropostaID, nova.Parcela - 1, (Int32)Cobranca.eTipo.Normal, pm);
                                    if (cobnormal != null && cobnormal.ID != null && !cobnormal.Pago)
                                    {
                                        cobnormal.ValorPgto = cobnormal.Valor;
                                        cobnormal.DataPgto = nova.DataPgto;
                                        cobnormal.Pago = true;
                                        //cobnormal.Salvar();
                                        pm.Save(cobnormal);

                                        nova.ValorPgto = nova.ValorPgto - cobnormal.ValorPgto;
                                        //nova.Salvar();
                                        pm.Save(nova);
                                    }
                                }
                            }
                            else if (nova.DataPgto.ToString("dd/MM/yyy") != vo.DataPgto.ToString("dd/MM/yyy"))
                            {
                                //Duplicidade
                                vo.EmDuplicidade = true;
                            }
                        }
                    }

                    rout.Descricao = rout.Descricao.Split('-')[0].Trim() + " - TÍTULOS: " + _vos.Count.ToString() + " - BAIXADOS: " + total.ToString();
                    //rout.Salvar();
                    pm.Save(rout);
                    total = _vos.Count;

                    pm.CloseSingleCommandInstance();
                    pm.Dispose();
                }

                this.CarregaOutputs();
                this.cboStatus_Change(null, null);
                this.Alerta(null, this, "okNC", "Cobrança gerada com sucesso.");
            }
        }

        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if(Server.HtmlDecode(e.Row.Cells[9].Text.ToLower()) == "tÍtulo baixado")
                {
                    e.Row.Cells[9].ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    e.Row.Cells[9].ForeColor = System.Drawing.Color.Red;
                }

                ((LinkButton)e.Row.Cells[10].Controls[0]).CommandArgument = e.Row.RowIndex.ToString();
                ((LinkButton)e.Row.Cells[10].Controls[0]).OnClientClick = "return confirm('Deseja realmente gerar essa cobrança?');";
            }
        }

        protected void gridOutput_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("ver"))
            {
                //ViewState["retoutid"] = gridOutput.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;

                RetornoOutput rout = new RetornoOutput();
                rout.ID = gridOutput.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                rout.Carregar();

                this.outputId = rout.ID;
                this.vos = RetornoOutput.Desserializar(rout.SerializedValueObject);

                if (Convert.ToInt32(rout.ID) == 914 || Convert.ToInt32(rout.ID) == 915 ||
                    Convert.ToInt32(rout.ID) == 916 || Convert.ToInt32(rout.ID) == 917 || 
                    Convert.ToInt32(rout.ID) == 918)
                {
                    PersistenceManager pm = new PersistenceManager();
                    pm.UseSingleCommandInstance();
                    System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
                    String qry = "select cobranca_valor from cobranca where cobranca_id=";
                    Object ret = null;

                    foreach (CriticaRetornoVO vo in this.vos)
                    {
                        if (vo.ValorMenor)
                        {
                            ret = LocatorHelper.Instance.ExecuteScalar(qry + vo.CobrancaID, null, null, pm);
                            vo.ValorMenor = false;
                            vo.Valor = Convert.ToDecimal(ret, cinfo);
                            vo.ValorPgto = vo.Valor;
                        }
                    }

                    pm.CloseSingleCommandInstance();
                    pm.Dispose();
                }

                grid.DataSource = vos;
                grid.DataBind();

                if (this.vos != null)
                {
                    Decimal valor = 0;
                    foreach (CriticaRetornoVO vo in this.vos)
                    {
                        valor += vo.ValorPgto;
                    }

                    litTotal.Text = "Total: " + valor.ToString("C");
                }

                gridOutput.Visible = false; 
                pnlFiltro.Visible = false;
                pnlDetalhe2.Visible = true;
            }
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            grid.DataSource = filtraValueObjects(false);
            grid.DataBind();
        }

        protected void cmdToExcel_Click(Object sende, EventArgs e)
        {
            //grid.DataSource = filtraValueObjects(true);
            //grid.DataBind();
            //String attachment = "attachment; filename=file.xls";
            //Response.ClearContent();
            //Response.AddHeader("content-disposition", attachment);
            //Response.ContentType = "application/ms-excel";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);
            //grid.RenderControl(htw);
            //Response.Write(sw.ToString());
            //Response.End(); 

            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.Charset = "";

            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment;filename=\"file.xls\"");

            IList<CriticaRetornoVO> lista = filtraValueObjects(true);
            
            DataTable dt = new DataTable();
            dt.Columns.Add("OperadoraNome");
            dt.Columns.Add("TitularNome");
            dt.Columns.Add("TitularCPF");
            dt.Columns.Add("NossoNumero");
            dt.Columns.Add("Parcela");
            dt.Columns.Add("DataVencto");
            dt.Columns.Add("Valor");
            dt.Columns.Add("ValorPgto");
            dt.Columns.Add("PropostaNumero");
            dt.Columns.Add("DataInativacaoCancelamento");
            dt.Columns.Add("Status");

            foreach (CriticaRetornoVO vo in lista)
            {
                DataRow nova = dt.NewRow();
                nova["OperadoraNome"]   = vo.OperadoraNome;
                nova["TitularNome"]     = vo.TitularNome;
                nova["TitularCPF"]      = vo.TitularCPF;
                nova["NossoNumero"]     = vo.NossoNumero;
                nova["Parcela"]         = vo.Parcela;
                nova["DataVencto"]      = vo.DataVencto;
                nova["Valor"]           = vo.Valor;
                nova["ValorPgto"]       = vo.ValorPgto;
                nova["PropostaNumero"]  = vo.PropostaNumero;
                nova["Status"]          = vo.Status;

                if(vo.DataInativacaoCancelamento != DateTime.MinValue)
                    nova["DataInativacaoCancelamento"] = vo.DataInativacaoCancelamento.ToString("dd/MM/yyyy");

                dt.Rows.Add(nova);
            }

            // create a string writer
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    // instantiate a datagrid
                    DataGrid dg = new DataGrid();
                    //dg.AutoGenerateColumns = false;
                    dg.DataSource = dt;
                    dg.DataBind();
                    dg.RenderControl(htw);
                    response.Write(sw.ToString());
                    response.End();
                }
            }

        }

        protected void opt_CheckedChanged(Object sender, EventArgs e)
        {
            grid.DataSource = null;
            grid.DataBind();
            pnlDetalhe2.Visible = false;
            gridOutput.Visible = true;
            pnlFiltro.Visible = true;

            if (optEnviar.Checked)
            {
                pnlEnviar.Visible  = true;
                pnlDetalhe.Visible = false;
            }
            else
            {
                pnlEnviar.Visible  = false;
                pnlDetalhe.Visible = true;
            }
        }

        protected void lnkFechar_Click(Object sender, EventArgs e)
        {
            this.outputId = null;
            gridOutput.Visible = true;
            pnlFiltro.Visible = true;
            pnlDetalhe2.Visible = false;
            cboStatus.SelectedIndex = 0;
            //grid.Columns[7].Visible = false;
        }

        protected void gridOutput_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            gridOutput.PageIndex = e.NewPageIndex;
            this.CarregaOutputs();
        }

        protected void cmdCarregaRetorno_Click(Object sender, EventArgs e)
        {
            gridOutput.PageIndex = 0;
            this.CarregaOutputs();
        }
    }
}

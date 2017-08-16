namespace www
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class arquivoAvisoCobranca : PageBase
    {
        protected override void  OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaTipos();
                this.CarregaOperadoras();
                this.CarregaCombosNumericos();
            }
        }

        void CarregaCombosNumericos()
        {
            base.PreencheComboNumerico(this.cboMes, 1, 12, true);
            base.PreencheComboNumerico(this.cboAno, DateTime.Now.AddYears(-1).Year, DateTime.Now.Year, true);
            cboAno.SelectedValue = DateTime.Now.Year.ToString();
        }

        void CarregaTipos()
        {
            cboTipo.Items.Clear();
            cboTipo.Items.Add(new ListItem("Aviso de Voz", "0"));
            cboTipo.Items.Add(new ListItem("Carta de cancelamento", "1"));
            cboTipo.Items.Add(new ListItem("Aviso SMS", "2"));
            cboTipo.Items.Add(new ListItem("Carta de reativação", "3"));
            cboTipo.Items.Add(new ListItem("Boleto por e-mail", "4"));
        }

        void CarregaOperadoras()
        {
            lstOperadoras.Items.Clear();
            lstOperadoras.SelectedIndex  = -1;
            lstOperadoras.DataValueField = "ID";
            lstOperadoras.DataTextField  = "Nome";

            if (cboTipo.SelectedValue != "3") //diferente de carta de reativacao - boleto DUPLO
                lstOperadoras.DataSource = Operadora.CarregarTodas(true);
            else
                lstOperadoras.DataSource = Operadora.CarregarTodasQueEnviamCartaCobranca();
            lstOperadoras.DataBind();
            foreach (ListItem item in lstOperadoras.Items)
                item.Selected = true;
        }

        protected void cboTipo_Changed(Object sender, EventArgs e)
        {
            this.CarregaOperadoras();
        }

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            #region validacoes 

            String[] operIDs = this.PegaIDsSelecionados(lstOperadoras);
            if (operIDs == null)
            {
                base.Alerta(null, this, "__errOpe", "Selecione ao menos uma operadora.");
                return;
            }

            #endregion

            if (cboTipo.SelectedValue == "4") //boleto por e-mail
            {
                pnlResultadoCobrancaDupla.Visible = false;

                IList<ArquivoAvisoCobranca.RetornoProcessamentoVO> vos =
                    ArquivoAvisoCobranca.GeraArquivo(((ArquivoAvisoCobranca.eTipoAviso)Convert.ToInt32(cboTipo.SelectedValue)),
                    operIDs, Convert.ToInt32(cboMes.SelectedValue), Convert.ToInt32(cboAno.SelectedValue));

                ViewState[ArquivosObjKey] = vos;

                grid.DataSource = vos;
                grid.DataBind();

                if (vos == null || vos.Count == 0)
                {
                    litMsg.Text = "Nenhum cliente localizado.";
                    pnlResultado.Visible = false;
                }
                else
                {
                    litMsg.Text = "Resultado:";
                    pnlResultado.Visible = true;
                }
            }
            else if (cboTipo.SelectedValue != "3") //diferente de carta de reativacao - boleto DUPLO
            {
                pnlResultadoCobrancaDupla.Visible = false;

                IList<ArquivoAvisoCobranca.RetornoProcessamentoVO> vos =
                    ArquivoAvisoCobranca.GeraArquivo(((ArquivoAvisoCobranca.eTipoAviso)Convert.ToInt32(cboTipo.SelectedValue)),
                    operIDs, Convert.ToInt32(cboMes.SelectedValue), Convert.ToInt32(cboAno.SelectedValue));

                ViewState[ArquivosObjKey] = vos;

                grid.DataSource = vos;
                grid.DataBind();

                if (vos == null || vos.Count == 0)
                {
                    litMsg.Text = "Nenhum cliente localizado.";
                    pnlResultado.Visible = false;
                }
                else
                {
                    litMsg.Text = "Resultado:";
                    pnlResultado.Visible = true;
                }
            }
            else
            {
                #region carta de reativacao 

                pnlResultado.Visible = false;

                List<SumarioArquivoGeradoVO> vos =
                    ArquivoCobrancaUnibanco.GeraDocumentoCobrancaDUPLA_UNIBANCO(operIDs,
                    Convert.ToInt32(cboMes.SelectedValue), Convert.ToInt32(cboAno.SelectedValue));

                ViewState[ArquivosObjKey] = vos;

                grid2.DataSource = vos;
                grid2.DataBind();

                if (vos == null || vos.Count == 0)
                {
                    litMsg.Text = "Nenhuma cobrança localizada.";
                    pnlResultadoCobrancaDupla.Visible = false;
                }
                else
                {
                    litMsg.Text = "Resultado:";
                    pnlResultadoCobrancaDupla.Visible = true;
                }
                #endregion
            }
        }

        protected void grid_RowCreated(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton imbDownloadArquivoButton = (ImageButton)e.Row.FindControl("imbDownloadArquivo");
                imbDownloadArquivoButton.CommandArgument = e.Row.RowIndex.ToString();
                ScriptManager.GetCurrent(this).RegisterPostBackControl(imbDownloadArquivoButton);
            }
        }
        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("baixar"))
            {
                List<ArquivoAvisoCobranca.RetornoProcessamentoVO> vos = (List<ArquivoAvisoCobranca.RetornoProcessamentoVO>)ViewState[ArquivosObjKey];
                ArquivoAvisoCobranca.RetornoProcessamentoVO vo = vos[Convert.ToInt32(e.CommandArgument)];
                String nomeArquivo = String.Concat("_", vo.ArquivoAvisoID, "_", DateTime.Now.ToString("ddMMyyHHmmssfff"), "_", vo.TipoAviso, ".txt");
                BaixaArquivoOther(nomeArquivo, vo.ArquivoConteudo);
            }
            else if (e.CommandName.Equals("setarComoEnviado"))
            {
                Object id = grid.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[0];
                if (id == null) { return; }

                ArquivoAvisoCobranca.SetarComoProcessado(id);
                base.Alerta(null, this, "_envOk", "Operação feita com sucesso.");
            }
        }
        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (grid.DataKeys[e.Row.RowIndex].Values[0] == null) //se ja foi enviado, oculta os botoes
                {
                    e.Row.Cells[1].Controls[1].Visible = false;
                    e.Row.Cells[2].Controls[0].Visible = false;
                }
                else
                {
                    base.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja setar este arquivo como \\'enviado\\'?\\nEsta operação não poderá ser desfeita.");
                }
            }
        }

        protected void BaixaArquivoOther(String arquivoNome, String conteudo)
        {
            if (!String.IsNullOrEmpty(arquivoNome))
            {
                String ArquivoCaminho = Server.MapPath("/") + ConfigurationManager.AppSettings["otherFilePath"].Replace("/", @"\");
                
                if (!System.IO.Directory.Exists(ArquivoCaminho))
                {
                    System.IO.Directory.CreateDirectory(ArquivoCaminho);
                }

                String strFilePath = String.Concat(ArquivoCaminho, arquivoNome);

                try
                {
                    File.WriteAllText(strFilePath, conteudo, System.Text.Encoding.GetEncoding("iso-8859-1"));
                }
                catch (Exception) { throw; }

                //System.IO.File.WriteAllText(strFilePath, content, System.Text.Encoding.ASCII);

                System.IO.FileStream fileStream = null;

                try
                {
                    fileStream = new System.IO.FileStream(strFilePath, System.IO.FileMode.Open);
                }
                catch (System.IO.FileNotFoundException)
                {
                    throw;
                }

                Byte[] arrByte = new Byte[fileStream.Length];
                fileStream.Read(arrByte, 0, arrByte.Length);
                fileStream.Close();
                fileStream.Dispose();
                fileStream = null;

                this.Response.Clear();
                this.Response.ContentType = "application/octet-stream";
                this.Response.AppendHeader("Content-Length", arrByte.Length.ToString());
                this.Response.AppendHeader("Content-Disposition", String.Concat("attachment; filename=", arquivoNome));
                this.Response.BinaryWrite(arrByte);
                this.Response.Flush();
            }
        }

        protected void grid2_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ImageButton imbDownloadArquivoButton = (ImageButton)e.Row.FindControl("imbDownloadArquivo");
                imbDownloadArquivoButton.CommandArgument = e.Row.RowIndex.ToString();
                ScriptManager.GetCurrent(this).RegisterPostBackControl(imbDownloadArquivoButton);
            }
        }
        protected void grid2_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("baixar"))
            {
                IList<SumarioArquivoGeradoVO> vos = (IList<SumarioArquivoGeradoVO>)ViewState[ArquivosObjKey];
                SumarioArquivoGeradoVO vo = vos[Convert.ToInt32(e.CommandArgument)];

                base.BaixaArquivoFinanceiro(vo.ArquivoNome);
            }
        }
        protected void grid2_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
        }
    }
}
﻿namespace www.reports
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using System.Data.SqlClient;
    using LC.Web.PadraoSeguros.Entity;

    public partial class controladoriaTaxa : PageBase
    {
        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaAgenda();
                this.CarregaEstipulantes();
                this.CarregaOperadoras();

                base.PreencheComboNumerico(cboHora, 0, 23, true);
                base.PreencheComboNumerico(cboMinuto, 0, 59, true);

                txtProcessarEm.Text = DateTime.Now.ToString("dd/MM/yyyy");
                cboHora.SelectedValue = DateTime.Now.Hour.ToString();
                cboMinuto.SelectedValue = DateTime.Now.Minute.ToString();
            }
        }

        void CarregaAgenda()
        {
            gridAgenda.DataSource = AgendaRelatorio.CarregarPorUsuario(Usuario.Autenticado.ID, AgendaRelatorio.eTipo.ControladoriaTaxa);
            gridAgenda.DataBind();

            if (gridAgenda.Rows.Count > 0) pnlAgenda.Visible = true;
            else pnlAgenda.Visible = false;
        }
        void CarregaEstipulantes()
        {
            IList<Estipulante> estipulantes = Estipulante.Carregar(false);

            this.lstEstipulantes.DataValueField = "ID";
            this.lstEstipulantes.DataTextField = "Descricao";
            this.lstEstipulantes.DataSource = estipulantes;
            this.lstEstipulantes.DataBind();
        }
        void CarregaOperadoras()
        {
            IList<Operadora> operadoras = Operadora.CarregarTodas(false);

            this.lstOperadora.DataValueField = "ID";
            this.lstOperadora.DataTextField = "Nome";
            this.lstOperadora.DataSource = operadoras;
            this.lstOperadora.DataBind();
        }

        protected void cmdGerar_Click(Object sender, EventArgs e)
        {
            DateTime dtFrom = new DateTime();
            DateTime dtTo = new DateTime();
            UIHelper.TyParseToDateTime(txtDe.Text, out dtFrom);
            UIHelper.TyParseToDateTime(txtAte.Text, out dtTo);

            String[] oper = base.PegaIDsSelecionados(lstOperadora);
            String[] estp = base.PegaIDsSelecionados(lstEstipulantes);

            if (estp == null || oper == null || dtFrom == DateTime.MinValue || dtTo == DateTime.MinValue)
            { base.Alerta(null, this, "_err", "Todos os parâmetros são requeridos."); return; }

            if (!chkAgendar.Checked)
            {
                #region relatorio 

                String qry = AgendaRelatorio.ControladoriaTaxaQUERY(dtFrom, dtTo, oper, estp);
                DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];

                grid.DataSource = dt;
                grid.DataBind();

                if (dt.Rows.Count == 0)
                    pnl.Visible = false;
                else
                    pnl.Visible = true;

                #endregion relatorio
            }
            else
            {
                DateTime processarEm = base.CStringToDateTime(
                    txtProcessarEm.Text, cboHora.SelectedValue + ":" + cboMinuto.SelectedValue);

                if (processarEm == DateTime.MinValue)
                {
                    base.Alerta(null, this, "_err", "Data de agendamento inválida."); return;
                }

                AgendaRelatorio obj = new AgendaRelatorio();
                obj.DataAte = dtTo;
                obj.DataDe = dtFrom;
                obj.EstipulanteIDs = String.Join(",", estp);
                obj.OperadoraIDs = String.Join(",", oper);
                obj.ProcessarEm = processarEm;
                obj.Tipo = (int)AgendaRelatorio.eTipo.ControladoriaTaxa;
                obj.UsuarioID = Usuario.Autenticado.ID;
                obj.Salvar();

                this.CarregaAgenda();
                base.Alerta(null, this, "_ok", "Agendamento realizado com sucesso."); return;
            }
        }

        protected void cmdToExcel_Click(Object sender, ImageClickEventArgs e)
        {
            String attachment = "attachment; filename=file.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        protected void gridAgenda_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "arquivo")
            {
                Object id = gridAgenda.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                BaixaArquivo_REPORT(id + ".csv");
            }
            else if (e.CommandName == "excluir")
            {
                Object id = gridAgenda.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                AgendaRelatorio obj = new AgendaRelatorio(id);
                try
                {
                    obj.Remover();
                    this.CarregaAgenda();
                    base.Alerta(null, this, "_ok", "Exclusão realizada com sucesso.");
                }
                catch
                {
                    base.Alerta(null, this, "_delErr", "Não foi possível excluir o item.");
                }
            }
        }

        protected void gridAgenda_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Boolean processado = Convert.ToBoolean(gridAgenda.DataKeys[e.Row.RowIndex][1]);
                if (!processado) 
                {
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Enabled = false;
                    ((LinkButton)e.Row.Cells[2].Controls[0]).Visible = false;
                }

                if(!processado)
                    base.grid_RowDataBound_Confirmacao(sender, e, 3, "Deseja excluir o agendamento?");
                else
                    base.grid_RowDataBound_Confirmacao(sender, e, 3, "Deseja excluir o relatório?");

            //    ((Image)e.Row.Cells[4].Controls[1]).Visible = rascunho;

            //    Boolean cancelado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][2]);
            //    Boolean inativado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][3]);

            //    UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[5].Controls[0], new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.PropostaBeneficiarioIDKey });
            //    UIHelper.AuthCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
            //    UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
            //    base.grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja realmente prosseguir com a exclusão?\\nEssa operação não poderá ser desfeita.");

            //    if (Usuario.Autenticado.PerfilID != Perfil.AdministradorIDKey) { gridContratos.Columns[7].Visible = false; }

            //    if (cancelado || inativado)
            //    {
            //        e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
            //        ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../images/unactive.png' title='inativo' alt='inativo' border='0'>";
            //        //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente ativar o contrato?");
            }
            //    else
            //    {
            //        //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente cancelar o contrato?");
            //        ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../images/active.png' title='ativo' alt='ativo' border='0'>";
            //    }
            //}
        }

        protected void gridAgenda_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            //gridContratos.PageIndex = e.NewPageIndex;
            //this.CarregaContratos();
        }

        protected void chkAgendar_CheckedChanged(Object sender, EventArgs e)
        {
            if (chkAgendar.Checked)
            {
                cmdGerar.Text = "Agendar";
                trProcessarEm.Visible = true;
            }
            else
            {
                cmdGerar.Text = "Gerar";
                trProcessarEm.Visible = false;
            }
        }

        protected void BaixaArquivo_REPORT(String arquivoNome)
        {
            if (!String.IsNullOrEmpty(arquivoNome))
            {
                String ArquivoCaminho = Server.MapPath("/") + ConfigurationManager.AppSettings["reports_file"].Replace("/", @"\");

                String strFilePath = String.Concat(ArquivoCaminho, arquivoNome);

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
    }
}

namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using LC.Web.PadraoSeguros.Entity;

    public partial class comissionamentos : PageBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.CarregaPerfis();
                this.CarregaTabelas();
            }
        }

        void CarregaPerfis()
        {
            IList<Perfil> lista = Perfil.CarregarTodos(new Perfil.eTipo[] { Perfil.eTipo.Produtor, Perfil.eTipo.Telemarketing }, true);

            cboPerfis.Items.Clear();
            cboPerfis.DataValueField = "ID";
            cboPerfis.DataTextField = "Descricao";
            cboPerfis.DataSource = lista;
            cboPerfis.DataBind();
            cboPerfis.Items.Insert(0, new ListItem("TODOS", "-1"));
        }

        void CarregaTabelas()
        {
            if(cboPerfis.SelectedValue == "-1")
                gridTabelas.DataSource = Comissionamento.CarregarTodos(Comissionamento.eTipo.PagoAoOperador);// .CarregarPorContratoId(cboContrato.SelectedValue, Comissionamento.eTipo.PagoAoOperador);
            else
                gridTabelas.DataSource = Comissionamento.CarregarTodos(Comissionamento.eTipo.PagoAoOperador, cboPerfis.SelectedValue);
            gridTabelas.DataBind();
        }

        protected void cboPerfis_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelas();
        }

        protected void gridTabelas_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            gridTabelas.PageIndex = e.NewPageIndex;
            this.CarregaTabelas();
        }

        protected void gridTabelas_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session[IDKey] = gridTabelas.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("comissionamento.aspx?", IDKey, "=", Session[IDKey]));
            }
            else if (e.CommandName == "excluir")
            {
                try
                {
                    Comissionamento com = new Comissionamento();
                    com.ID = gridTabelas.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                    com.Remover();
                    this.CarregaTabelas();
                }
                catch
                {
                    base.Alerta(null, this, "_delErr", "Não foi possível excluir a tabela.");
                }
            }
        }

        protected void gridTabelas_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            base.grid_RowDataBound_Confirmacao(sender, e, 3, "Deseja realmente prosseguir com a exclusão?\\nEssa operação não poderá ser desfeita.");
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(3000);
            Response.Redirect("~/comissionamento.aspx");
        }

        protected void cboOperadoras_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelas();
        }

        protected void cboContrato_SelectedIndexChanged(Object sender, EventArgs e)
        {
            this.CarregaTabelas();
        }
    }
}
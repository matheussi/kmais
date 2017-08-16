namespace www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    public partial class user : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                this.PreencheGrid1();
                this.PreencheGrid2();
            }
        }

        void PreencheGrid1()
        {
            #region grid 1

            DataTable dt = new DataTable();
            dt.Columns.Add("Código");
            dt.Columns.Add("Comissionamento");
            dt.Columns.Add("Ativo");

            DataRow linha1 = dt.NewRow();
            linha1["Código"] = "00001";
            linha1["Comissionamento"] = "UNIMED M1 pOURO";
            linha1["Ativo"] = "SIM";

            dt.Rows.Add(linha1);

            DataRow linha2 = dt.NewRow();
            linha2["Código"] = "00001";
            linha2["Comissionamento"] = "UNIMED M1 p1231";
            linha2["Ativo"] = "SIM";

            dt.Rows.Add(linha2);

            DataRow linha3 = dt.NewRow();
            linha3["Código"] = "00001";
            linha3["Comissionamento"] = "UNIMED M1 p1232";
            linha3["Ativo"] = "SIM";

            dt.Rows.Add(linha3);

            DataRow linha4 = dt.NewRow();
            linha4["Código"] = "00001";
            linha4["Comissionamento"] = "UNIMED M1 p1233";
            linha4["Ativo"] = "SIM";

            dt.Rows.Add(linha4);

            gridComissionamento.DataSource = dt;
            gridComissionamento.DataBind();

            #endregion
        }

        void PreencheGrid2()
        {
            #region grid 2

            DataTable dt = new DataTable();
            dt.Columns.Add("Parcela");
            dt.Columns.Add("Imposto");
            dt.Columns.Add("Comissão");

            DataRow linha1 = dt.NewRow();
            linha1["Parcela"] = "01";
            linha1["Imposto"] = "0,00%";
            linha1["Comissão"] = "20,00%";
            dt.Rows.Add(linha1);

            DataRow linha2 = dt.NewRow();
            linha2["Parcela"] = "02";
            linha2["Imposto"] = "0,00%";
            linha2["Comissão"] = "15,00%";
            dt.Rows.Add(linha2);

            DataRow linha3 = dt.NewRow();
            linha3["Parcela"] = "03";
            linha3["Imposto"] = "0,00%";
            linha3["Comissão"] = "15,00%";
            dt.Rows.Add(linha3);

            DataRow linha4 = dt.NewRow();
            linha4["Parcela"] = "04";
            linha4["Imposto"] = "0,00%";
            linha4["Comissão"] = "15,00%";
            dt.Rows.Add(linha4);

            DataRow linha5 = dt.NewRow();
            linha5["Parcela"] = "05";
            linha5["Imposto"] = "0,00%";
            linha5["Comissão"] = "00,00%";
            dt.Rows.Add(linha5);

            DataRow linha6 = dt.NewRow();
            linha6["Parcela"] = "06";
            linha6["Imposto"] = "0,00%";
            linha6["Comissão"] = "00,00%";
            dt.Rows.Add(linha6);

            DataRow linha7 = dt.NewRow();
            linha7["Parcela"] = "07";
            linha7["Imposto"] = "0,00%";
            linha7["Comissão"] = "00,00%";
            dt.Rows.Add(linha7);

            DataRow linha8 = dt.NewRow();
            linha8["Parcela"] = "08";
            linha8["Imposto"] = "0,00%";
            linha8["Comissão"] = "00,00%";
            dt.Rows.Add(linha8);

            DataRow linha9 = dt.NewRow();
            linha9["Parcela"] = "09";
            linha9["Imposto"] = "0,00%";
            linha9["Comissão"] = "00,00%";
            dt.Rows.Add(linha9);

            DataRow linha10 = dt.NewRow();
            linha10["Parcela"] = "10";
            linha10["Imposto"] = "0,00%";
            linha10["Comissão"] = "00,00%";
            dt.Rows.Add(linha10);

            DataRow linha11 = dt.NewRow();
            linha11["Parcela"] = "11";
            linha11["Imposto"] = "0,00%";
            linha11["Comissão"] = "00,00%";
            dt.Rows.Add(linha11);

            DataRow linha12 = dt.NewRow();
            linha12["Parcela"] = "12";
            linha12["Imposto"] = "10,00%";
            linha12["Comissão"] = "00,00%";
            dt.Rows.Add(linha12);

            gridComissionamentoDetalhe.DataSource = dt;
            gridComissionamentoDetalhe.DataBind();

            #endregion
        }

        protected void gridComissionamento_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "detalhes")
            {
                gridComissionamento.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                tab.TabIndex = 1;
            }
        }
    }
}
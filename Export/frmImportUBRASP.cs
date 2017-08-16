using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

using LC.Framework.DataUtil;
using LC.Framework.BusinessLayer;
using LC.Framework.Phantom;

using LC.Web.PadraoSeguros.Entity;

namespace Export
{
    public partial class frmImportUBRASP : Form
    {
        public frmImportUBRASP()
        {
            InitializeComponent();

            //string aux = "14011936";
            //string dia = aux.Substring(0, 2);
            //string mes = aux.Substring(2, 2);
            //string ano = aux.Substring(4, 4);
        }

        private void cmdPessoas_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja iniciar a importação de pessoas e endereços?", "Importar pessoas", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return;

            lblPessoas.Text = "Carregando dados fonte...";
            Application.DoEvents();

            //string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\APP15D1.accdb;Persist Security Info=False");
            string connAccess = string.Concat(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\APP15D1.mdb;Persist Security Info=False");
            DataTable dt = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [dados] order by id", connection); 
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            lblPessoas.Text = string.Concat("Total de registros: ", dt.Rows.Count);
            Application.DoEvents();

            Beneficiario b = null;
            Endereco en = null;

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    i++;
                    lblPessoas.Text = string.Concat("Registro ", i, " de ", dt.Rows.Count);
                    Application.DoEvents();

                    b = new Beneficiario();
                    en = new Endereco();

                    b.CPF = toString(row["CPF"]);
                    b.DataNascimento = toDataNascimento(row["DT_NASC"]);
                    b.Iniciais = toString(row["INICIAIS"]);
                    b.MatriculaAssociativa = toString(row["MAT_ASS"]);
                    b.MatriculaFuncional = toString(row["MAT_FUNC"]);
                    b.Nome = toString(row["NOME"]);
                    b.Sexo = toString(row["SEXO"]) == "M" ? "1" : "2";
                    b.Telefone = toTelefone(row["TELEFONE"]);

                    pm.Save(b);

                    en.Bairro = toString(row["BAIRRO"]);
                    en.CEP = toString(row["CEP"]).PadLeft(8, '0');
                    en.Cidade = toString(row["CIDADE"]);
                    en.Complemento="";
                    if (toString(row["BLOCO"]) != "")
                    {
                        en.Complemento = string.Concat("Bloco ", row["BLOCO"]);
                    }
                    if (toString(row["COMPLEMENTO"]) != "")
                    {
                        if (en.Complemento.Length > 0) { en.Complemento += " - "; }
                        en.Complemento += toString(row["COMPLEMENTO"]);
                    }

                    en.DonoId = b.ID;
                    en.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                    en.Logradouro = toString(row["ENDERECO"]);
                    en.Numero = null;
                    en.Tipo = (int)Endereco.TipoEndereco.Residencial;
                    en.UF = toString(row["UF"]);

                    pm.Save(en);
                }

                pm.Commit();
                lblPessoas.Text = "Concluído.";
                Application.DoEvents();
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

        string toString(object param)
        {
            if (param == null || param == DBNull.Value)
                return string.Empty;
            else
                return Convert.ToString(param);
        }

        DateTime toDataNascimento(object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim().Length != 8)
                return DateTime.MinValue;
            else
            {
                string aux = Convert.ToString(param);

                if (aux.IndexOf("/") > -1)
                {
                    string[] arr = aux.Split('/');
                    if (arr[0].Length == 2)
                        return new DateTime(Int32.Parse(arr[2]), Int32.Parse(arr[1]), Int32.Parse(arr[0]));
                    else
                        return new DateTime(Int32.Parse(arr[0]), Int32.Parse(arr[1]), Int32.Parse(arr[2]));
                }
                else
                {
                    string ano = aux.Substring(0, 4);
                    string mes = aux.Substring(4, 2);
                    string dia = aux.Substring(6, 2);

                    try
                    {
                        return new DateTime(Int32.Parse(ano), Int32.Parse(mes), Int32.Parse(dia));
                    }
                    catch
                    {
                        try
                        {
                            dia = aux.Substring(0, 2);
                            mes = aux.Substring(2, 2);
                            ano = aux.Substring(4, 4);

                            return new DateTime(Int32.Parse(ano), Int32.Parse(mes), Int32.Parse(dia));
                        }
                        catch
                        {
                            return DateTime.MinValue;
                        }
                    }
                }
            }
        }

        string toTelefone(object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim().Length != 8 || Convert.ToString(param) == "00000000")
                return null;
            else
                return string.Concat("(00) ", Convert.ToString(param).Substring(0, 4), "-", Convert.ToString(param).Substring(4, 4));

        }
    }
}
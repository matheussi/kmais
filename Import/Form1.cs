using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

using LC.Framework.DataUtil;
//using LC.Framework.BusinessLayer;
using LC.Framework.Phantom;
using System.Collections;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;

namespace Import
{
    public partial class Form1 : Form
    {
        #region helpers 

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

        DateTime toData(object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim().Length != 10)
                return DateTime.MinValue;
            else
            {
                string aux = Convert.ToString(param);

                if (aux.IndexOf("/") > -1)
                {
                    string[] arr = aux.Split('/');
                    return new DateTime(Int32.Parse(arr[2]), Int32.Parse(arr[1]), Int32.Parse(arr[0]));
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

        #endregion

        public Form1()
        {
            InitializeComponent();
        }
        /*
         * REAUSTES 03/08/2014 - Dados enviados pela Tatiane
        */

        void executar_reajuste_taxa()
        {
            string tabela = " _taxa_reaj ";
            DataRow[] rows = null;
            DataTable dtAdicionais = null;
            DataTable dtTaxas = LocatorHelper.Instance.ExecuteQuery("select * from " + tabela + " order by mat_ss", "result").Tables[0];
            List<string> matass = new List<string>();

            foreach (DataRow row in dtTaxas.Rows)
            {
                if (matass.Contains(Convert.ToString(row["MAT_ASS"]))) continue;

                matass.Add(Convert.ToString(row["MAT_ASS"]));
            }

            object benefId = null;
            object adicionalId = null;

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                foreach (string mat in matass)
                {
                    //localiza o id do usuario
                    benefId = LocatorHelper.Instance.ExecuteScalar(string.Concat("select beneficiario_id,beneficiario_nome,beneficiario_matriculaAssociativa,beneficiario_matriculaFuncional from ", tabela, " where beneficiario_matriculaAssociativa='", mat, "'"), null, null, pm);

                    if (benefId == null)
                    {
                        //NonQueryHelper.Instance.ExecuteNonQuery(string.Concat("update ", tabela, " set processado=1,naoLocalizado=1 where MAT_ASS='", mat, "'"), pm);
                        NonQueryHelper.Instance.ExecuteNonQuery(string.Concat("update ", tabela, " set processado=1 where MAT_ASS='", mat, "'"), pm);
                        continue;
                    }

                    dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_codigo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id = adicionalbeneficiario_adicionalId where adicional_tipo=0 and adicionalbeneficiario_beneficiarioid=" + benefId, "result", pm).Tables[0];

                    if (dtAdicionais.Rows.Count == 0)
                    {
                        //NonQueryHelper.Instance.ExecuteNonQuery(string.Concat("update ", tabela, " set semAdicionais=1,naoLocalizado=1 where MAT_ASS='", mat, "'"), pm);
                        NonQueryHelper.Instance.ExecuteNonQuery(string.Concat("update ", tabela, " set semAdicionais=1 where MAT_ASS='", mat, "'"), pm);
                        continue;
                    }

                    rows = dtTaxas.Select(string.Concat("mat_ass='", mat, "'"));

                    foreach(DataRow row in rows)
                    {
                        adicionalId = NonQueryHelper.Instance.ExecuteNonQuery(string.Concat("select adicional_id from adicional_codigo='", row["PLANO"], "'"), pm);
                        if (adicionalId == null)
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery(string.Concat("update ", tabela, " set processado=1,adicionalNaoLocaliado=1 where id=", row["id"]), pm);
                            continue;
                        }

                        //atualiza os valores no banco de dados UBRASP CadBen
                        NonQueryHelper.Instance.ExecuteNonQuery(
                            string.Concat("update adicional_beneficiario set adicionalbeneficiario_valorAntigo=adicionalbeneficiario_valor where adicionalbeneficiario_beneficiarioId=", benefId, " and adicionalbeneficiario_adicionalid=", adicionalId),
                            pm);

                        NonQueryHelper.Instance.ExecuteNonQuery(
                            string.Concat("update adicional_beneficiario set adicionalbeneficiario_valor='", Convert.ToString(row["VALOR_01"]).Replace(".", "").Replace(",", "."), "' where adicionalbeneficiario_beneficiarioId=", benefId, " and adicionalbeneficiario_adicionalid=", adicionalId), 
                            pm);

                        //atualiza fonte para processado
                        NonQueryHelper.Instance.ExecuteNonQuery(string.Concat("update ", tabela, " set processado=1 where id=", row["id"]), pm);
                    }
                }
            }
        }

        /*********************************************************/

        private void cmdPessoas_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja iniciar a importação de pessoas e endereços?", "Importar pessoas", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return;

            lblPessoas.Text = "Carregando fonte de dados...";
            Application.DoEvents();

            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\importacao\3a_20160728\app15d1_pessoas.accdb;Persist Security Info=False");
            //string connAccess = string.Concat(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\APP15D1.mdb;Persist Security Info=False");
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
                object aux = null;
                foreach (DataRow row in dt.Rows)
                {
                    i++;
                    lblPessoas.Text = string.Concat("Registro ", i, " de ", dt.Rows.Count);
                    Application.DoEvents();

                    aux = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select beneficiario_id from beneficiario where beneficiario_matriculaAssociativa='", row["MAT_ASS"], "'"),
                        null, null, pm);

                    if (aux != null && aux != DBNull.Value) continue;

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
                    b.CodigoOrgaoAverbador = toString(row["ORGAO_AVE"]);

                    b.Legado = 1;

                    pm.Save(b);

                    en.Bairro = toString(row["BAIRRO"]);
                    en.CEP = toString(row["CEP"]).PadLeft(8, '0');
                    en.Cidade = toString(row["CIDADE"]);
                    en.Complemento = "";
                    if (toString(row["BLOCO"]) != "")
                    {
                        en.Complemento = string.Concat("Bloco ", row["BLOCO"]);
                    }
                    if (toString(row["COMPLEMEN"]) != "")
                    {
                        if (en.Complemento.Length > 0) { en.Complemento += " - "; }
                        en.Complemento += toString(row["COMPLEMEN"]);
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

        /// <summary>
        /// Não precisa mais.
        /// </summary>
        void arrumaArgaoAverbador()
        {
            lblPessoas.Text = "Carregando fonte de dados...";
            Application.DoEvents();

            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\APP15D1.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [dados]", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();
            DataTable benef = new DataTable();
            //benef = LocatorHelper.Instance.ExecuteQuery("select * from beneficiario where beneficiario_id >= 11 and beneficiario_codigoOrgao is null", "res", pm).Tables[0];
            benef = LocatorHelper.Instance.ExecuteQuery("select * from beneficiario where beneficiario_legado=1 and beneficiario_codigoOrgao is null", "res", pm).Tables[0];

            try
            {
                int i = 0;
                DataRow[] rows = null;
                foreach (DataRow b in benef.Rows)
                {
                    i++;
                    lblPessoas.Text = string.Concat("Registro ", i, " de ", dt.Rows.Count);
                    Application.DoEvents();

                    rows = dt.Select(string.Concat("mat_ass='", b["beneficiario_matriculaAssociativa"], "'"));

                    if (rows.Length == 0) continue;

                    NonQueryHelper.Instance.ExecuteNonQuery(
                        string.Concat("update beneficiario set beneficiario_codigoOrgao='", rows[0]["ORGAO_AVE"], "' where beneficiario_id=", b["beneficiario_id"]),
                        pm);
                }
            }
            catch
            {
            }
            finally
            {
                pm.Dispose();
                lblPessoas.Text = "Concluído.";
                Application.DoEvents();
            }
        }

        /*************************************************************************/
        /*************************************************************************/
        /// <summary>
        /// Para importar contratos adms
        /// </summary>
        private void cmdContratosADM_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja iniciar a importação de contratos administrativos?", "Importar contratos", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return;

            lblContratosADM.Text = "Carregando fonte de dados...";
            Application.DoEvents();

            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\APP03D1_entidades.accdb;Persist Security Info=False");
            DataTable aux = new DataTable();
            DataTable dt = new DataTable();

            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [dados] order by id", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(aux);
                DataView view = new DataView(aux);
                dt = view.ToTable(true, "COD_ORGAO", "SIGLA", "NOME");
                aux.Dispose();
            }

            List<string> codigos = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                if (codigos.Contains(Convert.ToString(row["COD_ORGAO"]))) continue;
                codigos.Add(Convert.ToString(row["COD_ORGAO"]));
            }

            lblContratosADM.Text = string.Concat("Total de registros: ", codigos.Count);
            Application.DoEvents();

            ContratoADM c = null;

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                int i = 0;
                DateTime data = DateTime.Now;
                DataRow[] rows = null;
                foreach (string codigo in codigos)
                {
                    i++;
                    lblContratosADM.Text = string.Concat("Registro ", i, " de ", codigos.Count);
                    Application.DoEvents();

                    rows = dt.Select("COD_ORGAO='" + codigo + "'");

                    c = new ContratoADM();

                    c.Ativo = true;
                    c.CodAdministradora = toString(rows[0]["COD_ORGAO"]);
                    c.CodFilial = toString(rows[0]["COD_ORGAO"]);
                    c.CodUnidade = toString(rows[0]["COD_ORGAO"]);
                    c.Data = data;
                    c.Descricao = toString(rows[0]["NOME"]);
                    c.EstipulanteID = 113; //UBRASP
                    c.Numero = toString(rows[0]["SIGLA"]);
                    c.OperadoraID = 4; //SALUTAR

                    pm.Save(c);
                }

                pm.Commit();
                lblContratosADM.Text = "Concluído.";
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

        /// <summary>
        /// Para importar contratos adms
        /// </summary>
        private void cmdContratosADM_v2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja iniciar a importação de contratos administrativos?", "Importar contratos", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return;

            lblContratosADM.Text = "Carregando fonte de dados...";
            Application.DoEvents();

            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\APP03D1_entidades_V2.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();

            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [dados] order by id", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            lblContratosADM.Text = string.Concat("Total de registros: ", dt.Rows.Count);
            Application.DoEvents();

            IList<ContratoADM> contratos = null;
            ContratoADM c = null;

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                int i = 0;
                DateTime data = DateTime.Now;
                foreach (DataRow row in dt.Rows )
                {
                    i++;
                    lblContratosADM.Text = string.Concat("Registro ", i, " de ", dt.Rows.Count);
                    Application.DoEvents();

                    contratos = LocatorHelper.Instance.ExecuteQuery<ContratoADM>("* from contratoadm where contratoadm_codUnidade='" + row["Codigo"] + "'", typeof(ContratoADM), pm);
                    if (contratos == null)
                    {
                        c = new ContratoADM();
                        c.Data = data;
                    }
                    else
                        c = contratos[0];

                    c.Ativo = true;
                    c.CodAdministradora = toString(row["Codigo"]);
                    c.CodFilial = toString(row["CodProdesp"]);
                    c.CodUnidade = toString(row["Codigo"]);
                    c.Descricao = toString(row["Orgao"]);
                    c.EstipulanteID = 113; //UBRASP
                    c.Numero = toString(row["Sigla"]);
                    c.OperadoraID = 4; //SALUTAR

                    pm.Save(c);
                }

                pm.Commit();
                lblContratosADM.Text = "Concluído.";
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

        /*************************************************************************/
        /*************************************************************************/

        private void cmdTaxas_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja iniciar a importação de taxas?", "Importar taxas", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return;

            lblTaxas.Text = "Carregando fonte de dados...";
            Application.DoEvents();

            string operadoraId = "4";

            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\importacao\3a_20160728\app15d2_taxas.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();

            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [dados] /*where mat_ass='8043671'*/ order by mat_ass,dt_inicio", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            List<string> matr = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                if (matr.Contains(Convert.ToString(row["mat_ass"]))) continue;

                matr.Add(Convert.ToString(row["mat_ass"]));
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext(); //pm.UseSingleCommandInstance();

            try
            {
                DataRow[] rows = null;
                object aux = null;
                Contrato contrato = null;
                ContratoBeneficiario cb = null;
                AdicionalBeneficiario ab = null;
                DateTime agora = DateTime.Now;
                IList<Plano> planos = null;
                object matriculaFuncional = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
                int i = 0;
                foreach (string matricula in matr)
                {
                    i++;
                    lblTaxas.Text = string.Concat("Registro ", i, " de ", dt.Rows.Count);
                    Application.DoEvents();

                    rows = dt.Select(string.Concat("mat_ass='", matricula, "'"), "DT_INICIO");

                    matriculaFuncional = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select beneficiario_matriculaFuncional from beneficiario where beneficiario_matriculaAssociativa='", rows[0]["mat_ass"], "'"), 
                        null, null, pm);

                    aux = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select contrato_id from contrato where contrato_numero='", matriculaFuncional, "'"), 
                        null, null, pm);

                    //aux = null; //ATENCAO: REMOVER ISSO SEM PENSAR!!!

                    if (aux != null && aux != DBNull.Value) continue;

                    
                    contrato = new Contrato();
                    contrato.Adimplente = true;
                    contrato.Admissao = toData(rows[0]["DT_INICIO"]);
                    contrato.Alteracao = contrato.Admissao;
                    contrato.Cancelado = false; //////////////
                    contrato.CobrarTaxaAssociativa = false;

                    aux = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select max(contrato_codcobranca) from contrato"), null, null, pm);

                    contrato.CodCobranca = Convert.ToInt32(aux) + 1;

                    aux = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select beneficiario_codigoOrgao from beneficiario where beneficiario_matriculaAssociativa='", rows[0]["mat_ass"], "'"),
                        null, null, pm);

                    contrato.ContratoADMID = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select contratoadm_id from contratoadm where contratoadm_codUnidade='", aux, "'"),
                        null, null, pm);

                    if (aux == null || aux == DBNull.Value)
                    {
                        throw new ApplicationException("Orgao não localizado: " + rows[0]["mat_ass"]);
                    }

                    contrato.CorretorTerceiroCPF = "";
                    contrato.CorretorTerceiroNome = "";
                    contrato.Data = agora;
                    //contrato.DataCancelamento ??
                    contrato.Desconto = 0;
                    contrato.DonoID = 2;
                    contrato.EmailCobranca = "";

                    aux = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select beneficiario_id from beneficiario where beneficiario_matriculaAssociativa='", rows[0]["mat_ass"], "'"),
                        null, null, pm);
                    contrato.EnderecoCobrancaID = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select endereco_id from endereco where endereco_donoid=", aux),
                        null, null, pm);
                    contrato.EnderecoReferenciaID = contrato.EnderecoCobrancaID;

                    contrato.EstipulanteID = 113;
                    contrato.FilialID = 3;
                    contrato.Inativo = contrato.Cancelado;

                    contrato.Numero = toString(matriculaFuncional);
                    contrato.NumeroID = null;
                    contrato.NumeroMatricula = contrato.Numero;
                    contrato.OperadoraID = operadoraId;
                    contrato.Pendente = false;

                    planos = LocatorHelper.Instance.ExecuteQuery<Plano>("select * from plano where plano_contratoid=" + contrato.ContratoADMID,typeof(Plano),pm);
                    if(planos == null  || planos.Count == 0)
                    {
                        planos = new List<Plano>();
                        planos.Add(new Plano());
                        planos[0].Ativo = true;
                        planos[0].QuartoComum = true;
                        planos[0].QuartoParticular = false;
                        planos[0].ContratoID = contrato.ContratoADMID;
                        planos[0].Descricao = "Padrão";
                        pm.Save(planos[0]);

                    }
                    contrato.PlanoID = planos[0].ID;
                    contrato.Rascunho = false;

                    contrato.ResponsavelCPF = ""; /////

                    contrato.Status = 0;
                    contrato.TipoAcomodacao = 0;
                    contrato.TipoContratoID = 1;
                    contrato.UsuarioID = 2;
                    contrato.ValorAto = 0;
                    contrato.Vencimento = agora; /////////
                    contrato.Vigencia = contrato.Admissao;
                    contrato.Legado = true;

                    pm.Save(contrato);

                    cb = new ContratoBeneficiario();
                    cb.Altura = 1.7M;
                    cb.Ativo = true;

                    aux = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select beneficiario_id from beneficiario where beneficiario_matriculaAssociativa='", rows[0]["mat_ass"], "'"),
                        null, null, pm);
                    cb.BeneficiarioID = aux;
                    cb.CarenciaCodigo = "";
                    cb.ContratoID = contrato.ID;
                    cb.Data = agora;

                    pm.Save(cb);

                    //adicionais
                    foreach (DataRow row in rows)
                    {
                        ab = new AdicionalBeneficiario();

                        aux = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select adicional_id from adicional where adicional_tipo=0 and adicional_codigo='", row["plano"], "'"),
                            null, null, pm);
                        ab.AdicionalID = aux;
                        ab.BeneficiarioID = cb.BeneficiarioID;
                        ab.FormaPagto = Convert.ToInt32(row["forma_pg"]);
                        ab.PropostaID = contrato.ID;
                        ab.Recorrente = true;
                        //ab.Sim = true;
                        ab.Status = toString(row["status"]);
                        ab.Status01 = toString(row["st_sp_01"]);
                        ab.Valor01 = Convert.ToDecimal(row["valor_01"], cinfo);

                        pm.Save(ab);
                    }
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                lblTaxas.Text = "Concluído.";
                Application.DoEvents();
            }
        }

        void cadastraAdicionais(string operadoraId)
        {
            lblTaxas.Text = "Carregando fonte de dados...";
            Application.DoEvents();

            Hashtable definidos = new Hashtable();
            definidos.Add("4437", "PLANO DE SAÚDE (4437)");
            definidos.Add("4439", "PLANO DE SAÚDE (4439)");
            definidos.Add("4100", "TAXA ASSOCIATIVA");
            definidos.Add("4435", "PLANO DE SAÚDE (4435)");
            definidos.Add("4438", "PLANO DE SAÚDE (4438)");
            definidos.Add("4436", "PLANO DE SAÚDE (4436)");
            definidos.Add("4441", "PLANO DE SAÚDE (4441)");
            definidos.Add("4442", "PLANO DE SAÚDE (4442)");
            definidos.Add("4440", "PLANO DE SAÚDE (4440)");
            definidos.Add("4424", "PREVIDENT (ODONTOLÓGICO 4424)");
            definidos.Add("4425", "PREVIDENT (ODONTOLÓGICO 4425)");
            definidos.Add("4400", "TAXA");

            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\APP15D2_taxa.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();

            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select DISTINCT(PLANO) from [dados]", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            try
            {

                object aux = null;
                Adicional ad = null;
                string texto = "", codigo = "";
                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    i++;
                    lblTaxas.Text = string.Concat("Registro ", i, " de ", dt.Rows.Count);
                    Application.DoEvents();

                    if (definidos.ContainsKey(Convert.ToString(row[0])))
                    {
                        texto = Convert.ToString(definidos[Convert.ToString(row[0])]);
                    }
                    else
                    {
                        texto = string.Concat("TAXA (", row[0], ")");
                    }

                    codigo = Convert.ToString(row[0]);

                    aux = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select adicional_id from adicional where adicional_tipo=0 and adicional_codigo='", codigo, "'"),
                        null, null, pm);

                    if (aux == null || aux == DBNull.Value)
                    {
                        ad = new Adicional();
                        ad.Ativo = true;
                        ad.Codigo = codigo;
                        ad.Descricao = texto;
                        ad.OperadoraID = operadoraId;
                        ad.ParaTodaProposta = false;
                        pm.Save(ad);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                pm.Dispose();
                lblTaxas.Text = "Concluído.";
                Application.DoEvents();
            }
        }
        void verificaTaxas()
        {
            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\APP15D2_taxa.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();

            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select distinct(PLANO) as Pla from [dados]", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            List<string> definidos = new List<string>();
            definidos.Add("4437");
            definidos.Add("4439");
            definidos.Add("4100");
            definidos.Add("4435");
            definidos.Add("4438");
            definidos.Add("4436");
            definidos.Add("4441");
            definidos.Add("4442");
            definidos.Add("4440");
            definidos.Add("4424");
            definidos.Add("4425");
            definidos.Add("4400");

            List<string> naoEncontrados = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                if (definidos.Contains(Convert.ToString(row[0]))) continue;

                if (!naoEncontrados.Contains(Convert.ToString(row[0])))
                {
                    naoEncontrados.Add(Convert.ToString(row[0]));
                    sb.Append(", "); sb.Append(row[0]);
                }
            }

            lblTaxas.Text = sb.ToString();
        }

        /*************************************************************************/
        /*************************************************************************/

        private void cmdSeguros_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja iniciar a importação de seguros?", "Importar seguros", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No) return;

            string operadoraId = "4";
            lblSeguros.Text = "Carregando fonte de dados...";
            Application.DoEvents();

            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\importacao\3a_20160728\app15d3_seguro.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();

            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [dados]", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            List<string> matr = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                if (matr.Contains(Convert.ToString(row["mat_ass"]))) continue;

                matr.Add(Convert.ToString(row["mat_ass"]));
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext(); //pm.UseSingleCommandInstance();

            try
            {
                Contrato contrato = null; ContratoBeneficiario cb = null;
                DataRow[] rows = null;
                object aux = null;
                IList<Plano> planos = null;
                AdicionalBeneficiario ab = null;
                DateTime agora = DateTime.Now;
                object contratoId = null, beneficiarioId = null;
                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
                int i = 0;
                foreach (string matricula in matr)
                {
                    i++;
                    lblSeguros.Text = string.Concat("Registro ", i, " de ", matr.Count);
                    Application.DoEvents();

                    rows = dt.Select(string.Concat("mat_ass='", matricula, "'"), "DT_INICIO");

                    beneficiarioId = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select beneficiario_id from beneficiario where beneficiario_matriculaAssociativa='", matricula, "'"),
                        null, null, pm);

                    if (beneficiarioId == null || beneficiarioId == DBNull.Value) continue;

                    contratoId = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select contratobeneficiario_contratoid from contrato_beneficiario where contratobeneficiario_beneficiarioid=", beneficiarioId),
                        null, null, pm);

                    if (contratoId == null || contratoId == DBNull.Value)
                    {
                        #region grava o contrato

                        contrato = new Contrato();
                        contrato.Adimplente = true;
                        contrato.Admissao = toData(rows[0]["DT_INICIO"]);
                        contrato.Alteracao = contrato.Admissao;
                        contrato.Cancelado = false; //////////////
                        contrato.CobrarTaxaAssociativa = false;


                        aux = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select max(contrato_codcobranca) from contrato"), null, null, pm);

                        contrato.CodCobranca = Convert.ToInt32(aux) + 1;

                        aux = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select beneficiario_codigoOrgao from beneficiario where beneficiario_matriculaAssociativa='", rows[0]["mat_ass"], "'"),
                            null, null, pm);

                        contrato.ContratoADMID = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select contratoadm_id from contratoadm where contratoadm_codUnidade='", aux, "'"),
                            null, null, pm);

                        if (aux == null || aux == DBNull.Value)
                        {
                            throw new ApplicationException("Orgao não localizado: " + rows[0]["mat_ass"]);
                        }

                        contrato.CorretorTerceiroCPF = "";
                        contrato.CorretorTerceiroNome = "";
                        contrato.Data = agora;
                        //contrato.DataCancelamento ??
                        contrato.Desconto = 0;
                        contrato.DonoID = 2;
                        contrato.EmailCobranca = "";

                        contrato.EnderecoCobrancaID = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select endereco_id from endereco where endereco_donoid=", beneficiarioId),
                            null, null, pm);
                        contrato.EnderecoReferenciaID = contrato.EnderecoCobrancaID;

                        contrato.EstipulanteID = 113;
                        contrato.FilialID = 3;
                        contrato.Inativo = contrato.Cancelado;

                        contrato.Numero = toString(LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select beneficiario_matriculaFuncional from beneficiario where beneficiario_id=", beneficiarioId),
                            null, null, pm));
                        contrato.NumeroID = null;
                        contrato.NumeroMatricula = contrato.Numero;
                        contrato.OperadoraID = operadoraId;
                        contrato.Pendente = false;

                        planos = LocatorHelper.Instance.ExecuteQuery<Plano>("select * from plano where plano_contratoid=" + contrato.ContratoADMID, typeof(Plano), pm);
                        if (planos == null || planos.Count == 0)
                        {
                            planos = new List<Plano>();
                            planos.Add(new Plano());
                            planos[0].Ativo = true;
                            planos[0].QuartoComum = true;
                            planos[0].QuartoParticular = false;
                            planos[0].ContratoID = contrato.ContratoADMID;
                            planos[0].Descricao = "Padrão";
                            pm.Save(planos[0]);

                        }
                        contrato.PlanoID = planos[0].ID;
                        contrato.Rascunho = false;

                        contrato.ResponsavelCPF = ""; /////

                        contrato.Status = 0;
                        contrato.TipoAcomodacao = 0;
                        contrato.TipoContratoID = 1;
                        contrato.UsuarioID = 2;
                        contrato.ValorAto = 0;
                        contrato.Vencimento = agora; /////////
                        contrato.Vigencia = contrato.Admissao;
                        contrato.Legado = true;

                        pm.Save(contrato);

                        contratoId = contrato.ID;

                        cb = new ContratoBeneficiario();
                        cb.Altura = 1.7M;
                        cb.Ativo = true;

                        cb.BeneficiarioID = beneficiarioId;
                        cb.CarenciaCodigo = "";
                        cb.ContratoID = contrato.ID;
                        cb.Data = agora;

                        pm.Save(cb);

                        #endregion
                    }

                    //adicionais
                    foreach (DataRow row in rows)
                    {
                        ab = new AdicionalBeneficiario();

                        aux = LocatorHelper.Instance.ExecuteScalar(
                            string.Concat("select adicional_id from adicional where adicional_tipo=1 and adicional_codigo='", row["plano"], "'"),
                            null, null, pm);
                        ab.AdicionalID = aux;
                        ab.BeneficiarioID = beneficiarioId;
                        ab.PropostaID = contratoId;
                        ab.FormaPagto = Convert.ToInt32(row["forma_pg"]);

                        ab.COD_COB_1 = Convert.ToInt32(row["COD_COB_1"]);
                        ab.PRE_COB_1 = Convert.ToDecimal(row["PRE_COB_1"], cinfo);
                        ab.ST_SG_1 = toString(row["ST_SG_01"]);
                        ab.DT_SG_1 = toData(row["DT_SG_01"]);

                        ab.COD_COB_2 = Convert.ToInt32(row["COD_COB_2"]);
                        ab.PRE_COB_2 = Convert.ToDecimal(row["PRE_COB_2"], cinfo);
                        ab.ST_SG_2 = toString(row["ST_SG_02"]);
                        ab.DT_SG_2 = toData(row["DT_SG_02"]);

                        ab.COD_COB_3 = Convert.ToInt32(row["COD_COB_3"]);
                        ab.PRE_COB_3 = Convert.ToDecimal(row["PRE_COB_3"], cinfo);
                        ab.ST_SG_3 = toString(row["ST_SG_03"]);
                        ab.DT_SG_3 = toData(row["DT_SG_03"]);

                        ab.COD_COB_4 = Convert.ToInt32(row["COD_COB_4"]);
                        ab.PRE_COB_4 = Convert.ToDecimal(row["PRE_COB_4"], cinfo);
                        ab.ST_SG_4 = toString(row["ST_SG_04"]);
                        ab.DT_SG_4 = toData(row["DT_SG_04"]);

                        ab.COD_COB_5 = Convert.ToInt32(row["COD_COB_5"]);
                        ab.PRE_COB_5 = Convert.ToDecimal(row["PRE_COB_5"], cinfo);
                        ab.ST_SG_5 = toString(row["ST_SG_05"]);
                        ab.DT_SG_5 = toData(row["DT_SG_05"]);

                        ab.COD_COB_6 = Convert.ToInt32(row["COD_COB_6"]);
                        ab.PRE_COB_6 = Convert.ToDecimal(row["PRE_COB_6"], cinfo);
                        ab.ST_SG_6 = toString(row["ST_SG_06"]);
                        ab.DT_SG_6 = toData(row["DT_SG_06"]);

                        pm.Save(ab);
                    }
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                lblSeguros.Text = "Concluído.";
                Application.DoEvents();
            }
        }

        void cadastraAdicionaisSeguro(string operadoraId)
        {
            lblSeguros.Text = "Carregando fonte de dados...";
            Application.DoEvents();

            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\APP15D3_seguro.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();

            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select DISTINCT(PLANO) from [dados]", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            try
            {

                object aux = null;
                Adicional ad = null;
                string texto = "", codigo = "";
                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    i++;
                    lblSeguros.Text = string.Concat("Registro ", i, " de ", dt.Rows.Count);
                    Application.DoEvents();

                    texto = string.Concat("Seguro (", row[0], ")");
                    codigo = Convert.ToString(row[0]);

                    aux = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select adicional_id from adicional where adicional_tipo=1 and adicional_codigo='", codigo, "'"),
                        null, null, pm);

                    if (aux == null || aux == DBNull.Value)
                    {
                        ad = new Adicional();
                        ad.Ativo = true;
                        ad.Codigo = codigo;
                        ad.Descricao = texto;
                        ad.OperadoraID = operadoraId;
                        ad.ParaTodaProposta = false;
                        ad.Tipo = 1;
                        pm.Save(ad);
                    }

                }
            }
            catch
            {
                throw;
            }
            finally
            {
                pm.Dispose();
                lblSeguros.Text = "Concluído.";
                Application.DoEvents();
            }
        }



        /**********************************************************************************************/

        void arrumaCPFs_a_partir_da_MatricaAssociativa()
        {
            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\ubrasp\importacao\scc_cpfs.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();

            string tabelaAcc = "descontados";
            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [" + tabelaAcc + "]", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);

                PersistenceManager pm = new PersistenceManager();
                pm.UseSingleCommandInstance();

                object aux = "";
                foreach (DataRow row in dt.Rows)
                {
                    aux = LocatorHelper.Instance.ExecuteScalar(
                        "select beneficiario_id from beneficiario where beneficiario_matriculaassociativa= '" + row["Funcional"] + "' or beneficiario_matriculaFuncional='" + row["Funcional"] + "'", null, null, pm);

                    if (aux == null || aux == DBNull.Value)
                    {
                        command.CommandText = "update [" + tabelaAcc + "] set encontrado='0' where id=" + row["id"];
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        NonQueryHelper.Instance.ExecuteNonQuery("update beneficiario set beneficiario_cpf='" + row["CPF"] + "' where beneficiario_id=" + aux, pm);

                        command.CommandText = "update [" + tabelaAcc + "] set encontrado='1' where id=" + row["id"];
                        command.ExecuteNonQuery();
                    }
                }

                pm.CloseSingleCommandInstance();
                pm.Dispose();
            }
        }

        /********************************************/


        private void Form1_Load(object sender, EventArgs e)
        {
            //aplicarReajusteMEDIAL_DESCONTADOS();
            //aplicarReajusteMEDIAL_NAO_DESCONTADOS();
            //aplicarReajusteMEDIAL_COBRADOS_POR_BOLETO();
            //aplicarReajusteMEDIAL_COBRADOS_POR_BOLETO_VerificaEnvioIndevido();
            //aplicarReajusteMEDIAL_DESCONTADOS_VerificaValoresErrados();
        }

        decimal toDecimal(object param, System.Globalization.CultureInfo cinfo, object param2)
        {
            if (param2 == null || param2 == DBNull.Value) return 0;

            if (Convert.ToString(param2).ToUpper() != "A") return 0;

            if (param == null || param == DBNull.Value)
                return 0;
            else
                return Convert.ToDecimal(param, cinfo);
        }

        void gerarArquivoSCC()
        {
            decimal valor1 = 0, valor2 = 0;
            List<string> averbSemErro = new List<string>();
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                string where = " where cpf in ('15546985887') "; // where = " cpf in ('15546985887','9255966804', '3521540804', '20735359849') ";

                DataTable dtCPFS = LocatorHelper.Instance.ExecuteQuery("select * from ___cpfs " + where + " order by erro,valor_tratadodec,Funcional", "result", pm).Tables[0];

                DataTable dtPES = new DataTable();
                DataTable dtTAXAS = new DataTable();
                string cpfCorrete = "", nome = "", associativa = "", dtinicio = ""; Boolean primeiro = true;
                foreach (DataRow rcpf in dtCPFS.Rows)
                {
                    //valor1 = 0; valor2 = 0;

                    if (Convert.ToString(rcpf["erro"]) == "0")
                    {
                        if (!averbSemErro.Contains(Convert.ToString(rcpf["Nuavebev"])))
                        {
                            averbSemErro.Add(Convert.ToString(rcpf["Nuavebev"]));
                        }
                    }
                    else if (averbSemErro.Contains(Convert.ToString(rcpf["Nuavebev"]))) continue; //nao processa o erro se o que deu certo ja foi processado

                    if (cpfCorrete == "")
                    {
                        primeiro = true;
                        cpfCorrete = Convert.ToString(rcpf["cpf"]);
                    }
                    else if (cpfCorrete != "" && cpfCorrete != Convert.ToString(rcpf["cpf"]))
                    {
                        primeiro = true;
                        cpfCorrete = Convert.ToString(rcpf["cpf"]);
                    }
                    else
                    {
                        primeiro = false;
                    }

                    dtPES.Rows.Clear();
                    dtPES.Dispose();
                    dtPES = LocatorHelper.Instance.ExecuteQuery(
                        "select * from ___pessoas where CPF like '%" + rcpf["CPF"] + "'", "result", pm).Tables[0];
                        //"select * from ___pessoas where MAT_FUNC='" + rcpf["Funcional"] + "'", "result", pm).Tables[0];

                    if (dtPES.Rows.Count == 0)
                    {
                        //tenta por funcional 
                        dtPES = LocatorHelper.Instance.ExecuteQuery(
                            "select * from ___pessoas where MAT_FUNC='" + rcpf["Funcional"] + "'", "result", pm).Tables[0];

                        if (dtPES.Rows.Count == 0)
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update ___cpfs set encontrado=0 where id=" + rcpf["id"], pm);
                            continue;
                        }
                        //else
                        //{
                        //    foreach (DataRow rPES in dtPES.Rows)
                        //    {
                        //        NonQueryHelper.Instance.ExecuteNonQuery("update ___pessoas set cpf='" + rcpf["CPF"] + "' where MAT_FUNC=" + rcpf["Funcional"], pm);
                        //    }
                        //}
                    }

                    NonQueryHelper.Instance.ExecuteNonQuery("update ___cpfs set encontrado=1 where id=" + rcpf["id"], pm);

                    if (primeiro)
                    {
                        dtTAXAS.Rows.Clear();
                        dtTAXAS.Dispose();
                        valor1 = 0;
                        foreach (DataRow rPES in dtPES.Rows)
                        {
                            nome = Convert.ToString(rPES["nome"]);
                            associativa = Convert.ToString(rPES["MAT_ASS"]);

                            //precisa agora localizar as taxas >= 4435
                            dtTAXAS = LocatorHelper.Instance.ExecuteQuery(
                                "select * from ___taxas where STATUS='A' and MAT_ASS='" + rPES["MAT_ASS"] + "'", //plano >= 4435 AND 
                                "result", pm).Tables[0];

                            if (dtTAXAS.Rows.Count > 0)
                            {
                                foreach (DataRow rTAXA in dtTAXAS.Rows)
                                {
                                    dtinicio = Convert.ToDateTime(rTAXA["dt_inicio"], cinfo).ToString("yyyy-MM-dd");

                                    valor1 += toDecimal(rTAXA["VALOR_01"], cinfo, rTAXA["ST_SP_01"]);
                                    valor1 += toDecimal(rTAXA["VALOR_02"], cinfo, rTAXA["ST_SP_02"]);
                                    valor1 += toDecimal(rTAXA["VALOR_03"], cinfo, rTAXA["ST_SP_03"]);
                                    valor1 += toDecimal(rTAXA["VALOR_04"], cinfo, rTAXA["ST_SP_04"]);
                                }

                                valor1 += (valor1 * 0.1849M);

                                //atualiza valores
                                NonQueryHelper.Instance.ExecuteNonQuery(string.Concat(
                                    "update ___cpfs set nome='", nome.Replace("'", ""), "', associativa='", associativa, "', cnpjOrgao='04198514000154',",
                                    "valorNovo='", valor1.ToString("N2", cinfo).Replace(".", "").Replace(",", "."), "', inicio='", dtinicio, "' ",
                                    "where id=", rcpf["id"]), pm);
                            }
                        }
                    }
                    else
                    {
                        dtTAXAS.Rows.Clear();
                        dtTAXAS.Dispose();
                        valor2 = 0;

                        foreach (DataRow rPES in dtPES.Rows)
                        {
                            nome = Convert.ToString(rPES["nome"]);
                            associativa = Convert.ToString(rPES["MAT_ASS"]);

                            //precisa agora localizar as seguros
                            dtTAXAS = LocatorHelper.Instance.ExecuteQuery(
                                "select * from ___seguros where STATUS='A' and MAT_ASS='" + rPES["MAT_ASS"] + "'", //plano >= 4435 AND 
                                "result", pm).Tables[0];

                            if (dtTAXAS.Rows.Count > 0)
                            {
                                foreach (DataRow rTAXA in dtTAXAS.Rows)
                                {
                                    dtinicio = Convert.ToDateTime(rTAXA["dt_inicio"], cinfo).ToString("yyyy-MM-dd");

                                    valor2 += toDecimal(rTAXA["PRE_COB_1"], cinfo, rTAXA["ST_SG_01"]);
                                    valor2 += toDecimal(rTAXA["PRE_COB_2"], cinfo, rTAXA["ST_SG_02"]);
                                    valor2 += toDecimal(rTAXA["PRE_COB_3"], cinfo, rTAXA["ST_SG_03"]);
                                    valor2 += toDecimal(rTAXA["PRE_COB_4"], cinfo, rTAXA["ST_SG_03"]);
                                    valor2 += toDecimal(rTAXA["PRE_COB_5"], cinfo, rTAXA["ST_SG_05"]);
                                    valor2 += toDecimal(rTAXA["PRE_COB_6"], cinfo, rTAXA["ST_SG_06"]);
                                }

                                valor2 += (valor2 * 0.0591M);

                                //atualiza valores
                                NonQueryHelper.Instance.ExecuteNonQuery(string.Concat(
                                    "update ___cpfs set nome='", nome.Replace("'",""), "', associativa='", associativa, "', cnpjOrgao='04198514000154',",
                                    "valorNovo='", valor2.ToString("N2", cinfo).Replace(".", "").Replace(",", "."), "', inicio='", dtinicio, "' ",
                                    "where id=", rcpf["id"]), pm);
                            }
                        }
                    }
                }
            }
        }

        void gerarArquivoFisicoSCC()
        {
            ReajusteCabecalho cab = new ReajusteCabecalho();
            List<ReajusteItem> itens = new List<ReajusteItem>();

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ___cpfs where cpf='15546985887' and encontrado=1", "result", pm).Tables[0];

                try
                {
                    #region salva cabeçalho

                    cab.AdicionalId = CToInt(1);
                    cab.Cobertura = CToDecimal(1);
                    cab.ContratoADMId = CToInt(25);
                    cab.Descricao = "Teste";
                    cab.FormaPagto = "";
                    cab.Premio = 0;
                    cab.Tipo = 1;
                    cab.TipoArquivo = 1;
                    cab.UsuarioID = 1;
                    //cad.NomeArquivo     = "";
                    pm.Save(cab);
                    #endregion

                    System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                    pm.Commit();

                    #region escreve arquivo

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    string CNPJBASEIF = "49938327"; //??
                    string DATAREF = cab.Data.ToString("yyyyMMddHHmm"); //?? formato
                    String SEQIF = Convert.ToInt32(cab.ID).ToString().PadLeft(7, '0'); //??
                    String BaseCNPJConsignatario = "49938327"; // 
                    String BaseCNPJCIP = "02992335";

                    string nomeArquivo = string.Concat(
                        Convert.ToInt32(cab.ID).ToString().PadLeft(5, '0'),
                        "_", CNPJBASEIF, "_", DATAREF, "_", SEQIF);

                    sb.Append("0");                                 //IdentcLinha
                    sb.Append(nomeArquivo.PadRight(35, ' '));       //NCNPJBaseEmissor. 
                    sb.Append(BaseCNPJConsignatario);               //CNPJBaseEmissor. 
                    sb.Append(BaseCNPJCIP);                         //CNPJBaseDestinatario
                    sb.Append(Convert.ToInt32(cab.ID).ToString().PadLeft(20, '0')); //NumCtrlReq
                    sb.Append(cab.Data.ToString("yyyyMMdd"));       //DtRef
                    sb.Append(cab.Data.ToString("yyyyMMddHHmm"));   //DtHrArq
                    sb.Append(" ".PadRight(030, ' '));              //Brancos

                    int i = 0;
                    string cpf = "", matricula = "", valor = "0";
                    foreach (DataRow row in dados.Rows)
                    {
                        sb.Append(Environment.NewLine);
                        cpf = CToString(row["cpf"]).Replace(".", "").Replace("-", "");
                        matricula = CToString(row["Nuavebev"]).Replace(".", "").Replace("-", "");
                        valor = CToDecimal(row["valorNovo"], cinfo).ToString("N2").Replace(".", "").Replace(",", "").PadLeft(15, '0');

                        sb.Append("1");                                 //IdentcLinha
                        sb.Append(BaseCNPJConsignatario);               //?? IdentdPartAdmdo
                        sb.Append(Convert.ToInt32(cab.ID).ToString().PadLeft(20, '0')); //NumCtrlConsigrio
                        sb.Append(CToString(row["cnpjOrgao"]).Substring(0, 8));         //CNPJBaseEnte
                        sb.Append("097480");                            //?? NumConsigrioEnte
                        sb.Append("  ");                                //NumDigtConsigrioEnte - não obrigatório
                        sb.Append(cpf.PadLeft(11, '0'));                //NumCPFServdr
                        sb.Append(matricula.PadRight(21, ' '));         //NUAvebcSCC
                        sb.Append(valor);                               //VlrAltd
                        sb.Append("   ");                               //?? QtdTotParcl - estou enviado interminado, tem problema?
                        sb.Append("001");                               // IdentcMotvAlt
                        sb.Append("    ");                              //SitProc
                        sb.Append(" ".PadLeft(20, ' '));                //NumCtrlCIP

                        i++;
                    }

                    sb.Append(Environment.NewLine);
                    sb.Append("9");                                                     //IdentcLinha
                    sb.Append((dados.Rows.Count + 2).ToString().PadLeft(9, '0'));       //QtdLinhas
                    sb.Append(" ".PadRight(112, ' '));                                  //Filler 

                    string text = sb.ToString();

                    //File.WriteAllText(@"C:\Users\ACER E1 572 6830\Documents\fotos", sb.ToString(), System.Text.Encoding.GetEncoding("iso-8859-1"));

                    #endregion
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
        }

        /// <summary>
        /// Aplica os reajustes, faz a diferença do que foi descontado, gera o boleto
        /// </summary>
        void aplicarReajusteMEDIAL_DESCONTADOS()
        {
            decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M;
            string tabela = "___2aEmissaoDescontados";

            Cobranca cob = null;
            DataTable dtAdicionais = new DataTable();
            DataTable dtDescontado = null;
            object aux = null;
            AdicionalBeneficiario ab = null;
            decimal valorTotalReajustado = 0, descontado = 0, valorSemReajuste = 0;
            DateTime vigencia = new DateTime(1950, 01, 01);
            Beneficiario beneficiario = null;
            string funcional = ""; bool temMedial = false;

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    DataTable dt1 = LocatorHelper.Instance.ExecuteQuery("select distinct(funcional) from " + tabela, "result", pm).Tables[0]; // + " where funcional='1097878'"   where funcional='1002193658100'
                    List<Result> resultado = new List<Result>();

                    int total = dt1.Rows.Count;
                    int i = 0;

                    bool salto = false;
                    foreach (DataRow row in dt1.Rows)
                    {
                        i++;
                        salto = false;
                        funcional = Convert.ToString(row["Funcional"]);
                        valorTotalReajustado = 0;
                        aux = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculafuncional like '%" + funcional + "'", null, null, pm);

                        if (aux == null)
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update " + tabela + " set naoLocalizado=1 where Funcional='" + funcional + "'", pm);
                            continue;
                        }

                        beneficiario = new Beneficiario(aux);
                        pm.Load(beneficiario);

                        temMedial = false;

                        dtAdicionais.Rows.Clear();
                        dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid where adicionalbeneficiario_beneficiarioid=" + aux, "result", pm).Tables[0];

                        valorSemReajuste = 0;

                        foreach (DataRow rowAd in dtAdicionais.Rows)
                        {
                            ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                            if (ab == null) continue;
                            if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                            valorSemReajuste += ab.RetornaValor(-1, vigencia);

                            //ab.AplicaRejuste(reajustePlano, reajusteTaxa, reajusteSeguro);
                            //ab.Atualizado = true;

                            valorTotalReajustado += ab.RetornaValor(-1, vigencia);

                            if (Convert.ToInt32(ab.AdicionalCodigo) >= 4435 && Convert.ToInt32(ab.AdicionalCodigo) <= 4439) temMedial = true;

                            //pm.Save(ab);
                        }

                        //verifica se tem plano
                        if (!temMedial) // não gera boleto para quem não é medial
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update " + tabela + " set semPlano=1 where Funcional='" + funcional + "'", pm);
                            salto = true;
                        }

                        //verifica se tem previdencia. se tiver, nao deve enviar boleto. 
                        aux = LocatorHelper.Instance.ExecuteScalar("select id from ___previdencia where dados like '%" + beneficiario.MatriculaFuncional + "%'", null, null, pm);
                        if (aux != null && aux != DBNull.Value)
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update " + tabela + " set previdencia=1 where Funcional='" + funcional + "'", pm);
                            salto = true;
                        }

                        if (salto) continue;

                        //Calcula a diferenca
                        dtDescontado = LocatorHelper.Instance.ExecuteQuery(string.Concat("select ValorTratado from " + tabela + " where funcional like '%", funcional, "'"), "desc", pm).Tables[0];
                        descontado = 0;
                        foreach (DataRow descRow in dtDescontado.Rows)
                        {
                            descontado += Convert.ToDecimal(descRow[0]);
                        }

                        Result r = new Result();
                        r.Associativa = beneficiario.MatriculaAssociativa;
                        r.Funcional = beneficiario.MatriculaFuncional;
                        r.Nome = beneficiario.Nome;
                        r.ValorReajustado = valorTotalReajustado.ToString("N2");
                        r.ValorSemReajuste = valorSemReajuste.ToString("N2");
                        r.ValorDoArquivoEnviado = descontado.ToString("N2");
                        r.ValorACobrar = (valorTotalReajustado - descontado).ToString("N2");
                        r.Tipo = "DESCONTADO (DIFERENCA)";
                        resultado.Add(r);

                        //obtem id do contrato
                        aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 where contratobeneficiario_beneficiarioId=" + beneficiario.ID, null, null, pm);

                        cob = new Cobranca();
                        cob.Tipo = (int)Cobranca.eTipo.DiferencaUbraSP;
                        cob.Parcela = 0;
                        //cob.DataVencimento = Convert.ToDateTime(row["vencimento"]);
                        //cob.DataVencimento = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, cob.DataVencimento.Day, 23, 59, 59, 995);
                        cob.DataVencimento = new DateTime(2016, 09, 19, 23, 59, 59, 995);
                        cob.Valor = (valorTotalReajustado - descontado);
                        cob.CobrancaRefID = null;
                        cob.DataPgto = DateTime.MinValue;
                        cob.ValorPgto = Decimal.Zero;
                        cob.Pago = false;
                        cob.PropostaID = aux;
                        cob.Cancelada = false;
                        cob.ArquivoIDUltimoEnvio = -3;

                        if (cob.Valor > 3)
                        {
                            pm.Save(cob);
                            NonQueryHelper.Instance.ExecuteNonQuery("update ___result set result_boletoGerado=1 where result_matriculaAsso='" + r.Associativa + "'", pm);
                        }
                    }

                    foreach (var r in resultado)
                    {
                        //pm.Save(r);
                    }

                    pm.Commit();
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
        }

        void aplicarReajusteMEDIAL_NAO_DESCONTADOS()
        {
            decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M;
            string tabela = "___2aEmissaoNaoDescontados";

            Cobranca cob = null;
            DataTable dtAdicionais = new DataTable();
            DataTable dtDescontado = null;
            object aux = null;
            AdicionalBeneficiario ab = null;
            decimal valorTotalReajustado = 0, cobradoNoArquivo = 0, valorSemReajuste = 0;
            DateTime vigencia = new DateTime(1850, 01, 01);
            Beneficiario beneficiario = null;
            string funcional = ""; bool temMedial = false;

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    DataTable dt1 = LocatorHelper.Instance.ExecuteQuery("select distinct(funcional) from " + tabela, "result", pm).Tables[0]; //where funcional='1002193658100'
                    List<Result> resultado = new List<Result>();

                    int total = dt1.Rows.Count;
                    int i = 0;

                    bool salto = false;
                    foreach (DataRow row in dt1.Rows)
                    {
                        i++;
                        salto = false;
                        funcional = Convert.ToString(row["Funcional"]);
                        valorTotalReajustado = 0;
                        aux = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculafuncional like '%" + funcional + "'", null, null, pm);

                        if (aux == null)
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update " + tabela + " set naoLocalizado=1 where Funcional='" + funcional + "'", pm);
                            continue;
                        }

                        beneficiario = new Beneficiario(aux);
                        pm.Load(beneficiario);

                        temMedial = false;

                        dtAdicionais.Rows.Clear();
                        dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid where adicionalbeneficiario_beneficiarioid=" + aux, "result", pm).Tables[0];

                        valorSemReajuste = 0;

                        foreach (DataRow rowAd in dtAdicionais.Rows)
                        {
                            ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                            if (ab == null) continue;
                            if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                            valorSemReajuste += ab.RetornaValor(-1, vigencia);

                            //ab.AplicaRejuste(reajustePlano, reajusteTaxa, reajusteSeguro);
                            //ab.Atualizado = true;

                            valorTotalReajustado += ab.RetornaValor(-1, vigencia);

                            if (Convert.ToInt32(ab.AdicionalCodigo) >= 4435 && Convert.ToInt32(ab.AdicionalCodigo) <= 4439) temMedial = true;

                            //pm.Save(ab);
                        }

                        //verifica se tem previdencia. se tiver, nao deve enviar boleto. 
                        aux = LocatorHelper.Instance.ExecuteScalar("select id from ___previdencia where dados like '%" + beneficiario.MatriculaFuncional + "%'", null, null, pm);
                        if (aux != null && aux != DBNull.Value)
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update " + tabela + " set previdencia=1 where Funcional='" + funcional + "'", pm);
                            salto = true;
                        }

                        //verifica se tem plano
                        if (!temMedial) // não gera boleto para quem não é medial
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update " + tabela + " set semPlano=1 where Funcional='" + funcional + "'", pm);
                            salto = true;
                        }

                        if (salto) continue;

                        //Calcula a diferenca
                        dtDescontado = LocatorHelper.Instance.ExecuteQuery(string.Concat("select ValorTratado from " + tabela + " where funcional like '%", funcional, "'"), "desc", pm).Tables[0];
                        cobradoNoArquivo = 0;
                        foreach (DataRow descRow in dtDescontado.Rows)
                        {
                            cobradoNoArquivo += Convert.ToDecimal(descRow[0]);
                        }

                        Result r = new Result();
                        r.Associativa = beneficiario.MatriculaAssociativa;
                        r.Funcional = beneficiario.MatriculaFuncional;
                        r.Nome = beneficiario.Nome;
                        r.ValorReajustado = valorTotalReajustado.ToString("N2");
                        r.ValorSemReajuste = valorSemReajuste.ToString("N2");
                        r.ValorDoArquivoEnviado = cobradoNoArquivo.ToString("N2");
                        r.ValorACobrar = valorTotalReajustado.ToString("N2");
                        r.Tipo = "NAO DESCONTADO";
                        resultado.Add(r);

                        //obtem id do contrato
                        aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 where contratobeneficiario_beneficiarioId=" + beneficiario.ID, null, null, pm);

                        cob = new Cobranca();
                        cob.Tipo = (int)Cobranca.eTipo.DiferencaUbraSP;
                        cob.Parcela = 0;
                        //cob.DataVencimento = Convert.ToDateTime(row["vencimento"]);
                        //cob.DataVencimento = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, cob.DataVencimento.Day, 23, 59, 59, 995);
                        cob.DataVencimento = new DateTime(2016, 09, 19, 23, 59, 59, 995);
                        cob.Valor = valorTotalReajustado;
                        cob.CobrancaRefID = null;
                        cob.DataPgto = DateTime.MinValue;
                        cob.ValorPgto = Decimal.Zero;
                        cob.Pago = false;
                        cob.PropostaID = aux;
                        cob.Cancelada = false;
                        cob.ArquivoIDUltimoEnvio = -2;

                        if (cob.Valor > 3)
                        {
                            //pm.Save(cob);
                            NonQueryHelper.Instance.ExecuteNonQuery("update ___result set result_boletoGerado=1 where result_matriculaAsso='" + r.Associativa + "'", pm);
                        }
                    }

                    foreach (var r in resultado)
                    {
                        //pm.Save(r);
                    }

                    pm.Commit();
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
        }

        void aplicarReajusteMEDIAL_COBRADOS_POR_BOLETO()
        {
            decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M;

            Cobranca cob = null;
            DataTable dtAdicionais = new DataTable();
            object aux = null;
            AdicionalBeneficiario ab = null;
            decimal valorTotalReajustado = 0, cobradoNoArquivo = 0, valorSemReajuste = 0;
            DateTime vigencia = new DateTime(1850, 01, 01);
            Beneficiario beneficiario = null;
            bool temMedial = false;

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    DataTable dt1 = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.*,beneficiario_matriculaFuncional,beneficiario_matriculaAssociativa  from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid  inner join beneficiario on beneficiario_id = adicionalbeneficiario_beneficiarioid  inner join contrato on contrato_id = adicionalbeneficiario_propostaId where adicionalbeneficiario_formaPagto=31 and adicionalbeneficiario_recorrente=1 and contrato_inativo=0 and contrato_cancelado=0 order by adicionalbeneficiario_beneficiarioid,adicionalbeneficiario_propostaId", "result", pm).Tables[0]; //and contrato_id=35598 
                    List<Result> resultado = new List<Result>();
                    List<string> funcionais = new List<string>();

                    foreach (DataRow row in dt1.Rows)
                    {
                        if (funcionais.Contains(Convert.ToString(row["beneficiario_matriculaFuncional"]))) continue;

                        funcionais.Add(Convert.ToString(row["beneficiario_matriculaFuncional"]));
                    }

                    int total = dt1.Rows.Count;
                    int i = 0;

                    foreach(string funcional in funcionais)//foreach (DataRow row in dt1.Rows)
                    {
                        i++;
                        valorTotalReajustado = 0;
                        aux = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculafuncional = '" + funcional + "'", null, null, pm);

                        beneficiario = new Beneficiario(aux);
                        pm.Load(beneficiario);

                        dtAdicionais.Rows.Clear();
                        dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid where adicionalbeneficiario_formaPagto=31 and adicionalbeneficiario_recorrente=1 and adicionalbeneficiario_beneficiarioid=" + aux, "result", pm).Tables[0];

                        valorSemReajuste = 0;

                        temMedial = false;

                        foreach (DataRow rowAd in dtAdicionais.Rows)
                        {
                            ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                            if (ab == null) continue;
                            if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                            valorSemReajuste += ab.RetornaValor(AdicionalBeneficiario._FormaPagtoBoleto, vigencia);

                            //if (!ab.Atualizado)
                            //{
                            //    ab.AplicaRejuste(reajustePlano, reajusteTaxa, reajusteSeguro);
                            //    ab.Atualizado = true;
                            //}

                            valorTotalReajustado += ab.RetornaValor(-1, vigencia);

                            if (Convert.ToInt32(ab.AdicionalCodigo) >= 4435 && Convert.ToInt32(ab.AdicionalCodigo) <= 4439) temMedial = true;

                            //pm.Save(ab);
                        }

                        //verifica se tem plano
                        if (!temMedial) // não gera boleto para quem não é medial
                        {
                            continue;
                        }

                        Result r = new Result();
                        r.Associativa = beneficiario.MatriculaAssociativa;
                        r.Funcional = beneficiario.MatriculaFuncional;
                        r.Nome = beneficiario.Nome;
                        r.ValorReajustado = valorTotalReajustado.ToString("N2");
                        r.ValorSemReajuste = valorSemReajuste.ToString("N2");
                        r.ValorDoArquivoEnviado = cobradoNoArquivo.ToString("N2");
                        r.ValorACobrar = valorTotalReajustado.ToString("N2");
                        r.Tipo = "BOLETOS";
                        resultado.Add(r);

                        //obtem id do contrato
                        aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 where contratobeneficiario_beneficiarioId=" + beneficiario.ID, null, null, pm);

                        cob = new Cobranca();
                        cob.Tipo = (int)Cobranca.eTipo.BoletoUbraspSP;
                        cob.Parcela = 0;
                        //cob.DataVencimento = Convert.ToDateTime(row["vencimento"]);
                        //cob.DataVencimento = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, cob.DataVencimento.Day, 23, 59, 59, 995);
                        cob.DataVencimento = new DateTime(2016, 09, 19, 23, 59, 59, 995);
                        cob.Valor = valorTotalReajustado;
                        cob.CobrancaRefID = null;
                        cob.DataPgto = DateTime.MinValue;
                        cob.ValorPgto = Decimal.Zero;
                        cob.Pago = false;
                        cob.PropostaID = aux;
                        cob.Cancelada = false;
                        cob.ArquivoIDUltimoEnvio = -4;

                        if (cob.Valor > 3)
                        {
                            //pm.Save(cob);
                            NonQueryHelper.Instance.ExecuteNonQuery("update ___result set result_boletoGerado=1 where result_matriculaAsso='" + r.Associativa + "'", pm);
                        }
                    }

                    foreach (var r in resultado)
                    {
                        //pm.Save(r);
                    }

                    pm.Commit();
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
        }

        void aplicarReajusteMEDIAL_COBRADOS_POR_BOLETO_VerificaEnvioIndevido()
        {
            decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M;

            Cobranca cob = null;
            DataTable dtAdicionais = new DataTable();
            object aux = null;
            AdicionalBeneficiario ab = null;
            decimal valorTotalReajustado = 0, cobradoNoArquivo = 0, valorSemReajuste = 0;
            DateTime vigencia = new DateTime(1850, 01, 01);
            Beneficiario beneficiario = null;
            bool temMedial = false;
            List<string> beneficiarioIds = new List<string>();

            using (PersistenceManager pm = new PersistenceManager())
            {
                //pm.BeginTransactionContext();
                pm.UseSingleCommandInstance();

                try
                {
                    DataTable dt1 = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.*,beneficiario_matriculaFuncional,beneficiario_matriculaAssociativa  from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid  inner join beneficiario on beneficiario_id = adicionalbeneficiario_beneficiarioid  inner join contrato on contrato_id = adicionalbeneficiario_propostaId where adicionalbeneficiario_formaPagto=31 and adicionalbeneficiario_recorrente=1 and contrato_inativo=0 and contrato_cancelado=0 order by adicionalbeneficiario_beneficiarioid,adicionalbeneficiario_propostaId ", "result", pm).Tables[0]; //and contrato_id=35598 //  and adicionalbeneficiario_beneficiarioid=39537
                    List<Result> resultado = new List<Result>();
                    List<string> funcionais = new List<string>();

                    foreach (DataRow row in dt1.Rows)
                    {
                        if (funcionais.Contains(Convert.ToString(row["beneficiario_matriculaFuncional"]))) continue;

                        funcionais.Add(Convert.ToString(row["beneficiario_matriculaFuncional"]));
                    }

                    int total = dt1.Rows.Count;
                    int i = 0;

                    foreach (string funcional in funcionais)//foreach (DataRow row in dt1.Rows)
                    {
                        i++;
                        valorTotalReajustado = 0;
                        aux = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculafuncional = '" + funcional + "'", null, null, pm);

                        beneficiario = new Beneficiario(aux);
                        pm.Load(beneficiario);

                        dtAdicionais.Rows.Clear();
                        dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid where adicionalbeneficiario_formaPagto=31 and adicionalbeneficiario_recorrente=1 and adicionalbeneficiario_beneficiarioid=" + aux, "result", pm).Tables[0];

                        valorSemReajuste = 0;

                        temMedial = false;

                        foreach (DataRow rowAd in dtAdicionais.Rows)
                        {
                            ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                            if (ab == null) continue;
                            if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                            valorSemReajuste += ab.RetornaValor(AdicionalBeneficiario._FormaPagtoBoleto, vigencia);


                            valorTotalReajustado += ab.RetornaValor(-1, vigencia);

                            if (Convert.ToInt32(ab.AdicionalCodigo) >= 4435 && Convert.ToInt32(ab.AdicionalCodigo) <= 4439) temMedial = true;

                            //pm.Save(ab);
                        }

                        //verifica se tem plano
                        if (!temMedial) // não gera boleto para quem não é medial
                        {
                            continue;
                        }
                        else
                        {
                            foreach (DataRow rowAd in dtAdicionais.Rows)
                            {
                                ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);

                                if (Convert.ToInt32(ab.AdicionalCodigo) >= 4435 && Convert.ToInt32(ab.AdicionalCodigo) <= 4439)
                                {
                                    if (ab.Status.ToUpper() != "A" && ab.Status01.ToUpper() == "A")
                                    {
                                        if (beneficiarioIds.Contains(Convert.ToString(aux)) == false)
                                        {
                                            beneficiarioIds.Add(Convert.ToString(aux));
                                        }
                                    }
                                    break;
                                }
                            }

                            continue;
                        }
                    }

                    //foreach (var r in resultado)
                    //{
                    //    //pm.Save(r);
                    //}

                    //pm.Commit();
                    pm.CloseSingleCommandInstance();
                    string inids = string.Join(",", beneficiarioIds.ToArray());
                    string breakp = "";

                }
                catch
                {
                    //pm.Rollback();
                    throw;
                }
                finally
                {
                    pm.Dispose();
                }
            }
        }

        void aplicarReajusteMEDIAL_DESCONTADOS_VerificaValoresErrados()
        {
            decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M;
            string tabela = "___2aEmissaoDescontados";

            Cobranca cob = null;
            DataTable dtAdicionais = new DataTable();
            DataTable dtDescontado = null;
            object aux = null;
            AdicionalBeneficiario ab = null;
            decimal valorTotalReajustado = 0, descontado = 0, valorSemReajuste = 0;
            DateTime vigencia = new DateTime(1950, 01, 01);
            Beneficiario beneficiario = null;
            string funcional = ""; bool temMedial = false;

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    DataTable dt1 = LocatorHelper.Instance.ExecuteQuery("select distinct(funcional) from " + tabela + " where funcional='8013190501'", "result", pm).Tables[0]; // + " where funcional='1097878'"   where funcional='1002193658100'
                    List<Result> resultado = new List<Result>();

                    int total = dt1.Rows.Count;
                    int i = 0;

                    bool salto = false;
                    foreach (DataRow row in dt1.Rows)
                    {
                        i++;
                        salto = false;
                        funcional = Convert.ToString(row["Funcional"]);
                        valorTotalReajustado = 0;
                        aux = LocatorHelper.Instance.ExecuteScalar("select beneficiario_id from beneficiario where beneficiario_matriculafuncional like '%" + funcional + "'", null, null, pm);

                        if (aux == null)
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update " + tabela + " set naoLocalizado=1 where Funcional='" + funcional + "'", pm);
                            continue;
                        }

                        beneficiario = new Beneficiario(aux);
                        pm.Load(beneficiario);

                        temMedial = false;

                        dtAdicionais.Rows.Clear();
                        dtAdicionais = LocatorHelper.Instance.ExecuteQuery("select adicional_tipo,adicional_beneficiario.* from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid where adicionalbeneficiario_beneficiarioid=" + aux, "result", pm).Tables[0];

                        valorSemReajuste = 0;

                        foreach (DataRow rowAd in dtAdicionais.Rows)
                        {
                            ab = AdicionalBeneficiario.Carregar(rowAd["adicionalbeneficiario_id"], pm);
                            if (ab == null) continue;
                            if (!ab.Recorrente) continue; //nao processa cobranças avulsas não recorrentes

                            valorSemReajuste += ab.RetornaValor(-1, vigencia);

                            ab.AplicaRejuste(reajustePlano, reajusteTaxa, reajusteSeguro);
                            //ab.Atualizado = true;

                            valorTotalReajustado += ab.RetornaValor(-1, vigencia);

                            if (Convert.ToInt32(ab.AdicionalCodigo) >= 4435 && Convert.ToInt32(ab.AdicionalCodigo) <= 4439) temMedial = true;

                        }

                        //verifica se tem plano
                        if (!temMedial) // não gera boleto para quem não é medial
                        {
                            //NonQueryHelper.Instance.ExecuteNonQuery("update " + tabela + " set semPlano=1 where Funcional='" + funcional + "'", pm);
                            salto = true;
                        }

                        //verifica se tem previdencia. se tiver, nao deve enviar boleto. 
                        aux = LocatorHelper.Instance.ExecuteScalar("select id from ___previdencia where dados like '%" + beneficiario.MatriculaFuncional + "%'", null, null, pm);
                        if (aux != null && aux != DBNull.Value)
                        {
                            NonQueryHelper.Instance.ExecuteNonQuery("update " + tabela + " set previdencia=1 where Funcional='" + funcional + "'", pm);
                            salto = true;
                        }

                        if (salto) continue;

                        //Calcula a diferenca
                        dtDescontado = LocatorHelper.Instance.ExecuteQuery(string.Concat("select ValorTratado from " + tabela + " where funcional like '%", funcional, "'"), "desc", pm).Tables[0];
                        descontado = 0;
                        foreach (DataRow descRow in dtDescontado.Rows)
                        {
                            descontado += Convert.ToDecimal(descRow[0]);
                        }

                        Result r = new Result();
                        r.Associativa = beneficiario.MatriculaAssociativa;
                        r.Funcional = beneficiario.MatriculaFuncional;
                        r.Nome = beneficiario.Nome;
                        r.ValorReajustado = valorTotalReajustado.ToString("N2");
                        r.ValorSemReajuste = valorSemReajuste.ToString("N2");
                        r.ValorDoArquivoEnviado = descontado.ToString("N2");
                        r.ValorACobrar = (valorTotalReajustado - descontado).ToString("N2");
                        r.Tipo = "DESCONTADO (DIFERENCA)";
                        resultado.Add(r);

                        //obtem id do contrato
                        aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 where contratobeneficiario_beneficiarioId=" + beneficiario.ID, null, null, pm);

                        cob = new Cobranca();
                        cob.Tipo = (int)Cobranca.eTipo.DiferencaUbraSP;
                        cob.Parcela = 0;
                        //cob.DataVencimento = Convert.ToDateTime(row["vencimento"]);
                        //cob.DataVencimento = new DateTime(cob.DataVencimento.Year, cob.DataVencimento.Month, cob.DataVencimento.Day, 23, 59, 59, 995);
                        cob.DataVencimento = new DateTime(2016, 09, 19, 23, 59, 59, 995);
                        cob.Valor = (valorTotalReajustado - descontado);
                        cob.CobrancaRefID = null;
                        cob.DataPgto = DateTime.MinValue;
                        cob.ValorPgto = Decimal.Zero;
                        cob.Pago = false;
                        cob.PropostaID = aux;
                        cob.Cancelada = false;
                        cob.ArquivoIDUltimoEnvio = -3;

                        if (cob.Valor > 3)
                        {
                            //pm.Save(cob);
                            //NonQueryHelper.Instance.ExecuteNonQuery("update ___result set result_boletoGerado=1 where result_matriculaAsso='" + r.Associativa + "'", pm);
                        }
                    }

                    foreach (var r in resultado)
                    {
                        //pm.Save(r);
                    }

                    pm.Commit();
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
        }

        void arrumaErro()
        {
            string ids = "97066,65211,65212,78462,78463,78464,69651,103932,69826,104197,76226,76227,99138,99139,66721,66722,68395,102185,77973,77974,98901,98902,66609,66610,68646,68647,102485,102486,72089,72090,108608,68330,68331,102111,73476,110141,97239,97240,65358,77677,77678,77679,77680,77681,70814,70815";

            DataTable dtCerto = new DataTable();
            using (SqlConnection conn = new SqlConnection("server=dev01;database=ubrasp_20160902;user=sa;pwd=lcmaster0000"))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter("select * from adicional_beneficiario where adicionalbeneficiario_id in (" + ids + ") order by adicionalbeneficiario_id", conn);
                adp.Fill(dtCerto);
                conn.Close();
            }

            string[] arr = ids.Split(',');

            AdicionalBeneficiario ab = new AdicionalBeneficiario();
            foreach (string id in arr)
            {
                ab.ID = id;
                ab.Carregar();

                DataRow[] rows = dtCerto.Select("adicionalbeneficiario_id=" + id);

                ab.Valor01 = Convert.ToDecimal(rows[0]["adicionalbeneficiario_valor"]);
                ab.PRE_COB_1 = Convert.ToDecimal(rows[0]["adicionalbeneficiario_preCob1"]);
                ab.PRE_COB_2 = Convert.ToDecimal(rows[0]["adicionalbeneficiario_preCob2"]);
                ab.PRE_COB_3 = Convert.ToDecimal(rows[0]["adicionalbeneficiario_preCob3"]);
                ab.PRE_COB_4 = Convert.ToDecimal(rows[0]["adicionalbeneficiario_preCob4"]);
                ab.PRE_COB_5 = Convert.ToDecimal(rows[0]["adicionalbeneficiario_preCob5"]);
                ab.PRE_COB_6 = Convert.ToDecimal(rows[0]["adicionalbeneficiario_preCob6"]);

                ab.Salvar();
            }
        }

        protected int CToInt(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
                return Convert.ToInt32(param);
        }
        protected Decimal CToDecimal(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
                return Convert.ToDecimal(param, new System.Globalization.CultureInfo("pt-Br"));
        }
        protected Decimal CToDecimal(Object param, System.Globalization.CultureInfo cinfo)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
                return Convert.ToDecimal(param, cinfo);
        }
        protected String CToString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return String.Empty;
            else
                return Convert.ToString(param);
        }
    }


    #region classes 

    [DBTable("___result")]
    public class Result : EntityBase, IPersisteableEntity
    {
        [DBFieldInfo("result_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID { get; set; }

        [DBFieldInfo("result_matriculaAsso", FieldType.Single)]
        public String Associativa { get; set; }
        [DBFieldInfo("result_matriculaFunc", FieldType.Single)]
        public String Funcional { get; set; }
        [DBFieldInfo("result_nome", FieldType.Single)]
        public String Nome { get; set; }

        [DBFieldInfo("result_valorACobrar", FieldType.Single)]
        public String ValorACobrar { get; set; }

        [DBFieldInfo("result_valorReajustado", FieldType.Single)]
        public String ValorReajustado { get; set; }
        [DBFieldInfo("result_valorSemReajuste", FieldType.Single)]
        public String ValorSemReajuste { get; set; }

        [DBFieldInfo("result_valorDoArquivoEnviado", FieldType.Single)]
        public String ValorDoArquivoEnviado { get; set; }

        [DBFieldInfo("result_tipo", FieldType.Single)]
        public String Tipo { get; set; }
        [DBFieldInfo("result_mensagem", FieldType.Single)]
        public String Mensagem { get; set; }
    }

    [Serializable()]
    [DBTable("reajusteCabecalho")]
    public class ReajusteCabecalho : EntityBase, IPersisteableEntity
    {
        public ReajusteCabecalho() { Data = DateTime.Now; }

        #region Propriedades

        [DBFieldInfo("reajuste_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID { get; set; }

        [DBFieldInfo("reajuste_usuarioId", FieldType.Single)]
        public Object UsuarioID { get; set; }

        [DBFieldInfo("reajuste_descricao", FieldType.Single)]
        public string Descricao { get; set; }

        [DBFieldInfo("reajuste_contratoAdmId", FieldType.Single)]
        public int ContratoADMId { get; set; }

        /// <summary>
        /// 1 = PSCC
        /// </summary>
        [DBFieldInfo("reajuste_tipoArquivo", FieldType.Single)]
        public int TipoArquivo { get; set; }

        [DBFieldInfo("reajuste_formaPagto", FieldType.Single)]
        public string FormaPagto { get; set; }

        [DBFieldInfo("reajuste_tipo", FieldType.Single)]
        public int Tipo { get; set; }

        [DBFieldInfo("reajuste_adicionalId", FieldType.Single)]
        public int AdicionalId { get; set; }

        [DBFieldInfo("reajuste_premio", FieldType.Single)]
        public decimal Premio { get; set; }

        [DBFieldInfo("reajuste_cobertura", FieldType.Single)]
        public decimal Cobertura { get; set; }

        [DBFieldInfo("reajuste_data", FieldType.Single)]
        public DateTime Data { get; set; }

        #endregion

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
    }

    [Serializable()]
    [DBTable("reajusteItem")]
    public class ReajusteItem : EntityBase, IPersisteableEntity
    {
        public ReajusteItem() { }

        #region Propriedades

        [DBFieldInfo("reajusteitem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID { get; set; }

        [DBFieldInfo("reajusteitem_cabecalhoId", FieldType.Single)]
        public Object CabecalhoId { get; set; }

        [DBFieldInfo("reajusteitem_adicionalBeneficiarioId", FieldType.Single)]
        public object AdicionalBeneficiarioID { get; set; }

        [DBFieldInfo("reajusteitem_valorAtual", FieldType.Single)]
        public decimal ValorAtual { get; set; }

        [DBFieldInfo("reajusteitem_valorReajustado", FieldType.Single)]
        public decimal ValorReajustado { get; set; }

        #endregion

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
    }

    public enum SearchMatchType : int
    {
        QualquerParteDoCampo,
        InicioDoCampo,
        CampoInteiro
    }

    [DBTable("plano")]
    public class Plano : EntityBase, IPersisteableEntity
    {
        public Plano() { _ativo = true; }
        public Plano(Object id) : this() { _id = id; }

        #region Campos

        Object _id;
        Object _contratoId;
        Object _tabelaValorAtualId;
        String _descricao;
        String _codigo;
        String _subplano;
        DateTime _inicioColetivo;
        String _codigoParticular;
        String _subplanoParticular;
        DateTime _inicioParticular;
        String _caracteristicas;
        Boolean _ativo;
        Boolean _quartoParticular;
        Boolean _quartoComum;
        String _ansComum;
        String _ansParticular;

        #endregion

        #region Propriedades

        [DBFieldInfo("plano_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("plano_contratoId", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId = value; }
        }

        [DBFieldInfo("plano_tabelaValorAtualId", FieldType.Single)]
        public Object TabelaValoreAtualID
        {
            get { return _tabelaValorAtualId; }
            set { _tabelaValorAtualId = value; }
        }

        [DBFieldInfo("plano_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao = value; }
        }

        public String DescricaoPlanoSubPlano
        {
            get
            {
                String ret = "";

                if (!String.IsNullOrEmpty(_codigo)) { ret += "Cod " + _codigo; }
                if (!String.IsNullOrEmpty(_subplano))
                {
                    if (ret.Length > 0) { ret += " "; }
                    ret += "Sub " + _subplano;
                }

                if (!String.IsNullOrEmpty(_codigoParticular))
                {
                    if (ret.Length > 0) { ret += " - "; }
                    ret += "Cod " + _codigoParticular;
                }

                if (!String.IsNullOrEmpty(_subplanoParticular))
                {
                    if (ret.Length > 0) { ret += " "; }
                    ret += "Sub " + _subplanoParticular;
                }

                return String.Concat(_descricao, " (", ret, ")");
            }
        }

        [DBFieldInfo("plano_codigo", FieldType.Single)]
        public String Codigo
        {
            get { return _codigo; }
            set { _codigo = value; }
        }

        [DBFieldInfo("plano_subplano", FieldType.Single)]
        public String SubPlano
        {
            get { return _subplano; }
            set { _subplano = value; }
        }

        [DBFieldInfo("plano_inicioColetivo", FieldType.Single)]
        public DateTime InicioColetivo
        {
            get { return _inicioColetivo; }
            set { _inicioColetivo = value; }
        }

        [DBFieldInfo("plano_codigoParticular", FieldType.Single)]
        public String CodigoParticular
        {
            get { return _codigoParticular; }
            set { _codigoParticular = value; }
        }

        [DBFieldInfo("plano_subplanoParticular", FieldType.Single)]
        public String SubPlanoParticular
        {
            get { return _subplanoParticular; }
            set { _subplanoParticular = value; }
        }

        [DBFieldInfo("plano_inicioParticular", FieldType.Single)]
        public DateTime InicioParticular
        {
            get { return _inicioParticular; }
            set { _inicioParticular = value; }
        }

        [DBFieldInfo("plano_caracteristica", FieldType.Single)]
        public String Caracteristicas
        {
            get { return _caracteristicas; }
            set { _caracteristicas = value; }
        }

        [DBFieldInfo("plano_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo = value; }
        }

        [DBFieldInfo("plano_quartoComum", FieldType.Single)]
        public Boolean QuartoComum
        {
            get { return _quartoComum; }
            set { _quartoComum = value; }
        }

        [DBFieldInfo("plano_quartoParticular", FieldType.Single)]
        public Boolean QuartoParticular
        {
            get { return _quartoParticular; }
            set { _quartoParticular = value; }
        }

        [DBFieldInfo("plano_quartoComunAns", FieldType.Single)]
        public String AnsQuartoComum
        {
            get { return _ansComum; }
            set { _ansComum = value; }
        }

        [DBFieldInfo("plano_quartoParticularAns", FieldType.Single)]
        public String AnsQuartoParticular
        {
            get { return _ansParticular; }
            set { _ansParticular = value; }
        }

        public String strDatasInicio
        {
            get
            {
                String ret = "";

                if (_inicioColetivo != DateTime.MinValue)
                    ret += "coletivo: " + _inicioColetivo.ToString("dd/MM/yyyy");

                if (_inicioParticular != DateTime.MinValue)
                {
                    if (ret.Length > 0) { ret += " e "; }

                    ret += "particular: " + _inicioParticular.ToString("dd/MM/yyyy");
                }

                return ret;
            }
        }
        #endregion


        #region persistence methods

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }
        #endregion

        public static IList<Plano> CarregarPorContratoID(Object contratoID)
        {
            return Plano.CarregarPorContratoID(contratoID, false);
        }

        public static IList<Plano> CarregarPorContratoID(Object contratoID, Boolean apenasAtivos)
        {
            String query = "* FROM plano WHERE plano_contratoId=" + contratoID;
            if (apenasAtivos) { query += " AND plano_ativo=1"; }
            query += " ORDER BY plano_descricao";

            return LocatorHelper.Instance.ExecuteQuery<Plano>(query, typeof(Plano));
        }

        public static Object CarregarID(Object contratoAdmID, String codigo, String subPlano, PersistenceManager pm)
        {
            String qry = "SELECT plano_id FROM plano WHERE plano_contratoId=" + contratoAdmID + " AND (plano_codigo='" + codigo + "' OR plano_codigoParticular='" + codigo + "') AND (plano_subplano='" + subPlano + "' OR plano_subplanoParticular='" + subPlano + "')";

            return LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
        }

        public static Plano Carregar(Object contratoAdmID, String codigo, String subPlano, PersistenceManager pm)
        {
            String qry = "SELECT * FROM plano WHERE plano_contratoId=" + contratoAdmID + " AND (plano_codigo='" + codigo + "' OR plano_codigoParticular='" + codigo + "') AND (plano_subplano='" + subPlano + "' OR plano_subplanoParticular='" + subPlano + "')";

            IList<Plano> list = LocatorHelper.Instance.ExecuteQuery<Plano>(qry, typeof(Plano), pm);
            if (list == null || list.Count == 0)
                return null;
            else
                return list[0];
        }

        public static IList<Plano> CarregarPorOperadoraID(Object operadoraId)
        {
            String query = "plano.* FROM plano INNER JOIN contratoadm ON plano_contratoId= contratoadm_id INNER JOIN operadora on operadora_id = contratoadm_operadoraId WHERE operadora_inativa=0 AND contratoadm_ativo=1 AND plano_ativo=1 AND operadora_id=" + operadoraId + " ORDER BY plano_descricao";

            return LocatorHelper.Instance.ExecuteQuery<Plano>(query, typeof(Plano));
        }

        public static IList<Plano> CarregaPlanosDaTabelaDeValor(Object tabelaDeValorId)
        {
            String qry = "DISTINCT(tabelavaloritem_planoId) as plano_id FROM tabela_valor_item WHERE tabelavaloritem_tabelaid=" + tabelaDeValorId;
            return LocatorHelper.Instance.ExecuteQuery<Plano>(qry, typeof(Plano));
        }

        public static Boolean Existe(Object contratoId, Object planoId, String planoDescricao, String qcCodigo, String qcSubPlano, String qpCodigo, String qpSubPlano)
        {
            String qry = "SELECT COUNT(*) FROM plano WHERE plano_descricao='" + planoDescricao + "' AND plano_contratoId=" + contratoId;

            #region TODO: parametrizar a frase sql

            if (!String.IsNullOrEmpty(qcCodigo))
            {
                qry += " AND plano_codigo='" + qcCodigo + "'";
            }
            if (!String.IsNullOrEmpty(qpSubPlano))
            {
                qry += " AND plano_subplano='" + qcSubPlano + "'";
            }
            if (!String.IsNullOrEmpty(qpCodigo))
            {
                qry += " AND plano_codigoParticular='" + qpCodigo + "'";
            }
            if (!String.IsNullOrEmpty(qpSubPlano))
            {
                qry += " AND plano_subplanoParticular='" + qpSubPlano + "'";
            }
            #endregion

            if (planoId != null)
            {
                qry += " AND plano_id <> " + planoId;
            }

            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, null, null);
            if (ret == null || ret == DBNull.Value || Convert.ToInt32(ret) == 0)
                return false;
            else
                return true;
        }

        public static void AlterarStatus(Object PlanoID, Boolean ativo)
        {
            String command = "UPDATE plano SET plano_ativo=" + Convert.ToInt32(ativo) + " WHERE plano_id=" + PlanoID;
            NonQueryHelper.Instance.ExecuteNonQuery(command, null);
        }

        public static void SetaTabelaValorAutal(Object planoId, Object tabelaValorId, PersistenceManager pm)
        {
            String command = "UPDATE plano SET plano_tabelaValorAtualId=" + tabelaValorId + " WHERE plano_id=" + planoId;
            NonQueryHelper.Instance.ExecuteNonQuery(command, pm);
        }

        public void SetaTabelaValorAutal(Object tabelaValorId, PersistenceManager pm)
        {
            Plano.SetaTabelaValorAutal(this._id, tabelaValorId, pm);
        }
    }


    /// <summary>
    /// Representa a associação de um produto adicional e um beneficiário.
    /// </summary>
    [Serializable]
    [DBTable("adicional_beneficiario")]
    public class AdicionalBeneficiario : EntityBase, IPersisteableEntity
    {
        #region fields

        Object _id;
        Object _propostaid;
        Object _adicionalId;
        Object _beneficiarioid;
        bool _recorrente;
        bool _atualizado;

        String _adicionalDescricao;
        string _adicionalCodigo;
        String _adicionalCodTitular;
        Object _adicionalOperadoraId;
        Boolean _adicionalDental;

        #endregion

        #region propriedades

        [DBFieldInfo("adicionalbeneficiario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("adicionalbeneficiario_propostaId", FieldType.Single)]
        public Object PropostaID
        {
            get { return _propostaid; }
            set { _propostaid = value; }
        }

        [Joinned("adicional_id")]
        [DBFieldInfo("adicionalbeneficiario_adicionalid", FieldType.Single)]
        public Object AdicionalID
        {
            get { return _adicionalId; }
            set { _adicionalId = value; }
        }

        [DBFieldInfo("adicionalbeneficiario_beneficiarioid", FieldType.Single)]
        public Object BeneficiarioID
        {
            get { return _beneficiarioid; }
            set { _beneficiarioid = value; }
        }

        [DBFieldInfo("adicionalbeneficiario_recorrente", FieldType.Single)]
        public Boolean Recorrente
        {
            get { return _recorrente; }
            set { _recorrente = value; }
        }

        [DBFieldInfo("adicionalbeneficiario_atualizado", FieldType.Single)]
        public Boolean Atualizado
        {
            get { return _atualizado; }
            set { _atualizado = value; }
        }

        /// <summary>
        /// Join
        /// </summary>
        [Joinned("adicional_descricao")]
        public String AdicionalDescricao
        {
            get { return _adicionalDescricao; }
            set { _adicionalDescricao = value; }
        }

        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("adicional_codigo")]
        public String AdicionalCodigo
        {
            get { return _adicionalCodigo; }
            set { _adicionalCodigo = value; }
        }

        /// <summary>
        /// Join
        /// </summary>
        [Joinned("adicional_codTitular")]
        public String AdicionalCodTitular
        {
            get { return _adicionalCodTitular; }
            set { _adicionalCodTitular = value; }
        }
        /// <summary>
        /// Join
        /// </summary>
        [Joinned("adicional_operadoraId")]
        public Object AdicionalOperadoraID
        {
            get { return _adicionalOperadoraId; }
            set { _adicionalOperadoraId = value; }
        }
        /// <summary>
        /// Join
        /// </summary>
        [Joinned("adicional_dental")]
        public Boolean AdicionalDental
        {
            get { return _adicionalDental; }
            set { _adicionalDental = value; }
        }

        /// <summary>
        /// Joinned - 0 = Taxa, 1 = Seguro, 2 = Previdencia, 3 = Normal
        /// </summary>
        [Joinned("adicional_tipo")]
        public Int32 AdicionalTipo
        {
            get;
            set;
        }

        public Boolean Sim
        {
            get { return _beneficiarioid != null; }
        }

        [DBFieldInfo("adicionalbeneficiario_formaPagto", FieldType.Single)]
        public int FormaPagto { get; set; }

        //TAXA
        [DBFieldInfo("adicionalbeneficiario_valor", FieldType.Single)]
        public decimal Valor01 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_status01", FieldType.Single)]
        public string Status01 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_status", FieldType.Single)]
        public string Status { get; set; }

        //SEGURO
        [DBFieldInfo("adicionalbeneficiario_codCob1", FieldType.Single)]
        public int COD_COB_1 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_preCob1", FieldType.Single)]
        public decimal PRE_COB_1 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_stSgCob1", FieldType.Single)]
        public string ST_SG_1 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_dtSgCob1", FieldType.Single)]
        public DateTime DT_SG_1 { get; set; }

        [DBFieldInfo("adicionalbeneficiario_codCob2", FieldType.Single)]
        public int COD_COB_2 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_preCob2", FieldType.Single)]
        public decimal PRE_COB_2 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_stSgCob2", FieldType.Single)]
        public string ST_SG_2 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_dtSgCob2", FieldType.Single)]
        public DateTime DT_SG_2 { get; set; }

        [DBFieldInfo("adicionalbeneficiario_codCob3", FieldType.Single)]
        public int COD_COB_3 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_preCob3", FieldType.Single)]
        public decimal PRE_COB_3 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_stSgCob3", FieldType.Single)]
        public string ST_SG_3 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_dtSgCob3", FieldType.Single)]
        public DateTime DT_SG_3 { get; set; }

        [DBFieldInfo("adicionalbeneficiario_codCob4", FieldType.Single)]
        public int COD_COB_4 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_preCob4", FieldType.Single)]
        public decimal PRE_COB_4 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_stSgCob4", FieldType.Single)]
        public string ST_SG_4 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_dtSgCob4", FieldType.Single)]
        public DateTime DT_SG_4 { get; set; }

        [DBFieldInfo("adicionalbeneficiario_codCob5", FieldType.Single)]
        public int COD_COB_5 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_preCob5", FieldType.Single)]
        public decimal PRE_COB_5 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_stSgCob5", FieldType.Single)]
        public string ST_SG_5 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_dtSgCob5", FieldType.Single)]
        public DateTime DT_SG_5 { get; set; }

        [DBFieldInfo("adicionalbeneficiario_codCob6", FieldType.Single)]
        public int COD_COB_6 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_preCob6", FieldType.Single)]
        public decimal PRE_COB_6 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_stSgCob6", FieldType.Single)]
        public string ST_SG_6 { get; set; }
        [DBFieldInfo("adicionalbeneficiario_dtSgCob6", FieldType.Single)]
        public DateTime DT_SG_6 { get; set; }

        

        #endregion

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }
        #endregion

        public AdicionalBeneficiario() { }

        public static IList<AdicionalBeneficiario> Carregar(Object propostaId, Object beneficiarioId)
        {
            return Carregar(propostaId, beneficiarioId, null);
        }
        public static IList<AdicionalBeneficiario> Carregar(Object propostaId, Object beneficiarioId, PersistenceManager pm)
        {
            String query = "adicional_beneficiario.*, adicional_descricao, adicional_operadoraId, adicional_codTitular,adicional_dental FROM adicional_beneficiario INNER JOIN adicional ON adicionalbeneficiario_adicionalid=adicional_id WHERE adicionalbeneficiario_propostaId=" + propostaId + " AND adicionalbeneficiario_beneficiarioid=" + beneficiarioId + " ORDER BY adicional_descricao";
            return LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario), pm);
        }

        public static AdicionalBeneficiario Carregar(Object adicionalBeneficiarioId, PersistenceManager pm)
        {
            String query = "adicional_beneficiario.*, adicional_descricao, adicional_codigo, adicional_tipo, adicional_operadoraId, adicional_codTitular,adicional_dental FROM adicional_beneficiario INNER JOIN adicional ON adicionalbeneficiario_adicionalid=adicional_id WHERE adicionalbeneficiario_id=" + adicionalBeneficiarioId;
            IList<AdicionalBeneficiario> lista = LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario), pm);

            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }

        public static Boolean TemDental(IList<AdicionalBeneficiario> lista)
        {
            if (lista == null) { return false; }

            foreach (AdicionalBeneficiario ab in lista)
            {
                if (ab == null) { continue; }
                if (ab.AdicionalDental) { return true; }
            }

            return false;
        }

        public static Boolean EDental(AdicionalBeneficiario ab)
        {
            if (ab != null && ab.AdicionalDental)
                return true;
            else
                return false;
        }

        public static IList<AdicionalBeneficiario> Carregar(Object contratoADMId, Object planoId, Object propostaId, Object beneficiarioId)
        {
            String query = String.Concat("adicional_beneficiario.*, adicional_id, adicional_descricao, adicional_codigo ",
                "FROM adicional",
                "  INNER JOIN contratoADM_plano_adicional ON adicional_id=contratoplanoadicional_adicionalid",
                "  LEFT JOIN adicional_beneficiario ON adicionalbeneficiario_adicionalid=adicional_id ");

            if (propostaId != null) { query += "AND adicionalbeneficiario_propostaid=" + propostaId; }

            query = String.Concat(query, " AND adicionalbeneficiario_beneficiarioid=", beneficiarioId,
                " WHERE contratoplanoadicional_contratoid=", contratoADMId, " AND contratoplanoadicional_planoid=", planoId, " ORDER BY adicional_descricao");

            return LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario));
        }

        public static AdicionalBeneficiario CarregarParaBeneficiario(Object contratoId, Object beneficiarioId, Object adicionalId, PersistenceManager pm)
        {
            String query = String.Concat("* ",
                "FROM adicional_beneficiario ",
                "  WHERE adicionalbeneficiario_propostaId=", contratoId, " AND adicionalbeneficiario_beneficiarioId=", beneficiarioId, " AND adicionalbeneficiario_adicionalId=", adicionalId);

            IList<AdicionalBeneficiario> lista = LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario));

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        public static IList<AdicionalBeneficiario> Carregar(Object contratoADMId, Object planoId, Object propostaId)
        {
            String query = String.Concat("adicional_beneficiario.*, adicional_id, adicional_descricao, adicional_codigo ",
                "FROM adicional",
                "  INNER JOIN contratoADM_plano_adicional ON adicional_id=contratoplanoadicional_adicionalid",
                "  LEFT JOIN adicional_beneficiario ON adicionalbeneficiario_adicionalid=adicional_id ");

            if (propostaId != null) { query += "AND adicionalbeneficiario_propostaid=" + propostaId; }

            query = String.Concat(query, " WHERE contratoplanoadicional_contratoid=", contratoADMId, " AND contratoplanoadicional_planoid=", planoId, " ORDER BY adicional_descricao");

            return LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario));
        }


        ///////////

        /// <summary>
        /// 31
        /// </summary>
        public static readonly int _FormaPagtoBoleto = 31;
        /// <summary>
        /// 09
        /// </summary>
        public static readonly int _FormaPagtoCredito = 09;
        /// <summary>
        /// 10
        /// </summary>
        public static readonly int _FormaPagtoDebito = 10;
        /// <summary>
        /// 11
        /// </summary>
        public static readonly int _FormaPagtoDescFolha = 11;
        /// <summary>
        /// 81
        /// </summary>
        public static readonly int _FormaPagtoDescConta = 81;

        public string RetornaNome()
        {
            if (this.AdicionalTipo == 0) return "Taxa";
            else if (this.AdicionalTipo == 3) return "Normal";
            else if (this.AdicionalTipo == 1) return "Seguro";
            else if (this.AdicionalTipo == 2) return "Previdencia";
            else return "";
        }

        public decimal RetornaValor(int formaPagto, DateTime vigencia, bool naoRecorrente = false)
        {
            if (this.Recorrente && naoRecorrente) return 0;

            if (this.AdicionalTipo != 1) //não é seguro
            {
                if (this.FormaPagto == formaPagto || formaPagto == -1)
                {
                    if (this.FormaPagto == AdicionalBeneficiario._FormaPagtoBoleto)
                    {
                        if (this.Status == null || this.Status.ToUpper() != "A") return 0;
                        else return this.Valor01;
                    }
                    else
                    {
                        if (this.Status == null || this.Status.ToUpper() != "A") return 0;
                        else return this.Valor01;
                    }
                }
                else
                    return 0;
            }
            else if (this.AdicionalTipo == 1) //seguro
            {
                if (this.FormaPagto == formaPagto || formaPagto == -1)
                {
                    //if (this.Status == null || this.Status.ToUpper() != "A") return 0;

                    decimal total = 0;

                    if (this.ST_SG_1 != null && this.ST_SG_1.ToUpper() == "A") total += this.PRE_COB_1;
                    if (this.ST_SG_2 != null && this.ST_SG_2.ToUpper() == "A") total += this.PRE_COB_2;
                    if (this.ST_SG_3 != null && this.ST_SG_3.ToUpper() == "A") total += this.PRE_COB_3;
                    if (this.ST_SG_4 != null && this.ST_SG_4.ToUpper() == "A") total += this.PRE_COB_4;
                    if (this.ST_SG_5 != null && this.ST_SG_5.ToUpper() == "A") total += this.PRE_COB_5;
                    if (this.ST_SG_6 != null && this.ST_SG_6.ToUpper() == "A") total += this.PRE_COB_6;

                    return total;
                }
                else
                    return 0;
            }
            else
                return 0;
            }

        public void AplicaRejuste(decimal reajustePlano, decimal reajusteTaxa, decimal reajusteSeguro)
        {
            if (this.AdicionalTipo != 1) //NAO é SEGURO
            {
                string status = this.Status;

                //if (this.FormaPagto == AdicionalBeneficiario._FormaPagtoBoleto) status = this.Status01;

                if (status != null && status.ToUpper() == "A")
                {
                    int plano = Convert.ToInt32(this._adicionalCodigo);

                    //decimal reajustePlano = 18.49M, reajusteTaxa = 5.91M, reajusteSeguro = 5.91M; <= cola

                    if (plano == 4425 || plano == 4424)
                    {
                        this.Valor01 = (reajusteTaxa / 100M) * this.Valor01 + this.Valor01;
                    }
                    else if (plano >= 4435 && plano <= 4439)
                    {
                        this.Valor01 = (reajustePlano / 100M) * this.Valor01 + this.Valor01;
                    }
                    else
                        this.Valor01 = (reajusteTaxa / 100M) * this.Valor01 + this.Valor01;
                }
            }
            else if (this.AdicionalTipo == 1) //seguro
            {
                if (this.ST_SG_1 != null && this.ST_SG_1.ToUpper() == "A") this.PRE_COB_1 = (reajusteSeguro / 100M) * this.PRE_COB_1 + this.PRE_COB_1;
                if (this.ST_SG_2 != null && this.ST_SG_2.ToUpper() == "A") this.PRE_COB_2 = (reajusteSeguro / 100M) * this.PRE_COB_2 + this.PRE_COB_2;
                if (this.ST_SG_3 != null && this.ST_SG_3.ToUpper() == "A") this.PRE_COB_3 = (reajusteSeguro / 100M) * this.PRE_COB_3 + this.PRE_COB_3;
                if (this.ST_SG_4 != null && this.ST_SG_4.ToUpper() == "A") this.PRE_COB_4 = (reajusteSeguro / 100M) * this.PRE_COB_4 + this.PRE_COB_4;
                if (this.ST_SG_5 != null && this.ST_SG_5.ToUpper() == "A") this.PRE_COB_5 = (reajusteSeguro / 100M) * this.PRE_COB_5 + this.PRE_COB_5;
                if (this.ST_SG_6 != null && this.ST_SG_6.ToUpper() == "A") this.PRE_COB_6 = (reajusteSeguro / 100M) * this.PRE_COB_6 + this.PRE_COB_6;
            }
        }
    }

    [Serializable]
    [DBTable("adicional")]
    public class Adicional : EntityBase, IPersisteableEntity
    {
        #region fields

        Object _id;
        Object _operadoraId;
        String _descricao;
        String _codTitular;
        Boolean _ativo;
        Decimal _valorUnico;
        Boolean _paraTodaProposta;
        DateTime _data;
        Boolean _dental;

        #endregion

        #region propriedades

        [DBFieldInfo("adicional_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("adicional_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId = value; }
        }

        [DBFieldInfo("adicional_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao = value; }
        }

        [DBFieldInfo("adicional_codTitular", FieldType.Single)]
        public String CodTitular
        {
            get { return _codTitular; }
            set { _codTitular = value; }
        }

        [DBFieldInfo("adicional_paraTodaProposta", FieldType.Single)]
        public Boolean ParaTodaProposta
        {
            get { return _paraTodaProposta; }
            set { _paraTodaProposta = value; }
        }

        [DBFieldInfo("adicional_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo = value; }
        }

        [DBFieldInfo("adicional_dental", FieldType.Single)]
        public Boolean Dental
        {
            get { return _dental; }
            set { _dental = value; }
        }

        [DBFieldInfo("adicional_codigo", FieldType.Single)]
        public String Codigo
        {
            get;
            set;
        }
        /// <summary>
        /// 0 = Taxa, 1 = Seguro
        /// </summary>
        [DBFieldInfo("adicional_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get;
            set;
        }

        //[DBFieldInfo("adicional_valorUnico", FieldType.Single)]
        //public Decimal ValorUnico
        //{
        //    get { return _valorUnico; }
        //    set { _valorUnico= value; }
        //}

        //[DBFieldInfo("adicional_data", FieldType.Single)]
        //public DateTime Data
        //{
        //    get { return _data; }
        //    set { _data= value; }
        //}

        #endregion

        public Adicional() { _ativo = true; _data = DateTime.Now; Tipo = 0; }
        public Adicional(Object id) : this() { _id = id; }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<Adicional> CarregarPorOperadoraID(Object operadoraId)
        {
            String query = "adicional.*, operadora_nome FROM adicional LEFT JOIN operadora ON operadora_id=adicional_operadoraId WHERE adicional_operadoraId=" + operadoraId + " ORDER BY adicional_descricao";
            return LocatorHelper.Instance.ExecuteQuery<Adicional>(query, typeof(Adicional));
        }

        public static Adicional CarregarPorOperadoraID(Object operadoraId, String adicionalDescricao, PersistenceManager pm)
        {
            String query = "adicional.*, operadora_nome FROM adicional LEFT JOIN operadora ON operadora_id=adicional_operadoraId WHERE adicional_operadoraId=" + operadoraId + " AND adicional_descricao='" + adicionalDescricao + "'";
            IList<Adicional> lista = LocatorHelper.Instance.ExecuteQuery<Adicional>(query, typeof(Adicional), pm);

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        public static IList<Adicional> Carregar(Object[] ids)
        {
            String inClausule = String.Join(",", (String[])ids);
            String query = "adicional.*, operadora_nome FROM adicional LEFT JOIN operadora ON operadora_id=adicional_operadoraId WHERE adicional_id IN (" + inClausule + ") ORDER BY adicional_descricao";
            return LocatorHelper.Instance.ExecuteQuery<Adicional>(query, typeof(Adicional));
        }

        public static Object CarregarIDPorCodigoTitular(String codigoTitular, Object operadoraId, PersistenceManager pm)
        {
            String query = "SELECT adicional_id FROM adicional WHERE adicional_codTitular='" + codigoTitular + "' AND adicional_operadoraID=" + operadoraId;
            return LocatorHelper.Instance.ExecuteScalar(query, null, null, pm);
        }

        public static Decimal CalculaValor(Object adicionalId, Object beneficiarioId, Int32 beneficiarioIdade)
        {
            return CalculaValor(adicionalId, beneficiarioId, beneficiarioIdade, null, null);
        }
        public static Decimal CalculaValor(Object adicionalId, Object beneficiarioId, Int32 beneficiarioIdade, DateTime? dataReferencia, PersistenceManager pm)
        {
            Adicional adicional = new Adicional();
            adicional.ID = adicionalId;
            adicional.Carregar();

            if (adicional.ID == null) { return -1; }

            IList<AdicionalFaixa> faixa = AdicionalFaixa.CarregarPorTabela(adicionalId, dataReferencia, pm);

            if (faixa != null && faixa.Count > 0)
            {
                if (beneficiarioIdade == -1)
                {
                    Beneficiario beneficiario = new Beneficiario();
                    beneficiario.ID = beneficiarioId;
                    if (pm == null)
                        beneficiario.Carregar();
                    else
                        pm.Load(beneficiario);
                    if (beneficiario.ID == null) { return -1; }
                    beneficiarioIdade = Beneficiario.CalculaIdade(beneficiario.DataNascimento);
                }

                foreach (AdicionalFaixa _item in faixa)
                {
                    if (beneficiarioIdade >= _item.IdadeInicio && _item.IdadeFim == 0)
                    {
                        return _item.Valor;
                    }
                    else if (beneficiarioIdade >= _item.IdadeInicio && beneficiarioIdade <= _item.IdadeFim)
                    {
                        return _item.Valor;
                    }
                }

                return 0;
            }
            else
                return 0;
        }
    }

    [Serializable]
    [DBTable("adicional_faixa")]
    public class AdicionalFaixa : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _adicionalId;
        int _idadeInicio;
        int _idadeFim;
        Decimal _valor;
        DateTime _vigencia;

        #region propriedades

        [DBFieldInfo("adicionalfaixa_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("adicionalfaixa_adicionalid", FieldType.Single)]
        public Object AdicionalId
        {
            get { return _adicionalId; }
            set { _adicionalId = value; }
        }

        [DBFieldInfo("adicionalfaixa_idadeInicio", FieldType.Single)]
        public int IdadeInicio
        {
            get { return _idadeInicio; }
            set { _idadeInicio = value; }
        }

        [DBFieldInfo("adicionalfaixa_idadeFim", FieldType.Single)]
        public int IdadeFim
        {
            get { return _idadeFim; }
            set { _idadeFim = value; }
        }

        [DBFieldInfo("adicionalfaixa_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }

        [DBFieldInfo("adicionalfaixa_vigencia", FieldType.Single)]
        public DateTime Vigencia
        {
            get { return _vigencia; }
            set { _vigencia = value; }
        }

        public String strVigencia
        {
            get
            {
                if (_vigencia == DateTime.MinValue)
                    return "";
                else
                    return _vigencia.ToString("dd/MM/yyyy");
            }
        }

        public String FaixaEtaria
        {
            get
            {
                String ret = "";

                if (_idadeFim > 0)
                    ret = String.Concat("de ", _idadeInicio, " a ", _idadeFim);
                else
                    ret = String.Concat("de ", _idadeInicio, " em diante");

                return ret;
            }
        }

        #endregion

        public AdicionalFaixa() { }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<AdicionalFaixa> CarregarPorTabela(Object adicionalId, DateTime? dataReferencia)
        {
            return CarregarPorTabela(adicionalId, null, null);
        }
        public static IList<AdicionalFaixa> CarregarPorTabela(Object adicionalId, DateTime? vigencia, PersistenceManager pm)
        {
            String query = "* FROM adicional_faixa WHERE adicionalfaixa_adicionalid=" + adicionalId;

            if (vigencia != null)
            {
                query += " and adicionalfaixa_vigencia <= '" + vigencia.Value.ToString("yyyy-MM-dd") + "'";
            }

            query += " ORDER BY adicionalfaixa_vigencia DESC, adicionalfaixa_idadeInicio"; //and adicionalfaixa_vigencia = (select max(adicionalfaixa_vigencia) FROM adicional_faixa WHERE adicionalfaixa_adicionalid=2)

            if (vigencia == null)
                return LocatorHelper.Instance.ExecuteQuery<AdicionalFaixa>(query, typeof(AdicionalFaixa), pm);
            else
            {
                IList<AdicionalFaixa> lista = LocatorHelper.Instance.ExecuteQuery<AdicionalFaixa>(query, typeof(AdicionalFaixa), pm);

                if (lista == null) { return null; }

                DateTime vigenciaRetornada = lista[0]._vigencia;

                List<AdicionalFaixa> ret = new List<AdicionalFaixa>();
                foreach (AdicionalFaixa af in lista)
                {
                    if (af._vigencia == vigenciaRetornada) { ret.Add(af); }
                }

                return ret;
            }
        }
    }

    [DBTable("beneficiario")]
    public class Beneficiario : EntityBase, IPersisteableEntity
    {
        Object _id;
        String _nome;
        String _cpf;
        String _rg;
        String _rgUF;
        String _rgOrgaoExp;
        Object _estadoCivilId;
        DateTime _dataNascimento;
        DateTime _dataCasamento;
        String _fone;
        String _ramal1;
        String _fone2;
        String _ramal2;
        String _cel;
        String _celOperadora;
        String _email;
        String _nomeMae;
        String _sexo;
        Decimal _peso;
        Decimal _altura;
        String _declaracaoNascimentoVivo;
        String _cns;

        String _contratoNumero;
        int _tipoParticipacaoContrato;
        int _importId;
        Object _enriquecimentoId;

        #region propriedades

        [DBFieldInfo("beneficiario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("beneficiario_nome", FieldType.Single)]
        public String Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        [DBFieldInfo("beneficiario_codigoOrgao", FieldType.Single)]
        public String CodigoOrgaoAverbador
        {
            get;
            set;
        }

        [DBFieldInfo("beneficiario_cpf", FieldType.Single)]
        public String CPF
        {
            get { return _cpf; }
            set { _cpf = value; }
        }

        [DBFieldInfo("beneficiario_rg", FieldType.Single)]
        public String RG
        {
            get { return _rg; }
            set { _rg = value; }
        }

        [DBFieldInfo("beneficiario_rgUF", FieldType.Single)]
        public String RgUF
        {
            get { return _rgUF; }
            set { _rgUF = value; }
        }

        [DBFieldInfo("beneficiario_rgOrgaoExp", FieldType.Single)]
        public String RgOrgaoExp
        {
            get { return _rgOrgaoExp; }
            set { _rgOrgaoExp = value; }
        }

        //[DBFieldInfo("beneficiario_estadoCivilId", FieldType.Single)]
        //Object EstadoCivilID/////////////////////////////////////////////////////////////////////////
        //{
        //    get { return _estadoCivilId; }
        //    set { _estadoCivilId= value; }
        //}

        [DBFieldInfo("beneficiario_dataNascimento", FieldType.Single)]
        public DateTime DataNascimento
        {
            get { return _dataNascimento; }
            set { _dataNascimento = value; }
        }

        //[DBFieldInfo("beneficiario_dataCasamento", FieldType.Single)]
        //DateTime DataCasamento/////////////////////////////////////////////////////////////////////////
        //{
        //    get { return _dataCasamento; }
        //    set { _dataCasamento= value; }
        //}

        [DBFieldInfo("beneficiario_nomeMae", FieldType.Single)]
        public String NomeMae
        {
            get { return _nomeMae; }
            set { _nomeMae = value; }
        }

        [DBFieldInfo("beneficiario_telefone", FieldType.Single)]
        public String Telefone
        {
            get { return _fone; }
            set { _fone = value; }
        }

        public String FTelefone
        {
            get
            {
                return base.FormataTelefone(_fone);
            }
        }

        [DBFieldInfo("beneficiario_ramal", FieldType.Single)]
        public String Ramal
        {
            get { return _ramal1; }
            set { _ramal1 = value; }
        }

        [DBFieldInfo("beneficiario_telefone2", FieldType.Single)]
        public String Telefone2
        {
            get { return _fone2; }
            set { _fone2 = value; }
        }

        [DBFieldInfo("beneficiario_ramal2", FieldType.Single)]
        public String Ramal2
        {
            get { return _ramal2; }
            set { _ramal2 = value; }
        }

        [DBFieldInfo("beneficiario_celular", FieldType.Single)]
        public String Celular
        {
            get { return _cel; }
            set { _cel = value; }
        }

        [DBFieldInfo("beneficiario_celularOperadora", FieldType.Single)]
        public String CelularOperadora
        {
            get { return _celOperadora; }
            set { _celOperadora = value; }
        }

        public String FCelular
        {
            get
            {
                return base.FormataTelefone(_cel);
            }
        }

        [DBFieldInfo("beneficiario_email", FieldType.Single)]
        public String Email
        {
            get { return ToLower(_email); }
            set { _email = value; }
        }

        [DBFieldInfo("beneficiario_sexo", FieldType.Single)]
        public String Sexo
        {
            get { return _sexo; }
            set { _sexo = value; }
        }

        //[DBFieldInfo("beneficiario_peso", FieldType.Single)]
        //Decimal Peso//////////////////////////////////////////////////////////////////////////////////////
        //{
        //    get { return _peso; }
        //    set { _peso= value; }
        //}

        //[DBFieldInfo("beneficiario_altura", FieldType.Single)]
        //Decimal Altura/////////////////////////////////////////////////////////////////////////////////////
        //{
        //    get { return _altura; }
        //    set { _altura= value; }
        //}

        [DBFieldInfo("beneficiario_declaracaoNascimentoVivo", FieldType.Single)]
        public String DeclaracaoNascimentoVivo
        {
            get { return _declaracaoNascimentoVivo; }
            set { _declaracaoNascimentoVivo = value; }
        }

        [DBFieldInfo("beneficiario_cns", FieldType.Single)]
        public String CNS
        {
            get { return _cns; }
            set { _cns = value; }
        }

        [DBFieldInfo("beneficiario_matriculaAssociativa", FieldType.Single)]
        public string MatriculaAssociativa
        {
            get;
            set;
        }
        [DBFieldInfo("beneficiario_matriculaFuncional", FieldType.Single)]
        public string MatriculaFuncional
        {
            get;
            set;
        }
        [DBFieldInfo("beneficiario_iniciais", FieldType.Single)]
        public string Iniciais
        {
            get;
            set;
        }

        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("contrato_numero")]
        public String ContratoNumero
        {
            get { return _contratoNumero; }
            set { _contratoNumero = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("contratobeneficiario_tipo")]
        public int TipoParticipacaoContrato
        {
            get { return _tipoParticipacaoContrato; }
            set { _tipoParticipacaoContrato = value; }
        }

        [DBFieldInfo("importId", FieldType.Single)]
        public int ImportID
        {
            get { return _importId; }
            set { _importId = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("enriquecimentoId")]
        public Object EnriquecimentoID
        {
            get { return _enriquecimentoId; }
            set { _enriquecimentoId = value; }
        }


        #endregion

        #region métodos EntityBase

        public void Salvar()
        {
            Beneficiario.limpaCPF(ref _cpf);
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public void Carregar_DataNascimento(PersistenceManager pm)
        {
            String qry = "SELECT beneficiario_dataNascimento FROM beneficiario WHERE beneficiario_id=" + this._id;
            IList<Beneficiario> benef = LocatorHelper.Instance.ExecuteQuery<Beneficiario>(qry, typeof(Beneficiario), pm);

            if (benef == null)
                return;
            else
                this._dataNascimento = benef[0].DataNascimento;
        }
        #endregion

        [DBFieldInfo("beneficiario_legado", FieldType.Single)]
        public int Legado
        {
            get;
            set;
        }



        /// <summary>
        /// Método para sinalizar o Beneficiário em um contrato. (Mudança de Status)
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        /// <param name="Status">Status a ser sinalizado.</param>
        private void DisparaEventoParaGeracaoArquivo(Object ContratoID, ContratoBeneficiario.eStatus Status)
        {
            if (ContratoID != null)
            {
                try
                {
                    ContratoBeneficiario.AlteraStatusBeneficiario(ContratoID, this._id, Status);
                }
                catch (Exception) { throw; }
            }
            else
                throw new ArgumentNullException("ID de Contrato é nulo.");
        }

        private static String DateDiff(int interval, DateTime data)
        {
            return DateDiff(interval, data, DateTime.Now);
        }

        private static String DateDiff(int interval, DateTime data, DateTime dataReferencia)
        {
            String retorno = "";

            TimeSpan tsDuration;
            tsDuration = dataReferencia - data;

            Int32 dias = 0;
            Decimal iMeses = 0;
            Int32 meses = 0;
            Decimal iAnos = 0;
            Int32 anos = 0;

            if (interval == 1)
            {
                iAnos = Convert.ToDecimal(tsDuration.Days / 365.25);
                anos = (int)iAnos;
                iMeses = Convert.ToDecimal((iAnos - anos) * 12);
                meses = (int)iMeses;
                dias = (int)((iMeses - meses) * 24);

                TimeSpan tsDurationDia;
                data = data.AddYears(anos);
                data = data.AddMonths(meses);
                tsDurationDia = DateTime.Now - data;

                retorno = Convert.ToString(anos + "a " + meses + "m " + tsDurationDia.Days + "d");
            }
            else if (interval == 2)
            {
                //retorno = Convert.ToString(Convert.ToInt32(tsDuration.Days / 365));

                iAnos = Convert.ToDecimal(tsDuration.Days / 365.25);
                anos = (int)iAnos;

                retorno = anos.ToString();
            }

            return retorno;
        }

        /// <summary>
        /// Sinaliza o Beneficiário para o Arquivo de Alteração de Cadastro.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        public void DisparaAlteracaoCadastro(Object ContratoID)
        {
            try
            {
                this.DisparaEventoParaGeracaoArquivo(ContratoID, ContratoBeneficiario.eStatus.AlteracaoCadastroPendente);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Sinaliza o Beneficiário para o Arquivo de Exclusão de Cadastro.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        public void DisparaExclusaoCadastro(Object ContratoID)
        {
            try
            {
                this.DisparaEventoParaGeracaoArquivo(ContratoID, ContratoBeneficiario.eStatus.ExclusaoPendente);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Sinaliza o Beneficiário para o Arquivo de Segunda Via de Cartão Magnético.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        public void DisparaSegundaViaCartao(Object ContratoID)
        {
            try
            {
                this.DisparaEventoParaGeracaoArquivo(ContratoID, ContratoBeneficiario.eStatus.SegundaViaCartaoPendente);
            }
            catch (Exception) { throw; }
        }

        public static int CalculaIdade(DateTime dataNascimento)
        {
            return Convert.ToInt32(DateDiff(2, dataNascimento));
        }

        public static int CalculaIdade(DateTime dataNascimento, DateTime dataReferencia)
        {
            int anos = Convert.ToInt32(DateDiff(2, dataNascimento, dataReferencia));
            return anos;
            //int anos = dataReferencia.Year - dataNascimento.Year;

            //if (dataReferencia.Month < dataNascimento.Month || (dataReferencia.Month == dataNascimento.Month && dataReferencia.Day < dataNascimento.Day))
            //    anos--;

            //return anos;
        }

        public Beneficiario() { _dataCasamento = DateTime.MinValue; }
        public Beneficiario(Object id) : this() { _id = id; }

        public static IList<Beneficiario> CarregarTodos()
        {
            return null;
        }

        static void limpaCPF(ref String cpf)
        {
            if (!String.IsNullOrEmpty(cpf))
            {
                cpf = cpf.Replace("_", "");
                cpf = cpf.Replace(".", "");
                cpf = cpf.Replace("-", "");
            }
        }

        public static IList<Beneficiario> CarregarPorParametro(String nome, String cpf, String rg, SearchMatchType smtype)
        {
            limpaCPF(ref cpf);

            StringBuilder query = new StringBuilder();
            query.Append("TOP 100 beneficiario.*, (select top 1 id_telMail from mailing where id_beneficiario=beneficiario.beneficiario_id and concluido=0) as enriquecimentoId FROM beneficiario ");

            System.Collections.Hashtable parameterAndValues = new System.Collections.Hashtable();

            String startChar = "";
            String finisChar = "";
            String operatorChar = "";

            if (smtype == SearchMatchType.InicioDoCampo || smtype == SearchMatchType.QualquerParteDoCampo)
            {
                finisChar = "%";
                operatorChar = " LIKE ";
            }
            if (smtype == SearchMatchType.QualquerParteDoCampo)
            {
                startChar = "%";
            }
            else if (smtype == SearchMatchType.CampoInteiro)
            {
                operatorChar = "=";
            }

            StringBuilder whereAnd = new StringBuilder();
            if (!String.IsNullOrEmpty(nome))
            {
                whereAnd.Append(" WHERE beneficiario_nome");
                whereAnd.Append(operatorChar);
                whereAnd.Append("@beneficiario_nome");

                parameterAndValues.Add("@beneficiario_nome", startChar + nome + finisChar);
            }

            if (!String.IsNullOrEmpty(cpf))
            {
                if (whereAnd.Length > 0) { whereAnd.Append(" AND "); }
                else { whereAnd.Append(" WHERE "); }

                whereAnd.Append(" beneficiario_cpf = @beneficiario_cpf");
                parameterAndValues.Add("@beneficiario_cpf", cpf);
            }

            if (!String.IsNullOrEmpty(rg))
            {
                if (whereAnd.Length > 0) { whereAnd.Append(" AND "); }
                else { whereAnd.Append(" WHERE "); }

                whereAnd.Append(" beneficiario_rg = @beneficiario_rg");
                parameterAndValues.Add("@beneficiario_rg", rg);
            }

            query.Append(whereAnd.ToString());

            query.Append(" ORDER BY beneficiario_nome");

            if (parameterAndValues.Count > 0)
            {
                String[] _params = new String[parameterAndValues.Count];
                String[] _values = new String[parameterAndValues.Count];

                int i = 0;
                foreach (System.Collections.DictionaryEntry item in parameterAndValues)
                {
                    _params[i] = Convert.ToString(item.Key);
                    _values[i] = Convert.ToString(item.Value);
                    i++;
                }

                return LocatorHelper.Instance.ExecuteParametrizedQuery
                    <Beneficiario>(query.ToString(), _params, _values, typeof(Beneficiario));
            }
            else
            {
                return LocatorHelper.Instance.ExecuteQuery<Beneficiario>(query.ToString(), typeof(Beneficiario));
            }
        }

        /// <summary>
        /// Carrega um ou mais beneficiários com a mesma data de nascimento, nome, mas cpf DIFERENTE do informado
        /// </summary>
        public static IList<Beneficiario> CarregarPorParametro(DateTime nascimento, String nome, String cpf)
        {
            String qry = "* FROM beneficiario WHERE beneficiario_cpf = @cpf AND beneficiario_nome=@nome";

            if (nascimento != DateTime.MinValue)
            {
                qry = String.Concat(qry, " AND CONVERT(VARCHAR(20), beneficiario_dataNascimento, 103)='", nascimento.ToString("dd/MM/yyyy"), "'");
            }

            String[] _params = new String[2] { "@cpf", "@nome" };
            String[] _values = new String[2] { cpf, nome };

            return LocatorHelper.Instance.ExecuteParametrizedQuery
                    <Beneficiario>(qry, _params, _values, typeof(Beneficiario));
        }

        public static IList<Beneficiario> CarregarPorParametro(String cpf, DateTime nascimento)
        {
            return CarregarPorParametro(cpf, nascimento, null);
        }

        public static IList<Beneficiario> CarregarPorParametro(String cpf, DateTime nascimento, PersistenceManager pm)
        {
            limpaCPF(ref cpf);
            System.Collections.Hashtable parameterAndValues = new System.Collections.Hashtable();

            StringBuilder query = new StringBuilder();
            query.Append("* FROM beneficiario ");

            query.Append(" WHERE beneficiario_cpf=@beneficiario_cpf ");
            parameterAndValues.Add("@beneficiario_cpf", cpf);

            if (nascimento != DateTime.MinValue)
            {
                query.Append(" AND  CONVERT(VARCHAR(20), beneficiario_dataNascimento, 103)='");
                query.Append(nascimento.ToString("dd/MM/yyyy"));
                query.Append("'");
            }

            String[] _params = new String[parameterAndValues.Count];
            String[] _values = new String[parameterAndValues.Count];

            int i = 0;
            foreach (System.Collections.DictionaryEntry item in parameterAndValues)
            {
                _params[i] = Convert.ToString(item.Key);
                _values[i] = Convert.ToString(item.Value);
                i++;
            }

            return LocatorHelper.Instance.ExecuteParametrizedQuery
                    <Beneficiario>(query.ToString(), _params, _values, typeof(Beneficiario), pm);
        }

        public static IList<Beneficiario> CarregarPorParametro(String cpf, DateTime nascimento, String nomeMae, PersistenceManager pm)
        {
            limpaCPF(ref cpf);
            System.Collections.Hashtable parameterAndValues = new System.Collections.Hashtable();

            StringBuilder query = new StringBuilder();
            query.Append("* FROM beneficiario ");

            query.Append(" WHERE beneficiario_cpf=@beneficiario_cpf ");
            parameterAndValues.Add("@beneficiario_cpf", cpf);

            if (nascimento != DateTime.MinValue)
            {
                query.Append(" AND  CONVERT(VARCHAR(20), beneficiario_dataNascimento, 103)='");
                query.Append(nascimento.ToString("dd/MM/yyyy"));
                query.Append("'");
            }

            query.Append(" AND beneficiario_nomeMae=@beneficiario_nomeMae ");
            parameterAndValues.Add("@beneficiario_nomeMae", nomeMae);

            String[] _params = new String[parameterAndValues.Count];
            String[] _values = new String[parameterAndValues.Count];

            int i = 0;
            foreach (System.Collections.DictionaryEntry item in parameterAndValues)
            {
                _params[i] = Convert.ToString(item.Key);
                _values[i] = Convert.ToString(item.Value);
                i++;
            }

            return LocatorHelper.Instance.ExecuteParametrizedQuery
                    <Beneficiario>(query.ToString(), _params, _values, typeof(Beneficiario), pm);
        }

        public static Object CarregarPorParametro(String nome, String nomeMae, PersistenceManager pm, DateTime nascimento, String cpf)
        {
            System.Collections.Hashtable parameterAndValues = new System.Collections.Hashtable();

            StringBuilder query = new StringBuilder();
            query.Append("SELECT beneficiario_id FROM beneficiario ");

            query.Append(" WHERE beneficiario_nome=@beneficiario_nome ");
            parameterAndValues.Add("@beneficiario_nome", nome);

            query.Append(" AND beneficiario_nomeMae=@beneficiario_nomeMae ");
            parameterAndValues.Add("@beneficiario_nomeMae", nomeMae);

            query.Append(" AND beneficiario_cpf=@beneficiario_cpf ");
            parameterAndValues.Add("@beneficiario_cpf", cpf);

            if (nascimento.Year > 1753)
            {
                query.Append(" AND CONVERT(varchar(20), beneficiario_dataNascimento, 103)=@beneficiario_dataNascimento ");
                parameterAndValues.Add("@beneficiario_dataNascimento", nascimento.ToString("dd/MM/yyyy"));
            }

            String[] _params = new String[parameterAndValues.Count];
            String[] _values = new String[parameterAndValues.Count];

            int i = 0;
            foreach (System.Collections.DictionaryEntry item in parameterAndValues)
            {
                _params[i] = Convert.ToString(item.Key);
                _values[i] = Convert.ToString(item.Value);
                i++;
            }

            return LocatorHelper.Instance.ExecuteScalar(query.ToString(), _params, _values, pm);
        }

        public static Object CarregarPorParametro(Object importId, PersistenceManager pm)
        {
            return LocatorHelper.Instance.ExecuteScalar("SELECT beneficiario_id FROM beneficiario WHERE importId=" + importId, null, null, pm);
        }

        public static IList<Beneficiario> CarregarPorContratoId(Object contratoId)
        {
            return null;
        }

        public static IList<Beneficiario> CarregarPorOperadoraId(Object operadoraId, String nome, String cpf, String rg)
        {
            limpaCPF(ref cpf);

            StringBuilder query = new StringBuilder();
            query.Append("beneficiario.*, contrato_numero, contratobeneficiario_tipo FROM beneficiario INNER JOIN contrato_beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 INNER JOIN contrato ON contrato_id=contratobeneficiario_contratoId INNER JOIN plano ON plano_id = contrato_planoId INNER JOIN contratoadm ON plano_contratoId = contratoadm_id WHERE contratoadm_operadoraid= ");
            query.Append(operadoraId);

            System.Collections.Hashtable parameterAndValues = new System.Collections.Hashtable();
            StringBuilder whereAnd = new StringBuilder();

            if (!String.IsNullOrEmpty(nome))
            {
                query.Append(" AND beneficiario_nome LIKE @beneficiario_nome");
                parameterAndValues.Add("@beneficiario_nome", "%" + nome + "%");
            }

            if (!String.IsNullOrEmpty(cpf))
            {
                query.Append(" AND beneficiario_cpf = @beneficiario_cpf");
                parameterAndValues.Add("@beneficiario_cpf", cpf);
            }

            if (!String.IsNullOrEmpty(rg))
            {
                query.Append(" AND beneficiario_rg = @beneficiario_rg");
                parameterAndValues.Add("@beneficiario_rg", rg);
            }

            query.Append(" ORDER BY beneficiario_nome, contrato_numero");

            if (parameterAndValues.Count > 0)
            {
                String[] _params = new String[parameterAndValues.Count];
                String[] _values = new String[parameterAndValues.Count];

                int i = 0;
                foreach (System.Collections.DictionaryEntry item in parameterAndValues)
                {
                    _params[i] = Convert.ToString(item.Key);
                    _values[i] = Convert.ToString(item.Value);
                    i++;
                }

                return LocatorHelper.Instance.ExecuteParametrizedQuery
                    <Beneficiario>(query.ToString(), _params, _values, typeof(Beneficiario));
            }
            else
            {
                return LocatorHelper.Instance.ExecuteQuery<Beneficiario>(query.ToString(), typeof(Beneficiario));
            }
        }

        /// <summary>
        /// Checa se o cpf em questão está em uso. També faz a checagem quanto a validade do 
        /// cpf informado.
        /// </summary>
        public static Boolean ChecaCpf(Object beneficiarioId, String cpf)
        {
            limpaCPF(ref cpf);
            return ValidaCpf(cpf);
            //if (!ValidaCpf(cpf)) { return false; } else { return true; }

            //String query = "SELECT beneficiario_id FROM beneficiario WHERE beneficiario_cpf=@CPF";
            //if (beneficiarioId != null)
            //    query += " AND beneficiario_id <> " + beneficiarioId;

            //System.Data.DataTable dt = LocatorHelper.Instance.
            //    ExecuteParametrizedQuery(query, new String[] { "@CPF" }, new String[] { cpf }).Tables[0];

            //return dt == null || dt.Rows.Count == 0;
        }

        public static Boolean ChecaCpfEmUso(Object beneficiarioId, String cpf)
        {
            limpaCPF(ref cpf);
            if (!ValidaCpf(cpf)) { return false; }

            if (cpf == "99999999999") { return false; }

            String query = "SELECT beneficiario_id FROM beneficiario WHERE beneficiario_cpf=@CPF";
            if (beneficiarioId != null)
                query += " AND beneficiario_id <> " + beneficiarioId;

            System.Data.DataTable dt = LocatorHelper.Instance.
                ExecuteParametrizedQuery(query, new String[] { "@CPF" }, new String[] { cpf }).Tables[0];

            return dt == null || dt.Rows.Count == 0;
        }

        public static bool ValidaCpf(String vrCPF)
        {
            //if (System.Configuration.ConfigurationManager.AppSettings["naoValidaDocs"] != null &&
            //    System.Configuration.ConfigurationManager.AppSettings["naoValidaDocs"].ToUpper() == "Y")
            //{
            //    return true;
            //}

            string valor = vrCPF.Replace(".", "");
            valor = valor.Replace("-", "");
            valor = valor.Replace("_", "");

            if (valor.Length != 11)
                return false;

            if (valor == "99999999999") { return false; }

            bool igual = true;
            for (int i = 1; i < 11 && igual; i++)
                if (valor[i] != valor[0])
                    igual = false;

            if (igual || valor == "12345678909")
                return false;

            int[] numeros = new int[11];

            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(
                  valor[i].ToString());

            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];

            int resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }
            else if (numeros[9] != 11 - resultado)
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else
                if (numeros[10] != 11 - resultado)
                    return false;

            return true;
        }

        public static Object VerificaExistenciaCPF(String cpf)
        {
            limpaCPF(ref cpf);
            String query = "SELECT beneficiario_id FROM beneficiario WHERE beneficiario_cpf=@CPF";

            System.Data.DataTable dt = LocatorHelper.Instance.
                ExecuteParametrizedQuery(query, new String[] { "@CPF" }, new String[] { cpf }).Tables[0];

            Object beneficiarioId = new Object();

            if (dt != null && dt.Rows.Count > 0)
                beneficiarioId = dt.Rows[0].ItemArray[0];
            else
                beneficiarioId = null;

            return beneficiarioId;
        }

        /// <summary>
        /// Verifica a existência do Beneficiário por CPF e Data de Nascimento.
        /// </summary>
        /// <param name="cpf">CPF.</param>
        /// <param name="dataNascimento">Data de Nascimento.</param>
        /// <returns>Retorna o ID do Beneficiário.</returns>
        public static Object VerificaExistenciaCPF(String cpf, DateTime dataNascimento)
        {
            limpaCPF(ref cpf);
            String query = "SELECT beneficiario_id FROM beneficiario WHERE beneficiario_cpf=@CPF AND beneficiario_dataNascimento = @dataNascimento";

            System.Data.DataTable dt = LocatorHelper.Instance.
                ExecuteParametrizedQuery(query, new String[] { "@CPF", "@dataNascimento" }, new String[] { cpf, dataNascimento.ToString("yyyy-MM-dd") }).Tables[0];

            Object beneficiarioId = new Object();

            if (dt != null && dt.Rows.Count > 0)
                beneficiarioId = dt.Rows[0].ItemArray[0];
            else
                beneficiarioId = null;

            return beneficiarioId;
        }

        public static Object VerificaExistenciaCPF(String cpf, DateTime dataNascimento, String nomeMae, PersistenceManager pm)
        {
            limpaCPF(ref cpf);
            String query = "SELECT beneficiario_id FROM beneficiario WHERE beneficiario_cpf=@CPF AND beneficiario_dataNascimento = @dataNascimento AND beneficiario_nomeMae=@nomeMae";

            System.Data.DataTable dt = LocatorHelper.Instance.
                ExecuteParametrizedQuery(query, new String[] { "@CPF", "@dataNascimento", "@nomeMae" }, new String[] { cpf, dataNascimento.ToString("yyyy-MM-dd"), nomeMae }, pm).Tables[0];

            Object beneficiarioId = new Object();

            if (dt != null && dt.Rows.Count > 0)
                beneficiarioId = dt.Rows[0].ItemArray[0];
            else
                beneficiarioId = null;

            return beneficiarioId;
        }

        public Endereco Endereco
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Importar os Beneficiários de um MDB. A versão tem que ser 2003. No caso de Windows 7 colocar o 
        /// MDB em uma pasta que tenha permissão, não utilize o Desktop.
        /// </summary>
        /// <param name="MDBPath">Caminho do MDB.</param>
        public static void Importar(String MDBPath)
        {
            #region Table Header

            const String NomeColumn = "NOME";
            const String SexoColumn = "SEXO";
            const String CPFColumn = "CPF";
            const String RGColumn = "RG";
            const String DataNascimentoColumn = "NASCIMENTO";
            const String EmailColumn = "EMAIL";
            const String NomeMaeColumn = "NOMEMAE";
            const String DDD1Column = "DDD1";
            const String Telefone1Column = "TEL1";
            const String Ramal1Column = "RAMAL1";
            const String DDD2Column = "DDD2";
            const String Telefone2Column = "TEL2";
            const String Ramal2Column = "RAMAL2";
            const String CelDDDColumn = "CEL_DDD";
            const String CelColumn = "CEL";
            const String CelOperadoraColumn = "CEL_OPERADORA";
            const String TipoLogr1Column = "TIPO_LOGR1";
            const String Logr1Column = "LOGRADOURO1";
            const String NumLogr1Column = "NUM_LOGR1";
            const String ComplLogr1Column = "COMPL_LOGR1";
            const String Bairro1Column = "BAIRRO1";
            const String Cidade1Column = "CIDADE1";
            const String UF1Column = "UF1";
            const String CEP1Column = "CEP1";
            const String TipoEnd1Column = "TIPO_END1";
            const String TipoLogr2Column = "TIPO_LOGR2";
            const String Logr2Column = "LOGRADOURO2";
            const String NumLogr2Column = "NUM_LOGR2";
            const String ComplLogr2Column = "COMPL_LOGR2";
            const String Bairro2Column = "BAIRRO2";
            const String Cidade2Column = "CIDADE2";
            const String UF2Column = "UF2";
            const String CEP2Column = "CEP2";
            const String TipoEnd2Column = "TIPO_END2";

            #endregion

            String mdbFilePath = MDBPath;
            String connectionString = String.Concat("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=", mdbFilePath, ";User Id=Admin;");

            OleDbConnection connectionMDB = new OleDbConnection(connectionString);

            try
            {
                connectionMDB.Open();
            }
            catch (Exception) { throw; }

            OleDbCommand cmdMDB = connectionMDB.CreateCommand();

            cmdMDB.CommandType = CommandType.Text;
            cmdMDB.CommandText = "SELECT * FROM beneficiarios";

            OleDbDataReader drBeneficiario = cmdMDB.ExecuteReader();

            #region BeneficiarioImportVars

            String beneficiarioNome;
            String beneficiarioSexo;
            String beneficiarioCPF;
            String beneficiarioRG;
            String beneficiarioDataNascimento;
            String beneficiarioEmail;
            String beneficiarioNomeMae;
            String beneficiarioDDD1;
            String beneficiarioTelefone1;
            String beneficiarioRamal1;
            String beneficiarioDDD2;
            String beneficiarioTelefone2;
            String beneficiarioRamal2;
            String beneficiarioCelDDD;
            String beneficiarioCel;
            String beneficiarioCelOperadora;
            String beneficiarioTipoLogr1;
            String beneficiarioLogr1;
            String beneficiarioNumLogr1;
            String beneficiarioComplLogr1;
            String beneficiarioBairro1;
            String beneficiarioCidade1;
            String beneficiarioUF1;
            String beneficiarioCEP1;
            String beneficiarioTipoEnd1;
            String beneficiarioTipoLogr2;
            String beneficiarioLogr2;
            String beneficiarioNumLogr2;
            String beneficiarioComplLogr2;
            String beneficiarioBairro2;
            String beneficiarioCidade2;
            String beneficiarioUF2;
            String beneficiarioCEP2;
            String beneficiarioTipoEnd2;
            Beneficiario beneficiario = null;
            Endereco beneficiarioEndereco1 = null;
            Endereco beneficiarioEndereco2 = null;

            #endregion

            Int32 i = 0;

            PersistenceManager PMTransaction = new PersistenceManager();
            PMTransaction.BeginTransactionContext();

            while (drBeneficiario.HasRows && drBeneficiario.Read())
            {
                beneficiario = new Beneficiario();
                beneficiarioEndereco1 = new Endereco();
                beneficiarioEndereco2 = new Endereco();

                beneficiarioNome = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeColumn)).ToString() : null;
                beneficiarioSexo = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(SexoColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(SexoColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(SexoColumn)).ToString() : null;
                beneficiarioCPF = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CPFColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CPFColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CPFColumn)).ToString() : null;
                beneficiarioRG = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(RGColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(RGColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(RGColumn)).ToString() : null;
                beneficiarioDataNascimento = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DataNascimentoColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DataNascimentoColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DataNascimentoColumn)).ToString() : null;

                try
                {
                    beneficiarioEmail = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(EmailColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(EmailColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(EmailColumn)).ToString() : null;
                }
                catch (Exception) { beneficiarioEmail = String.Empty; }

                try
                {
                    beneficiarioNomeMae = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeMaeColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeMaeColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeMaeColumn)).ToString() : null;
                }
                catch (Exception) { beneficiarioNomeMae = String.Empty; }

                beneficiarioDDD1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD1Column)).ToString() : null;
                beneficiarioTelefone1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone1Column)).ToString() : null;
                beneficiarioRamal1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal1Column)).ToString() : null;
                beneficiarioDDD2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD2Column)).ToString() : null;
                beneficiarioTelefone2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone2Column)).ToString() : null;
                beneficiarioRamal2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal2Column)).ToString() : null;
                beneficiarioCelDDD = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelDDDColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelDDDColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelDDDColumn)).ToString() : null;
                beneficiarioCel = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelColumn)).ToString() : null;
                beneficiarioCelOperadora = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelOperadoraColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelOperadoraColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelOperadoraColumn)).ToString() : null;

                beneficiarioTipoLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr1Column)).ToString() : null;
                beneficiarioLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr1Column)).ToString() : null;
                beneficiarioNumLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr1Column)).ToString() : null;
                beneficiarioComplLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr1Column)).ToString() : null;
                beneficiarioBairro1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro1Column)).ToString() : null;
                beneficiarioCidade1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade1Column)).ToString() : null;
                beneficiarioUF1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF1Column)).ToString() : null;
                beneficiarioCEP1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP1Column)).ToString() : null;
                beneficiarioTipoEnd1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd1Column)).ToString() : null;

                beneficiarioTipoLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr2Column)).ToString() : null;
                beneficiarioLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr2Column)).ToString() : null;
                beneficiarioNumLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr2Column)).ToString() : null;
                beneficiarioComplLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr2Column)).ToString() : null;
                beneficiarioBairro2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro2Column)).ToString() : null;
                beneficiarioCidade2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade2Column)).ToString() : null;
                beneficiarioUF2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF2Column)).ToString() : null;
                beneficiarioCEP2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP2Column)).ToString() : null;
                beneficiarioTipoEnd2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd2Column)).ToString() : null;

                beneficiario.ID = Beneficiario.VerificaExistenciaCPF(beneficiarioCPF, Convert.ToDateTime(beneficiarioDataNascimento));

                beneficiario.Nome = beneficiarioNome;
                beneficiario.CPF = beneficiarioCPF;
                beneficiario.Sexo = (!String.IsNullOrEmpty(beneficiarioSexo)) ? (beneficiarioSexo.Equals("M")) ? "1" : "2" : null;
                beneficiario.RG = beneficiarioRG;
                beneficiario.DataNascimento = Convert.ToDateTime(beneficiarioDataNascimento);
                beneficiario.Email = beneficiarioEmail;
                beneficiario.NomeMae = beneficiarioNomeMae;

                if (!String.IsNullOrEmpty(beneficiarioDDD1) && !String.IsNullOrEmpty(beneficiarioTelefone1))
                {
                    beneficiario.Telefone = String.Concat("(", Convert.ToInt32(beneficiarioDDD1).ToString(), ") ", beneficiarioTelefone1);
                    beneficiario.Ramal = beneficiarioRamal1;
                }

                if (!String.IsNullOrEmpty(beneficiarioDDD2) && !String.IsNullOrEmpty(beneficiarioTelefone2))
                {
                    beneficiario.Telefone2 = String.Concat("(", Convert.ToInt32(beneficiarioDDD2).ToString(), ") ", beneficiarioTelefone2);
                    beneficiario.Ramal2 = beneficiarioRamal2;
                }

                if (!String.IsNullOrEmpty(beneficiarioCelDDD) && !String.IsNullOrEmpty(beneficiarioCel) && !String.IsNullOrEmpty(beneficiarioCelOperadora))
                {
                    beneficiario.Celular = String.Concat("(", Convert.ToInt32(beneficiarioCelDDD).ToString(), ") ", beneficiarioCel);
                    beneficiario.CelularOperadora = beneficiarioCelOperadora;
                }

                try
                {
                    PMTransaction.Save(beneficiario);
                }
                catch (Exception)
                {
                    PMTransaction.Rollback();
                    throw;
                }

                beneficiarioEndereco1.DonoId = beneficiario.ID;
                beneficiarioEndereco1.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                beneficiarioEndereco1.Logradouro = String.Concat(beneficiarioTipoLogr1.Replace(":", String.Empty), " ", beneficiarioLogr1);
                beneficiarioEndereco1.Numero = beneficiarioNumLogr1;
                beneficiarioEndereco1.Complemento = beneficiarioComplLogr1;
                beneficiarioEndereco1.Bairro = beneficiarioBairro1;
                beneficiarioEndereco1.Cidade = beneficiarioCidade1;
                beneficiarioEndereco1.UF = beneficiarioUF1;
                beneficiarioEndereco1.CEP = beneficiarioCEP1;
                beneficiarioEndereco1.Tipo = (!String.IsNullOrEmpty(beneficiarioTipoEnd1)) ? (beneficiarioTipoEnd1.Equals("RESIDENCIA")) ? (int)Endereco.TipoEndereco.Residencial : (int)Endereco.TipoEndereco.Comercial : 0; ;

                try
                {
                    beneficiarioEndereco1.Importar(PMTransaction);
                }
                catch (Exception)
                {
                    PMTransaction.Rollback();
                    throw;
                }

                if (!String.IsNullOrEmpty(beneficiarioLogr2))
                {
                    beneficiarioEndereco2.DonoId = beneficiario.ID;
                    beneficiarioEndereco2.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                    beneficiarioEndereco2.Logradouro = String.Concat(beneficiarioTipoLogr2.Replace(":", String.Empty), " ", beneficiarioLogr2);
                    beneficiarioEndereco2.Numero = beneficiarioNumLogr2;
                    beneficiarioEndereco2.Complemento = beneficiarioComplLogr2;
                    beneficiarioEndereco2.Bairro = beneficiarioBairro2;
                    beneficiarioEndereco2.Cidade = beneficiarioCidade2;
                    beneficiarioEndereco2.UF = beneficiarioUF2;
                    beneficiarioEndereco2.CEP = beneficiarioCEP2;
                    beneficiarioEndereco2.Tipo = (!String.IsNullOrEmpty(beneficiarioTipoEnd2)) ? (beneficiarioTipoEnd2.Equals("RESIDENCIA")) ? (int)Endereco.TipoEndereco.Residencial : (int)Endereco.TipoEndereco.Comercial : 0; ;

                    try
                    {
                        beneficiarioEndereco2.Importar(PMTransaction);
                    }
                    catch (Exception)
                    {
                        PMTransaction.Rollback();
                        throw;
                    }
                }

                i++;

                PMTransaction.Commit();
            }

            PMTransaction.Dispose();
            PMTransaction = null;

            drBeneficiario.Close();
            drBeneficiario.Dispose();
            drBeneficiario = null;

            cmdMDB.Dispose();
            cmdMDB = null;

            connectionMDB.Close();
            connectionMDB.Dispose();
            connectionMDB = null;
        }
    }

    [Serializable()]
    public abstract class EntityBase
    {
        protected EntityBase() { }

        protected static string _connString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;

        protected static readonly String DateFormat = "yyyy-MM-dd";
        protected static readonly String DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        protected void Salvar(IPersisteableEntity entity)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.Save(entity);
            pm = null;
        }

        public static DateTime ToDateTime(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
            {
                return DateTime.MinValue;
            }
            else
            {
                return Convert.ToDateTime(param, new System.Globalization.CultureInfo("pt-Br"));
            }
        }

        public static String CToString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return String.Empty;
            else
                return Convert.ToString(param);
        }

        public static Decimal CToDecimal(Object param)
        {
            if (param == null || param == DBNull.Value)
                return Decimal.Zero;
            else
                return Convert.ToDecimal(param, new System.Globalization.CultureInfo("pt-Br"));
        }

        protected void Remover(IPersisteableEntity entity)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.Remove(entity);
            pm = null;
        }

        protected void Carregar(IPersisteableEntity entity)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.Load(entity);
            pm = null;
        }

        protected String FormataTelefone(String fone)
        {
            if (String.IsNullOrEmpty(fone)) { return fone; }

            fone = fone.Replace("(", "").Replace(")", "").Replace(" ", ""); ;

            try
            {
                if (fone.Length == 10)
                {
                    return String.Format("{0:(##) ####-####}", Convert.ToDouble(fone));
                }
                else if (fone.Length == 8)
                {
                    return String.Format("{0: ####-####}", Convert.ToDouble(fone));
                }
                else
                    return fone;
            }
            catch
            {
                return String.Empty;
            }
        }

        protected String ToLower(String param)
        {
            if (param == null)
                return null;
            else
                return param.ToLower();
        }

        public static String GeraNumeroDeContrato(Int32 numero, Int32 qtdZerosEsquerda, String letra)
        {
            String _numero = Convert.ToString(numero);

            if (qtdZerosEsquerda > 0)
            {
                String mascara = new String('0', qtdZerosEsquerda);
                _numero = String.Format("{0:" + mascara + "}", numero);
            }

            if (!String.IsNullOrEmpty(letra))
                _numero = letra + _numero;

            return _numero;
        }

        public static String PrimeiraPosicaoELetra(String param)
        {
            if (String.IsNullOrEmpty(param)) { return ""; }

            String pos1 = param.Substring(0, 1);

            if (pos1 != "0" && pos1 != "1" && pos1 != "2" && pos1 != "3" && pos1 != "4" &&
                pos1 != "5" && pos1 != "6" && pos1 != "7" && pos1 != "8" && pos1 != "9")
            {
                return param.Substring(0, 1);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Método para Retirar a acentuação de um texto.
        /// </summary>
        /// <param name="Texto">Texto a ser modificado.</param>
        /// <returns>Texto sem acentuação.</returns>
        public static String RetiraAcentos(String Texto)
        {
            if (String.IsNullOrEmpty(Texto)) { return Texto; }
            String comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            String semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
                Texto = Texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());

            return Texto.Replace("'", "");
        }

        #region AppendPreparedField

        /// <summary>
        /// Inclui o Valor de um Campo de acordo com o seu tamanho. 
        /// </summary>
        /// <param name="SB">StringBuilder com as informações.</param>
        /// <param name="Value">Valor a ser Incluído.</param>
        /// <param name="ValueLength">Tamanho máximo do Valor a ser Incluído.</param>
        /// <returns>True se conseguiu incluir e False se não conseguir incluir.</returns>
        internal static Boolean AppendPreparedField(ref StringBuilder SB, Object Value, Int32 ValueLength)
        {
            if (SB != null && Value != null)
            {
                Value = EntityBase.RetiraAcentos(Value.ToString());

                if (Value.ToString().Length > ValueLength)
                    SB.Append(Value.ToString().Substring(0, ValueLength));
                else
                    SB.Append(Value.ToString().PadRight(ValueLength, ' '));

                return true;
            }

            return false;
        }

        #endregion

        internal static String Join(IList<String> list, String separator)
        {
            if (list == null) { return null; }

            StringBuilder sb = new StringBuilder();
            foreach (String item in list)
            {
                if (sb.Length > 0) { sb.Append(separator); }
                sb.Append(item);
            }
            return sb.ToString();
        }
    }

    [Serializable()]
    [DBTable("endereco")]
    public class Endereco : EntityBase, IPersisteableEntity
    {
        public enum TipoDono : int
        {
            Beneficiario,
            CorretorOuSupervisor,
            Operadora,
            Filial,
            Produtor
        }

        public enum TipoEndereco : int
        {
            Residencial,
            Comercial
        }

        Object _id;
        Object _donoId;
        Int32 _donoTipo;
        String _logradouro;
        String _numero;
        String _complemento;
        String _bairro;
        String _cidade;
        String _uf;
        String _cep;
        Int32 _tipo;

        #region Propriedades

        [DBFieldInfo("endereco_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("endereco_donoId", FieldType.Single)]
        public Object DonoId
        {
            get { return _donoId; }
            set { _donoId = value; }
        }

        [DBFieldInfo("endereco_donoTipo", FieldType.Single)]
        public Int32 DonoTipo
        {
            get { return _donoTipo; }
            set { _donoTipo = value; }
        }

        [DBFieldInfo("endereco_logradouro", FieldType.Single)]
        public String Logradouro
        {
            get { return _logradouro; }
            set { _logradouro = value; }
        }

        [DBFieldInfo("endereco_numero", FieldType.Single)]
        public String Numero
        {
            get { return _numero; }
            set { _numero = value; }
        }

        [DBFieldInfo("endereco_complemento", FieldType.Single)]
        public String Complemento
        {
            get { return _complemento; }
            set { _complemento = value; }
        }

        [DBFieldInfo("endereco_bairro", FieldType.Single)]
        public String Bairro
        {
            get { return _bairro; }
            set { _bairro = value; }
        }

        [DBFieldInfo("endereco_cidade", FieldType.Single)]
        public String Cidade
        {
            get { return _cidade; }
            set { _cidade = value; }
        }

        [DBFieldInfo("endereco_uf", FieldType.Single)]
        public String UF
        {
            get { return _uf; }
            set { _uf = value; }
        }

        [DBFieldInfo("endereco_cep", FieldType.Single)]
        public String CEP
        {
            get { return _cep; }
            set { _cep = value; }
        }

        [DBFieldInfo("endereco_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        #endregion

        public Endereco(Object id) : this() { _id = id; }
        public Endereco() { _tipo = (Int32)TipoEndereco.Residencial; }

        public void Salvar()
        {
            if (_cep != null) { _cep = _cep.Replace("-", ""); }
            base.Salvar(this);
        }

        /// <summary>
        /// Importar Endereço.
        /// </summary>
        public void Importar()
        {
            this.Importar(null);
        }

        /// <summary>
        /// Importar Endereço. Se duplicado por Completo não Importa.
        /// </summary>
        /// <param name="PM">Instância do Persistence Manager.</param>
        public void Importar(PersistenceManager PM)
        {
            if (PM == null) PM = new PersistenceManager();

            #region Parameters

            String[] Params = new String[10];
            String[] Values = new String[10];

            Params[0] = "@donoId";
            Params[1] = "@donoTipo";
            Params[2] = "@logradouro";
            Params[3] = "@numero";
            Params[4] = "@complemento";
            Params[5] = "@bairro";
            Params[6] = "@cidade";
            Params[7] = "@uf";
            Params[8] = "@cep";
            Params[9] = "@tipo";

            Values[0] = (this._donoId != null && this._donoId.ToString().Length > 0) ? this._donoId.ToString() : String.Empty;
            Values[1] = (this._donoTipo > -1) ? this._donoTipo.ToString() : "0";
            Values[2] = (!String.IsNullOrEmpty(this._logradouro)) ? this._logradouro : String.Empty;
            Values[3] = (!String.IsNullOrEmpty(this._numero)) ? this._numero : String.Empty;
            Values[4] = (!String.IsNullOrEmpty(this._complemento)) ? this._complemento : String.Empty;
            Values[5] = (!String.IsNullOrEmpty(this._bairro)) ? this._bairro : String.Empty;
            Values[6] = (!String.IsNullOrEmpty(this._cidade)) ? this._cidade : String.Empty;
            Values[7] = (!String.IsNullOrEmpty(this._uf)) ? this._uf : String.Empty;
            Values[8] = (!String.IsNullOrEmpty(this._cep)) ? this._cep : String.Empty;
            Values[9] = (this._tipo > -1) ? this._tipo.ToString() : "0";

            #endregion

            String strSQL = String.Concat("SELECT ",
                                          "      endereco_id ",
                                          "FROM endereco ",
                                          "WHERE endereco_donoId = @donoId AND endereco_donoTipo = @donoTipo AND endereco_logradouro = @logradouro AND endereco_numero = @numero AND ",
                                          "      endereco_complemento = @complemento AND endereco_bairro = @bairro AND endereco_cidade = @cidade AND ",
                                          "      endereco_uf = @uf AND endereco_cep = @cep AND endereco_tipo = @tipo");
            try
            {
                IList<Endereco> lstEndereco = LocatorHelper.Instance.ExecuteParametrizedQuery<Endereco>(strSQL, Params, Values, typeof(Endereco), PM);

                if (lstEndereco == null || lstEndereco.Count == 0)
                    PM.Save(this);
            }
            catch (Exception) { throw; }
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public void Carregar(PersistenceManager pm)
        {
            pm.Load(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public static IList<Endereco> CarregarPorDono(Object donoID, Endereco.TipoDono tipoDono)
        {
            return CarregarPorDono(donoID, tipoDono, null);
        }

        public static IList<Endereco> CarregarPorDono(Object donoID, Endereco.TipoDono tipoDono, PersistenceManager pm)
        {
            String query = "* FROM endereco WHERE endereco_donoid=" + donoID + " AND endereco_donotipo=" + Convert.ToInt32(tipoDono);

            IList<Endereco> lista = LocatorHelper.Instance.ExecuteQuery<Endereco>(query, typeof(Endereco), pm);

            return lista;
        }

        public static IList<Endereco> Carregar(ArrayList donoIDs)
        {
            String query = "* FROM endereco WHERE endereco_id IN (";

            String inClausule = "";

            foreach (Object id in donoIDs)
            {
                if (inClausule.Length > 0) { inClausule += ","; }
                inClausule += id;
            }

            query += inClausule + ")";

            IList<Endereco> lista = LocatorHelper.Instance.ExecuteQuery<Endereco>(query, typeof(Endereco));

            return lista;
        }
    }

    [Serializable]
    [DBTable("contrato_beneficiario")]
    public class ContratoBeneficiario : EntityBase, IPersisteableEntity
    {
        public class UI
        {
            //public static void FillDropdownWithStatus(System.Web.UI.WebControls.DropDownList cbo)
            //{
            //    cbo.Items.Clear();
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Novo", "0"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Pendente na operadora", "1"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Incluído", "2"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Devolvido", "3"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Alteração de cadastro pendente no sistema", "4"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Exclusão pendente no sistema", "5"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Segunda via de cartão pendente no sistema", "6"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Alteração de cadastro pendente na operadora", "7"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Exclusão pendente na operadora", "8"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Segunda via de cartão pendente na operadora", "9"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Excluído", "10"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Mudança de plano pendente no sistema", "11"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Mudança de plano pendente na operadora", "12"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Cancelamento de contrato pendente no sistema", "13"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Cancelamento de contrato pendente na operadora", "14"));
            //    //cbo.Items.Add(new System.Web.UI.WebControls.ListItem("", "15"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Alteração cadastral devolvida pela operadora", "16"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Exclusão de beneficiário devolvida pela operadora", "17"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Segunda via de cartão devolvida pela operadora", "18"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Mudança de plano devolvida pela operadora", "19"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Cancelamento de contrato devolvido pela operadora", "20"));
            //    //cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Contrato cancelado", "21"));
            //}
        }

        public enum TipoRelacao : int
        {
            Titular = 0,
            Dependente,
            Agregado
        }

        public enum eStatus : int
        {
            /// <summary>
            /// Novo beneficiário cadastrado. A operadora ainda NÃO foi notificada.
            /// </summary>
            Novo = 0,
            PendenteNaOperadora,
            Incluido, //acatado pela operadora
            /// <summary>
            /// Inclusão de contrato ou beneficiário devolvida pela Operadora.
            /// </summary>
            Devolvido,  //3
            /// <summary>
            /// Alteração de Dados Cadastrais pendente no Sistema (Fica disponivel na geração de arquivos transacionais).
            /// </summary>
            AlteracaoCadastroPendente, //4
            /// <summary>
            /// Exclusão de Beneficiário pendente no Sistema (Fica disponivel na geração de arquivos transacionais).
            /// </summary>
            ExclusaoPendente, //5
            /// <summary>
            /// Segunda Via de Cartão pendente no Sistema (Fica disponivel na geração de arquivos transacionais).
            /// </summary>
            SegundaViaCartaoPendente, //6
            /// <summary>
            /// Alteração de Dados Cadastrais pendente na Operadora (Necessita de processamento da Operadora).
            /// </summary>
            AlteracaoCadastroPendenteNaOperadora, //7
            /// <summary>
            /// Exclusão de Beneficiário pendente na Operadora (Necessita de processamento da Operadora).
            /// </summary>
            ExclusaoPendenteNaOperadora, //8
            /// <summary>
            /// Segunda Via de Cartão pendente na Operadora (Necessita de processamento da Operadora).
            /// </summary>
            SegundaViaCartaoPendenteNaOperadora, //9
            /// <summary>
            /// O beneficiário está cancelado na operadora e no sistema.
            /// </summary>
            Excluido, //10
            /// <summary>
            /// Mudança de Plano pendente no Sistema (Fica disponivel na geração de arquivos transacionais).
            /// </summary>
            MudancaPlanoPendente, //11
            /// <summary>
            /// Mudança de Plano pendente na Operadora (Necessita de processamento da Operadora).
            /// </summary>
            MudancaPlanoPendenteNaOperadora,
            /// <summary>
            /// Cancelamento de Contrato pendente no Sistema (Fica disponivel na geração de arquivos transacionais).
            /// </summary>
            CancelamentoPendente,
            /// <summary>
            /// Cancelamento de Contrato pendente na Operadora (Necessita de processamento da Operadora).
            /// </summary>
            CancelamentoPendenteNaOperadora, //14
            /// <summary>
            /// Desconhecido.
            /// </summary>
            Desconhecido,
            /// <summary>
            /// Alteração de dados cadastrais devolvida pela Operadora.
            /// </summary>
            AlteracaoCadastroDevolvido,
            /// <summary>
            /// Exclusao de beneficiario devolvida pela Operadora.
            /// </summary>
            ExclusaoDevolvido, //17
            /// <summary>
            /// Segunda via de cartão devolvida pela Operadora.
            /// </summary>
            SegundaViaCartaoDevolvido,
            /// <summary>
            /// Mudança de plano devolvida pela Operadora.
            /// </summary>
            MudancaDePlanoDevolvido,
            /// <summary>
            /// Cancelamento de contrato devolvido pela Operadora.
            /// </summary>
            CancelamentoDevolvido, //20
            /// <summary>
            /// Contrato Cancelado.
            /// </summary>
            Cancelado
        }

        #region fields

        Object _id;
        Object _contratoId;
        Object _beneficiarioId;
        Object _parentescoId;
        Object _estadoCivilId;
        int _tipo;
        DateTime _data;
        DateTime _vigencia;
        DateTime _dataInativacao;
        Boolean _ativo;
        Int32 _status;
        Int32 _numeroSequencial;

        //Object      _estadoCivil;
        DateTime _dataCasamento;
        Decimal _peso;
        Decimal _altura;

        String _carenciaOperadora;
        Object _carenciaOperadoraId;
        String _carenciaOperadoraDescricao;
        String _carenciaMatriculaNumero;
        DateTime _carenciaContratoDe;
        DateTime _carenciaContratoAte;
        Int32 _carenciaContratoTempo; //em meses.
        String _carenciaCodigo;
        bool _dmed;

        Decimal _valor;
        String _portabilidade;

        String _numeroMatriculaSaude;
        String _numeroMatriculaDental;

        String _beneficiarioNome;
        String _beneficiarioNomeMae;
        String _beneficiarioCpf;
        String _beneficiarioSexo;
        DateTime _beneficiarioNascimento;
        String _parentescoDescricao;
        String _parentescoCodigo;
        String _estadoCivilDescricao;
        String _estadoCivilCodigo;

        DateTime _beneficiarioDataNascimento;

        #endregion

        #region propriedades

        [DBFieldInfo("contratobeneficiario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("contratobeneficiario_contratoId", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId = value; }
        }

        [DBFieldInfo("contratobeneficiario_beneficiarioId", FieldType.Single)]
        public Object BeneficiarioID
        {
            get { return _beneficiarioId; }
            set { _beneficiarioId = value; }
        }

        [DBFieldInfo("contratobeneficiario_parentescoId", FieldType.Single)]
        public Object ParentescoID
        {
            get { return _parentescoId; }
            set { _parentescoId = value; }
        }

        [DBFieldInfo("contratobeneficiario_tipo", FieldType.Single)]
        public int Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        /// <summary>
        /// Data de admissão do beneficiário.
        /// </summary>
        [DBFieldInfo("contratobeneficiario_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data = value; }
        }

        [DBFieldInfo("contratobeneficiario_vigencia", FieldType.Single)]
        public DateTime Vigencia
        {
            get { return _vigencia; }
            set { _vigencia = value; }
        }

        [DBFieldInfo("contratobeneficiario_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo = value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaOperadoraOrigemId", FieldType.Single)]
        public Object CarenciaOperadoraID
        {
            get { return _carenciaOperadoraId; }
            set { _carenciaOperadoraId = value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaOperadoraId", FieldType.Single)]
        public String CarenciaOperadora
        {
            get { return _carenciaOperadora; }
            set { _carenciaOperadora = value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaOperadoraDescricao", FieldType.Single)]
        public String CarenciaOperadoraDescricao
        {
            get { return _carenciaOperadoraDescricao; }
            set { _carenciaOperadoraDescricao = value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaMatriculaNumero", FieldType.Single)]
        public String CarenciaMatriculaNumero
        {
            get { return _carenciaMatriculaNumero; }
            set { _carenciaMatriculaNumero = value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaContratoDataDe", FieldType.Single)]
        public DateTime CarenciaContratoDe
        {
            get { return _carenciaContratoDe; }
            set { _carenciaContratoDe = value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaContratoDataAte", FieldType.Single)]
        public DateTime CarenciaContratoAte
        {
            get { return _carenciaContratoAte; }
            set { _carenciaContratoAte = value; }
        }

        /// <summary>
        /// Tempo de contrato anterior (em meses)
        /// </summary>
        [DBFieldInfo("contratobeneficiario_carenciaContratoTempo", FieldType.Single)]
        public Int32 CarenciaContratoTempo
        {
            get { return _carenciaContratoTempo; }
            set { _carenciaContratoTempo = value; }
        }

        [DBFieldInfo("contratobeneficiario_carenciaCodigo", FieldType.Single)]
        public String CarenciaCodigo
        {
            get { return _carenciaCodigo; }
            set { _carenciaCodigo = value; }
        }

        [DBFieldInfo("contratobeneficiario_status", FieldType.Single)]
        public Int32 Status
        {
            get { return _status; }
            set { _status = value; }
        }

        [DBFieldInfo("contratobeneficiario_numeroSequencia", FieldType.Single)]
        public Int32 NumeroSequencial
        {
            get { return _numeroSequencial; }
            set { _numeroSequencial = value; }
        }

        [DBFieldInfo("contratobeneficiario_estadoCivilId", FieldType.Single)]
        public Object EstadoCivilID
        {
            get { return _estadoCivilId; }
            set { _estadoCivilId = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////////

        [DBFieldInfo("contratobeneficiario_dataCasamento", FieldType.Single)]
        public DateTime DataCasamento
        {
            get { return _dataCasamento; }
            set { _dataCasamento = value; }
        }

        [DBFieldInfo("contratobeneficiario_peso", FieldType.Single)]
        public Decimal Peso
        {
            get { return _peso; }
            set { _peso = value; }
        }

        [DBFieldInfo("contratobeneficiario_altura", FieldType.Single)]
        public Decimal Altura
        {
            get { return _altura; }
            set { _altura = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////////

        [DBFieldInfo("contratobeneficiario_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }

        [DBFieldInfo("contratobeneficiario_portabilidade", FieldType.Single)]
        public String Portabilidade
        {
            get { return _portabilidade; }
            set { _portabilidade = value; }
        }

        [DBFieldInfo("contratobeneficiario_numMatriculaSaude", FieldType.Single)]
        public String NumeroMatriculaSaude
        {
            get { return _numeroMatriculaSaude; }
            set { _numeroMatriculaSaude = value; }
        }

        [DBFieldInfo("contratobeneficiario_numMatriculaDental", FieldType.Single)]
        public String NumeroMatriculaDental
        {
            get { return _numeroMatriculaDental; }
            set { _numeroMatriculaDental = value; }
        }

        [DBFieldInfo("contratobeneficiario_dataInativo", FieldType.Single)]
        public DateTime DataInativacao
        {
            get { return _dataInativacao; }
            set { _dataInativacao = value; }
        }

        /// <summary>
        /// Joinned. Aprovado na DMED?
        /// </summary>
        [Joinned("beneficiario_dmed")]
        public bool DMED
        {
            get { return _dmed; }
            set { _dmed = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("beneficiario_nome")]
        public String BeneficiarioNome
        {
            get { return _beneficiarioNome; }
            set { _beneficiarioNome = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("beneficiario_nomeMae")]
        public String BeneficiarioNomeMae
        {
            get { return _beneficiarioNomeMae; }
            set { _beneficiarioNomeMae = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("beneficiario_cpf")]
        public String BeneficiarioCPF
        {
            get { return _beneficiarioCpf; }
            set { _beneficiarioCpf = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("beneficiario_sexo")]
        public String BeneficiarioSexo
        {
            get { return _beneficiarioSexo; }
            set { _beneficiarioSexo = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("beneficiario_dataNascimento")]
        public DateTime BeneficiarioNascimento
        {
            get { return _beneficiarioNascimento; }
            set { _beneficiarioNascimento = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("contratoAdmparentescoagregado_parentescoDescricao")]
        public String ParentescoDescricao
        {
            get { return _parentescoDescricao; }
            set { _parentescoDescricao = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("contratoAdmparentescoagregado_parentescoCodigo")]
        public String ParentescoCodigo
        {
            get { return _parentescoCodigo; }
            set { _parentescoCodigo = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("estadocivil_descricao")]
        public String EstadoCivilDescricao
        {
            get { return _estadoCivilDescricao; }
            set { _estadoCivilDescricao = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("estadocivil_codigo")]
        public String EstadoCivilCodigo
        {
            get { return _estadoCivilCodigo; }
            set { _estadoCivilCodigo = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("beneficiario_dataNascimento")]
        public DateTime BeneficiarioDataNascimento
        {
            get { return _beneficiarioDataNascimento; }
            set { _beneficiarioDataNascimento = value; }
        }

        #endregion

        //public ItemDeclaracaoSaudeINSTANCIA ItemDeclaracaoSaudeINSTANCIA
        //{
        //    get
        //    {
        //        throw new System.NotImplementedException();
        //    }
        //    set
        //    {
        //    }
        //}

        public ContratoBeneficiario() { _status = 0; _data = DateTime.Now; _ativo = true; _numeroSequencial = -1; _dmed = true; }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static ContratoBeneficiario CarregarPorContratoEBeneficiario(Object contratoId, Object beneficiarioId, PersistenceManager pm)
        {
            String query = String.Concat("contrato_beneficiario.*, beneficiario_nome, contratoAdmparentescoagregado_parentescoDescricao,beneficiario_dmed",
                " FROM contrato_beneficiario",
                " INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id",
                " LEFT JOIN contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId",
                " WHERE contratobeneficiario_contratoId=", contratoId, " AND contratobeneficiario_beneficiarioId=", beneficiarioId);

            IList<ContratoBeneficiario> ret = LocatorHelper.Instance.ExecuteQuery<ContratoBeneficiario>(query, typeof(ContratoBeneficiario), pm);
            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static IList<ContratoBeneficiario> CarregarPorContratoID(Object contratoId, Boolean semTitular)
        {
            return CarregarPorContratoID(contratoId, true, semTitular);
        }

        public static IList<ContratoBeneficiario> CarregarPorContratoID(Object contratoId, Boolean apenasAtivos, Boolean semTitular)
        {
            return CarregarPorContratoID(contratoId, apenasAtivos, semTitular, null);
        }

        public static IList<ContratoBeneficiario> CarregarPorContratoID(Object contratoId, Boolean apenasAtivos, Boolean semTitular, PersistenceManager pm)
        {
            String semTitularCondition = "";

            if (semTitular)
            {
                semTitularCondition = " AND contratobeneficiario_tipo <> " + Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular);
            }

            String apenasAtivosCondition = "";
            if (apenasAtivos) { apenasAtivosCondition = "contratobeneficiario_ativo=1 AND "; }

            String query = String.Concat("contrato_beneficiario.*, beneficiario_nome, beneficiario_nomeMae, beneficiario_cpf, beneficiario_sexo, beneficiario_dataNascimento, contratoAdmparentescoagregado_parentescoDescricao, contratoAdmparentescoagregado_parentescoCodigo, estadocivil_descricao,estadocivil_codigo,beneficiario_dmed ",
                " FROM contrato_beneficiario",
                " INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id",
                " LEFT JOIN contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId",
                " LEFT JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                " WHERE ", apenasAtivosCondition, " contratobeneficiario_contratoId=", contratoId, semTitularCondition,
                " ORDER BY contratobeneficiario_numeroSequencia");

            //query = String.Concat("SELECT contrato_beneficiario.*, beneficiario_nome, beneficiario_cpf, estadocivil_id,estadocivil_descricao,estadocivil_codigo ",
            //    " FROM contrato_beneficiario",
            //    "   INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
            //    "   LEFT JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
            //    " WHERE ",
            //    "   contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular), " AND contratobeneficiario_contratoId=", contratoId);

            return LocatorHelper.Instance.ExecuteQuery<ContratoBeneficiario>(query, typeof(ContratoBeneficiario), pm);
        }

        public static IList<ContratoBeneficiario> Carregar(string[] ids, PersistenceManager pm)
        {
            String query = String.Concat("contrato_beneficiario.*, beneficiario_nome, beneficiario_nomeMae, beneficiario_cpf, beneficiario_sexo, beneficiario_dataNascimento, contratoAdmparentescoagregado_parentescoDescricao, contratoAdmparentescoagregado_parentescoCodigo, estadocivil_descricao,estadocivil_codigo,beneficiario_dmed ",
                " FROM contrato_beneficiario",
                " INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id",
                " LEFT JOIN contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId",
                " LEFT JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                " WHERE contratobeneficiario_id in (", string.Join(",", ids), ") ",
                " ORDER BY contratobeneficiario_numeroSequencia");

            return LocatorHelper.Instance.ExecuteQuery<ContratoBeneficiario>(query, typeof(ContratoBeneficiario), pm);
        }

        public static IList<ContratoBeneficiario> CarregarPorContratoID_Parcial(Object contratoId, Boolean apenasAtivos, Boolean semTitular, PersistenceManager pm)
        {
            String semTitularCondition = "";

            if (semTitular)
            {
                semTitularCondition = " AND contratobeneficiario_tipo <> " + Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular);
            }

            String apenasAtivosCondition = "";
            if (apenasAtivos) { apenasAtivosCondition = "contratobeneficiario_ativo=1 AND "; }

            String query = String.Concat("contratobeneficiario_id,contratobeneficiario_beneficiarioId, contratobeneficiario_contratoId, contratobeneficiario_tipo, beneficiario_dataNascimento,contratobeneficiario_ativo,contratobeneficiario_dataInativo,contratobeneficiario_data,contratobeneficiario_vigencia,beneficiario_dmed ",
                " FROM contrato_beneficiario",
                " INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id",
                " LEFT JOIN contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId",
                " WHERE ", apenasAtivosCondition, " contratobeneficiario_contratoId=", contratoId, semTitularCondition,
                " ORDER BY contratobeneficiario_numeroSequencia");

            return LocatorHelper.Instance.ExecuteQuery<ContratoBeneficiario>(query, typeof(ContratoBeneficiario), pm);
        }

        /// <summary>
        /// ID do beneficiario titular
        /// </summary>
        public static Object CarregaTitularID(Object contratoId, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@contratoId" };
            String[] paramVl = new String[] { Convert.ToString(contratoId) };
            return LocatorHelper.Instance.ExecuteScalar("SELECT contratobeneficiario_beneficiarioid FROM contrato_beneficiario WHERE contratobeneficiario_contratoid=@contratoId AND contratobeneficiario_tipo=" + Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular).ToString(), paramNm, paramVl, pm);
        }
        /// <summary>
        /// ID do beneficiario titular
        /// </summary>
        //public static Object CarregaTitularID(String contratoNumero, Object operadoraId, PersistenceManager pm)
        //{
        //    Object contratoId = Contrato.CarregaContratoID(operadoraId, contratoNumero, pm);
        //    if (contratoId == null) { return null; }
        //    return CarregaTitularID(contratoId, pm);
        //}

        public static Object CarregaID(Object contratoId, Object beneficiarioId, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@contratoId", "@beneficiarioId" };
            String[] paramVl = new String[] { Convert.ToString(contratoId), Convert.ToString(beneficiarioId) };
            return LocatorHelper.Instance.ExecuteScalar("SELECT contratobeneficiario_id FROM contrato_beneficiario WHERE contratobeneficiario_contratoid=@contratoId AND contratobeneficiario_beneficiarioid=@beneficiarioId", paramNm, paramVl, pm);
        }

        /// <summary>
        /// ID do ContratoBeneficiario para o titular
        /// </summary>
        public static Object CarregaID_ParaTitular(Object contratoId, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@contratoId" };
            String[] paramVl = new String[] { Convert.ToString(contratoId) };
            return LocatorHelper.Instance.ExecuteScalar("SELECT contratobeneficiario_id FROM contrato_beneficiario WHERE contratobeneficiario_ativo=1 AND contratobeneficiario_contratoid=@contratoId AND contratobeneficiario_tipo=" + Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular).ToString(), paramNm, paramVl, pm);
        }

        /// <summary>
        /// 
        /// </summary>
        public static IList<ContratoBeneficiario> CarregarPorContratoNumero(String contratoNumero, Object operadoraId, Boolean somenteAtivos)
        {
            String[] paramNm = new String[] { "@Numero" };
            String[] paramVl = new String[] { contratoNumero };

            String ativoCond = "";
            if (somenteAtivos)
            {
                ativoCond = " AND contratobeneficiario_ativo=1 ";
            }

            String query = String.Concat("contrato_beneficiario.*, beneficiario_nome, beneficiario_cpf, contratoAdmparentescoagregado_parentescoDescricao",
                " FROM contrato_beneficiario",
                " INNER JOIN contrato ON contrato_id=contratobeneficiario_contratoId ", ativoCond,
                " INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id", ativoCond,
                " LEFT JOIN contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId",
                " WHERE contrato_numero=@Numero AND contrato_operadoraId=", operadoraId,
                " ORDER BY contrato_id, contratobeneficiario_numeroSequencia");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<ContratoBeneficiario>(query, paramNm, paramVl, typeof(ContratoBeneficiario));
        }

        public static ContratoBeneficiario CarregarTitularPorContratoNumero(String contratoNumero, String operadoraNome, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@Numero", "@OperadoraNome" };
            String[] paramVl = new String[] { contratoNumero, operadoraNome };

            String query = String.Concat("top 1 beneficiario_id,beneficiario_nome, beneficiario_cpf ",
                " FROM contrato_beneficiario",
                " INNER JOIN contrato ON contrato_id=contratobeneficiario_contratoId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                " INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                " LEFT JOIN contratoADM_parentesco_agregado ON contratoAdmparentescoagregado_id=contratobeneficiario_parentescoId",
                " WHERE contrato_numero=@Numero AND contrato_operadoraNome=@operadoraNome",
                " ORDER BY contratobeneficiario_numeroSequencia");

            IList<ContratoBeneficiario> ret = LocatorHelper.Instance.ExecuteParametrizedQuery<ContratoBeneficiario>(query, paramNm, paramVl, typeof(ContratoBeneficiario), pm);

            if (ret == null || ret.Count == 0)
                return null;
            else
                return ret[0];
        }

        public static ContratoBeneficiario CarregarTitular(Object contratoId, PersistenceManager pm)
        {
            String query = String.Concat("SELECT contrato_beneficiario.*, beneficiario_nome, beneficiario_cpf, beneficiario_dataNascimento, estadocivil_id,estadocivil_descricao,estadocivil_codigo,beneficiario_dmed ",
                " FROM contrato_beneficiario",
                "   INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
                "   LEFT JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                " WHERE ",
                "   contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular), " AND contratobeneficiario_contratoId=", contratoId);

            IList<ContratoBeneficiario> lista = LocatorHelper.Instance.ExecuteQuery<ContratoBeneficiario>(query, typeof(ContratoBeneficiario), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }

        public static ContratoBeneficiario CarregarPorIDContratoBeneficiario(Object id, PersistenceManager pm)
        {
            String query = String.Concat("SELECT contrato_beneficiario.*, beneficiario_nome, beneficiario_cpf, estadocivil_id,estadocivil_descricao,estadocivil_codigo,beneficiario_dmed ",
                " FROM contrato_beneficiario",
                "   INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
                "   LEFT JOIN estado_civil ON estadocivil_id=contratobeneficiario_estadoCivilId ",
                " WHERE ",
                "   contratobeneficiario_id=", id);

            IList<ContratoBeneficiario> lista = LocatorHelper.Instance.ExecuteQuery<ContratoBeneficiario>(query, typeof(ContratoBeneficiario), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }

        /// <summary>
        /// Método para pegar o CPF do Titular de contrato (proposta para cliente final).
        /// </summary>
        /// <param name="contratoId">ID do Contrato.</param>
        /// <returns>Retorna o CPF do Titular do Contrato.</returns>
        public static String GetCPFTitular(Object contratoId)
        {
            return GetCPFTitular(contratoId, null);
        }

        /// <summary>
        /// Método para pegar o CPF do Titular de contrato (proposta para cliente final).
        /// </summary>
        /// <param name="contratoId">ID do Contrato.</param>
        /// <returns>Retorna o CPF do Titular do Contrato.</returns>
        public static String GetCPFTitular(Object contratoId, PersistenceManager PM)
        {
            if (contratoId != null)
            {
                String[] strParam = new String[1];
                String[] strValue = new String[1];

                strParam[0] = "@contrato_id";
                strValue[0] = contratoId.ToString();

                String strSQL = String.Concat("SELECT ",
                                              "      Ben.beneficiario_cpf ",
                                              "  FROM contrato_beneficiario cBen ",
                                              "  INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id ",
                                              "  WHERE contratobeneficiario_tipo = 0 AND contratobeneficiario_contratoId = @contrato_id");

                Object retVal = null;

                if (PM == null) PM = new PersistenceManager();

                try
                {
                    retVal = LocatorHelper.Instance.ExecuteScalar(strSQL, strParam, strValue, PM);
                }
                catch (Exception) { throw; }

                if (retVal != null && !(retVal is DBNull))
                    return retVal.ToString().Trim();
                else
                    return null;
            }
            else
                throw new ArgumentNullException("O ID do contrato está nulo.");
        }

        public static String GetNomeTitular(Object contratoId, PersistenceManager PM)
        {
            if (contratoId != null)
            {
                String[] strParam = new String[1];
                String[] strValue = new String[1];

                strParam[0] = "@contrato_id";
                strValue[0] = contratoId.ToString();

                String strSQL = String.Concat("SELECT ",
                                              "      Ben.beneficiario_nome ",
                                              "  FROM contrato_beneficiario cBen ",
                                              "  INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id ",
                                              "  WHERE contratobeneficiario_tipo = 0 AND contratobeneficiario_contratoId = @contrato_id");
                Object retVal = null;

                try
                {
                    retVal = LocatorHelper.Instance.ExecuteScalar(strSQL, strParam, strValue, PM);
                }
                catch (Exception) { throw; }

                if (retVal != null && !(retVal is DBNull))
                    return retVal.ToString().Trim();
                else
                    return null;
            }
            else
                throw new ArgumentNullException("O ID do contrato está nulo.");
        }

        /// <summary>
        /// Retorna o próximo número sequencial para beneficiário de um contrato.
        /// </summary>
        /// <param name="contratoId">ID do contrato (proposta)</param>
        /// <returns>Próximo número sequencial para beneficiário de um contrato</returns>
        public static Int32 ProximoNumeroSequencial(Object contratoId, Object beneficiarioId, PersistenceManager pm)
        {
            Object ret = null;
            if (beneficiarioId != null)
            {
                ret = LocatorHelper.Instance.ExecuteScalar("SELECT contratobeneficiario_numeroSequencia FROM contrato_beneficiario WHERE contratobeneficiario_contratoId=" + contratoId + " AND contratobeneficiario_beneficiarioId=" + beneficiarioId, null, null, pm);
                if (ret != null)
                    return Convert.ToInt32(ret);
            }

            ret = LocatorHelper.Instance.ExecuteScalar("SELECT MAX(contratobeneficiario_numeroSequencia) FROM contrato_beneficiario WHERE contratobeneficiario_contratoId=" + contratoId, null, null, pm);

            if (ret == null)
                return 1;
            else
                return (Convert.ToInt32(ret) + 1);
        }

        /// <summary>
        /// Retorna o próximo Status de acordo com a Movimentação.
        /// </summary>
        /// <param name="Movimentacao">Inclusão de Beneficiário, Alteração de Dados Cadastrais, Mudança de Plano, etc.</param>
        /// <returns>Retorna o próximo Status do Workflow.</returns>
        //public static eStatus ProximoStatusPorMovimentacao(String Movimentacao)
        //{
        //    switch (Movimentacao)
        //    {
        //        case ArqTransacionalUnimed.Movimentacao.InclusaoBeneficiario:
        //            return eStatus.PendenteNaOperadora;
        //        case ArqTransacionalUnimed.Movimentacao.AlteracaoBeneficiario:
        //            return eStatus.AlteracaoCadastroPendenteNaOperadora;
        //        case ArqTransacionalUnimed.Movimentacao.ExclusaoBeneficiario:
        //            return eStatus.ExclusaoPendenteNaOperadora;
        //        case ArqTransacionalUnimed.Movimentacao.SegundaViaCartaoBeneficiario:
        //            return eStatus.SegundaViaCartaoPendenteNaOperadora;
        //        case ArqTransacionalUnimed.Movimentacao.MudancaDePlano:
        //            return eStatus.MudancaPlanoPendenteNaOperadora;
        //        case ArqTransacionalUnimed.Movimentacao.CancelamentoContrato:
        //            return eStatus.CancelamentoPendenteNaOperadora;
        //        default:
        //            return eStatus.Desconhecido;
        //    }
        //}

        /// <summary>
        /// Ao desfazer o envio de um lote da operadora, o beneficiário precisa reassumir seu status de pendência no sistema.
        /// Este método calcula esse status com base no status assumido quando enviado à operadora.
        /// </summary>
        internal static ContratoBeneficiario.eStatus StatusAntesDeDesfazerEnvio(ContratoBeneficiario.eStatus statusAtual)
        {
            switch (statusAtual)
            {
                case ContratoBeneficiario.eStatus.PendenteNaOperadora:
                    {
                        return ContratoBeneficiario.eStatus.Novo;
                    }
                case ContratoBeneficiario.eStatus.AlteracaoCadastroPendenteNaOperadora:
                    {
                        return ContratoBeneficiario.eStatus.AlteracaoCadastroPendente;
                    }
                case ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora:
                    {
                        return ContratoBeneficiario.eStatus.ExclusaoPendente;
                    }
                case ContratoBeneficiario.eStatus.SegundaViaCartaoPendenteNaOperadora:
                    {
                        return ContratoBeneficiario.eStatus.SegundaViaCartaoPendente;
                    }
                case ContratoBeneficiario.eStatus.MudancaPlanoPendenteNaOperadora:
                    {
                        return ContratoBeneficiario.eStatus.MudancaPlanoPendente;
                    }
                case ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora:
                    {
                        return ContratoBeneficiario.eStatus.CancelamentoPendente;
                    }
            }

            return ContratoBeneficiario.eStatus.Desconhecido;
        }

        internal static void SetaStatusDevolvidoParaContratoBeneficiario(ContratoBeneficiario.eStatus statusAtual, Object contratoId, Object beneficiarioId, PersistenceManager pm)
        {
            ContratoBeneficiario.eStatus novoStatus = ContratoBeneficiario.eStatus.Desconhecido;

            #region obtém próximo status

            switch (statusAtual)
            {
                case ContratoBeneficiario.eStatus.AlteracaoCadastroPendenteNaOperadora:
                    {
                        novoStatus = ContratoBeneficiario.eStatus.AlteracaoCadastroDevolvido;
                        break;
                    }
                case ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora:
                    {
                        novoStatus = ContratoBeneficiario.eStatus.CancelamentoDevolvido;
                        break;
                    }
                case ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora:
                    {
                        novoStatus = ContratoBeneficiario.eStatus.ExclusaoDevolvido;
                        break;
                    }
                case ContratoBeneficiario.eStatus.MudancaPlanoPendenteNaOperadora:
                    {
                        novoStatus = ContratoBeneficiario.eStatus.MudancaDePlanoDevolvido;
                        break;
                    }
                case ContratoBeneficiario.eStatus.PendenteNaOperadora:
                    {
                        novoStatus = ContratoBeneficiario.eStatus.Devolvido;
                        break;
                    }
                case ContratoBeneficiario.eStatus.SegundaViaCartaoPendenteNaOperadora:
                    {
                        novoStatus = ContratoBeneficiario.eStatus.SegundaViaCartaoDevolvido;
                        break;
                    }
            }
            #endregion

            if (novoStatus != ContratoBeneficiario.eStatus.Desconhecido)
            {
                ContratoBeneficiario.AlteraStatusBeneficiario(contratoId, beneficiarioId, novoStatus, pm);
                //ContratoStatusHistorico csh = new ContratoStatusHistorico();
                //csh.Data = DateTime.Now;
                //csh.OperadoraID = null;
                //csh.PropostaNumero = "";
                //csh.Status = ContratoStatusHistorico.eStatus.
            }
        }


        /// <summary>
        /// Método par aAlterar o Status de um Beneficiário dentro de um Contrato.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        /// <param name="BeneficiarioID">ID do Beneficiário.</param>
        /// <param name="Status">Status do Beneficiário.</param>
        public static void AlteraStatusBeneficiario(Object ContratoID, Object BeneficiarioID, eStatus Status)
        {
            AlteraStatusBeneficiario(ContratoID, BeneficiarioID, Status, new PersistenceManager());
        }

        /// <summary>
        /// Método par aAlterar o Status de um Beneficiário dentro de um Contrato.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        /// <param name="BeneficiarioID">ID do Beneficiário.</param>
        /// <param name="Status">Status do Beneficiário.</param>
        public static void AlteraStatusBeneficiario(Object ContratoID, Object BeneficiarioID, eStatus Status, PersistenceManager PM)
        {
            if (BeneficiarioID != null && ContratoID != null)
            {
                String[] strParam = new String[3];
                String[] strVaule = new String[3];

                strParam[0] = "@status";
                strParam[1] = "@contrato_id";
                strParam[2] = "@beneficiario_id";

                strVaule[0] = ((Int32)Status).ToString();
                strVaule[1] = ContratoID.ToString();
                strVaule[2] = BeneficiarioID.ToString();

                String strSQL = "UPDATE contrato_beneficiario SET contratobeneficiario_status = @status WHERE contratobeneficiario_contratoId = @contrato_id AND contratobeneficiario_beneficiarioId = @beneficiario_id";

                try
                {
                    if (PM == null) PM = new PersistenceManager();

                    LocatorHelper.Instance.ExecuteScalar(strSQL, strParam, strVaule, PM);
                }
                catch (Exception) { throw; }
            }
            else
                throw new ArgumentNullException("O ID do beneficiario ou do contrato não foi informado.");
        }

        public static void AlteraStatusBeneficiario(Object ContratoBeneficiarioID, eStatus Status, PersistenceManager PM)
        {
            String[] strParam = new String[2];
            String[] strVaule = new String[2];

            strParam[0] = "@status";
            strParam[1] = "@contratobeneficiario_id";

            strVaule[0] = ((Int32)Status).ToString();
            strVaule[1] = ContratoBeneficiarioID.ToString();

            String strSQL = "UPDATE contrato_beneficiario SET contratobeneficiario_status = @status WHERE contratobeneficiario_id = @contratobeneficiario_id";

            try
            {
                if (PM == null) PM = new PersistenceManager();

                NonQueryHelper.Instance.ExecuteNonQuery(strSQL, strParam, strVaule, PM);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Inativa o Beneficiário no contrato
        /// </summary>
        /// <param name="ContratoID">Id do contrato</param>
        /// <param name="BeneficiarioId">Id do beneficiário</param>
        public static void InativaBeneficiario(Object ContratoID, Object BeneficiarioId, PersistenceManager PM)
        {
            if (BeneficiarioId != null && ContratoID != null)
            {
                String[] strParam = new String[2];
                String[] strVaule = new String[2];

                strParam[0] = "@contrato_id";
                strParam[1] = "@beneficiario_id";

                strVaule[0] = ContratoID.ToString();
                strVaule[1] = BeneficiarioId.ToString();

                String strSQL = "UPDATE contrato_beneficiario SET contratobeneficiario_ativo = 0, contratobeneficiario_dataInativo = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE contratobeneficiario_contratoId = @contrato_id AND contratobeneficiario_beneficiarioId = @beneficiario_id ";

                try
                {
                    if (PM == null) PM = new PersistenceManager();

                    LocatorHelper.Instance.ExecuteScalar(strSQL, strParam, strVaule, PM);
                }
                catch (Exception) { throw; }
            }
            else
                throw new ArgumentNullException("O ID do beneficiario ou do contrato não foi informado.");
        }

    }

    [Serializable]
    [DBTable("contratoADM")]
    public class ContratoADM : EntityBase, IPersisteableEntity
    {
        #region fields

        Object _id;
        Object _operadoraId;
        Object _estipulanteId;
        Object _tabelaComissionamentoAtivaId;
        String _descricao;
        String _contratoSaude;
        String _contratoDental;
        String _numero;
        DateTime _data;
        Boolean _ativo;
        String _codFilial;
        String _codUnidade;
        String _codAdministradora;

        String _operadoraDescricao;
        String _estipulanteDescricao;
        Decimal _totalNormal;

        Object _contratoGrupoId;

        #endregion

        #region propriedades

        [DBFieldInfo("contratoadm_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("contratoadm_tabelaComissionamentoAtivaId", FieldType.Single)]
        public Object TabelaComissionamentoAtivaID
        {
            get { return _tabelaComissionamentoAtivaId; }
            set { _tabelaComissionamentoAtivaId = value; }
        }

        [DBFieldInfo("contratoadm_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao = value; }
        }

        [DBFieldInfo("contratoadm_contratoSaude", FieldType.Single)]
        public String ContratoSaude
        {
            get { return _contratoSaude; }
            set { _contratoSaude = value; }
        }

        [DBFieldInfo("contratoadm_contratoDental", FieldType.Single)]
        public String ContratoDental
        {
            get { return _contratoDental; }
            set { _contratoDental = value; }
        }

        [DBFieldInfo("contratoadm_numero", FieldType.Single)]
        public String Numero
        {
            get { return _numero; }
            set { _numero = value; }
        }

        [DBFieldInfo("contratoadm_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId = value; }
        }

        [DBFieldInfo("contratoadm_estipulanteId", FieldType.Single)]
        public Object EstipulanteID
        {
            get { return _estipulanteId; }
            set { _estipulanteId = value; }
        }

        [DBFieldInfo("contratoadm_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo = value; }
        }

        [DBFieldInfo("contratoadm_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data = value; }
        }

        [DBFieldInfo("contratoadm_codFilial", FieldType.Single)]
        public String CodFilial
        {
            get { return _codFilial; }
            set { _codFilial = value; }
        }

        [DBFieldInfo("contratoadm_codUnidade", FieldType.Single)]
        public String CodUnidade
        {
            get { return _codUnidade; }
            set { _codUnidade = value; }
        }

        [DBFieldInfo("contratoadm_codAdministradora", FieldType.Single)]
        public String CodAdministradora
        {
            get { return _codAdministradora; }
            set { _codAdministradora = value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraDescricao
        {
            get { return _operadoraDescricao; }
            set { _operadoraDescricao = value; }
        }

        [Joinned("estipulante_descricao")]
        public String EstipulanteDescricao
        {
            get { return _estipulanteDescricao; }
            set { _estipulanteDescricao = value; }
        }

        [Joinned("TotalNormal")]
        public Decimal TotalNormal
        {
            get { return _totalNormal; }
            set { _totalNormal = value; }
        }

        /// <summary>
        /// ID do grupo de comissionamento ao qual este contrato adm pertence.
        /// </summary>
        [Joinned("contratoadmgrupo_id")]
        public Object ContratoGrupoID
        {
            get { return _contratoGrupoId; }
            set { _contratoGrupoId = value; }
        }

        [Joinned("contratoadmgrupo_grupoId")]
        public Object ContratoGrupo_GrupoID
        {
            get { return _contratoGrupoId; }
            set { _contratoGrupoId = value; }
        }

        public String DescricaoCodigoSaudeDental
        {
            get
            {
                String value = _descricao;

                if (!String.IsNullOrEmpty(_contratoDental) || !String.IsNullOrEmpty(_contratoSaude))
                {
                    value = String.Concat(value, " (");

                    if (!String.IsNullOrEmpty(_contratoDental))
                        value = String.Concat(value, "Dental: ", _contratoDental);

                    if (!String.IsNullOrEmpty(_contratoSaude))
                    {
                        if (!String.IsNullOrEmpty(_contratoDental))
                            value = String.Concat(value, " - ");

                        value = String.Concat(value, "Saúde: ", _contratoSaude);
                    }

                    value = String.Concat(value, ")");
                }

                return value;
            }
        }

        #endregion


        public ContratoADM() { _ativo = true; _data = DateTime.Now; _totalNormal = Decimal.Zero; }
        public ContratoADM(Object id) : this() { this._id = id; }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }
        #endregion

        public static IList<ContratoADM> CarregarTodos()
        {
            String query = String.Concat("contratoADM.*, operadora_nome ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                //"INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "ORDER BY operadora_nome, contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> CarregarTodos(Object estipulanteId, Object operadoraId, Boolean somenteAtivos)
        {
            String _ativoCond = " and contratoadm_ativo=1 ";
            if (!somenteAtivos) { _ativoCond = ""; }

            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "WHERE contratoadm_operadoraId=",
                operadoraId, " AND contratoadm_estipulanteId=", estipulanteId, _ativoCond);

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> CarregarPorTabelaComissionamento(Object tabelaComissionamentoModeloId)
        {
            String query = String.Concat("contratoADM.*, operadora_nome,contratoadmgrupo_grupoId ",
                "   FROM contratoADM ",
                "       INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "       INNER JOIN contratoAdmGrupo ON contratoadmgrupo_contratoAdmId = contratoadm_id ",
                "   WHERE contratoadmgrupo_tabelaId=", tabelaComissionamentoModeloId,
                "   ORDER BY operadora_nome, contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        [Obsolete("Em desuso", true)]
        public static IList<ContratoADM> _CarregarTodos(Object grupoId)
        {
            String query = String.Concat("SUM(comissaomodeloitem_percentual) as TotalNormal, operadora_nome, contratoadm_id, contratoadm_tabelaComissionamentoAtivaId, contratoadm_tabelaReajusteAtualId, contratoadm_descricao,contratoadm_numero, contratoadm_operadoraId, contratoadm_estipulanteId, contratoadm_ativo, contratoadm_data ",
                "   FROM contratoADM ",
                "       INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "       LEFT JOIN comissao_modelo_item ON contratoadm_id=comissaomodeloitem_contratoid AND comissaomodeloitem_comissaomodeloid=", grupoId,
                "   GROUP BY operadora_nome, contratoadm_id, contratoadm_tabelaComissionamentoAtivaId, contratoadm_tabelaReajusteAtualId, contratoadm_descricao,contratoadm_numero, contratoadm_operadoraId, contratoadm_estipulanteId, contratoadm_ativo, contratoadm_data ",
                "   ORDER BY operadora_nome, contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static Object CarregarID(String numero, Object operadoraId, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@numero" };
            String[] paramVl = new String[] { numero };
            return LocatorHelper.Instance.ExecuteScalar("SELECT contratoadm_id FROM contratoADM WHERE contratoadm_numero=@numero AND contratoadm_operadoraId=" + operadoraId, paramNm, paramVl, pm);
        }

        public static Object CarregarID(String numero, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@numero" };
            String[] paramVl = new String[] { numero };
            return LocatorHelper.Instance.ExecuteScalar("SELECT contratoadm_id FROM contratoADM WHERE contratoadm_numero=@numero", paramNm, paramVl, pm);
        }

        public static ContratoADM Carregar(String numero, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@numero" };
            String[] paramVl = new String[] { numero };
            IList<ContratoADM> lista = LocatorHelper.Instance.ExecuteParametrizedQuery<ContratoADM>("SELECT TOP 1 * FROM contratoADM WHERE contratoadm_numero=@numero", paramNm, paramVl, typeof(ContratoADM));

            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }

        public static IList<ContratoADM> Carregar(Object operadoraId)
        {
            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "WHERE contratoadm_operadoraId=",
                operadoraId, " ORDER BY contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> Carregar(String[] operadoraIDs, String[] estipulantesIDs)
        {
            String estipulanteCond = "";
            if (estipulantesIDs != null && estipulantesIDs.Length > 0)
                estipulanteCond = String.Concat(" AND contratoadm_estipulanteId IN (", String.Join(",", estipulantesIDs), ") ");

            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "   INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "   INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                " WHERE ",
                //"   contratoadm_ativo=1 AND ",
                "   contratoadm_operadoraId IN(", String.Join(",", operadoraIDs), ") ", estipulanteCond,
                " ORDER BY contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> Carregar(Object operadoraId, Boolean ativo)
        {
            String _ativo = "1";
            if (!ativo) { _ativo = "0"; }

            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "WHERE contratoadm_operadoraId=",
                operadoraId, " AND contratoadm_ativo=", _ativo, " ORDER BY contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> Carregar(Object operadoraId, String param1, String param2, String param3, Boolean ativo)
        {
            String _ativo = "1";
            if (!ativo) { _ativo = "0"; }

            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "WHERE contratoadm_operadoraId=",
                operadoraId,
                " and contratoadm_descricao like '%", param1,
                "%' and contratoadm_descricao like '%", param2,
                "%' and contratoadm_descricao like '%", param3, "%' AND contratoadm_ativo=", _ativo, " ORDER BY contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> Carregar(Object estipulanteId, Object operadoraId)
        {
            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "WHERE contratoadm_operadoraId=",
                operadoraId, " AND contratoadm_estipulanteId=", estipulanteId);

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> Carregar(Object estipulanteId, Object operadoraId, Boolean ativo)
        {
            String _ativo = "1";
            if (!ativo) { _ativo = "0"; }

            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "WHERE contratoadm_operadoraId=",
                operadoraId, " AND contratoadm_estipulanteId=", estipulanteId, " AND contratoadm_ativo=", _ativo);

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static void SetaTabelaComissionamentoAutal(Object contratoId, Object tabelaComissaoId, PersistenceManager pm)
        {
            String command = "UPDATE contratoadm SET contratoadm_tabelaComissionamentoAtivaId=" + tabelaComissaoId + " WHERE contratoadm_id=" + contratoId;
            NonQueryHelper.Instance.ExecuteNonQuery(command, pm);
        }

        public static Boolean ExisteNumero(Object contratoId, String numero, Object estipulanteId, Object operadoraId)
        {
            String qry = "SELECT COUNT(*) FROM contratoADM WHERE contratoadm_numero='" + numero + "' AND contratoadm_estipulanteId=" + estipulanteId + " AND contratoadm_operadoraId=" + operadoraId;

            if (contratoId != null)
            {
                qry += " AND contratoadm_id <> " + contratoId;
            }

            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, null, null);
            if (ret == null || ret == DBNull.Value || Convert.ToInt32(ret) == 0)
                return false;
            else
                return true;
        }

        public static Boolean ExisteDescricao(Object contratoId, String descricao, Object operadoraId)
        {
            String qry = "SELECT COUNT(*) FROM contratoADM WHERE contratoadm_operadoraId=" + operadoraId + " AND contratoadm_descricao=@Descricao";

            if (contratoId != null)
            {
                qry += " AND contratoadm_id <> " + contratoId;
            }

            String[] paramnames = new String[] { "@Descricao" };
            String[] paramvalue = new String[] { descricao };

            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, paramnames, paramvalue);
            if (ret == null || ret == DBNull.Value || Convert.ToInt32(ret) == 0)
                return false;
            else
                return true;
        }

        public void SetaTabelaComissionamentoAutal(Object contratoId, PersistenceManager pm)
        {
            ContratoADM.SetaTabelaReajusteAutal(this._id, contratoId, pm);
        }

        public static void SetaTabelaReajusteAutal(Object contratoId, Object tabelaReajusteId, PersistenceManager pm)
        {
            String command = "UPDATE contratoadm SET contratoadm_tabelaReajusteAtualId=" + tabelaReajusteId + " WHERE contratoadm_id=" + contratoId;
            NonQueryHelper.Instance.ExecuteNonQuery(command, pm);
        }

        public void SetaTabelaReajusteAutal(Object contratoId, PersistenceManager pm)
        {
            ContratoADM.SetaTabelaReajusteAutal(this._id, contratoId, pm);
        }
    }

    [DBTable("contrato")]
    public class Contrato_ : EntityBase, IPersisteableEntity
    {
        public enum eTipoAcomodacao : int
        {
            quartoComun = 0,
            quartoParticular,
            indefinido
        }
        public enum eStatus : int
        {
            Normal,
            /// <summary>
            /// Proposta importada com o status 'Nao Implantada'.
            /// </summary>
            NaoImplantadoNaImportacao
        }

        #region fields

        Object _id;
        Object _filialId;
        Object _estipulanteId;
        Object _operadoraId;
        Object _contratoadmId;
        Object _planoId;
        Object _donoId; //corretor que vendeu a proposta
        Object _usuarioId;

        String _corretorTerceiroNome;
        String _corretorTerceiroCPF;
        String _superiorTerceiroNome;
        String _superiorTerceiroCPF;

        Object _operadorTmktId;
        Object _tipoContratoId;
        Object _enderecoReferenciaId;
        Object _enderecoCobrancaId;
        String _numero;
        Object _numeroId; //é o id da proposta, do impresso registrado no almoxarifado.
        String _vingencia;
        String _numeroMatricula;
        Decimal _valorAto;
        Boolean _adimplente;
        Boolean _cobrarTaxaAssociativa;
        DateTime _data;
        DateTime _dataCancelamento;
        String _emailCobranca;

        String _responsavelNome;
        String _responsavelCPF;
        String _responsavelRG;
        DateTime _responsavelDataNascimento;
        String _responsavelSexo;
        Object _responsavelParentescoId;
        Int32 _tipoAcomodacao;

        DateTime _admissao;
        DateTime _vigencia;
        DateTime _vencimento;

        String _empresaAnterior;
        String _empresaAnteriorMatricula;
        Int32 _empresaAnteriorTempo;
        Boolean _rascunho;
        Boolean _cancelado;
        Boolean _inativo;
        Boolean _pendente;
        String _obs;
        DateTime _alteracao;
        Int32 _codigoCobranca;
        Decimal _desconto;
        Int32 _status;

        bool _legado;

        String _planoDescricao;
        String _operadoraDescricao;
        String _beneficiarioTitularNome;
        String _beneficiarioTitularNomeMae;
        DateTime _beneficiarioTitularDataNascimento;
        String _beneficiarioTipo;
        String _beneficiarioCpf;

        Object _contratobeneficiario_beneficiarioId;

        String _empresaCobrancaNome;

        #endregion

        #region propriedades

        [DBFieldInfo("contrato_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("contrato_filialId", FieldType.Single)]
        public Object FilialID
        {
            get { return _filialId; }
            set { _filialId = value; }
        }

        [DBFieldInfo("contrato_estipulanteId", FieldType.Single)]
        public Object EstipulanteID
        {
            get { return _estipulanteId; }
            set { _estipulanteId = value; }
        }

        [DBFieldInfo("contrato_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId = value; }
        }

        [DBFieldInfo("contrato_contratoAdmId", FieldType.Single)]
        public Object ContratoADMID
        {
            get { return _contratoadmId; }
            set { _contratoadmId = value; }
        }

        [DBFieldInfo("contrato_planoId", FieldType.Single)]
        public Object PlanoID
        {
            get { return _planoId; }
            set { _planoId = value; }
        }

        [DBFieldInfo("contrato_legado", FieldType.Single)]
        public bool Legado
        {
            get { return _legado; }
            set { _legado = value; }
        }

        [DBFieldInfo("contrato_tipoContratoId", FieldType.Single)]
        public Object TipoContratoID
        {
            get { return _tipoContratoId; }
            set { _tipoContratoId = value; }
        }

        /// <summary>
        /// Corretor que vendeu a proposta
        /// </summary>
        [DBFieldInfo("contrato_donoId", FieldType.Single)]
        public Object DonoID
        {
            get { return _donoId; }
            set { _donoId = value; }
        }

        [DBFieldInfo("contrato_corretorTerceiroNome", FieldType.Single)]
        public String CorretorTerceiroNome
        {
            get { return _corretorTerceiroNome; }
            set { _corretorTerceiroNome = value; }
        }
        [DBFieldInfo("contrato_corretorTerceiroCPF", FieldType.Single)]
        public String CorretorTerceiroCPF
        {
            get { return _corretorTerceiroCPF; }
            set { _corretorTerceiroCPF = value; }
        }

        [DBFieldInfo("contrato_superiorTerceiroNome", FieldType.Single)]
        public String SuperiorTerceiroNome
        {
            get { return _superiorTerceiroNome; }
            set { _superiorTerceiroNome = value; }
        }
        [DBFieldInfo("contrato_superiorTerceiroCPF", FieldType.Single)]
        public String SuperiorTerceiroCPF
        {
            get { return _superiorTerceiroCPF; }
            set { _superiorTerceiroCPF = value; }
        }

        /// <summary>
        /// Operador de telemarketing que participou ou propiciou a venda.
        /// </summary>
        [DBFieldInfo("contrato_operadorTmktId", FieldType.Single)]
        public Object OperadorTmktID
        {
            get { return _operadorTmktId; }
            set { _operadorTmktId = value; }
        }

        [DBFieldInfo("contrato_enderecoReferenciaId", FieldType.Single)]
        public Object EnderecoReferenciaID
        {
            get { return _enderecoReferenciaId; }
            set { _enderecoReferenciaId = value; }
        }

        [DBFieldInfo("contrato_enderecoCobrancaId", FieldType.Single)]
        public Object EnderecoCobrancaID
        {
            get { return _enderecoCobrancaId; }
            set { _enderecoCobrancaId = value; }
        }

        /// <summary>
        /// Número do contrato.
        /// </summary>
        [DBFieldInfo("contrato_numero", FieldType.Single)]
        public String Numero
        {
            get { return _numero; }
            set { _numero = value; }
        }

        [DBFieldInfo("contrato_emailCobranca", FieldType.Single)]
        public String EmailCobranca
        {
            get { return _emailCobranca; }
            set { _emailCobranca = value; }
        }

        /// <summary>
        /// É o ID da proposta, do impresso registrado no almoxarifado.
        /// </summary>
        [DBFieldInfo("contrato_numeroId", FieldType.Single)]
        public Object NumeroID
        {
            get { return _numeroId; }
            set { _numeroId = value; }
        }

        [Obsolete("Em desuso.", true)]
        [DBFieldInfo("contrato_vingencia", FieldType.Single)]
        public String Vingencia
        {
            get { return _vingencia; }
            set { _vingencia = value; }
        }

        /// <summary>
        /// Número da matrícula.
        /// </summary>
        [DBFieldInfo("contrato_numeroMatricula", FieldType.Single)]
        public String NumeroMatricula
        {
            get { return _numeroMatricula; }
            set { _numeroMatricula = value; }
        }

        [DBFieldInfo("contrato_valorAto", FieldType.Single)]
        public Decimal ValorAto
        {
            get { return _valorAto; }
            set { _valorAto = value; }
        }

        /// <summary>
        /// Informa se o contrato está adimplente.
        /// </summary>
        [DBFieldInfo("contrato_adimplente", FieldType.Single)]
        public Boolean Adimplente
        {
            get { return _adimplente; }
            set { _adimplente = value; }
        }

        [DBFieldInfo("contrato_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Usuário que digitou a proposta.
        /// </summary>
        [DBFieldInfo("contrato_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId = value; }
        }

        [DBFieldInfo("contrato_dataCancelamento", FieldType.Single)]
        public DateTime DataCancelamento
        {
            get { return _dataCancelamento; }
            set { _dataCancelamento = value; }
        }

        [DBFieldInfo("contrato_responsavelNome", FieldType.Single)]
        public String ResponsavelNome
        {
            get { return _responsavelNome; }
            set { _responsavelNome = value; }
        }

        [DBFieldInfo("contrato_responsavelCPF", FieldType.Single)]
        public String ResponsavelCPF
        {
            get { return _responsavelCPF; }
            set { _responsavelCPF = value; }
        }

        [DBFieldInfo("contrato_responsavelRG", FieldType.Single)]
        public String ResponsavelRG
        {
            get { return _responsavelRG; }
            set { _responsavelRG = value; }
        }

        [DBFieldInfo("contrato_responsavelDataNascimento", FieldType.Single)]
        public DateTime ResponsavelDataNascimento
        {
            get { return _responsavelDataNascimento; }
            set { _responsavelDataNascimento = value; }
        }

        [DBFieldInfo("contrato_responsavelSexo", FieldType.Single)]
        public String ResponsavelSexo
        {
            get { return _responsavelSexo; }
            set { _responsavelSexo = value; }
        }

        [DBFieldInfo("contrato_responsavelParentescoId", FieldType.Single)]
        public Object ResponsavelParentescoID
        {
            get { return _responsavelParentescoId; }
            set { _responsavelParentescoId = value; }
        }

        [DBFieldInfo("contrato_tipoAcomodacao", FieldType.Single)]
        public Int32 TipoAcomodacao
        {
            get { return _tipoAcomodacao; }
            set { _tipoAcomodacao = value; }
        }

        [DBFieldInfo("contrato_admissao", FieldType.Single)]
        public DateTime Admissao
        {
            get { return _admissao; }
            set { _admissao = value; }
        }

        [DBFieldInfo("contrato_vigencia", FieldType.Single)]
        public DateTime Vigencia
        {
            get { return _vigencia; }
            set { _vigencia = value; }
        }

        [DBFieldInfo("contrato_vencimento", FieldType.Single)]
        public DateTime Vencimento
        {
            get { return _vencimento; }
            set { _vencimento = value; }
        }

        [DBFieldInfo("contrato_empresaAnterior", FieldType.Single)]
        public String EmpresaAnterior
        {
            get { return _empresaAnterior; }
            set { _empresaAnterior = value; }
        }

        [DBFieldInfo("contrato_empresaAnteriorMatricula", FieldType.Single)]
        public String EmpresaAnteriorMatricula
        {
            get { return _empresaAnteriorMatricula; }
            set { _empresaAnteriorMatricula = value; }
        }

        [DBFieldInfo("contrato_empresaAnteriorTempo", FieldType.Single)]
        public Int32 EmpresaAnteriorTempo
        {
            get { return _empresaAnteriorTempo; }
            set { _empresaAnteriorTempo = value; }
        }

        [DBFieldInfo("contrato_rascunho", FieldType.Single)]
        public Boolean Rascunho
        {
            get { return _rascunho; }
            set { _rascunho = value; }
        }

        [DBFieldInfo("contrato_cancelado", FieldType.Single)]
        public Boolean Cancelado
        {
            get { return _cancelado; }
            set { _cancelado = value; }
        }

        [DBFieldInfo("contrato_inativo", FieldType.Single)]
        public Boolean Inativo
        {
            get { return _inativo; }
            set { _inativo = value; }
        }

        /// <summary>
        /// Quando um novo contrato é cadastrado, e um ou mais beneficiários têm algum item de saúde marcado,
        /// a proposta fica pendente, dependendo de análise técnica.
        /// </summary>
        [DBFieldInfo("contrato_pendente", FieldType.Single)]
        public Boolean Pendente
        {
            get { return _pendente; }
            set { _pendente = value; }
        }

        [DBFieldInfo("contrato_cobrarTaxaAssociativa", FieldType.Single)]
        public Boolean CobrarTaxaAssociativa
        {
            get { return _cobrarTaxaAssociativa; }
            set { _cobrarTaxaAssociativa = value; }
        }

        [DBFieldInfo("contrato_obs", FieldType.Single)]
        public String Obs
        {
            get { return _obs; }
            set { _obs = value; }
        }

        [DBFieldInfo("contrato_codcobranca", FieldType.Single)]
        public Int32 CodCobranca
        {
            get { return _codigoCobranca; }
            set { _codigoCobranca = value; }
        }

        [DBFieldInfo("contrato_alteracao", FieldType.Single)]
        public DateTime Alteracao
        {
            get { return _alteracao; }
            set { _alteracao = value; }
        }

        [DBFieldInfo("contrato_desconto", FieldType.Single)]
        public Decimal Desconto
        {
            get { return _desconto; }
            set { _desconto = value; }
        }

        [DBFieldInfo("contrato_status", FieldType.Single)]
        public Int32 Status
        {
            get { return _status; }
            set { _status = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("plano_descricao")]
        public String PlanoDescricao
        {
            get { return _planoDescricao; }
            set { _planoDescricao = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("operadora_nome")]
        public String OperadoraDescricao
        {
            get { return _operadoraDescricao; }
            set { _operadoraDescricao = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("contratobeneficiario_beneficiarioId")]
        public Object ContratoBeneficiarioId
        {
            get { return _contratobeneficiario_beneficiarioId; }
            set { _contratobeneficiario_beneficiarioId = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("beneficiario_nome")]
        public String BeneficiarioTitularNome
        {
            get { return _beneficiarioTitularNome; }
            set { _beneficiarioTitularNome = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("beneficiario_nomeMae")]
        public String BeneficiarioTitularNomeMae
        {
            get { return _beneficiarioTitularNomeMae; }
            set { _beneficiarioTitularNomeMae = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("beneficiario_dataNascimento")]
        public DateTime BeneficiarioTitularDataNascimento
        {
            get { return _beneficiarioTitularDataNascimento; }
            set { _beneficiarioTitularDataNascimento = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("beneficiario_cpf")]
        public String BeneficiarioCPF
        {
            get { return _beneficiarioCpf; }
            set { _beneficiarioCpf = value; }
        }

        public String BeneficiarioCPFFormatado
        {
            get
            {
                if (String.IsNullOrEmpty(_beneficiarioCpf)) { return _beneficiarioCpf; }
                return String.Format(@"{0:000\.000\.000\-00}", Convert.ToInt64(_beneficiarioCpf));
            }
        }

        /// <summary>
        /// Se o beneficiário é titular ou dependente. Joinned
        /// </summary>
        [Joinned("contratobeneficiario_tipo")]
        public String TipoParticipacaoContrato
        {
            get { return _beneficiarioTipo; }
            set { _beneficiarioTipo = value; }
        }
        /// <summary>
        /// Joinned
        /// </summary>
        [Joinned("empresa_nome")]
        public String EmpresaCobranca
        {
            get { return _empresaCobrancaNome; }
            set { _empresaCobrancaNome = value; }
        }

        /// <summary>
        /// Condição para retornar, em uma query sql, apenas contratos não cancelados, ativos e que não sejam rascunhos.
        /// </summary>
        internal static String CondicaoBasicaQuery
        {
            get
            {
                return " contrato_cancelado <> 1 AND contrato_inativo <> 1 AND contrato_rascunho <> 1 "; //contrato_adimplente=1 AND 
            }
        }

        #endregion

        public Contrato_() { _legado = false;  _inativo = false; _cancelado = false; _adimplente = true; _pendente = false; _responsavelDataNascimento = DateTime.MinValue; _alteracao = DateTime.Now; _status = 0; }
        public Contrato_(Object id) : this() { _id = id; }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            String query = "delete from contrato_beneficiario WHERE contratobeneficiario_contratoId=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from contrato_valor where contratovalor_contratoId=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from contrato_plano_historico where contratoplano_contratoid=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from cobranca where cobranca_propostaId=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from contrato_beneficiario where contratobeneficiario_contratoid=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from adicional_beneficiario where adicionalbeneficiario_propostaid=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            pm.Remove(this);

            pm.CloseSingleCommandInstance();
            pm = null;
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public static Contrato CarregarParcial(Object id, PersistenceManager pm)
        {
            String qry = "contrato_id,contrato_numero, contrato_operadoraid, contrato_estipulanteId, contrato_cobrarTaxaAssociativa, contrato_contratoAdmId, contrato_admissao, contrato_vigencia,contrato_codcobranca,contrato_inativo,contrato_cancelado,contrato_dataCancelamento,contrato_responsavelNome,contrato_responsavelCPF FROM contrato WHERE contrato_id=" + id; ;
            IList<Contrato> ret = LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato), pm);
            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static Contrato CarregarParcial(String propostaNumero, Object operadoraId)
        {
            return CarregarParcial(propostaNumero, operadoraId, null);
        }
        public static Contrato CarregarParcial(String propostaNumero, Object operadoraId, PersistenceManager pm)
        {
            String qry = "contrato_id, contrato_numero, contrato_contratoAdmId,contrato_admissao,contrato_planoId,contrato_inativo,contrato_status FROM contrato WHERE contrato_numero=@numero AND contrato_operadoraId=" + operadoraId;

            String[] names = new String[1] { "@numero" };
            String[] value = new String[1] { propostaNumero };

            IList<Contrato> ret = LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, names, value, typeof(Contrato), pm);
            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static Contrato CarregarParcialPorCodCobranca(Object codCobranca, PersistenceManager pm)
        {
            IList<Contrato> lista = LocatorHelper.Instance.ExecuteQuery<Contrato>("contrato_id, contrato_operadoraId, contrato_numero, contrato_contratoAdmId, contrato_admissao, contrato_inativo, contrato_cancelado, contrato_adimplente, contrato_datacancelamento FROM contrato WHERE contrato_codCobranca=" + codCobranca, typeof(Contrato));

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        #endregion

        public static Contrato CarregarPorParametros(String numeroContrato, Object operadoraId, PersistenceManager pm)
        {
            String qry = String.Concat("SELECT * ",
                "   FROM contrato ",
                "   WHERE ",
                "       contrato_numero = @NumeroContrato AND ",
                "       contrato_operadoraId = ", operadoraId);

            String[] pName = new String[] { "@NumeroContrato" };
            String[] pValue = new String[] { numeroContrato };

            IList<Contrato> ret = LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, pName, pValue, typeof(Contrato), pm);

            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static IList<Contrato> CarregarPorParametros(String numeroContrato, Object operadoraId, DateTime vigencia, String titularCpf, String titularNome)
        {
            String qry = String.Concat("SELECT contrato.*, beneficiario_nome, beneficiario_cpf, beneficiario_nomeMae, beneficiario_dataNascimento ",
                "   FROM contrato ",
                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_contratoId=contrato_id AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "   WHERE ",
                "       contrato_operadoraId = ", operadoraId);

            List<String> paramNames = new List<String>();
            List<String> paramValues = new List<String>();

            String[] pName = null;
            String[] pValue = null;

            if (!String.IsNullOrEmpty(numeroContrato))
            {
                qry += " AND contrato_numero = @NumeroContrato ";
                paramNames.Add("@NumeroContrato");
                paramValues.Add(numeroContrato);
            }

            if (vigencia != DateTime.MinValue)
            {
                qry = String.Concat(qry, " AND DAY(contrato_vigencia)=", vigencia.Day, " AND MONTH(contrato_vigencia)=", vigencia.Month, " AND YEAR(contrato_vigencia)=", vigencia.Year);
            }

            if (!String.IsNullOrEmpty(titularCpf))
            {
                qry = String.Concat(qry, " AND beneficiario_cpf='", titularCpf, "'");
            }

            if (!String.IsNullOrEmpty(titularNome))
            {
                qry += " AND beneficiario_nome LIKE @TitularNome ";
                paramNames.Add("@TitularNome");
                paramValues.Add(titularNome + "%");
            }

            if (paramNames.Count > 0)
            {
                pName = paramNames.ToArray();
                pValue = paramValues.ToArray();
            }

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, pName, pValue, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorNumero(String numero, PersistenceManager pm)
        {
            String[] values = null;
            String[] pnames = null;

            pnames = new String[1] { "@contrato_numero" };
            values = new String[1] { numero };

            String query = String.Concat("contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE contrato_numero=@contrato_numero",
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato), pm);
        }

        public static IList<Contrato> CarregarPorParametros(String numero, String benficiarioNome)
        {
            String whereAnd = "";
            String[] values = null;
            String[] pnames = null;

            if (!String.IsNullOrEmpty(numero))
            {
                //numero = String.Format("{0:0000000000}", Convert.ToInt32(numero));
                whereAnd = " AND contrato_numero=@contrato_numero ";
                pnames = new String[2] { "@contrato_numero", "@beneficiario_nome" };
                values = new String[2] { numero, "%" + benficiarioNome + "%" };
            }
            else
            {
                pnames = new String[1] { "@beneficiario_nome" };
                values = new String[1] { "%" + benficiarioNome + "%" };
            }

            String query = String.Concat("contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorParametros(String numero, String benficiarioNome, String codCobranca)
        {
            String whereAnd = "";
            String[] pnames = new String[3] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca" };
            String[] values = { numero, "%" + benficiarioNome + "%", codCobranca };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd = " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            String query = String.Concat("contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorParametros(String numero, String benficiarioNome, String codCobranca, String protocolo)
        {
            String whereAnd = "";
            String joinAtendimento = "";
            String[] pnames = new String[4] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca", "@atendimento_id" };
            String[] values = { numero, "%" + benficiarioNome + "%", codCobranca, protocolo };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd = " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            if (!String.IsNullOrEmpty(protocolo))
            {
                whereAnd += " AND atendimento_id=@atendimento_id ";
                joinAtendimento = " INNER JOIN _atendimento on atendimento_propostaId=contrato_id ";
            }

            String query = String.Concat("TOP 50 contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static DataTable DTCarregarPorParametros(String numero, String benficiarioNome, String codCobranca, String protocolo)
        {
            return DTCarregarPorParametros(numero, benficiarioNome, codCobranca, protocolo, null, false, null, false);
        }

        public static DataTable DTCarregarPorParametros(String numero, String benficiarioNome, String codCobranca, String protocolo, Object empresaCobrancaId, Boolean somenteInativos, String cpf, Boolean somenteAtivos)
        {
            if (cpf == null) { cpf = ""; }

            String whereAnd = "";
            String joinAtendimento = "";
            String[] pnames = new String[5] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca", "@atendimento_id", "@beneficiario_cpf" };
            String[] values = { numero, "%" + benficiarioNome + "%", codCobranca, protocolo, cpf };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd = " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            if (!String.IsNullOrEmpty(protocolo))
            {
                whereAnd += " AND atendimento_id=@atendimento_id ";
                joinAtendimento = " INNER JOIN _atendimento on atendimento_propostaId=contrato_id ";
            }

            if (empresaCobrancaId != null)
            {
                whereAnd += " AND contrato_empresaCobrancaId= " + empresaCobrancaId;
            }

            if (somenteInativos)
            {
                whereAnd += " AND (contrato_cancelado=1 or contrato_inativo=1) ";
            }

            if (somenteAtivos)
            {
                whereAnd += " AND ( (contrato_cancelado=0 or contrato_cancelado is null ) and ( contrato_inativo=0 or contrato_inativo is null )) ";
            }

            if (!String.IsNullOrEmpty(cpf))
            {
                whereAnd += " and beneficiario_cpf=@beneficiario_cpf ";
            }

            String query = String.Concat("select TOP 60 contrato_id as ID, empresa_nome as EmpresaCobranca, contrato_numero as Numero, contrato_rascunho as Rascunho, contrato_cancelado as Cancelado, contrato_inativo as Inativo, plano_descricao as PlanoDescricao, operadora_nome as OperadoraNome, beneficiario_nome as BeneficiarioTitularNome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                " left join cobranca_empresa on contrato_empresaCobrancaId = empresa_id ",
                "WHERE (beneficiario_nome LIKE @beneficiario_nome) ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery(query, pnames, values).Tables[0];
        }

        /// <summary>
        /// Carrega o contrato e seus beneficiários (todos os beneficiários ativos, titular e dependentes)
        /// </summary>
        /// <param name="operadoraId">ID da operadora à qual pertence o contrato.</param>
        /// <param name="numeroContrato">Número do contrato.</param>
        /// <param name="cpf">TODO: Se fornecido um cpf, somente o beneficiário dono dele será carregado.</param>
        /// <returns></returns>
        public static IList<Contrato> Carregar(Object operadoraId, String numeroContrato, String cpf)
        {
            String qry = String.Concat("SELECT contrato.*, plano_descricao, operadora_nome, contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, beneficiario_nome, beneficiario_cpf ",
                "   FROM contrato ",
                "       INNER JOIN plano ON contrato_planoId=plano_id ",
                "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 ",
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "   WHERE ",
                "       contrato_numero=@NumeroContrato AND ",
                "       contrato_operadoraId=", operadoraId);

            String[] pName = new String[] { "@NumeroContrato" };
            String[] pValue = new String[] { numeroContrato };

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, pName, pValue, typeof(Contrato));
        }

        public static IList<Contrato> Carregar(Object contratoId)
        {
            String qry = String.Concat("SELECT contrato.*, plano_descricao, operadora_nome, contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, beneficiario_nome, beneficiario_cpf ",
                "   FROM contrato ",
                "       INNER JOIN plano ON contrato_planoId=plano_id ",
                "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 ",
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "   WHERE ",
                "       contrato_id=", contratoId);

            return LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorBeneficiário(Object beneficiarioId)
        {
            String query = String.Concat("contrato_id, contrato_numero, empresa_nome, operadora_nome, contrato_data, contratobeneficiario_tipo FROM contrato ",
                "INNER JOIN contrato_beneficiario ON contrato_id = contratobeneficiario_contratoId ",
                "INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id ",
                "INNER JOIN operadora ON contrato_operadoraId=operadora_id ",
                "LEFT JOIN cobranca_empresa ON contrato_empresaCobrancaId=empresa_id ",
                "WHERE beneficiario_id=", beneficiarioId, " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteQuery<Contrato>(query, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome)
        {
            return CarregarPorOperadoraID(operadoraId, numero, benficiarioNome, null);
        }

        public static IList<Contrato> CarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome, String codCobranca)
        {
            String whereAnd = "";
            if (codCobranca == null) { codCobranca = ""; }
            if (benficiarioNome == null) { benficiarioNome = ""; }
            String[] pnames = new String[3] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca" };
            String[] values = new String[3] { numero, "%" + benficiarioNome + "%", codCobranca };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd += " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            String query = String.Concat("TOP 100 contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome, String codCobranca, String protocolo)
        {
            String whereAnd = "";
            String joinAtendimento = "";
            if (benficiarioNome == null) { benficiarioNome = ""; }
            String[] pnames = new String[4] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca", "@atendimento_id" };
            String[] values = new String[4] { numero, "%" + benficiarioNome + "%", codCobranca, protocolo };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd += " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            if (!String.IsNullOrEmpty(protocolo))
            {
                whereAnd += " AND atendimento_id=@atendimento_id ";
                joinAtendimento = " INNER JOIN _atendimento on atendimento_propostaId=contrato_id ";
            }

            String query = String.Concat("TOP 60 contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static DataTable DTCarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome, String codCobranca, String protocolo)
        {
            return DTCarregarPorOperadoraID(operadoraId, numero, benficiarioNome, codCobranca, protocolo, null, false, null, false);
        }

        public static DataTable DTCarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome, String codCobranca, String protocolo, Object empresaCobrancaId, Boolean somenteInativos, String cpf, Boolean somenteAtivos)
        {
            if (cpf == null) { cpf = ""; }

            String whereAnd = "";
            String joinAtendimento = "";
            if (benficiarioNome == null) { benficiarioNome = ""; }
            String[] pnames = new String[5] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca", "@atendimento_id", "@beneficiario_cpf" };
            String[] values = new String[5] { numero, "%" + benficiarioNome + "%", codCobranca, protocolo, cpf };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd += " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            if (!String.IsNullOrEmpty(protocolo))
            {
                whereAnd += " AND atendimento_id=@atendimento_id ";
                joinAtendimento = " INNER JOIN _atendimento on atendimento_propostaId=contrato_id ";
            }

            if (empresaCobrancaId != null)
            {
                whereAnd += " AND contrato_empresaCobrancaId=" + empresaCobrancaId;
            }

            if (somenteInativos)
            {
                whereAnd += " AND (contrato_cancelado=1 or contrato_inativo=1) ";
            }

            if (somenteAtivos)
            {
                whereAnd += " AND ( (contrato_cancelado=0 or contrato_cancelado is null ) and ( contrato_inativo=0 or contrato_inativo is null )) ";
            }

            if (!String.IsNullOrEmpty(cpf))
            {
                whereAnd += " AND beneficiario_cpf=@beneficiario_cpf ";
            }

            String query = String.Concat("select TOP 60 contrato_id as ID, empresa_nome as EmpresaCobranca, contrato_numero as Numero, contrato_rascunho as Rascunho, contrato_cancelado as Cancelado, contrato_inativo as Inativo, plano_descricao as PlanoDescricao, operadora_nome as OperadoraNome, beneficiario_nome as BeneficiarioTitularNome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                " left join cobranca_empresa on contrato_empresaCobrancaId = empresa_id ",
                "WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery(query, pnames, values).Tables[0];
        }

        /// <summary>
        /// Carrega contratos inadimplentes levando em consideração cobranças em aberto, e não a propriedade Adimplente do objeto Contrato.
        /// </summary>
        /// <returns>Contratos inadimplentes.</returns>
        public static IList<Contrato> BuscarECarregarInadimplentes()
        {
            return BuscarECarregarInadimplentes_PORCOBRANCA(null);
        }
        /// <summary>
        /// Carrega contratos inadimplentes levando em consideração cobranças em aberto, e não a propriedade Adimplente do objeto Contrato.
        /// </summary>
        /// <param name="pm">Objeto PersistenceManager participante de uma transação.</param>
        /// <returns>Contratos inadimplentes.</returns>
        public static IList<Contrato> BuscarECarregarInadimplentes_PORCOBRANCA(PersistenceManager pm)
        {
            String qry = String.Concat("SELECT DISTINCT(cobranca_propostaId) AS ContratoID, contrato_id,contrato_estipulanteId,contrato_operadoraId,contrato_contratoAdmId,contrato_planoId,contrato_tipoContratoId,contrato_donoId,contrato_enderecoReferenciaId,contrato_enderecoCobrancaId,contrato_numero,contrato_numeroId,contrato_adimplente,contrato_cobrarTaxaAssociativa, contrato_tipoAcomodacao,contrato_admissao,contrato_vigencia,contrato_vencimento ",
                "FROM cobranca ",
                "   INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "WHERE ",
                "   contrato_cancelado <> 1 AND ",
                "   contrato_rascunho <> 1 AND ",
                "   cobranca_pago=0 AND ",
                "   cobranca_datavencimento IS NOT NULL AND ",
                " cobranca_datavencimento < GETDATE()");

            using (DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0])
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    List<Contrato> lista = new List<Contrato>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Contrato contrato = new Contrato(dr["contrato_id"]);
                        contrato.EstipulanteID = dr["contrato_estipulanteId"];
                        contrato.OperadoraID = dr["contrato_operadoraId"];
                        contrato.ContratoADMID = dr["contrato_contratoAdmId"];
                        contrato.PlanoID = dr["contrato_planoId"];
                        contrato.TipoContratoID = dr["contrato_tipoContratoId"];
                        contrato.DonoID = dr["contrato_donoId"];

                        lista.Add(contrato);
                    }

                    return lista;
                }
                else
                    return null;
            }

            // LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato), pm);
        }

        public static void SetaUsuarioLiberador(Object contratoId, Object usuarioId, PersistenceManager pm)
        {
            String command = String.Concat(
                "UPDATE contrato SET contrato_usuarioLiberador=",
                usuarioId, " WHERE contrato_id = ", contratoId);

            NonQueryHelper.Instance.ExecuteNonQuery(command, pm);
        }

        /// <summary>
        /// Checa se o número de contrato não está sendo usado por outro contrato.
        /// </summary>
        public static Boolean ContratoDisponivel(Object contratoId, Object operadoraId, String numero, ref String msgRetorno)
        {
            String query = "SELECT contrato_id FROM contrato WHERE contrato_rascunho=0 AND contrato_operadoraId=" + operadoraId + " AND contrato_numero=@numero ";

            if (contratoId != null)
                query += " AND contrato_id <> " + contratoId;

            DataTable dt = LocatorHelper.Instance.ExecuteParametrizedQuery(
                query, new String[] { "@numero" }, new String[] { numero }).Tables[0];

            Boolean valido = dt == null || dt.Rows.Count == 0;

            if (!valido)
            {
                msgRetorno = "Número de contrato já está em uso para essa operadora.";
            }

            return valido;
        }

        public static Boolean NumeroJaUtilizado(String numero, Object contratoId)
        {
            String query = "SELECT top 1 contrato_id FROM contrato WHERE contrato_numero=@numero ";

            if (contratoId != null)
                query += " AND contrato_id <> " + contratoId;

            DataTable dt = LocatorHelper.Instance.ExecuteParametrizedQuery(
                query, new String[] { "@numero" }, new String[] { numero }).Tables[0];

            Boolean valido = dt == null || dt.Rows.Count == 0;

            return valido;
        }

        public static Boolean NumeroDeContratoEmUso(String numero, String letra, Int32 zerosAEsquerda, Object operadoraId, PersistenceManager pm)
        {
            String _numero = EntityBase.GeraNumeroDeContrato(Convert.ToInt32(numero), zerosAEsquerda, letra);

            String qry = "SELECT contrato_id FROM contrato WHERE contrato_operadoraId=" + operadoraId + " AND contrato_numero=@NUM";
            IList<Contrato> ret = LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, new String[] { "@NUM" }, new String[] { _numero }, typeof(Contrato), pm);
            return ret != null;
        }

        public static Boolean CanceladoOuInativo(Object contratoId)
        {
            String qry = "SELECT contrato_id FROM contrato WHERE (contrato_inativo=1 OR contrato_cancelado=1) and contrato_id=" + contratoId;
            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, null, null, null);
            return ret != null;
        }

        /// <summary>
        /// Retorna o id do contrato administrativo de uma proposta.
        /// </summary>
        /// <param name="contratoId">ID da proposta (contrato com o segurado).</param>
        /// <param name="pm">Objeto PersistenceManager participante da transação, caso exista uma, ou null.</param>
        /// <returns>ID do contrato administrativo.</returns>
        public static Object CarregaContratoAdmID(Object contratoId, PersistenceManager pm)
        {
            String qry = "SELECT contrato_contratoAdmId FROM contrato WHERE contrato_id=" + contratoId;
            return LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
        }

        public static Object CarregaContratoID(Object operadoraId, String contratoNumero, PersistenceManager pm)
        {
            String qry = "SELECT contrato_id FROM contrato WHERE contrato_numero='" + contratoNumero + "' AND contrato_operadoraId=" + operadoraId;
            return LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
        }

        public static Boolean VerificaExistenciaBeneficiarioNoContrato(Object beneficiarioId, Object contratoId)
        {
            String[] pName = new String[0];
            String[] pValue = new String[0];

            String query = " SELECT contrato.* " +
                        " FROM contrato " +
                        " INNER JOIN contrato_beneficiario ON contrato_id = contratobeneficiario_contratoId " +
                        " WHERE contratobeneficiario_beneficiarioId = " + beneficiarioId + " AND contrato_id = " + contratoId + " AND contratobeneficiario_ativo = 1 ";

            DataTable dt = LocatorHelper.Instance.ExecuteParametrizedQuery(
                query, pName, pValue).Tables[0];

            Boolean valido = dt == null || dt.Rows.Count == 0;

            return valido;
        }

        public static void AlterarNumeroDeContrato(Object contratoId, String novoNumero, PersistenceManager pm)
        {
            String[] names = new String[1] { "@numero" };
            String[] value = new String[1] { novoNumero };

            NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_numero=@numero WHERE contrato_id=" + contratoId, names, value, pm);
        }

        public static void AlterarNumeroDeMatricula(Object contratoId, String novoNumero)
        {
            String[] names = new String[1] { "@numero" };
            String[] value = new String[1] { novoNumero };

            NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_numeroMatricula=@numero WHERE contrato_id=" + contratoId, names, value, null);
        }

    }

    [DBTable("cobranca")]
    public class Cobranca : EntityBase, IPersisteableEntity
    {
        public class UI
        {
            //public static void FillComboCarteira(System.Web.UI.WebControls.DropDownList cbo)
            //{
            //    cbo.Items.Clear();
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Unibanco", "0"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Itau sem registro", "1"));
            //}

            //public static void FillComboFormato(System.Web.UI.WebControls.DropDownList cbo)
            //{
            //    cbo.Items.Clear();
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Formato antigo", "0"));
            //    cbo.Items.Add(new System.Web.UI.WebControls.ListItem("Formato novo (Carnê)", "1"));
            //}
        }

        public static string ConvenioUBRASP = "2887755";

        public enum eTipoBanco : int
        {
            Unibanco,
            Itau,
            BancoDoBrasil
        }

        public enum eTipo : int
        {
            Normal,
            Complementar,
            Dupla,
            Indefinido,
            Parcelamento,
            DiferencaUbraSP,
            BoletoUbraspSP
        }

        public enum eCarteira : int
        {
            /// <summary>
            /// 0
            /// </summary>
            Unibanco,
            /// <summary>
            /// 1
            /// </summary>
            ItauSemRegistro
        }

        #region Fields

        Object _id;
        Object _propostaId;
        Decimal _valor;
        Decimal _valorNominal;
        DateTime _vencimento;
        DateTime _vencimentoIsencaoJuro;
        DateTime _dataCriacao;
        DateTime _dataBaixaAuto;
        Boolean _pago;
        DateTime _dataPagto;
        Decimal _valorPagto;
        Object _arquivoIdUltimoEnvio;
        Int32 _parcela;
        Int32 _tipo;
        Int32 _tipoTemp;
        Object _cobrancaRefId;
        Boolean _comissaoPaga;
        Boolean _cancelada;
        Boolean _dataVencimentoForcada;
        String _nossoNumero;

        Int32 _carteira;

        String _contratoCodCobranca;
        Object _operadoraId;
        String _contratoNumero;
        String _contratoTitularNome;
        string _contratoTitularCpf;
        Object _contratoEnderecoCobrancaId;
        String _filialNome;
        String _operadoraNome;
        String _estipulanteNome;
        Object _contratoBeneficiarioId;
        String _contratoBeneficiarioEmail;

        Object _composicaoBeneficiarioId;
        Int32 _composicaoTipo;
        Decimal _composicaoValor;
        String _composicaoResumo;

        Object _contratoAdmId;
        DateTime _contratoDataAdmissao;

        Object _headerParcId;
        Object _headerItemId;
        String _itemParcObs;

        Decimal _jurosRS;
        Decimal _multaRS;
        Decimal _amortizacao;

        //String _obsGerais;

        //ArquivoRemessaCriterio _criterio;
        #endregion

        #region Properties

        [DBFieldInfo("cobranca_id", FieldType.PrimaryKeyAndIdentity)]
        //[DBFieldInfo("cobranca_id", FieldType.Single)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("cobranca_propostaId", FieldType.Single)]
        public Object PropostaID
        {
            get { return _propostaId; }
            set { _propostaId = value; }
        }

        /// <summary>
        /// Quando cobrança dupla, esta propriedade guardará o ID da cobrança que compões esta cobrança.
        /// </summary>
        [DBFieldInfo("cobranca_cobrancaRefId", FieldType.Single)]
        public Object CobrancaRefID
        {
            get { return _cobrancaRefId; }
            set { _cobrancaRefId = value; }
        }

        [DBFieldInfo("cobranca_arquivoUltimoEnvioId", FieldType.Single)]
        public Object ArquivoIDUltimoEnvio
        {
            get { return _arquivoIdUltimoEnvio; }
            set { _arquivoIdUltimoEnvio = value; }
        }

        [DBFieldInfo("cobranca_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }

        /// <summary>
        /// Usado em cobranças duplas, qdo necessário, para guardar o valor original da cobrança antes de ser dupla
        /// </summary>
        [DBFieldInfo("cobranca_valorNominal", FieldType.Single)]
        public Decimal ValorNominal
        {
            get { return _valorNominal; }
            set { _valorNominal = value; }
        }

        [DBFieldInfo("cobranca_dataVencimentoIsencaoJuro", FieldType.Single)]
        public DateTime DataVencimentoISENCAOJURO
        {
            get { return _vencimentoIsencaoJuro; }
            set { _vencimentoIsencaoJuro = value; }
        }

        [DBFieldInfo("cobranca_dataVencimento", FieldType.Single)]
        public DateTime DataVencimento
        {
            get { return _vencimento; }
            set { _vencimento = value; }
        }

        [DBFieldInfo("cobranca_dataCriacao", FieldType.Single)]
        public DateTime DataCriacao
        {
            get { return _dataCriacao; }
            set { _dataCriacao = value; }
        }

        [DBFieldInfo("cobranca_dataBaixaAuto", FieldType.Single)]
        public DateTime DataBaixaAutomatica
        {
            get { return _dataBaixaAuto; }
            set { _dataBaixaAuto = value; }
        }

        [DBFieldInfo("cobranca_pago", FieldType.Single)]
        public Boolean Pago
        {
            get { return _pago; }
            set { _pago = value; }
        }

        [DBFieldInfo("cobranca_dataPagto", FieldType.Single)]
        public DateTime DataPgto
        {
            get { return _dataPagto; }
            set { _dataPagto = value; }
        }

        [DBFieldInfo("cobranca_valorPagto", FieldType.Single)]
        public Decimal ValorPgto
        {
            get { return _valorPagto; }
            set { _valorPagto = value; }
        }

        [DBFieldInfo("cobranca_parcela", FieldType.Single)]
        public Int32 Parcela
        {
            get { return _parcela; }
            set { _parcela = value; }
        }

        [DBFieldInfo("cobranca_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        [DBFieldInfo("cobranca_tipoTemp", FieldType.Single)]
        public Int32 TipoTemp
        {
            get { return _tipoTemp; }
            set { _tipoTemp = value; }
        }

        /// <summary>
        /// Indica se a comissao sobre esta cobrança já foi paga.
        /// </summary>
        [DBFieldInfo("cobranca_comissaoPaga", FieldType.Single)]
        public Boolean ComissaoPaga
        {
            get { return _comissaoPaga; }
            set { _comissaoPaga = value; }
        }

        [DBFieldInfo("cobranca_cancelada", FieldType.Single)]
        public Boolean Cancelada
        {
            get { return _cancelada; }
            set { _cancelada = value; }
        }

        [DBFieldInfo("cobranca_dataVencimentoForcada", FieldType.Single)]
        public Boolean DataVencimentoForcada
        {
            get { return _dataVencimentoForcada; }
            set { _dataVencimentoForcada = value; }
        }

        [DBFieldInfo("cobranca_nossoNumero", FieldType.Single)]
        public String NossoNumero
        {
            get { return _nossoNumero; }
            set { _nossoNumero = value; }
        }

        [DBFieldInfo("cobranca_carteira", FieldType.Single)]
        public Int32 Carteira
        {
            get { return _carteira; }
            set { _carteira = value; }
        }

        [Joinned("contrato_codcobranca")]
        public String ContratoCodCobranca
        {
            get { return _contratoCodCobranca; }
            set { _contratoCodCobranca = value; }
        }

        [Joinned("operadora_id")]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId = value; }
        }

        /// <summary>
        /// O nome do titular da proposta.
        /// </summary>
        [Joinned("beneficiario_nome")]
        public String ContratoTitularNome
        {
            get { return _contratoTitularNome; }
            set { _contratoTitularNome = value; }
        }

        [Joinned("beneficiario_cpf")]
        public String ContratoTitularCPF
        {
            get { return _contratoTitularCpf; }
            set { _contratoTitularCpf = value; }
        }

        /// <summary>
        /// O número da proposta, do contrato impresso.
        /// </summary>
        [Joinned("contrato_numero")]
        public String ContratoNumero
        {
            get { return _contratoNumero; }
            set { _contratoNumero = value; }
        }

        [Joinned("contrato_enderecoCobrancaId")]
        public Object ContratoEnderecoCobrancaID
        {
            get { return _contratoEnderecoCobrancaId; }
            set { _contratoEnderecoCobrancaId = value; }
        }

        [Joinned("filial_nome")]
        public String FilialNome
        {
            get { return _filialNome; }
            set { _filialNome = value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraNome
        {
            get { return _operadoraNome; }
            set { _operadoraNome = value; }
        }

        [Joinned("estipulante_descricao")]
        public String EstipulanteNome
        {
            get { return _estipulanteNome; }
            set { _estipulanteNome = value; }
        }

        [Joinned("beneficiario_id")]
        public Object BeneficiarioId
        {
            get { return _contratoBeneficiarioId; }
            set { _contratoBeneficiarioId = value; }
        }

        [Joinned("beneficiario_email")]
        public String BeneficiarioEmail
        {
            get { return _contratoBeneficiarioEmail; }
            set { _contratoBeneficiarioEmail = value; }
        }

        [Joinned("cobrancacomp_beneficiarioId")]
        public Object ComposicaoBeneficiarioID
        {
            get { return _composicaoBeneficiarioId; }
            set { _composicaoBeneficiarioId = value; }
        }

        [Joinned("cobrancacomp_tipo")]
        public Int32 ComposicaoTipo
        {
            get { return _composicaoTipo; }
            set { _composicaoTipo = value; }
        }

        [Joinned("cobrancacomp_valor")]
        public Decimal ComposicaoValor
        {
            get { return _composicaoValor; }
            set { _composicaoValor = value; }
        }

        [Joinned("contratoadm_id")]
        public Object ContratoAdmID
        {
            get { return _contratoAdmId; }
            set { _contratoAdmId = value; }
        }

        [Joinned("contrato_admissao")]
        public DateTime ContratoDataAdmissao
        {
            get { return _contratoDataAdmissao; }
            set { _contratoDataAdmissao = value; }
        }

        /// <summary>
        /// Para parcelas originais.
        /// </summary>
        [Joinned("parccob_headerId")]
        public Object HeaderParcID
        {
            get { return _headerParcId; }
            set { _headerParcId = value; }
        }

        /// <summary>
        /// Para parcelas geradas para o parcelamento.
        /// </summary>
        [Joinned("parcitem_headerId")]
        public Object HeaderItemID
        {
            get { return _headerItemId; }
            set { _headerItemId = value; }
        }

        /// <summary>
        /// Observação cadastrada durante a criação da parcela de negociação
        /// </summary>
        [Joinned("parcitem_obs")]
        public String ItemParcelamentoOBS
        {
            get { return _itemParcObs; }
            set { _itemParcObs = value; }
        }

        public String ComposicaoResumo
        {
            get { return _composicaoResumo; }
        }

        public String STRNossoNumero
        {
            get
            {
                if (this._dataCriacao.Year >= 2013)
                    return geraNossoNumeroItau();
                else
                    return geraNossoNumeroUnibanco();
            }
        }

        public String strPago
        {
            get { if (_pago) { return "Sim"; } else { return "Não"; } }
        }

        public String strDataPago
        {
            get
            {
                if (_dataPagto != DateTime.MinValue)
                    return _dataPagto.ToString("dd/MM/yyyy");
                else
                    return "";
            }
        }

        public String strEnviado
        {
            get { if (_arquivoIdUltimoEnvio == null) { return "Não"; } else { return "Sim"; } }
        }

        public static Boolean NossoNumeroITAU
        {
            get
            {
                String tipo =  ConfigurationManager.AppSettings["tipoNossoNumero"];
                if (String.IsNullOrEmpty(tipo)) { return false; }
                if (tipo.ToLower() == "itau")
                    return true;
                else
                    return false;
            }
        }

        //public ArquivoRemessaCriterio Criterio
        //{
        //    get { return _criterio; }
        //    set { _criterio = value; }
        //}

        /// <summary>
        /// Juros incididos sobre a parcela, em R$.
        /// </summary>
        public Decimal JurosRS
        {
            get { return _jurosRS; }
            set { _jurosRS = value; }
        }

        /// <summary>
        /// Multa sobre o atraso da parcela, em R$.
        /// </summary>
        public Decimal MultaRS
        {
            get { return _multaRS; }
            set { _multaRS = value; }
        }

        public Decimal Amortizacao
        {
            get { return _amortizacao; }
            set { _amortizacao = value; }
        }

        /// <summary>
        /// No caso do boleto itaú, essa propriedade retorna o parâmetro necessário para ser enviado 
        /// ao boletomail, utilizando a conta da PSPADRAO.
        /// </summary>
        public static String BoletoUrlCompPSPadrao
        {
            get
            {
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["boletoMailUrlParam"]))
                {
                    return String.Empty;
                }
                else
                {
                    return String.Concat("&", ConfigurationManager.AppSettings["boletoMailUrlParam"]);
                }
            }
        }

        /// <summary>
        /// No caso do boleto itaú, essa propriedade retorna o parâmetro necessário para ser enviado 
        /// ao boletomail, utilizando a conta da QUALICORP.
        /// </summary>
        public static String BoletoUrlCompQualicorp
        {
            get
            {
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["boletoMailUrlParamQ"]))
                {
                    return String.Empty;
                }
                else
                {
                    return String.Concat("&", ConfigurationManager.AppSettings["boletoMailUrlParamQ"]);
                }
            }
        }

        #endregion

        public Cobranca(Object id) : this() { _id = id; }
        public Cobranca() { _dataCriacao = DateTime.Now; _pago = false; _cancelada = false; _carteira = (Int32)eCarteira.Unibanco; _dataVencimentoForcada = false; }

        #region EntityBase methods

        public void Salvar()
        {
            if (((eTipo)this._tipo) == eTipo.Dupla && this._valorNominal == 0)
            {
                if (this._cobrancaRefId != null)
                {
                    Cobranca cobrancaRef = new Cobranca(this._cobrancaRefId);
                    cobrancaRef.Carregar();
                    this._valorNominal = this._valor - cobrancaRef.Valor;
                }
                else
                {
                    Cobranca cobrancaRef = new Cobranca();
                }
            }

            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        /// <summary>
        /// Para o unibanco, gera com DV. Para o itau, gera SEM o DV.
        /// </summary>
        public String GeraNossoNumero()
        {
            //if (!Cobranca.NossoNumeroITAU)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    sb.Append(this._tipo);
            //    sb.Append(this._contratoCodCobranca.PadLeft(10, '0'));

            //    String dv = this._CalculaDVMod11(sb.ToString() + _parcela.ToString().PadLeft(3, '0'));

            //    sb.Append(_parcela.ToString().PadLeft(3, '0'));
            //    sb.Append(dv);

            //    String nossonumero = sb.ToString();
            //    sb.Remove(0, sb.Length);
            //    sb = null;
            //    return nossonumero;
            //}
            //else
            //{
            //String nossonumero = Convert.ToString(this._id).PadLeft(8, '0');
            ////nossonumero = String.Concat(nossonumero, CalculaDVMod11(nossonumero));
            //return nossonumero;

            return string.Concat(Cobranca.ConvenioUBRASP, Convert.ToString(this._id).PadLeft(10, '0'));
            //}
        }

        String geraNossoNumeroItau()
        {
            return Convert.ToString(this._id).PadLeft(8, '0');
        }
        String geraNossoNumeroUnibanco()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this._tipo);
            sb.Append(this._contratoCodCobranca.PadLeft(10, '0'));

            String dv = this._CalculaDVMod11(sb.ToString() + _parcela.ToString().PadLeft(3, '0'));

            sb.Append(_parcela.ToString().PadLeft(3, '0'));
            sb.Append(dv);

            String nossonumero = sb.ToString();
            sb.Remove(0, sb.Length);
            sb = null;
            return String.Format("{0:" + new String('0', 12) + "}", Convert.ToInt64(nossonumero));
        }

        public String _CalculaDVMod11(Int32 tipo, String contratoCodCobranca, Int32 parcela)
        {
            StringBuilder sb = new StringBuilder();

            if (!Cobranca.NossoNumeroITAU)
            {
                sb.Append(tipo);
                sb.Append(contratoCodCobranca.PadLeft(10, '0'));
            }
            else
            {
                sb.Append(Convert.ToString(this._id).PadLeft(11, '0'));
            }

            return this._CalculaDVMod11(sb.ToString() + parcela.ToString().PadLeft(3, '0'));
        }

        public String _CalculaDVMod11(String nossoNumero)
        {
            Int32 fatorMult = 2;
            Int32 resultado = 0;

            char[] buffer = nossoNumero.ToCharArray();
            Array.Reverse(buffer);
            String nossoNumeroReverso = new String(buffer);

            for (int i = 0; i < nossoNumeroReverso.Length; i++)
            {
                resultado += Convert.ToInt32(nossoNumeroReverso.Substring(i, 1)) * fatorMult;
                fatorMult++;
                if (fatorMult > 9) { fatorMult = 2; }
            }

            resultado *= 10;
            resultado %= 11;
            resultado %= 10;

            return resultado.ToString();
        }

        public String CalculaDVMod10(Int32 tipo, String contratoCodCobranca, Int32 parcela)
        {
            StringBuilder sb = new StringBuilder();

            if (!Cobranca.NossoNumeroITAU)
            {
                sb.Append(tipo);
                sb.Append(contratoCodCobranca.PadLeft(10, '0'));
                return this.CalculaDVMod10(sb.ToString() + parcela.ToString().PadLeft(3, '0'));
            }
            else
            {
                sb.Append(Convert.ToString(this._id).PadLeft(8, '0'));
                return this.CalculaDVMod10(sb.ToString());
            }
        }

        public string CalculaDVMod10(String nossoNumero)
        {
            nossoNumero = String.Concat("0646", "04260", "175", nossoNumero);

            int i = 2;
            int sum = 0;
            int res = 0;
            Char[] inverse = nossoNumero.ToCharArray();
            Array.Reverse(inverse);
            String[] strarray = new String[nossoNumero.Length];

            int index = 0;
            foreach (char c in inverse)
            {
                res = Convert.ToInt32(c.ToString()) * i;
                sum += res > 9 ? (res - 9) : res;
                i = i == 2 ? 1 : 2;

                strarray[index] = res.ToString();
                index++;
            }

            sum = 0;
            Array.Reverse(strarray);

            foreach (String item in strarray)
            {
                if (item.Length == 1)
                {
                    sum += Convert.ToInt32(item);
                }
                else
                {
                    sum += Convert.ToInt32(item.Substring(0, 1));
                    sum += Convert.ToInt32(item.Substring(1, 1));
                }
            }

            int resto = sum % 10;
            return Convert.ToString(10 - resto);
        }

        public void LeNossoNumero(String nossoNumero)
        {
            if (!Cobranca.NossoNumeroITAU)
            {
                Cobranca.LeNossoNumero(nossoNumero, out this._tipo, out this._contratoCodCobranca, out this._parcela);
            }
            else
            {
                this._id = Convert.ToInt32(nossoNumero);
            }
        }

        public void LeNossoNumeroUNIBANCO(String nossoNumero)
        {
            Cobranca.LeNossoNumero(nossoNumero, out this._tipo, out this._contratoCodCobranca, out this._parcela);
        }

        public static void LeNossoNumero(String nossoNumero, out Int32 tipo, out String codCobranca, out Int32 parcela)
        {
            //tipo = 0; propostaId = 0; parcela = 0;
            try
            {
                tipo = Convert.ToInt32(nossoNumero.Substring(0, 1));
                codCobranca = Convert.ToInt32(nossoNumero.Substring(1, 10)).ToString();
                parcela = Convert.ToInt32(nossoNumero.Substring(11, 3));
            }
            catch
            {
                tipo = (int)Cobranca.eTipo.Indefinido;
                codCobranca = null;
                parcela = -1;
            }
        }

        #region Load methods

        public static Cobranca CarregarPor(Object propostaId, Int32 mes, Int32 ano, Cobranca.eTipo tipo, PersistenceManager pm)
        {
            IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>("* FROM cobranca WHERE cobranca_propostaId=" + propostaId + " AND MONTH(cobranca_dataVencimento)=" + mes.ToString() + " AND YEAR(cobranca_dataVencimento)=" + ano.ToString() + " AND cobranca_tipo=" + Convert.ToInt32(tipo).ToString(), typeof(Cobranca), pm);

            if (cobrancas == null || cobrancas.Count == 0)
                return null;
            else
                return cobrancas[0];
        }
        public static Cobranca CarregarPor(Object propostaId, Int32 parcela, Int32 cobrancaTipo)
        {
            return CarregarPor(propostaId, parcela, cobrancaTipo, null);
        }
        public static Cobranca CarregarPor(Object propostaId, Int32 parcela, Int32 cobrancaTipo, PersistenceManager pm)
        {
            String tipoCond = "";
            if (((eTipo)cobrancaTipo) != eTipo.Indefinido)
            {
                tipoCond = " cobranca_tipo=" + cobrancaTipo + " AND ";
            }

            String qry = String.Concat(
                "SELECT operadora_id, filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                "       cobranca_parcela=", parcela, " AND ",
                tipoCond,
                "       cobranca_propostaId =", propostaId,
                "   ORDER BY filial_nome, estipulante_descricao, operadora_nome, cobranca_dataVencimento");

            IList<Cobranca> lista = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
            if (lista == null)
                return null;
            else if (lista.Count == 1)
                return lista[0];
            else
                return lista[0]; //return null; //throw new ApplicationException("Mais de uma cobrança foi retornada.");
        }

        public static Cobranca CarregarPorNossoNumero(string nossoNumero, PersistenceManager pm)
        {
            String qry = String.Concat(
                "SELECT operadora_id, filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                "       cobranca_nossoNumero='", nossoNumero, "'");

            IList<Cobranca> lista = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
            if (lista == null)
                return null;
            else if (lista.Count == 1)
                return lista[0];
            else
                throw new ApplicationException("Mais de uma cobrança foi retornada com o nosso número " + nossoNumero);
        }

        public static Cobranca CarregarPor(Object propostaId, DateTime vencimento, Int32 cobrancaTipo, PersistenceManager pm)
        {
            String tipoCond = "";
            if (((eTipo)cobrancaTipo) != eTipo.Indefinido)
            {
                tipoCond = " cobranca_tipo=" + cobrancaTipo + " AND ";
            }

            String qry = String.Concat(
                "SELECT top 1 cobranca_id ", // operadora_id, filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, cobranca.* ",
                "   FROM cobranca ",
                "   WHERE ",
                "       DAY(cobranca_dataVencimento)=", vencimento.Day, " AND MONTH(cobranca_dataVencimento)=", vencimento.Month, " and YEAR(cobranca_dataVencimento)=", vencimento.Year, " AND ",
                tipoCond,
                "       cobranca_propostaId =", propostaId);

            IList<Cobranca> lista = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else //if (lista.Count == 1)
                return lista[0];
            //else
            //    return lista[0]; //return null; //throw new ApplicationException("Mais de uma cobrança foi retornada.");
        }

        public static Cobranca CarregarEnviadasPor(Object propostaId, DateTime vencimento, Int32 cobrancaTipo, PersistenceManager pm)
        {
            String tipoCond = "";
            if (((eTipo)cobrancaTipo) != eTipo.Indefinido)
            {
                tipoCond = " cobranca_tipo=" + cobrancaTipo + " AND ";
            }

            String qry = String.Concat(
                "SELECT cobranca_id ",
                "   FROM cobranca ",
                "   WHERE cobranca_arquivoultimoenvioid is not null and ",
                "       DAY(cobranca_dataVencimento)=", vencimento.Day, " AND MONTH(cobranca_dataVencimento)=", vencimento.Month, " and YEAR(cobranca_dataVencimento)=", vencimento.Year, " AND ",
                tipoCond,
                "       cobranca_propostaId =", propostaId);

            IList<Cobranca> lista = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }

        public static Cobranca CarregarPorParcelaTipo(Object cobrancaId, Int32 parcela, Int32 cobrancaTipo)
        {
            return null;//CarregarPor(propostaId, parcela, cobrancaTipo, null);
        }

        public static IList<Cobranca> CarregarTodasNaoPagas(DateTime venctoDe, DateTime venctoAte)
        {
            String qry = "* FROM cobranca WHERE cobranca_pago <> 1 AND cobranca_cancelada <> 1 AND cobranca_dataVencimento BETWEEN '" + venctoDe.ToString("yyyy-MM-dd 00:00:00.000") + "' AND '" + venctoAte.ToString("yyyy-MM-dd 23:59:59.850") + "'";

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca));
        }

        public static IList<Cobranca> CarregarTodas(Object propostaId)
        {
            return CarregarTodas(propostaId, null);
        }
        public static IList<Cobranca> CarregarTodas(Object propostaId, PersistenceManager pm)
        {
            String qry = "cobranca.*, operadora_id FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_propostaId=" + propostaId + " ORDER BY cobranca_parcela, cobranca_id";

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarTodasORDERBYVencto(Object propostaId, PersistenceManager pm)
        {
            String qry = "cobranca.*, operadora_id FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_propostaId=" + propostaId + " ORDER BY cobranca_dataVencimento, cobranca_id";

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarTodas(Object propostaId, Boolean apenasAtivas, PersistenceManager pm)
        {
            return CarregarTodas(propostaId, apenasAtivas, false, pm);
        }
        public static IList<Cobranca> CarregarTodas(Object propostaId, Boolean apenasAtivas, Boolean apenasEmAberto, Int32 anoVencimento, PersistenceManager pm)
        {
            String ativasCond = "";
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }
            if (apenasEmAberto)
            {
                ativasCond += " AND (cobranca_pago=0 or cobranca_pago is null) ";
            }

            if (anoVencimento != -1)
                ativasCond += " AND year(cobranca_dataVencimento)=" + anoVencimento.ToString();

            String qry = String.Concat("cobranca.*, operadora_id FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_propostaId=", propostaId, ativasCond, " ORDER BY cobranca_parcela");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }
        public static IList<Cobranca> CarregarTodas(Object propostaId, Boolean apenasAtivas, Boolean parcelaDesc, PersistenceManager pm)
        {
            String ativasCond = "";
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }

            String strdesc = " ";
            if (parcelaDesc) { strdesc = " DESC "; }
            String qry = String.Concat("cobranca.*, operadora_id FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_propostaId=", propostaId, ativasCond, " ORDER BY cobranca_parcela", strdesc, ",cobranca_id");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }
        public static IList<Cobranca> CarregarTodas(Object propostaId, Boolean apenasAtivas, eTipo tipo, PersistenceManager pm)
        {
            String ativasCond = "";
            String tipoCond = "";
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }

            if (tipo != eTipo.Indefinido)
            {
                tipoCond = " AND cobranca_tipo=" + Convert.ToInt32(tipo).ToString();
            }

            String qry = String.Concat("cobranca.*, operadora_id FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_propostaId=", propostaId, ativasCond, tipoCond, " ORDER BY cobranca_parcela, cobranca_id");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }
        public static IList<Cobranca> CarregarTodas_Composite(Object propostaId, Boolean apenasAtivas, Boolean parcelaDesc, PersistenceManager pm)
        {
            String ativasCond = "";
            if (apenasAtivas) { ativasCond = " AND cobranca_cancelada <> 1 "; }

            String strdesc = " ";
            if (parcelaDesc) { strdesc = " DESC "; }

            String qry = String.Concat("cobranca.*,operadora_id,cobrancacomp_beneficiarioId,cobrancacomp_tipo,cobrancacomp_valor ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id = contrato_operadoraId ",
                "       LEFT JOIN cobranca_composicao on cobranca_id=cobrancacomp_cobranaId and cobrancacomp_valor > 0 ",
                "   WHERE cobranca_propostaId=", propostaId, ativasCond,
                "   ORDER BY cobranca_parcela", strdesc, ",cobranca_id");

            IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);

            if (cobrancas == null) { return null; }

            List<String> ids = new List<String>();

            List<Cobranca> cobrancasARetornar = new List<Cobranca>();

            foreach (Cobranca _cob in cobrancas)
            {
                if (ids.Contains(Convert.ToString(_cob.ID))) { continue; }

                cobrancasARetornar.Add(_cob);
                ids.Add(Convert.ToString(_cob.ID));
            }

            foreach (Cobranca cobrancaARetornar in cobrancasARetornar)
            {
                foreach (Cobranca cob in cobrancas)
                {
                    if (Convert.ToString(cob.ID) == Convert.ToString(cobrancaARetornar.ID))
                    {
                        if (cobrancaARetornar._composicaoResumo != null && cobrancaARetornar._composicaoResumo.Length > 0) { cobrancaARetornar._composicaoResumo += "<br>"; }

                        if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.Adicional)
                            cobrancaARetornar._composicaoResumo += "Adicional: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.Desconto)
                            cobrancaARetornar._composicaoResumo += "Desconto: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.Plano)
                            cobrancaARetornar._composicaoResumo += "Plano: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.TaxaAssociacao)
                            cobrancaARetornar._composicaoResumo += "Taxa associativa: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.TaxaTabelaValor)
                            cobrancaARetornar._composicaoResumo += "Taxa: ";

                        cobrancaARetornar._composicaoResumo += cob._composicaoValor.ToString("C");
                    }
                }
            }

            return cobrancasARetornar;
        }

        public static IList<Cobranca> CarregarTodas(IList<String> cobrancaIDs, PersistenceManager pm)
        {
            String qry = String.Concat("operadora_id, filial_nome, beneficiario_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_enderecoCobrancaId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                //"       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                //"       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                //"       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "       LEFT JOIN usuario_filial          ON contrato_donoId=usuariofilial_usuarioId ",
                "       LEFT JOIN filial                  ON filial_id=usuariofilial_filialId ",
                "   WHERE cobranca_id IN (", EntityBase.Join(cobrancaIDs, ","), ")",
                "   ORDER BY filial_nome, estipulante_descricao, operadora_nome, cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarTodas_OrdemPorContratoParcela(IList<String> cobrancaIDs, PersistenceManager pm)
        {
            String qry = String.Concat("operadora_id, filial_nome, beneficiario_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_enderecoCobrancaId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                //"       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                //"       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                //"       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "       LEFT JOIN usuario_filial          ON contrato_donoId=usuariofilial_usuarioId ",
                "       LEFT JOIN filial                  ON filial_id=usuariofilial_filialId ",
                "   WHERE cobranca_id IN (", EntityBase.Join(cobrancaIDs, ","), ")",
                "   ORDER BY contrato_id, cobranca_parcela");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarTodas_OrdemPorContratoParcela_Optimized(IList<String> cobrancaIDs, PersistenceManager pm)
        {
            List<Cobranca> cobrancas = null;

            String qry = String.Concat("operadora_id, filial_nome, beneficiario_nome, beneficiario_cpf, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_enderecoCobrancaId, contrato_codcobranca, contratoadm_id, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       INNER JOIN contratoadm on contratoadm_id=contrato_contratoadmId ",
                //"       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                //"       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                //"       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "       LEFT JOIN usuario_filial          ON contrato_donoId=usuariofilial_usuarioId ",
                "       LEFT JOIN filial                  ON filial_id=usuariofilial_filialId ",
                "   WHERE cobranca_id IN (", cobrancaIDs[0], ")",
                "   ORDER BY contrato_id, cobranca_parcela");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result", pm).Tables[0];
            DataTable aux = null; DataRow novaRow = null;
            for (int i = 1; i < cobrancaIDs.Count; i++)
            {
                qry = String.Concat("operadora_id, filial_nome, beneficiario_nome, beneficiario_cpf, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_enderecoCobrancaId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN usuario_filial          ON contrato_donoId=usuariofilial_usuarioId ",
                "       LEFT JOIN filial                  ON filial_id=usuariofilial_filialId ",
                "   WHERE cobranca_id =", cobrancaIDs[i]);

                aux = LocatorHelper.Instance.ExecuteQuery(qry, "result", pm).Tables[0];

                if (aux.Rows.Count == 0) { continue; }

                novaRow = dt.NewRow();
                novaRow["cobranca_id"] = aux.Rows[0]["cobranca_id"];
                novaRow["cobranca_arquivoUltimoEnvioId"] = aux.Rows[0]["cobranca_arquivoUltimoEnvioId"];
                novaRow["cobranca_cancelada"] = aux.Rows[0]["cobranca_cancelada"];
                novaRow["cobranca_carteira"] = aux.Rows[0]["cobranca_carteira"];
                novaRow["cobranca_cobrancaRefId"] = aux.Rows[0]["cobranca_cobrancaRefId"];
                novaRow["cobranca_comissaoPaga"] = aux.Rows[0]["cobranca_comissaoPaga"];
                novaRow["contrato_codcobranca"] = aux.Rows[0]["contrato_codcobranca"];
                novaRow["contrato_numero"] = aux.Rows[0]["contrato_numero"];
                novaRow["beneficiario_nome"] = aux.Rows[0]["beneficiario_nome"];
                novaRow["beneficiario_cpf"] = aux.Rows[0]["beneficiario_cpf"];
                novaRow["cobranca_dataCriacao"] = aux.Rows[0]["cobranca_dataCriacao"];
                novaRow["cobranca_dataPagto"] = aux.Rows[0]["cobranca_dataPagto"];
                novaRow["cobranca_dataVencimentoForcada"] = aux.Rows[0]["cobranca_dataVencimentoForcada"];
                novaRow["estipulante_descricao"] = aux.Rows[0]["estipulante_descricao"];
                novaRow["filial_nome"] = aux.Rows[0]["filial_nome"];
                novaRow["cobranca_nossoNumero"] = aux.Rows[0]["cobranca_nossoNumero"];
                novaRow["operadora_id"] = aux.Rows[0]["operadora_id"];
                novaRow["operadora_nome"] = aux.Rows[0]["operadora_nome"];
                novaRow["cobranca_pago"] = aux.Rows[0]["cobranca_pago"];
                novaRow["cobranca_parcela"] = aux.Rows[0]["cobranca_parcela"];
                novaRow["cobranca_propostaId"] = aux.Rows[0]["cobranca_propostaId"];
                novaRow["cobranca_tipo"] = aux.Rows[0]["cobranca_tipo"];
                novaRow["cobranca_tipoTemp"] = aux.Rows[0]["cobranca_tipoTemp"];
                novaRow["cobranca_valor"] = aux.Rows[0]["cobranca_valor"];
                novaRow["cobranca_valorNominal"] = aux.Rows[0]["cobranca_valorNominal"];
                novaRow["cobranca_valorPagto"] = aux.Rows[0]["cobranca_valorPagto"];
                novaRow["cobranca_dataVencimento"] = aux.Rows[0]["cobranca_dataVencimento"];
                novaRow["cobranca_dataVencimentoIsencaoJuro"] = aux.Rows[0]["cobranca_dataVencimentoIsencaoJuro"];
                novaRow["contrato_enderecoCobrancaId"] = aux.Rows[0]["contrato_enderecoCobrancaId"];
                dt.Rows.Add(novaRow);
            }

            if (dt.Rows.Count > 0) { cobrancas = new List<Cobranca>(); }

            DataRow[] arrRow = dt.Select("cobranca_id <> -1", "cobranca_propostaId ASC, cobranca_parcela ASC");

            Cobranca cobranca = null;
            foreach (DataRow row in arrRow)
            {
                cobranca = new Cobranca(row["cobranca_id"]);
                cobranca._arquivoIdUltimoEnvio = toObject(row["cobranca_arquivoUltimoEnvioId"]);
                cobranca._cancelada = toBoolean(row["cobranca_cancelada"]);
                cobranca._carteira = toInt(row["cobranca_carteira"]);
                cobranca._cobrancaRefId = toObject(row["cobranca_cobrancaRefId"]);
                cobranca._comissaoPaga = toBoolean(row["cobranca_comissaoPaga"]);
                cobranca._contratoCodCobranca = toString(row["contrato_codcobranca"]);
                cobranca._contratoNumero = toString(row["contrato_numero"]);
                cobranca._contratoTitularNome = toString(row["beneficiario_nome"]);
                cobranca._contratoTitularCpf = toString(row["beneficiario_cpf"]);
                cobranca._dataCriacao = Convert.ToDateTime(row["cobranca_dataCriacao"]);
                cobranca._dataPagto = toDateTime(row["cobranca_dataPagto"]);
                cobranca._dataVencimentoForcada = toBoolean(row["cobranca_dataVencimentoForcada"]);
                cobranca._estipulanteNome = toString(row["estipulante_descricao"]);
                cobranca._filialNome = toString(row["filial_nome"]);
                cobranca._nossoNumero = toString(row["cobranca_nossoNumero"]);
                cobranca._operadoraId = row["operadora_id"];
                cobranca._operadoraNome = toString(row["operadora_nome"]);
                cobranca._pago = toBoolean(row["cobranca_pago"]);
                cobranca._parcela = toInt(row["cobranca_parcela"]);
                cobranca._propostaId = row["cobranca_propostaId"];
                cobranca._tipo = toInt(row["cobranca_tipo"]);
                cobranca._tipoTemp = toInt(row["cobranca_tipoTemp"]);
                cobranca._valor = toDecimal(row["cobranca_valor"]);
                cobranca._valorNominal = toDecimal(row["cobranca_valorNominal"]);
                cobranca._valorPagto = toDecimal(row["cobranca_valorPagto"]);
                cobranca._vencimento = toDateTime(row["cobranca_dataVencimento"]);
                cobranca._vencimentoIsencaoJuro = toDateTime(row["cobranca_dataVencimentoIsencaoJuro"]);
                cobranca._contratoEnderecoCobrancaId = toObject(row["contrato_enderecoCobrancaId"]);

                cobrancas.Add(cobranca);
            }

            dt.Dispose();
            return cobrancas;
        }

        public static IList<Cobranca> CarregarTodas_Optimized(Object propostaId, Boolean apenasAtivas, eTipo tipo, PersistenceManager pm)
        {
            String ativasCond = "";
            String tipoCond = "";
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }

            if (tipo != eTipo.Indefinido)
            {
                tipoCond = " AND cobranca_tipo=" + Convert.ToInt32(tipo).ToString();
            }

            String qry = String.Concat("operadora_id, cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_propostaId=", propostaId, ativasCond, tipoCond, " ORDER BY cobranca_parcela, cobranca_id");
            //String qry = String.Concat("operadora_id, cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_id=2715955");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];
            if (dt.Rows.Count == 0) { return null; }
            List<Cobranca> cobrancas = new List<Cobranca>();

            Cobranca cobranca = null;
            foreach (DataRow row in dt.Rows)
            {
                cobranca = new Cobranca(row["cobranca_id"]);
                cobranca._arquivoIdUltimoEnvio = toObject(row["cobranca_arquivoUltimoEnvioId"]);
                cobranca._cancelada = toBoolean(row["cobranca_cancelada"]);
                cobranca._carteira = toInt(row["cobranca_carteira"]);
                cobranca._cobrancaRefId = toObject(row["cobranca_cobrancaRefId"]);
                cobranca._comissaoPaga = toBoolean(row["cobranca_comissaoPaga"]);
                cobranca._dataCriacao = Convert.ToDateTime(row["cobranca_dataCriacao"]);
                cobranca._dataPagto = toDateTime(row["cobranca_dataPagto"]);
                cobranca._dataVencimentoForcada = toBoolean(row["cobranca_dataVencimentoForcada"]);
                cobranca._nossoNumero = toString(row["cobranca_nossoNumero"]);
                cobranca._operadoraId = row["operadora_id"];
                cobranca._pago = toBoolean(row["cobranca_pago"]);
                cobranca._parcela = toInt(row["cobranca_parcela"]);
                cobranca._propostaId = row["cobranca_propostaId"];
                cobranca._tipo = toInt(row["cobranca_tipo"]);
                cobranca._tipoTemp = toInt(row["cobranca_tipoTemp"]);
                cobranca._valor = toDecimal(row["cobranca_valor"]);
                cobranca._valorNominal = toDecimal(row["cobranca_valorNominal"]);
                cobranca._valorPagto = toDecimal(row["cobranca_valorPagto"]);
                cobranca._vencimento = toDateTime(row["cobranca_dataVencimento"]);
                cobranca._vencimentoIsencaoJuro = toDateTime(row["cobranca_dataVencimentoIsencaoJuro"]);

                cobrancas.Add(cobranca);
            }

            return cobrancas;
        }

        public static IList<Cobranca> CarregarTodas_Optimized(Boolean apenasAtivas, eTipo tipo, PersistenceManager pm)
        {
            String ativasCond = "";
            String tipoCond = "";
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }

            if (tipo != eTipo.Indefinido)
            {
                tipoCond = " AND cobranca_tipo=" + Convert.ToInt32(tipo).ToString();
            }

            String qry = String.Concat("operadora_id, cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE (cobranca_arquivoUltimoEnvioId is null or cobranca_arquivoUltimoEnvioId=-2 or cobranca_arquivoUltimoEnvioId=-3) ", ativasCond, tipoCond, " ORDER BY cobranca_parcela, cobranca_id");
            //String qry = String.Concat("operadora_id, cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_id=2715955");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];
            if (dt.Rows.Count == 0) { return null; }
            List<Cobranca> cobrancas = new List<Cobranca>();

            Cobranca cobranca = null;
            foreach (DataRow row in dt.Rows)
            {
                cobranca = new Cobranca(row["cobranca_id"]);
                cobranca._arquivoIdUltimoEnvio = toObject(row["cobranca_arquivoUltimoEnvioId"]);
                cobranca._cancelada = toBoolean(row["cobranca_cancelada"]);
                cobranca._carteira = toInt(row["cobranca_carteira"]);
                cobranca._cobrancaRefId = toObject(row["cobranca_cobrancaRefId"]);
                cobranca._comissaoPaga = toBoolean(row["cobranca_comissaoPaga"]);
                cobranca._dataCriacao = Convert.ToDateTime(row["cobranca_dataCriacao"]);
                cobranca._dataPagto = toDateTime(row["cobranca_dataPagto"]);
                cobranca._dataVencimentoForcada = toBoolean(row["cobranca_dataVencimentoForcada"]);
                cobranca._nossoNumero = toString(row["cobranca_nossoNumero"]);
                cobranca._operadoraId = row["operadora_id"];
                cobranca._pago = toBoolean(row["cobranca_pago"]);
                cobranca._parcela = toInt(row["cobranca_parcela"]);
                cobranca._propostaId = row["cobranca_propostaId"];
                cobranca._tipo = toInt(row["cobranca_tipo"]);
                cobranca._tipoTemp = toInt(row["cobranca_tipoTemp"]);
                cobranca._valor = toDecimal(row["cobranca_valor"]);
                cobranca._valorNominal = toDecimal(row["cobranca_valorNominal"]);
                cobranca._valorPagto = toDecimal(row["cobranca_valorPagto"]);
                cobranca._vencimento = toDateTime(row["cobranca_dataVencimento"]);
                cobranca._vencimentoIsencaoJuro = toDateTime(row["cobranca_dataVencimentoIsencaoJuro"]);

                cobrancas.Add(cobranca);
            }

            return cobrancas;
        }


        public static IList<Cobranca> CarregarTodasComParcelamentoInfo(IList<String> cobrancaIDs, PersistenceManager pm)
        {
            String qry = String.Concat("operadora_id, parccob_headerId, parcitem_headerId,filial_nome, beneficiario_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_enderecoCobrancaId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN usuario_filial          ON contrato_donoId=usuariofilial_usuarioId ",
                "       LEFT JOIN filial                  ON filial_id=usuariofilial_filialId ",
                "       LEFT JOIN cobranca_parcelamentoCobrancaOriginal ON cobranca_id = parccob_cobrancaId ",
                "       LEFT JOIN cobranca_parcelamentoItem ON cobranca_id = parcitem_cobrancaId ",
                "   WHERE cobranca_id IN (", EntityBase.Join(cobrancaIDs, ","), ")",
                "   ORDER BY filial_nome, estipulante_descricao, operadora_nome, cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarTodasComParcelamentoInfo(Object propostaId, Boolean apenasAtivas, Boolean apenasEmAberto, Int32 anoVencimento, PersistenceManager pm, DateTime cancelamentoContrato)
        {
            String ativasCond = "";
            if (apenasAtivas)
            {
                ativasCond = " AND cobranca_cancelada <> 1 ";
            }
            if (apenasEmAberto)
            {
                ativasCond += " AND ((cobranca_pago=0 or cobranca_pago is null) or cobranca_id in (select parccob_cobrancaId from cobranca_parcelamentoCobrancaOriginal) )";
            }

            if (anoVencimento != -1)
                ativasCond += " AND year(cobranca_dataVencimento)=" + anoVencimento.ToString();

            if (cancelamentoContrato != DateTime.MinValue)
            {
                ativasCond += " and (cobranca_datavencimento < '" + cancelamentoContrato.ToString("yyyy-MM-dd 23:59:59.995") + "' or cobranca_tipo = 4) ";
            }

            String qry = String.Concat("cobranca.*, parccob_headerId, operadora_id FROM cobranca ",
                "   INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "   INNER JOIN operadora ON operadora_id = contrato_operadoraId ",
                "   LEFT  JOIN cobranca_parcelamentoCobrancaOriginal ON cobranca_id = parccob_cobrancaId ",
                "   WHERE cobranca_propostaId=", propostaId, ativasCond, " ORDER BY cobranca_parcela");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }
        public static IList<Cobranca> CarregarTodasComParcelamentoInfo_Composite(Object propostaId, Boolean apenasAtivas, Boolean parcelaDesc, PersistenceManager pm)
        {
            String ativasCond = "";
            if (apenasAtivas) { ativasCond = " AND cobranca_cancelada <> 1 "; }

            String strdesc = " ";
            if (parcelaDesc) { strdesc = " DESC "; }

            String qry = String.Concat("cobranca.*,parccob_headerId,parcitem_headerId,parcitem_obs,operadora_id,cobrancacomp_beneficiarioId,cobrancacomp_tipo,cobrancacomp_valor ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id = contrato_operadoraId ",
                "       LEFT JOIN cobranca_composicao on cobranca_id=cobrancacomp_cobranaId and cobrancacomp_valor > 0 ",
                "       LEFT JOIN cobranca_parcelamentoCobrancaOriginal ON cobranca_id = parccob_cobrancaId ",
                "       LEFT JOIN cobranca_parcelamentoItem ON cobranca_id = parcitem_cobrancaId ",
                "   WHERE cobranca_propostaId=", propostaId, ativasCond,
                "   ORDER BY cobranca_dataVencimento", strdesc, ",cobranca_id");

            IList<Cobranca> cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);

            if (cobrancas == null) { return null; }

            List<String> ids = new List<String>();

            List<Cobranca> cobrancasARetornar = new List<Cobranca>();

            foreach (Cobranca _cob in cobrancas)
            {
                if (ids.Contains(Convert.ToString(_cob.ID))) { continue; }

                cobrancasARetornar.Add(_cob);
                ids.Add(Convert.ToString(_cob.ID));
            }

            foreach (Cobranca cobrancaARetornar in cobrancasARetornar)
            {
                foreach (Cobranca cob in cobrancas)
                {
                    if (Convert.ToString(cob.ID) == Convert.ToString(cobrancaARetornar.ID))
                    {
                        if (cobrancaARetornar._composicaoResumo != null && cobrancaARetornar._composicaoResumo.Length > 0) { cobrancaARetornar._composicaoResumo += "<br>"; }

                        if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.Adicional)
                            cobrancaARetornar._composicaoResumo += "Adicional: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.Desconto)
                            cobrancaARetornar._composicaoResumo += "Desconto: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.Plano)
                            cobrancaARetornar._composicaoResumo += "Plano: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.TaxaAssociacao)
                            cobrancaARetornar._composicaoResumo += "Taxa associativa: ";
                        else if (cob._composicaoTipo == (Int32)CobrancaComposite.eComposicaoTipo.TaxaTabelaValor)
                            cobrancaARetornar._composicaoResumo += "Taxa: ";

                        cobrancaARetornar._composicaoResumo += cob._composicaoValor.ToString("C");
                    }
                }
            }

            return cobrancasARetornar;
        }

        public static IList<Cobranca> CarregarTodasComParcelamentoInfo(Object propostaId, Boolean apenasAtivas, eTipo tipo, PersistenceManager pm)
        {
            String cond = "";
            if (apenasAtivas)
            {
                cond = " AND cobranca_cancelada <> 1 ";
            }
            if (tipo != eTipo.Indefinido)
            {
                cond += " AND cobranca_tipo=" + Convert.ToInt32(tipo); ;
            }

            String qry = String.Concat("cobranca.*, parccob_headerId, operadora_id FROM cobranca ",
                "   INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "   INNER JOIN operadora ON operadora_id = contrato_operadoraId ",
                "   LEFT  JOIN cobranca_parcelamentoCobrancaOriginal ON cobranca_id = parccob_cobrancaId ",
                "   WHERE cobranca_propostaId=", propostaId, cond, " ORDER BY cobranca_parcela");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        static Decimal toDecimal(Object param)
        {
            if (param == DBNull.Value || param == null)
                return Decimal.Zero;
            else
            {
                try
                {
                    return Convert.ToDecimal(param);
                }
                catch
                {
                    return Decimal.Zero;
                }
            }
        }
        static DateTime toDateTime(Object param)
        {
            if (param == DBNull.Value || param == null)
                return DateTime.MinValue;
            else
            {
                try
                {
                    return Convert.ToDateTime(param);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }
        static Object toObject(Object param)
        {
            if (param == DBNull.Value)
                return null;
            else
                return param;
        }
        static Boolean toBoolean(Object param)
        {
            if (param == DBNull.Value || param == null)
                return false;
            else
            {
                try
                {
                    return Convert.ToBoolean(param);
                }
                catch
                {
                    return false;
                }
            }
        }
        static Int32 toInt(Object param)
        {
            if (param == DBNull.Value || param == null)
                return 0;
            else
            {
                try
                {
                    return Convert.ToInt32(param);
                }
                catch
                {
                    return 0;
                }
            }
        }
        static String toString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return null;
            else
                return Convert.ToString(param);
        }

        public static IList<Cobranca> CarregarPorArquivoRemessaID(Object arquivoId, Boolean emAberto)
        {
            return CarregarPorArquivoRemessaID(arquivoId, emAberto, null);
        }
        public static IList<Cobranca> CarregarPorArquivoRemessaID(Object arquivoId, Boolean emAberto, PersistenceManager pm)
        {
            String emAbertoCond = "";
            if (emAberto)
            {
                emAbertoCond = " cobranca_pago=0 AND ";
            }

            String qry = String.Concat(
                "SELECT DISTINCT(cobranca_id) as cid, operadora_id, filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                //"       INNER JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                //"       INNER JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                //"       INNER JOIN filial ON filial_id=almox_produto_filialId ",
                "       INNER JOIN usuario_filial          ON contrato_donoId=usuariofilial_usuarioId ",
                "       INNER JOIN filial                  ON filial_id=usuariofilial_filialId ",
                "       INNER JOIN arquivoCobrancaUnibanco_cobanca ON arqitem_cobrancaId=cobranca_id ",
                "   WHERE ", emAbertoCond,
                "       arqitem_arquivoId =", arquivoId,
                "   ORDER BY cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarTodas(IList<String> cobrancaIDs)
        {
            return CarregarTodas(cobrancaIDs, null);
        }

        /// <summary>
        /// Checa se só há uma cobrança por proposta em uma coleção de cobranças.
        /// </summary>
        /// <param name="lista">Lista de cobranças.</param>
        /// <param name="propostaId">Id da proposta a ser comparado</param>
        /// <returns>True, caso só haja uma cobrança por proposta. Do contrário, False.</returns>
        static Boolean umaCobrancaPorProposta(IList<Cobranca> lista, String propostaId)
        {
            Int32 qtd = 0;
            foreach (Cobranca cobranca in lista)
            {
                if (Convert.ToString(cobranca.PropostaID) == propostaId)
                    qtd++;

                if (qtd > 1) { return false; }
            }

            return true;
        }

        public static IList<Cobranca> CarregarPorNumeroDeContrato(Object operadoraId, String contratoNumero, PersistenceManager pm)
        {
            String cond = "";
            if (operadoraId != null) { cond = " and operadora_id=" + operadoraId; }

            String qry = String.Concat(
                "filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                "       contrato_numero='", contratoNumero, "' ",
                cond,
                "   ORDER BY cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarPorID(Object id, PersistenceManager pm)
        {
            String qry = String.Concat(
                "filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                "       cobranca_id=", id);

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static Cobranca CarregarUnicaPorID(string id, PersistenceManager pm)
        {
            String qry = String.Concat(
                "SELECT operadora_id, filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       LEFT JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       LEFT JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       LEFT JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                "       cobranca_id=", id);

            IList<Cobranca> lista = LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
            if (lista == null)
                return null;
            else if (lista.Count == 1)
                return lista[0];
            else
                throw new ApplicationException("Mais de uma cobrança foi retornada com o id " + id);
        }

        public static IList<Cobranca> CarregarAtrasadas(Object operadoraId)
        {
            return CarregarAtrasadas(operadoraId, null);
        }
        public static IList<Cobranca> CarregarAtrasadas(Object operadoraId, PersistenceManager pm)
        {
            String qry = String.Concat(
                "filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       INNER JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       INNER JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       INNER JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                "       cobranca_dataVencimento < GETDATE() AND cobranca_pago=0 AND operadora_id=", operadoraId,
                "   ORDER BY cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        public static IList<Cobranca> CarregarAtrasadas(Object operadoraId, Int32 mes, Int32 ano)
        {
            return CarregarAtrasadas(operadoraId, mes, ano, null);
        }
        public static IList<Cobranca> CarregarAtrasadas(Object operadoraId, Int32 mes, Int32 ano, PersistenceManager pm)
        {
            String qry = String.Concat(
                "filial_nome, estipulante_descricao, operadora_nome, contrato_id, contrato_numero, contrato_numeroId, contrato_codcobranca, cobranca.* ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       left JOIN almox_contrato_impresso ON contrato_numeroId=almox_contratoimp_id ",
                "       left JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                "       left JOIN filial ON filial_id=almox_produto_filialId ",
                "   WHERE ",
                Contrato.CondicaoBasicaQuery, " AND ",
                "       MONTH(cobranca_dataVencimento)=", mes, " AND ",
                "       YEAR(cobranca_dataVencimento)=", ano, " AND ",
                "       cobranca_pago=0 AND operadora_id=", operadoraId,
                "   ORDER BY cobranca_dataVencimento");

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca), pm);
        }

        #endregion

        public static void MarcarCobrancaComoNaoEnviadas(Object arquivoRemessaId, Object cobrancaId)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                MarcarCobrancaComoNaoEnviadas(arquivoRemessaId, cobrancaId, pm);
                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }
        public static void MarcarCobrancaComoNaoEnviadas(Object arquivoRemessaId, Object cobrancaId, PersistenceManager pm)
        {
            Cobranca cobranca = new Cobranca(cobrancaId);
            pm.Load(cobranca);
            cobranca.ArquivoIDUltimoEnvio = null;
            pm.Save(cobranca);

            String command = "DELETE FROM arquivoCobrancaUnibanco_cobanca WHERE arqitem_arquivoId=" + arquivoRemessaId + " AND arqitem_cobrancaId=" + cobrancaId;
            NonQueryHelper.Instance.ExecuteNonQuery(command, pm);
        }

        public static void MarcarCobrancasComoNaoEnviadas(Object arquivoRemessaId, Boolean apenasNaoPagas)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                MarcarCobrancasComoNaoEnviadas(arquivoRemessaId, apenasNaoPagas, pm);
                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }
        public static void MarcarCobrancasComoNaoEnviadas(Object arquivoRemessaId, Boolean apenasNaoPagas, PersistenceManager pm)
        {
            IList<Cobranca> cobrancas = Cobranca.CarregarPorArquivoRemessaID(arquivoRemessaId, apenasNaoPagas, pm);
            if (cobrancas == null) { return; }

            String command = "DELETE FROM arquivoCobrancaUnibanco_cobanca WHERE arqitem_arquivoId=" + arquivoRemessaId + " AND arqitem_cobrancaId=";
            foreach (Cobranca cobranca in cobrancas)
            {
                if (cobranca.Pago) { continue; }
                cobranca.ArquivoIDUltimoEnvio = null;

                if (pm != null)
                    pm.Save(cobranca);
                else
                    cobranca.Salvar();

                NonQueryHelper.Instance.ExecuteNonQuery(command + cobranca.ID, pm);
            }
        }

        /// <summary>
        /// Carrega cobranças NÃO enviadas em arquivo de remessa.
        /// </summary>
        public static IList<Cobranca> ProcessarCobrancasPorTipoParaGerarRemessa(String[] estipulanteIDs, String[] operadoraIDs, String[] filialIDs, Int32 mes, Int32 ano, Cobranca.eTipo tipo)
        {
            #region query

            String filIN = String.Join(",", filialIDs);
            String opeIN = String.Join(",", operadoraIDs);
            String estIN = String.Join(",", estipulanteIDs);

            String qry = String.Concat(
                "SELECT DISTINCT(cobranca_id), cobranca.*, operadora_id, filial_nome, estipulante_descricao, operadora_nome, contrato_numero, contrato_enderecoCobrancaId, contrato_codcobranca, contrato_vencimento ",
                "   FROM cobranca ",
                "       INNER JOIN contrato ON contrato_id=cobranca_propostaId ",
                "       INNER JOIN usuario_filial ON contrato_donoId=usuariofilial_usuarioId ",
                "       INNER JOIN operadora ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN filial ON filial_id=usuariofilial_filialId ",
                "       INNER JOIN estipulante ON estipulante_id=contrato_estipulanteId ",
                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_status NOT IN (0,1,10,15,17,21) AND contratobeneficiario_contratoId=contrato_id ",
                "   WHERE cobranca_tipo=", Convert.ToInt32(tipo), " AND ", Contrato.CondicaoBasicaQuery,
                "       AND cobranca_cancelada <> 1 AND cobranca_arquivoUltimoEnvioId IS NULL AND ",
                "       usuariofilial_filialId IN (", filIN, ") AND ",
                "       contrato_estipulanteId IN (", estIN, ") AND ",
                "       contrato_operadoraId IN (", opeIN, ") ",
                "   ORDER BY filial_nome, estipulante_descricao, operadora_nome, contrato_vencimento");

            #endregion

            return LocatorHelper.Instance.ExecuteQuery<Cobranca>(qry, typeof(Cobranca));
        }

        static Cobranca ultimaCobrancaNormalDaColecao(IList<Cobranca> cobrancas)
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            foreach (Cobranca cobranca in cobrancas)
            {
                if (cobranca.Tipo == (int)Cobranca.eTipo.Normal) { list.Add(cobranca); }
            }

            if (list.Count > 0) { return list[list.Count - 1] as Cobranca; }
            else { return null; }
        }

        static int MonthDifference(DateTime lValue, DateTime rValue)
        {
            return Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
        }

        public static IList<Cobranca> ArrumaFurosNaCadeia(ref String err)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            String qry = String.Concat(
                "select contrato_id,contrato_vencimento from contrato ",
                "   where ",
                //"       contrato_id > 202402 and ", // contrato_id <> 177176 and
                "       (contrato_inativo is null or contrato_inativo=0) and ",
                "       (contrato_cancelado is null or contrato_cancelado=0) and year(contrato_data) >= 2012 order by contrato_id ");

            List<Contrato> contratos = new List<Contrato>();
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                Contrato contrato = new Contrato(row[0]);
                contrato.Vencimento = Convert.ToDateTime(row[1], cinfo);
                contratos.Add(contrato);
            }

            //IList<Contrato> contratos = LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato), pm);

            if (contratos == null) { pm.Dispose(); return null; }

            //int min = 1, max = 0, curr = 0;

            //Object ret = null;
            Cobranca cobranca = null;
            IList<Cobranca> cobrancas = new List<Cobranca>();
            DateTime vencimento, proxVencimento;
            List<CobrancaComposite> composite = null;
            int diff = 0, index = 0;

            qry = "select max(cobranca_parcela) from cobranca where cobranca_propostaId=";
            qry = "select cobranca_id,cobranca_datavencimento,cobranca_parcela from cobranca where cobranca_tipo=0 and cobranca_propostaId=";

            foreach (Contrato contrato in contratos)
            {
                vencimento = DateTime.MinValue;
                proxVencimento = DateTime.MinValue;

                index++;

                cobrancas = LocatorHelper.Instance.ExecuteQuery<Cobranca>(String.Concat(qry, contrato.ID, " order by cobranca_dataVencimento"), typeof(Cobranca), pm);

                if (cobrancas == null) { continue; }

                foreach (Cobranca _cobranca in cobrancas)
                {
                    //a primeira parcela sempre existe, pois é criada com o cadastro do contrato
                    if (_cobranca.Parcela == 1) { continue; }

                    if (_cobranca.Parcela == 2)
                    {
                        proxVencimento = _cobranca.DataVencimento.AddMonths(1);
                        continue;
                    }

                    //reseta as datas para comparação
                    proxVencimento = new DateTime(proxVencimento.Year, proxVencimento.Month, proxVencimento.Day, 0, 0, 0, 0);
                    _cobranca.DataVencimento = new DateTime(_cobranca.DataVencimento.Year,
                        _cobranca.DataVencimento.Month, _cobranca.DataVencimento.Day, 0, 0, 0, 0);

                    if (_cobranca.DataVencimento > proxVencimento &&
                        _cobranca.DataVencimento.Month != proxVencimento.Month)
                    {
                        //TEM FURO NA CADEIA, arrumar
                        //tira a diferenca para saber qtas cobrancas devem ser geradas
                        diff = MonthDifference(proxVencimento, _cobranca.DataVencimento);
                        vencimento = proxVencimento;

                        if (diff < 1) { break; }

                        if (diff > 1) { int j = 0; }

                        for (int i = 1; i <= diff; i++)
                        {
                            cobranca = new Cobranca();
                            cobranca.BeneficiarioEmail = "";
                            cobranca.BeneficiarioId = null;
                            cobranca.ContratoEnderecoCobrancaID = contrato.EnderecoCobrancaID;
                            cobranca.ContratoNumero = contrato.Numero;
                            cobranca.ContratoTitularNome = "";
                            cobranca.DataCriacao = DateTime.Now;

                            cobranca.EstipulanteNome = "";
                            cobranca.FilialNome = "";
                            cobranca.OperadoraID = contrato.OperadoraID;
                            cobranca.OperadoraNome = "";
                            cobranca.Pago = false;
                            cobranca.Parcela = 0;
                            cobranca.PropostaID = contrato.ID;
                            cobranca.Tipo = (Int32)Cobranca.eTipo.Normal;

                            cobranca.DataVencimento = new DateTime(proxVencimento.Year,
                                proxVencimento.Month, proxVencimento.Day, 23, 59, 59, 500);

                            cobranca.Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite, true); //força vigencia

                            if (cobranca.Valor == 0) { err += Convert.ToString(contrato.ID) + ","; break; } //throw new ApplicationException("Erro valorando cobrança"); }

                            pm.Save(cobranca);////////////////////////////////////

                            //incrementa o vencimento
                            if (i == diff)
                                proxVencimento = _cobranca.DataVencimento.AddMonths(1);// proxVencimento.AddMonths(1);
                            else
                                proxVencimento = proxVencimento.AddMonths(1);

                            //index++;
                        }
                    }
                    else
                    {
                        proxVencimento = _cobranca.DataVencimento.AddMonths(1);
                        if (proxVencimento.Day != contrato.Vencimento.Day)
                        {
                            proxVencimento = new DateTime(proxVencimento.Year,
                                    proxVencimento.Month, contrato.Vencimento.Day, 23, 59, 59, 500);
                        }
                    }
                }
            }


            pm.CloseSingleCommandInstance();
            pm.Dispose();

            return null;
        }

        public static IList<Cobranca> CarregarBoletos(Object operadoraId, String numeroContrato)
        {
            String qry = String.Concat("SELECT cobranca_id, cobranca_valor, cobranca_dataVencimento, beneficiario_nome, beneficiario_id, beneficiario_email ",
                "   FROM contrato ",
                "       INNER JOIN endereco ON contrato_enderecoCobrancaId = endereco_id ",
                "       INNER JOIN cobranca ON cobranca_propostaId = contrato_id ",
                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_contratoId = contrato_id ",
                "       INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId = beneficiario_id ",
                "   WHERE ",
                "       contrato_numero = @NumeroContrato AND ",
                "       contrato_operadoraId = ", operadoraId, " AND ",
                "       contratobeneficiario_tipo = 0 AND ",
                "       cobranca_pago = 0 ",
                "   ORDER BY ",
                "       cobranca_dataVencimento ASC");

            String[] pName = new String[] { "@NumeroContrato" };
            String[] pValue = new String[] { numeroContrato };

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Cobranca>(qry, pName, pValue, typeof(Cobranca));
        }

        /// <summary>
        /// Loga a alteração da data de vencimento de uma cobrana, para emissão sem juro e multa.
        /// </summary>
        public static void LogaNovaDataDeVencimentoParaEmissao(Object cobrancaId, DateTime venctoOriginal, DateTime venctoNovo, Object usuarioId, PersistenceManager pm)
        {
        }

        //public static void LogaCobrancaEnviada(Object boletoId
    }

    [DBTable("cobranca_composicao")]
    public class CobrancaComposite : EntityBase, IPersisteableEntity
    {
        public enum eComposicaoTipo : int
        {
            Plano,
            TaxaAssociacao,
            TaxaTabelaValor,
            Adicional,
            Desconto
        }

        #region fields

        Object _id;
        Object _cobrancaId;
        Object _beneficiarioId;
        Int32 _tipo;
        Decimal _valor;

        String _beneficiarioNome;

        #endregion

        #region properties

        [DBFieldInfo("cobrancacomp_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("cobrancacomp_cobranaId", FieldType.Single)]
        public Object CobrancaID
        {
            get { return _cobrancaId; }
            set { _cobrancaId = value; }
        }

        [DBFieldInfo("cobrancacomp_beneficiarioId", FieldType.Single)]
        public Object BeneficiarioID
        {
            get { return _beneficiarioId; }
            set { _beneficiarioId = value; }
        }

        [DBFieldInfo("cobrancacomp_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        [DBFieldInfo("cobrancacomp_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }

        [Joinned("beneficiario_nome")]
        public String BeneficiarioNome
        {
            get { return _beneficiarioNome; }
            set { _beneficiarioNome = value; }
        }

        public String StrTipo
        {
            get
            {
                if ((eComposicaoTipo)_tipo == eComposicaoTipo.Adicional)
                    return "Adicional";
                else if ((eComposicaoTipo)_tipo == eComposicaoTipo.Desconto)
                    return "Desconto";
                else if ((eComposicaoTipo)_tipo == eComposicaoTipo.Plano)
                    return "Plano";
                else if ((eComposicaoTipo)_tipo == eComposicaoTipo.TaxaAssociacao)
                    return "Taxa associativa";
                else
                    return "Taxa (tabela)";
            }
        }

        #endregion

        public CobrancaComposite() { }
        public CobrancaComposite(Object cobrancaId, Object beneficiarioId, eComposicaoTipo tipo, Decimal valor)
        {
            _cobrancaId = cobrancaId;
            _beneficiarioId = beneficiarioId;
            _tipo = Convert.ToInt32(tipo);
            _valor = valor;
        }

        public static void Salvar(Object cobrancaId, IList<CobrancaComposite> lista, PersistenceManager pm)
        {
            StringBuilder sb = new StringBuilder();

            if (lista == null) { return; }
            foreach (CobrancaComposite compos in lista)
            {
                sb.Append("INSERT INTO cobranca_composicao (cobrancacomp_cobranaId,cobrancacomp_beneficiarioId,cobrancacomp_tipo,cobrancacomp_valor) VALUES (");
                sb.Append(cobrancaId); sb.Append(", ");

                if (compos.BeneficiarioID != null)
                    sb.Append(compos.BeneficiarioID);
                else
                    sb.Append("NULL");
                sb.Append(", ");

                sb.Append(Convert.ToInt32(compos.Tipo));
                sb.Append(", '");
                sb.Append(compos.Valor.ToString(new System.Globalization.CultureInfo("en-US")));
                sb.Append("') ;");
            }

            try
            {
                NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
            }
            catch
            {
            }

            sb.Remove(0, sb.Length);
            sb = null;
        }

        #region entity base methods

        public void Salvar(PersistenceManager pm)
        {
            base.Salvar(this);
        }

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static void Remover(Object cobrancaId, PersistenceManager pm)
        {
            String cmd = "delete from cobranca_composicao where cobrancacomp_cobranaId=" + cobrancaId;
            NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);
        }

        public static IList<CobrancaComposite> Carregar(Object cobrancaId)
        {
            return Carregar(cobrancaId, null);
        }

        public static IList<CobrancaComposite> Carregar(Object cobrancaId, PersistenceManager pm)
        {
            String qry = String.Concat("cobranca_composicao.*,beneficiario_nome from cobranca_composicao left join beneficiario on cobrancacomp_beneficiarioId=beneficiario_id where cobrancacomp_cobranaId=", cobrancaId);

            return LocatorHelper.Instance.ExecuteQuery<CobrancaComposite>(qry, typeof(CobrancaComposite), pm);
        }
    }

    [DBTable("contrato")]
    public class Contrato : EntityBase, IPersisteableEntity
    {
        public enum eTipoAcomodacao : int
        {
            quartoComun = 0,
            quartoParticular,
            indefinido
        }
        public enum eStatus : int
        {
            Normal,
            /// <summary>
            /// Proposta importada com o status 'Nao Implantada'.
            /// </summary>
            NaoImplantadoNaImportacao
        }

        #region fields

        Object _id;
        Object _filialId;
        Object _estipulanteId;
        Object _operadoraId;
        Object _contratoadmId;
        Object _planoId;
        Object _donoId; //corretor que vendeu a proposta
        Object _usuarioId;

        String _corretorTerceiroNome;
        String _corretorTerceiroCPF;
        String _superiorTerceiroNome;
        String _superiorTerceiroCPF;

        Object _operadorTmktId;
        Object _tipoContratoId;
        Object _enderecoReferenciaId;
        Object _enderecoCobrancaId;
        String _numero;
        Object _numeroId; //é o id da proposta, do impresso registrado no almoxarifado.
        String _vingencia;
        String _numeroMatricula;
        Decimal _valorAto;
        Boolean _adimplente;
        Boolean _cobrarTaxaAssociativa;
        DateTime _data;
        DateTime _dataCancelamento;
        String _emailCobranca;

        String _responsavelNome;
        String _responsavelCPF;
        String _responsavelRG;
        DateTime _responsavelDataNascimento;
        String _responsavelSexo;
        Object _responsavelParentescoId;
        Int32 _tipoAcomodacao;

        DateTime _admissao;
        DateTime _vigencia;
        DateTime _vencimento;

        String _empresaAnterior;
        String _empresaAnteriorMatricula;
        Int32 _empresaAnteriorTempo;
        Boolean _rascunho;
        Boolean _cancelado;
        Boolean _inativo;
        Boolean _pendente;
        String _obs;
        DateTime _alteracao;
        Int32 _codigoCobranca;
        Decimal _desconto;
        Int32 _status;

        bool _legado;

        String _planoDescricao;
        String _operadoraDescricao;
        String _beneficiarioTitularNome;
        String _beneficiarioTitularNomeMae;
        DateTime _beneficiarioTitularDataNascimento;
        String _beneficiarioTipo;
        String _beneficiarioCpf;

        Object _contratobeneficiario_beneficiarioId;

        String _empresaCobrancaNome;

        #endregion

        #region propriedades

        [DBFieldInfo("contrato_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("contrato_filialId", FieldType.Single)]
        public Object FilialID
        {
            get { return _filialId; }
            set { _filialId = value; }
        }

        [DBFieldInfo("contrato_estipulanteId", FieldType.Single)]
        public Object EstipulanteID
        {
            get { return _estipulanteId; }
            set { _estipulanteId = value; }
        }

        [DBFieldInfo("contrato_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId = value; }
        }

        [DBFieldInfo("contrato_contratoAdmId", FieldType.Single)]
        public Object ContratoADMID
        {
            get { return _contratoadmId; }
            set { _contratoadmId = value; }
        }

        [DBFieldInfo("contrato_planoId", FieldType.Single)]
        public Object PlanoID
        {
            get { return _planoId; }
            set { _planoId = value; }
        }

        [DBFieldInfo("contrato_legado", FieldType.Single)]
        public bool Legado
        {
            get { return _legado; }
            set { _legado = value; }
        }

        [DBFieldInfo("contrato_tipoContratoId", FieldType.Single)]
        public Object TipoContratoID
        {
            get { return _tipoContratoId; }
            set { _tipoContratoId = value; }
        }

        /// <summary>
        /// Corretor que vendeu a proposta
        /// </summary>
        [DBFieldInfo("contrato_donoId", FieldType.Single)]
        public Object DonoID
        {
            get { return _donoId; }
            set { _donoId = value; }
        }

        [DBFieldInfo("contrato_corretorTerceiroNome", FieldType.Single)]
        public String CorretorTerceiroNome
        {
            get { return _corretorTerceiroNome; }
            set { _corretorTerceiroNome = value; }
        }
        [DBFieldInfo("contrato_corretorTerceiroCPF", FieldType.Single)]
        public String CorretorTerceiroCPF
        {
            get { return _corretorTerceiroCPF; }
            set { _corretorTerceiroCPF = value; }
        }

        [DBFieldInfo("contrato_superiorTerceiroNome", FieldType.Single)]
        public String SuperiorTerceiroNome
        {
            get { return _superiorTerceiroNome; }
            set { _superiorTerceiroNome = value; }
        }
        [DBFieldInfo("contrato_superiorTerceiroCPF", FieldType.Single)]
        public String SuperiorTerceiroCPF
        {
            get { return _superiorTerceiroCPF; }
            set { _superiorTerceiroCPF = value; }
        }

        /// <summary>
        /// Operador de telemarketing que participou ou propiciou a venda.
        /// </summary>
        [DBFieldInfo("contrato_operadorTmktId", FieldType.Single)]
        public Object OperadorTmktID
        {
            get { return _operadorTmktId; }
            set { _operadorTmktId = value; }
        }

        [DBFieldInfo("contrato_enderecoReferenciaId", FieldType.Single)]
        public Object EnderecoReferenciaID
        {
            get { return _enderecoReferenciaId; }
            set { _enderecoReferenciaId = value; }
        }

        [DBFieldInfo("contrato_enderecoCobrancaId", FieldType.Single)]
        public Object EnderecoCobrancaID
        {
            get { return _enderecoCobrancaId; }
            set { _enderecoCobrancaId = value; }
        }

        /// <summary>
        /// Número do contrato.
        /// </summary>
        [DBFieldInfo("contrato_numero", FieldType.Single)]
        public String Numero
        {
            get { return _numero; }
            set { _numero = value; }
        }

        [DBFieldInfo("contrato_emailCobranca", FieldType.Single)]
        public String EmailCobranca
        {
            get { return _emailCobranca; }
            set { _emailCobranca = value; }
        }

        /// <summary>
        /// É o ID da proposta, do impresso registrado no almoxarifado.
        /// </summary>
        [DBFieldInfo("contrato_numeroId", FieldType.Single)]
        public Object NumeroID
        {
            get { return _numeroId; }
            set { _numeroId = value; }
        }

        [Obsolete("Em desuso.", true)]
        [DBFieldInfo("contrato_vingencia", FieldType.Single)]
        public String Vingencia
        {
            get { return _vingencia; }
            set { _vingencia = value; }
        }

        /// <summary>
        /// Número da matrícula.
        /// </summary>
        [DBFieldInfo("contrato_numeroMatricula", FieldType.Single)]
        public String NumeroMatricula
        {
            get { return _numeroMatricula; }
            set { _numeroMatricula = value; }
        }

        [DBFieldInfo("contrato_valorAto", FieldType.Single)]
        public Decimal ValorAto
        {
            get { return _valorAto; }
            set { _valorAto = value; }
        }

        /// <summary>
        /// Informa se o contrato está adimplente.
        /// </summary>
        [DBFieldInfo("contrato_adimplente", FieldType.Single)]
        public Boolean Adimplente
        {
            get { return _adimplente; }
            set { _adimplente = value; }
        }

        [DBFieldInfo("contrato_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Usuário que digitou a proposta.
        /// </summary>
        [DBFieldInfo("contrato_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId = value; }
        }

        [DBFieldInfo("contrato_dataCancelamento", FieldType.Single)]
        public DateTime DataCancelamento
        {
            get { return _dataCancelamento; }
            set { _dataCancelamento = value; }
        }

        [DBFieldInfo("contrato_responsavelNome", FieldType.Single)]
        public String ResponsavelNome
        {
            get { return _responsavelNome; }
            set { _responsavelNome = value; }
        }

        [DBFieldInfo("contrato_responsavelCPF", FieldType.Single)]
        public String ResponsavelCPF
        {
            get { return _responsavelCPF; }
            set { _responsavelCPF = value; }
        }

        [DBFieldInfo("contrato_responsavelRG", FieldType.Single)]
        public String ResponsavelRG
        {
            get { return _responsavelRG; }
            set { _responsavelRG = value; }
        }

        [DBFieldInfo("contrato_responsavelDataNascimento", FieldType.Single)]
        public DateTime ResponsavelDataNascimento
        {
            get { return _responsavelDataNascimento; }
            set { _responsavelDataNascimento = value; }
        }

        [DBFieldInfo("contrato_responsavelSexo", FieldType.Single)]
        public String ResponsavelSexo
        {
            get { return _responsavelSexo; }
            set { _responsavelSexo = value; }
        }

        [DBFieldInfo("contrato_responsavelParentescoId", FieldType.Single)]
        public Object ResponsavelParentescoID
        {
            get { return _responsavelParentescoId; }
            set { _responsavelParentescoId = value; }
        }

        [DBFieldInfo("contrato_tipoAcomodacao", FieldType.Single)]
        public Int32 TipoAcomodacao
        {
            get { return _tipoAcomodacao; }
            set { _tipoAcomodacao = value; }
        }

        [DBFieldInfo("contrato_admissao", FieldType.Single)]
        public DateTime Admissao
        {
            get { return _admissao; }
            set { _admissao = value; }
        }

        [DBFieldInfo("contrato_vigencia", FieldType.Single)]
        public DateTime Vigencia
        {
            get { return _vigencia; }
            set { _vigencia = value; }
        }

        [DBFieldInfo("contrato_vencimento", FieldType.Single)]
        public DateTime Vencimento
        {
            get { return _vencimento; }
            set { _vencimento = value; }
        }

        [DBFieldInfo("contrato_empresaAnterior", FieldType.Single)]
        public String EmpresaAnterior
        {
            get { return _empresaAnterior; }
            set { _empresaAnterior = value; }
        }

        [DBFieldInfo("contrato_empresaAnteriorMatricula", FieldType.Single)]
        public String EmpresaAnteriorMatricula
        {
            get { return _empresaAnteriorMatricula; }
            set { _empresaAnteriorMatricula = value; }
        }

        [DBFieldInfo("contrato_empresaAnteriorTempo", FieldType.Single)]
        public Int32 EmpresaAnteriorTempo
        {
            get { return _empresaAnteriorTempo; }
            set { _empresaAnteriorTempo = value; }
        }

        [DBFieldInfo("contrato_rascunho", FieldType.Single)]
        public Boolean Rascunho
        {
            get { return _rascunho; }
            set { _rascunho = value; }
        }

        [DBFieldInfo("contrato_cancelado", FieldType.Single)]
        public Boolean Cancelado
        {
            get { return _cancelado; }
            set { _cancelado = value; }
        }

        [DBFieldInfo("contrato_inativo", FieldType.Single)]
        public Boolean Inativo
        {
            get { return _inativo; }
            set { _inativo = value; }
        }

        /// <summary>
        /// Quando um novo contrato é cadastrado, e um ou mais beneficiários têm algum item de saúde marcado,
        /// a proposta fica pendente, dependendo de análise técnica.
        /// </summary>
        [DBFieldInfo("contrato_pendente", FieldType.Single)]
        public Boolean Pendente
        {
            get { return _pendente; }
            set { _pendente = value; }
        }

        [DBFieldInfo("contrato_cobrarTaxaAssociativa", FieldType.Single)]
        public Boolean CobrarTaxaAssociativa
        {
            get { return _cobrarTaxaAssociativa; }
            set { _cobrarTaxaAssociativa = value; }
        }

        [DBFieldInfo("contrato_obs", FieldType.Single)]
        public String Obs
        {
            get { return _obs; }
            set { _obs = value; }
        }

        [DBFieldInfo("contrato_codcobranca", FieldType.Single)]
        public Int32 CodCobranca
        {
            get { return _codigoCobranca; }
            set { _codigoCobranca = value; }
        }

        [DBFieldInfo("contrato_alteracao", FieldType.Single)]
        public DateTime Alteracao
        {
            get { return _alteracao; }
            set { _alteracao = value; }
        }

        [DBFieldInfo("contrato_desconto", FieldType.Single)]
        public Decimal Desconto
        {
            get { return _desconto; }
            set { _desconto = value; }
        }

        [DBFieldInfo("contrato_status", FieldType.Single)]
        public Int32 Status
        {
            get { return _status; }
            set { _status = value; }
        }

        [Joinned("plano_descricao")]
        public String PlanoDescricao
        {
            get { return _planoDescricao; }
            set { _planoDescricao = value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraDescricao
        {
            get { return _operadoraDescricao; }
            set { _operadoraDescricao = value; }
        }

        [Joinned("contratobeneficiario_beneficiarioId")]
        public Object ContratoBeneficiarioId
        {
            get { return _contratobeneficiario_beneficiarioId; }
            set { _contratobeneficiario_beneficiarioId = value; }
        }

        [Joinned("beneficiario_nome")]
        public String BeneficiarioTitularNome
        {
            get { return _beneficiarioTitularNome; }
            set { _beneficiarioTitularNome = value; }
        }

        [Joinned("beneficiario_nomeMae")]
        public String BeneficiarioTitularNomeMae
        {
            get { return _beneficiarioTitularNomeMae; }
            set { _beneficiarioTitularNomeMae = value; }
        }

        [Joinned("beneficiario_dataNascimento")]
        public DateTime BeneficiarioTitularDataNascimento
        {
            get { return _beneficiarioTitularDataNascimento; }
            set { _beneficiarioTitularDataNascimento = value; }
        }

        [Joinned("beneficiario_cpf")]
        public String BeneficiarioCPF
        {
            get { return _beneficiarioCpf; }
            set { _beneficiarioCpf = value; }
        }

        public String BeneficiarioCPFFormatado
        {
            get
            {
                if (String.IsNullOrEmpty(_beneficiarioCpf)) { return _beneficiarioCpf; }
                return String.Format(@"{0:000\.000\.000\-00}", Convert.ToInt64(_beneficiarioCpf));
            }
        }

        /// <summary>
        /// Se o beneficiário é titular ou dependente.
        /// </summary>
        [Joinned("contratobeneficiario_tipo")]
        public String TipoParticipacaoContrato
        {
            get { return _beneficiarioTipo; }
            set { _beneficiarioTipo = value; }
        }

        [Joinned("empresa_nome")]
        public String EmpresaCobranca
        {
            get { return _empresaCobrancaNome; }
            set { _empresaCobrancaNome = value; }
        }

        /// <summary>
        /// Condição para retornar, em uma query sql, apenas contratos não cancelados, ativos e que não sejam rascunhos.
        /// </summary>
        internal static String CondicaoBasicaQuery
        {
            get
            {
                return " contrato_cancelado <> 1 AND contrato_inativo <> 1 AND contrato_rascunho <> 1 "; //contrato_adimplente=1 AND 
            }
        }

        #endregion

        public ContratoBeneficiario ContratoBeneficiario
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public Plano Plano
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public Beneficiario Beneficiario
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Contrato() { _legado = false; _inativo = false; _cancelado = false; _adimplente = true; _pendente = false; _responsavelDataNascimento = DateTime.MinValue; _alteracao = DateTime.Now; _status = 0; }
        public Contrato(Object id) : this() { _id = id; }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            String query = "delete from contrato_beneficiario WHERE contratobeneficiario_contratoId=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from contrato_valor where contratovalor_contratoId=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from contrato_plano_historico where contratoplano_contratoid=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from cobranca where cobranca_propostaId=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from contrato_beneficiario where contratobeneficiario_contratoid=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            query = "delete from adicional_beneficiario where adicionalbeneficiario_propostaid=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

            pm.Remove(this);

            pm.CloseSingleCommandInstance();
            pm = null;
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public static Contrato CarregarParcial(Object id, PersistenceManager pm)
        {
            String qry = "contrato_id,contrato_numero, contrato_operadoraid, contrato_estipulanteId, contrato_cobrarTaxaAssociativa, contrato_contratoAdmId, contrato_admissao, contrato_vigencia,contrato_codcobranca,contrato_inativo,contrato_cancelado,contrato_dataCancelamento,contrato_responsavelNome,contrato_responsavelCPF FROM contrato WHERE contrato_id=" + id; ;
            IList<Contrato> ret = LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato), pm);
            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static Contrato CarregarParcial(String propostaNumero, Object operadoraId)
        {
            return CarregarParcial(propostaNumero, operadoraId, null);
        }
        public static Contrato CarregarParcial(String propostaNumero, Object operadoraId, PersistenceManager pm)
        {
            String qry = "contrato_id, contrato_numero, contrato_contratoAdmId,contrato_admissao,contrato_planoId,contrato_inativo,contrato_status FROM contrato WHERE contrato_numero=@numero AND contrato_operadoraId=" + operadoraId;

            String[] names = new String[1] { "@numero" };
            String[] value = new String[1] { propostaNumero };

            IList<Contrato> ret = LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, names, value, typeof(Contrato), pm);
            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static Contrato CarregarParcialPorCodCobranca(Object codCobranca, PersistenceManager pm)
        {
            IList<Contrato> lista = LocatorHelper.Instance.ExecuteQuery<Contrato>("contrato_id, contrato_operadoraId, contrato_numero, contrato_contratoAdmId, contrato_admissao, contrato_inativo, contrato_cancelado, contrato_adimplente, contrato_datacancelamento FROM contrato WHERE contrato_codCobranca=" + codCobranca, typeof(Contrato));

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        #endregion

        public static Contrato CarregarPorParametros(String numeroContrato, Object operadoraId, PersistenceManager pm)
        {
            String qry = String.Concat("SELECT * ",
                "   FROM contrato ",
                "   WHERE ",
                "       contrato_numero = @NumeroContrato AND ",
                "       contrato_operadoraId = ", operadoraId);

            String[] pName = new String[] { "@NumeroContrato" };
            String[] pValue = new String[] { numeroContrato };

            IList<Contrato> ret = LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, pName, pValue, typeof(Contrato), pm);

            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static IList<Contrato> CarregarPorParametros(String numeroContrato, Object operadoraId, DateTime vigencia, String titularCpf, String titularNome)
        {
            String qry = String.Concat("SELECT contrato.*, beneficiario_nome, beneficiario_cpf, beneficiario_nomeMae, beneficiario_dataNascimento ",
                "   FROM contrato ",
                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_contratoId=contrato_id AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "   WHERE ",
                "       contrato_operadoraId = ", operadoraId);

            List<String> paramNames = new List<String>();
            List<String> paramValues = new List<String>();

            String[] pName = null;
            String[] pValue = null;

            if (!String.IsNullOrEmpty(numeroContrato))
            {
                qry += " AND contrato_numero = @NumeroContrato ";
                paramNames.Add("@NumeroContrato");
                paramValues.Add(numeroContrato);
            }

            if (vigencia != DateTime.MinValue)
            {
                qry = String.Concat(qry, " AND DAY(contrato_vigencia)=", vigencia.Day, " AND MONTH(contrato_vigencia)=", vigencia.Month, " AND YEAR(contrato_vigencia)=", vigencia.Year);
            }

            if (!String.IsNullOrEmpty(titularCpf))
            {
                qry = String.Concat(qry, " AND beneficiario_cpf='", titularCpf, "'");
            }

            if (!String.IsNullOrEmpty(titularNome))
            {
                qry += " AND beneficiario_nome LIKE @TitularNome ";
                paramNames.Add("@TitularNome");
                paramValues.Add(titularNome + "%");
            }

            if (paramNames.Count > 0)
            {
                pName = paramNames.ToArray();
                pValue = paramValues.ToArray();
            }

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, pName, pValue, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorNumero(String numero, PersistenceManager pm)
        {
            String[] values = null;
            String[] pnames = null;

            pnames = new String[1] { "@contrato_numero" };
            values = new String[1] { numero };

            String query = String.Concat("contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE contrato_numero=@contrato_numero",
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato), pm);
        }

        public static IList<Contrato> CarregarPorParametros(String numero, String benficiarioNome)
        {
            String whereAnd = "";
            String[] values = null;
            String[] pnames = null;

            if (!String.IsNullOrEmpty(numero))
            {
                //numero = String.Format("{0:0000000000}", Convert.ToInt32(numero));
                whereAnd = " AND contrato_numero=@contrato_numero ";
                pnames = new String[2] { "@contrato_numero", "@beneficiario_nome" };
                values = new String[2] { numero, "%" + benficiarioNome + "%" };
            }
            else
            {
                pnames = new String[1] { "@beneficiario_nome" };
                values = new String[1] { "%" + benficiarioNome + "%" };
            }

            String query = String.Concat("contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorParametros(String numero, String benficiarioNome, String codCobranca)
        {
            String whereAnd = "";
            String[] pnames = new String[3] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca" };
            String[] values = { numero, "%" + benficiarioNome + "%", codCobranca };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd = " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            String query = String.Concat("contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorParametros(String numero, String benficiarioNome, String codCobranca, String protocolo)
        {
            String whereAnd = "";
            String joinAtendimento = "";
            String[] pnames = new String[4] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca", "@atendimento_id" };
            String[] values = { numero, "%" + benficiarioNome + "%", codCobranca, protocolo };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd = " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            if (!String.IsNullOrEmpty(protocolo))
            {
                whereAnd += " AND atendimento_id=@atendimento_id ";
                joinAtendimento = " INNER JOIN _atendimento on atendimento_propostaId=contrato_id ";
            }

            String query = String.Concat("TOP 50 contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static DataTable DTCarregarPorParametros(String numero, String benficiarioNome, String codCobranca, String protocolo)
        {
            return DTCarregarPorParametros(numero, benficiarioNome, codCobranca, protocolo, null, false, null, false);
        }

        public static DataTable DTCarregarPorParametros(String numero, String benficiarioNome, String codCobranca, String protocolo, Object empresaCobrancaId, Boolean somenteInativos, String cpf, Boolean somenteAtivos)
        {
            if (cpf == null) { cpf = ""; }

            String whereAnd = "";
            String joinAtendimento = "";
            String[] pnames = new String[5] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca", "@atendimento_id", "@beneficiario_cpf" };
            String[] values = { numero, "%" + benficiarioNome + "%", codCobranca, protocolo, cpf };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd = " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            if (!String.IsNullOrEmpty(protocolo))
            {
                whereAnd += " AND atendimento_id=@atendimento_id ";
                joinAtendimento = " INNER JOIN _atendimento on atendimento_propostaId=contrato_id ";
            }

            if (empresaCobrancaId != null)
            {
                whereAnd += " AND contrato_empresaCobrancaId= " + empresaCobrancaId;
            }

            if (somenteInativos)
            {
                whereAnd += " AND (contrato_cancelado=1 or contrato_inativo=1) ";
            }

            if (somenteAtivos)
            {
                whereAnd += " AND ( (contrato_cancelado=0 or contrato_cancelado is null ) and ( contrato_inativo=0 or contrato_inativo is null )) ";
            }

            if (!String.IsNullOrEmpty(cpf))
            {
                whereAnd += " and beneficiario_cpf=@beneficiario_cpf ";
            }

            if (string.IsNullOrEmpty(benficiarioNome)) benficiarioNome = "";

            String query = String.Concat("select TOP 60 contrato_id as ID, empresa_nome as EmpresaCobranca, contrato_numero as Numero, contrato_rascunho as Rascunho, contrato_cancelado as Cancelado, contrato_inativo as Inativo, plano_descricao as PlanoDescricao, operadora_nome as OperadoraNome, beneficiario_nome as BeneficiarioTitularNome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                " left join cobranca_empresa on contrato_empresaCobrancaId = empresa_id ",
                "WHERE (beneficiario_nome LIKE '%", benficiarioNome.Replace("'", ""), "%') ", whereAnd,//"WHERE (beneficiario_nome LIKE @beneficiario_nome) ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery(query, pnames, values).Tables[0];
        }

        /// <summary>
        /// Carrega o contrato e seus beneficiários (todos os beneficiários ativos, titular e dependentes)
        /// </summary>
        /// <param name="operadoraId">ID da operadora à qual pertence o contrato.</param>
        /// <param name="numeroContrato">Número do contrato.</param>
        /// <param name="cpf">TODO: Se fornecido um cpf, somente o beneficiário dono dele será carregado.</param>
        /// <returns></returns>
        public static IList<Contrato> Carregar(Object operadoraId, String numeroContrato, String cpf)
        {
            String qry = String.Concat("SELECT contrato.*, plano_descricao, operadora_nome, contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, beneficiario_nome, beneficiario_cpf ",
                "   FROM contrato ",
                "       INNER JOIN plano ON contrato_planoId=plano_id ",
                "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 ",
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "   WHERE ",
                "       contrato_numero=@NumeroContrato AND ",
                "       contrato_operadoraId=", operadoraId);

            String[] pName = new String[] { "@NumeroContrato" };
            String[] pValue = new String[] { numeroContrato };

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, pName, pValue, typeof(Contrato));
        }

        public static IList<Contrato> Carregar(Object contratoId)
        {
            String qry = String.Concat("SELECT contrato.*, plano_descricao, operadora_nome, contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, beneficiario_nome, beneficiario_cpf ",
                "   FROM contrato ",
                "       INNER JOIN plano ON contrato_planoId=plano_id ",
                "       INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                "       INNER JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 ",
                "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "   WHERE ",
                "       contrato_id=", contratoId);

            return LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorBeneficiário(Object beneficiarioId)
        {
            String query = String.Concat("contrato_id, contrato_numero, empresa_nome, operadora_nome, contrato_data, contratobeneficiario_tipo FROM contrato ",
                "INNER JOIN contrato_beneficiario ON contrato_id = contratobeneficiario_contratoId ",
                "INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id ",
                "INNER JOIN operadora ON contrato_operadoraId=operadora_id ",
                "LEFT JOIN cobranca_empresa ON contrato_empresaCobrancaId=empresa_id ",
                "WHERE beneficiario_id=", beneficiarioId, " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteQuery<Contrato>(query, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome)
        {
            return CarregarPorOperadoraID(operadoraId, numero, benficiarioNome, null);
        }

        public static IList<Contrato> CarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome, String codCobranca)
        {
            String whereAnd = "";
            if (codCobranca == null) { codCobranca = ""; }
            if (benficiarioNome == null) { benficiarioNome = ""; }
            String[] pnames = new String[3] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca" };
            String[] values = new String[3] { numero, "%" + benficiarioNome + "%", codCobranca };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd += " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            String query = String.Concat("TOP 100 contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static IList<Contrato> CarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome, String codCobranca, String protocolo)
        {
            String whereAnd = "";
            String joinAtendimento = "";
            if (benficiarioNome == null) { benficiarioNome = ""; }
            String[] pnames = new String[4] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca", "@atendimento_id" };
            String[] values = new String[4] { numero, "%" + benficiarioNome + "%", codCobranca, protocolo };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd += " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            if (!String.IsNullOrEmpty(protocolo))
            {
                whereAnd += " AND atendimento_id=@atendimento_id ";
                joinAtendimento = " INNER JOIN _atendimento on atendimento_propostaId=contrato_id ";
            }

            String query = String.Concat("TOP 60 contrato.*, plano_descricao, operadora_nome, beneficiario_nome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                "WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>
                (query, pnames, values, typeof(Contrato));
        }

        public static DataTable DTCarregarPorOperadoraID(Object operadoraId, String numeroProposta, String benficiarioNome)
        {
            String whereAnd = "";
            String joinAtendimento = "";
            if (benficiarioNome == null) { benficiarioNome = ""; }

            String[] pnames = null;
            String[] values = null;

            if (!string.IsNullOrEmpty(numeroProposta))
            {
                pnames = new String[2] { "@contrato_numero", "@beneficiario_nome" };
                values = new String[2] { numeroProposta, "'%" + benficiarioNome + "%'" };

                //whereAnd = " AND contrato_numero=@contrato_numero ";
                whereAnd = string.Concat(" AND contrato_numero='", numeroProposta, "' ");
            }
            else
            {
                pnames = new String[1] { "@beneficiario_nome" };
                values = new String[1] { "'%" + benficiarioNome + "%'" }; //"%" +  + "%"
            }

            String query = String.Concat("select TOP 60 contrato_id as ID, empresa_nome as EmpresaCobranca, contrato_numero as Numero, contrato_rascunho as Rascunho, contrato_cancelado as Cancelado, contrato_inativo as Inativo, plano_descricao as PlanoDescricao, operadora_nome as OperadoraNome, beneficiario_nome as BeneficiarioTitularNome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                " left join cobranca_empresa on contrato_empresaCobrancaId = empresa_id ",
                //"WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                //" ORDER BY contrato_data DESC");
                "WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE '%", benficiarioNome.Replace("'", ""), "%'", whereAnd,
                " ORDER BY contrato_data DESC");

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(EntityBase._connString))
            {
                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                //for (int i = 0; i < pnames.Length; i++)
                //{
                //    adp.SelectCommand.Parameters.AddWithValue(pnames[i], values[i]);
                //}

                adp.Fill(dt);
                adp.Dispose();
                conn.Close();
            }

            return dt;
        }

        public static DataTable DTCarregarPorOperadoraID_LEGADO(Object operadoraId, String numeroProposta, String benficiarioNome)
        {
            String whereAnd = "";
            String joinAtendimento = "";
            if (benficiarioNome == null) { benficiarioNome = ""; }

            String[] pnames = null;
            String[] values = null;

            if (!string.IsNullOrEmpty(numeroProposta))
            {
                pnames = new String[2] { "@contrato_numero", "@beneficiario_nome" };
                values = new String[2] { numeroProposta, "'%" + benficiarioNome + "%'" };

                //whereAnd = " AND contrato_numero=@contrato_numero ";
                whereAnd = string.Concat(" AND (contrato_numero='", numeroProposta, "' OR beneficiario_matriculaAssociativa='", numeroProposta, "') ");
            }
            else
            {
                pnames = new String[1] { "@beneficiario_nome" };
                values = new String[1] { "'%" + benficiarioNome + "%'" }; //"%" +  + "%"
            }

            String query = String.Concat("select TOP 60 contrato_id as ID, empresa_nome as EmpresaCobranca, contrato_numero as Numero, contrato_rascunho as Rascunho, contrato_cancelado as Cancelado, contrato_inativo as Inativo, plano_descricao as PlanoDescricao, operadora_nome as OperadoraNome, beneficiario_nome as BeneficiarioTitularNome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                " left join cobranca_empresa on contrato_empresaCobrancaId = empresa_id ",
                //"WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                //" ORDER BY contrato_data DESC");
                "WHERE contrato_legado=1 and operadora_id=", operadoraId, " AND beneficiario_nome LIKE '%", benficiarioNome.Replace("'", ""), "%'", whereAnd,
                " ORDER BY contrato_data DESC");

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(EntityBase._connString))
            {
                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                //for (int i = 0; i < pnames.Length; i++)
                //{
                //    adp.SelectCommand.Parameters.AddWithValue(pnames[i], values[i]);
                //}

                adp.Fill(dt);
                adp.Dispose();
                conn.Close();
            }

            return dt;
        }

        public static DataTable DTCarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome, String codCobranca, String protocolo)
        {
            return DTCarregarPorOperadoraID(operadoraId, numero, benficiarioNome, codCobranca, protocolo, null, false, null, false);
        }

        public static DataTable DTCarregarPorOperadoraID(Object operadoraId, String numero, String benficiarioNome, String codCobranca, String protocolo, Object empresaCobrancaId, Boolean somenteInativos, String cpf, Boolean somenteAtivos)
        {
            if (cpf == null) { cpf = ""; }

            String whereAnd = "";
            String joinAtendimento = "";
            if (benficiarioNome == null) { benficiarioNome = ""; }
            String[] pnames = new String[5] { "@contrato_numero", "@beneficiario_nome", "@contrato_codcobranca", "@atendimento_id", "@beneficiario_cpf" };
            String[] values = new String[5] { numero, "%" + benficiarioNome + "%", codCobranca, protocolo, cpf };

            if (!String.IsNullOrEmpty(numero))
            {
                whereAnd = " AND contrato_numero=@contrato_numero ";
            }

            if (!String.IsNullOrEmpty(codCobranca))
            {
                whereAnd += " AND contrato_codcobranca=@contrato_codcobranca ";
            }

            if (!String.IsNullOrEmpty(protocolo))
            {
                whereAnd += " AND atendimento_id=@atendimento_id ";
                joinAtendimento = " INNER JOIN _atendimento on atendimento_propostaId=contrato_id ";
            }

            if (empresaCobrancaId != null)
            {
                whereAnd += " AND contrato_empresaCobrancaId=" + empresaCobrancaId;
            }

            if (somenteInativos)
            {
                whereAnd += " AND (contrato_cancelado=1 or contrato_inativo=1) ";
            }

            if (somenteAtivos)
            {
                whereAnd += " AND ( (contrato_cancelado=0 or contrato_cancelado is null ) and ( contrato_inativo=0 or contrato_inativo is null )) ";
            }

            if (!String.IsNullOrEmpty(cpf))
            {
                whereAnd += " AND beneficiario_cpf=@beneficiario_cpf ";
            }

            String query = String.Concat("select TOP 60 contrato_id as ID, empresa_nome as EmpresaCobranca, contrato_numero as Numero, contrato_rascunho as Rascunho, contrato_cancelado as Cancelado, contrato_inativo as Inativo, plano_descricao as PlanoDescricao, operadora_nome as OperadoraNome, beneficiario_nome as BeneficiarioTitularNome ",
                "FROM contrato ",
                "INNER JOIN plano ON contrato_planoId=plano_id ",
                "INNER JOIN operadora ON contrato_operadoraid=operadora_id ",
                joinAtendimento,
                " LEFT JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                " LEFT JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                " left join cobranca_empresa on contrato_empresaCobrancaId = empresa_id ",
                "WHERE operadora_id=", operadoraId, " AND beneficiario_nome LIKE @beneficiario_nome ", whereAnd,
                " ORDER BY contrato_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery(query, pnames, values).Tables[0];
        }

        /// <summary>
        /// Carrega contratos inadimplentes levando em consideração cobranças em aberto, e não a propriedade Adimplente do objeto Contrato.
        /// </summary>
        /// <returns>Contratos inadimplentes.</returns>
        public static IList<Contrato> BuscarECarregarInadimplentes()
        {
            return BuscarECarregarInadimplentes_PORCOBRANCA(null);
        }
        /// <summary>
        /// Carrega contratos inadimplentes levando em consideração cobranças em aberto, e não a propriedade Adimplente do objeto Contrato.
        /// </summary>
        /// <param name="pm">Objeto PersistenceManager participante de uma transação.</param>
        /// <returns>Contratos inadimplentes.</returns>
        public static IList<Contrato> BuscarECarregarInadimplentes_PORCOBRANCA(PersistenceManager pm)
        {
            String qry = String.Concat("SELECT DISTINCT(cobranca_propostaId) AS ContratoID, contrato_id,contrato_estipulanteId,contrato_operadoraId,contrato_contratoAdmId,contrato_planoId,contrato_tipoContratoId,contrato_donoId,contrato_enderecoReferenciaId,contrato_enderecoCobrancaId,contrato_numero,contrato_numeroId,contrato_adimplente,contrato_cobrarTaxaAssociativa, contrato_tipoAcomodacao,contrato_admissao,contrato_vigencia,contrato_vencimento ",
                "FROM cobranca ",
                "   INNER JOIN contrato ON cobranca_propostaId=contrato_id ",
                "WHERE ",
                "   contrato_cancelado <> 1 AND ",
                "   contrato_rascunho <> 1 AND ",
                "   cobranca_pago=0 AND ",
                "   cobranca_datavencimento IS NOT NULL AND ",
                " cobranca_datavencimento < GETDATE()");

            using (DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0])
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    List<Contrato> lista = new List<Contrato>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Contrato contrato = new Contrato(dr["contrato_id"]);
                        contrato.EstipulanteID = dr["contrato_estipulanteId"];
                        contrato.OperadoraID = dr["contrato_operadoraId"];
                        contrato.ContratoADMID = dr["contrato_contratoAdmId"];
                        contrato.PlanoID = dr["contrato_planoId"];
                        contrato.TipoContratoID = dr["contrato_tipoContratoId"];
                        contrato.DonoID = dr["contrato_donoId"];

                        lista.Add(contrato);
                    }

                    return lista;
                }
                else
                    return null;
            }

            // LocatorHelper.Instance.ExecuteQuery<Contrato>(qry, typeof(Contrato), pm);
        }

        public static void SetaUsuarioLiberador(Object contratoId, Object usuarioId, PersistenceManager pm)
        {
            String command = String.Concat(
                "UPDATE contrato SET contrato_usuarioLiberador=",
                usuarioId, " WHERE contrato_id = ", contratoId);

            NonQueryHelper.Instance.ExecuteNonQuery(command, pm);
        }

        /// <summary>
        /// Checa se o número de contrato não está sendo usado por outro contrato.
        /// </summary>
        public static Boolean ContratoDisponivel(Object contratoId, Object operadoraId, String numero, ref String msgRetorno)
        {
            String query = "SELECT contrato_id FROM contrato WHERE contrato_rascunho=0 AND contrato_operadoraId=" + operadoraId + " AND contrato_numero=@numero ";

            if (contratoId != null)
                query += " AND contrato_id <> " + contratoId;

            DataTable dt = LocatorHelper.Instance.ExecuteParametrizedQuery(
                query, new String[] { "@numero" }, new String[] { numero }).Tables[0];

            Boolean valido = dt == null || dt.Rows.Count == 0;

            if (!valido)
            {
                msgRetorno = "Número de contrato já está em uso para essa operadora.";
            }

            return valido;
        }

        public static Boolean NumeroJaUtilizado(String numero, Object contratoId)
        {
            String query = "SELECT top 1 contrato_id FROM contrato WHERE contrato_numero=@numero ";

            if (contratoId != null)
                query += " AND contrato_id <> " + contratoId;

            DataTable dt = LocatorHelper.Instance.ExecuteParametrizedQuery(
                query, new String[] { "@numero" }, new String[] { numero }).Tables[0];

            Boolean valido = dt == null || dt.Rows.Count == 0;

            return valido;
        }

        public static Boolean NumeroDeContratoEmUso(String numero, String letra, Int32 zerosAEsquerda, Object operadoraId, PersistenceManager pm)
        {
            String _numero = EntityBase.GeraNumeroDeContrato(Convert.ToInt32(numero), zerosAEsquerda, letra);

            String qry = "SELECT contrato_id FROM contrato WHERE contrato_operadoraId=" + operadoraId + " AND contrato_numero=@NUM";
            IList<Contrato> ret = LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, new String[] { "@NUM" }, new String[] { _numero }, typeof(Contrato), pm);
            return ret != null;
        }

        public static Boolean CanceladoOuInativo(Object contratoId)
        {
            String qry = "SELECT contrato_id FROM contrato WHERE (contrato_inativo=1 OR contrato_cancelado=1) and contrato_id=" + contratoId;
            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, null, null, null);
            return ret != null;
        }

        /// <summary>
        /// Realiza entrada no historico de mudanças do contrato.
        /// </summary>
        public static void AlteraStatusDeContrato(Object contratoId, Boolean cancelado)
        {
            AlteraStatusDeContrato(contratoId, cancelado, null);
        }
        /// <summary>
        /// Realiza entrada no historico de mudanças do contrato.
        /// </summary>
        public static void AlteraStatusDeContrato(Object contratoId, Boolean cancelado, PersistenceManager _pm)
        {
            String valor = "0", dataCancelamentoParam = ", contrato_alteracao=getdate() ";
            if (cancelado) { valor = "1"; dataCancelamentoParam = ", contrato_alteracao=getdate(), contrato_dataCancelamento=GetDate()"; }

            PersistenceManager pm = null;
            if (_pm != null)
                pm = _pm;
            else
            {
                pm = new PersistenceManager();
                pm.BeginTransactionContext();
            }

            try
            {
                String command = "UPDATE contrato SET contrato_cancelado=" + valor + dataCancelamentoParam + " WHERE contrato_id=" + contratoId;
                NonQueryHelper.Instance.ExecuteNonQuery(command, pm);

                Contrato contrato = new Contrato(contratoId);
                pm.Load(contrato);

                if (cancelado)
                    ContratoStatusHistorico.Salvar(contrato.Numero, contrato.OperadoraID, ContratoStatusHistorico.eStatus.Cancelado, pm);
                else
                    ContratoStatusHistorico.Salvar(contrato.Numero, contrato.OperadoraID, ContratoStatusHistorico.eStatus.ReAtivado, pm);

                if (_pm == null) { pm.Commit(); }
            }
            catch (Exception ex)
            {
                if (_pm == null) { pm.Rollback(); }
                throw ex;
            }
            finally
            {
                if (_pm == null) { pm = null; }
            }
        }

        /// <summary>
        /// Retorna o id do contrato administrativo de uma proposta.
        /// </summary>
        /// <param name="contratoId">ID da proposta (contrato com o segurado).</param>
        /// <param name="pm">Objeto PersistenceManager participante da transação, caso exista uma, ou null.</param>
        /// <returns>ID do contrato administrativo.</returns>
        public static Object CarregaContratoAdmID(Object contratoId, PersistenceManager pm)
        {
            String qry = "SELECT contrato_contratoAdmId FROM contrato WHERE contrato_id=" + contratoId;
            return LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
        }

        public static Object CarregaContratoID(Object operadoraId, String contratoNumero, PersistenceManager pm)
        {
            String qry = "SELECT contrato_id FROM contrato WHERE contrato_numero='" + contratoNumero + "' AND contrato_operadoraId=" + operadoraId;
            return LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
        }

        /// <summary>
        /// Esse método foi escrito APENAS para calculo de demonstrativo de pagamento.
        /// </summary>
        public static Decimal CalculaValorDaProposta_TODOS(Object propostaId, DateTime dataReferencia, PersistenceManager pm, Boolean calculaTaxa, Boolean calculaValorEstipulante, ref List<CobrancaComposite> composite, Boolean forcaTabelaVigente)
        {
            if (pm == null) { pm = new PersistenceManager(); }
            composite = new List<CobrancaComposite>();

            Decimal valorAdicionais = 0, valorPlano = 0, valorPlanoAux = 0, valorAdicional = 0;
            Contrato contrato = new Contrato(propostaId);
            pm.Load(contrato);

            //beneficiarios ativos da proposta
            IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID_Parcial(propostaId, false, false, pm);
            if (beneficiarios == null) { return 0; }

            //calcula valores de TAXA DE ASSOCIACAO
            Decimal valorEstipulante = 0;
            if (calculaValorEstipulante) { valorEstipulante = CalculaValorDaTaxaAssociativa(contrato, beneficiarios.Count, null, pm); }
            composite.Add(new CobrancaComposite(null, null, CobrancaComposite.eComposicaoTipo.TaxaAssociacao, valorEstipulante));

            ContratoBeneficiario.eStatus status = ContratoBeneficiario.eStatus.Desconhecido;
            foreach (ContratoBeneficiario contratoBeneficiario in beneficiarios)
            {
                status = (ContratoBeneficiario.eStatus)contratoBeneficiario.Status;

                #region nesses casos não calcula o valor
                if (status == ContratoBeneficiario.eStatus.Cancelado ||
                    status == ContratoBeneficiario.eStatus.CancelamentoDevolvido ||
                    status == ContratoBeneficiario.eStatus.CancelamentoPendente ||
                    status == ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora ||
                    status == ContratoBeneficiario.eStatus.Excluido ||
                    status == ContratoBeneficiario.eStatus.ExclusaoDevolvido ||
                    status == ContratoBeneficiario.eStatus.ExclusaoPendente ||
                    status == ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora ||
                    !contratoBeneficiario.DMED ||
                    //status == ContratoBeneficiario.eStatus.Novo ||
                    (dataReferencia < contratoBeneficiario.Vigencia && contratoBeneficiario.Tipo != 0)) //Vigencia
                {
                    continue; // nesses casos não calcula o valor
                }

                if (!contratoBeneficiario.Ativo && contratoBeneficiario.DataInativacao != DateTime.MinValue && dataReferencia != DateTime.MinValue)
                {
                    contratoBeneficiario.DataInativacao = new DateTime(contratoBeneficiario.DataInativacao.Year,
                        contratoBeneficiario.DataInativacao.Month, contratoBeneficiario.DataInativacao.Day, 23, 59, 59, 998);
                    if (contratoBeneficiario.DataInativacao < dataReferencia) { continue; }
                }
                else if (!contratoBeneficiario.Ativo) { continue; }

                #endregion

                Beneficiario beneficiario = new Beneficiario(contratoBeneficiario.BeneficiarioID);
                beneficiario.Carregar_DataNascimento(pm);
                Int32 idade = Beneficiario.CalculaIdade(beneficiario.DataNascimento, dataReferencia);

                //calcula valores de ADICIONAIS
                IList<AdicionalBeneficiario> adicionaisBeneficiario =
                    AdicionalBeneficiario.Carregar(propostaId, contratoBeneficiario.BeneficiarioID, pm);

                #region adicionais

                if (adicionaisBeneficiario != null)
                {
                    foreach (AdicionalBeneficiario adicionalBeneficiario in adicionaisBeneficiario)
                    {
                        valorAdicional = Adicional.CalculaValor(adicionalBeneficiario.AdicionalID,
                            adicionalBeneficiario.BeneficiarioID, idade, dataReferencia, pm);

                        valorAdicionais += valorAdicional;
                        composite.Add(new CobrancaComposite(null, adicionalBeneficiario.BeneficiarioID, CobrancaComposite.eComposicaoTipo.Adicional, valorAdicional));
                    }

                    adicionaisBeneficiario = null;
                }
                #endregion

                //calcula valor do PLANO
                valorPlanoAux = TabelaValor.CalculaValor(contratoBeneficiario.BeneficiarioID, idade,
                    contrato.ContratoADMID, contrato.PlanoID,
                    (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao, pm, contrato.Admissao, dataReferencia, forcaTabelaVigente);

                if (valorPlanoAux == 0) { return 0; } //não foi localizada uma tabela vigente. interrompe o cálculo

                composite.Add(new CobrancaComposite(null, contratoBeneficiario.BeneficiarioID, CobrancaComposite.eComposicaoTipo.Plano, valorPlanoAux));
                valorPlano += valorPlanoAux;
            }

            beneficiarios = null;
            Decimal valorTaxa = 0;

            #region taxa da tabela de valor

            if (calculaTaxa)
            {
                IList<TabelaValor> tabela = TabelaValor.CarregarTabelaAtual(contrato.ContratoADMID, pm);
                if (tabela != null && tabela.Count > 0)
                {
                    Taxa taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                    tabela = null;
                    if (taxa != null && !taxa.Embutido)
                    {
                        valorTaxa = taxa.ValorEmbutido;
                        if (taxa.ValorEmbutido > 0)
                        {
                            composite.Add(new CobrancaComposite(null, null, CobrancaComposite.eComposicaoTipo.TaxaTabelaValor, valorTaxa));
                        }
                        taxa = null;
                    }
                }
            }
            #endregion

            return valorPlano + valorAdicionais + valorEstipulante + valorTaxa - contrato.Desconto;
        }

        /* CALCULA VALOR */

        public static Decimal CalculaValorDaProposta2(Object propostaId, DateTime dataReferencia, PersistenceManager pm, Boolean calculaTaxa, Boolean calculaValorEstipulante, ref List<CobrancaComposite> composite, Boolean naoRecorrente)
        {
            return CalculaValorDaProposta(propostaId, dataReferencia, pm, calculaTaxa, calculaValorEstipulante, ref composite, false, naoRecorrente);
        }

        public static Decimal CalculaValorDaProposta(Object propostaId, DateTime dataReferencia, PersistenceManager pm, Boolean calculaTaxa, Boolean calculaValorEstipulante, ref List<CobrancaComposite> composite, Boolean forcaTabelaVigente, bool naoRecorrente)
        {
            if (pm == null) { pm = new PersistenceManager(); }
            composite = new List<CobrancaComposite>();

            Decimal valorAdicionais = 0, valorPlano = 0, valorPlanoAux = 0;
            Contrato contrato = new Contrato(propostaId);
            pm.Load(contrato);

            //beneficiarios ativos da proposta
            IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID_Parcial(propostaId, false, false, pm);
            if (beneficiarios == null) { return 0; }

            //calcula valores de TAXA DE ASSOCIACAO
            Decimal valorEstipulante = 0;
            if (calculaValorEstipulante) { valorEstipulante = CalculaValorDaTaxaAssociativa(contrato, beneficiarios.Count, dataReferencia, pm); }
            if (valorEstipulante > 0)
            {
                composite.Add(new CobrancaComposite(null, null, CobrancaComposite.eComposicaoTipo.TaxaAssociacao, valorEstipulante));
            }

            ContratoBeneficiario.eStatus status = ContratoBeneficiario.eStatus.Desconhecido;
            foreach (ContratoBeneficiario contratoBeneficiario in beneficiarios)
            {
                status = (ContratoBeneficiario.eStatus)contratoBeneficiario.Status;

                #region nesses casos não calcula o valor
                if (status == ContratoBeneficiario.eStatus.Cancelado ||
                    status == ContratoBeneficiario.eStatus.CancelamentoDevolvido ||
                    status == ContratoBeneficiario.eStatus.CancelamentoPendente ||
                    status == ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora ||
                    status == ContratoBeneficiario.eStatus.Excluido ||
                    status == ContratoBeneficiario.eStatus.ExclusaoDevolvido ||
                    status == ContratoBeneficiario.eStatus.ExclusaoPendente ||
                    status == ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora ||
                    ////status == ContratoBeneficiario.eStatus.Novo ||
                    //dataReferencia < contratoBeneficiario.Vigencia && contratoBeneficiario.Tipo != (int)ContratoBeneficiario.TipoRelacao.Titular ||
                    contratoBeneficiario.DataInativacao != DateTime.MinValue && contratoBeneficiario.DataInativacao < dataReferencia)
                {
                    continue; // nesses casos não calcula o valor
                }

                #endregion

                Beneficiario beneficiario = new Beneficiario(contratoBeneficiario.BeneficiarioID);
                beneficiario.Carregar_DataNascimento(pm);
                Int32 idade = Beneficiario.CalculaIdade(beneficiario.DataNascimento, dataReferencia);

                //calcula valores de ADICIONAIS
                IList<AdicionalBeneficiario> adicionaisBeneficiario =
                    AdicionalBeneficiario.Carregar(propostaId, contratoBeneficiario.BeneficiarioID, pm);

                #region adicionais

                if (adicionaisBeneficiario != null)
                {
                    foreach (AdicionalBeneficiario adicionalBeneficiario in adicionaisBeneficiario)
                    {
                        //valorAdicional = Adicional.CalculaValor(adicionalBeneficiario.AdicionalID,
                        //    adicionalBeneficiario.BeneficiarioID, idade, dataReferencia, pm);

                        //valorAdicionais += valorAdicional;
                        //composite.Add(new CobrancaComposite(null, adicionalBeneficiario.BeneficiarioID, CobrancaComposite.eComposicaoTipo.Adicional, valorAdicional));

                        valorAdicionais += adicionalBeneficiario.RetornaValor(AdicionalBeneficiario._FormaPagtoBoleto, contrato.Vigencia, naoRecorrente);
                    }

                    adicionaisBeneficiario = null;
                }
                #endregion

                //calcula valor do PLANO
                valorPlanoAux = TabelaValor.CalculaValor(contratoBeneficiario.BeneficiarioID, idade,
                    contrato.ContratoADMID, contrato.PlanoID,
                    (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao, pm, contrato.Admissao, dataReferencia, forcaTabelaVigente);

                //if (valorPlanoAux == 0) { return 0; } //não foi localizada uma tabela vigente. interrompe o cálculo

                if (valorPlanoAux > 0)
                {
                    composite.Add(new CobrancaComposite(null, contratoBeneficiario.BeneficiarioID, CobrancaComposite.eComposicaoTipo.Plano, valorPlanoAux));
                    valorPlano += valorPlanoAux;
                }
            }

            beneficiarios = null;
            Decimal valorTaxa = 0;

            #region taxa da tabela de valor

            if (calculaTaxa)
            {
                IList<TabelaValor> tabela = TabelaValor.CarregarTabelaAtual(contrato.ContratoADMID, pm);
                if (tabela != null && tabela.Count > 0)
                {
                    Taxa taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                    tabela = null;
                    if (taxa != null && !taxa.Embutido)
                    {
                        valorTaxa = taxa.ValorEmbutido;
                        if (taxa.ValorEmbutido > 0)
                        {
                            composite.Add(new CobrancaComposite(null, null, CobrancaComposite.eComposicaoTipo.TaxaTabelaValor, valorTaxa));
                        }
                        taxa = null;
                    }
                }
            }
            #endregion

            return valorPlano + valorAdicionais + valorEstipulante + valorTaxa - contrato.Desconto;
        }

        public static Decimal ________CalculaValorDaProposta(Object propostaId, DateTime dataReferencia, PersistenceManager pm, Boolean calculaTaxa, Boolean calculaValorEstipulante, ref List<CobrancaComposite> composite, Boolean forcaTabelaVigente)
        {
            if (pm == null) { pm = new PersistenceManager(); }
            composite = new List<CobrancaComposite>();

            Decimal valorAdicionais = 0, valorPlano = 0, valorPlanoAux = 0, valorAdicional = 0;
            Contrato contrato = new Contrato(propostaId);
            pm.Load(contrato);

            //beneficiarios ativos da proposta
            IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID_Parcial(propostaId, false, false, pm);
            if (beneficiarios == null) { return 0; }

            //calcula valores de TAXA DE ASSOCIACAO
            Decimal valorEstipulante = 0;
            if (calculaValorEstipulante) { valorEstipulante = CalculaValorDaTaxaAssociativa(contrato, beneficiarios.Count, dataReferencia, pm); }
            if (valorEstipulante > 0)
            {
                composite.Add(new CobrancaComposite(null, null, CobrancaComposite.eComposicaoTipo.TaxaAssociacao, valorEstipulante));
            }

            ContratoBeneficiario.eStatus status = ContratoBeneficiario.eStatus.Desconhecido;
            foreach (ContratoBeneficiario contratoBeneficiario in beneficiarios)
            {
                status = (ContratoBeneficiario.eStatus)contratoBeneficiario.Status;

                #region nesses casos não calcula o valor
                if (status == ContratoBeneficiario.eStatus.Cancelado ||
                    status == ContratoBeneficiario.eStatus.CancelamentoDevolvido ||
                    status == ContratoBeneficiario.eStatus.CancelamentoPendente ||
                    status == ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora ||
                    status == ContratoBeneficiario.eStatus.Excluido ||
                    status == ContratoBeneficiario.eStatus.ExclusaoDevolvido ||
                    status == ContratoBeneficiario.eStatus.ExclusaoPendente ||
                    status == ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora ||
                    //status == ContratoBeneficiario.eStatus.Novo ||
                    dataReferencia < contratoBeneficiario.Vigencia && contratoBeneficiario.Tipo != (int)ContratoBeneficiario.TipoRelacao.Titular ||
                    contratoBeneficiario.DataInativacao != DateTime.MinValue && contratoBeneficiario.DataInativacao < dataReferencia)
                {
                    continue; // nesses casos não calcula o valor
                }

                #endregion

                Beneficiario beneficiario = new Beneficiario(contratoBeneficiario.BeneficiarioID);
                beneficiario.Carregar_DataNascimento(pm);
                Int32 idade = Beneficiario.CalculaIdade(beneficiario.DataNascimento, dataReferencia);

                //calcula valores de ADICIONAIS
                IList<AdicionalBeneficiario> adicionaisBeneficiario =
                    AdicionalBeneficiario.Carregar(propostaId, contratoBeneficiario.BeneficiarioID, pm);

                #region adicionais

                if (adicionaisBeneficiario != null)
                {
                    foreach (AdicionalBeneficiario adicionalBeneficiario in adicionaisBeneficiario)
                    {
                        valorAdicional = Adicional.CalculaValor(adicionalBeneficiario.AdicionalID,
                            adicionalBeneficiario.BeneficiarioID, idade, dataReferencia, pm);

                        valorAdicionais += valorAdicional;
                        composite.Add(new CobrancaComposite(null, adicionalBeneficiario.BeneficiarioID, CobrancaComposite.eComposicaoTipo.Adicional, valorAdicional));
                    }

                    adicionaisBeneficiario = null;
                }
                #endregion

                //calcula valor do PLANO
                valorPlanoAux = TabelaValor.CalculaValor(contratoBeneficiario.BeneficiarioID, idade,
                    contrato.ContratoADMID, contrato.PlanoID,
                    (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao, pm, contrato.Admissao, dataReferencia, forcaTabelaVigente);

                if (valorPlanoAux == 0) { return 0; } //não foi localizada uma tabela vigente. interrompe o cálculo

                composite.Add(new CobrancaComposite(null, contratoBeneficiario.BeneficiarioID, CobrancaComposite.eComposicaoTipo.Plano, valorPlanoAux));
                valorPlano += valorPlanoAux;
            }

            beneficiarios = null;
            Decimal valorTaxa = 0;

            #region taxa da tabela de valor

            if (calculaTaxa)
            {
                IList<TabelaValor> tabela = TabelaValor.CarregarTabelaAtual(contrato.ContratoADMID, pm);
                if (tabela != null && tabela.Count > 0)
                {
                    Taxa taxa = Taxa.CarregarPorTabela(tabela[0].ID, pm);
                    tabela = null;
                    if (taxa != null && !taxa.Embutido)
                    {
                        valorTaxa = taxa.ValorEmbutido;
                        if (taxa.ValorEmbutido > 0)
                        {
                            composite.Add(new CobrancaComposite(null, null, CobrancaComposite.eComposicaoTipo.TaxaTabelaValor, valorTaxa));
                        }
                        taxa = null;
                    }
                }
            }
            #endregion

            return valorPlano + valorAdicionais + valorEstipulante + valorTaxa - contrato.Desconto;
        }

        public static Decimal CalculaValorDaPropostaSemTaxaAssociativa(Object propostaId, ContratoBeneficiario contratoBeneficiario, DateTime dataReferencia, PersistenceManager pm)
        {
            Decimal valorAdicionais = 0, valorPlano = 0;
            Contrato contrato = new Contrato(propostaId);
            pm.Load(contrato);

            Beneficiario beneficiario = new Beneficiario(contratoBeneficiario.BeneficiarioID);
            pm.Load(beneficiario);
            Int32 idade = Beneficiario.CalculaIdade(beneficiario.DataNascimento, dataReferencia);

            //calcula valores de ADICIONAIS
            IList<AdicionalBeneficiario> adicionaisBeneficiario =
                AdicionalBeneficiario.Carregar(propostaId, contratoBeneficiario.BeneficiarioID, pm);

            if (adicionaisBeneficiario != null)
            {
                foreach (AdicionalBeneficiario adicionalBeneficiario in adicionaisBeneficiario)
                {
                    valorAdicionais += Adicional.CalculaValor(adicionalBeneficiario.AdicionalID,
                        adicionalBeneficiario.BeneficiarioID, idade, dataReferencia, pm);
                }

                adicionaisBeneficiario = null;
            }

            //calcula valor do PLANO
            if (dataReferencia == null || dataReferencia == DateTime.MinValue)
            {
                valorPlano += TabelaValor.CalculaValor(contratoBeneficiario.BeneficiarioID, idade,
                    contrato.ContratoADMID, contrato.PlanoID,
                    (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao, pm, contrato.Admissao, null);
            }
            else
            {
                valorPlano += TabelaValor.CalculaValor(contratoBeneficiario.BeneficiarioID, idade,
                    contrato.ContratoADMID, contrato.PlanoID,
                    (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao, pm, contrato.Admissao, dataReferencia);
            }

            return valorPlano + valorAdicionais - contrato.Desconto;
        }

        public static Decimal CalculaValorDaPropostaSemTaxaAssocSemAdicional(Contrato contrato, ContratoBeneficiario contratoBeneficiario, DateTime dataReferencia, PersistenceManager pm)
        {
            Decimal valorPlano = 0;

            Beneficiario beneficiario = new Beneficiario(contratoBeneficiario.BeneficiarioID);
            pm.Load(beneficiario);
            Int32 idade = Beneficiario.CalculaIdade(beneficiario.DataNascimento, dataReferencia);

            //calcula valor do PLANO
            valorPlano += TabelaValor.CalculaValor(contratoBeneficiario.BeneficiarioID, idade,
                contrato.ContratoADMID, contrato.PlanoID,
                (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao, pm, contrato.Admissao, null);

            return valorPlano - contrato.Desconto;
        }

        public static Decimal CalculaValorDaTaxaAssociativa(Contrato contrato, Int32 qtdBeneficiarios, DateTime? dataRef, PersistenceManager pm)
        {
            if (contrato.CobrarTaxaAssociativa)
            {
                EstipulanteTaxa estipulanteTaxa = null;
                if (dataRef == null || dataRef.Value == DateTime.MinValue)
                    estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, pm);
                else
                    estipulanteTaxa = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, dataRef.Value, pm);

                if (estipulanteTaxa != null)
                {
                    if (((EstipulanteTaxa.eTipoTaxa)estipulanteTaxa.TipoTaxa) == EstipulanteTaxa.eTipoTaxa.PorProposta)
                    {
                        return estipulanteTaxa.Valor;
                    }
                    else // taxa por vida
                    {
                        if (qtdBeneficiarios == -1)
                        {
                            IList<ContratoBeneficiario> benefs = ContratoBeneficiario.CarregarPorContratoID_Parcial(contrato.ID, true, false, pm);
                            if (benefs != null)
                                qtdBeneficiarios = benefs.Count;
                            else
                                qtdBeneficiarios = 0;
                            benefs = null;
                        }
                        return (estipulanteTaxa.Valor * qtdBeneficiarios);
                    }
                }
                else
                    return 0;
            }
            else
                return 0;
        }


        public static Boolean VerificaExistenciaBeneficiarioNoContrato(Object beneficiarioId, Object contratoId)
        {
            String[] pName = new String[0];
            String[] pValue = new String[0];

            String query = " SELECT contrato.* " +
                        " FROM contrato " +
                        " INNER JOIN contrato_beneficiario ON contrato_id = contratobeneficiario_contratoId " +
                        " WHERE contratobeneficiario_beneficiarioId = " + beneficiarioId + " AND contrato_id = " + contratoId + " AND contratobeneficiario_ativo = 1 ";

            DataTable dt = LocatorHelper.Instance.ExecuteParametrizedQuery(
                query, pName, pValue).Tables[0];

            Boolean valido = dt == null || dt.Rows.Count == 0;

            return valido;
        }

        public static void AlterarNumeroDeContrato(Object contratoId, String novoNumero, PersistenceManager pm)
        {
            String[] names = new String[1] { "@numero" };
            String[] value = new String[1] { novoNumero };

            NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_numero=@numero WHERE contrato_id=" + contratoId, names, value, pm);
        }

        public static void AlterarNumeroDeMatricula(Object contratoId, String novoNumero)
        {
            String[] names = new String[1] { "@numero" };
            String[] value = new String[1] { novoNumero };

            NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_numeroMatricula=@numero WHERE contrato_id=" + contratoId, names, value, null);
        }

    }

    [DBTable("tabela_valor")]
    public class TabelaValor : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _contratoId;
        String _descricao;
        DateTime _data;
        Object _corrente;

        DateTime _inicio;
        DateTime _fim;

        DateTime _inicioVencimento;
        DateTime _fimVencimento;

        String _contratoDescricao;

        #region propriedades

        [DBFieldInfo("TabelaValor_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("TabelaValor_contratoid", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId = value; }
        }

        //[DBFieldInfo("TabelaValor_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _contratoDescricao; }
            set { _descricao = value; }
        }

        [DBFieldInfo("tabelavalor_inicio", FieldType.Single)]
        public DateTime Inicio
        {
            get { return _inicio; }
            set { _inicio= value; }
        }

        [DBFieldInfo("tabelavalor_fim", FieldType.Single)]
        public DateTime Fim
        {
            get { return _fim; }
            set { _fim= value; }
        }

        [DBFieldInfo("tabelavalor_vencimentoInicio", FieldType.Single)]
        public DateTime VencimentoInicio
        {
            get { return _inicioVencimento; }
            set { _inicioVencimento = value; }
        }

        [DBFieldInfo("tabelavalor_vencimentoFim", FieldType.Single)]
        public DateTime VencimentoFim
        {
            get { return _fimVencimento; }
            set { _fimVencimento = value; }
        }

        public String VencimentoInicioStr
        {
            get { if (_inicioVencimento == DateTime.MinValue) return ""; else return _inicioVencimento.ToString("dd/MM/yyyy"); }
        }

        public String VencimentoFimStr
        {
            get { if (_fimVencimento == DateTime.MinValue) return ""; else return _fimVencimento.ToString("dd/MM/yyyy"); }
        }

        [Obsolete("Em desuso.", true)]
        [DBFieldInfo("TabelaValor_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        /// <summary>
        /// Estará na entidade plano o id da tabela corrente
        /// </summary>
        [Joinned("plano_tabelaValorAtualId")]
        public Object Corrente
        {
            get
            {
                return _corrente;
            }
            set { _corrente = value; }
        }

        [Joinned("contratoadm_descricao")]
        public String ContratoDescricao
        {
            get { return _contratoDescricao; }
            set { _contratoDescricao = value; }
        }

        #endregion

        public TabelaValorItem TabelaValorItem
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public TabelaValor() { _data = DateTime.Now; }
        public TabelaValor(Object id) : this() { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.TransactionContext();

            try
            {
                String query = "DELETE FROM tabela_valor_item WHERE tabelavaloritem_tabelaid=" + this._id;
                NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

                query = "DELETE FROM tabela_valor_data WHERE tvdata_tabelaId=" + this._id;
                NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

                query = "DELETE FROM taxa WHERE taxa_tabelaValorId=" + this._id;
                NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

                pm.Remove(this);
                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
            //base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<TabelaValor> CarregarPorContratoID(Object contratoAdmId)
        {
            return CarregarPorContratoID(contratoAdmId, null);
        }

        public static IList<TabelaValor> CarregarPorContratoID(Object contratoAdmId, PersistenceManager pm)
        {
            String query = "tabela_valor.*, contratoadm_descricao FROM tabela_valor LEFT JOIN contratoAdm ON tabelavalor_contratoid=contratoAdm_id WHERE tabelavalor_contratoid=" + contratoAdmId + " ORDER BY tabelavalor_inicio DESC, tabelavalor_id DESC";
            return LocatorHelper.Instance.ExecuteQuery<TabelaValor>(query, typeof(TabelaValor), pm);
        }

        public static IList<TabelaValor> CarregarPorContratoID_Parcial(Object contratoAdmId, PersistenceManager pm)
        {
            String query = "tabelavalor_id FROM tabela_valor WHERE tabelavalor_contratoid=" + contratoAdmId + " ORDER BY tabelavalor_inicio DESC, tabelavalor_id DESC";
            return LocatorHelper.Instance.ExecuteQuery<TabelaValor>(query, typeof(TabelaValor), pm);
        }

        public static IList<TabelaValor> CarregarTabelaAtual(Object contratoAdmId)
        {
            return CarregarTabelaAtual(contratoAdmId, null);
        }
        public static IList<TabelaValor> CarregarTabelaAtual(Object contratoAdmId, PersistenceManager pm)
        {
            String query = "TOP 1 tabela_valor.*, contratoadm_descricao FROM tabela_valor INNER JOIN contratoAdm ON tabelavalor_contratoid=contratoAdm_id WHERE tabelavalor_contratoid=" + contratoAdmId + " ORDER BY tabelavalor_inicio DESC, tabelavalor_id DESC";
            return LocatorHelper.Instance.ExecuteQuery<TabelaValor>(query, typeof(TabelaValor), pm);
        }

        public static IList<TabelaValor> CarregarTabelaVigente(Object contratoAdmId, DateTime admissaoProposta, DateTime? vencimentoCobranca, PersistenceManager pm)
        {
            return CarregarTabelaVigente(contratoAdmId, admissaoProposta, vencimentoCobranca, pm, false); ;
        }
        public static IList<TabelaValor> CarregarTabelaVigente(Object contratoAdmId, DateTime admissaoProposta, DateTime? vencimentoCobranca, PersistenceManager pm, Boolean forcaTabelaVigente)
        {
            String vencimentoCond = "";
            if (vencimentoCobranca != null)
            {
                vencimentoCond = String.Concat(" OR '", vencimentoCobranca.Value.ToString("yyyy-MM-dd"), "'  BETWEEN tabelavalor_vencimentoInicio AND tabelavalor_vencimentoFim");
            }

            String query = String.Concat("TOP 1 tabela_valor.*, contratoadm_descricao ",
                "   FROM tabela_valor ",
                "       INNER JOIN contratoAdm ON tabelavalor_contratoid=contratoAdm_id",
                "   WHERE ",
                "       tabelavalor_contratoid=", contratoAdmId,
                "       AND ('", admissaoProposta.ToString("yyyy-MM-dd"), "' BETWEEN tabelavalor_inicio AND tabelavalor_fim ", vencimentoCond, ")",
                "   ORDER BY tabelavalor_inicio DESC, tabelavalor_id DESC");

            IList<TabelaValor> ret = LocatorHelper.Instance.ExecuteQuery<TabelaValor>(query, typeof(TabelaValor), pm);

            if (ret == null) { return null; }

            if (vencimentoCobranca != null && forcaTabelaVigente)
            {
                //se foi informado um vencimento de cobrança
                //se esse vencimento for maior que o limite de vencimento da tabela, retorna null.
                //nesses casos, nao basta estar de acordo com a admissao do contrato:
                ret[0].VencimentoFim = new DateTime(ret[0].VencimentoFim.Year, ret[0].VencimentoFim.Month, ret[0].VencimentoFim.Day, 23, 59, 59, 998);
                vencimentoCobranca = new DateTime(vencimentoCobranca.Value.Year, vencimentoCobranca.Value.Month, vencimentoCobranca.Value.Day, 23, 59, 59, 997);
                if (vencimentoCobranca > ret[0].VencimentoFim) { return null; }
            }

            return ret;
        }

        public static Decimal CalculaValor(Object beneficiarioId, int beneficiarioIdade, Object contratoId, Object planoId, Contrato.eTipoAcomodacao tipoAcomodacao, DateTime admissaoProposta, DateTime? vencimentoCobranca)
        {
            return CalculaValor(beneficiarioId, beneficiarioIdade, contratoId, planoId, tipoAcomodacao, null, admissaoProposta, vencimentoCobranca, false);
        }
        public static Decimal CalculaValor(Object beneficiarioId, int beneficiarioIdade, Object contratoAdmId, Object planoId, Contrato.eTipoAcomodacao tipoAcomodacao, PersistenceManager pm, DateTime admissaoProposta, DateTime? vencimentoCobranca)
        {
            return CalculaValor(beneficiarioId, beneficiarioIdade, contratoAdmId, planoId, tipoAcomodacao, pm, admissaoProposta, vencimentoCobranca, false);
        }
        public static Decimal CalculaValor(Object beneficiarioId, int beneficiarioIdade, Object contratoAdmId, Object planoId, Contrato.eTipoAcomodacao tipoAcomodacao, PersistenceManager pm, DateTime admissaoProposta, DateTime? vencimentoCobranca, Boolean forcaTabelaVigente)
        {
            IList<TabelaValor> lista = TabelaValor.CarregarTabelaVigente(contratoAdmId, admissaoProposta, vencimentoCobranca, pm, forcaTabelaVigente); //TabelaValor.CarregarTabelaAtual(contratoAdmId, pm);
            if (lista == null || lista.Count == 0) { return 0; }

            IList<TabelaValorItem> itens = TabelaValorItem.CarregarPorTabela(lista[0].ID, planoId, pm);
            if (itens == null || itens.Count == 0) { return 0; }

            if (beneficiarioIdade == 0)
            {
                Beneficiario beneficiario = new Beneficiario();
                beneficiario.ID = beneficiarioId;

                if (beneficiario.ID != null)
                {
                    if (pm == null)
                        beneficiario.Carregar();
                    else
                        pm.Load(beneficiario);

                    if (beneficiario.ID == null) { return 0; }

                    beneficiarioIdade = Beneficiario.CalculaIdade(beneficiario.DataNascimento);
                }
            }

            foreach (TabelaValorItem _item in itens)
            {
                if (beneficiarioIdade >= _item.IdadeInicio && _item.IdadeFim == 0)
                {
                    if (tipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                        return _item.QCValor;
                    else
                        return _item.QPValor;
                }
                else if (beneficiarioIdade >= _item.IdadeInicio && beneficiarioIdade <= _item.IdadeFim)
                {
                    if (tipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                        return _item.QCValor;
                    else
                        return _item.QPValor;
                }
            }

            return 0;
        }

        public static Decimal CalculaValorNET(Contrato contrato, PersistenceManager pm)
        {
            return CalculaValorNET(contrato, pm, null, null, null);
        }

        public static Decimal CalculaValorNET(Object contratoId, Object contratoAdmId, Object planoId, Object beneficiarioId, Int32 contratoTipoAcomodacao, DateTime? admissao, DateTime beneficiarioDataNascimento, DateTime? vencimento, DateTime? dataReferencia, out Int32 beneficiarioIdade, PersistenceManager pm)
        {
            Contrato.eTipoAcomodacao tipoAcomodacao = (Contrato.eTipoAcomodacao)contratoTipoAcomodacao;
            IList<TabelaValor> lista = null;
            beneficiarioIdade = -1;

            if (admissao == null || vencimento == null)
                lista = TabelaValor.CarregarTabelaAtual(contratoAdmId, pm);
            else
                lista = TabelaValor.CarregarTabelaVigente(contratoAdmId, admissao.Value, vencimento.Value, pm);

            if (lista == null || lista.Count == 0) { return 0; }

            IList<TabelaValorItem> itens = TabelaValorItem.CarregarPorTabela(lista[0].ID, planoId, pm);
            if (itens == null || itens.Count == 0) { return 0; }

            if (dataReferencia == null)
                beneficiarioIdade = Beneficiario.CalculaIdade(beneficiarioDataNascimento);
            else
                beneficiarioIdade = Beneficiario.CalculaIdade(beneficiarioDataNascimento, dataReferencia.Value);

            Decimal valorTotal = 0;
            foreach (TabelaValorItem _item in itens)
            {
                if (beneficiarioIdade >= _item.IdadeInicio && _item.IdadeFim == 0)
                {
                    if (tipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                        valorTotal += _item.QCValorPagamento;
                    else
                        valorTotal += _item.QPValorPagamento;
                    break;
                }
                else if (beneficiarioIdade >= _item.IdadeInicio && beneficiarioIdade <= _item.IdadeFim)
                {
                    if (tipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                        valorTotal += _item.QCValorPagamento;
                    else
                        valorTotal += _item.QPValorPagamento;
                    break;
                }
            }

            return valorTotal;
        }

        public static Decimal CalculaValorNET(Contrato contrato, PersistenceManager pm, DateTime? admissao, DateTime? vencimento, DateTime? dataReferencia)
        {
            Contrato.eTipoAcomodacao tipoAcomodacao = (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao;

            IList<TabelaValor> lista = null;

            if (admissao == null || vencimento == null)
                lista = TabelaValor.CarregarTabelaAtual(contrato.ContratoADMID, pm);
            else
                lista = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, admissao.Value, vencimento.Value, pm);

            if (lista == null || lista.Count == 0) { return 0; }

            IList<TabelaValorItem> itens = TabelaValorItem.CarregarPorTabela(lista[0].ID, contrato.PlanoID, pm);
            if (itens == null || itens.Count == 0) { return 0; }

            IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID_Parcial(contrato.ID, true, false, pm);

            int beneficiarioIdade = 0;
            Decimal valorTotal = 0;
            foreach (ContratoBeneficiario beneficiario in beneficiarios)
            {
                if (beneficiario.ID == null) { return 0; }

                if(dataReferencia == null)
                    beneficiarioIdade = Beneficiario.CalculaIdade(beneficiario.BeneficiarioDataNascimento);
                else
                    beneficiarioIdade = Beneficiario.CalculaIdade(beneficiario.BeneficiarioDataNascimento, dataReferencia.Value);

                foreach (TabelaValorItem _item in itens)
                {
                    if (beneficiarioIdade >= _item.IdadeInicio && _item.IdadeFim == 0)
                    {
                        if (tipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                            valorTotal += _item.QCValorPagamento;
                        else
                            valorTotal += _item.QPValorPagamento;
                        break;
                    }
                    else if (beneficiarioIdade >= _item.IdadeInicio && beneficiarioIdade <= _item.IdadeFim)
                    {
                        if (tipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                            valorTotal += _item.QCValorPagamento;
                        else
                            valorTotal += _item.QPValorPagamento;
                        break;
                    }
                }
            }

            beneficiarios = null;
            return valorTotal;
        }

        public static Boolean ExisteTabelaComVigencia(DateTime inicio, DateTime fim, Object contratoAdmId, Object tabelaId)
        {
            String query = "SELECT COUNT(*) FROM tabela_valor WHERE ((tabelavalor_inicio <= '" + inicio.ToString("yyyy-MM-dd 00:00:00") + "' AND tabelavalor_fim >='" + inicio.ToString("yyyy-MM-dd 23:59:59") + "') OR (tabelavalor_inicio <= '" + fim.ToString("yyyy-MM-dd 00:00:00") + "' AND tabelavalor_fim >='" + fim.ToString("yyyy-MM-dd 23:59:59") + "')) AND tabelavalor_contratoId=" + contratoAdmId;

            if (tabelaId != null)
            {
                query += " AND tabelavalor_id <> " + tabelaId;
            }

            Object ret = LocatorHelper.Instance.ExecuteScalar(query, null, null);

            if (ret == null || ret == DBNull.Value || Convert.ToInt32(ret) == 0)
                return false;
            else
                return true;
        }
    }

    //[DBTable("tabela_valor_data")]
    //public class TabelaValorData : EntityBase, IPersisteableEntity
    //{
    //    Object _id;
    //    Object _tabelaId;
    //    Object _planoId;
    //    DateTime _data;

    //    #region propriedades 

    //    [DBFieldInfo("tvdata_id", FieldType.PrimaryKeyAndIdentity)]
    //    public Object ID
    //    {
    //        get { return _id; }
    //        set { _id= value; }
    //    }

    //    [DBFieldInfo("tvdata_tabelaId", FieldType.Single)]
    //    public Object TabelaID
    //    {
    //        get { return _tabelaId; }
    //        set { _tabelaId= value; }
    //    }

    //    [DBFieldInfo("tvdata_planoId", FieldType.Single)]
    //    public Object PlanoID
    //    {
    //        get { return _planoId; }
    //        set { _planoId= value; }
    //    }

    //    [DBFieldInfo("tvdata_data", FieldType.Single)]
    //    public DateTime Data
    //    {
    //        get { return _data; }
    //        set { _data= value; }
    //    }

    //    public String strData
    //    {
    //        get 
    //        {
    //            if (_data == DateTime.MinValue)
    //                return String.Empty;
    //            else
    //                return _data.ToString("dd/MM/yyyy");
    //        }
    //    }

    //    #endregion

    //    public TabelaValorData() { }
    //    public TabelaValorData(Object id) { _id = id; }

    //    #region métodos EntityBase 

    //    public void Salvar()
    //    {
    //        base.Salvar(this);
    //    }

    //    public void Remover()
    //    {
    //        base.Remover(this);
    //    }

    //    public void Carregar()
    //    {
    //        base.Carregar(this);
    //    }
    //    #endregion

    //    public static IList<TabelaValorData> Carregar(Object tabelaId, Object planoId)
    //    {
    //        String query = "* FROM tabela_valor_data WHERE tvdata_tabelaId=" + tabelaId + " AND tvdata_planoId=" + planoId + " ORDER BY tvdata_data DESC, tvdata_id DESC";
    //        return LocatorHelper.Instance.ExecuteQuery<TabelaValorData>(query, typeof(TabelaValorData));
    //    }
    //}

    [Serializable()]
    [DBTable("tabela_valor_item")]
    public class TabelaValorItem : EntityBase, IPersisteableEntity
    {
        #region campos

        Object _id;
        Object _tabelaId;
        Object _planoId;
        int _idadeInicio;
        int _idadeFim;
        Boolean _calculoAutomatico;
        Decimal _qcValor;
        Decimal _qpValor;
        Decimal _qcValorPagamento;
        Decimal _qpValorPagamento;
        Decimal _qcValorCompraCarencia;
        Decimal _qpValorCompraCarencia;
        Decimal _qcValorMigracao;
        Decimal _qpValorMigracao;
        Decimal _qcValorCondicaoEspecial;
        Decimal _qpValorCondicaoEspecial;
        //DateTime _data;

        #endregion

        #region propriedades

        [DBFieldInfo("TabelaValoritem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("TabelaValoritem_tabelaid", FieldType.Single)]
        public Object TabelaID
        {
            get { return _tabelaId; }
            set { _tabelaId = value; }
        }

        [DBFieldInfo("tabelaValorItem_planoId", FieldType.Single)]
        public Object PlanoID
        {
            get { return _planoId; }
            set { _planoId = value; }
        }

        [DBFieldInfo("TabelaValoritem_idadeInicio", FieldType.Single)]
        public int IdadeInicio
        {
            get { return _idadeInicio; }
            set { _idadeInicio = value; }
        }

        [DBFieldInfo("TabelaValoritem_idadeFim", FieldType.Single)]
        public int IdadeFim
        {
            get { return _idadeFim; }
            set { _idadeFim = value; }
        }

        /// <summary>
        /// Cliente.
        /// </summary>
        [DBFieldInfo("tabelavaloritem_qComum", FieldType.Single)]
        public Decimal QCValor
        {
            get { return _qcValor; }
            set { _qcValor = value; }
        }

        [DBFieldInfo("tabelavaloritem_calculoAutomatico", FieldType.Single)]
        public Boolean CalculoAutomatico
        {
            get { return _calculoAutomatico; }
            set { _calculoAutomatico= value; }
        }

        public String QCValorSTR
        {
            get { return _qcValor.ToString("N2"); }
        }

        /// <summary>
        /// Cliente.
        /// </summary>
        [DBFieldInfo("tabelavaloritem_qParticular", FieldType.Single)]
        public Decimal QPValor
        {
            get { return _qpValor; }
            set { _qpValor = value; }
        }

        public String QPValorSTR
        {
            get { return _qpValor.ToString("N2"); }
        }

        /// <summary>
        /// Operadora
        /// </summary>
        [DBFieldInfo("tabelavaloritem_qComumPagamento", FieldType.Single)]
        public Decimal QCValorPagamento
        {
            get { return _qcValorPagamento; }
            set { _qcValorPagamento = value; }
        }

        public String QCValorPagamentoSTR
        {
            get { return _qcValorPagamento.ToString("N2"); }
        }

        /// <summary>
        /// Operadora
        /// </summary>
        [DBFieldInfo("tabelavaloritem_qParticularPagamento", FieldType.Single)]
        public Decimal QPValorPagamento
        {
            get { return _qpValorPagamento; }
            set { _qpValorPagamento = value; }
        }

        public String QPValorPagamentoSTR
        {
            get { return _qpValorPagamento.ToString("N2"); }
        }

        [DBFieldInfo("tabelavaloritem_qComumCompraCarencia", FieldType.Single)]
        public Decimal QCValorCompraCarencia
        {
            get { return _qcValorCompraCarencia; }
            set { _qcValorCompraCarencia = value; }
        }

        [DBFieldInfo("tabelavaloritem_qParticularCompraCarencia", FieldType.Single)]
        public Decimal QPValorCompraCarencia
        {
            get { return _qpValorCompraCarencia; }
            set { _qpValorCompraCarencia = value; }
        }

        [DBFieldInfo("tabelavaloritem_qComumMigracao", FieldType.Single)]
        public Decimal QCValorMigracao
        {
            get { return _qcValorMigracao; }
            set { _qcValorMigracao = value; }
        }

        [DBFieldInfo("tabelavaloritem_qParticularMigracao", FieldType.Single)]
        public Decimal QPValorMigracao
        {
            get { return _qpValorMigracao; }
            set { _qpValorMigracao = value; }
        }

        [DBFieldInfo("tabelavaloritem_qComumEspecial", FieldType.Single)]
        public Decimal QCValorCondicaoEspecial
        {
            get { return _qcValorCondicaoEspecial; }
            set { _qcValorCondicaoEspecial = value; }
        }

        [DBFieldInfo("tabelavaloritem_qParticularEspecial", FieldType.Single)]
        public Decimal QPValorCondicaoEspecial
        {
            get { return _qpValorCondicaoEspecial; }
            set { _qpValorCondicaoEspecial = value; }
        }

        //[DBFieldInfo("tabelavaloritem_data", FieldType.Single)]
        //public DateTime Data
        //{
        //    get { return _data; }
        //    set { _data= value; }
        //}

        public String FaixaEtaria
        {
            get
            {
                String ret = "";

                if (_idadeFim > 0)
                    ret = String.Concat("de ", _idadeInicio, " a ", _idadeFim);
                else
                    ret = String.Concat("de ", _idadeInicio, " em diante");

                return ret;
            }
        }

        #endregion

        public TabelaValorItem() { _calculoAutomatico = true; }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public void AplicaTaxa(Taxa taxa, Boolean calculaReajuste)
        {
            //se o calculo é manual
            if (!this._calculoAutomatico)// && calculaReajuste)
            {
                if (taxa.PercentualReajuste > 0 && this.QCValorPagamento != 0 && calculaReajuste) //usado na duplicação
                {
                    this.QCValor          = ((taxa.PercentualReajuste / 100) * this.QCValor) + this.QCValor;
                    this.QCValorPagamento = ((taxa.PercentualReajuste / 100) * this.QCValorPagamento) + this.QCValorPagamento;
                }

                if (taxa.PercentualReajuste > 0 && this.QPValorPagamento != 0 && calculaReajuste) //usado na duplicação§Ã£o
                {
                    this.QPValor          = ((taxa.PercentualReajuste / 100) * this.QPValor) + this.QPValor;
                    this.QPValorPagamento = ((taxa.PercentualReajuste / 100) * this.QPValorPagamento) + this.QPValorPagamento;
                }

                //if (taxa.Over > 0)
                //{
                //    //this.QCValor = taxa.Fixo + this.QCValorPagamento + ((taxa.Over / 100) * this.QCValorPagamento);
                //    this.QCValor = taxa.Fixo + this.QCValor + ((taxa.Over / 100) * this.QCValor);
                //    this.QPValor = taxa.Fixo + this.QPValor + ((taxa.Over / 100) * this.QPValor);

                //    this.QCValorPagamento = taxa.Fixo + this.QCValorPagamento + ((taxa.Over / 100) * this.QCValorPagamento);
                //    this.QPValorPagamento = taxa.Fixo + this.QPValorPagamento + ((taxa.Over / 100) * this.QPValorPagamento);
                //}
                //else
                //{
                //    this.QCValor = this.QCValor + taxa.Fixo;
                //    this.QPValor = this.QPValor + taxa.Fixo;

                //    this.QCValorPagamento = this.QCValorPagamento + taxa.Fixo;
                //    this.QPValorPagamento = this.QPValorPagamento + taxa.Fixo;
                //}
            }
            else //if(this._calculoAutomatico)
            {
                if (taxa.PercentualReajuste > 0 && this.QCValorPagamento != 0 && calculaReajuste) //usado na duplicação
                {
                    this.QCValorPagamento = ((taxa.PercentualReajuste / 100) * this.QCValorPagamento) + this.QCValorPagamento;
                }

                if (taxa.PercentualReajuste > 0 && this.QPValorPagamento != 0 && calculaReajuste) //usado na duplicação
                {
                    this.QPValorPagamento = ((taxa.PercentualReajuste / 100) * this.QPValorPagamento) + this.QPValorPagamento;
                }

                if (taxa.Over > 0)
                {
                    this.QCValor = taxa.Fixo + this.QCValorPagamento + ((taxa.Over / 100) * this.QCValorPagamento);
                }
                else
                    this.QCValor = this.QCValorPagamento + taxa.Fixo;

                if (taxa.Over > 0)
                    this.QPValor = taxa.Fixo + this.QPValorPagamento + ((taxa.Over / 100) * this.QPValorPagamento);
                else
                    this.QPValor = this.QPValorPagamento + taxa.Fixo;


                //if (taxa.PercentualReajuste > 0 && this.QCValorPagamento != 0 && calculaReajuste) //usado na duplicação
                //{
                //    this.QCValorPagamento = ((taxa.PercentualReajuste / 100) * this.QCValorPagamento) + this.QCValorPagamento;
                //    this.QPValorPagamento = ((taxa.PercentualReajuste / 100) * this.QPValorPagamento) + this.QPValorPagamento;
                //}
            }
        }

        public static IList<TabelaValorItem> CarregarPorTabela(Object tabelaID, Object planoId)
        {
            return CarregarPorTabela(tabelaID, planoId, null);
        }

        public static IList<TabelaValorItem> CarregarPorTabela(Object tabelaID, Object planoId, PersistenceManager pm)
        {
            String query = "* FROM tabela_valor_item WHERE TabelaValoritem_tabelaid=" + tabelaID + " AND tabelavaloritem_planoId=" + planoId + " ORDER BY TabelaValoritem_idadeInicio";
            return LocatorHelper.Instance.ExecuteQuery<TabelaValorItem>(query, typeof(TabelaValorItem), pm);
        }

        public static IList<TabelaValorItem> CarregarPorTabela(Object tabelaID, PersistenceManager pm)
        {
            String query = "* FROM tabela_valor_item WHERE TabelaValoritem_tabelaid=" + tabelaID + " ORDER BY tabelaValorItem_planoId, TabelaValoritem_idadeInicio";
            return LocatorHelper.Instance.ExecuteQuery<TabelaValorItem>(query, typeof(TabelaValorItem), pm);
        }
    }

    [DBTable("contratoStatusHistorico")]
    public class ContratoStatusHistorico : EntityBase, IPersisteableEntity
    {
        public enum eStatus : int
        {
            NoEstoque,
            ComCorretor,            // 1
            Rasurado,               // 2
            EmConferencia,          // 3
            NoCadastro,             // 4
            Cadastrado,             // 5
            Cancelado,              // 6
            ReAtivado,              // 7
            //DevolucaoParaAcerto   //8
            AlteradoNaOperadora,    // 8
            BeneficiarioAdicionado, // 9
            BeneficiarioAlterado,   //10
            BeneficiarioCancelado,  //11
            MudancaDePlano          //12
        }

        Object _id;
        Object _operadoraId;
        String _propostaNumero;
        Int32 _status;
        DateTime _data;

        #region propriedades

        [DBFieldInfo("contratostatushistorico_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("contratostatushistorico_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId = value; }
        }

        [DBFieldInfo("contratostatushistorico_propostaNumero", FieldType.Single)]
        public String PropostaNumero
        {
            get { return _propostaNumero; }
            set { _propostaNumero = value; }
        }

        [DBFieldInfo("contratostatushistorico_status", FieldType.Single)]
        public Int32 Status
        {
            get { return _status; }
            set { _status = value; }
        }

        [DBFieldInfo("contratostatushistorico_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data = value; }
        }

        #endregion

        public ContratoStatusHistorico() { _status = 0; }
        public ContratoStatusHistorico(Object id) : this() { _id = id; }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public static ContratoStatusHistorico Salvar(String numero, Object operadoraId, eStatus status, PersistenceManager pm)
        {
            ContratoStatusHistorico csh = new ContratoStatusHistorico();
            csh.Data = DateTime.Now;
            csh.OperadoraID = operadoraId;
            csh.PropostaNumero = numero;
            csh.Status = Convert.ToInt32(status);

            if (pm != null)
                pm.Save(csh);
            else
                csh.Salvar();

            return csh;
        }

        public static ContratoStatusHistorico Salvar(Int32 numero, Int32 qtdZerosEsquerda, String letra, Object operadoraId, eStatus status, PersistenceManager pm)
        {
            String _numero = EntityBase.GeraNumeroDeContrato(numero, qtdZerosEsquerda, letra);
            return ContratoStatusHistorico.Salvar(_numero, operadoraId, status, pm);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<ContratoStatusHistorico> Carregar(Object operadoraId, String propostaNumero)
        {
            String query = String.Concat("* FROM contratoStatusHistorico WHERE contratostatushistorico_operadoraId = ",
                operadoraId, " AND contratostatushistorico_propostaNumero='", propostaNumero, "' ORDER BY contratostatushistorico_data DESC");

            return LocatorHelper.Instance.ExecuteQuery
                <ContratoStatusHistorico>(query, typeof(ContratoStatusHistorico));
        }
    }

    [DBTable("contrato_valor")]
    public class ContratoValor : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _contratoId;
        Decimal _valorTotal;
        Boolean _status;
        DateTime _data;

        #region propriedades

        [DBFieldInfo("contratovalor_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("contratovalor_contratoId", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId = value; }
        }

        [DBFieldInfo("contratovalor_valorFinal", FieldType.Single)]
        public Decimal ValorTotal
        {
            get { return _valorTotal; }
            set { _valorTotal = value; }
        }

        [DBFieldInfo("contratovalor_status", FieldType.Single)]
        public Boolean Status
        {
            get { return _status; }
            set { _status = value; }
        }

        [DBFieldInfo("contratovalor_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data = value; }
        }

        #endregion

        public ContratoValor(Object id) : this() { _id = id; }
        public ContratoValor() { _status = true; _data = DateTime.Now; }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static ContratoValor CarregarUltimo(Object contratoId, PersistenceManager pm)
        {
            String query = String.Concat(" TOP 1 * FROM contrato_valor WHERE contratovalor_contratoId = ",
                contratoId, " AND contratovalor_status=1 ORDER BY contratovalor_data DESC");

            IList<ContratoValor> ret = LocatorHelper.Instance.
                ExecuteQuery<ContratoValor>(query, typeof(ContratoValor), pm);

            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public static void InsereNovoValorSeNecessario(Object contratoId, Decimal valor, PersistenceManager pm)
        {
            ContratoValor cValor = ContratoValor.CarregarUltimo(contratoId, pm);
            if (cValor == null || cValor.ValorTotal != valor)
            {
                cValor = new ContratoValor();
                cValor.ContratoID = contratoId;
                cValor.Data = DateTime.Now;
                cValor.Status = true;
                cValor.ValorTotal = Convert.ToDecimal(valor);
                pm.Save(cValor);
            }
        }
    }

    [DBTable("taxa")]
    public class Taxa : EntityBase, IPersisteableEntity
    {
        #region campos

        Object _id;
        Object _tabelaValorId;
        Decimal _over;
        Decimal _fixo;
        Boolean _embutido;
        Decimal _valorEmbutido;
        Decimal _percentual;
        DateTime _data;

        #endregion

        #region propriedades

        [DBFieldInfo("taxa_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("taxa_tabelaValorId", FieldType.Single)]
        public Object TabelaValorID
        {
            get { return _tabelaValorId; }
            set { _tabelaValorId = value; }
        }

        [DBFieldInfo("taxa_over", FieldType.Single)]
        public Decimal Over
        {
            get { return _over; }
            set { _over = value; }
        }

        [DBFieldInfo("taxa_fixo", FieldType.Single)]
        public Decimal Fixo
        {
            get { return _fixo; }
            set { _fixo = value; }
        }

        [DBFieldInfo("taxa_embutido", FieldType.Single)]
        public Boolean Embutido
        {
            get { return _embutido; }
            set { _embutido = value; }
        }

        [DBFieldInfo("taxa_valorEmbutido", FieldType.Single)]
        public Decimal ValorEmbutido
        {
            get { return _valorEmbutido; }
            set { _valorEmbutido = value; }
        }

        [DBFieldInfo("taxa_percentualReajuste", FieldType.Single)]
        public Decimal PercentualReajuste
        {
            get { return _percentual; }
            set { _percentual = value; }
        }

        [DBFieldInfo("taxa_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data = value; }
        }

        #endregion

        public Taxa() { _percentual = 0; }
        public Taxa(Object id) : this() { _id = id; }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static Taxa CarregarPorTabela(Object tabelaValorId)
        {
            return CarregarPorTabela(tabelaValorId, null);
        }
        public static Taxa CarregarPorTabela(Object tabelaValorId, PersistenceManager pm)
        {
            String query = "* FROM taxa WHERE taxa_tabelaValorId=" + tabelaValorId + " ORDER BY taxa_data DESC, taxa_id DESC";
            IList<Taxa> lista = LocatorHelper.Instance.ExecuteQuery<Taxa>(query, typeof(Taxa), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }
    }

    [DBTable("estipulante")]
    public class Estipulante : EntityBase, IPersisteableEntity
    {
        public enum eTipoTaxa : int
        {
            PorVida = 0,
            PorProposta
        }

        Object _id;
        String _descricao;
        String _textoBoleto;
        Boolean _ativo;

        #region propriedades

        [DBFieldInfo("estipulante_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("estipulante_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao = value; }
        }

        [DBFieldInfo("estipulante_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo = value; }
        }

        [DBFieldInfo("estipulante_textoBoleto", FieldType.Single)]
        public String TextoBoleto
        {
            get { return _textoBoleto; }
            set { _textoBoleto = value; }
        }

        #endregion

        public Estipulante() { _ativo = true; }
        public Estipulante(Object id) : this() { _id = id; }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            String comm = "DELETE FROM estipulante_taxa WHERE estipulantetaxa_estipulanteId=" + this.ID;
            NonQueryHelper.Instance.ExecuteNonQuery(comm, null);
            base.Remover(this);

        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<Estipulante> Carregar(Boolean apenasAtivos)
        {
            String query = "* FROM estipulante";
            if (apenasAtivos) { query += " WHERE estipulante_ativo=1"; }
            query += " ORDER BY estipulante_descricao";

            return LocatorHelper.Instance.ExecuteQuery<Estipulante>(query, typeof(Estipulante));
        }

        public static Boolean Duplicado(Object estipulanteId, String descricao)
        {
            String qry = "SELECT DISTINCT(estipulante_id) FROM estipulante WHERE estipulante_descricao=@descricao";

            String[] pNames = new String[] { "@descricao" };
            String[] pValues = new String[] { descricao };

            if (estipulanteId != null)
            {
                qry += " AND estipulante_id <> " + estipulanteId;
            }

            Object returned = LocatorHelper.Instance.ExecuteScalar(qry, pNames, pValues);

            if (returned == null || returned == DBNull.Value)
                return false;
            else
                return true;
        }

        public static Object CarregaID(String descricao, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@descricao" };
            String[] paramVl = new String[] { descricao };
            return LocatorHelper.Instance.ExecuteScalar("SELECT estipulante_id FROM estipulante WHERE estipulante_descricao=@descricao", paramNm, paramVl, pm);
        }
    }

    [DBTable("estipulante_taxa")]
    public class EstipulanteTaxa : EntityBase, IPersisteableEntity
    {
        public enum eTipoTaxa : int
        {
            PorVida = 0,
            PorProposta
        }

        Object _id;
        Object _estipulanteId;
        Decimal _valor;
        Int32 _tipoTaxa;
        DateTime _vigencia;

        #region propriedades

        [DBFieldInfo("estipulantetaxa_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("estipulantetaxa_estipulanteId", FieldType.Single)]
        public Object EstipulanteID
        {
            get { return _estipulanteId; }
            set { _estipulanteId = value; }
        }

        [DBFieldInfo("estipulantetaxa_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }

        [DBFieldInfo("estipulantetaxa_tipo", FieldType.Single)]
        public Int32 TipoTaxa
        {
            get { return _tipoTaxa; }
            set { _tipoTaxa = value; }
        }

        public String strTipoTaxa
        {
            get
            {
                if (_tipoTaxa == (Int32)eTipoTaxa.PorProposta)
                    return "POR PROPOSTA";
                else
                    return "POR VIDA";
            }
        }

        [DBFieldInfo("estipulantetaxa_vigencia", FieldType.Single)]
        public DateTime Vigencia
        {
            get { return _vigencia; }
            set { _vigencia = value; }
        }

        #endregion

        public EstipulanteTaxa() { _valor = 0; TipoTaxa = (Int32)eTipoTaxa.PorProposta; }
        public EstipulanteTaxa(Object id) : this() { _id = id; }

        #region métodos EntityBase

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<EstipulanteTaxa> CarregarTodas(Object estipulanteId)
        {
            String query = "* FROM estipulante_taxa WHERE estipulantetaxa_estipulanteId=" + estipulanteId + " ORDER BY estipulantetaxa_vigencia DESC";

            return LocatorHelper.Instance.ExecuteQuery<EstipulanteTaxa>(query, typeof(EstipulanteTaxa));
        }

        public static EstipulanteTaxa CarregarVigente(Object estipulanteId)
        {
            return CarregarVigente(estipulanteId, null);
        }
        public static EstipulanteTaxa CarregarVigente(Object estipulanteId, PersistenceManager pm)
        {
            String query = "TOP 1 * FROM estipulante_taxa WHERE estipulantetaxa_estipulanteId=" + estipulanteId + " ORDER BY estipulantetaxa_vigencia DESC";
            IList<EstipulanteTaxa> lista = LocatorHelper.Instance.ExecuteQuery<EstipulanteTaxa>(query, typeof(EstipulanteTaxa), pm);

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        public static EstipulanteTaxa CarregarVigente(Object estipulanteId, DateTime dataRef)
        {
            return CarregarVigente(estipulanteId, dataRef, null);
        }
        public static EstipulanteTaxa CarregarVigente(Object estipulanteId, DateTime dataRef, PersistenceManager pm)
        {
            String query = String.Concat(
                "TOP 1 * FROM estipulante_taxa WHERE estipulantetaxa_estipulanteId=",
                estipulanteId,
                " and estipulantetaxa_vigencia <= '", dataRef.ToString("yyyy-MM-dd 23:59:59.995"), "'",
                " ORDER BY estipulantetaxa_vigencia DESC");

            IList<EstipulanteTaxa> lista = LocatorHelper.Instance.ExecuteQuery<EstipulanteTaxa>(query, typeof(EstipulanteTaxa), pm);

            if (lista == null)
                return null;
            else
                return lista[0];
        }
    }

    #endregion


}

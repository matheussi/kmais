namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using System.Data.SqlClient;
    using System.Configuration;

    using System.Data.OleDb;
    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;

    public class ErroSumario
    {
        String _contratoAdmNumero;
        String _codigo;
        String _subPlano;
        String _msg;

        public String ContratoAdmNumero
        {
            get { return _contratoAdmNumero; }
            set { _contratoAdmNumero = value; }
        }

        public String Codigo
        {
            get { return _codigo; }
            set { _codigo = value; }
        }

        public String SubPlano
        {
            get { return _subPlano; }
            set { _subPlano = value; }
        }

        public String MSG
        {
            get { return _msg; }
            set { _msg = value; }
        }
    }

    public sealed class ImportFacade
    {
        #region Singleton 

        static ImportFacade _instance;
        public static ImportFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new ImportFacade(); }
                return _instance;
            }
        }
        #endregion

        private ImportFacade() { }

        String mdbPath = @"C:\padrao_import\__.accdb";
        String sqlConn = @"Server=187.16.27.102;timeout=1999999999;Database=padrao_producaoDB;USER ID=sa;PWD=!-sql4f34U!65"; //@"Server=MATHEUSSIPC\SQLEXPRESS2008R2;Database=padrao_producaoDB;USER ID=sa;PWD=lcmaster0000;timeout=1999999999";
        Int32 corretorPerilId = 3;

        String   toString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return String.Empty;
            else
                return Convert.ToString(param);
        }
        Int32    toInt32(Object param)
        {
            if (param == null || param == DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(param);
        }
        DateTime toDateTime(Object param)
        {
            if (param == null || param == DBNull.Value)
                return DateTime.MinValue;
            else
                return Convert.ToDateTime(param, new System.Globalization.CultureInfo("pt-Br"));
        }
        Decimal  toDecimal(Object param)
        {
            if (param == null || param == DBNull.Value)
                return Decimal.Zero;
            else
                return Convert.ToDecimal(param, new System.Globalization.CultureInfo("pt-Br"));
        }

        //String traduzTipoConta(Object tipoConta)
        //{
        //    if (tipoConta == null || tipoConta == DBNull.Value)
        //        return "1"; //conta corrente

        //    return "";
        //}

        /// <summary>
        /// TODO
        /// </summary>
        int traduzEstadoCivil(Object param)
        {
            if (param == null || param == DBNull.Value)
                return 1;

            return 1;
        }
        int traduzSexo(Object param)
        {
            if (param == null || param == DBNull.Value)
                return 0;

            if (Convert.ToString(param).ToUpper() == "M")
                return 1;
            else
                return 0;
        }
        int traduzTipoPessoa(Object param)
        {
            if (param == null || param == DBNull.Value)
                return 1;

            if (Convert.ToString(param).ToUpper() == "PJ")
                return 2;
            else
                return 1;
        }
        Object traduzTipoContrato(Object param)
        {
            String tipo = toString(param);

            if (tipo.ToUpper() == "NOVO")
                return 1;
            else if (tipo.ToUpper() == "ADMINISTRATIVA")
                return 2;
            else if (tipo.ToUpper() == "MIGRACAO")
                return 3;
            else if (tipo.ToUpper() == "COMPRA DE CARENCIA")
                return 4;
            else if (tipo.ToUpper() == "ESPECIAL")
                return 5;

            return null;
        }
        int traduzTipoEndereco(Object param)
        {
            if (toString(param).ToUpper().Trim() == "RESIDENCIAL")
                return Convert.ToInt32(Endereco.TipoEndereco.Residencial);
            else
                return Convert.ToInt32(Endereco.TipoEndereco.Comercial);
        }

        String traduzTipoAcomodacaoParaColuna(Object param)
        {
            if(toString(param) == "QP")
                return " tabelavaloritem_qParticular ";
            else
                return " tabelavaloritem_qComum ";
        }

        ////////////////////////////////////////////////////////////////////////////////////////

        public void ImportarCorretores()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM CORRETORES", conn);
                adp.Fill(dsOrigem, "corretores");
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            Usuario corretor = null;

            try
            {
                IList<Usuario> ret = null;
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    ret = Usuario.CarregarCorretorPorDoc(toString(row["CPF_CNPJ"]), pm);
                    if (ret != null)
                        corretor = ret[0];
                    else
                        corretor = new Usuario();

                    corretor.Agencia                = toString(row["AGENCIA"]);
                    corretor.AlteraValorContratos   = false;
                    corretor.Apelido                = toString(row["APELIDO"]);
                    corretor.Ativo = true;
                    corretor.Banco = toString(row["BANCO"]);
                    corretor.CategoriaID = null;
                    corretor.Celular = toString(row["CELULAR"]);
                    corretor.CelularOperadora = toString(row["CEL_OPERADORA"]);
                    corretor.Codigo = ""; // ToString(row["CEL_OPERADORA"]);
                    corretor.Conta = toString(row["CONTA"]);
                    corretor.ContaTipo = toString(row["TP_CONTA"]);
                    corretor.DataNascimento = toDateTime(row["DATA_NASC"]);
                    if (corretor.DataNascimento.Year < 1753)
                        corretor.DataNascimento = DateTime.MinValue;

                    corretor.DDD1 = toString(row["DDD_1"]);
                    corretor.DDD2 = toString(row["DDD_2"]);
                    corretor.DDD3 = toString(row["CEL_DDD"]);
                    corretor.Documento1 = toString(row["CPF_CNPJ"]);
                    corretor.Documento2 = toString(row["RG_IE"]);
                    corretor.Email = toString(row["EMAIL"]);
                    corretor.EntrevistadoEm = toDateTime(row["DATA_ENTREVISTA"]);
                    corretor.EntrevistadoPor = toString(row["ENTREVISTADOR"]);
                    corretor.EstadoCivil = traduzEstadoCivil(row["ENTREVISTADOR"]);
                    corretor.Favorecido = toString(row["FAVORECIDO"]);
                    corretor.Fone1 = toString(row["TEL1"]);
                    corretor.Fone2 = toString(row["TEL2"]);
                    corretor.LiberaContratos = false;
                    corretor.Nome = toString(row["NOME_CORR"]);
                    corretor.Obs = toString(row["OBS"]);
                    corretor.PerfilID = corretorPerilId;
                    corretor.Ramal1 = toString(row["RAMAL_1"]);
                    corretor.Ramal1 = toString(row["RAMAL_2"]);
                    corretor.Senha = corretor.Documento1;
                    corretor.Sexo = traduzSexo(row["SEXO"]);
                    corretor.SystemUser = false;
                    corretor.TipoPessoa = traduzTipoPessoa(row["TIPO"]);

                    pm.Save(corretor);
                }

                dsOrigem.Dispose();
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
        public void ImportarCorretores_Acerto()
        {
            String qry = "select usuario_id, usuario_filialId, usuariofilial_filialId from usuario inner join usuario_filial on usuariofilial_usuarioId=usuario_id where usuario_filialid is null and usuario_perfilid=" + corretorPerilId;
            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];

            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                if (sb.Length > 0) { sb.Append(" ; "); }
                sb.Append("UPDATE usuario SET usuario_filialId = ");
                sb.Append(row["usuariofilial_filialId"]);
                sb.Append(" WHERE usuario_id=");
                sb.Append(row["usuario_id"]);
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
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
                pm = null;
            }
        }

        /// <summary>
        /// Importar os Beneficiários de um MDB. No caso de Windows 7 colocar o 
        /// MDB em uma pasta que tenha permissão, não utilize o Desktop.
        /// </summary>
        /// <param name="MDBPath">Caminho do MDB.</param>
        public void ImportarBeneficiarios()
        {
            #region Table Header 

            const String NomeColumn = "NOME";
            const String SexoColumn = "SEXO";
            const String CPFColumn = "CPF";
            const String RGColumn = "RG";
            const String DataNascimentoColumn = "NASCIMENTO";
            const String EmailColumn = "EMAIL";
            const String NomeMaeColumn = "NOME_MAE";
            const String DDD1Column = "DDD1";
            const String Telefone1Column = "TEL1";
            const String Ramal1Column = "RAMAL1";
            const String DDD2Column = "DDD2";
            const String Telefone2Column = "TEL2";
            const String Ramal2Column = "RAMAL2";
            const String CelDDDColumn = "CEL_DDD";
            const String CelColumn = "CEL";
            const String CelOperadoraColumn = "CEL_OPERADORA";
            //const String TipoLogr1Column = "TIPO_LOGR1";
            //const String Logr1Column = "LOGRADOURO1";
            //const String NumLogr1Column = "NUM_LOGR1";
            //const String ComplLogr1Column = "COMPL_LOGR1";
            //const String Bairro1Column = "BAIRRO1";
            //const String Cidade1Column = "CIDADE1";
            //const String UF1Column = "UF1";
            //const String CEP1Column = "CEP1";
            //const String TipoEnd1Column = "TIPO_END1";
            //const String TipoLogr2Column = "TIPO_LOGR2";
            //const String Logr2Column = "LOGRADOURO2";
            //const String NumLogr2Column = "NUM_LOGR2";
            //const String ComplLogr2Column = "COMPL_LOGR2";
            //const String Bairro2Column = "BAIRRO2";
            //const String Cidade2Column = "CIDADE2";
            //const String UF2Column = "UF2";
            //const String CEP2Column = "CEP2";
            //const String TipoEnd2Column = "TIPO_END2";

            #endregion

            String connectionString = String.Concat("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=", mdbPath, ";User Id=Admin;");

            OleDbConnection connectionMDB = new OleDbConnection(connectionString);

            try
            {
                connectionMDB.Open();
            }
            catch (Exception ex) { throw ex; }

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
            //String beneficiarioTipoLogr1;
            //String beneficiarioLogr1;
            //String beneficiarioNumLogr1;
            //String beneficiarioComplLogr1;
            //String beneficiarioBairro1;
            //String beneficiarioCidade1;
            //String beneficiarioUF1;
            //String beneficiarioCEP1;
            //String beneficiarioTipoEnd1;
            //String beneficiarioTipoLogr2;
            //String beneficiarioLogr2;
            //String beneficiarioNumLogr2;
            //String beneficiarioComplLogr2;
            //String beneficiarioBairro2;
            //String beneficiarioCidade2;
            //String beneficiarioUF2;
            //String beneficiarioCEP2;
            //String beneficiarioTipoEnd2;
            Beneficiario beneficiario = null;
            //Endereco beneficiarioEndereco1 = null;
            //Endereco beneficiarioEndereco2 = null;

            #endregion

            Int32 i = 0;

            PersistenceManager PMTransaction = new PersistenceManager();
            PMTransaction.BeginTransactionContext();

            while (drBeneficiario.HasRows && drBeneficiario.Read())
            {
                beneficiario = new Beneficiario();
                //beneficiarioEndereco1 = new Endereco();
                //beneficiarioEndereco2 = new Endereco();

                beneficiarioNome = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeColumn)).ToString() : null;
                beneficiarioSexo = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(SexoColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(SexoColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(SexoColumn)).ToString() : null;
                beneficiarioCPF = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CPFColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CPFColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CPFColumn)).ToString() : null;
                beneficiarioRG = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(RGColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(RGColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(RGColumn)).ToString() : null;
                beneficiarioDataNascimento = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DataNascimentoColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DataNascimentoColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DataNascimentoColumn)).ToString() : null;

                beneficiario.ImportID = toInt32(drBeneficiario["ID"]);

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

                #region comentado 

                //beneficiarioTipoLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr1Column)).ToString() : null;
                //beneficiarioLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr1Column)).ToString() : null;
                //beneficiarioNumLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr1Column)).ToString() : null;
                //beneficiarioComplLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr1Column)).ToString() : null;
                //beneficiarioBairro1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro1Column)).ToString() : null;
                //beneficiarioCidade1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade1Column)).ToString() : null;
                //beneficiarioUF1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF1Column)).ToString() : null;
                //beneficiarioCEP1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP1Column)).ToString() : null;
                //beneficiarioTipoEnd1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd1Column)).ToString() : null;

                //beneficiarioTipoLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr2Column)).ToString() : null;
                //beneficiarioLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr2Column)).ToString() : null;
                //beneficiarioNumLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr2Column)).ToString() : null;
                //beneficiarioComplLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr2Column)).ToString() : null;
                //beneficiarioBairro2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro2Column)).ToString() : null;
                //beneficiarioCidade2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade2Column)).ToString() : null;
                //beneficiarioUF2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF2Column)).ToString() : null;
                //beneficiarioCEP2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP2Column)).ToString() : null;
                //beneficiarioTipoEnd2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd2Column)).ToString() : null;

                #endregion

                //beneficiario.ID = Beneficiario.CarregarPorParametro(beneficiario.ImportID, PMTransaction); Beneficiario.CarregarPorParametro(beneficiarioNome, beneficiarioNomeMae, PMTransaction); // .VerificaExistenciaCPF(beneficiarioCPF, Convert.ToDateTime(beneficiarioDataNascimento), beneficiarioNomeMae, PMTransaction);

                beneficiario.Nome = beneficiarioNome;
                beneficiario.CPF = beneficiarioCPF;
                beneficiario.Sexo = (!String.IsNullOrEmpty(beneficiarioSexo)) ? (beneficiarioSexo.Equals("M")) ? "1" : "2" : null;
                beneficiario.RG = beneficiarioRG;

                beneficiario.DataNascimento = Convert.ToDateTime(beneficiarioDataNascimento);
                if (beneficiario.DataNascimento.Year <= 1753) { beneficiario.DataNascimento = DateTime.MinValue; }

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

                #region comentado 
                //beneficiarioEndereco1.DonoId = beneficiario.ID;
                //beneficiarioEndereco1.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                //beneficiarioEndereco1.Logradouro = String.Concat(beneficiarioTipoLogr1.Replace(":", String.Empty), " ", beneficiarioLogr1);
                //beneficiarioEndereco1.Numero = beneficiarioNumLogr1;
                //beneficiarioEndereco1.Complemento = beneficiarioComplLogr1;
                //beneficiarioEndereco1.Bairro = beneficiarioBairro1;
                //beneficiarioEndereco1.Cidade = beneficiarioCidade1;
                //beneficiarioEndereco1.UF = beneficiarioUF1;
                //beneficiarioEndereco1.CEP = beneficiarioCEP1;
                //beneficiarioEndereco1.Tipo = (!String.IsNullOrEmpty(beneficiarioTipoEnd1)) ? (beneficiarioTipoEnd1.Equals("RESIDENCIA")) ? (int)Endereco.TipoEndereco.Residencial : (int)Endereco.TipoEndereco.Comercial : 0; ;

                //try
                //{
                //    beneficiarioEndereco1.Importar(PMTransaction);
                //}
                //catch (Exception)
                //{
                //    PMTransaction.Rollback();
                //    throw;
                //}

                //if (!String.IsNullOrEmpty(beneficiarioLogr2))
                //{
                //    beneficiarioEndereco2.DonoId = beneficiario.ID;
                //    beneficiarioEndereco2.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                //    beneficiarioEndereco2.Logradouro = String.Concat(beneficiarioTipoLogr2.Replace(":", String.Empty), " ", beneficiarioLogr2);
                //    beneficiarioEndereco2.Numero = beneficiarioNumLogr2;
                //    beneficiarioEndereco2.Complemento = beneficiarioComplLogr2;
                //    beneficiarioEndereco2.Bairro = beneficiarioBairro2;
                //    beneficiarioEndereco2.Cidade = beneficiarioCidade2;
                //    beneficiarioEndereco2.UF = beneficiarioUF2;
                //    beneficiarioEndereco2.CEP = beneficiarioCEP2;
                //    beneficiarioEndereco2.Tipo = (!String.IsNullOrEmpty(beneficiarioTipoEnd2)) ? (beneficiarioTipoEnd2.Equals("RESIDENCIA")) ? (int)Endereco.TipoEndereco.Residencial : (int)Endereco.TipoEndereco.Comercial : 0; ;

                //    try
                //    {
                //        beneficiarioEndereco2.Importar(PMTransaction);
                //    }
                //    catch (Exception)
                //    {
                //        PMTransaction.Rollback();
                //        throw;
                //    }
                //}
                #endregion

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

        public void ImportarEnderecosBeneficiarios()
        {
            DataSet dsEnderecos = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM ENDERECOS", conn);
                adp.Fill(dsEnderecos, "data");
                adp.Dispose();
                OleDbCommand ocmd = conn.CreateCommand();

                PersistenceManager pm = null;
                Object ret = null;

                String qryDonoEnd = "SELECT ID FROM BENEFICIARIOS WHERE ID=";
                foreach (DataRow rowEnd in dsEnderecos.Tables[0].Rows)
                {
                    ocmd.CommandText = qryDonoEnd + toString(rowEnd["idNewSys"]);
                    using (OleDbDataReader odr = ocmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (odr.Read())
                        {
                            pm = new PersistenceManager();
                            pm.BeginTransactionContext();

                            try
                            {
                                //DateTime dtNascimento = toDateTime(odr["NASCIMENTO"]);
                                //if(dtNascimento == DateTime.MinValue)
                                //{
                                //    throw new ApplicationException("Data de nascimento inválida.");
                                //}
                                //ret = Beneficiario.CarregarPorParametro(toString(odr["NOME"]), toString(odr["NOME_MAE"]), pm, toDateTime(odr["NASCIMENTO"]), toString(odr["CPF"])); //Beneficiario.VerificaExistenciaCPF(toString(odr["CPF"]), dtNascimento, toString(odr["NOME_MAE"]), pm);
                                ret = Beneficiario.CarregarPorParametro(odr["ID"], pm);

                                if (ret != null)
                                {
                                    Endereco end = new Endereco();
                                    end.Bairro = toString(rowEnd["BAIRRO"]);
                                    end.CEP = toString(rowEnd["CEP"]);
                                    end.Cidade = toString(rowEnd["CIDADE"]);
                                    end.Complemento = toString(rowEnd["COMPL_LOGR"]);
                                    end.DonoId = ret;
                                    end.DonoTipo = Convert.ToInt32(Endereco.TipoDono.Beneficiario);
                                    end.Logradouro = toString(rowEnd["TIPO_LOGR"]) + " " + toString(rowEnd["LOGRADOURO"]);
                                    end.Numero = toString(rowEnd["NUM_LOGR"]);
                                    end.Tipo = traduzTipoEndereco(toString(rowEnd["TIPO"]));
                                    end.UF   = toString(rowEnd["UF"]);

                                    #region Seta ID

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

                                    Values[0] = (end.DonoId != null && end.DonoId.ToString().Length > 0) ? end.DonoId.ToString() : String.Empty;
                                    Values[1] = (end.DonoTipo > -1) ? end.DonoTipo.ToString() : "0";
                                    Values[2] = (!String.IsNullOrEmpty(end.Logradouro)) ? end.Logradouro : String.Empty;
                                    Values[3] = (!String.IsNullOrEmpty(end.Numero)) ? end.Numero : String.Empty;
                                    Values[4] = (!String.IsNullOrEmpty(end.Complemento)) ? end.Complemento : String.Empty;
                                    Values[5] = (!String.IsNullOrEmpty(end.Bairro)) ? end.Bairro : String.Empty;
                                    Values[6] = (!String.IsNullOrEmpty(end.Cidade)) ? end.Cidade : String.Empty;
                                    Values[7] = (!String.IsNullOrEmpty(end.UF)) ? end.UF : String.Empty;
                                    Values[8] = (!String.IsNullOrEmpty(end.CEP)) ? end.CEP : String.Empty;
                                    Values[9] = (end.Tipo > -1) ? end.Tipo.ToString() : "0";

                                    #endregion

                                    String strSQL = String.Concat("SELECT ",
                                          "      endereco_id ",
                                          "FROM endereco ",
                                          "WHERE endereco_donoId = @donoId AND endereco_donoTipo = @donoTipo AND endereco_logradouro = @logradouro AND endereco_numero = @numero AND ",
                                          "      endereco_complemento = @complemento AND endereco_bairro = @bairro AND endereco_cidade = @cidade AND ",
                                          "      endereco_uf = @uf AND endereco_cep = @cep AND endereco_tipo = @tipo");

                                    end.ID = LocatorHelper.Instance.ExecuteScalar(strSQL, Params, Values, pm);

                                    //if (end.ID != null)
                                    //{
                                    //    int aret = 0;
                                    //}

                                    #endregion

                                    //if (end.ID == null) { pm.Save(end); }
                                    pm.Save(end);
                                }
                                else
                                {
                                    int j = 0;
                                }

                                pm.Commit();
                            }
                            catch// (Exception ex)
                            {
                                pm.Rollback();
                                throw; //ex;
                            }
                            finally
                            {
                                pm = null;
                            }
                        }

                        odr.Close();
                    }
                }
            }
        }



        public void ImportarPropostas(ref List<ErroSumario> errors)
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT PROPOSTAS.*, ID_BENEFICIARIO, CPF, NASCIMENTO, NOME, NOME_MAE FROM PROPOSTAS, BENEFICIARIOS_PROPOSTA, BENEFICIARIOS WHERE NUM_CONTRATO=NUM_CONTRATO_FK AND TIT_DEP='T' AND CNPJ_OPERADORA=CNPJ_OPERADORA_FK AND ID=ID_BENEFICIARIO AND num_contrato IN ('21009443','21009444', '21009446') ORDER BY NUM_CONTRATO", conn); //AND num_contrato='21009446' //and num_contrato in ('C2081614','21009443','2036528','21009446','21009444','127317','21004348','84404')
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            //String[] arrNm, arrVl;
            PersistenceManager pm = null; String sql = "";
            Plano planoTemp = null; Object ret = null;

            int i = 0;
            //errors = new List<ErroSumario>();
            foreach (DataRow row in dsOrigem.Tables[0].Rows)
            {
                i++;
                if (toString(row["NUM_CONTRATO"]).Trim().Length == 0)
                {
                    ErroSumario erro = new ErroSumario();
                    erro.ContratoAdmNumero = toString(row["CONTRATO_ADM"]);
                    erro.MSG = "Sem número de contrato. Contrato ADM: " + toString(row["CONTRATO_ADM"]);
                    errors.Add(erro);
                    continue;
                }
                //if (toString(row["NUM_CONTRATO"]).Trim() == "0000001") { continue; }
                if (toString(row["CEP_END_COBR"]).Trim().Length == 0)
                {
                    ErroSumario erro = new ErroSumario();
                    erro.ContratoAdmNumero = toString(row["CONTRATO_ADM"]);
                    erro.MSG = "Sem endereço de cobrança. Contrato: " + toString(row["NUM_CONTRATO"]);
                    errors.Add(erro);
                    continue;
                }

                pm = new PersistenceManager();
                pm.BeginTransactionContext();

                try
                {
                    #region 

                    Contrato contrato = new Contrato();
                    contrato.Adimplente = true;             //TODO: ATENÇÃO - rodar rotina que checa adimplência
                    contrato.Admissao = toDateTime(row["DATA_ADM"]);
                    contrato.Cancelado = false;
                    contrato.CobrarTaxaAssociativa = true;  //TODO: checar
                    contrato.CorretorTerceiroCPF   = null;
                    contrato.CorretorTerceiroNome  = null;
                    contrato.Data = toDateTime(row["DATA_DIGT"]);
                    contrato.DataCancelamento = DateTime.MinValue;

                    ret = LocatorHelper.Instance.ExecuteScalar("SELECT usuario_id FROM usuario WHERE usuario_perfilId=" + corretorPerilId + " AND usuario_documento1='" + toString(row["CPF_CNPJ_CORR"]) + "'", null, null, pm); //Usuario.CarregarCorretorPorDoc(toString(row["CPF_CNPJ_CORR"]), pm);
                    if (ret != null)
                    {
                        contrato.DonoID = ret;
                        ret = null;
                    }
                    else
                    {
                        ErroSumario erro = new ErroSumario();
                        erro.ContratoAdmNumero = toString(row["CONTRATO_ADM"]);
                        erro.Codigo = toString(row["PLANO"]);
                        erro.SubPlano = toString(row["NUM_CONTRATO"]);
                        erro.MSG = "Proposta não possui um corretor ou corretor não localizado no cadastro existente. Documento: " + toString(row["CPF_CNPJ_CORR"]);
                        errors.Add(erro);
                        continue;
                        //throw new ApplicationException("Proposta não possui um corretor ou corretor não localizado no cadastro existente. Documento: " + toString(row["CPF_CNPJ_CORR"]));
                    }

                    contrato.EmailCobranca = null;

                    contrato.Numero = toString(row["NUM_CONTRATO"]);
                    contrato.NumeroID = null;
                    contrato.NumeroMatricula = toString(row["MATRICULA_NUM"]);
                    contrato.Obs = null;

                    contrato.OperadoraID = Operadora.CarregarIDPorCNPJ(toString(row["CNPJ_OPERADORA"]).Replace(".", ""), pm);
                    if (contrato.OperadoraID == null)
                    {
                        //pm.Rollback();
                        //continue;
                        throw new ApplicationException("Operadora não localizada. Cnpj: " + toString(row["CNPJ_OPERADORA"]));
                    }

                    contrato.ContratoADMID = ContratoADM.CarregarID(toString(row["CONTRATO_ADM"]), contrato.OperadoraID, pm);
                    if (contrato.ContratoADMID == null)
                    {
                        //pm.Rollback();
                        ErroSumario erro = new ErroSumario();
                        erro.ContratoAdmNumero = toString(row["CONTRATO_ADM"]) + " (nao localizado)";
                        erro.Codigo = toString(row["PLANO"]);
                        //erro.SubPlano = toString(row["NUM_CONTRATO"]);
                        erro.MSG = "Contrato ADM não localizado: " + toString(row["CONTRATO_ADM"]) + " | Proposta: " + toString(row["NUM_CONTRATO"]);
                        errors.Add(erro);
                        continue;

                        //throw new ApplicationException("Contrato ADM não localizado. Número: " + toString(row["CONTRATO_ADM"]));
                    }

                    contrato.ID = Contrato.CarregaContratoID(contrato.OperadoraID, contrato.Numero, pm);

                    if (contrato.ID != null)
                    {
                        pm.Rollback();
                        continue;
                        //throw new ApplicationException("Proposta ja cadastradao: " + toString(row["NUM_CONTRATO"]));
                    }

                    #region ID dos endereços

                    //if (toString(row["CEP_END_COBR"]).Replace("-", "").Trim() == "05025020")
                    //{
                    //    int j = 0;
                    //}

                    sql = String.Concat("SELECT endereco_id ",
                        "   FROM endereco ",
                        "       INNER JOIN beneficiario ON endereco_donoId=beneficiario_id AND endereco_donoTipo=", Convert.ToInt32(Endereco.TipoDono.Beneficiario),
                        "   WHERE ",
                        "       endereco_cep='", toString(row["CEP_END_COBR"]).Replace("-", "").Trim(), "' AND ",
                        "       importId=", Convert.ToInt32(row["ID_BENEFICIARIO"]));

                    contrato.EnderecoCobrancaID = LocatorHelper.Instance.ExecuteScalar(sql, null, null, pm);
                    if (contrato.EnderecoCobrancaID == null)
                    {
                        //pm.Rollback();
                        //continue;
                        throw new ApplicationException("Endereço de cobrança não localizado: " + toString(row["CEP_END_COBR"]));
                    }

                    sql = String.Concat("SELECT endereco_id ",
                        "   FROM endereco ",
                        "       INNER JOIN beneficiario ON endereco_donoId=beneficiario_id AND endereco_donoTipo=", Convert.ToInt32(Endereco.TipoDono.Beneficiario),
                        "   WHERE ",
                        "       endereco_cep='", toString(row["CEP_END_REF"]).Replace("-", "").Trim(), "' AND ",
                        "       importId=", Convert.ToInt32(row["ID_BENEFICIARIO"]));

                    contrato.EnderecoReferenciaID = LocatorHelper.Instance.ExecuteScalar(sql, null, null, pm);
                    if (contrato.EnderecoReferenciaID == null)
                    {
                        //pm.Rollback();
                        //continue;
                        throw new ApplicationException("Endereço de referência não localizado: " + toString(row["CEP_END_REF"]));
                    }

                    #endregion

                    contrato.EstipulanteID = Estipulante.CarregaID(toString(row["ESTIPULANTE"]), pm);
                    if (contrato.EstipulanteID == null)
                    {
                        throw new ApplicationException("Estipulantes não localizado: " + toString(row["ESTIPULANTE"]));
                    }

                    contrato.OperadorTmktID = null;     //TODO
                    contrato.Pendente = false;

                    contrato.PlanoID = Plano.CarregarID(contrato.ContratoADMID, toString(row["PLANO"]), toString(row["SUB_PLANO"]), pm);
                    if (contrato.PlanoID == null)
                    {
                        ErroSumario erro = new ErroSumario();
                        erro.ContratoAdmNumero = toString(row["CONTRATO_ADM"]);
                        erro.Codigo = toString(row["PLANO"]);
                        erro.SubPlano = toString(row["NUM_CONTRATO"]);
                        erro.MSG = "Plano não localizado. Código e subplano: " + toString(row["PLANO"]) + " | " + toString(row["SUB_PLANO"]) + " | PROPOSTA: " + toString(row["NUM_CONTRATO"]) + " | CONTRATO ADM: " + toString(row["CONTRATO_ADM"]);
                        errors.Add(erro);
                        //pm.Rollback();
                        continue;
                        //throw new ApplicationException("Plano não localizado. Código e subplano: " + toString(row["PLANO"]) + " | " + toString(row["SUB_PLANO"]));
                    }

                    contrato.Rascunho = false;
                    contrato.ResponsavelCPF = toString(row["CPF_RESP"]);
                    contrato.ResponsavelDataNascimento = toDateTime(row["DATA_NASC_RESP"]);
                    contrato.ResponsavelNome = toString(row["RESPONSÁVEL"]);
                    contrato.ResponsavelParentescoID = null; //TODO
                    contrato.ResponsavelRG = toString(row["RG_RESP"]);
                    contrato.ResponsavelSexo = null;        //TODO

                    contrato.SuperiorTerceiroCPF = null;
                    contrato.SuperiorTerceiroNome = null;

                    planoTemp = new Plano(contrato.PlanoID);
                    pm.Load(planoTemp);
                    if (planoTemp.Codigo == toString(row["PLANO"]) && planoTemp.SubPlano == toString(row["SUB_PLANO"]))
                        contrato.TipoAcomodacao = Convert.ToInt32(Contrato.eTipoAcomodacao.quartoComun);
                    else
                        contrato.TipoAcomodacao = Convert.ToInt32(Contrato.eTipoAcomodacao.quartoParticular);

                    contrato.TipoContratoID = traduzTipoContrato(row["TIPO_PROPOSTA"]);
                    if (contrato.TipoContratoID == null)
                    {
                        throw new ApplicationException("Tipo de contrato não localizado: " + toString(row["TIPO_PROPOSTA"]));
                    }

                    contrato.Vigencia = toDateTime(row["DATA_VIGENCIA"]);
                    if (contrato.Vigencia == DateTime.MinValue)
                    {
                        throw new ApplicationException("Vigência inválida: " + toString(row["DATA_VIGENCIA"]));
                    }

                    contrato.Vencimento = toDateTime(row["PRIM_VENC"]);
                    if (contrato.Vencimento == DateTime.MinValue)
                    {
                        throw new ApplicationException("Vencimento inválido: " + toString(row["PRIM_VENC"]));
                    }

                    pm.Save(contrato);
                    pm.Commit();

                    #endregion
                }
                catch// (Exception ex)
                {
                    pm.Rollback();
                    throw;// ex;
                }
                finally
                {
                    pm = null;
                }
            }

            dsOrigem.Dispose();
        }

        public void ImportarPropostaBeneficiarios()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT BENEFICIARIOS_PROPOSTA.*, ID, NOME, NOME_MAE, NASCIMENTO, CPF, NOME_MAE, DATA_ADM, DATA_VIGENCIA, VALOR_PRÊMIO, VALORES_ADICIONAIS, CNPJ_OPERADORA, CONTRATO_ADM, CEP_END_COBR FROM BENEFICIARIOS_PROPOSTA, BENEFICIARIOS, PROPOSTAS WHERE ID_BENEFICIARIO=ID AND NUM_CONTRATO=NUM_CONTRATO_FK AND CNPJ_OPERADORA=CNPJ_OPERADORA_FK AND num_contrato IN ('21009443','21009444', '21009446') ORDER BY NUM_CONTRATO_FK", conn); //AND num_contrato IN('11018462', '21017564')
                adp.Fill(dsOrigem, "contrato_beneficiarios");
                adp.Dispose();
            }

            Object operadoraId = null, contratoAdmId = null;
            PersistenceManager pm = null; ContratoADMParentescoAgregado parentesco = null;
            foreach (DataRow row in dsOrigem.Tables[0].Rows)
            {
                if (toString(row["NUM_CONTRATO_FK"]).Trim().Length == 0) { continue; }
                if (toString(row["CEP_END_COBR"]).Trim().Length == 0) { continue; }

                pm = new PersistenceManager();
                pm.BeginTransactionContext();

                ContratoBeneficiario cb = null;

                try
                {
                    cb = new ContratoBeneficiario();
                    cb.Altura = toDecimal(row["ALTURA"]);
                    cb.Ativo = true;

                    cb.BeneficiarioID = Beneficiario.CarregarPorParametro(row["ID"], pm); //Beneficiario.CarregarPorParametro(toString(row["NOME"]), toString(row["NOME_MAE"]), pm, toDateTime(row["NASCIMENTO"]), toString(row["CPF"]));
                    if (cb.BeneficiarioID == null)
                    {
                        pm.Rollback();
                        continue;
                    }

                    cb.CarenciaCodigo = null; //TODO
                    cb.CarenciaContratoTempo = toInt32(row["TEMPO_CONTRATO"]);
                    cb.CarenciaMatriculaNumero = toString(row["MATRICULA_ANT"]);
                    cb.CarenciaOperadoraID = null; //TODO: !!!
                    cb.CarenciaOperadoraDescricao = toString(row["COMPRA_CAR_OPERADORA"]);

                    operadoraId   = Operadora.CarregarIDPorCNPJ(toString(row["CNPJ_OPERADORA"]), pm);
                    cb.ContratoID = Contrato.CarregaContratoID(operadoraId, toString(row["NUM_CONTRATO_FK"]), pm);

                    if (cb.ContratoID == null)
                    {
                        pm.Rollback();
                        continue;
                    }

                    cb.Data = toDateTime(row["DATA_ADM"]);
                    cb.DataCasamento = DateTime.MinValue;
                    cb.EstadoCivilID = EstadoCivil.CarregarID(toString(row["EST_CIVIL"]), operadoraId, pm);

                    if (cb.EstadoCivilID == null)
                    {
                        EstadoCivil ec = new EstadoCivil();
                        ec.Codigo = toString(row["EST_CIVIL"]);
                        ec.Descricao = toString(row["EST_CIVIL"]);
                        ec.OperadoraID = operadoraId;
                        pm.Save(ec);
                        cb.EstadoCivilID = ec.ID;
                    }

                    cb.NumeroSequencial = toInt32(row["SEQUENCIA"]);

                    if(toString(row["PARENTESCO"]).Trim().ToUpper() != "TITULAR")
                    {
                        contratoAdmId = ContratoADM.CarregarID(toString(row["CONTRATO_ADM"]), pm);
                        parentesco = ContratoADMParentescoAgregado.Carregar(contratoAdmId, toString(row["PARENTESCO"]).Trim().ToUpper(), pm);
                        if (parentesco != null)
                            cb.ParentescoID = parentesco.ID;
                        else
                        {
                            parentesco = new ContratoADMParentescoAgregado();
                            parentesco.ContratoAdmID = contratoAdmId;
                            parentesco.ParentescoDescricao = toString(row["PARENTESCO"]).Trim().ToUpper();
                            parentesco.ParentescoCodigo = parentesco.ParentescoDescricao;
                            parentesco.ParentescoTipo   = Convert.ToInt32(Parentesco.eTipo.Dependente);
                            pm.Save(parentesco);
                            cb.ParentescoID = parentesco.ID;
                        }
                    }
                    else
                        cb.ParentescoID = null;

                    cb.Peso = toDecimal(row["PESO"]);
                    cb.Status = Convert.ToInt32(ContratoBeneficiario.eStatus.Incluido);

                    if (toString(row["TIT_DEP"]).ToUpper().Trim() == "T")
                        cb.Tipo = Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular);
                    else
                        cb.Tipo = Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Dependente);

                    cb.Valor    = toDecimal(row["VALOR_PRÊMIO"]) + toDecimal(row["VALORES_ADICIONAIS"]);
                    cb.Vigencia = toDateTime(row["DATA_VIGENCIA"]);

                    //cb.ID = ContratoBeneficiario.CarregaID(cb.ContratoID, cb.BeneficiarioID, pm);

                    pm.Save(cb);
                    pm.Commit();
                }
                catch
                {
                    pm.Rollback();
                    throw;
                }
                finally
                {
                    pm = null;
                }
            }

            dsOrigem.Dispose();
        }



        public void ImportarCobrancas()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT COBRANCAS.*, CEP_END_COBR FROM COBRANCAS, PROPOSTAS WHERE NUM_CONTRATO=NUM_CONTRATO_FK AND CNPJ_OPERADORA=CNPJ_OPERADORA_FK AND (COBRANÇA_PARCELA = 8 OR COBRANÇA_PARCELA = 8) ORDER BY NUM_CONTRATO_FK, COBRANÇA_PARCELA", conn);
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            PersistenceManager pm = null;
            Object propostaId = null, operadoraId = null;
            Cobranca cob = null;
            List<Cobranca> cobrancas = new List<Cobranca>();

            pm = new PersistenceManager();
            pm.BeginTransactionContext();

            foreach (DataRow row in dsOrigem.Tables[0].Rows)
            {
                if (toString(row["NUM_CONTRATO_FK"]).Trim().Length == 0) { continue; }
                //if (toString(row["NUM_CONTRATO"]).Trim() == "0000001") { continue; }
                if (toString(row["CEP_END_COBR"]).Trim().Length == 0) { continue; }

                try
                {
                    operadoraId = Operadora.CarregarIDPorCNPJ(toString(row["CNPJ_OPERADORA_FK"]), pm);
                    if (operadoraId == null)
                    {
                        throw new ApplicationException("Operadora não localizada: " + toString(row["CNPJ_OPERADORA_FK"]));
                    }

                    propostaId = Contrato.CarregaContratoID(operadoraId, toString(row["NUM_CONTRATO_FK"]), pm);
                    if (propostaId == null)
                    {
                        throw new ApplicationException("Proposta não localizada: Núm.:" + toString(row["NUM_CONTRATO_FK"]) + " OperadoraID: " + operadoraId.ToString());
                    }

                    cob = new Cobranca();
                    cob.ArquivoIDUltimoEnvio    = null;
                    cob.Cancelada               = Convert.ToBoolean(row["Cobrança_cancelada"]);
                    cob.CobrancaRefID           = null;
                    cob.ComissaoPaga            = Convert.ToBoolean(row["Cobrança_comissaoPaga"]);
                    cob.DataCriacao             = DateTime.Now;
                    cob.DataPgto                = toDateTime(row["Cobrança_dataPagto"]);
                    cob.DataVencimento          = toDateTime(row["Cobrança_dataVencimento"]);
                    cob.Pago                    = Convert.ToBoolean(row["Cobrança_pago"]);
                    cob.Parcela                 = toInt32(row["Cobrança_parcela"]);
                    cob.PropostaID              = propostaId;
                    cob.Tipo                    = toInt32(row["Cobrança_tipo"]);
                    cob.Valor                   = toDecimal(row["Cobrança_valor"]);
                    cob.ValorPgto               = toDecimal(row["Cobrança_valorPagto"]);

                    if (cob.DataPgto.Year <= 1753)
                        cob.DataPgto = DateTime.MinValue;

                    if (cob.DataVencimento.Year <= 1753)
                        cob.DataVencimento = DateTime.MinValue;

                    cobrancas.Add(cob);

                    //pm.Save(cob);
                    //pm.Commit();
                }
                catch
                {
                    pm.Rollback();
                    throw;
                }
            }

            pm.Commit();
            pm = null;

            dsOrigem.Dispose();

            if (cobrancas.Count > 0)
            {
                pm = new PersistenceManager();
                pm.BeginTransactionContext();

                try
                {
                    foreach (Cobranca cobranca in cobrancas)
                    {
                        pm.Save(cobranca);
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
                    pm = null;
                }
            }
        }

        public String ImportarCobrancas_V2(int parcela)
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                //OleDbDataAdapter adp = new OleDbDataAdapter("SELECT COBRANCAS.*, CEP_END_COBR FROM COBRANCAS, PROPOSTAS WHERE NUM_CONTRATO=NUM_CONTRATO_FK AND CNPJ_OPERADORA=CNPJ_OPERADORA_FK AND (COBRANÇA_PARCELA = " + parcela.ToString() + ") ORDER BY NUM_CONTRATO_FK, COBRANÇA_PARCELA", conn);
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM COBRANCAS WHERE COBRANÇA_PARCELA = " + parcela.ToString() + " ORDER BY NUM_CONTRATO_FK, COBRANÇA_PARCELA", conn); //and num_contrato_fk IN ('21009443','21009444', '21009446') 
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            Object propostaId = null, operadoraId = null;

            StringBuilder sb = new StringBuilder();
            DateTime data = DateTime.MinValue;

            #region 

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                int i = 0, j = 0;
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    if (toString(row["NUM_CONTRATO_FK"]).Trim().Length == 0) { continue; }
                    ////if (toString(row["NUM_CONTRATO"]).Trim() == "0000001") { continue; }
                    //if (toString(row["CEP_END_COBR"]).Trim().Length == 0) { continue; }

                    try
                    {
                        cmd.CommandText = "SELECT operadora_id FROM operadora WHERE operadora_cnpj='" + toString(row["CNPJ_OPERADORA_FK"]).Replace(".", "").Replace("/","").Replace("-","") + "'";
                        operadoraId = cmd.ExecuteScalar();
                        //operadoraId = Operadora.CarregarIDPorCNPJ(toString(row["CNPJ_OPERADORA_FK"]), pm);
                        if (operadoraId == null)
                        {
                            //throw new ApplicationException("Operadora não localizada: " + toString(row["CNPJ_OPERADORA_FK"]));
                            continue;
                        }

                        cmd.CommandText = "SELECT contrato_id FROM contrato WHERE contrato_operadoraId=" + operadoraId + " AND contrato_numero='" + toString(row["NUM_CONTRATO_FK"]) + "'";
                        propostaId = cmd.ExecuteScalar();
                        //propostaId = Contrato.CarregaContratoID(operadoraId, toString(row["NUM_CONTRATO_FK"]), pm);
                        if (propostaId == null)
                        {
                            //throw new ApplicationException("Proposta não localizada: Núm.:" + toString(row["NUM_CONTRATO_FK"]) + " OperadoraID: " + operadoraId.ToString());
                            continue;
                        }

                        sb.Append("IF NOT EXISTS(SELECT cobranca_id FROM cobranca WHERE cobranca_propostaId=");
                        sb.Append(propostaId);
                        sb.Append(" AND cobranca_parcela="); sb.Append(row["Cobrança_parcela"]);
                        sb.Append(") BEGIN ");

                        sb.Append("INSERT INTO cobranca (cobranca_nossoNumero, cobranca_propostaId, cobranca_cancelada,cobranca_comissaoPaga,cobranca_dataCriacao,cobranca_dataPagto, cobranca_dataVencimento,cobranca_pago,cobranca_parcela,cobranca_tipo,cobranca_valor,cobranca_valorPagto) VALUES (");
                        sb.Append("'");
                        sb.Append(row["Nosso_Numero"]);
                        sb.Append("',");
                        sb.Append(propostaId);
                        sb.Append(",");
                        sb.Append(Convert.ToInt32(Convert.ToBoolean(row["Cobrança_cancelada"])));
                        sb.Append(",");
                        sb.Append(Convert.ToInt32(Convert.ToBoolean(row["Cobrança_comissaoPaga"])));
                        sb.Append(",'");
                        sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        #region data pagamento
                        data = toDateTime(row["Cobrança_dataPagto"]);
                        if (data != DateTime.MinValue && data.Year > 1753)
                        {
                            sb.Append("','");
                            sb.Append(data.ToString("yyyy-MM-dd"));
                            sb.Append("',");
                        }
                        else
                        {
                            sb.Append("',NULL,");
                        }
                        #endregion

                        #region data vencimento
                        data = toDateTime(row["Cobrança_dataVencimento"]);
                        if (data != DateTime.MinValue && data.Year > 1753)
                        {
                            sb.Append("'");
                            sb.Append(data.ToString("yyyy-MM-dd 23:59:59"));
                            sb.Append("',");
                        }
                        else
                        {
                            sb.Append("NULL,");
                        }
                        #endregion

                        sb.Append(Convert.ToInt32(Convert.ToBoolean(row["Cobrança_pago"])));
                        sb.Append(",");
                        sb.Append(toInt32(row["Cobrança_parcela"]));
                        sb.Append(",");
                        sb.Append(toInt32(row["Cobrança_tipo"]));
                        sb.Append(",'");
                        sb.Append(toDecimal(row["Cobrança_valor"]).ToString("N2").Replace(".", "").Replace(",", "."));
                        sb.Append("','");
                        sb.Append(toDecimal(row["Cobrança_valorPagto"]).ToString("N2").Replace(".", "").Replace(",", "."));
                        sb.Append("') ");
                        sb.Append(" END  ");

                        #region update

                        //sb.Append(" ELSE BEGIN ");

                        //sb.Append("UPDATE cobranca SET cobranca_pago=");
                        //sb.Append(Convert.ToInt32(Convert.ToBoolean(row["Cobrança_pago"])));
                        //sb.Append(", cobranca_valor='");
                        //sb.Append(toDecimal(row["Cobrança_valor"]).ToString("N2").Replace(".", "").Replace(",", "."));
                        //sb.Append("', cobranca_valorPagto='");
                        //sb.Append(toDecimal(row["Cobrança_valorPagto"]).ToString("N2").Replace(".", "").Replace(",", "."));
                        //sb.Append("', cobranca_dataPagto=");

                        //#region data pagamento
                        //data = toDateTime(row["Cobrança_dataPagto"]);
                        //if (data != DateTime.MinValue && data.Year > 1753)
                        //{
                        //    sb.Append("'");
                        //    sb.Append(data.ToString("yyyy-MM-dd"));
                        //    sb.Append("'");
                        //}
                        //else
                        //{
                        //    sb.Append("NULL");
                        //}
                        //#endregion

                        //sb.Append(", cobranca_comissaoPaga=");
                        //sb.Append(Convert.ToInt32(Convert.ToBoolean(row["Cobrança_comissaoPaga"])));

                        //sb.Append(" WHERE cobranca_propostaId="); sb.Append(propostaId);
                        //sb.Append(" AND cobranca_parcela="); sb.Append(row["Cobrança_parcela"]);

                        //sb.Append(" END ");
                        #endregion
                        sb.Append(Environment.NewLine);

                        i++; j++;

                        if (j == 250)
                        {
                            cmd.CommandText = sb.ToString();
                            cmd.ExecuteNonQuery();
                            sb.Remove(0, sb.Length);
                            j = 0;
                        }
                        
                    }
                    catch
                    {
                        throw;
                    }

                    //break;///////////////////////////////
                }

                if (sb.Length > 0)
                {
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    sb.Remove(0, sb.Length);
                }
            }
            #endregion

            dsOrigem.Dispose();

            return sb.ToString();
        }

        public void GerarCobrancasComoENVIADAS()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM boletos", conn);
                adp.Fill(dsOrigem, "boletos");
                adp.Dispose();
                conn.Close();
            }

            PersistenceManager pm = null;

            Contrato contrato = null;
            Cobranca cobranca = null, cobrancaTemp = null; 
            List<CobrancaComposite> composite = null;
            foreach (DataRow row in dsOrigem.Tables[0].Rows)
            {
                pm = new PersistenceManager();
                pm.BeginTransactionContext();

                contrato = Contrato.CarregarParcialPorCodCobranca(row["ID_COBRANCA"], pm);

                if (contrato == null || contrato.ID == null) { pm.Rollback(); pm.Dispose(); continue; }

                cobranca = Cobranca.CarregarPor(contrato.ID, Convert.ToInt32(row["PARCELA"]), Convert.ToInt32(Cobranca.eTipo.Normal), pm);

                if (cobranca != null && cobranca.ID != null)
                {
                    cobranca.ArquivoIDUltimoEnvio = -1;
                    pm.Save(cobranca);
                }
                else
                {
                    cobrancaTemp = Cobranca.CarregarPor(contrato.ID, (Convert.ToInt32(row["PARCELA"])-1), Convert.ToInt32(Cobranca.eTipo.Normal), pm);
                    if (cobrancaTemp == null || cobrancaTemp.ID == null) { pm.Rollback(); pm.Dispose(); continue; }

                    cobranca = new Cobranca();
                    cobranca.ArquivoIDUltimoEnvio = -2;
                    cobranca.Cancelada = false;
                    cobranca.CobrancaRefID = null;
                    cobranca.ComissaoPaga = true;
                    cobranca.DataCriacao = DateTime.Now;
                    cobranca.DataVencimento = cobrancaTemp.DataVencimento.AddMonths(1);
                    cobranca.DataVencimentoISENCAOJURO = cobrancaTemp.DataVencimentoISENCAOJURO;
                    cobranca.Pago = false;
                    cobranca.Parcela = Convert.ToInt32(row["PARCELA"]);
                    cobranca.PropostaID = contrato.ID;
                    cobranca.Tipo = (int)Cobranca.eTipo.Normal;
                    cobranca.Valor = Contrato.CalculaValorDaProposta2(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite, false);
                    pm.Save(cobranca);
                }

                pm.Commit();
            }
        }

        public void ImportarFiliaisParaProdutores()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM FILIAL", conn);
                adp.Fill(dsOrigem, "resultset");
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                IList<Usuario> produtores = null;
                Object filialId = null;
                DateTime data = DateTime.MinValue;
                UsuarioFilial uf = null;
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    produtores = Usuario.CarregarCorretorPorDoc(toString(row["CORRETOR_DOC"]), pm);
                    if (produtores == null)
                    {
                        throw new ApplicationException("Corretor não encontrado: " + toString(row["CORRETOR_DOC"]));
                    }
                    if (produtores.Count > 1)
                    {
                        throw new ApplicationException("Mais de um corretor encontrado: " + toString(row["CORRETOR_DOC"]));
                    }

                    filialId = LocatorHelper.Instance.ExecuteScalar("SELECT filial_id FROM filial WHERE filial_nome='" + toString(row["FILIAL"]).Trim() + "'", null, null, pm);
                    if (filialId == null || filialId == DBNull.Value)
                    {
                        throw new ApplicationException("Filial não encontrada: " + toString(row["FILIAL"]));
                    }

                    data = toDateTime(row["DATA"]);
                    if (data == DateTime.MinValue)
                    {
                        throw new ApplicationException("Data inválida: " + toString(row["DATA"]));
                    }

                    uf = new UsuarioFilial();
                    uf.Data = data;
                    uf.FilialID = filialId;
                    uf.UsuarioID = produtores[0].ID;
                    pm.Save(uf);
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
                dsOrigem.Dispose();
                pm.Dispose();
                pm = null;
            }
        }

        public void ImportarGruposDeVendaParaProdutores()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM GRUPO_DE_VENDA", conn);
                adp.Fill(dsOrigem, "resultset");
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                IList<Usuario> produtores = null;
                Object grupoId = null;
                DateTime data = DateTime.MinValue;
                UsuarioGrupoVenda ugv = null;
                DataTable dt = dsOrigem.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    produtores = Usuario.CarregarCorretorPorDoc(toString(row["CORRETOR_DOC"]), pm);
                    if (produtores == null)
                    {
                        throw new ApplicationException("Corretor não encontrado: " + toString(row["CORRETOR_DOC"]));
                    }
                    if (produtores.Count > 1)
                    {
                        throw new ApplicationException("Mais de um corretor encontrado: " + toString(row["CORRETOR_DOC"]));
                    }

                    grupoId = LocatorHelper.Instance.ExecuteScalar("SELECT grupovenda_id FROM grupo_venda WHERE grupovenda_descricao='" + toString(row["GRUPO"]).Trim() + "'", null, null, pm);
                    if (grupoId == null || grupoId == DBNull.Value)
                    {
                        throw new ApplicationException("Grupo de Venda não encontrado: " + toString(row["GRUPO"]));
                    }

                    data = toDateTime(row["DATA_VIG"]);
                    if (data == DateTime.MinValue)
                    {
                        throw new ApplicationException("Data inválida: " + toString(row["DATA_VIG"]));
                    }

                    ugv = new UsuarioGrupoVenda();
                    ugv.Data = data;
                    ugv.GrupoVendaID = grupoId;
                    ugv.UsuarioID = produtores[0].ID;
                    pm.Save(ugv);
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
                dsOrigem.Dispose();
                pm.Dispose();
                pm = null;
            }
        }

        public void ImportarConfiguracoesDeEquipes()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM SUBORDINACAO WHERE corretor_doc <> '03235631801'", conn);
                adp.Fill(dsOrigem, "resultset");
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                IList<Usuario> subordinados = null; IList<Usuario> superiores = null;
                //Object perfilId = null;
                DateTime data = DateTime.MinValue;
                SuperiorSubordinado ss = null;
                DataTable dt = dsOrigem.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    if (toString(row["CORRETOR_DOC"]).Trim() == "03235631801") //manobra
                    {
                        subordinados = new List<Usuario>();
                        Usuario us = new Usuario();
                        us.ID = 603;
                        subordinados.Add(us);
                    }
                    else
                    {
                        subordinados = Usuario.CarregarCorretorPorDoc(toString(row["CORRETOR_DOC"]).Trim(), pm);
                    }

                    if (subordinados == null)
                    {
                        throw new ApplicationException("Subordinado não encontrado: " + toString(row["CORRETOR_DOC"]));
                    }
                    //if (subordinados.Count > 1)
                    //{
                    //    throw new ApplicationException("Mais de um subordinado encontrado: " + toString(row["CORRETOR_DOC"]));
                    //}

                    if (toString(row["CORRETOR_DOC"]).Trim() == "03235631801") //manobra
                    {
                        superiores = new List<Usuario>();
                        Usuario us = new Usuario();
                        us.ID = 15;
                        superiores.Add(us);
                    }
                    else
                    {
                        superiores = Usuario.CarregarCorretorPorDoc(toString(row["SUPERIOR_DOC"]).Trim(), pm);
                    }
                    if (superiores == null)
                    {
                        throw new ApplicationException("Superior não encontrado: " + toString(row["SUPERIOR_DOC"]));
                    }
                    //if (superiores.Count > 1)
                    //{
                    //    throw new ApplicationException("Mais de um superior encontrado: " + toString(row["SUPERIOR_DOC"]));
                    //}

                    data = toDateTime(row["DATA_VIG"]);
                    if (data == DateTime.MinValue)
                    {
                        throw new ApplicationException("Data inválida: " + toString(row["DATA_VIG"]));
                    }

                    ss = new SuperiorSubordinado();
                    ss.Data = data;
                    //ss.SuperiorPerfilID = perfilId;
                    ss.SubordinadoID = subordinados[0].ID;
                    ss.SuperiorID    = superiores[0].ID;
                    pm.Save(ss);
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
                dsOrigem.Dispose();
                pm.Dispose();
                pm = null;
            }
        }
        public void ImportarConfiguracoesDeEquipes_Acerto()
        {
            DataTable dtOrigem = LocatorHelper.Instance.ExecuteQuery("SELECT * FROM superior_subordinado", "resultset").Tables[0];
            StringBuilder sb = new StringBuilder();

            foreach (DataRow rowOrigem in dtOrigem.Rows)
            {
                sb.Append("UPDATE usuario SET usuario_superiorId=");
                sb.Append(rowOrigem["ss_superiorId"]);
                sb.Append(" WHERE usuario_id=");
                sb.Append(rowOrigem["ss_subordinadoId"]);
                sb.Append("; "); 
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
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
                pm = null;
                dtOrigem.Dispose();
            }
        }

        public void ImportarTabelasDeValor()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM TABELA ORDER BY CONTRATO,COD_PLANO,COD_SUBPLANO,ACOMODACAO,IDADE_INI", conn);
                adp.Fill(dsOrigem, "resultset");
                adp.Dispose();
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                DataTable dt = dsOrigem.Tables[0];
                Object contratoAdmId  = null, planoId = null, tabelaId = null;
                DataTable dtLinhasItem = null;
                DataRow[] rows = null;
                String sql = "", qcValor = "", qpValor = "";
                TabelaValor tabela = null;
                foreach (DataRow row in dt.Rows)
                {
                    contratoAdmId = ContratoADM.CarregarID(toString(row["CONTRATO"]), pm);
                    if (contratoAdmId == null)
                    {
                        continue;
                    }

                    planoId = Plano.CarregarID(contratoAdmId, toString(row["COD_PLANO"]), toString(row["COD_SUBPLANO"]), pm);
                    if (planoId == null)
                    {
                        continue;
                    }

                    //checa se a linha do item da tabela de valor ja existe.
                    sql = String.Concat("SELECT tabelavalor_id, tabela_valor_item.* ",
                        " FROM tabela_valor ",
                        "   LEFT JOIN tabela_valor_item ON tabelavalor_id=tabelavaloritem_tabelaid ",
                        " WHERE tabelavalor_contratoId=", contratoAdmId, " AND tabelavaloritem_planoId=", planoId);

                    dtLinhasItem = LocatorHelper.Instance.ExecuteQuery(sql, "resultset", pm).Tables[0];

                    if (dtLinhasItem != null && dtLinhasItem.Rows != null && dtLinhasItem.Rows.Count > 0)
                    {
                        //achou. agora, checa se existe a linha para o intervalo de idade 
                        rows = dtLinhasItem.Select("tabelavaloritem_idadeInicio=" + toString(row["IDADE_INI"]) + " AND tabelavaloritem_idadeFim=" + toString(row["IDADE_FIN"]));

                        if (rows != null && rows.Length > 0)
                        {
                            //achou a linha. atualiza.
                            sql = String.Concat("UPDATE tabela_valor_item SET ",
                                traduzTipoAcomodacaoParaColuna(row["ACOMODACAO"]), 
                                "='",
                                toDecimal(row["VALOR_TAB1"]).ToString("N2").Replace(".", "").Replace(",", "."), 
                                "' WHERE tabelavaloritem_id=", 
                                toString(rows[0]["tabelavaloritem_id"]));

                            NonQueryHelper.Instance.ExecuteNonQuery(sql, pm);
                        }
                        else
                        {
                            //nao achou a linha, insere.
                            qcValor = "0.00"; qpValor = "0.00";
                            if (toString(row["ACOMODACAO"]) == "QP")
                                qpValor = toDecimal(row["VALOR_TAB1"]).ToString("N2").Replace(".", "").Replace(",", ".");
                            else
                                qcValor = toDecimal(row["VALOR_TAB1"]).ToString("N2").Replace(".", "").Replace(",", ".");

                            sql = String.Concat("INSERT INTO tabela_valor_item (tabelavaloritem_tabelaid, tabelavaloritem_planoId, tabelavaloritem_idadeInicio, tabelavaloritem_idadeFim, tabelavaloritem_qComum, tabelavaloritem_qParticular) VALUES (",
                                dtLinhasItem.Rows[0]["tabelavalor_id"], ",",
                                planoId, ",",
                                row["IDADE_INI"], ",",
                                row["IDADE_FIN"], ",",
                                "'", qcValor, "',",
                                "'", qpValor, "')");

                            NonQueryHelper.Instance.ExecuteNonQuery(sql, pm);
                        }
                    }
                    else
                    {
                        //nao achou.checa se a tabela existe.
                        //se nao existe, insere a tabela. em seguida, insere o item da tabela
                        tabelaId = LocatorHelper.Instance.ExecuteScalar("SELECT tabelavalor_id FROM tabela_valor WHERE tabelavalor_contratoId=" + contratoAdmId, null, null, pm);
                        tabela = new TabelaValor();

                        if (tabelaId == null)
                        {
                            tabela.ContratoID = contratoAdmId;
                            //tabela.Data = agora;
                            tabela.Inicio = new DateTime(2010, 01, 01, 0, 0, 0, 0);
                            tabela.Fim = tabela.Inicio.AddYears(2);
                            pm.Save(tabela);
                        }
                        else
                        {
                            tabela.ID = tabelaId;
                        }

                        //nao achou a linha, insere.
                        qcValor = "0.00"; qpValor = "0.00";
                        if (toString(row["ACOMODACAO"]) == "QP")
                            qpValor = toDecimal(row["VALOR_TAB1"]).ToString("N2").Replace(".", "").Replace(",", ".");
                        else
                            qcValor = toDecimal(row["VALOR_TAB1"]).ToString("N2").Replace(".", "").Replace(",", ".");

                        sql = String.Concat("INSERT INTO tabela_valor_item (tabelavaloritem_tabelaid, tabelavaloritem_planoId, tabelavaloritem_idadeInicio, tabelavaloritem_idadeFim, tabelavaloritem_qComum, tabelavaloritem_qParticular) VALUES (",
                            tabela.ID, ",",
                            planoId, ",",
                            row["IDADE_INI"], ",",
                            row["IDADE_FIN"], ",",
                            "'", qcValor, "',",
                            "'", qpValor, "')");

                        NonQueryHelper.Instance.ExecuteNonQuery(sql, pm);
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
                dsOrigem.Dispose();
            }
        }

        public void ImportarAtendimentos()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM ocorrencias where id > 108000", conn);
                adp.Fill(dsOrigem, "ocorrencias");
                adp.Dispose();
            }

            Object propostaId = null;

            StringBuilder sb = new StringBuilder();
            DateTime data = DateTime.MinValue;

            #region

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                int i = 0, j = 0;
                String titulo = "";
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    try
                    {
                        cmd.CommandText = "SELECT contrato_id FROM contrato WHERE contrato_codCobranca=" + toString(row["ID_COBRANCA"]);
                        propostaId = cmd.ExecuteScalar();
                        if (propostaId == null)
                        {
                            continue;
                        }

                        if (toString(row["DESCRIC_OCORR"]).Length > 30)
                            titulo = toString(row["DESCRIC_OCORR"]).Substring(0, 29) + "(...)";
                        else
                            titulo = toString(row["DESCRIC_OCORR"]);

                        titulo = titulo.Replace("'", "´");

                        sb.Append("INSERT INTO _atendimento (atendimento_propostaId, atendimento_titulo,atendimento_texto,atendimento_dataInicio,atendimento_dataPrevisao, atendimento_dataTermino,atendimento_data,atendimento_cadastrado,atendimento_resolvido) VALUES (");
                        sb.Append(propostaId);
                        sb.Append(",'");
                        sb.Append(titulo);
                        sb.Append("','");
                        sb.Append(toString(row["DESCRIC_OCORR"]).Replace("'", "´"));

                        #region data inicio
                        data = toDateTime(row["DATA_OCORR"]);
                        if (data != DateTime.MinValue && data.Year > 1753)
                        {
                            sb.Append("','");
                            sb.Append(data.ToString("yyyy-MM-dd"));
                            sb.Append("',");
                        }
                        else
                        {
                            sb.Append("',NULL,");
                        }
                        #endregion

                        #region data prevista
                        data = toDateTime(row["DATA_PREVISTA"]);
                        if (data != DateTime.MinValue && data.Year > 1753)
                        {
                            sb.Append("'");
                            sb.Append(data.ToString("yyyy-MM-dd 23:59:59"));
                            sb.Append("',");
                        }
                        else
                        {
                            sb.Append("NULL,");
                        }
                        #endregion

                        #region data termino
                        data = toDateTime(row["DATA_CONCLUSAO"]);
                        if (data != DateTime.MinValue && data.Year > 1753)
                        {
                            sb.Append("'");
                            sb.Append(data.ToString("yyyy-MM-dd 23:59:59"));
                            sb.Append("',");
                        }
                        else
                        {
                            sb.Append("NULL,");
                        }
                        #endregion

                        #region data
                        data = toDateTime(row["DATA_OCORR"]);
                        if (data != DateTime.MinValue && data.Year > 1753)
                        {
                            sb.Append("'");
                            sb.Append(data.ToString("yyyy-MM-dd 23:59:59"));
                            sb.Append("',");
                        }
                        else
                        {
                            sb.Append("NULL,");
                        }
                        #endregion

                        sb.Append("'");
                        sb.Append(toString(row["CADASTRADO_POR"]).Replace("'", "´"));
                        sb.Append("','");
                        sb.Append(toString(row["RESOLVIDO_POR"]).Replace("'", "´"));
                        sb.Append("');");
                        sb.Append(Environment.NewLine);

                        i++; j++;

                        if (j == 500)
                        {
                            cmd.CommandText = sb.ToString();
                            cmd.ExecuteNonQuery();
                            sb.Remove(0, sb.Length);
                            j = 0;
                        }

                    }
                    catch
                    {
                        throw;
                    }
                }

                if (sb.Length > 0)
                {
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    sb.Remove(0, sb.Length);
                }
            }
            #endregion

            dsOrigem.Dispose();

            #region
            /*
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                AtendimentoTemp atendimento = null; Contrato contrato = null;
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    contrato = Contrato.CarregarParcialPorCodCobranca(row["ID_COBRANCA"], pm);
                    if (contrato == null) { continue; }

                    atendimento = new AtendimentoTemp();

                    atendimento.Data       = toDateTime(row["DATA_OCORR"]);
                    atendimento.DataInicio = toDateTime(row["DATA_OCORR"]);
                    atendimento.DataFim    = toDateTime(row["DATA_CONCLUSAO"]);
                    atendimento.PropostaID = contrato.ID;
                    atendimento.Texto      = toString(row["DESCRIC_OCORR"]);

                    pm.Save(atendimento);
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
                pm = null;
            }
            */
            #endregion
        }


        //--------------------------------------------------------------------------------//

        //public String ArrumaCODsDeCobrancaParaProposta()
        //{
        //    DataSet dsOrigem = new DataSet();

        //    //using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
        //    //{
        //    //    conn.Open();
        //    //    OleDbDataAdapter adp = new OleDbDataAdapter("SELECT NUM_CONTRATO, CNPJ_OPERADORA, ID_PROPOSTA FROM PROPOSTAS", conn); //AND num_contrato='S0409' 
        //    //    adp.Fill(dsOrigem, "propostas");
        //    //    adp.Dispose();
        //    //}

        //    using (SqlConnection conn = new SqlConnection(sqlConn))
        //    {
        //        conn.Open();
        //        SqlDataAdapter adp = new SqlDataAdapter("select * from contrato where contrato_codcobranca is null and ", conn); //AND num_contrato='S0409' 
        //        adp.Fill(dsOrigem, "propostas");
        //        adp.Dispose();
        //    }

        //    //PersistenceManager pm = new PersistenceManager();
        //    //pm.BeginTransactionContext();

        //    using (SqlConnection conn = new SqlConnection(sqlConn))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = conn.CreateCommand();
        //        cmd.Transaction = conn.BeginTransaction();

        //        try
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            int i = 0; Object operadoraId = null; Object propostaId = null;

        //            foreach (DataRow row in dsOrigem.Tables[0].Rows)
        //            {
        //                cmd.CommandText = String.Concat("SELECT operadora_id FROM operadora WHERE operadora_cnpj='", toString(row["CNPJ_OPERADORA"]), "'");
        //                operadoraId = cmd.ExecuteScalar();
        //                if (operadoraId == null) { continue; }

        //                cmd.CommandText = String.Concat("SELECT contrato_id FROM contrato WHERE contrato_numero='", toString(row["NUM_CONTRATO"]), "' AND contrato_operadoraId=", operadoraId);
        //                propostaId = cmd.ExecuteScalar();
        //                if (propostaId == null) { continue; }

        //                sb.Append("\nUPDATE contrato SET contrato_codcobranca=");
        //                sb.Append(toString(row["ID_PROPOSTA"]));
        //                sb.Append(" WHERE contrato_id=");
        //                sb.Append(propostaId);

        //                i++;
        //            }

        //            cmd.CommandText = sb.ToString();
        //            cmd.ExecuteNonQuery();
        //            cmd.Transaction.Commit();
        //            return sb.ToString();
        //        }
        //        catch
        //        {
        //            cmd.Transaction.Rollback();
        //            throw;
        //        }
        //        finally
        //        {
        //        }
        //    }
        //}

        public String ArrumaCODsDeCobrancaParaProposta()
        {
            DataSet dsOrigem = new DataSet();

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter("select contrato_id from contrato where contrato_codcobranca is null or contrato_codcobranca=0", conn); //AND num_contrato='S0409' 
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd  = conn.CreateCommand();
                cmd.Transaction = conn.BeginTransaction();
                cmd.CommandText = "select max(contrato_codcobranca) from contrato";

                int i = Convert.ToInt32(cmd.ExecuteScalar());

                try
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (DataRow row in dsOrigem.Tables[0].Rows)
                    {
                        sb.Append("\nUPDATE contrato SET contrato_codcobranca=");
                        sb.Append(i);
                        sb.Append(" WHERE contrato_id=");
                        sb.Append(row["contrato_id"]);

                        i++;
                    }

                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    return sb.ToString();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
                finally
                {
                }
            }
        }

        public void ArrumaDocumentos()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM DEPARA", conn); //AND num_contrato='S0409' 
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            String qry = "SELECT usuario_id FROM usuario where usuario_documento1='";

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            Object ret = null;

            StringBuilder sb = new StringBuilder();

            try
            {
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    if (toString(row["de"]) == toString(row["para"])) { continue; }
                    if (sb.Length > 0) { sb.Append(" ; "); }

                    ret = LocatorHelper.Instance.ExecuteScalar(qry + toString(row["para"]) + "'", null, null, pm);

                    if (ret != null)
                    {
                        //o cpf PARA já existe no banco
                        //pm.Rollback();
                        //return;
                        continue;
                    }

                    sb.Append("UPDATE usuario SET usuario_documento1='");
                    sb.Append(toString(row["para"]));
                    sb.Append("' WHERE usuario_documento1='");
                    sb.Append(toString(row["de"]));
                    sb.Append("'");
                }

                //NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);

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
                pm = null;
            }
        }

        public void ArrumaStatusDePropostas2(ref List<ErroSumario> erros)
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT num_contrato,cnpj_operadora,status,DATA_CANCEL,obs FROM PROPOSTAS WHERE num_contrato IN ('2066669','2073536','26756','7030563','E702173','E706977','E712459') ORDER BY NUM_CONTRATO", conn); //AND num_contrato='S0409' 
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
                conn.Close();
            }

            Object operadoraId = null;
            erros = new List<ErroSumario>();

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                int i = 0;
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    try
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "SELECT operadora_id FROM operadora WHERE operadora_cnpj='" + toString(row["cnpj_operadora"]) + "'";
                        operadoraId = cmd.ExecuteScalar();

                        if (operadoraId == null)
                        {
                            ErroSumario erro = new ErroSumario();
                            erro.MSG = "Operadora nao localizada: " + toString(row["cnpj_operadora"]);
                            erros.Add(erro);
                            continue;
                        }

                        if (toString(row["status"]) == "1")
                        {
                            cmd.CommandText = "UPDATE contrato SET contrato_inativo=0, contrato_cancelado=0, contrato_dataCancelamento=null, contrato_obs='" + toString(row["obs"]) + "' WHERE contrato_numero='" + toString(row["num_contrato"]) + "' AND contrato_operadoraId=" + operadoraId; //, new String[] { "@contrato_obs" }, new String[] { toString(row["obs"]) }, pm);
                            cmd.ExecuteNonQuery();
                        }
                        else if (toString(row["status"]) == "2") //inativo
                        {
                            cmd.CommandText = String.Concat("UPDATE contrato SET contrato_inativo=1, contrato_cancelado=0, contrato_dataCancelamento='", toDateTime(row["DATA_CANCEL"]).ToString("yyyy-MM-dd"), "', contrato_obs='", toString(row["obs"]), "' WHERE contrato_numero='", toString(row["num_contrato"]), "' AND contrato_operadoraId=", operadoraId); //, new String[] { "@contrato_obs" }, new String[] { toString(row["obs"]) }, pm);
                            cmd.ExecuteNonQuery();
                        }
                        else //cancelado
                        {
                            cmd.CommandText = String.Concat("UPDATE contrato SET contrato_inativo=0, contrato_cancelado=1, contrato_dataCancelamento='", toDateTime(row["DATA_CANCEL"]).ToString("yyyy-MM-dd"), "', contrato_obs='", toString(row["obs"]), "' WHERE contrato_numero='", toString(row["num_contrato"]), "' AND contrato_operadoraId=", operadoraId); //, new String[] { "@contrato_obs" }, new String[] { toString(row["obs"]) }, pm);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    catch (Exception ex)
                    {
                        ErroSumario erro = new ErroSumario();
                        erro.MSG = ex.Message + " | " + toString(row["num_contrato"]);
                        erros.Add(erro);

                        continue;
                    }

                    i++;
                }
            }
        }

        public void ArrumaStatusDePropostas(ref List<ErroSumario> erros)
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT num_contrato,cnpj_operadora,status,DATA_CANCEL,obs FROM PROPOSTAS ORDER BY NUM_CONTRATO", conn); //AND num_contrato='S0409' 
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
                conn.Close();
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();
            Object operadoraId = null;

            erros = new List<ErroSumario>();

            foreach (DataRow row in dsOrigem.Tables[0].Rows)
            {
                try
                {
                    operadoraId = Operadora.CarregarIDPorCNPJ(toString(row["cnpj_operadora"]), pm);

                    if (operadoraId == null)
                    {
                        ErroSumario erro = new ErroSumario();
                        erro.MSG = "Operadora nao localizada: " + toString(row["cnpj_operadora"]);
                        erros.Add(erro);
                        continue;
                    }

                    if (toString(row["status"]) == "1")
                    {
                        NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_inativo=0, contrato_cancelado=0, contrato_dataCancelamento=null, contrato_obs=@contrato_obs WHERE contrato_numero='" + toString(row["num_contrato"]) + "' AND contrato_operadoraId=" + operadoraId, new String[] { "@contrato_obs" }, new String[] { toString(row["obs"]) }, pm);
                    }
                    else if (toString(row["status"]) == "2") //inativo
                    {
                        NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_inativo=1, contrato_cancelado=0, contrato_dataCancelamento='" + toDateTime(row["DATA_CANCEL"]).ToString("yyyy-MM-dd") + "', contrato_obs=@contrato_obs WHERE contrato_numero='" + toString(row["num_contrato"]) + "' AND contrato_operadoraId=" + operadoraId, new String[] { "@contrato_obs" }, new String[] { toString(row["obs"]) }, pm);
                    }
                    else //cancelado
                    {
                        NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_inativo=0, contrato_cancelado=1, contrato_dataCancelamento='" + toDateTime(row["DATA_CANCEL"]).ToString("yyyy-MM-dd") + "', contrato_obs=@contrato_obs WHERE contrato_numero='" + toString(row["num_contrato"]) + "' AND contrato_operadoraId=" + operadoraId, new String[] { "@contrato_obs" }, new String[] { toString(row["obs"]) }, pm);
                    }
                }

                catch (Exception ex)
                {
                    ErroSumario erro = new ErroSumario();
                    erro.MSG = ex.Message + " | " + toString(row["num_contrato"]);
                    erros.Add(erro);

                    continue;
                }
            }

            pm.Commit();
        }

        public String ArrumaCPFs()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM correcao_cpf", conn);
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                int j = 0;

                try
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (DataRow row in dsOrigem.Tables[0].Rows)
                    {
                        if (sb.Length > 0) { sb.Append(";"); sb.Append(Environment.NewLine); }

                        sb.Append("UPDATE beneficiario SET beneficiario_cpf='");
                        sb.Append(row["CPF"]);
                        sb.Append("' WHERE importId=");
                        sb.Append(row["ID"]);

                        j++;

                        if (j == 200)
                        {
                            cmd.CommandText = sb.ToString();
                            cmd.ExecuteNonQuery();
                            sb.Remove(0, sb.Length);
                            j = 0;
                        }
                    }

                    if (sb.Length > 0)
                    {
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }

                    return sb.ToString();
                }
                catch
                {
                    throw;
                }
                finally
                {
                }
            }
        }

        public String ChecaCPFs()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM correcao_cpf", conn);
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                StringBuilder sb = new StringBuilder();
                object ret = null;

                try
                {
                    foreach (DataRow row in dsOrigem.Tables[0].Rows)
                    {
                        cmd.CommandText = String.Concat("SELECT beneficiario_id from beneficiario where importId=",
                            row["ID"]);

                        ret = cmd.ExecuteScalar();

                        if (ret == null || ret == DBNull.Value)
                        {
                            sb.Append("ID: ");
                            sb.Append(row["ID"]);
                            sb.Append(" | CPF: ");
                            sb.Append(row["CPF"]);
                            sb.Append(Environment.NewLine);
                        }
                    }

                    return sb.ToString();
                }
                catch
                {
                    throw;
                }
                finally
                {
                }
            }
        }

        public void ArrumaCobrancaDeTaxasAssociativasEmPropostas()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM DESCONTO", conn);
                adp.Fill(dsOrigem, "DESCONTO");
                adp.Dispose();
            }

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                Object propostaId = null;
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    cmd.CommandText = "SELECT contrato_id FROM contrato WHERE contrato_codcobranca=" + row["ID_COBRANCA"];
                    propostaId = cmd.ExecuteScalar();

                    if (propostaId == null) { throw new ApplicationException("Proposta não localizada."); }

                    cmd.CommandText = "UPDATE contrato SET contrato_cobrartaxaassociativa=1 WHERE contrato_id=" + propostaId;
                    cmd.ExecuteNonQuery();
                }
            }

            dsOrigem.Dispose();
        }

        //--------------------------------------------------------------------------------//

        public void DuplicaLayoutsCustomizados()
        {
            LayoutArquivoCustomizado lac = null;
            IList<ItemLayoutArquivoCustomizado> itens = null;
            IList<Operadora> operadoras = Operadora.CarregarTodas(true);

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            foreach (Operadora operadora in operadoras)
            {
                lac = new LayoutArquivoCustomizado(1);
                pm.Load(lac);
                itens = ItemLayoutArquivoCustomizado.Carregar(lac.ID, pm);

                lac.ID   = null;
                lac.Tipo = (int)LayoutArquivoCustomizado.eTipoTransacao.SINCRONIZACAO_SEG;
                lac.OperadoraID = operadora.ID;
                pm.Save(lac);

                foreach (ItemLayoutArquivoCustomizado _item in itens)
                {
                    _item.ID = null;
                    _item.LayoutID = lac.ID;
                    pm.Save(_item);
                }
            }

            pm.Commit();
        }

        public void ArrumaBeneficiariosDuplicados()
        {
            String qry = "select beneficiario_cpf, COUNT(beneficiario_cpf) as qtd from beneficiario group by beneficiario_cpf, beneficiario_cpf having COUNT (beneficiario_cpf) > 1";
            DataSet ds = new DataSet();
            /*

            select beneficiario_nome, beneficiario_cpf, COUNT(beneficiario_cpf) from 
            beneficiario 
            where beneficiario_cpf <> '99999999999'
            group by beneficiario_nome, beneficiario_cpf, beneficiario_cpf 
            having COUNT (beneficiario_cpf) > 1 

            */
            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter(qry, conn);
                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count == 0) { return; }

                String nome = "";
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    //
                }
            }
        }

        public void DuplicaDependentes()
        {
            String operadoraId   = "16";
            String estipulanteId = "9";
            String contratoAdmId = "53";

            String qry = "SELECT contratoadm_id FROM contratoadm WHERE contratoadm_operadoraid=" + operadoraId + " AND contratoadm_id <> " + contratoAdmId;

            DataTable dtContratos = LocatorHelper.Instance.ExecuteQuery(qry, "contratosadm").Tables[0];

            qry = "SELECT * FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=";
            DataTable dtDependentes = null;
            DataRow[] result = null;
            foreach (DataRow contrato in dtContratos.Rows)
            {
                dtDependentes = LocatorHelper.Instance.ExecuteQuery(qry + contrato["contratoadm_id"], "depend").Tables[0];
            }
        }

        public String SetaReativacoes()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM REATIVACOES", conn);
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            String qry = "SELECT contrato_id, contrato_obs FROM contrato where (contrato_inativo <> 0 OR contrato_cancelado <> 0) AND contrato_numero='";

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = conn.BeginTransaction();
                String obs = "";

                try
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (DataRow row in dsOrigem.Tables[0].Rows)
                    {
                        cmd.CommandText = String.Concat(qry, toString(row["PROPOSTA"]), "'");

                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (!dr.Read())
                            {
                                //nao achou a proposta!
                                dr.Close(); dr.Dispose();
                                continue;
                            }

                            if (sb.Length > 0) { sb.Append(";"); sb.Append(Environment.NewLine); }

                            obs = String.Concat(toString(dr["contrato_obs"]),Environment.NewLine, toString(row["OBS"]));

                            sb.Append("UPDATE contrato SET contrato_cancelado=0, contrato_inativo=0, contrato_dataCancelamento=null, contrato_obs='");
                            sb.Append(obs.Replace("'", "´"));
                            sb.Append("' WHERE contrato_id=");
                            sb.Append(dr["contrato_id"]);

                            dr.Close(); dr.Dispose();
                        }
                    }

                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    return sb.ToString();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
                finally
                {
                }
            }
        }

        public String SetaInativacoes()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM INATIVACOES", conn);
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            String qry = "SELECT contrato_id, contrato_obs FROM contrato where (contrato_inativo <> 1 AND contrato_cancelado <> 1) AND contrato_numero='";

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = conn.BeginTransaction();
                String obs = "";
                DateTime data;

                try
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (DataRow row in dsOrigem.Tables[0].Rows)
                    {
                        cmd.CommandText = String.Concat(qry, toString(row["PROPOSTA"]), "'");

                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (!dr.Read())
                            {
                                //nao achou a proposta!
                                dr.Close(); dr.Dispose();
                                continue;
                            }

                            if (sb.Length > 0) { sb.Append(";"); sb.Append(Environment.NewLine); }

                            obs = String.Concat(toString(dr["contrato_obs"]), Environment.NewLine, toString(row["OBS"]));
                            data = toDateTime(row["DATA_CANCELAMENTO"]);

                            sb.Append("UPDATE contrato SET contrato_cancelado=0, contrato_inativo=1, contrato_dataCancelamento='");
                            sb.Append(data.ToString("yyyy-MM-dd"));
                            sb.Append("', contrato_obs='");
                            sb.Append(obs.Replace("'", "´"));
                            sb.Append("' WHERE contrato_id=");
                            sb.Append(dr["contrato_id"]);

                            dr.Close(); dr.Dispose();
                        }
                    }

                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    return sb.ToString();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
                finally
                {
                }
            }
        }

        public void GeraParentescos()
        {
            int operadoraId = 17;
            Object count;

            String cmd = null;

            //IList<Estipulante> estipulantes = Estipulante.Carregar(false);
            //foreach (Estipulante estipulante in estipulantes)
            //{
                IList<ContratoADM> contratos = ContratoADM.Carregar(operadoraId, true);
                foreach (ContratoADM contrato in contratos)
                {
                    count = LocatorHelper.Instance.ExecuteScalar("SELECT COUNT(*) FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=" + contrato.ID + " AND contratoAdmparentescoagregado_parentescoDescricao LIKE 'ESPOS%'", null, null);
                    if (count == null || count == DBNull.Value || Convert.ToInt32(count) == 0)
                    {
                        cmd = String.Concat(
                            "INSERT INTO contratoAdm_parentesco_agregado (contratoAdmparentescoagregado_contratoAdmId,contratoAdmparentescoagregado_parentescoDescricao,contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo) VALUES (", contrato.ID, ", 'ESPOSO(A)',2,1)");
                        NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
                    }

                    count = LocatorHelper.Instance.ExecuteScalar("SELECT COUNT(*) FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=" + contrato.ID + " AND contratoAdmparentescoagregado_parentescoDescricao LIKE 'FILH%'", null, null);
                    if (count == null || count == DBNull.Value || Convert.ToInt32(count) == 0)
                    {
                        cmd = String.Concat(
                            "INSERT INTO contratoAdm_parentesco_agregado (contratoAdmparentescoagregado_contratoAdmId,contratoAdmparentescoagregado_parentescoDescricao,contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo) VALUES (", contrato.ID, ", 'FILHO(A)',3,1)");
                        NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
                    }

                    count = LocatorHelper.Instance.ExecuteScalar("SELECT COUNT(*) FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=" + contrato.ID + " AND contratoAdmparentescoagregado_parentescoDescricao LIKE 'IRM%'", null, null);
                    if (count == null || count == DBNull.Value || Convert.ToInt32(count) == 0)
                    {
                        cmd = String.Concat(
                            "INSERT INTO contratoAdm_parentesco_agregado (contratoAdmparentescoagregado_contratoAdmId,contratoAdmparentescoagregado_parentescoDescricao,contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo) VALUES (", contrato.ID, ", 'IRMA(O)',4,1)");
                        NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
                    }

                    count = LocatorHelper.Instance.ExecuteScalar("SELECT COUNT(*) FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=" + contrato.ID + " AND contratoAdmparentescoagregado_parentescoDescricao LIKE 'OUTR%'", null, null);
                    if (count == null || count == DBNull.Value || Convert.ToInt32(count) == 0)
                    {
                        cmd = String.Concat(
                            "INSERT INTO contratoAdm_parentesco_agregado (contratoAdmparentescoagregado_contratoAdmId,contratoAdmparentescoagregado_parentescoDescricao,contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo) VALUES (", contrato.ID, ", 'OUTROS',4,1)");
                        NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
                    }

                    count = LocatorHelper.Instance.ExecuteScalar("SELECT COUNT(*) FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=" + contrato.ID + " AND contratoAdmparentescoagregado_parentescoDescricao LIKE '%PAI%'", null, null);
                    if (count == null || count == DBNull.Value || Convert.ToInt32(count) == 0)
                    {
                        cmd = String.Concat(
                            "INSERT INTO contratoAdm_parentesco_agregado (contratoAdmparentescoagregado_contratoAdmId,contratoAdmparentescoagregado_parentescoDescricao,contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo) VALUES (", contrato.ID, ", 'PAI / MAE',1,1)");
                        NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
                    }
                }
            //}
        }


        public void Arruma1Cobranca()
        {
            String qry = "select contrato_id, contrato_numero, contrato_data, contrato_admissao, cobranca_id from contrato left join cobranca on contrato_id=cobranca_propostaId where cobranca_id is null ";

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];

            Cobranca cobranca = null;

            PersistenceManager pm = null;
            pm = new PersistenceManager();
            pm.BeginTransactionContext();
            List<CobrancaComposite> composite = null;

            int i = 0;
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (i == 500) { break; }
                    cobranca                = new Cobranca();
                    cobranca.Cancelada      = false;
                    cobranca.ComissaoPaga   = true;
                    cobranca.DataCriacao    = toDateTime(row["contrato_data"]);
                    cobranca.DataPgto       = toDateTime(row["contrato_admissao"]);
                    cobranca.DataVencimento = cobranca.DataPgto;
                    cobranca.Pago           = true;
                    cobranca.Parcela        = 1;
                    cobranca.PropostaID     = row["contrato_id"];
                    cobranca.Tipo           = (int)Cobranca.eTipo.Normal;
                    cobranca.Valor          = Contrato.CalculaValorDaProposta2(row["contrato_id"], cobranca.DataPgto, pm, true, true, ref composite, false);

                    cobranca.ValorNominal = cobranca.Valor;
                    cobranca.ValorPgto    = cobranca.Valor;

                    pm.Save(cobranca);
                    i++;
                }

                pm.Commit();

            }
            catch(Exception ex)
            {
                //if(pm != null)
                //pm.Rollback();
                throw;
            }
            finally
            {
                dt.Dispose();
                //if (pm != null)
                //pm.Dispose();
            }
        }

        public void SetaCobrancasPAGAS()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM baixa", conn);
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                Object propostaId = null;
                Object cobrancaId = null;
                DateTime data = DateTime.MinValue;

                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    cmd.CommandText = "SELECT contrato_id FROM contrato WHERE contrato_codCobranca=" + row["id"];
                    propostaId = cmd.ExecuteScalar();

                    cmd.CommandText = "SELECT cobranca_id FROM cobranca WHERE cobranca_parcela=" + row["parcela"] + " AND cobranca_propostaId=" + propostaId;
                    cobrancaId = cmd.ExecuteScalar();

                    cmd.CommandText = String.Concat("UPDATE cobranca SET cobranca_pago=1, cobranca_valorPagto='",
                    toDecimal(row["Cobrança_valor"]).ToString("N2").Replace(".", "").Replace(",", "."), 
                    "', cobranca_dataPagto=");

                    #region data pagamento
                    data = toDateTime(row["dt_pagto"]);
                    if (data != DateTime.MinValue && data.Year > 1753)
                    {
                        cmd.CommandText += "'";
                        cmd.CommandText += data.ToString("yyyy-MM-dd");
                        cmd.CommandText += "'";
                    }
                    else
                    {
                        cmd.CommandText += "NULL";
                    }
                    #endregion

                     cmd.CommandText += " WHERE cobranca_id=" + cobrancaId;
                     cmd.ExecuteNonQuery();
                }
            }
        }
    }

    //public sealed class ImportKMais
    //{
    //    String toString(Object param)
    //    {
    //        if (param == null || param == DBNull.Value)
    //            return String.Empty;
    //        else
    //            return Convert.ToString(param);
    //    }
    //    Int32 toInt32(Object param)
    //    {
    //        if (param == null || param == DBNull.Value)
    //            return 0;
    //        else
    //            return Convert.ToInt32(param);
    //    }

    //    DateTime toDateTime(Object param)
    //    {
    //        if (param == null || param == DBNull.Value)
    //            return DateTime.MinValue;
    //        else if (Convert.ToString(param).IndexOf('/') == -1)
    //        {
    //            int ano = Convert.ToInt32(Convert.ToString(param).Substring(0, 4));
    //            int mes = Convert.ToInt32(Convert.ToString(param).Substring(4, 2));
    //            int dia = Convert.ToInt32(Convert.ToString(param).Substring(6, 2));

    //            return new DateTime(ano, mes, dia);
    //        }
    //        else
    //            return Convert.ToDateTime(param, new System.Globalization.CultureInfo("pt-Br"));
    //    }
    //    Decimal toDecimal(Object param)
    //    {
    //        if (param == null || param == DBNull.Value)
    //            return Decimal.Zero;
    //        else
    //            return Convert.ToDecimal(param, new System.Globalization.CultureInfo("pt-Br"));
    //    }

    //    public void k_importaPessoasContratos()
    //    {
    //        DateTime agora = DateTime.Now, vencimento = DateTime.MinValue;

    //        DataSet dsSOCIOS = new DataSet();
    //        DataSet dsDEPENDS = new DataSet();
    //        DataSet dsMANUTS = new DataSet();

    //        //using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + k_mdbPath + ";Persist Security Info=False;"))
    //        using (System.Data.SqlClient.SqlConnection conn = new SqlConnection("Server=DEV01;Database=kMais01;Uid=sa;Pwd=lcmaster0000;timeout=1999999999"))
    //        {
    //            conn.Open();

    //            SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM SOCIOS", conn);
    //            adp.Fill(dsSOCIOS, "SOCIOS");

    //            adp.SelectCommand.CommandText = "SELECT * FROM DEPENDS";
    //            adp.Fill(dsDEPENDS, "DEPENDS");

    //            adp.SelectCommand.CommandText = "SELECT * FROM MANUTS";
    //            adp.Fill(dsMANUTS, "MANUTS");

    //            adp.Dispose();
    //            conn.Close();
    //        }

    //        using (PersistenceManager pm = new PersistenceManager())
    //        {
    //            //pm.UseSingleCommandInstance();
    //            pm.BeginTransactionContext();

    //            try
    //            {
    //                object aux = null;
    //                Contrato contrato = null;
    //                Endereco endereco = null;
    //                Cobranca cobranca = null;
    //                Beneficiario titular = null;
    //                Beneficiario dependente = null;
    //                ContratoBeneficiario cbTit = null;
    //                ContratoBeneficiario cbDep = null;

    //                DataRow[] res_depends = null;
    //                DataRow[] res_cobrancas = null;

    //                int cont = 0, contDep = 0;

    //                #region titulares

    //                foreach (DataRow socio in dsSOCIOS.Tables[0].Rows)
    //                {
    //                    if (cont > 300) break;
    //                    cont++;

    //                    titular = new Beneficiario();

    //                    aux = LocatorHelper.Instance.ExecuteScalar(
    //                        string.Concat("select beneficiario_id from beneficiario where beneficiario_matriculaAssociativa='", socio["TIT_"], "'"),
    //                        null, null, pm);

    //                    if (aux == null || aux == DBNull.Value)
    //                    {
    //                        titular.CPF = toString(socio["CPF_"]).Replace(".", "").Replace("-", "").Replace(" ", "");
    //                        titular.DataNascimento = toDateTime(socio["NASC"]);
    //                        titular.MatriculaAssociativa = toString(socio["TIT_"]);
    //                        //titular.ImportID = toInt32(socio["ID_"]);
    //                        titular.Nome = toString(socio["NOME_"]).Trim();
    //                        titular.NomeMae = toString(socio["MAE_"]);
    //                        titular.Telefone = toString(socio["TEL_"]);
    //                        titular.Sexo = "1";

    //                        if (string.IsNullOrEmpty(titular.Nome)) continue;

    //                        pm.Save(titular);
    //                    }
    //                    else
    //                    {
    //                        titular.ID = aux;
    //                        pm.Load(titular);
    //                    }

    //                    #region Endereço

    //                    endereco = null;

    //                    aux = LocatorHelper.Instance.ExecuteScalar(
    //                        string.Concat("select endereco_id from endereco where endereco_donotipo=0 and endereco_donoId=", titular.ID),
    //                        null, null, pm);

    //                    if (aux == null || aux == DBNull.Value)
    //                    {
    //                        endereco = new Endereco();
    //                        endereco.Bairro = toString(socio["BAIR_"]).Trim();
    //                        endereco.CEP = toString(socio["CEP_"]).Replace("-", "").Trim();
    //                        endereco.Cidade = toString(socio["MUNI_"]).Trim();
    //                        endereco.DonoId = titular.ID;
    //                        endereco.DonoTipo = 0;
    //                        endereco.Logradouro = toString(socio["ENDE_"]).Trim();
    //                        endereco.Tipo = 0;
    //                        endereco.UF = toString(socio["ESTA_"]).Trim();

    //                        pm.Save(endereco);
    //                    }
    //                    #endregion

    //                    #region Contrato

    //                    contrato = null;

    //                    if (endereco != null)
    //                    {
    //                        aux = LocatorHelper.Instance.ExecuteScalar(
    //                        string.Concat("select contrato_id from contrato where contrato_numero='", titular.MatriculaAssociativa, "'"),
    //                        null, null, pm);

    //                        if (aux == null || aux == DBNull.Value)
    //                        {
    //                            contrato = new Contrato();
    //                            contrato.Adimplente = true;
    //                            contrato.Admissao = toDateTime(socio["DATIT_"]);
    //                            if (contrato.Admissao == DateTime.MinValue) contrato.Admissao = agora;
    //                            contrato.Cancelado = toString(socio["STATUS_"]) != "A" ? true : false;
    //                            contrato.Inativo = contrato.Cancelado;
    //                            contrato.CobrarTaxaAssociativa = false;
    //                            contrato.CodCobranca = -1;
    //                            contrato.ContratoADMID = 1;
    //                            contrato.Data = agora;
    //                            contrato.Desconto = 0;
    //                            contrato.EnderecoCobrancaID = endereco.ID;
    //                            contrato.EnderecoReferenciaID = endereco.ID;
    //                            contrato.EstipulanteID = 114;
    //                            contrato.FilialID = 2;
    //                            contrato.Legado = true;
    //                            contrato.Numero = titular.MatriculaAssociativa;
    //                            contrato.NumeroMatricula = contrato.NumeroMatricula;
    //                            contrato.OperadoraID = 6;
    //                            contrato.PlanoID = 1; /////////////////?????????
    //                            contrato.Rascunho = false;
    //                            contrato.Status = 0;
    //                            contrato.TipoContratoID = 1;
    //                            contrato.UsuarioID = 1;
    //                            contrato.Vencimento = agora; /////// será arrumado na importacao de cobrancas
    //                            contrato.Vigencia = agora;   ///////???????

    //                            pm.Save(contrato);
    //                        }
    //                        else
    //                        {
    //                            contrato.ID = aux;
    //                            pm.Load(contrato);
    //                        }
    //                    }

    //                    #endregion

    //                    #region ContratoBeneficiario - Titular

    //                    cbTit = null;

    //                    aux = LocatorHelper.Instance.ExecuteScalar(
    //                        string.Concat("select contratobeneficiario_id from contrato_beneficiario where contratobeneficiario_tipo=0 and contratobeneficiario_contratoId=", contrato.ID, " and contratobeneficiario_beneficiarioID=", titular.ID),
    //                        null, null, pm);

    //                    if (aux == null || aux == DBNull.Value)
    //                    {
    //                        cbTit = new ContratoBeneficiario();

    //                        cbTit.Altura = 1.7M;
    //                        cbTit.Ativo = true;
    //                        cbTit.ContratoID = contrato.ID;
    //                        cbTit.Data = contrato.Admissao;
    //                        cbTit.EstadoCivilID = 1; //solteiro
    //                        if (toString(socio["ECIV_"]).ToLower().IndexOf("cas") > -1) cbTit.EstadoCivilID = 2;
    //                        cbTit.NumeroMatriculaSaude = titular.MatriculaAssociativa;
    //                        cbTit.NumeroSequencial = 0;
    //                        cbTit.Peso = 70M;
    //                        cbTit.Status = 0;
    //                        cbTit.Tipo = 0;
    //                        cbTit.Valor = 0M;
    //                        cbTit.Vigencia = contrato.Vigencia;
    //                        //cbTit.CarenciaContratoTempo = 0;

    //                        pm.Save(cbTit);
    //                    }

    //                    #endregion

    //                    #region Dependentes

    //                    res_depends = dsDEPENDS.Tables[0].Select(string.Concat("TIT_='", titular.MatriculaAssociativa, "'"));
    //                    if (res_depends != null && res_depends.Length > 0)
    //                    {
    //                        contDep = 0;

    //                        foreach (DataRow depend in res_depends)
    //                        {
    //                            contDep++;

    //                            aux = LocatorHelper.Instance.ExecuteScalar(
    //                                string.Concat("select beneficiario_id from beneficiario where beneficiario_matriculaAssociativa='", socio["TIT_"], "-", toString(depend["DEP_"]).Replace(" ", ""), "'"),
    //                                null, null, pm);

    //                            dependente = new Beneficiario();

    //                            if (aux == null || aux == DBNull.Value)
    //                            {
    //                                dependente.CPF = "99999999999";
    //                                dependente.DataNascimento = toDateTime(depend["NASC"]);
    //                                dependente.MatriculaAssociativa = string.Concat(socio["TIT_"], "-", toString(depend["DEP_"]).Replace(" ", ""));
    //                                //dependente.ImportID = toInt32(socio["ID_"]);
    //                                dependente.Nome = toString(depend["DEP_"]).Trim();
    //                                dependente.Sexo = "1";

    //                                if (string.IsNullOrEmpty(dependente.Nome)) continue;

    //                                pm.Save(dependente);
    //                            }
    //                            else
    //                            {
    //                                dependente.ID = aux;
    //                                pm.Load(dependente);
    //                            }

    //                            #region ContratoBeneficiario - Titular

    //                            cbTit = null;

    //                            aux = LocatorHelper.Instance.ExecuteScalar(
    //                                string.Concat("select contratobeneficiario_id from contrato_beneficiario where contratobeneficiario_contratoId=", contrato.ID, " and contratobeneficiario_beneficiarioID=", dependente.ID),
    //                                null, null, pm);

    //                            if (aux == null || aux == DBNull.Value)
    //                            {
    //                                cbDep = new ContratoBeneficiario();

    //                                cbDep.Altura = 1.7M;
    //                                cbDep.Ativo = true;
    //                                cbDep.ContratoID = contrato.ID;
    //                                cbDep.Data = contrato.Admissao;
    //                                cbDep.EstadoCivilID = 1; //solteiro
    //                                cbDep.NumeroMatriculaSaude = titular.MatriculaAssociativa;
    //                                cbDep.NumeroSequencial = contDep;
    //                                cbDep.Peso = 70M;
    //                                cbDep.Status = 0;
    //                                cbDep.Tipo = 1;
    //                                cbDep.Valor = 0M;
    //                                cbDep.Vigencia = contrato.Vigencia;
    //                                cbDep.ParentescoID = 15; //outro
    //                                //cbTit.CarenciaContratoTempo = 0;

    //                                pm.Save(cbDep);
    //                            }

    //                            #endregion
    //                        }
    //                    }

    //                    #endregion

    //                    #region Cobrancas

    //                    res_cobrancas = dsMANUTS.Tables[0].Select(string.Concat("TIT_='", titular.MatriculaAssociativa, "'"));
    //                    if (res_cobrancas != null && res_cobrancas.Length > 0)
    //                    {
    //                        foreach (DataRow rcob in res_cobrancas)
    //                        {
    //                            vencimento = toDateTime(rcob["DV_"]);

    //                            aux = LocatorHelper.Instance.ExecuteScalar(
    //                                string.Concat("select cobranca_id from cobranca where cobranca_propostaId=", contrato.ID, " and day(cobranca_datavencimento)=", vencimento.Day, " and month(cobranca_datavencimento)=", vencimento.Month, " and year(cobranca_datavencimento)=", vencimento.Year),
    //                                null, null, pm);

    //                            if (aux == null || aux == DBNull.Value)
    //                            {
    //                                cobranca = new Cobranca();
    //                                cobranca.Cancelada = false;
    //                                cobranca.DataCriacao = agora;
    //                                cobranca.DataPgto = toDateTime(rcob["DP_"]);
    //                                cobranca.DataVencimento = vencimento;
    //                                cobranca.Pago = false;
    //                                cobranca.Parcela = 0;
    //                                cobranca.PropostaID = contrato.ID;
    //                                cobranca.Tipo = 0;
    //                                cobranca.Valor = toDecimal(rcob["VL_"]);
    //                                cobranca.ValorPgto = toDecimal(rcob["VP_"]);
    //                                cobranca.Pago = cobranca.ValorPgto > 0;

    //                                pm.Save(cobranca);
    //                            }
    //                        }
    //                    }

    //                    #endregion
    //                }

    //                #endregion

    //                NonQueryHelper.Instance.ExecuteNonQuery("udpate beneficiario set beneficiario_legado=1", pm);
    //                pm.Commit();
    //            }
    //            catch
    //            {
    //                pm.Rollback();
    //                //pm.CloseSingleCommandInstance();
    //                return;
    //            }
    //            finally
    //            {
    //                pm.Dispose();
    //            }
    //        }
    //    }
    //}
}
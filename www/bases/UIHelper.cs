namespace www
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    using System.Collections;
    using System.Configuration;
    using System.IO.Compression;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using LC.Web.PadraoSeguros.Entity;

    public class UIHelper
    {
        /// <summary>
        /// Seta a visibilidade de um controle segundo as credenciais fornecidas.
        /// </summary>
        public static void AuthCtrl(Control ctrl, String[] allowTo)
        {
            ctrl.Visible = false;
            String currProfile = Usuario.Autenticado.PerfilID;

            if (currProfile == Perfil.AdministradorIDKey)
            {
                ctrl.Visible = true;
            }
            else if (allowTo != null)
            {
                foreach (String allowed in allowTo)
                {
                    if (allowed.Equals(currProfile))
                    {
                        ctrl.Visible = true;
                        break;
                    }
                }
            }
        }

        public static void AuthWebCtrl(WebControl ctrl, String[] allowTo)
        {
            ctrl.Enabled = false;
            String currProfile = Usuario.Autenticado.PerfilID;

            if (currProfile == Perfil.AdministradorIDKey)
            {
                ctrl.Enabled = true;
            }
            else if (allowTo != null)
            {
                foreach (String allowed in allowTo)
                {
                    if (allowed.Equals(currProfile))
                    {
                        ctrl.Enabled = true;
                        break;
                    }
                }
            }
        }

        public static Byte[] Compress(Byte[] data)
        {
            MemoryStream output = new MemoryStream();
            GZipStream gzip = new GZipStream(output,CompressionMode.Compress, true);
            gzip.Write(data, 0, data.Length);
            gzip.Close();
            return output.ToArray();
        }

        public static Byte[] Decompress(Byte[] data)
        {
            MemoryStream input = new MemoryStream();
            input.Write(data, 0, data.Length);
            input.Position = 0;
            GZipStream gzip = new GZipStream(input, CompressionMode.Decompress, true);
            MemoryStream output = new MemoryStream();
            Byte[] buff = new Byte[64];
            Int32 read = -1;

            read = gzip.Read(buff, 0, buff.Length);
            while (read > 0)
            {
                output.Write(buff, 0, read);
                read = gzip.Read(buff, 0, buff.Length);
            }
            gzip.Close();
            return output.ToArray();
        }

        public static Boolean PrimeiraPosicaoELetra(String param)
        {
            if (String.IsNullOrEmpty(param)) { return false; }

            String pos1 = param.Substring(0, 1);

            if (pos1 != "0" && pos1 != "1" && pos1 != "2" && pos1 != "3" && pos1 != "4" &&
                pos1 != "5" && pos1 != "6" && pos1 != "7" && pos1 != "8" && pos1 != "9")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int QtdZerosAEsquerda(String param)
        {
            if (String.IsNullOrEmpty(param)) { return 0; }

            int qtdZeros = 0;

            for(int i = 0; i < param.Length; i++)
            {
                if (param.Substring(i, 1) == "0")
                    qtdZeros++;
                else
                    break;
            }

            return qtdZeros;
        }

        //public static void ExibirTipoDeColunaComissionamento(DropDownList combo, Boolean itemSelecione, Boolean operadora)
        //{
        //    combo.Items.Clear();
        //    if (itemSelecione) { combo.Items.Add(new ListItem("selecione", "-1")); }
        //    combo.Items.Add(new ListItem("NORMAL", "0"));
        //    combo.Items.Add(new ListItem("CARENCIA", "1"));
        //    combo.Items.Add(new ListItem("MIGRACAO", "2"));
        //    if(!operadora)
        //        combo.Items.Add(new ListItem("ADMINISTR.", "3"));

        //    combo.Items.Add(new ListItem("ESPECIAL", "4"));

        //    if (!operadora)
        //        combo.Items.Add(new ListItem("IDADE", "5"));
        //}

        public static Boolean VerificaCnpj(String cnpj)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["naoValidaDocs"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["naoValidaDocs"].ToUpper() == "Y")
            {
                return true;
            }

            if (Regex.IsMatch(cnpj, @"(^(\d{2}.\d{3}.\d{3}/\d{4}-\d{2})|(\d{14})$)"))
            //if (Regex.IsMatch(cnpj, @"d{2}.?d{3}.?d{3}/?d{4}-?d{2}"))
            {
                return validaCnpj(cnpj);
            }
            else
            {
                return false;
            }
        }

        static Boolean validaCnpj(String cnpj)
        {
            Int32[] digitos, soma, resultado;
            Int32 nrDig;
            String ftmt;
            Boolean[] cnpjOk;
            cnpj = cnpj.Replace("/", "");
            cnpj = cnpj.Replace(".", "");
            cnpj = cnpj.Replace("-", "");
            cnpj = cnpj.Replace("_", "");

            if (cnpj == "00000000000000")
            {
                return false;
            }

            ftmt = "6543298765432";
            digitos = new Int32[14];
            soma = new Int32[2];
            soma[0] = 0;
            soma[1] = 0;
            resultado = new Int32[2];
            resultado[0] = 0;
            resultado[1] = 0;
            cnpjOk = new Boolean[2];
            cnpjOk[0] = false;
            cnpjOk[1] = false;

            try
            {
                for (nrDig = 0; nrDig < 14; nrDig++)
                {
                    digitos[nrDig] = int.Parse(cnpj.Substring(nrDig, 1));

                    if (nrDig <= 11)
                    {
                        soma[0] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig + 1, 1)));
                    }

                    if (nrDig <= 12)
                    {
                        soma[1] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig, 1)));
                    }
                }

                for (nrDig = 0; nrDig < 2; nrDig++)
                {
                    resultado[nrDig] = (soma[nrDig] % 11);

                    if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1))
                        cnpjOk[nrDig] = (digitos[12 + nrDig] == 0);
                    else
                        cnpjOk[nrDig] = (digitos[12 + nrDig] == (11 - resultado[nrDig]));
                }

                return (cnpjOk[0] && cnpjOk[1]);

            }
            catch
            {
                return false;
            }
        }

        public static bool ValidaCpf(String vrCPF)
        {
            return Beneficiario.ValidaCpf(vrCPF);
        }

        public static Boolean ValidaUF(String uf)
        {
            return UFs().Contains(uf.ToUpper());
        }

        public static void ExibirUFs(DropDownList combo, Boolean itemSELECIONE)
        {
            List<String> ufs = UFs();

            if (itemSELECIONE)
                combo.Items.Add(new ListItem("Selecione", "-1"));

            foreach (String uf in ufs)
                combo.Items.Add(new ListItem(uf, uf));
        }

        public static List<String> UFs()
        {
            List<String> ufs = new List<String>();

            ufs.Add("AC");
            ufs.Add("AL");
            ufs.Add("AM");
            ufs.Add("AP");
            ufs.Add("BA");
            ufs.Add("CE");
            ufs.Add("DF");
            ufs.Add("ES");
            ufs.Add("GO");
            ufs.Add("MA");
            ufs.Add("MG");
            ufs.Add("MS");
            ufs.Add("MT");
            ufs.Add("PA");
            ufs.Add("PB");
            ufs.Add("PE");
            ufs.Add("PI");
            ufs.Add("PR");
            ufs.Add("RJ");
            ufs.Add("RN");
            ufs.Add("RO");
            ufs.Add("RR");
            ufs.Add("RS");
            ufs.Add("SC");
            ufs.Add("SE");
            ufs.Add("SP");
            ufs.Add("TO");

            return ufs;
        }

        #region metodos para checagem de perfis 

        public static Boolean PerfilADMIN(String perfilId)
        {
            return perfilId == "1";
        }

        public static Boolean PerfilConferencia(String perfilId)
        {
            return perfilId == "9";
        }

        public static Boolean PerfilCadastro(String perfilId)
        {
            return perfilId == "10";
        }

        public static Boolean PerfilMedico(String perfilId)
        {
            return perfilId == "11";
        }

        public static Boolean PerfilDeptoTecnico(String perfilId)
        {
            return perfilId == "12";
        }

        #endregion



        public static String ChecaGridIntervalosInterios(GridView grid, Int32 indiceColunaIdade1, Int32 indiceColunaIdade2)
        {
            foreach (GridViewRow row in grid.Rows)
            {
                if (!ChecaGridIntervalosInterios(grid.Rows, row, indiceColunaIdade1, indiceColunaIdade2))
                    return "Há uma ou mais inconsistências nos intervalos informados.\\nPor favor verifique.";
            }

            return String.Empty;
        }

        private static Boolean ChecaGridIntervalosInterios(GridViewRowCollection rows, GridViewRow row, Int32 indiceColunaIdade1, Int32 indiceColunaIdade2)
        {
            Int32 valInicio, valFim, valInicioCorrente, valFimCorrente;

            for (int i = 0; i < rows.Count; i++)
            {
                valInicioCorrente = CToInt(((TextBox)row.Cells[indiceColunaIdade1].Controls[1]).Text);
                valFimCorrente = CToInt(((TextBox)row.Cells[indiceColunaIdade2].Controls[1]).Text);

                if (valInicioCorrente > valFimCorrente) { return false; }

                if (i == row.RowIndex) { continue; }

                if (valFimCorrente < valInicioCorrente) { return false; }

                valInicio = CToInt(((TextBox)rows[i].Cells[indiceColunaIdade1].Controls[1]).Text);
                valFim = CToInt(((TextBox)rows[i].Cells[indiceColunaIdade2].Controls[1]).Text);

                if (valFimCorrente == valFim) { return false; }
                if (valInicioCorrente == valInicio) { return false; }

                if (valInicioCorrente > valInicio && valInicioCorrente <= valFim)
                    return false;

                if (valFimCorrente >= valInicio && valFimCorrente < valFim)
                    return false;
            }

            return true;
        }

        public static String ChecaGridIntervalosMonetarios(GridView grid, Int32 indiceColunaIdade1, Int32 indiceColunaIdade2)
        {
            foreach (GridViewRow row in grid.Rows)
            {
                if (!ChecaGridIntervalosMonetarios(grid.Rows, row, indiceColunaIdade1, indiceColunaIdade2))
                    return "Há uma ou mais inconsistências nos intervalos informados.\\nPor favor verifique.";
            }

            return String.Empty;
        }

        static Boolean ChecaGridIntervalosMonetarios(GridViewRowCollection rows, GridViewRow row, Int32 indiceColunaIdade1, Int32 indiceColunaIdade2)
        {
            Decimal valInicio, valFim, valInicioCorrente, valFimCorrente;

            for (int i = 0; i < rows.Count; i++)
            {
                valInicioCorrente = CToDecimal(((TextBox)row.Cells[indiceColunaIdade1].Controls[1]).Text);
                valFimCorrente = CToDecimal(((TextBox)row.Cells[indiceColunaIdade2].Controls[1]).Text);

                if (valInicioCorrente > valFimCorrente) { return false; }

                if (i == row.RowIndex) { continue; }

                if (valFimCorrente < valInicioCorrente) { return false; }

                valInicio = CToDecimal(((TextBox)rows[i].Cells[indiceColunaIdade1].Controls[1]).Text);
                valFim = CToDecimal(((TextBox)rows[i].Cells[indiceColunaIdade2].Controls[1]).Text);

                if (valFimCorrente == valFim) { return false; }
                if (valInicioCorrente == valInicio) { return false; }

                if (valInicioCorrente > valInicio && valInicioCorrente <= valFim)
                    return false;

                if (valFimCorrente >= valInicio && valFimCorrente < valFim)
                    return false;
            }

            return true;
        }


        public static String ChecaGridIdades(GridView grid, Int32 indiceColunaIdade)
        {
            foreach (GridViewRow row in grid.Rows)
            {
                if (!ChecaGridDeParcelas(grid.Rows, row, indiceColunaIdade, row.RowIndex))
                    return "Não pode haver idades duplicadas.";
            }

            return String.Empty;
        }

        /// <summary>
        /// Checa se há inconsistências nos intervalos de idade informados.
        /// </summary>
        public static String ChecaGridIntervaloDeIdades(GridView grid, Int32 indiceColunaIdade1, Int32 indiceColunaIdade2)
        {
            foreach (GridViewRow row in grid.Rows)
            {
                if (!ChecaGridIntervaloDeIdades(grid.Rows, row, indiceColunaIdade1, indiceColunaIdade2))
                    return "Há uma ou mais inconsistências nos intervalos de idade informados.\\nPor favor verifique.";
            }

            return String.Empty;
        }

        /// <summary>
        /// Checa se há inconsistências nos intervalos de idade informados.
        /// </summary>
        static Boolean ChecaGridIntervaloDeIdades(GridViewRowCollection rows, GridViewRow row, Int32 indiceColunaIdade1, Int32 indiceColunaIdade2)
        {
            Int32 idadeInicio, idadeFim, idadeInicioCorrente, idadeFimCorrente;

            for (int i = 0; i < rows.Count; i++)
            {
                idadeInicioCorrente = CToInt(((TextBox)row.Cells[indiceColunaIdade1].Controls[1]).Text);
                idadeFimCorrente = CToInt(((TextBox)row.Cells[indiceColunaIdade2].Controls[1]).Text);

                if (idadeInicioCorrente > idadeFimCorrente) { return false; }

                if (i == row.RowIndex) { continue; }

                if (idadeFimCorrente < idadeInicioCorrente) { return false; }

                idadeInicio = CToInt(((TextBox)rows[i].Cells[indiceColunaIdade1].Controls[1]).Text);
                idadeFim    = CToInt(((TextBox)rows[i].Cells[indiceColunaIdade2].Controls[1]).Text);

                if (idadeFimCorrente == idadeFim)       { return false; }
                if (idadeInicioCorrente == idadeInicio) { return false; }

                if(idadeInicioCorrente > idadeInicio && idadeInicioCorrente <= idadeFim)
                    return false;

                if(idadeFimCorrente >= idadeInicio && idadeFimCorrente < idadeFim)
                    return false;
            }

            return true;
        }

        public static String ChecaGridIntervaloDeIdades(GridView grid, Int32 indiceColunaIdade1, Int32 indiceColunaIdade2, Int32 indiceColunaData)
        {
            if (grid.Rows.Count == 0) { return String.Empty; }
            System.Collections.ArrayList aChecar = new System.Collections.ArrayList();

            foreach (GridViewRow row in grid.Rows)
            {
                if (!aChecar.Contains(Convert.ToString(((TextBox)row.Cells[indiceColunaData].Controls[1]).Text)))
                {
                    aChecar.Add(Convert.ToString(((TextBox)row.Cells[indiceColunaData].Controls[1]).Text));
                }
            }

            foreach (String data in aChecar)
            {
                foreach (GridViewRow row in grid.Rows)
                {
                    if (data != ((TextBox)row.Cells[indiceColunaData].Controls[1]).Text) { continue; }

                    if (!ChecaGridIntervaloDeIdades(grid.Rows, row, indiceColunaIdade1, indiceColunaIdade2, indiceColunaData, data))
                        return "Há uma ou mais inconsistências nos intervalos de idade informados.<br>Por favor verifique.";
                }
            }

            return String.Empty;
        }

        static Boolean ChecaGridIntervaloDeIdades(GridViewRowCollection rows, GridViewRow row, Int32 indiceColunaIdade1, Int32 indiceColunaIdade2, Int32 indiceColunaData, String data)
        {
            Int32 idadeInicio, idadeFim, idadeInicioCorrente, idadeFimCorrente;
            String _data = "";

            for (int i = 0; i < rows.Count; i++)
            {
                idadeInicioCorrente = CToInt(((TextBox)row.Cells[indiceColunaIdade1].Controls[1]).Text);
                idadeFimCorrente = CToInt(((TextBox)row.Cells[indiceColunaIdade2].Controls[1]).Text);
                _data = ((TextBox)rows[i].Cells[indiceColunaData].Controls[1]).Text;

                if (_data != data) { continue; }

                if (idadeInicioCorrente > idadeFimCorrente) { return false; }

                if (i == row.RowIndex) { continue; }

                if (idadeFimCorrente < idadeInicioCorrente) { return false; }

                idadeInicio = CToInt(((TextBox)rows[i].Cells[indiceColunaIdade1].Controls[1]).Text);
                idadeFim = CToInt(((TextBox)rows[i].Cells[indiceColunaIdade2].Controls[1]).Text);

                if (idadeFimCorrente == idadeFim) { return false; }
                if (idadeInicioCorrente == idadeInicio) { return false; }

                if (idadeInicioCorrente > idadeInicio && idadeInicioCorrente <= idadeFim)
                    return false;

                if (idadeFimCorrente >= idadeInicio && idadeFimCorrente < idadeFim)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checa se há parcelas duplicadas.
        /// </summary>
        public static String ChecaGridDeParcelas(GridView grid, Int32 indiceColunaParcela)
        {
            foreach (GridViewRow row in grid.Rows)
            {
                if (CToInt(((TextBox)row.Cells[indiceColunaParcela].Controls[1]).Text) <= 0)
                    return "O número da percela deve ser maior ou igual a 1 (um).";

                if (!ChecaGridDeParcelas(grid.Rows, row, indiceColunaParcela, row.RowIndex))
                    return "Não pode haver parcelas duplicadas.";
            }

            return String.Empty;
        }

        /// <summary>
        /// Checa se há parcelas duplicadas.
        /// </summary>
        static Boolean ChecaGridDeParcelas(GridViewRowCollection rows, GridViewRow row, Int32 indiceColunaParcela, Int32 curerntRowIndex)
        {
            for(int i = 0; i < rows.Count; i++)
            {
                if (i == curerntRowIndex) { continue; }

                if (CToInt(((TextBox)rows[i].Cells[indiceColunaParcela].Controls[1]).Text) ==
                    CToInt(((TextBox)row.Cells[indiceColunaParcela].Controls[1]).Text))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Converte um tipo para Int32.
        /// </summary>
        public static Int32 CToInt(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
                return Convert.ToInt32(param);
        }

        public static String CToString(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return String.Empty;
            else
                return Convert.ToString(param);
        }

        public static Boolean CToBool(Object param)
        {
            if (param == null || param == DBNull.Value)
                return false;
            else
                return Convert.ToBoolean(param);
        }

        public static Decimal CToDecimal(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
                return Convert.ToDecimal(param);
        }

        public static SearchMatchType PegaTipoDeBusca(RadioButton optQualquer, RadioButton optInicio, RadioButton optInteiro)
        {
            if (optQualquer.Checked) { return SearchMatchType.QualquerParteDoCampo; }
            if (optInicio.Checked) { return SearchMatchType.InicioDoCampo; }
            else { return SearchMatchType.CampoInteiro; }
        }

        public static Boolean TyParseToDateTime(String strData, out DateTime data)
        {
            System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("pt-Br");
            return DateTime.TryParse(strData, info, System.Globalization.DateTimeStyles.None, out data);
        }

        /// <summary>
        /// Função para escrever por extenso os valores em Real.
        /// </summary>
        /// <param name="Valor">Valor a ser transformado para extenso.</param>
        /// <returns>Retorna String com o valor por extenso.</returns>
        public static String GetValorExtenso(Double Valor)
        {
            if (Valor > 0)
            {
                ValorExtenso valEx = new ValorExtenso();
                return valEx.Extenso_Valor(Convert.ToDecimal(Valor));
            }
            else
                return String.Empty;
        }

        /// <summary>
        /// Mascara o Telefone com DDD (10 Digitos) ou sem DDD (8 Digitos).
        /// </summary>
        /// <param name="Telefone">Telefone a ser mascarado.</param>
        /// <returns>Retorna String com o telefone mascarado.</returns>
        public static String MascaraTelefone(String Telefone)
        {
            String strTelefoneNovo = null;;

            if (!String.IsNullOrEmpty(Telefone))
            {
                Telefone = Telefone.Replace(" ", String.Empty);

                if (Telefone.Length.Equals(12))
                {
                    strTelefoneNovo = String.Concat(Telefone.Substring(0, 4),
                                                    Telefone.Substring(4, 4), "-", Telefone.Substring(8, 4));
                }
                else if (Telefone.Length.Equals(10))
                {
                    strTelefoneNovo = String.Concat("(", Telefone.Substring(0, 2), ")",
                                                    Telefone.Substring(2, 4), "-", Telefone.Substring(6, 4));
                }
                else if (Telefone.Length.Equals(8))
                {
                    strTelefoneNovo = String.Concat(Telefone.Substring(0, 4), "-", Telefone.Substring(4, 4));
                }
                else
                    strTelefoneNovo = Telefone;
            }
            else
                strTelefoneNovo = Telefone;

            return strTelefoneNovo;
        }

        #region ValorExtenso Class

        public class ValorExtenso
        {
            /// <summary>
            /// Função para escrever por extenso os valores em Real (em C# - suporta até R$ 9.999.999.999,99) 
            /// Rotina Criada para ler um número e transformá-lo em extenso 
            /// Limite máximo de 9 Bilhões (9.999.999.999,99).
            /// Não aceita números negativos.
            /// </summary>
            /// <param name="pdbl_Valor">Valor para converter em extenso. Limite máximo de 9 Bilhões (9.999.999.999,99).</param>
            /// <returns>String do valor por Extenso</returns>
            public string Extenso_Valor(Decimal pdbl_Valor)
            {
                if (pdbl_Valor == 0) { return "zero"; }

                if (pdbl_Valor < 0) { pdbl_Valor = pdbl_Valor * -1; }

                string strValorExtenso = ""; //Variável que irá armazenar o valor por extenso do número informado
                string strNumero = ""; //Irá armazenar o número para exibir por extenso
                string strCentena = "";
                string strDezena = "";
                string strDezCentavo = "";

                decimal dblCentavos = 0;
                decimal dblValorInteiro = 0;
                int intContador = 0;
                bool bln_Bilhao = false;
                bool bln_Milhao = false;
                bool bln_Mil = false;
                bool bln_Unidade = false;

                //Verificar se foi informado um dado indevido
                if (pdbl_Valor <= 0)
                {
                    throw new Exception("Valor não suportado pela Função. Verificar se há valor negativo ou nada foi informado");
                }
                if (pdbl_Valor > (decimal)9999999999.99)
                {
                    throw new Exception("Valor não suportado pela Função. Verificar se o Valor está acima de 9999999999.99");
                }
                else //Entrada padrão do método
                {
                    //Gerar Extenso Centavos
                    pdbl_Valor = (Decimal.Round(pdbl_Valor, 2));
                    dblCentavos = pdbl_Valor - (Int64)pdbl_Valor;

                    //Gerar Extenso parte Inteira
                    dblValorInteiro = (Int64)pdbl_Valor;
                    if (dblValorInteiro > 0)
                    {
                        if (dblValorInteiro > 999)
                        {
                            bln_Mil = true;
                        }
                        if (dblValorInteiro > 999999)
                        {
                            bln_Milhao = true;
                            bln_Mil = false;
                        }
                        if (dblValorInteiro > 999999999)
                        {
                            bln_Mil = false;
                            bln_Milhao = false;
                            bln_Bilhao = true;
                        }

                        for (int i = (dblValorInteiro.ToString().Trim().Length) - 1; i >= 0; i--)
                        {
                            // strNumero = Mid(dblValorInteiro.ToString().Trim(), (dblValorInteiro.ToString().Trim().Length - i) + 1, 1);
                            strNumero = Mid(dblValorInteiro.ToString().Trim(), (dblValorInteiro.ToString().Trim().Length - i) - 1, 1);
                            switch (i)
                            { /*******/
                                case 9: /*Bilhão*
                                    /*******/
                                    {
                                        strValorExtenso = fcn_Numero_Unidade(strNumero) + ((int.Parse(strNumero) > 1) ? " Bilhões e" : " Bilhão e");
                                        bln_Bilhao = true;
                                        break;
                                    }
                                case 8: /********/
                                case 5: //Centena*
                                case 2: /********/
                                    {
                                        if (int.Parse(strNumero) > 0)
                                        {
                                            strCentena = Mid(dblValorInteiro.ToString().Trim(), (dblValorInteiro.ToString().Trim().Length - i) - 1, 3);

                                            if (int.Parse(strCentena) > 100 && int.Parse(strCentena) < 200)
                                            {
                                                strValorExtenso = strValorExtenso + " Cento e ";
                                            }
                                            else
                                            {
                                                strValorExtenso = strValorExtenso + " " + fcn_Numero_Centena(strNumero);
                                            }
                                            if (intContador == 8)
                                            {
                                                bln_Milhao = true;
                                            }
                                            else if (intContador == 5)
                                            {
                                                bln_Mil = true;
                                            }
                                        }
                                        break;
                                    }
                                case 7: /*****************/
                                case 4: //Dezena de Milhão*
                                case 1: /*****************/
                                    {
                                        if (int.Parse(strNumero) > 0)
                                        {
                                            strDezena = Mid(dblValorInteiro.ToString().Trim(), (dblValorInteiro.ToString().Trim().Length - i) - 1, 2);//

                                            if (int.Parse(strDezena) > 10 && int.Parse(strDezena) < 20)
                                            {
                                                strValorExtenso = strValorExtenso + (Right(strValorExtenso, 5).Trim() == "entos" ? " e " : " ")
                                                + fcn_Numero_Dezena0(Right(strDezena, 1));//corrigido

                                                bln_Unidade = true;
                                            }
                                            else
                                            {
                                                strValorExtenso = strValorExtenso + (Right(strValorExtenso, 5).Trim() == "entos" ? " e " : " ")
                                                + fcn_Numero_Dezena1(Left(strDezena, 1));//corrigido

                                                bln_Unidade = false;
                                            }
                                            if (intContador == 7)
                                            {
                                                bln_Milhao = true;
                                            }
                                            else if (intContador == 4)
                                            {
                                                bln_Mil = true;
                                            }
                                        }
                                        break;
                                    }
                                case 6: /******************/
                                case 3: //Unidade de Milhão*
                                case 0: /******************/
                                    {
                                        if (int.Parse(strNumero) > 0 && !bln_Unidade)
                                        {
                                            if ((Right(strValorExtenso, 5).Trim()) == "entos"
                                            || (Right(strValorExtenso, 3).Trim()) == "nte"
                                            || (Right(strValorExtenso, 3).Trim()) == "nta")
                                            {
                                                strValorExtenso = strValorExtenso + " e ";
                                            }
                                            else
                                            {
                                                strValorExtenso = strValorExtenso + " ";
                                            }
                                            strValorExtenso = strValorExtenso + fcn_Numero_Unidade(strNumero);
                                        }
                                        if (i == 6)
                                        {
                                            if (bln_Milhao || int.Parse(strNumero) > 0)
                                            {
                                                strValorExtenso = strValorExtenso + ((int.Parse(strNumero) == 1) && !bln_Unidade ? " Milhão" : " Milhões");
                                                strValorExtenso = strValorExtenso + ((int.Parse(strNumero) > 1000000) ? " " : " e");
                                                bln_Milhao = true;
                                            }
                                        }
                                        if (i == 3)
                                        {
                                            if (bln_Mil || int.Parse(strNumero) > 0)
                                            {
                                                strValorExtenso = strValorExtenso + " Mil";
                                                strValorExtenso = strValorExtenso + ((int.Parse(strNumero) > 1000) ? " " : " e");
                                                bln_Mil = true;
                                            }
                                        }
                                        if (i == 0)
                                        {
                                            if ((bln_Bilhao && !bln_Milhao && !bln_Mil
                                            && Right((dblValorInteiro.ToString().Trim()), 3) == "0")
                                            || (!bln_Bilhao && bln_Milhao && !bln_Mil
                                            && Right((dblValorInteiro.ToString().Trim()), 3) == "0"))
                                            {
                                                strValorExtenso = strValorExtenso + " e ";
                                            }
                                            strValorExtenso = strValorExtenso + ((Int64.Parse(dblValorInteiro.ToString())) > 1 ? " Reais" : " Real");
                                        }
                                        bln_Unidade = false;
                                        break;
                                    }
                            }
                        }//
                    }
                    if (dblCentavos > 0)
                    {

                        if (dblCentavos > 0 && dblCentavos < 0.1M)
                        {
                            strNumero = Right((Decimal.Round(dblCentavos, 2)).ToString().Trim(), 1);
                            strValorExtenso = strValorExtenso + ((dblCentavos > 0) ? " e " : " ")
                            + fcn_Numero_Unidade(strNumero) + ((dblCentavos > 0.01M) ? " Centavos" : " Centavo");
                        }
                        else if (dblCentavos > 0.1M && dblCentavos < 0.2M)
                        {
                            strNumero = Right(((Decimal.Round(dblCentavos, 2) - (decimal)0.1).ToString().Trim()), 1);
                            strValorExtenso = strValorExtenso + ((dblCentavos > 0) ? " " : " e ")
                            + fcn_Numero_Dezena0(strNumero) + " Centavos ";
                        }
                        else
                        {
                            strNumero = Right(dblCentavos.ToString().Trim(), 2);
                            strDezCentavo = Mid(dblCentavos.ToString().Trim(), 2, 1);

                            strValorExtenso = strValorExtenso + ((int.Parse(strNumero) > 0) ? " e " : " ");
                            strValorExtenso = strValorExtenso + fcn_Numero_Dezena1(Left(strDezCentavo, 1));

                            if ((dblCentavos.ToString().Trim().Length) > 2)
                            {
                                strNumero = Right((Decimal.Round(dblCentavos, 2)).ToString().Trim(), 1);
                                if (int.Parse(strNumero) > 0)
                                {
                                    if (dblValorInteiro <= 0)
                                    {
                                        if (Mid(strValorExtenso.Trim(), strValorExtenso.Trim().Length - 2, 1) == "e")
                                        {
                                            strValorExtenso = strValorExtenso + " e " + fcn_Numero_Unidade(strNumero);
                                        }
                                        else
                                        {
                                            strValorExtenso = strValorExtenso + " e " + fcn_Numero_Unidade(strNumero);
                                        }
                                    }
                                    else
                                    {
                                        strValorExtenso = strValorExtenso + " e " + fcn_Numero_Unidade(strNumero);
                                    }
                                }
                            }
                            strValorExtenso = strValorExtenso + " Centavos ";
                        }
                    }
                    if (dblValorInteiro < 1) strValorExtenso = Mid(strValorExtenso.Trim(), 2, strValorExtenso.Trim().Length - 2);
                }

                return strValorExtenso.Trim();
            }

            private string fcn_Numero_Dezena0(string pstrDezena0)
            {
                //Vetor que irá conter o número por extenso
                ArrayList array_Dezena0 = new ArrayList();

                array_Dezena0.Add("Onze");
                array_Dezena0.Add("Doze");
                array_Dezena0.Add("Treze");
                array_Dezena0.Add("Quatorze");
                array_Dezena0.Add("Quinze");
                array_Dezena0.Add("Dezesseis");
                array_Dezena0.Add("Dezessete");
                array_Dezena0.Add("Dezoito");
                array_Dezena0.Add("Dezenove");

                return array_Dezena0[((int.Parse(pstrDezena0)) - 1)].ToString();
            }
            private string fcn_Numero_Dezena1(string pstrDezena1)
            {
                //Vetor que irá conter o número por extenso
                ArrayList array_Dezena1 = new ArrayList();

                array_Dezena1.Add("Dez");
                array_Dezena1.Add("Vinte");
                array_Dezena1.Add("Trinta");
                array_Dezena1.Add("Quarenta");
                array_Dezena1.Add("Cinquenta");
                array_Dezena1.Add("Sessenta");
                array_Dezena1.Add("Setenta");
                array_Dezena1.Add("Oitenta");
                array_Dezena1.Add("Noventa");

                return array_Dezena1[Int16.Parse(pstrDezena1) - 1].ToString();
            }

            private string fcn_Numero_Centena(string pstrCentena)
            {
                //Vetor que irá conter o número por extenso
                ArrayList array_Centena = new ArrayList();

                array_Centena.Add("Cem");
                array_Centena.Add("Duzentos");
                array_Centena.Add("Trezentos");
                array_Centena.Add("Quatrocentos");
                array_Centena.Add("Quinhentos");
                array_Centena.Add("Seiscentos");
                array_Centena.Add("Setecentos");
                array_Centena.Add("Oitocentos");
                array_Centena.Add("Novecentos");

                return array_Centena[((int.Parse(pstrCentena)) - 1)].ToString();
            }
            private string fcn_Numero_Unidade(string pstrUnidade)
            {
                //Vetor que irá conter o número por extenso
                ArrayList array_Unidade = new ArrayList();

                array_Unidade.Add("Um");
                array_Unidade.Add("Dois");
                array_Unidade.Add("Três");
                array_Unidade.Add("Quatro");
                array_Unidade.Add("Cinco");
                array_Unidade.Add("Seis");
                array_Unidade.Add("Sete");
                array_Unidade.Add("Oito");
                array_Unidade.Add("Nove");

                return array_Unidade[(int.Parse(pstrUnidade) - 1)].ToString();
            }

            private static string Left(string param, int length)
            {
                //we start at 0 since we want to get the characters starting from the
                //left and with the specified lenght and assign it to a variable
                if (param == "")
                    return "";
                string result = param.Substring(0, length);
                //return the result of the operation
                return result;
            }

            private static string Right(string param, int length)
            {
                //start at the index based on the lenght of the sting minus
                //the specified lenght and assign it a variable
                if (param == "")
                    return "";
                string result = param.Substring(param.Length - length, length);
                //return the result of the operation
                return result;
            }

            private static string Mid(string param, int startIndex, int length)
            {
                //start at the specified index in the string ang get N number of
                //characters depending on the lenght and assign it to a variable
                string result = param.Substring(startIndex, length);
                //return the result of the operation
                return result;
            }

            private static string Mid(string param, int startIndex)
            {
                //start at the specified index and return all characters after it
                //and assign it to a variable
                string result = param.Substring(startIndex);
                //return the result of the operation
                return result;
            }
        }

        #endregion
    }
}

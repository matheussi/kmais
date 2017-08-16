using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LC.Framework.DataUtil;
using LC.Framework.BusinessLayer;
using LC.Framework.Phantom;
using System.IO;

namespace Export
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void cmdCadProcessar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Prosseguir ?", "Prosseguir", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            string qry = "";

            #region query

            qry = string.Concat(
                "SELECT ",
                "   A.contrato_id,A.contrato_numero,A.contrato_estipulanteId,D.contratoadm_contratoSaude AS FATURA,",
                "CASE WHEN A.contrato_dataCancelamento IS NULL	  THEN '01' ",
                "     WHEN A.contrato_dataCancelamento IS NOT NULL THEN '03' ",
		        "     ELSE '01' END										                AS COD_OPERACAO,",
                "'' AS NUMERO_SUBFATURA,C.beneficiario_id as ID_BENEFICIARIO,B.contratobeneficiario_numMatriculaSaude AS NUMERO_CARTAO,",
                "CASE WHEN A.contrato_dataCancelamento IS NULL	   THEN '02' ",
                "     WHEN A.contrato_dataCancelamento IS NOT NULL THEN '32' ",
                "     ELSE '' END											            AS STATUS_PROPOSTA,",
                "a.contrato_id as MATRICULA,'' as NUMERO_UNIDADE, '' as CENTRO_CUSTO,",
                "CASE WHEN LEN(c.beneficiario_cpf) = 0 THEN '00000000000' ",
                "     ELSE c.beneficiario_cpf END                                     AS CPF_TITULAR,",
                "REPLACE(CONVERT(VARCHAR(11),C.beneficiario_dataNascimento,121),'-','')	AS DT_NASC_TITULAR,",
                "CASE WHEN LEN(LTRIM(RTRIM(C.beneficiario_nome))) > 70 THEN SUBSTRING(LTRIM(RTRIM(C.beneficiario_nome)),0,70) ",
                "     ELSE RTRIM(LTRIM(C.beneficiario_nome)) END                        AS NOME_SEGURADO,",
                "REPLACE(CONVERT(VARCHAR(11),A.contrato_admissao,121),'-','')			AS DT_ADMISSAO,",
                "CASE WHEN A.contrato_tipoAcomodacao = 0 THEN E.plano_codigo ",
                "     WHEN A.contrato_tipoAcomodacao = 1 THEN E.plano_codigoparticular ",
                "     ELSE '' END									                AS COD_PLANO,",
                "REPLACE(CONVERT(VARCHAR(11),A.contrato_vigencia,121),'-','')   	AS DT_INICIO_VIGENCIA,",
                "REPLACE(CONVERT(VARCHAR(11),A.contrato_dataCancelamento,121),'-','')	AS DT_FIM_VIGENCIA,",
                "C.beneficiario_sexo								                AS SEXO_SEGURADO,",
                "F.DE_PARA_CODCIVIL									                AS ESTADO_CIVIL,",
                "ISNULL(G.Cod_De_Para_Parentesco,'00')				                AS GRAU_PARENTESCO,",
                "CASE WHEN B.contratobeneficiario_numeroSequencia=0 ",
                "               THEN '1'ELSE '2' END								AS TIPO_SEGURADO,",
                "'(Outros)'											                AS CARGO_OCUPACAO,",
                "RTRIM(LTRIM(C.beneficiario_nomeMae))			                    AS NOME_DA_MAE,",
                "B.contratobeneficiario_numeroSequencia				                AS CODIGO_DEPENDENTE,",
                "REPLACE(CONVERT(VARCHAR(11),C.beneficiario_dataNascimento,121),'-','') AS DT_NASC_TITDEP,",
                "C.beneficiario_cpf									                AS CPF,",
                "'Brasileira'										                AS NACIONALIDADE,",
                "CASE WHEN LEN(RTRIM(LTRIM(H.endereco_logradouro))) > 80 THEN SUBSTRING(RTRIM(LTRIM(H.endereco_logradouro)),0,80) ",
                "     ELSE RTRIM(LTRIM(H.endereco_logradouro))  END                  AS LOGRADOURO_RES,",
                "REPLACE(RTRIM(LTRIM(H.endereco_numero)), 'B','')                    AS NUMERO_RES,",
                "CASE WHEN H.endereco_complemento is null then '                              ' ",
                "     WHEN LEN(RTRIM(LTRIM(H.endereco_complemento))) > 30 then SUBSTRING(RTRIM(LTRIM(H.endereco_complemento)),0,30) ",
                "     ELSE LTRIM(RTRIM(H.endereco_complemento)) END                   AS COMPLEMENTO_RES,",
                "CASE WHEN H.endereco_bairro is null then '                              ' ",
                "     WHEN LEN(H.endereco_bairro) > 30 then SUBSTRING(H.endereco_bairro,0,30) ",
                "     ELSE RTRIM(H.endereco_bairro) END                               AS BAIRRO_RES,",
                "H.endereco_cidade as CIDADE_RES, H.endereco_uf as ESTADO_RES, H.endereco_cep AS CEP_RES,",
                "REPLACE(REPLACE(SUBSTRING(C.beneficiario_telefone,2,2),'(',''),')','')	AS DDD_RES,",
                "SUBSTRING(C.beneficiario_telefone,5,15)				                AS TELEFONE_RES,",
                "REPLACE(REPLACE(SUBSTRING(C.beneficiario_celular,2,2),'(',''),')','') AS DDD_CEL,",
                "REPLACE(SUBSTRING(C.beneficiario_celular,5,15),'-','')               AS TELEFONE_CELULAR,",
                "RTRIM(LTRIM(I.endereco_logradouro))					AS LOGRADOURO_COML",
                ",	REPLACE(RTRIM(LTRIM(I.endereco_numero)), 'B','')	AS NUMERO_COML",
                ",	CASE WHEN I.endereco_complemento is null then '                              ' ",
                "       WHEN LEN(RTRIM(LTRIM(I.endereco_complemento))) > 30 then SUBSTRING(RTRIM(LTRIM(I.endereco_complemento)),0,30) ",
                "       ELSE LTRIM(RTRIM(I.endereco_complemento)) END AS COMPLEMENTO_COML ",
                ",	CASE WHEN I.endereco_bairro is null then '                              ' ",
                "       WHEN LEN(I.endereco_bairro) > 30 then SUBSTRING(I.endereco_bairro,0,30)	",
                "       ELSE RTRIM(I.endereco_bairro) END AS BAIRRO_COML ",
                ",	I.endereco_cidade									AS CIDADE_COML",
                ",	I.endereco_uf										AS ESTADO_COML",
                ",	I.endereco_cep										AS CEP_COML",
                ",	REPLACE(REPLACE(SUBSTRING(C.beneficiario_telefone2,2,2),'(',''),')','')	AS DDD_COML",
                ",	SUBSTRING(C.beneficiario_telefone2,5,15)			AS TELEFONE_COML",
                ",	'' AS DDD_FAX,'' AS FAX",
                ",	CASE WHEN LEN(RTRIM(LTRIM(C.beneficiario_email))) > 30 THEN SUBSTRING(RTRIM(LTRIM(C.beneficiario_email)),0, 30)",
                "        ELSE RTRIM(LTRIM(C.beneficiario_email)) END AS EMAIL",
                ",'' AS BANCO,'' as AGENCIA,'' as PRIMEIRO_DV_DA_AGENCIA,'' as CONTA_CORRENTE,'' as DV_DA_CONTA,'' as NOME_SUBFATURA",
                ",	RTRIM(LTRIM(J.operadora_nome))						AS NOME_DA_UNIDADE",
                ",	REPLACE(CONVERT(VARCHAR(11),A.contrato_dataCancelamento,121),'-','')	AS DATA_CANCELAMENTO",
                ",	RTRIM(LTRIM(C.beneficiario_rg))						AS RG_NUMERO",
                ",	RTRIM(LTRIM(C.beneficiario_rgOrgaoExp))				AS RG_EMISSOR",
                ",	''													AS RG_DATA",
                ",	'02'												AS TIPO_COBRANCA",
                ",	case when D.contratoadm_contratoSaude is null then '          '",
                "       when len(D.contratoadm_contratoSaude) > 30 then substring(D.contratoadm_contratoSaude,0,30) ",
                "       else rtrim(D.contratoadm_contratoSaude) end     AS CODIGO_FATURA ",
                ",	CASE WHEN B.contratobeneficiario_carenciaCodigo is null THEN '00' ",
                "       WHEN B.contratobeneficiario_carenciaCodigo = ''    THEN '00' ",
                "       ELSE													'01' END AS CODIGO_CARENCIA ",
                ",	CASE WHEN DAY(A.contrato_admissao) = 01 THEN 10 ",
                "       ELSE 25 END											AS DIA_VENCIMENTO ",
                ",	CASE WHEN A.contrato_cobrarTaxaAssociativa = 0 THEN 'S' ELSE 'N' END AS ISENCAO_DE_TARIFA ",
                ",	CASE WHEN C.beneficiario_nome LIKE '%(J*)%' THEN 'S' ELSE 'N' END AS ACAO_JUDICIAL",
                ",	case when A.contrato_desconto is null THEN '00000000000' else REPLACE(CONVERT(VARCHAR(100), A.contrato_desconto),'.','') END AS VALOR_DESCONTO",
                ",'' as VALOR_DESCONTO,'' as LOCAL_ATENDIMENTO,'' as DT_INICIO_INTERCAMBIO,'' as DT_FIM_INTERCAMBIO,'' as TIPO_INTERCAMBIO,'' as ID_TIPO_INTERCAMBIO",
                ",	J.operadora_nome									AS OPERADORA",
                ",	N.estipulante_descricao								AS ENTIDADE",
                ",	CASE WHEN A.contrato_tipoAcomodacao = 0 then REPLACE(CONVERT(VARCHAR(100), tabela_valor_item.tabelavaloritem_qComum),'.','')",
                "        WHEN A.contrato_tipoAcomodacao = 1 then REPLACE(CONVERT(VARCHAR(100), tabela_valor_item.tabelavaloritem_qParticular),'.','') ",
                "        ELSE 0 END									    AS VALOR_PLANO",
                ",	'00000000000'										AS VALOR_GRUPO_FAMILIAR ",
                "FROM	contrato A ",
                "       INNER	JOIN contrato_beneficiario				B ON ( A.contrato_id = B.contratobeneficiario_contratoId and b.contratobeneficiario_ativo=1) ",
                "       INNER	JOIN beneficiario						C ON ( B.contratobeneficiario_beneficiarioId = C.beneficiario_id ) ",
                "       INNER	JOIN contratoADM						D WITH ( NOLOCK ) ON ( A.contrato_contratoAdmId = D.contratoadm_id )",
                "       INNER	JOIN plano								E WITH ( NOLOCK ) ON ( A.contrato_planoId = E.plano_id )",
                "       LEFT	JOIN estado_civil_De_Para				F WITH ( NOLOCK ) ON ( B.contratobeneficiario_estadoCivilId = F.estadocivil_id )",
                "       LEFT	JOIN dbo.Grau_Parentesco_De_Para		G WITH ( NOLOCK ) ON ( B.contratobeneficiario_parentescoId = G.contratoAdmparentescoagregado_id ) ",
                "       LEFT	JOIN endereco							H WITH ( NOLOCK ) ON ( A.contrato_enderecoReferenciaId = H.endereco_id ) ",
                "       LEFT	JOIN endereco							I WITH ( NOLOCK ) ON ( A.contrato_enderecoCobrancaId = I.endereco_id ) ",
                "       INNER	JOIN operadora							J WITH ( NOLOCK ) ON ( A.contrato_operadoraId = J.operadora_id ) ",
                "       LEFT	JOIN tabela_valor										  ON tabela_valor.tabelavalor_contratoId = A.contrato_contratoAdmId AND '2014/01' >= CONVERT(varchar(7), tabela_valor.tabelavalor_vencimentoInicio, 111) AND '2014/01' <= CONVERT(varchar(7), tabela_valor.tabelavalor_vencimentoFim, 111)",
                "       LEFT	JOIN tabela_valor_item									  ON tabela_valor_item.tabelavaloritem_tabelaid = tabela_valor.tabelavalor_id AND tabela_valor_item.tabelavaloritem_planoId = A.contrato_planoId AND (DATEDIFF(dd,beneficiario_dataNascimento,GETDATE()) / 365) between tabela_valor_item.tabelavaloritem_idadeInicio  AND tabela_valor_item.tabelavaloritem_idadeFim",
                "       INNER	JOIN estipulante						N WITH ( NOLOCK ) ON ( A.contrato_estipulanteId = N.estipulante_id ) ",
                " WHERE a.contrato_id in (238251,238272,238133,238148) order by a.contrato_id, b.contratobeneficiario_numeroSequencia "); //A.contrato_dataCancelamento IS NULL OR A.contrato_dataCancelamento > = '2013-12-31 23:59:59.998'

            #endregion

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            DataTable dtMain = LocatorHelper.Instance.ExecuteQuery(qry, "result", pm).Tables[0];

            int lnNb = 1;
            StringBuilder sb = new StringBuilder();

            adHeaderCadastro(ref sb, dtMain, lnNb);
            sb.Append(Environment.NewLine);

            foreach (DataRow row in dtMain.Rows)
            {
                lnNb++;
                adLinhaCadastro(ref sb, row, dtMain, lnNb, null);
                sb.Append(Environment.NewLine);

                if (Convert.ToString(row["TIPO_SEGURADO"]) == "1")
                {
                    //titular, checa se tem adicional. para cada um, deve-se adicionar uma linha
                    object aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select adicionalfaixa_valor from adicional_beneficiario inner join adicional on adicional_id=adicionalbeneficiario_adicionalid inner join adicional_faixa on adicional_id=adicionalfaixa_adicionalid and adicionalfaixa_id = (select max(adicionalfaixa_id) from adicional_faixa where adicionalfaixa_adicionalid = adicional_id) where adicionalbeneficiario_propostaId=", row["contrato_id"], " and adicionalbeneficiario_beneficiarioid=", row["ID_BENEFICIARIO"]), null, null, pm);

                    if (aux != null && aux != DBNull.Value)
                    {
                        lnNb++;
                        adLinhaCadastro(ref sb, row, dtMain, lnNb, Convert.ToString(aux));
                        sb.Append(Environment.NewLine);
                    }

                    //titular, checa se tem taxa associativa, caso tenha, deve-se tambem adicionar uma linha
                    if (Convert.ToString(row["ISENCAO_DE_TARIFA"]) == "N")
                    {
                        aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select top 1 estipulantetaxa_valor from estipulante_taxa where estipulantetaxa_estipulanteId=", row["contrato_estipulanteId"], " order by estipulantetaxa_vigencia desc"), null, null, pm);

                        if (aux != null && aux != DBNull.Value)
                        {
                            lnNb++;
                            adLinhaCadastro(ref sb, row, dtMain, lnNb, Convert.ToString(aux));
                            sb.Append(Environment.NewLine);
                        }
                    }

                    aux = null;
                }
            }

            lnNb++;
            adTraillerCadastro(ref sb, dtMain, lnNb);

            dtMain.Dispose();

            string caminhoFull = Path.Combine(txtCaminhoBase.Text,txtNomeArqCadastro.Text);

            if (File.Exists(caminhoFull)) { File.Delete(caminhoFull); }

            File.WriteAllText(caminhoFull, sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

            MessageBox.Show("Arquivo de cadastro concluído:\n\n" + caminhoFull, "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void adLinhaCadastro(ref StringBuilder sb, DataRow row, DataTable dt, int lnNb, string valorAdicional)
        {
            sb.Append(lnNb.ToString().PadLeft(7, '0'));
            sb.Append("D");
            sb.Append("01");
            sb.Append(toString(row["NUMERO_SUBFATURA"], 3).PadLeft(3, '0'));
            sb.Append(toString(row["ID_BENEFICIARIO"], 40).PadLeft(40, '0'));
            sb.Append(toString(row["NUMERO_CARTAO"], 40).PadLeft(40, '0'));
            sb.Append(toString(row["MATRICULA"], 20).PadLeft(20, '0'));
            sb.Append(toString(row["STATUS_PROPOSTA"], 0));
            sb.Append(toString(row["NUMERO_UNIDADE"], 3).PadLeft(3, '0'));
            sb.Append(toString(row["CENTRO_CUSTO"], 40).PadLeft(40, '0'));
            sb.Append(row["CPF_TITULAR"]);

            if (toString(row["DT_NASC_TITULAR"], 0).Length != 8) sb.Append("00000000");
            else sb.Append(toString(row["DT_NASC_TITULAR"], 8).Trim());

            sb.Append(toString(row["NOME_SEGURADO"], 70).Trim().PadRight(70, ' '));

            if (toString(row["DT_ADMISSAO"], 0).Length != 8) sb.Append("00000000");
            else sb.Append(toString(row["DT_ADMISSAO"], 8).Trim());

            sb.Append(toString(row["COD_PLANO"], 10).Trim().PadRight(10, ' '));

            if (toString(row["DT_INICIO_VIGENCIA"], 0).Length != 8) sb.Append("00000000");
            else sb.Append(toString(row["DT_INICIO_VIGENCIA"], 8).Trim());

            if (toString(row["DT_FIM_VIGENCIA"], 0).Length != 8) sb.Append("00000000");
            else sb.Append(toString(row["DT_FIM_VIGENCIA"], 8).Trim());

            sb.Append(row["SEXO_SEGURADO"]);

            if (row["ESTADO_CIVIL"] == DBNull.Value || toString(row["ESTADO_CIVIL"], 0).Trim() == "0") sb.Append("1");
            else sb.Append(toString(row["ESTADO_CIVIL"], 0).Trim());

            sb.Append(row["GRAU_PARENTESCO"]);

            sb.Append(toString(row["TIPO_SEGURADO"], 0).PadLeft(1, '0')); ///////////////////////////////////?

            sb.Append(toString(row["CARGO_OCUPACAO"], 20).Trim().PadRight(20, ' '));
            sb.Append(toString(row["NOME_DA_MAE"], 70).Trim().PadRight(70, ' '));

            sb.Append("0"); sb.Append(row["CODIGO_DEPENDENTE"]);

            if (toString(row["DT_NASC_TITDEP"], 0).Length != 8) sb.Append("00000000");
            else sb.Append(toString(row["DT_NASC_TITDEP"], 8));

            if (toString(row["CPF"], 0).Length == 0) sb.Append("00000000000");
            else sb.Append(toString(row["CPF"], 11));
            sb.Append(toString(row["NACIONALIDADE"], 25).Trim().PadRight(25, ' '));

            //endereco residencial
            sb.Append(toString(row["LOGRADOURO_RES"], 80).Trim().PadRight(80, ' '));

            if (toString(row["NUMERO_RES"], 0).Length == 0) sb.Append("00000");
            else sb.Append(toString(row["NUMERO_RES"], 5).PadLeft(5, '0'));

            sb.Append(toString(row["COMPLEMENTO_RES"], 30).Trim().PadRight(30, ' '));
            sb.Append(toString(row["BAIRRO_RES"], 30).Trim().PadRight(30, ' '));
            sb.Append(toString(row["CIDADE_RES"], 30).Trim().PadRight(30, ' '));

            if (toString(row["ESTADO_RES"], 0).Length == 0) sb.Append("  ");
            else sb.Append(toString(row["ESTADO_RES"], 2).PadRight(2, ' '));

            if (toString(row["CEP_RES"], 0).Length != 8) sb.Append("00000000");
            else sb.Append(toString(row["CEP_RES"], 8).PadLeft(8, '0'));

            if (toString(row["DDD_RES"], 0).Length == 0) sb.Append("    ");
            else sb.Append(toString(row["DDD_RES"], 2).PadRight(4, ' '));
            if (toString(row["TELEFONE_RES"], 0).Length != 8) sb.Append("000000000");
            else sb.Append(toString(row["TELEFONE_RES"], 8).PadLeft(9, '0'));

            if (toString(row["DDD_CEL"], 0).Length == 0) sb.Append("    ");
            else sb.Append(toString(row["DDD_CEL"], 2).PadRight(4, ' '));
            if (toString(row["TELEFONE_CELULAR"], 0).Length == 0) sb.Append("000000000");
            else sb.Append(toString(row["TELEFONE_CELULAR"], 9).PadLeft(9, '0'));

            //endereco comercial
            sb.Append(toString(row["LOGRADOURO_COML"], 80).Trim().PadRight(80, ' '));
            sb.Append(toString(row["NUMERO_COML"], 5).PadLeft(5, '0'));
            sb.Append(toString(row["COMPLEMENTO_COML"], 30).Trim().PadRight(30, ' '));
            sb.Append(toString(row["BAIRRO_COML"], 30).Trim().PadRight(30, ' '));
            sb.Append(toString(row["CIDADE_COML"], 30).Trim().PadRight(30, ' '));
            sb.Append(toString(row["ESTADO_COML"], 2).PadRight(2, ' '));
            sb.Append(toString(row["CEP_COML"], 8).PadLeft(8, '0'));

            sb.Append(toString(row["DDD_COML"], 2).PadRight(4, ' '));
            sb.Append(toString(row["TELEFONE_COML"], 8).PadLeft(9, '0'));

            sb.Append(toString(row["DDD_FAX"], 2).PadRight(4, ' '));
            sb.Append(toString(row["FAX"], 8).PadLeft(9, '0'));

            sb.Append(toString(row["EMAIL"], 30).PadRight(30, ' '));

            sb.Append(toString(row["BANCO"], 3).PadLeft(3, '0'));
            sb.Append(toString(row["AGENCIA"], 5).PadLeft(5, '0'));
            sb.Append(toString(row["PRIMEIRO_DV_DA_AGENCIA"], 1).PadRight(1, ' '));
            sb.Append(toString(row["CONTA_CORRENTE"], 13).PadLeft(13, '0'));
            sb.Append(toString(row["DV_DA_CONTA"], 2).PadRight(2, ' '));

            sb.Append(toString(row["NOME_SUBFATURA"], 35).PadRight(35, ' '));

            sb.Append("                                   ");

            sb.Append(toString(row["DATA_CANCELAMENTO"], 8).PadLeft(8, '0'));

            sb.Append(toString(row["RG_NUMERO"], 18).PadRight(18, ' '));
            sb.Append(toString(row["RG_EMISSOR"], 18).PadRight(18, ' '));
            sb.Append(toString(row["RG_DATA"], 8).PadLeft(8, '0'));
            sb.Append("02");

            sb.Append(toString(row["CODIGO_FATURA"], 30).PadLeft(30, '0'));
            sb.Append(toString(row["CODIGO_CARENCIA"], 10).PadLeft(10, '0'));
            sb.Append(toString(row["DIA_VENCIMENTO"], 2).PadLeft(2, '0'));

            if (toString(row["ISENCAO_DE_TARIFA"], 0).ToUpper() == "S") sb.Append("1");
            else if (toString(row["ISENCAO_DE_TARIFA"], 0).ToUpper() == "N") sb.Append("0");
            else sb.Append(row["ISENCAO_DE_TARIFA"]);

            if (toString(row["ACAO_JUDICIAL"], 0).ToUpper() == "S") sb.Append("1");
            else if (toString(row["ACAO_JUDICIAL"], 0).ToUpper() == "N") sb.Append("0");
            else sb.Append(row["ACAO_JUDICIAL"]);

            sb.Append(toString(row["VALOR_DESCONTO"], 11).PadLeft(11, '0'));
            sb.Append(toString(row["LOCAL_ATENDIMENTO"], 10).PadRight(10, ' '));
            sb.Append(toString(row["NOME_DA_UNIDADE"], 70).PadRight(70, ' '));
            sb.Append(toString(row["ENTIDADE"], 70).PadRight(70, ' '));

            if (valorAdicional == null)
                sb.Append(toString(row["VALOR_PLANO"], 11).PadLeft(11, '0'));
            else
                sb.Append(Convert.ToString(valorAdicional).Trim().Replace(",", "").Replace(".", "").PadLeft(11, '0'));

            int vlGrupoFam = Convert.ToInt32(dt.Compute("SUM(VALOR_PLANO)", string.Concat("contrato_id=", row["contrato_id"])));

            sb.Append(vlGrupoFam.ToString().PadLeft(11, '0'));
        }

        void adHeaderCadastro(ref StringBuilder sb, DataTable dt, int lnNb)
        {
            sb.Append(lnNb.ToString().PadLeft(7, '0'));
            sb.Append("H");
            sb.Append("01");
            sb.Append("POSIÇÃO CADASTRAL   ");
            sb.Append("201401");
            sb.Append(DateTime.Now.ToString("ddMMyyyyHHmm"));
            sb.Append("00000000");
            sb.Append("                                   ");
            sb.Append("000000");
        }

        void adTraillerCadastro(ref StringBuilder sb, DataTable dt, int lnNb)
        {
            sb.Append(lnNb.ToString().PadLeft(7, '0'));
            sb.Append("T");
            sb.Append((lnNb - 2).ToString().PadLeft(8, '0'));

            int aux = 0;

            //total de titulares
            //aux = Convert.ToInt32(dt.Compute("count(*)", "TIPO_SEGURADO=1"));
            DataRow[] arr = dt.Select("TIPO_SEGURADO=1");
            sb.Append(arr.Length.ToString().PadLeft(8, '0'));

            //total de dependentes e agregados
            //aux = Convert.ToInt32(dt.Compute("count(*)", "TIPO_SEGURADO=2"));
            arr = dt.Select("TIPO_SEGURADO=2");
            sb.Append(arr.Length.ToString().PadLeft(8, '0'));
            sb.Append(arr.Length.ToString().PadLeft(8, '0'));
        }

        string toString(object param, int parte)
        {
            if (param == null || param == DBNull.Value)
                return string.Empty;
            else
            {
                if (parte == 0 || Convert.ToString(param).Trim().Length <= parte)
                    return Convert.ToString(param).Trim();
                else
                    return Convert.ToString(param).Trim().Substring(0, parte - 1);
            }
        }
    }
}

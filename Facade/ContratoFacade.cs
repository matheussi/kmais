namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;

    public sealed class ContratoFacade
    {
        #region Singleton 

        static ContratoFacade _instance;
        public static ContratoFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new ContratoFacade(); }
                return _instance;
            }
        }
        #endregion

        private ContratoFacade() { }

        public void Salvar(Contrato contrato, ContratoBeneficiario titular, IList<ContratoBeneficiario> dependentes, Object[] fichas, Object usuarioLiberadorID, IList<AdicionalBeneficiario> adicionalBeneficiario, Conferencia conferencia, Decimal valorTotal, bool gerarCobranca = true)
        {
            Decimal valorTotalContrato = 0;
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            //TODO: checar se o contrato foi retirado do estoque (almox_contrato_impresso -> almox_contratoimp_produtorId <> null)
            //      se nao, seta para o produtor da proposta

            try
            {
                Boolean novoContrato = contrato.ID == null;

                if (novoContrato)
                {
                    //calcula o codigo de cobranca para o contrato.
                    String qry = "SELECT MAX(contrato_codcobranca) FROM contrato";
                    object ret = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
                    if (ret == null || ret == DBNull.Value) ret = 0;
                    contrato.CodCobranca = Convert.ToInt32(ret) + 1;
                }

                //Salva o contrato.
                pm.Save(contrato);

                #region gera primeria cobranca 
                if (novoContrato && gerarCobranca)
                {
                    //gera a primeira cobranca ja paga
                    Cobranca cobranca = new Cobranca();
                    cobranca.Cancelada = false;
                    cobranca.ComissaoPaga = true;
                    cobranca.ContratoCodCobranca = Convert.ToString(contrato.CodCobranca);
                    cobranca.DataCriacao = DateTime.Now;
                    cobranca.DataPgto = contrato.Admissao;
                    cobranca.DataVencimento = contrato.Admissao;
                    cobranca.Pago = true;
                    cobranca.Parcela = 1;
                    cobranca.PropostaID = contrato.ID;
                    cobranca.Tipo = (int)Cobranca.eTipo.Normal;
                    cobranca.Valor = valorTotal;
                    cobranca.ValorPgto = cobranca.Valor;
                    pm.Save(cobranca);

                    List<CobrancaComposite> composite = new List<CobrancaComposite>();
                    Contrato.CalculaValorDaProposta2(cobranca.PropostaID, cobranca.DataVencimento, pm, false, true, ref composite, false);
                    CobrancaComposite.Salvar(cobranca.ID, composite, pm);
                    composite = null;
                }
                #endregion

                if (usuarioLiberadorID != null)
                    Contrato.SetaUsuarioLiberador(contrato.ID, usuarioLiberadorID, pm);

                //Salva o titular
                titular.ContratoID = contrato.ID;
                titular.NumeroSequencial = 0;
                titular.Vigencia = contrato.Vigencia;

                if (titular.BeneficiarioID == null)
                {
                    titular.BeneficiarioID = ContratoBeneficiario.CarregaTitularID(contrato.ID, pm);
                }
                if (titular.ID == null)
                {
                    titular.ID = ContratoBeneficiario.CarregaID_ParaTitular(contrato.ID, pm);
                }
                //if (titular.ID != null) { pm.Load(titular); } nao pode carregar, pois sobrescreve dados preenchidos na tela
                else 
                {
                    titular.Tipo = Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular);
                }

                //if (titular.ID == null) { titular.Status = (Int32)ContratoBeneficiario.eStatus.Novo; }
                /*else*/if (titular.Status == Convert.ToInt32(ContratoBeneficiario.eStatus.Incluido)) { valorTotalContrato += titular.Valor; }

                if (novoContrato) { titular.Status = (Int32)ContratoBeneficiario.eStatus.Novo; titular.Data = contrato.Admissao; titular.Vigencia = contrato.Vigencia; }
                pm.Save(titular);

                #region Salva os dependentes 

                if (dependentes != null)
                {
                    CalendarioVencimento rcv = null;
                    DateTime vigencia = DateTime.MinValue, vencimento = DateTime.MinValue; Int32 diasDataSemJuros = 0; Object valorDataLimite = null;
                    foreach (ContratoBeneficiario dependente in dependentes)
                    {
                        if (dependente.NumeroSequencial < 0)
                        {
                            dependente.NumeroSequencial = ContratoBeneficiario.ProximoNumeroSequencial(contrato.ID, dependente.BeneficiarioID, pm);
                        }

                        dependente.ContratoID = contrato.ID;

                        if (dependente.ID == null)
                        {
                            dependente.Status = (Int32)ContratoBeneficiario.eStatus.Novo;

                            if (novoContrato)
                                dependente.Vigencia = contrato.Vigencia;
                            else
                            {
                                CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contrato.ContratoADMID,
                                    dependente.Data, out vigencia, out vencimento, out diasDataSemJuros, out valorDataLimite, out rcv, pm);
                                dependente.Vigencia = vigencia;
                            }
                        }
                        else if (dependente.Status == (Int32)ContratoBeneficiario.eStatus.Incluido)
                        {
                            valorTotalContrato += dependente.Valor;
                        }

                        pm.Save(dependente);
                    }
                }
                #endregion

                //Salva as fichas de saúde dos beneficiários
                Boolean aprovadoPeloDepto = true;
                if (fichas != null && fichas.Length > 0) //&& Convert.ToInt32(contrato.ID) != 152685)
                {
                    foreach (IList<ItemDeclaracaoSaudeINSTANCIA> lista in fichas)
                    {
                        if (lista == null) { continue; }
                        foreach (ItemDeclaracaoSaudeINSTANCIA item in lista)
                        {
                            //Se id==null, tenta achar pelo id do beneficiario e do item de ficha de saude
                            if (item.ID == null)
                            {
                                item.ID = ItemDeclaracaoSaudeINSTANCIA.CarregarID(
                                    item.BeneficiarioID, item.ItemDeclaracaoID, pm);
                            }

                            if (item.ID != null)
                                pm.Save(item);
                            else if (item.Sim)
                                pm.Save(item);

                            //se tem positivacao sem aprovacao do técnico, a proposta fica pendente
                            if (item.Sim && !item.AprovadoPeloDeptoTecnico && aprovadoPeloDepto == true) { aprovadoPeloDepto = false; }
                        }
                    }
                }

                if (novoContrato && !aprovadoPeloDepto) //se o contrato é novo e há positivacoes nas fichas de saude SEM a aprovacao do Depto. Tecnico:
                {
                    contrato.Pendente = true;
                    pm.Save(contrato);
                }

                //salva os produtos adicionais contratados
                if (adicionalBeneficiario != null)
                {
                    foreach (AdicionalBeneficiario ad in adicionalBeneficiario)
                    {
                        ad.PropostaID = contrato.ID;
                        pm.Save(ad);
                    }
                }

                ////checa historico de planos e atualiza a última entrada se necessário.
                //ContratoPlano obj = ContratoPlano.CarregarAtual(contrato.ID, pm);
                //if (obj != null)
                //{
                //    if (Convert.ToString(obj.PlanoID) != Convert.ToString(contrato.PlanoID))
                //    {
                //        obj.PlanoID = contrato.PlanoID;
                //        pm.Save(obj);
                //    }

                //    obj = null;
                //}

                //checa almoxarifado - contrato impresso
                String letra = "";
                if (PrimeiraPosicaoELetra(contrato.Numero))
                    letra = contrato.Numero.Substring(0, 1);

                AlmoxContratoImpresso aci = null;
                //if (!String.IsNullOrEmpty(letra))
                //    aci = AlmoxContratoImpresso.Carregar(contrato.OperadoraID, contrato.Numero.Replace(letra, ""), letra, -1, pm);
                //else
                //    aci = AlmoxContratoImpresso.Carregar(contrato.OperadoraID, contrato.Numero, letra, -1, pm);

                if (aci != null && aci.AgenteID == null)
                {
                    aci.AgenteID = contrato.DonoID;
                    aci.Data = contrato.Data;
                    pm.Save(aci);
                }
                else if (aci == null)
                {
                    #region IMPOSSIVEL
                    //aci = new AlmoxContratoImpresso();
                    //aci.AgenteID = contrato.DonoID;
                    //aci.Data = contrato.Data;
                    //aci.MovID = null; //?????
                    //aci.Numero = contrato.Numero;
                    //aci.OperadoraID = contrato.OperadoraID;
                    //aci.ProdutoID = null; //?????
                    ////pm.Save(aci);
                    #endregion
                }

                //Altera status da proposta em conferencia / cadastro
                if (conferencia != null)
                {
                    conferencia.Carregar();
                    conferencia.Departamento = 6; //TODO: corrigir (Int32)ContratoStatusHistorico.eStatus.Cadastrado;
                    pm.Save(conferencia);

                    ContratoStatusHistorico.Salvar(contrato.Numero, contrato.OperadoraID, ContratoStatusHistorico.eStatus.Cadastrado, pm);
                }
                else if (novoContrato)
                {
                    ContratoStatusHistorico.Salvar(contrato.Numero, contrato.OperadoraID, ContratoStatusHistorico.eStatus.Cadastrado, pm);
                }

                //Checa se é necessário gravar o valor total do contrato
                if (valorTotalContrato > 0)
                {
                    ContratoValor.InsereNovoValorSeNecessario(contrato.ID, valorTotalContrato, pm);
                }

                pm.Commit();
            }
            catch(Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }

        //As cobrancas não sao geradas em lotes mais
        //public void GerarOuAtualizarCobrancas(List<String> contratoIDs)
        //{
        //    String inClausule = String.Join(",", contratoIDs.ToArray());
        //    PersistenceManager pm = new PersistenceManager();
        //    pm.BeginTransactionContext();

        //    try
        //    {
        //        String qry = String.Concat("SELECT contrato_id as ContratoID, contrato_contratoAdmId as ContratoAdmID, contrato_vencimento as ContratoVencimento, SUM(contratobeneficiario_valor) AS Total ", 
        //        "   FROM contrato ",
        //        "       INNER JOIN contrato_beneficiario ON contratobeneficiario_contratoId = contrato_id ",
        //        "   WHERE contratobeneficiario_status=2 AND contrato_id IN (", inClausule, ") ",
        //        "   GROUP BY contrato_id, contrato_contratoAdmId, contrato_vencimento HAVING SUM(contratobeneficiario_valor) > 0");

        //        DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];

        //        DateTime vigenciaBeneficiario, vencimentoBeneficiario;
        //        Int32 diaDataSemJuros; Object valorDataLimite;

        //        if (dt.Rows.Count > 0)
        //        {
        //            IList<Cobranca> cobrancasExistentes = null;
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                cobrancasExistentes = Cobranca.CarregarTodas(row["ContratoID"], pm);

        //                if (cobrancasExistentes == null) //nao tem cobranças geradas. deve-se somente gerá-las
        //                {
        //                    Cobranca.Gerar(row["ContratoID"],
        //                        Convert.ToDateTime(row["ContratoVencimento"], new System.Globalization.CultureInfo("pt-Br")), 12, pm);

        //                    ContratoValor.InsereNovoValorSeNecessario(row["ContratoID"], Convert.ToDecimal(row["Total"]), pm);
        //                }
        //                else //Tem cobranças geradas. Precisa ver qual delas deverá ser atualizada.
        //                {
        //                    //Recalcula o valor total do contrato somando todos os beneficiarios ativos e com vencimento compativel às cobrancas
        //                    IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID(row["ContratoID"], true, false, pm);
        //                    Contrato contrato = new Contrato(row["ContratoID"]);
        //                    pm.Load(contrato);

        //                    Decimal novoTotalContrato;

        //                    Int32 i = 0;
        //                    foreach (Cobranca cobrancaExistente in cobrancasExistentes)
        //                    {
        //                        novoTotalContrato = 0;
        //                        Int32 qtdBeneficiarioParaEssaCobranca = 0;
        //                        foreach (ContratoBeneficiario beneficiario in beneficiarios)
        //                        {
        //                            CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(
        //                                row["ContratoAdmID"], beneficiario.Data, out vigenciaBeneficiario, out vencimentoBeneficiario, out diaDataSemJuros, out valorDataLimite, pm);

        //                            //se o beneficiario ja entra na cobranca, incrementa seu valor
        //                            if (cobrancaExistente.DataVencimento >= vencimentoBeneficiario)
        //                            {
        //                                qtdBeneficiarioParaEssaCobranca++;
        //                                novoTotalContrato += Contrato.CalculaValorDaPropostaSemTaxaAssociativa(beneficiario.ContratoID, beneficiario, cobrancaExistente.DataVencimento, pm); //beneficiario.Valor;
        //                            }
        //                        }

        //                        novoTotalContrato += Contrato.CalculaValorDaTaxaAssociativa(contrato, qtdBeneficiarioParaEssaCobranca, pm);
        //                        cobrancaExistente.Valor = novoTotalContrato;

        //                        i++;
        //                        pm.Save(cobrancaExistente);

        //                        if (i == cobrancasExistentes.Count) //atualiza o novo valor do contrato
        //                        {
        //                            ContratoValor.InsereNovoValorSeNecessario(row["ContratoID"], novoTotalContrato, pm);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        pm.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        pm.Rollback();
        //        throw ex;
        //    }
        //    finally
        //    {
        //        pm = null;
        //    }
        //}

        /// <summary>
        /// Atribui o status de Adimplente ou Inadimplente (proprieade Adimplente do objeto Contrato) 
        /// segundo cobranças em aberto de cada contrato.
        /// </summary>
        /// <param name="pm">Objeto PersistenceManager participante de uma transação.</param>
        public void AtribuiStatusAdimplenteOuInadimplente(PersistenceManager pm)
        {
            PersistenceManager _pm = null;

            IList<Contrato> inadimplentes = Contrato.BuscarECarregarInadimplentes_PORCOBRANCA(pm);

            if (pm == null)
            {
                _pm = new PersistenceManager();
                _pm.BeginTransactionContext();
            }
            else
                _pm = pm;

            try
            {
                if (inadimplentes != null && inadimplentes.Count > 0)
                {
                    String[] ids = new String[inadimplentes.Count];
                    for (Int32 i = 0; i < inadimplentes.Count; i++)
                    {
                        ids[i] = Convert.ToString(inadimplentes[i].ID);
                    }

                    String inClausule = String.Join(",", ids);
                    String command = "UPDATE contrato SET contrato_adimplente=0 WHERE contrato_adimplente=1 AND contrato_id IN (" + inClausule + ")";
                    NonQueryHelper.Instance.ExecuteNonQuery(command, _pm);

                    //restaura contratos ok para ADIMPLENTE
                    command = "UPDATE contrato SET contrato_adimplente=1 WHERE contrato_adimplente=0 AND contrato_cancelado=0 AND contrato_rascunho=0 AND contrato_inativo <> 1 AND contrato_id NOT IN (" + inClausule + ")";
                    NonQueryHelper.Instance.ExecuteNonQuery(command, _pm);
                }
                else
                {
                    //todos os contratos estão ADIMPLENTES
                    String command = "UPDATE contrato SET contrato_adimplente=1 WHERE contrato_adimplente=0 AND contrato_cancelado=0 AND contrato_rascunho=0 AND contrato_inativo <> 1 ";
                    NonQueryHelper.Instance.ExecuteNonQuery(command, _pm);
                }

                if (pm == null) { _pm.Commit(); }
            }
            catch
            {
                if (pm == null) { _pm.Rollback(); }
                throw;
            }
            finally
            {
                if (pm == null) { _pm = null; }
            }
        }

        static Boolean PrimeiraPosicaoELetra(String param)
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

        public void SetaDataAlteracaoContratos(Object beneficiarioId)
        {
            IList<Contrato> contratos = Contrato.CarregarPorBeneficiário(beneficiarioId);
            if (contratos == null || contratos.Count == 0) { return; }

            StringBuilder sb = new StringBuilder();
            DateTime data = DateTime.Now;

            foreach (Contrato contrato in contratos)
            {
                if (sb.Length > 0) { sb.Append(";"); }

                sb.Append("UPDATE contrato SET contrato_alteracao='");
                sb.Append(data.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.Append("' WHERE contrato_id="); sb.Append(contrato.ID);
            }

            NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), null);
        }

        public System.Collections.Hashtable InativaContratos(String[] numerosDasPropostas, Object operadoraId, DateTime dataInativacao, DateTime? faltaDePagamentoEm, Object statusId, String statusObs, Object usuarioId)
        {
            if (operadoraId == null || numerosDasPropostas == null || numerosDasPropostas.Length == 0) { return null; }

            Object oret = null;
            Int32 iret = 0;
            String obs = "";

            if(faltaDePagamentoEm != null)
                obs = String.Concat("Inativado em ", dataInativacao.ToString("dd/MM/yyyy"), " por falta de pagamento em ", faltaDePagamentoEm.Value.ToString("dd/MM/yyyy"), ".");

            StringBuilder sb = new StringBuilder();

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            System.Collections.Hashtable ht = new System.Collections.Hashtable();

            try
            {
                ContratoStatusInstancia csi = null;
                foreach (String numero in numerosDasPropostas)
                {
                    if (numero == null || numero.Trim() == "") { continue; }

                    //Pega as observacoes do contrato
                    if (!String.IsNullOrEmpty(obs))
                    {
                        oret = LocatorHelper.Instance.ExecuteScalar(String.Concat("SELECT contrato_obs FROM contrato WHERE contrato_numero='", numero.Trim(), "' AND contrato_operadoraId=", operadoraId), null, null, pm);
                        if (oret == null || oret == DBNull.Value)
                            oret = obs;
                        else
                            oret = String.Concat(Convert.ToString(oret), Environment.NewLine, obs);
                    }
                    else
                        oret = null;

                    sb.Append("UPDATE contrato SET contrato_inativo=1, contrato_dataCancelamento='");
                    sb.Append(dataInativacao.ToString("yyyy-MM-dd"));

                    if (oret != null)
                    {
                        sb.Append("', contrato_obs='");
                        sb.Append(Convert.ToString(oret).Replace("'", "´"));
                    }

                    sb.Append("', contrato_alteracao='");
                    sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sb.Append("' WHERE contrato_numero='");
                    sb.Append(numero.Trim()); sb.Append("' AND contrato_operadoraId=");
                    sb.Append(operadoraId);

                    iret = NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
                    sb.Remove(0, sb.Length);

                    //pega o id do contrato
                    oret = LocatorHelper.Instance.ExecuteScalar(String.Concat("SELECT contrato_id FROM contrato WHERE contrato_numero='", numero.Trim(), "' AND contrato_operadoraId=", operadoraId), null, null, pm);

                    if (!ht.ContainsKey(numero.Trim()))
                    {
                        if (iret > 0)
                        {
                            ht.Add(numero.Trim(), "ok");
                            csi = new ContratoStatusInstancia();
                            csi.ContratoID = oret;
                            csi.Data = dataInativacao;
                            csi.DataSistema = DateTime.Now;
                            csi.StatusID = statusId;
                            csi.StatusTipo = (int)ContratoStatus.eTipo.Inativacao;
                            csi.UsuarioID = usuarioId;
                            csi.OBS = statusObs;
                            pm.Save(csi);
                        }
                        else
                            ht.Add(numero.Trim(), "falhou");
                    }

                    //TODO: marcar os beneficiarios para geracao de arquivos
                }

                pm.Commit();
                return ht;
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

        public System.Collections.Hashtable ReativaContratos(String[] numerosDasPropostas, Object operadoraId, DateTime dataReativacao, Object statusId, String statusObs, Object usuarioId)
        {
            if (operadoraId == null || numerosDasPropostas == null || numerosDasPropostas.Length == 0) { return null; }

            Object oret = null;
            Int32 iret = 0;

            StringBuilder sb = new StringBuilder();

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            System.Collections.Hashtable ht = new System.Collections.Hashtable();

            try
            {
                ContratoStatusInstancia csi = null;
                foreach (String numero in numerosDasPropostas)
                {
                    if (numero == null || numero.Trim() == "") { continue; }

                    //Pega as observacoes do contrato
                    oret = LocatorHelper.Instance.ExecuteScalar(String.Concat("SELECT contrato_obs FROM contrato WHERE contrato_numero='", numero.Trim(), "' AND contrato_operadoraId=", operadoraId), null, null, pm);
                    if (oret == null || oret == DBNull.Value || Convert.ToString(oret) == "")
                        oret = statusObs;
                    else
                        oret = String.Concat(Convert.ToString(oret), Environment.NewLine, statusObs);

                    sb.Append("UPDATE contrato SET contrato_inativo=0, contrato_dataCancelamento=NULL");

                    sb.Append(", contrato_obs='");
                    sb.Append(Convert.ToString(oret).Replace("'", "´"));

                    sb.Append("', contrato_alteracao='");
                    sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sb.Append("' WHERE contrato_numero='");
                    sb.Append(numero.Trim()); sb.Append("' AND contrato_operadoraId=");
                    sb.Append(operadoraId);

                    iret = NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
                    sb.Remove(0, sb.Length);

                    //pega o id do contrato
                    oret = LocatorHelper.Instance.ExecuteScalar(String.Concat("SELECT contrato_id FROM contrato WHERE contrato_numero='", numero.Trim(), "' AND contrato_operadoraId=", operadoraId), null, null, pm);

                    if (!ht.ContainsKey(numero.Trim()))
                    {
                        if (iret > 0)
                        {
                            ht.Add(numero.Trim(), "ok");
                            csi = new ContratoStatusInstancia();
                            csi.ContratoID = oret;
                            csi.Data = dataReativacao;
                            csi.DataSistema = DateTime.Now;
                            csi.StatusID = statusId;
                            csi.StatusTipo = (int)ContratoStatus.eTipo.Reativacao;
                            csi.UsuarioID = usuarioId;
                            csi.OBS = statusObs;
                            pm.Save(csi);
                        }
                        else
                            ht.Add(numero.Trim(), "falhou");
                    }

                    //marcar os beneficiarios para geracao de arquivos
                    NonQueryHelper.Instance.ExecuteNonQuery(String.Concat(
                        "update contrato_beneficiario set contratobeneficiario_status=", (int)ContratoBeneficiario.eStatus.Novo, " where contratobeneficiario_ativo=1 and contratobeneficiario_contratoid=", oret), 
                        pm);
                }

                pm.Commit();
                return ht;
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

        public DataTable RelatorioConferenciaDigitacao(String[] contratoAdmIds, DateTime vigencia)
        {
            String qry = String.Concat("SELECT contratoadm_descricao as ContratoAdmDescricao, contrato_numero as PropostaNumero, beneficiario_nome as TitularNome, (SELECT COUNT(contratobeneficiario_id) FROM contrato_beneficiario WHERE contratobeneficiario_contratoId=contrato_id) as QtdVidas ",
                "   FROM contrato ",
                "       INNER JOIN contratoADM ON contratoadm_id = contrato_contratoAdmId ",
                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_contratoId=contrato_id ",
                "       INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id AND contratobeneficiario_tipo=0 ",
                "   WHERE contrato_rascunho<>1 AND contrato_cancelado<>1 AND contrato_inativo<>1 ",
                "       AND contrato_vigencia='", vigencia.ToString("yyyy-MM-dd 00:00:00"), "'",
                "       AND contrato_contratoAdmId IN (", String.Join(",", contratoAdmIds),") ",
                "   ORDER BY contratoadm_descricao, contrato_numero");

            return LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];
        }

        public void AtualizaValorDeCobrancas(Object contratoId, out String msg)
        {
            msg = "";

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            Contrato contrato = new Contrato(contratoId);
            pm.Load(contrato);

            if (contrato.Legado) { pm.Rollback(); return; }

            try
            {
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas(contratoId, true, Cobranca.eTipo.Normal, pm);

                if (cobrancas != null)
                {
                    List<CobrancaComposite> composite = null;
                    foreach (Cobranca cobranca in cobrancas)
                    {
                        //if (cobranca.Pago) { continue; }
                        if (cobranca.Parcela == 1) { continue; } //nao calcula a parcela 1 pq ela nao está na vigência do beneficiário
                        if (cobranca.Tipo != Convert.ToInt32(Cobranca.eTipo.Normal)) { continue; }

                        if (cobranca.DataVencimento.Year >= DateTime.Now.Year) //atualiza somente parcelas atuais
                        {
                            cobranca.Valor = Contrato.CalculaValorDaProposta2(contratoId, cobranca.DataVencimento, pm, false, true, ref composite, true);

                            if (cobranca.Valor > 0)
                            {
                                if (!cobranca.Pago) { pm.Save(cobranca); }
                            }
                            else if (String.IsNullOrEmpty(msg))
                            {
                                if (!cobranca.Pago)
                                {
                                    msg = String.Concat("Data de vencimento ",
                                        cobranca.DataVencimento.ToString("dd/MM/yyyy"), " descoberta. Cobrança com valor incorreto.");
                                    continue;
                                }
                            }

                            if (composite != null)
                            {
                                CobrancaComposite.Remover(cobranca.ID, pm);
                                foreach (CobrancaComposite comp in composite)
                                {
                                    comp.CobrancaID = cobranca.ID;
                                    pm.Save(comp);
                                }
                            }
                        }
                    }
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                //throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }
        }
    }
}
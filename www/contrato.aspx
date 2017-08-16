<%@ Page EnableEventValidation="false" Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="contrato.aspx.cs" Inherits="www.contrato" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr>
            <td width="65" rowspan="3">
                <img src="images/icones/propostas.png" /><%--<img height="50" src="images/imgTitulos/contratos_65_50.png" />--%>
            </td>
        </tr>
        <tr>
            <td> 
                <span class="titulo">Proposta</span>
            </td>
        </tr>
        <tr>
            <td nowrap>
                <span class="subtitulo">Preencha os dados abaixo e clique em salvar</span>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <input type="hidden" id="txtIdKey" runat="server" />

    <script src="jQuery/jquery-1.3.2.js" type="text/javascript"></script>

    <script src="jQuery/plugins/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        function pageLoad() {

            $(document).ready(function() {
                $("#<%= txtCorretor.ClientID %>").autocomplete("searchCorretorMethod.aspx", {
                    width: 300,
                    scroll: false,
                    dataType: "json",
                    cacheLength: 0,
                    extraParams: { filial: function () { return $("#<%= cboFilial.ClientID %>").val(); } },
                    parse: function(data) {
                        var parsed = [];
                        data = data.Corretores;

                        for (var i = 0; i < data.length; i++) {
                            parsed[parsed.length] = {
                                data: data[i],
                                value: data[i].ID,
                                result: data[i].Nome + " (" + data[i].Doc + " - " + data[i].Codigo + ")"
                            };
                        }

                        return parsed;
                    },
                    formatItem: function(item) {
                        return item.Nome + " (" + item.Doc + " - " + item.Codigo + ")";
                    }
                }).result(function(e, item) {
                    document.getElementById('<%= txtCorretorID.ClientID %>').value = item.ID;
                    //$('ctl00_cphContent_tab_p1_txtCorretor').flushCache();
                    //                    alert(item.TipoPessoa);
                    if (item.Tipo == 1) {
                        document.getElementById('<%= txtCorretorTerceiroCPF.ClientID %>').value = item.Doc;
                        document.getElementById('<%= txtCorretorTerceiroIdentificacao.ClientID %>').value = item.Nome;
                    }
                    else {
                        document.getElementById('<%= txtCorretorTerceiroCPF.ClientID %>').value = '';
                        document.getElementById('<%= txtCorretorTerceiroIdentificacao.ClientID %>').value = '';
                    }
                }
                );
            });

            //http://stackoverflow.com/questions/3216186/jquery-autocomplete-clear-cache

            $(document).ready(function() {
                $("#<%= txtOperador.ClientID %>").autocomplete("searchOperadorTMKTMethod.aspx", {
                    width: 300,
                    scroll: false,
                    dataType: "json",
                    parse: function(data) {
                        var parsed = [];
                        data = data.Operadores;

                        for (var i = 0; i < data.length; i++) {
                            parsed[parsed.length] = {
                                data: data[i],
                                value: data[i].ID,
                                result: data[i].Nome + " (" + data[i].Doc + ")"
                            };
                        }

                        return parsed;
                    },
                    formatItem: function(item) {
                        return item.Nome + " (" + item.Doc + ")";
                    }
                }).result(function(e, item) {
                    document.getElementById('<%= txtCorretorID.ClientID %>').value = item.ID;
                }
                );
            });

            $(document).ready(function() {
                $("#<%= cboCarenciaOperadora.ClientID %>").autocomplete("searchOperadoraOrigemMethod.aspx", {
                    width: 300,
                    scroll: false,
                    dataType: "json",
                    parse: function(data) {
                        var parsed = [];
                        data = data.Operadoras;

                        for (var i = 0; i < data.length; i++) {
                            parsed[parsed.length] = {
                                data: data[i],
                                value: data[i].ID,
                                result: data[i].Nome + " (" + data[i].ANS + ")"
                            };
                        }

                        return parsed;
                    },
                    formatItem: function(item) {
                        return item.Nome + " (" + item.ANS + ")";
                    }
                }).result(function(e, item) {
                    document.getElementById('<%= txtCarenciaOperadoraID.ClientID %>').value = item.ID;
                }
                );
            });

            $(document).ready(function() {
                $("#<%= cboCarenciaDependenteOperadora.ClientID %>").autocomplete("searchOperadoraOrigemMethod.aspx", {
                    width: 300,
                    scroll: false,
                    dataType: "json",
                    parse: function(data) {
                        var parsed = [];
                        data = data.Operadoras;

                        for (var i = 0; i < data.length; i++) {
                            parsed[parsed.length] = {
                                data: data[i],
                                value: data[i].ID,
                                result: data[i].Nome + " (" + data[i].ANS + ")"
                            };
                        }

                        return parsed;
                    },
                    formatItem: function(item) {
                        return item.Nome + " (" + item.ANS + ")";
                    }
                }).result(function(e, item) {
                    document.getElementById('<%= txtCarenciaDependenteOperadoraID.ClientID %>').value = item.ID;
                }
                );
            });

            /******************************************************************************************/

            $(document).ready(function() {
                $("#<%= txtCorretorTerceiroIdentificacao.ClientID %>").autocomplete("searchCorrTercMethod.aspx", {
                    width: 300,
                    scroll: false,
                    dataType: "json",
                    parse: function(data) {
                        var parsed = [];
                        data = data.CorretoresTerc;

                        for (var i = 0; i < data.length; i++) {
                            parsed[parsed.length] = {
                                data: data[i],
                                value: data[i].Nome,
                                result: data[i].Nome + " (" + data[i].CPF + ")"
                            };
                        }

                        return parsed;
                    },
                    formatItem: function(item) {
                        return item.Nome + " (" + item.CPF + ")";
                    }
                }).result(function(e, item) {
                    document.getElementById('<%= txtCorretorTerceiroIdentificacao.ClientID %>').value = item.Nome;
                    document.getElementById('<%= txtCorretorTerceiroCPF.ClientID %>').value = item.CPF;
                }
                );
            });

            $(document).ready(function() {
                $("#<%= txtSuperiorTerceiroIdentificacao.ClientID %>").autocomplete("searchSuperTercMethod.aspx", {
                    width: 300,
                    scroll: false,
                    dataType: "json",
                    parse: function(data) {
                        var parsed = [];
                        data = data.SuperioresTerc;

                        for (var i = 0; i < data.length; i++) {
                            parsed[parsed.length] = {
                                data: data[i],
                                value: data[i].Nome,
                                result: data[i].Nome + " (" + data[i].CPF + ")"
                            };
                        }

                        return parsed;
                    },
                    formatItem: function(item) {
                        return item.Nome + " (" + item.CPF + ")";
                    }
                }).result(function(e, item) {
                    document.getElementById('<%= txtSuperiorTerceiroIdentificacao.ClientID %>').value = item.Nome;
                    document.getElementById('<%= txtSuperiorTerceiroCPF.ClientID %>').value = item.CPF;
                }
                );
            });
        }
    </script>

    <script language="javascript" type="text/javascript">
        function checaConclusaoAtendimento()
        {
            if (document.getElementById('<%= chkAtendimentoConcluido.ClientID %>').checked)  //(data != '') {
            {
                return confirm('Deseja realmente encerrar o atendimento?');
            }
            else
            {
                return true;
            }
        }
        function beneficiarioNaoLocalizado(tipo)
        {
            if (confirm('Beneficiário não localizado. Deseja cadastrá-lo agora?'))
            {
                var cpf = '';
                var keyid = '';
                if (tipo == 2)
                {
                    cpf = document.getElementById('<%= txtCPFDependente.ClientID %>').value;
                    keyid = document.getElementById('<%= txtIdKey.ClientID %>').value;
                }
                else if (tipo == 1)
                {
                    cpf = document.getElementById('<%= txtCPF.ClientID %>').value;
                }
                win = window.open('beneficiarioP.aspx?et=' + tipo + '&cpf=' + cpf + '&keyid=' + keyid, 'janela', 'toolbar=no,scrollbars=1,width=860,height=420');
                win.moveTo(100, 150);
            }
        }

        function setActiveTabIndex(index) 
        {
            $find('<%=tab.ClientID%>').set_activeTabIndex(index);
        }
        function ActiveTabChanged(sender, e)
        {
            switch (sender.get_activeTabIndex())
            {
                case 1: //beneficiario titular
                {
                    __doPostBack('<%=upBeneficiario.ClientID%>', '');
                    break;
                }
                case 2: //dados cadastrais
                {
                    __doPostBack('<%=upDadosCadastrais.ClientID%>', '');
                    break;
                }
                case 3: //dependentes
                {
                    __doPostBack('<%=upDependente.ClientID%>', '');
                    break;
                }
//                case 4: //ficha de saude
//                {
//                    __doPostBack('<%=upFichaSaude.ClientID%>', '');
//                    break;
//                }
                case 4: //adicionais 5
                {
                    __doPostBack('<%=upAdicionais.ClientID%>', '');
                    break;
                }
                case 5: //finalizacao 6
                {
                    __doPostBack('<%=upFinalizacao.ClientID%>', '');
                    break;
                }
            }
        }
    </script>

    <cc1:TabContainer BorderStyle="None" BorderWidth="0" Width="100%" ID="tab" runat="server" ActiveTabIndex="0" OnClientActiveTabChanged="ActiveTabChanged">
        <cc1:TabPanel runat="server" ID="p1">
            <HeaderTemplate>
                <span class="subtitulo">Dados comuns</span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel runat="server" ID="upDadosComuns" UpdateMode="Conditional">
                    <ContentTemplate>
                        <br />
                        <asp:Panel ID="pnlEnriquecimento" runat="server" Visible="false" EnableViewState="false">
                            <table cellpadding="2" width="600" border="0" style="border: solid 1px #507CD1;background-color:#EFF3FB">
                                <tr>
                                    <td><strong><span style="color:#507CD1;font-size:9pt">Há informações de enriquecimento para confimarção:</span></strong></td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:GridView ID="gridEnriquecimento" runat="server" SkinID="gridViewSkin" 
                                            OnRowDataBound="gridEnriquecimento_RowDataBound" OnRowCommand="gridEnriquecimento_RowCommand"
                                            DataKeyNames="id_telMail,id_beneficiario" AutoGenerateColumns="False" Width="100%">
                                            <Columns>
                                                <asp:BoundField ItemStyle-Wrap="false" DataField="beneficiario_nome" HeaderText="Nome" Visible="true">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField ItemStyle-Wrap="false" DataField="tipo" HeaderText="Tipo" Visible="true">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField ItemStyle-Wrap="false" DataField="ddd" HeaderText="DDD">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField ItemStyle-Wrap="false" DataField="telefone" HeaderText="Número">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField ItemStyle-Wrap="false" DataField="ramal" HeaderText="Ramal">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField ItemStyle-Wrap="false" DataField="email" HeaderText="E-mail" >
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:ButtonField Text="<img src='images/tick.png' title='confirmar' alt='confirmar' border='0' />" CommandName="ok">
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                            <br />
                        </asp:Panel>
                        <input type="hidden" runat="server" id="txtIdTitularOculto" />
                        <asp:Panel ID="pnlSelNumeroContral" runat="server" EnableViewState="false" Visible="false">
                            <table width="600px" cellpadding="4" cellspacing="0" style="border: solid 1px #507CD1">
                                <tr>
                                    <td class="tdNormal1">
                                        Selecione abaixo para qual operadora está cadastrando esta proposta.
                                    </td>
                                </tr>
                            </table>
                            <asp:GridView ID="gridSelNumeroContral" Width="600px" SkinID="gridViewSkin" EnableViewState="false"
                                runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="ID,OperadoraID,AgenteID"
                                OnRowCommand="gridSelNumeroContral_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:ButtonField Text="<img src='images/active.png' title='selecionar' alt='selecionar' border='0' />"
                                        CommandName="usar">
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                            <br />
                        </asp:Panel>
                        
                        <table cellpadding="2" width="600" border="0" style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdPrincipal1" width="102px">
                                    Num. Contrato
                                </td>
                                <td class="tdNormal1" width="80px">
                                    <asp:TextBox Width="70px" runat="server" SkinID="textboxSkin" MaxLength="12" ID="txtNumeroContrato" />
                                </td>
                                <%--AutoPostBack="true" OnTextChanged="txtContrato_TextChanged"--%>
                                <td class="tdPrincipal1" width="98px">
                                    Confirme
                                </td>
                                <td class="tdNormal1" width="300">
                                    <asp:TextBox Width="70px" runat="server" SkinID="textboxSkin" MaxLength="12" ID="txtNumeroContratoConfirme" />
                                    &nbsp;
                                    <asp:LinkButton ID="lnkOkContrato" runat="server" Text="confirmar número" OnClick="lnkOkContrato_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Filial
                                </td>
                                <td class="tdNormal1" colspan="3">
                                    <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboFilial" Width="393px" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" valign="top">
                                    Produtor
                                </td>
                                <td class="tdNormal1" colspan="3">
                                    <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtCorretor" Width="390px" />
                                    <input type="hidden" name="txtCorretorID" id="txtCorretorID" runat="server" />
                                    <br />
                                    <br />
                                    <table width="100%" cellpadding="1" cellspacing="0">
                                        <tr>
                                            <td width="90px">
                                                Ident. Corretor
                                            </td>
                                            <td width="174px">
                                                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtCorretorTerceiroIdentificacao"
                                                    Width="170px" MaxLength="240" />
                                            </td>
                                            <td width="40px" align="center">
                                                CPF
                                            </td>
                                            <td>
                                                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtCorretorTerceiroCPF" Width="79px" />
                                                <cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeCorretorTerceiroCPF"
                                                    Mask="999,999,999-99" ClearMaskOnLostFocus="true" TargetControlID="txtCorretorTerceiroCPF" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="90px">
                                                Ident. Superior
                                            </td>
                                            <td width="174px">
                                                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtSuperiorTerceiroIdentificacao"
                                                    Width="170px" MaxLength="240" />
                                            </td>
                                            <td width="40px" align="center">
                                                CPF
                                            </td>
                                            <td>
                                                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtSuperiorTerceiroCPF" Width="79px" />
                                                <cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeSuperiorTerceiroCPF"
                                                    Mask="999,999,999-99" ClearMaskOnLostFocus="true" TargetControlID="txtSuperiorTerceiroCPF" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" enableviewstate="false" visible="false">
                                <td class="tdPrincipal1">
                                    Plataforma
                                </td>
                                <td class="tdNormal1" colspan="3">
                                    <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtOperador" Width="390px" />
                                    <input type="hidden" name="txtOperadorID" id="txtOperadorID" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Núm. da Matrícula
                                </td>
                                <td class="tdNormal1" colspan="3">
                                    <asp:TextBox Width="155px" runat="server" CssClass="textbox" MaxLength="12" ID="txtNumeroMatricula" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="2" width="600" border="0" style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdPrincipal1" width="102px">
                                    Tipo de proposta
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboTipoProposta" AutoPostBack="true"
                                        OnSelectedIndexChanged="cboTipoProposta_OnSelectedIndexChanged" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="2" width="600" style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdPrincipal1" width="102px">
                                    Estipulante
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList SkinID="dropdownSkin" Width="395px" runat="server" ID="cboEstipulante"
                                        AutoPostBack="true" OnSelectedIndexChanged="cboEstipulante_OnSelectedIndexChanged" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="102px">
                                    Operadora
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList SkinID="dropdownSkin" AutoPostBack="True" Width="395px" runat="server"
                                        ID="cboOperadora" OnSelectedIndexChanged="cboOperadora_SelectedIndexChanged" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="102px">
                                    Contrato
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList SkinID="dropdownSkin" AutoPostBack="True" Width="395px" runat="server"
                                        ID="cboContrato" OnSelectedIndexChanged="cboContrato_SelectedIndexChanged" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="102px">
                                    Plano
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList SkinID="dropdownSkin" Width="395px" runat="server" ID="cboPlano"
                                        AutoPostBack="true" OnSelectedIndexChanged="cboPlano_OnSelectedIndexChanged" />
                                    &nbsp;
                                    <asp:Button runat="server" SkinID="botaoAzulBorda" Text="migrar" ID="cmdAlterarPlano"
                                        OnClick="cmdAlterarPlano_Click" EnableViewState="false" Visible="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="102px">
                                    Acomodação
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList SkinID="dropdownSkin" Width="395px" runat="server" ID="cboAcomodacao"
                                        AutoPostBack="true" OnSelectedIndexChanged="cboAcomodacao_OnSelectedIndexChanged" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <asp:Panel runat="server" ID="pnlInfoAnterior" Visible="false">
                            <table cellpadding="3" cellspacing="1" width="600" style="border: solid 1px #507CD1">
                                <tr>
                                    <td class="tdPrincipal1" width="102px">
                                        Empresa anterior
                                    </td>
                                    <td class="tdNormal1">
                                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtEmpresaAnterior" Width="395px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tdPrincipal1" width="102px">
                                        Matrícula
                                    </td>
                                    <td class="tdNormal1">
                                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtEmpresaAnteriorMatricula"
                                            MaxLength="100" Width="120px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tdPrincipal1" width="102px">
                                        Tempo
                                    </td>
                                    <td class="tdNormal1">
                                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtEmpresaAnteriorMeses" MaxLength="4"
                                            Width="30px" />&nbsp;meses
                                    </td>
                                </tr>
                            </table>
                            <br />
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlHistoricoPlano" Visible="false">
                            <table cellpadding="0" cellspacing="0" width="600" style="border: solid 1px #507CD1">
                                <tr>
                                    <td class="tdNormal1">
                                        <asp:CheckBox ID="chkHistoricoPlano" title='Histórico de alterações de plano' Text="Histórico de alterações de plano"
                                            runat="server" SkinID="checkboxSkin" AutoPostBack="true" OnCheckedChanged="chkHistoricoPlano_OnCheckedChanged"
                                            EnableViewState="true" />
                                    </td>
                                </tr>
                                <tr id="trHistoricoPlano" runat="server" visible="false" enableviewstate="true">
                                    <td class="tdNormal1">
                                        <asp:GridView ID="gridHistoricoPlano" OnRowDataBound="gridHistoricoPlano_OnRowDataBound"
                                            OnRowCommand="gridHistoricoPlano_RowCommand" Width="100%" SkinID="gridViewSkin"
                                            EnableViewState="true" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
                                            <Columns>
                                                <asp:BoundField DataField="PlanoDescricao" HeaderText="Plano">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Data" HeaderText="Admissão" DataFormatString="{0:dd/MM/yyyy}">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle Width="10%" />
                                                </asp:BoundField>
                                                <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />"
                                                    CommandName="excluir">
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                            <br />
                        </asp:Panel>
                        <table cellpadding="2" width="600" border="0" style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdPrincipal1" width="102px">
                                    Admissão
                                </td>
                                <td class="tdNormal1" width="93px">
                                    <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtAdmissao" Width="66px" AutoPostBack="true" OnTextChanged="txtAdmissao_OnTextChanged" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    <asp:Image SkinID="imgCanlendario" ID="imgAdmissao" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtAdmissao" PopupButtonID="imgAdmissao" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                </td>
                                <td class="tdPrincipal1" width="70px">
                                    Vigência
                                </td>
                                <td class="tdNormal1" width="63px">
                                    <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtVigencia" BackColor="lightgray"
                                        ReadOnly="false" Width="66px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                </td>
                                <td class="tdPrincipal1" width="80px">
                                    Vencimento
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtVencimento" BackColor="lightgray"
                                        ReadOnly="false" Width="66px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="p2" runat="server">
            <HeaderTemplate>
                <span class="subtitulo">Beneficiário Titular</span>
            </HeaderTemplate>
            <ContentTemplate>
                <br />
                <asp:UpdatePanel runat="server" ID="upBeneficiario" UpdateMode="Conditional">
                    <ContentTemplate>
                        <span class="subtitulo">Beneficiário Titular</span><br />
                        <asp:Panel runat="server" Visible="false" ID="pnlSelTitular" EnableViewState="false">
                            <asp:GridView ID="gridSelTitular" Width="65%" SkinID="gridViewSkin" EnableViewState="false"
                                runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="ID"
                                OnRowCommand="gridSelTitular_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="Nome" HeaderText="Nome">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:ButtonField Text="<img src='images/active.png' title='selecionar' alt='selecionar' border='0' />"
                                        CommandName="usar">
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                            <br />
                        </asp:Panel>
                        <table cellpadding="2" width="80%">
                            <tr>
                                <td width="110" class="tdPrincipal1">
                                    CPF
                                </td>
                                <td class="tdNormal1" width="154">
                                    <asp:TextBox runat="server" CssClass="textbox" ID="txtCPF" Width="112px" /><cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeCPF" Mask="999,999,999-99" ClearMaskOnLostFocus="true" TargetControlID="txtCPF" />
                                    <asp:ImageButton runat="server" ImageUrl="~/images/search.png" ToolTip="localizar..." ID="cmdCarregaBeneficiarioPorCPF" EnableViewState="true" OnClick="cmdCarregaBeneficiarioPorCPF_Click" />
                                </td>
                                <td width="110" class="tdPrincipal1">
                                    RG
                                </td>
                                <td class="tdNormal1">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td>
                                                <asp:TextBox runat="server" CssClass="textbox" ID="txtRG" Width="100px" />&nbsp;
                                                <asp:ImageButton runat="server" ImageUrl="~/images/search.png" ToolTip="localizar..."
                                                    ID="cmdCarregaBeneficiarioPorRG" EnableViewState="true" OnClick="cmdCarregaBeneficiarioPorRG_Click" />
                                            </td>
                                            <td align="right">
                                                <asp:ImageButton Visible="false" ImageUrl="~/images/change.gif" ToolTip="alterar beneficiário"
                                                    EnableViewState="true" runat="server" ID="cmdAlterarBeneficiarioTitular" />
                                                &nbsp;&nbsp;&nbsp;&nbsp;
                                                <asp:ImageButton EnableViewState="true" runat="server" ToolTip="novo beneficiário"
                                                    ID="cmdNovoTitular" ImageUrl="~/images/new.png" />&nbsp;&nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td width="110" class="tdPrincipal1">
                                    Data Nasc.
                                </td>
                                <td colspan="3" class="tdNormal1">
                                    <asp:TextBox runat="server" CssClass="textbox" ID="txtDataNascimento" Width="112px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                    <asp:Image SkinID="imgCanlendario" ID="imgDataNascimento" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataNascimento" TargetControlID="txtDataNascimento" PopupButtonID="imgDataNascimento" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                </td>
                            </tr>
                            <tr>
                                <td width="110" class="tdPrincipal1">
                                    Estado Civil
                                </td>
                                <td class="tdNormal1" width="154">
                                    <asp:DropDownList Width="112px" runat="server" SkinID="dropdownSkin" ID="cboEstadoCivil" />
                                </td>
                                <td width="110" class="tdPrincipal1">
                                    Data Casamento
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox ID="txtTitDataCasamento" Width="80" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                    <asp:Image SkinID="imgCanlendario" ID="imgTitDataCasamento" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceTitDataCasamento" TargetControlID="txtTitDataCasamento" PopupButtonID="imgTitDataCasamento" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                </td>
                            </tr>
                            <tr>
                                <td width="110" class="tdPrincipal1">
                                    Matrícula - Saúde
                                </td>
                                <td class="tdNormal1" width="154">
                                    <asp:TextBox ID="txtNumMatriculaSaude" Width="110" SkinID="textboxSkin" MaxLength="16"
                                        runat="server" />
                                </td>
                                <td width="110" class="tdPrincipal1">
                                    Matrícula - Dental
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox ID="txtNumMatriculaDental" Width="80" SkinID="textboxSkin" MaxLength="16"
                                        runat="server" />
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="2" width="80%">
                            <tr>
                                <td width="110" class="tdPrincipal1">
                                    Nome
                                </td>
                                <td colspan="3" class="tdNormal1">
                                    <asp:TextBox BackColor="lightgray" ReadOnly="true" runat="server" CssClass="textbox"
                                        ID="txtNome" Width="97%" />
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="2" width="80%">
                            <tr>
                                <td width="110" class="tdPrincipal1">
                                    Sexo
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList BackColor="LightGray" Enabled="false" Width="107px" runat="server"
                                        CssClass="textbox" ID="cboSexo">
                                        <asp:ListItem Text="MASCULINO" Value="1" Selected="True" />
                                        <asp:ListItem Text="FEMININO" Value="2" />
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="2" width="80%">
                            <tr>
                                <td width="110" class="tdPrincipal1">
                                    Nome da mãe
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox BackColor="lightgray" ReadOnly="true" runat="server" CssClass="textbox"
                                        ID="txtNomeMae" Width="97%" />
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="2" width="80%">
                            <td width="110" class="tdPrincipal1">
                                Peso
                            </td>
                            <td class="tdNormal1" width="56">
                                <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtTitPeso" Width="50" />
                            </td>
                            <td width="69" class="tdPrincipal1">
                                Altura
                            </td>
                            <td class="tdNormal1">
                                <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtTitAltura" Width="50" />
                            </td>
                        </table>

                        <br />
                        <span class="subtitulo">Compra de carência</span><br />
                        <table cellpadding="2" width="65%">
                            <tr>
                                <td width="110" class="tdPrincipal1">
                                    Operadora
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" ID="cboCarenciaOperadora" SkinID="textboxSkin" Width="300" />
                                    <input type="hidden" name="txtCarenciaOperadoraID" id="txtCarenciaOperadoraID" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Matrícula
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" ID="txtCarenciaMatricula" SkinID="textboxSkin" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Tempo de contrato
                                </td>
                                <td class="tdNormal1_NonBold">
                                    de <asp:TextBox ID="txtTitTempoContratoDe" Width="61" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                    a&nbsp;&nbsp;<asp:TextBox ID="txtTitTempoContratoAte" Width="61" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    <i><span class="subtitulo"><font size="1">(data do último pagto.)</font></span></i>
                                    <asp:TextBox BackColor="lightgray" ReadOnly="true" runat="server" ID="txtCarenciaTempoContrato" MaxLength="4" SkinID="textboxSkin" Width="20" Visible="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Código
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" ID="txtCarenciaCodigo" SkinID="textboxSkin" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Portabilidade
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" ID="txtPortabilidade" SkinID="textboxSkin" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <span class="subtitulo">Responsável Legal (<i>preencher somente quando o titular for menor de idade</i>)</span><br />
                        <table cellpadding="2" width="80%">
                            <tr>
                                <td width="89" class="tdPrincipal1">
                                    Nome
                                </td>
                                <td colspan="3" class="tdNormal1">
                                    <asp:TextBox runat="server" CssClass="textbox" ID="txtNomeResponsavel" Width="97%" />
                                </td>
                            </tr>
                            <tr>
                                <td width="89" class="tdPrincipal1">
                                    CPF
                                </td>
                                <td class="tdNormal1" width="114">
                                    <asp:TextBox runat="server" CssClass="textbox" ID="txtCPFResponsavel" Width="112px" />
                                    <cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeCPFResponsavel"
                                        Mask="999,999,999-99" TargetControlID="txtCPFResponsavel" />
                                </td>
                                <td width="89" class="tdPrincipal1">
                                    RG
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" CssClass="textbox" ID="txtRGResponsavel" Width="100px" />
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="2" width="80%">
                            <tr>
                                <td width="89" class="tdPrincipal1">
                                    Data Nasc.
                                </td>
                                <td class="tdNormal1" width="116">
                                    <asp:TextBox runat="server" CssClass="textbox" ID="txtDataNascimentoResponsavel" Width="70px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    <asp:Image SkinID="imgCanlendario" ID="imgDataNascimentoResponsavel" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataNascimentoResponsavel" TargetControlID="txtDataNascimentoResponsavel" PopupButtonID="imgDataNascimentoResponsavel" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                </td>
                                <td width="89" class="tdPrincipal1">
                                    Sexo
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList Width="107px" runat="server" CssClass="textbox" ID="cboSexoResponsavel">
                                        <asp:ListItem Text="MASCULINO" Value="1" Selected="True" />
                                        <asp:ListItem Text="FEMININO" Value="2" />
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td width="89" class="tdPrincipal1">
                                    Parentesco
                                </td>
                                <td class="tdNormal1" colspan="3">
                                    <asp:DropDownList Width="118px" runat="server" CssClass="textbox" ID="cboParentescoResponsavel" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="p3" runat="server">
            <HeaderTemplate>
                <span class="subtitulo">Dados cadastrais</span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel runat="server" ID="upDadosCadastrais" UpdateMode="Conditional">
                    <ContentTemplate>
                        <br />
                        <span runat="server" id="spanEnderecosDisponiveis_Titular" class="subtitulo">Endereços
                            do titular</span><br />
                        <asp:GridView ID="gridEnderecosDisponiveis_Titular" runat="server" SkinID="gridViewSkin"
                            DataKeyNames="ID" AutoGenerateColumns="False" Width="650" OnRowDataBound="gridEnderecosDisponiveis_Titular_OnRowDataBound"
                            OnRowCommand="gridEnderecosDisponiveis_Titular_RowCommand">
                            <Columns>
                                <asp:BoundField ItemStyle-Wrap="false" DataField="Logradouro" HeaderText="Endereço">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="97%" />
                                </asp:BoundField>
                                <asp:ButtonField ButtonType="Image" ImageUrl="~/images/search.png" CommandName="usar">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Button" CommandName="referencia" Text="referência" HeaderText="Utilizar para">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                    <ControlStyle Width="80" BorderWidth="0" BackColor="#507CD1" ForeColor="White" />
                                </asp:ButtonField>
                                <asp:ButtonField ButtonType="Button" CommandName="cobranca" Text="cobrança">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                    <ControlStyle Width="80" BorderWidth="0" BackColor="#507CD1" ForeColor="White" />
                                </asp:ButtonField>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <span class="subtitulo">Endereços cadastrados para a proposta</span><br />
                        <asp:GridView ID="gridEnderecosSelecionados" runat="server" SkinID="gridViewSkin"
                            DataKeyNames="ID" AutoGenerateColumns="False" Width="650" OnRowDataBound="gridEnderecosSelecionados_OnRowDataBound"
                            OnRowCommand="gridEnderecosSelecionados_RowCommand">
                            <Columns>
                                <asp:BoundField ItemStyle-Wrap="false" DataField="Logradouro" HeaderText="Endereço">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Logradouro" HeaderText="Tipo">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                </asp:BoundField>
                                <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />"
                                    CommandName="excluir">
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <span class="subtitulo">Endereço (visualização)</span><br />
                        <table width="650" cellpadding="2">
                            <tr>
                                <td class="tdPrincipal1" width="13%">
                                    CEP
                                </td>
                                <td class="tdNormal1" colspan="3">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtCEP" Width="65px" MaxLength="9"
                                        BackColor="lightgray" ReadOnly="true" /><cc1:MaskedEditExtender runat="server" EnableViewState="false"
                                            ID="meeCEP" Mask="99999-999" TargetControlID="txtCEP" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Logradouro
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtLogradouro" Width="290px" MaxLength="300"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                                <td class="tdPrincipal1">
                                    &nbsp;Número
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtNumero" Width="65px" MaxLength="9"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Complemento
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtComplemento" Width="200px"
                                        MaxLength="250" BackColor="lightgray" ReadOnly="true" />
                                </td>
                                <td class="tdPrincipal1" width="10%">
                                    &nbsp;Bairro
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtBairro" Width="190px" MaxLength="300"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                        <table width="650" cellpadding="2">
                            <tr>
                                <td class="tdPrincipal1" width="13%">
                                    Cidade
                                </td>
                                <td class="tdNormal1" width="293">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtCidade" Width="200px" MaxLength="300"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                                <td class="tdPrincipal1" width="10%">
                                    UF
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtUF" Width="20px" MaxLength="2"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                        <table width="650" cellpadding="2">
                            <tr>
                                <td class="tdPrincipal1" width="13%">
                                    Tipo
                                </td>
                                <td class="tdNormal1" width="573">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:DropDownList BackColor="LightGray" Enabled="false" Width="206px" runat="server"
                                                    ID="cboTipoEndereco" CssClass="textbox">
                                                    <asp:ListItem Text="RESIDENCIAL" Value="0" Selected="True" />
                                                    <asp:ListItem Text="COMERCIAL" Value="1" />
                                                </asp:DropDownList>
                                            </td>
                                            <td align="right">
                                                <asp:Button ID="cmdEnderecoAcoes" Visible="false" runat="server" SkinID="botaoPequeno"
                                                    Text="Gravar" OnClick="cmdEnderecoAcoes_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <span class="subtitulo">Contato</span>
                        <br />
                        <table cellpadding="2">
                            <tr>
                                <td class="tdPrincipal1" width="50">
                                    DDD
                                </td>
                                <td class="tdNormal1" width="50">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtDDD1" Width="30px" MaxLength="3"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                                <td class="tdPrincipal1" width="50">
                                    Fone
                                </td>
                                <td class="tdNormal1" width="80">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtFone1" Width="62px" MaxLength="9"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                                <td class="tdPrincipal1" width="50">
                                    Ramal
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtRamal1" Width="40px" MaxLength="4"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    DDD
                                </td>
                                <td class="tdNormal1" width="50">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtDDD2" Width="30px" MaxLength="3"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                                <td class="tdPrincipal1">
                                    Fone
                                </td>
                                <td class="tdNormal1" width="80">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtFone2" Width="62px" MaxLength="9"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                                <td class="tdPrincipal1">
                                    Ramal
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtRamal2" Width="40px" MaxLength="4"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    DDD
                                </td>
                                <td class="tdNormal1" width="50">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtDDD3" Width="30px" MaxLength="3"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                                <td class="tdPrincipal1">
                                    Celular
                                </td>
                                <td class="tdNormal1" width="80">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtFone3" Width="62px" MaxLength="9"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                                <td>
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="2">
                            <tr>
                                <td class="tdPrincipal1" width="50">
                                    E-mail
                                </td>
                                <td class="tdNormal1" width="300">
                                    <asp:TextBox CssClass="textbox" Width="295px" runat="server" ID="txtEmail" MaxLength="65"
                                        BackColor="lightgray" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="p4" runat="server" Visible="true" EnableViewState="true">
            <HeaderTemplate>
                <span class="subtitulo">Dependentes</span>
            </HeaderTemplate>
            <ContentTemplate>
                <br />
                <span class="subtitulo">Beneficiário Dependente</span><br />
                <asp:UpdatePanel runat="server" ID="upDependente" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel runat="server" Visible="false" ID="pnlSelDependente" EnableViewState="false">
                            <asp:GridView PageSize="150" ID="gridSelDependente" Width="65%" SkinID="gridViewSkin"
                                EnableViewState="false" runat="server" AllowPaging="false" AutoGenerateColumns="False"
                                DataKeyNames="ID" OnRowCommand="gridSelDependente_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="Nome" HeaderText="Nome">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:ButtonField Text="<img src='images/active.png' title='selecionar' alt='selecionar' border='0' />"
                                        CommandName="usar">
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                            <br />
                        </asp:Panel>
                        <table cellpadding="2" width="69%">
                            <tr>
                                <td width="120" class="tdPrincipal1">
                                    CPF
                                </td>
                                <td class="tdNormal1" width="143">
                                    <asp:TextBox runat="server" CssClass="textbox" ID="txtCPFDependente" Width="112px" />
                                    <cc1:MaskedEditExtender runat="server" EnableViewState="False" ID="meeCPFDependente"
                                        Mask="999,999,999-99" TargetControlID="txtCPFDependente" />
                                    &nbsp;
                                    <asp:ImageButton runat="server" ImageUrl="~/images/search.png" ToolTip="localizar..."
                                        ID="cmdCarregaBeneficiarioDependentePorCPF" EnableViewState="False" OnClick="cmdCarregaBeneficiarioDependentePorCPF_Click" />
                                </td>
                                <td width="119" class="tdPrincipal1">
                                    Nome
                                </td>
                                <td class="tdNormal1">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td>
                                                <asp:TextBox runat="server" CssClass="textbox" ID="txtRGDependente" Width="117px" />
                                                <asp:ImageButton runat="server" ImageUrl="~/images/search.png" ToolTip="localizar..."
                                                    ID="cmdCarregaBeneficiarioDependentePorRG" EnableViewState="False" OnClick="cmdCarregaBeneficiarioDependentePorRG_Click" />
                                            </td>
                                            <td align="right">
                                                <asp:ImageButton Visible="false" ImageUrl="~/images/new.png" ToolTip="alterar beneficiário"
                                                    EnableViewState="False" runat="server" ID="cmdAlterarBeneficiarioDependente"
                                                    OnClick="cmdAlterarBeneficiarioDependente_Click" />
                                                <asp:ImageButton ImageUrl="~/images/new.png" ToolTip="novo beneficiário" EnableViewState="true"
                                                    runat="server" ID="cmdNovoBeneficiario" />&nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="2" width="69%">
                            <tr>
                                <td class="tdPrincipal1" width="120px">
                                    Data Nasc.
                                </td>
                                <td class="tdNormal1" width='143px'>
                                    <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDataNascimentoDependente" Width="112px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    <asp:Image SkinID="imgCanlendario" ID="imgDataNascimentoDependente" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataNascimentoDependente" TargetControlID="txtDataNascimentoDependente" PopupButtonID="imgDataNascimentoDependente" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                </td>
                                <td class="tdPrincipal1" width='119px'>
                                    Parentesco
                                </td>
                                <td class="tdNormal1" colspan="4">
                                    <asp:DropDownList Width="118px" runat="server" CssClass="textbox" ID="cboParentescoDependente" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Estado Civil
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList Width="118px" runat="server" CssClass="textbox" ID="cboEstadoCivilDependente" />
                                </td>
                                <td class="tdPrincipal1">
                                    Data Casamento
                                </td>
                                <td class="tdNormal1" colspan="4">
                                    <asp:TextBox ID="txtDepDataCasamento" Width="80" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    <asp:Image SkinID="imgCanlendario" ID="imgDepDataCasamento" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDepDataCasamento" TargetControlID="txtDepDataCasamento" PopupButtonID="imgDepDataCasamento" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Matrícula - Saúde
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox ID="txtNumMatriculaSaudeDep" Width="118px" SkinID="textboxSkin" MaxLength="16"
                                        runat="server" />
                                </td>
                                <td class="tdPrincipal1">
                                    Matrícula - Dental
                                </td>
                                <td class="tdNormal1" colspan="4">
                                    <asp:TextBox ID="txtNumMatriculaDentalDep" Width="80" SkinID="textboxSkin" MaxLength="16"
                                        runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Nome
                                </td>
                                <td colspan="6" class="tdNormal1">
                                    <asp:TextBox BackColor="lightgray" ReadOnly="true" runat="server" SkinID="textboxSkin"
                                        ID="txtNomeDependente" Width="97%" />
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="2" width="69%">
                            <tr>
                                <td width="120" class="tdPrincipal1">
                                    Sexo
                                </td>
                                <td class="tdNormal1">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:DropDownList BackColor="lightgray" Width="107px" runat="server" CssClass="textbox" ID="cboSexoDependente" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table cellpadding="2" width="69%">
                            <tr>
                                <td width="120" class="tdPrincipal1">
                                    Peso
                                </td>
                                <td class="tdNormal1" width="56">
                                    <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDepPeso" Width="50" />
                                </td>
                                <td width="69" class="tdPrincipal1">
                                    Altura
                                </td>
                                <td width="60" class="tdNormal1">
                                    <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDepAltura" Width="50" />
                                </td>
                                <td width="69" class="tdPrincipal1">
                                    Admissão
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox EnableViewState="false" runat="server" SkinID="textboxSkin" ID="txtDepAdmissao" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    <asp:Image SkinID="imgCanlendario" ID="imgDepAdmissao" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDepAdmissao" TargetControlID="txtDepAdmissao" PopupButtonID="imgDepAdmissao" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <span class="subtitulo">Compra de carência</span><br />
                        <table cellpadding="2" width="69%">
                            <tr>
                                <td width="120" class="tdPrincipal1">
                                    Operadora
                                </td>
                                <td colspan="2" class="tdNormal1">
                                    <asp:TextBox runat="server" ID="cboCarenciaDependenteOperadora" SkinID="textboxSkin" Width="300" />
                                    <input type="hidden" name="txtCarenciaDependenteOperadoraID" id="txtCarenciaDependenteOperadoraID" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Matrícula
                                </td>
                                <td colspan="2" class="tdNormal1">
                                    <asp:TextBox runat="server" ID="txtCarenciaDependenteMatricula" SkinID="textboxSkin" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Tempo de contrato
                                </td>
                                <td colspan="2" class="tdNormal1_NonBold">
                                    de <asp:TextBox ID="txtDepTempoContratoDe" Width="61" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    <asp:Image SkinID="imgCanlendario" ID="imgDepTempoContratoDe" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDepTempoContratoDe" TargetControlID="txtDepTempoContratoDe" PopupButtonID="imgDepTempoContratoDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                    &nbsp;&nbsp;a&nbsp;&nbsp;<asp:TextBox ID="txtDepTempoContratoAte" Width="61" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    <asp:Image SkinID="imgCanlendario" ID="imgDepTempoContratoAte" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDepTempoContratoAte" TargetControlID="txtDepTempoContratoAte" PopupButtonID="imgDepTempoContratoAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                    <i><span class="subtitulo"><font size="1">(data do último pagto.)</font></span></i>
                                    <asp:TextBox runat="server" ID="txtCarenciaDependenteTempoContrato" MaxLength="4" SkinID="textboxSkin" Width="20" Visible="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Código
                                </td>
                                <td colspan="2" class="tdNormal1">
                                    <asp:TextBox runat="server" ID="txtCarenciaDependenteCodigo" SkinID="textboxSkin" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Portabilidade
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" ID="txtDependentePortabilidade" SkinID="textboxSkin" />
                                </td>
                                <td class="tdNormal1" align="right" width="1%">
                                    <asp:ImageButton ImageUrl="~/images/add.gif" EnableViewState="true" runat="server" ID="cmdAddDependente" ToolTip="adicionar ao contrato" OnClick="cmdAddDependente_Click" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <span runat="server" id="spanDependentesCadastrados" class="subtitulo">Dependentes cadastrados</span>
                        <br />
                        <asp:GridView Width="69%" ID="gridDependentes" runat="server" AutoGenerateColumns="False"
                            DataKeyNames="ID,BeneficiarioID" SkinID="gridViewSkin" OnRowCommand="gridDependentes_RowCommand"
                            OnRowDataBound="gridDependentes_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="NumeroSequencial" HeaderText="Núm." ItemStyle-Width="1%">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="BeneficiarioNome" HeaderText="Nome">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ParentescoDescricao" HeaderText="Parentesco">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="EstadoCivilDescricao" HeaderText="EstadoCivil">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Tipo" HeaderText="Tipo">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Data" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Admissão">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' title='remover do contrato' alt='remover do contrato' border='0' />">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle Width="1%" ForeColor="#CC0000" />
                                </asp:ButtonField>
                                <asp:ButtonField CommandName="editar" ButtonType="Link" Text="<img src='images/edit.png' title='editar cadastro' alt='editar cadastro' border='0' />">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                                <asp:ButtonField CommandName="editarDadosDaProposta" ButtonType="Link" Text="<img src='images/detail2.png' title='editar' alt='editar' border='0' />">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="p5" runat="server">
            <HeaderTemplate>
                <span class="subtitulo">Ficha de Saúde</span>
            </HeaderTemplate>
            <ContentTemplate>
                <br />
                <asp:UpdatePanel runat="server" ID="upFichaSaude" UpdateMode="Always">
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="0" style="border: solid 1px #507CD1" width="70%">
                            <tr>
                                <td class="tdPrincipal1" width="90">
                                    Beneficiário
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList Width="300px" ID="cboBeneficiarioFicha" runat="server" SkinID="dropdownSkin"
                                        AutoPostBack="True" OnSelectedIndexChanged="cboBeneficiarioFicha_SelectedIndexChanged" />
                                    &nbsp;
                                    <asp:ImageButton Visible="false" ID="cmdCarregarComboFichaSaudeBeneficiarios" ImageUrl="~/images/refresh.png"
                                        ToolTip="atualizar lista" runat="server" OnClick="cmdCarregarComboFichaSaudeBeneficiarios_Click" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <span class="subtitulo">Ficha de saúde</span>
                        <asp:DataList CellPadding="0" CellSpacing="0" ID="dlFicha" DataKeyField="ID" runat="server"
                            OnItemCommand="dlFicha_ItemCommand" OnItemDataBound="dlFicha_ItemDataBound">
                            <HeaderTemplate>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <br />
                                <table cellpadding="3" cellspacing="0" width="600">
                                    <tr>
                                        <td colspan="2" bgcolor='#EFF3FB' style="border-left: solid 1px #507CD1; border-top: solid 1px #507CD1; border-bottom: solid 1px #507CD1" align="left">
                                            <asp:Label ID="lblQuesta" Text='<%# DataBinder.Eval(Container.DataItem, "ItemDeclaracaoTexto") %>' runat="server" />
                                            <asp:Literal ID="litItemDeclaracaoID" Text='<%# DataBinder.Eval(Container.DataItem, "ItemDeclaracaoID") %>' runat="server" Visible="false" />
                                        </td>
                                        <td bgcolor='#EFF3FB' style="border-top: solid 1px #507CD1; border-bottom: solid 1px #507CD1; border-right: solid 1px #507CD1" align="center" width="1%">
                                            <asp:CheckBox OnCheckedChanged="checkboxSkin_Changed2" AutoPostBack="true" SkinID="checkboxSkin" ID="chkFSim" runat="server" Checked='<%# Bind("Sim") %>' />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="tr1Ficha" visible="false">
                                        <td style="border-left: solid 1px #507CD1">
                                            Data
                                        </td>
                                        <td colspan="2" style="border-right: solid 1px #507CD1">
                                            Descrição
                                        </td>
                                    </tr>
                                    <tr runat="server" id="tr2Ficha" visible="false">
                                        <td style="border-left: solid 1px #507CD1; border-bottom: solid 1px #507CD1">
                                            <asp:TextBox SkinID="textboxSkin" Width="66px" runat="server" ID="txtFichaSaudeData" Text='<%# DataBinder.Eval(Container.DataItem, "strData") %>' onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                        </td>
                                        <td width="90%" colspan="2" style="border-bottom: solid 1px #507CD1; border-right: solid 1px #507CD1">
                                            <asp:TextBox ID="txtFichaSaudeDescricao" Width="99%" SkinID="textboxSkin" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Descricao") %>' />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="tr4Ficha" visible="false" >
                                        <td style="border-left: solid 1px #507CD1" width="5%">
                                            CID
                                        </td>
                                        <td style="" width="10%">
                                            Observações
                                        </td>
                                        <td style="border-right: solid 1px #507CD1">
                                            Aprovado?
                                        </td>
                                    </tr>
                                    <tr runat="server" id="tr5Ficha" visible="false">
                                        <td style="border-left: solid 1px #507CD1;border-bottom: solid 1px #507CD1" valign="top">
                                            Inicial 
                                            <asp:TextBox Width="30" MaxLength="4" SkinID="textboxSkin" ID="txtCIDInicial" runat="server" Text='<%# Bind("CIDInicial") %>' />
                                            &nbsp;
                                            Final 
                                            <asp:TextBox Width="30" MaxLength="4" SkinID="textboxSkin" ID="txtCIDFinal" runat="server" Text='<%# Bind("CIDFinal") %>' />
                                        </td>
                                        <td style="border-bottom: solid 1px #507CD1" valign="top">
                                            <asp:TextBox Width="99%" MaxLength="500" TextMode="MultiLine" Height="50" SkinID="textboxSkin" ID="txtOBSMedico" runat="server" Text='<%# Bind("OBSMedico") %>' />
                                        </td>
                                        <td style="border-right: solid 1px #507CD1;border-bottom: solid 1px #507CD1" valign="top">
                                            <asp:CheckBox ID="chkAprovado" runat="server" Checked='<%# Bind("AprovadoPeloMedico") %>' />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="tr3Ficha" visible="false">
                                        <td colspan="3" style="border-left: solid 1px #507CD1; border-bottom: solid 1px #507CD1;border-right: solid 1px #507CD1" align="center">
                                            <asp:Button ID="cmdSalvarFicha" SkinID="botaoPequeno" Text="salvar" runat="server" CommandName="salvar" CommandArgument="<%# Container.ItemIndex %>" />
                                            <asp:Literal runat="server" EnableViewState="false" ID="litFichaAviso" />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:DataList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="p5a" runat="server">
            <HeaderTemplate>
                <span class="subtitulo">Adicionais</span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel runat="server" ID="upAdicionais" UpdateMode="Conditional">
                    <ContentTemplate>
                        <br />
                        <table cellpadding="4" cellspacing="0" width="100%" cellspacing="0" style="border:1px solid #507CD1">
                            <tr>
                                <td class="tdPrincipal1" width="90">
                                    Adicionais
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList Width="500px" ID="cboAdicionaisParaAdicionar" runat="server" SkinID="dropdownSkin"/>
                                </td>
                                <td class="tdNormal1">
                                    <asp:LinkButton ID="lnkAdicionarAdicional" Text="adicionar" OnClick="lnkAdicionarAdicional_click" runat="server" OnClientClick="return confirm('Deseja realmente acrescentar um adicional ao beneficiário?');" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <span class="subtitulo"><b>Taxas</b></span><br />
                        <asp:GridView ID="gridAdicional" runat="server" SkinID="gridViewSkin" DataKeyNames="AdicionalID,ID"
                            AutoGenerateColumns="False" Width="100%" OnRowDataBound="gridAdicional_OnRowDataBound"
                            OnRowCommand="gridAdicional_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="AdicionalDescricao" ItemStyle-Width="33%" HeaderText="Produto">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Valor R$" HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:TextBox SkinID="textboxSkin" ID="txtValorAdicional" onkeypress="filtro_SoNumeros(event);" runat="server" Text='<%# Bind("Valor01") %>' MaxLength="10" Width="85px" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" Width="33%" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Forma Pagto." HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:DropDownList SkinID="dropdownSkin" ID="cboFormaPagtoAdicional" runat="server" SelectedValue='<%# Bind("FormaPagto") %>' Width="100%">
                                            <asp:ListItem Value="00" Text="Nenhum" />
                                            <asp:ListItem Value="31" Text="Boleto" />
                                            <asp:ListItem Value="09" Text="Crédito" />
                                            <asp:ListItem Value="10" Text="Débito" />
                                            <asp:ListItem Value="81" Text="Desconto em conta" />
                                            <asp:ListItem Value="11" Text="Desconto em folha" />
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" Width="26%" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Recorrente" HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:CheckBox SkinID="checkboxSkin" ID="chkRecorrente" runat="server" Checked='<%# Bind("Recorrente") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="15%" />
                                </asp:TemplateField>

                                <asp:ButtonField Text="<img src='images/saveEditGrid.png' title='gravar' alt='gravar' border='0' />" CommandName="gravar">
                                    <ItemStyle Width="1%" HorizontalAlign="Center" />
                                    <ControlStyle Font-Size="10px" ForeColor="#cc0000" />
                                </asp:ButtonField>
                                <asp:ButtonField Text="<img src='images/delete.png' title='cancelar' alt='cancelar' border='0' />" CommandName="excluir">
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>                               
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:CheckBox OnCheckedChanged="checkboxGridAdicional_Changed" AutoPostBack="true" SkinID="checkboxSkin" ID="chkSimAd" runat="server" Checked='<%# Bind("Sim") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="1%" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <span class="subtitulo"><b>Seguros</b></span><br />
                        <asp:GridView ID="gridSeguro" runat="server" SkinID="gridViewSkin" DataKeyNames="AdicionalID,ID"
                            AutoGenerateColumns="False" Width="100%" OnRowDataBound="gridSeguro_OnRowDataBound"
                            OnRowCommand="gridSeguro_RowCommand">
                            <Columns>
                                <asp:ButtonField ButtonType="Link" DataTextField="AdicionalDescricao" ItemStyle-Width="33%" HeaderText="Produto" CommandName="detalheSeguro">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Wrap="False" />
                                    <ControlStyle ForeColor="#507CD1" Font-Bold="true" />
                                </asp:ButtonField>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Prêmio 1">
                                    <%--<HeaderTemplate>
                                        <asp:LinkButton ForeColor="White" ID="lnkPremio1" Text="Prêmio 1" OnClick="lnkPremio1_click" runat="server" />
                                    </HeaderTemplate>--%>
                                    <ItemTemplate>
                                        <asp:TextBox SkinID="textboxSkin" ID="txtPremio1" runat="server" Text='<%# Bind("PRE_COB_1") %>' MaxLength="10" Width="65px" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Prêmio 2" HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:TextBox SkinID="textboxSkin" ID="txtPremio2" runat="server" Text='<%# Bind("PRE_COB_2") %>' MaxLength="10" Width="65px" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Prêmio 3" HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:TextBox SkinID="textboxSkin" ID="txtPremio3" runat="server" Text='<%# Bind("PRE_COB_3") %>' MaxLength="10" Width="65px" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Prêmio 4" HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:TextBox SkinID="textboxSkin" ID="txtPremio4" runat="server" Text='<%# Bind("PRE_COB_4") %>' MaxLength="10" Width="65px" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Prêmio 5" HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:TextBox SkinID="textboxSkin" ID="txtPremio5" runat="server" Text='<%# Bind("PRE_COB_5") %>' MaxLength="10" Width="65px" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Prêmio 6" HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:TextBox SkinID="textboxSkin" ID="txtPremio6" runat="server" Text='<%# Bind("PRE_COB_6") %>' MaxLength="10" Width="65px" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Forma Pagto." HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:DropDownList SkinID="dropdownSkin" ID="cboFormaPagtoAdicional" runat="server" SelectedValue='<%# Bind("FormaPagto") %>' Width="100%">
                                            <asp:ListItem Value="00" Text="Nenhum" />
                                            <asp:ListItem Value="31" Text="Boleto" />
                                            <asp:ListItem Value="09" Text="Crédito" />
                                            <asp:ListItem Value="10" Text="Débito" />
                                            <asp:ListItem Value="81" Text="Desconto em conta" />
                                            <asp:ListItem Value="11" Text="Desconto em folha" />
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" Width="33%" />
                                </asp:TemplateField>
                                <%--<asp:ButtonField Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>--%>

                                <asp:TemplateField HeaderText="Recorrente" HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:CheckBox SkinID="checkboxSkin" ID="chkRecorrente" runat="server" Checked='<%# Bind("Recorrente") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="15%" />
                                </asp:TemplateField>

                                <asp:ButtonField Text="<img src='images/saveEditGrid.png' title='gravar' alt='gravar' border='0' />" CommandName="gravar">
                                    <ItemStyle Width="1%" HorizontalAlign="Center" />
                                    <ControlStyle Font-Size="10px" ForeColor="#cc0000" />
                                </asp:ButtonField>
                                <asp:ButtonField Text="<img src='images/delete.png' title='cancelar' alt='cancelar' border='0' />" CommandName="excluir">
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>  
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:CheckBox OnCheckedChanged="checkboxGridAdicional_Changed" AutoPostBack="true" SkinID="checkboxSkin" ID="chkSimAd" runat="server" Checked='<%# Bind("Sim") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="1%" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <span class="subtitulo"><b>Previdências</b></span><br />
                        <asp:GridView ID="gridPrevidencia" runat="server" SkinID="gridViewSkin" DataKeyNames="AdicionalID,ID"
                            AutoGenerateColumns="False" Width="100%" OnRowDataBound="gridPrevidencia_OnRowDataBound"
                            OnRowCommand="gridPrevidencia_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="AdicionalDescricao" ItemStyle-Width="33%" HeaderText="Produto">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Wrap="False" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Valor R$" HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:TextBox SkinID="textboxSkin" ID="txtValorAdicional" runat="server" Text='<%# Bind("Valor01") %>' MaxLength="10" Width="85px" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" Width="33%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Forma Pagto." HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:DropDownList SkinID="dropdownSkin" ID="cboFormaPagtoAdicional" runat="server" SelectedValue='<%# Bind("FormaPagto") %>' Width="100%">
                                            <asp:ListItem Value="00" Text="Nenhum" />
                                            <asp:ListItem Value="31" Text="Boleto" />
                                            <asp:ListItem Value="09" Text="Crédito" />
                                            <asp:ListItem Value="10" Text="Débito" />
                                            <asp:ListItem Value="81" Text="Desconto em conta" />
                                            <asp:ListItem Value="11" Text="Desconto em folha" />
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" Width="33%" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Recorrente" HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:CheckBox SkinID="checkboxSkin" ID="chkRecorrente" runat="server" Checked='<%# Bind("Recorrente") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="15%" />
                                </asp:TemplateField>

                                <asp:ButtonField Text="<img src='images/saveEditGrid.png' title='gravar' alt='gravar' border='0' />" CommandName="gravar">
                                    <ItemStyle Width="1%" HorizontalAlign="Center" />
                                    <ControlStyle Font-Size="10px" ForeColor="#cc0000" />
                                </asp:ButtonField>
                                <asp:ButtonField Text="<img src='images/delete.png' title='cancelar' alt='cancelar' border='0' />" CommandName="excluir">
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>  
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:CheckBox OnCheckedChanged="checkboxGridAdicional_Changed" AutoPostBack="true" SkinID="checkboxSkin" ID="chkSimAd" runat="server" Checked='<%# Bind("Sim") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="1%" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>

        <cc1:TabPanel ID="p6" runat="server">
            <HeaderTemplate>
                <span class="subtitulo">Finalização</span>
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel ID="upFinalizacao" UpdateMode="Conditional" runat="server" >
                    <ContentTemplate>
                        <br />
                        <table width="100%" border='0'>
                            <tr>
                                <td valign="top" width="50%">
                                    <table cellpadding="2" width="100%">
                                        <tr>
                                            <td class="tdPrincipal1" width="90px">
                                                Data digitação
                                            </td>
                                            <td class="tdNormal1">
                                                <asp:TextBox runat="server" BackColor="lightgray" ReadOnly="true" CssClass="textbox"
                                                    ID="txtDataContrato" Width="66" />
                                            </td>
                                        </tr>
                                        <tr id="trDesconto" runat="server">
                                            <td class="tdPrincipal1" width="90px">
                                                Desconto R$
                                            </td>
                                            <td class="tdNormal1">
                                                <asp:TextBox runat="server" BackColor="lightgray" ReadOnly="false" SkinID="textboxSkin"
                                                    ID="txtDesconto" Width="66" AutoPostBack="true" OnTextChanged="txtDesconto_Changed" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="tdPrincipal1" width="90px">
                                                Valor
                                            </td>
                                            <td class="tdNormal1">
                                                <asp:TextBox runat="server" BackColor="lightgray" ReadOnly="false" SkinID="textboxSkin"
                                                    ID="txtValorTotal" Width="66" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="tdPrincipal1">
                                                Novo dia vencto.
                                            </td>
                                            <td class="tdNormal1">
                                                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtVencimentoAlterado" BackColor="lightgray"
                                                    ReadOnly="false" Width="66px" onkeypress="filtro_SoNumeros(event);" MaxLength="2"/>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                    <table cellpadding="2" width="100%" runat="server" enableviewstate="false" visible="false">
                                        <tr>
                                            <td class="tdPrincipal1">
                                                <asp:CheckBox ID="chkCobrarTaxa" Text="Cobrar taxa associativa" SkinID="checkboxSkin2" runat="server" AutoPostBack="true" OnCheckedChanged="chkCobrarTaxa_CheckedChanged" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table align="center" cellpadding="2" cellspacing="0" width="300" runat="server"
                                        id="tblMsgRegras" visible="false">
                                        <tr>
                                            <td colspan="2">
                                                <table width="100%" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td align="center">
                                                            <b><font color='red'>A seguintes regras foram quebradas</font></b>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="tdNormal1">
                                                            <asp:Literal runat="server" ID="litMsgErroRegra" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" bgcolor="whitesmoke" style="border: solid 1px black" align="center">
                                                <b><font color='blue'>Liberação</font></b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="50" style="border-left: solid 1px black">
                                                <b>Usuário</b>
                                            </td>
                                            <td width="250" style="border-right: solid 1px black">
                                                <asp:TextBox Width="95%" TextMode="Password" MaxLength="75" runat="server" SkinID="textboxSkin"
                                                    ID="txtLogin" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="border-left: solid 1px black">
                                                <b>Senha</b>
                                            </td>
                                            <td style="border-right: solid 1px black">
                                                <asp:TextBox Width="95%" TextMode="Password" MaxLength="40" runat="server" SkinID="textboxSkin"
                                                    ID="txtSenha" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="border-left: solid 1px black; border-bottom: solid 1px black;
                                                border-right: solid 1px black" align="center">
                                                <asp:Button OnClick="cmdLiberar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false"
                                                    ID="cmdLiberar" Text="Liberar" />
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                    <asp:Literal ID="litSumario" runat="server" />
                                    <br />
                                    <table cellpadding="2" cellspacing="0" width="100%">
                                        <tr>
                                            <td class="tdPrincipal1" valign="top">
                                                Observações
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:TextBox Height="55" ID="txtObsEdit" runat="server" SkinID="textboxSkin" Width="99%" TextMode="MultiLine" />
                                                <asp:TextBox Height="55" ID="txtObs" runat="server" SkinID="textboxSkin" Width="99%" TextMode="MultiLine" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td valign="top" width="50%">
                                    <table cellpadding="2" width="100%">
                                        <tr>
                                            <td class="tdPrincipal1" align="center" colspan="2" height="20">
                                                Status da proposta
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="90px" class="tdPrincipal1">
                                                Status
                                            </td>
                                            <td class="tdNormal1">
                                                <asp:RadioButton ID="optNormal" onclick='return false;' Checked="true" Text="Normal" GroupName="status" runat="server" />
                                                <asp:RadioButton ID="optInativo" onclick='return false;' Text="Inativo" GroupName="status" runat="server" />
                                                <asp:RadioButton ID="optCancelado" onclick='return false;' Text="Cancelado" GroupName="status" runat="server" />
                                                &nbsp;
                                                <asp:LinkButton ID="lnkAlterarStatus" Text="alterar" runat="server" OnClick="lnkAlterarStatus_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                    <table cellpadding="0" width="100%" border='0'>
                                        <tr>
                                            <td class="tdPrincipal1" align="center" height="20">
                                                Histórico de mudança de status da proposta
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:GridView ID="gridHistoricoStatus" runat="server" SkinID="gridViewSkin" 
                                                    DataKeyNames="ID,StatusID" AutoGenerateColumns="False" Width="430px">
                                                    <Columns>
                                                        <asp:BoundField ItemStyle-Wrap="false" DataField="StatusDescricao" HeaderText="Motivo" Visible="true">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField ItemStyle-Wrap="false" DataField="StatusTipoTRADUZIDO" HeaderText="Tipo">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField ItemStyle-Wrap="false" DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle Width="1%" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>

        <cc1:TabPanel ID="p7Atendimento" runat="server" Visible="false">
            <HeaderTemplate>
                <asp:Literal ID="litAtendimentoHeader" runat="server" Text="<span class='subtitulo'>Atendimento</span>" />
            </HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel runat="server" ID="upAtendimento" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="pnlAtendimento" runat="server" Visible="false">
                            <table>
                                <tr>
                                    <td id="tdAtendimento" runat="server">
                                        <table cellpadding="0" cellspacing="1">
                                            <tr>
                                                <td class='tdPrincipal1' height="28">
                                                    &nbsp;Histórico de Atendimento
                                                </td>
                                                <td class='tdPrincipal1' height="28">
                                                    &nbsp;Detalhe do Atendimento
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top" width="420">
                                                    <asp:GridView ID="gridAtendimento" runat="server" SkinID="gridViewSkin" DataKeyNames="ID"
                                                        AutoGenerateColumns="False" Width="420" PageSize="10" OnRowDataBound="gridAtendimento_RowDataBound"
                                                        OnRowCommand="gridAtendimento_RowCommand" OnPageIndexChanging="gridAtendimento_PageIndexChanging">
                                                        <Columns>
                                                            <asp:BoundField ItemStyle-Wrap="false" DataField="ID" HeaderText="Protocolo" Visible="true">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField ItemStyle-Wrap="false" DataField="TituloOuCategoria" HeaderText="Atendimento">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField ItemStyle-Wrap="false" DataField="DataInicio" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ItemStyle Width="1%" />
                                                            </asp:BoundField>
                                                            <%--<asp:ButtonField ButtonType="Image" ImageUrl="~/images/search.png" Text="ver" CommandName="detalhe">--%>
                                                            <asp:ButtonField ButtonType="Link" Text="<img border='0' alt='ver' title='ver' src='images/search.png' />" CommandName="detalhe">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                <ItemStyle Width="1%" />
                                                            </asp:ButtonField>
                                                        </Columns>
                                                    </asp:GridView>
                                                    <asp:Button Visible="false" ID="cmdNovoAtendimento" Font-Size="10px" Text="Novo"
                                                        SkinID="botaoAzulBorda" runat="server" />
                                                </td>
                                                <td valign="top">
                                                    <table style="border: solid 1px #507CD1" width="100%">
                                                        <tr id="trAssunto" runat="server" visible="false" enableviewstate="false">
                                                            <td>
                                                                Assunto
                                                            </td>
                                                            <td>
                                                                <asp:TextBox Width="200" ID="txtTitulo" runat="server" SkinID="textboxSkin" MaxLength="250" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Protocolo</td>
                                                            <td><asp:Label ID="lblAtendimentoProtocolo" runat="server" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="top">
                                                                Texto
                                                            </td>
                                                            <td>
                                                                <asp:TextBox Width="345" Height="78" ID="txtTexto"  TextMode="MultiLine" runat="server" SkinID="textboxSkin" />
                                                                <asp:TextBox Width="345" Height="78" ID="txtTexto2" TextMode="MultiLine" runat="server" SkinID="textboxSkin" Visible="false" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Data
                                                            </td>
                                                            <td>
                                                                <asp:TextBox runat="server" CssClass="textbox" ID="txtDataInicio" Width="60px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                                <asp:Image SkinID="imgCanlendario" ID="imgDataInicio" runat="server" EnableViewState="false" />
                                                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataInicio" TargetControlID="txtDataInicio" PopupButtonID="imgDataInicio" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                                &nbsp;<asp:Literal ID="litCriadoPor" runat="server" EnableViewState="false" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Tipo
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="cboTipoAtendimento" SkinID="dropdownSkin" Width="345" AutoPostBack="true" OnSelectedIndexChanged="cboTipoAtendimento_Change" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Subtipo
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="cboSubTipoAtendimento" SkinID="dropdownSkin" Width="345" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Previsão
                                                            </td>
                                                            <td>
                                                                <asp:TextBox runat="server" CssClass="textbox" ID="txtDataPrevisao" Width="60px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                                <asp:Image SkinID="imgCanlendario" ID="imgDataPrevisao" runat="server" EnableViewState="false" />
                                                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataPrevisao" TargetControlID="txtDataPrevisao" PopupButtonID="imgDataPrevisao" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <asp:CheckBox ID="chkAtendimentoConcluido" Text="Concluído" runat="server" SkinID="checkboxSkin" />
                                                                <asp:Literal ID="litResolvidoPor" runat="server" EnableViewState="true" />
                                                            </td>
                                                            <%--<td>
                                                                <asp:TextBox runat="server" CssClass="textbox" ID="txtDataConclusao" Width="60px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                                <asp:Image SkinID="imgCanlendario" ID="imgDataConclusao" runat="server" EnableViewState="false" />
                                                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataConclusao" TargetControlID="txtDataConclusao" PopupButtonID="imgDataConclusao" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                            </td>--%>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" align="center">
                                                                <asp:Button ID="cmdFecharAtendimento" Font-Size="10px" Text="Novo" SkinID="botaoAzulBorda"
                                                                    OnClick="cmdFecharAtendimento_Click" runat="server" />
                                                                &nbsp;&nbsp;&nbsp;
                                                                <asp:Button ID="cmdSalvarAtendimento" Font-Size="10px" Text="Gravar" SkinID="botaoAzulBorda"
                                                                    OnClick="cmdSalvarAtendimento_Click" OnClientClick="return checaConclusaoAtendimento();"
                                                                    runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td id="tdCobrancas" runat="server" visible="false" valign="top" enableviewstate="false">
                                        <!---->
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <br />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="p8Cobranca" runat="server">
            <HeaderTemplate>
                <asp:Literal ID="litCobrancaHeader" runat="server" Text="<span class='subtitulo'>Cobranças</span>" />
            </HeaderTemplate>
            <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="upCobrancas">
                <ContentTemplate>
                <table style="border: solid 1px #507CD1" width="800" cellpadding="3" cellspacing="0">
                    <tr>
                        <td class="tdNormal1" valign="top" colspan="2">
                            Código do cliente:&nbsp;
                            <asp:Label ID="lblCodigoCliente" runat="server" />
                            <br />
                            <br />
                            E-mail para envio<br />
                            <asp:TextBox ID="txtEmailAtendimento" Width="210px" SkinID="textboxSkin" runat="server" MaxLength="70" />
                            <br />
                            <br />
                            Cópia para<br />
                            <asp:TextBox ID="txtEmailAtendimentoCC" Width="210px" SkinID="textboxSkin" runat="server" MaxLength="70" />
                        </td><%--NEGOCIACAO ,HeaderParcID,HeaderItemID--%>
                        <td class="tdPrincipal1" align="center" rowspan="2" valign="top">
                            <asp:Literal ID="litTemp" runat="server" />
                            <asp:GridView ID="gridCobranca" runat="server" SkinID="gridViewSkin" DataKeyNames="ID,HeaderParcID,HeaderItemID"
                                AutoGenerateColumns="False" Width="100%" OnRowDataBound="gridCobranca_RowDataBound"
                                OnRowCommand="gridCobranca_RowCommand" OnPageIndexChanging="gridCobranca_PageIndexChanging"
                                AllowPaging="True" PageSize="15">
                                <Columns>
                                    <asp:BoundField DataField="Parcela" HeaderText="Parc.">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Valor">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkValor" CommandArgument='<%# Container.DataItemIndex %>' runat="server"
                                                Font-Size="10px" Text='<%# Bind("Valor", "{0:C}") %>' CommandName="detalhe" />
                                            <asp:Panel ID="pnlComposite" runat="server" Visible="false" >
                                                <asp:Literal ID="litComposite" runat="server" Text="aguardando..." />
                                            </asp:Panel>
                                            <cc1:BalloonPopupExtender Enabled="false" ID="balloon" UseShadow="false" DisplayOnClick="false"
                                                DisplayOnMouseOver="true" BalloonPopupControlID="pnlComposite" runat="server"
                                                TargetControlID="lnkValor" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ValorPgto" HeaderText="Valor Pago" DataFormatString="{0:C}">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Vencimento">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtVenctoCobGrid" Width="60" runat="server" Font-Size="10px" Text='<%# Bind("DataVencimento", "{0:dd/MM/yyyy}") %>'
                                                SkinID="textboxSkin" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="strEnviado" HeaderText="Enviado">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="strPago" HeaderText="Pago">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="strDataPago" HeaderText="Data Pgto">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:ButtonField Text="<img src='images/mail.gif' border='0' alt='enviar e-mail' title='enviar e-mail' />"
                                        CommandName="email">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                    <asp:ButtonField Visible="False" ButtonType="Image" ImageUrl="~/images/refresh.png"
                                        Text="recalcular" CommandName="recalcular">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                    <asp:ButtonField ButtonType="Image" ImageUrl="~/images/print.png" Text="imprimir"
                                        CommandName="print">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                </Columns>
                                <PagerSettings PageButtonCount="30" />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td style="border-top: solid 1px #507CD1" colspan="2" valign="top">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <asp:Panel ID="pnlHideCob" runat="server" Visible="false" EnableViewState="false"></asp:Panel>
                                <tr>
                                    <td colspan="2">
                                        <b>Gerar nova parcela</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" height="4">
                                    </td>
                                </tr>
                                <tr>
                                    <td width="60">
                                        Parcela
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtParcelaCob" ReadOnly="True" BackColor="WhiteSmoke" Width="25px"
                                            MaxLength="4" SkinID="textboxSkin" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" height="4">
                                    </td>
                                </tr>
                                <tr>
                                    <td width="50">
                                        Vencimento
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtVencimentoCob" ReadOnly="false" Width="60px" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" height="4">
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Valor
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtValorCob" ReadOnly="True" BackColor="WhiteSmoke" 
                                            Width="60px" MaxLength="10"
                                            SkinID="textboxSkin" runat="server" />&nbsp;
                                        <asp:ImageButton Visible="False" EnableViewState="False" ID="btnCalcularValorCob"
                                            ImageUrl="~/images/refresh.png" ImageAlign="Bottom" ToolTip="calcular valor"
                                            runat="server" OnClick="btnCalcularValorCob_Click" />&nbsp;&nbsp;
                                        <asp:Button EnableViewState="False" ID="cmdGerarCobranca" Text="Gerar cobrança"
                                            SkinID="botaoAzulBorda" OnClick="cmdGerarCobranca_Click" OnClientClick="return confirm('ATENÇÃO!\nDeseja realmente gerar uma nova cobrança?');"
                                            runat="server" Font-Size="11px" />
                                    </td>
                                </tr>

                                <tr height="10">
                                    <td height="10">
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2">
                                        <br />
                                        <asp:LinkButton ID="cmdRelacaoCobrancas" runat="server" OnClick="cmdRelacaoCobrancas_Click" Text="Relação de parcelas" />
                                        <br />
                                        <asp:LinkButton ID="cmdTabelaValor" runat="server" OnClick="cmdTabelaValor_Click" Text="Consultar tabela de valores" />
                                        <hr width='250' align="center" />


                                        <asp:Panel ID="pnlDemonstPagto" Visible="false" EnableViewState="false" runat="server">
                                        <b><asp:Label EnableViewState="false" ID="cmdDemonsPagto" runat="server" Text="Demonstrativo de pagamentos" /> <br /></b>
                                        Ps Padrão:&nbsp;
                                        <asp:ImageButton Visible="true" ImageUrl="~/images/print.png" ImageAlign="AbsMiddle" ToolTip="visualizar"       EnableViewState="false" ID="imgDemonstPagtoPrint" runat="server" OnClick="cmdDemonsPagto_Click"/>&nbsp;
                                        <asp:ImageButton Visible="true" ImageUrl="~/images/mail.gif" ImageAlign="AbsMiddle" ToolTip="enviar por e-mail" EnableViewState="false" ID="imgDemonstPagtoMail" runat="server" OnClick="imgDemonstPagtoMail_Click" OnClientClick="return confirm('Deseja realmente enviar o DEMONSTRATIVO DE PAGTOS para o e-mail ' + document.getElementById('ctl00_cphContent_tab_p8Cobranca_txtEmailAtendimento').value + ' ?');" />
                                        <br />
                                        <br />
                                        <b><asp:Label EnableViewState="false" ID="lblCartaDePermanecia" Text="Carta de permanência:" runat="server" /></b> &nbsp;
                                        <asp:ImageButton ImageUrl="~/images/print.png" ImageAlign="AbsMiddle" ToolTip="visualizar"       EnableViewState="false" ID="cmdCartaDePermanenciaPrint" runat="server" OnClick="cmdCartaDePermanenciaPrint_Click"/>&nbsp;
                                        <asp:ImageButton ImageUrl="~/images/mail.gif" ImageAlign="AbsMiddle" ToolTip="enviar por e-mail" EnableViewState="false" ID="cmdCartaDePermanecia"       runat="server" OnClick="cmdCartaDePermanecia_Click" OnClientClick="return confirm('Deseja realmente enviar a CARTA DE PERMANÊNCIA para o e-mail ' + document.getElementById('ctl00_cphContent_tab_p8Cobranca_txtEmailAtendimento').value + ' ?');" />
                                        <br />
                                        <b><asp:Label EnableViewState="false" ID="Label1" Text="Termo de quitação Anual:" runat="server" /></b> &nbsp;
                                        <asp:ImageButton ImageUrl="~/images/print.png" ImageAlign="AbsMiddle" ToolTip="visualizar"       EnableViewState="false" ID="cmdTermoAnualPrint" runat="server" OnClick="cmdTermoAnualPrint_Click"/>&nbsp;
                                        <asp:ImageButton ImageUrl="~/images/mail.gif" ImageAlign="AbsMiddle" ToolTip="enviar por e-mail" EnableViewState="false" ID="cmdTermoAnual"       runat="server" OnClick="cmdTermoAnual_Click" OnClientClick="return confirm('Deseja realmente enviar o TERMO DE QUITAÇÃO para o e-mail ' + document.getElementById('ctl00_cphContent_tab_p8Cobranca_txtEmailAtendimento').value + ' ?');" />
                                        </asp:Panel>

                                        Demonstrativo de pagamento:&nbsp;
                                        <asp:ImageButton Visible="true" ImageUrl="~/images/print.png" ImageAlign="AbsMiddle" ToolTip="visualizar"       EnableViewState="false" ID="imgDemonstPagtoQualiPrint" runat="server" OnClick="cmdDemonsPagtoQuali_Click"/>&nbsp;
                                        <asp:ImageButton Visible="true" ImageUrl="~/images/mail.gif" ImageAlign="AbsMiddle" ToolTip="enviar por e-mail" EnableViewState="false" ID="imgDemonstPagtoQualiMail" runat="server" OnClick="imgDemonstPagtoQualiMail_Click" OnClientClick="return confirm('Deseja realmente enviar o DEMONSTRATIVO DE PAGTOS para o e-mail ' + document.getElementById('ctl00_cphContent_tab_p8Cobranca_txtEmailAtendimento').value + ' ?');" />
                                        
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>
    </cc1:TabContainer>
    <table cellpadding="2" width="70%">
        <tr>
            <td align="left">
                <asp:Button Width="70" OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server"
                    EnableViewState="false" ID="cmdVoltar" Text="Voltar" />
            </td>
            <td align="right">
                <asp:UpdatePanel ID="upSalvar" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:Button EnableViewState="true" Width="70" SkinID="botaoAzul" runat="server" ID="cmdSalvar"
                            Text="Salvar" OnClick="cmdSalvar_Click" />&nbsp;
                        <asp:Button EnableViewState="true" Visible="false" Width="70" SkinID="botaoAzul"
                            runat="server" ID="cmdRascunho" Text="Rascunho" OnClick="cmdRascunho_Click" OnClientClick="return confirm('Como rascunho, esta proposta não será contabilizada.\nDeseja realmente continuar?');" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>

    <asp:Panel ID="pnlPlanoAlteracaoHIDE" Visible="false" EnableViewState="false" runat="server">
    <asp:UpdatePanel runat="server" ID="upPlanoAlteracao" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:ModalPopupExtender ID="MPEPlano"         runat="server" EnableViewState="false" TargetControlID="linkPlano"         PopupControlID="pnlPlano"      BackgroundCssClass="modalBackground" CancelControlID="cmdCloseAlert"         DropShadow="true" />
            
            <asp:Panel runat="server" ID="pnlPlano">
                <asp:LinkButton runat="server" EnableViewState="false" ID="linkPlano" />
                <table width="350" align="center" bgcolor="white" style="border: solid 1px black">
                    <tr>
                        <td colspan="2" class="tdPrincipal1" align="center">
                            Alteração de plano
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" width="100">
                            Plano
                        </td>
                        <td class="tdNormal1">
                            <asp:DropDownList Width="240" ID="cboPlanoAltera" runat="server" SkinID="dropdownSkin"
                                AutoPostBack="true" OnSelectedIndexChanged="cboPlanoAltera_OnSelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" width="100">
                            Acomodação
                        </td>
                        <td class="tdNormal1">
                            <asp:DropDownList Width="240" ID="cboAcomodacaoAltera" runat="server" SkinID="dropdownSkin" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" width="100">
                            Admissão
                        </td>
                        <td class="tdNormal1">
                            <asp:TextBox EnableViewState="false" runat="server" SkinID="textboxSkin" ID="txtPlanoAdmissao" Width="60"  onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <font color='red'>
                                <asp:Literal ID="litMigrarPlanoMsg" runat="server" EnableViewState="false" /></font>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="tdNormal1" align="center">
                            <asp:Button runat="server" SkinID="botaoAzulBorda" Text="fechar" ID="cmdFecharPlano"
                                OnClick="cmdFecharPlano_Click" EnableViewState="false" />
                            &nbsp;&nbsp;
                            <asp:Button runat="server" SkinID="botaoAzulBorda" Text="salvar" ID="cmdSalvarPlano"
                                OnClick="cmdSalvarPlano_Click" EnableViewState="false" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </asp:Panel>

    <asp:UpdatePanel runat="server" ID="upDetalheSeguro" UpdateMode="Always">
        <ContentTemplate>
            <cc1:ModalPopupExtender ID="MPEDetalheSeguro" runat="server" EnableViewState="false" TargetControlID="linkDetalheSeguro" PopupControlID="pnlDetalheSeguro" BackgroundCssClass="modalBackground" CancelControlID="cmdCloseDetalheSeguro" DropShadow="true" />
            <asp:Panel runat="server" ID="pnlDetalheSeguro">
                <asp:LinkButton runat="server" EnableViewState="false" ID="linkDetalheSeguro" />
                <table width="350" cellpadding="4px" cellspacing="0" align="center" bgcolor="white" style="border: solid 1px black">
                    <tr>
                        <td colspan="2" class="tdPrincipal1" align="left" width="150px">
                            <asp:Literal ID="litDetalheSeguroTitulo" Text="Detalhes do seguro " runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="1" class="tdNormal1" align="left" width="150px">
                            Selecione o prêmio
                        </td>
                        <td Width="200px">
                            <asp:DropDownList SkinID="dropdownSkin" ID="cboPremioDetalhe" runat="server" Width="200px" AutoPostBack="true" OnSelectedIndexChanged="cboPremioDetalhe_change" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" width="100">
                            Prêmio
                        </td>
                        <td class="tdNormal1">
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDetalheSeguro_Premio" Width="65"  MaxLength="14" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" width="100">
                            Cobertura
                        </td>
                        <td class="tdNormal1">
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDetalheSeguro_Cobertura" ReadOnly="true" Width="65"  MaxLength="14" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" width="100">
                            Início
                        </td>
                        <td class="tdNormal1">
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDetalheSeguro_DataInicio" Width="65"  onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" width="100">
                            Forma Pagto.
                        </td>
                        <td class="tdNormal1">
                            <asp:DropDownList Enabled="false" runat="server" SkinID="dropdownSkin" ID="cboDetalheSeguro_Pagto" Width="200px">
                                <asp:ListItem Value="31" Text="Boleto" />
                                <asp:ListItem Value="09" Text="Crédito" />
                                <asp:ListItem Value="10" Text="Débito" />
                                <asp:ListItem Value="81" Text="Desconto em conta" />
                                <asp:ListItem Value="11" Text="Desconto em folha" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" width="100">
                            Status
                        </td>
                        <td class="tdNormal1">
                            <asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboDetalheSeguro_Status" Width="80px">
                                <asp:ListItem Text="Ativo" Value="A" />
                                <asp:ListItem Text="Inativo" Value="B" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <font color='red'>
                                <asp:Literal ID="litDetalheSeguroMsg" runat="server" EnableViewState="false" /></font>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="tdNormal1" align="center">
                            <asp:Button runat="server" SkinID="botaoAzulBorda" Text="fechar" ID="cmdCloseDetalheSeguro"
                                OnClick="cmdFecharDetalheSeguro_Click" EnableViewState="false" />
                            &nbsp;&nbsp;
                            <asp:Button runat="server" SkinID="botaoAzulBorda" Text="salvar" ID="cmdSalvarDetalheSeguro"
                                OnClick="cmdSalvarDetalheSeguro_Click" EnableViewState="false" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>


    <asp:UpdatePanel runat="server" ID="upnlAlerta" UpdateMode="Conditional" EnableViewState="false">
        <ContentTemplate>
            <cc1:ModalPopupExtender ID="MPE" runat="server" EnableViewState="false" TargetControlID="lnk" PopupControlID="pnlAlert" BackgroundCssClass="modalBackground" CancelControlID="cmdCloseAlert" DropShadow="true" />
            <asp:Panel runat="server" ID="pnlAlert" EnableViewState="false">
                <asp:LinkButton runat="server" EnableViewState="false" ID="lnk" />
                <table width="350" align="center" bgcolor="gainsboro" style="border: solid 1px black">
                    <tr>
                        <td align="center">
                            <asp:Literal runat="server" ID="litAlert" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr height="8">
                        <td height="8">
                            &nbsp
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <input runat="server" style="width: 45px; font-size: 12px; font-family: Arial" id="cmdCloseAlert"
                                type="button" value="OK" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <!------------------------------------------------------------------------------------------------>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upCobrancaDetalhe" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:ModalPopupExtender ID="mpeCobrancaDetalhe" runat="server" PopupControlID="pnlCobrancaDetalhe" CancelControlID="cmdFecharCobrancaDetalhe" TargetControlID="target">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlCobrancaDetalhe" EnableViewState="false" runat="server">
                <asp:LinkButton runat="server" EnableViewState="false" ID="target" />
                <asp:TextBox ID="txtIdCobrancaEmDetalhe" runat="server" />
                <table width="500" cellpadding="0" cellspacing="4" style="border:solid 4px gray;background-color:white">
                    <tr>
                        <td align="center" height='30' valign="middle" style="background-color:Gray;color:White">
                            <asp:Literal ID="litTitulo" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" height='20' valign="middle" style="background-color:Whitesmoke;">
                            <b>Composição da Parcela</b>&nbsp;&nbsp;
                            <asp:ImageButton ID="cmdRecalcularComposicao" OnClientClick="return confirm('Deseja recalcular a composição da parcela?');" ImageAlign="AbsMiddle" ImageUrl="~/images/refresh.png" ToolTip="recalcular composição da parcela" OnClick="cmdRecalcularComposicao_click" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="gridComposicao" runat="server" SkinID="gridViewSkin2"
                                AutoGenerateColumns="False" Width="500"
                                AllowPaging="false">
                                <Columns>
                                    <asp:BoundField HeaderText="Tipo" DataField="StrTipo" />
                                    <asp:BoundField HeaderText="Beneficiário" DataField="BeneficiarioNome" />
                                    <asp:BoundField HeaderText="Valor" DataField="Valor" DataFormatString="{0:N2}" />
                                </Columns>
                                <HeaderStyle HorizontalAlign="Left" />
                                <RowStyle Height="15px" />
                            </asp:GridView>
                        </td>
                    </tr>
                    <asp:Literal ID="litBaixa" runat="server" EnableViewState="false" />
                    <asp:Literal ID="litNegociacao" runat="server" EnableViewState="false" />
                    <tr height='10'><td height='10' style="border-top:solid 1px Gray"></td></tr>
                    <tr>
                        <td align="center">
                            <asp:Button Text="Fechar" SkinID="botaoPequeno" ID="cmdFecharCobrancaDetalhe" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="upAlteraStatus" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:ModalPopupExtender ID="mpeAlteraStatus" runat="server" PopupControlID="pnlAlteraStatus" CancelControlID="cmdFecharHistoricoStatus" TargetControlID="target2">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlAlteraStatus" EnableViewState="true" runat="server">
                <asp:LinkButton runat="server" EnableViewState="false" ID="target2" />
                <table width="500" cellpadding="0" cellspacing="4" style="border:solid 4px gray;background-color:white">
                    <tr>
                        <td class="tdPrincipal1" align="center" colspan="2" height="20">
                            Status da proposta
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1">Selecione</td>
                        <td class="tdNormal1">
                            <asp:RadioButton ID="optNormalEdit"    Text="Reativação"   GroupName="statusEd" runat="server" AutoPostBack="true" OnCheckedChanged="optStatusEdit_Changed" EnableViewState="true" Checked="true" />
                            <asp:RadioButton ID="optInativoEdit"   Text="Inativação"   GroupName="statusEd" runat="server" AutoPostBack="true" OnCheckedChanged="optStatusEdit_Changed" EnableViewState="true" />
                            <asp:RadioButton ID="optCanceladoEdit" Text="Cancelamento" GroupName="statusEd" runat="server" AutoPostBack="true" OnCheckedChanged="optStatusEdit_Changed" EnableViewState="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1">Motivo</td>
                        <td class="tdNormal1"><asp:DropDownList ID="cboStatusMotivo" runat="server" Width="300" SkinID="dropdownSkin" /></td>
                    </tr>
                    <tr>
                        <td class="tdNormal1">Data</td>
                        <td class="tdNormal1"><asp:TextBox runat="server" EnableViewState="false" CssClass="textbox" ID="txtDataInativacao" Width="60px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" /></td>
                    </tr>
                    <tr height='10'><td height='10' style="border-top:solid 1px Gray" colspan="2"></td></tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Button Text="Fechar" SkinID="botaoPequeno" ID="cmdFecharHistoricoStatus" runat="server" EnableViewState="false" />
                            <asp:Button Text="Salvar" SkinID="botaoPequeno" ID="cmdSalvarHistoricoStatus" runat="server" EnableViewState="false" OnClick="cmdSalvarHistoricoStatus_Click" OnClientClick="return confirm('Deseja realmente alterar o status da proposta?');" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    
</asp:Content>
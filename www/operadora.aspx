<%@ Page EnableEventValidation="false" EnableViewState="true" Language="C#" Theme="Theme" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="operadora.aspx.cs" Inherits="www.operadora" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"><img height="50" src="images/imgTitulos/operadoras.jpg" /></td></tr>
        <tr><td><span class="titulo">Operadora</span></td></tr>
        <tr><td nowrap="true"><span class="subtitulo">Preencha os campos abaixo e clique em salvar</span></td></tr>
    </table>
</asp:Content>

<asp:Content ID="cphContent" ContentPlaceHolderID="cphContent" runat="server">

    <script type="text/javascript">
    function contratosSubMnu(newIndex)
    {
        var currentIndex = document.getElementById('ctl00_cphContent_tab_p2_hiddenContratosMnu').value;
        if (currentIndex != '')
        {
            if (currentIndex == newIndex)
            {
                //do nothing
            }
            else if (currentIndex == '2')
            {
                document.getElementById('ctl00_cphContent_tab_p2_lnkpanelComissionamentos').click();
//                $find('ctl00_cphContent_tab_p2_expanelContratos_ClientState')._doCollapse();
//                $find('ctl00_cphContent_tab_p2_expanelComissiao_ClientState')._doExpand();
            }
            else if (currentIndex == '1')
            {
                document.getElementById('ctl00_cphContent_tab_p2_lnkpanelContratos').click();
            }
        }
        else
            document.getElementById('ctl00_cphContent_tab_p2_lnkpanelContratos').click();

        document.getElementById('ctl00_cphContent_tab_p2_hiddenContratosMnu').value = newIndex;
    }

    function planosSubMnu(newIndex) 
    {
        var currentIndex = document.getElementById('ctl00_cphContent_tab_p4_hiddenPlanosMnu').value;
        if (currentIndex != '')
        {
            if (currentIndex == '1')
                document.getElementById('ctl00_cphContent_tab_p4_lnkpanelPlanos').click();
            else if (currentIndex == '2')
                document.getElementById('ctl00_cphContent_tab_p4_lnkpanelTValores').click();
            else if (currentIndex == '3')
                document.getElementById('ctl00_cphContent_tab_p4_lnkpanelTReajustes').click();
            else if (currentIndex == '4')
                document.getElementById('ctl00_cphContent_tab_p4_lnkpanelTAdicionaisPlano').click();
        }
        else if (currentIndex == newIndex)
        {
            //do nothing
        }
        else
            document.getElementById('ctl00_cphContent_tab_p4_lnkpanelPlanos').click();

        document.getElementById('ctl00_cphContent_tab_p4_hiddenPlanosMnu').value = newIndex;
    }

    function ActiveTabChanged(sender, e)
    {
        switch(sender.get_activeTabIndex())
        {
            case 1: //planos
            {
                break;
            }
            case 2: //contratos
            {
                break;
            }
            case 3: //adicionais
            {
                break;
            }
            case 4: //planos
            {
                __doPostBack('<%=upPlanos.ClientID%>', '');
                break;
            }
            case 5: //calendario
            {
                __doPostBack('<%=upCalendario.ClientID%>', '');
                break;
            }
        }
    }
</script>
    <cc1:TabContainer BorderStyle="None" BorderWidth="0" Width="100%" ID="tab" runat="server" ActiveTabIndex="0" OnClientActiveTabChanged="ActiveTabChanged" >

        <cc1:TabPanel runat="server" ID="p1">
            <HeaderTemplate><font color="black">Dados comuns</font></HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel runat="server" ID="upDadosComuns" UpdateMode="Conditional">
                    <ContentTemplate>
                        <font color='black'>Preencha os campos abaixo</font><br /><br />
                        <table cellpadding="2">
                            <tr>
                                <td class="tdPrincipal1" colspan="2"><b>Dados comuns do cadastro</b></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="88px">Nome</td>
                                <td class="tdNormal1"><asp:TextBox CssClass="textbox" Width="560px" runat="server" ID="txtNome" MaxLength="180" /></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="88px">CNPJ</td>
                                <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtCnpj" Width="102px" MaxLength="14" /><cc1:MaskedEditExtender runat="server" id="meeCNPJ" Mask="99\.999\.999\/9999-99" TargetControlID="txtCNPJ" /></td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="2">
                            <tr>
                                <td class="tdPrincipal1" colspan="4"><b>Endereço</b></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="59px">CEP</td>
                                <td class="tdNormal1" colspan="3">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtCEP" Width="65px" MaxLength="9" />
                                    <asp:ImageButton runat="server" EnableViewState="false" ToolTip="checar CEP" ImageUrl="~/images/endereco.png" ID="cmdBuscaEndereco" OnClick="cmdBuscaEndereco_Click" />
                                    <cc1:MaskedEditExtender TargetControlID="txtCEP" Mask="99999-999" 
                                        runat="server" ID="meeCEP" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" EnableViewState="false"
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">Logradouro</td>
                                <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtLogradouro" Width="290px" MaxLength="300" /></td>
                                <td class="tdPrincipal1">&nbsp;Número</td>
                                <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtNumero" Width="65px" MaxLength="9" /></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">Complemento</td>
                                <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtComplemento" Width="200px" MaxLength="250" /></td>
                                <td class="tdPrincipal1" width="72px">&nbsp;Bairro</td>
                                <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtBairro" Width="190px" MaxLength="300" /></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="80px">Cidade</td>
                                <td class="tdNormal1" width="293"><asp:TextBox CssClass="textbox" runat="server" ID="txtCidade" Width="200px" MaxLength="300" /></td>
                                <td class="tdPrincipal1" width="72px">UF</td>
                                <td class="tdNormal1" width="195"><asp:TextBox CssClass="textbox" runat="server" ID="txtUF" Width="20px" MaxLength="2" /></td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="2">
                            <tr>
                                <td class="tdPrincipal1" colspan="2"><b>Outras Informações</b></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="165px">Mensagem do arquivo de voz</td>
                                <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtMensagemRemessa" Width="240px" MaxLength="150" /></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>

        <cc1:TabPanel runat="server" ID="p1a">
            <HeaderTemplate><font runat="server" id="titP1a" color="black">Contatos</font></HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel runat="server" ID="upContatos" UpdateMode="Conditional">
                    <ContentTemplate>
                        <font color='black'>Contatos na operadora</font><br /><br />
                        <asp:Panel runat="server" ID="pnlListaContatos">
                            <asp:GridView DataKeyNames="ID" Font-Size="10px" Width="70%" ID="gridContatos" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" BorderWidth="1px" BorderColor="#507CD1" GridLines="None" OnRowCommand="gridContatos_OnRowCommand" OnRowDataBound="gridContatos_OnRowDataBound" >
                                <RowStyle BackColor="#EFF3FB" Font-Size="10px" />
                                <Columns>
                                    <asp:BoundField DataField="Nome" HeaderText="Contato" ItemStyle-Width="40%">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Departamento" HeaderText="Departamento" ItemStyle-Width="30%">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Fone" HeaderText="Fone">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Email" HeaderText="E-mail">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' align='middle' title='excluir' alt='excluir' border='0' />">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                    <asp:ButtonField CommandName="editar" Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />">
                                        <ItemStyle Width="1%" HorizontalAlign="Center" />
                                        <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                    </asp:ButtonField>
                                </Columns>
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <EditRowStyle BackColor="#2461BF" />
                                <AlternatingRowStyle  Font-Size="10px" BackColor="White" />
                            </asp:GridView>
                            <br />
                            <asp:Button Text="novo" SkinID="botaoPequeno" EnableViewState="false" ID="cmdNovoContato" OnClick="cmdNovoContato_Click" runat="server" />
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlDetalheContato" Visible="false">
                            <table width="306" cellpadding="3" cellspacing="0">
                                <tr>
                                    <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo" runat="server" id="span7">Detalhes do contato</span></td>
                                    <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdFecharContato" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdFecharContato_Click" /></td>
                                </tr>
                            </table>
                            <table width="306" bgcolor='#EFF3FB' border="0">
                                <tr>
                                    <td width="15%">Contato</td>
                                    <td width="85%"><asp:TextBox CssClass="textbox" runat="server" ID="txtContato" Width="98%" MaxLength="149" EnableViewState="false" /></td>
                                </tr>
                                <tr>
                                    <td width="15%">Depto.</td>
                                    <td width="85%"><asp:TextBox CssClass="textbox" runat="server" ID="txtContatoDepartamento" Width="98%" MaxLength="249" EnableViewState="false" /></td>
                                </tr>
                            </table>
                            <table width="306" bgcolor='#EFF3FB' border="0">
                                <tr>
                                    <td width="15%">DDD</td>
                                    <td width="40px"><asp:TextBox CssClass="textbox" runat="server" ID="txtDDD1" Width="30px" MaxLength="3" EnableViewState="false" /><cc1:MaskedEditExtender runat="server" id="meeDDD" Mask="99" TargetControlID="txtDDD1" /></td>
                                    <td width="34px">Fone</td>
                                    <td width="61px"><asp:TextBox CssClass="textbox" runat="server" ID="txtFone1" Width="60px" MaxLength="9" EnableViewState="false" /><cc1:MaskedEditExtender runat="server" id="meeFone" Mask="9999-9999" TargetControlID="txtFone1" /></td>
                                    <td width="37px" align="right">&nbsp;Ramal&nbsp;</td>
                                    <td align="left"><asp:TextBox CssClass="textbox" runat="server" ID="txtRamal1" Width="89%" MaxLength="5" EnableViewState="false" />&nbsp;</td>
                                </tr>
                            </table>
                            <table width="306" bgcolor='#EFF3FB'>
                                <tr>
                                    <td width="15%">Email</td>
                                    <td width="85%"><asp:TextBox CssClass="textbox" Width="98%" runat="server" ID="txtEmail" MaxLength="65" EnableViewState="false" /></td>
                                </tr>
                            </table>
                            <table cellpadding="0" cellspacing="0" width="306">
                                <tr>
                                    <td align="right" height="27" class="tdPrincipal1" style="padding-right:2px">
                                        <div style="text-align:center; width: 110px; height:20px; border:solid 1px #EFF3FB;">
                                            <asp:Image ImageUrl="~/images/save.png" EnableViewState="false" runat="server" /><asp:Button Width="90" runat="server" ID="cmdSalvarContato" SkinID="botaoAzul" Text="salvar contato" BorderWidth="0px" OnClick="cmdSalvarContato_Click" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>

        <cc1:TabPanel runat="server" ID="p2">
            <HeaderTemplate><font runat="server" id="titP2" color="black">Contratos</font></HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel runat="server" ID="upContratos" UpdateMode="Conditional">
                    <ContentTemplate>

                        <cc1:CollapsiblePanelExtender ID="expanelContratos" runat="Server" EnableViewState="false"
                            TargetControlID="pnlTopoContrato"
                            Collapsed="false"
                            ExpandControlID="lnkpanelContratos"
                            CollapseControlID="lnkpanelContratos"
                            AutoCollapse="False"
                            AutoExpand="true" 
                            ExpandDirection="Vertical" SuppressPostBack="true"/>
                        
                        <cc1:CollapsiblePanelExtender ID="expanelComissiao" runat="Server" EnableViewState="false"
                            TargetControlID="pnlTopoComissao"
                            Collapsed="true"
                            ExpandControlID="lnkpanelComissionamentos"
                            CollapseControlID="lnkpanelComissionamentos"
                            AutoCollapse="False"
                            AutoExpand="true"
                            ExpandDirection="Vertical" SuppressPostBack="true" />

                        <table width="99%" border="0">
                            <tr>
                                <td valign="top" width="150">
                                    <asp:TextBox style="display:none" runat="server" ID="hiddenContratosMnu" />
                                    <table width="95%" cellpadding="3" cellspacing="2" bgcolor="#EFF3FB">
                                        <tr>
                                            <td style="border: solid 1px #507CD1"><asp:LinkButton ForeColor="#507CD1" Font-Size="10pt" OnClientClick="contratosSubMnu('1');" runat="server" EnableViewState="false" Text="Contratos" ID="lnkpanelContratos"/></td>
                                        </tr>
                                        <tr>
                                            <td style="border: solid 1px #507CD1"><asp:LinkButton ForeColor="#507CD1" Font-Size="10pt" OnClientClick="contratosSubMnu('2');" runat="server" EnableViewState="false" Text="Comissionamento" ID="lnkpanelComissionamentos"/></td>
                                        </tr>
                                    </table>
                                </td>
                                <td valign="top">
                                    <asp:Panel runat="server" ID="pnlTopoContrato">
                                        <font style="font-size:13px" color='black'>Contratos da operadora</font><br /><br />
                                        <asp:Panel runat="server" ID="pnlListaContratos">
                                        <asp:GridView SkinID="gridViewSkin" DataKeyNames="ID,Ativo" Font-Size="10px" Width="500px" ID="gridContratos" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" BorderWidth="1px" BorderColor="#507CD1" GridLines="None" OnRowCommand="gridContratos_OnRowCommand" OnRowDataBound="gridContratos_OnRowDataBound" >
                                            <Columns>
                                                <asp:BoundField DataField="Numero" HeaderText="Sigla">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle Width="3%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Descricao" HeaderText="Contrato">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="EstipulanteDescricao" HeaderText="Estipulante">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />">
                                                    <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                    <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                                </asp:ButtonField>
                                                <asp:ButtonField CommandName="ativar" Text="<img src='images/active.png' align='middle' title='tornar atual a tabela' alt='tornar atual a tabela' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                                <asp:ButtonField CommandName="editar" Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />">
                                                    <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                    <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                                </asp:ButtonField>
                                            </Columns>
                                        </asp:GridView>
                                        <br />
                                        <asp:Button Text="novo" SkinID="botaoPequeno" EnableViewState="false" ID="cmdNovoContrato" OnClick="cmdNovoContrato_Click" runat="server" />
                                    </asp:Panel>
		                                <asp:Panel runat="server" ID="pnlDetalheContrato">

                                            <table width="306"cellpadding="3" cellspacing="0">
                                                <tr>
                                                    <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo" runat="server" id="span3">Detalhes do contrato</span></td>
                                                    <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdFecharContrato" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdFecharContrato_Click" /></td>
                                                </tr>
                                            </table>
                                            <table width="306" bgcolor='#EFF3FB'>
                                                <tr>
                                                    <td width="76"><span class="subtitulo">Estipulante</span></td>
                                                    <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboContratoEstipulante" Width="100%" /></td>
                                                </tr>
                                                <tr>
                                                    <td><span class="subtitulo">Descrição</span></td>
                                                    <td><asp:TextBox runat="server" EnableViewState="false" ID="txtContratoDescricao" Width="98%" MaxLength="200" SkinID="textboxSkin" /></td>
                                                </tr>
                                                
                                                <tr>
                                                    <td><span class="subtitulo">Contrato Saúde</span></td>
                                                    <td><asp:TextBox runat="server" EnableViewState="false" ID="txtContratoSaude" Width="50%" MaxLength="100" SkinID="textboxSkin" /></td>
                                                </tr>
                                                <tr>
                                                    <td><span class="subtitulo">Contrato Dental</span></td>
                                                    <td><asp:TextBox runat="server" EnableViewState="false" ID="txtContratoDental" Width="50%" MaxLength="100" SkinID="textboxSkin" /></td>
                                                </tr>
                                                
                                                <tr>
                                                    <td><span class="subtitulo">Número</span></td>
                                                    <td><asp:TextBox runat="server" EnableViewState="false" ID="txtContratoNumero" Width="50%" MaxLength="100" SkinID="textboxSkin" /></td>
                                                </tr>
                                                <tr>
                                                    <td><span class="subtitulo">Cód. Filial</span></td>
                                                    <td><asp:TextBox runat="server" EnableViewState="false" ID="txtContratoCodFilial" Width="50%" MaxLength="50" SkinID="textboxSkin" /></td>
                                                </tr>
                                                <tr>
                                                    <td><span class="subtitulo">Cód. Unidade</span></td>
                                                    <td><asp:TextBox runat="server" EnableViewState="false" ID="txtContratoCodUnidade" Width="50%" MaxLength="50" SkinID="textboxSkin" /></td>
                                                </tr>
                                                <tr>
                                                    <td><span class="subtitulo">Cód. Administr.</span></td>
                                                    <td><asp:TextBox runat="server" EnableViewState="false" ID="txtContratoCodAdministradora" Width="50%" MaxLength="50" SkinID="textboxSkin" /></td>
                                                </tr>
                                                <tr>
                                                    <td><span class="subtitulo">Tipo Cálculo</span></td>
                                                    <td><asp:DropDownList runat="server" EnableViewState="true" ID="cboContratoAdmTipoCalc" Width="60%" SkinID="dropdownSkin" /></td>
                                                </tr>
                                                <tr>
                                                    <td><span class="subtitulo">Data</span></td>
                                                    <td>
                                                        <asp:TextBox EnableViewState="false" SkinID="textboxSkin" Width="67px" runat="server" ID="txtContratoData" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" class="tdNormal1"><asp:CheckBox runat="server" EnableViewState="false" ID="chkContratoAtivo" Text="Ativo" SkinID="checkboxSkin" Checked="true" /></td>
                                                </tr>
                                            </table>
                                            <table cellpadding="0" cellspacing="0" width="306">
                                                <tr>
                                                    <td align="right" height="27" class="tdPrincipal1" style="padding-right:2px">
                                                        <div style="text-align:center; width: 110px; height:20px; border:solid 1px #EFF3FB;">
                                                            <asp:Image ID="Image1" ImageUrl="~/images/save.png" EnableViewState="false" runat="server" /><asp:Button Width="90" runat="server" ID="cmdSalvarContrato" SkinID="botaoAzul" Text="salvar contrato" BorderWidth="0px" OnClick="cmdSalvarContrato_Click" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>


                                        </asp:Panel>
                                    </asp:Panel>
                                    <asp:Panel runat="Server" ID="pnlTopoComissao">
                                        <font style="font-size:13px" color='black'>Tabelas de comissionamento</font><br /><br />
                                        <asp:Panel ID="pnlComissaoLista" runat="server">
                                            <table style="border:solid 1px #507CD1" cellpadding="4" cellspacing="0" width="500">
                                                <tr>
                                                    <td class="tdNormal1" width="60">Contrato</td>
                                                    <td class="tdNormal1"><asp:DropDownList OnSelectedIndexChanged="cboComissionamentoContrato_OnSelectedIndexChanged" AutoPostBack="true" CssClass="textbox" Width="256px" runat="server" ID="cboComissionamentoContrato" /></td>
                                                </tr>
                                            </table>
                                            <asp:GridView ID="gridComissionamento" Width="500px" SkinID="gridViewSkin" 
                                                runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID"
                                                OnRowCommand="gridComissionamento_RowCommand" OnRowDataBound="gridComissionamento_RowDataBound">
                                                <Columns>
                                                    <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="EstipulanteDescricao" HeaderText="Estipulante">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="ContratoAdmNumero" HeaderText="Núm. Contrato">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField Visible="true" DataField="Data" HeaderText="Vigência" DataFormatString="{0:dd/MM/yyyy}">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                                                        <ItemStyle ForeColor="#cc0000" Width="1%" />
                                                    </asp:ButtonField>
                                                    <asp:ButtonField Visible="false" Text="<img src='images/active.png' title='ativar tabela de comissionamento' alt='ativar tabela de comissionamento' border='0' />" CommandName="ativar" >
                                                        <ItemStyle Width="1%" />
                                                    </asp:ButtonField>
                                                    <asp:ButtonField Visible="false" Text="<img src='images/condidade.png' title='condições por idade' alt='condições por idade' border='0' />" CommandName="idade" >
                                                        <ItemStyle Width="1%" />
                                                    </asp:ButtonField>
                                                    <asp:ButtonField Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                                                        <ItemStyle Width="1%" />
                                                    </asp:ButtonField>
                                                </Columns>
                                            </asp:GridView>
                                            <br />
                                            <asp:Button EnableViewState="false" runat="server" SkinID="botaoPequeno" ID="cmdNovoComissionamento" Text="Nova" OnClick="cmdNovoComissionamento_Click" />
                                        </asp:Panel>
                                        <asp:Panel ID="pnlComissaoDetalhe" runat="server" Visible="False">
                                            <table width="75%" cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo">Detalhes da tabela de comissionamento</span> </td>
                                                    <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdComissaoFechar" runat="server" EnableViewState="False" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdComissaoFechar_Click" /></td>
                                                </tr>
                                            </table>
                                            <table style="border:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="75%">
                                                <tr>
                                                    <td class="tdNormal1">Contrato</td>
                                                    <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" Width="245px" runat="server" ID="cboComissionamentoContratoDETALHE" AutoPostBack="true" OnSelectedIndexChanged="cboComissionamentoContratoDETALHE_OnSelectedIndexChanged" /></td>
                                                </tr>
                                                <tr>
                                                    <td class="tdNormal1">Vigência</td>
                                                    <td class="tdNormal1">
                                                        <asp:TextBox EnableViewState="true" CssClass="textbox" Width="67px" runat="server" ID="txtComissionamentoData" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:GridView ID="gridComissionamentoItensDetalhe" SkinID="gridViewSkin" Width="75%" runat="server" 
                                                AutoGenerateColumns="False" DataKeyNames="ID" 
                                                onrowdatabound="gridComissionamentoItensDetalhe_RowDataBound" onrowcommand="gridComissionamentoItensDetalhe_RowCommand">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Parcela">
                                                        <ItemTemplate>
                                                            <asp:TextBox CssClass="textbox" Width="25" ID="txtParcela" runat="server" Text='<%# Bind("Parcela") %>' />
                                                            <cc1:MaskedEditExtender Mask="99" runat="server" ID="meeParcela" TargetControlID="txtParcela" />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Percentual">
                                                        <ItemTemplate>
                                                            <asp:TextBox CssClass="textbox" Width="50" ID="txtPercentual" runat="server" Text='<%# Bind("Percentual") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Perc. Carência">
                                                        <ItemTemplate>
                                                            <asp:TextBox CssClass="textbox" Width="50" ID="txtPercentualCompraCarencia" runat="server" Text='<%# Bind("PercentualCompraCarencia") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Perc. Migração">
                                                        <ItemTemplate>
                                                            <asp:TextBox CssClass="textbox" Width="50" ID="txtPercentualMigracao" runat="server" Text='<%# Bind("PercentualMigracao") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField Visible="false" HeaderText="Perc. ADM">
                                                        <ItemTemplate>
                                                            <asp:TextBox CssClass="textbox" Width="50" ID="txtPercentualADM" runat="server" Text='<%# Bind("PercentualADM") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Perc. Especial">
                                                        <ItemTemplate>
                                                            <asp:TextBox CssClass="textbox" Width="50" ID="txtPercentualEspecial" runat="server" Text='<%# Bind("PercentualEspecial") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Idade" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:TextBox CssClass="textbox" Width="50" ID="txtIdade" runat="server" Text='<%# Bind("PercentualIdade") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>

                                                    <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir">
                                                        <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                        <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                                    </asp:ButtonField>

                                                </Columns>
                                            </asp:GridView>
                                            <table cellpadding="2" cellspacing="0" width="75%">
                                                <tr>
                                                    <td align="right" height="27" class="tdPrincipal1"><asp:Button Width="147" EnableViewState="false" runat="server" ID="cmdAddItemCom" style="cursor: pointer;"  SkinID="botaoAzulBorda" Text="adicionar item à tabela" OnClick="cmdAddItemCom_Click" /></td>
                                                </tr>
                                            </table>
                                            <table style="border-right:solid 1px #507CD1;border-left:solid 1px #507CD1;border-bottom:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="75%">
                                                <tr>
                                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">&nbsp;</td>
                                                    <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Vitalícia</td>
                                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">A partir da parcela</td>
                                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;">Percentual</td>
                                                </tr>
                                                <tr>
                                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Normal</td>
                                                    <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicio" runat="server" /></td>
                                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicio" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeNumeroParcelaVitalicio" TargetControlID="txtComissionamentoNumeroParcelaVitalicio" /></td>
                                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentual" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Compra de carência</td>
                                                    <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioCarencia" runat="server" /></td>
                                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioCarencia" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioCarencia" TargetControlID="txtComissionamentoNumeroParcelaVitalicioCarencia" /></td>
                                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualCarencia" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Migração</td>
                                                    <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioMigracao" runat="server" /></td>
                                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioMigracao" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioMigracao" TargetControlID="txtComissionamentoNumeroParcelaVitalicioMigracao" /></td>
                                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualMigracao" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <td class="tdNormal1" style="border-right:solid 1px #507CD1">Especial</td>
                                                    <td class="tdNormal1" align="center" style="border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioEspecial" runat="server" /></td>
                                                    <td class="tdNormal1" style="border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioEspecial" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioEspecial" TargetControlID="txtComissionamentoNumeroParcelaVitalicioEspecial" /></td>
                                                    <td class="tdNormal1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualEspecial" runat="server" /></td>
                                                </tr>
                                            </table>
                                            <table cellpadding="0" cellspacing="0" border="0" width="75%">
                                                <tr>
                                                    <td align="right" class="tdPrincipal1" height="27" style="padding-right: 2px;">
                                                        <div style="text-align:center; width: 153px; height:20px; border:solid 1px #EFF3FB;">
                                                            <asp:Image ID="Image7" ImageUrl="~/images/save.png" EnableViewState="false" runat="server" />
                                                            <asp:Button EnableViewState="true" Width="78" runat="server" ID="cmdSalvarComissionamento" SkinID="botaoAzul" Text="salvar tabela" style="cursor: pointer;" BorderWidth="0px" OnClick="cmdSalvarComissionamento_Click" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlComissaoIdade" runat="server" Visible="False">
                                            <table width="55%" cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td align="center" bgcolor="#507CD1" style="border-bottom:solid 1px #EFF3FB"><span style="color:white" class="subtitulo">Condições etárias&nbsp;<asp:Literal ID="litCondicoesEtarias" EnableViewState="false" runat="server"/></span></td>
                                                    <td align="right"  bgcolor="#507CD1" style="border-bottom:solid 1px #EFF3FB"><asp:ImageButton ID="cmdComissaoFecharIdade" runat="server" EnableViewState="False" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdComissaoFecharIdade_Click" /></td>
                                                </tr>
                                            </table>
                                            <asp:GridView ID="gridComissionamentoIdade" SkinID="gridViewSkin" Width="55%" runat="server" 
                                                AutoGenerateColumns="False" DataKeyNames="ID" 
                                                onrowdatabound="gridComissionamentoIdade_RowDataBound" onrowcommand="gridComissionamentoIdade_RowCommand">
                                                <Columns>
                                                    <asp:BoundField DataField="Resumo" HeaderText="Condição">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir">
                                                        <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                        <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                                    </asp:ButtonField>
                                                    <asp:ButtonField Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar">
                                                        <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                        <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                                    </asp:ButtonField>
                                                </Columns>
                                            </asp:GridView>
                                            <br />
                                            <asp:Button EnableViewState="false" runat="server" SkinID="botaoPequeno" ID="cmdNovaTabelaIdade" Text="Nova" OnClick="cmdNovaTabelaIdade_Click" />
                                        </asp:Panel>
                                        <asp:Panel ID="pnlComissaoIdadeDETALHE" runat="server" Visible="False">
                                            <br />
                                            <table style="border:solid 1px #507CD1" cellpadding="4" cellspacing="0" width="55%">
                                                <tr>
                                                    <td class="tdNormal1" width="34px">Idade</td>
                                                    <td class="tdNormal1"><asp:TextBox ID="txtComissaoIdade" Width="30" SkinID="textboxSkin" EnableViewState="false" runat="server" /></td>
                                                    <td class="tdNormal1" align="right"><asp:ImageButton ID="cmdComissaoFecharIdadeItem" runat="server" EnableViewState="False" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdComissaoFecharIdadeItem_Click" /></td>
                                                 </tr>
                                             </table>
                                             <asp:GridView ID="gridComissionamentoIdadeItem" SkinID="gridViewSkin" Width="55%" runat="server" 
                                                AutoGenerateColumns="False" DataKeyNames="ID,ComissionamentoIdadeID" 
                                                OnRowDataBound="gridComissionamentoIdadeItem_RowDataBound" OnRowCommand="gridComissionamentoIdadeItem_RowCommand">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Parcela">
                                                        <ItemTemplate>
                                                            <asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtParcelaIdade" runat="server" Text='<%# Bind("Parcela") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Percentual">
                                                        <ItemTemplate>
                                                            <asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtPercentualIdade" runat="server" Text='<%# Bind("Percentual") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir">
                                                        <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                        <ControlStyle Font-Size="11px" ForeColor="#CC0000" />
                                                    </asp:ButtonField>
                                                </Columns>
                                            </asp:GridView>
                                            <table cellpadding="0" cellspacing="0" width="55%">
                                            <tr>
                                                <td align="right" class="tdPrincipal1" height="27" style="padding-right:2px" >
                                                    <div style="text-align:center; width: 163px; height:20px; border:solid 1px #EFF3FB;">
                                                        <asp:Button Width="143px" runat="server" ID="cmdAddItemIdade" EnableViewState="false" SkinID="botaoAzul" Text="adicionar item à tabela" BorderWidth="0px" OnClick="cmdAddItemIdade_Click" style="cursor: pointer;" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                            <br />
                                            <table cellpadding="0" cellspacing="0" width="55%">
                                                <tr>
                                                    <td align="right" class="tdPrincipal1" height="27" style="padding-right:2px" >
                                                        <div style="text-align:center; width: 163px; height:20px; border:solid 1px #EFF3FB;">
                                                            <asp:Image ID="Image8" ImageUrl="~/images/save.png" EnableViewState="false" runat="server" />
                                                            <asp:Button EnableViewState="false" Width="76px" runat="server" ID="cmdSalvarTabelaIdade" SkinID="botaoAzul" Text="salvar tabela" BorderWidth="0px" OnClick="cmdSalvarTabelaIdade_Click" style="cursor: pointer;" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
         </cc1:TabPanel>

        <cc1:TabPanel runat="server" ID="p3">
            <HeaderTemplate><font runat="server" id="titP3" color="black">Adicionais</font></HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel runat="server" ID="upAdicionais" UpdateMode="Always">
                <ContentTemplate>
                    <font color='black'>Adicionais da operadora</font><br /><br />
                    <asp:Panel runat="server" ID="pnlListaAdicionais">
                        <asp:GridView DataKeyNames="ID,Ativo" Font-Size="10px" Width="70%" ID="gridAdicional" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" BorderWidth="1px" BorderColor="#507CD1" GridLines="None" OnRowCommand="gridAdicional_OnRowCommand" OnRowDataBound="gridAdicional_OnRowDataBound" >
                            <RowStyle BackColor="#EFF3FB" Font-Size="10px" />
                            <Columns>
                                <asp:BoundField DataField="Descricao" HeaderText="Adicional">
                                <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:ButtonField CommandName="detalhes" Text="<img src='images/detail2.png' align='middle' title='detalhes' alt='detalhes' border='0' />">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                                <asp:ButtonField CommandName="ativar" Text="<img src='images/active.png' align='middle' title='tornar atual a tabela' alt='tornar atual a tabela' border='0' />">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                                <asp:ButtonField CommandName="editar" Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />">
                                    <ItemStyle Width="1%" HorizontalAlign="Center" />
                                    <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                </asp:ButtonField>
                            </Columns>
                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <EditRowStyle BackColor="#2461BF" />
                            <AlternatingRowStyle  Font-Size="10px" BackColor="White" />
                        </asp:GridView>
                        <br />
                        <table visible="false" bgcolor="#507CD1" runat="server" id="tblAdicionalFaixa" width="45%" cellpadding="0" cellspacing="0">
                            <tr>
                                <td align="center"><span style="color:white" class="subtitulo" runat="server" id="span1">Detalhes do produto adicional selecionado</span></td>
                                <td align="right"><asp:ImageButton ToolTip="fechar" ImageUrl="~/images/close.png" runat="server" Visible="true" ID="cmdOcultarAdicionalFaixa" OnClick="cmdOcultarAdicionalFaixa_Click" EnableViewState="false" /></td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:GridView ID="gridAdicionalFaixa" Width="100%" DataKeyNames="ID" runat="server" AutoGenerateColumns="False" SkinID="gridViewSkin">
                                        <Columns>
                                            <asp:BoundField DataField="strVigencia" HeaderText="Vigência">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="FaixaEtaria" HeaderText="Faixa Etária">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataFormatString="{0:C}" DataField="Valor" HeaderText="Valor (R$)">
                                                <HeaderStyle HorizontalAlign="center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:ButtonField Visible="false" CommandName="editar" Text="editar">
                                                <HeaderStyle HorizontalAlign="Center" />
                                            </asp:ButtonField>
                                            <asp:ButtonField Visible="false" CommandName="excluir" Text="excluir">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle Width="1%" />
                                            </asp:ButtonField>
                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                        <asp:Button Text="novo" SkinID="botaoPequeno" EnableViewState="false" ID="cmdNovoAdicional" OnClick="cmdNovoAdicional_Click" runat="server" />
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlDetalheAdicional" Visible="false">
                        <table width="300"cellpadding="3" cellspacing="0">
                            <tr>
                                <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo" runat="server" id="span2">Detalhes do adicional</span></td>
                                <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdFecharAdicional" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdFecharAdicional_Click" /></td>
                            </tr>
                        </table>
                        <table cellpadding="2" cellspacing="0" width="300" style="border:solid 1px #507CD1">
                            <tr>
                                <td class="tdNormal1" width="5%">&nbsp;Tipo</td>
                                <td class="tdNormal1">
                                    <asp:DropDownList ID="cboAdicionalTipo" Width="99%" SkinID="dropdownSkin" runat="server">
                                        <asp:ListItem Text="Normal" Value="3" Selected="True" />
                                        <asp:ListItem Text="Previdência" Value="2" />
                                        <asp:ListItem Text="Seguro" Value="1" />
                                        <asp:ListItem Text="Taxa" Value="0" />
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdNormal1" width="5%">&nbsp;Descrição</td>
                                <td class="tdNormal1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="98%" runat="server" ID="txtAdicionalDescricao" MaxLength="180" /></td>
                            </tr>
                            <tr>
                                <td class="tdNormal1" width="5%">&nbsp;Cód. Tit.</td>
                                <td class="tdNormal1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="38%" runat="server" ID="txtAdicionalCodigoTitular" MaxLength="30" /></td>
                            </tr>
                            
                            <tr>
                                <td colspan="2" class="tdNormal1"><asp:CheckBox ID="chkDental" Text="Tipo dental" runat="server" SkinID="checkboxSkin" /></td>
                            </tr>
                            <tr>
                                <td colspan="2" class="tdNormal1"><asp:CheckBox ID="chkAdicionalTodaProposta" Text="vale para todos os beneficiários da proposta" runat="server" SkinID="checkboxSkin" Checked="false" Enabled="false" /></td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="0" cellspacing="0" width="300" >
                            <tr>
                                <td>
                                    <asp:GridView ID="gridAdicionalFaixaNovo" SkinID="gridViewSkin" Width="100%" runat="server" 
                                        AutoGenerateColumns="False" DataKeyNames="ID,AdicionalId" 
                                        onrowdatabound="gridAdicionalFaixaNovo_RowDataBound" onrowcommand="gridAdicionalFaixaNovo_RowCommand">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Vigência">
                                                <ItemTemplate>
                                                    <asp:TextBox CssClass="textbox" Width="60" ID="txtVigencia" runat="server" Text='<%# Bind("strVigencia") %>' onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Idade início">
                                                <ItemTemplate>
                                                    <asp:TextBox CssClass="textbox" Width="25" ID="txtIdadeInicio" runat="server" Text='<%# Bind("IdadeInicio") %>' />
                                                    <cc1:MaskedEditExtender Mask="999" runat="server" ID="meeIdadeInicio" TargetControlID="txtIdadeInicio" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Idade fim">
                                                <ItemTemplate>
                                                    <asp:TextBox CssClass="textbox" Width="25" ID="txtIdadeFim" runat="server" Text='<%# Bind("IdadeFim") %>' />
                                                    <cc1:MaskedEditExtender Mask="999" runat="server" ID="meeIdadeFim" TargetControlID="txtIdadeFim" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Valor">
                                                <ItemTemplate>
                                                    <asp:TextBox CssClass="textbox" Width="50" ID="txtQC" runat="server" Text='<%# Bind("Valor") %>' />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir">
                                                <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                            </asp:ButtonField>
                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="tdPrincipal1" height="27" style="padding-right:2px">
                                    <div style="text-align:center; width: 180px; height:20px; border:solid 1px #EFF3FB;">
                                        <asp:Button Width="155" runat="server" ID="cmdAddItem" SkinID="botaoAzul" Text="adicionar item à tabela" BorderWidth="0px" OnClick="cmdAddItem_Click" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="0" cellspacing="0" width="300">
                            <tr>
                                <td align="right" height="27" class="tdPrincipal1" style="padding-right:2px" ><div style="width: 180px; height:20px; border:solid 1px #EFF3FB;"><asp:Image ID="Image2" ImageUrl="~/images/save.png" EnableViewState="false" runat="server" /><asp:Button Width="155" runat="server" ID="cmdSalvarAdicional" SkinID="botaoAzul" style="cursor: pointer;" Text="salvar produto adicional" BorderWidth="0px" OnClick="cmdSalvarAdicional_Click" /><div></div></td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>

        <cc1:TabPanel Visible="true" BorderStyle="None" ID="p4" runat="server">
            <HeaderTemplate><font runat="server" id="titP4" color="black">Planos</font></HeaderTemplate>
            <ContentTemplate>
                <asp:UpdatePanel runat="server" ID="upPlanos" UpdateMode="Conditional">
                <ContentTemplate>
                
                    <cc1:CollapsiblePanelExtender ID="expanelPlanos" runat="Server" EnableViewState="false"
                        TargetControlID="pnlTopoPlanos"
                        Collapsed="false"
                        ExpandControlID="lnkpanelPlanos"
                        CollapseControlID="lnkpanelPlanos"
                        AutoCollapse="False"
                        AutoExpand="false" 
                        ExpandDirection="Vertical" SuppressPostBack="true" />
                    
                    <cc1:CollapsiblePanelExtender ID="expanelTValores" runat="Server" EnableViewState="false"
                        TargetControlID="pnlTopoTValores"
                        Collapsed="true"
                        ExpandControlID="lnkpanelTValores"
                        CollapseControlID="lnkpanelTValores"
                        AutoCollapse="False"
                        AutoExpand="false" 
                        ExpandDirection="Vertical" SuppressPostBack="true" />
                    
                    <%--<cc1:CollapsiblePanelExtender ID="expanelTReajustes" runat="Server" EnableViewState="false"
                        TargetControlID="pnlTopoTReajustes"
                        Collapsed="true"
                        ExpandControlID="lnkpanelTReajustes"
                        CollapseControlID="lnkpanelTReajustes"
                        AutoCollapse="False"
                        AutoExpand="false" 
                        ExpandDirection="Vertical" SuppressPostBack="true" />--%>
                    
                    <cc1:CollapsiblePanelExtender ID="expanelPlanosAdicionais" runat="Server" EnableViewState="false"
                        TargetControlID="pnlTopoPlanoAdicionais"
                        Collapsed="true"
                        ExpandControlID="lnkpanelTAdicionaisPlano"
                        CollapseControlID="lnkpanelTAdicionaisPlano"
                        AutoCollapse="False"
                        AutoExpand="false" 
                        ExpandDirection="Vertical" SuppressPostBack="true" />
                
                    <table width="99%" border="0">
                        <tr>
                            <td valign="top" width="150">
                                <asp:TextBox style="display:none" runat="server" ID="hiddenPlanosMnu" />
                                <table width="95%" cellpadding="3" cellspacing="2" bgcolor="#EFF3FB">
                                    <tr>
                                        <td style="border: solid 1px #507CD1"><asp:LinkButton ForeColor="#507CD1" Font-Size="10pt" OnClientClick="planosSubMnu('1');" runat="server" EnableViewState="false" Text="Planos" ID="lnkpanelPlanos"/></td>
                                    </tr>
                                    <tr>
                                        <td style="border: solid 1px #507CD1"><asp:LinkButton ForeColor="#507CD1" Font-Size="10pt" OnClientClick="planosSubMnu('2');" runat="server" EnableViewState="false" Text="Tabelas de valores" ID="lnkpanelTValores"/></td>
                                    </tr>
                                    <%--<tr>
                                        <td style="border: solid 1px #507CD1"><asp:LinkButton ForeColor="#507CD1" Font-Size="10pt" OnClientClick="planosSubMnu('3');" runat="server" EnableViewState="false" Text="Tabelas de reajustes" ID="lnkpanelTReajustes"/></td>
                                    </tr>--%>
                                    <tr>
                                        <td style="border: solid 1px #507CD1"><asp:LinkButton ForeColor="#507CD1" Font-Size="10pt" OnClientClick="planosSubMnu('4');" runat="server" EnableViewState="false" Text="Adicionais por plano" ID="lnkpanelTAdicionaisPlano"/></td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top">
                                <asp:Panel runat="server" ID="pnlTopoPlanos">
                                    <font style="font-size:13px" color='black'>Planos da operadora<asp:Literal runat="server" ID="litNenhumPlano" /></font>
                                    <asp:Panel runat="server" ID="pnlPlanoLista">
                                        <br />
                                        <table width="530px" style="border:solid 1px #507CD1" cellpadding="4" cellspacing="0">
                                            <tr>
                                                <td class="tdNormal1" width="60">Contrato</td>
                                                <td class="tdNormal1"><asp:DropDownList ID="cboPlanoContrato" AutoPostBack="true" OnSelectedIndexChanged="cboPlanoContrato_OnSelectedIndexChanged" SkinID="dropdownSkin" Width="256px"  runat="server" /></td>
                                            </tr>
                                        </table>
                                        <asp:GridView DataKeyNames="ID,Ativo" Font-Size="10px" Width="530px" ID="gridPlano" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" BorderWidth="1px" BorderColor="#507CD1" GridLines="None" OnRowCommand="gridPlano_OnRowCommand" OnRowDataBound="gridPlano_OnRowDataBound" >
                                            <RowStyle BackColor="#EFF3FB" Font-Size="10px" />
                                            <Columns>
                                                <asp:BoundField DataField="Descricao" HeaderText="Plano">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Codigo" HeaderText="QC Código">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="SubPlano" HeaderText="QC Sub-plano">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                
                                                <asp:BoundField DataField="CodigoParticular" HeaderText="QP Código">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="SubPlanoParticular" HeaderText="QP Sub-plano">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                
                                                <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' align='middle' title='excluir' alt='excluir' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                                <asp:ButtonField Visible="true" CommandName="inativar" Text="inativar">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle width="1%" />
                                                </asp:ButtonField>
                                                <asp:ButtonField CommandName="editar" Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />">
                                                        <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                        <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                                </asp:ButtonField>
                                            </Columns>
                                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                            <EditRowStyle BackColor="#2461BF" />
                                            <AlternatingRowStyle  Font-Size="10px" BackColor="White" />
                                        </asp:GridView>
                                        <br />
                                        <asp:Button runat="server" SkinID="botaoPequeno" ID="cmdNovoPlano" Visible="true" Text="Novo" OnClick="cmdNovoPlano_Click" />
                                    </asp:Panel>
                                    <asp:Panel runat="server" ID="pnlPlanoDetalhe" Visible="false">
                                        <br />
                                        <table width="350"cellpadding="3" cellspacing="0">
                                            <tr>
                                                <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo" runat="server" id="span4">Detalhes do plano</span></td>
                                                <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdPlanoFechar" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdPlanoFechar_Click" /></td>
                                            </tr>
                                        </table>
                                        <table cellpadding="3" cellspacing="0" width="350" style="border: solid 1px #507CD1">
                                            <tr><td class="tdNormal1" colspan="2" height="4"></td></tr>
                                            <tr>
                                                <td class="tdNormal1" width="62px">Contrato</td>
                                                <td class="tdNormal1"><asp:DropDownList Width="233px" runat="server" SkinID="dropdownSkin" ID="cboPlanoContratoDETALHE" /></td>
                                            </tr>
                                            <tr>
                                                <td class="tdNormal1" width="62px">Descrição</td>
                                                <td class="tdNormal1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="225px" runat="server" ID="txtPlanoDescricao" MaxLength="180" /></td>
                                            </tr>
                                            <tr>
                                                <td class="tdNormal1" width="62px"><br /></td>
                                                <td class="tdNormal1"></td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" class="tdNormal1">
                                                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td width="135"><asp:CheckBox EnableViewState="false" runat="server" SkinID="checkboxSkin" ID="chkQuartoComum" Checked="true" Text="Quarto coletivo" /></td>
                                                            <td>Código</td>
                                                            <td><asp:TextBox EnableViewState="false" CssClass="textbox" Width="80px" runat="server" ID="txtPlanoCodigo" MaxLength="50" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td></td>
                                                            <td>Sub-plano</td>
                                                            <td><asp:TextBox EnableViewState="false" CssClass="textbox" Width="80px" runat="server" ID="txtPlanoSubPlano" MaxLength="50" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td></td>
                                                            <td>Cód. ANS</td>
                                                            <td><asp:TextBox EnableViewState="false" CssClass="textbox" Width="80px" runat="server" ID="txtPlanoAnsComum" MaxLength="20" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td></td>
                                                            <td>Início</td>
                                                            <td>
                                                                <asp:TextBox EnableViewState="false" CssClass="textbox" Width="80px" runat="server" ID="txtPlanoQCInicio" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" class="tdNormal1">
                                                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td width="135"><asp:CheckBox EnableViewState="false" runat="server" SkinID="checkboxSkin" ID="chkQuartoParticular" Checked="true" Text="Quarto Particular" /></td>
                                                            <td>Código</td>
                                                            <td><asp:TextBox EnableViewState="false" CssClass="textbox" Width="80px" runat="server" ID="txtPlanoCodigoParticular" MaxLength="50" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td></td>
                                                            <td>Sub-plano</td>
                                                            <td><asp:TextBox EnableViewState="false" CssClass="textbox" Width="80px" runat="server" ID="txtPlanoSubPlanoParticular" MaxLength="50" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td></td>
                                                            <td>Cód. ANS</td>
                                                            <td><asp:TextBox EnableViewState="false" CssClass="textbox" Width="80px" runat="server" ID="txtPlanoAnsParticular" MaxLength="20" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td></td>
                                                            <td>Início</td>
                                                            <td>
                                                                <asp:TextBox EnableViewState="false" CssClass="textbox" Width="80px" runat="server" ID="txtPlanoQPInicio" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="3"><asp:CheckBox EnableViewState="false" runat="server" SkinID="checkboxSkin" ID="chkPlanoAtivo" Checked="true" Text="Plano ativo" /></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="tdNormal1" width="62px"><br /></td>
                                                <td class="tdNormal1"></td>
                                            </tr>
                                            <tr>
                                                <td valign="top" class="tdNormal1">Características</td>
                                                <td class="tdNormal1">
                                                    <asp:TextBox ID="txtPlanoCaracteristicas" Height="55" TextMode="MultiLine" Width="225px" EnableViewState="false" SkinID="textboxSkin" runat="server" />
                                                </td>
                                            </tr>
                                            <tr><td class="tdNormal1" colspan="2" height="3"></td></tr>
                                        </table>
                                        <table cellpadding="0" cellspacing="0" width="350">
                                            <tr>
                                                <td align="right" height="27" class="tdPrincipal1" style="padding-right:2px" ><div style="width: 98px; height:20px; border:solid 1px #EFF3FB;"><asp:Image ID="Image3" ImageUrl="~/images/save.png" EnableViewState="false" runat="server" /><asp:Button EnableViewState="false" style="cursor: pointer;" Width="78" runat="server" ID="cmdPlanoSalvar" SkinID="botaoAzul" Text="salvar plano" BorderWidth="0px" OnClick="cmdSalvarPlano_Click" /></div></td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </asp:Panel>
                                <asp:Panel runat="server" ID="pnlTopoTValores">
                                    <font style="font-size:13px" color='black'>Tabelas de valores<asp:Literal runat="server" ID="litNenhumaTabelaValor" /></font><br /><br />
                                    <asp:Panel ID="pnlTabelaValorLista" runat="server">
                                        <table style="border:solid 1px #507CD1" cellpadding="4" cellspacing="0" width="90%">
                                            <tr>
                                                <td class="tdNormal1" width="60">Contrato</td>
                                                <td class="tdNormal1"><asp:DropDownList OnSelectedIndexChanged="cboTValoresContrato_OnSelectedIndexChanged" AutoPostBack="true" CssClass="textbox" Width="256px" runat="server" ID="cboTValoresContrato" /></td>
                                            </tr>
                                        </table>
                                        <asp:GridView ID="gridTabelaValores" SkinID="gridViewSkin" 
                                            OnRowCommand="gridTabelaValores_OnRowCommand" 
                                            OnRowDataBound="gridTabelaValores_OnRowDataBound" Font-Size="10px" DataKeyNames="ID,Corrente" Width="90%" runat="server" AutoGenerateColumns="false">
                                            <Columns>
                                                <asp:BoundField Visible="false" DataField="ID" HeaderText="Código">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Descricao" HeaderText="Tabela">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Corrente" HeaderText="Atual" Visible="false">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Inicio" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Adm.Início">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Fim" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Adm.Fim">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="VencimentoInicioStr" HeaderText="Vencto.Início">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="VencimentoFimStr" HeaderText="Vencto.Fim">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>

                                                <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' align='middle' title='excluir' alt='excluir' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                                <asp:ButtonField Visible="false" CommandName="detalhes" Text="<img src='images/detail2.png' align='middle' title='detalhes' alt='detalhes' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                                <asp:ButtonField Visible="true" CommandName="duplicar" Text="<img src='images/duplicar.png' align='middle' title='duplicar' alt='duplicar' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                                <asp:ButtonField CommandName="editar" Text="<img src='images/edit.png' align='middle' title='editar' alt='editar' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                                <asp:ButtonField CommandName="recalcular" Text="<img src='images/refresh.png' align='middle' title='recalcular vencimentos' alt='recalcular vencimentos' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                            </Columns>
                                        </asp:GridView>
                                        <br />
                                        <asp:Button EnableViewState="false" runat="server" SkinID="botaoPequeno" ID="cmdNovaTabelaValor" Visible="true" Text="Nova" OnClick="cmdNovaTabelaValor_Click" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlTabelaValorDetalhe" runat="server" Visible="false">
                                        <table width="70%"cellpadding="2" cellspacing="0">
                                            <tr>
                                                <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo" runat="server" id="span5">Detalhes da tabela de valores</span></td>
                                                <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdTabelaValorFechar" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdTabelaValorFechar_Click" /></td>
                                            </tr>
                                        </table>
                                        <table border="0" style="border-left:solid 1px #507CD1;border-right:solid 1px #507CD1;border-top:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="70%">
                                            <tr>
                                                <td width="55px" class="tdNormal1">Contrato</td>
                                                <td class="tdNormal1" colspan="3"><asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboTValoresContratoDETALHE" AutoPostBack="true" OnSelectedIndexChanged="cboTValoresContratoDETALHE_OnSelectedIndexChanged" Width="234" /> </td>
                                            </tr>
                                            <tr>
                                                <td class="tdNormal1" width="55px">Início</td>
                                                <td class="tdNormal1" width="1%">
                                                    <asp:TextBox EnableViewState="true" CssClass="textbox" Width="67px" runat="server" ID="txtTValoresInicio" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                </td>
                                                <td class="tdNormal1" width="4%">Fim</td>
                                                <td class="tdNormal1">
                                                    <asp:TextBox EnableViewState="true" CssClass="textbox" Width="67px" runat="server" ID="txtTValoresFim" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                </td>
                                            </tr>
                                        </table>
                                        <table style="border-bottom:solid 1px #507CD1;border-left:solid 1px #507CD1;border-right:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="70%">
                                            <tr>
                                                <td width="48" class="tdNormal1">% Over</td>
                                                <td width="65" class="tdNormal1"><asp:TextBox SkinID="textboxSkin" Width="67px" runat="server" ID="txtOverPrice" MaxLength="10" /></td>
                                                <td width="45" class="tdNormal1">R$ Fixo</td>
                                                <td width="70" class="tdNormal1"><asp:TextBox SkinID="textboxSkin" Width="67px" runat="server" ID="txtFixo" MaxLength="10" /></td>
                                                <td width="55" class="tdNormal1">R$ Tarifa</td>
                                                <td class="tdNormal1"><asp:TextBox SkinID="textboxSkin" Width="67px" runat="server" ID="txtValorEmbutido" MaxLength="10" />&nbsp;<asp:CheckBox ID="chkValorEmbutido" Text="embutida" runat="server" SkinID="checkboxSkin" /></td>
                                            </tr>
                                        </table>
                                        <asp:Literal runat="server" EnableViewState="false" ID="litTVSemPlano" />
                                        <asp:Panel ID="pnlTVPlanos" runat="server" Visible="false">
                                            <asp:GridView ID="gridTVPlanos" SkinID="gridViewSkin" width="70%" runat="server" AutoGenerateColumns="False" DataKeyNames="ID" OnRowCommand="gridTVPlanos_RowCommand">
                                                <Columns>
                                                    <asp:BoundField DataField="Descricao" HeaderText="Planos">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="strDatasInicio" HeaderText="Início">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:ButtonField Text="<img src='../images/detail2.png' title='selecionar' alt='selecionar' border='0' />" CommandName="selecionar" >
                                                        <ItemStyle Width="1%" />
                                                    </asp:ButtonField>
                                                </Columns>
                                            </asp:GridView>
                                            <br />
                                        </asp:Panel>
                                        <asp:Panel ID="pnlTabelaValorITENS" runat="server" Visible="false">
                                            <asp:Panel ID="pnlTabelaValorITENS_ListaPlanos" EnableViewState="true" runat="server">
                                                <asp:GridView ID="gridTabelaValoresItemDETALHE" SkinID="gridViewSkin" Width="70%" 
                                                    AutoGenerateColumns="False" DataKeyNames="ID,CalculoAutomatico"  runat="server" Visible="true"
                                                    onrowdatabound="gridTabelaValoresItemDETALHE_RowDataBound" onrowcommand="gridTabelaValoresItemDETALHE_RowCommand">
                                                    <Columns>

                                                        <asp:TemplateField HeaderText="Idade início">
                                                            <ItemTemplate>
                                                                <asp:TextBox CssClass="textbox" Width="25" ID="txtIdadeInicio" runat="server" Text='<%# Bind("IdadeInicio") %>' />
                                                                <cc1:MaskedEditExtender Mask="999" runat="server" ID="meeIdadeInicio" TargetControlID="txtIdadeInicio" />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Idade fim">
                                                            <ItemTemplate>
                                                                <asp:TextBox CssClass="textbox" Width="25" ID="txtIdadeFim" runat="server" Text='<%# Bind("IdadeFim") %>' />
                                                                <cc1:MaskedEditExtender Mask="999" runat="server" ID="meeIdadeFim" TargetControlID="txtIdadeFim" />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="QC Operadora">
                                                            <ItemTemplate>
                                                                <asp:TextBox CssClass="textbox" Width="50" ID="txtQCPg" runat="server" Text='<%# Bind("QCValorPagamentoSTR") %>' />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="QP Operadora">
                                                            <ItemTemplate>
                                                                <asp:TextBox CssClass="textbox" Width="50" ID="txtQPPg" runat="server" Text='<%# Bind("QPValorPagamentoSTR") %>' />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="QC Cliente">
                                                            <ItemTemplate>
                                                                <asp:TextBox BackColor="lightgray" ReadOnly="true" CssClass="textbox" Width="50" ID="txtQC" runat="server" Text='<%# Bind("QCValorSTR") %>' />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="QP Cliente">
                                                            <ItemTemplate>
                                                                <asp:TextBox BackColor="lightgray" ReadOnly="true" CssClass="textbox" Width="50" ID="txtQP" runat="server" Text='<%# Bind("QPValorSTR") %>' />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>

                                                        <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir">
                                                            <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                            <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                                        </asp:ButtonField>
                                                        <asp:BoundField Visible="false" DataField="TabelaID" />

                                                        <asp:ButtonField ButtonType="Image" ImageUrl="~/images/cadeado.png" CommandName="calculo">
                                                            <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                            <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                                        </asp:ButtonField>
                                                    </Columns>
                                                </asp:GridView>
                                                <table cellpadding="0" cellspacing="0" width="70%">
                                                    <tr>
                                                        <td align="right" class="tdPrincipal1" height="27" style="padding-right:2px" >
                                                            <div style="text-align:center; width: 153px; height:20px; border:solid 1px #EFF3FB;">
                                                                <asp:Button Width="143px" runat="server" ID="cmdAddItemTabelaValor" EnableViewState="false" SkinID="botaoAzul" Text="adicionar item à tabela" BorderWidth="0px" OnClick="cmdAddItemTabelaValor_Click" style="cursor: pointer;" />
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                            <br />
                                            <table cellpadding="0" cellspacing="0" width="70%">
                                                <tr>
                                                    <td align="right" style="padding-right:2px" height="27" class="tdPrincipal1">
                                                        <div style="width: 153px; height:20px; border:solid 1px #EFF3FB;">
                                                            <asp:Image style="cursor: pointer;" ID="Image4" ImageUrl="~/images/save.png" EnableViewState="false" runat="server" />
                                                            <asp:Button EnableViewState="false" Width="134" runat="server" ID="cmdSalvarTabelaValor" SkinID="botaoAzul" Text="salvar tabela" BorderWidth="0px" OnClick="cmdSalvarTabelaValor_Click" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlTabelaValorDuplicar" runat="server" Visible="false">
                                        <table width="74%"cellpadding="2" cellspacing="0">
                                            <tr>
                                                <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo">Duplicar tabela de valor</span></td>
                                                <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdTVDuplicarFechar" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdTVDuplicarFechar_Click" /><asp:Literal ID="litTabelaValorDuplicarIDs" runat="server" EnableViewState="true" Visible="false" /></td>
                                            </tr>
                                        </table>
                                        <table style="border-left:solid 1px #507CD1;border-right:solid 1px #507CD1;border-top:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="74%">
                                            <tr>
                                                <td width="55" class="tdNormal1">Início</td>
                                                <td width="80" class="tdNormal1">
                                                    <asp:TextBox EnableViewState="true" CssClass="textbox" Width="67px" runat="server" ID="txtTValoresInicioDUPLICAR" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                </td>
                                                <td width="40" class="tdNormal1">Fim</td>
                                                <td width="70" class="tdNormal1">
                                                    <asp:TextBox EnableViewState="true" CssClass="textbox" Width="67px" runat="server" ID="txtTValoresFimDUPLICAR" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                </td>
                                                <td width="65" class="tdNormal1">% Reajuste</td>
                                                <td class="tdNormal1"><asp:TextBox EnableViewState="false" SkinID="textboxSkin" Width="67px" runat="server" ID="txtReajusteDuplicar" MaxLength="10" /></td>
                                            </tr>
                                        </table>
                                        <table style="border-left:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="74%">
                                            <tr>
                                                <td width="55" class="tdNormal1">% Over</td>
                                                <td width="80" class="tdNormal1"><asp:TextBox SkinID="textboxSkin" Width="67px" runat="server" ID="txtOverPriceDuplicar" MaxLength="10" /></td>
                                                <td width="40" class="tdNormal1">R$ Fixo</td>
                                                <td width="70" class="tdNormal1"><asp:TextBox SkinID="textboxSkin" Width="67px" runat="server" ID="txtFixoDuplicar" MaxLength="10" /></td>
                                                <td width="65" class="tdNormal1">R$ Tarifa</td>
                                                <td class="tdNormal1"><asp:TextBox SkinID="textboxSkin" Width="67px" runat="server" ID="txtValorEmbutidoDuplicar" MaxLength="10" />&nbsp;<asp:CheckBox ID="chkValorEmbutidoDuplicar" Text="embutida" runat="server" SkinID="checkboxSkin" /></td>
                                            </tr>
                                        </table>
                                        <br />
                                        <table cellpadding="0" cellspacing="0" width="74%">
                                            <tr>
                                                <td align="right" style="padding-right:2px" height="27" class="tdPrincipal1">
                                                    <div style="width: 175px; height:20px; border:solid 1px #EFF3FB;">
                                                        <asp:Image style="cursor: pointer;" ID="Image9" ImageUrl="~/images/save.png" EnableViewState="false" runat="server" />
                                                        <asp:Button EnableViewState="false" Width="154" runat="server" ID="cmdDuplicarTabelaValor" SkinID="botaoAzul" Text="duplicar tabela de valor" BorderWidth="0px" OnClick="cmdDuplicarTabelaValor_Click" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </asp:Panel>
                                <asp:Panel ID="pnlTopoTReajustes" EnableViewState="false" runat="server" Visible="false">
                                    <font style="font-size:13px" color='black'>Tabelas de reajuste</font><asp:Literal runat="server" ID="litNenhumaTabelaReajuste" /><br /><br />
                                    <asp:Panel ID="pnlTReajusteLista" runat="server">
                                        <table style="border:solid 1px #507CD1" cellpadding="4" cellspacing="0" width="70%">
                                            <tr>
                                                <td class="tdNormal1" width="60">Contrato</td>
                                                <td class="tdNormal1"><asp:DropDownList OnSelectedIndexChanged="cboTReajusteContrato_OnSelectedIndexChanged" AutoPostBack="true" CssClass="textbox" Width="256px" runat="server" ID="cboTReajusteContrato" /></td>
                                            </tr>
                                        </table>
                                        <asp:GridView Width="70%" DataKeyNames="ID,Corrente" ID="gridTabelaReajustes" OnRowCommand="gridTabelaReajustes_OnRowCommand" OnRowDataBound="gridTabelaReajustes_OnRowDataBound" runat="server" AutoGenerateColumns="False" SkinID="gridViewSkin">
                                            <Columns>
                                                <asp:BoundField Visible="False" DataField="ID" HeaderText="Código">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Descricao" HeaderText="Tabela">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Corrente" HeaderText="Atual">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Data" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Data">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:ButtonField Visible="False" CommandName="alterar" Text="aterar">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                </asp:ButtonField>
                                                <asp:ButtonField CommandName="detalhes" Text="<img src='images/detail2.png' align='middle' title='detalhes' alt='detalhes' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                                <asp:ButtonField CommandName="ativar" Text="<img src='images/active.png' align='middle' title='tornar atual a tabela' alt='tornar atual a tabela' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                                <asp:ButtonField CommandName="editar" Text="<img src='images/edit.png' align='middle' title='editar' alt='editar' border='0' />">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                            </Columns>
                                        </asp:GridView>
                                        <br />
                                        <table visible="False" bgcolor="#507CD1" runat="server" id="tblTabelaReajustesItem" width="40%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td align="center"><span style="color:white" id="spanDetalhesTabelaReajusteSelecionada" runat="server" class="subtitulo">Detalhes da tabela selecionada</span> </td>
                                                <td align="right"><asp:ImageButton EnableViewState="false" ToolTip="fechar" ImageUrl="~/images/close.png" runat="server" ID="cmdOcultarTReajustesDetalhes" OnClick="cmdOcultarTReajustesDetalhes_Click" /></td>
                                            </tr>
                                            <tr >
                                                <td colspan="2">
                                                    <asp:GridView ID="gridTabelaReajustesItem" Width="100%" DataKeyNames="ID" runat="server" AutoGenerateColumns="False" SkinID="gridViewSkin">
                                                        <Columns>
                                                            <asp:BoundField Visible="False" DataField="ID" HeaderText="Código">
                                                                <HeaderStyle HorizontalAlign="Left" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="IdadeInicio" HeaderText="A partir de (anos)">
                                                                <HeaderStyle HorizontalAlign="Center" Wrap="False" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataFormatString="{0:F}" DataField="PercentualReajuste" HeaderText="Reajuste (%)">
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:ButtonField Visible="False" CommandName="editar" Text="editar">
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                            </asp:ButtonField>
                                                            <asp:ButtonField Visible="False" CommandName="excluir" Text="excluir">
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle Width="1%" />
                                                            </asp:ButtonField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:Button EnableViewState="false" runat="server" SkinID="botaoPequeno" ID="cmdNovaTabelaReajuste" Text="Nova" OnClick="cmdNovaTabelaReajuste_Click" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlTReajusteDetalhe" runat="server" Visible="False">
                                        <table width="500"cellpadding="2" cellspacing="0">
                                            <tr>
                                                <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo" runat="server" id="span6">Detalhes da tabela de reajuste</span> </td>
                                                <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdTabelaReajusteFechar" 
                                                        runat="server" EnableViewState="False" ImageUrl="~/images/close.png" 
                                                        ToolTip="fechar" OnClick="cmdTabelaReajusteFechar_Click" /></td>
                                            </tr>
                                        </table>
                                        <table style="border:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="500">
                                            <tr>
                                                <td class="tdNormal1" width="5%">Contrato</td>
                                                <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" Width="245px" runat="server" ID="cboTRContratoDETALHE" /></td>
                                            </tr>
                                            <tr>
                                                <td class="tdNormal1" width="58px">Descrição</td>
                                                <td class="tdNormal1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="240px" runat="server" ID="txtTRDescricao" MaxLength="180" /></td>
                                            </tr>
                                            <tr>
                                                <td class="tdNormal1">Data</td>
                                                <td class="tdNormal1">
                                                    <asp:TextBox EnableViewState="false" CssClass="textbox" Width="67px" runat="server" ID="txtTRData" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:GridView ID="gridTReajusteItemDETALHE" SkinID="gridViewSkin" Width="500px" runat="server" 
                                            AutoGenerateColumns="False" DataKeyNames="ID" 
                                            onrowdatabound="gridTReajusteItemDETALHE_RowDataBound" 
                                            onrowcommand="gridTReajusteItemDETALHE_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Idade início">
                                                    <ItemTemplate>
                                                        <asp:TextBox CssClass="textbox" Width="25" ID="txtIdadeInicio" runat="server" Text='<%# Bind("IdadeInicio") %>' />
                                                        <cc1:MaskedEditExtender Mask="999" runat="server" ID="meeIdadeInicio" TargetControlID="txtIdadeInicio" />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Reajuste (%)">
                                                    <ItemTemplate>
                                                        <asp:TextBox CssClass="textbox" Width="50" ID="txtReajuste" runat="server" Text='<%# Bind("PercentualReajuste") %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir">
                                                    <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                    <ControlStyle Font-Size="11px" ForeColor="#CC0000" />
                                                </asp:ButtonField>
                                            </Columns>
                                        </asp:GridView>
                                        <table cellpadding="0" cellspacing="0" width="500">
                                            <tr>
                                                <td align="right" class="tdPrincipal1" height="27" style="padding-right:2px" >
                                                    <div style="text-align:center; width: 163px; height:20px; border:solid 1px #EFF3FB;">
                                                        <asp:Button Width="143px" runat="server" ID="cmdAddItemTabelaReajuste" EnableViewState="false" SkinID="botaoAzul" Text="adicionar item à tabela" BorderWidth="0px" OnClick="cmdAddItemTabelaReajuste_Click" style="cursor: pointer;" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                        <br />
                                        <table cellpadding="0" cellspacing="0" width="500">
                                            <tr>
                                                <td align="right" class="tdPrincipal1" height="27" style="padding-right:2px" >
                                                    <div style="width: 163px; height:20px; border:solid 1px #EFF3FB;">
                                                        <asp:Image ID="Image5" ImageUrl="~/images/save.png" EnableViewState="false" runat="server" />
                                                        <asp:Button EnableViewState="false" Width="143px" runat="server" ID="cmdSalvarTabelaReajuste" SkinID="botaoAzul" Text="salvar tabela de reajuste" BorderWidth="0px" OnClick="cmdSalvarTabelaReajuste_Click" style="cursor: pointer;" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </asp:Panel>
                                <asp:Panel ID="pnlTopoPlanoAdicionais" runat="server">
                                    <font style="font-size:13px" color='black'>Produtos adicionais por plano</font>
                                    <br /><br />
                                    <table style="border: solid 1px #507CD1" cellpadding="3" cellspacing="0" width="390">
                                        <tr>
                                            <td class="tdNormal1" width="58px">Contrato</td>
                                            <td class="tdNormal1"><asp:DropDownList ID="cboPlanoAdicional_Contrato" OnSelectedIndexChanged="cboPlanoAdicional_Contrato_OnSelectedIndexChanged" AutoPostBack="true" CssClass="textbox" Width="325" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td class="tdNormal1" width="58px">Plano</td>
                                            <td class="tdNormal1"><asp:DropDownList ID="cboPlanoAdicional_Plano" OnSelectedIndexChanged="cboPlanoAdicional_Plano_OnSelectedIndexChanged" AutoPostBack="true" CssClass="textbox" Width="100%" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td class="tdNormal1" width="58px">Adicional</td>
                                            <td class="tdNormal1"><asp:DropDownList ID="cboPlanoAdicional_Adicional" SkinID="dropdownSkin" Width="100%" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" align="right" height="27" class="tdPrincipal1">
                                                <asp:Image ID="Image6" ImageUrl="~/images/save.png" EnableViewState="false" runat="server" Visible="false" />
                                                <asp:Button EnableViewState="false" Width="137px" runat="server" ID="cmdAddAdicional" SkinID="botaoAzulBorda" Text="adicionar produto" style="cursor: pointer;" BorderWidth="0px" OnClick="cmdAddAdicional_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                    <asp:GridView ID="gridPlanoAdicional" SkinID="gridViewSkin" Width="390" 
                                        AutoGenerateColumns="False" DataKeyNames="ID"  runat="server"
                                        onrowdatabound="gridPlanoAdicional_RowDataBound" onrowcommand="gridPlanoAdicional_RowCommand">
                                        <Columns>
                                            <asp:BoundField DataField="AdicionalDescricao" HeaderText="Adicional">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>

                                            <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir">
                                                <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                            </asp:ButtonField>
                                            <asp:BoundField Visible="false" DataField="TabelaID" />
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                    

                </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>

        <cc1:TabPanel Visible="true" BorderStyle="None" ID="p5" runat="server">
            <HeaderTemplate><font runat="server" id="titP5" color="black">Calendário</font></HeaderTemplate>
            <ContentTemplate>
                <table>
                    <tr>
                        <td><font style="font-size:13px" color='black'>Calendário da operadora</font></td>
                    </tr>
                </table>
                <br />
                <table cellpadding="3" cellspacing="0" style="border:solid 1px #507CD1" width="550">
                    <tr>
                        <td bgcolor='#EFF3FB'>
                            <asp:CheckBox ID="chkPermiteReativacao" Font-Bold="true" Text="Permite reativação" SkinID="checkboxSkin" runat="server" />
                            &nbsp;&nbsp;
                            <asp:CheckBox ID="chkEnviaCarta" Font-Bold="true" Text="Envia carta de cobrança" SkinID="checkboxSkin" runat="server" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:UpdatePanel runat="server" ID="upCalendario" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table cellpadding="3" cellspacing="0">
                            <tr>
                                <td height="29" colspan="9" style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1;border-left:solid 1px #507CD1">
                                    <b>Contrato</b>&nbsp;
                                    <asp:DropDownList ID="cboCalendarioContrato" AutoPostBack="true" OnSelectedIndexChanged="cboPlanoCalendario_OnSelectedIndexChanged" SkinID="dropdownSkin" Width="256px"  runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td bgcolor='#EFF3FB' style="border-left:solid 1px #507CD1;border-right:solid 1px #507CD1;border-top:solid 1px #507CD1" colspan="4" align="center"><b>Admissão</b></td>
                                <td style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1" colspan="2" align="center"><b>Vigência</b></td>
                                <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1" colspan="2" align="center"><b>Data</b></td>
                                <td style="border-left:solid 1px #507CD1;border-top:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" rowspan="4" valign="middle" align="center"><asp:Button ID="cmdCalendarioSalvar" Text="salvar" SkinID="botaoPequeno" runat="server" OnClick="cmdCalendarioSalvar_Click" /></td>
                            </tr>
                            <tr>
                                <td bgcolor='#EFF3FB' style="border-left:solid 1px #507CD1;border-bottom:solid 1px #507CD1"><asp:DropDownList ID="cboAdmissaoDe_DIA" runat="server" SkinID="dropdownSkin"/></td>
                                <td bgcolor='#EFF3FB' style="border-bottom:solid 1px #507CD1;"><asp:DropDownList ID="cboAdmissaoDe_Tipo" runat="server" SkinID="dropdownSkin" Width="122" /></td>
                                <td bgcolor='#EFF3FB' style="border-bottom:solid 1px #507CD1;">a <asp:DropDownList ID="cboAdmissaoAte_DIA" runat="server" SkinID="dropdownSkin" /></td>
                                <td bgcolor='#EFF3FB' style="border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1"><asp:DropDownList ID="cboAdmissaoAte_Tipo" runat="server" SkinID="dropdownSkin" /></td>
                                <td style="border-bottom:solid 1px #507CD1;"><asp:DropDownList ID="cboVigencia" runat="server" SkinID="dropdownSkin"/></td>
                                <td style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:DropDownList ID="cboVigencia_Tipo" runat="server" SkinID="dropdownSkin"/></td>
                                <td bgcolor='#EFF3FB' style="border-bottom:solid 1px #507CD1;">
                                    <asp:TextBox ID="txtCalendarioData" runat="server" SkinID="textboxSkin" Width="55" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <asp:DataList CellPadding="0" CellSpacing="0" ID="dlCalendario" DataKeyField="ID" runat="server" OnItemCommand="dlCalendario_ItemCommand">
                            <HeaderTemplate>
                                <table cellpadding="1" cellspacing="0" width="700" >
                                    <tr>
                                        <td bgcolor='#EFF3FB' style="border-left:solid 1px #507CD1;border-right:solid 1px #507CD1;border-top:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center"><b>Admissão</b></td>
                                        <td style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center"><b>Vigência</b></td>
                                        <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center"><b>Data</b></td>

                                        <td style="border-bottom:solid 1px #507CD1">&nbsp;</td>
                                        <td style="border-bottom:solid 1px #507CD1">&nbsp;</td>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td bgcolor='#EFF3FB' style="border-left:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center">
                                        de <asp:Label ID="lblAdmissaoDe_DIA" Text='<%# DataBinder.Eval(Container.DataItem, "AdmissaoDe_Dia") %>' runat="server" />
                                        do <asp:Label ID="lblAdmissaoDe_Tipo" Text='<%# DataBinder.Eval(Container.DataItem, "StrAdmissaoDe_Tipo") %>' runat="server" />
                                        a <asp:Label ID="lblAdmissaoAte_DIA" Text='<%# DataBinder.Eval(Container.DataItem, "AdmissaoAte_Dia") %>' runat="server" />
                                        do <asp:Label ID="lblAdmissaoAte_Tipo" Text='<%# DataBinder.Eval(Container.DataItem, "StrAdmissaoAte_Tipo") %>' runat="server" />
                                    </td>
                                    <td style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                        no dia <asp:Label ID="lblVigencia" Text='<%# DataBinder.Eval(Container.DataItem, "VigenciaDia") %>' runat="server" />
                                        do <asp:Label ID="lblVigencia_Tipo" Text='<%# DataBinder.Eval(Container.DataItem, "StrVigenciaTipo") %>' runat="server" />
                                    </td>
                                    <td bgcolor='#EFF3FB' style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                        <asp:Label ID="Label2" Text='<%# DataBinder.Eval(Container.DataItem, "strData") %>' runat="server" />
                                    </td>

                                    <td style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                        <asp:ImageButton ImageUrl="~/images/delete.png" OnClientClick="return confirm('Deseja realmente excluir?');" ToolTip="excluir" ID="cmdCalendarioExcluir" CommandName="excluir" runat="server" EnableViewState="false" />
                                    </td>
                                    <td style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                        <asp:ImageButton ImageUrl="~/images/detail2.png" ToolTip="exibir calendário de vencimento" ID="cmdCalendarioExibirVencimento" CommandName="vencimento" runat="server" EnableViewState="true" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:DataList>
                        
                        <asp:Panel ID="pnlCalendarioVencimento" runat="server" Visible="false">
                            <br />
                            <table width="827" cellpadding="3" cellspacing="0">
                                <tr>
                                    <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo" runat="server" id="span9">Novo Calendário de Vencimento</span></td>
                                    <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdFecharCalendarioVencimento" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdFecharCalendarioVencimento_Click" /></td>
                                </tr>
                            </table>
                            <table cellpadding="3" cellspacing="0">
                                <tr>
                                    <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-left:solid 1px #507CD1" align="center" colspan="2"><b>1º Vencimento</b></td>
                                    <td bgcolor='#FFFFFF' style="border-top:solid 1px #507CD1;border-left:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center"><b>Data sem juros</b></td>
                                    <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center" ><b>Data limite</b></td>
                                    <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center" ><b>Data limite 2</b></td>
                                    <td bgcolor='#FFFFFF' style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center"><b>Data</b></td>
                                    <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" rowspan="2" valign="middle" align="center"><asp:Button ID="cmdCalendarioSalvarVencimento" Text="salvar" SkinID="botaoPequeno" runat="server" OnClick="cmdCalendarioVencimentoSalvar_Click" /></td>
                                </tr>
                                <tr>
                                    <td bgcolor='#EFF3FB' style="border-left:solid 1px #507CD1;border-bottom:solid 1px #507CD1;"><asp:DropDownList ID="cboVencimento" runat="server" SkinID="dropdownSkin"/></td>
                                    <td bgcolor='#EFF3FB' style="border-bottom:solid 1px #507CD1;"><asp:DropDownList ID="cboVencimento_Tipo" runat="server" SkinID="dropdownSkin"/></td>
                                    <td bgcolor='#FFFFFF' style="border-left:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center"><asp:DropDownList ID="cboDataSemJuros_DIAS" runat="server" SkinID="dropdownSkin" Width="40" />&nbsp;dia(s) após o vencimento</td>
                                    <td valign="top" bgcolor='#EFF3FB' style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                        <asp:DropDownList ID="cboTipo" runat="server" SkinID="dropdownSkin" AutoPostBack="true" OnSelectedIndexChanged="cboTipo_Changed">
                                            <asp:ListItem Text="Todo dia" Value="1" Selected="True" />
                                            <asp:ListItem Text="Texto personalizado" Value="2" />
                                        </asp:DropDownList>
                                        <asp:DropDownList ID="cboDataLimiteDia" runat="server" SkinID="dropdownSkin" Width="40" />
                                        <asp:TextBox ID="txtTextopersonalizado" TextMode="MultiLine" SkinID="textboxSkin" runat="server" Visible="false" Height="45" Width="240" MaxLength="550" />
                                    </td>
                                    <td valign="top" bgcolor='#EFF3FB' style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                        <asp:DropDownList ID="cboDataLimite2" runat="server" SkinID="dropdownSkin" AutoPostBack="true" OnSelectedIndexChanged="cboTipo_Changed">
                                            <asp:ListItem Text="Nenhum" Value="0" Selected="True" />
                                        </asp:DropDownList>
                                        dias do vencimento
                                    </td>
                                    <td valign="top" bgcolor='#FFFFFF' style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                        <asp:TextBox CssClass="textbox" Width="60" ID="txtDataCalendarioVencimento" runat="server" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <asp:DataList CellPadding="0" CellSpacing="0" ID="dlCalendarioVencimento" DataKeyField="ID" runat="server" OnItemCommand="dlCalendarioVencimento_ItemCommand">
                                <HeaderTemplate>
                                    <table cellpadding="1" cellspacing="0" width="700" >
                                        <tr>
                                            <td bgcolor='#EFF3FB' style="border-left:solid 1px #507CD1;border-top:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center"><b>1º Vencimento</b></td>
                                            
                                            <td style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center"><b>Data sem juros</b></td>
                                            <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center"><b>Data limite</b></td>
                                            <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center"><b>Data limite 2</b></td>
                                            <td style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center"><b>Data</b></td>
                                            
                                            <td style="border-bottom:solid 1px #507CD1">&nbsp;</td>
                                            <td style="border-bottom:solid 1px #507CD1">&nbsp;</td>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td bgcolor='#EFF3FB' style="border-left:solid 1px #507CD1;border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                            no dia <asp:Label ID="lblVencimento" Text='<%# DataBinder.Eval(Container.DataItem, "VencimentoDia") %>' runat="server" />
                                            do <asp:Label ID="lblVencimento_Tipo" Text='<%# DataBinder.Eval(Container.DataItem, "StrVencimentoTipo") %>' runat="server" />
                                        </td>

                                        <td style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                            <asp:Label ID="Label1" Text='<%# DataBinder.Eval(Container.DataItem, "DataSemJuros_Dia") %>' runat="server" /> dia(s) após o vencimento
                                        </td>
                                        <td bgcolor='#EFF3FB' style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                            <asp:Label ID="Label2" Text='<%# DataBinder.Eval(Container.DataItem, "strDataLimite") %>' runat="server" />
                                        </td>
                                        
                                        <td bgcolor='#EFF3FB' style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                            <asp:Label ID="Label4" Text='<%# DataBinder.Eval(Container.DataItem, "strLimiteAposVencimento") %>' runat="server" />
                                        </td>
                                        
                                        <td bgcolor='#FFFFFF' style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                            <asp:Label ID="Label3" Text='<%# DataBinder.Eval(Container.DataItem, "strData") %>' runat="server" />
                                        </td>

                                        <td style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                            <asp:ImageButton ImageUrl="~/images/delete.png" OnClientClick="return confirm('Deseja realmente excluir?');" ToolTip="excluir" ID="cmdCalendarioVencimentoExcluir" CommandName="excluir" runat="server" EnableViewState="false" />
                                        </td>
                                        <td style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                            <asp:ImageButton ImageUrl="~/images/detail2.png" ToolTip="exibir calendário de recebimento" ID="cmdCalendarioVencimentoExibirRecebimento" CommandName="recebimento" runat="server" EnableViewState="true" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:DataList>
                        </asp:Panel>
                    
                        <asp:Panel ID="pnlCalendarioFaturaLista" runat="server" Visible="false">
                            <br />
                            <table width="660" cellpadding="3" cellspacing="0">
                                <tr>
                                    <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo" runat="server" id="span8">Calendário de Recebimento</span></td>
                                    <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdFecharCalendarioRecebimento" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdFecharCalendarioRecebimento_Click" /></td>
                                </tr>
                            </table>
                            <table width="660" cellpadding="3" cellspacing="0" style="border:solid 1px #507CD1">
                                <tr>
                                    <td style="border-bottom:solid 1px #507CD1" colspan="2" align="center" bgcolor="#EFF3FB"><b>Pagto. Fatura</b></td>
                                    <td style="border-bottom:solid 1px #507CD1" colspan="2" align="center" bgcolor="#EFF3FB"><b>Pagto. Comissão</b></td>
                                    <td style="border-bottom:solid 1px #507CD1" colspan="1" align="center" bgcolor="#EFF3FB"><b>Percent. s/ Fatura</b></td>
                                    <td style="border-left:solid 1px #507CD1" rowspan="2" valign="middle" align="center"><asp:Button ID="cmdCalendarioRecebimentoSalvar" Text="salvar" SkinID="botaoPequeno" runat="server" OnClick="cmdCalendarioRecebimentoSalvar_Click" /></td>
                                </tr>
                                <tr>
                                    <td>dia &nbsp;<asp:DropDownList ID="cboFatura_DIA" runat="server" SkinID="dropdownSkin"/></td>
                                    <td> do &nbsp;<asp:DropDownList ID="cboFatura_Tipo" runat="server" SkinID="dropdownSkin"/></td>
                                    <td>dia &nbsp;<asp:DropDownList ID="cboComissao_DIA" runat="server" SkinID="dropdownSkin"/></td>
                                    <td> do &nbsp;<asp:DropDownList ID="cboComissao_Tipo" runat="server" SkinID="dropdownSkin"/></td>
                                    <td align="center"><asp:DropDownList ID="cboComissao_Percentual" runat="server" SkinID="dropdownSkin"/></td>
                                </tr>
                            </table>
                            <br />
                            <asp:DataList ID="dlCalendarioRecebimento" DataKeyField="ID" runat="server" OnItemCommand="dlCalendarioRecebimento_ItemCommand">
                                <HeaderTemplate>
                                    <table cellpadding="1" cellspacing="0" width="660" >
                                        <tr>
                                            <td bgcolor='#EFF3FB' style="border-left:solid 1px #507CD1;border-right:solid 1px #507CD1;border-top:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center"><b>Pgto. Fatura</b></td>
                                            <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center"><b>Pagto. Comissão</b></td>
                                            <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="center"><b>Percent. s/ fatura</b></td>
                                            <td style="border-bottom:solid 1px #507CD1">&nbsp;</td>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1;border-left:solid 1px #507CD1" align="center">
                                            no dia <asp:Label ID="lblFatura" Text='<%# DataBinder.Eval(Container.DataItem, "FaturaDia") %>' runat="server" />
                                            do <asp:Label ID="lblFatura_Tipo" Text='<%# DataBinder.Eval(Container.DataItem, "StrFaturaTipo") %>' runat="server" />
                                        </td>
                                        <td style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                            no dia <asp:Label ID="lblComissao" Text='<%# DataBinder.Eval(Container.DataItem, "ComissaoDia") %>' runat="server" />
                                            do <asp:Label ID="lblComissao_Tipo" Text='<%# DataBinder.Eval(Container.DataItem, "StrComissaoTipo") %>' runat="server" />
                                        </td>
                                        <td style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                            <asp:Label ID="lblComissao_Percentual" Text='<%# DataBinder.Eval(Container.DataItem, "ComissaoPercentual") %>' runat="server" />%
                                        </td>
                                        <td style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                            <asp:ImageButton ImageUrl="~/images/delete.png" OnClientClick="return confirm('Deseja realmente excluir?');" ToolTip="excluir" ID="cmdCalendarioRecebimentoExcluir" CommandName="excluir" runat="server" EnableViewState="false" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:DataList>
                        </asp:Panel>

                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </cc1:TabPanel>

        <cc1:TabPanel ID="p6" runat="server" Visible="false">
            <HeaderTemplate><font runat="server" visible="false" id="titp6" color="black">Parâmetros</font></HeaderTemplate>
            <ContentTemplate>
                <font color='black'>Calendário de recebimento</font><asp:Literal runat="server" ID="Literal1" /><br /><br />
                <table>
                    <tr>
                        <td><b>Dia de pagamento</b></td>
                        <td width="165"><asp:DropDownList runat="server" Width="40" SkinID="dropdownSkin" ID="cboDiaPagamento" /></td>
                        <td><b>Dia de recebimento</b></td>
                        <td><asp:DropDownList runat="server" Width="40" SkinID="dropdownSkin" ID="cboDiaRecebimento" /></td>
                        <td><asp:Button ID="cmdAddCal" Text="+" SkinID="botaoAzul" runat="server" OnClick="cmdAddCal_click" /></td>
                    </tr>
                    <tr>
                        <td colspan="5">
                            <asp:GridView OnRowCommand="grid_OnRowCommand" Width="100%" DataKeyNames="ID" ID="grid" runat="server" AutoGenerateColumns="False" SkinID="gridViewSkin">
                                <Columns>
                                    <asp:BoundField DataField="DiaRecebimento" HeaderText="Dia recebimento">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DiaPagamento" HeaderText="Dia Pagamento">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' align='middle' title='excluir' alt='excluir' border='0' />">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
                <br />
                <table runat="server" visible="false" id="_temp">
                    <tr>
                        <td><b>Tamanho máximo do logradouro do beneficiário</b></td>
                        <td>
                            <asp:TextBox runat="server" Width="40" MaxLength="5" CssClass="textbox" ID="txtLogradouroTamanhoMaximo" />
                            <cc1:MaskedEditExtender Mask="9999" runat="server" ID="meeLogradouroTamanhoMaximo" TargetControlID="txtLogradouroTamanhoMaximo" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </cc1:TabPanel>

    </cc1:TabContainer>
    <br />
    <table width="100%">
        <tr>
            <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
            <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
        </tr>
    </table>

    <asp:UpdatePanel runat="server" ID="upnlAlerta" UpdateMode="Conditional" EnableViewState="false">
        <ContentTemplate>
            <cc1:ModalPopupExtender ID="MPE" runat="server"
                TargetControlID="lnk"
                PopupControlID="pnlAlert"
                BackgroundCssClass="modalBackground" 
                CancelControlID="cmdCloseAlert"
                DropShadow="true"  />
            <asp:Panel runat="server" ID="pnlAlert" EnableViewState="false">
                <asp:LinkButton runat="server" EnableViewState="false" ID="lnk" />
                <table width="350" align="center" bgcolor="gainsboro" style="border:solid 1px black">
                    <tr>
                        <td align="center">
                            <asp:Literal runat="server" ID="litAlert" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr height="8"><td height="8">&nbsp</td></tr>
                    <tr>
                        <td align="center">
                            <input runat="server" style="width:45px;font-size:12px;font-family:Arial" id="cmdCloseAlert" type="button" value="OK" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    
</asp:Content>
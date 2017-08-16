<%@ Page Language="C#" Theme="Theme" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="comissionamentosUsuario.aspx.cs" Inherits="www.comissionamentosUsuario" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"><img src="images/imgTitulos/tabela_comissionario.png" /></td></tr>
        <tr><td><span class="titulo">Configurações de comissionamento</span></td></tr>
        <tr><td><span class="subtitulo">Utilize o filtro abaixo para localizar os agentes comissionáveis"</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <cc1:TabContainer BorderStyle="None" BorderWidth="0" Width="100%" ID="tab" runat="server" ActiveTabIndex="0">
                <cc1:TabPanel runat="server" ID="p1">
                    <HeaderTemplate><font color='black'>Comissionamento do corretor</font></HeaderTemplate>
                    <ContentTemplate>
                        <table cellpadding="2" width="320">
                            <tr>
                                <td class="tdPrincipal1">Filial</td>
                                <td class="tdNormal1"><asp:DropDownList Width="99%" ID="cboFiliais" SkinID="dropdownSkin" AutoPostBack="true" runat="server" onselectedindexchanged="cboFiliais_SelectedIndexChanged" /></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">Agentes</td>
                                <td class="tdNormal1"><asp:DropDownList Width="99%" ID="cboUsuarios" SkinID="dropdownSkin" AutoPostBack="true" runat="server" onselectedindexchanged="cboUsuarios_SelectedIndexChanged" /></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">Operadora</td>
                                <td class="tdNormal1"><asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboOperadora" Width="250" AutoPostBack="True" onselectedindexchanged="cboOperadora_SelectedIndexChanged" /></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">Contrato</td>
                                <td class="tdNormal1"><asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboContrato" Width="250" AutoPostBack="True" onselectedindexchanged="cboContrato_SelectedIndexChanged" /></td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="2" width="322" cellpadding="3" cellspacing="0">
                            <tr><td class="tdPrincipal1" align="center">Tabelas de comissionamento</td></tr>
                        </table>
                        <div id="rolagem" style="border:solid 1px #507CD1; width:320; height:90; position:relative;margin: 0px 0px 0px 0px;overflow: auto;">
                            <table cellpadding="0" width="320">
                                <tr>
                                    <td>
                                        <asp:RadioButtonList ID="chklTabelas" runat="server" 
                                            OnDataBound="chklTabelas_DataBound"></asp:RadioButtonList>
                                    </td>
                                </tr>
                            </table>&nbsp;
                        </div>
                        <br />
                        <table width="320">
                            <tr>
                                <td align="right">
                                    <asp:Button EnableViewState="false" runat="server" ID="cmdSalvar" SkinID="botaoAzul" Text="Salvar" Width="80" onclick="cmdSalvar_Click" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel runat="server" ID="p2">
                    <HeaderTemplate><font color='black'>Comissionamento dos superiores</font></HeaderTemplate>
                    <ContentTemplate>
                        <table cellpadding="2" width="320">
                            <tr>
                                <td class="tdPrincipal1">Superiores</td>
                                <td class="tdNormal1"><asp:DropDownList Width="250px" ID="cboSuperiores" SkinID="dropdownSkin" AutoPostBack="true" runat="server" onselectedindexchanged="cboSuperiores_SelectedIndexChanged" /></td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="2" width="322" cellpadding="3" cellspacing="0">
                            <tr><td class="tdPrincipal1" align="center">Tabelas de comissionamento</td></tr>
                        </table>
                        <div id="rolagemSup" style=" border:solid 1px #507CD1; width:320; height:90; position:relative;margin: 0px 0px 0px 0px;overflow: auto;">
                            <table cellpadding="0" width="320">
                                <tr>
                                    <td>
                                        <asp:RadioButtonList ID="chklTabelasSuperiores" runat="server" 
                                            OnDataBound="chklTabelas_DataBound"></asp:RadioButtonList>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <br />
                        <table width="320">
                            <tr>
                                <td align="right">
                                    <asp:Button EnableViewState="false" runat="server" ID="cmdSalvarSuperior" SkinID="botaoAzul" Text="Salvar" Width="80" onclick="cmdSalvarSuperior_Click" />
                                </td>
                            </tr>
                    </ContentTemplate>
                </cc1:TabPanel>
            </cc1:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="conferencia3.aspx.cs" Inherits="www.admin.conferencia4" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register src="../usercontrols/ucConferenciaPainelMensagem.ascx" TagName="ucConferenciaPainelMensagem" tagprefix="uc1" %>
<%@ Register src="../usercontrols/ucConferenciaPassos.ascx" TagName="ucConferenciaPassos" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Conferência</span></td></tr>
        <tr><td><span class="subtitulo">Conferência de propostas</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <uc2:ucConferenciaPassos ID="ucConferenciaPassos" runat="server" />
            <br />
            <b><font color='black'>FINALIZAÇÃO</font></b><br /><br />
            <table runat="server" enableviewstate="false" visible="false" border="0" bgcolor='#EFF3FB' cellpadding="4" cellspacing="0" width="720px" style="border: solid 1px #507CD1">
                <tr>
                    <td width="65%" valign="top">
                        <b><font color='#507CD1'>Beneficiário</font></b><br />
                        <asp:DropDownList ID="cboBeneficiario" Width="396" runat="server" SkinID="dropdownSkin" AutoPostBack="true" OnSelectedIndexChanged="cboBeneficiario_OnSelectedIndexChanged" /><br /><br />
                        <asp:DataList Width="100%" CellPadding="0" CellSpacing="0" ID="dlFicha" DataKeyField="ID" runat="server">
                            <HeaderTemplate></HeaderTemplate>
                            <ItemTemplate>
                                <table cellpadding="3" cellspacing="0" width="99%">
                                    <tr>
                                        <td colspan="2" bgcolor='#EFF3FB' style="border-left:solid 1px #507CD1;border-top:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="left">
                                            <asp:Label ID="lblQuesta" Text='<%# DataBinder.Eval(Container.DataItem, "Descricao") %>' runat="server" />
                                            <asp:Literal ID="litItemDeclaracaoID" Text='<%# DataBinder.Eval(Container.DataItem, "ID") %>' runat="server" Visible="false" />
                                        </td>
                                        <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center" width="1%">
                                            <asp:CheckBox OnCheckedChanged="chkFSim_Changed" AutoPostBack="true" SkinID="checkboxSkin" ID="chkFSim" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                                <table><tr><td height="1px"></td></tr></table>
                            </ItemTemplate>
                        </asp:DataList>
                    </td>
                    <td valign="top" align="right" style="border-left:solid 0px #507CD1">
                        <uc1:ucConferenciaPainelMensagem ID="ucCPM" runat="server" />
                    </td>
                </tr>
            </table>
            <table bgcolor='#EFF3FB' cellpadding="4" cellspacing="0" width="720px" style="border: solid 1px #507CD1">
                <tr>
                    <td width="63%" valign="top">
                        <font color='#507CD1'>OBS.</font>
                        <br />
                        <asp:TextBox ID="txtOBS" Height="63" Width="96%" runat="server" SkinID="textboxSkin" TextMode="MultiLine" />
                    </td>
                    <td valign="top">
                        <font color='#507CD1'>Estágio</font>
                        <br />
                        <asp:DropDownList ID="cboDepartamento" Width="240" SkinID="dropdownSkin" runat="server" />
                        <br /><br />
                        <font color='#507CD1'>Prazo</font>
                        <br />
                        <asp:TextBox ID="txtPrazo" Width="80" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                    </td>
                </tr>
            </table>
            <br />
            <table cellpadding="2" cellspacing="0" width="720px" style="border: solid 1px #507CD1">
                <tr>
                    <td align="left"><asp:Button runat="server" Width="80" ID="cmdVoltar" Text="Voltar" SkinID="botaoAzul" onclick="cmdVoltar_Click" /></td>
                    <td align="right"><asp:Button runat="server" Width="80" ID="cmdProximo" Text="Salvar" SkinID="botaoAzul" onclick="cmdProximo_Click" OnClientClick="return confirm('Deseja realmente salvar a proposta?');"/></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="feriado.aspx.cs" Inherits="www.admin.feriado" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"></td></tr>
        <tr><td><span class="titulo">Feriado</span></td></tr>
        <tr><td><span class="subtitulo">Preencha os campos abaixo e clique em salvar</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="upDadosComuns" UpdateMode="Conditional">
        <ContentTemplate>
            <table width="650" cellpadding="1">
                <tr>
                    <td width="69" class="tdPrincipal1">&nbsp;Descrição</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" SkinID="textboxSkin" MaxLength="149" ID="txtDescricao" Width="98%" /></td>
                </tr>
                <tr>
                    <td width="69" class="tdPrincipal1">&nbsp;Data</td>
                    <td class="tdNormal1">
                        <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtData" Width="70" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                    </td>
                </tr>
                <tr>
                    <td width="69" class="tdPrincipal1" valign="top">&nbsp;Observações</td>
                    <td class="tdNormal1">
                        <asp:TextBox runat="server" SkinID="textboxSkin" TextMode="MultiLine" Rows="5" ID="txtObs" Width="98%" />
                    </td>
                </tr>
            </table>
            <br />
            <table width="650" cellpadding="1">
                <tr>
                    <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
                    <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
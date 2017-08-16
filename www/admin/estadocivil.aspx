<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="estadocivil.aspx.cs" Inherits="www.admin.estadocivil" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"></td></tr>
        <tr><td><span class="titulo">Estado Civil - Detalhe</span></td></tr>
        <tr><td><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="upDadosComuns" UpdateMode="Conditional">
        <ContentTemplate>
            <table width="400" cellpadding="1">
                <tr>
                    <td width="90" class="tdPrincipal1">&nbsp;Operadora</td>
                    <td class="tdNormal1"><asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboOperadora" Width="299" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">&nbsp;Descrição</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" SkinID="textboxSkin" MaxLength="149" ID="txtDescricao" Width="180" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">&nbsp;Código</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" SkinID="textboxSkin" MaxLength="149" ID="txtCodigo" Width="180" /></td>
                </tr>
            </table>
            <br />
            <table width="400" cellpadding="1">
                <tr>
                    <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
                    <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
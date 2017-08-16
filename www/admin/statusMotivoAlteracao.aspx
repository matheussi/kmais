<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="statusMotivoAlteracao.aspx.cs" Inherits="www.admin.statusMotivoAlteracao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr>
            <td><span class="titulo">Motivo de alteração de status de proposta</span></td>
        </tr>
        <tr>
            <td nowrap><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table cellpadding="2" width="305">
        <tr>
            <td class="tdPrincipal1">Evento</td>
            <td class="tdNormal1"><asp:DropDownList runat="server" ID="cboTipo" Width="240" SkinID="dropdownSkin" /></td>
        </tr>
        <tr>
            <td class="tdPrincipal1">Descrição</td>
            <td class="tdNormal1"><asp:TextBox runat="server" ID="txtDescricao" Width="240" MaxLength="200" SkinID="textboxSkin" /></td>
        </tr>
    </table>
    <table cellpadding="2" width="306">
        <tr>
            <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
            <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
        </tr>
    </table>
</asp:Content>
﻿<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="tipocontrato.aspx.cs" Inherits="www.admin.tipocontrato" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"><img src="../images/imgTitulos/tipos_de_contrato.png" /></td></tr>
        <tr>
            <td><span class="titulo">Tipo de contrato</span></td>
        </tr>
        <tr>
            <td nowrap><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <table cellpadding="2" width="305">
        <tr>
            <td class="tdPrincipal1">Descrição</td>
            <td class="tdNormal1"><asp:TextBox runat="server" ID="txtDescricao" Width="240" MaxLength="200" SkinID="textboxSkin" /></td>
        </tr>
        <tr>
            <td class="tdPrincipal1">Comissionamento</td>
            <td class="tdNormal1"><asp:DropDownList ID="cboComissao" runat="server" SkinID="dropdownSkin" Width="245" /></td>
        </tr>
        <tr>
            <td colspan="2" class="tdNormal1"><asp:CheckBox runat="server" ID="chkAtivo" Text="Ativo" SkinID="checkboxSkin" Checked="true" /></td>
        </tr>
    </table>
    <table cellpadding="2" width="306">
        <tr>
            <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
            <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
        </tr>
    </table>
</asp:Content>
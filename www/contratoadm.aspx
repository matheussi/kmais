<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="contratoadm.aspx.cs" Inherits="www.contratoadm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
   <table width="70%">
        <tr><td width="65" rowspan="3"><img height="50" src="images/imgTitulos/contratos_65_50.png" /></td></tr>
        <tr><td><span class="titulo">Contrato Administrativo</span></td></tr>
        <tr><td><span class="subtitulo">Preencha os dados abaixo e clique em salvar</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table width="306">
        <tr>
            <td width="76"><span class="subtitulo">Estipulante</span></td>
            <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboEstipulante" Width="100%" /></td>
        </tr>
        <tr>
            <td width="76"><span class="subtitulo">Operadora</span></td>
            <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadoras" Width="100%" /></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Descrição</span></td>
            <td><asp:TextBox runat="server" ID="txtDescricao" Width="240" MaxLength="200" SkinID="textboxSkin" /></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Número</span></td>
            <td><asp:TextBox runat="server" ID="txtNumero" Width="110" MaxLength="100" SkinID="textboxSkin" /></td>
        </tr>
        <tr>
            <td colspan="2" class="tdNormal1"><asp:CheckBox runat="server" ID="chkAtiva" Text="Ativo" SkinID="checkboxSkin" Checked="true" /></td>
        </tr>
    </table>
    <br />
    <table cellpadding="2" width="306">
        <tr>
            <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
            <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
        </tr>
    </table>
</asp:Content>

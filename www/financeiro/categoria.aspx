<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="categoria.aspx.cs" Inherits="www.financeiro.categoria" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td><span class="titulo">Categoria de entradas de conta corrente</span></td></tr>
        <tr><td nowrap="nowrap"><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table width="40%" cellpadding="2" cellspacing="1">
                <tr>
                    <td width="85" class="tdPrincipal1">&nbsp;Categoria</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" SkinID="textboxSkin" MaxLength="149" ID="txtNome" Width="98%" /></td>
                </tr>
                <tr>
                    <td width="85" class="tdPrincipal1">&nbsp;Movimentação</td>
                    <td class="tdNormal1">
                        <asp:RadioButton runat="server" ID="optCredito" Text="Crédito" GroupName="a" Checked="true" />&nbsp;
                        <asp:RadioButton runat="server" ID="optDebito" Text="Débito" GroupName="a" />
                    </td>
                </tr>
            </table>
            <br />
            <table width="40%" cellpadding="1">
                <tr>
                    <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
                    <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
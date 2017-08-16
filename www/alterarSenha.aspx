<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="alterarSenha.aspx.cs" Inherits="www.alterarSenha" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="30"><img src="../images/imgTitulos/usuarios.png" alt="" /></td></tr>
        <tr><td><span class="titulo">Alterar senha</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" width="425" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1" width="34%">Senha atual</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" ID="txtSenhaAtual" Width="90" MaxLength="25" SkinID="textboxSkin" TextMode="Password" /></td>
                </tr>
            </table>
            <br />
            <table cellpadding="2" width="425" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1">Login</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" ID="txtLogin" Width="200" MaxLength="70" SkinID="textboxSkin" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="34%">Nova senha</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" ID="txtSenha1" Width="90" MaxLength="25" SkinID="textboxSkin" TextMode="Password" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Confirme a nova senha</td>
                    <td class="tdNormal1">
                        <asp:TextBox runat="server" ID="txtSenha2" Width="90" MaxLength="25" SkinID="textboxSkin" TextMode="Password" />
                        &nbsp;
                        <asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

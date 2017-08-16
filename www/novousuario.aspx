<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="novousuario.aspx.cs" Inherits="www.novousuario" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <table cellpadding="2" cellspacing="1" width="425" style="border: solid 1px #507CD1">
            <tr>
                <td class="tdPrincipal1">Nome</td>
                <td class="tdNormal1"><asp:TextBox runat="server" ID="txtNome" Width="260" MaxLength="200" SkinID="textboxSkin" /></td>
            </tr>
            <tr>
                <td class="tdPrincipal1">Login</td>
                <td class="tdNormal1"><asp:TextBox runat="server" ID="txtEmail" Width="260" MaxLength="70" SkinID="textboxSkin" /></td>
            </tr>
            <tr>
                <td class="tdPrincipal1" colspan="2"><asp:CheckBox ID="chkTipo" Text="Altera data de vencimento" runat="server" /></td>
            </tr>
        </table>
        <br />
        <table cellpadding="2" width="425" style="border: solid 1px #507CD1">
            <tr>
                <td class="tdPrincipal1" width="34%">Senha</td>
                <td class="tdNormal1"><asp:TextBox runat="server" ID="txtSenha1" Width="90" MaxLength="25" SkinID="textboxSkin" TextMode="Password" /></td>
            </tr>
            <tr>
                <td class="tdPrincipal1">Confirme</td>
                <td class="tdNormal1"><asp:TextBox runat="server" ID="txtSenha2" Width="90" MaxLength="25" SkinID="textboxSkin" TextMode="Password" /></td>
            </tr>
        </table>
        <br />
        <table cellpadding="2" width="425">
            <tr>
                <td align="center"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
            </tr>
        </table>
    </form>
</body>
</html>

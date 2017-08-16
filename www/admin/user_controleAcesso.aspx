<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="user_controleAcesso.aspx.cs" Inherits="www.admin.user_controleAcesso" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Acesso</span></td></tr>
        <tr><td><span class="subtitulo">Controle de acesso de usuários</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table width="480px">
        <tr>
            <td width="126"><span class="subtitulo">Usuário</span></td>
            <td colspan="3"><asp:Literal ID="litUsuario" runat="server" /></td>
        </tr>
    </table>
    <table width="580px">
        <tr>
            <td width="75"><asp:CheckBox ID="chkDomingo" Text="Domingo" Font-Bold="true" SkinID="checkboxSkin" runat="server" /></td>
            <td valign="bottom">das</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboDomDasHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboDomDasMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">às</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboDomAsHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboDomAsMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">IPs</td>
            <td valign="bottom"><asp:TextBox ID="txtDomingoIPs" SkinID="textboxSkin" runat="server" Width="200" /></td>
        </tr>
     </table>
     <table width="580px">
        <tr>
            <td width="75"><asp:CheckBox ID="chkSegunda" Text="Segunda" Font-Bold="true" SkinID="checkboxSkin" runat="server" /></td>
            <td valign="bottom">das</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboSegundaDasHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboSegundaDasMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">às</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboSegundaAsHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboSegundaAsMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">IPs</td>
            <td valign="bottom"><asp:TextBox ID="txtSegundaIPs" SkinID="textboxSkin" runat="server" Width="200" /></td>
        </tr>
     </table>
     <table width="580px">
        <tr>
            <td width="75"><asp:CheckBox ID="chkTerca" Text="Terça" Font-Bold="true" SkinID="checkboxSkin" runat="server" /></td>
            <td valign="bottom">das</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboTercaDasHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboTercaDasMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">às</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboTercaAsHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboTercaAsMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">IPs</td>
            <td valign="bottom"><asp:TextBox ID="txtTercaIPs" SkinID="textboxSkin" runat="server" Width="200" /></td>
        </tr>
     </table>
     <table width="580px">
        <tr>
            <td width="75"><asp:CheckBox ID="chkQuarta" Text="Quarta" Font-Bold="true" SkinID="checkboxSkin" runat="server" /></td>
            <td valign="bottom">das</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboQuartaDasHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboQuartaDasMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">às</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboQuartaAsHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboQuartaAsMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">IPs</td>
            <td valign="bottom"><asp:TextBox ID="txtQuartaIPs" SkinID="textboxSkin" runat="server" Width="200" /></td>
        </tr>
     </table>
     <table width="580px">
        <tr>
            <td width="75"><asp:CheckBox ID="chkQuinta" Text="Quinta" Font-Bold="true" SkinID="checkboxSkin" runat="server" /></td>
            <td valign="bottom">das</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboQuintaDasHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboQuintaDasMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">às</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboQuintaAsHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboQuintaAsMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">IPs</td>
            <td valign="bottom"><asp:TextBox ID="txtQuintaIPs" SkinID="textboxSkin" runat="server" Width="200" /></td>
        </tr>
     </table>
     <table width="580px">
        <tr>
            <td width="75"><asp:CheckBox ID="chkSexta" Text="Sexta" Font-Bold="true" SkinID="checkboxSkin" runat="server" /></td>
            <td valign="bottom">das</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboSextaDasHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboSextaDasMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">às</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboSextaAsHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboSextaAsMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">IPs</td>
            <td valign="bottom"><asp:TextBox ID="txtSextaIPs" SkinID="textboxSkin" runat="server" Width="200" /></td>
        </tr>
     </table>
     <table width="580px">
        <tr>
            <td width="75"><asp:CheckBox ID="chkSabado" Text="Sábado" Font-Bold="true" SkinID="checkboxSkin" runat="server" /></td>
            <td valign="bottom">das</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboSabadoDasHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboSabadoDasMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">às</td>
            <td valign="bottom">
                <asp:DropDownList ID="cboSabadoAsHora" SkinID="dropdownSkin" runat="server" />:
                <asp:DropDownList ID="cboSabadoAsMinuto" SkinID="dropdownSkin" runat="server" />
            </td>
            <td valign="bottom">IPs</td>
            <td valign="bottom"><asp:TextBox ID="txtSabadoIPs" SkinID="textboxSkin" runat="server" Width="200" /></td>
        </tr>
     </table>
    <br />
    <table cellpadding="2" width="580px">
        <tr>
            <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
            <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
        </tr>
    </table>
</asp:Content>

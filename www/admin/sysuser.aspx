<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="sysuser.aspx.cs" Inherits="www.admin.sysuser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="30"><img src="../images/imgTitulos/usuarios.png" alt="" /></td></tr>
        <tr><td><span class="titulo">Usuário</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<script language="javascript" type="text/javascript" src="../js/common.js"></script>
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1" width="425" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1" width="139">Perfil</td>
                    <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" Width="265px" runat="server" ID="cboPerfil" AutoPostBack="true" OnSelectedIndexChanged="cboPerfil_OnSelectedIndexChanged" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Nome</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" ID="txtNome" Width="260" MaxLength="200" SkinID="textboxSkin" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">E-mail</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" ID="txtEmail" Width="260" MaxLength="70" SkinID="textboxSkin" /></td>
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
                <tr>
                    <td colspan="2" class="tdNormal1"><asp:CheckBox runat="server" ID="chkAlteraValor" Text="Altera valor total em propostas" SkinID="checkboxSkin" /></td>
                </tr>
                <tr>
                    <td colspan="2" class="tdNormal1"><asp:CheckBox runat="server" ID="chkLibera" Text="Libera contratos em não-conformidade com as regras de saúde" SkinID="checkboxSkin" /></td>
                </tr>
                <tr>
                    <td colspan="2" class="tdNormal1"><asp:CheckBox runat="server" ID="chkAlteraProdutor" Text="Altera produtor" SkinID="checkboxSkin" /></td>
                </tr>
                <tr>
                    <td colspan="2" class="tdNormal1"><asp:CheckBox runat="server" ID="chkAtivo" Text="Ativo" SkinID="checkboxSkin" Checked="true" /></td>
                </tr>
            </table>
            <br />
            <table cellpadding="2" cellspacing="1" width="425" style="border: solid 1px #507CD1">
                <tr>
                    <td colspan="2" class="tdPrincipal1" align="center" ><b>Observações</b></td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="txtObs" Width="99%" Height="70" SkinID="textboxSkin" TextMode="MultiLine" runat="server" />
                    </td>
                </tr>
            </table>
            <br />
            <table cellpadding="2" width="425">
                <tr>
                    <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
                    <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

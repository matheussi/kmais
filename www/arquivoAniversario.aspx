<%@ Page Theme="Theme" Language="C#" AutoEventWireup="true" MasterPageFile="~/layout.Master" CodeBehind="arquivoAniversario.aspx.cs" Inherits="www.arquivoAniversario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Aniversário</span></td></tr>
        <tr><td><span class="subtitulo">Gerar arquivos de aniversariantes</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdGerar" />
        </Triggers>
        <ContentTemplate>
            <table width="420px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1">Data Aniversário</td>
                    <td class="tdNormal1"><b>Dia</b>&nbsp;<asp:DropDownList ID="cboDia" Width="45" SkinID="dropdownSkin" runat="server" />&nbsp;&nbsp;&nbsp;<b>Mês</b>&nbsp;<asp:DropDownList ID="cboMes" Width="55" SkinID="dropdownSkin" runat="server" />&nbsp;&nbsp;&nbsp;<asp:Button ID="cmdGerar" Text="Gerar arquivo" Width="100" SkinID="botaoAzulBorda" runat="server" OnClick="cmdGerar_Click" OnClientClick="return confirm('Deseja realmente prosseguir?');" />&nbsp;</td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<%@ Page Language="C#" Theme="Theme" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="descontosOcorridos.aspx.cs" Inherits="www.UBRASP.arquivos.scc.descontosOcorridos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"></td></tr>
        <tr><td><span class="titulo">PSCC - Descontos</span></td></tr>
        <tr><td nowrap="true"><span class="subtitulo">Utilize os campos abaixo para informar os descontos ocorridos</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">

    <asp:UpdatePanel ID="up" runat="server" UpdateMode="Always">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdProcessar" />
        </Triggers>
        <ContentTemplate>
            <table style="border: 1px solid #507CD1" cellpadding="8" cellspacing="0" width="574px">
                <tr>
                    <td class="tdNormal1">Órgão</td>
                    <td class="tdNormal1" colspan="2"><asp:DropDownList SkinID="dropdownSkin" Width="99%" runat="server" ID="cboContratoADM" /></td>
                </tr>
                <tr>
                    <td class="tdNormal1">Arquivo</td>
                    <td class="tdNormal1"><asp:FileUpload runat="server" ID="ufArquivo" Width="100%" /></td>
                    <td class="tdNormal1"><asp:Button ID="cmdProcessar" runat="server" Text="processar" SkinID="botaoAzulBorda" OnClick="cmdProcessar_click" OnClientClick="return confirm('Deseja realmente processar o arquivo?');" /></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

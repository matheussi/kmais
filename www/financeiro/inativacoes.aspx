<%@ Page Language="C#" Theme="Theme" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="inativacoes.aspx.cs" Inherits="www.financeiro.inativacoes" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Inativações</span></td></tr>
        <tr><td><span class="subtitulo">Inativações em lote</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
<ContentTemplate>
    <table width="480px">
        <tr>
            <td width="126"><span class="subtitulo">Operadora</span></td>
            <td colspan="3"><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadoras" AutoPostBack="true" Width="100%" onselectedindexchanged="cboOperadoras_SelectedIndexChanged" /></td>
        </tr>
        <tr runat="server" id="trGerarArquivo" visible="false">
            <td></td>
            <td colspan="3"><asp:CheckBox ID="chkGerarArquivo" SkinID="checkboxSkin" Text="Guardar para gerar arquivo de movimentação" runat="server" /></td>
        </tr>
        <tr>
            <td width="126"><span class="subtitulo">Inativado em</span></td>
            <td width="23%">
                <asp:TextBox SkinID="textboxSkin" runat="server" id="txtInativadoEm" Width="66px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
            </td>
            <td width="30%" align="right"><span class="subtitulo">Por falta de pagto. em</span></td>
            <td align="right">
                <asp:TextBox SkinID="textboxSkin" runat="server" id="txtFaltaPagtoEM" Width="66px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
            </td>
        </tr>
        <tr>
            <td>Motivo</td>
            <td colspan="3"><asp:DropDownList ID="cboStatusMotivo" runat="server" Width="100%" SkinID="dropdownSkin" /></td>
        </tr>
        <tr>
            <td valign="top">Observações</td>
            <td colspan="3"><asp:TextBox ID="txtObs" runat="server" SkinID="textboxSkin" TextMode="MultiLine" Width="99%" Rows="3" /></td>
        </tr>
        <tr>
            <td width="126" valign="top"><span class="subtitulo">Número de propostas</span></td>
            <td colspan="3">
                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtPropostas" Width="99%" Height="190" Columns="1" TextMode="MultiLine" />
            </td>
        </tr>
    </table>
    <br />
    <table width="480px">
        <tr>
            <td align="right">
                <asp:Button runat="server" ID="cmdInativar" SkinID="botaoAzul" Text="Inativar" Width="80" onclick="cmdInativar_Click" OnClientClick="return confirm('Atenção!\nDeseja realmente inativar essas propostas?\nEssa operação não poderá ser desfeita.');" />
            </td>
        </tr>
    </table>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
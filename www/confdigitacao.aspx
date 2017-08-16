<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="confdigitacao.aspx.cs" Inherits="www.confdigitacao" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Relatório</span></td></tr>
        <tr><td><span class="subtitulo">Relatório para conferência por vigência</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
<ContentTemplate>
    <table width="480px">
        <%--<tr>
            <td width="75"><span class="subtitulo">Estipulante</span></td>
            <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboEstipulante" Width="100%" onselectedindexchanged="cboEstipulante_SelectedIndexChanged" AutoPostBack="true" /></td>
        </tr>--%>
        <tr>
            <td width="75"><span class="subtitulo">Operadora</span></td>
            <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadoras" Width="100%" onselectedindexchanged="cboOperadoras_SelectedIndexChanged" AutoPostBack="true" /></td>
        </tr>
        <tr>
            <td width="75" valign="top"><span class="subtitulo">Contratos</span></td>
            <td><asp:ListBox SkinID="listBoxSkin" runat="server" ID="lstContratos" SelectionMode="Multiple" Height="80" Width="100%" /></td>
        </tr>
        <tr>
            <td width="75"><span class="subtitulo">Vigência</span></td>
            <td>
                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtVigencia" Width="66px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                <asp:Image SkinID="imgCanlendario" ID="imgVig" runat="server" EnableViewState="false" />
                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtVigencia" PopupButtonID="imgVig" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
            </td>
        </tr>
        <tr>
            <td align="right" colspan="2">
                <asp:Button runat="server" ID="cmdGerar" SkinID="botaoAzul" Text="Gerar" Width="80" onclick="cmdGerar_Click" />
            </td>
        </tr>
    </table>
    <br />
    <font color='darkgray'><asp:Literal ID="litSumario" runat="server" EnableViewState="false" /></font>
    <asp:GridView ID="grid" runat="server" AutoGenerateColumns="False" EnableViewState="False" Width="480px">
        <Columns>
            <asp:BoundField DataField="ContratoAdmDescricao" HeaderText="Contrato Adm.">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="PropostaNumero" HeaderText="Proposta">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="TitularNome" HeaderText="Titular">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="QtdVidas" HeaderText="Vidas">
                <ItemStyle Width="1%" HorizontalAlign="Center" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
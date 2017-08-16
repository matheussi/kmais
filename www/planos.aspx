<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="planos.aspx.cs" Inherits="www.planos" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img height="48" src="images/imgTitulos/planos.png" /></td></tr>
        <tr><td><span class="titulo">Planos</span></td></tr>
        <tr><td><span class="subtitulo">Selecione a operadora para exibir seus planos</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
<ContentTemplate>
    <table>
        <tr><td width="65"><span class="subtitulo">Operadora</span></td>
        <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadoras" Width="400" onselectedindexchanged="cboOperadoras_SelectedIndexChanged" AutoPostBack="true" /></td></tr>
        <tr><td width="65"><span class="subtitulo">Contrato</span></td>
        <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboContrato" Width="400" onselectedindexchanged="cboContrato_SelectedIndexChanged" AutoPostBack="true" /></td></tr>
    </table>
    <br />
    <asp:GridView ID="gridPlanos" Width="474px" SkinID="gridViewSkin" 
        runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,Ativo"
        onrowcommand="gridPlanos_RowCommand" onrowdatabound="gridPlanos_RowDataBound">
        <Columns>
            <asp:BoundField DataField="Descricao" HeaderText="Plano">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:ButtonField Text="inativar" CommandName="inativar">
                <ItemStyle ForeColor="#CC0000" Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField Text="<img src='images/edit.png' alt='editar' border='0' />" CommandName="editar" >
            <ItemStyle Width="1%" />
            </asp:ButtonField>
        </Columns>
    </asp:GridView>
    <br />
    <table width="474px">
        <tr>
            <td align="right">
                <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
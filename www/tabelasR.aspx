<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="tabelasR.aspx.cs" Inherits="www.tabelasR" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img src="images/imgTitulos/tabela_reajuste.png" /></td></tr>
        <tr><td><span class="titulo">Tabelas de reajuste</span></td></tr>
        <tr><td><span class="subtitulo">Selecione a operadora para exibir suas tabelas de reajuste</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <table>
        <tr>
            <td width="65"><span class="subtitulo">Operadora</span></td>
            <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadoras" Width="400" onselectedindexchanged="cboOperadoras_SelectedIndexChanged" AutoPostBack="true" /></td>
        </tr>
        <tr>
            <td width="65"><span class="subtitulo">Contrato</span></td>
            <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboContratos" Width="400" onselectedindexchanged="cboContratos_SelectedIndexChanged" AutoPostBack="true" /></td>
        </tr>
    </table>
    <br />
    <asp:GridView ID="gridTabelas" Width="474px" SkinID="gridViewSkin" 
        runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,Corrente"
        onrowcommand="gridTabelas_RowCommand" onrowdatabound="gridTabelas_RowDataBound">
        <Columns>
            <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="Corrente" HeaderText="Atual">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" Width="1%" />
            </asp:BoundField>
            <asp:ButtonField Text="<img src='images/edit.png' alt='editar' border='0' />" CommandName="editar" >
                <ItemStyle Width="1%" />
            </asp:ButtonField>
        </Columns>
    </asp:GridView>
    <br />
    <table width="474px">
        <tr>
            <td align="right">
                <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Nova" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
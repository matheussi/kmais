<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="tiposcontrato.aspx.cs" Inherits="www.admin.tiposcontrato" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img src="../images/imgTitulos/tipos_de_contrato.png" /></td></tr>
        <tr><td><span class="titulo">Tipos de contrato</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo os tipos de contrato cadastrados no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:GridView ID="grid" Width="474px" SkinID="gridViewSkin" 
        runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,Ativo"
        onrowcommand="grid_RowCommand" onrowdatabound="grid_RowDataBound">
        <Columns>
            <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:ButtonField Text="inativar" CommandName="inativar">
                <ItemStyle ForeColor="#CC0000" Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField Text="<img src='../images/edit.png' alt='editar' border='0' />" CommandName="editar" >
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
</asp:Content>
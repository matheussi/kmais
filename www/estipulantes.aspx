<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="estipulantes.aspx.cs" Inherits="www.estipulantes" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img height="50" src="images/imgTitulos/estipulantes.jpg" /></td></tr>
        <tr><td><span class="titulo">Estipulantes</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo os estipulantes cadastrados no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
    <ContentTemplate>
    <asp:GridView ID="grid" Width="474px" SkinID="gridViewSkin" 
        runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,Ativo"
        onrowcommand="grid_RowCommand" onrowdatabound="grid_RowDataBound" 
            onpageindexchanging="grid_PageIndexChanging">
        <Columns>
            <asp:BoundField DataField="Descricao" HeaderText="Estipulante">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />">
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField ButtonType="Link" Text="inativar" CommandName="inativar">
                <ItemStyle ForeColor="#CC0000" Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                <ItemStyle Width="1%" />
            </asp:ButtonField>
        </Columns>
    </asp:GridView>
    </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <table width="474px">
        <tr>
            <td align="right">
                <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
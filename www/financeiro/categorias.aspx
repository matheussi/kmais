<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="categorias.aspx.cs" Inherits="www.financeiro.categorias" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td><span class="titulo">Categorias de entradas de conta corrente</span></td></tr>
        <tr><td nowrap="nowrap"><span class="subtitulo">Abaixo as categorias cadastradas no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <asp:GridView ID="grid" Width="650px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,Sistema"
                OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="strTipo" HeaderText="Tipo">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Descricao" HeaderText="Categoria">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar">
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <br />
            <table width="650px">
                <tr>
                    <td align="right">
                        <asp:Button runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Nova" Width="80" onclick="cmdNovo_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
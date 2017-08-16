<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="categorias.aspx.cs" Inherits="www.admin.categorias" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img src="../images/imgTitulos/categorias.png" /></td></tr>
        <tr><td><span class="titulo">Categorias</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo as categorias cadastradas no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <asp:GridView ID="grid" Width="474px" SkinID="gridViewSkin" runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,Ativo"
                OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound"  PageSize="20"
                OnPageIndexChanging="grid_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="Descricao" HeaderText="Categoria">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="PerfilDescricao" HeaderText="Perfil">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField Text="inativar" CommandName="inativar">
                        <ItemStyle ForeColor="#CC0000" Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                        <ItemStyle Font-Size="10px" Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/edit.png' alt='editar' border='0' />" CommandName="editar" >
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
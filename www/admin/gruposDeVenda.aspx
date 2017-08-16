<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="gruposDeVenda.aspx.cs" Inherits="www.admin.gruposDeVenda" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img src="../images/imgTitulos/usuarios.png" /></td></tr>
        <tr><td><span class="titulo">Grupos de venda</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo grupos de venda cadastrados no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <asp:GridView ID="gridGrupos" Width="474px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,Ativo"
                onrowcommand="gridGrupos_RowCommand" onrowdatabound="gridGrupos_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Descricao" HeaderText="Grupo">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="inativar" CommandName="inativar" >
                        <ItemStyle ForeColor="#cc0000" Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <table width="474px">
        <tr>
            <td align="right">
                <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>

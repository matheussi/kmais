<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="comissionamentos.aspx.cs" Inherits="www.comissionamentos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3" width="40"><img src="images/imgTitulos/tabela_comissionario.png" /></td></tr>
        <tr><td><span class="titulo">Tabelas de comissionamento</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo as tabelas de comissionamento cadastradas no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
    <ContentTemplate>
       <table cellpadding="2" cellspacing="1" width="473" style="border: solid 1px #507CD1">
            <tr>
                <td class="tdPrincipal1" width="48">Perfil</td>
                <td class="tdNormal1"><asp:DropDownList Width="99%" ID="cboPerfis" SkinID="dropdownSkin" AutoPostBack="true" runat="server" onselectedindexchanged="cboPerfis_SelectedIndexChanged" /></td>
            </tr>
        </table>
        <br />
        <asp:GridView ID="gridTabelas" Width="474px" SkinID="gridViewSkin" 
            runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID"
            OnPageIndexChanging="gridTabelas_PageIndexChanging" onrowcommand="gridTabelas_RowCommand" onrowdatabound="gridTabelas_RowDataBound">
            <Columns>
                <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="CategoriaNome" HeaderText="Categoria">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Categoria_PerfilDescricao" HeaderText="Perfil">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                    <ItemStyle ForeColor="#cc0000" Width="1%" />
                </asp:ButtonField>
                <asp:ButtonField Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
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
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="filiais.aspx.cs" Inherits="www.filiais" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img src="images/icones/filiais.png" /></td></tr>
        <tr><td><span class="titulo">Filiais</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo as filiais cadastradas no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
    <ContentTemplate>
    <asp:GridView ID="grid" Width="474px" SkinID="gridViewSkin" 
        runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,Ativa"
        onrowcommand="grid_RowCommand" onrowdatabound="grid_RowDataBound" 
            onpageindexchanging="grid_PageIndexChanging">
        <Columns>
            <asp:BoundField DataField="Nome" HeaderText="Filial">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="FTelefone" HeaderText="Fone">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="Email" HeaderText="E-mail">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />">
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField Text="inativar" CommandName="inativar">
                <ItemStyle ForeColor="#CC0000" Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
            <ItemStyle Width="1%" />
            </asp:ButtonField>
        </Columns>
    </asp:GridView>
    <br />
    </ContentTemplate>
    </asp:UpdatePanel>
    <table width="474px">
        <tr>
            <td align="right">
                <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Nova" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
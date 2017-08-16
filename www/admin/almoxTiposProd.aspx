<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="almoxTiposProd.aspx.cs" Inherits="www.admin.almoxTiposProd" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img alt='' src="../images/imgTitulos/produtos.png" /></td></tr>
        <tr><td><span class="titulo">Almoxarifado - Produtos</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo os produtos cadastrados no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
    <ContentTemplate>
    <table width="480px" cellpadding="4" cellspacing="0" style="border:solid 1px #507CD1">
        <tr>
            <td class="tdNormal1" width="64px">Operadora</td>
            <td class="tdNormal1"><asp:DropDownList ID="cboOperadoras" Width="99%" SkinID="dropdownSkin" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboOperadoras_OnSelectedIndexChanged" /></td>
        </tr>
    </table>
    <asp:GridView ID="gridProdutos" Width="480px" SkinID="gridViewSkin" 
        runat="server" AllowPaging="True" AutoGenerateColumns="False"  
        DataKeyNames="ID,Ativo" onrowcommand="gridProdutos_RowCommand" onrowdatabound="gridProdutos_RowDataBound">
        <Columns>
            <asp:BoundField DataField="TipoProdutoDescricao" HeaderText="Tipo">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="FilialNome" Visible="true" HeaderText="Filial">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="QTD" HeaderText="Qtd.">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="QTDMin" HeaderText="Qtd. Mín.">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:ButtonField Text="inativar" CommandName="inativar">
                <ItemStyle ForeColor="#CC0000" Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                <ItemStyle Font-Size="10px" Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField Text="<img src='../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                <ItemStyle Font-Size="10px" Width="1%" />
            </asp:ButtonField>
        </Columns>
    </asp:GridView>
    <br />
    <table width="480px">
        <tr>
            <td align="right">
                <asp:Button runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="operadoras.aspx.cs" Inherits="www.operadoras" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img height="50" src="images/imgTitulos/operadoras.jpg" /></td></tr>
        <tr><td><span class="titulo">Operadoras</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo as operadoras cadastradas no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
    <ContentTemplate>
    <asp:GridView ID="gridOperadoras" Width="650px" SkinID="gridViewSkin" PageSize="12"
        runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,Inativa"
        OnRowCommand="gridOperadoras_RowCommand" OnRowDataBound="gridOperadoras_RowDataBound" OnPageIndexChanging="gridOperadoras_PageIndexChanging">
        <Columns>
            <asp:BoundField DataField="Nome" HeaderText="Operadora">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField Visible="false" DataField="Email" HeaderText="E-mail">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField Visible="false" DataField="FFone" HeaderText="Fone">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField Visible="false" DataField="Contato" HeaderText="Contato">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />">
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField Text="inativar" CommandName="inativar">
                <ItemStyle Width="1%" ForeColor="#CC0000" />
            </asp:ButtonField>
            <asp:ButtonField Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar">
                <ItemStyle Width="1%" />
            </asp:ButtonField>
        </Columns>
    </asp:GridView>
    </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <table width="650px">
        <tr>
            <td align="right">
                <asp:Button runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Nova" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
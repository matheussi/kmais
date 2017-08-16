<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="feriados.aspx.cs" Inherits="www.admin.feriados" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"></td></tr>
        <tr><td><span class="titulo">Feriados</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo os feriados cadastrados no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="upDadosComuns" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:GridView ID="grid" Width="550px" SkinID="gridViewSkin" 
            runat="server" AllowPaging="False" AutoGenerateColumns="False" DataKeyNames="ID"
            OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
            <Columns>
                <asp:BoundField DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:ButtonField Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                    <ItemStyle Width="1%" />
                </asp:ButtonField>
                <asp:ButtonField Text="<img src='../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                    <ItemStyle Width="1%" />
                </asp:ButtonField>
            </Columns>
        </asp:GridView>
        <table width="550px">
            <tr>
                <td align="right">
                    <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

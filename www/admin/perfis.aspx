<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="perfis.aspx.cs" Inherits="www.admin.perfis" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3">&nbsp;</td></tr>
        <tr><td><span class="titulo">Perfis</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo perfis cadastrados no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <asp:GridView ID="gridPerfis" Width="474px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID"
                onrowcommand="gridPerfis_RowCommand" 
                onrowdatabound="gridPerfis_RowDataBound" 
                onpageindexchanging="gridPerfis_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="Descricao" HeaderText="Grupo">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TipoSTR" HeaderText="Tipo">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField Visible="false" CommandName="excluir" Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle Width="1%" />
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
                <asp:Button Visible="false" EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>

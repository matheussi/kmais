<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="comissionamentosOperadora.aspx.cs" Inherits="www.comissionamentosOperadora" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img hspace="2" src="images/imgTitulos/tabela_valores.png" /></td></tr>
        <tr><td><span class="titulo">Tabelas de Comissionamento</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo as tabelas de comissionamento cadastradas no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <asp:GridView ID="gridTabelas" Width="474px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID"
                onrowcommand="gridTabelas_RowCommand" onrowdatabound="gridTabelas_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Data" HeaderText="Vigência" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle Width="15%" />
                    </asp:BoundField>
                    <asp:ButtonField Text="<img src='images/delete.png' alt='excluir' border='0' />" CommandName="excluir" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='images/edit.png' alt='editar' border='0' />" CommandName="editar" >
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
                <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Nova" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>

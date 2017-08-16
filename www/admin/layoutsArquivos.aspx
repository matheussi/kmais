<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="layoutsArquivos.aspx.cs" Inherits="www.admin.layoutsArquivos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Layouts de arquivo</span></td></tr>
        <tr><td><span class="subtitulo">Layouts customizados de arquivo</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
    <ContentTemplate>
       <table cellpadding="2" cellspacing="1" width="473" style="border: solid 1px #507CD1">
            <tr>
                <td class="tdPrincipal1" width="88">Operadora</td>
                <td class="tdNormal1"><asp:DropDownList Width="99%" ID="cboOperadora" SkinID="dropdownSkin" runat="server" /></td>
            </tr>
            <tr>
                <td class="tdPrincipal1">Tipo de arquivo</td>
                <td class="tdNormal1">
                    <asp:DropDownList Width="74%" ID="cboTipoArquivo" SkinID="dropdownSkin" runat="server" />
                    &nbsp;
                    <asp:Button Text="Localizar" ID="cmdBuscar" runat="server" SkinID="botaoAzulBorda" onclick="cmdBuscar_Click" />
                </td>
            </tr>
        </table>
        <br />
        <asp:Literal ID="litResultado" runat="server" EnableViewState="false" />
        <asp:GridView ID="grid" Width="474px" SkinID="gridViewSkin" 
            runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID"
            OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
            <Columns>
                <asp:BoundField DataField="Nome" HeaderText="Arquivo">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:ButtonField Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                    <ItemStyle ForeColor="#cc0000" Width="1%" />
                </asp:ButtonField>
                <asp:ButtonField Text="<img src='../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                    <ItemStyle Width="1%" />
                </asp:ButtonField>
            </Columns>
        </asp:GridView>
        <br />
        <table width="474px">
            <tr>
                <td align="right">
                    <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
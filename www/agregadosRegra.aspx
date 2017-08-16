<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="agregadosRegra.aspx.cs" Inherits="www.agregadosRegra" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="474px">
        <tr><td rowspan="3"><img src="images/imgTitulos/regras_comercializacao.png" /></td></tr>
        <tr>
            <td><span class="titulo">Regras para agregados e dependentes</span></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Selecione o contrato para exibir as regras de agregados ou dependentes</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
<ContentTemplate>
     <table>
        <tr>
            <td width="65"><span class="subtitulo">Operadora</span></td>
            <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadoras" Width="400" onselectedindexchanged="cboOperadoras_SelectedIndexChanged" AutoPostBack="true" /></td>
        </tr>
        <tr>
            <td width="65"><span class="subtitulo">Contrato</span></td>
            <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboContrato" Width="400" onselectedindexchanged="cboContrato_SelectedIndexChanged" AutoPostBack="true" /></td>
        </tr>
    </table>
    <br />
     <asp:GridView ID="grid" Width="474px" SkinID="gridViewSkin" 
        runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID"
        onrowcommand="grid_RowCommand" onrowdatabound="grid_RowDataBound">
        <Columns>
            <asp:BoundField DataField="Resumo" HeaderText="Descrição">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:ButtonField Text="<img src='images/delete.png' alt='excluir' border='0' />" CommandName="excluir" >
                <ItemStyle ForeColor="#cc0000" Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField Text="<img src='images/edit.png' alt='editar' border='0' />" CommandName="editar" >
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
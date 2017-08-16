<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="adicionaisRegra.aspx.cs" Inherits="www.adicionaisRegra" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="474px">
        <tr><td rowspan="3"><img src="images/imgTitulos/regras_comercializacao.png" /></td></tr>
        <tr>
            <td><span class="titulo">Regras para produtos adicionais</span></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Selecione a operadora para exibir as regras de regras já cadastradas</span></td>
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
    </table>
    <br />
     <asp:GridView ID="grid" Width="474px" SkinID="gridViewSkin" 
        runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID"
        onrowcommand="grid_RowCommand" onrowdatabound="grid_RowDataBound">
        <Columns>
            <asp:BoundField DataField="Resumo" HeaderText="Descrição">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="AdicionalDescricao" HeaderText="Adicional">
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

<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="agregados.aspx.cs" Inherits="www.agregados" %>
<asp:Content ID="Content" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="474px">
        <tr><td rowspan="3"><img src="images/imgTitulos/regras_comercializacao.png" /></td></tr>
        <tr>
            <td><span class="titulo">Agregados e dependentes</span></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Selecione o contrato para exibir os agregados ou dependentes</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
<ContentTemplate>
    <table cellpadding="2" width="474px">
        <tr>
            <td class="tdNormal1" width="237px">Estipulante</td>
            <td class="tdNormal1" width="237px">Operadora</td>
        </tr>
        <tr>
            <td><asp:DropDownList ID="cboEstipulante" SkinID="dropdownSkin" runat="server" Width="99%" AutoPostBack="true" onselectedindexchanged="cboEstipulante_SelectedIndexChanged" /></td>
            <td>
                 <asp:DropDownList ID="cboOperadora" SkinID="dropdownSkin" runat="server" Width="99%" AutoPostBack="true" onselectedindexchanged="cboOperadora_SelectedIndexChanged" />
            </td>
        </tr>
        <tr>
            <td class="tdNormal1">Contrato</td>
            <td class="tdNormal1">Tipo</td>
        </tr>
        <tr>
            <td><asp:DropDownList ID="cboContrato" ForeColor="Red" SkinID="dropdownSkin" runat="server" Width="99%" AutoPostBack="true" onselectedindexchanged="cboContrato_SelectedIndexChanged" /></td>
            <td>
                <asp:RadioButton ID="optAgregado" runat="server" GroupName="a" Text="Agregado" Checked="true" oncheckedchanged="opt_CheckedChanged" AutoPostBack="true" />
                &nbsp;
                <asp:RadioButton ID="optDependente" runat="server" GroupName="a" Text="Dependente" oncheckedchanged="opt_CheckedChanged" AutoPostBack="true" />
            </td>
        </tr>
    </table>
    <br />
    <asp:GridView Width="474px" AutoGenerateColumns="False" ID="grid" SkinID="gridViewSkin" runat="server" DataKeyNames="ID" onrowcommand="grid_RowCommand" onrowdatabound="grid_RowDataBound">
        <Columns>
            <asp:BoundField  DataField="ParentescoDescricao" HeaderText="Parentesco">
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField  DataField="ParentescoCodigo" HeaderText="Código">
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />">
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField CommandName="editar" Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />">
                <HeaderStyle HorizontalAlign="Left" />
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
<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="adicionalRegra.aspx.cs" Inherits="www.adicionalRegra" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="474px">
        <tr><td rowspan="3"><img src="images/imgTitulos/regras_comercializacao.png" /></td></tr>
        <tr>
            <td><span class="titulo">Regra para produtos adicionais</span></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Informe os dados abaixo e clique em Salvar</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
<ContentTemplate>

       <table cellpadding="2" width="474px">
            <tr>
                <td class="tdNormal1" width="237px">Operadora</td>
                <td class="tdNormal1" width="237px">Produto</td>
            </tr>
            <tr>
                <td><asp:DropDownList ID="cboOperadora" SkinID="dropdownSkin" runat="server" Width="99%" AutoPostBack="true" onselectedindexchanged="cboOperadora_SelectedIndexChanged" /></td>
                <td><asp:DropDownList ID="cboProduto" SkinID="dropdownSkin" runat="server" Width="99%" /></td>
            </tr>
            <tr>
                <td class="tdNormal1" colspan="2">Restrição</td>
            </tr>
            <tr>
                <td colspan="2"><asp:DropDownList ID="cboRestricao" SkinID="dropdownSkin" runat="server" Width="99%" /></td>
            </tr>
        </table>
         <br />
        <table cellpadding="2" width="474px">
            <tr>
                <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
                <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
            </tr>
        </table>

</ContentTemplate>
</asp:UpdatePanel>

</asp:Content>

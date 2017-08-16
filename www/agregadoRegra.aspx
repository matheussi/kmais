<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="agregadoRegra.aspx.cs" Inherits="www.agregadoRegra" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="474px">
        <tr><td rowspan="3"><img src="images/imgTitulos/regras_comercializacao.png" /></td></tr>
        <tr>
            <td><span class="titulo">Regra para agregados ou dependentes</span></td>
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
                <td class="tdNormal1" width="237px">Contrato</td>
            </tr>
            <tr>
                <td><asp:DropDownList ID="cboOperadora" SkinID="dropdownSkin" runat="server" Width="99%" AutoPostBack="true" onselectedindexchanged="cboOperadora_SelectedIndexChanged" /></td>
                <td><asp:DropDownList ID="cboContrato" SkinID="dropdownSkin" runat="server" Width="99%" /></td>
            </tr>
            <tr>
                <td class="tdNormal1" width="237px">Restrição</td>
                <td class="tdNormal1" width="237px"></td>
            </tr>
            <tr>
                <td><asp:DropDownList ID="cboRestricao" SkinID="dropdownSkin" runat="server" Width="99%" /></td>
                <td><asp:TextBox ID="txtValor" SkinID="textboxSkin" runat="server" Width="50" /></td>
            </tr>
            <tr>
                <td colspan="2" class="tdNormal1">Tipo</td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:RadioButton ID="optAgregado" runat="server" GroupName="a" Text="Agregado" Checked="true"  />
                    &nbsp;
                    <asp:RadioButton ID="optDependente" runat="server" GroupName="a" Text="Dependente" />
                </td>
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
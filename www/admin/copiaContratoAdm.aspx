<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="copiaContratoAdm.aspx.cs" Inherits="www.admin.copiaContratoAdm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"></td></tr>
        <tr><td><span class="titulo">Copiar contratos administrativos</span></td></tr>
        <tr><td nowrap="true"><span class="subtitulo">Utilize os campos abaixo para copiar contratos administrativos (calendários, planos e tabelas de valor)</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="2">
                <tr>
                    <td class="tdPrincipal1" width="90px">Operadora</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboOperadora" Width="613px" runat="server" SkinID="dropdownSkin" AutoPostBack="true" OnSelectedIndexChanged="cboOperadora_changed" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Parâmetros</td>
                    <td class="tdNormal1">
                        Parâmetro 1:&nbsp;
                        <asp:TextBox ID="txtParam1" SkinID="textboxSkin" Width="80" MaxLength="100" runat="server" EnableViewState="false" />
                        &nbsp;&nbsp;&nbsp;
                        Parâmetro 2:&nbsp;
                        <asp:TextBox ID="txtParam2" SkinID="textboxSkin" Width="80" MaxLength="100" runat="server" EnableViewState="false" />
                        &nbsp;&nbsp;&nbsp;
                        Parâmetro 3:&nbsp;
                        <asp:TextBox ID="txtParam3" SkinID="textboxSkin" Width="80" MaxLength="100" runat="server" EnableViewState="false" />
                        &nbsp;
                        <asp:ImageButton ID="imgFiltrar" ImageUrl="~/images/endereco.png" runat="server" OnClick="imgFiltrar_click" ToolTip="refinar..." />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Contrato Adm.</td>
                    <td class="tdNormal1"><asp:ListBox ID="cboContratoAdm" Width="613px" runat="server" SkinID="listBoxSkin" Rows="7" SelectionMode="Multiple" /></td>
                </tr>
                <tr>
                    <td align="center" colspan="2" class="tdNormal1">
                        <asp:Button Width="130" SkinID="botaoAzulBorda" ID="cmdModelo" runat="server" Text="selecionar modelo" OnClick="cmdModelo_click" />
                        &nbsp;&nbsp;&nbsp;
                        <asp:Button Width="130" SkinID="botaoAzulBorda" ID="cmdDestino" runat="server" Text="selecionar destinos" OnClick="cmdDestino_click" />
                    </td>
                </tr>
            </table>
            <br />
            <table cellpadding="2" cellspacing="2">
                <tr>
                    <td class="tdPrincipal1" width='50%'>Modelo</td>
                    <td class="tdPrincipal1" width='50%'>Destino</td>
                </tr>
                <tr>
                    <td class="tdNormal1" valign="top">
                        <asp:DropDownList ID="cboModeloSelecionado" Width="350px" runat="server" SkinID="dropdownSkin" />
                    </td>
                    <td class="tdNormal1">
                        <asp:ListBox ID="lstDestinosSelecionados" Width="350px" runat="server" SkinID="listBoxSkin" Rows="7" SelectionMode="Multiple" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Button Width="130" SkinID="botaoAzulBorda"     ID="cmdReset"  runat="server" Text="desfazer seleção" OnClick="cmdReset_click" />
                        &nbsp;&nbsp;&nbsp;
                        <asp:Button Width="130" SkinID="botaoVermelhoBorda" ID="cmdCopiar" runat="server" Text="efetivar cópia" OnClientClick="return confirm('Atenção!\nDeseja realmente realizar a cópia?\nEssa ação NÃO poderá ser defeita.');" OnClick="cmdCopiar_click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
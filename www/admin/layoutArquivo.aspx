<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="layoutArquivo.aspx.cs" Inherits="www.admin.layoutArquivo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Layout de arquivo</span></td></tr>
        <tr><td><span class="subtitulo">Detalhe do Layout de arquivo</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<script language="javascript" type="text/javascript" src="../js/common.js"></script>
   <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table width="420px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                <tr>
                    <td colspan="2" align="center" class="tdNormal1"><b>Configurações</b></td>
                </tr>
                <tr>
                    <td width="106" class="tdPrincipal1">Operadora</td>
                    <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadora" Width="100%" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Nome do arquivo</td>
                    <td class="tdNormal1"><asp:TextBox SkinID="textboxSkin" runat="server" ID="txtNomeArquivo" Width="200" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Tipo do arquivo</td>
                    <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboTipo" Width="100%" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Formato do arquivo</td>
                    <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboFormato" Width="100%" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Delimitador</td>
                    <td class="tdNormal1"><asp:TextBox ID="txtDelimitador" MaxLength="1" Font-Bold="true" Width="30" SkinID="textboxSkin" runat="server" /></td>
                </tr>
            </table>
            <br />
            <table width="420px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                <tr>
                    <td colspan="5" align="center" class="tdNormal1"><b>Campos do arquivo</b></td>
                </tr>
                <tr>
                    <td width="80" class="tdPrincipal1">Seção</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboSecao" SkinID="dropdownSkin" runat="server" Width="110" /></td>
                    <td width="80" align="center" class="tdPrincipal1">Tipo</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboTipoValor" SkinID="dropdownSkin" runat="server" Width="110" /></td>
                </tr>
                <tr>
                    <td width="80" class="tdPrincipal1">Tipo de dado</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboTipoDado" SkinID="dropdownSkin" runat="server" Width="110" /></td>
                    <td width="80" align="center" class="tdPrincipal1">Formato</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboFormatoItem" SkinID="dropdownSkin" runat="server" Width="110" /></td>
                </tr>
                <tr>
                    <td width="80" class="tdPrincipal1">Comportamento</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboBehavior" SkinID="dropdownSkin" runat="server" Width="110" AutoPostBack="true" OnSelectedIndexChanged="cboBehavior_SelectedIndexChanged" /></td>
                    <td width="80" align="center" class="tdPrincipal1">Preenchimento</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboTipoPreenchimento" SkinID="dropdownSkin" runat="server" Width="110" /></td>
                </tr>
                <tr>
                    <td width="80" class="tdPrincipal1">Tamanho</td>
                    <td class="tdNormal1" colspan="3"><asp:TextBox ID="txtTamanho" MaxLength="5" SkinID="textboxSkin" runat="server" Width="30" /></td>
                </tr>
                <tr>
                    <td width="80" class="tdPrincipal1">Valor</td>
                    <td class="tdNormal1" colspan="3">
                        <asp:DropDownList ID="cboValor" runat="server" SkinID="dropdownSkin" Width="100%" />
                        <asp:TextBox Visible="false" ID="txtValor" SkinID="textboxSkin" runat="server" Width="99%" />
                    </td>
                </tr>
            </table>
            <br />
            <table width="420px">
                <tr>
                    <td align="right">
                        <asp:Button EnableViewState="false" runat="server" ID="cmdAdd" SkinID="botaoAzulBorda" Text="Adicionar" Width="80" onclick="cmdAdd_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:GridView ID="grid" Width="420px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="false" AutoGenerateColumns="False"  DataKeyNames="ID"
                OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="strSecao" HeaderText="Seção">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="strTipoValor" HeaderText="Tipo">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Campo" HeaderText="Campo">
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
            <br /><br />
            <table cellpadding="2" cellspacing="0" style="border: solid 1px #507CD1" width="420px">
                <tr>
                    <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" Width="80" /></td>
                    <td align="right"><asp:CheckBox ID="chkNovo" Text="salvar como novo" EnableViewState="false" runat="server" />&nbsp;<asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" Width="80" /></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
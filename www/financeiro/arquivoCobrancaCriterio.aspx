<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="arquivoCobrancaCriterio.aspx.cs" Inherits="www.financeiro.arquivoCobrancaCriterio" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Cobranças</span></td></tr>
        <tr><td><span class="subtitulo">Gerar critérios para arquivos de remessa</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table width="620px" cellpadding="0" cellspacing="0" style="border: solid 1px #507CD1">
                <tr>
                    <td width="50%" valign="top">
                        <table width="100%" cellpadding="4" cellspacing="0">
                            <tr>
                                <td class="tdPrincipal1">Operadoras</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:ListBox Height="100" SelectionMode="Single" SkinID="listBoxSkin" runat="server" ID="lstOperadoras" Width="100%" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Button ID="cmdRefOperadoraContratoAdm" Text="filtrar contratos" SkinID="botaoPequeno" runat="server" OnClick="cmdRefinarContratoAdm_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td width="50%" valign="top">
                        <table width="100%" cellpadding="4" cellspacing="0">
                            <tr>
                                <td class="tdPrincipal1">Estipulantes</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:ListBox Height="100" SelectionMode="Multiple" SkinID="listBoxSkin" runat="server" ID="lstEstipulantes" Width="100%" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Button ID="Button1" Text="filtrar contratos" SkinID="botaoPequeno" runat="server" OnClick="cmdRefinarContratoAdm_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <br />
            <table width="620px" cellpadding="0" cellspacing="0" style="border: solid 1px #507CD1">
                <tr>
                    <td width="50%" valign="top">
                        <table width="100%" cellpadding="4" cellspacing="0">
                            <tr>
                                <td class="tdPrincipal1">Contratos Adm</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:ListBox Height="112" SelectionMode="Multiple" SkinID="listBoxSkin" runat="server" ID="lstContrato" Width="100%" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td width="50%" valign="top">
                        <table width="100%" cellpadding="4" cellspacing="0">
                            <tr>
                                <td class="tdPrincipal1">Demais critérios</td>
                            </tr>
                            <tr>
                                <td>
                                    Projeto<br />
                                    <asp:TextBox ID="txtProjeto" Width="280" runat="server" SkinID="textboxSkin" MaxLength="200" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Descrição<br />
                                    <asp:TextBox ID="txtDescricao" Width="280" runat="server" SkinID="textboxSkin" MaxLength="200" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Nome do arquivo<br />
                                    <asp:TextBox ID="txtNomeArquivo" Width="280" runat="server" SkinID="textboxSkin" MaxLength="200" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Fone de atendimento<br />
                                    <asp:TextBox ID="txtFone" Width="280" runat="server" SkinID="textboxSkin" MaxLength="200" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Operadora<br />
                                    <asp:TextBox ID="txtOperadora" Width="280" runat="server" SkinID="textboxSkin" MaxLength="200" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    ANS<br />
                                    <asp:TextBox ID="txtAns" Width="80" runat="server" SkinID="textboxSkin" MaxLength="200" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Filtro para taxa associativa
                                    <br />
                                    <asp:DropDownList ID="cboFiltroTaxa" Width="280" runat="server" SkinID="dropdownSkin" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="center">
                                    <asp:Button Text="Salvar" ID="cmdSalvar" SkinID="botaoAzulBorda" runat="server" OnClick="cmdSalvar_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <br />
            <br />
            <asp:Literal ID="litMsg" runat="server" EnableViewState="true" />
            <asp:GridView ID="grid" Width="620px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ID"
                OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" 
                onrowcreated="grid_RowCreated">
                <Columns>
                    <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Projeto" HeaderText="Projeto">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                     <asp:TemplateField>
                        <ItemStyle Width="1%" />
                        <ItemTemplate>
                            <asp:ImageButton CommandName="excluir" ID="imgExcluir" AlternateText="Excluir" ToolTip="Excluir" ImageUrl="~/images/delete.png" runat="server" />
                        </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField>
                        <ItemStyle Width="1%" />
                        <ItemTemplate>
                            <asp:ImageButton CommandName="editar" ID="imgEditar" AlternateText="Editar" ToolTip="Editar" ImageUrl="~/images/edit.png" runat="server" />
                        </ItemTemplate>
                     </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
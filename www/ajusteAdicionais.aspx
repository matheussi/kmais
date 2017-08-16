<%@ Page Language="C#" Theme="Theme" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="ajusteAdicionais.aspx.cs" Inherits="www.ajusteAdicionais" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"></td></tr>
        <tr><td><span class="titulo">Reajustes de adicionais</span></td></tr>
        <tr><td nowrap="true"><span class="subtitulo">Utilize os campos abaixo para reajustar os adicionais vigentes</span></td></tr>
    </table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel ID="up" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" width="474px">
                <tr>
                    <td class="tdNormal1" colspan="2">Destino</td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:DropDownList ID="cboDestino" runat="server" SkinID="dropdownSkin" Width="100%">
                            <asp:ListItem Text="PSCC" Value="1" Selected="True" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="tdNormal1" colspan="2">Descrição</td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="txtDescricao" runat="server" Width="100%" SkinID="textboxSkin" MaxLength="250" />
                    </td>
                </tr>
                <tr>
                    <td class="tdNormal1" width="237px">Órgão</td>
                    <td class="tdNormal1" width="237px">Forma Pagto</td>
                </tr>
                <tr>
                    <td>
                        <asp:ListBox SkinID="listBoxSkin" SelectionMode="Multiple" Width="99%" Rows="6" runat="server" ID="cboContratoADM" />
                    </td>
                    <td valign="top">
                         <asp:DropDownList SkinID="dropdownSkin" ID="cboFormaPagto" runat="server" Width="99%">
                            <asp:ListItem Value="00" Text="todos" />
                            <asp:ListItem Value="31" Text="Boleto" />
                            <asp:ListItem Value="09" Text="Crédito" />
                            <asp:ListItem Value="10" Text="Débito" />
                            <asp:ListItem Value="81" Text="Desconto em conta" />
                            <asp:ListItem Value="11" Text="Desconto em folha" />
                        </asp:DropDownList>
                    </td>
                </tr>

                <tr>
                    <td class="tdNormal1" width="237px">Tipo</td>
                    <td class="tdNormal1" width="237px">Adicional</td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ID="cboTipoAdicional" SkinID="dropdownSkin" runat="server" Width="99%" AutoPostBack="true" onselectedindexchanged="cboTipoAdicional_SelectedIndexChanged">
                            <asp:ListItem Text="Previdência" Value="2" />
                            <asp:ListItem Text="Seguro" Value="1" />
                            <asp:ListItem Text="Taxa" Value="0" Selected="True" />
                        </asp:DropDownList>
                    </td>
                    <td>
                         <asp:DropDownList ID="cboAdicional" SkinID="dropdownSkin" runat="server" Width="99%" />
                    </td>
                </tr>
                <tr>
                    <td class="tdNormal1">% Prêmio</td>
                    <td class="tdNormal1">% Cobertura</td>
                </tr>
                <tr>
                    <td><asp:TextBox ID="txtPremio"    SkinID="textboxSkin" runat="server" Width="40%" MaxLength="7" Text="0,00" /></td>
                    <td><asp:TextBox ID="txtCobertura" SkinID="textboxSkin" runat="server" Width="40%" MaxLength="7" Text="0,00" /></td>
                </tr>
            </table>
            <br />
            <table width="474px">
                <tr>
                    <td align="left">
                        <asp:Button EnableViewState="false" runat="server" ID="cmdVisualizar" SkinID="botaoAzulBorda" Text="Vizualizar reajuste" Width="180" onclick="cmdVisualizar_click" />
                    </td>
                    <td align="right">
                        <asp:Button EnableViewState="false" runat="server" ID="cmdAtribuirReajuste" SkinID="botaoVermelhoBorda" Text="Atribuir reajuste" Width="180" onclick="cmdAtribuirReajuste_Click" OnClientClick="return confirm('Deseja realmente atribuir o reajuste?\nEssa operação não poderá ser desfeita.');" />
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlResultado" runat="server" Visible="false">
                <table cellpadding="2" width="100%">
                    <tr><td><h2>Resultado</h2></td></tr>
                    <tr>
                        <td>
                            <asp:GridView ID="grid" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="true" PageSize="1000" OnPageIndexChanging="grid_OnPageChanging" PagerSettings-Position="TopAndBottom" AutoGenerateColumns="False" >
                                <Columns>
                                    <asp:BoundField DataField="contratoadm_descricao" HeaderText="Órgão">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="contrato_numero" HeaderText="Matr.Funcional">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="beneficiario_nome" HeaderText="Beneficiário">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="adicional_descricao" HeaderText="Adicional">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="tipo" HeaderText="Adicional Tipo">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="pagto" HeaderText="Forma Pagto">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="valorAtual" HeaderText="ValorAtual">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="valorReajuste" HeaderText="ValorReajustado">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

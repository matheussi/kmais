<%@ Page Language="C#" Theme="Theme" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="repTotalPorAdicinal.aspx.cs" Inherits="www.relatorios.repTotalPorAdicinal" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo"><asp:Literal ID="litTitulo" Text="Relatório Totais por Adicional" runat="server" /></span></td></tr>
        <tr><td nowrap><span class="subtitulo">Utilize os filtros abaixo para gerar o relatório</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">

    <asp:UpdatePanel ID="up" runat="server" UpdateMode="Always">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdToExcel" />
        </Triggers>
        <ContentTemplate>
            <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td width="100%" valign="top" colspan="2">
                        <table cellpadding="2" cellspacing="1" style="border-bottom: solid 1px #507CD1;border-top: solid 1px #507CD1;border-right: solid 1px #507CD1;border-left: solid 1px #507CD1" width="100%" >
                             <tr>
                                <td class="tdPrincipal1" valign="top" width="130px">Órgãos averbadores</td>
                                <td class="tdNormal1" valign="top">
                                    <asp:DropDownList ID="cboOrgaos" runat="server" SkinID="dropdownSkin" Width="100%"></asp:DropDownList>
                                </td>
                             </tr>
                        </table><%----%>
                    </td>
                </tr>
                <tr id="trFiltroTaxas" runat="server" visible="true">
                    <td width="50%" valign="top">
                        <table cellpadding="2" cellspacing="1" style="border-bottom: solid 1px #507CD1;border-left: solid 1px #507CD1;height:128px" width="100%" >
                             <tr>
                                <td class="tdPrincipal1" valign="top" width="130px">Adicionais</td>
                                <td class="tdNormal1" valign="top">
                                    <asp:CheckBoxList ID="cblTiposAdicionais" runat="server" >
                                        <asp:ListItem Text="Previdência" Value="2" Selected="True" />
                                        <asp:ListItem Text="Seguro" Value="1" Selected="True" />
                                        <asp:ListItem Text="Taxa" Value="0" Selected="True" />
                                    </asp:CheckBoxList>
                                </td>
                             </tr>
                        </table>
                    </td>
                    <td width="50%" valign="top">
                        <table cellpadding="2" cellspacing="1" style="border-left: solid 1px #507CD1;border-right: solid 1px #507CD1;border-bottom: solid 1px #507CD1;height:128px" width="100%" >
                             <tr>
                                <td class="tdPrincipal1" valign="top" width="130px">Formas de Pagto</td>
                                <td class="tdNormal1" valign="top">
                                    <asp:CheckBoxList ID="cblFormasPato" runat="server" >
                                        <asp:ListItem Text="Boleto" Value="31" Selected="True" />
                                        <asp:ListItem Text="Crédito" Value="09" Selected="false" />
                                        <asp:ListItem Text="Débito" Value="10" Selected="false" />
                                        <asp:ListItem Text="Desconto em conta" Value="81" Selected="false" />
                                        <asp:ListItem Text="Desconto em folha" Value="11" Selected="true" />
                                    </asp:CheckBoxList>
                                </td>
                             </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td width="100%" valign="top" colspan="2">
                        <table cellpadding="2" cellspacing="1" style="border: solid 0px #507CD1;" width="100%" >
                            <tr>
                                <td align="center">
                                    <asp:Button ID="cmdEmitir" Text="Emitir relatório" SkinID="botaoAzulBorda" runat="server" OnClick="cmdEmitir_click" />
                                    &nbsp;
                                    <asp:ImageButton Visible="false" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="Top" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" style="width: 16px" />
                                    <asp:Literal ID="litTotais" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

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
                    <asp:BoundField DataField="Total" HeaderText="Valor">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

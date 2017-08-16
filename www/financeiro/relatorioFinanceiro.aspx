<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="relatorioFinanceiro.aspx.cs" Inherits="www.financeiro.relatorioFinanceiro" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo">Relatório Financeiro</span></td></tr>
        <tr><td nowrap="nowrap"><span class="subtitulo">Selecione as opções abaixo para exibir o relatório desejado</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdToExcel" />
            <asp:PostBackTrigger ControlID="cmdToExcelRecebiveis" />
        </Triggers>
        <ContentTemplate>
            <table>
                <tr>
                    <td><b><asp:LinkButton ID="lnkAgendar" Text="Agendar processamento de novo relatório" runat="server" OnClick="lnkAgendar_Click" /></b></td>
                </tr>
                <asp:Panel runat="server" ID="pnlAgendar" Visible="false">
                <tr>
                    <td>
                        <table width="100%" cellpadding="2" cellspacing="1">
                         <tr>
                            <td class="tdPrincipal1">Filial</td>
                            <td class="tdNormal1">
                                <asp:ListBox ID="lstFilial" Rows="4" SelectionMode="Multiple" runat="server" SkinID="listBoxSkin" Width="225" />
                            </td>
                         </tr>
                         <tr>
                            <td class="tdPrincipal1">Estipulante</td>
                            <td class="tdNormal1">
                                <asp:ListBox ID="lstEstipulantes" Rows="4" SelectionMode="Multiple" runat="server" SkinID="listBoxSkin" Width="225" />
                            </td>
                         </tr>
                         <tr>
                            <td class="tdPrincipal1" width="90">Operadora</td>
                            <td class="tdNormal1"><asp:ListBox Rows="4" ID="lstOperadora" SelectionMode="Multiple" SkinID="listBoxSkin" Width="225"  runat="server" /></td>
                         </tr>
                         <tr>
                            <td class="tdPrincipal1" valign="top">Vencimento</td>
                            <td class="tdNormal1" valign="top">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td>
                                            De
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDe" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                        </td>
                                    </tr>
                                    <tr><td height="8"></td></tr>
                                    <tr>
                                        <td>
                                            Até
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtAte" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                         </tr>
                            <tr>
                                <td class="tdNormal1" colspan="2">
                                    Processar Em  
                                    <asp:TextBox ID="txtProcessarEm" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    &nbsp;
                                    <asp:DropDownList runat="server" ID="cboHora" Width="40" SkinID="dropdownSkin" />&nbsp;:&nbsp;<asp:DropDownList runat="server" ID="cboMinuto" Width="40" SkinID="dropdownSkin" />
                                    &nbsp;
                                    <asp:Button ID="cmdSalvar" Text="Salvar" SkinID="botaoAzulBorda" OnClick="cmdSalvar_Click" runat="server" EnableViewState="false" OnClientClick="return confirm('Confirma a solicitação de processamento?');" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                </asp:Panel>
            </table>
            <br />
            <table>
                <tr>
                    <td>
                        <b><asp:LinkButton ID="lnkExibir" Text="Exibir relatório" runat="server" OnClick="lnkExibir_Click" /></b>
                    </td>
                </tr>
                <asp:Panel runat="server" ID="pnlExibir" Visible="false">
                <tr>
                    <td><asp:Button Visible="false" ID="cmdRecebidosPdf" Text="Relatório de Recebidos" SkinID="botaoAzulBorda" runat="server" onclick="cmdRecebidosPdf_Click" /></td>
                    <td><asp:Button Visible="false" ID="cmdRecebiveisExcel" Text="Relatório de Recebíveis - Excel" SkinID="botaoAzulBorda" runat="server" onclick="cmdRecebiveisExcel_Click" /></td>
                    <td><asp:Button Visible="false" ID="cmdRecebiveisPdf" Text="Relatório de Recebíveis" SkinID="botaoAzulBorda" runat="server" onclick="cmdRecebiveisPdf_Click" /></td>
                    <td><asp:Button Visible="false" ID="cmdRecebidosExcel" Text="Relatório de Recebidos - Excel" SkinID="botaoAzulBorda" runat="server" onclick="cmdRecebidosExcel_Click" /></td>
                </tr>
                </asp:Panel>
            </table>
            <asp:Panel ID="pnlRecebidos" runat="server" Visible="false">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td width="50%" valign="top"><asp:Literal runat="server" ID="litCorpoRecebiveis" /></td>
                        <td width="50%" valign="top"><asp:Literal runat="server" ID="litCorpo" /></td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlRecebidosGRID" runat="server" Visible="false">
                <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" style="width: 16px" />
                <asp:GridView ID="grid" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="false" AutoGenerateColumns="False" >
                    <Columns>
                        <asp:BoundField DataField="REF_MES_VENCTO_BOLETADOS" HeaderText="REF. MÊS VENCTO. BOLETADOS" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="REF_MES_VENCTO_REATIVACOES" HeaderText="REF. MÊS VENCTO. REATIVAÇÕES" DataFormatString="{0:N2}" ItemStyle-Wrap="false">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TOTAL_PARCIAL" HeaderText="TOTAL PARCIAL" DataFormatString="{0:N2}" ItemStyle-Wrap="false">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ANTECIPACOES" HeaderText="ANTECIPAÇÕES" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PAGTOS_EM_ATRASO" HeaderText="PAGTOS. EM ATRASO" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TOTAL_NET" HeaderText="TOTAL NET" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="MULTA_REF_MES_VENCTO" HeaderText="MULTA REF. MÊS VENCTO" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="MULTA_PAGTOS_EM_ATRASO" HeaderText="MULTA PAGTOS. EM ATRASO" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TOTAL_JUROS_MULTA" HeaderText="TOTAL JUROS E MULTA" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TAXAS_REF_MES_VENCTO_BOLETADOS" HeaderText="TAXAS REF. MÊS VENCTO. BOLETADOS" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TAXAS_REF_MES_VENCTO_REATIVACOES" HeaderText="TAXAS REF. MÊS VENCTO. REATIVAÇÕES" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TAXAS_ATENCIPACOES" HeaderText="TAXAS ANTECIPAÇÕES" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TAXAS_PAGTOS_EM_ATRASO" HeaderText="TAXAS PAGTOS. EM ATRASO" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TOTAL_TAXAS" HeaderText="TOTAL TAXAS E JUROS" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OVER_REF_MES_VENCTO" HeaderText="OVER REF. MÊS VENCTO." DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OVER_ANTECIPACOES" HeaderText="OVER ANTECIPAÇÕES" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OVER_ATRASOS" HeaderText="OVER ATRASOS" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
            <asp:Panel ID="pnlRecebiveis" runat="server" Visible="false">
                
            </asp:Panel>
            <asp:Panel ID="pnlRecebiveisGRID" runat="server" Visible="false">
                <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcelRecebiveis" OnClick="cmdToExcelRecebiveis_Click" style="width: 16px" />
                <asp:GridView ID="gridRecebiveis" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="false" AutoGenerateColumns="False" >
                    <Columns>
                        <asp:BoundField DataField="NET" HeaderText="NET" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TAXAS" HeaderText="TAXAS SIND / BCO" DataFormatString="{0:N2}" ItemStyle-Wrap="false">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OVER" HeaderText="OVER PRICE" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="SUBTOTAL" HeaderText="SUBTOTAL" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="AGENCIAMENTO" HeaderText="AGENCIAMENTO" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="VITALICIO" HeaderText="VITALÍCIO" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TOTAL" HeaderText="TOTAL" DataFormatString="{0:N2}">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
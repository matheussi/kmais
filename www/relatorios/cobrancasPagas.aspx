<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="cobrancasPagas.aspx.cs" Inherits="www.relatorios.cobrancasPagas" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo">Relatório de cobranças </span></td></tr>
        <tr><td nowrap><span class="subtitulo">Utilize os filtros abaixo para gerar o relatório</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1" >
         <tr>
            <td class="tdPrincipal1">Estipulante</td>
            <td class="tdNormal1">
                <asp:ListBox ID="lstEstipulantes" Rows="2" SelectionMode="Multiple" runat="server" SkinID="listBoxSkin" Width="225" />
            </td>
         </tr>
         <tr>
            <td class="tdPrincipal1" width="120">Operadora</td>
            <td class="tdNormal1"><asp:ListBox Rows="2" ID="lstOperadora" SelectionMode="Multiple" SkinID="listBoxSkin" Width="225"  runat="server" /></td>
         </tr>
         <tr>
            <td class="tdPrincipal1" valign="top">Pagamento</td>
            <td class="tdNormal1" valign="top">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            De
                        </td>
                        <td>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDe" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                            <asp:Image SkinID="imgCanlendario" ID="imgDe" runat="server" EnableViewState="false" />
                            <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtDe" PopupButtonID="imgDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        </td>
                    </tr>
                    <tr><td height="8"></td></tr>
                    <tr>
                        <td>
                            Até
                        </td>
                        <td>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtAte" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                            <asp:Image SkinID="imgCanlendario" ID="imgAte" runat="server" EnableViewState="false" />
                            <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAte" TargetControlID="txtAte" PopupButtonID="imgAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        </td>
                    </tr>
                </table>
            </td>
         </tr>

         <tr>
            <td class="tdNormal1" align="left">
                <asp:CheckBox ID="chkAgendar" Visible="false" EnableViewState="false" Text="agendar" SkinID="checkboxSkin" runat="server" AutoPostBack="true" oncheckedchanged="chkAgendar_CheckedChanged" />
            </td>
            <td class="tdNormal1" align="right"><asp:Button ID="cmdGerar" OnClick="cmdGerar_Click" SkinID="botaoAzul" Text="Gerar" runat="server" Width="80" />&nbsp;<font color='red'><asp:Literal ID="lblGerarArquivoMessage" runat="server" EnableViewState="false" /></font></td>
         </tr>
    </table>
    <asp:Panel runat="server" ID="pnl" Visible="false">
        <br />
        <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" style="width: 16px" />
        <div>
        <asp:GridView ID="grid" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="false" AutoGenerateColumns="False" >
            <Columns>
                <asp:BoundField DataField="beneficiario_nome" HeaderText="Beneficiário">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="beneficiario_matriculaAssociativa" HeaderText="M.Associativa">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="beneficiario_matriculaFuncional" HeaderText="M.Funcional">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>

                <asp:BoundField DataField="cobranca_valor" HeaderText="Valor" DataFormatString="{0:N2}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_dataVencimento" HeaderText="Vencto." DataFormatString="{0:dd/MM/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="pago" HeaderText="Pago" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_dataPagto" HeaderText="Pagamento" DataFormatString="{0:dd/MM/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
        </div>
    </asp:Panel>
</asp:Content>

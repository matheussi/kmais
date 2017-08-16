<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="relatorioEntidades.aspx.cs" Inherits="www.movimentacao.relatorioEntidades" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo">Relatório de entidades</span></td></tr>
        <tr><td><span class="subtitulo">Utilize os filtros abaixo para gerar o relatório</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1" >
         <tr>
            <td class="tdPrincipal1">Estipulante</td>
            <td class="tdNormal1">
                <asp:ListBox ID="lstEstipulantes" Rows="4" SelectionMode="Multiple" runat="server" SkinID="listBoxSkin" Width="225" />
            </td>
         </tr>
         <tr>
            <td class="tdPrincipal1" width="120">Operadora</td>
            <td class="tdNormal1"><asp:ListBox Rows="4" ID="lstOperadora" SelectionMode="Multiple" SkinID="listBoxSkin" Width="225"  runat="server" /></td>
         </tr>
         <tr>
            <td class="tdPrincipal1">Propostas ativas</td>
            <td class="tdNormal1">
                <asp:DropDownList ID="cboAtivas" Width="225" SkinID="dropdownSkin" runat="server" />
            </td>
         </tr>
         <tr>
            <td class="tdPrincipal1">Taxas de sindical.</td>
            <td class="tdNormal1"><asp:DropDownList ID="cboTaxas" Width="225" SkinID="dropdownSkin" runat="server" /></td>
         </tr>
         <tr>
            <td class="tdPrincipal1" valign="top">Vigência</td>
            <td class="tdNormal1" valign="top">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>De</td>
                        <td>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDe" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                            <asp:Image SkinID="imgCanlendario" ID="imgDe" runat="server" EnableViewState="false" />
                            <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtDe" PopupButtonID="imgDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        </td>
                    </tr>
                    <tr><td height="8"></td></tr>
                    <tr>
                        <td>Até</td>
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
            <%--<td class="tdPrincipal1">Taxas de sindical.</td>--%>
            <td colspan="2" class="tdNormal1"><asp:CheckBox ID="chkSomentePagas" Text="Somente com a primeira parcela bancária paga" SkinID="checkboxSkin" EnableTheming="false" runat="server" /></td>
         </tr>
         <tr>
            <td class="tdNormal1" colspan="2" align="right"><asp:Button ID="cmdGerar" SkinID="botaoAzul" Text="Gerar" runat="server" Width="80" onclick="cmdGerar_Click" />&nbsp;<font color='red'><asp:Literal ID="lblGerarArquivoMessage" runat="server" EnableViewState="false" /></font></td>
         </tr>
    </table>
    <asp:Panel runat="server" ID="pnlResultado" Visible="false">
        <br />
        <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" />
        <asp:GridView ID="grid" Width="700px" SkinID="gridViewSkin" runat="server" AllowPaging="false" AutoGenerateColumns="False" >
            <Columns>
                <asp:BoundField DataField="operadora_nome" HeaderText="Operadora">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contrato_numero" HeaderText="Contrato">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contratoadm_descricao" HeaderText="Contrato ADM" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contratoadm_contratoSaude" HeaderText="Contrato Saúde" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="beneficiario_nome" HeaderText="Beneficiário">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="beneficiario_cpf" HeaderText="Documento">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                
                <asp:BoundField DataField="contrato_vigencia" HeaderText="Vigência" DataFormatString="{0:dd/MM/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_valor" HeaderText="Valor" >
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>

                <asp:BoundField DataField="cobranca_valorPagto" HeaderText="Valor pago" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="qtdvidas" HeaderText="Vidas" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
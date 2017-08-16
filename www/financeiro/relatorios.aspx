<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="relatorios.aspx.cs" Inherits="www.financeiro.relatorios" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td><span class="titulo">Relatórios</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" style="border: solid 1px #507CD1" width="430">
                <tr>
                    <td>
                        <table border="0" cellpadding="3" cellspacing="1" width="100%">
                            <tr>
                                <td class="tdPrincipal1" width="150">Tipo</td>
                                <td class="tdNormal1">
                                    <asp:DropDownList ID="ddlTipo"  runat="server" SkinID="dropdownSkin" Width="225" AutoPostBack="true" OnSelectedIndexChanged="ddlTipo_OnSelectedIndexChanged">
                                        <asp:ListItem Value="-1" Text="Selecione" Selected="true"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Comissionamento"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr runat="server" visible="false" id="trComissionamento">
                    <td colspan="2">
                        <table border="0" cellpadding="3" cellspacing="1" width="100%">
                            <tr runat="server" visible="false" id="trPeriodo">
                                <td class="tdPrincipal1">Período</td>
                                <td class="tdNormal1">
                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td>De:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                            <td>
                                                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtPeriodoDe" Width="65" AutoPostBack="true" OnTextChanged="txtPeriodoChange" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                <asp:Image SkinID="imgCanlendario" ID="imgVigDe" runat="server" EnableViewState="false" />
                                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtPeriodoDe" PopupButtonID="imgVigDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                            </td>
                                            <td>Até:&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                            <td>
                                                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtPeriodoAte" Width="65" AutoPostBack="true" OnTextChanged="txtPeriodoChange" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                <asp:Image SkinID="imgCanlendario" ID="imgVigAte" runat="server" EnableViewState="false" />
                                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAte" TargetControlID="txtPeriodoAte" PopupButtonID="imgVigAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="150">Listagem</td>
                                <td class="tdNormal1"><asp:DropDownList ID="lstListagem" runat="server" SkinID="dropdownSkin" Width="225" AutoPostBack="true" OnSelectedIndexChanged="lstListagem_OnSelectedIndexChanged"></asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td colspan='2'>
                                    <asp:CheckBox ID="chkSemRecibo" Text="sem recibo" runat="server" EnableViewState="false" />&nbsp;&nbsp;
                                    <asp:CheckBox ID="chkConsolidadoBanco" Text="Consolidado por banco" runat="server" EnableViewState="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="150">Perfil</td>
                                <td class="tdNormal1"><asp:ListBox ID="lstPerfil" runat="server" SkinID="listBoxSkin" SelectionMode="Multiple" Width="225" AutoPostBack="false" /></td>
                            </tr>
                            <tr>
                                <td class="tdNormal1" colspan='2' style="border-top: solid 1px #507CD1">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <td align="left">
                                            <asp:Button ID="cmdAnteriores" Width="150" Text="Listagens Anteriores" SkinID="botaoAzulBorda" runat="server" OnClick="cmdAnteriores_Click" />
                                        </td>
                                        <td align="right">
                                            <asp:Button ID="cmdGerar" Width="150" Text="Exibir" SkinID="botaoAzulBorda" runat="server" OnClick="cmdGerar_Click" />
                                        </td>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr runat="server" id="trReportViewer" visible="false">
                    <td>
                       <%-- <dx:ReportToolbar ID="rptRelatorioViewerToolbar" runat="server" ReportViewer="<%# rptRelatorioViewer %>">
                            <Items>
                                <dx:ReportToolbarButton ItemKind='Search' ToolTip='Display the search window' />
                                <dx:ReportToolbarSeparator />
                                <dx:ReportToolbarButton ItemKind='PrintReport' ToolTip='Print the report' />
                                <dx:ReportToolbarButton ItemKind='PrintPage' ToolTip='Print the current page' />
                                <dx:ReportToolbarSeparator />
                                <dx:ReportToolbarButton Enabled='False' ItemKind='FirstPage' ToolTip='First Page' />
                                <dx:ReportToolbarButton Enabled='False' ItemKind='PreviousPage' ToolTip='Previous Page' />
                                <dx:ReportToolbarLabel Text='Page' />
                                <dx:ReportToolbarComboBox ItemKind='PageNumber' Width='65px'>
                                </dx:ReportToolbarComboBox>
                                <dx:ReportToolbarLabel Text="of" />
                                <dx:ReportToolbarTextBox IsReadOnly='True' ItemKind='PageCount' />
                                <dx:ReportToolbarButton ItemKind='NextPage' ToolTip='Next Page' />
                                <dx:ReportToolbarButton ItemKind='LastPage' ToolTip='Last Page' />
                                <dx:ReportToolbarSeparator />
                                <dx:ReportToolbarButton ItemKind='SaveToDisk' ToolTip='Export a report and save it to the disk' />
                                <dx:ReportToolbarButton ItemKind='SaveToWindow' ToolTip='Export a report and show it in a new window' />
                                <dx:ReportToolbarComboBox ItemKind='SaveFormat' Width='70px'>
                        <Elements>
                            <dx:ListElement Text='Pdf' Value='pdf' />
                            <dx:ListElement Text='Xls' Value='xls' />
                            <dx:ListElement Text='Xlsx' Value='xlsx' />
                            <dx:ListElement Text='Rtf' Value='rtf' />
                            <dx:ListElement Text='Mht' Value='mht' />
                            <dx:ListElement Text='Html' Value='html' />
                            <dx:ListElement Text='Text' Value='txt' />
                            <dx:ListElement Text='Image' Value='png' />
                            <dx:ListElement Text='Csv' Value='csv' />
                        </Elements>
                    </dx:ReportToolbarComboBox>
                            </Items>
                        </dx:ReportToolbar> 
                        <dx:ReportViewer ID="rptRelatorioViewer" runat="server" ClientInstanceName="rptRelatorioViewer"></dx:ReportViewer>--%>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="relatorioGeral.aspx.cs" Inherits="www.movimentacao.relatorioGeral" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo">Relatório geral</span></td></tr>
        <tr><td><span class="subtitulo">Utilize os filtros abaixo para gerar o relatório</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel ID="up" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdToExcel" />
        </Triggers>
        <ContentTemplate>
            <table width="100%" border="0">
                <tr>
                    <td width="50%" valign="top" align="left">
                        <table cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1" >
                            <tr>
                                <td class="tdPrincipal1">Filial</td>
                                <td class="tdNormal1">
                                    <asp:DropDownList ID="lstFilial" runat="server" SkinID="dropdownSkin" Width="225" />
                                </td>
                             </tr>
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
                             <%--<tr>
                                <td class="tdPrincipal1">Propostas ativas</td>
                                <td class="tdNormal1">
                                    <asp:DropDownList ID="cboAtivas" Width="225" SkinID="dropdownSkin" runat="server" />
                                </td>
                             </tr>
                             <tr>
                                <td class="tdPrincipal1">Taxas de sindical.</td>
                                <td class="tdNormal1"><asp:DropDownList ID="cboTaxas" Width="225" SkinID="dropdownSkin" runat="server" /></td>
                             </tr>--%>
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
                                        <%--</tr>
                                        <tr><td height="8"></td></tr>
                                        <tr>--%>
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
                                <td class="tdPrincipal1" valign="top">Vencimento</td>
                                <td class="tdNormal1" valign="top">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td>De</td>
                                            <td>
                                                <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDeVencto" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                <asp:Image SkinID="imgCanlendario" ID="imgVencDe" runat="server" EnableViewState="false" />
                                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDeVencto" TargetControlID="txtDeVencto" PopupButtonID="imgVencDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                            </td>
                                            <td>Até</td>
                                            <td>
                                                <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtAteVencto" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                <asp:Image SkinID="imgCanlendario" ID="imgVencAte" runat="server" EnableViewState="false" />
                                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAteVencto" TargetControlID="txtAteVencto" PopupButtonID="imgVencAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                             </tr>
                             <tr>
                                <td class="tdPrincipal1" valign="top">Pagamento</td>
                                <td class="tdNormal1" valign="top">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td>De</td>
                                            <td>
                                                <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDePagto" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                <asp:Image SkinID="imgCanlendario" ID="imgPagDe" runat="server" EnableViewState="false" />
                                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDePagto" TargetControlID="txtDePagto" PopupButtonID="imgPagDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                            </td>
                                            <td>Até</td>
                                            <td>
                                                <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtAtePagto" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                <asp:Image SkinID="imgCanlendario" ID="imgPagAte" runat="server" EnableViewState="false" />
                                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAtePagto" TargetControlID="txtAtePagto" PopupButtonID="imgPagAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                             </tr>
                             <%--<tr>
                                <td colspan="2" class="tdNormal1"><asp:CheckBox ID="chkSomentePagas" Text="Somente com a primeira parcela bancária paga" SkinID="checkboxSkin" EnableTheming="false" runat="server" /></td>
                             </tr>--%>
                             <tr>
                                <td class="tdNormal1" colspan="2" align="right"><asp:Button ID="cmdGerar" SkinID="botaoAzul" Text="Gerar" runat="server" Width="80" onclick="cmdGerar_Click" />&nbsp;<font color='red'><asp:Literal ID="lblGerarArquivoMessage" runat="server" EnableViewState="false" /></font></td>
                             </tr>
                        </table>
                    </td>
                    <td width="50%" valign="top" align="left">
                        <table cellpadding="2" cellspacing="0" style="border: solid 1px #507CD1" >
                            <tr>
                                <td class="tdPrincipal1" width="60">Campos</td>
                                <td class="tdNormal1"><asp:DropDownList ID="cboCampos" runat="server" SkinID="dropdownSkin" Width="100%" /></td>
                            </tr>
                            <tr>
                                <td class="tdNormal1" colspan="2" align="center">
                                    <asp:Button ID="cmdAddCampo" Text="Adicionar campo" SkinID="botaoAzulBorda" runat="server" OnClick="cmdAddCampo_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:GridView ID="gridCampos" SkinID="gridViewSkin" runat="server" 
                                        AllowPaging="true" AutoGenerateColumns="False"  PageSize="10" Width="100%"
                                        OnPageIndexChanging="gridCampos_PageIndexChanging" DataKeyNames="campo" 
                                        OnRowCommand="gridCampos_RowCommand">
                                        <Columns>
                                            <asp:BoundField DataField="descricao" HeaderText="Campo">
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' title='remover' alt='remover' border='0' />">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle Width="1%" ForeColor="#CC0000" />
                                            </asp:ButtonField>
                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <asp:Panel runat="server" ID="pnlResultado" Visible="false">
                <br />
                <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" />
                <asp:GridView ID="grid" SkinID="gridViewSkin" runat="server" 
                    AllowPaging="true" AutoGenerateColumns="True"  PageSize="100"
                    onpageindexchanging="grid_PageIndexChanging" >
                </asp:GridView>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
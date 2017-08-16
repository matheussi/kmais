<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="cobrancaNaoGeradas.aspx.cs" Inherits="www.financeiro.cobrancaNaoGeradas" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <script src="../js/common.js" type="text/javascript"></script> 
    <table>
        <tr><td><span class="titulo">Cobranças não geradas</span></td></tr>
        <tr><td><span class="subtitulo">Controle de cobranças <b>não geradas</b> no período</span></td></tr>
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
            <td class="tdPrincipal1" valign="top">Vencimento</td>
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
            <%--<td class="tdNormal1" align="left">
                <asp:CheckBox ID="chkAgendar" Visible="false" EnableViewState="false" Text="agendar" SkinID="checkboxSkin" runat="server" AutoPostBack="true" oncheckedchanged="chkAgendar_CheckedChanged" />
            </td>--%>
            <td colspan="2" class="tdNormal1" align="right"><asp:Button ID="cmdGerar" OnClick="cmdGerar_Click" SkinID="botaoAzul" Text="Gerar" runat="server" Width="80" />&nbsp;<font color='red'><asp:Literal ID="lblGerarArquivoMessage" runat="server" EnableViewState="false" /></font></td>
         </tr>
    </table>
    <asp:Panel runat="server" ID="pnl" Visible="false">
        <br />
        <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" style="width: 16px" />
        <div>
        <asp:GridView ID="grid" DataKeyNames="contrato_id" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="false" AutoGenerateColumns="False" OnRowCommand="grid_RowCommand" >
            <Columns>
                <asp:BoundField DataField="operadora_nome" HeaderText="Operadora">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contratoadm_descricao" HeaderText="ContratoADM">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="plano_descricao" HeaderText="Plano">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contrato_numero" HeaderText="Contrato">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="beneficiario_nome" HeaderText="Beneficiário">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:ButtonField Text="Falhas" CommandName="detalhe">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ControlStyle ForeColor="Red" />
                </asp:ButtonField>
            </Columns>
        </asp:GridView>
        </div>
    </asp:Panel>

    <asp:UpdatePanel ID="upFalhaDetalhe" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:ModalPopupExtender ID="mpeFalhaDetalhe" runat="server" PopupControlID="pnlFalhaDetalhe" CancelControlID="cmdFecharFalhaDetalhe" TargetControlID="target">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlFalhaDetalhe" EnableViewState="false" runat="server">
                <asp:LinkButton runat="server" EnableViewState="false" ID="target" />
                <table width="500" cellpadding="0" cellspacing="4" style="border:solid 4px gray;background-color:white">
                    <tr>
                        <td align="center" height='30' valign="middle" style="background-color:Gray;color:White">
                            Resumo das falhas ao tentar gerar cobrança
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Literal ID="litDetalheFalha" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr height='10'><td height='10' style="border-top:solid 1px Gray"></td></tr>
                    <tr>
                        <td align="center">
                            <asp:Button Text="Fechar" SkinID="botaoPequeno" ID="cmdFecharFalhaDetalhe" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

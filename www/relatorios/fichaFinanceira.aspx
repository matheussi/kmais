<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="fichaFinanceira.aspx.cs" Inherits="www.reports.fichaFinanceira" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo">Ficha Financeira</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Utilize os filtros abaixo para gerar a ficha financeira</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1" >
         <tr>
            <td class="tdPrincipal1" width="120">Operadora</td>
            <td class="tdNormal1"><asp:DropDownList ID="lstOperadora" SkinID="dropdownSkin" Width="225"  runat="server" /></td>
         </tr>
         <tr>
            <td class="tdPrincipal1">Nº Proposta</td>
            <td class="tdNormal1">
                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtNumProposta" Width="110" MaxLength="40" />
            </td>
         </tr>
         <tr>
            <td class="tdPrincipal1">Cód. cobrança</td>
            <td class="tdNormal1">
                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtIDCobranca" Width="110" MaxLength="60" />
            </td>
         </tr>
<%--         <tr>
            <td class="tdPrincipal1">Protocolo Atend.</td>
            <td class="tdNormal1">
                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtProtocolo" Width="110" MaxLength="30" />
            </td>
         </tr>--%>
         <tr>
            <td class="tdPrincipal1">Nome do beneficiário</td>
            <td class="tdNormal1">
                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtBeneificarioNome" Width="250" MaxLength="40" />
            </td>
         </tr>
         <%--<tr>
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
         </tr>--%>
         
         <tr>
            <td class="tdNormal1" colspan="2" align="right"><asp:Button ID="cmdGerar" OnClick="cmdGerar_Click" SkinID="botaoAzul" Text="Gerar" runat="server" Width="80" />&nbsp;<font color='red'><asp:Literal ID="lblGerarArquivoMessage" runat="server" EnableViewState="false" /></font></td>
         </tr>
    </table>
    <asp:Panel runat="server" ID="pnl" Visible="false">
        <br />
        <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" style="width: 16px" />
        <div>
        <asp:GridView ID="grid" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="false" AutoGenerateColumns="False" >
            <Columns>
                <asp:BoundField DataField="CompeVencto" HeaderText="Competência">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Filial" HeaderText="Filial">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Operadora" HeaderText="Operadora">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Estipulante" HeaderText="Estipulante">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="NumeroContrato" HeaderText="Contrato">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Seq" HeaderText="Seq.">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="NomeBeneficiario" HeaderText="NomeBeneficiario">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="CPFbeneficiario" HeaderText="CPFBeneficiario" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="FaixaEtaria" HeaderText="Faixa Etária" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="IDADE" HeaderText="Idade">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="DataNascimento" HeaderText="Nascimento">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Parentesco" HeaderText="Parentesco">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="DataVigência" HeaderText="Vigência">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="DataCancelInativo" HeaderText="DataCancelInativo">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="DataVencto" HeaderText="Vencto.">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="DataPagto" HeaderText="Pagto.">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

                <asp:BoundField DataField="Valor_Boleto" HeaderText="Valor_Boleto" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Valor_Pago" HeaderText="Valor_Pago" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Tipo_Boleto" HeaderText="Tipo" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="TIPO" HeaderText="Taxa" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>


                <asp:BoundField DataField="Valor_Benef" HeaderText="Valor_Benef" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
        </div>
    </asp:Panel>
</asp:Content>
<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="inadimplentes.aspx.cs" Inherits="www.financeiro.inadimplentes" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo">Relatório de inadimplentes</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Utilize os filtros abaixo para gerar o relatório de inadimplentes</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1" >
        <%--<tr>
            <td class="tdPrincipal1">Tipo de relatório</td>
            <td class="tdNormal1">
                <asp:DropDownList ID="cboTipo" runat="server" SkinID="cboTipo" Width="225">
                    <asp:ListItem Selected="True"  Text="Ranking"   Value="0" />
                    <asp:ListItem Selected="false" Text="Sintético" Value="1" />
                </asp:DropDownList>
            </td>
         </tr>
         <tr>
            <td class="tdPrincipal1">Tipo de contrato</td>
            <td class="tdNormal1">
                <asp:ListBox ID="lstTipoContrato" Rows="4" SelectionMode="Multiple" runat="server" SkinID="listBoxSkin" Width="225" />
            </td>
         </tr>--%>
         <tr>
            <td class="tdPrincipal1">Cobranças</td>
            <td class="tdNormal1">
                <asp:DropDownList ID="cboTipoCobrancas" SkinID="dropdownSkin" runat="server" Width="225">
                    <asp:ListItem Selected="True" Text="Todas as cobranças" Value="-1" />
                    <asp:ListItem Text="Apenas as cobranças normais" Value="0" />
                    <asp:ListItem Text="Apenas as cobranças complementares" Value="1" />
                    <asp:ListItem Text="Apenas as cobranças duplas" Value="2" />
                </asp:DropDownList>
            </td>
         </tr>
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
            <td class="tdPrincipal1" width="120">Operadora</td>
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
            <td class="tdNormal1" colspan="2" align="right"><asp:Button ID="cmdGerar" OnClick="cmdGerar_Click" SkinID="botaoAzul" Text="Gerar" runat="server" Width="80" />&nbsp;<font color='red'><asp:Literal ID="lblGerarArquivoMessage" runat="server" EnableViewState="false" /></font></td>
         </tr>
    </table>
    <asp:Panel runat="server" ID="pnl" Visible="false">
        <br />
        <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" 
            ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" 
            runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" style="width: 16px" />
        <asp:GridView ID="grid" Width="100%" SkinID="gridViewSkin" 
            runat="server" AllowPaging="false" AutoGenerateColumns="False" >
            <Columns>
                <asp:BoundField DataField="operadora_nome" HeaderText="Operadora">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contrato_numero" HeaderText="Número">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

                <asp:BoundField DataField="contratoadm_contratoSaude" HeaderText="Contr. Saúde">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="contratoadm_contratoDental" HeaderStyle-Wrap="false" HeaderText="Contr. Dental">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

                <asp:BoundField DataField="beneficiario_nome" HeaderText="Titular">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>

                <asp:BoundField DataField="contratobeneficiario_numMatriculaSaude" HeaderText="Matr. Saúde">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="contratobeneficiario_numMatriculaDental" HeaderText="Matr. Dental">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

                <asp:BoundField DataField="cobranca_parcela" HeaderText="Parcela">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_valor" HeaderText="Valor" DataFormatString="{0:N2}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_dataVencimento" HeaderText="Vencimento" DataFormatString="{0:dd/MM/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField ItemStyle-Width="10%" ItemStyle-ForeColor="Red" DataField="cobranca_dataVencimentoAdiantado" HeaderText="Vencto. Adiant." DataFormatString="{0:dd/MM/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField ItemStyle-Width="9%" ItemStyle-ForeColor="Red" DataField="cobranca_dataPagtoAdiantado" HeaderText="Pagto. Adiant." DataFormatString="{0:dd/MM/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>

                <asp:BoundField DataField="Vidas" HeaderText="Vidas" ItemStyle-Width="1%">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contrato_codcobranca" HeaderText="Cod. Cobrança" ItemStyle-Width="1%" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="vendas.aspx.cs" Inherits="www.reports.vendas" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo">Relatório de vendas</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Utilize os filtros abaixo para gerar o relatório de vendas</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1" >
         <tr>
            <td class="tdPrincipal1">Cobranças</td>
            <td class="tdNormal1">
                <asp:DropDownList ID="cboTipoCobrancas" SkinID="dropdownSkin" runat="server" Width="225">
                    <asp:ListItem Text="Venda aceita" Value="1" Selected="True" />
                    <asp:ListItem Text="Venda implantada" Value="2" />
                </asp:DropDownList>
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
            <td class="tdPrincipal1" valign="top">Vigência</td>
            <td class="tdNormal1" valign="top">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>De</td>
                        <td>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtVigDe" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                            <asp:Image SkinID="imgCanlendario" ID="imgVigDe" runat="server" EnableViewState="false" />
                            <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtVigDe" PopupButtonID="imgVigDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        </td>
                    </tr>
                    <tr><td height="8"></td></tr>
                    <tr>
                        <td>Até</td>
                        <td>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtVigAte" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                            <asp:Image SkinID="imgCanlendario" ID="imgVigAte" runat="server" EnableViewState="false" />
                            <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAte" TargetControlID="txtVigAte" PopupButtonID="imgVigAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
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
        <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" style="width: 16px" />
        <asp:GridView ID="grid" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="false" AutoGenerateColumns="False" >
            <Columns>
                <asp:BoundField DataField="Filial" HeaderText="Filial">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="operadora_nome" HeaderText="Operadora">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>

                <asp:BoundField DataField="FilialContrato" HeaderText="FilialContrato">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>

                <asp:BoundField DataField="estipulante_descricao" HeaderText="Estipulante">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

                <asp:BoundField DataField="contrato_vigencia" HeaderText="Vigência">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_dataVencimento" HeaderText="Vencto">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

                <asp:BoundField DataField="cobranca_dataPagto" HeaderText="Data Pgto" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>

                <asp:BoundField DataField="contrato_datacancelamento" HeaderText="Data Cancel" HeaderStyle-Wrap="false" DataFormatString="{0:dd/MM/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>

                <asp:BoundField DataField="contrato_corretorTerceiroNome" HeaderText="Corretor">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="contrato_corretorTerceiroCPF" HeaderText="CPF Corretor" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                
                <asp:BoundField DataField="contrato_superiorTerceiroNome" HeaderText="Supervisor">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contrato_superiorTerceiroCPF" HeaderText="Supervisor CPF" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="usuario_nome" HeaderText="Produtor">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                
                <asp:BoundField DataField="usuario_codigo" HeaderText="Código">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="usuario_documento1" HeaderText="Produtor CNPJ" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="beneficiario_nome" HeaderText="Beneficiário">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="beneficiario_cpf" HeaderText="Beneficiário CPF" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contrato_numero" HeaderText="Contrato">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_valor" HeaderText="Valor">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_valorPagto" HeaderText="Pgto.">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Parcela" HeaderText="Parcela">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="qtdvidas" HeaderText="Vidas">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                
                <asp:BoundField DataField="aditivos" HeaderText="Aditivos">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                
                <asp:BoundField DataField="operadorNome" HeaderText="Digitador">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contrato_data" HeaderText="Data">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
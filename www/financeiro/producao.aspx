<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="producao.aspx.cs" Inherits="www.financeiro.producao" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo">Relatório de produção</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Relatórios de produção</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1" >
        <tr>
            <td class="tdPrincipal1">Tipo de relatório</td>
            <td class="tdNormal1">
                <asp:DropDownList ID="cboTipo" runat="server" SkinID="dropdownSkin" Width="225">
                    <asp:ListItem Selected="True"  Text="Ranking"   Value="0" />
                    <asp:ListItem Selected="false" Text="Sintético" Value="1" />
                </asp:DropDownList>
            </td>
         </tr>
         <tr>
            <td class="tdPrincipal1">Status do contrato</td>
            <td class="tdNormal1">
                <asp:DropDownList ID="cboStatus" runat="server" SkinID="dropdownSkin" Width="225">
                    <asp:ListItem Text="Todos" Value="-1" Selected="True" />
                    <asp:ListItem Text="Apenas ativos" Value="0" />
                    <asp:ListItem Text="Apenas inativos" Value="1" />
                </asp:DropDownList>
            </td>
         </tr>
         <tr>
            <td class="tdPrincipal1">Tipo de contrato</td>
            <td class="tdNormal1">
                <asp:ListBox ID="lstTipoContrato" Rows="4" SelectionMode="Multiple" runat="server" SkinID="listBoxSkin" Width="225" />
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
            <td class="tdPrincipal1" valign="top">Vigência</td>
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
            <td class="tdPrincipal1" valign="top">Vigência (dia)</td>
            <td class="tdNormal1" valign="top">
                <cc1:MaskedEditExtender MaskType="None" EnableViewState="false" TargetControlID="txtDeDia"  Mask="99" runat="server" ID="meeDeDia" />
                <cc1:MaskedEditExtender MaskType="None" EnableViewState="false" TargetControlID="txtAteDia" Mask="99" runat="server" ID="meeAteDia" />
                De <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDeDia" Width="25" EnableViewState="false" /> Até <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtAteDia" Width="25" EnableViewState="false" />
            </td>
         </tr>
         <tr>
            <td class="tdNormal1" colspan="2" align="right"><asp:Button ID="cmdGerar" OnClick="cmdGerar_Click" SkinID="botaoAzul" Text="Gerar" runat="server" Width="80" />&nbsp;<font color='red'><asp:Literal ID="lblGerarArquivoMessage" runat="server" EnableViewState="false" /></font></td>
         </tr>
    </table>
    <asp:Panel runat="server" ID="pnlRanking" Visible="false">
        <br />
        <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdRankingToExcel_Click" />
        <asp:GridView ID="gridRanking" Width="750px" SkinID="gridViewSkin" 
            runat="server" AllowPaging="false" AutoGenerateColumns="False" >
            <Columns>
                <asp:BoundField DataField="usuario_nome" HeaderText="Produtor">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="usuario_documento1" HeaderText="Documento">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Contratos" HeaderText="Contratos">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="Vidas" HeaderText="Vidas">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
        <asp:Literal ID="litTotalizadorRanking" EnableViewState="false" runat="server" />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlSintetico" Visible="false">
        <br />
        <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="ImageButton1" OnClick="cmdSinteticoToExcel_Click" />
        <asp:GridView ID="gridSintetico" Width="790px" SkinID="gridViewSkin" 
            runat="server" AllowPaging="false" AutoGenerateColumns="False" >
            <Columns>
                <asp:BoundField DataField="operadora_nome" HeaderText="Operadora">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="estipulante_descricao" HeaderText="Estipulante">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Contratos" HeaderText="Contratos">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Vidas" HeaderText="Vidas">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
        <asp:Literal ID="litTotalizadorSintetico" EnableViewState="false" runat="server" />
    </asp:Panel>
</asp:Content>
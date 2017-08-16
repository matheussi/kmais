<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="painel.aspx.cs" Inherits="www.financeiro.painel" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="400">
        <tr><td nowap><span class="titulo">Painel de conferência</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table cellpadding="2">

    </table>
    <table cellpadding="2">
        <tr>
            <td class="tdPrincipal1">Informe o arquivo</td>
            <td colspan="4" class="tdNormal1_NonBold">
                <asp:FileUpload Width="87%" ID="upl" runat="server" />
                <asp:Button ID="cmdEnviar" Text="enviar" SkinID="botaoPequeno" OnClick="cmdEnviar_Click" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="tdPrincipal1">Arquivo enviado</td>
            <td colspan="4" class="tdNormal1_NonBold"><asp:Label ID="lblArquivoEnviado" runat="server" Text="nenhum" /></td>
        </tr>
        <tr>
            <td class="tdPrincipal1" width="100">Competência</td>
            <td class="tdNormal1_NonBold" width="70">
                <asp:TextBox runat="server" ID="txtCompetencia" SkinID="textboxSkin" Width="50" MaxLength="7" />
                <cc1:MaskedEditExtender Mask="99/9999" MessageValidatorTip="true" runat="server" ID="meeCompetencia" EnableViewState="false" TargetControlID="txtCompetencia" ClearMaskOnLostFocus="false" />
            </td>
            <td class="tdPrincipal1" width="100">Corte (atraso)</td>
            <td class="tdNormal1_NonBold">
                Vencto. 10: 
                <asp:TextBox runat="server" ID="txtDataCorte10" SkinID="textboxSkin" Width="30" />
                <cc1:MaskedEditExtender Mask="99/99" MessageValidatorTip="true" runat="server" ID="meeDataCorte10" EnableViewState="false" TargetControlID="txtDataCorte10" ClearMaskOnLostFocus="false" /><%--<cc1:MaskedEditExtender Mask="99/99/9999" MaskType="Date" MessageValidatorTip="true" UserDateFormat="DayMonthYear" runat="server" ID="meeDataCorte" EnableViewState="false" TargetControlID="txtDataCorte" />--%>
                &nbsp;
                Vencto. 20: 
                <asp:TextBox runat="server" ID="txtDataCorte20" SkinID="textboxSkin" Width="30" />
                <cc1:MaskedEditExtender Mask="99/99" MessageValidatorTip="true" runat="server" ID="meeDataCorte20" EnableViewState="false" TargetControlID="txtDataCorte20" ClearMaskOnLostFocus="false" />
            </td>
            <td class="tdNormal1_NonBold"><asp:Button ID="cmdEmitir" Text="emitir" SkinID="botaoAzulBorda" runat="server" onclick="cmdEmitir_Click" /></td>
        </tr>
    </table>
    <br />
    <asp:GridView ID="grid1" Width="810px" SkinID="gridViewSkin" PageSize="12" runat="server" AllowPaging="false" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="operadora_nome" HeaderText="Operadora">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="estipulante_descricao" HeaderText="Estipulante" Visible="false">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="total_boletado" HeaderText="Boletado">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="em_aberto" HeaderText="Em aberto" HeaderStyle-Wrap="false">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="venda_nova" HeaderText="Venda nova" HeaderStyle-Wrap="false">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="total_cancelado" HeaderText="Cancelado" HeaderStyle-Wrap="false">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="antecipado" HeaderText="Antecipado" HeaderStyle-Wrap="false">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="atrasado" HeaderText="Em atraso" HeaderStyle-Wrap="false">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="fatura" HeaderText="Fatura" HeaderStyle-Wrap="false">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="faturaCopart" HeaderText="Fatura COPART" HeaderStyle-Wrap="false">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
</asp:Content>

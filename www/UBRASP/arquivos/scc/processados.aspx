<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="processados.aspx.cs" Inherits="www.UBRASP.arquivos.scc.processados" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="574px">
        <tr><td rowspan="3" width="40px"></td></tr>
        <tr><td><span class="titulo">PSCC - Descontos processados</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">

    <script src="../../../js/common.js" type="text/javascript"></script>

    <asp:UpdatePanel ID="up" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table style="border: 1px solid #507CD1" cellpadding="8" cellspacing="0" width="100%">
                <tr>
                    <td class="tdNormal1" width="100px">Órgão</td>
                    <td class="tdNormal1" colspan="2"><asp:DropDownList SkinID="dropdownSkin" Width="99%" runat="server" ID="cboContratoADM" AutoPostBack="true" OnSelectedIndexChanged="cboContratoAdm_SelectChange" /></td>
                </tr>
                <tr>
                    <td class="tdNormal1">Arquivos</td>
                    <td class="tdNormal1" colspan="2"><asp:DropDownList SkinID="dropdownSkin" Width="99%" runat="server" ID="cboArquivos" AutoPostBack="true" OnSelectedIndexChanged="cboArquivos_SelectChange" /></td>
                </tr>
            </table>
            <asp:Panel ID="pnlResultado" runat="server" Visible="false">
                <br />

                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td width="110px"><strong>Filtro por códido</strong></td>
                        <td><asp:DropDownList SkinID="dropdownSkin" Width="100%" runat="server" ID="cboFiltro" AutoPostBack="true" OnSelectedIndexChanged="cboFiltro_SelectChange" /></td>
                    </tr>
                    <tr><td colspan="2">&nbsp;</td></tr>
                    <tr>
                        <td colspan="2">
                            <asp:GridView ID="grid" DataKeyNames="ID,BeneficiarioId,HeaderID" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="true" PageSize="1000" OnRowCreated="grid_RowCreated" OnPageIndexChanging="grid_OnPageChanging" OnRowDataBound="grid_RowDataBound" OnRowCommand="grid_RowCommand" PagerSettings-Position="TopAndBottom" AutoGenerateColumns="False" >
                                <Columns>
                                    <asp:BoundField DataField="BeneficiarioNome" HeaderText="Beneficiário">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BeneficiarioMatricula" HeaderText="Matr.Funcional">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ParcelaNumero" HeaderText="Parcela">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ParcelaMes" HeaderText="Mês/Ano">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ParcelaValor" HeaderText="Valor">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ParcelaValorENTE" HeaderText="Descontado">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CodigoRetornoConciliacao" HeaderText="Ocorrência">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkBoleto" Text="gerar boleto para" runat="server" CommandName="boletoAction" CommandArgument='<%# Container.DataItemIndex %>' />
                                            <br />
                                            <asp:TextBox ID="txtBoletoVencimento" runat="server" MaxLength="10" Width="70px" SkinID="textboxSkin" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>


</asp:Content>

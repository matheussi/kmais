<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="listagemLista.aspx.cs" Inherits="www.financeiro.comissionamentoLista" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td><span class="titulo">Quadro de comissão</span></td></tr>
        <tr><td nowrap="nowrap"><span class="subtitulo">Efetue ou visualize o pagamento de comissão aos produtores</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <script src="../jQuery/jquery-1.3.2.js" type="text/javascript"></script>
    <script src="../jQuery/plugins/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <%--<script language="javascript" type="text/javascript">
        function pageLoad() {
            $(document).ready(function() {
                $("#ctl00_cphContent_txtProdutor").autocomplete("../searchUsuarioMethod.aspx", {
                    width: 300,
                    scroll: false,
                    dataType: "json",
                    parse: function(data) {
                        var parsed = [];
                        data = data.Produtores;

                        for (var i = 0; i < data.length; i++) {
                            parsed[parsed.length] = {
                                data: data[i],
                                value: data[i].ID,
                                result: data[i].Nome + " (" + data[i].Doc + ")"
                            };
                        }

                        return parsed;
                    },
                    formatItem: function(item) {
                        return item.Nome + " (" + item.Doc + ")";
                    }
                }).result(function(e, item) {
                    document.getElementById('ctl00_cphContent_txtProdutorID').value = item.ID;
                }
                );
            });
        }
    </script>--%>
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="3" cellspacing="1" style="border: solid 1px #507CD1" width="440">
                <tr runat="server" visible="false" enableviewstate="false">
                    <td class="tdPrincipal1">Filial</td>
                    <td class="tdNormal1"><asp:DropDownList ID="lstFilial" runat="server" SkinID="dropdownSkin" Width="225" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="150">Data de corte</td>
                    <td class="tdNormal1"><asp:TextBox ID="txtDataCorte" Width="57" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Período de processamento</td>
                    <td class="tdNormal1">
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td>De:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                <td>
                                    <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtPeriodoDe" Width="65" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    <asp:Image SkinID="imgCanlendario" ID="imgDe" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtPeriodoDe" PopupButtonID="imgDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                </td>
                                <td>Até:&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                <td>
                                    <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtPeriodoAte" Width="65" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    <asp:Image SkinID="imgCanlendario" ID="imgAte" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAte" TargetControlID="txtPeriodoAte" PopupButtonID="imgAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="tdNormal1" colspan="2" align="center" style="border-top: solid 1px #507CD1">
                        <asp:Button ID="cmdExibir" Width="100" Text="Exibir" SkinID="botaoAzulBorda" runat="server" OnClick="cmdExibir_Click" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button runat="server" Width="100" Text="Nova Listagem" SkinID="botaoAzulBorda"  ID="cmdNovaListagem" OnClientClick="location.assign('listagem.aspx');" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:Panel runat="server" ID="pnlNaoEncontrado" Visible="false">Não encontrado</asp:Panel>
            <asp:GridView runat="server" ID="gdvListagem" SkinID="gridViewSkin" AllowPaging="false"
                OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" DataKeyNames="ID"
                AutoGenerateColumns="False" visible="true" Width="600">
                <Columns>
                    <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ProcessarEm" HeaderText="Processar em" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DataCorte" HeaderText="Data de Corte" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Competencia" HeaderText="Competência">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="strProcessadoData" HeaderText="Processado" >
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <br />
            <table cellpadding="0" cellspacing="0" width="600">
                <tr>
                    <td align="right"></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
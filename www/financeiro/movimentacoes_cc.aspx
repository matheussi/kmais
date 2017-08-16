<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="movimentacoes_cc.aspx.cs" Inherits="www.financeiro.movimentacoes_cc" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td><span class="titulo">Movimentações de conta corrente</span></td></tr>
        <tr><td nowrap="nowrap"><span class="subtitulo">Informe o produtor e o período para exibir as movimentações de sua conta corrente</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <script src="../jQuery/jquery-1.3.2.js" type="text/javascript"></script>
    <script src="../jQuery/plugins/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
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
    </script>
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1" width="380" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1" width="70">Produtor</td>
                    <td class="tdNormal1">
                        <asp:TextBox Width="247" ID="txtProdutor" SkinID="textboxSkin" runat="server" />
                        <input type="hidden" name="txtCorretorID" id="txtProdutorID" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="70">Período</td>
                    <td class="tdNormal1">
                        De&nbsp;
                        <asp:TextBox runat="server" SkinID="textboxSkin" EnableViewState="true" ID="txtDe" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        <asp:Image SkinID="imgCanlendario" ID="imgDe" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtDe" PopupButtonID="imgDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        &nbsp;
                        Até&nbsp;
                        <asp:TextBox runat="server" SkinID="textboxSkin" EnableViewState="true" ID="txtAte" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        <asp:Image SkinID="imgCanlendario" ID="imgAte" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAte" TargetControlID="txtAte" PopupButtonID="imgAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        &nbsp;&nbsp;
                        <asp:Button ID="cmdExibir" Text="Exibir" runat="server" SkinID="botaoAzulBorda" OnClick="cmdBuscar_OnClick" />
                    </td>
                </tr>
            </table>
            <br />
            <font color='black'><asp:Literal ID="litMsg" runat="server" EnableViewState="false" /></font>
            <asp:GridView ID="grid" Width="314" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="ID,ProdutorID,CategoriaID,CobrancaID,CategoriaTipo"
                OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" OnPageIndexChanging="grid_PageIndexChanging" PageSize="50">
                <Columns>
                    <asp:BoundField DataField="CategoriaDescricao" HeaderText="Descrição">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="CategoriaStrTipo" HeaderText="Tipo">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
            <br />
            <table cellpadding="2" cellspacing="1" width="382">
                <tr>
                    <td align="right">
                        <asp:Button ID="cmdNova" Text="Nova movimentação" SkinID="botaoAzul" OnClick="cmdNova_Click" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
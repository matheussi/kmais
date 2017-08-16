<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="listagem.aspx.cs" Inherits="www.financeiro.comissionamento" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td><span class="titulo">Agendar fechamento de comissão</span></td></tr>
        <tr><td nowrap="nowrap"><span class="subtitulo">Preencha os campos abaixo e clique em "Agendar listagem" para criar uma pendência de processamento de comissionamento</span></td></tr>
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
            <table cellpadding="3" cellspacing="1" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1">Descrição</td>
                    <td class="tdNormal1"><asp:TextBox ID="txtNome" MaxLength="250" runat="server" SkinID="textboxSkin" Width="222" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Filial</td>
                    <td class="tdNormal1"><asp:DropDownList ID="lstFilial" runat="server" SkinID="dropdownSkin" Width="225" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="150">Operadora</td>
                    <td class="tdNormal1"><asp:ListBox ID="lstOperadora" runat="server" SkinID="listBoxSkin" SelectionMode="Multiple" Width="225" Height="130"  AutoPostBack="false" /></td>
                </tr>
                <%--<tr>
                    <td class="tdPrincipal1" width="150">Perfil</td>
                    <td class="tdNormal1"><asp:ListBox ID="lstPerfil" runat="server" SkinID="listBoxSkin" SelectionMode="Multiple" Width="225"  AutoPostBack="false" /></td>
                </tr>--%>
                <tr>
                    <td class="tdPrincipal1" width="150">Data de corte</td>
                    <td class="tdNormal1"><asp:TextBox ID="txtDataCorte" Width="57" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="150">Competência</td>
                    <td class="tdNormal1"><asp:TextBox ID="txtCompetenciaP1" Width="17" SkinID="textboxSkin" MaxLength="2" runat="server" /> / <asp:TextBox ID="txtCompetenciaP2" Width="27" SkinID="textboxSkin" MaxLength="4" runat="server" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="150">Processar em</td>
                    <td class="tdNormal1">
                        <asp:TextBox ID="txtProcessarEm" Width="57" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        <asp:DropDownList runat="server" ID="cboHora" Width="40" SkinID="dropdownSkin" />&nbsp;:&nbsp;<asp:DropDownList runat="server" ID="cboMinuto" Width="40" SkinID="dropdownSkin" />
                   </td>
                </tr>
                <%--<tr>
                    <td class="tdPrincipal1" width="150">Produtor</td>
                    <td class="tdNormal1">
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtProdutor" Width="220" />
                        <input type="hidden" name="txtCorretorID" id="txtProdutorID" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Mensagem</td>
                    <td class="tdNormal1"><asp:TextBox ID="txtMensagem" Height="40" TextMode="MultiLine" MaxLength="150" runat="server" SkinID="textboxSkin" Width="222" /></td>
                </tr>
                <tr>--%>
                    <td class="tdNormal1" colspan="2" align="center" style="border-top: solid 1px #507CD1">
                        <asp:Button ID="cmdVoltar" Width="140" Text="Voltar" SkinID="botaoAzulBorda" runat="server" OnClick="cmdVoltar_Click" />
                        &nbsp;
                       <asp:Button ID="cmdExibir" Width="140" Text="Agendar listagem" SkinID="botaoAzulBorda" runat="server" OnClick="cmdFecharListagem_Click" OnClientClick="return confirm('Atenção!\nDeseja realmente agendar a listagem?');" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:DataList Visible="false" CellPadding="0" CellSpacing="0" ID="dl" DataKeyField="CobrancaID" runat="server" OnItemCommand="dl_ItemCommand" OnItemDataBound="dl_ItemDataBound">
                <HeaderTemplate></HeaderTemplate>
                <ItemTemplate>
                    <br />
                    <table cellpadding="3" cellspacing="0" width="395">
                        <tr>
                            <td bgcolor='#EFF3FB' style="border-left:solid 1px #507CD1;border-top:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="left">
                                <b>Valor do pagamento</b>
                            </td>
                            <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">
                                <asp:Label ID="lblValor" Text='<%# Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "CobrancaValorPago")).ToString("C") %>' runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td style="border-left: solid 1px #507CD1"><b>Produtor</b></td>
                            <td style="border-right: solid 1px #507CD1"><b>Comissão</b></td>
                        </tr>
                        <tr>
                            <td style="border-left: solid 1px #507CD1;border-bottom: solid 1px #507CD1"><asp:Label ID="lblProdutorNome" Text='<%# DataBinder.Eval(Container.DataItem, "ProdutorNome") %>' runat="server" /></td>
                            <td style="border-right: solid 1px #507CD1;border-bottom: solid 1px #507CD1"><asp:Label ID="Label1" Text='<%# Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "ProdutorValor")).ToString("C") %>' runat="server" /></td>
                        </tr>
                        <tr runat="server" id="tr3Ficha" visible="false">
                            <td colspan="2" style="border-left:solid 1px #507CD1;border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                <asp:Button ID="cmdSalvarFicha" SkinID="botaoPequeno" Text="salvar" runat="server" CommandName="salvar" CommandArgument="<%# Container.ItemIndex %>" /><asp:Literal runat="server" EnableViewState="false" ID="litFichaAviso" />
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
                <FooterTemplate></FooterTemplate>
            </asp:DataList>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
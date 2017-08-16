<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="AlmoxEntrada.aspx.cs" Inherits="www.admin.AlmoxEntrada" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img src="../images/imgTitulos/produtos_entrada.png" /></td></tr>
        <tr><td><span class="titulo">Almoxarifado - Entrada de Produtos</span></td></tr>
        <tr><td><span class="subtitulo">Preencha os campos abaixo e clique em salvar para informar uma nova entrada de produtos no estoque</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <script src="../jQuery/jquery-1.3.2.js" type="text/javascript"></script>
    <script src="../jQuery/plugins/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function pageLoad() {
            $(document).ready(function() {
                $("#ctl00_cphContent_txtCorretorSearch").autocomplete("../searchUsuarioMethod.aspx", {
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
                    document.getElementById('ctl00_cphContent_txtCorretorIDSearch').value = item.ID;
                }
                );
            });
        }
    </script>

    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
    <ContentTemplate>
    <table width="430" cellpadding="2" cellspacing="0" style="border:solid 1px #507CD1">
        <tr>
            <td width="85" class="tdPrincipal1"><b>Tipo</b></td>
            <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" Width="270px" ID="cboTipo" runat="server" AutoPostBack="True" onselectedindexchanged="cboTipo_SelectedIndexChanged" /></td>
        </tr>
        <tr>
            <td width="85" class="tdPrincipal1"><b>Operadora</b></td>
            <td class="tdNormal1"><asp:DropDownList ID="cboOperadoras" Width="270px" SkinID="dropdownSkin" runat="server" /></td><%--AutoPostBack="true" OnSelectedIndexChanged="cboOperadoras_OnSelectedIndexChanged"--%>
        </tr>
        <tr>
            <td width="85" class="tdPrincipal1"><b>Movimentação</b></td>
            <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" Width="270px" ID="cboMovimentacao" AutoPostBack="true" OnSelectedIndexChanged="cboMovimentacao_OnSelectedIndexChanged" runat="server" /></td>
        </tr>
        <tr runat="server" id="trAgente" visible="false">
            <td class="tdPrincipal1">Usuário</td>
            <td class="tdNormal1">
                <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtCorretorSearch" Width="270px" />
                <input type="hidden" name="txtCorretorIDSearch" id="txtCorretorIDSearch" runat="server" />
            </td>
        </tr>
       <%-- <tr>
            <td class="tdPrincipal1"><b>Descrição</b></td>
            <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" Width="270px" ID="cboDescricao" runat="server" AutoPostBack="false" /></td>
        </tr>--%>
        <tr>
            <td class="tdPrincipal1"><b>Quantidade</b></td>
            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1">
                <asp:TextBox ID="txtQtd" Width="39" MaxLength="10" SkinID="textboxSkin" runat="server" AutoPostBack="true" OnTextChanged="Qtd_TextChanged" />
                <cc1:MaskedEditExtender EnableViewState="false" Mask="999999" runat="server" ID="meeQtd" TargetControlID="txtQtd" />
            </td>
        </tr>
        <tr><td colspan="2" height="8"></td></tr>
        <asp:Panel runat="server" ID="pnlNumeracao">
        <tr>
            <td class="tdPrincipal1" colspan="2"><b>Numeração</b></td>
        </tr>
        <tr>
            <td colspan="2" class="tdNormal1">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td width="85"><b>De</b></td>
                        <td width="68">
                            <asp:TextBox ID="txtDe" Width="49" MaxLength="20" SkinID="textboxSkin" runat="server" AutoPostBack="true" OnTextChanged="Qtd_TextChanged" />
                        </td>
                        <td width="43"><b>até</b></td>
                        <td>
                            <asp:TextBox ID="txtAte" Width="49" MaxLength="20" SkinID="textboxSkin" runat="server" ReadOnly="true" BackColor="lightgray" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        </asp:Panel>
    </table>
    <br />
    <table cellpadding="2" width="430">
        <tr>
            <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
            <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
        </tr>
    </table>
    </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel runat="server" ID="upnlAlerta" UpdateMode="Conditional" EnableViewState="false">
        <ContentTemplate>
            <cc1:ModalPopupExtender ID="MPE" runat="server" EnableViewState="false"
                TargetControlID="lnk"
                PopupControlID="pnlAlert"
                BackgroundCssClass="modalBackground" 
                CancelControlID="cmdCloseAlert"
                DropShadow="true"  />
            <asp:Panel runat="server" ID="pnlAlert" EnableViewState="false">
                <asp:LinkButton runat="server" EnableViewState="false" ID="lnk" />
                <table width="350" align="center" bgcolor="gainsboro" style="border:solid 1px black">
                    <tr>
                        <td align="center">
                            <asp:Literal runat="server" ID="litAlert" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr height="8"><td height="8">&nbsp</td></tr>
                    <tr>
                        <td align="center">
                            <input runat="server" style="width:45px;font-size:12px;font-family:Arial" id="cmdCloseAlert" type="button" value="OK" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
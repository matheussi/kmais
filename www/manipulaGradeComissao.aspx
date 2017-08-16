<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="manipulaGradeComissao.aspx.cs" Inherits="www.manipulaGradeComissao" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td width="65" rowspan="3"></td></tr>
        <tr><td><span class="titulo">Manipulação de grade de comissão</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Utilize os filtros abaixo</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">

    <script src="jQuery/jquery-1.3.2.js" type="text/javascript"></script>
    <script src="jQuery/plugins/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function pageLoad() {
            $(document).ready(function() {
                $("#ctl00_cphContent_txtSuperior").autocomplete("searchUsuarioMethod.aspx", {
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
                    document.getElementById('ctl00_cphContent_txtSuperiorID').value = item.ID;
                }
                );
            });
        }
    </script>
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1">
                <tr>
                    <td class="tdPrincipal1" width="150">Perfil</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboPerfil" runat="server" SkinID="dropdownSkin" Width="225"  AutoPostBack="true" OnSelectedIndexChanged="cboPerfil_OnSelectedIndexChanged" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Filial</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboFilial" runat="server" SkinID="dropdownSkin" Width="225" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Grupo de vendas</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboGrupoVendas" runat="server" SkinID="dropdownSkin" Width="225" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Do superior</td>
                    <td class="tdNormal1">
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtSuperior" Width="220" />
                        <input type="hidden" name="txtCorretorID" id="txtSuperiorID" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Ganhando sobre a tabela</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboTabelaComissaoAtual" runat="server" SkinID="dropdownSkin" Width="225" /></td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <asp:Button ID="cmdExibir" Text="Exibir" EnableViewState="false" SkinID="botaoAzul" runat="server" OnClick="cmdExibir_OnClick" />
                    </td>
                </tr>
            </table>
            <br />
            <table bgcolor='#D1DDF1' style="border-left: solid 1px #507CD1;border-right: solid 1px #507CD1;border-top: solid 1px #507CD1" cellpadding="2" cellspacing="2" width="710" id="tblResultado" runat="server" visible="false">
                <tr>
                    <td><font color='#507CD1' style='font-size:12px'><b>Resultado</b></font></td>
                </tr>
            </table>
            <asp:GridView BorderWidth="0" cellpadding="2" cellspacing="1" ID="grid" 
                Width="710" SkinID="gridViewSkin" runat="server" AllowPaging="True" 
                AutoGenerateColumns="False" DataKeyNames="UsuarioID"
                OnRowDataBound="grid_RowDataBound" OnRowCommand="grid_RowCommand" 
                onpageindexchanging="grid_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="UsuarioNome" HeaderText="Produtor">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="UsuarioApelido" HeaderText="Apelido">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ComissaoDescricao" HeaderText="Tabela atual">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ComissaoVigencia" HeaderText="Vigência" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' alt='excluir' border='0' />">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    </Columns>
            </asp:GridView>
            <br />
            <table style="border: solid 1px #507CD1" cellpadding="2" cellspacing="1" width="384" id="tblAtribuir" runat="server" visible="false">
                <tr>
                    <td class="tdPrincipal1" width="150">Nova tabela</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboTabelaComissaoNova" runat="server" SkinID="dropdownSkin" Width="225" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">A partir de</td>
                    <td class="tdNormal1">
                        <asp:TextBox EnableViewState="false" runat="server" SkinID="textboxSkin" ID="txtVigenciaNova" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <asp:Button ID="cmdAtribuir" Text="Atribuir" EnableViewState="false" SkinID="botaoAzul" runat="server" OnClientClick="javascript:return confirm('Deseja realmente prosseguir?');" OnClick="cmdAtribuir_OnClick" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
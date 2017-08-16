<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="manipulaEquipe.aspx.cs" Inherits="www.manipulaEquipe" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="contentTitle" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo">Manipulação de Equipes</span></td></tr>
        <tr><td><span class="subtitulo">Utilize os filtros abaixo</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="contentMain" ContentPlaceHolderID="cphContent" runat="server">
    <script src="jQuery/jquery-1.3.2.js" type="text/javascript"></script>
    <script src="jQuery/plugins/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
       function pageLoad()
       {
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
            
            $(<asp:Literal ID="litAutoCompSuperiorPara" EnableViewState="true" runat="server"></asp:Literal>).autocomplete("searchUsuarioMethod.aspx", {
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
                document.getElementById('<asp:Literal ID="litCorretorParaID" EnableViewState="true" runat="server"></asp:Literal>').value = item.ID;
            });
        });
      }
    </script>
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1">
                <tr>
                    <td class="tdPrincipal1">Do superior</td>
                    <td class="tdNormal1">
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtSuperior" Width="220" />
                        <input type="hidden" name="txtCorretorID" id="txtSuperiorID" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Para superior</td>
                    <td class="tdNormal1">
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtSuperiorPara" Width="220" />
                        <input type="hidden" name="txtCorretorParaID" id="txtCorretorParaID" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="150">Perfil</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboPerfil" runat="server" SkinID="dropdownSkin" Width="225"  AutoPostBack="true" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="150">Vigência</td>
                    <td class="tdNormal1">
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                            <tr>
                                <td align="left">
                                    <asp:TextBox EnableViewState="false" runat="server" SkinID="textboxSkin" ID="txtVigenciaNova" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                </td>
                                <td align="right">
                                    <asp:Button Width="60" ID="cmdExibir" Text="Exibir" EnableViewState="false" SkinID="botaoAzul" runat="server" OnClick="cmdExibir_OnClick" />
                                    <asp:Button Width="60" ID="cmdAtribuir" Text="Atribuir" EnableViewState="true" SkinID="botaoAzul" runat="server" OnClientClick="javascript:return confirm('Deseja realmente prosseguir?');" OnClick="cmdAtribuir_OnClick" Visible="false" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <br />
            <asp:GridView BorderWidth="0" cellpadding="2" cellspacing="1" ID="gridEquipe" Width="710" SkinID="gridViewSkin" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="ID"
                OnRowDataBound="gridEquipe_RowDataBound" OnRowCommand="gridEquipe_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Nome" HeaderText="Produtor">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Apelido" HeaderText="Apelido">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="PerfilDescricao" HeaderText="Perfil">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' alt='excluir' border='0' />">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
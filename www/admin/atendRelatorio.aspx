<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="atendRelatorio.aspx.cs" Inherits="www.admin.atendRelatorio" EnableEventValidation="false" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <script src="../jQuery/jquery-1.3.2.js" type="text/javascript"></script>
    <script src="../jQuery/plugins/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function pageLoad() {
            $(document).ready(function() {
                $("#ctl00_cphContent_txtAbertoPor").autocomplete("../searchAtendenteMethod.aspx", {
                    width: 300,
                    scroll: false,
                    dataType: "json",
                    parse: function(data) {
                        var parsed = [];
                        data = data.Atendentes;

                        for (var i = 0; i < data.length; i++) {
                            parsed[parsed.length] = {
                                data: data[i],
                                value: data[i].Atendente,
                                result: data[i].Atendente
                            };
                        }

                        return parsed;
                    },
                    formatItem: function(item) {
                        return item.Atendente;
                    }
                }).result(function(e, item) {
                    //document.getElementById('ctl00_cphContent_txtAbertoPor').value = item.ID;
                }
                );
            });
        }
    </script>
    <table width="70%">
        <tr><td rowspan="3" width="40"></td></tr>
        <tr>
            <td><span class="titulo">Relatório de Atendimentos</span></td>
        </tr>
        <tr>
            <td nowrap><span class="subtitulo">Utilize os filtros para visualizar os atendimento realizados</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdToExcel" />
        </Triggers>
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" width="355">
                <tr>
                    <td class="tdPrincipal1">Categoria</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboCategoria" SkinID="dropdownSkin" Width="100%" runat="server" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Subcategoria</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboSubCategoria" SkinID="dropdownSkin" Width="100%" runat="server" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" valign="top">Operadora</td>
                    <td class="tdNormal1"><asp:ListBox ID="cboOperadora" SelectionMode="Multiple" SkinID="listBoxSkin" Width="100%" runat="server" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" valign="top">Aberto por</td>
                    <td class="tdNormal1"><asp:TextBox ID="txtAbertoPor" Width="98%" runat="server" SkinID="textboxSkin" /> </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Período</td>
                    <td class="tdNormal1">
                        <asp:TextBox runat="server" ID="txtDe" Width="66px" SkinID="textboxSkin" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                        <asp:Image SkinID="imgCanlendario" ID="imgDe" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtDe" PopupButtonID="imgDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                         a 
                        <asp:TextBox runat="server" ID="txtAte" Width="66px" SkinID="textboxSkin" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                        <asp:Image SkinID="imgCanlendario" ID="imgAte" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAte" TargetControlID="txtAte" PopupButtonID="imgAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Status</td>
                    <td class="tdNormal1">
                        <asp:DropDownList ID="cboStatuc" SkinID="dropdownSkin" runat="server">
                            <asp:ListItem Text="Todos" Value="0" Selected="True" />
                            <asp:ListItem Text="Concluídos" Value="1" />
                            <asp:ListItem Text="Em aberto" Value="2" />
                        </asp:DropDownList>
                        &nbsp;
                        <asp:Button ID="cmdLocalizar" Text="Localizar" SkinID="botaoAzulBorda" runat="server" onclick="cmdLocalizar_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:ImageButton Visible="false" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" />
            <asp:GridView ID="grid" Width="100%" SkinID="gridViewSkin" PageSize="100" runat="server" AllowPaging="false" AutoGenerateColumns="False"  
                DataKeyNames="ID" OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" OnPageIndexChanging="grid_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ContratoNumero" HeaderText="Contrato">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ContratoVigencia" HeaderText="Vigência" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    
                    <asp:BoundField DataField="DataCancelamento" HeaderText="Cancelamento" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    
                    <asp:BoundField DataField="PlanoDescricao" HeaderText="Plano">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TitularNome" HeaderText="Titular">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TitularCPF" HeaderText="CPF">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>

                    <asp:BoundField DataField="Saude" HeaderStyle-Wrap="false" HeaderText="Matr. Saúde">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Dental" HeaderStyle-Wrap="false" HeaderText="Matr. Dental">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>

                    <asp:BoundField DataField="AtendimentoTipo" HeaderText="Atendimento">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="IniciadoPor" HeaderText="Aberto por">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DataInicio" HeaderText="Aberto em" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ResolvidoPor" HeaderText="Concluído por">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="strDataFim" HeaderText="Concluído em" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DataPrevisao" HeaderText="Previsão" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <%--<asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image ID="Image1" ImageUrl="~/images/rascunho.png" ToolTip="rascunho" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="1%" />
                    </asp:TemplateField>
                    <asp:ButtonField Text="<img src='images/active.png' title='excluir' alt='excluir' border='0' />" CommandName="inativar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                        <ItemStyle Font-Size="10px" Width="1%" />
                    </asp:ButtonField>--%>
                </Columns>
            </asp:GridView>
            <asp:Literal ID="litAviso" runat="server" EnableViewState="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
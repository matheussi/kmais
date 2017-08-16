<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="arquivoTransacional.aspx.cs" Inherits="www.arquivoTransacional" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr>
            <td rowspan="3" width="54">
                <img src="images/imgTitulos/beneficiarios.png" alt="Beneficiarios" />
            </td>
        </tr>
        <tr>
            <td>
                <span class="titulo">Arquivos Transacionais</span>
            </td>
        </tr>
        <tr>
            <td>
                <span class="subtitulo">Gerencia os arquivos transacionais</span>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cPageContent" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel ID="up" UpdateMode="Always" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnGerarArquivo" />
        </Triggers>
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td>
                        <span class="titulo_int">Criação de Arquivos</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 1px;">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 1px; background-color: #507CD1; width: 100%">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 5px;">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="2" cellspacing="1" width='450' style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdNormal1" colspan="2">
                                    <asp:CheckBox ID="chkExportacao" Text="Arquivo para exportação SEG" SkinID="checkboxSkin"
                                        AutoPostBack="true" OnCheckedChanged="cboOperadoraGerarArquivo_OnSelectedIndexChanged"
                                        runat="server" Visible="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="120">
                                    Operadora
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList ID="cboOperadoraGerarArquivo" AutoPostBack="true" SkinID="dropdownSkin"
                                        Width="285" runat="server" OnSelectedIndexChanged="cboOperadoraGerarArquivo_OnSelectedIndexChanged" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Contratos ADM
                                </td>
                                <td class="tdNormal1">
                                    <asp:ListBox ID="lstContratosAdm" Rows="4" SelectionMode="Multiple" runat="server" SkinID="listBoxSkin" Width="285" />
                                </td>
                            </tr>
                            <tr id="trAdicionais" runat="server" visible="false">
                                <td class="tdPrincipal1">
                                    Adicionais
                                </td>
                                <td class="tdNormal1">
                                    <asp:ListBox ID="lstAdicionais" Rows="4" SelectionMode="Multiple" runat="server"
                                        SkinID="listBoxSkin" Width="285" />
                                    <br />
                                    <asp:CheckBox ID="chkSomenteComAdicional" Text="Só beneficiários com adicional" runat="server"
                                        Checked="true" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Transação
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboTipoTransacao" AutoPostBack="true"
                                        Width="225" OnSelectedIndexChanged="cboTipo_OnSelectedIndexChanged" />
                                </td>
                            </tr>
                            <tr runat="server" id="trLayout" visible="false">
                                <td class="tdPrincipal1">
                                    Layouts de arquivo
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboLayouts" AutoPostBack="false"
                                        Width="225" />
                                </td>
                            </tr>
                            <tr runat="server" id="trVigencia" visible="false">
                                <td class="tdPrincipal1">
                                    Data Vigência
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtVigenciaGerar" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                    <asp:Image SkinID="imgCanlendario" ID="imgVigenciaGerar" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceVigenciaGerar" TargetControlID="txtVigenciaGerar" PopupButtonID="imgVigenciaGerar" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                        
                                </td>
                            </tr>
                            <tr runat="server" id="trPeriodo" visible="false">
                                <td class="tdPrincipal1" valign="top">
                                    Período
                                </td>
                                <td class="tdNormal1" valign="top">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td>
                                                De
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDe" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                                <asp:Image SkinID="imgCanlendario" ID="imgDe" runat="server" EnableViewState="false" />
                                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtDe" PopupButtonID="imgDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                <asp:DropDownList ID="cboDeHora" SkinID="dropdownSkin" Width="42" runat="server" />
                                                :<asp:DropDownList ID="cboDeMinuto" SkinID="dropdownSkin" Width="42" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="8">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Até
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtAte" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                                <asp:Image SkinID="imgCanlendario" ID="imgAte" runat="server" EnableViewState="false" />
                                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAte" TargetControlID="txtAte" PopupButtonID="imgAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                                <asp:DropDownList ID="cboAteHora" SkinID="dropdownSkin" Width="42" runat="server" />
                                                :<asp:DropDownList ID="cboAteMinuto" SkinID="dropdownSkin" Width="42" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdNormal1" colspan="2" align="right">
                                    <asp:Button ID="btnGerarArquivo" SkinID="botaoAzul" Text="Gerar" OnClick="btnGerarArquivo_OnClick"
                                        runat="server" OnClientClick="return confirm('Deseja realmente prosseguir com a geração do arquivo?');"
                                        Width="80" />&nbsp;<font color='red'><asp:Literal ID="lblGerarArquivoMessage" runat="server"
                                            EnableViewState="false" /></font>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 1px;">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 1px; background-color: #507CD1; width: 100%">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 5px;">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="titulo_int">Filtro de Localização</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 1px;">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 1px; background-color: #507CD1; width: 100%">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 5px;">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                            <tr>
                                <td colspan="2" class="tdNormal1">
                                    <asp:CheckBox ID="chkExportacaoListar" Text="Arquivos para exportação" SkinID="checkboxSkin"
                                        OnCheckedChanged="chkExportacaoListar_Checked" AutoPostBack="true" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Operadora
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList ID="cboOperadora" SkinID="dropdownSkin" Width="225" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Transação
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList ID="cboTipo_Carregar" Width="225" SkinID="dropdownSkin" AutoPostBack="true"
                                        OnSelectedIndexChanged="cboTipo_Carregar_OnSelectedIndexChanged" runat="server" />
                                </td>
                            </tr>
                            <tr runat="server" id="trLayout_Carregar" visible="false">
                                <td class="tdPrincipal1">
                                    Layouts de arquivo
                                </td>
                                <td class="tdNormal1">
                                    <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboLayouts_Carregar" AutoPostBack="false"
                                        Width="225" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Data Vigência
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtVigencia" Width="60" Text="" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    <asp:Image SkinID="imgCanlendario" ID="imgVigencia" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceVigencia" TargetControlID="txtVigencia" PopupButtonID="imgVigencia" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="120">
                                    Data inicial
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDataInicial" Width="60" Text="" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                    <asp:Image SkinID="imgCanlendario" ID="imgDataInicial" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataInicial" TargetControlID="txtDataInicial" PopupButtonID="imgDataInicial" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                    <asp:PlaceHolder ID="pnlDeLocalizar" Visible="false" runat="server">
                                        <asp:DropDownList ID="cboDeHoraLocalizar" SkinID="dropdownSkin" Width="42" runat="server" />
                                        :<asp:DropDownList ID="cboDeMinutoLocalizar" SkinID="dropdownSkin" Width="42" runat="server" />
                                    </asp:PlaceHolder>
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">
                                    Data final
                                </td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDataFinal" Width="60" Text="" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                    <asp:Image SkinID="imgCanlendario" ID="imgDataFinal" runat="server" EnableViewState="false" />
                                    <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataFinal" TargetControlID="txtDataFinal" PopupButtonID="imgDataFinal" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                                    <asp:PlaceHolder ID="pnlAteLocalizar" Visible="false" runat="server">
                                        <asp:DropDownList ID="cboAteHoraLocalizar" SkinID="dropdownSkin" Width="42" runat="server" />
                                        :<asp:DropDownList ID="cboAteMinutoLocalizar" SkinID="dropdownSkin" Width="42" runat="server" />
                                    </asp:PlaceHolder>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="right" class="tdNormal1">
                                    <asp:Button ID="btnListarLote" Style="cursor: pointer;" SkinID="botaoAzul" Text="Localizar"
                                        OnClick="btnListarLote_Click" runat="server" Width="80" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 5px;">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 1px; background-color: #507CD1; width: 100%">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 5px;">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="pnlListaArquivos" Visible="false" runat="server">
                            <asp:GridView ID="gridLotes" Width="100%" PageSize="40" SkinID="gridViewSkin" runat="server"
                                AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="ID" OnRowDataBound="gridLotes_RowDataBound"
                                OnRowCreated="gridLotes_RowCreated" OnRowCommand="gridLotes_RowCommand" OnPageIndexChanging="gridLotes_OnPageIndexChanging">
                                <Columns>
                                    <asp:BoundField HeaderText="Operadora">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="Quantidade">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="Numeração">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="Data">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="Movimentação">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:TemplateField ItemStyle-Width="1%">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imbDownloadArquivo" CommandName="down" AlternateText="Baixar"
                                                ToolTip="Baixar" ImageUrl="~/images/download.gif" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                        <asp:Literal ID="litMessage" Visible="false" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="arquivotransacionalAMIL.aspx.cs" Inherits="www.movimentacao.arquivotransacionalAMIL" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr>
            <td rowspan="3" width="54">
                <img src="../images/imgTitulos/beneficiarios.png" alt="" />
            </td>
        </tr>
        <tr>
            <td>
                <span class="titulo">Arquivos Transacionais - Inclusão Amil</span>
            </td>
        </tr>
        <tr>
            <td>
                <span class="subtitulo">Geração de arquivos transacionais de inclusão Amil</span>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel ID="up" UpdateMode="Always" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnGerarArquivo" />
        </Triggers>
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1" width='680' style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1">Proposta</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" SkinID="textboxSkin" ID="txtNumContrato" Width="60" EnableViewState="false" MaxLength="20" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Operadora</td>
                    <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadora" AutoPostBack="true" Width="225" OnSelectedIndexChanged="cboOperadora_OnSelectedIndexChanged" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Contratos ADM</td>
                    <td class="tdNormal1">
                        <asp:ListBox ID="lstContratosAdm" Rows="8" SelectionMode="Multiple" runat="server" SkinID="listBoxSkin" Width="285" />
                    </td>
                </tr>
                <%--
                <tr>
                    <td class="tdPrincipal1">
                        Transação
                    </td>
                    <td class="tdNormal1">
                        <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboTipoTransacao" AutoPostBack="true" Width="225" OnSelectedIndexChanged="cboTipo_OnSelectedIndexChanged" />
                    </td>
                </tr>
                --%>
                <tr >
                    <td class="tdPrincipal1">Data de vigência</td>
                    <td class="tdNormal1">
                        <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtVigenciaGerar" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        &nbsp;
                        <asp:Button ID="btnVisualizar" SkinID="botaoAzulBorda" Text="visualizar" OnClick="btnVisualizar_OnClick" runat="server" Width="80" />
                        <asp:Button ID="btnGerarArquivo" SkinID="botaoAzulBorda" Text="gerar arquivo" OnClick="btnGerarArquivo_OnClick" runat="server" Width="110" />
                    </td>
                </tr>
                <%--<tr runat="server" id="trPeriodo" visible="false">
                    <td class="tdPrincipal1" valign="top">
                        Período
                    </td>
                    <td class="tdNormal1" valign="top">
                        <cc1:MaskedEditExtender MaskType="Date" EnableViewState="false" TargetControlID="txtDe"
                            Mask="99/99/9999" runat="server" ID="meeDe" />
                        <cc1:MaskedEditExtender MaskType="Date" EnableViewState="false" TargetControlID="txtAte"
                            Mask="99/99/9999" runat="server" ID="meeAte" />
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td>
                                    De
                                </td>
                                <td>
                                    <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDe" Width="60" EnableViewState="false" />
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
                                    <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtAte" Width="60" EnableViewState="false" />
                                    <asp:DropDownList ID="cboAteHora" SkinID="dropdownSkin" Width="42" runat="server" />
                                    :<asp:DropDownList ID="cboAteMinuto" SkinID="dropdownSkin" Width="42" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>--%>
                <%--<tr>
                    <td class="tdNormal1" colspan="2" align="right">
                        <asp:Button ID="btnGerarArquivo" SkinID="botaoAzul" Text="Gerar" OnClick="btnGerarArquivo_OnClick"
                            runat="server" OnClientClick="return confirm('Deseja realmente prosseguir com a geração do arquivo?');"
                            Width="80" />&nbsp;<font color='red'><asp:Literal ID="lblGerarArquivoMessage" runat="server"
                                EnableViewState="false" /></font>
                    </td>
                </tr>--%>
            </table>
            <br />
            <asp:GridView ID="grid" Width="680px" SkinID="gridViewSkin" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="contrato_id" onrowcommand="grid_RowCommand" onrowdatabound="grid_RowDataBound" PageSize="250" OnPageIndexChanging="grid_PageIndexChanging">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:CheckBox ID="chk" runat="server" />
                        </ItemTemplate>
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkHr" runat="server" AutoPostBack="true" OnCheckedChanged="chkHr_changed" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="1%" />
                        <HeaderStyle HorizontalAlign="Center" Width="1%" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="contrato_numero" HeaderText="Número">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="contratoadm_descricao" HeaderText="Contrato ADM">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="contrato_vigencia" HeaderText="Vigência" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="beneficiario_nome" HeaderText="Titular">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
            <asp:Literal ID="litAviso" runat="server" EnableViewState="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
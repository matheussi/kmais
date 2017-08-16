<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="arquivoCobrancasLerRetorno.aspx.cs" Inherits="www.financeiro.arquivoCobrancasLerRetorno" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Cobranças</span></td></tr>
        <tr><td><span class="subtitulo">Ler arquivos de retorno</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">

    <asp:UpdatePanel ID="up" runat="server" UpdateMode="Always">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdLer" />
            <asp:PostBackTrigger ControlID="cmdToExcel" />
        </Triggers>
        <ContentTemplate>
            <table width="550px" cellpadding="2" cellspacing="0" style="border: solid 1px #507CD1">
                <tr>
                    <td width="50%" class="tdNormal1" align="center">
                        <asp:RadioButton ID="optEnviar" Checked="true" Text="enviar arquivo" GroupName="a" runat="server" AutoPostBack="true" OnCheckedChanged="opt_CheckedChanged" />
                    </td>
                    <td width="50%" class="tdNormal1" align="center">
                        <asp:RadioButton ID="optProcessados" Text="consultar processados" GroupName="a" runat="server" AutoPostBack="true" OnCheckedChanged="opt_CheckedChanged" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:Panel ID="pnlEnviar" runat="server">
                <table width="550px" cellpadding="2" cellspacing="0" style="border: solid 1px #507CD1">
                    <tr>
                        <td class="tdNormal1" width="50">Banco</td>
                        <td class="tdNormal1" colspan="2">
                            <asp:RadioButton ID="optSantander" Text="Santander" Checked="true" GroupName="ba" runat="server" />
                            <asp:RadioButton ID="optBancoBrasil" Text="Banco do Brasil" EnableViewState="false" Visible="false" Checked="false" GroupName="ba" runat="server" />
                            <asp:RadioButton ID="optUnibanco" Text="Unibanco" EnableViewState="false" Visible="false" GroupName="ba" runat="server" />
                            <asp:RadioButton ID="optItau" Text="Itaú" EnableViewState="false" Visible="false" GroupName="ba" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" width="50">Arquivo</td>
                        <td class="tdNormal1">
                            <asp:FileUpload ID="uplFile" Width="100%" runat="server" />
                        </td>
                        <td class="tdNormal1" width="50">
                            <asp:Button ID="cmdLer" Width="50" runat="server" Text="Ler" SkinID="botaoAzulBorda" OnClick="cmdLer_Click" OnClientClick="return confirm('Atenção:\nConfirma o banco selecionado?');" />
                        </td>
                    </tr>
                </table>
                <br />
            </asp:Panel>



            <asp:Panel ID="pnlDetalhe" runat="server" Visible="false">

                <asp:Panel ID="pnlDetalhe2" runat="server" Visible="false">
                    <table cellpadding="0" cellspacing="0" style="border: solid 1px #507CD1" width="750px">
                        <tr>
                            <td class="tdNormal1">
                                <asp:Literal ID="litMsg" runat="server" EnableViewState="true" />
                                <asp:DropDownList ID="cboStatus" runat="server" SkinID="dropdownSkin" AutoPostBack="true" OnSelectedIndexChanged="cboStatus_Change">
                                    <asp:ListItem Text="Todos" Value="0" Selected="True" />
                                    <asp:ListItem Text="Não localizados" Value="5" />
                                    <asp:ListItem Text="Valores menores" Value="1" />
                                    <asp:ListItem Text="Pagamento em duplicidade" Value="2" />
                                    <asp:ListItem Text="Pagamento de clientes inativos" Value="3" />
                                    <asp:ListItem Text="Pagamentos rejeitados" Value="4" />
                                </asp:DropDownList>
                                &nbsp;
                                <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" />
                            </td>
                            <td class="tdNormal1" align="right">
                                <asp:LinkButton runat="server" OnClick="lnkFechar_Click" ID="lnkFechar" Text="<img src='../images/close.png' title='fechar' alt='fecar' border='0' />" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:GridView ID="grid" Width="880px" SkinID="gridViewSkin" 
                                    runat="server" AllowPaging="true" AutoGenerateColumns="False" DataKeyNames="CobrancaID"
                                    OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" PageSize="1500" 
                                    onpageindexchanging="grid_PageIndexChanging">
                                    <Columns>
                                        <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TitularNome" HeaderText="Titular">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TitularCPF" HeaderText="CPF">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="NossoNumero" HeaderText="Nosso Núm.">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Parcela" HeaderText="Parc">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="DataVencto" HeaderText="Vencimento" DataFormatString="{0:dd/MM/yyyy}">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:N2}">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ValorPgto" HeaderText="Valor Pgto" DataFormatString="{0:N2}">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="PropostaNumero" HeaderText="Contrato">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Status" HeaderText="Status">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:ButtonField Visible="false" Text="<img src='../images/tick.png' title='gerar cobrança' alt='gerar cobrança' border='0' />" CommandName="gerar">
                                            <ItemStyle Width="1%" />
                                        </asp:ButtonField>
                                    </Columns>
                                </asp:GridView>
                                <asp:Literal ID="litTotal" EnableViewState="false" runat="server" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>

                <asp:Panel ID="pnlFiltro" runat="server">
                    <table cellpadding="4" cellspacing="0" style="border: solid 1px #507CD1" width="550px">
                        <tr>
                            <td width="20">De</td>
                            <td width="120">
                                <asp:TextBox ID="txtDe" runat="server" SkinID="textboxSkin" Width="65" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                <asp:Image SkinID="imgCanlendario" ID="imgDe" runat="server" EnableViewState="false" />
                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtDe" PopupButtonID="imgDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                            </td>
                            <td width="20">Até</td>
                            <td width="125">
                                <asp:TextBox ID="txtAte" runat="server" SkinID="textboxSkin" Width="70" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                <asp:Image SkinID="imgCanlendario" ID="imgAte" runat="server" EnableViewState="false" />
                                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAte" TargetControlID="txtAte" PopupButtonID="imgAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                            </td>
                            <td><asp:Button ID="cmdCarregaRetorno" Text="Carregar..." SkinID="botaoAzulBorda" runat="server" OnClick="cmdCarregaRetorno_Click" />    </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:GridView ID="gridOutput" Width="550px" SkinID="gridViewSkin" OnRowCommand="gridOutput_RowCommand"
                    runat="server" AllowPaging="true" AutoGenerateColumns="False" PageSize="15"
                    DataKeyNames="ID" OnPageIndexChanging="gridOutput_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="20%" />
                        </asp:BoundField>
                        <asp:ButtonField Text="<img src='../images/search.png' title='detalhes' alt='detalhes' border='0' />" CommandName="ver">
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
            

            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>  
</asp:Content>
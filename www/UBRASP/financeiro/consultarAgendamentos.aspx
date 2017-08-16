<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="consultarAgendamentos.aspx.cs" Inherits="www.UBRASP.financeiro.consultarAgendamentos" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Cobranças</span></td></tr>
        <tr><td><span class="subtitulo">Consultar arquivos de remessa</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <%--<table width="390px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                <tr>
                    <td width="96" class="tdPrincipal1">Vencto. Inicial</td>
                    <td class="tdNormal1">
                        <asp:TextBox ID="txtVencimentoDe" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Vencto. Final</td>
                    <td class="tdNormal1">
                        <asp:TextBox ID="txtVencimentoAte" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                    </td>
                </tr>

                <tr>
                    <td width="96" class="tdPrincipal1">Vigência Inicial</td>
                    <td class="tdNormal1">
                        <asp:TextBox ID="txtVigDe" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Vigência Final</td>
                    <td class="tdNormal1">
                        <asp:TextBox ID="txtVigAte" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                    </td>
                </tr>

                <tr>
                    <td class="tdPrincipal1">Processado em</td>
                    <td class="tdNormal1">
                        <asp:TextBox ID="txtProcessadoEm" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        &nbsp;&nbsp;
                        <asp:Button ID="cmdLocalizarArquivos" runat="server" OnClick="cmdLocalizarArquivos_Click" SkinID="botaoAzulBorda" Text="Localizar arquivos" Width="125" />
                    </td>
                </tr>
            </table>--%>
            <asp:Panel runat="server" ID="pnl" EnableViewState="true" Visible="false">
            <table width="390px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                <tr>
                    <td width="76" class="tdPrincipal1">Operadora</td>
                    <td class="tdNormal1">
                        <asp:DropDownList ID="cboOperadora" Width="100%" runat="server" SkinID="dropdownSkin" />
                    </td>
                </tr>
                <tr>
                    <td width="76" class="tdPrincipal1">Vencimento</td>
                    <td class="tdNormal1">
                        <b>Mês</b>&nbsp;<asp:DropDownList ID="cboMes" Width="45" SkinID="dropdownSkin" runat="server" />&nbsp;&nbsp;&nbsp;<b>Ano</b>&nbsp;&nbsp;<asp:DropDownList ID="cboAno" Width="55" SkinID="dropdownSkin" runat="server" />&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="cmdLocalizar" Text="Localizar arquivos" Width="125" SkinID="botaoAzulBorda" runat="server" OnClick="cmdLocalizar_Click" />
                    </td>
                </tr>
            </table>
            </asp:Panel>
            <br />
            <asp:Literal ID="litMsg" runat="server" EnableViewState="true" />
                <asp:GridView ID="gridResult" Width="690px" SkinID="gridViewSkin" 
                    runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ID"
                    OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" OnRowCreated="grid_RowCreated">
                    <Columns>
                        <asp:BoundField DataField="ArquivoNomeInstance" HeaderText="Arquivo">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ControlStyle Width="40%" />
                            <ItemStyle  Width="40%" />
                        </asp:BoundField>
                        <asp:BoundField  DataField="STRVigencia" HeaderText="Vigência">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="STRVencimento" HeaderText="Vencimento">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField Visible="false" DataField="ProcessamentoEm" HeaderText="Processado em" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:TemplateField>
                            <ItemStyle Width="1%" />
                            <ItemTemplate>
                                <asp:ImageButton CommandName="baixar" ID="imbDownloadArquivo" AlternateText="Baixar arquivo" ToolTip="Baixar arquivo" ImageUrl="~/images/download.gif" runat="server" />
                            </ItemTemplate>
                         </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            <asp:Panel ID="pnlListaArquivos" runat="server" Visible="false">
                <asp:GridView ID="grid" Width="390px" SkinID="gridViewSkin" 
                    runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ID,OperadoraID"
                    OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" OnRowCreated="grid_RowCreated">
                    <Columns>
                        <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Versao" HeaderText="Versão">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataCriacao" HeaderText="Data envio" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="QtdCobrancasEnviadas" HeaderText="Enviadas">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:ButtonField Text="<img src='../images/detail2.png' title='cobranças em aberto' alt='cobranças em aberto' border='0' />" CommandName="cobrancas" >
                            <ItemStyle Font-Size="10px" Width="1%" />
                        </asp:ButtonField>
                        <asp:ButtonField Text="<img src='../images/cancelEditGrid.png' title='marcar todas como não enviado' alt='marcar todas como não enviado' border='0' />" CommandName="marcarComoNaoEnviadas" >
                            <ItemStyle Font-Size="10px" Width="1%" />
                        </asp:ButtonField>
                         <asp:TemplateField>
                            <ItemStyle Width="1%" />
                            <ItemTemplate>
                                <asp:ImageButton CommandName="baixar" ID="imbDownloadArquivo" AlternateText="Baixar arquivo" ToolTip="Baixar arquivo" ImageUrl="~/images/download.gif" runat="server" />
                            </ItemTemplate>
                         </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Literal ID="litMsgCobrancas" runat="server" EnableViewState="true" />
                <asp:GridView ID="gridCobrancas" Width="600px" SkinID="gridViewSkin" 
                    runat="server" AllowPaging="false" AutoGenerateColumns="False" 
                    DataKeyNames="ID,Cancelada" OnRowCommand="gridCobrancas_RowCommand" 
                    OnRowCreated="gridCobrancas_RowCreated" OnRowDataBound="gridCobrancas_RowDataBound" >
                    <Columns>
                        <asp:BoundField DataField="FilialNome" HeaderText="Filial">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="EstipulanteNome" HeaderText="Estipulante">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ContratoNumero" HeaderText="Contrato Núm.">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Parcela" HeaderText="Parc.">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataVencimento" HeaderText="Vencimento" DataFormatString="{0:dd/MM/yyyy}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="strPago" HeaderText="Pago">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:ButtonField Text="<img src='../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                            <ItemStyle Font-Size="10px" Width="1%" />
                        </asp:ButtonField>
                        <asp:ButtonField Text="<img src='../images/cancelEditGrid.png' title='marcar como não enviado' alt='marcar como não enviado' border='0' />" CommandName="marcarComoNaoEnviada" >
                            <ItemStyle Font-Size="10px" Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Panel ID="pnlDetalhe" runat="server" Visible="false">
                    <table width="235px" cellpadding="3" cellspacing="0" style="border: solid 1px #507CD1">
                        <tr>
                            <td colspan="2" class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1"><font color='#507CD1'>Detalhes da cobrança</font></td>
                        </tr>
                        <tr>
                            <td width="76" class="tdNormal1">Parcela</td>
                            <td class="tdNormal1">
                                <asp:TextBox ID="txtParcela" MaxLength="3" runat="server" SkinID="textboxSkin" Width="30" />
                            </td>
                        </tr>
                        <tr>
                            <td width="76" class="tdNormal1">Vencimento</td>
                            <td class="tdNormal1">
                                <asp:TextBox ID="txtDataVencimento" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                            </td>
                        </tr>
                        <tr>
                            <td width="76" class="tdNormal1">Valor</td>
                            <td class="tdNormal1">
                                <asp:TextBox ID="txtValor" runat="server" SkinID="textboxSkin" Width="60" />
                            </td>
                        </tr>
                        <tr><td colspan="2" align="center" style="border-top: solid 1px #507CD1;border-bottom: solid 1px #507CD1">Pagamento</td></tr>
                        <tr>
                            <td width="76" class="tdNormal1">Data Pgto.</td>
                            <td class="tdNormal1">
                                <asp:TextBox ID="txtDataPgto" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                            </td>
                        </tr>
                        <tr>
                            <td width="76" class="tdNormal1">Valor Pgto.</td>
                            <td class="tdNormal1">
                                <asp:TextBox ID="txtValorPagto" runat="server" SkinID="textboxSkin" Width="60" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center" class="tdNormal1" style="border-top:solid 1px #507CD1">
                                <asp:Button ID="cmdFechar" Text="fechar" SkinID="botaoAzulBorda" OnClick="cmdFechar_Click" runat="server" />
                                &nbsp;
                                <asp:Button ID="cmdSalvar" Text="salvar" SkinID="botaoAzulBorda" OnClick="cmdSalvar_Click" runat="server" OnClientClick="return confirm('Deseja realmente salvar a cobrança?');" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

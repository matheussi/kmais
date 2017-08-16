<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="cobrancaOpManual.aspx.cs" Inherits="www.financeiro.cobrancaOpManual" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <script src="../js/common.js" type="text/javascript"></script> 
    <table>
        <tr><td><span class="titulo">Cobranças</span></td></tr>
        <tr><td><span class="subtitulo">Gerar, editar e consultar cobranças</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always" >
        <ContentTemplate>


            <table width="650px" cellpadding="5" cellspacing="0" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdNormal1" width='33%'><asp:RadioButton ID="optBuscaPorContrato" Text="Buscar por número de contrato" GroupName="a" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="opt_changed" /></td>
                    <td class="tdNormal1" width='33%'><asp:RadioButton ID="optBuscaPorNossoNumero" Text="Buscar pelo 'nosso número'" GroupName="a" runat="server" AutoPostBack="true" OnCheckedChanged="opt_changed" /></td>
                    <td class="tdNormal1"><asp:RadioButton ID="optBuscaPorNome" Text="Buscar pelo nome" GroupName="a" runat="server" AutoPostBack="true" OnCheckedChanged="opt_changed" /></td>
                </tr>
            </table>
            
            <asp:Panel ID="pnlBuscaPorContrato" runat="server">
                <br />
                <table width="650px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                    <tr>
                        <td width="76" class="tdPrincipal1">Nº Contrato</td>
                        <td class="tdNormal1" width="60"><asp:TextBox ID="txtNumContrato" Width="55" runat="server" SkinID="textboxSkin" MaxLength="25" /></td>
                        <td width="76" class="tdPrincipal1">Operadora</td>
                        <td class="tdNormal1">
                            <asp:DropDownList SelectionMode="Multiple" SkinID="dropdownSkin" runat="server" ID="cboOperadora" Width="180" />
                            &nbsp;&nbsp;
                            <asp:Button Width="100" ID="cmdLocalizar" Text="Localizar" SkinID="botaoAzulBorda" runat="server" OnClick="cmdLocalizar_Click" />
                            &nbsp;
                            <asp:Button Width="110" ID="cmdNovaCobranca" Text="Nova cobrança" OnClick="cmdNovaCobranca_Click" SkinID="botaoAzulBorda" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            
            <asp:Panel ID="pnlBuscaPorNossoNumero" runat="server" Visible="false">
                <br />
                <table width="650px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                    <tr>
                        <td class="tdPrincipal1" width='165'>Informe o 'nosso número'</td>
                        <td class="tdNormal1">
                            &nbsp;
                            <asp:TextBox ID="txtNossoNumero" Width="65" runat="server" SkinID="textboxSkin" MaxLength="25" />
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button Width="100" ID="cmdLocalizar2" Text="Localizar" SkinID="botaoAzulBorda" runat="server" OnClick="cmdLocalizar2_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:Panel ID="pnlBuscaPorNome" runat="server" Visible="false">
                <br />
                <table width="650px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                    <tr>
                        <td class="tdPrincipal1" width='210px'>Informe o nome do beneficiário</td>
                        <td class="tdNormal1">
                            &nbsp;
                            <asp:TextBox ID="txtNomeBenefciario" Width="240px" runat="server" SkinID="textboxSkin" MaxLength="350" />
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button Width="100" ID="cmdLocalizarPorNome" Text="Localizar" SkinID="botaoAzulBorda" runat="server" OnClick="cmdLocalizarPorNome_click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:Panel ID="pnlIntermediario_BuscaPorNome" runat="server" Visible="false">
                <br />
                <asp:GridView ID="gridIntermediario" Width="650px" SkinID="gridViewSkin" runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ID"
                    OnRowCommand="gridIntermediario_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="BeneficiarioTitularNome" HeaderStyle-Wrap="false" HeaderText="Nome">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="EstipulanteNome" HeaderText="Estipulante">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OperadoraDescricao" HeaderText="Operadora">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Numero" HeaderText="Contrato Núm.">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:ButtonField Text="<img src='../images/active.png' title='selecionar' alt='selecionar' border='0' />" CommandName="selecionar" >
                            <ItemStyle Font-Size="10px" Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
            
            
            <asp:Literal ID="litMsg" runat="server" EnableViewState="true" />
            <asp:Panel ID="pnlResultado" runat="server" Visible="false">
                <asp:GridView ID="grid" Width="650px" SkinID="gridViewSkin" runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ID,Cancelada"
                    OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" OnRowCreated="grid_RowCreated">
                    <Columns>
                        <asp:BoundField DataField="STRNossoNumero" HeaderStyle-Wrap="false" HeaderText="Nosso Número">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="EstipulanteNome" HeaderText="Estipulante">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ContratoNumero" HeaderText="Contrato Núm.">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Parcela" HeaderText="Parc.">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C}">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataVencimento" HeaderText="Vencimento" DataFormatString="{0:dd/MM/yyyy}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="strEnviado" HeaderText="Enviado">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="strPago" HeaderText="Pago">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:ButtonField Text="<img src='../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                            <ItemStyle Font-Size="10px" Width="1%" />
                        </asp:ButtonField>
                        <asp:TemplateField>
                            <ItemStyle Width="1%" />
                            <ItemTemplate>
                                <asp:LinkButton CommandName="cnab" ID="lnkCnab" AlternateText="CNAB" runat="server" SkinID="botaoAzul" Text="<img src='../images/download.gif' title='CNAB' alt='CNAB' border='0' />" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField Visible="false" Text="<img src='../images/download.gif' title='CNAB' alt='CNAB' border='0' />" CommandName="cnab" >
                            <ItemStyle Font-Size="10px" Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
                <asp:Literal ID="litLink" runat="server" EnableViewState="false" />
                <br />
            </asp:Panel>
            <asp:Panel ID="pnlDetalhe" runat="server" Visible="false">
                <table>
                    <tr>
                        <td runat="server" id="tblLog" visible="false" valign="top" width="395px">
                            <table width="395px" cellpadding="3" cellspacing="0" style="border: solid 1px #507CD1">
                                <tr>
                                    <td colspan="2" class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1"><font color='red'>Motivo da alteração</font></td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1" width="50px">Motivo</td>
                                    <td class="tdNormal1"><asp:DropDownList ID="cboMotivo" runat="server" SkinID="dropdownSkin" Width="100%" /></td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1">Tipo</td>
                                    <td class="tdNormal1">
                                        <asp:DropDownList ID="cboTipoBaixa" runat="server" SkinID="dropdownSkin" Width="100%">
                                            <asp:ListItem Selected="True"  Text="Baixa" Value="0" />
                                            <asp:ListItem Selected="False" Text="Estorno" Value="1" />
                                            <asp:ListItem Selected="False" Text="Alteração" Value="2" />
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1" colspan="2"><asp:CheckBox ID="chkBaixaFinanceira" Text="Baixa financeira" Font-Bold="true" Checked="true" runat="server" EnableViewState="false" SkinID="checkboxSkin" /></td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1" colspan="2"><asp:CheckBox ID="chkBaixaProvisoria" Text="Baixa provisória" Font-Bold="true" Checked="false" runat="server" EnableViewState="false" SkinID="checkboxSkin" /></td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1" valign="top">Obs.</td>
                                    <td class="tdNormal1"><asp:TextBox ID="txtObs" runat="server" SkinID="textboxSkin" TextMode="MultiLine" Width="98%" Height="130" EnableViewState="false" /></td>
                                </tr>
                            </table>
                        </td>
                        <td valign="top" width="245px">
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
                                        <td width="76" class="tdNormal1">Tipo</td>
                                        <td class="tdNormal1">
                                            <asp:DropDownList ID="cboTipo" runat="server" SkinID="dropdownSkin" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="cboTipo_Changed">
                                                <asp:ListItem Text="Normal" Value="0" Selected="True" />
                                                <asp:ListItem Text="Complementar" Value="1" />
                                                <asp:ListItem Text="Diferença UBRASP" Value="5" />
                                                <asp:ListItem Text="Boleto UBRASP" Value="6" />
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trDupla" visible="false">
                                        <td class="tdNormal1">Referente à</td>
                                        <td class="tdNormal1"><asp:DropDownList ID="cboCobrancaReferente" runat="server" SkinID="dropdownSkin" Width="100%" /></td>
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
                                    <tr>
                                        <td colspan="2" class="tdNormal1">
                                            <asp:CheckBox ID="chkCancelada" Text="Cancelada" runat="Server" ForeColor="Red" />
                                            <br />
                                            <asp:CheckBox ID="chkReenvioBanco" Text="Marcar para reenvio CNAB" runat="Server" ForeColor="Red" />
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
                        </td>
                    </tr>
                </table>
            </asp:Panel>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

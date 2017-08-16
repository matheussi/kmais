<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="admproposta.aspx.cs" Inherits="www.admin.admproposta" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo">Administraçao de contrato</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Alteração de número de proposta e status de beneficiários</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table width="750px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                <tr>
                    <td width="76" class="tdPrincipal1">Nº Contrato</td>
                    <td class="tdNormal1" width="60"><asp:TextBox ID="txtNumContrato" Width="55" runat="server" SkinID="textboxSkin" MaxLength="25" /></td>
                    <td width="76" class="tdPrincipal1">Operadora</td>
                    <td class="tdNormal1">
                        <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadora" Width="290" />
                        &nbsp;&nbsp;
                        <asp:Button Width="100" ID="cmdLocalizar" Text="Localizar" SkinID="botaoAzulBorda" runat="server" OnClick="cmdLocalizar_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:Literal ID="litMsg" runat="server" EnableViewState="true" />
            <asp:Panel ID="pnlResultado" runat="server" Visible="false">
                <table runat="server" visible="false" width="750px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                    <tr>
                        <td width="76" class="tdPrincipal1">Nº Contrato</td>
                        <td class="tdNormal1">
                            <asp:TextBox ID="txtNumContratoNovo" Width="55" runat="server" SkinID="textboxSkin" MaxLength="25" />
                            &nbsp;
                            <asp:Button ID="cmdSalvar" Text="alterar" SkinID="botaoPequeno" runat="server" onclick="cmdSalvar_Click" />
                            &nbsp;
                            <asp:Button ID="cmdDetalhe" Text="detalhes" SkinID="botaoPequeno" runat="server" onclick="cmdDetalhes_Click" />
                        </td>
                    </tr>
                </table>
                <br />
                <table width="750px" style="border:solid 1px #507CD1" cellpadding="6" cellspacing="0">
                    <tr><td class="tdNormal1">Beneficiários da proposta</td></tr>
                </table>
                <asp:GridView ID="grid" Width="750px" SkinID="gridViewSkin" 
                    runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ID,ContratoID"
                    OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" 
                    onrowcreated="grid_RowCreated">
                    <Columns>
                        <asp:BoundField DataField="ContratoID" HeaderText="Contrato ID">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="BeneficiarioNome" HeaderText="Beneficiário">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Novo número">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:TextBox ID="txtNumero" SkinID="textboxSkin" Width="100" MaxLength="50" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status" Visible="false">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:DropDownList ID="cboStatus" SkinID="dropdownSkin" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataFormatString="{0:dd/MM/yyyy}" DataField="Vigencia" HeaderText="Vigência">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:ButtonField Text="<img src='../images/save.png' title='salvar' alt='salvar' border='0' />" CommandName="salvar" >
                            <ItemStyle Font-Size="10px" Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
                <br />
            </asp:Panel>
            <asp:Panel ID="pnlDetalhe" runat="server" Visible="false">
                <%--<table width="235px" cellpadding="3" cellspacing="0" style="border: solid 1px #507CD1">
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
                                <asp:ListItem Text="Dupla" Value="2" />
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
                            <asp:TextBox ID="txtDataVencimento" runat="server" SkinID="textboxSkin" Width="60" />
                            <cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeDataVencimento" Mask="99/99/9999" MaskType="Date" ClearMaskOnLostFocus="true" TargetControlID="txtDataVencimento" />
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
                        </td>
                    </tr>
                    <tr><td colspan="2" align="center" style="border-top: solid 1px #507CD1;border-bottom: solid 1px #507CD1">Pagamento</td></tr>
                    <tr>
                        <td width="76" class="tdNormal1">Data Pgto.</td>
                        <td class="tdNormal1">
                            <asp:TextBox ID="txtDataPgto" runat="server" SkinID="textboxSkin" Width="60" />
                            <cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeDataPgto" Mask="99/99/9999" MaskType="Date" ClearMaskOnLostFocus="true" TargetControlID="txtDataPgto" />
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
                </table>--%>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
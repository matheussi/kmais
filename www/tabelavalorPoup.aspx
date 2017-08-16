<%@ Page Theme="Theme" Language="C#" AutoEventWireup="true" CodeBehind="tabelavalorPoup.aspx.cs" Inherits="www.tabelavalorPoup" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>SABE</title>
    <script src="js/common.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="css/style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <cc1:ToolkitScriptManager ID="sm" runat="server" />
        <asp:UpdatePanel runat="server" ID="up">
            <ContentTemplate>
        <asp:Panel runat="server" ID="pnlTopoTValores">
            <asp:Panel ID="pnlTabelaValorLista" runat="server">
                <table style="border:solid 1px #507CD1" cellpadding="4" cellspacing="0" width="90%">
                    <tr>
                        <td class="tdNormal1" width="60">Operadora</td>
                        <td class="tdNormal1"><asp:DropDownList OnSelectedIndexChanged="cboOperadora_OnSelectedIndexChanged" AutoPostBack="true" CssClass="textbox" Width="256px" runat="server" ID="cboOperadora" /></td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" width="60">Contrato</td>
                        <td class="tdNormal1"><asp:DropDownList OnSelectedIndexChanged="cboTValoresContrato_OnSelectedIndexChanged" AutoPostBack="true" CssClass="textbox" Width="256px" runat="server" ID="cboTValoresContrato" /></td>
                    </tr>
                </table>
                <asp:GridView ID="gridTabelaValores" SkinID="gridViewSkin" 
                    OnRowCommand="gridTabelaValores_OnRowCommand" 
                    OnRowDataBound="gridTabelaValores_OnRowDataBound" Font-Size="10px" DataKeyNames="ID,Corrente" Width="90%" runat="server" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField Visible="false" DataField="ID" HeaderText="Código">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Descricao" HeaderText="Tabela">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Corrente" HeaderText="Atual" Visible="false">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Inicio" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Adm.Início">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Fim" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Adm.Fim">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:BoundField DataField="VencimentoInicioStr" HeaderText="Vencto.Início">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="VencimentoFimStr" HeaderText="Vencto.Fim">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:ButtonField Visible="false" CommandName="excluir" Text="<img src='images/delete.png' align='middle' title='excluir' alt='excluir' border='0' />">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                        <asp:ButtonField Visible="false" CommandName="detalhes" Text="<img src='images/detail2.png' align='middle' title='detalhes' alt='detalhes' border='0' />">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                        <asp:ButtonField Visible="false" CommandName="duplicar" Text="<img src='images/duplicar.png' align='middle' title='duplicar' alt='duplicar' border='0' />">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                        <asp:ButtonField CommandName="editar" Text="<img src='images/detail2.png' align='middle' title='detalhes' alt='detalhes' border='0' />">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                        <asp:ButtonField Visible="false" CommandName="recalcular" Text="<img src='images/refresh.png' align='middle' title='recalcular vencimentos' alt='recalcular vencimentos' border='0' />">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
                <br />
            </asp:Panel>
            <asp:Panel ID="pnlTabelaValorDetalhe" runat="server" Visible="false">
                <table width="70%"cellpadding="2" cellspacing="0">
                    <tr>
                        <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo" runat="server" id="span5">Detalhes da tabela de valores</span></td>
                        <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdTabelaValorFechar" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdTabelaValorFechar_Click" /></td>
                    </tr>
                </table>
                <table border="0" style="border-left:solid 1px #507CD1;border-right:solid 1px #507CD1;border-top:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="70%">
                    <tr>
                        <td width="55px" class="tdNormal1">Contrato</td>
                        <td class="tdNormal1" colspan="3"><asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboTValoresContratoDETALHE" AutoPostBack="true" OnSelectedIndexChanged="cboTValoresContratoDETALHE_OnSelectedIndexChanged" Width="234" /> </td>
                    </tr>
                </table>
                <asp:Literal runat="server" EnableViewState="false" ID="litTVSemPlano" />
                <asp:Panel ID="pnlTVPlanos" runat="server" Visible="false">
                    <asp:GridView ID="gridTVPlanos" SkinID="gridViewSkin" width="70%" runat="server" AutoGenerateColumns="False" DataKeyNames="ID" OnRowCommand="gridTVPlanos_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Descricao" HeaderText="Planos">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="strDatasInicio" HeaderText="Início">
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:ButtonField Text="<img src='../images/detail2.png' title='selecionar' alt='selecionar' border='0' />" CommandName="selecionar" >
                                <ItemStyle Width="1%" />
                            </asp:ButtonField>
                        </Columns>
                    </asp:GridView>
                    <br />
                </asp:Panel>
                <asp:Panel ID="pnlTabelaValorITENS" runat="server" Visible="false">
                    <asp:Panel ID="pnlTabelaValorITENS_ListaPlanos" EnableViewState="true" runat="server">
                        <asp:GridView ID="gridTabelaValoresItemDETALHE" SkinID="gridViewSkin" Width="70%" 
                            AutoGenerateColumns="False" DataKeyNames="ID,CalculoAutomatico"  runat="server" Visible="true"
                            onrowdatabound="gridTabelaValoresItemDETALHE_RowDataBound" onrowcommand="gridTabelaValoresItemDETALHE_RowCommand">
                            <Columns>

                                <asp:TemplateField HeaderText="Idade início">
                                    <ItemTemplate>
                                        <asp:TextBox CssClass="textbox" Width="25" ID="txtIdadeInicio" runat="server" Text='<%# Bind("IdadeInicio") %>' />
                                        <cc1:MaskedEditExtender Mask="999" runat="server" ID="meeIdadeInicio" TargetControlID="txtIdadeInicio" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Idade fim">
                                    <ItemTemplate>
                                        <asp:TextBox CssClass="textbox" Width="25" ID="txtIdadeFim" runat="server" Text='<%# Bind("IdadeFim") %>' />
                                        <cc1:MaskedEditExtender Mask="999" runat="server" ID="meeIdadeFim" TargetControlID="txtIdadeFim" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="QC Operadora" Visible="false">
                                    <ItemTemplate>
                                        <asp:TextBox CssClass="textbox" Width="50" ID="txtQCPg" runat="server" Text='<%# Bind("QCValorPagamentoSTR") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="QP Operadora" Visible="false">
                                    <ItemTemplate>
                                        <asp:TextBox CssClass="textbox" Width="50" ID="txtQPPg" runat="server" Text='<%# Bind("QPValorPagamentoSTR") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="QC Cliente">
                                    <ItemTemplate>
                                        <asp:TextBox BackColor="lightgray" ReadOnly="true" CssClass="textbox" Width="50" ID="txtQC" runat="server" Text='<%# Bind("QCValorSTR") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="QP Cliente">
                                    <ItemTemplate>
                                        <asp:TextBox BackColor="lightgray" ReadOnly="true" CssClass="textbox" Width="50" ID="txtQP" runat="server" Text='<%# Bind("QPValorSTR") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:ButtonField Visible="false" Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir">
                                    <ItemStyle Width="1%" HorizontalAlign="Center" />
                                    <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                </asp:ButtonField>
                                <asp:BoundField Visible="false" DataField="TabelaID" />

                                <asp:ButtonField Visible="false" ButtonType="Image" ImageUrl="~/images/cadeado.png" CommandName="calculo">
                                    <ItemStyle Width="1%" HorizontalAlign="Center" />
                                    <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                </asp:ButtonField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                    <br />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel ID="pnlTabelaValorDuplicar" runat="server" Visible="false">
                <table width="74%"cellpadding="2" cellspacing="0">
                    <tr>
                        <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo">Duplicar tabela de valor</span></td>
                        <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdTVDuplicarFechar" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdTVDuplicarFechar_Click" /><asp:Literal ID="litTabelaValorDuplicarIDs" runat="server" EnableViewState="true" Visible="false" /></td>
                    </tr>
                </table>
                <table style="border-left:solid 1px #507CD1;border-right:solid 1px #507CD1;border-top:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="74%">
                    <tr>
                        <td width="55" class="tdNormal1">Início</td>
                        <td width="80" class="tdNormal1">
                            <asp:TextBox EnableViewState="true" CssClass="textbox" Width="67px" runat="server" ID="txtTValoresInicioDUPLICAR" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        </td>
                        <td width="40" class="tdNormal1">Fim</td>
                        <td width="70" class="tdNormal1">
                            <asp:TextBox EnableViewState="true" CssClass="textbox" Width="67px" runat="server" ID="txtTValoresFimDUPLICAR" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        </td>
                        <td width="65" class="tdNormal1">% Reajuste</td>
                        <td class="tdNormal1"><asp:TextBox EnableViewState="false" SkinID="textboxSkin" Width="67px" runat="server" ID="txtReajusteDuplicar" MaxLength="10" /></td>
                    </tr>
                </table>
                <table style="border-left:solid 1px #507CD1;border-right:solid 1px #507CD1;border-bottom:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="74%">
                    <tr>
                        <td width="55" class="tdNormal1">% Over</td>
                        <td width="80" class="tdNormal1"><asp:TextBox SkinID="textboxSkin" Width="67px" runat="server" ID="txtOverPriceDuplicar" MaxLength="10" /></td>
                        <td width="40" class="tdNormal1">R$ Fixo</td>
                        <td width="70" class="tdNormal1"><asp:TextBox SkinID="textboxSkin" Width="67px" runat="server" ID="txtFixoDuplicar" MaxLength="10" /></td>
                        <td width="65" class="tdNormal1">R$ Tarifa</td>
                        <td class="tdNormal1"><asp:TextBox SkinID="textboxSkin" Width="67px" runat="server" ID="txtValorEmbutidoDuplicar" MaxLength="10" />&nbsp;<asp:CheckBox ID="chkValorEmbutidoDuplicar" Text="embutida" runat="server" SkinID="checkboxSkin" /></td>
                    </tr>
                </table>
                <br />
                <table cellpadding="0" cellspacing="0" width="74%">
                    <tr>
                        <td align="right" style="padding-right:2px" height="27" class="tdPrincipal1">
                            <div style="width: 175px; height:20px; border:solid 1px #EFF3FB;">
                                <asp:Image style="cursor: pointer;" ID="Image9" ImageUrl="~/images/save.png" EnableViewState="false" runat="server" />
                                <asp:Button EnableViewState="false" Width="154" runat="server" ID="cmdDuplicarTabelaValor" SkinID="botaoAzul" Text="duplicar tabela de valor" BorderWidth="0px" OnClick="cmdDuplicarTabelaValor_Click" />
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>

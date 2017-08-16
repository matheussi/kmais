<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="cobrancaRecuperacao.aspx.cs" Inherits="www.admin.cobrancaRecuperacao" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img height="50" src="../images/imgTitulos/contratos_65_50.png" /></td></tr>
        <tr><td><span class="titulo">Propostas</span></td></tr>
        <tr><td><span class="subtitulo">Selecione a operadora para exibir seus contratos ou informe o número da proposta</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table width="480px">
        <tr>
            <td width="126"><span class="subtitulo">Operadora</span></td>
            <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadoras" Width="100%" /></td>
        </tr>
    </table>
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table width="480px" border="0" >
                <tr>
                    <td><span class="subtitulo">Nº Proposta</span></td>
                    <td colspan="2">
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtNumProposta" Width="110" MaxLength="40" />
                    </td>
                </tr>
                <tr>
                    <td><span class="subtitulo">Cód. cobrança</span></td>
                    <td colspan="2">
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtIDCobranca" Width="110" MaxLength="60" />
                    </td>
                </tr>
                <tr>
                    <td><span class="subtitulo">Protocolo Atend.</span></td>
                    <td colspan="2">
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtProtocolo" Width="110" MaxLength="30" />
                    </td>
                </tr>
                <tr>
                    <td><span class="subtitulo">Empresa</span></td>
                    <td colspan="2">
                        <asp:DropDownList Width="252" ID="cboEmpresaFiltro" SkinID="dropdownSkin" runat="server" />
                        &nbsp;&nbsp;<asp:Button SkinID="botaoVermelhoBorda" ID="Button1" runat="server" 
                            Text="Limpar" Visible="false" onclick="Button1_Click" OnClientClick="return confirm('Limpar testes?');" />
                    </td>
                </tr>
                <tr>
                    <td><span class="subtitulo">Nome do beneficiário</span></td>
                    <td colspan="2">
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtBeneficiarioNome" Width="250" MaxLength="40" />
                    </td>
                </tr>
                <tr>
                    <td width="126"><span class="subtitulo">CPF do beneficiário</span></td>
                    <td>
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtBeneficiarioCPF" Width="79px" />
                        <%--<cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeCPF" Mask="999,999,999-99" ClearMaskOnLostFocus="true" TargetControlID="txtBeneficiarioCPF" />--%>
                    </td>
                    <td align="right"><asp:Button runat="server" ID="cmdLocalizar" SkinID="botaoAzul" Text="Localizar" Width="80" onclick="cmdLocalizar_Click" /></td>
                </tr>
            </table>
            <br />
            <asp:GridView ID="gridContratos" Width="615px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False"  
                DataKeyNames="ID,Rascunho,Cancelado,Inativo" onrowcommand="gridContratos_RowCommand" 
                onrowdatabound="gridContratos_RowDataBound" PageSize="25"
                OnPageIndexChanging="gridContratos_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="Numero" HeaderText="Número">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="PlanoDescricao" HeaderText="Plano">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="BeneficiarioTitularNome" HeaderText="Titular">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    
                    <asp:BoundField DataField="EmpresaCobranca" HeaderText="Empresa">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    
                    <asp:TemplateField Visible="false">
                        <ItemTemplate>
                            <asp:Image ID="Image1" ImageUrl="~/images/rascunho.png" ToolTip="rascunho" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="1%" />
                    </asp:TemplateField>

                    <asp:ButtonField Visible="false" Text="<img src='../images/active.png' title='excluir' alt='excluir' border='0' />" CommandName="inativar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/edit.png' title='editar contrato' alt='editar contrato' border='0' />" CommandName="editar" >
                        <ItemStyle Font-Size="10px" Width="1%" />
                    </asp:ButtonField>

                    <asp:ButtonField Text="<img src='../images/detail2.png' border='0' alt='detalhes do parcelamento' title='detalhes do parcelamento' />" CommandName="email">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>

                    <asp:ButtonField Text="<img src='../images/search.png' title='exibir pendências' alt='exibir pendências' border='0' />" CommandName="exibirCobrancas" >
                        <ItemStyle Font-Size="10px" Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <asp:Literal ID="litAviso" runat="server" EnableViewState="false" />
            <br />
            <asp:Panel ID="pnlCobrancas" runat="server" Visible="false">
                <table cellpadding="2" cellspacing="1" width="615px" style="border: solid 1px #507CD1">
                    <tr><td><span class="tituloAnuncio">Pagamentos em aberto</span></td></tr>
                </table>
                <asp:GridView ID="gridCobranca" runat="server" SkinID="gridViewSkin" DataKeyNames="ID,HeaderParcID,Tipo"
                    AutoGenerateColumns="False" Width="615px" OnRowDataBound="gridCobranca_RowDataBound"
                    OnRowCommand="gridCobranca_RowCommand" OnPageIndexChanging="gridCobranca_PageIndexChanging"
                    AllowPaging="False" PageSize="15">
                    <Columns>
                        <asp:BoundField DataField="Parcela" HeaderText="Parcela">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataVencimento" HeaderText="Vencimento" DataFormatString="{0:dd/MM/yyyy}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="strEnviado" HeaderText="Enviado">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <%--<asp:ButtonField ButtonType="Image" ImageUrl="~/images/print.png" Text="imprimir"
                            CommandName="print">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>--%>
                        
                        <%--<asp:TemplateField HeaderText="Multa R$">
                            <ItemTemplate>
                                <asp:TextBox ID="txtMulta" Width="80" runat="server" Font-Size="10px" Text='<%# Bind("MultaRS", "{0:C}") %>' SkinID="textboxSkin" MaxLength="10" />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Juros R$">
                            <ItemTemplate>
                                <asp:TextBox ID="txtJuros" Width="80" runat="server" Font-Size="10px" Text='<%# Bind("JurosRS", "{0:C}") %>' SkinID="textboxSkin" MaxLength="10" />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>--%>

                        <asp:TemplateField ItemStyle-Width="1%">
                            <ItemTemplate>
                                <asp:CheckBox ID="chk" ToolTip="selecionar" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--<asp:ButtonField Text="<img src='../images/mail.gif' border='0' alt='enviar e-mail' title='enviar e-mail' />" CommandName="email">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>--%>
                    </Columns>
                    <PagerSettings PageButtonCount="30" />
                </asp:GridView>
                <br />
                <table cellpadding="2" cellspacing="1" width="615px" style="border: solid 1px #507CD1">
                    <tr>
                        <td class="tdPrincipal1" width="70">Vencimento</td>
                        <td class="tdNormal1" colspan='3'>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtVencto" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                            <asp:Image SkinID="imgCanlendario" ID="imgVencto" runat="server" EnableViewState="false" />
                            <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtVencto" PopupButtonID="imgVencto" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        </td>
                    </tr>
                    
                    <tr>
                        <td class="tdPrincipal1">E-mail</td>
                        <td class="tdNormal1" colspan='3'>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtEmail" Width="250" EnableViewState="false" MaxLength="80" />
                        </td>
                    </tr>
                    
                    <tr>
                        <td class="tdPrincipal1" width="70">Desconto</td>
                        <td class="tdNormal1" width="200px">
                            <asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboDesconto" Width="100" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:CheckBox ID="chkIsentarJuros" Text="Isentar juros" SkinID="checkboxSkin" runat="server" />
                        </td>
                        <td class="tdPrincipal1" width="70">Multa</td>
                        <td class="tdNormal1">
                            <asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboMulta" Width="98%" />
                        </td>
                    </tr>
                    
                    <tr>
                        <td class="tdPrincipal1" width="70">Parcelas</td>
                        <td class="tdNormal1" colspan='3'><asp:TextBox ID="txtQtdParcelas" runat="server" Width="30" MaxLength="2" SkinID="textboxSkin" />
                    </tr>
                    <tr>
                        <td class="tdPrincipal1" width="70" valign="top">Observações</td>
                        <td class="tdNormal1" valign="top" colspan="2">
                            <asp:TextBox Width="98%" TextMode="MultiLine" Rows="3" ID="txtObs" SkinID="textboxSkin" runat="server" />
                        </td>
                        <td class="tdNormal1" valign="middle" width="60" align="center">
                            <asp:Button ID="cmdCalcular" Text="Calcular" runat="server" SkinID="botaoAzulBorda" OnClick="cmdCalcular_click" />
                        </td>
                    </tr>
                </table>
                <br />
                <table cellpadding="2" cellspacing="0" width="615px" style="border: solid 1px #507CD1">
                    <tr>
                        <td class="tdPrincipal1">Parcelamento <asp:Literal ID="litTotalParcelamento" runat="server" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="gridParcelamento" runat="server" SkinID="gridViewSkin" 
                                DataKeyNames="Parcela" AutoGenerateColumns="False" Width="100%" 
                                AllowPaging="false" CellPadding="2" CellSpacing="0">
                                <Columns>
                                    <asp:BoundField DataField="Parcela" HeaderText="Parcela" Visible="false">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Valor">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtValor" ReadOnly="true" SkinID="textboxSkin" MaxLength="10" Width="60" runat="server" Text='<%# Bind("Valor", "{0:N2}") %>' />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Vencimento">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtVenctoGrid" Width="60" runat="server" Font-Size="10px" Text='<%# Bind("DataVencimento", "{0:dd/MM/yyyy}") %>'
                                                SkinID="textboxSkin" ReadOnly="true" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Observações">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtObsGrid" ReadOnly="true" Width="200" Rows="4" TextMode="MultiLine" runat="server" Font-Size="10px" Text='<%# Bind("ItemParcelamentoOBS") %>'
                                                SkinID="textboxSkin" MaxLength="1000" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Multa R$" Visible="false">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtMulta" Width="80" runat="server" Font-Size="10px" Text='<%# Bind("MultaRS", "{0:C}") %>' SkinID="textboxSkin" MaxLength="10" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Multa/Juros R$">
                                        <ItemTemplate>
                                            <asp:TextBox ReadOnly="true" ID="txtJuros" Width="80" runat="server" Font-Size="10px" Text='<%# Bind("JurosRS", "{0:C}") %>' SkinID="textboxSkin" MaxLength="10" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Amortização">
                                        <ItemTemplate>
                                            <asp:TextBox ReadOnly="true" ID="txtAmortizacao" Width="80" runat="server" Font-Size="10px" Text='<%# Bind("Amortizacao", "{0:C}") %>' SkinID="textboxSkin" MaxLength="10" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                </Columns>
                                <PagerSettings PageButtonCount="30" />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1" align="center">
                            <asp:Button ID="cmdGravar" Text="Gravar" runat="server" SkinID="botaoAzulBorda" OnClick="cmdGravar_click" OnClientClick="return confirm('Deseja realmente prosseguir com essa operação?');" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlEmails" runat="server" Visible="false">
                <table cellpadding="2" cellspacing="1" width="615px" style="border-Top: solid 1px #507CD1;border-right: solid 1px #507CD1;border-left: solid 1px #507CD1">
                    <tr>
                        <td><span class="tituloAnuncio">Detalhes do parcelamento</span></td>
                        <td align="right">
                            <asp:Button SkinID="botaoVermelhoBorda" ID="cmdCancelarParcelamento" runat="server" 
                            Text="Cancelar parcelamento?" Visible="true" OnClick="cmdCancelarParcelamento_click" OnClientClick="return confirm('Deseja realmente CANCELAR o parcelamento?\nEssa operação não poderá ser desfeita.');" />
                        </td>
                    </tr>
                </table>
                <table cellpadding="2" cellspacing="1" width="615px" style="border: solid 1px #507CD1">
                    <tr>
                        <td class="tdPrincipal1">Vencimento</td>
                        <td class="tdNormal1" colspan='3'>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtVenctoR" ReadOnly="True" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        </td>
                    </tr>
                    
                    <tr>
                        <td class="tdPrincipal1">E-mail</td>
                        <td class="tdNormal1" colspan='3'>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtEmailR" Width="250" MaxLength="80" />
                        </td>
                    </tr>
                    
                    <tr>
                        <td class="tdPrincipal1" width="70">Desconto</td>
                        <td class="tdNormal1" width="300px">
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="cboDescontoR" ReadOnly="true" Width="100" />
                            &nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:CheckBox ID="chkIsentarJurosR" Text="Isentar juros" SkinID="checkboxSkin" runat="server" />
                        </td>
                        <td class="tdPrincipal1">Multa</td>
                        <td class="tdNormal1">
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="cboMultaR" Width="100" ReadOnly="true" />
                        </td>
                    </tr>
                    
                    <tr>
                        <td class="tdPrincipal1">Parcelas</td>
                        <td class="tdNormal1" colspan='3'><asp:TextBox ID="txtQtdParcelasR" runat="server" Width="30" ReadOnly="True" SkinID="textboxSkin" />
                    </tr>
                    <tr>
                        <td class="tdPrincipal1" valign="top">Observações</td>
                        <td class="tdNormal1" valign="top" colspan="3">
                            <asp:TextBox Width="98%" TextMode="MultiLine" Rows="3" ID="txtObsR" ReadOnly="True" SkinID="textboxSkin" runat="server" />
                        </td>
                    </tr>
                </table>
                <asp:GridView ID="gridEmail" runat="server" SkinID="gridViewSkin" DataKeyNames="ID,HeaderParcID,Tipo"
                    AutoGenerateColumns="False" Width="615px" OnRowDataBound="gridEmail_RowDataBound"
                    OnRowCommand="gridEmail_RowCommand" OnPageIndexChanging="gridEmail_PageIndexChanging"
                    AllowPaging="False">
                    <Columns>
                        <asp:BoundField DataField="Parcela" HeaderText="Parcela" Visible="false">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataVencimento" HeaderText="Vencimento" DataFormatString="{0:dd/MM/yyyy}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>

                        <asp:ButtonField Text="<img src='../images/mail.gif' border='0' alt='enviar e-mail' title='enviar e-mail' />" CommandName="email">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                    <PagerSettings PageButtonCount="30" />
                </asp:GridView>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
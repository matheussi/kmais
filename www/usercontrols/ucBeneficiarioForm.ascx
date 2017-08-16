<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ucBeneficiarioForm.ascx.cs" Inherits="www.usercontrols.ucBeneficiarioForm" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<script language="javascript" type="text/javascript">
    function semNumeroConfirm(campoID)
    {
        var valor = document.getElementById(campoID).value;
        if (valor == '')
        {
            return confirm('Deseja cadastrar o endereço sem número?');
        }
    }
</script>
<asp:Literal runat="server" ID="litFechar" />
<asp:Panel ID="pnlEnriquecimento" runat="server" Visible="false">
    <table cellpadding="2" width="600" border="0" style="border: solid 1px #507CD1;background-color:#EFF3FB">
        <tr>
            <td><strong><span style="color:#507CD1;font-size:9pt">Há informações de enriquecimento para confimarção:</span></strong></td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="gridEnriquecimento" runat="server" SkinID="gridViewSkin" 
                    OnRowDataBound="gridEnriquecimento_RowDataBound" OnRowCommand="gridEnriquecimento_RowCommand"
                    DataKeyNames="id_telMail,id_beneficiario" AutoGenerateColumns="False" Width="100%">
                    <Columns>
                        <asp:BoundField ItemStyle-Wrap="false" DataField="beneficiario_nome" HeaderText="Nome" Visible="true">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField ItemStyle-Wrap="false" DataField="tipo" HeaderText="Tipo" Visible="true">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField ItemStyle-Wrap="false" DataField="ddd" HeaderText="DDD" Visible="false">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField ItemStyle-Wrap="false" DataField="telefone" HeaderText="Número" Visible="false">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField ItemStyle-Wrap="false" DataField="ramal" HeaderText="Ramal" Visible="false">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField ItemStyle-Wrap="false" DataField="email" HeaderText="E-mail" Visible="false" >
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        
                        <asp:BoundField ItemStyle-Wrap="false" DataField="dado" HeaderText="Dado" Visible="true">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        
                        <asp:ButtonField Text="<img src='images/delete.png' title='inválido' alt='inválido' border='0' />" CommandName="invalido">
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                        <asp:ButtonField Text="<img src='images/tick.png' title='confirmar' alt='confirmar' border='0' />" CommandName="ok">
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <br />
</asp:Panel>
<cc1:TabContainer BorderStyle="None" BorderWidth="0" Width="100%" ID="tab" runat="server" ActiveTabIndex="0">
    <cc1:TabPanel runat="server" ID="p1">
        <HeaderTemplate><font color="black">Dados comuns</font></HeaderTemplate>
        <ContentTemplate>
            <table cellpadding="2" width="535">
                <tr>
                    <td width="85" class="tdPrincipal1">Nome</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" CssClass="textbox" ID="txtNome" Width="97%" /></td>
                </tr>
                <tr runat="server" id="trMatriculaAssociativa">
                    <td width="85" class="tdPrincipal1">Matr.Assoc.</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" CssClass="textbox" ID="txtMatriculaAssociativa" Width="97%" /></td>
                </tr>
            </table>
            <table cellpadding="2" width="535">
                <tr>
                    <td width="85" class="tdPrincipal1">Data Nasc.</td>
                    <td width="110" class="tdNormal1">
                        <asp:TextBox runat="server" CssClass="textbox" ID="txtDataNascimento" 
                            Width="100px" />
                        <cc1:MaskedEditExtender MaskType="Date" EnableViewState="False" 
                            TargetControlID="txtDataNascimento" Mask="99/99/9999" runat="server" 
                            ID="meeDataNascimento" CultureAMPMPlaceholder="" 
                            CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                            CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                            CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
                    </td>
                    <td width="62" class="tdPrincipal1">RG</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" CssClass="textbox" ID="txtRG" Width="90px" /></td>
                    <td width="42" class="tdPrincipal1">CPF</td>
                    <td class="tdNormal1">
                        <asp:TextBox runat="server" CssClass="textbox" ID="txtCPF" Width="90px" />
                        <cc1:MaskedEditExtender EnableViewState="False" TargetControlID="txtCPF" 
                            Mask="999,999,999-99" runat="server" ID="meeCPF" CultureAMPMPlaceholder="" 
                            CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                            CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                            CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
                    </td>
                </tr>
            </table>
            <table cellpadding="2" width="535">
                <tr>
                    <td class="tdPrincipal1" width="85">
                        RG órgão exp.</td>
                    <td class="tdNormal1" width="110">
                        <asp:TextBox ID="txtRgOrgao" runat="server" CssClass="textbox" Width="97%"></asp:TextBox>
                    </td>
                    <td class="tdPrincipal1" width="62">
                        Rg UF</td>
                    <td class="tdNormal1">
                        <asp:TextBox ID="txtRgUF" runat="server" CssClass="textbox" MaxLength="2" 
                            Width="30px"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <table cellpadding="2" width="535">
                <tr>
                    <td width="85" class="tdPrincipal1">Sexo</td>
                    <td class="tdNormal1">
                        <asp:DropDownList SkinID="dropdownSkin" Width="107px" runat="server" 
                            ID="cboSexo" />
                    </td>
                </tr>
            </table>
            <table cellpadding="2" width="535">
                <tr runat="server" id="trParentesco" visible="False">
                    <td width="85" class="tdPrincipal1" runat="server">Parentesco</td>
                    <td class="tdNormal1" runat="server"><asp:DropDownList SkinID="dropdownSkin" 
                            Width="107px" runat="server" ID="cboParentesco" /></td>
                </tr>
                <tr>
                    <td width="85" class="tdPrincipal1">Nome da mãe</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" CssClass="textbox" ID="txtNomeMae" Width="97%" /></td>
                </tr>
            </table>
            <table cellpadding="2" width="535">
                <tr>
                    <td class="tdPrincipal1" width="85">
                        Dec. nasc. vivo</td>
                    <td class="tdNormal1" width="110">
                        <asp:TextBox ID="txtDeclaracaoNascimentoVivo" runat="server" CssClass="textbox" 
                            Width="97%"></asp:TextBox>
                    </td>
                    <td class="tdPrincipal1" width="62">
                        CNS</td>
                    <td class="tdNormal1">
                        <asp:TextBox ID="txtCNS" runat="server" CssClass="textbox" Width="90px"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <br />
            <b>Dados de contato</b>&nbsp;<asp:ImageButton runat="server" Visible="False" 
                ToolTip="Puxar dados de contato do Titular da propsta" 
                ImageUrl="~/images/duplicar.png" ID="cmdPuxarFonesDoTitular" 
                OnClick="cmdPuxarFonesDoTitular_Click" />
            
            <table>
                <tr>
                    <td valign="top">
                    
            <table cellpadding="2">
                <tr>
                    <td class="tdPrincipal1" width="50">DDD</td>
                    <td class="tdNormal1" width="40">
                        <asp:TextBox CssClass="textbox" runat="server" ID="txtDDD1" Width="30px" MaxLength="3" />
                        <%--<cc1:MaskedEditExtender EnableViewState="False" TargetControlID="txtDDD1" 
                            Mask="99" runat="server" ID="meeDDD1" CultureAMPMPlaceholder="" 
                            CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                            CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                            CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />--%>
                    </td>
                    <td class="tdPrincipal1" width="50">Fone</td>
                    <td class="tdNormal1" width="80">
                        <asp:TextBox CssClass="textbox" runat="server" ID="txtFone1" Width="62px" MaxLength="9" />
                        <%--<cc1:MaskedEditExtender EnableViewState="False" TargetControlID="txtFone1" 
                            Mask="9999-9999" runat="server" ID="meeFone1" CultureAMPMPlaceholder="" 
                            CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                            CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                            CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />--%>
                    </td>
                    <td class="tdPrincipal1" width="70">Ramal</td>
                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtRamal1" 
                            Width="40px" MaxLength="4" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">DDD</td>
                    <td class="tdNormal1" width="40">
                        <asp:TextBox CssClass="textbox" runat="server" ID="txtDDD2" Width="30px" MaxLength="3" />
                        <%--<cc1:MaskedEditExtender EnableViewState="False" TargetControlID="txtDDD2" 
                            Mask="99" runat="server" ID="meeDDD2" CultureAMPMPlaceholder="" 
                            CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                            CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                            CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />--%>
                    </td>
                    <td class="tdPrincipal1">Fone</td>
                    <td class="tdNormal1" width="80">
                        <asp:TextBox CssClass="textbox" runat="server" ID="txtFone2" Width="62px" MaxLength="9" />
                        <%--<cc1:MaskedEditExtender EnableViewState="False" TargetControlID="txtFone2" 
                            Mask="9999-9999" runat="server" ID="meeFone2" CultureAMPMPlaceholder="" 
                            CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                            CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                            CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />--%>
                    </td>
                    <td class="tdPrincipal1">Ramal</td>
                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtRamal2" Width="40px" MaxLength="4" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">DDD</td>
                    <td class="tdNormal1" width="40">
                        <asp:TextBox CssClass="textbox" runat="server" ID="txtDDDCelular" Width="30px" MaxLength="3" />
                        <%--<cc1:MaskedEditExtender EnableViewState="False" TargetControlID="txtDDDCelular" 
                            Mask="99" runat="server" ID="meeDDDCelular" CultureAMPMPlaceholder="" 
                            CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                            CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                            CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />--%>
                    </td>
                    <td class="tdPrincipal1">Celular</td>
                    <td class="tdNormal1">
                        <asp:TextBox CssClass="textbox" runat="server" ID="txtCelular" Width="62px" MaxLength="10" />
                    </td>
                    <td class="tdPrincipal1">Operadora</td>
                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtCelularOperadora" Width="80px" MaxLength="99" /></td>
                </tr>
            </table>
            <table cellpadding="2" width="412">
                <tr>
                    <td class="tdPrincipal1" width="50">E-mail</td>
                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" Width="99%" runat="server" ID="txtEmail" MaxLength="65" /></td>
                </tr>
            </table>
            
                    </td>
                    <td valign="top" align="left">
                        <asp:Panel ID="pnlEnriquecimentoValido" Visible="False" runat="server">
                            <table>
                                <tr>
                                    <asp:DataList CellPadding="0" ID="dlEnriquecimento" DataKeyField="id_telMail"
                                        RepeatDirection="Horizontal" runat="server" 
                                        OnItemCommand="dlEnriquecimento_ItemCommand" RepeatColumns="3">
                                        <ItemTemplate>
                                            <td valign="top" align="left">
                                                <table cellpadding="2" border="0" style="border: solid 1px #507CD1;background-color:#EFF3FB">
                                                    <tr>
                                                        <td>Tipo:</td>
                                                        <td nowap><asp:Literal ID="litTipo" Text='<%# DataBinder.Eval(Container.DataItem, "tipo") %>' runat="server" /></td>
                                                        <td align="right"><asp:ImageButton ID="cmdFechar" ImageUrl="~/images/close.png" ImageAlign="Top" hspacing='0' ToolTip="fechar" OnClientClick="return confirm('Deseja fechar o item?');" runat="server" CommandName="fechar" CommandArgument='<%# Eval("id_telMail") %>' /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Dado:</td>
                                                        <td colspan="2" nowap><asp:Literal ID="Literal1" Text='<%# DataBinder.Eval(Container.DataItem, "dado") %>' runat="server" /></td>
                                                    </tr>
                                                    <%--<tr>
                                                        <td>DDD</td>
                                                        <td colspan="2"><asp:Literal ID="litDDD" Text='<%# DataBinder.Eval(Container.DataItem, "ddd") %>' runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Número</td>
                                                        <td colspan="2"><asp:Literal ID="litNumero" Text='<%# DataBinder.Eval(Container.DataItem, "telefone") %>' runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Ramal</td>
                                                        <td colspan="2"><asp:Literal ID="litRamal" Text='<%# DataBinder.Eval(Container.DataItem, "ramal") %>' runat="server" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>E-mail</td>
                                                        <td colspan="2"><asp:Literal ID="litEmail" Text='<%# DataBinder.Eval(Container.DataItem, "email") %>' runat="server" /></td>
                                                    </tr>--%>
                                                </table>
                                            </td>
                                        </ItemTemplate>
                                    </asp:DataList>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </cc1:TabPanel>
    <cc1:TabPanel runat="server" ID="p2">
        <HeaderTemplate><font color="black">Endereços</font></HeaderTemplate>
        <ContentTemplate>
            <br /><b>Endereço</b><br />
            <table cellpadding="2">
                <tr>
                    <td class="tdPrincipal1" width="59px">CEP</td>
                    <td class="tdNormal1" colspan="3">
                        <asp:TextBox CssClass="textbox" onkeypress="filtro_SoNumeros(event); mascara_CEP(this, event);" runat="server" ID="txtCEP" Width="65px" MaxLength="9" />
                        <asp:ImageButton runat="server" EnableViewState="false" ToolTip="checar CEP" ImageUrl="~/images/endereco.png" ID="cmdBuscaEndereco" OnClick="cmdBuscaEndereco_Click" />&nbsp;
                        <asp:ImageButton runat="server" Visible="false" ToolTip="Puxar endereços do Titular da propsta" ImageUrl="~/images/duplicar.png" ID="cmdPuxarEnderecosDoTitular" OnClick="cmdPuxarEnderecosDoTitular_Click" />
                        <cc1:MaskedEditExtender TargetControlID="txtCEP" Mask="99999-999" 
                            runat="server" ID="meeCEP" ClearMaskOnLostFocus="true" Enabled="false" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Logradouro</td>
                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtLogradouro" Width="290px" MaxLength="300" /></td>
                    <td class="tdPrincipal1">&nbsp;Número</td>
                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtNumero" Width="65px" MaxLength="9" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Complemento</td>
                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtComplemento" Width="200px" MaxLength="250" /></td>
                    <td class="tdPrincipal1" width="72px">&nbsp;Bairro</td>
                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtBairro" Width="190px" MaxLength="300" /></td>
                </tr>
            </table>
            <table cellpadding="2">
                <tr>
                    <td class="tdPrincipal1" width="80px">Cidade</td>
                    <td class="tdNormal1" width="293"><asp:TextBox CssClass="textbox" runat="server" ID="txtCidade" Width="200px" MaxLength="300" /></td>
                    <td class="tdPrincipal1" width="72px">UF</td>
                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtUF" Width="20px" MaxLength="2" /></td>
                </tr>
            </table>
            <table cellpadding="2">
                <tr>
                    <td class="tdPrincipal1" width="80px">Tipo</td>
                    <td class="tdNormal1" width="573">
                        <asp:DropDownList Width="206px" runat="server" ID="cboTipoEndereco" CssClass="textbox">
                            <asp:ListItem Text="RESIDENCIAL" Value="0" Selected="True" />
                            <asp:ListItem Text="COMERCIAL" Value="1" />
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <br />
            <asp:Button runat="server" SkinID="botaoPequeno" EnableViewState="true" Text="Adicionar" ID="cmdAddEndereco" OnClick="cmdAddEndereco_Click" />
            <br /><br />
            <span runat="server" id="spanEnderecosCadastrados"><b>Endereços cadastrados</b></span> <br />
            <asp:GridView Font-Size="10px" Width="76%" ID="gridEnderecos" runat="server" 
                AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" DataKeyNames="ID" 
                BorderWidth="1px" BorderColor="#507CD1" GridLines="None" 
                onrowcommand="gridEnderecos_RowCommand" 
                onrowdatabound="gridEnderecos_RowDataBound">
                <RowStyle BackColor="#EFF3FB" Font-Size="10px" />
                <Columns>
                    <asp:BoundField Visible="false" DataField="ID" HeaderText="Código">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Logradouro" HeaderText="Logradouro">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Tipo" HeaderText="Tipo">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField CommandName="alterar" Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                </Columns>
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="#2461BF" />
                <AlternatingRowStyle  Font-Size="10px" BackColor="White" />
            </asp:GridView>
        </ContentTemplate>
    </cc1:TabPanel>
    <cc1:TabPanel runat="server" ID="p3" Visible="false">
        <HeaderTemplate><asp:Literal ID="litP3" runat="server" Visible="false"><font color="black">Ficha de Saúde</font></asp:Literal></HeaderTemplate>
        <ContentTemplate>
            <asp:DataList CellPadding="0" CellSpacing="0" ID="dlFicha" DataKeyField="ID" runat="server" OnItemCommand="dlFicha_ItemCommand" OnItemDataBound="dlFicha_ItemDataBound">
            <HeaderTemplate>
            </HeaderTemplate>
            <ItemTemplate>
                <br />
                <table cellpadding="3" cellspacing="0" width="600">
                    <tr>
                        <td colspan="2" bgcolor='#EFF3FB' style="border-left:solid 1px #507CD1;border-top:solid 1px #507CD1;border-bottom:solid 1px #507CD1" align="left">
                            <asp:Label ID="lblQuesta" Text='<%# DataBinder.Eval(Container.DataItem, "ItemDeclaracaoTexto") %>' runat="server" />
                            <asp:Literal ID="litItemDeclaracaoID" Text='<%# DataBinder.Eval(Container.DataItem, "ItemDeclaracaoID") %>' runat="server" Visible="false" />
                        </td>
                        <td bgcolor='#EFF3FB' style="border-top:solid 1px #507CD1;border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center" width="1%">
                            <asp:CheckBox OnCheckedChanged="checkboxSkin_Changed2" AutoPostBack="true" SkinID="checkboxSkin" ID="chkFSim" runat="server" Checked='<%# Bind("Sim") %>' />
                        </td>
                    </tr>
                    <tr runat="server" id="tr1Ficha" visible="false">
                        <td style="border-left: solid 1px #507CD1">Data</td>
                        <td width="90%" colspan="2" style="border-right: solid 1px #507CD1">Descrição</td>
                    </tr>
                    <tr runat="server" id="tr2Ficha" visible="false">
                        <td style="border-left: solid 1px #507CD1;border-bottom: solid 1px #507CD1">
                            <asp:TextBox SkinID="textboxSkin" Width="66px" runat="server" ID="txtFichaSaudeData" MaxLength="10" Text='<%# DataBinder.Eval(Container.DataItem, "strData") %>' />
                            <cc1:MaskedEditExtender MaskType="Date" EnableViewState="false" TargetControlID="txtFichaSaudeData" Mask="99/99/9999" runat="server" ID="meeFichaSaudeData" />
                        </td>
                         <td width="90%" colspan="2" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">
                            <asp:TextBox ID="txtFichaSaudeDescricao" Width="99%" SkinID="textboxSkin" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Descricao") %>' />
                        </td>
                    </tr>
                    <tr runat="server" id="tr3Ficha" visible="false">
                        <td colspan="3" style="border-left:solid 1px #507CD1;border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                            <asp:Button ID="cmdSalvarFicha" SkinID="botaoPequeno" Text="salvar" runat="server" CommandName="salvar" CommandArgument="<%# Container.ItemIndex %>" /><asp:Literal runat="server" EnableViewState="false" ID="litFichaAviso" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:DataList>
        </ContentTemplate>
    </cc1:TabPanel>
    <cc1:TabPanel runat="server" ID="p4" Visible="false">
        <HeaderTemplate><asp:Literal ID="litP4" runat="server" Visible="false"><font color="black">Adicionais</font></asp:Literal></HeaderTemplate>
        <ContentTemplate>
            <asp:GridView ID="gridAdicional" runat="server" SkinID="gridViewSkin" 
                DataKeyNames="AdicionalID,ID" AutoGenerateColumns="False"
                width="650px" OnRowDataBound="gridAdicional_OnRowDataBound" 
                onrowcommand="gridAdicional_RowCommand">
                <Columns>
                    <asp:BoundField DataField="AdicionalDescricao" HeaderText="Produto">
                        <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:CheckBox oncheckedchanged="checkboxGridAdicional_Changed" AutoPostBack="true" SkinID="checkboxSkin" ID="chkSimAd" runat="server" Checked='<%# Bind("Sim") %>' />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="1%" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </cc1:TabPanel>
</cc1:TabContainer>
<table cellpadding="2" width="65%">
    <tr id="trUsar" runat="server" visible="false">
        <td colspan="2" align="center" bgcolor="whitesmoke">
            <asp:LinkButton Text="Já existe um beneficiário com este nome cadastrado no sistema. Quero usá-lo!" ID="lnkUsar" runat="server" OnClick="lnkUsar_Click" ForeColor="Red" />
        </td>
    </tr>
    <tr>
        <td align="left"><asp:Button Width="130" OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
        <td align="right"><asp:Button Width="130" OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="true" ID="cmdSalvar" Text="Salvar" /></td>
    </tr>
</table>
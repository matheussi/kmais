<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="comissionamento.aspx.cs" Inherits="www.comissionamento" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"><img src="images/imgTitulos/tabela_comissionario.png" /></td></tr>
        <tr>
            <td><span class="titulo">Tabela de comissionamento</span></td>
        </tr>
        <tr>
            <td nowrap><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <script language="javascript" type="text/javascript">
         /* <![CDATA[ */
		 function SelectAllCheckboxes(spanChk){

		   // Added as ASPX uses SPAN for checkbox
		   var oItem = spanChk.children;
		   var theBox= (spanChk.type=="checkbox") ? 
				spanChk : spanChk.children.item[0];
		   xState=theBox.checked;
		   elm=theBox.form.elements;

		   for(i=0;i<elm.length;i++)
			 if(elm[i].type=="checkbox" && 
					  elm[i].id!=theBox.id)
			 {
			   //elm[i].click();
			   if(elm[i].checked!=xState)
				 elm[i].click();
			   //elm[i].checked=xState;
			 }
		 }
		 /* ]]> */
	</script>
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1" width="445" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1" width="110">Categoria</td>
                    <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" Width="245px" runat="server" ID="cboCategoriaComissionamento" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Idade especial</td>
                    <td class="tdNormal1"><asp:TextBox SkinID="textboxSkin" Width="29px" runat="server" ID="txtIdade" MaxLength="3" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Data</td>
                    <td class="tdNormal1">
                        <asp:TextBox SkinID="textboxSkin" Width="59px" runat="server" ID="txtData" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                    </td>
                </tr>
            </table>
            <br />
            <asp:GridView ID="gridGrupos" SkinID="gridViewSkin" width="445" runat="server" 
                AutoGenerateColumns="False" DataKeyNames="ID" OnRowCommand="gridGrupos_RowCommand" OnRowCreated="gridGrupos_RowCreated">
                <Columns>
                    <asp:BoundField DataField="Descricao" HeaderText="Grupo">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TotalNormal" HeaderText="%" DataFormatString="{0:F}">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir">
                        <ItemStyle Width="1%" HorizontalAlign="Center" />
                        <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/detail2.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/condidade.png' title='parcelas de comissão' alt='parcelas de comissão' border='0' />" CommandName="parcelas" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/new_generic.png' title='contratos' alt='contratos' border='0' />" CommandName="contratos" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <table cellpadding="2" cellspacing="1" width="445" style="border-left: solid 1px #507CD1;border-right: solid 1px #507CD1;border-bottom: solid 1px #507CD1">
                <tr runat="server" id="trAdicionarGrupo">
                    <td class="tdNormal1" align="right">
                        <asp:LinkButton Visible="true" ID="cmdAdicionarGrupo" Text="adicionar grupo" runat="server" OnClick="cmdAdicionarGrupo_Click" />
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlGrupoDetalhe" runat="server" Visible="false">
                <table width="445" cellpadding="3" cellspacing="0" style="border-top: solid 1px #507CD1;border-left: solid 1px #507CD1;border-right: solid 1px #507CD1;">
                    <tr>
                        <td align="left" class="tdPrincipal1"><span style="color:black" class="subtitulo" runat="server" id="lblSuperior" enableviewstate="false"></span></td>
                        <td align="right" class="tdPrincipal1"><asp:ImageButton ID="cmdFecharGrupoDetalhe" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdFecharGrupoDetalhe_Click"  /></td>
                    </tr>
                </table>
                <table cellpadding="2" cellspacing="0" width="445" style="border-left: solid 1px #507CD1;border-right: solid 1px #507CD1;border-bottom: solid 1px #507CD1">
                    <tr>
                        <td class="tdNormal1" width="50">Grupo</td>
                        <td class="tdNormal1"><asp:TextBox SkinID="textboxSkin" Width="245px" runat="server" ID="txtGrupoDescricao" EnableViewState="false" /></td>
                    </tr>
                    <tr>
                        <td align="center" class="tdNormal1"></td>
                        <td align="center" class="tdNormal1">
                            <input type="hidden" id="txtGrupoID" runat="server" />
                            <asp:Button ID="cmdSalvarGrupo" Text="salvar grupo" SkinID="botaoAzulBorda" runat="server" OnClick="cmdSalvarGrupo_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <asp:Literal runat="server" ID="litMsg" EnableViewState="false" />
            <asp:Panel ID="pnl" runat="server" Visible="false">
                <%--<table cellpadding="2" cellspacing="1" width="445" style="border-left: solid 1px #507CD1;border-right: solid 1px #507CD1;border-bottom: solid 1px #507CD1">
                    <tr runat="server" id="trAdicionar">
                        <td class="tdNormal1" align="right">
                            <asp:LinkButton Visible="true" ID="cmdAdiciona" Text="adicionar" runat="server" OnClick="cmdAdicionar_Click" />
                        </td>
                    </tr>
                </table>
                <br />--%>
                <asp:Panel ID="pnl2" runat="server">
                    <asp:GridView ID="gridItens" SkinID="gridViewSkin" width="445" runat="server" AutoGenerateColumns="False" DataKeyNames="ID" OnRowDataBound="gridItens_RowDataBound" OnRowCommand="gridItens_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="Parcela">
                                <ItemTemplate>
                                    <asp:TextBox SkinID="textboxSkin" Width="20" ID="txtParcela" runat="server" Text='<%# Bind("Parcela") %>' />
                                    <cc1:MaskedEditExtender Mask="99" runat="server" ID="meeParcela" TargetControlID="txtParcela" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Percentual">
                                <ItemTemplate>
                                    <asp:TextBox SkinID="textboxSkin" Width="40" ID="txtPercentual" runat="server" Text='<%# Bind("Percentual") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Perc. Carência">
                                <ItemTemplate>
                                    <asp:TextBox SkinID="textboxSkin" Width="40" ID="txtPercentualCompraCarencia" runat="server" Text='<%# Bind("PercentualCompraCarencia") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Perc. Migração">
                                <ItemTemplate>
                                    <asp:TextBox SkinID="textboxSkin" Width="40" ID="txtPercentualMigracao" runat="server" Text='<%# Bind("PercentualMigracao") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Perc. ADM">
                                <ItemTemplate>
                                    <asp:TextBox SkinID="textboxSkin" Width="40" ID="txtPercentualADM" runat="server" Text='<%# Bind("PercentualADM") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Perc. Especial">
                                <ItemTemplate>
                                    <asp:TextBox SkinID="textboxSkin" Width="40" ID="txtPercentualEspecial" runat="server" Text='<%# Bind("PercentualEspecial") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Perc. Idade">
                                <ItemTemplate>
                                    <asp:TextBox CssClass="textbox" Width="40" ID="txtIdade" runat="server" Text='<%# Bind("Idade") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>

                            <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir">
                                <ItemStyle Width="1%" HorizontalAlign="Center" />
                                <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                            </asp:ButtonField>
                        </Columns>
                    </asp:GridView>
                    <table cellpadding="2" cellspacing="0" width="445">
                        <tr>
                            <td align="right" height="27" class="tdPrincipal1">
                                <asp:Button Width="147" runat="server" ID="cmdAddItem" style="cursor: pointer;"  SkinID="botaoAzulBorda" Text="adicionar item à tabela" BorderWidth="0px" OnClick="cmdAddItem_Click" />
                            </td>
                        </tr>
                    </table>
                    <table style="border-right:solid 1px #507CD1;border-left:solid 1px #507CD1;border-bottom:solid 1px #507CD1;border-top:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="445">
                        <tr>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">&nbsp;</td>
                            <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Vitalícia</td>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">A partir da parcela</td>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;">Percentual</td>
                        </tr>
                        <tr>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Normal</td>
                            <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicio" runat="server" /></td>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicio" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeNumeroParcelaVitalicio" TargetControlID="txtComissionamentoNumeroParcelaVitalicio" /></td>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentual" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Compra de carência</td>
                            <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioCarencia" runat="server" /></td>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioCarencia" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioCarencia" TargetControlID="txtComissionamentoNumeroParcelaVitalicioCarencia" /></td>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualCarencia" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Migração</td>
                            <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioMigracao" runat="server" /></td>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioMigracao" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioMigracao" TargetControlID="txtComissionamentoNumeroParcelaVitalicioMigracao" /></td>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualMigracao" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Administrativa</td>
                            <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioADM" runat="server" /></td>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioADM" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioADM" TargetControlID="txtComissionamentoNumeroParcelaVitalicioADM" /></td>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualADM" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Especial</td>
                            <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioEspecial" runat="server" /></td>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioEspecial" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioEspecial" TargetControlID="txtComissionamentoNumeroParcelaVitalicioEspecial" /></td>
                            <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualEspecial" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="tdNormal1" style="border-right:solid 1px #507CD1">Idade</td>
                            <td class="tdNormal1" align="center" style="border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioIdade" runat="server" /></td>
                            <td class="tdNormal1" style="border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioIdade" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioIdade" TargetControlID="txtComissionamentoNumeroParcelaVitalicioIdade" /></td>
                            <td class="tdNormal1" ><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualIdade" runat="server" /></td>
                        </tr>
                    </table>
                    <table style="border-right:solid 1px #507CD1;border-left:solid 1px #507CD1;border-bottom:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="445">
                        <tr>
                            <td align="center" width="50%"><asp:Button ID="cmdFecharParcelas" Text="fechar" Width="70" runat="server" SkinID="botaoAzulBorda" OnClick="cmdFecharParcelas_Click" /></td>
                            <td align="center" width="50%"><asp:Button ID="cmdSalvarParcelas" Text="salvar" Width="70" runat="server" SkinID="botaoAzulBorda" OnClick="cmdSalvarParcelas_Click" /></td>
                        </tr>
                    </table>
                </asp:Panel>
            </asp:Panel>
            <asp:Panel ID="pnlContratos" runat="server" Visible="false">
                <br />
                <table style="border-right:solid 1px #507CD1;border-left:solid 1px #507CD1;border-bottom:solid 1px #507CD1;border-top:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="445">
                    <tr>
                        <td class="tdNormal1" width='20%'>Operadora</td>
                        <td class="tdNormal1" width='80%'><asp:DropDownList Width="99%" ID="cboOperadora_Novo" runat="server" SkinID="dropdownSkin" AutoPostBack="true" OnSelectedIndexChanged="cboOperadora_Novo_SelectedIndexChanged" /> </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" width='20%'>Contrato ADM</td>
                        <td class="tdNormal1" width='80%'>
                            <asp:DropDownList Width="85%" ID="cboContratoADM_Novo" runat="server" SkinID="dropdownSkin" />
                            <asp:Button ID="cmdAddRelacionamento" Text="salvar" SkinID="botaoPequeno" runat="server" OnClick="cmdAddRelacionamento_Click" />
                        </td>
                    </tr>
                </table>
                <asp:GridView ID="gridContratos" SkinID="gridViewSkin" width="445" runat="server" AutoGenerateColumns="False" DataKeyNames="ID,ContratoGrupoID,OperadoraID" OnRowCommand="gridContratos_RowCommand" OnRowCreated="gridContratos_RowCreated">
                    <Columns>
                     <asp:TemplateField Visible="false">
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkCheckAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chkContrato" runat="server" />
                        </ItemTemplate>
                     </asp:TemplateField>
                        <asp:BoundField DataField="OperadoraDescricao" HeaderText="Operadora">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Descricao" HeaderText="Contrato">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField Visible="false" DataField="TotalNormal" HeaderText="Total %" DataFormatString="{0:F}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:ButtonField Visible="true" Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
                <table style="border-right:solid 1px #507CD1;border-left:solid 1px #507CD1;border-bottom:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="445">
                    <tr>
                        <%--<td class="tdNormal1" width="1%" style="border-right:solid 1px #507CD1"><asp:ImageButton ID="cmdExcluirContratos" ImageUrl="~/images/delete.png" runat="server" ToolTip="excluir selecionados" OnClientClick="return confirm('Deseja realmente excluir os contratos selecionados da relação?');" OnClick="cmdExcluirContratos_Click" /></td>--%>
                        <td align="center"><asp:Button ID="cmdFecharContratos" Text="fechar" Width="70" runat="server" SkinID="botaoAzulBorda" OnClick="cmdFecharContratos_Click" /></td>
                        <%--<td align="center" width="50%"><asp:Button ID="cmdSalvarContratos" Text="salvar" Width="70" runat="server" SkinID="botaoAzulBorda" OnClick="cmdSalvarContratos_Click" /></td>--%>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <table cellpadding="2" width="445">
                <tr>
                    <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
                    <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
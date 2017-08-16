<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="conferenciaTecnico.aspx.cs" Inherits="www.admin.conferenciaTecnico" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"></td></tr>
        <tr><td><span class="titulo">Propostas para conferência</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo as propostas para conferência</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
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
            <table cellpadding="2" cellspacing="0" width="550px" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1" colspan="2" width="180px">Parâmetros</td>
                    <td class="tdPrincipal1"></td>
                </tr>
                <tr>
                    <td class="tdNormal1" width="95">
                        <asp:RadioButton ID="optSem" Text="Pendentes" runat="server" GroupName="a" Checked="true" />
                    </td>
                    <td class="tdNormal1">
                        <asp:RadioButton ID="optCom" Text="Concluídos em" runat="server" GroupName="a" />&nbsp;
                        <asp:TextBox ID="txtData" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                    </td>
                    <td class="tdNormal1" align="center" width="80">
                        <asp:Button ID="cmdLocalizar" Text="Localizar" runat="server" 
                            SkinID="botaoAzulBorda" onclick="cmdLocalizar_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:GridView ID="grid" Width="550" SkinID="gridViewSkin" runat="server" AllowPaging="True" AutoGenerateColumns="False"  
                DataKeyNames="BeneficiarioID,ContratoID,AprovadoDeptoTecnico" OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
                <Columns>
                    <%--<asp:TemplateField HeaderStyle-Width="1%" ItemStyle-Width="1%">
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkCheckAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server" />
                        </HeaderTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:CheckBox ID="chkCliente" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:BoundField DataField="ContratoNumero" HeaderText="Proposta" ItemStyle-Width="25">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="BeneficiarioNome" HeaderText="Beneficiário" ItemStyle-Width="40">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="BeneficiarioSexo" HeaderText="Sexo" ItemStyle-Width="40">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="BeneficiarioIdade" HeaderText="Idade" ItemStyle-Width="30" ItemStyle-HorizontalAlign="Center">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField CommandName="detail" Text="<img src='../images/detail2.png' title='detalhes' alt='detalhes' border='0' />">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:TemplateField Visible="false" HeaderText="" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40">
                        <ItemTemplate>
                            <asp:Image runat="server" ID="imgStatus" />
                        </ItemTemplate>
                        <ItemStyle Width="1%" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <cc1:ModalPopupExtender ID="MPE" runat="server"
                TargetControlID="lnk"
                PopupControlID="pnlDetail"
                BackgroundCssClass="modalBackground" 
                CancelControlID="cmdCloseAlert"
                DropShadow="true"  />
            <asp:Panel runat="server" ID="pnlDetail" EnableViewState="true">
                <asp:LinkButton runat="server" EnableViewState="false" ID="lnk" />
                <table width="350" align="center" bgcolor="gainsboro" style="border:solid 1px black">
                    <tr>
                        <td align="center">
                            <asp:Literal runat="server" ID="litBeneficiario" EnableViewState="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DataList Width="100%" CellPadding="0" CellSpacing="0" ID="dlItens" 
                                DataKeyField="ID" runat="server" OnItemDataBound="dlItens_ItemDataBound">
                                <HeaderTemplate>
                                    <table cellpadding="2" width="550" border="0" style="border:solid 1px #507CD1">
                                </HeaderTemplate>
                                <ItemTemplate>
                                        <tr>
                                            <td class="tdPrincipal1">Evento</td>
                                            <td class="tdNormal1" colspan="3"><asp:Literal ID="lblEvento" Text='<%# DataBinder.Eval(Container.DataItem, "ItemSaudeInstanciaDescricao") %>' runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td width="25%" class="tdPrincipal1">Aprovação médica</td>
                                            <td width="25%" class="tdPrincipal1">CID inicial</td>
                                            <td width="25%" class="tdPrincipal1">CID final</td>
                                            <td width="25%" class="tdPrincipal1">Data</td>
                                        </tr>
                                        <tr>
                                            <td class="tdNormal1"><asp:Literal ID="lblAprovMed" Text='<%# DataBinder.Eval(Container.DataItem, "AprovadoMedico") %>' runat="server" /></td>
                                            <td class="tdNormal1"><asp:Literal ID="lblCidInicial" Text='<%# DataBinder.Eval(Container.DataItem, "CIDInicial") %>' runat="server" /></td>
                                            <td class="tdNormal1"><asp:Literal ID="lblCidFinal" Text='<%# DataBinder.Eval(Container.DataItem, "CIDFinal") %>' runat="server" /></td>
                                            <td class="tdNormal1"><asp:Literal ID="lblDataMed" Text='<%# DataBinder.Eval(Container.DataItem, "strDataAprovadoPeloMedico") %>' runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td class="tdPrincipal1">Parecer médico</td>
                                            <td class="tdNormal1" colspan="3"><asp:Literal ID="lblParecerMed" Text='<%# DataBinder.Eval(Container.DataItem, "OBSMedico") %>' runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <asp:Literal runat="server" ID="litRegraOutput" Text="<i>regra: não verficado</i>" />
                                                <asp:Button ForeColor="blue" CssClass="fonte11" ID="lnkRegra" Text="consultar regras" runat="server" OnClick="lnkRegra_Click" CausesValidation="False" Visible="false" />
                                            </td>
                                        </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:DataList>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <input runat="server" style="width:55px;font-size:10px;font-family:Arial" id="cmdCloseAlert" type="button" value="Fechar" />
                            &nbsp;
                            <asp:Button runat="server" style="width:55px;font-size:10px;font-family:Arial" id="cmdRegras" Text="Regras" ForeColor="blue" OnClick="cmdRegras_Click" />
                            &nbsp;
                            <asp:Button runat="server" style="width:55px;font-size:10px;font-family:Arial" id="cmdAprovar" Text="Aprovar" ForeColor="green" OnClientClick="return confirm('Deseja realmente aprovar este beneficiário?');" OnClick="cmdAprovar_Click" />
                            &nbsp;
                            <asp:Button runat="server" style="width:55px;font-size:10px;font-family:Arial" id="cmdReprovar" Text="Reprovar" ForeColor="red" OnClientClick="return confirm('Deseja realmente reprovar este beneficiário?\nIsso cancelará a proposta.');" OnClick="cmdReprovar_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <%--<br />
            <table id="tblSalvar" visible="false" runat="server" cellpadding="2" cellspacing="0" width="100%">
                <tr height="18">
                    <td height="18" align="right" valign="bottom">Concluído em</td>
                    <td height="18"></td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:TextBox ID="txtDataSalvar" runat="server" SkinID="textboxSkin" Width="60" />
                        <cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeDataSalvar" Mask="99/99/9999" MaskType="Date" ClearMaskOnLostFocus="true" TargetControlID="txtDataSalvar" />
                    </td>
                    <td width="55"><asp:Button ID="cmdSalvar" Text="Salvar" OnClientClick="return confirm('Deseja realmente prosseguir?');" SkinID="botaoAzulBorda" runat="server" Width="65" onclick="cmdSalvar_Click" /></td>
                </tr>
            </table>--%>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
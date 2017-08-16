<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false"
    CodeBehind="arquivoConsultaAtendimento.aspx.cs" Inherits="www.arquivoConsultaAtendimento" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr>
            <td rowspan="3" width="40">
                <img height="50" src="images/imgTitulos/operadoras.jpg" />
            </td>
        </tr>
        <tr>
            <td>
                <span class="titulo">Consulta de Atendimentos</span>
            </td>
        </tr>
        <tr>
            <td nowrap="true">
                <span class="subtitulo">Preencha os filtros abaixo para encontrar os atendimentos desejados</span>
            </td>
        </tr>
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
    <asp:UpdatePanel runat="server" ID="upDadosComuns" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="2" border="0">
                <tr>
                    <td class="tdPrincipal1" width="88px">Operadora</td>
                    <td class="tdNormal1" colspan="3">
                        <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadora"
                            AutoPostBack="true" Width="258px" OnSelectedIndexChanged="cboOperadora_OnSelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="88px">Assunto</td>
                    <td class="tdNormal1" colspan="3">
                        <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboTipo" 
                            AutoPostBack="true" Width="258px" OnSelectedIndexChanged="cboTipo_OnSelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="88px">Atendente</td>
                    <td class="tdNormal1" colspan="3">
                        <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboAtendente" AutoPostBack="true" Width="258px" OnSelectedIndexChanged="cboAtendente_Changed" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="88px">De</td>
                    <td class="tdNormal1">
                        <asp:TextBox CssClass="textbox" Width="60px" runat="server" ID="txtDataDe" AutoPostBack="true" OnTextChanged="onParamaterChange" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                        <asp:Image SkinID="imgCanlendario" ID="imgDataDe" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataDe" TargetControlID="txtDataDe" PopupButtonID="imgDataDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                    </td>
                    <td class="tdPrincipal1" width="88px">Até</td>
                    <td class="tdNormal1">
                        <asp:TextBox CssClass="textbox" Width="60px" runat="server" ID="txtDataAte" AutoPostBack="true" OnTextChanged="onParamaterChange" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                        <asp:Image SkinID="imgCanlendario" ID="imgDataAte" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDataAte" TargetControlID="txtDataAte" PopupButtonID="imgDataAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                    </td>
                </tr>
                <tr>
                    <td class="tdNormal1" colspan="4">
                        <asp:RadioButton runat="server" ID="rdbPendente" Text="Pendentes" GroupName="rdbStatus" Checked="true" AutoPostBack="true" OnCheckedChanged="onParamaterChange" />
                        <asp:RadioButton runat="server" ID="rdbConcluido" Text="Concluídos" GroupName="rdbStatus" AutoPostBack="true" OnCheckedChanged="onParamaterChange" />
                    </td>
                </tr>
            </table>
            <asp:Panel runat="server" ID="pnlAtendimentos" Visible="false">
                <br />
                <asp:Label runat="Server" ID="lblMsg" Visible="false">Nenhum atendimento encontrado.</asp:Label>
                <asp:GridView runat="server" ID="gdvAtendimentos" SkinID="gridViewSkin" AllowPaging="false"
                    AutoGenerateColumns="False" visible="false" DataKeyNames="ItemId,OperadoraID,NumeroContrato,ItemBeneficiarioIds,ItemPrazo" OnRowDataBound="gdvAtendimentos_OnRowDataBound">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkCheckAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server" />
                            </HeaderTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:CheckBox ID="chkAtendimento" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Protocolo" HeaderText="Protocolo">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NumeroContrato" HeaderText="Contrato">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataHora" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ItemPrazo" HeaderText="Prazo" DataFormatString="{0:dd/MM/yyyy}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Nome" HeaderText="Nome">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Telefone" HeaderText="Telefone">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Email" HeaderText="E-mail">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CPF" HeaderText="CPF">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ItemTexto" HeaderText="Texto">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
                <asp:DataList Width="100%" CellPadding="0" CellSpacing="0" ID="dlAtendimento" DataKeyField="ItemId" runat="server" OnItemDataBound="dlAtendimento_ItemDataBound">
                    <HeaderTemplate>
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td height="25" class="tdPrincipal1"><asp:CheckBox ID="chkDlCheckAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server" /></td>
                                <td height="25" class="tdPrincipal1">Protocolo</td>
                                <td height="25" class="tdPrincipal1">Contrato</td>
                                <td height="25" class="tdPrincipal1">Data</td>
                                <td height="25" class="tdPrincipal1">Prazo</td>
                                <td height="25" class="tdPrincipal1">Telefone</td>
                                <td height="25" class="tdPrincipal1">E-mail</td>
                                <td height="25" class="tdPrincipal1">CPF</td>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="tdNormal1" width="1" style="border-left: solid 1px #507CD1">
                                <asp:CheckBox ID="chkDlCheck" runat="server" />
                            </td>
                            <td width="100" class="tdNormal1_NonBold" align="left">
                                <asp:Literal ID="lblProtocolo" Text='<%# DataBinder.Eval(Container.DataItem, "Protocolo") %>' runat="server" />
                                <asp:Literal ID="litItemDeclaracaoIDKey" Text='<%# DataBinder.Eval(Container.DataItem, "OperadoraID") %>' runat="server" Visible="false" />
                                <asp:Literal ID="litNumeroContratoKey" Text='<%# DataBinder.Eval(Container.DataItem, "NumeroContrato") %>' runat="server" Visible="false" />
                                <asp:Literal ID="litItemBeneficiarioIDsKey" Text='<%# DataBinder.Eval(Container.DataItem, "ItemBeneficiarioIds") %>' runat="server" Visible="false" />
                                <asp:Literal ID="litItemPrazoKey" Text='<%# DataBinder.Eval(Container.DataItem, "ItemPrazo") %>' runat="server" Visible="false" />
                            </td>
                            <td width="60" class="tdNormal1_NonBold" align="left">
                                <asp:Literal ID="lblNumContrato" Text='<%# DataBinder.Eval(Container.DataItem, "NumeroContrato") %>' runat="server" />
                            </td>
                            <td width="125" class="tdNormal1_NonBold" align="left">
                                <asp:Literal ID="lblDataHora" Text='<%# DataBinder.Eval(Container.DataItem, "DataHora") %>' runat="server" />
                            </td>
                            <td width="100" class="tdNormal1_NonBold" align="left">
                                <asp:Literal ID="lblPrazo" Text='<%# Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "ItemPrazo")).ToString("dd/MM/yyyy") %>' runat="server" />
                            </td>
                            <td width="90" class="tdNormal1_NonBold" align="left">
                                <asp:Literal ID="lblFone" Text='<%# DataBinder.Eval(Container.DataItem, "Telefone") %>' runat="server" />
                            </td>
                            <td width="190" class="tdNormal1_NonBold" align="left">
                                <asp:Literal ID="lblEmail" Text='<%# DataBinder.Eval(Container.DataItem, "Email") %>' runat="server" />
                            </td>
                            <td width="100" style="border-right: solid 1px #507CD1" class="tdNormal1_NonBold" align="left">
                                <asp:Literal ID="lblCpf" Text='<%# DataBinder.Eval(Container.DataItem, "CPF") %>' runat="server" />&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td style="border-left: solid 1px #507CD1" class="tdNormal1">&nbsp;</td>
                            <td class="tdNormal1" colspan="3">
                                Nome
                            </td>
                            <td style="border-right: solid 1px #507CD1" class="tdNormal1" colspan="5">
                                Texto
                            </td>
                        </tr>
                        <tr>
                            <td style="border-bottom: solid 1px #507CD1;border-left: solid 1px #507CD1" class="tdNormal1">&nbsp;</td>
                            <td style="border-bottom: solid 1px #507CD1" class="tdNormal1_NonBold" colspan="3">
                                <asp:Literal ID="Literal1" Text='<%# DataBinder.Eval(Container.DataItem, "Nome") %>' runat="server" />
                            </td>
                            <td style="border-bottom: solid 1px #507CD1;border-right: solid 1px #507CD1" class="tdNormal1_NonBold" colspan="5">
                                <asp:Literal ID="Literal2" Text='<%# DataBinder.Eval(Container.DataItem, "ItemTexto") %>' runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:DataList>
                <br />
                <table cellpadding="0" cellspacing="0" border="0">
                    <tr><td height="5"></td></tr>
                    <tr>
                        <td>
                            <asp:Button Width="140" runat="server" ID="cmdEfetivarAtendimento" SkinID="botaoAzulBorda" Text="Efetivar Atendimento" OnClick="cmdEfetivarAtendimento_OnClick" visible="false" OnClientClick="return confirm('Atenção!\nDeseja realmente dar por \'concluídos\' os atendimentos selecionados?');" />
                        </td>
                        <td>
                            <asp:Button Width="140" runat="server" ID="cmdDeclinar" SkinID="botaoAzulBorda" Text="Cancelar Atendimento" OnClick="cmdDeclinar_OnClick" visible="false" OnClientClick="return confirm('Atenção!\nDeseja realmente dar por \'cancelados\' os atendimentos selecionados?');" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

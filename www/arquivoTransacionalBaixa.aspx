<%@Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="arquivoTransacionalBaixa.aspx.cs" Inherits="www.arquivoTransacionalBaixa"  %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="cPageTitle" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td rowspan="3" width="54"><img src="images/imgTitulos/beneficiarios.png" alt="Beneficiarios" /></td></tr>
        <tr><td><span class="titulo">Arquivos Transacionais - Baixa</span></td></tr>
        <tr><td><span class="subtitulo">Baixa de Beneficiários</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="cPageContent" ContentPlaceHolderID="cphContent" runat="server">
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
    <asp:UpdatePanel ID="up" UpdateMode="Always" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnRegerar" />
        </Triggers>
        <ContentTemplate>
            <table width="100%">
                <tr>
                    <td>
                        <table>
                             <tr>
                                <td>Operadora:</td>
                                <td><asp:DropDownList ID="cboOperadora" AutoPostBack="true" OnSelectedIndexChanged="cboOperadora_OnSelectedIndexChanged" SkinID="dropdownSkin" Width="225"  runat="server"></asp:DropDownList></td>
                             </tr>
                             <tr>
                                <td>Tipo Transação:</td>
                                <td><asp:DropDownList ID="cboTipoTransacao" AutoPostBack="true" OnSelectedIndexChanged="cboTipoTransacao_OnSelectedIndexChanged" SkinID="dropdownSkin" Width="225" runat="server" /><asp:Literal ID="litQtdResult" runat="server" /></td>
                             </tr>
                             <tr>
                                <td>Lotes:</td>
                                <td><asp:DropDownList ID="cboLotes" AutoPostBack="true" OnSelectedIndexChanged="cboLotes_OnSelectedIndexChanged" SkinID="dropdownSkin" Width="225" runat="server" /><asp:Literal ID="Literal1" runat="server" /></td>
                             </tr>
                        </table>
                    </td>
                </tr>
                <tr><td><div style="height: 1px;"></div></td></tr>
                <tr><td><div style="height: 1px; background-color: #507CD1; width: 100%"></div></td></tr>
                <tr><td><div style="height: 5px;"></div></td></tr>
                <tr>
                    <td>
                        <asp:Panel ID="pnlListaBeneficiario" Visible="false" runat="server">
                            <asp:GridView PagerSettings-PageButtonCount="50" ID="gridBeneficiario" Width="100%" PageSize="40" SkinID="gridViewSkin" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="contratobeneficiario_contratoId, contratobeneficiario_beneficiarioId, contratobeneficiario_id,item_lote_id" OnRowDataBound="gridBeneficiario_RowDataBound" OnRowCreated="gridBeneficiario_RowCreated" OnPageIndexChanging="gridBeneficiario_OnPageIndexChanging">
                                <Columns>
                                     <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkCheckAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server" />
                                        </HeaderTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkBeneficiario" runat="server" />
                                        </ItemTemplate>
                                     </asp:TemplateField>
                                     <asp:BoundField HeaderText="Proposta" DataField="contrato_numero">
                                        <HeaderStyle HorizontalAlign="Left" />
                                     </asp:BoundField>
                                     <asp:BoundField HeaderText="Beneficiario" DataField="beneficiario_nome">
                                        <HeaderStyle HorizontalAlign="Left" />
                                     </asp:BoundField>
                                     <asp:BoundField HeaderText="Data" ><%--DataField="lote_data_criacao" DataFormatString="{0:dd/MM/yyyy HH:mm}"--%>
                                        <HeaderStyle HorizontalAlign="Left" />
                                     </asp:BoundField>
                                     <asp:BoundField HeaderText="Lote Numeração" DataField="">
                                        <HeaderStyle HorizontalAlign="Left" />
                                     </asp:BoundField>
                                     <asp:BoundField HeaderText="Movimentação">
                                        <HeaderStyle HorizontalAlign="Left" />
                                     </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                        <asp:Literal ID="litMessage" runat="server" Visible="false"></asp:Literal>
                    </td>
                </tr>
                <tr><td><div style="height: 1px;"></div></td></tr>
                <tr><td><div style="height: 1px; background-color: #507CD1; width: 100%"></div></td></tr>
                <tr><td><div style="height: 5px;"></div></td></tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="btnRegerar" OnClientClick="return confirm('Deseja realmente desfazer o lote?\nEssa operação não poderá ser desfeita!');" Text="Desfazer Lote" OnClick="btnRegerar_OnClick" runat="server" SkinID="botaoAzul" Visible="false" Width="144" />&nbsp;
                        <asp:Button ID="btnBaixarTodas" SkinID="botaoAzul" Visible="false" OnClientClick="return confirm('Deseja realmente baixar TODOS os beneficiários ?\nEssa operação não poderá ser desfeita!');" OnClick="btnBaixarTudo_OnClick" Text="Baixar Todos" runat="server" Width="144" />&nbsp;
                        <asp:Button ID="btnBaixa" SkinID="botaoAzul" Visible="false" OnClientClick="return confirm('Deseja realmente baixar os beneficiários selecionados?\nEssa operação não poderá ser desfeita!');" OnClick="btnBaixa_OnClick" Text="Baixar Selecionados" runat="server" Width="144" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

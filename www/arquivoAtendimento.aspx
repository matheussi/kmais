<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="arquivoAtendimento.aspx.cs" Inherits="www.arquivoAtendimento" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register src="usercontrols/ucBeneficiarioForm.ascx" tagname="ucBeneficiarioForm" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr>
            <td rowspan="3" width="40">
                <img height="50" src="images/imgTitulos/operadoras.jpg" />
            </td>
        </tr>
        <tr>
            <td>
                <span class="titulo">Atendimento</span>
            </td>
        </tr>
        <tr>
            <td nowrap="true">
                <span class="subtitulo">Preencha os campos abaixo e clique em salvar</span>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <script language="javascript" type="text/javascript">
         /* <![CDATA[ */
		 function SelectAllCheckboxes(spanChk){

		   var oItem = spanChk.children;
		   var theBox= (spanChk.type=="checkbox") ? 
				spanChk : spanChk.children.item[0];
		   xState=theBox.checked;
		   elm=theBox.form.elements;

		   for(i=0;i<elm.length;i++)
			 if(elm[i].type=="checkbox" && 
					  elm[i].id!=theBox.id)
			 {
			   if(elm[i].checked!=xState)
				 elm[i].click();
			 }
		 }
		 /* ]]> */
	</script>
    <asp:UpdatePanel runat="server" ID="upDadosComuns" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="2" border="0" width="700" style="border: 1px solid #507CD1">
                <tr>
                    <td colspan="4" class="tdNormal1" align="center" height="30">Filtro</td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="100">Operadora</td>
                    <td class="tdNormal1" colspan="3">
                        <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadora" Width="487"/>
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="100">N. do Contrato</td>
                    <td class="tdNormal1" colspan="3">
                        <asp:TextBox CssClass="textbox" Width="146px" runat="server" ID="txtNumeroContrato" MaxLength="50" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Beneficiário</td>
                    <td class="tdNormal1" colspan="3">
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtBeneificarioNome" Width="250" MaxLength="40" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="100">CPF</td>
                    <td class="tdNormal1" colspan="3">
                        <asp:TextBox CssClass="textbox" Width="146px" runat="server" ID="txtCPF" MaxLength="14" />
                        <cc1:MaskedEditExtender runat="server" ID="meeCPF" Mask="999\.999\.999-99" TargetControlID="txtCPF" />
                        &nbsp;&nbsp;&nbsp;
                        <asp:Button Visible="false" Width="90" runat="server" ID="cmdAbrir" SkinID="botaoAzul" Text="Abrir" OnClick="cmdAbrir_OnClick" />
                        <asp:Button runat="server" ID="Button1" SkinID="botaoAzul" Text="Localizar" Width="80" onclick="cmdLocalizar_Click" />
                    </td>
                </tr>
            </table>
            <asp:GridView ID="gridContratos" Width="700" runat="server" AllowPaging="True" AutoGenerateColumns="False"  
                DataKeyNames="ID,Rascunho,Cancelado" OnPageIndexChanging="gridContratos_PageIndexChanging" OnRowDataBound="gridContratos_RowDataBound" OnRowCommand="gridContratos_RowCommand" >
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
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image ID="Image1" ImageUrl="~/images/rascunho.png" ToolTip="rascunho" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="1%" />
                    </asp:TemplateField>
                    <asp:ButtonField Text="<img src='images/active.png' title='status' alt='status' border='0' />" CommandName="inativar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                        <ItemStyle Font-Size="10px" Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='images/new_generic.png' title='abrir atendimento' alt='abrir atendimento' border='0' />" CommandName="abrir" >
                        <ItemStyle Font-Size="10px" Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <br />
            <table cellpadding="2" style="border: 1px solid #507CD1" cellspacing="2" border="0" width="700">
                <tr>
                    <td colspan="4" class="tdNormal1" align="center" height="30">Atendimento</td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="100">Data</td>
                    <td class="tdNormal1" width='320'>
                        <asp:TextBox CssClass="textbox" Width="70px" runat="server" ID="txtData" ReadOnly="true" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                    </td>
                    <td class="tdPrincipal1" width="100">Hora</td>
                    <td class="tdNormal1" width='277'>
                        <asp:TextBox CssClass="textbox" Width="30px" runat="server" ID="txtHora" MaxLength="5" ReadOnly="true" />
                        <cc1:MaskedEditExtender runat="server" MaskType="Time" ID="meeHora" Mask="99:99"
                            TargetControlID="txtHora" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="100">Titular</td>
                    <td class="tdNormal1" width='320'>
                        <asp:TextBox CssClass="textbox" Width="220px" runat="server" ID="txtNome" MaxLength="255" ReadOnly="true" />
                    </td>
                    <td class="tdPrincipal1" width="100">Telefone</td>
                    <td class="tdNormal1" width='277'>
                        <asp:TextBox CssClass="textbox" Width="80px" runat="server" ID="txtTelefone" MaxLength="20" ReadOnly="true" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="100">E-mail</td>
                    <td class="tdNormal1" colspan="3">
                        <asp:TextBox CssClass="textbox" Width="220px" runat="server" ID="txtEmail" MaxLength="255" ReadOnly="true" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:Panel ID="pnlItensAtendimento" runat="server" Visible="false">
                <table cellpadding="2" cellspacing="2" border="0" width="700">
                    <tr>
                        <td class="tdPrincipal1" colspan="4"><asp:Literal runat="server" ID="litProtocolo"></asp:Literal></td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1" width='100'>Assunto</td>
                        <td class="tdNormal1" width='320'>
                            <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboTipo"
                                AutoPostBack="true" OnSelectedIndexChanged="cboTipo_OnSelectedIndexChanged" />
                        </td>
                        <td class="tdPrincipal1" width='100'>Prazo</td>
                        <td class="tdNormal1" width='277'>
                            <asp:TextBox CssClass="textbox" Width="70px" runat="server" ID="txtPrazo" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1" colspan='4'>Observações</td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:TextBox runat="server" ID="txtTexto" Rows="4" Width="688" BorderColor="#000000" BorderWidth="1" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Panel runat="server" ID="pnlClientes" Visible="false">
                                <br />
                                <asp:GridView runat="server" ID="gdvClientes" SkinID="gridViewSkin" AllowPaging="false"
                                    AutoGenerateColumns="False" Width="692px" DataKeyNames="ContratoBeneficiarioId" 
                                    OnRowCommand="gdvClientes_RowCommand" OnRowCreated="gdvClientes_RowCreated">
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkCheckAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server" />
                                            </HeaderTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkCliente" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemStyle Width="1%" />
                                            <ItemTemplate>
                                                <asp:LinkButton CommandName="EditarCadastro" ID="btnEditaCadastro" AlternateText="Editar" runat="server" SkinID="botaoAzul" Text="Editar" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="OperadoraDescricao" HeaderText="Operadora">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Numero" HeaderText="N. Contrato">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="PlanoDescricao" HeaderText="Plano">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Admissao" HeaderText="Admissão" DataFormatString="{0:dd/MM/yyyy}">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="BeneficiarioTitularNome" HeaderText="Beneficiário">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="BeneficiarioCPF" HeaderText="CPF">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlAlteracaoCadastralCPF" Visible="false" Width="695">
                                <br />
                                <table cellpadding="2" cellspacing="2" border="0" width="692">
                                    <tr>
                                        <td class="tdPrincipal1" width="100">CPF</td>
                                        <td class="tdNormal1">
                                            <asp:TextBox CssClass="textbox" Width="130px" runat="server" ID="txtCPFNovoBeneficiario" MaxLength="14" />
                                            <cc1:MaskedEditExtender runat="server" ID="ttmask2" Mask="999\.999\.999-99" TargetControlID="txtCPFNovoBeneficiario" />
                                            &nbsp;&nbsp;&nbsp;
                                            <asp:Button Width="90" runat="server" ID="cmdLocalizar" SkinID="botaoAzul" Text="Localizar" OnClick="cmdLocalizar_OnClick" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlAlteracaoCadastral" Visible="false" width="695">
                                <br />
                                <uc1:ucBeneficiarioForm CarregarDeInicio="true" ID="UcBeneficiarioForm" runat="server" />
                                <asp:Button Width="90" runat="server" ID="btnFechar" SkinID="botaoAzul" Text="Fechar" OnClick="btnFechar_OnClick" />
                                <asp:Button Width="90" runat="server" ID="btnSalvarCadastro" SkinID="botaoAzul" Text="Salvar Dados" OnClick="btnSalvarCadastro_OnClick" />
                            </asp:Panel>
                            <asp:Panel ID="pnl2ViaBoleto" runat="server" Visible="false">
                                <br />
                                <asp:GridView runat="server" ID="gdvCobrancas" SkinID="gridViewSkin" AllowPaging="false"
                                    AutoGenerateColumns="False" visible="false" Width="692px" 
                                    OnRowCommand="gdvCobrancas_RowCommand" DataKeyNames="ID,Valor,DataVencimento,ContratoTitularNome,BeneficiarioEmail" 
                                    OnRowCreated="gdvCobrancas_RowCreated">
                                    <Columns>
                                        <asp:BoundField DataField="DataVencimento" HeaderText="Data de Vencimento" DataFormatString="{0:dd/MM/yyyy}">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C}">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:TemplateField>
                                            <ItemStyle Width="1%" />
                                            <ItemTemplate>
                                                <asp:Button CommandName="GerarBoleto" ID="btnGerarBoleto" AlternateText="Gerar Boleto" runat="server" SkinID="botaoAzul" Text="Gerar Boleto" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlAlteracaoPlano" Visible="false">
                                <br />
                                <table cellpadding="2" cellspacing="2" border="0" width="692">
                                    <tr>
                                        <td class="tdPrincipal1" colspan="2">Alteração de Planos</td>
                                    </tr>
                                    <tr>
                                        <td class="tdPrincipal1" width="100">Contrato</td>
                                        <td class="tdNormal1">
                                            <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboContrato"
                                                AutoPostBack="true" OnSelectedIndexChanged="cboContrato_OnSelectedIndexChanged" Width="300px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdPrincipal1" width="100">Plano</td>
                                        <td class="tdNormal1">
                                            <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboPlano"
                                                AutoPostBack="true" OnSelectedIndexChanged="cboPlano_OnSelectedIndexChanged" Width="300px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdPrincipal1" width="100">Acomodação</td>
                                        <td class="tdNormal1">
                                            <asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboAcomodacao"
                                                AutoPostBack="true" Width="300px" />
                                        </td>
                                    </tr>
                                 </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr runat="server" id="trBtnSalvar">
                        <td colspan="2" align="left">
                            <asp:Button Width="100" runat="server" ID="cmdFinalizar" SkinID="botaoAzul" Text="Finalizar"
                                OnClick="cmdFinalizar_OnClick" OnClientClick="return confirm('Atenção!\nDeseja realmente finalizar o atendimento?');"  />
                        </td>
                        <td colspan="2" align="right">
                            <asp:Button Width="100" runat="server" ID="cmdSalvar" SkinID="botaoAzul" Text="Salvar"
                                OnClick="cmdSalvar_OnClick" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel runat="server" ID="upnlAlerta" UpdateMode="Conditional" EnableViewState="false">
        <ContentTemplate>
            <cc1:ModalPopupExtender ID="MPE" runat="server"
                TargetControlID="lnk"
                PopupControlID="pnlAlert"
                BackgroundCssClass="modalBackground" 
                CancelControlID="cmdCloseAlert"
                DropShadow="true"  />
            <asp:Panel runat="server" ID="pnlAlert" EnableViewState="false">
                <asp:LinkButton runat="server" EnableViewState="false" ID="lnk" />
                <table width="350" align="center" bgcolor="gainsboro" style="border:solid 1px black">
                    <tr>
                        <td align="center">
                            <asp:Literal runat="server" ID="litAlert" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr height="8"><td height="8">&nbsp</td></tr>
                    <tr>
                        <td align="center">
                            <input runat="server" style="width:45px;font-size:12px;font-family:Arial" id="cmdCloseAlert" type="button" value="OK" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="arquivoCobrancas.aspx.cs" Inherits="www.financeiro.cobrancas" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Cobranças</span></td></tr>
        <tr><td><span class="subtitulo">Gerar arquivos de remessa</span></td></tr>
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
            <span class="subtitulo">Tipos de arquivo de cobrança</span>
            <asp:GridView ID="gridCriterios" Width="720px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ID">
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle Width="1%" />
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkCheckAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSel" runat="server" />
                        </ItemTemplate>
                     </asp:TemplateField>
                    <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Projeto" HeaderText="Projeto">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
            <br />
            <table width="420px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                <tr style="display:none">
                    <td width="150" class="tdPrincipal1">Carteira</td>
                    <td class="tdNormal1">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:DropDownList ID="cboCarteira" runat="server" Width="164" SkinID="dropdownSkin" />
                    </td>
                </tr>
                <tr style="display:none">
                    <td width="150" class="tdPrincipal1">Formato</td>
                    <td class="tdNormal1">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:DropDownList ID="cboFormato" runat="server" Width="164" SkinID="dropdownSkin" AutoPostBack="true" OnSelectedIndexChanged="cboFormato_Changed" />
                    </td>
                </tr>
                <tr runat="server" id="trNomeArquivo" visible="false">
                    <td width="150" class="tdPrincipal1">Nome do arquivo</td>
                    <td class="tdNormal1">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:TextBox EnableViewState="false" ID="txtNomeArquivo" runat="server" Width="161" MaxLength="40" SkinID="textboxSkin" />
                    </td>
                </tr>
                <tr>
                    <td width="150" class="tdPrincipal1">Qtd. de boletos</td>
                    <td class="tdNormal1">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:TextBox ID="txtQtdBoletos" runat="server" SkinID="textboxSkin" Width="45" MaxLength="2" />
                    </td>
                </tr>
                <tr>
                    <td width="150" class="tdPrincipal1">Intervalo de vencimento</td>
                    <td class="tdNormal1">
                        De&nbsp;
                        <asp:TextBox ID="txtVencimentoDe" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        <asp:Image SkinID="imgCanlendario" ID="imgVencimentoDe" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceVencimentoDe" TargetControlID="txtVencimentoDe" PopupButtonID="imgVencimentoDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        &nbsp;&nbsp;Até&nbsp;
                        <asp:TextBox ID="txtVencimentoAte" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        <asp:Image SkinID="imgCanlendario" ID="imgVencimentoAte" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceVencimentoAte" TargetControlID="txtVencimentoAte" PopupButtonID="imgVencimentoAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                    </td>
                </tr>
                <tr>
                    <td width="150" class="tdPrincipal1">Intervalo de vigência</td>
                    <td class="tdNormal1">
                        De&nbsp;
                        <asp:TextBox ID="txtVigenciaDe" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        <asp:Image SkinID="imgCanlendario" ID="imgVigenciaDe" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceVigenciaDe" TargetControlID="txtVigenciaDe" PopupButtonID="imgVigenciaDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        &nbsp;&nbsp;Até&nbsp;
                        <asp:TextBox ID="txtVigenciaAte" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        <asp:Image SkinID="imgCanlendario" ID="imgVigenciaAte" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceVigenciaAte" TargetControlID="txtVigenciaAte" PopupButtonID="imgVigenciaAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                    </td>
                </tr>
                <tr>
                    <td width="150" class="tdPrincipal1">Processar</td>
                    <td class="tdNormal1">
                        Em  
                        <asp:TextBox ID="txtProcessarEm" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        <asp:Image SkinID="imgCanlendario" ID="imgProcessarEm" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceProcessarEm" TargetControlID="txtProcessarEm" PopupButtonID="imgProcessarEm" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        &nbsp;
                        <asp:DropDownList runat="server" ID="cboHora" Width="40" SkinID="dropdownSkin" />&nbsp;:&nbsp;<asp:DropDownList runat="server" ID="cboMinuto" Width="40" SkinID="dropdownSkin" />
                        &nbsp;
                        <asp:Button ID="cmdSalvar" Text="Salvar" OnClick="cmdSalvar_Click" SkinID="botaoAzulBorda" runat="server" EnableViewState="false" OnClientClick="return confirm('Confirma a solicitação de processamento de arquivo de remessa?');" />
                    </td>
                </tr>
            </table>
            <br />
            <span class="subtitulo"><asp:Literal runat="server" ID="litAgendamento" Text="Agendamentos de processamento (nenhum)" /></span>
            <asp:GridView ID="gridAgendamento" Width="720px" SkinID="gridViewSkin" OnRowCommand="gridAgendamento_RowCommand" OnRowCreated="gridAgendamento_RowCreated"
                runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ID">
                <Columns>
                    <asp:BoundField DataField="STRVencimento" HeaderText="Vencimento">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="STRVigencia" HeaderText="Vigência">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="QtdBoletos" HeaderText="Boletos">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ProcessamentoEm" HeaderText="Processamento" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                     <asp:TemplateField>
                        <ItemStyle Width="1%" />
                        <ItemTemplate>
                            <asp:ImageButton CommandName="excluir" ID="imgExcluir" AlternateText="Excluir" ToolTip="Excluir" ImageUrl="~/images/delete.png" runat="server" />
                        </ItemTemplate>
                     </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Panel runat="server" ID="pnl" Visible="false" EnableViewState="false">
            <table width="420px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                <tr>
                    <td width="76" class="tdPrincipal1">Estipulantes</td>
                    <td class="tdNormal1"><asp:ListBox Height="100" SelectionMode="Multiple" SkinID="listBoxSkin" runat="server" ID="lstEstipulantes" Width="100%" /></td> <%--AutoPostBack="true" OnSelectedIndexChanged="lstEstipulante_SelectedIndexChanged"--%>
                </tr>
                <tr>
                    <td width="76" class="tdPrincipal1">Operadoras</td>
                    <td class="tdNormal1"><asp:ListBox Height="100" SelectionMode="Multiple" SkinID="listBoxSkin" runat="server" ID="lstOperadoras" Width="100%" /></td> <%--AutoPostBack="true" onselectedindexchanged="cboOperadoras_SelectedIndexChanged" --%>
                </tr>
                <tr>
                    <td width="76" class="tdPrincipal1">Filiais</td>
                    <td class="tdNormal1"><asp:ListBox SelectionMode="Multiple" SkinID="listBoxSkin" runat="server" ID="lstFiliais" Width="100%" /></td> 
                </tr>
                <tr>
                    <td class="tdPrincipal1">Tipo</td>
                    <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboTipo" Width="100%" /></td> 
                </tr>
                <tr>
                    <td width="76" class="tdPrincipal1">Vencimento</td>
                    <td class="tdNormal1"><b>Mês</b>&nbsp;<asp:DropDownList ID="cboMes" Width="45" SkinID="dropdownSkin" runat="server" />&nbsp;&nbsp;&nbsp;<b>Ano</b>&nbsp;<asp:DropDownList ID="cboAno" Width="55" SkinID="dropdownSkin" runat="server" />&nbsp;&nbsp;&nbsp;<asp:Button ID="cmdGerar" Text="Gerar arquivos" Width="90" SkinID="botaoAzulBorda" runat="server" OnClick="cmdGerar_Click" OnClientClick="return confirm('Deseja realmente prosseguir?');" /></td>
                </tr>
            </table>
            <br />
            <asp:Literal ID="litMsg" runat="server" EnableViewState="true" />
            <asp:Panel ID="pnlResultado" runat="server" Visible="false">
                <asp:GridView ID="grid" Width="420px" SkinID="gridViewSkin" 
                    runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ArquivoID,OperadoraID"
                    OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" 
                    onrowcreated="grid_RowCreated">
                    <Columns>
                        <asp:BoundField DataField="ArquivoNome" HeaderText="Arquivo">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ArquivoVersao" HeaderText="Versão">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="QtdCobrancas" HeaderText="Qtd.">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                         <asp:TemplateField>
                            <ItemStyle Width="1%" />
                            <ItemTemplate>
                                <asp:ImageButton CommandName="baixar" ID="imbDownloadArquivo" AlternateText="Baixar" ToolTip="Baixar" ImageUrl="~/images/download.gif" runat="server" />
                            </ItemTemplate>
                         </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
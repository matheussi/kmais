<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="cReceberTaxa.aspx.cs" Inherits="www.reports.cReceberTaxa" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo">Relatório de contas a receber - Pago</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Utilize os filtros abaixo para gerar o relatório de contas a receber</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel ID="pnlAgenda" runat="server" Visible="false">
        <asp:GridView ID="gridAgenda" Width="360px" SkinID="gridViewSkin" 
            runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ID,Processado" onrowcommand="gridAgenda_RowCommand" 
            onrowdatabound="gridAgenda_RowDataBound" PageSize="10" OnPageIndexChanging="gridAgenda_PageIndexChanging">
            <Columns>
                <asp:BoundField DataField="strTipo" HeaderText="Tipo">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="ProcessarEm" HeaderText="Processamento" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:ButtonField Text="<img src='../images/download.gif' title='baixar' alt='baixar' border='0' />" CommandName="arquivo" >
                    <ItemStyle Font-Size="10px" Width="1%" />
                </asp:ButtonField>
                <asp:ButtonField Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                    <ItemStyle Font-Size="10px" Width="1%" />
                </asp:ButtonField>
            </Columns>
        </asp:GridView>
        <br />
    </asp:Panel>
    <table cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1" >
         <tr>
            <td class="tdPrincipal1">Estipulante</td>
            <td class="tdNormal1">
                <asp:ListBox ID="lstEstipulantes" Rows="4" SelectionMode="Multiple" runat="server" SkinID="listBoxSkin" Width="225" />
            </td>
         </tr>
         <tr>
            <td class="tdPrincipal1" width="120">Operadora</td>
            <td class="tdNormal1"><asp:ListBox Rows="4" ID="lstOperadora" SelectionMode="Multiple" SkinID="listBoxSkin" Width="225"  runat="server" /></td>
         </tr>
         <tr>
            <td class="tdPrincipal1" valign="top">Pagamento</td>
            <td class="tdNormal1" valign="top">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            De
                        </td>
                        <td>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDe" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                            <asp:Image SkinID="imgCanlendario" ID="imgDe" runat="server" EnableViewState="false" />
                            <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtDe" PopupButtonID="imgDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        </td>
                    </tr>
                    <tr><td height="8"></td></tr>
                    <tr>
                        <td>
                            Até
                        </td>
                        <td>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtAte" Width="60" EnableViewState="false" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                            <asp:Image SkinID="imgCanlendario" ID="imgAte" runat="server" EnableViewState="false" />
                            <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAte" TargetControlID="txtAte" PopupButtonID="imgAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        </td>
                    </tr>
                </table>
            </td>
         </tr>

         <tr id="trProcessarEm" runat="server" visible="false">
            <td class="tdPrincipal1" width="120">Processar em</td>
            <td class="tdNormal1">
                <asp:TextBox ID="txtProcessarEm" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                <asp:Image SkinID="imgCanlendario" ID="imgProcessarEm" runat="server" EnableViewState="false" />
                <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceProcessarEm" TargetControlID="txtProcessarEm" PopupButtonID="imgProcessarEm" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                &nbsp;
                <asp:DropDownList runat="server" ID="cboHora" Width="40" SkinID="dropdownSkin" />&nbsp;:&nbsp;<asp:DropDownList runat="server" ID="cboMinuto" Width="40" SkinID="dropdownSkin" />
            </td>
         </tr>

         <tr>
            <td class="tdNormal1" align="left">
                <asp:CheckBox ID="chkAgendar" Text="agendar" SkinID="checkboxSkin" runat="server" AutoPostBack="true" oncheckedchanged="chkAgendar_CheckedChanged" />
            </td>
            <td class="tdNormal1" align="right"><asp:Button ID="cmdGerar" OnClick="cmdGerar_Click" SkinID="botaoAzul" Text="Gerar" runat="server" Width="80" />&nbsp;<font color='red'><asp:Literal ID="lblGerarArquivoMessage" runat="server" EnableViewState="false" /></font></td>
         </tr>
    </table>
    <asp:Panel runat="server" ID="pnl" Visible="false">
        <br />
        <asp:ImageButton Visible="true" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" style="width: 16px" />
        <div>
        <asp:GridView ID="grid" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="false" AutoGenerateColumns="False" >
            <Columns>
                <asp:BoundField DataField="usuario_nome" HeaderText="Produtor">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="CompeVencto" HeaderText="Competência">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Filial" HeaderText="Filial">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="operadora_nome" HeaderText="Operadora">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="estipulante_descricao" HeaderText="Estipulante">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="estipulante_carteira" HeaderText="Carteira">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

                <asp:BoundField DataField="contrato_numero" HeaderText="Contrato">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contratobeneficiario_numeroSequencia" HeaderText="Seq.">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="beneficiario_nome" HeaderText="Beneficiário">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="beneficiario_cpf" HeaderText="Beneficiário CPF" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="FaixaEtaria" HeaderText="Faixa Etária" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="IDADE" HeaderText="Idade">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="beneficiario_dataNascimento" HeaderText="Nascimento">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Parentesco" HeaderText="Parentesco">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="contrato_vigencia" HeaderText="Vigência">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="contrato_datacancelamento" HeaderText="Cancelamento">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_dataVencimento" HeaderText="Vencto.">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_dataPagto" HeaderText="Pagto.">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_valor" HeaderText="Valor" DataFormatString="{0:N2}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_valorPagto" HeaderText="Pagto." DataFormatString="{0:N2}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="contrato_codcobranca" HeaderText="Cód. Cobrança" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="cobranca_parcela" HeaderText="Parcela" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="TipoBaixa" HeaderText="Baixa" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="MotivoBaixa" HeaderText="MotivoBaixa" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                
                <asp:BoundField DataField="Tipo_Boleto" HeaderText="Tipo" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="TIPO" HeaderText="Taxa" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="EmpresaCobranca" HeaderText="Empresa" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>

                <asp:BoundField DataField="plano_descricao" HeaderText="Plano" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="Acomodacao" HeaderText="Acomodação" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="StatusBoleto" HeaderText="Status" DataFormatString="{0:N2}" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="ValorDetalhe" HeaderText="Detalhe" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>

                <asp:BoundField DataField="cobranca_dataCriacao" HeaderText="Criação" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Cedente" HeaderText="Cedente" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <%--<asp:BoundField DataField="cobranca_dataPagto" HeaderText="Data Pgto" HeaderStyle-Wrap="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>--%>
            </Columns>
        </asp:GridView>
        </div>
    </asp:Panel>
</asp:Content>
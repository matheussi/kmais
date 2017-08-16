<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="conferenciaRel.aspx.cs" Inherits="www.admin.conferenciaRel" EnableEventValidation="false" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"></td></tr>
        <tr>
            <td><span class="titulo">Relatório de Pré-Cadastro</span></td>
        </tr>
        <tr>
            <td nowrap><span class="subtitulo">Utilize os filtros para visualizar os cadastros realizados</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdToExcel" />
        </Triggers>
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" width="355">
                <tr>
                    <td class="tdPrincipal1">Status</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboStatus" SkinID="dropdownSkin" Width="100%" runat="server" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" valign="top">Operadora</td>
                    <td class="tdNormal1"><asp:ListBox ID="cboOperadora" SelectionMode="Multiple" SkinID="listBoxSkin" Width="100%" runat="server" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Período</td>
                    <td class="tdNormal1">
                        <asp:TextBox runat="server" ID="txtDe" Width="66px" SkinID="textboxSkin" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                        <asp:Image SkinID="imgCanlendario" ID="imgDe" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceDe" TargetControlID="txtDe" PopupButtonID="imgDe" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                         a 
                        <asp:TextBox runat="server" ID="txtAte" Width="66px" SkinID="textboxSkin" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                        <asp:Image SkinID="imgCanlendario" ID="imgAte" runat="server" EnableViewState="false" />
                        <cc1:CalendarExtender TodaysDateFormat="dd/MM/yyyy" FirstDayOfWeek="Monday" ID="ceAte" TargetControlID="txtAte" PopupButtonID="imgAte" runat="server" EnableViewState="false" Format="dd/MM/yyyy" />
                        &nbsp;
                        <asp:Button ID="cmdLocalizar" Text="Localizar" SkinID="botaoAzulBorda" runat="server" onclick="cmdLocalizar_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:ImageButton Visible="false" ImageUrl="~/images/excel.png" ToolTip="exportar para o excel" ImageAlign="AbsBottom" BorderWidth="0" runat="server" ID="cmdToExcel" OnClick="cmdToExcel_Click" />&nbsp;<asp:Literal ID="litCabecalho" runat="server" />
            <asp:GridView ID="grid" Width="100%" SkinID="gridViewSkin" PageSize="100" runat="server" AllowPaging="false" AutoGenerateColumns="False"  
                DataKeyNames="ID" OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" OnPageIndexChanging="grid_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="FilialNome" HeaderText="Filial">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="EstipulanteNome" HeaderText="Estipulante">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Status" HeaderText="Status">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ContratoNumero" HeaderText="Proposta">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TitularNome" HeaderText="Nome Titular">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TitularCpf" HeaderText="CPF Titular">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TitularIdade" HeaderText="Idade Titular">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ValorFinal" HeaderText="Valor" DataFormatString="{0:C}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Admissao" HeaderText="Admissão" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="QtdVidas" HeaderText="Vidas">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Data" HeaderText="Inclusão" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
            <asp:Literal ID="litAviso" runat="server" EnableViewState="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
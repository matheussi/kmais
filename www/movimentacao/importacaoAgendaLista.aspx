<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="importacaoAgendaLista.aspx.cs" Inherits="www.movimentacao.importacaoAgendaLista" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Importação de propostas</span></td></tr>
        <tr><td><span class="subtitulo">Importação de propostas</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel ID="up" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="680px" cellpadding="2" cellspacing="0" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1">
                        <font color='yellow'>Status:</font>
                    </td>
                    <td class="tdPrincipal1">
                        <asp:RadioButton ID="optProcessadas" Text="Importações realizadas" runat="server" GroupName="statusGroup" Checked="true" />&nbsp;
                        <asp:RadioButton ID="optPendentes" Text="Importações pendentes" runat="server" GroupName="statusGroup" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">
                        <font color='yellow'>Período:</font>
                    </td>
                    <td class="tdPrincipal1">
                        de  <asp:TextBox SkinID="textboxSkin" runat="server" id="txtDe" Width="66px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        até <asp:TextBox SkinID="textboxSkin" runat="server" id="txtAte" Width="66px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />&nbsp;&nbsp;
                        <asp:Button ID="cmdFiltrar" Text="Filtrar" SkinID="botaoAzulBorda" runat="server" onclick="cmdFiltrar_Click" />
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlFirstResult" runat="server">
                <asp:GridView ID="gridItens" Width="680px" SkinID="gridViewSkin" 
                    runat="server" AllowPaging="True" AutoGenerateColumns="False"  
                    DataKeyNames="ID,Processado" onrowcommand="gridItens_RowCommand" 
                    onrowdatabound="gridItens_RowDataBound" 
                    OnPageIndexChanging="gridItens_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Arquivo" HeaderText="Arquivo">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ProcessarEm" DataFormatString="{0: dd/MM/yyyy HH:mm}" HeaderText="Processar em">
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ProcessadoData" HeaderText="Processado em" DataFormatString="{0: dd/MM/yyyy HH:mm}" >
                            <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        </asp:BoundField>
                        <asp:ButtonField Text="<img src='../images/rascunho.png' title='crítica' alt='excluir' border='0' />" CommandName="critica" >
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                        <asp:ButtonField Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                        <asp:ButtonField Text="<img src='../images/download.gif' title='baixar arquivo' alt='editar' border='0' />" CommandName="baixar" >
                            <ItemStyle Font-Size="10px" Width="1%" />
                        </asp:ButtonField>
                        <asp:ButtonField Text="<img src='../images/alert16.png' title='erro' alt='editar' border='0' />" CommandName="erro" >
                            <ItemStyle Font-Size="10px" Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
            <asp:Panel ID="pnlCritica" runat="server" Visible="false" EnableViewState="false">
                <asp:GridView ID="gridCritica" Width="680px" SkinID="gridViewSkin" Font-Size="11px" 
                    runat="server" AllowPaging="false" AutoGenerateColumns="False"  
                    DataKeyNames="ID">
                    <Columns>
                        <asp:BoundField DataField="Motivo" HeaderText="Motivo">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NumeroProposta" HeaderText="Proposta">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
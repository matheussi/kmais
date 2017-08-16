<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="arquivoAvisoCobranca.aspx.cs" Inherits="www.arquivoAvisoCobranca" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Arquivos de aviso de cobrança</span></td></tr>
        <tr><td><span class="subtitulo">Gerar arquivos de aviso de cobrança</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table width="420px" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                <tr>
                    <td width="76" class="tdPrincipal1">Tipo</td>
                    <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboTipo" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="cboTipo_Changed" /></td> 
                </tr>
                <tr>
                    <td width="76" class="tdPrincipal1">Operadoras</td>
                    <td class="tdNormal1"><asp:ListBox SelectionMode="Multiple" SkinID="listBoxSkin" runat="server" ID="lstOperadoras" Width="100%" ToolTip="Você pode utilizar a tecla CTRL para desmarcar uma ou mais operadoras" /></td>
                </tr>
                <tr>
                    <td width="76" class="tdPrincipal1">Vencimento</td>
                    <td class="tdNormal1"><b>Mês</b>&nbsp;<asp:DropDownList ID="cboMes" Width="45" SkinID="dropdownSkin" runat="server" />&nbsp;&nbsp;&nbsp;<b>Ano</b>&nbsp;<asp:DropDownList ID="cboAno" Width="55" SkinID="dropdownSkin" runat="server" />&nbsp;&nbsp;&nbsp;<asp:Button ID="cmdGerar" Text="Gerar arquivo" Width="100" SkinID="botaoAzulBorda" runat="server" OnClick="cmdGerar_Click" OnClientClick="return confirm('Deseja realmente prosseguir?');" />&nbsp;</td>
                </tr>
            </table>
            <br />
            <asp:Literal ID="litMsg" runat="server" EnableViewState="true" />
            <asp:Panel ID="pnlResultado" runat="server" Visible="false">
                <asp:GridView ID="grid" Width="420px" SkinID="gridViewSkin" 
                    runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ArquivoAvisoID,OperadoraID"
                    OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" 
                    OnRowCreated="grid_RowCreated">
                    <Columns>
                        <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                         <asp:TemplateField>
                            <ItemStyle Width="1%" />
                            <ItemTemplate>
                                <asp:ImageButton CommandName="baixar" ID="imbDownloadArquivo" AlternateText="Baixar" ToolTip="Download" ImageUrl="~/images/download.gif" runat="server" />
                            </ItemTemplate>
                         </asp:TemplateField>
                         <asp:ButtonField CommandName="setarComoEnviado" Text="<img src='images/tick.png' title='Setar como enviado' alt='Setar como enviado' border='0' />">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
            <asp:Panel ID="pnlResultadoCobrancaDupla" runat="server" Visible="false">
                <asp:GridView ID="grid2" Width="420px" SkinID="gridViewSkin" 
                    runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ArquivoID,OperadoraID"
                    OnRowCommand="grid2_RowCommand" OnRowDataBound="grid2_RowDataBound" 
                    onrowcreated="grid2_RowCreated">
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
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
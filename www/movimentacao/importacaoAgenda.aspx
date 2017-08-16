<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="importacaoAgenda.aspx.cs" Inherits="www.movimentacao.importacaoAgenda" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td><span class="titulo">Importação de propostas</span></td></tr>
        <tr><td><span class="subtitulo">Importação de propostas - Agendamento</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel ID="up" runat="server" UpdateMode="Always">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdLer" />
        </Triggers>
        <ContentTemplate>
            <asp:Panel ID="pnlEnviar" runat="server">
                <table width="550px" cellpadding="2" cellspacing="0" style="border: solid 1px #507CD1">
                    <tr>
                        <td class="tdNormal1" width="90">Descrição</td>
                        <td class="tdNormal1" colspan="2"><asp:TextBox ID="txtDescricao" MaxLength="149" runat="server" SkinID="textboxSkin" Width="300" /></td>
                    </tr>
                    <tr>
                        <td class="tdNormal1">Processar em</td>
                        <td class="tdNormal1" colspan="2">
                            <asp:TextBox ID="txtProcessarEm" Width="63px" runat="server" SkinID="textboxSkin"  onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                            &nbsp;
                            <asp:DropDownList runat="server" ID="cboHora" Width="40" SkinID="dropdownSkin" />&nbsp;:&nbsp;<asp:DropDownList runat="server" ID="cboMinuto" Width="40" SkinID="dropdownSkin" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" >Arquivo</td>
                        <td class="tdNormal1">
                            <asp:FileUpload ID="uplFile" Width="100%" runat="server" />
                        </td>
                        <td class="tdNormal1" width="50">
                            <asp:Button ID="cmdLer" Width="70" runat="server" Text="Salvar" SkinID="botaoAzulBorda" OnClick="cmdLer_Click" />
                        </td>
                    </tr>
                </table>
                <br />
            </asp:Panel>

            <asp:Panel ID="pnlDetalhe" runat="server" Visible="false">

                <asp:Panel ID="pnlDetalhe2" runat="server" Visible="false">
                    <table cellpadding="0" cellspacing="0" style="border: solid 1px #507CD1" Width="750px">
                        <tr>
                            <td>
                                <asp:GridView ID="grid" Width="750px" SkinID="gridViewSkin" 
                                    runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="CobrancaID"
                                    OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="NossoNumero" HeaderText="Nosso Núm.">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Parcela" HeaderText="Parc">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="DataVencto" HeaderText="Vencimento" DataFormatString="{0:dd/MM/yyyy}">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:N2}">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="PropostaNumero" HeaderText="Contrato">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Status" HeaderText="Status">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                        <asp:ButtonField Visible="false" Text="<img src='../images/tick.png' title='gerar cobrança' alt='gerar cobrança' border='0' />" CommandName="gerar">
                                            <ItemStyle Width="1%" />
                                        </asp:ButtonField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>


            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>  
</asp:Content>
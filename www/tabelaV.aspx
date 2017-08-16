<%@ Page Theme="Theme"Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="tabelaV.aspx.cs" Inherits="www.tabelaV" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"><img hspace="2" src="images/imgTitulos/tabela_valores.png" /></td></tr>
        <tr>
            <td><span class="titulo">Tabela de Valores</span></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" width="515">
                <tr>
                    <td class="tdPrincipal1">Categoria</td>
                    <td class="tdNormal1"><asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboCategoria" Width="100%" /> </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Data</td>
                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" Width="57px" runat="server" ID="txtData" MaxLength="10" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="95px">Operadora</td>
                    <td class="tdNormal1"><asp:DropDownList AutoPostBack="true" runat="server" SkinID="dropdownSkin" ID="cboOperadora" Width="100%" onselectedindexchanged="cboOperadora_SelectedIndexChanged" /> </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Contrato</td>
                    <td class="tdNormal1"><asp:DropDownList AutoPostBack="true" runat="server" SkinID="dropdownSkin" ID="cboContrato" Width="100%" onselectedindexchanged="cboContrato_SelectedIndexChanged" /> </td>
                </tr>
<%--                <tr>
                    <td class="tdPrincipal1" width="58px">Descrição</td>
                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" Width="240px" runat="server" ID="txtNome" MaxLength="180" /></td>
                </tr>--%>
            </table>
            <br />
            <asp:Panel ID="pnl" runat="server" Visible="false">
                <asp:GridView ID="gridPlanos" SkinID="gridViewSkin" width="515" runat="server" AutoGenerateColumns="False" DataKeyNames="ID" OnRowCommand="gridPlanos_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="Descricao" HeaderText="Plano">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:ButtonField Text="<img src='../images/detail2.png' alt='detalhes' border='0' />" CommandName="detalhe" >
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
                <asp:Panel ID="pnl2" runat="server" Visible="false">
                    <table cellpadding="0" cellspacing="0" width="515">
                        <tr>
                            <td>
                                <asp:GridView ID="gridItens" SkinID="gridViewSkin" Width="100%" runat="server" 
                                    AutoGenerateColumns="False" DataKeyNames="ID" 
                                    onrowdatabound="gridItens_RowDataBound" onrowcommand="gridItens_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Idade início">
                                            <ItemTemplate>
                                                <asp:TextBox CssClass="textbox" Width="20" ID="txtIdadeInicio" runat="server" Text='<%# Bind("IdadeInicio") %>' />
                                                <cc1:MaskedEditExtender Mask="99" MaskType="Number" runat="server" ID="meeIdadeInicio" TargetControlID="txtIdadeInicio" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField HeaderText="Idade fim">
                                            <ItemTemplate>
                                                <asp:TextBox CssClass="textbox" Width="20" ID="txtIdadeFim" runat="server" Text='<%# Bind("IdadeFim") %>' />
                                                <cc1:MaskedEditExtender Mask="99" MaskType="Number" runat="server" ID="meeIdadeFim" TargetControlID="txtIdadeFim" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField HeaderText="QC Operadora">
                                            <ItemTemplate>
                                                <asp:TextBox CssClass="textbox" Width="40" ID="txtQCPg" runat="server" Text='<%# Bind("QCValorPagamento") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="QP Operadora">
                                            <ItemTemplate>
                                                <asp:TextBox CssClass="textbox" Width="40" ID="txtQPPg" runat="server" Text='<%# Bind("QPValorPagamento") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="QC Cliente">
                                            <ItemTemplate>
                                                <asp:TextBox CssClass="textbox" Width="40" ID="txtQC" runat="server" Text='<%# Bind("QCValor") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="QP Cliente">
                                            <ItemTemplate>
                                                <asp:TextBox CssClass="textbox" Width="40" ID="txtQP" runat="server" Text='<%# Bind("QPValor") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>

                                        <asp:ButtonField Text="<img src='images/delete.png' alt='excluir' border='0' />" CommandName="excluir">
                                            <ItemStyle Width="1%" HorizontalAlign="Center" />
                                            <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                        </asp:ButtonField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" height="27" class="tdPrincipal1">
                                <table cellpadding="2" cellspacing="0">
                                    <tr>
                                        <td>
                                            <span style="height:10px; border:solid 1px #EFF3FB">
                                                <asp:Button Width="147" style="cursor: pointer;" runat="server" ID="cmdAddItem" SkinID="botaoAzul" Text="adicionar item à tabela" BorderWidth="0px" OnClick="cmdAddItem_Click" />&nbsp;
                                            </span>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </asp:Panel>
            <br />
            <asp:Panel ID="pnlTaxas" runat="server" Visible="false">
                <asp:GridView ID="gridTaxas" SkinID="gridViewSkin" width="515" runat="server" AutoGenerateColumns="False" DataKeyNames="ID">
                    <Columns>
                        <asp:BoundField DataField="Descricao" HeaderText="Plano">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:ButtonField Text="<img src='../images/detail2.png' alt='detalhes' border='0' />" CommandName="detalhe" >
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
                <br />
            </asp:Panel>
            <table cellpadding="2" width="515">
                <tr>
                    <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
                    <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="comissionamentoOperadora.aspx.cs" Inherits="www.comissionamentoOperadora" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"><img hspace="2" src="images/imgTitulos/tabela_valores.png" alt="" /></td></tr>
        <tr><td><span class="titulo">Tabela de Comissionamento</span></td></tr>
        <tr><td><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1" width="395" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1" width="90">Descrição</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" ID="txtDescricao" Width="98%" MaxLength="200" SkinID="textboxSkin" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Data</td>
                    <td class="tdNormal1">
                        <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtData" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                        &nbsp;&nbsp;&nbsp;<asp:CheckBox runat="server" ID="chkAtivo" Text="Tabela de comissionamento ativa" SkinID="checkboxSkin" Checked="true" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:Panel ID="pnl" runat="server" Visible="false">
                <table cellpadding="2" cellspacing="1" width="395" style="border: solid 1px #507CD1">
                    <tr>
                        <td class="tdPrincipal1" width="90">Operadora</td>
                        <td class="tdNormal1"><asp:DropDownList ID="cboOperadoras" Width="260" SkinID="dropdownSkin" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboOperadoras_Change" /></td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1" width="90">Contrato</td>
                        <td class="tdNormal1"><asp:DropDownList ID="cboContratos" Width="260" SkinID="dropdownSkin" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboContratos_Change" /></td>
                    </tr>
                </table>
                <br />
                <asp:GridView ID="gridItens" SkinID="gridViewSkin" Width="395" runat="server" 
                    AutoGenerateColumns="False" DataKeyNames="ID" 
                    onrowdatabound="gridComissionamentoItensDetalhe_RowDataBound" onrowcommand="gridComissionamentoItensDetalhe_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="Parcela">
                            <ItemTemplate>
                                <asp:TextBox CssClass="textbox" Width="20" ID="txtParcela" runat="server" Text='<%# Bind("Parcela") %>' />
                                <cc1:MaskedEditExtender Mask="99" runat="server" ID="meeParcela" TargetControlID="txtParcela" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Percentual">
                            <ItemTemplate>
                                <asp:TextBox CssClass="textbox" Width="40" ID="txtPercentual" runat="server" Text='<%# Bind("Percentual") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Perc. Carência">
                            <ItemTemplate>
                                <asp:TextBox CssClass="textbox" Width="40" ID="txtPercentualCompraCarencia" runat="server" Text='<%# Bind("PercentualCompraCarencia") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Perc. Migração">
                            <ItemTemplate>
                                <asp:TextBox CssClass="textbox" Width="40" ID="txtPercentualMigracao" runat="server" Text='<%# Bind("PercentualMigracao") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField Visible="false" HeaderText="Perc. ADM">
                            <ItemTemplate>
                                <asp:TextBox CssClass="textbox" Width="40" ID="txtPercentualADM" runat="server" Text='<%# Bind("PercentualADM") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Perc. Especial">
                            <ItemTemplate>
                                <asp:TextBox CssClass="textbox" Width="40" ID="txtPercentualEspecial" runat="server" Text='<%# Bind("PercentualEspecial") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Idade" Visible="false">
                            <ItemTemplate>
                                <asp:TextBox CssClass="textbox" Width="40" ID="txtIdade" runat="server" Text='<%# Bind("Idade") %>' />
                                <cc1:MaskedEditExtender Mask="99" MaskType="Number" runat="server" ID="meeIdade" TargetControlID="txtIdade" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:ButtonField Text="<img src='images/delete.png' alt='excluir' border='0' />" CommandName="excluir">
                            <ItemStyle Width="1%" HorizontalAlign="Center" />
                            <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                        </asp:ButtonField>

                    </Columns>
                </asp:GridView>
                <table cellpadding="2" cellspacing="0" width="395">
                    <tr>
                        <td align="right" height="27" class="tdPrincipal1"><span style="height:10px; border:solid 1px #EFF3FB"><asp:Button Width="147" EnableViewState="false" runat="server" ID="cmdAddItemCom" style="cursor: pointer;"  SkinID="botaoAzul" Text="adicionar item à tabela" BorderWidth="0px" OnClick="cmdAddItemCom_Click" /></span></td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <table cellpadding="2" width="395">
                <tr>
                    <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
                    <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

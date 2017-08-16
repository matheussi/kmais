<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="tabelaR.aspx.cs" Inherits="www.tabelaR" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"><img src="images/imgTitulos/tabela_reajuste.png" /></td></tr>
        <tr>
            <td><span class="titulo">Tabela de reajuste</span></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <table cellpadding="2" width="300">
        <tr>
            <td class="tdPrincipal1">Operadora</td>
            <td class="tdNormal1"><asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboOperadora" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="cboOperadora_OnSelectedIndexChanged" /> </td>
        </tr>
        <tr>
            <td class="tdPrincipal1">Contrato</td>
            <td class="tdNormal1"><asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboContrato" Width="100%" /> </td>
        </tr>
        <tr>
            <td class="tdPrincipal1" width="58px">Descrição</td>
            <td class="tdNormal1"><asp:TextBox CssClass="textbox" Width="240px" runat="server" ID="txtNome" MaxLength="180" /></td>
        </tr>
        <tr>
            <td class="tdPrincipal1">Data</td>
            <td class="tdNormal1"><asp:TextBox CssClass="textbox" Width="57px" runat="server" ID="txtData" MaxLength="10" /></td>
        </tr>
    </table>
    <table width="310">
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <asp:GridView ID="gridItens" SkinID="gridViewSkin" Width="100%" runat="server" 
                                AutoGenerateColumns="False" onrowcreated="gridItens_RowCreated" DataKeyNames="ID" 
                                onrowdatabound="gridItens_RowDataBound" onrowcommand="gridItens_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Idade início">
                                        <ItemTemplate>
                                            <asp:TextBox CssClass="textbox" Width="20" ID="txtIdadeInicio" runat="server" Text='<%# Bind("IdadeInicio") %>' />
                                            <cc1:MaskedEditExtender Mask="99" runat="server" ID="meeIdadeInicio" TargetControlID="txtIdadeInicio" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Reajuste (%)">
                                        <ItemTemplate>
                                            <asp:TextBox CssClass="textbox" Width="40" ID="txtReajuste" runat="server" Text='<%# Bind("PercentualReajuste") %>' />
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
                        <td align="right" class="tdPrincipal1"><asp:Button Width="147" runat="server" ID="cmdAddItem" SkinID="botaoAzul" Text="adicionar item à tabela" BorderWidth="0px" OnClick="cmdAddItem_Click" /></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="2" width="306">
        <tr>
            <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
            <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
        </tr>
    </table>
</asp:Content>
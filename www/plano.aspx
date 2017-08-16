<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="plano.aspx.cs" Inherits="www.plano" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"><img height="48" src="images/imgTitulos/planos.png" /></td></tr>
        <tr>
            <td><span class="titulo">Plano</span></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
<ContentTemplate>

    <cc1:TabContainer BorderStyle="None" BorderWidth="0" Width="100%" ID="tab" runat="server" ActiveTabIndex="0" >
        <cc1:TabPanel runat="server" ID="p1">
            <HeaderTemplate><font color="black">Dados comuns</font></HeaderTemplate>
            <ContentTemplate>
                <table cellpadding="2" width="300">
                    <tr>
                        <td class="tdPrincipal1">Operadora</td>
                        <td class="tdNormal1"><asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboOperadora" AutoPostBack="true" OnSelectedIndexChanged="cboOperadora_OnSelectedIndexChanged" Width="100%" /> </td>
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
                        <td class="tdPrincipal1" width="58px">Código</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" Width="80px" runat="server" ID="txtCodigo" MaxLength="50" /></td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1" width="58px">Sub-plano</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" Width="80px" runat="server" ID="txtSubPlano" MaxLength="50" /></td>
                    </tr>
                    <tr>
                        <td colspan="2"><asp:CheckBox runat="server" SkinID="checkboxSkin" ID="chkAtivo" Checked="true" Text="Plano ativo" /></td>
                    </tr>
                </table>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel runat="server" ID="p2">
            <HeaderTemplate><font color="black">Agregados</font></HeaderTemplate>
            <ContentTemplate>
                <span class="subtitulo">Adicione os agregados permitidos neste plano</span><br />
                <table cellpadding="2" width="300">
                    <tr>
                        <td class="tdPrincipal1" width="75">Agregado</td>
                        <td class="tdNormal1"><asp:DropDownList Width="230px" runat="server" ID="cboParentesco" SkinID="dropdownSkin" /></td>
                    </tr>
                    <tr>
                        <td colspan="2" align="right"><asp:Button EnableViewState="False" runat="server" OnClick="cmdAdd_Click" ID="cmdAdd" Text="Adicionar" SkinID="botaoPequeno" /></td>
                    </tr>
                </table><br />
                <span runat="server" visible="False" id="spanParentescosPermitidos" class="subtitulo">Agregados permitidos</span>
                <asp:GridView AutoGenerateColumns="False" ID="gridPPA" SkinID="gridViewSkin" Width="300px" runat="server" DataKeyNames="ID" onrowcommand="gridPPA_RowCommand" onrowdatabound="gridPPA_RowDataBound">
                    <Columns>
                        <asp:BoundField  DataField="ParentescoDescricao" HeaderText="Parentesco">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' alt='excluir' border='0' />">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel runat="server" ID="p3">
            <HeaderTemplate><font color="black">Dependentes</font></HeaderTemplate>
            <ContentTemplate>
            <span class="subtitulo">Adicione os dependentes permitidos neste plano</span><br />
            <table cellpadding="2" width="300">
                    <tr>
                        <td class="tdPrincipal1" width="75">Dependente</td>
                        <td class="tdNormal1"><asp:DropDownList Width="230px" runat="server" ID="cboDependente" SkinID="dropdownSkin" /></td>
                    </tr>
                    <tr>
                        <td colspan="2" align="right"><asp:Button EnableViewState="False" runat="server" OnClick="cmdAddDependente_Click" ID="cmdAddDependente" Text="Adicionar" SkinID="botaoPequeno" /></td>
                    </tr>
                </table><br />
                <span runat="server" visible="False" id="spanDependentesPermitidos" class="subtitulo">Dependentes permitidos</span>
                <asp:GridView AutoGenerateColumns="False" ID="gridDependentes" SkinID="gridViewSkin" Width="300px" runat="server" DataKeyNames="ID" onrowcommand="gridDependentes_RowCommand" onrowdatabound="gridDependentes_RowDataBound">
                    <Columns>
                        <asp:BoundField  DataField="ParentescoDescricao" HeaderText="Dependente">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:ButtonField CommandName="excluir" Text="<img src='images/delete.png' alt='excluir' border='0' />">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
                </ContentTemplate>
        </cc1:TabPanel>
    </cc1:TabContainer>
    <br />
    <table cellpadding="2" width="306">
        <tr>
            <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
            <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
        </tr>
    </table>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
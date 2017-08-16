<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="filial.aspx.cs" Inherits="www.filial" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3"><img src="images/icones/filiais.png" /></td></tr>
        <tr><td><span class="titulo">Filial</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Preencha os campos abaixo e clique em salvar</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="upDadosComuns" UpdateMode="Conditional">
    <ContentTemplate>
    <table width="40%" cellpadding="1">
        <tr>
            <td width="69" class="tdPrincipal1">&nbsp;Nome</td>
            <td class="tdNormal1"><asp:TextBox runat="server" SkinID="textboxSkin" MaxLength="149" ID="txtNome" Width="98%" /></td>
        </tr>
        <tr>
            <td width="69" class="tdPrincipal1">&nbsp;E-mail</td>
            <td class="tdNormal1"><asp:TextBox runat="server" SkinID="textboxSkin" MaxLength="69" ID="txtEmail" Width="98%" /></td>
        </tr>
        <tr>
            <td width="69" class="tdPrincipal1">&nbsp;Telefone</td>
            <td class="tdNormal1">
                <asp:TextBox runat="server" SkinID="textboxSkin" MaxLength="15" ID="txtFone" Width="75" />
                <cc1:MaskedEditExtender EnableViewState="false" TargetControlID="txtFone" Mask="(99) 9999-9999" runat="server" ID="meetxtFone" />
            </td>
        </tr>
        <tr>
            <td class="tdNormal1" colspan="2">
                <asp:CheckBox id="chkAtiva" Checked="true" Text="Ativa" runat="Server" />
            </td>
        </tr>
    </table>
    <br />
    <table width="40%" cellpadding="1">
        <tr>
            <td class="tdPrincipal1" width="59px">&nbsp;CEP</td>
            <td class="tdNormal1" colspan="3">
                <asp:TextBox CssClass="textbox" runat="server" ID="txtCEP" Width="65px" MaxLength="9" />
                <asp:ImageButton runat="server" EnableViewState="false" ToolTip="checar CEP" ImageUrl="~/images/endereco.png" ID="cmdBuscaEndereco" OnClick="cmdBuscaEndereco_Click" />
                <cc1:MaskedEditExtender TargetControlID="txtCEP" Mask="99999-999" 
                    runat="server" ID="meeCEP" CultureAMPMPlaceholder="" 
                    CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                    CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                    CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
            </td>
        </tr>
        <tr>
            <td class="tdPrincipal1">&nbsp;Logradouro</td>
            <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtLogradouro" Width="290px" MaxLength="300" /></td>
            <td class="tdPrincipal1">&nbsp;Número</td>
            <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtNumero" Width="65px" MaxLength="9" /></td>
        </tr>
        <tr>
            <td class="tdPrincipal1">&nbsp;Complemento</td>
            <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtComplemento" Width="200px" MaxLength="250" /></td>
            <td class="tdPrincipal1" width="72px">&nbsp;Bairro</td>
            <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtBairro" Width="190px" MaxLength="300" /></td>
        </tr>
        <tr>
            <td class="tdPrincipal1">&nbsp;Cidade</td>
            <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtCidade" Width="200px" MaxLength="300" /></td>
            <td class="tdPrincipal1">&nbsp;UF</td>
            <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtUF" Width="20px" MaxLength="2" /></td>
        </tr>
    </table>
    <br />
    <table width="73%" cellpadding="1">
        <tr>
            <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" Text="Voltar" /></td>
            <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" Text="Salvar" /></td>
        </tr>
    </table>
    </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel runat="server" ID="upnlAlerta" UpdateMode="Conditional" EnableViewState="false">
        <ContentTemplate>
            <cc1:ModalPopupExtender ID="MPE" runat="server" EnableViewState="false"
                TargetControlID="lnk"
                PopupControlID="pnlAlert"
                BackgroundCssClass="modalBackground" 
                CancelControlID="cmdCloseAlert"
                DropShadow="true"  />
            <asp:Panel runat="server" ID="pnlAlert" EnableViewState="false">
                <asp:LinkButton runat="server" EnableViewState="false" ID="lnk" />
                <table width="350" align="center" bgcolor="gainsboro" style="border:solid 1px black">
                    <tr>
                        <td align="center">
                            <asp:Literal runat="server" ID="litAlert" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr height="8"><td height="8">&nbsp</td></tr>
                    <tr>
                        <td align="center">
                            <input runat="server" style="width:45px;font-size:12px;font-family:Arial" id="cmdCloseAlert" type="button" value="OK" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
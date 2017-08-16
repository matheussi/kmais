<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="almoxTipoProd.aspx.cs" Inherits="www.admin.almoxTipoProd" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img src="../images/imgTitulos/produtos.png" /></td></tr>
        <tr><td><span class="titulo">Almoxarifado - Produto</span></td></tr>
        <tr><td><span class="subtitulo">Preencha os campos abaixo e clique em salvar</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
    <ContentTemplate>
    <table width="630" style="border: solid 1px #507CD1">
        <tr>
            <td width="60" class="tdPrincipal1">&nbsp;<b>Tipo</b></td>
            <td width="220"><asp:DropDownList SkinID="dropdownSkin" Width="217px" ID="cboTipo" runat="server" AutoPostBack="True" onselectedindexchanged="cboTipo_SelectedIndexChanged" /></td>
            <td width="17" class="tdPrincipal1"><b>ou</b></td>
            <td><asp:TextBox ID="txtTipo" MaxLength="90" SkinID="textboxSkin" runat="server" /></td>
        </tr>
        <tr>
            <td></td>
            <td colspan="3"><asp:CheckBox Font-Bold="true" SkinID="checkboxSkin" ID="chkControlado" Text="controlado por numeração" runat="server" /></td>
        </tr>
        <tr><td height="8" colspan="4"></td></tr>
        <tr>
            <td class="tdPrincipal1">&nbsp;<b>Filial</b></td>
            <td colspan="3"><asp:DropDownList ID="cboFilial" Width="215px" MaxLength="90" SkinID="dropdownSkin" runat="server" /></td>
        </tr>
        <tr>
            <td class="tdPrincipal1">&nbsp;<b>Operadora</b></td>
            <td colspan="3"><asp:DropDownList ID="cboOperadora" Width="215px" MaxLength="90" SkinID="dropdownSkin" runat="server" /></td>
        </tr>
        <%--<tr>
            <td class="tdPrincipal1">&nbsp;<b>Descrição</b></td>
            <td colspan="3"><asp:TextBox ID="txtDescricao" Width="210px" MaxLength="90" SkinID="textboxSkin" runat="server" /></td>
        </tr>--%>
        <tr><td height="8" colspan="4"></td></tr>
        <tr>
            <td colspan="4">
                <table cellspacing="0" cellpadding="2">
                    <tr>
                        <td width="59" class="tdPrincipal1">&nbsp;<b>Qtd. Mín.</b></td>
                        <td width="43">
                            <asp:TextBox ID="txtQtdMin" Width="30" MaxLength="10" SkinID="textboxSkin" runat="server" />
                            <cc1:MaskedEditExtender Mask="99999" runat="server" ID="meeQtdMin" TargetControlID="txtQtdMin" />
                        </td>
                        <td width="33" class="tdPrincipal1"><b><font color='yellow'>&nbsp;Qtd.</font></b></td>
                        <td><asp:TextBox ID="txtQtd" Width="30" BackColor="lightgray" ReadOnly="true" MaxLength="10" SkinID="textboxSkin" runat="server" /></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="2" width="430">
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
                DropShadow="true" />
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
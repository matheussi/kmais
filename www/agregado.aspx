<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="agregado.aspx.cs" Inherits="www.agregado" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="474px">
        <tr><td rowspan="3"><img src="images/imgTitulos/regras_comercializacao.png" /></td></tr>
        <tr>
            <td><span class="titulo">Agregados e dependentes</span></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Informe os dados abaixo e clique em Salvar</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
    <ContentTemplate>
       <table cellpadding="2" width="474px">
            <tr>
                <td class="tdNormal1" width="237px">Estipulante</td>
                <td class="tdNormal1" width="237px">Operadora</td>
            </tr>
            <tr>
                <td><asp:DropDownList ID="cboEstipulante" SkinID="dropdownSkin" runat="server" Width="99%" AutoPostBack="true" onselectedindexchanged="cboEstipulante_SelectedIndexChanged" /></td>
                <td>
                     <asp:DropDownList ID="cboOperadora" SkinID="dropdownSkin" runat="server" Width="99%" AutoPostBack="true" onselectedindexchanged="cboOperadora_SelectedIndexChanged" />
                </td>
            </tr>
            <tr>
                <td class="tdNormal1">Contrato</td>
                <td class="tdNormal1">Tipo</td>
            </tr>
            <tr>
                <td><asp:DropDownList ID="cboContrato" ForeColor="Red" SkinID="dropdownSkin" runat="server" Width="99%" /></td>
                <td>
                    <asp:RadioButton ID="optAgregado" runat="server" GroupName="a" Text="Agregado" Checked="true"  />
                    &nbsp;
                    <asp:RadioButton ID="optDependente" runat="server" GroupName="a" Text="Dependente" />
                </td>
            </tr>
            <tr>
                <td colspan="1" class="tdNormal1">Descrição</td>
                <td colspan="1" class="tdNormal1">Código</td>
            </tr>
            <tr>
                <td><asp:TextBox ID="txtDescricao" SkinID="textboxSkin" Width="237px" MaxLength="200" runat="server" /></td>
                <td><asp:TextBox ID="txtCodigo"    SkinID="textboxSkin" Width="99%"   MaxLength="80" runat="server" /></td>
            </tr>
        </table>
        <br />
        <table cellpadding="2" width="474px">
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
<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="declaracaoSaudeRegras.aspx.cs" Inherits="www.declaracaoSaudeRegras" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="474px">
        <tr><td rowspan="3"><img src="images/imgTitulos/regras_comercializacao.png" /></td></tr>
        <tr>
            <td><span class="titulo">Regras para a declaração de saúde</span></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Selecione a operadora para exibir os itens da ficha de declaração de saúde</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
<ContentTemplate>
    <table cellpadding="2" width="474px">
        <tr>
            <td class="tdNormal1">Operadora</td>
        </tr>
        <tr>
            <td><asp:DropDownList ID="cboOperadora" 
                    SkinID="dropdownSkin" runat="server" Width="99%" AutoPostBack="true" 
                    onselectedindexchanged="cboOperadora_SelectedIndexChanged" /> </td>
        </tr>
        <tr height="6"><td height="6"></td></tr>
        <tr>
            <td class="tdNormal1">Descrição da regra</td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtDescricao" SkinID="textboxSkin" 
                    MaxLength="77" runat="server" Width="99%" /></td>
        </tr>
        <tr height="6"><td height="6"></td></tr>
        <tr>
            <td class="tdNormal1">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td width="100">
                            <asp:CheckBox Font-Bold="true" 
                                Text="checar idade" SkinID="checkboxSkin" runat="server" ID="chkIdade" /></td>
                        <td width="127">
                            <asp:DropDownList ID="cboIdadeOperador" runat="server" Width="125">
                                <asp:ListItem Text="maior que" Value="+" Selected="True" />
                                <asp:ListItem Text="maior ou igual a" Value="+=" />
                                <asp:ListItem Text="igual a" Value="=" />
                                <asp:ListItem Text="menor ou igual a" Value="-=" />
                                <asp:ListItem Text="menor que" Value="-" />
                            </asp:DropDownList>
                        </td>
                        <td width="28">
                            <asp:TextBox ID="txtIdade" runat="server" SkinID="textboxSkin" Width="20" />
                        </td>
                        <td width="100">
                            <asp:CheckBox ID="chkSexo" Text="e sexo igual a" runat="server" 
                                SkinID="checkboxSkin" />
                        </td>
                        <td>
                            <asp:DropDownList ID="cboSexo" SkinID="dropdownSkin" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr height="6"><td height="6"></td></tr>
        <tr>
            <td class="tdNormal1">Para a(s) questão(ões) selecionada(s) logo abaixo:</td>
        </tr>
        <tr>
            <td>
                <div id="rolagem" 
                    
                    style="width:500; height:110; position:relative;margin: 0px 0px 0px 0px;overflow: auto;">
                    <asp:CheckBoxList runat="server" ID="checks" Width="474px" />
                </div>
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="2" width="474px">
        <tr>
            <td align="left">
                <asp:Button OnClick="cmdVoltar_Click" 
                    SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltar" 
                    Text="Voltar" /></td>
            <td align="right">
                <asp:Button OnClick="cmdSalvar_Click" 
                    SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdSalvar" 
                    Text="Salvar" /></td>
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
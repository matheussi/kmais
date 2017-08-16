<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="declaracaoSaudeRegrasLista.aspx.cs" Inherits="www.declaracaoSaudeRegrasLista" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="474px">
        <tr><td rowspan="3"><img src="images/imgTitulos/regras_comercializacao.png" /></td></tr>
        <tr>
            <td><span class="titulo">Regras de comercialização</span></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Selecione a operadora para exibir as regras de comercialização</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
<ContentTemplate>
    <table cellpadding="2" width="474px">
        <tr>
            <td class="tdNormal1">Operadora</td>
        </tr>
        <tr>
            <td><asp:DropDownList ID="cboOperadora" SkinID="dropdownSkin" runat="server" Width="99%" AutoPostBack="true" onselectedindexchanged="cboOperadora_SelectedIndexChanged" /> </td>
        </tr>
    </table>
    <br />
    <asp:GridView ID="gridRegras" Width="474px" SkinID="gridViewSkin" 
        runat="server" AllowPaging="True" AutoGenerateColumns="False"  
        DataKeyNames="ID" onrowcommand="gridRegras_RowCommand" 
        onrowdatabound="gridRegras_RowDataBound">
        <Columns>
            <asp:BoundField DataField="Descricao" HeaderText="Regra">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                <ItemStyle Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                <ItemStyle Width="1%" />
            </asp:ButtonField>
        </Columns>
    </asp:GridView>
    <br />
    <table width="474px">
        <tr>
            <td align="right">
                <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
            </td>
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
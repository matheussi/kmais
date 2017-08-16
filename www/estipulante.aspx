<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="estipulante.aspx.cs" Inherits="www.estipulante" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"><img height="50" src="images/imgTitulos/estipulantes.jpg" /></td></tr>
        <tr>
            <td><span class="titulo">Estipulante</span></td>
        </tr>
        <tr>
            <td nowrap><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">

    <asp:UpdatePanel runat="server" ID="upDadosComuns" UpdateMode="Conditional">
    <ContentTemplate>

    <table cellpadding="2" cellspacing="1" width="315">
        <tr>
            <td class="tdPrincipal1" width="100">Nome</td>
            <td class="tdNormal1"><asp:TextBox runat="server" ID="txtDescricao" Width="178" MaxLength="200" SkinID="textboxSkin" /></td>
        </tr>
        <tr>
            <td class="tdPrincipal1" width="100">Texto no boleto</td>
            <td class="tdNormal1"><asp:TextBox runat="server" TextMode="MultiLine" Height="50" ID="txtTextoBoleto" Width="178" MaxLength="200" SkinID="textboxSkin" /></td>
        </tr>
        <tr>
            <td colspan="2" class="tdNormal1"><asp:CheckBox runat="server" ID="chkAtiva" Text="Ativo" SkinID="checkboxSkin" Checked="true" /></td>
        </tr>
    </table>
    <table cellpadding="2" cellspacing="1" width="315">
        <tr>
            <td class="tdPrincipal1" width='100'>Taxa Associativa</td>
            <td class="tdNormal1">
                <asp:TextBox ID="txtTaxa" runat="server" Width="65px" SkinID="textboxSkin" MaxLength="10" />
                &nbsp;
                <asp:DropDownList Width="115" ID="cboTipoTaxa" Runat="server" SkinID="dropdownSkin" />
            </td>
        </tr>
        <tr>
            <td class="tdPrincipal1">Vigência</td>
            <td class="tdNormal1">
                <asp:TextBox EnableViewState="false" SkinID="textboxSkin" Width="65px" runat="server" ID="txtVigencia"  onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                &nbsp;
                <asp:ImageButton ID="cmdAdd" runat="server" ImageUrl="~/images/add.png" ToolTip="adicionar" EnableViewState="true" OnClick="cmdAdd_Click" />
            </td>
        </tr>
    </table>
    <br />
    <asp:GridView BorderWidth="0" cellpadding="2" cellspacing="1" ID="gridTaxas" Width="315" SkinID="gridViewSkin" runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID"
        OnRowDataBound="gridTaxas_RowDataBound" OnRowCommand="gridTaxas_RowCommand">
        <Columns>
            <asp:BoundField DataField="strTipoTaxa" HeaderText="Tipo">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C}">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="Vigencia" HeaderText="Vigência" DataFormatString="{0:dd/MM/yyyy}">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' alt='excluir' border='0' />">
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle Width="1%" />
            </asp:ButtonField>
            </Columns>
    </asp:GridView>
    <br />
    <table cellpadding="2" width="306">
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
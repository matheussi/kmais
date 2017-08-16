<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="declaracaoSaude.aspx.cs" Inherits="www.declaracaoSaude" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="474px">
        <tr><td rowspan="3"><img src="images/imgTitulos/declaracao_saude.png" /></td></tr>
        <tr>
            <td><span class="titulo">Itens da declaração de saúde</span></td>
        </tr>
        <tr>
            <td><span class="subtitulo">Selecione a operadora para exibir sua ficha de declaração de saúde</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" width="474px">
                <tr>
                    <td class="tdPrincipal1" width="70">Operadora</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboOperadora" SkinID="dropdownSkin" runat="server" Width="99%" AutoPostBack="true" onselectedindexchanged="cboOperadora_SelectedIndexChanged" /> </td>
                </tr>
            </table>
            <br />
            <table cellpadding="0" cellspacing="0" width="574px" runat="server" id="tblItens">
                <tr>
                    <td>
                        <span class="subtitulo">Questões cadastradas</span>
                        <asp:GridView ID="gridItens" Width="574px" SkinID="gridViewSkin" 
                            runat="server" AutoGenerateColumns="False" DataKeyNames="ID" 
                            onrowcommand="gridItens_RowCommand" onrowdatabound="gridItens_RowDataBound" 
                            onrowcancelingedit="gridItens_RowCancelingEdit" 
                            onrowediting="gridItens_RowEditing" onrowupdating="gridItens_RowUpdating">
                            <Columns>
                                <asp:CommandField ButtonType="Image" CancelImageUrl="~/images/cancelEditGrid.png" UpdateImageUrl="~/images/saveEditGrid.png" ShowEditButton="True" DeleteText="excluir" EditText="editar" UpdateText="salvar" CancelText="cancelar" EditImageUrl="~/images/edit.png" >
                                    <ItemStyle Font-Size="10px" Width="1%" />
                                </asp:CommandField>
                                <asp:TemplateField HeaderText="Ordem">
                                    <ItemStyle Width="100" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblOrdem" runat="server" Text='<%# Bind("Ordem") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox Width="25" ID="txtOrdem" SkinID="textboxSkin" runat="server" Text='<%# Bind("Ordem") %>' />
                                        <cc1:MaskedEditExtender Mask="99" runat="server" ID="meeOrdem" TargetControlID="txtOrdem" />
                                    </EditItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Center" Width="1%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Código">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" Width="100" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblCodigo" runat="server" Text='<%# Bind("Codigo") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox Width="45" ID="txtCodigo" SkinID="textboxSkin" runat="server" Text='<%# Bind("Codigo") %>' />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Texto">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTexto" runat="server" Text='<%# Bind("Texto") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtTexto" Rows="5" Width="100%" TextMode="MultiLine" SkinID="textboxSkin" runat="server" Text='<%# Bind("Texto") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir">
                                    <ItemStyle Width="1%" HorizontalAlign="Center" />
                                    <ControlStyle Font-Size="10px" ForeColor="#cc0000" />
                                </asp:ButtonField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td align="right" class="tdPrincipal1">
                        <table cellpadding="2" cellspacing="0" width="75%">
                            <tr>
                                <td align="right" height="27" class="tdPrincipal1">
                                    <span style="height:27px; width:100px;">
                                        <asp:Button Width="90px" runat="server" ID="cmdAddItem" Font-Bold="true" SkinID="botaoAzulBorda" Text="adicionar" BorderColor="white" OnClick="cmdAddItem_Click" />
                                    </span>
                                </td>
                            </tr>
                        </table>
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
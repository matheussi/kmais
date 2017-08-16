<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="manipulacaoGradeComissaoProd.aspx.cs" Inherits="www.manipulacaoGradeComissaoProd" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td width="65" rowspan="3"></td></tr>
        <tr><td><span class="titulo">Manipulação de grade de comissão</span></td></tr>
        <tr><td nowrap="nowrap"><span class="subtitulo">Utilize os filtros abaixo</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1" >
                <tr>
                    <td class="tdPrincipal1" width="120">Data inicial</td>
                    <td class="tdNormal1">
                        <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDataInicial" Width="60" Text="" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Data final</td>
                    <td class="tdNormal1">
                        <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDataFinal" Width="60" Text="" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Perfil</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboPerfil" runat="server" SkinID="dropdownSkin" Width="225"  AutoPostBack="true" OnSelectedIndexChanged="cboPerfil_OnSelectedIndexChanged" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" valign="top">Operadora</td>
                    <td class="tdNormal1"><asp:ListBox ID="lstOperadora" runat="server" Height="80" SelectionMode="Multiple" Width="225" SkinID="listBoxSkin" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Tipo de produção</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboTipoProducao" runat="server" SkinID="dropdownSkin" Width="225"  AutoPostBack="true" OnSelectedIndexChanged="cboTipoProducao_OnSelectedIndexChanged" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" valign="top">Tipo de venda</td>
                    <td class="tdNormal1"><asp:ListBox ID="lstTipoVenda" runat="server" Height="80" SelectionMode="Multiple" Width="225" SkinID="listBoxSkin" /></td>
                </tr>
            </table>
            <br />
           <table bgcolor='#D1DDF1' style="border-left: solid 1px #507CD1;border-right: solid 1px #507CD1;border-top: solid 1px #507CD1" cellpadding="2" cellspacing="2" width="451">
                <tr>
                    <td><font color='#507CD1' style='font-size:12px'><b>Parâmetros para atribuição</b></font></td>
                </tr>
            </table> 
            <asp:GridView BorderWidth="0px" cellpadding="2" cellspacing="1" ID="grid" 
                Width="451px" SkinID="gridViewSkin" runat="server" AllowPaging="True" 
                AutoGenerateColumns="False" 
                OnRowDataBound="grid_RowDataBound" OnRowCommand="grid_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="De" ItemStyle-Width="70">
                        <ItemTemplate>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDe" Width="60" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Até" ItemStyle-Width="70">
                        <ItemTemplate>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtAte" Width="60" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Nova tabela">
                        <ItemTemplate>
                            <asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboTabela" Width="100%" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Vigência" ItemStyle-Width="70">
                        <ItemTemplate>
                            <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtVigencia" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' alt='excluir' border='0' />">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <table width="451px" cellpadding="0" cellspacing="0">
                <tr><td height="2"></td></tr>
                <tr>
                    <td align="right">
                        <asp:Button Width="147" EnableViewState="false" runat="server" 
                            ID="cmdAddItemCom" style="cursor: pointer;"  SkinID="botaoPequeno" 
                            Text="adicionar item à tabela" onclick="cmdAddItemCom_Click"  />
                    </td>
                </tr>
            </table>
            <br />
            <table width="451" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="right">
                        <asp:Button ID="cmdExibir" Width="80" Text="Exibir" SkinID="botaoAzul" EnableViewState="false" runat="server" onclick="cmdExibir_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <table width="451" cellpadding="0" cellspacing="0" runat="server" visible="false" id="tblAtribuicao">
                <tr>
                    <td align="left">
                       <table bgcolor='#D1DDF1' style="border-left: solid 1px #507CD1;border-right: solid 1px #507CD1;border-top: solid 1px #507CD1" cellpadding="2" cellspacing="2" width="451" id="tblResultado" runat="server" visible="true">
                            <tr>
                                <td><font color='#507CD1' style='font-size:12px'><b>Resultado</b></font></td>
                            </tr>
                        </table>
                        <asp:GridView BorderWidth="0" cellpadding="2" cellspacing="1" ID="gridEquipe" Width="451" SkinID="gridViewSkin" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="UsuarioID"
                            OnRowDataBound="gridEquipe_RowDataBound" OnRowCommand="gridEquipe_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="UsuarioNome" HeaderText="Produtor">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="UsuarioApelido" HeaderText="Apelido">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Nova tabela">
                                    <ItemTemplate>
                                        <asp:DropDownList runat="server" SkinID="dropdownSkin" ID="cboTabela" Width="100%" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Vigência" ItemStyle-Width="70">
                                    <ItemTemplate>
                                        <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtVigencia" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' alt='excluir' border='0' />">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                                </Columns>
                        </asp:GridView>
                        
                    </td>
                </tr>
                <tr height="4"><td height="4"></td></tr>
                <tr>
                    <td align="right">
                        <asp:Button ID="cmdAtribuir" Width="80" Text="Atribuir" SkinID="botaoAzul" EnableViewState="false" runat="server" OnClientClick="javascript:return confirm('Deseja realmente prosseguir?');" OnClick="cmdAtribuir_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
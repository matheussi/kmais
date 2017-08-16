<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="users.aspx.cs" Inherits="www.admin.users" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img src="../images/icones/produtores.png" /></td></tr>
        <tr><td><span class="titulo">Produtores</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo produtores cadastrados no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
<script language="javascript" type="text/javascript" src="../js/common.js"></script>
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1" width="550px" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1" width="70">Filial</td>
                    <td class="tdNormal1"><asp:DropDownList Width="99%" ID="cboFiliais" SkinID="dropdownSkin" runat="server" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="70">Perfil</td>
                    <td class="tdNormal1"><asp:DropDownList Width="99%" ID="cboPerfis" SkinID="dropdownSkin" runat="server" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="70">Documento</td>
                    <td class="tdNormal1">
                        <asp:TextBox Width="180" MaxLength="60" ID="txtDocumento" SkinID="textboxSkin" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="70">Apelido</td>
                    <td class="tdNormal1">
                        <asp:TextBox Width="180" MaxLength="60" ID="txtApelido" SkinID="textboxSkin" runat="server" />
                        &nbsp;&nbsp;
                        <asp:Button ID="cmdBuscar" Text="Localizar" runat="server" SkinID="botaoAzulBorda" OnClick="cmdBuscar_OnClick" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:GridView ID="gridUsuarios" Width="730px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="ID,Ativo"
                onrowcommand="gridUsuarios_RowCommand"
                onrowdatabound="gridUsuarios_RowDataBound" 
                onpageindexchanging="gridUsuarios_PageIndexChanging" PageSize="100">
                <Columns>
                    <asp:BoundField DataField="Nome" HeaderText="Nome">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Apelido" HeaderText="Apelido">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Documento1" HeaderText="Documento">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="PerfilDescricao" HeaderText="Perfil">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="FilialDescricao" HeaderText="Filial">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField Text="<img src='../images/new.png' title='exibir subordinados' alt='exibir subordinados' border='0' />" CommandName="subordinados" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="inativar" CommandName="inativar" >
                        <ItemStyle ForeColor="#cc0000" Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/condidade.png' title='acessos' alt='acessos' border='0' />" CommandName="acessos" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <asp:Panel ID="pnlSubordinados" runat="server" Visible="false" EnableViewState="false">
                <br />
                <table width="730px" cellpadding="3" cellspacing="0" style="border: solid 1px #507CD1">
                    <tr>
                        <td align="left" class="tdNormal1"><span style="color:black" class="subtitulo" runat="server" id="lblSuperior" enableviewstate="false"></span></td>
                        <td align="right" class="tdNormal1"><asp:ImageButton ID="cmdFecharContato" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar"  /></td>
                    </tr>
                </table>
                <asp:GridView ID="gridSubordinados" Width="730px" SkinID="gridViewSkin" EnableViewState="false"
                    runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ID,Ativo"
                    onrowcommand="gridSubordinados_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="Nome" HeaderText="Nome">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PerfilDescricao" HeaderText="Perfil">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="FilialDescricao" HeaderText="Filial">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:ButtonField Text="<img src='../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
            <br />
        </ContentTemplate>
    </asp:UpdatePanel>
    <table width="730px">
        <tr>
            <td align="right">
                <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
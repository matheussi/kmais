<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="sysusers.aspx.cs" Inherits="www.admin.sysusers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img src="../images/imgTitulos/usuarios.png" alt="" /></td></tr>
        <tr><td><span class="titulo">Usuários</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo usuários cadastrados no sistema</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1" width="474" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1">Perfil</td>
                    <td class="tdNormal1"><asp:DropDownList Width="99%" ID="cboPerfis" SkinID="dropdownSkin" runat="server" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1" width="48">Nome</td>
                    <td class="tdNormal1">
                        <asp:TextBox ID="txtNome" runat="server" MaxLength="200" EnableViewState="false" SkinID="textboxSkin" Width="316" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">E-mail</td>
                    <td class="tdNormal1">
                        <asp:TextBox ID="txtEmail" runat="server" MaxLength="100" EnableViewState="false" SkinID="textboxSkin" Width="316" />
                        &nbsp;
                        <asp:Button Text="Localizar" ID="cmdLocalizar" runat="server" EnableViewState="false" SkinID="botaoAzul" onclick="cmdLocalizar_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:GridView ID="gridUsuarios" Width="474px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,Ativo,SystemUser"
                onrowcommand="gridUsuarios_RowCommand" PageSize="20"
                onrowdatabound="gridUsuarios_RowDataBound" 
                OnPageIndexChanging="gridUsuarios_PageIndexChanging" 
                onselectedindexchanging="gridUsuarios_SelectedIndexChanging">
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
                    <asp:ButtonField Text="<img src='../images/unactive.png' alt='editar' border='0' />" CommandName="inativar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                            <asp:Image ID="imgUser" runat="server" ImageUrl="~/images/tick.png" ToolTip="possui acesso ao sistema" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:ButtonField Text="<img src='../images/edit.png' alt='editar' border='0' />" CommandName="editar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <br />
        </ContentTemplate>
    </asp:UpdatePanel>
    <table width="474px">
        <tr>
            <td align="right">
                <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
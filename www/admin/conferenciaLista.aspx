<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="conferenciaLista.aspx.cs" Inherits="www.admin.conferenciaLista" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"></td></tr>
        <tr><td><span class="titulo">Propostas para conferência</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo as propostas para conferência</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1" width="550px" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1" width="100">Operadora</td>
                    <td class="tdNormal1"><asp:DropDownList Width="99%" ID="cboOperadora" 
                            SkinID="dropdownSkin" runat="server" AutoPostBack="True" 
                            onselectedindexchanged="cboOperadora_SelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Núm. da proposta</td>
                    <td class="tdNormal1">
                        <asp:TextBox ID="txtNum" Width="75" MaxLength="60" SkinID="textboxSkin" runat="server" EnableViewState="false" />
                        &nbsp;
                        <asp:Button ID="cmdLocalizar" Text="localizar" SkinID="botaoAzulBorda" EnableViewState="false" OnClick="cmdLocalizar_Click" runat="server" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:GridView ID="grid" Width="550px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="ID"
                OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" 
                onpageindexchanging="grid_PageIndexChanging">
                <Columns>
                    <asp:TemplateField ItemStyle-Width="1%">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkCheck" runat="server" ToolTip="marque para alterar o status" />
                        </ItemTemplate>
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkCheckHeader" runat="server" ToolTip="marque para alterar o status" AutoPostBack="true" OnCheckedChanged="chkCheckHeader_Change" />
                        </HeaderTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="PropostaNumero" HeaderText="Proposta">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TitularNome" HeaderText="Titular">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Prazo" HeaderText="Prazo" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Departamento" HeaderText="Estágio">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Visible="false" Text="<img src='../images/new_generic.png' title='cadastrar' alt='cadastrar' border='0' />" CommandName="cadastrar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField> 
                </Columns>
            </asp:GridView>
            <br />
            <table cellspacing="0" id="tblNovoEstagio" width="553px" style="border: solid 1px #507CD1" runat="server">
                <tr>
                    <td class="tdNormal1" align="right">
                        Selecione o novo estágio&nbsp;
                        <asp:DropDownList runat="server" ID="cboNovoEstagio" SkinID="dropdownSkin" Width="100" />
                        <asp:Button ID="cmdNovoEstagio" Font-Size="9pt" Text="Enviar" SkinID="botaoAzulBorda" runat="server" OnClientClick="return confirm('Deseja realmente continuar com a operação\n e alterar o estágio das propostas selecionadas?')" OnClick="cmdNovoEstagio_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <table width="553px">
        <tr>
            <td align="right">
                <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
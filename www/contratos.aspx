<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="contratos.aspx.cs" Inherits="www.contratos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img height="50" src="images/imgTitulos/contratos_65_50.png" /></td></tr>
        <tr><td><span class="titulo">Propostas</span></td></tr>
        <tr><td><span class="subtitulo">Selecione a operadora para exibir seus contratos ou informe o número da proposta</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <table width="480px">
        <tr>
            <td width="126"><span class="subtitulo">Operadora</span></td>
            <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadoras" Width="100%" /></td>
        </tr>
    </table>
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table width="480px" border="0" >
                <tr>
                    <td><span class="subtitulo">Nº Proposta</span></td>
                    <td colspan="2">
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtNumProposta" Width="110" MaxLength="40" />
                    </td>
                </tr>
                <tr>
                    <td><span class="subtitulo">Cód. cobrança</span></td>
                    <td colspan="2">
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtIDCobranca" Width="110" MaxLength="60" />
                    </td>
                </tr>
                <tr>
                    <td><span class="subtitulo">Protocolo Atend.</span></td>
                    <td colspan="2">
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtProtocolo" Width="110" MaxLength="30" />
                    </td>
                </tr>
                <tr>
                    <td width="126"><span class="subtitulo">Nome do beneficiário</span></td>
                    <td>
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtBeneificarioNome" Width="250" MaxLength="40" />
                    </td>
                    <td align="right"><asp:Button runat="server" ID="cmdLocalizar" SkinID="botaoAzul" Text="Localizar" Width="80" onclick="cmdLocalizar_Click" /></td>
                </tr>
            </table>
            <br />
            <asp:GridView ID="gridContratos" Width="480px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False"  
                DataKeyNames="ID,Rascunho,Cancelado,Inativo" onrowcommand="gridContratos_RowCommand" 
                onrowdatabound="gridContratos_RowDataBound" PageSize="25"
                OnPageIndexChanging="gridContratos_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="Numero" HeaderText="Número">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="PlanoDescricao" HeaderText="Plano">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="BeneficiarioTitularNome" HeaderText="Titular">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="EmpresaCobranca" HeaderText="Empresa" Visible="false">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image ID="Image1" ImageUrl="~/images/rascunho.png" ToolTip="rascunho" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="1%" />
                    </asp:TemplateField>
                    <asp:ButtonField Text="<img src='images/active.png' title='excluir' alt='excluir' border='0' />" CommandName="inativar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                        <ItemStyle Font-Size="10px" Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                        <ItemStyle Font-Size="10px" Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <asp:Literal ID="litAviso" runat="server" EnableViewState="false" />
            <br />
            <table width="480px">
                <tr>
                    <td align="right">
                        <asp:Button runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
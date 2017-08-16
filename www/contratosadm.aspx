<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="contratosadm.aspx.cs" Inherits="www.contratosadm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img height="50" src="images/imgTitulos/contratos_65_50.png" /></td></tr>
        <tr><td><span class="titulo">Contratos Administrativos</span></td></tr>
        <tr><td><span class="subtitulo">Selecione o estipulante e a operadora para exibir seus contratos</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <table width="480px">
        <tr>
            <td width="76"><span class="subtitulo">Estipulante</span></td>
            <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboEstipulante" AutoPostBack="true" Width="100%" onselectedindexchanged="cboEstipulante_SelectedIndexChanged" /></td>
        </tr>
        <tr>
            <td width="76"><span class="subtitulo">Operadora</span></td>
            <td><asp:DropDownList SkinID="dropdownSkin" runat="server" ID="cboOperadoras" AutoPostBack="true" Width="100%" onselectedindexchanged="cboOperadoras_SelectedIndexChanged" /></td>
        </tr>
    </table>
    <br />
    <asp:GridView ID="gridContratos" Width="480px" SkinID="gridViewSkin" 
        runat="server" AllowPaging="True" AutoGenerateColumns="False"  
        DataKeyNames="ID,Ativo" OnRowDataBound="gridContratos_RowDataBound" 
        OnRowCommand="gridContratos_RowCommand">
        <Columns>
            <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField ItemStyle-Width="1%" DataField="Data" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Data">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:ButtonField CommandName="ativar" Text="<img src='images/active.png' align='middle' alt='ativo' border='0' />">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField Text="<img src='images/edit.png' alt='editar' border='0' />" CommandName="editar" >
                <ItemStyle Font-Size="10px" Width="1%" />
            </asp:ButtonField>
        </Columns>
    </asp:GridView>
    <br />
    <table width="480px">
        <tr>
            <td align="right">
                <asp:Button runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="Default.aspx.cs" Inherits="www._Default" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
    <tr>
        <td><font size='2'><b>Bem-vindo(a),</b>&nbsp;<asp:Label ID="lblUsuario" runat="server" />.</font></td>
    </tr>
    <tr>
        <td><font size='2'><asp:Label ID="lblData" runat="server" />.</font></td>
    </tr>
</table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel ID="pnlAtalhos" runat="server">
        <table width="100%">
            <tr><td colspan="4" height="20"><h2 style="color:gray">Atalhos</h2></td></tr>
            <tr><td colspan="4" height="20"></td></tr>
            <tr>
                <td width="25%" align="center"><a href="contratos.aspx"><img src="images/icones/propostas.png" border='0' /></a></td>
                <td width="25%" align="center"><a href="beneficiarios.aspx"><img src="images/icones/beneficiarios2.png" border='0' /></a></td>
                
                <td width="25%" align="center"><a href="filiais.aspx"><img src="images/icones/filiais.png" border='0' /></a></td>
                <td width="25%" align="center"><a href="admin/users.aspx"><img src="images/icones/produtores.png" border='0' /></a></td>
            </tr>
            <tr>
                <td width="25%" align="center"><b><font size='3' color='#507CD1'>Contratos</font></b></td>
                <td width="25%" align="center"><b><font size='3' color='#507CD1'>Beneficiários</font></b></td>
                <td width="25%" align="center"><b><font size='3' color='#507CD1'>Filiais</font></b></td>
                <td width="25%" align="center"><b><font size='3' color='#507CD1'>Produtores</font></b></td>
            </tr>
            <tr><td colspan="4" height="20"></td></tr>
            <%--<tr>
                <td width="25%" align="center"><a href="filiais.aspx"><img src="images/imgTitulos/operadoras.png" border='0' /></a></td>
                <td width="25%" align="center"><a href="estipulantes.aspx"><img src="images/imgTitulos/operadoras.png" border='0' /></a></td>
                <td width="25%" align="center"><a href="comissionamentos.aspx"><img src="images/imgTitulos/operadoras.png" border='0' /></a></td>
                <td width="25%" align="center"><a href="admin/almoxEntradas.aspx"><img src="images/imgTitulos/operadoras.png" border='0' /></a></td>
            </tr>
            <tr>
                <td width="25%" align="center"><b><font size='3' color='#507CD1'>Filiais</font></b></td>
                <td width="25%" align="center"><b><font size='3' color='#507CD1'>Estipulantes</font></b></td>
                <td width="25%" align="center"><b><font size='3' color='#507CD1'>Comissionamento</font></b></td>
                <td width="25%" align="center"><b><font size='3' color='#507CD1'>Almoxarifado</font></b></td>
            </tr>
            <tr><td colspan="4" height="20"></td></tr>
            <tr>
                <td width="25%" align="center"><a href="admin/gruposDeVenda.aspx"><img src="images/imgTitulos/operadoras.png" border='0' /></a></td>
                <td width="25%" align="center"><a href="admin/layoutsArquivos.aspx"><img src="images/imgTitulos/operadoras.png" border='0' /></a></td>
                <td width="25%" align="center"><a href="admin/tiposcontrato.aspx"><img src="images/imgTitulos/operadoras.png" border='0' /></a></td>
                <td width="25%" align="center"><a href="admin/sysusers.aspx"><img src="images/imgTitulos/operadoras.png" border='0' /></a></td>
                
            </tr>
            <tr>
                <td width="25%" align="center"><b><font size='3' color='#507CD1'>Grupos de venda</font></b></td>
                <td width="25%" align="center"><b><font size='3' color='#507CD1'>Layouts de arquivo</font></b></td>
                <td width="25%" align="center"><b><font size='3' color='#507CD1'>Tipos de contrato</font></b></td>
                <td width="25%" align="center"><b><font size='3' color='#507CD1'>Usuários</font></b></td>
            </tr>--%>
        </table>
    </asp:Panel>
</asp:Content>
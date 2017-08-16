<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ucConferenciaPassos.ascx.cs" Inherits="www.usercontrols.ucConferenciaPassos" %>
<table align="center" border='0' cellspacing='0' cellpadding='0' width='850px' style='background:url(../images/site/bgPassoaPasso.gif) no-repeat;'>
    <tr>
        <td height='52' valign='top' align='center' style='padding:0px 15px;'><img runat="server" id="IMG1" src='~/images/site/Passo_on.gif'  alt='' /><br /><span style='font-size:14px; color:#280a69;'><strong><asp:Literal ID="lit1" runat="server" Text="Fase 1" /></strong></span></td>
        <td height='52' valign='top' align='center' style='padding:0px 15px;'><img runat="server" id="IMG2" src='~/images/site/Passo_on.gif'  alt='' /><br /><span style='font-size:14px; color:#280a69;'><strong><asp:Literal ID="lit2" runat="server" Text="Fase 2" /></strong></span></td>
        <td height='52' valign='top' align='center' style='padding:0px 15px;'><img runat="server" id="IMG3" src='~/images/site/Passo_on.gif'  alt='' /><br /><span style='font-size:14px; color:#280a69;'><strong><asp:Literal ID="lit3" runat="server" Text="Fase 3" /></strong></span></td>
        <%--<td height='52' valign='top' align='center' style='padding:0px 15px;'><img runat="server" id="IMG4" src='~/images/site/Passo_off.gif' alt='' /><br /><span style='font-size:14px; color:#280a69;'><strong><asp:Literal ID="lit4" runat="server" Text="Fase 4" /></strong></span></td>--%>
    </tr>
</table>
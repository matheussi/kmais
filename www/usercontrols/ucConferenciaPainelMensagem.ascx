<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ucConferenciaPainelMensagem.ascx.cs" Inherits="www.usercontrols.ucConferenciaPainelMensagem" %>
<style type='text/css'>
  .Box {
     background:#f4e7f8;
     border-left:1px solid #c195c3;
     border-right:1px solid #c195c3;
     width:293px;
     height:300px;
     padding:8px 0px;
  }
  
  .TextBox {
     padding:10px;
     margin:0px 10px;
     overflow:auto;
     height:280px;
  }
</style>
<table width="100%" cellpadding="2" cellspacing="1">
    <%--<tr>
        <td><b>Mensagens</b></td>
    </tr>
    <tr>--%>
        <td align="left">
            <div><img runat="server" enableviewstate="false" src='~/images/site/boxTop.gif' alt='' /></div>
            <div class='Box'>
            <div class='TextBox'><b>Mensagens</b><br /><br /><asp:Literal ID="litMsg" runat="server" /></div>
            <div><img runat="server" enableviewstate="false" src='~/images/site/boxBottom.gif' alt='' /></div>
        </td>
    </tr>
</table>
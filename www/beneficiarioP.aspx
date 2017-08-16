<%@ Page Theme="Theme" Language="C#" AutoEventWireup="false" CodeBehind="beneficiarioP.aspx.cs" Inherits="www.beneficiarioP" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register src="usercontrols/ucBeneficiarioForm.ascx" tagname="ucBeneficiarioForm" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Beneficiário</title>
    <link href="css/style.css" rel="stylesheet" type="text/css" />
    <script src="js/common.js" type="text/javascript"></script> 
</head>
<body>
    <form id="form1" runat="server">
    <cc1:ToolkitScriptManager ID="sm" runat="server" />
    <asp:UpdateProgress runat="server" ID="updateProgress">
        <ProgressTemplate>
            <div style="background-color:#507CD1; position:fixed; left: 50%; top: 0; width:100px; height: 15px; margin-left: -50px; text-align:center; color:white">AGUARDE...</div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <table width="70%">
        <tr><td rowspan="3" width="54"><img src="images/icones/beneficiarios2.png" />&nbsp;</td></tr>
        <tr><td><span class="titulo">Beneficiário</span></td></tr>
        <tr><td><span class="subtitulo">Preencha os campos abaixo e clique em salvar</span></td></tr>
    </table>
    <br />
    <asp:UpdatePanel runat="server" ID="up">
        <ContentTemplate>
            <uc1:ucBeneficiarioForm EsconderBotaoVoltar="true" ID="ucBeneficiarioDetalhe" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
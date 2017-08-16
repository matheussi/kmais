<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="termoquitacao.aspx.cs" Inherits="www.termoquitacao" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body style="font-family:arial">
    <form id="form1" runat="server">
        <table width="99%" align="center">
            <tr>
                <td align="right"><img src="http://www.linkecerebro.com.br/qualicorp/qualicorp2.png" /></td>
            </tr>
            <tr><td><br /><br /><br /></td></tr>
            <tr>
                <td align="left">Cliente Sr(a). <asp:Literal ID="litNome" runat="server" /></td>
            </tr>
            <tr>
                <td align="left">CPF: <asp:Literal ID="litCpf" runat="server" /></td>
            </tr>
        </table>
        <br /><br /><br /><br /><br /><br />
        <table width="99%" align="center">
            <tr>
                <td align="left">Ref.: DECLARAÇÃO DE QUITAÇÃO ANUAL DE DÉBITOS</td>
            </tr>
        </table>
        <br /><br /><br /><br /><br /><br />
        <table width="99%" align="center">
            <tr>
                <td align="left">Em cumprimento à Lei nº 12.007, de 29 de julho de 2009, declaramos que o cliente acima
está quite com suas faturas vencidas no ano de 2013 e anos anteriores.</td>
            </tr>
        </table>
        <br /><br /><br /><br /><br />
        <table width="99%" align="center">
            <tr>
                <td align="left">Atenciosamente,</td>
            </tr>
        </table>
        <br />
        <table width="99%" align="center">
            <tr>
                <td align="left" colspan="2">QUALICORP ADM. E SERV. LTDA</td>
            </tr>
            <tr>
                <td align="left">CNPJ 03.609.855/0001-02</td>
                <td align="right">www.qualicorp.com.br</td>
            </tr>
        </table>
        <br />
        <table width="99%" align="center">
            <tr>
                <td width="350" align="left">TODO O BRASIL (exceto RJ)</td>
                <td align="left">RIO DE JANEIRO</td>
            </tr>
            <tr>
                <td align="left"><b>0800-16-2000</b></td>
                <td align="left"><b>0800-778-4004</b></td>
            </tr>
        </table>
    </form>
</body>


</html>

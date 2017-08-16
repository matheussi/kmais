<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="relacaoCobrancas.aspx.cs" Inherits="www.reports.relacaoCobrancas" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <link rel="stylesheet" type="text/css" href="../css/style.css" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <table width="60%" align="center">
            <tr>
                <td><img runat="server" src="~/images/print.png" align="left" /> <a href="javascript:window.print()" class="fonte11"><font color="black">IMPRIMIR</font></a></td>
            </tr>
            <tr height="10"><td height="10"></td></tr>
            <tr>
                <td>
                    <table cellpadding="2" cellspacing="0" style="border: solid 1px black" width="100%">
                        <tr>
                            <td width="100"><font style="font-size:10pt">CONTRATO</font></td>
                            <td><font style="font-size:10pt"><asp:Literal ID="litContrato" runat="server" /></font></td>
                        </tr>
                        <tr>
                            <td><font style="font-size:10pt">TITULAR</font></td>
                            <td><font style="font-size:10pt"><asp:Literal ID="litTitNome" runat="server" /></font></td>
                        </tr>
                        <tr>
                            <td><font style="font-size:10pt">CPF</font></td>
                            <td><font style="font-size:10pt"><asp:Literal ID="litTitCpf" runat="server" /></font></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr height="10"><td height="10"></td></tr>
            <tr>
                <td align="center">
                    <asp:GridView CellPadding="2" RowStyle-BorderStyle="Solid" RowStyle-BorderColor="Black" RowStyle-BorderWidth="1px" CellSpacing="0" GridLines="Both" Font-Names="Arial" Font-Size="10pt" BorderStyle="Solid" BorderColor="Black" BorderWidth="1px" ID="gridCobranca" runat="server" DataKeyNames="ID" AutoGenerateColumns="False" Width="100%" AllowPaging="false">
                       <Columns>
                            <asp:BoundField DataField="Parcela" HeaderText="Parc.">
                                <HeaderStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C}">
                                <HeaderStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="ValorPgto" HeaderText="Valor Pago" DataFormatString="{0:C}">
                                <HeaderStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="DataVencimento" HeaderText="Vencimento" DataFormatString="{0:dd/MM/yyyy}">
                                <HeaderStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="strPago" HeaderText="Pago">
                                <HeaderStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="strDataPago" HeaderText="Data Pgto">
                                <HeaderStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>

    </form>
</body>
</html>

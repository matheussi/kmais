<%@ Page Language="C#" Theme="Theme" AutoEventWireup="false" CodeBehind="demonstPagtos.aspx.cs" Inherits="www.demonstPagtos" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Demonstrativo de pagamentos</title>
    <link rel="stylesheet" type="text/css" href="css/style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <table align="center" width="95%">
            <tr>
                <td>
                    <h2>Demonstrativo de Pagamentos <asp:Literal ID="litAno" runat="server" /></h2>
                    <table style="font-size:12px">
                        <tr>
                            <td colspan="2"><asp:Literal ID="litData" runat="server" /></td>
                        </tr>
                        <tr><td height='5px'></td></tr>
                        <tr>
                            <td width="140px"><b>Ilmo(a). Senhor(a)</b></td>
                            <td><asp:Literal ID="litNomeTitular" runat="server" /></td>
                        </tr>
                        <tr>
                            <td><b>CPF:</b></td>
                            <td><asp:Literal ID="litCpfTitular" runat="server" /></td>
                        </tr>
                    </table>
                    <br />
                    <table style="font-size:12px">
                        <tr>
                            <td><b>Beneficiário(a) UBRASP,</b></td>
                        </tr>
                        <tr><td height='8'></td></tr>
                        <tr>
                            <td>
                                <%--Abaixo o demonstrativo de pagamentos efetuados, durante o ano calendário de , à  UBRASP 
                                Administradora de Benefícios Ltda., inscrita no CNPJ/MF sob o nº 49.938.327/0001-06, e destinados à 
                                manutenção do plano privado de assistência à  saúde, coletivo por adesão, por meio de contrato coletivo
                                firmado com a operadora .<br />
                                Esse demonstrativo relaciona as despesas médicas que foram pagas pelo(a) Sr(a). e que são dedutí­veis em
                                Declaração de Imposto de Renda.--%>
                                Abaixo o demonstrativo de pagamentos efetuados, durante o ano calendário de <asp:Literal ID="litAnoCalendario" runat="server" />, à  UBRASP 
                                Administradora de Benefícios Ltda., inscrita no CNPJ/MF sob o nº 49.938.327/0001-06, e destinados à 
                                manutenção do plano privado de assistência à  saúde, coletivo por adesão, por meio de contrato coletivo
                                firmado com a operadora <asp:Literal ID="litOperadoraNome" runat="server" />.<br />
                                Esse demonstrativo relaciona as despesas médicas que foram pagas pelo(a) Sr(a). e que são dedutí­veis em
                                Declaração de Imposto de Renda.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <br />
        <table align='center' cellpadding="4" cellspacing="0" style="border:solid 1px black;font-size:12px" width="400px">
            <tr>
                <td style="border-bottom:solid 1px black" bgcolor='whitesmoke'><b>Competência</b></td>
                <td style="border-bottom:solid 1px black" bgcolor='whitesmoke'><b>*Valor mensal</b></td>
            </tr>
            <tr>
                <td>Janeiro</td>
                <td><asp:Literal ID="litJaneiro" runat="server" Text="0,00" /></td>
            </tr>
            <tr>
                <td>Fevereiro</td>
                <td><asp:Literal ID="litFevereiro" runat="server" Text="0,00" /></td>
            </tr>
            <tr>
                <td>Março</td>
                <td><asp:Literal ID="litMarco" runat="server" Text="0,00" /></td>
            </tr>
            <tr>
                <td>Abril</td>
                <td><asp:Literal ID="litAbril" runat="server" Text="0,00" /></td>
            </tr>
            <tr>
                <td>Maio</td>
                <td><asp:Literal ID="litMaio" runat="server" Text="0,00" /></td>
            </tr>
            <tr>
                <td>Junho</td>
                <td><asp:Literal ID="litJunho" runat="server" Text="0,00" /></td>
            </tr>
            <tr>
                <td>Julho</td>
                <td><asp:Literal ID="litJulho" runat="server" Text="0,00" /></td>
            </tr>
            <tr>
                <td>Agosto</td>
                <td><asp:Literal ID="litAgosto" runat="server" Text="0,00" /></td>
            </tr>
            <tr>
                <td>Setembro</td>
                <td><asp:Literal ID="litSetembro" runat="server" Text="0,00" /></td>
            </tr>
            <%----%>
            <tr>
                <td>Outubro</td>
                <td><asp:Literal ID="litOutubro" runat="server" Text="0,00" /></td>
            </tr>
            <tr>
                <td>Novembro</td>
                <td><asp:Literal ID="litNovembro" runat="server" Text="0,00" /></td>
            </tr>
            <tr>
                <td>Dezembro</td>
                <td><asp:Literal ID="litDezembro" runat="server" Text="0,00" /></td>
            </tr>
            <tr>
                <td style="border-top:solid 1px black" bgcolor='whitesmoke'>*Valor total</td>
                <td style="border-top:solid 1px black" bgcolor='whitesmoke'><asp:Literal ID="litTotal" runat="server" /></td>
            </tr>
            <tr>
                <td colspan="2" style="border-top:solid 1px black;font-size:11px"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td>
            </tr>
        </table>
        <br /><br />
        <center><div style="color:black"><b>COMPOSIÇÃO DO GRUPO FAMILIAR</b></div></center>
        <table align='center' cellpadding="4" cellspacing="0" style="border:solid 1px black;font-size:12px" width="400px">
            <tr>
                <td style="border-bottom:solid 1px black" bgcolor='whitesmoke'><b>Condição</b></td>
                <td style="border-bottom:solid 1px black" bgcolor='whitesmoke'><b>Nome</b></td>
                <td style="border-bottom:solid 1px black" bgcolor='whitesmoke'><b>*Valor por beneficiário(a)</b></td>
            </tr>
            <asp:Literal ID="litTrs" runat="server" />
            <tr>
                <td style="border-top:solid 1px black" bgcolor='whitesmoke'>*Valor total</td>
                <td style="border-top:solid 1px black" bgcolor='whitesmoke' colspan="2"><asp:Literal ID="litTotalBeneficiario" runat="server" /></td>
            </tr>
            <tr>
                <td style="border-top:solid 1px black;font-size:11px" colspan="3"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td>
            </tr>
        </table>
        <br /><br />
        <table align="center" width="95%">
            <tr>
                <td>
                Atenção: Caso este informe seja utilizado para fins de declaração de Imposto de Renda, esclarecemos que
                somente podem ser deduzidas as parcelas relativas ao contribuinte e aos dependentes devidamente relacionados
                na própria declaração. As deduções estão sujeitas às regras estabelecidas pela legislação que regulamenta o
                imposto (Decreto nº 3.000/99).
                </td>
            </tr>
            <tr><td height='8'></td></tr>
            <tr><td><b>União Brasileira dos Servidores Públicos</b></td></tr>
        </table>
    </form>
    <script type="text/javascript">
        window.print();
    </script>
</body>
</html>

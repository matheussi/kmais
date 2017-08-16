<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="conferencia1b.aspx.cs" Inherits="www.admin.conferencia1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../usercontrols/ucConferenciaPainelMensagem.ascx" TagName="ucConferenciaPainelMensagem" tagprefix="uc1" %>
<%@ Register Src="../usercontrols/ucConferenciaPassos.ascx" TagName="ucConferenciaPassos" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"></td></tr>
        <tr><td><span class="titulo">Conferência</span></td></tr>
        <tr><td><span class="subtitulo">Conferência de propostas</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <uc2:ucConferenciaPassos ID="ucConferenciaPassos" runat="server" />
            <br />
            <table cellpadding="2" cellspacing="0" border="0" width="750px">
                <tr>
                    <td width="85%" valign="top" style="border: solid 0px #507CD1">
                        <table width="90%" cellpadding="3" cellspacing="0" style="border: solid 1px #507CD1">
                            <tr>
                                <td width="95" bgcolor='#507CD1'><font color='#EFF3FB'><b>CPF</b></font></td>
                                <td bgcolor='#EFF3FB'><asp:TextBox ID="txtCpf" SkinID="textboxSkin" runat="server" /><cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeCPF" Mask="999,999,999-99" ClearMaskOnLostFocus="true" TargetControlID="txtCpf" /></td>
                                <td bgcolor='#EFF3FB'><asp:Button ID="cmdValidarCpf" Text="validar" runat="server" SkinID="botaoPequeno" onclick="cmdValidarCpf_Click" /></td>
                            </tr>
                            <tr height="1"><td height="1"></td></tr>
                            <tr>
                                <td bgcolor='#507CD1'><font color='#EFF3FB'><b>Nome do titular</b></font></td>
                                <td colspan="2" bgcolor='#EFF3FB'><asp:TextBox ID="txtNomeTitular" Width="180" SkinID="textboxSkin" runat="server" /></td>
                            </tr>
                            <tr height="1"><td height="1"></td></tr>
                            <tr>
                                <td bgcolor='#507CD1'><font color='#EFF3FB'><b>CEP</b></font></td>
                                <td bgcolor='#EFF3FB'><asp:TextBox Width="60" ID="txtCep" SkinID="textboxSkin" runat="server" /><cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeCEP" Mask="99999-999" ClearMaskOnLostFocus="true" TargetControlID="txtCep" /></td>
                                <td bgcolor='#EFF3FB'><asp:Button ID="cmdValidarCep" Text="validar" runat="server" SkinID="botaoPequeno" onclick="cmdValidarCep_Click" /></td>
                            </tr>
                            <tr>
                                <td colspan="3" bgcolor='#EFF3FB'>
                                    <asp:Literal runat="server" ID="litEndereco" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td width="200"></td>
                    <td valign="top">
                        <uc1:ucConferenciaPainelMensagem ID="ucCPM" runat="server" />
                    </td>
                </tr>
            </table>
            <br />
            <table cellpadding="2" cellspacing="0" width="750px" style="border: solid 1px #507CD1">
                <tr>
                    <td align="left"><asp:Button runat="server" ID="cmdVoltar" Text="Voltar" Width="80" SkinID="botaoAzul" onclick="cmdVoltar_Click" /></td>
                    <td align="right"><asp:Button runat="server" ID="cmdProximo" Text="Próximo" Width="80" SkinID="botaoAzul" onclick="cmdProximo_Click" /></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
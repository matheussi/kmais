<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="beneficiario.aspx.cs" Inherits="www.beneficiario" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register src="usercontrols/ucBeneficiarioForm.ascx" tagname="ucBeneficiarioForm" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="54"><img src="images/icones/beneficiarios2.png" />&nbsp;</td></tr>
        <tr><td><span class="titulo">Beneficiário</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Preencha os campos abaixo e clique em salvar</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
<ContentTemplate>
    <uc1:ucBeneficiarioForm CarregarDeInicio="true" ID="ucBeneficiarioForm1" runat="server" />
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
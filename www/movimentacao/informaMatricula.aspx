<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="informaMatricula.aspx.cs" Inherits="www.movimentacao.informaMatricula" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel ID="up" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="650px" style="border:solid 1px #507CD1;background-color:#EFF3FB">
                <tr>
                    <td>Operadora</td>
                    <td><asp:DropDownList ID="cboOperadora" runat="server" SkinID="dropdownSkin" /></td>
                </tr>
                <tr>
                    <td>Número da proposta</td>
                    <td><asp:TextBox ID="txtNumeroProposta" runat="server" SkinID="textboxSkin" /></td>
                </tr>
                <tr>
                    <td>Vigência</td>
                    <td>
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtVigencia" Width="66px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                    </td>
                </tr>
                <tr>
                    <td>CPF do titular</td>
                    <td>
                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtTitularCPF" Width="79px" />
                        <cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeTitularCPF" Mask="999,999,999-99" ClearMaskOnLostFocus="true" TargetControlID="txtTitularCPF" />
                    </td>
                </tr>
                <tr>
                    <td>Nome do titular</td>
                    <td><asp:TextBox ID="txtTitularNome" runat="server" Width="300" SkinID="textboxSkin" /></td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Button ID="cmdFiltrar" Text="Filtrar" runat="server" SkinID="botaoAzulBorda" onclick="cmdFiltrar_Click" />
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlGrid" Visible="false" runat="server">
                <br />
                <asp:GridView ID="grid" Width="650px" SkinID="gridViewSkin" 
                    runat="server" AllowPaging="false" AutoGenerateColumns="False" DataKeyNames="ID"
                    OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="Numero" HeaderText="Número">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Vigencia" HeaderText="Vigência" DataFormatString="{0:dd/MM/yyyy}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="BeneficiarioTitularNome" HeaderText="Titular">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="BeneficiarioCPFFormatado" HeaderText="CPF">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="BeneficiarioTitularDataNascimento" HeaderText="Data Nasc." DataFormatString="{0:dd/MM/yyyy}">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="BeneficiarioTitularNomeMae" HeaderText="Nome da mãe">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:ButtonField Text="<img src='../images/tick.png' title='selecionar' alt='selecionar' border='0' />" CommandName="selecionar" >
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
                <br />
                <table width="650px" style="border:solid 1px #507CD1;background-color:#EFF3FB">
                    <tr>
                        <td width='50px'>Matrícula</td>
                        <td>
                            <asp:TextBox ID="txtMatricula" EnableViewState="false" runat="server" SkinID="textboxSkin" />
                            <asp:Button Text="Salvar" ID="cmdAlterar" SkinID="botaoAzulBorda" runat="server" onclick="cmdAlterar_Click" />
                        </td>
                        <td align="right"><i><asp:Literal ID="lblMatriculaAtual" runat="server" EnableViewState="false" /></i></td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

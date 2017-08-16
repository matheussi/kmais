<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="AlmoxEntradas.aspx.cs" Inherits="www.admin.AlmoxEntradas" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img src="../images/imgTitulos/produtos_entrada.png" /></td></tr>
        <tr><td><span class="titulo">Almoxarifado - Entradas</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo as entradas em estoque ocorridas no sistema, em ordem decrescente</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table  cellpadding="2" cellspacing="0" style="border: solid 1px #507CD1" width="650">
                <tr>
                    <td class="tdNormal1" width="60px"><b>Operadora</b></td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboOperadora" SkinID="dropdownSkin" runat="server" Width="489px" AutoPostBack="true" onselectedindexchanged="cboOperadora_SelectedIndexChanged" /></td>
                </tr>
                <tr>
                    <td class="tdNormal1" width="60px"><b>Filial</b></td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboFilial" SkinID="dropdownSkin" runat="server" Width="489px" AutoPostBack="true" onselectedindexchanged="cboFilial_SelectedIndexChanged" /></td>
                </tr>
                <tr>
                    <td class="tdNormal1" width="60px"><b>Tipo</b></td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboTipo" SkinID="dropdownSkin" runat="server" Width="489px" AutoPostBack="true" onselectedindexchanged="cboTipo_SelectedIndexChanged" /></td>
                </tr>
                <%--<tr>
                    <td class="tdNormal1"><b>Produto</b></td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboProduto" SkinID="dropdownSkin" runat="server" Width="489px" AutoPostBack="true" OnSelectedIndexChanged="cboProduto_SelectedIndexChanged" /></td>
                </tr>--%>
            </table>
            <br />
            <asp:GridView ID="gridEntradas" Width="650" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False"  
                DataKeyNames="ID">
                <Columns>
                    <asp:BoundField DataField="TipoProdutoDescricao" HeaderText="Tipo">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField Visible="true" DataField="OperadoraNome" HeaderText="Operadora">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="QTD" HeaderText="Qtd. Inicial">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="QTDFlutuante" HeaderText="Qtd. Atual">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ProdutoQTDAtual" HeaderText="Etoque Atual">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="NumeracaoResumo" HeaderText="Numeração">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataFormatString="{0:dd/MM/yyyy}" DataField="Data" HeaderText="Data">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <table width="650px">
        <tr>
            <td align="right">
                <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Nova" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
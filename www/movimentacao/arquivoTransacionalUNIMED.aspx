<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="arquivoTransacionalUNIMED.aspx.cs" Inherits="www.movimentacao.arquivoTransacionalUNIMED" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td><span class="titulo">Arquivos Transacionais</span></td></tr>
        <tr><td><span class="subtitulo">Gerar arquivos transacionais para Unimed</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">

    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdGerar" />
        </Triggers>
        <ContentTemplate>
            <table cellpadding="4" cellspacing="0" style="border: solid 1px #507CD1">
                <tr>
                    <td ><b>Proposta</b></td>
                    <td ><asp:TextBox ID="txtProposta" runat="server" MaxLength="25" SkinID="textboxSkin" />&nbsp;<asp:ImageButton ToolTip="localizar beneficiários" ID="imgLocalizarBeneficiarios" ImageUrl="~/images/search.png" runat="server" OnClick="imgLocalizarBeneficiarios_Click" /></td>
                </tr>
                <asp:Panel ID="pnlBeneficiarios" Visible="false" runat="server">
                <tr>
                    <td style="border-top: solid 1px #507CD1" colspan="2" class="tdNormal1"><b>Beneficiários</b></td>
                </tr>
                <tr>
                    <td colspan="2" class="tdNormal1">
                        <asp:GridView ID="gridBeneficiarios" OnRowCommand="gridBeneficiarios_RowCommand" OnRowDataBound="gridBeneficiarios_RowDataBound" Width="100%" SkinID="gridViewSkin" runat="server" AutoGenerateColumns="False"  DataKeyNames="ID,ContratoID,BeneficiarioID">
                            <Columns>
                                <asp:BoundField DataField="BeneficiarioNome" HeaderText="Beneficiario">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="BeneficiarioCPF" HeaderText="CPF">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="NumeroSequencial" HeaderText="Número">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Ativo" HeaderText="Ativo">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:ButtonField CommandName="selecionar" Text="<img src='../images/tick.png' title='selecionar' alt='selecionar' border='0' />">
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                </asp:Panel>
                <tr>
                    <td style="border-top: solid 1px #507CD1"><b>Tipo de transação</b></td>
                    <td style="border-top: solid 1px #507CD1">
                        <asp:DropDownList ID="cboTipoTransacao" SkinID="dropdownSkin" runat="server" />&nbsp;
                        <asp:Button ID="cmdGuardar" Text="guardar" SkinID="botaoAzulBorda" runat="server" onclick="cmdGuardar_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:Panel ID="pnlAgenda" runat="server" Visible="false">
                <table width="494" cellpadding="4" cellspacing="0" style="border: solid 1px #507CD1">
                    <tr>
                        <td class="tdNormal1">Movimentação corrente</td>
                    </tr>
                    <tr>
                        <td class="tdNormal1">
                            <asp:GridView ID="grid" Width="480px" SkinID="gridViewSkin" runat="server" AllowPaging="false" AutoGenerateColumns="False"  DataKeyNames="ID,Tipo,PropostaID,BeneficiarioID" OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound" OnPageIndexChanging="grid_PageIndexChanging">
                                <Columns>
                                    <asp:BoundField DataField="PropostaNumero" HeaderText="Proposta">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BeneficiarioNome" HeaderText="Nome">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="BeneficiarioCPF" HeaderText="CPF">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField Visible="true" DataField="TipoDescricao" HeaderText="Transação">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1">
                            <asp:Button ID="cmdGerar" Text="gerar arquivo para" OnClientClick="return confirm('Deseja realmente gerar o arquivo de movimentação?');" SkinID="botaoAzulBorda" runat="server" onclick="cmdGerar_Click" />
                            &nbsp;
                            <asp:DropDownList ID="cboTipoTransacaoGerar" SkinID="dropdownSkin" runat="server" />
                            &nbsp;
                            e contrato
                            &nbsp;
                            <asp:DropDownList ID="cboContratoAdm" Width="142" SkinID="dropdownSkin" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
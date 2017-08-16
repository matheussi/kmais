<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="emissoesPorBoleto.aspx.cs" Inherits="www.UBRASP.arquivos.scc.emissoesPorBoleto" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td rowspan="3" width="40"></td></tr>
        <tr><td><span class="titulo">PSCC - Emissões de boleto</span></td></tr>
        <tr><td nowrap="true"><span class="subtitulo">Geração de rol para emissão de arquivo CNAB</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <script src="../../../js/common.js" type="text/javascript"></script>
    <asp:UpdatePanel ID="up" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <%--<table style="border: 1px solid #507CD1" cellpadding="8" cellspacing="0" width="574px">
                <tr>
                    <td class="tdNormal1">Arquivo</td>
                    <td class="tdNormal1"><asp:FileUpload runat="server" ID="fuArquivo" Width="100%" /></td>
                    <td class="tdNormal1"><asp:Button ID="cmdProcessar" runat="server" Text="processar" SkinID="botaoAzulBorda" OnClientClick="return confirm('Deseja realmente processar o arquivo?');" /></td>
                </tr>
            </table>
            <br />--%>
            <table width="100%" border="0">
                <tr>
                    <td><b>Boletos<asp:Literal ID="litDesc" runat="server" EnableViewState="true" /></b></td>
                    <td align="right">
                        Qtd. <asp:TextBox ID="txtQtd" Text="1" onkeypress="filtro_SoNumeros(event);" runat="server" MaxLength="2" Width="50px" />
                    </td>
                    <td align="right" width="175x">
                        Vencimento: <asp:TextBox ID="txtVencimento" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" runat="server" MaxLength="10" Width="90px" />
                    </td>
                    <td align="right" width="130px">
                        <asp:Button ID="cmdBoleto" SkinID="botaoPequeno" Text="Gerar agendamento" OnClick="cmdBoleto_click" OnClientClick="return confirm('ATENÇÃO!\nDeseja realmente gerar os boletos e agendar geração de arquivo?');" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:GridView ID="grid" DataKeyNames="Funcional" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="true" PageSize="1000" AutoGenerateColumns="False" >
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="marcado" runat="server" Checked='<%# bool.Parse(Eval("Marcado").ToString()) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Nome" HeaderText="Nome">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Funcional" HeaderText="Funcional">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ValorSistema" HeaderText="Sistema" DataFormatString="{0:N2}">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle ForeColor="Red" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Atualizado" HeaderText="Atualizado">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

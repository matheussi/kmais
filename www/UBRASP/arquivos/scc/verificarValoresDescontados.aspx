<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="verificarValoresDescontados.aspx.cs" Inherits="www.UBRASP.arquivos.scc.verificarValoresDescontados" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="100%">
        <tr><td rowspan="3" width="40"></td></tr>
        <tr><td><span class="titulo">PSCC - Verificar valores descontados</span></td></tr>
        <tr><td nowrap="true"><span class="subtitulo">Informe no campo abaixo o arquivo a ser processado</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <script src="../../../js/common.js" type="text/javascript"></script>
    <asp:UpdatePanel ID="up" runat="server" UpdateMode="Always">
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdProcessar" />
        </Triggers>
        <ContentTemplate>
            <table style="border: 1px solid #507CD1" cellpadding="8" cellspacing="0" width="574px">
                <tr>
                    <td class="tdNormal1">Arquivo</td>
                    <td class="tdNormal1"><asp:FileUpload runat="server" ID="fuArquivo" Width="100%" /></td>
                    <td class="tdNormal1"><asp:Button ID="cmdProcessar" runat="server" Text="processar" SkinID="botaoAzulBorda" OnClick="cmdProcessar_click" OnClientClick="return confirm('Deseja realmente processar o arquivo?');" /></td>
                </tr>
            </table>
            <br />
            <table width="100%">
                <tr>
                    <td><b>DESCONTADOS<asp:Literal ID="litDesc" runat="server" EnableViewState="true" /></b></td>
                    <td align="right" width="175x">
                        Vencimento: <asp:TextBox ID="txtVencimentoDescontados" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" runat="server" MaxLength="10" Width="90px" />
                    </td>
                    <td>
                        <asp:Button ID="cmdBoletoDesc" Text="Gerar agendamento" Visible="false" SkinID="botaoPequeno" OnClick="cmdBoletoDesc_click" OnClientClick="return confirm('ATENÇÃO!\nDeseja realmente gerar os boletos e agendar geração de arquivo?');" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:GridView ID="gridDescontados" DataKeyNames="Funcional" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="true" PageSize="1000" AutoGenerateColumns="False" >
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
                                <asp:BoundField DataField="Status" HeaderText="Status">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ValorArquivo" HeaderText="Valor" DataFormatString="{0:N2}">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ValorDescontado" HeaderText="Desconto" DataFormatString="{0:N2}">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Residual" HeaderText="Residual" DataFormatString="{0:N2}">
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
            <br />
            <table width="100%">
                <tr>
                    <td><b>NÃO DESCONTADOS<asp:Literal ID="litNaoDesc" runat="server" EnableViewState="true" /></b></td>
                    <td align="right" width="175x">
                        Vencimento: <asp:TextBox Enabled="false" ID="txtVencimentoNAODescontados" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" runat="server" MaxLength="10" Width="90px" />
                    </td>
                    <td>
                        <asp:Button ID="cmdBoletoNaoDesc" Enabled="false" Text="Gerar agendamento" Visible="false" SkinID="botaoPequeno" OnClick="cmdBoletoNaoDesc_click" OnClientClick="return confirm('ATENÇÃO!\nDeseja realmente gerar os boletos e agendar geração de arquivo?');" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
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
                                <asp:BoundField DataField="Funcional" HeaderText="Associativa">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Status" HeaderText="Status">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ValorArquivo" HeaderText="Valor" DataFormatString="{0:N2}">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ValorDescontado" HeaderText="Descontado" DataFormatString="{0:N2}">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Residual" HeaderText="Residual" DataFormatString="{0:N2}">
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

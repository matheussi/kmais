<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="true" CodeBehind="processar.aspx.cs" Inherits="www.UBRASP.arquivos.prodesp.processar" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="40"></td></tr>
        <tr><td><span class="titulo">Prodesp - Retorno</span></td></tr>
        <tr><td nowrap="true"><span class="subtitulo">Arquivos de retorno Prodesp</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <script src="../../../js/common.js" type="text/javascript"></script>
    <table>
        <tr>
            <td width="100px">Arquivo de retorno</td>
            <td width="455px"><asp:FileUpload ID="fuArquivo" runat="server" Width="450px" /> </td>
            <td width="100px"><asp:Button ID="cmdEnviar" Text="Enviar" runat="server" OnClick="cmdEnviar_Click" /> </td>
            <td width="100px"><asp:Button ID="cmdExportar" Visible="false" Text="Enviar" runat="server" /> </td>
        </tr>
    </table>
    <br />
    <table width="100%">
         <tr>
            <td><b>Boletos<asp:Literal ID="litDesc" runat="server" EnableViewState="true" /></b></td>
            <td align="right" width="175x">
                Vencimento: <asp:TextBox ID="txtVencimento" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" runat="server" MaxLength="10" Width="90px" />
            </td>
            <td>
                <asp:Button ID="cmdBoleto" SkinID="botaoPequeno" Text="Gerar agendamento" OnClick="cmdBoleto_click" OnClientClick="return confirm('ATENÇÃO!\nDeseja realmente gerar os boletos e agendar geração de arquivo?');" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:GridView ID="grid" DataKeyNames="Funcional" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="true" PageSize="1000" AutoGenerateColumns="False" >
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="marcado" runat="server" Checked='<%# bool.Parse(Eval("Marcado").ToString()) %>' Enabled="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Nome" HeaderText="Nome">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NomeLocalizado" HeaderText="Localizado">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Funcional" HeaderText="Funcional">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ValorArquivo" HeaderText="Valor">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Status" HeaderText="Status">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ValorSistema" HeaderText="ValorSistema" DataFormatString="{0:N2}">
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
</asp:Content>
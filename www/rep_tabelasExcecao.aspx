<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="rep_tabelasExcecao.aspx.cs" Inherits="www.rep_tabelasExcecao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td width="65" rowspan="3"></td></tr>
        <tr><td><span class="titulo">Relatório de tabelas de exceção</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Utilize o filtro abaixo</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
   <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1" width="80">Operadora</td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboOperadora" runat="server" SkinID="dropdownSkin" Width="225"  AutoPostBack="true" OnSelectedIndexChanged="cboOperadora_OnSelectedIndexChanged" /></td>
                </tr>
            </table>
            <br />
            <font color='black' size='2'><asp:Literal runat="server" ID="litMsg" EnableViewState="false" /></font>
            <asp:GridView BorderWidth="0" cellpadding="2" cellspacing="1" ID="grid" Width="100%" SkinID="gridViewSkin" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="TabelaExecaoID,UsuarioID"
                OnRowDataBound="grid_RowDataBound" OnRowCommand="grid_RowCommand">
                <Columns>
                    <asp:BoundField DataField="UsuarioNome" HeaderText="Produtor">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField HeaderStyle-Wrap="false" DataField="TabelaComissaoNome" HeaderText="Tabela de comissionamento">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField HeaderStyle-Wrap="false" DataField="TabelaComissaoVigencia" HeaderText="Vigência da tabela de com." DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora em exceção">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ContratoAdmDescricao" HeaderText="Contrato">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TabelaExcecaoVigencia" HeaderText="Vigência exceção" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField CommandName="detalhe" Text="<img src='../images/detail2.png' alt='detalhes' border='0' />">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    </Columns>
            </asp:GridView>
            <br />
            <asp:Panel runat="server" ID="pnlTabelaExcecaoDetalhe" Visible="false">
                <table cellpadding="2" cellspacing="0" width="445" style="border: solid 1px #507CD1">
                    <tr>
                        <td class="tdNormal1" width="65">Operadora</td>
                        <td class="tdNormal1">
                            <asp:Label runat="server" ID="lblOperadora" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" width="65">Contrato</td>
                        <td class="tdNormal1">
                            <asp:Label runat="server" ID="lblContrato" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdNormal1">Dt. Vig.</td>
                        <td class="tdNormal1">
                            <asp:Label runat="server" ID="lblDataVigencia" EnableViewState="false" />
                        </td>
                    </tr>
                </table>
                <asp:GridView ID="gridItens" SkinID="gridViewSkin" width="445" runat="server" 
                    AutoGenerateColumns="False" DataKeyNames="ID">
                    <Columns>
                        <asp:TemplateField HeaderText="Parcela">
                            <ItemTemplate>
                                <asp:Label ID="txtParcela" runat="server" Text='<%# Bind("Parcela") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Percentual">
                            <ItemTemplate>
                                <asp:Label ID="txtPercentual" runat="server" Text='<%# Bind("Percentual") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Perc. Carência">
                            <ItemTemplate>
                                <asp:Label ID="txtPercentualCompraCarencia" runat="server" Text='<%# Bind("PercentualCompraCarencia") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Perc. Migração">
                            <ItemTemplate>
                                <asp:Label ID="txtPercentualMigracao" runat="server" Text='<%# Bind("PercentualMigracao") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Perc. ADM">
                            <ItemTemplate>
                                <asp:Label ID="txtPercentualADM" runat="server" Text='<%# Bind("PercentualADM") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Perc. Especial">
                            <ItemTemplate>
                                <asp:Label ID="txtPercentualEspecial" runat="server" Text='<%# Bind("PercentualEspecial") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Perc. Idade">
                            <ItemTemplate>
                                <asp:Label ID="txtIdade" runat="server" Text='<%# Bind("Idade") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <table style="border-right:solid 1px #507CD1;border-left:solid 1px #507CD1;border-bottom:solid 1px #507CD1;border-top:solid 1px #507CD1" cellpadding="2" cellspacing="0" width="445">
                    <tr>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">&nbsp;</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Vitalícia</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">A partir da parcela</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;">Percentual</td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Normal</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox Enabled="false" EnableViewState="false" ID="chkComissionamentoVitalicio" runat="server" /></td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:Label EnableViewState="false" runat="server" ID="txtComissionamentoNumeroParcelaVitalicio" />&nbsp;</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1"><asp:Label EnableViewState="false" ID="txtComissionamentoVitalicioPercentual" runat="server" />&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Compra de carência</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox Enabled="false" EnableViewState="false" ID="chkComissionamentoVitalicioCarencia" runat="server" /></td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:Label EnableViewState="false" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioCarencia" />&nbsp;</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1"><asp:Label EnableViewState="false" ID="txtComissionamentoVitalicioPercentualCarencia" runat="server" />&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Migração</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox Enabled="false" EnableViewState="false" ID="chkComissionamentoVitalicioMigracao" runat="server" /></td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:Label EnableViewState="false" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioMigracao" />&nbsp;</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1"><asp:Label EnableViewState="false" ID="txtComissionamentoVitalicioPercentualMigracao" runat="server" />&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Administrativa</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox Enabled="false" EnableViewState="false" ID="chkComissionamentoVitalicioADM" runat="server" /></td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:Label EnableViewState="false" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioADM" />&nbsp;</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1"><asp:Label EnableViewState="false" ID="txtComissionamentoVitalicioPercentualADM" runat="server" />&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Especial</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox Enabled="false" EnableViewState="false" ID="chkComissionamentoVitalicioEspecial" runat="server" /></td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:Label EnableViewState="false" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioEspecial" />&nbsp;</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1"><asp:Label EnableViewState="false" ID="txtComissionamentoVitalicioPercentualEspecial" runat="server" />&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" style="border-right:solid 1px #507CD1">Idade</td>
                        <td class="tdNormal1" align="center" style="border-right:solid 1px #507CD1"><asp:CheckBox Enabled="false" EnableViewState="false" ID="chkComissionamentoVitalicioIdade" runat="server" /></td>
                        <td class="tdNormal1" align="center" style="border-right:solid 1px #507CD1"><asp:Label EnableViewState="false" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioIdade" />&nbsp;</td>
                        <td class="tdNormal1" align="center" ><asp:Label EnableViewState="false" ID="txtComissionamentoVitalicioPercentualIdade" runat="server" />&nbsp;</td>
                    </tr>
                </table>
                <br />
                <table cellpadding="2" width="445">
                    <tr>
                        <td align="center"><asp:Button OnClick="cmdVoltarTabelaExcecao_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltarComissionamento" Text="Voltar" /></td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
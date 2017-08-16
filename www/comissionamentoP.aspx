<%@ Page Theme="Theme" Language="C#" AutoEventWireup="false" CodeBehind="comissionamentoP.aspx.cs" Inherits="www.comissionamentoP" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Tabela de Comissionamento - Detalhe</title>
    <link rel="stylesheet" type="text/css" href="css/style.css" />
</head>
<body marginleft="50" margintop="50">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="sm" runat="server"></asp:ScriptManager>
<%--         <asp:ScriptManager ID="sm" runat="server"></asp:ScriptManager>
       <asp:UpdateProgress runat="server" ID="updateProgressPoup">
            <ProgressTemplate>
                <div style="background-color:#507CD1; position:fixed; left: 50%; top: 0; width:100px; height: 15px; margin-left: -50px; text-align:center; color:white">AGUARDE...</div>
            </ProgressTemplate>
        </asp:UpdateProgress>--%>

        <table width="99%">
            <tr><td rowspan="3" width="40"><img src="images/imgTitulos/tabela_comissionario.png" /></td></tr>
            <tr>
                <td><span class="titulo">Tabela de comissionamento</span></td>
            </tr>
            <tr>
                <td><span class="subtitulo">Abaixo os detalhes da tabela de comissionamento</span></td>
            </tr>
        </table>

<%--        <asp:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
            <ContentTemplate>--%>

            <table cellpadding="2" cellspacing="1" width="445" style="border: solid 1px #507CD1">
                <%--<tr>
                    <td class="tdPrincipal1" width="110">Descrição</td>
                    <td class="tdNormal1">
                        <asp:TextBox CssClass="textbox" Width="240px" runat="server" ID="txtNome" MaxLength="180" ReadOnly="True" BackColor="#CBC9CD" />
                    </td>
                </tr>--%>

                <tr>
                    <td class="tdPrincipal1" width="110">Categoria</td>
                    <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" Width="245px" runat="server" ID="cboCategoriaComissionamento" Enabled="False" BackColor="#CBC9CD" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Idade especial</td>
                    <td class="tdNormal1"><asp:TextBox SkinID="textboxSkin" Width="29px" runat="server" ID="txtIdade" MaxLength="3" ReadOnly="true" BackColor="#CBC9CD" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Data</td>
                    <td class="tdNormal1">
                        <asp:TextBox CssClass="textbox" Width="59px" runat="server" 
                            ID="txtData" MaxLength="10" ReadOnly="True" BackColor="#CBC9CD" /></td>
                </tr>
            </table>
            <br />

            <asp:GridView ID="gridContratos" SkinID="gridViewSkin" width="445" runat="server" AutoGenerateColumns="False" DataKeyNames="ID,OperadoraID,ContratoGrupo_GrupoID" OnRowCommand="gridContratos_RowCommand">
                <Columns>
                    <asp:BoundField DataField="OperadoraDescricao" HeaderText="Operadora">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Descricao" HeaderText="Contrato">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField Text="<img src='../images/detail2.png' alt='detalhes' border='0' />" CommandName="detalhe" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <br />
            <asp:Panel ID="pnl" runat="server" Visible="false">
                <asp:Panel ID="pnl2" runat="server">
                <asp:GridView ID="gridItens" SkinID="gridViewSkin" width="445" runat="server" 
                    AutoGenerateColumns="False" DataKeyNames="ID">
                    <Columns>
                        <asp:TemplateField HeaderText="Parcela">
                            <ItemTemplate>
                                <asp:Label Width="20" ID="txtParcela" runat="server" Text='<%# Bind("Parcela") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Percentual">
                            <ItemTemplate>
                                <asp:Label Width="40" ID="txtPercentual" runat="server" Text='<%# Bind("Percentual") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Perc. Carência">
                            <ItemTemplate>
                                <asp:Label Width="40" ID="txtPercentualCompraCarencia" runat="server" Text='<%# Bind("PercentualCompraCarencia") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Perc. Migração">
                            <ItemTemplate>
                                <asp:Label Width="40" ID="txtPercentualMigracao" runat="server" Text='<%# Bind("PercentualMigracao") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Perc. ADM">
                            <ItemTemplate>
                                <asp:Label Width="40" ID="txtPercentualADM" runat="server" Text='<%# Bind("PercentualADM") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Perc. Especial">
                            <ItemTemplate>
                                <asp:Label Width="40" ID="txtPercentualEspecial" runat="server" Text='<%# Bind("PercentualEspecial") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Perc. Idade">
                            <ItemTemplate>
                                <asp:Label Width="40" ID="txtIdade" runat="server" Text='<%# Bind("Idade") %>' />
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
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">A partir da parcela</td>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;">Percentual</td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Normal</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox Enabled="false" EnableViewState="false" ID="chkComissionamentoVitalicio" runat="server" /></td>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox Enabled="false" EnableViewState="false" BackColor="LightGray" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicio" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeNumeroParcelaVitalicio" TargetControlID="txtComissionamentoNumeroParcelaVitalicio" /></td>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox Enabled="false" BackColor="LightGray" EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentual" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Compra de carência</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox Enabled="false" EnableViewState="false" ID="chkComissionamentoVitalicioCarencia" runat="server" /></td>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox Enabled="false" BackColor="LightGray" EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioCarencia" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioCarencia" TargetControlID="txtComissionamentoNumeroParcelaVitalicioCarencia" /></td>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox Enabled="false" BackColor="LightGray" EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualCarencia" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Migração</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox Enabled="false" EnableViewState="false" ID="chkComissionamentoVitalicioMigracao" runat="server" /></td>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox Enabled="false" BackColor="LightGray" EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioMigracao" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioMigracao" TargetControlID="txtComissionamentoNumeroParcelaVitalicioMigracao" /></td>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox Enabled="false" BackColor="LightGray" EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualMigracao" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Administrativa</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox Enabled="false" EnableViewState="false" ID="chkComissionamentoVitalicioADM" runat="server" /></td>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox Enabled="false" BackColor="LightGray" EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioADM" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioADM" TargetControlID="txtComissionamentoNumeroParcelaVitalicioADM" /></td>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox Enabled="false" BackColor="LightGray" EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualADM" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" style="border-right:solid 1px #507CD1">Especial</td>
                        <td class="tdNormal1" align="center" style="border-right:solid 1px #507CD1"><asp:CheckBox Enabled="false" EnableViewState="false" ID="chkComissionamentoVitalicioEspecial" runat="server" /></td>
                        <td class="tdNormal1" style="border-right:solid 1px #507CD1"><asp:TextBox Enabled="false" BackColor="LightGray" EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioEspecial" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioEspecial" TargetControlID="txtComissionamentoNumeroParcelaVitalicioEspecial" /></td>
                        <td class="tdNormal1"><asp:TextBox Enabled="false" BackColor="LightGray" EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualEspecial" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Idade</td>
                        <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox Enabled="false" EnableViewState="false" ID="chkComissionamentoVitalicioIdade" runat="server" /></td>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox Enabled="false" BackColor="LightGray" EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioIdade" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioIdade" TargetControlID="txtComissionamentoNumeroParcelaVitalicioIdade" /></td>
                        <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox Enabled="false" BackColor="LightGray" EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualIdade" runat="server" /></td>
                    </tr>
                </table>
                </asp:Panel>
            </asp:Panel>


<%--            </ContentTemplate>
        </asp:UpdatePanel>--%>
    </form>
</body>
</html>
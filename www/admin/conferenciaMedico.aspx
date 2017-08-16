<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="conferenciaMedico.aspx.cs" Inherits="www.admin.conferenciaMedico" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"></td></tr>
        <tr><td><span class="titulo">Propostas para conferência</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo as propostas para conferência</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" width="550px" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1" colspan="2" width="180px">Parâmetros</td>
                    <td class="tdPrincipal1"></td>
                </tr>
                <tr>
                    <td class="tdNormal1">
                        <asp:RadioButton ID="optSem" Text="Com CID não informado" runat="server" GroupName="a" Checked="true" />
                    </td>
                    <td class="tdNormal1">
                        <asp:RadioButton ID="optCom" Text="Com CID informado em" runat="server" GroupName="a" />&nbsp;
                        <asp:TextBox ID="txtData" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                    </td>
                    <td class="tdNormal1" align="center">
                        <asp:Button ID="cmdLocalizar" Text="Localizar" runat="server" 
                            SkinID="botaoAzulBorda" onclick="cmdLocalizar_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:GridView ID="grid" Width="100%" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False"  
                DataKeyNames="ID" OnRowCommand="grid_RowCommand" 
                OnRowDataBound="grid_RowDataBound" onpageindexchanging="grid_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="ContratoNumero" HeaderText="Proposta" ItemStyle-Width="20">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="BeneficiarioNome" HeaderText="Beneficiário" ItemStyle-Width="30">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="BeneficiarioSexo" HeaderText="Sexo" ItemStyle-Width="20">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="BeneficiarioIdade" HeaderText="Idade" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="BeneficiarioAlturaPeso" HeaderText="Al/Pe" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ItemSaudeInstanciaData" ItemStyle-Width="20" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ItemSaudeInstanciaDescricao" ItemStyle-Font-Size="7pt" HeaderText="Evento">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="CID Inicial" ItemStyle-Width="30">
                        <ItemTemplate>
                            <asp:TextBox Width="30" MaxLength="4" SkinID="textboxSkin" ID="txtCIDInicial" runat="server" Text='<%# Bind("CIDInicial") %>' />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="CID Final" ItemStyle-Width="30">
                        <ItemTemplate>
                            <asp:TextBox Width="30" MaxLength="4" SkinID="textboxSkin" ID="txtCIDFinal" runat="server" Text='<%# Bind("CIDFinal") %>' />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Aprovado" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="40">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkAprovado" runat="server" Checked='<%# Bind("AprovadoMedico") %>' />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="OBS" ItemStyle-Width="90">
                        <ItemTemplate>
                            <asp:TextBox Width="190" MaxLength="500" TextMode="MultiLine" Height="50" SkinID="textboxSkin" ID="txtOBS" runat="server" Text='<%# Bind("OBSMedico") %>' />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <table id="tblSalvar" visible="false" runat="server" cellpadding="2" cellspacing="0" width="100%">
                <tr height="18">
                    <td height="18" align="right" valign="bottom">Aprovado em</td>
                    <td height="18"></td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:TextBox ID="txtDataSalvar" runat="server" SkinID="textboxSkin" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                    </td>
                    <td width="55"><asp:Button ID="cmdSalvar" Text="Salvar" OnClientClick="return confirm('Deseja realmente prosseguir?');" SkinID="botaoAzulBorda" runat="server" Width="65" onclick="cmdSalvar_Click" /></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
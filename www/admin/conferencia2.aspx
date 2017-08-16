<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="conferencia2.aspx.cs" Inherits="www.admin.conferencia3" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register src="../usercontrols/ucConferenciaPainelMensagem.ascx" tagname="ucConferenciaPainelMensagem" tagprefix="uc1" %>
<%@ Register src="../usercontrols/ucConferenciaPassos.ascx" tagname="ucConferenciaPassos" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <script src="../js/common.js" type="text/javascript"></script> 
    <table>
        <tr><td rowspan="3"></td></tr>
        <tr><td><span class="titulo">Conferência</span></td></tr>
        <tr><td><span class="subtitulo">Conferência de propostas</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <uc2:ucConferenciaPassos ID="ucConferenciaPassos" runat="server" />
            <br />
            <b><font color='black'>BENEFICIÁRIOS</font></b><br /><br />
            <table cellpadding="2" cellspacing="0" width="750px" style="border: solid 0px #507CD1">
                <tr>
                    <td width="85%" valign="top" >
                        <table width="394" cellpadding="1" cellspacing="0">
                            <tr>
                                <td height="24" style="border-top:solid 1px #507CD1;border-left:solid 1px #507CD1;border-right:solid 1px #507CD1" bgcolor='#D1DDF1' align="center"><font color='#507CD1' style='font-size:12px'><b>ENDEREÇO</b></font></td>
                            </tr>
                        </table>
                        <table width="394" cellpadding="3" cellspacing="1" style="border-bottom: solid 1px #507CD1;border-top: solid 1px #507CD1;border-left: solid 1px #507CD1;border-right: solid 1px #507CD1">
                            <tr>
                                <td bgcolor='#507CD1' width="105"><font color='#EFF3FB'><b>CEP</b></font></td>
                                <td bgcolor='#EFF3FB'><asp:TextBox Width="80" ID="txtCep" SkinID="textboxSkin" runat="server" /><cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeCEP" Mask="99999-999" ClearMaskOnLostFocus="true" TargetControlID="txtCep" /></td>
                                <td bgcolor='#EFF3FB'><asp:Button ID="cmdValidarCep" Text="validar" runat="server" SkinID="botaoPequeno" onclick="cmdValidarCep_Click" />&nbsp;<asp:Literal runat="server" ID="litCEP" EnableViewState="false" /></td>
                            </tr>
                            <tr>
                                <td colspan="3" bgcolor='#EFF3FB'>
                                    <asp:Literal runat="server" ID="litEndereco" EnableViewState="false" />
                                </td>
                            </tr>
                        </table>
                        <!--BENEFICIARIOS-->
                        <br />
                        <table width="394" cellpadding="1" cellspacing="0">
                            <tr>
                                <td height="24" style="border-top:solid 1px #507CD1;border-left:solid 1px #507CD1;border-right:solid 1px #507CD1" bgcolor='#D1DDF1' align="center"><font color='#507CD1' style='font-size:12px'><b>BENEFICIÁRIOS</b></font></td>
                            </tr>
                        </table>
                        <table width="394" cellpadding="2" cellspacing="1" style="border-top:solid 1px #507CD1;border-left:solid 1px #507CD1;border-right:solid 1px #507CD1" >
                            <tr>
                                <td bgcolor='#507CD1' align="right"><font color='#EFF3FB'><b>CPF</b></font></td>
                                <td bgcolor='#EFF3FB'><asp:TextBox ID="txtCpfDependente" SkinID="textboxSkin" Width="80" runat="server" /><cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeCPF" Mask="999,999,999-99" ClearMaskOnLostFocus="true" TargetControlID="txtCpfDependente" /></td>
                                <td bgcolor='#EFF3FB'><asp:Button ID="cmdValidarCpfDependente" Text="validar" runat="server" SkinID="botaoPequeno" onclick="cmdValidarCpfDependente_Click" />&nbsp;<asp:Literal runat="server" ID="litCpfDependente" EnableViewState="false" /></td>
                            </tr>
                            <tr runat="server" id="trListaDependentes" visible="false">
                                <td colspan="3" width="394">
                                    <asp:GridView ID="gridBeneficiariosDependentes" runat="server" AutoGenerateColumns="False" 
                                        ShowHeader="true" Width="100%" SkinID="gridViewSkin" DataKeyNames="ID" 
                                        BorderStyle="Dotted" BorderWidth="1px" onrowcommand="gridBeneficiariosDEPENDENTES_RowCommand">
                                        <Columns>
                                            <asp:ButtonField DataTextField= "Nome" ItemStyle-HorizontalAlign="Left" HeaderText="Selecione o beneficiário" ButtonType="Link" CommandName="sel">
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:ButtonField >
                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr runat="server" id="trNomeDependente" visible="true">
                                <td bgcolor='#507CD1' align="right"><font color='#EFF3FB'><b>Nome</b></font></td>
                                <td colspan="2" bgcolor='#EFF3FB'><asp:TextBox ID="txtNomeDependente" ReadOnly="false" Width="180" SkinID="textboxSkin" runat="server" /></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1' width="105" align="right"><font color='#EFF3FB'><b>Data nascimento</b></font></td>
                                <td colspan="2" bgcolor='#EFF3FB'><asp:TextBox ID="txtDataNascimento" Width="80" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1' align="right"><font color='#EFF3FB'><b>Parentesco</b></font></td>
                                <td colspan="2" bgcolor='#EFF3FB'><asp:DropDownList Width="184" ID="cboParentesco" SkinID="dropdownSkin" runat="server" /></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1' align="right"><font color='#EFF3FB'><b>Estado civil</b></font></td>
                                <td colspan="2" bgcolor='#EFF3FB'><asp:DropDownList Width="184" ID="cboEstadoCivil" SkinID="dropdownSkin" runat="server" /></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1' align="right"><font color='#EFF3FB'><b>Data casamento</b></font></td>
                                <td colspan="2" bgcolor='#EFF3FB'><asp:TextBox ID="txtDataCasamento" Width="80" SkinID="textboxSkin" runat="server" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1' align="right"><font color='#EFF3FB'><b>Peso</b></font></td>
                                <td colspan="2" bgcolor='#EFF3FB'>
                                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="40" bgcolor='#EFF3FB'><asp:TextBox SkinID="textboxSkin" runat="server" ID="txtPeso" Width="30" /></td>
                                            <td bgcolor='#507CD1' width="40">&nbsp;<font color='#EFF3FB'><b>Altura</b></font></td>
                                            <td bgcolor='#EFF3FB'>&nbsp;<asp:TextBox SkinID="textboxSkin" runat="server" ID="txtAltura" Width="30" /></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table width="394" cellpadding="1" cellspacing="0">
                            <tr>
                                <td height="24" style="border-top:solid 1px #507CD1;border-bottom:solid 1px #507CD1;border-left:solid 1px #507CD1;border-right:solid 1px #507CD1" bgcolor='#D1DDF1' align="center"><font color='#507CD1' style='font-size:12px'><b>Adicionais</b></font></td>
                            </tr>
                            <tr>
                                <td style="border-bottom:solid 1px #507CD1;border-left:solid 1px #507CD1;border-right:solid 1px #507CD1" align="center">
                                    <asp:GridView ID="gridAdicionais" runat="server" AutoGenerateColumns="False" ShowHeader="False" Width="390" SkinID="gridViewSkin" DataKeyNames="ID" BorderStyle="None">
                                        <Columns>
                                            <asp:BoundField DataField="Descricao">
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:TemplateField>
                                                <ItemStyle Width="1%" />
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkCheck" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView><asp:Literal ID="litNenhumAdicional" Text="<i>(nenhum)</i>" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td style="border-bottom:solid 1px #507CD1;border-left:solid 1px #507CD1;border-right:solid 1px #507CD1" align="right">
                                    <asp:Button ID="cmdAdicionarBeneficiario" Text="adicionar" runat="server" SkinID="botaoAzulBorda" Enabled="true" OnClick="cmdAdicionarBeneficiario_Click" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <!--BENEFICIÁRIOS ADICIONADOS-->
                        <table id="tblAdicionados" width="100%" cellpadding="2" cellspacing="1" runat="server">
                            <tr>
                                <td>
                                    <table bgcolor='#D1DDF1' style="border-left: solid 1px #507CD1;border-right: solid 1px #507CD1;border-top: solid 1px #507CD1" cellpadding="2" cellspacing="2" width="394">
                                        <tr>
                                            <td align="center"><font color='#507CD1' style='font-size:12px'><b>BENEFICIÁRIOS ADICIONADOS</b></font></td>
                                        </tr>
                                    </table>
                                   <asp:GridView ID="gridAdicionados" runat="server" AutoGenerateColumns="False" Width="394" SkinID="gridViewSkin" DataKeyNames="ID" 
                                        OnRowCommand="gridAdicionados_RowCommand" OnRowDataBound="gridAdicionados_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="Nome" HeaderText="Nome">
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="ParentescoDescricao" HeaderText="Tipo">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Idade" HeaderText="Idade">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C}">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' align='middle' title='excluir' alt='excluir' border='0' />">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle Width="1%" />
                                            </asp:ButtonField>
                                        </Columns>
                                    </asp:GridView>
                                    <br />
                                    <asp:Literal runat="server" ID="litTotal" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td width="200"></td>
                    <td valign="top">
                        <uc1:ucConferenciaPainelMensagem ID="ucCPM" runat="server" />
                    </td>
                </tr>
            </table>
            <br />
            <table cellpadding="2" cellspacing="0" width="750px" style="border: solid 1px #507CD1">
                <tr>
                    <td align="left"><asp:Button runat="server" ID="cmdVoltar" Text="Voltar" Width="80" SkinID="botaoAzul" onclick="cmdVoltar_Click" /></td>
                    <td align="right"><asp:Button runat="server" ID="cmdProximo" Text="Próximo" Width="80" SkinID="botaoAzul" onclick="cmdProximo_Click" /></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
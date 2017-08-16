<%@ Page Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="user.aspx.cs" Inherits="www.user" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="content" ContentPlaceHolderID="cphContent" runat="server">
    <table width="70%">
        <tr>
            <td><span class="titulo">Agente do sistema</span></td>
        </tr>
    </table>
    <br />
    <cc1:TabContainer BorderStyle="None" BorderWidth="0" Width="100%" ID="tab" runat="server" ActiveTabIndex="0" >
        <cc1:TabPanel runat="server" ID="p1">
            <HeaderTemplate><font color="black">Dados comuns</font></HeaderTemplate>
            <ContentTemplate>
                <table cellpadding="2">
                    <tr>
                        <td class="tdPrincipal1" width="88px">Tipo</td>
                        <td class="tdNormal1">
                            <asp:DropDownList CssClass="textbox" Width="206px" runat="server" 
                                ID="cboTipoPessoa" MaxLength="150">
                                <asp:ListItem Text="FÍSICA" Value="0" Selected="True" />
                                <asp:ListItem Text="JURÍDICA" Value="1" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1">Nome</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" Width="200px" runat="server" 
                                ID="txtNome" MaxLength="150" /></td>
                    </tr>
                </table>
                <table cellpadding="2">
                    <tr>
                        <td class="tdPrincipal1" width="88px"><asp:Label runat="server" ID="lblDataNascimentoFundacao" Text="Nascimento" /></td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" 
                                ID="txtNascimentoFundacao" Width="65px" MaxLength="10" /></td>
                        <td class="tdPrincipal1">&nbsp;<asp:Label runat="server" ID="lblCpfCnpj" Text="CPF" /></td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" 
                                ID="txtCpfCnpj" Width="92px" MaxLength="14" /></td>
                    </tr>
                </table>
                <br />
                <table cellpadding="2">
                    <tr><td colspan="6"><b>Dados de contato</b></td></tr>
                    <tr>
                        <td class="tdPrincipal1">DDD</td>
                        <td class="tdNormal1" width="50"><asp:TextBox CssClass="textbox" runat="server" 
                                ID="txtDDD1" Width="30px" MaxLength="3" /></td>
                        <td class="tdPrincipal1">Fone</td>
                        <td class="tdNormal1" width="80"><asp:TextBox CssClass="textbox" runat="server" 
                                ID="txtFone1" Width="62px" MaxLength="9" /></td>
                        <td class="tdPrincipal1">Ramal</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtRamal1" 
                                Width="40px" MaxLength="4" /></td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1">DDD</td>
                        <td class="tdNormal1" width="50"><asp:TextBox CssClass="textbox" runat="server" 
                                ID="txtDDD2" Width="30px" MaxLength="3" /></td>
                        <td class="tdPrincipal1">Fone</td>
                        <td class="tdNormal1" width="80"><asp:TextBox CssClass="textbox" runat="server" 
                                ID="txtFone2" Width="62px" MaxLength="9" /></td>
                        <td class="tdPrincipal1">Ramal</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtRamal2" 
                                Width="40px" MaxLength="4" /></td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1">DDD</td>
                        <td class="tdNormal1" width="50"><asp:TextBox CssClass="textbox" runat="server" 
                                ID="txtDDD3" Width="30px" MaxLength="3" /></td>
                        <td class="tdPrincipal1">Fone</td>
                        <td class="tdNormal1" width="80"><asp:TextBox CssClass="textbox" runat="server" 
                                ID="txtFone3" Width="62px" MaxLength="9" /></td>
                        <td class="tdPrincipal1">Ramal</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtRamal3" 
                                Width="40px" MaxLength="4" /></td>
                    </tr>
                </table>
                <br />
                <table cellpadding="2">
                    <tr>
                        <td class="tdPrincipal1">Email&nbsp;</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" Width="246px" runat="server" ID="txtEmail" MaxLength="65" /></td>
                    </tr>
                </table>
                <br />
                <table cellpadding="2">
                    <tr>
                        <td class="tdPrincipal1" colspan="4"><b>Endereço</b></td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1" width="59px">CEP</td>
                        <td class="tdNormal1" colspan="3"><asp:TextBox CssClass="textbox" runat="server" ID="txtCEP" Width="65px" MaxLength="9" /></td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1">Logradouro</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtLogradouro" Width="290px" MaxLength="300" /></td>
                        <td class="tdPrincipal1">&nbsp;Número</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtNumero" Width="65px" MaxLength="9" /></td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1">Complemento</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtComplemento" Width="200px" MaxLength="250" /></td>
                        <td class="tdPrincipal1" width="72px">&nbsp;Bairro</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtBairro" Width="190px" MaxLength="300" /></td>
                    </tr>
                </table>
                <table cellpadding="2">
                    <tr>
                        <td class="tdPrincipal1" width="67px">Cidade</td>
                        <td class="tdNormal1" width="293"><asp:TextBox CssClass="textbox" runat="server" ID="txtCidade" Width="200px" MaxLength="300" /></td>
                        <td class="tdPrincipal1" width="72px">UF</td>
                        <td class="tdNormal1" width="195"><asp:TextBox CssClass="textbox" runat="server" ID="txtUF" Width="20px" MaxLength="2" /></td>
                    </tr>
                </table>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel BorderStyle="None" ID="p2" runat="server" HeaderText="p2">
            <HeaderTemplate><font color="black">Comissionamento</font></HeaderTemplate>
            <ContentTemplate>
                <table cellpadding="2" width="70%">
                    <tr>
                        <td class="tdPrincipal1" width="130">Operadora</td>
                        <td class="tdNormal1">
                            <asp:DropDownList CssClass="textbox" Width="256px" runat="server" ID="cboOperadora">
                                <asp:ListItem Text="UNIMED PAULISTANA" Value="0" Selected="True" />
                                <asp:ListItem Text="SUL AMERICA" Value="1" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1">Comissionamento</td>
                        <td class="tdNormal1">
                            <asp:DropDownList CssClass="textbox" Width="256px" runat="server" ID="cboComissionamentoModelo">
                                <asp:ListItem Text="UNIMED - MODELO1" Value="0" Selected="True" />
                                <asp:ListItem Text="UNIMED - MODELO2" Value="1" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <br />
                Tabelas de comissionamento
                <asp:GridView Font-Size="10px" Width="70%" ID="gridComissionamento" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" BorderWidth="1px" BorderColor="#507CD1" GridLines="None" onrowcommand="gridComissionamento_RowCommand">
                    <RowStyle BackColor="#EFF3FB" Font-Size="10px" />
                    <Columns>
                        <asp:BoundField DataField="Código" HeaderText="Código">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Comissionamento" HeaderText="Descrição">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Ativo" HeaderText="Ativo">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:ButtonField CommandName="detalhes" Text="detalhes">
                        <HeaderStyle HorizontalAlign="Center" />
                        </asp:ButtonField>
                    </Columns>
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#2461BF" />
                    <AlternatingRowStyle  Font-Size="10px" BackColor="White" />
                </asp:GridView>
                <br />
                Detalhes
                <br />
                <asp:GridView Font-Size="10px" Width="40%" ID="gridComissionamentoDetalhe" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" BorderWidth="1px" BorderColor="#507CD1" GridLines="None">
                    <RowStyle BackColor="#EFF3FB" Font-Size="10px" />
                    <Columns>
                        <asp:BoundField DataField="Parcela" HeaderText="Parcela">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Imposto" HeaderText="Imposto">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Comissão" HeaderText="Comissão">
                        <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:ButtonField ControlStyle-Font-Size="10px" CommandName="detalhes" Text="salvar">
                        <HeaderStyle HorizontalAlign="Center" />
                        </asp:ButtonField>
                    </Columns>
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#2461BF" />
                    <AlternatingRowStyle  Font-Size="10px" BackColor="White" />
                </asp:GridView>
            </ContentTemplate>
        </cc1:TabPanel>
        <cc1:TabPanel ID="p3" runat="server">
            <HeaderTemplate><font color="black">Acesso</font></HeaderTemplate>
            <ContentTemplate>
                <table cellpadding="2" width="34%">
                    <tr>
                        <td class="tdPrincipal1" width="29%">Tipo</td>
                        <td class="tdNormal1">
                            <asp:DropDownList CssClass="textbox" Width="190" runat="server" ID="cboTipo" MaxLength="150">
                                <asp:ListItem Text="Corretor" Value="2" Selected="True" />
                                <asp:ListItem Text="Supervisor" Value="1" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1">E-mail</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" Width="190" runat="server" ID="txtEmailLogin" MaxLength="65" /></td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1">Senha</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" TextMode="Password" Width="100" runat="server" ID="txtSenha" MaxLength="20" /></td>
                    </tr>
                    <tr>
                        <td class="tdPrincipal1">Confirme</td>
                        <td class="tdNormal1"><asp:TextBox CssClass="textbox" TextMode="Password" Width="100" runat="server" ID="txtSenhaConfirma" MaxLength="20" /></td>
                    </tr>
                </table>
            </ContentTemplate>
        </cc1:TabPanel>
    </cc1:TabContainer>
</asp:Content>
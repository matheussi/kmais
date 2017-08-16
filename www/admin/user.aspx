<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="user.aspx.cs" Inherits="www.admin.user" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3" width="30"><img src="../images/icones/produtores.png" alt="" /></td></tr>
        <tr>
            <td><span class="titulo">Produtor</span></td>
        </tr>
        <tr>
            <td nowrap><span class="subtitulo">Preencha os campos abaixo e clique em "Salvar"</span></td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
<script language="javascript" type="text/javascript" src="../js/common.js"></script>
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1" width="425" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1" width="139">Perfil</td>
                    <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" Width="265px" AutoPostBack="true" runat="server" ID="cboPerfil" onselectedindexchanged="cboPerfil_SelectedIndexChanged" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Nome</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" ID="txtNome" Width="260" MaxLength="200" SkinID="textboxSkin" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Apelido</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" ID="txtApelido" Width="100" MaxLength="149" SkinID="textboxSkin" /></td>
                </tr>
                <tr runat="server" enableviewstate="false" id="trCategoriaComissionamento" visible="false">
                    <td class="tdPrincipal1">Categoria comissionamento</td>
                    <td class="tdNormal1"><asp:DropDownList SkinID="dropdownSkin" Width="265px" AutoPostBack="false" runat="server" ID="cboCategoriaComissionamento" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">E-mail</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" ID="txtEmail" Width="260" MaxLength="70" SkinID="textboxSkin" /></td>
                </tr>
                <tr>
                    <td class="tdPrincipal1">Marca ótica</td>
                    <td class="tdNormal1"><asp:TextBox runat="server" ID="txtMarcaOtica" Width="260" MaxLength="70" SkinID="textboxSkin" /></td>
                </tr>
            </table>
            <br />
            <cc1:TabContainer BackColor="White" BorderStyle="None" BorderWidth="0" Width="100%" ID="tab" runat="server" ActiveTabIndex="0" >
                <cc1:TabPanel runat="server" ID="p1">
                    <HeaderTemplate><font color="black">Cadastro</font></HeaderTemplate>
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="1" width="425" style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdPrincipal1" width="129">Tipo</td>
                                <td class="tdNormal1">
                                    <asp:RadioButton ID="optFisica" Text="Pessoa Física" GroupName="a" 
                                        runat="server" Checked="True" AutoPostBack="True" 
                                        OnCheckedChanged="optChanged" />&nbsp;
                                    <asp:RadioButton ID="optJuridica" Text="Pessoa Jurídica" GroupName="a" 
                                        runat="server" AutoPostBack="True" OnCheckedChanged="optChanged" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" ><asp:Label ID="lblCPFouCNPJ" runat="server" Text="CPF" /></td>
                                <td class="tdNormal1">
                                    <asp:TextBox ID="txtCPFouCNPJ" Text="CPF" Width="112px" SkinID="textboxSkin" 
                                        runat="server"  />
                                    <cc1:MaskedEditExtender EnableViewState="true" TargetControlID="txtCPFouCNPJ" 
                                        Mask="999,999,999-99" runat="server" ID="meeCPF" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="129"><asp:Label ID="lblRGouIE" runat="server" Text="RG" /></td>
                                <td class="tdNormal1"><asp:TextBox ID="txtRGouIE" Width="112px" 
                                        SkinID="textboxSkin" runat="server" MaxLength="55" /></td>
                            </tr>
                         </table>
                        <br />
                        <table runat="server" id="tblDadosPessoaFisica" cellpadding="2" cellspacing="1" width="425" style="border: solid 1px #507CD1">
                            <tr runat="server">
                                <td class="tdPrincipal1" width="129" runat="server">Data de Nascimento</td>
                                <td class="tdNormal1" runat="server">
                                    <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtDataNascimento" Width="60px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                </td>
                            </tr>
                            <tr runat="server">
                                <td class="tdPrincipal1" runat="server">Sexo</td>
                                <td class="tdNormal1" runat="server"><asp:DropDownList SkinID="dropdownSkin" 
                                        Width="107px" runat="server" ID="cboSexo" /></td>
                            </tr>
                            <tr runat="server">
                                <td class="tdPrincipal1" runat="server">Estado Civil</td>
                                <td class="tdNormal1" runat="server"><asp:DropDownList SkinID="dropdownSkin" 
                                        Width="107px" runat="server" ID="cboEstadoCivil" /></td>
                            </tr>
                         </table>
                        <asp:Literal runat="server" ID="litBr" Text="<br>" />
                        <table cellpadding="2" cellspacing="1" width="425" style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdPrincipal1" width="80px">DDD</td>
                                <td class="tdNormal1" width="50">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtDDD1" Width="30px" MaxLength="3" />
                                    <cc1:MaskedEditExtender EnableViewState="False" TargetControlID="txtDDD1" 
                                        Mask="99" runat="server" ID="meeDDD1" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
                                </td>
                                <td class="tdPrincipal1" width="50">Fone</td>
                                <td class="tdNormal1" width="80">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtFone1" Width="62px" MaxLength="9" />
                                    <cc1:MaskedEditExtender EnableViewState="False" TargetControlID="txtFone1" 
                                        Mask="9999-9999" runat="server" ID="meeFone1" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
                                </td>
                                <td class="tdPrincipal1" width="50">Ramal</td>
                                <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtRamal1" Width="40px" MaxLength="4" /></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">DDD</td>
                                <td class="tdNormal1" width="50">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtDDD2" Width="30px" MaxLength="3" />
                                    <cc1:MaskedEditExtender EnableViewState="False" TargetControlID="txtDDD2" 
                                        Mask="99" runat="server" ID="meeDDD2" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
                                </td>
                                <td class="tdPrincipal1">Fone</td>
                                <td class="tdNormal1" width="80">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtFone2" Width="62px" MaxLength="9" />
                                    <cc1:MaskedEditExtender EnableViewState="False" TargetControlID="txtFone2" 
                                        Mask="9999-9999" runat="server" ID="meeFone2" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
                                </td>
                                <td class="tdPrincipal1">Ramal</td>
                                <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtRamal2" 
                                        Width="40px" MaxLength="4" /></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">DDD</td>
                                <td class="tdNormal1" width="50">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtDDDCelular" Width="30px" MaxLength="3" />
                                    <cc1:MaskedEditExtender EnableViewState="False" TargetControlID="txtDDDCelular" 
                                        Mask="99" runat="server" ID="meeDDDCelular" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
                                </td>
                                <td class="tdPrincipal1">Celular</td>
                                <td class="tdNormal1">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtCelular" Width="62px" MaxLength="9" />
                                    <cc1:MaskedEditExtender EnableViewState="False" TargetControlID="txtCelular" 
                                        Mask="9999-9999" runat="server" ID="meeCelular" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
                                </td>
                                <td class="tdPrincipal1">Operadora</td>
                                <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtCelularOperadora" Width="80px" MaxLength="99" /></td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="2" cellspacing="1" width="425" style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdPrincipal1">CEP</td>
                                <td class="tdNormal1" colspan="3">
                                    <asp:TextBox CssClass="textbox" runat="server" ID="txtCEP" Width="65px" MaxLength="9" />
                                    <asp:ImageButton runat="server" EnableViewState="False" ToolTip="checar CEP" 
                                        ImageUrl="~/images/endereco.png" ID="cmdBuscaEndereco" 
                                        OnClick="cmdBuscaEndereco_Click" />
                                    <cc1:MaskedEditExtender TargetControlID="txtCEP" Mask="99999-999" 
                                        runat="server" ID="meeCEP" EnableViewState="False" CultureAMPMPlaceholder="" 
                                        CultureCurrencySymbolPlaceholder="" CultureDateFormat="" 
                                        CultureDatePlaceholder="" CultureDecimalPlaceholder="" 
                                        CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True" />
                                </td>
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
                            <tr>
                                <td class="tdPrincipal1" width="80px">Cidade</td>
                                <td class="tdNormal1" width="293"><asp:TextBox CssClass="textbox" runat="server" ID="txtCidade" Width="200px" MaxLength="300" /></td>
                                <td class="tdPrincipal1" width="72px">&nbsp;UF</td>
                                <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtUF" Width="20px" MaxLength="2" /></td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="2" cellspacing="1" width="425" style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdPrincipal1" width="129">Entrevistado por</td>
                                <td class="tdNormal1"><asp:TextBox runat="server" SkinID="textboxSkin" ID="txtEntrevistadoPor" Width="98%" /></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1">Entrevistado em</td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" SkinID="textboxSkin" ID="txtEntrevistadoEm" Width="60px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="2" cellspacing="1" width="425" style="border: solid 1px #507CD1">
                            <tr>
                                <td colspan="2" bgcolor='gainsboro' align="center" ><b>Dados para depósito bancário</b></td>
                            </tr>

                            <tr>
                                <td class="tdPrincipal1" width="129">Banco</td>
                                <td class="tdNormal1"><asp:TextBox ID="txtBanco" Width="175px" MaxLength="100" 
                                        SkinID="textboxSkin" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="129">Agência</td>
                                <td class="tdNormal1"><asp:TextBox ID="txtAgencia" MaxLength="50" Width="80px" 
                                        SkinID="textboxSkin" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="129"><asp:DropDownList ID="cboTipoContaBancaria" SkinID="dropdownSkin" Width="99%" runat="server" /></td>
                                <td class="tdNormal1"><asp:TextBox ID="txtContaCorrente" MaxLength="50" 
                                        Width="80px" SkinID="textboxSkin" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="tdPrincipal1" width="129">Favorecido</td>
                                <td class="tdNormal1"><asp:TextBox ID="txtFavorecido" MaxLength="250" Width="175px" 
                                        SkinID="textboxSkin" runat="server" /></td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="2" cellspacing="1" width="425" style="border: solid 1px #507CD1">
                            <tr>
                                <td bgcolor='gainsboro' align="center" ><b>Observações</b></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtObs" Width="99%" Height="70px" SkinID="textboxSkin" 
                                        TextMode="MultiLine" runat="server" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <table cellpadding="2" width="306">
                            <tr>
                                <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="False" ID="cmdVoltar" Text="Voltar" /></td>
                                <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="true" ID="cmdSalvar" Text="Salvar" /></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </cc1:TabPanel>
                
                <cc1:TabPanel runat="server" ID="p1a">
                    <HeaderTemplate><font runat="server" id="titP1a" color="black">Filiais</font></HeaderTemplate>
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="0" width="445" style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdPrincipal1" colspan="3">Filiais</td>
                            </tr>
                            <tr>
                                <td class="tdNormal1" width="65">Filial</td>
                                <td class="tdNormal1">
                                    <asp:DropDownList SkinID="dropdownSkin" Width="300" runat="server" ID="cboFilial" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdNormal1">Data</td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" SkinID="textboxSkin" EnableViewState="false" ID="txtDataFilial" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                    &nbsp;
                                    <asp:LinkButton EnableViewState="true" ID="cmdAdicionarFilial" Text="adicionar" runat="server" OnClick="cmdAdicionarFilial_Click" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <asp:GridView ID="gridFiliais" Width="445" SkinID="gridViewSkin" runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,FilialID"
                            OnRowDataBound="gridFiliais_RowDataBound" OnRowCommand="gridFiliais_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="FilialNome" HeaderText="Filial">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                                </Columns>
                        </asp:GridView>
                        <br />
                        <table cellpadding="2" width="445">
                            <tr>
                                <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltarFilial" Text="Voltar" /></td>
                                <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="true" ID="Button1" Text="Salvar" /></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </cc1:TabPanel>
                
                <cc1:TabPanel runat="server" ID="p2">
                    <HeaderTemplate><font runat="server" id="titP2" color="black">Comissionamento</font></HeaderTemplate>
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="0" width="445">
                            <tr><td><span class="subtitulo">Tabelas de comissionamento</span></td></tr>
                        </table>
                        <table cellpadding="2" cellspacing="0" width="445" style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdNormal1" width="65">Tabela</td>
                                <td class="tdNormal1">
                                    <asp:DropDownList SkinID="dropdownSkin" Width="300" runat="server" ID="cboTabela" />
                                </td>
                            </tr>
                            <tr runat="server" id="trTabelaGrupoVenda">
                                <td class="tdNormal1">Grupo de venda</td>
                                <td class="tdNormal1"><asp:DropDownList runat="server" ID="cboTabelaGrupoVenda" SkinID="dropdownSkin" Width="300" /></td>
                            </tr>
                            <tr>
                                <td class="tdNormal1">Dt. Vig.</td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" SkinID="textboxSkin" EnableViewState="false" ID="txtDataVigencia" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                    &nbsp;
                                    <asp:LinkButton EnableViewState="true" ID="cmdAdicionar" Text="adicionar" runat="server" OnClick="cmdAdicionar_Click" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <asp:GridView ID="gridTabelasDoUsuario" Width="445" SkinID="gridViewSkin" runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,TabelaComissionamentoID,GrupoVendaID"
                            OnRowDataBound="gridTabelasDoUsuario_RowDataBound" OnRowCommand="gridTabelasDoUsuario_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="TabelaComissionamentoNome" Visible="false" HeaderText="Tabela">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="TabelaComissionamentoCategoriaNome" HeaderText="Tabela">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField HeaderStyle-Wrap="false" DataField="TabelaComissionamentoData" HeaderText="Data tabela" DataFormatString="{0:dd/MM/yyyy}">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Data" HeaderText="Vigência" DataFormatString="{0:dd/MM/yyyy}">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="GrupoVendaDescricao" HeaderText="Grupo de venda">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="PerfilDescricao" HeaderText="Perfil">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                                <asp:ButtonField CommandName="editar" Text="<img src='../images/detail2.png' title='detalhes' alt='detalhes' border='0' />">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>

                                <asp:ButtonField CommandName="tabelaExcecao" Text="<img src='../images/new_generic.png' title='criar tabela de exceção' alt='criar tabela de exceção' border='0' />">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <table cellpadding="2" cellspacing="0" width="445">
                            <tr>
                                <td><span class="subtitulo">Tabelas de exceção<asp:Literal ID="litTabelaExcecaoLista" runat="server" EnableViewState="false" /></span></td>
                            </tr>
                        </table>
                        <asp:Panel runat="server" ID="pnlTabelaExcecaoLista">
                            <asp:GridView ID="gridTabelasDeExcecao" Width="445" SkinID="gridViewSkin" runat="server" AllowPaging="False" AutoGenerateColumns="False"  DataKeyNames="ID,OperadoraID,ContratoAdmID"
                                OnRowDataBound="gridTabelasDeExcecao_RowDataBound" OnRowCommand="gridTabelasDeExcecao_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="OperadoraNome" Visible="true" HeaderText="Operadora">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField HeaderStyle-Wrap="false" DataField="ContratoAdmDescricao" HeaderText="Contrato">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Vigencia" HeaderText="Vigência" DataFormatString="{0:dd/MM/yyyy}">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                    <asp:ButtonField CommandName="editar" Text="<img src='../images/detail2.png' title='detalhes' alt='detalhes' border='0' />">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlTabelaExcecaoDetalhe" Visible="false">
                            <table cellpadding="2" cellspacing="0" width="445" style="border: solid 1px #507CD1">
                                <tr>
                                    <td class="tdNormal1" width="65">Operadora</td>
                                    <td class="tdNormal1">
                                        <asp:DropDownList SkinID="dropdownSkin" Width="300" runat="server" ID="cboTabelaExcecaoOperadora" AutoPostBack="true" OnSelectedIndexChanged="cboTabelaExcecaoOperadora_OnSelectedIndexChanged" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1" width="65">Contrato</td>
                                    <td class="tdNormal1">
                                        <asp:DropDownList SkinID="dropdownSkin" Width="300" runat="server" ID="cboTabelaExcecaoContrato" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1">Dt. Vig.</td>
                                    <td class="tdNormal1">
                                        <asp:TextBox runat="server" SkinID="textboxSkin" EnableViewState="false" ID="txtTabelaExcecaoVigencia" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                    </td>
                                </tr>
                            </table>
                          <asp:GridView ID="gridItens" SkinID="gridViewSkin" width="445" runat="server" 
                                AutoGenerateColumns="False" DataKeyNames="ID" 
                                onrowdatabound="gridItens_RowDataBound" onrowcommand="gridItens_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Parcela">
                                        <ItemTemplate>
                                            <asp:TextBox SkinID="textboxSkin" Width="20" ID="txtParcela" runat="server" Text='<%# Bind("Parcela") %>' />
                                            <cc1:MaskedEditExtender Mask="99" runat="server" ID="meeParcela" TargetControlID="txtParcela" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Percentual">
                                        <ItemTemplate>
                                            <asp:TextBox SkinID="textboxSkin" Width="40" ID="txtPercentual" runat="server" Text='<%# Bind("Percentual") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Perc. Carência">
                                        <ItemTemplate>
                                            <asp:TextBox SkinID="textboxSkin" Width="40" ID="txtPercentualCompraCarencia" runat="server" Text='<%# Bind("PercentualCompraCarencia") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Perc. Migração">
                                        <ItemTemplate>
                                            <asp:TextBox SkinID="textboxSkin" Width="40" ID="txtPercentualMigracao" runat="server" Text='<%# Bind("PercentualMigracao") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Perc. ADM">
                                        <ItemTemplate>
                                            <asp:TextBox SkinID="textboxSkin" Width="40" ID="txtPercentualADM" runat="server" Text='<%# Bind("PercentualADM") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Perc. Especial">
                                        <ItemTemplate>
                                            <asp:TextBox SkinID="textboxSkin" Width="40" ID="txtPercentualEspecial" runat="server" Text='<%# Bind("PercentualEspecial") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Perc. Idade">
                                        <ItemTemplate>
                                            <asp:TextBox CssClass="textbox" Width="40" ID="txtIdade" runat="server" Text='<%# Bind("Idade") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>

                                    <asp:ButtonField Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir">
                                        <ItemStyle Width="1%" HorizontalAlign="Center" />
                                        <ControlStyle Font-Size="11px" ForeColor="#cc0000" />
                                    </asp:ButtonField>
                                </Columns>
                            </asp:GridView>
                            <table cellpadding="2" cellspacing="0" width="445">
                                <tr>
                                    <td align="right" height="27" class="tdPrincipal1">
                                        <asp:Button Width="157" runat="server" ID="cmdAddItemTabelaExcecao" style="cursor: pointer;"  SkinID="botaoAzulBorda" Text="adicionar parcela à tabela" BorderWidth="0px" OnClick="cmdAddItemTabelaExcecao_Click" />
                                    </td>
                                </tr>
                            </table>
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
                                    <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicio" runat="server" /></td>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicio" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeNumeroParcelaVitalicio" TargetControlID="txtComissionamentoNumeroParcelaVitalicio" /></td>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentual" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Compra de carência</td>
                                    <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioCarencia" runat="server" /></td>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioCarencia" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioCarencia" TargetControlID="txtComissionamentoNumeroParcelaVitalicioCarencia" /></td>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualCarencia" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Migração</td>
                                    <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioMigracao" runat="server" /></td>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioMigracao" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioMigracao" TargetControlID="txtComissionamentoNumeroParcelaVitalicioMigracao" /></td>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualMigracao" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Administrativa</td>
                                    <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioADM" runat="server" /></td>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioADM" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioADM" TargetControlID="txtComissionamentoNumeroParcelaVitalicioADM" /></td>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualADM" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1">Especial</td>
                                    <td class="tdNormal1" align="center" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioEspecial" runat="server" /></td>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1;border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioEspecial" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioEspecial" TargetControlID="txtComissionamentoNumeroParcelaVitalicioEspecial" /></td>
                                    <td class="tdNormal1" style="border-bottom:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualEspecial" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1" style="border-right:solid 1px #507CD1">Idade</td>
                                    <td class="tdNormal1" align="center" style="border-right:solid 1px #507CD1"><asp:CheckBox EnableViewState="false" ID="chkComissionamentoVitalicioIdade" runat="server" /></td>
                                    <td class="tdNormal1" style="border-right:solid 1px #507CD1"><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40px" runat="server" ID="txtComissionamentoNumeroParcelaVitalicioIdade" MaxLength="4" /><cc1:MaskedEditExtender EnableViewState="false" Mask="99" runat="server" ID="meeComissionamentoNumeroParcelaVitalicioIdade" TargetControlID="txtComissionamentoNumeroParcelaVitalicioIdade" /></td>
                                    <td class="tdNormal1" ><asp:TextBox EnableViewState="false" CssClass="textbox" Width="40" ID="txtComissionamentoVitalicioPercentualIdade" runat="server" /></td>
                                </tr>
                            </table>
                            <br />
                            <table cellpadding="2" width="445">
                                <tr>
                                    <td align="left"><asp:Button OnClick="cmdVoltarTabelaExcecao_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltarComissionamento" Text="Voltar" /></td>
                                    <td align="right"><asp:Button OnClick="cmdSalvarTabelaExcecao_Click" SkinID="botaoAzul" runat="server" EnableViewState="true" ID="Button2" Text="Salvar" /></td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </cc1:TabPanel>

                <cc1:TabPanel runat="server" ID="p3">
                    <HeaderTemplate><font color="black">Equipe</font></HeaderTemplate>
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="0" width="445" style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdPrincipal1" colspan="3">Chefes de equipe</td>
                            </tr>
                            <tr>
                                <td class="tdNormal1" width="100px"><asp:DropDownList ID="cboSuperiorPerfil" SkinID="dropdownSkin" Width="99%" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboSuperiorPerfil_Change" /></td>
                                <td class="tdNormal1">
                                    <asp:DropDownList ID="cboSuperior" SkinID="dropdownSkin" Width="265px" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdNormal1">Data de Vigência</td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" SkinID="textboxSkin" EnableViewState="false" ID="txtDataVigenciaEquipe" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                    &nbsp;
                                    <asp:LinkButton EnableViewState="true" ID="cmdAdicionarEquipe" Text="adicionar" runat="server" OnClick="cmdAdicionarEquipe_Click" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <asp:GridView ID="gridEquipe" Width="445" SkinID="gridViewSkin" runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,SuperiorID"
                            OnRowDataBound="gridEquipe_RowDataBound" OnRowCommand="gridEquipe_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="SuperiorNome" HeaderText="Superior">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="SuperiorPerfilDescricao" HeaderText="Perfil">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <table cellpadding="2" width="445">
                            <tr>
                                <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltarEquipe" Text="Voltar" /></td>
                                <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="true" ID="Button3" Text="Salvar" /></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </cc1:TabPanel>

                <cc1:TabPanel runat="server" ID="p4_GrupoVenda">
                    <HeaderTemplate><font color="black">Grupo de Venda</font></HeaderTemplate>
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="0" width="445" style="border: solid 1px #507CD1">
                            <tr>
                                <td class="tdPrincipal1" colspan="3">Grupos de venda</td>
                            </tr>
                            <tr>
                                <td class="tdNormal1" width="65">Grupo</td>
                                <td class="tdNormal1">
                                    <asp:DropDownList SkinID="dropdownSkin" Width="300" runat="server" ID="cboGrupo" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tdNormal1">Dt. Vig.</td>
                                <td class="tdNormal1">
                                    <asp:TextBox runat="server" SkinID="textboxSkin" EnableViewState="false" ID="txtDataVigenciaGrupo" Width="60" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                    &nbsp;
                                    <asp:LinkButton EnableViewState="true" ID="cmdAdicionarGrupo" Text="adicionar" runat="server" OnClick="cmdAdicionarGrupo_Click" />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <asp:GridView ID="gridGruposDoUsuario" Width="445" SkinID="gridViewSkin" runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,GrupoVendaID"
                            OnRowDataBound="gridGruposDoUsuario_RowDataBound" OnRowCommand="gridGruposDoUsuario_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="GrupoVendaDescricao" Visible="true" HeaderText="Grupo">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Data" HeaderText="Vigência" DataFormatString="{0:dd/MM/yyyy}">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="1%" />
                                </asp:ButtonField>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <table cellpadding="2" width="445">
                            <tr>
                                <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltarGrupo" Text="Voltar" /></td>
                                <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="true" ID="Button4" Text="Salvar" /></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </cc1:TabPanel>

                <cc1:TabPanel runat="server" ID="p5_Contatos">
                    <HeaderTemplate><font color="black">Contatos</font></HeaderTemplate>
                    <ContentTemplate>
                            <table cellpadding="2" cellspacing="0" width="445" style="border-top: solid 1px #507CD1;border-left: solid 1px #507CD1;border-right: solid 1px #507CD1">
                                <tr>
                                    <td class="tdPrincipal1" colspan="3">Contatos</td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1" width="65">Contato</td>
                                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtContato" Width="98%" MaxLength="149" EnableViewState="false" /></td>
                                </tr>
                                <tr>
                                    <td class="tdNormal1">Depto.</td>
                                    <td class="tdNormal1"><asp:TextBox CssClass="textbox" runat="server" ID="txtContatoDepartamento" Width="98%" MaxLength="249" EnableViewState="false" /></td>
                                </tr>
                            </table>
                            <table cellpadding="2" cellspacing="0" width="445" style="border-left: solid 1px #507CD1;border-right: solid 1px #507CD1">
                                <tr>
                                    <td class="tdNormal1" width="65">DDD</td>
                                    <td class="tdNormal1" width="40px"><asp:TextBox CssClass="textbox" runat="server" ID="txtDDDCont" Width="30px" MaxLength="3" EnableViewState="false" /><cc1:MaskedEditExtender runat="server" id="meeDDD" Mask="99" TargetControlID="txtDDDCont" /></td>
                                    <td class="tdNormal1" width="34px">Fone</td>
                                    <td class="tdNormal1" width="61px"><asp:TextBox CssClass="textbox" runat="server" ID="txtFoneCont" Width="60px" MaxLength="9" EnableViewState="false" /><cc1:MaskedEditExtender runat="server" id="meeFone" Mask="9999-9999" TargetControlID="txtFoneCont" /></td>
                                    <td class="tdNormal1" width="37px" align="right">&nbsp;Ramal&nbsp;</td>
                                    <td class="tdNormal1" align="left"><asp:TextBox CssClass="textbox" runat="server" ID="txtRamalCont" Width="96%" MaxLength="5" EnableViewState="false" />&nbsp;</td>
                                </tr>
                            </table>
                            <table cellpadding="2" cellspacing="0" width="445" style="border-bottom: solid 1px #507CD1;border-left: solid 1px #507CD1;border-right: solid 1px #507CD1">
                                <tr>
                                    <td class="tdNormal1" width="65">Email</td>
                                    <td class="tdNormal1">
                                        <asp:TextBox CssClass="textbox" Width="82%" runat="server" ID="txtEmailCont" MaxLength="65" EnableViewState="false" />
                                        &nbsp;
                                        <asp:LinkButton EnableViewState="true" ID="lnkAdicionarContato" Text="adicionar" runat="server" OnClick="cmdAdicionarContato_Click" />
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <asp:GridView DataKeyNames="ID" Font-Size="10px" Width="445" ID="gridContatos" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" BorderWidth="1px" BorderColor="#507CD1" GridLines="None" OnRowCommand="gridContatos_OnRowCommand" OnRowDataBound="gridContatos_OnRowDataBound" >
                                <RowStyle BackColor="#EFF3FB" Font-Size="10px" />
                                <Columns>
                                    <asp:BoundField DataField="Nome" HeaderText="Contato" ItemStyle-Width="40%">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Departamento" HeaderText="Departamento" ItemStyle-Width="30%">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Fone" HeaderText="Fone">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Email" HeaderText="E-mail">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:ButtonField CommandName="excluir" Text="<img src='../images/delete.png' align='middle' title='excluir' alt='excluir' border='0' />">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle Width="1%" />
                                    </asp:ButtonField>
                                    <asp:ButtonField Text="<img src='../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                                        <ItemStyle Width="1%" />
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
                            <table cellpadding="2" width="445">
                            <tr>
                                <td align="left"><asp:Button OnClick="cmdVoltar_Click" SkinID="botaoAzul" runat="server" EnableViewState="false" ID="cmdVoltarContato" Text="Voltar" /></td>
                                <td align="right"><asp:Button OnClick="cmdSalvar_Click" SkinID="botaoAzul" runat="server" EnableViewState="true" ID="Button5" Text="Salvar" /></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </cc1:TabPanel>
            </cc1:TabContainer>

        </ContentTemplate>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel runat="server" ID="upnlAlerta" UpdateMode="Conditional" EnableViewState="false">
        <ContentTemplate>
            <cc1:ModalPopupExtender ID="MPE" runat="server" EnableViewState="false"
                TargetControlID="lnk"
                PopupControlID="pnlAlert"
                BackgroundCssClass="modalBackground" 
                CancelControlID="cmdCloseAlert"
                DropShadow="true"  />
            <asp:Panel runat="server" ID="pnlAlert" EnableViewState="false">
                <asp:LinkButton runat="server" EnableViewState="false" ID="lnk" />
                <table width="350" align="center" bgcolor="gainsboro" style="border:solid 1px black">
                    <tr>
                        <td align="center">
                            <asp:Literal runat="server" ID="litAlert" EnableViewState="false" />
                        </td>
                    </tr>
                    <tr height="8"><td height="8">&nbsp</td></tr>
                    <tr>
                        <td align="center">
                            <input runat="server" style="width:45px;font-size:12px;font-family:Arial" id="cmdCloseAlert" type="button" value="OK" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
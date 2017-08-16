<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="AlmoxSaidas.aspx.cs" Inherits="www.admin.AlmoxSaidas" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"><img src="../images/imgTitulos/produtos_saida.png" /></td></tr>
        <tr><td><span class="titulo">Almoxarifado - Saídas</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo as movimentações de saída em estoque, em ordem decrescente</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">

    <script src="../jQuery/jquery-1.3.2.js" type="text/javascript"></script>
    <script src="../jQuery/plugins/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function pageLoad() {
            $(document).ready(function() {
                $("#ctl00_cphContent_txtCorretor").autocomplete("../searchUsuarioMethod.aspx", {
                    width: 300,
                    scroll: false,
                    dataType: "json",
                    parse: function(data) {
                        var parsed = [];
                        data = data.Produtores;

                        for (var i = 0; i < data.length; i++) {
                            parsed[parsed.length] = {
                                data: data[i],
                                value: data[i].ID,
                                result: data[i].Nome + " (" + data[i].Doc + ")"
                            };
                        }

                        return parsed;
                    },
                    formatItem: function(item) {
                        return item.Nome + " (" + item.Doc + ")";
                    }
                }).result(function(e, item) {
                    document.getElementById('ctl00_cphContent_txtCorretorID').value = item.ID;
                    }
                );
                });

                $(document).ready(function() {
                    $("#ctl00_cphContent_txtCorretorSearch").autocomplete("../searchUsuarioMethod.aspx", {
                        width: 300,
                        scroll: false,
                        dataType: "json",
                        parse: function(data) {
                            var parsed = [];
                            data = data.Produtores;

                            for (var i = 0; i < data.length; i++) {
                                parsed[parsed.length] = {
                                    data: data[i],
                                    value: data[i].ID,
                                    result: data[i].Nome + " (" + data[i].Doc + ")"
                                };
                            }

                            return parsed;
                        },
                        formatItem: function(item) {
                            return item.Nome + " (" + item.Doc + ")";
                        }
                    }).result(function(e, item) {
                        document.getElementById('ctl00_cphContent_txtCorretorIDSearch').value = item.ID;
                    }
                );
            });
        }
    </script>
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="0" style="border: solid 1px #507CD1" width="550px">
                <tr>
                    <td class="tdNormal1" width="60px"><b>Operadora</b></td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboOperadora" SkinID="dropdownSkin" runat="server" Width="489px" AutoPostBack="true" onselectedindexchanged="cboOperadora_SelectedIndexChanged" /></td>
                </tr>
                <tr>
                    <td class="tdNormal1" width="60px"><b>Filial</b></td>
                    <td class="tdNormal1"><asp:DropDownList ID="cboFilial" SkinID="dropdownSkin" runat="server" Width="489px" AutoPostBack="true" onselectedindexchanged="cboFilial_SelectedIndexChanged" /></td>
                </tr>
                <tr>
                    <td class="tdNormal1" width="60px"><b>Tipo</b></td>
                    <td class="tdNormal1" width="490px"><asp:DropDownList ID="cboTipo" SkinID="dropdownSkin" runat="server" Width="489px" AutoPostBack="true" OnSelectedIndexChanged="cboTipo_SelectedIndexChanged" /></td>
                </tr>
                <%--<tr>
                    <td class="tdNormal1" width="60px"><b>Produto</b></td>
                    <td class="tdNormal1" width="490px"><asp:DropDownList ID="cboProduto" SkinID="dropdownSkin" runat="server" Width="489px" AutoPostBack="true" OnSelectedIndexChanged="cboProduto_SelectedIndexChanged" /></td>
                </tr>--%>
            </table>
            <br />
            <asp:GridView ID="gridEntradas" Width="550px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="true" AutoGenerateColumns="False"  
                DataKeyNames="ID,ProdutoID" OnRowCommand="gridEntradas_RowCommand" 
                onrowdatabound="gridEntradas_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="TipoProdutoDescricao" HeaderText="Tipo">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField Visible="true" DataField="OperadoraNome" HeaderText="Operadora">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="QTDFlutuante" HeaderText="Qtd. Atual">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="NumeracaoResumo" HeaderText="Numeração">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataFormatString="{0:dd/MM/yyyy}" DataField="Data" HeaderText="Data">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField Text="<img src='../images/detail2.png' title='detalhes' alt='detalhes' border='0' />" CommandName="detalhes">
                        <ItemStyle HorizontalAlign="Center" Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/retiradaEstoque.png' title='retirar' alt='retirar' border='0' />" CommandName="acao" >
                        <ItemStyle Font-Size="10px" Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <br />
            <table id="tblDetalhe" runat="server" enableviewstate="true" visible="false" width="550px" cellpadding="3" cellspacing="0">
                <tr>
                    <td align="center" bgcolor="#507CD1"><span style="color:white" class="subtitulo" runat="server" id="span3"><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Histórico de Retiradas</b><asp:Literal runat="server" EnableViewState="false" ID="litHistorico" /></span></td>
                    <td align="right"  bgcolor="#507CD1"><asp:ImageButton ID="cmdFecharDetalhe" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar" OnClick="cmdFecharDetalhe_Click" /></td>
                </tr>
            </table>
            <asp:GridView ID="gridDetalhes" OnRowCommand="gridDetalhes_RowCommand" OnRowDataBound="gridDetalhes_RowDataBound" Width="550px" SkinID="gridViewSkin" runat="server" AutoGenerateColumns="False" DataKeyNames="ID,Rasurado" EnableViewState="true">
                <Columns>
                    <asp:BoundField DataField="AgenteNome" HeaderText="Corretor">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="NumOuQtd" HeaderText="Retirado">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="EmprestadoA" HeaderText="Emprestado a">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField Text="<img src='../images/unactive.png' title='rasurado' alt='rasurado' border='0' />" CommandName="rasurar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <br />
            <asp:Panel runat="server" ID="pnl" Visible="false">
                <table bgcolor="#EFF3FB" border="0" cellpadding="2" cellspacing="0" style="border: #507CD1 3px solid" width="550">
                    <tr><td colspan="2" height="3"></td></tr>
                    <tr>
                        <td colspan="2" align='center'>
                            <b><font color='#507CD1' size='2'>SAÍDA DE MATERIAL</font></b>
                            <img src="../images/retiradaEstoque.png" border='0'/>
                        </td>
                    </tr>
                    <tr><td colspan="2" height="3"></td></tr>
                    <tr height="1"><td colspan="2" height="1" style="background-color:#507CD1"></td></tr>
                    <tr><td colspan="1" height="3"></td></tr>
                    <tr>
                        <td width="50px"><b>Produto</b></td>
                        <td width="500"><asp:Label runat="server" ID="lblProdutoCarregado" /></td>
                    </tr>
                    <tr>
                        <td width="50px"><b>Motivo</b></td>
                        <td width="500"><asp:DropDownList ID="cboMotivo" Width="300" SkinID="dropdownSkin" runat="server" AutoPostBack="true" onselectedindexchanged="cboMotivo_SelectedIndexChanged" /></td>
                    </tr>
                    <tr runat="server" id="trCorretor">
                        <td width="50px"><b>Usuário</b></td>
                        <td width="500">
                            <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtCorretorSearch" Width="300px" />
                            <input type="hidden" name="txtCorretorIDSearch" id="txtCorretorIDSearch" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td width="50px"><b>Quantidade</b></td>
                        <td width="500">
                            <asp:TextBox SkinID="textboxSkin" Width="50" runat="server" ID="txtQtd" AutoPostBack="true" OnTextChanged="txtNumDe_TextChanged" />
                            <cc1:MaskedEditExtender Mask="99999" runat="server" ID="meeQtd" EnableViewState="false" TargetControlID="txtQtd" />
                        </td>
                    </tr>
                    <tr runat="server" id="trNumeracao">
                        <td width="50px"><b>Numeração</b></td>
                        <td width="500">
                            <asp:TextBox SkinID="textboxSkin" Width="50" runat="server" ID="txtNumDe" AutoPostBack="true" ontextchanged="txtNumDe_TextChanged" />
                            a
                            <asp:TextBox BackColor="lightgray" ReadOnly="true" SkinID="textboxSkin" Width="50" runat="server" ID="txtNumAte" />
                        </td>
                    </tr>
                    <tr runat="server" id="trObs">
                        <td width="50px"><b>Obs.</b></td>
                        <td width="500"><asp:TextBox Width="290" Height="35" ID="txtObs" TextMode="MultiLine" SkinID="textboxSkin" runat="server" /></td>
                    </tr>
                    <tr><td colspan="1" height="15"></td></tr>
                    <tr>
                        <td colspan="2" align="center" width="550px">
                            <asp:Button EnableViewState="false" runat="server" ID="cmdCancelar" SkinID="botaoAzul" Text="Cancelar" Width="80" onclick="cmdCancelar_Click" />
                            &nbsp;&nbsp;
                            <asp:Button EnableViewState="false" runat="server" ID="cmdSalvar" SkinID="botaoAzul" Text="Salvar" Width="80" onclick="cmdSalvar_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <cc1:CollapsiblePanelExtender ID="cpe" runat="Server" EnableViewState="false"
                TargetControlID="pnlRelatorioCondensado"
                CollapsedSize="0"
                Collapsed="True"
                ExpandControlID="lnkAbrir"
                CollapseControlID="lnkFechar"
                AutoCollapse="False"
                AutoExpand="False"
                ScrollContents="false"
                ExpandDirection="Vertical" />
            <table cellpadding="4" cellspacing="0" width="550px" style="border:solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1"><asp:LinkButton ForeColor="White" runat="server" ID="lnkAbrir" Text="<img align='absmiddle' src='../images/detail.png' border='0' />&nbsp;Relatório condensado de retiradas" EnableViewState="false" /></td>
                    <td class="tdPrincipal1" align="right"><asp:LinkButton runat="server" ID="lnkFechar" Text="<img src='../images/close.png' title='fechar' alt='fecar' border='0' />" /></td>
                </tr>
                <tr>
                    <td class="tdNormal1" colspan="2">
                        <asp:Panel ID="pnlRelatorioCondensado" runat="server" EnableViewState="true">
                            <table width="100%">
                                <tr>
                                    <td width="55px">Corretor</td>
                                    <td>
                                        <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtCorretor" Width="250px" EnableViewState="false" />&nbsp;<asp:ImageButton ID="cmdLocalizar" runat="server" ImageUrl="~/images/search.png" ToolTip="mostrar retiradas" EnableViewState="false" OnClick="cmdLocalizar_Click" />&nbsp;&nbsp;<asp:Literal ID="litMsg" runat="server" EnableViewState="false" />
                                        <input type="hidden" id="txtCorretorID" name="txtCorretorID" runat="server" />
                                    </td>
                                </tr>
                                <tr height="8"><td colspan="2" height="8"></td></tr>
                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:GridView DataKeyNames="PRODID,CORRETORID" ID="gridCondensado" Width="530px" SkinID="gridViewSkin" runat="server" AutoGenerateColumns="False" EnableViewState="true" OnRowCommand="gridCondensado_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="PRODUTONOME" HeaderText="Contrato">
                                                    <HeaderStyle HorizontalAlign="LEFT" />
                                                    <ItemStyle HorizontalAlign="LEFT" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="OPERADORANOME" HeaderText="Operadora">
                                                    <HeaderStyle HorizontalAlign="LEFT" />
                                                    <ItemStyle HorizontalAlign="LEFT" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="QTD" HeaderText="Qtd.">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:ButtonField Text="<img src='../images/search.png' title='detalhes' alt='detalhes' border='0' />" CommandName="detalhes" >
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                            </Columns>
                                        </asp:GridView>
                                        <br />
                                        <asp:GridView ID="gridCondensado_Detalhe" OnRowDataBound="gridCondensado_Detalhe_RowDataBound" OnRowCommand="gridCondensado_Detalhe_RowCommand" DataKeyNames="ID,RASURADO,PRODNUMLETRA,PRODNUMZEROS" Width="530px" SkinID="gridViewSkin" runat="server" AutoGenerateColumns="False" EnableViewState="false" Visible="false">
                                            <Columns>
                                                <asp:BoundField DataField="PRODUTONOME" HeaderText="Contrato">
                                                    <HeaderStyle HorizontalAlign="LEFT" />
                                                    <ItemStyle HorizontalAlign="LEFT" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="PRODNUM" HeaderText="Núm. contrato">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="DTRETIRADA" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="OPERADORANOME" HeaderText="Operadora">
                                                    <HeaderStyle HorizontalAlign="LEFT" />
                                                    <ItemStyle HorizontalAlign="LEFT" />
                                                </asp:BoundField>
                                                <asp:ButtonField Text="<img src='../images/unactive.png' title='rasurado' alt='rasurado' border='0' />" CommandName="rasurar" >
                                                    <ItemStyle Width="1%" />
                                                </asp:ButtonField>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="conferencia1.aspx.cs" Inherits="www.admin.conferencia2" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register src="../usercontrols/ucConferenciaPainelMensagem.ascx" tagname="ucConferenciaPainelMensagem" tagprefix="uc1" %>
<%@ Register src="../usercontrols/ucConferenciaPassos.ascx" tagname="ucConferenciaPassos" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"></td></tr>
        <tr><td><span class="titulo">Conferência</span></td></tr>
        <tr><td><span class="subtitulo">Conferência de propostas</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <script src="../jQuery/jquery-1.3.2.js" type="text/javascript"></script>
    <script src="../jQuery/plugins/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function pageLoad() 
        {
            $(document).ready(function()
            {
                $("#ctl00_cphContent_txtCorretor").autocomplete("../searchCorretorMethod.aspx", {
                    width: 300,
                    scroll: false,
                    dataType: "json",
                    parse: function(data) {
                        var parsed = [];
                        data = data.Corretores;

                        for (var i = 0; i < data.length; i++) {
                            parsed[parsed.length] = {
                                data: data[i],
                                value: data[i].ID,
                                result: data[i].Nome + " (" + data[i].Doc + " - " + data[i].Codigo + ")"
                            };
                        }

                        return parsed;
                    },
                    formatItem: function(item) {
                        return item.Nome + " (" + item.Doc + " - " + item.Codigo + ")";
                    }
                }).result(function(e, item) {
                        document.getElementById('ctl00_cphContent_txtCorretorID').value = item.ID;
                    }
                );
            });

            $(document).ready(function() {
                $("#ctl00_cphContent_txtCorretorTerceiroIdentificacao").autocomplete("../searchCorrTercMethod.aspx", {
                    width: 300,
                    scroll: false,
                    dataType: "json",
                    parse: function(data) {
                        var parsed = [];
                        data = data.CorretoresTerc;

                        for (var i = 0; i < data.length; i++) {
                            parsed[parsed.length] = {
                                data: data[i],
                                value: data[i].Nome,
                                result: data[i].Nome + " (" + data[i].CPF + ")"
                            };
                        }

                        return parsed;
                    },
                    formatItem: function(item) {
                        return item.Nome + " (" + item.CPF + ")";
                    }
                }).result(function(e, item) {
                    document.getElementById('ctl00_cphContent_txtCorretorTerceiroIdentificacao').value = item.Nome;
                    document.getElementById('ctl00_cphContent_txtCorretorTerceiroCPF').value = item.CPF;
                   }
                );
               });

               $(document).ready(function() {
                    $("#ctl00_cphContent_txtSuperiorTerceiroIdentificacao").autocomplete("../searchSuperTercMethod.aspx", {
                       width: 300,
                       scroll: false,
                       dataType: "json",
                       parse: function(data) {
                           var parsed = [];
                           data = data.SuperioresTerc;

                           for (var i = 0; i < data.length; i++) {
                               parsed[parsed.length] = {
                                   data: data[i],
                                   value: data[i].Nome,
                                   result: data[i].Nome + " (" + data[i].CPF + ")"
                               };
                           }

                           return parsed;
                       },
                       formatItem: function(item) {
                           return item.Nome + " (" + item.CPF + ")";
                       }
                   }).result(function(e, item) {
                        document.getElementById('ctl00_cphContent_txtSuperiorTerceiroIdentificacao').value = item.Nome;
                        document.getElementById('ctl00_cphContent_txtSuperiorTerceiroCPF').value = item.CPF;
                   }
                );
               });
        }
    </script>
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <uc2:ucConferenciaPassos ID="ucConferenciaPassos" runat="server" />
            <br />
            <table cellpadding="2" cellspacing="0" width="750px">
                <tr>
                    <td width="65%" valign="top">
                        <table width="380" cellpadding="2" cellspacing="1" style="border: solid 1px #507CD1">
                            <tr>
                                <td bgcolor='#507CD1' width="105"><font color='#EFF3FB'><b>Filial</b></font></td>
                                <td bgcolor='#EFF3FB' colspan="2"><asp:DropDownList Width="184" ID="cboFilial" SkinID="dropdownSkin" runat="server" /></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1' width="105"><font color='#EFF3FB'><b>Estipulante</b></font></td>
                                <td bgcolor='#EFF3FB' colspan="2"><asp:DropDownList Width="184" ID="cboEstipulante" SkinID="dropdownSkin" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboEstipulante_Changing" /></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1'><font color='#EFF3FB'><b>Número da proposta</b></font></td>
                                <td bgcolor='#EFF3FB' colspan="2"><asp:TextBox ID="txtNumeroProposta" Width="180" SkinID="textboxSkin" MaxLength="16" runat="server" /></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1'><font color='#EFF3FB'><b>Confirme</b></font></td>
                                <td bgcolor='#EFF3FB' width="182"><asp:TextBox ID="txtNumeroPropostaConfirme" Width="180" SkinID="textboxSkin" MaxLength="16" runat="server" /></td>
                                <td bgcolor='#EFF3FB'><asp:Button ID="cmdValidarNumeroProposta" Text="validar" runat="server" SkinID="botaoPequeno" onclick="cmdValidarNumeroProposta_Click" /></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1'><font color='#EFF3FB'><b>Operadora</b></font></td>
                                <td bgcolor='#EFF3FB' colspan="2"><asp:DropDownList Width="184" ID="cboOperadora" SkinID="dropdownSkin" runat="server" OnSelectedIndexChanged="cboOperadora_SelectedIndexChanged" AutoPostBack="true" /></td>
                            </tr>
                             <tr>
                                <td bgcolor='#507CD1'><font color='#EFF3FB'><b>Contrato Adm.</b></font></td>
                                <td bgcolor='#EFF3FB' colspan="2"><asp:DropDownList Width="184" ID="cboContratoADM" SkinID="dropdownSkin" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboContratoADM_Changing" /></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1'><font color='#EFF3FB'><b>Plano</b></font></td>
                                <td bgcolor='#EFF3FB' colspan="2"><asp:DropDownList Width="184" ID="cboPlano" SkinID="dropdownSkin" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cboPlano_Changing" /></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1'><font color='#EFF3FB'><b>Acomodação</b></font></td>
                                <td bgcolor='#EFF3FB' colspan="2"><asp:DropDownList Width="184" ID="cboAcomodacao" SkinID="dropdownSkin" runat="server" /></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1'><font color='#EFF3FB'><b>Corretor</b></font></td>
                                <td bgcolor='#EFF3FB' colspan="2">
                                    <asp:TextBox SkinID="textboxSkin" runat="server" ID="txtCorretor" Width="180" />
                                    <input type="hidden" name="txtCorretorID" id="txtCorretorID" runat="server" />
                                </td>
                            </tr>

                                <tr>
                                    <td bgcolor='#507CD1'><font color='yellow'><b>Ident. Corretor</b></font></td>
                                    <td bgcolor='#EFF3FB' colspan="2"><asp:TextBox SkinID="textboxSkin" runat="server" ID="txtCorretorTerceiroIdentificacao" Width="180px" MaxLength="240" /></td>
                                </tr>
                                <tr>
                                    <td bgcolor='#507CD1'><font color='yellow'><b>CPF</b></font></td>
                                    <td bgcolor='#EFF3FB' colspan="2"><asp:TextBox SkinID="textboxSkin" runat="server" ID="txtCorretorTerceiroCPF" Width="79px" /><cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeCorretorTerceiroCPF" Mask="999,999,999-99" ClearMaskOnLostFocus="true" TargetControlID="txtCorretorTerceiroCPF" /></td>
                                </tr>
                                <tr>
                                    <td bgcolor='#507CD1'><font color='yellow'><b>Ident. Superior</b></font></td>
                                    <td bgcolor='#EFF3FB' colspan="2"><asp:TextBox SkinID="textboxSkin" runat="server" ID="txtSuperiorTerceiroIdentificacao" Width="180px" MaxLength="240" /></td>
                                </tr>
                                <tr>
                                    <td bgcolor='#507CD1'><font color='yellow'><b>CPF</b></font></td>
                                    <td bgcolor='#EFF3FB' colspan="2"><asp:TextBox SkinID="textboxSkin" runat="server" ID="txtSuperiorTerceiroCPF" Width="79px" /><cc1:MaskedEditExtender runat="server" EnableViewState="false" ID="meeSuperiorTerceiroCPF" Mask="999,999,999-99" ClearMaskOnLostFocus="true" TargetControlID="txtSuperiorTerceiroCPF" /></td>
                                </tr>

                            <tr>
                                <td bgcolor='#507CD1'><font color='#EFF3FB'><b>Tipo de contrato</b></font></td>
                                <td bgcolor='#EFF3FB' colspan="2"><asp:DropDownList Width="184" ID="cboTipoContrato" SkinID="dropdownSkin" runat="server" /></td>
                            </tr>
                            <tr>
                                <td bgcolor='#507CD1'><font color='#EFF3FB'><b>Admissão</b></font></td>
                                <td bgcolor='#EFF3FB' colspan="2">
                                    <asp:TextBox SkinID="textboxSkin" runat="server" id="txtAdmissao" Width="66px" onkeypress="filtro_SoNumeros(event); mascara_DATA(this, event);" MaxLength="10"/>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <div id="divCheckList" style="width:380px; border:solid 1px #507CD1; background-color:#EFF3FB">
                            <font color='#507CD1'>&nbsp;<b>Checklist</b></font>
                            <asp:CheckBoxList Width="100%" ID="chklCheckList" runat="server" />
                        </div>
                    </td>
                    <td valign="top" width="35%">
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
            <asp:Panel ID="pnlSelNumeroContral" runat="server" EnableViewState="false" Visible="false">
                <table width="600px" cellpadding="4" cellspacing="0" style="border:solid 1px #507CD1">
                    <tr>
                        <td class="tdNormal1">Selecione abaixo para qual operadora está cadastrando esta proposta.</td>
                    </tr>
                </table>
                <asp:GridView ID="gridSelNumeroContral" Width="600px" SkinID="gridViewSkin" EnableViewState="false"
                    runat="server" AllowPaging="True" AutoGenerateColumns="False"  DataKeyNames="ID,OperadoraID,AgenteID"
                    onrowcommand="gridSelNumeroContral_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="OperadoraNome" HeaderText="Operadora">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:ButtonField Text="<img src='images/active.png' title='selecionar' alt='selecionar' border='0' />" CommandName="usar" >
                            <ItemStyle Width="1%" />
                        </asp:ButtonField>
                    </Columns>
                </asp:GridView>
                <br />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
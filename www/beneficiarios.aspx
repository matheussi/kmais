<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="beneficiarios.aspx.cs" Inherits="www.beneficiarios" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table width="70%">
        <tr><td rowspan="3"><img src="images/icones/beneficiarios2.png" />&nbsp;</td></tr>
        <tr><td><span class="titulo">Beneficiários</span></td></tr>
        <tr><td nowrap><span class="subtitulo">Selecione a operadora para exibir seus beneficiários</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="cphContent" runat="server">
<asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
<ContentTemplate>
    <table>
        <tr>
            <td><span class="subtitulo">Tipo de busca</span></td>
        </tr>
        <tr>
            <td>
                <asp:RadioButton Text="Qualquer parte do campo" runat="server" ID="optQualquer" GroupName="a" Checked="true" />
                &nbsp;
                <asp:RadioButton Text="Início do campo " runat="server" ID="optInicio" GroupName="a"  />
                &nbsp;
                <asp:RadioButton Text="Campo inteiro" runat="server" ID="optInteiro" GroupName="a"  />
            </td>
        </tr>
    </table>
    <br />
    <table>
        <tr>
            <td width="65"><span class="subtitulo">Nome</span></td>
            <td><asp:TextBox CssClass="textbox" runat="server" ID="txtNome" Width="250" /></td>
        </tr>
        <tr runat="server" id="trMatricula" visible="false">
            <td width="65"><span class="subtitulo">Matrícula</span></td>
            <td><asp:TextBox CssClass="textbox" runat="server" ID="txtMatricula" Width="100" MaxLength="50" /></td>
        </tr>
    </table>
    <table>
        <tr>
            <td width="65"><span class="subtitulo">CPF</td>
            <td width="120"><asp:TextBox CssClass="textbox" runat="server" ID="txtCPF" Width="100" /><cc1:MaskedEditExtender runat="server" id="meeCPF" Mask="999,999,999-99" TargetControlID="txtCPF" /></td>
            <td width="20"><span class="subtitulo">RG</td>
            <td><asp:TextBox CssClass="textbox" runat="server" ID="txtRG" Width="102" /></td>
            <td><asp:Button runat="server" ID="cmdConsultar" Text="Buscar" SkinID="botaoAzul" onclick="cmdConsultar_Click" /></td>
        </tr>
    </table>
    <br />
    <asp:GridView DataKeyNames="ID,EnriquecimentoID" Width="70%" ID="gridBeneficiarios" 
        runat="server" AutoGenerateColumns="False" SkinID="gridViewSkin" 
        onrowcommand="gridBeneficiarios_RowCommand" 
        onrowdatabound="gridBeneficiarios_RowDataBound">
        <Columns>
            <asp:BoundField ItemStyle-Wrap="false" DataField="Nome" HeaderText="Nome">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField ItemStyle-Wrap="false" DataField="FTelefone" HeaderText="Fone">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField ItemStyle-Wrap="false"  DataField="FCelular" HeaderText="Celular">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField ItemStyle-Wrap="false"  DataField="Email" HeaderText="E-mail">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField  DataField="MatriculaAssociativa" HeaderText="Matr.Associativa">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <%--<asp:BoundField Visible="false" DataField="TipoParticipacaoContrato" HeaderText="Titular">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>--%>
            <asp:ButtonField CommandName="contratos" Text="<img src='images/detail2.png' title='contratos' alt='contratos' border='0' />">
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle Width="1%" />
            </asp:ButtonField>
            <asp:ButtonField CommandName="editar" Text="<img src='images/edit.png' title='editar' alt='editar' border='0' />">
                <HeaderStyle HorizontalAlign="Left" />
                <ItemStyle Width="1%" />
            </asp:ButtonField>
        </Columns>
    </asp:GridView>
    <br />
    <asp:Panel ID="pnlContratos" EnableViewState="false" Visible="false" runat="server">
        <table width="70%" cellpadding="3" cellspacing="0" style="border: solid 1px #507CD1">
            <tr>
                <td align="left" class="tdNormal1"><span style="color:black" class="subtitulo" runat="server" id="lblSuperior" enableviewstate="false">Contratos do beneficiário</span></td>
                <%--<td align="right" class="tdNormal1"><asp:ImageButton ID="cmdFecharContato" runat="server" EnableViewState="false" ImageUrl="~/images/close.png" ToolTip="fechar"  /></td>--%>
            </tr>
        </table>
        <asp:GridView DataKeyNames="ID" Width="70%" ID="gridContratos" EnableViewState="false"
            runat="server" AutoGenerateColumns="False" SkinID="gridViewSkin" 
            OnRowCommand="gridContratos_RowCommand" OnRowDataBound="gridContratos_RowDataBound">
            <Columns>
                <asp:BoundField ItemStyle-Wrap="false" DataField="Numero" HeaderText="Número">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField ItemStyle-Wrap="false" DataField="OperadoraDescricao" HeaderText="Operadora">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="EmpresaCobranca" HeaderText="Empresa">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField ItemStyle-Wrap="false"  DataField="Data" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField ItemStyle-Wrap="false" DataField="TipoParticipacaoContrato" HeaderText="Titular">
                    <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:ButtonField CommandName="editar" Text="<img src='images/edit.png' alt='editar' border='0' />">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle Width="1%" />
                </asp:ButtonField>
            </Columns>
        </asp:GridView>
        <br />
    </asp:Panel>
    <table width="70%">
        <tr>
            <td align="right">
                <asp:Button runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
            </td>
        </tr>
    </table>
</ContentTemplate>
</asp:UpdatePanel>
</asp:Content>
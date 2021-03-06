﻿<%@ Page Theme="Theme" Language="C#" MasterPageFile="~/layout.Master" AutoEventWireup="false" CodeBehind="estadoscivis.aspx.cs" Inherits="www.admin.estadoscivis" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitle" runat="server">
    <table>
        <tr><td rowspan="3"></td></tr>
        <tr><td><span class="titulo">Estados Civis</span></td></tr>
        <tr><td><span class="subtitulo">Abaixo os estados civis cadastrados</span></td></tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:UpdatePanel runat="server" ID="up" UpdateMode="Always">
        <ContentTemplate>
            <table cellpadding="2" cellspacing="1" width="550px" style="border: solid 1px #507CD1">
                <tr>
                    <td class="tdPrincipal1" width="70">Operadora</td>
                    <td class="tdNormal1"><asp:DropDownList Width="99%" ID="cboOperadora" SkinID="dropdownSkin" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cboOperadora_SelectedIndexChanged" /></td>
                </tr>
            </table>
            <br />
            <asp:GridView ID="grid" Width="550px" SkinID="gridViewSkin" 
                runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="ID"
                OnRowCommand="grid_RowCommand" OnRowDataBound="grid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Codigo" HeaderText="Código">
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:ButtonField Text="<img src='../images/delete.png' title='excluir' alt='excluir' border='0' />" CommandName="excluir" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                    <asp:ButtonField Text="<img src='../images/edit.png' title='editar' alt='editar' border='0' />" CommandName="editar" >
                        <ItemStyle Width="1%" />
                    </asp:ButtonField>
                </Columns>
            </asp:GridView>
            <br />
            <table width="553px">
                <tr>
                    <td align="right">
                        <asp:Button EnableViewState="false" runat="server" ID="cmdNovo" SkinID="botaoAzul" Text="Novo" Width="80" onclick="cmdNovo_Click" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
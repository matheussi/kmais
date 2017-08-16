<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="login.aspx.cs" Inherits="www.login" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="pt-BR">
<head>
   <title>CADBEN- Login</title>
   <!-- jQuery //-->
   <!-- BackgroundPNG //-->
    <script src="jQuery/plugins/pngBackground/DD_belatedPNG_0.0.7a.js" type="text/javascript"></script>
    <script src="js/commom.js" type="text/javascript"></script>
    <script type="text/javascript">
      DD_belatedPNG.fix('#Corpo, #Faixa, img');
    </script>
    <!-- BackgroundPNG //-->
   <link rel="stylesheet" type="text/css" href="css/style.css" />
</head>
<body>
   <form runat="server" id="Form1">
   <div id='Geral'>
      <div id='Faixa'></div>
      <div id='AcessoLogin'>Acesso ao Sistema UBRASP</div>
      <div id='Corpo'>
         <div id='LogoLogin'><img src='images/site/LogoLogin.png' alt='' border='0' /></div>
         <div id='BoxLogin'>
            <div id='BoxEmail'>
               <div id='Email'><span class='fonte12'><b>E-mail:</b></span></div>
               <div id='InputEmail'><input runat="server" id="txtEmail" type='text' style='width:180px;' /></div>
            </div>
            <div id='BoxSenha'>
               <div id='Senha'><span class='fonte12'><b>Senha:</b></span></div>
               <div id='InputSenha'><input runat="server" id="txtSenha" type='password' style='width:180px;' /></div>
               <div id='btnEntrar'><asp:ImageButton runat="server" ID="cmdEntrar" src='images/site/btnEntrar.jpg' alt='Entrar' border='0' onclick="cmdEntrar_Click" /></div>
            </div>
         </div>
         <div id='LinhaLogin'><img src='images/site/linha.jpg' height='2' width='471' alt='' border='0' /></div>
         <!--<div id='EsqueciLogin'>Esqueceu sua senha?</div>--><font color='red'><asp:Literal ID="litMsg" runat="server" EnableViewState="false" /></font>
      </div>
   </div>
   </form>  
</body>
</html>

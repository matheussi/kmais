function mascara(campotexto) {
    try
    {
        var campo = document.getElementById(campotexto);
        var temp = campo.value;
        var re = new RegExp("\\D", "g");
        temp = temp.replace(re, "");
        var temp1 = temp.substr(0, temp.length - 2);
        var temp2 = temp.substr(temp.length - 2, 2);
        re = new RegExp("^0+");
        temp1 = temp1.replace(re, "");
        if (temp1 == "") { temp1 = "0"; }
        if (temp2.length == 0) { temp2 = "00"; }
        if (temp2.length == 1) { temp2 = "0" + temp2; }
        temp = temp1 + "," + temp2;
        campo.value = temp;
//        campo.focus();
    }
    catch (e) { alert(e); }
}

function showhide(divid, imgid) {
  try {
    if (divid && (divid != '') ) {
      var divx = document.getElementById(divid);
      var imgx = document.getElementById(imgid);

      if (divx) {
        if (divx.style.display == 'block') {
          divx.style.display = 'none';
          if (imgx) imgx.src = 'imagens/modal/max_dark_blue_highlight.gif';
        }
        else {
          divx.style.display = 'block';
          if (imgx) imgx.src = 'imagens/modal/min_dark_blue_highlight.gif';
        }
      }
    }
  }
  catch(e) {}
}

function filtro_SoNumeros(evt) {

    var tecla;

    if (evt.keyCode)
        tecla = evt.keyCode;
    else
        tecla = evt.which;

    if (tecla < 48 || tecla > 57) {
        if (evt.keyCode)
            evt.returnValue = false;
        else
            evt.preventDefault();
    }
}

function mascara_DATA(Campo, teclapres) {
    var tecla = teclapres.keyCode;

    var vr = new String(Campo.value);
    vr = vr.replace(".", "");
    vr = vr.replace("/", "");
    vr = vr.replace("-", "");

    tam = vr.length + 1;

    if (tecla != 9 && tecla != 8) {
        if (tam > 2 && tam < 4)
            Campo.value = vr.substr(0, 2) + '/' + vr.substr(3, tam);
        if (tam > 4 && tam < 11)
            Campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/' + vr.substr(5, tam - 4);
    }
}

function mascara_CEP(Campo, teclapres) {

    if (teclapres.keyCode == 8 || teclapres.keyCode == 46 || teclapres.keyCode == 37 || teclapres.keyCode == 39) {
        return;
    }

    var tecla = teclapres.keyCode;

    var vr = new String(Campo.value);
    vr = vr.replace(".", "");
    vr = vr.replace(".", "");
    vr = vr.replace("/", "");
    vr = vr.replace("-", "");

    tam = vr.length + 1;

    if (tam > 5)
        Campo.value = vr.substr(0, 5) + '-' + vr.substr(5, tam);
}

function mascara_CNPJ(Campo, teclapres) {

    var tecla = teclapres.keyCode;

    var vr = new String(Campo.value);
    vr = vr.replace(".", "");
    vr = vr.replace(".", "");
    vr = vr.replace("/", "");
    vr = vr.replace("-", "");

    tam = vr.length + 1;

    if (tecla != 9 && tecla != 8) {
        if (tam > 2 && tam < 6)
            Campo.value = vr.substr(0, 2) + '.' + vr.substr(2, tam);
        if (tam >= 6 && tam < 9)
            Campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(5, tam - 5);
        if (tam >= 9 && tam < 13)
            Campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(5, 3) + '/' + vr.substr(8, tam - 8);
        if (tam >= 13 && tam < 15)
            Campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(5, 3) + '/' + vr.substr(8, 4) + '-' + vr.substr(12, tam - 12);
    }
}

function mascara_CPF(Campo, teclapres) {
    var tecla = teclapres.keyCode;

    var vr = new String(Campo.value);
    vr = vr.replace(".", "");
    vr = vr.replace(".", "");
    vr = vr.replace("-", "");

    tam = vr.length + 1;

    if (tecla != 9 && tecla != 8) {
        if (tam > 3 && tam < 7)
            Campo.value = vr.substr(0, 3) + '.' + vr.substr(3, tam);
        if (tam >= 7 && tam < 10)
            Campo.value = vr.substr(0, 3) + '.' + vr.substr(3, 3) + '.' + vr.substr(6, tam - 6);
        if (tam >= 10 && tam < 12)
            Campo.value = vr.substr(0, 3) + '.' + vr.substr(3, 3) + '.' + vr.substr(6, 3) + '-' + vr.substr(9, tam - 9);
    }
}

function filtro_SoNumeros_AllBrowser(evt) {
   
   var tecla;
   
   if (evt.keyCode)
	   tecla = evt.keyCode;
	else
	   tecla = evt.which;
	   
   if (tecla < 48 || tecla > 57) 
   {
      if (evt.keyCode)
         evt.returnValue = false;
      else
         evt.preventDefault();
   }
}

// MODAL DIALOG

var inx = -1;
var iny = -1;
var modalDialog = false;
function dialog(title, dest, w, h) {
  try {
    if ( !modalDialog ) {
      modalDialog = new DHTML_modalMessage();  // We only create one object of this class
      if (modalDialog) {
        modalDialog.setShadowOffset(4);        // Large shadow
        modalDialog.setSource('?mode=ajax&' + dest);
	modalDialog.setTitle(title);
        modalDialog.setCssClassMessageBox(false);
        modalDialog.setSize(w,h);
        modalDialog.setShadowDivVisible(true);  // Enable shadow for these boxes
        modalDialog.display();
      }
    }
  }
  catch(e) { alert('ERRO: ' + e); }
  return false;
}

function closeDialog()
{
  try {
    if (modalDialog) {
      modalDialog.close();
      modalDialog = false;
    }
  }
  catch(e) {}
}


function dialogDoMove(evt) {
  try {
    if (!evt) var evt = window.event;
    if (modalDialog) {
      if (inx < -500) {
        inx = evt.pageX;
        iny = evt.pageY;
      }
      var x = evt.pageX - inx;
      var y = evt.pageY - iny;
      modalDialog.movePos(x, y);
      inx = evt.pageX;
      iny = evt.pageY;
    }
  }
  catch(e) {}
}

function dialogMove() {
  try {
    window.captureEvents(Event.MOUSEMOVE);
    window.onmousemove = dialogDoMove;
    window.onmouseup   = dialogStop;
    inx = -1000;
    iny = -1000;
  }
  catch(e) {}
}

function dialogStop(evt) {
  try {
    window.releaseEvents(Event.MOUSEMOVE);
    window.onmousemove = null;
    inx = -1000;
    iny = -1000;
  }
  catch(e) {}
}


// FIM MODAL DIALOG


function soNumbers(m) {
  try {
    if (isNaN(m.value)) {
      m.value = '';
      alert("Deve preencher com numeros!");
      focus(m);
    }
  }
  catch(e) {}
}




//***************** CANCELAMENTO DE VENDA 

function cancel_enviar(dest,formx) {
  try {
    if(document.formu.matricula.value==''){
      alert("O campo 'Matricula' e obrigatorio!");
    }
    else if(document.formu.autorizacao.value==''){
      alert("O campo 'Nro. Autorizao' e obrigatorio!");
    }
    else{
      mainpost(dest,formx);//document.formu.submit();
    }
  }
  catch(e) { alert(e); }
}


//********************* SENHAS
function validar_senha() {
  try {
    var p0 = document.formsenha.pass0.value;
    var p1 = document.formsenha.pass1.value;
    var p2 = document.formsenha.pass2.value;

    if ( p1 == p0 ) { alert('Nao pode ser igual a senha anterior!'); document.formsenha.pass1.focus(); return false; }
    if ( p1.length < 8 ) { alert('Deve ter no minimo 8 caracteres!'); document.formsenha.pass1.focus(); return false; }
    if ( p1 != p2 ) { alert('A confirmacao da senha e diferente da nova senha!'); document.formsenha.pass1.focus(); return false; }
    return true;
  }
  catch(e) { alert(e); }
  return false;
}


//**************** VENDAS

function verificar_venda() {
  try {
    if (document.formu.matricula.value == '') {
      alert("O campo 'Matricula' e obrigatorio!");
      document.formu.matricula.focus();
      return false;
    }
    if (isNaN(document.formu.matricula.value)) {
      alert("Matricula invalida!");
      document.formu.matricula.focus();
      return false;
    }
    if (document.formu.valor.value == '') {
      alert("O campo 'Valor Total' e obrigatorio!");
      document.formu.valor.focus();
      return false;
    }
    if (document.formu.promotor.value == '') {
      alert("O campo 'No. do Vendedor' e obrigatorio!");
      document.formu.promotor.focus();
      return false;
    }
    return true;
  }
  catch(e) {}
  return false;
}


//************************* DIVIDAS

function dividas_format(str) {
  try {
    index = str.indexOf(".");
    if (index < 0) str = str + ".00";
    else {
      str = str.substring(0, index + 3);
      if (str.length < (index + 3)) str += "0";
    }
    return str;
  }
  catch(e) {}
  return '';
}

function dividas_addX(inpX,valX) {
  var total = document.filtros.total2.value;
  try {
    val2 = eval(Math.round(valX * 100)) / 100;
    if ( inpX.checked ) {
      total = eval(Math.round((total * 1 + val2) * 100)) / 100;
    }
    else {
      total = eval(Math.round((total * 1 - val2) * 100)) / 100;
    }
    document.filtros.total2.value = total;
    document.filtros.total.value = dividas_format(document.filtros.total2.value);
  }
  catch(e) { alert(e); }
}



//********************* FUNCIONARIOS
function funcionario_detalhe(matricula) {
  if (isNaN(matricula) || !(matricula > 0)) {
    alert("Matricula invalida!");
    return false;
  }
  //window.open('?pag=detalhe&matricula=' + matricula,'Detalhe','width=600,height=460,status=no,resizable=yes,scrollbars=yes');
  return true;
}
function funcionario_associar(matricula) {
  if (isNaN(matricula) || !(matricula > 0)) {
    alert("Matricula invalida!");
    return false;
  }
  //window.open('?pag=associar&matricula=' + matricula,'Associar','width=600,height=460,status=no,resizable=yes,scrollbars=yes');
  return true;
}

function funcionario_alterar_agencia(dest) {
  try {
    //window.open('?pag=contac&mat='+<?=$funcionario['matricula']?>,'Conta','width=400,height=320,status=no,resizable=no,scrollbars=no');
    //window.open('?pag=contac&mat=' + matricula,'Conta','width=400,height=320,status=no');
    if (dest && (dest != '')) {
      dialog('Edi&ccedil;&atilde;o de Agencia Bancaria', dest, 400, 300);
    }
  }
  catch(e) {}
  return false;
}

function funcionario_set_arcojoia() {
  try {
    var opcao = document.formjoia.joia.value;
    var valor = document.formjoia.valorjoia.value;

    if ( !isNaN(opcao) && !isNaN(valor) )  {
      if (opcao == 0) document.formjoia.valjoia.value = '0';
      if (opcao == 1) document.formjoia.valjoia.value = valor * 0.8 / 100;
      if (opcao == 2) document.formjoia.valjoia.value = valor / (12 * 100);
      if (opcao == 3) document.formjoia.valjoia.value = valor / (24 * 100);
    }
  }
  catch(e) {}
  return false;
}

function funcionario_habilitarLT() {
  try {
    document.formL.lim_total.disabled = false;
    document.formL.mudou.value = '1';
  }
  catch(e) {}
}

function funcionario_habilitarLP() {
  try {
    document.formL.lim_parcial.disabled = false;
    document.formL.mudou.value = '1';
  }
  catch(e) {}
}

function funcionario_excessao_enviar() {
  try {
    if (document.formL.mudou.value != 1) {
      alert ('Os limites nao mudaram!');
      return false;
    }
    if (isNaN(document.formL.lim_total.value)) {
      alert('Os limites tem que ser so numeros entre 0 e 100!');
      return false;
    }
    if (isNaN(document.formL.lim_parcial.value)) {
      alert('Os limites tem que ser so numeros entre 0 e 100!');
      return false;
    }
    if (document.formL.lim_total.value > 100) {
      alert('Os limites nao podem ser superiores ao 100%!');
      return false;
    }
    if (document.formL.lim_parcial.value > 100) {
      alert('Os limites nao podem ser superiores ao 100%!');
      return false;
    }
    if (document.formL.lim_total.value < 0) {
      alert('Os limites nao podem ser inferiores ao 0%!');
      return false;
    }
    if (document.formL.lim_parcial.value < 0) {
      alert('Os limites nao podem ser inferiores ao 0%!');
      return false;
    }
    document.formL.eliminar.value = '0';
    document.formL.lim_total.disabled = false;
    document.formL.lim_parcial.disabled = false;
    return true;
    //document.formL.submit();
  }
  catch(e) {}
  return false;
}

function funcionario_excessao_cancelar() {
  try {
    document.formL.mudou.value    = '1';
    document.formL.eliminar.value = '1';
    //document.formL.submit();
    return true;
  }
  catch(e) {}
  return false;
}




//************ PAGAMENTOS
function pagamento_novo() {
  try {
    if ( (document.pago.matricula.value == '') || isNaN(document.pago.matricula.value) ) {
      alert('Matricula invalida!');
      return false;
    }
    return true;
  }
  catch(e) { alert(e); }
  return false;
}

function novopag_sel_saldo(num) {
  var saldoX = eval('document.pagamentos.saldo_' + num);
  var valorX = eval('document.pagamentos.valor_' + num);

  if (saldoX.checked == false) {
    saldoX.value   = '0';
  }
  else {
    saldoX.value   = '1';
  }

  var total = 0;
  var qtde = document.pagamentos.cant.value;
  for (i=0; i < qtde; i++) {
    var campY = eval('document.pagamentos.saldo_' + i);
    var valY  = eval('document.pagamentos.valor_' + i);
    if (campY.value == '1') {
      total += (valY.value * 100);
    }
  }
  document.pagamentos.total.value = total / 100;

  return true;
}

function novopag_pagar() {
  if (document.pagamentos.total.value == 0) {
    alert('Tem que escolher as parcelas que estao sendo pagas!');
    return false;
  }
  document.pagamentos.total.disabled = false;
  return true;
}



//******************** CONCILIAR
function conciliar_arquivo() {
  try {
    document.filtros.exportar.value = '1';
    document.filtros.submit();
  }
  catch(e) {}
  return false;
}


//**************** USUARIOS

function xusuario_initpass() {
  if ( document.formX.init.checked ) {
    document.formX.senha1.value = '';
    document.formX.senha2.value = '';
    document.formX.senha1.disabled = false;
    document.formX.senha2.disabled = false;
  }
  else {
    document.formX.senha1.disabled = true;
    document.formX.senha2.disabled = true;
  }
  return;
}
function xusuario_novaloja() {
  if ( document.formX.initl.checked ) {
    document.formX.lojaX.disabled = false;
    document.formX.entidad.disabled = true;
  }
  else {
    document.formX.lojaX.disabled = true;
    document.formX.entidad.disabled = false;
  }
  return;
}

function xusuario_salvar() {
  // loja
  if ( document.formX.initl.checked == true ) {
    if ( document.formX.lojaX.value == '' ) {
      document.formX.lojaX.focus();
      alert('Deve informar o nome da nova loja!');
      return false;
    }
  }
  else {
    if ( document.formX.entidad.value == 0 ) {
      document.formX.entidad.focus();
      alert('Deve informar o nome da loja!');
      return false;
    }
  }
  // repasse
  if ( document.formX.repasse.value == '') {
    document.formX.repasse.focus();
    alert('Deve informar o repasse da loja!');
    return false;
  }
  if ( isNaN(document.formX.repasse.value) ) {
    document.formX.repasse.focus();
    alert('O repasse tem que ser un valor numerico!');
    return false;
  }
  // nome
  if ( document.formX.nome.value == '' ) {
    document.formX.nome.focus();
    alert('Deve informar o nome do usuario!');
    return false;
  }
  // usuario
  if ( document.formX.user.value == '' ) {
    document.formX.user.focus();
    alert('Deve informar o USUARIO(login)!');
    return false;
  }
  // senha
  if ( document.formX.init.checked == true ) {
    if ( document.formX.senha1.value == '' ) {
      document.formX.senha1.focus();
      alert('Deve informar a senha do usuario!');
      return false;
    }
    if ( document.formX.senha2.value == '' ) {
      document.formX.senha2.focus();
      alert('Deve confirmar a senha do usuario!');
      return false;
    }
    if ( document.formX.senha1.value != document.formX.senha2.value ) {
      document.formX.senha1.value = '';
      document.formX.senha2.value = '';
      document.formX.senha1.focus();
      alert('A confirmação da senha está errada. Tente novamente.');
      return false;
    }
    var temp = document.formX.senha1.value;
    if ( temp.length < 8 ) {
      document.formX.senha1.value = '';
      document.formX.senha2.value = '';
      document.formX.senha1.focus();
      alert('A senha deve ter no minimo 8 caracteres.');
      return false;
    }
  }
  document.formX.senha1.disabled = false;
  return true;
}




function matricula_valida(matx) {
  try {
    //if (matx && (matx != '') && !isNaN(matx) && (matx.length == 8) ) {
    if ( matx && (matx != '') && !isNaN(matx) && (matx.length >= 8) ) {
      return true;
    }
  }
  catch(e) {}
  return false;
}



//***** BANCOS ****

function arquivosbancos_ver(id_env) {
  try {
    //document.view.action = '?pag=view_env';
    document.view.envio.value = id_env;
    //document.view.submit();
    return true;
  }
  catch(e) { }
  return false;
}


function arquivosbancos_consolidar(id_env) {
  try {
    //document.view.action = '?pag=consolidar';
    document.view.envio.value = id_env;
    document.view.cons.value = 1;
    //document.view.submit();
    return true;
  }
  catch(e) {}
  return false;
}


function arquivosbancos_consolidado(id_env) {
  try {
    //document.view.action = '?pag=consolidar';
    document.view.envio.value = id_env;
    //document.view.submit();
    return true;
  }
  catch(e) {}
  return false;
}


function arquivosbancos_resumo() {
  try {
    //document.view.action = '?pag=resumo';
    document.view.envio.value = 0;
    //document.view.submit();
    return true;
  }
  catch(e) {}
  return false;
}




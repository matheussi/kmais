namespace www.usercontrols
{
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Collections;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    public partial class ucConferenciaPainelMensagem : System.Web.UI.UserControl
    {
        Hashtable messagesStack
        {
            get { return Session["___msg"] as Hashtable; }
            set { Session["___msg"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack) {}
        }

        /// <summary>
        /// Escreve mensagens de alerta ao usuário, caso existam.
        /// </summary>
        public void EscreveMensagens()
        {
            litMsg.Text = "";
            if (this.messagesStack == null) { return; }

            foreach (DictionaryEntry entry in this.messagesStack)
            {
                litMsg.Text += Convert.ToString(entry.Value) + "<br>";
            }
        }

        /// <summary>
        /// Inclui uma mensagem ao usuário na pilha de mensagens
        /// </summary>
        /// <param name="chave">Indexador da mensagem na pilha</param>
        /// <param name="msg">Mensagem a ser exibida ao usuário</param>
        public void SetaMsg(String chave, String msg)
        {
            if (this.messagesStack == null) { this.messagesStack = new Hashtable(); }
            this.messagesStack[chave] = "- " + msg;
            this.EscreveMensagens();
        }

        public void RemoveMsg(String chave)
        {
            if (this.messagesStack == null) { return; }
            this.messagesStack.Remove(chave);
            this.EscreveMensagens();
        }

        public void LimpaPilha()
        {
            if (this.messagesStack == null) { return; }
            this.messagesStack.Clear();
            litMsg.Text = "";
        }
    }
}
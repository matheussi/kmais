namespace Services
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerRetorno = new System.Windows.Forms.Timer(this.components);
            this.timerRemessa = new System.Windows.Forms.Timer(this.components);
            this.timerImportacao = new System.Windows.Forms.Timer(this.components);
            this.timerRemessa2 = new System.Windows.Forms.Timer(this.components);
            this.pb = new System.Windows.Forms.ProgressBar();
            this.cmdIniciar = new System.Windows.Forms.Button();
            this.lblMsg = new System.Windows.Forms.Label();
            this.lblPerc = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblMsgRemessa = new System.Windows.Forms.Label();
            this.cmdIniciarRemessa = new System.Windows.Forms.Button();
            this.lblPercRemessa = new System.Windows.Forms.Label();
            this.pbRemessa = new System.Windows.Forms.ProgressBar();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmdIniciarListagem = new System.Windows.Forms.Button();
            this.lblPercListagem = new System.Windows.Forms.Label();
            this.pbListagem = new System.Windows.Forms.ProgressBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblcount = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblRelatorio = new System.Windows.Forms.Label();
            this.cmdIniciarRelatorioFaturamento = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.timerRelatorioFinanceiro = new System.Windows.Forms.Timer(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtAgendaId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtArquivo = new System.Windows.Forms.TextBox();
            this.lblImportacaoStatus = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblMsgRemessa2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmdIniciarRemessa2 = new System.Windows.Forms.Button();
            this.lblPercRemessa2 = new System.Windows.Forms.Label();
            this.pbRemessa2 = new System.Windows.Forms.ProgressBar();
            this.timerComissao = new System.Windows.Forms.Timer(this.components);
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.lblRelAgend = new System.Windows.Forms.Label();
            this.cmdRelAgend = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timerRelatoriosAgendados = new System.Windows.Forms.Timer(this.components);
            this.timerMeuResultadoEMAILS = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerRetorno
            // 
            this.timerRetorno.Enabled = true;
            this.timerRetorno.Interval = 3600000;
            this.timerRetorno.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // timerRemessa
            // 
            this.timerRemessa.Interval = 600000;
            this.timerRemessa.Tick += new System.EventHandler(this.timerRemessa_Tick);
            // 
            // timerImportacao
            // 
            this.timerImportacao.Interval = 5900000;
            this.timerImportacao.Tick += new System.EventHandler(this.timerImportacao_Tick);
            // 
            // timerRemessa2
            // 
            this.timerRemessa2.Enabled = true;
            this.timerRemessa2.Interval = 300000;
            this.timerRemessa2.Tick += new System.EventHandler(this.timerRemessa2_Tick);
            // 
            // pb
            // 
            this.pb.Location = new System.Drawing.Point(6, 62);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(314, 23);
            this.pb.TabIndex = 0;
            // 
            // cmdIniciar
            // 
            this.cmdIniciar.Location = new System.Drawing.Point(6, 26);
            this.cmdIniciar.Name = "cmdIniciar";
            this.cmdIniciar.Size = new System.Drawing.Size(75, 23);
            this.cmdIniciar.TabIndex = 1;
            this.cmdIniciar.Text = "Iniciar";
            this.cmdIniciar.UseVisualStyleBackColor = true;
            this.cmdIniciar.Click += new System.EventHandler(this.cmdIniciar_Click);
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Location = new System.Drawing.Point(87, 31);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(0, 13);
            this.lblMsg.TabIndex = 2;
            // 
            // lblPerc
            // 
            this.lblPerc.AutoSize = true;
            this.lblPerc.BackColor = System.Drawing.Color.Transparent;
            this.lblPerc.ForeColor = System.Drawing.Color.Black;
            this.lblPerc.Location = new System.Drawing.Point(154, 67);
            this.lblPerc.Name = "lblPerc";
            this.lblPerc.Size = new System.Drawing.Size(19, 13);
            this.lblPerc.TabIndex = 3;
            this.lblPerc.Text = "00";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmdIniciar);
            this.groupBox1.Controls.Add(this.lblPerc);
            this.groupBox1.Controls.Add(this.lblMsg);
            this.groupBox1.Controls.Add(this.pb);
            this.groupBox1.Location = new System.Drawing.Point(1, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(326, 100);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Retorno de arquivo de remessa";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(87, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "[label1]";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblMsgRemessa);
            this.groupBox2.Controls.Add(this.cmdIniciarRemessa);
            this.groupBox2.Controls.Add(this.lblPercRemessa);
            this.groupBox2.Controls.Add(this.pbRemessa);
            this.groupBox2.Location = new System.Drawing.Point(9, 79);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(326, 100);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Gerar arquivo de remessa";
            this.groupBox2.Visible = false;
            // 
            // lblMsgRemessa
            // 
            this.lblMsgRemessa.AutoSize = true;
            this.lblMsgRemessa.Location = new System.Drawing.Point(87, 27);
            this.lblMsgRemessa.Name = "lblMsgRemessa";
            this.lblMsgRemessa.Size = new System.Drawing.Size(13, 13);
            this.lblMsgRemessa.TabIndex = 7;
            this.lblMsgRemessa.Text = "[]";
            // 
            // cmdIniciarRemessa
            // 
            this.cmdIniciarRemessa.Location = new System.Drawing.Point(6, 22);
            this.cmdIniciarRemessa.Name = "cmdIniciarRemessa";
            this.cmdIniciarRemessa.Size = new System.Drawing.Size(75, 23);
            this.cmdIniciarRemessa.TabIndex = 5;
            this.cmdIniciarRemessa.Text = "Iniciar";
            this.cmdIniciarRemessa.UseVisualStyleBackColor = true;
            this.cmdIniciarRemessa.Click += new System.EventHandler(this.cmdIniciarRemessa_Click);
            // 
            // lblPercRemessa
            // 
            this.lblPercRemessa.AutoSize = true;
            this.lblPercRemessa.BackColor = System.Drawing.Color.Transparent;
            this.lblPercRemessa.ForeColor = System.Drawing.Color.Black;
            this.lblPercRemessa.Location = new System.Drawing.Point(154, 62);
            this.lblPercRemessa.Name = "lblPercRemessa";
            this.lblPercRemessa.Size = new System.Drawing.Size(19, 13);
            this.lblPercRemessa.TabIndex = 6;
            this.lblPercRemessa.Text = "00";
            // 
            // pbRemessa
            // 
            this.pbRemessa.Location = new System.Drawing.Point(6, 57);
            this.pbRemessa.Name = "pbRemessa";
            this.pbRemessa.Size = new System.Drawing.Size(314, 23);
            this.pbRemessa.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmdIniciarListagem);
            this.groupBox3.Controls.Add(this.lblPercListagem);
            this.groupBox3.Controls.Add(this.pbListagem);
            this.groupBox3.Location = new System.Drawing.Point(1, 229);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(326, 100);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Fechar listagem de comissionamento";
            // 
            // cmdIniciarListagem
            // 
            this.cmdIniciarListagem.Enabled = false;
            this.cmdIniciarListagem.Location = new System.Drawing.Point(6, 22);
            this.cmdIniciarListagem.Name = "cmdIniciarListagem";
            this.cmdIniciarListagem.Size = new System.Drawing.Size(75, 23);
            this.cmdIniciarListagem.TabIndex = 5;
            this.cmdIniciarListagem.Text = "Iniciar";
            this.cmdIniciarListagem.UseVisualStyleBackColor = true;
            this.cmdIniciarListagem.Click += new System.EventHandler(this.cmdIniciarListagem_Click);
            // 
            // lblPercListagem
            // 
            this.lblPercListagem.AutoSize = true;
            this.lblPercListagem.BackColor = System.Drawing.Color.Transparent;
            this.lblPercListagem.ForeColor = System.Drawing.Color.Black;
            this.lblPercListagem.Location = new System.Drawing.Point(154, 62);
            this.lblPercListagem.Name = "lblPercListagem";
            this.lblPercListagem.Size = new System.Drawing.Size(19, 13);
            this.lblPercListagem.TabIndex = 6;
            this.lblPercListagem.Text = "00";
            // 
            // pbListagem
            // 
            this.pbListagem.Location = new System.Drawing.Point(6, 57);
            this.pbListagem.Name = "pbListagem";
            this.pbListagem.Size = new System.Drawing.Size(314, 23);
            this.pbListagem.TabIndex = 4;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(678, 229);
            this.textBox1.MaxLength = 1999999999;
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(139, 92);
            this.textBox1.TabIndex = 7;
            this.textBox1.Visible = false;
            // 
            // lblcount
            // 
            this.lblcount.AutoSize = true;
            this.lblcount.Location = new System.Drawing.Point(791, 256);
            this.lblcount.Name = "lblcount";
            this.lblcount.Size = new System.Drawing.Size(35, 13);
            this.lblcount.TabIndex = 9;
            this.lblcount.Text = "label1";
            this.lblcount.Visible = false;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(757, 285);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.lblRelatorio);
            this.groupBox4.Controls.Add(this.cmdIniciarRelatorioFaturamento);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Location = new System.Drawing.Point(352, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(326, 100);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Relatório de Faturamento";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(232, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "[label7]";
            // 
            // lblRelatorio
            // 
            this.lblRelatorio.AutoSize = true;
            this.lblRelatorio.Location = new System.Drawing.Point(6, 68);
            this.lblRelatorio.Name = "lblRelatorio";
            this.lblRelatorio.Size = new System.Drawing.Size(76, 13);
            this.lblRelatorio.TabIndex = 11;
            this.lblRelatorio.Text = "Status: parado";
            // 
            // cmdIniciarRelatorioFaturamento
            // 
            this.cmdIniciarRelatorioFaturamento.Enabled = false;
            this.cmdIniciarRelatorioFaturamento.Location = new System.Drawing.Point(6, 26);
            this.cmdIniciarRelatorioFaturamento.Name = "cmdIniciarRelatorioFaturamento";
            this.cmdIniciarRelatorioFaturamento.Size = new System.Drawing.Size(75, 23);
            this.cmdIniciarRelatorioFaturamento.TabIndex = 1;
            this.cmdIniciarRelatorioFaturamento.Text = "Iniciar";
            this.cmdIniciarRelatorioFaturamento.UseVisualStyleBackColor = true;
            this.cmdIniciarRelatorioFaturamento.Click += new System.EventHandler(this.cmdIniciarRelatorioFaturamento_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(87, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 2;
            // 
            // timerRelatorioFinanceiro
            // 
            this.timerRelatorioFinanceiro.Interval = 3690000;
            this.timerRelatorioFinanceiro.Tick += new System.EventHandler(this.timerRelatorioFinanceiro_Tick);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.groupBox2);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.txtAgendaId);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.txtArquivo);
            this.groupBox5.Controls.Add(this.lblImportacaoStatus);
            this.groupBox5.Controls.Add(this.button2);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Location = new System.Drawing.Point(352, 227);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(326, 100);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Importação AMIL";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(124, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Agenda ID";
            this.label6.Visible = false;
            // 
            // txtAgendaId
            // 
            this.txtAgendaId.Location = new System.Drawing.Point(127, 71);
            this.txtAgendaId.MaxLength = 10;
            this.txtAgendaId.Name = "txtAgendaId";
            this.txtAgendaId.Size = new System.Drawing.Size(73, 20);
            this.txtAgendaId.TabIndex = 14;
            this.txtAgendaId.Text = "0";
            this.txtAgendaId.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(124, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Arquivo";
            this.label5.Visible = false;
            // 
            // txtArquivo
            // 
            this.txtArquivo.Location = new System.Drawing.Point(127, 29);
            this.txtArquivo.MaxLength = 100;
            this.txtArquivo.Name = "txtArquivo";
            this.txtArquivo.Size = new System.Drawing.Size(193, 20);
            this.txtArquivo.TabIndex = 12;
            this.txtArquivo.Visible = false;
            // 
            // lblImportacaoStatus
            // 
            this.lblImportacaoStatus.AutoSize = true;
            this.lblImportacaoStatus.Location = new System.Drawing.Point(9, 63);
            this.lblImportacaoStatus.Name = "lblImportacaoStatus";
            this.lblImportacaoStatus.Size = new System.Drawing.Size(76, 13);
            this.lblImportacaoStatus.TabIndex = 11;
            this.lblImportacaoStatus.Text = "Status: parado";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 26);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Iniciar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(87, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 2;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lblMsgRemessa2);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.cmdIniciarRemessa2);
            this.groupBox6.Controls.Add(this.lblPercRemessa2);
            this.groupBox6.Controls.Add(this.pbRemessa2);
            this.groupBox6.Location = new System.Drawing.Point(352, 118);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(326, 100);
            this.groupBox6.TabIndex = 12;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Gerar arquivo de remessa (NOVO FORMATO)";
            // 
            // lblMsgRemessa2
            // 
            this.lblMsgRemessa2.AutoSize = true;
            this.lblMsgRemessa2.Location = new System.Drawing.Point(87, 27);
            this.lblMsgRemessa2.Name = "lblMsgRemessa2";
            this.lblMsgRemessa2.Size = new System.Drawing.Size(13, 13);
            this.lblMsgRemessa2.TabIndex = 8;
            this.lblMsgRemessa2.Text = "[]";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(120, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 7;
            // 
            // cmdIniciarRemessa2
            // 
            this.cmdIniciarRemessa2.Location = new System.Drawing.Point(6, 22);
            this.cmdIniciarRemessa2.Name = "cmdIniciarRemessa2";
            this.cmdIniciarRemessa2.Size = new System.Drawing.Size(75, 23);
            this.cmdIniciarRemessa2.TabIndex = 5;
            this.cmdIniciarRemessa2.Text = "Iniciar";
            this.cmdIniciarRemessa2.UseVisualStyleBackColor = true;
            this.cmdIniciarRemessa2.Click += new System.EventHandler(this.cmdIniciarRemessa2_Click);
            // 
            // lblPercRemessa2
            // 
            this.lblPercRemessa2.AutoSize = true;
            this.lblPercRemessa2.BackColor = System.Drawing.Color.Transparent;
            this.lblPercRemessa2.ForeColor = System.Drawing.Color.Black;
            this.lblPercRemessa2.Location = new System.Drawing.Point(154, 62);
            this.lblPercRemessa2.Name = "lblPercRemessa2";
            this.lblPercRemessa2.Size = new System.Drawing.Size(19, 13);
            this.lblPercRemessa2.TabIndex = 6;
            this.lblPercRemessa2.Text = "00";
            // 
            // pbRemessa2
            // 
            this.pbRemessa2.Location = new System.Drawing.Point(6, 57);
            this.pbRemessa2.Name = "pbRemessa2";
            this.pbRemessa2.Size = new System.Drawing.Size(314, 23);
            this.pbRemessa2.TabIndex = 4;
            // 
            // timerComissao
            // 
            this.timerComissao.Interval = 2600000;
            this.timerComissao.Tick += new System.EventHandler(this.timerComissao_Tick);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.lblRelAgend);
            this.groupBox7.Controls.Add(this.cmdRelAgend);
            this.groupBox7.Controls.Add(this.label9);
            this.groupBox7.Controls.Add(this.progressBar1);
            this.groupBox7.Location = new System.Drawing.Point(1, 118);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(326, 100);
            this.groupBox7.TabIndex = 13;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Relatórios agendados";
            // 
            // lblRelAgend
            // 
            this.lblRelAgend.AutoSize = true;
            this.lblRelAgend.Location = new System.Drawing.Point(87, 27);
            this.lblRelAgend.Name = "lblRelAgend";
            this.lblRelAgend.Size = new System.Drawing.Size(13, 13);
            this.lblRelAgend.TabIndex = 7;
            this.lblRelAgend.Text = "[]";
            // 
            // cmdRelAgend
            // 
            this.cmdRelAgend.Location = new System.Drawing.Point(6, 22);
            this.cmdRelAgend.Name = "cmdRelAgend";
            this.cmdRelAgend.Size = new System.Drawing.Size(75, 23);
            this.cmdRelAgend.TabIndex = 5;
            this.cmdRelAgend.Text = "Iniciar";
            this.cmdRelAgend.UseVisualStyleBackColor = true;
            this.cmdRelAgend.Click += new System.EventHandler(this.cmdRelAgend_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(154, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(19, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "00";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(6, 57);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(314, 23);
            this.progressBar1.TabIndex = 4;
            // 
            // timerRelatoriosAgendados
            // 
            this.timerRelatoriosAgendados.Interval = 300000;
            this.timerRelatoriosAgendados.Tick += new System.EventHandler(this.timerRelatoriosAgendados_Tick);
            // 
            // timerMeuResultadoEMAILS
            // 
            this.timerMeuResultadoEMAILS.Enabled = true;
            this.timerMeuResultadoEMAILS.Interval = 3600000;
            this.timerMeuResultadoEMAILS.Tick += new System.EventHandler(this.timerMeuResultadoEMAILS_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 339);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.lblcount);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Serviços (V-BB)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pb;
        private System.Windows.Forms.Button cmdIniciar;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Label lblPerc;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdIniciarRemessa;
        private System.Windows.Forms.Label lblPercRemessa;
        private System.Windows.Forms.ProgressBar pbRemessa;
        private System.Windows.Forms.Label lblMsgRemessa;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button cmdIniciarListagem;
        private System.Windows.Forms.Label lblPercListagem;
        private System.Windows.Forms.ProgressBar pbListagem;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblcount;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button cmdIniciarRelatorioFaturamento;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblRelatorio;
        private System.Windows.Forms.Timer timerRelatorioFinanceiro;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtAgendaId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtArquivo;
        private System.Windows.Forms.Label lblImportacaoStatus;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cmdIniciarRemessa2;
        private System.Windows.Forms.Label lblPercRemessa2;
        private System.Windows.Forms.ProgressBar pbRemessa2;
        private System.Windows.Forms.Label lblMsgRemessa2;
        private System.Windows.Forms.Timer timerRetorno;
        private System.Windows.Forms.Timer timerRemessa;
        private System.Windows.Forms.Timer timerImportacao;
        private System.Windows.Forms.Timer timerRemessa2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Timer timerComissao;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label lblRelAgend;
        private System.Windows.Forms.Button cmdRelAgend;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timerRelatoriosAgendados;
        private System.Windows.Forms.Timer timerMeuResultadoEMAILS;
    }
}
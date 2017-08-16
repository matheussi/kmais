namespace Export
{
    partial class Form1
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdCadProcessar = new System.Windows.Forms.Button();
            this.txtCaminhoBase = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNomeArqCadastro = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtNomeArqCadastro);
            this.groupBox1.Controls.Add(this.cmdCadProcessar);
            this.groupBox1.Location = new System.Drawing.Point(12, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 107);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cadastro";
            // 
            // cmdCadProcessar
            // 
            this.cmdCadProcessar.Location = new System.Drawing.Point(44, 60);
            this.cmdCadProcessar.Name = "cmdCadProcessar";
            this.cmdCadProcessar.Size = new System.Drawing.Size(140, 23);
            this.cmdCadProcessar.TabIndex = 0;
            this.cmdCadProcessar.Text = "Processar dados";
            this.cmdCadProcessar.UseVisualStyleBackColor = true;
            this.cmdCadProcessar.Click += new System.EventHandler(this.cmdCadProcessar_Click);
            // 
            // txtCaminhoBase
            // 
            this.txtCaminhoBase.Location = new System.Drawing.Point(12, 30);
            this.txtCaminhoBase.Name = "txtCaminhoBase";
            this.txtCaminhoBase.Size = new System.Drawing.Size(250, 20);
            this.txtCaminhoBase.TabIndex = 1;
            this.txtCaminhoBase.Text = "c:\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Caminho base";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Nome do arquivo";
            // 
            // txtNomeArqCadastro
            // 
            this.txtNomeArqCadastro.Location = new System.Drawing.Point(100, 23);
            this.txtNomeArqCadastro.Name = "txtNomeArqCadastro";
            this.txtNomeArqCadastro.Size = new System.Drawing.Size(136, 20);
            this.txtNomeArqCadastro.TabIndex = 3;
            this.txtNomeArqCadastro.Text = "cadastro.txt";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCaminhoBase);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Exportação";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmdCadProcessar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNomeArqCadastro;
        private System.Windows.Forms.TextBox txtCaminhoBase;
        private System.Windows.Forms.Label label1;
    }
}


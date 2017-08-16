namespace ImportData
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
            this.cmdImportarCobrancas = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmdImportarCorretores = new System.Windows.Forms.Button();
            this.cmdBeneficiarios = new System.Windows.Forms.Button();
            this.cmdImportarEnderecos = new System.Windows.Forms.Button();
            this.cmdImportarPropostas = new System.Windows.Forms.Button();
            this.txtErros = new System.Windows.Forms.TextBox();
            this.cmdPropBenef = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdImportarFiliaisParaProdutores = new System.Windows.Forms.Button();
            this.cmdImportarGruposDeVendaParaProdutores = new System.Windows.Forms.Button();
            this.cmdImportarEquipesParaProdutores = new System.Windows.Forms.Button();
            this.cmdImportarTabelasValor = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdImportarCobrancas
            // 
            this.cmdImportarCobrancas.Enabled = false;
            this.cmdImportarCobrancas.Location = new System.Drawing.Point(69, 167);
            this.cmdImportarCobrancas.Name = "cmdImportarCobrancas";
            this.cmdImportarCobrancas.Size = new System.Drawing.Size(75, 23);
            this.cmdImportarCobrancas.TabIndex = 1;
            this.cmdImportarCobrancas.Text = "Cobranças";
            this.cmdImportarCobrancas.UseVisualStyleBackColor = true;
            this.cmdImportarCobrancas.Click += new System.EventHandler(this.cmdImportarCobrancas_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(411, 8);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(35, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "label1";
            // 
            // cmdImportarCorretores
            // 
            this.cmdImportarCorretores.Enabled = false;
            this.cmdImportarCorretores.Location = new System.Drawing.Point(69, 22);
            this.cmdImportarCorretores.Name = "cmdImportarCorretores";
            this.cmdImportarCorretores.Size = new System.Drawing.Size(75, 23);
            this.cmdImportarCorretores.TabIndex = 0;
            this.cmdImportarCorretores.Text = "Corretores";
            this.cmdImportarCorretores.UseVisualStyleBackColor = true;
            this.cmdImportarCorretores.Click += new System.EventHandler(this.cmdImportarCorretores_Click);
            // 
            // cmdBeneficiarios
            // 
            this.cmdBeneficiarios.Enabled = false;
            this.cmdBeneficiarios.Location = new System.Drawing.Point(69, 51);
            this.cmdBeneficiarios.Name = "cmdBeneficiarios";
            this.cmdBeneficiarios.Size = new System.Drawing.Size(75, 23);
            this.cmdBeneficiarios.TabIndex = 3;
            this.cmdBeneficiarios.Text = "Beneficiários";
            this.cmdBeneficiarios.UseVisualStyleBackColor = true;
            this.cmdBeneficiarios.Click += new System.EventHandler(this.cmdBeneficiarios_Click);
            // 
            // cmdImportarEnderecos
            // 
            this.cmdImportarEnderecos.Enabled = false;
            this.cmdImportarEnderecos.Location = new System.Drawing.Point(69, 80);
            this.cmdImportarEnderecos.Name = "cmdImportarEnderecos";
            this.cmdImportarEnderecos.Size = new System.Drawing.Size(75, 23);
            this.cmdImportarEnderecos.TabIndex = 4;
            this.cmdImportarEnderecos.Text = "Endereços";
            this.cmdImportarEnderecos.UseVisualStyleBackColor = true;
            this.cmdImportarEnderecos.Click += new System.EventHandler(this.cmdImportarEnderecos_Click);
            // 
            // cmdImportarPropostas
            // 
            this.cmdImportarPropostas.Enabled = false;
            this.cmdImportarPropostas.Location = new System.Drawing.Point(69, 109);
            this.cmdImportarPropostas.Name = "cmdImportarPropostas";
            this.cmdImportarPropostas.Size = new System.Drawing.Size(75, 23);
            this.cmdImportarPropostas.TabIndex = 5;
            this.cmdImportarPropostas.Text = "Propostas";
            this.cmdImportarPropostas.UseVisualStyleBackColor = true;
            this.cmdImportarPropostas.Click += new System.EventHandler(this.cmdImportarPropostas_Click);
            // 
            // txtErros
            // 
            this.txtErros.Location = new System.Drawing.Point(217, 24);
            this.txtErros.Multiline = true;
            this.txtErros.Name = "txtErros";
            this.txtErros.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtErros.Size = new System.Drawing.Size(497, 444);
            this.txtErros.TabIndex = 6;
            this.txtErros.WordWrap = false;
            // 
            // cmdPropBenef
            // 
            this.cmdPropBenef.Enabled = false;
            this.cmdPropBenef.Location = new System.Drawing.Point(69, 138);
            this.cmdPropBenef.Name = "cmdPropBenef";
            this.cmdPropBenef.Size = new System.Drawing.Size(75, 23);
            this.cmdPropBenef.TabIndex = 7;
            this.cmdPropBenef.Text = "Pro X Benef";
            this.cmdPropBenef.UseVisualStyleBackColor = true;
            this.cmdPropBenef.Click += new System.EventHandler(this.cmdPropBenef_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(12, 209);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 10);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // cmdImportarFiliaisParaProdutores
            // 
            this.cmdImportarFiliaisParaProdutores.Enabled = false;
            this.cmdImportarFiliaisParaProdutores.Location = new System.Drawing.Point(69, 246);
            this.cmdImportarFiliaisParaProdutores.Name = "cmdImportarFiliaisParaProdutores";
            this.cmdImportarFiliaisParaProdutores.Size = new System.Drawing.Size(75, 23);
            this.cmdImportarFiliaisParaProdutores.TabIndex = 9;
            this.cmdImportarFiliaisParaProdutores.Text = "Setar Filiais";
            this.cmdImportarFiliaisParaProdutores.UseVisualStyleBackColor = true;
            this.cmdImportarFiliaisParaProdutores.Click += new System.EventHandler(this.cmdImportarFiliaisParaProdutores_Click);
            // 
            // cmdImportarGruposDeVendaParaProdutores
            // 
            this.cmdImportarGruposDeVendaParaProdutores.Enabled = false;
            this.cmdImportarGruposDeVendaParaProdutores.Location = new System.Drawing.Point(69, 275);
            this.cmdImportarGruposDeVendaParaProdutores.Name = "cmdImportarGruposDeVendaParaProdutores";
            this.cmdImportarGruposDeVendaParaProdutores.Size = new System.Drawing.Size(75, 23);
            this.cmdImportarGruposDeVendaParaProdutores.TabIndex = 10;
            this.cmdImportarGruposDeVendaParaProdutores.Text = "Setar Grupo";
            this.cmdImportarGruposDeVendaParaProdutores.UseVisualStyleBackColor = true;
            this.cmdImportarGruposDeVendaParaProdutores.Click += new System.EventHandler(this.cmdImportarGruposDeVendaParaProdutores_Click);
            // 
            // cmdImportarEquipesParaProdutores
            // 
            this.cmdImportarEquipesParaProdutores.Enabled = false;
            this.cmdImportarEquipesParaProdutores.Location = new System.Drawing.Point(69, 304);
            this.cmdImportarEquipesParaProdutores.Name = "cmdImportarEquipesParaProdutores";
            this.cmdImportarEquipesParaProdutores.Size = new System.Drawing.Size(75, 23);
            this.cmdImportarEquipesParaProdutores.TabIndex = 11;
            this.cmdImportarEquipesParaProdutores.Text = "Setar Equip";
            this.cmdImportarEquipesParaProdutores.UseVisualStyleBackColor = true;
            this.cmdImportarEquipesParaProdutores.Click += new System.EventHandler(this.cmdImportarEquipesParaProdutores_Click);
            // 
            // cmdImportarTabelasValor
            // 
            this.cmdImportarTabelasValor.Enabled = false;
            this.cmdImportarTabelasValor.Location = new System.Drawing.Point(69, 333);
            this.cmdImportarTabelasValor.Name = "cmdImportarTabelasValor";
            this.cmdImportarTabelasValor.Size = new System.Drawing.Size(75, 23);
            this.cmdImportarTabelasValor.TabIndex = 12;
            this.cmdImportarTabelasValor.Text = "Tabela Valor";
            this.cmdImportarTabelasValor.UseVisualStyleBackColor = true;
            this.cmdImportarTabelasValor.Click += new System.EventHandler(this.cmdImportarTabelasValor_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(69, 427);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 480);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmdImportarTabelasValor);
            this.Controls.Add(this.cmdImportarEquipesParaProdutores);
            this.Controls.Add(this.cmdImportarGruposDeVendaParaProdutores);
            this.Controls.Add(this.cmdImportarFiliaisParaProdutores);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdPropBenef);
            this.Controls.Add(this.txtErros);
            this.Controls.Add(this.cmdImportarPropostas);
            this.Controls.Add(this.cmdImportarEnderecos);
            this.Controls.Add(this.cmdBeneficiarios);
            this.Controls.Add(this.cmdImportarCorretores);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.cmdImportarCobrancas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Importações";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdImportarCobrancas;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button cmdImportarCorretores;
        private System.Windows.Forms.Button cmdBeneficiarios;
        private System.Windows.Forms.Button cmdImportarEnderecos;
        private System.Windows.Forms.Button cmdImportarPropostas;
        private System.Windows.Forms.TextBox txtErros;
        private System.Windows.Forms.Button cmdPropBenef;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmdImportarFiliaisParaProdutores;
        private System.Windows.Forms.Button cmdImportarGruposDeVendaParaProdutores;
        private System.Windows.Forms.Button cmdImportarEquipesParaProdutores;
        private System.Windows.Forms.Button cmdImportarTabelasValor;
        private System.Windows.Forms.Button button1;
    }
}


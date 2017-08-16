namespace LabelPrinting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.printDoc = new System.Drawing.Printing.PrintDocument();
            this.printPreviewDialog = new System.Windows.Forms.PrintPreviewDialog();
            this.printPreviewButton = new System.Windows.Forms.Button();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.cmdAbrir = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboDadosCarregados = new System.Windows.Forms.ComboBox();
            this.lblSumario = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmdVisualiar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cboFormato = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // printPreviewDialog
            // 
            this.printPreviewDialog.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog.Enabled = true;
            this.printPreviewDialog.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog.Icon")));
            this.printPreviewDialog.Name = "printPreviewDialog";
            this.printPreviewDialog.Visible = false;
            // 
            // printPreviewButton
            // 
            this.printPreviewButton.Location = new System.Drawing.Point(326, 81);
            this.printPreviewButton.Name = "printPreviewButton";
            this.printPreviewButton.Size = new System.Drawing.Size(172, 23);
            this.printPreviewButton.TabIndex = 0;
            this.printPreviewButton.Text = "Imprimir";
            this.printPreviewButton.UseVisualStyleBackColor = true;
            this.printPreviewButton.Click += new System.EventHandler(this.printPreviewButton_Click);
            // 
            // cmdAbrir
            // 
            this.cmdAbrir.Location = new System.Drawing.Point(131, 29);
            this.cmdAbrir.Name = "cmdAbrir";
            this.cmdAbrir.Size = new System.Drawing.Size(243, 23);
            this.cmdAbrir.TabIndex = 1;
            this.cmdAbrir.Text = "Informe o arquivo de dados";
            this.cmdAbrir.UseVisualStyleBackColor = true;
            this.cmdAbrir.Click += new System.EventHandler(this.cmdAbrir_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboDadosCarregados);
            this.groupBox1.Controls.Add(this.lblSumario);
            this.groupBox1.Controls.Add(this.cmdAbrir);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(504, 116);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Obter Dados";
            // 
            // cboDadosCarregados
            // 
            this.cboDadosCarregados.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDadosCarregados.FormattingEnabled = true;
            this.cboDadosCarregados.Location = new System.Drawing.Point(6, 76);
            this.cboDadosCarregados.Name = "cboDadosCarregados";
            this.cboDadosCarregados.Size = new System.Drawing.Size(492, 21);
            this.cboDadosCarregados.TabIndex = 3;
            // 
            // lblSumario
            // 
            this.lblSumario.AutoSize = true;
            this.lblSumario.Location = new System.Drawing.Point(199, 59);
            this.lblSumario.Name = "lblSumario";
            this.lblSumario.Size = new System.Drawing.Size(106, 13);
            this.lblSumario.TabIndex = 2;
            this.lblSumario.Text = "Dados carregados: 0";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmdVisualiar);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.printPreviewButton);
            this.groupBox2.Location = new System.Drawing.Point(12, 131);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(504, 118);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Impressão";
            // 
            // cmdVisualiar
            // 
            this.cmdVisualiar.Location = new System.Drawing.Point(6, 81);
            this.cmdVisualiar.Name = "cmdVisualiar";
            this.cmdVisualiar.Size = new System.Drawing.Size(172, 23);
            this.cmdVisualiar.TabIndex = 6;
            this.cmdVisualiar.Text = "Visualizar impressão";
            this.cmdVisualiar.UseVisualStyleBackColor = true;
            this.cmdVisualiar.Click += new System.EventHandler(this.cmdVisualiar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Formato";
            // 
            // cboFormato
            // 
            this.cboFormato.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFormato.FormattingEnabled = true;
            this.cboFormato.Items.AddRange(new object[] {
            "Etiqueta Pimaco 7072"});
            this.cboFormato.Location = new System.Drawing.Point(18, 176);
            this.cboFormato.Name = "cboFormato";
            this.cboFormato.Size = new System.Drawing.Size(492, 21);
            this.cboFormato.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 268);
            this.Controls.Add(this.cboFormato);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Etiquetas";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Drawing.Printing.PrintDocument printDoc;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog;
        private System.Windows.Forms.Button printPreviewButton;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.Button cmdAbrir;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboDadosCarregados;
        private System.Windows.Forms.Label lblSumario;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboFormato;
        private System.Windows.Forms.Button cmdVisualiar;
    }
}


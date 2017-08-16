namespace Export
{
    partial class frmImportUBRASP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportUBRASP));
            this.cmdPessoas = new System.Windows.Forms.Button();
            this.lblPessoas = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmdPessoas
            // 
            resources.ApplyResources(this.cmdPessoas, "cmdPessoas");
            this.cmdPessoas.Name = "cmdPessoas";
            this.cmdPessoas.UseVisualStyleBackColor = true;
            this.cmdPessoas.Click += new System.EventHandler(this.cmdPessoas_Click);
            // 
            // lblPessoas
            // 
            resources.ApplyResources(this.lblPessoas, "lblPessoas");
            this.lblPessoas.Name = "lblPessoas";
            // 
            // frmImportUBRASP
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblPessoas);
            this.Controls.Add(this.cmdPessoas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportUBRASP";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdPessoas;
        private System.Windows.Forms.Label lblPessoas;
    }
}
namespace CM50_Configurator
{
    partial class StartForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lstModules = new System.Windows.Forms.ListBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(12, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(211, 13);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Seleziona il tipo di lavoro da effettuare:";
            //
            // lstModules
            //
            this.lstModules.FormattingEnabled = true;
            this.lstModules.ItemHeight = 15;
            this.lstModules.Location = new System.Drawing.Point(12, 35);
            this.lstModules.Name = "lstModules";
            this.lstModules.Size = new System.Drawing.Size(460, 124);
            this.lstModules.TabIndex = 1;
            this.lstModules.DoubleClick += new System.EventHandler(this.lstModules_DoubleClick);
            //
            // lblDescription
            //
            this.lblDescription.Location = new System.Drawing.Point(12, 168);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(460, 60);
            this.lblDescription.TabIndex = 2;
            //
            // btnOpen
            //
            this.btnOpen.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnOpen.Location = new System.Drawing.Point(352, 236);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(120, 27);
            this.btnOpen.TabIndex = 3;
            this.btnOpen.Text = "Apri";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            //
            // StartForm
            //
            this.AcceptButton = this.btnOpen;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 275);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lstModules);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StartForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CM50 Configurator - Seleziona modulo";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ListBox lstModules;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button btnOpen;
    }
}

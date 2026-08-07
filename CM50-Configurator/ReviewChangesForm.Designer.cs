namespace CM50_Configurator
{
    partial class ReviewChangesForm
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
            this.lblLegend = new System.Windows.Forms.Label();
            this.dgvChanges = new System.Windows.Forms.DataGridView();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnWrite = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChanges)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // lblLegend
            //
            this.lblLegend.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLegend.Location = new System.Drawing.Point(0, 0);
            this.lblLegend.Name = "lblLegend";
            this.lblLegend.Padding = new System.Windows.Forms.Padding(12, 12, 12, 8);
            this.lblLegend.Size = new System.Drawing.Size(860, 34);
            this.lblLegend.TabIndex = 0;
            this.lblLegend.Text = "Verde = riga nuova · Giallo = riga modificata. Le righe invariate non sono mostrate.";
            //
            // dgvChanges
            //
            this.dgvChanges.AllowUserToAddRows = false;
            this.dgvChanges.AllowUserToDeleteRows = false;
            this.dgvChanges.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChanges.Location = new System.Drawing.Point(0, 34);
            this.dgvChanges.Name = "dgvChanges";
            this.dgvChanges.ReadOnly = true;
            this.dgvChanges.RowHeadersWidth = 25;
            this.dgvChanges.Size = new System.Drawing.Size(860, 366);
            this.dgvChanges.TabIndex = 1;
            //
            // pnlButtons
            //
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnWrite);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 400);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(860, 50);
            this.pnlButtons.TabIndex = 2;
            //
            // btnCancel
            //
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.Location = new System.Drawing.Point(736, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Annulla";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnWrite
            //
            this.btnWrite.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnWrite.Location = new System.Drawing.Point(596, 11);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(130, 27);
            this.btnWrite.TabIndex = 0;
            this.btnWrite.Text = "Scrivi su DB";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            //
            // ReviewChangesForm
            //
            this.AcceptButton = this.btnWrite;
            this.CancelButton = this.btnCancel;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 450);
            this.Controls.Add(this.dgvChanges);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.lblLegend);
            this.MinimumSize = new System.Drawing.Size(600, 350);
            this.Name = "ReviewChangesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Revisione modifiche prima della scrittura sul DB";
            ((System.ComponentModel.ISupportInitialize)(this.dgvChanges)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblLegend;
        private System.Windows.Forms.DataGridView dgvChanges;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnWrite;
    }
}

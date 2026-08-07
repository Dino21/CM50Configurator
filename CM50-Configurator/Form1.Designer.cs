namespace CM50_Configurator
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvUserCustomizations = new System.Windows.Forms.DataGridView();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnDetail = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnImportJson = new System.Windows.Forms.Button();
            this.btnReviewAndWrite = new System.Windows.Forms.Button();
            this.btnExportJson = new System.Windows.Forms.Button();
            this.btnLoadFromDb = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserCustomizations)).BeginInit();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // dgvUserCustomizations
            //
            this.dgvUserCustomizations.AllowUserToAddRows = false;
            this.dgvUserCustomizations.AllowUserToDeleteRows = false;
            this.dgvUserCustomizations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUserCustomizations.Location = new System.Drawing.Point(0, 0);
            this.dgvUserCustomizations.Name = "dgvUserCustomizations";
            this.dgvUserCustomizations.ReadOnly = true;
            this.dgvUserCustomizations.RowHeadersWidth = 25;
            this.dgvUserCustomizations.Size = new System.Drawing.Size(984, 521);
            this.dgvUserCustomizations.TabIndex = 0;
            this.dgvUserCustomizations.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUserCustomizations_CellDoubleClick);
            //
            // pnlActions
            //
            this.pnlActions.Controls.Add(this.lblStatus);
            this.pnlActions.Controls.Add(this.btnDetail);
            this.pnlActions.Controls.Add(this.btnAddNew);
            this.pnlActions.Controls.Add(this.btnImportJson);
            this.pnlActions.Controls.Add(this.btnReviewAndWrite);
            this.pnlActions.Controls.Add(this.btnExportJson);
            this.pnlActions.Controls.Add(this.btnLoadFromDb);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActions.Location = new System.Drawing.Point(0, 521);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(984, 90);
            this.pnlActions.TabIndex = 1;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(600, 37);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 6;
            //
            // btnDetail
            //
            this.btnDetail.Location = new System.Drawing.Point(316, 8);
            this.btnDetail.Name = "btnDetail";
            this.btnDetail.Size = new System.Drawing.Size(150, 27);
            this.btnDetail.TabIndex = 2;
            this.btnDetail.Text = "Dettaglio / Modifica...";
            this.btnDetail.UseVisualStyleBackColor = true;
            this.btnDetail.Click += new System.EventHandler(this.btnDetail_Click);
            //
            // btnAddNew
            //
            this.btnAddNew.Location = new System.Drawing.Point(144, 8);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(160, 27);
            this.btnAddNew.TabIndex = 1;
            this.btnAddNew.Text = "Nuova Configurazione...";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            //
            // btnImportJson
            //
            this.btnImportJson.Location = new System.Drawing.Point(12, 41);
            this.btnImportJson.Name = "btnImportJson";
            this.btnImportJson.Size = new System.Drawing.Size(160, 27);
            this.btnImportJson.TabIndex = 3;
            this.btnImportJson.Text = "Importa JSON...";
            this.btnImportJson.UseVisualStyleBackColor = true;
            this.btnImportJson.Click += new System.EventHandler(this.btnImportJson_Click);
            //
            // btnReviewAndWrite
            //
            this.btnReviewAndWrite.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnReviewAndWrite.Location = new System.Drawing.Point(184, 41);
            this.btnReviewAndWrite.Name = "btnReviewAndWrite";
            this.btnReviewAndWrite.Size = new System.Drawing.Size(160, 27);
            this.btnReviewAndWrite.TabIndex = 4;
            this.btnReviewAndWrite.Text = "Scrivi su DB...";
            this.btnReviewAndWrite.UseVisualStyleBackColor = true;
            this.btnReviewAndWrite.Click += new System.EventHandler(this.btnReviewAndWrite_Click);
            //
            // btnExportJson
            //
            this.btnExportJson.Enabled = false;
            this.btnExportJson.Location = new System.Drawing.Point(356, 41);
            this.btnExportJson.Name = "btnExportJson";
            this.btnExportJson.Size = new System.Drawing.Size(120, 27);
            this.btnExportJson.TabIndex = 5;
            this.btnExportJson.Text = "Esporta JSON...";
            this.btnExportJson.UseVisualStyleBackColor = true;
            this.btnExportJson.Click += new System.EventHandler(this.btnExportJson_Click);
            //
            // btnLoadFromDb
            //
            this.btnLoadFromDb.Location = new System.Drawing.Point(12, 8);
            this.btnLoadFromDb.Name = "btnLoadFromDb";
            this.btnLoadFromDb.Size = new System.Drawing.Size(120, 27);
            this.btnLoadFromDb.TabIndex = 0;
            this.btnLoadFromDb.Text = "Carica da DB";
            this.btnLoadFromDb.UseVisualStyleBackColor = true;
            this.btnLoadFromDb.Click += new System.EventHandler(this.btnLoadFromDb_Click);
            //
            // Form1
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 611);
            this.Controls.Add(this.dgvUserCustomizations);
            this.Controls.Add(this.pnlActions);
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.Name = "Form1";
            this.Text = "CM50 Configurator - Personalizzazioni Utente";
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserCustomizations)).EndInit();
            this.pnlActions.ResumeLayout(false);
            this.pnlActions.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvUserCustomizations;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnDetail;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnImportJson;
        private System.Windows.Forms.Button btnReviewAndWrite;
        private System.Windows.Forms.Button btnExportJson;
        private System.Windows.Forms.Button btnLoadFromDb;
    }
}


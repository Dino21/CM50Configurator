namespace CM50_Configurator
{
    partial class NewUserCustomizationForm
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.cboClient = new System.Windows.Forms.ComboBox();
            this.lblClient = new System.Windows.Forms.Label();
            this.cboProduct = new System.Windows.Forms.ComboBox();
            this.lblProduct = new System.Windows.Forms.Label();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlTop
            //
            this.pnlTop.Controls.Add(this.cboClient);
            this.pnlTop.Controls.Add(this.lblClient);
            this.pnlTop.Controls.Add(this.cboProduct);
            this.pnlTop.Controls.Add(this.lblProduct);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(480, 70);
            this.pnlTop.TabIndex = 0;
            //
            // lblProduct
            //
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(12, 15);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(51, 13);
            this.lblProduct.TabIndex = 0;
            this.lblProduct.Text = "Prodotto*:";
            //
            // cboProduct
            //
            this.cboProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProduct.FormattingEnabled = true;
            this.cboProduct.Location = new System.Drawing.Point(90, 12);
            this.cboProduct.Name = "cboProduct";
            this.cboProduct.Size = new System.Drawing.Size(378, 21);
            this.cboProduct.TabIndex = 1;
            //
            // lblClient
            //
            this.lblClient.AutoSize = true;
            this.lblClient.Location = new System.Drawing.Point(12, 42);
            this.lblClient.Name = "lblClient";
            this.lblClient.Size = new System.Drawing.Size(46, 13);
            this.lblClient.TabIndex = 2;
            this.lblClient.Text = "Cliente*:";
            //
            // cboClient
            //
            this.cboClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClient.FormattingEnabled = true;
            this.cboClient.Location = new System.Drawing.Point(90, 39);
            this.cboClient.Name = "cboClient";
            this.cboClient.Size = new System.Drawing.Size(378, 21);
            this.cboClient.TabIndex = 3;
            //
            // propertyGrid
            //
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.HelpVisible = true;
            this.propertyGrid.Location = new System.Drawing.Point(0, 70);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid.Size = new System.Drawing.Size(480, 371);
            this.propertyGrid.TabIndex = 1;
            this.propertyGrid.ToolbarVisible = false;
            //
            // pnlButtons
            //
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnCreate);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 441);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(480, 50);
            this.pnlButtons.TabIndex = 2;
            //
            // btnCancel
            //
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.Location = new System.Drawing.Point(356, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Annulla";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnCreate
            //
            this.btnCreate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnCreate.Location = new System.Drawing.Point(248, 11);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(100, 27);
            this.btnCreate.TabIndex = 0;
            this.btnCreate.Text = "Crea";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            //
            // NewUserCustomizationForm
            //
            this.AcceptButton = this.btnCreate;
            this.CancelButton = this.btnCancel;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 491);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlTop);
            this.MinimumSize = new System.Drawing.Size(420, 400);
            this.Name = "NewUserCustomizationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nuova Configurazione";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.ComboBox cboClient;
        private System.Windows.Forms.Label lblClient;
        private System.Windows.Forms.ComboBox cboProduct;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnCreate;
    }
}

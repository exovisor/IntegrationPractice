namespace CurrencyViewer.Forms
{
    partial class CurrenciesForm
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
            this.viewPanel = new System.Windows.Forms.Panel();
            this.currenciesDataGridView = new System.Windows.Forms.DataGridView();
            this.viewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.currenciesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // viewPanel
            // 
            this.viewPanel.Controls.Add(this.currenciesDataGridView);
            this.viewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewPanel.Location = new System.Drawing.Point(0, 0);
            this.viewPanel.Name = "viewPanel";
            this.viewPanel.Size = new System.Drawing.Size(800, 450);
            this.viewPanel.TabIndex = 0;
            // 
            // currenciesDataGridView
            // 
            this.currenciesDataGridView.AllowUserToAddRows = false;
            this.currenciesDataGridView.AllowUserToDeleteRows = false;
            this.currenciesDataGridView.AllowUserToOrderColumns = true;
            this.currenciesDataGridView.AllowUserToResizeRows = false;
            this.currenciesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.currenciesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currenciesDataGridView.Location = new System.Drawing.Point(0, 0);
            this.currenciesDataGridView.Name = "currenciesDataGridView";
            this.currenciesDataGridView.ReadOnly = true;
            this.currenciesDataGridView.Size = new System.Drawing.Size(800, 450);
            this.currenciesDataGridView.TabIndex = 0;
            // 
            // CurrenciesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.viewPanel);
            this.Name = "CurrenciesForm";
            this.Text = "CurrenciesForm";
            this.viewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.currenciesDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Panel viewPanel;
        private System.Windows.Forms.DataGridView currenciesDataGridView;
    }
}
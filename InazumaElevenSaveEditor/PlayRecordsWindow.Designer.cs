
namespace InazumaElevenSaveEditor
{
    partial class PlayRecordsWindow
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.unlockAllButton = new System.Windows.Forms.Button();
            this.resetAllButton = new System.Windows.Forms.Button();
            this.NameTextBox = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Unlocked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameTextBox,
            this.Unlocked});
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(360, 426);
            this.dataGridView1.TabIndex = 3;
            // 
            // unlockAllButton
            // 
            this.unlockAllButton.Enabled = false;
            this.unlockAllButton.Location = new System.Drawing.Point(182, 432);
            this.unlockAllButton.Name = "unlockAllButton";
            this.unlockAllButton.Size = new System.Drawing.Size(167, 25);
            this.unlockAllButton.TabIndex = 5;
            this.unlockAllButton.Text = "Unlock All";
            this.unlockAllButton.UseVisualStyleBackColor = true;
            this.unlockAllButton.Click += new System.EventHandler(this.UnlockAllButton_Click);
            // 
            // resetAllButton
            // 
            this.resetAllButton.Enabled = false;
            this.resetAllButton.Location = new System.Drawing.Point(9, 432);
            this.resetAllButton.Name = "resetAllButton";
            this.resetAllButton.Size = new System.Drawing.Size(167, 25);
            this.resetAllButton.TabIndex = 4;
            this.resetAllButton.Text = "Reset All";
            this.resetAllButton.UseVisualStyleBackColor = true;
            this.resetAllButton.Click += new System.EventHandler(this.ResetAllButton_Click);
            // 
            // NameTextBox
            // 
            this.NameTextBox.HeaderText = "Name";
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.ReadOnly = true;
            this.NameTextBox.Width = 200;
            // 
            // Unlocked
            // 
            this.Unlocked.HeaderText = "Unlocked";
            this.Unlocked.Name = "Unlocked";
            // 
            // PlayRecordsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 462);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.unlockAllButton);
            this.Controls.Add(this.resetAllButton);
            this.MaximumSize = new System.Drawing.Size(376, 501);
            this.Name = "PlayRecordsWindow";
            this.Text = "PlayRecordsWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PlayRecordsWindow_FormClosing);
            this.Load += new System.EventHandler(this.PlayRecordsWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dataGridView1;
        public System.Windows.Forms.Button unlockAllButton;
        public System.Windows.Forms.Button resetAllButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameTextBox;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Unlocked;
    }
}
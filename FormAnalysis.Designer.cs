namespace TextsBase
{
    partial class FormAnalysis
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAnalysis));
            this.dgvLettersAnalysis = new System.Windows.Forms.DataGridView();
            this.chkRelativeValues = new System.Windows.Forms.CheckBox();
            this.saveAbsoluteBTN = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.saveRelativeBTN = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLettersAnalysis)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLettersAnalysis
            // 
            this.dgvLettersAnalysis.AllowUserToAddRows = false;
            this.dgvLettersAnalysis.AllowUserToDeleteRows = false;
            this.dgvLettersAnalysis.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dgvLettersAnalysis.ColumnHeadersHeight = 29;
            this.dgvLettersAnalysis.Location = new System.Drawing.Point(13, 15);
            this.dgvLettersAnalysis.Margin = new System.Windows.Forms.Padding(4);
            this.dgvLettersAnalysis.Name = "dgvLettersAnalysis";
            this.dgvLettersAnalysis.RowHeadersVisible = false;
            this.dgvLettersAnalysis.RowHeadersWidth = 51;
            this.dgvLettersAnalysis.RowTemplate.ReadOnly = true;
            this.dgvLettersAnalysis.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLettersAnalysis.Size = new System.Drawing.Size(1282, 577);
            this.dgvLettersAnalysis.TabIndex = 0;
            this.dgvLettersAnalysis.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgvLettersAnalysis_ColumnAdded);
            // 
            // chkRelativeValues
            // 
            this.chkRelativeValues.AutoSize = true;
            this.chkRelativeValues.Location = new System.Drawing.Point(16, 604);
            this.chkRelativeValues.Margin = new System.Windows.Forms.Padding(4);
            this.chkRelativeValues.Name = "chkRelativeValues";
            this.chkRelativeValues.Size = new System.Drawing.Size(150, 20);
            this.chkRelativeValues.TabIndex = 1;
            this.chkRelativeValues.Text = "Відносні величини";
            this.chkRelativeValues.UseVisualStyleBackColor = true;
            this.chkRelativeValues.CheckedChanged += new System.EventHandler(this.chkRelativeValues_CheckedChanged);
            // 
            // saveAbsoluteBTN
            // 
            this.saveAbsoluteBTN.Location = new System.Drawing.Point(1131, 599);
            this.saveAbsoluteBTN.Margin = new System.Windows.Forms.Padding(4);
            this.saveAbsoluteBTN.Name = "saveAbsoluteBTN";
            this.saveAbsoluteBTN.Size = new System.Drawing.Size(164, 55);
            this.saveAbsoluteBTN.TabIndex = 2;
            this.saveAbsoluteBTN.Text = "Зберегти абсолютні величини";
            this.saveAbsoluteBTN.UseVisualStyleBackColor = true;
            this.saveAbsoluteBTN.Click += new System.EventHandler(this.saveAbsoluteBTN_Click);
            // 
            // saveRelativeBTN
            // 
            this.saveRelativeBTN.Location = new System.Drawing.Point(959, 600);
            this.saveRelativeBTN.Margin = new System.Windows.Forms.Padding(4);
            this.saveRelativeBTN.Name = "saveRelativeBTN";
            this.saveRelativeBTN.Size = new System.Drawing.Size(164, 55);
            this.saveRelativeBTN.TabIndex = 3;
            this.saveRelativeBTN.Text = "Зберегти відносні величини";
            this.saveRelativeBTN.UseVisualStyleBackColor = true;
            this.saveRelativeBTN.Click += new System.EventHandler(this.saveRelativeBTN_Click);
            // 
            // FormAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1308, 670);
            this.Controls.Add(this.saveRelativeBTN);
            this.Controls.Add(this.saveAbsoluteBTN);
            this.Controls.Add(this.chkRelativeValues);
            this.Controls.Add(this.dgvLettersAnalysis);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormAnalysis";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormAnalysis";
            this.Load += new System.EventHandler(this.FormAnalysis_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLettersAnalysis)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLettersAnalysis;
        private System.Windows.Forms.CheckBox chkRelativeValues;
        private System.Windows.Forms.Button saveAbsoluteBTN;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button saveRelativeBTN;
    }
}
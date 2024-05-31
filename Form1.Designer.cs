namespace TextsBase
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btn_ChoseFolderTexts = new System.Windows.Forms.Button();
            this.ss = new System.Windows.Forms.StatusStrip();
            this.tssLabelCountFiles = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.dgv_Texts = new System.Windows.Forms.DataGridView();
            this.cPP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cNameText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cCharsCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.Chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBoxAnalyse = new System.Windows.Forms.GroupBox();
            this.rbShowSymbols = new System.Windows.Forms.RadioButton();
            this.rbShowBukvu = new System.Windows.Forms.RadioButton();
            this.btn_show_all = new System.Windows.Forms.Button();
            this.dgv_CharStat = new System.Windows.Forms.DataGridView();
            this.cLetter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cFreq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnExit = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbSpaceDisable = new System.Windows.Forms.ComboBox();
            this.cbTextEncoding = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBoxNGramm = new System.Windows.Forms.GroupBox();
            this.btnNgramm = new System.Windows.Forms.Button();
            this.nud_n = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cbLanguage = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.IgnoreCase = new System.Windows.Forms.ComboBox();
            this.ss.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Texts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Chart)).BeginInit();
            this.groupBoxAnalyse.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_CharStat)).BeginInit();
            this.groupBoxNGramm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_n)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_ChoseFolderTexts
            // 
            this.btn_ChoseFolderTexts.BackColor = System.Drawing.Color.Transparent;
            this.btn_ChoseFolderTexts.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_ChoseFolderTexts.Location = new System.Drawing.Point(12, 12);
            this.btn_ChoseFolderTexts.Name = "btn_ChoseFolderTexts";
            this.btn_ChoseFolderTexts.Size = new System.Drawing.Size(126, 88);
            this.btn_ChoseFolderTexts.TabIndex = 0;
            this.btn_ChoseFolderTexts.Text = "Вибір папки текстів";
            this.btn_ChoseFolderTexts.UseVisualStyleBackColor = false;
            this.btn_ChoseFolderTexts.Click += new System.EventHandler(this.Btn_ChoseFolderTexts_Click);
            // 
            // ss
            // 
            this.ss.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssLabelCountFiles,
            this.tssProgressBar});
            this.ss.Location = new System.Drawing.Point(0, 582);
            this.ss.Name = "ss";
            this.ss.Size = new System.Drawing.Size(1254, 22);
            this.ss.TabIndex = 2;
            this.ss.Text = "statusStrip1";
            // 
            // tssLabelCountFiles
            // 
            this.tssLabelCountFiles.Name = "tssLabelCountFiles";
            this.tssLabelCountFiles.Size = new System.Drawing.Size(161, 17);
            this.tssLabelCountFiles.Text = "Загальна кількість файлів: 0";
            // 
            // tssProgressBar
            // 
            this.tssProgressBar.Name = "tssProgressBar";
            this.tssProgressBar.Size = new System.Drawing.Size(1000, 16);
            this.tssProgressBar.Step = 1;
            // 
            // dgv_Texts
            // 
            this.dgv_Texts.AllowUserToAddRows = false;
            this.dgv_Texts.AllowUserToDeleteRows = false;
            this.dgv_Texts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_Texts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgv_Texts.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.dgv_Texts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Texts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cPP,
            this.cNameText,
            this.cCharsCount});
            this.dgv_Texts.Location = new System.Drawing.Point(3, 3);
            this.dgv_Texts.MultiSelect = false;
            this.dgv_Texts.Name = "dgv_Texts";
            this.dgv_Texts.ReadOnly = true;
            this.dgv_Texts.RowHeadersVisible = false;
            this.dgv_Texts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_Texts.Size = new System.Drawing.Size(345, 467);
            this.dgv_Texts.TabIndex = 3;
            // 
            // cPP
            // 
            this.cPP.HeaderText = "№ п/п";
            this.cPP.Name = "cPP";
            this.cPP.ReadOnly = true;
            this.cPP.Width = 59;
            // 
            // cNameText
            // 
            this.cNameText.HeaderText = "Назва текстового файлу";
            this.cNameText.Name = "cNameText";
            this.cNameText.ReadOnly = true;
            this.cNameText.Width = 144;
            // 
            // cCharsCount
            // 
            this.cCharsCount.HeaderText = "Кількість симовлів";
            this.cCharsCount.Name = "cCharsCount";
            this.cCharsCount.ReadOnly = true;
            this.cCharsCount.Width = 116;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 106);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgv_Texts);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.Chart);
            this.splitContainer1.Size = new System.Drawing.Size(1051, 473);
            this.splitContainer1.SplitterDistance = 351;
            this.splitContainer1.TabIndex = 4;
            // 
            // Chart
            // 
            chartArea1.Name = "ChartArea1";
            this.Chart.ChartAreas.Add(chartArea1);
            this.Chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Chart.Location = new System.Drawing.Point(0, 0);
            this.Chart.Name = "Chart";
            series1.ChartArea = "ChartArea1";
            series1.Name = "Series1";
            this.Chart.Series.Add(series1);
            this.Chart.Size = new System.Drawing.Size(696, 473);
            this.Chart.TabIndex = 0;
            // 
            // groupBoxAnalyse
            // 
            this.groupBoxAnalyse.Controls.Add(this.rbShowSymbols);
            this.groupBoxAnalyse.Controls.Add(this.rbShowBukvu);
            this.groupBoxAnalyse.Controls.Add(this.btn_show_all);
            this.groupBoxAnalyse.Location = new System.Drawing.Point(357, 12);
            this.groupBoxAnalyse.Name = "groupBoxAnalyse";
            this.groupBoxAnalyse.Size = new System.Drawing.Size(231, 72);
            this.groupBoxAnalyse.TabIndex = 5;
            this.groupBoxAnalyse.TabStop = false;
            this.groupBoxAnalyse.Text = "Кількість";
            // 
            // rbShowSymbols
            // 
            this.rbShowSymbols.AutoSize = true;
            this.rbShowSymbols.Location = new System.Drawing.Point(6, 43);
            this.rbShowSymbols.Name = "rbShowSymbols";
            this.rbShowSymbols.Size = new System.Drawing.Size(71, 17);
            this.rbShowSymbols.TabIndex = 2;
            this.rbShowSymbols.Text = "символів";
            this.rbShowSymbols.UseVisualStyleBackColor = true;
            this.rbShowSymbols.CheckedChanged += new System.EventHandler(this.RbShowSymbols_CheckedChanged);
            // 
            // rbShowBukvu
            // 
            this.rbShowBukvu.AutoSize = true;
            this.rbShowBukvu.Checked = true;
            this.rbShowBukvu.Location = new System.Drawing.Point(6, 20);
            this.rbShowBukvu.Name = "rbShowBukvu";
            this.rbShowBukvu.Size = new System.Drawing.Size(50, 17);
            this.rbShowBukvu.TabIndex = 1;
            this.rbShowBukvu.TabStop = true;
            this.rbShowBukvu.Text = "літер";
            this.rbShowBukvu.UseVisualStyleBackColor = true;
            this.rbShowBukvu.CheckedChanged += new System.EventHandler(this.RbShowBukvu_CheckedChanged);
            // 
            // btn_show_all
            // 
            this.btn_show_all.BackColor = System.Drawing.Color.Transparent;
            this.btn_show_all.Location = new System.Drawing.Point(90, 19);
            this.btn_show_all.Name = "btn_show_all";
            this.btn_show_all.Size = new System.Drawing.Size(135, 41);
            this.btn_show_all.TabIndex = 0;
            this.btn_show_all.Text = "Показати загальну кількість";
            this.btn_show_all.UseVisualStyleBackColor = false;
            this.btn_show_all.Click += new System.EventHandler(this.Btn_show_all_Click);
            // 
            // dgv_CharStat
            // 
            this.dgv_CharStat.AllowUserToAddRows = false;
            this.dgv_CharStat.AllowUserToDeleteRows = false;
            this.dgv_CharStat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_CharStat.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgv_CharStat.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgv_CharStat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_CharStat.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cLetter,
            this.cFreq});
            this.dgv_CharStat.Location = new System.Drawing.Point(1069, 106);
            this.dgv_CharStat.MultiSelect = false;
            this.dgv_CharStat.Name = "dgv_CharStat";
            this.dgv_CharStat.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgv_CharStat.RowHeadersVisible = false;
            this.dgv_CharStat.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_CharStat.Size = new System.Drawing.Size(173, 473);
            this.dgv_CharStat.TabIndex = 6;
            // 
            // cLetter
            // 
            this.cLetter.HeaderText = "Символ";
            this.cLetter.Name = "cLetter";
            this.cLetter.ReadOnly = true;
            this.cLetter.Width = 71;
            // 
            // cFreq
            // 
            this.cFreq.HeaderText = "Кількість";
            this.cFreq.Name = "cFreq";
            this.cFreq.ReadOnly = true;
            this.cFreq.Width = 78;
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExit.Location = new System.Drawing.Point(1069, 12);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(173, 88);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Вихід з програми";
            this.btnExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(145, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Враховувати пробіл:";
            // 
            // cbSpaceDisable
            // 
            this.cbSpaceDisable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSpaceDisable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbSpaceDisable.FormattingEnabled = true;
            this.cbSpaceDisable.Items.AddRange(new object[] {
            "Ні",
            "Так"});
            this.cbSpaceDisable.Location = new System.Drawing.Point(259, 36);
            this.cbSpaceDisable.Name = "cbSpaceDisable";
            this.cbSpaceDisable.Size = new System.Drawing.Size(92, 21);
            this.cbSpaceDisable.TabIndex = 13;
            this.cbSpaceDisable.SelectedIndexChanged += new System.EventHandler(this.CbSpaceDisable_SelectedIndexChanged);
            // 
            // cbTextEncoding
            // 
            this.cbTextEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTextEncoding.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbTextEncoding.FormattingEnabled = true;
            this.cbTextEncoding.Items.AddRange(new object[] {
            "UTF-8",
            "Win-1251"});
            this.cbTextEncoding.Location = new System.Drawing.Point(227, 12);
            this.cbTextEncoding.Name = "cbTextEncoding";
            this.cbTextEncoding.Size = new System.Drawing.Size(124, 21);
            this.cbTextEncoding.TabIndex = 15;
            this.cbTextEncoding.SelectedIndexChanged += new System.EventHandler(this.CbTextEncoding_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(145, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Кодування:";
            // 
            // groupBoxNGramm
            // 
            this.groupBoxNGramm.Controls.Add(this.btnNgramm);
            this.groupBoxNGramm.Controls.Add(this.nud_n);
            this.groupBoxNGramm.Controls.Add(this.label4);
            this.groupBoxNGramm.Location = new System.Drawing.Point(840, 12);
            this.groupBoxNGramm.Name = "groupBoxNGramm";
            this.groupBoxNGramm.Size = new System.Drawing.Size(223, 72);
            this.groupBoxNGramm.TabIndex = 16;
            this.groupBoxNGramm.TabStop = false;
            this.groupBoxNGramm.Text = "N-Gram";
            // 
            // btnNgramm
            // 
            this.btnNgramm.BackColor = System.Drawing.Color.Transparent;
            this.btnNgramm.ForeColor = System.Drawing.Color.Black;
            this.btnNgramm.Location = new System.Drawing.Point(126, 19);
            this.btnNgramm.Name = "btnNgramm";
            this.btnNgramm.Size = new System.Drawing.Size(80, 41);
            this.btnNgramm.TabIndex = 2;
            this.btnNgramm.Text = "Розрахувати";
            this.btnNgramm.UseVisualStyleBackColor = false;
            this.btnNgramm.Click += new System.EventHandler(this.BtnNgramm_Click);
            // 
            // nud_n
            // 
            this.nud_n.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nud_n.Location = new System.Drawing.Point(80, 29);
            this.nud_n.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nud_n.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_n.Name = "nud_n";
            this.nud_n.ReadOnly = true;
            this.nud_n.Size = new System.Drawing.Size(40, 22);
            this.nud_n.TabIndex = 1;
            this.nud_n.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(8, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Порядок:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.cbLanguage);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(594, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 72);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Вибір мови";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(9, 43);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(225, 20);
            this.textBox1.TabIndex = 2;
            // 
            // cbLanguage
            // 
            this.cbLanguage.FormattingEnabled = true;
            this.cbLanguage.Items.AddRange(new object[] {
            "Українська ",
            "Англійська ",
            "Всі, що зустрічаються ",
            "Символи",
            "Цифри ",
            "Додати"});
            this.cbLanguage.Location = new System.Drawing.Point(74, 16);
            this.cbLanguage.Name = "cbLanguage";
            this.cbLanguage.Size = new System.Drawing.Size(117, 21);
            this.cbLanguage.TabIndex = 1;
            this.cbLanguage.SelectedIndexChanged += new System.EventHandler(this.cbLanguage_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Мова";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(145, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Ігнорувати регістр:";
            // 
            // IgnoreCase
            // 
            this.IgnoreCase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IgnoreCase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.IgnoreCase.FormattingEnabled = true;
            this.IgnoreCase.Items.AddRange(new object[] {
            "Так",
            "Ні"});
            this.IgnoreCase.Location = new System.Drawing.Point(259, 63);
            this.IgnoreCase.Name = "IgnoreCase";
            this.IgnoreCase.Size = new System.Drawing.Size(92, 21);
            this.IgnoreCase.TabIndex = 19;
            this.IgnoreCase.SelectedIndexChanged += new System.EventHandler(this.IgnoreCase_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1254, 604);
            this.Controls.Add(this.IgnoreCase);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxNGramm);
            this.Controls.Add(this.cbTextEncoding);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbSpaceDisable);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.dgv_CharStat);
            this.Controls.Add(this.groupBoxAnalyse);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.ss);
            this.Controls.Add(this.btn_ChoseFolderTexts);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Text analysis";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ss.ResumeLayout(false);
            this.ss.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Texts)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Chart)).EndInit();
            this.groupBoxAnalyse.ResumeLayout(false);
            this.groupBoxAnalyse.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_CharStat)).EndInit();
            this.groupBoxNGramm.ResumeLayout(false);
            this.groupBoxNGramm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_n)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_ChoseFolderTexts;
        private System.Windows.Forms.StatusStrip ss;
        private System.Windows.Forms.ToolStripProgressBar tssProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel tssLabelCountFiles;
        private System.Windows.Forms.DataGridView dgv_Texts;
        private System.Windows.Forms.DataGridViewTextBoxColumn cPP;
        private System.Windows.Forms.DataGridViewTextBoxColumn cNameText;
        private System.Windows.Forms.DataGridViewTextBoxColumn cCharsCount;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataVisualization.Charting.Chart Chart;
        private System.Windows.Forms.GroupBox groupBoxAnalyse;
        private System.Windows.Forms.Button btn_show_all;
        private System.Windows.Forms.DataGridView dgv_CharStat;
        private System.Windows.Forms.RadioButton rbShowSymbols;
        private System.Windows.Forms.RadioButton rbShowBukvu;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLetter;
        private System.Windows.Forms.DataGridViewTextBoxColumn cFreq;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbSpaceDisable;
        private System.Windows.Forms.ComboBox cbTextEncoding;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBoxNGramm;
        private System.Windows.Forms.NumericUpDown nud_n;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnNgramm;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox IgnoreCase;
        private System.Windows.Forms.ComboBox cbLanguage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
    }
}


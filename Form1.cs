using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TextsBase.Utill;

namespace TextsBase
{
    public partial class Form1 : Form
    {
        private string[] _files;
        private bool _ignoreCase;
        private bool _ignoreSpaces;
        private string[] _manualInput;
        NGramStatistic _nGramStatistic;

        public Form1()
        {
            InitializeComponent();
            cbSpaceDisable.SelectedIndex = 0;
            cbTextEncoding.SelectedIndex = 0;
            IgnoreCase.SelectedIndex = 0;
            cbLanguage.SelectedIndex = 0;
        }

        private async void Btn_ChoseFolderTexts_Click(object sender, EventArgs e)
        {
            try
            {
                tssLabelCountFiles.Text = "Загальна кількість файлів: 0";
                dgv_Texts.Rows.Clear();

                FolderBrowserDialog fbd = new FolderBrowserDialog
                {
                    Description = "Виберіть папку з текстами для дослідження"
                };

                if (!string.IsNullOrEmpty(Properties.Settings.Default.LastSelectedFolder) &&
                    Directory.Exists(Properties.Settings.Default.LastSelectedFolder))
                {
                    fbd.SelectedPath = Properties.Settings.Default.LastSelectedFolder;
                }

                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    Properties.Settings.Default.LastSelectedFolder = fbd.SelectedPath;
                    Properties.Settings.Default.Save();

                    _files = Directory.GetFiles(fbd.SelectedPath, "*.txt", SearchOption.AllDirectories);

                    FileInfo fileInfo;
                    foreach (string file in _files)
                    {
                        fileInfo = new FileInfo(file);
                        dgv_Texts.Rows.Add(dgv_Texts.Rows.Count + 1, fileInfo.Name, $"{fileInfo.Length} байт");
                    }

                    tssLabelCountFiles.Text = $"Загальна кількість файлів: {_files.Length}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private List<string> GetFilesInSubDirectories(string dir)
        {
            List<string> result = new List<string>();
            string[] dirs = Directory.GetDirectories(dir);
            foreach (var innerDir in dirs)
            {
                var innerDirs = Directory.GetDirectories(innerDir);
                if (innerDirs.Length > 0)
                {
                    result.AddRange(GetFilesInSubDirectories(innerDir));
                }
                else
                {
                    result.AddRange(Directory.GetFiles(innerDir, "*.txt"));
                }
            }
            return result;
        }

        private void Btn_show_all_Click(object sender, EventArgs e)
        {
            try
            {
                //Перевіряємо чи є тексти 
                if (_nGramStatistic == null) return;

                //Виводимо результати підрахунку
                VisualizeStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        private void VisualizeStats()
        {
            //Очищаємо графік
            Chart.Series[0].Points.Clear();
            Chart.ChartAreas[0].AxisX.CustomLabels.Clear();
            //Очищаємо таблицю з статистикою символів
            dgv_CharStat.Rows.Clear();

            // Get the selected value from the ComboBox
            int selectedValue = cbLanguage.SelectedIndex;
            Dictionary<string, int> filteredStats = new Dictionary<string, int>();

            // Filter the statistics based on the selected value
            switch (selectedValue)
            {
                case 0:
                    filteredStats = _nGramStatistic.totalAbsoluteStatistic.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    break;
                case 1:
                    filteredStats = _nGramStatistic.totalAbsoluteStatistic
                        .Where(kvp => IsUkrainianWord(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    break;
                case 2:
                    filteredStats = _nGramStatistic.totalAbsoluteStatistic
                        .Where(kvp => IsEnglishWord(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    break;
                case 3:
                    filteredStats = _nGramStatistic.totalAbsoluteStatistic
                        .Where(kvp => IsSymbol(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    break;
                case 4:
                    filteredStats = _nGramStatistic.totalAbsoluteStatistic
                        .Where(kvp => IsDigit(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    break;
                case 5:
                    filteredStats = _nGramStatistic.totalAbsoluteStatistic
                        .Where(kvp => _manualInput.Contains(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    break;
            }

            int i = 0;
            //Рахуємо загальну кількість символів
            int totalCharsCount = filteredStats.Sum(k => k.Value);

            //Рахуємо частоту зустрічі в тексті символів
            foreach (var ch in filteredStats.OrderByDescending(v => v.Value))
            {
                dgv_CharStat.Rows.Add();
                dgv_CharStat.Rows[dgv_CharStat.Rows.Count - 1].Cells["cLetter"].Value = ch.Key;
                dgv_CharStat.Rows[dgv_CharStat.Rows.Count - 1].Cells["cFreq"].Value = ch.Value;

                //Виводимо дані на графік
                Chart.Series[0].Points.AddXY(i++, ch.Value / (double)totalCharsCount);
                Chart.ChartAreas[0].AxisX.CustomLabels.Add((i - 1) - 0.5, i - 0.5, ch.Key);
            }

            //Сортуємо таблицю по алфавіту
            dgv_CharStat.Sort(dgv_CharStat.Columns["cLetter"], ListSortDirection.Ascending);
        }

        private bool IsUkrainianWord(string s)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(s, @"^[А-Яа-яҐґЄєІіЇїЁё]+$");
        }

        private bool IsEnglishWord(string s)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(s, @"^[A-Za-z]+$");
        }

        private bool IsSymbol(string s)
        {
            return s.Length == 1 && (char.IsSymbol(s[0]) || char.IsPunctuation(s[0]));
        }

        private bool IsDigit(string s)
        {
            return s.Length == 1 && char.IsDigit(s[0]);
        }


        private void RbShowSymbols_CheckedChanged(object sender, EventArgs e) {}

        private void RbShowBukvu_CheckedChanged(object sender, EventArgs e){}


        private void Form1_Load(object sender, EventArgs e)
        {
            InitDemoDataChart();
        }

        private void InitDemoDataChart()
        {
            try
            {
                for (double i = 1; i < 10; i++)
                {

                    Chart.Series[0].Points.AddXY(i,i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void CbSpaceDisable_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbSpaceDisable.SelectedIndex == 1)
                {
                    TextAnalyzer.ignoreSpaces = false;
                    _ignoreSpaces = false;
                }
                else
                {
                    TextAnalyzer.ignoreSpaces = true;
                    _ignoreSpaces = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static Encoding TextEncoding = Encoding.GetEncoding("windows-1251");
        private void CbTextEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbTextEncoding.SelectedIndex == 1)
                {
                    TextEncoding = Encoding.GetEncoding("windows-1251");
                }
                else
                {
                    TextEncoding = Encoding.GetEncoding("UTF-8");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnNgramm_Click(object sender, EventArgs e)
        {
            new FormAnalysis(_nGramStatistic).ShowDialog();
        }

        

        public void IgnoreCase_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (IgnoreCase.SelectedIndex == 0)
                {
                    TextAnalyzer.ignoreCase = true;
                    _ignoreCase = true;
                }
                else
                {
                    TextAnalyzer.ignoreCase = false;
                    _ignoreCase = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbLanguage.SelectedIndex == 5)
            {
                textBox1.Visible = true;
            }
            else
            {
                textBox1.Visible = false;
            }
        }

        private async void calculateButton_Click(object sender, EventArgs e)
        {
            calculateButton.Enabled = false;
            tssProgressBar.Style = ProgressBarStyle.Marquee;

            _nGramStatistic = new NGramStatistic(_files, (int)nud_n.Value, _ignoreCase, _ignoreSpaces);

            _nGramStatistic.ProcessFile();
            _nGramStatistic.CalculateRelativeStatistic();

            tssProgressBar.Style = ProgressBarStyle.Blocks;
            calculateButton.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _manualInput = textBox1.Text.Trim().Split(',');
        }
    }
}

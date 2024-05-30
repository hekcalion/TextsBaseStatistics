using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static TextsBase.NGrammUtil;

namespace TextsBase
{
    public partial class FormAnalysis : Form
    {
        private Dictionary<DataGridViewColumn, DataGridViewColumn> _columnMapping;
        public FormAnalysis(List<Text> textInfos, string CalculateType)
        {
            InitializeComponent();
            TI = textInfos;
            CT = CalculateType;
        }
        public FormAnalysis(List<Text> textInfos, string CalculateType, byte nGramLevel)
        {
            InitializeComponent();
            TI = textInfos;
            CT = CalculateType;
            ngramLevel = nGramLevel;
            _columnMapping = new Dictionary<DataGridViewColumn, DataGridViewColumn>();
            chkRelativeValues.CheckedChanged += chkRelativeValues_CheckedChanged;
        }

        Languages ngramLang;
        byte ngramLevel = 1;

        List<Text> TI;
        string CT;
        char[] pattern = null;
        string[] patternNgram = null;
        bool ngram = false;

        private void FormAnalysis_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = CT;
                
                pattern = TextAnalyzer.GetLettersOrDigits();
                UpdateDataGridView();

                if (CT == "nGram")
                {
                    switch (Form1.LanguageType)
                    {
                        //Українська
                        case 0:
                            ngramLang = Languages.UA;
                            break;
                        //Російська
                        case 1:
                            ngramLang = Languages.EN;
                            break;
                        //Англійська
                        case 2:
                            ngramLang = Languages.EN;
                            break;
                        default:
                            break;
                    }
                }
                switch (CT)
                {
                    case "Аналіз літер":
                        AnalysisLetter();
                        break;
                    case "Аналіз символів":
                        AnalysisSymbol();
                        break;
                    case "Літери (абсолютні значення)":
                        AbsoluteLetter();
                        break;
                    case "Символи (абсолютні значення)":
                        AbsoluteSymbol();
                        break;
                    case "nGram":
                        CalculateNGramm(ngramLevel, ngramLang);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private const string doubleFormat = "{0:0.000000000000}";
        bool saveToFile = false;
        public NGrammUtil NGrammUtil { get; private set; }
        private StatsUtil _statsUtil { get; set; }
        private async void CalculateNGramm(byte nGrammLevel, Languages lang)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            #region Initialization

            _rowIndices = new Dictionary<string, int>();

            NGrammUtil nGrammUtil = new NGrammUtil(nGrammLevel, lang, true, false, false);
            _statsUtil = new StatsUtil(nGrammUtil);

            #endregion

            #region Phase1 Preprocessing

            Parallel.ForEach(TI, file =>
            {
                var text = new List<char>();
                using (var sr = new StreamReader(file.Path, Form1.TextEncoding, true))
                {
                    var buffer = new char[4096];
                    int readCount;
                    while ((readCount = sr.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        text.AddRange(buffer.Take(readCount));
                    }
                }

                var textProcessor = new TextProcessor(nGrammUtil, file.Path, text.Select(c => Char.ToLowerInvariant(c)).ToArray());

                int totalNGrammsCount;
                var analysisResults = textProcessor.Analyze(out totalNGrammsCount);
                if (totalNGrammsCount == 0)
                {
                    return;
                }

                lock (_statsUtil)
                {
                    _statsUtil.AddToResults(textProcessor.TextInfo.TextFull, analysisResults, totalNGrammsCount);
                }

                Application.DoEvents();
            });

            _statsUtil.CalculateMXs();

            #endregion

            #region Phase2 Processing

            var textIndex = 0;
            Parallel.ForEach(TI, file =>
            {
                var text = new List<char>();
                using (var sr = new StreamReader(file.Path, Form1.TextEncoding, true))
                {
                    var buffer = new char[4096];
                    int readCount;
                    while ((readCount = sr.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        text.AddRange(buffer.Take(readCount));
                    }
                }

                var textProcessor = new TextProcessor(nGrammUtil, file.Path, text.Select(c => Char.ToLowerInvariant(c)).ToArray());

                int totalNGrammsCount;
                var analysisResults = textProcessor.Analyze(out totalNGrammsCount);
                if (totalNGrammsCount == 0)
                {
                    return;
                }

                lock (_statsUtil)
                {
                    _statsUtil.AddToDx(analysisResults, totalNGrammsCount);
                }

                DisplayData(textProcessor, analysisResults, totalNGrammsCount, _statsUtil.L[textIndex], _statsUtil.W[textIndex]);

                textIndex++;
                Application.DoEvents();
            });

            _statsUtil.CalculateSigmas();

            #endregion

            #region Writing MX and Sigma

            // Ensure rows for MX, Sigma, and Total
            var mxRowIndex = EnsureRowExists("MX");
            var sigmaRowIndex = EnsureRowExists("Sigma");
            var totalRowIndex = EnsureRowExists("Total");

            foreach (var kvp in _statsUtil.Statistics)
            {
                var colIndexAbsolute = EnsureColumnExists(kvp.Key, out DataGridViewColumn absColumn, isRelative: false);
                var colIndexRelative = EnsureColumnExists(kvp.Key, out DataGridViewColumn relColumn, isRelative: true);

                // Maintain column mapping
                _columnMapping[absColumn] = relColumn;
                _columnMapping[relColumn] = absColumn;

                dgvLettersAnalysis.Rows[mxRowIndex].Cells[colIndexAbsolute].Value = string.Format(doubleFormat, kvp.Value.MX);
                dgvLettersAnalysis.Rows[mxRowIndex].Cells[colIndexRelative].Value = string.Format(doubleFormat, kvp.Value.MX);

                dgvLettersAnalysis.Rows[sigmaRowIndex].Cells[colIndexAbsolute].Value = string.Format(doubleFormat, kvp.Value.Sigma);
                dgvLettersAnalysis.Rows[sigmaRowIndex].Cells[colIndexRelative].Value = string.Format(doubleFormat, kvp.Value.Sigma);
            }

            #endregion

            stopwatch.Stop();
            MessageBox.Show(stopwatch.Elapsed.ToString());

            Enabled = true;

        }
        private string GetRelativeValueIfNeeded(double originalValue, int totalCount)
        {
            return string.Format(doubleFormat, originalValue / (double)totalCount);
        }
        
        DataTable dt = new DataTable();
        private void DisplayData(TextProcessor textProcessor, Dictionary<string, int> stats, int totalNGrammsCount, int l, double w)
        {
            Invoke(new Action(() =>
            {
            // Ensure columns for FileName, L, and W exist
            var fileNameColIndex = EnsureColumnExists("FileName", out _);
            var lColIndex = EnsureColumnExists("L", out _);
            var wColIndex = EnsureColumnExists("W", out _);

            // Add a new row for the file
            var rowIndex = dgvLettersAnalysis.Rows.Add();
            var row = dgvLettersAnalysis.Rows[rowIndex];

            // Set the file name in the first column
            row.Cells[fileNameColIndex].Value = textProcessor.TextInfo.TextFileName;

            // Add L value
            row.Cells[lColIndex].Value = l;

            // Add W value
            row.Cells[wColIndex].Value = string.Format(doubleFormat, w);

            // Add stats values
            foreach (var kvp in stats)
            {
                var colIndexAbsolute = EnsureColumnExists(kvp.Key, out DataGridViewColumn absColumn, isRelative: false);
                    var colIndexRelative = EnsureColumnExists(kvp.Key, out DataGridViewColumn relColumn, isRelative: true);

                    // Maintain column mapping
                    _columnMapping[absColumn] = relColumn;
                    _columnMapping[relColumn] = absColumn;

                    row.Cells[colIndexAbsolute].Value = kvp.Value; // Assuming kvp.Value is the absolute value
                    row.Cells[colIndexRelative].Value = GetRelativeValueIfNeeded(kvp.Value, totalNGrammsCount);
                }
            }));

        }
        private int EnsureColumnExists(string colName, out DataGridViewColumn column, bool isRelative = false)
        {
            string fullColName = isRelative ? colName + "_Rel" : colName + "_Abs";
            foreach (DataGridViewColumn col in dgvLettersAnalysis.Columns)
            {
                if (col.Name == fullColName)
                {
                    column = col;
                    return col.Index;
                }
            }

            int colIndex = dgvLettersAnalysis.Columns.Add(fullColName, colName);
            column = dgvLettersAnalysis.Columns[colIndex];
            column.Visible = !isRelative; // Initially show absolute columns
            return colIndex;
        }

        private int EnsureRowExists(string rowName)
        {
            foreach (DataGridViewRow row in dgvLettersAnalysis.Rows)
            {
                if (row.HeaderCell.Value?.ToString() == rowName)
                {
                    return row.Index;
                }
            }

            int rowIndex = dgvLettersAnalysis.Rows.Add();
            dgvLettersAnalysis.Rows[rowIndex].HeaderCell.Value = rowName;
            return rowIndex;
        }

        private void UpdateDataGridView()
        {

            bool showRelative = chkRelativeValues.Checked;
            foreach (var kvp in _columnMapping)
            {
                kvp.Key.Visible = showRelative;
                kvp.Value.Visible = !showRelative;
            }
        }
        private Dictionary<string, int> _rowIndices { get; set; }

        /// <summary>
        /// Сиволи абсолютні показники
        /// </summary>
        private void AbsoluteSymbol()
        {
            try
            {
                //Створюємо першу колонку "Назва тексту"
                dgvLettersAnalysis.Columns.Add("cFileName", "Назва файлу");

                //Створюємо колонки літер, та заповнюємо їх назви
                foreach (char ColumnName in TextAnalyzer.textSpecialSymbols.OrderBy(x => x))
                {
                    dgvLettersAnalysis.Columns.Add(String.Format("c{0}", ColumnName), ColumnName.ToString());
                }

                //Створюємо останню колонку "Кількість літер"
                dgvLettersAnalysis.Columns.Add("cCountLetter", "Кількість символів");

                dgvLettersAnalysis.RowCount = TI.Count + 4;

                //Загальна кількість символів
                int totalSum = 0;

                for (var i = 0; i < TI.Count; i++)
                {
                    dgvLettersAnalysis.Rows[i].Cells["cFileName"].Value = TI[i].TextFileName;
                    var sum = TextAnalyzer.CountLettersCount(TI[i].CharsStat, TextAnalyzer.GetSpecialSymbols());
                    totalSum += sum;
                    dgvLettersAnalysis.Rows[i].Cells["cCountLetter"].Value = sum;

                    foreach (char ch in TextAnalyzer.GetSpecialSymbols())
                    {
                        if (TI[i].CharsStat.ContainsKey(ch))
                        {
                            dgvLettersAnalysis.Rows[i].Cells[string.Format("c{0}", ch)].Value = string.Format("{0}", TI[i].CharsStat[ch]);
                        }
                    }
                }

                //Виводимо в таблицю сумму всих літер
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells["cCountLetter"].Value = totalSum;
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells["cCountLetter"].Style.BackColor = Color.LightCoral;

                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells[dgvLettersAnalysis.ColumnCount - 2].Value = "Сума:";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells[dgvLettersAnalysis.ColumnCount - 2].Style.BackColor = Color.LightCoral;


                Dictionary<char, StatsInfo> stats = TextAnalyzer.CalculateStatistics(TI, pattern);

                //MX (середнє значення) появи в текстах
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells["cFileName"].Value = "MX";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells["cFileName"].Style.BackColor = Color.LightBlue;


                foreach (char ch in TextAnalyzer.GetSpecialSymbols())
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000000}", stats[ch].MX);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightBlue;
                }


                //Sigma - межа верхньої та нижньої похибки
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Value = "Sigma";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Style.BackColor = Color.LightGreen;


                foreach (char ch in TextAnalyzer.GetSpecialSymbols())
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000000}", stats[ch].Sigma);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightGreen;
                }

                DGV_Export_to_CSV.Export(dgvLettersAnalysis, @"symbolsStatistics.csv");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Літери абсолютні показники
        /// </summary>
        private void AbsoluteLetter()
        {
            try
            {
                //Створюємо першу колонку "Назва тексту"
                dgvLettersAnalysis.Columns.Add("cFileName", "Назва файлу");

                //Створюємо колонки літер, та заповнюємо їх назви
                foreach (char ColumnName in pattern)
                {
                    dgvLettersAnalysis.Columns.Add(String.Format("c{0}", ColumnName), ColumnName.ToString());
                }

                //Створюємо останню колонку "Кількість літер"
                dgvLettersAnalysis.Columns.Add("cCountLetter", "Кількість літер");
                dgvLettersAnalysis.RowCount = TI.Count + 4;


                int totalSum = 0;
                for (var i = 0; i < TI.Count; i++)
                {
                    dgvLettersAnalysis.Rows[i].Cells[0].Value = TI[i].TextFileName;
                    var sum = TextAnalyzer.CountLettersCount(TI[i].CharsStat, pattern);
                    totalSum += sum;
                    dgvLettersAnalysis.Rows[i].Cells["cCountLetter"].Value = sum;

                    foreach (char ch in pattern)
                    {
                        if (TI[i].CharsStat.ContainsKey(ch))
                        {
                            dgvLettersAnalysis.Rows[i].Cells[string.Format("c{0}", ch)].Value = string.Format("{0}", TI[i].CharsStat[ch]);
                        }
                    }
                }

                //Виводимо в таблицю сумму всих літер
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells["cCountLetter"].Value = totalSum;
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells["cCountLetter"].Style.BackColor = Color.LightCoral;

                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells[dgvLettersAnalysis.ColumnCount - 2].Value = "Сума:";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells[dgvLettersAnalysis.ColumnCount - 2].Style.BackColor = Color.LightCoral;


                Dictionary<char, StatsInfo> stats = TextAnalyzer.CalculateStatistics(TI, pattern);

                //MX (середнє значення) появи в текстах
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells["cFileName"].Value = "MX";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells["cFileName"].Style.BackColor = Color.LightBlue;


                foreach (char ch in pattern)
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000000}", stats[ch].MX);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightBlue;
                }


                //Sigma - межа верхньої та нижньої похибки
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Value = "Sigma";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Style.BackColor = Color.LightGreen;


                foreach (char ch in pattern)
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000000}", stats[ch].Sigma);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightGreen;
                }

                DGV_Export_to_CSV.Export(dgvLettersAnalysis, @"textStatisticsAbsolute.csv");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Аналіз символів
        /// </summary>
        private void AnalysisSymbol()
        {
            try
            {
                //Створюємо першу колонку "Назва тексту"
                dgvLettersAnalysis.Columns.Add("cFileName", "Назва файлу");

                //Створюємо колонки літер, та заповнюємо їх назви
                foreach (char ColumnName in TextAnalyzer.GetSpecialSymbols())
                {
                    dgvLettersAnalysis.Columns.Add(String.Format("c{0}", ColumnName), ColumnName.ToString());
                }

                //Створюємо останню колонку "Кількість літер"
                dgvLettersAnalysis.Columns.Add("cCountLetter", "Кількість символів");

                dgvLettersAnalysis.RowCount = TI.Count + 4;

                //Загальна кількість символів
                var totalSum = 0;

                //Рахуемо частоту повторення символів, та кількість
                for (var i = 0; i < TI.Count; i++)
                {
                    //Назва файлу
                    dgvLettersAnalysis.Rows[i].Cells["cFileName"].Value = TI[i].TextFileName;

                    //Рахуемо кількісь символів
                    int sum = TextAnalyzer.CountLettersCount(TI[i].CharsStat, TextAnalyzer.GetSpecialSymbols());
                    totalSum += sum;

                    dgvLettersAnalysis.Rows[i].Cells["cCountLetter"].Value = sum;

                    foreach (char ch in TextAnalyzer.GetSpecialSymbols())
                    {
                        if (TI[i].CharsStat.ContainsKey(ch))
                        {
                            dgvLettersAnalysis.Rows[i].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000000}", TI[i].CharsStat[ch] / (double)sum);
                        }
                    }
                }

                //Виводимо в таблицю сумму всих літер
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells["cCountLetter"].Value = totalSum;
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells["cCountLetter"].Style.BackColor = Color.LightCoral;

                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells[dgvLettersAnalysis.ColumnCount - 2].Value = "Сума:";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells[dgvLettersAnalysis.ColumnCount - 2].Style.BackColor = Color.LightCoral;

                //Рахуємо статистику по символам
                Dictionary<char, StatsInfo> stats = TextAnalyzer.CalculateStatistics(TI, pattern);

                //MX (середнє значення) появи в текстах
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells["cFileName"].Value = "MX";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells["cFileName"].Style.BackColor = Color.LightBlue;


                foreach (char ch in TextAnalyzer.GetSpecialSymbols())
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000000}", stats[ch].MX);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightBlue;
                }

                //Sigma - межа верхньої та нижньої похибки
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Value = "Sigma";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Style.BackColor = Color.LightGreen;


                foreach (char ch in TextAnalyzer.GetSpecialSymbols())
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000000}", stats[ch].Sigma);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightGreen;
                }

                //Експорт в CSV
                DGV_Export_to_CSV.Export(dgvLettersAnalysis, @"symbolsStatistics.csv");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Аналіз літер
        /// </summary>
        private void AnalysisLetter()
        {
            try
            {

                //Створюємо першу колонку "Назва тексту"
                dgvLettersAnalysis.Columns.Add("cFileName", "Назва файлу");

                //Створюємо колонки літер, та заповнюємо їх назви
                foreach (char ColumnName in pattern)
                {
                    dgvLettersAnalysis.Columns.Add(String.Format("c{0}", ColumnName), ColumnName.ToString());
                }

                //Створюємо останню колонку "Кількість літер"
                dgvLettersAnalysis.Columns.Add("cCountLetter", "Кількість літер");



                dgvLettersAnalysis.RowCount = TI.Count + 4;
                dgvLettersAnalysis.ColumnCount = pattern.Length + 2;


                //Кількість всих літер
                int totalSum = 0;

                for (var i = 0; i < TI.Count; i++)
                {
                    //Записуємо в таблицю назву файлу
                    dgvLettersAnalysis.Rows[i].Cells["cFileName"].Value = TI[i].TextFileName;

                    //Рахуемо суму всих літер в файлі
                    int sum = TextAnalyzer.CountLettersCount(TI[i].CharsStat, pattern);

                    //Додаємо до загальної суми
                    totalSum += sum;

                    //Записуємо в таблицю суму по файлу
                    dgvLettersAnalysis.Rows[i].Cells["cCountLetter"].Value = sum;

                    //Вираховуємо та заносимо в таблицю частоту появи літери в кожному з текстів
                    foreach (char key in pattern)
                    {
                        if (TI[i].CharsStat.ContainsKey(key))
                        {
                            dgvLettersAnalysis.Rows[i].Cells[string.Format("c{0}", key)].Value = string.Format("{0:0.000000}", TI[i].CharsStat[key] / (double)sum);
                        }
                    }

                }

                //Виводимо в таблицю сумму всих літер
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells["cCountLetter"].Value = totalSum;
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells["cCountLetter"].Style.BackColor = Color.LightCoral;

                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells[dgvLettersAnalysis.ColumnCount - 2].Value = "Сума:";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 3].Cells[dgvLettersAnalysis.ColumnCount - 2].Style.BackColor = Color.LightCoral;

                //Рахуємо статистику
                Dictionary<char, StatsInfo> stats = TextAnalyzer.CalculateStatistics(TI, pattern);

                //MX (середнє значення) появи в текстах
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells["cFileName"].Value = "MX";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells["cFileName"].Style.BackColor = Color.LightBlue;


                foreach (char ch in pattern)
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.00000}", stats[ch].MX);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightBlue;
                }

                //Sigma - межа верхньої та нижньої похибки
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Value = "Sigma";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Style.BackColor = Color.LightGreen;



                foreach (char ch in pattern)
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.00000}", stats[ch].Sigma);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightGreen;
                }


                DGV_Export_to_CSV.Export(dgvLettersAnalysis, @"textStatisticLetters.csv");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void dgvLettersAnalysis_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            var cv1 = e.CellValue1 == null ? null : e.CellValue1.ToString();
            if (string.IsNullOrEmpty(cv1))
            {
                cv1 = "0";
            }

            var cv2 = e.CellValue2 == null ? null : e.CellValue2.ToString();
            if (string.IsNullOrEmpty(cv2))
            {
                cv2 = "0";
            }

            if (double.Parse(cv1) > double.Parse(cv2))
            {
                e.SortResult = 1;
            }
            else if (double.Parse(cv1) < double.Parse(cv2))
            {
                e.SortResult = -1;
            }
            else
            {
                e.SortResult = 0;
            }

            e.Handled = true;
        }

        private void chkRelativeValues_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDataGridView();
        }

        private void saveAbsoluteBTN_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            
            saveFileDialog1.RestoreDirectory = true;

            DGV_Export_to_CSV dDV_Export_To_CSV = new DGV_Export_to_CSV();

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                dDV_Export_To_CSV.Save(dgvLettersAnalysis, filePath, isRelative: false);
            }
        }

        private void saveRelativeBTN_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
     
            saveFileDialog1.RestoreDirectory = true;

            DGV_Export_to_CSV dDV_Export_To_CSV = new DGV_Export_to_CSV();

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                dDV_Export_To_CSV.Save(dgvLettersAnalysis, filePath, isRelative: true);
            }
        }
    }
    
}

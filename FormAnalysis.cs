using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static TextsBase.NGrammUtil;

namespace TextsBase
{
    public partial class FormAnalysis : Form
    {
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
                            ngramLang = Languages.RU;
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
        private void CalculateNGramm(byte nGrammLevel, Languages lang)
        {
            try
            {
                _rowIndices = new Dictionary<string, int>();
                var fileWriter = new FileWriter();
                if (saveToFile)
                {
                    fileWriter.AddRow("Header_CustomRow", string.Empty);
                    fileWriter.AddRow("L_CustomRow", 'L');
                    fileWriter.AddRow("W_CustomRow", 'W');
                }
                NGrammUtil = new NGrammUtil(nGrammLevel, lang, true, false, false);
                _statsUtil = new StatsUtil(NGrammUtil);

                for (var i = 0; i < TI.Count; i++)
                {
                    //Начало расчета n-грами
                    TextProcessor textProcessor = new TextProcessor(NGrammUtil, TI[i].Path, TI[i].TextFull.Select(c => Char.ToLowerInvariant(c)).ToArray());

                    int totalNGrammsCount;

                    Dictionary<string, int> analysisResults = textProcessor.Analyze(out totalNGrammsCount);
                    textProcessor.TextInfo.CharsStat = TI[i].CharsStat;
                    _statsUtil.AddToResults(textProcessor.TextInfo.TextFull, analysisResults, totalNGrammsCount);
                    Application.DoEvents();
                }
                _statsUtil.CalculateMXs();
                #region Phase2 Processing

                var textIndex = 0;
                foreach (var Text in TI)
                {
                    var text = new List<char>();
                    using (var sr = new StreamReader(Text.Path, Form1.TextEncoding, true))
                    {
                        var count = 1024;
                        var characterBuffer = new char[count];
                        while (!sr.EndOfStream)
                        {
                            var readCount = sr.Read(characterBuffer, 0, characterBuffer.Length);
                            for (var i = 0; i < readCount; i++)
                            {
                                text.Add(characterBuffer[i]);
                            }
                        }
                    }

                    var textProcessor = new TextProcessor(NGrammUtil, Text.Path, text.Select(c => Char.ToLowerInvariant(c)).ToArray());
                    textProcessor.TextInfo.CharsStat = Text.CharsStat;
                    int totalNGrammsCount;
                    var analysisResults = textProcessor.Analyze(out totalNGrammsCount);
                    if (totalNGrammsCount == 0)
                    {
                        continue;
                    }

                    _statsUtil.AddToDx(analysisResults, totalNGrammsCount);

                    if (saveToFile)
                    {
                        fileWriter.AddColumn(textProcessor.TextInfo.TextFileName);
                        fileWriter.SetRowValue("L_CustomRow", _statsUtil.L[textIndex]);
                        fileWriter.SetRowValue("W_CustomRow", string.Format(doubleFormat, _statsUtil.W[textIndex]));

                        foreach (var kvp in analysisResults)
                        {
                            fileWriter.SetRowValue(kvp.Key, GetRelativeValueIfNeeded(kvp.Value, totalNGrammsCount));
                        }
                    }

                    if (!saveToFile || NGrammUtil.NGrammLevel == 1)
                    {
                        DisplayData(textProcessor, analysisResults, totalNGrammsCount, _statsUtil.L[textIndex], _statsUtil.W[textIndex]);
                    }

                    textIndex++;
                    Application.DoEvents();
                }

                _statsUtil.CalculateSigmas();

                #endregion
                #region Writing MX and Sigma

                if (saveToFile)
                {
                    fileWriter.AddColumn("MX");
                    foreach (var kvp in _statsUtil.Statistics)
                    {
                        fileWriter.SetRowValue(kvp.Key, string.Format(doubleFormat, kvp.Value.MX));
                    }

                    fileWriter.AddColumn("Sigma");
                    foreach (var kvp in _statsUtil.Statistics)
                    {
                        fileWriter.SetRowValue(kvp.Key, string.Format(doubleFormat, kvp.Value.Sigma));
                    }

                    fileWriter.AddColumn("Total");
                    fileWriter.SetRowValue("L_CustomRow", string.Empty);
                    fileWriter.SetRowValue("L_CustomRow", string.Empty);
                    fileWriter.SetRowValue("L_CustomRow", _statsUtil.TotalL);

                    fileWriter.SaveData(Application.StartupPath, $"Result-{DateTime.Now.Ticks}.csv");
                }

                //MX and SIGMA
                DataRow drMX = dt.NewRow();
                DataRow drSigma = dt.NewRow();

                drMX["Назва файлу"] = "MX";
                drSigma["Назва файлу"] = "Sigma";

                foreach (var kvp in _statsUtil.Statistics)
                {
                    drMX[kvp.Key] = string.Format(doubleFormat, kvp.Value.MX);
                    drSigma[kvp.Key] = string.Format(doubleFormat, kvp.Value.Sigma);
                }
                dt.Rows.Add(drMX);
                dt.Rows.Add(drSigma);

                for (int i = 0; i < dgvLettersAnalysis.Columns.Count; i++)
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[i].Style.BackColor = Color.LightBlue;
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[i].Style.BackColor = Color.LightGreen;
                }


                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private string GetRelativeValueIfNeeded(int originalValue, int totalCount)
        {
            return Form1.RelativesValues ? string.Format(doubleFormat, originalValue / (double)totalCount) : originalValue.ToString();
        }
        DataTable dt = new DataTable();
        private void DisplayData(TextProcessor textProcessor, Dictionary<string, int> stats, int totalNGrammsCount, int l, double w)
        {
            try
            {
                if (dt.Rows.Count == 0)
                {
                    dt = new DataTable();
                    dt.Columns.Add("Назва файлу");
                }

                //Створюємо новий рядок
                DataRow dr = dt.NewRow();

                //Записуємо назву файла
                dr["Назва файлу"] = textProcessor.TextInfo.TextFileName;

                //Створюємо колонки назв n-грам, та заповнюємо їх назви
                foreach (KeyValuePair<string, int> ColumnName in stats)
                {
                    if (dt.Rows.Count == 0)
                    {
                        dt.Columns.Add(ColumnName.Key);
                        dr[ColumnName.Key] = ColumnName.Value;
                    }
                    else
                    {
                        if (dt.Columns.Contains(ColumnName.Key))
                        {
                            dr[ColumnName.Key] = ColumnName.Value;
                        }
                        else
                        {
                            dt.Columns.Add(ColumnName.Key);
                            dr[ColumnName.Key] = ColumnName.Value;
                        }
                    }
                }

                //Створюємо колонку "Кількість n-грам"
                if (dt.Rows.Count == 0)
                {
                    dt.Columns.Add("Кількість n-грам");
                }
                else
                {
                    // колонку "Кількість n-грам" в кінець таблиці та копіююємо туди дані
                    List<string> tmpValueFromDC = new List<string>();
                    foreach (DataRow tmpDR in dt.Rows)
                    {
                        tmpValueFromDC.Add(tmpDR["Кількість n-грам"].ToString());
                    }

                    DataColumn dc = new DataColumn();
                    dc = dt.Columns["Кількість n-грам"];
                    dt.Columns.Remove(dt.Columns["Кількість n-грам"]);
                    dt.Columns.Add(dc);

                    int i = 0;
                    foreach (DataRow tmpDR2 in dt.Rows)
                    {
                        tmpDR2["Кількість n-грам"] = tmpValueFromDC[i];
                        i++;
                    }

                }
                dr["Кількість n-грам"] = totalNGrammsCount;


                //Колонка "Загальна кількість символів
                if (dt.Rows.Count == 0)
                {
                    dt.Columns.Add("Кількість символів");
                }
                else
                {
                    List<string> tmpValueFromDC = new List<string>();
                    foreach (DataRow tmpDR in dt.Rows)
                    {
                        tmpValueFromDC.Add(tmpDR["Кількість символів"].ToString());
                    }

                    DataColumn dc = new DataColumn();
                    dc = dt.Columns["Кількість символів"];
                    dt.Columns.Remove(dt.Columns["Кількість символів"]);
                    dt.Columns.Add(dc);

                    int i = 0;
                    foreach (DataRow tmpDR2 in dt.Rows)
                    {
                        tmpDR2["Кількість символів"] = tmpValueFromDC[i];
                        i++;
                    }
                }

                int sum = TextAnalyzer.CountLettersCount(textProcessor.TextInfo.CharsStat, pattern);
                dr["Кількість символів"] = sum;

                dt.Rows.Add(dr);

                dgvLettersAnalysis.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000}", stats[ch].MX);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightBlue;
                }


                //Sigma - межа верхньої та нижньої похибки
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Value = "Sigma";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Style.BackColor = Color.LightGreen;


                foreach (char ch in TextAnalyzer.GetSpecialSymbols())
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000}", stats[ch].Sigma);
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
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000}", stats[ch].MX);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightBlue;
                }


                //Sigma - межа верхньої та нижньої похибки
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Value = "Sigma";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Style.BackColor = Color.LightGreen;


                foreach (char ch in pattern)
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000}", stats[ch].Sigma);
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
                            dgvLettersAnalysis.Rows[i].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000}", TI[i].CharsStat[ch] / (double)sum);
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
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000}", stats[ch].MX);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightBlue;
                }

                //Sigma - межа верхньої та нижньої похибки
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Value = "Sigma";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Style.BackColor = Color.LightGreen;


                foreach (char ch in TextAnalyzer.GetSpecialSymbols())
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000}", stats[ch].Sigma);
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
                            dgvLettersAnalysis.Rows[i].Cells[string.Format("c{0}", key)].Value = string.Format("{0:0.000}", TI[i].CharsStat[key] / (double)sum);
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
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000}", stats[ch].MX);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 2].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightBlue;
                }

                //Sigma - межа верхньої та нижньої похибки
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Value = "Sigma";
                dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells["cFileName"].Style.BackColor = Color.LightGreen;


                foreach (char ch in pattern)
                {
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Value = string.Format("{0:0.000}", stats[ch].Sigma);
                    dgvLettersAnalysis.Rows[dgvLettersAnalysis.RowCount - 1].Cells[string.Format("c{0}", ch)].Style.BackColor = Color.LightGreen;
                }


                DGV_Export_to_CSV.Export(dgvLettersAnalysis, @"textStatisticLetters.csv");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

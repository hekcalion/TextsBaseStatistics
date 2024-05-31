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
                // тут взагалі ше нічого не працює з вибором мови тому всюди англійськи бо розбирав англ тексти
                this.Text = CT;
                
                pattern = TextAnalyzer.GetLettersOrDigits();
                UpdateDataGridView();
                if (CT == "nGram")
                {
                    switch (Form1.LanguageType)
                    {
                        //Українська
                        case 0:
                            ngramLang = Languages.EN;
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
                // переведення в нижній регістр тут треба ше побавитись бо завжди ігнорує регістри
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

            Parallel.ForEach(TI, (file, state, index) =>
            {
                var textIndex = 0;
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
                // переведення в нижній регістр тут треба ше побавитись бо завжди ігнорує регістри
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

                int l = _statsUtil.L[(int)index];
                double w = _statsUtil.W[(int)index];
                DisplayData(textProcessor, analysisResults, totalNGrammsCount, l, w);

                textIndex++;
                Application.DoEvents();
            });

            _statsUtil.CalculateSigmas();

            #endregion

            #region Writing MX and Sigma

            
            var mxRowIndex = EnsureRowExists("MX");
            var sigmaRowIndex = EnsureRowExists("Sigma");
            var totalRowIndex = EnsureRowExists("Total");

            foreach (var kvp in _statsUtil.Statistics)
            {
                var colIndexAbsolute = EnsureColumnExists(kvp.Key, out DataGridViewColumn absColumn, isRelative: false);
                var colIndexRelative = EnsureColumnExists(kvp.Key, out DataGridViewColumn relColumn, isRelative: true);

                
                _columnMapping[absColumn] = relColumn;
                _columnMapping[relColumn] = absColumn;

                dgvLettersAnalysis.Rows[mxRowIndex].Cells[colIndexAbsolute].Value = kvp.Value.MX;
                dgvLettersAnalysis.Rows[mxRowIndex].Cells[colIndexRelative].Value = kvp.Value.MX;

                dgvLettersAnalysis.Rows[sigmaRowIndex].Cells[colIndexAbsolute].Value = kvp.Value.Sigma;
                dgvLettersAnalysis.Rows[sigmaRowIndex].Cells[colIndexRelative].Value = kvp.Value.Sigma;
               
            }

            var lColIndex = EnsureColumnExists("L", out _);
            var fileNameColIndex = EnsureColumnExists("FileName", out _);
            dgvLettersAnalysis.Rows[totalRowIndex].Cells[lColIndex].Value = _statsUtil.TotalL;
            dgvLettersAnalysis.Rows[mxRowIndex].Cells[fileNameColIndex].Value = "MX";
            dgvLettersAnalysis.Rows[sigmaRowIndex].Cells[fileNameColIndex].Value = "Sigma";
            dgvLettersAnalysis.Rows[totalRowIndex].Cells[fileNameColIndex].Value = "Total";
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
                
                var fileNameColIndex = EnsureColumnExists("FileName", out _);
                var lColIndex = EnsureColumnExists("L", out _);
                var wColIndex = EnsureColumnExists("W", out _);

               
                var rowIndex = dgvLettersAnalysis.Rows.Add();
                var row = dgvLettersAnalysis.Rows[rowIndex];

                
                row.Cells[fileNameColIndex].Value = textProcessor.TextInfo.TextFileName;

                
                row.Cells[lColIndex].Value = l;

               
                row.Cells[wColIndex].Value = w;

                
                foreach (var kvp in stats)
                {
                    var colIndexAbsolute = EnsureColumnExists(kvp.Key, out DataGridViewColumn absColumn, isRelative: false);
                    var colIndexRelative = EnsureColumnExists(kvp.Key, out DataGridViewColumn relColumn, isRelative: true);

                    // це новий метод відображення стовців замість colName + "_Rel" : colName + "_Abs"
                    _columnMapping[absColumn] = relColumn;
                    _columnMapping[relColumn] = absColumn;

                    row.Cells[colIndexAbsolute].Value = kvp.Value; 
                    row.Cells[colIndexRelative].Value = double.Parse(GetRelativeValueIfNeeded(kvp.Value, totalNGrammsCount));
                }
            }));
        }
        private int EnsureColumnExists(string colName, out DataGridViewColumn column, bool isRelative = false)
        {
            string fullColName = isRelative ? colName + "_Rel" : colName + "_Abs"; // посувало б пофіксити але і так працює з початку в мене колонки мали різні імена для відносних і абсолютних
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
            column.ValueType = typeof(double); 
            column.Visible = !isRelative; 
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

        private void dgvLettersAnalysis_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (dgvLettersAnalysis == null || dgvLettersAnalysis.Rows == null)
            {
                e.SortResult = 0;
                e.Handled = true;
                return;
            }

            int rowIndex1 = e.RowIndex1;
            int rowIndex2 = e.RowIndex2;

            
            int totalRows = dgvLettersAnalysis.Rows.Count;

            
            if (rowIndex1 >= totalRows - 3 && rowIndex2 >= totalRows - 3)
            {
                
                e.SortResult = rowIndex1.CompareTo(rowIndex2);
            }
            else if (rowIndex1 >= totalRows - 3)
            {
                
                e.SortResult = 1;
            }
            else if (rowIndex2 >= totalRows - 3)
            {
                
                e.SortResult = -1;
            }
            else
            {
                /// вискакує ерорка треба ше посидіти пошаманити
                e.SortResult = String.Compare(e.CellValue1.ToString(), e.CellValue2.ToString());
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

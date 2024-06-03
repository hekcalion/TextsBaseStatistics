using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TextsBase.Utill;

namespace TextsBase
{
    public partial class FormAnalysis : Form
    {
        private NGramStatistic _nGramStatistic;
        private DataTable _dataTableWithAbsoluteData;
        private DataTable _dataTableWithRelativeData;

        public FormAnalysis(NGramStatistic nGramStatistic)
        {
            InitializeComponent();
            _dataTableWithAbsoluteData = new DataTable();
            _dataTableWithRelativeData = new DataTable();
            _nGramStatistic = nGramStatistic;
            GenerateAbsoluteDataTable();
            GenerateRelativeDataTable();
        }

        private void FormAnalysis_Load(object sender, EventArgs e)
        {
            try
            {
                dgvLettersAnalysis.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvLettersAnalysis.DataSource = _dataTableWithAbsoluteData;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GenerateAbsoluteDataTable()
        {
            _dataTableWithAbsoluteData.Columns.Add("Назва файлу", typeof(string));

            Func<string, bool> isSymbol = s => !s.All(char.IsLetterOrDigit);

            var nGrams = _nGramStatistic.totalAbsoluteStatistic
                .Select(x => x.Key)
                .ToList();

            var nGramList = nGrams.Where(x => !isSymbol(x)).OrderBy(x => x).ToList();
            var symbolList = nGrams.Where(isSymbol).OrderBy(x => x).ToList();

            var sortedNGram = nGramList.Concat(symbolList).ToList();


            foreach (var nGram in sortedNGram)
            {
                _dataTableWithAbsoluteData.Columns.Add(nGram, typeof(string));
            }

            
            foreach(var stat in _nGramStatistic.filesAbsoluteStatistic)
            {
                DataRow row = _dataTableWithAbsoluteData.NewRow();
                row["Назва файлу"] = stat.Key;

                foreach (var ngram in sortedNGram)
                {
                    row[ngram] = stat.Value.TryGetValue(ngram, out int value) ? value.ToString() : "";
                }

                _dataTableWithAbsoluteData.Rows.Add(row);
            }

            DataRow mxRow = _dataTableWithAbsoluteData.NewRow();
            mxRow["Назва файлу"] = "MX";
            foreach (var ngram in sortedNGram)
            {
                mxRow[ngram] = _nGramStatistic.MX.TryGetValue(ngram, out double value) ? value.ToString() : "";
            }
            _dataTableWithAbsoluteData.Rows.Add(mxRow);


            DataRow sigmaRow = _dataTableWithAbsoluteData.NewRow();
            sigmaRow["Назва файлу"] = "Sigma";
            foreach (var ngram in sortedNGram)
            {
                sigmaRow[ngram] = _nGramStatistic.Sigma.TryGetValue(ngram, out double value) ? value.ToString() : "";
            }
            _dataTableWithAbsoluteData.Rows.Add(sigmaRow);
        }

        private void GenerateRelativeDataTable()
        {
            _dataTableWithRelativeData.Columns.Add("Назва файлу", typeof(string));

            Func<string, bool> isSymbol = s => !s.All(char.IsLetterOrDigit);

            var nGrams = _nGramStatistic.totalAbsoluteStatistic
                .Select(x => x.Key)
                .ToList();

            var nGramList = nGrams.Where(x => !isSymbol(x)).OrderBy(x => x).ToList();
            var symbolList = nGrams.Where(isSymbol).OrderBy(x => x).ToList();

            var sortedNGram = nGramList.Concat(symbolList).ToList();


            foreach (var nGram in sortedNGram)
            {
                _dataTableWithRelativeData.Columns.Add(nGram, typeof(string));
            }


            foreach (var stat in _nGramStatistic.filesRelativeStatistic)
            {
                DataRow row = _dataTableWithRelativeData.NewRow();
                row["Назва файлу"] = stat.Key;

                foreach (var ngram in sortedNGram)
                {
                    row[ngram] = stat.Value.TryGetValue(ngram, out double value) ? value.ToString() : "";
                }

                _dataTableWithRelativeData.Rows.Add(row);
            }


            DataRow mxRow = _dataTableWithRelativeData.NewRow();
            mxRow["Назва файлу"] = "MX";
            foreach (var ngram in sortedNGram)
            {
                mxRow[ngram] = _nGramStatistic.MX.TryGetValue(ngram, out double value) ? value.ToString() : "";
            }
            _dataTableWithRelativeData.Rows.Add(mxRow);


            DataRow sigmaRow = _dataTableWithRelativeData.NewRow();
            sigmaRow["Назва файлу"] = "Sigma";
            foreach (var ngram in sortedNGram)
            {
                sigmaRow[ngram] = _nGramStatistic.Sigma.TryGetValue(ngram, out double value) ? value.ToString() : "";
            }
            _dataTableWithRelativeData.Rows.Add(sigmaRow);
        }


        private void AdjustColumnFillWeight(DataGridView dgv)
        {
            int columnCount = dgv.Columns.Count;

            if (columnCount == 0) return;

            float baseFillWeight = 65535f / columnCount;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.FillWeight = baseFillWeight;
            }
        }

        private void saveAbsoluteBTN_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                ExportDataTableToCSV(_dataTableWithAbsoluteData, filePath);
            }
        }

        private void saveRelativeBTN_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                ExportDataTableToCSV(_dataTableWithRelativeData, filePath);
            }
        }


        private void ExportDataTableToCSV(DataTable dataTable, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            // Use UTF-8 encoding with BOM
            File.WriteAllText(filePath, sb.ToString(), new System.Text.UTF8Encoding(true));
        }

        private void dgvLettersAnalysis_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.FillWeight = 5;
        }

        private void chkRelativeValues_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRelativeValues.Checked == true)
            {
                dgvLettersAnalysis.DataSource = _dataTableWithRelativeData;
            }
            else
            {
                dgvLettersAnalysis.DataSource = _dataTableWithAbsoluteData;
            }
        }
    }
}

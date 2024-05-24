using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextsBase
{
    public class FileWriter
    {
        private Dictionary<string, StringBuilder> _rows = new Dictionary<string, StringBuilder>();
        private List<string> _columns = new List<string>();

        public void AddRow(string rowId, object rowHeader)
        {
            var row = new StringBuilder();
            row.Append(rowHeader);
            row.Append('\t');

            foreach (var column in _columns)
            {
                row.Append('\t');
            }

            _rows.Add(rowId, row);
        }

        public void AddColumn(string columnName)
        {
            var iterator = 0;
            foreach (var kvp in _rows)
            {
                if (iterator == 0)
                {
                    kvp.Value.Append(columnName);
                    _columns.Add(columnName);
                }

                kvp.Value.Append('\t');
                iterator++;
            }
        }

        public void SetRowValue(string rowId, object value)
        {
            if (!_rows.ContainsKey(rowId))
            {
                AddRow(rowId, rowId);
            }

            var row = _rows[rowId];
            if (row[row.Length - 2] == '\t' && row[row.Length - 1] == '\t')
            {
                row.Remove(row.Length - 1, 1);
            }

            row.Append(value);
            row.Append('\t');
        }

        public void SaveData(string directory, string fileName)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var fullPath = Path.Combine(directory, fileName);
            var sb = new StringBuilder();
            foreach (var kvp in _rows)
            {
                sb.Append(kvp.Value);
                sb.Append('\n');
            }

            File.WriteAllText(fullPath, sb.ToString());
        }
    }
}

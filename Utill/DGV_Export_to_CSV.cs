using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextsBase
{
    public class DGV_Export_to_CSV
    {
        private static string[] GetRowValues(DataGridView dgv, int rowId)
        {
            var result = new string[dgv.ColumnCount];
            for (var j = 0; j < dgv.ColumnCount; j++)
            {
                result[j] = dgv[j, rowId].Value == null ? "" : dgv[j, rowId].Value.ToString();
            }
            return result;
        }

        
        public static void Export(DataGridView dgv, string fileName)
        {
            using (var sw = new StreamWriter(fileName))
            {
                for (var i = 0; i < dgv.RowCount; i++)
                {
                    sw.WriteLine(string.Join(";", GetRowValues(dgv, i)));
                }
            }
        }
       

        public  void Save (DataGridView dgv, string filePath, bool isRelative)
        {
            StringBuilder csvData = new StringBuilder();

            // Write header
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if ((isRelative && column.Name.EndsWith("_Rel")) || (!isRelative && column.Name.EndsWith("_Abs")) || column.Name == "FileName")
                {
                    csvData.Append(column.HeaderText + ",");
                }
            }
            csvData.AppendLine();

            // Write rows
            foreach (DataGridViewRow row in dgv.Rows)
            {
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    if ((isRelative && column.Name.EndsWith("_Rel")) || (!isRelative && column.Name.EndsWith("_Abs")) || column.Name == "FileName")
                    {
                        csvData.Append(row.Cells[column.Index].Value + ",");
                    }
                }
                csvData.AppendLine();
            }

            File.WriteAllText(filePath, csvData.ToString());
        }
    }
    }
    
    

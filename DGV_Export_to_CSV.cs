using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextsBase
{
    public static class DGV_Export_to_CSV
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

        /// <summary>
        /// Експорт вмісту таблиці в CSV файл
        /// </summary>
        /// <param name="dgv">Екземпляр DataGridView</param>
        /// <param name="fileName">Назва файлу</param>
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
    }
}

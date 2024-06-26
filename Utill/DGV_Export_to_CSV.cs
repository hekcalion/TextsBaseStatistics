﻿using System;
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
        
        public  void Save (DataGridView dgv, string filePath, bool isRelative)
        {
            StringBuilder csvData = new StringBuilder();

            // напевно тут ше треба підшаманити бо коли зберігаєм відносні файли втікає перша колонка
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if ((isRelative && column.Name.EndsWith("_Rel")) || (!isRelative && column.Name.EndsWith("_Abs")) || column.Name == "FileName")
                {
                    csvData.Append(column.HeaderText + ",");
                }
            }
            csvData.AppendLine();

            
            foreach (DataGridViewRow row in dgv.Rows)
            {
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    if ((isRelative && column.Name.EndsWith("_Rel")) || (!isRelative && column.Name.EndsWith("_Abs")) || column.Name == "FileName")
                    {
                        var value = row.Cells[column.Index].Value;
                        if (value is double doubleValue)
                        {
                            
                            csvData.Append(doubleValue.ToString("F6", CultureInfo.InvariantCulture) + ",");
                        }
                        else
                        {
                            csvData.Append(value + ",");
                        }
                    }
                }
                csvData.AppendLine();
            }

            File.WriteAllText(filePath, csvData.ToString(), Encoding.UTF8);
        }
    }
    }
    
    

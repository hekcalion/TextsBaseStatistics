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

namespace TextsBase
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cbSelectLanguage.SelectedIndex = 0;
            cbSpaceDisable.SelectedIndex = 0;
            cbTextEncoding.SelectedIndex = 0;
        }

        private List<Text> Texts;
        public static int LanguageType = 0;
        private void Btn_ChoseFolderTexts_Click(object sender, EventArgs e)
        {
            try
            {
                Texts = new List<Text>();
                tssLabelCountFiles.Text = "Загальна кількість файлів: 0";
                dgv_Texts.Rows.Clear();
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Виберіть папку з текстами для дослідження";
                if (fbd.ShowDialog() == DialogResult.OK && !fbd.SelectedPath.Equals(""))
                {
                    //Завантажуемо список файлів із вибраної папки
                    List<string> lst_files = Directory.GetFiles(fbd.SelectedPath, "*.txt").ToList();
                    //Додаємо до списку файли які знаходяться в підпапках
                    lst_files.AddRange(GetFilesInSubDirectories(fbd.SelectedPath));
                    //Виводимо кількість вибраних файлів
                    tssLabelCountFiles.Text = string.Format("Загальна кількість файлів: {0}", lst_files.Count);
                    //Прогресбар ставимо в 0
                    tssProgressBar.Value = 0;
                    //Максимум прогресбару
                    tssProgressBar.Maximum = lst_files.Count;
                    //Почерзі читаємо файли
                    foreach (string file in lst_files)
                    {
                        List<char> chars = new List<char>();
                        using (StreamReader sr = new StreamReader(file, TextEncoding))
                        {
                            int BufferSize = 4096;
                            char[] CharBuffer = new char[BufferSize];
                            while (!sr.EndOfStream)
                            {
                                int ReadCount = sr.Read(CharBuffer, 0, CharBuffer.Length);
                                for (int i = 0; i < ReadCount; i++)
                                {
                                    chars.Add(CharBuffer[i]);
                                }
                            }
                        }
                        //Створюємо новий екземпляр класу "Text"
                        Text t = new Text();
                        t.TextFull = chars.ToArray();
                        t.TextFileName = Path.GetFileName(file);
                        t.Path = file;

                        //Проводимо аналіз тексту
                        t.CharsStat = TextAnalizer.Analize(t.TextFull);

                        //Виводимо назву текстового файлу користувачеві
                        dgv_Texts.Rows.Add();
                        dgv_Texts.Rows[dgv_Texts.Rows.Count - 1].Cells["cPP"].Value = dgv_Texts.Rows.Count;
                        dgv_Texts.Rows[dgv_Texts.Rows.Count - 1].Cells["cNameText"].Value = t.TextFileName;
                        dgv_Texts.Rows[dgv_Texts.Rows.Count - 1].Cells["cCharsCount"].Value = t.TextFull.Length;

                        Texts.Add(t);
                        //Збільшуємо позицію індикатора ProgressBar
                        tssProgressBar.PerformStep();
                        
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Метод для отримання списку всих файлів у вкладених папках
        /// </summary>
        /// <param name="dir">Папка для перевірки на наявність папок і файлів в них</param>
        /// <returns></returns>
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
                if (Texts == null) return;
                //Створюємо екземпляр класу Text, в якому будемо зберігати результати підрахунку
                Text t = new Text { CharsStat = new Dictionary<char, int>() };

                //В циклі рахуемо частоту символів та записуємо в еземпляр класу
                foreach (Text text in Texts)
                {
                    foreach (KeyValuePair<char,int> cs in text.CharsStat)
                    {
                        if (t.CharsStat.ContainsKey(cs.Key))
                        {
                            t.CharsStat[cs.Key] += cs.Value;
                        }
                        else
                        {
                            t.CharsStat.Add(cs.Key, cs.Value);
                        }
                    }
                }
                //Виводимо результати підрахунку
                VisualizeStats(t, rbShowSymbols.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Метод для візуалізації даних
        /// </summary>
        /// <param name="info">Екземпляр класу з даними для візуалізації</param>
        /// <param name="visualiseSymbols">Показувати символи</param>
        private void VisualizeStats(Text info, bool visualiseSymbols)
        {
            //Очищаємо графік
            Chart.Series[0].Points.Clear();
            Chart.ChartAreas[0].AxisX.CustomLabels.Clear();
            //Очищаемо таблицю з статистикою символів
            dgv_CharStat.Rows.Clear();

            int i = 0;
            //Рахуємо загальну кількість символів
            int totalCharsCount = info.CharsStat.Sum(k => k.Value);

            //Рахуємо частоту зустрічі в тексті символів
            foreach (KeyValuePair<char, int> ch in info.CharsStat.OrderByDescending(v => v.Value))
            {
                //Створюємо шаблон для підрахунку, на основі вибору користувача на головній формі
                char[] pattern = null; 
                if (visualiseSymbols)
                {
                    //Шаблон при вибору тільки символів
                    pattern = TextAnalizer.patternSymbols;
                }
                else
                {
                    //Вибір мови літер для підрахунку
                    switch (cbSelectLanguage.SelectedIndex)
                    {
                        //Українська
                        case 0:
                            pattern = TextAnalizer.patternLettersUA;
                            break;
                        //Російська
                        case 1:
                            pattern = TextAnalizer.patternLettersRU;
                            break;
                        //Англійська
                        case 2:
                            pattern = TextAnalizer.patternLettersEN;
                            break;
                        default:
                            break;
                    }
                }



                if (!pattern.Contains(ch.Key) || !info.CharsStat.ContainsKey(ch.Key)) continue;

                //Виводимо дані в таблицю частоти зустрічі в тексті симовлів
                dgv_CharStat.Rows.Add();
                dgv_CharStat.Rows[dgv_CharStat.Rows.Count - 1].Cells["cLetter"].Value = ch.Key;
                dgv_CharStat.Rows[dgv_CharStat.Rows.Count - 1].Cells["cFreq"].Value = info.CharsStat[ch.Key];

                //Виводимо дані на графік
                Chart.Series[0].Points.AddXY(i++, info.CharsStat[ch.Key] / (double)totalCharsCount);
                Chart.ChartAreas[0].AxisX.CustomLabels.Add((i - 1) - 0.5, i - 0.5, new String(ch.Key, 1));
            }
        }

        private void RbShowSymbols_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void RbShowBukvu_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

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
                    //Chart.ChartAreas[0].AxisX.CustomLabels.Add((i - 1) - 0.5, i - 0.5, new String(ch.Key, 1)); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnLettersAnalysis_Click(object sender, EventArgs e)
        {
            new FormAnalysis(Texts, "Аналіз літер").ShowDialog();
        }

        private void CbSelectLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            LanguageType = cbSelectLanguage.SelectedIndex;
        }

        private void BtnSymbolAnalysis_Click(object sender, EventArgs e)
        {
            new FormAnalysis(Texts, "Аналіз символів").ShowDialog();
        }

        private void BtnLetterAbsolute_Click(object sender, EventArgs e)
        {
            new FormAnalysis(Texts, "Літери (абсолютні значення)").ShowDialog();
        }

        private void BtnSymbolAbsolute_Click(object sender, EventArgs e)
        {
            new FormAnalysis(Texts, "Символи (абсолютні значення)").ShowDialog();
        }

        private void CbSpaceDisable_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbSpaceDisable.SelectedIndex == 1)
                {
                    TextAnalizer.DisableSpace(true);
                }
                else
                {
                    TextAnalizer.DisableSpace(false);
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
                    TextEncoding = Encoding.GetEncoding("UTF-8");
                }
                else
                {
                    TextEncoding = Encoding.GetEncoding("windows-1251");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnNgramm_Click(object sender, EventArgs e)
        {
            new FormAnalysis(Texts, "nGram", (byte)nud_n.Value).ShowDialog();
        }

        public static bool RelativesValues = false;
        private void RelativeValues_CheckedChanged(object sender, EventArgs e)
        {
           RelativesValues = relativeValues.Checked;
        }
    }
}

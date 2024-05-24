using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextsBase
{
    /// <summary>
    /// Клас який описує текст та його статистичні данні
    /// </summary>
    public class Text
    {
        /// <summary>
        /// Назва тексту
        /// </summary>
        public string TextFileName { get; set; }
        /// <summary>
        /// Статистика по символам
        /// </summary>
        public Dictionary<char,int> CharsStat { get; set; }
        /// <summary>
        /// Сам текст
        /// </summary>
        public char[] TextFull { get; set; }
        /// <summary>
        /// Шлях до файлу
        /// </summary>
        public string Path { get; set; }
    }
}

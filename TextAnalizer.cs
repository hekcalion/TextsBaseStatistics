using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextsBase
{
    public static class TextAnalizer
    {
        private static char[] pattern = new char[] { 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'є', 'ж', 'з', 'и', 'і', 'ї', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я', '.', ',', '!', '?', ':', ';', '-', '—', ' ', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        public static char[] patternLettersUA = new char[] { 'а', 'б', 'в', 'г', 'д', 'е', 'є', 'ж', 'з', 'и', 'і', 'ї', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ь', 'ю', 'я' };
        public static char[] patternLettersRU = new char[] { 'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ы', 'ь', 'э', 'ю', 'я' };
        public static char[] patternLettersEN = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        public static char[] patternSymbols = new char[] { ' ', '.', ',', '!', '?', ':', ';', '-', '—' };

        public static string[] patternNGramUA = new string[] { };
        public static string[] patternNGramRU = new string[] { };
        public static string[] patternNGramEN = new string[] { };

        public static Dictionary<char, int> Analize(char[] text)
        {
            var result = new Dictionary<char, int>();

            for (var i = 0; i < text.Length; i++)
            {
                var currentChar = char.ToLower(text[i]);
                if (!pattern.Contains(currentChar)) continue;

                if (result.ContainsKey(currentChar))
                {
                    result[currentChar]++;
                }
                else
                {
                    result.Add(currentChar, 1);
                }
            }

            return result;
        }

        public static void DisableSpace(bool Disable)
        {
            if (Disable)
            {
                patternSymbols = new char[] { '.', ',', '!', '?', ':', ';', '-', '—' };
            }
            else
            {
                patternSymbols = new char[] { ' ', '.', ',', '!', '?', ':', ';', '-', '—' };
            }
        }

        public static Dictionary<char, StatsInfo> CalculateStatistics(List<Text> textInfos, char[] patternLet)
        {
            var result = new Dictionary<char, StatsInfo>();
            var lettersCount = CountLettersCount(textInfos, patternLet) / (double)textInfos.Count;
            var lettersSpacesCount = (lettersCount + textInfos.Select(t => t).SelectMany(t => t.CharsStat).Where(k => k.Key == ' ').Sum(k => k.Value)) / (double)textInfos.Count;
            var totalCharCount = textInfos.Select(t => t).SelectMany(t => t.CharsStat).Sum(k => k.Value) / (double)textInfos.Count;
            foreach (var letter in patternLet)
            {
                var statInfo = new StatsInfo();
                statInfo.MX = CalculateMX(textInfos, letter);
                statInfo.DX = CalculateDX(textInfos, letter, statInfo.MX);
                statInfo.MX = (statInfo.MX / (double)lettersCount);
                statInfo.Sigma = (Math.Sqrt(statInfo.DX) / (double)lettersCount);
                result.Add(letter, statInfo);
            }
            foreach (var character in patternSymbols)
            {
                if (character == ' ') continue;
                var statInfo = new StatsInfo();
                statInfo.MX = CalculateMX(textInfos, character);
                statInfo.DX = CalculateDX(textInfos, character, statInfo.MX);
                statInfo.MX = (statInfo.MX / (double)totalCharCount);
                statInfo.Sigma = (Math.Sqrt(statInfo.DX) / (double)totalCharCount);
                result.Add(character, statInfo);
            }
            var statInfoSpace = new StatsInfo();
            statInfoSpace.MX = CalculateMX(textInfos, ' ');
            statInfoSpace.DX = CalculateDX(textInfos, ' ', statInfoSpace.MX);
            statInfoSpace.MX = (statInfoSpace.MX / ((double)lettersSpacesCount + (double)totalCharCount));
            statInfoSpace.Sigma = (Math.Sqrt(statInfoSpace.DX) / (double)lettersSpacesCount);
            result.Add(' ', statInfoSpace);
            return result;
        }

        public static double CalculateMX(List<Text> textInfos, char character)
        {
            var result = 0d;
            for (var i = 0; i < textInfos.Count; i++)
            {
                if (!textInfos[i].CharsStat.ContainsKey(character)) continue;
                result += textInfos[i].CharsStat[character];
            }
            result /= (double)textInfos.Count;
            return result;
        }

        public static double CalculateDX(List<Text> textInfos, char character, double MX)
        {
            var result = 0d;
            for (var i = 0; i < textInfos.Count; i++)
            {
                if (!textInfos[i].CharsStat.ContainsKey(character)) continue;
                result += (textInfos[i].CharsStat[character] - MX) * (textInfos[i].CharsStat[character] - MX);
            }
            result /= (double)textInfos.Count * (double)textInfos.Count;
            return result;
        }

        public static int CountLettersCount(Dictionary<char, int> textInfo, char[] pattern)
        {
            var result = 0;
            foreach (var txt in textInfo)
            {
                if (!pattern.Contains(txt.Key)) continue;
                result += txt.Value;
            }
            return result;
        }

        public static int CountLettersCount(List<Text> textInfos, char[] pattern)
        {
            var result = 0;
            foreach (var ti in textInfos)
            {
                foreach (var txt in ti.CharsStat)
                {
                    if (!pattern.Contains(txt.Key)) continue;
                    result += txt.Value;
                }
            }
            return result;
        }
    }
}

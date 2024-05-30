using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TextsBase
{
    public static class TextAnalyzer
    {
        public static bool ignoreSpaces = true;
        public static bool ignoreCase = true;
        public static ConcurrentBag<char> textLettersOrDigits = new ConcurrentBag<char>();
        public static ConcurrentBag<char> textSpecialSymbols = new ConcurrentBag<char>();


        public static char[] GetLettersOrDigits()
        {
            if(ignoreCase) return textLettersOrDigits
                    .Select(x => Char.ToLower(x))
                    .Distinct()
                    .OrderBy(x => x).ToArray();

            return textLettersOrDigits.OrderBy(x => x).ToArray();
        }

        public static char[] GetSpecialSymbols()
        {
            if(ignoreSpaces) return textSpecialSymbols.Where(x => !char.IsWhiteSpace(x) && x != '\r' && x != '\n')
                    .OrderBy(x => x)
                    .ToArray();

            return textSpecialSymbols.Where(x => x != '\r' && x != '\n')
                    .OrderBy(x => x)
                    .ToArray();
        }

        public static Dictionary<char, int> Analize(char[] text)
        {
            var result = new Dictionary<char, int>();

            for (var i = 0; i < text.Length; i++)
            {
                char currentChar = text[i];

                if (IsSpecialSymbol(currentChar))
                {
                    if(!textSpecialSymbols.Contains(currentChar)) textSpecialSymbols.Add(currentChar);
                }
                else
                {
                    if (!textLettersOrDigits.Contains(currentChar)) textLettersOrDigits.Add(currentChar);
                }
                
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

        public static bool IsSpecialSymbol(char c)
        {
            return !Char.IsLetterOrDigit(c);
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
                statInfo.MX = (statInfo.MX / (double)lettersCount);
                statInfo.DX = CalculateDX(textInfos, letter, statInfo.MX);              
                statInfo.Sigma = (Math.Sqrt(statInfo.DX) / (double)lettersCount);
                result.Add(letter, statInfo);
            }
            foreach (var character in TextAnalyzer.GetSpecialSymbols())
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

        public static Dictionary<char, int> GetFilesFrequency(List<Text> texts)
        {
            Dictionary<char, int> frequency = new Dictionary<char, int>();

            foreach (Text text in texts)
            {
                foreach (KeyValuePair<char, int> cs in text.CharsStat)
                {
                    char key = (ignoreCase) ? char.ToLower(cs.Key) : cs.Key;

                    if (frequency.ContainsKey(key))
                    {
                        frequency[key] += cs.Value;
                    }
                    else
                    {
                        frequency.Add(key, cs.Value);
                    }
                }
            }

            return frequency;
        }
    }
}

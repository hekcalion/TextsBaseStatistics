using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextsBase
{
    public class NGrammUtil
    {
        public enum Languages
        {
            UA,
            EN,
            All,
            ADD
        }

        public static readonly Dictionary<Languages, List<char>> PatternsLetters = new Dictionary<Languages, List<char>>
        {
            { Languages.UA, new List<char> { 'а', 'б', 'в', 'г', 'д', 'е', 'є', 'ж', 'з', 'и', 'і', 'ї', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ь', 'ю', 'я' } },
            { Languages.EN, new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' } },
            { Languages.All, new List<char> {  } },
            { Languages.ADD, new List<char> {  } },
        };

        public static readonly List<char> PatternSymbols = new List<char> { '.', ',', '!', '?', ':', ';', '-', '—' };

        public List<char> PatternLetters
        {
            get
            {
                return PatternsLetters[Language];
            }
        }

        public byte NGrammLevel { get; private set; }

        public Languages Language { get; private set; }
        public bool UseLetters { get; private set; }
        public bool UseSymbols { get; private set; }
        public bool UseSpaceSymbol { get; private set; }

        public List<char> Pattern { get; private set; }

        public NGrammUtil(byte nGrammLevel, Languages language, bool useLetters, bool useSymbols, bool useSpaceSymbol)
        {
            NGrammLevel = nGrammLevel;

            Language = language;
            UseLetters = useLetters;
            UseSymbols = useSymbols;
            UseSpaceSymbol = useSpaceSymbol;

            GenerateFullPattern();
        }

        public void GenerateFullPattern()
        {
            Pattern = new List<char>();

            if (UseLetters)
            {
                Pattern.AddRange(PatternLetters);
            }

            if (UseSymbols)
            {
                Pattern.AddRange(PatternSymbols);
            }

            if (UseSpaceSymbol)
            {
                Pattern.Add(' ');
            }
        }

        public bool NGrammIsOk(string nGramm)
        {
            if (nGramm.Length != NGrammLevel)
            {
                return false;
            }

            if (nGramm.Any(c => !Pattern.Contains(c)))
            {
                return false;
            }

            return true;
        }

        public List<string> GenerateNGramm(byte level, Languages language, bool useSymbols = false, bool useSpaceSymbol = false)
        {
            var result = new List<string>();

            if (level == 1)
            {
                return Pattern.Select(c => c.ToString()).ToList();
            }
            else
            {
                var prevResult = GenerateNGramm((byte)(level - 1), language, useSymbols, useSpaceSymbol);
                for (var i = 0; i < Pattern.Count; i++)
                {
                    for (var j = 0; j < prevResult.Count; j++)
                    {
                        result.Add(Pattern[i] + prevResult[j]);
                    }
                }
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextsBase
{
    class TextProcessor
    {
        public Text TextInfo { get; private set; }
        public NGrammUtil _nGrammUtil { get; private set; }

        public TextProcessor(NGrammUtil ngrammUtil, string textPath, char[] text)
        {
            _nGrammUtil = ngrammUtil;

            TextInfo = new Text();
            TextInfo.TextFileName = Path.GetFileName(textPath);
            TextInfo.TextFull = text;
        }

        public Dictionary<string, int> Analyze(out int totalNGrammsCount)
        {
            var result = new Dictionary<string, int>();
            totalNGrammsCount = 0;

            for (var i = 0; i < TextInfo.TextFull.Length - 1; i++)
            {
                if (TextInfo.TextFull.Length - i < _nGrammUtil.NGrammLevel)
                {
                    continue;
                }

                var nGramm = new string(TextInfo.TextFull, i, _nGrammUtil.NGrammLevel);
                if (result.ContainsKey(nGramm))
                {
                    result[nGramm]++;
                    totalNGrammsCount++;
                }
                else if (_nGrammUtil.NGrammIsOk(nGramm))
                {
                    result.Add(nGramm, 1);
                    totalNGrammsCount++;
                }
            }

            return result;
        }
    }
}

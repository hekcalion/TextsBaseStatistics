using System;
using System.Collections.Generic;

namespace TextsBase
{
    public class StatsUtil
    {
        private int _addedResultsCount = 0;
        private int _totalNGrammsCount = 0;

        private readonly NGrammUtil _ngramUtil;

        public Dictionary<string, double> MXAnalysisResults { get; private set; }
        public Dictionary<string, StatsInfo> Statistics { get; private set; }

        public List<int> L { get; private set; }
        public int TotalL { get; set; }
        public List<double> W { get; set; }

        public StatsUtil(NGrammUtil nGramUtil)
        {
            _ngramUtil = nGramUtil;

            MXAnalysisResults = new Dictionary<string, double>();
            Statistics = new Dictionary<string, StatsInfo>();
            L = new List<int>();
            W = new List<double>();
        }

        public void AddToResults(char[] text, Dictionary<string, int> analysisResult, int totalNGrammsCount)
        {
            var l = CountCharactersCount(text);
            foreach (var kvp in analysisResult)
            {
                var value = (kvp.Value / (double)totalNGrammsCount) * l;
                if (MXAnalysisResults.ContainsKey(kvp.Key))
                {
                    MXAnalysisResults[kvp.Key] += value;
                }
                else
                {
                    MXAnalysisResults.Add(kvp.Key, value);
                }
            }

            _addedResultsCount++;
            _totalNGrammsCount += totalNGrammsCount;

            TotalL += l;
            L.Add(l);
        }

        public void AddToDx(Dictionary<string, int> analysisResult, int totalNGrammsCount)
        {
            foreach (var kvp in analysisResult)
            {
                if (Statistics.ContainsKey(kvp.Key))
                {
                    var statistics = Statistics[kvp.Key];
                    var value = kvp.Value / (double)totalNGrammsCount;
                    statistics.DX += (value - statistics.MX) * (value - statistics.MX);
                }
            }
        }

        public void CalculateMXs()
        {
            CalculateW();

            var nGrammCount = _totalNGrammsCount / (double)_addedResultsCount;
            foreach (var nGramm in MXAnalysisResults.Keys)
            {
                if (!Statistics.ContainsKey(nGramm))
                {
                    Statistics.Add(nGramm, new StatsInfo());
                }

                Statistics[nGramm].MX = CalculateMX(nGramm);
            }
        }

        public void CalculateSigmas()
        {
            foreach (var nGramm in MXAnalysisResults.Keys)
            {
                var statistics = Statistics[nGramm];
                statistics.DX = CalculateDX(nGramm, statistics.MX);
                statistics.Sigma = Math.Sqrt(statistics.DX);
            }
        }

        private void CalculateW()
        {
            foreach (var l in L)
            {
                W.Add(l / (double)TotalL);
            }
        }

        private double CalculateMX(string nGramm)
        {
            double result = 0;

            if (MXAnalysisResults.ContainsKey(nGramm))
            {
                result = MXAnalysisResults[nGramm];
            }

            result /= TotalL;

            return result;
        }

        private double CalculateDX(string nGramm, double MX)
        {
            double result = 0;

            if (Statistics.ContainsKey(nGramm))
            {
                result = Statistics[nGramm].DX;
            }

            result /= _addedResultsCount - 1;

            return result;
        }

        private int CountCharactersCount(char[] text)
        {
            return text.Length;
        }
    }
}

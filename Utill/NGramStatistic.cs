using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextsBase.Utill
{
    public class NGramStatistic
    {
        private string[] _files;
        private int _nGramSize;
        private bool _ignoreCase;
        private bool _ignoreSpaces;

        // Абсолютна статистика
        public ConcurrentDictionary<string, int> totalAbsoluteStatistic;
        public ConcurrentDictionary<string, ConcurrentDictionary<string, int>> filesAbsoluteStatistic;

        // Відносна статистика
        public ConcurrentDictionary<string, ConcurrentDictionary<string, double>> filesRelativeStatistic;
        public ConcurrentDictionary<string, double> totalRelativeStatistic;

        // Вагові коефіцієнти текстів
        public ConcurrentDictionary<string, double> fileWeightCoefficients;

        // Середнє значення n-грам
        public ConcurrentDictionary<string, double> MX { get; private set; }

        // Стандартне відхилення n-грам
        public ConcurrentDictionary<string, double> Sigma { get; private set; }

        public NGramStatistic(string[] files, int nGramSize, bool ignoreCase, bool ignoreSpaces)
        {
            _files = files;
            _nGramSize = nGramSize;
            _ignoreCase = ignoreCase;
            _ignoreSpaces = ignoreSpaces;

            totalAbsoluteStatistic = new ConcurrentDictionary<string, int>();
            filesAbsoluteStatistic = new ConcurrentDictionary<string, ConcurrentDictionary<string, int>>();
            totalRelativeStatistic = new ConcurrentDictionary<string, double>();
            filesRelativeStatistic = new ConcurrentDictionary<string, ConcurrentDictionary<string, double>>();
            fileWeightCoefficients = new ConcurrentDictionary<string, double>();
            MX = new ConcurrentDictionary<string, double>();
            Sigma = new ConcurrentDictionary<string, double>();
        }

        public void ProcessFile()
        {
            Parallel.ForEach(_files, file =>
            {
                var fileContent = File.ReadAllText(file);
                var fileNGrams = GetNGrams(fileContent, _nGramSize, _ignoreSpaces, _ignoreCase);

                var fileStatistic = new ConcurrentDictionary<string, int>();

                foreach (var nGram in fileNGrams)
                {
                    totalAbsoluteStatistic.AddOrUpdate(nGram, 1, (key, value) => value + 1);
                    fileStatistic.AddOrUpdate(nGram, 1, (key, value) => value + 1);
                }

                filesAbsoluteStatistic.TryAdd(Path.GetFileName(file), fileStatistic);
            });
        }

        public void CalculateRelativeStatistic()
        {
            Parallel.ForEach(_files, file =>
            {
                string fileName = Path.GetFileName(file);
                var fileAbsoluteNGram = filesAbsoluteStatistic[fileName];
                var fileRelevantStatistic = new ConcurrentDictionary<string, double>();
                double nGramCount = fileAbsoluteNGram.Sum(x => x.Value);

                foreach (var nGram in fileAbsoluteNGram)
                {
                    fileRelevantStatistic[nGram.Key] = nGram.Value / nGramCount;
                }

                filesRelativeStatistic.TryAdd(fileName, fileRelevantStatistic);
            });

            // Після обчислення відносної статистики, обчислюємо середнє значення
            CalculateWeightedAverage();
            // Після обчислення середнього значення, обчислюємо стандартне відхилення
            CalculateSigma();
        }

        public void CalculateWeightCoefficients()
        {
            double totalLength = _files.Sum(file => File.ReadAllText(file).Length);

            foreach (var file in _files)
            {
                double fileLength = File.ReadAllText(file).Length;
                fileWeightCoefficients.TryAdd(Path.GetFileName(file), fileLength / totalLength);
            }
        }

        public void CalculateWeightedAverage()
        {
            // Обчислюємо вагові коефіцієнти
            CalculateWeightCoefficients();

            // Для зберігання проміжних результатів
            var intermediateSum = new ConcurrentDictionary<string, double>();

            // Обчислюємо середнє значення з урахуванням вагових коефіцієнтів
            foreach (var file in _files)
            {
                string fileName = Path.GetFileName(file);
                var fileRelativeStats = filesRelativeStatistic[fileName];
                double fileWeight = fileWeightCoefficients[fileName];

                foreach (var nGram in fileRelativeStats)
                {
                    intermediateSum.AddOrUpdate(nGram.Key, nGram.Value * fileWeight, (key, value) => value + nGram.Value * fileWeight);
                }
            }

            foreach (var entry in intermediateSum)
            {
                MX.TryAdd(entry.Key, entry.Value);
            }
        }

        public void CalculateSigma()
        {
            // Для зберігання проміжних результатів
            var intermediateSum = new ConcurrentDictionary<string, double>();

            // Обчислюємо стандартне відхилення з урахуванням вагових коефіцієнтів
            foreach (var file in _files)
            {
                string fileName = Path.GetFileName(file);
                var fileRelativeStats = filesRelativeStatistic[fileName];
                double fileWeight = fileWeightCoefficients[fileName];

                foreach (var nGram in fileRelativeStats)
                {
                    double mean = MX[nGram.Key];
                    double difference = nGram.Value - mean;
                    double weightedDifferenceSquared = fileWeight * difference * difference;

                    intermediateSum.AddOrUpdate(nGram.Key, weightedDifferenceSquared, (key, value) => value + weightedDifferenceSquared);
                }
            }

            foreach (var entry in intermediateSum)
            {
                Sigma.TryAdd(entry.Key, Math.Sqrt(entry.Value));
            }
        }

        static IEnumerable<string> GetNGrams(string text, int n, bool ignoreSpaces, bool ignoreCase)
        {
            var nGrams = new List<string>();

            if (ignoreCase)
            {
                text = text.ToLower();
            }

            var elements = ignoreSpaces
                ? Regex.Replace(text, @"\s+", "").ToCharArray().Select(c => c.ToString()).ToArray()
                : Regex.Split(text, @"\W+").Where(token => !string.IsNullOrEmpty(token)).ToArray();

            for (int i = 0; i <= elements.Length - n; i++)
            {
                var nGram = ignoreSpaces
                    ? string.Join("", elements.Skip(i).Take(n))
                    : string.Join(" ", elements.Skip(i).Take(n));
                nGrams.Add(nGram);
            }

            return nGrams;
        }
    }
}
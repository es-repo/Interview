using System;
using System.IO;
using System.Diagnostics;

namespace Terrasoft
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = args.Length > 0 ? args[0] : "text.txt";

            Stopwatch sw = new Stopwatch();
            Console.WriteLine($"Analysing file {filePath}...");
            sw.Start();
            Metric[] metrics = AnalyseText(filePath);
            sw.Stop();
            Console.WriteLine($"Finished in {sw.Elapsed}.");
            Console.WriteLine();
            Console.WriteLine("Results:");
            foreach (var m in metrics)
            {
                Console.WriteLine(" - " + m);
            }
        }

        private static Metric[] AnalyseText(string filePath)
        {
            Tokeniser tokeniser = new Tokeniser(
                new CharacterTokenBuilder(),
                new WordTokenBuilder(),
                new SentenceTokenBuilder(),
                new NumberTokenBuilder());

            TextAnalyser textAnalyser = new TextAnalyser(tokeniser);

            Metric[] metrics = new Metric[] {
                new MostFrequentCharacterMetric(),
                new WordsCountMetric(),
                new ExclamationSentenceCountMetric(),
                new NumbersSumMetric(),
                new AverageNumberMetric()
            };
            using(FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using(StreamReader sr = new StreamReader(fs))
            {
                textAnalyser.Analyse(sr, metrics);
            }

            return metrics;
        }
    }
}
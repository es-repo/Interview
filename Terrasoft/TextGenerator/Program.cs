using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Terrasoft
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = args.Length > 0 ? args[0] : "text.txt";
            long maxSize = args.Length > 1 ? long.Parse(args[1]) : 1000000;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            GenerateTextAndWriteToFile(filePath, maxSize);
            sw.Stop();
            Console.WriteLine($"Done in {sw.Elapsed}");
        }

        private static void GenerateTextAndWriteToFile(string filePath, long maxSize, int pageWidth = 100)
        {
            TextGenerator textGenerator = new TextGenerator();
            using(FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using(StreamWriter sw = new StreamWriter(fs))
            {
                long size = 0;
                bool end = false;
                foreach (string textChunk in textGenerator.Generate())
                {
                    string textChunkToWrite = textChunk;
                    if (size + textChunk.Length > maxSize)
                    {
                        int limit = (int) (maxSize - size);
                        textChunkToWrite = textChunk.Substring(0, limit);
                        end = true;
                    }
                    size += textChunkToWrite.Length;
                    
                    sw.Write(textChunkToWrite);

                    if (end)
                        break;
                }
            }
        }
    }
}
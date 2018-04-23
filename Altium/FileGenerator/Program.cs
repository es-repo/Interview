using System;
using System.Diagnostics;
using System.IO;

namespace Altium
{
  class Program
  {
    static void Main(string[] args)
    {
      string filePath = args.Length > 0 ? args[0] : "table.txt";
      long maxSize = args.Length > 1 ? long.Parse(args[1]) : 1024*1024*10;
      Stopwatch sw = new Stopwatch();
      Console.WriteLine($"Generating file: {filePath} ...");
      sw.Start();
      GenerateFile(filePath, maxSize);
      sw.Stop();
      Console.WriteLine($"Done in {sw.Elapsed}");
    }

    private static void GenerateFile(string filePath, long maxSize)
    {
      RecordGenerator stringGenerator = new RecordGenerator();
      using(FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
      using(StreamWriter sw = new StreamWriter(fs))
      {
        foreach (string s in stringGenerator.Generate(maxSize))
          sw.WriteLine(s);
      }
    }
  }
}
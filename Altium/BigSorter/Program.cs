using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Altium.BigSorter
{
  class Program
  {
    static void Main(string[] args)
    {
      string inputPath = args.Length > 0 ? args[0] : "table.txt";
      string outputPath = args.Length > 1 ? args[1] : "sorted-table.txt";
      long bufferSize = args.Length > 2 ? long.Parse(args[2]) : 1024 * 1024 * 1024;

      Stopwatch sw = new Stopwatch();
      Console.WriteLine($"Sorting table file: {inputPath} ...");
      sw.Start();
      SortTable(inputPath, outputPath, bufferSize);
      sw.Stop();
      Console.WriteLine($"Finished in {sw.Elapsed}.");
    }

    private static void SortTable(string inputPath, string outputPath, long bufferSize)
    {
      string tempDir = "~Temp";
      using(FileStream input = new FileStream(inputPath, FileMode.Open))
      using(FileStream output = new FileStream(outputPath, FileMode.Create))
      using(FileTempStreams tempStreams = new FileTempStreams(tempDir))
      {
        BigArray<byte> buffer = new BigArray<byte>(bufferSize);
        BigTableSorter bigTableSorter = new BigTableSorter(buffer, new RecordParser(), new RecordBytesConverter());
        bigTableSorter.Sort(input, new int[] { 1, 0 }, output, tempStreams);
      }
      if (Directory.Exists(tempDir))
        Directory.Delete(tempDir, true);
    }
  }
}
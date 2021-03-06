﻿using System;
using System.Diagnostics;
using System.IO;

namespace Altium.BigSorter
{
  class Program
  {
    static void Main(string[] args)
    {
      string inputPath = args.Length > 0 ? args[0] : @"1gb.txt";
      string outputPath = GetOutputPath(inputPath);
      long bufferSize = args.Length > 1 ? long.Parse(args[1]) : 1024L * 1024 * 1024;

      Stopwatch sw = new Stopwatch();
      Console.WriteLine($"Sorting file: {inputPath} ...");
      sw.Start();
      SortTable(inputPath, outputPath, bufferSize);
      sw.Stop();
      Console.WriteLine($"Finished in {sw.Elapsed}.");
    }

    private static void SortTable(string inputPath, string outputPath, long bufferSize)
    {
      string tempDir = GetTempDirPath(inputPath);
      using(FileStream input = new FileStream(inputPath, FileMode.Open))
      using(FileStream output = new FileStream(outputPath, FileMode.Create))
      using(FileTempStreams tempStreams = new FileTempStreams(tempDir))
      {
        BigTableSorter bigTableSorter = new BigTableSorter(tempStreams, bufferSize);
        bigTableSorter.Sort(input, new int[] { 1, 0 }, output);
      }
      if (Directory.Exists(tempDir))
        Directory.Delete(tempDir, true);
    }

    private static string GetOutputPath(string inputPath)
    {
      return Path.Combine(Path.GetDirectoryName(inputPath),
        Path.GetFileNameWithoutExtension(inputPath) + "-sorted" +
        Path.GetExtension(inputPath));
    }

    private static string GetTempDirPath(string inputPath)
    {
      return Path.Combine(Path.GetDirectoryName(inputPath), "~Temp");
    }
  }
}
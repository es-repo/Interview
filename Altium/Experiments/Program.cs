using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Altium;
using Altium.BigSorter;

namespace Experiments
{
  partial class Program
  {
    static void Main(string[] args)
    {
      string file = "1gb.txt";

      //Profile(() => { ReadFileByBytes(file); }, $"Read {file} by bytes");
      //Profile(() => { ReadFileByLines(file); }, $"Read {file} by lines");
      // Profile(() => { ReadFileByBuffers(file); }, $"Read {file} by buffers");
      // Profile(() => { ReadFileInOneBuffer(file); }, $"Read {file} in one buffer");
      //Profile(() => { ReadLinesAndAddToList(file); }, $"Read {file} by lines and add to list");
      //Profile(() => { ReadLinesAndBinaryInsert(file); }, $"Read {file} by lines and binary insert in list");
      //Profile(() => { ReadIndexAndSortLines(file); }, $"Read {file}, index and sort lines");
      //Profile(() => { ReadIndexAndSortLinesUnsafe(file); }, $"Read {file}, index and sort lines unsafe");
      //Profile(() => { Read_Index_ParallelSort_Lines_Unsafe(file); }, $"Read {file}, index and parallel sort lines unsafe");

      //Profile(() => { Read_Index_SortUnsafe_Write_Lines(file); }, $"Read {file}, index, sort-unsafe, write lines");
      //Profile(() => { Read_Index_Sort_Write_Lines(file); }, $"Read {file}, index, sort, write lines");

      //Profile(() => { ReadBlocks(file); }, $"Read blocks from {file}");
      //Profile(() => { Read_Index_Write_Lines(file); }, $"Read {file}, index, write lines");

      //Profile(() => { ReadBlocks_Write_Sort_Blocks(file, false); }, $"Read and write blocks from {file}");
      //Profile(() => { ReadBlocks_Write_Sort_Blocks(file, true); }, $"Read, write, sort blocks from {file}");
      
      //Profile(() => { ReadRecords_Then_WriteRecordsBuffered(file); }, $"Read records and writes them buffered {file}");
    }

    static void ReadRecords_Then_WriteRecordsBuffered(string file)
    {
      string outputFile = GetDerivedFileName(file, "buffered-output-test");
      using(var input = new FileStream(file, FileMode.Open))
      using(var output = new FileStream(outputFile, FileMode.Create))
      {
        byte[] buffer = new byte[200000000];

        ArrayView<byte> av = new ArrayView<byte>(buffer, 0, buffer.Length / 2);
        RecordsReader rr = new RecordsReader(av, null, input);

        ArrayView<byte> av2 = new ArrayView<byte>(buffer, buffer.Length / 2);
        using(BufferedRecordsWriter rw = new BufferedRecordsWriter(av2, output))
        {
          foreach (var r in rr.ReadRecords())
          {
            rw.WriteRecord(r);
          }
        }
      }
    }

    static void ReadBlocks_Write_Sort_Blocks(string file, bool sort)
    {
      byte[] buffer = new byte[100000000];
      ArrayView<byte> bufferView = new ArrayView<byte>(buffer, 0);
      using(FileStream fs = new FileStream(file, FileMode.Open))
      {
        RecordsReader r = new RecordsReader(bufferView, (bv, f) => new IntStringRecordComparer(bv, f), fs);
        int blocksCount = 0;
        int totalRecords = 0;
        foreach (RecordsBuffer block in r.ReadBlocks())
        {
          blocksCount++;
          totalRecords += block.RecordsInfo.Count;
          Console.WriteLine($"Block {blocksCount}: {block.RecordsInfo.Count} records");
          if (sort) block.Sort(0);
          WriteBlock(block, blocksCount, file);
        }
        Console.WriteLine($"Total blocks {blocksCount}");
        Console.WriteLine($"Total records {totalRecords}");
      }
    }

    static void WriteBlock(RecordsBuffer block, int blockNumber, string file)
    {
      string f = GetBlockFileName(file, blockNumber);
      using(var fs = new FileStream(f, FileMode.Create))
      {
        new RecordsWriter(fs).Write(block);
      }
      Console.WriteLine($"Block {f} has been written");
    }

    static string GetBlockFileName(string file, int block)
    {
      string dir = Path.Combine(Path.GetDirectoryName(file), "blocks");
      Directory.CreateDirectory(dir);
      return Path.Combine(dir,
        Path.GetFileNameWithoutExtension(file) + ".block-" + block + ".txt");
    }

    static void ReadFileByBytes(string file)
    {
      using(var fs = new FileStream(file, FileMode.Open))
      {
        int b = -1;
        long c = 0;
        byte bb;
        while ((b = fs.ReadByte()) != -1)
        {
          c++;
          bb = (byte) b;
          c += bb;
        }
        Console.WriteLine($"{c} bytes");
      }
    }

    static void ReadFileByLines(string file)
    {
      using(var fs = new FileStream(file, FileMode.Open))
      using(var sr = new StreamReader(fs))
      {
        string s;
        long c = 0;
        while ((s = sr.ReadLine()) != null)
        {
          c++;
        }
        Console.WriteLine($"{c} lines");
      }
    }

    static void ReadFileByBuffers(string file)
    {
      using(var fs = new FileStream(file, FileMode.Open))
      {
        int b = -1;
        long c = 0;
        byte[] buffer = new byte[4096];
        while ((b = fs.Read(buffer, 0, buffer.Length)) != 0)
        {
          c++;
        }
        Console.WriteLine($"{c} buffers of {buffer.Length}");
      }
    }

    static void ReadFileInOneBuffer(string file)
    {
      //using(var fs = new FileStream(file, FileMode.Open))
      //{
      //byte[] buffer = new byte[1058176409];
      //fs.Read(buffer, 0, buffer.Length);
      //}
      File.ReadAllBytes(file);
    }

    static void ReadLinesAndSort(string file)
    {
      using(var fs = new FileStream(file, FileMode.Open))
      using(var sr = new StreamReader(fs))
      {
        string s;
        List<string> l = new List<string>();
        while ((s = sr.ReadLine()) != null)
        {
          l.Add(s);
        }
        l.Sort();
      }
    }

    static void ReadLinesAndBinaryInsert(string file)
    {
      using(var fs = new FileStream(file, FileMode.Open))
      using(var sr = new StreamReader(fs))
      {
        string s;
        List<string> l = new List<string>();
        while ((s = sr.ReadLine()) != null)
        {
          //int i = l.BinarySearch(s);
          //if (i < 0) i = ~i;
          //l.Insert(i, s);
          l.Insert(0, s);
        }
      }
    }

    static void ReadLinesAndAddToList(string file)
    {
      using(var fs = new FileStream(file, FileMode.Open))
      using(var sr = new StreamReader(fs))
      {
        string s;
        List<string> l = new List<string>();
        while ((s = sr.ReadLine()) != null)
        {
          l.Add(s);
        }
      }
    }

    static Lines ReadIndexAndSortLines(string file)
    {
      Lines lines = ReadAndIndexLines(file);
      LineComparer comparer = new LineComparer(lines.Data);
      lines.Index.Sort(comparer);
      return lines;
    }

    static void Read_Index_Sort_Write_Lines(string file)
    {
      Lines lines = ReadAndIndexLines(file);
      LineComparer comparer = new LineComparer(lines.Data);
      lines.Index.Sort(comparer);
      string outputFile = GetSortedFileName(file);
      WriteLines(lines, outputFile);
    }

    static Lines ReadIndexAndSortLinesUnsafe(string file)
    {
      Lines lines = ReadAndIndexLines(file);
      LineComparerUnsafe comparer = new LineComparerUnsafe(lines.Data);
      lines.Index.Sort(comparer);
      return lines;
    }

    static Lines Read_Index_ParallelSort_Lines_Unsafe(string file)
    {
      Lines lines = ReadAndIndexLines(file);
      LineComparerUnsafe comparer = new LineComparerUnsafe(lines.Data);
      ParallelSort.QuicksortParallel(lines.Index, comparer);
      return lines;
    }

    static Lines ReadAndIndexLines(string file)
    {
      Lines lines = new Lines();
      byte[] data = File.ReadAllBytes(file);
      lines.Data = data;
      int start = 0;
      int len = 0;
      for (int i = 0; i < lines.Data.Length; i++)
      {
        if (data[i] == 0x0A)
        {
          lines.Index.Add(new Line(start, len));
          start += len + 1;
          len = 0;
        }
        else len++;
      }
      Console.WriteLine($"Lines count {lines.Index.Count}");
      return lines;
    }

    static void Read_Index_SortUnsafe_Write_Lines(string file)
    {
      Lines lines = ReadIndexAndSortLinesUnsafe(file);
      string outputFile = GetSortedFileName(file, "-unsafe");
      WriteLines(lines, outputFile);
    }

    static void Read_Index_Write_Lines(string file)
    {
      Lines lines = ReadAndIndexLines(file);
      string outputFile = GetSortedFileName(file);
      WriteLines(lines, outputFile);
    }

    static void WriteLines(Lines lines, string file)
    {
      using(var fs = new FileStream(file, FileMode.Create))
      {
        for (int i = 0; i < lines.Index.Count; i++)
        {
          Line line = lines.Index[i];
          fs.Write(lines.Data, line.Pos, line.Len + 1);
        }
      }
    }

    static string GetSortedFileName(string file, string suf = "")
    {
      return GetDerivedFileName(file, "sorted" + suf);
    }

    static string GetDerivedFileName(string file, string suf)
    {
      return Path.Combine(Path.GetDirectoryName(file),
        Path.GetFileNameWithoutExtension(file) + "-" + suf + Path.GetExtension(file));
    }

    static void Profile(Action action, string description)
    {
      Console.WriteLine("Start " + description + " ...");
      Stopwatch sw = new Stopwatch();
      sw.Start();
      action();
      sw.Stop();
      Console.WriteLine("Finished in " + sw.Elapsed);
      Console.WriteLine();
    }
  }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Altium.BigSorter.Tests
{
  public class MemoryStreamWithOnDisposeHandler : MemoryStream
  {
    private readonly Action<MemoryStreamWithOnDisposeHandler> _onDispose;

    public MemoryStreamWithOnDisposeHandler(Action<MemoryStreamWithOnDisposeHandler> onDispose)
    {
      _onDispose = onDispose;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        _onDispose(this);
      base.Dispose(disposing);
    }
  }

  public class TempStreams : ITempStreams
  {
    private readonly ConcurrentDictionary<int, byte[]> _blocks = new ConcurrentDictionary<int, byte[]>();

    public List<Dictionary<int, byte[]>> Blocks { get; } = new List<Dictionary<int, byte[]>>();
    public byte[] TempOutput { get; private set; }

    public void ClearBlocks()
    {
      Dictionary<int, byte[]> copy = new Dictionary<int, byte[]>(_blocks);
      Blocks.Add(copy);
      _blocks.Clear();
    }

    public Stream CreateBlockStream(int blockIndex)
    {
      if (_blocks.ContainsKey(blockIndex))
        throw new InvalidOperationException($"Block {blockIndex} already was created");

      _blocks.TryAdd(blockIndex, null);
      return new MemoryStreamWithOnDisposeHandler(ms =>
      {
        _blocks[blockIndex] = ms.ToArray();
      });
    }

    public Stream OpenBlockStream(int blockIndex)
    {
      return new MemoryStream(_blocks[blockIndex]);
    }

    public Stream CreateTempOutputStream()
    {
      return new MemoryStreamWithOnDisposeHandler(ms =>
      {
        ms.Position = 0;
        TempOutput = ms.ToArray();
      });
    }

    public void Dispose() { }
  }

  public class BigTableSorterTests
  {
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    public void TestSortMultipleBlocks(int workersCount)
    {
      string[] records = new string[]
      {
        "2. e",
        "1. c",
        "2. a",
        "1. a",
        "1. d",

        "1. b",
        "3. a",
        "2. e",
        "1. c",
        "2. a",

        "1. a",
        "1. d",
        "1. b",
        "3. a",
        "2. e",

        "1. c",
        "2. a",
        "1. a",
        "1. d",
      };

      int sizeOfRecord = System.Runtime.InteropServices.Marshal.SizeOf<Record>() + 2;
      int bufSize = sizeOfRecord * 5 * workersCount;
      TempStreams tempStreams = new TempStreams();
      BigTableSorter tableSorter = new BigTableSorter(tempStreams, bufSize, workersCount);

      MemoryStream input = StringsToStream(records);
      MemoryStream output = new MemoryStream();
      tableSorter.Sort(input, 1, output);

      Assert.Equal(4, tempStreams.Blocks[0].Count);

      string[][] expectedBlockRecords = new string[][]
      {
        new string[]
        {
        "2. a",
        "1. a",
        "1. c",
        "1. d",
        "2. e",
        },

        new string[]
        {
        "3. a",
        "2. a",
        "1. b",
        "1. c",
        "2. e",
        },

        new string[]
        {
        "1. a",
        "3. a",
        "1. b",
        "1. d",
        "2. e",
        },

        new string[]
        {
        "2. a",
        "1. a",
        "1. c",
        "1. d",
        }
      };

      for (int i = 0; i < expectedBlockRecords.Length; i++)
      {
        Assert.Equal(expectedBlockRecords[i], BytesToStrings(tempStreams.Blocks[0][i]));
      }
      string[] expectedRecords = new string[]
      {
        "2. a",
        "1. a",
        "3. a",
        "2. a",
        "2. a",

        "1. a",
        "1. a",
        "3. a",
        "1. b",
        "1. b",

        "1. c",
        "1. c",
        "1. c",
        "1. d",
        "1. d",

        "1. d",
        "2. e",
        "2. e",
        "2. e",
      };
      string expectedRecordsStr = string.Join("\r\n", expectedRecords) + "\r\n";
      output.Position = 0;
      string result = new StreamReader(output).ReadToEnd();
      Assert.Equal(expectedRecordsStr, result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    public void TestSortSingleBlock(int workersCount)
    {
      string[] records = new string[]
      {
        "2. e",
        "1. c",
        "2. a",
        "1. a",
        "1. d",
        "1. b",
        "3. a",
        "2. e",
        "1. c",
        "2. a",
        "1. a",
        "1. d",
        "1. b",
        "3. a",
        "2. e",
        "1. c",
        "2. a",
        "1. a",
        "1. d",
      };

      int sizeOfRecord = System.Runtime.InteropServices.Marshal.SizeOf<Record>() + 2;
      int bufSize = sizeOfRecord * records.Length * workersCount;
      TempStreams tempStreams = new TempStreams();
      BigTableSorter tableSorter = new BigTableSorter(tempStreams, bufSize, workersCount);

      MemoryStream input = StringsToStream(records);
      MemoryStream output = new MemoryStream();

      tableSorter.Sort(input, 1, output);

      Assert.Empty(tempStreams.Blocks);

      string[] expectedRecords = new string[]
      {
        "2. a",
        "2. a",
        "1. a",
        "2. a",
        "3. a",
        "1. a",
        "1. a",
        "3. a",
        "1. b",
        "1. b",
        "1. c",
        "1. c",
        "1. c",
        "1. d",
        "1. d",
        "1. d",
        "2. e",
        "2. e",
        "2. e",
      };
      string expectedRecordsStr = string.Join("\r\n", expectedRecords) + "\r\n";

      output.Position = 0;
      string result = new StreamReader(output).ReadToEnd();
      Assert.Equal(expectedRecordsStr, result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    public void TestSortByMultipleFields(int workersCount)
    {
      string[] records = new string[]
      {
        "2. e",
        "1. c",
        "2. a",
        "1. a",
        "2. d",

        "3. b",
        "3. a",
        "2. e",
        "3. c",
        "2. a",

        "1. a",
        "2. d",
        "1. b",
        "3. a",
        "1. e",

        "1. c",
        "2. a",
        "1. a",
        "3. d",

        "5. a",
        "7. a",
        "9. a",
        "4. a",
        "6. a",
      };

      int sizeOfRecord = System.Runtime.InteropServices.Marshal.SizeOf<Record>() + 2;
      int bufSize = sizeOfRecord * 6 * workersCount;
      TempStreams tempStreams = new TempStreams();
      BigTableSorter tableSorter = new BigTableSorter(tempStreams, bufSize, workersCount);

      MemoryStream input = StringsToStream(records);
      MemoryStream output = new MemoryStream();
      tableSorter.Sort(input, new int[] { 1, 0 }, output);

      string[] expectedRecords = new string[]
      {
        "1. a",
        "1. a",
        "1. a",
        "2. a",
        "2. a",
        "2. a",
        "3. a",
        "3. a",
        "4. a",
        "5. a",
        "6. a",
        "7. a",
        "9. a",

        "1. b",
        "3. b",

        "1. c",
        "1. c",
        "3. c",

        "2. d",
        "2. d",
        "3. d",

        "1. e",
        "2. e",
        "2. e",
      };
      string expectedRecordsStr = string.Join("\r\n", expectedRecords) + "\r\n";

      output.Position = 0;
      string result = new StreamReader(output).ReadToEnd();
      Assert.Equal(expectedRecordsStr, result);
    }

    private static MemoryStream StringsToStream(string[] ss)
    {
      MemoryStream ms = new MemoryStream();
      StreamWriter sw = new StreamWriter(ms);
      foreach (string s in ss)
      {
        sw.Write(s);
        sw.Write("\r\n");
      }
      sw.Flush();
      ms.Position = 0;
      return ms;
    }

    private static List<string> BytesToStrings(byte[] bytes)
    {
      using(MemoryStream ms = new MemoryStream(bytes))
      using(StreamReader sr = new StreamReader(ms))
      {
        List<string> list = new List<string>();
        string s = null;
        while ((s = sr.ReadLine()) != null)
        {
          list.Add(s);
        }
        return list;
      }
    }
  }
}
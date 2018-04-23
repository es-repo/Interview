using System;
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
    private readonly List<byte[]> _blocks = new List<byte[]>();

    public List<List<byte[]>> Blocks { get; } = new List<List<byte[]>>();
    public byte[] TempOutput { get; private set; }

    public void ClearBlocks()
    {
      List<byte[]> copy = new List<byte[]>(_blocks);
      Blocks.Add(copy);
      _blocks.Clear();
    }

    public Stream CreateBlockStream(int blockIndex)
    {
      if (blockIndex > _blocks.Count - 1)
      {
        _blocks.Add(null);
        return new MemoryStreamWithOnDisposeHandler(ms =>
        {
          _blocks[blockIndex] = ms.ToArray();
        });
      }

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

    public void Dispose()
    { }
  }

  public class BigTableSorterTests
  {
    [Fact]
    public void TestSortMultipleBlocks()
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

      BigTableSorter tableSorter = new BigTableSorter(6 * 5, new RecordParser());

      MemoryStream input = StringsToStream(records);
      MemoryStream output = new MemoryStream();

      TempStreams tempStreams = new TempStreams();
      tableSorter.Sort(input, 1, output, tempStreams);

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
        "3. a",
        "1. a",
        "2. a",
        "1. a",

        "3. a",
        "2. a",
        "1. a",
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
      string expectedRecordsStr = string.Join(Environment.NewLine, expectedRecords) + Environment.NewLine;
      output.Position = 0;
      string result = new StreamReader(output).ReadToEnd();
      Assert.Equal(expectedRecordsStr, result);
    }

    [Fact]
    public void TestSortSingleBlock()
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

      BigTableSorter tableSorter = new BigTableSorter(6 * records.Length, new RecordParser());

      MemoryStream input = StringsToStream(records);
      MemoryStream output = new MemoryStream();

      TempStreams tempStreams = new TempStreams();
      tableSorter.Sort(input, 1, output, tempStreams);

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
      string expectedRecordsStr = string.Join(Environment.NewLine, expectedRecords) + Environment.NewLine;

      output.Position = 0;
      string result = new StreamReader(output).ReadToEnd();
      Assert.Equal(expectedRecordsStr, result);
    }

    [Fact]
    public void TestSortByMultipleFields()
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

      BigTableSorter tableSorter = new BigTableSorter(6 * 6, new RecordParser());

      MemoryStream input = StringsToStream(records);
      MemoryStream output = new MemoryStream();

      TempStreams tempStreams = new TempStreams();
      tableSorter.Sort(input, new int[] { 1, 0 }, output, tempStreams);

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
      string expectedRecordsStr = string.Join(Environment.NewLine, expectedRecords) + Environment.NewLine;

      output.Position = 0;
      string result = new StreamReader(output).ReadToEnd();
      Assert.Equal(expectedRecordsStr, result);
    }

    private static MemoryStream StringsToStream(string[] ss)
    {
      MemoryStream ms = new MemoryStream();
      StreamWriter sw = new StreamWriter(ms);
      foreach (string s in ss)
        sw.WriteLine(s);
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
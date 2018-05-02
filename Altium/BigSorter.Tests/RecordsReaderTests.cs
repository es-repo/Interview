using System;
using System.IO;
using Xunit;

namespace Altium.BigSorter.Tests
{
  public class RecordsReaderTests
  {
    [Fact]
    public void TestReadBlocks()
    {
      string[] records = new []
      {
        "1. a",
        "2. b",
        "3. c"
      };

      MemoryStream stream = LinesToStream(records);
      int sizeOfRecord = System.Runtime.InteropServices.Marshal.SizeOf<Record>() + 2;
      RecordsReader recordsReader = new RecordsReader(new StreamReader(stream), sizeOfRecord * 2 + 1);

      Record[][] expectedRecordsPerBlock = new Record[][]
      {
        new Record[]
        {
        new Record(1, "a"),
        new Record(2, "b")
        },

        new Record[]
        {
        new Record(3, "c")
        }
      };

      int i = 0;
      foreach (RecordsBuffer rb in recordsReader.ReadBlocks())
      {
        Assert.Equal(expectedRecordsPerBlock[i].Length, rb.Records.Count);

        for (int j = 0; j < rb.Records.Count; j++)
        {
          Assert.Equal(expectedRecordsPerBlock[i][j], rb.Records[j]);
        }
        i++;
      }
    }

    [Fact]
    public void TestReadBlocksWhileSameFieldValue()
    {
      string[] records = new []
      {
        "1. a",
        "2. a",
        "3. a",
        "1. b",
        "2. b",
        "3. b",
        "4. c"
      };

      MemoryStream stream = LinesToStream(records);
      int sizeOfRecord = System.Runtime.InteropServices.Marshal.SizeOf<Record>() + 2;
      RecordsReader recordsReader = new RecordsReader(new StreamReader(stream), sizeOfRecord * 2 + 1, 1);

      Record[][][] expectedRecordsPerBlock = new Record[][][]
      {
        new Record[][]
        {
        new Record[]
        {
        new Record(1, "a"),
        new Record(2, "a")
        },

        new Record[]
        {
        new Record(3, "a"),
        }
        },

        new Record[][]
        {
        new Record[]
        {
        new Record(1, "b"),
        new Record(2, "b")
        },

        new Record[]
        {
        new Record(3, "b"),
        }
        },

        new Record[][]
        {
        new Record[]
        {
        new Record(4, "c"),
        }
        },
      };

      int k = 0;
      while (!recordsReader.IsEnd)
      {
        int i = 0;
        foreach (RecordsBuffer rb in recordsReader.ReadBlocks())
        {
          Assert.Equal(expectedRecordsPerBlock[k][i].Length, rb.Records.Count);

          for (int j = 0; j < rb.Records.Count; j++)
          {
            Assert.Equal(expectedRecordsPerBlock[k][i][j], rb.Records[j]);
          }
          i++;
        }
        k++;
      }
    }

    private static MemoryStream LinesToStream(string[] lines)
    {
      MemoryStream ms = new MemoryStream();
      StreamWriter sw = new StreamWriter(ms);
      foreach (string s in lines)
        sw.WriteLine(s);
      sw.Flush();
      ms.Position = 0;
      return ms;
    }
  }
}
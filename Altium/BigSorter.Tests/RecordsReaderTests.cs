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

      MemoryStream stream = StringsToStream(records);

      RecordsReader recordsReader = new RecordsReader(13, new RecordParser(), new StreamReader(stream));

      object[][][] expectedRecordsPerBlock = new object[][][]
      {
        new object[][]
        {
        new object[] { 1, "a" },
        new object[] { 2, "b" }
        },

        new object[][]
        {
        new object[] { 3, "c" },
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
    public void TestReadBlocksWhileEqualToFieldIndex()
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

      MemoryStream stream = StringsToStream(records);

      StreamReader sr = new StreamReader(stream);
      RecordsReader recordsReader = new RecordsReader(12, new RecordParser(), sr, 1);

      object[][][][] expectedRecordsPerBlock = new object[][][][]
      {
        new object[][][]
        {
        new object[][]
        {
        new object[] { 1, "a" },
        new object[] { 2, "a" }
        },

        new object[][]
        {
        new object[] { 3, "a" },
        }
        },

        new object[][][]
        {
        new object[][]
        {
        new object[] { 1, "b" },
        new object[] { 2, "b" }
        },

        new object[][]
        {
        new object[] { 3, "b" },
        }
        },

        new object[][][]
        {
        new object[][]
        {
        new object[] { 4, "c" },
        }
        },
      };

      int k = 0;
      while (!sr.EndOfStream)
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
  }
}
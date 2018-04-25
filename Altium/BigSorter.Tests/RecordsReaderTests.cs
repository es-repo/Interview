using System;
using System.IO;
using Xunit;

namespace Altium.BigSorter.Tests
{
  public class RecordsReaderTests
  {
    [Fact]
    public void TestReadBlocks2()
    {
      string[] records = new []
      {
        "1. a",
        "2. b",
        "3. c"
      };

      MemoryStream stream = LinesToStream(records);
      byte[] buffer = new byte[5 * 2 + 1];
      ArrayView<byte> bufferView = new ArrayView<byte>(buffer, 0);
      RecordsReader2 recordsReader = new RecordsReader2(bufferView, null, stream);

      string[][] expectedRecordsPerBlock = new string[][]
      {
        new string[]
        {
        "1. a",
        "2. b"
        },

        new string[]
        {
        "3. c",
        }
      };

      int i = 0;
      foreach (RecordsBuffer2 block in recordsReader.ReadBlocks())
      {
        Assert.Equal(expectedRecordsPerBlock[i].Length, block.RecordsInfo.Count);

        for (int j = 0; j < block.RecordsInfo.Count; j++)
        {
          Assert.True(RecordsEqual(expectedRecordsPerBlock[i][j], block, j));
        }
        i++;
      }
    }

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

      MemoryStream stream = LinesToStream(records);

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

    private static MemoryStream LineToStream(string l)
    {
      return LinesToStream(new string[] { l });
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

    private static byte[] LineToBytes(string line)
    {
      MemoryStream ms = LineToStream(line);
      ms.Position = 0;
      return ms.ToArray();
    }

    private static bool RecordsEqual(string recordA, RecordsBuffer2 recordBBuffer, int recordBIndex)
    {
      byte[] recordABytes = LineToBytes(recordA);
      RecordInfo recordB = recordBBuffer.RecordsInfo[recordBIndex];
      byte[] recordBBytes = new byte[recordB.Length + 1];
      Buffer.BlockCopy(recordBBuffer.BufferView.Array, recordB.Position, recordBBytes, 0, recordB.Length + 1);
      if (recordABytes.Length != recordABytes.Length)
        return false;

      for(int i = 0; i < recordABytes.Length; i++)
      {
        if (recordABytes[i] != recordABytes[i])
          return false;
      }
      return true;
    }
  }
}
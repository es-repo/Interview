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

      BigArray<byte> buffer = new BigArray<byte>(32);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);

      RecordsReader recordsReader =
        new RecordsReader(new RecordParser(), new RecordBytesConverter(), new StreamReader(stream), av);

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
        Assert.Equal((long) expectedRecordsPerBlock[i].Length, rb.RecordsCount);

        for (long j = 0; j < rb.RecordsCount; j++)
        {
          Assert.Equal(expectedRecordsPerBlock[i][j], rb.GetRecord(j));
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

      BigArray<byte> buffer = new BigArray<byte>(28);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);

      StreamReader sr = new StreamReader(stream);
      RecordsReader recordsReader =
        new RecordsReader(new RecordParser(), new RecordBytesConverter(), sr, av, 1);

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
          Assert.Equal((long) expectedRecordsPerBlock[k][i].Length, rb.RecordsCount);

          for (long j = 0; j < rb.RecordsCount; j++)
          {
            Assert.Equal(expectedRecordsPerBlock[k][i][j], rb.GetRecord(j));
          }
          i++;
        }
        k++;
      }
    }

    [Fact]
    public void TestReadBlocksRaw()
    {
      byte[][] rawRecords = new byte[][]
      {
        new byte[] { 0, 0, 0, 6, 0, 0, 0, 1, 0, (byte) 'a' },
        new byte[] { 0, 0, 0, 6, 0, 0, 0, 2, 0, (byte) 'b' },
        new byte[] { 0, 0, 0, 6, 0, 0, 0, 3, 0, (byte) 'c' }
      };

      MemoryStream stream = BytesToStream(rawRecords);

      BigArray<byte> buffer = new BigArray<byte>(32);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);

      RecordsReader recordsReader =
        new RecordsReader(new RecordParser(), new RecordBytesConverter(), stream, av);

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
      foreach (RecordsBuffer rb in recordsReader.ReadBlocksRaw())
      {
        Assert.Equal((long) expectedRecordsPerBlock[i].Length, rb.RecordsCount);

        for (long j = 0; j < rb.RecordsCount; j++)
        {
          Assert.Equal(expectedRecordsPerBlock[i][j], rb.GetRecord(j));
        }
        i++;
      }
    }

    [Fact]
    public void TestReadRecordBytes()
    {
      byte[][] rawRecords = new byte[][]
      {
        new byte[] { 0, 0, 0, 6, 0, 0, 0, 1, 0, (byte) 'a' },
        new byte[] { 0, 0, 0, 6, 0, 0, 0, 2, 0, (byte) 'b' },
        new byte[] { 0, 0, 0, 6, 0, 0, 0, 3, 0, (byte) 'c' }
      };

      MemoryStream stream = BytesToStream(rawRecords);

      BigArray<byte> buffer = new BigArray<byte>(32);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);

      RecordsReader recordsReader =
        new RecordsReader(new RecordParser(), new RecordBytesConverter(), stream, av);

      byte[][] expectedRecordBytes = new byte[][]
      {
        new byte[] { 0, 0, 0, 1, 0, (byte) 'a' },
        new byte[] { 0, 0, 0, 2, 0, (byte) 'b' },
        new byte[] { 0, 0, 0, 3, 0, (byte) 'c' }
      };

      int i = 0;
      foreach (ArrayView<byte> r in recordsReader.ReadRecordBytes())
      {
        Assert.Equal(expectedRecordBytes[i], r.Enumerate());
        i++;
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

    private static MemoryStream BytesToStream(byte[][] bytes)
    {
      MemoryStream ms = new MemoryStream();
      foreach (byte[] bb in bytes)
        foreach (byte b in bb)
          ms.WriteByte(b);

      ms.Flush();
      ms.Position = 0;
      return ms;
    }
  }
}
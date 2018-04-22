using System.Collections.Generic;
using System.IO;
using Altium.BigSorter;
using Xunit;

namespace Altium.BigSorter.Tests
{
  public class RecordsBufferTests
  {
    private static RecordsBuffer CreateRecordsBuffer(long bufferLen)
    {
      BigArray<byte> buffer = new BigArray<byte>(bufferLen);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);
      return new RecordsBuffer(av, new RecordBytesConverter());
    }

    [Fact]
    public void TestAddRecord()
    {
      object[][] records = new object[][]
      {
        new object[] { 0x1, "a" },
        new object[] { 0x2, "bc" },
        new object[] { 0x4, "def" },
        new object[] { 0x8, "xyz" }
      };

      long recordsBufferLen = 0;
      for (int i = 0; i < records.Length - 1; i++)
      {
        recordsBufferLen += (long) (sizeof(int) + ((string) records[i][1]).Length * sizeof(char) + 8);
      }

      recordsBufferLen += 13;

      BigArray<byte> buffer = new BigArray<byte>(recordsBufferLen);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);
      RecordsBuffer recordsBuffer = new RecordsBuffer(av, new RecordBytesConverter());

      for (int i = 0; i < records.Length - 1; i++)
      {
        bool added = recordsBuffer.AddRecord(records[i]);
        Assert.True(added);
      }

      bool addedLast = recordsBuffer.AddRecord(records[records.Length - 1]);
      Assert.False(addedLast);
      Assert.Equal(3, recordsBuffer.RecordsCount);

      byte[] expectedBytes = new byte[]
      {
        0,
        0,
        0,
        1,
        0,
        (byte)
        'a',
        0,
        0,
        0,
        2,
        0,
        (byte)
        'b',
        0,
        (byte)
        'c',
        0,
        0,
        0,
        4,
        0,
        (byte)
        'd',
        0,
        (byte)
        'e',
        0,
        (byte)
        'f',
        0,
        0,
        0,
        8,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        10,
        0,
        0,
        0,
        14,
        0,
        0,
        0,
        8,
        0,
        0,
        0,
        6,
        0,
        0,
        0,
        6,
        0,
        0,
        0,
        0
      };

      Assert.Equal(expectedBytes, buffer.Enumerate());
    }

    [Fact]
    public void TestAddRecordBytes()
    { 
      BigArray<byte> buffer = new BigArray<byte>(20);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);
      RecordsBuffer recordsBuffer = new RecordsBuffer(av, new RecordBytesConverter());

      BigArray<byte> recordBytesArr = new BigArray<byte>(8);
      recordBytesArr.CopyFrom(new byte[] { 0, 0, 0, 3, 0, (byte)'a', 0, (byte)'b'});
      ArrayView<byte> recordBytes = new ArrayView<byte>(recordBytesArr, 0);
      
      bool added = recordsBuffer.AddRecordBytes(recordBytes);
      Assert.True(added);
      added = recordsBuffer.AddRecordBytes(recordBytes);
      Assert.False(added);
      Assert.Equal(1, recordsBuffer.RecordsCount);

      IEnumerable<object> values = recordsBuffer.GetRecord(0);
      Assert.Equal(new object[] {3, "ab"}, values);
    }

    [Fact]
    public void TestAddRecordBytesFromStream()
    { 
      BigArray<byte> buffer = new BigArray<byte>(36);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);
      RecordsBuffer recordsBuffer = new RecordsBuffer(av, new RecordBytesConverter());

      MemoryStream stream = new MemoryStream(new byte[] { 0, 0, 0, 3, 0, (byte)'a', 0, (byte)'b', 0, 0, 0, 4, 0, (byte)'c', 0, (byte)'d', 0, 0, 0, 5, 0, (byte)'e', 0, (byte)'f'});
      
      bool added = recordsBuffer.AddRecordBytes(stream, 8);
      Assert.True(added);
      added = recordsBuffer.AddRecordBytes(stream, 8);
      Assert.True(added);
      added = recordsBuffer.AddRecordBytes(stream, 8);
      Assert.False(added);
      Assert.Equal(2, recordsBuffer.RecordsCount);

      Assert.Equal(new object[] {3, "ab"}, recordsBuffer.GetRecord(0));
      Assert.Equal(new object[] {4, "cd"}, recordsBuffer.GetRecord(1));
    }

    [Fact]
    public void TestGetRecordBytes()
    {
      object[][] records = new object[][]
      {
        new object[] { 0x1, "a" },
        new object[] { 0x2, "bc" },
        new object[] { 0x8, "xyz" }
      };

      BigArray<byte> buffer = new BigArray<byte>(100);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);
      RecordsBuffer recordsBuffer = new RecordsBuffer(av, new RecordBytesConverter());

      for (int i = 0; i < records.Length; i++)
      {
        recordsBuffer.AddRecord(records[i]);
      }

      byte[][] expectedBytes = new byte[][]
      {
        new byte[]
        {
        0,
        0,
        0,
        1,
        0,
        (byte)
        'a'
        },

        new byte[]
        {
        0,
        0,
        0,
        2,
        0,
        (byte)
        'b',
        0,
        (byte)
        'c'
        },

        new byte[]
        {
        0,
        0,
        0,
        8,
        0,
        (byte)
        'x',
        0,
        (byte)
        'y',
        0,
        (byte)
        'z'
        }
      };

      for (int i = 0; i < records.Length; i++)
      {
        ArrayView<byte> recordBytes = recordsBuffer.GetRecordBytes((long) i);
        Assert.Equal(expectedBytes[i], recordBytes.Enumerate());
      }
    }

    [Fact]
    public void TestGetRecord()
    {
      object[][] records = new object[][]
      {
        new object[] { 0x1, "a" },
        new object[] { 0x2, "bc" },
        new object[] { 0x8, "xyz" }
      };

      BigArray<byte> buffer = new BigArray<byte>(100);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);
      RecordsBuffer recordsBuffer = new RecordsBuffer(av, new RecordBytesConverter());
      for (int i = 0; i < records.Length; i++)
      {
        recordsBuffer.AddRecord(records[i]);
      }

      for (int i = 0; i < records.Length; i++)
      {
        IEnumerable<object> record = recordsBuffer.GetRecord((long) i);
        Assert.Equal(records[i], record);
      }
    }

    [Fact]
    public void TestSwapRecords()
    {
      object[][] records = new object[][]
      {
        new object[] { 0x1, "a" },
        new object[] { 0x2, "bc" },
        new object[] { 0x8, "xyz" }
      };

      BigArray<byte> buffer = new BigArray<byte>(100);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);
      RecordsBuffer recordsBuffer = new RecordsBuffer(av, new RecordBytesConverter());
      for (int i = 0; i < records.Length; i++)
      {
        recordsBuffer.AddRecord(records[i]);
      }

      recordsBuffer.SwapRecords(0, 1);
      Assert.Equal(records[0], recordsBuffer.GetRecord(1));
      Assert.Equal(records[1], recordsBuffer.GetRecord(0));

      recordsBuffer.SwapRecords(0, 2);
      Assert.Equal(records[2], recordsBuffer.GetRecord(0));
      Assert.Equal(records[1], recordsBuffer.GetRecord(2));
    }

    [Fact]
    public void TestSort()
    {
      object[][] records = new object[][]
      {
        new object[] { 1, "a" },
        new object[] { 2, "aa" },
        new object[] { 7, "b" },
        new object[] { 2, "ab" },
        new object[] { 8, "aa" },
        new object[] { 4, "ba" },
        new object[] { 5, "bb" }
      };

      BigArray<byte> buffer = new BigArray<byte>(200);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);
      RecordsBuffer recordsBuffer = new RecordsBuffer(av, new RecordBytesConverter());
      for (int i = 0; i < records.Length; i++)
      {
        recordsBuffer.AddRecord(records[i]);
      }

      recordsBuffer.Sort(1);

      object[][] expectedSortedByStringRecords = new object[][]
      {
        new object[] { 1, "a" },
        new object[] { 8, "aa" },
        new object[] { 2, "aa" },
        new object[] { 2, "ab" },
        new object[] { 7, "b" },
        new object[] { 4, "ba" },
        new object[] { 5, "bb" }
      };

      for (long i = 0; i < recordsBuffer.RecordsCount; i++)
      {
        Assert.Equal(expectedSortedByStringRecords[i], recordsBuffer.GetRecord(i));
      }

      recordsBuffer.Sort(0);

      object[][] expectedSortedByIntRecords = new object[][]
      {
        new object[] { 1, "a" },
        new object[] { 2, "ab" },
        new object[] { 2, "aa" },
        new object[] { 4, "ba" },
        new object[] { 5, "bb" },
        new object[] { 7, "b" },
        new object[] { 8, "aa" }
      };

      for (long i = 0; i < recordsBuffer.RecordsCount; i++)
      {
        Assert.Equal(expectedSortedByIntRecords[i], recordsBuffer.GetRecord(i));
      }
    }
  }
}
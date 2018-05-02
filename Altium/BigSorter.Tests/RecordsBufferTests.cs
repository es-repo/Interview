using System.Collections.Generic;
using System.IO;
using Altium.BigSorter;
using Xunit;

namespace Altium.BigSorter.Tests
{
  public class RecordsBufferTests
  {
    [Fact]
    public void TestAddRecord()
    {
      Record[] records = new Record[]
      {
        new Record ( 0x1, "a" ),
        new Record ( 0x2, "bc" ),
        new Record ( 0x4, "def" ),
        new Record ( 0x8, "xyz" )
      };

      long recordsBufferLen = 0;
      for (int i = 0; i < records.Length - 1; i++)
      {
        recordsBufferLen += records[i].SizeInBytes;
      }

      recordsBufferLen += 2;

      RecordsBuffer recordsBuffer = new RecordsBuffer(recordsBufferLen);

      for (int i = 0; i < records.Length - 1; i++)
      {
        bool added = recordsBuffer.AddRecord(records[i]);
        Assert.True(added);
      }

      bool addedLast = recordsBuffer.AddRecord(records[records.Length - 1]);
      Assert.False(addedLast);
      Assert.Equal(3, recordsBuffer.Records.Count);
    }

    [Fact]
    public void TestSort()
    {
      Record[] records = new Record[]
      {
        new Record ( 1, "a"),
        new Record ( 2, "aa" ),
        new Record ( 7, "b" ),
        new Record ( 2, "ab" ),
        new Record ( 8, "aa" ),
        new Record ( 4, "ba" ),
        new Record ( 5, "bb" )
      };

      RecordsBuffer recordsBuffer = new RecordsBuffer(200);
      for (int i = 0; i < records.Length; i++)
      {
        recordsBuffer.AddRecord(records[i]);
      }

      recordsBuffer.Sort(1);

      Record[] expectedSortedByStringRecords = new Record[]
      {
        new Record ( 1, "a" ),
        new Record ( 2, "aa" ),
        new Record ( 8, "aa" ),
        new Record ( 2, "ab" ),
        new Record ( 7, "b" ),
        new Record ( 4, "ba" ),
        new Record ( 5, "bb" )
      };

      for (int i = 0; i < recordsBuffer.Records.Count; i++)
      {
        Assert.Equal(expectedSortedByStringRecords[i], recordsBuffer.Records[i]);
      }

      recordsBuffer.Sort(0);

      Record[] expectedSortedByIntRecords = new Record[]
      {
        new Record ( 1, "a" ),
        new Record ( 2, "aa" ),
        new Record ( 2, "ab" ),
        new Record ( 4, "ba" ),
        new Record ( 5, "bb" ),
        new Record ( 7, "b" ),
        new Record ( 8, "aa" )
      };

      for (int i = 0; i < recordsBuffer.Records.Count; i++)
      {
        Assert.Equal(expectedSortedByIntRecords[i], recordsBuffer.Records[i]);
      }
    }
  }
}
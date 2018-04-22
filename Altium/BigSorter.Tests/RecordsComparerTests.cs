using Xunit;
using System.Linq;

namespace Altium.BigSorter.Tests
{
  public class RecordsComparerTests
  {
    [Fact]
    public void Test()
    {
      BigArray<byte> buffer = new BigArray<byte>(100);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);
      RecordBytesConverter recordBytesConverter = new RecordBytesConverter();
      RecordsBuffer recordsBuffer = new RecordsBuffer(av, recordBytesConverter);

      object[][] records = new object[][]
      {
        new object[] {1, "a"},
        new object[] {2, "aa"},
        new object[] {3, "b"},
        new object[] {3, "b"},
        new object[] {4, "ab"},
      };

      foreach(object[] values in records)
      {
        recordsBuffer.AddRecord(values);
      }

      ArrayView<byte>[] recordBytes = records.Select((r, i) => recordsBuffer.GetRecordBytes((long)i))
        .ToArray();

      RecordComparer stringFieldComparer = new RecordComparer(recordBytesConverter, 1);
      Assert.Equal(1, stringFieldComparer.Compare(recordBytes[1], recordBytes[0]));
      Assert.Equal(-1, stringFieldComparer.Compare(recordBytes[0], recordBytes[1]));
      Assert.Equal(1, stringFieldComparer.Compare(recordBytes[2], recordBytes[1]));
      Assert.Equal(0, stringFieldComparer.Compare(recordBytes[2], recordBytes[3]));
      Assert.Equal(1, stringFieldComparer.Compare(recordBytes[3], recordBytes[4]));

      RecordComparer intFieldComparer = new RecordComparer(recordBytesConverter, 0);
      Assert.Equal(1, stringFieldComparer.Compare(recordBytes[1], recordBytes[0]));
      Assert.Equal(-1, stringFieldComparer.Compare(recordBytes[1], recordBytes[4]));
      Assert.Equal(0, stringFieldComparer.Compare(recordBytes[2], recordBytes[3]));
    }
  }
}
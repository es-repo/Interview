using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Altium.BigSorter.Tests
{
  public class RecordsBufferedWriterTests
  {
    [Fact]
    public void TestWriteRecord()
    {
      MemoryStream ms = new MemoryStream();
      StreamWriter sw = new StreamWriter(ms);
      BigArray<byte> buffer = new BigArray<byte>(32);
      ArrayView<byte> aw = new ArrayView<byte>(buffer, 0);

      RecordsBufferedWriter writer = new RecordsBufferedWriter(
        new RecordParser(), new RecordBytesConverter(), sw, aw);

      ArrayView<byte>[] records = new byte[][]
      {
        new byte[]{0,0,0,1,0,(byte)'a'},
        new byte[]{0,0,0,2,0,(byte)'b',0,(byte)'c'},
        new byte[]{0,0,0,3,0,(byte)'d'},
      }.Select(r => 
      {
        BigArray<byte> a = new BigArray<byte>((long)r.Length);
        a.CopyFrom(r);
        return new ArrayView<byte>(a, 0);
      }).ToArray();

      writer.WriteRecord(records[0]);
      sw.Flush();
      Assert.Equal(0, ms.Position);

      writer.WriteRecord(records[1]);
      sw.Flush();      
      Assert.Equal(0, ms.Position);

      writer.WriteRecord(records[2]);
      sw.Flush();
      Assert.Equal(Join(new string[] {"1. a", "2. bc"}), MemoryStreamToString(ms));

      writer.Flush();
      sw.Flush();
      Assert.Equal(Join(new string[] {"1. a", "2. bc", "3. d"}), MemoryStreamToString(ms));
    }

    private static string MemoryStreamToString(MemoryStream ms)
    {
      long pos = ms.Position;
      ms.Position = 0;
      StreamReader sr = new StreamReader(ms);
      string r = sr.ReadToEnd();
      ms.Position = pos;
      return r;
    }

    private static string Join(string[] a)
    {
      return string.Join(Environment.NewLine, a) + Environment.NewLine;
    }
  }
}
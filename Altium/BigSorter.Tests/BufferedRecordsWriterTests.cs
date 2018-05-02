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
      int sizeOfRecord = System.Runtime.InteropServices.Marshal.SizeOf<Record>() + 2;
      BufferedRecordsWriter writer = new BufferedRecordsWriter(sw, sizeOfRecord*2 + 4);

      Record[] records = new Record[]
      {
        new Record(1, "a"),
        new Record(2, "bc"),
        new Record(3, "d")
      };

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
      return string.Join("\r\n", a) + "\r\n";
    }
  }
}
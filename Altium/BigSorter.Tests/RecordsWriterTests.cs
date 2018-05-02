using System;
using System.IO;
using Xunit;

namespace Altium.BigSorter.Tests
{
  public class RecordsWriterTests
  {
    [Fact]
    public void WriteRecordsTest()
    {
      RecordsBuffer recordsBuffer = new RecordsBuffer(100);

      Record[] records = new Record[]
      {
        new Record ( 1, "ab" ),
        new Record ( 2, "cd" ),
        new Record ( 3, "ef" )
      };

      foreach (Record r in records)
        recordsBuffer.AddRecord(r);

      MemoryStream ms = new MemoryStream();
      StreamWriter sw = new StreamWriter(ms);

      RecordsWriter recordsWriter = new RecordsWriter(sw);
      recordsWriter.WriteRecords(recordsBuffer);
      sw.Flush();
      ms.Position = 0;
      StreamReader sr = new StreamReader(ms);
      string result = sr.ReadToEnd();
      string expected = string.Join("\r\n", new string[] { "1. ab", "2. cd", "3. ef" }) + "\r\n";
      Assert.Equal(expected, result);
    }
  }
}
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
      BigArray<byte> buffer = new BigArray<byte>(100);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);
      RecordsBuffer recordsBuffer = new RecordsBuffer(av, new RecordBytesConverter());

      object[][] records = new object[][]
      {
        new object[] { 1, "ab" },
        new object[] { 2, "cd" },
        new object[] { 3, "ef" },
      };

      foreach (object[] values in records)
        recordsBuffer.AddRecord(values);

      MemoryStream ms = new MemoryStream();
      StreamWriter sw = new StreamWriter(ms);

      RecordsWriter recordsWriter = new RecordsWriter(new RecordParser(), sw);
      recordsWriter.WriteRecords(recordsBuffer);
      sw.Flush();
      ms.Position = 0;
      StreamReader sr = new StreamReader(ms);
      string result = sr.ReadToEnd();
      string expected = string.Join(Environment.NewLine, new string[] { "1. ab", "2. cd", "3. ef" }) + Environment.NewLine;
      Assert.Equal(expected, result);
    }

    [Fact]
    public void WriteRecordsRawTest()
    {
      BigArray<byte> buffer = new BigArray<byte>(100);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);
      RecordsBuffer recordsBuffer = new RecordsBuffer(av, new RecordBytesConverter());

      object[][] records = new object[][]
      {
        new object[] { 1, "a" },
        new object[] { 2, "cd" }
      };

      foreach (object[] values in records)
        recordsBuffer.AddRecord(values);

      MemoryStream ms = new MemoryStream();

      RecordsWriter recordsWriter = new RecordsWriter(new RecordParser(), ms);
      recordsWriter.WriteRecordsRaw(recordsBuffer);
      ms.Flush();
      ms.Position = 0;

      byte[] expected = new byte[] { 0, 0, 0, 6, 0, 0, 0, 1, 0, (byte) 'a', 0, 0, 0, 8, 0, 0, 0, 2, 0, (byte) 'c', 0, (byte) 'd' };
      byte[] result = new byte[expected.Length];
      ms.Read(result, 0, result.Length);

      Assert.Equal(expected, result);
    }
  }
}
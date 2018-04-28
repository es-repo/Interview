// using System;
// using System.IO;
// using Xunit;

// namespace Altium.BigSorter.Tests
// {
//   public class RecordsWriterTests
//   {
//     [Fact]
//     public void WriteRecordsTest()
//     {
//       RecordsBuffer recordsBuffer = new RecordsBuffer(100);

//       object[][] records = new object[][]
//       {
//         new object[] { 1, "ab" },
//         new object[] { 2, "cd" },
//         new object[] { 3, "ef" },
//       };

//       foreach (object[] values in records)
//         recordsBuffer.AddRecord(values);

//       MemoryStream ms = new MemoryStream();
//       StreamWriter sw = new StreamWriter(ms);

//       RecordsWriter recordsWriter = new RecordsWriter(new RecordParser(), sw);
//       recordsWriter.WriteRecords(recordsBuffer);
//       sw.Flush();
//       ms.Position = 0;
//       StreamReader sr = new StreamReader(ms);
//       string result = sr.ReadToEnd();
//       string expected = string.Join(Environment.NewLine, new string[] { "1. ab", "2. cd", "3. ef" }) + Environment.NewLine;
//       Assert.Equal(expected, result);
//     }
//   }
// }
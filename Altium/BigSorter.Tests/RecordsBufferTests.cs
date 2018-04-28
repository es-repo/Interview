// using System.Collections.Generic;
// using System.IO;
// using Altium.BigSorter;
// using Xunit;

// namespace Altium.BigSorter.Tests
// {
//   public class RecordsBufferTests
//   {
//     [Fact]
//     public void TestAddRecord()
//     {
//       object[][] records = new object[][]
//       {
//         new object[] { 0x1, "a" },
//         new object[] { 0x2, "bc" },
//         new object[] { 0x4, "def" },
//         new object[] { 0x8, "xyz" }
//       };

//       long recordsBufferLen = 0;
//       for (int i = 0; i < records.Length - 1; i++)
//       {
//         recordsBufferLen += (sizeof(int) + ((string) records[i][1]).Length * sizeof(char));
//       }

//       recordsBufferLen += 2;

//       RecordsBuffer recordsBuffer = new RecordsBuffer(recordsBufferLen);

//       for (int i = 0; i < records.Length - 1; i++)
//       {
//         bool added = recordsBuffer.AddRecord(records[i]);
//         Assert.True(added);
//       }

//       bool addedLast = recordsBuffer.AddRecord(records[records.Length - 1]);
//       Assert.False(addedLast);
//       Assert.Equal(3, recordsBuffer.Records.Count);
//     }

//     [Fact]
//     public void TestSort()
//     {
//       object[][] records = new object[][]
//       {
//         new object[] { 1, "a" },
//         new object[] { 2, "aa" },
//         new object[] { 7, "b" },
//         new object[] { 2, "ab" },
//         new object[] { 8, "aa" },
//         new object[] { 4, "ba" },
//         new object[] { 5, "bb" }
//       };

//       RecordsBuffer recordsBuffer = new RecordsBuffer(200);
//       for (int i = 0; i < records.Length; i++)
//       {
//         recordsBuffer.AddRecord(records[i]);
//       }

//       recordsBuffer.Sort(1);

//       object[][] expectedSortedByStringRecords = new object[][]
//       {
//         new object[] { 1, "a" },
//         new object[] { 2, "aa" },
//         new object[] { 8, "aa" },
//         new object[] { 2, "ab" },
//         new object[] { 7, "b" },
//         new object[] { 4, "ba" },
//         new object[] { 5, "bb" }
//       };

//       for (int i = 0; i < recordsBuffer.Records.Count; i++)
//       {
//         Assert.Equal(expectedSortedByStringRecords[i], recordsBuffer.Records[i]);
//       }

//       recordsBuffer.Sort(0);

//       object[][] expectedSortedByIntRecords = new object[][]
//       {
//         new object[] { 1, "a" },
//         new object[] { 2, "aa" },
//         new object[] { 2, "ab" },
//         new object[] { 4, "ba" },
//         new object[] { 5, "bb" },
//         new object[] { 7, "b" },
//         new object[] { 8, "aa" }
//       };

//       for (int i = 0; i < recordsBuffer.Records.Count; i++)
//       {
//         Assert.Equal(expectedSortedByIntRecords[i], recordsBuffer.Records[i]);
//       }
//     }
//   }
// }
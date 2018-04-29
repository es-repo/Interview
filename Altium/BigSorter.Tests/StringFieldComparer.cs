// using System;
// using Xunit;

// namespace Altium.BigSorter.Tests
// {
//   public class StringFieldComparerTests
//   {
//     private readonly byte[] _buffer = new byte[100];
//     private int _nextRecordStart;
//     private readonly StringFieldComparer _comparer;

//     public StringFieldComparerTests()
//     {
//       _comparer = new StringFieldComparer(_buffer);
//     }

//     [Theory]
//     // [InlineData("a", "a", 0)]
//     [InlineData("a", "A", -1)]
//     // [InlineData("aa", "a", 1)]
//     // [InlineData("a", "aa", -1)]
//     // [InlineData("a", "b", -1)]
//     // [InlineData("aa", "b", -1)]
//     // [InlineData("abcde0", "abcde", 1)]
//     // [InlineData("abcdekkkkX", "abcdekkkky", 1)]
//     public void Test(string x, string y, int expectedRes)
//     {
//       _nextRecordStart = 0;
//       Record rx = CreateRecord(3, x);
//       Record ry = CreateRecord(5, y);
//       int res = _comparer.Compare(rx, ry);
//       Assert.Equal(expectedRes, Math.Sign(res));
//     }

//     private Record CreateRecord(int numberLen, string strVal)
//     {
//       int recStart = _nextRecordStart;
//       int stringStart = recStart + numberLen;
//       for (int i = 0; i < strVal.Length; i++)
//       {
//         _buffer[stringStart + i] = (byte)strVal[i];
//       }
//       _nextRecordStart = stringStart + strVal.Length;
//       return new Record(recStart, _nextRecordStart - recStart, 0, stringStart);
//     }
//   }
// }
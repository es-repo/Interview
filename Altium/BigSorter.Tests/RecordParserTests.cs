// using Xunit;

// namespace Altium.BigSorter.Tests
// {
//   public class RecordParserTests
//   {
//     [Fact]
//     public void TestToString()
//     {
//       RecordParser parser = new RecordParser();
//       Assert.Equal("123. abcd", parser.ToString(new object[] {123, "abcd"}));
//     }

//     [Fact]
//     public void TestParse()
//     {
//       RecordParser parser = new RecordParser();
//       Assert.Equal(new object[] {123, "abcd"}, parser.Parse("123. abcd"));
//     }
//   }
// }
using Xunit;

namespace Altium.BigSorter.Tests
{
  public class RecordParserTests
  {
    [Fact]
    public void TestToString()
    {
      RecordParser parser = new RecordParser();
      Record record = new Record(123, "abcd");
      Assert.Equal("123. abcd", parser.ToString(record));
    }

    [Fact]
    public void TestParse()
    {
      RecordParser parser = new RecordParser();
      Record record = parser.Parse("123. abcd");
      Assert.Equal(new Record(123, "abcd"), record);
    }
  }
}
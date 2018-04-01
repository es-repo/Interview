using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace Terrasoft.Tests
{
  public class NumberTokenBuilderTests
    {
        [Fact]
        public void TestAccept() {

            List<NumberToken> tokens = TokenBuilderTestUtils
                .Tokenise<NumberTokenBuilder, NumberToken>("ab1 a12 123a \n777 3 avx 999");
            Assert.Equal(new [] { 123, 777, 3, 999 }, tokens.Select(t => t.Value));
            Assert.Equal(new long[] { 8, 14, 18, 24 }, tokens.Select(t => t.Position));
        }
    }
}
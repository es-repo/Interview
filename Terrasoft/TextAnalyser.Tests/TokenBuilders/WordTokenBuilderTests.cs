using System.Collections.Generic;
using System.Linq;
using Terrasoft;
using Xunit;

namespace Terrasoft.Tests
{
    public class WordTokenBuilderTests
    {
        [Fact]
        public void Test()
        {
            List<WordToken> tokens = TokenBuilderTestUtils
                .Tokenise<WordTokenBuilder, WordToken>("ab1 .z\nab");
            Assert.Equal(new [] { "ab1", "z", "ab" }, tokens.Select(t => t.Value));
            Assert.Equal(new long[] { 0, 5, 7 }, tokens.Select(t => t.Position));
        }
    }
}
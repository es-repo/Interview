using System.Collections.Generic;
using System.Linq;
using Terrasoft;
using Xunit;

namespace Terrasoft.Tests
{
    public class SentenceTokenBuilderTests
    {
        [Fact]
        public void Test()
        {
            string text = "  Hello world!!! \nHello world.\n   Hello worl";
            List<SentenceToken> tokens = TokenBuilderTestUtils
                .Tokenise<SentenceTokenBuilder, SentenceToken>(text);
            Assert.Equal(new [] { "Hello world!!!", "Hello world.", "Hello worl" }, 
                tokens.Select(t => t.Value));
            Assert.Equal(new long[] {2, 18, 34}, tokens.Select(t => t.Position));    
        }
    }
}
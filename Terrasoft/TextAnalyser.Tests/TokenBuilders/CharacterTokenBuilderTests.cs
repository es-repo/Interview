using System.Collections.Generic;
using System.Linq;
using Terrasoft;
using Xunit;

namespace Terrasoft.Tests
{
  public class CharacterTokenBuilderTests
    {
        [Fact]
        public void Test()
        {
            string text = "ab1 .z\na";
            List<CharacterToken> tokens = TokenBuilderTestUtils
                .Tokenise<CharacterTokenBuilder, CharacterToken>(text);
            Assert.Equal(new [] { 'a', 'b', '1', ' ', '.', 'z', '\n', 'a' }, tokens.Select(t => t.Value));
            Assert.Equal(new long[] {0, 1, 2, 3, 4 , 5 ,6, 7}, tokens.Select(t => t.Position));    
        }
    }
}
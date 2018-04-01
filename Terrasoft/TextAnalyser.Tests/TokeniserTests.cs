using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Terrasoft.Tests
{
  public class TokeniserTests
  {
    [Theory,

      InlineData("",
        new string[0],
        new long[0],
        new string[0],
        new long[0]),

      InlineData("a",
        new string[] { "a" },
        new long[] { 0 },
        new string[] { "a" },
        new long[] { 0 }),

      InlineData("\n Hello World!! Neque porro quisquam... Lorem Ipsum",
        new string[] { "Hello", "World", "Neque", "porro", "quisquam", "Lorem", "Ipsum" },
        new long[] { 2, 8, 16, 22, 28, 40, 46 },
        new string[] { "Hello World!!", "Neque porro quisquam...", "Lorem Ipsum" },
        new long[] { 2, 16, 40 })
    ]
    public void TestTokinise(string text,
      string[] expectedWordTokens,
      long[] expectedWordTokenPositions,
      string[] expectedSentenceTokens,
      long[] expectedSenteceTokenPositions)
    {
      Tokeniser tokeniser = new Tokeniser(
        new CharacterTokenBuilder(),
        new WordTokenBuilder(),
        new SentenceTokenBuilder()
      );

      IList<Token> tokens;
      using(MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
      using(StreamReader sr = new StreamReader(ms))
      {
        tokens = tokeniser.Tokenise(sr).Select(t =>(Token) t.Clone()).ToList();
      }

      Assert.Equal(expectedWordTokens, tokens.OfType<WordToken>().Select(t => t.Value));
      Assert.Equal(expectedWordTokenPositions, tokens.OfType<WordToken>().Select(t => t.Position));

      Assert.Equal(expectedSentenceTokens, tokens.OfType<SentenceToken>().Select(t => t.Value));
      Assert.Equal(expectedSenteceTokenPositions, tokens.OfType<SentenceToken>().Select(t => t.Position));
    }
  }
}
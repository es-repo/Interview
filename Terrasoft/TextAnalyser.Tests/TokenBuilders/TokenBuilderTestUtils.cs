using System.Collections.Generic;
using System;

namespace Terrasoft.Tests
{
  public static class TokenBuilderTestUtils
    {
        public static List<TToken> Tokenise<TTokenBuilder, TToken>(string text)
            where TToken : Token
            where TTokenBuilder : TokenBuilder 
        {
            TTokenBuilder tokenBuilder = (TTokenBuilder)Activator.CreateInstance(typeof(TTokenBuilder));
            List<TToken> tokens = new List<TToken>();
            tokenBuilder.TokenReady += (s, e) =>
            {
                tokens.Add((TToken) e.Token.Clone());
            };

            long pos = 0;
            foreach (char ch in text)
            {
                tokenBuilder.OnNextChar(ch, pos);
                pos++;
            }
            tokenBuilder.OnEnd();

            return tokens;
        }
    }
}
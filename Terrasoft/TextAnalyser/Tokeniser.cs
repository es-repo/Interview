using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Terrasoft
{
  /// <summary>
  /// Tokenise input text stream using <see cref="TokenBuilder"/>s recieved in constructor. 
  /// </summary>
  public class Tokeniser
  {
    private TokenBuilder[] _tokenBuilders;
    private List<Token> _tokens;

    public Tokeniser(params TokenBuilder[] tokenBuilders)
    {
      _tokens = new List<Token>();
      _tokenBuilders = tokenBuilders;
    }

    /// <summary>
    /// Tokenise input text using <see cref="TokenBuilder"/>s recieved in constructor. 
    /// </summary>
    public IEnumerable<Token> Tokenise(string text)
    {
      using(MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
      using(StreamReader sr = new StreamReader(ms))
        return Tokenise(sr);
    }

    /// <summary>
    /// Tokenise input text stream using <see cref="TokenBuilder"/>s recieved in constructor. 
    /// </summary>
    public IEnumerable<Token> Tokenise(StreamReader stream)
    {
      try
      {
        SubscribeOnTokenReadyEvents();

        _tokens.Clear();
        int next;
        long position = 0;
        while ((next = stream.Read()) != -1)
        {
          char nextChar = (char) next;
          foreach (TokenBuilder tb in _tokenBuilders)
          {
            tb.OnNextChar(nextChar, position);
          }

          // Returns tokens which were accumulated on TokenReady events of token builders.
          if (_tokens.Count > 0)
          {
            foreach (Token t in _tokens)
            {
              yield return t;
            }
            _tokens.Clear();
          }

          position++;
        }

        // Signal token builders that stream is ended.
        foreach (TokenBuilder tb in _tokenBuilders)
        {
          tb.OnEnd();
        }
        // Returns tokens which were accumulated on TokenReady events of token builders.
        foreach (Token t in _tokens)
        {
          yield return t;
        }
      }
      finally
      {
        UnsubscribeFromTokenReadyEvents();
      }
    }

    private void SubscribeOnTokenReadyEvents()
    {
      foreach (TokenBuilder tb in _tokenBuilders)
      {
        tb.TokenReady += OnTokenReady;
      }
    }

    private void UnsubscribeFromTokenReadyEvents()
    {
      foreach (TokenBuilder tb in _tokenBuilders)
      {
        tb.TokenReady -= OnTokenReady;
      }
    }

    private void OnTokenReady(object sender, TokenReadyEventArgs ea)
    {
      _tokens.Add(ea.Token);
    }
  }
}
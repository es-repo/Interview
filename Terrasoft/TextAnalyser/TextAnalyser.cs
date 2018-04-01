using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Terrasoft
{
  public class TextAnalyser
  {
    private readonly Tokeniser _tokeniser;

    public TextAnalyser(Tokeniser tokeniser)
    {
      _tokeniser = tokeniser;
    }

    public void Analyse(StreamReader stream, params Metric[] metrics)
    {
      foreach (Token t in _tokeniser.Tokenise(stream))
      {
        foreach (Metric m in metrics)
          m.OnNextToken(t);
      }
    }

    public void Analyse(string text, params Metric[] metrics)
    {
      using(MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(text)))
      using(StreamReader sr = new StreamReader(ms))
      {
        Analyse(sr, metrics);
      }
    }
  }
}
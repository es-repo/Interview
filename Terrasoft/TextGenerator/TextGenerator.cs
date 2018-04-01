using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Terrasoft
{
  class TextGenerator
  {
    private static readonly string[] _endOfSentencePunctuations = { ".", "?", "!", "...", "!!!", "!!", "?!" };
    private static readonly string[] _words;
    private readonly Random _rnd;

    static TextGenerator()
    {
      Assembly assembly = typeof(TextGenerator).GetTypeInfo().Assembly;
      using(Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Words.txt"))
      using(StreamReader reader = new StreamReader(stream))
      {
        string text = reader.ReadToEnd();
        _words = text.Split('\n');
      }
    }

    public TextGenerator()
    {
      _rnd = new Random();
    }

    public IEnumerable<string> Generate()
    {
      while (true)
      {
        yield return GenerateParagraph() + Environment.NewLine;
      }
    }

    private string GenerateParagraph()
    {
      int length = _rnd.Next(5, 30);
      IList<string> sentences = Enumerable.Range(0, length)
        .Select(i => GenerateSentence()).ToList();

      return "\t" + String.Join(" ", sentences) + Environment.NewLine;
    }

    private string GenerateSentence()
    {
      int length = _rnd.Next(1, 20);
      IList<string> words = Enumerable.Range(0, length)
        .Select(i => GetRandomWord()).ToList();

      words[0] = Capitalize(words[0]);
      string endPunctuation = GetRandomEndOfSentencePunctuation();
      return String.Join(" ", words) + endPunctuation;
    }

    private string GetRandomWord()
    {
      bool preferNumber = _rnd.NextDouble() < 0.02;
      return preferNumber ?
        _rnd.Next(1000).ToString() :
        _words[_rnd.Next(_words.Length)];
    }

    private string GetRandomEndOfSentencePunctuation()
    {
      bool dot = _rnd.NextDouble() < 0.7;
      return dot ?
        "." :
        _endOfSentencePunctuations[_rnd.Next(_endOfSentencePunctuations.Length)];
    }

    private static string Capitalize(string str)
    {
      if (str.Length < 1)
        return str;

      return char.ToUpper(str[0]) + str.Substring(1);
    }
  }
}
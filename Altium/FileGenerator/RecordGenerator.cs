using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Altium
{
  public class RecordGenerator
  {
    private static readonly List<string> _words;
    private readonly Random _rnd = new Random();

    static RecordGenerator()
    {
      Assembly assembly = typeof(RecordGenerator).GetTypeInfo().Assembly;
      using(Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Words.txt"))
      using(StreamReader reader = new StreamReader(stream))
      {
        _words = new List<string>();
        string s = null;
        while((s = reader.ReadLine()) != null)
        {
          _words.Add(s);
        }
      }
    }

    public string Generate()
    {
      StringBuilder sb = new StringBuilder();
      int wordCount = _rnd.Next(1, 4);
      string[] words = new string[wordCount];
      for (int i = 0; i < wordCount; i++)
      {
        string w = _words[_rnd.Next(_words.Count)];
        words[i] = w;
      }

      int number = _rnd.Next(10000);
      return number + ". " + String.Join(" ", words);
    }

    public IEnumerable<string> Generate(long maxTotalLength)
    {
      long length = 0;
      while (true)
      {
        string s = Generate();
        length += s.Length + 2;
        if (length > maxTotalLength)
          break;

        yield return s;
      }
    }
  }
}
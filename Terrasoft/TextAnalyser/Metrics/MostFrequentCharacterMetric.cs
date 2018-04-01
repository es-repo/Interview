using System;
using System.Collections.Generic;
using System.Linq;

namespace Terrasoft
{
  public class MostFrequentCharacterMetric : Metric<CharacterToken>
  {
    Dictionary<char, int> _frequency = new Dictionary<char, int>();

    public char Character
    {
      get
      {
        if (_frequency.Count == 0)
          throw new InvalidOperationException("The metric didn't recieve any character yet.");

        return _frequency.Keys.OrderByDescending(k => _frequency[k]).First();
      }
    }

    public long OccurenciesCount
    {
      get
      {
        return _frequency[Character];
      }
    }

    protected override void OnNextToken(CharacterToken token)
    {
      if (char.IsWhiteSpace(token.Value) || char.IsSeparator(token.Value))
        return;  

      if (!_frequency.ContainsKey(token.Value))
      {
        _frequency.Add(token.Value, 1);
      }
      else
      {
        _frequency[token.Value]++;
      }
    }

    public override string ToString()
    {
      return $"Most frequented character is '{Character}'. Number of occurrences is {OccurenciesCount}.";
    }
  }
}
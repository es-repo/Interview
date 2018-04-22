using System;
using System.Collections.Generic;
using System.Linq;

namespace Altium.BigSorter
{
  public class RecordParser : IRecordParser
  { 
    private const string Separator = ". ";

    public IEnumerable<object> Parse(string str)
    {
      string[] arr = str.Split(Separator, 2);
      yield return Int32.Parse(arr[0]);
      yield return arr[1];
    }

    public string ToString(IEnumerable<object> values)
    {
      return string.Join(Separator, values.ToArray());
    }
  }
}
using System.Collections.Generic;

namespace Altium.BigSorter
{
  public class RecordParser
  {
    public Record Parse(string s)
    {
      int numEnd;
      int number = ParseNumber(s, out numEnd);
      string str = s.Substring(numEnd + 2);
      return new Record(number, str);
    }

    private int ParseNumber(string s, out int numberEnd)
    {
      int i = 0;
      int n = 0;
      while(s[i] != '.')
      {
        n *= 10;
        n += s[i] - '0';
        i++;
      }
      numberEnd = i;
      return n;
    }

    public string ToString(Record record)
    {
      return record.Number.ToString() + ". " + record.String;
    }
  }
}
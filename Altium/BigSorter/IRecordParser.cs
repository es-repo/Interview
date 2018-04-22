using System.Collections.Generic;

namespace Altium.BigSorter
{
  public interface IRecordParser
  {
    IEnumerable<object> Parse(string str);
    string ToString(IEnumerable<object> values);
  }
}
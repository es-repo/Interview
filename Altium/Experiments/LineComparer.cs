using System.Collections.Generic;

namespace Experiments
{
  partial class Program
  {
    class LineComparer : IComparer<Line>
    {
      byte[] _data;

      public LineComparer(byte[] data)
      {
        _data = data;
      }

      public int Compare(Line x, Line y)
      {
        int minLen = x.Len < y.Len ? x.Len : y.Len;
        for (int i = 0; i < minLen; i++)
        {
          if (_data[x.Pos + i] < _data[y.Pos + i]) return -1;
          if (_data[x.Pos + i] > _data[y.Pos + i]) return 1;
        }
        if (x.Len < y.Len) return -1;
        if (x.Len > y.Len) return 1;
        return 0;
      }
    }
  }
}
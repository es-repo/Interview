using System.Collections.Generic;

namespace Experiments
{
  partial class Program
  {
    class LineComparerUnsafe : IComparer<Line>
    {
      byte[] _data;

      public LineComparerUnsafe(byte[] data)
      {
        _data = data;
      }

      unsafe public int Compare(Line x, Line y)
      {
        int l = x.Len < y.Len ? x.Len : y.Len;
        fixed(byte * pd = _data)
        {
          byte * px = pd + x.Pos;
          byte * py = pd + y.Pos;
          for (int i = 0; i < l; i++, px++, py++)
          {
            if (*px < *py) return -1;
            if (*px > *py) return 1;
          }
          // int i;
          // for (i = 0; i < l / 8; i++, px += 8, py += 8)
          // {
          //   if ( * ((ulong * ) px) < * ((ulong * ) py)) return 1;
          //   if ( * ((ulong * ) px) > * ((ulong * ) py)) return -1;
          // }
          // l -= i;
          // if (l > 0)
          // {
          //   if ( * ((uint * ) px) < * ((uint * ) py)) return 1;
          //   if ( * ((uint * ) px) > * ((uint * ) py)) return -1;
          //   px += 4;
          //   py += 4;
          //   l -= 4;
          // }

          // if (l > 0)
          // {
          //   if ( * ((ushort * ) px) < * ((ushort * ) py)) return 1;
          //   if ( * ((ushort * ) px) > * ((ushort * ) py)) return -1;
          //   px += 2;
          //   py += 2;
          //   l -= 2;
          // }

          // if (l > 0)
          // {
          //   if ( * ((byte * ) px) < * ((byte * ) py)) return -1;
          //   if ( * ((byte * ) px) > * ((byte * ) py)) return 1;
          // }
        }

        if (x.Len < y.Len) return -1;
        if (x.Len > y.Len) return 1;
        return 0;
      }
    }
  }
}
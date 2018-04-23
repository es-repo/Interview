using System;
using System.Collections.Generic;

namespace Altium.BigSorter
{
  public class RecordComparer : IComparer<object[]>
  {
    private readonly int _fieldIndex;

    public RecordComparer(int fieldIndex)
    {
      _fieldIndex = fieldIndex;
    }

    public int Compare(object[] x, object[] y)
    {
      switch (_fieldIndex)
      {
        case 0:
          return (int) x[0] - (int) y[0];
        case 1:
          return ((string) x[1]).CompareTo((string) y[1]);
        default: 
          return 0;
      }
    }
  }
}
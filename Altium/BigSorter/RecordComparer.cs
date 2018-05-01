using System;
using System.Collections.Generic;

namespace Altium.BigSorter
{
  /// <summary>
  /// Factory for record field comparers.
  /// </summary>
  public class RecordComparer
  {
    public IRecordFieldComparer CreateRecordFieldComparer(int field)
    {
      switch (field)
      {
        case 0: return new NumberFieldComparer();
        case 1: return new StringFieldComparer();
        default: throw new NotImplementedException();
      }
    }
  }

  public interface IRecordFieldComparer : IComparer<Record>
  {
  }

  /// <summary>
  /// Compares records by their String property.
  /// </summary>
  public class StringFieldComparer : IRecordFieldComparer
  {
    unsafe public int Compare(Record x, Record y)
    {
      return x.String.CompareTo(y.String);
    }
  }

  /// <summary>
  /// Compares records by their Number property.
  /// </summary>
  public class NumberFieldComparer : IRecordFieldComparer
  {
    unsafe public int Compare(Record x, Record y)
    {
      return x.Number - y.Number;
    }
  }
}
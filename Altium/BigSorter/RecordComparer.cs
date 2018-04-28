using System;
using System.Collections.Generic;

namespace Altium.BigSorter
{
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

  public class StringFieldComparer : IRecordFieldComparer
  {
    unsafe public int Compare(Record x, Record y)
    {
      return x.String.CompareTo(y.String);
    }
  }

  public class NumberFieldComparer : IRecordFieldComparer
  {
    unsafe public int Compare(Record x, Record y)
    {
      return x.Number - y.Number;
    }
  }
}
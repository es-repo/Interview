using System.Collections.Generic;

namespace Altium.BigSorter
{
  public interface IRecordComparer
  {
    IRecordFieldComparer CreateRecordFieldComparer(int field);
  }

  public interface IRecordFieldComparer : IComparer<Record>
  {
  }
}
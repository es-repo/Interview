using System.Collections.Generic;

namespace Altium.BigSorter
{
  public interface IRecordComparer : IComparer<RecordInfo> { }

  public delegate IRecordComparer CreateRecordComparer(byte[] _buffer, int field);
}
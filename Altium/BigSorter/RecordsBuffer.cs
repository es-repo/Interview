using System;
using System.Collections.Generic;
using System.IO;

namespace Altium.BigSorter
{
  public class RecordsBuffer
  {
    private readonly long _maxSizeInBytes;
    private long _size;
    public List<object[]> Records {get; } = new List<object[]>();

    public RecordsBuffer(long maxSizeInBytes)
    {
      _maxSizeInBytes = maxSizeInBytes;
    }

    public bool AddRecord(object[] values)
    {
      int recordSize = RecordSizeInBytes(values);
      if (_size + recordSize > _maxSizeInBytes)
        return false;
      
      _size += recordSize;
      Records.Add(values);
      return true;
    }

    private int RecordSizeInBytes(object[] values)
    {
      return sizeof(int) + ((string)values[1]).Length * sizeof(char);
    }

    public void Sort(int fieldIndex)
    {
      RecordComparer comparer = new RecordComparer(fieldIndex);
      Records.Sort(comparer);
    }
  }
}
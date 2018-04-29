using System;
using System.Collections.Generic;
using System.Linq;

namespace Altium.BigSorter
{
  public class RecordsBuffer
  {
    private readonly RecordComparer _recordComparer;
    private readonly long _maxSizeInBytes;
    private long _size;
    public List<Record> Records { get; private set; } = new List<Record>();

    public RecordsBuffer(long maxSizeInBytes)
    {
      _recordComparer = new RecordComparer();
      _maxSizeInBytes = maxSizeInBytes;
    }

    public bool AddRecord(Record record)
    {
      if (_size + record.SizeInBytes > _maxSizeInBytes)
        return false;

      _size += record.SizeInBytes;
      Records.Add(record);
      return true;
    }

    public void Sort(int field)
    {
      IRecordFieldComparer comparer = _recordComparer.CreateRecordFieldComparer(field);
      Records.ParallelMergeSort(comparer);
      
      //Records = ParallelSorter.Sort(Records, comparer);
      //Records.Sort(comparer);
    }
  }
}
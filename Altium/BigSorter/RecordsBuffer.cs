using System;
using System.Collections.Generic;
using System.Linq;

namespace Altium.BigSorter
{
  /// <summary>
  /// Keeps list of table records.
  /// </summary>
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

    /// <summary>
    /// Adds a record to the buffer if size limit is not reached.
    /// </summary>
    /// <param name="record">Record to add.</param>
    /// <returns>Returns true if record was added otherwise false.</returns>
    public bool AddRecord(Record record)
    {
      if (_size + record.SizeInBytes > _maxSizeInBytes || Records.Count == int.MaxValue)
        return false;

      _size += record.SizeInBytes;
      Records.Add(record);
      return true;
    }

    /// <summary>
    /// Sorts records by specified field.
    /// </summary>
    /// <param name="field">Field index by which sorting will happen.</param>
    public void Sort(int field)
    {
      IRecordFieldComparer comparer = _recordComparer.CreateRecordFieldComparer(field);
      Records = Records.ParallelSort(comparer);
    }
  }
}
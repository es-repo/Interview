using System;
using System.Collections.Generic;
using System.Linq;

namespace Altium.BigSorter
{
  public class RecordsBuffer
  {
    private readonly IRecordComparer _recordComparer;
    private int _nextRecordPosition;

    public readonly ArrayView<byte> BufferView;
    public readonly List<Record> Records;

    public RecordsBuffer(ArrayView<byte> bufferView, List<Record> recordsInfo,
      IRecordComparer recordComparer)
    {
      BufferView = bufferView;
      Records = recordsInfo;
      _recordComparer = recordComparer;
      _nextRecordPosition = BufferView.Start;
    }

    public bool AddRecord(Record record)
    {
      if (_nextRecordPosition + record.Length > BufferView.Start + BufferView.Length)
        return false;

      byte[] buffer = BufferView.Array;
      Buffer.BlockCopy(buffer, record.Start, buffer, _nextRecordPosition, record.Length);
      Records.Add(new Record(_nextRecordPosition, record.Length, record.Number, record.StringStart));
      _nextRecordPosition += record.Length;
      return true;
    }

    public void Sort(int field)
    {
      IRecordFieldComparer comparer = _recordComparer.CreateRecordFieldComparer(field);
      Records.Sort(comparer);
    }
  }
}
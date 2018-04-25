using System;
using System.Collections.Generic;
using System.Linq;

namespace Altium.BigSorter
{
  public class RecordsBuffer
  {
    private readonly CreateRecordComparer _createRecordComparer;
    private int _nextRecordPosition;

    public readonly ArrayView<byte> BufferView;
    public readonly List<RecordInfo> RecordsInfo;

    public RecordsBuffer(ArrayView<byte> bufferView, List<RecordInfo> recordsInfo,
      CreateRecordComparer createRecordComparer)
    {
      BufferView = bufferView;
      RecordsInfo = recordsInfo;
      _createRecordComparer = createRecordComparer;
      _nextRecordPosition = BufferView.Start;
    }

    public bool AddRecord(RecordInfo record)
    {
      if (_nextRecordPosition + record.Length > BufferView.Start + BufferView.Length)
        return false;

      byte[] buffer = BufferView.Array;
      Buffer.BlockCopy(buffer, record.Position, buffer, _nextRecordPosition, record.Length);
      RecordsInfo.Add(new RecordInfo(_nextRecordPosition, record.Length));
      _nextRecordPosition += record.Length;
      return true;
    }

    public void Sort(int field)
    {
      IRecordComparer recordComparer = _createRecordComparer(BufferView.Array, field);
      RecordsInfo.Sort(recordComparer);
    }
  }
}
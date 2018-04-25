using System;
using System.Collections.Generic;
using System.IO;

namespace Altium.BigSorter
{
  public class RecordsReader
  {
    private readonly ArrayView<byte> _bufferView;
    private readonly CreateRecordComparer _createRecordComparer;
    private readonly Stream _stream;

    public RecordsReader(ArrayView<byte> bufferView, Stream stream, CreateRecordComparer createRecordComparer)
    {
      _bufferView = bufferView;
      _createRecordComparer = createRecordComparer;
      _stream = stream;
    }

    public bool IsLastBlock {get; private set;}

    public IEnumerable<RecordsBuffer> ReadBlocks()
    {
      IsLastBlock = false;
      int offset = _bufferView.Start;
      int count = _bufferView.Length;
      int bytesCount;
      int unfinishedRecordLen = 0;
      while ((bytesCount = _stream.Read(_bufferView.Array, offset, count)) > 0 || unfinishedRecordLen > 0)
      {
        bytesCount += unfinishedRecordLen;
        int unfinishedRecordStart = -1;
        List<RecordInfo> records = GetRecordsInfo(bytesCount, out unfinishedRecordStart);

        unfinishedRecordLen = 0;
        if (unfinishedRecordStart != -1)
        {
          unfinishedRecordLen = bytesCount - unfinishedRecordStart + _bufferView.Start;
        }
        int recordsBytesCount = bytesCount - unfinishedRecordLen;

        IsLastBlock = _stream.Position == _stream.Length;
        yield return new RecordsBuffer(
          new ArrayView<byte>(_bufferView, 0, recordsBytesCount),
          records,
          _createRecordComparer);

        offset = _bufferView.Start + unfinishedRecordLen;
        count = _bufferView.Length - unfinishedRecordLen;
        if (unfinishedRecordLen > 0)
        {
          Buffer.BlockCopy(_bufferView.Array, unfinishedRecordStart, _bufferView.Array, _bufferView.Start, unfinishedRecordLen);
        }
      }
      IsLastBlock = true;
    }

    private List<RecordInfo> GetRecordsInfo(int bytesCount, out int unfinishedRecordStart)
    {
      unfinishedRecordStart = -1;

      int recLen = 0;
      int recStart = _bufferView.Start;
      byte[] buffer = _bufferView.Array;
      List<RecordInfo> records = new List<RecordInfo>();
      int end = _bufferView.Start + bytesCount;
      for (int i = _bufferView.Start; i < end; i++)
      {
        recLen++;
        if (buffer[i] == 0x0D && (i+1 < buffer.Length) && buffer[i+1] == 0x0A)
        {
          i++;
          recLen++;
          records.Add(new RecordInfo(recStart, recLen));
          recStart += recLen;
          recLen = 0;
        }
      }

      if (recStart < end)
        unfinishedRecordStart = recStart;

      return records;
    }

    public IEnumerable<RecordInfo> ReadRecords()
    {
      foreach (RecordsBuffer block in ReadBlocks())
        foreach (RecordInfo ri in block.RecordsInfo)
          yield return ri;
    }
  }
}
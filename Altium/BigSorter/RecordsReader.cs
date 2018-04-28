using System;
using System.Collections.Generic;
using System.IO;

namespace Altium.BigSorter
{
  public class RecordsReader
  {
    private readonly IRecordComparer _recordComparer;
    private readonly RecordParser _recordParser;

    private readonly ArrayView<byte> _bufferView;
    private readonly Stream _stream;

    public RecordsReader(IRecordComparer recordComparer, ArrayView<byte> bufferView, Stream stream)
    {
      _recordComparer = recordComparer;
      _recordParser = new RecordParser();
      _bufferView = bufferView;
      _stream = stream;
    }

    public bool IsLastBlock { get; private set; }

    public IEnumerable<RecordsBuffer> ReadBlocks()
    {
      IsLastBlock = false;
      int readOffset = _bufferView.Start;
      int readCount = _bufferView.Length;
      int bytesCount;
      int blockCount = 0;
      while ((bytesCount = _stream.Read(_bufferView.Array, readOffset, readCount)) > 0)
      {
        int bytesNotParsedStart;
        int bytesToParseCount = bytesCount + readOffset - _bufferView.Start;
        List<Record> records = _recordParser.Parse(_bufferView, bytesToParseCount, out bytesNotParsedStart);
        
        IsLastBlock = _stream.Position == _stream.Length;
        yield return new RecordsBuffer(
          _bufferView,
          records,
          _recordComparer);

        blockCount ++;

        int notParsedBytesCount =  _bufferView.Start + bytesToParseCount - bytesNotParsedStart;
        Buffer.BlockCopy(_bufferView.Array, bytesNotParsedStart, _bufferView.Array, 
          _bufferView.Start, notParsedBytesCount);

        readOffset = _bufferView.Start + notParsedBytesCount;
        readCount = _bufferView.Length - notParsedBytesCount;
      }
    }

    public IEnumerable<Record> ReadRecords()
    {
      foreach (RecordsBuffer block in ReadBlocks())
        foreach (Record ri in block.Records)
          yield return ri;
    }
  }
}
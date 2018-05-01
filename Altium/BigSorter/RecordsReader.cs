using System;
using System.Collections.Generic;
using System.IO;

namespace Altium.BigSorter
{
  /// <summary>
  /// Reads records from stream.
  /// </summary>
  public class RecordsReader
  {
    private readonly RecordParser _recordParser;
    private readonly RecordComparer _recordComparer;
    private readonly StreamReader _streamReader;
    private readonly long _bufferSizeInBytes;
    private readonly int _readWhileSameFieldValue;
    private Record _recordAhead;
    private bool _hasRecordAhead;

    public RecordsReader(StreamReader streamReader, long bufferSizeInBytes, int readWhileSameFieldValue = -1)
    {
      _recordParser = new RecordParser();
      _recordComparer = new RecordComparer();
      _streamReader = streamReader;
      _bufferSizeInBytes = bufferSizeInBytes;
      _readWhileSameFieldValue = readWhileSameFieldValue;
    }

    public bool IsLastBlock { get; private set; }

    public bool IsEnd { get { return _streamReader.EndOfStream && !_hasRecordAhead; } }

    /// <summary>
    /// Reads records by blocks of specific size returning one after another.
    /// </summary>
    public IEnumerable<RecordsBuffer> ReadBlocks()
    {
      IsLastBlock = false;
      RecordsBuffer recordsBuffer = new RecordsBuffer(_bufferSizeInBytes);
      Record? recordToCompare = null;
      IRecordFieldComparer comparer = _readWhileSameFieldValue != -1
        ? _recordComparer.CreateRecordFieldComparer(_readWhileSameFieldValue)
        : null;

      if (_hasRecordAhead)
      {
        recordToCompare = _recordAhead;
        recordsBuffer.AddRecord(_recordAhead);
        _hasRecordAhead = false;
      }

      string line = null;
      while ((line = _streamReader.ReadLine()) != null)
      {
        Record record = _recordParser.Parse(line);

        if (_readWhileSameFieldValue >= 0)
        {
          if (recordToCompare == null)
          {
            recordToCompare = record;
          }
          else if (comparer.Compare(record, recordToCompare.Value) != 0)
          {
            _recordAhead = record;
            _hasRecordAhead = true;
            break;
          }
        }

        bool added = recordsBuffer.AddRecord(record);
        if (!added)
        {
          if (recordsBuffer.Records.Count == 0)
            throw new InvalidOperationException("Buffer is too small. Can't add even one record.");

          yield return recordsBuffer;
          recordsBuffer = new RecordsBuffer(_bufferSizeInBytes);
          recordsBuffer.AddRecord(record);
        }
      }

      if (recordsBuffer.Records.Count > 0)
      {
        IsLastBlock = true;
        yield return recordsBuffer;
      }
    }

    /// <summary>
    /// Reads records by blocks of specific size and enumerate all records one after another.
    /// </summary>
    public IEnumerable<Record> ReadRecords()
    {
      IEnumerable<RecordsBuffer> blocks = ReadBlocks();
      foreach (RecordsBuffer block in blocks)
      {
        for (int i = 0; i < block.Records.Count; i++)
        {
          yield return block.Records[i];
        }
      }
    }
  }
}
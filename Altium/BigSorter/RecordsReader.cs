using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Altium.BigSorter
{
  public class RecordsReader
  {
    private readonly long _bufferSizeInBytes;
    private readonly IRecordParser _recordParser;
    private readonly StreamReader _streamReader;
    private readonly int _readWhileEqualFieldIndex;
    private Object[] _recordAhead;

    public RecordsReader(long bufferSizeInBytes, IRecordParser recordParser, StreamReader streamReader, int readWhileEqualToFieldIndex = -1)
    {
      _bufferSizeInBytes = bufferSizeInBytes;
      _recordParser = recordParser;
      _streamReader = streamReader;
      _readWhileEqualFieldIndex = readWhileEqualToFieldIndex;
    }

    public bool IsLastBlock { get; private set; }

    public bool IsEnd { get { return _streamReader.EndOfStream && _recordAhead == null;}}

    public IEnumerable<RecordsBuffer> ReadBlocks()
    {
      IsLastBlock = false;
      string line = null;
      RecordsBuffer recordsBuffer = new RecordsBuffer(_bufferSizeInBytes);
      object valueToCompare = null;

      if (_recordAhead != null)
      {
        valueToCompare = _recordAhead[_readWhileEqualFieldIndex];
        recordsBuffer.AddRecord(_recordAhead);
        _recordAhead = null;
      }

      while ((line = _streamReader.ReadLine()) != null)
      {
        if (line == "")
          continue;

        object[] record = _recordParser.Parse(line).ToArray();

        if (_readWhileEqualFieldIndex >= 0)
        {
          object value = record[_readWhileEqualFieldIndex];
          if (valueToCompare == null)
          {
            valueToCompare = value;
          }
          else if (!value.Equals(valueToCompare))
          {
            _recordAhead = record;
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

    public IEnumerable<object[]> ReadRecords()
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
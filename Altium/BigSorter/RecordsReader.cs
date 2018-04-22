using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Altium.BigSorter
{
  public class RecordsReader
  {
    private readonly IRecordParser _recordParser;
    private readonly IRecordBytesConverter _recordBytesConverter;
    private readonly Stream _stream;
    private ArrayView<byte> _buffer;
    private readonly StreamReader _streamReader;
    private readonly int _readWhileEqualFieldIndex;
    private Object[] _recordAhead;

    public RecordsReader(IRecordParser recordParser, IRecordBytesConverter recordBytesConverter,
      Stream stream, ArrayView<byte> buffer)
    {
      _recordParser = recordParser;
      _recordBytesConverter = recordBytesConverter;
      _stream = stream;
      _buffer = buffer;
    }

    public RecordsReader(IRecordParser recordParser, IRecordBytesConverter recordBytesConverter,
      StreamReader streamReader, ArrayView<byte> buffer, int readWhileEqualToFieldIndex = -1)
    {
      _recordParser = recordParser;
      _recordBytesConverter = recordBytesConverter;
      _streamReader = streamReader;
      _buffer = buffer;
      _readWhileEqualFieldIndex = readWhileEqualToFieldIndex;
    }

    public bool IsLastBlock { get; private set; }

    public bool IsEnd { get { return _streamReader.EndOfStream && _recordAhead == null;}}

    public IEnumerable<RecordsBuffer> ReadBlocks()
    {
      IsLastBlock = false;
      string line = null;
      RecordsBuffer recordsBuffer = new RecordsBuffer(_buffer, _recordBytesConverter);
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
          if (recordsBuffer.RecordsCount == 0)
            throw new InvalidOperationException("Buffer is too small. Can't add even one record.");

          yield return recordsBuffer;
          recordsBuffer = new RecordsBuffer(_buffer, _recordBytesConverter);
          recordsBuffer.AddRecord(record);
        }
      }

      if (recordsBuffer.RecordsCount > 0)
      {
        IsLastBlock = true;
        yield return recordsBuffer;
      }
    }

    public IEnumerable<RecordsBuffer> ReadBlocksRaw()
    {
      RecordsBuffer recordsBuffer = new RecordsBuffer(_buffer, _recordBytesConverter);
      long recordLen = 0;
      while (ReadValue(recordsBuffer.SizeOfRecordLengthType, out recordLen))
      {
        bool added = recordsBuffer.AddRecordBytes(_stream, recordLen);
        if (!added)
        {
          if (recordsBuffer.RecordsCount == 0)
            throw new InvalidOperationException("Buffer is too small. Can't add even one record.");

          yield return recordsBuffer;
          recordsBuffer = new RecordsBuffer(_buffer, _recordBytesConverter);
          recordsBuffer.AddRecordBytes(_stream, recordLen);
        }
      }

      if (recordsBuffer.RecordsCount > 0)
        yield return recordsBuffer;
    }

    private bool ReadValue(int sizeOfValue, out long value)
    {
      value = 0;
      for (int i = 0; i < sizeOfValue; i++)
      {
        value <<= 8;
        int b = _stream.ReadByte();
        if (b == -1)
          return false;
        value += (int) b;
      }
      return true;
    }

    public IEnumerable<ArrayView<byte>> ReadRecordBytes()
    {
      IEnumerable<RecordsBuffer> blocks = ReadBlocksRaw();
      foreach (RecordsBuffer block in blocks)
      {
        for (long i = 0; i < block.RecordsCount; i++)
        {
          yield return block.GetRecordBytes(i);
        }
      }
    }
  }
}
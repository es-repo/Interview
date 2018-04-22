using System.Collections.Generic;
using System.IO;

namespace Altium.BigSorter
{
  public class RecordsWriter
  {
    private readonly IRecordParser _recordParser;
    private readonly Stream _stream;
    private readonly TextWriter _textWriter;

    public RecordsWriter(IRecordParser recordParser, Stream stream)
    {
      _recordParser = recordParser;
      _stream = stream;
    }

    public RecordsWriter(IRecordParser recordParser, TextWriter textWriter)
    {
      _recordParser = recordParser;
      _textWriter = textWriter;
    }

    public void WriteRecordsRaw(RecordsBuffer recordsBuffer)
    {
      for (long i = 0; i < recordsBuffer.RecordsCount; i++)
      {
        ArrayView<byte> recordBytes = recordsBuffer.GetRecordBytes(i);
        WriteRecordBytes(recordBytes, recordsBuffer.SizeOfRecordLengthType);
      }
    }

    private void WriteRecordBytes(ArrayView<byte> recordBytes, int sizeOfRecordLen)
    {
      byte[] lenBytes = new byte[sizeOfRecordLen];
      ValueToBytes(recordBytes.Length, lenBytes);
      for (int i = 0; i < lenBytes.Length; i++)
      {
        _stream.WriteByte(lenBytes[i]);
      }

      for (long i = 0; i < recordBytes.Length; i++)
      {
        _stream.WriteByte(recordBytes[i]);
      }
    }

    private void ValueToBytes(long value, byte[] bytes)
    {
      for (int i = bytes.Length - 1; i >= 0; i--)
      {
        bytes[i] = (byte) value;
        value >>= 8;
      }
    }

    public void WriteRecords(RecordsBuffer buffer)
    {
      for (long i = 0; i < buffer.RecordsCount; i++)
      {
        IEnumerable<object> values = buffer.GetRecord(i);
        string recordString = _recordParser.ToString(values);
        _textWriter.WriteLine(recordString);
      }
    }
  }
}
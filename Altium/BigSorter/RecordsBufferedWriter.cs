using System;
using System.IO;

namespace Altium.BigSorter
{
  public class RecordsBufferedWriter
  {
    private readonly RecordsWriter _recordsWriter;
    private readonly ArrayView<byte> _buffer;
    private RecordsBuffer _recordsBuffer;
    private readonly IRecordBytesConverter _recordBytesConverter;

    public RecordsBufferedWriter(IRecordParser recordParser, IRecordBytesConverter recordBytesConverter,
      TextWriter textWriter, ArrayView<byte> buffer)
    {
      _recordsWriter = new RecordsWriter(recordParser, textWriter);
      _buffer = buffer;
      _recordsBuffer = new RecordsBuffer(buffer, recordBytesConverter);
      _recordBytesConverter = recordBytesConverter;
    }

    public void WriteRecord(ArrayView<byte> recordBytes)
    {
      bool added = _recordsBuffer.AddRecordBytes(recordBytes);
      if (!added)
      {
        if (_recordsBuffer.RecordsCount == 0)
          throw new InvalidOperationException("Buffer is too small. Can't add even one record.");

        Flush();
        _recordsBuffer = new RecordsBuffer(_buffer, _recordBytesConverter);
        added = _recordsBuffer.AddRecordBytes(recordBytes);
        if (!added)
          throw new InvalidOperationException("Can't add record.");
      }
    }

    public void Flush()
    {
      _recordsWriter.WriteRecords(_recordsBuffer);
    }
  }
}
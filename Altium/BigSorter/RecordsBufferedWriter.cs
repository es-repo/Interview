using System;
using System.IO;

namespace Altium.BigSorter
{
  public class RecordsBufferedWriter
  {
    private readonly RecordsWriter _recordsWriter;
    private readonly long _bufferSizeInBytes;    
    private RecordsBuffer _recordsBuffer;

    public RecordsBufferedWriter(IRecordParser recordParser,
      TextWriter textWriter, long bufferSizeInBytes)
    {
      _recordsWriter = new RecordsWriter(recordParser, textWriter);
      _bufferSizeInBytes = bufferSizeInBytes;
      _recordsBuffer = new RecordsBuffer(bufferSizeInBytes);
    }

    public void WriteRecord(object[] values)
    {
      bool added = _recordsBuffer.AddRecord(values);
      if (!added)
      {
        if (_recordsBuffer.Records.Count == 0)
          throw new InvalidOperationException("Buffer is too small. Can't add even one record.");

        Flush();
        _recordsBuffer = new RecordsBuffer(_bufferSizeInBytes);
        added = _recordsBuffer.AddRecord(values);
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
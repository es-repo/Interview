using System;
using System.IO;

namespace Altium.BigSorter
{
  public class BufferedRecordsWriter : IDisposable
  {
    private readonly RecordsWriter _recordsWriter;
    private readonly long _bufferSizeInBytes;
    private RecordsBuffer _recordsBuffer;

    public BufferedRecordsWriter(TextWriter textWriter, long bufferSizeInBytes)
    {
      _recordsWriter = new RecordsWriter(textWriter);
      _bufferSizeInBytes = bufferSizeInBytes;
      _recordsBuffer = new RecordsBuffer(bufferSizeInBytes);
    }

    public void WriteRecord(Record record)
    {
      bool added = _recordsBuffer.AddRecord(record);
      if (!added)
      {
        if (_recordsBuffer.Records.Count == 0)
          throw new InvalidOperationException("Buffer is too small. Can't add even one record.");

        Flush();
        _recordsBuffer = new RecordsBuffer(_bufferSizeInBytes);
        _recordsBuffer.AddRecord(record);
      }
    }

    public void Flush()
    {
      _recordsWriter.WriteRecords(_recordsBuffer);
    }

    public void Dispose()
    {
      Flush();
    }
  }
}
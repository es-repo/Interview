using System;
using System.Collections.Generic;
using System.IO;

namespace Altium.BigSorter
{
  public class BufferedRecordsWriter : IDisposable
  {
    private ArrayView<byte> _bufferView;
    private Stream _stream;
    private readonly RecordsWriter _recordsWriter;
    private RecordsBuffer _recordsBuffer;
    

    public BufferedRecordsWriter(ArrayView<byte> bufferView, Stream stream)
    {
      _bufferView = bufferView;
      _stream = stream;
      _recordsWriter = new RecordsWriter(stream);
      _recordsBuffer = new RecordsBuffer(_bufferView, new List<Record>(), null);
    }

    public void WriteRecord(Record recordInfo)
    {
      bool added = _recordsBuffer.AddRecord(recordInfo);
      if (added)
        return;

      Flush();
      _recordsBuffer = new RecordsBuffer(_bufferView, new List<Record>(), null);
      _recordsBuffer.AddRecord(recordInfo);
    }

    public void Flush()
    {
      _recordsWriter.Write(_recordsBuffer);
    }

    public void Dispose()
    {
      Flush();
    }
  }
}
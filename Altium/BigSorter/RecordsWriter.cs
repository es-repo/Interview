using System.IO;

namespace Altium.BigSorter
{
  public class RecordsWriter
  {
    private readonly Stream _stream;

    public RecordsWriter(Stream stream)
    {
      _stream = stream;
    }

    public void Write(RecordsBuffer recordsBuffer)
    {
      for (int i = 0; i < recordsBuffer.RecordsInfo.Count; i++)
      {
        RecordInfo ri = recordsBuffer.RecordsInfo[i];
        _stream.Write(recordsBuffer.BufferView.Array, ri.Start, ri.Length);
      }
    }
  }
}
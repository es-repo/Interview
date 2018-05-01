using System.IO;

namespace Altium.BigSorter
{
  /// <summary>
  /// Writs records to a stream.
  /// </summary>
  public class RecordsWriter
  {
    private readonly RecordParser _recordParser;
    private readonly TextWriter _textWriter;

    public RecordsWriter(TextWriter textWriter)
    {
      _recordParser = new RecordParser();
      _textWriter = textWriter;
    }

    /// <summary>
    /// Writes all records from a records buffer.
    /// </summary>
    public void WriteRecords(RecordsBuffer buffer)
    {
      for (int i = 0; i < buffer.Records.Count; i++)
      {
        Record record = buffer.Records[i];
        string recordString = _recordParser.ToString(record);
        _textWriter.Write(recordString);
        _textWriter.Write("\r\n");
      }
    }
  }
}
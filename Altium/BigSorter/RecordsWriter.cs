using System.IO;

namespace Altium.BigSorter
{
  public class RecordsWriter
  {
    private readonly RecordParser _recordParser;
    private readonly TextWriter _textWriter;

    public RecordsWriter(TextWriter textWriter)
    {
      _recordParser = new RecordParser();
      _textWriter = textWriter;
    }

    public void WriteRecords(RecordsBuffer buffer)
    {
      for (int i = 0; i < buffer.Records.Count; i++)
      {
        Record record = buffer.Records[i];
        string recordString = _recordParser.ToString(record);
        _textWriter.Write(recordString + "\r\n");
      }
    }
  }
}
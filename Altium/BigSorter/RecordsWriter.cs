using System.Collections.Generic;
using System.IO;

namespace Altium.BigSorter
{
  public class RecordsWriter
  {
    private readonly IRecordParser _recordParser;
    private readonly TextWriter _textWriter;

    public RecordsWriter(IRecordParser recordParser, TextWriter textWriter)
    {
      _recordParser = recordParser;
      _textWriter = textWriter;
    }

    public void WriteRecords(RecordsBuffer buffer)
    {
      for (int i = 0; i < buffer.Records.Count; i++)
      {
        object[] values = buffer.Records[i];
        string recordString = _recordParser.ToString(values);
        _textWriter.WriteLine(recordString);
      }
    }
  }
}
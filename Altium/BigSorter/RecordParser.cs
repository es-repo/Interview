using System.Collections.Generic;

namespace Altium.BigSorter
{
  public class RecordParser
  {
    public List<Record> Parse(ArrayView<byte> bufferView, int bytesCount, out int bytesNotParsedStart)
    {
      bytesNotParsedStart = bufferView.Start;
      int recLen = 0;
      int stringStart = 0;
      byte[] buffer = bufferView.Array;
      int end = bufferView.Start + bytesCount;
      int number = 0;
      bool isNumberParsed = false;
      List<Record> records = new List<Record>();
      for (int i = bufferView.Start; i < end; i++)
      {
        recLen++;

        if (buffer[i] == (byte)'.' && !isNumberParsed)
        {
          number = ParseNumber(buffer, bytesNotParsedStart, i - 1);
          isNumberParsed = true;
          stringStart = i + 2;
        }
        if (buffer[i] == 0x0D && (i + 1 < end) && buffer[i + 1] == 0x0A)
        {
          i++;
          recLen++;
          records.Add(new Record(bytesNotParsedStart, recLen, number, stringStart));
          isNumberParsed = false;
          bytesNotParsedStart = i + 1;
          recLen = 0;
        }
      }

      return records;
    }

    private int ParseNumber(byte[] buffer, int start, int end)
    {
      int n = 0;
      int i = start;
      while (i <= end)
      {
        n *= 10;
        n += buffer[i];
        i++;
      }
      return n;
    }
  }
}
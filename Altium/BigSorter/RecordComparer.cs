using System.Collections.Generic;

namespace Altium.BigSorter
{
  public class RecordComparer : IComparer<ArrayView<byte>>
  {
    private readonly IRecordBytesConverter _recordBytesConverter;
    private readonly int _fieldIndex;

    public RecordComparer(IRecordBytesConverter recordBytesConverter, int fieldIndex)
    {
      _recordBytesConverter = recordBytesConverter;
      _fieldIndex = fieldIndex;
    }

    public int Compare(ArrayView<byte> recordA, ArrayView<byte> recordB)
    {
      ArrayView<byte> aValueBytes = _recordBytesConverter.GetValueBytes(recordA, _fieldIndex);
      ArrayView<byte> bValueBytes = _recordBytesConverter.GetValueBytes(recordB, _fieldIndex);
      long minLen = aValueBytes.Length < bValueBytes.Length ? aValueBytes.Length : bValueBytes.Length;
      for (long i = 0; i < minLen; i++)
      {
        if (aValueBytes[i] < bValueBytes[i])
          return -1;

        if (aValueBytes[i] > bValueBytes[i])
          return 1;
      }

      if (aValueBytes.Length < bValueBytes.Length)
        return -1;

      if (aValueBytes.Length > bValueBytes.Length)
        return 1;

      return 0;
    }
  }
}
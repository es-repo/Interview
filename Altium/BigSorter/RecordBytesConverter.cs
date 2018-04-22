using System;
using System.Collections.Generic;

namespace Altium.BigSorter
{
  public class RecordBytesConverter : IRecordBytesConverter
  {
    private readonly ByteConverter _byteConverter = new ByteConverter();

    public ArrayView<byte> GetValueBytes(ArrayView<byte> recordBytes, int fieldIndex)
    {
      switch (fieldIndex)
      {
        case 0:
          // int
          return new ArrayView<byte>(recordBytes, 0, sizeof(int));
        case 1:
          // string
          return new ArrayView<byte>(recordBytes, sizeof(int));

        default:
          throw new NotImplementedException();
      }
    }

    public object GetValue(ArrayView<byte> recordBytes, int fieldIndex)
    {
      switch (fieldIndex)
      {
        case 0:
          ArrayView<byte> intBytes = GetValueBytes(recordBytes, 0);
          return _byteConverter.To(typeof(int), intBytes);

        case 1:
          ArrayView<byte> stringBytes = GetValueBytes(recordBytes, 1);
          return _byteConverter.To(typeof(string), stringBytes);

        default:
          throw new NotImplementedException();
      }
    }

    public IEnumerable<object> GetValues(ArrayView<byte> recordBytes)
    {
      // int
      yield return GetValue(recordBytes, 0);
      // string
      yield return GetValue(recordBytes, 1);
    }

    public long SetBytes(IEnumerable<object> values, ArrayView<byte> recordBytes)
    {
      long totalLen = 0;
      int fieldIndex = 0;
      foreach (object v in values)
      {
        long valueLen = GetValueLength(v, fieldIndex);
        if (totalLen + valueLen > recordBytes.Length)
          return 0;

        ArrayView<byte> valueBytes = new ArrayView<byte>(recordBytes, totalLen, valueLen);
        _byteConverter.ToBytes(v, valueBytes);
        totalLen += valueLen;
        fieldIndex++;
      }
      return totalLen;
    }

    private long GetValueLength(object value, int fieldIndex)
    {
      switch(fieldIndex)
      {
        case 0: return sizeof(int);
        case 1: return (long)((string)value).Length * sizeof(char);
        default: throw new NotImplementedException();
      }
    }
  }
}
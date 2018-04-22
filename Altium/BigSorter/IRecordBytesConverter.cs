using System.Collections.Generic;

namespace Altium.BigSorter
{
  public interface IRecordBytesConverter
  {
    long SetBytes(IEnumerable<object> values, ArrayView<byte> recordBytes);

    IEnumerable<object> GetValues(ArrayView<byte> recordBytes);

    object GetValue(ArrayView<byte> recordBytes, int fieldIndex);

    ArrayView<byte> GetValueBytes(ArrayView<byte> recordBytes, int fieldIndex);
  }
}
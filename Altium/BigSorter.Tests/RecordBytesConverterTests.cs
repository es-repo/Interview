using System.Collections.Generic;
using Xunit;

namespace Altium.BigSorter.Tests
{
  public class RecordBytesConverterTests
  {
    private readonly RecordBytesConverter _converter = new RecordBytesConverter();

    [Theory]
    [InlineData(new object[]
      {
        0x00ab00cd,
        "Hello"
      },
      14,
      14,
      new byte[] { 0, 0xab, 0, 0xcd, 0, (byte) 'H', 0, (byte) 'e', 0, (byte) 'l', 0, (byte) 'l', 0, (byte) 'o' })]
    [InlineData(new object[]
      {
        0x00ab00cd,
        "Hello"
      },
      13,
      0,
      new byte[] { })]

    public void TestWriteValues(object[] values, long bufferLength,
      long expectedRecordLen, byte[] expectedBytes)
    {
      BigArray<byte> buffer = new BigArray<byte>(bufferLength);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0, expectedRecordLen);

      long recordLen = _converter.SetBytes(values, av);
      Assert.Equal(expectedRecordLen, recordLen);
      if (recordLen != 0)
        Assert.Equal(expectedBytes, av.Enumerate());
    }

    [Theory]
    [InlineData(
      new byte[] { 0, 0xab, 0, 0xcd, 0, (byte) 'H', 0, (byte) 'e', 0, (byte) 'l', 0, (byte) 'l', 0, (byte) 'o' },
      new object[]
      {
        0x00ab00cd,
        "Hello"
      })]
    public void TestReadValues(byte[] recordBytes, object[] expectedValues)
    {
      BigArray<byte> buffer = new BigArray<byte>((long) recordBytes.Length);
      for (int i = 0; i < recordBytes.Length; i++)
      {
        buffer[(long) i] = recordBytes[i];
      }

      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);

      for (int i = 0; i < expectedValues.Length; i++)
      {
        object v = _converter.GetValue(av, i);
        Assert.Equal(expectedValues[i], v);
      }

      IEnumerable<object> values = _converter.GetValues(av);
      Assert.Equal(expectedValues, values);
    }
  }
}
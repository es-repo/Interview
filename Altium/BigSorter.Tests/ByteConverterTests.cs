using Altium.BigSorter;
using Xunit;

namespace Altium.BigSorter.Tests
{
  public class ByteConverterTests
  {
    private readonly ByteConverter _converter = new ByteConverter();

    [Fact]
    public void TestInt32ToBytes()
    {
      BigArray<byte> buffer = new BigArray<byte>(4);
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);

      _converter.ToBytes(0xaf, av);
      Assert.Equal(new byte[] { 0, 0, 0, 0xaf }, buffer.Enumerate());

      _converter.ToBytes(0x003300af, av);
      Assert.Equal(new byte[] { 0, 0x33, 0, 0xaf }, buffer.Enumerate());
    }

    [Fact]
    public void TestStringToBytes()
    {
      string s = "Hello";
      BigArray<byte> buffer = new BigArray<byte>((long) s.Length * sizeof(char));
      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);

      _converter.ToBytes(s, av);
      Assert.Equal(new byte[] { 0, (byte) 'H', 0, (byte) 'e', 0, (byte) 'l', 0, (byte) 'l', 0, (byte) 'o', }, buffer.Enumerate());
    }

    [Fact]
    public void TestToInt32()
    {
      BigArray<byte> buffer = new BigArray<byte>(4);
      byte[] bytes = new byte[] { 0, 0x33, 0, 0xaf };
      for (int i = 0; i < bytes.Length; i++)
        buffer[(long) i] = bytes[i];

      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);

      int v = _converter.ToInt32(av);
      Assert.Equal(0x003300af, v);
    }

    [Fact]
    public void TestToString()
    {
      byte[] bytes = new byte[] { 0, (byte) 'H', 0, (byte) 'e', 0, (byte) 'l', 0, (byte) 'l', 0, (byte) 'o' };
      BigArray<byte> buffer = new BigArray<byte>((long) bytes.Length);
      for (int i = 0; i < bytes.Length; i++)
        buffer[(long) i] = bytes[i];

      ArrayView<byte> av = new ArrayView<byte>(buffer, 0);

      string v = _converter.ToString(av);
      Assert.Equal("Hello", v);
    }
  }
}
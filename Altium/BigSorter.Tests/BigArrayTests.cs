using Xunit;

namespace Altium.BigSorter.Tests
{
  public class BigArrayTests
  {
    [Fact]
    public void Test()
    {
      BigArray<byte> a = new BigArray<byte>(5); 
      for (long i = 0; i < a.Length; i++)
        a[i] = (byte)(i * i);

      Assert.Equal(5, a.Length);
      Assert.Equal(9, a[3]);
    }
  }
}
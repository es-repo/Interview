using Xunit;
using Altium.BigSorter;

namespace Altium.BigSorter.Tests
{
  public class ArrayViewTests
  {
    private static BigArray<byte> CreateFilledArray(long length = 5)
    {
      BigArray<byte> array = new BigArray<byte>(length); 
      for (long  i = 0; i < array.Length; i++)
        array[i] = (byte)(i * i);
      return array;
    }

    [Fact]
    public void Test()
    {
      BigArray<byte> a = CreateFilledArray();

      ArrayView<byte> av = new ArrayView<byte>(a, 2);
      Assert.Equal(3, av.Length);
      Assert.Equal(4, av[0]);
      Assert.Equal(16, av[2]);

      av[2] = 25;
      Assert.Equal(25, a[4]);

      av = new ArrayView<byte>(a, 1, 2);
      Assert.Equal(2, av.Length);
      Assert.Equal(1, av[0]); 
    }

    [Fact]
    public void TestWrapped()
    {
      BigArray<byte> a = CreateFilledArray(6);

      ArrayView<byte> av = new ArrayView<byte>(a, 1, 4);
      ArrayView<byte> avWrapper = new ArrayView<byte>(av, 1, 2);
      
      Assert.Equal(2, avWrapper.Length);
      Assert.Equal(4, avWrapper[0]);
      Assert.Equal(9, avWrapper[1]);

      avWrapper[1] = 100;
      Assert.Equal(100, a[3]);
    }
  }
}
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Altium.BigSorter.Tests
{
  public class MinHeapTests
  {
    [Fact]
    public void Test()
    {
      MinHeap<int> heap = new MinHeap<int>(9, Comparer<int>.Default);
      heap.Add(5);
      Assert.Equal(5, heap.Root);

      heap.Add(2);
      Assert.Equal(2, heap.Root);

      heap.Add(7);
      Assert.Equal(2, heap.Root);

      heap.Add(1);
      Assert.Equal(1, heap.Root);

      heap.Add(4);
      Assert.Equal(1, heap.Root);

      Assert.Equal(5, heap.Size);

      Assert.Equal(new int[]{1, 2, 4, 5, 7}, Enumerable.Repeat(0, 5).Select(i => heap.Extract()));
    }
  }
}
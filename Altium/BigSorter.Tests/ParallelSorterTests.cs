using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Altium.BigSorter.Tests
{
  public class ParallelSorterTests
  {
    [Theory]
    [InlineData(
      new int[] { 5, 1, 2, 9, 3, 6, 6, 1, 9, 4, 6, 6, 9, 1 },
      4, 1,
      new [] { 1, 2, 5, 3, 6, 9, 1, 6, 9, 1, 4, 6, 6, 9 },
      new [] { 1, 1, 1, 2, 3, 4, 5, 6, 6, 6, 6, 9, 9, 9 })]

    [InlineData(
      new int[] { 5, 1, 2, 9, 3, 6, 6, 1, 9, 4, 6, 6, 9, 1 },
      2, 1,
      new [] { 1, 2, 3, 5, 6, 6, 9, 1, 1, 4, 6, 6, 9, 9 },
      new [] { 1, 1, 1, 2, 3, 4, 5, 6, 6, 6, 6, 9, 9, 9 })]

    [InlineData(
      new int[] { 5, 1, 2, 9, 3, 6, 6, 1, 9, 4, 6, 6, 9, 1 },
      6, 1,
      new [] { 1, 5, 2, 9, 3, 6, 1, 6, 4, 9, 1, 6, 6, 9 },
      new [] { 1, 1, 1, 2, 3, 4, 5, 6, 6, 6, 6, 9, 9, 9 })]

    [InlineData(
      new int[] { 5, 1, 2, 9, 3, 6, 6, 1, 9, 4, 6, 6, 9, 1 },
      4, 4,
      new [] { 1, 1, 1, 2, 3, 4, 5, 6, 6, 6, 6, 9, 9, 9 },
      new [] { 1, 1, 1, 2, 3, 4, 5, 6, 6, 6, 6, 9, 9, 9 })]
    public void Test(int[] input, int chunksCount, int threshold, int[] expectedChunksSorted, int[] expectedSorted)
    {
      List<int> list = input.ToList();
      List<int> sortedList = ParallelSorter.Sort(list, Comparer<int>.Default, chunksCount, threshold);
      Assert.Equal(expectedChunksSorted, list);
      Assert.Equal(expectedSorted, sortedList);
    }
  }
}
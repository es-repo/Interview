using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Altium.BigSorter
{
  /// <summary>
  /// Sorts a list spreading loading over multiple threads.
  /// </summary>
  public static class ParallelSorter
  {
    /// <summary>
    /// Sorts a list spreading loading over multiple threads.
    /// Each thread does a quick sort of specific chunk of the list.  
    /// Then sorted chunks are merged into one fully sorted list.
    /// </summary>
    /// <param name="list">A list to sort.</param>
    /// <param name="comparer">A comparer to compare list elements.</param>
    /// <param name="chunkCount">
    /// Count of chunks onto which a list should be partitioned.
    /// Every chunk is sorted in separate thread.
    /// </param>
    /// <param name="threshold">
    /// Minimum size of list to run sort in parallel mode.
    /// If list size is less then the threshold then the list will be sorted sequentially.
    /// </param>
    /// <returns></returns>
    public static List<T> ParallelSort<T>(this List<T> list, IComparer<T> comparer, int chunkCount, int threshold)
    {
      if (chunkCount < 2 || list.Count <= threshold)
      {
        list.Sort(comparer);
        return list;
      }

      int chunkSize = list.Count / chunkCount;
      int lastChunkSize = chunkSize + list.Count % chunkCount;
      Parallel.For(0, chunkCount,
        i =>
        {
          list.Sort(i * chunkSize, i == chunkCount - 1 ? lastChunkSize : chunkSize, comparer);
        });

      return Merge(list, chunkCount, chunkSize, comparer);
    }

    public static List<T> ParallelSort<T>(this List<T> list, IComparer<T> comparer)
    {
      return ParallelSort(list, comparer, Environment.ProcessorCount, 4096);
    }

    private static List<T> Merge<T>(List<T> list, int chunkCount, int chunkSize, IComparer<T> comparer)
    {
      int[] chunkIndexes = new int[chunkCount];
      int[] chunkEnds = new int[chunkCount];
      for (int i = 0; i < chunkCount; i++)
      {
        chunkIndexes[i] = i * chunkSize;
        chunkEnds[i] = i * chunkSize + chunkSize;
      }
      chunkEnds[chunkEnds.Length - 1] = list.Count;

      MinHeap<int> heap = new MinHeap<int>(chunkCount,
        new SelectComparer<int, T>(i => list[chunkIndexes[i]], comparer));

      for (int i = 0; i < chunkCount; i++)
        heap.Add(i);

      List<T> result = new List<T>(list.Count);
      for (int i = 0; i < list.Count; i++)
      {
        result.Add(list[chunkIndexes[heap.Root]]);

        chunkIndexes[heap.Root]++;
        if (chunkIndexes[heap.Root] == chunkEnds[heap.Root])
        {
          heap.Extract();
        }
        else
        {
          heap.DownHeap();
        }
      }

      return result;
    }
  }
}
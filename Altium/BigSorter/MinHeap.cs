using System.Collections.Generic;

namespace Altium.BigSorter
{
  /// <summary>
  /// Implementation for min binary heap.
  /// Supports only operations necessary for this test task. 
  /// </summary>
  public class MinHeap<T>
  {
    private T[] _array;
    private readonly IComparer<T> _comparer;

    public MinHeap(int capacity, IComparer<T> comparer)
    {
      _array = new T[capacity + 1];
      _comparer = comparer;
    }

    /// <summary>
    /// Number of elements in the heap.
    /// </summary>
    public int Size { get; private set; }

    /// <summary>
    /// Root element.
    /// </summary>
    public T Root { get { return _array[1]; } }

    /// <summary>
    /// Adds element to the heap.
    /// </summary>
    /// <param name="elem"></param>
    public void Add(T elem)
    {
      _array[++Size] = elem;
      UpHeap();
    }

    /// <summary>
    /// Removes root element from the heap and returns it.
    /// </summary>
    public T Extract()
    {
      T elem = Root;
      _array[1] = _array[Size];
      Size--;
      DownHeap();
      return elem;
    } 

    /// <summary>
    /// Rebalances the heap from the bottom.
    /// </summary>
    public void UpHeap()
    {
      int i = Size;
      int p = i >> 1;
      while (p != 0 && _comparer.Compare(_array[i], _array[p]) < 0)
      {
        T t = _array[i];
        _array[i] = _array[p];
        _array[p] = t;
        i = p;
        p >>= 1;
      }
    }

    /// <summary>
    /// Rebalances the heap from the top.
    /// </summary>
    public void DownHeap()
    {
      int i = 1;
      while (true)
      {
        int min = i;
        int ch = i << 1;
        if (ch <= Size)
        {
          if (_comparer.Compare(_array[min], _array[ch]) > 0)
            min = ch;

          ch++;
          if (ch <= Size)
          {
            if (_comparer.Compare(_array[min], _array[ch]) > 0)
              min = ch;
          }
        }
        if (i != min)
        {
          T t = _array[i];
          _array[i] = _array[min];
          _array[min] = t;
          i = min;
        }
        else break;
      }
    }
  }
}
using System.Collections.Generic;

namespace Altium.BigSorter
{
  public class MinHeap<T>
  {
    public T[] _array;
    private readonly IComparer<T> _comparer;

    public MinHeap(int capacity, IComparer<T> comparer)
    {
      _array = new T[capacity + 1];
      _comparer = comparer;
    }

    public int Size { get; set; }

    public T Root { get { return _array[1]; } }

    public void Add(T elem)
    {
      _array[++Size] = elem;
      UpHeap();
    }

    public T Extract()
    {
      T elem = Root;
      _array[1] = _array[Size];
      Size--;
      DownHeap();
      return elem;
    } 

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
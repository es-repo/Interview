using System;
using System.Collections.Generic;

namespace Altium.BigSorter
{
  public class ArrayView<T>
  {
    private readonly BigArray<T> _array;
    private readonly long _start;

    public long Length { get; private set; }

    public ArrayView(BigArray<T> array, long start, long length = 0)
    {
      if (start + length > array.Length)
        throw new ArgumentException();

      _array = array;
      _start = start;
      Length = length == 0 ? array.Length - start : length;
    }

    public ArrayView(ArrayView<T> arrayView, long start, long length = 0) : this(arrayView._array, arrayView._start + start,
      length == 0 ? arrayView.Length - start : length) { }

    public T this [long index]
    {
      get
      {
        return _array[_start + index];
      }
      set
      {
        _array[_start + index] = value;
      }
    }

    public IEnumerable<T> Enumerate()
    {
      for (long i = 0; i < Length; i++)
        yield return this [i];
    }
  }
}
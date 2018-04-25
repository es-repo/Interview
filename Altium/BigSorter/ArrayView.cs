using System;
using System.Collections.Generic;

namespace Altium.BigSorter
{
  public class ArrayView<T>
  {
    public readonly T[] Array;
    public readonly int Start;

    public readonly int Length;

    public ArrayView(T[] array, int start, int length = 0)
    {
      if (start + length > array.Length)
        throw new ArgumentException();

      Array = array;
      Start = start;
      Length = length == 0 ? array.Length - start : length;
    }

    public ArrayView(ArrayView<T> arrayView, int start, int length = 0) : this(arrayView.Array, arrayView.Start + start,
      length == 0 ? arrayView.Length - start : length) { }

    public T this [int index]
    {
      get
      {
        return Array[Start + index];
      }
      set
      {
        Array[Start + index] = value;
      }
    }

    public IEnumerable<T> Enumerate()
    {
      for (int i = 0; i < Length; i++)
        yield return this [i];
    }
  }
}
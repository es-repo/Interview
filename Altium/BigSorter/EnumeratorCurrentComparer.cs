using System.Collections.Generic;

namespace Altium.BigSorter
{
  public class EnumeratorCurrentComparer<T> : IComparer<IEnumerator<T>>
  {
    private readonly IComparer<T> _elementComparer;

    public EnumeratorCurrentComparer(IComparer<T> elementComparer)
    {
      _elementComparer = elementComparer;
    }

    public int Compare(IEnumerator<T> x, IEnumerator<T> y)
    {
      return _elementComparer.Compare(x.Current, y.Current);
    }
  }
}
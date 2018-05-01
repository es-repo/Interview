using System;
using System.Collections.Generic;

namespace Altium.BigSorter
{
  /// <summary>
  /// A comparer to compare elements's properties.
  /// Accepts in constructor a selector function to access element's property
  /// and a comparer to compare values of these properties. 
  /// </summary>
  public class SelectComparer<K, T> : IComparer<K>
  {
    private readonly Func<K, T> _select;
    private readonly IComparer<T> _comparer;

    public SelectComparer(Func<K, T> select, IComparer<T> comparer)
    {
      _select = select;
      _comparer = comparer;
    }

    public int Compare(K x, K y)
    {
      return _comparer.Compare(_select(x), _select(y));
    }
  }
}
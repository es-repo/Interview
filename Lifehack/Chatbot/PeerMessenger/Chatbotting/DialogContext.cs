using System;
using System.Collections.Generic;

namespace PeerMessenger.Chatbotting
{
  public class DialogContext
  {
    private readonly Dictionary<Type, object> _items;

    public DialogContext()
    {
      _items = new Dictionary<Type, object>();
    }

    public T GetItem<T>()
    {
      Type t = typeof(T);
      return _items.ContainsKey(t) ? (T)_items[t] : default;
    }

    public bool HasItem<T>()
    {
      return GetItem<T>() != null;
    }

    public void Upsert<T>(T item)
    {
      Type t = item.GetType();
      if (_items.ContainsKey(t))
      {
        _items[t] = item;
      }
      else
      {
        _items.Add(t, item);
      }
    }
  }
}
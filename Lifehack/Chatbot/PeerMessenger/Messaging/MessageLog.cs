using System;
using System.Collections.Generic;

namespace PeerMessenger.Messaging
{
  public class MessageLog
  {
    private readonly List<Message> _list;

    public IReadOnlyList<Message> Messages { get => _list; }

    public event EventHandler<Message> MessageAdded;

    public MessageLog()
    {
      _list = new List<Message>();
      MessageAdded += (s, a) => { };
    }

    public void Add(Message message)
    {
      _list.Add(message);
      MessageAdded(this, message);
    }
  }
}
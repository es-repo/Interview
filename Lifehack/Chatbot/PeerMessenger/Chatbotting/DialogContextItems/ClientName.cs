using System;

namespace PeerMessenger.Chatbotting.DialogContextItems
{
  public class ClientName
  {
    public string Value { get; }
    public bool IsAnonymous { get => string.IsNullOrWhiteSpace(Value); }

    public ClientName(string value)
    {
      Value = String.IsNullOrWhiteSpace(value) ? null : value;
    }
  }
}
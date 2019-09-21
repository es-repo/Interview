using System;

namespace PeerMessenger.Messaging.MessengerCommands
{
  public class MessengerCommandParserException : Exception
  {
    public MessengerCommandParserException(string message) : base(message) { }
  }
}
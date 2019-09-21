using PeerMessenger.Messaging.MessengerCommands;

namespace PeerMessenger.Messaging.MessengerCommands
{
  public interface IMessengerCommandParser
  {
    MessengerCommand Parse(string input);
  }
}
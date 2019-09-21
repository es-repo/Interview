using System;
using System.Threading.Tasks;

namespace PeerMessenger.Messaging.MessengerCommands
{
  public abstract class MessengerCommand
  {
    public abstract Task AcceptHandlerAsync(IMessengerCommandHandler handler);
  }
}

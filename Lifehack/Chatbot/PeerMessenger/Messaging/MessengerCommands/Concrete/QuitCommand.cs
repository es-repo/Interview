using System.Threading.Tasks;

namespace PeerMessenger.Messaging.MessengerCommands.Concrete
{
  public class QuitCommand : MessengerCommand
  {
    public override Task AcceptHandlerAsync(IMessengerCommandHandler handler)
    {
      return handler.HandleAsync(this);
    }
  }
}

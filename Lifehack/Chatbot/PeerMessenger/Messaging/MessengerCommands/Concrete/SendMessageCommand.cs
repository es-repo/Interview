using System.Threading.Tasks;

namespace PeerMessenger.Messaging.MessengerCommands.Concrete
{
  public class SendMessageCommand : MessengerCommand
  {
    public string Text { get; private set; }

    public SendMessageCommand(string text)
    {
      Text = text;
    }

    public override Task AcceptHandlerAsync(IMessengerCommandHandler handler)
    {
      return handler.HandleAsync(this);
    }
  }
}

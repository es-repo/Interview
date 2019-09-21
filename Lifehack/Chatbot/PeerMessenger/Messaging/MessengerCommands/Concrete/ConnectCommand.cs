using System.Threading.Tasks;

namespace PeerMessenger.Messaging.MessengerCommands.Concrete
{
  public class ConnectCommand : MessengerCommand
  {
    public string Host { get; private set; }
    public int Port { get; private set; }

    public ConnectCommand(int port)
    {
      Host = "127.0.0.1";
      Port = port;
    }

    public override Task AcceptHandlerAsync(IMessengerCommandHandler handler)
    {
      return handler.HandleAsync(this);
    }
  }
}

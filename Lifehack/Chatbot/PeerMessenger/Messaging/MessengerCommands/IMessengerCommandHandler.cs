using System.Threading.Tasks;
using PeerMessenger.Messaging.MessengerCommands.Concrete;

namespace PeerMessenger.Messaging.MessengerCommands
{
  public interface IMessengerCommandHandler
  {
    Task HandleAsync(QuitCommand command);

    Task HandleAsync(ConnectCommand command);

    Task HandleAsync(HelpCommand command);

    Task HandleAsync(SendMessageCommand command);
  }
}
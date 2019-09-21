using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PeerMessenger.Messaging.MessengerCommands.Concrete;

namespace PeerMessenger.Messaging.MessengerCommands
{
  public class MessengerCommandHandler : IMessengerCommandHandler
  {
    private readonly ITcpPeerMessenger _messenger;
    private readonly MessageLog _messageLog;
    private IPEndPoint _lastConnectedEndPoint;

    public MessengerCommandHandler(ITcpPeerMessenger messenger, MessageLog messageLog)
    {
      _messenger = messenger;
      _messageLog = messageLog;

      _messenger.ClientAccepted += (s, endPoint) => { _lastConnectedEndPoint = endPoint; };
    }

    public Task HandleAsync(QuitCommand command)
    {
      _messenger.Stop();
      return Task.CompletedTask;
    }

    public async Task HandleAsync(ConnectCommand command)
    {
      IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(command.Host), command.Port);
      try
      {
        await _messenger.ConnectToAsync(endPoint);
        _lastConnectedEndPoint = endPoint;
        AddSystemMessage($"Connected to {endPoint.Port}");
      }
      catch (SocketException)
      {
        AddSystemMessage($"Can't connect to {endPoint.Port}");
      }
    }

    public Task HandleAsync(HelpCommand command)
    {
      string help =
        @"
Command usage: /[command name] [command arguments]

Commands:
 - connect [port number]      Connect to a peer.
 - help                       Show this help.
 - quit                       Quit from the messenger.
";
      AddSystemMessage(help);
      return Task.CompletedTask;
    }

    public Task HandleAsync(SendMessageCommand command)
    {
      if (_lastConnectedEndPoint == null)
      {
        AddSystemMessage("Not connected to any peer. Connect to a peer first to send messages");
        return Task.CompletedTask;
      }

      return _messenger.SendMessageAsync(_lastConnectedEndPoint, command.Text);
    }

    private void AddSystemMessage(string text)
    {
      _messageLog.Add(new SystemMessage(text));
    }
  }
}
using System;
using PeerMessenger.Messaging.MessengerCommands;
using PeerMessenger.Messaging.MessengerCommands.Concrete;

namespace PeerMessenger.Messaging.MessengerCommands
{
  public class MessengerCommandParser : IMessengerCommandParser
  {
    public MessengerCommand Parse(string input)
    {
      if (!input.StartsWith('/'))
      {
        return new SendMessageCommand(input);
      }

      string[] parts = input.Substring(1).Split(" ", StringSplitOptions.RemoveEmptyEntries);
      if (parts.Length < 1)
      {
        throw new MessengerCommandParserException("Invalid input");
      }

      switch (parts[0])
      {
        case "quit":
          return new QuitCommand();
        case "connect":
          return CreateConnectCommand(parts);
        case "help":
          return new HelpCommand();
        default:
          throw new MessengerCommandParserException("Unknown command");
      }
    }

    private static ConnectCommand CreateConnectCommand(string[] inputParts)
    {
      if (inputParts.Length < 2)
      {
        throw new MessengerCommandParserException("Port is not specified");
      }

      if (TryParsePort(inputParts[1], out int port))
      {
        return new ConnectCommand(port);
      }
      else
      {
        throw new MessengerCommandParserException("Invalid port");
      }
    }

    private static bool TryParsePort(string v, out int port)
    {
      if (int.TryParse(v, out port))
      {
        return port > 0 && port < 65536;
      }

      return false;
    }
  }
}
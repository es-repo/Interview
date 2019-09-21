using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PeerMessenger.Chatbotting;
using PeerMessenger.Messaging;
using PeerMessenger.Messaging.MessengerCommands;
using PeerMessenger.Presentation;

namespace PeerMessenger
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      CommandLineArguments commandLineArguments = new CommandLineArguments(args);
      var (messenger, messageLog) = CreateMessenger(commandLineArguments.Port);

      Task messengerStartTask = messenger.StartAsync();
      messageLog.Add(new SystemMessage($"Listening on port {messenger.EndPoint.Port}..."));

      if (commandLineArguments.IsChatbotMode)
      {
        new Chatbot().Start(messenger);
      }

      Task t = RenderUiAndHandleInputAsync(messenger, messageLog, commandLineArguments.IsChatbotMode);

      Task.WaitAll(messengerStartTask, t);
    }

    private static (ITcpPeerMessenger messenger, MessageLog messageLog) CreateMessenger(int port)
    {
      IMessageSerializer messageSerializer = new MessageSerializer();
      TcpPeerMessenger messenger = new TcpPeerMessenger(messageSerializer, port);
      MessageLog messageLog = new MessageLog();

      messenger.ClientAccepted += (s, endPoint) =>
      {
        messageLog.Add(new SystemMessage($"Accepted connection {endPoint.Port}"));
      };

      messenger.MessageReceived += (s, msg) =>
      {
        messageLog.Add(msg);
      };

      messenger.MessageSent += (s, msg) =>
      {
        messageLog.Add(msg);
      };

      return (messenger, messageLog);
    }

    private static async Task RenderUiAndHandleInputAsync(ITcpPeerMessenger messenger, MessageLog messageLog, bool isChatbotMode)
    {
      IMessengerCommandHandler commandHandler = new MessengerCommandHandler(messenger, messageLog);
      IMessengerCommandParser commandParser = new MessengerCommandParser();

      int headerHeight = 2;
      int splitterHeight = 1;
      int inputHeight = 5;

      Header header = new Header(isChatbotMode);

      int logViewHeight = Console.WindowHeight - headerHeight - splitterHeight - inputHeight;
      int logViewTop = headerHeight;
      MessageLogView messageLogView = new MessageLogView(logViewTop, logViewHeight, messageLog, messenger.EndPoint);

      int splitterTop = headerHeight + logViewHeight;
      HorizontalSplitter splitter = new HorizontalSplitter(splitterTop);

      int inputTop = headerHeight + splitterHeight + logViewHeight;
      MessageTextInput messageTextInput = new MessageTextInput(inputTop);
      messageTextInput.MessageEntered += async (s, input) =>
      {
        if (input != "")
        {
          try
          {
            MessengerCommand command = commandParser.Parse(input);
            await command.AcceptHandlerAsync(commandHandler);
          }
          catch (MessengerCommandParserException ex)
          {
            messageLog.Add(new SystemMessage(ex.Message));
          }
          catch (Exception e)
          {
            messenger.Stop();
            throw e;
          }
        }
      };

      var components = new List<ConsoleUiComponent>
      {
        header, messageLogView, splitter, messageTextInput
      };

      Console.Clear();
      while (messenger.IsStarted)
      {
        foreach (var c in components)
        {
          c.Render();
        }
        await Task.Delay(500);
      }
    }
  }
}
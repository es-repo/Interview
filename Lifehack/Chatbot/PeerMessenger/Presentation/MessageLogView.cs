using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using PeerMessenger.Messaging;

namespace PeerMessenger.Presentation
{
  public class MessageLogView : ConsoleUiComponent
  {
    private readonly TextView _textView;

    public MessageLogView(int top, int height, MessageLog messageLog, IPEndPoint localEndPoint) : base(0, 0)
    {
      _textView = new TextView(top, height)
      {
        Text = MessagesToString(messageLog.Messages, localEndPoint)
      };

      messageLog.MessageAdded += (s, m) =>
      {
        _textView.Text = MessagesToString(messageLog.Messages, localEndPoint);
      };
    }

    private static string MessagesToString(IEnumerable<Message> messages, IPEndPoint localEndPoint)
    {
      StringBuilder sb = new StringBuilder();
      Message prevMessage = null;
      foreach (Message m in messages)
      {
        if (prevMessage != null && prevMessage.From?.Port != m.From?.Port)
        {
          sb.Append(Environment.NewLine);
        }
        sb.AppendLine(MessageToString(m, localEndPoint));
        prevMessage = m;
      }

      return sb.ToString();
    }

    private static string MessageToString(Message message, IPEndPoint localEndPoint)
    {
      string prefix;

      bool isSystem = message is SystemMessage;
      if (isSystem)
      {
        prefix = "$: ";
      }
      else
      {
        bool isLocal = message.From?.Port == localEndPoint?.Port;
        if (!isLocal)
        {
          prefix = (message.From?.Port.ToString() ?? "") + ": ";
        }
        else
        {
          prefix = ">";
        }
      }

      return $"{prefix}{message.Body?.Trim() ?? ""}";
    }

    public override void Render()
    {
      _textView.Render();
    }
  }
}
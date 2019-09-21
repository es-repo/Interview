using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using PeerMessenger.Chatbotting.DialogContextItems;
using PeerMessenger.Chatbotting.DialogUnits;
using PeerMessenger.Chatbotting.DialogUnits.Concrete;
using PeerMessenger.Chatbotting.DialogUnits.Concrete.PhraseExchanges;
using PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions;
using PeerMessenger.Chatbotting.DialogUnits.Concrete.Statements;
using PeerMessenger.Messaging;

namespace PeerMessenger.Chatbotting
{
  public class Chatbot
  {
    private readonly Dictionary<IPEndPoint, ChatbotDialog> _dialogs = new Dictionary<IPEndPoint, ChatbotDialog>();

    public void Start(ITcpPeerMessenger messenger)
    {
      messenger.ClientAccepted += async (s, endPoint) =>
      {
        ChatbotDialog dialog = CreateDialog();
        dialog.DialogContext.Upsert<GetConnectedClients>(GetConnectedClients);

        _dialogs.Add(endPoint, dialog);
        foreach (string m in dialog.Handle())
        {
          await messenger.SendMessageAsync(endPoint, m);
        }
      };

      messenger.MessageReceived += async (s, msg) =>
      {
        ChatbotDialog chatbot = _dialogs[msg.From];
        foreach (string m in chatbot.Handle(msg.Body))
        {
          await messenger.SendMessageAsync(msg.From, m);
        }
      };
    }

    private string[] GetConnectedClients()
    {
      return _dialogs.Values
        .Where(c => !c.IsEnded)
        .Select(c => c.DialogContext.GetItem<ClientName>()?.Value)
        .ToArray();
    }

    private static ChatbotDialog CreateDialog()
    {
      return new ChatbotDialog(CreateDialogPlan(), () => DialogUnitsLib(), new Farewell(), new UnhandledFallback());
    }

    private static DialogPlan CreateDialogPlan()
    {
      return new DialogPlan(new List<DialogPlanItem>
      {
        new DialogPlanItem(new DialogUnit[]
        {
          new Greetings(),
          new WhatsYourName()
        }),

        new DialogPlanItem(new QuestionsCanAsk(), c => c.HasItem<ClientName>())
      });
    }

    private static IEnumerable<DialogUnit> DialogUnitsLib()
    {
      yield return new Greetings();
      yield return new MyNameIs("Бот Р2-Д2");
      yield return new WhatTimeIsIt();
      yield return new HowAreYou();
      yield return new ConnectedClients();
    }
  }
}
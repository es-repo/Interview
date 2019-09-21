using System;
using System.Linq;
using PeerMessenger.Chatbotting.DialogContextItems;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.PhraseExchanges
{
  public class Farewell : PhraseExchange
  {
    protected override string GetPhrase(DialogContext context)
    {
      ClientName clientName = context.GetItem<ClientName>();
      return clientName != null && !clientName.IsAnonymous ?
        $"Пока, {clientName.Value}!" :
        "Пока!";
    }

    protected override bool IsMatched(string message)
    {
      string[] greetings = new[] { "пока", "досвидания", "до свидания", "прощай" };
      return greetings.Contains(message);
    }
  }
}
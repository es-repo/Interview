using System;
using System.Linq;
using PeerMessenger.Chatbotting.DialogContextItems;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.PhraseExchanges
{
  public class Greetings : PhraseExchange
  {
    protected override string GetPhrase(DialogContext context)
    {
      ClientName clientName = context.GetItem<ClientName>();
      return clientName != null && !clientName.IsAnonymous ?
        $"Привет, {clientName.Value}!" :
        "Привет!";
    }

    protected override bool IsMatched(string message)
    {
      string[] greetings = new[] { "привет", "здравствуйте", "добрый день", "здрасте" };
      return greetings.Contains(message);
    }
  }
}
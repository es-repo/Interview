using System;
using System.Linq;
using PeerMessenger.Chatbotting.DialogContextItems;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions
{
  public class ConnectedClients : Answer
  {
    protected override string GetAnswer(DialogContext context)
    {
      GetConnectedClients getConnectedClients = context.GetItem<GetConnectedClients>();
      if (getConnectedClients == null)
      {
        return "Я не знаю.";
      }

      string[] clients = getConnectedClients();
      if (clients.Length == 0)
      {
        return "Нет присоединенных пользователей в чате.";
      }

      var names = clients.Where(n => !string.IsNullOrWhiteSpace(n));
      int anonClientsCount = clients.Count(n => string.IsNullOrWhiteSpace(n));
      if (anonClientsCount > 0)
      {
        string anons = (anonClientsCount == 1
          ? "1 анонимный пользователь"
          : $"{anonClientsCount} анонимных пользователя");

        names = names.Concat(new[] { anons });
      }
      return "\n" + string.Join('\n', names.Select(n => " - " + n));
    }

    protected override bool IsQuestionMatched(string message)
    {
      string[] questions = new string[]
      {
        "список пользователей в чате",
        "список пользователей",
        "список присоединенных пользователей в чате",
        "список присоединенных пользователей"
      };
      return message != null && questions.Contains(message);
    }
  }
}
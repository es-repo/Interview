using PeerMessenger.Chatbotting.DialogContextItems;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.Statements
{
  public class QuestionsCanAsk : Statement
  {
    protected override string GetStatement(DialogContext context)
    {
      ClientName clientName = context.GetItem<ClientName>();
      bool hasName = clientName != null && !clientName.IsAnonymous;
      string pre = hasName ? $"{clientName.Value}, ты" : "Ты";
      return $@"{pre} можешь спросить меня:
 - Как меня зовут
 - Как дела
 - Сколько сейчас времени
 - Список пользователей в чате";
    }
  }
}
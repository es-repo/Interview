using PeerMessenger.Chatbotting;
using PeerMessenger.Chatbotting.DialogContextItems;
using PeerMessenger.Chatbotting.DialogUnits;
using PeerMessenger.Chatbotting.DialogUnits.Concrete.PhraseExchanges;
using PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions;
using PeerMessenger.Chatbotting.DialogUnits.Concrete.Statements;
using Xunit;

namespace PeerMessenger.Tests.Chatbotting
{
  public class DialogPlanTests
  {
    [Fact]
    public void TestNext()
    {
      Greetings greetings = new Greetings();
      WhatsYourName whatsYourName = new WhatsYourName();
      QuestionsCanAsk questionsCanAsk = new QuestionsCanAsk();

      DialogPlanItem[] items = new DialogPlanItem[]
      {
        new DialogPlanItem(new DialogUnit[] { greetings, whatsYourName }),

        new DialogPlanItem(
          questionsCanAsk,
          (context) => context.HasItem<ClientName>())
      };

      DialogPlan dialogPlan = new DialogPlan(items);
      DialogContext dialogContext = new DialogContext();

      Assert.Equal(new DialogUnit[] { greetings, whatsYourName }, dialogPlan.Next(dialogContext));
      Assert.Empty(dialogPlan.Next(dialogContext));

      dialogContext.Upsert(new ClientName(null));
      Assert.Equal(new DialogUnit[] { questionsCanAsk }, dialogPlan.Next(dialogContext));
    }
  }
}
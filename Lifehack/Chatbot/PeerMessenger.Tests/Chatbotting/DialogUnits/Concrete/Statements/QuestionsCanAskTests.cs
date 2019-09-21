using PeerMessenger.Chatbotting;
using PeerMessenger.Chatbotting.DialogContextItems;
using PeerMessenger.Chatbotting.DialogUnits;
using PeerMessenger.Chatbotting.DialogUnits.Concrete.Statements;
using PeerMessenger.Chatbotting.DialogUnits.Tests;
using Xunit;

namespace PeerMessenger.Tests.Chatbotting.DialogUnits.Concrete.Statements
{
  public class QuestionsCanAskTests
  {
    [Fact]
    public void TestStating()
    {
      QuestionsCanAsk dialogUnit = new QuestionsCanAsk();
      DialogContext context = new DialogContext();
      DialogUnitTestUtils.TestStating(dialogUnit, context, "Ты можешь спросить меня:");

      context.Upsert(new ClientName("Иван"));
      DialogUnitTestUtils.TestStating(dialogUnit, context, "Иван, ты можешь спросить меня:");
    }

    [Fact]
    public void TestNotResponding()
    {
      QuestionsCanAsk dialogUnit = new QuestionsCanAsk();
      DialogContext context = new DialogContext();
      DialogUnitTestUtils.TestNotResponding(dialogUnit, context, "Привет");
    }
  }
}
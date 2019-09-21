using PeerMessenger.Chatbotting.DialogContextItems;
using PeerMessenger.Chatbotting.DialogUnits.Tests;
using Xunit;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.PhraseExchanges.Tests
{

  public class GreetingsTests
  {
    [Fact]
    public void TestInitiating()
    {
      Greetings dialogUnit = new Greetings();
      DialogContext context = new DialogContext();
      DialogUnitTestUtils.TestInitiating(dialogUnit, context, "Привет!", "Здрасте!");

      context.Upsert(new ClientName("Иван"));
      DialogUnitTestUtils.TestInitiating(dialogUnit, context, "Привет, Иван!", "Здрасте!");

      context.Upsert(new ClientName(null));
      DialogUnitTestUtils.TestInitiating(dialogUnit, context, "Привет!", "Здрасте!");
    }

    [Fact]
    public void TestResponding()
    {
      Greetings dialogUnit = new Greetings();
      DialogContext context = new DialogContext();
      DialogUnitTestUtils.TestResponding(dialogUnit, context, "Привет!", "Привет!");

      context.Upsert(new ClientName("Иван"));
      DialogUnitTestUtils.TestResponding(dialogUnit, context, "Здрасте!", "Привет, Иван!");
    }

    [Fact]
    public void TestNotResponding()
    {
      Greetings dialogUnit = new Greetings();
      DialogContext context = new DialogContext();
      DialogUnitTestUtils.TestNotResponding(dialogUnit, context, "Как дела?");
    }
  }
}
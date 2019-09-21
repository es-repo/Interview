using PeerMessenger.Chatbotting.DialogContextItems;
using PeerMessenger.Chatbotting.DialogUnits.Tests;
using Xunit;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.PhraseExchanges.Tests
{
  public class FarewellTests
  {
    [Fact]
    public void TestInitiating()
    {
      Farewell dialogUnit = new Farewell();
      DialogContext context = new DialogContext();
      DialogUnitTestUtils.TestInitiating(dialogUnit, context, "Пока!", "До свидания!");

      context.Upsert(new ClientName("Иван"));
      DialogUnitTestUtils.TestInitiating(dialogUnit, context, "Пока, Иван!", "пока");

      context.Upsert(new ClientName(null));
      DialogUnitTestUtils.TestInitiating(dialogUnit, context, "Пока!", "пока!");
    }

    [Fact]
    public void TestResponding()
    {
      Farewell dialogUnit = new Farewell();
      DialogContext context = new DialogContext();
      DialogUnitTestUtils.TestResponding(dialogUnit, context, "Пока!", "Пока!");

      context.Upsert(new ClientName("Иван"));
      DialogUnitTestUtils.TestResponding(dialogUnit, context, "Пока!", "Пока, Иван!");
    }

    [Fact]
    public void TestNotResponding()
    {
      Farewell dialogUnit = new Farewell();
      DialogContext context = new DialogContext();
      DialogUnitTestUtils.TestNotResponding(dialogUnit, context, "Как дела?");
    }
  }
}
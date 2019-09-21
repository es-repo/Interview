using System;
using PeerMessenger.Chatbotting.DialogContextItems;
using PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions;
using PeerMessenger.Chatbotting.DialogUnits.Tests;
using Xunit;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions.Tests
{
  public class AskNameTests
  {
    [Theory]
    [InlineData("Меня зовут Иван", "Иван")]
    [InlineData("Мое имя Иван", "Иван")]
    [InlineData("Иван", "Иван")]
    [InlineData(" ", null)]
    public void TestAsking(string response, string expectedName)
    {
      WhatsYourName dialogUnit = new WhatsYourName();
      DialogContext context = new DialogContext();
      DialogUnitTestUtils.TestInitiating(dialogUnit, context, "Как тебя зовут?", response);
      ClientName clientName = context.GetItem<ClientName>();
      Assert.NotNull(clientName);
      Assert.Equal(expectedName, clientName.Value);
    }

    [Fact]
    public void TestNotResponding()
    {
      WhatsYourName dialogUnit = new WhatsYourName();
      DialogContext context = new DialogContext();
      DialogUnitTestUtils.TestNotResponding(dialogUnit, context, "привет");
    }
  }
}
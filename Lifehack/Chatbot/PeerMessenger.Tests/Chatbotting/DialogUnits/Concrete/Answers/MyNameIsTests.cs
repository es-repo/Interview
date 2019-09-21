using System;
using PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions;
using PeerMessenger.Chatbotting.DialogUnits.Tests;
using Xunit;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions.Tests
{
  public class MyNameIsTests
  {
    [Theory]
    [InlineData("Как тебя зовут?")]
    [InlineData("Как твое имя?")]
    public void TestResponding(string question)
    {
      string name = "Федор";
      DialogUnit dialogUnit = new MyNameIs(name);
      DialogContext context = new DialogContext();

      DialogUnitHandleResult result = dialogUnit.Handle(context, question);
      Assert.Equal($"Меня зовут {name}.", result.Message);
      Assert.False(result.IsResponseAwaiting);
      Assert.True(result.IsHandled);
    }

    [Fact]
    public void TestNotResponding()
    {
      string name = "Федор";
      DialogUnit dialogUnit = new MyNameIs(name);
      DialogContext context = new DialogContext();
      DialogUnitTestUtils.TestNotResponding(dialogUnit, context, "привет");
    }
  }
}
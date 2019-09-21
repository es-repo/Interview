using PeerMessenger.Chatbotting;
using PeerMessenger.Chatbotting.DialogUnits;
using PeerMessenger.Chatbotting.DialogUnits.Concrete;
using Xunit;

namespace PeerMessenger.Tests.Chatbotting.DialogUnits
{
  public class UnhandledFallbackTests
  {
    [Fact]
    public void TestHandle()
    {
      DialogContext context = new DialogContext();
      UnhandledFallback unhandledFallback = new UnhandledFallback();
      DialogUnitHandleResult actualResult = unhandledFallback.Handle(context, "Что-то невнятное");
      Assert.True(actualResult.IsHandled);
      Assert.False(actualResult.IsResponseAwaiting);
      Assert.Equal("Я тебя не понимаю.", actualResult.Message);

      Assert.False(unhandledFallback.Handle(context).IsHandled);
    }
  }
}
using Xunit;

namespace PeerMessenger.Chatbotting.DialogUnits.Tests
{
  public static class DialogUnitTestUtils
  {
    public static void TestInitiating<T>(T dialogUnit, DialogContext context, string expectedInitMessage, string responseMessage)
    where T : DialogUnit
    {
      DialogUnitHandleResult result = dialogUnit.Handle(context);
      Assert.Equal(expectedInitMessage, result.Message);
      Assert.True(result.IsResponseAwaiting);
      Assert.True(result.IsHandled);

      DialogUnitHandleResult responseAwaitedResult = dialogUnit.Handle(context, responseMessage);
      Assert.Null(responseAwaitedResult.Message);
      Assert.False(responseAwaitedResult.IsResponseAwaiting);
      Assert.True(responseAwaitedResult.IsHandled);
    }

    public static void TestResponding<T>(T dialogUnit, DialogContext context, string initMessage, string expectedResponseMessage)
    where T : DialogUnit
    {
      DialogUnitHandleResult result = dialogUnit.Handle(context, initMessage);
      Assert.Equal(expectedResponseMessage, result.Message);
      Assert.False(result.IsResponseAwaiting);
      Assert.True(result.IsHandled);
    }

    public static void TestNotResponding<T>(T dialogUnit, DialogContext context, string initMessage)
    where T : DialogUnit
    {
      DialogUnitHandleResult result = dialogUnit.Handle(context, initMessage);
      Assert.Null(result.Message);
      Assert.False(result.IsResponseAwaiting);
      Assert.False(result.IsHandled);
    }

    public static void TestStating<T>(T dialogUnit, DialogContext context, string expectedStatingMessageStartWith)
    where T : Statement
    {
      DialogUnitHandleResult result = dialogUnit.Handle(context);
      Assert.StartsWith(expectedStatingMessageStartWith, result.Message);
      Assert.False(result.IsResponseAwaiting);
      Assert.True(result.IsHandled);

      result = dialogUnit.Handle(context, "whatever");
      Assert.Null(result.Message);
      Assert.False(result.IsResponseAwaiting);
      Assert.False(result.IsHandled);
    }
  }
}
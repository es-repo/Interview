using PeerMessenger.Chatbotting.DialogContextItems;
using Xunit;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions.Tests
{
  public class ConnectedClientsTests
  {
    [Theory]
    [InlineData("Список пользователей в чате")]
    [InlineData("Список пользователей")]
    [InlineData("Список присоединенных пользователей в чате")]
    [InlineData("Список присоединенных пользователей")]
    public void TestRespondingOnDifferentQuestions(string question)
    {
      DialogUnit dialogUnit = new ConnectedClients();
      DialogContext context = new DialogContext();

      context.Upsert<GetConnectedClients>(() => new string[] { "Иван" });

      DialogUnitHandleResult result = dialogUnit.Handle(context, question);
      Assert.True(result.IsHandled);
      Assert.Equal($"\n - Иван", result.Message);
      Assert.False(result.IsResponseAwaiting);
    }

    [Fact]
    public void TestDontKnowResponding()
    {
      DialogUnit dialogUnit = new ConnectedClients();
      DialogContext context = new DialogContext();

      DialogUnitHandleResult result = dialogUnit.Handle(context, "список пользователей");
      Assert.True(result.IsHandled);
      Assert.Equal($"Я не знаю.", result.Message);
      Assert.False(result.IsResponseAwaiting);
    }

    [Theory]
    [InlineData(new string[] { }, "Нет присоединенных пользователей в чате.")]
    [InlineData(new string[] { "Иван" }, "\n - Иван")]
    [InlineData(new string[] { "Иван", "Петр" }, "\n - Иван\n - Петр")]
    [InlineData(new string[] { "Иван", null, "Петр" }, "\n - Иван\n - Петр\n - 1 анонимный пользователь")]
    [InlineData(new string[] { "Иван", null, "Петр", null }, "\n - Иван\n - Петр\n - 2 анонимных пользователя")]
    [InlineData(new string[] { null }, "\n - 1 анонимный пользователь")]
    public void TestDifferentClientLists(string[] clients, string expectedAnswer)
    {
      DialogUnit dialogUnit = new ConnectedClients();
      DialogContext context = new DialogContext();
      context.Upsert<GetConnectedClients>(() => clients);

      DialogUnitHandleResult result = dialogUnit.Handle(context, "список пользователей");
      Assert.True(result.IsHandled);
      Assert.Equal(expectedAnswer, result.Message);
      Assert.False(result.IsResponseAwaiting);
    }
  }
}
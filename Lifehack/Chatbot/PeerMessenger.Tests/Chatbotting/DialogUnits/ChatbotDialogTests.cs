using System.Collections.Generic;
using System.Linq;
using PeerMessenger.Chatbotting;
using PeerMessenger.Chatbotting.DialogContextItems;
using PeerMessenger.Chatbotting.DialogUnits;
using PeerMessenger.Chatbotting.DialogUnits.Concrete;
using PeerMessenger.Chatbotting.DialogUnits.Concrete.PhraseExchanges;
using PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions;
using PeerMessenger.Chatbotting.DialogUnits.Concrete.Statements;
using Xunit;

namespace PeerMessenger.Tests.Chatbotting.DialogUnits
{
  public class ChatbotDialogTests
  {
    [Fact]
    public void TestHandle()
    {
      ChatbotDialog dialog = new ChatbotDialog(CreateDialogPlan(), DialogUnitsLib, new Farewell(), new UnhandledFallback());
      Assert.False(dialog.IsEnded);

      Assert.Equal(new string[] { "Привет!", "Как тебя зовут?" }, dialog.Handle());
      Assert.Equal(new string[] { }, dialog.Handle("здрасте"));
      Assert.StartsWith("Иван, ты можешь спросить меня", dialog.Handle("Иван").FirstOrDefault());
      Assert.Equal("Иван", dialog.DialogContext.GetItem<ClientName>()?.Value);
      Assert.Equal(new string[] { "Меня зовут Федор." }, dialog.Handle("Как тебя зовут?"));
      Assert.Equal(new string[] { "Хорошо!" }, dialog.Handle("Как дела?"));
      Assert.StartsWith("Сейчас", dialog.Handle("который час?").First());
      Assert.Equal(new string[] { "Привет, Иван!" }, dialog.Handle("здрасте"));
      Assert.Equal(new string[] { "Я тебя не понимаю." }, dialog.Handle("Что-то невнятное"));
      Assert.Equal(new string[] { "Пока, Иван!" }, dialog.Handle("Пока"));
      Assert.True(dialog.IsEnded);
    }

    private static DialogPlan CreateDialogPlan()
    {
      return new DialogPlan(new List<DialogPlanItem>
      {
        new DialogPlanItem(new DialogUnit[]
        {
          new Greetings(),
          new WhatsYourName()
        }),

        new DialogPlanItem(new QuestionsCanAsk(), c => c.HasItem<ClientName>())
      });
    }

    private static IEnumerable<DialogUnit> DialogUnitsLib()
    {
      yield return new Greetings();
      yield return new MyNameIs("Федор");
      yield return new WhatTimeIsIt();
      yield return new HowAreYou(3);
    }
  }
}
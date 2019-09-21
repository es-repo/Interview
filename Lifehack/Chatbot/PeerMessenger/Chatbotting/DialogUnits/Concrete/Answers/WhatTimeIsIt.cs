using System;
using System.Linq;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions
{
  public class WhatTimeIsIt : Answer
  {
    protected override string GetAnswer(DialogContext context)
    {
      return $"Сейчас {DateTime.Now.ToShortTimeString()}.";
    }

    protected override bool IsQuestionMatched(string message)
    {
      string[] questions = new string[]
      {
        "сколько времени",
        "который час",
        "сколько сейчас времени",
        "который сейчас час"
      };
      return message != null && questions.Contains(message);
    }
  }
}
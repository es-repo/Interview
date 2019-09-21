using System;
using System.Linq;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions
{
  public class HowAreYou : Answer
  {
    private readonly Random _random;

    public HowAreYou(int? seed = null)
    {
      _random = seed == null ? new Random() : new Random(seed.Value);
    }

    protected override string GetAnswer(DialogContext context)
    {
      string[] answers = new string[] { "Хорошо!", "Отлично!", "Бывало и получше." };
      return answers[_random.Next(answers.Length)];
    }

    protected override bool IsQuestionMatched(string message)
    {
      string[] questions = new string[]
      {
        "как дела",
        "как у тебя дела"
      };
      return message != null && questions.Contains(message);
    }
  }
}
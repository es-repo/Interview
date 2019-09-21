using System;
using System.Linq;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions
{
  public class MyNameIs : Answer
  {
    private readonly string _name;

    public MyNameIs(string name)
    {
      _name = name;
    }

    protected override bool IsQuestionMatched(string message)
    {
      string[] questions = new string[]
      {
        "как тебя зовут",
        "как твое имя"
      };
      return message != null && questions.Contains(message);
    }

    protected override string GetAnswer(DialogContext context)
    {
      return $"Меня зовут {_name}.";
    }
  }
}
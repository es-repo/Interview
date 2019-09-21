using PeerMessenger.Chatbotting.DialogContextItems;

namespace PeerMessenger.Chatbotting.DialogUnits.Concrete.Questions
{
  public class WhatsYourName : Question
  {
    protected override string GetQuestion(DialogContext context)
    {
      return "Как тебя зовут?";
    }

    protected override string ExtractAnswer(string message)
    {
      string name = message
        .Replace("меня зовут", "")
        .Replace("мое имя", "")
        .Trim();

      return name == "" ?
        "" :
        char.ToUpperInvariant(name[0]) + name.Substring(1);
    }

    protected override void AddAnswerToContext(DialogContext context, string answer)
    {
      context.Upsert(new ClientName(answer));
    }
  }
}
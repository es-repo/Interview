namespace PeerMessenger.Chatbotting.DialogUnits
{
  public abstract class Answer : DialogUnit
  {
    protected override DialogUnitHandleResult HandleInternal(DialogContext context, string message, DialogUnitHandleResult previousHandledResult)
    {
      if (IsQuestionMatched(message))
      {
        return new DialogUnitHandleResult
        {
          Message = GetAnswer(context),
          IsHandled = true
        };
      }
      return DialogUnitHandleResult.NotHandled;
    }

    protected abstract bool IsQuestionMatched(string message);

    protected abstract string GetAnswer(DialogContext context);
  }
}
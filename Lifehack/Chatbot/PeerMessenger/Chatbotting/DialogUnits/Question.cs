namespace PeerMessenger.Chatbotting.DialogUnits
{
  public abstract class Question : DialogUnit
  {
    protected override DialogUnitHandleResult HandleInternal(DialogContext context, string message, DialogUnitHandleResult previousHandledResult)
    {
      if (message == null)
      {
        return new DialogUnitHandleResult
        {
          Message = GetQuestion(context),
          IsResponseAwaiting = true,
          IsHandled = true
        };
      }
      else
      {
        if (previousHandledResult.IsResponseAwaiting)
        {
          string answer = ExtractAnswer(message);
          AddAnswerToContext(context, answer);
          return new DialogUnitHandleResult
          {
            Message = null,
            IsHandled = true
          };
        }
        else
        {
          return DialogUnitHandleResult.NotHandled;
        }
      }
    }

    protected abstract string GetQuestion(DialogContext context);

    protected abstract string ExtractAnswer(string message);

    protected abstract void AddAnswerToContext(DialogContext context, string answer);
  }
}
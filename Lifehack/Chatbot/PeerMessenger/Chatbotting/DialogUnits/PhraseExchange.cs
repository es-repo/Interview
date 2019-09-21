namespace PeerMessenger.Chatbotting.DialogUnits
{
  public abstract class PhraseExchange : DialogUnit
  {
    protected override DialogUnitHandleResult HandleInternal(DialogContext context, string message, DialogUnitHandleResult previousHandledResult)
    {
      if (message == null)
      {
        return new DialogUnitHandleResult
        {
          Message = GetPhrase(context),
          IsHandled = true,
          IsResponseAwaiting = true
        };
      }
      else
      {
        if (IsMatched(message))
        {
          return previousHandledResult.IsResponseAwaiting
            ? new DialogUnitHandleResult
            {
              IsResponseAwaiting = false,
              IsHandled = true
            }
            : new DialogUnitHandleResult
            {
              IsResponseAwaiting = false,
              IsHandled = true,
              Message = GetPhrase(context)
            };
        }
        else
        {
          return DialogUnitHandleResult.NotHandled;
        }
      }
    }

    protected abstract string GetPhrase(DialogContext context);

    protected abstract bool IsMatched(string message);
  }
}
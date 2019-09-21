namespace PeerMessenger.Chatbotting.DialogUnits.Concrete
{
  public class UnhandledFallback : DialogUnit
  {
    protected override DialogUnitHandleResult HandleInternal(DialogContext context, string message, DialogUnitHandleResult previousHandledResult)
    {
      return !string.IsNullOrEmpty(message)
        ? new DialogUnitHandleResult
        {
          Message = "Я тебя не понимаю.",
          IsHandled = true,
          IsResponseAwaiting = false
        }
        : new DialogUnitHandleResult
        {
          IsHandled = false
        };
    }
  }
}
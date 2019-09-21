namespace PeerMessenger.Chatbotting.DialogUnits
{
  public class DialogUnitHandleResult
  {
    public bool IsHandled { get; set; }
    public bool IsResponseAwaiting { get; set; }
    public string Message { get; set; }

    public static DialogUnitHandleResult NotHandled
    {
      get => new DialogUnitHandleResult { IsHandled = false };
    }
  }

  public abstract class DialogUnit
  {
    private DialogUnitHandleResult _previousHandledResult = new DialogUnitHandleResult();

    public DialogUnitHandleResult Handle(DialogContext context, string message = null)
    {
      string messageNormalized = message != null ? NormalizeMessage(message) : null;
      DialogUnitHandleResult result = HandleInternal(context, messageNormalized, _previousHandledResult);
      if (result.IsHandled)
      {
        _previousHandledResult = result;
      }
      return result;
    }

    protected abstract DialogUnitHandleResult HandleInternal(DialogContext context, string message, DialogUnitHandleResult previousHandledResult);

    private static string NormalizeMessage(string message)
    {
      return message.ToLowerInvariant()
        .Replace(".", "")
        .Replace("?", "")
        .Replace("!", "")
        .Trim();
    }
  }
}
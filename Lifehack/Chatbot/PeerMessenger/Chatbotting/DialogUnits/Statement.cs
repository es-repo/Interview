using System;

namespace PeerMessenger.Chatbotting.DialogUnits
{
  public abstract class Statement : DialogUnit
  {
    protected abstract string GetStatement(DialogContext context);

    protected override DialogUnitHandleResult HandleInternal(DialogContext context, string message, DialogUnitHandleResult previousHandledResult)
    {
      if (message != null)
      {
        return DialogUnitHandleResult.NotHandled;
      }

      return new DialogUnitHandleResult
      {
        Message = GetStatement(context),
        IsHandled = true,
        IsResponseAwaiting = false
      };
    }
  }
}
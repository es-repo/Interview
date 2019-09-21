using System;
using System.Collections.Generic;
using System.Linq;
using PeerMessenger.Chatbotting.DialogUnits;

namespace PeerMessenger.Chatbotting
{
  public class ChatbotDialog
  {
    private readonly DialogPlan _dialogPlan;
    private readonly Func<IEnumerable<DialogUnit>> _dialogUnitsLib;
    private readonly DialogUnit[] _farewells;
    private readonly DialogUnit[] _unhandledFallbacks;
    private readonly List<DialogUnit> _responseAwaitingDialogUnits;

    public DialogContext DialogContext { get; }

    public bool IsEnded { get; private set; }

    public ChatbotDialog(DialogPlan dialogPlan, Func<IEnumerable<DialogUnit>> dialogUnitsLib, DialogUnit farewell, DialogUnit unhandledFallback)
    {
      _dialogPlan = dialogPlan;
      _dialogUnitsLib = dialogUnitsLib;
      _farewells = new DialogUnit[] { farewell };
      _unhandledFallbacks = new DialogUnit[] { unhandledFallback };
      _responseAwaitingDialogUnits = new List<DialogUnit>();
      DialogContext = new DialogContext();
    }

    public IEnumerable<string> Handle(string message = null)
    {
      if (IsEnded)
      {
        return Array.Empty<string>();
      }

      if (message == null)
      {
        return NextMessageByDialogPlan();
      }
      else
      {
        var (result, handledDialogUnit) = HandleMessage(
          DialogContext,
          message,
          _responseAwaitingDialogUnits
            .Concat(_dialogUnitsLib())
            .Concat(_farewells)
            .Concat(_unhandledFallbacks),
          _responseAwaitingDialogUnits);

        if (handledDialogUnit != null)
        {
          if (_farewells.Contains(handledDialogUnit))
          {
            IsEnded = true;
          }

          string[] response = result.Message != null ? new[] { result.Message } : Array.Empty<string>();
          return response.Concat(NextMessageByDialogPlan());
        }
        else
        {
          return Array.Empty<string>();
        }
      }
    }

    private IEnumerable<string> NextMessageByDialogPlan()
    {
      foreach (DialogUnit dialogUnit in _dialogPlan.Next(DialogContext))
      {
        var (result, handledDialogUnit) = HandleMessage(
          DialogContext,
          null,
          new[] { dialogUnit },
          _responseAwaitingDialogUnits);

        if (handledDialogUnit != null)
        {
          yield return result.Message;
        }
      }
    }

    private static (DialogUnitHandleResult Result, DialogUnit HandledDialogUnit) HandleMessage(
      DialogContext context, string message, IEnumerable<DialogUnit> dialogUnits, ICollection<DialogUnit> responseAwaitingDialogUnits)
    {
      foreach (DialogUnit dialogUnit in dialogUnits)
      {
        DialogUnitHandleResult result = dialogUnit.Handle(context, message);
        if (result.IsHandled)
        {
          if (result.IsResponseAwaiting)
          {
            responseAwaitingDialogUnits.Add(dialogUnit);
          }
          else
          {
            responseAwaitingDialogUnits.Remove(dialogUnit);
          }

          return (result, dialogUnit);
        }
      }
      return default;
    }
  }
}
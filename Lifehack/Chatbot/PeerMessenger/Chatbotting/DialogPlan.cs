using System;
using System.Collections.Generic;
using System.Linq;
using PeerMessenger.Chatbotting.DialogUnits;

namespace PeerMessenger.Chatbotting
{
  public class DialogPlanItem
  {
    public DialogUnit[] DialogUnits { get; }

    public Func<DialogContext, bool> Predicate { get; }

    public DialogPlanItem(DialogUnit dialogUnit) : this(new DialogUnit[] { dialogUnit }, c => true) { }

    public DialogPlanItem(DialogUnit[] dialogUnits) : this(dialogUnits, c => true) { }

    public DialogPlanItem(DialogUnit dialogUnit, Func<DialogContext, bool> predicate) : this(new DialogUnit[] { dialogUnit }, predicate) { }

    public DialogPlanItem(DialogUnit[] dialogUnits, Func<DialogContext, bool> predicate)
    {
      DialogUnits = dialogUnits;
      Predicate = predicate;
    }
  }

  public class DialogPlan
  {
    private readonly List<DialogPlanItem> _planItems;

    public DialogPlan(IEnumerable<DialogPlanItem> planItems)
    {
      _planItems = planItems.ToList();
    }

    public IEnumerable<DialogUnit> Next(DialogContext context)
    {
      if (_planItems.Count == 0)
      {
        return Array.Empty<DialogUnit>();
      }

      DialogPlanItem nextItem = _planItems.FirstOrDefault(i => i.Predicate(context));
      if (nextItem != null)
      {
        _planItems.Remove(nextItem);
        return nextItem.DialogUnits;
      }

      return Array.Empty<DialogUnit>();
    }
  }
}
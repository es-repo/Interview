using System;

namespace PeerMessenger.Presentation
{
  public class Header : ConsoleUiComponent
  {
    private readonly string _bootomLine;
    private readonly string _title;

    public Header(bool isChatbotMode) : base(0, 2)
    {
      _title = "PEER MESSENGER V.0.0.1" + (isChatbotMode ? " - CHATBOT MODE" : "");
      _bootomLine = new string('-', Console.WindowWidth);
    }

    protected override void RenderInternal()
    {
      Console.Write(_title);
      Console.SetCursorPosition(0, 1);
      Console.Write(_bootomLine);
    }
  }
}
using System;

namespace PeerMessenger.Presentation
{
  public class HorizontalSplitter : ConsoleUiComponent
  {
    private readonly string _splitterLine;

    public HorizontalSplitter(int top) : base(top, 1)
    {
      _splitterLine = new String('=', Console.WindowWidth);
    }

    protected override void RenderInternal()
    {
      Console.Write(_splitterLine);
    }
  }
}
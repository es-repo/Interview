using System;
using PeerMessenger.Utils;

namespace PeerMessenger.Presentation
{
  public class TextView : ConsoleUiComponent
  {
    private string text;

    public string Text { get => text; set { text = value; } }

    public TextView(int top, int height) : base(top, height)
    {
    }

    protected override void RenderInternal()
    {
      if (string.IsNullOrEmpty(Text))
      {
        return;
      }

      int linesCount = Text.CountLines();
      int startLine = Math.Max(0, linesCount - Height);
      int startIndex = startLine == 0
        ? 0
        : Text.IndexOfNth('\n', startLine - 1) + 1;

      Console.Write(Text.Substring(startIndex));
    }
  }
}
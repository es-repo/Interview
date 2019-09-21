using System;

namespace PeerMessenger.Presentation
{
  public abstract class ConsoleUiComponent
  {
    private readonly string emptyString = new string(' ', Console.BufferWidth);

    public int Top { get; }
    public int Height { get; }

    protected ConsoleUiComponent(int top, int height)
    {
      Top = top;
      Height = height;
    }

    public virtual void Render()
    {
      if (Height == 0)
      {
        return;
      }

      int cursorLeft = Console.CursorLeft;
      int cursorTop = Console.CursorTop;

      Clear();

      Console.SetCursorPosition(0, Top);
      RenderInternal();
      Console.SetCursorPosition(cursorLeft, cursorTop);
    }

    protected virtual void RenderInternal() { }

    private void Clear()
    {
      for (int i = Top; i < Top + Height; i++)
      {
        Console.SetCursorPosition(0, i);
        Console.WriteLine(this.emptyString);
      }
    }
  }
}
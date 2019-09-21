using System;
using System.Threading.Tasks;

namespace PeerMessenger.Presentation
{
  public class TextInput : ConsoleUiComponent
  {
    public event EventHandler<string> Entered;

    public TextInput(int top) : base(top, 1)
    {
      Entered += (s, a) => { };
    }

    protected override void RenderInternal()
    {
      Console.Write("> ");
      string text = Console.ReadLine();
      Entered(this, text);
    }
  }
}
using System;

namespace PeerMessenger.Presentation
{
  public class MessageTextInput : ConsoleUiComponent
  {
    private readonly TextInput _input;

    public event EventHandler<string> MessageEntered;

    public MessageTextInput(int top) : base(0, 0)
    {
      _input = new TextInput(top);

      _input.Entered += (s, a) =>
      {
        MessageEntered(this, a);
      };

      MessageEntered += (s, a) => { };
    }

    public override void Render()
    {
      _input.Render();
    }
  }
}
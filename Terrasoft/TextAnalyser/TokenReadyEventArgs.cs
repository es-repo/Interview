using System;

namespace Terrasoft
{
  public class TokenReadyEventArgs : EventArgs
  {
    public Token Token { get; private set; }

    public TokenReadyEventArgs(Token token)
    {
      Token = token;
    }
  }
}
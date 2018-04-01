using System;
using System.Text;

namespace Terrasoft
{
  /// <summary>
  /// This class serves to accepts characters one by one to build a token. 
  /// </summary>
  public abstract class TokenBuilder
  {
    protected readonly StringBuilder _characters = new StringBuilder();
    protected long _tokenPosition;

    /// <summary>
    /// Previous character which was passed to the builder.
    /// </summary>
    protected char PrevCharacter { get; private set; }

    /// <summary>
    /// Last character which was added to the internal list of characters 
    /// or 0 if the list is empty.
    /// </summary>
    /// <returns></returns>
    protected char LastCharacter
    {
      get
      {
        return _characters.Length > 0 ? _characters[_characters.Length - 1] : (char) 0;
      }
    }

    /// <summary>
    /// Event which  is fired when token is ready.
    /// </summary>
    public event EventHandler<TokenReadyEventArgs> TokenReady = delegate { };

    private void OnTokenReady()
    {
      Token token = CreateToken();
      TokenReady(this, new TokenReadyEventArgs(token));
      _characters.Clear();
    }

    /// <summary>
    /// Feeds the token with a next character.
    /// Raises TokenReady event if token is ready.
    /// </summary>
    public void OnNextChar(char nextChar, long position)
    {
      if (_characters.Length > 0 && IsTokenReady(nextChar))
      {
        OnTokenReady();
      }

      if (CanAppend(nextChar))
      {
        if (_characters.Length == 0)
        {
          _tokenPosition = position;
        }
        _characters.Append(nextChar);
      }
      else
      {
        _characters.Clear();
      }

      PrevCharacter = nextChar;
    }

    /// <summary>
    /// Signals the token builder that character stream is ended.
    /// The token builder will raise TokenReady event if it has a ready token.
    /// </summary>
    public void OnEnd()
    {
      if (_characters.Length > 0 && IsTokenReady())
      {
        OnTokenReady();
      }
    }

    protected abstract bool CanAppend(char nextChar);

    protected abstract bool IsTokenReady(char nextChar);

    protected virtual bool IsTokenReady()
    {
      return true;
    }

    protected abstract Token CreateToken();
  }

  /// <summary>
  /// Generic version of <see cref="TokenBuilder"/>.
  /// </summary>
  public abstract class TokenBuilder<TToken> : TokenBuilder where TToken : Token
  {
    protected override Token CreateToken()
    {
      return this.CreateTokenOfType();
    }

    protected abstract TToken CreateTokenOfType();
  }
}
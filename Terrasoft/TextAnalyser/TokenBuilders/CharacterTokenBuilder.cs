namespace Terrasoft
{
  /// <summary>
  /// A token builder for tokens which represents a single character.
  /// </summary>
  public class CharacterTokenBuilder : TokenBuilder<CharacterToken>
  {
    protected override bool CanAppend(char nextChar)
    {
      return true;
    }

    protected override bool IsTokenReady(char nextChar)
    {
      return true;
    }

    protected override CharacterToken CreateTokenOfType()
    {
      return new CharacterToken { Position = _tokenPosition, Value = _characters[0] };
    }
  }
}
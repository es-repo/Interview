namespace Terrasoft
{
  /// <summary>
  /// A token builder for tokens which reprsents a number.
  /// </summary>
  public class NumberTokenBuilder : TokenBuilder<NumberToken>
  {
    protected override bool CanAppend(char nextChar)
    {
      return (char.IsSeparator(PrevCharacter) 
        || char.IsWhiteSpace(PrevCharacter) 
        || char.IsDigit(LastCharacter))
        && char.IsDigit(nextChar);
    }

    protected override bool IsTokenReady(char nextChar)
    {
      return !char.IsDigit(nextChar);
    }

    protected override NumberToken CreateTokenOfType()
    {
      return new NumberToken { Position = _tokenPosition, Value = int.Parse(_characters.ToString()) };
    }
  }
}
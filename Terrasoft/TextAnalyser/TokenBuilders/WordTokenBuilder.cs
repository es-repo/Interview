namespace Terrasoft
{
  /// <summary>
  /// A token builder for tokens which reprsents a single word.
  /// </summary>
  public class WordTokenBuilder : TokenBuilder<WordToken>
  {

    protected override bool CanAppend(char nextChar)
    {
      return char.IsLetterOrDigit(nextChar);
    }

    protected override bool IsTokenReady(char nextChar)
    {
      return !char.IsLetterOrDigit(nextChar);
    }

    protected override WordToken CreateTokenOfType()
    {
      return new WordToken { Position = _tokenPosition, Value = _characters.ToString() };
    }
  }
}
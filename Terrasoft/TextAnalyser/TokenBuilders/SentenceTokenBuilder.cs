namespace Terrasoft
{
  /// <summary>
  /// A token builder for tokens which reprsents a sentence.
  /// </summary>
  public class SentenceTokenBuilder : TokenBuilder<SentenceToken>
  {
    protected override bool CanAppend(char nextChar)
    {
      bool isFirst = _characters.Length == 0;
      if (isFirst)
        return char.IsLetterOrDigit(nextChar);

      return char.IsLetterOrDigit(nextChar) ||
        char.IsPunctuation(nextChar) ||
        char.IsSeparator(nextChar);
    }

    protected override bool IsTokenReady(char nextChar)
    {
      bool lastIsPunctuation = char.IsPunctuation(LastCharacter);
      return (lastIsPunctuation && char.IsSeparator(nextChar)) || nextChar == '\n';
    }

    protected override SentenceToken CreateTokenOfType()
    {
      return new SentenceToken { Position = _tokenPosition, Value = _characters.ToString() };
    }
  }
}
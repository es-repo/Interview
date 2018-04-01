namespace Terrasoft
{
  /// <summary>
  /// Base class for metric implementations.
  /// </summary>
  public abstract class Metric
  {
    /// <summary>
    /// Feeds the metric with next token.
    /// </summary>
    /// <param name="token"></param>
    public abstract void OnNextToken(Token token);
  }

  /// <summary>
  /// Generic version of base class for metric implementaions.
  /// </summary>
  public abstract class Metric<TToken> : Metric where TToken : Token
  {
    public override void OnNextToken(Token token)
    {
      if (typeof(TToken) != token.GetType())
        return;

      OnNextToken((TToken) token);
    }

    protected abstract void OnNextToken(TToken token);
  }
}
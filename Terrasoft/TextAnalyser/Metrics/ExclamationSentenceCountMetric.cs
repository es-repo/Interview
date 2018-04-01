namespace Terrasoft
{
  public class ExclamationSentenceCountMetric : Metric<SentenceToken>
  {
    public long Count { get; private set; }

    protected override void OnNextToken(SentenceToken token)
    {
      if (token.Value.EndsWith("!"))
        Count++;
    }

    public override string ToString()
    {
      return $"Number of exclamation sentences is {Count}.";
    }
  }
}
namespace Terrasoft
{
  public class WordsCountMetric : Metric<WordToken>
  {
    public long Count { get; private set; }

    protected override void OnNextToken(WordToken token)
    {
      Count++;
    }

    public override string ToString()
    {
      return $"Number of words is {Count}.";
    }
  }
}
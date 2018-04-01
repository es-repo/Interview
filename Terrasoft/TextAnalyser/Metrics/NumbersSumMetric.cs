namespace Terrasoft
{
  public class NumbersSumMetric : Metric<NumberToken>
  {
    public int Sum { get; private set; }

    protected override void OnNextToken(NumberToken token)
    {
      Sum += token.Value;
    }

    public override string ToString()
    {
      return $"Sum of all numbers is {Sum}.";
    }
  }
}
namespace Terrasoft
{
  public class AverageNumberMetric : Metric<NumberToken>
  {
    private int _numbersCount;
    private int _sum;

    public int Average
    {
      get { return (int) _sum / _numbersCount; }
    }

    protected override void OnNextToken(NumberToken token)
    {
      _sum += token.Value;
      _numbersCount++;
    }

    public override string ToString()
    {
      return $"Average number is {Average}.";
    }
  }
}
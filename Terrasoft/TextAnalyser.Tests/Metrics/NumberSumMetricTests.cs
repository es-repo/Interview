using Xunit;
using Terrasoft;

namespace Terrasoft.Tests
{
  public class NumberSumMetricTests
  {
    [Theory,
      InlineData(new [] { 1 }, 1),
      InlineData(new [] { 1, 2, 3 }, 6)
    ]
    public void Test(int[] numbers, long expected)
    {
      NumbersSumMetric metric = new NumbersSumMetric();
      foreach (int n in numbers)
        metric.OnNextToken(new NumberToken { Value = n });

      Assert.Equal(expected, metric.Sum);
    }
  }

  public class AverageNumberMetricTests
  {
    [Theory,
      InlineData(new [] { 1 }, 1),
      InlineData(new [] { 1, 2, 3 }, 2),
      InlineData(new [] { 1, 5, 6 }, 4)
    ]
    public void Test(int[] numbers, long expected)
    {
      AverageNumberMetric metric = new AverageNumberMetric();
      foreach (int n in numbers)
        metric.OnNextToken(new NumberToken { Value = n });

      Assert.Equal(expected, metric.Average);
    }
  }
}
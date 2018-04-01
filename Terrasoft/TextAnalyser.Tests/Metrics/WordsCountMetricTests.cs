using Terrasoft;
using Xunit;

namespace Terrasoft.Tests
{
  public class WordsCountMetricTests
  {
    [Theory,
      InlineData(new [] { "a" }, 1),
      InlineData(new [] { "abc", "hello", "ok" }, 3)
    ]
    public void Test(string[] words, long expected)
    {
      WordsCountMetric metric = new WordsCountMetric();
      foreach (string w in words)
        metric.OnNextToken(new WordToken { Value = w });

      Assert.Equal(expected, metric.Count);
    }
  }
}
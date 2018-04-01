using Xunit;

namespace Terrasoft.Tests
{
  public class ExclamationSentenceCountMetricTests
  {
    [Theory,
      InlineData(new [] { "Hello world." }, 0),
      InlineData(new [] { "Hello world!" }, 1),
      InlineData(new [] { "Hello!", "Hello world", "Hello world!!!" }, 2)
    ]
    public void Test(string[] sentences, long expected)
    {
      ExclamationSentenceCountMetric metric = new ExclamationSentenceCountMetric();
      foreach (string s in sentences)
        metric.OnNextToken(new SentenceToken { Value = s });

      Assert.Equal(expected, metric.Count);
    }
  }
}
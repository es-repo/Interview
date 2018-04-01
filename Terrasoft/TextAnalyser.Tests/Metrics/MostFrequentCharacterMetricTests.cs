using Terrasoft;
using Xunit;

namespace Terrasoft.Tests
{
    public class MostFrequentCharacterMetricTests
    {
        [Theory,
            InlineData("a", 'a'),
            InlineData("abc", 'a'),
            InlineData("abb", 'b'),
            InlineData("abb cb dd aaa ccczdscc", 'c')
        ]
        public void Test(string text, char expected)
        {
            MostFrequentCharacterMetric metric = new MostFrequentCharacterMetric();
            foreach (char ch in text)
                metric.OnNextToken(new CharacterToken { Value = ch });

            Assert.Equal(expected, metric.Character);
        }
    }
}
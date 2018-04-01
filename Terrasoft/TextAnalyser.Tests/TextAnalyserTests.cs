using Terrasoft;
using Xunit;

namespace Terrasoft.Tests
{
  public class TextAnalyserTests
  {
    [Fact]
    public void TestAnalyse()
    {
      Tokeniser tokeniser = new Tokeniser(
        new CharacterTokenBuilder(),
        new WordTokenBuilder(),
        new SentenceTokenBuilder(),
        new NumberTokenBuilder());

      TextAnalyser textAnalyser = new TextAnalyser(tokeniser);
      var mostFrecCharMetric = new MostFrequentCharacterMetric();
      var wordsCountMetric = new WordsCountMetric();
      var exclamationSentenceCountMetric = new ExclamationSentenceCountMetric();
      var numbersSumMetric = new NumbersSumMetric();

      textAnalyser.Analyse("Hello World! 1 Lorem ipsum 49... Lorem ipsum!! Dolor 23 sit amet? Consectetur adipiscing elit.",
        mostFrecCharMetric, 
        wordsCountMetric, 
        exclamationSentenceCountMetric,
        numbersSumMetric);

      Assert.Equal('e', mostFrecCharMetric.Character);
      Assert.Equal(7, mostFrecCharMetric.OccurenciesCount);
      Assert.Equal(15, wordsCountMetric.Count); 
      Assert.Equal(2, exclamationSentenceCountMetric.Count);
      Assert.Equal(73, numbersSumMetric.Sum); 
    }
  }
}
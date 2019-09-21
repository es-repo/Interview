using Xunit;

namespace PeerMessenger.Tests
{
  public class CommandLineArgumentsTests
  {
    [Theory]
    [InlineData("", 0, false)]
    [InlineData("x", 0, false)]
    [InlineData("-port", 0, false)]
    [InlineData("-port 400", 400, false)]
    [InlineData("-port a", 0, false)]
    [InlineData("-port 400 -bot", 400, true)]
    [InlineData("-port -bot", 0, true)]
    [InlineData("-port -bot 400", 0, true)]
    [InlineData("-bot 400", 0, true)]
    [InlineData("-bot -port 400", 400, true)]
    public void TestCreation(string commandLineArgsString, int expPort, bool expDoStartChatbot)
    {
      string[] args = commandLineArgsString.Split(" ");
      CommandLineArguments commandLineArgs = new CommandLineArguments(args);
      Assert.Equal(expPort, commandLineArgs.Port);
      Assert.Equal(expDoStartChatbot, commandLineArgs.IsChatbotMode);
    }
  }
}
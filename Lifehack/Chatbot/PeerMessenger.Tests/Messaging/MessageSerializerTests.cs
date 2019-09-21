using PeerMessenger.Messaging;
using Xunit;

namespace PeerMessenger.Tests.Messaging
{
  public class MessageSerializerTests
  {
    [Theory]
    [InlineData("", "{\"body\":\"\"}")]
    [InlineData("msg", "{\"body\":\"msg\"}")]
    [InlineData("msg1\nmsg2", "{\"body\":\"msg1\\nmsg2\"}")]
    public void TestSerializeMessageBody(string body, string expected)
    {
      MessageSerializer serializer = new MessageSerializer();
      string actual = serializer.SerializeMessageBody(body);
      Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("{\"body\":\"\"}", "")]
    [InlineData("{\"body\":\"msg\"}", "msg")]
    [InlineData("{\"body\":\"msg1\\nmsg2\"}", "msg1\nmsg2")]
    public void TestDeserializeMessageBodies(string bodies, string expected)
    {
      MessageSerializer serializer = new MessageSerializer();
      string actual = serializer.DeserializeMessageBody(bodies);
      Assert.Equal(expected, actual);
    }
  }
}
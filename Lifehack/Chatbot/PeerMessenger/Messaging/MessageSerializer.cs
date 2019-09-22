using Newtonsoft.Json;

namespace PeerMessenger.Messaging
{
  public class MessageSerializer : IMessageSerializer
  {
    public string DeserializeMessageBody(string input)
    {
      return JsonConvert.DeserializeAnonymousType(input, new { body = "" }).body;
    }

    public string SerializeMessageBody(string body)
    {
      return JsonConvert.SerializeObject(new { body });
    }
  }
}
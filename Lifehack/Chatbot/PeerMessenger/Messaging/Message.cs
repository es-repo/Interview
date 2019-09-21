using System.Net;

namespace PeerMessenger.Messaging
{
  public class Message
  {
    public IPEndPoint From { get; }

    public string Body { get; }

    public Message(IPEndPoint from, string body)
    {
      From = from;
      Body = body;
    }
  }
}
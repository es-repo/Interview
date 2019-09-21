namespace PeerMessenger.Messaging
{
  public class SystemMessage : Message
  {
    public SystemMessage(string body) : base(null, body)
    { }
  }
}
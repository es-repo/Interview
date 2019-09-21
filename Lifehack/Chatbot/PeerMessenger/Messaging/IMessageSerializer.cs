using System.Collections.Generic;

namespace PeerMessenger.Messaging
{
  public interface IMessageSerializer
  {
    string DeserializeMessageBody(string input);
    string SerializeMessageBody(string stringBody);
  }
}
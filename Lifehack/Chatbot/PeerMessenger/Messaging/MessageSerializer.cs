using Newtonsoft.Json;

namespace PeerMessenger.Messaging
{
  public class MessageSerializer : IMessageSerializer
  {
    public string DeserializeMessageBody(string input)
    {
      return JsonConvert.DeserializeAnonymousType(input, new { body = "" }).body;

      // if (!input.StartsWith("(") && !input.EndsWith(")"))
      // {
      //   throw new ArgumentException("Input should start with '(' and and with ')'");
      // }

      // int startIndex = 0;
      // int length = -1;
      // for (int i = 0; i < input.Length; i++)
      // {
      //   if (input[i] == ')')
      //   {
      //     yield return input.Substring(startIndex + 1, length);
      //     startIndex = i + 1;
      //     length = -1;

      //     if (startIndex < input.Length && input[startIndex] != '(')
      //     {
      //       throw new ArgumentException($"'(' is expected at {startIndex} position");
      //     }
      //   }
      //   else
      //   {
      //     length++;
      //   }
      // }
    }

    public string SerializeMessageBody(string body)
    {
      return JsonConvert.SerializeObject(new { body });
    }
  }
}
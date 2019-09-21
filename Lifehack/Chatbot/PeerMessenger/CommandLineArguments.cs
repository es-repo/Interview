namespace PeerMessenger
{
  public class CommandLineArguments
  {
    public int Port { get; }
    public bool IsChatbotMode { get; }

    public CommandLineArguments(string[] args)
    {
      int i = 0;
      while (i < args.Length)
      {
        string arg = args[i];
        switch (arg)
        {
          case "-port":
            {
              if ((i + 1) < args.Length && int.TryParse(args[i + 1], out int port))
              {
                Port = port;
                i++;
              }
              break;
            }
          case "-bot":
            {
              IsChatbotMode = true;
              break;
            }
        }
        i++;
      }
    }
  }
}
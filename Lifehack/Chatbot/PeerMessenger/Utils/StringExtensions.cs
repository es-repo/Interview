using System.Linq;

namespace PeerMessenger.Utils
{
  public static class StringExtensions
  {
    public static int CountLines(this string text)
    {
      int newLinesCount = text.Count(ch => ch == '\n');
      if (newLinesCount == 0)
      {
        return 1;
      }
      if (text.LastIndexOf('\n') < text.Length - 1)
      {
        return newLinesCount + 1;
      }

      return newLinesCount;
    }

    public static int IndexOfNth(this string s, char ch, int n)
    {
      int c = -1;
      for (int i = 0; i < s.Length; i++)
      {
        if (s[i] == ch)
        {
          c++;
          if (c == n)
          {
            return i;
          }
        }
      }
      return -1;
    }
  }
}
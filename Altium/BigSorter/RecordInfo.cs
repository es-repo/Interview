namespace Altium.BigSorter
{
  public struct RecordInfo
  {
    public readonly int Start;
    public readonly int Length;
    public readonly int StringStart;
    public readonly int StringLength;
    public readonly int Number;

    public RecordInfo(int start, int length, int number, int stringStart)
    {
      Start = start;
      Length = length;
      StringStart = stringStart;
      StringLength = Length - (StringStart - Start);
      Number = number;
    }
  }
}
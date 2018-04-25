namespace Altium.BigSorter
{
  public struct RecordInfo
  {
    public readonly int Position;
    public readonly int Length;

    public RecordInfo(int position, int length)
    {
      Position = position;
      Length = length;
    }
  }
}
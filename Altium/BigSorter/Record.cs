namespace Altium.BigSorter
{
  public struct Record
  {
    public readonly int Number;
    public readonly string String;

    public Record(int number, string @string)
    {
      Number = number;
      String = @string;
    }

    public int SizeInBytes
    {
      get { return sizeof(int) + String.Length * sizeof(char); }
    }
  }
}
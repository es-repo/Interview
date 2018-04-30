namespace Altium.BigSorter
{
  public struct Record
  {
    private static readonly int _sizeOfRecord = System.Runtime.InteropServices.Marshal.SizeOf<Record>();

    public readonly int Number;
    public readonly string String;

    public Record(int number, string @string)
    {
      Number = number;
      String = @string;
    }

    public int SizeInBytes
    {
      get { return _sizeOfRecord + String.Length * sizeof(char); }
    }
  }
}
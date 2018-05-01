namespace Altium.BigSorter
{
  /// <summary>
  /// Represents a record structure for the current test task.
  /// The implementation is intentionally is not flexible (does not implemented to
  /// be a record of any number and any types of fields) by performance reason.
  /// </summary>
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
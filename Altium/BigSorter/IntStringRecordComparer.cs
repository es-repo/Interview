namespace Altium.BigSorter
{
  public class IntStringRecordComparer : IRecordComparer
  {
    private readonly byte[] _buffer;
    private readonly int _field;

    public IntStringRecordComparer(byte[] buffer, int field)
    {
      _buffer = buffer;
      _field = field;
    }

    public int Compare(RecordInfo x, RecordInfo y)
    {
      int minLen = x.Length < y.Length ? x.Length : y.Length;
      for (int i = 0; i < minLen; i++)
      {
        if (_buffer[x.Position + i] < _buffer[y.Position + i]) return -1;
        if (_buffer[x.Position + i] > _buffer[y.Position + i]) return 1;
      }
      if (x.Length < y.Length) return -1;
      if (x.Length > y.Length) return 1;
      return 0;
    }
  }
}
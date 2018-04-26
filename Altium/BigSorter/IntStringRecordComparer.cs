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

    unsafe public int Compare(RecordInfo x, RecordInfo y)
    {
      fixed (byte* pb = _buffer)
      {
        byte* px = pb + x.Position;
        byte* py = pb + y.Position;

        int diff = *px - *py;
        if (diff != 0)
          return diff;
        
        int len = x.Length < y.Length ? x.Length : y.Length;
        while (len >= 8)
        {
          if (*((long*)px) != *((long*)py))
          {
            len = 8;
            break;
          }
          px += 8;
          py += 8;
          len -= 8;
        }

        while (len >= 4)
        {
          if (*((int*)px) != *((int*)py))
          {
            len = 4;
            break;
          }
          px += 4;
          py += 4;
          len -= 4;
        }

        while (len >= 2)
        {
          if (*((short*)px) != *((short*)py))
          {
            len = 2;
            break;
          }
          px += 2;
          py += 2;
          len -= 2;
        }

        while (len > 0)
        {
          diff = *px - *py;
          if (diff != 0)
            return diff;

          px++;
          py++;
          len--;
        }
      }
      //for (int i = 0; i < minLen; i++)
      //{
      //  if (_buffer[x.Position + i] < _buffer[y.Position + i]) return -1;
      //  if (_buffer[x.Position + i] > _buffer[y.Position + i]) return 1;
      //}
      if (x.Length < y.Length) return -1;
      if (x.Length > y.Length) return 1;
      return 0;
    }
  }
}
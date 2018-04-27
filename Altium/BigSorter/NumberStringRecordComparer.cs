using System;

namespace Altium.BigSorter
{
  public class NumberStringRecordComparer : IRecordComparer
  {
    private readonly byte[] _buffer;

    public NumberStringRecordComparer(byte[] buffer)
    {
      _buffer = buffer;
    }

    public IRecordFieldComparer CreateRecordFieldComparer(int field)
    {
      switch (field)
      {
        case 0: return new NumberFieldComparer(_buffer);
        case 1: return new StringFieldComparer(_buffer);
        default: throw new NotImplementedException();
      }
    }
  }

  public class StringFieldComparer : IRecordFieldComparer
  {
    private readonly byte[] _buffer;

    public StringFieldComparer(byte[] buffer)
    {
      _buffer = buffer;
    }

    unsafe public int Compare(RecordInfo x, RecordInfo y)
    {
      fixed (byte* pb = _buffer)
      {
        byte* px = pb + x.StringStart;
        byte* py = pb + y.StringStart;

        //while (*px != (byte)'.')
        //  px++;
        //px += 2;

        //while (*py != (byte)'.')
        //  py++;
        //py += 2;

        int diff = *px - *py;
        if (diff != 0)
          return diff;

        int len = x.StringLength < y.StringLength ? x.StringLength : y.StringLength;
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
      return x.StringLength - y.StringLength;
    }
  }

  public class NumberFieldComparer : IRecordFieldComparer
  {
    private readonly byte[] _buffer;

    public NumberFieldComparer(byte[] buffer)
    {
      _buffer = buffer;
    }

    unsafe public int Compare(RecordInfo x, RecordInfo y)
    {
      return x.Number - y.Number;
      //fixed (byte* pb = _buffer)
      //{
      //  byte* xp = pb + x.Position;
      //  byte* yp = pb + y.Position;

      //  int diff = 0;
      //  while (true)
      //  {
      //    if (diff == 0)
      //      diff = *xp - *yp;

      //    xp++;
      //    yp++;

      //    if (*xp == (byte)'.')
      //      return *yp == (byte)'.' ? diff : -1;

      //    if (*yp == (byte)'.')
      //      return 1;
      //  }
      //}

      //int xp = x.Position;
      //int yp = y.Position;
      //int diff = 0;
      //while(true)
      //{
      //  if (diff == 0)
      //    diff = _buffer[xp] - _buffer[yp];

      //  xp++;
      //  yp++;

      //  if (_buffer[xp] == (byte)'.')
      //    return _buffer[yp] == (byte)'.' ? diff : -1;

      //  if (_buffer[yp] == (byte)'.')
      //    return 1;
      //}
    }
  }
}
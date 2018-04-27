using System;

namespace Altium.BigSorter
{
  public class RecordComparer : IRecordComparer
  {
    private readonly byte[] _buffer;

    public RecordComparer(byte[] buffer)
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

    unsafe public int Compare(Record x, Record y)
    {
      fixed (byte* pb = _buffer)
      {
        byte* px = pb + x.StringStart;
        byte* py = pb + y.StringStart;

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

    unsafe public int Compare(Record x, Record y)
    {
      return x.Number - y.Number;
    }
  }
}
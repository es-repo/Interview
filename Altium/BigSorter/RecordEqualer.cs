using System;

namespace Altium.BigSorter
{
  public class RecordEqualer : IRecordEqualer
  {
    private readonly byte[] _buffer;

    public RecordEqualer(byte[] buffer)
    {
      _buffer = buffer;
    }

    public IRecordFieldEqualer CreateRecordFieldEqualer(int field)
    {
      switch (field)
      {
        case 0:
          return new NumberFieldEqualer();
        case 1:
          return new StringFieldEqualer(_buffer);
        default:
          throw new NotImplementedException();
      }
    }
  }

  public class StringFieldEqualer : IRecordFieldEqualer
  {
    private byte[] _buffer;

    public StringFieldEqualer(byte[] buffer)
    {
      _buffer = buffer;
    }

    unsafe public bool Equals(Record x, Record y)
    {
      if (x.StringLength != y.StringLength)
        return false;

      fixed(byte * pb = _buffer)
      {
        byte * px = pb + x.StringStart;
        byte * py = pb + y.StringStart;

        if ( * px != * py)
          return false;

        int len = x.StringLength;
        while (len >= 8)
        {
          if ( * ((long * ) px) != * ((long * ) py))
            return false;

          px += 8;
          py += 8;
          len -= 8;
        }

        while (len >= 4)
        {
          if ( * ((long * ) px) != * ((long * ) py))
            return false;

          px += 4;
          py += 4;
          len -= 4;
        }

        while (len >= 2)
        {
          if ( * ((long * ) px) != * ((long * ) py))
            return false;

          px += 2;
          py += 2;
          len -= 2;
        }

        if ( * ((long * ) px) != * ((long * ) py))
          return false;
      }
      
      return true;
    }
  }

  public class NumberFieldEqualer : IRecordFieldEqualer
  {
    public bool Equals(Record x, Record y)
    {
      return x.Number == y.Number;
    }
  }
}
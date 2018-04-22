using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altium.BigSorter
{
  public class ByteConverter
  {
    public object To(Type type, ArrayView<byte> bytes)
    {
      if (type == typeof(int))
        return ToInt32(bytes);

      if (type == typeof(string))
        return ToString(bytes);

      throw new NotImplementedException();
    }

    public int ToInt32(ArrayView<byte> bytes)
    {
      int value = 0;
      long s = sizeof(int);
      for (long i = 0; i < s; i++)
      {
        value <<= 8;
        value += bytes[i];
      }
      return value;
    }

    public string ToString(ArrayView<byte> bytes)
    {
      char[] chars = new char[bytes.Length >> 1];
      int ch = 0;
      for (long i = 0, k = 0; i < bytes.Length; i++)
      {
        ch <<= 8;
        ch += bytes[i];

        if (i % 2 == 1)
        {
          chars[k++] = (char) ch;
          ch = 0;
        }
      }

      return new string(chars);
    }

    public void ToBytes(object o, ArrayView<byte> bytes)
    {
      Type type = o.GetType();

      if (type == typeof(int))
        ToBytes((int) o, bytes);
      else if (type == typeof(string))
        ToBytes((string) o, bytes);
      else
        throw new NotImplementedException();
    }

    public void ToBytes(int value, ArrayView<byte> bytes)
    {
      long s = sizeof(int);
      for (long i = 0, k = s - 1; i < s; i++, k--)
      {
        byte b = (byte) value;
        bytes[k] = b;
        value >>= 8;
      }
    }

    public void ToBytes(string value, ArrayView<byte> bytes)
    {
      long j = 0;
      for (int i = 0; i < value.Length; i++)
      {
        bytes[j + 1] = (byte) value[i];
        bytes[j] = (byte) (value[i] >> 8);
        j += 2;
      }
    }
  }
}
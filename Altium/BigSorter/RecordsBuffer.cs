using System;
using System.Collections.Generic;
using System.IO;

namespace Altium.BigSorter
{
  public class OutOfSpaceException : Exception { }

  /// <summary>
  /// This class serves to store big amount of strings without worring of OutOfMemoryException
  /// during a client code lifetime.
  /// 
  /// A client code at the begining of its workflow specifyes length of the buffer.
  /// The buffer allocates necessary amount of memory. 
  /// Then the client code can start to add strings to the buffer.
  ///    
  /// Internal buffer is a big byte array which will be filled from both sides:
  ///  - From its begining it will be filled with byte sequences into which incoming strings
  ///   will be converted.
  ///  - From its end it will be filled with 2 values for every incoming string:
  ///     1) position of begining of a string's byte secuence
  ///     2) length of a string in bytes.
  /// 
  /// That is internally there will be handled two indexes which will move towards each other: 
  ///   1) current position for next a string's byte secuence.
  ///   2) current position for next a string's byte secuence's position and length.
  /// 
  /// When these two indexes will be attempted to overlap that will mean that the buffer is filled
  /// and cant accept strings anymore.
  /// 
  /// 
  /// For optimization purpose this class is parametrized with two types:
  ///   
  /// - TPosition is a type which defines maximum allowed number of strings. 
  /// Appropriate types would be: ubyte, ushort, int, long.
  /// 
  /// - TLength is a type which defined maxumum allowed lenght of a string.
  /// Appropriate types would be: ubyte, ushort, int, long.
  /// 
  /// This way client code could somewhat optimize memory usage depending on whether 
  /// there are will be bigger amount of shorter strings or lesser amount of longer strings.
  /// </summary>
  public class RecordsBuffer
  {
    private readonly ArrayView<byte> _buffer;
    private readonly int _sizeOfPositionType;
    private readonly long _sizeOfPosAndLenTypes;
    private long _forwardIndex;
    private long _backwardIndex;
    private readonly Type _positionType;
    private readonly IRecordBytesConverter _recordBytesConverter;
    
    public readonly int SizeOfRecordLengthType;
    
    /// <param name="buffer">A byte array which will be used as internal buffer.</param>
    public RecordsBuffer(ArrayView<byte> buffer, IRecordBytesConverter recordBytesConverter, int sizeOfRecordLenType = 4)
    {
      _buffer = buffer;
      _recordBytesConverter = recordBytesConverter;

      _forwardIndex = 0;
      _backwardIndex = buffer.Length - 1;

      _positionType = buffer.Length > int.MaxValue ? typeof(long) : typeof(int);
      _sizeOfPositionType = (int) System.Runtime.InteropServices.Marshal.SizeOf(_positionType);
      SizeOfRecordLengthType = (int) sizeOfRecordLenType;
      _sizeOfPosAndLenTypes = (long) (_sizeOfPositionType + SizeOfRecordLengthType);
    }

    public long RecordsCount
    {
      get
      {
        return (_buffer.Length - _backwardIndex) / _sizeOfPosAndLenTypes;
      }
    }

    public bool AddRecord(IEnumerable<object> values)
    {
      if (_backwardIndex < _forwardIndex + _sizeOfPosAndLenTypes)
        return false;

      long maxLen = _backwardIndex - _forwardIndex - _sizeOfPosAndLenTypes + 1;

      ArrayView<byte> arrayView = new ArrayView<byte>(_buffer, _forwardIndex, maxLen);
      long recordLen = _recordBytesConverter.SetBytes(values, arrayView);
      if (recordLen == 0)
        return false;

      AddBackward(_forwardIndex, _sizeOfPositionType);
      AddBackward(recordLen, SizeOfRecordLengthType);
      _forwardIndex += recordLen;
      return true;
    }

    public bool AddRecordBytes(Stream stream, long recordLength)
    {
      if (_backwardIndex < _forwardIndex + _sizeOfPosAndLenTypes + recordLength - 1)
        return false;

      AddBackward(_forwardIndex, _sizeOfPositionType);
      AddBackward(recordLength, SizeOfRecordLengthType);
      for(long i = 0; i < recordLength; i++)
      {
        byte b = (byte)stream.ReadByte();
        _buffer[_forwardIndex++] = b;
      }
      return true;
    }

    public bool AddRecordBytes(ArrayView<byte> recordsBytes)
    {
      if (_backwardIndex < _forwardIndex + _sizeOfPosAndLenTypes + recordsBytes.Length - 1)
        return false;

      AddBackward(_forwardIndex, _sizeOfPositionType);
      AddBackward(recordsBytes.Length, SizeOfRecordLengthType);
      for(long i = 0; i < recordsBytes.Length; i++)
      {
        _buffer[_forwardIndex++] = recordsBytes[i];
      }
      return true;
    }

    private void AddBackward(long value, int bytesCount)
    {
      for (int i = 0; i < bytesCount; i++)
      {
        byte b = (byte) value;
        _buffer[_backwardIndex--] = b;
        value = value >> 8;
      }
    }

    public IEnumerable<object> GetRecord(long recordIndex)
    {
      ArrayView<byte> bytes = GetRecordBytes(recordIndex);
      return _recordBytesConverter.GetValues(bytes);
    }

    public ArrayView<byte> GetRecordBytes(long recordIndex)
    {
      long recordPosition, recordLength;
      GetRecordPositionAndLength(recordIndex, out recordPosition, out recordLength);
      return new ArrayView<byte>(_buffer, recordPosition, recordLength);
    }

    private void GetRecordPositionAndLength(long recordIndex, out long position, out long length)
    {
      long lengthIndex = _buffer.Length - (recordIndex + 1) * _sizeOfPosAndLenTypes;
      length = GetValue(lengthIndex, SizeOfRecordLengthType);

      long positionIndex = lengthIndex + SizeOfRecordLengthType;
      position = GetValue(positionIndex, _sizeOfPositionType);
    }

    private long GetValue(long position, int bytesCount)
    {
      long value = 0;
      for (int i = 0; i < bytesCount; i++)
      {
        value <<= 8;
        value += _buffer[position++];
      }
      return value;
    }

    public void Sort(int fieldIndex)
    {
      RecordComparer comparer = new RecordComparer(_recordBytesConverter, fieldIndex);
      QuickSort(0, RecordsCount - 1, comparer);
    }

    private void QuickSort(long lo, long hi, RecordComparer comparer)
    {
      if (lo < hi)
      {
        long p = Partition(lo, hi, comparer);
        QuickSort(lo, p - 1, comparer);
        QuickSort(p + 1, hi, comparer);
      }
    }

    private long Partition(long lo, long hi, RecordComparer comparer)
    {
      ArrayView<byte> pivot = GetRecordBytes(hi);
      long i = lo - 1;
      for (long j = lo; j < hi; j++)
      {
        ArrayView<byte> jRecord = GetRecordBytes(j);
        if (comparer.Compare(pivot, jRecord) > 0)
        {
          i++;
          SwapRecords(i, j);
        }
      }
      SwapRecords(i + 1, hi);
      return i + 1;
    }

    public void SwapRecords(long aRecordIndex, long bRecordIndex)
    {
      if (aRecordIndex == bRecordIndex)
        return;

      long ai = _buffer.Length - (aRecordIndex + 1) * _sizeOfPosAndLenTypes;
      long bi = _buffer.Length - (bRecordIndex + 1) * _sizeOfPosAndLenTypes;
      for (long i = 0; i < _sizeOfPosAndLenTypes; i++)
      {
        byte t = _buffer[ai];
        _buffer[ai] = _buffer[bi];
        _buffer[bi] = t;
        ai++;
        bi++;
      }
    }
  }
}
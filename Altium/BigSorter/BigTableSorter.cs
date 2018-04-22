using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Altium.BigSorter
{
  public class BigTableSorter
  {
    private readonly IRecordParser _recordParser;
    private readonly IRecordBytesConverter _recordBytesConverter;
    private BigArray<byte> _buffer;

    public BigTableSorter(BigArray<byte> buffer, IRecordParser recordParser, IRecordBytesConverter recordBytesConverter)
    {
      _buffer = buffer;
      _recordParser = recordParser;
      _recordBytesConverter = recordBytesConverter;
    }

    public void Sort(Stream input, int field, Stream output, ITempStreams tempStreams)
    {
      Sort(input, new int[] { field }, output, tempStreams);
    }

    public void Sort(Stream input, int[] fields, Stream output, ITempStreams tempStreams)
    {
      int prevField = -1;
      Stream originalOutput = output;
      int tempOutputFirst = fields.Length % 2;
      Stream tempOutput = fields.Length > 1 ? tempStreams.CreateTempOutputStream() : null;

      try
      {
        for (int i = 0; i < fields.Length; i++)
        {
          output = (i % 2) == tempOutputFirst ? tempOutput : originalOutput;
          input.Position = 0;
          output.Position = 0;
          StreamReader sr = new StreamReader(input);
          StreamWriter sw = new StreamWriter(output);
          ArrayView<byte> av = new ArrayView<byte>(_buffer, 0);
          RecordsReader recordsReader = new RecordsReader(_recordParser, _recordBytesConverter, sr, av, prevField);
          while (!recordsReader.IsEnd)
          {
            Sort(recordsReader, fields[i], sw, tempStreams);
          }
          sw.Flush();
          prevField = fields[i];
          input = output;
        }
      }
      finally
      {
        if (tempOutput != null)
          tempOutput.Dispose();
      }
    }

    private void Sort(RecordsReader recordsReader, int field, StreamWriter output, ITempStreams tempStreams)
    {
      RecordsBuffer firstBlock;
      int blockCount = SortBlocks(recordsReader, field, tempStreams, out firstBlock);
      if (blockCount == 1)
      {
        RecordsWriter recordsWriter = new RecordsWriter(_recordParser, output);
        recordsWriter.WriteRecords(firstBlock);
      }
      else
      {
        MergeBlocks(tempStreams, blockCount, field, output);
        tempStreams.ClearBlocks();
      }
    }

    private int SortBlocks(RecordsReader recordsReader, int field, ITempStreams tempStreams,
      out RecordsBuffer firstBlock)
    {
      int blockIndex = 0;
      firstBlock = null;
      IEnumerable<RecordsBuffer> blocks = recordsReader.ReadBlocks();
      foreach (RecordsBuffer block in blocks)
      {
        block.Sort(field);

        if (blockIndex == 0)
        {
          firstBlock = block;
          if (recordsReader.IsLastBlock)
            return 1;
        }

        using(Stream blockStream = tempStreams.CreateBlockStream(blockIndex))
        {
          RecordsWriter recordsWriter = new RecordsWriter(_recordParser, blockStream);
          recordsWriter.WriteRecordsRaw(block);
        }
        blockIndex++;
      }
      return blockIndex;
    }

    private void MergeBlocks(ITempStreams tempStreams, int blockCount, int field, StreamWriter output)
    {
      long blockLen = _buffer.Length / ((long) blockCount + 1);

      List<ArrayView<byte>> blockBuffers = new List<ArrayView<byte>>();
      for (int i = 0; i < blockCount; i++)
      {
        blockBuffers.Add(new ArrayView<byte>(_buffer, (int) i * blockLen, blockLen));
      }

      ArrayView<byte> outputBuffer = new ArrayView<byte>(_buffer, (long) blockCount * blockLen);

      List<Stream> blockStreams = new List<Stream>();
      try
      {
        List<IEnumerator<ArrayView<byte>> > blockRecordsEnumerators = new List<IEnumerator<ArrayView<byte>> > ();
        for (int i = 0; i < blockCount; i++)
        {
          Stream blockStream = tempStreams.CreateBlockStream(i);
          blockStreams.Add(blockStream);
          RecordsReader blockReader = new RecordsReader(_recordParser, _recordBytesConverter, blockStream, blockBuffers[i]);
          blockRecordsEnumerators.Add(blockReader.ReadRecordBytes().GetEnumerator());
        }

        RecordsBufferedWriter recordsWriter =
          new RecordsBufferedWriter(_recordParser, _recordBytesConverter, output, outputBuffer);

        MergeBlocks(blockRecordsEnumerators, recordsWriter, field, output);
      }
      finally
      {
        foreach (Stream s in blockStreams)
          s.Dispose();
      }
    }

    private void MergeBlocks(List<IEnumerator<ArrayView<byte>> > blockRecordsEnumerators,
      RecordsBufferedWriter recordsWriter, int field, StreamWriter outputStream)
    {
      var currentRecordComparer = new EnumeratorCurrentComparer<ArrayView<byte>>(
        new RecordComparer(_recordBytesConverter, field));

      foreach (var e in blockRecordsEnumerators)
      {
        e.MoveNext();
      }

      blockRecordsEnumerators.Sort(currentRecordComparer);

      while (blockRecordsEnumerators.Count > 0)
      {
        IEnumerator<ArrayView<byte>> min = blockRecordsEnumerators[0];
        recordsWriter.WriteRecord(min.Current);

        bool empty = !min.MoveNext();
        if (empty)
        {
          blockRecordsEnumerators.RemoveAt(0);
        }
        else if (blockRecordsEnumerators.Count > 1)
        {
          int i = blockRecordsEnumerators.BinarySearch(1, blockRecordsEnumerators.Count - 1,
            min, currentRecordComparer);

          if ((i < 0 && ~i > 1) || (i > 1))
          {
            int pos = (i < 0 ? ~i : i) - 1;
            blockRecordsEnumerators.RemoveAt(0);
            blockRecordsEnumerators.Insert(pos, min);
          }
        }
      }

      recordsWriter.Flush();
    }
  }
}
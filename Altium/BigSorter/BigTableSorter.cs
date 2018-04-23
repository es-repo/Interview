using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Altium.BigSorter
{
  public class BigTableSorter
  {
    private readonly IRecordParser _recordParser;
    private readonly long _bufferSizeInBytes;    

    public BigTableSorter(long bufferSizeInBytes, IRecordParser recordParser)
    {
      _bufferSizeInBytes = bufferSizeInBytes;
      _recordParser = recordParser;
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
          RecordsReader recordsReader = new RecordsReader(_bufferSizeInBytes, _recordParser, sr, prevField);
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
        Console.WriteLine("Sorted block with " + block.Records.Count + " records");
        GC.Collect();
        if (blockIndex == 0)
        {
          firstBlock = block;
          if (recordsReader.IsLastBlock)
            return 1;
        }

        using(Stream blockStream = tempStreams.CreateBlockStream(blockIndex))
        using(StreamWriter sw = new StreamWriter(blockStream)){
          RecordsWriter recordsWriter = new RecordsWriter(_recordParser, sw);
          recordsWriter.WriteRecords(block);
        }
        blockIndex++;
      }
      return blockIndex;
    }

    private void MergeBlocks(ITempStreams tempStreams, int blockCount, int field, StreamWriter output)
    {
      long blockLen = _bufferSizeInBytes / ((long) blockCount + 1);

      List<Stream> blockStreams = new List<Stream>();
      try
      {
        List<IEnumerator<object[]> > blockRecordsEnumerators = new List<IEnumerator<object[]> > ();
        for (int i = 0; i < blockCount; i++)
        {
          Stream blockStream = tempStreams.CreateBlockStream(i);
          StreamReader sr = new StreamReader(blockStream);
          blockStreams.Add(blockStream);
          RecordsReader blockReader = new RecordsReader(blockLen, _recordParser, sr);
          blockRecordsEnumerators.Add(blockReader.ReadRecords().GetEnumerator());
        }

        RecordsBufferedWriter recordsWriter =
          new RecordsBufferedWriter(_recordParser, output, _bufferSizeInBytes);

        MergeBlocks(blockRecordsEnumerators, recordsWriter, field, output);
      }
      finally
      {
        foreach (Stream s in blockStreams)
          s.Dispose();
      }
    }

    private void MergeBlocks(List<IEnumerator<object[]> > blockRecordsEnumerators,
      RecordsBufferedWriter recordsWriter, int field, StreamWriter outputStream)
    {
      var currentRecordComparer = new EnumeratorCurrentComparer<object[]>(new RecordComparer(field));

      foreach (var e in blockRecordsEnumerators)
      {
        e.MoveNext();
      }

      blockRecordsEnumerators.Sort(currentRecordComparer);

      while (blockRecordsEnumerators.Count > 0)
      {
        IEnumerator<object[]> min = blockRecordsEnumerators[0];
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
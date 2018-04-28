using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Altium.BigSorter
{
  public class BigTableSorter
  {
    private readonly RecordComparer _recordComparer;
    private readonly long _bufferSizeInBytes;    

    public BigTableSorter(long bufferSizeInBytes)
    {
      _recordComparer = new RecordComparer();
      _bufferSizeInBytes = bufferSizeInBytes;
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
          RecordsReader recordsReader = new RecordsReader(sr, _bufferSizeInBytes, prevField);
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
        RecordsWriter recordsWriter = new RecordsWriter(output);
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
        using(StreamWriter sw = new StreamWriter(blockStream)){
          RecordsWriter recordsWriter = new RecordsWriter(sw);
          recordsWriter.WriteRecords(block);
        }
        blockIndex++;
      }
      return blockIndex;
    }

    private void MergeBlocks(ITempStreams tempStreams, int blockCount, int field, StreamWriter output)
    {
      long blockSize = _bufferSizeInBytes / (blockCount + 1);

      List<Stream> blockStreams = new List<Stream>();
      try
      {
        List<IEnumerator<Record>> blockRecordsEnumerators = new List<IEnumerator<Record>>();
        for (int i = 0; i < blockCount; i++)
        {
          Stream blockStream = tempStreams.CreateBlockStream(i);
          blockStreams.Add(blockStream);
          StreamReader blockStreamReader = new StreamReader(blockStream);
          RecordsReader blockReader = new RecordsReader(blockStreamReader, blockSize);
          blockRecordsEnumerators.Add(blockReader.ReadRecords().GetEnumerator());
        }

        using(BufferedRecordsWriter recordsWriter = new BufferedRecordsWriter(output, blockSize))
        {
          MergeBlocks(blockRecordsEnumerators, recordsWriter, field);
        }
      }
      finally
      {
        foreach (Stream s in blockStreams)
          s.Dispose();
      }
    }

    private void MergeBlocks(List<IEnumerator<Record>> blockRecordsEnumerators,
      BufferedRecordsWriter recordsWriter, int field)
    {
      IRecordFieldComparer comparer = _recordComparer.CreateRecordFieldComparer(field);
      var currentRecordComparer = new SelectComparer<int, Record>(
        i => blockRecordsEnumerators[i].Current,
        comparer);

      var heap = new MinHeap<int>(blockRecordsEnumerators.Count, currentRecordComparer);
      for (int i = 0; i < blockRecordsEnumerators.Count; i++)
      {
        blockRecordsEnumerators[i].MoveNext();
        heap.Add(i);
      }

      while (heap.Size > 0)
      {
        recordsWriter.WriteRecord(blockRecordsEnumerators[heap.Root].Current);

        bool empty = !blockRecordsEnumerators[heap.Root].MoveNext();
        if (empty)
          heap.Extract();
        else
          heap.DownHeap();
      }
    }
  }
}
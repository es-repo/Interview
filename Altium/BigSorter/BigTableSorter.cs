using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Altium.BigSorter
{
  public class BigTableSorter
  {
    private readonly byte[] _buffer;
    private readonly CreateRecordComparer _createRecordComparer;

    public BigTableSorter(byte[] buffer, CreateRecordComparer createRecordComparer)
    {
      _buffer = buffer;
      _createRecordComparer = createRecordComparer;
    }

    public void Sort(Stream input, int field, Stream output, ITempStreams tempStreams)
    {
      Sort(input, new int[] { field }, output, tempStreams);
    }

    public void Sort(Stream input, int[] fields, Stream output, ITempStreams tempStreams)
    {
      //int prevField = -1;
      //Stream originalOutput = output;
      //int tempOutputFirst = fields.Length % 2;
      //Stream tempOutput = fields.Length > 1 ? tempStreams.CreateTempOutputStream() : null;

      ArrayView<byte> bufferView = new ArrayView<byte>(_buffer, 0);

      try
      {
        for (int i = 0; i < fields.Length; i++)
        {
          //  output = (i % 2) == tempOutputFirst ? tempOutput : originalOutput;
          //input.Position = 0;
          //output.Position = 0;
          //StreamReader sr = new StreamReader(input);
          //StreamWriter sw = new StreamWriter(output);
          RecordsReader recordsReader = new RecordsReader(bufferView, input, _createRecordComparer /*prevField*/ );
          // while (!recordsReader.IsEnd)
          // {
          Sort(recordsReader, fields[i], output, tempStreams);
          // }
          output.Flush();
          //prevField = fields[i];
          //input = output;
        }
      }
      finally
      {
        //if (tempOutput != null)
        //  tempOutput.Dispose();
      }
    }

    private void Sort(RecordsReader recordsReader, int field, Stream output, ITempStreams tempStreams)
    {
      RecordsBuffer firstBlock;

      Stopwatch swSort = new Stopwatch();
      swSort.Start();
      int blockCount = SortBlocks(recordsReader, field, tempStreams, out firstBlock);
      swSort.Stop();
      Console.WriteLine($"Sorted {blockCount} bloks in {swSort.Elapsed}");

      if (blockCount == 1)
      {
        RecordsWriter recordsWriter = new RecordsWriter(output);
        recordsWriter.Write(firstBlock);
      }
      else
      {
        Stopwatch swMerge = new Stopwatch();
        swMerge.Start();
        MergeBlocks(tempStreams, blockCount, field, output);
        swMerge.Stop();
        Console.WriteLine($"Merged {blockCount} bloks in {swMerge.Elapsed}");

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
          RecordsWriter recordsWriter = new RecordsWriter(blockStream);
          recordsWriter.Write(block);
        }
        blockIndex++;
      }
      return blockIndex;
    }

    private void MergeBlocks(ITempStreams tempStreams, int blockCount, int field, Stream output)
    {
      int blockLen = _buffer.Length / (blockCount + 1);

      List<Stream> blockStreams = new List<Stream>();
      try
      {
        List<IEnumerator<RecordInfo>> blockRecordsEnumerators = new List<IEnumerator<RecordInfo>>();
        for (int i = 0; i < blockCount; i++)
        {
          Stream blockStream = tempStreams.CreateBlockStream(i);
          blockStreams.Add(blockStream);
          ArrayView<byte> blockBuffer = new ArrayView<byte>(_buffer, i * blockLen, blockLen);
          RecordsReader blockReader = new RecordsReader(blockBuffer, blockStream, null);
          blockRecordsEnumerators.Add(blockReader.ReadRecords().GetEnumerator());
        }

        ArrayView<byte> outputBlockBuffer = new ArrayView<byte>(_buffer, blockCount * blockLen);
        using(BufferedRecordsWriter recordsWriter =
          new BufferedRecordsWriter(outputBlockBuffer, output))
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

    private void MergeBlocks(List<IEnumerator<RecordInfo>> blockRecordsEnumerators,
      BufferedRecordsWriter recordsWriter, int field)
    {
      IRecordComparer recordComparer = _createRecordComparer(_buffer, field);
      var currentRecordComparer = new EnumeratorCurrentComparer<RecordInfo>(recordComparer);

      var heap = new MinHeap<IEnumerator<RecordInfo>>(blockRecordsEnumerators.Count, currentRecordComparer);
      foreach (var e in blockRecordsEnumerators)
      {
        e.MoveNext();
        heap.Add(e);
      }

      while (heap.Size > 0)
      {
        recordsWriter.WriteRecord(heap.Root.Current);

        bool empty = !heap.Root.MoveNext();
        if (empty)
          heap.Extract();
        else
          heap.DownHeap();
      }
    }
  }
}
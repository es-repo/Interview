using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Altium.BigSorter
{
  public class BigTableSorter
  {
    private readonly RecordComparer _recordComparer;
    private readonly long _bufferSizeInBytes;
    private readonly int _workersCount;

    public BigTableSorter(long bufferSizeInBytes)
    {
      _recordComparer = new RecordComparer();
      _workersCount = Environment.ProcessorCount;
      _bufferSizeInBytes = bufferSizeInBytes / _workersCount;
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
          Console.WriteLine($"Sorting by field {fields[i]}...");
          Stopwatch stopwatch = new Stopwatch();
          stopwatch.Start();

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

          stopwatch.Stop();
          Console.WriteLine($"Sorting by field {fields[i]} done in {stopwatch.Elapsed}");
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
      Stopwatch sortSw = new Stopwatch();
      sortSw.Start();
      RecordsBuffer firstBlock;
      int blockCount = SortBlocks(recordsReader, field, tempStreams, out firstBlock);
      sortSw.Stop();
      if (blockCount == 1)
      {
        RecordsWriter recordsWriter = new RecordsWriter(output);
        recordsWriter.WriteRecords(firstBlock);
      }
      else
      {
        Console.WriteLine($"{blockCount} blocks sorted in {sortSw.Elapsed}");

        Stopwatch mergeSw = new Stopwatch();
        mergeSw.Start();
        MergeBlocks(tempStreams, blockCount, field, output);
        mergeSw.Stop();
        Console.WriteLine($"{blockCount} blocks merged in {mergeSw.Elapsed}");

        tempStreams.ClearBlocks();
      }
    }

    //private int SortBlocks(RecordsReader recordsReader, int field, ITempStreams tempStreams,
    //  out RecordsBuffer firstBlock)
    //{
    //  int blockIndex = 0;
    //  firstBlock = null;
    //  IEnumerable<RecordsBuffer> blocks = recordsReader.ReadBlocks();
    //  foreach (RecordsBuffer block in blocks)
    //  {
    //    block.Sort(field);
    //    if (blockIndex == 0)
    //    {
    //      if (recordsReader.IsLastBlock)
    //      {
    //        firstBlock = block;
    //        return 1;
    //      }
    //    }

    //    using (Stream blockStream = tempStreams.CreateBlockStream(blockIndex))
    //    using (StreamWriter sw = new StreamWriter(blockStream))
    //    {
    //      RecordsWriter recordsWriter = new RecordsWriter(sw);
    //      recordsWriter.WriteRecords(block);
    //    }
    //    blockIndex++;
    //    Console.WriteLine($"Block {blockIndex} sorted");
    //  }
    //  return blockIndex;
    //}

    private int SortBlocks3(RecordsReader recordsReader, int field, ITempStreams tempStreams, out RecordsBuffer firstBlock)
    {
      firstBlock = null;
      IEnumerator<RecordsBuffer> blocks = recordsReader.ReadBlocks().GetEnumerator();
      if (!blocks.MoveNext())
        return 0;

      if (recordsReader.IsLastBlock)
      {
        firstBlock = blocks.Current;
        firstBlock.ParallelSort(field);
        return 1;
      }

      int blockCount = 0;
      int serie = 0;
      while(blocks.Current != null)
      {
        Parallel.For(0, _workersCount, (i, state) =>
        {
          RecordsBuffer block = null;
          lock (blocks)
          {
            block = blocks.Current;
            blockCount++;
            blocks.MoveNext();
          }

          if (block == null)
            return;

          int blockIndex = serie * _workersCount + i;
          Console.WriteLine($"Block {blockIndex + 1} sorting started...");
          
          block.ParallelSort(field);
          using (Stream blockStream = tempStreams.CreateBlockStream(blockIndex))
          using (StreamWriter sw = new StreamWriter(blockStream))
          {
            RecordsWriter recordsWriter = new RecordsWriter(sw);
            recordsWriter.WriteRecords(block);
          }
          Console.WriteLine($"Block {blockIndex + 1} sorted");
        });
        serie++;
      }
      return blockCount;
    }


    private int SortBlocks(RecordsReader recordsReader, int field, ITempStreams tempStreams, out RecordsBuffer firstBlock)
    {
      firstBlock = null;
      IEnumerator<RecordsBuffer> blocks = recordsReader.ReadBlocks().GetEnumerator();
      if (!blocks.MoveNext())
        return 0;

      if (recordsReader.IsLastBlock)
      {
        firstBlock = blocks.Current;
        firstBlock.ParallelSort(field);
        return 1;
      }

      int blockIndex = 0;
      using (var blockCollection = new BlockingCollection<Tuple<RecordsBuffer, int>>(_workersCount))
      using (var sortCompletionCollection = new BlockingCollection<bool>(_workersCount))
      {
        Task blocksReadingTask = Task.Factory.StartNew(() =>
        {
          do
          {
            blockCollection.Add(new Tuple<RecordsBuffer, int>(blocks.Current, blockIndex++));
            sortCompletionCollection.Add(true);
          } while (blocks.MoveNext());
          blockCollection.CompleteAdding();
        });

        Task blocksSortingTask = Task.Factory.StartNew(() =>
        {
          List<Task> sortTasks = new List<Task>();
          try
          {
            while (true)
            {
              Tuple<RecordsBuffer, int> blockAndIndex = blockCollection.Take();

              Task t = StartBlockSortingTask(blockAndIndex.Item1, blockAndIndex.Item2,
                field, tempStreams, sortCompletionCollection);
              sortTasks.Add(t);
            }
          }
          catch (InvalidOperationException)
          {
            // An InvalidOperationException means that Take() was called on a completed collection
          }
          Task.WaitAll(sortTasks.ToArray());
        });

        Task.WaitAll(blocksReadingTask, blocksSortingTask);
      }
      return blockIndex;
    }

    private static Task StartBlockSortingTask(RecordsBuffer block, int blockIndex, int field,
      ITempStreams tempStreams, BlockingCollection<bool> sortCompletionCollection)
    {
      return Task.Factory.StartNew(() =>
      {
        block.ParallelSort(field);
        using (Stream blockStream = tempStreams.CreateBlockStream(blockIndex))
        using (StreamWriter sw = new StreamWriter(blockStream))
        {
          RecordsWriter recordsWriter = new RecordsWriter(sw);
          recordsWriter.WriteRecords(block);
        }
        sortCompletionCollection.Take();
        Console.WriteLine($"Block {blockIndex} sorted");
      });
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

        using (BufferedRecordsWriter recordsWriter = new BufferedRecordsWriter(output, blockSize))
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
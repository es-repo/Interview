using System;
using System.IO;

namespace Altium.BigSorter
{
  /// <summary>
  /// Interface to create streams to store temp data.
  /// </summary>
  public interface ITempStreams : IDisposable
  {
    /// <summary>
    /// Creates a stream to store one sorted block of table.
    /// </summary>
    Stream CreateBlockStream(int blockIndex);

    /// <summary>
    /// When table is sorted by more then one field then
    /// intermediate sorted table will be written into this stream.
    /// </summary>
    Stream CreateTempOutputStream();

    /// <summary>
    /// Clears temp sorted blocks.
    /// </summary>
    void ClearBlocks();
  }
}
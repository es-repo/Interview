using System;
using System.IO;

namespace Altium.BigSorter
{
  public interface ITempStreams : IDisposable
  {
    Stream CreateBlockStream(int blockIndex);
    Stream CreateTempOutputStream();
    void ClearBlocks();
  }
}
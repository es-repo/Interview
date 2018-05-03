using System.Collections.Generic;
using System.IO;

namespace Altium.BigSorter
{
  /// <summary>
  /// Implementation of ITempStreams interface to store temp data in file system.
  /// (Another implementation resides in unit test project which store data in memory).
  /// </summary>
  public class FileTempStreams : ITempStreams
  {
    private readonly string _tempDir;
    private readonly string _blocksDir;
    private readonly string _tempOutputFileName; 

    public FileTempStreams(string tempDir)
    {
      _tempDir =  tempDir;
      _blocksDir = Path.Combine(tempDir, "blocks");
      _tempOutputFileName = Path.Combine(tempDir, "output.txt");
      Dispose();
    }

    public Stream CreateBlockStream(int blockIndex)
    {
      Directory.CreateDirectory(_blocksDir);
      string fn = Path.Combine(_blocksDir, GetBlockFileName(blockIndex));
      return new FileStream(fn, FileMode.CreateNew, FileAccess.Write);
    }

    public Stream OpenBlockStream(int blockIndex)
    {
      string fn = Path.Combine(_blocksDir, GetBlockFileName(blockIndex));
      return new FileStream(fn, FileMode.Open, FileAccess.Read);
    }

    public Stream CreateTempOutputStream()
    {
      Directory.CreateDirectory(_tempDir);
      FileStream fs = new FileStream(_tempOutputFileName, FileMode.CreateNew, FileAccess.ReadWrite);
      BufferedStream bs = new BufferedStream(fs);
      return bs;
    }

    public void ClearBlocks()
    {
      if (Directory.Exists(_blocksDir))
        Directory.Delete(_blocksDir, true);
    }

    private static string GetBlockFileName(int blockIndex)
    {
      return "block_" + blockIndex + ".txt";
    }

    public void Dispose()
    {
      ClearBlocks();
      if(File.Exists(_tempOutputFileName))
        File.Delete(_tempOutputFileName);
    }
  }
}
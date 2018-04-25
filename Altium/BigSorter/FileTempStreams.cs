using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Altium.BigSorter
{
  public class FileTempStreams : ITempStreams
  {
    private readonly string _tempDir;
    private readonly string _blocksDir;
    private readonly List<string> _blockFiles;
    private readonly string _tempOutputFileName; 

    public FileTempStreams(string tempDir)
    {
      _tempDir =  tempDir;
      _blocksDir = Path.Combine(tempDir, "blocks");
      _tempOutputFileName = Path.Combine(tempDir, "output");
      _blockFiles = new List<string>();
    }

    public Stream CreateBlockStream(int blockIndex)
    {
      Directory.CreateDirectory(_blocksDir);
      string fn = Path.Combine(_blocksDir, GetBlockFileName(blockIndex));
      _blockFiles.Add(fn);
      FileStream fs = new FileStream(fn, FileMode.OpenOrCreate, FileAccess.ReadWrite);
      BufferedStream bs = new BufferedStream(fs);
      return bs;
    }

    public Stream CreateTempOutputStream()
    {
      Directory.CreateDirectory(_tempDir);
      FileStream fs = new FileStream(_tempOutputFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
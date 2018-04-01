using System;
using System.Collections.Generic;
using System.Text;

namespace Terrasoft
{
  /// <summary>
  /// Base class for derived which will represent different kind of tokens 
  /// (character, word, number, sentence etc).
  /// </summary>
  public abstract class Token : ICloneable
  {
    public long Position { get; set; }

    public object Clone()
    {
      return this.MemberwiseClone();
    }
  }
}
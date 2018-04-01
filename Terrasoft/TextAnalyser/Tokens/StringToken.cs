namespace Terrasoft
{
  /// <summary>
  /// Base class for tokens whose value is type of `string` 
  /// (unlike character token whose vaue type of `char`).
  /// </summary>
  public abstract class StringToken : Token
  {
    public string Value { get; set; }
  }
}
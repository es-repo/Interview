namespace Altium.BigSorter
{
  public interface IRecordEqualer
  {
    IRecordFieldEqualer CreateRecordFieldEqualer(int field);
  }

    public interface IRecordFieldEqualer
  {
    bool Equals(Record x, Record y);
  }
}
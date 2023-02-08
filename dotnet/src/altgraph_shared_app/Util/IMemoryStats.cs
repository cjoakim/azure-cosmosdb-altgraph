namespace altgraph_shared_app.Util
{
  public interface IMemoryStats
  {
    double Epoch { get; set; }
    // string Note { get; set; }
    double TotalMb { get; set; }
    double FreeMb { get; set; }
    double MaxMb { get; set; }
    double PctFree { get; set; }
    double PctMax { get; set; }
    double TotalPerDoc { get; set; }

    string AsDelimitedDataLine(string delim);
    string AsDelimitedHeaderLine(string delim);
    void Display();
  }
}
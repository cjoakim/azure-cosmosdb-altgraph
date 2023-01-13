namespace altgraph_web_app.Models
{
  public class GraphStatsStruct
  {
    public string Date { get; set; } = DateTime.Now.ToString();
    public int VertexCount { get; set; }
    public int EdgeCount { get; set; }
    public long ElapsedMs { get; set; }
    public long RefreshMs { get; set; }
    public DateTime? RefreshDate { get; set; } = null;
    public string RefreshSource { get; set; } = string.Empty;
    public double Epoch { get; set; }
    public double TotalMb { get; set; }
    public double FreeMb { get; set; }
    public double MaxMb { get; set; }
    public double PctFree { get; set; }
  }
}
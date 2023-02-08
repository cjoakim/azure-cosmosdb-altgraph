namespace altgraph_shared_app.Services.Graph.v2
{
  public class JRank
  {
    public string Key { get; set; } = string.Empty;
    public double Value { get; set; }

    public JRank(string key, double value)
    {
      Key = key;
      Value = value;
    }

    public int CompareTo(Object other)
    {
      JRank otherInstance = (JRank)other;

      if (Value > otherInstance.Value)
      {
        return -1;
      }
      else if (Value < otherInstance.Value)
      {
        return 1;
      }
      else
      {
        return 0;
      }
    }
  }
}